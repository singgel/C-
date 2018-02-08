using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Tool
{
    public class ExportHelper
    {
        /// <summary>
        /// DevExpress控件通用导出Excel,支持多个控件同时导出在同一个Sheet表或者分不同工作薄
        /// eg:ExportToXlsx("test",true,"控件",gridControl1,gridControl2);
        /// 将gridControl1和gridControl2的数据一同导出到同一个文件不同的工作薄
        /// eg:ExportToXlsx("test",false,"",gridControl1,gridControl2);
        /// 将gridControl1和gridControl2的数据一同导出到同一个文件同一个的工作薄
        /// <param name="title">文件</param>
        /// <param name="isPageForEachLink">多个打印控件是否分多个工作薄显示</param>
        /// <param name="sheetName">工作薄名称</param>
        /// <param name="printables">控件集 eg:GridControl,PivotGridControl,TreeList,ChartControl...</param>
        public static void ExportToExcel(string FileName, bool isPageForEachLink, string sheetName, params DevExpress.XtraPrinting.IPrintable[] printables)
        {
            if (string.IsNullOrEmpty(FileName))
                return;
            using (DevExpress.XtraPrintingLinks.CompositeLink link = new DevExpress.XtraPrintingLinks.CompositeLink(new DevExpress.XtraPrinting.PrintingSystem()))
            {
                foreach (var item in printables)
                {
                    var plink = new DevExpress.XtraPrinting.PrintableComponentLink() { Component = item };
                    link.Links.Add(plink);
                }
                if (isPageForEachLink)
                    link.CreatePageForEachLink();
                if (string.IsNullOrEmpty(sheetName))
                    sheetName = "Sheet";
                //默认工作薄名称
                try
                {
                    if (FileName.LastIndexOf(".xlsx") >= FileName.Length - 5)
                    {
                        DevExpress.XtraPrinting.XlsxExportOptions options = new DevExpress.XtraPrinting.XlsxExportOptions() { SheetName = sheetName };
                        if (isPageForEachLink)
                            options.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFilePageByPage;
                        link.ExportToXlsx(FileName, options);
                    }
                    else
                    {
                        DevExpress.XtraPrinting.XlsExportOptions options = new DevExpress.XtraPrinting.XlsExportOptions() { SheetName = sheetName };
                        if (isPageForEachLink)
                            options.ExportMode = DevExpress.XtraPrinting.XlsExportMode.SingleFile;
                        link.ExportToXls(FileName, options);
                    }
                }
                catch (Exception ex)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ex.Message);
                }
            }
        }
    }
}
