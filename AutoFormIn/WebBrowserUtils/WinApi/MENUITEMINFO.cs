using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.WinApi
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MENUITEMINFO
    {
        public uint cbSize;
        public uint fMask;
        public uint fType;
        public uint fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        public String dwTypeData;
        public uint cch;
        public IntPtr hbmpItem;

        // Return the size of the structure
        public static uint sizeOf
        {
            get { return (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); }
        }
    }
}
