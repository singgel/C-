namespace Catarc.Adc.NewEnergyApproveSys.Form_Config
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
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.xtpUserInfo = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.teDataBase = new DevExpress.XtraEditors.TextEdit();
            this.teDataPort = new DevExpress.XtraEditors.TextEdit();
            this.teDataPassword = new DevExpress.XtraEditors.TextEdit();
            this.teDataUserName = new DevExpress.XtraEditors.TextEdit();
            this.teDataAddr = new DevExpress.XtraEditors.TextEdit();
            this.xtc = new DevExpress.XtraTab.XtraTabControl();
            this.xtpFtpInfo = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.teFtpPassword = new DevExpress.XtraEditors.TextEdit();
            this.teFtpUserName = new DevExpress.XtraEditors.TextEdit();
            this.teFtpAddr = new DevExpress.XtraEditors.TextEdit();
            this.xtpUserInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).BeginInit();
            this.panelControl5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teDataBase.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataPort.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataAddr.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtc)).BeginInit();
            this.xtc.SuspendLayout();
            this.xtpFtpInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teFtpPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teFtpUserName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teFtpAddr.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(195, 201);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 24);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "关闭";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(93, 201);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 24);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "保存设置";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // xtpUserInfo
            // 
            this.xtpUserInfo.Controls.Add(this.panelControl5);
            this.xtpUserInfo.Name = "xtpUserInfo";
            this.xtpUserInfo.Size = new System.Drawing.Size(369, 152);
            this.xtpUserInfo.Text = "数据库设置";
            // 
            // panelControl5
            // 
            this.panelControl5.Controls.Add(this.labelControl11);
            this.panelControl5.Controls.Add(this.labelControl6);
            this.panelControl5.Controls.Add(this.labelControl7);
            this.panelControl5.Controls.Add(this.labelControl8);
            this.panelControl5.Controls.Add(this.labelControl9);
            this.panelControl5.Controls.Add(this.teDataBase);
            this.panelControl5.Controls.Add(this.teDataPort);
            this.panelControl5.Controls.Add(this.teDataPassword);
            this.panelControl5.Controls.Add(this.teDataUserName);
            this.panelControl5.Controls.Add(this.teDataAddr);
            this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl5.Location = new System.Drawing.Point(0, 0);
            this.panelControl5.Name = "panelControl5";
            this.panelControl5.Size = new System.Drawing.Size(369, 152);
            this.panelControl5.TabIndex = 3;
            // 
            // labelControl11
            // 
            this.labelControl11.Location = new System.Drawing.Point(21, 106);
            this.labelControl11.Name = "labelControl11";
            this.labelControl11.Size = new System.Drawing.Size(60, 14);
            this.labelControl11.TabIndex = 22;
            this.labelControl11.Text = "数据库实例";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(192, 28);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(24, 14);
            this.labelControl6.TabIndex = 22;
            this.labelControl6.Text = "端口";
            // 
            // labelControl7
            // 
            this.labelControl7.Location = new System.Drawing.Point(192, 63);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(24, 14);
            this.labelControl7.TabIndex = 24;
            this.labelControl7.Text = "密码";
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(21, 63);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(36, 14);
            this.labelControl8.TabIndex = 23;
            this.labelControl8.Text = "用户名";
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(21, 28);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(35, 14);
            this.labelControl9.TabIndex = 21;
            this.labelControl9.Text = "IP地址";
            // 
            // teDataBase
            // 
            this.teDataBase.Location = new System.Drawing.Point(96, 103);
            this.teDataBase.Name = "teDataBase";
            this.teDataBase.Size = new System.Drawing.Size(173, 20);
            this.teDataBase.TabIndex = 21;
            // 
            // teDataPort
            // 
            this.teDataPort.Location = new System.Drawing.Point(267, 25);
            this.teDataPort.Name = "teDataPort";
            this.teDataPort.Size = new System.Drawing.Size(90, 20);
            this.teDataPort.TabIndex = 18;
            // 
            // teDataPassword
            // 
            this.teDataPassword.Location = new System.Drawing.Point(267, 60);
            this.teDataPassword.Name = "teDataPassword";
            this.teDataPassword.Properties.PasswordChar = '*';
            this.teDataPassword.Size = new System.Drawing.Size(90, 20);
            this.teDataPassword.TabIndex = 20;
            // 
            // teDataUserName
            // 
            this.teDataUserName.Location = new System.Drawing.Point(66, 60);
            this.teDataUserName.Name = "teDataUserName";
            this.teDataUserName.Size = new System.Drawing.Size(100, 20);
            this.teDataUserName.TabIndex = 19;
            // 
            // teDataAddr
            // 
            this.teDataAddr.Location = new System.Drawing.Point(66, 25);
            this.teDataAddr.Name = "teDataAddr";
            this.teDataAddr.Size = new System.Drawing.Size(100, 20);
            this.teDataAddr.TabIndex = 17;
            // 
            // xtc
            // 
            this.xtc.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(237)))), ((int)(((byte)(241)))));
            this.xtc.Appearance.Options.UseBackColor = true;
            this.xtc.Location = new System.Drawing.Point(10, 12);
            this.xtc.Name = "xtc";
            this.xtc.SelectedTabPage = this.xtpUserInfo;
            this.xtc.Size = new System.Drawing.Size(375, 181);
            this.xtc.TabIndex = 14;
            this.xtc.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtpUserInfo,
            this.xtpFtpInfo});
            // 
            // xtpFtpInfo
            // 
            this.xtpFtpInfo.Controls.Add(this.panelControl1);
            this.xtpFtpInfo.Name = "xtpFtpInfo";
            this.xtpFtpInfo.Size = new System.Drawing.Size(369, 152);
            this.xtpFtpInfo.Text = "ftp服务设置";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Controls.Add(this.labelControl5);
            this.panelControl1.Controls.Add(this.teFtpPassword);
            this.panelControl1.Controls.Add(this.teFtpUserName);
            this.panelControl1.Controls.Add(this.teFtpAddr);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(369, 152);
            this.panelControl1.TabIndex = 4;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(42, 103);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(24, 14);
            this.labelControl3.TabIndex = 24;
            this.labelControl3.Text = "密码";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(42, 69);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(36, 14);
            this.labelControl4.TabIndex = 23;
            this.labelControl4.Text = "用户名";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(42, 35);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(35, 14);
            this.labelControl5.TabIndex = 21;
            this.labelControl5.Text = "IP地址";
            // 
            // teFtpPassword
            // 
            this.teFtpPassword.Location = new System.Drawing.Point(117, 100);
            this.teFtpPassword.Name = "teFtpPassword";
            this.teFtpPassword.Properties.PasswordChar = '*';
            this.teFtpPassword.Size = new System.Drawing.Size(173, 20);
            this.teFtpPassword.TabIndex = 20;
            // 
            // teFtpUserName
            // 
            this.teFtpUserName.Location = new System.Drawing.Point(117, 66);
            this.teFtpUserName.Name = "teFtpUserName";
            this.teFtpUserName.Size = new System.Drawing.Size(173, 20);
            this.teFtpUserName.TabIndex = 19;
            // 
            // teFtpAddr
            // 
            this.teFtpAddr.Location = new System.Drawing.Point(117, 32);
            this.teFtpAddr.Name = "teFtpAddr";
            this.teFtpAddr.Size = new System.Drawing.Size(173, 20);
            this.teFtpAddr.TabIndex = 17;
            // 
            // SetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(389, 232);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.xtc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SetForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置";
            this.Load += new System.EventHandler(this.SetForm_Load);
            this.xtpUserInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl5)).EndInit();
            this.panelControl5.ResumeLayout(false);
            this.panelControl5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teDataBase.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataPort.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teDataAddr.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xtc)).EndInit();
            this.xtc.ResumeLayout(false);
            this.xtpFtpInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.teFtpPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teFtpUserName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teFtpAddr.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraTab.XtraTabPage xtpUserInfo;
        private DevExpress.XtraTab.XtraTabControl xtc;
        private DevExpress.XtraEditors.PanelControl panelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl11;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.TextEdit teDataBase;
        private DevExpress.XtraEditors.TextEdit teDataPort;
        private DevExpress.XtraEditors.TextEdit teDataPassword;
        private DevExpress.XtraEditors.TextEdit teDataUserName;
        private DevExpress.XtraEditors.TextEdit teDataAddr;
        private DevExpress.XtraTab.XtraTabPage xtpFtpInfo;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit teFtpPassword;
        private DevExpress.XtraEditors.TextEdit teFtpUserName;
        private DevExpress.XtraEditors.TextEdit teFtpAddr;
    }
}