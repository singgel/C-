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
    class CQCFillConfigurationAction : FillConfigurationAction
    {
        public CQCFillConfigurationAction(WebBrowser webBrowser)
            : base(webBrowser)
        {
        
        
        
        }

        //3C第一个页面点击按钮
        public void tryFill3CStep1PressButton()
        {
            //this.getHtmlAppUtil().getHtmlActor().click("let4543", "汽车参数申报", "", "leftFrame");
            this.getHtmlAppUtil().getHtmlActor().navigateScript("onClickTreeItem();onClickFolder(nod4543);");
            //this.getWebBrowser().Navigate("");
            //this.getHtmlAppUtil().getHtmlActor().execScript("onClickTreeItem();onClickFolder(nod4543);void(0);", "leftFrame");
        }

        //3C选择车型
        public void tryFill3CStep1Fill()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            hevs.setRadioChecked(null, "id", "20141103213616gg6100", "mainFrame");
        }

        public void tryFill3CStep1Submit()
        {
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hea.click(null, "continueWrite","   查   看   ", "mainFrame");
        }

        public void tryFill3CStep2PressButton()
        {
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hea.clickArea("0总则", "mainFrame", "paraleft");
        }

        //3C参数填报
        public void tryFill3CStep2Fill()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            hevs.setTextValue("f_c_0_1_2", "f_c_0_1_2", "测试生产厂名称", "mainFrame", "paramain");
            hevs.setTextValue("f_0_2", "f_0_2", "测试生产厂名称", "mainFrame", "paramain");
            hevs.setTextValue("f_0_2_0_2", "f_0_2_0_2", "测试车身型式", "mainFrame", "paramain");
            hevs.setTextValue("f_c_0_2_0_0_1_2", "f_c_0_2_0_0_1_2", "测试整车型号", "mainFrame", "paramain");
            hevs.setTextValue("f_c_0_2_1", "f_c_0_2_1", "中文名称", "mainFrame", "paramain");
            hevs.setTextValue("f_0_2_1", "f_0_2_1", "英文名称", "mainFrame", "paramain");
        }
    }
}
