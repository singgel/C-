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
using FuelDataSysClient.Form_DBManager;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Compare
{
    public partial class CertificateComparisonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        DataTable dtCertifi = new DataTable(); //合格证数据

        public CertificateComparisonForm()
        {
            InitializeComponent();
        }

        private void CertificateComparisonForm_Load(object sender, EventArgs e)
        {
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
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
                this.gcDataXT.DataSource = null;
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
                    this.gcDataXT.DataSource = dt;
                    this.gvDataXT.BestFitColumns();
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
            this.gcDataHGZ.DataSource = null;
            this.groupBox2.Text = "合格证数据";
            if (!Utils.IsFuelTest)   //测试用户 测试数据
            {
                this.gcDataHGZ.DataSource = DataFormat(CertificateData());
                dtCertifi = CertificateData();
                dtCertifi.Columns["VIN"].ColumnName = "VIN";
                dtCertifi.Columns["CLXH"].ColumnName = "车辆型号";
                dtCertifi.Columns["CLZZRQ"].ColumnName = "车辆制造日期/进口日期";
                dtCertifi.Columns["RLLX"].ColumnName = "燃料类型";
            }
            else
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    DataSet ds = new DataSet();
                    if (Utils.userId.Equals("FADCCSHDZU001"))
                    {
                        DataSet ds1 = Utils.serviceCertificate.QueryCertificate(Utils.userId, Utils.password, "上海大众汽车有限公司", dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                        DataSet ds2 = Utils.serviceCertificate.QueryCertificate(Utils.userId, Utils.password, "上汽大众汽车有限公司", dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
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
                        ds = Utils.serviceCertificate.QueryCertificate(Utils.userId, Utils.password, "大众汽车(中国)销售有限公司", dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                    }
                    else
                    {
                        ds = Utils.serviceCertificate.QueryCertificate(Utils.userId, Utils.password, Utils.qymc, dtStartTime.Text, DateTime.Parse(dtEndTime.Text).AddDays(1).ToString(), string.Empty);
                    }

                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        dtCertifi = ds.Tables[0];
                        this.gcDataHGZ.DataSource = DataFormat(ds.Tables[0]);
                        this.gvDataHGZ.BestFitColumns();
                        this.groupBox2.Text = String.Format("合格证数据（共{0}条）", ds.Tables[0].Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("无合格证数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        //全部选中
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
                GridControlHelper.SelectItem(gridview, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //取消全选
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
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

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    ExportToExcel toExcel = new ExportToExcel();
                    DataSet ds = new DataSet();
                    if (gcSupplements.DataSource != null)
                    {
                        DataTable dt1 = (DataTable)gcSupplements.DataSource;
                        DataTable dtc1 = dt1.Copy();
                        dtc1.TableName = "补传数据";
                        dtc1.Columns.Remove(dtc1.Columns[0]);
                        ds.Tables.Add(dtc1);
                    }
                    if (gcRecall.DataSource != null)
                    {
                        DataTable dt2 = (DataTable)gcRecall.DataSource;
                        DataTable dtc2 = dt2.Copy();
                        dtc2.TableName = "撤销数据";
                        dtc2.Columns.Remove(dtc2.Columns[0]);
                        ds.Tables.Add(dtc2);
                    }
                    toExcel.ToExcelSheet(ds, saveFileDialog.FileName);
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
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
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fuelData = (DataTable)this.gcDataXT.DataSource;
            var CertificateData = dtCertifi;
            if (fuelData == null || CertificateData == null)
            {
                MessageBox.Show("没有需要比较的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
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
                dtRecall.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dtRecall.Columns["check"].SetOrdinal(0);
                this.gcSupplements.DataSource = dtSupplement;
                this.gvSupplements.BestFitColumns();
                this.gcRecall.DataSource = dtRecall;
                this.gvRecall.BestFitColumns();
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
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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