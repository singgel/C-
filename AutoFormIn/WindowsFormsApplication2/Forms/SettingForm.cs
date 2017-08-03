using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Assistant.Forms
{
    public partial class SettingForm : DevExpress.XtraEditors.XtraForm
    {
        public SettingForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            initProxyTab();
            initWSAddTab();

        }

        private void initProxyTab() {
            bool isProxy = Properties.Settings.Default.IsUsingProxy;
            this.isProxyCheck.Checked = isProxy;
            if (isProxy)
            {
                String proxyAddress = Properties.Settings.Default.ProxyAddress;
                String proxyPort = Properties.Settings.Default.ProxyPort;
                String proxyUsername = Properties.Settings.Default.ProxyUsername;
                String proxyPassword = Properties.Settings.Default.ProxyPassword;
                String domainName = Properties.Settings.Default.DomainName;
                this.proxyAddress.Text = proxyAddress;
                this.proxyPassword.Text = proxyPassword;
                this.proxyPort.Text = proxyPort;
                this.proxyUsername.Text = proxyUsername;
                this.domainName.Text = domainName;
            }
            else {
                this.proxyInfoPanel.Enabled = false;
            }
        }

        private void initWSAddTab() {
            this.outerWebserviceText.Text = Properties.Settings.Default.Assistant_FlexService_IFlexUserServiceService;
            this.innerWebServiceText.Text = Properties.Settings.Default.Assistant_ConfigCodeService_IPackageInfoServiceService;
        }

        /// <summary>
        /// 选中使用代理下面的面板才能够控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void isProxyCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isProxyCheck.Checked)
            {
                this.proxyInfoPanel.Enabled = true;
            }
            else
            {
                this.proxyInfoPanel.Enabled = false;
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {

            try
            {
                SaveProxyInfo();
                SaveWSInfo();
                ClassFactory.fuss = ClassFactory.GetFlexService();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected void SaveWSInfo() {
            Properties.Settings.Default.Assistant_ConfigCodeService_IPackageInfoServiceService = this.innerWebServiceText.Text;
            Properties.Settings.Default.Assistant_FlexService_IFlexUserServiceService = this.outerWebserviceText.Text;
        }


        // 保存代理设置
        protected void SaveProxyInfo()
        {
            try
            {
                bool IsProxy = this.isProxyCheck.Checked;
                Properties.Settings.Default.IsUsingProxy = IsProxy;

                if (IsProxy)
                {
                    Properties.Settings.Default.ProxyAddress = this.proxyAddress.Text.Trim();
                    Properties.Settings.Default.ProxyPort = this.proxyPort.Text.Trim();
                    Properties.Settings.Default.ProxyUsername = this.proxyUsername.Text.Trim();
                    Properties.Settings.Default.ProxyPassword = this.proxyPassword.Text.Trim();
                    Properties.Settings.Default.DomainName = this.domainName.Text.Trim();
                }
                else
                {
                    Properties.Settings.Default.ProxyAddress = string.Empty;
                    Properties.Settings.Default.ProxyPort = string.Empty;
                    Properties.Settings.Default.ProxyUsername = string.Empty;
                    Properties.Settings.Default.ProxyPassword = string.Empty;
                    Properties.Settings.Default.DomainName = string.Empty;
                }
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void proxyPort_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}