using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using Catarc.Adc.NewEnergyApproveSys.Form_Config;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using System.Collections;
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyApproveSys.Common;
using System.Net;

namespace Catarc.Adc.NewEnergyApproveSys
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        public static Hashtable htRole = GridControlHelper.GetRoleName("");

        public LoginForm()
        {
            InitializeComponent();
        }

        //登录
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (!checkOracleConn() || !checkFtpConn())
                {
                    XtraMessageBox.Show("请先进行正确的设置!");
                    return;
                }
                if (CheckLogin(txtUserName.Text.Trim(), txtPassword.Text.Trim()))
                {
                    this.DialogResult = DialogResult.OK;
                    //记录操作日志
                    LogUtils.ReviewLogManager.ReviewLog(Settings.Default.LocalUserName, this.btnLogin.Text.Trim());
                }
                else
                {
                    XtraMessageBox.Show("登陆用户名或密码不正确！或没有权限登录！");
                    return;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void LoginForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (var setform = new SetForm())
            {
                setform.ShowDialog();
            }
        }

        private void labelControl4_Click(object sender, EventArgs e)
        {
            using (var setform = new SetForm())
            {
                setform.ShowDialog();
            }
        }

        private static bool CheckLogin(string userId, string password)
        {
            bool flg = false;
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                {
                    return false;
                }
                DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, String.Format("SELECT U.*,R.AUTHORITY FROM SYS_USERINFO U LEFT JOIN SYS_ROLE R ON U.USERROLEID=R.ID WHERE U.STATUS=0 AND U.USERLOGINNAME='{0}' AND U.PWD='{1}'", userId, EncryptUtil.Md532(password)), null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    flg = true;
                    Settings.Default.LocalUserName = userId;
                    Settings.Default.LocalUserPassword = password;
                    Settings.Default.LocalUserID = ds.Tables[0].Rows[0]["ID"].ToString();
                    Settings.Default.Authority = ds.Tables[0].Rows[0]["AUTHORITY"].ToString();
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "异常：", MessageBoxButtons.OK, MessageBoxIcon.Error);
                flg = false;
            }
            return flg;
        }

        private static bool checkOracleConn()
        {
            string connectionString = String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", Settings.Default.DataAddr, Settings.Default.DataPort, Settings.Default.DataBase, Settings.Default.DataUserId, Settings.Default.DataUserPWD);
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private static bool checkFtpConn()
        {
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(String.Format("ftp://{0}/", Settings.Default.FtpIPAddr)));
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Settings.Default.FtpUserID, Settings.Default.FtpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}