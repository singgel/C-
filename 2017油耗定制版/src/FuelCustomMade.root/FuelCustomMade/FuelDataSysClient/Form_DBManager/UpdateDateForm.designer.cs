namespace FuelDataSysClient.Form_DBManager
{
    partial class UpdateDateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDateForm));
            this.deClzzrq = new DevExpress.XtraEditors.DateEdit();
            this.lblClzzrq = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveDate = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.deClzzrq.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deClzzrq.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // deClzzrq
            // 
            this.deClzzrq.EditValue = null;
            this.deClzzrq.Location = new System.Drawing.Point(170, 44);
            this.deClzzrq.Name = "deClzzrq";
            this.deClzzrq.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.deClzzrq.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.deClzzrq.Size = new System.Drawing.Size(170, 20);
            this.deClzzrq.TabIndex = 0;
            // 
            // lblClzzrq
            // 
            this.lblClzzrq.Location = new System.Drawing.Point(22, 47);
            this.lblClzzrq.Name = "lblClzzrq";
            this.lblClzzrq.Size = new System.Drawing.Size(101, 14);
            this.lblClzzrq.TabIndex = 1;
            this.lblClzzrq.Text = "下线日期/生产日期";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(188, 113);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 20);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "关闭";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSaveDate
            // 
            this.btnSaveDate.Location = new System.Drawing.Point(102, 113);
            this.btnSaveDate.Name = "btnSaveDate";
            this.btnSaveDate.Size = new System.Drawing.Size(64, 20);
            this.btnSaveDate.TabIndex = 5;
            this.btnSaveDate.Text = "保存";
            this.btnSaveDate.Click += new System.EventHandler(this.btnSaveDate_Click);
            // 
            // UpdateDateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 158);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSaveDate);
            this.Controls.Add(this.lblClzzrq);
            this.Controls.Add(this.deClzzrq);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateDateForm";
            this.Text = "修改下线日期/生产日期";
            ((System.ComponentModel.ISupportInitialize)(this.deClzzrq.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deClzzrq.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.DateEdit deClzzrq;
        private DevExpress.XtraEditors.LabelControl lblClzzrq;
        private DevExpress.XtraEditors.SimpleButton btnClose;
        private DevExpress.XtraEditors.SimpleButton btnSaveDate;
    }
}