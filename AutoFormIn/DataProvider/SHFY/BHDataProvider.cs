using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Assistant.DataProviders.SHFY
{
    /// <summary>
    /// 上海泛亚北环数据提供类。
    /// </summary>
    public class BHDataProvider : NotifyPropertyChanged, IDataProvider
    {
        private string _connectionString;
        //private DataSet conn;

        private static readonly PropertyChangedEventArgs ProductsChanged = new PropertyChangedEventArgs("Products");
        private static readonly PropertyChangedEventArgs SearchStringChanged = new PropertyChangedEventArgs("SearchString");

        //private string sheetname = "北环";

        string IDataProvider.DataSourceFile
        {
            get { return _connectionString; }
            set
            {
                //if (conn != null)
                //{
                //    conn.Clear();
                //}
                //conn = null;
                //if (value != null)
                //{
                //    conn = new DataSet();
                //    string strConn;
                //    if (Path.GetExtension(value) == ".xls")
                //    {
                //        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + value + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
                //    }
                //    else if (Path.GetExtension(value) == ".xlsx")
                //    {
                //        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + value + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1';";
                //    }
                //    else
                //    {
                //        throw (new Exception("提交的文件不是excel文件！"));
                //    }
                //    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "$]", strConn);
                //    oada.Fill(conn);
                //}
                _connectionString = value;
            }
        }

        bool IDataProvider.AllowAlternately
        {
            get { return false; }
        }

        public BHDataProvider()
        {
        }

        void IDataProvider.Clean()
        {
        }

        DataProviders.ValueConverter IDataProvider.GetConverter()
        {
            return new ValueConverter();
        }

        object IDataProvider.ProvideData(object state)
        {
            if (state != null)
            {
                object[] array = state as object[];
                return ExcelFileHelper.ReadTableData(this, array[0] as string, array[1] as List<int>);
                //return this.ReadTableData(array[0] as string, array[1] as List<int>);
            }
            Hashtable result = new Hashtable();
            IWorkbook workbook = ExcelFileHelper.GetWorkbook(_connectionString);
            ISheet sheet = workbook.GetSheetAt(0);
            Hashtable columnHeader = new Hashtable();
            IRow row = sheet.GetRow(0);
            for (int colIndex = 0; colIndex < row.LastCellNum; colIndex++)
            {
                ICell cell = row.GetCell(colIndex);
                if (cell != null)
                    columnHeader.Add(cell.ColumnIndex, ExcelFileHelper.GetCellValue(cell, null));
            }
            string key = "", value = "", code = "";
            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                row = sheet.GetRow(rowIndex);
                if (row == null)
                    continue;
                for (int colIndex = 0; colIndex < row.LastCellNum; colIndex++)
                {
                    ICell cell = row.GetCell(colIndex);
                    if (cell == null)
                        continue;
                    switch (columnHeader[colIndex] as string)
                    {
                        case "编号":
                            key = ExcelFileHelper.GetCellValue(cell, null);
                            break;
                        case "参数值":
                            value = ExcelFileHelper.GetCellValue(cell, null);
                            break;
                        case "PATAC统一参数编码":
                            code = ExcelFileHelper.GetCellValue(cell, null);
                            break;
                    }
                }
                if (key != null && result.ContainsKey(key) == false)
                {
                    result.Add(key, new object[] { value, code });
                }
            }
            //if (state != null)
            //    return null;
            //if (conn == null)
            //    throw new ArgumentException("未指定有效的数据源！");
            //Hashtable result = new Hashtable();
            //DataTable data = conn.Tables[0];
            //string keyStr = "编号";
            //string valueStr = "参数值";
            //string codeStr = "PATAC统一参数编码";
            //int keyIndex = -1;
            //int valueIndex = -1;
            //int codeIndex = -1;
            //for (int j = 0; j < data.Rows.Count; j++)
            //{
            //    if (keyIndex < 0 || valueIndex < 0)
            //    {
            //        for (int i = 0; i < data.Columns.Count; i++)
            //        {
            //            string str = data.Rows[j][i].ToString();
            //            if (keyStr == str) { keyIndex = i; }
            //            if (valueStr == str) { valueIndex = i; }
            //            if (codeStr == str) { codeIndex = i; }
            //            if (keyIndex >= 0 && valueIndex >= 0 && codeIndex >= 0) break;
            //        }
            //    }
            //    else 
            //    {
            //        string key = data.Rows[j][keyIndex].ToString();
            //        if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key)) 
            //        {
            //            string value = data.Rows[j][valueIndex].ToString();
            //            result.Add(key, new object[] { value, data.Rows[j][codeIndex] });
            //        }
            //    }
            //}
            return result;
        }

        //private Hashtable ReadTableData(string labelName, List<int> fillIndexes)
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
        //    string sheetName = string.Format("{0}_{1}", dataSheetName, fillIndexes.Count == 0 ? 1 : fillIndexes[0] + 1);

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

        //private Hashtable ReadTableData(string labelName, List<int> fillIndexes)
        //{
        //    bool isColumnHeader = false;
        //    object content = null;
        //    Hashtable columnHeader = new Hashtable();
        //    Hashtable tableList = null;
        //    DataTable table = null;
        //    DataRow row = null;
        //    int index = 0;
        //    IDataProvider provider = this;
        //    using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(provider.DataSourceFile))
        //    {
        //        reader.Open();
        //        // 表格数据的工作表标签使用"前缀_序号"格式命名。
        //        WebValueConverter converter = provider.GetConverter() as WebValueConverter;
        //        string dataSheetName = converter == null ? labelName : converter.GetSheetName(labelName);
        //        string sheetName = string.Format("{0}_{1}", dataSheetName, fillIndexes.Count == 0 ? 1 : fillIndexes[0] + 1);
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
        
        private void InitProductList()
        {
        }

        bool IDataProvider.ShowWindow()
        {
            return false;
        }

        bool IDataProvider.CanValidation
        {
            get { return false; }
        }

        bool IDataProvider.Validate()
        {
            return true;
        }
    }
}
