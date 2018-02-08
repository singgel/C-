using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Common
{
    public class C2M
    {
        /// <summary>
        /// 选中选择项
        /// </summary>
        /// <param name="dv">数据源</param>
        /// <param name="column">返回选择列</param>
        /// <returns></returns>
        public static List<string> SelectedParamEntityIds(DataView dv, string column)
        {
            List<string> selectList = new List<string>();

            bool result = false;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    bool.TryParse(dv.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        selectList.Add(dv.Table.Rows[i][column].ToString());
                    }
                }
            }
            return selectList;
        }


        /// <summary>
        /// 选中选择项
        /// </summary>
        /// <param name="dv">数据源</param>
        /// <param name="column">返回选择列</param>
        /// <returns></returns>
        public static DataView SelectedParamEntityDataView(DataView dv, string column)
        {
            if (dv == null) return null;
            DataTable selectList = dv.Table.Clone();
            
            bool result = false;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    bool.TryParse(dv.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        //selectList.NewRow();
                        selectList.Rows.Add(dv.Table.Rows[i].ItemArray);
                    }
                }
            }
            return selectList.DefaultView;
        }

        /// <summary>
        /// List string 转换成 vins 逗号分隔
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string List2Str(List<string> list)
        {
            string vins = "(";
            foreach (string r in list)
            {
                vins += "'" + r + "'" + ",";
            }
            vins += "''" + ")";
            return vins;
        }
    }
}
