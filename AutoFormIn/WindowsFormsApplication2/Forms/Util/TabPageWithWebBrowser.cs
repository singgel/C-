using System;
using System.Windows.Forms;
using DevExpress.XtraTab;
using mshtml;
using Assistant.Forms.Util;
using WebBrowserUtils.HtmlUtils.Fillers;
using WebBrowserUtils.ExtendWebBrowser;

namespace Assistant.Forms
{
    public partial class TabPageWithWebBrowser : DevExpress.XtraTab.XtraTabPage
    {
        #region Script
        private const string AttachScript = @"
// 重写window.close方法
function close(){ 
    window.external.OnQuit();
}

// 重写window.open方法
function open(url, name, features,showModal){
    return window.external.Open(url,name,features, showModal, window);
}
// 重写window.showModalDialog方法
function showModalDialog(url, name, option){
    return window.external.Open(url,name,option,true,window);
}

function getDialogResult(){ return window.returnValue; }

function confirm(str) { 
    if(str != null && str.indexOf('是否继续备案该车型') >= 0)
        return false;
    else
        return true; 
}

function alert(str) { window.external.alertMessage(str); }

function setOpener(obj) { window.opener = obj; }";
        #endregion
        private bool isModalDialog;
        private mshtml.IHTMLWindow2 opener;
        private WebFillManager _fillManager;

        public object DialogResult
        {
            get;
            private set;
        }

        public WebFillManager FillManager
        {
            get { return _fillManager; }
            internal set { _fillManager = value; }
        }

        public TabPageWithWebBrowser()
            : this(null, false)
        {
        }

        public TabPageWithWebBrowser(mshtml.IHTMLWindow2 opener)
            :this(opener, false)
        {
            
        }

        public TabPageWithWebBrowser(mshtml.IHTMLWindow2 opener, bool isModalDialog)
        {
            InitializeComponent();
            this.opener = opener;
            this.isModalDialog = isModalDialog;
            webBrowser1.Navigated += WebBrowser_Navigated;
            webBrowser1.NewWindow3 += WebBrowser_NewWindow3;
            webBrowser1.DocumentCompleted += WebBrowser_DocumentCompleted;
        }
        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (webBrowser1 != null)
            {
                webBrowser1.Navigated -= WebBrowser_Navigated;
                webBrowser1.NewWindow3 -= WebBrowser_NewWindow3;
                webBrowser1.DocumentCompleted -= WebBrowser_DocumentCompleted;
                if (isModalDialog)
                {
                     this.DialogResult = WebBrowser2.InvokeScript(webBrowser1.Document.DomDocument as IHTMLDocument, "getDialogResult");
                }
            }
            if (disposing)
            {
                if (webBrowser1 != null)
                {
                    webBrowser1.Dispose();
                    if (_fillManager != null)
                        _fillManager.DetachWebBrowser(webBrowser1);
                }
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
            _fillManager = null;
            webBrowser1 = null;
        }

        private void WebBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            WebBrowser2 browser = sender as WebBrowser2;
            browser.ObjectForScripting = new WebPageCloseHandler(browser.Parent as TabPageWithWebBrowser);
            HtmlDocument doc = FindDocument(browser.Document, e.Url);
            if (doc != null)
            {
                WebBrowser2.AttachScript(doc.DomDocument as IHTMLDocument, AttachScript, HtmlElementInsertionOrientation.AfterBegin);
                if (opener != null)
                {
                    browser.Document.Cookie = opener.document.cookie;
                    browser.Document.InvokeScript("setOpener", new object[] { opener });
                }
            }
            this.Text = string.IsNullOrEmpty(browser.Document.Title) ? "未命名" : browser.Document.Title;
        }

        private void WebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser2 browser = sender as WebBrowser2;
            MainRibbonForm form = this.FindForm() as MainRibbonForm;
            if (form != null)
            {
                if (e.Url.ToString() == Constants.APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE)
                {
                    form.simpleButton1.Visible = true;
                }
                else
                {
                    form.simpleButton1.Visible = false;
                }
            }
        }
        /// <summary>
        /// 从指定html文档中查找Url地址为url的html文档。
        /// </summary>
        /// <param name="doc">要查找的html文档。</param>
        /// <returns></returns>
        private HtmlDocument FindDocument(HtmlDocument doc, Uri url)
        {
            if (doc == null)
                return null;
            if (doc.Url == url)
                return doc;
            HtmlDocument finded = null;
            foreach (HtmlWindow item in doc.Window.Frames)
            {
                finded = FindDocument(item.Document, url);
                if (finded != null)
                    return finded;
            }
            return null;
        }
        /// <summary>
        /// 拦截弹出窗口。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_NewWindow3(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = true;
            WebBrowser browser = sender as WebBrowser;
            HtmlElement element = browser.Document.ActiveElement;
            while (element != null)
            {
                if (element.TagName == "FRAME" || element.TagName == "IFRAME")
                {
                    if (string.IsNullOrEmpty(element.Id))
                    {
                        foreach (HtmlWindow wnd in element.Document.Window.Frames)
                        {
                            if (wnd.WindowFrameElement == element)
                            {
                                element = wnd.Document.ActiveElement;
                                break;
                            }
                        }
                    }
                    else
                        element = element.Document.Window.Frames[element.Id].Document.ActiveElement;
                }
                else
                    break;
            }
            HtmlDocument opener = element == null ? null : element.Document;
            TabPageWithWebBrowser page = new TabPageWithWebBrowser(opener.Window.DomWindow as IHTMLWindow2);
            WebFillManager webFillManager = _fillManager as WebFillManager;
            if (webFillManager != null)
                webFillManager.AttachWebBrowser(page.WebBrowser1);
            page.FillManager = webFillManager;
            XtraTabControl tabControl = this.Parent as XtraTabControl;
            tabControl.TabPages.Add(page);
            tabControl.SelectedTabPage = page; //显示该页
        }
    }
}
