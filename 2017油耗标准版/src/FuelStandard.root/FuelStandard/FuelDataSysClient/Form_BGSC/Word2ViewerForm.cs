using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Microsoft.Office.Interop;
using DevExpress.XtraRichEdit;
using System.IO;
using FuelDataSysClient.FuelCafc;
using DevExpress.XtraRichEdit.API.Native;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using Common;
using FuelDataSysClient.Tool;
using System.Threading;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient
{
    public partial class Word2ViewerForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;

        private const string Year2 = "\\ExcelHeaderTemplate\\Year2.doc";
        private  string test = System.Windows.Forms.Application.StartupPath + "\\ExcelHeaderTemplate\\test.doc";
        public Word2ViewerForm()
        {
            InitializeComponent();
            initComboBox();
            this.richEditControl1.InitializeDocument += new EventHandler(richEditControl1_InitializeDocument);
            if (!Utils.IsFuelTest)
            {
                comboBoxEdit1.Enabled = false;
                barButtonItem2.Enabled = true;
                this.richEditControl1.LoadDocument(test, DocumentFormat.Doc);
            }

        }

        void richEditControl1_InitializeDocument(object sender, EventArgs e)
        {
            this.InitializeDocument((RichEditControl)sender);
        }


        private void InitializeDocument(RichEditControl control)
        {
            DevExpress.XtraRichEdit.API.Native.Document document = control.Document;
            document.BeginUpdate();
            try
            {
                document.DefaultCharacterProperties.FontName = "新宋体";
                document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Sesquialteral;    //行距
            }
            finally
            {
                document.EndUpdate();
            }
        }


        private void initComboBox()
        {
            for (int i = 2013; i <= DateTime.Now.Year; i++)
            {
                comboBoxEdit1.Properties.Items.Add(i);
            }
            comboBoxEdit1.SelectedIndex = comboBoxEdit1.Properties.Items.Count -1;
        }

        private void ReadWord(string pathName,DocumentFormat df)
        {
            this.richEditControl1.LoadDocument(pathName,df);
            DeleteFile(pathName);
        }

        private void DeleteFile(string pathName)
        {
            try
            {
                File.Delete(pathName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


        private string InitRead(string Template, Dictionary<string, string> datas,System.Data.DataTable dt,string userType,string year,string qymc)
        { 
            Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            //将要导出的新word文件名
            string physicNewFile =  DateTime.Now.ToString("yyyyMMddHHmmssss") + ".doc";
            try
            {
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                app.Visible = false;
                object fileName = System.Windows.Forms.Application.StartupPath + Template ;//Year1;//模板文件
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

                object unit;
                unit = Microsoft.Office.Interop.Word.WdUnits.wdStory;
                app.Selection.EndKey(ref unit, ref oMissing);
                app.Selection.TypeParagraph();//插入段落

                if (userType == "C")  //国产
                {

                    app.Selection.Text = String.Format("{0}年{1}企业国产乘用车产品燃料消耗量", year.Substring(0, 4), qymc);
                    Word.Range range = doc.Paragraphs.Last.Range;
                    Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, dt.Rows.Count+1, 12, ref oMissing, ref oMissing);
                    //设置表格的字体大小粗细
                    table.Range.Font.Size = 10;
                    table.Range.Font.Bold = 0;
                    //为表格划线
                    range.Tables[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderHorizontal].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderVertical].LineStyle = WdLineStyle.wdLineStyleSingle;

                    //设置表格标题
                    table.Cell(1, 1).Range.Text = "序号";
                    table.Cell(1, 2).Range.Text = "乘用车生产企业";
                    table.Cell(1, 3).Range.Text = "产品型号";
                    table.Cell(1, 4).Range.Text = "燃料种类";
                    table.Cell(1, 5).Range.Text = "整备质量";
                    table.Cell(1, 6).Range.Text = "变速器型式";
                    table.Cell(1, 7).Range.Text = "座椅排数";
                    table.Cell(1, 8).Range.Text = "纯电动驱动模式综合工况续驶里程";
                    table.Cell(1, 9).Range.Text = "车型燃料消耗量目标值①";
                    table.Cell(1, 10).Range.Text = "综合工况燃烧消耗量实际值②";
                    table.Cell(1, 11).Range.Text = "实际生产量③";
                    table.Cell(1, 12).Range.Text = "备注";

                    //循环数据创建数据行
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        table.Cell(i + 2, 1).Range.Text = (i + 1).ToString();
                        table.Cell(i + 2, 2).Range.Text = dt.Rows[i]["Qcscqy"].ToString();
                        table.Cell(i + 2, 3).Range.Text = dt.Rows[i]["Clxh"].ToString();
                        table.Cell(i + 2, 4).Range.Text = dt.Rows[i]["Rllx"].ToString();
                        table.Cell(i + 2, 5).Range.Text = dt.Rows[i]["Zczbzl"].ToString();
                        table.Cell(i + 2, 6).Range.Text = dt.Rows[i]["Bsqxs"].ToString();
                        table.Cell(i + 2, 7).Range.Text = dt.Rows[i]["Zwps"].ToString();
                        table.Cell(i + 2, 8).Range.Text = dt.Rows[i]["Zhgkxslc"].ToString();
                        table.Cell(i + 2, 9).Range.Text = dt.Rows[i]["TgtZhgkrlxhl"].ToString();
                        table.Cell(i + 2, 10).Range.Text = dt.Rows[i]["ActZhgkrlxhl"].ToString();
                        table.Cell(i + 2, 11).Range.Text = dt.Rows[i]["Sl_act"].ToString();
                    }
                }
                else  //进口
                {
                    app.Selection.Text = String.Format("{0}年{1}企业进口乘用车产品燃料消耗量", year.Substring(0, 4), qymc);
                    Word.Range range = doc.Paragraphs.Last.Range;
                    Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, dt.Rows.Count + 1, 12, ref oMissing, ref oMissing);
                    //设置表格的字体大小粗细
                    table.Range.Font.Size = 10;
                    table.Range.Font.Bold = 0;
                    //为表格划线
                    range.Tables[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderHorizontal].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderVertical].LineStyle = WdLineStyle.wdLineStyleSingle;

                    //设置表格标题
                    table.Cell(1, 1).Range.Text = "序号";
                    table.Cell(1, 2).Range.Text = "乘用车生产企业";
                    table.Cell(1, 3).Range.Text = "产品型号";
                    table.Cell(1, 4).Range.Text = "燃料种类";
                    table.Cell(1, 5).Range.Text = "整备质量";
                    table.Cell(1, 6).Range.Text = "变速器型式";
                    table.Cell(1, 7).Range.Text = "座椅排数";
                    table.Cell(1, 8).Range.Text = "纯电动驱动模式综合工况续驶里程";
                    table.Cell(1, 9).Range.Text = "车型燃料消耗量目标值①";
                    table.Cell(1, 10).Range.Text = "综合工况燃烧消耗量实际值②";
                    table.Cell(1, 11).Range.Text = "实际生产量③";
                    table.Cell(1, 12).Range.Text = "备注";

                    //循环数据创建数据行
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        table.Cell(i + 2, 1).Range.Text = (i + 1).ToString();
                        table.Cell(i + 2, 2).Range.Text = dt.Rows[i]["Qcscqy"].ToString();
                        table.Cell(i + 2, 3).Range.Text = dt.Rows[i]["Clxh"].ToString();
                        table.Cell(i + 2, 4).Range.Text = dt.Rows[i]["Rllx"].ToString();
                        table.Cell(i + 2, 5).Range.Text = dt.Rows[i]["Zczbzl"].ToString();
                        table.Cell(i + 2, 6).Range.Text = dt.Rows[i]["Bsqxs"].ToString();
                        table.Cell(i + 2, 7).Range.Text = dt.Rows[i]["Zwps"].ToString();
                        table.Cell(i + 2, 8).Range.Text = dt.Rows[i]["Zhgkxslc"].ToString();
                        table.Cell(i + 2, 9).Range.Text = dt.Rows[i]["TgtZhgkrlxhl"].ToString();
                        table.Cell(i + 2, 10).Range.Text = dt.Rows[i]["ActZhgkrlxhl"].ToString();
                        table.Cell(i + 2, 11).Range.Text = dt.Rows[i]["Sl_act"].ToString();
                    }
                }

                unit = Microsoft.Office.Interop.Word.WdUnits.wdStory;
                app.Selection.EndKey(ref unit, ref oMissing);
                app.Selection.TypeParagraph();//插入段落


                app.Selection.Text = @"注：①车型燃料消耗量目标值，同一产品型号因整备质量、座椅排数、变速器形式不同有多个不同的燃料消耗量目标值时，计算企业平均消耗量目标值时采用最小的燃料消耗量目标值。
②燃料消耗量（综合）实际值，四舍五入至小数点后一位；如汽车燃料消耗量通告系统中同一车型有多个不同的燃料消耗量（综合）实际值，则填写该车型最高的燃料消耗量（综合）实际值。
③实际生产量/实际进口量，不含出口量。";
               

                //对替换好的word模板另存为一个新的word文档
                doc.SaveAs(System.Windows.Forms.Application.StartupPath + "\\ExcelHeaderTemplate\\" + physicNewFile,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

                //准备导出word
              
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

        //全年
        private bool WriteYear2()
        {
            var list = DateFormat(comboBoxEdit1.Text, "year2");
            var cafcDetai = GetCafcDetailData(StaticUtil.NeCafc, list[0], list[1]);
            if (cafcDetai != null && cafcDetai.Length != 0)
            {
                Dictionary<string, string> datas = new Dictionary<string, string>();
                datas.Add("{qymc}", Utils.qymc);
                datas.Add("{year}", comboBoxEdit1.Text);
                datas.Add("{count}", cafcDetai[0].Sl_act.ToString());
                datas.Add("{tcafc}", cafcDetai[0].Tcafc.ToString());
                datas.Add("{cafc}", cafcDetai[0].Cafc.ToString());
                //decimal percent = (cafcDetai[0].Tcafc - cafcDetai[0].Cafc) / cafcDetai[0].Tcafc;
                //percent = percent * 10;
                //percent = Math.Round(percent, 1, MidpointRounding.ToEven);

                decimal tt = (cafcDetai[0].Tcafc - cafcDetai[0].Cafc);
                decimal percent = Math.Round(tt / cafcDetai[0].Tcafc, 10, MidpointRounding.AwayFromZero);
                percent = percent * 100;
                percent = Math.Round(percent, 1, MidpointRounding.AwayFromZero);

                datas.Add("{percent}", percent.ToString());


                string userType = Utils.userId.Substring(4, 1);

                var dTime = Convert.ToDateTime(list[1]).AddDays(1);
                var Data = cafcService.QueryCalParamDetails(Utils.userId, Utils.password, string.Empty, string.Empty, list[0], dTime.ToString());
                var dt = DataTableHelper.ToDataTable<CafcService.FuelCAFCDetails>(Data);
                dt.Columns["Bsqxs"].SetOrdinal(5);
                string fileName = System.Windows.Forms.Application.StartupPath + "\\ExcelHeaderTemplate\\" + InitRead(Year2, datas, dt, userType, list[0], Utils.qymc);
                ReadWord(fileName, DocumentFormat.Doc);
                return true;
            }
            else
            {
                return false;
            }
        }


        private IList<string> DateFormat(string date,string type)
        {
            IList<string> list = new List<string>();
            string dateTime= string.Empty;
            if (type == "year1")  //半年
            {
                list.Add(String.Format("{0}-01-01", date));
                list.Add(String.Format("{0}-06-30", date));
                
            }
            if (type == "year2")  //一年
            {
                list.Add(String.Format("{0}-01-01", date));
                list.Add(String.Format("{0}-12-31", date));
            }
            return list;
        }


        private CafcService.FuelCafcAndTcafc[] GetCafcDetailData(string cafcType, string startTime, string endTime)
        {
            CafcService.FuelCafcAndTcafc[] cafcData = null;

            try
            {
                if (cafcType == StaticUtil.NeCafc)
                {
                    cafcData = cafcService.QueryNECafc(Utils.userId, Utils.password, startTime, endTime);
                }
                else if (cafcType == StaticUtil.TeCafc)
                {
                    cafcData = cafcService.QueryTECafc(Utils.userId, Utils.password, startTime, endTime);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return cafcData;
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            //弹出加载对话框
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                bool flag = WriteYear2();
                if (flag)
                {
                    MessageBox.Show("生成成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("数据获取失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

    }
}