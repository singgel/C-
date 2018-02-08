using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataModel;
using System.Collections;

namespace FuelDataSysClient.SubForm
{
    public partial class PrintForm : DevExpress.XtraEditors.XtraForm
    {
        public PrintForm()
        {
            InitializeComponent();
            SetLableValues();
        }


        private void btnPrePrint_Click(object sender, EventArgs e)
        {
            this.printPreviewDialog.ShowDialog();
        }

        private void btnSetPrint_Click(object sender, EventArgs e)
        {
            this.pageSetupDialog.ShowDialog(); 
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (this.printDialog.ShowDialog() == DialogResult.OK)
            {
                this.printDocument.Print();
            }
        }

        int page = 0;
        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int count = Utils.printModel.Count;
            if (count > 0) 
            {
                page++;
                e.HasMorePages = true;
                if (page < count + 1)
                {
                    Bitmap _NewBitmap = new Bitmap(this.imageSlider1.Width, imageSlider1.Height);
                    imageSlider1.DrawToBitmap(_NewBitmap, new Rectangle(0, 0, _NewBitmap.Width, _NewBitmap.Height));
                    e.Graphics.DrawImage(_NewBitmap, 120, 150, 600, 800);

                    Font font = new Font("宋体", 12, FontStyle.Regular);

                    e.Graphics.DrawString(Utils.printModel[page - 1].Qcscqy, font, Brushes.Black, 250, 298);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Clxh, font, Brushes.Black, 250, 320);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Fdjxh, font, Brushes.Black, 270, 347);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Pl, font, Brushes.Black, 230, 371);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Bsqlx, font, Brushes.Black, 270, 395);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Zczbzl, font, Brushes.Black, 290, 420);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Qtxx, font, Brushes.Black, 260, 445);

                    e.Graphics.DrawString(Utils.printModel[page - 1].Rllx, font, Brushes.Black, 530, 347);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Edgl, font, Brushes.Black, 530, 371);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Qdxs, font, Brushes.Black, 530, 395);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Zdsjzzl, font, Brushes.Black, 585, 420);

                    e.Graphics.DrawString(Utils.printModel[page - 1].Sq, font, Brushes.Black, 450, 510);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Zh, new Font("宋体", 20, FontStyle.Regular), Brushes.Black, 450, 535);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Sj, font, Brushes.Black, 450, 574);

                    //e.Graphics.DrawString("有效期至：", font, Brushes.Black, 420, 600);//有效期
                    e.Graphics.DrawString(getLineCount(this.textBox2.Text.Trim(), 10), new Font("宋体", 15, FontStyle.Regular), Brushes.Black, 380, 620);
                    e.Graphics.DrawString(getLineCount(this.textBox1.Text.Trim(), 6), new Font("宋体", 18, FontStyle.Regular), Brushes.Black, 170, 200);

                    e.Graphics.DrawString(Utils.printModel[page - 1].Bah, font, Brushes.Black, 230, 872);
                    e.Graphics.DrawString(Utils.printModel[page - 1].Qysj, font, Brushes.Black, 540, 872);

                }
                if (page == count)
                {
                    page = 0;
                    e.HasMorePages = false;
                }
            }
            
        }


        public void SetLableValues() 
        {
            try
            {
                if (Utils.printModel.Count > 1)
                {
                    this.labelControl1.Visible = false;
                    this.labelControl2.Visible = false;
                    this.labelControl3.Visible = false;
                    this.labelControl4.Visible = false;
                    this.labelControl5.Visible = false;
                    this.labelControl6.Visible = false;
                    this.labelControl7.Visible = false;
                    this.labelControl8.Visible = false;
                    this.labelControl9.Visible = false;
                    this.labelControl10.Visible = false;
                    this.labelControl11.Visible = false;
                    this.labelControl12.Visible = false;
                    this.labelControl13.Visible = false;
                    this.labelControl14.Visible = false;
                    this.labelControl15.Visible = false;
                    this.labelControl16.Visible = false;
                }
                else
                {
                    this.labelControl1.Text = Utils.printModel[0].Qcscqy;
                    this.labelControl2.Text = Utils.printModel[0].Clxh;
                    this.labelControl3.Text = Utils.printModel[0].Fdjxh;
                    this.labelControl4.Text = Utils.printModel[0].Pl;
                    this.labelControl5.Text = Utils.printModel[0].Bsqlx;
                    this.labelControl6.Text = Utils.printModel[0].Zczbzl;
                    this.labelControl7.Text = Utils.printModel[0].Qtxx;
                    this.labelControl8.Text = Utils.printModel[0].Rllx;
                    this.labelControl9.Text = Utils.printModel[0].Edgl;
                    this.labelControl10.Text = Utils.printModel[0].Qdxs;
                    this.labelControl11.Text = Utils.printModel[0].Zdsjzzl;
                    this.labelControl12.Text = Utils.printModel[0].Sq;
                    this.labelControl13.Text = Utils.printModel[0].Zh;
                    this.labelControl14.Text = Utils.printModel[0].Sj;
                    this.labelControl15.Text = Utils.printModel[0].Bah;
                    this.labelControl16.Text = Utils.printModel[0].Qysj;
                }
            }
            catch { }
        }

        public string getLineCount(string str, int intLeng) 
        {
            string copy_str = "";
            if (str != null)
            {
                try
                {
                    for (int i = 0; i < str.Length / intLeng; i++)
                    {
                        copy_str += str.Substring(i * intLeng, intLeng) + "\n";
                    }

                }
                catch
                {
                    Clipboard.SetDataObject((object)copy_str, true);
                    copy_str = "";
                }
                copy_str += str.Substring(str.Length / intLeng * intLeng);
            }
            return copy_str;
        }
    }
}