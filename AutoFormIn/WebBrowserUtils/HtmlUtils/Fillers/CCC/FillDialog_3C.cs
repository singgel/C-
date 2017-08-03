using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public abstract class FillDialog_3C
    {
        private string _title;
        private static bool _run;
        private const int SWP_NOSIZE = 0x1;

        public Main_3C Owner
        {
            get;
            private set;
        }

        public IntPtr HWnd
        {
            get;
            private set;
        }

        public string Title
        {
            get { return _title; }
        }

        protected internal object FillValue
        {
            get;
            set;
        }

        protected FillDialog_3C(IntPtr _hwnd)
        {
            HWnd = _hwnd;
            StringBuilder title = new StringBuilder(256);
            NativeApi.GetWindowText(_hwnd, title, 255);
            _title = title.ToString();
        }

        static FillDialog_3C()
        {
            _run = true;
        }

        public virtual void InitHandle()
        {
        }

        public abstract bool DoFillWork(object state);

        public abstract bool IsValidWindow();

        public static IntPtr GetWindowHandle(IntPtr owner, uint processId)
        {
            IntPtr hWnd = IntPtr.Zero;
            NativeApi.EnumWindows((hwnd, lparam) =>
            {
                uint currentProcessId = 0;
                NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                if (currentProcessId == processId)
                {
                    long style = NativeApi.GetWindowLong(hwnd, -16);
                    if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE)
                        return true;
                    if (owner == IntPtr.Zero)
                        hWnd = hwnd;
                    else
                    {
                        //返回owner的子窗口
                        IntPtr hwndOwner = NativeApi.GetWindow(hwnd, 0x4);
                        if (hwndOwner == owner)
                            hWnd = hwnd;
                    }
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return hWnd;
        }

        public static FillDialog_3C GetFillDialog(string type, uint processId)
        {
            return GetFillDialog(type, IntPtr.Zero, processId);
        }

        private static FillDialog_3C GetAttachWindow(uint processId)
        {
            FillDialog_3C current = null;
            StringBuilder text = new StringBuilder(256);
            do
            {
                System.Threading.Thread.Sleep(500);
                NativeApi.EnumWindows((hwnd, lparam) =>
                {
                    text.Clear();
                    uint currentProcessId = 0;
                    NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                    if (currentProcessId == processId)
                    {
                        long style = NativeApi.GetWindowLong(hwnd, -16);
                        if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE)
                            return true;
                        NativeApi.GetWindowText(hwnd, text, 255);
                        if (text.ToString() == "编辑附件")
                        {
                            current = new AttachWindow_3C(hwnd);
                            current.InitHandle();
                            return false;
                        }
                    }
                    return true;
                }, IntPtr.Zero);
            } while (_run && (current == null || current.IsValidWindow() == false));
            return current;
        }

        private static FillDialog_3C GetSaveDialog(uint processId)
        {
            FillDialog_3C current = null;
            StringBuilder text = new StringBuilder(256);
            do
            {
                System.Threading.Thread.Sleep(500);
                NativeApi.EnumWindows((hwnd, lparam) =>
                {
                    text.Clear();
                    uint currentProcessId = 0;
                    NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                    if (currentProcessId == processId)
                    {
                        long style = NativeApi.GetWindowLong(hwnd, -16);
                        if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE)
                            return true;
                        NativeApi.GetWindowText(hwnd, text, 255);
                        current = new SaveDialog_3C(hwnd);
                        current.InitHandle();
                        if(current.IsValidWindow())
                            return false;
                    }
                    return true;
                }, IntPtr.Zero);
            } while (_run && (current == null || current.IsValidWindow() == false));
            return current;
        }

        public static FillDialog_3C GetFillDialog(string type, IntPtr owner, uint processId)
        {
            IntPtr hwnd = IntPtr.Zero;
            FillDialog_3C current = null;
            if (type == CCCWindowType.AttachWindow)
                current = GetAttachWindow(processId);
            else if (type == CCCWindowType.SaveWindow)
                current = GetSaveDialog(processId);
            else
            {
                do
                {
                    System.Threading.Thread.Sleep(500);
                    hwnd = GetWindowHandle(owner, processId);
                    switch (type)
                    {
                    case CCCWindowType.AddWindow:
                        current = new AddWindow(hwnd);
                        break;
                    case CCCWindowType.FirmWindow:
                        current = new SelectFirmRelation_3C(hwnd);
                        break;
                    case CCCWindowType.InfoTipWindow:
                        current = new InfoTips_3C(hwnd);
                        break;
                    case CCCWindowType.InputFileNameWindow:
                        current = new InputFileName_3C(hwnd);
                        break;
                    case CCCWindowType.ListCheckBoxWindow:
                        current = new CheckListBox_3C(hwnd);
                        break;
                    case CCCWindowType.LoginWindow:
                        current = new Login_3C(hwnd);
                        break;
                    case CCCWindowType.MultiValueAndNote:
                        current = new AddValueAndNote(hwnd);
                        break;
                    case CCCWindowType.OpenFileWindow:
                        current = new OpenFileDialog_3C(hwnd);
                        break;
                    case CCCWindowType.PropertyWindow:
                        current = new SelectProperty_3C(hwnd);
                        break;
                    case CCCWindowType.RadioWindow:
                        current = new RadioButtonWindow(hwnd);
                        break;
                    case CCCWindowType.TextWindow:
                        current = new TextEditWindow(hwnd);
                        break;
                    case CCCWindowType.ColumnWindow:
                        current = new ColumnWindow(hwnd);
                        break;
                    }
                    if (current != null)
                        current.InitHandle();
                } while (_run && (hwnd == IntPtr.Zero || current == null || current.IsValidWindow() == false));
            }
            return current;
        }

        public static FillDialog_3C GetFillDialog(string type, Main_3C main, uint processId)
        {
            FillDialog_3C current = GetFillDialog(type, main == null ? IntPtr.Zero : main.Hwnd, processId);
            if (current != null)
                current.Owner = main;
            return current;
        }

        public static FillDialog_3C GetFillDialog(out string windowType, uint processId)
        {
            windowType = null;
            FillDialog_3C current = null;
            IntPtr hwnd = GetWindowHandle(IntPtr.Zero, processId);
            current = new Login_3C(hwnd);
            current.InitHandle();
            if (current.IsValidWindow() == false)
            {
                current = new SelectFirmRelation_3C(hwnd);
                current.InitHandle();
                if (current.IsValidWindow() == false)
                    return null;
                else
                {
                    windowType = CCCWindowType.FirmWindow;
                    return current;
                }
            }
            else
            {
                windowType = CCCWindowType.LoginWindow;
                return current;
            }
        }

        public static SaveRequireWindow GetSaveRequireWindow(Main_3C main, uint processId)
        {
            IntPtr hwnd = IntPtr.Zero;
            SaveRequireWindow current = null;
            do
            {
                System.Threading.Thread.Sleep(500);
                hwnd = GetWindowHandle(main.Hwnd, processId);
                if (hwnd != IntPtr.Zero)
                    current = new SaveRequireWindow(hwnd);
                if (current != null)
                    current.InitHandle();
            } while (_run && (hwnd == IntPtr.Zero || current == null || current.IsValidWindow() == false));
            if (current != null && current.IsValidWindow())
            {
                current.Owner = main;
                return current;
            }
            return null;
        }

        internal static void EndListen()
        {
            _run = false;
        }

        internal static void BeginListen()
        {
            _run = true;
        }

        internal static void BeginListenSaveRequire(Main_3C main)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(ListenSaveRequireWindow, main);
        }
        /// <summary>
        /// 监听询问是否保存窗口。
        /// </summary>
        /// <param name="value">当前选择的节点文本。</param>
        internal static void ListenSaveRequireWindow(object value)
        {
            Main_3C main = value as Main_3C;
            while (_run)
            {
                SaveRequireWindow fill = FillDialog_3C.GetSaveRequireWindow(main, main.ProcessId);
                string message = string.Format("是否保存{0}数据？", main.SelectedText);
                if (fill != null && fill.Message != null && fill.Message == message)
                {
                    fill.DoFillWork(value);
                }
            }
        }
    }
}
