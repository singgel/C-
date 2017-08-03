using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// InputFileName_3C,DoFillWork传入值应为FillValue3C对象。
    /// </summary>
    public class InputFileName_3C : FillDialog_3C
    {
        private IntPtr _editHandle, _okButtonHandle;

        public InputFileName_3C(IntPtr hwnd)
            :base(hwnd)
        {
        }

        public override void InitHandle()
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (handle, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(handle, className, 255);
                string str = className.ToString();
                if (str.StartsWith(CCCFillManager.EditClassName))
                    _editHandle = handle;
                else if (str.StartsWith(CCCFillManager.ButtonClassName))
                {
                    StringBuilder text = new StringBuilder(6);
                    NativeApi.GetWindowText(handle, text, 5);
                    if (text.ToString() == "确定")
                        _okButtonHandle = handle;
                }
                if (_editHandle != IntPtr.Zero && _okButtonHandle != IntPtr.Zero)
                    return false;
                return true;
            }, IntPtr.Zero);
        }

        public override bool DoFillWork(object fileName)
        {
            FillValue3C value = fileName as FillValue3C;
            if (value == null || value.Value == null || _editHandle == IntPtr.Zero || _okButtonHandle == IntPtr.Zero)
                return false;
            ApiSetter.SetText(_editHandle, value.Value);
            return ApiSetter.ClickButton(_okButtonHandle, base.HWnd, null, null);
        }

        public override bool IsValidWindow()
        {
            return base.Title == "请输入文件名";
        }
    }
}
