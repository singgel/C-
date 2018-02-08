namespace FuelDataSysClient.Form_Modify
{
	partial class ApplyDelViewForm
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
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.dtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.dtStartTime = new DevExpress.XtraEditors.DateEdit();
            this.cbTimeType = new DevExpress.XtraEditors.ComboBoxEdit();
            this.tbClzl = new DevExpress.XtraEditors.ComboBoxEdit();
            this.cbRllx = new DevExpress.XtraEditors.ComboBoxEdit();
            this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.tbClxh = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.tbVin = new DevExpress.XtraEditors.TextEdit();
            this.lcPageNum = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnPrePage = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.gcApplyDelQuery = new DevExpress.XtraGrid.GridControl();
            this.gvApplyQuery = new DevExpress.XtraGrid.Views.Grid.GridView();
            this._STATUS = new DevExpress.XtraGrid.Columns.GridColumn();
            this._App_Vin = new DevExpress.XtraGrid.Columns.GridColumn();
            this._QCSCQY = new DevExpress.XtraGrid.Columns.GridColumn();
            this._JKQCZJXS = new DevExpress.XtraGrid.Columns.GridColumn();
            this._CLXH = new DevExpress.XtraGrid.Columns.GridColumn();
            this._CLZL = new DevExpress.XtraGrid.Columns.GridColumn();
            this._RLLX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLZZRQ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CreateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.EntityList = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTimeType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbClzl.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbRllx.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbClxh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVin.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcApplyDelQuery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvApplyQuery)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnRefresh});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 2;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1015, 147);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Caption = "刷新";
            this.btnRefresh.Id = 1;
            this.btnRefresh.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_Refresh;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnRefresh_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.btnRefresh);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "通用";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 489);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1015, 31);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraScrollableControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 147);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1015, 92);
            this.panelControl1.TabIndex = 2;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.dtEndTime);
            this.xtraScrollableControl1.Controls.Add(this.dtStartTime);
            this.xtraScrollableControl1.Controls.Add(this.cbTimeType);
            this.xtraScrollableControl1.Controls.Add(this.tbClzl);
            this.xtraScrollableControl1.Controls.Add(this.cbRllx);
            this.xtraScrollableControl1.Controls.Add(this.labelControl8);
            this.xtraScrollableControl1.Controls.Add(this.labelControl9);
            this.xtraScrollableControl1.Controls.Add(this.labelControl4);
            this.xtraScrollableControl1.Controls.Add(this.labelControl3);
            this.xtraScrollableControl1.Controls.Add(this.tbClxh);
            this.xtraScrollableControl1.Controls.Add(this.labelControl2);
            this.xtraScrollableControl1.Controls.Add(this.tbVin);
            this.xtraScrollableControl1.Controls.Add(this.lcPageNum);
            this.xtraScrollableControl1.Controls.Add(this.labelControl1);
            this.xtraScrollableControl1.Controls.Add(this.btnPrePage);
            this.xtraScrollableControl1.Controls.Add(this.btnNextPage);
            this.xtraScrollableControl1.Controls.Add(this.btnSearch);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(1011, 88);
            this.xtraScrollableControl1.TabIndex = 0;
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(678, 35);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtEndTime.Size = new System.Drawing.Size(120, 20);
            this.dtEndTime.TabIndex = 112;
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(534, 35);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 110;
            // 
            // cbTimeType
            // 
            this.cbTimeType.EditValue = "上报日期";
            this.cbTimeType.Location = new System.Drawing.Point(408, 35);
            this.cbTimeType.Name = "cbTimeType";
            this.cbTimeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbTimeType.Properties.Items.AddRange(new object[] {
            "上报日期",
            "制造日期/进口日期"});
            this.cbTimeType.Size = new System.Drawing.Size(120, 20);
            this.cbTimeType.TabIndex = 114;
            // 
            // tbClzl
            // 
            this.tbClzl.Location = new System.Drawing.Point(408, 4);
            this.tbClzl.Name = "tbClzl";
            this.tbClzl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tbClzl.Properties.Items.AddRange(new object[] {
            "乘用车（M1）",
            "轻型客车（M2）",
            "轻型货车（N1）"});
            this.tbClzl.Size = new System.Drawing.Size(120, 20);
            this.tbClzl.TabIndex = 107;
            // 
            // cbRllx
            // 
            this.cbRllx.Location = new System.Drawing.Point(678, 4);
            this.cbRllx.Name = "cbRllx";
            this.cbRllx.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbRllx.Size = new System.Drawing.Size(120, 20);
            this.cbRllx.TabIndex = 109;
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(660, 38);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(12, 14);
            this.labelControl8.TabIndex = 111;
            this.labelControl8.Text = "至";
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(294, 38);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(48, 14);
            this.labelControl9.TabIndex = 113;
            this.labelControl9.Text = "时间类型";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(579, 7);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 14);
            this.labelControl4.TabIndex = 108;
            this.labelControl4.Text = "燃料类型";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(294, 7);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(48, 14);
            this.labelControl3.TabIndex = 106;
            this.labelControl3.Text = "车辆种类";
            // 
            // tbClxh
            // 
            this.tbClxh.Location = new System.Drawing.Point(124, 35);
            this.tbClxh.Name = "tbClxh";
            this.tbClxh.Size = new System.Drawing.Size(120, 20);
            this.tbClxh.TabIndex = 105;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(21, 38);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 104;
            this.labelControl2.Text = "车辆型号";
            // 
            // tbVin
            // 
            this.tbVin.Location = new System.Drawing.Point(124, 4);
            this.tbVin.Name = "tbVin";
            this.tbVin.Size = new System.Drawing.Size(120, 20);
            this.tbVin.TabIndex = 103;
            // 
            // lcPageNum
            // 
            this.lcPageNum.Location = new System.Drawing.Point(124, 68);
            this.lcPageNum.Name = "lcPageNum";
            this.lcPageNum.Size = new System.Drawing.Size(31, 14);
            this.lcPageNum.TabIndex = 101;
            this.lcPageNum.Text = "第0页";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(21, 7);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 14);
            this.labelControl1.TabIndex = 102;
            this.labelControl1.Text = "备案号(VIN)";
            // 
            // btnPrePage
            // 
            this.btnPrePage.Location = new System.Drawing.Point(21, 66);
            this.btnPrePage.Name = "btnPrePage";
            this.btnPrePage.Size = new System.Drawing.Size(50, 20);
            this.btnPrePage.TabIndex = 116;
            this.btnPrePage.Text = "上一页";
            this.btnPrePage.Click += new System.EventHandler(this.btnPrePage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Location = new System.Drawing.Point(194, 66);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(50, 20);
            this.btnNextPage.TabIndex = 117;
            this.btnNextPage.Text = "下一页";
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(849, 7);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 25);
            this.btnSearch.TabIndex = 115;
            this.btnSearch.Text = "查   询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // gcApplyDelQuery
            // 
            this.gcApplyDelQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcApplyDelQuery.Location = new System.Drawing.Point(0, 239);
            this.gcApplyDelQuery.MainView = this.gvApplyQuery;
            this.gcApplyDelQuery.MenuManager = this.ribbon;
            this.gcApplyDelQuery.Name = "gcApplyDelQuery";
            this.gcApplyDelQuery.Size = new System.Drawing.Size(1015, 250);
            this.gcApplyDelQuery.TabIndex = 3;
            this.gcApplyDelQuery.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvApplyQuery});
            this.gcApplyDelQuery.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gcApplyDelQuery_MouseDoubleClick);
            // 
            // gvApplyQuery
            // 
            this.gvApplyQuery.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this._STATUS,
            this._App_Vin,
            this._QCSCQY,
            this._JKQCZJXS,
            this._CLXH,
            this._CLZL,
            this._RLLX,
            this.CLZZRQ,
            this.CreateTime,
            this.EntityList});
            this.gvApplyQuery.GridControl = this.gcApplyDelQuery;
            this.gvApplyQuery.Name = "gvApplyQuery";
            this.gvApplyQuery.OptionsBehavior.ReadOnly = true;
            this.gvApplyQuery.OptionsMenu.EnableColumnMenu = false;
            this.gvApplyQuery.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvApplyQuery.OptionsView.ShowFooter = true;
            this.gvApplyQuery.OptionsView.ShowGroupedColumns = true;
            // 
            // _STATUS
            // 
            this._STATUS.Caption = "状态";
            this._STATUS.FieldName = "Status";
            this._STATUS.Name = "_STATUS";
            this._STATUS.Visible = true;
            this._STATUS.VisibleIndex = 0;
            // 
            // _App_Vin
            // 
            this._App_Vin.Caption = "备案号(VIN)";
            this._App_Vin.FieldName = "App_Vin";
            this._App_Vin.Name = "_App_Vin";
            this._App_Vin.OptionsColumn.AllowEdit = false;
            this._App_Vin.Visible = true;
            this._App_Vin.VisibleIndex = 1;
            // 
            // _QCSCQY
            // 
            this._QCSCQY.Caption = "乘用车生产企业";
            this._QCSCQY.FieldName = "Qcscqy";
            this._QCSCQY.Name = "_QCSCQY";
            this._QCSCQY.OptionsColumn.AllowEdit = false;
            this._QCSCQY.Visible = true;
            this._QCSCQY.VisibleIndex = 2;
            // 
            // _JKQCZJXS
            // 
            this._JKQCZJXS.Caption = "进口乘用车供应企业";
            this._JKQCZJXS.FieldName = "Jkqczjxs";
            this._JKQCZJXS.Name = "_JKQCZJXS";
            this._JKQCZJXS.OptionsColumn.AllowEdit = false;
            this._JKQCZJXS.Visible = true;
            this._JKQCZJXS.VisibleIndex = 3;
            // 
            // _CLXH
            // 
            this._CLXH.Caption = "车辆型号";
            this._CLXH.FieldName = "Clxh";
            this._CLXH.Name = "_CLXH";
            this._CLXH.OptionsColumn.AllowEdit = false;
            this._CLXH.Visible = true;
            this._CLXH.VisibleIndex = 4;
            // 
            // _CLZL
            // 
            this._CLZL.Caption = "车辆种类";
            this._CLZL.FieldName = "Clzl";
            this._CLZL.Name = "_CLZL";
            this._CLZL.OptionsColumn.AllowEdit = false;
            this._CLZL.Visible = true;
            this._CLZL.VisibleIndex = 5;
            // 
            // _RLLX
            // 
            this._RLLX.Caption = "燃料类型";
            this._RLLX.FieldName = "Rllx";
            this._RLLX.Name = "_RLLX";
            this._RLLX.OptionsColumn.AllowEdit = false;
            this._RLLX.Visible = true;
            this._RLLX.VisibleIndex = 6;
            // 
            // CLZZRQ
            // 
            this.CLZZRQ.Caption = "制造日期/进口日期";
            this.CLZZRQ.FieldName = "Clzzrq";
            this.CLZZRQ.Name = "CLZZRQ";
            this.CLZZRQ.OptionsColumn.AllowEdit = false;
            this.CLZZRQ.Visible = true;
            this.CLZZRQ.VisibleIndex = 7;
            // 
            // CreateTime
            // 
            this.CreateTime.Caption = "申请日期";
            this.CreateTime.FieldName = "CreateTime";
            this.CreateTime.Name = "CreateTime";
            this.CreateTime.OptionsColumn.AllowEdit = false;
            this.CreateTime.Visible = true;
            this.CreateTime.VisibleIndex = 8;
            // 
            // EntityList
            // 
            this.EntityList.Caption = "gcList";
            this.EntityList.FieldName = "EntityList";
            this.EntityList.Name = "EntityList";
            this.EntityList.OptionsColumn.AllowEdit = false;
            // 
            // ApplyDelViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 520);
            this.Controls.Add(this.gcApplyDelQuery);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "ApplyDelViewForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "撤销申请查询";
            this.Load += new System.EventHandler(this.ApplyDelViewForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.xtraScrollableControl1.ResumeLayout(false);
            this.xtraScrollableControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbTimeType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbClzl.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cbRllx.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbClxh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbVin.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcApplyDelQuery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvApplyQuery)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
		private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem btnRefresh;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraGrid.GridControl gcApplyDelQuery;
        private DevExpress.XtraGrid.Views.Grid.GridView gvApplyQuery;
        private DevExpress.XtraGrid.Columns.GridColumn _App_Vin;
        private DevExpress.XtraGrid.Columns.GridColumn _QCSCQY;
        private DevExpress.XtraGrid.Columns.GridColumn _JKQCZJXS;
        private DevExpress.XtraGrid.Columns.GridColumn _CLXH;
        private DevExpress.XtraGrid.Columns.GridColumn _CLZL;
        private DevExpress.XtraGrid.Columns.GridColumn _RLLX;
        private DevExpress.XtraGrid.Columns.GridColumn EntityList;
        private DevExpress.XtraGrid.Columns.GridColumn _STATUS;
        private DevExpress.XtraGrid.Columns.GridColumn CLZZRQ;
        private DevExpress.XtraGrid.Columns.GridColumn CreateTime;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraEditors.DateEdit dtEndTime;
        private DevExpress.XtraEditors.DateEdit dtStartTime;
        private DevExpress.XtraEditors.ComboBoxEdit cbTimeType;
        private DevExpress.XtraEditors.ComboBoxEdit tbClzl;
        private DevExpress.XtraEditors.ComboBoxEdit cbRllx;
        private DevExpress.XtraEditors.LabelControl labelControl8;
        private DevExpress.XtraEditors.LabelControl labelControl9;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit tbClxh;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit tbVin;
        private DevExpress.XtraEditors.LabelControl lcPageNum;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnPrePage;
        private DevExpress.XtraEditors.SimpleButton btnNextPage;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
	}
}