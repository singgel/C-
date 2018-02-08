using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CertificateWebCrawlerWindowsService.Utils;
using CertificateWebCrawlerWindowsService.Helper;
using System.Data;
using System.Net;
using System.Threading;

namespace CertificateWebCrawlerWindowsService
{
    public class WSUtils
    {
        Dictionary<string, string> postParams = Tool.SetPostParams();
        LogManager logMgr = new LogManager();
        private bool IsStopped = false;
        string HtmlName = "";
        string MaxPageNum = "1";
        string param = string.Empty;
        string PageFrom = "1";
        string PageTo = string.Empty;

        public WSUtils()
        {
        }

        //开始监控
        public void Start(string param, string paramPageFrom, string paramPageTo)
        {
            if (!string.IsNullOrEmpty(param))
            {
                this.param = param;
            }
            if (!string.IsNullOrEmpty(paramPageFrom))
            {
                this.PageFrom = paramPageFrom;
            }
            if (!string.IsNullOrEmpty(paramPageTo))
            {
                this.PageTo = paramPageTo;
            }
            IsStopped = true;
            try
            {
                Thread thAutoCrawler;

                thAutoCrawler = new Thread(new ThreadStart(AutoCrawlerData));  //获取线程
                thAutoCrawler.IsBackground = true;
                thAutoCrawler.Start();
            }
            catch (System.Exception ex)
            {
                string msg = DateTime.Now.ToString("G") + " 自动抓取完税信息失败：" + ex.Message;
                LogWrite(msg);
            }
        }
        //停止监控
        public void Stop()
        {
            IsStopped = false;
            LogWrite(DateTime.Now.ToString("G") + "  停止抓取完税信息");
        }


        private void AutoCrawlerData()
        {
            string msg = string.Empty;
            string MaxPageNumm = Tool.GetHtmlSourceListMaxPageNum(Tool.strTargerUrlListWS + PageFrom);
            int iMaxPageNumm = int.Parse(MaxPageNumm);
            msg = string.Format("{0} 完税信息数据共{1}页", DateTime.Now.ToString("G"), MaxPageNumm);
            LogWrite(msg);
            if (string.IsNullOrEmpty(PageTo))
            {
                this.PageTo = MaxPageNumm;
            }
            int iMaxPageNum = int.Parse(PageTo);
            int iMinPageNum = int.Parse(PageFrom);

            if (iMaxPageNum > iMaxPageNumm)
            {
                iMaxPageNum = iMaxPageNumm;
            }
            DataTable data = new DataTable();
            MaxPageNum = iMaxPageNum.ToString();

            for (int i = iMinPageNum; i <= iMaxPageNum; i++)
            {
                if (IsStopped)
                {
                    try
                    {

                        // 资源编录列表-完税信息界面
                        data = GetHtmlSourceListWS(Tool.strTargerUrlListWS + i.ToString());

                        if (data != null && data.Rows.Count > 0)
                        {
                        }
                        else
                        {
                            msg = DateTime.Now.ToString("G") + "  停止抓取完税信息";
                            LogWrite(msg);
                            return;
                        }
                        LogWrite(string.Format("{0}  完税信息共{1}页，正在抓取第{2}页，该页面共{3}条数据", DateTime.Now.ToString("G"), MaxPageNum, i.ToString(), data.Rows.Count));

                        //插入数据库
                        if (data != null && data.Rows.Count > 0)
                        {
                            InsertDataWS(Tool.strLoginUrl, Tool.strTargerUrlDetailsWS, data);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        msg = string.Format("{0} 自动抓取完税信息失败:{1}", DateTime.Now.ToString("G"), ex.Message);

                        LogWrite(msg);
                    }
                }
                else
                {
                    break;
                }

            }
            LogWrite(DateTime.Now.ToString("G") + "  停止抓取完税信息");
        }

        /// <summary>
        /// 资源编录列表-完税信息
        /// </summary>
        /// <param name="strLoginUrl"></param>
        /// <param name="strTargerUrl2"></param>
        public DataTable GetHtmlSourceListWS(string strTargerUrl)
        {
            DataTable data = new DataTable();
            string strContex =Tool.ReadHTML(strTargerUrl);
            if (!string.IsNullOrEmpty(strContex))
            {
                ConvertWS convertWS = new ConvertWS();
                var dataList = convertWS.getListWS(strContex);

                //按时间返回信息
                if (!string.IsNullOrEmpty(param))
                {
                    data = dataList.Clone();
                    DataRow[] drs = dataList.Select(param);
                    if (drs != null && drs.Count() > 0)
                    {
                        data = drs.CopyToDataTable();
                    }
                }
                else
                {
                    data = dataList;
                }
            }
            return data;
        }

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="strLoginUrl"></param>
        /// <param name="strTargerUrl"></param>
        /// <param name="data">列表数据</param>
        public void InsertDataWS(string strLoginUrl, string strTargerUrl, DataTable data)
        {
            string msg = string.Empty;
            try
            {
                string strContex = string.Empty;
                int dataCount = data.Rows.Count;
                InsertWS insertWS = new InsertWS();
                insertWS.InsertDBWS(data);
                msg = string.Format("{0} 完税信息数据 插入列表{1}条数据", DateTime.Now.ToString("G"), dataCount.ToString());
                LogWrite(msg);
            }
            catch (ArgumentException ex)
            {
                msg = string.Format("{0} 插入合格证详细信息,出错：{1}", DateTime.Now.ToString("G"),  ex.Message);
                LogWrite(msg);
            }

        }

        //写入界面和日志
        private void LogWrite(string Message)
        {
            logMgr.WriteToFile(Message);
        }

    }
}
