using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Common;
using System.Data.OleDb;
using FuelDataSysClient.Model;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Change
{
    public partial class ChangeQuery : DevExpress.XtraEditors.XtraForm
    {
        string type = string.Empty;
        string guid = string.Empty;
        DataTable Initdt = new DataTable();
        DataTable InitdtEntity = new DataTable();
        DataTable rllxParam = new DataTable();
        FuelDataService.FuelDataSysWebService service = Utils.service;
        InitDataTime initTime = new InitDataTime();

        RepositoryItemComboBox riComboCtny = new RepositoryItemComboBox();
        RepositoryItemComboBox riComboCds = new RepositoryItemComboBox();
        RepositoryItemComboBox riComboFcds = new RepositoryItemComboBox();
        RepositoryItemComboBox riComboCdd = new RepositoryItemComboBox();
        RepositoryItemComboBox riComboRldc = new RepositoryItemComboBox();

        public ChangeQuery(string type,string guid)
        {
            InitializeComponent();
            this.type = type;
            this.guid = guid;
            this.gridView4.BestFitColumns();
            this.gridView4.IndicatorWidth = 30;//设置显示行号的列宽
            gridView4.CustomDrawRowIndicator += new RowIndicatorCustomDrawEventHandler(gridView4_CustomDrawRowIndicator);

            string sql = "select * from RLLX_PARAM";
            rllxParam = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null).Tables[0];
            InitComBox();
            SetFuelType();
            this.dateEdit1.Text = initTime.getStartTime();
            this.dateEdit2.Text = initTime.getEndTime();
        }

        /// <summary>
        /// 设置燃料类型下拉框的值
        /// </summary>
        protected void SetFuelType()
        {
            List<string> fuelTypeList = Utils.GetFuelType("SEARCH");
            this.cbRllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        private void InitComBox()
        {
            BindComBox(riComboCtny, MitsUtils.CTNY);
            BindComBox(riComboCds, MitsUtils.CDSHHDL);
            BindComBox(riComboFcds, MitsUtils.FCDSHHDL);
            BindComBox(riComboCdd, MitsUtils.CDD);
            BindComBox(riComboRldc, MitsUtils.RLDC);
            this.gcTable1.RepositoryItems.Add(riComboCtny);
            this.gcTable1.RepositoryItems.Add(riComboCds);
            this.gcTable1.RepositoryItems.Add(riComboFcds);
            this.gcTable1.RepositoryItems.Add(riComboCdd);
            this.gcTable1.RepositoryItems.Add(riComboRldc);
            
        }

        private void BindComBox(RepositoryItemComboBox ricb,string rllx)
        {
            
            CboItemEntity cie = null;
            foreach (DataRow r in rllxParam.Select("FUEL_TYPE='"+rllx+"'"))
            {
                cie = new CboItemEntity();
                cie.Text = r["PARAM_NAME"];
                cie.Value = r["PARAM_CODE"];
                ricb.Items.Add(cie);
            }
            ricb.SelectedIndexChanged += new EventHandler(repositoryItemComboBox1_SelectedIndexChanged);
            ricb.ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(repositoryItemComboBox1_ParseEditValue);
        }

        void gridView4_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
                if (e.Info.IsRowIndicator && e.RowHandle >= 0)
                {
                    e.Info.DisplayText= (e.RowHandle + 1).ToString();
                }
        }

       
        /// <summary>
        /// 初始化燃料参数（新的方式）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private DataTable InitRLLX(string type, string vin, string strClxh, string strClzl, string strRllx, DateTime startDate, DateTime endDate) 
        {
            string sqlCondition = " AND 1=1";
            if (!string.IsNullOrEmpty(vin))
            {
                sqlCondition += String.Format(" AND FC_CLJBXX.VIN='{0}'", vin.Trim());
            }
            if (!string.IsNullOrEmpty(strClxh))
            {
                sqlCondition += String.Format(" AND FC_CLJBXX.CLXH='{0}'", strClxh.Trim());
            }
            if (!string.IsNullOrEmpty(strClzl))
            {
                sqlCondition += String.Format(" AND FC_CLJBXX.CLZL='{0}'", strClzl.Trim());
            }
            if (!string.IsNullOrEmpty(strRllx))
            {
                sqlCondition += String.Format(" AND FC_CLJBXX.RLLX='{0}'", strRllx.Trim());
            }
            switch (this.type)
            {
                case "1":
                    sqlCondition += String.Format(" AND FC_CLJBXX.STATUS='{0}'", this.type);
                    break;
                case "2":
                    sqlCondition += String.Format(" AND FC_CLJBXX.STATUS='{0}'", this.type);
                    break;
                case "0":
                    sqlCondition += String.Format(" AND FC_CLJBXX.STATUS='{0}'", this.type);
                    break;
            }
            StringBuilder sql = new StringBuilder();
            sql = sql.Append("select * from  ");
                    sql.Append("( ");
                    sql.Append("  select FC_CLJBXX.VIN,CLXH,TYMC,RLLX,CT_ZHGKRLXHL,ZCZBZL,CT_BSQXS,ZWPS ");
                    sql.Append("  FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY ON FC_CLJBXX.VIN = VIEW_RLLX_PARAM_ENTTITY.VIN");
                    sql.AppendFormat("  WHERE (((FC_CLJBXX.RLLX)='汽油' Or (FC_CLJBXX.RLLX)='柴油' Or (FC_CLJBXX.RLLX)='两用燃料' Or (FC_CLJBXX.RLLX)='双燃料' Or (FC_CLJBXX.RLLX)== '气体燃料'))  AND FC_CLJBXX.CLZZRQ BETWEEN #{0}# AND #{1}# {2}", startDate, endDate, sqlCondition);
                    sql.Append("  union");
                    sql.Append("  select FC_CLJBXX.VIN,CLXH,TYMC,RLLX,'',ZCZBZL,'',ZWPS ");
                    sql.Append("  FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_CDD ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_CDD.VIN");
                    sql.AppendFormat("  WHERE FC_CLJBXX.RLLX='纯电动'  AND FC_CLJBXX.CLZZRQ BETWEEN #{0}# AND #{1}# {2}", startDate, endDate, sqlCondition);
                    sql.Append("  union");
                    sql.Append("  select FC_CLJBXX.VIN,CLXH,TYMC,RLLX,FCDS_HHDL_ZHGKRLXHL,ZCZBZL,FCDS_HHDL_BSQXS,ZWPS ");
                    sql.Append("  FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_FCDS ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_FCDS.VIN");
                    sql.AppendFormat("  WHERE FC_CLJBXX.RLLX='非插电式混合动力' AND FC_CLJBXX.CLZZRQ BETWEEN #{0}# AND #{1}# {2}", startDate, endDate, sqlCondition);
                    sql.Append("  union");
                    sql.Append("  select FC_CLJBXX.VIN,CLXH,TYMC,RLLX,CDS_HHDL_ZHGKRLXHL,ZCZBZL,CDS_HHDL_BSQXS,ZWPS ");
                    sql.Append("  FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_CDS ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_CDS.VIN");
                    sql.AppendFormat("  WHERE FC_CLJBXX.RLLX='插电式混合动力' AND FC_CLJBXX.CLZZRQ BETWEEN #{0}# AND #{1}# {2}", startDate, endDate, sqlCondition);
                    sql.Append("  union");
                    sql.Append("  select FC_CLJBXX.VIN,CLXH,TYMC,RLLX,'',ZCZBZL,'',ZWPS ");
                    sql.Append("  FROM  FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_RLDC ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_RLDC.VIN");
                    sql.AppendFormat("  WHERE FC_CLJBXX.RLLX='燃料电池'  AND FC_CLJBXX.CLZZRQ BETWEEN #{0}# AND #{1}# {2}", startDate, endDate, sqlCondition);
                    sql.Append(") s");

            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql.ToString(), null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }


        //初始化燃料参数
        #region 初始化燃料参数（旧的方式）
//        private DataTable InitRLLX(string type,string startDate,string endDate)
//        {
//            string sql = string.Empty;
//            DateTime dt = DateTime.Now;
         

//            int pageNum = 1;
//            List<FuelDataService.VehicleBasicInfo> listTemp = new List<FuelDataService.VehicleBasicInfo>();
//            List<FuelDataService.VehicleBasicInfo> list = new List<FuelDataService.VehicleBasicInfo>();
//            while (true)
//            {
//                FuelDataService.VehicleBasicInfo[] queryInfoArr = null;
//                if (type == "1")  //补传
//                {
//                    queryInfoArr = service.QueryApplyUpLoadOT(Utils.userId, Utils.password, pageNum, 50, "", "", "", "", startDate, endDate, "MANUFACTURE_TIME");  //MANUFACTURE_TIME 车辆制造日期
//                }
//                else if (type == "2")  //修改
//                {
//                    queryInfoArr = service.QueryApplyUpdateInfo(Utils.userId, Utils.password, pageNum, 50, "", "", "", "", startDate, endDate, "MANUFACTURE_TIME");  //MANUFACTURE_TIME 车辆制造日期
//                }
//                else  //撤销
//                {
//                    queryInfoArr = service.QueryApplyDelInfo(Utils.userId, Utils.password, pageNum, 50, "", "", "", "", startDate, endDate, "MANUFACTURE_TIME");  //MANUFACTURE_TIME 车辆制造日期
//                }
                
                
                
//                if (queryInfoArr == null || queryInfoArr.Length==0)
//                    break;
//                pageNum++;
//                listTemp.AddRange(queryInfoArr);
//            }

//            if (dt.Day>15)   //当前日期大于15号  查询 当月1号以前的数据
//            {
//                list = listTemp.FindAll(u => u.Clzzrq < dt.AddDays(1 - dt.Day));
//            }
//            else  //查询 上个月1号以前的数据
//            {
//                list = listTemp.FindAll(u => u.Clzzrq < dt.AddMonths(-1).AddDays(1 - dt.Day));
//            }
//            string str = string.Empty;
//            foreach (var item in list)
//            {
//                str += "'" + item.App_Vin + "',";
//            }
//            str = str.TrimEnd(',');


            
//            if (!string.IsNullOrEmpty(str))
//            {
//                sql = @"select * from 
//                        (
//                        select VIN,CLXH,TYMC,RLLX,CT_ZHGKRLXHL,ZCZBZL,CT_BSQXS,ZWPS 
//FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY ON FC_CLJBXX.VIN = VIEW_RLLX_PARAM_ENTTITY.VIN
//WHERE (((FC_CLJBXX.RLLX)='汽油' Or (FC_CLJBXX.RLLX)='柴油' Or (FC_CLJBXX.RLLX)='两用燃料' Or (FC_CLJBXX.RLLX)='双燃料')) 
//
//                        union
//                        select VIN,CLXH,TYMC,RLLX,'',ZCZBZL,'',ZWPS FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_CDD ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_CDD.VIN
//WHERE FC_CLJBXX.RLLX='纯电动' 
//                        union
//                        select VIN,CLXH,TYMC,RLLX,FCDS_HHDL_ZHGKRLXHL,ZCZBZL,FCDS_HHDL_BSQXS,ZWPS FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_FCDS ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_FCDS.VIN
//WHERE FC_CLJBXX.RLLX='非插电式混合动力'
//                        union
//                        select VIN,CLXH,TYMC,RLLX,CDS_HHDL_ZHGKRLXHL,ZCZBZL,CDS_HHDL_BSQXS,ZWPS FROM FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_CDS ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_CDS.VIN
//WHERE FC_CLJBXX.RLLX='插电式混合动力' 
//                        union
//                        select VIN,CLXH,TYMC,RLLX,'',ZCZBZL,'',ZWPS from  FC_CLJBXX LEFT JOIN VIEW_RLLX_PARAM_ENTTITY_RLDC ON FC_CLJBXX.VIN=VIEW_RLLX_PARAM_ENTTITY_RLDC.VIN
//WHERE FC_CLJBXX.RLLX='燃料电池' 
//
//                        ) where VIN in (" + str + ")";
//                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
//                if (ds != null && ds.Tables[0].Rows.Count > 0)
//                {
//                    return ds.Tables[0];
//                }

//            }
          
//            return null;

//        }

        #endregion

        //绑定combobox
        private void BindComboBoxDataSource()
        {
            string sql = "select * from RLLX_PARAM ";//where FUEL_TYPE='"+rllx+"'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                CboItemEntity cie = null;
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    cie = new CboItemEntity();
                    cie.Text = r["PARAM_NAME"];
                    cie.Value = r["PARAM_CODE"];
                    this.repositoryItemComboBox1.Items.Add(cie);
                }
            }

            repositoryItemComboBox1.SelectedIndexChanged += new EventHandler(repositoryItemComboBox1_SelectedIndexChanged);
            repositoryItemComboBox1.ParseEditValue += new DevExpress.XtraEditors.Controls.ConvertEditValueEventHandler(repositoryItemComboBox1_ParseEditValue);
        }

        void repositoryItemComboBox1_ParseEditValue(object sender, DevExpress.XtraEditors.Controls.ConvertEditValueEventArgs e)
        {
            e.Value = e.Value.ToString(); e.Handled = true;
        }

        void repositoryItemComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CboItemEntity item = new CboItemEntity();
            try
            {
                //1.获取下拉框选中值
                item = (CboItemEntity)(sender as ComboBoxEdit).SelectedItem;
                string text = item.Text.ToString();
                string value = item.Value.ToString();
                //2.获取gridview选中的行
                GridView myView = (gcTable1.MainView as GridView);
                int dataIndex = myView.GetDataSourceRowIndex(myView.FocusedRowHandle);
                var gridViewRow = myView.GetDataRow(myView.FocusedRowHandle);
                //3.保存选中值到datatable
                //Initdt.Rows[dataIndex]["value"] = value;
                Initdt.Rows[dataIndex]["UPDATEFIELD"] = text;
                Initdt.Rows[dataIndex]["FIELDNEW"] = QueryServerRLLX(gridViewRow["VIN"].ToString(), value);
                Initdt.Rows[dataIndex]["FIELDOLD"] = QueryRLLX(gridViewRow["VIN"].ToString(), value); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string QueryRLLX(string vin ,string param)
        {
            string sql = string.Format("select PARAM_VALUE from RLLX_PARAM_ENTITY where vin='{0}' and PARAM_CODE='{1}'",vin,param);
            var result = AccessHelper.ExecuteScalar(AccessHelper.conn, sql, null);
            return result.ToString();
        }

        private string QueryServerRLLX(string vin, string param)
        {
            var serverData = service.QueryHisFuelData(Utils.userId, Utils.password, 1, 1, vin, "", "", "", "", "", "MANUFACTURE_TIME");
            if (serverData != null && serverData.Length > 0)
            {
                foreach (var item in serverData[0].EntityList)
                {
                    if (item.Param_Code == param)
                    {
                        return item.Param_Value;
                    }
                }
            }
            return string.Empty;
        }

        private DataTable AddColumns(DataTable dt,string applytype)
        {
            if (dt == null) return null;
            foreach (DataRow r in dt.Rows)
            {
                r["check"] = true;
                r["APPLYTYPE"] = applytype;
            }
            return dt;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                GridView view = (GridView)gcTable1.MainView;
                view.FocusedRowHandle = 0;
                view.FocusedColumn = view.Columns[1];
                //var view = gcTable1.MainView;
                Utils.SelectItem(view, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            try
            {
                GridView view = (GridView)gcTable1.MainView;
                view.FocusedRowHandle = 0;
                view.FocusedColumn = view.Columns[1];
                Utils.SelectItem(view,false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {

            DataView dv = GetCheckData();
            if (dv != null && dv.Table.Rows.Count > 0)
            {
                ChangeAddXtraForm caxf = (ChangeAddXtraForm)this.Owner;
                caxf.LoadDataByVin(dv);
                if (MessageBox.Show("添加成功，是否继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("请选择要操作的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        /// <summary>
        /// 选择选中数据
        /// </summary>
        /// <returns></returns>
        private DataView GetCheckData()
        {
            var view = gcTable1.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            return C2M.SelectedParamEntityDataView(dv, "check");
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.dateEdit1.Text) || string.IsNullOrEmpty(this.dateEdit2.Text))
            {
                MessageBox.Show("请选择查询的日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string strVin = tbVin.Text;
            string strClxh = tbClxh.Text;
            string strClzl = tbClzl.Text;
            string strRllx = cbRllx.Text;
            DateTime dtStart = DateTime.Parse(dateEdit1.Text);
            DateTime dtEnd = DateTime.Parse(dateEdit2.Text);
            Initdt = InitRLLX(this.type, strVin, strClxh, strClzl, strRllx, dtStart, dtEnd);
            //Initdt = InitRLLX(this.type, dateEdit1.Text, dateEdit2.Text);
            if (Initdt == null)
            {
                Initdt = new DataTable();
            } 
            if (Initdt != null && Initdt.Rows.Count > 0)
            {
                Initdt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                Initdt.Columns["check"].SetOrdinal(0);
                Initdt.Columns.Add("UPDATEFIELD");
                Initdt.Columns.Add("FIELDOLD");
                Initdt.Columns.Add("FIELDNEW");
                Initdt.Columns.Add("APPLYTYPE");
            }

            switch (this.type)
            {
                case "1":
                    gridView4.Columns["UPDATEFIELD"].OptionsColumn.AllowEdit = false;
                    gridView4.Columns["FIELDOLD"].OptionsColumn.AllowEdit = false;
                    Initdt = AddColumns(Initdt, "补传");
                    break;
                case "2":
                    Initdt = AddColumns(Initdt, "修改");
                    break;
                case "0":
                    gridView4.Columns["UPDATEFIELD"].OptionsColumn.AllowEdit = false;
                    gridView4.Columns["FIELDOLD"].OptionsColumn.AllowEdit = false;
                    Initdt = AddColumns(Initdt, "撤销");
                    break;
            }
            this.gcTable1.DataSource = Initdt;
            //BindComboBoxDataSource();
            lblNum.Text = string.Format("共{0}条", Initdt.Rows.Count);
        }


        private void gridView4_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            switch (e.CellValue.ToString())
            {
                case "汽油":
                    gridColumn9.ColumnEdit = riComboCtny;
                    e.RepositoryItem = riComboCtny;
                    break;
                case "柴油":
                    gridColumn9.ColumnEdit = riComboCtny;
                    e.RepositoryItem = riComboCtny;
                    break;
                case "两用燃料":
                    gridColumn9.ColumnEdit = riComboCtny;
                    e.RepositoryItem = riComboCtny;
                    break;
                case "双燃料":
                    gridColumn9.ColumnEdit = riComboCtny;
                    e.RepositoryItem = riComboCtny;
                    break;
                case "气体燃料":
                    gridColumn9.ColumnEdit = riComboCtny;
                    e.RepositoryItem = riComboCtny;
                    break;
                case "非插电式混合动力":
                    gridColumn9.ColumnEdit = riComboFcds;
                    e.RepositoryItem = riComboFcds;
                    break;
                case "插电式混合动力":
                    gridColumn9.ColumnEdit = riComboCds;
                    e.RepositoryItem = riComboCds;
                    break;
                      case "纯电动":
                    gridColumn9.ColumnEdit = riComboCdd;
                    e.RepositoryItem = riComboCdd;
                    break;
                case "燃料电池":
                    gridColumn9.ColumnEdit = riComboRldc;
                    e.RepositoryItem = riComboRldc;
                    break;
            }     
        }

    }
}