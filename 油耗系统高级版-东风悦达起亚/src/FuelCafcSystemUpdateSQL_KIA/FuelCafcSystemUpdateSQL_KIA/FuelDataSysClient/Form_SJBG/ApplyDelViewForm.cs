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
using FuelDataSysClient.Form_SJSB;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using System.Text.RegularExpressions;
using FuelDataSysClient.Model;
using FuelDataModel;

namespace FuelDataSysClient.Form_SJBG
{
    public partial class ApplyDelViewForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Dictionary<string, FuelDataModel.RllxParamEntity[]> rpeht = new Dictionary<string, FuelDataModel.RllxParamEntity[]>();
        private static int dataCount;

        public ApplyDelViewForm()
        {
            InitializeComponent();
            // 设置燃料类型下拉框的值
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelType("SEARCH"));
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //查询撤销数据
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.GetDataFromService(1);
            this.lcPageNum.Text = "第1页";
        }

        //查询服务器数据库
        private void GetDataFromService(int pageNum)
        {
            if (!Utils.CheckUser())
            {
                return;
            }
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
                FuelDataService.VehicleBasicInfo[] queryInfoArr = Utils.service.QueryApplyDelInfo(Utils.userId, Utils.password, pageNum, 20, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                if (queryInfoArr != null && queryInfoArr.Length > 0)
                {
                    List<VehicleBasicInfo> vbis = Utils.FuelInfoS2C(queryInfoArr);
                    for (int i = 0; i < vbis.Count; i++)
                    {
                        VehicleBasicInfo vbi = vbis[i];
                        if (rpeht.ContainsKey(vbi.App_Vin))
                        {
                            rpeht.Remove(vbi.App_Vin);
                        }
                        rpeht.Add(vbi.App_Vin, vbi.EntityList);
                        vbi.EntityList = null;

                    }
                    dataCount = queryInfoArr.Length;
                    gcApplyDelQuery.DataSource = vbis;
                }
                else
                {
                    gcApplyDelQuery.DataSource = null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("错误：" + e.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //刷新当前页面
        private void btnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            int pageNum = Convert.ToInt32(Regex.Matches(lcPageNum.Text, @"\d+")[0].Value);
            if (pageNum != 0)
            {
                GetDataFromService(pageNum);
                lcPageNum.Text = String.Format("第{0}页", pageNum);
            }
        }

        //上一页
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

        //下一页
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

        //双击事件
        private void gcApplyDelQuery_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)gcApplyDelQuery.FocusedView;
            VehicleBasicInfo vbi = (VehicleBasicInfo)cv.GetFocusedRow();

            if (vbi == null)
            {
                return;
            }

            // 弹出详细信息窗口
            JbxxViewForm jvf = new JbxxViewForm();
            setControlValue(jvf, "tbvin", vbi.App_Vin, false);
            setControlValue(jvf, "tbHgspbm", vbi.Hgspbm, false);
            setControlValue(jvf, "tbQcscqy", vbi.Qcscqy, false);
            setControlValue(jvf, "tbJkqczjxs", vbi.Jkqczjxs, false);
            setControlValue(jvf, "tbClxh", vbi.Clxh, false);
            setControlValue(jvf, "tbClzl", vbi.Clzl, false);
            setControlValue(jvf, "tbRllx", vbi.Rllx, false);
            setControlValue(jvf, "tbZczbzl", vbi.Zczbzl, false);
            setControlValue(jvf, "tbZgcs", vbi.Zgcs, false);
            setControlValue(jvf, "tbLtgg", vbi.Ltgg, false);
            setControlValue(jvf, "tbZj", vbi.Zj, false);
            setControlValue(jvf, "tbClzzrq", vbi.Clzzrq.ToString(), false);
            setControlValue(jvf, "tbTymc", vbi.Tymc, false);
            setControlValue(jvf, "tbYyc", vbi.Yyc, false);
            setControlValue(jvf, "tbZwps", vbi.Zwps, false);
            setControlValue(jvf, "tbZdsjzzl", vbi.Zdsjzzl, false);
            setControlValue(jvf, "tbEdzk", vbi.Edzk, false);
            setControlValue(jvf, "tbLj", vbi.Lj, false);
            setControlValue(jvf, "tbQdxs", vbi.Qdxs, false);
            setControlValue(jvf, "tbJyjgmc", vbi.Jyjgmc, false);
            setControlValue(jvf, "tbJybgbh", vbi.Jybgbh, false);
            setControlValue(jvf, "tbQtxx", vbi.Qtxx, false);

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

        //初始化基本信息窗体的数据
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
                        || cbe.Text == "双燃料" || cbe.Text == "纯电动" || cbe.Text == "非插电式混合动力" || cbe.Text == "插电式混合动力" || cbe.Text == "燃料电池")
                    {
                        string rlval;
                        if (cbe.Text == "汽油" || cbe.Text == "柴油" || cbe.Text == "两用燃料" || cbe.Text == "双燃料")
                            rlval = "传统能源";
                        else
                            rlval = cbe.Text;

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
            //   MANUFACTURE_TIME 表示车辆制造日期/进口日期
            string timeType;
            if (cbTimeType.Text == "制造/进口日期")
                timeType = "MANUFACTURE_TIME";
            else
                timeType = "UPLOAD_TIME";
            return timeType;
        }

    }
}