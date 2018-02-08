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

namespace FuelDataSysClient.Form_DBManager
{
    public partial class JbxxViewForm : Form
    {
        public event FormClosingEventHandler formClosingEventHandel;

        FuelDataService.FuelDataSysWebService service = Utils.service;
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
            //btnPrint.Visible = true;
            this.SetFuelType();
        }

        public JbxxViewForm(string uploadType)
        {
            InitializeComponent();
            this.panelControl.Location = new Point(0, 0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            //btnPrint.Visible = true;

            this.UploadType = uploadType;
            this.btnbaocunshangbao.Text = "补传";

            this.SetFuelType();
        }

        protected void SetFuelType()
        {
            List<string> fuelTypeList = Utils.GetFuelType("");
            this.tbrllx.Properties.Items.AddRange(fuelTypeList.ToArray());
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
            string sql = "SELECT PARAM_CODE, PARAM_NAME, FUEL_TYPE, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE FROM RLLX_PARAM WHERE   (FUEL_TYPE = '" + strType + "' AND STATUS = '1')  ORDER BY ORDER_RULE";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                // textbox类型
                if (dr["CONTROL_TYPE"].ToString() == "TEXT")
                {
                    Label lbl = new Label();
                    lbl.Width = 160;
                    lbl.Height = 30;
                    lbl.Name = "lbl" + dr["PARAM_CODE"].ToString();
                    lbl.Text = dr["PARAM_NAME"].ToString();
                    lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    TextEdit tb = new TextEdit();
                    tb.Width = 250;
                    tb.Height = 28;
                    tb.Name = dr["PARAM_CODE"].ToString();
                    tb.Enabled = enable;

                    Label lbll = new Label();
                    lbll.Width = 100;
                    lbll.Height = 30;
                    lbll.Text = dr["PARAM_REMARK"].ToString();
                    lbll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(tb);
                    this.tlp.Controls.Add(lbll);
                }
                // OPTION类型
                if (dr["CONTROL_TYPE"].ToString() == "OPTION")
                {
                    Label lbl = new Label();
                    lbl.Width = 160;
                    lbl.Height = 30;
                    lbl.Name = "lbl" + dr["PARAM_CODE"].ToString();
                    lbl.Text = dr["PARAM_NAME"].ToString();
                    lbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    DevExpress.XtraEditors.ComboBoxEdit cb = new DevExpress.XtraEditors.ComboBoxEdit();
                    cb.Width = 250;
                    cb.Height = 28;
                    cb.Name = dr["PARAM_CODE"].ToString();
                    cb.Properties.Items.AddRange(getArray(dr["CONTROL_VALUE"].ToString()));
                    cb.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                    cb.Enabled = enable;

                    Label lbll = new Label();
                    lbll.Width = 100;
                    lbll.Height = 30;
                    lbll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(cb);
                    this.tlp.Controls.Add(lbll);
                }

                tlp.Location = new Point(0, 15);
            }
        }

        public String[] getArray(string strValue)
        {
            String[] str = new String[] { };
            return strValue.Split('/');

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
            string msg = string.Empty;
            msg += this.VerifyRequired();
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
                                saveParam(true);
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
                        if (this.UploadType == "UPLOADOT" || this.UploadType==null)
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
                MessageBox.Show("保存失败："+ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 保存
        private void saveParam(bool flag)
        {
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();
            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
            try
            {
                string strCreater = Utils.userId;
                string strBah = this.tbvin.Text.Trim();
                string sqlJbxx = "DELETE FROM FC_CLJBXX WHERE VIN = '" + strBah + "'";
                string sqlParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + strBah + "'";

                if (!flag)
                {
                    AccessHelper.ExecuteNonQuery(tra, sqlJbxx, null);
                    AccessHelper.ExecuteNonQuery(tra, sqlParam, null);
                }
               

                string mainId = this.GetMainId(strBah);

                ArrayList sqlList = new ArrayList();

                #region 遍历参数,保存
                foreach (Control c in this.tlp.Controls)
                {
                    if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        //Control[] lblc = clj.Controls.Find("lbl" + c.Name, true);
                        string paramCode = c.Name;
                        string paramValue = c.Text;
                        string strSQL = @"INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                    VALUES(@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";

                        OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",paramCode),
                                     new OleDbParameter("@VIN",strBah),
                                     new OleDbParameter("@PARAM_VALUE",paramValue),
                                     new OleDbParameter("@V_ID","")
                                   };
                        AccessHelper.ExecuteNonQuery(tra, strSQL, paramList);
                    }

                }
                #endregion

                #region 保存车辆基本信息
                // 保存车辆基本信息
                string sqlStr = @"INSERT INTO FC_CLJBXX
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
                                (   @VIN,
                                    @HGSPBM,
                                    @USER_ID,
                                    @QCSCQY,
                                    @JKQCZJXS,
                                    @CLXH,
                                    @CLZL,
                                    @RLLX,
                                    @ZCZBZL,
                                    @ZGCS,
                                    @LTGG,
                                    @ZJ,
                                    @CLZZRQ,
                                    @UPLOADDEADLINE,
                                    @TYMC,
                                    @YYC,
                                    @ZWPS,
                                    @ZDSJZZL,
                                    @EDZK,
                                    @LJ,
                                    @QDXS,
                                    @STATUS,
                                    @JYJGMC,
                                    @JYBGBH,
                                    @QTXX,
                                    @CREATETIME,
                                    @UPDATETIME)";

                DateTime clzzrqDate = DateTime.Parse(this.tbclzzrq.Text.Trim());
                OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                clzzrq.OleDbType = OleDbType.DBDate;

                DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                uploadDeadline.OleDbType = OleDbType.DBDate;

                OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                creTime.OleDbType = OleDbType.DBDate;
                OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                upTime.OleDbType = OleDbType.DBDate;

                OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",this.tbvin.Text.Trim().ToUpper()),
                                     new OleDbParameter("@HGSPBM",this.tbHgspbm.Text.Trim().ToUpper()),
                                     new OleDbParameter("@USER_ID",strCreater),
                                     new OleDbParameter("@QCSCQY",this.tbqcscqy.Text.Trim()),
                                     new OleDbParameter("@JKQCZJXS",this.tbjkqczjxs.Text.Trim()),
                                     new OleDbParameter("@CLXH",this.tbclxh.Text.Trim()),
                                     new OleDbParameter("@CLZL",this.tbclzl.Text.Trim()),
                                     new OleDbParameter("@RLLX",this.tbrllx.Text.Trim()),
                                     new OleDbParameter("@ZCZBZL",this.tbzczbzl.Text.Trim()),
                                     new OleDbParameter("@ZGCS",this.tbzgcs.Text.Trim()),
                                     new OleDbParameter("@LTGG",this.tbltgg.Text.Trim()),
                                     new OleDbParameter("@ZJ",this.tbzj.Text.Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@TYMC",this.tbtymc.Text.Trim()),
                                     new OleDbParameter("@YYC",this.tbyyc.Text.Trim()),
                                     new OleDbParameter("@ZWPS",this.tbzwps.Text.Trim()),
                                     new OleDbParameter("@ZDSJZZL",this.tbzdsjzzl.Text.Trim()),
                                     new OleDbParameter("@EDZK",this.tbedzk.Text.Trim()),
                                     new OleDbParameter("@LJ",this.tblj.Text.Trim()),
                                     new OleDbParameter("@QDXS",this.tbqdxs.Text.Trim()),
                                     new OleDbParameter("@STATUS",this.status),
                                     new OleDbParameter("@JYJGMC",this.tbjyjgmc.Text.Trim()),
                                     new OleDbParameter("@JYBGBH",this.tbjybgbh.Text.Trim()),
                                     new OleDbParameter("@QTXX",this.tbQtxx.Text.Trim()),
                                     creTime,
                                     upTime
                                     };
                #endregion

                AccessHelper.ExecuteNonQuery(tra, sqlStr, param);
                tra.Commit();
                strVin = strBah; //备案号

            }
            catch (Exception ex)
            {
                // MessageBox.Show("保存失败!");
                tra.Rollback();
                throw ex;
            }
            finally
            {
                con.Close();
            }
        }

        // 上报
        private FuelDataService.OperateResult applyParam(string uploadType)
        {
            try
            {
                List<FuelDataModel.VehicleBasicInfo> lbiList = new List<FuelDataModel.VehicleBasicInfo>();
                FuelDataModel.VehicleBasicInfo lbi = new FuelDataModel.VehicleBasicInfo();
                //VehicleBasicInfo[] lbiList = new VehicleBasicInfo[1] { new VehicleBasicInfo() };
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
                        Control[] lblc = this.Controls.Find(strLabName, true);
                        string paramCode = c.Name;
                        string paramValue = c.Text;
                        //string paramName = lblc[0].Text;

                        pe.V_Id = "";
                        pe.Param_Code = paramCode;
                        pe.Vin = this.tbvin.Text.Trim();
                        //pe.Param_Name = paramName;
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
                        return service.UploadOverTime(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");
                    }
                    return null;
                }
                else
                {
                    return service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");
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
                    saveParam(false);
                    FuelDataService.OperateResult result = applyParam(this.UploadType);
                    int count = Utils.ApplyFlg(result).Count;
                    if (count > 0)
                    {
                        this.btnPrint.Visible = false;

                        OperateResult oResult = Utils.OperateResultS2C(result);
                        NameValuePair nvp = oResult.ResultDetail[0] as NameValuePair;
                        MessageBox.Show("备案号（VIN）：" + nvp.Name + "\n信息：" + nvp.Value + "】", "上报失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        Utils.UpdataState(result);
                        setEnable();

                        this.btnPrint.Visible = true;
                        this.btnbaocunshangbao.Enabled = false;
                        this.btnbaocun.Enabled = false;
                        string strVIN = this.tbvin.Text.Trim();
                        string strVID = GetSussVID(strVIN);
                        if (strVID != null)
                        {
                            MessageBox.Show("备案号（VIN）：" + strVIN + "\n反馈码（VID）：" + strVID, "上报成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // 上报成功，返回本地数据库的VID
        public string GetSussVID(string strBah)
        {
            string strVid = "";
            string strSql = "SELECT V_ID FROM FC_CLJBXX WHERE VIN ='" + strBah + "'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, strSql, null);
            if (ds != null)
            {
                strVid = ds.Tables[0].Rows[0]["V_ID"].ToString();
            }
            return strVid;
        }

        // 打印
        private void btnPrint_Click(object sender, EventArgs e)
        {
            Utils.printModel = GetPrintData(this.tbvin.Text.Trim());
            using (PrintForm pf = new PrintForm())
            {
                pf.ShowDialog();
            }
        }

        public string GetMainId(string vin)
        {
            string mainId = string.Empty;
            string sql = string.Format(@"SELECT MAIN_ID FROM FC_CLJBXX WHERE VIN ='{0}'", vin);
            try
            {
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
                if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    mainId = ds.Tables[0].Rows[0]["MAIN_ID"] == null ? "" : ds.Tables[0].Rows[0]["MAIN_ID"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return mainId;
        }

        public List<PrintModel> GetPrintData(string strVin)
        {
            PrintModel printModel = new PrintModel();
            FuelDataService.VehicleBasicInfo[] queryInfoArr = service.QueryUploadedFuelData(Utils.userId, Utils.password, 1, 20, strVin, null, null, null, null, null, null);
            if (queryInfoArr != null)
            {
                List<FuelDataModel.VehicleBasicInfo> vbis = Utils.FuelInfoS2C(queryInfoArr);
                if (vbis.Count > 0)
                {
                    PrintForm pf = new PrintForm();
                    printModel.Qcscqy = vbis[0].Qcscqy == "" ? vbis[0].Jkqczjxs : vbis[0].Qcscqy;
                    printModel.Clxh = vbis[0].Clxh;
                    printModel.Zczbzl = vbis[0].Zczbzl.ToString();
                    string strRllx = vbis[0].Rllx;
                    printModel.Qdxs = vbis[0].Qdxs;
                    printModel.Zdsjzzl = vbis[0].Zdsjzzl.ToString();
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


        // 校验必填数据
        private string VerifyRequired()
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(this.tbqcscqy.Text.Trim()))
            {
                msg += "乘用车生产企业不能为空\r\n";
            }
            if (Utils.userId.Substring(4, 1).Equals("F") && string.IsNullOrEmpty(this.tbjkqczjxs.Text.Trim()))
            {
                msg += "进口乘用车供应企业不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbjyjgmc.Text.Trim()))
            {
                msg += "检测机构名称不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbjybgbh.Text.Trim()))
            {
                msg += "报告编号不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbvin.Text.Trim()))
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
            if (string.IsNullOrEmpty(this.tbclzzrq.Text.Trim()))
            {
                msg += "制造日期/进口日期不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbclzl.Text.Trim()))
            {
                msg += "车辆种类不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbtymc.Text.Trim()))
            {
                msg += "通用名称不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbqdxs.Text.Trim()))
            {
                msg += "驱动型式不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbyyc.Text.Trim()))
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
            if (string.IsNullOrEmpty(this.tbrllx.Text.Trim()))
            {
                msg += "燃料类型不能为空\r\n";
            }
            if (!string.IsNullOrEmpty(msg))
            {
                msg = Environment.NewLine + msg;
            }
            return msg;
        }
    }
}
