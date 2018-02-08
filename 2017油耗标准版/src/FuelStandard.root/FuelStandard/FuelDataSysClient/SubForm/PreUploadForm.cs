using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Collections;
using FuelDataModel;
using DevExpress.XtraGrid.Views.Grid;
using FuelDataSysClient.Tool;


namespace FuelDataSysClient.SubForm
{
    public partial class PreUploadForm : DevExpress.XtraEditors.XtraForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        public PreUploadForm()
        {
            InitializeComponent();
            getUploadList();
        }
        private void getUploadList()
        {
            string sqlJbxx = "SELECT QCSCQY,VIN,CLXH FROM FC_CLJBXX WHERE STATUS ='" + 1 + "'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlJbxx, null);
            ds.Tables[0].Columns.Add("check", System.Type.GetType("System.Boolean"));
            this.gridControl1.DataSource = ds.Tables[0];
            //this.gridControl1.
        }

        private void barSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectItem(this.gridView1, true);
           // SelectItem(true);
        }

        private void barClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectItem(this.gridView1, false);
            //SelectItem(false);
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            getUploadList();
        }

        private void barUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            UploadData(this.gridView1);
        }

        public void UploadData(object objCon) 
        {
            GridView dgv = objCon as GridView;
            try
            {
                // 取list列表选中的VIN
                ArrayList al = new ArrayList();
                dgv.PostEditor();
                string value = "";
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    value = dgv.GetDataRow(i)["check"].ToString();
                    if (value == "True")
                    {
                        al.Add(dgv.GetRowCellValue(i, "VIN"));
                    }
                }

                List<VehicleBasicInfo> lbiList = new List<VehicleBasicInfo>();
                if (al.Count > 0)
                {
                    foreach (string strVin in al)
                    {
                        #region  基本信息
                        VehicleBasicInfo lbi = new VehicleBasicInfo();
                        string strJbSql = "SELECT * FROM FC_CLJBXX WHERE VIN = '" + strVin + "'";
                        //string stringSql = "SELECT * FROM FC_CLJBXX A JOIN RLLX_PARAM_ENTITY B ON A.V_ID = B.V_ID  WHERE A.VIN = '" + strVin + "'";
                        Object obj = AccessHelper.ExecuteDataSet(AccessHelper.conn, strJbSql, null);
                        DataSet ds = (DataSet)obj;
                        lbi.User_Id = Utils.userId;
                        lbi.Qcscqy = ds.Tables[0].Rows[0]["QCSCQY"].ToString();
                        lbi.Jkqczjxs = ds.Tables[0].Rows[0]["JKQCZJXS"].ToString();
                        lbi.Vin = ds.Tables[0].Rows[0]["VIN"].ToString();
                        lbi.Clxh = ds.Tables[0].Rows[0]["CLXH"].ToString();
                        lbi.Clzl = ds.Tables[0].Rows[0]["CLZL"].ToString();
                        lbi.Rllx = ds.Tables[0].Rows[0]["RLLX"].ToString();
                        lbi.Zczbzl = ds.Tables[0].Rows[0]["ZCZBZL"].ToString();
                        lbi.Zgcs = ds.Tables[0].Rows[0]["ZGCS"].ToString();
                        lbi.Ltgg = ds.Tables[0].Rows[0]["LTGG"].ToString();
                        lbi.Zj = ds.Tables[0].Rows[0]["ZJ"].ToString();
                        lbi.Clzzrq = DateTime.Parse(ds.Tables[0].Rows[0]["CLZZRQ"].ToString());
                        lbi.Tymc = ds.Tables[0].Rows[0]["TYMC"].ToString();
                        lbi.Yyc = ds.Tables[0].Rows[0]["YYC"].ToString();
                        lbi.Zwps = ds.Tables[0].Rows[0]["ZWPS"].ToString();
                        lbi.Zdsjzzl = ds.Tables[0].Rows[0]["ZDSJZZL"].ToString();
                        lbi.Edzk = ds.Tables[0].Rows[0]["EDZK"].ToString();
                        lbi.Lj = ds.Tables[0].Rows[0]["LJ"].ToString();
                        lbi.Qdxs = ds.Tables[0].Rows[0]["QDXS"].ToString();
                        lbi.Jyjgmc = ds.Tables[0].Rows[0]["JYJGMC"].ToString();
                        lbi.Jybgbh = ds.Tables[0].Rows[0]["JYBGBH"].ToString();
                        lbi.CreateTime = DateTime.Now;
                        lbi.UpdateTime = DateTime.Now;
                        lbi.Status = "1";
                        #endregion

                        #region
                        //string strParamSql = "SELECT A.VIN,A.PARAM_NAME,A.PARAM_VALUE,B.PARAM_CODE FROM RLLX_PARAM_ENTITY A JOIN RLLX_PARAM B ON A.PARAM_ID = B.PARAM_ID WHERE A.VIN = '" + strVin + "'";
                        string strParamSql = "SELECT A.VIN,A.PARAM_NAME,A.PARAM_VALUE FROM RLLX_PARAM_ENTITY A WHERE A.VIN = '" + strVin + "'";
                        Object objParam = AccessHelper.ExecuteDataSet(AccessHelper.conn, strParamSql, null);
                        DataSet dsParam = (DataSet)objParam;
                        DataTable dt = dsParam.Tables[0];
                        List<RllxParamEntity> listParam = new List<RllxParamEntity>();
                        foreach (DataRow dr in dt.Rows)
                        {
                            RllxParamEntity rpe = new RllxParamEntity();
                            // rpe.Param_Code = dr[""].ToString();
                            rpe.Vin = dr["VIN"].ToString();
                            //rpe.Param_Name = dr["PARAM_NAME"].ToString();
                            rpe.Param_Value = dr["PARAM_VALUE"].ToString();
                            listParam.Add(rpe);
                        }
                        lbi.EntityList = listParam.ToArray();
                        lbiList.Add(lbi);

                        FuelDataService.OperateResult result = new FuelDataService.OperateResult();
                        result = service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(lbiList).ToArray(), "CATARC_CUSTOM_2012");
                        Utils.UpdataState(result);
                        #endregion
                    }
                }
                else
                {
                    MessageBox.Show("没有选中可上报数据!");
                }

            }
            catch
            {
                MessageBox.Show("上报出错!");
            }
        }

    }
}