using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// CheckListBox_3C，DoFillWork传入值应为null。
    /// </summary>
    public class InfoTips_3C : FillDialog_3C
    {
        private IntPtr _okButton;

        public InfoTips_3C(IntPtr hwnd)
            :base(hwnd)
        {
        }

        public static InfoTips_3C GetInfoTipWindow(int processId)
        {
            InfoTips_3C current = null;
            NativeApi.EnumWindows((hwnd, lparam) =>
            {
                uint currentProcessId = 0;
                NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                if (currentProcessId == processId)
                {
                    StringBuilder title = new StringBuilder(256);
                    int len = NativeApi.GetWindowText(hwnd, title, 255);
                    if (title.ToString().StartsWith("信息提示"))
                    {
                        current = new InfoTips_3C(hwnd);
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);
            if (current != null)
                current.InitHandle();
            return current;
        }

        public override void InitHandle()
        {
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                if (NativeApi.GetDlgCtrlID(hwnd) == 1)
                {
                    _okButton = hwnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
        }

        public override bool DoFillWork(object state)
        {
            return ApiSetter.ClickButton(_okButton, base.HWnd, null, null);
        }

        public override bool IsValidWindow()
        {
            return base.Title.Contains("信息提示");
        }
    }
}
