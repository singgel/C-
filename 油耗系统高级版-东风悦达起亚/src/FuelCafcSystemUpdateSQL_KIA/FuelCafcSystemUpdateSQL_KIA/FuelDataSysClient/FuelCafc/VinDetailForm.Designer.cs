namespace FuelDataSysClient.FuelCafc
{
    partial class VinDetailForm
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
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.gcVinData = new DevExpress.XtraGrid.GridControl();
            this.gvVinData = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.VIN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.QCSCQY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLXH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RLLX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Zwps = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Zczbzl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLZZRQ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Createtime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Bsqxs = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Zhgkxslc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Zhgkrlxhl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcVinData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVinData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnExport);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(984, 43);
            this.panelControl1.TabIndex = 10;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(12, 12);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 20);
            this.btnExport.TabIndex = 54;
            this.btnExport.Text = "导出";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // gcVinData
            // 
            this.gcVinData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcVinData.Location = new System.Drawing.Point(0, 43);
            this.gcVinData.MainView = this.gvVinData;
            this.gcVinData.Name = "gcVinData";
            this.gcVinData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gcVinData.Size = new System.Drawing.Size(984, 448);
            this.gcVinData.TabIndex = 11;
            this.gcVinData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvVinData});
            // 
            // gvVinData
            // 
            this.gvVinData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.VIN,
            this.QCSCQY,
            this.CLXH,
            this.RLLX,
            this.Zwps,
            this.Zczbzl,
            this.CLZZRQ,
            this.Createtime,
            this.Bsqxs,
            this.Zhgkxslc,
            this.Zhgkrlxhl});
            this.gvVinData.GridControl = this.gcVinData;
            this.gvVinData.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, "", null, "")});
            this.gvVinData.Name = "gvVinData";
            this.gvVinData.OptionsMenu.EnableColumnMenu = false;
            this.gvVinData.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvVinData.OptionsView.ShowFooter = true;
            this.gvVinData.OptionsView.ShowGroupedColumns = true;
            // 
            // VIN
            // 
            this.VIN.Caption = "备案号(VIN)";
            this.VIN.FieldName = "Vin";
            this.VIN.Name = "VIN";
            this.VIN.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, "Vin", "总计：{0}条")});
            this.VIN.Visible = true;
            this.VIN.VisibleIndex = 0;
            // 
            // QCSCQY
            // 
            this.QCSCQY.Caption = "汽车生产企业";
            this.QCSCQY.FieldName = "Qcscqy";
            this.QCSCQY.Name = "QCSCQY";
            this.QCSCQY.OptionsColumn.AllowEdit = false;
            // 
            // CLXH
            // 
            this.CLXH.Caption = "车辆型号";
            this.CLXH.FieldName = "Clxh";
            this.CLXH.Name = "CLXH";
            this.CLXH.Visible = true;
            this.CLXH.VisibleIndex = 1;
            // 
            // RLLX
            // 
            this.RLLX.Caption = "燃料类型";
            this.RLLX.FieldName = "Rllx";
            this.RLLX.Name = "RLLX";
            this.RLLX.OptionsColumn.AllowEdit = false;
            this.RLLX.Visible = true;
            this.RLLX.VisibleIndex = 2;
            // 
            // Zwps
            // 
            this.Zwps.Caption = "座位排数";
            this.Zwps.FieldName = "Zwps";
            this.Zwps.Name = "Zwps";
            this.Zwps.Visible = true;
            this.Zwps.VisibleIndex = 3;
            // 
            // Zczbzl
            // 
            this.Zczbzl.Caption = "整车整备质量";
            this.Zczbzl.FieldName = "Zczbzl";
            this.Zczbzl.Name = "Zczbzl";
            this.Zczbzl.Visible = true;
            this.Zczbzl.VisibleIndex = 4;
            // 
            // CLZZRQ
            // 
            this.CLZZRQ.Caption = "制造日期/进口日期";
            this.CLZZRQ.FieldName = "Clzzrq";
            this.CLZZRQ.Name = "CLZZRQ";
            this.CLZZRQ.OptionsColumn.AllowEdit = false;
            this.CLZZRQ.Visible = true;
            this.CLZZRQ.VisibleIndex = 5;
            // 
            // Createtime
            // 
            this.Createtime.Caption = "上报日期";
            this.Createtime.FieldName = "CreateTime";
            this.Createtime.Name = "Createtime";
            this.Createtime.Visible = true;
            this.Createtime.VisibleIndex = 6;
            // 
            // Bsqxs
            // 
            this.Bsqxs.Caption = "变速器型式";
            this.Bsqxs.FieldName = "Bsqxs";
            this.Bsqxs.Name = "Bsqxs";
            this.Bsqxs.Visible = true;
            this.Bsqxs.VisibleIndex = 7;
            // 
            // Zhgkxslc
            // 
            this.Zhgkxslc.Caption = "综合工况续驶里程";
            this.Zhgkxslc.FieldName = "Zhgkxslc";
            this.Zhgkxslc.Name = "Zhgkxslc";
            this.Zhgkxslc.Visible = true;
            this.Zhgkxslc.VisibleIndex = 8;
            // 
            // Zhgkrlxhl
            // 
            this.Zhgkrlxhl.Caption = "综合工况燃料消耗量";
            this.Zhgkrlxhl.FieldName = "Zhgkrlxhl";
            this.Zhgkrlxhl.Name = "Zhgkrlxhl";
            this.Zhgkrlxhl.Visible = true;
            this.Zhgkrlxhl.VisibleIndex = 9;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // VinDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 491);
            this.Controls.Add(this.gcVinData);
            this.Controls.Add(this.panelControl1);
            this.Name = "VinDetailForm";
            this.Text = "VIN明细";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.VinDetailForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcVinData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVinData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl gcVinData;
        private DevExpress.XtraGrid.Views.Grid.GridView gvVinData;
        private DevExpress.XtraGrid.Columns.GridColumn VIN;
        private DevExpress.XtraGrid.Columns.GridColumn QCSCQY;
        private DevExpress.XtraGrid.Columns.GridColumn CLXH;
        private DevExpress.XtraGrid.Columns.GridColumn RLLX;
        private DevExpress.XtraGrid.Columns.GridColumn Zwps;
        private DevExpress.XtraGrid.Columns.GridColumn Zczbzl;
        private DevExpress.XtraGrid.Columns.GridColumn CLZZRQ;
        private DevExpress.XtraGrid.Columns.GridColumn Createtime;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn Bsqxs;
        private DevExpress.XtraGrid.Columns.GridColumn Zhgkxslc;
        private DevExpress.XtraGrid.Columns.GridColumn Zhgkrlxhl;
        private DevExpress.XtraEditors.SimpleButton btnExport;
    }
}