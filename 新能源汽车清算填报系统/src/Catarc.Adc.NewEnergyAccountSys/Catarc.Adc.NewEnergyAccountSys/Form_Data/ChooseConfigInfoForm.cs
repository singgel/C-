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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using Catarc.Adc.NewEnergyAccountSys.ControlUtils;

namespace Catarc.Adc.NewEnergyAccountSys.Form_Data
{
    public partial class ChooseConfigInfoForm : DevExpress.XtraEditors.XtraForm
    {
        public Dictionary<string, string> mapCLXHData = new Dictionary<string, string>();
        public string[] mapdes = { "CLXH", "CSLY", "GGPC1", "DCDTXX_XH", "DCDTXX_SCQY", "DCZXX_XH", "DCZXX_ZRL", "DCZXX_SCQY"
                                    ,"QDDJXX_XH_1","QDDJXX_EDGL_1","QDDJXX_SCQY_1","RLDCXX_XH","RLDCXX_EDGL","RLDCXX_SCQY"};
        public ChooseConfigInfoForm()
        {
            InitializeComponent();
        }
        public ChooseConfigInfoForm(DataTable dt)
        {
            InitializeComponent();
            InitGrid(dt);
           
        }
        private void InitGrid(DataTable dt)
        {

            BandedGridView view = advBandedGridView1 as BandedGridView;

            //advBandedGridView1.BestFitColumns();
            view.BeginUpdate(); //开始视图的编辑，防止触发其他事件
            view.BeginDataUpdate(); //开始数据的编辑
            view.Bands.Clear();
            view.OptionsView.ColumnAutoWidth = true;
            //view.BestFitColumns();
            //修改附加选项
            view.OptionsView.ShowColumnHeaders = false;                         //因为有Band列了，所以把ColumnHeader隐藏
            view.OptionsView.ShowGroupPanel = false;                            //如果没必要分组，就把它去掉
            view.OptionsView.EnableAppearanceEvenRow = false;                   //是否启用偶数行外观
            view.OptionsView.EnableAppearanceOddRow = true;                     //是否启用奇数行外观
            view.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;   //是否显示过滤面板
            view.OptionsCustomization.AllowColumnMoving = false;                //是否允许移动列
            view.OptionsCustomization.AllowColumnResizing = true;              //是否允许调整列宽
            view.OptionsCustomization.AllowGroup = false;                       //是否允许分组
            view.OptionsCustomization.AllowFilter = true;                      //是否允许过滤
            view.OptionsCustomization.AllowSort = true;                         //是否允许排序
            view.OptionsSelection.EnableAppearanceFocusedCell = true;           //???
          //  view.OptionsBehavior.Editable = false;                               //是否允许用户编辑单元格
            view.OptionsView.ColumnAutoWidth = false;                           //是否显示水平滚动条
            view.OptionsView.ShowFooter = false;                                 //是否显示表底部
            view.OptionsView.AllowCellMerge = true;

            GridBand bandCheck = view.Bands.AddBand("选择");
           // bandCheck.MinWidth = 10;
            bandCheck.Width = 30;
            GridBand bandCLXH = view.Bands.AddBand("车辆型号");
            bandCLXH.MinWidth = 50;
            GridBand bandCSLY = view.Bands.AddBand("参数来源");
            bandCSLY.MinWidth = 30;
            //GridBand bandCPH = view.Bands.AddBand("配置号ID");
            //bandCPH.MinWidth = 75;
            GridBand bandGGPC = view.Bands.AddBand("公告批次");
            bandGGPC.MinWidth = 100;
            GridBand bandDCZ = view.Bands.AddBand("电池组（或超级电容）");
            GridBand bandDCZDTXH = bandDCZ.Children.AddBand("单体型号");
            bandDCZDTXH.MinWidth = 30;
            GridBand bandDCZDTSCQY = bandDCZ.Children.AddBand("单体生产企业");
            bandDCZDTSCQY.MinWidth = 120;
            GridBand bandDCZCXXH = bandDCZ.Children.AddBand("成箱型号");
            bandDCZCXXH.MinWidth = 30;
            GridBand bandDCZZNL = bandDCZ.Children.AddBand("电池组总能量（kWh）");
            bandDCZZNL.MinWidth = 150;
            GridBand bandDCZSCQY = bandDCZ.Children.AddBand("电池组生产企业");
            bandDCZSCQY.MinWidth = 120;
            GridBand bandQDDJ = view.Bands.AddBand("驱动电机");
            GridBand bandQDDJXH = bandQDDJ.Children.AddBand("型号");
            bandQDDJXH.MinWidth = 30;
            GridBand bandQDDJEDGL = bandQDDJ.Children.AddBand("额定功率(kW)");
            bandQDDJEDGL.MinWidth = 100;
            GridBand bandQDDJSCQY = bandQDDJ.Children.AddBand("生产企业");
            bandQDDJSCQY.MinWidth = 30;
            GridBand bandRLDC = view.Bands.AddBand("燃料电池");
            GridBand bandRLDCXH = bandRLDC.Children.AddBand("型号");
            bandRLDCXH.MinWidth = 30;
            GridBand bandRLDCEDGL = bandRLDC.Children.AddBand("额定功率(kW)");
            bandRLDCEDGL.MinWidth = 100;
            GridBand bandRLDCSCQY = bandRLDC.Children.AddBand("生产企业");
            bandRLDCSCQY.MinWidth = 30;



            bandCheck.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandCLXH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandCSLY.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //bandCPH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandGGPC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDCZ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDCZDTXH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDCZDTSCQY.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDCZCXXH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDCZZNL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandDCZSCQY.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandQDDJ.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandQDDJXH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandQDDJEDGL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandQDDJSCQY.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandRLDC.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandRLDCXH.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandRLDCEDGL.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            bandRLDCSCQY.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;


            dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
            dt.Columns["check"].ReadOnly = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["check"] = false;
            }

            gridControl1.DataSource = null;
            gridControl1.DataSource = dt;
            gridControl1.MainView.PopulateColumns();
            view.Columns["MODEL_VEHICLE"].OptionsColumn.ReadOnly = true;
            view.Columns["DATASOURCE"].OptionsColumn.ReadOnly = true;
            view.Columns["DATASOURCE"].OptionsColumn.AllowEdit = false;
            //view.Columns["CONFIG_ID"].OptionsColumn.ReadOnly = true;
            //view.Columns["CONFIG_ID"].Visible = false;
            view.Columns["BATCH"].OptionsColumn.ReadOnly = true;
            view.Columns["MODEL_SINGLE"].OptionsColumn.ReadOnly = true;
            view.Columns["MFRS_SINGLE"].OptionsColumn.ReadOnly = true;
            view.Columns["MODEL_WHOLE"].OptionsColumn.ReadOnly = true;
            view.Columns["CAPACITY_BAT"].OptionsColumn.ReadOnly = true;
            view.Columns["MFRS_BAT"].OptionsColumn.ReadOnly = true;
            view.Columns["MODEL_DRIVE"].OptionsColumn.ReadOnly = true;
            view.Columns["RATEPOW_DRIVE"].OptionsColumn.ReadOnly = true;
            view.Columns["MFRS_DRIVE"].OptionsColumn.ReadOnly = true;
            view.Columns["MDEL_FUEL"].OptionsColumn.ReadOnly = true;
            view.Columns["RATEPOW_FUEL"].OptionsColumn.ReadOnly = true;
            view.Columns["MFRS_FUEL"].OptionsColumn.ReadOnly = true;
            

            view.Columns["check"].OwnerBand = bandCheck;
            view.Columns["MODEL_VEHICLE"].OwnerBand = bandCLXH;
            view.Columns["DATASOURCE"].OwnerBand = bandCSLY;
            //view.Columns["CONFIG_ID"].OwnerBand = bandCPH;
            //view.Columns["CONFIG_ID"].VisibleIndex = -1;
            view.Columns["BATCH"].OwnerBand = bandGGPC;
            view.Columns["MODEL_SINGLE"].OwnerBand = bandDCZDTXH;
            view.Columns["MFRS_SINGLE"].OwnerBand = bandDCZDTSCQY;
            view.Columns["MODEL_WHOLE"].OwnerBand = bandDCZCXXH;
            view.Columns["CAPACITY_BAT"].OwnerBand = bandDCZZNL;
            view.Columns["MFRS_BAT"].OwnerBand = bandDCZSCQY;
            view.Columns["MODEL_DRIVE"].OwnerBand = bandQDDJXH;
            view.Columns["RATEPOW_DRIVE"].OwnerBand = bandQDDJEDGL;
            view.Columns["MFRS_DRIVE"].OwnerBand = bandQDDJSCQY;
            view.Columns["MDEL_FUEL"].OwnerBand = bandRLDCXH;
            view.Columns["RATEPOW_FUEL"].OwnerBand = bandRLDCEDGL;
            view.Columns["MFRS_FUEL"].OwnerBand = bandRLDCSCQY;

            
            view.EndDataUpdate();//结束数据的编辑
            view.EndUpdate();   //结束视图的编辑

            /*  int[] siz = new int[advBandedGridView1.Columns.Count];
              int z = 0*/
          /*  foreach (DevExpress.XtraGrid.Columns.GridColumn dc  in advBandedGridView1.Columns)
            {
                dc.OptionsColumn.
            }*/

        }

        private void btComfirm_Click(object sender, EventArgs e)
        {

            var dtExport = (DataTable)this.gridControl1.DataSource;
            string msg = string.Empty;
            if (dtExport == null || dtExport.Rows.Count < 1)
            {
                MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GridControlHelper.SelectedItems(this.advBandedGridView1).Rows.Count != 1)
            {
                MessageBox.Show("当前只能操作一条记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Object[] ItemArray = GridControlHelper.SelectedItems(this.advBandedGridView1).Rows[0].ItemArray;
            bool bRet = false;
            if (mapdes.Length+1 == ItemArray.Length)
            {
                for (int i = 0; i < mapdes.Length;i++ )
                {
                    mapCLXHData.Add(mapdes[i], ItemArray[i].ToString());
                    if (mapdes[i]== "RLDCXX_XH" ||mapdes[i]== "RLDCXX_EDGL" ||mapdes[i]== "RLDCXX_SCQY"  )
                    {
                        if (ItemArray[i].ToString() != "N/A" && ItemArray[i].ToString() != "" && !String.IsNullOrEmpty(ItemArray[i].ToString()))
                        {
                            bRet = true;
                        }
                    }
                }
                if (bRet)
                {
                    mapCLXHData.Add("CLSFYRLDC", "是");
                }
                else
                {
                    mapCLXHData.Add("CLSFYRLDC", "否");
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //修改列显示文本
        private void advBandedGridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DATASOURCE")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "gg":
                        e.DisplayText = "公告";
                        break;
                    case "nerds":
                        e.DisplayText = "推荐目录";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
        }
    }
}