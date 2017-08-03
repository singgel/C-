using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assistant.Container;
using Assistant.Service;
using EntityLib;
using System.Collections;


namespace Assistant.Entity
{
    //一个车型下的一个配置的参数类
    public class CarParams
    {
        public const String CONFIGCODESERVICE = "临时配置序列号（ES）";
        //确定配置的唯一标识
        public String packageCode;
        //参数集合
        public List<Param> listParams = new List<Param>();

        public Hashtable data = new Hashtable();
        //真正能添加到页面上的参数集合
        public List<HtmlAttribute> listHtmlAttributes = new List<HtmlAttribute>();
        //参数是否重复
        public Boolean isDuplicated;
        //是否需要申请配置号
        public Boolean needConfigCode;
        //当前配置版本号
        public String version;
        //配置号
        public String configCode;

        public String ConfigCode
        {
            get { return configCode; }
            set { 
                configCode = value;
                ImportServiceForSHFY.setParameter(CONFIGCODESERVICE, value, ParamsCollection.runningCarParams.listParams);
            }
        }

        //是否已经成功发送配置号
        public Boolean sendConfigCode = false;
        //是否已经成功提交配置号
        public Boolean submitConfigCode = false;


    }

    

}
