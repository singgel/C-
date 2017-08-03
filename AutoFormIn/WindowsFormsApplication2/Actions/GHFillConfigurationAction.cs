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
using WebBrowserUtils.HtmlUtils.Actions;

namespace Assistant.Actions
{
    class GHFillConfigurationAction : FillConfigurationAction
    {
        public GHFillConfigurationAction(WebBrowser webBrowser) : base(webBrowser)
        {
        
        }

        //国环第一个页面参数填报
        public void tryFillGHStep1()
        {
            this.getHtmlAppUtil().getHtmlSetter().setTextValue("mid2", "mid", "10120290-4");
            this.getHtmlAppUtil().getHtmlSetter().setTextValue("mid", "username", "huwei");
            this.getHtmlAppUtil().getHtmlSetter().setTextValue("mid2", "pwd", "123456");
        }

        public void tryFillGHStep1Submit()
        {
            this.getHtmlAppUtil().getHtmlActor().click(null, "Submit");
        }

        //国环第二个页面 选择车辆类别参数填报
        public void tryFillGHStep2()
        {
            this.getHtmlAppUtil().getHtmlSetter().setTextValue("nbbh", "nbbh", "Catarc_t_02");
            this.getHtmlAppUtil().getHtmlSetter().selectItem("pfjd", "jspfjd", "4");
            //只有重型汽油车和轻型汽油车都是可以的
            this.getHtmlAppUtil().getHtmlSetter().setRadioChecked(null, "cllb", "轻型汽油车");
        }

        //国环第二个页面 选择车辆类别参数提交
        public void tryFillGHStep2Submit()
        {
            this.getHtmlAppUtil().getHtmlActor().click("action", "action");
        }

        //国环第三个页面/step2 选择执行标准
        public void tryFillGHStep3()
        {
            this.getHtmlAppUtil().getHtmlSetter().setChecked("gjbz", "gjbz", "GB 17691-2005第四阶段");
            this.getHtmlAppUtil().getHtmlSetter().setChecked("gjbz", "gjbz", "GB 3847-2005");
            this.getHtmlAppUtil().getHtmlSetter().setTextValue("qybz", "qybz", DeclarationInfo.DeclarationGH.Step2_QYBZ);

        }

        //国环第三个页面/step2 选择执行标准提交按钮
        public void tryFillGHStep3Submit()
        {
            this.getHtmlAppUtil().getHtmlActor().click("action", "action");
        }

        public void tryFillGHStep4Submit()
        {
            this.getHtmlAppUtil().getHtmlActor().click("action", "action", "创建附录A");
        }

        public void tryFillGHStep3AddAppendixInfo()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            hevs.selectItem("select", "newsb", "福田牌");
            hevs.setTextValue("zzcxh", "zclxh", "BJ5042XTY-G1");
            hevs.setTextValue("zcxmc", "zclmc", "密闭式桶装垃圾车");
            hevs.setTextValue("a", "a", "BJ5042XTY-G2");
            hevs.setTextValue("newvinwz", "newvinwz", "发动机右侧");
            hevs.selectItem("select", "newcllb", "N1");
            hevs.setTextValue("newsccdz2", "newcxbswz", "N/A");
            hevs.selectItem("newsccmc", "newsccmc", "北汽福田汽车股份有限公司");
            hevs.setTextValue("newsccdz", "newsccdz", "北汽福田汽车股份有限公司");
            hevs.setTextValue("newobdwz", "newobdwz", "车身后部");
        }

        public void testGH()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hea.click(null, "action", "上传图片");
        }

        public void tryFillGHStep3AddEngineSample()
        {


        }

        //覆盖js里面的null 和 confirm等方法
        public void overrideJS() {
            this.getHtmlAppUtil().getHtmlActor().execScript("window.alert=null;window.onerror=null;window.confirm=null;window.open=null;window.showModalDialog=null;");
        }

        public void deleteFirstRadio() {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            hevs.setRadioChecked();
            this.getHtmlAppUtil().getHtmlActor().click("action", "action", "删除");
        }




    }
}
