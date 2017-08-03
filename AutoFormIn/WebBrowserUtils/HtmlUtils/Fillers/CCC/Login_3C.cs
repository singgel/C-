using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 证书登录窗口。
    /// </summary>
    public class Login_3C : FillDialog_3C
    {
        private IntPtr _comboBoxHandle, _okBtnHandle;

        public Login_3C(IntPtr hwnd)
            :base(hwnd)
        {
            _comboBoxHandle = IntPtr.Zero;
            _okBtnHandle = IntPtr.Zero;
        }

        public override void InitHandle()
        {
            WinApi.NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                NativeApi.GetClassName(hwnd, className, 255);
                string text = className.ToString();
                if (text.StartsWith(CCCFillManager.ComboBoxClassName))
                    _comboBoxHandle = hwnd;
                else if (text.StartsWith(CCCFillManager.ButtonClassName))
                {
                    className.Clear();
                    NativeApi.GetWindowText(hwnd, className, 255);
                    if (className.ToString() == "确定")
                        _okBtnHandle = hwnd;
                }
                if (_comboBoxHandle != IntPtr.Zero && _okBtnHandle != IntPtr.Zero)
                    return false;
                return true;
            }, IntPtr.Zero);
        }

        public override bool DoFillWork(object fillValue)
        {
            bool result = false;
            FillValue3C value = fillValue as FillValue3C;
            if (value != null)
                result = ApiSetter.SetComboBoxSelected(base.HWnd, _comboBoxHandle, value.Value, true);
            ApiSetter.ClickButton(_okBtnHandle, base.HWnd, StartFillSelectFirm, this.FillValue);
            return result;
        }

        private void StartFillSelectFirm(object state)
        {
            FillValue3C value = state as FillValue3C;
            if (value != null)
            {
                uint processId;
                NativeApi.GetWindowThreadProcessId(base.HWnd, out processId);
                FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.FirmWindow, processId);
                fill.DoFillWork(value.Value);
            }
        }

        public override bool IsValidWindow()
        {
            return base.Title == "证书登录";
        }
    }
}
