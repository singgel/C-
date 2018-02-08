using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using FuelDataSysClient.Tool;
using Common;
using FuelDataSysClient.FuelCafc;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Form_BGSC
{
    public partial class PreReportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        Dictionary<int, string> dictInit = new Dictionary<int, string>();
        CafcService.CafcWebService cafcService = Utils.serviceCafc;
        private const string forecast = "\\ExcelHeaderTemplate\\forecast.doc";
        
        public PreReportForm()
        {
            InitializeComponent();
            InitDict();
        }

        private void InitDict()
        {
            dictInit.Add(2012, "1.09");
            dictInit.Add(2013, "1.06");
            dictInit.Add(2014, "1.03");
            dictInit.Add(2015, "1.00");
            dictInit.Add(2016, "1.34");
            dictInit.Add(2017, "1.28");
            dictInit.Add(2018, "1.20");
            dictInit.Add(2019, "1.10");
            dictInit.Add(2020, "1.00");
        }

        private void PreReportForm_Load(object sender, EventArgs e)
        {
            for (int i = 2013; i <= 2020; i++)
            {
                comboBoxEdit1.Properties.Items.Add(i);
            }
            comboBoxEdit1.Text = DateTime.Now.Year.ToString();
        }

        private void btnImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel|*.xls;*.xlsx|All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    MitsUtils miutils = new MitsUtils();
                    DataSet ds = miutils.ReadExcel(ofd.FileName, "Sheet1");

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        dt.Columns["产品型号"].ColumnName = "CLXH";
                        dt.Columns["燃料种类"].ColumnName = "RLLX";
                        dt.Columns["变速器型式"].ColumnName = "BSQXS";
                        dt.Columns["整备质量"].ColumnName = "ZCZBZL";
                        dt.Columns["座椅排数"].ColumnName = "ZWPS";
                        dt.Columns["纯电动驱动模式综合工况续驶里程"].ColumnName = "ZHGKXSLC";
                        dt.Columns["燃料消耗量（综合）"].ColumnName = "ZHGKRLXHL";
                        dt.Columns["预计制造/进口量"].ColumnName = "SL";
                        DataTableHelper.removeEmpty(dt);
                        cafcService.DeleteForeCast(Utils.userId, Utils.password, "prj_" + Utils.userId);
                        if (newProject())
                        {
                            if (newCarType(dt))
                            {
                                this.gcParam.DataSource = null;
                                this.gcParam.DataSource = dt;
                                this.gcCafc.DataSource = null;
                                MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("数据为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
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
        }

        private void btnProduce_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable dt = (DataTable)this.gcParam.DataSource;
            if (dt == null) { return; }
            if (dt.Rows.Count > 0)
            {
                try
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string path = fbd.SelectedPath;
                        List<CafcService.FuelCafcAndTcafc> cafcData = new List<CafcService.FuelCafcAndTcafc>();
                        if (Convert.ToInt32(this.comboBoxEdit1.Text) > 2015)
                        {
                            var data = cafcService.QueryForeCastNECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                            if (data != null && data.Length > 0)
                            {
                                cafcData = data.ToList();
                            }
                        }
                        else
                        {
                            var data = cafcService.QueryForeCastTECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                            if (data != null && data.Length > 0)
                            {
                                cafcData = data.ToList();
                            }
                        }
                        if (cafcData.Count > 0)
                        {
                            Dictionary<string, string> datas = new Dictionary<string, string>();
                            datas.Add("{qymc}", Utils.qymc);
                            datas.Add("{year}", comboBoxEdit1.Text);
                            datas.Add("{count}", cafcData[0].Sl_act.ToString());
                            datas.Add("{tcafc}", cafcData[0].Tcafc.ToString());
                            datas.Add("{cafc}", cafcData[0].Cafc.ToString());

                            int year = Int32.Parse(comboBoxEdit1.Text);
                            decimal ttcafc = Math.Round(cafcData[0].Tcafc * Convert.ToDecimal(dictInit[year]), 2, MidpointRounding.AwayFromZero);
                            decimal tt = (ttcafc - cafcData[0].Cafc);
                            decimal percent = Math.Round(tt / ttcafc, 10, MidpointRounding.AwayFromZero);
                            //decimal percent =  Math.Round((cafcData[0].Tcafc - cafcData[0].Cafc) / cafcData[0].Tcafc,6);
                            percent = percent * 100;
                            percent = Math.Round(percent, 1, MidpointRounding.AwayFromZero);
                            datas.Add("{percent}", percent.ToString());
                            InitRead(forecast, datas, path);
                            MessageBox.Show("生成成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    MessageBox.Show("生成失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                MessageBox.Show("请导入预测车型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTeSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            DataTable dt = (DataTable)this.gcParam.DataSource;
            if (dt == null)
            {
                MessageBox.Show("请先导入预测数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dt.Rows.Count > 0)
            {
                this.DataBinds();
            }
            else
            {
                MessageBox.Show("请先导入预测数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool newProject()
        {
            string preTime = String.Format("{0}/{1}/{2}", this.comboBoxEdit1.Text, DateTime.Now.Month, DateTime.Now.Day);
            CafcService.ForecastPrj prjObj = new CafcService.ForecastPrj() 
            { 
                Prj_Id = "prj_" + Utils.userId, 
                StartTime = Convert.ToDateTime(preTime), 
                EndTime = Convert.ToDateTime(preTime) 
            };
            return cafcService.SaveForecastPrj(Utils.userId, Utils.password, prjObj);
        }

        private bool newCarType(DataTable _dt)
        {
            DataTable dt = _dt;
            //var dataArr = (from d in dt.AsEnumerable()
            //            select new CafcService.ForecastParam
            //            {
            //                Prj_Id = "prj_" + Utils.userId,
            //                Qcscqy = Utils.qymc,
            //                Clxh = d.Field<string>("CLXH"),
            //                Rllx = d.Field<string>("RLLX"),
            //                Bsqxs = d.Field<string>("BSQXS"),
            //                Zwps = string.IsNullOrEmpty(d.Field<string>("ZWPS")) ? 0 : Convert.ToInt32(d.Field<string>("ZWPS")),
            //                Zczbzl = string.IsNullOrEmpty(d.Field<string>("ZCZBZL")) ? 0 : Convert.ToInt32(d.Field<string>("ZCZBZL")),
            //                Zhgkxslc = string.IsNullOrEmpty(d.Field<string>("ZHGKXSLC")) ? 0 : Convert.ToDecimal(d.Field<string>("ZHGKXSLC")),
            //                Zhgkrlxhl = string.IsNullOrEmpty(d.Field<string>("ZHGKRLXHL")) ? 0 : Convert.ToDecimal(d.Field<string>("ZHGKRLXHL")),
            //                Sl = string.IsNullOrEmpty(d.Field<string>("SL")) ? 0 : Convert.ToInt32(d.Field<string>("SL")),
            //            }).ToArray();
            List<CafcService.ForecastParam> dataList = new List<CafcService.ForecastParam>();
            foreach(DataRow dr in dt.Rows)
            {
                CafcService.ForecastParam obj = new CafcService.ForecastParam();
                obj.Prj_Id = "prj_"+Utils.userId;
                obj.Qcscqy = Utils.qymc;
                obj.Clxh = string.IsNullOrEmpty(dr["CLXH"].ToString()) ? "" : dr["CLXH"].ToString();
                obj.Rllx = string.IsNullOrEmpty(dr["RLLX"].ToString()) ? "" : dr["RLLX"].ToString();
                obj.Bsqxs = string.IsNullOrEmpty(dr["BSQXS"].ToString()) ? "" : dr["BSQXS"].ToString();
                obj.Zwps = string.IsNullOrEmpty(dr["ZWPS"].ToString()) ? 0 : Convert.ToInt32(dr["ZWPS"]);
                obj.Zczbzl = string.IsNullOrEmpty(dr["ZCZBZL"].ToString()) ? 0 : Convert.ToInt32(dr["ZCZBZL"]);
                obj.Zhgkxslc = string.IsNullOrEmpty(dr["ZHGKXSLC"].ToString()) ? 0 : Convert.ToDecimal(dr["ZHGKXSLC"]);
                obj.Zhgkrlxhl = string.IsNullOrEmpty(dr["ZHGKRLXHL"].ToString()) ? 0 : Convert.ToDecimal(dr["ZHGKRLXHL"]);
                obj.Sl = string.IsNullOrEmpty(dr["SL"].ToString()) ? 0 : Convert.ToInt32(dr["SL"]);
                dataList.Add(obj);
            }
            return cafcService.SaveForecastParam(Utils.userId, Utils.password, dataList.ToArray());
        }

        private void DataBinds()
        {
            try
            {
                CafcService.FuelCafcAndTcafc[] neCafcData = cafcService.QueryForeCastNECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                CafcService.FuelCafcAndTcafc[] teCafcData = cafcService.QueryForeCastTECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                CafcModel cafc = new CafcModel(); 
                if (neCafcData != null && neCafcData.Length > 0)
                {
                    cafc.ne_Sl_act = neCafcData[0].Sl_act;
                    cafc.ne_Sl_hs = neCafcData[0].Sl_hs;
                    cafc.ne_Cafc = neCafcData[0].Cafc;
                    cafc.ne_Tcafc = neCafcData[0].Tcafc;
                }
                if (teCafcData != null && teCafcData.Length > 0)
                {
                    cafc.te_Sl_act = teCafcData[0].Sl_act;
                    cafc.te_Sl_hs = teCafcData[0].Sl_hs;
                    cafc.te_Cafc = teCafcData[0].Cafc;
                    cafc.te_Tcafc = teCafcData[0].Tcafc;
                }
                this.gcCafc.DataSource = new CafcModel[]{cafc};
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)this.gcParam.DataSource;
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    cafcService.DelForecastPrj(Utils.userId, Utils.password, new string[] { "prj_" + Utils.userId });
                    newProject();
                    newCarType(dt);
                }
            }
        }

        private string InitRead(string Template, Dictionary<string, string> datas, string filePath)
        {
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            //将要导出的新word文件名
            string physicNewFile = "预报告.doc";
            try
            {
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                app.Visible = false;
                object fileName = System.Windows.Forms.Application.StartupPath + Template;//Year1;//模板文件
                //打开模板文件
                object oMissing = System.Reflection.Missing.Value;
                doc = app.Documents.Open(ref fileName,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);



                object replace = Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll;
                foreach (var item in datas)
                {
                    app.Selection.Find.Replacement.ClearFormatting();
                    app.Selection.Find.ClearFormatting();
                    app.Selection.Find.Text = item.Key;//需要被替换的文本
                    app.Selection.Find.Replacement.Text = item.Value;//替换文本 

                    //执行替换操作
                    app.Selection.Find.Execute(
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing, ref oMissing,
                    ref oMissing, ref replace,
                    ref oMissing, ref oMissing,
                    ref oMissing, ref oMissing);
                }


                //对替换好的word模板另存为一个新的word文档
                doc.SaveAs(String.Format(@"{0}\{1}", filePath, physicNewFile),
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                //准备导出word

            }
            catch (System.Threading.ThreadAbortException ex)
            {
                //这边为了捕获Response.End引起的异常
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (doc != null)
                {
                    doc.Close();//关闭word文档
                }
                if (app != null)
                {
                    app.Quit();//退出word应用程序
                }
            }
            return physicNewFile;
        }

        public class CafcModel
        {

            public int te_Sl_act{get;set;}

            public int te_Sl_hs{get;set;}

            public decimal te_Cafc{get;set;}

            public decimal te_Tcafc{get;set;}
                            
            public int ne_Sl_act{get;set;}

            public int ne_Sl_hs{get;set;}

            public decimal ne_Cafc{get;set;}

            public decimal ne_Tcafc{get;set;}                 
                             
        }
    }
}