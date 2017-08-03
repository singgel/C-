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
    class ImportServiceForHCBM
    {
        public static void importRuleData(String ruleFilePath)
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
                if (dicRules.ContainsKey(name) || String.IsNullOrEmpty(name))
                {
                    continue;
                }
                dicRules.Add(name, ha);
            }
            ParamsCollection.dicRules = dicRules;
        }
        /// <summary>
        /// 返回华晨宝马数据文件车型配置信息开始之前有多少行
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static int getRowCounterBeforeConfig(DataTable dt)
        {
            return 17;
        }
        /// <summary>
        /// 从datatable中寻找某个特定值，返回坐标数组
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private static int[] searchFromDataTable(DataTable dt, String target) {
            for(int j = 0; j < dt.Rows.Count; j++) {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    String value = dt.Rows[j][i].ToString();
                    if (value == target) {
                        return new int[] { j, i };
                    }
                }
            }

            return new int[] { -1, -1 };
        }

        public static void importCarsParams(String dataFilePath)
        {
            DataTable dt = ExcelUtils.ExcelUtil.getDataTableFromExcel(dataFilePath);

            int rowParamNameFirst = 4; //检查参数名称第一顺位行
            int rowParamNameSecond = 3; //检查参数名称第二顺位行
            int[] cursorPackageCode = searchFromDataTable(dt, "Model");
            ParamsCollection.dt = dt;
            Dictionary<String, HtmlAttribute> dicRules = ParamsCollection.dicRules;
            //所有车型配置信息的list
            List<CarParams> listCarParams = new List<CarParams>();
            int rowCounterBeforeConfig = getRowCounterBeforeConfig(dt);
            int carConfigCounter = dt.Rows.Count - rowCounterBeforeConfig;
            //针对每个配置信息获取对应参数
            for (int carConfigIter = 0; carConfigIter < carConfigCounter; carConfigIter++)
            {
                CarParams cps = new CarParams();
                List<paramPo> listParampo = new List<paramPo>();
                //配置所在行数
                int rowNo = carConfigIter + rowCounterBeforeConfig;
                //行号
                int columnNo = 0;

                //当没有到达最后一列的时候继续增加列数
                while (columnNo < dt.Columns.Count)
                {
                    String cname = null;
                    String value = null;
                    String cnameFirst = null;
                    String cnameSecond = null;
                    Param p = null;
                    //因为packageCode与其他参数在不同行，所以需要单独判断
                    if (cursorPackageCode[1] >= 0 && columnNo <= cursorPackageCode[1] + 2) {
                        int rowNoPackageCode = cursorPackageCode[0];
                        cname = dt.Rows[rowNoPackageCode][columnNo].ToString().Trim();
                        value = dt.Rows[rowNo][columnNo].ToString().Trim();
                        if (!String.IsNullOrEmpty(cname))
                        {
                            p = new EngineParam(cname, null, value);
                            cps.listParams.Add(p);
                            
                        }
                        columnNo++;
                        continue;
                    }
                    cnameFirst = dt.Rows[rowParamNameFirst][columnNo].ToString();
                    cnameSecond = dt.Rows[rowParamNameSecond][columnNo].ToString();
                    value = dt.Rows[rowNo][columnNo].ToString().Trim();
                    cname = null;
                    if (!String.IsNullOrEmpty(cnameFirst))
                    {
                        cname = cnameFirst.Trim();
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(cnameSecond))
                        {
                            cname = cnameSecond.Trim();
                        }
                        else
                        {
                            columnNo++;
                            continue;
                        }
                    }
                    p = new EngineParam(cname, null, value);
                    cps.listParams.Add(p);
                    columnNo++;
                }
                //获取配置号信息
                String packageCode = ImportServiceForHCBM.getParameter("Model", cps.listParams) + "_"
                    + ImportServiceForSHFY.getParameter("Package", cps.listParams) + "_"
                    + ImportServiceForSHFY.getParameter("Profile", cps.listParams);
                cps.packageCode = packageCode;
                listCarParams.Add(cps);
            }
            //将全部参数添加到参数合集当中
            ParamsCollection.carParams = listCarParams;
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
        public static Boolean setParameter(String paramname, String value, List<Param> list)
        {
            Param param = (from d in list where d.cname.Equals(paramname) select d).First();
            if (param == null)
            {
                return false;
            }
            else
            {
                param.value = value;
                return true;
            }
        }
    }
}
