using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.FuelCafc
{
    public partial class AddForePrjForm : DevExpress.XtraEditors.XtraForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        InitDataTime initTime = new InitDataTime();

        private string operateType = string.Empty;

        public string OperateType
        {
            get { return operateType; }
            set { operateType = value; }
        }

        public AddForePrjForm(string operateType)
        {
            InitializeComponent();

            // 操作类型，
            this.OperateType = operateType;
            this.InitControl();
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        public AddForePrjForm(string operateType, CafcService.ForecastPrj prjObj): this(operateType)
        {
            this.InitData(prjObj);
        }

        private void InitControl()
        {
            if (this.operateType == StaticUtil.AddOp)
            {
                this.tePrjId.Enabled = true;
            }
            if (this.operateType == StaticUtil.EditOp)
            {
                this.tePrjId.Enabled = false;
            }
        }

        private void InitData(CafcService.ForecastPrj prjObj)
        {
            this.tePrjId.Text = prjObj.Prj_Id;
            this.dtEndTime.Text = Convert.ToString(prjObj.StartTime);
            this.dtEndTime.Text = Convert.ToString(prjObj.EndTime);
            this.teRemarks.Text = prjObj.Remarks;
        }

        // 保存预测项目名称
        private void btnSave_Click(object sender, EventArgs e)
        {
            CafcService.ForecastPrj prjObj = new CafcService.ForecastPrj();

            string prjId = this.tePrjId.Text.Trim();
            string startTime = this.dtStartTime.Text.Trim();
            string endTime = this.dtEndTime.Text.Trim();
            string remarks = this.teRemarks.Text.Trim();

            if (true)
            {
                // TODO 验证输入信息
            }

            prjObj.Prj_Id = prjId;
            prjObj.StartTime = Convert.ToDateTime(startTime);
            prjObj.EndTime = Convert.ToDateTime(endTime);
            prjObj.Remarks = remarks;

            bool flag = false;
            string msg = string.Empty;
            this.DialogResult = DialogResult.No;

            try
            {
                flag = this.SaveForecastPrj(prjObj);

                if (flag)
                {
                    msg = "项目保存成功!";
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    msg = "项目保存失败";
                }
            }
            catch (Exception ex)
            {
                msg = "项目保存失败：" + ex.Message;
            }

            MessageBox.Show(msg);
            this.Close();
        }

        // 取消保存
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 保存预测项目数据
        /// </summary>
        /// <param name="prjObj"></param>
        /// <returns></returns>
        private bool SaveForecastPrj(CafcService.ForecastPrj prjObj)
        {
            bool flag = false;
            try
            {
                if (this.operateType == StaticUtil.AddOp)
                {
                    flag = cafcService.SaveForecastPrj(Utils.userId, Utils.password, prjObj);
                }
                if (this.operateType == StaticUtil.EditOp)
                {
                    flag = cafcService.UpdateForecastPrj(Utils.userId, Utils.password, prjObj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return flag;
        }

    }
}