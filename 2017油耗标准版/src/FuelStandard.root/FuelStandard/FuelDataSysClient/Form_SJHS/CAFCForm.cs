using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using FuelDataSysClient.Tool;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.FuelCafc
{
    public partial class CAFCForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        InitDataTime initTime = new InitDataTime();
        CafcUtils utils = new CafcUtils();

        public CAFCForm()
        {
            InitializeComponent();
            if (!Utils.IsFuelTest)
            {
                var data = TestDataInit();

                this.gcNeCafc.DataSource = data;

                this.gcTeCafc.DataSource = data;
              
                btnNeSearch.Enabled = false;
                btnTeSearch.Enabled = false;
            }
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        private void btnTeSearch_Click(object sender, EventArgs e)
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
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            msg = this.DataBinds(StaticUtil.TeCafc, startTime, endTime);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnNeSearch_Click(object sender, EventArgs e)
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
            if (Convert.ToDateTime(endTime) < Convert.ToDateTime(startTime))
            {
                msg += "结束时间不能小于开始时间\r\n";
            }

            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                msg = this.DataBinds(StaticUtil.NeCafc, startTime, endTime);
                if (!string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private string DataBinds(string cafcType, string startTime, string endTime)
        {
            string msg = string.Empty;
            try
            {
                CafcService.FuelCafcAndTcafc[] detailData = this.GetCafcDetailData(cafcType, startTime, endTime);

                if (detailData != null)
                {
                    if (cafcType == StaticUtil.NeCafc)
                    {
                        this.gcNeCafc.DataSource = detailData;
                    }
                    else if (cafcType == StaticUtil.TeCafc)
                    {
                        this.gcTeCafc.DataSource = detailData;
                    }
                }
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        private CafcService.FuelCafcAndTcafc[] GetCafcDetailData(string cafcType, string startTime, string endTime)
        {
            CafcService.FuelCafcAndTcafc[] cafcData = null;
 
            try
            {
                if (cafcType == StaticUtil.NeCafc)
                {
                    cafcData = cafcService.QueryNECafc(Utils.userId, Utils.password, startTime, endTime);
                }
                else if (cafcType == StaticUtil.TeCafc)
                {
                    cafcData = cafcService.QueryTECafc(Utils.userId, Utils.password, startTime, endTime);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cafcData;
        }

        private DataTable TestDataInit()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Sl_act","334");
            data.Add("Sl_hs","445");
            data.Add("Cafc","8.2");
            data.Add("Tcafc", "9.2");

            return Common.DataTableHelper.FillDataTable(data, 2);
        }

        
    }
}