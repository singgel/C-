using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace FuelDataSysClient.SubForm
{
    public partial class ReasonForm : Form
    {
        public ReasonForm()
        {
            InitializeComponent();
        }

        private string reason;

        public string Reason
        {
            get { return reason; }
            set { reason = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(meReason.Text.Trim()))
            {
                MessageBox.Show("请输入原因");
                return;
            }

            this.Reason = this.meReason.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}