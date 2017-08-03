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
    class BHFillConfigurationAction : FillConfigurationAction
    {
        public BHFillConfigurationAction(WebBrowser webBrowser)
            : base(webBrowser)
        {
        }

        public void step0Login()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            hevs.setTextValue("departNo", "departNo", "78585081-3");
            hevs.setTextValue("userno", "userno", "admin");
            hevs.setTextValue("password", "password", "Saic_T&V");
        }

        public void step0Submit() {
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hea.invokeScript("FormSubmit");
        }

        public void step1Fill()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hevs.setRadioChecked("RadioGroup1_0", "cartype", "QQC", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.setTextValue("car", "entity.car", "BJ5042XTY-G1", DeclarationInfo.DeclarationBH.MainFrameName);
        }

        public void step1Submit()
        {
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hea.invokeScript("formSubmit", DeclarationInfo.DeclarationBH.MainFrameName);
        }

        public void step2Submit()
        {
            HtmlElementActions hea = this.getHtmlAppUtil().getHtmlActor();
            hea.invokeScript("nextStep", DeclarationInfo.DeclarationBH.MainFrameName);
            //hea.click(null, null, "下一步");
        }

        public void step3Fill()
        {
            HtmlElementValueSetter hevs = this.getHtmlAppUtil().getHtmlSetter();
            hevs.setTextValue(null, "carname", "测试名称", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.setTextValue(null, "popname", "测试俗名", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.selectItem("standard", "standard", "4", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.selectItem(null, "category", "1", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.setTextValue(null, "maxspeed", "100", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.setTextValue(null, "maxweight", "1000", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.setTextValue(null, "standweight", "1000", DeclarationInfo.DeclarationBH.MainFrameName);
            hevs.setTextValue(null, "seats", "1000", DeclarationInfo.DeclarationBH.MainFrameName);

        }
        
    }

  
}
