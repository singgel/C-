using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using Catarc.Adc.NewEnergyApproveSys.DevForm;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using Oracle.ManagedDataAccess.Client;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
{
    public partial class ContactInfoForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ContactInfoForm()
        {
            InitializeComponent();
        }

        //全部选中
        private void barBtnSelect_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvDataInfo.FocusedRowHandle = 0;
            this.gvDataInfo.FocusedColumn = gvDataInfo.Columns[1];
            GridControlHelper.SelectItem(this.gvDataInfo, true);
        }

        //全部取消
        private void barBtnCancle_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvDataInfo.FocusedRowHandle = 0;
            this.gvDataInfo.FocusedColumn = gvDataInfo.Columns[1];
            GridControlHelper.SelectItem(this.gvDataInfo, false);
        }

        //刷新
        private void barBtnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.refrashCurrentPage();
        }

        //导出Excel
        private void barBtnExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnExport.Caption));

                var dtExport = (DataTable)this.gcDataInfo.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    this.gvDataInfo.Columns["check"].Visible = false;
                    var options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    this.gcDataInfo.ExportToXls(saveFileDialog.FileName, options);
                    this.gvDataInfo.Columns["check"].Visible = true;
                    if (XtraMessageBox.Show("操作成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchLocal(1);
        }

        //清空查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.QYMC.Text = string.Empty;
        }

        //首页
        private void btnFirPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(1);
        }

        //上一页
        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(--pageNum);
        }

        //下一页
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(++pageNum);
        }

        //尾页
        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(pageCou);
        }

        // 刷新
        private void refrashCurrentPage()
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 0) SearchLocal(pageNum);
            else SearchLocal(1);
        }

        //是否显示全部
        private void ceQueryAll_CheckedChanged(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }

        // 查询
        private void SearchLocal(int pageNum)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //获取总数目
                int dataCount = queryCount();
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    var dtQuery = queryByPage(pageNum);
                    dtQuery.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtQuery.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dtQuery.Rows.Count; i++)
                    {
                        dtQuery.Rows[i]["check"] = false;
                    }
                    this.gcDataInfo.DataSource = dtQuery;
                    this.gvDataInfo.BestFitColumns();
                    int pageSize = Convert.ToInt32(this.spanNumber.Text);
                    int pageCount = dataCount / pageSize;
                    if (dataCount % pageSize > 0) pageCount++;
                    int dataLast;
                    if (pageNum == pageCount)
                        dataLast = dataCount;
                    else
                        dataLast = pageSize * pageNum;
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", (pageSize * (pageNum - 1) + 1), dataLast);
                    this.txtPage.Text = String.Format("{0}/{1}", pageNum, pageCount);
                }
                else
                {
                    var dtQuery = queryAll();
                    dtQuery.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtQuery.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dtQuery.Rows.Count; i++)
                    {
                        dtQuery.Rows[i]["check"] = false;
                    }
                    this.gcDataInfo.DataSource = dtQuery;
                    this.gvDataInfo.BestFitColumns();
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", 1, dataCount);
                    this.txtPage.Text = String.Format("{0}/{1}", 1, 1);
                }
                if (dataCount == 0)
                {
                    this.labPage.Text = "当前显示0至0条";
                    this.txtPage.Text = "0/0";
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //获取总数
        private int queryCount()
        {
            string sqlCount = String.Format("select count(*) from DB_CONTACTS where 1=1 {0}", this.queryParam());
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sqlCount));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            string sqlWhere = this.queryParam();
            string sqlStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM DB_CONTACTS where 1=1 {0} order by ID desc) A WHERE ROWNUM <= {1}) WHERE RN > {2}", sqlWhere, pageSize * pageNum, pageSize * (pageNum - 1));
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //获取全部数据
        private DataTable queryAll()
        {
            string sqlAll = String.Format("select * from DB_CONTACTS where 1=1 {0}", this.queryParam());
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlAll, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            if (!string.IsNullOrEmpty(QYMC.Text))
            {
                sqlStr.AppendFormat(" AND (ENTNAME like '%{0}%')", QYMC.Text);
            }
            return sqlStr.ToString();
        }

        //编辑列的行号
        private void gvDataInfo_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        //删除
        private void barBtnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, this.Text + "-" + this.barBtnDelete.Caption);

            if (this.gvDataInfo.DataSource == null)
            {
                XtraMessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvDataInfo.PostEditor();
            var dataSource = (DataView)this.gvDataInfo.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                    }
                }
            }
            if (dtSelected.Rows.Count == 0)
            {
                XtraMessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (XtraMessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    foreach (DataRow dr in dtSelected.Rows)
                    {
                        try
                        {
                            OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("DELETE FROM DB_CONTACTS WHERE　ID = '{0}'", dr["ID"]), null);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            XtraMessageBox.Show(String.Format("数据库操作出现异常，删除失败：{0}！", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
            this.refrashCurrentPage();
        }
    }
}