namespace FuelDataSysClient.SubForm
{
    partial class GlobalExceptionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlobalExceptionForm));
            this.btnFeedback = new System.Windows.Forms.Button();
            this.linkLblSystemDirectory = new System.Windows.Forms.LinkLabel();
            this.linkLblCurrentDirectory = new System.Windows.Forms.LinkLabel();
            this.linkLblHelpLink = new System.Windows.Forms.LinkLabel();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtTargeSite = new System.Windows.Forms.TextBox();
            this.txtStackTrace = new System.Windows.Forms.TextBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblSystemDirectory = new System.Windows.Forms.Label();
            this.lblOSVersion = new System.Windows.Forms.Label();
            this.lblMachineName = new System.Windows.Forms.Label();
            this.lblCurrentDirectory = new System.Windows.Forms.Label();
            this.lblTargeSite = new System.Windows.Forms.Label();
            this.lblStackTrace = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblHelpLink = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pbEnvironment = new System.Windows.Forms.PictureBox();
            this.pbInfo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbEnvironment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFeedback
            // 
            this.btnFeedback.Location = new System.Drawing.Point(26, 389);
            this.btnFeedback.Name = "btnFeedback";
            this.btnFeedback.Size = new System.Drawing.Size(120, 21);
            this.btnFeedback.TabIndex = 24;
            this.btnFeedback.Text = "发送反馈信息(&F)";
            this.btnFeedback.UseVisualStyleBackColor = true;
            this.btnFeedback.Click += new System.EventHandler(this.btnFeedback_Click);
            // 
            // linkLblSystemDirectory
            // 
            this.linkLblSystemDirectory.AutoSize = true;
            this.linkLblSystemDirectory.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLblSystemDirectory.Location = new System.Drawing.Point(146, 318);
            this.linkLblSystemDirectory.Name = "linkLblSystemDirectory";
            this.linkLblSystemDirectory.Size = new System.Drawing.Size(0, 14);
            this.linkLblSystemDirectory.TabIndex = 41;
            // 
            // linkLblCurrentDirectory
            // 
            this.linkLblCurrentDirectory.AutoSize = true;
            this.linkLblCurrentDirectory.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLblCurrentDirectory.Location = new System.Drawing.Point(146, 247);
            this.linkLblCurrentDirectory.Name = "linkLblCurrentDirectory";
            this.linkLblCurrentDirectory.Size = new System.Drawing.Size(0, 14);
            this.linkLblCurrentDirectory.TabIndex = 37;
            // 
            // linkLblHelpLink
            // 
            this.linkLblHelpLink.AutoSize = true;
            this.linkLblHelpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLblHelpLink.Location = new System.Drawing.Point(146, 58);
            this.linkLblHelpLink.Name = "linkLblHelpLink";
            this.linkLblHelpLink.Size = new System.Drawing.Size(0, 14);
            this.linkLblHelpLink.TabIndex = 29;
            // 
            // btnIgnore
            // 
            this.btnIgnore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnIgnore.Location = new System.Drawing.Point(476, 389);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(120, 21);
            this.btnIgnore.TabIndex = 45;
            this.btnIgnore.Text = "忽略当前错误(&I)";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(350, 389);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(120, 21);
            this.btnAbort.TabIndex = 44;
            this.btnAbort.Text = "中止程序运行(&A)";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // txtSource
            // 
            this.txtSource.BackColor = System.Drawing.SystemColors.Window;
            this.txtSource.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtSource.Location = new System.Drawing.Point(148, 77);
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.Size = new System.Drawing.Size(448, 22);
            this.txtSource.TabIndex = 31;
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtMessage.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMessage.Location = new System.Drawing.Point(148, 32);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(448, 22);
            this.txtMessage.TabIndex = 27;
            // 
            // txtTargeSite
            // 
            this.txtTargeSite.BackColor = System.Drawing.SystemColors.Window;
            this.txtTargeSite.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtTargeSite.Location = new System.Drawing.Point(148, 220);
            this.txtTargeSite.Name = "txtTargeSite";
            this.txtTargeSite.ReadOnly = true;
            this.txtTargeSite.Size = new System.Drawing.Size(448, 22);
            this.txtTargeSite.TabIndex = 35;
            // 
            // txtStackTrace
            // 
            this.txtStackTrace.BackColor = System.Drawing.SystemColors.Window;
            this.txtStackTrace.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtStackTrace.Location = new System.Drawing.Point(148, 101);
            this.txtStackTrace.Multiline = true;
            this.txtStackTrace.Name = "txtStackTrace";
            this.txtStackTrace.ReadOnly = true;
            this.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStackTrace.Size = new System.Drawing.Size(448, 116);
            this.txtStackTrace.TabIndex = 33;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(92, 362);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(62, 14);
            this.lblVersion.TabIndex = 43;
            this.lblVersion.Text = ".NET版本:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(104, 340);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(47, 14);
            this.lblUserName.TabIndex = 42;
            this.lblUserName.Text = "用户名:";
            // 
            // lblSystemDirectory
            // 
            this.lblSystemDirectory.AutoSize = true;
            this.lblSystemDirectory.Location = new System.Drawing.Point(92, 318);
            this.lblSystemDirectory.Name = "lblSystemDirectory";
            this.lblSystemDirectory.Size = new System.Drawing.Size(59, 14);
            this.lblSystemDirectory.TabIndex = 40;
            this.lblSystemDirectory.Text = "系统路径:";
            // 
            // lblOSVersion
            // 
            this.lblOSVersion.AutoSize = true;
            this.lblOSVersion.Location = new System.Drawing.Point(92, 295);
            this.lblOSVersion.Name = "lblOSVersion";
            this.lblOSVersion.Size = new System.Drawing.Size(59, 14);
            this.lblOSVersion.TabIndex = 39;
            this.lblOSVersion.Text = "操作系统:";
            // 
            // lblMachineName
            // 
            this.lblMachineName.AutoSize = true;
            this.lblMachineName.Location = new System.Drawing.Point(104, 270);
            this.lblMachineName.Name = "lblMachineName";
            this.lblMachineName.Size = new System.Drawing.Size(47, 14);
            this.lblMachineName.TabIndex = 38;
            this.lblMachineName.Text = "机器名:";
            // 
            // lblCurrentDirectory
            // 
            this.lblCurrentDirectory.AutoSize = true;
            this.lblCurrentDirectory.Location = new System.Drawing.Point(92, 247);
            this.lblCurrentDirectory.Name = "lblCurrentDirectory";
            this.lblCurrentDirectory.Size = new System.Drawing.Size(59, 14);
            this.lblCurrentDirectory.TabIndex = 36;
            this.lblCurrentDirectory.Text = "当前路径:";
            // 
            // lblTargeSite
            // 
            this.lblTargeSite.AutoSize = true;
            this.lblTargeSite.Location = new System.Drawing.Point(116, 223);
            this.lblTargeSite.Name = "lblTargeSite";
            this.lblTargeSite.Size = new System.Drawing.Size(35, 14);
            this.lblTargeSite.TabIndex = 34;
            this.lblTargeSite.Text = "方法:";
            // 
            // lblStackTrace
            // 
            this.lblStackTrace.AutoSize = true;
            this.lblStackTrace.Location = new System.Drawing.Point(116, 104);
            this.lblStackTrace.Name = "lblStackTrace";
            this.lblStackTrace.Size = new System.Drawing.Size(35, 14);
            this.lblStackTrace.TabIndex = 32;
            this.lblStackTrace.Text = "堆栈:";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(116, 80);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(35, 14);
            this.lblSource.TabIndex = 30;
            this.lblSource.Text = "对象:";
            // 
            // lblHelpLink
            // 
            this.lblHelpLink.AutoSize = true;
            this.lblHelpLink.Location = new System.Drawing.Point(92, 58);
            this.lblHelpLink.Name = "lblHelpLink";
            this.lblHelpLink.Size = new System.Drawing.Size(59, 14);
            this.lblHelpLink.TabIndex = 28;
            this.lblHelpLink.Text = "帮助链接:";
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(26, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(570, 20);
            this.lblInfo.TabIndex = 25;
            this.lblInfo.Text = "{0} 遇到问题需要关闭。我们对此引起的不便表示抱歉。请将此问题报告给 {1}。";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(116, 35);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(35, 14);
            this.lblMessage.TabIndex = 26;
            this.lblMessage.Text = "信息:";
            // 
            // pbEnvironment
            // 
            this.pbEnvironment.Image = ((System.Drawing.Image)(resources.GetObject("pbEnvironment.Image")));
            this.pbEnvironment.Location = new System.Drawing.Point(26, 247);
            this.pbEnvironment.Name = "pbEnvironment";
            this.pbEnvironment.Size = new System.Drawing.Size(60, 60);
            this.pbEnvironment.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbEnvironment.TabIndex = 23;
            this.pbEnvironment.TabStop = false;
            // 
            // pbInfo
            // 
            this.pbInfo.Image = ((System.Drawing.Image)(resources.GetObject("pbInfo.Image")));
            this.pbInfo.Location = new System.Drawing.Point(26, 32);
            this.pbInfo.Name = "pbInfo";
            this.pbInfo.Size = new System.Drawing.Size(60, 60);
            this.pbInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbInfo.TabIndex = 22;
            this.pbInfo.TabStop = false;
            // 
            // GlobalExceptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 423);
            this.Controls.Add(this.btnFeedback);
            this.Controls.Add(this.linkLblSystemDirectory);
            this.Controls.Add(this.linkLblCurrentDirectory);
            this.Controls.Add(this.linkLblHelpLink);
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtTargeSite);
            this.Controls.Add(this.txtStackTrace);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblSystemDirectory);
            this.Controls.Add(this.lblOSVersion);
            this.Controls.Add(this.lblMachineName);
            this.Controls.Add(this.lblCurrentDirectory);
            this.Controls.Add(this.lblTargeSite);
            this.Controls.Add(this.lblStackTrace);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.lblHelpLink);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pbEnvironment);
            this.Controls.Add(this.pbInfo);
            this.Name = "GlobalExceptionForm";
            this.ShowIcon = false;
            ((System.ComponentModel.ISupportInitialize)(this.pbEnvironment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFeedback;
        private System.Windows.Forms.LinkLabel linkLblSystemDirectory;
        private System.Windows.Forms.LinkLabel linkLblCurrentDirectory;
        private System.Windows.Forms.LinkLabel linkLblHelpLink;
        private System.Windows.Forms.Button btnIgnore;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.TextBox txtTargeSite;
        private System.Windows.Forms.TextBox txtStackTrace;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblSystemDirectory;
        private System.Windows.Forms.Label lblOSVersion;
        private System.Windows.Forms.Label lblMachineName;
        private System.Windows.Forms.Label lblCurrentDirectory;
        private System.Windows.Forms.Label lblTargeSite;
        private System.Windows.Forms.Label lblStackTrace;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblHelpLink;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.PictureBox pbEnvironment;
        private System.Windows.Forms.PictureBox pbInfo;
    }
}