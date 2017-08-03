using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Assistant.CCCApi
{
    public class CCCInterop
    {
        private const string dllName = @"D:\Projects\CSharp\FlexDeclaration\HCBM\Debug\3CInterop.dll";
        [DllImport(dllName)]
        public extern static IntPtr SetCallWndProcHook(IntPtr notifyWnd, uint threadId);
        [DllImport(dllName)]
        public extern static bool UnHookCallWndProc(IntPtr hhk);
    }
}
