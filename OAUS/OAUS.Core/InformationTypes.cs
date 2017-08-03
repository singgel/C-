using System;
using System.Collections.Generic;
using System.Text;

namespace OAUS.Core
{
    /// <summary>
    /// 自定义信息的类型
    /// </summary>
    public static class InformationTypes
    {
        /// <summary>
        /// 获取所有文件信息
        /// </summary>
        public const int GetAllFilesInfo = 0;
        /// <summary>
        /// 请求下载文件
        /// </summary>
        public const int DownloadFiles = 1;
        /// <summary>
        /// 获取服务端最后更新的时间
        /// </summary>
        public const int GetLastUpdateTime = 2;

    }
}
