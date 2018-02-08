using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;

namespace CreateWordTable
{
    public partial class Form1 : Form
    {
        private const string Change = "\\ExcelHeaderTemplate\\Change.docx";
        public Form1()
        {
            InitializeComponent();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
             Microsoft.Office.Interop.Word.Application app = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            //将要导出的新word文件名
            string physicNewFile = DateTime.Now.ToString("yyyyMMddHHmmssss") + ".doc";
            try
            {
                app = new Microsoft.Office.Interop.Word.Application();//创建word应用程序
                object fileName = System.Windows.Forms.Application.StartupPath + Change;//Year1;//模板文件
                app.Visible = true;
                //打开模板文件
                object oMissing = System.Reflection.Missing.Value;
                doc = app.Documents.Open(ref fileName,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);



                Word.Range range = doc.Paragraphs.Last.Range;
                //object tmp = "BT_CREATE";
                //Word.Range range = app.ActiveDocument.Bookmarks.get_Item(ref tmp).Range;

                    //Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
                    Microsoft.Office.Interop.Word.Table table = app.Selection.Tables.Add(range, 2, 4, ref oMissing, ref oMissing);
                    //设置表格的字体大小粗细
                    table.Range.Font.Size = 10;
                    table.Range.Font.Bold = 0;

                    //设置表格标题
                    int rowIndex = 1;
                    table.Cell(rowIndex, 1).Range.Text = "班级";
                    table.Cell(rowIndex, 2).Range.Text = "姓名";
                    table.Cell(rowIndex, 3).Range.Text = "成绩";
                    table.Cell(rowIndex, 4).Range.Text = "班主任";

                    //循环数据创建数据行

                    table.Cell(2, 1).Range.Text = "1";//班级
                    table.Cell(2, 4).Range.Text = "4";//人数
                    table.Cell(2, 2).Range.Text = "2";//班主任
                    table.Cell(2, 3).Range.Text = "3";




                    //为表格划线
                    range.Tables[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderHorizontal].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range.Tables[1].Borders[WdBorderType.wdBorderVertical].LineStyle = WdLineStyle.wdLineStyleSingle;

                    object count = 14;
                    //object WdLine = Word.WdUnits.wdLine;//换一行;
                    //app.Selection.MoveDown(ref WdLine, ref count, ref oMissing);//移动焦点
                    //
                    object unit;
                    unit = Word.WdUnits.wdStory;
                    app.Selection.EndKey(ref unit, ref oMissing);
                    app.Selection.TypeParagraph();//插入段落

                    app.Selection.Text = "aaaa";


                    Word.Range range1 = doc.Paragraphs.Last.Range;
                    //object tmp = "BT_CREATE";
                    //Word.Range range = app.ActiveDocument.Bookmarks.get_Item(ref tmp).Range;

                    //Microsoft.Office.Interop.Word.Range range = app.Selection.Range;
                    Microsoft.Office.Interop.Word.Table table1 = app.Selection.Tables.Add(range1, 5, 5, ref oMissing, ref oMissing);
                    //设置表格的字体大小粗细
                    table1.Range.Font.Size = 10;
                    table1.Range.Font.Bold = 0;


                    //为表格划线
                    range1.Tables[1].Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range1.Tables[1].Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range1.Tables[1].Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range1.Tables[1].Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range1.Tables[1].Borders[WdBorderType.wdBorderHorizontal].LineStyle = WdLineStyle.wdLineStyleSingle;
                    range1.Tables[1].Borders[WdBorderType.wdBorderVertical].LineStyle = WdLineStyle.wdLineStyleSingle;
               

                    //设置表格标题
                    int rowIndex1 = 1;
                    table1.Cell(rowIndex1, 1).Range.Text = "班级";
                    table1.Cell(rowIndex1, 2).Range.Text = "姓名";
                    table1.Cell(rowIndex1, 3).Range.Text = "成绩";
                    table1.Cell(rowIndex1, 4).Range.Text = "班主任";
                    table1.Cell(rowIndex1, 5).Range.Text = "112233";

                    //循环数据创建数据行

                    table1.Cell(2, 1).Range.Text = "一";//班级
                    table1.Cell(2, 4).Range.Text = "四";//人数
                    table1.Cell(2, 2).Range.Text = "二";//班主任
                    table1.Cell(2, 3).Range.Text = "三";
                    table1.Cell(2, 5).Range.Text = "三1";




               


                

                //对替换好的word模板另存为一个新的word文档
                doc.SaveAs(System.Windows.Forms.Application.StartupPath + "\\ExcelHeaderTemplate\\" + physicNewFile,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);

            }
            catch (System.Threading.ThreadAbortException ex)
            {
                //这边为了捕获Response.End引起的异常
            }
            catch (Exception ex)
            {

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
           
        
        }
    }
}
