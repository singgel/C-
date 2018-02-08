using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;


namespace FuelDataSysClient.SubForm
{
    public partial class SetForm : DevExpress.XtraEditors.XtraForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        public SetForm()
        {
            InitializeComponent();
            bool IsFuelTest = FuelDataSysClient.Properties.Settings.Default.IsFuelTest;
            string strDx = FuelDataModel.Utils.strDx;
            string strWt = FuelDataModel.Utils.strWt;
            string strTest = FuelDataModel.Utils.strTest;

            if (IsFuelTest == true)
            {
                this.comboBox1.SelectedIndex = 1;
            }
            else
            {
                this.comboBox1.SelectedIndex = 0;
            }

            // 初始化网络代理信息
            bool IsProxy = FuelDataSysClient.Properties.Settings.Default.IsProxy;
            if (IsProxy)
            {
                this.ceProxy.Checked = IsProxy;
                this.pcProxy.Enabled = IsProxy;
                this.teProxyAddr.Text = FuelDataSysClient.Properties.Settings.Default.ProxyAddr;
                this.teProxyPort.Text = FuelDataSysClient.Properties.Settings.Default.ProxyPort;
                this.teProxyUserId.Text = FuelDataSysClient.Properties.Settings.Default.ProxyUserId;
                this.teProxyPwd.Text = FuelDataSysClient.Properties.Settings.Default.ProxyPwd;
            }
            else
            {
                this.ceProxy.Checked = false;
                this.pcProxy.Enabled = false;
            }

            // 初始化用户名密码信息
            this.teUserName.Text = FuelDataSysClient.Properties.Settings.Default.UserId;
            this.tePassword.Text = FuelDataSysClient.Properties.Settings.Default.UserPWD;
            //textEdit1.Text = FuelDataSysClient.Properties.Settings.Default.DbPath;
        }

        // 保存所有设置
        private void btnSave_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            try
            {
                message = this.SaveNetLine();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                message = this.SaveProxyInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                message = this.SaveUserInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                //message = this.SaveDataPath();
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("保存成功！");
                }
                else
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            this.comboBox1.SelectedIndex = 0;
        }

        // 设置代理
        private void ceProxy_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.ceProxy.Checked)
            {
                this.pcProxy.Enabled = true;
            }
            else
            {
                this.pcProxy.Enabled = false;
            }
        }

        // 保存上传线路设置
        protected string SaveNetLine()
        {
            string msg = string.Empty;
            try
            {
                string strVal = this.comboBox1.Text;
                if (string.IsNullOrEmpty(strVal))
                {
                    return "请选择上传线路";
                }
                FuelDataModel.Utils u = new FuelDataModel.Utils();
                if (strVal == "正式线路")
                {
                    FuelDataSysClient.Properties.Settings.Default.IsFuelTest = true;
                    Utils.IsFuelTest = true;
                }
                //else if (strVal == "联通线路")
                //{
                //    FuelDataSysClient.Properties.Settings.Default.WsDXIP = u.GetLine("联通线路");
                //}
                else
                {
                    FuelDataSysClient.Properties.Settings.Default.IsFuelTest = false;
                    Utils.IsFuelTest = false;
                }

                FuelDataSysClient.Properties.Settings.Default.Save();
                //Utils.service.Url = FuelDataSysClient.Properties.Settings.Default.WsDXIP;
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }
            return msg;
        }

        // 保存代理设置
        protected string SaveProxyInfo()
        {
            string msg = string.Empty;
            try
            {
                bool IsProxy = this.ceProxy.Checked;
                FuelDataSysClient.Properties.Settings.Default.IsProxy = IsProxy;

                if (IsProxy)
                {
                    FuelDataSysClient.Properties.Settings.Default.ProxyAddr = this.teProxyAddr.Text.Trim();
                    FuelDataSysClient.Properties.Settings.Default.ProxyPort = this.teProxyPort.Text.Trim();
                    FuelDataSysClient.Properties.Settings.Default.ProxyUserId = this.teProxyUserId.Text.Trim();
                    FuelDataSysClient.Properties.Settings.Default.ProxyPwd = this.teProxyPwd.Text.Trim();
                }
                else
                {
                    FuelDataSysClient.Properties.Settings.Default.ProxyAddr = string.Empty;
                    FuelDataSysClient.Properties.Settings.Default.ProxyPort = string.Empty;
                    FuelDataSysClient.Properties.Settings.Default.ProxyUserId = string.Empty;
                    FuelDataSysClient.Properties.Settings.Default.ProxyPwd = string.Empty;
                }
                FuelDataSysClient.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        // 保存用户名密码
        protected string SaveUserInfo()
        {
            string msg = string.Empty;
            string userName = this.teUserName.Text.Trim();
            string userPassowrd = this.tePassword.Text.Trim();
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassowrd))
            {
                msg = "用户名或密码错误";
            }
            try
            {
                if (service != null && !string.IsNullOrEmpty(userName))
                {
                    if (Utils.service.CheckUser(userName, userPassowrd))
                    {
                        FuelDataSysClient.Properties.Settings.Default.UserId = userName;
                        FuelDataSysClient.Properties.Settings.Default.UserPWD = userPassowrd;
                        FuelDataSysClient.Properties.Settings.Default.Qymc = service.QueryQymc(userName, userPassowrd);
                        FuelDataSysClient.Properties.Settings.Default.TimeConstrain = service.QueryUploadTimeConstrain(userName, userPassowrd);

                        FuelDataSysClient.Properties.Settings.Default.Save();
                    }
                    else
                    {
                        msg = "用户名或密码错误";
                    }
                }
                else
                {
                    FuelDataSysClient.Properties.Settings.Default.UserId = userName;
                    FuelDataSysClient.Properties.Settings.Default.UserPWD = userPassowrd;
                    FuelDataSysClient.Properties.Settings.Default.Qymc = string.Empty;
                    FuelDataSysClient.Properties.Settings.Default.TimeConstrain = 48;
                    FuelDataSysClient.Properties.Settings.Default.Save();
                }
                Utils.userId = FuelDataSysClient.Properties.Settings.Default.UserId;
                Utils.password = FuelDataSysClient.Properties.Settings.Default.UserPWD;
                Utils.qymc = FuelDataSysClient.Properties.Settings.Default.Qymc;
                Utils.timeCons = FuelDataSysClient.Properties.Settings.Default.TimeConstrain;
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }


       
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //textEdit1.Text = of.FileName;
            }
        }

        protected string SaveDataPath()
        {
            string msg = string.Empty;
            try
            {
                //FuelDataSysClient.Properties.Settings.Default.DbPath = textEdit1.Text.Trim();
                FuelDataSysClient.Properties.Settings.Default.Save();
            }
            catch (Exception ex) { msg = ex.Message + "\n"; }
            return msg;
        }
    }
}