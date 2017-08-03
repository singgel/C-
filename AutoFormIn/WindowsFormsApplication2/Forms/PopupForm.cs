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

namespace Assistant.Forms
{
    [ComVisible(true)]
    public partial class PopupForm : Form
    {
        private HtmlDocument openerDocument;

        public Uri Url
        {
            get { return webBrowser.Url; }
        }

        public PopupForm(Uri url, HtmlDocument openerDocument)
        {
            InitializeComponent();
            webBrowser.Url = url;
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

        public void OnQuit()
        {
            webBrowser.Navigated -= OnNavigated;
            this.Close();
        }
    }
}
