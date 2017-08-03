using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using WebBrowserUtils.HtmlUtils;

namespace WebBrowserUtils.HtmlUtils.Setters
{
    public class HtmlElementValueSetter : BaseImpl
    {
        public HtmlElementValueSetter(WebBrowser webBrowser)
            : base(webBrowser)
        {
        }
        /// <summary>
        /// 根据id插入值，setValue(String elementId, String attribute, String value)
        /// </summary>
        public String setValue(String elementId, String attribute, String value)
        {
            try
            {
                this.htmlDocument.All[elementId].SetAttribute(attribute, value);
            }
            catch (Exception)
            {



            }
            return HtmlConstants.ActionReturnValues.SUCCESS;

        }

        public String setValue(String elementId, String elementName, String attribute, String value)
        {
            try
            {
                foreach (HtmlElement he in this.htmlDocument.All)
                {
                    String eId = he.Id;
                    String eName = he.Name;
                    if (eId == elementId && eName == elementName)
                    {
                        he.SetAttribute(attribute, value);
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String setValue(String elementId, String elementName, String attribute, String value, String frame)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                foreach (HtmlElement he in hw.Document.All)
                {
                    String eId = he.Id;
                    String eName = he.Name;
                    if (eId == elementId && eName == elementName)
                    {
                        he.SetAttribute(attribute, value);
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String setValue(String elementId, String elementName, String attribute, String value, String frame, String frameInside)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                hw = hw.Document.Window.Frames[frameInside];
                foreach (HtmlElement he in hw.Document.All)
                {
                    String eId = he.Id;
                    String eName = he.Name;
                    if (eId == elementId && eName == elementName)
                    {
                        he.SetAttribute(attribute, value);
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        /// <summary>
        /// 给text类元素设置value值，setTextValue(String elementId, String value)
        /// </summary>
        public String setTextValue(String elementId, String value)
        {
            return this.setValue(elementId, HtmlConstants.ElementType.TEXT_BOX_VALUE, value);
        }

        public String setTextValue(String elementId, String elementName, String value)
        {
            return this.setValue(elementId, elementName, HtmlConstants.ElementType.TEXT_BOX_VALUE, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <param name="frame">在页面内指定帧内查找元素</param>
        /// <returns></returns>
        public String setTextValue(String elementId, String elementName, String value, String frame)
        {
            return this.setValue(elementId, elementName, HtmlConstants.ElementType.TEXT_BOX_VALUE, value, frame);
        }

        public String setTextValue(String elementId, String elementName, String value, String frame, String frameInside)
        {
            return this.setValue(elementId, elementName, HtmlConstants.ElementType.TEXT_BOX_VALUE, value, frame, frameInside);
        }

        /// <summary>
        /// 对应车身结构的复选框中的，弹出空格的插入数据。
        /// setOtherCheckBoxValue(String divId, String textId, String check, String textValue)
        /// </summary>
        public String setOtherCheckBoxValue(String divId, String textId, String check, String textValue)
        {
            try
            {
                String display = HtmlConstants.ElementValueType.CHECK_BOX_CHECKED_OFF;
                if (!check.Equals(display))
                {
                    display = HtmlConstants.ElementValueType.CHECK_BOX_CHECKED_ON;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("display:").Append(display).Append(";");
                this.htmlDocument.All[divId].Style = sb.ToString();
                this.htmlDocument.All[textId].SetAttribute(HtmlConstants.ElementType.TEXT_BOX_VALUE, textValue);
            }
            catch
            {
            }

            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        /// <summary>
        /// 下拉框对象根据文字项目选中
        /// </summary>
        /// <param name="elementId">元素id</param>
        /// <param name="item">显示文本</param>
        /// <returns>执行结果</returns>
        public String selectItem(String elementId, String item)
        {
            try
            {
                //this.htmlDocument.GetElementById(elementId).Focus();
                StringBuilder sb = new StringBuilder();
                sb.Append("$('#" + elementId + "').find(\"option[text='" + item + "']\").attr(\"selected\",\"selected\");");
                //激发change事件,实现网页填报同样的效果
                sb.Append("$('#" + elementId + "').change();");
                this.htmlDocument2.parentWindow.execScript(sb.ToString(), "javascript");
            }
            catch
            {
            }

            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String selectItem2(String elementId, String item)
        {
            List<String> options = new List<String>();
            String selectedOption = null;
            StringBuilder sbJScript = new StringBuilder();
            if (item == null)
            {
                item = "";
            }
            item = item.Trim();
            //this.htmlDocument.GetElementById(elementId).Focus();
            foreach (HtmlElement ha in this.htmlDocument.All[elementId].Children)
            {
                String option = ha.InnerText;
                if (option == null) {
                    option = "";
                }
                options.Add(option.Trim());
            }
            if (options.Count == 2 && options.Contains(""))
            {  //只有唯一选项的时候直接选中
                item = options[1];
            }
            if (options.Count == 1)
            {
                item = options[0];
            }
            if (item.Contains("中国"))
            {
                var iter = from d in options where d.Contains("中国") select d;
                foreach (var v in iter)
                {
                    item = v;
                }
            }
            if (options.Contains(item))
            {

                sbJScript.Append("$('#" + elementId + "').find(\"option[text='" + item + "']\").attr(\"selected\",\"selected\");");
                //激发change事件,实现网页填报同样的效果
                sbJScript.Append("$('#" + elementId + "').change();");
                this.htmlDocument2.parentWindow.execScript(sbJScript.ToString(), "javascript");
            }
            else
            {
                if (options.Contains("其他"))
                {
                    sbJScript.Append("$('#" + elementId + "').find(\"option[text='" + "其他" + "']\").attr(\"selected\",\"selected\");");
                    //激发change事件,实现网页填报同样的效果
                    sbJScript.Append("$('#" + elementId + "').change();");
                    this.htmlDocument2.parentWindow.execScript(sbJScript.ToString(), "javascript");
                    //使用硬编码方式插入数据
                    this.setTextValue(elementId.Replace("Item", "Other"), item);
                }
                else
                {
                    return HtmlConstants.ActionReturnValues.ERROR;
                }
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public Boolean submitConfigCode(String configcode)
        {
            HtmlElement configCodeElement = null;
            HtmlElement submitElement = null;
            foreach (HtmlElement ha in this.htmlDocument.All)
            {
                if (ha.InnerText == configcode)
                {
                    configCodeElement = ha;
                }

            }
            if (configCodeElement == null)
            {
                return false;
            }
            submitElement = configCodeElement.Parent.Parent.Children[3].Children[0];
            if (submitElement == null)
            {
                return false;
            }
            submitElement.Focus();
            submitElement.InvokeMember(HtmlConstants.ActionType.CLICK);

            return true;
        }

        /// <summary>
        /// 下拉框选中select中option的value
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <param name="value">select下对应option的value</param>
        /// <returns></returns>
        public String selectItem(String elementId, String elementName, String value)
        {
            try
            {
                foreach (HtmlElement he in this.webBrowser.Document.All)
                {
                    if (he.Id == elementId && he.Name == elementName)
                    {
                        he.SetAttribute("value", value);
                    }
                }
            }
            catch
            {
            }

            return HtmlConstants.ActionReturnValues.SUCCESS;
        }
        /// <summary>
        /// 选择指定frame中的select
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public String selectItem(String elementId, String elementName, String value, String frame)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                foreach (HtmlElement he in hw.Document.All)
                {
                    if (he.Id == elementId && he.Name == elementName)
                    {
                        he.SetAttribute("value", value);
                    }
                }
            }
            catch
            {
            }

            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        /// <summary>
        /// 下拉框对象根据文字项目选中
        /// </summary>
        /// <param name="elementId">元素id</param>
        /// <param name="item">显示文本</param>
        /// <returns>执行结果</returns>
        public String selectOtherItem(String elementId, String item, String otherElementId, String value)
        {
            try
            {
                this.htmlDocument2.parentWindow
                    .execScript("$('#" + elementId + "').find(\"option[text='" + item
                    + "']\").attr(\"selected\",true);", "javascript");
                StringBuilder sb = new StringBuilder();
                sb.Append("display:").Append(HtmlConstants.ElementValueType.CHECK_BOX_CHECKED_ON).Append(";");
                this.htmlDocument.All["Div_" + otherElementId].Style = sb.ToString();
                this.htmlDocument.All[otherElementId].SetAttribute(HtmlConstants.ElementType.TEXT_BOX_VALUE, value);
            }
            catch
            {

            }

            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        private const String CHECKED = "checked";

        public String setChecked(String elementId)
        {
            try
            {
                this.htmlDocument.All[elementId].SetAttribute(CHECKED, CHECKED);
            }
            catch (Exception)
            {

                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }
        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public String setChecked(String elementId, String elementName, String value)
        {
            try
            {
                foreach (HtmlElement he in this.webBrowser.Document.All)
                {
                    if (he.Id == elementId && he.Name == elementName && he.GetAttribute("value") == value)
                    {

                        he.SetAttribute(CHECKED, true.ToString());
                        return HtmlConstants.ActionReturnValues.SUCCESS;
                    }
                }
            }
            catch
            {

            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        /// <summary>
        /// 选中对应id,name和value的radio按钮
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public String setRadioChecked(String elementId, String elementName, String value)
        {
            try
            {
                foreach (HtmlElement he in this.webBrowser.Document.All)
                {
                    if (he.Id == elementId && he.Name == elementName && he.GetAttribute("value") == value)
                    {
                        he.SetAttribute(CHECKED, true.ToString());
                        return HtmlConstants.ActionReturnValues.SUCCESS;
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String setRadioChecked(String elementId, String elementName, String value, String frame)
        {
            try
            {
                HtmlWindow hw = this.webBrowser.Document.Window.Frames[frame];
                foreach (HtmlElement he in hw.Document.All)
                {
                    if (he.Id == elementId && he.Name == elementName && he.GetAttribute("value") == value)
                    {
                        he.SetAttribute(CHECKED, true.ToString());
                        return HtmlConstants.ActionReturnValues.SUCCESS;
                    }

                }
            }
            catch (Exception)
            {

                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;
        }

        public String setRadioChecked()
        {
            try
            {
                foreach (HtmlElement he in this.webBrowser.Document.All)
                {
                    if (he.GetAttribute("type") == "radio")
                    {
                        if (he.Parent.Parent.Parent.InnerText.Contains("张少彦") &&
                            he.Parent.Parent.Parent.Children[2].InnerText.Trim().Length < 5)
                        {
                            he.SetAttribute(CHECKED, true.ToString());
                            return HtmlConstants.ActionReturnValues.SUCCESS;
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
            return HtmlConstants.ActionReturnValues.SUCCESS;


        }
    }
}