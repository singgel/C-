using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.WinApi
{
    public class ListBoxStyles
    {
        public const long LBS_NOTIFY = 0x0001L;
        public const long LBS_SORT = 0x0002L;
        public const long LBS_NOREDRAW = 0x0004L;
        public const long LBS_MULTIPLESEL = 0x0008L;
        public const long LBS_OWNERDRAWFIXED = 0x0010L;
        public const long LBS_OWNERDRAWVARIABLE = 0x0020L;
        public const long LBS_HASSTRINGS = 0x0040L;
        public const long LBS_USETABSTOPS = 0x0080L;
        public const long LBS_NOlongEGRALHEIGHT = 0x0100L;
        public const long LBS_MULTICOLUMN = 0x0200L;
        public const long LBS_WANTKEYBOARDINPUT = 0x0400L;
        public const long LBS_EXTENDEDSEL = 0x0800L;
        public const long LBS_DISABLENOSCROLL = 0x1000L;
        public const long LBS_NODATA = 0x2000L;
        public const long LBS_NOSEL = 0x4000L;
        public const long LBS_COMBOBOX = 0x8000L;
        public const long LBS_STANDARD = (LBS_NOTIFY | LBS_SORT | WindowStyles.WS_VSCROLL | WindowStyles.WS_BORDER);
    }
}
