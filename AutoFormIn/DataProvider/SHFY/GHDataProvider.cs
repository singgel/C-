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
    /// 上海泛亚国环数据提供类。
    /// </summary>
    public class GHDataProvider : NotifyPropertyChanged, IDataProvider
    {
        private string _connectionString;
        private DataSet conn;

        private static readonly PropertyChangedEventArgs ProductsChanged = new PropertyChangedEventArgs("Products");
        private static readonly PropertyChangedEventArgs SearchStringChanged = new PropertyChangedEventArgs("SearchString");

        private string sheetname = "汽油车";

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

        DataProviders.ValueConverter IDataProvider.GetConverter()
        {
            return new ValueConverter();
        }

        object IDataProvider.ProvideData(object state)
        {
            if (state != null)
                return null;
            if (conn == null)
                throw new ArgumentException("未指定有效的数据源！");
            Hashtable result = new Hashtable();
            DataTable data = conn.Tables[0];
            string keyStr = "标识符";
            string valueStr = "参数值";
            string codeStr = "PATAC统一参数编码";
            int keyIndex = -1;
            int valueIndex = -1;
            int codeIndex = -1;
            for (int j = 0; j < data.Rows.Count; j++)
            {
                if (keyIndex < 0 || valueIndex < 0)
                {
                    for (int i = 0; i < data.Columns.Count; i++)
                    {
                        string str = data.Rows[j][i].ToString();
                        if (keyStr == str) { keyIndex = i; }
                        if (valueStr == str) { valueIndex = i; }
                        if (codeStr == str) codeIndex = i;
                        if (keyIndex >= 0 && valueIndex >= 0 && codeIndex>=0) break;
                    }
                }
                else 
                {
                    string key = data.Rows[j][keyIndex].ToString();
                    if (!string.IsNullOrEmpty(key) && !result.ContainsKey(key)) 
                    {
                        string value = data.Rows[j][valueIndex].ToString();
                        result.Add(key, new object[] { value, data.Rows[j][codeIndex] });
                    }
                }
            }
            DataHandler.HandleFYData_GH(result);
            return result;
        }

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
