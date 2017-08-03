using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ExcelUtils;
using Assistant.Container;
using Assistant.Entity;

namespace Assistant.Matcher
{
    public class Matcher
    {
        public const String ELEMENT_ID = "eid";

        public const String ELEMENT_TYPE = "type";

        public const String ELEMENT_STYLE = "style";

        public const String TYPE_CHECK_BOX = "checkbox";

        public const String TYPE_OTHER_CHECK_BOX = "othercheck";

        public const String TYPE_TEXT = "text";

        public const String TYPE_SELECT = "select";

        public const String VALUE_CHECKBOX = "checked";

        //配置序列号参数
        public static List<CarParams> carParams = new List<CarParams>();

        public static List<HtmlAttribute> standardCheckBoxAttribute = null;

        public static List<OtherCheckBoxAttritute> otherCheckBoxAttribute = null;

        public static List<HtmlAttribute> standardSelectAttribute = null;

        public static List<HtmlAttribute> standardTextAttribute = null;

        public static List<OtherSelectAttribute> otherSelectAttribute = null;

        public static void readAllCarParams(String ruleFile, String dataFile) {
            Hashtable ruleKey = new Hashtable();
            // 读取匹配数据信息
            String ruleFileName = @"D:\柔性参数填报系统\上海通用\rules.xls";
            ExcelInstance rules = ExcelUtil.readExcel(ruleFileName);
            int i = 0;
            String key = ExcelUtil.reader.getCellValue(i, 2, rules.dataTables[0]).Trim();
            while (key != null && key.Length > 0)
            {
                Hashtable properties = new Hashtable();
                properties.Add(Matcher.ELEMENT_ID, ExcelUtil.reader.getCellValue(i, 1, rules.dataTables[0]));
                properties.Add(Matcher.ELEMENT_TYPE, ExcelUtil.reader.getCellValue(i, 3, rules.dataTables[0]));
                ruleKey.Add(key, properties);
                i++;

                if (i < (rules.dataTables[0].Rows.Count))
                {
                    key = ExcelUtil.reader.getCellValue(i, 2, rules.dataTables[0]);
                }
                else
                {
                    break;
                }
            }

            //读取样车数量和packageCode
            String dataFileName = dataFile;
            String sheetName = "Sheet1";
            ExcelInstance data = ExcelUtil.readExcel(dataFileName);
            int rowcounter = data.dataTables[0].Rows.Count - 3;
            int counter = data.dataTables[0].Columns.Count - 3;

            for (int j = 3; j < counter + 3; j++) {
                CarParams c = new CarParams();
                String modelcode = ExcelUtil.reader.getCellValue(5, j, data.dataTables[0]);
                String packagemode = ExcelUtil.reader.getCellValue(6, j, data.dataTables[0]);
                String ordercode = ExcelUtil.reader.getCellValue(7, j, data.dataTables[0]);
                if (String.IsNullOrEmpty(modelcode)
                    || String.IsNullOrEmpty(packagemode)
                    || String.IsNullOrEmpty(ordercode)) {
                        continue;
                }
                c.packageCode = modelcode + packagemode + "-" + ordercode;
                carParams.Add(c);
            }

            
        }
        
        
        public static void readMatchRules(String ruleFile, String dataFile)
        {
            Hashtable ruleKey = new Hashtable();
            Matcher.standardCheckBoxAttribute = new List<HtmlAttribute>();
            Matcher.standardSelectAttribute = new List<HtmlAttribute>();
            Matcher.otherSelectAttribute = new List<OtherSelectAttribute>();
            Matcher.otherCheckBoxAttribute = new List<OtherCheckBoxAttritute>();
            Matcher.standardTextAttribute = new List<HtmlAttribute>();

            // 读取匹配数据信息
            String ruleFileName = "rules.xls";
            ExcelInstance rules = ExcelUtil.readExcel(ruleFileName);

            int i = 0;
            String key = ExcelUtil.reader.getCellValue(i, 2, rules.dataTables[0]).Trim();
            while ( key != null && key.Length > 0 )
            {
                //Console.WriteLine(key + (key.Equals("其它配置_其它")));

                Hashtable properties = new Hashtable();
                properties.Add(Matcher.ELEMENT_ID, ExcelUtil.reader.getCellValue(i, 1, rules.dataTables[0]));
                properties.Add(Matcher.ELEMENT_TYPE, ExcelUtil.reader.getCellValue(i, 3, rules.dataTables[0]));
                ruleKey.Add(key, properties);
                i++;

                if (i < (rules.dataTables[0].Rows.Count))
                {
                    key = ExcelUtil.reader.getCellValue(i, 2, rules.dataTables[0]);
                }
                else
                {
                    break;
                }
            }

            // 读取填报数据

            Console.WriteLine( "-----------华丽的分割线-----------" );

            String dataFileName = dataFile;
            String sheetName = "asdf";
            ExcelInstance data = ExcelUtil.readExcel(dataFileName, sheetName);

            int item = 0;
            int col = 1;
            int row = 2;
            key = ExcelUtil.reader.getCellValue(row, item, data.dataTables[0]);
            while ( true )
            {
                String value = ExcelUtil.reader.getCellValue(row, col, data.dataTables[0]).Trim();
                //包含eid和type两项
                Hashtable p = (Hashtable)ruleKey[key];

                if (key.Equals("电子巡航系统 （下拉菜单）"))
                {
                    Console.WriteLine("got!");
                }
                // 选项名称可直接匹配的
                if (p != null && value != null && value.Length > 0) 
                {
                    HtmlAttribute attr = new HtmlAttribute((String)p[Matcher.ELEMENT_ID],
                        key,
                        (String)p[Matcher.ELEMENT_TYPE],
                        value);

                    // 处理复选框
                    if (attr.elementType.Equals(Matcher.TYPE_CHECK_BOX))
                    {
                        Matcher.standardCheckBoxAttribute.Add(attr);
                    }
                    // 处理其他类型复选框
                    if (attr.elementType.Equals(Matcher.TYPE_OTHER_CHECK_BOX))
                    {
                        Matcher.standardCheckBoxAttribute.Add(
                            new HtmlAttribute(
                                (String)p[Matcher.ELEMENT_ID],
                                key, 
                                "style", 
                                "checked"));
                        // 其他项的具体内容
                        key = key + "内容";
                        p = (Hashtable)ruleKey[key];
                        Matcher.otherCheckBoxAttribute.Add(
                            new OtherCheckBoxAttritute(
                                (String)p[Matcher.ELEMENT_ID], 
                                key, 
                                Matcher.ELEMENT_STYLE, 
                                value,
                                "Div_"+(String)p[Matcher.ELEMENT_ID], 
                                "checked"));
                    }
                }
                else // 选项不可直接匹配类
                {
                    if (key.Contains("（下拉菜单）"))
                    {
                        String selectItem = key.Replace("（下拉菜单）","").Trim();
                        //这里替换掉“下拉菜单”重新获取eid和type
                        p = (Hashtable)ruleKey[selectItem];
                        Boolean got = false;
                        //一个select项内的遍历
                        while (!key.Equals("其他"))
                        {
                            row++;
                            key = ExcelUtil.reader.getCellValue(row, item, data.dataTables[0]);
                            value = ExcelUtil.reader.getCellValue(row, col, data.dataTables[0]);
                            if (!got)
                            {
                                if (value.Equals("S") && p != null)
                                {
                                    //如果是S的话就是有这个配置，如果有这个配置就把对应的key作为value。因为这是一个下拉菜单项
                                    HtmlAttribute attr = new HtmlAttribute((String)p[Matcher.ELEMENT_ID],
                                    selectItem,
                                    (String)p[Matcher.ELEMENT_TYPE],
                                    key);
                                    Matcher.standardSelectAttribute.Add(attr);
                                    got = !got;
                                    if ( key.Equals("其他")) // 其他项目内容
                                    {
                                        String selectElementId = (String)p[Matcher.ELEMENT_ID];
                                        p = (Hashtable)ruleKey[selectItem + "_其他"];
                                        Matcher.otherSelectAttribute.Add(
                                            new OtherSelectAttribute(
                                                selectElementId, 
                                                "电子巡航系统_其他",
                                                (String)p[Matcher.ELEMENT_ID],
                                                value));
                                    }
                                }
                            }
                        }
                    }
                }

                row++;
                if (row < data.dataTables[0].Rows.Count)
                {
                    key = ExcelUtil.reader.getCellValue(row, item, data.dataTables[0]);     
                }
                else 
                {
                    break;
                }          
            }
        }
    }
}
