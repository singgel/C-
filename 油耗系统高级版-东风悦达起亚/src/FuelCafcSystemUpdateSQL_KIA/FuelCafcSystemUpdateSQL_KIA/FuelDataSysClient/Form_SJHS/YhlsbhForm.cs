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
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Model;

namespace FuelDataSysClient.Form_SJHS
{
    public partial class YhlsbhForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
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

        //初始化 DATATABLE 列
        private void InitData()
        {
            dtNEInit.Columns.Add(new DataColumn("年份"));
            dtNEInit.Columns.Add(new DataColumn("行业平均值", typeof(decimal)));
            dtNEInit.Columns.Add(new DataColumn("实际值", typeof(decimal)));
            dtNEInit.Columns.Add(new DataColumn("目标值", typeof(decimal)));
            dtNEInit.Columns.Add(new DataColumn("达标值", typeof(decimal)));
            dtTEInit = dtNEInit.Clone();
        }

        //初始化字典 倍数 2015 年之前标准
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

        //查询历史排名
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

        //数据源
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
                var order_Te = Utils.serviceCafc.GetProc_CAFC_TE_All(Utils.userId, Utils.password, Utils.userId.Substring(4, 1), "", i + "-01-01", (i + 1) + "-01-01", "");
                DataTable dt_Te = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order_Te);

                //计入新能源核算结果
                var order_Ne = Utils.serviceCafc.GetProc_CAFC_NE_All(Utils.userId, Utils.password, Utils.userId.Substring(4, 1), "", i + "-01-01", (i + 1) + "-01-01", "");
                DataTable dt_Ne = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order_Ne);

                DataRow drTe = GetHS_Data(dt_Te);
                DataRow drNe = GetHS_Data(dt_Ne);

                DataRow drNew = dtNew.NewRow();

                drNew["ID"] = fori + 1;
                drNew["bandYear"] = i;

                drNew["bandT_SL"] = drTe["Sl_act"];
                drNew["bandT_ZBZL"] = drTe["P_ZCZBZL"];
                drNew["bandT_MBZ"] = drTe["Tcafc"];
                drNew["bandT_DBZ"] = Math.Round(Convert.ToDecimal(drTe["Tcafc"]) * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero); // Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                drNew["bandT_SJZ"] = drTe["Cafc"];

                drNew["bandN_SL"] = drNe["Sl_act"];
                drNew["bandN_ZBZL"] = drNe["P_ZCZBZL"];
                drNew["bandN_MBZ"] = drNe["Tcafc"];
                drNew["bandN_DBZ"] = Math.Round(Convert.ToDecimal(drNe["Tcafc"]) * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero); // Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                drNew["bandN_SJZ"] = drNe["Cafc"];

                DataRow dr = i > 2015 ? GetRanking(dt_Ne, i, out ranking) : GetRanking(dt_Te, i, out ranking);
                drNew["bandS_FLG"] = dr["isYes"];
                drNew["bandS_Value"] = dr["c1"];
                drNew["bandS_Raing"] = ranking;

                dtNew.Rows.Add(drNew);
                fori++;
            }
            return dtNew;
        }

        //获取排名
        private DataRow GetRanking(DataTable dt,int year, out int ranking)
        {
            dt.Columns.Add("c1", typeof(decimal)); //排名值字段
            dt.Columns.Add("isYes");   //是否达标
            foreach (DataRow item in dt.Rows)
            {
                decimal dbz = Math.Round(Convert.ToDecimal(item["Tcafc"]) * Convert.ToDecimal(dictInit[year]), 2, MidpointRounding.AwayFromZero);
                item["c1"] = Math.Round(((Convert.ToDecimal(item["Cafc"]) / dbz) * 100), 2, MidpointRounding.AwayFromZero);
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
            int rank = Convert.ToInt32(OracleHelper.ExecuteScalar(OracleHelper.conn, sqlStr.ToString()));
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

        //获取当前企业的核算结果
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

        //初始化表格
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

            GridBand bandT = view.Bands.AddBand("不计新能源核算结果");
            GridBand bandT_SL = bandT.Children.AddBand("乘用车生产/进口量");
            GridBand bandT_ZBZL = bandT.Children.AddBand("平均整备质量");
            GridBand bandT_MBZ = bandT.Children.AddBand("目标值");
            GridBand bandT_DBZ = bandT.Children.AddBand("达标值");
            GridBand bandT_SJZ = bandT.Children.AddBand("实际值");

            GridBand bandN = view.Bands.AddBand("计入新能源核算结果");
            GridBand bandN_SL = bandN.Children.AddBand("乘用车生产/进口量");
            GridBand bandN_ZBZL = bandN.Children.AddBand("平均整备质量");
            GridBand bandN_MBZ = bandN.Children.AddBand("目标值");
            GridBand bandN_DBZ = bandN.Children.AddBand("达标值");
            GridBand bandN_SJZ = bandN.Children.AddBand("实际值");

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

        //初始化表格
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

            GridBand bandT = view.Bands.AddBand("不计新能源核算结果");
            GridBand bandT_SL = bandT.Children.AddBand("乘用车生产/进口量");
            GridBand bandT_ZBZL = bandT.Children.AddBand("平均整备质量");
            GridBand bandT_MBZ = bandT.Children.AddBand("目标值");
            GridBand bandT_DBZ = bandT.Children.AddBand("达标值");
            GridBand bandT_SJZ = bandT.Children.AddBand("实际值");

            GridBand bandN = view.Bands.AddBand("计入新能源核算结果");
            GridBand bandN_SL = bandN.Children.AddBand("乘用车生产/进口量");
            GridBand bandN_ZBZL = bandN.Children.AddBand("平均整备质量");
            GridBand bandN_MBZ = bandN.Children.AddBand("目标值");
            GridBand bandN_DBZ = bandN.Children.AddBand("达标值");
            GridBand bandN_SJZ = bandN.Children.AddBand("实际值");

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

        //初始化表格
        private void InitChartData()
        {
            for (int i = startyear; i < year; i++)
            {
                //计新能源 企业
                var cafcDataNE = Utils.serviceCafc.QueryNECafc(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01");
                ////计新能源 行业
                var cafcDataAllNE = Utils.serviceCafc.GetProc_CAFC_NE_Industry(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01", "");
                if (cafcDataNE.Length == 0)
                {
                    dtNEInit.Rows.Add(new object[] { i, cafcDataAllNE[2].Cafc, 0, 0, 0 });
                }
                else
                {
                    dtNEInit.Rows.Add(new object[] { i, cafcDataAllNE[2].Cafc, cafcDataNE[0].Cafc, cafcDataNE[0].Tcafc, Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero) });
                }
                //不计新能源 企业
                var cafcDataTE = Utils.serviceCafc.GetProc_CAFC_TE_All(Utils.userId, Utils.password, "", Utils.qymc, i + "-01-01", (i + 1) + "-01-01", "");
                ////不计新能源 行业
                var cafcDataAllTE = Utils.serviceCafc.GetProc_CAFC_TE_Industry(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01", "");
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

        //画线
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

    }
}