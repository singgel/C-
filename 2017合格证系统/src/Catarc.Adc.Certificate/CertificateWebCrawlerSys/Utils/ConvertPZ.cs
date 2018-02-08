using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace CertificateWebCrawlerSys.Utils
{
    public class ConvertPZ
    {
        public int getPageNumHGZ(string htmlContent)
        {
            int iStart = htmlContent.IndexOf("CreateNav(");
            if (iStart > 0)
            {
                string result4 = htmlContent.Substring(iStart + 10);
                int iEnd = result4.IndexOf(",");
                string result5 = result4.Substring(0, iEnd);
                try
                {
                    return Convert.ToInt32(result5);
                }
                catch (Exception ex)
                {
                    return 0; 
                }
            }
            else
            {
                return 0;
            }
        }

        public DataTable getListPZ(string htmlContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PZXLH", typeof(System.String));
            dt.Columns.Add("QYMC", typeof(System.String));
            dt.Columns.Add("CPXH", typeof(System.String));
            dt.Columns.Add("PZ_UPDATETIME", typeof(System.String));
            dt.Columns.Add("CREATETIME", typeof(System.DateTime));
            dt.Columns.Add("UPDATETIME", typeof(System.DateTime));
            try
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection trNodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]/tr");
                foreach (HtmlNode n in trNodes)
                {
                    DataRow dr = dt.NewRow();
                    HtmlNodeCollection tdNodes = n.SelectNodes("td");
                    dr["PZXLH"] = tdNodes[0].InnerText;
                    dr["QYMC"] = tdNodes[1].InnerText;
                    dr["CPXH"] = tdNodes[2].InnerText;
                    dr["PZ_UPDATETIME"] = tdNodes[3].InnerText;
                    dr["CREATETIME"] = DateTime.Now;
                    dr["UPDATETIME"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            return dt;
        }

        public DataTable getDetailsPZ(string htmlContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PZXLH", typeof(System.String));
            dt.Columns.Add("CLLX", typeof(System.String));
            dt.Columns.Add("QYDM", typeof(System.String));
            dt.Columns.Add("HGZQYMC", typeof(System.String));
            dt.Columns.Add("QYMC", typeof(System.String));
            dt.Columns.Add("CPXH", typeof(System.String));
            dt.Columns.Add("DW", typeof(System.String));
            dt.Columns.Add("ZW", typeof(System.String));
            dt.Columns.Add("PQL", typeof(System.String));
            dt.Columns.Add("GB", typeof(System.String));
            dt.Columns.Add("JBPZ", typeof(System.String));
            dt.Columns.Add("CJSJ", typeof(System.DateTime));
            dt.Columns.Add("GXSJ", typeof(System.DateTime));
            dt.Columns.Add("CREATETIME", typeof(System.DateTime));
            dt.Columns.Add("UPDATETIME", typeof(System.DateTime));
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection trNodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]/tr");
                DataRow dr = dt.NewRow();
                dr["CREATETIME"] = DateTime.Now;
                dr["UPDATETIME"] = DateTime.Now;
                for (int i = 0; i < trNodes.Count; i++)
                {
                    string tdValue = trNodes[i].SelectNodes("td")[1].InnerText;
                    if (dt.Columns[i].DataType == typeof(DateTime))
                    {
                        dr[i] = string.IsNullOrEmpty(tdValue) == true ? " 1753-1-1 12:00:00" : tdValue;
                    }
                    else
                    {
                        dr[i] = tdValue;
                    }
                }
                dt.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            return dt;
        }
    }
}
