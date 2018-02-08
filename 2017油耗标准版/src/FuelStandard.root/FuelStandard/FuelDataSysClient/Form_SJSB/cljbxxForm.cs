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

namespace FuelDataSysClient
{
    //DevExpress.XtraBars.Ribbon.RibbonForm
    //DevExpress.XtraEditors.XtraForm
    public partial class cljbxxForm : Form
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;

        public static string strVin;

        public cljbxxForm()
        {
            List<string> fuelTypeList = Utils.GetFuelType("");
            InitializeComponent();
            this.panelControl.Location= new Point(0,0);
            this.panelControl.Width = 1250;
            this.panelControl.Height = 1000;
            this.btnPrint.Visible = false;
            this.cbrllx.Text = fuelTypeList[0];
            this.cbIsJkqc.Text = "否";
            this.tbqcscqy.Text = Utils.qymc;
            this.tc.SelectedTabPage = this.tp1;
            this.dtclzzrq.EditValue = DateTime.Today;
            //comboBoxEdit1.
            //this.dropDownButton1

            string fuelTypeName = this.cbrllx.Text.Trim();
            if (fuelTypeName == "汽油" || fuelTypeName == "柴油" || fuelTypeName == "两用燃料" || fuelTypeName == "双燃料" || fuelTypeName == "气体燃料")
            {
                getParamList("传统能源");
            }
            else
            {
                getParamList(fuelTypeName);
            }
            this.cbrllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        private void cljbxxForm_Load(object sender, EventArgs e)
        {
            if (Utils.userId.Substring(4, 1).Equals("F"))
            {
                this.cbIsJkqc.Text = "是";
            }
            else
            {
                this.cbIsJkqc.Text = "否";
            }
        }

        private void cbrllx_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.tc.SelectedTabPage = this.tp2;
                string strType = this.cbrllx.SelectedItem.ToString().Trim();
                if (strType != "" && strType != null)
                {
                    if (strType == "汽油" || strType == "柴油" || strType == "两用燃料" || strType == "双燃料" || strType == "气体燃料")
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
                tbqcscqy.Enabled = true;
                tbjkqczjxs.Enabled = true;
                tbjkqczjxs.Text = Utils.qymc;
                tbHgspbm.Enabled = true;
            }
            else
            {
                tbjkqczjxs.Text = "";
                tbjkqczjxs.Enabled = false;
                tbqcscqy.Text = Utils.qymc;
                tbqcscqy.Enabled = true;
                tbHgspbm.Enabled = false;
            }
        }

        public void getParamList(string strType)
        {
            // 先清空，再添加
            this.tlp.Controls.Clear();
            this.tlp.Location = new Point(10, 30);
            string sql = "SELECT PARAM_CODE, PARAM_NAME, FUEL_TYPE, PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE FROM RLLX_PARAM WHERE   (FUEL_TYPE = '" + strType + "' AND STATUS = '1') ORDER BY ORDER_RULE";
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

                    DevExpress.XtraEditors.ComboBoxEdit cbe = new ComboBoxEdit();
                    cbe.Width = 250;
                    cbe.Height = 28;
                    cbe.Name = dr["PARAM_CODE"].ToString();
                    cbe.Properties.Items.AddRange(getArray(dr["CONTROL_VALUE"].ToString()));
                    cbe.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                    
                    Label lbll = new Label();
                    lbll.Width = 100;
                    lbll.Height = 30;
                    lbll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

                    this.tlp.Controls.Add(lbl);
                    this.tlp.Controls.Add(cbe);
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
            string msg = this.VerifyRequired();
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
                        MessageBox.Show("【备案号（VIN）：" + nvp.Name + "\n信息：" + nvp.Value + "】", "上报失败", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        Utils.UpdataState(result);
                        setEnable();

                        this.btnPrint.Visible = true;
                        this.btnbaocunshangbao.Enabled = false;
                        this.btnbaocun.Enabled = false;
                        string strVIN = this.tbbah.Text.Trim().ToUpper();
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
               throw ex;
            }
        }

        // 保存
        private void saveParam() 
        {
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();
            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
            try
            {
                string strCreater = Utils.userId;
                string strBah = this.tbbah.Text.Trim().ToUpper();
                string sqlJbxx = "DELETE FROM FC_CLJBXX WHERE VIN = '" + strBah + "' AND STATUS='1'";
                string sqlParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + strBah + "'";
                AccessHelper.ExecuteNonQuery(tra, sqlJbxx, null);
                AccessHelper.ExecuteNonQuery(tra, sqlParam, null);

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

                DateTime clzzrqDate = DateTime.Parse(this.dtclzzrq.Text.Trim());
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
                                     new OleDbParameter("@VIN",this.tbbah.Text.Trim().ToUpper()),
                                     new OleDbParameter("@HGSPBM",this.tbHgspbm.Text.Trim().ToUpper()),
                                     new OleDbParameter("@USER_ID",strCreater),
                                     new OleDbParameter("@QCSCQY",this.tbqcscqy.Text.Trim()),
                                     new OleDbParameter("@JKQCZJXS",this.tbjkqczjxs.Text.Trim()),
                                     new OleDbParameter("@CLXH",this.tbclxh.Text.Trim()),
                                     new OleDbParameter("@CLZL",this.cbclzl.Text.Trim()),
                                     new OleDbParameter("@RLLX",this.cbrllx.Text.Trim()),
                                     new OleDbParameter("@ZCZBZL",this.tbzczbzl.Text.Trim()),
                                     new OleDbParameter("@ZGCS",this.tbzgcs.Text.Trim()),
                                     new OleDbParameter("@LTGG",this.tbltgg.Text.Trim()),
                                     new OleDbParameter("@ZJ",this.tbzj.Text.Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@TYMC",this.tbtymc.Text.Trim()),
                                     new OleDbParameter("@YYC",this.cbyyc.Text.Trim()),
                                     new OleDbParameter("@ZWPS",this.tbzwps.Text.Trim()),
                                     new OleDbParameter("@ZDSJZZL",this.tbzdsjzzl.Text.Trim()),
                                     new OleDbParameter("@EDZK",this.tbedzk.Text.Trim()),
                                     new OleDbParameter("@LJ",this.tblj.Text.Trim()),
                                     new OleDbParameter("@QDXS",this.cbqdxs.Text.Trim()),
                                     new OleDbParameter("@STATUS","1"),
                                     new OleDbParameter("@JYJGMC",this.tbjcjgmc.Text.Trim()),
                                     new OleDbParameter("@JYBGBH",this.tbbgbh.Text.Trim()),
                                     new OleDbParameter("@QTXX",this.tbQtxx.Text.Trim()),
                                     creTime,
                                     upTime
                                     };
                #endregion

                AccessHelper.ExecuteNonQuery(tra, sqlStr, param);
                tra.Commit();
                //MessageBox.Show("保存成功!");
                strVin = strBah; //备案号
                //this.Close();

            }
            catch(Exception ex)
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
        private FuelDataService.OperateResult applyParam() 
        {
            //bool AppFlg = false;
            try
            {
                List<VehicleBasicInfo> lbiList = new List<VehicleBasicInfo>();
                VehicleBasicInfo lbi = new VehicleBasicInfo();
                //VehicleBasicInfo[] lbiList = new VehicleBasicInfo[1] { new VehicleBasicInfo() };
                lbi.V_Id = "";
                lbi.User_Id = Utils.userId;
                lbi.Qcscqy = tbqcscqy.Text.Trim();
                lbi.Jkqczjxs = this.tbjkqczjxs.Text.Trim();
                lbi.Vin = this.tbbah.Text.Trim().ToUpper();
                lbi.Hgspbm = this.tbHgspbm.Text.Trim().ToUpper();
                lbi.Clxh = this.tbclxh.Text.Trim();
                lbi.Clzl = this.cbclzl.Text.Trim();
                lbi.Rllx = this.cbrllx.Text.Trim();
                lbi.Zczbzl = this.tbzczbzl.Text.Trim();
                lbi.Zgcs = this.tbzgcs.Text.Trim();
                lbi.Ltgg = this.tbltgg.Text.Trim();
                lbi.Zj = this.tbzj.Text.Trim();
                lbi.Clzzrq = DateTime.Parse(this.dtclzzrq.Text);
                lbi.Tymc = this.tbtymc.Text.Trim();
                lbi.Yyc = this.cbyyc.Text.Trim();
                lbi.Zwps = this.tbzwps.Text.Trim();
                lbi.Zdsjzzl = this.tbzdsjzzl.Text.Trim();
                lbi.Edzk = this.tbedzk.Text.Trim();
                lbi.Lj = this.tblj.Text.Trim();
                lbi.Qdxs = this.cbqdxs.Text.Trim();
                lbi.Jyjgmc = this.tbjcjgmc.Text.Trim();
                lbi.Jybgbh = this.tbbgbh.Text.Trim();
                lbi.Qtxx = this.tbQtxx.Text.Trim();
                lbi.CreateTime = DateTime.Now;
                lbi.UpdateTime = DateTime.Now;
                lbi.Status = "0";

             

                List<RllxParamEntity> listParam = new List<RllxParamEntity>();
                foreach (Control c in this.tlp.Controls)
                {
                    if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        RllxParamEntity pe = new RllxParamEntity();
                        string strLabName = "lbl" + c.Name;
                        Control[] lblc = this.Controls.Find(strLabName, true);
                        string paramCode = c.Name;
                        string paramValue = c.Text;
                        //string paramName = lblc[0].Text;

                        pe.V_Id = "";
                        pe.Param_Code = paramCode;
                        pe.Vin = this.tbbah.Text.Trim().ToUpper();
                        //pe.Param_Name = paramName;
                        pe.Param_Value = c.Text;

                        listParam.Add(pe);
                    }
                }
                lbi.EntityList = listParam.ToArray();
                lbiList.Add(lbi);
                
                // 上报
                FuelDataService.OperateResult result = new FuelDataService.OperateResult();
                return result = service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");
                
                //// 判断是否上报成功
                //int count = Utils.ApplyFlg(result).Count;
                //if (count > 0)
                //{
                //    AppFlg = false;
                //    this.btnPrint.Visible = false;
                //}
                //else 
                //{
                //    Utils.UpdataState(result);
                //    setEnable();
                //    AppFlg = true;

                    
                //}
                //return AppFlg;
            }
            catch (Exception ex) 
            {
                throw ex;
                return null;
            }
        }

        // 上报成功，返回本地数据库的VID
        public string GetSussVID(string strBah)
        {
            string strVid = "";
            string strSql = "SELECT V_ID FROM FC_CLJBXX WHERE VIN ='" + strBah.ToUpper() + "'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, strSql, null);
            if (ds != null) 
            {
                strVid = ds.Tables[0].Rows[0]["V_ID"].ToString();
            }
            return strVid;
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
            PrintForm pf = new PrintForm();
            pf.ShowDialog();
        }

        public List<PrintModel> GetPrintData(string strVin) 
        {
            PrintModel printModel = new PrintModel();
            string strJbSql = "SELECT * FROM FC_CLJBXX WHERE VIN ='" + strVin.ToUpper() + "' AND STATUS = '0'";
            DataSet dsJb = AccessHelper.ExecuteDataSet(AccessHelper.conn, strJbSql, null);

            if (dsJb.Tables[0].Rows.Count > 0)
            {
                PrintForm pf = new PrintForm();
                printModel.Qcscqy = dsJb.Tables[0].Rows[0]["QCSCQY"].ToString() == "" ? dsJb.Tables[0].Rows[0]["JKQCZJXS"].ToString() : dsJb.Tables[0].Rows[0]["QCSCQY"].ToString();
                printModel.Clxh = dsJb.Tables[0].Rows[0]["CLXH"].ToString();
                printModel.Zczbzl = dsJb.Tables[0].Rows[0]["ZCZBZL"].ToString();
                string strRllx = dsJb.Tables[0].Rows[0]["RLLX"].ToString(); ;
                
                printModel.Qdxs = dsJb.Tables[0].Rows[0]["QDXS"].ToString();
                printModel.Zdsjzzl = dsJb.Tables[0].Rows[0]["ZDSJZZL"].ToString();

                if (strRllx == "汽油" || strRllx == "柴油" || strRllx == "两用燃料" || strRllx == "双燃料" || strRllx == "气体燃料")
                {
                    try
                    {
                        string strPamSql = "SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='" + strVin.ToUpper() + "'";
                        DataSet dsPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, strPamSql, null);
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
                if (strRllx == "插电式混合动力")
                {
                    try
                    {
                        string strPamSql = "SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='" + strVin.ToUpper() + "'";
                        DataSet dsPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, strPamSql, null);
                        for (int i = 0; i < dsPam.Tables[0].Rows.Count; i++)
                        {
                            string strCode = dsPam.Tables[0].Rows[i]["PARAM_CODE"].ToString();
                            string strValue = dsPam.Tables[0].Rows[i]["PARAM_VALUE"].ToString();
                            if (strCode == "CDS_HHDL_FDJXH")
                            {
                                printModel.Fdjxh = strValue;
                            }
                            if (strCode == "CDS_HHDL_PL")
                            {
                                printModel.Pl = strValue;
                            }
                            if (strCode == "CDS_HHDL_BSQXS")
                            {
                                printModel.Bsqlx = strValue;
                            }
                            if (strCode == "CT_QTXX")
                            {
                                printModel.Qtxx = strValue;
                            }
                            if (strCode == "CDS_HHDL_EDGL")
                            {
                                printModel.Edgl = strValue;
                            }
                            if (strCode == "CDS_HHDL_ZHGKRLXHL")
                            {
                                printModel.Zh = strValue;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (strRllx == "非插电式混合动力")
                {
                    try
                    {
                        string strPamSql = "SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='" + strVin.ToUpper() + "'";
                        DataSet dsPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, strPamSql, null);
                        for (int i = 0; i < dsPam.Tables[0].Rows.Count; i++)
                        {
                            string strCode = dsPam.Tables[0].Rows[i]["PARAM_CODE"].ToString();
                            string strValue = dsPam.Tables[0].Rows[i]["PARAM_VALUE"].ToString();
                            if (strCode == "FCDS_HHDL_FDJXH")
                            {
                                printModel.Fdjxh = strValue;
                            }
                            if (strCode == "FCDS_HHDL_PL")
                            {
                                printModel.Pl = strValue;
                            }
                            if (strCode == "FCDS_HHDL_BSQXS")
                            {
                                printModel.Bsqlx = strValue;
                            }
                            if (strCode == "CT_QTXX")
                            {
                                printModel.Qtxx = strValue;
                            }
                            if (strCode == "FCDS_HHDL_EDGL")
                            {
                                printModel.Edgl = strValue;
                            }
                            if (strCode == "FCDS_HHDL_SJGKRLXHL")
                            {
                                printModel.Sj = strValue;
                            }
                            if (strCode == "FCDS_HHDL_SQGKRLXHL")
                            {
                                printModel.Sq = strValue;
                            }
                            if (strCode == "FCDS_HHDL_ZHGKRLXHL")
                            {
                                printModel.Zh = strValue;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (strRllx == "纯电动")
                {
                    try
                    {
                        string strPamSql = "SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='" + strVin.ToUpper() + "'";
                        DataSet dsPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, strPamSql, null);
                        for (int i = 0; i < dsPam.Tables[0].Rows.Count; i++)
                        {
                            string strCode = dsPam.Tables[0].Rows[i]["PARAM_CODE"].ToString();
                            string strValue = dsPam.Tables[0].Rows[i]["PARAM_VALUE"].ToString();
                            if (strCode == "CT_QTXX")
                            {
                                printModel.Qtxx = strValue;
                            }
                            if (strCode == "CDD_QDDJEDGL")
                            {
                                printModel.Edgl = strValue;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (strRllx == "燃料电池")
                {
                    try
                    {
                        string strPamSql = "SELECT A.PARAM_VALUE,A.PARAM_CODE,B.PARAM_NAME FROM RLLX_PARAM_ENTITY A, RLLX_PARAM B WHERE A.PARAM_CODE = B.PARAM_CODE AND A.VIN ='" + strVin.ToUpper() + "'";
                        DataSet dsPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, strPamSql, null);
                        for (int i = 0; i < dsPam.Tables[0].Rows.Count; i++)
                        {
                            string strCode = dsPam.Tables[0].Rows[i]["PARAM_CODE"].ToString();
                            string strValue = dsPam.Tables[0].Rows[i]["PARAM_VALUE"].ToString();
                            if (strCode == "CT_QTXX")
                            {
                                printModel.Qtxx = strValue;
                            }
                            if (strCode == "RLDC_QDDJEDGL")
                            {
                                printModel.Edgl = strValue;
                            }
                            if (strCode == "RLDC_ZHGKHQL")
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
                printModel.Bah = dsJb.Tables[0].Rows[0]["VIN"].ToString();
                printModel.Qysj = DateTime.Now.ToShortDateString();
            }
            List<PrintModel> printModelList = new List<PrintModel>();
            printModelList.Add(printModel);
            return printModelList;
        }

        public bool GetSaveFlg(string strBah) 
        {
            bool flgSave = false;
            string sqlJbxx = "SELECT * FROM FC_CLJBXX WHERE VIN = '" + strBah + "' AND STATUS='1'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlJbxx, null);
            if (ds.Tables[0].Rows.Count > 0) 
            {
                flgSave = true;
            }
            return flgSave;
        }
        
        private void tbbah_Validating(object sender, CancelEventArgs e)
        {
            char bi;
            DataCheck dc = new DataCheck();
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
                    this.dxErrorProvider.SetError(tbbah, "【备案号(VIN)】校验失败！第9位应为:'" + bi + "'");
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
            else if (!this.VerifyParamFormat(this.tblj.Text.Trim(), '/') || this.tblj.Text.Trim().IndexOf('/') < 0 || !(tblj.Text.Split('/').Length - 1 == 1))
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
            else if (!this.IsInt(this.tbzgcs.Text.Trim()))
            {
                this.dxErrorProvider.SetError(tbzgcs, "应填写整数");
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
            else if (!this.VerifyParamFormat(this.tbzczbzl.Text.Trim(),','))
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
                msg += "乘用车生产企业不能为空\r\n";
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
                msg += "产品型号不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.dtclzzrq.Text.Trim()))
            {
                msg += "车辆制造日期/进口核销日期不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbclzl.Text.Trim()))
            {
                msg += "车辆类型不能为空\r\n";
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
                msg += "整备质量不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzwps.Text.Trim()))
            {
                msg += "座椅排数不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzgcs.Text.Trim()))
            {
                msg += "最高车速不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tbzdsjzzl.Text.Trim()))
            {
                msg += "总质量不能为空\r\n";
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
                msg += "前轮距/后轮距不能为空\r\n";
            }
            //if (string.IsNullOrEmpty(this.tbQtxx.Text.Trim()))
            //{
            //    msg += "其他信息不能为空\r\n";
            //}
            if (string.IsNullOrEmpty(this.cbrllx.Text.Trim()))
            {
                msg += "燃料种类不能为空\r\n";
            }
            if (!string.IsNullOrEmpty(msg))
            {
                msg = "\r\n" + msg;
            }
            return msg;
        }
    }
}