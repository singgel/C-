using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Base;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.FuelCafc
{
    public partial class ForecastDataForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;

        private string defaultPrjId;

        public ForecastDataForm()
        {
            InitializeComponent();
            this.SearchForecastPrj();
        }

        #region 预测项目

        /// <summary>
        /// 新增预测项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddPrj_ItemClick(object sender, ItemClickEventArgs e)
        {
            AddForePrjForm newPrjForm = new AddForePrjForm(StaticUtil.AddOp);
            DialogResult prjResult = newPrjForm.ShowDialog();
            
            // TODO 预测项目数据刷新
            if (prjResult == DialogResult.OK)
            {
                this.SearchForecastPrj();
            }
        }

        /// <summary>
        /// 修改预测项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditPrj_ItemClick(object sender, ItemClickEventArgs e)
        {
            ColumnView cv = (ColumnView)gcForePrj.FocusedView;
            CafcService.ForecastPrj prjObj = (CafcService.ForecastPrj)cv.GetFocusedRow();

            try
            {
                if (prjObj != null)
                {
                    AddForePrjForm editPrjForm = new AddForePrjForm(StaticUtil.EditOp, prjObj);
                    DialogResult editResult = editPrjForm.ShowDialog();

                    if (editResult == DialogResult.OK)
                    {
                        this.SearchForecastPrj(); ;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 删除预测项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelPrj_ItemClick(object sender, ItemClickEventArgs e)
        {
            string msg = string.Empty;
            CafcUtils cafcUtil = new CafcUtils();

            try
            {
                this.gvForePrj.PostEditor();
                CafcService.ForecastPrj[] prjObjArr = (CafcService.ForecastPrj[])this.gvForePrj.DataSource;
                List<string> selectPrjIdList = new List<string>();
                if (prjObjArr != null)
                {
                    foreach (CafcService.ForecastPrj prjObj in (CafcService.ForecastPrj[])this.gvForePrj.DataSource)
                    {
                        if (prjObj.Check)
                        {
                           selectPrjIdList.Add(prjObj.Prj_Id.Trim());
                        }
                    }
                }

                if (selectPrjIdList.Count > 0)
                {
                    bool flag = cafcService.DelForecastPrj(Utils.userId, Utils.password, selectPrjIdList.ToArray());

                    if (flag)
                    {
                        this.SearchForecastPrj();
                        this.ResetForecastParam();
                        msg = "项目删除成功!";
                    }
                    else
                    {
                        msg = "项目删除失败";
                    }
                }
                else
                {
                    msg = "请选择要删除的项目";
                }
            }
            catch (Exception ex)
            {
                msg += "项目保存失败：" + ex.Message;
            }
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 新增预测数据（现有数据源）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddData_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                CafcService.ForecastPrj addPrjObj = this.GetEidtForecastPrjObj();
                if (addPrjObj != null)
                {
                    this.defaultPrjId = addPrjObj.Prj_Id;

                    AddExistDataForm newDataFrm = new AddExistDataForm(addPrjObj.Prj_Id);
                    DialogResult addParamFrmResult = newDataFrm.ShowDialog();

                    if (addParamFrmResult == DialogResult.OK)
                    {
                        this.SearchForecastParam();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 新增预测数据（新车型）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddNewData_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                AddNewParamForm newParamFrm = new AddNewParamForm(StaticUtil.AddOp, this.defaultPrjId);
                DialogResult addParamFrmResult = newParamFrm.ShowDialog();

                if (addParamFrmResult == DialogResult.OK)
                {
                    this.SearchForecastParam();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 修改预测数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditData_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                CafcService.ForecastParam editParamObj = this.GetEidtForecastParamObj();
                if (editParamObj != null)
                {
                    this.defaultPrjId = editParamObj.Prj_Id;
                    AddNewParamForm editParamForm = new AddNewParamForm(StaticUtil.EditOp, editParamObj);
                    DialogResult editResult = editParamForm.ShowDialog();

                    if (editResult == DialogResult.OK)
                    {
                        this.SearchForecastParam(); ;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 删除预测数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelData_ItemClick(object sender, ItemClickEventArgs e)
        {
            string msg = string.Empty;

            try
            {
                this.gvParam.PostEditor();
                CafcService.ForecastParam[] paramObjArr = (CafcService.ForecastParam[])this.gvParam.DataSource;
                List<string> selectParamIdList = new List<string>();
                if (paramObjArr != null)
                {
                    foreach (CafcService.ForecastParam paramObj in (CafcService.ForecastParam[])this.gvParam.DataSource)
                    {
                        if (paramObj.Check)
                        {
                            selectParamIdList.Add(paramObj.Param_Id.ToString());
                        }
                    }
                }

                if (selectParamIdList.Count > 0)
                {
                    bool result = cafcService.DelForecastParam(Utils.userId, Utils.password, selectParamIdList.ToArray());

                    if (result)
                    {
                        this.SearchForecastParam();
                        msg = "数据删除成功!";
                    }
                    else
                    {
                        msg = "数据删除失败";
                    }
                }
                else
                {
                    msg = "请选择要删除的数据";
                }
            }
            catch (Exception ex)
            {
                msg = "数据删除失败：" + ex.Message;
            }
            MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 查询预测项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchPrj_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.SearchForecastPrj();
        }

        #endregion

        #region 预测数据

        private void btnSearchData_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.SearchForecastParam();
        }

        private void btnNeSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            msg = this.DataBinds(StaticUtil.NeCafc, this.defaultPrjId);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnTeSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;

            msg = this.DataBinds(StaticUtil.TeCafc, this.defaultPrjId);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvParam.PostEditor();
            CafcService.ForecastParam[] detailObjArr = (CafcService.ForecastParam[])this.gvParam.DataSource;

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
        private void btnClearAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvParam.PostEditor();
            CafcService.ForecastParam[] detailObjArr = (CafcService.ForecastParam[])this.gvParam.DataSource;

            foreach (var item in detailObjArr)
            {
                item.Check = false;
            }

            this.gvParam.PostEditor();
            this.gvParam.RefreshData();
        }

        #endregion

        /// <summary>
        /// 查询预测项目信息
        /// </summary>
        /// <returns></returns>
        private string SearchForecastPrj()
        {
            string msg = string.Empty;
            try
            {
                CafcService.ForecastPrj[] prjObjArr = cafcService.QueryForecastPrj(Utils.userId, Utils.password);

                this.gcForePrj.DataSource = prjObjArr;
                Utils.SelectItem(prjObjArr, false);

                this.gvParam.PostEditor();
                this.gvParam.RefreshData();
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        /// <summary>
        /// 查询预测数据信息
        /// </summary>
        /// <returns></returns>
        private string SearchForecastParam()
        {
            string msg = string.Empty;
            try
            {
                // 获取项目ID
                this.GetDefaultPrjId();
                if (!string.IsNullOrEmpty(this.defaultPrjId))
                {
                    CafcService.ForecastParam[] paramObjArr = cafcService.QueryForecastParam(Utils.userId, Utils.password, this.defaultPrjId);

                    this.gcParam.DataSource = paramObjArr;
                    Utils.SelectItem(paramObjArr, false);

                    this.gvParam.PostEditor();
                    this.gvParam.RefreshData();
                }
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        /// <summary>
        /// 重置预测数据信息
        /// </summary>
        /// <returns></returns>
        private string ResetForecastParam()
        {
            string msg = string.Empty;
            try
            {
                this.gcParam.DataSource = null;

                this.gvParam.PostEditor();
                this.gvParam.RefreshData();
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        /// <summary>
        /// 删除预测项目
        /// </summary>
        /// <param name="prjIds"></param>
        /// <returns></returns>
        private string DelForecastPrj(string prjIds)
        {
            string msg = string.Empty;
            try
            {
                //bool result = cafcService.QueryForecastPrj(Utils.userId, Utils.password);
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        //private bool DelForecastParam(string[] paramIdArr)
        //{
        //    string msg = string.Empty;
        //    try
        //    {
        //        bool result = cafcService.DelForecastParam(Utils.userId, Utils.password, paramIdArr);
        //    }
        //    catch (Exception ex)
        //    {
        //        msg += string.Format("查询出错：{0}\r\n", ex.Message);
        //    }

        //    return msg;
        //}

        /// <summary>
        /// 获取需要修改的预测项目信息
        /// </summary>
        /// <returns></returns>
        private CafcService.ForecastPrj GetEidtForecastPrjObj()
        {
            try
            {
                CafcUtils cafcUtil = new CafcUtils();

                this.gvForePrj.PostEditor();
                CafcService.ForecastPrj[] prjObjArr = (CafcService.ForecastPrj[])this.gvForePrj.DataSource;
                List<CafcService.ForecastPrj> selectPrjObjList = new List<CafcService.ForecastPrj>();
                if (prjObjArr != null)
                {
                    foreach (CafcService.ForecastPrj prjObj in (CafcService.ForecastPrj[])this.gvForePrj.DataSource)
                    {
                        if (prjObj.Check)
                        {
                            selectPrjObjList.Add(prjObj);
                        }
                    }
                }

                if (selectPrjObjList != null)
                {
                    if (selectPrjObjList.Count > 1)
                    {
                        MessageBox.Show("只能选择一个项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return null;
                    }
                    else if (selectPrjObjList.Count < 1)
                    {
                        MessageBox.Show("请选择一个项目", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return null;
                    }
                    else
                    {
                        return selectPrjObjList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return null;
        }

        /// <summary>
        /// 获取需要修改的预测项目信息
        /// </summary>
        /// <returns></returns>
        private CafcService.ForecastParam GetEidtForecastParamObj()
        {
            try
            {
                this.gvParam.PostEditor();
                CafcService.ForecastParam[] paramObjArr = (CafcService.ForecastParam[])this.gvParam.DataSource;
                List<CafcService.ForecastParam> selectParamObjList = new List<CafcService.ForecastParam>();
                if (paramObjArr != null)
                {
                    foreach (CafcService.ForecastParam paramObj in (CafcService.ForecastParam[])this.gvParam.DataSource)
                    {
                        if (paramObj.Check)
                        {
                            selectParamObjList.Add(paramObj);
                        }
                    }
                }

                if (selectParamObjList != null)
                {
                    if (selectParamObjList.Count > 1)
                    {
                        MessageBox.Show("只能选择一条数据进行修改", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                    else if (selectParamObjList.Count < 1)
                    {
                        MessageBox.Show("请选择一条数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                    else
                    {
                        return selectParamObjList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cafcType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private string DataBinds(string cafcType, string prjId)
        {
            string msg = string.Empty;
            try
            {
                CafcService.FuelCafcAndTcafc[] cafcData = null;// this.GetCafcDetailData(cafcType, startTime, endTime);

                this.GetDefaultPrjId();

                if (!string.IsNullOrEmpty(prjId))
                {
                    if (cafcType == StaticUtil.NeCafc)
                    {
                        cafcData = cafcService.QueryForeCastNECafc(Utils.userId, Utils.password, prjId);
                        this.gcNeCafc.DataSource = cafcData;
                    }
                    else if (cafcType == StaticUtil.TeCafc)
                    {
                        cafcData = cafcService.QueryForeCastTECafc(Utils.userId, Utils.password, prjId);
                        this.gcTeCafc.DataSource = cafcData;
                    }
                }
            }
            catch (Exception ex)
            {
                msg += string.Format("查询出错：{0}\r\n", ex.Message);
            }

            return msg;
        }

        private void GetDefaultPrjId()
        {
            //if (string.IsNullOrEmpty(this.defaultPrjId))
            //{
                CafcService.ForecastPrj defaultPrjObj = this.GetEidtForecastPrjObj();
                if (defaultPrjObj != null)
                {
                    this.defaultPrjId = defaultPrjObj.Prj_Id;
                }
            //}
        }

        private void gvForePrj_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var a = (DevExpress.XtraGrid.Views.Grid.GridView)sender;
                CafcService.ForecastPrj[] prjObjArr = (CafcService.ForecastPrj[])a.DataSource;

                Utils.SelectItem(prjObjArr, false);
                this.gvForePrj.PostEditor();

                prjObjArr[e.RowHandle].Check = true;

                this.gvForePrj.PostEditor();
                this.gvForePrj.RefreshData();

                this.SearchForecastParam();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void btnExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                saveFileDialog.FileName = "预测数据";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    //options.ExportMode = XlsExportMode.SingleFile;

                    this.gvParam.ExportToXls(saveFileDialog.FileName, options);

                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 双击预测项目行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gvForePrj_DoubleClick(object sender, EventArgs e)
        {
            ColumnView cv = (ColumnView)gcForePrj.FocusedView;
            CafcService.ForecastPrj prjObj = (CafcService.ForecastPrj)cv.GetFocusedRow();

            try
            {
                if (prjObj != null)
                {
                    AddForePrjForm editPrjForm = new AddForePrjForm(StaticUtil.EditOp, prjObj);
                    DialogResult editResult = editPrjForm.ShowDialog();

                    if (editResult == DialogResult.OK)
                    {
                        this.SearchForecastPrj(); ;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}