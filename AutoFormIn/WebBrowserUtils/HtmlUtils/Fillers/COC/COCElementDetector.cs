using mshtml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebBrowserUtils.HtmlUtils.Detectors;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class COCElementDetector:GHElementDetector
    {
        public COCElementDetector(WebBrowser browser)
            : base(browser)
        {
        }

        public COCElementDetector(WebBrowser browser, IList attrCollection)
            : base(browser, attrCollection)
        {
        }

        protected override bool OnRadioElementFinded(System.Windows.Forms.HtmlElement radioElement, Stack<KeyValuePair<System.Windows.Forms.HtmlElement, string>> textNode)
        {
            IHTMLDOMNode node = radioElement.DomElement as IHTMLDOMNode;
            IHTMLDOMAttribute2 attr = node.attributes == null ? null : node.attributes.getNamedItem("title");
            if (attr != null && attr.value != "null" && string.IsNullOrEmpty(attr.value) == false)
            {
                textNode.Push(new KeyValuePair<HtmlElement, string>(radioElement, attr.value));
            }
            return base.OnRadioElementFinded(radioElement, textNode);
        }

        protected override bool OnCheckBoxElementFinded(HtmlElement checkElement, Stack<KeyValuePair<HtmlElement, string>> textNode)
        {
            IHTMLDOMNode node = checkElement.DomElement as IHTMLDOMNode;
            IHTMLDOMAttribute2 attr = node.attributes == null ? null : node.attributes.getNamedItem("title");
            if (attr != null && attr.value != "null" && string.IsNullOrEmpty(attr.value) == false)
            {
                textNode.Push(new KeyValuePair<HtmlElement, string>(checkElement, attr.value));
            }
            return base.OnCheckBoxElementFinded(checkElement, textNode);
        }

        protected override bool OnOtherElementFinded(HtmlElement element)
        {
            IHTMLDOMNode node = element.DomElement as IHTMLDOMNode;
            IHTMLDOMAttribute2 attr = node.attributes == null ? null : node.attributes.getNamedItem("ondblclick");
            if (attr != null && attr.value != "null" && string.IsNullOrEmpty(attr.value) == false)
            {
                attr = node.attributes.getNamedItem("title");
                string title = (attr == null || attr.value == "null") ? "" : attr.value;
                base.writeElement(title, element.TagName, element);
            }
            return base.OnOtherElementFinded(element);
        }
    }
}
