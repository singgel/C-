using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.XtraTab;
using System.Windows.Forms;
using Assistant.Service;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace Assistant.Forms.Util
{
    [ComVisible(true)]
    public class WebPageCloseHandler
    {
        private TabPageWithWebBrowser _webPage;

        public WebPageCloseHandler(TabPageWithWebBrowser webPage)
        {
            _webPage = webPage;
        }

        public void OnQuit()
        {
            if (_webPage != null)
            {
                XtraTabControl tabControl = _webPage.Parent as XtraTabControl;
                if (tabControl != null)
                {
                    int index = tabControl.TabPages.IndexOf(_webPage);
                    if(index != -1)
                        tabControl.TabPages.RemoveAt(index);
                    tabControl.SelectedTabPageIndex = tabControl.TabPages.Count - 1;
                }
                _webPage.Dispose();
            }
        }

        public object Open(string url, string windowName, string feature, bool isModalWindow, mshtml.IHTMLWindow2 window)
        {
            XtraTabControl tabControl = _webPage.Parent as XtraTabControl;
            TabPageWithWebBrowser lastTab = tabControl.SelectedTabPage as TabPageWithWebBrowser;
            TabPageWithWebBrowser tabPage = new TabPageWithWebBrowser(window, isModalWindow);
            Uri uri = new Uri(new Uri(window.location.href), url);
            if (tabControl != null)
            {
                tabControl.TabPages.Add(tabPage);
                tabPage.FillManager = lastTab == null ? null : lastTab.FillManager;
                tabPage.WebBrowser1.Navigate(uri);
                tabControl.SelectedTabPage = tabPage;
            }
            if (tabPage.FillManager != null)
            {
                tabPage.FillManager.AttachWebBrowser(tabPage.WebBrowser1);
            }
            if (isModalWindow)  
            {
                // 模拟打开模态窗口
                object result = null;
                while (tabPage.WebBrowser1 != null && tabPage.WebBrowser1.IsDisposed == false)
                {
                    Application.DoEvents();
                }
                result = tabPage.DialogResult;
                return result;
            }
            else
            {
                if (tabPage.FillManager != null)
                    tabPage.FillManager.FireNewWindowEvent(uri);
                while (tabPage.WebBrowser1.Document == null)
                {
                    Application.DoEvents();
                }
                return tabPage.WebBrowser1.Document.Window.DomWindow;
            }
        }

        public void Suspend()
        {
            if (_webPage.FillManager != null)
            {
                if (_webPage.FillManager.CurrentFill != null)
                    _webPage.FillManager.CurrentFill.Reset();
            }
        }

        public void Resume()
        {
            if (_webPage.FillManager != null)
            {
                if (_webPage.FillManager.CurrentFill != null)
                    _webPage.FillManager.CurrentFill.Resume();
            }
        }

        public void alertMessage(String str)
        {
            if (str != null && str.Contains("提交成功"))
            {
                ParamsCollection.runningCarParams.submitConfigCode = true;
                return;
            }
            MessageBox.Show(str);
            if(_webPage.FillManager != null)
                _webPage.FillManager.FillRecords.Add(new FillRecord(ElementType.Unknown, RecordType.DialogMessage, str, null));
            if (str == null)
                return;
            String configCode = DataTransformServiceForSHFY.getConfigCodeFromTip(str);
            if (String.IsNullOrEmpty(configCode))
            {
            }
            else
            {
                ParamsCollection.runningCarParams.ConfigCode = configCode;
            }
        }
    }
}
