using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using System.Data.SqlClient;
using CertificateWebCrawlerSys.Utils;
using CertificateWebCrawlerSys.Helper;
using DevExpress.XtraGrid;
using System.Threading;
using CertificateWebCrawlerSys.Properties;


namespace CertificateWebCrawlerSys
{
    public class HGZUtilsThreadPool
    {
        private bool IsStopped = false;
        string HtmlName = "";
        GridControl gc;
        Label lbl;
        ListBox CrawlerLog;
        string param = string.Empty;
        LogManager logMgr = new LogManager();
        int PageFrom = 1;//线程开始页
        int PageTo = 1;//线程结束页 
        InsertHGZ insertHGZ = new InsertHGZ();
        ConvertHGZ convertHGZ = new ConvertHGZ();
        DataTable dtInsert = new DataTable();
        int iThread_ready = 0;
        InsertQueue insertQueue = new InsertQueue();

        public HGZUtilsThreadPool()
        {
        }

        public HGZUtilsThreadPool(string name, GridControl gc, ListBox lb)
        {
            this.gc = gc;
            this.HtmlName = name;
            this.CrawlerLog = lb;
            //this.lbl = null;
        }

        public HGZUtilsThreadPool(string name, GridControl gc, Label lbl)
        {
            this.gc = gc;
            this.HtmlName = name;
            this.lbl = lbl;
        }

        /// <summary>
        /// 开始监控:单个线程
        /// </summary>
        /// <param name="param">条件限制，日期限制</param>
        /// <param name="paramPageFrom">页数从</param>
        /// <param name="paramPageTo">页数至</param>
        public void StartThreadPool(string param, string paramPageFrom, string paramPageTo)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(param))
            {
                this.param = param;
            }
            if (!string.IsNullOrEmpty(paramPageFrom))
            {
                this.PageFrom = int.Parse(paramPageFrom);
            }

            //获取最大页数
            string MaxPageNumm = Tool.GetHtmlSourceListMaxPageNum(Tool.strTargerUrlListHGZ + "1");
            //如果出错，默认为1000
            int iMaxPageNumm = int.Parse(MaxPageNumm) == 0 ? 1000 : int.Parse(MaxPageNumm);
            msg = string.Format("机动车合格证申请数据共{0}页", MaxPageNumm);
            LogWrite(msg);
            if (!string.IsNullOrEmpty(paramPageTo))
            {
                this.PageTo = int.Parse(paramPageTo);
                if (PageTo > iMaxPageNumm && iMaxPageNumm!=0)
                {
                    PageTo = iMaxPageNumm;
                }
            }
            else
            {
                this.PageTo = iMaxPageNumm;
            }

            IsStopped = true;
            try
            {
                string dTime = string.Empty;
                if (IsStopped)
                {
                    Thread thAutoCrawler;
                    thAutoCrawler = new Thread(new ThreadStart(AutoCrawlerData));  //获取线程
                    thAutoCrawler.IsBackground = true;
                    thAutoCrawler.Start();
                }
            }
            catch (System.Exception ex)
            {
                msg = DateTime.Now.ToString("G") + " 合格证申请数据自动抓取失败：" + ex.Message;
                LogWrite(msg);
            }
        }
        //开始监控：根据页数LIST抓取页面，并且单条数据插入
        public void Start(string ids)
        {
            string msg = string.Empty;
            IsStopped = true;
            //获取最大页数
            string MaxPageNumm = Tool.GetHtmlSourceListMaxPageNum(Tool.strTargerUrlListHGZ + "1");
            //如果出错，默认为1000
            int iMaxPageNumm = int.Parse(MaxPageNumm) == 0 ? 1000 : int.Parse(MaxPageNumm);
            msg = string.Format("机动车合格证申请数据共{0}页", MaxPageNumm);
            LogWrite(msg);

            IsStopped = true;
            try
            {
                ParameterizedThreadStart start = new ParameterizedThreadStart(AutoCrawlerDataOne);
                string dTime = string.Empty;

                string[] dTimes = ids.Split(',');
                int iMinPageNum = int.Parse(dTimes[0]);
                int iMaxPageNum = int.Parse(dTimes[dTimes.Length - 1]);
                int iPage;
                for (int i = 0; i < dTimes.Length; i++)
                {
                    iPage = int.Parse(dTimes[i]);
                    if (iPage > iMaxPageNumm)
                    {
                        continue;
                    }
                    if (IsStopped)
                    {
                        Thread thread = new Thread(start);
                        thread.Start(iPage.ToString());
                    }
                }

            }
            catch (System.Exception ex)
            {
                msg = DateTime.Now.ToString("G") + " 合格证申请数据自动抓取失败：" + ex.Message;
                LogWrite(msg);
            }
        }
        //停止监控
        public void Stop()
        {
            IsStopped = false;
            LogWrite(DateTime.Now.ToString("G") + "  停止抓取机动车合格证申请数据");
        }

        /// <summary>
        /// 获取资源编录列表-机动车合格证申请界面
        /// </summary>
        /// <param name="strLoginUrl"></param>
        /// <param name="strTargerUrl2"></param>
        public DataTable GetHtmlSourceListHGZ(string strTargerUrl, String page)
        {
            DataTable data = new DataTable();
            string strContent = Tool.ReadHTML(strTargerUrl);
            if (!string.IsNullOrEmpty(strContent))
            {
                ConvertHGZ convertHGZ = new ConvertHGZ();
                var dataList = convertHGZ.getListHGZ(strContent, page);
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
        public void InsertDataHGZPool(string strTargerUrl, DataTable data, int page)
        {
            LogWrite(string.Format("{0} 合格证详细信息正在抓取第{1}页，共{2}条数据", DateTime.Now.ToString("G"), (page).ToString(), data.Rows.Count.ToString()));
            DetailInfo detailInfo = new DetailInfo();
            string strContex = string.Empty;
            string msg = string.Empty;
            int dataCount = data.Rows.Count;

            insertHGZ.InsertListHGZ(data);
            dtInsert = convertHGZ.getDetailsHGZTable();
            int i = 1;

            try
            {
                if (IsStopped)
                {
                    for (i = 0; i < dataCount; i++)
                    {
                        detailInfo = new DetailInfo();
                        detailInfo.SQBH = data.Rows[i]["SQBH"].ToString().Trim();
                        detailInfo.APP_TIME = data.Rows[i]["APP_TIME"].ToString().Trim();
                        detailInfo.APP_TYPE = data.Rows[i]["APP_TYPE"].ToString().Trim();
                        detailInfo.TargerUrl = strTargerUrl;
                        insertQueue.Push(detailInfo);
                    }

                    iThread_ready = 0;//线程数重置为0
                    Thread thAutoCrawlerDetail1;
                    thAutoCrawlerDetail1 = new Thread(new ThreadStart(InsertDataHGZDetails));  //获取线程1
                    thAutoCrawlerDetail1.IsBackground = true;
                    thAutoCrawlerDetail1.Start();

                    Thread thAutoCrawlerDetail2;
                    thAutoCrawlerDetail2 = new Thread(new ThreadStart(InsertDataHGZDetails));  //获取线程2
                    thAutoCrawlerDetail2.IsBackground = true;
                    thAutoCrawlerDetail2.Start();

                    Thread thAutoCrawlerDetail3;
                    thAutoCrawlerDetail3 = new Thread(new ThreadStart(InsertDataHGZDetails));  //获取线程3
                    thAutoCrawlerDetail3.IsBackground = true;
                    thAutoCrawlerDetail3.Start();

                    while (iThread_ready < 3)
                    {
                        Thread.Sleep(1000 * 2);//等待2秒
                    }
                }
                if (IsStopped)
                {
                    insertHGZ.InsertDBHGZ(dtInsert);
                    LogWrite(string.Format("{0} 合格证详细信息插入第{1}页，共{2}条数据", DateTime.Now.ToString("G"), (page).ToString(), dtInsert.Rows.Count.ToString()));
                }
            }
            catch (Exception ex)
            {
                msg = string.Format("{0} 合格证详细信息正在抓取第{1}页，插入合格证详细信息数据时出错：i={2},{3}", DateTime.Now.ToString("G"), page.ToString(), i.ToString(), ex.Message);
                LogWrite(msg);
            }
        }

        //抓取详细信息，并且赋值给datatable
        public void InsertDataHGZDetails()
        {
            try
            {
                while (insertQueue.Size() > 0 && IsStopped)//如果队列不为空且没有停止
                {

                    DetailInfo detailInfo = insertQueue.Pop();
                    if (detailInfo != null)
                    {
                        string strContent = Tool.ReadHTML(detailInfo.TargerUrl + detailInfo.SQBH);
                        if (!string.IsNullOrEmpty(strContent))
                        {
                            var dataDetails = convertHGZ.getDetailsHGZ(detailInfo.APP_TIME, detailInfo.APP_TYPE, strContent);
                            lock (dtInsert)
                            {
                                DataRow dr = dataDetails.Rows[0];
                                dtInsert.Rows.Add(dr.ItemArray);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //logMgr.WriteToFile(ex.Message);
                throw ex;
            }
            iThread_ready++;
        }
        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="strLoginUrl"></param>
        /// <param name="strTargerUrl"></param>
        /// <param name="data">列表数据</param>
        public void InsertDataHGZ(string strTargerUrl, DataTable data, int page)
        {
            LogWrite(string.Format("{0} 合格证详细信息正在抓取第{1}页，共{2}条数据", DateTime.Now.ToString("G"), (page).ToString(), data.Rows.Count.ToString()));

            string strContex = string.Empty;
            string msg = string.Empty;
            int dataCount = data.Rows.Count;
            insertHGZ.InsertListHGZ(data);
            DataTable dtInsertData = convertHGZ.getDetailsHGZTable();
            int i = 0;
            try
            {
                ThreadPool.SetMaxThreads(3, 3);

                for (i = 0; i < dataCount; i++)
                {
                    if (IsStopped)
                    {
                        string id = data.Rows[i]["SQBH"].ToString().Trim();
                        string app_time = data.Rows[i]["APP_TIME"].ToString().Trim();
                        string app_type = data.Rows[i]["APP_TYPE"].ToString().Trim();

                        string strContent = Tool.ReadHTML(strTargerUrl + id);
                        if (!string.IsNullOrEmpty(strContent))
                        {
                            var dataDetails = convertHGZ.getDetailsHGZ(app_time, app_type, strContent);
                            DataRow dr = dataDetails.Rows[0];
                            dtInsertData.Rows.Add(dr.ItemArray);

                        }
                    }
                }
                if (IsStopped && dtInsertData != null && dtInsertData.Rows.Count>0)
                {
                    insertHGZ.InsertDBHGZ(dtInsertData);
                    LogWrite(string.Format("{0} 合格证详细信息插入第{1}页，共{2}条数据", DateTime.Now.ToString("G"), (page).ToString(), dtInsert.Rows.Count.ToString()));
                }

            }
            catch (Exception ex)
            {
                msg = string.Format("{0} 合格证正在抓取第{1}页，插入数据时失败：i={2},{3}", DateTime.Now.ToString("G"), page.ToString(), i.ToString(), ex.Message);
                LogWrite(msg);
            }
            //msg = string.Format("{0} 正在抓取第{1}页，插入详细信息{2}条数据", DateTime.Now.ToString("G"), page.ToString(), dataCount.ToString());
            //LogWrite(msg);
        }

        /// <summary>
        /// 抓取数据：
        /// </summary>
        private void AutoCrawlerData()
        {
            string msg = string.Empty;
            DataTable data = new DataTable();
            int page = 1;
            for (page = PageFrom; page <= PageTo; page++)
            {
                try
                {
                    if (IsStopped)
                    {
                        // 资源编录列表-机动车合格证申请界面
                        data = GetHtmlSourceListHGZ(Tool.strTargerUrlListHGZ + page.ToString(), page.ToString());
                        if (data != null && data.Rows.Count > 0)
                        {
                            //界面显示
                            if (this.gc != null)
                            this.gc.Invoke(new Action(() =>
                            {
                                this.gc.DataSource = data;
                            }));
                        }
                        else
                        {
                            msg = DateTime.Now.ToString("G") + "  停止抓取机动车合格证申请数据";
                            LogWrite(msg);
                            return;
                        }
                        msg = string.Format("{0} 合格证申请，正在抓取第{1}页，该页面共{2}条数据", DateTime.Now.ToString("G"), page.ToString(), data.Rows.Count);
                        LogWrite(msg);
                        //插入数据库
                        if (data != null && data.Rows.Count > 0)
                        {
                            InsertDataHGZPool(Tool.strTargerUrlDetailsHGZ, data, page);
                        }
                    }
                    else
                    {
                        msg = DateTime.Now.ToString("G") + " 停止抓取机动车合格证申请数据";
                        LogWrite(msg);
                        return;
                    }
                }
                catch (System.Exception ex)
                {
                    msg = string.Format("{0} 合格证申请数据自动抓取失败:{1}", DateTime.Now.ToString("G"), ex.Message);
                    LogWrite(msg);
                    WriteErrorPage(page.ToString());
                }
            }
            msg = DateTime.Now.ToString("G") + " 停止抓取机动车合格证申请数据";
            LogWrite(msg);
            return;
        }

        /// <summary>
        /// 按照指定的页数的list抓取数据：
        /// </summary>
        /// <param name="dTime">页数的list</param>
        private void AutoCrawlerDataOne(object oPage)
        {
            string msg = string.Empty;
            DataTable data = new DataTable();
            int page = int.Parse(oPage.ToString());
            try
            {
                if (IsStopped)
                {
                    // 资源编录列表-机动车合格证申请界面
                    data = GetHtmlSourceListHGZ(Tool.strTargerUrlListHGZ + page.ToString(), page.ToString());

                    if (data != null && data.Rows.Count > 0 && this.gc != null)
                    {
                        //界面显示
                        this.gc.Invoke(new Action(() =>
                        {
                            this.gc.DataSource = data;
                        }));
                    }
                    msg = string.Format("{0} 合格证申请正在抓取第{1}页，该页面共{2}条数据", DateTime.Now.ToString("G"), page.ToString(), data.Rows.Count);
                    LogWrite(msg);
                    //插入数据库
                    if (data != null && data.Rows.Count > 0)
                    {
                        InsertDataHGZ(Tool.strTargerUrlDetailsHGZ, data, page);
                    }
                }
                else
                {
                    msg = DateTime.Now.ToString("G") + "  停止抓取机动车合格证申请数据";
                    LogWrite(msg);
                    return;
                }
            }
            catch (System.Exception ex)
            {
                msg = string.Format("{0} 合格证申请数据失败:{1}", DateTime.Now.ToString("G"), ex.Message);
                LogWrite(msg);
                WriteErrorPage(page.ToString());
            }
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

        private void WriteErrorPage(string page)
        {
            logMgr.WriteErrorPageToFile(page);
        }
    }
}
