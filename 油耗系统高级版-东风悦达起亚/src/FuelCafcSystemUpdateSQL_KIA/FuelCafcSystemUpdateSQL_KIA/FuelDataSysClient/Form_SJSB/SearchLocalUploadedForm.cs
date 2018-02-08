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
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Tool;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class SearchLocalUploadedForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SearchLocalUploadedForm()
        {
            InitializeComponent();
            this.cbRLLX.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            //初始时间和日期
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //查询满足VIN的基本信息表
        public void LocalData(DataView dv)
        {
            if (dv == null || dv.Table.Rows.Count <= 0) return;
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

        //双击行
        private void gvCLJBXX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //双击行默认选中
            ColumnView cv = (ColumnView)gcCLJBXX.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null)
            {
                return;
            }
            string vin = (string)dr.Row.ItemArray[0];
            ViewDetail(false,vin);
        }

        //查看详细
        private void ViewDetail(bool flag,string vin)
        {
            //ColumnView cv = (ColumnView)gcCLJBXX.FocusedView;
            //DataRowView dr = (DataRowView)cv.GetFocusedRow();
            //if (dr == null)
            //{
            //    return;
            //}
            //string vin = (string)dr.Row.ItemArray[0];
            DataTable dtBasic = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format(@"select * from FC_CLJBXX where vin = '{0}'", vin), null).Tables[0];
            DataTable dtParam = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format(@"select e.* from RLLX_PARAM_ENTITY e, RLLX_PARAM RPE where e.vin = '{0}' and e.PARAM_CODE=RPE.PARAM_CODE and RPE.STATUS='1'", vin), null).Tables[0];
            // 弹出详细信息窗口，可修改
            JbxxViewForm jvf = new JbxxViewForm(dtBasic, dtParam, true, flag);
            if (flag)
            {
                jvf.status = "1";
            }
            else
            {
                jvf.status = "2";
            }
            if (dtBasic.Rows.Count > 0)
            {
                for (int i = 0; i < dtBasic.Columns.Count; i++)
                {
                    DataColumn dc = dtBasic.Columns[i];
                    Control[] c = jvf.Controls.Find("tb" + dc.ColumnName, true);
                    if (c.Length > 0)
                    {
                        if (c[0] is TextEdit)
                        {
                            c[0].Text = dtBasic.Rows[0].ItemArray[i].ToString();
                            continue;
                        }
                        if (c[0] is DevExpress.XtraEditors.ComboBoxEdit)
                        {
                            DevExpress.XtraEditors.ComboBoxEdit cb = c[0] as DevExpress.XtraEditors.ComboBoxEdit;
                            cb.Text = dtBasic.Rows[0].ItemArray[i].ToString();
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
            for (int i = 0; i < dtParam.Rows.Count; i++)
            {
                DataRow drrlxx = dtParam.Rows[i];
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
            jvf.setVisible("btnbaocunshangbao", false);
            jvf.setVisible("btnCancel", true);
            jvf.setVisible("btnPrint", true);
            jvf.ShowDialog();
            if (jvf.DialogResult == DialogResult.Cancel)
                this.refrashCurrentPage();
        }

        //打印
        private void btnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.gvCLJBXX.PostEditor();
                List<PrintModel> printModelList = new List<PrintModel>();
                DataTable dt = (DataTable)gcCLJBXX.DataSource;
                if (dt != null)
                {
                    var dSelected = dt.Copy();
                    dSelected.Clear();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["check"].ToString() == "True")
                        {
                            dSelected.Rows.Add(dt.Rows[i].ItemArray);
                            PrintModel printModel = new PrintModel() { Qcscqy = dt.Rows[i]["QCSCQY"].ToString() == "" ? dt.Rows[i]["JKQCZJXS"].ToString() : dt.Rows[i]["QCSCQY"].ToString(), Clxh = dt.Rows[i]["CLXH"].ToString(), Zczbzl = dt.Rows[i]["ZCZBZL"].ToString(), Qdxs = dt.Rows[i]["QDXS"].ToString(), Zdsjzzl = dt.Rows[i]["ZDSJZZL"].ToString() };
                            string strRllx = dt.Rows[i]["RLLX"].ToString();
                            if (strRllx == "汽油" || strRllx == "柴油" || strRllx == "两用燃料" || strRllx == "双燃料")
                            {
                                try
                                {
                                    DataSet dsPam = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='{0}'", dt.Rows[i]["VIN"]), null);
                                    for (int j = 0; j < dsPam.Tables[0].Rows.Count; j++)
                                    {
                                        string strCode = dsPam.Tables[0].Rows[j]["PARAM_CODE"].ToString();
                                        string strValue = dsPam.Tables[0].Rows[j]["PARAM_VALUE"].ToString();
                                        if (strCode == "CT_FDJXH")
                                        {
                                            printModel.Fdjxh = strValue;
                                        }
                                        if (strCode == "CT_PL")
                                        {
                                            printModel.Pl = strValue;
                                        }
                                        if (strCode == "CT_BSQXS")
                                        {
                                            printModel.Bsqlx = strValue;
                                        }
                                        if (strCode == "CT_QTXX")
                                        {
                                            printModel.Qtxx = strValue;
                                        }
                                        if (strCode == "CT_EDGL")
                                        {
                                            printModel.Edgl = strValue;
                                        }
                                        if (strCode == "CT_SJGKRLXHL")
                                        {
                                            printModel.Sj = strValue;
                                        }
                                        if (strCode == "CT_SQGKRLXHL")
                                        {
                                            printModel.Sq = strValue;
                                        }
                                        if (strCode == "CT_ZHGKRLXHL")
                                        {
                                            printModel.Zh = strValue;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                };
                            }
                            printModel.Rllx = strRllx;
                            printModel.Bah = dt.Rows[i]["VIN"].ToString();
                            printModel.Qysj = DateTime.Now.ToShortDateString();
                            printModelList.Add(printModel);
                        }
                    }
                    if (dSelected.Rows.Count != 1)
                    {
                        MessageBox.Show(String.Format("每次只能操作一条记录，您选择了{0}条！", dSelected.Rows.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else {
                    MessageBox.Show(String.Format("没有可以操作的记录"), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Utils.printModel = printModelList;
                using (PrintForm pf = new PrintForm())
                {
                    pf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //修改
        private void barUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.gcCLJBXX.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gcCLJBXX.DefaultView.PostEditor();
            var dataSource = (DataTable)this.gcCLJBXX.DataSource;
            var dtSelected = dataSource.Copy();
            dtSelected.Clear();
            string vin = "";
            if (dataSource != null && dataSource.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Rows[i].ItemArray);
                        vin = (string)dataSource.Rows[i]["vin"];
                    }
                }
            }
            if (dtSelected.Rows.Count != 1)
            {
                MessageBox.Show(String.Format("每次只能操作一条记录，您选择了{0}条！", dtSelected.Rows.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ViewDetail(false,vin);
        }

        ProcessForm pf;
        //撤销
        private void barRepeal_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            try
            {
                //获取撤销的数据
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
                            string vin = (string)dt.Rows[i]["vin"];
                            vinList.Add(vin);
                        }
                    }
                }
                if (vinList == null || vinList.Count < 1)
                {
                    MessageBox.Show("请选择要撤销的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string delReason = string.Empty;
                ReasonForm rf = new ReasonForm();
                Utils.SetFormMid(rf);
                rf.Text = "撤销原因";
                rf.ShowDialog();
                if (rf.DialogResult == DialogResult.OK)
                {
                    pf = new ProcessForm();
                    pf.Show();
                    delReason = rf.Reason;
                    // 撤销
                    List<OperateResult> orList = ApplyParamMultiRows(vinList, delReason);
                    // 获取撤销结果
                    List<string> vinsSucc = new List<string>();
                    List<NameValuePair> vinsFail = new List<NameValuePair>();
                    Dictionary<string, string> dSuccVinVid = new Dictionary<string, string>();
                    Utils.getOperateResultListVins(orList, vinsSucc, vinsFail, dSuccVinVid);
                    string strSucc = "";// "备案号（VIN）：返回码（VID）";
                    // 修改本地状态为“3：已撤销”
                    if (vinsSucc.Count > 0)
                    {
                        string strUpdate = "";
                        for (int i = 0; i < vinsSucc.Count; i++)
                        {
                            strUpdate += String.Format(",'{0}'", vinsSucc[i]);
                            strSucc += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：{1}, \r成功 \r\n", vinsSucc[i], dSuccVinVid[vinsSucc[i]]);
                        }
                        // 删除已撤销数据
                        DelteLocalData(strUpdate.Substring(1));
                        // 刷新当前页面
                        this.refrashCurrentPage();
                    }
                    string strFail = "";
                    if (vinsFail.Count > 0)
                    {
                        for (int i = 0; i < vinsFail.Count; i++)
                        {
                            strFail += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：, \r失败:{1} \r\n", vinsFail[i].Name, vinsFail[i].Value);
                        }
                    }
                    string summary = string.Format("{0}条申请成功\r\n {1}条申请失败\r\n", vinsSucc.Count, vinsFail.Count);
                    MessageForm mf = new MessageForm(String.Format("{0}{1} \n{2}", summary, strSucc, strFail));
                    Utils.SetFormMid(mf);
                    mf.Text = "申请撤销结果";
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

        //撤销信息
        public List<OperateResult> ApplyParamMultiRows(List<string> vinList, string delReason)
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
                // 上报
                if (vinList.Count > 0)
                {
                    for (int i = 0; i < vinList.Count; )
                    {
                        // 最后一组上传不足pageSize条
                        if (vinList.Count < pageSize)
                        {
                            pageSize = vinList.Count;
                        }
                        // 截取剩余记录中的pageSize条
                        var res = vinList.Take(pageSize);
                        tempList = (from string s in res select s).ToList<string>();
                        // 上传
                        result = Utils.service.ApplyDelelte(Utils.userId, Utils.password, tempList.ToArray(), delReason, "CATARC_CUSTOM_2012");
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
                MessageBox.Show("申请撤销过程发生异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return resCltList;
        }

        //删除撤销数据
        private void DelteLocalData(string vin)
        {
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                OracleTransaction ts = conn.BeginTransaction();
                try
                {
                    string sql = String.Format(@" delete from FC_CLJBXX where vin in ({0}) ", vin);
                    string sqlentity = String.Format(@" delete from RLLX_PARAM_ENTITY where vin in ({0}) ", vin);
                    string sql_adc = String.Format(@" delete from FC_CLJBXX_ADC where vin in ({0}) ", vin);
                    string sqlentity_adc = String.Format(@" delete from RLLX_PARAM_ENTITY_ADC where vin in ({0}) ", vin);

                    OracleHelper.ExecuteNonQuery(ts, sql, null);
                    OracleHelper.ExecuteNonQuery(ts, sqlentity, null);
                    OracleHelper.ExecuteNonQuery(ts, sql_adc, null);
                    OracleHelper.ExecuteNonQuery(ts, sqlentity_adc, null);

                    ts.Commit();
                }
                catch (Exception ex)
                {
                    throw ex;
                    ts.Rollback();
                }
            }
        }

        //删除
        private void barBtnLocalDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCLJBXX.PostEditor();
            DataView dv = (DataView)this.gvCLJBXX.DataSource;
            string selectedParamEntityIds = "";
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
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
                        int jbxxcount = OracleHelper.ExecuteNonQuery(ts, sql, null);
                        int paramcount = OracleHelper.ExecuteNonQuery(ts, sqlentity, null);
                        ts.Commit();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    ts.Rollback();
                }
            }
            this.refrashCurrentPage();
        }

        //全选
        private void barSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCLJBXX.FocusedRowHandle = 0;
            this.gvCLJBXX.FocusedColumn = gvCLJBXX.Columns["SC_OCN"];
            Utils.SelectItem(this.gvCLJBXX, true);
        }

        //取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCLJBXX.FocusedRowHandle = 0;
            this.gvCLJBXX.FocusedColumn = gvCLJBXX.Columns["SC_OCN"];
            Utils.SelectItem(this.gvCLJBXX, false);
        }

        //导出
        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
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
                        sqlWhere.AppendFormat(" AND (RLLX = '{0}')", cbRLLX.Text);
                    }
                    if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
                    {
                        sqlWhere.AppendFormat(@" AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
                    }
                    if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("制造/进口日期"))
                    {
                        sqlWhere.AppendFormat(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
                    }
                    string sqlStrCTNY = string.Format(@"select V_ID,VIN,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,PFBZ,CT_ZHGKRLXHL,YHDYBAH,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,JYJGMC,JYBGBH,SC_OCN,XT_OCN,MI_XT_OCN,UPDATETIME from VIEW_T_ALL where 1=1 {0} ", sqlWhere);
                    string sqlStrFCDS = string.Format(@"select V_ID,VIN,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,PFBZ,FCDS_HHDL_ZHGKRLXHL,YHDYBAH,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,JYJGMC,JYBGBH,SC_OCN,XT_OCN,MI_XT_OCN,UPDATETIME from VIEW_T_ALL_FCDS where 1=1 {0} ", sqlWhere);
                    string sqlStrCDS = string.Format(@"select V_ID,VIN,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,PFBZ,CDS_HHDL_ZHGKRLXHL,YHDYBAH,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,JYJGMC,JYBGBH,SC_OCN,XT_OCN,MI_XT_OCN,UPDATETIME from VIEW_T_ALL_CDS where 1=1 {0} ", sqlWhere);
                    string sqlStrCDD = string.Format(@"select V_ID,VIN,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,PFBZ,YHDYBAH,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,JYJGMC,JYBGBH,SC_OCN,XT_OCN,MI_XT_OCN,UPDATETIME from VIEW_T_ALL_CDD where 1=1 {0} ", sqlWhere);
                    string sqlStrRLDC = string.Format(@"select V_ID,VIN,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,PFBZ,RLDC_ZHGKHQL,YHDYBAH,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,JYJGMC,JYBGBH,SC_OCN,XT_OCN,MI_XT_OCN,UPDATETIME from VIEW_T_ALL_RLDC where 1=1 {0} ", sqlWhere);
                    DataSet dsCTNY = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrCTNY, null);
                    DataSet dsFCDS = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrFCDS, null);
                    DataSet dsCDS = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrCDS, null);
                    DataSet dsCDD = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrCDD, null);
                    DataSet dsRLDC = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrRLDC, null);
                    DataSet dsExport = new DataSet();
                    if (dsCTNY != null && dsCTNY.Tables.Count > 0 && dsCTNY.Tables[0] != null && dsCTNY.Tables[0].Rows.Count > 0)
                    {
                        dsCTNY.Tables[0].TableName = "传统能源已上传数据";
                        dsExport.Tables.Add(dsCTNY.Tables[0].Copy());
                    }
                    if (dsFCDS != null && dsFCDS.Tables.Count > 0 && dsFCDS.Tables[0] != null && dsFCDS.Tables[0].Rows.Count > 0)
                    {
                        dsFCDS.Tables[0].TableName = "非插电式混合动力已上传数据";
                        dsExport.Tables.Add(dsFCDS.Tables[0].Copy());
                    }
                    if (dsCDS != null && dsCDS.Tables.Count > 0 && dsCDS.Tables[0] != null && dsCDS.Tables[0].Rows.Count > 0)
                    {
                        dsCDS.Tables[0].TableName = "插电式混合动力已上传数据";
                        dsExport.Tables.Add(dsCDS.Tables[0].Copy());
                    }
                    if (dsCDD != null && dsCDD.Tables.Count > 0 && dsCDD.Tables[0] != null && dsCDD.Tables[0].Rows.Count > 0)
                    {
                        dsCDD.Tables[0].TableName = "纯电动已上传数据";
                        dsExport.Tables.Add(dsCDD.Tables[0].Copy());
                    }
                    if (dsRLDC != null && dsRLDC.Tables.Count > 0 && dsRLDC.Tables[0] != null && dsRLDC.Tables[0].Rows.Count > 0)
                    {
                        dsRLDC.Tables[0].TableName = "燃料电池已上传数据";
                        dsExport.Tables.Add(dsRLDC.Tables[0].Copy());
                    }
                    if (dsExport.Tables.Count < 1)
                    {
                        MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    ExportToExcel toExcel = new ExportToExcel();
                    for (int i = 0; i < dsExport.Tables.Count; i++)
                    {
                        toExcel.ExportExcel(folderBrowserDialog1.SelectedPath, dsExport.Tables[i]);
                    }
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchLocal(1);
        }

        //清空查询条件
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
        private void ceQueryAll_CheckedChanged(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }

        // 查询
        private void SearchLocal(int pageNum)
        {
            // 验证查询时间：结束时间不能小于开始时间
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && Convert.ToDateTime(this.dtStartTime.Text) > Convert.ToDateTime(this.dtEndTime.Text))
            {
                MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("select count(*) from FC_CLJBXX where STATUS='0' ");
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
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
            {
                sqlStr.AppendFormat(@" AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("制造/进口日期"))
            {
                sqlStr.AppendFormat(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
            }
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sqlStr.ToString()));
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
                sqlWhere.AppendFormat(" AND (RLLX = '{0}')", cbRLLX.Text);
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
            {
                sqlWhere.AppendFormat(@" AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("制造/进口日期"))
            {
                sqlWhere.AppendFormat(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
            }
            string sqlVins = string.Format(@"select * from FC_CLJBXX where STATUS='0' {0}", sqlWhere);
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
            sqlStr.Append("select * from FC_CLJBXX where STATUS='0' ");
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
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
            {
                sqlStr.AppendFormat(@" AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("制造/进口日期"))
            {
                sqlStr.AppendFormat(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
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
