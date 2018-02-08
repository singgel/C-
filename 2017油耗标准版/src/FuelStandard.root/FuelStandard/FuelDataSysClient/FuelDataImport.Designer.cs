namespace FuelDataSysClient
{
    partial class FuelDataImportForm
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
            this.btnVIN = new DevExpress.XtraEditors.SimpleButton();
            this.teVIN = new DevExpress.XtraEditors.TextEdit();
            this.btnCOC = new DevExpress.XtraEditors.SimpleButton();
            this.teCOC = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.teVIN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCOC.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnVIN
            // 
            this.btnVIN.Location = new System.Drawing.Point(243, 19);
            this.btnVIN.Name = "btnVIN";
            this.btnVIN.Size = new System.Drawing.Size(75, 23);
            this.btnVIN.TabIndex = 0;
            this.btnVIN.Text = "导入VIN";
            this.btnVIN.Click += new System.EventHandler(this.btnVIN_Click);
            // 
            // teVIN
            // 
            this.teVIN.Location = new System.Drawing.Point(28, 19);
            this.teVIN.Name = "teVIN";
            this.teVIN.Size = new System.Drawing.Size(204, 20);
            this.teVIN.TabIndex = 1;
            // 
            // btnCOC
            // 
            this.btnCOC.Location = new System.Drawing.Point(243, 54);
            this.btnCOC.Name = "btnCOC";
            this.btnCOC.Size = new System.Drawing.Size(75, 23);
            this.btnCOC.TabIndex = 0;
            this.btnCOC.Text = "导入COC";
            this.btnCOC.Click += new System.EventHandler(this.btnCOC_Click);
            // 
            // teCOC
            // 
            this.teCOC.Location = new System.Drawing.Point(28, 54);
            this.teCOC.Name = "teCOC";
            this.teCOC.Size = new System.Drawing.Size(204, 20);
            this.teCOC.TabIndex = 1;
            // 
            // FuelDataImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(842, 462);
            this.Controls.Add(this.teCOC);
            this.Controls.Add(this.btnCOC);
            this.Controls.Add(this.teVIN);
            this.Controls.Add(this.btnVIN);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FuelDataImportForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "燃料数据导入";
            ((System.ComponentModel.ISupportInitialize)(this.teVIN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teCOC.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnVIN;
        private DevExpress.XtraEditors.TextEdit teVIN;
        private DevExpress.XtraEditors.SimpleButton btnCOC;
        private DevExpress.XtraEditors.TextEdit teCOC;
    }
}