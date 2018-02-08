using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace FuelDataSysClient.Tool
{
    class HttpUrlData
    {
        /// <summary>
        /// 获取url返回值
        /// </summary>
        /// <param name="url"></param>
        /// <returns>JSON数据</returns>
        public string GetInfo(string url)
        {
            string strBuff = "";
            Uri httpURL = new Uri(url);
            try
            {
                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(httpURL);
                HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
                Stream respStream = httpResp.GetResponseStream();
                StreamReader respStreamReader = new StreamReader(respStream, Encoding.UTF8);
                strBuff = respStreamReader.ReadToEnd();
                return strBuff;
            }
            catch (Exception e)
            {
                return "";
            }
        }
    }
}
