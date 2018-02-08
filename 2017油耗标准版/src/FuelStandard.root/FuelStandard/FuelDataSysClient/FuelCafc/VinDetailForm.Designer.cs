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
            this.lblSum = new DevExpress.XtraEditors.LabelControl();
            this.tbClxh = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tbVin = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnExport = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
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
            ((System.ComponentModel.ISupportInitialize)(this.tbClxh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcVinData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVinData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lblSum);
            this.panelControl1.Controls.Add(this.tbClxh);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.tbVin);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.btnExport);
            this.panelControl1.Controls.Add(this.btnSearch);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(984, 70);
            this.panelControl1.TabIndex = 10;
            // 
            // lblSum
            // 
            this.lblSum.Location = new System.Drawing.Point(17, 41);
            this.lblSum.Name = "lblSum";
            this.lblSum.Size = new System.Drawing.Size(32, 14);
            this.lblSum.TabIndex = 55;
            this.lblSum.Text = "共  条";
            // 
            // tbClxh
            // 
            this.tbClxh.Location = new System.Drawing.Point(326, 13);
            this.tbClxh.Name = "tbClxh";
            this.tbClxh.Size = new System.Drawing.Size(130, 20);
            this.tbClxh.TabIndex = 43;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(255, 16);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 42;
            this.labelControl2.Text = "产品型号";
            // 
            // tbVin
            // 
            this.tbVin.Location = new System.Drawing.Point(89, 13);
            this.tbVin.Name = "tbVin";
            this.tbVin.Size = new System.Drawing.Size(130, 20);
            this.tbVin.TabIndex = 41;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(17, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 14);
            this.labelControl1.TabIndex = 40;
            this.labelControl1.Text = "备案号(VIN)";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(662, 13);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 20);
            this.btnExport.TabIndex = 54;
            this.btnExport.Text = "导出";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(498, 13);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(130, 20);
            this.btnSearch.TabIndex = 54;
            this.btnSearch.Text = "查询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // gcVinData
            // 
            this.gcVinData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcVinData.Location = new System.Drawing.Point(0, 70);
            this.gcVinData.MainView = this.gvVinData;
            this.gcVinData.Name = "gcVinData";
            this.gcVinData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gcVinData.Size = new System.Drawing.Size(984, 421);
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
            this.VIN.OptionsColumn.AllowEdit = false;
            this.VIN.Visible = true;
            this.VIN.VisibleIndex = 0;
            // 
            // QCSCQY
            // 
            this.QCSCQY.Caption = "乘用车生产企业";
            this.QCSCQY.FieldName = "Qcscqy";
            this.QCSCQY.Name = "QCSCQY";
            this.QCSCQY.OptionsColumn.AllowEdit = false;
            // 
            // CLXH
            // 
            this.CLXH.Caption = "产品型号";
            this.CLXH.FieldName = "Clxh";
            this.CLXH.Name = "CLXH";
            this.CLXH.OptionsColumn.AllowEdit = false;
            this.CLXH.Visible = true;
            this.CLXH.VisibleIndex = 1;
            // 
            // RLLX
            // 
            this.RLLX.Caption = "燃料种类";
            this.RLLX.FieldName = "Rllx";
            this.RLLX.Name = "RLLX";
            this.RLLX.OptionsColumn.AllowEdit = false;
            this.RLLX.Visible = true;
            this.RLLX.VisibleIndex = 2;
            // 
            // Zwps
            // 
            this.Zwps.Caption = "座椅排数";
            this.Zwps.FieldName = "Zwps";
            this.Zwps.Name = "Zwps";
            this.Zwps.OptionsColumn.AllowEdit = false;
            this.Zwps.Visible = true;
            this.Zwps.VisibleIndex = 3;
            // 
            // Zczbzl
            // 
            this.Zczbzl.Caption = "整备质量";
            this.Zczbzl.FieldName = "Zczbzl";
            this.Zczbzl.Name = "Zczbzl";
            this.Zczbzl.OptionsColumn.AllowEdit = false;
            this.Zczbzl.Visible = true;
            this.Zczbzl.VisibleIndex = 4;
            // 
            // CLZZRQ
            // 
            this.CLZZRQ.Caption = "车辆制造日期/进口核销日期";
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
            this.Createtime.OptionsColumn.AllowEdit = false;
            this.Createtime.Visible = true;
            this.Createtime.VisibleIndex = 6;
            // 
            // Bsqxs
            // 
            this.Bsqxs.Caption = "变速器型式";
            this.Bsqxs.FieldName = "Bsqxs";
            this.Bsqxs.Name = "Bsqxs";
            this.Bsqxs.OptionsColumn.AllowEdit = false;
            this.Bsqxs.Visible = true;
            this.Bsqxs.VisibleIndex = 7;
            // 
            // Zhgkxslc
            // 
            this.Zhgkxslc.Caption = "综合工况续驶里程";
            this.Zhgkxslc.FieldName = "Zhgkxslc";
            this.Zhgkxslc.Name = "Zhgkxslc";
            this.Zhgkxslc.OptionsColumn.AllowEdit = false;
            this.Zhgkxslc.Visible = true;
            this.Zhgkxslc.VisibleIndex = 8;
            // 
            // Zhgkrlxhl
            // 
            this.Zhgkrlxhl.Caption = "燃料消耗量（综合）";
            this.Zhgkrlxhl.FieldName = "Zhgkrlxhl";
            this.Zhgkrlxhl.Name = "Zhgkrlxhl";
            this.Zhgkrlxhl.OptionsColumn.AllowEdit = false;
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
            this.Load += new System.EventHandler(this.VinDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbClxh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcVinData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvVinData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblSum;
        private DevExpress.XtraEditors.TextEdit tbClxh;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit tbVin;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
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