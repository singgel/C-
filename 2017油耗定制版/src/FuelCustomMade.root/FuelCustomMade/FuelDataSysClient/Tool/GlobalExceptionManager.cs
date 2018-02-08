using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Tool
{
    /// <summary>
    /// 全局异常管理类
    /// </summary>
    internal static class GlobalExceptionManager
    {
        #region Public Methods
        /// <summary>
        /// 显示全局异常提示信息
        /// </summary>
        /// <param name="globalException">捕获到的全局异常</param>
        /// <param name="applicationName">当前应用程序名称</param>
        /// <param name="developerName">程序开发者名称</param>
        public static void ShowGlobalExceptionInfo(Exception globalException, string applicationName, string developerName)
        {
            // 创建全局异常提示窗体实例对象
            GlobalExceptionForm frmGlobalException = new GlobalExceptionForm(globalException, applicationName, developerName);
            // 以对话框模式显示
            frmGlobalException.ShowDialog();
        }

        #endregion
    }
}
