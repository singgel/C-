using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.WinApi
{
    public class WindowStyles
    {
        public const long WS_OVERLAPPED = 0x00000000L
                                    , WS_POPUP = 0x80000000L
                                    , WS_CHILD = 0x40000000L
                                    , WS_MINIMIZE = 0x20000000L
                                    , WS_VISIBLE = 0x10000000L
                                    , WS_DISABLED = 0x08000000L
                                    , WS_CLIPSIBLINGS = 0x04000000L
                                    , WS_CLIPCHILDREN = 0x02000000L
                                    , WS_MAXIMIZE = 0x01000000L
                                    , WS_CAPTION = 0x00C00000L     /* WS_BORDER | WS_DLGFRAME  */
                                    , WS_BORDER = 0x00800000L
                                    , WS_DLGFRAME = 0x00400000L
                                    , WS_VSCROLL = 0x00200000L
                                    , WS_HSCROLL = 0x00100000L
                                    , WS_SYSMENU = 0x00080000L
                                    , WS_THICKFRAME = 0x00040000L
                                    , WS_GROUP = 0x00020000L
                                    , WS_TABSTOP = 0x00010000L

                                    , WS_MINIMIZEBOX = 0x00020000L
                                    , WS_MAXIMIZEBOX = 0x00010000L;
    }

    public class EditStyles
    {
        public const long ES_LEFT = 0x0000L
                                    , ES_CENTER = 0x0001L
                                    , ES_RIGHT = 0x0002L
                                    , ES_MULTILINE = 0x0004L
                                    , ES_UPPERCASE = 0x0008L
                                    , ES_LOWERCASE = 0x0010L
                                    , ES_PASSWORD = 0x0020L
                                    , ES_AUTOVSCROLL = 0x0040L
                                    , ES_AUTOHSCROLL = 0x0080L
                                    , ES_NOHIDESEL = 0x0100L
                                    , ES_OEMCONVERT = 0x0400L
                                    , ES_READONLY = 0x0800L
                                    , ES_WANTRETURN = 0x1000L
                                    , ES_NUMBER = 0x2000L;
    }
}
