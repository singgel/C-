using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 多个值和备注的可追加窗口。
    /// </summary>
    public class AddValueAndNote : AddWindow
    {
        public AddValueAndNote(IntPtr hwnd)
            : base(hwnd)
        {
        }

        public override bool DoFillWork(object state)
        {
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.Value == null || fillValue.Separators == null || fillValue.Separators.Length < 2)
                return false;

            FillValue3C value = new FillValue3C();
            char[] separators = fillValue.Separators;
            value.Separators = separators;
            StringBuilder text = new StringBuilder();
            foreach(var item in fillValue.OriginString.Split(separators[1]))
            {
                value.SetValue(item);
                text.Append(string.Format("{0}{1}{2}", value.Value, separators[0], value.Note));
                text.Append(separators[1]);
            }
            if (text.Length != 0)
                text.Remove(text.Length - 1, 1);
            value.SetValue(text.ToString());
            return base.DoFillWork(value);
        }
    }
}
