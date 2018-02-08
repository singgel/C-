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
    public partial class SyncParamForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;

        public SyncParamForm()
        {
            InitializeComponent();
            this.RefreshParamForm();
        }

        //刷新按钮
        private void btnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.RefreshParamForm();
        }

        //远程燃料参数同步
        private void btnSync_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utils.SyncParam();
            this.RefreshParamForm();
        }

        //查询数据库
        private void RefreshParamForm()
        {
            try
            {
                gcParam.DataSource = OracleHelper.ExecuteDataSet(OracleHelper.conn, (string)@"SELECT * FROM RLLX_PARAM", null).Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.gvParam.PostEditor();
            this.gvParam.RefreshData();
        }
    }
}