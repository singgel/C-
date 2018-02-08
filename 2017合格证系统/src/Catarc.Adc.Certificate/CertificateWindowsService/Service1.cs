using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Runtime.InteropServices;
using Common;

namespace CertificateWindowsService
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static string iniFileName = String.Format("{0}{1}config.ini", AppDomain.CurrentDomain.BaseDirectory, System.IO.Path.DirectorySeparatorChar);

        readonly CertificateProcess cp = new CertificateProcess();
        int iInterval = 3600;
        int iDeleteInterval = 1;
        string connectionString ="";
        String filename = "C:";

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //读取配置文件
            readconfig();
            //设置参数
            cp.iInterval = iInterval;
            cp.connectionString = connectionString;
            cp.filename = filename;
            cp.iDeleteInterval = iDeleteInterval;
            //开始服务
            cp.Start();
        }

        protected override void OnStop()
        {
            cp.bStop = true;
        }
        private void readconfig()
        {
            //读取查询间隔
            try
            {
                iInterval = Convert.ToInt32(ReadIni("Interval", "Interval","3600"));        
            }
            catch(Exception ex)
            {
                LogManager.Log("Log", "Error", ex.Message);     
            }
            //读取删除间隔
            try
            {
                iDeleteInterval = Convert.ToInt32(ReadIni("Interval", "DeleteInterval", "1"));
            }
            catch (Exception ex)
            {
                LogManager.Log("Log", "Error", ex.Message);
            }
            //读取输出文件路径
            try
            {
                filename = ReadIni("OUTPUT", "filename", "");
            }
            catch (Exception ex)
            {
                LogManager.Log("Log", "Error", ex.Message);
            }
            //获取数据库配置
            try
            {
               String ip =  ReadIni("Databasa", "IP", "");
               String port = ReadIni("Databasa", "PORT", "1433");
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

    }
}
