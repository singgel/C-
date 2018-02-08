using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Catarc.Adc.NewEnergyAccountSys.DevForm
{
    public partial class waitForm : Form
    {
        public waitForm(string strWait)
        {
            InitializeComponent();
            this.TopMost = true;
            SetText(strWait);
        }
        private delegate void SetTextHandler(string text);
        public void SetText(string text)
        {
            if (this.label1.InvokeRequired)
            {
                this.Invoke(new SetTextHandler(SetText), text);
            }
            else
            {
                if (text == "close")
                    this.Close();
                else
                    this.label1.Text = text;
                // 下载于www.51aspx.com
                //this.label1.Refresh();
            }
        }
    }
}
