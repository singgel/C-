using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CertificateWebCrawlerSys.Utils;
using CertificateWebCrawlerSys.Helper;
using System.Data;
using System.Net;
using DevExpress.XtraGrid;
using System.Windows.Forms;
using System.Threading;

namespace CertificateWebCrawlerSys
{
    public class PZUtils
    {
        LogManager logMgr = new LogManager();
        Dictionary<string, string> postParams = Tool.SetPostParams();
        private bool IsStopped = false;
        string HtmlName = "";
        GridControl gc;
        ListBox CrawlerLog;
        string MaxPageNum = "1";
        Label lbl;
        string param = string.Empty;
        string PageFrom = "1";
        string PageTo = string.Empty;
        int page = 1;//正在抓取的页数

        public PZUtils()
        {
        }

        public PZUtils(string name, GridControl gc, ListBox lb)
        {
            this.gc = gc;
            this.HtmlName = name;
            this.CrawlerLog = lb;
        }
        public PZUtils(string name, GridControl gc, Label lbl)
        {
            this.gc = gc;
            this.HtmlName = name;
            this.lbl = lbl;
            //this.CrawlerLog = null;
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
                string msg = DateTime.Now.ToString("G") + " 自动抓取配置信息失败：" + ex.Message;
                LogWrite(msg);
            }
        }
        //停止监控
        public void Stop()
        {
            IsStopped = false;
            LogWrite(DateTime.Now.ToString("G") + " 停止抓取配置信息");
        }

        private void AutoCrawlerData()
        {
            string msg = string.Empty;
            string MaxPageNumm = Tool.GetHtmlSourceListMaxPageNum(Tool.strTargerUrlListPZ + PageFrom);
            int iMinPageNumm = int.Parse(MaxPageNumm) == 0 ? 5 : int.Parse(MaxPageNumm);
            msg = string.Format("{0} 配置信息数据共{1}页", DateTime.Now.ToString("G"), MaxPageNumm);
            LogWrite(msg);
            if (string.IsNullOrEmpty(PageTo))
            {
                this.PageTo = MaxPageNumm;
            }
            int iMaxPageNum = int.Parse(PageTo);
            int iMinPageNum = int.Parse(PageFrom);


            if (iMaxPageNum > iMinPageNumm && iMinPageNumm!=0)
            {
                iMaxPageNum = iMinPageNumm;
            }
            MaxPageNum = iMaxPageNum.ToString();
            DataTable data = new DataTable();

            for (page = iMinPageNum; page <= iMaxPageNum; page++)
            {
                if (IsStopped)
                {
                    try
                    {
                        // 资源编录列表-配置信息界面
                        data = GetHtmlSourceListPZ(Tool.strTargerUrlListPZ + page.ToString());

                        if (data != null && data.Rows.Count > 0)
                        {
                            //界面显示
                            if (this.gc != null)
                            {
                                this.gc.Invoke(new Action(() =>
                                    {
                                        this.gc.DataSource = data;
                                    }));
                            }
                        }
                        else
                        {
                            msg = DateTime.Now.ToString("G") + " 停止抓取配置信息";
                            LogWrite(msg);
                            return;
                        }
                        LogWrite(string.Format("{0} 配置信息共{1}页，正在抓取第{2}页，该页面共{3}条数据", DateTime.Now.ToString("G"), MaxPageNum, page.ToString(), data.Rows.Count));
                        //插入数据库
                        if (data != null && data.Rows.Count > 0)
                        {
                            InsertDataPZ(Tool.strTargerUrlDetailsPZ, data);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        msg = string.Format("{0} 自动抓取配置信息失败:{1}", DateTime.Now.ToString("G"), ex.Message);

                        LogWrite(msg);
                    }
                }
                else
                {
                    break;
                }

            }
            IsStopped = false;
            LogWrite(DateTime.Now.ToString("G") + " 停止抓取配置信息");
        }





        /// <summary>
        /// 资源编录列表-配置信息
        /// </summary>
        /// <param name="strLoginUrl"></param>
        /// <param name="strTargerUrl2"></param>
        public DataTable GetHtmlSourceListPZ(string strTargerUrl)
        {
            DataTable data = new DataTable();
            string strContent = Tool.ReadHTML(strTargerUrl);
            if (!string.IsNullOrEmpty(strContent))
            {
                ConvertPZ convertPZ = new ConvertPZ();
                var dataList = convertPZ.getListPZ(strContent);

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
        public void InsertDataPZ(string strTargerUrl, DataTable data)
        {
            string strContex = string.Empty;
            string msg = string.Empty;
            int dataCount = data.Rows.Count;
            InsertPZ insertPZ = new InsertPZ();
            ConvertPZ convertPZ = new ConvertPZ();
            DataTable dtInsert = new DataTable();
            int i = 0;
            try
            {
                for (i = 0; i < dataCount; i++)
                {
                    if (IsStopped)
                    {
                        string id = data.Rows[i]["PZXLH"].ToString().Trim();

                        strContex = Tool.ReadHTML(strTargerUrl + id);
                        if (!string.IsNullOrEmpty(strContex))
                        {
                            var dataDetails = convertPZ.getDetailsPZ(strContex);
                            //insertPZ.InsertDBPZ(dataDetails);
                            if (i == 0)
                            {
                                dtInsert = dataDetails;
                            }
                            else
                            {
                                DataRow dr = dataDetails.Rows[0];
                                dtInsert.Rows.Add(dr.ItemArray);
                            }
                        }
                    }
                }
                if (IsStopped && dtInsert != null && dtInsert.Rows.Count>0)
                {
                    insertPZ.InsertDBPZ(dtInsert);
                    LogWrite(string.Format("{0} 插入配置信息数据第{1}页，共{2}条数据", DateTime.Now.ToString("G"), (page).ToString(), dtInsert.Rows.Count.ToString()));
                }
            }
            catch (Exception ex)
            {
                msg = string.Format("{0} 正在抓取第{1}页，插入配置信息数据时出错：i={3},{4}", DateTime.Now.ToString("G"), page.ToString(), i.ToString(), ex.Message);
                LogWrite(msg);
            }

            //msg = string.Format("{0} 配置信息数据 插入详细信息{1}条数据", DateTime.Now.ToString("G"), dataCount.ToString());
            //LogWrite(msg);

        }

        //写入界面和日志
        private void LogWrite(string Message)
        {
            if (CrawlerLog != null)
            {
                this.CrawlerLog.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(Message);
                    //this.CrawlerLog.TopIndex = this.CrawlerLog.Items.Count - 1;
                }));
            }
            if (lbl != null)
            {
                this.lbl.Invoke(new Action(() =>
                {
                    this.lbl.Text = Message;
                }));
            }
            logMgr.WriteToFile(Message);
        }

    }
}
