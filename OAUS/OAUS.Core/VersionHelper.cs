using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core
{
    /// <summary>
    /// 给应用的客户端使用，用于获取升级的版本信息。
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// 获取当前客户端的版本号。
        /// </summary>        
        public static int GetCurrentVersion()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "AutoUpdater\\UpdateConfiguration.xml";
            UpdateConfiguration config = (UpdateConfiguration)UpdateConfiguration.Load(path);
            return config.ClientVersion;
        }

        /// <summary>
        /// 从服务端获得最新客户端的版本号。【前提是服务端的配置项RemotingServiceEnabled必需为true】
        /// </summary>
        /// <param name="oausServerIP">OAUS服务端的IP</param>
        /// <param name="oausServerPort">OAUS服务端的端口</param>        
        public static int GetLatestVersion(string oausServerIP, int oausServerPort)
        {
            IOausService service = (IOausService)Activator.GetObject(typeof(IOausService), string.Format("tcp://{0}:{1}/OausService", oausServerIP, oausServerPort+2));
            return service.GetLatestVersion();        
        }

        /// <summary>
        /// 是否有新版本？【前提是服务端的配置项RemotingServiceEnabled必需为true】
        /// </summary>
        /// <param name="oausServerIP">OAUS服务端的IP</param>
        /// <param name="oausServerPort">OAUS服务端的端口</param>        
        public static bool HasNewVersion(string oausServerIP, int oausServerPort)
        {
            return VersionHelper.GetLatestVersion(oausServerIP, oausServerPort) > VersionHelper.GetCurrentVersion();
        }
    }
}
