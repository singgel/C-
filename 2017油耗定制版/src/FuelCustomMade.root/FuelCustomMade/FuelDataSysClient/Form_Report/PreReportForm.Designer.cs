namespace FuelDataSysClient.Form_Report
{
    partial class PreReportForm
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
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.btnImport = new DevExpress.XtraBars.BarButtonItem();
            this.btnProduce = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.gcCafc = new DevExpress.XtraGrid.GridControl();
            this.bgvCafc = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
            this.gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.te_Qcscqy = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_Sl_act = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_Sl_hs = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_CAFC = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_TCAFC = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_ed = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_TCAFC106 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.te_TCAFC109 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.gridBand2 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
            this.ne_Sl_act = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_Sl_hs = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_Cafc = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_Tcafc = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_Qcscqy = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_ed = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_TCAFC106 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.ne_TCAFC109 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.comboBoxEdit1 = new DevExpress.XtraEditors.ComboBoxEdit();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.gcParam = new DevExpress.XtraGrid.GridControl();
            this.gvParam = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.checkData = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rceData = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.QCSCQY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLXH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.RLLX = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BSQXS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ZCZBZL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ZWPS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ZHGKXSLC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ZHGKRLXHL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.SL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BZ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ZHGKRLXHL_TGT = new DevExpress.XtraGrid.Columns.GridColumn();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcCafc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bgvCafc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcParam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvParam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rceData)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.btnImport,
            this.btnProduce});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 16;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1134, 147);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // btnImport
            // 
            this.btnImport.Caption = "导入规划车型";
            this.btnImport.Id = 14;
            this.btnImport.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_ImportOld;
            this.btnImport.Name = "btnImport";
            this.btnImport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnImport_ItemClick);
            // 
            // btnProduce
            // 
            this.btnProduce.Caption = "生成预报告";
            this.btnProduce.Id = 15;
            this.btnProduce.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_ExportWord;
            this.btnProduce.Name = "btnProduce";
            this.btnProduce.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnProduce_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.btnImport);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "数据";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnProduce);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "操作";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 508);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1134, 31);
            this.ribbonStatusBar.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsAdd,
            this.cmsEdit,
            this.cmsDelete});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(149, 70);
            // 
            // cmsAdd
            // 
            this.cmsAdd.Name = "cmsAdd";
            this.cmsAdd.Size = new System.Drawing.Size(148, 22);
            this.cmsAdd.Text = "新增预测项目";
            // 
            // cmsEdit
            // 
            this.cmsEdit.Name = "cmsEdit";
            this.cmsEdit.Size = new System.Drawing.Size(148, 22);
            this.cmsEdit.Text = "编辑预测项目";
            // 
            // cmsDelete
            // 
            this.cmsDelete.Name = "cmsDelete";
            this.cmsDelete.Size = new System.Drawing.Size(148, 22);
            this.cmsDelete.Text = "删除预测项目";
            // 
            // gcCafc
            // 
            this.gcCafc.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gcCafc.Location = new System.Drawing.Point(0, 375);
            this.gcCafc.MainView = this.bgvCafc;
            this.gcCafc.MenuManager = this.ribbon;
            this.gcCafc.Name = "gcCafc";
            this.gcCafc.Size = new System.Drawing.Size(1134, 133);
            this.gcCafc.TabIndex = 57;
            this.gcCafc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.bgvCafc});
            // 
            // bgvCafc
            // 
            this.bgvCafc.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.bgvCafc.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.bgvCafc.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
            this.gridBand1,
            this.gridBand2});
            this.bgvCafc.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[] {
            this.te_Qcscqy,
            this.te_Sl_act,
            this.te_Sl_hs,
            this.te_CAFC,
            this.te_TCAFC,
            this.te_ed,
            this.te_TCAFC106,
            this.te_TCAFC109,
            this.ne_Qcscqy,
            this.ne_Sl_act,
            this.ne_Sl_hs,
            this.ne_Cafc,
            this.ne_Tcafc,
            this.ne_ed,
            this.ne_TCAFC106,
            this.ne_TCAFC109});
            this.bgvCafc.GridControl = this.gcCafc;
            this.bgvCafc.Name = "bgvCafc";
            this.bgvCafc.OptionsBehavior.ReadOnly = true;
            this.bgvCafc.OptionsView.ShowGroupPanel = false;
            // 
            // gridBand1
            // 
            this.gridBand1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBand1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBand1.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridBand1.Caption = "不计新能源";
            this.gridBand1.Columns.Add(this.te_Qcscqy);
            this.gridBand1.Columns.Add(this.te_Sl_act);
            this.gridBand1.Columns.Add(this.te_Sl_hs);
            this.gridBand1.Columns.Add(this.te_CAFC);
            this.gridBand1.Columns.Add(this.te_TCAFC);
            this.gridBand1.Columns.Add(this.te_ed);
            this.gridBand1.Columns.Add(this.te_TCAFC106);
            this.gridBand1.Columns.Add(this.te_TCAFC109);
            this.gridBand1.Name = "gridBand1";
            this.gridBand1.Width = 708;
            // 
            // te_Qcscqy
            // 
            this.te_Qcscqy.Caption = "企业名称";
            this.te_Qcscqy.FieldName = "te_Qcscqy";
            this.te_Qcscqy.Name = "te_Qcscqy";
            this.te_Qcscqy.Width = 260;
            // 
            // te_Sl_act
            // 
            this.te_Sl_act.AppearanceCell.Options.UseTextOptions = true;
            this.te_Sl_act.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_Sl_act.Caption = "实际数量";
            this.te_Sl_act.FieldName = "te_Sl_act";
            this.te_Sl_act.Name = "te_Sl_act";
            this.te_Sl_act.Visible = true;
            this.te_Sl_act.Width = 168;
            // 
            // te_Sl_hs
            // 
            this.te_Sl_hs.AppearanceCell.Options.UseTextOptions = true;
            this.te_Sl_hs.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_Sl_hs.Caption = "核算数量";
            this.te_Sl_hs.FieldName = "te_Sl_hs";
            this.te_Sl_hs.Name = "te_Sl_hs";
            this.te_Sl_hs.Visible = true;
            this.te_Sl_hs.Width = 168;
            // 
            // te_CAFC
            // 
            this.te_CAFC.AppearanceCell.Options.UseTextOptions = true;
            this.te_CAFC.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_CAFC.Caption = "CAFC";
            this.te_CAFC.FieldName = "te_Cafc";
            this.te_CAFC.Name = "te_CAFC";
            this.te_CAFC.Visible = true;
            this.te_CAFC.Width = 184;
            // 
            // te_TCAFC
            // 
            this.te_TCAFC.AppearanceCell.Options.UseTextOptions = true;
            this.te_TCAFC.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_TCAFC.Caption = "TCAFC";
            this.te_TCAFC.FieldName = "te_Tcafc";
            this.te_TCAFC.Name = "te_TCAFC";
            this.te_TCAFC.Visible = true;
            this.te_TCAFC.Width = 188;
            // 
            // te_ed
            // 
            this.te_ed.Caption = "额度";
            this.te_ed.FieldName = "te_ed";
            this.te_ed.Name = "te_ed";
            // 
            // te_TCAFC106
            // 
            this.te_TCAFC106.Caption = "TCAFC*106%";
            this.te_TCAFC106.FieldName = "te_TCAFC106";
            this.te_TCAFC106.Name = "te_TCAFC106";
            // 
            // te_TCAFC109
            // 
            this.te_TCAFC109.Caption = "TCAFC*109%";
            this.te_TCAFC109.FieldName = "te_TCAFC109";
            this.te_TCAFC109.Name = "te_TCAFC109";
            // 
            // gridBand2
            // 
            this.gridBand2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridBand2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridBand2.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gridBand2.Caption = "计入新能源";
            this.gridBand2.Columns.Add(this.ne_Sl_act);
            this.gridBand2.Columns.Add(this.ne_Sl_hs);
            this.gridBand2.Columns.Add(this.ne_Cafc);
            this.gridBand2.Columns.Add(this.ne_Tcafc);
            this.gridBand2.Name = "gridBand2";
            this.gridBand2.Width = 703;
            // 
            // ne_Sl_act
            // 
            this.ne_Sl_act.Caption = "实际数量";
            this.ne_Sl_act.FieldName = "ne_Sl_act";
            this.ne_Sl_act.Name = "ne_Sl_act";
            this.ne_Sl_act.Visible = true;
            this.ne_Sl_act.Width = 177;
            // 
            // ne_Sl_hs
            // 
            this.ne_Sl_hs.Caption = "核算数量";
            this.ne_Sl_hs.FieldName = "ne_Sl_hs";
            this.ne_Sl_hs.Name = "ne_Sl_hs";
            this.ne_Sl_hs.Visible = true;
            this.ne_Sl_hs.Width = 157;
            // 
            // ne_Cafc
            // 
            this.ne_Cafc.Caption = "CAFC";
            this.ne_Cafc.FieldName = "ne_Cafc";
            this.ne_Cafc.Name = "ne_Cafc";
            this.ne_Cafc.Visible = true;
            this.ne_Cafc.Width = 155;
            // 
            // ne_Tcafc
            // 
            this.ne_Tcafc.Caption = "TCAFC";
            this.ne_Tcafc.FieldName = "ne_Tcafc";
            this.ne_Tcafc.Name = "ne_Tcafc";
            this.ne_Tcafc.Visible = true;
            this.ne_Tcafc.Width = 214;
            // 
            // ne_Qcscqy
            // 
            this.ne_Qcscqy.Caption = "企业名称";
            this.ne_Qcscqy.FieldName = "ne_Qcscqy";
            this.ne_Qcscqy.Name = "ne_Qcscqy";
            // 
            // ne_ed
            // 
            this.ne_ed.Caption = "额度";
            this.ne_ed.FieldName = "ne_ed";
            this.ne_ed.Name = "ne_ed";
            // 
            // ne_TCAFC106
            // 
            this.ne_TCAFC106.Caption = "TCAFC*106%";
            this.ne_TCAFC106.FieldName = "ne_TCAFC106";
            this.ne_TCAFC106.Name = "ne_TCAFC106";
            // 
            // ne_TCAFC109
            // 
            this.ne_TCAFC109.Caption = "TCAFC*109%";
            this.ne_TCAFC109.FieldName = "ne_TCAFC109";
            this.ne_TCAFC109.Name = "ne_TCAFC109";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.comboBoxEdit1);
            this.panelControl1.Controls.Add(this.simpleButton1);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 147);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1134, 40);
            this.panelControl1.TabIndex = 64;
            // 
            // comboBoxEdit1
            // 
            this.comboBoxEdit1.EditValue = "";
            this.comboBoxEdit1.Location = new System.Drawing.Point(80, 10);
            this.comboBoxEdit1.MenuManager = this.ribbon;
            this.comboBoxEdit1.Name = "comboBoxEdit1";
            this.comboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.comboBoxEdit1.Size = new System.Drawing.Size(100, 20);
            this.comboBoxEdit1.TabIndex = 61;
            this.comboBoxEdit1.SelectedIndexChanged += new System.EventHandler(this.comboBoxEdit1_SelectedIndexChanged);
            // 
            // simpleButton1
            // 
            this.simpleButton1.Location = new System.Drawing.Point(196, 10);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(80, 20);
            this.simpleButton1.TabIndex = 5;
            this.simpleButton1.Text = "核    算";
            this.simpleButton1.Click += new System.EventHandler(this.btnTeSearch_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(26, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(48, 14);
            this.labelControl1.TabIndex = 60;
            this.labelControl1.Text = "选择年度";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.gcParam);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 187);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1134, 188);
            this.panelControl2.TabIndex = 67;
            // 
            // gcParam
            // 
            this.gcParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcParam.Location = new System.Drawing.Point(2, 2);
            this.gcParam.MainView = this.gvParam;
            this.gcParam.Name = "gcParam";
            this.gcParam.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.rceData});
            this.gcParam.Size = new System.Drawing.Size(1130, 184);
            this.gcParam.TabIndex = 62;
            this.gcParam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvParam});
            // 
            // gvParam
            // 
            this.gvParam.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.checkData,
            this.QCSCQY,
            this.CLXH,
            this.RLLX,
            this.BSQXS,
            this.ZCZBZL,
            this.ZWPS,
            this.ZHGKXSLC,
            this.ZHGKRLXHL,
            this.SL,
            this.BZ,
            this.ZHGKRLXHL_TGT});
            this.gvParam.GridControl = this.gcParam;
            this.gvParam.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Count, "", null, "")});
            this.gvParam.Name = "gvParam";
            // 
            // checkData
            // 
            this.checkData.Caption = "选择";
            this.checkData.ColumnEdit = this.rceData;
            this.checkData.FieldName = "Check";
            this.checkData.Name = "checkData";
            this.checkData.Tag = false;
            this.checkData.Visible = true;
            this.checkData.VisibleIndex = 0;
            this.checkData.Width = 50;
            // 
            // rceData
            // 
            this.rceData.AutoHeight = false;
            this.rceData.Name = "rceData";
            // 
            // QCSCQY
            // 
            this.QCSCQY.Caption = "企业名称";
            this.QCSCQY.FieldName = "QCSCQY";
            this.QCSCQY.Name = "QCSCQY";
            this.QCSCQY.OptionsColumn.ReadOnly = true;
            this.QCSCQY.Width = 210;
            // 
            // CLXH
            // 
            this.CLXH.Caption = "车辆型号";
            this.CLXH.FieldName = "CLXH";
            this.CLXH.Name = "CLXH";
            this.CLXH.OptionsColumn.ReadOnly = true;
            this.CLXH.Visible = true;
            this.CLXH.VisibleIndex = 1;
            this.CLXH.Width = 139;
            // 
            // RLLX
            // 
            this.RLLX.Caption = "燃料类型";
            this.RLLX.FieldName = "RLLX";
            this.RLLX.Name = "RLLX";
            this.RLLX.OptionsColumn.ReadOnly = true;
            this.RLLX.Visible = true;
            this.RLLX.VisibleIndex = 2;
            this.RLLX.Width = 110;
            // 
            // BSQXS
            // 
            this.BSQXS.Caption = "变速器型式";
            this.BSQXS.FieldName = "BSQXS";
            this.BSQXS.Name = "BSQXS";
            this.BSQXS.OptionsColumn.ReadOnly = true;
            this.BSQXS.Visible = true;
            this.BSQXS.VisibleIndex = 3;
            this.BSQXS.Width = 100;
            // 
            // ZCZBZL
            // 
            this.ZCZBZL.AppearanceCell.Options.UseTextOptions = true;
            this.ZCZBZL.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ZCZBZL.Caption = "整车整备质量";
            this.ZCZBZL.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.ZCZBZL.FieldName = "ZCZBZL";
            this.ZCZBZL.Name = "ZCZBZL";
            this.ZCZBZL.OptionsColumn.ReadOnly = true;
            this.ZCZBZL.Visible = true;
            this.ZCZBZL.VisibleIndex = 4;
            this.ZCZBZL.Width = 100;
            // 
            // ZWPS
            // 
            this.ZWPS.AppearanceCell.Options.UseTextOptions = true;
            this.ZWPS.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ZWPS.Caption = "座位排数";
            this.ZWPS.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.ZWPS.FieldName = "ZWPS";
            this.ZWPS.Name = "ZWPS";
            this.ZWPS.OptionsColumn.ReadOnly = true;
            this.ZWPS.Visible = true;
            this.ZWPS.VisibleIndex = 5;
            this.ZWPS.Width = 100;
            // 
            // ZHGKXSLC
            // 
            this.ZHGKXSLC.AppearanceCell.Options.UseTextOptions = true;
            this.ZHGKXSLC.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ZHGKXSLC.Caption = "纯电动驱动模式综合工况续驶里程";
            this.ZHGKXSLC.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.ZHGKXSLC.FieldName = "ZHGKXSLC";
            this.ZHGKXSLC.Name = "ZHGKXSLC";
            this.ZHGKXSLC.OptionsColumn.ReadOnly = true;
            this.ZHGKXSLC.Visible = true;
            this.ZHGKXSLC.VisibleIndex = 6;
            this.ZHGKXSLC.Width = 100;
            // 
            // ZHGKRLXHL
            // 
            this.ZHGKRLXHL.AppearanceCell.Options.UseTextOptions = true;
            this.ZHGKRLXHL.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ZHGKRLXHL.Caption = "综合工况燃料消耗量";
            this.ZHGKRLXHL.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.ZHGKRLXHL.FieldName = "ZHGKRLXHL";
            this.ZHGKRLXHL.Name = "ZHGKRLXHL";
            this.ZHGKRLXHL.OptionsColumn.ReadOnly = true;
            this.ZHGKRLXHL.Visible = true;
            this.ZHGKRLXHL.VisibleIndex = 7;
            this.ZHGKRLXHL.Width = 100;
            // 
            // SL
            // 
            this.SL.AppearanceCell.Options.UseTextOptions = true;
            this.SL.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.SL.Caption = "预计制造/进口量";
            this.SL.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.SL.FieldName = "SL";
            this.SL.Name = "SL";
            this.SL.OptionsColumn.ReadOnly = true;
            this.SL.Visible = true;
            this.SL.VisibleIndex = 8;
            this.SL.Width = 100;
            // 
            // BZ
            // 
            this.BZ.Caption = "备注";
            this.BZ.FieldName = "BZ";
            this.BZ.Name = "BZ";
            this.BZ.OptionsColumn.ReadOnly = true;
            // 
            // ZHGKRLXHL_TGT
            // 
            this.ZHGKRLXHL_TGT.Caption = "车型燃料消耗量目标值";
            this.ZHGKRLXHL_TGT.FieldName = "ZHGKRLXHL_TGT";
            this.ZHGKRLXHL_TGT.Name = "ZHGKRLXHL_TGT";
            this.ZHGKRLXHL_TGT.OptionsColumn.ReadOnly = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "预报告生成结果";
            this.saveFileDialog.Filter = "Word文件(*.doc)|*.doc";
            this.saveFileDialog.Title = "生成Word";
            // 
            // PreReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 539);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.gcCafc);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "PreReportForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "预报告生成";
            this.Load += new System.EventHandler(this.PreReportForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcCafc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bgvCafc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.comboBoxEdit1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcParam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvParam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rceData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cmsAdd;
        private System.Windows.Forms.ToolStripMenuItem cmsEdit;
        private System.Windows.Forms.ToolStripMenuItem cmsDelete;
        private DevExpress.XtraBars.BarButtonItem btnImport;
        private DevExpress.XtraGrid.GridControl gcCafc;
        private DevExpress.XtraBars.BarButtonItem btnProduce;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraEditors.ComboBoxEdit comboBoxEdit1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton simpleButton1;
        private DevExpress.XtraGrid.GridControl gcParam;
        private DevExpress.XtraGrid.Views.Grid.GridView gvParam;
        private DevExpress.XtraGrid.Columns.GridColumn checkData;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit rceData;
        private DevExpress.XtraGrid.Columns.GridColumn QCSCQY;
        private DevExpress.XtraGrid.Columns.GridColumn CLXH;
        private DevExpress.XtraGrid.Columns.GridColumn RLLX;
        private DevExpress.XtraGrid.Columns.GridColumn BSQXS;
        private DevExpress.XtraGrid.Columns.GridColumn ZCZBZL;
        private DevExpress.XtraGrid.Columns.GridColumn ZWPS;
        private DevExpress.XtraGrid.Columns.GridColumn ZHGKXSLC;
        private DevExpress.XtraGrid.Columns.GridColumn ZHGKRLXHL;
        private DevExpress.XtraGrid.Columns.GridColumn SL;
        private DevExpress.XtraGrid.Columns.GridColumn BZ;
        private DevExpress.XtraGrid.Columns.GridColumn ZHGKRLXHL_TGT;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bgvCafc;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_Qcscqy;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_Sl_act;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_Sl_hs;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_CAFC;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_TCAFC;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_ed;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_TCAFC106;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn te_TCAFC109;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_Qcscqy;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_Sl_act;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_Sl_hs;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_Cafc;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_Tcafc;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_ed;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_TCAFC106;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn ne_TCAFC109;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand1;
        private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}