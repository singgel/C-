using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public enum FillState
    {
        /// <summary>
        /// 新建填报。
        /// </summary>
        New = 0,
        /// <summary>
        /// 正在执行填报。
        /// </summary>
        Running = 1,
        /// <summary>
        /// 填报因为子页面或窗口的打开而被挂起。
        /// </summary>
        Suspended = 2,
        /// <summary>
        /// 填报进入等待状态。
        /// </summary>
        Waiting = 3,
        /// <summary>
        /// 填报已结束。
        /// </summary>
        End = 4,
        /// <summary>
        /// 填报时发生异常。
        /// </summary>
        Exception = 5,
        /// <summary>
        /// 填报被终止。
        /// </summary>
        Abort = 6
    }
}
