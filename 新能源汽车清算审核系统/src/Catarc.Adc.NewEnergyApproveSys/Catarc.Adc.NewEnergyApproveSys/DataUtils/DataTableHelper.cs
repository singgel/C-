using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace Catarc.Adc.NewEnergyApproveSys.DataUtils
{
    public class DataTableHelper
    {
        /// <summary>
        /// List装换为dataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> collection)
        {
            DataTable newDataTable = new DataTable();
            Type impliedType = typeof(T);
            PropertyInfo[] _propInfo = impliedType.GetProperties();
            foreach (PropertyInfo pi in _propInfo)
                newDataTable.Columns.Add(pi.Name, pi.PropertyType);

            foreach (T item in collection)
            {
                DataRow newDataRow = newDataTable.NewRow();
                newDataRow.BeginEdit();
                foreach (PropertyInfo pi in _propInfo)
                    newDataRow[pi.Name] = pi.GetValue(item, null);
                newDataRow.EndEdit();
                newDataTable.Rows.Add(newDataRow);
            }
            return newDataTable;
        }

        /// <summary>  
        /// 合并两个DataTable列  
        /// </summary>  
        /// <param name="dt1"></param>  
        /// <param name="dt2"></param>  
        /// <returns></returns>  
        public static DataTable MergeDataTable(DataTable dt1, DataTable dt2)
        {
            //定义dt的行数   
            int dtRowCount = 0;
            //dt的行数为dt1或dt2中行数最大的行数   
            if (dt1.Rows.Count > dt2.Rows.Count)
            {
                dtRowCount = dt1.Rows.Count;
            }
            else
            {
                dtRowCount = dt2.Rows.Count;
            }
            DataTable dt = new DataTable();
            //向dt中添加dt1的列名   
            for (int i = 0; i < dt1.Columns.Count; i++)
            {
                dt.Columns.Add(dt1.Columns[i].ColumnName + "1");
            }
            //向dt中添加dt2的列名   
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt.Columns.Add(dt2.Columns[i].ColumnName + "2");
            }
            for (int i = 0; i < dtRowCount; i++)
            {
                DataRow row = dt.NewRow();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    for (int k = 0; k < dt1.Columns.Count; k++) { if ((dt1.Rows.Count - 1) >= i) { row[k] = dt1.Rows[i].ItemArray[k]; } }
                    for (int k = 0; k < dt2.Columns.Count; k++) { if ((dt2.Rows.Count - 1) >= i) { row[dt1.Columns.Count + k] = dt2.Rows[i].ItemArray[k]; } }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 创建新table,测试数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="count">循环行数</param>
        /// <returns></returns>
        public static DataTable FillDataTable(Dictionary<string, string> data, int count)
        {
            DataTable dt = new DataTable();
            foreach (var col in data)
            {
                dt.Columns.Add(col.Key);
            }
            for (int i = 1; i < count; i++)
            {
                var dr = dt.NewRow();
                foreach (DataColumn item in dt.Columns)
                {
                    dr[item] = data[item.ColumnName];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 循环去除datatable中的空行
        /// </summary>
        /// <param name="dt"></param>
        public static void removeEmpty(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool rowdataisnull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    if (!string.IsNullOrEmpty(dt.Rows[i][j].ToString().Trim()))
                    {

                        rowdataisnull = false;
                    }

                }
                if (rowdataisnull)
                {
                    removelist.Add(dt.Rows[i]);
                }

            }
            for (int i = 0; i < removelist.Count; i++)
            {
                dt.Rows.Remove(removelist[i]);
            }
        }

        /// <summary>
        /// 循环去除dataSet中的空行
        /// </summary>
        /// <param name="ds"></param>
        public static void removeEmpty(DataSet ds)
        {
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTableHelper.removeEmpty(ds.Tables[i]);
            }
        }
    }
}
