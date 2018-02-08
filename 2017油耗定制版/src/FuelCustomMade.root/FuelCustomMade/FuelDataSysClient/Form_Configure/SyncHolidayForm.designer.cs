namespace FuelDataSysClient.Form_Configure
{
    partial class SyncHolidayForm
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
            this.btnSearch = new DevExpress.XtraBars.BarButtonItem();
            this.btnSyncHoliday = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.gcHoliday = new DevExpress.XtraGrid.GridControl();
            this.gvHoliday = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.HOL_DAYS = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHoliday)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHoliday)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.ExpandCollapseItem.Name = "";
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.btnSearch,
            this.btnSyncHoliday});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 4;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1009, 147);
            // 
            // btnSearch
            // 
            this.btnSearch.Caption = "刷新";
            this.btnSearch.Id = 2;
            this.btnSearch.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_Refresh;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSearch_ItemClick);
            // 
            // btnSyncHoliday
            // 
            this.btnSyncHoliday.Caption = "同步";
            this.btnSyncHoliday.Id = 3;
            this.btnSyncHoliday.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_HolidaySync;
            this.btnSyncHoliday.Name = "btnSyncHoliday";
            this.btnSyncHoliday.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnSyncHoliday_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "操作";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSyncHoliday);
            this.ribbonPageGroup1.ItemLinks.Add(this.btnSearch);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "数据";
            // 
            // gcHoliday
            // 
            this.gcHoliday.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcHoliday.Location = new System.Drawing.Point(0, 147);
            this.gcHoliday.MainView = this.gvHoliday;
            this.gcHoliday.MenuManager = this.ribbonControl1;
            this.gcHoliday.Name = "gcHoliday";
            this.gcHoliday.Size = new System.Drawing.Size(1009, 333);
            this.gcHoliday.TabIndex = 1;
            this.gcHoliday.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvHoliday});
            // 
            // gvHoliday
            // 
            this.gvHoliday.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.HOL_DAYS});
            this.gvHoliday.GridControl = this.gcHoliday;
            this.gvHoliday.Name = "gvHoliday";
            // 
            // HOL_DAYS
            // 
            this.HOL_DAYS.Caption = "节假日日期";
            this.HOL_DAYS.FieldName = "HOL_DAYS";
            this.HOL_DAYS.Name = "HOL_DAYS";
            this.HOL_DAYS.OptionsColumn.AllowEdit = false;
            this.HOL_DAYS.Visible = true;
            this.HOL_DAYS.VisibleIndex = 0;
            // 
            // SyncHolidayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 480);
            this.Controls.Add(this.gcHoliday);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "SyncHolidayForm";
            this.Ribbon = this.ribbonControl1;
            this.Text = "节假日数据同步";
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcHoliday)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvHoliday)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem btnSearch;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraGrid.GridControl gcHoliday;
        private DevExpress.XtraGrid.Views.Grid.GridView gvHoliday;
        private DevExpress.XtraGrid.Columns.GridColumn HOL_DAYS;
        private DevExpress.XtraBars.BarButtonItem btnSyncHoliday;
    }
}