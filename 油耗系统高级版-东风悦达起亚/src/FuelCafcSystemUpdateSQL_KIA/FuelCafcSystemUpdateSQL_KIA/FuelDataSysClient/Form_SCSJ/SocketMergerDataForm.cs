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

namespace FuelDataSysClient.Form_SCSJ
{
    public partial class SocketMergerDataForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {

        public SocketMergerDataForm()
        {
            InitializeComponent();
            //初始时间和日期
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //全选
        private void barSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvVIN_INFO.FocusedRowHandle = 0;
            this.gvVIN_INFO.FocusedColumn = gvVIN_INFO.Columns["SC_OCN"];
            Utils.SelectItem(this.gvVIN_INFO, true);
        }

        //取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvVIN_INFO.FocusedRowHandle = 0;
            this.gvVIN_INFO.FocusedColumn = gvVIN_INFO.Columns["SC_OCN"];
            Utils.SelectItem(this.gvVIN_INFO, false);
        }

        //导出数据
        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
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
            this.tbSC_OCN.Text = string.Empty;
            this.tbCLJBXX_VERSION.Text = string.Empty;
            this.tbRLLX_VERSION.Text = string.Empty;
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
            sqlStr.Append("select count(*) from VIN_MERGER_INFO where 1=1 ");
            if (!string.IsNullOrEmpty(this.tbVIN.Text))
            {
                sqlStr.Append(string.Format(@" AND VIN LIKE '%{0}%' ", this.tbVIN.Text));
            }
            if (!string.IsNullOrEmpty(this.tbSC_OCN.Text))
            {
                sqlStr.Append(string.Format(@" AND SC_OCN LIKE '%{0}%' ", this.tbSC_OCN.Text));
            }
            if (!string.IsNullOrEmpty(this.tbCLJBXX_VERSION.Text))
            {
                sqlStr.Append(string.Format(@" AND CLJBXX_VERSION LIKE '%{0}%' ", this.tbCLJBXX_VERSION.Text));
            }
            if (!string.IsNullOrEmpty(this.tbRLLX_VERSION.Text))
            {
                sqlStr.Append(string.Format(@" AND RLLX_VERSION LIKE '%{0}%' ", this.tbRLLX_VERSION.Text));
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlStr.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sqlStr.ToString()));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(this.tbVIN.Text))
            {
                sqlWhere.Append(string.Format(@" AND VIN LIKE '%{0}%' ", this.tbVIN.Text));
            }
            if (!string.IsNullOrEmpty(this.tbSC_OCN.Text))
            {
                sqlWhere.Append(string.Format(@" AND SC_OCN LIKE '%{0}%' ", this.tbSC_OCN.Text));
            }
            if (!string.IsNullOrEmpty(this.tbCLJBXX_VERSION.Text))
            {
                sqlWhere.Append(string.Format(@" AND CLJBXX_VERSION LIKE '%{0}%' ", this.tbCLJBXX_VERSION.Text));
            }
            if (!string.IsNullOrEmpty(this.tbRLLX_VERSION.Text))
            {
                sqlWhere.Append(string.Format(@" AND RLLX_VERSION LIKE '%{0}%' ", this.tbRLLX_VERSION.Text));
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlWhere.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            string sqlVins = string.Format(@"select * from VIN_MERGER_INFO where 1=1  {0}", sqlWhere);
            string sqlStr = string.Format(@"select * from (select F.*,ROWNUM RN from ({0}) F where ROWNUM<={1}) where RN>{2}", sqlVins, pageSize * pageNum, pageSize * (pageNum - 1));
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
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
            sqlStr.Append("select * from VIN_MERGER_INFO where 1=1 ");
            if (!string.IsNullOrEmpty(this.tbVIN.Text))
            {
                sqlStr.Append(string.Format(@" AND VIN LIKE '%{0}%' ", this.tbVIN.Text));
            }
            if (!string.IsNullOrEmpty(this.tbSC_OCN.Text))
            {
                sqlStr.Append(string.Format(@" AND SC_OCN LIKE '%{0}%' ", this.tbSC_OCN.Text));
            }
            if (!string.IsNullOrEmpty(this.tbCLJBXX_VERSION.Text))
            {
                sqlStr.Append(string.Format(@" AND CLJBXX_VERSION LIKE '%{0}%' ", this.tbCLJBXX_VERSION.Text));
            }
            if (!string.IsNullOrEmpty(this.tbRLLX_VERSION.Text))
            {
                sqlStr.Append(string.Format(@" AND RLLX_VERSION LIKE '%{0}%' ", this.tbRLLX_VERSION.Text));
            }
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text))
            {
                sqlStr.Append(string.Format(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
            }
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr.ToString(), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

    }
}
