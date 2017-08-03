using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 打开文件窗口。
    /// </summary>
    public class OpenFileDialog_3C : FillDialog_3C
    {
        private IntPtr fileName, okButton;

        public OpenFileDialog_3C(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public static OpenFileDialog_3C GetOpenFileDialog(int processId)
        {
            OpenFileDialog_3C current = null;
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumWindows((hwnd, lParam) =>
            {
                uint currentProcessId = 0;
                NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                if (currentProcessId == processId)
                {
                    //保存提示                   
                    //提示                   
                    if (className.ToString() == "#32770")
                        current = new OpenFileDialog_3C(hwnd);
                }
                return current == null;
            }, IntPtr.Zero);
            return current;
        }

        public override bool DoFillWork(object state)
        {
            string fileName = state as string;
            ApiSetter.SetText(this.fileName, fileName);
            ApiSetter.ClickButton(okButton, base.HWnd, null, null);
            return true;
        }

        public override void InitHandle()
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                NativeApi.GetClassName(hwnd, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr=="Button")
                {
                    int ctrlId = NativeApi.GetDlgCtrlID(hwnd);
                    if (ctrlId == 1)
                        okButton = hwnd;
                }
                else if (classNameStr == "Edit")
                {
                    int ctrlId = NativeApi.GetDlgCtrlID(hwnd);
                    if (ctrlId == 0x047C)
                        fileName = hwnd;
                }
                return okButton == IntPtr.Zero || fileName == IntPtr.Zero;
            }, IntPtr.Zero);
        }

        public override bool IsValidWindow()
        {
            return fileName != IntPtr.Zero && okButton != IntPtr.Zero;
        }
    }
}
