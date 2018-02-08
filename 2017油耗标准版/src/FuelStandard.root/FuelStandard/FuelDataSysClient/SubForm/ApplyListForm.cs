using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Reflection;
using System.Collections;
using FuelDataModel;

namespace FuelDataSysClient.SubForm
{
    public partial class ApplyListForm : DevExpress.XtraEditors.XtraForm
    {
        public ApplyListForm()
        {
            InitializeComponent();
            getServiceData();
        }

        private void barClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Close();
        }

        private void barClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SelectItem(false);
        }

        private void barSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SelectItem(true);
        }

        private void barRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            getServiceData();
        }

        private void barPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {

        }

        public void SelectItem(bool flg)
        {
            DataView dv = (DataView)this.gridView1.DataSource;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    dv.Table.Rows[i]["check"] = flg;
                }
                this.gridView1.PostEditor();
                this.gridView1.RefreshData();
            }
        }

        public void getServiceData() 
        {
            // 查询服务器端的数据
            FuelDataService.FuelDataSysWebService ws = new FuelDataService.FuelDataSysWebService();
            FuelDataService.VehicleBasicInfo[] queryInfoArr = ws.QueryUploadedFuelData(Utils.userId, Utils.password, 1, 10, null, null, null, null, null, null, null);

            List<VehicleBasicInfo> listV = Utils.FuelInfoS2C(queryInfoArr);
            DataSet ds = ToDataSet<VehicleBasicInfo>(listV);
            //foreach (VehicleBasicInfo v in listV)
            //{
            //    v.EntityList = null;
            //}
            this.gridControl1.DataSource = listV;// ds.Tables[0];
        }

        // 泛型转DataSet

        public void Entity2Table(List<VehicleBasicInfo> list,DataTable table)
        {

        }



        public DataSet ToDataSet<VehicleBasicInfo>(List<VehicleBasicInfo> list)
        {
            return ToDataSet<VehicleBasicInfo>(list, null);
        }

        public DataSet ToDataSet<T>(List<T> p_List, params string[] p_PropertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (p_PropertyName != null)
                propertyNameList.AddRange(p_PropertyName);

            DataSet result = new DataSet();
            DataTable _DataTable = new DataTable();
            if (p_List.Count > 0)
            {
                PropertyInfo[] propertys = p_List[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        // 没有指定属性的情况下全部属性都要转换
                        _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            _DataTable.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < p_List.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(p_List[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(p_List[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    _DataTable.LoadDataRow(array, true);
                }
            }
            result.Tables.Add(_DataTable);
            return result;
        } 
    }
}