namespace Assistant.Forms
{
    partial class SettingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.代理设置 = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.proxyInfoPanel = new DevExpress.XtraEditors.PanelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.proxyPort = new DevExpress.XtraEditors.TextEdit();
            this.domainName = new DevExpress.XtraEditors.TextEdit();
            this.proxyAddress = new DevExpress.XtraEditors.TextEdit();
            this.proxyPassword = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.proxyUsername = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.isProxyCheck = new DevExpress.XtraEditors.CheckEdit();
            this.webServiceTab = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.innerWebServiceText = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.outerWebserviceText = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.saveButton = new DevExpress.XtraEditors.SimpleButton();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.代理设置.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proxyInfoPanel)).BeginInit();
            this.proxyInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proxyPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.domainName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.proxyAddress.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.proxyPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.proxyUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.isProxyCheck.Properties)).BeginInit();
            this.webServiceTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.innerWebServiceText.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.outerWebserviceText.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.代理设置;
            this.xtraTabControl1.Size = new System.Drawing.Size(422, 240);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.代理设置,
            this.webServiceTab});
            // 
            // 代理设置
            // 
            this.代理设置.Controls.Add(this.panelControl1);
            this.代理设置.Name = "代理设置";
            this.代理设置.Size = new System.Drawing.Size(416, 207);
            this.代理设置.Text = "代理设置";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.proxyInfoPanel);
            this.panelControl1.Controls.Add(this.isProxyCheck);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(416, 207);
            this.panelControl1.TabIndex = 0;
            // 
            // proxyInfoPanel
            // 
            this.proxyInfoPanel.Controls.Add(this.labelControl5);
            this.proxyInfoPanel.Controls.Add(this.proxyPort);
            this.proxyInfoPanel.Controls.Add(this.domainName);
            this.proxyInfoPanel.Controls.Add(this.proxyAddress);
            this.proxyInfoPanel.Controls.Add(this.proxyPassword);
            this.proxyInfoPanel.Controls.Add(this.labelControl3);
            this.proxyInfoPanel.Controls.Add(this.labelControl2);
            this.proxyInfoPanel.Controls.Add(this.labelControl1);
            this.proxyInfoPanel.Controls.Add(this.proxyUsername);
            this.proxyInfoPanel.Controls.Add(this.labelControl4);
            this.proxyInfoPanel.Location = new System.Drawing.Point(0, 42);
            this.proxyInfoPanel.Name = "proxyInfoPanel";
            this.proxyInfoPanel.Size = new System.Drawing.Size(416, 165);
            this.proxyInfoPanel.TabIndex = 11;
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(15, 104);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(30, 18);
            this.labelControl5.TabIndex = 5;
            this.labelControl5.Text = "密码";
            // 
            // proxyPort
            // 
            this.proxyPort.Location = new System.Drawing.Point(275, 26);
            this.proxyPort.Name = "proxyPort";
            this.proxyPort.Size = new System.Drawing.Size(101, 24);
            this.proxyPort.TabIndex = 8;
            this.proxyPort.EditValueChanged += new System.EventHandler(this.proxyPort_EditValueChanged);
            // 
            // domainName
            // 
            this.domainName.Location = new System.Drawing.Point(275, 66);
            this.domainName.Name = "domainName";
            this.domainName.Size = new System.Drawing.Size(101, 24);
            this.domainName.TabIndex = 10;
            // 
            // proxyAddress
            // 
            this.proxyAddress.Location = new System.Drawing.Point(73, 26);
            this.proxyAddress.Name = "proxyAddress";
            this.proxyAddress.Size = new System.Drawing.Size(139, 24);
            this.proxyAddress.TabIndex = 6;
            // 
            // proxyPassword
            // 
            this.proxyPassword.Location = new System.Drawing.Point(73, 102);
            this.proxyPassword.Name = "proxyPassword";
            this.proxyPassword.Properties.PasswordChar = '*';
            this.proxyPassword.Size = new System.Drawing.Size(139, 24);
            this.proxyPassword.TabIndex = 9;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(227, 30);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(30, 18);
            this.labelControl3.TabIndex = 3;
            this.labelControl3.Text = "端口";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(15, 68);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(45, 18);
            this.labelControl2.TabIndex = 2;
            this.labelControl2.Text = "用户名";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(15, 30);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(30, 18);
            this.labelControl1.TabIndex = 1;
            this.labelControl1.Text = "地址";
            // 
            // proxyUsername
            // 
            this.proxyUsername.Location = new System.Drawing.Point(73, 66);
            this.proxyUsername.Name = "proxyUsername";
            this.proxyUsername.Size = new System.Drawing.Size(139, 24);
            this.proxyUsername.TabIndex = 7;
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(227, 68);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(30, 18);
            this.labelControl4.TabIndex = 4;
            this.labelControl4.Text = "域名";
            // 
            // isProxyCheck
            // 
            this.isProxyCheck.Location = new System.Drawing.Point(37, 14);
            this.isProxyCheck.Name = "isProxyCheck";
            this.isProxyCheck.Properties.Caption = "使用代理服务器";
            this.isProxyCheck.Size = new System.Drawing.Size(142, 23);
            this.isProxyCheck.TabIndex = 0;
            this.isProxyCheck.CheckedChanged += new System.EventHandler(this.isProxyCheck_CheckedChanged);
            // 
            // webServiceTab
            // 
            this.webServiceTab.Controls.Add(this.panelControl2);
            this.webServiceTab.Name = "webServiceTab";
            this.webServiceTab.Size = new System.Drawing.Size(416, 207);
            this.webServiceTab.Text = "服务端设置";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.innerWebServiceText);
            this.panelControl2.Controls.Add(this.labelControl7);
            this.panelControl2.Controls.Add(this.outerWebserviceText);
            this.panelControl2.Controls.Add(this.labelControl6);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(416, 207);
            this.panelControl2.TabIndex = 0;
            // 
            // innerWebServiceText
            // 
            this.innerWebServiceText.Location = new System.Drawing.Point(99, 68);
            this.innerWebServiceText.Name = "innerWebServiceText";
            this.innerWebServiceText.Size = new System.Drawing.Size(307, 24);
            this.innerWebServiceText.TabIndex = 10;
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(10, 71);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(80, 18);
            this.labelControl7.TabIndex = 9;
            this.labelControl7.Text = "内部服务器 ";
            // 
            // outerWebserviceText
            // 
            this.outerWebserviceText.Location = new System.Drawing.Point(98, 28);
            this.outerWebserviceText.Name = "outerWebserviceText";
            this.outerWebserviceText.Size = new System.Drawing.Size(307, 24);
            this.outerWebserviceText.TabIndex = 8;
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(9, 31);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(75, 18);
            this.labelControl6.TabIndex = 7;
            this.labelControl6.Text = "外部服务器";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(93, 255);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(96, 30);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "保存设置";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(225, 255);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(96, 30);
            this.simpleButton2.TabIndex = 2;
            this.simpleButton2.Text = "取消";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 298);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.xtraTabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingForm";
            this.Text = "软件设置";
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.代理设置.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.proxyInfoPanel)).EndInit();
            this.proxyInfoPanel.ResumeLayout(false);
            this.proxyInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proxyPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.domainName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.proxyAddress.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.proxyPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.proxyUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.isProxyCheck.Properties)).EndInit();
            this.webServiceTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.innerWebServiceText.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.outerWebserviceText.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage 代理设置;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.CheckEdit isProxyCheck;
        private DevExpress.XtraEditors.SimpleButton saveButton;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.TextEdit domainName;
        private DevExpress.XtraEditors.TextEdit proxyPort;
        private DevExpress.XtraEditors.TextEdit proxyUsername;
        private DevExpress.XtraEditors.TextEdit proxyAddress;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.PanelControl proxyInfoPanel;
        private DevExpress.XtraTab.XtraTabPage webServiceTab;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.TextEdit innerWebServiceText;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.TextEdit outerWebserviceText;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit proxyPassword;

    }
}