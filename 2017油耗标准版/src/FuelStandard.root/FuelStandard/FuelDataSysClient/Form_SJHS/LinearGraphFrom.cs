using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraCharts;
using FuelDataSysClient.FuelCafc;
using Common;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient
{
    public partial class LinearGraphFrom : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        DataTable dtNEInit = new DataTable();  //计入新能源
        DataTable dtTEInit = new DataTable();  //不计新能源
        Dictionary<int, string> dictInit = new Dictionary<int, string>();  //字典
        int year = DateTime.Now.Year + 1;  //近5年 当前年 +1
        int startyear = 0;                 //近5年 开始年份
        public LinearGraphFrom()
        {
            InitializeComponent();
            startyear = year - 4;
            InitData();
            InitDict();

            if (Utils.IsFuelTest)  //正式用户
            {
                chartControl1.Series.Clear();
                chartControl2.Series.Clear();
            }
            else  //测试用户登录
            {
                Utils.qymc = "三菱汽车销售（中国）有限公司";
                barButtonItem3.Enabled = false;
                this.gridControl1.DataSource = RankingTestTable("FADCFMITSU001", "c0d95d$B");
            }
            
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

       
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {

            for (int i = startyear; i < year; i++)
            {
                //计新能源 企业
                var cafcDataNE = cafcService.QueryNECafc(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01");
                var ss = cafcDataNE.Length;
                ////计新能源 行业
                var cafcDataAllNE = cafcService.GetProc_CAFC_NE_Industry(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01", "");
                if (cafcDataNE.Length == 0)
                {
                    dtNEInit.Rows.Add(new object[] { i, cafcDataAllNE[2].Cafc, 0, 0, 0 });
                }
                else
                {
                    dtNEInit.Rows.Add(new object[] { i, cafcDataAllNE[2].Cafc, cafcDataNE[0].Cafc, cafcDataNE[0].Tcafc, Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero) });
                }
                //不计新能源 企业
                var cafcDataTE = cafcService.GetProc_CAFC_TE_All(Utils.userId, Utils.password, "", Utils.qymc, i + "-01-01", (i + 1) + "-01-01", "");
                ////不计新能源 行业
                var cafcDataAllTE = cafcService.GetProc_CAFC_TE_Industry(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01", "");
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
            //this.gridControl1.DataSource = RankingTable();
            this.barButtonItem3.Enabled = false;

            this.gridControl1.DataSource = RankingNewTable();
        }

        /// <summary>
        /// 画线
        /// </summary>
        /// <param name="cc"></param>
        /// <param name="dt"></param>
        private void BindChart(ChartControl cc,DataTable dt)
        {
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
        //获取全部不计新能源数据
        //实计值除以达标值 计算排名正序
        //找到当前排名位置
        //取出后添加四列
        //添加到新表中
        private DataTable RankingTable()
        {
            string flag = string.Empty;
            int fori = 0;
            DataTable dtNew = new DataTable();
            for (int i = startyear; i < year; i++)
            {
                var order = cafcService.GetProc_CAFC_TE_All(Utils.userId, Utils.password, Utils.userId.Substring(4, 1), "", i + "-01-01", (i + 1) + "-01-01", "");
                DataTable dt = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order);
                dt.Columns.Add("year");
                dt.Columns.Add("jtcafc");  //目标值
                dt.Columns.Add("jtcafc1"); //达标值
                dt.Columns.Add("jcafc");   //实际值
                dt.Columns.Add("isYes");   //是否达标
                dt.Columns.Add("c1",typeof(decimal));   //排名值字段
                dt.Columns.Add("ranking"); //排名
                foreach (DataRow item in dt.Rows)
                {
                    if (string.IsNullOrEmpty(item["c1"].ToString()))
                    {
                        item["c1"] = "0.00";
                    }
                    else 
                    {
                        item["c1"] = Math.Round((Convert.ToDecimal(item["Cafc"]) / Convert.ToDecimal(item["Tcafc1"]) * 100), 2, MidpointRounding.AwayFromZero);
                    }
                    if (Convert.ToDecimal(item["c1"])<=Convert.ToDecimal(100.00))
                    {
                        item["isYes"] = "是";
                    }
                    else
                    {
                        item["isYes"] = "否";
                    }
                    //item["c1"] += "%"; 
                }
                DataRow[] drr = dt.Select("1=1", " c1 asc");
                //DataTable dtCopy = dt.Copy();
                //DataView dv = dt.DefaultView;
                //dv.Sort = "c1";

                //dtCopy = dv.ToTable();
                DataTable Temp = new DataTable();
                Temp = drr.CopyToDataTable();
                int ranking= 0;


                if(dtNew.Rows.Count==0)
                {
                    dtNew = Temp.Clone();
                }
                
                DataRow dr = GetRanking(Temp, out ranking);
                dtNew.Rows.Add(dr.ItemArray);
                var cafcDataNE = cafcService.QueryNECafc(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01");
                dtNew.Rows[fori]["year"] = i;
                dtNew.Rows[fori]["jtcafc"] = cafcDataNE[0].Tcafc;
                dtNew.Rows[fori]["jtcafc1"] = Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                dtNew.Rows[fori]["jcafc"] = cafcDataNE[0].Cafc;
                dtNew.Rows[fori]["ranking"] = ranking;
                fori++;
            }
            return dtNew;
        }


        /// <summary>
        /// 获取排名
        /// </summary>
        /// <param name="fuelCafcArray"></param>
        /// <returns></returns>
        private DataRow GetRanking(DataTable dt, out int ranking)
        {
            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                if (Utils.qymc == Convert.ToString(dt.Rows[i]["Qcscqy"]))
                {
                    ranking = i+1;
                    return dt.Rows[i];
                }
            }
            ranking = 0;
            return null;
        }


        //测试数据
        private DataTable RankingTestTable(string userName ,string passWord)
        {
            string flag = string.Empty;
            int fori = 0;
            DataTable dtNew = new DataTable();
            for (int i = startyear; i < year; i++)
            {
                var order = cafcService.GetProc_CAFC_TE_All(userName, passWord, userName.Substring(4, 1), "", i + "-01-01", i + "-12-04", "");
                DataTable dt = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order);
                dt.Columns.Add("year");
                dt.Columns.Add("jtcafc");  //目标值
                dt.Columns.Add("jtcafc1"); //达标值
                dt.Columns.Add("jcafc");   //实际值
                dt.Columns.Add("isYes");   //是否达标
                dt.Columns.Add("c1", typeof(decimal));   //排名值字段
                dt.Columns.Add("ranking"); //排名
                foreach (DataRow item in dt.Rows)
                {
                    item["c1"] = Math.Round((Convert.ToDecimal(item["Cafc"]) / Convert.ToDecimal(item["Tcafc1"]) * 100), 2, MidpointRounding.AwayFromZero);
                    if (Convert.ToDecimal(item["c1"]) <= Convert.ToDecimal(100.00))
                    {
                        item["isYes"] = "是";
                    }
                    else
                    {
                        item["isYes"] = "否";
                    }
                }
                DataRow[] drr = dt.Select("1=1", " c1 asc");
                DataTable Temp = new DataTable();
                Temp = drr.CopyToDataTable();
                int ranking = 0;


                if (dtNew.Rows.Count == 0)
                {
                    dtNew = Temp.Clone();
                }

                DataRow dr = GetRanking(Temp, out ranking);
                dtNew.Rows.Add(dr.ItemArray);
                var cafcDataNE = cafcService.QueryNECafc(userName, passWord, i + "-01-01", i + "-12-04");
                dtNew.Rows[fori]["year"] = i;
                dtNew.Rows[fori]["jtcafc"] = cafcDataNE[0].Tcafc;
                dtNew.Rows[fori]["jtcafc1"] = Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                dtNew.Rows[fori]["jcafc"] = cafcDataNE[0].Cafc;
                dtNew.Rows[fori]["ranking"] = ranking;
                fori++;
            }
            return dtNew;
        }

        //获取全部不计新能源数据
        //实计值除以达标值 计算排名正序
        //找到当前排名位置
        //取出后添加四列
        //添加到新表中
        private DataTable RankingNewTable()
        {
            string flag = string.Empty;
            int fori = 0;
            DataTable dtNew = new DataTable();
            for (int i = startyear; i < year; i++)
            {
                var order = cafcService.GetProc_CAFC_TE_All(Utils.userId, Utils.password, Utils.userId.Substring(4, 1), "", i + "-01-01", (i + 1) + "-01-01", "");
                DataTable dt = DataTableHelper.ToDataTable<CafcService.FuelCafcAndTcafcAndTcafc1>(order);
                dt.Columns.Add("year");
                dt.Columns.Add("jtcafc");  //目标值
                dt.Columns.Add("jtcafc1"); //达标值
                dt.Columns.Add("jcafc");   //实际值
                dt.Columns.Add("isYes");   //是否达标
                dt.Columns.Add("c1", typeof(decimal));   //排名值字段
                dt.Columns.Add("ranking"); //排名
                foreach (DataRow item in dt.Rows)
                {
                    if (string.IsNullOrEmpty(item["c1"].ToString()))
                    {
                        item["c1"] = "0.00";
                    }
                    else
                    {
                        item["c1"] = Math.Round((Convert.ToDecimal(item["Cafc"]) / Convert.ToDecimal(item["Tcafc1"]) * 100), 2, MidpointRounding.AwayFromZero);
                    }
                    if (Convert.ToDecimal(item["c1"]) <= Convert.ToDecimal(100.00))
                    {
                        item["isYes"] = "是";
                    }
                    else
                    {
                        item["isYes"] = "否";
                    }
                }
                DataRow[] drr = dt.Select("1=1", " c1 asc");
                DataTable Temp = new DataTable();
                Temp = drr.CopyToDataTable();
                int ranking = 0;

                if (dtNew.Rows.Count == 0)
                {
                    dtNew = Temp.Clone();
                }

                DataRow dr = GetRanking(Temp, out ranking);
                dtNew.Rows.Add(dr.ItemArray);
                var cafcDataNE = cafcService.QueryNECafc(Utils.userId, Utils.password, i + "-01-01", (i + 1) + "-01-01");
                dtNew.Rows[fori]["year"] = i;
                dtNew.Rows[fori]["jtcafc"] = cafcDataNE[0].Tcafc;
                dtNew.Rows[fori]["jtcafc1"] = Math.Round(cafcDataNE[0].Tcafc * Convert.ToDecimal(dictInit[i]), 2, MidpointRounding.AwayFromZero);
                dtNew.Rows[fori]["jcafc"] = cafcDataNE[0].Cafc;
                dtNew.Rows[fori]["ranking"] = ranking;
                fori++;
            }
            return dtNew;
        }


    }
}