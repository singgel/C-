using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class ColumnWindow : FillDialog_3C
    {
        private IntPtr comboBox, listBox, multiInstall, finish;

        public ColumnWindow(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public override void InitHandle()
        {
            List<IntPtr> listBoxes = new List<IntPtr>();
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(hwnd, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.LISTBOXClassName))
                    listBoxes.Add(hwnd);
                else if (classNameStr.StartsWith(CCCFillManager.ComboBoxClassName))
                    comboBox = hwnd;
                else if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                {
                    StringBuilder text = className.Clear();
                    NativeApi.GetWindowText(hwnd, text, 255);
                    if (text.ToString() == "完成")
                        finish = hwnd;
                    else if (text.ToString() == "并装")
                        multiInstall = hwnd;
                }
                return listBoxes.Count == 0 || finish == IntPtr.Zero || multiInstall == IntPtr.Zero || comboBox == IntPtr.Zero;
            }, IntPtr.Zero);
            ControlSorter.SortControlList(listBoxes);
            listBox = listBoxes[0];
        }

        public unsafe override bool DoFillWork(object state)
        {
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.Value == null || fillValue.Separators == null || fillValue.Separators.Length < 1)
                return false;
            string[] values = fillValue.Value.Split(fillValue.Separators[0]);
            if (values == null || values.Length == 0)
                return false;
            ApiSetter.SetComboBoxSelected(base.HWnd, comboBox, values[0]);
            if (values.Length > 1)
            {
                uint msg = WMMSG.LB_SETCURSEL;
                long style = NativeApi.GetWindowLong(listBox, -16);
                if ((style & ListBoxStyles.LBS_MULTIPLESEL) == ListBoxStyles.LBS_MULTIPLESEL)
                    msg = WMMSG.LB_SETSEL;
                int count = NativeApi.SendMessage(listBox, WMMSG.LB_GETCOUNT, 0, 0).ToInt32();
                StringBuilder text = new StringBuilder(256);
                for (int i = 0; i < count; i++)
                {
                    RECT rect;
                    IntPtr result = NativeApi.SendMessage(listBox, WMMSG.LB_GETTEXT, i, text);
                    if (values.Contains(text.ToString()))
                    {
                        NativeApi.SendMessage(listBox, msg, i, 0);
                        NativeApi.SendMessage(listBox, WMMSG.LB_GETITEMRECT, i, ((IntPtr)(&rect)).ToInt32());
                        NativeApi.SendMessage(listBox, WMMSG.WM_LBUTTONDOWN, MouseVirtualKeys.MK_CONTROL, (rect.Top << 16) | (rect.Left & 0xFFFF));
                        NativeApi.SendMessage(listBox, WMMSG.WM_LBUTTONUP, MouseVirtualKeys.MK_CONTROL, (rect.Top << 16) | (rect.Left & 0xFFFF));
                    }
                }
                ApiSetter.ClickButton(multiInstall, base.HWnd, null, null);
            }
            return ApiSetter.ClickButton(finish, base.HWnd, null, null);
        }

        public override bool IsValidWindow()
        {
            return comboBox != IntPtr.Zero && listBox != IntPtr.Zero && multiInstall != IntPtr.Zero && finish != IntPtr.Zero;
        }
    }
}
