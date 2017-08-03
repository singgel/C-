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
using EntityLib;

namespace Assistant.Service
{
    class ImportServiceForSHFY
    {
        public void importRuleData(String ruleFilePath)
        {
            //获取rules信息
            DataTable dtRules = ExcelUtils.ExcelUtil.getDataTableFromExcel(ruleFilePath);
            LoggerUtils.LoggerUtils.logger(dtRules.Rows.Count.ToString());
            Dictionary<String, HtmlAttribute> dicRules = new Dictionary<String, HtmlAttribute>();
            int totalRowRules = dtRules.Rows.Count;
            for (int rownoRules = 1; rownoRules < totalRowRules; rownoRules++)
            {
                String name = ExcelUtil.reader.getCellValue(rownoRules, 2, dtRules).Trim();
                String eid = ExcelUtil.reader.getCellValue(rownoRules, 1, dtRules).Trim();
                String etype = ExcelUtil.reader.getCellValue(rownoRules, 3, dtRules).Trim();
                HtmlAttribute ha = new HtmlAttribute(eid, name, etype, null);
                if (dicRules.ContainsKey(name) || String.IsNullOrEmpty(name)) {
                    continue;
                }
                dicRules.Add(name, ha);
            }
            ParamsCollection.dicRules = dicRules;
        }

        public void importCarsParams(String dataFilePath)
        {
            String engineInfoStartString = "车辆名称";
            String configInfoStartString = "MD配置信息";
            String patacCodeInitial = "P";
            DataTable dt = ExcelUtils.ExcelUtil.getDataTableFromExcel(dataFilePath);
            ParamsCollection.dt = dt;
            Dictionary<String, HtmlAttribute> dicRules = ParamsCollection.dicRules;
            //所有车型配置信息的list
            List<CarParams> listCarParams = new List<CarParams>();
            LoggerUtils.LoggerUtils.logger(dt.Columns.Count.ToString());
            int columnCounterBeforeConfig = 3;
            int carConfigCounter = dt.Columns.Count - columnCounterBeforeConfig;
            //针对每个配置信息获取对应参数
            for (int carConfigIter = 0; carConfigIter < carConfigCounter; carConfigIter++)
            {
                CarParams cps = new CarParams();
                List<paramPo> listParampo = new List<paramPo>();
                //配置所在列数
                int columnNo = carConfigIter + columnCounterBeforeConfig;
                //行号
                int rowno = 0;
                String cname = dt.Rows[rowno][0].ToString();
                String value = dt.Rows[rowno][columnNo].ToString();
                String pcode = dt.Rows[rowno][1].ToString();
                //当不是车辆名称的时候继续增加行数
                while (!cname.Equals("车辆名称"))
                {
                    rowno++;
                    cname = dt.Rows[rowno][0].ToString();
                    if (rowno >= dt.Rows.Count)
                    {
                        throw new Exception("已经超过最大行，还未找到车辆名称");
                    }
                }
                //从车辆名称到燃料种类
                while (!cname.Equals("部门会签"))
                {
                    if (String.IsNullOrEmpty(cname))
                    {
                        rowno++;
                        cname = dt.Rows[rowno][0].ToString();
                        continue;
                    }
                    value = dt.Rows[rowno][columnNo].ToString();
                    pcode = dt.Rows[rowno][1].ToString();
                    Param p = new EngineParam(cname, pcode, value);
                    cps.listParams.Add(p);
                    rowno++;
                    cname = dt.Rows[rowno][0].ToString();
                }

                //获取配置号信息
                String packageCode = ImportServiceForSHFY.getParameter("Model Code", cps.listParams)
                    + ImportServiceForSHFY.getParameter("Package Code", cps.listParams) + "-"
                    + ImportServiceForSHFY.getParameter("Order Sample", cps.listParams);
                cps.packageCode = packageCode;

                                //获取是否需要申报配置号信息
                String needConfig = ImportServiceForSHFY.getParameter("是否申报新配置", cps.listParams);
                if (needConfig.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    cps.needConfigCode = true;
                }
                else
                {
                    cps.needConfigCode = false;
                }

                //通过配置号信息获取工程信息数据
                List<String> listPackageCode = new List<string>();
                if (!String.IsNullOrEmpty(packageCode))
                {
                    //如果需要填报才显示对应的packagecode
                    if (cps.needConfigCode)
                    {
                        listPackageCode.Add(packageCode);
                    }
                    listParampo = ImportServiceForSHFY.getParamsByPackageCode(listPackageCode);
                }
                foreach (Param p in cps.listParams)
                {
                    //只有型式认证编码有效的才能够获取数据
                    if (p.patacode.StartsWith(patacCodeInitial))
                    {
                        String paramValue = ImportServiceForSHFY.getParameter(cps.packageCode, p.patacode, listParampo);
                        if (String.IsNullOrEmpty(p.value))
                        {
                            //p.value = "test";
                        }
                        if (String.IsNullOrEmpty(paramValue))
                        {
                            continue;
                        }
                        p.value = paramValue;
                    }
                }



                //获取配置信息版本号
                String versionNum = getVersionNumFromListParamPo(listParampo);
                cps.version = versionNum;

                //获取配置信息

                //从部门会签到MD配置信息
                while (!cname.Equals("MD配置信息"))
                {
                    rowno++;
                    cname = dt.Rows[rowno][0].ToString();
                }

                //对配置参数进行转化,转化为参数列表(listParams而不是listHtmlAttributes)
                while (rowno < dt.Rows.Count)
                {
                    cname = dt.Rows[rowno][0].ToString().Trim();
                    //将全部英文符号换成中文符号
                    cname = cname.Replace("(", "（").Replace(")", "）");
                    value = dt.Rows[rowno][columnNo].ToString().Trim();
                    if (String.IsNullOrEmpty(cname))
                    {
                        rowno++;
                        continue;
                    }
                    HtmlAttribute ha = null;
                    if (dicRules.ContainsKey(cname))
                    {
                        ha = dicRules[cname];
                    }
                    //当能够找到对应参数而且参数值不为空的时候才进行
                    //适用于能够直接找到对应参数名称的情况
                    if (ha != null && !String.IsNullOrEmpty(value))
                    {
                        //处理text类型和checkbox类型
                        if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT) || ha.elementType.Equals(Matcher.Matcher.TYPE_CHECK_BOX))
                        {

                            Param p = new ConfigParam(cname, null, Matcher.Matcher.VALUE_CHECKBOX);
                            cps.listParams.Add(p);
                        }
                        //处理othercheckbox类型
                        else if (ha.elementType.Equals(Matcher.Matcher.TYPE_OTHER_CHECK_BOX))
                        {
                            Param p1 = new ConfigParam(cname, null, Matcher.Matcher.VALUE_CHECKBOX);
                            cps.listParams.Add(p1);
                            if (dicRules.ContainsKey(cname + "内容"))
                            {
                                Param p2 = new ConfigParam(cname + "内容", null, value);
                                cps.listParams.Add(p2);
                            }
                        }
                    }
                    else if (cname.Contains("下拉菜单"))
                    {
                        cname = cname.Replace("（下拉菜单）", "").Trim();
                        //如果不包含直接下一个参数U
                        if (!dicRules.ContainsKey(cname))
                        {
                            rowno++;
                            continue;
                        }
                        ha = dicRules[cname];
                        Boolean got = false;
                        while (!cname.Equals("其他"))
                        {
                            rowno++;
                            cname = dt.Rows[rowno][0].ToString();
                            value = dt.Rows[rowno][columnNo].ToString();
                            if (!got)
                            {
                                if (!String.IsNullOrEmpty(value) && ha != null)
                                {
                                    //如果是S的话就是有这个配置，如果有这个配置就把对应的cname作为value。因为这是一个下拉菜单项
                                    Param p1 = new ConfigParam(ha.name, null, cname);
                                    cps.listParams.Add(p1);
                                    got = !got;
                                    if (cname.Equals("其他")) // 其他项目内容
                                    {
                                        Param p2 = new ConfigParam(ha.name + "其他", null, value);
                                        cps.listParams.Add(p2);
                                    }
                                }
                            }
                        }
                    }
                    rowno++;
                }
                listCarParams.Add(cps);

            }
            //将全部参数添加到参数合集当中
            ParamsCollection.carParams = listCarParams;
        }


        //从参数集合中获取版本号
        public static String getVersionNumFromListParamPo(List<paramPo> list){
            if (list == null || list.Count == 0)
            {
                //return new Exception("从型式认证获取参数为空,无法进行下一步填报");
                return "";
            }
            else {
                foreach (paramPo pp in list) {
                    if (String.IsNullOrEmpty(pp.version))
                    {
                        continue;
                    }
                    else {
                        return pp.version;
                    }
                }
                return "";
            }
        }

        public static List<HtmlAttribute> transformFromParamToHA(Param param, Dictionary<String, HtmlAttribute> dic)
        {
            String cname = param.cname;
            if (String.IsNullOrEmpty(cname))
            {
                return null;
            }
            if (!dic.ContainsKey(cname))
            {
                return null;
            }
            HtmlAttribute ha = dic[cname];
            HtmlAttribute realHtmlParam = null;
            List<HtmlAttribute> result = new List<HtmlAttribute>();
            if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT) || (ha.elementType.Equals(Matcher.Matcher.TYPE_CHECK_BOX)))
            {
                if (!param.value.EndsWith("_其他"))
                {
                    realHtmlParam = new HtmlAttribute(ha.elementId, ha.name, ha.elementType, param.value);
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

        //根据类型处理htmlAttribute
        public static String handleHtmlAttributes(HtmlAttribute ha)
        {
            StringBuilder sb = new StringBuilder();
            if (ha is OtherCheckBoxAttritute)
            {
                sb.Append(ha.name + "  " + "OtherCheckBoxAttritute" + "\r\n");


            }
            else if (ha is OtherSelectAttribute)
            {
                sb.Append(ha.name + "  " + "OtherSelectAttribute" + "\r\n");



            }
            else
            {
                if (ha.elementType.Equals(Matcher.Matcher.TYPE_TEXT))
                {
                    

                }
                else if (ha.elementType.Equals(Matcher.Matcher.TYPE_CHECK_BOX) || ha.elementType.Equals(Matcher.Matcher.TYPE_OTHER_CHECK_BOX))
                {
                    

                }
                else if (ha.elementType.Equals(Matcher.Matcher.TYPE_SELECT))
                {
                  

                }
            }
            return sb.ToString();
        }

        //从参数list中获取名称为name的参数
        public static String getParameter(String name, List<Param> list)
        {
            var result = from d in list where d.cname == name select d.value;
            foreach (var v in result)
            {
                //直接插入空串会让合并域消失，所以换成多个空格
                return v;
            }
            return null;
        }

        //从参数list中获取名称为name的参数
        public static String getParameter(String cname, String pcode, List<Param> list)
        {
            var result = from d in list where d.cname == cname && d.patacode == pcode select d.value;
            foreach (var v in result)
            {
                //直接插入空串会让合并域消失，所以换成多个空格
                return v;
            }
            return null;
        }

        //根据packagecode获取参数
        public static List<paramPo> getParamsByPackageCode(List<String> pcode)
        {
            IPackageInfoServiceService service = new IPackageInfoServiceService();
            paramPo[] paramPoArray = service.getParamsByPackageCode(pcode.ToArray());
            return paramPoArray.ToList();

        }

        //从list中获取packagecode中的paramcode对应的参数值
        public static String getParameter(String packageCode, String paramCode, List<paramPo> list)
        {
            var result = from d in list where d.packageCode == packageCode && d.paramCode == paramCode select d.paramValue;
            foreach (var v in result)
            {
                return v;
            }
            return null;
        }

        /// <summary>
        /// 将参数赋值会参数集合当中
        /// </summary>
        /// <param name="packageCode"></param>
        /// <param name="paramname"></param>
        /// <param name="value"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Boolean setParameter(String paramname, String value, List<Param> list) {
            Param param = (from d in list where d.cname.Equals(paramname) select d).FirstOrDefault();
            if (param == null)
            {
                return false;
            }
            else {
                param.value = value;
                return true;
            }
        }
    }
}
