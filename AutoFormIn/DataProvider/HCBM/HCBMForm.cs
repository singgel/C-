using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Assistant.DataProviders.HCBM
{
    public partial class HCBMForm : Form
    {
        private OleDbConnection _conn;
        private List<Product> list;

        public OleDbConnection Connection
        {
            get { return _conn; }
            set
            {
                if (_conn != value)
                {
                    _conn = value;
                    InitProductList();
                    this.dataGridView1.DataSource = list;
                }
            }
        }

        private int _SelectedIndex;

        public Product SelectedProduct;

        public HCBMForm()
        {
            InitializeComponent();
            _SelectedIndex = -1;
            SelectedProduct = null;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.CurrentCellChanged += dataGridView1_CurrentCellChanged;
        }

        void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            if (_SelectedIndex != -1)
            {
                dataGridView1.Rows[_SelectedIndex].Cells[0].Value = false;
                _SelectedIndex = -1;
            }
            int index = dataGridView1.CurrentRow.Index;
            if (index == -1)
                SelectedProduct = null;
            else
            {
                SelectedProduct = list.Count > index ? list[index] : null;
                _SelectedIndex = list.Count > index ? index : -1;
                if(_SelectedIndex != -1)
                    dataGridView1.CurrentRow.Cells[0].Value = true;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Query_Click(object sender, EventArgs e)
        {
            InitProductList();
            dataGridView1.DataSource = list;
        }

        private void InitProductList()
        {
            if (_conn == null)
                throw new ArgumentException("未指定有效的数据源！");
            list = new List<Product>();
            using (OleDbCommand cmd = new OleDbCommand("", _conn))
            {
                if (string.IsNullOrEmpty(FilterBox.Text))
                    cmd.CommandText = @"select cp_id, cpxh, Cpsb, Cpmc from cpsb_chpxx";
                else
                    cmd.CommandText = string.Format(@"select cp_id, cpxh, Cpsb, Cpmc from cpsb_chpxx where cpxh like '%{0}%' or Cpmc like '%{0}%' or Cpsb like '%{0}%'", FilterBox.Text);
                using (OleDbDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product() { Id = reader["cp_id"] as string, Model = reader["cpxh"] as string, Brand = reader["Cpsb"] as string, Name = reader["Cpmc"] as string, ApplicantType = "新产品" });
                    }
                }
            }
        }
    }
}
