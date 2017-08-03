using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;

using WebBrowserUtils;
using WebBrowserUtils.HtmlUtils.Setters;
using WebBrowserUtils.HtmlUtils.Actions;

namespace Assistant.Actions
{
    class BaseAction
    {
        public BaseAction(WebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;
            this.htmlAppUtil = new HtmlAppUtil(webBrowser);
        }

        private WebBrowser webBrowser = null;

        private HtmlAppUtil htmlAppUtil = null;


        public void setWebBrowser(WebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;
            this.htmlAppUtil = new HtmlAppUtil(webBrowser);
        }

        public WebBrowser getWebBrowser()
        {
            return this.webBrowser;
        }

        public HtmlAppUtil getHtmlAppUtil()
        {
            return this.htmlAppUtil;
        }

        //public abstract bool fillNextValue();

        //public abstract string getNextValue();
    }
}
