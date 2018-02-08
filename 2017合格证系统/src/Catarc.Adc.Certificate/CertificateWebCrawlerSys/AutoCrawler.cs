using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraGrid;
using System.Threading;

namespace CertificateWebCrawlerSys
{
    public class AutoCrawler
    {
        private bool IsStopped = false;
        private int iInterval = 300;
        string HtmlName = "";
        GridControl gc;
        string MaxPageNum = "1";

        public AutoCrawler()
        {
        }

        public AutoCrawler(string name,GridControl gc)
        {
            this.gc = gc;
            this.HtmlName = name;
        }

        //开始监控
        public void Start(string MaxPageNum)
        {
            if (string.IsNullOrEmpty(MaxPageNum))
            {
                return;
            }
            this.MaxPageNum = MaxPageNum;
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
                string msg = DateTime.Now.ToString("G") + " 自动抓取失败：" + ex.Message;
                //LogWrite(msg);
            }
        }
        //停止监控
        public void Stop()
        {
            IsStopped = false;
        }


        private void AutoCrawlerData()
        {
            string msg = string.Empty;
            int iMaxPageNum = int.Parse(MaxPageNum);

            for (int i = 1; i < iMaxPageNum; i++)
            {
                if (IsStopped)
                {
                    try
                    {
                        //msg = string.Format("{0} 自动发送{1}开始", DateTime.Now.ToString("G"), tableName);
                        //LogWrite(msg);
                        //QueryTable(tableName);
                    }
                    catch (System.Exception ex)
                    {
                        //msg = string.Format("{0} 自动发送{1}失败:{2}", DateTime.Now.ToString("G"), tableName, ex.Message);
                        //LogWrite(msg);
                    }
                }
                else
                {
                    break;
                }
                //获取发送总数
                //QueryDataSynchroCount(tableName);

            }
        }




    }
}
