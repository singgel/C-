using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.ExtendWebBrowser;
using System.Collections;
using System.Windows.Forms;
using Assistant.DataProviders;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public abstract class WebFillManager : FillManagerBase
    {
        private WebValueConverter converter;
        private WebBrowser2 _browser;
        private Hashtable _uris;
        private List<FillRecord> _records;
        /// <summary>
        /// 存储当前已填报的Url。
        /// </summary>
        private Hashtable _fillTable;
        private Hashtable _data;
        private FillBase _currentFill;
        private string _currentFillValue;
        private List<WebBrowser2> _openedBrowser;

        public virtual string CarType
        {
            get;
            set;
        }

        public virtual string Standard
        {
            get;
            set;
        }

        public WebValueConverter Converter
        {
            get { return converter; }
        }

        public override IList<FillRecord> FillRecords
        {
            get { return _records; }
        }
        /// <summary>
        /// 获取当前执行的是何种版本的填报（正常填报、元素感知）
        /// </summary>
        public string Version
        {
            get;
            protected set;
        }
        /// <summary>
        /// 存储正在进行新增的填报线程（用来确定如何拆分多级数据）。
        /// </summary>
        private List<FillBase> _fillIndexes;
        /// <summary>
        /// 获取所有可填报的页面URL（URL使用Uri.AbsolutePath）及页面标签。
        /// </summary>
        protected internal Hashtable Uris
        {
            get { return _uris; }
        }
        /// <summary>
        /// 获取当前的主页面所在浏览器。
        /// </summary>
        public WebBrowser2 Browser
        {
            get { return _browser; }
        }
        /// <summary>
        /// 获取从数据文件中读取到的由参数名称及参数值组成的Hashtable。
        /// </summary>
        public Hashtable Data
        {
            get { return _data; }
        }
        /// <summary>
        /// 获取当前正在执行填报的线程。
        /// </summary>
        public FillBase CurrentFill
        {
            get { return _currentFill; }
            protected set
            {
                _currentFill = value;
            }
        }
        /// <summary>
        /// 在打开文件对话框时获得当前需要填入的数据。
        /// </summary>
        public string CurrentValue
        {
            get
            {
                return (_currentFill != null && string.IsNullOrEmpty(_currentFill.CurrentFillValue) == false) ?
                    _currentFill.CurrentFillValue : _currentFillValue;
            }
        }
        ///// <summary>
        ///// 获取填报规则文件所在路径。
        ///// </summary>
        //public string RuleFilePath
        //{
        //    get;
        //    private set;
        //}

        protected List<FillBase> FillIndexes
        {
            get { return _fillIndexes; }
        }

        protected Hashtable FillTable
        {
            get { return _fillTable; }
        }

        public override string FillType
        {
            get
            {
                return base.FillType;
            }
            set
            {
                if (base.FillType != value)
                {
                    StartPageUri = FileHelper.GetStartPageUri(value);
                    EndPageUri = FileHelper.GetEndPageUri(value);
                    JumpWhenPage = FileHelper.GetJumpPage(value);
                }
                base.FillType = value;
            }
        }

        protected internal virtual string StartPageUri
        {
            get;
            private set;
        }

        protected internal virtual string EndPageUri
        {
            get;
            private set;
        }

        protected internal virtual string JumpWhenPage
        {
            get;
            private set;
        }

        public event WebBrowserDocumentCompletedEventHandler EndPageFill;
         /// <summary>
        /// 
        /// </summary>
        /// <param name="browser">主页面浏览器。</param>
        /// <param name="dataFile">存储数据的文件位置。</param>
        /// <param name="ruleFilePath">规则文件的存储路径。</param>
        internal WebFillManager(WebBrowser2 browser, string dataFile, string ruleFilePath)
            :base(dataFile, ruleFilePath)
        {
            _fillTable = new Hashtable();
            _uris = new Hashtable();
            _browser = browser;
            _fillIndexes = new List<FillBase>();
            _openedBrowser = new List<WebBrowser2>();
            _records = new List<FillRecord>();
            _records.Add(new FillRecord(ElementType.Text, RecordType.Success));
            _records.Add(new FillRecord(ElementType.Select, RecordType.Success));
            _records.Add(new FillRecord(ElementType.Radio, RecordType.Success));
            _records.Add(new FillRecord(ElementType.CheckBox, RecordType.Success));
            _records.Add(new FillRecord(ElementType.File, RecordType.Success));
        }

        public override void BeginFill()
        {
            _fillTable.Clear();
            _uris = this.GetUris();
            _data = base.DataProvider.ProvideData(null) as Hashtable;
            // 获取数据转换器
            converter = this.DataProvider.GetConverter() as WebValueConverter;
            // 从配置文件中获取是否进行数据转换
            if (FileHelper.GetIsUseConverter(this.Version, this.FillType))
            {
                converter.DataFilePath = string.IsNullOrEmpty(base.DataProvider.DataSourceFile) ? "" : System.IO.Directory.GetParent(base.DataProvider.DataSourceFile).FullName;
                string converterFile = FileHelper.GetConverterFile(this.FillType);
                _data = converter.Convert(converterFile, this.FillType, _data);
            }
            _browser.Invoke((Action)(() =>
            {
                // 监听浏览器事件，确定填报线程的执行顺序。
                _browser.Navigating += Browser_Navigating;
                _browser.DocumentCompleted += Browser_DocumentCompleted;
                _browser.NewWindow3 += Browser_NewWindow3;
                _openedBrowser.Add(_browser);

                if (_browser.IsBusy == false && _browser.Document != null)
                {
                    HtmlDocument doc = FindFillDocument(_browser.Document);
                    if (doc == null || CanFill(doc.Url) == false)
                        return;
                    _currentFill = CreateFillInner(doc.Url, _browser);
                    _currentFill.FillIndexes = _fillIndexes;
                    if (_currentFill != null)
                        _currentFill.BeginFill();
                }
            }));
        }

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser2 browser = sender as WebBrowser2;
            // 查询当前Url是否已有填报线程。
            FillBase findFill = GetFiller(e.Url);
            if (findFill == null)
            {
                if (CanFill(e.Url))
                { //若当前Url可填报且线程尚未创建，则创建一个新线程。
                    FillBase fill = CreateFillInner(e.Url, browser);
                    if (fill != null)
                        _currentFill = fill;
                }
                else
                {
                    OnDocumentCompleted(browser, e);
                    return;
                }
            }
            else
            {
                // 若当前填报线程已结束，则创建一个新的线程执行填报。
                if (findFill.FillState == FillState.End || findFill.FillState == FillState.Exception || findFill.FillState == FillState.Abort)
                {
                    findFill.Dispose();
                    findFill = CreateFillInner(e.Url, browser);
                }
                _currentFill = findFill;
            }
            if (browser.IsBusy == false && _currentFill != null && _currentFill.Browser == browser && !browser.IsDisposed)
            {
                _currentFill.FillIndexes = _fillIndexes;
                if (_currentFill.FillState == FillState.Suspended || _currentFill.FillState == FillState.Waiting
                    || _currentFill.FillState == FillState.Running)
                {
                    if (CanFill(e.Url))
                        _currentFill.UpdateDocument(e.Url);
                    else
                        _currentFill.UpdateDocument();
                    _currentFill.Resume();
                }
                else if (_currentFill.FillState == FillState.New)
                    _currentFill.BeginFill();
            }
            OnDocumentCompleted(browser, e);
        }

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            WebBrowserUtils.WinApi.NativeApi.DeleteUrlCacheEntry(e.Url.OriginalString);
            if (_currentFill != null)
            {  
                // 在导航到任何网页时挂起当前的填报线程。
                if (_currentFill != null)
                    _currentFill.Reset();
                _fillIndexes = _currentFill.FillIndexes == null ? _fillIndexes : _currentFill.FillIndexes;
                // 将当前填报线程正在进行填报的值存储到CurrentValue，以便在打开文件对话框中填入。
                _currentFillValue = _currentFill.CurrentFillValue;
                if(CanFill(e.Url))
                    _currentFill = null;
            }
            OnBrowserNavigating((WebBrowser2)sender, e);
        }

        private void Browser_NewWindow3(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (_currentFill != null)
            {
                _currentFill.Reset(); // 有新的页面打开时挂起当前填报线程。
                _fillIndexes = _currentFill.FillIndexes == null ? _fillIndexes : _currentFill.FillIndexes;
                _currentFill = null;
            }
            OnBrowserNewWindow3((WebBrowser2)sender, e);
        }

        private void Browser_Disposed(object sender, EventArgs e)
        {
            WebBrowser2 browser = sender as WebBrowser2;
            if (browser != null)
                DetachWebBrowser(browser); // 浏览器被关闭时停止监听其事件,并继续主WebBrowser中的填报线程。
            _currentFill = null;
            _openedBrowser.Remove(browser);
            HtmlDocument doc = FindFillDocument(_openedBrowser[_openedBrowser.Count - 1].Document);
            if (doc != null)
            {
                FillBase findFill = GetFiller(doc.Url);
                if (findFill != null)
                    _currentFill = findFill;
            }
            this.OnBrowserDisposed(browser);
        }
        /// <summary>
        /// 查询指定的Url是否可执行填报。
        /// </summary>
        /// <param name="url">要查询的Url。</param>
        /// <returns></returns>
        public bool CanFill(Uri url)
        {
            return _uris.ContainsKey(url.AbsolutePath);
        }

        protected FillBase CreateFillInner(Uri url, WebBrowser2 browser)
        {
            string uri = url.AbsolutePath;
            if (string.IsNullOrEmpty(uri))
                return null;
            UrlParameter parameter = _uris[uri] as UrlParameter;
            string key = parameter == null ? null : parameter.LabelName;
            if (key == null)
                return null;
            FillBase fill = CreateFill(url, browser);
            if (fill != null)
            {
                if (_fillTable.Contains(key))
                    _fillTable[key] = fill;
                else
                    _fillTable.Add(key, fill);
                fill.DataTable = _data;
                fill.RulePath = RuleFilePath;
                fill.FillVersion = this.Version;
                fill.FillRecords = this._records;
                fill.DataProvider = this.DataProvider;
                fill.FillStateChanged += new EventHandler(OnFillStateChanged);
            }
            return fill;
        }

        private void OnFillStateChanged(object sender, EventArgs e)
        {
            FillBase fill = sender as FillBase;
            if (fill != null)
            {
                OnFillerStateChanged(fill);
                if (fill.FillState == FillState.End || fill.FillState == FillState.Abort || fill.FillState == FillState.Exception)
                {
                    OnEndPageFill(new WebBrowserDocumentCompletedEventArgs(fill.CurrentUrl));
                    fill.FillStateChanged -= OnFillStateChanged;
                }
            }
        }

        protected virtual void OnFillerStateChanged(FillBase fill)
        {
            if (fill.FillState == FillState.End)
            {
                Uri url = null;
                try
                {
                    url = new Uri(this.EndPageUri);
                }
                catch (Exception)
                {
                    this.EndFill();
                    WebFillManager.ShowMessageBox(string.Format("填报类型{0}的EndFillPageUri的url地址不正确！", base.FillType), "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (fill.CurrentUrl.AbsolutePath == url.AbsolutePath)
                {
                    OnFinished(EventArgs.Empty);
                    WebFillManager.ShowMessageBox("填报已完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.EndFill();
                }
            }
            else if (fill.FillState == FillState.Exception)
            {
                if (fill.Exception != null)
                    LogHelper.Write(fill.Exception);
            }
        }

        protected virtual void OnEndPageFill(WebBrowserDocumentCompletedEventArgs e)
        {
            if (EndPageFill != null)
                EndPageFill(this, e);
        }

        protected abstract FillBase CreateFill(Uri url, WebBrowser2 browser);

        protected virtual void OnDocumentCompleted(WebBrowser2 browser, WebBrowserDocumentCompletedEventArgs e)
        {
            string uri = e.Url.AbsolutePath;
            Uri url = null;
            try
            {
                url = new Uri(this.JumpWhenPage);
                if (url.AbsolutePath == uri)
                {
                    browser.Navigate(this.StartPageUri);
                }
            }
            catch
            {
                this.EndFill();
                WebFillManager.ShowMessageBox(string.Format("填报类型{0}的StartFillPageUri的url地址不正确！", base.FillType), "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        protected virtual void OnBrowserNavigating(WebBrowser2 browser, WebBrowserNavigatingEventArgs e)
        {
            
        }

        protected virtual void OnBrowserNewWindow3(WebBrowser2 browser, WebBrowserNavigatingEventArgs e)
        {
        }

        protected virtual void OnBrowserDisposed(WebBrowser2 browser)
        {
        }

        public override void EndFill()
        {
            _browser.Navigating -= Browser_Navigating;
            _browser.DocumentCompleted -= Browser_DocumentCompleted;
            _browser.NewWindow3 -= Browser_NewWindow3;
            foreach (var item in _openedBrowser)
            {
                this.DetachWebBrowser(item);
            }
            if (_currentFill != null)
            {
                _currentFill.Reset();
                _currentFill = null;
            }
            this.DataProvider.Clean();
            foreach (FillBase item in _fillTable.Values)
            {
                if (item != null)
                    item.EndFill();
            }
            _fillTable.Clear();
        }

        public FillBase GetFiller(Uri url)
        {
            string uri = url.AbsolutePath;
            if (string.IsNullOrEmpty(uri))
                return null;
            UrlParameter parameter = _uris[uri] as UrlParameter;
            string key = parameter == null ? null : parameter.LabelName;
            if (key == null)
                return null;
            return _fillTable[key] as FillBase;
        }
        /// <summary>
        /// 在派生类中实现时获得可填报页面的URL及规则所在文件及页面标签。
        /// </summary>
        protected abstract Hashtable GetUris();
        /// <summary>
        /// 获取当前正在执行的填报的下一个参数值。
        /// </summary>
        /// <returns></returns>
        public string GetFillValue(string parameterName)
        {
            if (_currentFill == null)
                return null;
            return _currentFill.GetValue(parameterName); ;
        }
        /// <summary>
        /// 监听WebBrowser的Navigating、DocumentCompleted、NewWindow3、Disposed事件。
        /// </summary>
        /// <param name="browser"></param>
        public void AttachWebBrowser(WebBrowser2 browser)
        {
            _openedBrowser.Add(browser);
            browser.Navigating += Browser_Navigating;
            browser.DocumentCompleted += Browser_DocumentCompleted;
            browser.NewWindow3 += Browser_NewWindow3;
            browser.Disposed += Browser_Disposed;
        }
        /// <summary>
        /// 停止监听指定WebBrowser的事件。
        /// </summary>
        /// <param name="browser"></param>
        public void DetachWebBrowser(WebBrowser2 browser)
        {
            if (browser != null)
            {
                browser.Navigating -= Browser_Navigating;
                browser.DocumentCompleted -= Browser_DocumentCompleted;
                browser.NewWindow3 -= Browser_NewWindow3;
                browser.Disposed -= Browser_Disposed;
            }
        }
        /// <summary>
        /// 查找指定WebBrowser中的填报线程。
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public HtmlDocument FindFillDocument(HtmlDocument doc)
        {
            if (doc == null)
                return null;
            if (CanFill(doc.Url) || _fillTable[doc.Url.AbsolutePath] != null)
                return doc;
            HtmlDocument finded = null;
            foreach (HtmlWindow item in doc.Window.Frames)
            {
                finded = FindFillDocument(item.Document);
                if (finded != null)
                    return finded;
            }
            return null;
        }
        /// <summary>
        /// 将指定工作表的下一行数据作为表头存入到散列表中。
        /// </summary>
        /// <param name="sheet">要读取得工作表。</param>
        /// <returns></returns>
        public static Hashtable GetColumnHeader(Office.Excel.ForwardReadWorksheet sheet)
        {
            Hashtable columnHeader = new Hashtable();
            if (sheet.ReadNextRow())
            {
                while (sheet.ReadNextCell(false))
                {
                    object content = sheet.GetContent();
                    columnHeader.Add(sheet.CurrentCell.ColumnIndex, content == null ? "" : content.ToString());
                }
            }
            return columnHeader;
        }

        public void FireNewWindowEvent(Uri url)
        {
            this.Browser_NewWindow3(_currentFill == null ? null : _currentFill.Browser, new WebBrowserNavigatingEventArgs(url, "null"));
        }

        private void OpenDialogInterface_DialogOpened(object sender, DialogOpenedEventArgs e)
        {
            if (_currentFill != null)
                _currentFill.Reset();
        }

        private void OpenDialogInterface_DialogClosed(object sender, EventArgs e)
        {
            if (_currentFill != null)
                _currentFill.Resume();
        }

        public static void ShowMessageBox(string text, string caption, MessageBoxButtons button, MessageBoxIcon icon)
        {
            Form form = null;
            int index = 0;
            while (index < Application.OpenForms.Count)
            {
                form = Application.OpenForms[index];
                if (form.Visible)
                    break;
                index++;
            }
            if (form != null)
            {
                form.Invoke((Action)(() =>
                {
                    MessageBox.Show(form, text, caption, button, icon);
                }));
            }
            else
                MessageBox.Show(text, caption, button, icon);
        }
    }
}
