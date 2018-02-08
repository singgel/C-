using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using FuelDataSysServer.Tool;
using FuelDataSysServer.SubForm;

namespace FuelDataSysServer
{
    public partial class LocalLoginForm : DevExpress.XtraEditors.XtraForm
    {
        public LocalLoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(FuelDataSysServer.Properties.Settings.Default.UserId) || string.IsNullOrEmpty(FuelDataSysServer.Properties.Settings.Default.UserPWD))
                {
                    MessageBox.Show("请先设置用户名和密码");
                    return;
                }
                if (!checkOracleConn())
                {
                    MessageBox.Show("请先设置正确数据库连接!");
                    return;
                }
                if (CheckLogin(txtUserName.Text.Trim(), txtPassword.Text.Trim()))
                {
                    this.DialogResult = DialogResult.OK;
                    Utils.localUserId = txtUserName.Text.Trim();
                    Utils.GetLoginService();
                }
                else
                {
                    MessageBox.Show("登陆用户名或密码不正确!");
                    return;
                }
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
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
                flg = OracleHelper.Exists(OracleHelper.conn, String.Format(@"SELECT * FROM LOCAL_USER WHERE USER_ID='{0}' AND USER_PASSWORD='{1}'", userId, password));
            }
            catch (Exception)
            {
                flg = false;
            }
            return flg ;
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

        private void labelControl3_Click(object sender, EventArgs e)
        {
            using (SetForm sf = new SetForm())
            {
                sf.ShowDialog();
            }
        }

        private static bool checkOracleConn()
        {
            string connectionString = String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", FuelDataSysServer.Properties.Settings.Default.DataAddr, FuelDataSysServer.Properties.Settings.Default.DataPort, FuelDataSysServer.Properties.Settings.Default.DataBase, FuelDataSysServer.Properties.Settings.Default.DataUserId, FuelDataSysServer.Properties.Settings.Default.DataUserPWD);
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
            }
        }
    }
}