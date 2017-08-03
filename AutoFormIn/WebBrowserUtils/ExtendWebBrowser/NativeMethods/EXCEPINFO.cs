using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    [StructLayout(LayoutKind.Sequential)]
    public class EXCEPINFO
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort wCode;
        [MarshalAs(UnmanagedType.U2)]
        public ushort wReserved;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrSource;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrDescription;
        [MarshalAs(UnmanagedType.BStr)]
        public string bstrHelpFile;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwHelpContext;
        public IntPtr pvReserved;
        public IntPtr pfnDeferredFillIn;
        [MarshalAs(UnmanagedType.I4)]
        public int scode;

        public EXCEPINFO()
        {
        }
    }
}
