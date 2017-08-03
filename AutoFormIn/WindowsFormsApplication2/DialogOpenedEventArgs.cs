using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant
{
    public delegate void DialogOpenedEventHandler(object sender, DialogOpenedEventArgs e);

    public class DialogOpenedEventArgs : EventArgs
    {
        private IntPtr _handle;

        public IntPtr Handle
        {
            get { return _handle; }
        }

        public DialogOpenedEventArgs(IntPtr handle)
        {
            _handle = handle;
        }
    }
}
