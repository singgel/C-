using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    internal enum Facility
    {
        Null,
        Rpc,
        Dispatch,
        Storage,
        Itf,
        Win32 = 7,
        Windows,
        Control = 10,
        Ese = 3678
    }
}
