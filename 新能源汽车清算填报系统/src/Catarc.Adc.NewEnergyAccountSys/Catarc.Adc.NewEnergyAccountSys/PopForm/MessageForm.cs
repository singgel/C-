using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Catarc.Adc.NewEnergyAccountSys.PopForm
{
    public partial class MessageForm : DevExpress.XtraEditors.XtraForm
    {
        public MessageForm(string msg)
        {
            InitializeComponent();
            this.tbMsg.Text = msg;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}