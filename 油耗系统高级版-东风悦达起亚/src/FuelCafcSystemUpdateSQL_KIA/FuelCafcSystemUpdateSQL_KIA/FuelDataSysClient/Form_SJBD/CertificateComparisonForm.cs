using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Common;
using DevExpress.XtraGrid;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraPrinting;
using FuelDataSysClient.Tool;
using System.Threading;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Form_SJSB;

namespace FuelDataSysClient.Form_SJBD
{
    public partial class CertificateComparisonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        readonly CertificateService.CertificateComparison ccf = Utils.serviceCertificate;

        public CertificateComparisonForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //查询官方油耗数据
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "系统提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    this.gridControl1.DataSource = null;
                    this.groupBox1.Text = "系统油耗数据";
                    StringBuilder sqlStr = new StringBuilder();
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format("select VIN,TYMC,CLXH,CLZZRQ,RLLX from fc_cljbxx_adc where status='0' and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >=to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <=to_date('{1}','yyyy-mm-dd hh24:mi:ss')", dtStartTime.Text, Convert.ToDateTime(dtEndTime.Text)),null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        this.gridControl1.DataSource = dt;
                        this.groupBox1.Text = String.Format("系统油耗数据（共{0}条）", ds.Tables[0].Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception msg)
                {
                    MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //查询合格证油耗数据
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "合格证数据";
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //测试用户 测试数据
                if (!Utils.IsFuelTest)
                {
                    this.gridControl2.DataSource = DataSourceHelper.CertificateData();
                }
                else
                {
                    var ds = ccf.QueryCertificate(Utils.userId, Utils.password, Utils.qymc, dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].Columns["vin"].ColumnName = "VIN";
                        ds.Tables[0].Columns["clxh"].ColumnName = "CLXH";
                        ds.Tables[0].Columns["clzzrq"].ColumnName = "CLZZRQ";
                        ds.Tables[0].Columns["rllx"].ColumnName = "RLLX";
                        this.gridControl2.DataSource = ds.Tables[0];
                        this.groupBox2.Text = String.Format("合格证数据（共{0}条）", ds.Tables[0].Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("无合格证数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }

        }

        //全选
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                var view = gridControl.MainView;
                Utils.SelectItem(view, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //取消全选
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                var view = gridControl.MainView;
                Utils.SelectItem(view, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //导出到Excel
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gcTable1.DataSource == null && gcTable2.DataSource == null)
            {
                MessageBox.Show("没有比对结果", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "合格证数据比对结果",
                Title = "导出Excel",
                Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls"
            };
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    ExportHelper.ExportToExcel(saveFileDialog.FileName, true, String.Empty, gcTable1, gcTable2);
                    ExcelHelper excelBuilder = new ExcelHelper(saveFileDialog.FileName);
                    excelBuilder.ChangeNameWorkSheet("Sheet1", "补传数据");
                    excelBuilder.ChangeNameWorkSheet("Sheet2", "撤销数据");
                    excelBuilder.DeleteRows(1, 1);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.SaveFile();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //数据比对
        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fuelData = (DataTable)this.gridControl1.DataSource;
            var CertificateData = (DataTable)this.gridControl2.DataSource;
            if (fuelData == null || CertificateData == null)
            {
                MessageBox.Show("没有需要比较的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //补传数据
                var vinArr_F = fuelData.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                var supplementsData = from d in CertificateData.AsEnumerable()
                                      where !vinArr_F.Contains(d.Field<string>("VIN"))
                                      select d;
                DataTable dtSupplement = supplementsData.AsDataView().ToTable();
                //撤销数据
                var vinArr_HGZ = CertificateData.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                var recallData = from d in fuelData.AsEnumerable()
                                 where !vinArr_HGZ.Contains(d.Field<string>("VIN"))
                                 select d;
                DataTable dtRecall = recallData.AsDataView().ToTable();
                this.gcTable1.DataSource = dtSupplement;
                this.gcTable2.DataSource = dtRecall;
                //比较完初始选中的tab页
                int selectTabIndex = 1;
                if (dtSupplement.Rows.Count < 1)
                {
                    selectTabIndex = 2;
                    if (dtRecall.Rows.Count < 1)
                    {
                        MessageBox.Show("数据一致！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                xtraTabControl1.SelectedTabPageIndex = selectTabIndex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView5_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}