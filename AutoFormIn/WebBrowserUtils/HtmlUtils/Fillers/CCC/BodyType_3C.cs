using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.CCC
{
    public class BodyType_3C : FillDialog_3C
    {
        private IntPtr _okButton;

        private BodyType_3C(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public override void InitHandle()
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.Hwnd, (handle, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(handle, className, 255);
                string str = className.ToString();
                if (str.StartsWith(CCCFiller.ButtonClassName))
                {
                    StringBuilder text = new StringBuilder(6);
                    NativeApi.GetWindowText(handle, text, 5);
                    if (text.ToString() == "确定")
                    {
                        _okButton = handle;
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);
        }

        public static BodyType_3C GetBodyType(int processId)
        {
            BodyType_3C current = null;
            NativeApi.EnumWindows((hwnd, lparam) =>
            {
                uint currentProcessId = 0;
                NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                if (currentProcessId == processId)
                {
                    StringBuilder title = new StringBuilder(256);
                    int len = NativeApi.GetWindowText(hwnd, title, 255);
                    if (title.ToString() == "请输入文件名")
                    {
                        current = new BodyType_3C(hwnd);
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);
            if (current != null)
                current.InitHandle();
            return current;
        }

        public override bool DoFillWork(object str)
        {
            return ApiSetter.CheckRadioButton(base.Hwnd, str as string) && ApiSetter.ClickButton(_okButton, base.Hwnd);
        }
    }
}
