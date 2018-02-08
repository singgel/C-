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
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
{
    public partial class InspectInfoForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public InspectInfoForm()
        {
            InitializeComponent();
        }

        private void InspectInfoForm_Load(object sender, EventArgs e)
        {
            //查询条件下拉框填充
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from SYS_DIC where 1=1 ", null);
            CLXZ.Properties.Items.Add(string.Empty);
            CLYT.Properties.Items.Add(string.Empty);
            CLZL.Properties.Items.Add(string.Empty);
            //DQ.Properties.Items.Add(string.Empty);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int iDIC = ds.Tables[0].Rows.Count;
                for (int i = 0; i < iDIC; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    switch (dr["DIC_TYPE"].ToString().Trim())
                    {
                        case "车辆性质":
                            CLXZ.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        case "车辆用途":
                            CLYT.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        case "车辆种类":
                            CLZL.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        //case "地区":
                        //    DQ.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                        //    break;
                        default: break;
                    }
                }
            }
            //查询时间条件下拉框填充
            this.YEAR.Properties.Items.Clear();
            this.MONTH.Properties.Items.Clear();
            for (int i = 2015; i <= DateTime.Now.Year; i++)
            {
                this.YEAR.Properties.Items.Add(i);
            }
            for (int i = 1; i <= 12; i++)
            {
                if (i < 10)
                {
                    this.MONTH.Properties.Items.Add("0" + i);
                }
                else
                {
                    this.MONTH.Properties.Items.Add(i);
                }
            }

        }

        private void CLZL_SelectedIndexChanged(object sender, EventArgs e)
        {
            CLYT.Properties.Items.Clear();
            switch (CLZL.Properties.Items[CLZL.SelectedIndex].ToString())
            {
                case "插电式混合动力客车":
                    CLYT.Properties.Items.Add("公交");
                    CLYT.Properties.Items.Add("通勤");
                    CLYT.Properties.Items.Add("旅游");
                    CLYT.Properties.Items.Add("公路");
                    break;
                case "插电式混合动力乘用车":
                    CLYT.Properties.Items.Add("公务");
                    CLYT.Properties.Items.Add("出租");
                    CLYT.Properties.Items.Add("租赁");
                    CLYT.Properties.Items.Add("私人");
                    break;
                case "纯电动客车":
                    CLYT.Properties.Items.Add("公交");
                    CLYT.Properties.Items.Add("通勤");
                    CLYT.Properties.Items.Add("旅游");
                    CLYT.Properties.Items.Add("公路");
                    break;
                case "纯电动乘用车":
                    CLYT.Properties.Items.Add("公务");
                    CLYT.Properties.Items.Add("出租");
                    CLYT.Properties.Items.Add("租赁");
                    CLYT.Properties.Items.Add("私人");
                    break;
                case "纯电动特种车":
                    CLYT.Properties.Items.Add("邮政");
                    CLYT.Properties.Items.Add("物流");
                    CLYT.Properties.Items.Add("环卫");
                    CLYT.Properties.Items.Add("工程");
                    break;
                case "燃料电池客车":
                    CLYT.Properties.Items.Add("公交");
                    CLYT.Properties.Items.Add("通勤");
                    CLYT.Properties.Items.Add("旅游");
                    CLYT.Properties.Items.Add("公路");
                    break;
                case "燃料电池乘用车":
                    CLYT.Properties.Items.Add("公务");
                    CLYT.Properties.Items.Add("出租");
                    CLYT.Properties.Items.Add("租赁");
                    CLYT.Properties.Items.Add("私人");
                    break;
                case "燃料电池货车":
                    CLYT.Properties.Items.Add("邮政");
                    CLYT.Properties.Items.Add("物流");
                    CLYT.Properties.Items.Add("环卫");
                    CLYT.Properties.Items.Add("工程");
                    break;
            }
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
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, this.Text + "-" + this.barBtnExport.Caption);

                var dtExport = (DataTable)this.gcDataInfo.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    var options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    this.gcDataInfo.ExportToXls(saveFileDialog.FileName, options);
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


        //导出数据
        private void barBtnExportFloder_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnExportFloder.Caption));

                this.saveFileDialog.FileName = "实地核查" + DateTime.Now.ToString("yyyyMMddHHmmss");
                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    DataTable dt = this.queryAll();
                    InspectInfoUtils utils = new InspectInfoUtils();
                    string msg = utils.dataToModelExcel_XLS(dt, saveFileDialog.FileName);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        XtraMessageBox.Show(string.Format("操作失败，原因：{0}", msg), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
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
            finally
            {
                SplashScreenManager.CloseForm();
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
            this.CLXZ.Text = string.Empty;
            this.GCCS.Text = string.Empty;
            this.CLYXDW.Text = string.Empty;
            this.CLZL.Text = string.Empty;
            this.CLYT.Text = string.Empty;
            this.CLXH.Text = string.Empty;
            this.YEAR.Text = string.Empty;
            this.MONTH.Text = string.Empty;
            this.CLCSQY.Text = string.Empty;
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
                    //this.gvDataInfo.BestFitColumns();
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
                    //this.gvDataInfo.BestFitColumns();
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
            string sqlCount = String.Format("select count(*) from VIEW_INSPECT where 1=1 {0}", this.queryParam());
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sqlCount));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            string sqlStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM VIEW_INSPECT where 1=1 {0} order by CLSCQY desc) A WHERE ROWNUM <= {1}) WHERE RN > {2}", this.queryParam(), pageSize * pageNum, pageSize * (pageNum - 1));
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //获取全部数据
        private DataTable queryAll()
        {
            string sqlAll = String.Format("select * from VIEW_INSPECT where 1=1 {0}", this.queryParam());
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlAll, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            if (!string.IsNullOrEmpty(CLXZ.Text))
            {
                sqlStr.AppendFormat(" AND (CLXZ = '{0}')", CLXZ.Text);
            }
            if (!string.IsNullOrEmpty(GCCS.Text))
            {
                sqlStr.AppendFormat(" AND (GCCS like '%{0}%')", GCCS.Text);
            }
            if (!string.IsNullOrEmpty(CLYXDW.Text))
            {
                sqlStr.AppendFormat(" AND (CLYXDW like '%{0}%')", CLYXDW.Text);
            }
            if (!string.IsNullOrEmpty(CLZL.Text))
            {
                sqlStr.AppendFormat(" AND (CLZL = '{0}')", CLZL.Text);
            }
            if (!string.IsNullOrEmpty(CLYT.Text))
            {
                sqlStr.AppendFormat(" AND (CLYT = '{0}')", CLYT.Text);
            }
            if (!string.IsNullOrEmpty(CLXH.Text))
            {
                sqlStr.AppendFormat(" AND (CLXH like '%{0}%')", CLXH.Text);
            }
            if (!string.IsNullOrEmpty(CLCSQY.Text))
            {
                sqlStr.AppendFormat(" AND (CLSCQY like '%{0}%')", CLCSQY.Text);
            }
            if (!string.IsNullOrEmpty(YEAR.Text))
            {
                sqlStr.AppendFormat(" AND (substr(SPSJ,0,4) = '{0}')", YEAR.Text);
            }
            if (!string.IsNullOrEmpty(MONTH.Text))
            {
                sqlStr.AppendFormat(" AND (substr(SPSJ,6,2) = '{0}')", MONTH.Text);
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
    }
}