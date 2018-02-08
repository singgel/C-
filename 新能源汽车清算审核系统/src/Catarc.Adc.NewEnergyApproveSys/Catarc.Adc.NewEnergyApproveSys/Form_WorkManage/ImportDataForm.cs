using System;
using System.Collections.Generic;
using System.Linq;
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
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyApproveSys.DataUtils;
using HFSoft.Component.Windows;
using Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils;
using System.Threading;
using Catarc.Adc.NewEnergyApproveSys.LogUtils;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
{
    public partial class ImportDataForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private enum hAPP_STATUS
        {
            一审驳回 = 5,
            二审驳回 = 15,
            三审驳回 = 25,
            三审通过 = 40
        };

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

        public ImportDataForm()
        {
            InitializeComponent();
        }

        private void ImportDataForm_Load(object sender, EventArgs e)
        {
            //查询条件下拉框填充
            CLXZ.Properties.Items.Add(string.Empty);
            CLYT.Properties.Items.Add(string.Empty);
            CLZL.Properties.Items.Add(string.Empty);
            DQ.Properties.Items.Add(string.Empty);
            JZNF.Properties.Items.Add(string.Empty);
            PC.Properties.Items.Add(string.Empty);
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from SYS_DIC", null);
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
                string maxJZNF = ds.Tables[0].AsEnumerable().Where(d => d.Field<string>("DIC_TYPE").Equals("年份")).Select(d => d.Field<string>("DIC_NAME")).ToArray().Max<string>();
                JZNF.Text = maxJZNF;
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
                string maxPC = ds.Tables[0].AsEnumerable().Select(d => d.Field<string>("DIC_NAME")).ToArray().Max<string>();
                PC.Text = maxPC;
            }
        }

        //批量导入
        private void barBtnImportNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnImportNew.Caption));
             
            if (string.IsNullOrEmpty(this.DQ.Text.Trim()))
            {
                XtraMessageBox.Show("请选择地区！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(this.PC.Text.Trim()))
            {
                XtraMessageBox.Show("请选择批次！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string msg = string.Empty;
            this.folderBrowserDialog.Description = "批量导入新能源填报数据";
            if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                return;
            try
            {
                string CLSCQY = string.Empty;
                LoadingHandler.Show(this, args =>
                {
                    var utils = new ImportDataUtils();
                    msg = utils.ImportNewTemplate(folderBrowserDialog.SelectedPath, this.DQ.Text, this.PC.Text, ref CLSCQY);
                });
                Thread thSendData = new Thread(() => CompareThread(CLSCQY)) { IsBackground = true };
                thSendData.Start();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            using (var mf = new MessageForm(msg) { Text = "数据导入操作结果" })
            {
                mf.ShowDialog();
                if (mf.DialogResult == DialogResult.Cancel)
                    this.refrashCurrentPage();
            }
        }

        private void CompareThread(string clscqy)
        {
            try
            {
                string msg = string.Empty;
                if (string.IsNullOrEmpty(clscqy))
                {
                    LogManager.Log("Log", "CompareThread", "CLSCQY为null");
                    return;
                }
                var compareUtils = new DataCompareHelper();
                msg = compareUtils.CompareDataTableThread(clscqy);
                LogManager.Log("Log", "CompareThread", msg);
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "CompareThread", ex.Source + ex.Message);
            }
        }

        //数据恢复
        private void barBtnRecover_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnRecover.Caption));
             
            var dtExport = (DataTable)this.gcDataInfo.DataSource;
            string msg = string.Empty;
            if (dtExport == null || dtExport.Rows.Count < 1)
            {
                XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GridControlHelper.SelectedItems(this.gvDataInfo).Rows.Count < 1)
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
                var utils = new ImportDataUtils();
                var selDataTable = GridControlHelper.SelectedItems(this.gvDataInfo);
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

        //删除记录
        private void barBtnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnDelete.Caption));
             
            var dtExport = (DataTable)this.gcDataInfo.DataSource;
            string msg = string.Empty;
            if (dtExport == null || dtExport.Rows.Count < 1)
            {
                XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GridControlHelper.SelectedItems(this.gvDataInfo).Rows.Count < 1)
            {
                XtraMessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (XtraMessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                var utils = new ImportDataUtils();
                var selDataTable = GridControlHelper.SelectedItems(this.gvDataInfo);
                msg = utils.deleteDataInfo(selDataTable);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
            using (var mf = new MessageForm(msg) { Text = "删除操作结果" })
            {
                mf.ShowDialog();
                if (mf.DialogResult == DialogResult.Cancel)
                    this.refrashCurrentPage();
            }
        }

        //数据比对
        private void barBtnCompare_ItemClick(object sender, ItemClickEventArgs e)
        {
            string msg = string.Empty;
            try
            {
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnCompare.Caption));

                var dtSelect = GridControlHelper.SelectedItems(this.gvDataInfo);
                LoadingHandler.Show(this, args =>
                {
                    var compare = new DataCompareHelper();
                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        var guidList = dtSelect.AsEnumerable().Select(d => d.Field<string>("GUID")).ToList();
                        while (guidList.Count > 0)
                        {
                            var guidArrSkip = guidList.Take(1000);
                            stringBuilder.AppendFormat(" or GUID in('{0}')", string.Join("','", guidArrSkip));
                            if (guidList.Count > 999)
                            {
                                guidList.RemoveRange(0, 999);
                            }
                            else
                            {
                                guidList.RemoveRange(0, guidList.Count);
                            }
                        }
                        msg = compare.CompareDataTable(string.Format("and ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r')));
                    }
                    else
                    {
                        msg = compare.CompareDataTable(string.Empty);
                    }
                });
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            using (var mf = new MessageForm(msg) { Text = "比对操作结果" })
            {
                mf.ShowDialog();
                if (mf.DialogResult == DialogResult.Cancel)
                    this.refrashCurrentPage();
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

        //详情导出
        private void barBtnExportFloder_ItemClick(object sender, ItemClickEventArgs e)
        {
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnExportFloder.Caption));
            
            string msg = string.Empty;
            this.saveFileDialog.FileName = "详情数据" + DateTime.Now.ToString("yyyyMMddHHmmss");
            this.saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    DataTable dtExport = new DataTable();
                    dtExport = GridControlHelper.SelectedItems(this.gvDataInfo);
                    if (dtExport == null || dtExport.Rows.Count < 1)
                    {
                        dtExport = queryAll();
                    }
                    var utils = new ImportDataUtils();
                    //msg = utils.dataToModelExcel(dtExport, saveFileDialog.FileName);
                    msg = utils.dataToModelExcel_XLS(dtExport, saveFileDialog.FileName);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        XtraMessageBox.Show("导出失败：" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (XtraMessageBox.Show("导出成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
                catch (System.Exception ex)
                {
                    XtraMessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
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
            this.CLSCQY.Text = string.Empty;
            this.CLXZ.Text = string.Empty;
            this.CLZL.Text = string.Empty;
            this.CLYT.Text = string.Empty;
            this.CLXH.Text = string.Empty;
            this.VIN.Text = string.Empty;
            this.BDJG.Text = string.Empty;
            this.APP_STATUS.Text = string.Empty;
            this.DQ.Text = string.Empty;
            this.GCCS.Text = string.Empty;
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
                this.splitContainerControl1.SplitterPosition = 275;
                this.btnExtend.Text = "收起高级查询  ↑";
            }
            else
            {
                this.splitContainerControl1.SplitterPosition = 125;
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
            string sqlCount = string.Empty;
            if (this.PC.Text == "01")
            {
                string sqlStr = "select VIN as VIN,'' as CLXZ,CARTYPE as CLZL,'' as GCSF,BUYCITY as GCCS,CARPURPOSE as CLYT,CARMODEL as CLXH,BATCHCODE as GGPC,CARCODE as CLPZ,'' as EKGZ,BUYPRICE as GMJG,subsidyStandard as SQBZBZ,TICKETCODE as FPHM,TICKETTIME as FPSJ,VEHICLELICENSE as XSZSJ,'' as FPTP,'' as XSZTP,'' as FPTP_PICTURE,'' as XSZTP_PICTURE,capacitanceBoxModel as CJDRXX_CXXH,capacitanceCapacity as CJDRXX_DRZRL,capacitanceProduct as CJDRXX_DRZSCQY,batterySingleProduct as CJDRXX_DTSCQY,batterySingleModel as CJDRXX_DTXH,capacitanceSysPrice as CJDRXX_XTJG,capacitanceLife as CJDRXX_ZBNX,SUPPERCAPACITANCE as CLSFYCJDR,'' as CLSFYQDDJ2,FUELCELL as CLSFYRLDC,batteryProduct as DCDTXX_SCQY,batteryModel as DCDTXX_XH,batteryBoxProduct as DCZXX_SCQY,batteryBoxModel as DCZXX_XH,bateryBoxSysPrice as DCZXX_XTJG,batteryBoxRepair as DCZXX_ZBNX,batterysCapacity as DCZXX_ZRL,motorpower as QDDJXX_EDGL_1,'' as QDDJXX_EDGL_2,motorProduct as QDDJXX_SCQY_1,'' as QDDJXX_SCQY_2,motorModel as QDDJXX_XH_1,'' as QDDJXX_XH_2,motorSysPrice as QDDJXX_XTJG_1,'' as QDDJXX_XTJG_2,fuelcellpower as RLDCXX_EDGL,fuelcellSysPrice as RLDCXX_GMJG,fuelcellProduct as RLDCXX_SCQY,fuelcellmodel as RLDCXX_XH,fuelcelllife as RLDCXX_ZBNX,applyYear as JZNF,kilometersPower as BGLHDL,signleMileage as CLCMYCDNGXSLC,mileageTime as CLYCCMDSXSJ,carCompany as CLYXDW,monitorUnit as JKPDXXDW,charge as LJCDL,hydrogenKg as LJJQL,airKg as LJJQL_G,airL as LJJQL_L,oil as LJJYL,mileage as LJXSLC,runtimeDay as PJDRXYSJ,monitor as SFAZJKZZ,mileageMonth as YJXSLC,mileagePower as ZDCDGL,approvalPerson1 as APP_NAME_1_A,'' as APP_NAME_1_B,approvalTime1 as APP_TIME_1_A,'' as APP_TIME_1_B,'' as APP_RESULT_1_A,'' as APP_RESULT_1_B,approvalPerson2 as APP_NAME_2,approvalTime2 as APP_TIME_2,approvaldes2 as APP_RESULT_2,approvalPerson3 as APP_NAME_3,approvalTime3 as APP_TIME_3,approvaldes3 as APP_RESULT_3,approvalAccount as APP_MONEY,approvalFlag as APP_STATUS,entname as CLSCQY,recommendAccount as BTBZ,approvalFlag as BDJG,'' as BDJG_GG,dict as DQ,ID as GUID,1 PC from APPLY_IMP_DETAIL_201601";
                sqlCount = String.Format("select count(*) from ({0}) where 1=1 {1}", sqlStr, this.queryParam());
            }
            else if (this.PC.Text == "02")
            {
                string sqlStr = "select VIN as VIN,'' as CLXZ,CARTYPE as CLZL,'' as GCSF,BUYCITY as GCCS,CARPURPOSE as CLYT,CARMODEL as CLXH,BATCHCODE as GGPC,CARCODE as CLPZ,'' as EKGZ,BUYPRICE as GMJG,subsidyStandard as SQBZBZ,TICKETCODE as FPHM,TICKETTIME as FPSJ,VEHICLELICENSE as XSZSJ,'' as FPTP,'' as XSZTP,'' as FPTP_PICTURE,'' as XSZTP_PICTURE,capacitanceBoxModel as CJDRXX_CXXH,capacitanceCapacity as CJDRXX_DRZRL,capacitanceProduct as CJDRXX_DRZSCQY,batterySingleProduct as CJDRXX_DTSCQY,batterySingleModel as CJDRXX_DTXH,capacitanceSysPrice as CJDRXX_XTJG,capacitanceLife as CJDRXX_ZBNX,SUPPERCAPACITANCE as CLSFYCJDR,'' as CLSFYQDDJ2,FUELCELL as CLSFYRLDC,batteryProduct as DCDTXX_SCQY,batteryModel as DCDTXX_XH,batteryBoxProduct as DCZXX_SCQY,batteryBoxModel as DCZXX_XH,bateryBoxSysPrice as DCZXX_XTJG,batteryBoxRepair as DCZXX_ZBNX,batterysCapacity as DCZXX_ZRL,motorpower as QDDJXX_EDGL_1,'' as QDDJXX_EDGL_2,motorProduct as QDDJXX_SCQY_1,'' as QDDJXX_SCQY_2,motorModel as QDDJXX_XH_1,'' as QDDJXX_XH_2,motorSysPrice as QDDJXX_XTJG_1,'' as QDDJXX_XTJG_2,fuelcellpower as RLDCXX_EDGL,fuelcellSysPrice as RLDCXX_GMJG,fuelcellProduct as RLDCXX_SCQY,fuelcellmodel as RLDCXX_XH,fuelcelllife as RLDCXX_ZBNX,applyYear as JZNF,kilometersPower as BGLHDL,signleMileage as CLCMYCDNGXSLC,mileageTime as CLYCCMDSXSJ,carCompany as CLYXDW,monitorUnit as JKPDXXDW,charge as LJCDL,hydrogenKg as LJJQL,airKg as LJJQL_G,airL as LJJQL_L,oil as LJJYL,mileage as LJXSLC,runtimeDay as PJDRXYSJ,monitor as SFAZJKZZ,mileageMonth as YJXSLC,mileagePower as ZDCDGL,approvalPerson1 as APP_NAME_1_A,'' as APP_NAME_1_B,approvalTime1 as APP_TIME_1_A,'' as APP_TIME_1_B,'' as APP_RESULT_1_A,'' as APP_RESULT_1_B,approvalPerson2 as APP_NAME_2,approvalTime2 as APP_TIME_2,approvaldes2 as APP_RESULT_2,approvalPerson3 as APP_NAME_3,approvalTime3 as APP_TIME_3,approvaldes3 as APP_RESULT_3,approvalAccount as APP_MONEY,approvalFlag as APP_STATUS,entname as CLSCQY,recommendAccount as BTBZ,approvalFlag as BDJG,'' as BDJG_GG,dict as DQ,ID as GUID,1 PC from APPLY_IMP_DETAIL_201602";
                sqlCount = String.Format("select count(*) from ({0}) where 1=1 {1}", sqlStr, this.queryParam());
            }
            else
            {
                sqlCount = String.Format("select count(*) from DB_INFOMATION where 1=1 {0}", this.queryParam());
            }
            return Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, sqlCount));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            string linqStr = string.Empty;
            if (this.PC.Text == "01")
            {
                string sqlStr = "select VIN as VIN,'' as CLXZ,CARTYPE as CLZL,'' as GCSF,BUYCITY as GCCS,CARPURPOSE as CLYT,CARMODEL as CLXH,BATCHCODE as GGPC,CARCODE as CLPZ,'' as EKGZ,BUYPRICE as GMJG,subsidyStandard as SQBZBZ,TICKETCODE as FPHM,TICKETTIME as FPSJ,VEHICLELICENSE as XSZSJ,'' as FPTP,'' as XSZTP,'' as FPTP_PICTURE,'' as XSZTP_PICTURE,capacitanceBoxModel as CJDRXX_CXXH,capacitanceCapacity as CJDRXX_DRZRL,capacitanceProduct as CJDRXX_DRZSCQY,batterySingleProduct as CJDRXX_DTSCQY,batterySingleModel as CJDRXX_DTXH,capacitanceSysPrice as CJDRXX_XTJG,capacitanceLife as CJDRXX_ZBNX,SUPPERCAPACITANCE as CLSFYCJDR,'' as CLSFYQDDJ2,FUELCELL as CLSFYRLDC,batteryProduct as DCDTXX_SCQY,batteryModel as DCDTXX_XH,batteryBoxProduct as DCZXX_SCQY,batteryBoxModel as DCZXX_XH,bateryBoxSysPrice as DCZXX_XTJG,batteryBoxRepair as DCZXX_ZBNX,batterysCapacity as DCZXX_ZRL,motorpower as QDDJXX_EDGL_1,'' as QDDJXX_EDGL_2,motorProduct as QDDJXX_SCQY_1,'' as QDDJXX_SCQY_2,motorModel as QDDJXX_XH_1,'' as QDDJXX_XH_2,motorSysPrice as QDDJXX_XTJG_1,'' as QDDJXX_XTJG_2,fuelcellpower as RLDCXX_EDGL,fuelcellSysPrice as RLDCXX_GMJG,fuelcellProduct as RLDCXX_SCQY,fuelcellmodel as RLDCXX_XH,fuelcelllife as RLDCXX_ZBNX,applyYear as JZNF,kilometersPower as BGLHDL,signleMileage as CLCMYCDNGXSLC,mileageTime as CLYCCMDSXSJ,carCompany as CLYXDW,monitorUnit as JKPDXXDW,charge as LJCDL,hydrogenKg as LJJQL,airKg as LJJQL_G,airL as LJJQL_L,oil as LJJYL,mileage as LJXSLC,runtimeDay as PJDRXYSJ,monitor as SFAZJKZZ,mileageMonth as YJXSLC,mileagePower as ZDCDGL,approvalPerson1 as APP_NAME_1_A,'' as APP_NAME_1_B,approvalTime1 as APP_TIME_1_A,'' as APP_TIME_1_B,'' as APP_RESULT_1_A,'' as APP_RESULT_1_B,approvalPerson2 as APP_NAME_2,approvalTime2 as APP_TIME_2,approvaldes2 as APP_RESULT_2,approvalPerson3 as APP_NAME_3,approvalTime3 as APP_TIME_3,approvaldes3 as APP_RESULT_3,approvalAccount as APP_MONEY,approvalFlag as APP_STATUS,entname as CLSCQY,recommendAccount as BTBZ,approvalFlag as BDJG,'' as BDJG_GG,dict as DQ,ID as GUID,1 PC from APPLY_IMP_DETAIL_201601";
                linqStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM ({0}) where 1=1 {1} order by VIN desc) A WHERE ROWNUM <= {2}) WHERE RN > {3}", sqlStr, this.queryParam(), pageSize * pageNum, pageSize * (pageNum - 1));
            }
            else if (this.PC.Text == "02")
            {
                string sqlStr = "select VIN as VIN,'' as CLXZ,CARTYPE as CLZL,'' as GCSF,BUYCITY as GCCS,CARPURPOSE as CLYT,CARMODEL as CLXH,BATCHCODE as GGPC,CARCODE as CLPZ,'' as EKGZ,BUYPRICE as GMJG,subsidyStandard as SQBZBZ,TICKETCODE as FPHM,TICKETTIME as FPSJ,VEHICLELICENSE as XSZSJ,'' as FPTP,'' as XSZTP,'' as FPTP_PICTURE,'' as XSZTP_PICTURE,capacitanceBoxModel as CJDRXX_CXXH,capacitanceCapacity as CJDRXX_DRZRL,capacitanceProduct as CJDRXX_DRZSCQY,batterySingleProduct as CJDRXX_DTSCQY,batterySingleModel as CJDRXX_DTXH,capacitanceSysPrice as CJDRXX_XTJG,capacitanceLife as CJDRXX_ZBNX,SUPPERCAPACITANCE as CLSFYCJDR,'' as CLSFYQDDJ2,FUELCELL as CLSFYRLDC,batteryProduct as DCDTXX_SCQY,batteryModel as DCDTXX_XH,batteryBoxProduct as DCZXX_SCQY,batteryBoxModel as DCZXX_XH,bateryBoxSysPrice as DCZXX_XTJG,batteryBoxRepair as DCZXX_ZBNX,batterysCapacity as DCZXX_ZRL,motorpower as QDDJXX_EDGL_1,'' as QDDJXX_EDGL_2,motorProduct as QDDJXX_SCQY_1,'' as QDDJXX_SCQY_2,motorModel as QDDJXX_XH_1,'' as QDDJXX_XH_2,motorSysPrice as QDDJXX_XTJG_1,'' as QDDJXX_XTJG_2,fuelcellpower as RLDCXX_EDGL,fuelcellSysPrice as RLDCXX_GMJG,fuelcellProduct as RLDCXX_SCQY,fuelcellmodel as RLDCXX_XH,fuelcelllife as RLDCXX_ZBNX,applyYear as JZNF,kilometersPower as BGLHDL,signleMileage as CLCMYCDNGXSLC,mileageTime as CLYCCMDSXSJ,carCompany as CLYXDW,monitorUnit as JKPDXXDW,charge as LJCDL,hydrogenKg as LJJQL,airKg as LJJQL_G,airL as LJJQL_L,oil as LJJYL,mileage as LJXSLC,runtimeDay as PJDRXYSJ,monitor as SFAZJKZZ,mileageMonth as YJXSLC,mileagePower as ZDCDGL,approvalPerson1 as APP_NAME_1_A,'' as APP_NAME_1_B,approvalTime1 as APP_TIME_1_A,'' as APP_TIME_1_B,'' as APP_RESULT_1_A,'' as APP_RESULT_1_B,approvalPerson2 as APP_NAME_2,approvalTime2 as APP_TIME_2,approvaldes2 as APP_RESULT_2,approvalPerson3 as APP_NAME_3,approvalTime3 as APP_TIME_3,approvaldes3 as APP_RESULT_3,approvalAccount as APP_MONEY,approvalFlag as APP_STATUS,entname as CLSCQY,recommendAccount as BTBZ,approvalFlag as BDJG,'' as BDJG_GG,dict as DQ,ID as GUID,1 PC from APPLY_IMP_DETAIL_201602";
                linqStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM ({0}) where 1=1 {1} order by VIN desc) A WHERE ROWNUM <= {2}) WHERE RN > {3}", sqlStr, this.queryParam(), pageSize * pageNum, pageSize * (pageNum - 1));
            }
            else
            {
                string sqlStr = string.Format("SELECT * FROM (SELECT A.*, ROWNUM RN FROM (SELECT * FROM DB_INFOMATION where 1=1 {0} order by VIN desc) A WHERE ROWNUM <= {1}) WHERE RN > {2}", this.queryParam(), pageSize * pageNum, pageSize * (pageNum - 1));
                linqStr = string.Format("select i.*,n.GGPC_GG,n.DCDTXX_XH_GG,n.DCDTXX_SCQY_GG,n.DCZXX_XH_GG,n.DCZXX_ZRL_GG,n.DCZXX_SCQY_GG,n.QDDJXX_XH_1_GG,n.QDDJXX_EDGL_1_GG,n.QDDJXX_SCQY_1_GG,n.RLDCXX_XH_GG,n.RLDCXX_EDGL_GG,n.RLDCXX_SCQY_GG from ({0}) i left outer join DB_DIFFERENT n on i.VIN=n.VIN", sqlStr);
            }
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, linqStr, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //获取全部数据
        private DataTable queryAll()
        {
            string linqStr = string.Empty;
            if (this.PC.Text == "01")
            {
                linqStr = "select VIN as VIN,'' as CLXZ,CARTYPE as CLZL,'' as GCSF,BUYCITY as GCCS,CARPURPOSE as CLYT,CARMODEL as CLXH,BATCHCODE as GGPC,CARCODE as CLPZ,'' as EKGZ,BUYPRICE as GMJG,subsidyStandard as SQBZBZ,TICKETCODE as FPHM,TICKETTIME as FPSJ,VEHICLELICENSE as XSZSJ,'' as FPTP,'' as XSZTP,'' as FPTP_PICTURE,'' as XSZTP_PICTURE,capacitanceBoxModel as CJDRXX_CXXH,capacitanceCapacity as CJDRXX_DRZRL,capacitanceProduct as CJDRXX_DRZSCQY,batterySingleProduct as CJDRXX_DTSCQY,batterySingleModel as CJDRXX_DTXH,capacitanceSysPrice as CJDRXX_XTJG,capacitanceLife as CJDRXX_ZBNX,SUPPERCAPACITANCE as CLSFYCJDR,'' as CLSFYQDDJ2,FUELCELL as CLSFYRLDC,batteryProduct as DCDTXX_SCQY,batteryModel as DCDTXX_XH,batteryBoxProduct as DCZXX_SCQY,batteryBoxModel as DCZXX_XH,bateryBoxSysPrice as DCZXX_XTJG,batteryBoxRepair as DCZXX_ZBNX,batterysCapacity as DCZXX_ZRL,motorpower as QDDJXX_EDGL_1,'' as QDDJXX_EDGL_2,motorProduct as QDDJXX_SCQY_1,'' as QDDJXX_SCQY_2,motorModel as QDDJXX_XH_1,'' as QDDJXX_XH_2,motorSysPrice as QDDJXX_XTJG_1,'' as QDDJXX_XTJG_2,fuelcellpower as RLDCXX_EDGL,fuelcellSysPrice as RLDCXX_GMJG,fuelcellProduct as RLDCXX_SCQY,fuelcellmodel as RLDCXX_XH,fuelcelllife as RLDCXX_ZBNX,applyYear as JZNF,kilometersPower as BGLHDL,signleMileage as CLCMYCDNGXSLC,mileageTime as CLYCCMDSXSJ,carCompany as CLYXDW,monitorUnit as JKPDXXDW,charge as LJCDL,hydrogenKg as LJJQL,airKg as LJJQL_G,airL as LJJQL_L,oil as LJJYL,mileage as LJXSLC,runtimeDay as PJDRXYSJ,monitor as SFAZJKZZ,mileageMonth as YJXSLC,mileagePower as ZDCDGL,approvalPerson1 as APP_NAME_1_A,'' as APP_NAME_1_B,approvalTime1 as APP_TIME_1_A,'' as APP_TIME_1_B,'' as APP_RESULT_1_A,'' as APP_RESULT_1_B,approvalPerson2 as APP_NAME_2,approvalTime2 as APP_TIME_2,approvaldes2 as APP_RESULT_2,approvalPerson3 as APP_NAME_3,approvalTime3 as APP_TIME_3,approvaldes3 as APP_RESULT_3,approvalAccount as APP_MONEY,approvalFlag as APP_STATUS,entname as CLSCQY,recommendAccount as BTBZ,approvalFlag as BDJG,'' as BDJG_GG,dict as DQ,ID as GUID,1 PC from APPLY_IMP_DETAIL_201601";

            }
            else if (this.PC.Text == "02")
            {
                linqStr = "select VIN as VIN,'' as CLXZ,CARTYPE as CLZL,'' as GCSF,BUYCITY as GCCS,CARPURPOSE as CLYT,CARMODEL as CLXH,BATCHCODE as GGPC,CARCODE as CLPZ,'' as EKGZ,BUYPRICE as GMJG,subsidyStandard as SQBZBZ,TICKETCODE as FPHM,TICKETTIME as FPSJ,VEHICLELICENSE as XSZSJ,'' as FPTP,'' as XSZTP,'' as FPTP_PICTURE,'' as XSZTP_PICTURE,capacitanceBoxModel as CJDRXX_CXXH,capacitanceCapacity as CJDRXX_DRZRL,capacitanceProduct as CJDRXX_DRZSCQY,batterySingleProduct as CJDRXX_DTSCQY,batterySingleModel as CJDRXX_DTXH,capacitanceSysPrice as CJDRXX_XTJG,capacitanceLife as CJDRXX_ZBNX,SUPPERCAPACITANCE as CLSFYCJDR,'' as CLSFYQDDJ2,FUELCELL as CLSFYRLDC,batteryProduct as DCDTXX_SCQY,batteryModel as DCDTXX_XH,batteryBoxProduct as DCZXX_SCQY,batteryBoxModel as DCZXX_XH,bateryBoxSysPrice as DCZXX_XTJG,batteryBoxRepair as DCZXX_ZBNX,batterysCapacity as DCZXX_ZRL,motorpower as QDDJXX_EDGL_1,'' as QDDJXX_EDGL_2,motorProduct as QDDJXX_SCQY_1,'' as QDDJXX_SCQY_2,motorModel as QDDJXX_XH_1,'' as QDDJXX_XH_2,motorSysPrice as QDDJXX_XTJG_1,'' as QDDJXX_XTJG_2,fuelcellpower as RLDCXX_EDGL,fuelcellSysPrice as RLDCXX_GMJG,fuelcellProduct as RLDCXX_SCQY,fuelcellmodel as RLDCXX_XH,fuelcelllife as RLDCXX_ZBNX,applyYear as JZNF,kilometersPower as BGLHDL,signleMileage as CLCMYCDNGXSLC,mileageTime as CLYCCMDSXSJ,carCompany as CLYXDW,monitorUnit as JKPDXXDW,charge as LJCDL,hydrogenKg as LJJQL,airKg as LJJQL_G,airL as LJJQL_L,oil as LJJYL,mileage as LJXSLC,runtimeDay as PJDRXYSJ,monitor as SFAZJKZZ,mileageMonth as YJXSLC,mileagePower as ZDCDGL,approvalPerson1 as APP_NAME_1_A,'' as APP_NAME_1_B,approvalTime1 as APP_TIME_1_A,'' as APP_TIME_1_B,'' as APP_RESULT_1_A,'' as APP_RESULT_1_B,approvalPerson2 as APP_NAME_2,approvalTime2 as APP_TIME_2,approvaldes2 as APP_RESULT_2,approvalPerson3 as APP_NAME_3,approvalTime3 as APP_TIME_3,approvaldes3 as APP_RESULT_3,approvalAccount as APP_MONEY,approvalFlag as APP_STATUS,entname as CLSCQY,recommendAccount as BTBZ,approvalFlag as BDJG,'' as BDJG_GG,dict as DQ,ID as GUID,1 PC from APPLY_IMP_DETAIL_201602";
            }
            else
            {
                string sqlStr = String.Format("select * from DB_INFOMATION where 1=1 {0}", this.queryParam());
                linqStr = string.Format("select i.*,n.GGPC_GG,n.DCDTXX_XH_GG,n.DCDTXX_SCQY_GG,n.DCZXX_XH_GG,n.DCZXX_ZRL_GG,n.DCZXX_SCQY_GG,n.QDDJXX_XH_1_GG,n.QDDJXX_EDGL_1_GG,n.QDDJXX_SCQY_1_GG,n.RLDCXX_XH_GG,n.RLDCXX_EDGL_GG,n.RLDCXX_SCQY_GG from ({0}) i left outer join DB_DIFFERENT n on i.VIN=n.VIN", sqlStr);
            }
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, linqStr, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            if (!string.IsNullOrEmpty(CLSCQY.Text))
            {
                sqlStr.AppendFormat(" AND (CLSCQY like '%{0}%')", CLSCQY.Text);
            }
            if (!string.IsNullOrEmpty(CLXZ.Text) && this.PC.Text != "01" && this.PC.Text != "02")
            {
                sqlStr.AppendFormat(" AND (CLXZ = '{0}')", CLXZ.Text);
            }
            if (!string.IsNullOrEmpty(APP_STATUS.Text))
            {
                string[] steArray = APP_STATUS.Text.Split(',');
                string strAppStatus = string.Empty;
                if (this.PC.Text == "01" || this.PC.Text == "02")
                {
                    foreach (string strAppS in steArray)
                    {
                        strAppStatus += Convert.ToInt32(Enum.Parse(typeof(hAPP_STATUS), strAppS.Trim())).ToString() + ",";
                    }
                }
                else
                {
                    foreach (string strAppS in steArray)
                    {
                        strAppStatus += Convert.ToInt32(Enum.Parse(typeof(eAPP_STATUS), strAppS.Trim())).ToString() + ",";
                    }
                }
                sqlStr.AppendFormat(" AND (APP_STATUS in ({0}))", strAppStatus.Substring(0, strAppStatus.Length-1));
            }
            if (!string.IsNullOrEmpty(DQ.Text))
            {
                sqlStr.AppendFormat(" AND DQ = '{0}'  ", DQ.Text);
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
            if (!string.IsNullOrEmpty(VIN.Text))
            {
                sqlStr.AppendFormat(" AND (VIN like '%{0}%')", VIN.Text);
            }
            if (!string.IsNullOrEmpty(BDJG.Text) && this.PC.Text != "01" && this.PC.Text != "02")
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
                    int bdjg = Convert.ToInt32(Enum.Parse(typeof(eBDJG), BDJG.Text));
                    sqlStr.AppendFormat(" AND (BDJG = 20 OR BDJG = 21 OR BDJG = 22)", bdjg.ToString());
                }
            }
            if (!string.IsNullOrEmpty(GCCS.Text))
            {
                sqlStr.AppendFormat(" AND GCCS like '%{0}%'  ", GCCS.Text);
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
            if (!string.IsNullOrEmpty(PC.Text) && this.PC.Text != "01" && this.PC.Text != "02")
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
            if (dr["BDJG_GG"].ToString().Equals("3") || (Convert.ToDecimal(dr["LJXSLC"].ToString()) < 30000) || string.IsNullOrEmpty(dr["BTBZ"].ToString()) || dr["VIN"].ToString().Length != 17 || dr["BDJG"].ToString().Equals("20") || dr["BDJG"].ToString().Equals("21") || dr["BDJG"].ToString().Equals("22"))
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
                    case "5":
                        e.DisplayText = "一审驳回";
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
                    case "15":
                        e.DisplayText = "二审驳回";
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
                    case "35":
                        e.DisplayText = "三审驳回";
                        break;
                    case "40":
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
            ColumnView cv = (ColumnView)this.gcDataInfo.FocusedView;
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
    }
}