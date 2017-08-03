using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.WinApi
{
    public class SysTreeView
    {
        private IntPtr _handle;
        public const int TVE_COLLAPSE = 0x1;
        public const int TVE_EXPAND = 0x2;

        public const int TVGN_ROOT = 0x0000;
        public const int TVGN_NEXT = 0x0001;
        public const int TVGN_PREVIOUS = 0x0002;
        public const int TVGN_PARENT = 0x0003;
        public const int TVGN_CHILD = 0x0004;
        public const int TVGN_FIRSTVISIBLE = 0x0005;
        public const int TVGN_NEXTVISIBLE = 0x0006;
        public const int TVGN_PREVIOUSVISIBLE = 0x0007;
        public const int TVGN_DROPHILITE = 0x0008;
        public const int TVGN_CARET = 0x0009;
        public const int TVGN_LASTVISIBLE = 0x000A;

        public const int TVIF_TEXT = 0x0001;
        public const int TVIF_IMAGE = 0x0002;
        public const int TVIF_PARAM = 0x0004;
        public const int TVIF_STATE = 0x0008;
        public const int TVIF_HANDLE = 0x0010;
        public const int TVIF_SELECTEDIMAGE = 0x0020;
        public const int TVIF_CHILDREN = 0x0040;
        public const int TVIF_INTEGRAL = 0x0080;

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public SysTreeView(IntPtr handle)
        {
            _handle = handle;
        }

        public uint GetCount()
        {
            return (uint)NativeApi.SendMessage(_handle, WMMSG.TVM_GETCOUNT, 0, 0);
        }
        /// <summary>
        /// 获取指定项文本
        /// </summary>
        /// <param name="TreeViewHwnd">树对象句柄</param>
        /// <param name="ItemHwnd">节点句柄</param>
        /// <returns></returns>
        public unsafe string GetItemText(IntPtr ItemHwnd, IntPtr process, IntPtr addr)
        {
            uint n = 0;
            TVITEM tv = new TVITEM();
            tv.hItem = ItemHwnd;
            tv.mask = TVIF_TEXT;
            tv.pszText = addr + sizeof(TVITEM);
            tv.cchTextMax = 1024;
            byte[] text = new byte[1024];
            fixed (byte* b = text)
            {
                NativeApi.WriteProcessMemory(process, addr, (IntPtr)(&tv), sizeof(TVITEM), ref n);
                NativeApi.WriteProcessMemory(process, tv.pszText, (IntPtr)(b), 1024, ref n);
                IntPtr length = NativeApi.SendMessage(_handle, WMMSG.TVM_GETITEM, 0, addr.ToInt32());
                NativeApi.ReadProcessMemory(process, addr, (IntPtr)(&tv), sizeof(TVITEM), ref n);
                NativeApi.ReadProcessMemory(process, addr + sizeof(TVITEM), (IntPtr)(b), 1024, ref n);
            }
            string nodeText = Encoding.Default.GetString(text);
            return nodeText.Substring(0, nodeText.IndexOf('\0'));
        }
        /// <summary>
        /// 选取TreeView指定项
        /// </summary>
        /// <param name="TreeViewHwnd">树对象句柄</param>
        /// <param name="ItemHwnd">节点对象句柄</param>
        /// <returns>成功选中返回true 没找到返回false</returns>
        public bool SelectNode(IntPtr ItemHwnd)
        {
            IntPtr result = NativeApi.SendMessage(_handle, WMMSG.TVM_SELECTITEM, TVGN_CARET, ItemHwnd.ToInt32());
            return result != IntPtr.Zero;
        }
        /// <summary>
        /// 获取TreeView根节点
        /// </summary>
        /// <param name="TreeViewHwnd">树对象句柄</param>
        /// <returns>成功返回根节点句柄 否则返回 0</returns>
        public unsafe IntPtr GetRoot()
        {
            TVITEM tv = new TVITEM();
            IntPtr hStr = Marshal.AllocHGlobal(1024);
            tv.hItem = _handle;
            tv.mask = TVIF_TEXT;
            tv.pszText = hStr;
            tv.cchTextMax = 1024;
            IntPtr result = NativeApi.SendMessage(_handle, WMMSG.TVM_GETNEXTITEM, TVGN_ROOT, (int)&tv);
            Marshal.FreeHGlobal(hStr);
            return result;
        }
        /// <summary>
        /// 获取同级下一节点句柄
        /// </summary>
        /// <param name="TreeViewHwnd">树对象句柄</param>
        /// <param name="node">上一项节点句柄</param>
        /// <returns>成功返回下一项节点句柄 否则返回 0</returns>
        public IntPtr GetNextNode(IntPtr node)
        {
            IntPtr mbHwnd = IntPtr.Zero;
            mbHwnd = NativeApi.SendMessage(_handle, WMMSG.TVM_GETNEXTITEM, TVGN_NEXT, node.ToInt32());
            return mbHwnd;
        }
        /// <summary>
        /// 获取指定元素的下一个可见节点。
        /// </summary>
        /// <param name="node">上一个可见节点句柄。</param>
        /// <returns>成功返回下一项节点句柄 否则返回 0</returns>
        public IntPtr GetNextVisible(IntPtr node)
        {
            IntPtr mbHwnd = IntPtr.Zero;
            mbHwnd = NativeApi.SendMessage(_handle, WMMSG.TVM_GETNEXTITEM, TVGN_NEXTVISIBLE, node.ToInt32());
            return mbHwnd;
        }
        /// <summary>
        /// 获取第一个子节点句柄
        /// </summary>
        /// <param name="TreeViewHwnd">树对象句柄</param>
        /// <param name="parentNode">父节点句柄</param>
        /// <returns>成功返回第一个子节点句柄 否则返回 0</returns>
        public IntPtr GetFirstChildItem(IntPtr parentNode)
        {
            IntPtr result = NativeApi.SendMessage(_handle, WMMSG.TVM_GETNEXTITEM, TVGN_CHILD, parentNode.ToInt32());
            return result;
        }
        /// <summary>
        /// 展开指定节点。
        /// </summary>
        /// <param name="node"></param>
        public void ExpandNode(IntPtr node)
        {
            NativeApi.SendMessage(_handle, WMMSG.TVM_EXPAND, TVE_EXPAND, node.ToInt32());
        }
        /// <summary>
        /// 获取指定节点的父级节点。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IntPtr GetParentNode(IntPtr node)
        {
            return NativeApi.SendMessage(_handle, WMMSG.TVM_GETNEXTITEM, TVGN_PARENT, node.ToInt32());
        }

        public void ScrollIntoView(IntPtr node)
        {
            NativeApi.SendMessage(_handle, WMMSG.TVM_ENSUREVISIBLE, 0, node.ToInt32());
        }

        public IntPtr GetSelected()
        {
            return NativeApi.SendMessage(_handle, WMMSG.TVM_GETNEXTITEM, TVGN_CARET, 0);
        }

        public void CollapseNode(IntPtr node)
        {
            NativeApi.SendMessage(_handle, WMMSG.TVM_EXPAND, TVE_COLLAPSE, node.ToInt32());
        }
    }
}
