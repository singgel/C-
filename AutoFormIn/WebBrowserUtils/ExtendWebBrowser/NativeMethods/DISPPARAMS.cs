using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DISPPARAMS
    {
        public IntPtr rgvarg;
        public IntPtr rgdispidNamedArgs;
        [MarshalAs(UnmanagedType.U4)]
        public uint cArgs;
        [MarshalAs(UnmanagedType.U4)]
        public uint cNamedArgs;

        public DISPPARAMS()
        {
        }
    }
}
