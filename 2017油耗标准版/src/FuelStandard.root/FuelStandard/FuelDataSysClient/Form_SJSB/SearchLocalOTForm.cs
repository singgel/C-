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

namespace FuelDataSysClient
{
    public partial class SearchLocalOTForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        FuelDataOpen.FuelDataOpen serverOpen = Utils.serviceOpen;
        InitDataTime initTime = new InitDataTime();
        MitsUtils mitsUtil;
        public SearchLocalOTForm()
        {
            InitializeComponent();

            // 设置燃料类型下拉框的值
            this.SetFuelType();
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        // 查看详细
        private void dgvCljbxx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)dgvCljbxx.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            
            if (dr == null)
            {
                return;
            }
            string vin = (string)dr.Row.ItemArray[0];
            
            // 获取此VIN的详细信息，带入窗口
            string sql = @"select * from FC_CLJBXX where vin = @vin";
            OleDbParameter[] param = {
                                     new OleDbParameter("@vin",vin)
                                     };
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, param);
            DataTable dt = ds.Tables[0];

            // 弹出详细信息窗口，可修改
            JbxxViewForm jvf = new JbxxViewForm("UPLOADOT");
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
                                || cb.Text == "双燃料" || cb.Text == "气体燃料" || cb.Text == "纯电动" || cb.Text == "非插电式混合动力" || cb.Text == "插电式混合动力" || cb.Text == "燃料电池")
                            {
                                string rlval = cb.Text;
                                if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                                || cb.Text == "双燃料" || cb.Text == "气体燃料")
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

        // 导入Excel批量修改通关日期
        private void btnCmdUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
           
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (mitsUtil.ReadUpdateDate(openFileDialog1.FileName, "") == 1)
                    {
                        MessageBox.Show("批量修改时间成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("批量修改时间失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    this.searchLocal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("批量修改时间失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 导入Excel批量查询
        private void btnCmdSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    DataSet ds = mitsUtil.ReadSearchExcel(openFileDialog1.FileName, "", ((int)Status.待上报).ToString(), string.Format(" AND UPLOADDEADLINE<=#{0}#", DateTime.Today));
                    DataTable dt = ds.Tables[0];
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dgvCljbxx.DataSource = dt;

                    lblSum.Text = string.Format("共{0}条", dt.Rows.Count);
                    Utils.SelectItem(this.gridView1, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("批量查询失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 全选
        private void barBtnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectItem(this.gridView1, true);
        }

        // 取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectItem(this.gridView1, false);
        }

        // 删除
        private void barBtnLocalDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.PostEditor();

            DataView dv = (DataView)this.gridView1.DataSource;
            string selectedParamEntityIds = "";
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if ((bool)dv.Table.Rows[i]["check"])
                    {
                        selectedParamEntityIds += ",'"+dv.Table.Rows[i]["VIN"]+"'";
                    }
                }
            }
            if("" == selectedParamEntityIds)
            {
                MessageBox.Show("请选择要删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            selectedParamEntityIds = selectedParamEntityIds.Substring(1);

            OleDbConnection conn = new OleDbConnection(AccessHelper.conn);
            conn.Open();
            OleDbTransaction ts = conn.BeginTransaction();
            try
            {
                if ("" != selectedParamEntityIds)
                {
                    string sql = @"delete * from FC_CLJBXX where vin in (" + selectedParamEntityIds + ")";
                    string sqlentity = @"delete * from RLLX_PARAM_ENTITY where vin in (" + selectedParamEntityIds + ")";

                    int jbxxcount = AccessHelper.ExecuteNonQuery(ts, sql, null);
                    int paramcount = AccessHelper.ExecuteNonQuery(ts, sqlentity, null);

                    ts.Commit();
                }
            }
            catch (Exception)
            {
                ts.Rollback();
            }
            finally
            {
                conn.Close();
            }
            searchLocal();
        }

        public void LocalData(DataView dv)
        {
            if (dv == null) return;
            StringBuilder strAdd = new StringBuilder();
            strAdd.Append("select * from FC_CLJBXX where VIN in(");
            foreach (DataRow r in dv.Table.Rows)
            {
                strAdd.Append("'");
                strAdd.Append(Convert.ToString(r["VIN"]));
                strAdd.Append("',");
            }
            string sql = strAdd.ToString().TrimEnd(',') + ")";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Add("check", System.Type.GetType("System.Boolean"));
                dgvCljbxx.DataSource = ds.Tables[0];
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
            // 验证查询时间：结束时间不能小于开始时间
            if (!this.VerifyStartEndTime())
            {
                MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 获取本地车辆基本信息
            string sql = string.Format(@"SELECT * FROM FC_CLJBXX WHERE STATUS='1' AND cdate(Format(UPLOADDEADLINE,'yyyy/mm/dd'))<=#{0}#", DateTime.Today);
            string sw = "";
            if (!"".Equals(tbVin.Text))
            {
                sw += " and (vin like '%" + tbVin.Text + "%')";
            }
            if (!"".Equals(tbClxh.Text))
            {
                sw += " and (CLXH like '%" + tbClxh.Text + "%')";
            }
            if (!"".Equals(tbClzl.Text))
            {
                sw += " and (CLZL like '%" + tbClzl.Text + "%')";
            }
            if (!"".Equals(cbRllx.Text))
            {
                sw += " and (rllx like '%" + cbRllx.Text + "%')";
            }
            sw += string.Format(@" AND cdate(Format(CLZZRQ,'yyyy/mm/dd')) >= #{0}# AND cdate(Format(CLZZRQ,'yyyy/mm/dd'))<= #{1}# ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));

            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
            DataTable dt = ds.Tables[0];
            dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
            dgvCljbxx.DataSource = dt;

            lblSum.Text = string.Format("共{0}条", dt.Rows.Count);
            Utils.SelectItem(this.gridView1, false);
        }

        /// <summary>
        /// 批量修改进口时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                UpdateDateForm dateForm = new UpdateDateForm();
                dateForm.VinList = vinList;

                if (dateForm.ShowDialog() == DialogResult.OK)
                {
                    this.searchLocal();
                }
            }
            catch (Exception)
            {
            }
        }

        ProcessForm pf;
        private void btnUploadOT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            try
            {
                //获取补传的数据
                List<string> vinList = new List<string>();
                this.dgvCljbxx.DefaultView.PostEditor();
                DataTable dt = (DataTable)this.dgvCljbxx.DataSource;
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
                    MessageBox.Show("请选择要补传的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string delReason = string.Empty;
                ReasonForm rf = new ReasonForm();
                Utils.SetFormMid(rf);
                rf.Text = "补传原因";
                rf.ShowDialog();

                //Utils.Open("INSERT_O");

                if (rf.DialogResult == DialogResult.OK)
                {
                    pf = new ProcessForm();
                    pf.Show();

                    // 补传
                    List<OperateResult> orList = this.applyOTParamMultiRows(vinList, rf.Reason);

                    // 获取补传结果
                    List<string> vinsSucc = new List<string>();
                    List<NameValuePair> vinsFail = new List<NameValuePair>();
                    Dictionary<string, string> dSuccVinVid = new Dictionary<string, string>();
                    Utils.getOperateResultListVins(orList, vinsSucc, vinsFail, dSuccVinVid);

                    string strSucc = "";// "备案号（VIN）：返回码（VID）";
                    // 修改本地状态为“4：已申请补传”
                    if (vinsSucc.Count > 0)
                    {
                        string strUpdate = "";
                        for (int i = 0; i < vinsSucc.Count; i++)
                        {
                            strUpdate += String.Format(",'{0}'", vinsSucc[i]);
                            strSucc += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：{1}, \r成功 \r\n", vinsSucc[i], dSuccVinVid[vinsSucc[i]]);
                        }
                        //Utils.setStatusForUpload(strUpdate.Substring(1), "0");
                        //// 反馈码入库
                        //Utils.setVidForUpload(dSuccVinVid);

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
                    string summary = string.Format("{0}条补传成功\r\n {1}条补传失败\r\n", vinsSucc.Count, vinsFail.Count);
                    MessageForm mf = new MessageForm(String.Format("{0}{1} \n{2}", summary, strSucc, strFail));
                    Utils.SetFormMid(mf);
                    mf.Text = "补传结果";
                    mf.ShowDialog();
                }
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
                //关端口
                //Utils.Close("INSERT_O");
            }
        }



        // 补传信息
        protected List<OperateResult> applyOTParamMultiRows(List<string> vinList,string reason)
        {
            // 一次最多上传50条
            int pageSize = 50;
            // 返回结果
            FuelDataService.OperateResult result = null;
            List<FuelDataService.OperateResult> resSerList = new List<FuelDataService.OperateResult>();
            List<FuelDataModel.OperateResult> resCltList = new List<OperateResult>();

            // 分组上传时的临时变量
            List<string> tempList = new List<string>();

            pf.TotalMax = (int)Math.Ceiling((decimal)vinList.Count / (decimal)pageSize);
            pf.ShowProcessBar();

            try
            {
                // 补传
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
                        foreach (VehicleBasicInfo vInfo in vbiList)
                        {
                            vInfo.Reason = reason;
                        }

                        // 补传
                        result = service.UploadOverTime(Utils.userId, Utils.password, Utils.FuelInfoC2S(vbiList).ToArray(), "CATARC_CUSTOM_2012");
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

                foreach (FuelDataService.OperateResult res in resSerList)
                {
                    resCltList.Add(Utils.OperateResultS2C(res));
                }
                pf.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("补传过程发生异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // 设置燃料类型下拉框的值
        protected void SetFuelType()
        {
            List<string> fuelTypeList = Utils.GetFuelType("SEARCH");
            this.cbRllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }
        
        // 清空查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVin.Text = string.Empty;
            this.tbClxh.Text = string.Empty;
            this.tbClzl.Text = string.Empty;
            this.cbRllx.Text = string.Empty;
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        private void btnSynchronous_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DataTable t = (DataTable)this.dgvCljbxx.DataSource;
            if (t == null || t.Rows.Count == 0) { MessageBox.Show("请选择数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (mitsUtil.ActionUpdate(this.gridView1,t))
            {
                MessageBox.Show("同步状态成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("同步状态失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.searchLocal();
        }

        private void SearchLocalOTForm_Load(object sender, EventArgs e)
        {
            mitsUtil = new MitsUtils();
        }
    }
}
