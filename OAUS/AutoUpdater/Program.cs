using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using AutoUpdater.Properties;

namespace AutoUpdater
{
    static class Program
    {
        /// <summary>
        /// 说明：
        /// OAUS使用的是免费版的通信框架ESFramework，最多支持10个人同时在线更新。如果要突破10人限制，请联系 www.oraycn.com
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["CurrentLanguage"]);
                
                string serverIP = ConfigurationManager.AppSettings["ServerIP"];
                int serverPort = int.Parse(ConfigurationManager.AppSettings["ServerPort"]);               
                string callBackExeName= ConfigurationManager.AppSettings["CallbackExeName"];
                string title = ConfigurationManager.AppSettings["Title"];     
                string processName = callBackExeName.Substring(0, callBackExeName.Length - 4);
                bool haveRun = ESBasic.Helpers.ApplicationHelper.IsAppInstanceExist(processName);
                if (haveRun)
                {
                    MessageBox.Show(Resources.TargetIsRunning);                   
                    return;
                }

                MainForm form = new MainForm(serverIP, serverPort, callBackExeName, title);                
                Application.Run(form);         
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

     
    }
}
