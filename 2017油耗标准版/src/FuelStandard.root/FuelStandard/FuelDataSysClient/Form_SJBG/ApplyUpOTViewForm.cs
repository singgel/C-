using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Base;
using System.Threading;
using FuelDataSysClient.Tool;
using System.Text.RegularExpressions;
using FuelDataSysClient.SubForm;
using DevExpress.XtraSplashScreen;

namespace FuelDataSysClient
{
    public partial class ApplyUpOTForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        private Dictionary<string, FuelDataModel.RllxParamEntity[]> rpeht = new Dictionary<string, FuelDataModel.RllxParamEntity[]>();
        private static int dataCount;
        InitDataTime initTime = new InitDataTime();


        public ApplyUpOTForm()
        {
            InitializeComponent();

            // 设置燃料类型下拉框的值
            this.SetFuelType();
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dataCount = this.GetDataFromService(1);
            this.lcPageNum.Text = "第1页";
        }

        private int GetDataFromService(int pageNum)
        {
            if (!Utils.CheckUser())
            {
                return -1;
            }

            // 验证查询时间：结束时间不能小于开始时间
            if (!this.VerifyStartEndTime())
            {
                MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return -1;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                FuelDataService.VehicleBasicInfo[] queryInfoArr = service.QueryApplyUpLoadOT(Utils.userId, Utils.password, pageNum, 20, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                if (queryInfoArr != null && queryInfoArr.Length > 0)
                {
                    List<FuelDataModel.VehicleBasicInfo> vbis = Utils.FuelInfoS2C(queryInfoArr);
                    for (int i = 0; i < vbis.Count; i++)
                    {
                        FuelDataModel.VehicleBasicInfo vbi = vbis[i];
                        if (rpeht.ContainsKey(vbi.App_Vin))
                        {
                            rpeht.Remove(vbi.App_Vin);
                        }
                        rpeht.Add(vbi.App_Vin, vbi.EntityList);
                        vbi.EntityList = null;

                    }
                    dataCount = queryInfoArr.Length;
                    gcApplyUpOTQuery.DataSource = vbis;
                    return vbis.Count;
                }
                else
                {
                    gcApplyUpOTQuery.DataSource = null;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                SplashScreenManager.CloseForm();
            }
            return -1;
        }

        private void btnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            int pageNum = Convert.ToInt32(Regex.Matches(lcPageNum.Text, @"\d+")[0].Value);
            if (pageNum != 0)
            {
                GetDataFromService(pageNum);
                lcPageNum.Text = String.Format("第{0}页", pageNum);
            }
        }

        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(Regex.Matches(lcPageNum.Text, @"\d+")[0].Value);
            if (pageNum != 0)
            {
                if (pageNum > 1)
                {
                    GetDataFromService(--pageNum);
                    lcPageNum.Text = String.Format("第{0}页", pageNum);
                }
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(Regex.Matches(lcPageNum.Text, @"\d+")[0].Value);
            if (pageNum != 0)
            {
                if (dataCount == 20)
                {
                    GetDataFromService(++pageNum);
                    lcPageNum.Text = String.Format("第{0}页", pageNum);
                }
            }
        }

        private void gcApplyUpOTQuery_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)gcApplyUpOTQuery.FocusedView;
            FuelDataModel.VehicleBasicInfo vbi = (FuelDataModel.VehicleBasicInfo)cv.GetFocusedRow();

            if (vbi == null)
            {
                return;
            }

            // 弹出详细信息窗口
            JbxxViewForm jvf = new JbxxViewForm();
            setControlValue(jvf, "tbvin", vbi.App_Vin, false);
            setControlValue(jvf, "tbQcscqy", vbi.Qcscqy, false);
            setControlValue(jvf, "tbJkqczjxs", vbi.Jkqczjxs, false);
            setControlValue(jvf, "tbClxh", vbi.Clxh, false);
            setControlValue(jvf, "tbClzl", vbi.Clzl, false);
            setControlValue(jvf, "tbRllx", vbi.Rllx, false);
            setControlValue(jvf, "tbZczbzl", vbi.Zczbzl.ToString(), false);
            setControlValue(jvf, "tbZgcs", vbi.Zgcs.ToString(), false);
            setControlValue(jvf, "tbLtgg", vbi.Ltgg, false);
            setControlValue(jvf, "tbZj", vbi.Zj.ToString(), false);
            setControlValue(jvf, "tbClzzrq", vbi.Clzzrq.ToString(), false);
            setControlValue(jvf, "tbTymc", vbi.Tymc, false);
            setControlValue(jvf, "tbYyc", vbi.Yyc, false);
            setControlValue(jvf, "tbZwps", vbi.Zwps.ToString(), false);
            setControlValue(jvf, "tbZdsjzzl", vbi.Zdsjzzl.ToString(), false);
            setControlValue(jvf, "tbEdzk", vbi.Edzk.ToString(), false);
            setControlValue(jvf, "tbLj", vbi.Lj.ToString(), false);
            setControlValue(jvf, "tbQdxs", vbi.Qdxs, false);
            setControlValue(jvf, "tbJyjgmc", vbi.Jyjgmc, false);
            setControlValue(jvf, "tbJybgbh", vbi.Jybgbh, false);

            // 获取燃料信息
            FuelDataModel.RllxParamEntity[] rpelist = this.rpeht[vbi.App_Vin];
            for (int i = 0; rpelist != null && i < rpelist.Length; i++)
            {
                FuelDataModel.RllxParamEntity rpe = rpelist[i];
                setControlValue(jvf, rpe.Param_Code, rpe.Param_Value, false);
            }

            (jvf.Controls.Find("tc", true)[0] as XtraTabControl).SelectedTabPageIndex = 0;
            jvf.MaximizeBox = false;
            jvf.MinimizeBox = false;
            Utils.SetFormMid(jvf);
            jvf.setVisible("btnbaocun", false);
            jvf.setVisible("btnbaocunshangbao", false);
            jvf.setVisible("btnPrint", false);
            jvf.ShowDialog();
        }

        public void setControlValue(JbxxViewForm jvf, string cName, String val, bool enable)
        {
            if (cName == null || "" == cName)
            {
                return;
            }

            Control[] c = jvf.Controls.Find(cName, true);
            if (c.Length > 0)
            {
                if (c[0] is TextEdit)
                {
                    c[0].Text = val;
                }
                if (c[0] is DevExpress.XtraEditors.ComboBoxEdit)
                {
                    DevExpress.XtraEditors.ComboBoxEdit cbe = c[0] as DevExpress.XtraEditors.ComboBoxEdit;
                    cbe.Text = val;
                    if (cbe.Text == "汽油" || cbe.Text == "柴油" || cbe.Text == "两用燃料"
                        || cbe.Text == "双燃料" || cbe.Text == "气体燃料" || cbe.Text == "纯电动" || cbe.Text == "非插电式混合动力" || cbe.Text == "插电式混合动力" || cbe.Text == "燃料电池")
                    {
                        string rlval = cbe.Text;
                        if (cbe.Text == "汽油" || cbe.Text == "柴油" || cbe.Text == "两用燃料"
                        || cbe.Text == "双燃料" || cbe.Text == "气体燃料")
                        {
                            rlval = "传统能源";
                        }

                        // 构建燃料参数控件
                        jvf.getParamList(rlval, false);
                    }
                }
                c[0].Enabled = enable;
            }
        }



        // 获取模糊查询中的时间类型
        protected string GetTimeType()
        {
            // 查询日期类型：
            //   UPLOAD_TIME 表示上报日期
            //   MANUFACTURE_TIME 表示车辆制造日期/进口核销日期
            string timeType = "UPLOAD_TIME";
            if (cbTimeType.Text == "车辆制造日期/进口核销日期")
            {
                timeType = "MANUFACTURE_TIME";
            }
            return timeType;
        }

        // 验证查询时间：结束时间不能小于开始时间
        protected bool VerifyStartEndTime()
        {
            string startTime = dtStartTime.Text;
            string endTime = dtEndTime.Text;

            try
            {
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime) && Convert.ToDateTime(startTime) > Convert.ToDateTime(endTime))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        // 设置燃料类型下拉框的值
        protected void SetFuelType()
        {
            List<string> fuelTypeList = Utils.GetFuelType("SEARCH");
            this.cbRllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }
    }
}