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

namespace Assistant.DataProviders.VOLVO
{
    /// <summary>
    /// 上海泛亚国环数据提供类。
    /// </summary>
    public class GHDataProvider : NotifyPropertyChanged, IDataProvider
    {
        private string _connectionString;
        private DataSet conn;

        private static readonly PropertyChangedEventArgs ProductsChanged = new PropertyChangedEventArgs("Products");
        private static readonly PropertyChangedEventArgs SearchStringChanged = new PropertyChangedEventArgs("SearchString");

        private string sheetname = "国环";

        string IDataProvider.DataSourceFile
        {
            get { return _connectionString; }
            set
            {
                if (conn != null)
                {
                    conn.Clear();
                }
                conn = null;
                if (value != null)
                {
                    conn = new DataSet();
                    string strConn;
                    if (Path.GetExtension(value) == ".xls")
                    {
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + value + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
                    }
                    else if (Path.GetExtension(value) == ".xlsx")
                    {
                        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + value + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1';";
                    }
                    else
                    {
                        throw (new Exception("提交的文件不是excel文件！"));
                    }
                    var sheet = ExcelSheetName(value);
                    if (sheet.Count > 0) {
                        sheetname = sheet[0];
                    }
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheetname + "$]", strConn);
                    oada.Fill(conn);
                }
                _connectionString = value;
            }
        }

        bool IDataProvider.AllowAlternately
        {
            get { return false; }
        }

        public GHDataProvider()
        {
        }

        void IDataProvider.Clean()
        {
        }

        ValueConverter IDataProvider.GetConverter()
        {
            return new WebValueConverter();
        }

        object IDataProvider.ProvideData(object state)
        {
            if (state != null)
                return null;
            if (conn == null)
                throw new ArgumentException("未指定有效的数据源！");
            Hashtable result = new Hashtable();
            DataTable data = conn.Tables[0];
            string keyStr = "国环编码";
            string valueStr = "参数值";
            int keyIndex = -1;
            int valueIndex = -1;
            for (int j = 0; j < data.Rows.Count; j++)
            {
                if (keyIndex < 0 || valueIndex < 0)
                {
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        string str = data.Rows[j][i].ToString();
                        if (keyStr == str) { keyIndex = i; }
                        if (valueStr == str) { valueIndex = i; }
                        if (keyIndex >= 0 && valueIndex >= 0) break;
                    }
                }
                else 
                {
                    string key = data.Rows[j][keyIndex].ToString();
                    if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key)) 
                    {
                        string value = data.Rows[j][valueIndex].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            result.Add(key, value);
                        }
                    }
                }
            }
            return result;
        }

        private void InitProductList()
        {
        }

        bool IDataProvider.ShowWindow()
        {
            return false;
        }

        private static List<string> ExcelSheetName(string filepath)
        {
            List<string> al = new List<string>();
            string strConn;
            if (Path.GetExtension(filepath) == ".xls")
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filepath + ";Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            }
            else
            {
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filepath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1';";
            }
            OleDbConnection connection = new OleDbConnection(strConn);
            connection.Open();
            DataTable sheetNames = connection.GetOleDbSchemaTable
            (System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            connection.Close();
            foreach (DataRow dr in sheetNames.Rows)
            {
                al.Add(dr[2].ToString().Replace("$", ""));
            }
            return al;
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
