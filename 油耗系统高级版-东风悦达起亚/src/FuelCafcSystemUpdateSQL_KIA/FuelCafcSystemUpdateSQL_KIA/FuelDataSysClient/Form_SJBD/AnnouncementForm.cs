using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Common;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Model;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Form_SJBD
{
    public partial class AnnouncementForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        readonly CertificateService.CertificateComparison ccf = Utils.serviceCertificate;

        public AnnouncementForm()
        {
            InitializeComponent();
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //查询官方油耗数据
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 0;
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
                if (string.IsNullOrEmpty(this.dtStartTime.Text) || string.IsNullOrEmpty(this.dtEndTime.Text))
                {
                    MessageBox.Show("请选择比对的日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    StringBuilder strSql = new StringBuilder();
                    strSql.AppendFormat("select CLXH,RLLX,ZCZBZL,CT_ZHGKRLXHL as ZHGK from ADC_T_ALL where to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >=to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <=to_date('{1}','yyyy-mm-dd hh24:mi:ss') group by CLXH,RLLX,ZCZBZL,CT_ZHGKRLXHL", this.dtStartTime.Text, this.dtEndTime.Text);
                    strSql.Append(" UNION ");
                    strSql.AppendFormat("select CLXH,RLLX,ZCZBZL,CDS_HHDL_ZHGKRLXHL as ZHGK from ADC_T_ALL_CDS where to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >=to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <=to_date('{1}','yyyy-mm-dd hh24:mi:ss') group by CLXH,RLLX,ZCZBZL,CDS_HHDL_ZHGKRLXHL", this.dtStartTime.Text, this.dtEndTime.Text);
                    strSql.Append(" UNION ");
                    strSql.AppendFormat("select CLXH,RLLX,ZCZBZL,FCDS_HHDL_ZHGKRLXHL as ZHGK from ADC_T_ALL_FCDS where to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >=to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <=to_date('{1}','yyyy-mm-dd hh24:mi:ss') group by CLXH,RLLX,ZCZBZL,FCDS_HHDL_ZHGKRLXHL", this.dtStartTime.Text, this.dtEndTime.Text);
                    strSql.Append(" UNION ");
                    strSql.AppendFormat("select CLXH,RLLX,ZCZBZL,RLDC_ZHGKHQL as ZHGK from ADC_T_ALL_RLDC where to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >=to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <=to_date('{1}','yyyy-mm-dd hh24:mi:ss') group by CLXH,RLLX,ZCZBZL,RLDC_ZHGKHQL", this.dtStartTime.Text, this.dtEndTime.Text);
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, strSql.ToString(), null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0].Copy();
                        //去除5字头的车型
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string str_Clxh = dt.Rows[i]["clxh"].ToString();
                            var flg = Regex.Replace(str_Clxh, @"[^\d.]*", "");
                            if (!string.IsNullOrEmpty(flg) && flg.Substring(0, 1).Equals("5"))
                            {
                                dt.Rows[i].Delete();
                            }
                        }
                        dt.AcceptChanges();
                        this.gridControl1.DataSource = dt;
                        this.gridView1.BestFitColumns();
                        this.groupBox1.Text = String.Format("系统油耗数据（共{0}条车型数据）", ds.Tables[0].Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        //查询通告数据
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.xtraTabControl1.SelectedTabPageIndex = 0;
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "通告数据";
            if (string.IsNullOrEmpty(dtStartTime.Text) || string.IsNullOrEmpty(dtEndTime.Text))
            {
                MessageBox.Show("请选择比对的日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //测试用户 测试数据
                if (!Utils.IsFuelTest)
                {
                    this.gridControl2.DataSource = DataSourceHelper.AnnouncementData();
                }
                else
                {
                    var ds = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, Utils.qymc, "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                    {
                        DataTable dt = ds.Tables[0].Copy();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            dt.Rows[i]["ZHGK"] = String.Format("{0:N1}", Convert.ToDecimal(dt.Rows[i]["ZHGK"]));
                        }
                        this.gridControl2.DataSource = dt;
                        this.gridView2.BestFitColumns();
                        this.groupBox2.Text = String.Format("通告数据（共{0}条车型数据）", ds.Tables[0].Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("该时间段内通告数据不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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

        //数据比对
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                this.gcTable1.DataSource = null;
                this.gcTable2.DataSource = null;
                this.gcTable3.DataSource = null;
                //获取要比对的数据
                DataTable dataTable_yh = (DataTable)gridControl1.DataSource;
                DataTable dataTable_tg = (DataTable)gridControl2.DataSource;
                //是否有必要比
                if (dataTable_yh == null || dataTable_yh == null)
                {
                    MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //整车整备质量不一致
                var zczbzlData = (from d in dataTable_yh.AsEnumerable()
                                  join dd in dataTable_tg.AsEnumerable()
                                  on d.Field<string>("CLXH") equals dd.Field<string>("CLXH")
                                  where d.Field<string>("RLLX") == dd.Field<string>("RLLX")
                                  && dd.Field<string>("ZCZBZL") != d.Field<string>("ZCZBZL")
                                  select new
                                  {
                                      CLXH = d.Field<string>("CLXH"),
                                      RLLX = d.Field<string>("RLLX"),
                                      YH_ZCZBZL = d.Field<string>("ZCZBZL"),
                                      TG_ZCZBZL = dd.Field<string>("ZCZBZL")
                                  }).ToList();
                //去除一种车型多种整备质量
                for (int i = zczbzlData.Count - 1; i >= 0; i--)
                {
                    string clxh = dataTable_yh.Rows[i]["CLXH"].ToString();
                    string rllx = dataTable_yh.Rows[i]["RLLX"].ToString();
                    string zczbzl = dataTable_yh.Rows[i]["ZCZBZL"].ToString();
                    var flg = (from d in dataTable_tg.AsEnumerable()
                               where clxh.Contains(d.Field<string>("CLXH"))
                               && rllx.Contains(d.Field<string>("RLLX"))
                               && zczbzl.Contains(d.Field<string>("ZCZBZL"))
                               select new
                               {
                                   CLXH = d.Field<string>("CLXH"),
                                   RLLX = d.Field<string>("RLLX"),
                                   ZCZBZL = d.Field<string>("ZCZBZL")
                               }).ToList();
                    if (flg.Count > 0)
                    {
                        zczbzlData.RemoveAt(i);
                    }
                }
                this.gcTable1.DataSource = zczbzlData.Distinct().ToList();
                this.gvTable1.BestFitColumns();
                //油耗实际值不一致
                var zhgkData = (from d in dataTable_yh.AsEnumerable()
                                join dd in dataTable_tg.AsEnumerable()
                                on d.Field<string>("CLXH") equals dd.Field<string>("CLXH")
                                where d.Field<string>("RLLX") == dd.Field<string>("RLLX")
                                && dd.Field<string>("ZHGK") != d.Field<string>("ZHGK")
                                select new
                                {
                                    CLXH = d.Field<string>("CLXH"),
                                    RLLX = d.Field<string>("RLLX"),
                                    YH_ZHGK = d.Field<string>("ZHGK"),
                                    TG_ZHGK = dd.Field<string>("ZHGK"),
                                }).ToList();
                //去除一种车型多种油耗数据
                for (int i = zhgkData.Count - 1; i >= 0; i--)
                {
                    string clxh = dataTable_yh.Rows[i]["CLXH"].ToString();
                    string rllx = dataTable_yh.Rows[i]["RLLX"].ToString();
                    string zhgk = dataTable_yh.Rows[i]["ZHGK"].ToString();
                    var flg = (from d in dataTable_tg.AsEnumerable()
                               where clxh.Contains(d.Field<string>("CLXH"))
                               && rllx.Contains(d.Field<string>("RLLX"))
                               && zhgk.Contains(d.Field<string>("ZHGK"))
                               select new
                               {
                                   CLXH = d.Field<string>("CLXH"),
                                   RLLX = d.Field<string>("RLLX"),
                                   ZHGK = d.Field<string>("ZHGK"),
                               }).ToList();
                    if (flg.Count > 0)
                    {
                        zhgkData.RemoveAt(i);
                    }
                }
                this.gcTable2.DataSource = zhgkData.Distinct().ToList();
                this.gvTable2.BestFitColumns();
                //通告中不存在
                var cllxArr = dataTable_tg.AsEnumerable().Select(d => d.Field<string>("CLXH")).Distinct().ToArray();
                var rllxArr = dataTable_tg.AsEnumerable().Select(d => d.Field<string>("RLLX")).Distinct().ToArray();
                var errorData = from d in dataTable_yh.AsEnumerable()
                                where !cllxArr.Contains(d.Field<string>("CLXH"))
                                || !rllxArr.Contains(d.Field<string>("RLLX"))
                                select new
                                {
                                    CLXH = d.Field<string>("CLXH"),
                                    RLLX = d.Field<string>("RLLX"),
                                };
                this.gcTable3.DataSource = errorData.Distinct().ToList();
                this.gvTable3.BestFitColumns();
                //比较完初始选中的tab页
                int selectTabIndex = 1;
                if (zczbzlData.Count() < 1)
                {
                    selectTabIndex = 2;
                    if (zhgkData.Count() < 1)
                    {
                        selectTabIndex = 3;
                        if (errorData.Count() < 1)
                        {
                            MessageBox.Show("数据一致！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                xtraTabControl1.SelectedTabPageIndex = selectTabIndex;
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

        //导出到Excel
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (gcTable1.DataSource == null && gcTable2.DataSource == null && gcTable3.DataSource == null)
            {
                MessageBox.Show("没有比对结果", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = "通告数据比对结果",
                Title = "导出Excel",
                Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls"
            };
            DialogResult dialogResult = saveFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    ExportHelper.ExportToExcel(saveFileDialog.FileName, true, String.Empty, gcTable1, gcTable2, gcTable3);
                    ExcelHelper excelBuilder = new ExcelHelper(saveFileDialog.FileName);
                    excelBuilder.ChangeNameWorkSheet("Sheet1", "整备质量不一致");
                    excelBuilder.ChangeNameWorkSheet("Sheet2", "油耗实际值不一致");
                    excelBuilder.ChangeNameWorkSheet("Sheet3", "查找不到车型");
                    excelBuilder.DeleteRows(1, 1);
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

        private void gridView3_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
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