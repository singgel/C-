namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    partial class DataDictionaryForm
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
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraGrid.StyleFormatCondition();
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barBtnImportNew = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSingle = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnDelete = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnSelect = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnCancle = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnRefresh = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnExportFloder = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnUpdate = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.spanNumber = new DevExpress.XtraEditors.SpinEdit();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.btnFirPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrePage = new DevExpress.XtraEditors.SimpleButton();
            this.btnLastPage = new DevExpress.XtraEditors.SimpleButton();
            this.btnNextPage = new DevExpress.XtraEditors.SimpleButton();
            this.labPage = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.ceQueryAll = new DevExpress.XtraEditors.CheckEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.lblSum = new DevExpress.XtraEditors.LabelControl();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.Type = new DevExpress.XtraEditors.ComboBoxEdit();
            this.Define = new DevExpress.XtraEditors.TextEdit();
            this.labType = new DevExpress.XtraEditors.LabelControl();
            this.labDefine = new DevExpress.XtraEditors.LabelControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.gcDataInfo = new DevExpress.XtraGrid.GridControl();
            this.gvDataInfo = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn0 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spanNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceQueryAll.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Type.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Define.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcDataInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDataInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.barBtnImportNew,
            this.barBtnSingle,
            this.barBtnDelete,
            this.barBtnSelect,
            this.barBtnCancle,
            this.barBtnRefresh,
            this.barBtnExportFloder,
            this.barBtnUpdate});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 14;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1118, 147);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barBtnImportNew
            // 
            this.barBtnImportNew.Caption = "导入";
            this.barBtnImportNew.Id = 1;
            this.barBtnImportNew.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.barBtn_fuelAdd;
            this.barBtnImportNew.Name = "barBtnImportNew";
            this.barBtnImportNew.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnImportNew_ItemClick);
            // 
            // barBtnSingle
            // 
            this.barBtnSingle.Caption = "单条增加";
            this.barBtnSingle.Id = 3;
            this.barBtnSingle.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.bar_page_first;
            this.barBtnSingle.Name = "barBtnSingle";
            this.barBtnSingle.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSingle_ItemClick);
            // 
            // barBtnDelete
            // 
            this.barBtnDelete.Caption = "删除记录";
            this.barBtnDelete.Id = 4;
            this.barBtnDelete.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.barBtn_Del;
            this.barBtnDelete.Name = "barBtnDelete";
            this.barBtnDelete.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnDelete_ItemClick);
            // 
            // barBtnSelect
            // 
            this.barBtnSelect.Caption = "全部选中";
            this.barBtnSelect.Id = 5;
            this.barBtnSelect.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.barBtn_SelectAll;
            this.barBtnSelect.Name = "barBtnSelect";
            this.barBtnSelect.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSelect_ItemClick);
            // 
            // barBtnCancle
            // 
            this.barBtnCancle.Caption = "全部取消";
            this.barBtnCancle.Id = 6;
            this.barBtnCancle.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.barBtn_ClearAll;
            this.barBtnCancle.Name = "barBtnCancle";
            this.barBtnCancle.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnCancle_ItemClick);
            // 
            // barBtnRefresh
            // 
            this.barBtnRefresh.Caption = "刷新";
            this.barBtnRefresh.Id = 7;
            this.barBtnRefresh.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.barBtn_Refresh;
            this.barBtnRefresh.Name = "barBtnRefresh";
            this.barBtnRefresh.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnRefresh_ItemClick);
            // 
            // barBtnExportFloder
            // 
            this.barBtnExportFloder.Caption = "数据导出";
            this.barBtnExportFloder.Id = 10;
            this.barBtnExportFloder.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.bar_upload;
            this.barBtnExportFloder.Name = "barBtnExportFloder";
            this.barBtnExportFloder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnExportFloder_ItemClick);
            // 
            // barBtnUpdate
            // 
            this.barBtnUpdate.Caption = "单条修改";
            this.barBtnUpdate.Id = 11;
            this.barBtnUpdate.LargeGlyph = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.barBtn_Update;
            this.barBtnUpdate.Name = "barBtnUpdate";
            this.barBtnUpdate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnUpdate_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnSingle);
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnImportNew);
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnUpdate);
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnDelete);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "数据";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.barBtnSelect);
            this.ribbonPageGroup2.ItemLinks.Add(this.barBtnCancle);
            this.ribbonPageGroup2.ItemLinks.Add(this.barBtnRefresh);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "通用";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 631);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1118, 31);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 147);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.xtraScrollableControl1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.gcDataInfo);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(1118, 484);
            this.splitContainerControl1.TabIndex = 2;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.spanNumber);
            this.xtraScrollableControl1.Controls.Add(this.txtPage);
            this.xtraScrollableControl1.Controls.Add(this.btnFirPage);
            this.xtraScrollableControl1.Controls.Add(this.btnPrePage);
            this.xtraScrollableControl1.Controls.Add(this.btnLastPage);
            this.xtraScrollableControl1.Controls.Add(this.btnNextPage);
            this.xtraScrollableControl1.Controls.Add(this.labPage);
            this.xtraScrollableControl1.Controls.Add(this.labelControl6);
            this.xtraScrollableControl1.Controls.Add(this.ceQueryAll);
            this.xtraScrollableControl1.Controls.Add(this.labelControl5);
            this.xtraScrollableControl1.Controls.Add(this.lblSum);
            this.xtraScrollableControl1.Controls.Add(this.btnClear);
            this.xtraScrollableControl1.Controls.Add(this.Type);
            this.xtraScrollableControl1.Controls.Add(this.Define);
            this.xtraScrollableControl1.Controls.Add(this.labType);
            this.xtraScrollableControl1.Controls.Add(this.labDefine);
            this.xtraScrollableControl1.Controls.Add(this.btnSearch);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(1118, 100);
            this.xtraScrollableControl1.TabIndex = 60;
            // 
            // spanNumber
            // 
            this.spanNumber.EditValue = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.spanNumber.Location = new System.Drawing.Point(155, 71);
            this.spanNumber.Name = "spanNumber";
            this.spanNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.spanNumber.Properties.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.spanNumber.Properties.IsFloatValue = false;
            this.spanNumber.Properties.Mask.EditMask = "n0";
            this.spanNumber.Properties.MaxValue = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.spanNumber.Properties.MinValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spanNumber.Size = new System.Drawing.Size(60, 20);
            this.spanNumber.TabIndex = 19;
            // 
            // txtPage
            // 
            this.txtPage.BackColor = System.Drawing.SystemColors.Menu;
            this.txtPage.Enabled = false;
            this.txtPage.Location = new System.Drawing.Point(676, 70);
            this.txtPage.Name = "txtPage";
            this.txtPage.Size = new System.Drawing.Size(60, 22);
            this.txtPage.TabIndex = 25;
            this.txtPage.Text = "0/0";
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnFirPage
            // 
            this.btnFirPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnFirPage.Image = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.arrow_left_top;
            this.btnFirPage.Location = new System.Drawing.Point(614, 72);
            this.btnFirPage.Name = "btnFirPage";
            this.btnFirPage.Size = new System.Drawing.Size(25, 19);
            this.btnFirPage.TabIndex = 23;
            this.btnFirPage.Click += new System.EventHandler(this.btnFirPage_Click);
            // 
            // btnPrePage
            // 
            this.btnPrePage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.btnPrePage.Image = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.arrow_left;
            this.btnPrePage.Location = new System.Drawing.Point(645, 72);
            this.btnPrePage.Name = "btnPrePage";
            this.btnPrePage.Size = new System.Drawing.Size(25, 19);
            this.btnPrePage.TabIndex = 24;
            this.btnPrePage.Click += new System.EventHandler(this.btnPrePage_Click);
            // 
            // btnLastPage
            // 
            this.btnLastPage.Image = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.arrow_right_top;
            this.btnLastPage.Location = new System.Drawing.Point(773, 72);
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(25, 19);
            this.btnLastPage.TabIndex = 27;
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Image = global::Catarc.Adc.NewEnergyApproveSys.Properties.Resources.arrow_right;
            this.btnNextPage.Location = new System.Drawing.Point(742, 72);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(25, 19);
            this.btnNextPage.TabIndex = 26;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // labPage
            // 
            this.labPage.Location = new System.Drawing.Point(408, 74);
            this.labPage.Name = "labPage";
            this.labPage.Size = new System.Drawing.Size(86, 14);
            this.labPage.TabIndex = 22;
            this.labPage.Text = "当前显示0至0条";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(222, 74);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(24, 14);
            this.labelControl6.TabIndex = 20;
            this.labelControl6.Text = "条数";
            // 
            // ceQueryAll
            // 
            this.ceQueryAll.Location = new System.Drawing.Point(294, 72);
            this.ceQueryAll.Name = "ceQueryAll";
            this.ceQueryAll.Properties.Caption = "显示全部";
            this.ceQueryAll.Size = new System.Drawing.Size(75, 19);
            this.ceQueryAll.TabIndex = 21;
            this.ceQueryAll.CheckedChanged += new System.EventHandler(this.ceQueryAll_CheckedChanged);
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(125, 74);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(24, 14);
            this.labelControl5.TabIndex = 18;
            this.labelControl5.Text = "显示";
            // 
            // lblSum
            // 
            this.lblSum.Location = new System.Drawing.Point(21, 74);
            this.lblSum.Name = "lblSum";
            this.lblSum.Size = new System.Drawing.Size(31, 14);
            this.lblSum.TabIndex = 17;
            this.lblSum.Text = "共0条";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(919, 52);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(130, 20);
            this.btnClear.TabIndex = 29;
            this.btnClear.Text = "清空查询条件";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // Type
            // 
            this.Type.Location = new System.Drawing.Point(326, 11);
            this.Type.Name = "Type";
            this.Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.Type.Properties.Items.AddRange(new object[] {
            "",
            "地区",
            "车企名称",
            "车辆用途",
            "车辆性质",
            "车辆种类",
            "ftp路径",
            "驳回原因"});
            this.Type.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.Type.Size = new System.Drawing.Size(120, 20);
            this.Type.TabIndex = 8;
            // 
            // Define
            // 
            this.Define.Location = new System.Drawing.Point(111, 11);
            this.Define.Name = "Define";
            this.Define.Size = new System.Drawing.Size(120, 20);
            this.Define.TabIndex = 6;
            // 
            // labType
            // 
            this.labType.Location = new System.Drawing.Point(254, 14);
            this.labType.Name = "labType";
            this.labType.Size = new System.Drawing.Size(24, 14);
            this.labType.TabIndex = 3;
            this.labType.Text = "类型";
            // 
            // labDefine
            // 
            this.labDefine.Location = new System.Drawing.Point(21, 14);
            this.labDefine.Name = "labDefine";
            this.labDefine.Size = new System.Drawing.Size(24, 14);
            this.labDefine.TabIndex = 1;
            this.labDefine.Text = "名称";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(919, 23);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(130, 20);
            this.btnSearch.TabIndex = 28;
            this.btnSearch.Text = "查    询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // gcDataInfo
            // 
            this.gcDataInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcDataInfo.Location = new System.Drawing.Point(0, 0);
            this.gcDataInfo.MainView = this.gvDataInfo;
            this.gcDataInfo.MenuManager = this.ribbon;
            this.gcDataInfo.Name = "gcDataInfo";
            this.gcDataInfo.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.gcDataInfo.Size = new System.Drawing.Size(1118, 379);
            this.gcDataInfo.TabIndex = 0;
            this.gcDataInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDataInfo});
            // 
            // gvDataInfo
            // 
            this.gvDataInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn0,
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            styleFormatCondition1.Appearance.BackColor = System.Drawing.Color.Pink;
            styleFormatCondition1.Appearance.Options.UseBackColor = true;
            styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Expression;
            this.gvDataInfo.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition1});
            this.gvDataInfo.GridControl = this.gcDataInfo;
            this.gvDataInfo.GroupPanelText = "将列表头拖拽到此处以分组";
            this.gvDataInfo.IndicatorWidth = 50;
            this.gvDataInfo.Name = "gvDataInfo";
            this.gvDataInfo.OptionsCustomization.AllowSort = false;
            this.gvDataInfo.OptionsMenu.EnableColumnMenu = false;
            this.gvDataInfo.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvDataInfo.OptionsPrint.AutoWidth = false;
            this.gvDataInfo.OptionsView.ShowFooter = true;
            this.gvDataInfo.OptionsView.ShowGroupedColumns = true;
            this.gvDataInfo.OptionsView.ShowGroupPanel = false;
            this.gvDataInfo.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDataInfo_CustomDrawRowIndicator);
            // 
            // gridColumn0
            // 
            this.gridColumn0.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn0.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn0.Caption = "选择";
            this.gridColumn0.ColumnEdit = this.repositoryItemCheckEdit1;
            this.gridColumn0.FieldName = "check";
            this.gridColumn0.Name = "gridColumn0";
            this.gridColumn0.Visible = true;
            this.gridColumn0.VisibleIndex = 0;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "序号";
            this.gridColumn1.Name = "gridColumn1";
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn2.Caption = "名称";
            this.gridColumn2.FieldName = "DIC_NAME";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            // 
            // gridColumn3
            // 
            this.gridColumn3.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn3.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn3.Caption = "类型";
            this.gridColumn3.FieldName = "DIC_TYPE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.ReadOnly = true;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "新能源申报清算数据（新）";
            this.saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            this.saveFileDialog.Title = "导出Excel";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // DataDictionaryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 662);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "DataDictionaryForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "数据字典";
            this.Load += new System.EventHandler(this.DataDictionaryForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.xtraScrollableControl1.ResumeLayout(false);
            this.xtraScrollableControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spanNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ceQueryAll.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Type.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Define.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcDataInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDataInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem barBtnImportNew;
        private DevExpress.XtraBars.BarButtonItem barBtnSingle;
        private DevExpress.XtraBars.BarButtonItem barBtnDelete;
        private DevExpress.XtraBars.BarButtonItem barBtnSelect;
        private DevExpress.XtraBars.BarButtonItem barBtnCancle;
        private DevExpress.XtraBars.BarButtonItem barBtnRefresh;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private DevExpress.XtraGrid.GridControl gcDataInfo;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDataInfo;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraEditors.SpinEdit spanNumber;
        private System.Windows.Forms.TextBox txtPage;
        private DevExpress.XtraEditors.SimpleButton btnFirPage;
        private DevExpress.XtraEditors.SimpleButton btnPrePage;
        private DevExpress.XtraEditors.SimpleButton btnLastPage;
        private DevExpress.XtraEditors.SimpleButton btnNextPage;
        private DevExpress.XtraEditors.LabelControl labPage;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.CheckEdit ceQueryAll;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl lblSum;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private DevExpress.XtraEditors.ComboBoxEdit Type;
        private DevExpress.XtraEditors.TextEdit Define;
        private DevExpress.XtraEditors.LabelControl labType;
        private DevExpress.XtraEditors.LabelControl labDefine;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn0;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private DevExpress.XtraBars.BarButtonItem barBtnExportFloder;
        private DevExpress.XtraBars.BarButtonItem barBtnUpdate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}