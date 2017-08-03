using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Detectors;
using WebBrowserUtils.ExtendWebBrowser;
using System.Collections;
using System.IO;
using mshtml;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    class PrivateDetector : ElementDetectorBase
    {
        private string _fullName;
        private string _fileName;
        private IList _excelColumnHeader;
        private Office.Excel.ForwardExcelWriter _writer;
        private Office.Excel.ForwardWriteWorksheet sheet;

        public string FullName
        {
            get { return _fullName; }
        }

        public PrivateDetector(WebBrowser2 browser, string fileName, IList attrStringList)
            : base(browser)
        {
            string tempPath = System.IO.Path.GetTempPath();
            _fileName = fileName;
            _fullName = string.Format("{0}\\{1}", tempPath, fileName);
            FileInfo info = new FileInfo(_fullName);
            if (info.Directory.Exists == false)
                info.Directory.Create();
            _excelColumnHeader = attrStringList;
            _writer = new Office.Excel.ForwardExcelWriter(_fullName);
            _writer.Open();
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

        public void Detect(string sheetName)
        {
            sheet = _writer.CreateWorksheet(sheetName) as Office.Excel.ForwardWriteWorksheet;
            writeExcelHeader();
            base.Detect();
        }

        public override void Save()
        {
            _writer.Save();
        }

        private void writeExcelHeader()
        {
            sheet.WriteStartRow(1);
            sheet.WriteTextCell(1, _writer.AddSharedString("类别"));
            int columnIndex = 2;
            for (; columnIndex - 2 < _excelColumnHeader.Count; columnIndex++)
            {
                sheet.WriteTextCell(columnIndex, _writer.AddSharedString(string.Format("元素{0}", _excelColumnHeader[columnIndex - 2] as string)));
            }
        }

        private void writeElement(string type, System.Windows.Forms.HtmlElement element)
        {
            sheet.WriteNextRow();
            sheet.WriteTextCell(1, _writer.AddSharedString(type));
            string attr = null, attrValue = null;

            IHTMLDOMNode node = element.DomElement as IHTMLDOMNode;
            IHTMLDOMAttribute2 domAttr = null;
            int columnIndex = 2;
            for (; columnIndex - 2 < _excelColumnHeader.Count; columnIndex++)
            {
                attr = _excelColumnHeader[columnIndex - 2] as string;
                if (string.IsNullOrEmpty(attr))
                    attrValue = "";
                else
                {
                    domAttr = node.attributes.getNamedItem(attr);
                    if (domAttr != null)
                        attrValue = domAttr.value;
                }
                if (attrValue != "null" && string.IsNullOrEmpty(attrValue) == false)
                    sheet.WriteTextCell(columnIndex, _writer.AddSharedString(attrValue));
                attrValue = null;
                domAttr = null;
            }
        }

        protected override string HandleFindedText(string text)
        {
            return text;
        }

        protected override bool OnButtonElementFinded(System.Windows.Forms.HtmlElement buttonElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_BUTTON, buttonElement);
            return true;
        }

        protected override bool OnCheckBoxElementFinded(System.Windows.Forms.HtmlElement checkElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_CHECKBOX, checkElement);
            return true;
        }

        protected override bool OnFileElementFinded(System.Windows.Forms.HtmlElement fileElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_FILE, fileElement);
            return true;
        }

        protected override bool OnLinkElementFinded(System.Windows.Forms.HtmlElement linkElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_A, linkElement);
            return true;
        }

        protected override bool OnFormElementDetected(System.Windows.Forms.HtmlElement formElement)
        {
            writeElement(Matcher.TYPE_FORM, formElement);
            return true;
        }

        protected override bool OnRadioElementFinded(System.Windows.Forms.HtmlElement radioElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_RADIO, radioElement);
            return true;
        }

        protected override bool OnSubmitElementFinded(System.Windows.Forms.HtmlElement submitElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_SUBMIT, submitElement);
            return true;
        }

        protected override bool OnSelectElementFinded(System.Windows.Forms.HtmlElement selectElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_SELECT, selectElement);
            return true;
        }

        protected override bool OnTextElementFinded(System.Windows.Forms.HtmlElement textElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_TEXT, textElement);
            return true;
        }

        protected override bool OnPasswordElementFinded(System.Windows.Forms.HtmlElement passwordElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_PASSWORD, passwordElement);
            return true;
        }

        protected override bool OnTextAreaElementDetected(System.Windows.Forms.HtmlElement textAreaElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            writeElement(Matcher.TYPE_TEXTAREA, textAreaElement);
            return true;
        }
    }
}
