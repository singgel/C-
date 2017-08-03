using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using mshtml;
using System.IO;

namespace WebBrowserUtils.HtmlUtils.Detectors
{
    public class GHElementDetector : ElementDetectorBase
    {
        private const string _typeColumnHeader = "类型";
        private const string _paraNameColumnHeader = "参数名称";
        private const string _elementTypeColumnHeader = "元素类型";
        private const string _optionColumnHeader = "Select选项";
        private IList _excelColumnHeader;
        private Office.Excel.ForwardExcelWriter _writer;
        private Office.Excel.ForwardWriteWorksheet _sheet;
        private KeyValuePair<string, List<HtmlElement>>? _group;
        private string lastRadioName = "";
        private static readonly char[] _trimChar;
        /// <summary>
        /// 初始化国环网页元素探测器。
        /// </summary>
        /// <param name="browser">要探测的网页所在浏览器。</param>
        public GHElementDetector(WebBrowser browser)
            : this(browser, new ArrayList())
        {
        }
        /// <summary>
        /// 初始化国环网页元素探测器。
        /// </summary>
        /// <param name="browser">要探测的网页所在浏览器。</param>
        /// <param name="attrStringList">需要记录的元素属性列表。</param>
        public GHElementDetector(WebBrowser browser, IList attrStringList)
            : base(browser)
        {
            _group = null;
            _excelColumnHeader = attrStringList;
            _writer = new Office.Excel.ForwardExcelWriter(Path.GetTempFileName());
            _writer.Open();
            _sheet = _writer.CreateWorksheet() as Office.Excel.ForwardWriteWorksheet;
            writeExcelHeader();
        }

        static GHElementDetector()
        {
            _trimChar = new char[] { ':', '：', ' ', '\\' };
        }

        private void writeExcelHeader()
        {
            _sheet.WriteStartRow(1);
            _sheet.WriteTextCell(1, _writer.AddSharedString(_typeColumnHeader));
            _sheet.WriteTextCell(2, _writer.AddSharedString(_paraNameColumnHeader));
            _sheet.WriteTextCell(3, _writer.AddSharedString(_elementTypeColumnHeader));
            _sheet.WriteTextCell(4, _writer.AddSharedString("frameId"));
            _sheet.WriteTextCell(5, _writer.AddSharedString("Url"));
            int columnIndex = 6;
            for (; columnIndex - 6 < _excelColumnHeader.Count; columnIndex++)
            {
                _sheet.WriteTextCell(columnIndex, _writer.AddSharedString(_excelColumnHeader[columnIndex - 6] as string));
            }
            _sheet.WriteTextCell(columnIndex, _writer.AddSharedString(_optionColumnHeader));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_writer != null)
            {
                if (disposing)
                    _writer.Close();
                else
                    _writer.Dispose();
                _writer = null;
            }
        }

        private string FindField(Stack<KeyValuePair<HtmlElement, string>> textNodes)
        {
            KeyValuePair<HtmlElement, string>? last = null, lastParent = null;
            StringBuilder text = new StringBuilder();
            if (textNodes.Count == 0)
                return null;

            while (textNodes.Count > 0)
            {
                last = textNodes.Pop();
                if (lastParent == null)
                {
                    lastParent = last.Value;
                    text.Append(last.Value.Value);
                    continue;
                }
                int maxDistance = findCommonParentMaxDistance(1, lastParent.Value.Key, last.Value.Key);
                if (maxDistance == -1 || maxDistance > 1)
                {
                    textNodes.Push(last.Value);
                    break;
                }
                text.Insert(0, last.Value.Value);
            }
            textNodes.Clear();
            return text.ToString();
        }

        //private string FindField(Stack<IHTMLElement> textNodes)
        //{
        //    IHTMLElement last = null, lastParent = null;
        //    string text = null;
        //    while (textNodes.Count > 0)
        //    {
        //        last = textNodes.Pop();
        //        if (lastParent == null)
        //            lastParent = last.parentElement;
        //        else if (last.parentElement != lastParent)
        //        {
        //            textNodes.Push(last);
        //            break;
        //        }
        //        text = last.outerText.Trim(_trimChar);
        //        if (text.StartsWith("（") || text.StartsWith("("))
        //            continue;
        //    }
        //    return text;
        //}
        private void handleRadioOrCheckBox(HtmlElement element, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            if (string.IsNullOrEmpty(lastRadioName) == false && element.Name != lastRadioName)
            {
                finishGroup();
            }
            if (_group == null)
            {
                string field = FindField(textNode);
                _group = new KeyValuePair<string, List<HtmlElement>>(field, new List<HtmlElement>());
            }
            _group.Value.Value.Add(element);
        }

        protected override string HandleFindedText(string text)
        {
            return text == null ? "" : text.Trim().Trim(_trimChar);
        }

        private void finishGroup()
        {
            if (_group != null)
            {
                writeGroup();
                _group = null;
            }
        }

        protected void writeElement(string field, string type, HtmlElement element)
        {
            writeElement("element", field, type, element);
        }

        protected void writeElement(string fieldType, string field, string type, HtmlElement element)
        {
            writeElement(fieldType, field, type, null, element);
        }

        protected void writeElement(string fieldType, string field, string type, string options, HtmlElement element)
        {
            _sheet.WriteNextRow();
            _sheet.WriteTextCell(1, _writer.AddSharedString(fieldType));
            if (string.IsNullOrEmpty(field) == false)
                _sheet.WriteTextCell(2, _writer.AddSharedString(field));
            if (string.IsNullOrEmpty(type) == false)
                _sheet.WriteTextCell(3, _writer.AddSharedString(type));
            HtmlElement frameElement = element.Document.Window.WindowFrameElement;
            string frameId = (frameElement == null || string.IsNullOrEmpty(frameElement.Id)) ? "" : frameElement.Id;
            _sheet.WriteTextCell(4, _writer.AddSharedString(frameId));
            _sheet.WriteTextCell(5, _writer.AddSharedString(element.Document.Url.OriginalString));
            string attr = null, attrValue = null;

            IHTMLDOMNode node = element.DomElement as IHTMLDOMNode;
            IHTMLDOMAttribute2 domAttr = null;
            int columnIndex = 6;
            for (; columnIndex - 6 < _excelColumnHeader.Count; columnIndex++)
            {
                attr = _excelColumnHeader[columnIndex - 6] as string;
                if (string.IsNullOrEmpty(attr))
                    attrValue = "";
                else
                {
                    domAttr = node.attributes == null ? null : node.attributes.getNamedItem(attr);
                    if (domAttr != null)
                        attrValue = domAttr.value;
                }
                if (attrValue != "null" && string.IsNullOrEmpty(attrValue) == false)
                    _sheet.WriteTextCell(columnIndex, _writer.AddSharedString(attrValue));
                attrValue = null;
                domAttr = null;
            }
            if (string.IsNullOrEmpty(options) == false)
                _sheet.WriteTextCell(columnIndex, _writer.AddSharedString(options));
        }

        private void writeGroup()
        {
            string groupName = _group.Value.Key;
            List<HtmlElement> list = _group.Value.Value;
            for (int i = 0; i < list.Count; i++)
            {
                writeElement(string.Format("group:{0}", groupName), list[i].GetAttribute("value"), list[i].GetAttribute("type").ToUpper(), list[i]);
            }
        }

        protected override bool OnButtonElementFinded(HtmlElement buttonElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            finishGroup();
            string field = buttonElement.GetAttribute("value");
            writeElement(field, Matcher.TYPE_BUTTON, buttonElement);
            return true;
        }

        protected override bool OnCheckBoxElementFinded(HtmlElement checkElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            KeyValuePair<HtmlElement, string>? clost = null;
            if (textNode.Count > 0)
                clost = textNode.Peek();
            if (clost != null && clost.Value.Value == checkElement.GetAttribute("value").Trim())
                textNode.Pop();
            handleRadioOrCheckBox(checkElement, textNode);
            return true;
        }

        protected override bool OnFileElementFinded(HtmlElement fileElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            string field = FindField(textNode);
            writeElement("element", field, Matcher.TYPE_FILE, fileElement);
            return true;
        }

        protected override bool OnFormElementDetected(HtmlElement formElement)
        {
            writeElement("form", "", Matcher.TYPE_FORM, formElement);
            return true;
        }

        protected override bool OnLinkElementFinded(HtmlElement linkElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            string field = FindField(textNode);
            writeElement("A", field, Matcher.TYPE_A, linkElement);
            return true;
        }
        
        protected override bool OnPasswordElementFinded(HtmlElement passwordElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            string field = FindField(textNode);
            writeElement(field, Matcher.TYPE_PASSWORD, passwordElement);
            return true;
        }

        protected override bool OnRadioElementFinded(HtmlElement radioElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            OnCheckBoxElementFinded(radioElement, textNode);
            return true;
        }

        protected override bool OnSelectElementFinded(HtmlElement selectElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            finishGroup();
            string field = FindField(textNode);
            StringBuilder options = new StringBuilder();
            foreach (HtmlElement option in selectElement.All)
            {
                options.Append(string.Format("{0}、", option.OuterText));
            }
            if (options.Length != 0)
                options.Remove(options.Length - 1, 1);
            writeElement("element", field, Matcher.TYPE_SELECT, options.ToString(), selectElement);
            return true;
        }

        protected override bool OnSubmitElementFinded(HtmlElement submitElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            finishGroup();
            string field = submitElement.GetAttribute("value");
            writeElement(field, Matcher.TYPE_SUBMIT, submitElement);
            return true;
        }

        protected override bool OnTextAreaElementDetected(HtmlElement textAreaElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            string field = FindField(textNode);
            writeElement(field, Matcher.TYPE_TEXTAREA, textAreaElement);
            return true;
        }

        protected override bool OnTextElementFinded(HtmlElement textElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            finishGroup();
            string field = FindField(textNode);
            writeElement(field, Matcher.TYPE_TEXT, textElement);
            return true;
        }

        //protected override void onTextElementFinded(IHTMLElement textElement, Stack<IHTMLElement> textNode)
        //{
        //    string field = FindField(textNode);
        //}

        //protected override void onRadioElementFinded(IHTMLElement radioElement, Stack<IHTMLElement> textNode)
        //{
        //    string field = radioElement.getAttribute("value");
        //}

        //protected override void onCheckBoxElementFinded(IHTMLElement checkElement, Stack<IHTMLElement> textNode)
        //{
        //    string field = checkElement.getAttribute("value");
        //}

        //protected override void onSubmitElementFinded(IHTMLElement submitElement, IHTMLElement form, Stack<IHTMLElement> textNode)
        //{
        //    string field = submitElement.getAttribute("value");
        //}

        //protected override void onSelectElementFinded(IHTMLElement selectElement, Stack<IHTMLElement> textNode)
        //{
        //    string field = FindField(textNode);
        //    if (string.IsNullOrEmpty(field))
        //        field = getFieldFromElement(selectElement);
        //}
        //private string getFieldFromElement(IHTMLElement element)
        //{
        //    IHTMLElement parent = element.parentElement;
        //    string field = "";
        //    while (parent != null)
        //    {
        //        if (parent.outerText != null)
        //        {
        //            int index = parent.outerText.IndexOf(element.outerText);
        //            if (index != -1)
        //                field = parent.outerText.Substring(0, index).Trim(_trimChar);
        //        }
        //        if (string.IsNullOrEmpty(field) == false)
        //            break;
        //        parent = parent.parentElement;
        //    }
        //    return field;
        //}
        public override void Save()
        {
            finishGroup();
            Office.Excel.ForwardExcelWriter file = _writer;
            _writer = new Office.Excel.ForwardExcelWriter(Path.GetTempFileName());
            _writer.Open();
            _sheet = _writer.CreateWorksheet() as Office.Excel.ForwardWriteWorksheet;
            writeExcelHeader();
            file.Save();
            string filename = file.FileName;
            file.Dispose();
            if (File.Exists("generated.xlsx"))
                File.Delete("generated.xlsx");
            File.Move(filename, "generated.xlsx");
        }
    }
}
