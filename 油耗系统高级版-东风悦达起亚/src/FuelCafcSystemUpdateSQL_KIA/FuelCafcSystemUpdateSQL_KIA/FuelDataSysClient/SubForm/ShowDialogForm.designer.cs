namespace FuelDataSysClient.SubForm
{
    partial class ShowDialogForm
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblCaption = new DevExpress.XtraEditors.LabelControl();
            this.lblMessage = new DevExpress.XtraEditors.LabelControl();
            this.lblContent = new DevExpress.XtraEditors.LabelControl();
            this.progressShow = new DevExpress.XtraEditors.ProgressBarControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressShow.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
            this.panelControl1.Controls.Add(this.lblCaption);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(436, 34);
            this.panelControl1.TabIndex = 0;
            // 
            // lblCaption
            // 
            this.lblCaption.Location = new System.Drawing.Point(5, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(41, 14);
            this.lblCaption.TabIndex = 0;
            this.lblCaption.Text = "Caption";
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(24, 7);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(46, 14);
            this.lblMessage.TabIndex = 2;
            this.lblMessage.Text = "Message";
            // 
            // lblContent
            // 
            this.lblContent.Location = new System.Drawing.Point(24, 31);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(45, 14);
            this.lblContent.TabIndex = 3;
            this.lblContent.Text = "Content";
            // 
            // progressShow
            // 
            this.progressShow.EditValue = 1;
            this.progressShow.Location = new System.Drawing.Point(24, 59);
            this.progressShow.Name = "progressShow";
            this.progressShow.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.progressShow.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.progressShow.Properties.EndColor = System.Drawing.Color.Empty;
            this.progressShow.Properties.LookAndFeel.SkinName = "Blue";
            this.progressShow.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
            this.progressShow.Properties.LookAndFeel.UseWindowsXPTheme = true;
            this.progressShow.Properties.ReadOnly = true;
            this.progressShow.Properties.ShowTitle = true;
            this.progressShow.Properties.StartColor = System.Drawing.Color.Empty;
            this.progressShow.Properties.Step = 1;
            this.progressShow.Size = new System.Drawing.Size(400, 15);
            this.progressShow.TabIndex = 4;
            // 
            // panelControl2
            // 
            this.panelControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControl2.Controls.Add(this.lblContent);
            this.panelControl2.Controls.Add(this.progressShow);
            this.panelControl2.Controls.Add(this.lblMessage);
            this.panelControl2.Location = new System.Drawing.Point(0, 38);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(436, 83);
            this.panelControl2.TabIndex = 5;
            // 
            // ShowDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(436, 124);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowDialogForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShowDialogForm";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.progressShow.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.panelControl2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblCaption;
        private DevExpress.XtraEditors.LabelControl lblMessage;
        private DevExpress.XtraEditors.LabelControl lblContent;
        private DevExpress.XtraEditors.ProgressBarControl progressShow;
        private DevExpress.XtraEditors.PanelControl panelControl2;
    }
}