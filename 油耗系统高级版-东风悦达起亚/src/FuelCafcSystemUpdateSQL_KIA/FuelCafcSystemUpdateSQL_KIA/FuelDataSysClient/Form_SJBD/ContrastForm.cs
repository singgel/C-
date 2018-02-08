using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.OleDb;
using System.Threading;
using Common;
using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrintingLinks;
using System.Web.Services.Protocols;
using System.Net;
using FuelDataSysClient.FuelCafc;
using FuelDataSysClient.Tool;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Form_SJSB;

namespace FuelDataSysClient.Form_SJBD
{
    public partial class ContrastForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        MitsUtils miutils = new MitsUtils();

        public ContrastForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        private void ContrastForm_Load(object sender, EventArgs e)
        {
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView2.OptionsBehavior.Editable = false;
        }

        //获取数据库中数据
        private DataTable GetLocalALL(string tableName)
        {
            using (DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format("select * from {0} where to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >=to_date('{1}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <=to_date('{2}','yyyy-mm-dd hh24:mi:ss') ", tableName, dtStartTime.Text, dtEndTime.Text), null))
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0];
                }
            }
            return null;
        }

        //备份油耗数据
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel|*.xls;*.xlsx" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                    MitsUtils miutils = new MitsUtils();
                    this.gridControl2.DataSource = null;
                    this.groupBox2.Text = "Excel油耗数据";
                    this.gridView2.Columns.Clear();
                    DataSet dsRead = new DataSet();
                    switch (radioGroup1.SelectedIndex)
                    {
                        case 0:
                            dsRead = miutils.ReadExcel(ofd.FileName, MitsUtils.CTNY);
                            break;
                        case 1:
                            dsRead = miutils.ReadExcel(ofd.FileName, MitsUtils.FCDSHHDL);
                            break;
                        case 2:
                            dsRead = miutils.ReadExcel(ofd.FileName, MitsUtils.FCDSHHDL);
                            break;
                        case 3:
                            dsRead = miutils.ReadExcel(ofd.FileName, MitsUtils.FCDSHHDL);
                            break;
                        case 4:
                            dsRead = miutils.ReadExcel(ofd.FileName, MitsUtils.FCDSHHDL);
                            break;
                    }

                    if (dsRead != null && dsRead.Tables[0].Rows.Count > 0)
                    {
                        //验证单元格格式
                        for (int i = 0; i < dsRead.Tables[0].Columns.Count; i++)
                        {
                            if (dsRead.Tables[0].Columns[i].ColumnName.Equals("车辆制造日期"))
                            {
                                for (int j = 0; j < dsRead.Tables[0].Rows.Count; j++)
                                {
                                    if (dsRead.Tables[0].Rows[j][i].GetType() != typeof(System.DateTime))
                                    {
                                        MessageBox.Show(String.Format("【{1}】列中第【{0}】行的单元格格式不正确，应为日期格式!", j + 2, dsRead.Tables[0].Columns[i].ColumnName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                }
                                break;
                            }
                        }

                        DataView dv = dsRead.Tables[0].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        this.gridControl2.DataSource = dt;
                        this.groupBox2.Text = String.Format("Excel油耗数据（共{0}条）", dsRead.Tables[0].Rows.Count);
                        this.gridView2.BestFitColumns();
                    }
                    else
                    {
                        MessageBox.Show("该时间段内备份油耗数据不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception msg)
                {
                    MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
            this.gridView2.OptionsView.ColumnAutoWidth = false;
        }

        //官方油耗数据
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "系统提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    MitsUtils miutils = new MitsUtils();
                    this.xtraTabControl1.SelectedTabPageIndex = 0;
                    this.gridControl1.DataSource = null;
                    this.groupBox1.Text = "官方油耗数据";
                    DataTable dt = null;
                    this.gridView1.Columns.Clear();
                    switch (radioGroup1.SelectedIndex)
                    {
                        case 0:
                            dt = GetLocalALL("ADC_T_ALL");
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                dt = miutils.E2C(miutils.dictCTNY, dt, MitsUtils.CTNY);
                            }
                            break;
                        case 1:
                            dt = GetLocalALL("ADC_T_ALL_FCDS");
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                dt = miutils.E2C(miutils.dictFCDSHHDL, dt, MitsUtils.FCDSHHDL);
                            }
                            break;
                        case 2:
                            dt = GetLocalALL("ADC_T_ALL_CDS");
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                dt = miutils.E2C(miutils.dictCDSHHDL, dt, MitsUtils.CDSHHDL);
                            }
                            break;
                        case 3:
                            dt = GetLocalALL("ADC_T_ALL_CDD");
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                dt = miutils.E2C(miutils.dictCDD, dt, MitsUtils.CDD);
                            }
                            break;
                        case 4:
                            dt = GetLocalALL("ADC_T_ALL_RLDC");
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                dt = miutils.E2C(miutils.dictRLDC, dt, MitsUtils.RLDC);
                            }
                            break;
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        this.gridControl1.DataSource = dt;
                        this.groupBox1.Text = String.Format("官方油耗数据（共{0}条）", dt.Rows.Count);
                        this.gridView1.BestFitColumns();
                    }
                    else
                    {
                        MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception msg)
                {
                    MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //导出到Excel
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gcTable1.DataSource == null && gcTable2.DataSource == null && gcDiff.DataSource == null)
            {
                MessageBox.Show("没有比对结果", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "本地Excel数据比对结果",
                Title = "导出Excel",
                Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls"
            };
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    ExportHelper.ExportToExcel(saveFileDialog.FileName, true, String.Empty, gcTable1, gcTable2, gcDiff);
                    ExcelHelper excelBuilder = new ExcelHelper(saveFileDialog.FileName);
                    excelBuilder.ChangeNameWorkSheet("Sheet1", "补传数据");
                    excelBuilder.ChangeNameWorkSheet("Sheet2", "撤销数据");
                    excelBuilder.ChangeNameWorkSheet("Sheet3", "修改数据");
                    excelBuilder.DeleteRows(1, 1);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.SaveFile();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //全选
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("比对数据页面不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                GridView view = (GridView)gridControl.MainView;
                view.FocusedRowHandle = 0;
                view.FocusedColumn = view.Columns["VIN"];
                Utils.SelectItem(view, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //反选
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("比对数据页面不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                GridView view = (GridView)gridControl.MainView;
                view.FocusedRowHandle = 0;
                view.FocusedColumn = view.Columns["VIN"];
                Utils.SelectItem(view, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //数据处理
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text == "补传数据")   //需要补传数据
                {
                    var vins = GetDataFormat();

                    if (vins != null && vins.Table.Rows.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var rllxName = GetRadio();
                    if (InsertFC_CLJBXX(vins, "1", rllxName))
                    {
                        foreach (Form f in Application.OpenForms)
                        {
                            if (f.Name == "SearchLocalOTForm")
                            {
                                f.Activate();
                                ((SearchLocalOTForm)f).LocalData(vins);
                                ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalOTForm)f).Ribbon.Pages[0];
                                return;
                            }
                        }
                        SearchLocalOTForm slo = new SearchLocalOTForm() { MdiParent = MdiParent };
                        slo.LocalData(vins);
                        ((MainForm)this.MdiParent).Ribbon.SelectedPage = slo.Ribbon.Pages[0];
                        slo.Show();
                    }
                    else
                    {
                        MessageBox.Show("操作失败，请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "撤销数据")  //需要撤销数据
                {
                    var vins = GetDataFormat();

                    if (vins != null && vins.Table.Rows.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    //将选中的处理数据更改状态
                    foreach (DataRow dr in vins.Table.Rows)
                    {
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("DELETE FROM FC_CLJBXX WHERE VIN = '{0}'", dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("INSERT INTO FC_CLJBXX (VIN,HGSPBM,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,QTXX,V_ID) SELECT VIN,HGSPBM,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,QTXX,V_ID FROM FC_CLJBXX_ADC WHERE VIN='{0}'", dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("UPDATE FC_CLJBXX SET STATUS='3',USER_ID='{0}' WHERE VIN='{1}'", Utils.localUserId, dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID) SELECT PARAM_CODE,VIN,PARAM_VALUE,V_ID FROM RLLX_PARAM_ENTITY_ADC WHERE VIN='{0}'", dr["VIN"]), null);
                    }
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.Name == "SearchLocalUploadedForm")
                        {
                            f.Activate();
                            ((SearchLocalUploadedForm)f).LocalData(vins);
                            ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUploadedForm)f).Ribbon.Pages[0];
                            return;
                        }
                    }

                    SearchLocalUploadedForm sluf = new SearchLocalUploadedForm() { MdiParent = this.MdiParent };
                    sluf.LocalData(vins);
                    ((MainForm)this.MdiParent).Ribbon.SelectedPage = sluf.Ribbon.Pages[0];
                    sluf.Show();
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "修改数据")    //需要修改数据
                {
                    var str = GetCheckString();
                    if (str == null || str.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    DataView vins = GetCheckData();

                    var dt = (DataTable)gridControl2.DataSource;
                    DataTable dtNew = dt.Clone();
                    foreach (string s in str)
                    {
                        var dr = dt.Select(String.Format("vin='{0}'", s));
                        if (dr.Length > 0)
                        {
                            foreach (DataRow r in dr)
                            {
                                dtNew.Rows.Add(r.ItemArray);
                            }

                            continue;
                        }
                    }
                    switch (radioGroup1.SelectedIndex)
                    {
                        case 0:

                            dtNew = miutils.D2D(miutils.dictCTNY, dtNew, MitsUtils.CTNY);
                            break;
                        case 1:

                            dtNew = miutils.D2D(miutils.dictFCDSHHDL, dtNew, MitsUtils.FCDSHHDL);
                            break;
                        case 2:

                            dtNew = miutils.D2D(miutils.dictCDSHHDL, dtNew, MitsUtils.CDSHHDL);
                            break;
                        case 3:

                            dtNew = miutils.D2D(miutils.dictCDD, dtNew, MitsUtils.CDD);
                            break;
                        case 4:

                            dtNew = miutils.D2D(miutils.dictRLDC, dtNew, MitsUtils.RLDC);
                            break;
                    }
                    var rllxName = GetRadio();
                    if (InsertFC_CLJBXX(dtNew.DefaultView, "2", rllxName))
                    {
                        foreach (Form f in Application.OpenForms)
                        {
                            if (f.Name == "SearchLocalUpdateForm")
                            {
                                f.Activate();
                                ((SearchLocalUpdateForm)f).LocalData(vins);
                                ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUpdateForm)f).Ribbon.Pages[0];
                                return;
                            }
                        }
                        SearchLocalUpdateForm suf = new SearchLocalUpdateForm() { MdiParent = this.MdiParent };
                        suf.LocalData(vins);
                        ((MainForm)this.MdiParent).Ribbon.SelectedPage = suf.Ribbon.Pages[0];
                        suf.Show();
                    }
                    else
                    {
                        MessageBox.Show("操作失败，请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //数据比对
        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                DataTable dtTable_gf = (DataTable)gridControl1.DataSource;
                DataTable dtTable_sc = (DataTable)gridControl2.DataSource;

                if (dtTable_gf == null || dtTable_sc == null || dtTable_gf.Rows.Count < 1 || dtTable_sc.Rows.Count < 1)
                {
                    MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    //弹出加载提示画面  
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    this.gcTable1.DataSource = null;
                    this.gcTable2.DataSource = null;
                    this.gcDiff.DataSource = null;
                    this.gvTable1.Columns.Clear();
                    this.gvTable2.Columns.Clear();
                    this.gvDiff.Columns.Clear();
                    //补传数据
                    var vinArr_GF = dtTable_gf.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                    var supplementsData = from d in dtTable_sc.AsEnumerable()
                                          where !vinArr_GF.Contains(d.Field<string>("VIN"))
                                          select d;
                    DataTable dtSupplement = supplementsData.AsDataView().ToTable();
                    //撤销数据
                    var vinArr_SC = dtTable_sc.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                    var recallData = from d in dtTable_gf.AsEnumerable()
                                     where !vinArr_SC.Contains(d.Field<string>("VIN"))
                                     select d;
                    DataTable dtRecall = recallData.AsDataView().ToTable();
                    //修改数据
                    List<string> vinList = new List<string>();
                    switch (radioGroup1.SelectedIndex)
                    {
                        case 0:
                            vinList = CompareHelper.CompareDataTableDiff(dtTable_gf, dtTable_sc, MitsUtils.CTNY);
                            break;
                        case 1:
                            vinList = CompareHelper.CompareDataTableDiff(dtTable_gf, dtTable_sc, MitsUtils.FCDSHHDL);
                            break;
                        case 2:
                            vinList = CompareHelper.CompareDataTableDiff(dtTable_gf, dtTable_sc, MitsUtils.CDSHHDL);
                            break;
                        case 3:
                            vinList = CompareHelper.CompareDataTableDiff(dtTable_gf, dtTable_sc, MitsUtils.CDD);
                            break;
                        case 4:
                            vinList = CompareHelper.CompareDataTableDiff(dtTable_gf, dtTable_sc, MitsUtils.RLDC);
                            break;
                    }
                    DataTable dtDiff = dtTable_sc.Clone();
                    dtDiff.Columns.Add("错误参数");
                    dtDiff.Columns.Add("官方值");
                    dtDiff.Columns.Add("备份值");
                    for (int i = 0; i < vinList.Count; i++)
                    {
                        DataRow rows_gf = dtTable_gf.Select(String.Format("VIN='{0}'", vinList[i])).FirstOrDefault();
                        DataRow rows_sc = dtTable_sc.Select(String.Format("VIN='{0}'", vinList[i])).FirstOrDefault();
                        for (int j = 0; j < dtTable_sc.Columns.Count; j++)
                        {
                            string columnName = dtTable_sc.Columns[j].ColumnName;
                            if (columnName.Equals("VIN") || columnName.Equals("唯一标示") || columnName.Equals("线路标示") || columnName.Equals("上报时间") || columnName.Equals("数据源标示") || columnName.Equals("选装件代码") || columnName.Equals("定制编号") || columnName.Equals("车型代码") || columnName.Equals("汽车节能技术") || columnName.Equals("上报人"))
                                continue;
                            if (!rows_sc[columnName].ToString().Equals(rows_gf[columnName].ToString()))
                            {
                                DataRow row = dtDiff.NewRow();
                                row.ItemArray = rows_sc.ItemArray;
                                row["错误参数"] = columnName;
                                row["官方值"] = rows_gf[columnName].ToString();
                                row["备份值"] = rows_sc[columnName].ToString();
                                dtDiff.Rows.Add(row);
                            }
                        }
                    }
                    //添加复选框
                    dtSupplement.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtSupplement.Columns["check"].ReadOnly = false;
                    dtSupplement.Columns["check"].Caption = "选择";
                    for (int i = 0; i < dtSupplement.Rows.Count; i++)
                    {
                        dtSupplement.Rows[i]["check"] = false;
                    }
                    dtSupplement.Columns["check"].SetOrdinal(0);
                    dtRecall.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRecall.Columns["check"].ReadOnly = false;
                    dtRecall.Columns["check"].Caption = "选择";
                    for (int i = 0; i < dtRecall.Rows.Count; i++)
                    {
                        dtRecall.Rows[i]["check"] = false;
                    }
                    dtRecall.Columns["check"].SetOrdinal(0);
                    dtDiff.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtDiff.Columns["check"].ReadOnly = false;
                    dtDiff.Columns["check"].Caption = "选择";
                    for (int i = 0; i < dtDiff.Rows.Count; i++)
                    {
                        dtDiff.Rows[i]["check"] = false;
                    }
                    dtDiff.Columns["check"].SetOrdinal(0);
                    dtDiff.Columns["错误参数"].SetOrdinal(2);
                    dtDiff.Columns["官方值"].SetOrdinal(3);
                    dtDiff.Columns["备份值"].SetOrdinal(4); 
                    this.gcTable1.DataSource = dtSupplement;
                    this.gcTable2.DataSource = dtRecall;
                    this.gcDiff.DataSource = dtDiff;
                    this.gvTable1.BestFitColumns();
                    this.gvTable2.BestFitColumns();
                    this.gvDiff.BestFitColumns();
                    foreach (DevExpress.XtraGrid.Columns.GridColumn col in this.gvTable1.Columns)
                    {
                        if (col.FieldName != "check")
                        {
                            col.OptionsColumn.ReadOnly = true;
                        }
                    }

                    foreach (DevExpress.XtraGrid.Columns.GridColumn col in this.gvTable2.Columns)
                    {
                        if (col.FieldName != "check")
                        {
                            col.OptionsColumn.ReadOnly = true;
                        }
                    }
                    foreach (DevExpress.XtraGrid.Columns.GridColumn col in this.gvDiff.Columns)
                    {
                        if (col.FieldName != "check")
                        {
                            col.OptionsColumn.ReadOnly = true;
                        }
                    }

                    //比较完初始选中的tab页
                    int selectTabIndex = 1;
                    if (supplementsData.AsDataView().ToTable().Rows.Count < 1)
                    {
                        selectTabIndex = 2;
                        if (recallData.AsDataView().ToTable().Rows.Count < 1)
                        {
                            selectTabIndex = 3;
                            if (this.gcDiff.DataSource == null)
                            {
                                MessageBox.Show("数据一致！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                    xtraTabControl1.SelectedTabPageIndex = selectTabIndex;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //获取选中的数据的dataView
        private DataView GetCheckData()
        {

            var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            return C2M.SelectedParamEntityDataView(dv, "check");
        }

        //获取选中的vin
        private List<string> GetCheckString()
        {

            var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            var selectedParamEntityIds = C2M.SelectedParamEntityIds(dv, "VIN");
            if (selectedParamEntityIds.Count > 0)
            {
                return selectedParamEntityIds;
            }
            return null;
        }

        //获取选中的表头
        private string GetRadio()
        {
            string radioName = string.Empty;
            switch (radioGroup1.SelectedIndex)
            {
                case 0:

                    radioName = "传统能源";
                    break;
                case 1:

                    radioName = "非插电式混合动力";
                    break;
                case 2:

                    radioName = "插电式混合动力";
                    break;
                case 3:

                    radioName = "纯电动";
                    break;
                case 4:

                    radioName = "燃料电池";
                    break;
            }
            return radioName;
        }

        //获取选中的数据的dataView-转化后的
        private DataView GetDataFormat()
        {
            DataView vins = GetCheckData();
            DataTable TempDt = new DataTable();
            if (vins != null)
            {
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:

                        TempDt = miutils.D2D(miutils.dictCTNY, vins.Table, MitsUtils.CTNY);
                        break;
                    case 1:

                        TempDt = miutils.D2D(miutils.dictFCDSHHDL, vins.Table, MitsUtils.FCDSHHDL);
                        break;
                    case 2:

                        TempDt = miutils.D2D(miutils.dictCDSHHDL, vins.Table, MitsUtils.CDSHHDL);
                        break;
                    case 3:

                        TempDt = miutils.D2D(miutils.dictCDD, vins.Table, MitsUtils.CDD);
                        break;
                    case 4:

                        TempDt = miutils.D2D(miutils.dictRLDC, vins.Table, MitsUtils.RLDC);
                        break;
                }
            }
            vins = TempDt.DefaultView;
            return vins;
        }

        //处理数据插入到数据库中
        private bool InsertFC_CLJBXX(DataView dv, string flag, string rllxParam)
        {
            bool result = false;
            DataTable dtParams = OracleHelper.ExecuteDataSet(OracleHelper.conn, (string)@"select * from RLLX_PARAM", null).Tables[0];
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = null;
                try
                {
                    tra = con.BeginTransaction();
                    foreach (DataRow drMain in dv.Table.Rows)
                    {
                        #region 待生成的燃料基本信息数据存入燃料基本信息表


                        string vin = drMain["VIN"].ToString().Trim();
                        string sqlDeleteBasic = String.Format("DELETE FROM FC_CLJBXX WHERE VIN='{0}'", vin);
                        OracleHelper.ExecuteNonQuery(tra, sqlDeleteBasic, null);
                        DateTime clzzrqDate;
                        try
                        {
                            clzzrqDate = DateTime.ParseExact(drMain["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            clzzrqDate = Convert.ToDateTime(drMain["CLZZRQ"]);
                        }
                        OracleParameter clzzrq = new OracleParameter("CLZZRQ", clzzrqDate) { DbType = DbType.Date };
                        DateTime uploadDeadlineDate = miutils.QueryUploadDeadLine(clzzrqDate);
                        OracleParameter uploadDeadline = new OracleParameter("UPLOADDEADLINE", uploadDeadlineDate) { DbType = DbType.Date };
                        OracleParameter creTime = new OracleParameter("CREATETIME", DateTime.Now) { DbType = DbType.Date };
                        OracleParameter upTime = new OracleParameter("UPDATETIME", DateTime.Now) { DbType = DbType.Date };
                        string qtxx;
                        if (dv.Table.Columns.Contains("CT_QTXX"))
                            qtxx = drMain["CT_QTXX"].ToString().Trim();
                        else
                            qtxx = string.Empty;

                        OracleParameter[] param = { 
                                    new OracleParameter("VIN",drMain["VIN"].ToString().Trim()),
                                    new OracleParameter("USER_ID",Utils.localUserId),
                                    new OracleParameter("QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                    new OracleParameter("JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("CLXH",drMain["CLXH"].ToString().Trim()),
                                    new OracleParameter("CLZL",drMain["CLZL"].ToString().Trim()),
                                    new OracleParameter("RLLX",drMain["RLLX"].ToString().Trim()),
                                    new OracleParameter("ZCZBZL",drMain["ZCZBZL"].ToString().Trim()),
                                    new OracleParameter("ZGCS",drMain["ZGCS"].ToString().Trim()),
                                    new OracleParameter("LTGG",drMain["LTGG"].ToString().Trim()),
                                    new OracleParameter("ZJ",drMain["ZJ"].ToString().Trim()),
                                    new OracleParameter("TYMC",drMain["TYMC"].ToString().Trim()),
                                    new OracleParameter("YYC",drMain["YYC"].ToString().Trim()),
                                    new OracleParameter("ZWPS",drMain["ZWPS"].ToString().Trim()),
                                    new OracleParameter("ZDSJZZL",drMain["ZDSJZZL"].ToString().Trim()),
                                    new OracleParameter("EDZK",drMain["EDZK"].ToString().Trim()),
                                    new OracleParameter("LJ",drMain["LJ"].ToString().Trim()),
                                    new OracleParameter("QDXS",drMain["QDXS"].ToString().Trim()),
                                    new OracleParameter("JYJGMC",drMain["JYJGMC"].ToString().Trim()),
                                    new OracleParameter("JYBGBH",drMain["JYBGBH"].ToString().Trim()),
                                    new OracleParameter("HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                    new OracleParameter("QTXX",qtxx),
                                    // 0：已上报；1：待上报/补传待上报；2：已修改未上报
                                    new OracleParameter("STATUS",flag),
                                    creTime,
                                    upTime,
                                    };
                        OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX
                            (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME
                            ) VALUES
                            (   :VIN,:USER_ID,:QCSCQY,:JKQCZJXS,:CLZZRQ,:UPLOADDEADLINE,:CLXH,:CLZL,
                                :RLLX,:ZCZBZL,:ZGCS,:LTGG,:ZJ,
                                :TYMC,:YYC,:ZWPS,:ZDSJZZL,:EDZK,:LJ,
                                :QDXS,:JYJGMC,:JYBGBH,:HGSPBM,:QTXX,:STATUS,:CREATETIME,:UPDATETIME)", param);

                        #endregion

                        #region 插入参数信息

                        string sqlDelParam = String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", drMain["VIN"].ToString().Trim());
                        OracleHelper.ExecuteNonQuery(tra, sqlDelParam, null);
                        var rows = dtParams.Select(String.Format("FUEL_TYPE='{0}' and STATUS='1'", rllxParam));
                        // 待生成的燃料参数信息存入燃料参数表
                        foreach (DataRow drParam in rows)
                        {
                            string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                            OracleParameter[] paramList = { 
                                    new OracleParameter("PARAM_CODE",paramCode),
                                    new OracleParameter("VIN",drMain["VIN"].ToString().Trim()),
                                    new OracleParameter("PARAM_VALUE",drMain[paramCode].ToString().Trim()),
                                    new OracleParameter("V_ID","")
                                };
                            OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO RLLX_PARAM_ENTITY 
                                        (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                    VALUES
                                        (:PARAM_CODE,:VIN,:PARAM_VALUE,:V_ID)", paramList);
                        }
                        #endregion
                    }

                    tra.Commit();
                    result = true;

                }
                catch (Exception ex)
                {
                    tra.Rollback();
                    throw ex;
                }
                finally
                {
                    tra.Dispose();
                    con.Close();
                }
            }
            return result;
        }

        private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView4_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView5_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView6_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.gridControl1.DataSource = null;
            this.gridView1.Columns.Clear();
            this.groupBox1.Text = "官方油耗数据";
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "Excel油耗数据";
            this.gridView2.Columns.Clear();
            this.xtraTabControl1.SelectedTabPageIndex = 0;
        }
    }
}