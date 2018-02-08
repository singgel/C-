using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool.Tool_Toyota;

namespace FuelDataSysClient.Form_DBManager.Form_Toyota
{
    public partial class PreviewForm : DevExpress.XtraEditors.XtraForm
    {
        private DataSet ds = new DataSet();
        public PreviewForm()
        {
            InitializeComponent();
        }

        public PreviewForm(DataSet ds)
        {
            InitializeComponent();
            this.ShowReadyData(ds);
            this.ds = ds;
        }

        protected void ShowReadyData(DataSet ds)
        {
            try
            {
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    this.dgvPreview.DataSource = dt;
                    this.gridView1.BestFitColumns();
                    this.lblReadyData.Text = string.Format("待生成数据共{0}条", dt.Rows.Count);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnGenFuelData_Click(object sender, EventArgs e)
        {
            try
            {
                ToyotaUtils toyotaUtils = new ToyotaUtils();
                string msg = toyotaUtils.SaveParam(ds);
                if (string.IsNullOrEmpty(msg))
                {
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("生成成功", "生成成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageForm mf = new MessageForm("以下数据已经存在\r\n" + msg);
                    Utils.SetFormMid(mf);
                    mf.Text = "生成结果";
                    mf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("燃料消耗量数据生成失败：" + ex.Message, "生成失败");
            }
        }
    }
}