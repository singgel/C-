using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraGrid.Views.Base;
using FuelDataSysClient.Tool;
using DevExpress.XtraPrinting;
using System.Net;
using System.Threading;
using FuelDataModel;
using System.Reflection;
using FuelDataSysClient.Tool.Tool_Porsche;
using FuelDataSysClient.FuelCafc;

namespace FuelDataSysClient.Form_Statistics
{
    public partial class FuelDataUploadTotalForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;

        public FuelDataUploadTotalForm()
        {
            InitializeComponent();
            if (!Utils.IsFuelTest)
            {
                var data = TestDataInit();
                this.gcStatistic.DataSource = data;
                this.lblNum.Text = string.Format("共{0}条", data.Rows.Count);
                btnSearch.Enabled = false;
                barButtonItem1.Enabled = false;
            }
        }

        private void FuelDataUploadTotalForm_Load(object sender, EventArgs e)
        {
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        /// <summary>
        /// 单击显示统计数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            string startTime = this.dtStartTime.Text.Trim();
            string endTime = this.dtEndTime.Text.Trim();

            if (string.IsNullOrEmpty(startTime))
            {
                msg += "统计开始时间不能为空\r\n";
            }
            if (string.IsNullOrEmpty(startTime))
            {
                msg += "统计结束时间不能为空\r\n";
            }

            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg);
                return;
            }

            msg = this.DataBinds(startTime, endTime);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg);
            }
        }

        /// <summary>
        /// 单击链接显示VIN数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rhlVin_Click(object sender, EventArgs e)
        {
            this.GetLinkedVins();
        }

        /// <summary>
        /// 双击行显示VIN数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gcStatistic_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.GetLinkedVins();
        }

        /// <summary>
        /// 显示统计数据
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string DataBinds(string startTime, string endTime)
        {
            string msg = string.Empty;
            try
            {
                CafcService.StatisticsData[] staData = this.GetStatisticData(startTime, endTime);

                if (staData != null)
                {
                    this.gcStatistic.DataSource = staData;
                    this.gvStatistic.BestFitColumns();
                    this.lblNum.Text = string.Format("共{0}条", staData.Length);
                }
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        /// <summary>
        /// 获取统计信息
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private CafcService.StatisticsData[] GetStatisticData(string startTime, string endTime)
        {
            DataTable dt = new DataTable();
            CafcService.StatisticsData[] staData = null;

            try
            {
                staData = cafcService.QueryStatisticsData(Utils.userId, Utils.password, "", "", startTime, endTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return staData;
        }

        /// <summary>
        /// 显示VIN数据
        /// </summary>
        private void GetLinkedVins()
        {
            try
            {
                ColumnView cv = (ColumnView)gcStatistic.FocusedView;
                CafcService.StatisticsData statData = (CafcService.StatisticsData)cv.GetFocusedRow();

                if (statData != null)
                {
                    VinDetailForm vinFrm = new VinDetailForm(statData);
                    vinFrm.ShowDialog();
                }
            }
            catch (Exception)
            {
            }

        }

        private DataTable TestDataInit()
        {
            Dictionary<string,string> data = new Dictionary<string,string>();
            data.Add("Clxh", "JJEIDXE");
            data.Add("Clzzrq",DateTime.Now.AddDays(-99).ToString("yyyy-MM-dd"));
            data.Add("Sl", "2024");
            return Common.DataTableHelper.FillDataTable( data, 20);
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Utils.userId.Equals("FADCFBSJ0U001"))
            {
                this.exportToExcel_Porsche();
            }
            else
            {
                this.exportToExcel();
            }
        }

        // 导出Excel
        private void exportToExcel()
        {
            try
            {
                if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };
                    this.gcStatistic.ExportToXls(saveFileDialog.FileName, options);
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 保时捷——导出Excel
        private void exportToExcel_Porsche()
        {
            Thread newThread = new Thread(extractDataAndExport);
            newThread.ApartmentState = ApartmentState.STA;
            newThread.Start();
        }

        // 保时捷——导出Excel
        private void extractDataAndExport()
        {
            ProcessForm pf = new ProcessForm();
            pf.Show();
            //获取显示的车辆型号和日期信息
            CafcService.StatisticsData[] dataSource = (CafcService.StatisticsData[])this.gcStatistic.DataSource;
            int total = 0;
            foreach (CafcService.StatisticsData sd in dataSource)
            {
                total += sd.Sl;
            }
            pf.TotalMax = total;
            pf.ShowProcessBar();
            //根据型号和日期（需要把开始日期和结束日期设为月份的第一天和最后一天）从服务器获取车型详细参数
            List<FuelDataService.VehicleBasicInfo> listVB = new List<FuelDataService.VehicleBasicInfo>();
            string startTime = this.dtStartTime.Text.Trim();
            string endTime = this.dtEndTime.Text.Trim();
            int pageCount = 1;

            while (true)
            {
                try
                {
                    FuelDataService.VehicleBasicInfo[] fuelData = Utils.service.QueryUploadedFuelData(Utils.userId, Utils.password, pageCount, 500, string.Empty, String.Empty, string.Empty, string.Empty, startTime, endTime, "MANUFACTURE_TIME");
                    if (fuelData != null)
                    {
                        if (fuelData.Length == 0)
                            break;
                        listVB.AddRange(fuelData);
                        pageCount++;
                        pf.progressBarControl1.Properties.Step = fuelData.Length;
                        pf.progressBarControl1.PerformStep();
                        Application.DoEvents();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (WebException ex)
                {
                    MessageBox.Show("请本地检测网络");
                    return;
                }
            }
            pf.Close();
            //}
            if (listVB.Count == 0)
            {
                return;
            }
            //转换成为本地结构
            List<VehicleBasicInfo> listConvertedVB = Utils.FuelInfoS2C(listVB.ToArray());
            //将信息转化成datatable然后导出到excel当中
            DataTable dtOutput = new DataTable();
            String[] header = new String[] { "VIN", "CLXH", "TYMC", "RLLX", "EDZK", "ZWPS", "BSQXS", "ZCZBZL", "CLZZRQ", "ZHGKRLXHL", "LJ", "LTGG", "UPDATETIME" };
            foreach (String s in header)
            {
                dtOutput.Columns.Add(s);
            }
            foreach (VehicleBasicInfo vb in listConvertedVB)
            {
                DataRow dr = dtOutput.NewRow();
                foreach (DataColumn dc in dtOutput.Columns)
                {
                    String columnName = dc.ColumnName;
                    bool found = false;
                    PropertyInfo[] properties = new VehicleBasicInfo().GetType().GetProperties();
                    foreach (PropertyInfo pi in properties)
                    {
                        if (pi.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase))
                        {
                            object result = pi.GetValue(vb, null);
                            if (result is DateTime)
                            {
                                DateTime dt = (DateTime)result;
                                dr[columnName] = dt.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                dr[columnName] = pi.GetValue(vb, null);
                            }
                            found = true;

                            break;
                        }
                    }
                    if (!found)
                    {
                        RllxParamEntity[] rllxParams = vb.EntityList;
                        if (rllxParams == null || rllxParams.Length == 0)
                        {
                            continue;
                        }
                        foreach (RllxParamEntity rpe in rllxParams)
                        {
                            if (rpe.Param_Code.Contains(columnName))
                            {
                                dr[columnName] = rpe.Param_Value;
                                break;
                            }
                        }
                    }
                }
                dtOutput.Rows.Add(dr);
            }
            PorscheUtils utils = new PorscheUtils(true);
            utils.ExportExcel(dtOutput, this, PorscheUtils.CLMXExport);
        }
    }
}