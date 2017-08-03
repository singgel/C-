using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class SelectProperty_3C : FillDialog_3C
    {
        private IntPtr _category, _property, _okButton;

        public SelectProperty_3C(IntPtr hwnd)
            :base(hwnd)
        {
            _category = IntPtr.Zero;
            _property = IntPtr.Zero;
            _okButton = IntPtr.Zero;
        }

        public override void InitHandle()
        {
            StringBuilder title = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (handle, lParam) =>
            {
                title.Clear();
                NativeApi.GetWindowText(handle, title, 255);
                string str = title.ToString();
                if (str == "请选择车辆类别")
                    _category = handle;
                else if (str == "请选择车辆属性")
                    _property = handle;
                else if (str == "确认")
                    _okButton = handle;
                if (_category != IntPtr.Zero && _property != IntPtr.Zero && _okButton != IntPtr.Zero)
                    return false;
                return true;
            }, IntPtr.Zero);
        }

        public static SelectProperty_3C GetSelectPropertyWnd(int processId)
        {
            SelectProperty_3C current = null;
            NativeApi.EnumWindows((hwnd, lparam) =>
            {
                uint currentProcessId = 0;
                NativeApi.GetWindowThreadProcessId(hwnd, out currentProcessId);
                if (currentProcessId == processId)
                {
                    StringBuilder title = new StringBuilder(256);
                    int len = NativeApi.GetWindowText(hwnd, title, 255);
                    if (title.ToString() == "选择属性类别")
                    {
                        current = new SelectProperty_3C(hwnd);
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);
            if (current != null)
                current.InitHandle();
            return current;
        }

        public override bool DoFillWork(object propertyAndCategory)
        {
            string[] values = propertyAndCategory as string[];
            if(values == null || values.Length != 2)
                return false;
            bool result = ApiSetter.CheckRadioButton(_category, values[0]) && ApiSetter.CheckRadioButton(_property, values[1]);
            return result && ApiSetter.ClickButton(_okButton, base.HWnd, ConfirmInfoTip, null);
        }

        private void ConfirmInfoTip(object state)
        {
            uint processId;
            NativeApi.GetWindowThreadProcessId(base.HWnd, out processId);
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.InfoTipWindow, base.HWnd, processId);
            fill.DoFillWork(null);
        }

        public override bool IsValidWindow()
        {
            return _category != IntPtr.Zero && _property != IntPtr.Zero;
        }
    }
}
