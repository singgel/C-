using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataModel;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Model;
using Oracle.ManagedDataAccess.Client;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class cljbxxForm : Form
    {
        public static string strVin;

        public cljbxxForm()
        {
            List<string> fuelTypeList = Utils.GetFuelType("");
            InitializeComponent();
            this.panelControl.Location = new Point(0, 0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            this.btnPrint.Visible = false;
            this.cbrllx.Text = fuelTypeList[0];
            this.cbIsJkqc.Text = "否";
            this.tbqcscqy.Text = Utils.qymc;
            this.tc.SelectedTabPage = this.tp1;
            this.dtclzzrq.EditValue = DateTime.Today;
            string fuelTypeName = this.cbrllx.Text.Trim();
            if (fuelTypeName == "汽油" || fuelTypeName == "柴油" || fuelTypeName == "两用燃料" || fuelTypeName == "双燃料")
            {
                getParamList("传统能源");
            }
            else
            {
                getParamList(fuelTypeName);
            }
            this.cbrllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        private void cbrllx_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.tc.SelectedTabPage = this.tp2;
                string strType = this.cbrllx.SelectedItem.ToString().Trim();
                if (strType != "" && strType != null)
                {
                    if (strType == "汽油" || strType == "柴油" || strType == "两用燃料" || strType == "双燃料")
                    {
                        getParamList("传统能源");
                    }
                    else
                    {
                        getParamList(strType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cbIsJkqc_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbIsJkqc.SelectedItem.ToString() == "是")
            {
                tbqcscqy.Text = "";
                tbjkqczjxs.Text = Utils.qymc;
            }
            else
            {
                tbjkqczjxs.Text = "";
                tbqcscqy.Text = Utils.qymc;
            }
        }

        public void getParamList(string strType)
        {
            // 先清空，再添加
            this.tlp.Controls.Clear();
            this.tlp.Location = new Point(10, 30);
            DataTable dt = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, FUEL_TYPE, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE FROM RLLX_PARAM WHERE   (FUEL_TYPE = '{0}' AND STATUS = '1') ORDER BY ORDER_RULE", strType), null).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                // textbox类型
                if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                {
                    Label lbl = new Label() { Width = 160, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    TextEdit tb = new TextEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                    Label lbll = new Label() { Width = 100, Height = 30, Text = dr["PARAM_REMARK"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(tb);
                    this.tlp.Controls.Add(lbll);
                }
                // OPTION类型
                if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                {
                    Label lbl = new Label() { Width = 160, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString() };
                    cbe.Properties.Items.AddRange(dr["CONTROL_VALUE"].ToString().Split('/'));
                    cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                    Label lbll = new Label() { Width = 100, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(cbe);
                    this.tlp.Controls.Add(lbll);
                }

                tlp.Location = new Point(0, 15);
            }

        }

        private void btnbaocun_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            msg += this.VerifyRequired();
            msg += Utils.VerifyRLParam(this.tlp.Controls);
            try
            {
                if (!this.dxErrorProvider.HasErrors && string.IsNullOrEmpty(msg))
                {
                    saveParam();
                    this.btnbaocunshangbao.Text = "上报";
                    MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请核对页面信息是否填写正确！" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnbaocunshangbao_Click(object sender, EventArgs e)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            string msg = string.Empty;
            msg += this.VerifyRequired();
            msg += Utils.VerifyRLParam(this.tlp.Controls);
            try
            {
                if (!this.dxErrorProvider.HasErrors && string.IsNullOrEmpty(msg))
                {
                    saveParam();
                    FuelDataService.OperateResult result = applyParam();
                    int count = Utils.ApplyFlg(result).Count;
                    if (count > 0)
                    {
                        this.btnPrint.Visible = false;

                        OperateResult oResult = Utils.OperateResultS2C(result);
                        NameValuePair nvp = oResult.ResultDetail[0] as NameValuePair;
                        MessageBox.Show(String.Format("【备案号（VIN）：{0}\n信息：{1}】", nvp.Name, nvp.Value), "上报失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        Utils.UpdataState(result);
                        setEnable();
                        this.btnPrint.Visible = true;
                        this.btnbaocunshangbao.Enabled = false;
                        this.btnbaocun.Enabled = false;
                        string strVID = OracleHelper.ExecuteScalar(OracleHelper.conn, String.Format("SELECT V_ID FROM FC_CLJBXX WHERE VIN ='{0}'", this.tbbah.Text.Trim().ToUpper())).ToString();
                        if (strVID != null)
                        {
                            MessageBox.Show(String.Format("备案号（VIN）：{0}\n反馈码（VID）：{1}", this.tbbah.Text.Trim().ToUpper(), strVID), "上报成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请核对页面信息是否填写正确！" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 保存
        private void saveParam()
        {
           using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
                try
                {
                    string strBah = this.tbbah.Text.Trim().ToUpper();
                    OracleHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM FC_CLJBXX WHERE VIN = '{0}' AND STATUS='1'", strBah), null);
                    OracleHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", strBah), null);

                    #region 遍历参数,保存
                    foreach (Control c in this.tlp.Controls)
                    {
                        if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                        {
                            string paramCode = c.Name;
                            string paramValue = c.Text;
                            OracleParameter[] paramList = { 
                                    new OracleParameter("PARAM_CODE",paramCode),
                                    new OracleParameter("VIN",strBah),
                                    new OracleParameter("PARAM_VALUE",paramValue),
                                    new OracleParameter("V_ID","")
                                };
                            OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                VALUES(:PARAM_CODE,:VIN,:PARAM_VALUE,:V_ID)", paramList);
                        }

                    }
                    #endregion

                    #region 保存车辆基本信息
                    // 保存车辆基本信息
                    DateTime clzzrqDate = DateTime.Parse(this.dtclzzrq.Text.Trim());
                    OracleParameter clzzrq = new OracleParameter("CLZZRQ", clzzrqDate) { DbType = DbType.Date };
                    DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                    OracleParameter uploadDeadline = new OracleParameter("UPLOADDEADLINE", uploadDeadlineDate) { DbType = DbType.Date };
                    OracleParameter creTime = new OracleParameter("CREATETIME", DateTime.Now) { DbType = DbType.Date };
                    OracleParameter upTime = new OracleParameter("UPDATETIME", DateTime.Now) { DbType = DbType.Date };
                    OracleParameter[] param = { 
                                    new OracleParameter("VIN",this.tbbah.Text.Trim().ToUpper()),
                                    new OracleParameter("HGSPBM",this.tbHgspbm.Text.Trim().ToUpper()),
                                    new OracleParameter("USER_ID",Utils.localUserId),
                                    new OracleParameter("QCSCQY",this.tbqcscqy.Text.Trim()),
                                    new OracleParameter("JKQCZJXS",this.tbjkqczjxs.Text.Trim()),
                                    new OracleParameter("CLXH",this.tbclxh.Text.Trim()),
                                    new OracleParameter("CLZL",this.cbclzl.Text.Trim()),
                                    new OracleParameter("RLLX",this.cbrllx.Text.Trim()),
                                    new OracleParameter("ZCZBZL",this.tbzczbzl.Text.Trim()),
                                    new OracleParameter("ZGCS",this.tbzgcs.Text.Trim()),
                                    new OracleParameter("LTGG",this.tbltgg.Text.Trim()),
                                    new OracleParameter("ZJ",this.tbzj.Text.Trim()),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("TYMC",this.tbtymc.Text.Trim()),
                                    new OracleParameter("YYC",this.cbyyc.Text.Trim()),
                                    new OracleParameter("ZWPS",this.tbzwps.Text.Trim()),
                                    new OracleParameter("ZDSJZZL",this.tbzdsjzzl.Text.Trim()),
                                    new OracleParameter("EDZK",this.tbedzk.Text.Trim()),
                                    new OracleParameter("LJ",this.tblj.Text.Trim()),
                                    new OracleParameter("QDXS",this.cbqdxs.Text.Trim()),
                                    new OracleParameter("STATUS","1"),
                                    new OracleParameter("JYJGMC",this.tbjcjgmc.Text.Trim()),
                                    new OracleParameter("JYBGBH",this.tbbgbh.Text.Trim()),
                                    new OracleParameter("QTXX",this.tbQtxx.Text.Trim()),
                                    creTime,
                                    upTime,
                                    };

                    OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX
                            (   VIN,
                                HGSPBM,
                                USER_ID,
                                QCSCQY,
                                JKQCZJXS,
                                CLXH,
                                CLZL,
                                RLLX,
                                ZCZBZL,
                                ZGCS,
                                LTGG,
                                ZJ,
                                CLZZRQ,
                                UPLOADDEADLINE,
                                TYMC,
                                YYC,
                                ZWPS,
                                ZDSJZZL,
                                EDZK,
                                LJ,
                                QDXS,
                                STATUS,
                                JYJGMC,
                                JYBGBH,
                                QTXX,
                                CREATETIME,
                                UPDATETIME
                            ) VALUES
                            (   :VIN,
                                :HGSPBM,
                                :USER_ID,
                                :QCSCQY,
                                :JKQCZJXS,
                                :CLXH,
                                :CLZL,
                                :RLLX,
                                :ZCZBZL,
                                :ZGCS,
                                :LTGG,
                                :ZJ,
                                :CLZZRQ,
                                :UPLOADDEADLINE,
                                :TYMC,
                                :YYC,
                                :ZWPS,
                                :ZDSJZZL,
                                :EDZK,
                                :LJ,
                                :QDXS,
                                :STATUS,
                                :JYJGMC,
                                :JYBGBH,
                                :QTXX,
                                :CREATETIME,
                                :UPDATETIME)", param);
                    tra.Commit();
                    strVin = strBah; //备案号
                    #endregion
                }
                catch (Exception ex)
                {
                    tra.Rollback();
                    throw ex;
                }
            }
        }

        // 上报
        private FuelDataService.OperateResult applyParam()
        {
            try
            {
                List<VehicleBasicInfo> lbiList = new List<VehicleBasicInfo>();
                VehicleBasicInfo lbi = new VehicleBasicInfo() { V_Id = "", User_Id = Utils.userId, Qcscqy = tbqcscqy.Text.Trim(), Jkqczjxs = this.tbjkqczjxs.Text.Trim(), Vin = this.tbbah.Text.Trim().ToUpper(), Hgspbm = this.tbHgspbm.Text.Trim().ToUpper(), Clxh = this.tbclxh.Text.Trim(), Clzl = this.cbclzl.Text.Trim(), Rllx = this.cbrllx.Text.Trim(), Zczbzl = this.tbzczbzl.Text.Trim(), Zgcs = this.tbzgcs.Text.Trim(), Ltgg = this.tbltgg.Text.Trim(), Zj = this.tbzj.Text.Trim(), Clzzrq = DateTime.Parse(this.dtclzzrq.Text), Tymc = this.tbtymc.Text.Trim(), Yyc = this.cbyyc.Text.Trim(), Zwps = this.tbzwps.Text.Trim(), Zdsjzzl = this.tbzdsjzzl.Text.Trim(), Edzk = this.tbedzk.Text.Trim(), Lj = this.tblj.Text.Trim(), Qdxs = this.cbqdxs.Text.Trim(), Jyjgmc = this.tbjcjgmc.Text.Trim(), Jybgbh = this.tbbgbh.Text.Trim(), Qtxx = this.tbQtxx.Text.Trim(), CreateTime = DateTime.Now, UpdateTime = DateTime.Now, Status = "0" };
                List<RllxParamEntity> listParam = new List<RllxParamEntity>();
                foreach (Control c in this.tlp.Controls)
                {
                    if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        RllxParamEntity pe = new RllxParamEntity() { V_Id = "", Param_Code = c.Name, Vin = this.tbbah.Text.Trim().ToUpper(), Param_Value = c.Text };
                        listParam.Add(pe);
                    }
                }
                lbi.EntityList = listParam.ToArray();
                lbiList.Add(lbi);
                // 上报
                FuelDataService.OperateResult result = new FuelDataService.OperateResult();
                return result = Utils.service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 设置不可编辑
        private void setEnable()
        {
            foreach (Control c in this.tlp.Controls)
            {
                if (c is TextEdit || c is DevExpress.XtraEditors.TextEdit)
                {
                    c.Enabled = false;
                }
            }
            foreach (Control c in this.paneljiben.Controls)
            {
                if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                {
                    c.Enabled = false;
                }
            }
        }

        private void btnCon_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Utils.printModel = GetPrintData(this.tbbah.Text.Trim().ToUpper());
            using (PrintForm pf = new PrintForm())
            {
                pf.ShowDialog();
            }
        }

        public List<PrintModel> GetPrintData(string strVin)
        {
            string strJbSql = String.Format("SELECT * FROM FC_CLJBXX WHERE VIN ='{0}' AND STATUS = '0'", strVin.ToUpper());
            DataSet dsJb = OracleHelper.ExecuteDataSet(OracleHelper.conn, strJbSql, null);
            if (dsJb.Tables[0].Rows.Count > 0)
            {
                List<PrintModel> printModelList = new List<PrintModel>();
                PrintModel printModel = new PrintModel() { Qcscqy = dsJb.Tables[0].Rows[0]["QCSCQY"].ToString() == "" ? dsJb.Tables[0].Rows[0]["JKQCZJXS"].ToString() : dsJb.Tables[0].Rows[0]["QCSCQY"].ToString(), Clxh = dsJb.Tables[0].Rows[0]["CLXH"].ToString(), Zczbzl = dsJb.Tables[0].Rows[0]["ZCZBZL"].ToString(), Qdxs = dsJb.Tables[0].Rows[0]["QDXS"].ToString(), Zdsjzzl = dsJb.Tables[0].Rows[0]["ZDSJZZL"].ToString() };
                string strRllx = dsJb.Tables[0].Rows[0]["RLLX"].ToString();
                if (strRllx == "汽油" || strRllx == "柴油" || strRllx == "两用燃料" || strRllx == "双燃料")
                {
                    try
                    {
                        string strPamSql = String.Format("SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='{0}'", strVin.ToUpper());
                        DataSet dsPam = OracleHelper.ExecuteDataSet(OracleHelper.conn, strPamSql, null);
                        for (int i = 0; i < dsPam.Tables[0].Rows.Count; i++)
                        {
                            string strCode = dsPam.Tables[0].Rows[i]["PARAM_CODE"].ToString();
                            string strValue = dsPam.Tables[0].Rows[i]["PARAM_VALUE"].ToString();
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
                    }
                    printModel.Rllx = "传统能源";
                }
                else
                {
                    printModel.Rllx = strRllx;
                }
                printModel.Bah = dsJb.Tables[0].Rows[0]["VIN"].ToString();
                printModel.Qysj = DateTime.Now.ToShortDateString();
                printModelList.Add(printModel);
                return printModelList;
            }
            else
            {
                return null;
            }
        }

        private void tbbah_Validating(object sender, CancelEventArgs e)
        {
            char bi;
            DataCheckVINHelper dc = new DataCheckVINHelper();
            if (this.tbbah.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbbah, "【备案号(VIN)】不能为空！");
            }
            else if (!dc.CheckCLSBDH(this.tbbah.Text.Trim().ToUpper(), out bi))
            {
                if (bi == '-')
                {
                    this.dxErrorProvider.SetError(tbbah, "请核对【备案号(VIN)】为17位字母或者数字！");
                }
                else
                {
                    this.dxErrorProvider.SetError(tbbah, String.Format("【备案号(VIN)】校验失败！第9位应为:'{0}'", bi));
                }
            }
            else
            {
                this.dxErrorProvider.SetError(tbbah, "");
            }
        }

        private void tbzwps_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzwps.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzwps, "");
            }
            else if (!IsInt(this.tbzwps.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzwps, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbzwps, "");
            }
        }

        private void tbzdsjzzl_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzdsjzzl.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzdsjzzl, "");
            }
            else if (!IsInt(this.tbzdsjzzl.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzdsjzzl, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbzdsjzzl, "");
            }
        }

        private void tbedzk_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbedzk.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbedzk, "");
            }
            else if (!IsInt(this.tbedzk.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbedzk, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbedzk, "");
            }
        }

        private void tblj_Validating(object sender, CancelEventArgs e)
        {
            if (this.tblj.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tblj, "");
            }
            else if (!this.VerifyParamFormat(this.tblj.Text.Trim(), '/') || this.tblj.Text.Trim().IndexOf('/') < 0)
            {
                this.dxErrorProvider.SetError(tblj, "应填写整数，前后轮距，中间用”/”隔开");
            }
            else
            {
                dxErrorProvider.SetError(tblj, "");
            }
        }

        private void tbzj_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzj.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzj, "");
            }
            else if (!this.IsInt(this.tbzj.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzj, "应填写整数");
            }
            else
            {
                dxErrorProvider.SetError(tbzj, "");
            }
        }

        private void tbzgcs_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzgcs.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzgcs, "");
            }
            else if (!this.VerifyParamFormat(this.tbzgcs.Text.Trim(), ','))
            {
                this.dxErrorProvider.SetError(tbzgcs, "应填写整数，多个数值应以半角“,”隔开，中间不留空格");
            }
            else
            {
                dxErrorProvider.SetError(tbzgcs, "");
            }
        }

        private void tbzczbzl_Validating(object sender, CancelEventArgs e)
        {
            if (this.tbzczbzl.Text.Trim() == "")
            {
                this.dxErrorProvider.SetError(tbzczbzl, "");
            }
            else if (!this.VerifyParamFormat(this.tbzczbzl.Text.Trim(), ','))
            {
                this.dxErrorProvider.SetError(tbzczbzl, "应填写整数，多个数值应以半角“,”隔开，中间不留空格");
            }
            else
            {
                dxErrorProvider.SetError(tbzczbzl, "");
            }
        }

        private void tbltgg_Validating(object sender, CancelEventArgs e)
        {
            string ltgg = this.tbltgg.Text.Trim();
            if (ltgg == "")
            {
                this.dxErrorProvider.SetError(tbltgg, "");
            }
            else if (VerifyLtgg(ltgg))
            {
                this.dxErrorProvider.SetError(tbltgg, "前后轮距不相同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)");
            }
            else
            {
                dxErrorProvider.SetError(tbltgg, "");
            }
        }

        private bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+]?\d*$");
        }

        private bool VerifyLtgg(string ltgg)
        {
            bool flag = false;
            try
            {
                if (!string.IsNullOrEmpty(ltgg))
                {
                    int indexLtgg = ltgg.IndexOf(")/(");
                    if (indexLtgg > -1)
                    {
                        string ltggHead = ltgg.Substring(0, indexLtgg + 1);
                        string ltggEnd = ltgg.Substring(indexLtgg + 3);

                        if (!ltggHead.StartsWith("(") || !ltggEnd.EndsWith(")"))
                        {
                            flag = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return flag;
        }

        private bool VerifyParamFormat(string value, char c)
        {
            if (!string.IsNullOrEmpty(c.ToString()))
            {
                string[] valueArr = value.Split(c);
                if (valueArr[0] == "" || valueArr[valueArr.Length - 1] == "")
                {
                    return false;
                }
                foreach (string val in valueArr)
                {
                    if (!this.IsInt(val))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // 校验必填数据
        private string VerifyRequired()
        {
            string msg = string.Empty; 
            if (string.IsNullOrEmpty(this.cbIsJkqc.Text.Trim()))
            {
                msg += "是否进口汽车不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbqcscqy.Text.Trim()))
            {
                msg += "汽车生产企业不能为空\r\n";
            }
            if (Utils.userId.Substring(4, 1).Equals("F") && string.IsNullOrEmpty(this.tbjkqczjxs.Text.Trim()))
            {
                msg += "进口汽车总经销商不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbjcjgmc.Text.Trim()))
            {
                msg += "检测机构名称不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbbgbh.Text.Trim()))
            {
                msg += "报告编号不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbbah.Text.Trim()))
            {
                msg += "备案号(VIN)不能为空\r\n";
            }
            if (Utils.userId.Substring(4, 1).Equals("F") && string.IsNullOrEmpty(this.tbHgspbm.Text.Trim()))
            {
                msg += "海关商品编码不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbclxh.Text.Trim()))
            {
                msg += "车辆型号不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.dtclzzrq.Text.Trim()))
            {
                msg += "制造日期/进口日期不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbclzl.Text.Trim()))
            {
                msg += "车辆种类不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbtymc.Text.Trim()))
            {
                msg += "通用名称不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbqdxs.Text.Trim()))
            {
                msg += "驱动型式不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbyyc.Text.Trim()))
            {
                msg += "越野车（G类）不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzczbzl.Text.Trim()))
            {
                msg += "整车整备质量不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzwps.Text.Trim()))
            {
                msg += "座位排数不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzgcs.Text.Trim()))
            {
                msg += "最高车速不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzdsjzzl.Text.Trim()))
            {
                msg += "最大设计总质量不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbltgg.Text.Trim()))
            {
                msg += "轮胎规格不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbedzk.Text.Trim()))
            {
                msg += "额定载客不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzj.Text.Trim()))
            {
                msg += "轴距不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tblj.Text.Trim()))
            {
                msg += "轮距（前/后）不能为空\r\n";
            }
            //if (string.IsNullOrEmpty(this.tbQtxx.Text.Trim()))
            //{
            //    msg += "其他信息不能为空\r\n";
            //}
            if (string.IsNullOrEmpty(this.cbrllx.Text.Trim()))
            {
                msg += "燃料类型不能为空\r\n";
            }
            if (!string.IsNullOrEmpty(msg))
            {
                msg = "\r\n" + msg;
            }
            return msg;
        }
    }
}