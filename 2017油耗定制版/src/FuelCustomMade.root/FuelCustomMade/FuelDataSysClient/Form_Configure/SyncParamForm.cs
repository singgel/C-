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

namespace FuelDataSysClient.Form_Configure
{
    public partial class SyncParamForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;

        public SyncParamForm()
        {
            InitializeComponent();
            this.RefreshParamForm();
        }

        private void btnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.RefreshParamForm();
        }

        private void btnSync_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utils.SyncParam();
            this.RefreshParamForm();
        }

        private void RefreshParamForm()
        {
            DataSet ds;
            DataTable dt=new DataTable();
            string sql = @"SELECT * FROM RLLX_PARAM";
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
            gcParam.DataSource = dt;

            this.gvParam.PostEditor();
            this.gvParam.RefreshData();
        }
    }
}