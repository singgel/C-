using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.FuelCafc
{
    public partial class IntegrateDataForm : DevExpress.XtraEditors.XtraForm
    {
        InitDataTime initTime = new InitDataTime();

        public IntegrateDataForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }
    }
}