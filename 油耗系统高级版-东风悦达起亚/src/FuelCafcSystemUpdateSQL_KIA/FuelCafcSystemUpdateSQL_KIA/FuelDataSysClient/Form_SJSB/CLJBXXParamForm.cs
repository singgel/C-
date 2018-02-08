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
using System.Text;
using System.Globalization;
using FuelDataSysClient.Tool;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class CLJBXXParamForm : Form
    {
        private string SC_OCN = string.Empty;

        public CLJBXXParamForm()
        {
            InitializeComponent();
        }

        private void CLJBXXParamForm_Load(object sender, EventArgs e)
        {
            this.SC_OCN = this.teSC_OCN.Text;
            this.teQCSCQY.Text = Utils.qymc;
            this.cbeRLLX.Properties.Items.AddRange(Utils.GetFuelType(string.Empty).ToArray());
        }

        // 确定按钮
        private void btnYES_Click(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                // 校验页面参数规则
                if (this.dxErrorProvider1.HasErrors)
                {
                    MessageBox.Show("请核对页面信息是否填写正确！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string msg = VerifyRequired();
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.Text.Equals("整车数据新增"))
                {
                    AddCljbxxParam();
                }
                if (this.Text.Equals("整车数据复制"))
                {
                    // 校验生产OCN是否改变
                    if (this.SC_OCN.Equals(this.teSC_OCN.Text.Trim()))
                    {
                        MessageBox.Show(String.Format("OCN编号：{0}复制失败！", this.SC_OCN), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    CopyCljbxxParam();
                }
                if (this.Text.Equals("整车数据修改"))
                {
                    // 校验页面参数是否改变
                    if (this.VerifyRepeat())
                    {
                        MessageBox.Show(String.Format("OCN编号：{0}修改失败！", this.SC_OCN), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    UpdateCljbxxParam();
                }
                MessageBox.Show("整车数据操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("整车数据操作异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        // 取消按钮
        private void btnNO_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 新增整车数据
        private void AddCljbxxParam()
        {
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        OracleParameter[] parameters = 
                                        {
				                            new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("XT_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("MI_XT_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("TYMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLXH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("PFBZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("SFJKQC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QCSCQY", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JKQCZJXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JCJGMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BGBH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BAH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLZZRQ", OracleDbType.Date),
				                            new OracleParameter("CLZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YYC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QDXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZWPS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZGCS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("EDZK", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LTGG", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YHDYBAH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZCZBZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZDSJZZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZHGKRLXHL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLXHLMBZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JDBZMBZ4", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BSQXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("PL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CDDQDMSZHGKXHLC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                            new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                            new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("VERSION", OracleDbType.Int32),
                                        };
                        parameters[0].Value = this.teSC_OCN.Text;
                        parameters[1].Value = this.teXT_OCN.Text;
                        parameters[2].Value = this.teMI_XT_OCN.Text;
                        parameters[3].Value = this.teTYMC.Text;
                        parameters[4].Value = this.teCLXH.Text;
                        parameters[5].Value = this.tePFBZ.Text;
                        parameters[6].Value = this.cbeSFJKQC.Text;
                        parameters[7].Value = this.teQCSCQY.Text;
                        parameters[8].Value = this.teJKQCZJXS.Text;
                        parameters[9].Value = this.teJCJGMC.Text;
                        parameters[10].Value = this.teBGBH.Text;
                        parameters[11].Value = this.teBAH.Text;
                        if (string.IsNullOrEmpty(this.dtCLZZRQ.Text.Trim()))
                        {
                            parameters[12].Value = null;
                        }
                        else
                        {
                            parameters[12].Value = Convert.ToDateTime(this.dtCLZZRQ.Text);
                        }
                        parameters[13].Value = this.cbeCLZL.Text;
                        parameters[14].Value = this.cbeYYC.Text;
                        parameters[15].Value = this.cbeQDXS.Text;
                        parameters[16].Value = this.teZWPS.Text;
                        parameters[17].Value = this.teZGCS.Text;
                        parameters[18].Value = this.teEDZK.Text;
                        parameters[19].Value = this.teLTGG.Text;
                        parameters[20].Value = this.teLJ.Text;
                        parameters[21].Value = this.teZJ.Text;
                        parameters[22].Value = this.cbeRLLX.Text;
                        parameters[23].Value = this.teYHDYBAH.Text;
                        parameters[24].Value = this.teZCZBZL.Text;
                        parameters[25].Value = this.teZDSJZZL.Text;
                        parameters[26].Value = this.teZHGKRLXHL.Text;
                        parameters[27].Value = this.teRLXHLMBZ.Text;
                        parameters[28].Value = this.teJDBZMBZ4.Text;
                        parameters[29].Value = this.cbeBSQXS.Text;
                        parameters[30].Value = this.tePL.Text;
                        parameters[31].Value = this.teCDDQDMSZHGKXHLC.Text;
                        parameters[32].Value = "1";
                        parameters[33].Value = System.DateTime.Today;
                        parameters[34].Value = Utils.localUserId;
                        parameters[35].Value = System.DateTime.Today;
                        parameters[36].Value = Utils.localUserId;
                        parameters[37].Value = 0;
                        OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_CLJBXX (SC_OCN,XT_OCN,MI_XT_OCN,TYMC,CLXH,PFBZ,SFJKQC,QCSCQY,JKQCZJXS,JCJGMC,BGBH,BAH,CLZZRQ,CLZL,YYC,QDXS,ZWPS,ZGCS,EDZK,LTGG,LJ,ZJ,RLLX,YHDYBAH,ZCZBZL,ZDSJZZL,ZHGKRLXHL,RLXHLMBZ,JDBZMBZ4,BSQXS,PL,CDDQDMSZHGKXHLC,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:XT_OCN,:MI_XT_OCN,:TYMC,:CLXH,:PFBZ,:SFJKQC,:QCSCQY,:JKQCZJXS,:JCJGMC,:BGBH,:BAH,:CLZZRQ,:CLZL,:YYC,:QDXS,:ZWPS,:ZGCS,:EDZK,:LTGG,:LJ,:ZJ,:RLLX,:YHDYBAH,:ZCZBZL,:ZDSJZZL,:ZHGKRLXHL,:RLXHLMBZ,:JDBZMBZ4,:BSQXS,:PL,:CDDQDMSZHGKXHLC,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
        }

        // 复制整车基础数据
        private void CopyCljbxxParam()
        {
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        OracleParameter[] parameters = 
                                        {
				                            new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("XT_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("MI_XT_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("TYMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLXH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("PFBZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("SFJKQC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QCSCQY", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JKQCZJXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JCJGMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BGBH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BAH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLZZRQ", OracleDbType.Date),
				                            new OracleParameter("CLZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YYC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QDXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZWPS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZGCS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("EDZK", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LTGG", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YHDYBAH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZCZBZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZDSJZZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZHGKRLXHL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLXHLMBZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JDBZMBZ4", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BSQXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("PL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CDDQDMSZHGKXHLC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                            new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                            new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("VERSION", OracleDbType.Int32),
                                        };
                        parameters[0].Value = this.teSC_OCN.Text;
                        parameters[1].Value = this.teXT_OCN.Text;
                        parameters[2].Value = this.teMI_XT_OCN.Text;
                        parameters[3].Value = this.teTYMC.Text;
                        parameters[4].Value = this.teCLXH.Text;
                        parameters[5].Value = this.tePFBZ.Text;
                        parameters[6].Value = this.cbeSFJKQC.Text;
                        parameters[7].Value = this.teQCSCQY.Text;
                        parameters[8].Value = this.teJKQCZJXS.Text;
                        parameters[9].Value = this.teJCJGMC.Text;
                        parameters[10].Value = this.teBGBH.Text;
                        parameters[11].Value = this.teBAH.Text;
                        if (string.IsNullOrEmpty(this.dtCLZZRQ.Text.Trim()))
                        {
                            parameters[12].Value = null;
                        }
                        else
                        {
                            parameters[12].Value = Convert.ToDateTime(this.dtCLZZRQ.Text);
                        }
                        parameters[13].Value = this.cbeCLZL.Text;
                        parameters[14].Value = this.cbeYYC.Text;
                        parameters[15].Value = this.cbeQDXS.Text;
                        parameters[16].Value = this.teZWPS.Text;
                        parameters[17].Value = this.teZGCS.Text;
                        parameters[18].Value = this.teEDZK.Text;
                        parameters[19].Value = this.teLTGG.Text;
                        parameters[20].Value = this.teLJ.Text;
                        parameters[21].Value = this.teZJ.Text;
                        parameters[22].Value = this.cbeRLLX.Text;
                        parameters[23].Value = this.teYHDYBAH.Text;
                        parameters[24].Value = this.teZCZBZL.Text;
                        parameters[25].Value = this.teZDSJZZL.Text;
                        parameters[26].Value = this.teZHGKRLXHL.Text;
                        parameters[27].Value = this.teRLXHLMBZ.Text;
                        parameters[28].Value = this.teJDBZMBZ4.Text;
                        parameters[29].Value = this.cbeBSQXS.Text;
                        parameters[30].Value = this.tePL.Text;
                        parameters[31].Value = this.teCDDQDMSZHGKXHLC.Text;
                        parameters[32].Value = "2";
                        parameters[33].Value = System.DateTime.Today;
                        parameters[34].Value = Utils.localUserId;
                        parameters[35].Value = System.DateTime.Today;
                        parameters[36].Value = Utils.localUserId;
                        parameters[37].Value = 0;
                        OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_CLJBXX (SC_OCN,XT_OCN,MI_XT_OCN,TYMC,CLXH,PFBZ,SFJKQC,QCSCQY,JKQCZJXS,JCJGMC,BGBH,BAH,CLZZRQ,CLZL,YYC,QDXS,ZWPS,ZGCS,EDZK,LTGG,LJ,ZJ,RLLX,YHDYBAH,ZCZBZL,ZDSJZZL,ZHGKRLXHL,RLXHLMBZ,JDBZMBZ4,BSQXS,PL,CDDQDMSZHGKXHLC,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:XT_OCN,:MI_XT_OCN,:TYMC,:CLXH,:PFBZ,:SFJKQC,:QCSCQY,:JKQCZJXS,:JCJGMC,:BGBH,:BAH,:CLZZRQ,:CLZL,:YYC,:QDXS,:ZWPS,:ZGCS,:EDZK,:LTGG,:LJ,:ZJ,:RLLX,:YHDYBAH,:ZCZBZL,:ZDSJZZL,:ZHGKRLXHL,:RLXHLMBZ,:JDBZMBZ4,:BSQXS,:PL,:CDDQDMSZHGKXHLC,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
        }

        // 更新整车基础数据
        private void UpdateCljbxxParam()
        {
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string version = OracleHelper.ExecuteScalar(trans, string.Format("SELECT MIN(VERSION) FROM OCN_CLJBXX WHERE OPERATION='4' AND SC_OCN='{0}'", this.SC_OCN)).ToString();
                        int versionNew = string.IsNullOrEmpty(version) ? 0 : Convert.ToInt32(version) - 1;
                        OracleHelper.ExecuteNonQuery(trans, string.Format("UPDATE OCN_CLJBXX SET OPERATION = '4',VERSION = '{0}' WHERE SC_OCN='{1}' AND OPERATION = '{2}' AND VERSION={3} ", versionNew, this.SC_OCN, Regex.Match(this.labOPERATION.Text, @"(\d+)").Groups[1].Value, Regex.Match(this.labVERSION.Text, @"(\d+)").Groups[1].Value));
                        OracleParameter[] parameters = 
                                        {
				                            new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("XT_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("MI_XT_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("TYMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLXH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("PFBZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("SFJKQC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QCSCQY", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JKQCZJXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JCJGMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BGBH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BAH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLZZRQ", OracleDbType.Date),
				                            new OracleParameter("CLZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YYC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QDXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZWPS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZGCS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("EDZK", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LTGG", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YHDYBAH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZCZBZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZDSJZZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZHGKRLXHL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLXHLMBZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JDBZMBZ4", OracleDbType.NVarchar2,255),
				                            new OracleParameter("BSQXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("PL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CDDQDMSZHGKXHLC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                            new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                            new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("VERSION", OracleDbType.Int32),
                                        };
                        parameters[0].Value = this.teSC_OCN.Text;
                        parameters[1].Value = this.teXT_OCN.Text;
                        parameters[2].Value = this.teMI_XT_OCN.Text;
                        parameters[3].Value = this.teTYMC.Text;
                        parameters[4].Value = this.teCLXH.Text;
                        parameters[5].Value = this.tePFBZ.Text;
                        parameters[6].Value = this.cbeSFJKQC.Text;
                        parameters[7].Value = this.teQCSCQY.Text;
                        parameters[8].Value = this.teJKQCZJXS.Text;
                        parameters[9].Value = this.teJCJGMC.Text;
                        parameters[10].Value = this.teBGBH.Text;
                        parameters[11].Value = this.teBAH.Text;
                        if (string.IsNullOrEmpty(this.dtCLZZRQ.Text.Trim()))
                        {
                            parameters[12].Value = null;
                        }
                        else
                        {
                            parameters[12].Value = Convert.ToDateTime(this.dtCLZZRQ.Text);
                        }
                        parameters[13].Value = this.cbeCLZL.Text;
                        parameters[14].Value = this.cbeYYC.Text;
                        parameters[15].Value = this.cbeQDXS.Text;
                        parameters[16].Value = this.teZWPS.Text;
                        parameters[17].Value = this.teZGCS.Text;
                        parameters[18].Value = this.teEDZK.Text;
                        parameters[19].Value = this.teLTGG.Text;
                        parameters[20].Value = this.teLJ.Text;
                        parameters[21].Value = this.teZJ.Text;
                        parameters[22].Value = this.cbeRLLX.Text;
                        parameters[23].Value = this.teYHDYBAH.Text;
                        parameters[24].Value = this.teZCZBZL.Text;
                        parameters[25].Value = this.teZDSJZZL.Text;
                        parameters[26].Value = this.teZHGKRLXHL.Text;
                        parameters[27].Value = this.teRLXHLMBZ.Text;
                        parameters[28].Value = this.teJDBZMBZ4.Text;
                        parameters[29].Value = this.cbeBSQXS.Text;
                        parameters[30].Value = this.tePL.Text;
                        parameters[31].Value = this.teCDDQDMSZHGKXHLC.Text;
                        parameters[32].Value = "3";
                        parameters[33].Value = System.DateTime.Today;
                        parameters[34].Value = Utils.localUserId;
                        parameters[35].Value = System.DateTime.Today;
                        parameters[36].Value = Utils.localUserId;
                        parameters[37].Value = Convert.ToInt32(Regex.Match(this.labVERSION.Text, @"(\d+)").Groups[1].Value) + 1;
                        OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_CLJBXX (SC_OCN,XT_OCN,MI_XT_OCN,TYMC,CLXH,PFBZ,SFJKQC,QCSCQY,JKQCZJXS,JCJGMC,BGBH,BAH,CLZZRQ,CLZL,YYC,QDXS,ZWPS,ZGCS,EDZK,LTGG,LJ,ZJ,RLLX,YHDYBAH,ZCZBZL,ZDSJZZL,ZHGKRLXHL,RLXHLMBZ,JDBZMBZ4,BSQXS,PL,CDDQDMSZHGKXHLC,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:XT_OCN,:MI_XT_OCN,:TYMC,:CLXH,:PFBZ,:SFJKQC,:QCSCQY,:JKQCZJXS,:JCJGMC,:BGBH,:BAH,:CLZZRQ,:CLZL,:YYC,:QDXS,:ZWPS,:ZGCS,:EDZK,:LTGG,:LJ,:ZJ,:RLLX,:YHDYBAH,:ZCZBZL,:ZDSJZZL,:ZHGKRLXHL,:RLXHLMBZ,:JDBZMBZ4,:BSQXS,:PL,:CDDQDMSZHGKXHLC,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
        }

        private void teTYMC_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teTYMC.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teTYMC, DataVerifyHelper.VerifyRequired(string.Empty, this.teTYMC.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teTYMC, "");
            }
        }

        private void teCLXH_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teCLXH.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teCLXH, DataVerifyHelper.VerifyRequired(string.Empty, this.teCLXH.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teCLXH, "");
            }
        }

        private void tePFBZ_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.tePFBZ.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(tePFBZ, DataVerifyHelper.VerifyRequired(string.Empty, this.tePFBZ.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(tePFBZ, "");
            }
        }

        private void teMI_XT_OCN_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teMI_XT_OCN.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teMI_XT_OCN, DataVerifyHelper.VerifyRequired(string.Empty, this.teMI_XT_OCN.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teMI_XT_OCN, "");
            }
        }

        private void teXT_OCN_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teXT_OCN.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teXT_OCN, DataVerifyHelper.VerifyRequired(string.Empty, this.teXT_OCN.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teXT_OCN, "");
            }
        }

        private void teSC_OCN_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teSC_OCN.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teSC_OCN, DataVerifyHelper.VerifyRequired(string.Empty, this.teSC_OCN.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teSC_OCN, "");
            }
        }

        private void cbeSFJKQC_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.cbeSFJKQC.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(cbeSFJKQC, DataVerifyHelper.VerifyRequired(string.Empty, this.cbeSFJKQC.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(cbeSFJKQC, "");
            }
        }

        private void teJDBZMBZ4_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teJDBZMBZ4.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teJDBZMBZ4, DataVerifyHelper.VerifyRequired(string.Empty, this.teJDBZMBZ4.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyFloat(string.Empty, this.teJDBZMBZ4.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teJDBZMBZ4, DataVerifyHelper.VerifyFloat(string.Empty, this.teJDBZMBZ4.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teJDBZMBZ4, "");
            }
        }

        private void teQCSCQY_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teQCSCQY.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teQCSCQY, DataVerifyHelper.VerifyRequired(string.Empty, this.teQCSCQY.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teQCSCQY, "");
            }
        }

        private void teJKQCZJXS_Validating(object sender, CancelEventArgs e)
        {
            if (Utils.userId.Substring(4, 1).Equals("F") && !string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teJKQCZJXS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teJKQCZJXS, DataVerifyHelper.VerifyRequired(string.Empty, this.teJKQCZJXS.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teJKQCZJXS, "");
            }
        }

        private void teJCJGMC_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teJCJGMC.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teJCJGMC, DataVerifyHelper.VerifyRequired(string.Empty, this.teJCJGMC.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teJCJGMC, "");
            }
        }

        private void teBGBH_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teBGBH.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teBGBH, DataVerifyHelper.VerifyRequired(string.Empty, this.teBGBH.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teBGBH, "");
            }
        }

        private void teBAH_Validating(object sender, CancelEventArgs e)
        {
            //if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teBAH.Text.Trim())))
            //{
            //    this.dxErrorProvider1.SetError(teBAH, DataVerifyHelper.VerifyRequired(string.Empty, this.teBAH.Text.Trim()).TrimStart('\n'));
            //}
            //else
            //{
            //    dxErrorProvider1.SetError(teBAH, "");
            //}
        }

        private void dtCLZZRQ_Validating(object sender, CancelEventArgs e)
        {
            //if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.dtCLZZRQ.Text.Trim())))
            //{
            //    this.dxErrorProvider1.SetError(dtCLZZRQ, DataVerifyHelper.VerifyRequired(string.Empty, this.dtCLZZRQ.Text.Trim()).TrimStart('\n'));
            //}
            //else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyDateTime(string.Empty, this.dtCLZZRQ.Text.Trim())))
            //{
            //    this.dxErrorProvider1.SetError(dtCLZZRQ, DataVerifyHelper.VerifyDateTime(string.Empty, this.dtCLZZRQ.Text.Trim()).TrimStart('\n'));
            //}
            //else
            //{
            //    dxErrorProvider1.SetError(dtCLZZRQ, "");
            //}
        }

        private void cbeCLZL_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.cbeCLZL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(cbeCLZL, DataVerifyHelper.VerifyRequired(string.Empty, this.cbeCLZL.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(cbeCLZL, "");
            }
        }

        private void cbeBSQXS_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.cbeBSQXS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(cbeBSQXS, DataVerifyHelper.VerifyRequired(string.Empty, this.cbeBSQXS.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(cbeBSQXS, "");
            }
        }

        private void cbeYYC_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.cbeYYC.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(cbeYYC, DataVerifyHelper.VerifyRequired(string.Empty, this.cbeYYC.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(cbeYYC, "");
            }
        }

        private void cbeQDXS_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.cbeQDXS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(cbeQDXS, DataVerifyHelper.VerifyRequired(string.Empty, this.cbeQDXS.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(cbeQDXS, "");
            }
        }

        private void teZWPS_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teZWPS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZWPS, DataVerifyHelper.VerifyRequired(string.Empty, this.teZWPS.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyInt(string.Empty, this.teZWPS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZWPS, DataVerifyHelper.VerifyInt(string.Empty, this.teZWPS.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teZWPS, "");
            }
        }

        private void teZGCS_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teZGCS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZGCS, DataVerifyHelper.VerifyRequired(string.Empty, this.teZGCS.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyInt(string.Empty, this.teZGCS.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZGCS, DataVerifyHelper.VerifyInt(string.Empty, this.teZGCS.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teZGCS, "");
            }
        }

        private void teEDZK_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teEDZK.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teEDZK, DataVerifyHelper.VerifyRequired(string.Empty, this.teEDZK.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyInt(string.Empty, this.teEDZK.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teEDZK, DataVerifyHelper.VerifyInt(string.Empty, this.teEDZK.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teEDZK, "");
            }
        }

        private void teLTGG_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teLTGG.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teLTGG, DataVerifyHelper.VerifyRequired(string.Empty, this.teLTGG.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teLTGG, "");
            }
        }

        private void teLJ_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teLJ.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teLJ, DataVerifyHelper.VerifyRequired(string.Empty, this.teLJ.Text.Trim()).TrimStart('\n'));
            }
            else if (this.teLJ.Text.Trim().IndexOf('/') < 0 || this.teLJ.Text.Trim().IndexOf('/').Equals(this.teLJ.Text.Trim().Length - 1))
            {
                this.dxErrorProvider1.SetError(teLJ,  "轮距（前/后）(mm)，中间用”/”隔开");
            }
            else
            {
                dxErrorProvider1.SetError(teLJ, "");
            }
        }

        private void tePL_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.tePL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(tePL, DataVerifyHelper.VerifyRequired(string.Empty, this.tePL.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(tePL, "");
            }
        }

        private void teZJ_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teZJ.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZJ, DataVerifyHelper.VerifyRequired(string.Empty, this.teZJ.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyInt(string.Empty, this.teZJ.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZJ, DataVerifyHelper.VerifyInt(string.Empty, this.teZJ.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teZJ, "");
            }
        }

        private void cbeRLLX_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.cbeRLLX.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(cbeRLLX, DataVerifyHelper.VerifyRequired(string.Empty, this.cbeRLLX.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(cbeRLLX, "");
            }
        }

        private void teYHDYBAH_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teYHDYBAH.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teYHDYBAH, DataVerifyHelper.VerifyRequired(string.Empty, this.teYHDYBAH.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teYHDYBAH, "");
            }
        }

        private void teZCZBZL_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teZCZBZL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZCZBZL, DataVerifyHelper.VerifyRequired(string.Empty, this.teZCZBZL.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyInt(string.Empty, this.teZCZBZL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZCZBZL, DataVerifyHelper.VerifyInt(string.Empty, this.teZCZBZL.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teZCZBZL, "");
            }
        }

        private void teZDSJZZL_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teZDSJZZL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZDSJZZL, DataVerifyHelper.VerifyRequired(string.Empty, this.teZDSJZZL.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyInt(string.Empty, this.teZDSJZZL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZDSJZZL, DataVerifyHelper.VerifyInt(string.Empty, this.teZDSJZZL.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teZDSJZZL, "");
            }
        }

        private void teZHGKRLXHL_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teZHGKRLXHL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZHGKRLXHL, DataVerifyHelper.VerifyRequired(string.Empty, this.teZHGKRLXHL.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyFloat(string.Empty, this.teZHGKRLXHL.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teZHGKRLXHL, DataVerifyHelper.VerifyFloat(string.Empty, this.teZHGKRLXHL.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teZHGKRLXHL, "");
            }
        }

        private void teRLXHLMBZ_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teRLXHLMBZ.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teRLXHLMBZ, DataVerifyHelper.VerifyRequired(string.Empty, this.teRLXHLMBZ.Text.Trim()).TrimStart('\n'));
            }
            else if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyFloat(string.Empty, this.teRLXHLMBZ.Text.Trim())))
            {
                this.dxErrorProvider1.SetError(teRLXHLMBZ, DataVerifyHelper.VerifyFloat(string.Empty, this.teRLXHLMBZ.Text.Trim()).TrimStart('\n'));
            }
            else
            {
                dxErrorProvider1.SetError(teRLXHLMBZ, "");
            }
        }

        private void teCDDQDMSZHGKXHLC_Validating(object sender, CancelEventArgs e)
        {
            //if (!string.IsNullOrEmpty(DataVerifyHelper.VerifyRequired(string.Empty, this.teCDDQDMSZHGKXHLC.Text.Trim())))
            //{
            //    this.dxErrorProvider1.SetError(teCDDQDMSZHGKXHLC, DataVerifyHelper.VerifyRequired(string.Empty, this.teCDDQDMSZHGKXHLC.Text.Trim()).TrimStart('\n'));
            //}
            //else
            //{
            //    dxErrorProvider1.SetError(teCDDQDMSZHGKXHLC, "");
            //}
        }

        // 校验数据是否更改
        private bool VerifyRepeat()
        {
            var dtOriginal = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("Select * From OCN_CLJBXX Where SC_OCN='{0}'", this.SC_OCN), null).Tables[0];
            foreach (DataColumn dc in dtOriginal.Columns)
            {
                if (dc.ColumnName.Equals("OPERATION") || dc.ColumnName.Equals("CREATE_TIME") || dc.ColumnName.Equals("CREATE_ROLE") || dc.ColumnName.Equals("UPDATE_TIME") || dc.ColumnName.Equals("UPDATE_ROLE")) continue;
                var teControls = this.Controls.Find("te" + dc.ColumnName, true);
                var cbeControls = this.Controls.Find("cbe" + dc.ColumnName, true);
                var dtControls = this.Controls.Find("dt" + dc.ColumnName, true);
                if ((teControls.Length == 1 && teControls[0].Text.Trim().Equals(dtOriginal.Rows[0][dc.ColumnName].ToString())) || (cbeControls.Length == 1 && cbeControls[0].Text.Trim().Equals(dtOriginal.Rows[0][dc.ColumnName].ToString())) || (dtControls.Length == 1 && dtControls[0].Text.Trim().Equals(string.IsNullOrEmpty(dtOriginal.Rows[0][dc.ColumnName].ToString()) == true ? string.Empty : Convert.ToDateTime(dtOriginal.Rows[0][dc.ColumnName].ToString()).ToString("yyyy/MM/dd"))))
                {
                    continue;
                }
                else
                {
                    //Console.WriteLine(dc.ColumnName + "-" + teControls.Length + ":" + (teControls.Length > 0 ? (dtOriginal.Rows[0][dc.ColumnName].ToString() + "~" + teControls[0].Text.Trim()) : string.Empty));
                    //Console.WriteLine(dc.ColumnName + "-" + cbeControls.Length + ":" + (cbeControls.Length > 0 ? (dtOriginal.Rows[0][dc.ColumnName].ToString() + "~" + cbeControls[0].Text.Trim()) : string.Empty));
                    //Console.WriteLine(dc.ColumnName + "-" + dtControls.Length + ":" + (dtControls.Length > 0 ? (string.IsNullOrEmpty(dtOriginal.Rows[0][dc.ColumnName].ToString()) == true ? string.Empty : Convert.ToDateTime(dtOriginal.Rows[0][dc.ColumnName].ToString()).ToString("yyyy/MM/dd") + "~" + dtControls[0].Text.Trim()) : string.Empty));
                    return false;
                }
            }
            return true;
        }

        // 校验必填数据
        private string VerifyRequired()
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(this.teTYMC.Text.Trim()))
            {
                msg += "通用名称不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teCLXH.Text.Trim()))
            {
                msg += "车辆型号不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.tePFBZ.Text.Trim()))
            {
                msg += "排放标准不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teMI_XT_OCN.Text.Trim()))
            {
                msg += "MI+系统OCN不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teXT_OCN.Text.Trim()))
            {
                msg += "系统OCN不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teSC_OCN.Text.Trim()))
            {
                msg += "生产OCN不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbeSFJKQC.Text.Trim()))
            {
                msg += "是否进口汽车不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teJDBZMBZ4.Text.Trim()))
            {
                msg += "4阶段标准目标值不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teQCSCQY.Text.Trim()))
            {
                msg += "汽车生产企业不能为空\r\n";
            }
            if (Utils.userId.Substring(4, 1).Equals("F") && string.IsNullOrEmpty(this.teJKQCZJXS.Text.Trim()))
            {
                msg += "进口汽车总经销商不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teJCJGMC.Text.Trim()))
            {
                msg += "检测机构名称不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teBGBH.Text.Trim()))
            {
                msg += "报告编号不能为空\r\n";
            }
            //if (string.IsNullOrEmpty(this.teBAH.Text.Trim()))
            //{
            //    msg += "备案号不能为空\r\n";
            //}
            //if (string.IsNullOrEmpty(this.dtCLZZRQ.Text.Trim()))
            //{
            //    msg += "车辆制造日期/进口日期不能为空\r\n";
            //}
            if (string.IsNullOrEmpty(this.cbeCLZL.Text.Trim()))
            {
                msg += "车辆种类不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbeBSQXS.Text.Trim()))
            {
                msg += "变速器形式不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbeYYC.Text.Trim()))
            {
                msg += "越野车（G类）不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbeQDXS.Text.Trim()))
            {
                msg += "驱动型式不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teZWPS.Text.Trim()))
            {
                msg += "座位排数不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teZGCS.Text.Trim()))
            {
                msg += "最高车速(km/h)不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teEDZK.Text.Trim()))
            {
                msg += "额定载客（人）不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teLTGG.Text.Trim()))
            {
                msg += "轮胎规格不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teLJ.Text.Trim()))
            {
                msg += "轮距（前/后）(mm)不能为空\r\n";
            }
            else if (this.teLJ.Text.Trim().IndexOf('/') < 0 || this.teLJ.Text.Trim().IndexOf('/').Equals(this.teLJ.Text.Trim().Length - 1))
            {
                msg += "轮距（前/后）(mm)，中间用”/”隔开";
            }
            if (string.IsNullOrEmpty(this.tePL.Text.Trim()))
            {
                msg += "排量不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teZJ.Text.Trim()))
            {
                msg += "轴距(mm)不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.cbeRLLX.Text.Trim()))
            {
                msg += "燃料类型不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teYHDYBAH.Text.Trim()))
            {
                msg += "油耗打印备案号不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teZCZBZL.Text.Trim()))
            {
                msg += "整车整备质量(kg)不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teZDSJZZL.Text.Trim()))
            {
                msg += "最大设计总质量(kg)不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teZHGKRLXHL.Text.Trim()))
            {
                msg += "综合工况燃料消耗量不能为空\r\n";
            }
            if (string.IsNullOrEmpty(this.teRLXHLMBZ.Text.Trim()))
            {
                msg += "燃料消耗量目标值不能为空\r\n";
            }
            //if (string.IsNullOrEmpty(this.teCDDQDMSZHGKXHLC.Text.Trim()))
            //{
            //    msg += "纯电动驱动模式综合工况续航里程不能为空\r\n";
            //}
            return msg;
        }
    }
}