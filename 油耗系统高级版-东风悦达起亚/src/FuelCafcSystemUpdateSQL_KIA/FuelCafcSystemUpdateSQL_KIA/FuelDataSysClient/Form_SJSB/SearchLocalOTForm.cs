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
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using FuelDataModel;
using FuelDataSysClient.Tool;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using DevExpress.XtraPrinting;
using FuelDataSysClient.Model;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class SearchLocalOTForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SearchLocalOTForm()
        {
            InitializeComponent();
            // 设置燃料类型下拉框的值
            this.cbRLLX.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //比对数据窗体-处理比对结果使用
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
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Add("check", System.Type.GetType("System.Boolean"));
                gcCLJBXX.DataSource = ds.Tables[0];
            }
        }

        // 查看详细
        private void gvCLJBXX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)gcCLJBXX.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null)
            {
                return;
            }
            string vin = (string)dr.Row.ItemArray[0];
            // 获取此VIN的详细信息，带入窗口
            string sql = String.Format(@"select * from FC_CLJBXX where vin = '{0}'", vin);
            // 获取燃料信息
            string rlsql = String.Format(@"select e.* from RLLX_PARAM_ENTITY e where e.vin = '{0}'", vin);
            DataTable dtJbxx = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null).Tables[0];
            DataTable dtRlxx = OracleHelper.ExecuteDataSet(OracleHelper.conn, rlsql, null).Tables[0];
            // 弹出详细信息窗口，可修改
            JbxxViewForm jvf = new JbxxViewForm("UPLOADOT") { status = "1" };
            if (dtJbxx.Rows.Count > 0)
            {
                for (int i = 0; i < dtJbxx.Columns.Count; i++)
                {
                    DataColumn dc = dtJbxx.Columns[i];
                    Control[] c = jvf.Controls.Find("tb" + dc.ColumnName, true);
                    if (c.Length > 0)
                    {
                        if (c[0] is TextEdit)
                        {
                            c[0].Text = dtJbxx.Rows[0].ItemArray[i].ToString();
                            continue;
                        }
                        if (c[0] is ComboBoxEdit)
                        {
                            ComboBoxEdit cb = c[0] as ComboBoxEdit;
                            cb.Text = dtJbxx.Rows[0].ItemArray[i].ToString();
                            if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                                || cb.Text == "双燃料" || cb.Text == "纯电动" || cb.Text == "非插电式混合动力" || cb.Text == "插电式混合动力" || cb.Text == "燃料电池")
                            {
                                string rlval;
                                if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料" || cb.Text == "双燃料")
                                    rlval = "传统能源";
                                else
                                    rlval = cb.Text;
                                // 构建燃料参数控件
                                jvf.getParamList(rlval, true);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < dtRlxx.Rows.Count; i++)
            {
                DataRow drrlxx = dtRlxx.Rows[i];
                string cName = drrlxx.ItemArray[1].ToString();
                Control[] c = jvf.Controls.Find(cName, true);
                if (c.Length > 0)
                {
                    if (c[0] is TextEdit)
                    {
                        c[0].Text = drrlxx.ItemArray[3].ToString();
                        continue;
                    }
                    if (c[0] is ComboBoxEdit)
                    {
                        ComboBoxEdit cb = c[0] as ComboBoxEdit;
                        cb.Text = drrlxx.ItemArray[3].ToString();
                    }
                }
            }
            (jvf.Controls.Find("tc", true)[0] as XtraTabControl).SelectedTabPageIndex = 0;
            jvf.MaximizeBox = false;
            jvf.MinimizeBox = false;
            Utils.SetFormMid(jvf);
            jvf.ShowDialog();
            if (jvf.DialogResult == DialogResult.Cancel)
                this.refrashCurrentPage();
        }

        ProcessForm pf;
        //油耗补传
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
                this.gcCLJBXX.DefaultView.PostEditor();
                DataTable dt = (DataTable)this.gcCLJBXX.DataSource;
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
                ReasonForm rf = new ReasonForm();
                Utils.SetFormMid(rf);
                rf.Text = "补传原因";
                rf.ShowDialog();
                if (rf.DialogResult == DialogResult.OK)
                {
                    pf = new ProcessForm();
                    pf.Show();
                    // 补传
                    List<OperateResult> orList = applyOTParamMultiRows(vinList, rf.Reason);
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
                        //// 反馈码入库
                        Utils.setVidStatusForUpload(dSuccVinVid);
                        // 刷新当前页面
                        this.refrashCurrentPage();
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
            }
        }

        //补传信息
        protected List<OperateResult> applyOTParamMultiRows(List<string> vinList, string reason)
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
                        result = Utils.service.UploadOverTime(Utils.userId, Utils.password, Utils.FuelInfoC2S(vbiList).ToArray(), "CATARC_CUSTOM_2012");
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

        //同步状态
        private void btnSynchronous_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MitsUtils mitsUtil = new MitsUtils();
            DataTable t = (DataTable)this.gcCLJBXX.DataSource;
            if (t == null || t.Rows.Count == 0) { MessageBox.Show("请选择数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (mitsUtil.ActionUpdate(this.gvCLJBXX, t))
            {
                MessageBox.Show("同步状态成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("同步状态失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.refrashCurrentPage();
        }

        // 删除
        private void barBtnLocalDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCLJBXX.PostEditor();

            DataView dv = (DataView)this.gvCLJBXX.DataSource;
            string selectedParamEntityIds = "";
            if (dv != null && dv.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dv.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dv.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        selectedParamEntityIds += String.Format(",'{0}'", dv.Table.Rows[i]["VIN"]);
                    }
                }
            }
            if ("" == selectedParamEntityIds)
            {
                MessageBox.Show("请选择要删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            selectedParamEntityIds = selectedParamEntityIds.Substring(1);
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                OracleTransaction ts = conn.BeginTransaction();
                try
                {
                    if ("" != selectedParamEntityIds)
                    {
                        string sql = String.Format(@"delete from FC_CLJBXX where vin in ({0})", selectedParamEntityIds);
                        string sqlentity = String.Format(@"delete from RLLX_PARAM_ENTITY where vin in ({0})", selectedParamEntityIds);
                        OracleHelper.ExecuteNonQuery(ts, sql, null);
                        OracleHelper.ExecuteNonQuery(ts, sqlentity, null);
                        ts.Commit();
                    }
                }
                catch (Exception)
                {
                    ts.Rollback();
                }
            }
            this.refrashCurrentPage();
        }

        // 全选
        private void barBtnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCLJBXX.FocusedRowHandle = 0;
            this.gvCLJBXX.FocusedColumn = gvCLJBXX.Columns["SC_OCN"];
            Utils.SelectItem(this.gvCLJBXX, true);
        }

        // 取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCLJBXX.FocusedRowHandle = 0;
            this.gvCLJBXX.FocusedColumn = gvCLJBXX.Columns["SC_OCN"];
            Utils.SelectItem(this.gvCLJBXX, false);
        }

        //导出
        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                DataTable dtExport = (DataTable)gcCLJBXX.DataSource;
                if (dtExport == null)
                {
                    MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult dialogResult = saveFileDialog1.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    gcCLJBXX.ExportToXls(saveFileDialog1.FileName, options);
                    ExcelHelper excelBuilder = new ExcelHelper(saveFileDialog1.FileName);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.SaveFile();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchLocal(1);
        }

        //清除查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVIN.Text = string.Empty;
            this.tbSC_OCN.Text = string.Empty;
            this.tbCLXH.Text = string.Empty;
            this.tbCLZL.Text = string.Empty;
            this.cbRLLX.Text = string.Empty;
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //首页
        private void btnFirPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(1);
        }

        //上一页
        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(--pageNum);
        }

        //下一页
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(++pageNum);
        }

        //尾页
        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(pageCou);
        }

        //刷新
        private void refrashCurrentPage()
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 0) SearchLocal(pageNum);
        }

        //是否显示全部
        private void ceQueryAll_CheckedChanged_1(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }

        // 查询
        private void SearchLocal(int pageNum)
        {
            // 验证查询时间
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
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //获取总数目
                int dataCount = queryCount();
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    DataTable dt = queryByPage(pageNum);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    gcCLJBXX.DataSource = dt;
                    this.gvCLJBXX.BestFitColumns();
                    int pageSize = Convert.ToInt32(this.spanNumber.Text);
                    int pageCount = dataCount / pageSize;
                    if (dataCount % pageSize > 0) pageCount++;
                    int dataLast;
                    if (pageNum == pageCount)
                        dataLast = dataCount;
                    else
                        dataLast = pageSize * pageNum;
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", (pageSize * (pageNum - 1) + 1), dataLast);
                    this.txtPage.Text = String.Format("{0}/{1}", pageNum, pageCount);
                }
                else
                {
                    DataTable dt = queryAll();
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    gcCLJBXX.DataSource = dt;
                    this.gvCLJBXX.BestFitColumns();
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", 1, dataCount);
                    this.txtPage.Text = String.Format("{0}/{1}", 1, 1);
                }
                if (dataCount == 0)
                {
                    this.labPage.Text = "当前显示0至0条";
                    this.txtPage.Text = "0/0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //获取总数
        private int queryCount()
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(@"SELECT COUNT(*) FROM FC_CLJBXX WHERE STATUS='1' AND to_date(to_char(UPLOADDEADLINE,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<=to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", DateTime.Today);
            if (!string.IsNullOrEmpty(tbVIN.Text))
            {
                sql.AppendFormat(" AND (VIN like '%{0}%')", tbVIN.Text);
            }
            if (!string.IsNullOrEmpty(tbSC_OCN.Text))
            {
                sql.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSC_OCN.Text);
            }
            if (!string.IsNullOrEmpty(tbCLXH.Text))
            {
                sql.AppendFormat(" AND (CLXH like '%{0}%')", tbCLXH.Text);
            }
            if (!string.IsNullOrEmpty(tbCLZL.Text))
            {
                sql.AppendFormat(" AND (CLZL like '%{0}%')", tbCLZL.Text);
            }
            if (!string.IsNullOrEmpty(cbRLLX.Text))
            {
                sql.AppendFormat(" AND (RLLX like '%{0}%')", cbRLLX.Text);
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sql.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sql.ToString()));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(tbVIN.Text))
            {
                sqlWhere.AppendFormat(" AND (VIN like '%{0}%')", tbVIN.Text);
            }
            if (!string.IsNullOrEmpty(tbSC_OCN.Text))
            {
                sqlWhere.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSC_OCN.Text);
            }
            if (!string.IsNullOrEmpty(tbCLXH.Text))
            {
                sqlWhere.AppendFormat(" AND (CLXH like '%{0}%')", tbCLXH.Text);
            }
            if (!string.IsNullOrEmpty(tbCLZL.Text))
            {
                sqlWhere.AppendFormat(" AND (CLZL like '%{0}%')", tbCLZL.Text);
            }
            if (!string.IsNullOrEmpty(cbRLLX.Text))
            {
                sqlWhere.AppendFormat(" AND (RLLX like '%{0}%')", cbRLLX.Text);
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlWhere.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            string sqlVins = string.Format(@"select * from FC_CLJBXX where STATUS='1' AND to_date(to_char(UPLOADDEADLINE,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<=to_date('{0}','yyyy-mm-dd hh24:mi:ss') {1}", DateTime.Today, sqlWhere);
            string sqlStr = string.Format(@"select * from (select F.*,ROWNUM RN from ({0}) F where ROWNUM<={1}) where RN>{2}", sqlVins, pageSize * pageNum, pageSize * (pageNum - 1));
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        //获取全部数据
        private DataTable queryAll()
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat(@"SELECT * FROM FC_CLJBXX WHERE STATUS='1' AND to_date(to_char(UPLOADDEADLINE,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<=to_date('{0}','yyyy-mm-dd hh24:mi:ss') ",  DateTime.Today);
            if (!string.IsNullOrEmpty(tbVIN.Text))
            {
                sqlStr.AppendFormat(" AND (VIN like '%{0}%')", tbVIN.Text);
            }
            if (!string.IsNullOrEmpty(tbSC_OCN.Text))
            {
                sqlStr.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSC_OCN.Text);
            }
            if (!string.IsNullOrEmpty(tbCLXH.Text))
            {
                sqlStr.AppendFormat(" AND (CLXH like '%{0}%')", tbCLXH.Text);
            }
            if (!string.IsNullOrEmpty(tbCLZL.Text))
            {
                sqlStr.AppendFormat(" AND (CLZL like '%{0}%')", tbCLZL.Text);
            }
            if (!string.IsNullOrEmpty(cbRLLX.Text))
            {
                sqlStr.AppendFormat(" AND (RLLX like '%{0}%')", cbRLLX.Text);
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlStr.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr.ToString(), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }


  
    }
}
