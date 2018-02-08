using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.IO;
using FuelDataSysClient.Tool.Tool_Jaguar;
using FuelDataSysClient.Tool;
using DevExpress.XtraGrid;

namespace FuelDataSysClient.Form_DBManager.Form_Jaguar
{
    public partial class ImportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 导入VIN(燃料)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportVin_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ImportVinFromExcel(Utils.FUEL);
        }

        /// <summary>
        /// 导入COC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportCOC_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ImportCOCData("IMPORT");
        }

        /// <summary>
        /// 修改COC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateCOC_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ImportCOCData("UPDATE");
        }

        /// <summary>
        /// 预览数据(燃料)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadyFuelData_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (!this.CheckAllDataReady(Utils.FUEL))
                {
                    return;
                }
                DataSet dsCtny = this.GetReadyDataFromLocalCoc(Utils.CTNY);
                DataSet dsFcds = this.GetReadyDataFromLocalCoc(Utils.FCDS);
                if (dsCtny != null || dsFcds != null)
                {
                    PreviewForm previewForm = new PreviewForm(dsCtny, dsFcds, Utils.FUEL);
                    previewForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("没有待生成的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 生成(燃料)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnGenerate_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!this.CheckAllDataReady(Utils.FUEL))
            {
                return;
            }
            try
            {
                //DONE-lyc L43-47
                DataSet dsCtny = this.GetReadyDataFromLocalCoc(Utils.CTNY);
                DataSet dsFcds = this.GetReadyDataFromLocalCoc(Utils.FCDS);
                JaguarUtils jaguarUtils = new JaguarUtils();
                string msg = jaguarUtils.SaveParam(dsCtny, Utils.CTNY);
                msg += jaguarUtils.SaveParam(dsFcds, Utils.FCDS);
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("生成成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageForm mf = new MessageForm("以下数据已经存在\r\n" + msg);
                    Utils.SetFormMid(mf);
                    mf.Text = "生成结果";
                    mf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("燃料消耗量数据生成失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 修改关联数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateVin_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 获取主表编号
            string cocIds = string.Empty;
            JaguarUtils jaguarUtils = new JaguarUtils();
            try
            {
                List<string> cocList = new List<string>();
                List<string> ctnyList = jaguarUtils.GetCocIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource);
                cocList.AddRange(ctnyList);

                List<string> fcdsList = jaguarUtils.GetCocIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);
                cocList.AddRange(fcdsList);

                if (cocList.Count < 1)
                {
                    MessageBox.Show("请选择COC信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                foreach (string cocId in cocList)
                {
                    cocIds += string.Format(",'{0}'", cocId);
                }

                if (!string.IsNullOrEmpty(cocIds))
                {
                    cocIds = cocIds.Substring(1);
                }

                ReviewUpdateVinForm reviewVinForm = new ReviewUpdateVinForm(cocIds);
                reviewVinForm.ShowDialog();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 导入VIN(国环)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportGhVin_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.ImportVinFromExcel(Utils.GH);
        }

        /// <summary>
        /// 预览数据(国环)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadyGHData_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (!this.CheckAllDataReady(Utils.GH))
                {
                    return;
                }
                //DONE-lyc
                DataSet dsCtny = this.GetReadyGHDataFromLocalCoc(Utils.CTNY);
                DataSet dsFcds = this.GetReadyGHDataFromLocalCoc(Utils.FCDS);
                if (dsCtny != null || dsFcds != null)
                {
                    PreviewForm previewForm = new PreviewForm(dsCtny, dsFcds, Utils.GH);
                    previewForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("没有待生成的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 生成(国环)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnGenGH_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!this.CheckAllDataReady(Utils.GH))
            {
                return;
            }
            try
            {
                DataSet dsCtny = this.GetReadyGHDataFromLocalCoc(Utils.CTNY);
                DataSet dsFcds = this.GetReadyGHDataFromLocalCoc(Utils.FCDS);

                JaguarUtils jaguarUtils = new JaguarUtils();
                string msg = jaguarUtils.SaveGHParam(dsCtny);
                msg += jaguarUtils.SaveGHParam(dsFcds);
                if (string.IsNullOrEmpty(msg))
                {
                    MessageBox.Show("生成成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageForm mf = new MessageForm("以下数据已经存在\r\n" + msg);
                    Utils.SetFormMid(mf);
                    mf.Text = "生成结果";
                    mf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("燃料消耗量数据生成失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 查看VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReviewVin_ItemClick(object sender, ItemClickEventArgs e)
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
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.SearchAllCocData();
        }

        // 删除
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            string vins = GetCheckData();
            if (string.IsNullOrEmpty(vins))
            {
                MessageBox.Show("请选择数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (this.xtraTabControl1.SelectedTabPage.Text == "传统能源")
            {
                string sql = string.Format("delete from COC_INFO where COC_ID in {0}", vins);
                AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            }
            else if (this.xtraTabControl1.SelectedTabPage.Text == "非插电式混合动力")
            {
                string sql = string.Format("delete from COC_FCDS where COC_ID  in {0}", vins);
                AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            }
            this.SearchAllCocData();
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchAllCocData();
        }

        //发动机生产厂
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            string importType = "UPDATE";
            JaguarUtils utils = new JaguarUtils();
            FolderDialog openFolder = new FolderDialog();
            List<string> cocIdList = new List<string>();

            string genFileName = string.Empty;
            string promptMsg = string.Empty;
            if (importType == "IMPORT")
            {
                genFileName = utils.CocFileName;
                promptMsg = "导入";
            }
            else if (importType == "UPDATE")
            {
                genFileName = utils.UpdateCocFileName;
                promptMsg = "修改";
            }
            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, genFileName);
                    if (fileNameList.Count > 0)
                    {
                        string returnMsg = string.Empty;

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            // 导入filename文件信息
                            returnMsg += utils.ReadCtnyCOCExcelFDJ(fileName, folderPath, importType, cocIdList);
                        }

                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = promptMsg + "结果";
                        mf.ShowDialog();

                        if (importType == "IMPORT")
                        {
                            this.SearchAllCocData();
                        }
                        else if (importType == "UPDATE")
                        {
                            this.SearchUpdatedCocData(cocIdList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("目录" + folderPath + "下没有文件" + genFileName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(promptMsg + "失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 指定某个Excel导入VIN
        private void ImportVinFromExcel(string dataType)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            JaguarUtils jaguarUtil = new JaguarUtils();

            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string returnFile = string.Empty;
                    string folderPath = Path.GetDirectoryName(openFileDialog1.FileName);

                    returnFile += jaguarUtil.ImportVinData(openFileDialog1.FileName, folderPath, dataType);

                    MessageForm mf = new MessageForm(returnFile);
                    Utils.SetFormMid(mf);
                    mf.Text = "VIN导入结果";
                    mf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //指定某个Excel导入COC
        protected void ImportCOCData(string importType)
        {
            JaguarUtils utils = new JaguarUtils();
            FolderDialog openFolder = new FolderDialog();
            List<string> cocIdList = new List<string>();

            string genFileName = string.Empty;
            string promptMsg = string.Empty;
            if (importType == "IMPORT")
            {
                genFileName = utils.CocFileName;
                promptMsg = "导入";
            }
            else if (importType == "UPDATE")
            {
                genFileName = utils.UpdateCocFileName;
                promptMsg = "修改";
            }
            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path.ToString();

                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(folderPath, genFileName);
                    if (fileNameList.Count > 0)
                    {
                        string returnMsg = string.Empty;

                        // 遍历读所有文件fileNameList
                        foreach (string fileName in fileNameList)
                        {
                            // 导入filename文件信息
                            returnMsg += utils.ReadCtnyCOCExcel(fileName, folderPath, importType, cocIdList, promptMsg);
                        }

                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = promptMsg + "结果";
                        mf.ShowDialog();

                        if (importType == "IMPORT")
                        {
                            this.SearchAllCocData();
                        }
                        else if (importType == "UPDATE")
                        {
                            this.SearchUpdatedCocData(cocIdList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("目录" + folderPath + "下没有文件" + genFileName, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(promptMsg + "失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //查询本地全部COC数据
        protected void SearchAllCocData()
        {
            try
            {
                string sqlCtny = string.Format(@"SELECT * FROM COC_INFO WHERE STATUS='1'");
                string sqlFcds = string.Format(@"SELECT * FROM COC_FCDS WHERE STATUS='1'");
                string sw = "";
                if (!string.IsNullOrEmpty(tbCoc.Text.Trim()))
                {
                    sw += " and (COC_ID like '%" + tbCoc.Text.Trim() + "%')";
                }
                if (!string.IsNullOrEmpty(tbClxh.Text.Trim()))
                {
                    sw += " and (CLXH like '%" + tbClxh.Text.Trim() + "%')";
                }
                if (!string.IsNullOrEmpty(tbClzl.Text.Trim()))
                {
                    sw += " and (CLZL like '%" + tbClzl.Text.Trim() + "%')";
                }

                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny + sw, null);
                DataSet dsFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds + sw, null);

                DataTable dtCtny = dsCtny.Tables[0];
                dtCtny.Columns.Add("check", System.Type.GetType("System.Boolean"));
                this.dgvCtny.DataSource = dtCtny;
                this.gvCtny.BestFitColumns();
                Utils.SelectItem(this.gvCtny, false);

                DataTable dtFcds = dsFcds.Tables[0];
                dtFcds.Columns.Add("check", System.Type.GetType("System.Boolean"));
                this.dgvFcds.DataSource = dtFcds;
                this.gvFcds.BestFitColumns();
                Utils.SelectItem(this.gvFcds, false);

                if (dsCtny.Tables[0].Rows.Count > 0)
                {
                    this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
                }
                else
                {
                    this.xtraTabControl1.SelectedTabPage = this.xtraTabPage2;
                }
                lblSum.Text = string.Format("共{0}条", dtCtny.Rows.Count + dtFcds.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //查询刚刚更新的COC数据
        protected void SearchUpdatedCocData(List<string> cocIdList)
        {
            string cocIds = string.Empty;
            try
            {
                if (cocIdList != null && cocIdList.Count > 0)
                {
                    foreach (string id in cocIdList)
                    {
                        cocIds += string.Format(",'{0}'", id);
                    }
                    if (!string.IsNullOrEmpty(cocIds))
                    {
                        cocIds = cocIds.Substring(1);
                    }
                }
                else
                {
                    cocIds = "''";
                }
                // 获取本地主表信息
                string sqlCtny = string.Format(@"SELECT * FROM COC_INFO WHERE STATUS='1' AND COC_ID IN ({0})", cocIds);
                string sqlFcds = string.Format(@"SELECT * FROM COC_FCDS WHERE STATUS='1' AND COC_ID IN ({0})", cocIds);
                string sw = "";
                if (!string.IsNullOrEmpty(tbCoc.Text.Trim()))
                {
                    sw += " and (COC_ID like '%" + tbCoc.Text.Trim() + "%')";
                }
                if (!string.IsNullOrEmpty(tbClxh.Text.Trim()))
                {
                    sw += " and (CLXH like '%" + tbClxh.Text.Trim() + "%')";
                }
                if (!string.IsNullOrEmpty(tbClzl.Text.Trim()))
                {
                    sw += " and (CLZL like '%" + tbClzl.Text.Trim() + "%')";
                }

                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny + sw, null);
                DataTable dtCtny = dsCtny.Tables[0];
                dtCtny.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dgvCtny.DataSource = dtCtny;
                Utils.SelectItem(this.gvCtny, true);

                DataSet dsFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds + sw, null);
                DataTable dtFcds = dsFcds.Tables[0];
                dtFcds.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dgvFcds.DataSource = dtFcds;
                Utils.SelectItem(this.gvFcds, true);

                lblSum.Text = string.Format("共{0}条", dtCtny.Rows.Count + dtFcds.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //检查所有源数据(VIN,COC)是否就绪
        private bool CheckAllDataReady(string dataType)
        {
            // 检查VIN源数据信息和COC源数据信息
            return (this.CheckIfVinDataReady(dataType) && this.CheckIfCocDataReady());
        }

        //检查VIN源数据表中的COC_ID对应的COC信息是否都已经存在
        protected bool CheckIfVinDataReady(string dataType)
        {
            string vinMsg = string.Empty;
            DataSet dsQuery = new DataSet();
            string sqlQuery = string.Format(@"SELECT VI.VIN FROM VIN_INFO VI WHERE VI.DATA_TYPE='{0}'", dataType);
            try
            {
                dsQuery = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQuery, null);
                if (dsQuery.Tables[0].Rows.Count < 1)
                {
                    MessageBox.Show("不存在VIN信息，请先导入\n", "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        //检查COC数据是否收集完整
        protected bool CheckIfCocDataReady()
        {
            string cocMsg = string.Empty;
            DataSet dsCtny = null;
            DataSet dsFcds = null;
            DataSet ds = null;
            string sqlCtny = @"SELECT CI.COC_ID FROM COC_INFO CI WHERE CI.COC_ID IN 
                                (SELECT DISTINCT(VI.COC_ID) FROM VIN_INFO VI)";
            string sqlFcds = @"SELECT CI.COC_ID FROM COC_FCDS CI WHERE CI.COC_ID IN 
                                (SELECT DISTINCT(VI.COC_ID) FROM VIN_INFO VI)";
            string sql = @"SELECT T1.COC_ID 
                                FROM (SELECT DISTINCT(COC_ID) FROM VIN_INFO) T1 
                                WHERE T1.COC_ID NOT IN 
                                (SELECT COC_ID FROM
                                (SELECT CI.COC_ID FROM COC_INFO CI UNION SELECT CF.COC_ID FROM COC_FCDS CF)
                                )";
            try
            {
                dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                dsFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
                if (dsCtny.Tables[0].Rows.Count + dsFcds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("VIN文件中的所有COC数据不存在，请先导入\n", "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        cocMsg += "缺少以下COC数据，只能生成部分数据\r\n";
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                cocMsg += dr["COC_ID"].ToString().Trim() + "\r\n";
                            }
                        }

                        MessageForm mf = new MessageForm(cocMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = "数据未准备就绪";
                        mf.ShowDialog();
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        //从源数据（VIN，COC）中查询待生成数据
        protected DataSet GetReadyDataFromLocalCoc(string fuelType)
        {
            // 获取本地车辆基本信息
            DataSet ds = null;
            string sql = string.Empty;
            try
            {
                //DONE-lyc L375-386
                if (fuelType == Utils.CTNY)
                {
                    sql = @" SELECT VI.VIN,VI.HGSPBM,VI.CLZZRQ,VI.GH_FDJXLH,VI.TGRQ,VI.DATA_TYPE, CI.* 
                                FROM VIN_INFO VI, COC_INFO CI
                                WHERE CI.STATUS='1' AND VI.DATA_TYPE='FUEL' AND CI.COC_ID=VI.COC_ID";
                }
                if (fuelType == Utils.FCDS)
                {
                    sql = @" SELECT VI.VIN,VI.HGSPBM,VI.CLZZRQ,VI.GH_FDJXLH,VI.TGRQ,VI.DATA_TYPE, CI.* 
                                FROM VIN_INFO VI, COC_FCDS CI 
                                WHERE CI.STATUS='1' AND VI.DATA_TYPE='FUEL' AND CI.COC_ID=VI.COC_ID";
                }
                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        //从源数据（VIN，COC）中查询待生成国环数据
        protected DataSet GetReadyGHDataFromLocalCoc(string fuelType)
        {
            // 获取本地车辆基本信息
            DataSet ds = null;
            string sql = string.Empty;
            try
            {
                if (fuelType == Utils.CTNY)
                {
                    sql = @"SELECT VI.VIN,VI.CLZZRQ,VI.GH_FDJXLH,VI.TGRQ,VI.DATA_TYPE, CI.COC_ID,CI.CLXH,CI.GH_FDJSCC 
                            FROM VIN_INFO VI,COC_INFO CI 
                            WHERE CI.STATUS='1' AND VI.DATA_TYPE='GH' AND CI.COC_ID=VI.COC_ID";
                }
                if (fuelType == Utils.FCDS)
                {
                    sql = @"SELECT VI.VIN,VI.CLZZRQ,VI.GH_FDJXLH,VI.TGRQ,VI.DATA_TYPE, CI.COC_ID,CI.CLXH,CI.GH_FDJSCC 
                            FROM VIN_INFO VI, COC_FCDS CI 
                            WHERE CI.STATUS='1' AND VI.DATA_TYPE='GH' AND CI.COC_ID=VI.COC_ID";
                }

                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        //选择选中数据
        private string GetCheckData()
        {

            var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            var selectedParamEntityIds = SelectedParamEntityIds(dv, "COC_ID");
            if (selectedParamEntityIds.Count > 0)
            {
                return List2Str(selectedParamEntityIds);
            }
            return "";
        }

        //选中选择项 数据源 返回选择列
        public static List<string> SelectedParamEntityIds(DataView dv, string column)
        {
            List<string> selectList = new List<string>();

            bool result = false;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    bool.TryParse(dv.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        selectList.Add(dv.Table.Rows[i][column].ToString());
                    }
                }
            }
            return selectList;
        }

        //List string 转换成 vins 逗号分隔
        public static string List2Str(List<string> list)
        {
            string vins = "(";
            foreach (string r in list)
            {
                vins += "'" + r + "'" + ",";
            }
            vins += "''" + ")";
            return vins;
        }
    }
}