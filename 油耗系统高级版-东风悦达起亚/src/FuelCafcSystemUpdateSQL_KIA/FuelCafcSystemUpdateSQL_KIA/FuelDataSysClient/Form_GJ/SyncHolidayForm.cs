using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_GJ
{
    public partial class SyncHolidayForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public SyncHolidayForm()
        {
            InitializeComponent();
            this.RefreshHolidayForm();
        }

        //刷新按钮
        private void btnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.RefreshHolidayForm();
        }

        //查询数据库
        private void RefreshHolidayForm()
        {
            try
            {
                gcHoliday.DataSource = OracleHelper.ExecuteDataSet(OracleHelper.conn, (string)@"SELECT * FROM FC_HOLIDAY WHERE HOL_DAYS IS NOT NULL ORDER BY HOL_DAYS DESC", null).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.gvHoliday.PostEditor();
            this.gvHoliday.RefreshData();
        }

        //通节假日数据
        private void btnSyncHoliday_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utils.SyncHolidays();
            this.RefreshHolidayForm();
        }
    }
}