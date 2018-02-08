using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Catarc.Adc.NewEnergyApproveSys.PopForm
{
    public partial class BigPictureForm : DevExpress.XtraEditors.XtraForm
    {
        public BigPictureForm(Image image)
        {
            InitializeComponent();
            this.pictureEdit1.Image = image;
            //this.xtraScrollableControl1.Width = image.Width;
            //this.xtraScrollableControl1.Height = image.Height;
        }
    }
}