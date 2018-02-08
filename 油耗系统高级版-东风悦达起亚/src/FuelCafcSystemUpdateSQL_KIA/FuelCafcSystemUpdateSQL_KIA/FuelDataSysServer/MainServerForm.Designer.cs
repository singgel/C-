namespace FuelDataSysServer
{
    partial class MainServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainServerForm));
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar2 = new DevExpress.XtraBars.Bar();
            this.barStart = new DevExpress.XtraBars.BarSubItem();
            this.barStop = new DevExpress.XtraBars.BarSubItem();
            this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
            this.barStartUpload = new DevExpress.XtraBars.BarSubItem();
            this.barStopUpload = new DevExpress.XtraBars.BarSubItem();
            this.barStaticItem2 = new DevExpress.XtraBars.BarStaticItem();
            this.barStatistics = new DevExpress.XtraBars.BarSubItem();
            this.barStaticItem3 = new DevExpress.XtraBars.BarStaticItem();
            this.barClear = new DevExpress.XtraBars.BarSubItem();
            this.barSave = new DevExpress.XtraBars.BarSubItem();
            this.barCopyright = new DevExpress.XtraBars.BarSubItem();
            this.bar3 = new DevExpress.XtraBars.Bar();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.userLoginLog = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            this.splitContainerControl1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar2,
            this.bar3});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.barStart,
            this.barStop,
            this.barStaticItem1,
            this.barStartUpload,
            this.barStopUpload,
            this.barStaticItem2,
            this.barClear,
            this.barSave,
            this.barCopyright,
            this.barStatistics,
            this.barStaticItem3});
            this.barManager1.MainMenu = this.bar2;
            this.barManager1.MaxItemId = 18;
            this.barManager1.StatusBar = this.bar3;
            // 
            // bar2
            // 
            this.bar2.BarName = "Main menu";
            this.bar2.DockCol = 0;
            this.bar2.DockRow = 0;
            this.bar2.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar2.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.barStart),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStop),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem1),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStartUpload),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStopUpload, true),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem2),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStatistics),
            new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem3),
            new DevExpress.XtraBars.LinkPersistInfo(this.barClear),
            new DevExpress.XtraBars.LinkPersistInfo(this.barSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.barCopyright)});
            this.bar2.OptionsBar.MultiLine = true;
            this.bar2.OptionsBar.UseWholeRow = true;
            this.bar2.Text = "Main menu";
            // 
            // barStart
            // 
            this.barStart.Caption = "开始接收合并";
            this.barStart.Id = 0;
            this.barStart.Name = "barStart";
            this.barStart.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStart_ItemClick);
            // 
            // barStop
            // 
            this.barStop.Caption = "停止接收合并";
            this.barStop.Id = 1;
            this.barStop.Name = "barStop";
            this.barStop.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStop_ItemClick);
            // 
            // barStaticItem1
            // 
            this.barStaticItem1.Caption = "|";
            this.barStaticItem1.Id = 2;
            this.barStaticItem1.Name = "barStaticItem1";
            this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barStartUpload
            // 
            this.barStartUpload.Caption = "开始上报";
            this.barStartUpload.Id = 11;
            this.barStartUpload.Name = "barStartUpload";
            this.barStartUpload.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.barStartUpload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStartUpload_ItemClick);
            // 
            // barStopUpload
            // 
            this.barStopUpload.Caption = "停止上报";
            this.barStopUpload.Id = 12;
            this.barStopUpload.Name = "barStopUpload";
            this.barStopUpload.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            this.barStopUpload.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStopUpload_ItemClick);
            // 
            // barStaticItem2
            // 
            this.barStaticItem2.Caption = "|";
            this.barStaticItem2.Id = 14;
            this.barStaticItem2.Name = "barStaticItem2";
            this.barStaticItem2.TextAlignment = System.Drawing.StringAlignment.Near;
            this.barStaticItem2.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            // 
            // barStatistics
            // 
            this.barStatistics.Caption = "统计";
            this.barStatistics.Id = 15;
            this.barStatistics.Name = "barStatistics";
            this.barStatistics.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barStatistics_ItemClick);
            // 
            // barStaticItem3
            // 
            this.barStaticItem3.Caption = "|";
            this.barStaticItem3.Id = 17;
            this.barStaticItem3.Name = "barStaticItem3";
            this.barStaticItem3.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // barClear
            // 
            this.barClear.Caption = "清空";
            this.barClear.Id = 3;
            this.barClear.Name = "barClear";
            this.barClear.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barClear_ItemClick);
            // 
            // barSave
            // 
            this.barSave.Caption = "保存日志";
            this.barSave.Id = 4;
            this.barSave.Name = "barSave";
            this.barSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barSave_ItemClick);
            // 
            // barCopyright
            // 
            this.barCopyright.Caption = "版权说明";
            this.barCopyright.Id = 5;
            this.barCopyright.Name = "barCopyright";
            this.barCopyright.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barCopyright_ItemClick);
            // 
            // bar3
            // 
            this.bar3.BarName = "Status bar";
            this.bar3.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
            this.bar3.DockCol = 0;
            this.bar3.DockRow = 0;
            this.bar3.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
            this.bar3.OptionsBar.AllowQuickCustomization = false;
            this.bar3.OptionsBar.DrawDragBorder = false;
            this.bar3.OptionsBar.UseWholeRow = true;
            this.bar3.Text = "Status bar";
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(754, 24);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 359);
            this.barDockControlBottom.Size = new System.Drawing.Size(754, 23);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 24);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 335);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(754, 24);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 335);
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 24);
            this.splitContainerControl1.Name = "splitContainerControl1";
            this.splitContainerControl1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainerControl1.Panel1.Text = "Panel1";
            this.splitContainerControl1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainerControl1.Panel2.Text = "Panel2";
            this.splitContainerControl1.Size = new System.Drawing.Size(754, 335);
            this.splitContainerControl1.SplitterPosition = 168;
            this.splitContainerControl1.TabIndex = 24;
            this.splitContainerControl1.Text = "splitContainerControl1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.userLoginLog);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(754, 168);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "客户端登录日志";
            // 
            // userLoginLog
            // 
            this.userLoginLog.BackColor = System.Drawing.SystemColors.Window;
            this.userLoginLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userLoginLog.FormattingEnabled = true;
            this.userLoginLog.ItemHeight = 14;
            this.userLoginLog.Location = new System.Drawing.Point(3, 18);
            this.userLoginLog.Name = "userLoginLog";
            this.userLoginLog.Size = new System.Drawing.Size(748, 147);
            this.userLoginLog.TabIndex = 11;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtDetails);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(754, 162);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "统计";
            // 
            // txtDetails
            // 
            this.txtDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDetails.Location = new System.Drawing.Point(3, 18);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(748, 141);
            this.txtDetails.TabIndex = 0;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "DateTime.Now.ToString(\"yyyyMMdd hh-mm-ss\") + \".log\"";
            this.saveFileDialog1.InitialDirectory = " Application.StartupPath + \"\\\\Logs\"";
            this.saveFileDialog1.RestoreDirectory = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 60000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MainServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 382);
            this.Controls.Add(this.splitContainerControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "油耗服务端系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainServerForm_FormClosing);
            this.Load += new System.EventHandler(this.MainServerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar2;
        private DevExpress.XtraBars.BarSubItem barStart;
        private DevExpress.XtraBars.BarSubItem barStop;
        private DevExpress.XtraBars.BarStaticItem barStaticItem1;
        private DevExpress.XtraBars.BarSubItem barStartUpload;
        private DevExpress.XtraBars.BarSubItem barStopUpload;
        private DevExpress.XtraBars.BarStaticItem barStaticItem2;
        private DevExpress.XtraBars.BarSubItem barClear;
        private DevExpress.XtraBars.BarSubItem barSave;
        private DevExpress.XtraBars.BarSubItem barCopyright;
        private DevExpress.XtraBars.Bar bar3;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox userLoginLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private DevExpress.XtraBars.BarSubItem barStatistics;
        private DevExpress.XtraBars.BarStaticItem barStaticItem3;
        private System.Windows.Forms.Timer timer1;
    }
}