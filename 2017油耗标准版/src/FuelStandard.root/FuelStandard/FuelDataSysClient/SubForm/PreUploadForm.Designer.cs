namespace FuelDataSysClient.SubForm
{
    partial class PreUploadForm
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
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barUpload = new DevExpress.XtraBars.BarButtonItem();
            this.barSelectAll = new DevExpress.XtraBars.BarButtonItem();
            this.barClearAll = new DevExpress.XtraBars.BarButtonItem();
            this.barRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barClose = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colChecked = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.colSCQY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colVIN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCLXH = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.ExpandCollapseItem.Name = "";
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.barUpload,
            this.barSelectAll,
            this.barClearAll,
            this.barRefresh,
            this.barClose});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 7;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(545, 145);
            // 
            // barUpload
            // 
            this.barUpload.Caption = "上传";
            this.barUpload.Glyph = global::FuelDataSysClient.Properties.Resources.UpdateSmall;
            this.barUpload.Id = 1;
            this.barUpload.LargeGlyph = global::FuelDataSysClient.Properties.Resources.Update;
            this.barUpload.Name = "barUpload";
            this.barUpload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barUpload_ItemClick);
            // 
            // barSelectAll
            // 
            this.barSelectAll.Caption = "全选";
            this.barSelectAll.Glyph = global::FuelDataSysClient.Properties.Resources.SelectAll;
            this.barSelectAll.Id = 2;
            this.barSelectAll.LargeGlyph = global::FuelDataSysClient.Properties.Resources.Settings1;
            this.barSelectAll.Name = "barSelectAll";
            this.barSelectAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSelectAll_ItemClick);
            // 
            // barClearAll
            // 
            this.barClearAll.Caption = "取消全选";
            this.barClearAll.Glyph = global::FuelDataSysClient.Properties.Resources.ClearAll;
            this.barClearAll.Id = 3;
            this.barClearAll.LargeGlyph = global::FuelDataSysClient.Properties.Resources.delete;
            this.barClearAll.Name = "barClearAll";
            this.barClearAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barClearAll_ItemClick);
            // 
            // barRefresh
            // 
            this.barRefresh.Caption = "刷新";
            this.barRefresh.Glyph = global::FuelDataSysClient.Properties.Resources.HtmlRefresh;
            this.barRefresh.Id = 4;
            this.barRefresh.LargeGlyph = global::FuelDataSysClient.Properties.Resources.HtmlRefreshLarge;
            this.barRefresh.Name = "barRefresh";
            this.barRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barRefresh_ItemClick);
            // 
            // barClose
            // 
            this.barClose.Caption = "关闭";
            this.barClose.Glyph = global::FuelDataSysClient.Properties.Resources.HtmlStop;
            this.barClose.Id = 5;
            this.barClose.LargeGlyph = global::FuelDataSysClient.Properties.Resources.HtmlStopLarge;
            this.barClose.Name = "barClose";
            this.barClose.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barClose_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1,
            this.ribbonPageGroup2});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "操作";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barUpload);
            this.ribbonPageGroup1.ItemLinks.Add(this.barRefresh);
            this.ribbonPageGroup1.ItemLinks.Add(this.barSelectAll);
            this.ribbonPageGroup1.ItemLinks.Add(this.barClearAll);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "操作";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barClose);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.ShowCaptionButton = false;
            this.ribbonPageGroup2.Text = "关闭";
            // 
            // gridControl1
            // 
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 145);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.MenuManager = this.ribbonControl1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gridControl1.Size = new System.Drawing.Size(545, 236);
            this.gridControl1.TabIndex = 1;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colChecked,
            this.colSCQY,
            this.colVIN,
            this.colCLXH});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            // 
            // colChecked
            // 
            this.colChecked.Caption = "选定";
            this.colChecked.ColumnEdit = this.repositoryItemCheckEdit1;
            this.colChecked.FieldName = "check";
            this.colChecked.Name = "colChecked";
            this.colChecked.Visible = true;
            this.colChecked.VisibleIndex = 0;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            this.repositoryItemCheckEdit1.ValueGrayed = "";
            // 
            // colSCQY
            // 
            this.colSCQY.Caption = "生产企业";
            this.colSCQY.FieldName = "QCSCQY";
            this.colSCQY.Name = "colSCQY";
            this.colSCQY.Visible = true;
            this.colSCQY.VisibleIndex = 1;
            // 
            // colVIN
            // 
            this.colVIN.Caption = "VIN";
            this.colVIN.FieldName = "VIN";
            this.colVIN.Name = "colVIN";
            this.colVIN.Visible = true;
            this.colVIN.VisibleIndex = 2;
            // 
            // colCLXH
            // 
            this.colCLXH.Caption = "产品型号";
            this.colCLXH.FieldName = "CLXH";
            this.colCLXH.Name = "colCLXH";
            this.colCLXH.Visible = true;
            this.colCLXH.VisibleIndex = 3;
            // 
            // PreUploadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 381);
            this.Controls.Add(this.gridControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "PreUploadForm";
            this.ShowIcon = false;
            this.Text = "数据待上传";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem barUpload;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.BarButtonItem barSelectAll;
        private DevExpress.XtraBars.BarButtonItem barClearAll;
        private DevExpress.XtraBars.BarButtonItem barRefresh;
        private DevExpress.XtraBars.BarButtonItem barClose;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraGrid.GridControl gridControl1;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colChecked;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn colSCQY;
        private DevExpress.XtraGrid.Columns.GridColumn colVIN;
        private DevExpress.XtraGrid.Columns.GridColumn colCLXH;
    }
}