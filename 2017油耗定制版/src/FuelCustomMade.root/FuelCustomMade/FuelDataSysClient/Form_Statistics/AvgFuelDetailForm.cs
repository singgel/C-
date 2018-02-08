using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Base;
using FuelDataSysClient.Tool;
using System.Threading;
using System.Net;
using FuelDataSysClient.Model.Model_Porsche;
using FuelDataSysClient.Tool.Tool_Porsche;
using System.Reflection;
using FuelDataSysClient.FuelCafc;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Statistics
{
    public partial class AvgFuelDetailForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;

        public AvgFuelDetailForm()
        {
            InitializeComponent();

            if (!Utils.IsFuelTest)
            {
                var data = TestDataInit();
                this.gcDetail.DataSource = data;
                this.lblNum.Text = string.Format("共{0}条", data.Rows.Count);
                btnSearch.Enabled = false;
                hzbgExport.Enabled = false;
            }
            if (Utils.userId.Equals("FADCFBSJ0U001"))
            {
                this.hzbgExport.Visible = true;
            }
        }

        private void AvgFuelDetailForm_Load(object sender, EventArgs e)
        {
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            string startTime = this.dtStartTime.Text.Trim();
            string endTime = this.dtEndTime.Text.Trim();
            if (string.IsNullOrEmpty(startTime))
            {
                msg += "统计开始时间不能为空\r\n";
            }
            if (string.IsNullOrEmpty(endTime))
            {
                msg += "统计结束时间不能为空\r\n";
            }
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                msg = this.DataBinds(this.txtclxh.Text.Trim(), this.cbRllx.Text.Trim(), startTime, endTime);
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private string DataBinds(string clxh, string rllx, string startTime, string endTime)
        {
            string msg = string.Empty;
            try
            {
                CafcService.FuelCAFCDetails[] detailData = this.GetCafcDetailData(clxh, rllx, startTime, endTime);

                if (detailData != null)
                {
                    this.gcDetail.DataSource = detailData;
                    this.gvDetail.BestFitColumns();
                    this.lblNum.Text = string.Format("共{0}条", detailData.Length);
                }
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        private CafcService.FuelCAFCDetails[] GetCafcDetailData(string clxh, string rllx, string startTime, string endTime)
        {
            CafcService.FuelCAFCDetails[] detailData = null;

            try
            {
                detailData = cafcService.QueryCalParamDetails(Utils.userId, Utils.password, rllx, clxh, startTime, endTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return detailData;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportDetail_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };
                    this.gcDetail.ExportToXls(saveFileDialog.FileName, options);
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

        private void gcDetail_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            VinDetailForm vinForm = new VinDetailForm();
            vinForm.ShowDialog();
        }

        private void rhlVin_Click(object sender, EventArgs e)
        {
            this.GetLinkedVins();
        }

        /// <summary>
        /// 显示VIN数据
        /// </summary>
        private void GetLinkedVins()
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
            catch (Exception)
            {
            }

        }

        private DataTable TestDataInit()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Clxh", "CCLEIX");
            data.Add("Rllx", "汽油");
            data.Add("Bsqxs", "334");
            data.Add("Zczbzl", "3322");
            data.Add("Zwps", "8");
            data.Add("Zhgkxslc", "4.4");
            data.Add("TgtZhgkrlxhl", "6.6");
            data.Add("ActZhgkrlxhl", "5.6");
            data.Add("Sl_hs", "556");
            data.Add("Sl_act", "224");
            return Common.DataTableHelper.FillDataTable(data, 20);
        }

        /// <summary>
        /// 汇总报告导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hzbgExport_Click(object sender, EventArgs e)
        {
            Thread newThread = new Thread(porscheExport);
            newThread.ApartmentState = ApartmentState.STA;
            newThread.Start();
        }

        //保时捷汇总导出线程方法
        public void porscheExport()
        {
            try
            {
                ProcessForm pf = new ProcessForm();
                pf.Show();
                //获取显示的车辆型号和日期信息
                CafcService.FuelCAFCDetails[] dataSource = (CafcService.FuelCAFCDetails[])this.gcDetail.DataSource;

                int total = 0;
                foreach (CafcService.FuelCAFCDetails sd in dataSource)
                {
                    total += sd.Sl_hs;
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
                
                if (listVB.Count == 0)
                {
                    return;
                }
                //转换成为本地结构
                List<HZDCModule> listHzdc = Utils.VehicleBasicInfoTo<HZDCModule>(listVB.ToArray());
                PorscheUtils porscheUtils = new PorscheUtils(true, startTime);
                List<exportModel> getMBZResult = new List<exportModel>();
                //对vin信息进行分组
                var vehicleBasicInfoGroupResult = (from t in listHzdc
                                                   group t by new { t.ClXH, t.TYMC, t.ZHGKRLXHL, t.RLLX, t.ZCZBZL, t.BSQXS, t.ZWPS, t.EDZK }
                                                       into g
                                                       select new
                                                       {
                                                           g.Key.ClXH,
                                                           g.Key.BSQXS,
                                                           g.Key.EDZK,
                                                           g.Key.RLLX,
                                                           g.Key.TYMC,
                                                           g.Key.ZCZBZL,
                                                           g.Key.ZHGKRLXHL,
                                                           g.Key.ZWPS,
                                                           SL = g.Count()
                                                       }).ToList();
                if (Convert.ToDateTime(startTime).Year < 2016)
                {
                    getMBZResult = (from s in vehicleBasicInfoGroupResult
                                    select new exportModel
                                    {
                                        bsqxs = s.BSQXS,
                                        clxh = s.ClXH,
                                        edzk = s.EDZK,
                                        rllx = s.RLLX,
                                        tymc = s.TYMC,
                                        zczbzl = Convert.ToInt32(s.ZCZBZL),
                                        zhgkrlxhlmbz = Convert.ToDecimal((from t in porscheUtils.dtTargetFuel.AsEnumerable()
                                                                          where  (s.BSQXS.Equals("MT") ? "MT" : "OT").Equals(t.Field<string>("BSQXS"))
                                                                                  && (Convert.ToInt32(s.ZWPS) < 3 ? "2" : "3").Equals(t.Field<string>("ZWPS"))
                                                                                  && Convert.ToDouble(s.ZCZBZL) > t.Field<double>("MIN_ZCZBZL")
                                                                                  && Convert.ToDouble(s.ZCZBZL) <= t.Field<double>("MAX_ZCZBZL")
                                                                          select t.Field<string>("TGT_ZHGKRLXHL")).FirstOrDefault()),
                                        zhgkrlxhl = Convert.ToDecimal(s.ZHGKRLXHL),
                                        zwps = s.ZWPS,
                                        sl = s.SL,
                                    }).ToList();
                }
                else
                {
                    getMBZResult = (from s in vehicleBasicInfoGroupResult
                                    select new exportModel
                                    {
                                        bsqxs = s.BSQXS,
                                        clxh = s.ClXH,
                                        edzk = s.EDZK,
                                        rllx = s.RLLX,
                                        tymc = s.TYMC,
                                        zczbzl = Convert.ToInt32(s.ZCZBZL),
                                        zhgkrlxhlmbz = Convert.ToDecimal((from t in porscheUtils.dtTargetFuel.AsEnumerable()
                                                                          where (Convert.ToInt32(s.ZWPS) < 3 ? "2" : "3").Equals(t.Field<string>("ZWPS"))
                                                                                  && Convert.ToDouble(s.ZCZBZL) > t.Field<double>("MIN_ZCZBZL")
                                                                                  && Convert.ToDouble(s.ZCZBZL) <= t.Field<double>("MAX_ZCZBZL")
                                                                          select t.Field<string>("TGT_ZHGKRLXHL")).FirstOrDefault()),
                                        zhgkrlxhl = Convert.ToDecimal(s.ZHGKRLXHL),
                                        zwps = s.ZWPS,
                                        sl = s.SL,
                                    }).ToList();
                }

                var vinGroupResult = (from s in getMBZResult
                                      group s by s.clxh into result
                                      select new
                                      {
                                          clxh = result.First().clxh,
                                          rdzhgkrlxhl = result.Max(s => s.zhgkrlxhl),
                                          rdzhgkrlxhlmbz = result.Min(s => s.zhgkrlxhlmbz)
                                      }).ToList();

                var allInfoReadyResult = (from s1 in getMBZResult
                                          join s2 in vinGroupResult
                                          on s1.clxh equals s2.clxh
                                          select new
                                          {
                                              bsqxs = s1.bsqxs.ToString(),
                                              clxh = s1.clxh.ToString(),
                                              edzk = s1.edzk.ToString(),
                                              rllx = s1.rllx.ToString(),
                                              tymc = s1.tymc.ToString(),
                                              zczbzl = Convert.ToInt16(s1.zczbzl),
                                              zhgkrlxhlmbz = Convert.ToDecimal(s1.zhgkrlxhlmbz),
                                              zhgkrlxhl = Convert.ToDecimal(s1.zhgkrlxhl),
                                              zwps = s1.zwps.ToString(),
                                              rdzhgkrlxhl = s2.rdzhgkrlxhl,
                                              rdzhgkrlxhlmbz = s2.rdzhgkrlxhlmbz,
                                              sl = s1.sl,
                                          }).ToList();

                //将信息转化成datatable然后导出到excel当中
                DataTable dtOutput = new DataTable();

                String[] header = new String[]{
                    "CLXH",
                    "TYMC",
                    "SL;int",
                    "ZHGKRLXHL;decimal",
                    "RDZHGKRLXHL;decimal",
                    "SL*ZHGKRLXHL;decimal",
                    "SL*RDZHGKRLXHL;decimal",
                    "RLLX",
                    "ZCZBZL;int",
                    "BSQXS",
                    "ZWPS",
                    "EDZK",
                    "ZHGKRLXHLMBZ;decimal",
                    "RDZHGKRLXHLMBZ;decimal",
                    "SL*ZHGKRLXHLMBZ;decimal",
                    "SL*RDZHGKRLXHLMBZ;decimal"};
                foreach (String s in header)
                {
                    String type = "string", expression = "";
                    String[] expGroup = s.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (expGroup.First().Contains("*"))
                    {
                        expression = expGroup.First().Replace("*", " * ");
                    }
                    if (expGroup.Length > 1)
                    {
                        type = expGroup.Last();
                    }
                    dtOutput.Columns.Add(expGroup.First(), Utils.GetTypeByString(type), expression);
                }
                foreach (var vb in allInfoReadyResult)
                {
                    DataRow dr = dtOutput.NewRow();
                    foreach (DataColumn dc in dtOutput.Columns)
                    {
                        String columnName = dc.ColumnName;
                        bool found = false;
                        PropertyInfo[] properties = vb.GetType().GetProperties();
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
                    }
                    dtOutput.Rows.Add(dr);
                }
                if (Convert.ToDateTime(startTime).Year < 2016)
                {
                    porscheUtils.ExportExcel(dtOutput, this, PorscheUtils.HZBGExport);
                }
                else
                {
                    porscheUtils.ExportExcel(dtOutput, this, PorscheUtils.HZBGExport_New); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message + "\r\n" + ex.StackTrace, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        class exportModel 
        {
            public string bsqxs { set; get; }
            public string clxh { set; get; }
            public string edzk { set; get; }
            public string rllx { set; get; }
            public string tymc { set; get; }
            public int zczbzl { set; get; }
            public decimal zhgkrlxhlmbz { set; get; }
            public decimal zhgkrlxhl { set; get; }
            public string zwps { set; get; }
            public int sl { set; get; } 
        }
    }
}