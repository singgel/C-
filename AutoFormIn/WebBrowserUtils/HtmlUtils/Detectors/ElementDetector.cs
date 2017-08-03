using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using mshtml;
using System.Runtime.InteropServices;
using System.IO;
using WebBrowserUtils.ExtendWebBrowser;

namespace WebBrowserUtils.HtmlUtils.Detectors
{
    public abstract class ElementDetectorBase : IDisposable
    {
        private WebBrowser2 _browser;

        public WebBrowser2 Browser
        {
            get { return _browser; }
            internal set { _browser = value; }
        }
        
        protected ElementDetectorBase(WebBrowser webBrowser)
        {
            _browser = webBrowser as WebBrowser2;
            _browser.ScriptErrorsSuppressed = true;
        }

        ~ElementDetectorBase()
        {
            Dispose(false);
        }

        static ElementDetectorBase()
        {
        }
        /// <summary>
        /// 开始探测元素。
        /// </summary>
        public void Detect()
        {
            //HtmlElementCollection elements = _browser.Document.All;
            if (_browser.InvokeRequired)
            {
                Action action = findElement;
                _browser.Invoke(action);
            }
            else
                findElement();
            //findElement((_browser.Document.Body.DomElement as IHTMLElement).children);
        }

        public void DetectAndSave()
        {
            if (_browser.InvokeRequired)
            {
                Action action = findElement;
                _browser.Invoke(action);
            }
            else
                findElement();
            this.Save();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        /// <summary>
        /// 查找两个元素的共同父级距两元素的最大距离。
        /// </summary>
        /// <param name="element1">元素1。</param>
        /// <param name="element2">元素2。</param>
        /// <returns>若两元素没有共同父级则返回-1。</returns>
        public int findCommonParentMaxDistance(HtmlElement element1, HtmlElement element2)
        {
            int distance = 0;
            while (element1 != null && element2 != null)
            {
                if (element1 == element2)
                    return distance;
                else if (element1 == element2.Parent || element2.Parent == element1)
                {
                    distance++;
                    return distance;
                }
                distance++;
                element1 = element1.Parent;
                element2 = element2.Parent;
            }
            return -1;
        }
        /// <summary>
        /// 查找两个元素的共同父级距两元素的最大距离。
        /// </summary>
        /// <param name="depth">向上查找的深度。</param>
        /// <param name="element1">元素1。</param>
        /// <param name="element2">元素2。</param>
        /// <returns>若达到指定查找深度时仍未找到共同父级则返回-1。</returns>
        public int findCommonParentMaxDistance(int depth, HtmlElement element1, HtmlElement element2)
        {
            int distance = 0;
            while (element1 != null && element2 != null)
            {
                if (element1 == element2)
                    return distance;
                else if (distance >= depth)
                    break;
                else if (element1 == element2.Parent || element2.Parent == element1)
                {
                    distance++;
                    return distance;
                }
                distance++;
                element1 = element1.Parent;
                element2 = element2.Parent;
            }
            return -1;
        }

        private void findElement()
        {
            HtmlElement body = _browser.Document.Body;
            //C#网页元素索引（不包含文本节点）
            int index = 0;
            //dom元素索引（包含文本节点）
            int domIndex = 0;
            IHTMLDOMNode node = null;
            HtmlElement element = null;
            Stack<KeyValuePair<HtmlDocument, int>> frameIndex = new Stack<KeyValuePair<HtmlDocument, int>>();
            // 保存网页中找到的文本节点
            Stack<KeyValuePair<HtmlElement, string>> textNode = new Stack<KeyValuePair<HtmlElement, string>>(body.All.Count);
            // 保存网页中的form
            //Stack<HtmlElement> form = new Stack<HtmlElement>(3);
            // 保存正在遍历的元素列表
            Stack<KeyValuePair<HtmlElement, int[]>> enumList = new Stack<KeyValuePair<HtmlElement, int[]>>(body.All.Count);

            HtmlElement currentElement = body;
            IHTMLDOMChildrenCollection domCollection = ((IHTMLDOMNode)body.DomElement).childNodes;
            int currentFrameIndex = 0;
            for (; domIndex < domCollection.length || enumList.Count > 0; domIndex++, index++)
            {
                if (domCollection.length <= domIndex)
                {
                    KeyValuePair<HtmlElement, int[]> prev = enumList.Pop();
                    domCollection = ((IHTMLDOMNode)prev.Key.DomElement).childNodes;
                    currentElement = prev.Key;
                    index = prev.Value[0];
                    domIndex = prev.Value[1];
                    continue;
                }
                node = domCollection.item(domIndex);
                if (node.nodeType == 3) // 若当前是文本节点则放入textNode中
                {
                    string text = HandleFindedText(node.nodeValue);
                    if (string.IsNullOrEmpty(text) == false)
                    {
                        textNode.Push(new KeyValuePair<HtmlElement, string>(currentElement, text));
                    }
                    index--;
                    continue;
                }

                if (index >= currentElement.Children.Count)
                    continue;
                element = currentElement.Children[index];
                switch (element.TagName)
                {
                case Matcher.TYPE_FRAME:
                case Matcher.TYPE_IFRAME:
                    pushToStack(enumList, currentElement, ref index, ref domIndex);
                    KeyValuePair<HtmlDocument, int>? lastFrame = frameIndex.Count == 0 ? (KeyValuePair<HtmlDocument, int>?)null : frameIndex.Peek();
                    HtmlWindow wnd = null;
                    if (lastFrame != null)
                    {
                        int findedIndex = 0;
                        foreach (var item in frameIndex)
                        {
                            if (item.Key.Window.Frames[item.Value].Document == element.Document)
                            {
                                while (findedIndex > 0)
                                {
                                    frameIndex.Pop();
                                    findedIndex--;
                                }
                                currentFrameIndex = 0;
                                break;
                            }
                            else if (item.Key == element.Document)
                            {
                                currentFrameIndex = item.Value + 1;
                                do
                                {
                                    frameIndex.Pop();
                                    findedIndex--;
                                } while (findedIndex >= 0);
                                break;
                            }
                            else
                                currentFrameIndex = 0;
                            findedIndex++;
                        }
                    }
                    else
                            currentFrameIndex = 0;
                    frameIndex.Push(new KeyValuePair<HtmlDocument, int>(element.Document, currentFrameIndex));
                    wnd = element.Document.Window.Frames[currentFrameIndex];
                    currentElement = wnd.Document.Body;
                    domCollection = ((IHTMLDOMNode)currentElement.DomElement).childNodes;
                    break;
                case Matcher.TYPE_FORM:
                    if (OnFormElementDetected(element) == false)
                        return;
                    break;
                case Matcher.TYPE_A:
                    if (OnLinkElementFinded(element, textNode) == false)
                        return;
                    break;
                case Matcher.TYPE_SELECT:
                    if (OnSelectElementFinded(element, textNode) == false)
                        return;
                    break;
                case Matcher.TYPE_INPUT:
                    if (InputElementFinded(element, textNode) == false)
                        return;
                    break;
                 case Matcher.TYPE_TEXTAREA:
                    if (OnTextAreaElementDetected(element, textNode) == false)
                        return;
                    break;
                default:
                    if (OnOtherElementFinded(element) == false)
                        return;
                    break;
                }
                if (node.childNodes.length > 0)
                {
                    pushToStack(enumList, currentElement, ref index, ref domIndex); 
                    currentElement = element;
                    domCollection = ((IHTMLDOMNode)currentElement.DomElement).childNodes;
                }
            }
        }

        private void pushToStack(Stack<KeyValuePair<HtmlElement, int[]>> stack, HtmlElement element, ref int index, ref int domIndex)
        {
            stack.Push(new KeyValuePair<HtmlElement, int[]>(element, new int[] { index, domIndex }));
            domIndex = -1;
            index = -1;
        }

        private bool InputElementFinded(HtmlElement inputElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            string type = inputElement.GetAttribute("type");
            type = string.IsNullOrEmpty(type) ? "" : type.ToUpper();
            switch (type)
            {
            case Matcher.TYPE_BUTTON:
                return OnButtonElementFinded(inputElement, textNode);
            case Matcher.TYPE_CHECKBOX:
                return OnCheckBoxElementFinded(inputElement, textNode);
            case Matcher.TYPE_RADIO:
                return OnRadioElementFinded(inputElement, textNode);
            case Matcher.TYPE_SUBMIT:
                return OnSubmitElementFinded(inputElement, textNode);
            case Matcher.TYPE_TEXT:
                return OnTextElementFinded(inputElement, textNode);
            case Matcher.TYPE_FILE:
                return OnFileElementFinded(inputElement, textNode);
            case Matcher.TYPE_PASSWORD:
                return OnPasswordElementFinded(inputElement, textNode);
            }
            return true;
        }

        protected virtual bool OnOtherElementFinded(HtmlElement element)
        {
            return true;
        }
        public abstract void Save();
        //private void findElement(IHTMLElementCollection collection)
        //{
        //    int index = 0, frameIndex = 0;
        //    IHTMLElement element = null;
        //    Stack<IHTMLElement> form = new Stack<IHTMLElement>(3);
        //    Stack<IHTMLElement> textNode = new Stack<IHTMLElement>(collection.length);
        //    Stack<KeyValuePair<IHTMLElementCollection, int>> enumList = new Stack<KeyValuePair<IHTMLElementCollection, int>>(collection.length);
        //    IHTMLElementCollection currentCollection = collection;
        //    for (; index < currentCollection.length || enumList.Count > 0; index++)
        //    {
        //        if (currentCollection.length <= index)
        //        {
        //            KeyValuePair<IHTMLElementCollection, int> prev = enumList.Pop();
        //            currentCollection = prev.Key;
        //            index = prev.Value;
        //            continue;
        //        }
        //        element = currentCollection.item(index);
        //        switch (element.tagName)
        //        {
        //        case Matcher.TYPE_FRAME:
        //        case Matcher.TYPE_IFRAME:
        //            IHTMLWindow2 wnd = element.document.parentWindow.frames.item(ref frameIndex);
        //            enumList.Push(new KeyValuePair<IHTMLElementCollection, int>(currentCollection, index));
        //            index = -1;
        //            currentCollection = wnd.document.body.children;
        //            break;
        //        case Matcher.TYPE_SELECT:
        //            selectElementFinded(element, textNode);
        //            break;
        //        case Matcher.TYPE_INPUT:
        //            inputElementFinded(element, textNode, form);
        //            break;
        //        case Matcher.TYPE_FORM:
        //            form.Push(element);
        //            break;
        //        default:
        //            if (element.children.length != 0)
        //            {
        //                enumList.Push(new KeyValuePair<IHTMLElementCollection, int>(currentCollection, index));
        //                currentCollection = element.children;
        //                index = -1;
        //                continue;
        //            }
        //            else
        //            {
        //                string text = element.outerText;
        //                if (text != null && text.Trim(' ') != "")
        //                    textNode.Push(element);
        //            }
        //            break;
        //        }
        //    }
        //}
        //private void selectElementFinded(IHTMLElement selElement, Stack<IHTMLElement> textNode)
        //{
        //    onSelectElementFinded(selElement, textNode);
        //}

        //private void inputElementFinded(IHTMLElement inputElement, Stack<IHTMLElement> textNode, Stack<IHTMLElement> formStack)
        //{
        //    string type = inputElement.getAttribute("type");
        //    type = string.IsNullOrEmpty(type) ? "" : type.ToUpper();
        //    switch (type)
        //    {
        //    case Matcher.TYPE_CHECKBOX:
        //        onCheckBoxElementFinded(inputElement, textNode);
        //        break;
        //    case Matcher.TYPE_RADIO:
        //        onRadioElementFinded(inputElement, textNode);
        //        break;
        //    case Matcher.TYPE_SUBMIT:
        //        IHTMLElement form = null;
        //        do
        //        {
        //            if (formStack.Count > 0)
        //                form = formStack.Pop();
        //            else
        //            {
        //                form = null;
        //                break;
        //            }
        //        } while (form.document != inputElement.document);
        //        onSubmitElementFinded(inputElement, form, textNode);
        //        break;
        //    case Matcher.TYPE_TEXT:
        //        onTextElementFinded(inputElement, textNode);
        //        break;
        //    }
        //}

        //protected abstract void onTextElementFinded(IHTMLElement textElement, Stack<IHTMLElement> textNode);
        //protected abstract void onRadioElementFinded(IHTMLElement radioElement, Stack<IHTMLElement> textNode);
        //protected abstract void onCheckBoxElementFinded(IHTMLElement checkElement, Stack<IHTMLElement> textNode);
        //protected abstract void onSubmitElementFinded(IHTMLElement submitElement, IHTMLElement form, Stack<IHTMLElement> textNode);
        //protected abstract void onSelectElementFinded(IHTMLElement selectElement, Stack<IHTMLElement> textNode);
        /// <summary>
        /// 当在网页中查找到文本时调用此方法。
        /// </summary>
        /// <param name="text">查找到的文本值。</param>
        /// <returns>返回处理后的文本。</returns>
        protected abstract string HandleFindedText(string text);
        protected abstract bool OnButtonElementFinded(HtmlElement buttonElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnCheckBoxElementFinded(HtmlElement checkElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnFileElementFinded(HtmlElement fileElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnLinkElementFinded(HtmlElement linkElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        /// <summary>
        /// 探测到Form元素时调用此方法。
        /// </summary>
        /// <param name="formElement">探测到的Form元素。</param>
        /// <returns>若继续向下探测返回true，否则返回false。</returns>
        protected abstract bool OnFormElementDetected(HtmlElement formElement);
        protected abstract bool OnRadioElementFinded(HtmlElement radioElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnSubmitElementFinded(HtmlElement submitElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnSelectElementFinded(HtmlElement selectElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnTextElementFinded(HtmlElement textElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        protected abstract bool OnPasswordElementFinded(HtmlElement passwordElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
        /// <summary>
        /// 探测到TextArea元素时调用此方法。
        /// </summary>
        /// <param name="textAreaElement">探测到的TextArea元素。</param>
        /// <param name="textNode">查找到的网页中的文本内容。</param>
        /// <returns>若继续向下探测返回true，否则返回false。</returns>
        protected abstract bool OnTextAreaElementDetected(HtmlElement textAreaElement, Stack<KeyValuePair<HtmlElement, string>> textNode);
    }

    //private class HTMLDocument2Helper
    //{
    //    public static object InvokeScript(IHTMLDocument2 doc, string scriptName, object[] args)
    //    {
    //        object result = null;
    //        doc.scripts.item(
    //        NativeMethods.tagDISPPARAMS tagDISPPARAMS = new NativeMethods.tagDISPPARAMS();
    //        tagDISPPARAMS.rgvarg = IntPtr.Zero;
    //        try
    //        {
    //            doc.
    //            UnsafeNativeMethods.IDispatch dispatch = doc.GetScript() as UnsafeNativeMethods.IDispatch;
    //            if (dispatch != null)
    //            {
    //                Guid empty = Guid.Empty;
    //                string[] rgszNames = new string[]
    //        {
    //            scriptName
    //        };
    //                int[] array = new int[]
    //        {
    //            -1
    //        };
    //                int iDsOfNames = dispatch.GetIDsOfNames(ref empty, rgszNames, 1, SafeNativeMethods.GetThreadLCID(), array);
    //                if (NativeMethods.Succeeded(iDsOfNames) && array[0] != -1)
    //                {
    //                    if (args != null)
    //                    {
    //                        Array.Reverse(args);
    //                    }
    //                    tagDISPPARAMS.rgvarg = ((args == null) ? IntPtr.Zero : HtmlDocument.ArrayToVARIANTVector(args));
    //                    tagDISPPARAMS.cArgs = ((args == null) ? 0 : args.Length);
    //                    tagDISPPARAMS.rgdispidNamedArgs = IntPtr.Zero;
    //                    tagDISPPARAMS.cNamedArgs = 0;
    //                    object[] array2 = new object[1];
    //                    if (dispatch.Invoke(array[0], ref empty, SafeNativeMethods.GetThreadLCID(), 1, tagDISPPARAMS, array2, new NativeMethods.tagEXCEPINFO(), null) == 0)
    //                    {
    //                        result = array2[0];
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (ClientUtils.IsSecurityOrCriticalException(ex))
    //            {
    //                throw;
    //            }
    //        }
    //        finally
    //        {
    //            if (tagDISPPARAMS.rgvarg != IntPtr.Zero)
    //            {
    //                HtmlDocument.FreeVARIANTVector(tagDISPPARAMS.rgvarg, args.Length);
    //            }
    //        }
    //        return result;
    //    }
    //}
}
