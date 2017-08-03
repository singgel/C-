using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.ExtendWebBrowser.NativeMethods
{
    internal class SafeNativeMethods
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FindSizeOfVariant
        {
            [MarshalAs(UnmanagedType.Struct)]
            public object var;
            public byte b;
        }

        private static readonly int VariantSize = (int)Marshal.OffsetOf(typeof(FindSizeOfVariant), "b");

        public static bool Succeeded(int hr)
        {
            return hr >= 0;
        }

        internal unsafe static IntPtr ArrayToVARIANTVector(object[] args)
        {
            int num = args.Length;
            IntPtr intPtr = Marshal.AllocCoTaskMem(num * VariantSize);
            byte* ptr = (byte*)((void*)intPtr);
            for (int i = 0; i < num; i++)
            {
                Marshal.GetNativeVariantForObject(args[i], (IntPtr)((void*)(ptr + VariantSize * i / 1)));
            }
            return intPtr;
        }

        internal unsafe static void FreeVARIANTVector(IntPtr mem, int len)
        {
            byte* ptr = (byte*)((void*)mem);
            for (int i = 0; i < len; i++)
            {
                SafeNativeMethods.VariantClear(new HandleRef(null, (IntPtr)((void*)(ptr + VariantSize * i / 1))));
            }
            Marshal.FreeCoTaskMem(mem);
        }

        [DllImport("oleaut32.dll", PreserveSig = false)]
        public static extern void VariantClear(HandleRef pObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "GetThreadLocale")]
        public static extern int GetThreadLCID();
    }
}
