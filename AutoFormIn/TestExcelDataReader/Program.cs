using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelUtils;
using Assistant;
using Assistant.Entity;
using System.Data;
using LoggerUtils;
using System.Collections;
using Assistant.Container;
using Assistant.Matcher;
using Assistant.ConfigCodeService;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using EntityLib;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;

namespace TestExcelDataReader
{
    class Program
    {
        [STAThread]
        static void Main_form()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainRibbonForm2());
        }
        static void Main_regex(string[] args) {
            String s = "保存成功,临时序列号为：32052011027001。如需提交至中机中心，请在未提交配置中提交。";
            String t = "错误0为sdfsdfsdfs";
            Regex reg = new Regex(@"\d{10,}");
            String u = reg.Match(s).ToString();
            Boolean b = reg.IsMatch(s);
            LoggerUtils.LoggerUtils.logger(u);
            LoggerUtils.LoggerUtils.logger(b.ToString());
        }

        static void Main_Date(String[] args) {
            DateTime now = DateTime.Now;
            LoggerUtils.LoggerUtils.logger(now.ToString(("yyyy-MM-dd HH:mm:ss")));
            DateTime timeline = DateTime.Parse("2010-07-01 12:00:00");
            if (now < timeline)
            {
                LoggerUtils.LoggerUtils.logger("尚未到期");
            }
            else {
                LoggerUtils.LoggerUtils.logger("已经到期");
            }
        }

        static void Main_join(String[] args) {
            //String s = "驾驶员及前排乘客安全气囊,    ，前后排头部气囊，驾驶员及前排乘客侧面安全气囊，胎压监测，防爆轮胎，ISO FIX儿童座椅接口，车内中控锁、遥控钥匙";
            String s = "wewewerwerwer";
            String[] separator = new String[] { "，", "、", "," };
            List<String> list = s.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList();
            LoggerUtils.LoggerUtils.logger(list.ToString());
        
        }

        static void Main_past(String[] args) {
            String filePath = @"D:\柔性参数填报系统\华晨宝马\2014.12配置信息填报规则 BBA -Updated in sequence 20141222_F30F35 0115  Semi - Copy.xls";
            FileStream excelFileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite);
            IWorkbook workbook = new HSSFWorkbook(excelFileStream);
            excelFileStream.Close();
            ISheet sheet = workbook.GetSheetAt(0);//取第一个表
            ICell cell = sheet.GetRow(291).GetCell(0);
            if (cell == null)
            {
                cell = sheet.GetRow(291).CreateCell(0, CellType.String);
                cell.SetCellValue("test");
            }
            else
            {
                cell.SetCellValue("test");
            }
            FileStream fs = File.Create(filePath);
            workbook.Write(fs);
            fs.Close();
        }

        static void Main_Stringtest(String[] args) {
            String s = "a、b、c";
            bool f = s.Contains("");
            //f = s.Contains(null);
            String[] arr = s.Split(new char[] { '、' }, StringSplitOptions.RemoveEmptyEntries);
            s = String.Join("、", arr);

            String s1 = "test";
            Hashtable h = new Hashtable();
            h.Add("s", new Entry("s"));
            Entry entry = h["s"] as Entry;
            changeString(ref s1);
            changeEntry(entry);
            Console.WriteLine(s1);

            FileInfo fi = new FileInfo(@"C:\1.txt");
            String s2 = fi.Name;

            String s3 = "adfasdfadsf_1";
            int index;
            if ((index = s3.IndexOf("_")) >= 0) { 
                s3 = s3.Substring(0, index);        
            }
        }

        static void Main(String[] args)
        {
            IWorkbook workbook;
            String filePath = @"C:\配置号数据文件.xlsx";
            string fileExt = Path.GetExtension(filePath);
            {
                using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fileExt == ".xls")
                    {
                        workbook = new HSSFWorkbook(file);
                    }
                    else if (fileExt == ".xlsx")
                    {
                        workbook = new XSSFWorkbook(file);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if (workbook != null) {
                ISheet sheet = workbook.GetSheetAt(0);
                int firstRowNo = sheet.FirstRowNum;
                int lastRowNo = sheet.LastRowNum;//返回行号 比较小
                int cellcount = sheet.GetRow(0).LastCellNum; //返回列号+1

            
            
            }

        }

        public class Entry {
            public Entry(String s) {
                this.s = s;
            }
            public String s;
        }

        public static void changeString(ref String s) {
            s += "append";
            changeString2(ref s);
        }

        public static void changeString2(ref String s){
            s += "append2";
        
        }

        public static void changeEntry(Entry s) {
            s.s = "test";
        
        }

        static void Main_service(string[] args)
        {
            //读取rule.xls配置文件，形成map包含填报全部参数
            DataTable dtRules = ExcelUtils.ExcelUtil.getDataTableFromExcel(@"D:\柔性参数填报系统\上海通用\rules.xls");
            LoggerUtils.LoggerUtils.logger(dtRules.Rows.Count.ToString());
            Dictionary<String, HtmlAttribute> dicRules = new Dictionary<String, HtmlAttribute>();
            int totalRowRules = dtRules.Rows.Count;
            for (int rownoRules = 1; rownoRules < totalRowRules; rownoRules++) {
                String name = ExcelUtil.reader.getCellValue(rownoRules, 2, dtRules).Trim();
                String eid = ExcelUtil.reader.getCellValue(rownoRules, 1, dtRules).Trim();
                String etype = ExcelUtil.reader.getCellValue(rownoRules, 3, dtRules).Trim();
                HtmlAttribute ha = new HtmlAttribute(eid, name, etype, null);
                dicRules.Add(name, ha);
            }

            String engineInfoStartString = "车辆名称";
            String configInfoStartString = "MD配置信息";
            String patacCodeInitial = "P";
            DataTable dt = ExcelUtils.ExcelUtil.getDataTableFromExcel(@"D:\柔性参数填报系统\上海通用\data.xls");
            //所有车型配置信息的list
            List<CarParams> listCarParams = new List<CarParams>();
            LoggerUtils.LoggerUtils.logger(dt.Columns.Count.ToString());
            int columnCounterBeforeConfig = 3;
            int carConfigCounter = dt.Columns.Count - columnCounterBeforeConfig;
            //针对每个配置信息获取对应参数
            for (int carConfigIter = 0; carConfigIter < carConfigCounter; carConfigIter++) {
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
                while (!cname.Equals("车辆名称")) {
                    rowno++;
                    cname = dt.Rows[rowno][0].ToString();
                    if (rowno >= dt.Rows.Count) {
                        throw new Exception("已经超过最大行，还未找到车辆名称");
                    }
                }
                //从车辆名称到燃料种类
                while (!cname.Equals("部门会签")) {
                    if (String.IsNullOrEmpty(cname)) {
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
                String packageCode = getParameter("Model Code", cps.listParams) 
                    + getParameter("Package Code", cps.listParams) + "-"
                    + getParameter("Order Sample", cps.listParams);
                cps.packageCode = packageCode;

                //通过配置号信息获取工程信息数据
                List<String> listPackageCode = new List<string>();
                if(!String.IsNullOrEmpty(packageCode)){
                    listPackageCode.Add(packageCode);
                    listParampo = getParamsByPackageCode(listPackageCode);
                }
                foreach (Param p in cps.listParams) { 
                    //只有型式认证编码有效的才能够获取数据
                    if (p.patacode.StartsWith(patacCodeInitial)) {
                        String paramValue = getParameter(cps.packageCode, p.patacode, listParampo);
                        p.value = "test";
                        if (String.IsNullOrEmpty(paramValue)) {
                            continue;
                        }
                    }
                }
                //获取是否需要申报配置号信息
                String needConfig = getParameter("是否申报新配置", cps.listParams);
                if (needConfig.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    cps.needConfigCode = true;
                }
                else {
                    cps.needConfigCode = false;
                }

                //获取配置信息
                
                //从部门会签到MD配置信息
                while (!cname.Equals("MD配置信息"))
                {
                    rowno++;
                    cname = dt.Rows[rowno][0].ToString();
                }

                //对配置参数进行转化,转化为参数列表(listParams而不是listHtmlAttributes)
                while (rowno < dt.Rows.Count) {
                    cname = dt.Rows[rowno][0].ToString().Trim();
                    value = dt.Rows[rowno][columnNo].ToString().Trim();
                    if(String.IsNullOrEmpty(cname)){
                        rowno++;
                        continue;
                    }
                    HtmlAttribute ha = null;
                    if (dicRules.ContainsKey(cname)) { 
                        ha = dicRules[cname];
                    }
                    //当能够找到对应参数而且参数值不为空的时候才进行
                    //适用于能够直接找到对应参数名称的情况
                    if (ha != null && !String.IsNullOrEmpty(value))
                    {
                        //处理text类型和checkbox类型
                        if (ha.elementType.Equals(Matcher.TYPE_TEXT) || ha.elementType.Equals(Matcher.TYPE_CHECK_BOX))
                        {

                            Param p = new Param(cname, null, Matcher.VALUE_CHECKBOX);
                            cps.listParams.Add(p);
                        }
                        //处理othercheckbox类型
                        else if (ha.elementType.Equals(Matcher.TYPE_OTHER_CHECK_BOX))
                        {
                            Param p1 = new Param(cname, null, Matcher.VALUE_CHECKBOX);
                            cps.listParams.Add(p1);
                            if (dicRules.ContainsKey(cname + "内容"))
                            {
                                Param p2 = new Param(cname + "内容", null, value);
                                cps.listParams.Add(p2);
                            }
                        }
                    }
                    else if(cname.Contains("下拉菜单")){
                        cname = cname.Replace("（下拉菜单）", "").Trim();
                        //如果不包含直接下一个参数U
                        if (!dicRules.ContainsKey(cname)) {
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
                //通过接口将型式认证参数导入
                //将参数转化为实际填报参数
                cps.listHtmlAttributes.Clear();
                cps.listHtmlAttributes.AddRange(getAllHAInACar(cps.listParams, dicRules));


                StringBuilder sb = new StringBuilder();
                foreach (Param p in cps.listParams)
                {
                    sb.Append(p.cname + "---" + p.value + "\r\n");
                }
                LoggerUtils.LoggerUtils.loggerTxt(@"d:\test.txt", sb.ToString());
                sb.Clear();
                foreach (HtmlAttribute ha in cps.listHtmlAttributes)
                {
                    sb.Append(handleHtmlAttributes(ha));
                }
                LoggerUtils.LoggerUtils.loggerTxt(@"d:\test2.txt", sb.ToString());
            }
        }

        public static List<HtmlAttribute> transformFromParamToHA(Param param, Dictionary<String, HtmlAttribute> dic)
        {
            String cname = param.cname;
            if (String.IsNullOrEmpty(cname)) {
                return null;
            }
            if (!dic.ContainsKey(cname)) {
                return null;
            }
            HtmlAttribute ha = dic[cname];
            HtmlAttribute realHtmlParam = null;
            List<HtmlAttribute> result = new List<HtmlAttribute>();
            if(ha.elementType.Equals(Matcher.TYPE_TEXT) || (ha.elementType.Equals(Matcher.TYPE_CHECK_BOX))){
                if (!param.value.EndsWith("_其他"))
                {
                    realHtmlParam = new HtmlAttribute(ha.elementId, ha.name, ha.elementType, param.value);
                    result.Add(realHtmlParam);
                }
                else {
                    if (ha.elementType.Equals(Matcher.TYPE_TEXT)) {
                        result.Add(new OtherSelectAttribute("test", ha.name, ha.elementId, param.value));
                    }
                }
                
            }
            //othercheckbox类型
            else if (ha.elementType.Equals(Matcher.TYPE_OTHER_CHECK_BOX)) {
                //没有内容二字的只是普通checkbox
                if (!ha.name.Contains("内容"))
                {
                    result.Add(new HtmlAttribute(ha.elementId, ha.name, ha.elementType, "checked"));
                }
                //包含内容二字就包括单元格了
                else {
                    result.Add(new OtherCheckBoxAttritute(ha.elementId, ha.name , Matcher.ELEMENT_STYLE, param.value,
                                "Div_" + ha.elementId,
                                "checked"));
                }
            }
            //select类型
            else if (ha.elementType.Equals(Matcher.TYPE_SELECT)) {
                result.Add(new HtmlAttribute(ha.elementId, ha.name, ha.elementType, param.value));
            }

            return result;
        }

        //根据类型处理htmlAttribute
        public static String handleHtmlAttributes(HtmlAttribute ha){
            StringBuilder sb = new StringBuilder();
            if(ha is OtherCheckBoxAttritute){
                sb.Append(ha.name + "  " + "OtherCheckBoxAttritute"+"\r\n");
            
            
            }else if(ha is OtherSelectAttribute){
                sb.Append(ha.name + "  " + "OtherSelectAttribute" + "\r\n");
            
            
            
            }else{
                if(ha.elementType.Equals(Matcher.TYPE_TEXT)){
                    sb.Append(ha.name + "  "  + Matcher.TYPE_TEXT + "\r\n");

                }
                else if (ha.elementType.Equals(Matcher.TYPE_CHECK_BOX) || ha.elementType.Equals(Matcher.TYPE_OTHER_CHECK_BOX))
                {
                    sb.Append(ha.name + "  " + Matcher.TYPE_CHECK_BOX + "\r\n");
                
                }
                else if (ha.elementType.Equals(Matcher.TYPE_SELECT)) {
                    sb.Append(ha.name + "  " + Matcher.TYPE_SELECT + "\r\n");
                
                }
            }
            return sb.ToString();
        }


        public static List<HtmlAttribute> getAllHAInACar(List<Param> listParam, Dictionary<String, HtmlAttribute> dic)
        {
            List<HtmlAttribute> result = new List<HtmlAttribute>();
            foreach (Param p in listParam) {
                List<HtmlAttribute> list = transformFromParamToHA(p, dic);
                if(list != null){
                    result.AddRange(list);
                }
                
            }
            return result;
        }



        public void getGMDataExcel()
        {
            ExcelInstance data = ExcelUtil.readExcel(@"D:\柔性参数填报系统\上海通用\data.xls");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.dataTables[0].Rows.Count; i++)
            {
                for (int j = 0; j < data.dataTables[0].Columns.Count; j++)
                {
                    sb.Append(ExcelUtil.reader.getCellValue(i, j, data.dataTables[0]) + " ");
                }
                sb.AppendLine("");

            }
            Console.WriteLine(sb.ToString());
            Console.ReadLine();
        }

        //从参数list中获取名称为name的参数
        public static String getParameter(String name, List<Param> list) {
            var result = from d in list where d.cname == name select d.value;
            foreach (var v in result)
            {
                //直接插入空串会让合并域消失，所以换成多个空格
                return v;
            }
            return null;
        }

        //根据packagecode获取参数
        public static List<paramPo> getParamsByPackageCode(List<String> pcode) {
            IPackageInfoServiceService service = new IPackageInfoServiceService();
            paramPo[] paramPoArray = service.getParamsByPackageCode(pcode.ToArray());
            return paramPoArray.ToList();
        }

        //从list中获取packagecode中的paramcode对应的参数值
        public static String getParameter(String packageCode, String paramCode, List<paramPo> list){
            var result = from d in list where d.packageCode == packageCode && d.paramCode == paramCode select d.paramValue;
            foreach (var v in result)
            {
                return v;
            }
            return null;
        }




    }
}
