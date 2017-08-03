using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.Runtime.InteropServices;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class ApiSetter
    {
        private const int BST_CHECKED = 0x1;

        public static bool IsEditable(IntPtr hwnd)
        {
            return IsEditable(hwnd, new IntPtr((int)NativeApi.GetWindowLong(hwnd, -8)));
        }

        public static bool IsEditable(IntPtr hwnd, IntPtr parent)
        {
            long style = NativeApi.GetWindowLong(hwnd, -16);
            if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE // 不可见控件
                || (style & WindowStyles.WS_DISABLED) == WindowStyles.WS_DISABLED // 禁用控件
                || (style & EditStyles.ES_READONLY) == EditStyles.ES_READONLY)  //只读文本框
                return false;
            else
            {
                RECT rect;
                NativeApi.SwitchToThisWindow(parent, true);
                NativeApi.SendMessage(hwnd, WMMSG.WM_SETFOCUS, 0, 0);
                NativeApi.GetWindowRect(hwnd, out rect);
                IntPtr handle = NativeApi.WindowFromPoint(new tagPoint() { x = rect.Left, y = rect.Top });
                return handle == hwnd;
            }
        }

        public static bool IsDisabled(IntPtr hwnd)
        {
            long style = NativeApi.GetWindowLong(hwnd, -16);
            return (style & WindowStyles.WS_DISABLED) == WindowStyles.WS_DISABLED;
        }

        public static bool SetText(IntPtr hwnd, string text)
        {
            IntPtr result = NativeApi.SendMessage(hwnd, WMMSG.WM_SETTEXT, 0, text);
            return result != IntPtr.Zero;
        }

        public static bool IsChecked(IntPtr checkbox)
        {
            int value = NativeApi.SendMessage(checkbox, WMMSG.BM_GETCHECK, 0, 0).ToInt32();
            value = NativeApi.SendMessage(checkbox, WMMSG.BM_GETSTATE, 0, 0).ToInt32();
            return value == BST_CHECKED;
        }

        public static void SetCheck(IntPtr checkbox, IntPtr parentWnd)
        {
            NativeApi.SwitchToThisWindow(parentWnd, true);
            NativeApi.PostMessage(checkbox, WMMSG.WM_LBUTTONDOWN, 1, 0);
            NativeApi.PostMessage(checkbox, WMMSG.WM_LBUTTONUP, 0, 0);
            NativeApi.SendMessage(checkbox, WMMSG.BM_SETCHECK, 1, 0);
        }

        public static bool SetComboBoxSelected(IntPtr parentWnd, IntPtr comboBox, string selectedStr)
        {
            IntPtr result = NativeApi.SendMessage(comboBox, WMMSG.CB_SELECTSTRING, -1, selectedStr);
            if (result.ToInt32() == -1)
                return false;
            int id = (int)NativeApi.GetWindowLong(comboBox, -12);
            result = NativeApi.SendMessage(parentWnd, WMMSG.WM_COMMAND, ((id & 0xFFFF) | 0x10000), comboBox.ToInt32());
            return result == IntPtr.Zero;
        }

        public static bool SetComboBoxSelected(IntPtr parentWnd, IntPtr comboBox, string selectString, bool contain)
        {
            if (contain == false)
                return SetComboBoxSelected(parentWnd, comboBox, selectString);
            if (string.IsNullOrEmpty(selectString))
                return false;
            StringBuilder text = new StringBuilder(256);
            int count = NativeApi.SendMessage(comboBox, WMMSG.CB_GETCOUNT, 0, 0).ToInt32();
            for (int i = 0; i < count; i++)
            {
                int len = NativeApi.SendMessage(comboBox, WMMSG.CB_GETLBTEXTLEN, i, 0).ToInt32();
                NativeApi.SendMessage(comboBox, WMMSG.CB_GETLBTEXT, i, text);
                if (text.ToString().Contains(selectString))
                {
                    return SetComboBoxSelected(parentWnd, comboBox, i);
                }
            }
            return false;
        }

        public static bool SetComboBoxSelected(IntPtr parentWnd, IntPtr comboBox, int selectedIndex)
        {
            IntPtr result = NativeApi.SendMessage(comboBox, WMMSG.CB_SETCURSEL, selectedIndex, 0);
            if (result.ToInt32() == -1)
                return false;
            int id = (int)NativeApi.GetWindowLong(comboBox, -12);
            result = NativeApi.SendMessage(parentWnd, WMMSG.WM_COMMAND, ((id & 0xFFFF) | 0x10000), comboBox.ToInt32());
            return result == IntPtr.Zero;
        }
        /// <summary>
        /// 点击按钮，并监听新窗口。
        /// </summary>
        /// <param name="button"></param>
        /// <param name="parentWnd"></param>
        /// <param name="callback">点击按钮后执行的查找新窗口回调方法。</param>
        /// <param name="value">传递给查找新窗口回调方法的值。</param>
        /// <returns></returns>
        public static bool ClickButton(IntPtr button, IntPtr parentWnd, System.Threading.WaitCallback callback, object value)
        {
            NativeApi.SwitchToThisWindow(parentWnd, true);
            NativeApi.SendMessage(button, WMMSG.WM_SETFOCUS, 0, 0);
            if (callback != null)
                System.Threading.ThreadPool.QueueUserWorkItem(callback, value);
            IntPtr result1 = NativeApi.SendMessage(button, WMMSG.BM_CLICK, 0, 0);
            //IntPtr result1 = NativeApi.SendMessage(button, WMMSG.WM_LBUTTONDOWN, MouseVirtualKeys.None, 0);
            //System.Threading.Thread.Sleep(10);
            //IntPtr result2 = NativeApi.SendMessage(button, WMMSG.WM_LBUTTONUP, MouseVirtualKeys.None, 0);
            //return result1 == IntPtr.Zero && result2 == IntPtr.Zero;
            return result1 == IntPtr.Zero;
        }

        public static bool CheckRadioButton(IntPtr _parent, string value)
        {
            StringBuilder text = new StringBuilder(256);
            bool result = NativeApi.EnumChildWindows(_parent, (handle, lParam) =>
            {
                text.Clear();
                NativeApi.GetWindowText(handle, text, 255);
                if (text.ToString() == value)
                {
                    IntPtr hwndParent = new IntPtr((int)NativeApi.GetWindowLong(_parent, -8));
                    NativeApi.SendMessage(handle, WMMSG.WM_SETFOCUS, 0, 0);
                    ApiSetter.ClickButton(handle, hwndParent, null, null);
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return result == false;
        }

        public static bool CheckCheckBox(IntPtr _parent, Main_3C owner, string note, List<string> values)
        {
            StringBuilder text = new StringBuilder(256);
            IntPtr saveButton = IntPtr.Zero;
            bool result = NativeApi.EnumChildWindows(_parent, (handle, lParam) =>
            {
                text.Clear();
                NativeApi.GetClassName(handle, text, 255);
                if (text.ToString().StartsWith(CCCFillManager.EditClassName))
                {
                    ApiSetter.SetText(handle, note);
                    return true;
                }
                else if (text.ToString().StartsWith(CCCFillManager.ButtonClassName))
                {
                    text.Clear();
                    NativeApi.GetWindowText(handle, text, 255);
                    int index = values.IndexOf(text.ToString());
                    if (index != -1)
                    {
                        IntPtr hwndParent = new IntPtr((int)NativeApi.GetWindowLong(_parent, -8));
                        NativeApi.SendMessage(handle, WMMSG.WM_SETFOCUS, 0, 0);
                        ApiSetter.ClickButton(handle, hwndParent, null, null);
                        values.RemoveAt(index);
                    }
                    else if (text.ToString() == "保存")
                    {
                        saveButton = handle;
                    }
                }
                return true;
            }, IntPtr.Zero);
            if (saveButton != IntPtr.Zero)
                owner.ClickSaveButton(saveButton);
            return true;
        }
    }
}
