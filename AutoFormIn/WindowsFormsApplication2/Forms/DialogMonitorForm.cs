using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Assistant.WinApi;
using WebBrowserUtils;
using System.Runtime.InteropServices;

namespace Assistant.Forms
{
    /// <summary>
    /// 表示可监视当前窗口是否有打开的模态对话框的窗口类。
    /// </summary>
    [ComVisible(true)]
    public partial class DialogMonitorForm : Form, IOpenDialog
    {
        private bool _isDialogOpen;
        public const int MSGF_DIALOGBOX = 0;

        public event EventHandler DialogClosed;
        public event DialogOpenedEventHandler DialogOpened;

        public DialogMonitorForm()
        {
            InitializeComponent();
            _isDialogOpen = false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
            case WMMSG.WM_ENTERIDLE:
                if (_isDialogOpen == false)
                {
                    int wparam = m.WParam.ToInt32();
                    IntPtr handle = m.LParam;
                    if (wparam == MSGF_DIALOGBOX)
                    {
                        _isDialogOpen = true;
                        OnDialogOpened(new DialogOpenedEventArgs(handle));
                    }
                }
                break;
            default:
                if (_isDialogOpen)
                {
                    _isDialogOpen = false;
                    OnDialogClosed(EventArgs.Empty);
                }
                break;
            }
            base.WndProc(ref m);
        }

        protected virtual void OnDialogClosed(EventArgs e)
        {
            if (DialogClosed != null)
                DialogClosed(this, e);
        }

        protected virtual void OnDialogOpened(DialogOpenedEventArgs e)
        {
            if (DialogOpened != null)
                DialogOpened(this, e);
        }
    }
}
