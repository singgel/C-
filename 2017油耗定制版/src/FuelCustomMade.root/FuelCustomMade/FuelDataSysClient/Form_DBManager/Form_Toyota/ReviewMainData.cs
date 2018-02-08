using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_DBManager.Form_Toyota
{
    public partial class ReviewMainData : DevExpress.XtraEditors.XtraForm
    {
        public ReviewMainData()
        {
            InitializeComponent();
            this.ShowImportedCocData();
        }

        protected void ShowImportedCocData()
        {
            try
            {
                DataSet ds = this.GetImportedCocData();
                if (ds != null)
                {
                    DataTable dt = ds.Tables[0];
                    this.gcReviewCocData.DataSource = dt;
                    this.gridView1.BestFitColumns();
                    this.lblCocData.Text = string.Format("主表数据共{0}条", dt.Rows.Count);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 查询已经导入的COC信息
        /// </summary>
        /// <returns></returns>
        protected DataSet GetImportedCocData()
        {
            DataSet ds = null;
            try
            {
                string sql = @"SELECT CI.* FROM MAIN_CTNY CI WHERE 1=1 ";
                string sw = string.Empty;
                if (!string.IsNullOrEmpty(tbCocId.Text.Trim()))
                {
                    sw += string.Format(" AND MAIN_ID LIKE '%{0}%'", tbCocId.Text.Trim());
                }
                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "显示失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ds;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.ShowImportedCocData();
        }
    }
}