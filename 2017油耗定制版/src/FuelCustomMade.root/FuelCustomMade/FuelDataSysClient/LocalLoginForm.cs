using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataModel;
using System.Threading;
using FuelDataSysClient.Properties;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Form_Configure;

namespace FuelDataSysClient
{
    public partial class LocalLoginForm : DevExpress.XtraEditors.XtraForm
    {
        public LocalLoginForm()
        {
            InitializeComponent();
            //System.Diagnostics.Process.Start(Application.StartupPath + "\\AutoUpdater.exe"); 
            //Thread th = new Thread(() =>
            //{
            //    AutoUpdater.AutoUpdater au = new AutoUpdater.AutoUpdater();
            //    if (AutoUpdater.AutoUpdater.result)
            //        au.ShowDialog();

            //});
            //th.SetApartmentState(ApartmentState.STA);
            //th.Start();
            //comboBoxEdit1.SelectedIndex = 2;
            
           
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
           
            try
            {
                if (string.IsNullOrEmpty(Settings.Default.UserId) || string.IsNullOrEmpty(Settings.Default.UserPWD))
                {
                    MessageBox.Show("请先设置用户名和密码");
                    return;
                }
                if (FuelDataSysClient.Properties.Settings.Default.IsFuelTest == false)  //测试线路
                {
                    txtUserName.Text = "admin";
                    txtPassword.Text = "admin";
                    Utils.IsFuelTest = false;
                }
                if (FuelDataSysClient.Properties.Settings.Default.IsFuelTest == true)  //正式线路
                {
                    Utils.IsFuelTest = true;
                }

                if (CheckLogin(txtUserName.Text.Trim(), txtPassword.Text.Trim()))
                {
                    Utils.localUserId = txtUserName.Text.Trim();
                    FuelDataService.FuelDataSysWebService service = Utils.GetLoginService();
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    MessageBox.Show("用户名或密码不正确!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool CheckLogin(string userId, string password)
        {
            bool flg = false;
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                {
                    return false;
                }
                string sql = String.Format(@"SELECT * FROM LOCAL_USER WHERE USER_ID='{0}' AND USER_PASSWORD='{1}'", userId, password);
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
                int count = ds.Tables[0].Rows.Count;
                if (count > 0)
                {
                    flg = true;
                }
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
            using (var sf = new SetForm())
            {
                sf.ShowDialog(); 
            }
        }

        
    }
}