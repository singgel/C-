using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// InputFileName_3C,DoFillWork传入值应为string[]。
    /// </summary>
    public class TextEditWindow : FillDialog_3C
    {
        private IntPtr _finishButton, _container;

        public TextEditWindow(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public override void InitHandle()
        {
            IntPtr childAfter = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            do
            {
                className.Clear();
                childAfter = NativeApi.FindWindowEx(base.HWnd, childAfter, null, null);
                if (childAfter != IntPtr.Zero)
                {
                    NativeApi.GetClassName(childAfter, className, 255);
                    string classNameStr = className.ToString();
                    if (classNameStr.StartsWith(CCCFillManager.ContainerClassName))
                        _container = childAfter;
                    else if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                    {
                        StringBuilder text = className.Clear();
                        NativeApi.GetWindowText(childAfter, text, 255);
                        if (text.ToString() == "完成" || text.ToString() == "确定")
                            _finishButton = childAfter;
                    }
                }
            } while (childAfter != IntPtr.Zero);
        }

        public override bool DoFillWork(object state)
        {
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.Value == null || fillValue.Separators == null || fillValue.Separators.Length < 1)
                return false;
            string[] values = fillValue.Value.Split(fillValue.Separators[0]);
            if (values == null || values.Length == 0)
                return true;
            List<IntPtr> list = ControlSorter.SortContainer(_container);
            if (list == null)
                return false;
            StringBuilder className = new StringBuilder(256);
            for (int i = 0; i < values.Length && i < list.Count; i++)
            {
                className.Clear();
                NativeApi.GetClassName(list[i], className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.EditClassName))
                    ApiSetter.SetText(list[i], values[i]);
                else if (classNameStr.StartsWith(CCCFillManager.ComboBoxClassName))
                    ApiSetter.SetComboBoxSelected(base.HWnd, list[i], values[i]);
            }
            ApiSetter.ClickButton(_finishButton, base.HWnd, null, null);
            return true;
        }

        public override bool IsValidWindow()
        {
            return _finishButton != IntPtr.Zero && _container != IntPtr.Zero;
        }
    }
}
