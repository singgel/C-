using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using FuelDataSysClient.Tool.Tool_Chrysler;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_SJSB.Form_Chrysler
{
    public partial class ImportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 导入VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportVin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ChryslerUtils utils = new ChryslerUtils();
                FolderDialog openFolder = new FolderDialog();
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.MainFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, utils.VinFileName);
                    if (fileNameList.Count > 0)
                    {
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;

                        // 获取全部车型参数数据，用作合并VIN数据
                        bool IsMainDataExist = utils.GetMainData("MAIN_RLLX"); // "MAIN_RLLX"表示燃料数据表（传统，非插电式混合动力等）
                        if (!IsMainDataExist)
                        {
                            MessageBox.Show("系统中不存在车型参数数据，请首先导入车型参数数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        bool IsCposDataExist = utils.GetMainData("MAIN_FUEL"); // "MAIN_FUEL"表示油耗值数据
                        if (!IsCposDataExist)
                        {
                            MessageBox.Show("系统中不存在油耗值数据，请首先导入油耗值数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("导入失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ChryslerUtils chryslerUtil = new ChryslerUtils();
            try
            {

                DataTable dtVin = chryslerUtil.GetImportedVinData("");

                if (dtVin.Rows.Count > 0)
                {

                    // 获取全部车型参数数据，用作合并VIN数据
                    bool IsMainDataExist = chryslerUtil.GetMainData("MAIN_FUEL"); // "MAIN_FUEL"表示燃料数据表（传统，非插电式混合动力等）
                    if (!IsMainDataExist)
                    {
                        MessageBox.Show("系统中不存在车型参数数据，请首先导入车型参数数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    bool IsCposDataExist = chryslerUtil.GetMainData("MAIN_CPOS"); // "MAIN_CPOS"表示轮胎信息表
                    if (!IsCposDataExist)
                    {
                        MessageBox.Show("系统中不存在油耗值数据，请首先导入油耗值数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    message = chryslerUtil.SaveVinInfo(dtVin);
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
        /// 导入车型参数表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportMainData("IMPORT");
        }

        /// <summary>
        /// 导入油耗值数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportFuel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportFuelData("IMPORT");
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

            try
            {
                ChryslerUtils utils = new ChryslerUtils();
                FolderDialog openFolder = new FolderDialog();
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.MainFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, utils.MainFileName);
                    if (fileNameList.Count > 0)
                    {
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;

                        //更新列表，记录更新的数据。存车型参数编号，初始为空
                        List<string> mainUpdateList = new List<string>();

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            fileNameMsg += Path.GetFileName(fileName) + "\r\n";

                            // 导入filename文件信息
                            returnMsg += utils.ImportMainData(fileName, folderPath, importType, mainUpdateList);
                        }

                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = operateType + "结果";
                        mf.ShowDialog();


                        if (importType == "IMPORT")
                        {
                            // 如果是新导入数据，导入完成后显示所有数据
                            this.ShowMainData();
                        }
                        else if (importType == "UPDATE")
                        {
                            // 如果是修改数据，界面只显示修改过的数据（为了接下来修改和这些车型参数关联的燃料数据）。
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
        /// 导入或修改CPOS
        /// </summary>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改已导入的轮胎信息/param>
        protected void ImportFuelData(string importType)
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

            try
            {
                ChryslerUtils utils = new ChryslerUtils();
                FolderDialog openFolder = new FolderDialog();
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.MainFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, utils.CposFileName);
                    if (fileNameList.Count > 0)
                    {
                        string fileNameMsg = string.Empty;
                        string returnMsg = string.Empty;

                        //更新列表，记录更新的数据。存油耗值编号，初始为空
                        List<string> fuelUpdateList = new List<string>();

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            fileNameMsg += Path.GetFileName(fileName) + "\r\n";

                            // 导入filename文件信息
                            returnMsg += utils.ImportFuelData(fileName, folderPath, importType, fuelUpdateList);
                        }

                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = operateType + "结果";
                        mf.ShowDialog();

                        // 如果是新导入数据，导入完成后显示所有数据
                        if (importType == "IMPORT")
                        {
                            this.ShowCposFuelData();
                        }
                        else if (importType == "UPDATE")
                        {
                            // 如果是修改数据，界面只显示修改过的数据（为了接下来修改和这些轮胎信心关联的燃料数据）。
                            this.ShowUpdatedCposFuelData(fuelUpdateList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("目录" + folderPath + "下没有文件" + utils.CposFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 修改车型参数表数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportMainData("UPDATE");
        }

        /// <summary>
        /// 修改轮胎信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateFuel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ImportFuelData("UPDATE");
        }

        /// <summary>
        /// 修改车型参数表关联数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateRelData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tabPage_PARAM.SelectedTabPage == xtraTabPage1)
            {
                this.UpdateFuelData("MAIN");
            }
            else if (tabPage_PARAM.SelectedTabPage == xtraTabPage2)
            {
                this.UpdateFuelData("FUEL");
            }
        }

        /// <summary>
        /// 修改车型参数或者油耗值信息后，修改他们关联的油耗数据
        /// </summary>
        /// <param name="updateType">修改类型：“FUEL”表示修改车型参数对应的油耗数据；“CPOS”表示修改轮胎规格对应的油耗数据</param>
        protected void UpdateFuelData(string updateType)
        {
            try
            {
                // 记录修改后的CPOS或者车型参数编号
                string mainIds = string.Empty;
                ChryslerUtils chryslerUtil = new ChryslerUtils();
                List<string> ctnyList = new List<string>();

                // 冲界面获取修改后的车型参数或者cpos信息
                if (updateType == "MAIN")
                {
                    ctnyList = chryslerUtil.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource, updateType);
                    //List<string> fcdsList = chryslerUtil.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);

                    if (ctnyList.Count < 1)
                    {
                        MessageBox.Show("请首先选择车型参数数据或者油耗值数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else if (updateType == "FUEL")
                {
                    ctnyList = chryslerUtil.GetMainIdFromControl(this.gvCpos, (DataTable)this.gcCpos.DataSource, updateType);
                    if (ctnyList.Count < 1)
                    {
                        MessageBox.Show("请首先选择车型参数数据或者油耗值数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                foreach (string ctnyId in ctnyList)
                {
                    if (updateType == "MAIN")
                    {
                        mainIds += string.Format(",{0}", ctnyId);
                    }
                    else if (updateType == "FUEL")
                    {
                        mainIds += string.Format(",{0}", ctnyId);
                    }
                }
                //foreach (string fcdsId in fcdsList)
                //{
                //    mainIds += string.Format(",'{0}'", fcdsId);
                //}

                if (!string.IsNullOrEmpty(mainIds))
                {
                    mainIds = mainIds.Substring(1);
                }

                // 查看修改后的CPOS或车型参数关联到的原始油耗数据
                ReviewUpdateVinForm reviewVinForm = new ReviewUpdateVinForm(mainIds, updateType);
                reviewVinForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tabPage_PARAM.SelectedTabPage == xtraTabPage1)
            {
                Utils.SelectItem(this.gvCtny, true);
            }
            else if (tabPage_PARAM.SelectedTabPage == xtraTabPage2)
            {
                Utils.SelectItem(this.gvCpos, true);
            }
        }

        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tabPage_PARAM.SelectedTabPage == xtraTabPage1)
            {
                Utils.SelectItem(this.gvCtny, false);
            }
            else if (tabPage_PARAM.SelectedTabPage == xtraTabPage2)
            {
                Utils.SelectItem(this.gvCpos, false);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tabPage_PARAM.SelectedTabPage == xtraTabPage1)
            {
                this.ShowMainData();
            }
            else if (tabPage_PARAM.SelectedTabPage == xtraTabPage2)
            {
                this.ShowCposFuelData();
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
        /// 删除车型参数或CPOS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (tabPage_PARAM.SelectedTabPage == xtraTabPage1)
            {
                this.DeleteMainCpos("MAIN");
            }
            else if (tabPage_PARAM.SelectedTabPage == xtraTabPage2)
            {
                this.DeleteMainCpos("FUEL");
            }
        }

        /// <summary>
        /// 删除车型参数或CPOS
        /// </summary>
        /// <param name="deleteType">修改类型：“FUEL”表示修改车型参数对应的油耗数据；“CPOS”表示修改轮胎规格对应的油耗数据</param>
        protected void DeleteMainCpos(string deleteType)
        {
            // 记录修改后的CPOS或者车型参数编号
            string mainIds = string.Empty;
            ChryslerUtils chryslerUtil = new ChryslerUtils();
            try
            {
                List<string> ctnyList = new List<string>();

                // 冲界面获取修改后的车型参数或者cpos信息
                if (deleteType == "MAIN")
                {
                    ctnyList = chryslerUtil.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource, deleteType);
                    //List<string> fcdsList = chryslerUtil.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);
                }
                else if (deleteType == "FUEL")
                {
                    ctnyList = chryslerUtil.GetMainIdFromControl(this.gvCpos, (DataTable)this.gcCpos.DataSource, deleteType);
                }
                if (ctnyList.Count < 1)
                {
                    MessageBox.Show("请选择要删除的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (string ctnyId in ctnyList)
                {
                    if (deleteType == "MAIN")
                    {
                        mainIds += string.Format(",{0}", ctnyId);
                    }
                    else if (deleteType == "FUEL")
                    {
                        mainIds += string.Format(",{0}", ctnyId);
                    }
                }
                //foreach (string fcdsId in fcdsList)
                //{
                //    mainIds += string.Format(",'{0}'", fcdsId);
                //}

                if (!string.IsNullOrEmpty(mainIds))
                {
                    mainIds = mainIds.Substring(1);
                }

                string delMsg = string.Empty;
                if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    delMsg = chryslerUtil.DeleteMainCpos(deleteType, mainIds);

                    if (deleteType == "MAIN")
                    {
                        this.ShowMainData();
                    }
                    else if (deleteType == "FUEL")
                    {
                        this.ShowCposFuelData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            //this.SearchMainData("FCDS");
        }

        /// <summary>
        /// 查询轮胎信息表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFuelSearch_Click(object sender, EventArgs e)
        {
            ShowCposFuelData();
        }

        /// <summary>
        /// 显示所有车型参数表数据
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

        /// <summary>
        /// 显示修改后的车型参数表数据
        /// </summary>
        /// <param name="mainUpdateList">修改过的车型参数编号</param>
        protected void ShowUpdatedMainData(List<string> mainUpdateList)
        {
            string mainIds = string.Empty;
            try
            {
                if (mainUpdateList != null && mainUpdateList.Count > 0)
                {
                    foreach (string id in mainUpdateList)
                    {
                        mainIds += string.Format(",{0}", id);
                    }
                    if (!string.IsNullOrEmpty(mainIds))
                    {
                        mainIds = mainIds.Substring(1);
                    }
                }
                else
                {
                    mainIds = "-1";
                }
                // 获取本地车型参数信息
                string sqlCtny = string.Format(@"SELECT * FROM MAIN_CTNY WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
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

                this.tabPage_PARAM.SelectedTabPage = this.xtraTabPage1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 查询本地车型参数数据
        /// </summary>
        /// <param name="rllx">“CTNY”表示传统能源；“FCDS”表示非插电式混合动力</param>
        protected void SearchMainData(string rllx)
        {
            try
            {
                string tableName = string.Empty;
                string cpos = string.Empty;
                string yearMode = string.Empty;
                string clxh = string.Empty;
                string bsx = string.Empty;
                string pl = string.Empty;
                if (rllx == "CTNY")
                {
                    tableName = "MAIN_CTNY";
                    cpos = this.tbCtnyCpos.Text.Trim();
                    yearMode = this.tbCtnyYear.Text.Trim();
                    clxh = this.tbCtnyClxh.Text.Trim();
                    bsx = this.tbCtnyBsx.Text.Trim();
                    pl = this.tbCtnyPl.Text.Trim();
                }
                //else if (rllx == "FCDS")
                //{
                //    tableName = "MAIN_FCDSHHDL";
                //    vin8 = tbFcdsParam.Text;
                //    ltgg = tbFcdsClxh.Text;
                //    idenPamam = tbFcdsClzl.Text;
                //}

                // 获取本地车型参数信息
                string sql = string.Format(@"SELECT * FROM {0} WHERE STATUS='1'", tableName);
                string sw = "";
                if (!"".Equals(cpos))
                {
                    sw += " and (CPOS like '%" + cpos + "%')";
                }
                if (!"".Equals(yearMode))
                {
                    sw += " and (YEAR_MODE like '%" + yearMode + "%')";
                }
                if (!"".Equals(clxh))
                {
                    sw += " and (CLXH like '%" + clxh + "%')";
                }
                if (!"".Equals(pl))
                {
                    sw += " AND (CT_PL LIKE '%" + pl + "%')";
                }
                if (!"".Equals(bsx))
                {
                    sw += " AND (BSX LIKE '%" + bsx + "%')";
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                if (rllx == "CTNY")
                {
                    dgvCtny.DataSource = dt;
                    Utils.SelectItem(this.gvCtny, false);
                    lblCtSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                //else if (rllx == "FCDS")
                //{
                //    dgvFcds.DataSource = dt;
                //    Utils.SelectItem(this.gvFcds, false);
                //    lblFcdsSum.Text = string.Format("共{0}条", dt.Rows.Count);
                //}
                tabPage_PARAM.SelectedTabPage = xtraTabPage1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 显示所有油耗数据
        /// </summary>
        protected void ShowCposFuelData()
        {
            try
            {
                string cpos = this.tbFuelCpos.Text.Trim();
                string yearMode = this.tbFuelYear.Text.Trim();
                string clxh = this.tbFuelClxh.Text.Trim();
                string bsx = this.tbFuelBsx.Text.Trim();
                string pl = this.tbFuelPl.Text.Trim();
                string qcscqy = this.tbFuelQcscqy.Text.Trim();

                // 获取本地轮胎信息信息
                string sql = string.Format(@"SELECT * FROM MAIN_FUEL WHERE 1=1");
                string sqlOrder = " ORDER BY CPOS";
                string sw = "";
                if (!"".Equals(cpos))
                {
                    sw += " AND (CPOS LIKE '%" + cpos + "%')";
                }
                if (!"".Equals(yearMode))
                {
                    sw += " AND (YEAR_MODE LIKE '%" + yearMode + "%')";
                }
                if (!"".Equals(clxh))
                {
                    sw += " AND (CLXH LIKE '%" + clxh + "%')";
                }
                if (!"".Equals(pl))
                {
                    sw += " AND (CT_PL LIKE '%" + pl + "%')";
                }
                if (!"".Equals(bsx))
                {
                    sw += " AND (BSX LIKE '%" + bsx + "%')";
                }
                if (!"".Equals(qcscqy))
                {
                    sw += " AND (QCSCQY LIKE '%" + qcscqy + "%')";
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw + sqlOrder, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                gcCpos.DataSource = dt;
                Utils.SelectItem(this.gvCpos, false);
                lblCposSum.Text = string.Format("共{0}条", dt.Rows.Count);

                tabPage_PARAM.SelectedTabPage = xtraTabPage2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 显示修改后的CPOS信息
        /// </summary>
        /// <param name="mainUpdateList">修改过的CPOS编号</param>
        protected void ShowUpdatedCposFuelData(List<string> fuelUpdateList)
        {
            string mainId = string.Empty;
            try
            {
                if (fuelUpdateList != null && fuelUpdateList.Count > 0)
                {
                    foreach (string id in fuelUpdateList)
                    {
                        mainId += string.Format(",{0}", id);
                    }
                    if (!string.IsNullOrEmpty(mainId))
                    {
                        mainId = mainId.Substring(1);
                    }
                }
                else
                {
                    mainId = "-1";
                }
                // 获取本地CPOS信息
                string sqlCpos = string.Format(@"SELECT * FROM MAIN_FUEL WHERE FUEL_ID IN ({0})", mainId);

                DataSet dsCpos = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCpos, null);
                DataTable dtCpos = dsCpos.Tables[0];
                dtCpos.Columns.Add("check", System.Type.GetType("System.Boolean"));

                gcCpos.DataSource = dtCpos;
                Utils.SelectItem(this.gvCpos, true);
                lblCposSum.Text = string.Format("共{0}条", dtCpos.Rows.Count);
                tabPage_PARAM.SelectedTabPage = xtraTabPage2;
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        /// <summary>
        /// 修改数据时查看对应的车型参数数据是否存在，如果有一条不存在则全部不更新
        /// </summary>
        /// <param name="cposList"></param>
        /// <returns></returns>
        protected bool CheckIfMainDataReady(List<string> cposList)
        {
            bool flag = false;
            string message = string.Empty;

            if (cposList.Count > 0)
            {
                DataSet ds = new DataSet();
                foreach (string cpos in cposList)
                {
                    // 查询组成一条油耗数据的所有关联参数
                    string sql = string.Format(@"SELECT T1.VIN8,T1.CPOS,T1.IDENTITY_PARAM,T1.LTGG,M2.MAIN_ID FROM 
                                        (SELECT DISTINCT LEFT(V1.VIN,8) AS VIN8,V1.IDENTITY_PARAM, M1.CPOS, M1.LTGG FROM FC_CLJBXX V1, MAIN_CPOS M1
                                            WHERE V1.CPOS=M1.CPOS AND M1.CPOS ='{0}') T1 
                                        LEFT JOIN MAIN_CTNY M2 ON T1.VIN8=M2.VIN8 AND T1.IDENTITY_PARAM=M2.IDENTITY_PARAM AND T1.LTGG=M2.LTGG", cpos);
                    ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

                    // 该CPOS没有关联的油耗数据
                    if (ds.Tables[0].Rows.Count < 1)
                    {
                        message += string.Format("CPOS:“{0}”不存在油耗数据，无需修改\r\n", cpos);
                    }
                    else
                    {
                        string newMainID = Convert.ToString(ds.Tables[0].Rows[0]["MAIN_ID"]);
                        string vin8 = Convert.ToString(ds.Tables[0].Rows[0]["VIN8"]);
                        string ltgg = Convert.ToString(ds.Tables[0].Rows[0]["LTGG"]);
                        string identity = Convert.ToString(ds.Tables[0].Rows[0]["IDENTITY_PARAM"]);

                        // 修改后cpos后，对应的车型参数不存在，导致无法修改油耗数据
                        if (string.IsNullOrEmpty(newMainID))
                        {
                            message += string.Format("“{0}--{1}--{2}”对应的车型参数不存在，请先导入\r\n", vin8, ltgg, identity);
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(message))
                {
                    MessageForm mf = new MessageForm(message);
                    Utils.SetFormMid(mf);
                    mf.Text = "修改结果";
                    mf.ShowDialog();
                }
            }

            return flag;
        }
    }
}
