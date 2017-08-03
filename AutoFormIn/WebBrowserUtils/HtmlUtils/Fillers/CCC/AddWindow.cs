using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 可追加窗口。
    /// </summary>
    public class AddWindow : FillDialog_3C
    {
        private int inputCount;
        private IntPtr addButton, saveButton, container, current;

        public AddWindow(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public override void InitHandle()
        {
            IntPtr childAfter = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            StringBuilder text = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                NativeApi.GetClassName(hwnd, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                {
                    NativeApi.GetWindowText(hwnd, text, 255);
                    string title = text.ToString();
                    if (title == "保存" || title == "完成")
                        saveButton = hwnd;
                    else if (title == "追加")
                        addButton = hwnd;
                    text.Clear();
                }
                else if (classNameStr.StartsWith(CCCFillManager.ComboBoxClassName) || classNameStr.StartsWith(CCCFillManager.EditClassName))
                {
                    if (ApiSetter.IsEditable(hwnd))
                        inputCount++;
                }
                className.Clear();
                return true;
            }, IntPtr.Zero);

            do
            {
                childAfter = NativeApi.FindWindowEx(base.HWnd, childAfter, null, null);
                NativeApi.GetClassName(childAfter, className, 255);
                if (className.ToString().StartsWith(CCCFillManager.ContainerClassName))
                {
                    container = childAfter;
                    current = NativeApi.FindWindowEx(container, IntPtr.Zero, null, null);
                }
            } while (childAfter != IntPtr.Zero);
        }

        public override bool DoFillWork(object state)
        {
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.Value == null || fillValue.Separators == null || fillValue.Separators.Length < 2 || inputCount == 0)
                return false;

            foreach (char c in fillValue.Separators)
            {
                fillValue.Value = fillValue.Value.Replace(c, ',');
            }
            List<string[]> values = new List<string[]>();
            string[] array = fillValue.Value.Split(new char[] { ',' }, StringSplitOptions.None);
            string[] group = null;
            for (int i = 0; i < array.Length; i++)
            {
                int mod = (i % inputCount);
                if (mod == 0)
                {
                    group = new string[inputCount];
                    values.Add(group);
                }
                group[mod] = array[i];
            }
            //List<string[]> values = new List<string[]>();
            //foreach (var item in fillValue.Value.Split(fillValue.Separators[1]))
            //{
            //    values.Add(item.Split(fillValue.Separators[0]));
            //}
            bool result = true;
            StringBuilder className = new StringBuilder(256);
            for (int index = 0; index < values.Count && current != IntPtr.Zero; index++)
            {
                List<IntPtr> sorted = ControlSorter.SortContainer(current);
                if (sorted != null)
                {
                    string[] parametrValues = values[index];
                    for (int i = 0; i < sorted.Count && parametrValues.Length > i; i++)
                    {
                        NativeApi.GetClassName(sorted[i], className, 255);
                        string classNameStr = className.ToString();
                        if (classNameStr.StartsWith(CCCFillManager.EditClassName))
                            ApiSetter.SetText(sorted[i], parametrValues[i]);
                        else if (classNameStr.StartsWith(CCCFillManager.ComboBoxClassName))
                            ApiSetter.SetComboBoxSelected(base.HWnd, sorted[i], parametrValues[i]);
                    }
                }
                if (index < values.Count - 1)
                    ApiSetter.ClickButton(addButton, base.HWnd, null, null);
                current = NativeApi.FindWindowEx(container, current, null, null);
            }
            return result && ApiSetter.ClickButton(saveButton, base.HWnd, null, null);
        }

        public override bool IsValidWindow()
        {
            return saveButton != IntPtr.Zero && addButton != IntPtr.Zero && container != IntPtr.Zero && current != IntPtr.Zero;
        }
    }
}
