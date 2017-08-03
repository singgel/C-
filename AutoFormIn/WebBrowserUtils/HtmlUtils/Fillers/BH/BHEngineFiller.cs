using mshtml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    class BHEngineFiller : BHFiller
    {
        internal BHEngineFiller(ExtendWebBrowser.WebBrowser2 browser, Uri currentUri, BHUrlParameter urlParameter)
            :base(browser, currentUri, urlParameter)
        {
        }

        protected override void BeginFillProtected()
        {
            base.InstallScript(base.Document);
            this.InvokeScriptSync(base.Document, "deleteRows", null);
            base.BeginFillProtected();
        }
    }
}
