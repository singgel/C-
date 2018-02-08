using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FuelDataSysClient
{
    public partial class MessageForm : Form
    {
        public MessageForm(string msg)
        {
            InitializeComponent();
            this.tbMsg.Text = msg;
        }

        // 关闭按钮
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
