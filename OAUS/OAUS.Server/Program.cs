using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESFramework;
using System.Xml;
using System.IO;
using ESFramework.Server.UserManagement;
using OAUS.Core;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Net;
using ESPlus;

namespace OAUS.Server
{
    /// <summary>
    /// 说明：
    /// OAUS使用的是免费版的通信框架ESFramework，最多支持10个人同时在线更新。如果要突破10人限制，请联系 www.oraycn.com
    /// </summary>
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        private static ESPlus.Rapid.IRapidServerEngine RapidServerEngine = ESPlus.Rapid.RapidEngineFactory.CreateServerEngine();
        internal static UpdateConfiguration UpgradeConfiguration = null;

        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //如果是其它类型的授权用户，请使用下面的语句设定正确的授权用户ID和密码。              
                GlobalUtil.SetAuthorizedUser("FreeUser", "");               

                //初始化服务端引擎
                CustomizeHandler customizeHandler = new CustomizeHandler();
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                RapidServerEngine.WriteTimeoutInSecs = -1;
                RapidServerEngine.Initialize(port, customizeHandler);
                RapidServerEngine.UserManager.RelogonMode = RelogonMode.IgnoreNew;

                //动态生成或加载配置信息                               
                if (!File.Exists(UpdateConfiguration.ConfigurationPath))
                {
                    Program.UpgradeConfiguration = new UpdateConfiguration();
                    Program.UpgradeConfiguration.Save();
                }
                else
                {
                    Program.UpgradeConfiguration = (UpdateConfiguration)UpdateConfiguration.Load(UpdateConfiguration.ConfigurationPath);
                }

                customizeHandler.Initialize(RapidServerEngine.FileController, Program.UpgradeConfiguration);

                bool remoting = bool.Parse(ConfigurationManager.AppSettings["RemotingServiceEnabled"]);
                if (remoting)
                {
                    ChannelServices.RegisterChannel(new TcpChannel(port + 2), false);
                    OausService service = new OausService(Program.UpgradeConfiguration);
                    RemotingServices.Marshal(service, "OausService"); 
                }


                //显示默认主窗体
                MainForm mainForm = new MainForm(Program.RapidServerEngine);
                mainForm.Text = ConfigurationManager.AppSettings["Title"];   
                Application.Run(mainForm);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message+" - " + ee.StackTrace);
            }
        }
        
    }
}
