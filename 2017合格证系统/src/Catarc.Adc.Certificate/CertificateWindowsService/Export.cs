using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Data.Common;

namespace CertificateWindowsService
{
    class Export
    {
       
        #region 导出数据到CSV文件 ExportDataToCsv
        /// <summary>
        /// 导出数据到CSV文件
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fileName"></param>
        public void ExportDataToCsv(DataTable dt, string fileName)
        {
            //获取路径
            string filePath = Path.GetDirectoryName(fileName);
            //判断路径是否存在
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(",", "，").Trim();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            string colData = data.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
            sw.WriteLine(colData);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString().Replace("\"", "\"").Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(",", "，").Trim();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                string rowData = data.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
                sw.WriteLine(rowData);
            }

            sw.Close();
            fs.Close();
        }
        #endregion

        #region 清空上一月份文件 ClearLastMonthData
        /// <summary>
        /// 清空上一月份文件
        /// </summary>
        /// <param name="path"></param>
        public void ClearLastMonthData(string path)
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyy-MM");
                string monthBegin = Convert.ToDateTime(datetime).ToString("yyyyMMdd");
                //IList<string> list = Directory.GetDirectories(path);
                DirectoryInfo di = new DirectoryInfo(path);
                IList<DirectoryInfo> list = di.GetDirectories();
                if (list != null && list.Count > 0)
                {
                    foreach (DirectoryInfo fileName in list)
                    {
                        if (Convert.ToInt32(fileName.Name) < Convert.ToInt32(monthBegin))
                        {
                            string allPath = Path.Combine(path,fileName.Name);
                            Directory.Delete(allPath, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
