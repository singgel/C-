using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.WinApi
{
    public class MouseVirtualKeys
    {
        /// <summary>
        /// 鼠标左键被按下。
        /// </summary>
        public const int None = 0x0;
        /// <summary>
        /// CTRL键被按下。
        /// </summary>
        public const int MK_CONTROL = 0x0008;
        /// <summary>
        /// 鼠标中键被按下。
        /// </summary>
        public const int MK_MBUTTON = 0x0010;
        /// <summary>
        /// 鼠标右键被按下。
        /// </summary>
        public const int MK_RBUTTON = 0x0002;
        /// <summary>
        /// SHIFT键被按下。
        /// </summary>
        public const int MK_SHIFT = 0x0004;
        /// <summary>
        /// 第一个X按钮被按下。
        /// </summary>
        public const int MK_XBUTTON1 = 0x0020;
        /// <summary>
        /// 第二个X按钮被按下。
        /// </summary>
        public const int MK_XBUTTON2 = 0x0040;
    }
}
