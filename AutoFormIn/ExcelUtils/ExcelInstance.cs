using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

using ExcelUtils.Readers;

namespace ExcelUtils
{
    public class ExcelInstance
    {
        internal ExcelInstance(String conStr)
        {
            this.conStr = conStr;
            inialize(ExcelInstance.SELECT_ALL_COMMAND);
        }

        internal ExcelInstance(String conStr, String sheetName)
        {
            this.conStr = conStr;
            this.sheetName = sheetName;
            String selectCommand = "select * from ["+this.sheetName+"$]";
            inialize(selectCommand);
        }

        private void inialize(String selectCommand)
        {
            this.connection = new OleDbConnection(conStr);
            this.dataAdapter = new OleDbDataAdapter(selectCommand, connection);

            this.dataSet = new DataSet();
            this.dataAdapter.Fill(this.dataSet);

            this.dataTables = dataSet.Tables;
        }

        private String conStr = null;
        private OleDbConnection connection = null;
        private OleDbDataAdapter dataAdapter = null;

        // 可更改变量类

        public const String SELECT_ALL_COMMAND = "select * from [Sheet1$]";

        public String sheetName = "Sheet1";

        public DataSet dataSet = null;

        public DataTableCollection dataTables = null;


    }
}
