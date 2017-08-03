using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// SaveDialog_3C,DoFillWork传入值应为null。
    /// </summary>
    public class SaveDialog_3C : FillDialog_3C
    {
        private IntPtr _okButton, _tips;

        public string Message
        {
            get;
            private set;
        }

        public SaveDialog_3C(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public override bool DoFillWork(object state)
        {
            if (_okButton == IntPtr.Zero)
                return false;
            return ApiSetter.ClickButton(_okButton, base.HWnd, null, null);
        }

        public override void InitHandle()
        {
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                int id = NativeApi.GetDlgCtrlID(hwnd);
                if (id == 0xFFFF)
                    _tips = hwnd;
                else if (id == 0x2)
                    _okButton = hwnd;
                return _okButton == IntPtr.Zero || _tips == IntPtr.Zero;
            }, IntPtr.Zero);
            StringBuilder text = new StringBuilder(256);
            NativeApi.GetWindowText(_tips, text, 255);
            this.Message = text.ToString();
        }

        public override bool IsValidWindow()
        {
            return _okButton != IntPtr.Zero && _tips != IntPtr.Zero;
            //StringBuilder className = new StringBuilder(256);
            //NativeApi.GetClassName(base.HWnd, className, 255);
            //if (className.ToString() == "#32770")
            //{
            //    if (_okButton != IntPtr.Zero && NativeApi.GetDlgCtrlID(_okButton) != 0x2)
            //        return false;
            //    return true;
            //}
            //return false;
        }
    }
}
