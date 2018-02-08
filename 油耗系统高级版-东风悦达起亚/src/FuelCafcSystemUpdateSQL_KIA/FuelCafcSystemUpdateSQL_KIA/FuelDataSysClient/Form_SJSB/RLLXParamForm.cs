using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class RLLXParamForm : DevExpress.XtraEditors.XtraForm
    {
        private string SC_OCN = string.Empty;

        public RLLXParamForm()
        {
            InitializeComponent();
        }

        private void RLLXParamForm_Load(object sender, EventArgs e)
        {
            this.SC_OCN = this.teSC_OCN.Text;
            this.cbeRLLX.Properties.Items.AddRange(Utils.GetFuelRLLX(string.Empty).ToArray());
            if (this.Text.Equals("燃料参数新增"))
            {
                // 初始化页面参数
                this.cbeRLLX.Text = "传统能源";
                var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("Select * from RLLX_PARAM WHERE FUEL_TYPE='{0}' And STATUS='1'", this.cbeRLLX.Text), null);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0].Copy();
                    dt.Columns["PARAM_CODE"].ColumnName = "CSBM";
                    dt.Columns["PARAM_NAME"].ColumnName = "CSMC";
                    dt.Columns.Add("CSZ", typeof(System.String));
                    this.gcOCN_RLLXPARAM.DataSource = dt;
                }
            }
            else
            {
                // 初始化页面参数，燃料类型不可更改
                this.cbeRLLX.Enabled = false;
                var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("Select * from OCN_RLLX_PARAM_ENTITY WHERE OPERATION!=4 AND SC_OCN='{0}' and VERSION={1}", this.teSC_OCN.Text, Convert.ToInt32(Regex.Match(this.labVERSION.Text, @"(\d+)").Groups[1].Value)), null);
                if (ds != null && ds.Tables.Count > 0)
                {
                    this.gcOCN_RLLXPARAM.DataSource = ds.Tables[0];
                }
            }
            if (this.Text.Equals("燃料参数复制"))
            {
                // 燃料参数不可更改
                this.gvOCN_RLLXPARAM.Columns["CSZ"].OptionsColumn.ReadOnly = true;
            }
            if (this.Text.Equals("燃料参数修改"))
            {
                // 生产OCN不可更改
                this.teSC_OCN.Enabled = false;
            }
        }

        // 确定按钮
        private void btnYES_Click(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                // 校验页面参数规则
                var resultVerify = this.VerifyRLLXPARAM();
                if (this.dxErrorProvider1.HasErrors || !string.IsNullOrEmpty(resultVerify))
                {
                    MessageBox.Show(String.Format("请核对页面信息是否填写正确！{0}", resultVerify), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (this.Text.Equals("燃料参数新增"))
                {
                    string exist = OracleHelper.ExecuteScalar(OracleHelper.conn, string.Format("SELECT COUNT(*) FROM OCN_RLLX_PARAM_ENTITY WHERE OPERATION!='4' AND SC_OCN='{0}'", this.teSC_OCN.Text)).ToString();
                    int existNum = string.IsNullOrEmpty(exist) ? 0 : Convert.ToInt32(exist);
                    if (existNum > 0)
                    {
                        MessageBox.Show(String.Format("SC_OCN:{0}已经存在！", this.teSC_OCN.Text), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    AddRllxParam();
                }
                if (this.Text.Equals("燃料参数复制"))
                {
                    // 校验生产OCN是否改变
                    if (this.SC_OCN.Equals(this.teSC_OCN.Text.Trim()))
                    {
                        MessageBox.Show(String.Format("OCN编号：{0}复制失败！", this.SC_OCN), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    CopyRllxParam();
                }
                if (this.Text.Equals("燃料参数修改"))
                {
                    // 校验页面参数是否改变
                    if (this.VerifyRepeat())
                    {
                        MessageBox.Show(String.Format("OCN编号：{0}修改失败！", this.SC_OCN), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    UpdateRllxParam();
                }
                MessageBox.Show("燃料参数操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("燃料参数操作异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        // 新增燃料参数
        private void AddRllxParam()
        {
            this.gvOCN_RLLXPARAM.CloseEditor();
            this.gvOCN_RLLXPARAM.UpdateCurrentRow();
            var dt = (DataTable)this.gcOCN_RLLXPARAM.DataSource;
            foreach (DataRow dr in dt.Rows)
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
				                            new OracleParameter("CSBM", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                            new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                            new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("VERSION", OracleDbType.Int32),
                                        };
                            parameters[0].Value = this.teSC_OCN.Text;
                            parameters[1].Value = dr["CSBM"].ToString().Trim();
                            parameters[2].Value = dr["CSMC"].ToString().Trim();
                            parameters[3].Value = this.cbeRLLX.Text;
                            parameters[4].Value = dr["CSZ"].ToString().Trim();
                            parameters[5].Value = "1";
                            parameters[6].Value = System.DateTime.Today;
                            parameters[7].Value = Utils.localUserId;
                            parameters[8].Value = System.DateTime.Today;
                            parameters[9].Value = Utils.localUserId;
                            parameters[10].Value = 0;
                            OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_RLLX_PARAM_ENTITY (SC_OCN,CSBM,CSMC,RLLX,CSZ,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:CSBM,:CSMC,:RLLX,:CSZ,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
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
        }

        // 复制燃料参数
        private void CopyRllxParam()
        {
            var dt = (DataTable)this.gcOCN_RLLXPARAM.DataSource;
            foreach (DataRow dr in dt.Rows)
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
				                            new OracleParameter("CSBM", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                            new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                            new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("VERSION", OracleDbType.Int32),
                                        };
                            parameters[0].Value = this.teSC_OCN.Text;
                            parameters[1].Value = dr["CSBM"].ToString().Trim();
                            parameters[2].Value = dr["CSMC"].ToString().Trim();
                            parameters[3].Value = this.cbeRLLX.Text;
                            parameters[4].Value = dr["CSZ"].ToString().Trim();
                            parameters[5].Value = "2";
                            parameters[6].Value = System.DateTime.Today;
                            parameters[7].Value = Utils.localUserId;
                            parameters[8].Value = System.DateTime.Today;
                            parameters[9].Value = Utils.localUserId;
                            parameters[10].Value = 0;
                            OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_RLLX_PARAM_ENTITY (SC_OCN,CSBM,CSMC,RLLX,CSZ,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:CSBM,:CSMC,:RLLX,:CSZ,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
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
        }

        // 更新燃料参数
        private void UpdateRllxParam()
        {
            this.gvOCN_RLLXPARAM.CloseEditor();
            this.gvOCN_RLLXPARAM.UpdateCurrentRow();
            var dt = (DataTable)this.gcOCN_RLLXPARAM.DataSource;
            foreach (DataRow dr in dt.Rows)
            {
                using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                {
                    conn.Open();
                    using (OracleTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            string version = OracleHelper.ExecuteScalar(trans, string.Format("SELECT MIN(VERSION) FROM OCN_RLLX_PARAM_ENTITY WHERE OPERATION='4' AND SC_OCN='{0}'", this.SC_OCN)).ToString();
                            int versionNew = string.IsNullOrEmpty(version) ? 0 : Convert.ToInt32(version) - 1;
                            OracleHelper.ExecuteNonQuery(trans, string.Format("UPDATE OCN_RLLX_PARAM_ENTITY SET OPERATION = '4',VERSION = '{0}' WHERE SC_OCN='{1}' AND OPERATION = '{2}' AND VERSION={3} ", versionNew, this.SC_OCN, Regex.Match(this.labOPERATION.Text, @"(\d+)").Groups[1].Value, Regex.Match(this.labVERSION.Text, @"(\d+)").Groups[1].Value));
                            OracleParameter[] parameters = 
                                        {
				                            new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSBM", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CSZ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
                                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                            new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                            new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                            new OracleParameter("VERSION", OracleDbType.Int32),
                                        };
                            parameters[0].Value = this.teSC_OCN.Text;
                            parameters[1].Value = dr["CSBM"].ToString().Trim();
                            parameters[2].Value = dr["CSMC"].ToString().Trim();
                            parameters[3].Value = this.cbeRLLX.Text;
                            parameters[4].Value = dr["CSZ"].ToString().Trim();
                            parameters[5].Value = "3";
                            parameters[6].Value = System.DateTime.Today;
                            parameters[7].Value = Utils.localUserId;
                            parameters[8].Value = System.DateTime.Today;
                            parameters[9].Value = Utils.localUserId;
                            parameters[10].Value = Convert.ToInt32(Regex.Match(this.labVERSION.Text, @"(\d+)").Groups[1].Value) + 1;
                            OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_RLLX_PARAM_ENTITY (SC_OCN,CSBM,CSMC,RLLX,CSZ,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:CSBM,:CSMC,:RLLX,:CSZ,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
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

        // 燃料类型更改，页面数据跟随刷新
        private void cbeRLLX_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("Select * from RLLX_PARAM WHERE FUEL_TYPE='{0}' And STATUS='1'", this.cbeRLLX.Text), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                DataTable dt = ds.Tables[0].Copy();
                dt.Columns["PARAM_CODE"].ColumnName = "CSBM";
                dt.Columns["PARAM_NAME"].ColumnName = "CSMC";
                dt.Columns.Add("CSZ", typeof(System.String));
                this.gcOCN_RLLXPARAM.DataSource = dt;
            }
        }

        // 校验数据是否合乎规则
        private string VerifyRLLXPARAM()
        {
            string message = string.Empty;
            this.gvOCN_RLLXPARAM.CloseEditor();
            this.gvOCN_RLLXPARAM.UpdateCurrentRow();
            //OCN验证
            if (teSC_OCN.Text.Equals(""))
            {
                message += "\nOCN不能为空";
            }
            var dt = (DataTable)this.gcOCN_RLLXPARAM.DataSource;
            int paramCountCTNY = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='传统能源'"));
            int paramCountCDD = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='纯电动'"));
            int paramCountCDS = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='插电式混合动力'"));
            int paramCountFCDS = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='非插电式混合动力'"));
            int paramCountRLDC = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='燃料电池'"));
            // 参数个数验证
            if (this.cbeRLLX.Text.Equals("纯电动") && paramCountCDD != dt.Rows.Count)
            {
                message += "\n" + this.cbeRLLX.Text + "的燃料参数个数应为" + paramCountCDD + "个，当前为" + dt.Rows.Count + "个";
            }
            else if (this.cbeRLLX.Text.Equals("非插电式混合动力") && paramCountFCDS != dt.Rows.Count)
            {
                message += "\n" + this.cbeRLLX.Text + "的燃料参数个数应为" + paramCountFCDS + "个，当前为" + dt.Rows.Count + "个";
            }
            else if (this.cbeRLLX.Text.Equals("插电式混合动力") && paramCountCDS != dt.Rows.Count)
            {
                message += "\n" + this.cbeRLLX.Text + "的燃料参数个数应为" + paramCountCDS + "个，当前为" + dt.Rows.Count + "个";
            }
            else if (this.cbeRLLX.Text.Equals("燃料电池") && paramCountRLDC != dt.Rows.Count)
            {
                message += "\n" + this.cbeRLLX.Text + "的燃料参数个数应为" + paramCountRLDC + "个，当前为" + dt.Rows.Count + "个";
            }
            else if (this.cbeRLLX.Text.Equals("传统能源") && paramCountCTNY != dt.Rows.Count)
            {
                message += "\n" + this.cbeRLLX.Text + "的燃料参数个数应为" + paramCountCTNY + "个，当前为" + dt.Rows.Count + "个";
            }
            foreach (DataRow dr in dt.Rows)
            {
                switch (this.cbeRLLX.Text)
                {
                    case "纯电动":
                        message += DataVerifyHelper.VerifyCDD_RLLXPARAM(dr);
                        break;
                    case "非插电式混合动力":
                        message += DataVerifyHelper.VerifyHHDL_RLLXPARAM(dr);
                        break;
                    case "插电式混合动力":
                        message += DataVerifyHelper.VerifyHHDL_RLLXPARAM(dr);
                        break;
                    case "燃料电池":
                        message += DataVerifyHelper.VerifyRLDC_RLLXPARAM(dr);
                        break;
                    default:
                        message += DataVerifyHelper.VerifyCTNY_RLLXPARAM(dr);
                        break;
                }
            }
            return message;
        }

        // 校验数据是否更改
        private bool VerifyRepeat()
        {
            this.gvOCN_RLLXPARAM.CloseEditor();
            this.gvOCN_RLLXPARAM.UpdateCurrentRow();
            var dt = (DataTable)this.gcOCN_RLLXPARAM.DataSource;
            var dtOriginal = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("Select * From OCN_RLLX_PARAM_ENTITY Where SC_OCN='{0}'", this.SC_OCN), null).Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                var rowsFind = dtOriginal.Select(String.Format("CSBM='{0}' and CSMC='{1}' and CSZ='{2}'", dr["CSBM"].ToString().Trim(), dr["CSMC"].ToString().Trim(), dr["CSZ"].ToString().Trim()));
                if (rowsFind.Length == 1)
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}