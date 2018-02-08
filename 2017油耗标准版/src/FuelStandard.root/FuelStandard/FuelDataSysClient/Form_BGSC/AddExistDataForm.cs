using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Common;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.FuelCafc
{
    public partial class AddExistDataForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        InitDataTime initTime = new InitDataTime();
        
        private string prjId;

        public string PrjId
        {
            get { return prjId; }
            set { prjId = value; }
        }

        public AddExistDataForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        public AddExistDataForm(string prjId) : this()
        {
            this.PrjId = prjId;
        }

        /// <summary>
        /// 查询已上报的详细数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            string clxh = this.txtclxh.Text.Trim();
            string rllx = this.txtrllx.Text.Trim();
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
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            msg = this.DataBinds(clxh, rllx, startTime, endTime);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 绑定查询到的数据
        /// </summary>
        /// <param name="clxh"></param>
        /// <param name="rllx"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string DataBinds(string clxh, string rllx, string startTime, string endTime)
        {
            string msg = string.Empty;
            try
            {
                CafcService.FuelCAFCDetails[] detailData = this.GetCafcDetailData(clxh, rllx, startTime, endTime);

                if (detailData != null)
                {
                    this.gcParam.DataSource = detailData;
                    this.lblNum.Text = string.Format("共{0}条", detailData.Length);
                    //Utils.SelectItem(detailData, false);
                }
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        /// <summary>
        /// 将已有数据添加为预测数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddForecast_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 添加新预测数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewForecast_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 添加已上报过的数据作为预测数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddExistData_ItemClick(object sender, ItemClickEventArgs e)
        {
            string msg = string.Empty;

            try
            {
                CafcService.ForecastParam[] selectedParam = this.GetSelectedData();
                bool flag = false;

                if (selectedParam != null && selectedParam.Length>0)
                {
                    flag = cafcService.SaveForecastParam(Utils.userId, Utils.password, selectedParam);

                    if (flag)
                    {
                        this.DialogResult = DialogResult.OK;
                        msg = "添加成功";
                    }
                    else
                    {
                        msg = "添加失败";
                    }
                }
                else
                {
                    msg = "请选择数据";
                }
            }
            catch (Exception ex)
            {
                msg = "添加失败" + ex.Message;
            }

            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 添加新预测数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewData_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnSelectAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvParam.PostEditor();
            CafcService.FuelCAFCDetails[] detailObjArr = (CafcService.FuelCAFCDetails[])this.gvParam.DataSource;

            foreach (var item in detailObjArr)
            {
                item.Check = true;
            }

            this.gvParam.PostEditor();
            this.gvParam.RefreshData();
        }

        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnClearAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvParam.PostEditor();
            CafcService.FuelCAFCDetails[] detailObjArr = (CafcService.FuelCAFCDetails[])this.gvParam.DataSource;

            foreach (var item in detailObjArr)
            {
                item.Check = false;
            }

            this.gvParam.PostEditor();
            this.gvParam.RefreshData();
        }

        /// <summary>
        /// 获取选中的数据
        /// </summary>
        /// <returns></returns>
        private CafcService.ForecastParam[] GetSelectedData()
        {
            List<CafcService.ForecastParam> selectedParam = new List<CafcService.ForecastParam>();

            try
            {
                this.gvParam.PostEditor();
                CafcService.FuelCAFCDetails[] detailObjArr = (CafcService.FuelCAFCDetails[])this.gvParam.DataSource;

                if (detailObjArr != null)
                {
                    foreach (CafcService.FuelCAFCDetails detailObj in detailObjArr)
                    {
                        if (detailObj.Check)
                        {
                            CafcService.ForecastParam paramObj = new CafcService.ForecastParam();

                            paramObj.Prj_Id = this.PrjId;
                            paramObj.Qcscqy = detailObj.Qcscqy;
                            paramObj.Clxh = detailObj.Clxh;
                            paramObj.Rllx = detailObj.Rllx;
                            paramObj.Bsqxs = detailObj.Bsqxs;
                            paramObj.Zczbzl = detailObj.Zczbzl;
                            paramObj.Zwps = detailObj.Zwps;
                            paramObj.Zhgkxslc = detailObj.Zhgkxslc;
                            paramObj.Zhgkrlxhl = detailObj.ActZhgkrlxhl;
                            paramObj.Sl = detailObj.Sl_act;

                            selectedParam.Add(paramObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return selectedParam.ToArray();
        }

        /// <summary>
        /// 获取已上报详细数据
        /// </summary>
        /// <param name="clxh"></param>
        /// <param name="rllx"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
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
    }
}