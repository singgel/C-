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
using Catarc.Adc.NewEnergyApproveSys.PopForm;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
{
    public partial class ApproveSecondForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private enum eAPP_STATUS
        {
            未审批 = 0,
            一审驳回 = 10,
            一审通过 = 11,
            A通过 = 13,
            A驳回 = 14,
            二审驳回 = 20,
            二审通过 = 21,
            三审驳回 = 30,
            三审通过 = 31
        };

        private enum eBDJG
        {
            VIN不是17位 = 1,
            VIN重复 = 2,
            找到 = 3,
            目录未找到 = 4
        };

        private enum eBDJG_GG
        {
            一致 = 2,
            不一致 = 3
        };

        public ApproveSecondForm()
        {
            InitializeComponent();
        }

        private void ApproveSecondForm_Load(object sender, EventArgs e)
        {
            //查询条件下拉框填充
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from SYS_DIC where 1=1 ", null);
            CLXZ.Properties.Items.Add(string.Empty);
            CLYT.Properties.Items.Add(string.Empty);
            CLZL.Properties.Items.Add(string.Empty);
            DQ.Properties.Items.Add(string.Empty);
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
                        case "地区":
                            DQ.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        case "年份":
                            JZNF.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        default: break;
                    }
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

        private void JZNF_SelectedIndexChanged(object sender, EventArgs e)
        {
            PC.Properties.Items.Clear();
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format("select * from SYS_DIC where DIC_TYPE='{0}' ", JZNF.Text));
            PC.Properties.Items.Add(string.Empty);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PC.Properties.Items.Add(ds.Tables[0].Rows[i]["DIC_NAME"].ToString().Trim());
                }
            }
        }

        //批量通过
        private void barBtnPass_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            //LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, this.Text + "-" + this.barBtnPass.Caption);
             
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                var dtExport = (DataTable)this.gcDataInfo.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dt = GridControlHelper.SelectedItems(this.gvDataInfo);
                if (dt.Rows.Count < 1)
                {
                    XtraMessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    using (var pf = new PassInfoForm(dt, 11))
                    {
                        pf.ShowDialog();
                        if (pf.DialogResult == DialogResult.OK)
                            this.refrashCurrentPage();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("只能在待审批页面操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //批量驳回
        private void barBtnReject_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnReject.Caption));
             
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                var dtExport = (DataTable)this.gcDataInfo.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dt = GridControlHelper.SelectedItems(this.gvDataInfo);
                if (dt.Rows.Count < 1)
                {
                    XtraMessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    using (var rf = new RejectInfoForm(dt, 11))
                    {
                        rf.ShowDialog();
                        if (rf.DialogResult == DialogResult.OK)
                            this.refrashCurrentPage();
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                XtraMessageBox.Show("只能在待审批页面操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //全部选中
        private void barBtnSelect_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                this.gvDataInfo.FocusedRowHandle = 0;
                this.gvDataInfo.FocusedColumn = gvDataInfo.Columns[1];
                GridControlHelper.SelectItem(this.gvDataInfo, true);
            }
            else
            {
                this.gvBackInfo.FocusedRowHandle = 0;
                this.gvBackInfo.FocusedColumn = gvBackInfo.Columns[1];
                GridControlHelper.SelectItem(this.gvBackInfo, true);
            }
        }

        //全部取消
        private void barBtnCancle_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                this.gvDataInfo.FocusedRowHandle = 0;
                this.gvDataInfo.FocusedColumn = gvDataInfo.Columns[1];
                GridControlHelper.SelectItem(this.gvDataInfo, false);
            }
            else
            {
                this.gvBackInfo.FocusedRowHandle = 0;
                this.gvBackInfo.FocusedColumn = gvBackInfo.Columns[1];
                GridControlHelper.SelectItem(this.gvBackInfo, false);
            }
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
             
                var dtExport = this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批") ? (DataTable)this.gcDataInfo.DataSource : (DataTable)this.gcBackInfo.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    var options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
                    {
                        this.gcDataInfo.ExportToXls(saveFileDialog.FileName, options);
                    }
                    else
                    {
                        this.gcBackInfo.ExportToXls(saveFileDialog.FileName, options);
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
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchLocal(1);
        }

        //清空查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.CQMC.Text = string.Empty;
            this.CLXZ.Text = string.Empty;
            this.CLZL.Text = string.Empty;
            this.CLYT.Text = string.Empty;
            this.CLXH.Text = string.Empty;
            this.VIN.Text = string.Empty;
            this.DQ.Text = string.Empty;
            this.BDJG.Text = string.Empty;
            this.txtGCCS.Text = string.Empty;
            this.dtEndXSZRQ.Text = string.Empty;
            this.dtStartXSZRQ.Text = string.Empty;
            this.dtEndFPRQ.Text = string.Empty;
            this.dtStartFPRQ.Text = string.Empty;
            this.txtEndLJXSLC.Text = string.Empty;
            this.txtStartLJXSLC.Text = string.Empty;
            this.JZNF.Text = string.Empty;
            this.PC.Text = string.Empty;
            if (this.btnExtend.Text.Equals("收起高级查询  ↑"))
            {
                this.DCDTXX_XH.Text = string.Empty;
                this.DCDTXX_SCQY.Text = string.Empty;
                this.DCZXX_XH.Text = string.Empty;
                this.DCZXX_SCQY.Text = string.Empty;
                this.QDDJXX_XH.Text = string.Empty;
                this.QDDJXX_SCQY.Text = string.Empty;
                this.RLDCXX_XH.Text = string.Empty;
                this.RLDCXX_SCQY.Text = string.Empty;
                this.CJDRXX_DTXH.Text = string.Empty;
                this.CJDRXX_DTSCQY.Text = string.Empty;
                this.CJDRXX_CXXH.Text = string.Empty;
                this.CJDRXX_DRZSCQY.Text = string.Empty;
            }
        }

        //高级查询
        private void btnExtend_Click(object sender, EventArgs e)
        {
            if (this.btnExtend.Text.Equals("展开高级查询  ↓"))
            {
                this.splitContainerControl1.SplitterPosition = 300;
                this.btnExtend.Text = "收起高级查询  ↑";
            }
            else
            {
                this.splitContainerControl1.SplitterPosition = 150;
                this.btnExtend.Text = "展开高级查询  ↓";
            }
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
                    if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
                    {
                        this.gcDataInfo.DataSource = dtQuery;
                        //this.gvDataInfo.BestFitColumns();
                    }
                    else
                    {
                        this.gcBackInfo.DataSource = dtQuery;
                        //this.gvBackInfo.BestFitColumns();
                    }
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
                    if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
                    {
                        this.gcDataInfo.DataSource = dtQuery;
                        //this.gvDataInfo.BestFitColumns();
                    }
                    else
                    {
                        this.gcBackInfo.DataSource = dtQuery;
                        //this.gvBackInfo.BestFitColumns();
                    }
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
            string sqlCount = string.Empty;
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                sqlCount = String.Format("select count(*) from DB_INFOMATION where APP_STATUS=11 {0}", this.queryParam());
            }
            else
            {
                sqlCount = String.Format("select count(*) from DB_INFOMATION where APP_STATUS=20 {0}", this.queryParam());
            }
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sqlCount));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            string sqlStr = string.Empty;
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                sqlStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM DB_INFOMATION where APP_STATUS=11 {0} order by VIN desc) A WHERE ROWNUM <= {1}) WHERE RN > {2}", this.queryParam(), pageSize * pageNum, pageSize * (pageNum - 1));
            }
            else
            {
                sqlStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM DB_INFOMATION where APP_STATUS=20 {0} order by VIN desc) A WHERE ROWNUM <= {1}) WHERE RN > {2}", this.queryParam(), pageSize * pageNum, pageSize * (pageNum - 1));
            }
            string linqStr = string.Format("select i.*,n.GGPC_GG,n.DCDTXX_XH_GG,n.DCDTXX_SCQY_GG,n.DCZXX_XH_GG,n.DCZXX_ZRL_GG,n.DCZXX_SCQY_GG,n.QDDJXX_XH_1_GG,n.QDDJXX_EDGL_1_GG,n.QDDJXX_SCQY_1_GG,n.RLDCXX_XH_GG,n.RLDCXX_EDGL_GG,n.RLDCXX_SCQY_GG from ({0}) i left outer join DB_DIFFERENT n on i.VIN=n.VIN", sqlStr);
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, linqStr, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //获取全部数据
        private DataTable queryAll()
        {
            string sqlStr = string.Empty;
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                sqlStr = String.Format("select * from DB_INFOMATION where APP_STATUS=11 {0}", this.queryParam());
            }
            else
            {
                sqlStr = String.Format("select * from DB_INFOMATION where APP_STATUS=20 {0}", this.queryParam());
            }
            string linqStr = string.Format("select i.*,n.GGPC_GG,n.DCDTXX_XH_GG,n.DCDTXX_SCQY_GG,n.DCZXX_XH_GG,n.DCZXX_ZRL_GG,n.DCZXX_SCQY_GG,n.QDDJXX_XH_1_GG,n.QDDJXX_EDGL_1_GG,n.QDDJXX_SCQY_1_GG,n.RLDCXX_XH_GG,n.RLDCXX_EDGL_GG,n.RLDCXX_SCQY_GG from ({0}) i left outer join DB_DIFFERENT n on i.VIN=n.VIN", sqlStr);
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, linqStr, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            if (!string.IsNullOrEmpty(CQMC.Text))
            {
                sqlStr.AppendFormat(" AND (CLSCQY like '%{0}%')", CQMC.Text);
            }
            if (!string.IsNullOrEmpty(CLXZ.Text))
            {
                sqlStr.AppendFormat(" AND (CLXZ = '{0}')", CLXZ.Text);
            }
            if (!string.IsNullOrEmpty(CLZL.Text))
            {
                sqlStr.AppendFormat(" AND (CLZL = '{0}')", CLZL.Text);
            }
            if (!string.IsNullOrEmpty(CLYT.Text))
            {
                sqlStr.AppendFormat(" AND (CLYT = '{0}')", CLYT.Text);
            }
            if (!string.IsNullOrEmpty(VIN.Text))
            {
                sqlStr.AppendFormat(" AND (VIN like '%{0}%')", VIN.Text);
            }
            if (!string.IsNullOrEmpty(CLXH.Text))
            {
                sqlStr.AppendFormat(" AND (CLXH like '%{0}%')", CLXH.Text);
            }
            if (!string.IsNullOrEmpty(DQ.Text))
            {
                sqlStr.AppendFormat(" AND DQ = '{0}'  ", DQ.Text);
            }
            if (!string.IsNullOrEmpty(txtGCCS.Text))
            {
                sqlStr.AppendFormat(" AND GCCS like '%{0}%'  ", txtGCCS.Text);
            }
            if (!string.IsNullOrEmpty(BDJG.Text))
            {
                if (BDJG.Text.Equals("一致") || BDJG.Text.Equals("不一致"))
                {
                    int bdjg_gg = Convert.ToInt32(Enum.Parse(typeof(eBDJG_GG), BDJG.Text));
                    sqlStr.AppendFormat(" AND (BDJG_GG = {0})", bdjg_gg.ToString());
                }
                else if (BDJG.Text.Equals("找到"))
                {
                    sqlStr.Append(" AND (BTBZ is not null)");
                }
                else if (BDJG.Text.Equals("目录未找到"))
                {
                    sqlStr.Append(" AND (BTBZ is null)");
                }
                else if (BDJG.Text.Equals("VIN不是17位"))
                {
                    sqlStr.Append(" AND (length(VIN)!=17)");
                }
                else
                {
                    sqlStr.Append(" AND (BDJG = 20 OR BDJG = 21 OR BDJG = 22)");
                }
            }
            if (!string.IsNullOrEmpty(dtStartXSZRQ.Text))
            {
                sqlStr.AppendFormat(" AND to_date(to_char(XSZSJ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartXSZRQ.Text));
            }
            if (!string.IsNullOrEmpty(dtEndXSZRQ.Text))
            {
                sqlStr.AppendFormat(" AND to_date(to_char(XSZSJ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtEndXSZRQ.Text));
            }
            if (!string.IsNullOrEmpty(dtStartFPRQ.Text))
            {
                sqlStr.AppendFormat(" AND to_date(to_char(FPSJ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartFPRQ.Text));
            }
            if (!string.IsNullOrEmpty(dtEndFPRQ.Text))
            {
                sqlStr.AppendFormat(" AND to_date(to_char(FPSJ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtEndFPRQ.Text));
            }
            if (!string.IsNullOrEmpty(txtStartLJXSLC.Text))
            {
                sqlStr.AppendFormat(" AND  to_number(LJXSLC) >= {0} ", this.txtStartLJXSLC.Text);
            }
            if (!string.IsNullOrEmpty(txtEndLJXSLC.Text))
            {
                sqlStr.AppendFormat(" AND to_number(LJXSLC) <= {0} ", this.txtEndLJXSLC.Text);
            }
            if (!string.IsNullOrEmpty(JZNF.Text))
            {
                sqlStr.AppendFormat(" AND JZNF = {0} ", this.JZNF.Text);
            }
            if (!string.IsNullOrEmpty(PC.Text))
            {
                sqlStr.AppendFormat(" AND PC = {0} ", Convert.ToInt32(this.PC.Text));
            }

            if (this.btnExtend.Text.Equals("收起高级查询  ↑"))
            {
                if (!string.IsNullOrEmpty(DCDTXX_XH.Text))
                {
                    sqlStr.AppendFormat(" AND (DCDTXX_XH like '%{0}%')", DCDTXX_XH.Text);
                }
                if (!string.IsNullOrEmpty(DCDTXX_SCQY.Text))
                {
                    sqlStr.AppendFormat(" AND (DCDTXX_SCQY like '%{0}%')", DCDTXX_SCQY.Text);
                }
                if (!string.IsNullOrEmpty(DCZXX_XH.Text))
                {
                    sqlStr.AppendFormat(" AND (DCZXX_XH like '%{0}%')", DCZXX_XH.Text);
                }
                if (!string.IsNullOrEmpty(DCZXX_SCQY.Text))
                {
                    sqlStr.AppendFormat(" AND (DCZXX_SCQY like '%{0}%')", DCZXX_SCQY.Text);
                }
                if (!string.IsNullOrEmpty(QDDJXX_XH.Text))
                {
                    sqlStr.AppendFormat(" AND (QDDJXX_XH_1 like '%{0}%' or QDDJXX_XH_2 like '%{0}%')", QDDJXX_XH.Text);
                }
                if (!string.IsNullOrEmpty(QDDJXX_SCQY.Text))
                {
                    sqlStr.AppendFormat(" AND (QDDJXX_SCQY_1 like '%{0}%' or QDDJXX_SCQY_2 like '%{0}%')", QDDJXX_SCQY.Text);
                }
                if (!string.IsNullOrEmpty(RLDCXX_XH.Text))
                {
                    sqlStr.AppendFormat(" AND (RLDCXX_XH like '%{0}%')", RLDCXX_XH.Text);
                }
                if (!string.IsNullOrEmpty(RLDCXX_SCQY.Text))
                {
                    sqlStr.AppendFormat(" AND (RLDCXX_SCQY like '%{0}%')", RLDCXX_SCQY.Text);
                }
                if (!string.IsNullOrEmpty(CJDRXX_DTXH.Text))
                {
                    sqlStr.AppendFormat(" AND (CJDRXX_DTXH like '%{0}%')", CJDRXX_DTXH.Text);
                }
                if (!string.IsNullOrEmpty(CJDRXX_DTSCQY.Text))
                {
                    sqlStr.AppendFormat(" AND (CJDRXX_DTSCQY like '%{0}%')", CJDRXX_DTSCQY.Text);
                }
                if (!string.IsNullOrEmpty(CJDRXX_CXXH.Text))
                {
                    sqlStr.AppendFormat(" AND (CJDRXX_CXXH like '%{0}%')", CJDRXX_CXXH.Text);
                }
                if (!string.IsNullOrEmpty(CJDRXX_DRZSCQY.Text))
                {
                    sqlStr.AppendFormat(" AND (CJDRXX_DRZSCQY like '%{0}%')", CJDRXX_DRZSCQY.Text);
                }
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

        //改变行颜色
        private void gvDataInfo_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            int hand = e.RowHandle;
            if (hand < 0) return;
            DataRow dr = this.gvDataInfo.GetDataRow(hand);
            if (dr == null) return;
            if (dr["BDJG_GG"].ToString().Equals("3") || (Convert.ToDecimal(dr["LJXSLC"].ToString()) < 30000) || string.IsNullOrEmpty(dr["BTBZ"].ToString()))
            {
                e.Appearance.BackColor = Color.LightPink;
            }
        }

        //修改列显示文本
        private void gvDataInfo_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            //0：未审批/10：一审驳回、11：一审通过、12：一审争议/20：二审驳回、21：二审通过/30：三审驳回、31：三审通过
            if (e.Column.FieldName == "APP_STATUS")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "-1":
                        e.DisplayText = "比对中";
                        break;
                    case "0":
                        e.DisplayText = "未审批";
                        break;
                    case "10":
                        e.DisplayText = "一审驳回";
                        break;
                    case "11":
                        e.DisplayText = "一审通过";
                        break;
                    case "13":
                        e.DisplayText = "A一审通过";
                        break;
                    case "14":
                        e.DisplayText = "A一审驳回";
                        break;
                    case "20":
                        e.DisplayText = "二审驳回";
                        break;
                    case "21":
                        e.DisplayText = "二审通过";
                        break;
                    case "30":
                        e.DisplayText = "三审驳回";
                        break;
                    case "31":
                        e.DisplayText = "三审通过";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
            //1:VIN不是17位/2:VIN重复/3:找到/4:目录未找到
            if (e.Column.FieldName == "BDJG")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "-1":
                        e.DisplayText = "比对中";
                        break;
                    case "0":
                        e.DisplayText = "未比对数据";
                        break;
                    case "1":
                        e.DisplayText = "VIN不是17位";
                        break;
                    case "20":
                        e.DisplayText = "VIN重复未审批";
                        break;
                    case "21":
                        e.DisplayText = "VIN重复审批已通过";
                        break;
                    case "22":
                        e.DisplayText = "VIN重复审批已驳回";
                        break;
                    case "3":
                        e.DisplayText = "找到";
                        break;
                    case "4":
                        e.DisplayText = "目录未找到";
                        break;
                    case "5":
                        e.DisplayText = "累计续驶里程小于3万公里";
                        break;
                    default:
                        e.DisplayText = "比对中";
                        break;
                }
            }
            //null：目录未找到/有值：目录存在
            if (e.Column.FieldName == "BTBZ")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "":
                        e.DisplayText = "目录未找到";
                        break;
                }
            }
        }

        //窗体双击事件,VIN号的顺序取决于所查询的表
        private void gcDataInfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            GridControl gc;
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                gc = this.gcDataInfo;
            }
            else
            {
                gc = this.gcBackInfo;
            }
            if (gc == null) return;
            ColumnView cv = (ColumnView)gc.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null) return;
            string guid = (string)dr.Row["GUID"];
            Dictionary<string, string> mapResult = QueryHelper.queryInfomationByVin(guid);
            if (mapResult.Count < 1)
            {
                XtraMessageBox.Show("未查到该vin对应数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Dictionary<string, string> mapRightData = new Dictionary<string, string>();
            if (!dr.Row["DCDTXX_XH_GG"].Equals(DBNull.Value)) mapRightData.Add("DCDTXX_XH", dr.Row["DCDTXX_XH_GG"].ToString());
            if (!dr.Row["DCDTXX_SCQY_GG"].Equals(DBNull.Value)) mapRightData.Add("DCDTXX_SCQY", dr.Row["DCDTXX_SCQY_GG"].ToString());
            if (!dr.Row["DCZXX_XH_GG"].Equals(DBNull.Value)) mapRightData.Add("DCZXX_XH", dr.Row["DCZXX_XH_GG"].ToString());
            if (!dr.Row["DCZXX_ZRL_GG"].Equals(DBNull.Value)) mapRightData.Add("DCZXX_ZRL", dr.Row["DCZXX_ZRL_GG"].ToString());
            if (!dr.Row["DCZXX_SCQY_GG"].Equals(DBNull.Value)) mapRightData.Add("DCZXX_SCQY", dr.Row["DCZXX_SCQY_GG"].ToString());
            if (!dr.Row["QDDJXX_XH_1_GG"].Equals(DBNull.Value)) mapRightData.Add("QDDJXX_XH_1", dr.Row["QDDJXX_XH_1_GG"].ToString());
            if (!dr.Row["QDDJXX_EDGL_1_GG"].Equals(DBNull.Value)) mapRightData.Add("QDDJXX_EDGL_1", dr.Row["QDDJXX_EDGL_1_GG"].ToString());
            if (!dr.Row["QDDJXX_SCQY_1_GG"].Equals(DBNull.Value)) mapRightData.Add("QDDJXX_SCQY_1", dr.Row["QDDJXX_SCQY_1_GG"].ToString());
            if (!dr.Row["RLDCXX_XH_GG"].Equals(DBNull.Value)) mapRightData.Add("RLDCXX_XH", dr.Row["RLDCXX_XH_GG"].ToString());
            if (!dr.Row["RLDCXX_EDGL_GG"].Equals(DBNull.Value)) mapRightData.Add("RLDCXX_EDGL", dr.Row["RLDCXX_EDGL_GG"].ToString());
            if (!dr.Row["RLDCXX_SCQY_GG"].Equals(DBNull.Value)) mapRightData.Add("RLDCXX_SCQY", dr.Row["RLDCXX_SCQY_GG"].ToString());
            if (Convert.ToDecimal(dr.Row["LJXSLC"]) < 30000) mapRightData.Add("LJXSLC", "累计续驶里程小于3万公里");
            using (var dlg = new SingleInfoForm(mapResult, mapRightData))
            {
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                    this.refrashCurrentPage();
            }
        }
        //数据恢复
        private void barBtnRecover_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnRecover.Caption));
             
            if (!this.xtraTabControl1.SelectedTabPage.Text.Equals("待审批"))
            {
                DataTable dtExport = new DataTable();
               
                dtExport = (DataTable)this.gcBackInfo.DataSource;

                string msg = string.Empty;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (GridControlHelper.SelectedItems(this.gvBackInfo).Rows.Count < 1)
                {
                    XtraMessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (XtraMessageBox.Show("确定要恢复吗？", "恢复确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
                {
                    return;
                }
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    var utils = new ApproveSecondUtils();
                    var selDataTable = GridControlHelper.SelectedItems(this.gvBackInfo);
                    msg = utils.recoverDataInfo(selDataTable);
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
                using (var mf = new MessageForm(msg) { Text = "恢复操作结果" })
                {
                    mf.ShowDialog();
                    if (mf.DialogResult == DialogResult.Cancel)
                        this.refrashCurrentPage();
                }
            }
            else
            {
                XtraMessageBox.Show("该操作不能在待审批页面操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}