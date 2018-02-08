namespace FuelDataSysClient.FuelCafc
{
    partial class AddExistDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddExistDataForm));
            this.ribbon = new DevExpress.XtraBars.Ribbon.RibbonControl();
            this.barBtnSelectAll = new DevExpress.XtraBars.BarButtonItem();
            this.barBtnClearAll = new DevExpress.XtraBars.BarButtonItem();
            this.btnAddExistData = new DevExpress.XtraBars.BarButtonItem();
            this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
            this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
            this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
            this.gcParam = new DevExpress.XtraGrid.GridControl();
            this.gvParam = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.checkData = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rceData = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.clscqy = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clxh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.rlzl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bsqxs = new DevExpress.XtraGrid.Columns.GridColumn();
            this.zczbzl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.zwps = new DevExpress.XtraGrid.Columns.GridColumn();
            this.Zhgkxslc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.zhgkrlxhlsjz = new DevExpress.XtraGrid.Columns.GridColumn();
            this.sjjkl = new DevExpress.XtraGrid.Columns.GridColumn();
            this.bz = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cxrlxhlmbz = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.lblNum = new DevExpress.XtraEditors.LabelControl();
            this.dtEndTime = new DevExpress.XtraEditors.DateEdit();
            this.dtStartTime = new DevExpress.XtraEditors.DateEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtrllx = new DevExpress.XtraEditors.TextEdit();
            this.txtclxh = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcParam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvParam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rceData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtrllx.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtclxh.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // ribbon
            // 
            this.ribbon.ExpandCollapseItem.Id = 0;
            this.ribbon.ExpandCollapseItem.Name = "";
            this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.ribbon.ExpandCollapseItem,
            this.barBtnSelectAll,
            this.barBtnClearAll,
            this.btnAddExistData});
            this.ribbon.Location = new System.Drawing.Point(0, 0);
            this.ribbon.MaxItemId = 5;
            this.ribbon.Name = "ribbon";
            this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            this.ribbonPage1});
            this.ribbon.Size = new System.Drawing.Size(932, 147);
            this.ribbon.StatusBar = this.ribbonStatusBar;
            // 
            // barBtnSelectAll
            // 
            this.barBtnSelectAll.Caption = "全选";
            this.barBtnSelectAll.Id = 1;
            this.barBtnSelectAll.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barBtnSelectAll.LargeGlyph")));
            this.barBtnSelectAll.Name = "barBtnSelectAll";
            this.barBtnSelectAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnSelectAll_ItemClick);
            // 
            // barBtnClearAll
            // 
            this.barBtnClearAll.Caption = "取消全选";
            this.barBtnClearAll.Id = 2;
            this.barBtnClearAll.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("barBtnClearAll.LargeGlyph")));
            this.barBtnClearAll.Name = "barBtnClearAll";
            this.barBtnClearAll.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barBtnClearAll_ItemClick);
            // 
            // btnAddExistData
            // 
            this.btnAddExistData.Caption = "添加为预测数据";
            this.btnAddExistData.Id = 3;
            this.btnAddExistData.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("btnAddExistData.LargeGlyph")));
            this.btnAddExistData.Name = "btnAddExistData";
            this.btnAddExistData.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.btnAddExistData_ItemClick);
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
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnSelectAll);
            this.ribbonPageGroup1.ItemLinks.Add(this.barBtnClearAll);
            this.ribbonPageGroup1.Name = "ribbonPageGroup1";
            this.ribbonPageGroup1.Text = "通用";
            // 
            // ribbonPageGroup2
            // 
            this.ribbonPageGroup2.ItemLinks.Add(this.btnAddExistData);
            this.ribbonPageGroup2.Name = "ribbonPageGroup2";
            this.ribbonPageGroup2.Text = "数据";
            // 
            // ribbonStatusBar
            // 
            this.ribbonStatusBar.Location = new System.Drawing.Point(0, 418);
            this.ribbonStatusBar.Name = "ribbonStatusBar";
            this.ribbonStatusBar.Ribbon = this.ribbon;
            this.ribbonStatusBar.Size = new System.Drawing.Size(932, 31);
            // 
            // gcParam
            // 
            this.gcParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gcParam.Location = new System.Drawing.Point(0, 252);
            this.gcParam.MainView = this.gvParam;
            this.gcParam.Name = "gcParam";
            this.gcParam.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.rceData});
            this.gcParam.Size = new System.Drawing.Size(932, 166);
            this.gcParam.TabIndex = 24;
            this.gcParam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvParam});
            // 
            // gvParam
            // 
            this.gvParam.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.checkData,
            this.clscqy,
            this.clxh,
            this.rlzl,
            this.bsqxs,
            this.zczbzl,
            this.zwps,
            this.Zhgkxslc,
            this.zhgkrlxhlsjz,
            this.sjjkl,
            this.bz,
            this.cxrlxhlmbz});
            this.gvParam.GridControl = this.gcParam;
            this.gvParam.Name = "gvParam";
            // 
            // checkData
            // 
            this.checkData.Caption = "选择";
            this.checkData.ColumnEdit = this.rceData;
            this.checkData.FieldName = "Check";
            this.checkData.MaxWidth = 50;
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
            // clscqy
            // 
            this.clscqy.Caption = "企业名称";
            this.clscqy.FieldName = "Qcscqy";
            this.clscqy.MaxWidth = 210;
            this.clscqy.Name = "clscqy";
            this.clscqy.Visible = true;
            this.clscqy.VisibleIndex = 1;
            this.clscqy.Width = 210;
            // 
            // clxh
            // 
            this.clxh.Caption = "产品型号";
            this.clxh.FieldName = "Clxh";
            this.clxh.Name = "clxh";
            this.clxh.Visible = true;
            this.clxh.VisibleIndex = 2;
            this.clxh.Width = 139;
            // 
            // rlzl
            // 
            this.rlzl.Caption = "燃料种类";
            this.rlzl.FieldName = "Rllx";
            this.rlzl.MaxWidth = 110;
            this.rlzl.Name = "rlzl";
            this.rlzl.Visible = true;
            this.rlzl.VisibleIndex = 3;
            this.rlzl.Width = 110;
            // 
            // bsqxs
            // 
            this.bsqxs.Caption = "变速器型式";
            this.bsqxs.FieldName = "Bsqxs";
            this.bsqxs.MaxWidth = 100;
            this.bsqxs.Name = "bsqxs";
            this.bsqxs.Visible = true;
            this.bsqxs.VisibleIndex = 4;
            this.bsqxs.Width = 100;
            // 
            // zczbzl
            // 
            this.zczbzl.AppearanceCell.Options.UseTextOptions = true;
            this.zczbzl.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.zczbzl.Caption = "整备质量";
            this.zczbzl.FieldName = "Zczbzl";
            this.zczbzl.MaxWidth = 100;
            this.zczbzl.Name = "zczbzl";
            this.zczbzl.Visible = true;
            this.zczbzl.VisibleIndex = 5;
            this.zczbzl.Width = 100;
            // 
            // zwps
            // 
            this.zwps.AppearanceCell.Options.UseTextOptions = true;
            this.zwps.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.zwps.Caption = "座椅排数";
            this.zwps.FieldName = "Zwps";
            this.zwps.MaxWidth = 100;
            this.zwps.Name = "zwps";
            this.zwps.Visible = true;
            this.zwps.VisibleIndex = 6;
            this.zwps.Width = 100;
            // 
            // Zhgkxslc
            // 
            this.Zhgkxslc.AppearanceCell.Options.UseTextOptions = true;
            this.Zhgkxslc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.Zhgkxslc.Caption = "纯电动驱动模式综合工况续驶里程";
            this.Zhgkxslc.FieldName = "Zhgkxslc";
            this.Zhgkxslc.MaxWidth = 100;
            this.Zhgkxslc.Name = "Zhgkxslc";
            this.Zhgkxslc.Visible = true;
            this.Zhgkxslc.VisibleIndex = 7;
            this.Zhgkxslc.Width = 100;
            // 
            // zhgkrlxhlsjz
            // 
            this.zhgkrlxhlsjz.AppearanceCell.Options.UseTextOptions = true;
            this.zhgkrlxhlsjz.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.zhgkrlxhlsjz.Caption = "燃料消耗量（综合）";
            this.zhgkrlxhlsjz.FieldName = "ActZhgkrlxhl";
            this.zhgkrlxhlsjz.MaxWidth = 100;
            this.zhgkrlxhlsjz.Name = "zhgkrlxhlsjz";
            this.zhgkrlxhlsjz.Visible = true;
            this.zhgkrlxhlsjz.VisibleIndex = 8;
            this.zhgkrlxhlsjz.Width = 100;
            // 
            // sjjkl
            // 
            this.sjjkl.AppearanceCell.Options.UseTextOptions = true;
            this.sjjkl.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.sjjkl.Caption = "预计进口量";
            this.sjjkl.FieldName = "Sl_act";
            this.sjjkl.MaxWidth = 100;
            this.sjjkl.Name = "sjjkl";
            this.sjjkl.Visible = true;
            this.sjjkl.VisibleIndex = 9;
            this.sjjkl.Width = 100;
            // 
            // bz
            // 
            this.bz.Caption = "备注";
            this.bz.FieldName = "bz";
            this.bz.Name = "bz";
            // 
            // cxrlxhlmbz
            // 
            this.cxrlxhlmbz.Caption = "车型燃料消耗量目标值";
            this.cxrlxhlmbz.FieldName = "ZHGKRLXHL_TGT";
            this.cxrlxhlmbz.Name = "cxrlxhlmbz";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.lblNum);
            this.panelControl1.Controls.Add(this.dtEndTime);
            this.panelControl1.Controls.Add(this.dtStartTime);
            this.panelControl1.Controls.Add(this.btnSearch);
            this.panelControl1.Controls.Add(this.txtrllx);
            this.panelControl1.Controls.Add(this.txtclxh);
            this.panelControl1.Controls.Add(this.labelControl3);
            this.panelControl1.Controls.Add(this.labelControl6);
            this.panelControl1.Controls.Add(this.labelControl5);
            this.panelControl1.Controls.Add(this.labelControl4);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 147);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(932, 105);
            this.panelControl1.TabIndex = 23;
            // 
            // lblNum
            // 
            this.lblNum.Location = new System.Drawing.Point(22, 79);
            this.lblNum.Name = "lblNum";
            this.lblNum.Size = new System.Drawing.Size(32, 14);
            this.lblNum.TabIndex = 52;
            this.lblNum.Text = "共  条";
            // 
            // dtEndTime
            // 
            this.dtEndTime.EditValue = null;
            this.dtEndTime.Location = new System.Drawing.Point(407, 48);
            this.dtEndTime.Name = "dtEndTime";
            this.dtEndTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtEndTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtEndTime.Size = new System.Drawing.Size(165, 20);
            this.dtEndTime.TabIndex = 51;
            // 
            // dtStartTime
            // 
            this.dtStartTime.EditValue = null;
            this.dtStartTime.Location = new System.Drawing.Point(132, 47);
            this.dtStartTime.Name = "dtStartTime";
            this.dtStartTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtStartTime.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtStartTime.Size = new System.Drawing.Size(120, 20);
            this.dtStartTime.TabIndex = 50;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(606, 49);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 20);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "查询(&S)";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtrllx
            // 
            this.txtrllx.Location = new System.Drawing.Point(407, 20);
            this.txtrllx.Name = "txtrllx";
            this.txtrllx.Size = new System.Drawing.Size(165, 20);
            this.txtrllx.TabIndex = 1;
            // 
            // txtclxh
            // 
            this.txtclxh.Location = new System.Drawing.Point(87, 20);
            this.txtclxh.Name = "txtclxh";
            this.txtclxh.Size = new System.Drawing.Size(165, 20);
            this.txtclxh.TabIndex = 1;
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(295, 22);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(48, 14);
            this.labelControl3.TabIndex = 0;
            this.labelControl3.Text = "燃料种类";
            // 
            // labelControl6
            // 
            this.labelControl6.Location = new System.Drawing.Point(295, 50);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(12, 14);
            this.labelControl6.TabIndex = 0;
            this.labelControl6.Text = "至";
            // 
            // labelControl5
            // 
            this.labelControl5.Location = new System.Drawing.Point(22, 50);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(77, 14);
            this.labelControl5.TabIndex = 0;
            this.labelControl5.Text = "车辆制造日期/进口核销日期";
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(22, 23);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(48, 14);
            this.labelControl4.TabIndex = 0;
            this.labelControl4.Text = "产品型号";
            // 
            // AddExistDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 449);
            this.Controls.Add(this.gcParam);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.ribbonStatusBar);
            this.Controls.Add(this.ribbon);
            this.Name = "AddExistDataForm";
            this.Ribbon = this.ribbon;
            this.StatusBar = this.ribbonStatusBar;
            this.Text = "AddForecastDataForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.ribbon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcParam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvParam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rceData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtEndTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties.VistaTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtStartTime.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtrllx.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtclxh.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.Ribbon.RibbonControl ribbon;
        private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
        private DevExpress.XtraBars.Ribbon.RibbonStatusBar ribbonStatusBar;
        private DevExpress.XtraGrid.GridControl gcParam;
        private DevExpress.XtraGrid.Views.Grid.GridView gvParam;
        private DevExpress.XtraGrid.Columns.GridColumn checkData;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit rceData;
        private DevExpress.XtraGrid.Columns.GridColumn clscqy;
        private DevExpress.XtraGrid.Columns.GridColumn clxh;
        private DevExpress.XtraGrid.Columns.GridColumn rlzl;
        private DevExpress.XtraGrid.Columns.GridColumn bsqxs;
        private DevExpress.XtraGrid.Columns.GridColumn zczbzl;
        private DevExpress.XtraGrid.Columns.GridColumn zwps;
        private DevExpress.XtraGrid.Columns.GridColumn Zhgkxslc;
        private DevExpress.XtraGrid.Columns.GridColumn zhgkrlxhlsjz;
        private DevExpress.XtraGrid.Columns.GridColumn sjjkl;
        private DevExpress.XtraGrid.Columns.GridColumn bz;
        private DevExpress.XtraGrid.Columns.GridColumn cxrlxhlmbz;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblNum;
        private DevExpress.XtraEditors.DateEdit dtEndTime;
        private DevExpress.XtraEditors.DateEdit dtStartTime;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtrllx;
        private DevExpress.XtraEditors.TextEdit txtclxh;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraBars.BarButtonItem barBtnSelectAll;
        private DevExpress.XtraBars.BarButtonItem barBtnClearAll;
        private DevExpress.XtraBars.BarButtonItem btnAddExistData;
        private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
    }
}