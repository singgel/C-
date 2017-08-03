using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils
{
    public interface IOpenDialog
    {
        event EventHandler DialogClosed;
        event DialogOpenedEventHandler DialogOpened;
    }
}
