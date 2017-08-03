using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace WebBrowserUtils.ExtendWebBrowser
{
    public delegate void WebBrowserWindowClosingEventHandler(object sender, WebBrowserWindowClosingEventArgs e);

    public class WebBrowserWindowClosingEventArgs : CancelEventArgs
    {
        private bool _isChildWindow;

        public bool IsChildWindow
        {
            get { return _isChildWindow; }
        }

        public WebBrowserWindowClosingEventArgs(bool isChildWindow, bool cancel)
            : base(cancel)
        {
            _isChildWindow = isChildWindow;
        }
    }
}
