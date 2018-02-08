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
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using FuelDataSysClient.Tool;
using System.Threading;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_DBManager
{
    public partial class ImportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 生成燃料消耗量数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnGenerate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string message = string.Empty;
            MitsUtils utils = new MitsUtils();
            try
            {
                DataTable dtVin = utils.GetImportedVinData("");

                if (dtVin.Rows.Count > 0)
                {
                    bool IsMainDataExist = utils.GetMainData();
                    if (!IsMainDataExist)
                    {
                        MessageBox.Show("系统中不存在车型数据，请首先导入车型数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    message = utils.SaveVinInfo(dtVin);
                }
                else
                {
                    MessageBox.Show("系统中不存在VIN数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 导入VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportVin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MitsUtils utils = new MitsUtils();
            //this.gvCtny.PostEditor();
            //int[] selectedHandle;
            ////selectedHandle = this.gvCtny.GetSelectedRows();
            //if (selectedHandle.Count() > 0)
            //{
            //    if (selectedHandle[0] < 0)
            //    {
            //        MessageBox.Show("请选择车型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("请选择车型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}
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

                        // 获取全部主表数据，用作合并VIN数据
                        bool IsMainDataExist = utils.GetMainData();
                        if (!IsMainDataExist)
                        {
                            MessageBox.Show("系统中不存在车型数据，请首先导入车型数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        MessageBox.Show("目录" + folderPath + "下没有文件" + utils.VinFileName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 导入车型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportMainData("IMPORT");
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
        /// <param name="importType"></param>
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
            MitsUtils utils = new MitsUtils();
            FolderDialog openFolder = new FolderDialog();
            if (openFolder.DisplayDialog() == DialogResult.OK)
            {
                // 获取folderPath下以格式为utils.CocFileName的所有文件
                List<string> fileNameList = utils.GetFileName(openFolder.Path, utils.MainFileName);
                if (fileNameList.Count > 0)
                {
                    try
                    {
                        SplashScreenManager.ShowForm(typeof(DevWaitForm));
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;
                        List<string> mainUpdateList = new List<string>();
                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            fileNameMsg += Path.GetFileName(fileName) + Environment.NewLine;
                            // 导入filename文件信息
                            returnMsg += utils.ImportMainData(fileName, openFolder.Path, importType, mainUpdateList);
                        }
                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = operateType + "结果";
                        mf.ShowDialog();
                        if (importType == "IMPORT")
                        {
                            this.ShowMainData();
                        }
                        else if (importType == "UPDATE")
                        {
                            this.ShowUpdatedMainData(mainUpdateList);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SplashScreenManager.CloseForm();
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("目录{0}下没有文件{1}", openFolder.Path, utils.MainFileName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        //全选
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var gridView = (GridView)((GridControl)this.btnFcdsSearch.SelectedTabPage.Controls[0]).MainView;
            gridView.FocusedRowHandle = 0;
            gridView.FocusedColumn = gridView.Columns[1];
            GridControlHelper.SelectItem(gridView, true);
        }

        //取消全选
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var gridView = (GridView)((GridControl)this.btnFcdsSearch.SelectedTabPage.Controls[0]).MainView;
            gridView.FocusedRowHandle = 0;
            gridView.FocusedColumn = gridView.Columns[1];
            GridControlHelper.SelectItem(gridView, false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var gridControl = (GridControl)this.btnFcdsSearch.SelectedTabPage.Controls[0];

            var tableName = string.Empty;
            if (gridControl.Name.Equals("dgvCtny"))
            {
                tableName = "CTNY";
            }
            else if (gridControl.Name.Equals("dgvFcds"))
            {
                tableName = "FCDS";
            }
            else if (gridControl.Name.Equals("dgvCds"))
            {
                tableName = "CDS";
            }
            else if (gridControl.Name.Equals("dgvCdd"))
            {
                tableName = "CDD";
            }
            else if (gridControl.Name.Equals("dgvRldc"))
            {
                tableName = "RLDC";
            }

            this.SearchMainData(tableName);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 显示所有主表数据
        /// </summary>
        protected void ShowMainData()
        {
            this.SearchMainData("CTNY");
            //this.SearchMainData("FCDS");
            //DataSet dsMainCtny = new DataSet();
            //DataSet dsMainFcds = new DataSet();

            //string sqlCtny = @"SELECT C.* FROM MAIN_CTNY C WHERE C.STATUS='1'";
            //string sqlFcds = @"SELECT F.* FROM MAIN_FCDSHHDL F WHERE F.STATUS='1'";

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

        protected void ShowUpdatedMainData(List<string> mainUpdateList)
        {
            this.SearchUpdatedMainData(mainUpdateList);
        }

        /// <summary>
        /// 查询已经导入的传统能源主表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCtnySearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CTNY");
        }

        /// <summary>
        /// 查询已经导入的非插电式主表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.SearchMainData("FCDS");
        }

        /// <summary>
        /// 查询本地主表数据
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

                    clxh = tbCtClxh.Text;
                    clzl = tbCtClzl.Text;
                }
                else if (rllx == "FCDS")
                {
                    tableName = "FCDS_MAIN";

                    clxh = tbFcdsClxh.Text;
                    clzl = cbFcdsClzl.Text;
                }
                else if (rllx == "CDS")
                {
                    tableName = "CDS_MAIN";

                    clxh = tbCdsClxh.Text;
                    clzl = cbCdsClzl.Text;
                }
                else if (rllx == "CDD")
                {
                    tableName = "CDD_MAIN";

                    clxh = tbCddClxh.Text;
                    clzl = cbCddClzl.Text;
                }
                else if (rllx == "RLDC")
                {
                    tableName = "RLDC_MAIN";

                    clxh = tbRldcClxh.Text;
                    clzl = cbRldcClzl.Text;
                }
                //else if (rllx == "FCDS")
                //{
                //    tableName = "MAIN_FCDSHHDL";
                //    paramNO = tbFcdsParam.Text;
                //    clxh = tbFcdsClxh.Text;
                //    clzl = tbFcdsClzl.Text;
                //}

                // 获取本地主表信息
                string sql = string.Format(@"SELECT * FROM {0} where 1=1 ", tableName);
                string sw = "";

                if (!string.IsNullOrEmpty(clxh))
                {
                    sw += " and (CLXH like '%" + clxh + "%')";
                }
                if (!string.IsNullOrEmpty(clzl))
                {
                    sw += " and (CLZL like '%" + clzl + "%')";
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
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 显示更新后的主表参数信息
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
                // 获取本地主表信息
                string sqlCtny = string.Format(@"SELECT * FROM CTNY_MAIN WHERE CLXH IN ({0})", mainIds);
                //string sqlFcds = string.Format(@"SELECT * FROM MAIN_FCDSHHDL WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);


                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                //DataSet dsFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);

                DataTable dtCtny = dsCtny.Tables[0];
                dtCtny.Columns.Add("check", System.Type.GetType("System.Boolean"));
                //DataTable dtFcds = dsFcds.Tables[0];
                //dtFcds.Columns.Add("check", System.Type.GetType("System.Boolean"));

                dgvCtny.DataSource = dtCtny;
                Utils.SelectItem(this.gvCtny, true);
                lblCtSum.Text = string.Format("共{0}条", dtCtny.Rows.Count);

                //dgvFcds.DataSource = dtFcds;
                //Utils.SelectItem(this.gvFcds, true);
                //lblFcdsSum.Text = string.Format("共{0}条", dtFcds.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 更新主表关联数据
        private void btnUpdateRelData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 获取主表编号
            string mainIds = string.Empty;
            MitsUtils mitsUtil = new MitsUtils();
            try
            {
                var gridControl = (GridControl)this.btnFcdsSearch.SelectedTabPage.Controls[0];
                gridControl.MainView.PostEditor();

                GridView gv = (GridView)gridControl.MainView;
                DataTable dt = (DataTable)gridControl.DataSource;

                //List<string> ctnyList = mitsUtil.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource);
                //List<string> fcdsList = mitsUtil.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);
                List<string> ctnyList = mitsUtil.GetMainIdFromControl(gv, dt);


                //if (ctnyList.Count + fcdsList.Count < 1)
                //{
                //    MessageBox.Show("请选择主表信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}


                var rllxType = string.Empty;
                if (gridControl.Name.Equals("dgvCtny"))
                {
                    rllxType = "CTNY";
                }
                else if (gridControl.Name.Equals("dgvFcds"))
                {
                    rllxType = "FCDS";
                }
                else if (gridControl.Name.Equals("dgvCds"))
                {
                    rllxType = "CDS";
                }
                else if (gridControl.Name.Equals("dgvCdd"))
                {
                    rllxType = "CDD";
                }
                else if (gridControl.Name.Equals("dgvRldc"))
                {
                    rllxType = "RLDC";
                }

                foreach (string ctnyId in ctnyList)
                {
                    mainIds += string.Format(",'{0}'", ctnyId);
                }
                //foreach (string fcdsId in fcdsList)
                //{
                //    mainIds += string.Format(",'{0}'", fcdsId);
                //}

                if (!string.IsNullOrEmpty(mainIds))
                {
                    mainIds = mainIds.Substring(1);
                }
                if (string.IsNullOrEmpty(mainIds))
                {
                    MessageBox.Show("请选择车型数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                ReviewUpdateVinForm reviewVinForm = new ReviewUpdateVinForm(mainIds, rllxType);
                reviewVinForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 新增车型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ClxhParamForm clxhParam = new ClxhParamForm();
            if (clxhParam.ShowDialog() == DialogResult.OK)
            {
                this.ShowMainData();
            }
        }

        private void btnDeleteMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var gridControl = (GridControl)this.btnFcdsSearch.SelectedTabPage.Controls[0];
            gridControl.MainView.PostEditor();
            

            DataView dv = (DataView)gridControl.MainView.DataSource;
            if (dv == null)
            {
                MessageBox.Show("请选择车型数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var result = from DataRow dr in dv.Table.Rows
                    where (bool)dr["check"] == true
                    select dr["UNIQUE_CODE"];

            string selectedParamEntityIds = "";
            if (result != null)
            {
                foreach (var a in result)
                {
                    selectedParamEntityIds += ",'" + a + "'";
                }
            }

            if ("" == selectedParamEntityIds)
            {
                MessageBox.Show("请选择要删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            selectedParamEntityIds = selectedParamEntityIds.Substring(1);

            var tableName = string.Empty;
            if (gridControl.Name.Equals("dgvCtny"))
            {
                tableName = "CTNY";
            }
            else if (gridControl.Name.Equals("dgvFcds"))
            {
                tableName = "FCDS";
            }
            else if (gridControl.Name.Equals("dgvCds"))
            {
                tableName = "CDS";
            }
            else if (gridControl.Name.Equals("dgvCdd"))
            {
                tableName = "CDD";
            }
            else if (gridControl.Name.Equals("dgvRldc"))
            {
                tableName = "RLDC";
            }
            string sql = @"delete * from " + tableName + "_MAIN where UNIQUE_CODE in (" + selectedParamEntityIds + ")";
            int i = AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);
            this.SearchMainData(tableName);
        }

      

        private void btnFcds2Search_Click(object sender, EventArgs e)
        {
            this.SearchMainData("FCDS");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtExport = (DataTable)dgvFcds.DataSource;
                if (dtExport == null)
                {
                    MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //MitsUtils mitsUtil = new MitsUtils();
                    //mitsUtil.ExportExcel(saveFileDialog.FileName, dtExport);
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    options.ExportMode = XlsExportMode.SingleFile;


                    dgvFcds.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCdsSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CDS");
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtExport = (DataTable)dgvCds.DataSource;
                if (dtExport == null)
                {
                    MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //MitsUtils mitsUtil = new MitsUtils();
                    //mitsUtil.ExportExcel(saveFileDialog.FileName, dtExport);
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    options.ExportMode = XlsExportMode.SingleFile;


                    dgvCds.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCddSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CDD");
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dtExport = (DataTable)dgvCdd.DataSource;
                if (dtExport == null)
                {
                    MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //MitsUtils mitsUtil = new MitsUtils();
                    //mitsUtil.ExportExcel(saveFileDialog.FileName, dtExport);
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    options.ExportMode = XlsExportMode.SingleFile;


                    dgvCdd.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRldcSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("RLDC");
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {

            try
            {
                DataTable dtExport = (DataTable)dgvRldc.DataSource;
                if (dtExport == null)
                {
                    MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //MitsUtils mitsUtil = new MitsUtils();
                    //mitsUtil.ExportExcel(saveFileDialog.FileName, dtExport);
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    options.ExportMode = XlsExportMode.SingleFile;


                    dgvRldc.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    

        private void simpleButton1_Click_1(object sender, EventArgs e)
        {
            try
            {
                DataTable dtExport = (DataTable)dgvCtny.DataSource;
                if (dtExport == null)
                {
                    MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    //MitsUtils mitsUtil = new MitsUtils();
                    //mitsUtil.ExportExcel(saveFileDialog.FileName, dtExport);
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    options.ExportMode = XlsExportMode.SingleFile;


                    dgvCtny.ExportToXls(saveFileDialog.FileName, options);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
