using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class SaveRequireWindow : FillDialog_3C
    {
        private IntPtr _yesBtn, _noBtn,  _tips;

        public SaveRequireWindow(IntPtr parentWnd)
            : base(parentWnd)
        {
        }

        public string Message
        {
            get;
            private set;
        }

        public override bool DoFillWork(object val)
        {
            if (_yesBtn == IntPtr.Zero)
                return false;
            ApiSetter.ClickButton(_yesBtn, base.HWnd, (state) =>
            {
                SaveDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.SaveWindow, base.Owner, base.Owner.ProcessId) as SaveDialog_3C;
                if (string.IsNullOrEmpty(fill.Message) || fill.Message.Contains("保存成功") == false)
                    base.Owner.Reset();
                else
                {
                    fill.DoFillWork(null);
                    base.Owner.SelectNextNode(true);
                }
            }, null);
            base.Owner.ClickSaveButton(_yesBtn, true);
            return true;
        }

        public override void InitHandle()
        {
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                int id = NativeApi.GetDlgCtrlID(hwnd);
                if (id == 0xFFFF)
                    _tips = hwnd;
                else if (id == 0x6)
                    _yesBtn = hwnd;
                else if (id == 0x7)
                    _noBtn = hwnd;
                return _yesBtn == IntPtr.Zero || _noBtn == IntPtr.Zero || _tips == IntPtr.Zero;
            }, IntPtr.Zero);
            StringBuilder text = new StringBuilder(256);
            NativeApi.GetWindowText(_tips, text, 255);
            this.Message = text.ToString();
        }

        public override bool IsValidWindow()
        {
            return _yesBtn != IntPtr.Zero && _noBtn != IntPtr.Zero;
        }
    }
}
