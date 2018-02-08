using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace FuelDataSysClient
{
    public partial class ProcessForm : DevExpress.XtraEditors.XtraForm
    {
        int _totalMax;

        public int TotalMax
        {
            get { return _totalMax; }
            set { _totalMax = value; }
        }
        public ProcessForm()
        {
            InitializeComponent();
        }

        public void ShowProcessBar()
        {
            //设置一个最小值
            progressBarControl1.Properties.Minimum = 0;
            //设置一个最大值
            progressBarControl1.Properties.Maximum = this.TotalMax;
            //设置步长，即每次增加的数
            progressBarControl1.Properties.Step = 1;
            //设置进度条的样式
            progressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
            progressBarControl1.Properties.StartColor = Color.Green;
            progressBarControl1.Position = 0;
        }
    }
}