using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using WebBrowserUtils.ExtendWebBrowser;
using mshtml;
using WebBrowserUtils.HtmlUtils.Detectors;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using Assistant.DataProviders;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public abstract class FillBase : IDisposable
    {
        private Uri _currentUri;
        private FillState _state;
        private HtmlDocument _doc;
        private Thread _fillThread;
        private FillAsyncHandler _asyncObject;
        private WebBrowser2 _browser;
        private Exception _exception;
        protected string _dataFile, _currentFillValue;

        public event EventHandler Disposed;

        public WebBrowser2 Browser
        {
            get { return _browser; }
            set { _browser = value; }
        }
        /// <summary>
        /// 获取当前多值填报的索引。
        /// </summary>
        public int CurrentIndex
        {
            get;
            internal set;
        }
        /// <summary>
        /// 当前正在填报的参数值。
        /// </summary>
        public string CurrentFillValue
        {
            get { return _currentFillValue; }
        }
        /// <summary>
        /// 获取在未知次数的多级填报时是否继续下一次填报。
        /// </summary>
        public bool Continue
        {
            get;
            internal protected set;
        }

        internal WebValueConverter Converter
        {
            get;
            set;
        }

        public Assistant.DataProviders.IDataProvider DataProvider
        {
            get;
            set;
        }

        public Uri CurrentUrl
        {
            get { return _currentUri; }
        }

        public string DataFile
        {
            get { return _dataFile; }
            internal set
            {
                if (_dataFile != value)
                {
                    _dataFile = value;
                    if (_dataFile != null)
                    {
                        if (File.Exists(_dataFile))
                        {
                            DataFilePath = Directory.GetParent(_dataFile).FullName;
                        }
                    }
                }
            }
        }

        public string DataFilePath
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取当前填报的参数值表。
        /// </summary>
        public Hashtable DataTable
        {
            get;
            internal set;
        }
        /// <summary>
        /// 获取CurrentUrl所对应的HTML文档。
        /// </summary>
        public HtmlDocument Document
        {
            get { return _doc; }
        }

        public Exception Exception
        {
            get { return _exception; }
        }

        internal IList<FillRecord> FillRecords
        {
            get;
            set;
        }
        /// <summary>
        /// 获取或设置多级填报的填报列表。
        /// </summary>
        protected internal List<FillBase> FillIndexes
        {
            get;
            internal set;
        }
        /// <summary>
        /// 获取当前填报状态。
        /// </summary>
        public FillState FillState
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    OnFillStateChanged(EventArgs.Empty);
                }
            }
        }

        public string FillVersion
        {
            get;
            internal set;
        }
        /// <summary>
        /// 获取或设置规则文件所在路径。
        /// </summary>
        internal string RulePath
        {
            get;
            set;
        }

        public event EventHandler FillStateChanged;

        protected FillBase(WebBrowser2 browser, Uri currentUri)
        {
            _browser = browser;
            _currentUri = currentUri;
            _asyncObject = new FillAsyncHandler();
            _fillThread = new Thread(FillWorker);
            _state = Fillers.FillState.New;
            _browser.ScriptErrorsSuppressed = true;
        }
        /// <summary>
        /// 开始异步填写。
        /// </summary>
        public void BeginFill()
        {
            if (_state != Fillers.FillState.New || _fillThread.ThreadState == ThreadState.Running)
                throw new ArgumentException("当前填报状态不允许执行此操作！");
            _fillThread.Start();
        }
        /// <summary>
        /// 在派生类中实现具体的填写方法。
        /// </summary>
        protected abstract void BeginFillProtected();
        /// <summary>
        /// 释放当前对象持有的资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _asyncObject.Dispose();
                if (_fillThread.ThreadState == ThreadState.Suspended)
                {
                    _fillThread.Resume();
                    _fillThread.Abort();
                }
            }
            OnDisposed(EventArgs.Empty);
        }
        /// <summary>
        /// 结束异步填报（若当前填报正在进行，它将被终止）。
        /// </summary>
        public void EndFill()
        {
            if (_fillThread.IsAlive)
            {
                if (_state == FillState.Suspended)
                    this.Resume();
                if (_fillThread.ThreadState == ThreadState.Suspended)
                    _fillThread.Resume();
                if (_state != Fillers.FillState.Exception && _state != Fillers.FillState.End)
                    FillState = FillState.Abort;
                System.Diagnostics.Trace.WriteLine(string.Format("结束填报：{0}", _currentUri == null ? "" : _currentUri.OriginalString));
                _fillThread.Abort();
            }
        }
        /// <summary>
        /// 填报checkbox数据。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="checkBoxGroup"></param>
        protected void FillCheckBoxGroup(string value, string parameterName, Hashtable checkBoxGroup)
        {
            string[] values = value == null ? null : value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (values == null)
                return;
            for (int i = 0; i < values.Length; i++)
            {
                FillParameter dElement = checkBoxGroup[values[i]] as FillParameter;
                if (dElement == null)
                {
                    FillRecords.Add(new FillRecord(ElementType.CheckBox, RecordType.Failed, string.Format("复选框中不包含项 {0}", values[i]), parameterName));
                    continue;
                }
                IHTMLInputElement element = GetElement(dElement, this.GetFormIndex(dElement)) as IHTMLInputElement;
                if (element == null)
                {
                    FillRecords.Add(new FillRecord(ElementType.CheckBox, RecordType.Failed, string.Format("未找复选框元素 {0}", values[i]), parameterName));
                    continue;
                }
                ((IHTMLElement)element).click();
            }
        }

        protected virtual void FillTextElement(IHTMLElement element, string value)
        {
            switch (element.tagName)
            {
                case "INPUT":
                    IHTMLInputElement textElement = element as IHTMLInputElement;
                    if (string.IsNullOrEmpty(textElement.value))
                    {
                        textElement.value = value;
                        this.InvokeOnChange(element);
                        this.FillRecords[(int)ElementType.Text].RecordCount++;
                    }
                    break;
                case "TEXTAREA":
                    IHTMLTextAreaElement textAreaElement = element as IHTMLTextAreaElement;
                    if (string.IsNullOrEmpty(textAreaElement.value))
                    {
                        textAreaElement.value = value;
                        this.InvokeOnChange(element);
                        this.FillRecords[(int)ElementType.Text].RecordCount++;
                    }
                    break;
            }
        }
        //填充参数
        protected virtual void FillElement(FillParameter parameter, string value)
        {
            if (parameter == null)
                return;
            IHTMLElement element = null;
            if (parameter.Type != Matcher.TYPE_FORM)
            {
                //查找元素，如果是null就是代表没有此元素
                element = GetElement(parameter, GetFormIndex(parameter));
                if (element == null)
                {
                    this.FillRecords.Add(new FillRecord(GetElementType(parameter.Type), RecordType.Failed, "未找到此元素", parameter.ParameterName));
                    return;
                }
                if (parameter.Type == Matcher.TYPE_FILE)
                {
                    if (string.IsNullOrEmpty(value))
                        return;
                    value = File.Exists(value) ? value : (string.IsNullOrEmpty(DataFilePath) ? value : string.Format("{0}\\{1}", DataFilePath, value));
                    if (System.IO.File.Exists(value) == false)
                    {
                        this.FillRecords.Add(new FillRecord(ElementType.File, RecordType.Failed, string.Format("文件{0}不存在", value), parameter.ParameterName));
                        return;
                    }
                }
                ((IHTMLElement2)element).focus();
            }
            if (string.IsNullOrEmpty(parameter.SplitExpr) == false && string.IsNullOrEmpty(value) == false)
            {
                Match match = Regex.Match(value, parameter.SplitExpr, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                if (match.Success)
                    value = match.Groups["value"].Value;
            }
            _currentFillValue = value;
            switch (parameter.Type)
            {
            case Matcher.TYPE_FORM:
                UpdateFormIndex(parameter);
                break;
            case Matcher.TYPE_RADIO:
                IHTMLInputElement radioElement = element as IHTMLInputElement;
                radioElement.@checked = true;
                this.FillRecords[(int)ElementType.Radio].RecordCount++;
                break;
            case Matcher.TYPE_SELECT:
                IHTMLSelectElement selectElement = element as IHTMLSelectElement;
                if (FillSelectElement(selectElement, parameter, value))
                    this.FillRecords[(int)ElementType.Select].RecordCount++;
                else
                    this.FillRecords.Add(new FillRecord(ElementType.Select, RecordType.Failed, string.Format("下拉框中不包含选项 {0}", value), parameter.ParameterName));
                break;
            case Matcher.TYPE_A:
                element.click();
                break;
            case Matcher.TYPE_FILE:
                element.click();
                this.FillRecords[(int)ElementType.File].RecordCount++;
                break;
            case Matcher.TYPE_SUBMIT:
            case Matcher.TYPE_BUTTON:
            case "BUTTON/SUBMIT":
                IHTMLInputElement fileElement = element as IHTMLInputElement;
                element.click();
                break;
            case Matcher.TYPE_TEXTAREA:
            case Matcher.TYPE_TEXT:
            case Matcher.TYPE_PASSWORD:
                FillTextElement(element, value);
                break;
            }
        }

        protected void FillElement(FillParameterKey parameterKey, object elementContainer, string parameterValue)
        {
            this.Wait();
            FillParameter fillParameter = elementContainer as FillParameter;
            if (fillParameter == null)
            {
                Hashtable table = elementContainer as Hashtable;
                if (table == null)
                    return;
                else if (parameterKey.Type == Matcher.TYPE_RADIO)
                {
                    // 处理radio类型的选择
                    if (parameterValue != null)
                    {
                        if (table.ContainsKey(parameterValue) == false)
                            this.FillRecords.Add(new FillRecord(ElementType.Radio, RecordType.Failed, string.Format("单选框中不包含项 {0}", parameterValue), parameterKey.Key));

                        fillParameter = table[parameterValue] as FillParameter;
                    }
                }
                else if (parameterKey.Type == Matcher.TYPE_CHECKBOX) // 处理checkBox类型
                {
                    FillCheckBoxGroup(parameterValue, parameterKey.Key, table);
                    return;
                }
            }
            FillElement(fillParameter, parameterValue);
        }

        protected bool FillSelectElement(IHTMLSelectElement element, FillParameter parameter, string value)
        {
            if (parameter.CanContain && value != null)
            {
                for (int i = 0; i < element.length; i++)
                {
                    IHTMLOptionElement option = element.item(i);
                    if (option.text != null && option.text.Contains(value))
                    {
                        //element.selectedIndex = i;
                        element.options[i].selected = true;
                        this.InvokeOnChange(element as IHTMLElement);
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < element.length; i++)
                {
                    IHTMLOptionElement option = element.item(i);
                    if (option.text == value)
                    {
                        //element.selectedIndex = i;
                        element.options[i].selected = true;
                        this.InvokeOnChange(element as IHTMLElement);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 填报线程的工作代码。
        /// </summary>
        private void FillWorker()
        {
            try
            {
                FillState = FillState.Running;
                this.UpdateDocument();
                BeginFillProtected();
                FillState = FillState.End;
                EndFill();
            }
            catch (Exception ex)
            {
                if (_state != Fillers.FillState.End && _state != Fillers.FillState.Abort)
                {
                    _exception = ex;
                    FillState = FillState.Exception;
                }
            }
        }

        protected virtual IHTMLElement FindElement(HtmlDocument doc, FillParameter parameter, bool isContain, string elementType, int formIndex)
        {
            if (string.IsNullOrEmpty(parameter.href) == false)
                return InvokeScriptSync(doc, "findElementByHref", new object[] { parameter.href }) as IHTMLElement;
            return InvokeScriptSync(doc, "findElement", new object[] { elementType, isContain, parameter.Id, parameter.Name, parameter.Value, parameter.OnClick, parameter.href, formIndex }) as IHTMLElement;

        }
        /// <summary>
        /// 使用指定的form名称查找form元素。
        /// </summary>
        /// <param formName="ele">form名称。</param>
        /// <param name="ownerFormIndex">form在当前document中的索引。</param>
        /// <returns></returns>
        public IHTMLElement FindForm(string frameId, string formName)
        {
            HtmlDocument document = null;
            if (string.IsNullOrEmpty(frameId))
                document = _doc;
            else
            {
                _browser.Invoke((Action)(() =>
                {
                    HtmlWindow window = _doc.Window.Frames[frameId];
                    document = window == null ? null : window.Document;
                }));
            }
            if (document == null)
                return null;
            InstallScript(document);
            return InvokeScriptSync(document, "findForm", new object[] { formName }) as IHTMLElement;
        }
        /// <summary>
        /// 从指定html文档中查找Url为CurrentUrl的html文档。
        /// </summary>
        /// <param name="doc">要查找的html文档。</param>
        /// <returns></returns>
        private HtmlDocument FindDocument(HtmlDocument doc)
        {
            if (doc == null)
                return null;
            if (doc.Url.AbsolutePath == _currentUri.AbsolutePath)
                return doc;
            HtmlDocument finded = null;
            foreach (HtmlWindow item in doc.Window.Frames)
            {
                finded = FindDocument(item.Document);
                if (finded != null)
                    return finded;
            }
            return null;
        }
        /// <summary>
        /// 使用指定的填报参数及所在form索引获得页面元素。
        /// </summary>
        /// <param name="ele">填报参数。</param>
        /// <param name="ownerFormIndex">form在当前document中的索引。</param>
        /// <returns></returns>
        public IHTMLElement GetElement(FillParameter ele, int ownerFormIndex)
        {
            HtmlDocument document = null;
            if (string.IsNullOrEmpty(ele.FrameId))
                document = _doc;
            else
            {
                _browser.Invoke((Action)(() =>
                {
                    HtmlElement element = _doc.Window.WindowFrameElement;
                    HtmlWindow window = null;
                    if(element != null && element.Id  == ele.FrameId)
                        window = _doc.Window;
                    else
                        window = _doc.Window.Frames[ele.FrameId];
                    document = window == null ? null : window.Document;
                }));
            }
            if (document == null)
                return null;
            InstallScript(document);
            string elementType = "";
            bool isContain = false;
            switch (ele.Type)
            {
            case Matcher.TYPE_BUTTON:
            case Matcher.TYPE_SUBMIT:
            case "BUTTON/SUBMIT":
                isContain = true;
                elementType = "input";
                break;
            case Matcher.TYPE_CHECKBOX:
            case Matcher.TYPE_TEXT:
            case Matcher.TYPE_RADIO:
            case Matcher.TYPE_FILE:
            case Matcher.TYPE_PASSWORD:
                elementType = "input";
                break;
            default:
                elementType = ele.Type == null ? "" : ele.Type.ToLower();
                break;
            }
            return FindElement(document, ele, isContain, elementType, ownerFormIndex);
        }

        protected ElementType GetElementType(string nodeType)
        {
            switch (nodeType)
            {
            case Matcher.TYPE_RADIO:
                return ElementType.Radio;
            case Matcher.TYPE_SELECT:
                return ElementType.Select;
            case Matcher.TYPE_FILE:
                return ElementType.File;
            case Matcher.TYPE_A:
            case Matcher.TYPE_SUBMIT:
            case Matcher.TYPE_BUTTON:
            case "BUTTON/SUBMIT":
                return ElementType.Button;
            case Matcher.TYPE_CHECKBOX:
                return ElementType.CheckBox;
            case Matcher.TYPE_TEXTAREA:
            case Matcher.TYPE_TEXT:
            case Matcher.TYPE_PASSWORD:
                return ElementType.Text;
            }
            return ElementType.Unknown;
        }

        protected abstract int GetFormIndex(FillParameter parameter);

        protected virtual string GetJSCode()
        {
            return
@"function findElement(elementType,isContainValue,id,name,value,onclick,href,formIndex)
{
    var form = formIndex == -1? null : (formIndex >= document.forms.length ? null : document.forms[formIndex]);
    var findCode = '';
    if(elementType != null && elementType != '' && elementType != undefined)
        findCode = elementType;
    if(id != null && id != '' && id != undefined)
        findCode += '[id=""'+id+'""]';
    if(name != null && name != '' && name != undefined)
        findCode += '[name=""'+name+'""]';
    if(value != null && value != '' && value != undefined){
        if(isContainValue)
            findCode += '[value*=""'+value+'""]';
        else
            findCode += '[value=""'+value+'""]';
    }
    if(onclick != null && onclick != '' && onclick != undefined)
        findCode += '[onclick*=""'+onclick+'""]';
    if(href != null && href != '' && href != undefined)
        findCode += '[href*=""'+href+'""]';
    var obj = $(findCode);

    if(elementType != 'form'){
        obj = obj.filter(':visible');
    }
//    if(obj.length == 0){
//        alert(id + ' ' + name + ' ' + value + ' ' + onclick + '未找到');
//    }
    var element = null;
    var len = obj.length;
    if(elementType == 'form')
        return len >= 1?obj.get(0):null;
    if(len == 1)
    {
        element = obj.get(0);
        if(element.form == null || element.form == form)
            return element;
        var formInfo;
        var i;
        for(i = 0; i < document.forms.length; i++){
            if(i > formIndex){
                break;
            }
            formInfo += (i + document.forms[i].id + document.forms[i].name + '\r\n');
        }
        //alert(formInfo);
        //alert(formIndex + element.form.name + form.name + '\r\n' + id + '  ' + name);
        return null;
    }
    for(var i = 0; i<len;i++)
    {
        element = obj.get(i);
        if(element.form == form)
            return element;
    }
    return null;
}

function findElementByHref(href)
{
    var obj = $('a[href=""' + href + '""]');
    if(obj.length >= 1)
        return obj.get(0);
    return null;
}

function canInvoke(){ return findElement != null && findElement != undefined; }

function invokeOnChange(obj){ 
    try{
        obj.fireEvent('onchange');
//        obj.onchange();
    }catch(e){
        alert('invokeonchange' + e.message + e.description);
    }
}

function invokeChange(obj){ 
    try{
        obj.change();
    }catch(e){
        //alert('invokechange' + e.message + e.description);
    }
}

function invokeChange2(id){
    if(id == null || id == ''){
        alert(id);
    }
    $('#'+id).change();
}";
        }
        /// <summary>
        /// 获取数据文件中指定参数名称的值。
        /// </summary>
        /// <returns></returns>
        public string GetValue(string parameterName)
        {
            return (parameterName == null || DataTable == null) ? null : DataTable[parameterName] as string;
        }
        /// <summary>
        /// 触发HTML中指定的Select元素的onchange事件。
        /// </summary>
        /// <param name="element">要触发onchange事件的Select元素。</param>
        public void InvokeOnChange(IHTMLElement element)
        {
            InstallScript(_doc);
            InvokeScriptSync(_doc, "invokeOnChange", new object[] { element });
        }

        public void InvokeChange2(IHTMLElement element)
        {
            InstallScript(_doc);
            InvokeScriptSync(_doc, "invokeChange2", new object[] { element.id });
        }

        protected void InstallScript(HtmlDocument document)
        {
            object result = InvokeScriptSync(document, "canInvoke", null);
            if (result == null || (bool)result == false)
            {
                InstallScriptSync(document);
            }
        }

        protected object InvokeScriptSync(HtmlDocument document, string scriptName, object[] args)
        {
            if (_browser.InvokeRequired)  // Document.InvokeScript方法必须在UI线程中执行，否则返回值永远为null。
            {
                Func<IHTMLDocument, string, object[], object> invokeDelegate = WebBrowser2.InvokeScript;
                return _browser.Invoke(invokeDelegate, document.DomDocument as IHTMLDocument, scriptName, args);
            }
            else
                return document.InvokeScript(scriptName, args);
        }

        private void InstallScriptSync(HtmlDocument document)
        {
            if (_browser.InvokeRequired)
            {
                _browser.Invoke(new Action<HtmlDocument>(InnerInstallScript), document);
            }
            else
                InnerInstallScript(document);
        }

        private void InnerInstallScript(HtmlDocument document)
        {
            WebBrowser2.InstallJQuery(document.DomDocument as IHTMLDocument);
            string code = this.GetJSCode();
            WebBrowser2.AttachScript(document.DomDocument as IHTMLDocument, code);
        }
        /// <summary>
        /// 在网页跳转后更新当前的html文档引用。
        /// </summary>
        public void UpdateDocument()
        {
            this.UpdateDocument(_currentUri);
        }
        /// <summary>
        /// 在网页跳转后更新当前的url地址及html文档引用。
        /// </summary>
        /// <param name="url">url地址。</param>
        public void UpdateDocument(Uri url)
        {
            _currentUri = url;
            this.UpdateDocumentSync();
        }

        private void UpdateDocumentSync()
        {
            if (_browser.InvokeRequired) // WebBrowser.Document对象必须在UI线程中获得，否则将会引发异常。
            {
                Browser.Invoke((Action)(() => { _doc = FindDocument(_browser.Document); }));
            }
            else
                _doc = FindDocument(_browser.Document);
        }

        protected abstract void UpdateFormIndex(FillParameter parameter);

        protected virtual void OnFillStateChanged(EventArgs e)
        {
            if (FillStateChanged != null)
                FillStateChanged(this, e);
        }
        /// <summary>
        /// 调用此方法以使调用Wait方法的线程挂起。
        /// </summary>
        public void Reset()
        {
            if (_state == Fillers.FillState.Running || _state == Fillers.FillState.New)
            {
                _asyncObject.Reset();
            }
        }
        /// <summary>
        /// 恢复一个被Wait方法或Suspend方法挂起的线程。
        /// </summary>
        public void Resume()
        {
            if (_state == Fillers.FillState.Waiting || _state == Fillers.FillState.Suspended || _state == Fillers.FillState.Running)
            {
                _asyncObject.Resume();
                FillState = FillState.Running;
            }
        }
        /// <summary>
        /// 挂起当前线程，直到Resume方法被调用。
        /// </summary>
        public void Suspend()
        {
            _asyncObject.Wait();
            FillState = FillState.Suspended;
            _asyncObject.Suspend();
        }
        /// <summary>
        /// 检测当前是否可执行填报，若当前不可填报，线程将被阻塞直到Resume方法调用。
        /// </summary>
        protected internal void Wait()
        {
            FillState = Fillers.FillState.Waiting;
            _asyncObject.Wait();
            FillState = Fillers.FillState.Running;
        }

        protected virtual void OnDisposed(EventArgs e)
        {
            if (Disposed != null)
                Disposed(this, e);
        }
    }
}
