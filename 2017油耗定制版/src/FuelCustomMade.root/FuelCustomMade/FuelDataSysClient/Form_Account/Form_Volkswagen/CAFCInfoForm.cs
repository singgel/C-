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
using System.Threading;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Account.Form_Volkswagen
{
    public partial class CAFCInfoForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public CAFCInfoForm()
        {
            InitializeComponent();
        }

        private void CAFCInfoForm_Load(object sender, EventArgs e)
        {
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("ZBZL");
            dt1.Columns.Add("ZWPS2");
            dt1.Columns.Add("ZWPS3");
            dt1.Rows.Add(new object[] { "CM≤750", "4.3", "4.5" });
            dt1.Rows.Add(new object[] { "750<CM≤865", "4.3", "4.5" });
            dt1.Rows.Add(new object[] { "865<CM≤980", "4.3", "4.5" });
            dt1.Rows.Add(new object[] { "980<CM≤1090", "4.5", "4.7" });
            dt1.Rows.Add(new object[] { "1090<CM≤1205", "4.7", "4.9" });
            dt1.Rows.Add(new object[] { "1205<CM≤1320", "4.9", "5.1" });
            dt1.Rows.Add(new object[] { "1320<CM≤1430", "5.1", "5.3" });
            dt1.Rows.Add(new object[] { "1430<CM≤1540", "5.3", "5.5" });
            dt1.Rows.Add(new object[] { "1540<CM≤1660", "5.5", "5.7" });
            dt1.Rows.Add(new object[] { "1660<CM≤1770", "5.7", "5.9" });
            dt1.Rows.Add(new object[] { "1770<CM≤1880", "5.9", "6.1" });
            dt1.Rows.Add(new object[] { "1880<CM≤2000", "6.2", "6.4" });
            dt1.Rows.Add(new object[] { "2000<CM≤2110", "6.4", "6.6" });
            dt1.Rows.Add(new object[] { "2110<CM≤2280", "6.6", "6.8" });
            dt1.Rows.Add(new object[] { "2280<CM≤2510", "7", "7.2" });
            dt1.Rows.Add(new object[] { "2510<CM", "7.3", "7.5" });
            this.gridControl1.DataSource = dt1;
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("YEAR");
            dt2.Columns.Add("RATIO");
            dt2.Rows.Add(new object[] { "2016年", "5" });
            dt2.Rows.Add(new object[] { "2017年", "5" });
            dt2.Rows.Add(new object[] { "2018年", "3" });
            dt2.Rows.Add(new object[] { "2019年", "3" });
            dt2.Rows.Add(new object[] { "2020年", "2" });
            this.gridControl2.DataSource = dt2;
            DataTable dt3 = new DataTable();
            dt3.Columns.Add("YEAR");
            dt3.Columns.Add("RATIO");
            dt3.Rows.Add(new object[] { "2016年", "3.5" });
            dt3.Rows.Add(new object[] { "2017年", "3.5" });
            dt3.Rows.Add(new object[] { "2018年", "2.5" });
            dt3.Rows.Add(new object[] { "2019年", "2.5" });
            dt3.Rows.Add(new object[] { "2020年", "1.5" });
            this.gridControl3.DataSource = dt3;
            DataTable dt4 = new DataTable();
            dt4.Columns.Add("YEAR");
            dt4.Columns.Add("RATIO");
            dt4.Rows.Add(new object[] { "2016年", "134%" });
            dt4.Rows.Add(new object[] { "2017年", "128%" });
            dt4.Rows.Add(new object[] { "2018年", "120%" });
            dt4.Rows.Add(new object[] { "2019年", "110%" });
            dt4.Rows.Add(new object[] { "2020年", "100%" });
            this.gridControl4.DataSource = dt4;
        }

        private void barBtnExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    ExportToExcel toExcel = new ExportToExcel();
                    DataSet ds = new DataSet();
                    if (gridControl1.DataSource != null)
                    {
                        DataTable dt1 = (DataTable)gridControl1.DataSource;
                        dt1.TableName = "车型油耗目标值";
                        DataTable dtc1 = dt1.Copy();
                        dtc1.Columns["ZBZL"].ColumnName = "整备质量/kg";
                        dtc1.Columns["ZWPS2"].ColumnName = "座位排数≤2/(L/100km)";
                        dtc1.Columns["ZWPS3"].ColumnName = "座位排数≥3/(L/100km)";
                        ds.Tables.Add(dtc1);
                    }
                    if (gridControl2.DataSource != null)
                    {
                        DataTable dt2 = (DataTable)gridControl2.DataSource;
                        dt2.TableName = "新能源车型核算倍数";
                        DataTable dtc2 = dt2.Copy();
                        dtc2.Columns["YEAR"].ColumnName = "年份";
                        dtc2.Columns["RATIO"].ColumnName = "核算倍数";
                        ds.Tables.Add(dtc2);
                    }
                    if (gridControl3.DataSource != null)
                    {
                        DataTable dt3 = (DataTable)gridControl3.DataSource;
                        dt3.TableName = "节能车核算倍数";
                        DataTable dtc3 = dt3.Copy();
                        dtc3.Columns["YEAR"].ColumnName = "年份";
                        dtc3.Columns["RATIO"].ColumnName = "核算倍数";
                        ds.Tables.Add(dtc3);
                    }
                    if (gridControl4.DataSource != null)
                    {
                        DataTable dt4 = (DataTable)gridControl4.DataSource;
                        dt4.TableName = "企业平均燃料消耗量要求";
                        DataTable dtc4 = dt4.Copy();
                        dtc4.Columns["YEAR"].ColumnName = "年份";
                        dtc4.Columns["RATIO"].ColumnName = "Phase in系数";
                        ds.Tables.Add(dtc4);
                    }
                    toExcel.ToExcelSheet(ds, saveFileDialog.FileName);
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }
    }
}
