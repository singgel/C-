using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assistant.Entity;
using Assistant.Container;
using System.Data;
using Assistant.Service;
using System.Net;
using System.Windows.Forms;

namespace Assistant
{
    class Constants
    {
        public static String LOGIN_PAGE = "http://code.vidc.info/subPage/login.aspx";

        public static String MAIN_MENU = "http://code.vidc.info/index.aspx";

        public static String APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE = "http://code.vidc.info/subPage/content.aspx";

        public static String declarationType = null;

        public static String SUCCESS = "success";

        public static int TIMER_INTERVAL = 5;

        public static String username = "";

        private static String localAddress;

        public static String LocalAddress
        {
            get { return ParamsCollection.getLocalAddr(); }
        }

        public static AssistantUpdater.LoginUser CurrentUser
        {
            get;
            internal set;
        }
    }

    public class DeclarationType
    {
        //国环
        public const String declarationGH = "GH";
        //北环
        public const String declarationBH = "BH";
        // 非道路机动车
        public const string declarationFDL = "FDL";
        //新能源
        public const String declarationXNY = "XNY";
        //3C填报
        public const String declaration3C = "CCC";
        //中机配置号填报
        public const String declarationPZ = "PZ";
    }

    public class DeclarationSite
    {
        //北环
        public const String BHSite = "http://58.30.229.122:8080/motor/login.action";

        //新能源
        //public const String XNYSite = "http://nea.catarc.info/Login/Index/";
        public const String XNYSite = "http://10.8.10.176:8090/Admin/Home/Index";
        //国环
        public const string GHSite = "http://xshz.vecc-mep.org.cn/newvip/login.jsp";
        //public const String GHSite = "http://ids.vecc-mep.org.cn/Ids/login.jsp";
        //public const String GHSite = "http://tec.cqccms.com.cn/";

        //3C
        public const String CCCSite = "http://tec.cqccms.com.cn/";

        //配置号
        public const String PZSite = "http://code.vidc.info/subPage/login.aspx";

        public const String ADCSite = "http://www.catarc.info/7_jd.html";

        public const string FDLSite = "http://xshz.vecc-mep.org.cn/fdl/login.jsp";
    }


    public class DeclarationInfo
    {
        public static class DeclarationGH
        {
            public const String Step2_QYBZ = "测试企业标准";
        }

        public static class DeclarationBH
        {
            public const String MainFrameName = "Framewindow";
        }

        public static class DeclarationXNY
        {
            public const String MainFrameName = "Framewindow";
        }
    }

    public class ParamType
    {
        public const String engineParam = "engineParam";
        public const String configParam = "configParam";
    }

    public class InstName
    {
        public const String SHFY = "上海泛亚";
        public const String HCBM = "华晨宝马";
    }

    public class instInfo
    {
        public String instName;
        public Dictionary<String, String> dataConverter;
    }

    public class HCBMSeparator
    {
        public const String packageCodeJoiner = "_";
    }

    //储存全部车辆参数
    public class ParamsCollection
    {
        //存储车辆参数
        public static List<CarParams> carParams = new List<CarParams>();
        //存储rules参数
        public static Dictionary<String, HtmlAttribute> dicRules = new Dictionary<string, HtmlAttribute>();
        //存储被选中的packagecode
        public static List<String> selectedPackageCodes = new List<string>();
        //获取程序运行路径
        public static String localAddr = getLocalAddr();

        public static String getLocalAddr()
        {
            string startPath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();   //获取dll所在路径
            int last = startPath.LastIndexOf('\\');
            startPath = startPath.Substring(0, last + 1);
            return startPath;
        }
        //是否已经获取配置号
        public static Boolean receiveConfigCode = false;
        public static CarParams runningCarParams = null;

        public static DataTable dt = null;
    }

    public class ClassFactory
    {
        public static FlexService.IFlexUserServiceService fuss = GetFlexService();

        public static FlexService.IFlexUserServiceService GetFlexService()
        {
            FlexService.IFlexUserServiceService service = new FlexService.IFlexUserServiceService();
            if (Properties.Settings.Default.IsUsingProxy)
            {
                WebProxy Proxy = new WebProxy(string.Format("http://{0}:{1}",
                    Properties.Settings.Default.ProxyAddress,
                    Properties.Settings.Default.ProxyPort), true);
                Proxy.Credentials = new NetworkCredential(Properties.Settings.Default.ProxyUsername,
                    Properties.Settings.Default.ProxyPassword, Properties.Settings.Default.DomainName);
                service.Proxy = Proxy;
            }
            service.Url = Properties.Settings.Default.Assistant_FlexService_IFlexUserServiceService;
            return service;
        }
    }
}
