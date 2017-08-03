using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.ExtendWebBrowser;
using mshtml;
using System.Text.RegularExpressions;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 在国环网页中查找指定的一系列值。
    /// </summary>
    internal class GHPageSearch : Detectors.ElementDetectorBase
    {
        private bool _isMatch;
        private List<string> valueList;
        private List<KeyValuePair<FillParameterKey, object>> searchOption;
        /// <summary>
        /// 网页文本匹配。
        /// </summary>
        /// <param name="searchValues">要匹配的文本列表。</param>
        /// <param name="searchOption">文本列表的匹配选项。</param>
        /// <param name="browser">要在其中查找文本的WebBrowser对象。</param>
        public GHPageSearch(List<string> searchValues, List<KeyValuePair<FillParameterKey, object>> searchOption, WebBrowser2 browser)
            : base(browser)
        {
            valueList = searchValues;
            this.searchOption = searchOption;
        }
        /// <summary>
        /// 在网页中匹配值列表，并在匹配成功后选择其后面出现的Radio。
        /// </summary>
        /// <returns>若匹配成功返回true，否则返回false。</returns>
        public bool SearchAndSelect()
        {
            base.Detect();
            return _isMatch;
        }

        public override void Save()
        {
        }

        protected override bool OnFormElementDetected(System.Windows.Forms.HtmlElement formElement)
        {
            return true;
        }

        protected override bool OnCheckBoxElementFinded(System.Windows.Forms.HtmlElement checkElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override bool OnButtonElementFinded(System.Windows.Forms.HtmlElement buttonElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override bool OnFileElementFinded(System.Windows.Forms.HtmlElement fileElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        //返回false就意味着没有必要再往下查找了
        protected override bool OnRadioElementFinded(System.Windows.Forms.HtmlElement radioElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            if (valueList == null || valueList.Count == 0)
                return false;
            _isMatch = true;
            for (int i = valueList.Count - 1; i >= 0; i--) // 匹配网页中查找到的文本与当前查找的值列表是否完全相同。
            {
                string searchString = searchOption[i].Key.SearchString;
                string value = textNode.Pop().Value;
                if (searchString == "等于")
                {
                    if (string.IsNullOrEmpty(valueList[i]))  // 若此参数用户未填写则默认为已匹配
                        continue;
                    else if (string.Compare(valueList[i], value, true) != 0)
                        _isMatch = false;
                }
                else if (Regex.IsMatch(value, searchString) == false)
                    _isMatch = false;
            }
            // 若有任何一个值不匹配，则应继续向下查找。
            if (_isMatch)
            {
                IHTMLInputElement element = radioElement.DomElement as IHTMLInputElement;
                element.@checked = true;
                return false;
            }
            return true;
        }

        protected override bool OnSubmitElementFinded(System.Windows.Forms.HtmlElement submitElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override bool OnSelectElementFinded(System.Windows.Forms.HtmlElement selectElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override bool OnTextElementFinded(System.Windows.Forms.HtmlElement textElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override bool OnPasswordElementFinded(System.Windows.Forms.HtmlElement passwordElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override bool OnTextAreaElementDetected(System.Windows.Forms.HtmlElement textAreaElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }

        protected override string HandleFindedText(string text)
        {
            return text == null ? "" : text.Trim();
        }

        protected override bool OnLinkElementFinded(System.Windows.Forms.HtmlElement linkElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return true;
        }
    }
}
