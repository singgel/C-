using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using System.Data.OleDb;
using FuelDataSysClient.Tool.Tool_Subaru;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_DBManager.Form_Subaru
{
    public partial class ImportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //全选
        private void barBtnSelectAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvCljbxx.FocusedRowHandle = 0;
            this.gvCljbxx.FocusedColumn = this.gvCljbxx.Columns[1];
            GridControlHelper.SelectItem(this.gvCljbxx, true);
        }

        //取消全选
        private void barBtnClearAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvCljbxx.FocusedRowHandle = 0;
            this.gvCljbxx.FocusedColumn = this.gvCljbxx.Columns[1];
            GridControlHelper.SelectItem(this.gvCljbxx, false);
        }

        //修改进口日期
        private void btnUpdateDate_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // 获取对象
                List<string> vinList = Utils.GetUpdateVin(this.gvCljbxx, (DataTable)this.gcCljbxx.DataSource);

                if (vinList == null || vinList.Count < 1)
                {
                    MessageBox.Show("请选择要修改的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (UpdateDateForm dateForm = new UpdateDateForm() { VinList = vinList })
                {
                    if (dateForm.ShowDialog() == DialogResult.OK)
                    {
                        this.searchLocal();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnLocalDel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.gcCljbxx == null || ((DataTable)this.gvCljbxx.DataSource).Rows.Count < 1)
            {
                MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dtSel = GridControlHelper.SelectedItems(this.gvCljbxx);
            if (dtSel.Rows.Count < 1)
            {
                MessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string vinStr = string.Join("','", dtSel.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray());
                AccessHelper.ExecuteNonQuery(AccessHelper.conn, string.Format("delete from FC_CLJBXX where VIN in ('{0}')", vinStr));
                AccessHelper.ExecuteNonQuery(AccessHelper.conn, string.Format("delete from RLLX_PARAM_ENTITY where VIN in ('{0}')", vinStr));
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
            searchLocal();
        }

        /// <summary>
        /// 手动导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnManualImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImportCsv importCsv = new ImportCsv();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.progressBarControl1.Position = 0;
                string msg = string.Empty;
                string file = string.Empty;
                importCsv.ProgressDoing += new ProgressEventHandler(operate_ProgressDoing);
                importCsv.ProgressCountDoing += new ProgressCountEventHandel(importCsv_ProgressCountDoing);
                Dictionary<string, Dictionary<string, string>> message = importCsv.ReadCsv(folderBrowserDialog1.SelectedPath);

                foreach (KeyValuePair<string, Dictionary<string, string>> item in message)
                {
                    file = item.Key + "\r\n";
                    string error = string.Empty;
                    foreach (KeyValuePair<string, string> kvp in item.Value)
                    {
                        error += String.Format("{0}\r\n{1}\r\n", kvp.Key, kvp.Value);
                    }
                    msg += file + error;
                }
                MessageForm mf = new MessageForm(msg + "\r\n导入完成");
                mf.Show();
                this.searchLocal();
            }
        }

        /// <summary>
        /// 批量查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImportCsv importCsv = new ImportCsv();
                DataSet ds = importCsv.ReadSharchExcel(openFileDialog1.FileName, "", ((int)Status.待上报).ToString());
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                gcCljbxx.DataSource = dt;
                this.lblSum.Text = String.Format("共{0}条", dt.Rows.Count);
                Utils.SelectItem(this.gvCljbxx, false);
            }
        }

        /// <summary>
        /// 批量修改进口日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnUpdateDate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImportCsv importCsv = new ImportCsv();
                if (importCsv.ReadUpdateDate(openFileDialog1.FileName, "") == 1)
                {

                    MessageBox.Show("批量修改时间成功");

                }
                else
                {
                    MessageBox.Show("批量修改时间失败");
                }
                this.searchLocal();
            }
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.searchLocal();
        }

        //清空查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVin.Text = string.Empty;
            this.tbClxh.Text = string.Empty;
            this.tbClzl.Text = string.Empty;
            this.cbRllx.Text = string.Empty;

            RepositoryItemDateEdit de = new RepositoryItemDateEdit();
            this.dtStartTime.EditValue = de.NullDate;
            this.dtEndTime.EditValue = de.NullDate;
        }

        private void searchLocal()
        {
            try
            {
                try
                {
                    if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                    {
                        MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                // 获取本地车辆基本信息
                string sql = @"SELECT * FROM FC_CLJBXX WHERE STATUS='1' ";
                string sw = "";
                if (!string.IsNullOrEmpty(tbVin.Text))
                {
                    sw += String.Format(" and (vin like '%{0}%')", tbVin.Text);
                }
                if (!string.IsNullOrEmpty(tbClxh.Text))
                {
                    sw += String.Format(" and (CLXH like '%{0}%')", tbClxh.Text);
                }
                if (!string.IsNullOrEmpty(tbClzl.Text))
                {
                    sw += String.Format(" and (CLZL like '%{0}%')", tbClzl.Text);
                }
                if (!string.IsNullOrEmpty(cbRllx.Text))
                {
                    sw += String.Format(" and (rllx like '%{0}%')", cbRllx.Text);
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text))
                {
                    sw += String.Format(" and (CREATETIME>=#{0}#)", Convert.ToDateTime(this.dtStartTime.Text));
                }
                if (!string.IsNullOrEmpty(this.dtEndTime.Text))
                {
                    sw += String.Format(" and (CREATETIME<=#{0}#)", Convert.ToDateTime(this.dtEndTime.Text));
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                this.gcCljbxx.DataSource = dt;
                this.gvCljbxx.BestFitColumns();
                this.lblSum.Text = String.Format("共{0}条", dt.Rows.Count);
                Utils.SelectItem(this.gvCljbxx, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        private void operate_ProgressDoing(object sender, ProgressEventArgs e)
        {
            if (e.Value <= e.Max)
            {
                if (e.Value == 0)
                {
                    this.progressBarControl1.Position = 0;

                }
                this.progressBarControl1.Properties.Maximum = e.Max;
                progressBarControl1.Properties.Step = 1;
                this.progressBarControl1.PerformStep();
            }
            Application.DoEvents();
        }

        private void importCsv_ProgressCountDoing(object sender, ProgressCountArgs e)
        {
            lblCount.Text = e.Modern + "/" + e.Count;
        }
    }
}