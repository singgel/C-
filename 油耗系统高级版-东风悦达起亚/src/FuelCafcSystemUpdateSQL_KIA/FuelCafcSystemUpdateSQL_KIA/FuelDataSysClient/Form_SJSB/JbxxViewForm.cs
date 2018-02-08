using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Collections;
using DevExpress.XtraEditors;
using FuelDataSysClient.SubForm;
using FuelDataModel;
using FuelDataSysClient.Tool;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using FuelDataSysClient.Model;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class JbxxViewForm : Form
    {
        public event FormClosingEventHandler formClosingEventHandel;
        private string uploadType;
        public string UploadType
        {
            get { return uploadType; }
            set { uploadType = value; }
        }
        public static string strVin;
        public string status;
        private DataTable dtBasic;
        private DataTable dtParam;
        private bool isUploaded = false;
        private bool isApply = false;

        public JbxxViewForm()
        {
            InitializeComponent();
            this.panelControl.Location = new Point(0, 0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            this.SetFuelType();
        }

        public JbxxViewForm(string uploadType)
        {
            InitializeComponent();
            this.panelControl.Location = new Point(0, 0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            this.UploadType = uploadType;
            this.btnbaocunshangbao.Text = "补传";
            this.SetFuelType();
        }

        public JbxxViewForm(DataTable dtBasic, DataTable dtParam, bool isUploaded,bool isApply)
        {
            InitializeComponent();
            this.panelControl.Location = new Point(0, 0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            this.dtBasic = dtBasic;
            this.dtParam = dtParam;
            this.isUploaded = isUploaded;
            this.isApply = isApply;
            // 设置燃料种类下拉框控件值
            this.SetFuelType();
            if (this.isApply)
            {
                this.tbvin.Properties.ReadOnly = false;
            }
        }

        protected void SetFuelType()
        {
            List<string> fuelTypeList = Utils.GetFuelType("");
            this.tbrllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        private void cbrllx_SelectedValueChanged(object sender, EventArgs e)
        {
            this.tc.SelectedTabPage = this.tp2;
            string strType = this.tbrllx.SelectedItem.ToString().Trim();
            if (strType != "" && strType != null)
            {
                if (strType == "汽油" || strType == "柴油" || strType == "两用燃料" || strType == "双燃料")
                {
                    getParamList("传统能源", true);
                }
                else
                {
                    getParamList(strType, true);
                }
            }
        }

        public void getParamList(string strType, bool enable)
        {
            // 先清空，再添加
            this.tlp.Controls.Clear();
            this.tlp.Location = new Point(10, 30);
            DataTable dt = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("SELECT PARAM_CODE, PARAM_NAME, FUEL_TYPE, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE FROM RLLX_PARAM WHERE   (FUEL_TYPE = '{0}' AND STATUS = '1')  ORDER BY ORDER_RULE", strType), null).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                // textbox类型
                if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                {
                    Label lbl = new Label() { Width = 160, Height = 30, Name = "lbl" + dr["PARAM_CODE"].ToString(), Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    TextEdit tb = new TextEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString(), Enabled = enable };
                    Label lbll = new Label() { Width = 100, Height = 30, Text = dr["PARAM_REMARK"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(tb);
                    this.tlp.Controls.Add(lbll);
                }
                // OPTION类型
                if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                {
                    Label lbl = new Label() { Width = 160, Height = 30, Name = "lbl" + dr["PARAM_CODE"], Text = dr["PARAM_NAME"].ToString(), TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    ComboBoxEdit cb = new ComboBoxEdit() { Width = 250, Height = 28, Name = dr["PARAM_CODE"].ToString(), Enabled = enable };
                    cb.Properties.Items.AddRange(dr["CONTROL_VALUE"].ToString().Split('/'));
                    cb.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                    Label lbll = new Label() { Width = 100, Height = 30, TextAlign = System.Drawing.ContentAlignment.MiddleLeft };
                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(cb);
                    this.tlp.Controls.Add(lbll);
                }
                tlp.Location = new Point(0, 15);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (formClosingEventHandel != null)
            {
                formClosingEventHandel(sender, null);
            }
            this.Close();
        }

        private void btnbaocun_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                       "请确认修改该条油耗数据？",
                       "提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                string msg = string.Empty;
                msg += Utils.VerifyRLParam(this.tlp.Controls);
                try
                {
                    if (!this.dxErrorProvider.HasErrors && string.IsNullOrEmpty(msg))
                    {
                        if (this.isUploaded)
                        {
                            if (CheckIsValueChanged())
                            {
                                if (this.status == "1")
                                {
                                    saveParam(false);
                                }
                                else
                                {
                                    saveParam(false);
                                }

                                MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("请修改记录后再保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            if (this.isApply)
                            {
                                saveParam(false);
                            }
                            if (this.UploadType == "UPLOADOT" || this.UploadType == null)
                            {
                                saveParam(false);
                            }
                            MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请核对页面信息是否填写正确！" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 保存
        private void saveParam(bool flag)
        {
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = con.BeginTransaction();
                try
                {
                    string strBah = this.tbvin.Text.Trim();
                    string strSC_OCN = OracleHelper.ExecuteScalar(OracleHelper.conn, String.Format("SELECT SC_OCN FROM FC_CLJBXX WHERE VIN = '{0}'", strBah)).ToString();
                    if (!flag)
                    {
                        OracleHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM FC_CLJBXX WHERE VIN = '{0}'", strBah), null);
                        OracleHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", strBah), null);
                    }

                    #region 遍历参数,保存
                    foreach (Control c in this.tlp.Controls)
                    {
                        if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                        {
                            OracleParameter[] paramList = { 
                                    new OracleParameter("PARAM_CODE",c.Name),
                                    new OracleParameter("VIN",strBah),
                                    new OracleParameter("PARAM_VALUE",c.Text),
                                    new OracleParameter("V_ID","")
                                };
                            OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                VALUES(:PARAM_CODE,:VIN,:PARAM_VALUE,:V_ID)", paramList);
                        }

                    }
                    #endregion

                    #region 保存车辆基本信息
                    // 保存车辆基本信息
                    DateTime clzzrqDate = DateTime.Parse(this.tbclzzrq.Text.Trim());
                    OracleParameter clzzrq = new OracleParameter("CLZZRQ", clzzrqDate) { DbType = DbType.Date };
                    DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                    OracleParameter uploadDeadline = new OracleParameter("UPLOADDEADLINE", uploadDeadlineDate) { DbType = DbType.Date };
                    OracleParameter creTime = new OracleParameter("CREATETIME", DateTime.Now) { DbType = DbType.Date };
                    OracleParameter upTime = new OracleParameter("UPDATETIME", DateTime.Now) { DbType = DbType.Date };
                    OracleParameter[] param = { 
                                    new OracleParameter("VIN",this.tbvin.Text.Trim().ToUpper()),
                                    new OracleParameter("SC_OCN",strSC_OCN),
                                    new OracleParameter("HGSPBM",this.tbHgspbm.Text.Trim().ToUpper()),
                                    new OracleParameter("USER_ID",Utils.localUserId),
                                    new OracleParameter("QCSCQY",this.tbqcscqy.Text.Trim()),
                                    new OracleParameter("JKQCZJXS",this.tbjkqczjxs.Text.Trim()),
                                    new OracleParameter("CLXH",this.tbclxh.Text.Trim()),
                                    new OracleParameter("CLZL",this.tbclzl.Text.Trim()),
                                    new OracleParameter("RLLX",this.tbrllx.Text.Trim()),
                                    new OracleParameter("ZCZBZL",this.tbzczbzl.Text.Trim()),
                                    new OracleParameter("ZGCS",this.tbzgcs.Text.Trim()),
                                    new OracleParameter("LTGG",this.tbltgg.Text.Trim()),
                                    new OracleParameter("ZJ",this.tbzj.Text.Trim()),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("TYMC",this.tbtymc.Text.Trim()),
                                    new OracleParameter("YYC",this.tbyyc.Text.Trim()),
                                    new OracleParameter("ZWPS",this.tbzwps.Text.Trim()),
                                    new OracleParameter("ZDSJZZL",this.tbzdsjzzl.Text.Trim()),
                                    new OracleParameter("EDZK",this.tbedzk.Text.Trim()),
                                    new OracleParameter("LJ",this.tblj.Text.Trim()),
                                    new OracleParameter("QDXS",this.tbqdxs.Text.Trim()),
                                    new OracleParameter("STATUS",this.status),
                                    new OracleParameter("JYJGMC",this.tbjyjgmc.Text.Trim()),
                                    new OracleParameter("JYBGBH",this.tbjybgbh.Text.Trim()),
                                    new OracleParameter("QTXX",this.tbQtxx.Text.Trim()),
                                    creTime,
                                    upTime,
                                    };
                    OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX
                            (   VIN,
                                SC_OCN,
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
                                :SC_OCN,
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
        private FuelDataService.OperateResult applyParam(string uploadType)
        {
            try
            {
                List<VehicleBasicInfo> lbiList = new List<VehicleBasicInfo>();
                VehicleBasicInfo lbi = new VehicleBasicInfo();
                lbi.V_Id = "";
                lbi.User_Id = Utils.userId;
                lbi.Qcscqy = tbqcscqy.Text.Trim();
                lbi.Jkqczjxs = this.tbjkqczjxs.Text.Trim();
                lbi.Vin = this.tbvin.Text.Trim().ToUpper();
                lbi.Hgspbm = this.tbHgspbm.Text.Trim().ToUpper();
                lbi.Clxh = this.tbclxh.Text.Trim();
                lbi.Clzl = this.tbclzl.Text.Trim();
                lbi.Rllx = this.tbrllx.Text.Trim();
                lbi.Zczbzl = this.tbzczbzl.Text.Trim();
                lbi.Zgcs = this.tbzgcs.Text.Trim();
                lbi.Ltgg = this.tbltgg.Text.Trim();
                lbi.Zj = this.tbzj.Text.Trim();
                lbi.Clzzrq = DateTime.Parse(this.tbclzzrq.Text);
                lbi.Tymc = this.tbtymc.Text.Trim();
                lbi.Yyc = this.tbyyc.Text.Trim();
                lbi.Zwps = this.tbzwps.Text.Trim();
                lbi.Zdsjzzl = this.tbzdsjzzl.Text.Trim();
                lbi.Edzk = this.tbedzk.Text.Trim();
                lbi.Lj = this.tblj.Text.Trim();
                lbi.Qdxs = this.tbqdxs.Text.Trim();
                lbi.Jyjgmc = this.tbjyjgmc.Text.Trim();
                lbi.Jybgbh = this.tbjybgbh.Text.Trim();
                lbi.Qtxx = this.tbQtxx.Text.Trim();
                lbi.CreateTime = DateTime.Now;
                lbi.UpdateTime = DateTime.Now;
                List<FuelDataModel.RllxParamEntity> listParam = new List<FuelDataModel.RllxParamEntity>();
                foreach (Control c in this.tlp.Controls)
                {
                    if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        FuelDataModel.RllxParamEntity pe = new FuelDataModel.RllxParamEntity();
                        string strLabName = "lbl" + c.Name;
                        string paramCode = c.Name;
                        pe.V_Id = "";
                        pe.Param_Code = paramCode;
                        pe.Vin = this.tbvin.Text.Trim();
                        pe.Param_Value = c.Text;
                        listParam.Add(pe);
                    }
                }
                lbi.EntityList = listParam.ToArray();
                lbiList.Add(lbi);
                // 上报
                if (uploadType == "UPLOADOT")
                {
                    string delReason = string.Empty;
                    ReasonForm rf = new ReasonForm();
                    Utils.SetFormMid(rf);
                    rf.Text = "补传原因";
                    rf.ShowDialog();
                    if (rf.DialogResult == DialogResult.OK)
                    {
                        foreach (VehicleBasicInfo vInfo in lbiList)
                        {
                            vInfo.Reason = rf.Reason;
                        }
                        return Utils.service.UploadOverTime(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");
                    }
                    return null;
                }
                else
                {
                    return Utils.service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 设置不可编辑
        public void setEnable()
        {
            foreach (Control c in this.tlp.Controls)
            {
                if (c is TextBox || c is DevExpress.XtraEditors.ComboBoxEdit || c is TextEdit)
                {
                    c.Enabled = false;
                }
            }
            foreach (Control c in this.paneljiben.Controls)
            {
                if (c is TextBox || c is DevExpress.XtraEditors.ComboBoxEdit || c is TextEdit)
                {
                    c.Enabled = false;
                }
            }
        }

        // 设置编辑状态
        public void setVisible(string cName, bool visible)
        {
            Control[] cs = this.Controls.Find(cName, true);
            if (cs.Length > 0)
            {
                cs[0].Visible = visible;
            }
        }

        // 保存并上报
        private void btnbaocunshangbao_Click(object sender, EventArgs e)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            string msg = string.Empty;
            msg += Utils.VerifyRLParam(this.tlp.Controls);
            try
            {
                if (!this.dxErrorProvider.HasErrors && string.IsNullOrEmpty(msg))
                {
                    saveParam(false);
                    FuelDataService.OperateResult result = applyParam(this.UploadType);
                    int count = Utils.ApplyFlg(result).Count;
                    if (count > 0)
                    {
                        this.btnPrint.Visible = false;

                        OperateResult oResult = Utils.OperateResultS2C(result);
                        NameValuePair nvp = oResult.ResultDetail[0] as NameValuePair;
                        MessageBox.Show(String.Format("备案号（VIN）：{0}\n信息：{1}】", nvp.Name, nvp.Value), "上报失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        Utils.UpdataState(result);
                        setEnable();
                        this.btnPrint.Visible = true;
                        this.btnbaocunshangbao.Enabled = false;
                        this.btnbaocun.Enabled = false;
                        string strVID = OracleHelper.ExecuteScalar(OracleHelper.conn, String.Format("SELECT V_ID FROM FC_CLJBXX WHERE VIN ='{0}'", this.tbvin.Text.Trim())).ToString();
                        if (strVID != null)
                        {
                            MessageBox.Show(String.Format("备案号（VIN）：{0}\n反馈码（VID）：{1}", this.tbvin.Text.Trim(), strVID), "上报成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("保存并上传失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 打印
        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            Utils.printModel = GetPrintData(this.tbvin.Text.Trim());
            using (PrintForm pf = new PrintForm())
            {
                pf.ShowDialog();
            }
        }

        public List<PrintModel> GetPrintData(string strVin)
        {
            PrintModel printModel = new PrintModel();
            FuelDataService.VehicleBasicInfo[] queryInfoArr = Utils.service.QueryUploadedFuelData(Utils.userId, Utils.password, 1, 20, strVin, null, null, null, null, null, null);
            if (queryInfoArr != null)
            {
                List<VehicleBasicInfo> vbis = Utils.FuelInfoS2C(queryInfoArr);
                if (vbis.Count > 0)
                {
                    using (PrintForm pf = new PrintForm())
                    {
                        printModel.Qcscqy = vbis[0].Qcscqy == "" ? vbis[0].Jkqczjxs : vbis[0].Qcscqy;
                        printModel.Clxh = vbis[0].Clxh;
                        printModel.Zczbzl = vbis[0].Zczbzl;
                        string strRllx = vbis[0].Rllx;
                        printModel.Qdxs = vbis[0].Qdxs;
                        printModel.Zdsjzzl = vbis[0].Zdsjzzl;
                        if (strRllx == "汽油" || strRllx == "柴油" || strRllx == "两用燃料" || strRllx == "双燃料")
                        {
                            try
                            {
                                RllxParamEntity[] rllxList = vbis[0].EntityList;
                                for (int i = 0; i < rllxList.Length; i++)
                                {
                                    string strCode = rllxList[i].Param_Code;
                                    string strValue = rllxList[i].Param_Value;
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
                            catch (Exception)
                            {
                            }
                        }
                        printModel.Rllx = strRllx;
                        printModel.Bah = vbis[0].Vin;
                        printModel.Qysj = DateTime.Now.ToShortDateString();
                    }
                }
            }
            List<PrintModel> printModelList = new List<PrintModel>();
            printModelList.Add(printModel);
            return printModelList;
        }

        private bool CheckIsValueChanged()
        {
            if (this.tbvin.Text.Trim() != dtBasic.Rows[0]["VIN"].ToString().Trim())
            { return true; }
            if (this.tbHgspbm.Text.Trim() != dtBasic.Rows[0]["HGSPBM"].ToString().Trim())
            { return true; }
            if (this.tbqcscqy.Text.Trim() != dtBasic.Rows[0]["QCSCQY"].ToString().Trim())
            { return true; }
            if (this.tbjkqczjxs.Text.Trim() != dtBasic.Rows[0]["JKQCZJXS"].ToString().Trim())
            { return true; }
            if (this.tbclxh.Text.Trim() != dtBasic.Rows[0]["CLXH"].ToString().Trim())
            { return true; }
            if (this.tbclzl.Text.Trim() != dtBasic.Rows[0]["CLZL"].ToString().Trim())
            { return true; }
            if (this.tbrllx.Text.Trim() != dtBasic.Rows[0]["RLLX"].ToString().Trim())
            { return true; }
            if (this.tbzczbzl.Text.Trim() != dtBasic.Rows[0]["ZCZBZL"].ToString().Trim())
            { return true; }
            if (this.tbzgcs.Text.Trim() != dtBasic.Rows[0]["ZGCS"].ToString().Trim())
            { return true; }
            if (this.tbltgg.Text.Trim() != dtBasic.Rows[0]["LTGG"].ToString().Trim())
            { return true; }
            if (this.tbzj.Text.Trim() != dtBasic.Rows[0]["ZJ"].ToString().Trim())
            { return true; }
            if (DateTime.Parse(this.tbclzzrq.Text) != DateTime.Parse(dtBasic.Rows[0]["CLZZRQ"].ToString().Trim()))
            { return true; }
            if (this.tbtymc.Text.Trim() != dtBasic.Rows[0]["TYMC"].ToString().Trim())
            { return true; }
            if (this.tbyyc.Text.Trim() != dtBasic.Rows[0]["YYC"].ToString().Trim())
            { return true; }
            if (this.tbzwps.Text.Trim() != dtBasic.Rows[0]["ZWPS"].ToString().Trim())
            { return true; }
            if (this.tbzdsjzzl.Text.Trim() != dtBasic.Rows[0]["ZDSJZZL"].ToString().Trim())
            { return true; }
            if (this.tbedzk.Text.Trim() != dtBasic.Rows[0]["EDZK"].ToString().Trim())
            { return true; }
            if (this.tblj.Text.Trim() != dtBasic.Rows[0]["LJ"].ToString().Trim())
            { return true; }
            if (this.tbqdxs.Text.Trim() != dtBasic.Rows[0]["QDXS"].ToString().Trim())
            { return true; }
            if (this.tbjyjgmc.Text.Trim() != dtBasic.Rows[0]["JYJGMC"].ToString().Trim())
            { return true; }
            if (this.tbjybgbh.Text.Trim() != dtBasic.Rows[0]["JYBGBH"].ToString().Trim())
            { return true; }
            if (this.tbQtxx.Text.Trim() != dtBasic.Rows[0]["QTXX"].ToString().Trim())
            { return true; }

            foreach (DataRow dr in dtParam.Rows)
            {
                Control[] c = this.Controls.Find(dr["PARAM_CODE"].ToString().Trim(), true);
                if (dr["PARAM_VALUE"].ToString().Trim() != c[0].Text.Trim())
                {
                    return true;
                }
            }

            return false;
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
            catch (Exception)
            {
            }
            return flag;
        }

    }
}
