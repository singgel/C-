using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using CertificateWebCrawlerSys.Utils;
using CertificateWebCrawlerSys.Helper;
using HtmlAgilityPack;

namespace CertificateWebCrawlerSys.Utils
{
    public class ConvertHGZ
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

        public DataTable getListHGZ(string htmlContent,String page)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SQBH", typeof(System.String));
            dt.Columns.Add("HGZBH", typeof(System.String));
            dt.Columns.Add("CLSBDH", typeof(System.String));
            dt.Columns.Add("APP_TYPE", typeof(System.String));
            dt.Columns.Add("APP_TIME", typeof(System.DateTime));
            dt.Columns.Add("CREATETIME", typeof(System.DateTime));
            dt.Columns.Add("UPDATETIME", typeof(System.DateTime));
            dt.Columns.Add("PAGE", typeof(System.String));
            try
            {
                
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                HtmlNodeCollection trNodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]/tr");
                foreach (HtmlNode n in trNodes)
                {
                    DataRow dr = dt.NewRow();
                    HtmlNodeCollection tdNodes = n.SelectNodes("td");
                    dr["SQBH"] = tdNodes[0].InnerText;
                    dr["HGZBH"] = tdNodes[1].InnerText;
                    dr["CLSBDH"] = tdNodes[2].InnerText;
                    dr["APP_TYPE"] = tdNodes[3].InnerText;
                    dr["APP_TIME"] = tdNodes[4].InnerText;
                    dr["CREATETIME"] = DateTime.Now;
                    dr["UPDATETIME"] = DateTime.Now;
                    dr["PAGE"] = page;
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            return dt;
        }

        public DataTable getDetailsHGZ(string htmlContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SQXLH", typeof(System.String));
            dt.Columns.Add("CLZTXX", typeof(System.String));
            dt.Columns.Add("HGZBH", typeof(System.String));
            dt.Columns.Add("CLSBDH", typeof(System.String));
            dt.Columns.Add("PZXLH", typeof(System.String));
            dt.Columns.Add("FZRQ", typeof(System.DateTime));
            dt.Columns.Add("CLZZQYMC", typeof(System.String));
            dt.Columns.Add("CLLX", typeof(System.String));
            dt.Columns.Add("CLMC", typeof(System.String));
            dt.Columns.Add("CLPP", typeof(System.String));
            dt.Columns.Add("CLXH", typeof(System.String));
            dt.Columns.Add("CSYS", typeof(System.String));
            dt.Columns.Add("DPXH", typeof(System.String));
            dt.Columns.Add("DPID", typeof(System.String));
            dt.Columns.Add("DPHGZBH", typeof(System.String));
            dt.Columns.Add("CJH", typeof(System.String));
            dt.Columns.Add("FDJXH", typeof(System.String));
            dt.Columns.Add("FDJH", typeof(System.String));
            dt.Columns.Add("RLZL", typeof(System.String));
            dt.Columns.Add("PFBZ", typeof(System.String));
            dt.Columns.Add("PL", typeof(System.String));
            dt.Columns.Add("GL", typeof(System.String));
            dt.Columns.Add("ZXXS", typeof(System.String));
            dt.Columns.Add("QLJ", typeof(System.String));
            dt.Columns.Add("HLJ", typeof(System.String));
            dt.Columns.Add("LTS", typeof(System.String));
            dt.Columns.Add("LTGG", typeof(System.String));
            dt.Columns.Add("GBTHPS", typeof(System.String));
            dt.Columns.Add("ZJ", typeof(System.String));
            dt.Columns.Add("ZH", typeof(System.String));
            dt.Columns.Add("ZS", typeof(System.String));
            dt.Columns.Add("WKC", typeof(System.String));
            dt.Columns.Add("WKK", typeof(System.String));
            dt.Columns.Add("WKG", typeof(System.String));
            dt.Columns.Add("HXNBC", typeof(System.String));
            dt.Columns.Add("HXNBK", typeof(System.String));
            dt.Columns.Add("HXNBG", typeof(System.String));
            dt.Columns.Add("ZZL", typeof(System.String));
            dt.Columns.Add("EDZZL", typeof(System.String));
            dt.Columns.Add("ZBZL", typeof(System.String));
            dt.Columns.Add("ZZLLYXS", typeof(System.String));
            dt.Columns.Add("ZQYZZL", typeof(System.String));
            dt.Columns.Add("EDZK", typeof(System.String));
            dt.Columns.Add("BGCAZZDYXZZL", typeof(System.String));
            dt.Columns.Add("JSSZCRS", typeof(System.String));
            dt.Columns.Add("ZXZS", typeof(System.String));
            dt.Columns.Add("ZGSJCS", typeof(System.String));
            dt.Columns.Add("CLZZRQ", typeof(System.DateTime));
            dt.Columns.Add("BZ", typeof(System.String));
            dt.Columns.Add("QYBZ", typeof(System.String));
            dt.Columns.Add("CLSCDWMC", typeof(System.String));
            dt.Columns.Add("CPSCDZ", typeof(System.String));
            dt.Columns.Add("QYQTXX", typeof(System.String));
            dt.Columns.Add("YH", typeof(System.String));
            dt.Columns.Add("CPH", typeof(System.String));
            dt.Columns.Add("PC", typeof(System.String));
            dt.Columns.Add("GGSXRQ", typeof(System.String));
            dt.Columns.Add("SCSJ", typeof(System.String));
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
                for (int i=0;i<trNodes.Count;i++)
                {
                    dr[i] = trNodes[i].SelectNodes("td")[1].InnerText;
                }
                dt.Rows.Add(dr);
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
            return dt;
        }

        public DataTable getDetailsHGZ(string appTime,string appType,string htmlContent)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SQXLH", typeof(System.String));
            dt.Columns.Add("CLZTXX", typeof(System.String));
            dt.Columns.Add("HGZBH", typeof(System.String));
            dt.Columns.Add("CLSBDH", typeof(System.String));
            dt.Columns.Add("PZXLH", typeof(System.String));
            dt.Columns.Add("FZRQ", typeof(System.DateTime));
            dt.Columns.Add("CLZZQYMC", typeof(System.String));
            dt.Columns.Add("CLLX", typeof(System.String));
            dt.Columns.Add("CLMC", typeof(System.String));
            dt.Columns.Add("CLPP", typeof(System.String));
            dt.Columns.Add("CLXH", typeof(System.String));
            dt.Columns.Add("CSYS", typeof(System.String));
            dt.Columns.Add("DPXH", typeof(System.String));
            dt.Columns.Add("DPID", typeof(System.String));
            dt.Columns.Add("DPHGZBH", typeof(System.String));
            dt.Columns.Add("CJH", typeof(System.String));
            dt.Columns.Add("FDJXH", typeof(System.String));
            dt.Columns.Add("FDJH", typeof(System.String));
            dt.Columns.Add("RLZL", typeof(System.String));
            dt.Columns.Add("PFBZ", typeof(System.String));
            dt.Columns.Add("PL", typeof(System.String));
            dt.Columns.Add("GL", typeof(System.String));
            dt.Columns.Add("ZXXS", typeof(System.String));
            dt.Columns.Add("QLJ", typeof(System.String));
            dt.Columns.Add("HLJ", typeof(System.String));
            dt.Columns.Add("LTS", typeof(System.String));
            dt.Columns.Add("LTGG", typeof(System.String));
            dt.Columns.Add("GBTHPS", typeof(System.String));
            dt.Columns.Add("ZJ", typeof(System.String));
            dt.Columns.Add("ZH", typeof(System.String));
            dt.Columns.Add("ZS", typeof(System.String));
            dt.Columns.Add("WKC", typeof(System.String));
            dt.Columns.Add("WKK", typeof(System.String));
            dt.Columns.Add("WKG", typeof(System.String));
            dt.Columns.Add("HXNBC", typeof(System.String));
            dt.Columns.Add("HXNBK", typeof(System.String));
            dt.Columns.Add("HXNBG", typeof(System.String));
            dt.Columns.Add("ZZL", typeof(System.String));
            dt.Columns.Add("EDZZL", typeof(System.String));
            dt.Columns.Add("ZBZL", typeof(System.String));
            dt.Columns.Add("ZZLLYXS", typeof(System.String));
            dt.Columns.Add("ZQYZZL", typeof(System.String));
            dt.Columns.Add("EDZK", typeof(System.String));
            dt.Columns.Add("BGCAZZDYXZZL", typeof(System.String));
            dt.Columns.Add("JSSZCRS", typeof(System.String));
            dt.Columns.Add("ZXZS", typeof(System.String));
            dt.Columns.Add("ZGSJCS", typeof(System.String));
            dt.Columns.Add("CLZZRQ", typeof(System.DateTime));
            dt.Columns.Add("BZ", typeof(System.String));
            dt.Columns.Add("QYBZ", typeof(System.String));
            dt.Columns.Add("CLSCDWMC", typeof(System.String));
            dt.Columns.Add("CPSCDZ", typeof(System.String));
            dt.Columns.Add("QYQTXX", typeof(System.String));
            dt.Columns.Add("YH", typeof(System.String));
            dt.Columns.Add("CPH", typeof(System.String));
            dt.Columns.Add("PC", typeof(System.String));
            dt.Columns.Add("GGSXRQ", typeof(System.DateTime));
            dt.Columns.Add("SCSJ", typeof(System.DateTime));
            dt.Columns.Add("APP_TIME", typeof(System.DateTime));
            dt.Columns.Add("APP_TYPE", typeof(System.String));
            dt.Columns.Add("CREATETIME", typeof(System.DateTime));
            dt.Columns.Add("UPDATETIME", typeof(System.DateTime));
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);
                HtmlNodeCollection trNodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]/tr");
                DataRow dr = dt.NewRow();
                dr["APP_TIME"] = appTime;
                dr["APP_TYPE"] = appType;
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

        public DataTable getDetailsHGZTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SQXLH", typeof(System.String));
            dt.Columns.Add("CLZTXX", typeof(System.String));
            dt.Columns.Add("HGZBH", typeof(System.String));
            dt.Columns.Add("CLSBDH", typeof(System.String));
            dt.Columns.Add("PZXLH", typeof(System.String));
            dt.Columns.Add("FZRQ", typeof(System.DateTime));
            dt.Columns.Add("CLZZQYMC", typeof(System.String));
            dt.Columns.Add("CLLX", typeof(System.String));
            dt.Columns.Add("CLMC", typeof(System.String));
            dt.Columns.Add("CLPP", typeof(System.String));
            dt.Columns.Add("CLXH", typeof(System.String));
            dt.Columns.Add("CSYS", typeof(System.String));
            dt.Columns.Add("DPXH", typeof(System.String));
            dt.Columns.Add("DPID", typeof(System.String));
            dt.Columns.Add("DPHGZBH", typeof(System.String));
            dt.Columns.Add("CJH", typeof(System.String));
            dt.Columns.Add("FDJXH", typeof(System.String));
            dt.Columns.Add("FDJH", typeof(System.String));
            dt.Columns.Add("RLZL", typeof(System.String));
            dt.Columns.Add("PFBZ", typeof(System.String));
            dt.Columns.Add("PL", typeof(System.String));
            dt.Columns.Add("GL", typeof(System.String));
            dt.Columns.Add("ZXXS", typeof(System.String));
            dt.Columns.Add("QLJ", typeof(System.String));
            dt.Columns.Add("HLJ", typeof(System.String));
            dt.Columns.Add("LTS", typeof(System.String));
            dt.Columns.Add("LTGG", typeof(System.String));
            dt.Columns.Add("GBTHPS", typeof(System.String));
            dt.Columns.Add("ZJ", typeof(System.String));
            dt.Columns.Add("ZH", typeof(System.String));
            dt.Columns.Add("ZS", typeof(System.String));
            dt.Columns.Add("WKC", typeof(System.String));
            dt.Columns.Add("WKK", typeof(System.String));
            dt.Columns.Add("WKG", typeof(System.String));
            dt.Columns.Add("HXNBC", typeof(System.String));
            dt.Columns.Add("HXNBK", typeof(System.String));
            dt.Columns.Add("HXNBG", typeof(System.String));
            dt.Columns.Add("ZZL", typeof(System.String));
            dt.Columns.Add("EDZZL", typeof(System.String));
            dt.Columns.Add("ZBZL", typeof(System.String));
            dt.Columns.Add("ZZLLYXS", typeof(System.String));
            dt.Columns.Add("ZQYZZL", typeof(System.String));
            dt.Columns.Add("EDZK", typeof(System.String));
            dt.Columns.Add("BGCAZZDYXZZL", typeof(System.String));
            dt.Columns.Add("JSSZCRS", typeof(System.String));
            dt.Columns.Add("ZXZS", typeof(System.String));
            dt.Columns.Add("ZGSJCS", typeof(System.String));
            dt.Columns.Add("CLZZRQ", typeof(System.DateTime));
            dt.Columns.Add("BZ", typeof(System.String));
            dt.Columns.Add("QYBZ", typeof(System.String));
            dt.Columns.Add("CLSCDWMC", typeof(System.String));
            dt.Columns.Add("CPSCDZ", typeof(System.String));
            dt.Columns.Add("QYQTXX", typeof(System.String));
            dt.Columns.Add("YH", typeof(System.String));
            dt.Columns.Add("CPH", typeof(System.String));
            dt.Columns.Add("PC", typeof(System.String));
            dt.Columns.Add("GGSXRQ", typeof(System.DateTime));
            dt.Columns.Add("SCSJ", typeof(System.DateTime));
            dt.Columns.Add("APP_TIME", typeof(System.DateTime));
            dt.Columns.Add("APP_TYPE", typeof(System.String));
            dt.Columns.Add("CREATETIME", typeof(System.DateTime));
            dt.Columns.Add("UPDATETIME", typeof(System.DateTime));
            return dt;
        }
    }
}
