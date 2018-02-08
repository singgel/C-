using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using FuelDataModel;
using FuelDataSysClient.SubForm;
using System.Data.OleDb;
using System.IO;
using FuelDataSysClient.Tool.Tool_Porsche;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Utils_Control;

namespace FuelDataSysClient.Form_DBManager.Form_Porsche
{
    public partial class ImportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        PorscheUtils pu = null;
        public ImportForm()
        {
            InitializeComponent();
            pu = new PorscheUtils(true);
        }

        /// <summary>
        /// 导入VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportVin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            PorscheUtils utils = new PorscheUtils(true);
            FolderDialog openFolder = new FolderDialog();
            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, utils.VinFileName);
                    if (fileNameList.Count > 0)
                    {
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;

                        // 获取全部车型参数数据，用作合并VIN数据
                        bool IsMainDataExist = utils.GetMainData();
                        bool IsLtggExist = utils.GetOtherMainData("LTGG");
                        bool IsLjExist = utils.GetOtherMainData("LJ");
                        bool IsZczbzlExist = utils.GetOtherMainData("ZCZBZL");
                        StringBuilder sbMsg = new StringBuilder("系统中不存在");
                        bool flag = false;
                        if (!IsMainDataExist)
                        {
                            flag = true;
                            sbMsg.Append("\r\n \t车型参数数据");
                        }
                        if (!IsLtggExist)
                        {
                            flag = true;
                            sbMsg.Append("\r\n \t轮胎规格数据");
                        }
                        if (!IsLjExist)
                        {
                            flag = true;
                            sbMsg.Append("\r\n \t轮距数据");
                        }
                        if (!IsZczbzlExist)
                        {
                            flag = true;
                            sbMsg.Append("\r\n \t整车整备质量数据");
                        }

                        if (flag)
                        {
                            MessageBox.Show(sbMsg.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            // fileNameMsg += Path.GetFileName(fileName) + "\r\n";

                            // 导入filename文件信息
                            returnMsg += utils.ImportVinData(fileName, folderPath);
                        }

                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = "导入结果";
                        mf.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("目录" + folderPath + "下没有文件" + utils.VinFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 生成燃料消耗量数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnGenerate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string message = string.Empty;
            PorscheUtils porscheUtils = new PorscheUtils(false);

            try
            {
                DataSet dsVin = porscheUtils.GetImportedVinData("");
                if (dsVin.Tables[0].Rows.Count > 0)
                {
                    // 获取全部车型参数数据，用作合并VIN数据
                    bool IsMainDataExist = porscheUtils.GetMainData();
                    bool IsLtggExist = porscheUtils.GetOtherMainData("LTGG");
                    bool IsLjExist = porscheUtils.GetOtherMainData("LJ");
                    bool IsZczbzlExist = porscheUtils.GetOtherMainData("ZCZBZL");
                    StringBuilder sbMsg = new StringBuilder("系统中不存在");
                    bool flag = false;
                    if (!IsMainDataExist)
                    {
                        flag = true;
                        sbMsg.Append("\r\n \t车型参数数据");
                    }
                    if (!IsLtggExist)
                    {
                        flag = true;
                        sbMsg.Append("\r\n \t轮胎规格数据");
                    }
                    if (!IsLjExist)
                    {
                        flag = true;
                        sbMsg.Append("\r\n \t轮距数据");
                    }
                    if (!IsZczbzlExist)
                    {
                        flag = true;
                        sbMsg.Append("\r\n \t整车整备质量数据");
                    }

                    if (flag)
                    {
                        MessageBox.Show(sbMsg.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    message = porscheUtils.SaveVinInfo(dsVin);
                }
                else
                {
                    MessageBox.Show("系统中不存在VIN数据");
                    return;
                }
            }
            catch (Exception ex)
            {
                message += ex.Message;
            }

            MessageForm mf = new MessageForm(message);
            Utils.SetFormMid(mf);
            mf.Text = "生成结果";
            mf.ShowDialog();

        }

        /// <summary>
        /// 导入车型参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportMainData("IMPORT");
        }

        /// <summary>
        /// 导入轮胎规格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportLtgg_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportOtherMainData("IMPORT", "LTGG", "LTGG");
        }

        /// <summary>
        /// 导入轮距
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportLj_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportOtherMainData("IMPORT", "LJ", "LJ");
        }

        /// <summary>
        /// 导入整车整备质量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportZczbzl_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportOtherMainData("IMPORT", "ZCZBZL", "ZCZBZL");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportMainData("UPDATE");
        }

        /// <summary>
        /// 导入或修改车型参数
        /// </summary>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改已导入的车型参数</param>
        protected void ImportMainData(string importType)
        {
            string operateType = string.Empty;
            if (importType == "IMPORT")
            {
                operateType = "导入";
            }
            else if (importType == "UPDATE")
            {
                operateType = "修改";
            }

            PorscheUtils utils = new PorscheUtils(true);
            FolderDialog openFolder = new FolderDialog();

            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, utils.MainFileName);
                    if (fileNameList.Count > 0)
                    {
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;
                        List<string> mainUpdateList = new List<string>();

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            fileNameMsg += Path.GetFileName(fileName) + "\r\n";

                            // 导入filename文件信息
                            returnMsg += utils.ImportMainData(fileName, folderPath, importType, mainUpdateList);
                        }

                        if (string.IsNullOrEmpty(returnMsg))
                        {
                            MessageForm mf = new MessageForm("以下文件" + operateType + "成功：\r\n" + fileNameMsg);
                            Utils.SetFormMid(mf);
                            mf.Text = operateType + "成功";
                            mf.ShowDialog();
                        }
                        else
                        {
                            MessageForm mf = new MessageForm(returnMsg);
                            Utils.SetFormMid(mf);
                            mf.Text = operateType + "结果";
                            mf.ShowDialog();
                        }

                        if (importType == "IMPORT")
                        {
                            this.ShowMainData();
                        }
                        else if (importType == "UPDATE")
                        {
                            this.ShowUpdatedMainData(mainUpdateList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("目录" + folderPath + "下没有文件" + utils.MainFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(operateType + "失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importType"></param>
        /// <param name="paramName"></param>
        /// <param name="fileType">文件类别，如轮胎规格文件</param>
        protected void ImportOtherMainData(string importType, string paramName, string fileType)
        {
            string operateType = string.Empty;
            if (importType == "IMPORT")
            {
                operateType = "导入";
            }
            else if (importType == "UPDATE")
            {
                operateType = "修改";
            }

            PorscheUtils utils = new PorscheUtils(true);
            FolderDialog openFolder = new FolderDialog();
            //openFolder.

            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, utils.GetMainFileName(fileType));
                    if (fileNameList.Count > 0)
                    {
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;
                        List<string> mainUpdateList = new List<string>();

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            fileNameMsg += Path.GetFileName(fileName) + "\r\n";

                            // 导入filename文件信息
                            returnMsg += utils.ImportOtherMainData(fileName, folderPath, paramName, importType, mainUpdateList);
                        }

                        if (string.IsNullOrEmpty(returnMsg))
                        {
                            MessageForm mf = new MessageForm("以下文件" + operateType + "成功：\r\n" + fileNameMsg);
                            Utils.SetFormMid(mf);
                            mf.Text = operateType + "成功";
                            mf.ShowDialog();
                        }
                        else
                        {
                            MessageForm mf = new MessageForm(returnMsg);
                            Utils.SetFormMid(mf);
                            mf.Text = operateType + "结果";
                            mf.ShowDialog();
                        }


                        if (importType == "IMPORT")
                        {
                            switch (paramName)
                            {
                                case "LTGG":
                                    this.ShowOtherMainData("LTGG");
                                    break;
                                case "LJ":
                                    this.ShowOtherMainData("LJ");
                                    break;
                                case "ZCZBZL":
                                    this.ShowOtherMainData("ZCZBZL");
                                    break;
                                default: break;
                            }
                        }
                        else if (importType == "UPDATE")
                        {
                            this.ShowUpdatedMainData(mainUpdateList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("目录" + folderPath + "下没有文件" + utils.MainFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(operateType + "失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 修改车型参数表关联数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateRelData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 获取车型参数编号
            string mainIds = string.Empty;
            PorscheUtils porscheUtils = new PorscheUtils(true);
            try
            {
                List<string> ctnyList = porscheUtils.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource);
                List<string> fcdsList = porscheUtils.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);

                if (ctnyList.Count + fcdsList.Count < 1)
                {
                    MessageBox.Show("请选择车型参数信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (string ctnyId in ctnyList)
                {
                    mainIds += string.Format(",'{0}'", ctnyId);
                }
                foreach (string fcdsId in fcdsList)
                {
                    mainIds += string.Format(",'{0}'", fcdsId);
                }

                if (!string.IsNullOrEmpty(mainIds))
                {
                    mainIds = mainIds.Substring(1);
                }

                ReviewUpdateVinForm reviewVinForm = new ReviewUpdateVinForm(mainIds);
                reviewVinForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //全选
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRLDCExport.SelectedTabPage == tpCtny)
            {
                this.gvCtny.FocusedRowHandle = 0;
                this.gvCtny.FocusedColumn = this.gvCtny.Columns[1];
                GridControlHelper.SelectItem(this.gvCtny, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpFcds)
            {
                this.gvFcds.FocusedRowHandle = 0;
                this.gvFcds.FocusedColumn = this.gvFcds.Columns[1];
                GridControlHelper.SelectItem(this.gvFcds, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpCds)
            {
                this.gvCds.FocusedRowHandle = 0;
                this.gvCds.FocusedColumn = this.gvCds.Columns[1];
                GridControlHelper.SelectItem(this.gvCds, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpCdd)
            {
                this.gvCdd.FocusedRowHandle = 0;
                this.gvCdd.FocusedColumn = this.gvCdd.Columns[1];
                GridControlHelper.SelectItem(this.gvCdd, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpRldc)
            {
                this.gvRldc.FocusedRowHandle = 0;
                this.gvRldc.FocusedColumn = this.gvRldc.Columns[1];
                GridControlHelper.SelectItem(this.gvRldc, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpLtgg)
            {
                this.gvLtgg.FocusedRowHandle = 0;
                this.gvLtgg.FocusedColumn = this.gvLtgg.Columns[1];
                GridControlHelper.SelectItem(this.gvLtgg, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpLj)
            {
                this.gvLj.FocusedRowHandle = 0;
                this.gvLj.FocusedColumn = this.gvLj.Columns[1];
                GridControlHelper.SelectItem(this.gvLj, true);
            }
            else if (btnRLDCExport.SelectedTabPage == tpZczbzl)
            {
                this.gvZb.FocusedRowHandle = 0;
                this.gvZb.FocusedColumn = this.gvZb.Columns[1];
                GridControlHelper.SelectItem(this.gvZb, true);
            }
        }

        //取消全选
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRLDCExport.SelectedTabPage == tpCtny)
            {
                this.gvCtny.FocusedRowHandle = 0;
                this.gvCtny.FocusedColumn = this.gvCtny.Columns[1];
                GridControlHelper.SelectItem(this.gvCtny, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpFcds)
            {
                this.gvFcds.FocusedRowHandle = 0;
                this.gvFcds.FocusedColumn = this.gvFcds.Columns[1];
                GridControlHelper.SelectItem(this.gvFcds, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpCds)
            {
                this.gvCds.FocusedRowHandle = 0;
                this.gvCds.FocusedColumn = this.gvCds.Columns[1];
                GridControlHelper.SelectItem(this.gvCds, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpCdd)
            {
                this.gvCdd.FocusedRowHandle = 0;
                this.gvCdd.FocusedColumn = this.gvCdd.Columns[1];
                GridControlHelper.SelectItem(this.gvCdd, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpRldc)
            {
                this.gvRldc.FocusedRowHandle = 0;
                this.gvRldc.FocusedColumn = this.gvRldc.Columns[1];
                GridControlHelper.SelectItem(this.gvRldc, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpLtgg)
            {
                this.gvLtgg.FocusedRowHandle = 0;
                this.gvLtgg.FocusedColumn = this.gvLtgg.Columns[1];
                GridControlHelper.SelectItem(this.gvLtgg, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpLj)
            {
                this.gvLj.FocusedRowHandle = 0;
                this.gvLj.FocusedColumn = this.gvLj.Columns[1];
                GridControlHelper.SelectItem(this.gvLj, false);
            }
            else if (btnRLDCExport.SelectedTabPage == tpZczbzl)
            {
                this.gvZb.FocusedRowHandle = 0;
                this.gvZb.FocusedColumn = this.gvZb.Columns[1];
                GridControlHelper.SelectItem(this.gvZb, false);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRLDCExport.SelectedTabPage == tpCtny)
            {
                this.SearchMainData("CTNY");
            }
            else if (btnRLDCExport.SelectedTabPage == tpFcds)
            {
                this.SearchMainData("FCDS");
            }
            else if (btnRLDCExport.SelectedTabPage == tpLtgg)
            {
                this.ShowOtherMainData("LTGG");
            }
            else if (btnRLDCExport.SelectedTabPage == tpLj)
            {
                this.ShowOtherMainData("LJ");
            }
            else if (btnRLDCExport.SelectedTabPage == tpZczbzl)
            {
                this.ShowOtherMainData("ZCZBZL");
            }
        }

        /// <summary>
        /// 删除车型参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barDelMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRLDCExport.SelectedTabPage == tpCtny)
            {
                this.DeleteMain("CTNY");
            }
            else if (btnRLDCExport.SelectedTabPage == tpFcds)
            {
                this.DeleteMain("FCDS");
            }
            else if (btnRLDCExport.SelectedTabPage == tpCds)
            {
                this.DeleteMain("CDS");
            }
            else if (btnRLDCExport.SelectedTabPage == tpCdd)
            {
                this.DeleteMain("CDD");
            }
            else if (btnRLDCExport.SelectedTabPage == tpRldc)
            {
                this.DeleteMain("RLDC");
            }
            else if (btnRLDCExport.SelectedTabPage == tpLtgg)
            {
                this.DeleteOtherMain("LTGG");
            }
            else if (btnRLDCExport.SelectedTabPage == tpLj)
            {
                this.DeleteOtherMain("LJ");
            }
            else if (btnRLDCExport.SelectedTabPage == tpZczbzl)
            {
                this.DeleteOtherMain("ZCZBZL");
            }
        }

        /// <summary>
        /// 删除车型参数
        /// </summary>
        /// <param name="deleteType">修改类型：“CTNY”表示修改传统能源车型参数对应的油耗数据；“FCDS”表示修改非插电式车型参数对应的油耗数据</param>
        protected void DeleteMain(string deleteFuel)
        {
            // 记录修改后的CPOS或者车型参数编号
            string mainIds = string.Empty;
            PorscheUtils porscheUtil = new PorscheUtils(false);
            try
            {
                List<string> mainIdList = new List<string>();

                // 冲界面获取修改后的车型参数或者cpos信息
                if (deleteFuel == "CTNY")
                {
                    mainIdList = porscheUtil.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource);
                }
                else if (deleteFuel == "FCDS")
                {
                    mainIdList = porscheUtil.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);
                }
                else if (deleteFuel == "CDS")
                {
                    mainIdList = porscheUtil.GetMainIdFromControl(this.gvCds, (DataTable)this.dgvCds.DataSource);
                }
                else if (deleteFuel == "CDD")
                {
                    mainIdList = porscheUtil.GetMainIdFromControl(this.gvCdd, (DataTable)this.gvCdd.DataSource);
                }
                else if (deleteFuel == "RLDC")
                {
                    mainIdList = porscheUtil.GetMainIdFromControl(this.gvRldc, (DataTable)this.gvRldc.DataSource);
                }
                if (mainIdList.Count < 1)
                {
                    MessageBox.Show("请选择要删除的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (string mainId in mainIdList)
                {
                    mainIds += string.Format(",'{0}'", mainId);
                }

                if (!string.IsNullOrEmpty(mainIds))
                {
                    mainIds = mainIds.Substring(1);
                }

                string delMsg = string.Empty;
                if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    delMsg = porscheUtil.DeleteMain(deleteFuel, mainIds);

                    if (deleteFuel == "CTNY")
                    {
                        this.SearchMainData("CTNY");
                    }
                    else if (deleteFuel == "FCDS")
                    {
                        this.SearchMainData("FCDS");
                    }
                    else if (deleteFuel == "CDS")
                    {
                        this.SearchMainData("CDS");
                    }
                    else if (deleteFuel == "CDD")
                    {
                        this.SearchMainData("CDD");
                    }
                    else if (deleteFuel == "RLDC")
                    {
                        this.SearchMainData("RLDC");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 删除q其他车型参数
        /// </summary>
        /// <param name="deleteType">修改类型：“CTNY”表示修改车型参数对应的油耗数据；“CPOS”表示修改轮胎规格对应的油耗数据</param>
        protected void DeleteOtherMain(string deleteType)
        {
            // 记录修改后的CPOS或者车型参数编号
            PorscheUtils porscheUtil = new PorscheUtils(false);
            try
            {
                List<string> mainIdList = new List<string>();

                // 冲界面获取修改后的车型参数或者cpos信息
                if (deleteType == "LTGG")
                {
                    mainIdList = porscheUtil.GetMainParamIdFromControl(this.gvLtgg, (DataTable)this.gcLtgg.DataSource, "LTGG_ID");
                }
                else if (deleteType == "LJ")
                {
                    mainIdList = porscheUtil.GetMainParamIdFromControl(this.gvLj, (DataTable)this.gcLj.DataSource, "LJ_ID");
                }
                else if (deleteType == "ZCZBZL")
                {
                    mainIdList = porscheUtil.GetMainParamIdFromControl(this.gvZb, (DataTable)this.gcZb.DataSource, "ZCZBZL_ID");
                }
                if (mainIdList.Count < 1)
                {
                    MessageBox.Show("请选择要删除的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string delMsg = string.Empty;
                if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    delMsg = porscheUtil.DeleteOtherMain(deleteType, mainIdList);

                    if (deleteType == "LTGG")
                    {
                        this.ShowOtherMainData("LTGG");
                    }
                    else if (deleteType == "LJ")
                    {
                        this.ShowOtherMainData("LJ");
                    }
                    else if (deleteType == "ZCZBZL")
                    {
                        this.ShowOtherMainData("ZCZBZL");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查看已导入但还未生成燃料数据的VIN信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReviewVin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ReviewVinData vinForm = new ReviewVinData();
                vinForm.ShowDialog();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 查询已经导入的传统能源车型参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCtnySearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CTNY");
        }

        /// <summary>
        /// 查询已经导入的非插电式车型参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFcdsSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("FCDS");
        }

        /// <summary>
        /// 查询已经导入的插电式车型参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCdsSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CDS");
        }

        private void btnCddSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CDD");
        }

        private void btnRldcSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("RLDC");
        }

        /// <summary>
        /// 查询轮胎信息表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLtggSearch_Click(object sender, EventArgs e)
        {
            this.ShowOtherMainData("LTGG", this.tbLtMainId.Text, this.tbLtggId.Text, this.tbLtgg.Text);
        }

        /// <summary>
        /// 查询轮距表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLjSearch_Click(object sender, EventArgs e)
        {
            this.ShowOtherMainData("LJ", this.tbLjMainId.Text, this.tbLjId.Text, this.tbLj.Text);
        }

        /// <summary>
        /// 查询整车整备质量表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnZbSearch_Click(object sender, EventArgs e)
        {
            this.ShowOtherMainData("ZCZBZL", this.tbZbMainId.Text, this.tbZbId.Text, this.tbZb.Text);
        }

        /// <summary>
        /// 查询本地车型参数数据
        /// </summary>
        /// <param name="rllx"></param>
        protected void SearchMainData(string rllx)
        {
            try
            {
                string tableName = string.Empty;
                string paramNO = string.Empty;
                string clxh = string.Empty;
                string clzl = string.Empty;
                if (rllx == "CTNY")
                {
                    tableName = "CTNY_MAIN";
                    paramNO = tbCtParam.Text;
                    clxh = tbCtClxh.Text;
                    clzl = tbCtClzl.Text;
                }
                else if (rllx == "FCDS")
                {
                    tableName = "FCDS_MAIN";
                    paramNO = tbFcdsParam.Text;
                    clxh = tbFcdsClxh.Text;
                    clzl = tbFcdsClzl.Text;
                }
                else if (rllx == "CDS")
                {
                    tableName = "CDS_MAIN";
                    paramNO = tbCdsParam.Text;
                    clxh = tbCdsClxh.Text;
                    clzl = tbCdsClzl.Text;
                }
                else if (rllx == "CDD")
                {
                    tableName = "CDD_MAIN";
                    paramNO = tbCddParam.Text;
                    clxh = tbCddClxh.Text;
                    clzl = tbCddClzl.Text;
                }
                else if (rllx == "RLDC")
                {
                    tableName = "RLDC_MAIN";
                    paramNO = tbRldcParam.Text;
                    clxh = tbRldcClxh.Text;
                    clzl = tbRldcClzl.Text;
                }

                // 获取本地车型参数信息
                string sql = string.Format(@"SELECT * FROM {0} WHERE STATUS='1'", tableName);
                string sw = "";
                if (!string.IsNullOrEmpty(paramNO))
                {
                    sw += " AND (MAIN_ID like '%" + paramNO + "%')";
                }
                if (!string.IsNullOrEmpty(clxh))
                {
                    sw += " AND (CLXH LIKE '%" + clxh + "%')";
                }
                if (!string.IsNullOrEmpty(clzl))
                {
                    sw += " AND (CLZL LIKE '%" + clzl + "%')";
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                if (rllx == "CTNY")
                {
                    this.dgvCtny.DataSource = dt;
                    this.gvCtny.BestFitColumns();
                    Utils.SelectItem(this.gvCtny, false);
                    lblCtSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                else if (rllx == "FCDS")
                {
                    this.dgvFcds.DataSource = dt;
                    this.gvFcds.BestFitColumns();
                    Utils.SelectItem(this.gvFcds, false);
                    lblFcdsSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                else if (rllx == "CDS")
                {
                    this.dgvCds.DataSource = dt;
                    this.gvCds.BestFitColumns();
                    Utils.SelectItem(this.gvCds, false);
                    lblCdsSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                else if (rllx == "CDD")
                {
                    this.dgvCdd.DataSource = dt;
                    this.gvCdd.BestFitColumns();
                    Utils.SelectItem(this.gvCdd, false);
                    lblCddSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                else if (rllx == "RLDC")
                {
                    this.dgvRldc.DataSource = dt;
                    this.gvRldc.BestFitColumns();
                    Utils.SelectItem(this.gvRldc, false);
                    lblRldcSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 显示更新后的车型参数参数信息
        /// </summary>
        /// <param name="mainUpdateList"></param>
        protected void SearchUpdatedMainData(List<string> mainUpdateList)
        {
            string mainIds = string.Empty;
            try
            {
                if (mainUpdateList != null && mainUpdateList.Count > 0)
                {
                    foreach (string id in mainUpdateList)
                    {
                        mainIds += string.Format(",'{0}'", id);
                    }
                    if (!string.IsNullOrEmpty(mainIds))
                    {
                        mainIds = mainIds.Substring(1);
                    }
                }
                else
                {
                    mainIds = "''";
                }
                // 获取本地车型参数信息
                string sqlCtny = string.Format(@"SELECT * FROM CTNY_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
                string sqlFcds = string.Format(@"SELECT * FROM FCDS_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
                string sqlCds = string.Format(@"SELECT * FROM CDS_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
                string sqlCdd = string.Format(@"SELECT * FROM CDD_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
                string sqlRldc = string.Format(@"SELECT * FROM RLDC_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);


                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                DataSet dsFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
                DataSet dsCds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCds, null);
                DataSet dsCdd = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCdd, null);
                DataSet dsRldc = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlRldc, null);

                DataTable dtCtny = dsCtny.Tables[0];
                dtCtny.Columns.Add("check", System.Type.GetType("System.Boolean"));
                DataTable dtFcds = dsFcds.Tables[0];
                dtFcds.Columns.Add("check", System.Type.GetType("System.Boolean"));
                DataTable dtCds = dsCds.Tables[0];
                dtCds.Columns.Add("check", System.Type.GetType("System.Boolean"));
                DataTable dtCdd = dsCdd.Tables[0];
                dtCdd.Columns.Add("check", System.Type.GetType("System.Boolean"));
                DataTable dtRldc = dsRldc.Tables[0];
                dtRldc.Columns.Add("check", System.Type.GetType("System.Boolean"));

                dgvCtny.DataSource = dtCtny;
                Utils.SelectItem(this.gvCtny, true);
                lblCtSum.Text = string.Format("共{0}条", dtCtny.Rows.Count);

                dgvFcds.DataSource = dtFcds;
                Utils.SelectItem(this.gvFcds, true);
                lblFcdsSum.Text = string.Format("共{0}条", dtFcds.Rows.Count);

                dgvCds.DataSource = dtCds;
                Utils.SelectItem(this.gvCds, true);
                lblCdsSum.Text = string.Format("共{0}条", dtCds.Rows.Count);

                dgvCdd.DataSource = dtCdd;
                Utils.SelectItem(this.gvCdd, true);
                lblCddSum.Text = string.Format("共{0}条", dtCdd.Rows.Count);

                dgvRldc.DataSource = dtRldc;
                Utils.SelectItem(this.gvRldc, true);
                lblRldcSum.Text = string.Format("共{0}条", dtRldc.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 显示所有车型参数数据
        /// </summary>
        protected void ShowMainData()
        {
            this.SearchMainData("CTNY");
            this.SearchMainData("FCDS");
            this.SearchMainData("CDS");
            this.SearchMainData("CDD");
            this.SearchMainData("RLDC");
            //DataSet dsMainCtny = new DataSet();
            //DataSet dsMainFcds = new DataSet();

            //string sqlCtny = @"SELECT C.* FROM CTNY_MAIN C WHERE C.STATUS='1'";
            //string sqlFcds = @"SELECT F.* FROM FCDS_MAIN F WHERE F.STATUS='1'";

            //try
            //{
            //    dsMainCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
            //    dsMainFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);

            //    this.dgvCtny.DataSource = dsMainCtny.Tables[0];
            //    this.dgvFcds.DataSource = dsMainFcds.Tables[0];

            //    this.lblCtSum.Text = string.Format("共{0}条", dsMainCtny.Tables[0].Rows.Count);
            //    this.lblFcdsSum.Text = string.Format("共{0}条", dsMainFcds.Tables[0].Rows.Count);
            //}
            //catch (Exception)
            //{
            //}
        }

        /// <summary>
        /// 显示修改后的车型参数表数据
        /// </summary>
        /// <param name="mainUpdateList">修改过的车型参数编号</param>
        protected void ShowUpdatedMainData(List<string> mainUpdateList)
        {
            this.SearchUpdatedMainData(mainUpdateList);
        }

        /// <summary>
        /// 显示所有轮胎信息
        /// </summary>
        public void ShowOtherMainData(string paramName, string mainId = "", string paramId = "", string paramValue = "")
        {
            string tableName = string.Empty;
            string field1Name = string.Empty;
            string field2Name = string.Empty;
            XtraTabPage selectedPage = null;
            LabelControl selectedLbl = null;
            DevExpress.XtraGrid.GridControl selectGc = null;
            DevExpress.XtraGrid.Views.Grid.GridView selectGv = null;

            try
            {
                switch (paramName)
                {
                    case "LTGG":
                        tableName = "MAIN_LTGG";
                        field1Name = "LTGG_ID";
                        field2Name = "LTGG";
                        selectedPage = tpLtgg;
                        selectedLbl = lblLtggSum;
                        selectGc = gcLtgg;
                        selectGv = gvLtgg;
                        break;
                    case "LJ":
                        tableName = "MAIN_LJ";
                        field1Name = "LJ_ID";
                        field2Name = "LJ";
                        selectedPage = tpLj;
                        selectedLbl = lblLjSum;
                        selectGc = gcLj;
                        selectGv = gvLj;
                        break;
                    case "ZCZBZL":
                        tableName = "MAIN_ZCZBZL";
                        field1Name = "ZCZBZL_ID";
                        field2Name = "ZCZBZL";
                        selectedPage = tpZczbzl;
                        selectedLbl = lblZbSum;
                        selectGc = gcZb;
                        selectGv = gvZb;
                        break;
                    default: break;
                }

                // 获取本地轮胎信息信息
                string sql = string.Format(@"SELECT * FROM {0} WHERE 1=1", tableName);
                string sqlOrder = string.Format(@" ORDER BY MAIN_ID,{0},{1}", field1Name, field2Name);
                string sw = "";
                if (!string.IsNullOrEmpty(mainId))
                {
                    sw += " AND (MAIN_ID LIKE '%" + mainId + "%')";
                }
                if (!string.IsNullOrEmpty(paramId))
                {
                    sw += string.Format(@" AND ({0} LIKE '%{1}%')", field1Name, paramId);
                }
                if (!string.IsNullOrEmpty(paramValue))
                {
                    sw += string.Format(@" AND ({0} LIKE '%{1}%')", field2Name, paramValue);
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw + sqlOrder, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                selectGc.DataSource = dt;
                selectGv.BestFitColumns();
                Utils.SelectItem(selectGv, false);
                selectedLbl.Text = string.Format("共{0}条", dt.Rows.Count);

                btnRLDCExport.SelectedTabPage = selectedPage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        private void btnCtnyExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.dgvCtny.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.CTNYExport);
        }

        private void btnFCDSExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.dgvFcds.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.FCDSExport);
        }

        private void btnCDSExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.dgvCds.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.CDSExport);
        }

        private void btnCDDExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.dgvCdd.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.CDDExport);
        }

        private void btnRLExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.dgvCdd.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.RLDCExport);
        }

        private void btnLTGGExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.gcLtgg.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.LTGG);
        }

        private void btnLJExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.gcLj.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.LJ);
        }

        private void btnZCZBZLExport_Click(object sender, EventArgs e)
        {
            DataTable source = (DataTable)this.gcZb.DataSource;
            pu.ExportExcel(source, this, PorscheUtils.ZCZBZL);
        }

    }
}
