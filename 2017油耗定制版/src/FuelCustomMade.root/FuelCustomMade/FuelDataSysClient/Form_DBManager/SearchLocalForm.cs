using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using FuelDataModel;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using FuelDataSysClient.Tool;
using System.Threading;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_DBManager
{
    public partial class SearchLocalForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        MitsUtils mu;

        public SearchLocalForm()
        {
            InitializeComponent();
        }

        private void SearchLocalForm_Load(object sender, EventArgs e)
        {
            mu = new MitsUtils();
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        // 查看详细
        private void dgvCljbxx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)gcCljbxx.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null)
            {
                return;
            }
            string vin = (string)dr.Row.ItemArray[0];
            // 获取此VIN的详细信息，带入窗口
            string sql = @"select * from FC_CLJBXX where  vin = @vin";
            OleDbParameter[] param = {
                                     new OleDbParameter("@vin",vin)
                                     };
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, param);
            DataTable dt = ds.Tables[0];
            // 弹出详细信息窗口，可修改
            JbxxViewForm jvf = new JbxxViewForm();
            jvf.status = "1";
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn dc = dt.Columns[i];
                    Control[] c = jvf.Controls.Find("tb" + dc.ColumnName, true);
                    if (c.Length > 0)
                    {
                        if (c[0] is TextEdit)
                        {
                            c[0].Text = dt.Rows[0].ItemArray[i].ToString();
                            continue;
                        }
                        if (c[0] is DevExpress.XtraEditors.ComboBoxEdit )
                        {
                            DevExpress.XtraEditors.ComboBoxEdit  cb = c[0] as DevExpress.XtraEditors.ComboBoxEdit ;
                            cb.Text = dt.Rows[0].ItemArray[i].ToString();
                            if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                                || cb.Text == "双燃料" || cb.Text == "纯电动" || cb.Text == "非插电式混合动力" || cb.Text == "插电式混合动力" || cb.Text == "燃料电池")
                            {
                                string rlval = cb.Text;
                                if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                                || cb.Text == "双燃料")
                                {
                                    rlval = "传统能源";
                                }

                                // 构建燃料参数控件
                                jvf.getParamList(rlval, true);
                            }
                        }
                    }
                }
            }
            // 获取燃料信息
            string rlsql = @"select e.* from RLLX_PARAM_ENTITY e where e.vin = @vin";
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, rlsql, param);
            dt = ds.Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drrlxx = dt.Rows[i];
                string cName = drrlxx.ItemArray[1].ToString();
                Control[] c = jvf.Controls.Find(cName, true);
                 if (c.Length > 0)
                 {
                     if (c[0] is TextEdit)
                     {
                         c[0].Text = drrlxx.ItemArray[3].ToString();
                         continue;
                     }
                     if (c[0] is DevExpress.XtraEditors.ComboBoxEdit)
                     {
                         DevExpress.XtraEditors.ComboBoxEdit cb = c[0] as DevExpress.XtraEditors.ComboBoxEdit;
                         cb.Text = drrlxx.ItemArray[3].ToString();
                     }
                 }
            }

            (jvf.Controls.Find("tc", true)[0] as XtraTabControl).SelectedTabPageIndex = 0;
            jvf.MaximizeBox = false;
            jvf.MinimizeBox = false;
            Utils.SetFormMid(jvf);
            jvf.formClosingEventHandel += new FormClosingEventHandler(refrashBySubForm);
            jvf.ShowDialog();
        }

        void refrashBySubForm(object sender, FormClosingEventArgs args)
        {
            searchLocal();
        }

        // 全选
        private void barBtnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCljbxx.FocusedRowHandle = 0;
            this.gvCljbxx.FocusedColumn = this.gvCljbxx.Columns[1];
            GridControlHelper.SelectItem(this.gvCljbxx, true);
        }

        // 取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCljbxx.FocusedRowHandle = 0;
            this.gvCljbxx.FocusedColumn = this.gvCljbxx.Columns[1];
            GridControlHelper.SelectItem(this.gvCljbxx, false);
        }

        // 删除
        private void barBtnLocalDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        // 批量修改进口时间
        private void btnUpdateDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
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

        // 查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchLocal();
        }

        // 查询
        private void searchLocal()
        {
            try
            {
                // 验证查询时间：结束时间不能小于开始时间
                if (!this.VerifyStartEndTime())
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // 获取本地车辆基本信息
                string sql = string.Format(@"SELECT * FROM FC_CLJBXX WHERE STATUS='1' AND cdate(Format(UPLOADDEADLINE,'yyyy/mm/dd'))>#{0}#", DateTime.Today);
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
                sw += string.Format(@" AND cdate(Format(CLZZRQ,'yyyy/mm/dd')) >= #{0}# AND cdate(Format(CLZZRQ,'yyyy/mm/dd'))<= #{1}# ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                this.gcCljbxx.DataSource = dt;
                this.gvCljbxx.BestFitColumns();
                lblSum.Text = string.Format("共{0}条",dt.Rows.Count);
                Utils.SelectItem(this.gvCljbxx, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        ProcessForm pf;
        private void barShangbao_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            try
            {
                //获取上报数据
                List<string> vinList = new List<string>();
                this.gcCljbxx.DefaultView.PostEditor();
                DataTable dt = (DataTable)this.gcCljbxx.DataSource;
                if (dt != null && dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        bool result = false;
                        bool.TryParse(dt.Rows[i]["check"].ToString(), out result);
                        if (result)
                        {
                            string vin = String.Format("'{0}'", dt.Rows[i]["vin"]);
                            vinList.Add(vin);
                        }
                    }
                }
                if (vinList == null || vinList.Count < 1)
                {
                    MessageBox.Show("请选择要上报的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                pf = new ProcessForm();
                pf.Show();
                // 上报
                List<OperateResult> orList = ApplyParamMultiRows(vinList);
                // 获取上报结果
                List<string> vinsSucc = new List<string>();
                List<NameValuePair> vinsFail = new List<NameValuePair>();
                Dictionary<string, string> dSuccVinVid = new Dictionary<string, string>();
                Utils.getOperateResultListVins(orList, vinsSucc, vinsFail, dSuccVinVid);

                string strSucc = "";// "备案号（VIN）：返回码（VID）";
                // 修改本地状态为“0：已上报”
                if (vinsSucc.Count > 0)
                {
                    string strUpdate = "";
                    for (int i = 0; i < vinsSucc.Count; i++)
                    {
                        strUpdate += String.Format(",'{0}'", vinsSucc[i]);
                        strSucc += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：{1}, \r成功 \r\n", vinsSucc[i], dSuccVinVid[vinsSucc[i]]);
                    }
                    Utils.setVidStatusForUpload(dSuccVinVid);
                    // 刷新当前页面
                    this.searchLocal();
                }

                string strFail = "";
                if (vinsFail.Count > 0)
                {
                    for (int i = 0; i < vinsFail.Count; i++)
                    {
                        strFail += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：, \r失败: {1} \r\n", vinsFail[i].Name, vinsFail[i].Value);
                    }
                }
                string summary = string.Format("{0}条上传成功\r\n {1}条上传失败\r\n", vinsSucc.Count, vinsFail.Count);
                MessageForm mf = new MessageForm(String.Format("{0}{1} \n{2}", summary, strFail, strSucc));
                Utils.SetFormMid(mf);
                mf.Text = "上报结果";
                mf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (pf != null)
                {
                    pf.Close();
                }
            }
        }

        

        // 上报信息
        public List<OperateResult> ApplyParamMultiRows(List<string> vinList)
        {
            // 一次调用接口上传数据条数
            int pageSize = 50;
            
            // 返回结果
            FuelDataService.OperateResult result = null;
            List<FuelDataService.OperateResult> resSerList = new List<FuelDataService.OperateResult>();
            List<FuelDataModel.OperateResult> resCltList = new List<OperateResult>();

            // 分组上传时的VIN临时变量
            List<string> tempList = new List<string>();

            pf.TotalMax = (int)Math.Ceiling((decimal)vinList.Count / (decimal)pageSize);
            pf.ShowProcessBar();

            try
            {
                // 上报
                if (vinList.Count > 0)
                {
                    for (int i = 0; i < vinList.Count; )
                    {
                        // 最后一组上传不足50条
                        if (vinList.Count < pageSize)
                        {
                            pageSize = vinList.Count;
                        }
                        // 截取剩余记录中的pageSize条
                        var res = vinList.Take(pageSize);
                        tempList = (from string s in res select s).ToList<string>();
                        List<VehicleBasicInfo> vbiList = Utils.GetApplyParam(tempList);
                        // 上传
                        result = Utils.service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(vbiList).ToArray(), "CATARC_CUSTOM_2012");
                        resSerList.Add(result);
                        // 移除已上传的pageSize条记录
                        var leftRes = vinList.Skip(pageSize);
                        vinList = (from string s in leftRes select s).ToList<string>();
                        pf.progressBarControl1.PerformStep();
                        Application.DoEvents();
                    }
                }
                else
                {
                    return null;
                }
                // 将service端OperateResult转换为Client端
                foreach (FuelDataService.OperateResult res in resSerList)
                {
                    resCltList.Add(Utils.OperateResultS2C(res));
                }
                pf.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("上报过程发生异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return resCltList;
        }

        // 验证查询时间：结束时间不能小于开始时间
        protected bool VerifyStartEndTime()
        {
            string startTime = dtStartTime.Text;
            string endTime = dtEndTime.Text;

            try
            {
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime) && Convert.ToDateTime(startTime) > Convert.ToDateTime(endTime))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVin.Text = string.Empty;
            this.tbClxh.Text = string.Empty;
            this.tbClzl.Text = string.Empty;
            this.cbRllx.Text = string.Empty;
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        private void BtnImportData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FolderDialog openFolder = new FolderDialog();
            string strMsg = string.Empty;
            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = mu.GetFileName(openFolder.Path, mu.VinFileName);
                    if (fileNameList.Count > 0)
                    {
                        foreach (string str in fileNameList)
                        {
                            strMsg += mu.ImportVinData(str, openFolder.Path);
                        }
                    }
                    else
                    {
                        strMsg = string.Format("目录{0}下没有文件{1}", openFolder.Path, mu.VinFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                strMsg = String.Format("{0}\r\n{1}", ex.Message, strMsg);
            }
            MessageForm mf = new MessageForm("导入完成\r\n" + strMsg);
            mf.Show();
            searchLocal();
        }

        private void btnSynchronous_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable t = (DataTable)this.gcCljbxx.DataSource;
            if (t == null || t.Rows.Count == 0) { MessageBox.Show("请选择数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (mu.ActionUpdate(this.gvCljbxx, t))
            {
                MessageBox.Show("同步状态成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("同步状态失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.searchLocal();
        }
    }
}
