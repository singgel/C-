using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Grid;
using FuelDataModel;
using FuelDataSysClient.SubForm;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using System.Threading;
using System.Text.RegularExpressions;
using FuelDataSysClient.Tool;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using Common;


namespace FuelDataSysClient.Form_SCSJ
{
    public partial class SocketDataForm_New : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        AutoActionHelper auto;

        public SocketDataForm_New()
        {
            InitializeComponent();
            this.xtraTabControl1.SelectedTabPageIndex = 0;
            auto = new AutoActionHelper(userLoginLog, txtDetails);
            //初始时间和日期
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        private void SocketDataForm_New_Load(object sender, EventArgs e)
        {
            try
            {
                this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 服务器启动 {0} *** ", DateTime.Now.ToString("G"))); }));
                LogManager.Log("Log", "Log", "*** 服务器启动 *** ");
                auto.Start();
                if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE!='IsAutoUpload'"))
                {
                    this.Invoke(new Action(() => { this.barBtnStart.Enabled = false; this.barBtnStop.Enabled = true; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始接收合并 {0} *** ", DateTime.Now.ToString("G"))); }));
                    LogManager.Log("Log", "Log", "*** 开始接收合并，伴随启动 *** ");
                }
                else
                {
                    this.Invoke(new Action(() => { this.barBtnStart.Enabled = true; this.barBtnStop.Enabled = false; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止接收合并 {0} *** ", DateTime.Now.ToString("G"))); }));
                    LogManager.Log("Log", "Log", "*** 停止接收合并，伴随启动 *** ");
                }
                if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE='IsAutoUpload'"))
                {
                    this.Invoke(new Action(() => { this.barBtnStartUpload.Enabled = false; this.barBtnStopUpload.Enabled = true; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始上报 {0} *** ", DateTime.Now.ToString("G"))); }));
                    LogManager.Log("Log", "Log", "*** 开始上报，伴随启动 *** ");
                }
                else
                {
                    this.Invoke(new Action(() => { this.barBtnStartUpload.Enabled = true; this.barBtnStopUpload.Enabled = false; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止上报 {0} *** ", DateTime.Now.ToString("G"))); }));
                    LogManager.Log("Log", "Log", "*** 停止上报，伴随启动 *** ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("远程控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //接收合并开始
        private void barBtnStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=1,OPERATOR='Client',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE!='IsAutoUpload'", DateTime.Today.ToString());
                if (FuelDataSysClient.Tool.OracleHelper.ExecuteNonQuery(FuelDataSysClient.Tool.OracleHelper.conn, sql, null) > 0)
                {
                    MessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.barBtnStart.Enabled = false;
                    this.barBtnStop.Enabled = true;
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始接收合并 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 开始接收合并 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("远程控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //接收合并停止
        private void barBtnStop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=0,OPERATOR='Client',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE!='IsAutoUpload'", DateTime.Today.ToString());
                if (FuelDataSysClient.Tool.OracleHelper.ExecuteNonQuery(FuelDataSysClient.Tool.OracleHelper.conn, sql, null) > 0)
                {
                    MessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.barBtnStart.Enabled = true;
                    this.barBtnStop.Enabled = false;
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止接收合并 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 停止接收合并 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("远程控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //上报开始
        private void barBtnStartUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=1,OPERATOR='Client',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE='IsAutoUpload'", DateTime.Today.ToString());
                if (FuelDataSysClient.Tool.OracleHelper.ExecuteNonQuery(FuelDataSysClient.Tool.OracleHelper.conn, sql, null) > 0)
                {
                    MessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.barBtnStartUpload.Enabled = false;
                    this.barBtnStopUpload.Enabled = true;
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始上报 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 开始上报 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("远程控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //上报结束
        private void barBtnStopUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=0,OPERATOR='Client',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE='IsAutoUpload'", DateTime.Today.ToString());
                if (FuelDataSysClient.Tool.OracleHelper.ExecuteNonQuery(FuelDataSysClient.Tool.OracleHelper.conn, sql, null) > 0)
                {
                    MessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    this.barBtnStartUpload.Enabled = true;
                    this.barBtnStopUpload.Enabled = false;
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止上报 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 停止上报 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("远程控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //合并数据
        private void barBtnMerger_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 1;
            string selectedParamEntityIds = "";
            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            try
            {
                this.gvVIN_INFO.PostEditor();

                DataView dv = (DataView)this.gvVIN_INFO.DataSource;

                List<string> lsData = new List<string>();
                if (dv != null)
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        bool result = false;
                        bool.TryParse(dv.Table.Rows[i]["check"].ToString(), out result);
                        if (result)
                        {
                            selectedParamEntityIds += String.Format(",'{0}'", dv.Table.Rows[i]["ID"]);
                            mapResult.Add(dv.Table.Rows[i]["ID"].ToString(), "VIN:"+dv.Table.Rows[i]["VIN"] + " 该数据已合并");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                selectedParamEntityIds = "";
            }
            if (selectedParamEntityIds == "")
            {
                MessageBox.Show("请选择要合并的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show(" 如果操作的记录中包含已合并数据,合成中该部分数据会被忽略！确定要合并吗？", "合并确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            selectedParamEntityIds = selectedParamEntityIds.Substring(1);
            String msg = "";

            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                MergerData merger = new MergerData();
                msg = merger.Merger(selectedParamEntityIds, mapResult);
            }
            catch (Exception ex)
            {
                msg = "合并出现错误：" + ex.Message;
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
            try
            {
                MessageForm mf = new MessageForm(msg);
                Utils.SetFormMid(mf);
                mf.Text = "合并结果";
                mf.ShowDialog();
            }
            catch (System.Exception ex)
            {

            }
            this.refrashCurrentPage();
            
                
            
        }

        //删除数据
        private void barBtnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 1;
            this.gvVIN_INFO.PostEditor();
            var dataSource = (DataView)this.gvVIN_INFO.DataSource;
            if (dataSource == null)
            {
                MessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
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
                MessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dtSelected.Select("MERGER_STATUS=1").Count() > 0)
            {
                MessageBox.Show("您选择要操作的记录中包含已合并数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            using (OracleConnection conn = new OracleConnection(FuelDataSysClient.Tool.OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    foreach (DataRow dr in dtSelected.Rows)
                    {
                        try
                        {
                            FuelDataSysClient.Tool.OracleHelper.ExecuteNonQuery(FuelDataSysClient.Tool.OracleHelper.conn, string.Format("Delete from VIN_INFO where ID='{0}'", dr["ID"]), null);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            MessageBox.Show(String.Format("数据库操作出现异常，删除失败：{0}！", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
            this.refrashCurrentPage();
        }

        //全选
        private void barSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 1;
            this.gvVIN_INFO.FocusedRowHandle = 0;
            this.gvVIN_INFO.FocusedColumn = gvVIN_INFO.Columns["SC_OCN"];
            Utils.SelectItem(this.gvVIN_INFO, true);
        }

        //取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 1;
            this.gvVIN_INFO.FocusedRowHandle = 0;
            this.gvVIN_INFO.FocusedColumn = gvVIN_INFO.Columns["SC_OCN"];
            Utils.SelectItem(this.gvVIN_INFO, false);
        }

        //导出数据
        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.xtraTabControl1.SelectedTabPageIndex = 1;
                DataTable dtExport = (DataTable)gcVIN_INFO.DataSource;
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
                var dialogResult = saveFileDialog1.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    gcVIN_INFO.ExportToXls(saveFileDialog1.FileName, options);
                    ExcelHelper excelBuilder = new ExcelHelper(saveFileDialog1.FileName);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.SaveFile();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 刷新
        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 1;
            refrashCurrentPage();
        }

        // 查询按钮
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchLocal(1);
        }

        // 清空查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVIN.Text = string.Empty;
            this.tbIPV4.Text = string.Empty;
            this.tbSC_OCN.Text = string.Empty;
            this.cbeMERGER.Text = string.Empty;
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

        //刷新界面
        private void refrashCurrentPage()
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            this.SearchLocal(pageNum);
        }

        //是否显示全部
        private void ceQueryAll_CheckedChanged(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }

        // 查询
        private void SearchLocal(int pageNum)
        {
            // 验证查询时间：结束时间不能小于开始时间
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && Convert.ToDateTime(this.dtStartTime.Text) > Convert.ToDateTime(this.dtEndTime.Text))
            {
                MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //获取总数目
                int dataCount = queryCount();
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    DataTable dt = queryByPage(pageNum);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    this.gcVIN_INFO.DataSource = dt;
                    this.gvVIN_INFO.BestFitColumns();
                    int pageSize = Convert.ToInt32(this.spanNumber.Text);
                    int pageCount = dataCount / pageSize;
                    if (dataCount % pageSize > 0) pageCount++;
                    int dataLast = pageSize * pageNum;
                    if (pageNum == pageCount) dataLast = dataCount;
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", (pageSize * (pageNum - 1) + 1), dataLast);
                    this.txtPage.Text = String.Format("{0}/{1}", pageNum, pageCount);
                }
                else
                {
                    DataTable dt = queryAll();
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    this.gcVIN_INFO.DataSource = dt;
                    this.gvVIN_INFO.BestFitColumns();
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
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //获取总数
        private int queryCount()
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("select count(*) from VIN_INFO where 1=1 ");
            if (!string.IsNullOrEmpty(this.tbVIN.Text))
            {
                sqlStr.AppendFormat(@" AND VIN LIKE '%{0}%' ", this.tbVIN.Text);
            }
            if (!string.IsNullOrEmpty(this.tbSC_OCN.Text))
            {
                sqlStr.AppendFormat(@" AND SC_OCN LIKE '%{0}%' ", this.tbSC_OCN.Text);
            }
            if (!string.IsNullOrEmpty(this.tbIPV4.Text))
            {
                sqlStr.AppendFormat(@" AND IPV4 LIKE '%{0}%' ", this.tbIPV4.Text);
            }
            if (!string.IsNullOrEmpty(this.cbeMERGER.Text))
            {
                if (this.cbeMERGER.Text.Equals("已合并"))
                {
                    sqlStr.Append(" AND MERGER_STATUS = 1 ");
                }
                if (this.cbeMERGER.Text.Equals("未合并"))
                {
                    sqlStr.Append(" AND MERGER_STATUS = 0 ");
                }
                if (this.cbeMERGER.Text.Equals("已合并的重复数据"))
                {
                    sqlStr.Append(" AND MERGER_STATUS = 2 ");
                }
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlStr.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            return Convert.ToInt32(FuelDataSysClient.Tool.OracleHelper.GetSingle(FuelDataSysClient.Tool.OracleHelper.conn, sqlStr.ToString()));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(this.tbVIN.Text))
            {
                sqlWhere.AppendFormat(@" AND VIN LIKE '%{0}%' ", this.tbVIN.Text);
            }
            if (!string.IsNullOrEmpty(this.tbSC_OCN.Text))
            {
                sqlWhere.AppendFormat(@" AND SC_OCN LIKE '%{0}%' ", this.tbSC_OCN.Text);
            }
            if (!string.IsNullOrEmpty(this.tbIPV4.Text))
            {
                sqlWhere.AppendFormat(@" AND IPV4 LIKE '%{0}%' ", this.tbIPV4.Text);
            }
            if (!string.IsNullOrEmpty(this.cbeMERGER.Text))
            {
                if (this.cbeMERGER.Text.Equals("已合并"))
                {
                    sqlWhere.Append(" AND MERGER_STATUS = 1 ");
                }
                if (this.cbeMERGER.Text.Equals("未合并"))
                {
                    sqlWhere.Append(" AND MERGER_STATUS = 0 ");
                }
                if (this.cbeMERGER.Text.Equals("已合并的重复数据"))
                {
                    sqlWhere.Append(" AND MERGER_STATUS = 2 ");
                }
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlWhere.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            string sqlVins = string.Format(@"select * from VIN_INFO where 1=1  {0}", sqlWhere);
            string sqlStr = string.Format(@"select * from (select F.*,ROWNUM RN from ({0}) F where ROWNUM<={1}) where RN>{2}", sqlVins, pageSize * pageNum, pageSize * (pageNum - 1));
            DataSet ds = FuelDataSysClient.Tool.OracleHelper.ExecuteDataSet(FuelDataSysClient.Tool.OracleHelper.conn, sqlStr, null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        //获取全部数据
        private DataTable queryAll()
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append("select * from VIN_INFO where 1=1 ");
            if (!string.IsNullOrEmpty(this.tbVIN.Text))
            {
                sqlStr.AppendFormat(@" AND VIN LIKE '%{0}%' ", this.tbVIN.Text);
            }
            if (!string.IsNullOrEmpty(this.tbSC_OCN.Text))
            {
                sqlStr.AppendFormat(@" AND SC_OCN LIKE '%{0}%' ", this.tbSC_OCN.Text);
            }
            if (!string.IsNullOrEmpty(this.tbIPV4.Text))
            {
                sqlStr.AppendFormat(@" AND IPV4 LIKE '%{0}%' ", this.tbIPV4.Text);
            }
            if (!string.IsNullOrEmpty(this.cbeMERGER.Text))
            {
                if (this.cbeMERGER.Text.Equals("已合并"))
                {
                    sqlStr.Append(" AND MERGER_STATUS = 1 ");
                }
                if (this.cbeMERGER.Text.Equals("未合并"))
                {
                    sqlStr.Append(" AND MERGER_STATUS = 0 ");
                }
                if (this.cbeMERGER.Text.Equals("已合并的重复数据"))
                {
                    sqlStr.Append(" AND MERGER_STATUS = 2 ");
                }
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlStr.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            DataSet ds = FuelDataSysClient.Tool.OracleHelper.ExecuteDataSet(FuelDataSysClient.Tool.OracleHelper.conn, sqlStr.ToString(), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        // 状态列显示文本
        private void gvVIN_INFO_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "MERGER_STATUS")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "0":
                        e.DisplayText = "未合并";
                        break;
                    case "1":
                        e.DisplayText = "已合并";
                        break;
                    case "2":
                        e.DisplayText = "已合并的重复数据";
                        break;
                    case "3":
                        e.DisplayText = "不合法的数据";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
        }

        // 状态行显示文本
        private void gvVIN_INFO_RowStyle(object sender, RowStyleEventArgs e)
        {
            int hand = e.RowHandle;
            if (hand < 0) return;
            DataRow dr = this.gvVIN_INFO.GetDataRow(hand);
            if (dr == null) return;
            if (dr["MERGER_STATUS"].ToString().Equals("0"))
            {
                e.Appearance.BackColor = Color.LightSkyBlue;
            }
            if (dr["MERGER_STATUS"].ToString().Equals("2"))
            {
                e.Appearance.BackColor = Color.IndianRed;
            }
            if (dr["MERGER_STATUS"].ToString().Equals("3"))
            {
                e.Appearance.BackColor = Color.LightPink;
            }
        }

        //定时刷新
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE!='IsAutoUpload'"))
            {
                this.Invoke(new Action(() => { this.barBtnStart.Enabled = false; this.barBtnStop.Enabled = true; }));
            }
            else
            {
                this.Invoke(new Action(() => { this.barBtnStart.Enabled = true; this.barBtnStop.Enabled = false; }));
            }
            if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE='IsAutoUpload'"))
            {
                this.Invoke(new Action(() => { this.barBtnStartUpload.Enabled = false; this.barBtnStopUpload.Enabled = true; }));
            }
            else
            {
                this.Invoke(new Action(() => { this.barBtnStartUpload.Enabled = true; this.barBtnStopUpload.Enabled = false; }));
            }
        }

        //统计
        private void barBtnStatistics_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 0;
            this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 统计成功 {0} *** ", DateTime.Now.ToString("G"))); }));
            StatisticsData statis = new StatisticsData();
            statis.Statistics(this.txtDetails);
        }

        //清空
        private void barBtnClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 0;
            userLoginLog.Items.Clear();
            txtDetails.Text = string.Empty;
        }

        //保存日志
        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 0;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog() { FileName = "日志_" + DateTime.Now.ToString("yyyy-MM-dd"), Title = "保存日志", Filter = "TXT文件(*.txt)|*.txt" };
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName);
                for (int i = 0; i < this.userLoginLog.Items.Count; i++)
                {
                    sw.WriteLine(userLoginLog.Items[i].ToString());
                }
                sw.Close();
            }
        }

    }
}
