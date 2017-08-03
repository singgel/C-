using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    public class ExcelFileHelper
    {
        public static IWorkbook GetWorkbook(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                if (extension == ".xlsx")
                {
                    return new NPOI.XSSF.UserModel.XSSFWorkbook(stream);
                }
                else
                {
                    return new NPOI.HSSF.UserModel.HSSFWorkbook(stream);
                }
            }
        }

        public static string GetCellValue(ICell cell, string dateFormat)
        {
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                case CellType.String:
                case CellType.Unknown:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    if (DateUtil.IsCellDateFormatted(cell))
                        return cell.DateCellValue.ToString(string.IsNullOrEmpty(dateFormat) ? "yyyy-MM-dd" : dateFormat);
                    else
                        return cell.NumericCellValue.ToString();
                default:
                    return "";
            }
        }

        public static Hashtable ReadTableData(IDataProvider provider, string labelName, List<int> fillIndexes)
        {
            bool isColumnHeader = false;
            //object content = null;
            Hashtable columnHeader = new Hashtable();
            Hashtable tableList = null;
            DataTable table = null;
            DataRow row = null;
            int index = 0;
            IWorkbook workbook = ExcelFileHelper.GetWorkbook(provider.DataSourceFile);
            WebValueConverter converter = provider.GetConverter() as WebValueConverter;
            string dataSheetName = converter == null ? labelName : converter.GetSheetName(labelName);
            string sheetName = string.Format("{0}_{1}", dataSheetName, fillIndexes.Count == 0 ? 1 : fillIndexes[0] + 1);

            ISheet sheet = workbook.GetSheet(sheetName);

            if (sheet != null)
            {
                tableList = new Hashtable();
                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow dataRow = sheet.GetRow(rowIndex);
                    if (dataRow == null)
                        continue;
                    for (int colIndex = 0; colIndex < dataRow.LastCellNum; colIndex++)
                    {
                        ICell cell = dataRow.GetCell(colIndex);
                        if (cell == null)
                            continue;
                        string str = ExcelFileHelper.GetCellValue(cell, null);
                        if (isColumnHeader == false && (str == "行标题" || table == null))
                        {
                            table = new System.Data.DataTable();
                            tableList.Add(index, table);
                            isColumnHeader = true;
                            row = null;
                            index++;
                        }
                        int columnIndex = cell.ColumnIndex;
                        if (isColumnHeader)
                        {
                            if (string.IsNullOrEmpty(str))
                                continue;
                            if (columnHeader.Contains(columnIndex))
                                columnHeader[columnIndex] = str;
                            else
                                columnHeader.Add(columnIndex, str);
                            try
                            {
                                if (string.IsNullOrEmpty(str) == false)
                                    table.Columns.Add(str);
                            }
                            catch (System.Data.DuplicateNameException)
                            {
                                throw new ArgumentException(string.Format("在Excel文件：{0}的{1}工作表中，{2}行的值有重复！", provider.DataSourceFile, sheetName, rowIndex + 1));
                            }
                        }
                        else
                        {
                            string columnName = columnHeader[columnIndex] as string;
                            if (columnName != null && table.Columns.Contains(columnName))
                                row[columnName] = str;
                        }
                    }
                    if (row != null)
                        table.Rows.Add(row);
                    row = table.NewRow();
                    isColumnHeader = false;
                }
            }
            return tableList;
        }
    }
}
