using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.WinApi
{
    public class NotificationCode
    {
        /*ListBox通知消息代码*/
        public const int LBN_ERRSPACE = -2;
        public const int LBN_SELCHANGE = 1;
        public const int LBN_DBLCLK = 2;
        public const int LBN_SELCANCEL = 3;
        public const int LBN_SETFOCUS = 4;
        public const int LBN_KILLFOCUS = 5;
        /*ComboBox 通知消息代码*/
        public const int CBN_ERRSPACE = -1;
        public const int CBN_SELCHANGE = 1;
        public const int CBN_DBLCLK = 2;
        public const int CBN_SETFOCUS = 3;
        public const int CBN_KILLFOCUS = 4;
        public const int CBN_EDITCHANGE = 5;
        public const int CBN_EDITUPDATE = 6;
        public const int CBN_DROPDOWN = 7;
        public const int CBN_CLOSEUP = 8;
        public const int CBN_SELENDOK = 9;
        public const int CBN_SELENDCANCEL = 10;
    }
}
