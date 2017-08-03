using Assistant.DataProviders;
using mshtml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Detectors;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    class FDLFiller : FillBase
    {
        private int _formIndex;
        private UrlParameter _urlParameter;
        private static readonly string[] emptyStringArray = new string[]{};
        private List<KeyValuePair<FillParameterKey, object>> _parameters;
        private const string engineModel = "发动机型号", mainVehicleModel = "主车型型号", formName = "frm_part3";

        public static string FillType
        {
            get { return "非道路机动车"; }
        }

        public string DefaultValue
        {
            get;
            internal set;
        }

        /// <summary>
        /// 获取当前排放标准。
        /// </summary>
        public string Standard
        {
            get { return DataTable == null ? null : DataTable["排放标准"] as string; }
        }
        /// <summary>
        /// 获取当前填报车型。
        /// </summary>
        public string CarType
        {
            get { return DataTable == null ? null : DataTable["申报车型"] as string; }
        }

        public FDLFiller(WebBrowser2 browser, Uri currentUri, UrlParameter urlParameter)
            : base(browser, currentUri)
        {
            _parameters = new List<KeyValuePair<FillParameterKey, object>>();
            _urlParameter = urlParameter;
        }

        protected override void BeginFillProtected()
        {
            if (base.Document == null)
                return;
            //获取当前页面的填报规则
            this.GetParameters();
            _formIndex = -1;
            Hashtable data = InitFillData();
            // 存储可新增/可查找的数据
            List<KeyValuePair<FillParameterKey, object>> list = new List<KeyValuePair<FillParameterKey, object>>();
            List<string> values = new List<string>();
            foreach (KeyValuePair<FillParameterKey, object> parameter in _parameters)
            {
                string parameterValue = string.IsNullOrEmpty(parameter.Key.Key) ? "" : data[parameter.Key.Key] as string; // 将向网页中填报的数据。
                if (string.IsNullOrEmpty(parameterValue) && (parameter.Key.Key == null || data.ContainsKey(parameter.Key.Key) == false))
                {
                    if (parameter.Key.Type != Matcher.TYPE_A && parameter.Key.Type != Matcher.TYPE_BUTTON && parameter.Key.Type != Matcher.TYPE_FORM
                        && parameter.Key.Type != Matcher.TYPE_SUBMIT && parameter.Key.Type != "BUTTON/SUBMIT" )
                        base.FillRecords.Add(new FillRecord(GetElementType(parameter.Key.Type), RecordType.Failed, "数据文件中不包含此参数", parameter.Key.Key));
                }
                // 当前数据为表格数据
                //if (string.IsNullOrEmpty(parameter.Key.TableName) == false || string.IsNullOrEmpty(lastTableName) == false)
                //{
                //    if (lastTableName == parameter.Key.TableName || string.IsNullOrEmpty(lastTableName))
                //    {
                //        lastTableName = parameter.Key.TableName;
                //        list.Add(parameter);
                //        continue;
                //    }
                //    else
                //    {
                //        FillTableValue(GetTableData(lastTableName), list);
                //        list.Clear();
                //        values.Clear();
                //        if (string.IsNullOrEmpty(parameter.Key.TableName) == false)
                //        {
                //            list.Add(parameter);
                //            lastTableName = parameter.Key.TableName;
                //            continue;
                //        }
                //        else
                //            lastTableName = "";
                //    }
                //}
                //else 
                if (parameter.Key.CanAdd)
                {
                    list.Add(parameter);
                    values.Add(parameterValue);

                    if (parameter.Key.Type == Matcher.TYPE_SUBMIT || parameter.Key.Type == "BUTTON/SUBMIT")
                        FillListElement(list, values);
                    continue;
                }
                else if (string.IsNullOrEmpty(parameter.Key.SearchString) == false)
                {
                    list.Add(parameter);
                    if (parameter.Key.Type == Matcher.TYPE_SUBMIT || parameter.Key.Type == "BUTTON/SUBMIT" || parameter.Key.Type == Matcher.TYPE_BUTTON)
                    {
                        if (SearchMatch(parameter.Value as FillParameter, list, values))
                            continue;
                        else
                            this.Reset();
                    }
                    values.Add(parameterValue);
                    continue;
                }
                if (list.Count != 0)
                    FillListElement(list, values);
                FillElement(parameter.Key, parameter.Value, parameterValue);
            }
        }

        private Hashtable InitFillData()
        {
            if (FillIndexes.Count == 0)  
                return DataTable;
            Hashtable data = new Hashtable();
            foreach (var item in _parameters)
            {
                if (item.Key.Key == null || data.ContainsKey(item.Key.Key) || item.Key.Type == Matcher.TYPE_FORM || item.Key.Type == Matcher.TYPE_A
                    || item.Key.Type == Matcher.TYPE_SUBMIT || item.Key.Type == "BUTTON/SUBMIT")
                    continue;
                data.Add(item.Key.Key, GetMultiFillValue(DataTable[item.Key.Key] as string, item.Key.CanAdd));
            }
            return data;
        }

        protected override void FillElement(FillParameter parameter, string value)
        {
            if (parameter == null)
                return;
            if (parameter.ParameterName == null || base.DataTable.Contains(parameter.ParameterName) == false)
                value = DefaultValue;
            if (parameter.Type != Matcher.TYPE_FORM)
            {
                if (parameter.Value == "上传图片")
                {
                    if (string.IsNullOrEmpty(value))
                        return;
                    string file = System.IO.File.Exists(value) ? value : (string.IsNullOrEmpty(base.DataFilePath) ? value : string.Format("{0}\\{1}", base.DataFilePath, value));
                    if (System.IO.File.Exists(file) == false)
                    {
                        base.FillRecords.Add(new FillRecord(ElementType.File, RecordType.Failed, string.Format("文件{0}不存在", file), parameter.ParameterName));
                        return;
                    }
                    else
                        DataTable["选择文件"] = value;
                }
            }
            base.FillElement(parameter, value);
        }
        ///// <summary>
        ///// 填报数据
        ///// </summary>
        ///// <param name="parameterKey">参数名称。</param>
        ///// <param name="elementContainer">用于从网页中查找元素的对象，当value为Hashtable时表示当前是一组Radio或CheckBox。</param>
        ///// <param name="parameterValue">用于填报的数据。</param>
        ///// <param name="list">用于存储多值数据的列表。</param>
        //private void FillElement(FillParameterKey parameterKey, object elementContainer, string parameterValue)
        //{
        //    this.Wait();
        //    FillParameter fillParameter = elementContainer as FillParameter;
        //    if (fillParameter == null)
        //    {
        //        Hashtable table = elementContainer as Hashtable;
        //        if (table == null)
        //            return;
        //        else if (parameterKey.Type == Matcher.TYPE_RADIO)
        //        {
        //            // 处理radio类型的选择
        //            if (parameterValue != null)
        //            {
        //                if (table.ContainsKey(parameterValue) == false)
        //                    base.FillRecords.Add(new FillRecord(ElementType.Radio, RecordType.Failed, string.Format("单选框中不包含项 {0}", parameterValue), parameterKey.Key));

        //                fillParameter = table[parameterValue] as FillParameter;
        //            }
        //        }
        //        else if (parameterKey.Type == Matcher.TYPE_CHECKBOX) // 处理checkBox类型
        //        {
        //            FillCheckBoxGroup(parameterValue, parameterKey.Key, table);
        //            return;
        //        }
        //    }
        //    FillElement(fillParameter, parameterValue);
        //}

        private void FillListElement(List<KeyValuePair<FillParameterKey, object>> list, List<string> values)
        {
            if (list.Count == 0)
                return;
            if (list.Count == 1 && (list[0].Key.Type == Matcher.TYPE_SUBMIT || list[0].Key.Type == "BUTTON/SUBMIT"
                || list[0].Key.Type == Matcher.TYPE_BUTTON))
            {
                FillListElementUncertain(list[0].Value as FillParameter);
                list.Clear();
                values.Clear();
                return;
            }
            string[] valueArray = null;
            int maxValueLength = 0;
            List<string[]> valueList = new List<string[]>();
            foreach (var value in values)
            {
                valueArray = value == null ? emptyStringArray : value.Split(base.Converter.SpliterChars[0]);
                valueList.Add(valueArray);
                maxValueLength = Math.Max(maxValueLength, valueArray.Length);
            }
            base.CurrentIndex = 0;
            FillIndexes.Add(this);
            for (int valueIndex = 0; valueIndex < maxValueLength; valueIndex++)
            {
                base.CurrentIndex = valueIndex;  // 更新当前填报索引
                FillParameterKey key = null;
                object elementContainer = null;
                string parameterValue = "";
                for (int i = 0; i < list.Count; i++)
                {
                    key = list[i].Key;
                    elementContainer = list[i].Value;
                    if (list[i].Key.Type == Matcher.TYPE_SUBMIT || list[i].Key.Type == "BUTTON/SUBMIT")
                        parameterValue = null;
                    else if (valueList[i].Length > valueIndex)
                        parameterValue = valueList[i][valueIndex];
                    else
                        parameterValue = valueList[i].Length > 0 ? valueList[i][valueList[i].Length - 1] : ""; // 若当前无对应的填报参数，则使用最后一个可用的填报值
                    FillElement(key, elementContainer, parameterValue);
                }
                this.Wait();
            }
            list.Clear();
            values.Clear();
            FillIndexes.RemoveAt(FillIndexes.Count - 1);
        }

        private void FillListElementUncertain(FillParameter parameter)
        {
            base.Continue = true;
            base.CurrentIndex = -1;
            FillIndexes.Add(this);
            FillParameter tmp = null;
            while (base.Continue)
            {
                base.Continue = false;
                base.CurrentIndex++;
                if (string.IsNullOrEmpty(parameter.href))
                    tmp = parameter;
                else
                    tmp = new FillParameter() { href = string.Format(parameter.href, base.CurrentIndex + 1), Type = parameter.Type, CanAdd = parameter.CanAdd };
                FillElement(tmp, null);
                this.Wait();
            }
            FillIndexes.RemoveAt(FillIndexes.Count - 1);
        }

        protected override IHTMLElement FindElement(HtmlDocument doc, FillParameter parameter, bool isContain, string elementType, int formIndex)
        {
            if (string.IsNullOrEmpty(parameter.href) == false)
                return InvokeScriptSync(doc, "findElementByHref", new object[] { parameter.href }) as IHTMLElement;
            return InvokeScriptSync(doc, "findElement", new object[] { elementType, isContain, parameter.Id, parameter.Name, parameter.Value, parameter.OnClick, parameter.href, formIndex }) as IHTMLElement;
        }

        protected override int GetFormIndex(FillParameter parameter)
        {
            return _formIndex;
        }
        /// <summary>
        /// 拆分网页中的多级填报数据。
        /// </summary>
        /// <param name="value">原参数值。</param>
        /// <param name="canAdd">是否为可新增参数。</param>
        /// <returns></returns>
        private string GetMultiFillValue(string value, bool canAdd)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            string[] array = null;
            int level = canAdd ? FillIndexes.Count : FillIndexes.Count - 1;
            int index = 0;
            if (level >= base.Converter.SpliterChars.Length)
                throw new ArgumentException(string.Format("未定义的多级填报级数：{0}级。", level + 1));

            for (int i = level; (canAdd && i > 0) || (canAdd == false && i >= 0); i--)
            {
                array = value.Split(base.Converter.SpliterChars[i]);
                index = FillIndexes[level - i].CurrentIndex; // 获取当前正在进行第几次填报
                if (array.Length > index)
                    value = array[index];
                else
                    value = array.Length > 0 ? array[array.Length - 1] : "";  // 若当前级数无对应的填报参数，则使用上一次的填报值
                if (array.Length - 1 > index)
                    FillIndexes[level - i].Continue = true;
            }
            return value;
        }

        private List<string> GetValueList(DataTable values)
        {
            // 存储以参数名为键值，多个参数值之间以默认分割符连接的值。
            Hashtable table = new Hashtable();
            char separator = base.Converter.SpliterChars[0];
            for (int rowIndex = 0; rowIndex < values.Rows.Count; rowIndex++)
            {
                DataRow row = values.Rows[rowIndex];
                foreach (DataColumn column in values.Columns)
                {
                    string key = column.ColumnName;
                    if (table.ContainsKey(key))
                        table[key] = string.Format("{0}{1}{2}", table[key], separator, row[column]);
                    else
                        table.Add(key, row[column]);
                }
            }

            List<string> valueList = new List<string>();
            foreach (KeyValuePair<FillParameterKey, object> parameter in _parameters)
            {
                if (parameter.Key.Key != null && table.ContainsKey(parameter.Key.Key))
                    valueList.Add(table[parameter.Key.Key] as string);
            }
            return valueList;
        }

        private bool SearchMatch(FillParameter parameter, List<KeyValuePair<FillParameterKey, object>> searchOption, List<string> values)
        {
            this.Wait();
            try
            {
                GHPageSearch search = new GHPageSearch(values, searchOption, base.Browser);
                if (search.SearchAndSelect())
                {
                    FillElement(parameter, null);
                    return true;
                }
            }
            finally
            {
                searchOption.Clear();
                values.Clear();
            }
            return false;
        }

        //获取参数
        private string GetParameterFile()
        {
            string formatStr = "{0}\\{1}";
            string publicPage = string.Format(formatStr, base.RulePath, FileHelper.GetPublicPage(base.FillVersion, FillType));
            string fileName = FileHelper.GetFillRuleFile(base.FillVersion, FillType, Standard, CarType);
            string carTypePage = string.Format(formatStr, base.RulePath, fileName);
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(carTypePage))
            {
                reader.Open();
                if (reader.Contains(_urlParameter.LabelName))
                    return carTypePage;
                else
                    return publicPage;
            }
        }

        private void GetParameters()
        {
            Hashtable uniqueTable = new Hashtable();
            _parameters.Clear();
            string note = "";
            Hashtable group = null;
            Hashtable columnHeader = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(GetParameterFile()))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(_urlParameter.LabelName) as Office.Excel.ForwardReadWorksheet;
                if (sheet == null)
                    return;
                object content = null;
                if (sheet.ReadNextRow())
                {
                    while (sheet.ReadNextCell(false))
                    {
                        content = sheet.GetContent();
                        columnHeader.Add(sheet.CurrentCell.ColumnIndex, content == null ? "" : content.ToString());
                    }
                }
                FillParameter dElement;
                FillParameterKey key;
                while (sheet.ReadNextRow())
                {
                    dElement = new FillParameter();
                    while (sheet.ReadNextCell(false))
                    {
                        content = sheet.GetContent();
                        string value = content == null ? "" : content.ToString();
                        switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                        {
                        case "元素id":
                            dElement.Id = value;
                            break;
                        case "元素name":
                            dElement.Name = value;
                            break;
                        case "元素value":
                            dElement.Value = value;
                            break;
                        case "类别":
                            dElement.Type = value == null ? "" : value.ToUpper();
                            break;
                        case "onclick":
                            dElement.OnClick = value;
                            break;
                        case "参数名称":
                            dElement.ParameterName = value;
                            break;
                        case "备注":
                            note = value;
                            break;
                        case "可新增":
                            dElement.CanAdd = value == "是";
                            break;
                        case "查找":
                            dElement.SearchString = value;
                            break;
                        case "frameId":
                            dElement.FrameId = value;
                            break;
                        case "数据所在工作表":
                            dElement.TableName = value;
                            break;
                        case "拆分规则":
                            dElement.SplitExpr = value;
                            break;
                        case "是否必填":
                            dElement.IsRequired = value == "是";
                            break;
                        case "模糊匹配":
                            dElement.CanContain = value == "是";
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(dElement.Type))
                        continue;
                    if (dElement.Type == Matcher.TYPE_RADIO || dElement.Type == Matcher.TYPE_CHECKBOX)
                    {   // 若当前参数为radio或checkbox则将其后面出现的所有此类元素作为同一参数
                        if (string.IsNullOrEmpty(dElement.ParameterName) == false
                            && uniqueTable.ContainsKey(dElement.ParameterName) == false)
                        {
                            group = new Hashtable();
                            key = new FillParameterKey(dElement.ParameterName, dElement.Type, dElement.CanAdd,dElement.IsRequired, dElement.SearchString);
                            _parameters.Add(new KeyValuePair<FillParameterKey, object>(key, group));
                            uniqueTable.Add(dElement.ParameterName, group);
                        }

                        //只保存第一个radio或者checkbox参数,在group里面保存全部的信息
                        group.Add(dElement.Value, dElement);
                    }
                    else
                    {
                        group = null;
                        key = new FillParameterKey(dElement.ParameterName, dElement.Type, dElement.CanAdd, dElement.IsRequired, dElement.SearchString);
                        key.TableName = dElement.TableName;
                        _parameters.Add(new KeyValuePair<FillParameterKey, object>(key, dElement));
                    }
                }
                uniqueTable.Clear();
            }
        }

        protected override void UpdateFormIndex(FillParameter parameter)
        {
            if (base.GetElement(parameter, -1) != null)
                _formIndex++;
        }
    }
}
