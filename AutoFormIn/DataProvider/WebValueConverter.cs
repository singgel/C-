//#define FY

using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Assistant.DataProviders
{
    /// <summary>
    /// 表示一个数据转换工具，按照转换规则对数据进行转换。
    /// </summary>
    public class WebValueConverter : ValueConverter
    {
        private char[] _spliterChars;
        private Hashtable sheetNameRelation;
        private Hashtable mergeParameter;
        private System.Text.RegularExpressions.Regex mergeHelper;
        private List<ConverterParameter> _converter;
        private static readonly System.Text.RegularExpressions.Regex replaceItem, calcParameterMatcher, defaultTypeMatcher;

        private class SplitParameter
        {
            public char splitChar;
            public string[] values;
            public int index;

            public SplitParameter(string[] values)
            {
                this.values = values;
                index = -1;
            }
        }
        /// <summary>
        /// 定义多级数据的拆分字符。
        /// </summary>
        public char[] SpliterChars
        {
            get { return _spliterChars; }
            internal set
            {
                _spliterChars = value;
                StringBuilder pattern = new StringBuilder();
                pattern.Append("(?<value>[^");
                foreach (var item in this.SpliterChars)
                {
                    if (item == '.' || item == '$' || item == '-' || item == '\\' || item == '|' || item == '^' || item == '!')
                        pattern.Append("\\");
                    pattern.Append(item);
                }
                pattern.Append("]*)(?<symbol>.)?");
                mergeHelper = new System.Text.RegularExpressions.Regex(pattern.ToString());
            }
        }

        public WebValueConverter()
        {
            _converter = new List<ConverterParameter>();
            mergeParameter = new Hashtable();
            sheetNameRelation = new Hashtable();
            SpliterChars = new char[] { ';', '@', '$' };
        }

        static WebValueConverter()
        {
            defaultTypeMatcher = new System.Text.RegularExpressions.Regex(@"\{(?<type>[^\}]+)\}");
            calcParameterMatcher = new System.Text.RegularExpressions.Regex(@"\[(?<name>[^\]]+)\]");
            replaceItem = new System.Text.RegularExpressions.Regex(@"(?<origin>[^:]+):(?<actual>[^\|]+)\|?");
        }

        public override object Convert(string fileName, string sheetName, object parameters)
        {
            return Convert(fileName, sheetName, parameters as Hashtable);
        }

        public virtual System.Collections.Hashtable Convert(string fileName, string sheetName, System.Collections.Hashtable table)
        {
            Hashtable converted = new Hashtable();
            this.ReadConverterRule(fileName, sheetName);
            StringBuilder result = new StringBuilder();
            foreach (var item in _converter)
            {
                if (item.UseOriginString == false)
                {
                    // 若不使用原始值
                    if (item.IsSheetRelation && string.IsNullOrEmpty(item.OriginName) == false)
                    {
                        sheetNameRelation.Add(item.OriginName, item.EntName);
                        continue;
                    }

                    if (string.IsNullOrEmpty(item.EntName))
                        item.EntName = item.OriginName;
                    // 计算表达式类型转换规则
                    if (string.IsNullOrEmpty(item.CalcExpression) == false)
                    {
                        converted.Add(item.OriginName, GetExpressionResult(item, table, converted));
                        continue;
                    }
                    // 获取原始值
                    string value = this.GetValue(table[item.EntName], item);
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        result.Clear();
                        this.GetValue(value, item, result);

                        if (converted.ContainsKey(item.OriginName))
                            converted.Remove(item.OriginName);

                        string str = result.ToString();
                        // str为空时返回默认值
                        //str = string.IsNullOrEmpty(str) ? item.DefaultValue : str;
                        converted.Add(item.OriginName, str);
                    }
                }
                else
                    converted.Add(item.OriginName, this.GetValue(table[item.EntName], item));

                if (string.IsNullOrEmpty(item.MergePara) == false)  // 待合并的参数
                    RecordMergeParameter(item);

                if (mergeParameter.Contains(item.EntName))
                {
                    ConverterParameter parameter = mergeParameter[item.EntName] as ConverterParameter;
                    if (parameter != null)
                        converted[parameter.OriginName] = MergeParameter(parameter, converted, table);
                }

                if (item.IsRequired)
                {
                    string val = converted[item.OriginName] as string;
                    if (string.IsNullOrEmpty(val) && string.IsNullOrEmpty(item.DefaultValue) == false)
                    {
                        if (string.IsNullOrEmpty(item.DefaultValue) == false)
                        {
                            System.Text.RegularExpressions.Match match = defaultTypeMatcher.Match(item.DefaultValue);
                            val = "";
                            if (match.Success)
                            {
                                switch (match.Groups["type"].Value.ToUpper())
                                {
                                    case "RANDOM":
                                        val = System.Guid.NewGuid().ToString();
                                        val = val.Substring(val.Length - 10, 10);
                                        break;
                                }
                            }
                            else
                                val = item.DefaultValue;
                            converted[item.OriginName] = val;
                        }
                    }
                }
#if  FY
                if (string.IsNullOrEmpty(str))
                {
                    converted[item.OriginName] = TestFile(item.EntName);
                }
#endif
            }
            return converted;
        }

        internal virtual string GetValue(object value, ConverterParameter parameter)
        {
            return value as string;
        }

        public string GetSheetName(string originName)
        {
            if (sheetNameRelation.ContainsKey(originName))
                return sheetNameRelation[originName] as string;
            else
                return originName;
        }

        private void GetValue(string originValue, ConverterParameter para, StringBuilder result)
        {
            Hashtable replaceTable = GetReplace(para.ValueReplaceGroup);
            if (string.IsNullOrEmpty(para.SplitParten))
            {
                result.Append(GetValue(originValue, para, replaceTable));
                return;
            }
            else
                SplitValue(originValue, para, result, replaceTable);
        }

        private string GetValue(string value, ConverterParameter para, Hashtable replaceTable)
        {
            // 使用转换参数的拆分正则匹配value，并返回匹配到的值，若匹配失败则返回原字符串。
            string str;
            if (string.IsNullOrEmpty(para.RegexParten))
                str = value;
            else
            {
                System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(value, para.RegexParten);
                str = match.Success ? match.Value : "";
            }
            if (replaceTable == null || string.IsNullOrEmpty(str) || replaceTable.ContainsKey(str) == false)
                return str;
            else
                return replaceTable[str] as string;
        }

        private void SplitValue(string originValue, ConverterParameter para, StringBuilder result, Hashtable replaceTable)
        {
            // 按填报级数分割符逐级拆分originValue
            Stack<SplitParameter> stack = new Stack<SplitParameter>();
            char[] spliters = para.SplitParten.ToCharArray();
            int splitDepth = spliters.Length;
            SplitParameter current = new SplitParameter(originValue.Split(spliters[splitDepth - 1]));
            string currentValue = "";
            do
            {
                for (int index = 0; index < current.values.Length; index++)
                {
                    if (splitDepth > 1)
                    {
                        current.index = index;
                        stack.Push(current);
                        currentValue = current.values[index];
                        splitDepth--;
                        current = new SplitParameter(currentValue.Split(spliters[splitDepth - 1]));
                        index = -1;
                    }
                    else
                    {
                        string str = GetValue(current.values[index], para, replaceTable);
                        result.Append(string.Format("{0}{1}", str, this.SpliterChars[0]));
                    }
                    if (current.values.Length - 1 == index)
                    {
                        result.Remove(result.Length - 1, 1);
                        SplitParameter t = stack.Count >= 1 ? stack.Peek() : null;
                        if (t != null)
                        {
                            if (t.values.Length - 1 > t.index)
                                result.Append(this.SpliterChars[splitDepth]);
                            current = stack.Pop();
                        }
                    }
                }
            } while (stack.Count > 0);
        }
        /// <summary>
        /// 从替换表达式中获取被替换值、替换值的键值对散列表。
        /// </summary>
        /// <param name="replaceGroup">替换表达式</param>
        /// <returns></returns>
        private Hashtable GetReplace(string replaceGroup)
        {
            if (string.IsNullOrEmpty(replaceGroup))
                return null;
            System.Text.RegularExpressions.Match match = replaceItem.Match(replaceGroup);
            Hashtable table = new Hashtable();
            while (match.Success)
            {
                if (string.IsNullOrEmpty(match.Groups["origin"].Value))
                    continue;
                else
                    table.Add(match.Groups["origin"].Value, match.Groups["actual"].Value);
                match = match.NextMatch();
            }
            return table;
        }

        private void RecordMergeParameter(ConverterParameter parameter)
        {
            string[] parameters = parameter.MergePara.Split(new char[] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in parameters)
            {
                if (mergeParameter.ContainsKey(str) == false)
                    mergeParameter.Add(str, parameter);
            }
        }

        private string GetExpressionResult(ConverterParameter parameter, Hashtable table, Hashtable converted)
        {
            StringBuilder result = new StringBuilder();
            System.Text.RegularExpressions.Match match = calcParameterMatcher.Match(parameter.CalcExpression);
            int startIndex = 0;
            while (match.Success)
            {
                result.Append(parameter.CalcExpression.Substring(startIndex, match.Index - startIndex));
                string name = match.Groups["name"].Value;
                string val = "";
                if (converted.Contains(name))
                    val = converted[name] as string;
                else if (table.Contains(name))
                    val = this.GetValue(table[name], parameter);
                if (string.IsNullOrEmpty(val))
                    return "";
                result.Append(val);
                startIndex = match.Index + match.Length;
                match = match.NextMatch();
            }
            result.Append(parameter.CalcExpression.Substring(startIndex));
            string expr = result.ToString();
            if (Applications.Maths.FormulaHelper.IsValidExpression(expr) == false)
                return "";
            return Applications.Maths.FormulaHelper.Calculate(expr).ToString("0.000");
        }

        // 此合并方法仅支持多级参数合并时所有参数长度均相同的情况
        private string MergeParameter(ConverterParameter parameter, Hashtable convertedData, Hashtable originData)
        {
            StringBuilder result = new StringBuilder();
            string[] parameters = parameter.MergePara.Split(new char[] { '&' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<string> strList = new List<string>();

            foreach (var str in parameters)
            {
                if (mergeParameter.ContainsKey(str))
                {
                    if (convertedData.ContainsKey(str))
                        strList.Add(convertedData[str] as string);
                    else
                        strList.Add(this.GetValue(originData[str], parameter));
                }
            }
            // 若合并所需的参数名称不全，则回滚所有操作
            if (strList.Count != parameters.Length)
            {
                foreach (var str in parameters)
                {
                    if (mergeParameter.ContainsKey(str) == false)
                        mergeParameter.Add(str, parameter);
                }
                return "";
            }

            List<System.Text.RegularExpressions.Match> matchList = new List<System.Text.RegularExpressions.Match>();
            foreach (var item in strList)
            {
                if(item != null)
                    matchList.Add(mergeHelper.Match(item));
            }

            bool merge = false;

            string lastSymbol;
            do
            {
                lastSymbol = "";
                for (int i = 0; i < matchList.Count; i++)
                {
                    if (matchList[i].Success)
                    {
                        if (string.IsNullOrEmpty(lastSymbol) || lastSymbol == matchList[i].Groups["symbol"].Value)
                        {
                            lastSymbol = matchList[i].Groups["symbol"].Value;
                            result.Append(matchList[i].Groups["value"].Value);
                            //if (string.IsNullOrEmpty(parameter.MergeSpliter) == false)
                            //    result.Append(parameter.MergeSpliter);
                            matchList[i] = matchList[i].NextMatch();
                        }
                        else if (matchList[i].Groups["symbol"].Value != lastSymbol)
                            matchList[i] = matchList[i].NextMatch();

                        if (string.IsNullOrEmpty(parameter.MergeSpliter) == false && i != matchList.Count - 1)
                            result.Append(parameter.MergeSpliter);
                    }
                }
                result.Append(lastSymbol);
            } while (merge);
            return result.ToString();
        }

        private void ReadConverterRule(string fileName, string sheetName)
        {
            _converter.Clear();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(fileName))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(sheetName) as Office.Excel.ForwardReadWorksheet;
                if (sheet == null)
                    return;
                Hashtable columnHeader = new Hashtable();
                if (sheet.ReadNextRow())
                {
                    while (sheet.ReadNextCell(false))
                    {
                        columnHeader.Add(sheet.CurrentCell.ColumnIndex, sheet.GetContent());
                    }
                }
                string str;
                object content;
                while (sheet.ReadNextRow())
                {
                    ConverterParameter para = new ConverterParameter();
                    while (sheet.ReadNextCell(false))
                    {
                        content = sheet.GetContent();
                        str = content == null ? "" : content.ToString();
                        switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                        {
                        case "字符替换":
                            para.ValueReplaceGroup = str;
                            break;
                        case "企业内部参数编号":
                            para.EntName = str;
                            break;
                        case "原参数名称":
                            para.OriginName = str;
                            break;
                        case "填报级数分割符":
                            para.SplitParten = str;
                            break;
                        case "参数拆分正则":
                            para.RegexParten = str;
                            break;
                        case "是否为表格":
                            para.IsSheetRelation = str == "是";
                            break;
                        case "参数合并":
                            para.MergePara = str;
                            break;
                        case "参数合并时使用的分割符号":
                            para.MergeSpliter = str;
                            break;
                        case "默认值":
                            para.DefaultValue = str;
                            break;
                        case "是否使用原始内容":
                            para.UseOriginString = str == "是";
                            break;
                        case "计算表达式":
                            para.CalcExpression = str;
                            break;
                        case "是否必填":
                            para.IsRequired = str == "是";
                            break;
                        case "元素类型":
                            para.ElementType = str;
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(para.OriginName))
                        continue;
                    _converter.Add(para);
                }
            }
        }
#if FY
        private string TestFile(string name)
        {
            if (name.Length <= 5)
                return "";
            name = name.Substring(3, name.Length - 5);
            string result;
            foreach (var ext in exts)
            {
                result = string.Format("{0}{1}{2}", this.DataFilePath, name, ext);
                if (System.IO.File.Exists(result))
                    return result;
            }
            return "";
        }
#endif
    }
}
