using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.LookAndFeel;

namespace Catarc.Adc.NewEnergyApproveSys
{
    static class Program
    {
        readonly static string applicationName = "新能源汽车补贴清算审核系统";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isExist = false;
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(true, "新能源汽车补贴清算审核系统", out isExist))
            {
            }
            if (!isExist) { Environment.Exit(1); }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            UserLookAndFeel.Default.SetSkinStyle(Catarc.Adc.NewEnergyApproveSys.Properties.Settings.Default.SkinStyle);
            Application.ThreadException += Application_ThreadException;

            using (LoginForm locLoginForm = new LoginForm())
            {
                locLoginForm.ShowDialog();
                if (locLoginForm.DialogResult == DialogResult.OK)
                {
                    Application.Run(new MainForm());
                }
                else
                {
                    return;
                }
            }
            //Application.Run(new MainForm());
        }

        /// <summary>
        /// 全局线程异常处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // 显示友好的自定义全局异常提示窗体
            Catarc.Adc.NewEnergyApproveSys.LogUtils.GlobalExceptionManager.ShowGlobalExceptionInfo(e.Exception, applicationName, "CATARC");
        }
    }
}