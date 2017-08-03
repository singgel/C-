using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Detectors;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using mshtml;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    class BHPageSearch : ElementDetectorBase
    {
        private int matchedIndex, totalPage, currentPage;
        private bool beginMatch;
        private bool isMatch, nextIsTotalPage;
        private List<string> valueToMatch;
        private List<KeyValuePair<FillParameterKey, object>> searchOption;
        private HtmlElement checkBox, jumpButton, inputPageNo;
        /// <summary>
        /// 网页文本匹配。
        /// </summary>
        /// <param name="valueList">要匹配的文本列表。</param>
        /// <param name="searchOption">文本列表的匹配选项。</param>
        /// <param name="browser">要在其中查找文本的WebBrowser对象。</param>
        public BHPageSearch(List<KeyValuePair<FillParameterKey, object>> searchOption, List<string> valueList, WebBrowser browser)
            :base(browser)
        {
            matchedIndex = 0;
            beginMatch = false;
            isMatch = false;
            valueToMatch = valueList;
            this.searchOption = searchOption;
            totalPage = 0;
            currentPage = 1;
        }

        public override void Save()
        {
        }

        protected override string HandleFindedText(string text)
        {
            if (totalPage == 0)
            {
                if (nextIsTotalPage)
                {
                    int.TryParse(text.Trim(), out totalPage);
                    nextIsTotalPage = false;
                }
                else
                {
                    if (text.Trim() == "总页数：")
                        nextIsTotalPage = true;
                }
            }
            return text;
        }

        protected override bool OnButtonElementFinded(System.Windows.Forms.HtmlElement buttonElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            IHTMLInputElement element = buttonElement.DomElement as IHTMLInputElement;
            if (element.value == "跳转")
                jumpButton = buttonElement;
            return !isMatch;
        }

        protected override bool OnCheckBoxElementFinded(System.Windows.Forms.HtmlElement checkElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            if (isMatch)
                return false;
            textNode.Clear();
            checkBox = checkElement;
            beginMatch = true;
            matchedIndex = 0;
            isMatch = false;
            return true;
        }

        protected override bool OnFileElementFinded(System.Windows.Forms.HtmlElement fileElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnLinkElementFinded(System.Windows.Forms.HtmlElement linkElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnFormElementDetected(System.Windows.Forms.HtmlElement formElement)
        {
            return !isMatch; ;
        }

        protected override bool OnRadioElementFinded(System.Windows.Forms.HtmlElement radioElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnSubmitElementFinded(System.Windows.Forms.HtmlElement submitElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnSelectElementFinded(System.Windows.Forms.HtmlElement selectElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnTextElementFinded(System.Windows.Forms.HtmlElement textElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            if (textElement.Id == "inputPageNo")
                inputPageNo = textElement;
            return !isMatch; ;
        }

        protected override bool OnPasswordElementFinded(System.Windows.Forms.HtmlElement passwordElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnTextAreaElementDetected(System.Windows.Forms.HtmlElement textAreaElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            return !isMatch; ;
        }

        protected override bool OnOtherElementFinded(HtmlElement element)
        {
            if (beginMatch && element.TagName == "TD")
            {
                string text = element.InnerText;
                string searchString = searchOption[matchedIndex].Key.SearchString;
                if (searchString == "等于")
                {
                    if (string.IsNullOrEmpty(valueToMatch[matchedIndex]))  // 若此参数用户未填写则默认为已匹配
                        matchedIndex++;
                    else if (string.Compare(valueToMatch[matchedIndex], text, true) != 0)
                        matchedIndex = 0;
                    else
                        matchedIndex++;
                }
                else if (Regex.IsMatch(text, searchString) == false)
                    matchedIndex = 0;
                else
                    matchedIndex++;

                if (matchedIndex == valueToMatch.Count - 1)
                {
                    IHTMLInputElement checkBoxElement = checkBox.DomElement as IHTMLInputElement;
                    if (checkBoxElement != null)
                        checkBoxElement.@checked = true;
                    isMatch = true;
                    return false;
                }
            }
            return true;
        }

        public bool SearchAndSelect()
        {
            while(true)
            {
                base.Detect();
                if (!isMatch)
                {
                    if (totalPage > currentPage && inputPageNo != null && jumpButton != null)
                    {
                        currentPage++;
                        IHTMLInputElement element = inputPageNo.DomElement as IHTMLInputElement;
                        element.value = currentPage.ToString();
                        IHTMLElement button = jumpButton.DomElement as IHTMLElement;
                        ((IHTMLElement2)button).focus();
                        button.click();
                    }
                    else
                        break;
                }
                else
                    break;
            }
            return isMatch;
        }
    }
}
