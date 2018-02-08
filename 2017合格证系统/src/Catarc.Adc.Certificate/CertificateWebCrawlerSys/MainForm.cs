using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using CertificateWebCrawlerSys.Utils;
using CertificateWebCrawlerSys.Helper;

namespace CertificateWebCrawlerSys
{
    public partial class MainForm : Form
    {
        Dictionary<string, string> postParams = new Dictionary<string, string>();

        CookieContainer cookieContainer = new CookieContainer();

        public MainForm()
        {
            InitializeComponent();
            SetPostParams();
        }

        public void SetPostParams()
        {
            postParams.Add("__EVENTTARGET", "login");
            postParams.Add("__EVENTARGUMENT", "");
            postParams.Add("__VIEWSTATE", "/wEPDwULLTIxMzY1ODgzNDNkZA==");
            postParams.Add("__EVENTVALIDATION", "/wEdAAT0DMIHeQWCXCoQAfP2Wj0fKhoCyVdJtLIis5AgYZ/RYe4sciJO3Hoc68xTFtZGQEgrU8x5SglfzmEU2KqYFKCX");
            postParams.Add("username", "user");
            postParams.Add("password", "uer");

        }

        private void buttonLogin(object sender, EventArgs e)
        {
            // 登录后-资源编录列表页面
            Login();

            // 资源编录列表-机动车合格证申请界面
            //Thread thHGZ = new Thread(GetHtmlSourceListHGZ);
            //thHGZ.Start();
            GetHtmlSourceListHGZ();

            // 资源编录列表-机动车合格证申请界面
            //Thread thPZ = new Thread(GetHtmlSourceListPZ);
            //thPZ.Start();
            //GetHtmlSourceListPZ();

            // 资源编录列表-机动车合格证申请界面
            //Thread thWS = new Thread(GetHtmlSourceListWS);
            //thWS.Start();
            //GetHtmlSourceListWS();
        }

        /// <summary>
        /// 资源编录列表-机动车合格证申请界面
        /// </summary>
        public void GetHtmlSourceListHGZ()
        {
            ConvertHGZ convertHGZ = new ConvertHGZ();
            int pageNum = 1;
            while(true)
            {
                string strTargerUrl = string.Format("http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=HGZ&PAGEID={0}", pageNum);
                string strContex = ReadHTML(strTargerUrl);
                var num = convertHGZ.getPageNumHGZ(strContex);
                var data = convertHGZ.getListHGZ(strContex,pageNum.ToString());
                LogManager.Log("MainLog", "HGZ", pageNum.ToString());
                //InsertHGZ insertHGZ = new InsertHGZ();
                //insertHGZ.InsertListHGZ(data);
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    // 资源明细-机动车合格证申请界面
                    string strTargerUrlDetailsHGZ = string.Format("http://resource.autoidc.cn/pages/details.aspx?RESOURCEKEY=HGZ&RESOURCEID={0}", "HIDC00000000000420491536");
                    GetHtmlSourceDetailsHGZ(data.Rows[i]["APP_TIME"].ToString(), data.Rows[i]["APP_TYPE"].ToString(), strTargerUrlDetailsHGZ);
                    LogManager.Log("MainLog", "HGZ", data.Rows[i]["SQBH"].ToString());
                }
                pageNum++;
                if (num == pageNum) break;
            }
        }

        /// <summary>
        /// 机动车合格证申请界面-详细信息
        /// </summary>
        public void GetHtmlSourceDetailsHGZ(string appTime, string appType, string strTargerUrl)
        {
            string strContex = ReadHTML(strTargerUrl);
            ConvertHGZ convertHGZ = new ConvertHGZ();
            //var data = convertHGZ.getDetailsHGZ(strContex);
            var data1 = convertHGZ.getDetailsHGZ(appTime, appType, strContex);
            InsertHGZ insertHGZ = new InsertHGZ();
            //insertHGZ.InsertDetailsHGZ(data);
            insertHGZ.InsertDBHGZ(data1);
            //insertHGZ.InsertSingleDBHGZ(data1);
            //insertHGZ.InsertTvpDBHGZ(data1);
        }

        /// <summary>
        /// 资源编录列表-配置信息
        /// </summary>
        public void GetHtmlSourceListPZ()
        {
            ConvertPZ convertPZ = new ConvertPZ();
            int pageNum = 1;
            while (true)
            {
                string strTargerUrl = string.Format("http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=PZ&PAGEID={0}", pageNum);
                string strContex = ReadHTML(strTargerUrl);
                var num = convertPZ.getPageNumHGZ(strContex);
                var data = convertPZ.getListPZ(strContex);
                LogManager.Log("MainLog", "PZ", pageNum.ToString());
                //InsertPZ insertPZ = new InsertPZ();
                //insertPZ.InsertListPZ(data);
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    // 资源明细-机动车合格证申请界面
                    string strTargerUrlDetailsPZ = string.Format("http://resource.autoidc.cn/pages/details.aspx?RESOURCEKEY=PZ&RESOURCEID={0}", data.Rows[i]["PZXLH"]);
                    GetHtmlSourceDetailsPZ(strTargerUrlDetailsPZ);
                    LogManager.Log("MainLog", "PZ", data.Rows[i]["PZXLH"].ToString());
                }
                pageNum++;
                if (num == pageNum) break;
            }
        }

        /// <summary>
        /// 配置信息-详细信息
        /// </summary>
        public void GetHtmlSourceDetailsPZ(string strTargerUrl)
        {
            string strContex = ReadHTML(strTargerUrl);
            ConvertPZ convertPZ = new ConvertPZ();
            var data = convertPZ.getDetailsPZ(strContex);
            InsertPZ insertPZ = new InsertPZ();
            //insertPZ.InsertDetailsPZ(data);
            //insertPZ.InsertDBPZ(data);
            //insertPZ.InsertSingleDBPZ(data);
            insertPZ.InsertTvpDBPZ(data);
        }

        /// <summary>
        /// 资源编录列表-完税信息
        /// </summary>
        public void GetHtmlSourceListWS()
        {
            ConvertWS convertWS = new ConvertWS();
            int pageNum = 1;
            while (true)
            {
                string strTargerUrl = string.Format("http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=WS&PAGEID={0}", pageNum);
                string strContex = ReadHTML(strTargerUrl);
                var num = convertWS.getPageNumHGZ(strContex);
                var data = convertWS.getListWS(strContex);
                InsertWS insertWS = new InsertWS();
                //insertWS.InsertDBWS(data);
                //insertWS.InsertSingleDBWS(data);
                insertWS.InsertTvpDBWS(data);
                LogManager.Log("MainLog", "WS", pageNum.ToString());
                //for (int i = 0; i < data.Rows.Count; i++)
                //{
                //    // 资源明细-机动车合格证申请界面
                //    string strTargerUrlDetailsWS = string.Format("http://resource.autoidc.cn/pages/details.aspx?RESOURCEKEY=WS&RESOURCEID={0}", data.Rows[i]["RESOURCE_ID"]);
                //    GetHtmlSourceDetailsWS(data.Rows[i]["RESOURCE_ID"].ToString(), strTargerUrlDetailsWS);
                //    LogManager.Log("MainLog", "WS", data.Rows[i]["RESOURCE_ID"].ToString());
                //}
                pageNum++;
                if (num == pageNum) break;
            }
        }

        /// <summary>
        /// 完税信息-详细信息
        /// </summary>
        public void GetHtmlSourceDetailsWS(string resourceId, string strTargerUrl)
        {
            string strContex = ReadHTML(strTargerUrl);
            ConvertWS convertWS = new ConvertWS();
            var data = convertWS.getDetailsWS(resourceId, strContex);
            InsertWS insertWS = new InsertWS();
            //insertWS.InsertDetailsWS(data);
            insertWS.InsertDBWS(data);
            //insertWS.InsertSingleDBWS(data);
        }

        private void Login()
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

        }

        private string ReadHTML(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = cookieContainer;
            request.Method = "GET";
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

    }
}
