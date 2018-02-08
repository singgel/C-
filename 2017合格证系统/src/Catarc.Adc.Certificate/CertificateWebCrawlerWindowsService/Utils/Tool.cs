using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using CertificateWebCrawlerWindowsService.Utils;
using CertificateWebCrawlerWindowsService.Helper;
using HtmlAgilityPack;

namespace CertificateWebCrawlerWindowsService.Utils
{
    public class Tool
    {
        static LogManager logMgr = new LogManager();
        public static CookieContainer cookieContainer = new CookieContainer();

        public static string strLoginUrl = "http://resource.autoidc.cn/pages/login.aspx";
        public static string strTargerUrl1 = "http://resource.autoidc.cn/Default.aspx";

        public static string strTargerUrlListHGZ = "http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=HGZ&PAGEID=";
        public static string strTargerUrlDetailsHGZ = "http://resource.autoidc.cn/pages/details.aspx?RESOURCEKEY=HGZ&RESOURCEID=";

        public static string strTargerUrlListPZ = "http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=PZ&PAGEID=";
        public static string strTargerUrlDetailsPZ = "http://resource.autoidc.cn/pages/details.aspx?RESOURCEKEY=PZ&RESOURCEID=";

        public static string strTargerUrlListWS = "http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=WS&PAGEID=";
        public static string strTargerUrlDetailsWS = "http://resource.autoidc.cn/pages/details.aspx?RESOURCEKEY=WS&RESOURCEID=92626763";


        public static Dictionary<string, string> SetPostParams()
        {
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            postParams.Add("__EVENTTARGET", "login");
            postParams.Add("__EVENTARGUMENT", "");
            postParams.Add("__VIEWSTATE", "/wEPDwULLTIxMzY1ODgzNDNkZA==");
            postParams.Add("__EVENTVALIDATION", "/wEdAAT0DMIHeQWCXCoQAfP2Wj0fKhoCyVdJtLIis5AgYZ/RYe4sciJO3Hoc68xTFtZGQEgrU8x5SglfzmEU2KqYFKCX");
            postParams.Add("username", "user");
            postParams.Add("password", "uer");
            return postParams;
        }

        /// <summary>
        /// 登录后-资源编录列表页面-获取最大页数
        /// </summary>
        /// <param name="strLoginUrl"></param>
        /// <param name="strTargerUrl1"></param>
        public static string GetHtmlSourceListMaxPageNum(string Url)
        {
            string tablelist = ReadHTML(Url);
            if (!string.IsNullOrEmpty(tablelist))
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(tablelist);

                HtmlNode node = doc.DocumentNode.SelectSingleNode(@"/html[1]/head[1]/script[4]");
                //获取最大页数  CreateNav(90,1,
                string result = node.InnerText.ToString();
                int iStart = result.IndexOf("CreateNav(");
                if (iStart > 0)
                {
                    string result4 = result.Substring(iStart + 10);
                    int iEnd = result4.IndexOf(",");
                    string result5 = result4.Substring(0, iEnd);
                    return result5;
                }
                else
                    return "";
            }
            else
            {
                GetHtmlSourceListMaxPageNum(Url);
            }
            return "0";
        }

        public static CookieContainer Login()
        {
            
            ///////////////////////////////////////////////////  
            // 1.打开 MyLogin.aspx 页面，获得 GetVeiwState & EventValidation  
            ///////////////////////////////////////////////////                  
            // 设置打开页面的参数  
            byte[] postData = Encoding.ASCII.GetBytes("__EVENTARGUMENT=&__EVENTTARGET=login&__EVENTVALIDATION=%2FwEdAAT0DMIHeQWCXCoQAfP2Wj0fKhoCyVdJtLIis5AgYZ%2FRYe4sciJO3Hoc68xTFtZGQEgrU8x5SglfzmEU2KqYFKCX&__VIEWSTATE=%2FwEPDwULLTIxMzY1ODgzNDNkZA%3D%3D&password=uer&username=user");

            HttpWebRequest request = WebRequest.Create("http://resource.autoidc.cn/pages/login.aspx") as HttpWebRequest;
            request.CookieContainer = cookieContainer;
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.AllowAutoRedirect = true;

            // 提交请求数据  
            System.IO.Stream outputStream = request.GetRequestStream();
            outputStream.Write(postData, 0, postData.Length);
            outputStream.Close();


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            return cookieContainer;
        }

        /// <summary>
        /// 根据URL获取页面信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ReadHTML(string url)
        {
            int i = 0;//循环次数
            string strContent = string.Empty;
            while (i < 2)
            {
                try
                {
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    request.CookieContainer = cookieContainer;
                    request.Method = "GET";
                    request.KeepAlive = true;
                    request.AllowAutoRedirect = true;

                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    System.IO.Stream responseStream = response.GetResponseStream();
                    System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
                    strContent = reader.ReadToEnd();
                    if (string.IsNullOrEmpty(strContent))
                    {
                        i++;//为空的时候再次循环
                    }
                    else//有返回值
                    {
                        return strContent;
                    }
                }
                catch (Exception ex)
                {
                    logMgr.WriteToFile(url + "出错：" + ex.Message);
                    //return "";
                    i++;//出错的时候再次循环
                    throw ex;
                }
            }
            return strContent;
        }

    }
}
