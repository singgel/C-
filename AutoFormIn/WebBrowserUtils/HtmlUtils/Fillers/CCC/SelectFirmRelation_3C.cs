using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class SelectFirmRelation_3C : FillDialog_3C
    {
        private IntPtr _comboBoxHandle, _okBtnHandle;

        public SelectFirmRelation_3C(IntPtr hwnd)
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
                    if (className.ToString() == "确  定")
                        _okBtnHandle = hwnd;
                }
                if (_comboBoxHandle != IntPtr.Zero && _okBtnHandle != IntPtr.Zero)
                    return false;
                return true;
            }, IntPtr.Zero);
        }

        public override bool DoFillWork(object firm)
        {
            return ApiSetter.SetComboBoxSelected(base.HWnd, _comboBoxHandle, firm as string)
                && ApiSetter.ClickButton(_okBtnHandle, base.HWnd, null, null); 
        }

        public bool SelectFirm(string firmName)
        {
            if (_comboBoxHandle != null)
            {
                IntPtr result = NativeApi.SendMessage(_comboBoxHandle, WMMSG.CB_SELECTSTRING, 0, firmName);
                return result.ToInt32() != -1;
            }
            return false;
        }

        public bool ClickOk()
        {
            if (_okBtnHandle != IntPtr.Zero)
            {
                NativeApi.SendMessage(_okBtnHandle, WMMSG.WM_LBUTTONDOWN, 0, 0);
                System.Threading.Thread.Sleep(10);
                NativeApi.SendMessage(_okBtnHandle, WMMSG.WM_LBUTTONUP, 0, 0);
                return true;
            }
            return false;
        }

        public override bool IsValidWindow()
        {
            return base.Title == "选择厂商关系";
        }
    }
}
