using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    internal class BHComparerFillManager : BHFillManager
    {
        private Hashtable _detected;
        private PrivateDetector _detector;
        private static IList attrList;

        public string DetectedFileName
        {
            get { return (_detector == null ? "" : _detector.FullName); }
        }

        public BHComparerFillManager(WebBrowser2 browser, string dataFile)
            :base(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.Detect))
        {
            _detected = new Hashtable();
            base.Version = Properties.Resources.Detect;
        }

        static BHComparerFillManager()
        {
            attrList = new List<string>(new string[] { "id", "name", "value", "onclick" });
        }

        protected override void OnFillerStateChanged(FillBase fill)
        {
            if (fill.FillState == FillState.Running)
            {
                if (_detector == null)
                {
                    string fileName = FileHelper.GetFillRuleFile(Properties.Resources.Compare, this.FillType, base.Standard, base.CarType);
                    _detector = new PrivateDetector(fill.Browser, fileName, attrList);
                }
                else
                    _detector.Browser = fill.Browser;
                string uri = fill.CurrentUrl.AbsolutePath;
                if (_detected.ContainsKey(uri) == false)
                {
                    fill.Reset();
                    _detected.Add(uri, "");
                    UrlParameter parameter = base.Uris[uri] as UrlParameter;
                    if (parameter == null)
                        throw new ArgumentException(string.Format("网址{0}未找到对应的工作表名称！", fill.CurrentUrl.OriginalString));
                    _detector.Detect(parameter.LabelName);
                    fill.Resume();
                }
            }
            else if (fill.FillState == FillState.Exception)
            {
                WebFillManager.ShowMessageBox(fill.Exception.Message, "错误", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
                base.EndFill();
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
