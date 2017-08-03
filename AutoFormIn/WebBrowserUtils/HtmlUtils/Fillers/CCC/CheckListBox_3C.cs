using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// CheckListBox_3C，DoFillWork传入值应为string[]。
    /// </summary>
    public class CheckListBox_3C : FillDialog_3C
    {
        private IntPtr _listBox, _finishButton;

        public CheckListBox_3C(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public unsafe override bool DoFillWork(object state)
        {
            NativeApi.SendMessage(_listBox, WMMSG.WM_SETFOCUS, 0, 0);
            uint msg = WMMSG.LB_SETCURSEL;
            long style = NativeApi.GetWindowLong(_listBox, -16);
            if ((style & ListBoxStyles.LBS_MULTIPLESEL) == ListBoxStyles.LBS_MULTIPLESEL)
                msg = WMMSG.LB_SETSEL;
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.Value == null || fillValue.Separators == null || fillValue.Separators.Length < 1)
                return false;
            string[] values = fillValue.Value.Split(fillValue.Separators[0]);
            if (values == null)
                return false;
            int count = NativeApi.SendMessage(_listBox, WMMSG.LB_GETCOUNT, 0, 0).ToInt32();
            StringBuilder text = new StringBuilder(256);
            for (int i = 0; i < count; i++)
            {
                RECT rect;
                IntPtr result = NativeApi.SendMessage(_listBox, WMMSG.LB_GETTEXT, i, text);
                if (values.Contains(text.ToString()))
                {
                    IntPtr checkedIndex = NativeApi.SendMessage(_listBox, msg, i, 0);
                    if (checkedIndex.ToInt32() == -1)
                        continue;
                    //NativeApi.SendMessage(base.HWnd, WMMSG.WM_COMMAND, (_listBox.ToInt32() & 0xFFFF | 0x10000), _listBox.ToInt32());
                    NativeApi.SendMessage(_listBox, WMMSG.LB_GETITEMRECT, i, ((IntPtr)(&rect)).ToInt32());
                    if (fillValue.DoubleClick)
                    {
                        NativeApi.SendMessage(_listBox, WMMSG.WM_LBUTTONDOWN, MouseVirtualKeys.None, (rect.Top << 16) | (rect.Left & 0xFFFF));
                        NativeApi.SendMessage(_listBox, WMMSG.WM_LBUTTONUP, MouseVirtualKeys.None, (rect.Top << 16) | (rect.Left & 0xFFFF));
                    }
                    NativeApi.SendMessage(_listBox, WMMSG.WM_LBUTTONDOWN, MouseVirtualKeys.None, (rect.Top << 16) | (rect.Left & 0xFFFF));
                    NativeApi.SendMessage(_listBox, WMMSG.WM_LBUTTONUP, MouseVirtualKeys.None, (rect.Top << 16) | (rect.Left & 0xFFFF));
                }
            }
            ApiSetter.ClickButton(_finishButton, base.HWnd, null, null);
            return true;
        }

        public override void InitHandle()
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(hwnd, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.LISTBOXClassName))
                    _listBox = hwnd;
                else if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                {
                    StringBuilder text = className.Clear();
                    NativeApi.GetWindowText(hwnd, text, 255);
                    if (text.ToString() == "完成" || text.ToString() == "确定")
                        _finishButton = hwnd;
                }
                return _listBox == IntPtr.Zero || _finishButton == IntPtr.Zero;
            }, IntPtr.Zero);
        }

        public override bool IsValidWindow()
        {
            return _listBox != IntPtr.Zero && _finishButton != IntPtr.Zero;
        }
    }
}
