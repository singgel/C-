using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_SJHS
{
    public partial class CafcTempCountForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        Dictionary<string, string> dicCafc = new Dictionary<string, string>();

        public CafcTempCountForm()
        {
            InitializeComponent();
            this.gridView2.OptionsView.ShowGroupPanel = false;
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
            InitGrid();
            InitDic();
            InitClxhText();
        }

        public void InitDic() 
        {
            dicCafc.Add("2016", "1.34");
            dicCafc.Add("2017", "1.28");
            dicCafc.Add("2018", "1.20");
            dicCafc.Add("2019", "1.10");
            dicCafc.Add("2020", "1");
        }

        /// <summary>
        /// 通用名称列表
        /// </summary>
        private void InitClxhText()
        {
            string sql = @"SELECT DISTINCT TYMC FROM OCN_CLJBXX WHERE OPERATION != 4";
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            cbRllx.Properties.Items.Clear();
            cbRllx.Properties.Items.Add("");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                cbRllx.Properties.Items.Add(row["TYMC"]);
            }
        }
        /// <summary>
        /// 初始化表格
        /// </summary>
        private void InitGrid()
        {
            BandedGridView view = CafcbandedGridView as BandedGridView;

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
            view.OptionsCustomization.AllowFilter = true;                      //是否允许过滤
            view.OptionsCustomization.AllowSort = true;                         //是否允许排序
            view.OptionsSelection.EnableAppearanceFocusedCell = true;           //???
            view.OptionsBehavior.Editable = false;                               //是否允许用户编辑单元格
            view.OptionsView.ColumnAutoWidth = false;                           //是否显示水平滚动条
            view.OptionsView.ShowFooter = true;                                 //是否显示表底部
            //view.OptionsView.AllowCellMerge = true;


            //添加列标题
            GridBand bandJbxx = view.Bands.AddBand("");
            GridBand bandNO = bandJbxx.Children.AddBand("NO");
            bandNO.MinWidth = 20;
            GridBand bandTymc = bandJbxx.Children.AddBand("通用名称");
            bandTymc.MinWidth = 60;
            GridBand bandPfbz = bandJbxx.Children.AddBand("排放标准");
            bandPfbz.MinWidth = 60;
            GridBand bandClxh = bandJbxx.Children.AddBand("车辆型号");
            bandClxh.MinWidth = 60;
            GridBand bandPl = bandJbxx.Children.AddBand("排量");
            bandPl.MinWidth = 60;
            GridBand bandZczbzl = bandJbxx.Children.AddBand("整车整备质量");
            bandZczbzl.MinWidth = 100;
            GridBand bandScbl = bandJbxx.Children.AddBand("生产比例");
            bandScbl.MinWidth = 60;
            GridBand bandScl = bandJbxx.Children.AddBand("生产量");
            bandScl.MinWidth = 60;
            GridBand bandHj = bandJbxx.Children.AddBand("合计");
            bandHj.MinWidth = 60;
            GridBand bandDyk = bandJbxx.Children.AddBand("DYK油耗申报值(L/100km)");
            bandDyk.MinWidth = 180;
            GridBand bandSjdbzmbz = bandJbxx.Children.AddBand("4阶段标准目标值(L/100km)");
            bandSjdbzmbz.MinWidth = 200;
            GridBand bandDycxyhdcl = bandJbxx.Children.AddBand("单一车型油耗达成率");
            bandDycxyhdcl.MinWidth = 150;

            GridBand bandCAFC = view.Bands.AddBand("企业平均燃料消耗量(CAFC)(L/100km)");
            bandCAFC.MinWidth = 160;
            //bandCAFC.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            GridBand bandDykScl = bandCAFC.Children.AddBand("DYK申报值×生产量");
            bandDykScl.MinWidth = 240;
            GridBand bandDykValues = bandCAFC.Children.AddBand("6.3109");
            bandDykValues.Caption = "";

            GridBand bandTCafc = view.Bands.AddBand("企业平均燃料消耗量目标值(TCAFC)(L/100km)");
            bandTCafc.MinWidth = 220;
            //bandTCafc.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            GridBand bandTSum = bandTCafc.Children.AddBand("4阶段目标值×生产量");
            bandTSum.MinWidth = 200;
            GridBand bandTcafcValues = bandTCafc.Children.AddBand("4.9429");
            bandTcafcValues.Caption = "";

            GridBand bandZczbzlAvg = view.Bands.AddBand("企业平均整备质量(kg)");
            bandZczbzlAvg.MinWidth = 220;
            //bandZczbzlAvg.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            GridBand bandDykSum = bandZczbzlAvg.Children.AddBand("DYK申报值×生产量");
            bandDykSum.MinWidth = 200;
            GridBand bandDykSumValue = bandZczbzlAvg.Children.AddBand("1284.714");
            bandDykSumValue.Caption = "";

            #region //列标题对齐方式
            bandJbxx.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandNO.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandTymc.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandPfbz.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandClxh.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandPl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandZczbzl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandScbl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandScl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandHj.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDyk.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandSjdbzmbz.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDycxyhdcl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandCAFC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykScl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykValues.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandTCafc.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandTSum.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandTcafcValues.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandZczbzlAvg.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykSum.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykSumValue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;


            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑

            
             #endregion
        }

       //数据源
        private DataTable QueryCafcData()
        {
            DataTable dtNew = new DataTable();
            //添加列
            dtNew.Columns.Add("NO");
            dtNew.Columns.Add("TYMC");
            dtNew.Columns.Add("PFBZ");
            dtNew.Columns.Add("CLXH");
            dtNew.Columns.Add("PL");
            dtNew.Columns.Add("ZCZBZL");
            dtNew.Columns.Add("SCBL");
            dtNew.Columns.Add("SCL", typeof(decimal));
            dtNew.Columns.Add("HJ", typeof(decimal));
            dtNew.Columns.Add("DYK_VALUES");
            dtNew.Columns.Add("T_CAFC");
            dtNew.Columns.Add("S_DCL");

            dtNew.Columns.Add("A_DYK", typeof(decimal));
            dtNew.Columns.Add("A_DYK_VALUES");

            dtNew.Columns.Add("T_DYK", typeof(decimal));
            dtNew.Columns.Add("T_DYK_VALUES");

            dtNew.Columns.Add("A_ZCZBZL", typeof(decimal));
            dtNew.Columns.Add("A_ZCZBZL_VALUES");

            for (int i = 1; i < 101; i++)
            {
                DataRow drNew = dtNew.NewRow();
                drNew["NO"] = i;
                drNew["TYMC"] = "TYMC" + i;
                drNew["PFBZ"] = "PFBZ" + i;
                drNew["CLXH"] = "CLXH" + i;
                drNew["PL"] = "PL" + i;
                drNew["ZCZBZL"] = "ZCZBZL" + i;
                drNew["SCBL"] = "SCBL" + i;
                drNew["SCL"] = i-1;
                drNew["HJ"] = i;
                drNew["DYK_VALUES"] = "DYK_VALUES" + i;
                drNew["T_CAFC"] = "T_CAFC" + i;
                drNew["S_DCL"] = "S_DCL" + i;

                drNew["A_DYK"] = i+1;
                drNew["T_DYK_VALUES"] = "T_DYK_VALUES" + i;

                drNew["T_DYK"] = i+2;
                drNew["T_DYK_VALUES"] = "";

                drNew["A_ZCZBZL"] = i+3;
                drNew["A_ZCZBZL_VALUES"] = "";
                dtNew.Rows.Add(drNew);
            }
            return dtNew;
        }

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonCount_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable dt= CountCafcData("","");
            this.gridControl1.DataSource = dt;
            this.gridView2.OptionsBehavior.Editable = false;
        }

        /// <summary>
        /// 汇总导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonCollect_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = "汇总信息",
                    Title = "导出Excel",
                    Filter = "Excel文件(*.xls)|*.xls"
                };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions();
                    options.SheetName = "汇总";
                    options.ShowGridLines = false;
                    options.TextExportMode = TextExportMode.Value;
                    this.gridControl1.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 明细导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonDetail_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = "明细信息",
                    Title = "导出Excel",
                    Filter = "Excel文件(*.xls)|*.xls"
                };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions();
                    options.SheetName = "明细";
                    options.ShowGridLines = false;
                    options.TextExportMode= TextExportMode.Value;
                    this.gridControl2.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchData_Click(object sender, EventArgs e)
        {
            InitGridCafcData();
        }

        //初始化表格
        private void InitGridCafcData()
        {
            // advBandedGridView1是表格上的默认视图，注意这里声明的是：BandedGridView
            BandedGridView view = CafcbandedGridView as BandedGridView;

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
            view.OptionsCustomization.AllowFilter = true;                      //是否允许过滤
            view.OptionsCustomization.AllowSort = true;                         //是否允许排序
            view.OptionsSelection.EnableAppearanceFocusedCell = true;           //???
            view.OptionsBehavior.Editable = false;                               //是否允许用户编辑单元格
            view.OptionsView.ColumnAutoWidth = false;                           //是否显示水平滚动条
            view.OptionsView.ShowFooter = true;                                 //是否显示表底部
            //view.OptionsView.AllowCellMerge = true;


            //添加列标题
            GridBand bandJbxx = view.Bands.AddBand("");
            GridBand bandNO = bandJbxx.Children.AddBand("NO");
            bandNO.MinWidth = 20;
            GridBand bandTymc = bandJbxx.Children.AddBand("通用名称");
            bandTymc.MinWidth = 60;
            GridBand bandPfbz = bandJbxx.Children.AddBand("排放标准");
            bandPfbz.MinWidth = 60;
            GridBand bandClxh = bandJbxx.Children.AddBand("车辆型号");
            bandClxh.MinWidth = 60;
            GridBand bandPl = bandJbxx.Children.AddBand("排量");
            bandPl.MinWidth = 60;
            GridBand bandZczbzl = bandJbxx.Children.AddBand("整车整备质量");
            bandZczbzl.MinWidth = 100;
            GridBand bandScbl = bandJbxx.Children.AddBand("生产比例");
            bandScbl.MinWidth = 60;
            GridBand bandScl = bandJbxx.Children.AddBand("生产量");
            bandScl.MinWidth = 60;
            GridBand bandHj = bandJbxx.Children.AddBand("合计");
            bandHj.MinWidth = 60;
            GridBand bandDyk = bandJbxx.Children.AddBand("DYK油耗申报值(L/100km)");
            bandDyk.MinWidth = 180;
            GridBand bandSjdbzmbz = bandJbxx.Children.AddBand("4阶段标准目标值(L/100km)");
            bandSjdbzmbz.MinWidth = 200;
            GridBand bandDycxyhdcl = bandJbxx.Children.AddBand("单一车型油耗达成率");
            bandDycxyhdcl.MinWidth = 150;

            GridBand bandCAFC = view.Bands.AddBand("企业平均燃料消耗量(CAFC)(L/100km)");
            bandCAFC.MinWidth = 220;
            //bandCAFC.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            GridBand bandDykScl = bandCAFC.Children.AddBand("DYK申报值×生产量");
            bandDykScl.MinWidth = 200;
            GridBand bandDykValues = bandCAFC.Children.AddBand("6.3109");
            bandDykValues.Caption = "";

            GridBand bandTCafc = view.Bands.AddBand("企业平均燃料消耗量目标值(TCAFC)(L/100km)");
            bandTCafc.MinWidth = 220;
            //bandTCafc.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            GridBand bandTSum = bandTCafc.Children.AddBand("4阶段目标值×生产量");
            bandTSum.MinWidth = 200;
            GridBand bandTcafcValues = bandTCafc.Children.AddBand("4.9429");
            bandTcafcValues.Caption = "";

            GridBand bandZczbzlAvg = view.Bands.AddBand("企业平均整备质量(kg)");
            bandZczbzlAvg.MinWidth = 180;
            //bandZczbzlAvg.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            GridBand bandDykSum = bandZczbzlAvg.Children.AddBand("DYK申报值×生产量");
            bandDykSum.MinWidth = 160;
            GridBand bandDykSumValue = bandZczbzlAvg.Children.AddBand("1284.714");
            bandDykSumValue.Caption = "";

            #region //列标题对齐方式
            bandJbxx.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandNO.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandTymc.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandPfbz.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandClxh.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandPl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandZczbzl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandScbl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandScl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandHj.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDyk.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandSjdbzmbz.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDycxyhdcl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandCAFC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykScl.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykValues.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandTCafc.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandTSum.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandTcafcValues.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            bandZczbzlAvg.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykSum.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDykSumValue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;


            //绑定数据源并显示
            DataTable dt = QueryCafcData(this.dtStartTime.Text, this.dtEndTime.Text, cbRllx.Text);
            gridControl2.DataSource = null;
            gridControl2.DataSource = dt;
            gridControl2.MainView.PopulateColumns();
            #endregion

            #region  //将标题列和数据列对应
            view.Columns["NO"].OwnerBand = bandNO;
            view.Columns["TYMC"].OwnerBand = bandTymc;
            view.Columns["PFBZ"].OwnerBand = bandPfbz;
            view.Columns["CLXH"].OwnerBand = bandClxh;
            view.Columns["PL"].OwnerBand = bandPl;
            view.Columns["ZCZBZL"].OwnerBand = bandZczbzl;
            view.Columns["SCBL"].OwnerBand = bandScbl;
            view.Columns["SCL"].OwnerBand = bandScl;
            view.Columns["HJ"].OwnerBand = bandHj;
            //bandHj.View.CellMerge += new DevExpress.XtraGrid.Views.Grid.CellMergeEventHandler(View_CellMerge);
            view.Columns["ACT_ZHGKRLXHL"].OwnerBand = bandDyk;
            view.Columns["TGT_ZHGKRLXHL"].OwnerBand = bandSjdbzmbz;
            view.Columns["DYCXYHDCL"].OwnerBand = bandDycxyhdcl;

            view.Columns["SUM_AVGCAFC"].OwnerBand = bandDykScl;
            view.Columns["SUM_AVGCAFC_VALUES"].OwnerBand = bandDykValues;

            view.Columns["SUM_TGTCAFC"].OwnerBand = bandTSum;
            view.Columns["SUM_TGTCAFC_VALUES"].OwnerBand = bandTcafcValues;

            view.Columns["SUM_ZCZBZL"].OwnerBand = bandDykSum;
            view.Columns["SUM_ZCZBZL_VALUES"].OwnerBand = bandDykSumValue;

            //汇总信息
            view.Columns["SCL"].SummaryItem.FieldName = "SCL";
            view.Columns["SCL"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["HJ"].SummaryItem.FieldName = "HJ";
            view.Columns["HJ"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["SUM_AVGCAFC"].SummaryItem.FieldName = "SUM_AVGCAFC";
            view.Columns["SUM_AVGCAFC"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["SUM_TGTCAFC"].SummaryItem.FieldName = "SUM_TGTCAFC";
            view.Columns["SUM_TGTCAFC"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["SUM_ZCZBZL"].SummaryItem.FieldName = "SUM_ZCZBZL";
            view.Columns["SUM_ZCZBZL"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            //view.Columns["A_ZCZBZL"].SummaryItem.DisplayFormat = "总数：{0}";

            //view.BestFitColumns();
            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑

            #endregion
        }


        void View_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 查询数据库获取计算数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private DataTable QueryCafcData(string startTime, string endTime, string clxh) 
        {
            string year = DateTime.Parse(this.dtStartTime.Text).Year.ToString();
            double t1 = 0, t2 = 0;
            if (year == "2016" || year == "2017") 
            {
                t1 = 5; t2 = 3.5;
            }
            else if (year == "2016" || year == "2017")
            {
                t1 = 3; t2 = 2.5;
            }else 
            {
                t1 = 2; t2 = 1.5;
            }
            string sqlTYMC = @"(SELECT  BC.TYMC, SUM(BC.HJ) AS SUM_TYMC "
                         + "  FROM "
                         + "  (SELECT BA.RLLX, BA.TYMC, BA.PFBZ, BA.CLXH, BA.PL, BA.ZCZBZL, "
                         + "        BA.ACT_ZHGKRLXHL, BA.TGT_ZHKGRLXHL AS TGT_ZHGKRLXHL, BA.SCL, "
                         + "        CASE WHEN (BA.RLLX='纯电动' OR BA.RLLX='燃料电池' OR (BA.RLLX='插电式混合动力' AND BA.ZHGKXSLC>=50)) THEN BA.SCL * " + t1 + " "
                         + "        WHEN BA.RLLX='插电式混合动力' AND BA.ZHGKXSLC < 50 AND ACT_ZHGKRLXHL<=2.8 THEN BA.SCL * " + t2 + " "
                         + "        WHEN (BA.RLLX='汽油' OR BA.RLLX='柴油'  OR BA.RLLX='两用燃料'  OR BA.RLLX='双燃料' OR BA.RLLX='非插电式混合动力') "
                         + "            AND BA.ACT_ZHGKRLXHL<=2.8 THEN BA.SCL * " + t2 + " "
                         + "            ELSE BA.SCL END HJ "
                         + "        FROM "
                         + "    (SELECT RLLX, TYMC, PFBZ, CLXH, PL, ZCZBZL, ACT_ZHGKRLXHL, TGT_ZHKGRLXHL, ZHGKXSLC, SUM(CVIN) AS SCL FROM "
                         + "        (SELECT VIN, CLZZRQ, RLLX, TYMC, PFBZ, CLXH, PL, nvl(ZCZBZL,0) AS ZCZBZL,  "
                         + "          TGT_ZHKGRLXHL, ACT_ZHGKRLXHL, nvl(ZHGKXSLC,0) AS ZHGKXSLC, 1 AS CVIN FROM VIEW_VIN_INFO "
                         + "        WHERE 1=1 AND TYMC LIKE '%" + clxh + "%' "
                         + "            AND CLZZRQ>=to_date('" + startTime + "','yyyy-mm-dd hh24:mi:ss') "
                         + "            AND CLZZRQ<to_date('" + endTime + "','yyyy-mm-dd hh24:mi:ss') "
                         + "            AND MERGER_STATUS=1) T0 "
                         + "    GROUP BY RLLX, TYMC, PFBZ, CLXH, PL, ZCZBZL, ACT_ZHGKRLXHL, TGT_ZHKGRLXHL, ZHGKXSLC) BA) BC "
                         + " GROUP BY BC.TYMC) TC";

            string sql = @"SELECT  rownum as NO, BC.RLLX, BC.TYMC, BC.PFBZ, BC.CLXH, BC.PL, BC.ZCZBZL, BC.ACT_ZHGKRLXHL, BC.TGT_ZHGKRLXHL, BC.SCL, BC.HJ, "
                         + "        ROUND((BC.SCL/TC.SUM_TYMC)*100,2)||'%' AS SCBL, ROUND((BC.ACT_ZHGKRLXHL/BC.TGT_ZHGKRLXHL)*100,2)||'%' AS DYCXYHDCL, "
                         + "        ROUND(BC.SCL*BC.ACT_ZHGKRLXHL,1) AS SUM_AVGCAFC, '' AS SUM_AVGCAFC_VALUES, "
                         + "        ROUND(BC.SCL*BC.TGT_ZHGKRLXHL,1) AS SUM_TGTCAFC, '' AS SUM_TGTCAFC_VALUES, "
                         + "        ROUND(BC.SCL*BC.ZCZBZL,1) AS SUM_ZCZBZL , '' AS SUM_ZCZBZL_VALUES  "
                         + "  FROM "
                         + "  (SELECT BA.RLLX, BA.TYMC, BA.PFBZ, BA.CLXH, BA.PL, BA.ZCZBZL, "
                         + "        BA.ACT_ZHGKRLXHL, BA.TGT_ZHKGRLXHL AS TGT_ZHGKRLXHL, BA.SCL, "
                         + "        CASE WHEN (BA.RLLX='纯电动' OR BA.RLLX='燃料电池' OR (BA.RLLX='插电式混合动力' AND BA.ZHGKXSLC>=50)) THEN BA.SCL * " + t1 + " "
                         + "        WHEN BA.RLLX='插电式混合动力' AND BA.ZHGKXSLC < 50 AND ACT_ZHGKRLXHL<=2.8 THEN BA.SCL * " + t2 + " "
                         + "        WHEN (BA.RLLX='汽油' OR BA.RLLX='柴油'  OR BA.RLLX='两用燃料'  OR BA.RLLX='双燃料' OR BA.RLLX='非插电式混合动力') "
                         + "            AND BA.ACT_ZHGKRLXHL<=2.8 THEN BA.SCL * " + t2 + " "
                         + "            ELSE BA.SCL END HJ "
                         + "        FROM "
                         + "    (SELECT RLLX, TYMC, PFBZ, CLXH, PL, ZCZBZL, ACT_ZHGKRLXHL, TGT_ZHKGRLXHL, ZHGKXSLC, SUM(CVIN) AS SCL FROM "
                         + "        (SELECT VIN, CLZZRQ, RLLX, TYMC, PFBZ, CLXH, PL, nvl(ZCZBZL,0) AS ZCZBZL,  "
                         + "          TGT_ZHKGRLXHL, ACT_ZHGKRLXHL, nvl(ZHGKXSLC,0) AS ZHGKXSLC, 1 AS CVIN FROM VIEW_VIN_INFO "
                         + "        WHERE 1=1 AND TYMC LIKE '%" + clxh + "%' "
                         + "            AND CLZZRQ>=to_date('" + startTime + "','yyyy-mm-dd hh24:mi:ss') "
                         + "            AND CLZZRQ<to_date('" + endTime + "','yyyy-mm-dd hh24:mi:ss') "
                         + "            AND MERGER_STATUS=1) T0 "
                         + "    GROUP BY RLLX, TYMC, PFBZ, CLXH, PL, ZCZBZL, ACT_ZHGKRLXHL, TGT_ZHKGRLXHL, ZHGKXSLC) BA) BC, "
                         + sqlTYMC
                         + " WHERE  BC.TYMC = TC.TYMC";
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 合并单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CafcbandedGridView_CellMerge(object sender, DevExpress.XtraGrid.Views.Grid.CellMergeEventArgs e)
        {
            //int row1 = e.RowHandle1;
            //int row2 = e.RowHandle2;
            //string value1 = this.CafcbandedGridView.GetDataRow(row1)["TYMC"].ToString();
            //string value2 = this.CafcbandedGridView.GetDataRow(row2)["TYMC"].ToString();
            //if (e.Column.FieldName == "HJ") 
            //{
            //    if (value1 != value2)
            //    {
            //        e.Handled = true;
            //    }
            //}
            //else
            //{
            //    e.Merge = false; 
            //    e.Handled = true;
            //}
            
        }

        /// <summary>
        /// 计算cafc
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private DataTable CountCafcData(string startTime, string endTime)
        {
            DataTable dtNew = new DataTable();
            string year = DateTime.Parse(this.dtStartTime.Text).Year.ToString();
            DataTable dt = QueryCafcData(this.dtStartTime.Text, this.dtEndTime.Text, cbRllx.Text);
            if (dt.Rows.Count > 0)
            {
                double sum_scl = double.Parse(dt.Compute("sum(SCL)", "").ToString());
                double sum_hj = double.Parse(dt.Compute("sum(HJ)", "").ToString());
                double sum_acafc = double.Parse(dt.Compute("sum(SUM_AVGCAFC)", "").ToString());
                double sum_tcafc = double.Parse(dt.Compute("sum(SUM_TGTCAFC)", "").ToString());
                double sum_zczbzl = double.Parse(dt.Compute("sum(SUM_ZCZBZL)", "").ToString());
                double cafc = sum_acafc / sum_hj;
                double tcafc = sum_tcafc / sum_scl;
                double avg_zczbzl = sum_zczbzl / sum_scl;

                double tcafc_1 = Math.Round(Math.Round(tcafc, 2) * float.Parse(dicCafc[year]), 2);
                //添加列
                dtNew.Columns.Add("SCL");
                dtNew.Columns.Add("CAFC");
                dtNew.Columns.Add("TCAFC");
                dtNew.Columns.Add("TCAFC_1");
                dtNew.Columns.Add("T1");
                dtNew.Columns.Add("T2");

                DataRow drNew = dtNew.NewRow();
                drNew["SCL"] = sum_scl;
                drNew["CAFC"] = Math.Round(cafc, 2); ;
                drNew["TCAFC"] = Math.Round(tcafc, 2);
                drNew["TCAFC_1"] = tcafc_1;
                drNew["T1"] = Math.Round((cafc / tcafc_1) * 100, 1) + "%";
                drNew["T2"] = Math.Round((cafc / Math.Round(tcafc, 2)) * 100, 1) + "%";
                dtNew.Rows.Add(drNew);
                updateTableHead(cafc, tcafc, avg_zczbzl);
            }
            return dtNew;
        }

        private void updateTableHead(double cafc, double tcafc, double zczbzl)
        {
            BandedGridView view = CafcbandedGridView as BandedGridView;
            view.Bands[1].Children[1].Caption = Math.Round(cafc, 2).ToString();
            view.Bands[2].Children[1].Caption = Math.Round(tcafc, 2).ToString();
            view.Bands[3].Children[1].Caption = Math.Round(zczbzl, 2).ToString();
        }
    }
}