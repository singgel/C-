namespace Catarc.Adc.NewEnergyAccountSys.Form_Data
{
    partial class SingleInfoForm
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
            this.tc = new DevExpress.XtraTab.XtraTabControl();
            this.tp1 = new DevExpress.XtraTab.XtraTabPage();
            this.paneljiben = new DevExpress.XtraEditors.PanelControl();
            this.tlp_base = new System.Windows.Forms.TableLayoutPanel();
            this.tp2 = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tlp = new System.Windows.Forms.TableLayoutPanel();
            this.sbt_cancle = new DevExpress.XtraEditors.SimpleButton();
            this.sbt_save_add = new DevExpress.XtraEditors.SimpleButton();
            this.sbt_save_exit = new DevExpress.XtraEditors.SimpleButton();
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.bt_view = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.tc)).BeginInit();
            this.tc.SuspendLayout();
            this.tp1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paneljiben)).BeginInit();
            this.paneljiben.SuspendLayout();
            this.tp2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.xtraTabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tc
            // 
            this.tc.Location = new System.Drawing.Point(2, 3);
            this.tc.Name = "tc";
            this.tc.SelectedTabPage = this.tp1;
            this.tc.Size = new System.Drawing.Size(930, 573);
            this.tc.TabIndex = 2;
            this.tc.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tp1,
            this.tp2,
            this.xtraTabPage1});
            this.tc.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tc_SelectedPageChanged);
            // 
            // tp1
            // 
            this.tp1.Controls.Add(this.paneljiben);
            this.tp1.Name = "tp1";
            this.tp1.Size = new System.Drawing.Size(924, 544);
            this.tp1.Text = "车辆基本信息";
            // 
            // paneljiben
            // 
            this.paneljiben.AllowTouchScroll = true;
            this.paneljiben.AutoSize = true;
            this.paneljiben.Controls.Add(this.tlp_base);
            this.paneljiben.Dock = System.Windows.Forms.DockStyle.Fill;
            this.paneljiben.Location = new System.Drawing.Point(0, 0);
            this.paneljiben.Name = "paneljiben";
            this.paneljiben.Size = new System.Drawing.Size(924, 544);
            this.paneljiben.TabIndex = 1;
            // 
            // tlp_base
            // 
            this.tlp_base.AutoSize = true;
            this.tlp_base.ColumnCount = 6;
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tlp_base.Location = new System.Drawing.Point(11, 5);
            this.tlp_base.Name = "tlp_base";
            this.tlp_base.RowCount = 1;
            this.tlp_base.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_base.Size = new System.Drawing.Size(878, 100);
            this.tlp_base.TabIndex = 3;
            // 
            // tp2
            // 
            this.tp2.Controls.Add(this.panelControl3);
            this.tp2.Name = "tp2";
            this.tp2.Size = new System.Drawing.Size(924, 544);
            this.tp2.Text = "车辆配置信息";
            // 
            // panelControl3
            // 
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(0, 0);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(924, 544);
            this.panelControl3.TabIndex = 2;
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.panelControl1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(924, 544);
            this.xtraTabPage1.Text = "车辆运行信息";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.tlp);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(924, 544);
            this.panelControl1.TabIndex = 0;
            // 
            // tlp
            // 
            this.tlp.AutoSize = true;
            this.tlp.ColumnCount = 6;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlp.Location = new System.Drawing.Point(17, 8);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 1;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.Size = new System.Drawing.Size(940, 100);
            this.tlp.TabIndex = 2;
            // 
            // sbt_cancle
            // 
            this.sbt_cancle.Location = new System.Drawing.Point(852, 582);
            this.sbt_cancle.Name = "sbt_cancle";
            this.sbt_cancle.Size = new System.Drawing.Size(75, 23);
            this.sbt_cancle.TabIndex = 265;
            this.sbt_cancle.Text = "退出";
            this.sbt_cancle.Click += new System.EventHandler(this.sbt_cancle_Click);
            // 
            // sbt_save_add
            // 
            this.sbt_save_add.Location = new System.Drawing.Point(635, 582);
            this.sbt_save_add.Name = "sbt_save_add";
            this.sbt_save_add.Size = new System.Drawing.Size(75, 23);
            this.sbt_save_add.TabIndex = 267;
            this.sbt_save_add.Text = "保存并新增";
            this.sbt_save_add.Click += new System.EventHandler(this.sbt_save_add_Click);
            // 
            // sbt_save_exit
            // 
            this.sbt_save_exit.Location = new System.Drawing.Point(745, 582);
            this.sbt_save_exit.Name = "sbt_save_exit";
            this.sbt_save_exit.Size = new System.Drawing.Size(75, 23);
            this.sbt_save_exit.TabIndex = 268;
            this.sbt_save_exit.Text = "保存并退出";
            this.sbt_save_exit.Click += new System.EventHandler(this.sbt_save_exit_Click);
            // 
            // bt_view
            // 
            this.bt_view.Location = new System.Drawing.Point(482, 582);
            this.bt_view.Name = "bt_view";
            this.bt_view.Size = new System.Drawing.Size(117, 23);
            this.bt_view.TabIndex = 269;
            this.bt_view.Text = "引用申报配置信息";
            this.bt_view.Click += new System.EventHandler(this.bt_view_Click);
            // 
            // SingleInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 612);
            this.Controls.Add(this.bt_view);
            this.Controls.Add(this.sbt_save_exit);
            this.Controls.Add(this.sbt_save_add);
            this.Controls.Add(this.sbt_cancle);
            this.Controls.Add(this.tc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SingleInfoForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "单条信息录入";
            ((System.ComponentModel.ISupportInitialize)(this.tc)).EndInit();
            this.tc.ResumeLayout(false);
            this.tp1.ResumeLayout(false);
            this.tp1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paneljiben)).EndInit();
            this.paneljiben.ResumeLayout(false);
            this.paneljiben.PerformLayout();
            this.tp2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.xtraTabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl tc;
        private DevExpress.XtraTab.XtraTabPage tp1;
        private DevExpress.XtraEditors.PanelControl paneljiben;
        private DevExpress.XtraTab.XtraTabPage tp2;
        private DevExpress.XtraEditors.PanelControl panelControl3;
        private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
        private DevExpress.XtraEditors.SimpleButton sbt_cancle;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.TableLayoutPanel tlp;
        private System.Windows.Forms.TableLayoutPanel tlp_base;
        private DevExpress.XtraEditors.SimpleButton sbt_save_add;
        private DevExpress.XtraEditors.SimpleButton sbt_save_exit;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraEditors.SimpleButton bt_view;

    }
}