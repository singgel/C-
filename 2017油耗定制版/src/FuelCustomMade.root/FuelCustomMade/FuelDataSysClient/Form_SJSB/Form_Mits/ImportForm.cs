using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.IO;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_SJSB.Form_Mits
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
            FuelDataSysClient.Tool.Tool_Mits.MitsUtils utils = new FuelDataSysClient.Tool.Tool_Mits.MitsUtils();
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
        /// 导入VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportVin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FuelDataSysClient.Tool.Tool_Mits.MitsUtils utils = new FuelDataSysClient.Tool.Tool_Mits.MitsUtils();
            this.gvCtny.PostEditor();
            int[] selectedHandle;
            selectedHandle = this.gvCtny.GetSelectedRows();
            if (selectedHandle.Count() > 0)
            {
                if (selectedHandle[0] < 0)
                {
                    MessageBox.Show("请选择数据");
                    return;
                }
            }
            else
            {
                MessageBox.Show("请选择数据");
                return;
            }
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
                            MessageBox.Show("系统中不存在车型数据，请首先导入车型数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 导入COC
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

            FuelDataSysClient.Tool.Tool_Mits.MitsUtils utils = new FuelDataSysClient.Tool.Tool_Mits.MitsUtils();
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
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utils.SelectItem(this.gvCtny, true);
        }

        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Utils.SelectItem(this.gvCtny, false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowMainData();
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

                if (!"".Equals(clxh))
                {
                    sw += " and (CLXH like '%" + tbCtClxh.Text + "%')";
                }
                if (!"".Equals(clzl))
                {
                    sw += " and (CLZL like '%" + tbCtClzl.Text + "%')";
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
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
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        // 更新主表关联数据
        private void btnUpdateRelData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // 获取主表编号
            string mainIds = string.Empty;
            FuelDataSysClient.Tool.Tool_Mits.MitsUtils mitsUtil = new FuelDataSysClient.Tool.Tool_Mits.MitsUtils();
            try
            {
                List<string> ctnyList = mitsUtil.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource);
                //List<string> fcdsList = mitsUtil.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);


                //if (ctnyList.Count + fcdsList.Count < 1)
                //{
                //    MessageBox.Show("请选择主表信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return;
                //}

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
                    MessageBox.Show("请选择数据");
                    return;
                }
                ReviewUpdateVinForm reviewVinForm = new ReviewUpdateVinForm(mainIds);
                reviewVinForm.ShowDialog();
            }
            catch (Exception)
            {
            }
        }

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
            this.gvCtny.PostEditor();

            DataView dv = (DataView)this.gvCtny.DataSource;

            var result = from DataRow dr in dv.Table.Rows
                         where (bool)dr["check"] == true
                         select dr["CLXH"];

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
                MessageBox.Show("请选择要删除的数据！");
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            selectedParamEntityIds = selectedParamEntityIds.Substring(1);

            string sql = @"delete * from CTNY_MAIN where CLXH in (" + selectedParamEntityIds + ")";
            int i = AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);
            this.SearchMainData("CTNY");
        }
    }
}