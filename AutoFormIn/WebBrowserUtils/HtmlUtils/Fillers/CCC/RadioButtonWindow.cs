using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// RadioButtonWindow，DoFillWork传入值应为string[]。
    /// </summary>
    public class RadioButtonWindow : FillDialog_3C
    {
        private List<IntPtr> radionPanels;
        private IntPtr okButton;

        public RadioButtonWindow(IntPtr hwnd)
            : base(hwnd)
        {
            radionPanels = new List<IntPtr>();
        }

        public override void InitHandle()
        {
            IntPtr childAfter = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            StringBuilder text = new StringBuilder(256);
            do
            {
                childAfter = NativeApi.FindWindowEx(base.HWnd, childAfter, null, null);
                NativeApi.GetClassName(childAfter, className, 255);
                NativeApi.GetWindowText(childAfter, text, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.ContainerClassName))
                {
                    radionPanels.Add(childAfter);
                    if(string.IsNullOrEmpty(text.ToString()) == false)
                        radionPanels.Add(childAfter);
                }
                else if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                {
                    if (text.ToString() == "确定")
                        okButton = childAfter;
                }
            } while (childAfter != IntPtr.Zero);
        }

        public override bool DoFillWork(object state)
        {
            bool result=true;
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.Value == null || fillValue.Separators == null || fillValue.Separators.Length < 1)
                return false;
            string[] values = fillValue.Value.Split(fillValue.Separators[0]);
            if (values != null)
            {
                for (int i = 0; i < radionPanels.Count && values.Length > i; i++)
                {
                    result = result && ApiSetter.CheckRadioButton(radionPanels[i], values[i]);
                }
            }
            return result && ApiSetter.ClickButton(okButton, base.HWnd, null, null);
        }

        public override bool IsValidWindow()
        {
            return radionPanels.Count != 0;
        }
    }
}
