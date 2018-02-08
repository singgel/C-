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
using FuelDataSysClient.Tool.Tool_Toyota;
using FuelDataSysClient.Form_DBManager;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Compare.Form_Toyota
{
    public partial class ComparisonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        ToyotaCompareUtils toyotaUtils = new ToyotaCompareUtils();
        DataTable dtTable1 = null;
        DataTable dtTable2 = null;

        public ComparisonForm()
        {
            InitializeComponent();
        }

        private void ComparisonForm_Load(object sender, EventArgs e)
        {
            this.gvDataXT.OptionsBehavior.Editable = false;
            this.gvDataQY.OptionsBehavior.Editable = false;
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //获取本地能源数据
        private DataTable GetLocalALL(string tableName)
        {
            xtraTabControl1.SelectedTabPageIndex = 0;
            string sql = string.Empty;
            if (this.cbTimeType.Text.Trim().Equals("上报日期"))
                sql = String.Format(@"select * from {0} where USER_ID = '{1}' and (UPDATETIME>=#{2}#) and  (UPDATETIME<#{3}#)", tableName, Utils.userId, Convert.ToDateTime(dtStartTime.Text), Convert.ToDateTime(dtEndTime.Text).Add(new TimeSpan(24, 0, 0)));
            else
                sql = String.Format(@"select * from {0} where USER_ID = '{1}' and (CLZZRQ>=#{2}#) and  (CLZZRQ<#{3}#)", tableName, Utils.userId, Convert.ToDateTime(dtStartTime.Text), Convert.ToDateTime(dtEndTime.Text).Add(new TimeSpan(24, 0, 0)));
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0].Copy();
                dt.Columns.Remove("USER_ID");
                if (tableName.Equals("VIEW_T_ALL") || tableName.Equals("VIEW_T_ALL_ADC"))
                {
                    dt.Columns.Remove("CT_QTXX");
                }
                return dt;
            }
            return null;
        }

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
            this.gcDataQY.DataSource = null;
            this.groupBox2.Text = "已上报油耗数据";
            this.gvDataQY.Columns.Clear();
            DataTable dt = null;
            switch (radioGroup1.SelectedIndex)
            {
                case 0:
                    dt = GetLocalALL("VIEW_T_ALL");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt = toyotaUtils.E2C(toyotaUtils.dictCTNY, dt, MitsUtils.CTNY);
                    }
                    break;
                case 1:
                    dt = GetLocalALL("VIEW_T_ALL_FCDS");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dt = toyotaUtils.E2C(toyotaUtils.dictFCDSHHDL, dt, MitsUtils.FCDSHHDL);
                    }
                    break;
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                this.gcDataQY.DataSource = dt;
                this.gvDataQY.BestFitColumns();
                this.groupBox2.Text = String.Format("已上报油耗数据（共{0}条）", dt.Rows.Count);
                this.gvDataQY.OptionsView.ColumnAutoWidth = false;
            }
            else
            {
                MessageBox.Show("该时间段内无已上报油耗数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

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
                this.gcDataXT.DataSource = null;
                this.groupBox1.Text = "系统油耗数据";
                DataTable dt = null;
                this.gvDataXT.Columns.Clear();
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        dt = GetLocalALL("VIEW_T_ALL_ADC");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = toyotaUtils.E2C(toyotaUtils.dictCTNY, dt, MitsUtils.CTNY);
                        }
                        break;
                    case 1:
                        dt = GetLocalALL("VIEW_T_ALL_FCDS_ADC");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = toyotaUtils.E2C(toyotaUtils.dictFCDSHHDL, dt, MitsUtils.FCDSHHDL);
                        }
                        break;
                }

                if (dt != null && dt.Rows.Count > 0)
                {
                    this.gcDataXT.DataSource = dt;
                    this.gvDataXT.BestFitColumns();
                    this.groupBox1.Text = String.Format("系统油耗数据（共{0}条）", dt.Rows.Count);
                    this.gvDataXT.OptionsView.ColumnAutoWidth = false;
                }
                else
                {
                    MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                using (CompositeLink complink = new CompositeLink(new PrintingSystem()))
                {
                    PrintableComponentLink linkTable1 = new PrintableComponentLink();
                    PrintableComponentLink linkTable2 = new PrintableComponentLink();
                    PrintableComponentLink linkDiff = new PrintableComponentLink();
                    linkTable1.Component = gcSupplements;
                    complink.Links.Add(linkTable1);
                    linkTable2.Component = gcRecall;
                    complink.Links.Add(linkTable2);
                    linkDiff.Component = gcModify;
                    complink.Links.Add(linkDiff);
                    complink.CreatePageForEachLink();
                    if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        XlsxExportOptions option = new XlsxExportOptions() { ExportMode = XlsxExportMode.SingleFilePageByPage };
                        complink.ExportToXlsx(saveFileDialog.FileName, option);
                        if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // 一键比对
        private void barButtonItem9_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "系统提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                dtTable1 = (DataTable)gcDataXT.DataSource;
                dtTable2 = (DataTable)gcDataQY.DataSource;

                if (dtTable1 == null || dtTable2 == null)
                {
                    MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    DataTable dtRetAdd = new DataTable();
                    DataTable dtRetDiff_CLXH = new DataTable();
                    DataTable dtRetDel = new DataTable();
                    dtRetAdd = CompareDataTableAdd(dtTable1, dtTable2);
                    dtRetDel = CompareDataTableDel(dtTable1, dtTable2);
                    dtRetDiff_CLXH = CompareDataTableDiff_CLXH(dtTable1, dtTable2);
                    dtRetAdd.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetAdd.Columns["check"].SetOrdinal(0);
                    dtRetDel.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDel.Columns["check"].SetOrdinal(0);
                    dtRetDiff_CLXH.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDiff_CLXH.Columns["check"].SetOrdinal(0);
                    this.groupBox3.Text = String.Format("共计{0}条", dtRetAdd.Rows.Count);
                    this.groupBox4.Text = String.Format("共计{0}条", dtRetDel.Rows.Count);
                    this.groupBox5.Text = String.Format("共计{0}条", dtRetDiff_CLXH.Rows.Count);
                    // 以下代码导出用，导入到一个Excel的多个sheet
                    this.gcSupplements.DataSource = dtRetAdd;
                    this.gvSupplements.BestFitColumns();
                    this.gcRecall.DataSource = dtRetDel;
                    this.gvRecall.BestFitColumns();
                    this.gcModify.DataSource = dtRetDiff_CLXH;
                    this.gvModify.BestFitColumns();
                    xtraTabControl1.SelectedTabPageIndex = 1;
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
        }

        /// <summary>
        /// 需要撤销的数据
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public DataTable CompareDataTableDel(DataTable dt1, DataTable dt2)
        {
            DataTable dtADD = new DataTable();
            dtADD = dt1.Clone();

            var vin1List = from d1 in dt1.AsEnumerable()
                           select d1.Field<string>("VIN车架号");

            var tbAdd = from t2 in dt2.AsEnumerable()
                        where vin1List.Contains(t2.Field<string>("VIN车架号")) == false
                        select t2;
            var rowList = tbAdd.ToList();
            foreach (DataRow dr in rowList)
            {
                DataRow dd = dtADD.NewRow();
                dd.ItemArray = dr.ItemArray;
                dtADD.Rows.Add(dd);
            }
            return dtADD;
        }

        /// <summary>
        /// 需要补传的数据
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public DataTable CompareDataTableAdd(DataTable dt1, DataTable dt2)
        {
            DataTable dtDel = new DataTable();
            dtDel = dt1.Clone();

            var vin2List = from d2 in dt2.AsEnumerable()
                           select d2.Field<string>("VIN车架号");

            var tbDel = from t1 in dt1.AsEnumerable()
                        where vin2List.Contains(t1.Field<string>("VIN车架号")) == false
                        select t1;
            var rowList = tbDel.ToList();
            foreach (DataRow dr in rowList)
            {
                DataRow dd = dtDel.NewRow();
                dd.ItemArray = dr.ItemArray;
                dtDel.Rows.Add(dd);
            }
            return dtDel;
        }

        /// <summary>
        /// 比较两个DataTable中VIN与CLXH的对应关系
        /// </summary>
        /// <param name="dt11"></param>
        /// <param name="dt22"></param>
        /// <returns></returns>
        private DataTable CompareDataTableDiff_CLXH(DataTable dt1, DataTable dt2)
        {
            DataTable dtDIFF = new DataTable();
            dtDIFF.Columns.Add("VIN车架号");
            dtDIFF.Columns.Add("系统车辆型号");
            dtDIFF.Columns.Add("本地车辆型号");

            var tbDIFF = from t1 in dt1.AsEnumerable()
                         join t2 in dt2.AsEnumerable()
                         on t1.Field<string>("VIN车架号") equals t2.Field<string>("VIN车架号")
                         where t1.Field<string>("车辆型号") != t2.Field<string>("车辆型号")
                         select new
                         {
                             VIN = t1.Field<string>("VIN车架号"),
                             CLXH_T1 = t1.Field<string>("车辆型号"),
                             CLXH_T2 = t2.Field<string>("车辆型号")
                         };

            var rowList = tbDIFF.ToList();
            foreach (var dr in rowList)
            {
                DataRow dd = dtDIFF.NewRow();
                dd.ItemArray = new object[] { dr.VIN, dr.CLXH_T1, dr.CLXH_T2 };
                dtDIFF.Rows.Add(dd);
            }
            return dtDIFF;
        }

        // 全选
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridview = (GridView)((GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0]).MainView;
                gridview.FocusedRowHandle = 0;
                gridview.FocusedColumn = gridview.Columns[1];
                GridControlHelper.SelectItem(gridview, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 取消全选
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridview = (GridView)((GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0]).MainView;
                gridview.FocusedRowHandle = 0;
                gridview.FocusedColumn = gridview.Columns[1];
                GridControlHelper.SelectItem(gridview, false);
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
                var vins = GetCheckData();
                if (vins != null && vins.Table.Rows.Count == 0)
                {
                    MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "补传数据")   //需要补传数据
                {
                    if (InsertFC_CLJBXX(vins))
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
                        SearchLocalOTForm slo = new SearchLocalOTForm() { MdiParent = this.MdiParent };
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
                if (this.xtraTabControl1.SelectedTabPage.Text == "车辆型号不一致")    //需要修改数据
                {
                    if (UpdateFC_CLJBXX(vins))
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

        private DataView GetCheckData()
        {
            var gridControl = (GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView vins = C2M.SelectedParamEntityDataView((DataView)view.DataSource, "check");
            DataTable TempDt = new DataTable();
            if (vins != null)
            {
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:

                        TempDt = toyotaUtils.C2E(toyotaUtils.dictCTNY, vins.Table, MitsUtils.CTNY);
                        break;
                    case 1:

                        TempDt = toyotaUtils.C2E(toyotaUtils.dictFCDSHHDL, vins.Table, MitsUtils.FCDSHHDL);
                        break;
                }
            }
            vins = TempDt.DefaultView;
            return vins;
        }

        private bool UpdateFC_CLJBXX(DataView dv)
        {
            bool result = false;
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                OleDbTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    tra = con.BeginTransaction();
                    foreach (DataRow drMain in dv.Table.Rows)
                    {
                        string vin = drMain["VIN"].ToString().Trim();
                        AccessHelper.ExecuteNonQuery(tra, String.Format("UPDATE FC_CLJBXX SET STATUS = '2' WHERE VIN = '{0}'", vin), null);
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

        private bool InsertFC_CLJBXX(DataView dv)
        {
            bool result = false;
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                OleDbTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    tra = con.BeginTransaction();
                    foreach (DataRow drMain in dv.Table.Rows)
                    {
                        string vin = drMain["VIN"].ToString().Trim();
                        AccessHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM FC_CLJBXX WHERE VIN='{0}'", vin), null);
                        AccessHelper.ExecuteNonQuery(tra, String.Format("INSERT INTO FC_CLJBXX SELECT * FROM FC_CLJBXX_ADC WHERE VIN='{0}'", vin), null);

                        AccessHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN='{0}'", vin), null);
                        AccessHelper.ExecuteNonQuery(tra, String.Format("INSERT INTO RLLX_PARAM_ENTITY SELECT * FROM RLLX_PARAM_ENTITY_ADC WHERE VIN='{0}'", vin), null);
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
            this.gcDataXT.DataSource = null;
            this.groupBox1.Text = "系统油耗数据";
            this.gcDataQY.DataSource = null;
            this.groupBox2.Text = "已上报油耗数据";
        }
    }
}