using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Catarc.Adc.NewEnergyAccountSys.ControlUtils;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using DevExpress.XtraSplashScreen;
using Catarc.Adc.NewEnergyAccountSys.DevForm;
using Catarc.Adc.NewEnergyAccountSys.FormUtils.DataUtils;
using Catarc.Adc.NewEnergyAccountSys.PopForm;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using Catarc.Adc.NewEnergyAccountSys.FormUtils;
using System.IO;
using Catarc.Adc.NewEnergyAccountSys.Common;
using DevExpress.XtraGrid.Views.Base;
using System.Linq;
using Catarc.Adc.NewEnergyAccountSys.DataUtils;
using DevExpress.XtraPrinting;
using System.Threading;
using HFSoft.Component.Windows;
using System.Diagnostics;
using Catarc.Adc.NewEnergyAccountSys.LogUtils;


namespace Catarc.Adc.NewEnergyAccountSys.Form_Data
{
    public partial class ImportNewInfoForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private DataTable dtGG = new DataTable();
        private DataTable dtXT = new DataTable();

        public ImportNewInfoForm()
        {
            InitializeComponent();
        }

        //窗体加载成功事件
        private void ImportNewInfoForm_Load(object sender, EventArgs e)
        {
            this.CLXZ.Properties.Items.AddRange(ComboBoxEditHelper.getOptionsByName("CLXZ"));
            this.CLZL.Properties.Items.AddRange(ComboBoxEditHelper.getOptionsByName("CLZL"));
            this.splitContainerControl1.SplitterPosition = 100;
            this.btnExtend.Text = "展开高级查询  ↓";
            //按钮显示
            string Item = this.Text; ;
            List<string> ButtonModel = Authority.ReadMenusXmlData("AuthorityUrl").Where(c => Item.ToString().Contains(c.ParentID.ToString())).Select(c => c.MenuName).ToList<string>();
            foreach (BarItemLink link in ribbonPageGroup1.ItemLinks)
            {
                object j = link.Item.Tag;
                if (ButtonModel.Contains(link.Caption))
                {
                    link.Item.Visibility = BarItemVisibility.Always;
                }
                else
                {
                    link.Item.Visibility = BarItemVisibility.Never;
                }
            }
        }

        //根据车辆种类修改车辆用途
        private void CLZL_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.CLYT.Text = string.Empty;
            this.CLYT.Properties.Items.Clear();
            if (this.CLZL.SelectedItem.ToString() == "插电式混合动力客车" || this.CLZL.SelectedItem.ToString() == "纯电动客车" || this.CLZL.SelectedItem.ToString() == "燃料电池客车")
            {
                this.CLYT.Properties.Items.Add((object)"公交");
                this.CLYT.Properties.Items.Add((object)"通勤");
                this.CLYT.Properties.Items.Add((object)"旅游");
                this.CLYT.Properties.Items.Add((object)"公路");
            }
            else if (this.CLZL.SelectedItem.ToString() == "插电式混合动力乘用车" || this.CLZL.SelectedItem.ToString() == "纯电动乘用车" || this.CLZL.SelectedItem.ToString() == "燃料电池乘用车")
            {
                this.CLYT.Properties.Items.Add((object)"公务");
                this.CLYT.Properties.Items.Add((object)"出租");
                this.CLYT.Properties.Items.Add((object)"租赁");
                this.CLYT.Properties.Items.Add((object)"私人");
            }
            else
            {
                if (!(CLZL.SelectedItem.ToString() == "纯电动特种车") && !(CLZL.SelectedItem.ToString() == "燃料电池货车"))
                    return;
                this.CLYT.Properties.Items.Add((object)"邮政");
                this.CLYT.Properties.Items.Add((object)"物流");
                this.CLYT.Properties.Items.Add((object)"环卫");
                this.CLYT.Properties.Items.Add((object)"工程");
            }
        }

        //窗体双击事件,VIN号的顺序取决于所查询的表
        private void gcDataInfo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)this.gcDataInfo.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null) return;
            string vin = (string)dr.Row["VIN"];
            Dictionary<string, string> mapData = QueryHelper.queryInfomationByVin(vin);
            if (mapData.Count < 1)
            {
                MessageBox.Show("未查到该vin对应数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string status = (string)dr.Row["STATUS"];
            Dictionary<string, string> mapRightData = new Dictionary<string, string>();
            if(status.Equals("1"))
                mapRightData = QueryHelper.queryRightByVin(vin);
            using (var dlg = new SingleInfoForm(mapData, mapRightData, true, false) { Text = "单条修改" })
            {
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                    this.refrashCurrentPage();
            }
        }

        //单条增加
        private void barBtnSingle_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var singleInfoForm = new SingleInfoForm() { Text = "单条增加" })
            {
                singleInfoForm.ShowDialog();
                if (singleInfoForm.DialogResult == DialogResult.OK)
                    this.SearchLocal(1);
            }
        }

        //批量导入(旧)
        private void barBtnImportOld_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.folderBrowserDialog.Description = "批量导入新能源填报数据";
            string msg = string.Empty;
            if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                LoadingHandler.Show(this, args =>
                {
                    var utils = new ImportOldInfoUtils();
                    msg = utils.ImportOldTemplate(folderBrowserDialog.SelectedPath);
                });
            }
            catch (System.Exception ex)
            {
                msg += String.Format("异常,操作出现错误：{0}", ex.Message);
            }
            using (var mf = new MessageForm(msg) { Text = "导入操作结果" })
            {
                mf.ShowDialog();
                if (mf.DialogResult == DialogResult.Cancel)
                    this.SearchLocal(1);
            }
        }

        //批量导入（新）
        private void barBtnImportNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.folderBrowserDialog.Description = "批量导入新能源填报数据";
            string msg = string.Empty;
            if (folderBrowserDialog.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                LoadingHandler.Show(this, args =>
                {
                    var utils = new ImportNewInfoUtils();
                    msg = utils.ImportNewTemplate(folderBrowserDialog.SelectedPath);
                });
            }
            catch (System.Exception ex)
            {
                msg += String.Format("异常,操作出现错误：{0}", ex.Message);
            }
            using (var mf = new MessageForm(msg) { Text = "导入操作结果" })
            {
                mf.ShowDialog();
                if (mf.DialogResult == DialogResult.Cancel)
                    this.SearchLocal(1);
            }
        }

        //复制新增
        private void barBtnCopy_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dtExport = (DataTable)this.gcDataInfo.DataSource;
            if (dtExport == null || dtExport.Rows.Count < 1)
            {
                MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GridControlHelper.SelectedItems(this.gvDataInfo).Rows.Count != 1)
            {
                MessageBox.Show("当前只能操作一条记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string vin = (string)(GridControlHelper.SelectedItems(this.gvDataInfo).Rows[0]["VIN"]);
            Dictionary<string, string> mapData = QueryHelper.queryInfomationByVin(vin);
            if (mapData.Count <1)
            {
                MessageBox.Show("未查到该vin对应数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Dictionary<string, string> mapRightData = new Dictionary<string, string>();
            using (var dlg = new SingleInfoForm(mapData, mapRightData, false, true) { Text = "复制新增" })
            {
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                    this.refrashCurrentPage();
            }
           
        }

        //单条修改
        private void barBtnUpdate_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dtExport = (DataTable)this.gcDataInfo.DataSource;
            string msg = string.Empty;
            if (dtExport == null || dtExport.Rows.Count < 1)
            {
                MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GridControlHelper.SelectedItems(this.gvDataInfo).Rows.Count != 1)
            {
                MessageBox.Show("当前只能操作一条记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string vin = (string)(GridControlHelper.SelectedItems(this.gvDataInfo).Rows[0].ItemArray[0]);
            Dictionary<string, string> mapData = QueryHelper.queryInfomationByVin(vin);
            if (mapData.Count <1)
            {
                MessageBox.Show("未查到该vin对应数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string status = (string)(GridControlHelper.SelectedItems(this.gvDataInfo).Rows[0]["STATUS"]);
            Dictionary<string, string> mapRightData = new Dictionary<string, string>();
            if (status.Equals("1"))
                mapRightData = QueryHelper.queryRightByVin(vin);
            using (var dlg = new SingleInfoForm(mapData, mapRightData, true, false) { Text = "单条修改" })
            {
                dlg.ShowDialog();
                if (dlg.DialogResult == DialogResult.OK)
                    this.refrashCurrentPage();
            }
        }

        //记录删除
        private void barBtnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {
            var dtExport = (DataTable)this.gcDataInfo.DataSource;
            string msg = string.Empty;
            if (dtExport == null || dtExport.Rows.Count < 1)
            {
                MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (GridControlHelper.SelectedItems(this.gvDataInfo).Rows.Count < 1)
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
                var utils = new ImportNewInfoUtils();
                var selDataTable = GridControlHelper.SelectedItems(this.gvDataInfo);
                msg = utils.deleteDataInfo(selDataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                var dtExport = (DataTable)this.gcDataInfo.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    var options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    this.gcDataInfo.ExportToXls(saveFileDialog.FileName, options);
                    if (MessageBox.Show("操作成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void updateData() 
        {
            Thread.Sleep(15000);
        }

        //导出摸板
        private void barBtnExportFloder_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var dtExport = (DataTable)this.gcDataInfo.DataSource;
                string msg = string.Empty;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FolderDialog openFolder = new FolderDialog();
                if (openFolder.DisplayDialog() == DialogResult.OK)
                {
                    string vin = string.Empty;
                    var selDataTable = GridControlHelper.SelectedItems(this.gvDataInfo);
                    var selVIN = selDataTable.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                    vin = string.Join("','", selVIN);
                    vin = string.Format("'{0}'", vin);
                    if (vin == "''")
                    {
                        vin = string.Empty;
                    }
                    if (string.IsNullOrEmpty(Settings.Default.ClearYear) || string.IsNullOrEmpty(Settings.Default.Vehicle_MFCS))
                    {
                        MessageBox.Show("请先设置车辆生存企业名称和清算年份！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    DialogResult dr = MessageBox.Show(String.Format("确定要导出【{0}】，清算年份为【{1}】的数据吗？", Settings.Default.Vehicle_MFCS, Settings.Default.ClearYear), "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                     if (dr == DialogResult.OK)
                     {
                         
                         try
                         {
                             LoadingHandler.Show(this, args =>
                             {
                                 var exutils = new ExportNewInfoUtils();
                                 msg = exutils.ExportNewTemplate(openFolder.Path, vin);
                             });
                             if (String.IsNullOrEmpty(msg))
                             {
                                 MessageBox.Show("导出完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                             }
                             else
                             {
                                 using (var mf = new MessageForm(msg) { Text = "校验结果" })
                                 {
                                     mf.ShowDialog();
                                 }
                             }

                         }
                         catch (System.Exception ex)
                         {
                             MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                         } 
                     }
                }
            }
            catch (System.Exception ex)
            {
            	
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
            this.CLPZ.Text = string.Empty;
            this.VIN.Text = string.Empty;
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

        //展开高级查询
        private void btnExtend_Click(object sender, EventArgs e)
        {
            if (this.btnExtend.Text.Equals("展开高级查询  ↓"))
            {
                this.splitContainerControl1.SplitterPosition = 250;
                this.btnExtend.Text = "收起高级查询  ↑";
            }
            else
            {
                this.splitContainerControl1.SplitterPosition = 100;
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
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                int dataCount = queryCount();
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    var dtQuery = queryByPage(pageNum);
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    LogManager.Log("TimeSpend", "CompareTime", String.Format("数据库分页取数耗时========：{0}时{1}分{2}秒", ts.Hours, ts.Minutes, ts.Seconds));
                    var dt = DataCompareHelper.CompareDataTable_DT(dtQuery);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    this.gcDataInfo.DataSource = dt;
                    this.dtXT = dt.Copy();
                    this.dtGG = AccessHelper.ExecuteDataSet(AccessHelper.conn, "select * from ANNOUNCE_MAX_ENTITY").Tables[0];
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
                    stopWatch.Stop();
                    TimeSpan ts = stopWatch.Elapsed;
                    LogManager.Log("TimeSpend", "CompareTime", String.Format("数据库全部取数耗时========：{0}时{1}分{2}秒", ts.Hours, ts.Minutes, ts.Seconds));
                    var dt = DataCompareHelper.CompareDataTable_DT(dtQuery);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    this.gcDataInfo.DataSource = dt;
                    this.dtXT = dt.Copy();
                    this.dtGG = AccessHelper.ExecuteDataSet(AccessHelper.conn, "select * from ANNOUNCE_MAX_ENTITY").Tables[0];
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
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //获取总数
        private int queryCount()
        {
            string sqlCount = String.Format("select count(*) from INFOMATION_ENTITIES where JZNF='{0}' {1}", Settings.Default.ClearYear, this.queryParam());
            return Convert.ToInt32(AccessHelper.GetSingle(AccessHelper.conn, sqlCount));
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            string sqlWhere = this.queryParam();
            string sqlVins = string.Format(@"select top {0} * from INFOMATION_ENTITIES where JZNF='{1}' {2} order by CLYXDW desc,VIN desc", (pageSize * pageNum), Settings.Default.ClearYear, sqlWhere);
            string sqlStr = string.Format(@"select top {0} * from ({1}) order by CLYXDW asc,VIN asc", pageSize, sqlVins);
            string sqlOrder = string.Format(@"select * from ({0}) order by CLYXDW desc,VIN desc", sqlStr);
            var ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlOrder, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //获取全部数据
        private DataTable queryAll()
        {
            string sqlAll = String.Format("select * from INFOMATION_ENTITIES where JZNF='{0}' {1} order by CLYXDW desc,VIN desc", Settings.Default.ClearYear, this.queryParam());
            var ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlAll, null);
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
            if (!string.IsNullOrEmpty(CLPZ.Text))
            {
                sqlStr.AppendFormat(" AND (CLPZ like '%{0}%')", CLPZ.Text);
            }
            if (!string.IsNullOrEmpty(VIN.Text))
            {
                sqlStr.AppendFormat(" AND (VIN like '%{0}%')", VIN.Text);
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

        //修改单元格样式
        private void gvDataInfo_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            switch (e.Column.FieldName)
            {
                case "STATUS":
                    {
                        if (e.CellValue != null)
                        {
                            if (e.CellValue.ToString() == "0")
                            {
                                e.Appearance.BackColor = System.Drawing.Color.LightBlue;
                            }
                            if (e.CellValue.ToString() == "1")
                            {
                                e.Appearance.BackColor = System.Drawing.Color.Pink;
                            }
                        }
                        break;
                    }
                case "DCDTXX_XH":
                    {
                        if (e.CellValue != null)
                        {
                            int rowNum = e.RowHandle;
                            if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                            {
                                string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MODEL_SINGLE")).FirstOrDefault().Trim();
                                if (!e.CellValue.ToString().Equals(cellValue))
                                {
                                    e.Appearance.BackColor = System.Drawing.Color.Pink;
                                }
                            }
                        }
                        break;
                    }
            case "DCDTXX_SCQY":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MFRS_SINGLE")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(cellValue.Replace('(', '（').Replace(')', '）').Trim()))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "DCZXX_XH":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MODEL_WHOLE")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Equals(cellValue))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "DCZXX_ZRL":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("CAPACITY_BAT")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Equals(cellValue))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "DCZXX_SCQY":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MFRS_BAT")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(cellValue.Replace('(', '（').Replace(')', '）').Trim()))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "QDDJXX_XH_1":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MODEL_DRIVE")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Equals(cellValue))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "QDDJXX_EDGL_1":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("RATEPOW_DRIVE")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Equals(cellValue))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "QDDJXX_SCQY_1":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MFRS_DRIVE")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(cellValue.Replace('(', '（').Replace(')', '）').Trim()))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "RLDCXX_XH":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MDEL_FUEL")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Equals(cellValue))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "RLDCXX_EDGL":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("RATEPOW_FUEL")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Equals(cellValue))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            case "RLDCXX_SCQY":
            {
                if (e.CellValue != null)
                {
                    int rowNum = e.RowHandle;
                    if (this.dtXT.Rows[e.RowHandle]["STATUS"].Equals("1"))
                    {
                        string cellValue = this.dtGG.AsEnumerable().Where(d => d.Field<string>("MODEL_VEHICLE").Equals(this.dtXT.Rows[e.RowHandle]["CLXH"].ToString().Trim())).Select(d => d.Field<string>("MFRS_FUEL")).FirstOrDefault().Trim();
                        if (!e.CellValue.ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(cellValue.Replace('(', '（').Replace(')', '）').Trim()))
                        {
                            e.Appearance.BackColor = System.Drawing.Color.Pink;
                        }
                    }
                }
                break;
            }
            }
        }

        //修改列显示文本
        private void gvDataInfo_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STATUS")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "-1":
                        e.DisplayText = "未找到";
                        break;
                    case "0":
                        e.DisplayText = "一致";
                        break;
                    case "1":
                        e.DisplayText = "不一致";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
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