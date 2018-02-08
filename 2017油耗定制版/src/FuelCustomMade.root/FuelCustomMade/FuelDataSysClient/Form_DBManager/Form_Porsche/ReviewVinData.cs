using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_DBManager.Form_Porsche
{
    public partial class ReviewVinData : DevExpress.XtraEditors.XtraForm
    {
        public ReviewVinData()
        {
            InitializeComponent();
            this.ShowImportedVinData();
        }

        protected void ShowImportedVinData()
        {
            try
            {
                DataSet ds = this.GetImportedVinData();
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    this.gcReviewVinData.DataSource = dt;
                    this.gridView1.BestFitColumns();
                    this.lblCocData.Text = string.Format("VIN数据共{0}条", dt.Rows.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "显示失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查询已经导入但未生成油耗数据的VIN信息
        /// </summary>
        /// <returns></returns>
        protected DataSet GetImportedVinData()
        {
            DataSet ds = null;
            try
            {
                string sql = @"SELECT VI.* FROM VIN_INFO VI WHERE STATUS='1'";

                string sw = string.Empty;
                if (!string.IsNullOrEmpty(tbVin.Text.Trim()))
                {
                    sw += string.Format(" AND VIN LIKE '%{0}%'", tbVin.Text.Trim());
                }

                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.ShowImportedVinData();
        }
    }
}