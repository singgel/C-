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
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Model;

namespace FuelDataSysClient.Form_SJHS
{
    public partial class CAFCForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        readonly Dictionary<int, string> dictInit = new Dictionary<int, string>();

        public CAFCForm()
        {
            InitializeComponent();
            if (!Utils.IsFuelTest)
            {
                this.gcNeCafc.DataSource = DataSourceHelper.CAFCFuelData();
                this.gcTeCafc.DataSource = DataSourceHelper.CAFCFuelData();
                btnNeSearch.Enabled = false;
                btnTeSearch.Enabled = false;
            }
            InitDict();
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //初始化字典 倍数 2015 年之前标准
        private void InitDict()
        {
            dictInit.Add(2012, "1.09");
            dictInit.Add(2013, "1.06");
            dictInit.Add(2014, "1.03");
            dictInit.Add(2015, "1.00");
            dictInit.Add(2016, "1.34");
            dictInit.Add(2017, "1.28");
            dictInit.Add(2018, "1.20");
            dictInit.Add(2019, "1.10");
            dictInit.Add(2020, "1.00");
        }

        //核算-不计入新能源
        private void btnTeSearch_Click(object sender, EventArgs e)
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
                CafcService.FuelCafcAndTcafc[] teCafc = Utils.serviceCafc.QueryTECafc(Utils.userId, Utils.password, this.dtStartTime.Text, this.dtEndTime.Text);
                this.gcTeCafc.DataSource = teCafc;
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

        //核算-计入新能源
        private void btnNeSearch_Click(object sender, EventArgs e)
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
                CafcService.FuelCafcAndTcafc[] neCafc = Utils.serviceCafc.QueryNECafc(Utils.userId, Utils.password, this.dtStartTime.Text, this.dtEndTime.Text);
                this.gcNeCafc.DataSource = neCafc;
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
}