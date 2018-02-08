namespace FuelDataSysClient.Form_SJSB
{
    partial class RLLXParamForm
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
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
            this.gcOCN_RLLXPARAM = new DevExpress.XtraGrid.GridControl();
            this.gvOCN_RLLXPARAM = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.CSBM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CSMC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CSZ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnNO = new DevExpress.XtraEditors.SimpleButton();
            this.btnYES = new DevExpress.XtraEditors.SimpleButton();
            this.teSC_OCN = new DevExpress.XtraEditors.TextEdit();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.cbeRLLX = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labVERSION = new DevExpress.XtraEditors.LabelControl();
            this.labOPERATION = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcOCN_RLLXPARAM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvOCN_RLLXPARAM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.teSC_OCN.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeRLLX.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // gcOCN_RLLXPARAM
            // 
            this.gcOCN_RLLXPARAM.Location = new System.Drawing.Point(12, 58);
            this.gcOCN_RLLXPARAM.MainView = this.gvOCN_RLLXPARAM;
            this.gcOCN_RLLXPARAM.Name = "gcOCN_RLLXPARAM";
            this.gcOCN_RLLXPARAM.Size = new System.Drawing.Size(768, 385);
            this.gcOCN_RLLXPARAM.TabIndex = 0;
            this.gcOCN_RLLXPARAM.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvOCN_RLLXPARAM});
            // 
            // gvOCN_RLLXPARAM
            // 
            this.gvOCN_RLLXPARAM.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.CSBM,
            this.CSMC,
            this.CSZ});
            this.gvOCN_RLLXPARAM.GridControl = this.gcOCN_RLLXPARAM;
            this.gvOCN_RLLXPARAM.Name = "gvOCN_RLLXPARAM";
            this.gvOCN_RLLXPARAM.OptionsView.ShowGroupPanel = false;
            // 
            // CSBM
            // 
            this.CSBM.Caption = "参数编码";
            this.CSBM.FieldName = "CSBM";
            this.CSBM.Name = "CSBM";
            this.CSBM.OptionsColumn.ReadOnly = true;
            this.CSBM.Visible = true;
            this.CSBM.VisibleIndex = 0;
            // 
            // CSMC
            // 
            this.CSMC.Caption = "参数名称";
            this.CSMC.FieldName = "CSMC";
            this.CSMC.Name = "CSMC";
            this.CSMC.OptionsColumn.ReadOnly = true;
            this.CSMC.Visible = true;
            this.CSMC.VisibleIndex = 1;
            // 
            // CSZ
            // 
            this.CSZ.Caption = "参数值";
            this.CSZ.FieldName = "CSZ";
            this.CSZ.Name = "CSZ";
            this.CSZ.Visible = true;
            this.CSZ.VisibleIndex = 2;
            // 
            // btnNO
            // 
            this.btnNO.Appearance.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNO.Appearance.Options.UseFont = true;
            this.btnNO.Location = new System.Drawing.Point(429, 460);
            this.btnNO.Name = "btnNO";
            this.btnNO.Size = new System.Drawing.Size(100, 30);
            this.btnNO.TabIndex = 3;
            this.btnNO.Text = "取消";
            this.btnNO.Click += new System.EventHandler(this.btnNO_Click);
            // 
            // btnYES
            // 
            this.btnYES.Appearance.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYES.Appearance.Options.UseFont = true;
            this.btnYES.Location = new System.Drawing.Point(186, 460);
            this.btnYES.Name = "btnYES";
            this.btnYES.Size = new System.Drawing.Size(100, 30);
            this.btnYES.TabIndex = 2;
            this.btnYES.Text = "确定";
            this.btnYES.Click += new System.EventHandler(this.btnYES_Click);
            // 
            // teSC_OCN
            // 
            this.teSC_OCN.Location = new System.Drawing.Point(186, 21);
            this.teSC_OCN.Name = "teSC_OCN";
            this.teSC_OCN.Size = new System.Drawing.Size(120, 20);
            this.teSC_OCN.TabIndex = 58;
            this.teSC_OCN.Validating += new System.ComponentModel.CancelEventHandler(this.teSC_OCN_Validating);
            // 
            // labelControl10
            // 
            this.labelControl10.Location = new System.Drawing.Point(385, 24);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(48, 14);
            this.labelControl10.TabIndex = 57;
            this.labelControl10.Text = "燃料类型";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(121, 24);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(24, 14);
            this.labelControl1.TabIndex = 56;
            this.labelControl1.Text = "OCN";
            // 
            // cbeRLLX
            // 
            this.cbeRLLX.Location = new System.Drawing.Point(473, 21);
            this.cbeRLLX.Name = "cbeRLLX";
            this.cbeRLLX.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbeRLLX.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.cbeRLLX.Size = new System.Drawing.Size(120, 20);
            this.cbeRLLX.TabIndex = 61;
            this.cbeRLLX.SelectedIndexChanged += new System.EventHandler(this.cbeRLLX_SelectedIndexChanged);
            this.cbeRLLX.Validating += new System.ComponentModel.CancelEventHandler(this.cbeRLLX_Validating);
            // 
            // labVERSION
            // 
            this.labVERSION.Location = new System.Drawing.Point(669, 24);
            this.labVERSION.Name = "labVERSION";
            this.labVERSION.Size = new System.Drawing.Size(111, 14);
            this.labVERSION.TabIndex = 57;
            this.labVERSION.Text = "燃料参数版本号：V0";
            this.labVERSION.Visible = false;
            // 
            // labOPERATION
            // 
            this.labOPERATION.Location = new System.Drawing.Point(669, 4);
            this.labOPERATION.Name = "labOPERATION";
            this.labOPERATION.Size = new System.Drawing.Size(99, 14);
            this.labOPERATION.TabIndex = 57;
            this.labOPERATION.Text = "原操作类型号：V0";
            this.labOPERATION.Visible = false;
            // 
            // RLLXParamForm
            // 
            this.Appearance.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(792, 507);
            this.Controls.Add(this.cbeRLLX);
            this.Controls.Add(this.teSC_OCN);
            this.Controls.Add(this.labOPERATION);
            this.Controls.Add(this.labVERSION);
            this.Controls.Add(this.labelControl10);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnNO);
            this.Controls.Add(this.btnYES);
            this.Controls.Add(this.gcOCN_RLLXPARAM);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RLLXParamForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "燃料参数新增";
            this.Load += new System.EventHandler(this.RLLXParamForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcOCN_RLLXPARAM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvOCN_RLLXPARAM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.teSC_OCN.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbeRLLX.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
        private DevExpress.XtraGrid.GridControl gcOCN_RLLXPARAM;
        private DevExpress.XtraGrid.Views.Grid.GridView gvOCN_RLLXPARAM;
        private DevExpress.XtraEditors.SimpleButton btnNO;
        private DevExpress.XtraEditors.SimpleButton btnYES;
        private DevExpress.XtraEditors.TextEdit teSC_OCN;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.Columns.GridColumn CSBM;
        private DevExpress.XtraGrid.Columns.GridColumn CSMC;
        private DevExpress.XtraGrid.Columns.GridColumn CSZ;
        private DevExpress.XtraEditors.ComboBoxEdit cbeRLLX;
        private DevExpress.XtraEditors.LabelControl labVERSION;
        private DevExpress.XtraEditors.LabelControl labOPERATION;


    }
}