using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace FuelDataSysClient.FuelCafc
{
    public partial class AddNewParamForm : DevExpress.XtraEditors.XtraForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;

        private string operateType = string.Empty;

        public string OperateType
        {
            get { return operateType; }
            set { operateType = value; }
        }

        private string prjId;

        public string PrjId
        {
            get { return prjId; }
            set { prjId = value; }
        }

        private CafcService.ForecastParam paramObj;

        public CafcService.ForecastParam ParamObj
        {
            get { return paramObj; }
            set { paramObj = value; }
        }

        public AddNewParamForm(string operateType)
        {
            InitializeComponent();

            // 操作类型
            this.OperateType = operateType;

            this.InitControl();
        }

        public AddNewParamForm(string operateType, string prjId) : this(operateType)
        {
            this.PrjId = prjId;
            this.tePrjId.Text = prjId;
        }

        /// <summary>
        /// 编辑预测参数的构造
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="paramObj"></param>
        public AddNewParamForm(string operateType, CafcService.ForecastParam paramObj) : this(operateType)
        {
            this.ParamObj = paramObj;
            this.InitData(this.ParamObj);
        }

        /// <summary>
        /// 初始化空间
        /// </summary>
        private void InitControl()
        {
            this.tePrjId.Enabled = false;

            // 查询客户已上报过的油耗数据
            this.QueryExistData();

            // 获取客户已上报过的车型数据
            this.GetClxh();

            // 将车型数据加载到界面
            this.cbClxh.Properties.Items.AddRange(clxhArr);
        }

        /// <summary>
        /// 初始化带修改的数据
        /// </summary>
        /// <param name="prjObj"></param>
        private void InitData(CafcService.ForecastParam paramObj)
        {
            this.tePrjId.Text = paramObj.Prj_Id;
            this.cbClxh.Text = paramObj.Clxh;
            this.cbRllx.Text = paramObj.Rllx;
            this.cbBsqxs.Text = paramObj.Bsqxs;
            this.teZwps.Text = Convert.ToString(paramObj.Zwps);
            this.teZczbzl.Text = Convert.ToString(paramObj.Zczbzl);
            this.teZhgkxslc.Text = Convert.ToString(paramObj.Zhgkxslc);
            this.teZhgkrlxhl.Text = Convert.ToString(paramObj.Zhgkrlxhl);
            this.teJksl.Text = Convert.ToString(paramObj.Sl);
        }

        private CafcService.ForecastParam[] existParamArray = null;
        private string[] clxhArr = new string[] { };

        string clxh = string.Empty;
        string rllx = string.Empty;
        string bsqxs = string.Empty;
        int zwps = 0;
        int zczbzl = 0;
        int jksl = 0;
        decimal zhgkxslc;
        decimal zhgkrlxhl;

        /// <summary>
        /// 保存并清空界面数据，添加下一条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOneMore_Click(object sender, EventArgs e)
        {
            string errorMsg = this.SaveForecastParam();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                this.DialogResult = DialogResult.No;
                MessageBox.Show(errorMsg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.CleanControl();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            string errorMsg = this.SaveForecastParam();
            if (string.IsNullOrEmpty(errorMsg))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.No;
                MessageBox.Show(errorMsg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.Close();
        }

        /// <summary>
        /// 保存预测数据
        /// </summary>
        private string SaveForecastParam()
        {
            string msg = string.Empty;
            bool flag = false;
            try
            {
                List<CafcService.ForecastParam> paramObjList = new List<CafcService.ForecastParam>();
                paramObjList.Add(this.GetPageData());

                if (this.operateType == StaticUtil.AddOp)
                {
                    flag = cafcService.SaveForecastParam(Utils.userId, Utils.password, paramObjList.ToArray());
                }
                if (this.operateType == StaticUtil.EditOp)
                {
                    flag = cafcService.UpdateForecastParam(Utils.userId, Utils.password, paramObjList.ToArray());
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }

        /// <summary>
        /// 获取界面上的数据
        /// </summary>
        public CafcService.ForecastParam GetPageData()
        {
            CafcService.ForecastParam pageParamObj = new CafcService.ForecastParam();

            if (this.operateType == StaticUtil.EditOp)
            {
                pageParamObj.Param_Id = this.ParamObj.Param_Id;
            }
            pageParamObj.Prj_Id = this.tePrjId.Text.Trim();
            pageParamObj.Qcscqy = Utils.qymc;
            pageParamObj.Clxh = this.cbClxh.Text.Trim();
            pageParamObj.Rllx = this.cbRllx.Text.Trim();
            pageParamObj.Bsqxs = this.cbBsqxs.Text.Trim();
            pageParamObj.Zwps = Convert.ToInt32(this.teZwps.Text.Trim());
            pageParamObj.Zczbzl = Convert.ToInt32(this.teZczbzl.Text.Trim());
            pageParamObj.Zhgkxslc = Convert.ToDecimal(this.teZhgkxslc.Text.Trim());
            pageParamObj.Zhgkrlxhl = Convert.ToDecimal(this.teZhgkrlxhl.Text.Trim());
            pageParamObj.Sl = Convert.ToInt32(this.teJksl.Text.Trim());

            return pageParamObj;
        }

        /// <summary>
        /// 清空界面数据
        /// </summary>
        private void CleanControl()
        {
            this.tePrjId.Text = this.PrjId;
            this.cbClxh.Text = string.Empty;
            this.cbRllx.Text = string.Empty;
            this.cbBsqxs.Text = string.Empty;
            this.teZwps.Text = string.Empty;
            this.teZczbzl.Text= string.Empty;
            this.teZhgkxslc.Text = string.Empty;
            this.teZhgkrlxhl.Text = string.Empty;
            this.teJksl.Text = string.Empty;
        }

        /// <summary>
        /// 查询客户已上报过的油耗数据
        /// </summary>
        public void QueryExistData()
        {
            try
            {
                existParamArray = cafcService.QueryExistParam(Utils.userId, Utils.password);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 查询客户已上报过的车型数据
        /// </summary>
        public void GetClxh()
        {
            List<string> clxhList = new List<string>();
            if (existParamArray != null && existParamArray.Length > 0)
            {
                foreach (CafcService.ForecastParam param in existParamArray)
                {
                    clxhList.Add(Convert.ToString(param.Clxh));
                }
            }

            this.clxhArr = clxhList.ToArray();
        }

        /// <summary>
        /// 选择车型是自动加载其他油耗数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbClxh_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedClxh = this.cbClxh.Text.Trim();
            CafcService.ForecastParam param = null;

            try
            {

                foreach (CafcService.ForecastParam p in existParamArray)
                {
                    if (p.Clxh == selectedClxh)
                    {
                        param = p;
                        break;
                    }
                }

                this.cbRllx.Text = Convert.ToString(param.Rllx);
                this.cbBsqxs.Text = Convert.ToString(param.Bsqxs);
                this.teZwps.Text = Convert.ToString(param.Zwps);
                this.teZczbzl.Text = Convert.ToString(param.Zczbzl);
                this.teZhgkxslc.Text = Convert.ToString(param.Zhgkxslc);
                this.teZhgkrlxhl.Text = Convert.ToString(param.Zhgkrlxhl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}