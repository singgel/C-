using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    [Guid("00020400-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IDispatch
    {
        int GetTypeInfoCount();
        [return: MarshalAs(UnmanagedType.Interface)]
        dynamic GetTypeInfo([MarshalAs(UnmanagedType.U4)] [In] int iTInfo, [MarshalAs(UnmanagedType.U4)] [In] int lcid);
        [PreserveSig]
        int GetIDsOfNames([In] ref Guid riid, [MarshalAs(UnmanagedType.LPArray)] [In] string[] rgszNames, [MarshalAs(UnmanagedType.U4)] [In] int cNames, [MarshalAs(UnmanagedType.U4)] [In] int lcid, [MarshalAs(UnmanagedType.LPArray)] [Out] int[] rgDispId);
        [PreserveSig]
        int Invoke(int dispIdMember, [In] ref Guid riid, [MarshalAs(UnmanagedType.U4)] [In] int lcid, [MarshalAs(UnmanagedType.U4)] [In] int dwFlags, [In] [Out] NativeMethods.tagDISPPARAMS pDispParams, [MarshalAs(UnmanagedType.LPArray)] [Out] object[] pVarResult, [In] [Out] NativeMethods.tagEXCEPINFO pExcepInfo, [MarshalAs(UnmanagedType.LPArray)] [Out] IntPtr[] pArgErr);
    }
}
