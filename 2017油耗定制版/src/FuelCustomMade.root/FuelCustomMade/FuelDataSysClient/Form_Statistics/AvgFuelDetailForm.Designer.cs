namespace FuelDataSysClient.Form_Statistics
{
    partial class AvgFuelDetailForm
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
            this.btnExportDetail = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.cbRllx = new DevExpress.XtraEditors.ComboBoxEdit();
            this.hzbgExport = new DevExpress.XtraEditors.SimpleButton();
            this.lblNum = new DevExpress.XtraEditors.LabelControl();
            this.dtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.dtStartTime = new DevExpress.XtraEditors.DateEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtclxh = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.gcDetail = new DevExpress.XtraGrid.GridControl();
            this.gvDetail = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.xh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clscqy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clxh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rlzl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.BSQXS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.zczbzl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.zwps = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cddqdmszhgkxslc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cxrlxhlmbz = new DevExpress.XtraGrid.Columns.GridColumn();
            this.zhgkrlxhlsjz = new DevExpress.XtraGrid.Columns.GridColumn();
            this.hsjkl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.sjjkl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rhlVin = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bz = new DevExpress.XtraGrid.Columns.GridColumn();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbRllx.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtclxh.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rhlVin)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.ExpandCollapseItem.Name = "";
            this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbonControl1.ExpandCollapseItem,
            this.btnExportDetail});
            this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
            this.ribbonControl1.MaxItemId = 2;
            this.ribbonControl1.Name = "ribbonControl1";
            this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbonControl1.Size = new System.Drawing.Size(1182, 147);
            this.ribbonControl1.StatusBar = this.ribbonStatusBar;
            // 
            // btnExportDetail
            // 
            this.btnExportDetail.Caption = "导出EXCEL";
            this.btnExportDetail.Id = 1;
            this.btnExportDetail.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_ExportExcel;
            this.btnExportDetail.Name = "btnExportDetail";
            this.btnExportDetail.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnExportDetail_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.btnExportDetail);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "数据";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 477);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbonControl1;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1182, 31);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraScrollableControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 147);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1182, 105);
            this.panelControl1.TabIndex = 2;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.cbRllx);
            this.xtraScrollableControl1.Controls.Add(this.hzbgExport);
            this.xtraScrollableControl1.Controls.Add(this.lblNum);
            this.xtraScrollableControl1.Controls.Add(this.dtEndTime);
            this.xtraScrollableControl1.Controls.Add(this.dtStartTime);
            this.xtraScrollableControl1.Controls.Add(this.btnSearch);
            this.xtraScrollableControl1.Controls.Add(this.txtclxh);
            this.xtraScrollableControl1.Controls.Add(this.labelControl3);
            this.xtraScrollableControl1.Controls.Add(this.labelControl6);
            this.xtraScrollableControl1.Controls.Add(this.labelControl5);
            this.xtraScrollableControl1.Controls.Add(this.labelControl4);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(1178, 101);
            this.xtraScrollableControl1.TabIndex = 0;
            // 
            // cbRllx
            // 
            this.cbRllx.Location = new System.Drawing.Point(411, 10);
            this.cbRllx.MenuManager = this.ribbonControl1;
            this.cbRllx.Name = "cbRllx";
            this.cbRllx.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cbRllx.Size = new System.Drawing.Size(120, 20);
            this.cbRllx.TabIndex = 66;
            // 
            // hzbgExport
            // 
            this.hzbgExport.Location = new System.Drawing.Point(852, 38);
            this.hzbgExport.Name = "hzbgExport";
            this.hzbgExport.Size = new System.Drawing.Size(120, 25);
            this.hzbgExport.TabIndex = 65;
            this.hzbgExport.Text = "汇总报告导出(&E)";
            this.hzbgExport.Visible = false;
            this.hzbgExport.Click += new System.EventHandler(this.hzbgExport_Click);
            // 
            // lblNum
            // 
            this.lblNum.Location = new System.Drawing.Point(23, 76);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(31, 14);
            this.lblNum.TabIndex = 64;
            this.lblNum.Text = "共0条";
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(411, 40);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtEndTime.Size = new System.Drawing.Size(120, 20);
            this.dtEndTime.TabIndex = 63;
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(128, 40);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 62;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(852, 8);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 25);
            this.btnSearch.TabIndex = 61;
            this.btnSearch.Text = "查  询(&S)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtclxh
            // 
            this.txtclxh.Location = new System.Drawing.Point(127, 10);
            this.txtclxh.MenuManager = this.ribbonControl1;
            this.txtclxh.Name = "txtclxh";
            this.txtclxh.Size = new System.Drawing.Size(120, 20);
            this.txtclxh.TabIndex = 59;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(297, 13);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(48, 14);
            this.labelControl3.TabIndex = 56;
            this.labelControl3.Text = "燃料类型";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(297, 43);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(12, 14);
            this.labelControl6.TabIndex = 55;
            this.labelControl6.Text = "至";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(24, 43);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(77, 14);
            this.labelControl5.TabIndex = 58;
            this.labelControl5.Text = "制造/进口日期";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(24, 13);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 14);
            this.labelControl4.TabIndex = 57;
            this.labelControl4.Text = "车辆型号";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.gcDetail);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 252);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1182, 225);
            this.panelControl2.TabIndex = 3;
            // 
            // gcDetail
            // 
            this.gcDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcDetail.Location = new System.Drawing.Point(2, 2);
            this.gcDetail.MainView = this.gvDetail;
            this.gcDetail.MenuManager = this.ribbonControl1;
            this.gcDetail.Name = "gcDetail";
            this.gcDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.rhlVin});
            this.gcDetail.Size = new System.Drawing.Size(1178, 221);
            this.gcDetail.TabIndex = 0;
            this.gcDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDetail});
            this.gcDetail.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gcDetail_MouseDoubleClick);
            // 
            // gvDetail
            // 
            this.gvDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.xh,
            this.clscqy,
            this.clxh,
            this.rlzl,
            this.BSQXS,
            this.zczbzl,
            this.zwps,
            this.cddqdmszhgkxslc,
            this.cxrlxhlmbz,
            this.zhgkrlxhlsjz,
            this.hsjkl,
            this.sjjkl,
            this.gridColumn1,
            this.gridColumn2,
            this.bz});
            this.gvDetail.GridControl = this.gcDetail;
            this.gvDetail.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Sl_act", null, "")});
            this.gvDetail.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.gvDetail.Name = "gvDetail";
            // 
            // xh
            // 
            this.xh.Caption = "序号";
            this.xh.FieldName = "xh";
            this.xh.Name = "xh";
            // 
            // clscqy
            // 
            this.clscqy.Caption = "企业";
            this.clscqy.FieldName = "Qcscqy";
            this.clscqy.Name = "clscqy";
            this.clscqy.OptionsColumn.ReadOnly = true;
            // 
            // clxh
            // 
            this.clxh.Caption = "车辆型号";
            this.clxh.FieldName = "Clxh";
            this.clxh.Name = "clxh";
            this.clxh.OptionsColumn.ReadOnly = true;
            this.clxh.Visible = true;
            this.clxh.VisibleIndex = 0;
            // 
            // rlzl
            // 
            this.rlzl.Caption = "燃料类型";
            this.rlzl.FieldName = "Rllx";
            this.rlzl.Name = "rlzl";
            this.rlzl.OptionsColumn.ReadOnly = true;
            this.rlzl.Visible = true;
            this.rlzl.VisibleIndex = 1;
            // 
            // BSQXS
            // 
            this.BSQXS.Caption = "变速器型式";
            this.BSQXS.FieldName = "Bsqxs";
            this.BSQXS.Name = "BSQXS";
            this.BSQXS.OptionsColumn.ReadOnly = true;
            this.BSQXS.Visible = true;
            this.BSQXS.VisibleIndex = 2;
            // 
            // zczbzl
            // 
            this.zczbzl.AppearanceCell.Options.UseTextOptions = true;
            this.zczbzl.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.zczbzl.Caption = "整车整备质量";
            this.zczbzl.FieldName = "Zczbzl";
            this.zczbzl.Name = "zczbzl";
            this.zczbzl.OptionsColumn.ReadOnly = true;
            this.zczbzl.Visible = true;
            this.zczbzl.VisibleIndex = 3;
            // 
            // zwps
            // 
            this.zwps.AppearanceCell.Options.UseTextOptions = true;
            this.zwps.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.zwps.Caption = "座位排数";
            this.zwps.FieldName = "Zwps";
            this.zwps.Name = "zwps";
            this.zwps.OptionsColumn.ReadOnly = true;
            this.zwps.Visible = true;
            this.zwps.VisibleIndex = 4;
            // 
            // cddqdmszhgkxslc
            // 
            this.cddqdmszhgkxslc.AppearanceCell.Options.UseTextOptions = true;
            this.cddqdmszhgkxslc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.cddqdmszhgkxslc.Caption = "纯电动驱动模式综合工况续驶里程";
            this.cddqdmszhgkxslc.FieldName = "Zhgkxslc";
            this.cddqdmszhgkxslc.Name = "cddqdmszhgkxslc";
            this.cddqdmszhgkxslc.OptionsColumn.ReadOnly = true;
            this.cddqdmszhgkxslc.Visible = true;
            this.cddqdmszhgkxslc.VisibleIndex = 5;
            // 
            // cxrlxhlmbz
            // 
            this.cxrlxhlmbz.AppearanceCell.Options.UseTextOptions = true;
            this.cxrlxhlmbz.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.cxrlxhlmbz.Caption = "车型燃料消耗量目标值";
            this.cxrlxhlmbz.FieldName = "TgtZhgkrlxhl";
            this.cxrlxhlmbz.Name = "cxrlxhlmbz";
            this.cxrlxhlmbz.OptionsColumn.ReadOnly = true;
            this.cxrlxhlmbz.Visible = true;
            this.cxrlxhlmbz.VisibleIndex = 6;
            // 
            // zhgkrlxhlsjz
            // 
            this.zhgkrlxhlsjz.AppearanceCell.Options.UseTextOptions = true;
            this.zhgkrlxhlsjz.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.zhgkrlxhlsjz.Caption = "综合工况燃料消耗量实际值";
            this.zhgkrlxhlsjz.FieldName = "ActZhgkrlxhl";
            this.zhgkrlxhlsjz.Name = "zhgkrlxhlsjz";
            this.zhgkrlxhlsjz.OptionsColumn.ReadOnly = true;
            this.zhgkrlxhlsjz.Visible = true;
            this.zhgkrlxhlsjz.VisibleIndex = 7;
            // 
            // hsjkl
            // 
            this.hsjkl.AppearanceCell.Options.UseTextOptions = true;
            this.hsjkl.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.hsjkl.Caption = "核算数量";
            this.hsjkl.FieldName = "Sl_hs";
            this.hsjkl.Name = "hsjkl";
            this.hsjkl.OptionsColumn.ReadOnly = true;
            this.hsjkl.Visible = true;
            this.hsjkl.VisibleIndex = 8;
            // 
            // sjjkl
            // 
            this.sjjkl.AppearanceCell.Options.UseTextOptions = true;
            this.sjjkl.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.sjjkl.Caption = "实际数量";
            this.sjjkl.ColumnEdit = this.rhlVin;
            this.sjjkl.FieldName = "Sl_act";
            this.sjjkl.Name = "sjjkl";
            this.sjjkl.OptionsColumn.ReadOnly = true;
            this.sjjkl.Visible = true;
            this.sjjkl.VisibleIndex = 9;
            // 
            // rhlVin
            // 
            this.rhlVin.AutoHeight = false;
            this.rhlVin.Name = "rhlVin";
            this.rhlVin.Click += new System.EventHandler(this.rhlVin_Click);
            // 
            // gridColumn1
            // 
            this.gridColumn1.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn1.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumn1.Caption = "车型实际油耗×进口量";
            this.gridColumn1.FieldName = "P_ACT";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.ReadOnly = true;
            // 
            // gridColumn2
            // 
            this.gridColumn2.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn2.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridColumn2.Caption = "车型油耗目标值×进口量";
            this.gridColumn2.FieldName = "P_TGT";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.ReadOnly = true;
            // 
            // bz
            // 
            this.bz.Caption = "备注";
            this.bz.FieldName = "bz";
            this.bz.Name = "bz";
            this.bz.OptionsColumn.ReadOnly = true;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "油耗参数明细";
            this.saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            this.saveFileDialog.Title = "导出Excel";
            // 
            // AvgFuelDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 508);
            this.Controls.Add(this.panelControl2);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbonControl1);
            this.Name = "AvgFuelDetailForm";
            this.Ribbon = this.ribbonControl1;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "油耗参数明细";
            this.Load += new System.EventHandler(this.AvgFuelDetailForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.xtraScrollableControl1.ResumeLayout(false);
            this.xtraScrollableControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cbRllx.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtclxh.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rhlVin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem btnExportDetail;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.PanelControl panelControl2;
        private DevExpress.XtraGrid.GridControl gcDetail;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDetail;
        private DevExpress.XtraGrid.Columns.GridColumn xh;
        private DevExpress.XtraGrid.Columns.GridColumn clscqy;
        private DevExpress.XtraGrid.Columns.GridColumn clxh;
        private DevExpress.XtraGrid.Columns.GridColumn rlzl;
        private DevExpress.XtraGrid.Columns.GridColumn zczbzl;
        private DevExpress.XtraGrid.Columns.GridColumn BSQXS;
        private DevExpress.XtraGrid.Columns.GridColumn zwps;
        private DevExpress.XtraGrid.Columns.GridColumn cddqdmszhgkxslc;
        private DevExpress.XtraGrid.Columns.GridColumn cxrlxhlmbz;
        private DevExpress.XtraGrid.Columns.GridColumn zhgkrlxhlsjz;
        private DevExpress.XtraGrid.Columns.GridColumn sjjkl;
        private DevExpress.XtraGrid.Columns.GridColumn bz;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn hsjkl;
        private DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit rhlVin;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraEditors.SimpleButton hzbgExport;
        private DevExpress.XtraEditors.LabelControl lblNum;
        private DevExpress.XtraEditors.DateEdit dtEndTime;
        private DevExpress.XtraEditors.DateEdit dtStartTime;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtclxh;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.ComboBoxEdit cbRllx;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}