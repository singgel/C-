namespace FuelDataSysClient
{
    partial class SearchLocalUpdateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchLocalUpdateForm));
            this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barBtnLocalSearch = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSelectAll = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnLocalDel = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnClearAll = new DevExpress.XtraBars.BarButtonItem();
            this.barUpdate = new DevExpress.XtraBars.BarButtonItem();
            this.barSynchronous = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.lblSum = new DevExpress.XtraEditors.LabelControl();
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
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.dgvCljbxx = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.cbFlag = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.VIN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.HGSPBM = new DevExpress.XtraGrid.Columns.GridColumn();
            this.QCSCQY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.JKQCZJXS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLXH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLZL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RLLX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLZZRQ = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvCljbxx)).BeginInit();
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
            this.barBtnLocalSearch,
            this.barBtnSelectAll,
            this.barBtnLocalDel,
            this.barBtnClearAll,
            this.barUpdate,
            this.barSynchronous});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 10;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1015, 147);
            // 
            // barBtnLocalSearch
            // 
            this.barBtnLocalSearch.Caption = "查询";
            this.barBtnLocalSearch.Id = 1;
            this.barBtnLocalSearch.LargeGlyph = global::FuelDataSysClient.Properties.Resources.ZoomLarge;
            this.barBtnLocalSearch.Name = "barBtnLocalSearch";
            // 
            // barBtnSelectAll
            // 
            this.barBtnSelectAll.Caption = "全选";
            this.barBtnSelectAll.Id = 5;
            this.barBtnSelectAll.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barBtnSelectAll.LargeGlyph")));
            this.barBtnSelectAll.Name = "barBtnSelectAll";
            this.barBtnSelectAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSelectAll_ItemClick);
            // 
            // barBtnLocalDel
            // 
            this.barBtnLocalDel.Caption = "删除";
            this.barBtnLocalDel.Id = 6;
            this.barBtnLocalDel.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barBtnLocalDel.LargeGlyph")));
            this.barBtnLocalDel.Name = "barBtnLocalDel";
            this.barBtnLocalDel.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnLocalDel_ItemClick);
            // 
            // barBtnClearAll
            // 
            this.barBtnClearAll.Caption = "取消全选";
            this.barBtnClearAll.Id = 7;
            this.barBtnClearAll.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barBtnClearAll.LargeGlyph")));
            this.barBtnClearAll.Name = "barBtnClearAll";
            this.barBtnClearAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnClearAll_ItemClick);
            // 
            // barUpdate
            // 
            this.barUpdate.Caption = "上报修改";
            this.barUpdate.Id = 8;
            this.barUpdate.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barUpdate.LargeGlyph")));
            this.barUpdate.Name = "barUpdate";
            this.barUpdate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barUpdate_ItemClick);
            // 
            // barSynchronous
            // 
            this.barSynchronous.Caption = "同步状态";
            this.barSynchronous.Id = 9;
            this.barSynchronous.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barSynchronous.LargeGlyph")));
            this.barSynchronous.Name = "barSynchronous";
            this.barSynchronous.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSynchronous_ItemClick);
            // 
            // ribbonPage1
            // 
            this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
            this.ribbonPageGroup2,
            this.ribbonPageGroup1});
            this.ribbonPage1.Name = "ribbonPage1";
            this.ribbonPage1.Text = "操作";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barUpdate);
            this.ribbonPageGroup2.ItemLinks.Add(this.barSynchronous);
            this.ribbonPageGroup2.ItemLinks.Add(this.barBtnLocalDel);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.ShowCaptionButton = false;
            this.ribbonPageGroup2.Text = "数据";
            // 
            // ribbonPageGroup1
            // 
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnSelectAll);
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnClearAll);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.ShowCaptionButton = false;
            this.ribbonPageGroup1.Text = "通用";
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 147);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.dgvCljbxx);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1015, 366);
            this.splitContainerControl1.SplitterPosition = 95;
            this.splitContainerControl1.TabIndex = 10;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraScrollableControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1015, 95);
            this.panelControl1.TabIndex = 2;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.btnClear);
            this.xtraScrollableControl1.Controls.Add(this.lblSum);
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
            this.xtraScrollableControl1.Controls.Add(this.labelControl1);
            this.xtraScrollableControl1.Controls.Add(this.btnSearch);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(1011, 91);
            this.xtraScrollableControl1.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(849, 44);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 25);
            this.btnClear.TabIndex = 87;
            this.btnClear.Text = "清空查询条件";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblSum
            // 
            this.lblSum.Location = new System.Drawing.Point(21, 71);
            this.lblSum.Name = "lblSum";
            this.lblSum.Size = new System.Drawing.Size(31, 14);
            this.lblSum.TabIndex = 86;
            this.lblSum.Text = "共0条";
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(678, 41);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtEndTime.Size = new System.Drawing.Size(120, 20);
            this.dtEndTime.TabIndex = 82;
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(534, 41);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 80;
            // 
            // cbTimeType
            // 
            this.cbTimeType.EditValue = "上报日期";
            this.cbTimeType.Location = new System.Drawing.Point(408, 41);
            this.cbTimeType.Name = "cbTimeType";
            this.cbTimeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbTimeType.Properties.Items.AddRange(new object[] {
            "上报日期",
            "车辆制造日期/进口核销日期"});
            this.cbTimeType.Size = new System.Drawing.Size(120, 20);
            this.cbTimeType.TabIndex = 84;
            // 
            // tbClzl
            // 
            this.tbClzl.Location = new System.Drawing.Point(408, 11);
            this.tbClzl.Name = "tbClzl";
            this.tbClzl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.tbClzl.Properties.Items.AddRange(new object[] {
            "乘用车（M1）",
            "轻型客车（M2）",
            "轻型货车（N1）"});
            this.tbClzl.Size = new System.Drawing.Size(120, 20);
            this.tbClzl.TabIndex = 77;
            // 
            // cbRllx
            // 
            this.cbRllx.Location = new System.Drawing.Point(125, 41);
            this.cbRllx.MenuManager = this.ribbonControl1;
            this.cbRllx.Name = "cbRllx";
            this.cbRllx.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbRllx.Size = new System.Drawing.Size(120, 20);
            this.cbRllx.TabIndex = 79;
            // 
            // labelControl8
            // 
            this.labelControl8.Location = new System.Drawing.Point(661, 44);
            this.labelControl8.Name = "labelControl8";
            this.labelControl8.Size = new System.Drawing.Size(12, 14);
            this.labelControl8.TabIndex = 81;
            this.labelControl8.Text = "至";
            // 
            // labelControl9
            // 
            this.labelControl9.Location = new System.Drawing.Point(294, 44);
            this.labelControl9.Name = "labelControl9";
            this.labelControl9.Size = new System.Drawing.Size(48, 14);
            this.labelControl9.TabIndex = 83;
            this.labelControl9.Text = "时间类型";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(21, 44);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 14);
            this.labelControl4.TabIndex = 78;
            this.labelControl4.Text = "燃料种类";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(294, 14);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(48, 14);
            this.labelControl3.TabIndex = 76;
            this.labelControl3.Text = "车辆类型";
            // 
            // tbClxh
            // 
            this.tbClxh.Location = new System.Drawing.Point(678, 11);
            this.tbClxh.MenuManager = this.ribbonControl1;
            this.tbClxh.Name = "tbClxh";
            this.tbClxh.Size = new System.Drawing.Size(120, 20);
            this.tbClxh.TabIndex = 75;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(579, 14);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(48, 14);
            this.labelControl2.TabIndex = 74;
            this.labelControl2.Text = "产品型号";
            // 
            // tbVin
            // 
            this.tbVin.Location = new System.Drawing.Point(125, 11);
            this.tbVin.MenuManager = this.ribbonControl1;
            this.tbVin.Name = "tbVin";
            this.tbVin.Size = new System.Drawing.Size(120, 20);
            this.tbVin.TabIndex = 73;
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(21, 14);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(66, 14);
            this.labelControl1.TabIndex = 72;
            this.labelControl1.Text = "备案号(VIN)";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(849, 9);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(100, 25);
            this.btnSearch.TabIndex = 85;
            this.btnSearch.Text = "查   询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgvCljbxx
            // 
            this.dgvCljbxx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCljbxx.Location = new System.Drawing.Point(0, 0);
            this.dgvCljbxx.MainView = this.gridView1;
            this.dgvCljbxx.Name = "dgvCljbxx";
            this.dgvCljbxx.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.dgvCljbxx.Size = new System.Drawing.Size(1015, 266);
            this.dgvCljbxx.TabIndex = 8;
            this.dgvCljbxx.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.dgvCljbxx.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgvCljbxx_MouseDoubleClick);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.cbFlag,
            this.VIN,
            this.HGSPBM,
            this.QCSCQY,
            this.JKQCZJXS,
            this.CLXH,
            this.CLZL,
            this.RLLX,
            this.CLZZRQ});
            this.gridView1.GridControl = this.dgvCljbxx;
            this.gridView1.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, "", null, "")});
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsMenu.EnableColumnMenu = false;
            this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView1.OptionsView.ShowFooter = true;
            this.gridView1.OptionsView.ShowGroupedColumns = true;
            // 
            // cbFlag
            // 
            this.cbFlag.Caption = "选择";
            this.cbFlag.ColumnEdit = this.repositoryItemCheckEdit1;
            this.cbFlag.FieldName = "check";
            this.cbFlag.MaxWidth = 50;
            this.cbFlag.Name = "cbFlag";
            this.cbFlag.Tag = false;
            this.cbFlag.Visible = true;
            this.cbFlag.VisibleIndex = 0;
            this.cbFlag.Width = 20;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // VIN
            // 
            this.VIN.Caption = "备案号(VIN)";
            this.VIN.FieldName = "VIN";
            this.VIN.Name = "VIN";
            this.VIN.OptionsColumn.AllowEdit = false;
            this.VIN.Visible = true;
            this.VIN.VisibleIndex = 1;
            // 
            // HGSPBM
            // 
            this.HGSPBM.Caption = "海关商品编码";
            this.HGSPBM.FieldName = "HGSPBM";
            this.HGSPBM.Name = "HGSPBM";
            this.HGSPBM.OptionsColumn.AllowEdit = false;
            // 
            // QCSCQY
            // 
            this.QCSCQY.Caption = "乘用车生产企业";
            this.QCSCQY.FieldName = "QCSCQY";
            this.QCSCQY.Name = "QCSCQY";
            this.QCSCQY.OptionsColumn.AllowEdit = false;
            this.QCSCQY.Visible = true;
            this.QCSCQY.VisibleIndex = 2;
            // 
            // JKQCZJXS
            // 
            this.JKQCZJXS.Caption = "进口乘用车供应企业";
            this.JKQCZJXS.FieldName = "JKQCZJXS";
            this.JKQCZJXS.Name = "JKQCZJXS";
            this.JKQCZJXS.OptionsColumn.AllowEdit = false;
            this.JKQCZJXS.Visible = true;
            this.JKQCZJXS.VisibleIndex = 3;
            // 
            // CLXH
            // 
            this.CLXH.Caption = "产品型号";
            this.CLXH.FieldName = "CLXH";
            this.CLXH.Name = "CLXH";
            this.CLXH.OptionsColumn.AllowEdit = false;
            this.CLXH.Visible = true;
            this.CLXH.VisibleIndex = 4;
            // 
            // CLZL
            // 
            this.CLZL.Caption = "车辆类型";
            this.CLZL.FieldName = "CLZL";
            this.CLZL.Name = "CLZL";
            this.CLZL.OptionsColumn.AllowEdit = false;
            this.CLZL.Visible = true;
            this.CLZL.VisibleIndex = 5;
            // 
            // RLLX
            // 
            this.RLLX.Caption = "燃料种类";
            this.RLLX.FieldName = "RLLX";
            this.RLLX.Name = "RLLX";
            this.RLLX.OptionsColumn.AllowEdit = false;
            this.RLLX.Visible = true;
            this.RLLX.VisibleIndex = 6;
            // 
            // CLZZRQ
            // 
            this.CLZZRQ.Caption = "车辆制造日期/进口核销日期";
            this.CLZZRQ.FieldName = "CLZZRQ";
            this.CLZZRQ.Name = "CLZZRQ";
            this.CLZZRQ.OptionsColumn.AllowEdit = false;
            this.CLZZRQ.Visible = true;
            this.CLZZRQ.VisibleIndex = 7;
            // 
            // SearchLocalUpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 513);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "SearchLocalUpdateForm";
            this.Ribbon = this.ribbonControl1;
            this.Text = "已修改未上报";
            this.Load += new System.EventHandler(this.SearchLocalUpdateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvCljbxx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.BarButtonItem barBtnLocalSearch;
        private DevExpress.XtraBars.BarButtonItem barBtnSelectAll;
        private DevExpress.XtraBars.BarButtonItem barBtnLocalDel;
        private DevExpress.XtraBars.BarButtonItem barBtnClearAll;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl dgvCljbxx;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn cbFlag;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Columns.GridColumn VIN;
        private DevExpress.XtraGrid.Columns.GridColumn QCSCQY;
        private DevExpress.XtraGrid.Columns.GridColumn JKQCZJXS;
        private DevExpress.XtraGrid.Columns.GridColumn CLXH;
        private DevExpress.XtraGrid.Columns.GridColumn CLZL;
        private DevExpress.XtraGrid.Columns.GridColumn RLLX;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraBars.BarButtonItem barUpdate;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraGrid.Columns.GridColumn CLZZRQ;
        private DevExpress.XtraGrid.Columns.GridColumn HGSPBM;
        private DevExpress.XtraBars.BarButtonItem barSynchronous;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private DevExpress.XtraEditors.LabelControl lblSum;
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
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
    }
}