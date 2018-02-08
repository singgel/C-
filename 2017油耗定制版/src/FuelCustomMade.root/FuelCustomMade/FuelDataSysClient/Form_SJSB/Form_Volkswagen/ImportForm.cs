using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;
using System.Data.OleDb;
using FuelDataSysClient.Tool.Tool_Volkswagen;
using FuelDataSysClient.Tool;
using System.IO;

namespace FuelDataSysClient.Form_SJSB.Form_Volkswagen
{
    public partial class ImportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = DateTime.Now.ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            List<string> fuelTypeList = Utils.GetFuelType("SEARCH");
            this.cbRllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnSelectAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            Utils.SelectItem(this.gridView1, true);
        }

        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnClearAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            Utils.SelectItem(this.gridView1, false);
        }

        /// <summary>
        /// 修改进口日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateDate_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // 获取对象
                List<string> vinList = Utils.GetUpdateVin(this.gridView1, (DataTable)this.dgvCljbxx.DataSource);

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
            this.gridView1.PostEditor();
            DataView dv = (DataView)this.gridView1.DataSource;
            string selectedParamEntityIds = string.Empty;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if ((bool)dv.Table.Rows[i]["check"])
                    {
                        selectedParamEntityIds += String.Format(",'{0}'", dv.Table.Rows[i]["VIN"]);
                    }
                }
            }
            if (string.IsNullOrEmpty(selectedParamEntityIds))
            {
                MessageBox.Show("请选择要删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(AccessHelper.conn))
            {
                conn.Open();
                OleDbTransaction ts = conn.BeginTransaction();
                try
                {
                    selectedParamEntityIds = selectedParamEntityIds.Substring(1);
                    if (!string.IsNullOrEmpty(selectedParamEntityIds))
                    {
                        string sql = String.Format(@"delete * from FC_CLJBXX where vin in ({0})", selectedParamEntityIds);
                        string sqlentity = String.Format(@"delete * from RLLX_PARAM_ENTITY where vin in ({0})", selectedParamEntityIds);

                        int jbxxcount = AccessHelper.ExecuteNonQuery(ts, sql, null);
                        int paramcount = AccessHelper.ExecuteNonQuery(ts, sqlentity, null);

                        ts.Commit();
                    }
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                }
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
                string msg = string.Empty;
                string file = string.Empty;
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
                MessageForm mf = new MessageForm(msg);
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
                try
                {
                    ImportCsv importCsv = new ImportCsv();
                    DataSet ds = importCsv.ReadSharchExcel(openFileDialog1.FileName, ((int)Status.待上报).ToString());
                    DataTable dt = ds.Tables[0];
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dgvCljbxx.DataSource = dt;
                    this.lblSum.Text = String.Format("共{0}条", dt.Rows.Count);
                    Utils.SelectItem(this.gridView1, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 批量修改进口日期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnUpdateDate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ImportCsv importCsv = new ImportCsv();
                    List<string> fileNameList = new List<string>();
                    DirectoryInfo folder = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                    foreach (FileInfo file in folder.GetFiles("*.xls"))
                    {
                        fileNameList.Add(file.FullName);
                    }
                    if (fileNameList.Count > 0)
                    {
                        foreach (string fileName in fileNameList)
                        {
                            if (importCsv.ReadUpdateDate(fileName) == 1)
                            {
                                MessageBox.Show(fileName + "批量修改时间成功");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (!"".Equals(tbVin.Text))
                {
                    sw += String.Format(" and (vin like '%{0}%')", tbVin.Text);
                }
                if (!"".Equals(tbClxh.Text))
                {
                    sw += String.Format(" and (CLXH like '%{0}%')", tbClxh.Text);
                }
                if (!"".Equals(tbClzl.Text))
                {
                    sw += String.Format(" and (CLZL like '%{0}%')", tbClzl.Text);
                }
                if (!"".Equals(cbRllx.Text))
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
                dgvCljbxx.DataSource = dt;
                this.lblSum.Text = String.Format("共{0}条", dt.Rows.Count);
                Utils.SelectItem(this.gridView1, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }
    }
}