using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Runtime.InteropServices;
using System.Timers;
using CertificateWebCrawlerWindowsService.Utils;
using System.Net;

namespace CertificateWebCrawlerWindowsService
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string iniFileName = String.Format("{0}{1}config.ini", AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.DirectorySeparatorChar);

        CookieContainer cookieContainer = new CookieContainer();
        LogManager logMgr = new LogManager();
        //readonly CertificateProcess cp = new CertificateProcess();
        HGZUtils hgzUtils;
        PZUtils pzUtils;
        WSUtils wsUtils;
        HGZUtilsThreadPool hgzUtilsPool;
        string dtStart;

        int iRunHour = 1;//开始启动时间
        int iRunMinute = 0;//开始启动时间
        static string connectionString = "";

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //cookieContainer = Tool.Login();
            //HGZ_Start();
            //PZ_Start();
            //WS_Start();
            logMgr.WriteToFile(string.Format("{0} 服务启动 ", DateTime.Now.ToString("G")));
            
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(RunDateInit);
            // 设置引发时间的时间间隔 此处设置为1分钟检测一次
            aTimer.Interval = 1000 * 60 * 1;
            //设置是执行一次（false）还是一直执行(true)，默认为true 
            aTimer.AutoReset = true;
            //开始计时
            aTimer.Enabled = true;

            //System.Threading.Thread.Sleep(1000 * 60 * 1);
        }

        protected override void OnStop()
        {
            hgzUtilsPool.Stop();
            hgzUtils.Stop();
            pzUtils.Stop();
            wsUtils.Stop();
        }

        private void readconfig()
        {
            //读取开始时间
            try
            {
                iRunHour = Convert.ToInt32(ReadIni("StartTime", "RunHour", "1"));
            }
            catch (Exception ex)
            {
                LogManager.Log("Log", "Error", "RunHour" + ex.Message);
            }
            try
            {
                iRunMinute = Convert.ToInt32(ReadIni("StartTime", "RunMinute", "1"));
            }
            catch (Exception ex)
            {
                LogManager.Log("Log", "Error", "RunMinute" + ex.Message);
            }
            //获取数据库配置
            try
            {
                String ip = ReadIni("Databasa", "IP", "");
                String port = ReadIni("Databasa", "PORT", "");
                String Initial = ReadIni("Databasa", "Initial", "");
                String userName = ReadIni("Databasa", "UserName", "");
                String password = ReadIni("Databasa", "Password", "");
                connectionString = String.Format("Data Source={0},{1};Initial Catalog={2};Persist Security Info=True;User ID={3};Password={4}; Connect Timeout=3000", ip, port, Initial, userName, password);
            }
            catch (Exception ex)
            {
                LogManager.Log("Log", "Error", ex.Message);
            }
        }
        //读取ini
        public static string ReadIni(string Section, string Key, string Default)
        {
            StringBuilder temp = new StringBuilder(1024);
            int rec = GetPrivateProfileString(Section, Key, Default, temp, 1024, iniFileName);
            return temp.ToString();
        }

        private void RunDateInit(object source, ElapsedEventArgs e)
        {
            //读取配置文件
            readconfig();
            logMgr.WriteToFile(string.Format("{0} RunDateInit查看时间 启动时间：{1}:{2}", DateTime.Now.ToString("G"), iRunHour, iRunMinute));

            //在每天的定时执行一次
            if (DateTime.Now.Hour == iRunHour && DateTime.Now.Minute == iRunMinute)
            {
                logMgr.WriteToFile(string.Format("{0} 开始启动 ", DateTime.Now.ToString("G")));

                //设置参数
                SqlHelper.connectionString = connectionString;
                //开始服务
                cookieContainer = Tool.Login();
                HGZ_Start();
                PZ_Start();
                WS_Start();
            }
        }

        //开始自动抓取
        private void HGZ_Start()
        {
            try
            {
                logMgr.WriteToFile(string.Format("{0} 开始抓取机动车合格证申请  ", DateTime.Now.ToString("G")));
                string param = queryParam("HGZ");
                hgzUtilsPool = new HGZUtilsThreadPool();
                hgzUtilsPool.StartThreadPool(param, "1", "");
            }
            catch (Exception ex)
            {
                logMgr.WriteToFile(string.Format("{0} 抓取机动车合格证申请开始异常：{1}  ", DateTime.Now.ToString("G"), ex.Message));
            }

        }
        //开始自动抓取
        private void PZ_Start()
        {
            try
            {
                logMgr.WriteToFile(string.Format("{0} 开始抓取完税信息  ", DateTime.Now.ToString("G")));
                string param = queryParam("PZ");
                pzUtils = new PZUtils();
                pzUtils.Start(param, "", "");
            }
            catch (Exception ex)
            {
                logMgr.WriteToFile(string.Format("{0} 抓取配置信息开始异常：{1}  ", DateTime.Now.ToString("G"), ex.Message));
            }

        }
        //开始自动抓取
        private void WS_Start()
        {
            try
            {
                logMgr.WriteToFile(string.Format("{0} 开始抓取完税信息  ", DateTime.Now.ToString("G")));

                wsUtils = new WSUtils();
                wsUtils.Start(queryParam("WS"), "1", "");
            }
            catch (Exception ex)
            {
                logMgr.WriteToFile(string.Format("{0} 抓取完税信息开始异常：{1}  ", DateTime.Now.ToString("G"), ex.Message));

            }

        }


        //查询条件
        private string queryParam(string timeType)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(" 1=1 ");
            string timetype = string.Empty;
            if ("HGZ".Equals(timeType))
            {
                timetype = "APP_TIME";
            }
            else if ("PZ".Equals(timeType))
            {
                timetype = "PZ_UPDATETIME";
            }
            else if ("WS".Equals(timeType))
            {
                timetype = "CREATETIME";
            }
            else
            {
                return "";
            }
            string dtStart = DateTime.Now.Date.AddDays(-1).ToString();
            if (!string.IsNullOrEmpty(dtStart))
            {
                sqlStr.AppendFormat(" AND {0} >= '{1}' ", timetype, dtStart);
            }

            return sqlStr.ToString();
        }

    }
}
