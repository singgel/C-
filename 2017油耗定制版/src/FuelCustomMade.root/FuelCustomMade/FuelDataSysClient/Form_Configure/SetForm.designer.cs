namespace FuelDataSysClient.Form_Configure
{
    partial class SetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetForm));
            this.xtc = new DevExpress.XtraTab.XtraTabControl();
            this.xtpSccssz = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.comboBox1 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.xtpProxy = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.pcProxy = new DevExpress.XtraEditors.PanelControl();
            this.lcPort = new DevExpress.XtraEditors.LabelControl();
            this.lcProxyPwd = new DevExpress.XtraEditors.LabelControl();
            this.lcProxyUserId = new DevExpress.XtraEditors.LabelControl();
            this.lcProxyAddr = new DevExpress.XtraEditors.LabelControl();
            this.teProxyPort = new DevExpress.XtraEditors.TextEdit();
            this.teProxyPwd = new DevExpress.XtraEditors.TextEdit();
            this.teProxyUserId = new DevExpress.XtraEditors.TextEdit();
            this.teProxyAddr = new DevExpress.XtraEditors.TextEdit();
            this.ceProxy = new DevExpress.XtraEditors.CheckEdit();
            this.xtpUserInfo = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.tePassword = new DevExpress.XtraEditors.TextEdit();
            this.teUserName = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.xtc)).BeginInit();
            this.xtc.SuspendLayout();
            this.xtpSccssz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox1.Properties)).BeginInit();
            this.xtpProxy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcProxy)).BeginInit();
            this.pcProxy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyPwd.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyUserId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyAddr.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceProxy.Properties)).BeginInit();
            this.xtpUserInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teUserName.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtc
            // 
            this.xtc.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(237)))), ((int)(((byte)(241)))));
            this.xtc.Appearance.Options.UseBackColor = true;
            this.xtc.Location = new System.Drawing.Point(0, 0);
            this.xtc.Name = "xtc";
            this.xtc.SelectedTabPage = this.xtpSccssz;
            this.xtc.Size = new System.Drawing.Size(375, 181);
            this.xtc.TabIndex = 11;
            this.xtc.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtpSccssz,
            this.xtpProxy,
            this.xtpUserInfo});
            // 
            // xtpSccssz
            // 
            this.xtpSccssz.Controls.Add(this.panelControl1);
            this.xtpSccssz.Name = "xtpSccssz";
            this.xtpSccssz.Size = new System.Drawing.Size(369, 152);
            this.xtpSccssz.Text = "上传线路设置";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.comboBox1);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(374, 152);
            this.panelControl1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.Location = new System.Drawing.Point(106, 52);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBox1.Properties.Items.AddRange(new object[] {
            "测试线路",
            "正式线路"});
            this.comboBox1.Size = new System.Drawing.Size(163, 20);
            this.comboBox1.TabIndex = 16;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(42, 54);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 13;
            this.labelControl1.Text = "上传线路";
            // 
            // xtpProxy
            // 
            this.xtpProxy.Controls.Add(this.panelControl2);
            this.xtpProxy.Name = "xtpProxy";
            this.xtpProxy.Size = new System.Drawing.Size(369, 152);
            this.xtpProxy.Text = "代理设置";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.pcProxy);
            this.panelControl2.Controls.Add(this.ceProxy);
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(372, 157);
            this.panelControl2.TabIndex = 0;
            // 
            // pcProxy
            // 
            this.pcProxy.Controls.Add(this.lcPort);
            this.pcProxy.Controls.Add(this.lcProxyPwd);
            this.pcProxy.Controls.Add(this.lcProxyUserId);
            this.pcProxy.Controls.Add(this.lcProxyAddr);
            this.pcProxy.Controls.Add(this.teProxyPort);
            this.pcProxy.Controls.Add(this.teProxyPwd);
            this.pcProxy.Controls.Add(this.teProxyUserId);
            this.pcProxy.Controls.Add(this.teProxyAddr);
            this.pcProxy.Location = new System.Drawing.Point(-1, 42);
            this.pcProxy.Name = "pcProxy";
            this.pcProxy.Size = new System.Drawing.Size(375, 115);
            this.pcProxy.TabIndex = 1;
            // 
            // lcPort
            // 
            this.lcPort.Location = new System.Drawing.Point(249, 18);
            this.lcPort.Name = "lcPort";
            this.lcPort.Size = new System.Drawing.Size(24, 14);
            this.lcPort.TabIndex = 14;
            this.lcPort.Text = "端口";
            // 
            // lcProxyPwd
            // 
            this.lcProxyPwd.Location = new System.Drawing.Point(10, 70);
            this.lcProxyPwd.Name = "lcProxyPwd";
            this.lcProxyPwd.Size = new System.Drawing.Size(24, 14);
            this.lcProxyPwd.TabIndex = 16;
            this.lcProxyPwd.Text = "密码";
            // 
            // lcProxyUserId
            // 
            this.lcProxyUserId.Location = new System.Drawing.Point(10, 44);
            this.lcProxyUserId.Name = "lcProxyUserId";
            this.lcProxyUserId.Size = new System.Drawing.Size(36, 14);
            this.lcProxyUserId.TabIndex = 15;
            this.lcProxyUserId.Text = "用户名";
            // 
            // lcProxyAddr
            // 
            this.lcProxyAddr.Location = new System.Drawing.Point(10, 18);
            this.lcProxyAddr.Name = "lcProxyAddr";
            this.lcProxyAddr.Size = new System.Drawing.Size(24, 14);
            this.lcProxyAddr.TabIndex = 13;
            this.lcProxyAddr.Text = "地址";
            // 
            // teProxyPort
            // 
            this.teProxyPort.Location = new System.Drawing.Point(289, 15);
            this.teProxyPort.Name = "teProxyPort";
            this.teProxyPort.Size = new System.Drawing.Size(60, 20);
            this.teProxyPort.TabIndex = 7;
            // 
            // teProxyPwd
            // 
            this.teProxyPwd.Location = new System.Drawing.Point(55, 67);
            this.teProxyPwd.Name = "teProxyPwd";
            this.teProxyPwd.Properties.PasswordChar = '*';
            this.teProxyPwd.Size = new System.Drawing.Size(176, 20);
            this.teProxyPwd.TabIndex = 9;
            // 
            // teProxyUserId
            // 
            this.teProxyUserId.Location = new System.Drawing.Point(55, 41);
            this.teProxyUserId.Name = "teProxyUserId";
            this.teProxyUserId.Size = new System.Drawing.Size(176, 20);
            this.teProxyUserId.TabIndex = 8;
            // 
            // teProxyAddr
            // 
            this.teProxyAddr.Location = new System.Drawing.Point(55, 15);
            this.teProxyAddr.Name = "teProxyAddr";
            this.teProxyAddr.Size = new System.Drawing.Size(176, 20);
            this.teProxyAddr.TabIndex = 6;
            // 
            // ceProxy
            // 
            this.ceProxy.Location = new System.Drawing.Point(5, 17);
            this.ceProxy.Name = "ceProxy";
            this.ceProxy.Properties.Caption = "使用代理服务器";
            this.ceProxy.Size = new System.Drawing.Size(115, 19);
            this.ceProxy.TabIndex = 5;
            this.ceProxy.CheckedChanged += new System.EventHandler(this.ceProxy_CheckStateChanged);
            // 
            // xtpUserInfo
            // 
            this.xtpUserInfo.Controls.Add(this.panelControl3);
            this.xtpUserInfo.Name = "xtpUserInfo";
            this.xtpUserInfo.Size = new System.Drawing.Size(369, 152);
            this.xtpUserInfo.Text = "用户名密码设置";
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.tePassword);
            this.panelControl3.Controls.Add(this.teUserName);
            this.panelControl3.Controls.Add(this.labelControl3);
            this.panelControl3.Controls.Add(this.labelControl2);
            this.panelControl3.Location = new System.Drawing.Point(-2, -2);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(372, 157);
            this.panelControl3.TabIndex = 1;
            // 
            // tePassword
            // 
            this.tePassword.Location = new System.Drawing.Point(103, 76);
            this.tePassword.Name = "tePassword";
            this.tePassword.Properties.PasswordChar = '*';
            this.tePassword.Size = new System.Drawing.Size(191, 20);
            this.tePassword.TabIndex = 12;
            // 
            // teUserName
            // 
            this.teUserName.Location = new System.Drawing.Point(103, 38);
            this.teUserName.Name = "teUserName";
            this.teUserName.Size = new System.Drawing.Size(191, 20);
            this.teUserName.TabIndex = 11;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(44, 80);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(24, 14);
            this.labelControl3.TabIndex = 18;
            this.labelControl3.Text = "密码";
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(44, 41);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(36, 14);
            this.labelControl2.TabIndex = 17;
            this.labelControl2.Text = "用户名";
            // 
            // simpleButton2
            // 
            this.simpleButton2.Location = new System.Drawing.Point(185, 189);
            this.simpleButton2.Name = "simpleButton2";
            this.simpleButton2.Size = new System.Drawing.Size(75, 24);
            this.simpleButton2.TabIndex = 13;
            this.simpleButton2.Text = "关闭";
            this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(83, 189);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 24);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "保存设置";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(370, 225);
            this.Controls.Add(this.simpleButton2);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.xtc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置";
            ((System.ComponentModel.ISupportInitialize)(this.xtc)).EndInit();
            this.xtc.ResumeLayout(false);
            this.xtpSccssz.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBox1.Properties)).EndInit();
            this.xtpProxy.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcProxy)).EndInit();
            this.pcProxy.ResumeLayout(false);
            this.pcProxy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyPwd.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyUserId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teProxyAddr.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceProxy.Properties)).EndInit();
            this.xtpUserInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelControl3.ResumeLayout(false);
            this.panelControl3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tePassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teUserName.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtc;
        private DevExpress.XtraTab.XtraTabPage xtpSccssz;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraTab.XtraTabPage xtpProxy;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.PanelControl pcProxy;
        private DevExpress.XtraEditors.LabelControl lcPort;
        private DevExpress.XtraEditors.LabelControl lcProxyPwd;
        private DevExpress.XtraEditors.LabelControl lcProxyUserId;
        private DevExpress.XtraEditors.LabelControl lcProxyAddr;
        private DevExpress.XtraEditors.TextEdit teProxyPort;
        private DevExpress.XtraEditors.TextEdit teProxyPwd;
        private DevExpress.XtraEditors.TextEdit teProxyUserId;
        private DevExpress.XtraEditors.TextEdit teProxyAddr;
        private DevExpress.XtraEditors.CheckEdit ceProxy;
        private DevExpress.XtraTab.XtraTabPage xtpUserInfo;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraEditors.TextEdit tePassword;
        private DevExpress.XtraEditors.TextEdit teUserName;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.SimpleButton simpleButton2;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.ComboBoxEdit comboBox1;

    }
}