using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Common;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraCharts;
using FuelDataSysClient.FuelCafc;
using FuelDataSysClient.Tool;
using DevExpress.XtraPrinting;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient
{
    public partial class YhlsbhForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        DataTable dtNEInit = new DataTable();  //计入新能源
        DataTable dtTEInit = new DataTable();  //不计新能源
        Dictionary<int, string> dictInit = new Dictionary<int, string>();  //字典
        int year = DateTime.Now.Year + 1;  //近5年 当前年 +1
        int startyear = 0;                 //近5年 开始年份

        public YhlsbhForm()
        {
            InitializeComponent();
            startyear = year - 4;
            InitData();
            InitDict();
            List<QueryCafcModel> list = new List<QueryCafcModel>();
            InitGridData(list);
        }

        /// <summary>
        /// 初始化 DATATABLE 列
        /// </summary>
        private void InitData()
        {
            dtNEInit.Columns.Add(new DataColumn("年份"));
            dtNEInit.Columns.Add(new DataColumn("行业平均值", typeof(decimal)));
            dtNEInit.Columns.Add(new DataColumn("实际值", typeof(decimal)));
            dtNEInit.Columns.Add(new DataColumn("目标值", typeof(decimal)));
            dtNEInit.Columns.Add(new DataColumn("达标值", typeof(decimal)));
            dtTEInit = dtNEInit.Clone();
        }

        /// <summary>
        /// 初始化字典 倍数 2015 年之前标准
        /// </summary>
        private void InitDict()
        {
            dictInit.Add(2012, "1.09");
            dictInit.Add(2013, "1.06");
            dictInit.Add(2014, "1.03");
            dictInit.Add(2015, "1.00");
            dictInit.Add(2016, "1.34");
            dictInit.Add(2017, "1.28");
            dictInit.Add(2018, "1.20");
            dictInit.Add(2019, "1.10");
            dictInit.Add(2020, "1.00");
        }


        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                InitGridData();
                InitChartData();
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
            
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.gridControl1.DataSource == null)
                {
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Title = "导出Excel", Filter = "Excel文件(*.xls)|*.xls", FileName = "油耗历史变化" };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };

                    this.gridControl1.ExportToXls(saveFileDialog.FileName, options);

                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable RankingNewTable()
        {
            string flag = string.Empty;
            int fori = 0;
            DataTable dtNew = new DataTable();
            //添加列
            dtNew.Columns.Add("ID");           //ID
            dtNew.Columns.Add("bandYear");     //年份

            dtNew.Columns.Add("bandT_SL");     //数量（计入新能源）
            dtNew.Columns.Add("bandT_ZBZL");   //平均整备质量（计入新能源）
            dtNew.Columns.Add("bandT_MBZ");    //目标值（计入新能源）
            dtNew.Columns.Add("bandT_DBZ");    //达标值（计入新能源）
            dtNew.Columns.Add("bandT_SJZ");    //实际值（计入新能源）

            dtNew.Columns.Add("bandN_SL");     //数量（计入新能源）
            dtNew.Columns.Add("bandN_ZBZL");   //平均整备质量（计入新能源）
            dtNew.Columns.Add("bandN_MBZ");    //目标值（计入新能源）
            dtNew.Columns.Add("bandN_DBZ");    //达标值（计入新能源）
            dtNew.Columns.Add("bandN_SJZ");    //实际值（计入新能源）

            dtNew.Columns.Add("bandS_FLG");    //是否达标
            dtNew.Columns.Add("bandS_Value", typeof(decimal));   //排名值字段
            dtNew.Columns.Add("bandS_Raing");  //排名

            for (int i = startyear; i < year; i++)
            {
                int ranking = 0;

                //不计新能源核算结果
                var order_Te = cafcService.GetProc_CAFC_TE_All(Utils.userId, Utils.password, Utils.userId.Substring(4, 1), "", i + "-01-01", i + "-12-31", "");
                if (order_Te == null) return null;
                DataTable dt_Te = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order_Te);

                //计入新能源核算结果
                var order_Ne = cafcService.GetProc_CAFC_NE_All(Utils.userId, Utils.password, Utils.userId.Substring(4, 1), "", i + "-01-01", i + "-12-31", "");
                if (order_Ne == null) return null;
                DataTable dt_Ne = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order_Ne);

                DataRow drTe = GetHS_Data(dt_Te);
                DataRow drNe = GetHS_Data(dt_Ne);

                DataRow drNew = dtNew.NewRow();

                drNew["ID"] = fori + 1;
                drNew["bandYear"] = i;

                drNew["bandT_SL"] = drTe == null ? string.Empty : drTe["Sl_act"];
                drNew["bandT_ZBZL"] = drTe == null ? string.Empty : drTe["P_ZCZBZL"];
                drNew["bandT_MBZ"] = drTe == null ? string.Empty : drTe["Tcafc"];
                drNew["bandT_DBZ"] = drTe == null ? (decimal)0.00 : Math.Round(Convert.ToDecimal(drTe["Tcafc"]) * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero); // Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                drNew["bandT_SJZ"] = drTe == null ? string.Empty : drTe["Cafc"];

                drNew["bandN_SL"] = drNe == null ? string.Empty : drNe["Sl_act"];
                drNew["bandN_ZBZL"] = drNe == null ? string.Empty : drNe["P_ZCZBZL"];
                drNew["bandN_MBZ"] = drNe == null ? string.Empty : drNe["Tcafc"];
                drNew["bandN_DBZ"] = drNe == null ? (decimal)0.00 : Math.Round(Convert.ToDecimal(drNe["Tcafc"]) * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero); // Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                drNew["bandN_SJZ"] = drNe == null ? string.Empty : drNe["Cafc"];

                DataRow dr = i > 2015 ? GetRanking(dt_Ne, i, out ranking) : GetRanking(dt_Te, i, out ranking);
                drNew["bandS_FLG"] = dr == null ? string.Empty : dr["isYes"];
                drNew["bandS_Value"] = dr == null ? (decimal)0.00 : dr["c1"];
                drNew["bandS_Raing"] = ranking;

                dtNew.Rows.Add(drNew);
                fori++;
            }
            return dtNew;
        }

        /// <summary>
        /// 获取排名
        /// </summary>
        /// <param name="fuelCafcArray"></param>
        /// <returns></returns>
        private DataRow GetRanking(DataTable dt,int year, out int ranking)
        {
            dt.Columns.Add("c1", typeof(decimal)); //排名值字段
            dt.Columns.Add("isYes");   //是否达标
            foreach (DataRow item in dt.Rows)
            {
                decimal dbz = Math.Round(Convert.ToDecimal(item["Tcafc"]) * Convert.ToDecimal(dictInit[year]), 2, MidpointRounding.AwayFromZero);
                item["c1"] = Math.Round(((Convert.ToDecimal(item["Cafc"]) / dbz) * 100), 2, MidpointRounding.AwayFromZero);
                //if (string.IsNullOrEmpty(item["c1"].ToString()))
                //{
                //    item["c1"] = "0.00";
                //}
                //else
                //{
                //    item["c1"] = Math.Round((Convert.ToDecimal(item["Cafc"]) / Convert.ToDecimal(item["Tcafc1"]) * 100), 2, MidpointRounding.AwayFromZero);
                //}
                if (Convert.ToDecimal(item["c1"]) <= Convert.ToDecimal(100.00))
                {
                    item["isYes"] = "是";
                }
                else
                {
                    item["isYes"] = "否";
                }
            }
            DataView dv = dt.DefaultView;
            dv.Sort = "c1 asc";
            dt = dv.ToTable();
            //dt.DefaultView.Sort = "c1 asc";
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(String.Format("select RANK from FUEL_RANKING where STATUS = 0 and QCSCQY like '{0}' and YEAR = {1} ", Utils.qymc, year));
            if (Utils.userId.Substring(4, 1).Equals("C"))
            {
                sqlStr.Append(" and DOMESTIC = 1 ");
            }
            else
            {
                sqlStr.Append(" and DOMESTIC = 0 ");
            }
            var ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStr.ToString(), null);
            int rank = 0;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                rank = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Utils.qymc == Convert.ToString(dt.Rows[i]["Qcscqy"]))
                {
                    if (year > 2015)
                    {
                        ranking = i + 1;
                    }
                    else
                    {
                        ranking = rank;
                    }
                    return dt.Rows[i];
                }
            }
            ranking = 0;
            return null;
        }

        /// <summary>
        /// 获取当前企业的核算结果
        /// </summary>
        /// <param name="fuelCafcArray"></param>
        /// <returns></returns>
        private DataRow GetHS_Data(DataTable dt)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Utils.qymc == Convert.ToString(dt.Rows[i]["Qcscqy"]))
                {
                    return dt.Rows[i];
                }
            }
            return null;
        }

       
        ///初始化表格
        private void InitGridData()
        {
            
            // advBandedGridView1是表格上的默认视图，注意这里声明的是：BandedGridView
            BandedGridView view = advBandedGridView1 as BandedGridView;

            view.BeginUpdate(); //开始视图的编辑，防止触发其他事件
            view.BeginDataUpdate(); //开始数据的编辑
            view.Bands.Clear();

            //修改附加选项
            view.OptionsView.ShowColumnHeaders = false;                         //因为有Band列了，所以把ColumnHeader隐藏
            view.OptionsView.ShowGroupPanel = false;                            //如果没必要分组，就把它去掉
            view.OptionsView.EnableAppearanceEvenRow = false;                   //是否启用偶数行外观
            view.OptionsView.EnableAppearanceOddRow = true;                     //是否启用奇数行外观
            view.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;   //是否显示过滤面板
            view.OptionsCustomization.AllowColumnMoving = false;                //是否允许移动列
            view.OptionsCustomization.AllowColumnResizing = false;              //是否允许调整列宽
            view.OptionsCustomization.AllowGroup = false;                       //是否允许分组
            view.OptionsCustomization.AllowFilter = false;                      //是否允许过滤
            view.OptionsCustomization.AllowSort = true;                         //是否允许排序
            view.OptionsSelection.EnableAppearanceFocusedCell = true;           //???
            view.OptionsBehavior.Editable = false;                               //是否允许用户编辑单元格
            view.OptionsView.ColumnAutoWidth = true;

            //添加列标题
            GridBand bandID = view.Bands.AddBand("ID");
            bandID.Visible = false; //隐藏ID列
            GridBand bandYear = view.Bands.AddBand("年份");

            GridBand bandN = view.Bands.AddBand("计入新能源核算结果");
            GridBand bandN_SL = bandN.Children.AddBand("乘用车生产/进口量");
            GridBand bandN_ZBZL = bandN.Children.AddBand("平均整备质量");
            GridBand bandN_MBZ = bandN.Children.AddBand("目标值");
            GridBand bandN_DBZ = bandN.Children.AddBand("达标值");
            GridBand bandN_SJZ = bandN.Children.AddBand("实际值");

            GridBand bandT = view.Bands.AddBand("不计新能源核算结果");
            GridBand bandT_SL = bandT.Children.AddBand("乘用车生产/进口量");
            GridBand bandT_ZBZL = bandT.Children.AddBand("平均整备质量");
            GridBand bandT_MBZ = bandT.Children.AddBand("目标值");
            GridBand bandT_DBZ = bandT.Children.AddBand("达标值");
            GridBand bandT_SJZ = bandT.Children.AddBand("实际值");

            GridBand bandS = view.Bands.AddBand("达标情况");
            GridBand bandS_FLG = bandS.Children.AddBand("是否达标");
            GridBand bandS_Value = bandS.Children.AddBand("实际值/达标值(%)");
            GridBand bandS_Raing = bandS.Children.AddBand("排名");

            #region
            //列标题对齐方式
            bandYear.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_SL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_ZBZL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_MBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_DBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_SJZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandN.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_SL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_ZBZL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_MBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_DBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_SJZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandS.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandS_FLG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandS_Value.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandS_Raing.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;


            //绑定数据源并显示
            DataTable dt = RankingNewTable();
            if (dt == null) return;
            gridControl1.DataSource = dt;
            gridControl1.MainView.PopulateColumns();
            #endregion

            //将标题列和数据列对应
            view.Columns["ID"].OwnerBand = bandID;
            view.Columns["bandYear"].OwnerBand = bandYear;
            view.Columns["bandT_SL"].OwnerBand = bandT_SL;
            view.Columns["bandT_ZBZL"].OwnerBand = bandT_ZBZL;
            view.Columns["bandT_MBZ"].OwnerBand = bandT_MBZ;
            view.Columns["bandT_DBZ"].OwnerBand = bandT_DBZ;
            view.Columns["bandT_SJZ"].OwnerBand = bandT_SJZ;

            view.Columns["bandN_SL"].OwnerBand = bandN_SL;
            view.Columns["bandN_ZBZL"].OwnerBand = bandN_ZBZL;
            view.Columns["bandN_MBZ"].OwnerBand = bandN_MBZ;
            view.Columns["bandN_DBZ"].OwnerBand = bandN_DBZ;
            view.Columns["bandN_SJZ"].OwnerBand = bandN_SJZ;

            view.Columns["bandS_FLG"].OwnerBand = bandS_FLG;
            view.Columns["bandS_Value"].OwnerBand = bandS_Value;
            view.Columns["bandS_Raing"].OwnerBand = bandS_Raing;

            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑

        }

        ///初始化表格
        private void InitGridData(List<QueryCafcModel> list)
        {

            // advBandedGridView1是表格上的默认视图，注意这里声明的是：BandedGridView
            BandedGridView view = advBandedGridView1 as BandedGridView;

            view.BeginUpdate(); //开始视图的编辑，防止触发其他事件
            view.BeginDataUpdate(); //开始数据的编辑
            view.Bands.Clear();

            //修改附加选项
            view.OptionsView.ShowColumnHeaders = false;                         //因为有Band列了，所以把ColumnHeader隐藏
            view.OptionsView.ShowGroupPanel = false;                            //如果没必要分组，就把它去掉
            view.OptionsView.EnableAppearanceEvenRow = false;                   //是否启用偶数行外观
            view.OptionsView.EnableAppearanceOddRow = true;                     //是否启用奇数行外观
            view.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;   //是否显示过滤面板
            view.OptionsCustomization.AllowColumnMoving = false;                //是否允许移动列
            view.OptionsCustomization.AllowColumnResizing = false;              //是否允许调整列宽
            view.OptionsCustomization.AllowGroup = false;                       //是否允许分组
            view.OptionsCustomization.AllowFilter = false;                      //是否允许过滤
            view.OptionsCustomization.AllowSort = true;                         //是否允许排序
            view.OptionsSelection.EnableAppearanceFocusedCell = true;           //???
            view.OptionsBehavior.Editable = false;                               //是否允许用户编辑单元格
            view.OptionsView.ColumnAutoWidth = true;

            //添加列标题
            GridBand bandID = view.Bands.AddBand("ID");
            bandID.Visible = false; //隐藏ID列
            GridBand bandYear = view.Bands.AddBand("年份");

            GridBand bandN = view.Bands.AddBand("计入新能源核算结果");
            GridBand bandN_SL = bandN.Children.AddBand("乘用车生产/进口量");
            GridBand bandN_ZBZL = bandN.Children.AddBand("平均整备质量");
            GridBand bandN_MBZ = bandN.Children.AddBand("目标值");
            GridBand bandN_DBZ = bandN.Children.AddBand("达标值");
            GridBand bandN_SJZ = bandN.Children.AddBand("实际值");

            GridBand bandT = view.Bands.AddBand("不计新能源核算结果");
            GridBand bandT_SL = bandT.Children.AddBand("乘用车生产/进口量");
            GridBand bandT_ZBZL = bandT.Children.AddBand("平均整备质量");
            GridBand bandT_MBZ = bandT.Children.AddBand("目标值");
            GridBand bandT_DBZ = bandT.Children.AddBand("达标值");
            GridBand bandT_SJZ = bandT.Children.AddBand("实际值");

            GridBand bandS = view.Bands.AddBand("达标情况");
            GridBand bandS_FLG = bandS.Children.AddBand("是否达标");
            GridBand bandS_Value = bandS.Children.AddBand("实际值/达标值(%)");
            GridBand bandS_Raing = bandS.Children.AddBand("排名");

            #region
            //列标题对齐方式
            bandYear.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_SL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_ZBZL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_MBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_DBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandT_SJZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandN.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_SL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_ZBZL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_MBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_DBZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandN_SJZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandS.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandS_FLG.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandS_Value.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandS_Raing.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;


            //绑定数据源并显示
            gridControl1.DataSource = list;
            gridControl1.MainView.PopulateColumns();
            #endregion

            //将标题列和数据列对应
            view.Columns["ID"].OwnerBand = bandID;
            view.Columns["bandYear"].OwnerBand = bandYear;
            view.Columns["bandT_SL"].OwnerBand = bandT_SL;
            view.Columns["bandT_ZBZL"].OwnerBand = bandT_ZBZL;
            view.Columns["bandT_MBZ"].OwnerBand = bandT_MBZ;
            view.Columns["bandT_DBZ"].OwnerBand = bandT_DBZ;
            view.Columns["bandT_SJZ"].OwnerBand = bandT_SJZ;

            view.Columns["bandN_SL"].OwnerBand = bandN_SL;
            view.Columns["bandN_ZBZL"].OwnerBand = bandN_ZBZL;
            view.Columns["bandN_MBZ"].OwnerBand = bandN_MBZ;
            view.Columns["bandN_DBZ"].OwnerBand = bandN_DBZ;
            view.Columns["bandN_SJZ"].OwnerBand = bandN_SJZ;

            view.Columns["bandS_FLG"].OwnerBand = bandS_FLG;
            view.Columns["bandS_Value"].OwnerBand = bandS_Value;
            view.Columns["bandS_Raing"].OwnerBand = bandS_Raing;

            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑

        }

        public List<QueryCafcModel> GetYhlspmData() 
        {
            List<QueryCafcModel> dataList = new List<QueryCafcModel>();
            return dataList;
        }

        private void InitChartData()
        {
            for (int i = startyear; i < year; i++)
            {
                //计新能源 企业
                var cafcDataNE = cafcService.QueryNECafc(Utils.userId, Utils.password, i + "-01-01", i + "-12-31");
                ////计新能源 行业
                var cafcDataAllNE = cafcService.GetProc_CAFC_NE_Industry(Utils.userId, Utils.password, i + "-01-01", i + "-12-31", "");
                if (cafcDataNE == null || cafcDataAllNE == null) return;
                if (cafcDataNE.Length == 0)
                {
                    dtNEInit.Rows.Add(new object[] { i, cafcDataAllNE[2].Cafc, 0, 0, 0 });
                }
                else
                {
                    dtNEInit.Rows.Add(new object[] { i, cafcDataAllNE[2].Cafc, cafcDataNE[0].Cafc, cafcDataNE[0].Tcafc, Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero) });
                }
                //不计新能源 企业
                var cafcDataTE = cafcService.GetProc_CAFC_TE_All(Utils.userId, Utils.password, "", Utils.qymc, i + "-01-01", i + "-12-31", "");
                ////不计新能源 行业
                var cafcDataAllTE = cafcService.GetProc_CAFC_TE_Industry(Utils.userId, Utils.password, i + "-01-01", i + "-12-31", "");
                if (cafcDataTE == null || cafcDataAllTE == null) return;
                if (cafcDataTE.Length == 0)
                {
                    dtTEInit.Rows.Add(new object[] { i, 0, 0, 0, 0 });
                }
                else
                {
                    dtTEInit.Rows.Add(new object[] { i, cafcDataAllTE[2].Cafc, cafcDataTE[0].Cafc, cafcDataTE[0].Tcafc, cafcDataTE[0].Tcafc1 });
                }
            }
            BindChart(this.chartControl1, dtNEInit);
            BindChart(this.chartControl2, dtTEInit);
        }

        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="dt"></param>
        private void BindChart(ChartControl cc, DataTable dt)
        {
            cc.Series.Clear();
            for (int i = 1; i < 5; i++)
            {
                // 线行图里的第一个行
                Series Series = new Series(dt.Columns[i].ColumnName, ViewType.Bar);
                Series.DataSource = dt;
                Series.ArgumentScaleType = ScaleType.Qualitative;
                Series.ArgumentDataMember = dt.Columns[0].ColumnName;
                Series.ValueScaleType = ScaleType.Numerical;
                Series.ValueDataMembers.AddRange(new string[] { dt.Columns[i].ColumnName });
                cc.Series.Add(Series);
            }
        }


        #region 测试代码
        ///初始化表格
        private void InitGrid()
        {
            // advBandedGridView1是表格上的默认视图，注意这里声明的是：BandedGridView
            BandedGridView view = advBandedGridView1 as BandedGridView;

            view.BeginUpdate(); //开始视图的编辑，防止触发其他事件
            view.BeginDataUpdate(); //开始数据的编辑
            view.Bands.Clear();

            //修改附加选项
            view.OptionsView.ShowColumnHeaders = false;                         //因为有Band列了，所以把ColumnHeader隐藏
            view.OptionsView.ShowGroupPanel = false;                            //如果没必要分组，就把它去掉
            view.OptionsView.EnableAppearanceEvenRow = false;                   //是否启用偶数行外观
            view.OptionsView.EnableAppearanceOddRow = true;                     //是否启用奇数行外观
            view.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;   //是否显示过滤面板
            view.OptionsCustomization.AllowColumnMoving = false;                //是否允许移动列
            view.OptionsCustomization.AllowColumnResizing = false;              //是否允许调整列宽
            view.OptionsCustomization.AllowGroup = false;                       //是否允许分组
            view.OptionsCustomization.AllowFilter = false;                      //是否允许过滤
            view.OptionsCustomization.AllowSort = true;                         //是否允许排序
            view.OptionsSelection.EnableAppearanceFocusedCell = true;           //???
            view.OptionsBehavior.Editable = false;                               //是否允许用户编辑单元格
            view.OptionsView.ColumnAutoWidth = true;

            //添加列标题
            GridBand bandID = view.Bands.AddBand("ID");
            bandID.Visible = false; //隐藏ID列
            GridBand bandName = view.Bands.AddBand("姓名");
            GridBand bandSex = view.Bands.AddBand("性别");
            GridBand bandBirth = view.Bands.AddBand("出生日期");
            GridBand bandScore = view.Bands.AddBand("分数");
            GridBand bandMath = bandScore.Children.AddBand("数学");
            GridBand bandChinese = bandScore.Children.AddBand("语文");
            GridBand bandEnglish = bandScore.Children.AddBand("英语");

            //列标题对齐方式
            bandName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandSex.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandBirth.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandScore.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandMath.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandChinese.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandEnglish.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            ////模拟几个数据
            List<Record> listDataSource = new List<Record>();
            listDataSource.Add(new Record(1, "张三", "男", Convert.ToDateTime("1989-5-6"), 115.5f, 101, 96, ""));
            listDataSource.Add(new Record(2, "李四", "女", Convert.ToDateTime("1987-12-23"), 92, 85, 87, ""));
            listDataSource.Add(new Record(3, "王五", "女", Convert.ToDateTime("1990-2-11"), 88, 69, 41.5f, ""));
            listDataSource.Add(new Record(4, "赵六", "男", Convert.ToDateTime("1988-9-1"), 119, 108, 110, "备注行"));

            //////绑定数据源并显示
            gridControl1.DataSource = listDataSource;
            gridControl1.MainView.PopulateColumns();

            //将标题列和数据列对应
            view.Columns["ID"].OwnerBand = bandID;
            view.Columns["Name"].OwnerBand = bandName;
            view.Columns["Sex"].OwnerBand = bandSex;
            view.Columns["Birth"].OwnerBand = bandBirth;
            view.Columns["Math"].OwnerBand = bandMath;
            view.Columns["Chinese"].OwnerBand = bandChinese;
            view.Columns["English"].OwnerBand = bandEnglish;

            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑

        }

        #region 运行时绑定到实现Ilist接口的数据源

        public class Record
        {
            int id;
            DateTime birth;
            string name, sex, remark;
            float math, chinese, english;
            public Record(int id, string name, string sex, DateTime birth, float math, float chinese, float english, string remark)
            {
                this.id = id;
                this.name = name;
                this.sex = sex;
                this.birth = birth;
                this.math = math;
                this.chinese = chinese;
                this.english = english;
                this.remark = remark;
            }
            public int ID { get { return id; } }
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public string Sex
            {
                get { return sex; }
                set { sex = value; }
            }
            public DateTime Birth
            {
                get { return birth; }
                set { birth = value; }
            }
            public float Math
            {
                get { return math; }
                set { math = value; }
            }
            public float Chinese
            {
                get { return chinese; }
                set { chinese = value; }
            }
            public float English
            {
                get { return english; }
                set { english = value; }
            }
            public string Remark
            {
                get { return remark; }
                set { remark = value; }
            }


        }

        #endregion
        #endregion

    }
}