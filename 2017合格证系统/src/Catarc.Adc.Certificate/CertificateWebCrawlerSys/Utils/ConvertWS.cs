using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace CertificateWebCrawlerSys.Utils
{
    public class ConvertWS
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

        public DataTable getListWS(string htmlContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("HGZBH", typeof(System.String));
            dt.Columns.Add("SWSBM", typeof(System.String));
            dt.Columns.Add("CJSJ", typeof(System.DateTime));
            dt.Columns.Add("RESOURCE_ID", typeof(System.String));
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
                    dr["HGZBH"] = tdNodes[0].InnerText;
                    dr["SWSBM"] = tdNodes[1].InnerText;
                    dr["CJSJ"] = tdNodes[2].InnerText;
                    string hrf = tdNodes[3].ChildNodes[0].Attributes["href"].Value;
                    dr["RESOURCE_ID"] = hrf.Substring(hrf.IndexOf("RESOURCEID=") + 11, hrf.Length - hrf.IndexOf("RESOURCEID=") - 11);
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

        public DataTable getDetailsWS(string resourceID, string htmlContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("HGZBH", typeof(System.String));
            dt.Columns.Add("SWSBM", typeof(System.String));
            dt.Columns.Add("CJSJ", typeof(System.DateTime));
            dt.Columns.Add("CREATETIME", typeof(System.DateTime));
            dt.Columns.Add("UPDATETIME", typeof(System.DateTime));
            dt.Columns.Add("RESOURCE_ID", typeof(System.String));
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection trNodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]/tr");
                DataRow dr = dt.NewRow();
                dr["CREATETIME"] = DateTime.Now;
                dr["UPDATETIME"] = DateTime.Now;
                dr["RESOURCE_ID"] = resourceID;
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
