namespace FuelDataSysClient.Form_Statistics
{
    partial class FuelDataUploadTotalForm
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
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblNum = new DevExpress.XtraEditors.LabelControl();
            this.dtStartTime = new DevExpress.XtraEditors.DateEdit();
            this.dtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.gcStatistic = new DevExpress.XtraGrid.GridControl();
            this.gvStatistic = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.QCSCQY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clxh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.jkrq = new DevExpress.XtraGrid.Columns.GridColumn();
            this.jkl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rhlVin = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcStatistic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvStatistic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rhlVin)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.barButtonItem1});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 2;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(1182, 147);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "导出Excel";
            this.barButtonItem1.Id = 1;
            this.barButtonItem1.LargeGlyph = global::FuelDataSysClient.Properties.Resources.barBtn_ExportExcel;
            this.barButtonItem1.Name = "barButtonItem1";
            this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.barButtonItem1);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "操作";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 477);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1182, 31);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lblNum);
            this.panelControl1.Controls.Add(this.dtStartTime);
            this.panelControl1.Controls.Add(this.dtEndTime);
            this.panelControl1.Controls.Add(this.btnSearch);
            this.panelControl1.Controls.Add(this.labelControl2);
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 147);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1182, 105);
            this.panelControl1.TabIndex = 5;
            // 
            // lblNum
            // 
            this.lblNum.Location = new System.Drawing.Point(22, 79);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(32, 14);
            this.lblNum.TabIndex = 53;
            this.lblNum.Text = "共  条";
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(126, 13);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 52;
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(410, 13);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtEndTime.Size = new System.Drawing.Size(120, 20);
            this.dtEndTime.TabIndex = 52;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(851, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "查询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(296, 16);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(12, 14);
            this.labelControl2.TabIndex = 0;
            this.labelControl2.Text = "至";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(23, 16);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(77, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "制造/进口日期";
            // 
            // gcStatistic
            // 
            this.gcStatistic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcStatistic.Location = new System.Drawing.Point(0, 252);
            this.gcStatistic.MainView = this.gvStatistic;
            this.gcStatistic.MenuManager = this.ribbon;
            this.gcStatistic.Name = "gcStatistic";
            this.gcStatistic.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.rhlVin});
            this.gcStatistic.Size = new System.Drawing.Size(1182, 225);
            this.gcStatistic.TabIndex = 6;
            this.gcStatistic.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvStatistic});
            this.gcStatistic.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.gcStatistic_MouseDoubleClick);
            // 
            // gvStatistic
            // 
            this.gvStatistic.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.QCSCQY,
            this.clxh,
            this.jkrq,
            this.jkl});
            this.gvStatistic.GridControl = this.gcStatistic;
            this.gvStatistic.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Sl", null, "")});
            this.gvStatistic.Name = "gvStatistic";
            // 
            // QCSCQY
            // 
            this.QCSCQY.Caption = "企业";
            this.QCSCQY.FieldName = "Qcscqy";
            this.QCSCQY.Name = "QCSCQY";
            this.QCSCQY.OptionsColumn.ReadOnly = true;
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
            // jkrq
            // 
            this.jkrq.Caption = "制造日期/进口日期";
            this.jkrq.FieldName = "Clzzrq";
            this.jkrq.Name = "jkrq";
            this.jkrq.OptionsColumn.ReadOnly = true;
            this.jkrq.Visible = true;
            this.jkrq.VisibleIndex = 1;
            // 
            // jkl
            // 
            this.jkl.AppearanceCell.Options.UseTextOptions = true;
            this.jkl.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.jkl.Caption = "进口量";
            this.jkl.ColumnEdit = this.rhlVin;
            this.jkl.FieldName = "Sl";
            this.jkl.Name = "jkl";
            this.jkl.Visible = true;
            this.jkl.VisibleIndex = 2;
            // 
            // rhlVin
            // 
            this.rhlVin.AutoHeight = false;
            this.rhlVin.Name = "rhlVin";
            this.rhlVin.Click += new System.EventHandler(this.rhlVin_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "数据上报统计";
            this.saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            this.saveFileDialog.Title = "导出Excel";
            // 
            // FuelDataUploadTotalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 508);
            this.Controls.Add(this.gcStatistic);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "FuelDataUploadTotalForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "数据上报统计";
            this.Load += new System.EventHandler(this.FuelDataUploadTotalForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcStatistic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvStatistic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rhlVin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraGrid.GridControl gcStatistic;
        private DevExpress.XtraGrid.Views.Grid.GridView gvStatistic;
        private DevExpress.XtraGrid.Columns.GridColumn QCSCQY;
        private DevExpress.XtraGrid.Columns.GridColumn clxh;
        private DevExpress.XtraGrid.Columns.GridColumn jkrq;
        private DevExpress.XtraGrid.Columns.GridColumn jkl;
        private DevExpress.XtraEditors.DateEdit dtStartTime;
        private DevExpress.XtraEditors.DateEdit dtEndTime;
        private DevExpress.XtraEditors.LabelControl lblNum;
        private DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit rhlVin;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}