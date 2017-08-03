using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    static class DataHandler
    {
        /// <summary>
        /// 泛亚国环数据特殊处理。
        /// </summary>
        /// <param name="data"></param>
        public static void HandleFYData_GH(Hashtable data)
        {
            // GB326A——车轴；GB327A——轮胎型号；GB328A——所有可选择的轮胎厂牌
            string[] empty = new string[0];
            string[] keys = new string[] { "GB326A", "GB327A", "GB328A" };
            int count = -1;
            int length = 0;
            if (int.TryParse(GetFY_Data(data[keys[0]]), out count))
            {
                string str = GetFY_Data(data[keys[1]]);
                string[] gb327a = str == null ? empty : str.Split(','); // 原始数据使用','分割
                str = data[keys[2]] as string;
                string[] gb328a = str == null ? empty : str.Split(','); // 原始数据使用','分割
                length = Math.Max(gb327a.Length, gb328a.Length);
                StringBuilder buffer326A = new StringBuilder(), buffer327A = new StringBuilder(), buffer328A = new StringBuilder();
                for (int j = 0; j < length; j++)
                {
                    for (int i = 0; i < count; i++)
                    {
                        buffer326A.Append(i + 1);
                        buffer326A.Append(";");
                        buffer327A.Append(gb327a.Length == 0 ? "" : (gb327a.Length > j ? gb327a[j] : gb327a[gb327a.Length - 1]));
                        buffer327A.Append(";");
                        buffer328A.Append(gb328a.Length == 0 ? "" : (gb328a.Length > j ? gb328a[j] : gb328a[gb328a.Length - 1]));
                        buffer328A.Append(";");
                    }
                }

                if (buffer326A.Length > 0)
                {
                    buffer326A.Remove(buffer326A.Length - 1, 1);
                    buffer327A.Remove(buffer327A.Length - 1, 1);
                    buffer328A.Remove(buffer328A.Length - 1, 1);
                }
                object[] array = null;
                array = data[keys[0]] as object[];
                array[0] = buffer326A.ToString();

                array = data[keys[1]] as object[];
                array[0] = buffer327A.ToString();

                array = data[keys[2]] as object[];
                array[0] = buffer328A.ToString();
            }
        }

        private static string GetFY_Data(object value)
        {
            object[] array = value as object[];
            return array == null || array.Length <= 1 ? "" : array[0] as string;
        }
    }
}
