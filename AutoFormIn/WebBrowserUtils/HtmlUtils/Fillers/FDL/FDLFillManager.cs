using Assistant.DataProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class FDLFillManager : GHFillManager
    {
        public FDLFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile)
            : this(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.FillRule))
        {
        }

        internal FDLFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile, string ruleFilePath)
            : base(browser, dataFile, ruleFilePath)
        {
            base.FillType = "非道路机动车";
            this.Version = Properties.Resources.FillRule;
        }

        protected override FillBase CreateFill(Uri url, ExtendWebBrowser.WebBrowser2 browser)
        {
            FDLFiller filler = new FDLFiller(browser, url, Uris[url.AbsolutePath] as UrlParameter);
            filler.DefaultValue = base.CurrentValue;
            filler.DataFile = base.DataProvider.DataSourceFile;
            filler.Converter = base.Converter;
            return filler;
        }
    }
}
