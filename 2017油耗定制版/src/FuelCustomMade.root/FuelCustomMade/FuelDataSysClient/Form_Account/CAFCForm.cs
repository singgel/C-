using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using FuelDataSysClient.Tool;
using FuelDataSysClient.FuelCafc;

namespace FuelDataSysClient.Form_Account
{
    public partial class CAFCForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public CAFCForm()
        {
            InitializeComponent();
        }

        private void CAFCForm_Load(object sender, EventArgs e)
        {
            if (!Utils.IsFuelTest)
            {
                var data = TestDataInit();
                this.gcNeCafc.DataSource = data;
                this.gcTeCafc.DataSource = data;
                btnNeSearch.Enabled = false;
                btnTeSearch.Enabled = false;
            }
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
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

            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            msg = this.DataBinds(StaticUtil.NeCafc, startTime, endTime);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    cafcData = Utils.serviceCafc.QueryNECafc(Utils.userId, Utils.password, startTime, endTime);
                }
                else if (cafcType == StaticUtil.TeCafc)
                {
                    cafcData = Utils.serviceCafc.QueryTECafc(Utils.userId, Utils.password, startTime, endTime);
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