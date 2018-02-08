using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Base;
using FuelDataSysClient.Tool;
using System.Threading;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.FuelCafc;

namespace FuelDataSysClient.Form_SJTJ
{
    public partial class AvgFuelDetailForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public AvgFuelDetailForm()
        {
            InitializeComponent();
            // 设置燃料类型下拉框的值
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
            if (!Utils.IsFuelTest)
            {
                this.gcDetail.DataSource = DataSourceHelper.AvgFuelDetailData();
                this.lblNum.Text = string.Format("共{0}条", DataSourceHelper.AvgFuelDetailData().Rows.Count);
                btnSearch.Enabled = false;
            }
        }

        //导出
        private void btnExportDetail_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = "油耗参数明细页面数据",
                    Title = "导出Excel",
                    Filter = "Excel文件(*.xls)|*.xls"
                };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };
                    this.gcDetail.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 验证查询时间
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
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                CafcService.FuelCAFCDetails[] detailData = Utils.serviceCafc.QueryCalParamDetails(Utils.userId, Utils.password, this.cbRllx.Text.Trim(), this.txtclxh.Text.Trim(), this.dtStartTime.Text, this.dtEndTime.Text);
                if (detailData != null)
                {
                    this.gcDetail.DataSource = detailData;
                    this.lblNum.Text = string.Format("共{0}条", detailData.Length);
                }
                this.gvDetail.BestFitColumns();
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

        //得到焦点处的油耗明细
        private void rhlVin_Click(object sender, EventArgs e)
        {
            try
            {
                string startTime = this.dtStartTime.Text.Trim();
                string endTime = this.dtEndTime.Text.Trim();
                ColumnView cv = (ColumnView)gcDetail.FocusedView;
                CafcService.FuelCAFCDetails fuelDetail = (CafcService.FuelCAFCDetails)cv.GetFocusedRow();
                if (fuelDetail != null)
                {
                    using (VinDetailForm vinFrm = new VinDetailForm(startTime, endTime, fuelDetail))
                    {
                        vinFrm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}   