namespace FuelDataSysClient
{
    partial class ReviewVinData
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
            DevExpress.XtraGrid.GridLevelNode gridLevelNode1 = new DevExpress.XtraGrid.GridLevelNode();
            this.gcReviewVinData = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.VIN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLXH = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CLZZRQ = new DevExpress.XtraGrid.Columns.GridColumn();
            this.CREATETIME = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.tbVin = new DevExpress.XtraEditors.TextEdit();
            this.lblVin = new DevExpress.XtraEditors.LabelControl();
            this.lblCocData = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gcReviewVinData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVin.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // gcReviewVinData
            // 
            this.gcReviewVinData.Dock = System.Windows.Forms.DockStyle.Fill;
            gridLevelNode1.RelationName = "Level1";
            this.gcReviewVinData.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[] {
            gridLevelNode1});
            this.gcReviewVinData.Location = new System.Drawing.Point(0, 0);
            this.gcReviewVinData.MainView = this.gridView1;
            this.gcReviewVinData.Name = "gcReviewVinData";
            this.gcReviewVinData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1,
            this.repositoryItemSpinEdit1,
            this.repositoryItemCheckEdit2});
            this.gcReviewVinData.Size = new System.Drawing.Size(924, 462);
            this.gcReviewVinData.TabIndex = 10;
            this.gcReviewVinData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.VIN,
            this.CLXH,
            this.CLZZRQ,
            this.CREATETIME});
            this.gridView1.GridControl = this.gcReviewVinData;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsMenu.EnableColumnMenu = false;
            this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            // 
            // VIN
            // 
            this.VIN.Caption = "VIN";
            this.VIN.FieldName = "VIN";
            this.VIN.Name = "VIN";
            this.VIN.OptionsColumn.AllowEdit = false;
            this.VIN.Visible = true;
            this.VIN.VisibleIndex = 0;
            // 
            // CLXH
            // 
            this.CLXH.Caption = "产品型号";
            this.CLXH.FieldName = "CLXH";
            this.CLXH.MinWidth = 30;
            this.CLXH.Name = "CLXH";
            this.CLXH.OptionsColumn.AllowEdit = false;
            this.CLXH.Visible = true;
            this.CLXH.VisibleIndex = 1;
            this.CLXH.Width = 50;
            // 
            // CLZZRQ
            // 
            this.CLZZRQ.Caption = "进口日期";
            this.CLZZRQ.FieldName = "CLZZRQ";
            this.CLZZRQ.Name = "CLZZRQ";
            this.CLZZRQ.OptionsColumn.AllowEdit = false;
            this.CLZZRQ.Visible = true;
            this.CLZZRQ.VisibleIndex = 2;
            this.CLZZRQ.Width = 50;
            // 
            // CREATETIME
            // 
            this.CREATETIME.Caption = "导入时间";
            this.CREATETIME.FieldName = "CREATETIME";
            this.CREATETIME.Name = "CREATETIME";
            this.CREATETIME.OptionsColumn.AllowEdit = false;
            this.CREATETIME.Visible = true;
            this.CREATETIME.VisibleIndex = 3;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // repositoryItemSpinEdit1
            // 
            this.repositoryItemSpinEdit1.AutoHeight = false;
            this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.btnSearch);
            this.panelControl1.Controls.Add(this.tbVin);
            this.panelControl1.Controls.Add(this.lblVin);
            this.panelControl1.Controls.Add(this.lblCocData);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(924, 35);
            this.panelControl1.TabIndex = 11;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(352, 9);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(130, 20);
            this.btnSearch.TabIndex = 61;
            this.btnSearch.Text = "查询";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // tbVin
            // 
            this.tbVin.Location = new System.Drawing.Point(200, 9);
            this.tbVin.Name = "tbVin";
            this.tbVin.Size = new System.Drawing.Size(130, 20);
            this.tbVin.TabIndex = 60;
            // 
            // lblVin
            // 
            this.lblVin.Location = new System.Drawing.Point(129, 12);
            this.lblVin.Name = "lblVin";
            this.lblVin.Size = new System.Drawing.Size(66, 14);
            this.lblVin.TabIndex = 59;
            this.lblVin.Text = "备案号(VIN)";
            // 
            // lblCocData
            // 
            this.lblCocData.Location = new System.Drawing.Point(5, 12);
            this.lblCocData.Name = "lblCocData";
            this.lblCocData.Size = new System.Drawing.Size(0, 14);
            this.lblCocData.TabIndex = 1;
            // 
            // ReviewVinData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 462);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.gcReviewVinData);
            this.Name = "ReviewVinData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VIN信息";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.gcReviewVinData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemSpinEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbVin.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gcReviewVinData;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn CLXH;
        private DevExpress.XtraGrid.Columns.GridColumn CLZZRQ;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repositoryItemSpinEdit1;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl lblCocData;
        private DevExpress.XtraGrid.Columns.GridColumn VIN;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit tbVin;
        private DevExpress.XtraEditors.LabelControl lblVin;
        private DevExpress.XtraGrid.Columns.GridColumn CREATETIME;

    }
}