using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebBrowserUtils.ExtendWebBrowser;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class BHFillManager : WebFillManager
    {
        private const string carTypeName = "车辆类型";
        private Hashtable _carTypeList;

        public override string CarType
        {
            get
            {
                return base.Data == null ? null : base.Data["CarType"] as string;
            }
            set
            {
                if (base.Data == null || _carTypeList == null)
                    return;
                base.Data["CarType"] = value;
                base.Data[carTypeName] = _carTypeList[value] as string;
            }
        }

        public override string Standard
        {
            get
            {
                return base.Data == null ? null : base.Data["排放标准"] as string;
            }
            set
            {
                if (base.Data == null)
                    return;
                base.Data["排放标准"] = value;
            }
        }

        public BHFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile)
            : this(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.FillRule))
        {
        }

        internal BHFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile, string ruleFilePath)
            : base(browser, dataFile, ruleFilePath)
        {
            base.FillType = "北环";
            base.Version = Properties.Resources.FillRule;
        }

        public override void BeginFill()
        {
            base.BeginFill();
            string carType = base.Data[carTypeName] as string;
            if (carType != null)
            {
                base.Data[carTypeName] = _carTypeList[carType];
                base.Data.Add("CarType", carType);
            }
        }

        protected override void OnFillerStateChanged(FillBase fill)
        {
            if (fill.FillState == FillState.Exception)
            {
                WebFillManager.ShowMessageBox(string.Format("填报过程发生错误，异常信息：{0}", fill.Exception.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            base.OnFillerStateChanged(fill);
        }

        protected override FillBase CreateFill(Uri url, ExtendWebBrowser.WebBrowser2 browser)
        {
            BHUrlParameter urlParameter = Uris[url.AbsolutePath] as BHUrlParameter;
            BHFiller filler = null;
            if (url.AbsolutePath == "/motor/part/carpart!input.action")
                filler = new BHEngineFiller(browser, url, urlParameter) { DataFile = base.DataProvider.DataSourceFile, Converter = this.Converter };
            else
                filler = new BHFiller(browser, url, urlParameter) { DataFile = base.DataProvider.DataSourceFile, Converter = this.Converter };
            return filler;
        }
        /// <summary>
        /// 北环得到路径
        /// </summary>
        /// <returns></returns>
        protected override Hashtable GetUris()
        {
            Hashtable uris = new Hashtable();
            //读取北环下面的公共页面（路径）
            string fileName = FileHelper.GetPublicPage(base.Version, base.FillType);
            using (Office.Excel.ForwardExcelReader pages = new Office.Excel.ForwardExcelReader(string.Format("{0}\\{1}", base.RuleFilePath, fileName)))
            {
                pages.Open();
                Office.Excel.ForwardReadWorksheet sheet = pages.Activate("页面汇总") as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                {
                    BHUrlParameter urlParameter = null;
                    Uri url = null;
                    Hashtable columnHeader = GetColumnHeader(sheet);
                    while (sheet.ReadNextRow()) // 读取下一行。
                    {
                        urlParameter = new BHUrlParameter();
                        while (sheet.ReadNextCell(false)) // 读取本行的下一个单元格数据。
                        {
                            switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                            {
                            case "URL":
                                url = new Uri(sheet.GetContent() as string);
                                break;
                            case "页面标签":
                                urlParameter.LabelName = sheet.GetContent() as string;
                                break;
                            case "所属Excel":
                                urlParameter.IsPublicUrl = ((sheet.GetContent() as string) == "公共页面");
                                break;
                            case "表数据":
                                urlParameter.IsTableData = ((sheet.GetContent() as string) == "是");
                                break;
                            }
                        }
                        if (url != null && string.IsNullOrEmpty(url.ToString()) == false)
                        {
                            if (uris.ContainsKey(url.AbsolutePath))
                                continue;
                            uris.Add(url.AbsolutePath, urlParameter);
                        }
                    }
                }
                sheet = pages.Activate(carTypeName) as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                    _carTypeList = ReadCarTypeList(sheet);
                else
                    throw new ArgumentException(string.Format("填报规则中没有名为“{0}”的工作表。", carTypeName));
            }
            return uris;
        }
        /// <summary>
        /// 北环得到数据
        /// </summary>
        /// <returns></returns>
        protected override object GetData(object state)
        {
            if (state == null)
            {
                Hashtable _data = new Hashtable();
                using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(base.DataProvider.DataSourceFile))
                {
                    reader.Open();
                    Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
                    if (sheet == null)
                        return null;
                    object parameter, content;
                    while (sheet.ReadNextRow())
                    {
                        parameter = null;
                        content = null;
                        while (sheet.ReadNextCell(false))
                        {
                            if (sheet.CurrentCell.ColumnIndex == 1)
                                parameter = sheet.GetContent();
                            else if (sheet.CurrentCell.ColumnIndex == 2)
                                content = sheet.GetContent();
                        }
                        if (parameter == null)
                            continue;
                        _data.Add(parameter, content == null ? "" : content.ToString());
                    }
                }
                string carType = _data[carTypeName] as string;
                if (carType != null)
                {
                    _data[carTypeName] = _carTypeList[carType];
                    _data.Add("CarType", carType);
                }
                return _data;
            }
            else
            {
                object[] array = state as object[];
                return ExcelFileHelper.ReadTableData(this, array.Length <= 0 ? null : array[0] as string, array[1] as List<int>);
            }
        }

        //private Hashtable ReadTableData(string labelName)
        //{
        //    bool isColumnHeader = false;
        //    //object content = null;
        //    Hashtable columnHeader = new Hashtable();
        //    Hashtable tableList = null;
        //    DataTable table = null;
        //    DataRow row = null;
        //    int index = 0;
        //    IDataProvider provider = this;
        //    IWorkbook workbook = ExcelFileHelper.GetWorkbook(provider.DataSourceFile);
        //    WebValueConverter converter = provider.GetConverter() as WebValueConverter;
        //    string dataSheetName = converter == null ? labelName : converter.GetSheetName(labelName);
        //    string sheetName = string.Format("{0}_{1}", dataSheetName, base.FillIndexes.Count == 0 ? 1 : base.FillIndexes[0].CurrentIndex + 1);

        //    ISheet sheet = workbook.GetSheet(sheetName);

        //    if (sheet != null)
        //    {
        //        tableList = new Hashtable();
        //        for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
        //        {
        //            IRow dataRow = sheet.GetRow(rowIndex);
        //            if (dataRow == null)
        //                continue;
        //            for (int colIndex = 0; colIndex < dataRow.LastCellNum; colIndex++)
        //            {
        //                ICell cell = dataRow.GetCell(colIndex);
        //                if (cell == null)
        //                    continue;
        //                string str = ExcelFileHelper.GetCellValue(cell, null);
        //                if (isColumnHeader == false && (str == "行标题" || table == null))
        //                {
        //                    table = new System.Data.DataTable();
        //                    tableList.Add(index, table);
        //                    isColumnHeader = true;
        //                    row = null;
        //                    index++;
        //                }
        //                int columnIndex = cell.ColumnIndex;
        //                if (isColumnHeader)
        //                {
        //                    if (string.IsNullOrEmpty(str))
        //                        continue;
        //                    if (columnHeader.Contains(columnIndex))
        //                        columnHeader[columnIndex] = str;
        //                    else
        //                        columnHeader.Add(columnIndex, str);
        //                    try
        //                    {
        //                        if (string.IsNullOrEmpty(str) == false)
        //                            table.Columns.Add(str);
        //                    }
        //                    catch (System.Data.DuplicateNameException)
        //                    {
        //                        throw new ArgumentException(string.Format("在Excel文件：{0}的{1}工作表中，{2}行的值有重复！", provider.DataSourceFile, sheetName, rowIndex + 1));
        //                    }
        //                }
        //                else
        //                {
        //                    string columnName = columnHeader[columnIndex] as string;
        //                    if (columnName != null && table.Columns.Contains(columnName))
        //                        row[columnName] = str;
        //                }
        //            }
        //            if (row != null)
        //                table.Rows.Add(row);
        //            row = table.NewRow();
        //            isColumnHeader = false;
        //        }
        //    }
        //    return tableList;
        //}

        //private Hashtable ReadTableData(string labelName)
        //{
        //    bool isColumnHeader = false;
        //    object content = null;
        //    Hashtable columnHeader = new Hashtable();
        //    Hashtable tableList = null;
        //    DataTable table = null;
        //    DataRow row = null;
        //    int index = 0;
        //    using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(base.DataProvider.DataSourceFile))
        //    {
        //        reader.Open();
        //        // 表格数据的工作表标签使用"前缀_序号"格式命名。
        //        string dataSheetName = this.Converter == null ? labelName : this.Converter.GetSheetName(labelName);
        //        string sheetName = string.Format("{0}_{1}", dataSheetName, base.FillIndexes.Count == 0 ? 1 : base.FillIndexes[0].CurrentIndex + 1);
        //        Office.Excel.ForwardReadWorksheet sheet = reader.Activate(sheetName) as Office.Excel.ForwardReadWorksheet;
        //        if (sheet != null)
        //        {
        //            tableList = new Hashtable();
        //            while (sheet.ReadNextRow())
        //            {
        //                while (sheet.ReadNextCell(false))
        //                {
        //                    content = sheet.GetContent();
        //                    string str = content == null ? "" : content.ToString();
        //                    if (isColumnHeader == false && (str == "行标题" || table == null))
        //                    {
        //                        table = new System.Data.DataTable();
        //                        tableList.Add(index, table);
        //                        isColumnHeader = true;
        //                        row = null;
        //                        index++;
        //                    }
        //                    int columnIndex = sheet.CurrentCell.ColumnIndex;
        //                    if (isColumnHeader)
        //                    {
        //                        if (string.IsNullOrEmpty(str))
        //                            continue;
        //                        if (columnHeader.Contains(columnIndex))
        //                            columnHeader[columnIndex] = str;
        //                        else
        //                            columnHeader.Add(columnIndex, str);
        //                        try
        //                        {
        //                            if (string.IsNullOrEmpty(str) == false)
        //                                table.Columns.Add(str);
        //                        }
        //                        catch (System.Data.DuplicateNameException)
        //                        {
        //                            throw new ArgumentException(string.Format("在Excel文件：{0}的{1}工作表中，{2}行的值有重复！",
        //                                sheet.Owner.FileName, sheet.Name, sheet.CurrentRowIndex));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        string columnName = columnHeader[columnIndex] as string;
        //                        if (table.Columns.Contains(columnName))
        //                            row[columnName] = str;
        //                    }
        //                }
        //                if (row != null)
        //                    table.Rows.Add(row);
        //                row = table.NewRow();
        //                isColumnHeader = false;
        //            }
        //        }
        //        return tableList;
        //    }
        //}

        private Hashtable ReadCarTypeList(Office.Excel.ForwardReadWorksheet sheet)
        {
            object content = null;
            Hashtable relation = new Hashtable();
            Hashtable columnHeader = GetColumnHeader(sheet);
            string key = "", value = "";
            while (sheet.ReadNextRow())
            {
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "全称":
                        key = content == null ? "" : content.ToString();
                        break;
                    case "缩写":
                        value = content == null ? "" : content.ToString();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(key))
                    continue;
                relation.Add(key, value);
            }
            return relation;
        }
        /// <summary>
        /// 查找指定WebBrowser中的填报线程。
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private HtmlDocument FindDocument(Uri url, HtmlDocument doc)
        {
            if (doc == null)
                return null;
            if (doc.Url == url)
                return doc;
            HtmlDocument finded = null;
            foreach (HtmlWindow item in doc.Window.Frames)
            {
                finded = FindDocument(url, item.Document);
                if (finded != null)
                    return finded;
            }
            return null;
        }

        //加载完成事件
        protected override void OnDocumentCompleted(ExtendWebBrowser.WebBrowser2 browser, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(browser, e);
            if(base.CurrentFill != null)
            {
                string uri = e.Url.AbsolutePath;
                if (uri == "/motor/car/car-declare-step6.action" || uri == "/motor/car/car-declare-step33.action" || uri == "/motor/part/carpart!method.action")
                {
                    base.CurrentFill.Resume();
                }
            }
        }
    }
}
