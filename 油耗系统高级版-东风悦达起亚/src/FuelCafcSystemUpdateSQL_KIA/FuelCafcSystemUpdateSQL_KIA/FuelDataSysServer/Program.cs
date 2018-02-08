using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FuelDataSysServer
{
    static class Program
    {
        /// <summary>
        /// 当前应用程序
        /// </summary>
        readonly static string applicationName = "油耗服务端系统";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isExist = false;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, "燃料消耗量数据管理系统", out isExist))
            {
            }
            if (!isExist) { Environment.Exit(1); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            LocalLoginForm locLoginForm = new LocalLoginForm();
            locLoginForm.ShowDialog();
            if (locLoginForm.DialogResult == DialogResult.OK)
            {
                Application.Run(new MainServerForm());
            }
            else
            {
                return;
            }
        }
    }
}
