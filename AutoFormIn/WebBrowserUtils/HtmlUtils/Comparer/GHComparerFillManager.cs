using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    internal class GHComparerFillManager : GHFillManager
    {
        private Hashtable _detected;
        private PrivateDetector _detector;
        private static IList attrList;

        public string DetectedFileName
        {
            get { return (_detector == null ? "" : _detector.FullName); }
        }

        public GHComparerFillManager(WebBrowser2 browser, string dataFile)
            :base(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.Detect))
        {
            _detected = new Hashtable();
            base.Version = Properties.Resources.Detect;
        }

        static GHComparerFillManager()
        {
            attrList = new List<string>(new string[] { "id", "name", "value", "onclick" });
        }

        protected override void OnFillerStateChanged(FillBase fill)
        {
            if (fill.FillState == FillState.Running)
            {
                if (_detector == null)
                    _detector = new PrivateDetector(fill.Browser, FileHelper.GetFillRuleFile(this.Version, this.FillType, this.Standard, this.CarType), attrList);
                else
                    _detector.Browser = fill.Browser;
                string uri = fill.CurrentUrl.AbsolutePath;
                if (_detected.ContainsKey(uri) == false)
                {
                    fill.Reset();
                    _detected.Add(uri, "");
                    UrlParameter parameter = base.Uris[uri] as UrlParameter;
                    _detector.Detect(parameter.LabelName);
                    fill.Resume();
                }
            }
            else if (fill.FillState == FillState.Exception)
            {
                WebFillManager.ShowMessageBox(fill.Exception.Message, "错误", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            else if (fill.CurrentUrl.OriginalString == base.EndPageUri)
                OnFinished(EventArgs.Empty);
        }

        public void Reset()
        {
            _detector.Save();
            _detector.Dispose();
            _detected.Clear();
            _detector = null;
            base.EndFill();
        }
    }
}
