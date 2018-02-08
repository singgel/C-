namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
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
            this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
            this.sbt_cancle = new DevExpress.XtraEditors.SimpleButton();
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
            this.tc.Size = new System.Drawing.Size(1085, 668);
            this.tc.TabIndex = 2;
            this.tc.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tp1,
            this.tp2,
            this.xtraTabPage1});
            // 
            // tp1
            // 
            this.tp1.Controls.Add(this.paneljiben);
            this.tp1.Name = "tp1";
            this.tp1.Size = new System.Drawing.Size(1079, 639);
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
            this.paneljiben.Size = new System.Drawing.Size(1079, 639);
            this.paneljiben.TabIndex = 1;
            // 
            // tlp_base
            // 
            this.tlp_base.AutoSize = true;
            this.tlp_base.ColumnCount = 6;
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 257F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 257F));
            this.tlp_base.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 149F));
            this.tlp_base.Location = new System.Drawing.Point(13, 6);
            this.tlp_base.Name = "tlp_base";
            this.tlp_base.RowCount = 1;
            this.tlp_base.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_base.Size = new System.Drawing.Size(1025, 117);
            this.tlp_base.TabIndex = 3;
            // 
            // tp2
            // 
            this.tp2.Controls.Add(this.panelControl3);
            this.tp2.Name = "tp2";
            this.tp2.Size = new System.Drawing.Size(1079, 639);
            this.tp2.Text = "车辆配置信息";
            // 
            // panelControl3
            // 
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(0, 0);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(1079, 639);
            this.panelControl3.TabIndex = 2;
            // 
            // xtraTabPage1
            // 
            this.xtraTabPage1.Controls.Add(this.panelControl1);
            this.xtraTabPage1.Name = "xtraTabPage1";
            this.xtraTabPage1.Size = new System.Drawing.Size(1079, 639);
            this.xtraTabPage1.Text = "车辆运行信息";
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.tlp);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1079, 639);
            this.panelControl1.TabIndex = 0;
            // 
            // tlp
            // 
            this.tlp.AutoSize = true;
            this.tlp.ColumnCount = 6;
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 105F));
            this.tlp.Location = new System.Drawing.Point(20, 9);
            this.tlp.Name = "tlp";
            this.tlp.RowCount = 1;
            this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp.Size = new System.Drawing.Size(1097, 117);
            this.tlp.TabIndex = 2;
            // 
            // sbt_cancle
            // 
            this.sbt_cancle.Location = new System.Drawing.Point(507, 679);
            this.sbt_cancle.Name = "sbt_cancle";
            this.sbt_cancle.Size = new System.Drawing.Size(87, 27);
            this.sbt_cancle.TabIndex = 265;
            this.sbt_cancle.Text = "关闭";
            this.sbt_cancle.Click += new System.EventHandler(this.sbt_cancle_Click);
            // 
            // SingleInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1101, 714);
            this.Controls.Add(this.sbt_cancle);
            this.Controls.Add(this.tc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SingleInfoForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "单条信息录入";
            this.Load += new System.EventHandler(this.SingleInfoForm_Load);
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
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.TableLayoutPanel tlp;
        private System.Windows.Forms.TableLayoutPanel tlp_base;
        private DevExpress.Utils.ToolTipController toolTipController1;
        private DevExpress.XtraEditors.SimpleButton sbt_cancle;

    }
}