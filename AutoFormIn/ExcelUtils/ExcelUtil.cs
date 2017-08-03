using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

using ExcelUtils.Readers;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using EntityLib;

namespace ExcelUtils
{
    public static class ExcelUtil
    {
        public static String provider = "Microsoft.Jet.OLEDB.4.0";

        public static String dataSource = null;

        public static String extendedProperties = "Excel 8.0";

        public static String getOleDBConnectionString()
        {
            return "Provider=" + provider +
                ";Data Source=" + dataSource +
                ";Extended Properties=" + extendedProperties + ";";
        }

        public static ExcelInstance readExcel(String source)
        {
            dataSource = source;
            return new ExcelInstance(getOleDBConnectionString());
        }

        public static ExcelInstance readExcel(String source, String selCmd)
        {
            dataSource = source;
            return new ExcelInstance(getOleDBConnectionString(), selCmd);
        }

        public static Reader reader = new OleDbExcelReaderImpl();

        public static DataTable getDataTableFromExcel(String filePath)
        {

            using (FileStream excelFileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                IWorkbook workbook = new HSSFWorkbook(excelFileStream);
                ISheet sheet = workbook.GetSheetAt(0);//取第一个表
                DataTable table = new DataTable();
                IRow headerRow = sheet.GetRow(0);//第一行为标题行
                int cellCount = headerRow.LastCellNum;//LastCellNum = PhysicalNumberOfCells
                int rowCount = sheet.LastRowNum;//LastRowNum = PhysicalNumberOfRows - 1
                //handling header.
                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    //DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    DataColumn column = new DataColumn("column" + i);
                    table.Columns.Add(column);
                }
                for (int i = sheet.FirstRowNum; i <= rowCount; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    if (row != null)
                    {
                        //cellCount = row.LastCellNum;
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {
                            if (row.GetCell(j) != null)
                            {
                                ICell cell = row.GetCell(j);
                                try
                                {
                                    String value = cell.ToString();
                                    if (value == null)
                                    {
                                        value = "";
                                    }
                                    else { 
                                        
                                    }
                                    dataRow[j] = cell.ToString();
                                }
                                catch (Exception)
                                {
                                    
                                    throw;
                                }
                            }

                        }
                    }
                    table.Rows.Add(dataRow);
                }
                return table;
            }
        }

        private static String handleTableValue(String value){
        
            return null;
        }

        public static MemoryStream RenderToExcel(DataTable table)
        {
            MemoryStream ms = new MemoryStream();

            IWorkbook workbook = new HSSFWorkbook();

            ISheet sheet = workbook.CreateSheet();
            IRow headerRow = sheet.CreateRow(0);

            // handling header.
            foreach (DataColumn column in table.Columns)
                headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value

            // handling value.
            int rowIndex = 1;

            foreach (DataRow row in table.Rows)
            {
                IRow dataRow = sheet.CreateRow(rowIndex);

                foreach (DataColumn column in table.Columns)
                {
                    dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;

            return ms;
        }

        public static void SaveToFile(MemoryStream ms, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
                data = null;
            }
        }

        /// <summary>
        /// 直接将申报参数填写会导入的excel文件中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="list"></param>
        public static void exportCarParamsForSHGM(String filePath, String packageCode, List<Param> listParams)
        {

            String modelCode = packageCode.Substring(0, 5);
            String subPackageCode = packageCode.Substring(5, 4);
            String orderCode = packageCode.Substring(10);
            FileStream excelFileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
            String cname = null;
            String pcode = null;
            IWorkbook workbook = new HSSFWorkbook(excelFileStream);
            excelFileStream.Close();
            ISheet sheet = workbook.GetSheetAt(0);//取第一个表
            int columnNo = 0;
            int rowno = 0;
            int rowCount = sheet.LastRowNum;
            while (rowno <= rowCount)
            {
                IRow row = sheet.GetRow(rowno);
                ICell cell = row.GetCell(columnNo);
                if (cell == null)
                {
                    rowno++;
                    continue;
                }
                cname = cell.StringCellValue;
                if (!String.IsNullOrEmpty(cname) && cname.Equals("Model Code"))
                {
                    break;
                }
                rowno++;
            }
            while (columnNo <= sheet.GetRow(rowno).LastCellNum)
            {
                String modelCodeValue = sheet.GetRow(rowno).GetCell(columnNo).ToString();
                String subPackageCodeValue = sheet.GetRow(rowno + 1).GetCell(columnNo).ToString();
                String orderCodeValue = sheet.GetRow(rowno + 2).GetCell(columnNo).ToString();
                if (modelCodeValue.Equals(modelCode) &&
                    subPackageCodeValue.Equals(subPackageCode) &&
                    orderCodeValue.Equals(orderCode))
                {
                    break;
                }
                columnNo++;
            }

            rowno = 0;
            cname = sheet.GetRow(rowno).GetCell(0).ToString();
            pcode = sheet.GetRow(rowno).GetCell(1).ToString();

            //从车辆名称到燃料种类
            while (!cname.Equals("部门会签") && rowno <= rowCount)
            {
                if (String.IsNullOrEmpty(cname))
                {
                    rowno++;
                    cname = getValueFromSheet(sheet, rowno, 0);
                    continue;
                }

                if (sheet.GetRow(rowno).GetCell(0) == null ||
                    sheet.GetRow(rowno).GetCell(1) == null)
                {
                    rowno++;
                    cname = getValueFromSheet(sheet, rowno, 0);
                    continue;
                }
                cname = getValueFromSheet(sheet, rowno, 0);
                pcode = getValueFromSheet(sheet, rowno, 1);
                String value = getParameter(cname, pcode, listParams);
                sheet.GetRow(rowno).GetCell(columnNo).SetCellValue(value);
                rowno++;
            }

            FileStream fs = File.Create(filePath);
            workbook.Write(fs);
            fs.Close();
        }

        /// <summary>
        /// 导出至华晨宝马数据文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="packageCode"></param>
        /// <param name="listParams"></param>
        public static void exportCarParamsForHCBM(String filePath, String packageCode, String configCode)
        {
            if (String.IsNullOrEmpty(filePath) 
                || String.IsNullOrEmpty(packageCode) 
                || String.IsNullOrEmpty(configCode)) {
                return;
            }
            List<String> listPackageCode = packageCode.Split(new String[] { "_" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (listPackageCode.Count != 3) {
                return;
            }
            
            String medelCode = listPackageCode[0];
            String subPackageCode = listPackageCode[1];
            String profile = listPackageCode[2];

            FileStream excelFileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
            IWorkbook workbook = new HSSFWorkbook(excelFileStream);
            excelFileStream.Close();
            ISheet sheet = workbook.GetSheetAt(0);//取第一个表
            int columnNo = 0;
            int rowNo = 7;
            int rowCount = sheet.LastRowNum;
            int colCount = sheet.GetRow(0).LastCellNum;
            while (columnNo <= colCount)
            {
                IRow row = sheet.GetRow(rowNo);
                ICell cell = row.GetCell(columnNo);
                if (cell == null)
                {
                    columnNo++;
                    continue;
                }
                String cname = cell.StringCellValue;
                if (!String.IsNullOrEmpty(cname) && cname.Equals("Model"))
                {
                    break;
                }
                columnNo++;
            }

            while (rowNo <= sheet.LastRowNum)
            {
                String medelCodeValue = getValueFromSheet(sheet, rowNo, columnNo);
                String subPackageCodeValue = getValueFromSheet(sheet, rowNo, columnNo + 1);
                String orderCodeValue = getValueFromSheet(sheet, rowNo, columnNo + 2);
                if (medelCodeValue.Equals(medelCode) &&
                    subPackageCodeValue.Equals(subPackageCode) &&
                    orderCodeValue.Equals(profile))
                {
                    break;
                }
                rowNo++;
            }

            setValueToSheet(sheet, rowNo, 0, configCode);

            FileStream fs = File.Create(filePath);
            workbook.Write(fs);
            fs.Close();
        }

        //从参数list中获取名称为name的参数
        public static String getParameter(String cname, String pcode, List<Param> list)
        {
            var result = from d in list where d.cname == cname && d.patacode == pcode select d.value;
            foreach (var v in result)
            {
                //直接插入空串会让合并域消失，所以换成多个空格
                return v;
            }
            return null;
        }



        public static String getValueFromSheet(ISheet sheet, int rowNo, int columnNo)
        {
            IRow row = sheet.GetRow(rowNo);
            if (row == null)
            {
                return "";
            }
            ICell cell = row.GetCell(columnNo);
            if (cell == null)
            {
                return "";
            }
            String value = cell.StringCellValue;
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                return value;
            }
        }

        public static Boolean setValueToSheet(ISheet sheet, int rowNo, int columnNo, String value)
        {

            //ICell cell = sheet.GetRow(rowNo).GetCell(columnNo);
            //if (cell == null)
            //{
            //    cell = sheet.GetRow(rowNo).CreateCell(columnNo, CellType.String);
            //    cell.SetCellValue(value);
            //}
            //else {
            //    cell.SetCellValue(value);
            //}
            //return true;

            IRow row = sheet.GetRow(rowNo);
            if (row == null)
            {
                return false;
            }
            ICell cell = row.GetCell(columnNo);
            if (cell == null)
            {
                cell = sheet.GetRow(rowNo).CreateCell(columnNo, CellType.String);
                cell.SetCellValue(value);
            }else {
                cell.SetCellValue(value);
            }
            return true;
        }
    }
}
