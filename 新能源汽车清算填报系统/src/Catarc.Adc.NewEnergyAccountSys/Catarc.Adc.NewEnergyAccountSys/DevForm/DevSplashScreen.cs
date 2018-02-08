using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace Catarc.Adc.NewEnergyAccountSys.DevForm
{
    public partial class DevSplashScreen : SplashScreen
    {
        public DevSplashScreen()
        {
            InitializeComponent();
            this.labelControl1.Text = "Copyright © 2012-" + DateTime.Now.Year;
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

        public enum SplashScreenCommand
        {
        }
    }
}