using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;

namespace FuelDataSysClient.Form_SJHS
{
    public partial class CAFCTempForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CAFCTempForm()
        {
            InitializeComponent();
            this.gridView2.OptionsView.ShowGroupPanel = false;
            InitGridData();
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
            
        }

        //初始化表格
        private void InitGridData()
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
            

            //添加列标题
            GridBand bandJbxx = view.Bands.AddBand("");
            GridBand bandNO = bandJbxx.Children.AddBand("NO");
            GridBand bandTymc = bandJbxx.Children.AddBand("通用名称");
            GridBand bandPfbz = bandJbxx.Children.AddBand("排放标准");
            GridBand bandClxh = bandJbxx.Children.AddBand("车辆型号");
            GridBand bandPl = bandJbxx.Children.AddBand("排量");
            GridBand bandZczbzl = bandJbxx.Children.AddBand("整车整备质量");
            GridBand bandScbl = bandJbxx.Children.AddBand("生产比例");
            GridBand bandScl = bandJbxx.Children.AddBand("生产量");
            GridBand bandHj = bandJbxx.Children.AddBand("合计");
            GridBand bandDyk= bandJbxx.Children.AddBand("DYK油耗申报值(L/100km)");
            GridBand bandSjdbzmbz= bandJbxx.Children.AddBand("4阶段标准目标值(L/100km)");
            GridBand bandDycxyhdcl = bandJbxx.Children.AddBand("单一车型油耗达成率");

            GridBand bandCAFC = view.Bands.AddBand("企业平均燃料消耗量(CAFC)(L/100km)");
            GridBand bandDykScl = bandCAFC.Children.AddBand("DYK申报值×生产量");
            GridBand bandDykValues = bandCAFC.Children.AddBand("6.3109");
            bandDykValues.Caption = "1234";

            GridBand bandTCafc = view.Bands.AddBand("企业平均燃料消耗量目标值(TCAFC)(L/100km)");
            GridBand bandTSum = bandTCafc.Children.AddBand("4阶段目标值×生产量");
            GridBand bandTcafcValues = bandTCafc.Children.AddBand("4.9429");
            bandTcafcValues.Caption = "1234";

            GridBand bandZczbzlAvg = view.Bands.AddBand("企业平均整备质量(kg)");
            GridBand bandDykSum = bandZczbzlAvg.Children.AddBand("DYK申报值×生产量");
            GridBand bandDykSumValue = bandZczbzlAvg.Children.AddBand("1284.714");
            bandTcafcValues.Caption = "1234";

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
            DataTable dt = QueryCafcData();
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
            view.Columns["DYK_VALUES"].OwnerBand = bandDyk;
            view.Columns["T_CAFC"].OwnerBand = bandSjdbzmbz;
            view.Columns["S_DCL"].OwnerBand = bandDycxyhdcl;

            view.Columns["A_DYK"].OwnerBand = bandDykScl;
            view.Columns["A_DYK_VALUES"].OwnerBand = bandDykValues;

            view.Columns["T_DYK"].OwnerBand = bandTSum;
            view.Columns["T_DYK_VALUES"].OwnerBand = bandTcafcValues;

            view.Columns["A_ZCZBZL"].OwnerBand = bandDykSum;
            view.Columns["A_ZCZBZL_VALUES"].OwnerBand = bandDykSumValue;

            //汇总信息
            view.Columns["SCL"].SummaryItem.FieldName = "SCL";
            view.Columns["SCL"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["HJ"].SummaryItem.FieldName = "HJ";
            view.Columns["HJ"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["A_DYK"].SummaryItem.FieldName = "A_DYK";
            view.Columns["A_DYK"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["T_DYK"].SummaryItem.FieldName = "T_DYK";
            view.Columns["T_DYK"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;

            view.Columns["A_ZCZBZL"].SummaryItem.FieldName = "A_ZCZBZL";
            view.Columns["A_ZCZBZL"].SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
            //view.Columns["A_ZCZBZL"].SummaryItem.DisplayFormat = "总数：{0}";

            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑
            //view.BestFitColumns();
             #endregion
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            InitGridData();
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

        private void btnSearch_Click(object sender, EventArgs e)
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
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };
                    this.gridControl2.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}