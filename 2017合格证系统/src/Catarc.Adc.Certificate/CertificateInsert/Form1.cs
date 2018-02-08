using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CertificateInsert
{
    public partial class Form1 : Form
    {
        Helper helper = new Helper();
        public Form1()
        {
            InitializeComponent();
        }
        //开始
        private void button1_Click(object sender, EventArgs e)
        {
            string id =textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(id))
                helper.Start(int.Parse(id));
            else
                MessageBox.Show("H_ID不能为空。");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            helper.Stop();
        }
    }
}
