using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelUtils;
using Assistant.Container;
using Assistant.Entity;
using Assistant.Matcher;
using Assistant.ConfigCodeService;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using EntityLib;


namespace Assistant.Service
{
    class DataTransformServiceForSHFY
    {

        /// <summary>
        /// 将单个参数param转化为htmlAttribute
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static List<HtmlAttribute> transformFromParamToHA(Param param, Dictionary<String, HtmlAttribute> dic)
        {
            String cname = param.cname;
            String value = param.value;
            if (cname.Equals("年款")) {
                if (String.IsNullOrWhiteSpace(value))
                {
                }
                else {
                    if (!value.EndsWith("款")) {
                        value = value + "款";
                    }
                }
            }
            List<HtmlAttribute> result = new List<HtmlAttribute>();
            if (String.IsNullOrEmpty(cname))
            {
                return null;
            }
            if (!dic.ContainsKey(cname))
            {
                List<HtmlAttribute> list = handleSpecialParam(param, dic);
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                else {
                    result.AddRange(list);
                    return result;
                }
            }
            HtmlAttribute ha = dic[cname];
            HtmlAttribute realHtmlParam = null;
            
            if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT) || (ha.elementType.Equals(Matcher.Matcher.TYPE_CHECK_BOX)))
            {
                if (!param.value.EndsWith("_其他"))
                {
                    realHtmlParam = new HtmlAttribute(ha.elementId, ha.name, ha.elementType, value);
                    result.Add(realHtmlParam);
                }
                else
                {
                    if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT))
                    {
                        result.Add(new OtherSelectAttribute("test", ha.name, ha.elementId, param.value));
                    }
                }

            }
            //othercheckbox类型
            else if (ha.elementType.Equals(Matcher.Matcher.TYPE_OTHER_CHECK_BOX))
            {
                //没有内容二字的只是普通checkbox
                if (!ha.name.Contains("内容"))
                {
                    result.Add(new HtmlAttribute(ha.elementId, ha.name, ha.elementType, "checked"));
                }
                //包含内容二字就包括单元格了
                else
                {
                    result.Add(new OtherCheckBoxAttritute(ha.elementId, ha.name, Matcher.Matcher.ELEMENT_STYLE, param.value,
                                "Div_" + ha.elementId,
                                "checked"));
                }
            }
            //select类型
            else if (ha.elementType.Equals(Matcher.Matcher.TYPE_SELECT))
            {
                result.Add(new HtmlAttribute(ha.elementId, ha.name, ha.elementType, param.value));
            }

            return result;
        }

        /// <summary>
        /// 将多个参数转化为HtmlAttribute
        /// </summary>
        /// <param name="listParam"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static List<HtmlAttribute> getAllHAInACar(List<Param> listParam, Dictionary<String, HtmlAttribute> dic)
        {
            List<HtmlAttribute> result = new List<HtmlAttribute>();
            foreach (Param p in listParam)
            {
                List<HtmlAttribute> list = transformFromParamToHA(p, dic);
                if (list != null)
                {
                    result.AddRange(list);
                }

            }
            return result;
        }

        /// <summary>
        /// 根据类型处理htmlAttribute
        /// </summary>
        /// <param name="ha"></param>
        /// <returns></returns>

        public static String handleHtmlAttributes(HtmlAttribute ha)
        {
            StringBuilder sb = new StringBuilder();
            if (ha is OtherCheckBoxAttritute)
            {
                sb.Append(ha.name + "  " + "OtherCheckBoxAttritute" + ha.value + "\r\n");
            }
            else if (ha is OtherSelectAttribute)
            {
                sb.Append(ha.name + "  " + "OtherSelectAttribute" + ha.value + "\r\n");
            }
            else
            {
                if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT))
                {
                    sb.Append(ha.name + "  " + Matcher.Matcher.TYPE_TEXT + ha.value + "\r\n");

                }
                else if (ha.elementType.Equals(Matcher.Matcher.TYPE_CHECK_BOX) || ha.elementType.Equals(Matcher.Matcher.TYPE_OTHER_CHECK_BOX))
                {
                    sb.Append(ha.name + "  " + Matcher.Matcher.TYPE_CHECK_BOX + ha.value + "\r\n");

                }
                else if (ha.elementType.Equals(Matcher.Matcher.TYPE_SELECT))
                {
                    sb.Append(ha.name + "  " + Matcher.Matcher.TYPE_SELECT + ha.value + "\r\n");

                }
            }
            return sb.ToString();
        }

        private static List<HtmlAttribute> handleSpecialParam(Param param, Dictionary<String, HtmlAttribute> dicRules) {
            List<HtmlAttribute> result = new List<HtmlAttribute>();
            String cname = param.cname.Trim();
            String value = param.value.Trim();
            if (cname.Equals("车身结构") || cname.Equals("发动机技术"))
            {
                if (dicRules.ContainsKey(value))
                {
                    HtmlAttribute ha = dicRules[value];
                    result.Add(new HtmlAttribute(ha.elementId, ha.name, ha.elementType, param.value));
                }
                else {
                    HtmlAttribute ha = dicRules[cname + "_其他"];
                    result.Add(new HtmlAttribute(ha.elementId, ha.name, ha.elementType, Matcher.Matcher.VALUE_CHECKBOX));
                    ha = dicRules[cname + "_其他" + "内容"];
                    result.Add(new OtherCheckBoxAttritute(ha.elementId, ha.name, Matcher.Matcher.ELEMENT_STYLE, param.value,
                                "Div_" + ha.elementId,
                                "checked"));
                }
            }
            return result;
        }

        /// <summary>
        /// 从提示信息中获取配置号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static String getConfigCodeFromTip(String s)
        {
            if (s == null)
                return null;
            Regex reg = new Regex(@"\d{10,}");
            String u = reg.Match(s).ToString();
            Boolean b = reg.IsMatch(s);
            if (b == true)
            {
                return u;
            }
            else
            {
                return null;
            }
        }
    }
}
