using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Assistant.Entity;

namespace Assistant
{
    public partial class ChooseSample : System.Windows.Forms.Form
    {
        public ChooseSample()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.checkedListBox1.CheckOnClick = true;

            if (ParamsCollection.carParams.Count == 0)
            {
                MessageBox.Show("未找到配置数据");
                return;
            }
            foreach (CarParams c in ParamsCollection.carParams)
            {
                this.checkedListBox1.Items.Add(c.packageCode);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ParamsCollection.selectedPackageCodes.Clear();
            foreach (String s in this.checkedListBox1.CheckedItems)
            {
                ParamsCollection.selectedPackageCodes.Add(s);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
        //全选按钮
        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                this.checkedListBox1.SetItemChecked(i, true);
            }
        }
        //反选按钮
        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    checkedListBox1.SetItemChecked(i, false);
                }
                else
                {
                    checkedListBox1.SetItemChecked(i, true);
                }
            }
        }
    }
}
