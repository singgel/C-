using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using mshtml;

namespace WebBrowserUtils.HtmlUtils
{
    public class BaseImpl
    {
        // 初始化时必须有webBrowser对象
        public BaseImpl(WebBrowser webBrowser) 
        { 
            this.webBrowser = webBrowser;
            this.htmlDocument = webBrowser.Document;
            this.htmlDocument2 = (IHTMLDocument2)this.webBrowser.Document.DomDocument;
        }

        protected WebBrowser webBrowser = null;

        protected HtmlDocument htmlDocument = null;

        protected IHTMLDocument2 htmlDocument2 = null;
    }
}
