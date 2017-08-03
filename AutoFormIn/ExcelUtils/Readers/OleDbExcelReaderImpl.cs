using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


namespace ExcelUtils.Readers
{
    class OleDbExcelReaderImpl:Reader
    {
        public OleDbExcelReaderImpl()
        {

        }

        public String getCellValue(int row, int col, DataTable table)
        {
            return table.Rows[row][col].ToString();
        }

        public String getCellValue(int col, DataRow row)
        {
            return row[col].ToString();
        }

    }
}
