using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;

using WebBrowserUtils.HtmlUtils.Actions;
using WebBrowserUtils.HtmlUtils.Setters;

namespace WebBrowserUtils
{
    public class HtmlAppUtil
    {
        public HtmlAppUtil(WebBrowser webBrowser)
        {
            this.htmlElementActions = new HtmlElementActions(webBrowser);
            this.htmlElementValueSetter = new HtmlElementValueSetter(webBrowser);
        }

        // 属性

        private HtmlElementActions htmlElementActions = null;

        private HtmlElementValueSetter htmlElementValueSetter = null;

        // 获取动作器
        public HtmlElementActions getHtmlActor(){
            return this.htmlElementActions;
        }

        // 获取插值器
        public HtmlElementValueSetter getHtmlSetter()
        {
            return this.htmlElementValueSetter;
        }
    }
}
