using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.SubForm
{
    public partial class UpdateUploadForm : DevExpress.XtraEditors.XtraForm
    {
        public UpdateUploadForm()
        {
            InitializeComponent();
            getUploadList();
        }
        private void getUploadList()
        {
            string sqlJbxx = "SELECT QCSCQY,VIN,CLXH FROM FC_CLJBXX WHERE STATUS ='" + 2 + "'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlJbxx, null);
            ds.Tables[0].Columns.Add("check", System.Type.GetType("System.Boolean"));
            this.gridControl1.DataSource = ds.Tables[0];
        }

        private void barUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PreUploadForm puf = new PreUploadForm();
            puf.UploadData(this.gridView1);
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            getUploadList();
        }

        private void barSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectItem(this.gridView1, true);
        }

        private void barClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectItem(this.gridView1, false);
        }

        private void barClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }


    }
}