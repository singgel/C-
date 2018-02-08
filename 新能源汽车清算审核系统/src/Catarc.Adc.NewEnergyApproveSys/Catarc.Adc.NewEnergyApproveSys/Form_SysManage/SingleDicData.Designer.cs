namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    partial class SingleDicData
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
            this.components = new System.ComponentModel.Container();
            this.gbOperatorInfo = new System.Windows.Forms.GroupBox();
            this.cmb_Type = new DevExpress.XtraEditors.ComboBoxEdit();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDicName = new System.Windows.Forms.TextBox();
            this.lblOperatorName = new System.Windows.Forms.Label();
            this.btn_Save = new System.Windows.Forms.Button();
            this.btn_Cancle = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gbOperatorInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmb_Type.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gbOperatorInfo
            // 
            this.gbOperatorInfo.Controls.Add(this.cmb_Type);
            this.gbOperatorInfo.Controls.Add(this.label8);
            this.gbOperatorInfo.Controls.Add(this.label5);
            this.gbOperatorInfo.Controls.Add(this.label2);
            this.gbOperatorInfo.Controls.Add(this.txtDicName);
            this.gbOperatorInfo.Controls.Add(this.lblOperatorName);
            this.gbOperatorInfo.Location = new System.Drawing.Point(12, 12);
            this.gbOperatorInfo.Name = "gbOperatorInfo";
            this.gbOperatorInfo.Size = new System.Drawing.Size(337, 129);
            this.gbOperatorInfo.TabIndex = 9;
            this.gbOperatorInfo.TabStop = false;
            this.gbOperatorInfo.Text = "数据字典";
            // 
            // cmb_Type
            // 
            this.cmb_Type.Location = new System.Drawing.Point(112, 84);
            this.cmb_Type.Name = "cmb_Type";
            this.cmb_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cmb_Type.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cmb_Type.Size = new System.Drawing.Size(195, 20);
            this.cmb_Type.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(314, 84);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(14, 14);
            this.label8.TabIndex = 17;
            this.label8.Text = "*";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(314, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 14);
            this.label5.TabIndex = 14;
            this.label5.Text = "*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "类型:";
            // 
            // txtDicName
            // 
            this.txtDicName.Location = new System.Drawing.Point(112, 31);
            this.txtDicName.Name = "txtDicName";
            this.txtDicName.Size = new System.Drawing.Size(196, 22);
            this.txtDicName.TabIndex = 1;
            // 
            // lblOperatorName
            // 
            this.lblOperatorName.AutoSize = true;
            this.lblOperatorName.Location = new System.Drawing.Point(29, 37);
            this.lblOperatorName.Name = "lblOperatorName";
            this.lblOperatorName.Size = new System.Drawing.Size(35, 14);
            this.lblOperatorName.TabIndex = 0;
            this.lblOperatorName.Text = "名称:";
            // 
            // btn_Save
            // 
            this.btn_Save.Location = new System.Drawing.Point(185, 157);
            this.btn_Save.Name = "btn_Save";
            this.btn_Save.Size = new System.Drawing.Size(59, 27);
            this.btn_Save.TabIndex = 10;
            this.btn_Save.Text = "保存";
            this.btn_Save.UseVisualStyleBackColor = true;
            this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
            // 
            // btn_Cancle
            // 
            this.btn_Cancle.Location = new System.Drawing.Point(290, 157);
            this.btn_Cancle.Name = "btn_Cancle";
            this.btn_Cancle.Size = new System.Drawing.Size(59, 27);
            this.btn_Cancle.TabIndex = 11;
            this.btn_Cancle.Text = "取消";
            this.btn_Cancle.UseVisualStyleBackColor = true;
            this.btn_Cancle.Click += new System.EventHandler(this.btn_Cancle_Click);
            // 
            // SingleDicData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 197);
            this.Controls.Add(this.btn_Cancle);
            this.Controls.Add(this.btn_Save);
            this.Controls.Add(this.gbOperatorInfo);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SingleDicData";
            this.ShowIcon = false;
            this.gbOperatorInfo.ResumeLayout(false);
            this.gbOperatorInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cmb_Type.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbOperatorInfo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDicName;
        private System.Windows.Forms.Label lblOperatorName;
        private DevExpress.XtraEditors.ComboBoxEdit cmb_Type;
        private System.Windows.Forms.Button btn_Save;
        private System.Windows.Forms.Button btn_Cancle;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}