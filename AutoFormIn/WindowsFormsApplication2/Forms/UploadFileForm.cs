using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using mshtml;
using System.Runtime.InteropServices;
using WebBrowserUtils.ExtendWebBrowser;
using Assistant.WinApi;
using WebBrowserUtils;

namespace Assistant.Forms
{
    [ComVisible(true)]
    public partial class UploadFileForm : DialogMonitorForm
    {
        private const int edit_ID = 0x47C, button_OK_ID = 0x1;
        private string fileName;
        private IntPtr textHandle, btnHandle;
        private HtmlDocument openerDocument;

        public Uri Url
        {
            get { return webBrowser.Url; }
        }

        public UploadFileForm(Uri url, HtmlDocument openerDocument, string fileName)
        {
            InitializeComponent();
            textHandle = IntPtr.Zero;
            webBrowser.Url = url;
            this.fileName = fileName;
            this.openerDocument = openerDocument;
            webBrowser.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            webBrowser.ObjectForScripting = this;
            HtmlElementCollection collection = webBrowser.Document.GetElementsByTagName("head");
            HtmlElement head = null;
            if (collection != null && collection.Count > 0)
                head = collection[0];
            else
                head = webBrowser.Document.Body;
            HtmlElement script = webBrowser.Document.CreateElement("script");
            script.SetAttribute("type", "text/javascript");
            script.SetAttribute("text", @"window.close= function(){ window.external.OnQuit(); };
            function setOpener(obj) { window.opener = obj; }");
            head.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, script);
            IHTMLWindow2 wnd = webBrowser.Document.Window.DomWindow as IHTMLWindow2;
            if (openerDocument != null)
            {
                webBrowser.Document.Cookie = openerDocument.Cookie;
                webBrowser.Document.InvokeScript("setOpener", new object[] { openerDocument.Window.DomWindow });
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            webBrowser.Navigated -= OnNavigated;
            base.OnClosed(e);
        }

        protected override void OnDialogClosed(EventArgs e)
        {
            textHandle = IntPtr.Zero;
            btnHandle = IntPtr.Zero;
            base.OnDialogClosed(e);
        }

        protected override void OnDialogOpened(DialogOpenedEventArgs e)
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.GetClassName(e.Handle, className, 255);
            if (className.ToString() == "#32770")
            {
                SetText(e.Handle);
            }
            base.OnDialogOpened(e);
        }

        public void OnQuit()
        {
            this.Close();
        }
    }
}
