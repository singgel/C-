namespace FuelDataSysClient.Form_Account
{
    partial class CAFCForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CAFCForm));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
            this.dtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.dtStartTime = new DevExpress.XtraEditors.DateEdit();
            this.btnNeSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnTeSearch = new DevExpress.XtraEditors.SimpleButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gcNeCafc = new DevExpress.XtraGrid.GridControl();
            this.gvNeCafc = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ne_qymc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ne_Sl_act = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ne_Sl_hs = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ne_cafc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ne_tcafc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ed = new DevExpress.XtraGrid.Columns.GridColumn();
            this.TCAFC106 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tcafc109 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gcTeCafc = new DevExpress.XtraGrid.GridControl();
            this.gvTeCafc = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.te_Qcscqy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.te_Sl_act = new DevExpress.XtraGrid.Columns.GridColumn();
            this.te_Sl_hs = new DevExpress.XtraGrid.Columns.GridColumn();
            this.te_CAFC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.te_TCAFC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.xtraScrollableControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gcNeCafc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvNeCafc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcTeCafc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTeCafc)).BeginInit();
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
            this.ribbon.Size = new System.Drawing.Size(1084, 50);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barButtonItem1
            // 
            this.barButtonItem1.Caption = "导出";
            this.barButtonItem1.Id = 1;
            this.barButtonItem1.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barButtonItem1.LargeGlyph")));
            this.barButtonItem1.Name = "barButtonItem1";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 444);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(1084, 31);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.xtraScrollableControl1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 50);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1084, 60);
            this.panelControl1.TabIndex = 6;
            // 
            // xtraScrollableControl1
            // 
            this.xtraScrollableControl1.Controls.Add(this.dtEndTime);
            this.xtraScrollableControl1.Controls.Add(this.dtStartTime);
            this.xtraScrollableControl1.Controls.Add(this.btnNeSearch);
            this.xtraScrollableControl1.Controls.Add(this.btnTeSearch);
            this.xtraScrollableControl1.Controls.Add(this.labelControl2);
            this.xtraScrollableControl1.Controls.Add(this.labelControl1);
            this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraScrollableControl1.Location = new System.Drawing.Point(2, 2);
            this.xtraScrollableControl1.Name = "xtraScrollableControl1";
            this.xtraScrollableControl1.Size = new System.Drawing.Size(1080, 56);
            this.xtraScrollableControl1.TabIndex = 0;
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(332, 18);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtEndTime.Size = new System.Drawing.Size(120, 20);
            this.dtEndTime.TabIndex = 56;
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(120, 18);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 57;
            // 
            // btnNeSearch
            // 
            this.btnNeSearch.Location = new System.Drawing.Point(600, 16);
            this.btnNeSearch.Name = "btnNeSearch";
            this.btnNeSearch.Size = new System.Drawing.Size(120, 25);
            this.btnNeSearch.TabIndex = 55;
            this.btnNeSearch.Text = "核算（计入新能源）";
            this.btnNeSearch.Click += new System.EventHandler(this.btnNeSearch_Click);
            // 
            // btnTeSearch
            // 
            this.btnTeSearch.Location = new System.Drawing.Point(780, 16);
            this.btnTeSearch.Name = "btnTeSearch";
            this.btnTeSearch.Size = new System.Drawing.Size(120, 25);
            this.btnTeSearch.TabIndex = 54;
            this.btnTeSearch.Text = "核算（不计新能源）";
            this.btnTeSearch.Click += new System.EventHandler(this.btnTeSearch_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(280, 21);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(12, 14);
            this.labelControl2.TabIndex = 52;
            this.labelControl2.Text = "至";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(20, 21);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(77, 14);
            this.labelControl1.TabIndex = 53;
            this.labelControl1.Text = "制造/进口日期";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 110);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.gcNeCafc);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gcTeCafc);
            this.splitContainer1.Size = new System.Drawing.Size(1084, 334);
            this.splitContainer1.SplitterDistance = 158;
            this.splitContainer1.TabIndex = 7;
            // 
            // gcNeCafc
            // 
            this.gcNeCafc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcNeCafc.Location = new System.Drawing.Point(0, 0);
            this.gcNeCafc.MainView = this.gvNeCafc;
            this.gcNeCafc.MenuManager = this.ribbon;
            this.gcNeCafc.Name = "gcNeCafc";
            this.gcNeCafc.Size = new System.Drawing.Size(1084, 158);
            this.gcNeCafc.TabIndex = 1;
            this.gcNeCafc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvNeCafc});
            // 
            // gvNeCafc
            // 
            this.gvNeCafc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.ne_qymc,
            this.ne_Sl_act,
            this.ne_Sl_hs,
            this.ne_cafc,
            this.ne_tcafc,
            this.ed,
            this.TCAFC106,
            this.tcafc109});
            this.gvNeCafc.GridControl = this.gcNeCafc;
            this.gvNeCafc.Name = "gvNeCafc";
            this.gvNeCafc.OptionsView.ShowGroupPanel = false;
            // 
            // ne_qymc
            // 
            this.ne_qymc.Caption = "汽车企业名称";
            this.ne_qymc.FieldName = "Qcscqy";
            this.ne_qymc.Name = "ne_qymc";
            this.ne_qymc.OptionsColumn.ReadOnly = true;
            // 
            // ne_Sl_act
            // 
            this.ne_Sl_act.AppearanceCell.Options.UseTextOptions = true;
            this.ne_Sl_act.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ne_Sl_act.Caption = "实际数量(计入新能源)";
            this.ne_Sl_act.FieldName = "Sl_act";
            this.ne_Sl_act.Name = "ne_Sl_act";
            this.ne_Sl_act.OptionsColumn.ReadOnly = true;
            this.ne_Sl_act.Visible = true;
            this.ne_Sl_act.VisibleIndex = 0;
            // 
            // ne_Sl_hs
            // 
            this.ne_Sl_hs.AppearanceCell.Options.UseTextOptions = true;
            this.ne_Sl_hs.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ne_Sl_hs.Caption = "核算数量(计入新能源)";
            this.ne_Sl_hs.FieldName = "Sl_hs";
            this.ne_Sl_hs.Name = "ne_Sl_hs";
            this.ne_Sl_hs.OptionsColumn.ReadOnly = true;
            this.ne_Sl_hs.Visible = true;
            this.ne_Sl_hs.VisibleIndex = 1;
            // 
            // ne_cafc
            // 
            this.ne_cafc.AppearanceCell.Options.UseTextOptions = true;
            this.ne_cafc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ne_cafc.Caption = "CAFC(计入新能源)";
            this.ne_cafc.FieldName = "Cafc";
            this.ne_cafc.Name = "ne_cafc";
            this.ne_cafc.OptionsColumn.ReadOnly = true;
            this.ne_cafc.Visible = true;
            this.ne_cafc.VisibleIndex = 2;
            // 
            // ne_tcafc
            // 
            this.ne_tcafc.AppearanceCell.Options.UseTextOptions = true;
            this.ne_tcafc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.ne_tcafc.Caption = "TCAFC(计入新能源)";
            this.ne_tcafc.FieldName = "Tcafc";
            this.ne_tcafc.Name = "ne_tcafc";
            this.ne_tcafc.OptionsColumn.ReadOnly = true;
            this.ne_tcafc.Visible = true;
            this.ne_tcafc.VisibleIndex = 3;
            // 
            // ed
            // 
            this.ed.Caption = "额度";
            this.ed.FieldName = "ed";
            this.ed.Name = "ed";
            this.ed.OptionsColumn.ReadOnly = true;
            // 
            // TCAFC106
            // 
            this.TCAFC106.Caption = "TCAFC*106%";
            this.TCAFC106.FieldName = "TCAFC106";
            this.TCAFC106.Name = "TCAFC106";
            this.TCAFC106.OptionsColumn.ReadOnly = true;
            // 
            // tcafc109
            // 
            this.tcafc109.Caption = "TCAFC*109%";
            this.tcafc109.FieldName = "TCAFC109";
            this.tcafc109.Name = "tcafc109";
            this.tcafc109.OptionsColumn.ReadOnly = true;
            // 
            // gcTeCafc
            // 
            this.gcTeCafc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcTeCafc.Location = new System.Drawing.Point(0, 0);
            this.gcTeCafc.MainView = this.gvTeCafc;
            this.gcTeCafc.MenuManager = this.ribbon;
            this.gcTeCafc.Name = "gcTeCafc";
            this.gcTeCafc.Size = new System.Drawing.Size(1084, 172);
            this.gcTeCafc.TabIndex = 5;
            this.gcTeCafc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvTeCafc});
            // 
            // gvTeCafc
            // 
            this.gvTeCafc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.te_Qcscqy,
            this.te_Sl_act,
            this.te_Sl_hs,
            this.te_CAFC,
            this.te_TCAFC,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8});
            this.gvTeCafc.GridControl = this.gcTeCafc;
            this.gvTeCafc.Name = "gvTeCafc";
            this.gvTeCafc.OptionsView.ShowGroupPanel = false;
            // 
            // te_Qcscqy
            // 
            this.te_Qcscqy.Caption = "汽车企业名称";
            this.te_Qcscqy.FieldName = "Qcscqy";
            this.te_Qcscqy.Name = "te_Qcscqy";
            this.te_Qcscqy.OptionsColumn.ReadOnly = true;
            // 
            // te_Sl_act
            // 
            this.te_Sl_act.AppearanceCell.Options.UseTextOptions = true;
            this.te_Sl_act.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_Sl_act.Caption = "实际数量(不计新能源)";
            this.te_Sl_act.FieldName = "Sl_act";
            this.te_Sl_act.Name = "te_Sl_act";
            this.te_Sl_act.OptionsColumn.ReadOnly = true;
            this.te_Sl_act.Visible = true;
            this.te_Sl_act.VisibleIndex = 0;
            // 
            // te_Sl_hs
            // 
            this.te_Sl_hs.AppearanceCell.Options.UseTextOptions = true;
            this.te_Sl_hs.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_Sl_hs.Caption = "核算数量(不计新能源)";
            this.te_Sl_hs.FieldName = "Sl_hs";
            this.te_Sl_hs.Name = "te_Sl_hs";
            this.te_Sl_hs.OptionsColumn.ReadOnly = true;
            this.te_Sl_hs.Visible = true;
            this.te_Sl_hs.VisibleIndex = 1;
            // 
            // te_CAFC
            // 
            this.te_CAFC.AppearanceCell.Options.UseTextOptions = true;
            this.te_CAFC.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_CAFC.Caption = "CAFC(不计新能源)";
            this.te_CAFC.FieldName = "Cafc";
            this.te_CAFC.Name = "te_CAFC";
            this.te_CAFC.OptionsColumn.ReadOnly = true;
            this.te_CAFC.Visible = true;
            this.te_CAFC.VisibleIndex = 2;
            // 
            // te_TCAFC
            // 
            this.te_TCAFC.AppearanceCell.Options.UseTextOptions = true;
            this.te_TCAFC.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.te_TCAFC.Caption = "TCAFC(不计新能源)";
            this.te_TCAFC.FieldName = "Tcafc";
            this.te_TCAFC.Name = "te_TCAFC";
            this.te_TCAFC.OptionsColumn.ReadOnly = true;
            this.te_TCAFC.Visible = true;
            this.te_TCAFC.VisibleIndex = 3;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "额度";
            this.gridColumn6.FieldName = "ed";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.ReadOnly = true;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "TCAFC*106%";
            this.gridColumn7.FieldName = "TCAFC106";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.ReadOnly = true;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "TCAFC*109%";
            this.gridColumn8.FieldName = "TCAFC109";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.ReadOnly = true;
            // 
            // CAFCForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 475);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "CAFCForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "油耗数据核算";
            this.Load += new System.EventHandler(this.CAFCForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.xtraScrollableControl1.ResumeLayout(false);
            this.xtraScrollableControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gcNeCafc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvNeCafc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcTeCafc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTeCafc)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraBars.BarButtonItem barButtonItem1;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
        private DevExpress.XtraEditors.DateEdit dtEndTime;
        private DevExpress.XtraEditors.DateEdit dtStartTime;
        private DevExpress.XtraEditors.SimpleButton btnNeSearch;
        private DevExpress.XtraEditors.SimpleButton btnTeSearch;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private DevExpress.XtraGrid.GridControl gcNeCafc;
        private DevExpress.XtraGrid.Views.Grid.GridView gvNeCafc;
        private DevExpress.XtraGrid.Columns.GridColumn ne_qymc;
        private DevExpress.XtraGrid.Columns.GridColumn ne_Sl_act;
        private DevExpress.XtraGrid.Columns.GridColumn ne_Sl_hs;
        private DevExpress.XtraGrid.Columns.GridColumn ne_cafc;
        private DevExpress.XtraGrid.Columns.GridColumn ne_tcafc;
        private DevExpress.XtraGrid.Columns.GridColumn ed;
        private DevExpress.XtraGrid.Columns.GridColumn TCAFC106;
        private DevExpress.XtraGrid.Columns.GridColumn tcafc109;
        private DevExpress.XtraGrid.GridControl gcTeCafc;
        private DevExpress.XtraGrid.Views.Grid.GridView gvTeCafc;
        private DevExpress.XtraGrid.Columns.GridColumn te_Qcscqy;
        private DevExpress.XtraGrid.Columns.GridColumn te_Sl_act;
        private DevExpress.XtraGrid.Columns.GridColumn te_Sl_hs;
        private DevExpress.XtraGrid.Columns.GridColumn te_CAFC;
        private DevExpress.XtraGrid.Columns.GridColumn te_TCAFC;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
    }
}