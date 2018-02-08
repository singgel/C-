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
using System.Data.SqlClient;
using FuelDataSysClient.Tool.Tool_Nissan;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_SJSB.Form_Nissan
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
            if (!this.CheckAllDataReady())
            {
                return;
            }
            try
            {
                DataSet ds = this.GetReadyDataFromLocalCoc();
                NissanUtils nissanUtils = new NissanUtils();
                string msg = nissanUtils.SaveParam(ds);
                if (string.IsNullOrEmpty(msg))
                {
                    this.ShowLocalReadyData();
                    MessageBox.Show("生成成功", "生成成功");
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
                MessageBox.Show("燃料消耗量数据生成失败：" + ex.Message, "生成失败");
            }

        }

        /// <summary>
        /// 导入VIN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportVin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            NissanUtils utils = new NissanUtils();
            FolderDialog openFolder = new FolderDialog();
            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path;

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
                            MessageBox.Show("系统中不存在主表数据，请首先导入主表数据", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        MessageBox.Show(String.Format("目录{0}下没有文件{1}", folderPath, utils.VinFileName));
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

            NissanUtils utils = new NissanUtils();
            FolderDialog openFolder = new FolderDialog();

            try
            {
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = openFolder.Path;

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
                            fileNameMsg += Path.GetFileName(fileName) + Environment.NewLine;

                            // 导入filename文件信息
                            returnMsg += utils.ImportMainData(fileName, folderPath, importType, mainUpdateList);
                        }

                        if (string.IsNullOrEmpty(returnMsg))
                        {
                            MessageForm mf = new MessageForm(String.Format("以下文件{0}成功：\r\n{1}", operateType, fileNameMsg));
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
                        MessageBox.Show(String.Format("目录{0}下没有文件{1}", folderPath, utils.MainFileName));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}失败：{1}", operateType, ex.Message), "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 激活以导入的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEnable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // 获取对象
                List<string> vinList = Utils.GetUpdateVin(this.gvCtny, (DataTable)this.dgvCtny.DataSource);

                if (vinList == null || vinList.Count < 1)
                {
                    MessageBox.Show("请选择要激活的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string vinStr = string.Empty;
                if (vinList != null && vinList.Count > 0)
                {
                    foreach (string vin in vinList)
                    {
                        vinStr += vin + "','";
                    }
                }

                vinStr = vinStr.Substring(0, vinStr.Length - 3);
                string sqlUpdateStatus = string.Format(@"UPDATE FC_CLJBXX SET STATUS='1' WHERE VIN IN ('{0}')", vinStr);

                using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                {
                    AccessHelper.ExecuteNonQuery(con, sqlUpdateStatus, null);
                }

                this.ShowLocalReadyData();
                MessageBox.Show("激活成功，请转到待上报或补传待上报界面查看！", "激活成功", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("激活失败：" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.xtraTabControl.SelectedTabPage.Text.Equals("传统能源"))
            {
                Utils.SelectItem(this.gvCtny, true);
            }
            if (this.xtraTabControl.SelectedTabPage.Text.Equals("非插电式混合动力"))
            {
                Utils.SelectItem(this.gvFcds, true);
            }
            if (this.xtraTabControl.SelectedTabPage.Text.Equals("纯电动"))
            {
                Utils.SelectItem(this.gvCdd, true);
            }
        }

        /// <summary>
        /// 取消全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.xtraTabControl.SelectedTabPage.Text.Equals("传统能源"))
            {
                Utils.SelectItem(this.gvCtny, false);
            }
            if (this.xtraTabControl.SelectedTabPage.Text.Equals("非插电式混合动力"))
            {
                Utils.SelectItem(this.gvFcds, false);
            }
            if (this.xtraTabControl.SelectedTabPage.Text.Equals("纯电动"))
            {
                Utils.SelectItem(this.gvCdd, false);
            }
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
        /// 查询待生成数据并显示在界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadyFuelData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!this.CheckAllDataReady())
                {
                    return;
                }
                DataSet ds = this.GetReadyDataFromLocalCoc();
                if (ds != null)
                {
                    PreviewForm previewForm = new PreviewForm(ds);
                    if (previewForm.ShowDialog() == DialogResult.OK)
                    {
                        this.ShowLocalReadyData();
                    }
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
        /// 查看已导入的COC信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReviewMainData_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ReviewCocData cocForm = new ReviewCocData();
                cocForm.ShowDialog();
            }
            catch (Exception ex)
            {
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
        /// 双击记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLocalFuelData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)dgvCtny.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();

            if (dr == null)
            {
                return;
            }
            string vin = (string)dr.Row.ItemArray[0];

            // 获取此VIN的详细信息，带入窗口
            string sql = @"select * from FC_CLJBXX where vin = @vin";
            OleDbParameter[] param = {
                                     new OleDbParameter("@vin",vin)
                                     };
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, param);
            DataTable dt = ds.Tables[0];

            // 弹出详细信息窗口，可修改
            JbxxViewForm jvf = new JbxxViewForm();
            jvf.Enabled = true;
            jvf.status = "9";
            jvf.setVisible("btnbaocunshangbao", false);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    DataColumn dc = dt.Columns[i];
                    Control[] c = jvf.Controls.Find("tb" + dc.ColumnName, true);
                    if (c.Length > 0)
                    {
                        if (c[0] is TextEdit)
                        {
                            c[0].Text = dt.Rows[0].ItemArray[i].ToString();
                            continue;
                        }
                        if (c[0] is DevExpress.XtraEditors.ComboBoxEdit)
                        {
                            DevExpress.XtraEditors.ComboBoxEdit cb = c[0] as DevExpress.XtraEditors.ComboBoxEdit;
                            cb.Text = dt.Rows[0].ItemArray[i].ToString();
                            if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                                || cb.Text == "双燃料" || cb.Text == "纯电动" || cb.Text == "非插电式混合动力" || cb.Text == "插电式混合动力" || cb.Text == "燃料电池")
                            {
                                string rlval = cb.Text;
                                if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                                || cb.Text == "双燃料")
                                {
                                    rlval = "传统能源";
                                }

                                // 构建燃料参数控件
                                jvf.getParamList(rlval, true);
                            }
                        }
                    }
                }
            }

            // 获取燃料信息
            string rlsql = @"select e.* from RLLX_PARAM_ENTITY e where e.vin = @vin";
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, rlsql, param);
            dt = ds.Tables[0];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow drrlxx = dt.Rows[i];
                string cName = drrlxx.ItemArray[1].ToString();
                Control[] c = jvf.Controls.Find(cName, true);
                if (c.Length > 0)
                {
                    if (c[0] is TextEdit)
                    {
                        c[0].Text = drrlxx.ItemArray[3].ToString();
                        continue;
                    }
                    if (c[0] is DevExpress.XtraEditors.ComboBoxEdit)
                    {
                        DevExpress.XtraEditors.ComboBoxEdit cb = c[0] as DevExpress.XtraEditors.ComboBoxEdit;
                        cb.Text = drrlxx.ItemArray[3].ToString();
                    }
                }
            }

            (jvf.Controls.Find("tc", true)[0] as XtraTabControl).SelectedTabPageIndex = 0;
            jvf.MaximizeBox = false;
            jvf.MinimizeBox = false;
            Utils.SetFormMid(jvf);
            jvf.formClosingEventHandel += new FormClosingEventHandler(refrashBySubForm);
            jvf.ShowDialog();
        }

        void refrashBySubForm(object sender, FormClosingEventArgs args)
        {
            ShowLocalReadyData();
        }

        /// <summary>
        /// 从源数据（VIN，COC）中查询待生成数据
        /// </summary>
        /// <returns></returns>
        protected DataSet GetReadyDataFromLocalCoc()
        {
            // 获取本地车辆基本信息
            DataSet ds = null;
            try
            {
                string sql = @"SELECT VI.VIN,VI.CLZZRQ,CI.* FROM VIN_INFO VI "
                                + " LEFT JOIN CTNY_MAIN CI ON CI.MAIN_ID=VI.MAIN_ID "
                                + " WHERE CI.STATUS='1'";

                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 显示所有主表数据
        /// </summary>
        protected void ShowMainData()
        {
            this.SearchMainData("CTNY");
            this.SearchMainData("FCDS");
            this.SearchMainData("CDD");

        }

        protected void ShowUpdatedMainData(List<string> mainUpdateList)
        {
            this.SearchUpdatedMainData(mainUpdateList);
        }

        /// <summary>
        /// 检查VIN源数据表中的MAIN_ID对应的COC信息是否都已经存在
        /// </summary>
        /// <returns></returns>
        protected bool CheckIfVinDataReady()
        {
            string vinMsg = string.Empty;
            DataSet dsQuery = new DataSet();
            const string sqlQuery = @"SELECT VI.VIN FROM VIN_INFO VI";
            try
            {
                dsQuery = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQuery, null);
                if (dsQuery.Tables[0].Rows.Count < 1)
                {
                    MessageBox.Show("不存在VIN信息，请先导入\r\n", "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        /// <summary>
        /// 检查COC数据是否收集完整
        /// </summary>
        protected bool CheckIfCocDataReady()
        {
            string cocMsg = string.Empty;
            DataSet dsQuery = null;
            const string sqlCocQuery = @"SELECT CI.MAIN_ID FROM CTNY_MAIN CI WHERE CI.MAIN_ID IN 
                                (SELECT DISTINCT(VI.MAIN_ID) FROM VIN_INFO VI)";
            const string sqlQuery = @"SELECT T1.MAIN_ID FROM (SELECT DISTINCT(MAIN_ID) FROM VIN_INFO) T1 
                                WHERE T1.MAIN_ID NOT IN ((SELECT CI.MAIN_ID FROM CTNY_MAIN CI))";
            try
            {
                dsQuery = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCocQuery, null);
                if (dsQuery.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("VIN文件中的所有COC数据不存在，请先导入\r\n", "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    dsQuery = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQuery, null);

                    if (dsQuery.Tables[0].Rows.Count > 0)
                    {
                        cocMsg += "缺少以下COC数据，只能生成部分数据\r\n";
                        foreach (DataRow dr in dsQuery.Tables[0].Rows)
                        {
                            cocMsg += dr["MAIN_ID"].ToString().Trim() + "\r\n";
                        }
                        MessageBox.Show(cocMsg, "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        /// <summary>
        /// 待生成数据绑定gridView1
        /// </summary>
        protected void ShowLocalReadyData()
        {
            string sql = @"SELECT * FROM FC_CLJBXX WHERE STATUS='9' ORDER BY CREATETIME DESC";
            try
            {
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                this.dgvCtny.DataSource = dt;
                Utils.SelectItem(this.gvCtny, false);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 检查所有源数据(VIN,COC)是否就绪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool CheckAllDataReady()
        {
            // 检查VIN源数据信息和COC源数据信息
            return (this.CheckIfVinDataReady() && this.CheckIfCocDataReady());
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
        /// 查询已经导入的纯电动主表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCddSearch_Click(object sender, EventArgs e)
        {
            this.SearchMainData("CDD");
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
                    paramNO = tbCtParam.Text;
                    clxh = tbCtClxh.Text;
                    clzl = tbCtClzl.Text;
                }
                else if (rllx == "FCDS")
                {
                    tableName = "CDS_MAIN";
                    paramNO = tbFcdsParam.Text;
                    clxh = tbFcdsClxh.Text;
                    clzl = tbFcdsClzl.Text;
                }
                else if (rllx == "CDD")
                {
                    tableName = "CDD_MAIN";
                    paramNO = tbCddParam.Text;
                    clxh = tbCddClxh.Text;
                    clzl = tbCddClzl.Text;
                }

                // 获取本地主表信息
                string sql = string.Format(@"SELECT * FROM {0} WHERE STATUS='1'", tableName);
                string sw = "";
                if (!"".Equals(paramNO))
                {
                    sw += String.Format(" and (MAIN_ID like '%{0}%')", paramNO);
                }
                if (!"".Equals(clxh))
                {
                    sw += String.Format(" and (CLXH like '%{0}%')", clxh);
                }
                if (!"".Equals(clzl))
                {
                    sw += String.Format(" and (CLZL like '%{0}%')", clzl);
                }

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));

                if (rllx == "CTNY")
                {
                    dgvCtny.DataSource = dt;
                    this.gvCtny.BestFitColumns();
                    Utils.SelectItem(this.gvCtny, false);
                    lblCtSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                else if (rllx == "FCDS")
                {
                    dgvFcds.DataSource = dt;
                    this.gvFcds.BestFitColumns();
                    Utils.SelectItem(this.gvFcds, false);
                    lblFcdsSum.Text = string.Format("共{0}条", dt.Rows.Count);
                }
                else if (rllx == "CDD")
                {
                    dgvCdd.DataSource = dt;
                    this.gvCdd.BestFitColumns();
                    Utils.SelectItem(this.gvCdd, false);
                    lblCddSum.Text = string.Format("共{0}条", dt.Rows.Count);
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
                string sqlCtny = string.Format(@"SELECT * FROM CTNY_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
                string sqlFcds = string.Format(@"SELECT * FROM CDS_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);
                string sqlCdd = string.Format(@"SELECT * FROM CDD_MAIN WHERE STATUS='1' AND MAIN_ID IN ({0})", mainIds);


                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                DataSet dsFcds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
                DataSet dsCdd = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCdd, null);

                DataTable dtCtny = dsCtny.Tables[0];
                dtCtny.Columns.Add("check", System.Type.GetType("System.Boolean"));
                DataTable dtFcds = dsFcds.Tables[0];
                dtFcds.Columns.Add("check", System.Type.GetType("System.Boolean"));
                DataTable dtCdd = dsCdd.Tables[0];
                dtCdd.Columns.Add("check", System.Type.GetType("System.Boolean"));

                dgvCtny.DataSource = dtCtny;
                Utils.SelectItem(this.gvCtny, true);
                lblCtSum.Text = string.Format("共{0}条", dtCtny.Rows.Count);

                dgvFcds.DataSource = dtFcds;
                Utils.SelectItem(this.gvFcds, true);
                lblFcdsSum.Text = string.Format("共{0}条", dtFcds.Rows.Count);

                dgvCdd.DataSource = dtCdd;
                Utils.SelectItem(this.gvCdd, true);
                lblCddSum.Text = string.Format("共{0}条", dtCdd.Rows.Count);
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
            NissanUtils nissanUtil = new NissanUtils();
            try
            {
                List<string> ctnyList = nissanUtil.GetMainIdFromControl(this.gvCtny, (DataTable)this.dgvCtny.DataSource);
                List<string> fcdsList = nissanUtil.GetMainIdFromControl(this.gvFcds, (DataTable)this.dgvFcds.DataSource);
                List<string> cddList = nissanUtil.GetMainIdFromControl(this.gvCdd, (DataTable)this.dgvCdd.DataSource);


                if (ctnyList.Count + fcdsList.Count + cddList.Count < 1)
                {
                    MessageBox.Show("请选择主表信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                foreach (string cddId in cddList)
                {
                    mainIds += string.Format(",'{0}'", cddId);
                }

                if (!string.IsNullOrEmpty(mainIds))
                {
                    mainIds = mainIds.Substring(1);
                }

                using (ReviewUpdateVinForm reviewVinForm = new ReviewUpdateVinForm(mainIds))
                {
                    reviewVinForm.ShowDialog();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
