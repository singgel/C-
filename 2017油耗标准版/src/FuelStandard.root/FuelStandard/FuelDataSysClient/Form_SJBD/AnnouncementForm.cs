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

namespace FuelDataSysClient.CertificateService
{
    public partial class AnnouncementForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CertificateService.CertificateComparison ccf = Utils.serviceCertificate;
        InitDataTime initTime = new InitDataTime();

        public AnnouncementForm()
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
                if (string.IsNullOrEmpty(this.dtStartTime.Text) || string.IsNullOrEmpty(this.dtEndTime.Text))
                {
                    MessageBox.Show("请选择比对的日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    StringBuilder strSql = new StringBuilder();
                    if (Utils.userId.Equals("FADCFFTZGU001"))
                    {
                        strSql.Append(String.Format("select CLXH,RLLX,ZCZBZL,CT_ZHGKRLXHL as ZHGK from VIEW_T_ALL_ADC where USER_ID='{0}' and cdate(Format(CLZZRQ,'yyyy/mm/dd')) between #{1}# and #{2}# group by CLXH,RLLX,ZCZBZL,CT_ZHGKRLXHL", Utils.userId, this.dtStartTime.Text, this.dtEndTime.Text));
                        strSql.Append(" UNION ");
                        strSql.Append(String.Format("select CLXH,RLLX,ZCZBZL,FCDS_HHDL_ZHGKRLXHL as ZHGK from VIEW_T_ALL_FCDS_ADC where USER_ID='{0}' and cdate(Format(CLZZRQ,'yyyy/mm/dd')) between #{1}# and #{2}# group by CLXH,RLLX,ZCZBZL,FCDS_HHDL_ZHGKRLXHL", Utils.userId, this.dtStartTime.Text, this.dtEndTime.Text));
                    }
                    else
                    {
                        strSql.Append(String.Format("select CLXH,RLLX,ZCZBZL,CT_ZHGKRLXHL as ZHGK from VIEW_T_ALL where USER_ID='{0}' and cdate(Format(CLZZRQ,'yyyy/mm/dd')) between #{1}# and #{2}# group by CLXH,RLLX,ZCZBZL,CT_ZHGKRLXHL", Utils.userId, this.dtStartTime.Text, this.dtEndTime.Text));
                        strSql.Append(" UNION ");
                        strSql.Append(String.Format("select CLXH,RLLX,ZCZBZL,CDS_HHDL_ZHGKRLXHL as ZHGK from VIEW_T_ALL_CDS where USER_ID='{0}' and cdate(Format(CLZZRQ,'yyyy/mm/dd')) between #{1}# and #{2}# group by CLXH,RLLX,ZCZBZL,CDS_HHDL_ZHGKRLXHL", Utils.userId, this.dtStartTime.Text, this.dtEndTime.Text));
                        strSql.Append(" UNION ");
                        strSql.Append(String.Format("select CLXH,RLLX,ZCZBZL,FCDS_HHDL_ZHGKRLXHL as ZHGK from VIEW_T_ALL_FCDS where USER_ID='{0}' and cdate(Format(CLZZRQ,'yyyy/mm/dd')) between #{1}# and #{2}# group by CLXH,RLLX,ZCZBZL,FCDS_HHDL_ZHGKRLXHL", Utils.userId, this.dtStartTime.Text, this.dtEndTime.Text));
                        strSql.Append(" UNION ");
                        strSql.Append(String.Format("select CLXH,RLLX,ZCZBZL,RLDC_ZHGKHQL as ZHGK from VIEW_T_ALL_RLDC where USER_ID='{0}' and cdate(Format(CLZZRQ,'yyyy/mm/dd')) between #{1}# and #{2}# group by CLXH,RLLX,ZCZBZL,RLDC_ZHGKHQL", Utils.userId, this.dtStartTime.Text, this.dtEndTime.Text));
                    }
                    var ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, strSql.ToString(), null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0].Copy();
                        //去除5字头的车型
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string str_Clxh = dt.Rows[i]["clxh"].ToString();
                            var flg = Regex.Replace(str_Clxh, @"[^\d.]*", "");
                            if ((!string.IsNullOrEmpty(flg) && flg.Substring(0, 1).Equals("5")) || string.IsNullOrEmpty(dt.Rows[i]["CLXH"].ToString()) || string.IsNullOrEmpty(dt.Rows[i]["RLLX"].ToString()) || string.IsNullOrEmpty(dt.Rows[i]["ZCZBZL"].ToString()) || string.IsNullOrEmpty(dt.Rows[i]["ZHGK"].ToString()))
                            {
                                dt.Rows[i].Delete();
                            }
                        }
                        dt.AcceptChanges();
                        this.gridControl1.DataSource = dt;
                        this.groupBox1.Text = String.Format("系统油耗数据（共{0}条车型数据）", dt.Rows.Count);
                    }
                    else
                    {
                        MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("查询超时,请重试", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "通告数据";
            if (string.IsNullOrEmpty(dtStartTime.Text) || string.IsNullOrEmpty(dtEndTime.Text))
            {
                MessageBox.Show("请选择比对的日期", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataSet ds = new DataSet();
                if ("FADCFJBLHU001".Equals(Utils.userId))
                {
                    DataSet ds1 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "英国捷豹汽车公司布朗维奇堡工厂", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds2 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "英国捷豹路虎有限公司布朗维奇堡工厂", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds3 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "英国捷豹路虎有限公司哈利伍德工厂", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds4 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "英国捷豹路虎有限公司索利赫尔工厂", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    if (ds1 != null)
                    {
                        ds.Merge(ds1, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds2 != null)
                    {
                        ds.Merge(ds2, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds3 != null)
                    {
                        ds.Merge(ds3, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds4 != null)
                    {
                        ds.Merge(ds4, true, MissingSchemaAction.AddWithKey);
                    }
                }
                else if ("FADCCSHDZU001".Equals(Utils.userId))
                {
                    DataSet ds1 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "上海大众汽车有限公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds2 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "上汽大众汽车有限公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    if (ds1 != null)
                    {
                        ds.Merge(ds1, true, MissingSchemaAction.AddWithKey); 
                    }
                    if (ds2 != null)
                    {
                        ds.Merge(ds2, true, MissingSchemaAction.AddWithKey);
                    }
                }
                else if ("FADCFBMZGU001".Equals(Utils.userId))
                {
                    ds = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "德国宝马汽车集团", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                }
                else if ("FADCFBSJ0U001".Equals(Utils.userId))
                {
                    ds = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "保时捷股份公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                }
                else if ("FADCFFTZGU001".Equals(Utils.userId))
                {
                    DataSet ds1 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "丰田汽车公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds2 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "日本丰田汽车公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    if (ds1 != null)
                    {
                        ds.Merge(ds1, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds2 != null)
                    {
                        ds.Merge(ds2, true, MissingSchemaAction.AddWithKey);
                    }
                }
                else if ("FADCFCHRYU001".Equals(Utils.userId))
                {
                    ds = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "克莱斯勒集团有限责任公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                }
                else if ("FADCFRCZGU001".Equals(Utils.userId))
                {
                    DataSet ds1 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "日产汽车有限公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds2 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "日产汽车公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds3 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "北美日产公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    DataSet ds4 = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "日产汽车制造（英国）有限公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                    if (ds1 != null)
                    {
                        ds.Merge(ds1, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds2 != null)
                    {
                        ds.Merge(ds2, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds3 != null)
                    {
                        ds.Merge(ds3, true, MissingSchemaAction.AddWithKey);
                    }
                    if (ds4 != null)
                    {
                        ds.Merge(ds4, true, MissingSchemaAction.AddWithKey);
                    }
                }
                else if ("FADCFDZXSU001".Equals(Utils.userId))
                {
                    ds = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, "大众汽车股份公司", "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                }
                else
                {
                    ds = ccf.QueryNoticeByQymc(Utils.userId, Utils.password, Utils.qymc, "2010/1/1", DateTime.Now.ToString("yyyy/MM/dd"));
                }
                
                if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
                {
                    DataTable dt = ds.Tables[0].Copy();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["ZHGK"] = String.Format("{0:N1}", Convert.ToDecimal(dt.Rows[i]["ZHGK"]));
                    }
                    this.gridControl2.DataSource = dt;
                    this.groupBox2.Text = String.Format("通告数据（共{0}条车型数据）", ds.Tables[0].Rows.Count);
                }
                else
                {
                    MessageBox.Show("通告数据不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询超时或当前查询条件数据不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            //获取要比对的数据
            DataTable dataTable_yh = (DataTable)gridControl1.DataSource;
            DataTable dataTable_tg = (DataTable)gridControl2.DataSource;
            //是否有必要比
            if (dataTable_yh == null || dataTable_tg == null)
            {
                MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //整备质量不一致
            var zczbzlData = (from d in dataTable_yh.AsEnumerable()
                             join dd in dataTable_tg.AsEnumerable()
                             on d.Field<string>("CLXH") equals dd.Field<string>("CLXH")
                             where d.Field<string>("RLLX") == dd.Field<string>("RLLX")
                             && dd.Field<string>("ZCZBZL") != d.Field<string>("ZCZBZL")
                             select new
                             {
                                 //VIN = d.Field<string>("VIN"),
                                 CLXH = d.Field<string>("CLXH"),
                                 RLLX = d.Field<string>("RLLX"),
                                 YH_ZCZBZL = d.Field<string>("ZCZBZL"),
                                 TG_ZCZBZL = dd.Field<string>("ZCZBZL")
                             }).ToList();
            //去除一种车型多种整备质量
            for (int i = zczbzlData.Count - 1; i >= 0; i--)
            {
                string clxh = zczbzlData[i].CLXH;
                string rllx = zczbzlData[i].RLLX;
                string zczbzl = zczbzlData[i].YH_ZCZBZL;
                var flg = (from d in dataTable_tg.AsEnumerable()
                           where clxh == d.Field<string>("CLXH")
                           && rllx == d.Field<string>("RLLX")
                           && zczbzl == d.Field<string>("ZCZBZL")
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
            //油耗实际值不一致
            var zhgkData = (from d in dataTable_yh.AsEnumerable()
                           join dd in dataTable_tg.AsEnumerable()
                           on d.Field<string>("CLXH") equals dd.Field<string>("CLXH")
                           where d.Field<string>("RLLX") == dd.Field<string>("RLLX")
                           && dd.Field<string>("ZHGK") != d.Field<string>("ZHGK")
                           select new
                           {
                               //VIN = d.Field<string>("VIN"),
                               CLXH = d.Field<string>("CLXH"),
                               RLLX = d.Field<string>("RLLX"),
                               YH_ZHGK = d.Field<string>("ZHGK"),
                               TG_ZHGK = dd.Field<string>("ZHGK")
                           }).ToList();
            //去除一种车型多种油耗数据
            for (int i = zhgkData.Count - 1; i >= 0; i--)
            {
                string clxh = zhgkData[i].CLXH;
                string rllx = zhgkData[i].RLLX;
                string zhgk = zhgkData[i].YH_ZHGK;
                var flg = (from d in dataTable_tg.AsEnumerable()
                           where clxh == d.Field<string>("CLXH")
                           && rllx == d.Field<string>("RLLX")
                           && zhgk == d.Field<string>("ZHGK")
                           select new
                           {
                               CLXH = d.Field<string>("CLXH"),
                               RLLX = d.Field<string>("RLLX"),
                               ZHGK = d.Field<string>("ZHGK")
                           }).ToList();
                if (flg.Count > 0)
                {
                    zhgkData.RemoveAt(i);
                }
            }
            this.gcTable2.DataSource = zhgkData.Distinct().ToList();
            //通告中不存在
            var cllxArr = dataTable_tg.AsEnumerable().Select(d => d.Field<string>("CLXH")).Distinct().ToArray();
            var rllxArr = dataTable_tg.AsEnumerable().Select(d => d.Field<string>("RLLX")).Distinct().ToArray();
            var errorData = from d in dataTable_yh.AsEnumerable()
                            where !cllxArr.Contains(d.Field<string>("CLXH"))
                            || !rllxArr.Contains(d.Field<string>("RLLX"))
                            select new
                            {
                                //VIN = d.Field<string>("VIN"),
                                CLXH = d.Field<string>("CLXH"),
                                RLLX = d.Field<string>("RLLX")
                            };
            this.gcTable3.DataSource = errorData.Distinct().ToList();
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

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                
                CompositeLink complink = new CompositeLink(new PrintingSystem());
                PrintableComponentLink linkTable1 = new PrintableComponentLink();
                PrintableComponentLink linkTable2 = new PrintableComponentLink();
                PrintableComponentLink linkDiff = new PrintableComponentLink();

                linkTable1.Component = gcTable1;
                complink.Links.Add(linkTable1);

                linkTable2.Component = gcTable2;
                complink.Links.Add(linkTable2);

                linkDiff.Component = gcTable3;
                complink.Links.Add(linkDiff);
                if (gcTable1.MainView.RowCount == 0 && gcTable2.MainView.RowCount == 0 && gcTable3.MainView.RowCount == 0)
                {
                    return;
                }
                else
                {
                    complink.CreatePageForEachLink();

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = "通告比对结果";
                    DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                    if (dialogResult == DialogResult.OK)
                    {
                        XlsxExportOptions option = new XlsxExportOptions();
                        option.ExportMode = XlsxExportMode.SingleFilePageByPage;

                        complink.ExportToXlsx(saveFileDialog.FileName, option);
                        MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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