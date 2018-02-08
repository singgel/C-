using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FuelDataSysClient.SubForm;
using FuelDataSysClient.Change;
using FuelDataSysClient.CertificateService;

namespace FuelDataSysClient
{
    static class Program
    {
        /// <summary>
        /// 当前应用程序
        /// </summary>
        static string applicationName = "油耗客户端系统";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isExist = false;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "燃料消耗量数据管理系统", out isExist);
            if (!isExist) { Environment.Exit(1); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //EvDXWinLocalization.DXActive.initialize();

            // 注册全局线程异常处理事件侦听
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            MainForm formMain = new MainForm();
            //AccessExeSqlBatchForm formMain = new AccessExeSqlBatchForm();
            //FuelDataImportForm formMain = new FuelDataImportForm();
            if (!formMain.IsDisposed)
            {
                Application.Run(formMain);
            }

        }

        /// <summary>
        /// 全局线程异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // 显示友好的自定义全局异常提示窗体
            FuelDataSysClient.Tool.GlobalExceptionManager.ShowGlobalExceptionInfo(e.Exception, applicationName, "CATARC");
        }
    }
}
