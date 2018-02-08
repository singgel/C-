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
using System.Diagnostics;

namespace FuelDataSysClient.Form_SJBD
{
    public partial class ProductContrastForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ProductContrastForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
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

        //官方数据
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
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "提示",
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
                        MessageBox.Show("该条件下的油耗数据不存在或未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception msg)
                {
                    MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //生产数据
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
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                MitsUtils miutils = new MitsUtils();
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                this.gridControl2.DataSource = null;
                this.groupBox2.Text = "系统油耗数据";
                DataTable dt = null;
                this.gridView2.Columns.Clear();
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        dt = GetLocalALL("VIEW_T_ALL");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictCTNY, dt, MitsUtils.CTNY);
                        }

                        break;
                    case 1:
                        dt = GetLocalALL("VIEW_T_ALL_FCDS");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictFCDSHHDL, dt, MitsUtils.FCDSHHDL);
                        }
                        break;
                    case 2:
                        dt = GetLocalALL("VIEW_T_ALL_CDS");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictCDSHHDL, dt, MitsUtils.CDSHHDL);
                        }
                        break;
                    case 3:
                        dt = GetLocalALL("VIEW_T_ALL_CDD");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictCDD, dt, MitsUtils.CDD);
                        }
                        break;
                    case 4:
                        dt = GetLocalALL("VIEW_T_ALL_RLDC");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictRLDC, dt, MitsUtils.RLDC);
                        }
                        break;
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    this.gridControl2.DataSource = dt;
                    this.groupBox2.Text = String.Format("系统油耗数据（共{0}条）", dt.Rows.Count);
                    this.gridView2.BestFitColumns();
                }
                else
                {
                    MessageBox.Show("该时间段内没有系统油耗数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception msg)
            {
                MessageBox.Show(msg.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                SplashScreenManager.CloseForm();
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
                FileName = "生产线数据比对结果",
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

        //取消全选
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
                //var view = gridControl.MainView;
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
                    DataTable dtDiff = new DataTable();
                    switch (radioGroup1.SelectedIndex)
                    {
                        case 0:
                            dtDiff = CompareHelper.CompareDataTableDetail_pro(dtTable_gf, dtTable_sc, MitsUtils.CTNY);
                            break;
                        case 1:
                            dtDiff = CompareHelper.CompareDataTableDetail_pro(dtTable_gf, dtTable_sc, MitsUtils.FCDSHHDL);
                            break;
                        case 2:
                            dtDiff = CompareHelper.CompareDataTableDetail_pro(dtTable_gf, dtTable_sc, MitsUtils.CDSHHDL);
                            break;
                        case 3:
                            dtDiff = CompareHelper.CompareDataTableDetail_pro(dtTable_gf, dtTable_sc, MitsUtils.CDD);
                            break;
                        case 4:
                            dtDiff = CompareHelper.CompareDataTableDetail_pro(dtTable_gf, dtTable_sc, MitsUtils.RLDC);
                            break;
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
                    if (dtSupplement.Rows.Count < 1)
                    {
                        selectTabIndex = 2;
                        if (dtRecall.Rows.Count < 1)
                        {
                            selectTabIndex = 3;
                            if (dtDiff.Rows.Count < 1)
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
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //数据处理
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                if(this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //获取选中的处理数据
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                var view = gridControl.MainView;
                view.PostEditor();
                DataView dv = C2M.SelectedParamEntityDataView((DataView)view.DataSource, "check");
                if (dv == null)
                {
                    MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (dv != null && dv.Table.Rows.Count == 0)
                {
                    MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "补传数据")
                {
                    //将选中的处理数据更改状态
                    foreach (DataRow dr in dv.Table.Rows)
                    {
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("UPDATE FC_CLJBXX SET STATUS = '1' where vin='{0}'", dr["VIN"]), null);
                    }
                    //打开补传待上报窗体显示处理后数据
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.Name == "SearchLocalOTForm")
                        {
                            f.Activate();
                            ((SearchLocalOTForm)f).LocalData(dv);
                            ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalOTForm)f).Ribbon.Pages[0];
                            return;
                        }
                    }
                    SearchLocalOTForm slo = new SearchLocalOTForm() { MdiParent = this.MdiParent };
                    slo.LocalData(dv);
                    ((MainForm)this.MdiParent).Ribbon.SelectedPage = slo.Ribbon.Pages[0];
                    slo.Show();
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "撤销数据")
                {
                    //将选中的处理数据更改状态
                    foreach (DataRow dr in dv.Table.Rows)
                    {
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("DELETE FROM FC_CLJBXX WHERE VIN = '{0}'", dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("INSERT INTO FC_CLJBXX (VIN,HGSPBM,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,QTXX,V_ID) SELECT VIN,HGSPBM,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,QTXX,V_ID FROM FC_CLJBXX_ADC WHERE VIN='{0}'", dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("UPDATE FC_CLJBXX SET STATUS='3',USER_ID='{0}' WHERE VIN='{1}'", Utils.localUserId, dr["VIN"]), null);
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID) SELECT PARAM_CODE,VIN,PARAM_VALUE,V_ID FROM RLLX_PARAM_ENTITY_ADC WHERE VIN='{0}'", dr["VIN"]), null);
                    }
                    //打开已上报窗体显示需要撤销的数据
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.Name == "SearchLocalUploadedForm")
                        {
                            f.Activate();
                            ((SearchLocalUploadedForm)f).LocalData(dv);
                            ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUploadedForm)f).Ribbon.Pages[0];
                            return;
                        }
                    }

                    SearchLocalUploadedForm sluf = new SearchLocalUploadedForm() { MdiParent = this.MdiParent };
                    sluf.LocalData(dv);
                    ((MainForm)this.MdiParent).Ribbon.SelectedPage = sluf.Ribbon.Pages[0];
                    sluf.Show();
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "修改数据")
                {
                    //将选中的处理数据更改状态
                    foreach (DataRow dr in dv.Table.Rows)
                    {
                        OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("UPDATE FC_CLJBXX SET STATUS = '2' where vin='{0}'", dr["VIN"]), null);
                    }
                    //打开已修改未上报窗体显示处理后数据
                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.Name == "SearchLocalUpdateForm")
                        {
                            f.Activate();
                            ((SearchLocalUpdateForm)f).LocalData(dv);
                            ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUpdateForm)f).Ribbon.Pages[0];
                            return;
                        }
                    }
                    SearchLocalUpdateForm suf = new SearchLocalUpdateForm() { MdiParent = this.MdiParent };
                    suf.LocalData(dv);
                    ((MainForm)this.MdiParent).Ribbon.SelectedPage = suf.Ribbon.Pages[0];
                    suf.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //处理数据插入到数据库中
        private void OpreratorDealData(DataView dv,string status)
        {
            MitsUtils miutils = new MitsUtils();
            DataTable dtRllxParam = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from RLLX_PARAM", null).Tables[0]; 
            string rllx = string.Empty;
            DataTable TempDt = new DataTable();
            switch (radioGroup1.SelectedIndex)
            {
                case 0:
                    rllx = "传统能源";
                    TempDt = miutils.C2E(miutils.dictCTNY, dv.Table, MitsUtils.CTNY);
                    break;
                case 1:
                    rllx = "非插电式混合动力";
                    TempDt = miutils.C2E(miutils.dictFCDSHHDL, dv.Table, MitsUtils.FCDSHHDL);
                    break;
                case 2:
                    rllx = "插电式混合动力";
                    TempDt = miutils.C2E(miutils.dictCDSHHDL, dv.Table, MitsUtils.CDSHHDL);
                    break;
                case 3:
                    rllx = "纯电动";
                    TempDt = miutils.C2E(miutils.dictCDD, dv.Table, MitsUtils.CDD);
                    break;
                case 4:
                    rllx = "燃料电池";
                    TempDt = miutils.C2E(miutils.dictRLDC, dv.Table, MitsUtils.RLDC);
                    break;
            }
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    tra = con.BeginTransaction();
                    foreach (DataRow drMain in TempDt.Rows)
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

                        OracleParameter clzzrq = new OracleParameter("@CLZZRQ", clzzrqDate) { OracleDbType = OracleDbType.Date };
                        DateTime uploadDeadlineDate = miutils.QueryUploadDeadLine(clzzrqDate);
                        OracleParameter uploadDeadline = new OracleParameter("@UPLOADDEADLINE", uploadDeadlineDate) { OracleDbType = OracleDbType.Date };
                        OracleParameter creTime = new OracleParameter("@CREATETIME", DateTime.Now) { OracleDbType = OracleDbType.Date };
                        OracleParameter upTime = new OracleParameter("@UPDATETIME", DateTime.Now) { OracleDbType = OracleDbType.Date };
                        string qtxx;
                        if (dv.Table.Columns.Contains("CT_QTXX"))
                            qtxx = drMain["CT_QTXX"].ToString().Trim();
                        else
                            qtxx = string.Empty;


                        OracleParameter[] param = { 
                                    new OracleParameter("@VIN",drMain["VIN"].ToString().Trim()),
                                    new OracleParameter("@USER_ID",Utils.localUserId),
                                    new OracleParameter("@QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                    new OracleParameter("@JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("@CLXH",drMain["CLXH"].ToString().Trim()),
                                    new OracleParameter("@CLZL",drMain["CLZL"].ToString().Trim()),
                                    new OracleParameter("@RLLX",drMain["RLLX"].ToString().Trim()),
                                    new OracleParameter("@ZCZBZL",drMain["ZCZBZL"].ToString().Trim()),
                                    new OracleParameter("@ZGCS",drMain["ZGCS"].ToString().Trim()),
                                    new OracleParameter("@LTGG",drMain["LTGG"].ToString().Trim()),
                                    new OracleParameter("@ZJ",drMain["ZJ"].ToString().Trim()),
                                    new OracleParameter("@TYMC",drMain["TYMC"].ToString().Trim()),
                                    new OracleParameter("@YYC",drMain["YYC"].ToString().Trim()),
                                    new OracleParameter("@ZWPS",drMain["ZWPS"].ToString().Trim()),
                                    new OracleParameter("@ZDSJZZL",drMain["ZDSJZZL"].ToString().Trim()),
                                    new OracleParameter("@EDZK",drMain["EDZK"].ToString().Trim()),
                                    new OracleParameter("@LJ",drMain["LJ"].ToString().Trim()),
                                    new OracleParameter("@QDXS",drMain["QDXS"].ToString().Trim()),
                                    new OracleParameter("@JYJGMC",drMain["JYJGMC"].ToString().Trim()),
                                    new OracleParameter("@JYBGBH",drMain["JYBGBH"].ToString().Trim()),
                                    new OracleParameter("@HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                    new OracleParameter("@QTXX",qtxx),
                                    new OracleParameter("@STATUS",status),
                                    creTime,
                                    upTime
                                    };
                        OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX
                            (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME
                            ) VALUES
                            (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                @QDXS,@JYJGMC,@JYBGBH,@HGSPBM,@QTXX,@STATUS,@CREATETIME,@UPDATETIME)", param);

                        #endregion

                        #region 插入参数信息

                        string sqlDelParam = String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", vin);
                        OracleHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                        // 待生成的燃料参数信息存入燃料参数表
                        var rows = dtRllxParam.Select(String.Format("FUEL_TYPE='{0}' and STATUS='1'", rllx));
                        foreach (DataRow drParam in rows)
                        {
                            string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                            OracleParameter[] paramList = { 
                                    new OracleParameter("@PARAM_CODE",paramCode),
                                    new OracleParameter("@VIN",drMain["VIN"].ToString().Trim()),
                                    new OracleParameter("@PARAM_VALUE",drMain[paramCode].ToString().Trim()),
                                    new OracleParameter("@V_ID","")
                                };
                            OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO RLLX_PARAM_ENTITY 
                                        (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                    VALUES
                                        (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)", paramList);
                        }
                        #endregion
                    }
                    tra.Commit();

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
        }

        private void TimerContrastDataForm_Load(object sender, EventArgs e)
        {
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView2.OptionsBehavior.Editable = false;

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
            this.groupBox1.Text = "官方油耗数据";
            this.gridView1.Columns.Clear();
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "系统油耗数据";
            this.gridView2.Columns.Clear();
            this.xtraTabControl1.SelectedTabPageIndex = 0;
        }
    }
}