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
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_DBManager.Form_Mits
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
            this.gvCljbxx.PostEditor();
            int[] selectedHandle;
            selectedHandle = this.gvCljbxx.GetSelectedRows();
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
                    // 获取folderPath下以格式为utils.CocFileName的所有文件
                    List<string> fileNameList = utils.GetFileName(openFolder.Path, utils.VinFileName);
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
                            // 导入filename文件信息
                            returnMsg += utils.ImportVinData(fileName, openFolder.Path);
                        }
                        MessageForm mf = new MessageForm(returnMsg);
                        Utils.SetFormMid(mf);
                        mf.Text = "导入结果";
                        mf.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show(String.Format("目录{0}下没有文件{1}", openFolder.Path, utils.VinFileName));
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

        //全选
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCljbxx.FocusedRowHandle = 0;
            this.gvCljbxx.FocusedColumn = this.gvCljbxx.Columns[1];
            GridControlHelper.SelectItem(this.gvCljbxx, true);
        }

        //取消全选
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvCljbxx.FocusedRowHandle = 0;
            this.gvCljbxx.FocusedColumn = this.gvCljbxx.Columns[1];
            GridControlHelper.SelectItem(this.gvCljbxx, false);
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
                // 获取本地主表信息
                string sql = string.Format(@"SELECT * FROM {0} where 1=1 ", tableName);
                string sw = "";

                if (!string.IsNullOrEmpty(clxh))
                {
                    sw += String.Format(" and (CLXH like '%{0}%')", tbCtClxh.Text);
                }
                if (!string.IsNullOrEmpty(clzl))
                {
                    sw += String.Format(" and (CLZL like '%{0}%')", tbCtClzl.Text);
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                if (rllx == "CTNY")
                {
                    this.gcCljbxx.DataSource = dt;
                    this.gvCljbxx.BestFitColumns();
                    Utils.SelectItem(this.gvCljbxx, false);
                    lblCtSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
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
                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                DataTable dtCtny = dsCtny.Tables[0];
                dtCtny.Columns.Add("check", System.Type.GetType("System.Boolean"));
                gcCljbxx.DataSource = dtCtny;
                Utils.SelectItem(this.gvCljbxx, true);
                lblCtSum.Text = string.Format("共{0}条", dtCtny.Rows.Count);
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
                List<string> ctnyList = mitsUtil.GetMainIdFromControl(this.gvCljbxx, (DataTable)this.gcCljbxx.DataSource);

                foreach (string ctnyId in ctnyList)
                {
                    mainIds += string.Format(",'{0}'", ctnyId);
                }
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

        //删除
        private void btnDeleteMain_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.ZGCS == null || ((DataTable)this.gvCljbxx.DataSource).Rows.Count < 1)
            {
                MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var dtSel = GridControlHelper.SelectedItems(this.gvCljbxx);
            if (dtSel.Rows.Count < 1)
            {
                MessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string clxhStr = string.Join("','", dtSel.AsEnumerable().Select(d => d.Field<string>("CLXH")).ToArray());
                AccessHelper.ExecuteNonQuery(AccessHelper.conn, string.Format("delete * from CTNY_MAIN where CLXH in '{0}')", clxhStr));
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
            this.SearchMainData("CTNY");
        }
    }
}