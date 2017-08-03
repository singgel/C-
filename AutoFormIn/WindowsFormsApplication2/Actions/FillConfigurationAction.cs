using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;
using System.Threading;

using Assistant.Container;

using Assistant.Matcher;
using WebBrowserUtils.HtmlUtils.Setters;

namespace Assistant.Actions
{
    class FillConfigurationAction:BaseAction
    {
        public FillConfigurationAction(WebBrowser webBrowser) : base(webBrowser) 
        {
            // 基本信息


        }


        /// <summary>
        /// 填充配置号填报页
        /// </summary>
        public String tryFillConfiguration()
        {
            //总之就是读取全部表格中信息
            foreach (HtmlAttribute ha in Matcher.Matcher.standardCheckBoxAttribute)
            {
                this.getHtmlAppUtil().getHtmlSetter().setChecked(ha.elementId);
            }

            foreach (OtherCheckBoxAttritute ha in Matcher.Matcher.otherCheckBoxAttribute)
            {
                this.getHtmlAppUtil().getHtmlSetter().setOtherCheckBoxValue(ha.divId, ha.elementId, ha.check, ha.value);
            }

            foreach (HtmlAttribute ha in Matcher.Matcher.standardSelectAttribute)
            {
                this.getHtmlAppUtil().getHtmlSetter().selectItem(ha.elementId, ha.value);
            }

            foreach (OtherSelectAttribute ha in Matcher.Matcher.otherSelectAttribute)
            {
                this.getHtmlAppUtil().getHtmlSetter().selectOtherItem(ha.elementId, ha.name, ha.contextId, ha.value);
            }

            foreach (HtmlAttribute ha in Matcher.Matcher.standardTextAttribute)
            {
                this.getHtmlAppUtil().getHtmlSetter().setTextValue(ha.elementId, ha.value);
            }

            this.getHtmlAppUtil().getHtmlActor().invokeScript("ChangeVisibility");
            
            return Constants.SUCCESS;
        }

        //测试填充全部参数
        public String fillConfigTest(List<HtmlAttribute> list) {
            foreach (HtmlAttribute ha in list) {
                if (ha is OtherCheckBoxAttritute)
                {
                    OtherCheckBoxAttritute ha2 = ha as OtherCheckBoxAttritute;
                    this.getHtmlAppUtil().getHtmlSetter().setOtherCheckBoxValue(ha2.divId, ha2.elementId, ha2.check, ha2.value);
                }
                else if (ha is OtherSelectAttribute)
                {
                    OtherSelectAttribute ha2 = ha as OtherSelectAttribute;
                    this.getHtmlAppUtil().getHtmlSetter().selectOtherItem(ha2.elementId, ha2.name, ha2.contextId, ha2.value);
                }
                else
                {
                    if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT))
                    {
                        this.getHtmlAppUtil().getHtmlSetter().setTextValue(ha.elementId, ha.value);

                    }
                    else if (ha.elementType.Equals(Matcher.Matcher.TYPE_CHECK_BOX) || ha.elementType.Equals(Matcher.Matcher.TYPE_OTHER_CHECK_BOX))
                    {
                        this.getHtmlAppUtil().getHtmlSetter().setChecked(ha.elementId);

                    }
                    else if (ha.elementType.Equals(Matcher.Matcher.TYPE_SELECT))
                    {
                        this.getHtmlAppUtil().getHtmlSetter().selectItem2(ha.elementId, ha.value);
                    }
                }
            }
            this.getHtmlAppUtil().getHtmlActor().invokeScript("ChangeVisibility");
            return Constants.SUCCESS;
        }

        //测试填充全部参数
        public String submit()
        {
            this.getHtmlAppUtil().getHtmlActor().click("Button_Save");
            return Constants.SUCCESS;
        }


        public String selectItemAndPageChange() { 
            this.getHtmlAppUtil().getHtmlSetter().selectItem("Basis_V_BC", "电车");
            this.getHtmlAppUtil().getHtmlActor().invokeScript("ChangeVisibility");
            return Constants.SUCCESS;
        }

        /// <summary>
        /// 找到config对应行的提交按钮
        /// </summary>
        /// <param name="web"></param>
        /// <param name="configCode"></param>
        /// <returns></returns>
        public String submitConfigCode(String configCode) {

            this.getHtmlAppUtil().getHtmlSetter().submitConfigCode(configCode);
            return Constants.SUCCESS;
        }

    }
}
