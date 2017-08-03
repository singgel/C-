using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace WebBrowserUtils
{
    public delegate void FillStateChangedEventHandler(object sender, FillStateChangedEventArgs e);

    public class FillStateChangedEventArgs : EventArgs
    {
        private FillState _state;

        public FillState State
        {
            get { return _state; }
        }

        public FillStateChangedEventArgs(FillState state)
        {
            _state = state;
        }
    }
}
