using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Catarc.Adc.NewEnergyAccountSys.Common;
using Catarc.Adc.NewEnergyAccountSys.Properties;

namespace Catarc.Adc.NewEnergyAccountSys.Form_Index
{
    public partial class IndexForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public IndexForm()
        {
            InitializeComponent();
        }

        private void hyperLinkEdit1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Utils.installPath + Settings.Default.Instructions);
        }
    }
}