using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataModel;
using System.Data.OleDb;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient
{
    public partial class SyncHolidayForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SyncHolidayForm()
        {
            InitializeComponent();
            this.RefreshHolidayForm();
        }

        private void btnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.RefreshHolidayForm();
        }

        private void RefreshHolidayForm()
        {
            DataSet ds;
            DataTable dt=new DataTable();
            string sql = @"SELECT * FROM FC_HOLIDAY";
            try
            {
                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            gcHoliday.DataSource = dt;

            this.gvHoliday.PostEditor();
            this.gvHoliday.RefreshData();
        }

        private void btnSyncHoliday_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utils.SyncHolidays();
            this.RefreshHolidayForm();
        }
    }
}