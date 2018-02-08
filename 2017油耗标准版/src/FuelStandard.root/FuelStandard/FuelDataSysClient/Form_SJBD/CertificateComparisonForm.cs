using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Common;
using DevExpress.XtraGrid;
using System.Linq;
using DevExpress.XtraPrintingLinks;
using DevExpress.XtraPrinting;
using FuelDataSysClient.Tool;
using System.Threading;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;
using DevExpress.XtraGrid.Views.Grid;

namespace FuelDataSysClient.CertificateService
{
    public partial class CertificateComparisonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        
        DataTableHelper dth = new DataTableHelper();
        DataTable dtCertifi = new DataTable(); //合格证数据
        CertificateService.CertificateComparison ccf = Utils.serviceCertificate;
        InitDataTime initTime = new InitDataTime();

        public CertificateComparisonForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
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
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "系统提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                this.gridControl1.DataSource = null;
                this.groupBox1.Text = "系统油耗数据";
                string sql = string.Empty;
                if (Utils.userId.Equals("FADCFFTZGU001"))
                {
                    sql = string.Format("select vin,clxh,clzzrq,rllx from fc_cljbxx_adc where status='0' and cdate(Format(CLZZRQ,'yyyy/mm/dd'))>=#{0}# and cdate(Format(CLZZRQ,'yyyy/mm/dd'))<=#{1}#", dtStartTime.Text, dtEndTime.Text);
                }
                else
                {
                    sql = string.Format("select vin,clxh,clzzrq,rllx from fc_cljbxx where status='0' and cdate(Format(CLZZRQ,'yyyy/mm/dd'))>=#{0}# and cdate(Format(CLZZRQ,'yyyy/mm/dd'))<=#{1}#", dtStartTime.Text, dtEndTime.Text);
                }
                var ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    this.gridControl1.DataSource = dt;
                    this.groupBox1.Text = String.Format("系统油耗数据（共{0}条）", ds.Tables[0].Rows.Count);
                }
                else
                {
                    MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// 模拟测试合格证数据  测试用户使用
        /// </summary>
        /// <returns></returns>
        private DataTable CertificateData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("vin");
            dt.Columns.Add("clxh");
            dt.Columns.Add("clzzrq");
            dt.Columns.Add("rllx");
            for (int i = 1; i < 20; i++)
            {
                var dr = dt.NewRow();
                dr["vin"] = "TESTXLLIEVBNM" + i.ToString() + "EI" + i.ToString();
                dr["clxh"] = "TESTCLXH";
                dr["clzzrq"] = "2015-02-05";
                dr["rllx"] = "汽油";
                dt.Rows.Add(dr);
            }
            return dt;
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
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "合格证数据";
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                if (!Utils.IsFuelTest)   //测试用户 测试数据
                {
                    this.gridControl2.DataSource = DataFormat(CertificateData());
                    dtCertifi = CertificateData();
                    dtCertifi.Columns["VIN"].ColumnName = "VIN";
                    dtCertifi.Columns["CLXH"].ColumnName = "产品型号";
                    dtCertifi.Columns["CLZZRQ"].ColumnName = "车辆制造日期/进口核销日期";
                    dtCertifi.Columns["RLLX"].ColumnName = "燃料种类";
                }
                else
                {
                    DataSet ds = new DataSet();
                    if (Utils.userId.Equals("FADCCSHDZU001"))
                    {
                        DataSet ds1 = ccf.QueryCertificate(Utils.userId, Utils.password, "上海大众汽车有限公司", dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                        DataSet ds2 = ccf.QueryCertificate(Utils.userId, Utils.password, "上汽大众汽车有限公司", dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                        if (ds1 != null)
                        {
                            ds.Merge(ds1, true, MissingSchemaAction.AddWithKey);
                        }
                        if (ds2 != null)
                        {
                            ds.Merge(ds2, true, MissingSchemaAction.AddWithKey);
                        }
                    }
                    else if ("FADCFDZXSU001".Equals(Utils.userId))
                    {
                        ds = ccf.QueryCertificate(Utils.userId, Utils.password, "大众汽车(中国)销售有限公司", dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                    }
                    else
                    {
                        ds = ccf.QueryCertificate(Utils.userId, Utils.password, Utils.qymc, dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                    }

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dtCertifi = ds.Tables[0];
                        this.gridControl2.DataSource = DataFormat(ds.Tables[0]);
                        this.groupBox2.Text = String.Format("合格证数据（共{0}条）", ds.Tables[0].Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("无合格证数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询超时,请重试", "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally 
            {
                SplashScreenManager.CloseForm();
            }
            
        }

        private DataTable DataFormat(DataTable dt)
        {
            DataTable tempDt = new DataTable();
            tempDt = dt.Copy();
            foreach (DataRow r in tempDt.Rows)
            {
                r["VIN"] = StrFormat(Convert.ToString(r["VIN"]));
            }

            return tempDt;
        }

        private string StrFormat(string str)
        {
           return str.Replace(str.Substring(8, 3), "***");
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                GridView view = (GridView)gridControl.MainView;
                view.FocusedRowHandle = 0;
                view.FocusedColumn = view.Columns[1];
                Utils.SelectItem(view, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                GridView view = (GridView)gridControl.MainView;
                view.FocusedRowHandle = 0;
                view.FocusedColumn = view.Columns[1];
                Utils.SelectItem(view, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                if (gcTable1.MainView.RowCount == 0 && gcTable2.MainView.RowCount == 0)
                {
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "油耗合格证比对结果";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    ExportToExcel toExcel = new ExportToExcel();
                    DataSet ds = new DataSet();
                    if (gcTable1.DataSource != null)
                    {
                        DataTable dt1 = (DataTable)gcTable1.DataSource;
                        DataTable dtc1 = dt1.Copy();
                        dtc1.TableName = "补传数据";
                        dtc1.Columns.Remove(dtc1.Columns[0]);
                        ds.Tables.Add(dtc1);
                    }
                    if (gcTable2.DataSource != null)
                    {
                        DataTable dt2 = (DataTable)gcTable2.DataSource;
                        DataTable dtc2 = dt2.Copy();
                        dtc2.TableName = "撤销数据";
                        dtc2.Columns.Remove(dtc2.Columns[0]);
                        ds.Tables.Add(dtc2);
                    }
                    toExcel.ToExcelSheet(ds, saveFileDialog.FileName);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，请检查！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fuelData = (DataTable)this.gridControl1.DataSource;
            var CertificateData = (DataTable)this.gridControl2.DataSource;
            if (fuelData == null || CertificateData == null)
            {
                MessageBox.Show("没有需要比较的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                //弹出加载提示画面  
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //补传数据
                var vinArr_F = fuelData.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                var supplementsData = from d in CertificateData.AsEnumerable()
                                      where !vinArr_F.Contains(d.Field<string>("VIN"))
                                      select d;
                DataTable dtSupplement = supplementsData.AsDataView().ToTable();
                //撤销数据
                var vinArr_HGZ = CertificateData.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                var recallData = from d in fuelData.AsEnumerable()
                                 where !vinArr_HGZ.Contains(d.Field<string>("VIN"))
                                 select d;
                DataTable dtRecall = recallData.AsDataView().ToTable();
                //添加复选框
                dtSupplement.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dtSupplement.Columns["check"].SetOrdinal(0);
                dtSupplement.Columns["check"].Caption = "选择";
                dtRecall.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dtRecall.Columns["check"].SetOrdinal(0);
                dtRecall.Columns["check"].Caption = "选择";
                this.gcTable1.DataSource = dtSupplement;
                this.gcTable2.DataSource = dtRecall;
                //比较完初始选中的tab页
                int selectTabIndex = 1;
                if (dtSupplement.Rows.Count < 1)
                {
                    selectTabIndex = 2;
                    if (dtRecall.Rows.Count < 1)
                    {
                        MessageBox.Show("数据一致！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                xtraTabControl1.SelectedTabPageIndex = selectTabIndex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataView vins = GetCheckData();
            if (vins == null)
            {
                MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (vins.Table == null)
            {
                MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            foreach (Form f in Application.OpenForms)
            {
                if (f.Name == "SearchLocalUploadedForm")
                {
                    f.Activate();
                    ((SearchLocalUploadedForm)f).LocalData(vins);
                    ribbon.SelectedPage = ((SearchLocalUploadedForm)f).Ribbon.Pages[0];
                    return;
                }
            }

            SearchLocalUploadedForm sluf = new SearchLocalUploadedForm();
            sluf.MdiParent = this.MdiParent;
            sluf.LocalData(vins);
            sluf.Show();
        }

        /// <summary>
        /// 选择选中数据
        /// </summary>
        /// <returns></returns>
        private DataView GetCheckData()
        {

            if (!this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
            {
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                if (gridControl.Name == "gcTable2")
                {
                    var view = gridControl.MainView;
                    view.PostEditor();
                    DataView dv = (DataView)view.DataSource;
                    return C2M.SelectedParamEntityDataView(dv, "check");
                }
                else
                {
                    return null;
                }
            }
            else
            {
                DataView dv = new DataView();
                return dv;
            }
           
        }

        private void gridView1_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView4_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView5_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}