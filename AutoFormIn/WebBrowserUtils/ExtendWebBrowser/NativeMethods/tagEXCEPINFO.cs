using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public class tagEXCEPINFO
    {
        [MarshalAs(UnmanagedType.U2)]
        public short wCode;
        [MarshalAs(UnmanagedType.U2)]
        public short wReserved;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrSource;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrDescription;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrHelpFile;
        [MarshalAs(UnmanagedType.U4)]
        public int dwHelpContext;
        public IntPtr pvReserved = IntPtr.Zero;
        public IntPtr pfnDeferredFillIn = IntPtr.Zero;
        [MarshalAs(UnmanagedType.U4)]
        public int scode;
    }
}
