using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class tagDISPPARAMS
    {
        public IntPtr rgvarg;
        public IntPtr rgdispidNamedArgs;
        [MarshalAs(UnmanagedType.U4)]
        public int cArgs;
        [MarshalAs(UnmanagedType.U4)]
        public int cNamedArgs;
    }
}
