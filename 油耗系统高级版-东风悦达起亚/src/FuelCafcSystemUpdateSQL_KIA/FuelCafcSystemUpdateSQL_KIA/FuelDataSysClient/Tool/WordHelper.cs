using System;
using System.IO;
using System.Text;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace FuelDataSysClient.Tool
{
    public class WordHelper
    {
        private _Application wordApp = null;
        private _Document wordDoc = null;

        public _Application Application
        {
            get
            {
                return wordApp;
            }
            set
            {
                wordApp = value;
            }
        }
        public _Document Document
        {
            get
            {
                return wordDoc;
            }
            set
            {
                wordDoc = value;
            }
        }

        /// <summary>
        /// 通过模板创建新文档
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void CreateNewDocument(string filePath)
        {
            killWinWordProcess();
            wordApp = new Application() { DisplayAlerts = WdAlertLevel.wdAlertsNone, Visible = false };
            object missing = System.Reflection.Missing.Value;
            object templateName = filePath;
            wordDoc = wordApp.Documents.Open(ref templateName, ref missing,
              ref missing, ref missing, ref missing, ref missing, ref missing,
              ref missing, ref missing, ref missing, ref missing, ref missing,
              ref missing, ref missing, ref missing, ref missing);
        }

        /// <summary>
        /// 保存新文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveDocument(string filePath)
        {
            try
            {
                object fileName = filePath;
                object miss = System.Reflection.Missing.Value;
                wordDoc.SaveAs(ref fileName, ref miss, ref miss,
                  ref miss, ref miss, ref miss, ref miss,
                  ref miss, ref miss, ref miss, ref miss,
                  ref miss, ref miss, ref miss, ref miss,
                  ref miss);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 在替换文本处插入值
        /// </summary>
        /// <param name="replaceText">要替换的文本</param>
        /// <param name="value">替换后的值</param>
        /// <returns></returns>
        public bool InsertReplaceText(string replaceText, string value)
        {
            object replace = WdReplace.wdReplaceAll;
            if (replace != null)
            {
                wordApp.Selection.Find.Replacement.ClearFormatting();
                wordApp.Selection.Find.ClearFormatting();
                wordApp.Selection.Find.Text = replaceText;
                wordApp.Selection.Find.Replacement.Text = value;
                object miss = System.Reflection.Missing.Value;
                wordApp.Selection.Find.Execute(
                ref miss, ref miss,
                ref miss, ref miss,
                ref miss, ref miss,
                ref miss, ref miss, ref miss,
                ref miss, ref replace,
                ref miss, ref miss,
                ref miss, ref miss);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 在书签处插入值
        /// </summary>
        /// <param name="bookmark">书签</param>
        /// <param name="value">要插入的值</param>
        /// <returns></returns>
        public bool InsertBookmark(string bookmark, string value)
        {
            object bkObj = bookmark;
            if (wordApp.ActiveDocument.Bookmarks.Exists(bookmark))
            {
                wordApp.ActiveDocument.Bookmarks.get_Item(ref bkObj).Select();
                wordApp.Selection.TypeText(value);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插入表格(bookmark书签,行数，列数，表格宽度)
        /// </summary>
        /// <param name="bookmark">书签</param>
        /// <param name="rows">行数</param>
        /// <param name="columns">列数</param>
        /// <param name="width">表格宽度</param>
        /// <returns></returns>
        public Table InsertTable(string bookmark, int rows, int columns, float width)
        {
            object miss = System.Reflection.Missing.Value;
            object oStart = bookmark;
            Range range = wordDoc.Bookmarks.get_Item(ref oStart).Range;//表格插入位置
            Table newTable = wordDoc.Tables.Add(range, rows, columns, ref miss, ref miss);
            //设置表的格式
            newTable.Borders.Enable = 1; //允许有边框，默认没有边框(为0时报错，1为实线边框，2、3为虚线边框，以后的数字没试过)
            newTable.Borders.OutsideLineWidth = WdLineWidth.wdLineWidth050pt;//边框宽度
            if (width != 0)
            {
                newTable.PreferredWidth = width;//表格宽度
            }
            newTable.AllowPageBreaks = false;
            return newTable;
        }

        /// <summary>
        /// 合并单元格 表名,开始行号,开始列号,结束行号,结束列号
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <param name="row1">行索引1</param>
        /// <param name="column1">列索引1</param>
        /// <param name="row2">行索引2</param>
        /// <param name="column2">列索引2</param>
        public void MergeCell(Table table, int row1, int column1, int row2, int column2)
        {
            table.Cell(row1, column1).Merge(table.Cell(row2, column2));
        }

        /// <summary>
        /// 设置表格内容对齐方式Align水平方向，Vertical垂直方向(左对齐，居中对齐，右对齐分别对应Align和Vertical的值为-1,0,1)
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <param name="Align">水平对齐方式</param>
        /// <param name="Vertical">垂直对齐方式</param>
        public void SetParagraph_Table(Table table, int Align, int Vertical)
        {
            switch (Align)
            {
                case -1: table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft; break;//左对齐
                case 0: table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter; break;//水平居中
                case 1: table.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphRight; break;//右对齐
            }
            switch (Vertical)
            {
                case -1: table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalTop; break;//顶端对齐
                case 0: table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter; break;//垂直居中
                case 1: table.Range.Cells.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalBottom; break;//底端对齐
            }
        }

        /// <summary>
        /// 设置表格字体(表格对象,字体样式,字体大小)
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <param name="fontName">字体样式</param>
        /// <param name="size">字体大小</param>
        public void SetFont_Table(Table table, string fontName, double size)
        {
            if (fontName != "")
            {
                table.Range.Font.Name = fontName;
            }
            if (size != 0)
            {
                table.Range.Font.Size = Convert.ToSingle(size);
            }
        }

        /// <summary>
        /// 设置表格字体(表格对象,字体样式,字体大小)
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <param name="fontName">字体样式</param>
        /// <param name="size">字体大小</param>
        /// <param name="size">字体粗细</param>
        public void SetFont_Table(Table table, string fontName, double size, double bold)
        {
            if (fontName != "")
            {
                table.Range.Font.Name = fontName;
            }
            if (size != 0)
            {
                table.Range.Font.Size = Convert.ToSingle(size);
            }
            if (bold != 0)
            {
                table.Range.Font.Bold = Convert.ToInt32(bold);
            }
        }

        /// <summary>
        /// 是否使用边框,n表格的序号,use是或否
        /// </summary>
        /// <param name="n">第几个表格对象</param>
        /// <param name="use">边框样式</param>
        public void UseBorder(int n, bool use)
        {
            if (use)
            {
                wordDoc.Content.Tables[n].Borders.Enable = 1; //允许有边框，默认没有边框(为0时无边框，1为实线边框，2、3为虚线边框，以后的数字没试过)
            }
            else
            {
                wordDoc.Content.Tables[n].Borders.Enable = 2; //允许有边框，默认没有边框(为0时无边框，1为实线边框，2、3为虚线边框，以后的数字没试过)
            }
        }

        /// <summary>
        /// 给表格插入一行,n表格的序号从1开始记
        /// </summary>
        /// <param name="n">n表格的序号从1开始记</param>
        public void AddRow(int n)
        {
            object miss = System.Reflection.Missing.Value;
            wordDoc.Content.Tables[n].Rows.Add(ref miss);
        }

        /// <summary>
        /// 给表格添加一行
        /// </summary>
        /// <param name="table">表格对象</param>
        public void AddRow(Table table)
        {
            object miss = System.Reflection.Missing.Value;
            table.Rows.Add(ref miss);
        }

        /// <summary>
        /// 给表格插入rows行,n为表格的序号
        /// </summary>
        /// <param name="n">n为表格的序号</param>
        /// <param name="rows">行数</param>
        public void AddRow(int n, int rows)
        {
            object miss = System.Reflection.Missing.Value;
            Table table = wordDoc.Content.Tables[n];
            for (int i = 0; i < rows; i++)
            {
                table.Rows.Add(ref miss);
            }
        }

        /// <summary>
        /// 给表格中单元格插入元素
        /// </summary>
        /// <param name="table">表格对象</param>
        /// <param name="row">行号</param>
        /// <param name="column">列号</param>
        /// <param name="value">插如的元素</param>
        public void InsertCell(Table table, int row, int column, string value)
        {
            table.Cell(row, column).Range.Text = value;
        }

        /// <summary>
        /// 给表格中单元格插入元素,n表格的序号从1开始记
        /// </summary>
        /// <param name="n">n表格的序号从1开始记</param>
        /// <param name="row">行号</param>
        /// <param name="column">列号</param>
        /// <param name="value">插入的元素</param>
        public void InsertCell(int n, int row, int column, string value)
        {
            wordDoc.Content.Tables[n].Cell(row, column).Range.Text = value;
        }

        /// <summary>
        /// 给表格插入一行数据,n为表格的序号
        /// </summary>
        /// <param name="n">n为表格的序号</param>
        /// <param name="row">行号</param>
        /// <param name="columns">列号</param>
        /// <param name="values">插入的元素</param>
        public void InsertCell(int n, int row, int columns, string[] values)
        {
            Table table = wordDoc.Content.Tables[n];
            for (int i = 0; i < columns; i++)
            {
                table.Cell(row, i + 1).Range.Text = values[i];
            }
        }

        /// <summary>
        /// 插入图片(标签，图片路径，高度，宽度)
        /// </summary>
        /// <param name="bookmark">标签</param>
        /// <param name="picturePath">图片路径</param>
        /// <param name="width">宽度</param>
        /// <param name="hight">高度</param>
        public void InsertPicture(string bookmark, string picturePath, float width, float hight)
        {
            object oStart = bookmark;
            Object linkToFile = false;    //图片是否为外部链接
            Object saveWithDocument = true; //图片是否随文档一起保存
            object range = wordDoc.Bookmarks.get_Item(ref oStart).Range;//图片插入位置
            wordDoc.InlineShapes.AddPicture(picturePath, ref linkToFile, ref saveWithDocument, ref range);
            wordDoc.Application.ActiveDocument.InlineShapes[1].Width = width; //设置图片宽度
            wordDoc.Application.ActiveDocument.InlineShapes[1].Height = hight; //设置图片高度
        }

        /// <summary>
        /// 插入一段文字(标签，文字内容)
        /// </summary>
        /// <param name="bookmark">标签</param>
        /// <param name="text">text为文字内容</param>
        public void InsertText(string bookmark, string text)
        {
            object oStart = bookmark;
            object range = wordDoc.Bookmarks.get_Item(ref oStart).Range;
            Paragraph wp = wordDoc.Content.Paragraphs.Add(ref range);
            wp.Format.SpaceBefore = 6;
            wp.Range.Text = text;
            wp.Format.SpaceAfter = 24;
            wp.Range.InsertParagraphAfter();
            wordDoc.Paragraphs.Last.Range.Text = "\n";
        }

        /// <summary>
        /// 释放Word资源
        /// </summary>
        private void Dispose()
        {
            //关闭wordDoc，wordApp对象
            object SaveChanges = WdSaveOptions.wdSaveChanges;
            object OriginalFormat = WdOriginalFormat.wdOriginalDocumentFormat;
            object RouteDocument = false;
            wordDoc.Close(ref SaveChanges, ref OriginalFormat, ref RouteDocument);
            wordApp.Quit(ref SaveChanges, ref OriginalFormat, ref RouteDocument);

            if (wordDoc != null)
            {
                Marshal.ReleaseComObject(wordDoc);
                wordDoc = null;
            }
            if (wordApp != null)
            {
                Marshal.ReleaseComObject(wordApp);
                wordApp = null;
            }
            GC.Collect();
            this.killWinWordProcess();
        }

        /// <summary>
        /// 杀掉winword.exe进程
        /// </summary>
        public void killWinWordProcess()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("WINWORD");
            foreach (System.Diagnostics.Process process in processes)
            {
                bool b = process.MainWindowTitle == "";
                if (process.MainWindowTitle == "")
                {
                    process.Kill();
                }
            }
        }
    }
}
