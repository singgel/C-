using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace Common
{
    public class DataTableHelper
    {
        public  void CompareDt2(DataTable dt1, DataTable dt2, List<string> keyList,
           out DataTable dtRetAdd, ref DataTable dtRetDiff, out DataTable dtRetDel)
        {
            try
            {
                //为三个表拷贝表结构
                dtRetDel = dt1.Clone();
                dtRetAdd = dtRetDel.Clone();
                //dtRetDiff = dtRetDel.Clone();

                foreach (DataColumn col in dtRetDel.Columns)
                {
                    dtRetDiff.Columns.Add(col.ColumnName+"_官方");
                    dtRetDiff.Columns.Add(col.ColumnName+"_本地");
                }

                int colCount = dt1.Columns.Count;

                DataView dv1 = dt1.DefaultView;
                DataView dv2 = dt2.DefaultView;

                string strCompare = string.Empty;

                //先以第一个表为参照，看第二个表是修改了还是删除了
                foreach (DataRowView dr1 in dv1)
                {
                    strCompare = string.Empty;
                    foreach (string key in keyList)
                    {
                        strCompare += string.Format("and {0} = '{1}' ", key, dr1[key].ToString());
                    }
                    if (!string.IsNullOrEmpty(strCompare))
                    {
                        strCompare = strCompare.Substring(4);
                    }

                    dv2.RowFilter = strCompare;
                    if (dv2.Count > 0)
                    {
                        CompareUpdate2(dr1, dv2[0], ref dtRetDiff, keyList);
                        //if (!this.CompareUpdate2(dr1, dv2[0], ref dtRetDiff))//比较是否有不同的
                        //{
                        //dtRetDiff.Rows.Add(dr1.Row.ItemArray);//修改前
                        //dtRetDif2.Rows[dtRetDif2.Rows.Count - 1]["FID"] = dr1.Row["FID"];//将ID赋给来自文件的表，因为它的ID全部==0
                        //continue;
                        //}
                    }
                    else
                    {
                        //已经被删除的
                        dtRetDel.Rows.Add(dr1.Row.ItemArray);
                    }
                }

                //以第一个表为参照，看记录是否是新增的
                dv2.RowFilter = "";//清空条件
                foreach (DataRowView dr2 in dv2)
                {
                    strCompare = string.Empty;
                    foreach (string key in keyList)
                    {
                        strCompare += string.Format("and {0} = '{1}'", key, dr2[key].ToString());
                    }
                    if (!string.IsNullOrEmpty(strCompare))
                    {
                        strCompare = strCompare.Substring(4);
                    }
                    dv1.RowFilter = strCompare;
                    if (dv1.Count == 0)
                    {
                        //新增的
                        dtRetAdd.Rows.Add(dr2.Row.ItemArray);
                    }
                }
        
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void CompareDt22(DataTable dt1, DataTable dt2, List<string> keyList,
           out DataTable dtRetAdd, ref DataTable dtRetDiff, out DataTable dtRetDel)
        {
            try
            {
                //为三个表拷贝表结构
                dtRetDel = dt1.Clone();
                dtRetAdd = dtRetDel.Clone();
                //dtRetDiff = dtRetDel.Clone();

                foreach (DataColumn col in dtRetDel.Columns)
                {
                    dtRetDiff.Columns.Add(col.ColumnName + "_官方");
                    dtRetDiff.Columns.Add(col.ColumnName + "_通告");
                }

                int colCount = dt1.Columns.Count;

                DataView dv1 = dt1.DefaultView;
                DataView dv2 = dt2.DefaultView;

                string strCompare = string.Empty;

                //先以第一个表为参照，看第二个表是修改了还是删除了
                foreach (DataRowView dr1 in dv1)
                {
                    strCompare = string.Empty;
                    foreach (string key in keyList)
                    {
                        strCompare += string.Format("and {0} = '{1}' ", key, dr1[key].ToString());
                    }
                    if (!string.IsNullOrEmpty(strCompare))
                    {
                        strCompare = strCompare.Substring(4);
                    }

                    dv2.RowFilter = strCompare;
                    if (dv2.Count > 0)
                    {
                        CompareUpdate2(dr1, dv2[0], ref dtRetDiff, keyList);
                        //if (!this.CompareUpdate2(dr1, dv2[0], ref dtRetDiff))//比较是否有不同的
                        //{
                        //dtRetDiff.Rows.Add(dr1.Row.ItemArray);//修改前
                        //dtRetDif2.Rows[dtRetDif2.Rows.Count - 1]["FID"] = dr1.Row["FID"];//将ID赋给来自文件的表，因为它的ID全部==0
                        //continue;
                        //}
                    }
                    else
                    {
                        //已经被删除的
                        dtRetDel.Rows.Add(dr1.Row.ItemArray);
                    }
                }

                //以第一个表为参照，看记录是否是新增的
                dv2.RowFilter = "";//清空条件
                foreach (DataRowView dr2 in dv2)
                {
                    strCompare = string.Empty;
                    foreach (string key in keyList)
                    {
                        strCompare += string.Format("and {0} = '{1}'", key, dr2[key].ToString());
                    }
                    if (!string.IsNullOrEmpty(strCompare))
                    {
                        strCompare = strCompare.Substring(4);
                    }
                    dv1.RowFilter = strCompare;
                    if (dv1.Count == 0)
                    {
                        //新增的
                        dtRetAdd.Rows.Add(dr2.Row.ItemArray);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




        //比较是否有不同的
        private void CompareUpdate2(DataRowView dr1, DataRowView dr2, ref DataTable dtDiff, List<string> keyList)
        {
            //行里只要有一项不一样，整个行就不一样,无需比较其它
            string val1;
            string val2;
            bool flag = true;
            DataRow dr = null;
            for (int i = 0; i < dr1.Row.ItemArray.Length; i++)
            {
                val1 = Convert.ToString(dr1[i]);
                val2 = Convert.ToString(dr2[i]);
                if (!val1.Equals(val2))
                {
                    if (flag)
                    {
                        dr = dtDiff.NewRow();
                        foreach (string key in keyList)
                        {
                            dr[key + "_官方"] = dr1[key].ToString();
                        }
                        flag = false;
                    }
                    dr[2 * i] = val1.ToString();
                    dr[2 * i + 1] = val2.ToString();
                }
            }
            if (!flag)
            {
                dtDiff.Rows.Add(dr);
            }
        }


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


      

        #region 合并两个DataTable列
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
        #endregion  


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



        //循环去除datatable中的空行
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
    }
}
