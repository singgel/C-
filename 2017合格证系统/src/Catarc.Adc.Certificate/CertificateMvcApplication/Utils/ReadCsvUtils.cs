using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;
using System.Text;
using CertificateMvcApplication.Models;

namespace CertificateMvcApplication.Utils
{
    public class ReadCsvUtils
    {
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCSV(string floderName, string fileName)
        {
            string floder = System.Configuration.ConfigurationManager.AppSettings["FilePath"];
            string filePath = Path.Combine(floder, floderName, fileName);
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs,  Encoding.GetEncoding("GBK"));//Encoding.UTF8);
            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            while ((strLine = sr.ReadLine()) != null)
            {
                if (IsFirst == true)
                {
                    tableHead = strLine.Split(',');
                    IsFirst = false;
                    columnCount = tableHead.Length;
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static List<HGZ_APPLIC_PART> readCSV(string floderName, string fileName)
        {
            List<HGZ_APPLIC_PART> lsData = new List<HGZ_APPLIC_PART>();
            HGZ_APPLIC_PART model = new HGZ_APPLIC_PART();
            string floder = System.Configuration.ConfigurationManager.AppSettings["FilePath"];
            string filePath = Path.Combine(floder, floderName, fileName);
            FileStream fs = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
            string strLine = "";
            string[] aryLine = null;
            string[] tableHead = null;
            int columnCount = 0;
            bool IsFirst = true;
            string colName = null;
            var items = model.GetType().GetProperties();
            string[] showCols = new string[] { "H_ID", "CJH", "WZHGZBH", "FZRQ", "CLZZQYMC", "CLLX", "CLMC", "CLPP", "CLXH", "DPXH", "DPHGZBH", "FDJXH" };
            while ((strLine = sr.ReadLine()) != null)
            {
                model = new HGZ_APPLIC_PART();
                if (IsFirst == true)
                {
                    IsFirst = false;
                    tableHead = strLine.Split(',');
                    columnCount = tableHead.Length;
                }
                else
                {
                    aryLine = strLine.Split(',');
                    int iPart = 0;
                    for (int j = 0; j < columnCount; j++)
                    {
                        colName = tableHead[j];
                        if (showCols.Contains(colName))
                        {
                            var item = items[iPart];
                            var val = aryLine[j];
                            if (!string.IsNullOrEmpty(aryLine[j]))
                            {
                                item.SetValue(model, aryLine[j], null);
                            }
                            iPart++;
                        }
                    }
                    lsData.Add(model);
                }
            }
            sr.Close();
            fs.Close();
            return lsData;
        }

    }
}