using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using mshtml;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Detectors;
using System.Text.RegularExpressions;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 国环网页填报器。
    /// </summary>
    public sealed class PZHFiller : FillBase
    {
        private int _formIndex;
        private UrlParameter _urlParameter;
        private static readonly string[] emptyStringArray = new string[]{};
        private List<KeyValuePair<FillParameterKey, object>> _parameters;
        private const string engineModel = "发动机型号", mainVehicleModel = "主车型型号", formName = "frm_part3";
        public string DefaultValue
        {
            get;
            internal set;
        }

        public PZHFiller(WebBrowser2 browser, Uri currentUri, UrlParameter urlParameter)
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
            this.Wait();
            Hashtable data = InitFillData();
            string lastTableName = "";
            // 存储可新增/可查找的数据
            List<KeyValuePair<FillParameterKey, object>> list = new List<KeyValuePair<FillParameterKey, object>>();
            List<string> values = new List<string>();
            foreach (KeyValuePair<FillParameterKey, object> parameter in _parameters)
            {
                string parameterValue = string.IsNullOrEmpty(parameter.Key.Key) ? "" : data[parameter.Key.Key] as string; // 将向网页中填报的数据。
                if (parameter.Key.CanAdd)
                {
                    if (string.IsNullOrEmpty(parameter.Key.TableName) == false)
                    {
                        if (lastTableName == parameter.Key.TableName || string.IsNullOrEmpty(lastTableName))
                        {
                            lastTableName = parameter.Key.TableName;
                            list.Add(parameter);
                        }
                        else
                        {
                            FillTableValue(GetTableData(lastTableName), list);
                            list.Add(parameter);
                            lastTableName = parameter.Key.TableName;
                        }
                    }
                    else
                    {
                        list.Add(parameter);
                        values.Add(parameterValue);
                    }
                    continue;
                }
                else if (string.IsNullOrEmpty(parameter.Key.SearchString) == false)
                {
                    if (parameter.Key.Type == Matcher.TYPE_SUBMIT || parameter.Key.Type == "BUTTON/SUBMIT"
                        || parameter.Key.Type == Matcher.TYPE_BUTTON)
                    {
                        if (SearchMatch(parameter.Value as FillParameter, list, values))
                            continue;
                    }
                    else
                    {
                        FillListElement(list, values);
                        list.Add(parameter);
                        values.Add(parameterValue);
                    }
                    continue;
                }
                if (list.Count != 0)
                    FillListElement(list, values);
                FillElement(parameter.Key, parameter.Value, data);
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

        private void FillElement(FillParameter parameter, ref string value)
        {
            //if (parameter.Type != Matcher.TYPE_FORM)
            //{
            //    if (parameter.Value == "上传图片")
            //    {
            //        if (string.IsNullOrEmpty(value))
            //            return;
            //        string file = string.IsNullOrEmpty(base.DataFilePath) ? value : string.Format("{0}\\{1}", base.DataFilePath, value);
            //        if (System.IO.File.Exists(file) == false)
            //        {
            //            base.FillRecords.Add(new FillRecord(ElementType.File, RecordType.Failed, string.Format("文件{0}不存在", file), parameter.ParameterName));
            //            return;
            //        }
            //    }
            //}
            //base.FillElement(parameter, value);
            if (parameter == null)
                return;
            IHTMLElement element = null;
            if (parameter.ParameterName == null || base.DataTable.Contains(parameter.ParameterName) == false)
                value = DefaultValue;
            if (parameter.Type != Matcher.TYPE_FORM)
            {
                element = base.GetElement(parameter, _formIndex);
                if (element == null)
                {
                    this.FillRecords.Add(new FillRecord(GetElementType(parameter.Type), RecordType.Failed, "未找到此元素", parameter.ParameterName));
                    return;
                }
                else if (parameter.Value == "上传图片")
                {
                    if (string.IsNullOrEmpty(value))
                        return;
                    string file = string.IsNullOrEmpty(base.DataFilePath) ? value : string.Format("{0}\\{1}", base.DataFilePath, value);
                    if (System.IO.File.Exists(file) == false)
                    {
                        this.FillRecords.Add(new FillRecord(ElementType.File, RecordType.Failed, string.Format("文件{0}不存在", value), parameter.ParameterName));
                        return;
                    }
                }
                ((IHTMLElement2)element).focus();
            }
            if (string.IsNullOrEmpty(parameter.SplitExpr) == false && string.IsNullOrEmpty(value) == false)
            {
                Match match = Regex.Match(value, parameter.SplitExpr, RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
                if (match.Success)
                    value = match.Groups["value"].Value;
            }
            _currentFillValue = value;
            switch (parameter.Type)
            {
                case Matcher.TYPE_FORM:
                    if (base.GetElement(parameter, -1) != null)
                        _formIndex++;
                    break;
                case Matcher.TYPE_RADIO:
                    IHTMLInputElement radioElement = element as IHTMLInputElement;
                    radioElement.@checked = true;
                    this.FillRecords[(int)ElementType.Radio].RecordCount++;
                    break;
                case Matcher.TYPE_SELECT:
                    if (FillSelectElement(parameter, ref value))
                    {
                        this.FillRecords[(int)ElementType.Select].RecordCount++;
                    }
                    else {
                        this.FillRecords.Add(new FillRecord(ElementType.Select, RecordType.Failed, string.Format("下拉框中不包含选项 {0}", value), parameter.ParameterName));
                    }
                    break;
                case Matcher.TYPE_A:
                    element.click();
                    break;
                case Matcher.TYPE_FILE:
                    value = string.IsNullOrEmpty(base.DataFilePath) ? value : string.Format("{0}\\{1}", base.DataFilePath, value);
                    _currentFillValue = value;
                    if (System.IO.File.Exists(value)) {
                        element.click();
                        this.FillRecords[(int)ElementType.File].RecordCount++;
                    }
                    break;
                case Matcher.TYPE_SUBMIT:
                case Matcher.TYPE_BUTTON:
                case "BUTTON/SUBMIT":
                    IHTMLInputElement fileElement = element as IHTMLInputElement;
                    element.click();
                    break;
                case Matcher.TYPE_TEXTAREA:
                    IHTMLTextAreaElement textAreaElement = element as IHTMLTextAreaElement;
                    textAreaElement.value = value;
                    this.FillRecords[(int)ElementType.Text].RecordCount++;
                    break;
                case Matcher.TYPE_TEXT:
                case Matcher.TYPE_PASSWORD:
                    IHTMLInputElement textElement = element as IHTMLInputElement;
                    textElement.value = value;
                    this.FillRecords[(int)ElementType.Text].RecordCount++;
                    break;
            }
        }
        /// <summary>
        /// 填报数据
        /// </summary>
        /// <param name="parameterKey">参数名称。</param>
        /// <param name="elementContainer">用于从网页中查找元素的对象，当value为Hashtable时表示当前是一组Radio或CheckBox。</param>
        /// <param name="parameterValue">用于填报的数据。</param>
        /// <param name="list">用于存储多值数据的列表。</param>
        private void FillElement(FillParameterKey parameterKey, object elementContainer, Hashtable data)
        {
            this.Wait();
            FillParameter fillParameter = elementContainer as FillParameter;

            String parameterValue = data[parameterKey.Key] as String;
            if (fillParameter == null)
            {
                Hashtable table = elementContainer as Hashtable;
                if (table == null)
                    return;
                else if (parameterKey.Type == Matcher.TYPE_RADIO)
                {
                    // 处理radio类型的选择
                    if (parameterValue != null)
                        fillParameter = table[parameterValue] as FillParameter;
                }
                else if (parameterKey.Type == Matcher.TYPE_CHECKBOX) // 处理checkBox类型
                {
                    FillCheckBoxGroup(ref parameterValue, table);
                    data.Remove(parameterKey.Key);
                    data.Add(parameterKey.Key, parameterValue);
                    return;
                }
            }
            FillElement(fillParameter, ref parameterValue);
            data.Remove(parameterKey.Key);
            data.Add(parameterKey.Key, parameterValue);
        }

        //private void FillElement(FillParameterKey parameterKey, object elementContainer, String parameterValue)
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
        //                fillParameter = table[parameterValue] as FillParameter;
        //        }
        //        else if (parameterKey.Type == Matcher.TYPE_CHECKBOX) // 处理checkBox类型
        //        {
        //            FillCheckBoxGroup(ref parameterValue, table);
        //            return;
        //        }
        //    }
        //    FillElement(fillParameter, ref parameterValue);
        //}

        //下拉框填报：1.如果只有一个选项就选中  2.如果选项有其他而且值没有其他符合项就选中其他
        private bool FillSelectElement(FillParameter parameter, ref string value)
        {
            IHTMLSelectElement element = base.GetElement(parameter, _formIndex) as IHTMLSelectElement;
            if (element == null)
                return false;
            List<IHTMLOptionElement> listOptions = new List<IHTMLOptionElement>();
            IHTMLOptionElement otherOption = null;

            //分割参数值,已经填充的参数值从中删掉
            string[] values = value == null ? null : value.Split(PZHFillManager.regularSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (values == null || values.Length < 1)
                return false ;
            List<String> listValues = new List<String>(values);

            for (int i = 0; i < element.length; i++)
            {
                IHTMLOptionElement option = element.item(i);
                if (String.IsNullOrWhiteSpace(option.text)) {
                    continue;
                }
                listOptions.Add(option);
                if (option.text == "其他") {
                    otherOption = option;
                }
                if (parameter.CanContain)
                {
                    String selectedItem = null;
                    foreach (String item in listValues) {
                        if (option.text.Contains(item)) {
                            option.selected = true;
                            selectedItem = item;
                            break;
                        }
                    }
                    if (parameter.CanDelete && !String.IsNullOrEmpty(selectedItem)) {
                        listValues.Remove(selectedItem);
                    }
                }
                else {
                    if (listValues.Contains(option.text))
                    {
                        option.selected = true;
                        if (parameter.CanDelete)
                        {
                            listValues.Remove(option.text);
                        }
                        value = String.Join(PZHFillManager.regularJoiner, listValues);
                        base.InvokeChange2(element as IHTMLElement);
                        return true;
                    }
                }
            }
            if (listOptions.Count == 1) {
                listOptions[0].selected = true;
                base.InvokeChange2(element as IHTMLElement);
                return true;
            }
            foreach (IHTMLOptionElement option in listOptions) {
                if (option.text == "其他") {
                    option.selected = true;
                    base.InvokeChange2(element as IHTMLElement);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 填报checkbox数据。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="checkBoxGroup"></param>
        private void FillCheckBoxGroup(ref string value, Hashtable checkBoxGroup)
        {
            //存储checkbox中的"xxx其他"选项
            FillParameter otherCheckBox = null;
            string[] values = value == null ? null : value.Split( PZHFillManager.regularSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (values == null || values.Length < 1)
                return;
            List<String> listValues = new List<String>(values);

            foreach (String key in checkBoxGroup.Keys)
            {
                FillParameter dElement = checkBoxGroup[key] as FillParameter;
                if (key.Contains("其他")) {
                    otherCheckBox = dElement;
                }
                if (listValues.Contains(key)) {
                    dElement.Value = null;
                    IHTMLElement element = base.GetElement(dElement, _formIndex) as IHTMLElement;
                    if (element == null)
                        continue;
                    //element.@checked = true;
                    element.click();
                    if (dElement.CanDelete)
                    {
                        listValues.Remove(key);
                    }
                }
            }

            if (listValues != null && listValues.Count > 0) {
                otherCheckBox.Value = null;
                IHTMLElement element = base.GetElement(otherCheckBox, _formIndex) as IHTMLElement;
                if (element != null) {
                    //element.@checked = true;
                    //base.InvokeOnChange(element as IHTMLElement);
                    element.click();
                    base.InvokeChange2(element as IHTMLElement);
                }
            }
            value = (listValues == null || listValues.Count == 0) ? "" : String.Join(PZHFillManager.regularJoiner, listValues);
        }

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
                        parameterValue = "";
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
            String value = String.Empty;
            while (base.Continue)
            {
                base.Continue = false;
                base.CurrentIndex++;
                if (string.IsNullOrEmpty(parameter.href))
                    tmp = parameter;
                else
                    tmp = new FillParameter() { href = string.Format(parameter.href, base.CurrentIndex + 1) };
                FillElement(tmp, ref value);
                this.Wait();
            }
            FillIndexes.RemoveAt(FillIndexes.Count - 1);
        }

        private void FillTableValue(DataTable table, List<KeyValuePair<FillParameterKey, object>> parameters)
        {
            List<string> valueList = null;
            valueList = GetValueList(table);
            this.Wait();
            FillListElement(parameters, valueList);
        }

        protected override void UpdateFormIndex(FillParameter parameter)
        {
            if (base.GetElement(parameter, -1) != null)
                _formIndex++;
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
            int count = canAdd ? FillIndexes.Count : FillIndexes.Count - 1;
            int index = 0;
            if (count >= base.Converter.SpliterChars.Length)
                throw new ArgumentException(string.Format("未定义的多级填报级数：{0}级。", count + 1));

            for (int i = count; (canAdd && i > 0) || (canAdd == false && i >= 0); i--)
            {
                array = value.Split(base.Converter.SpliterChars[i]);
                index = FillIndexes[count - i].CurrentIndex; // 获取当前正在进行第几次填报
                if (array.Length > index)
                    value = array[index];
                else
                    value = "";
                if (array.Length - 1 > index)
                    FillIndexes[count - i].Continue = true;
            }
            return value;
        }

        private DataTable GetTableData(string sheetName)
        {
            bool isColumnHeader = false;
            object content = null;
            Hashtable columnHeader = new Hashtable();
            DataTable table = null;
            DataRow row = null;
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(this.DataFile))
            {
                reader.Open();
                // 表格数据的工作表标签使用"前缀_序号"格式命名。
                sheetName = string.Format("{0}_{1}", sheetName, base.FillIndexes.Count == 0 ? 1 :
                    base.FillIndexes[0].CurrentIndex + 1);
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(sheetName) as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                {
                    while (sheet.ReadNextRow())
                    {
                        while (sheet.ReadNextCell(false))
                        {
                            content = sheet.GetContent();
                            string str = content == null ? "" : content.ToString();
                            if (isColumnHeader == false && table == null)
                            {
                                table = new System.Data.DataTable();
                                isColumnHeader = true;
                                row = null;
                            }
                            int columnIndex = sheet.CurrentCell.ColumnIndex;
                            if (isColumnHeader)
                            {
                                if (string.IsNullOrEmpty(str))
                                    continue;
                                columnHeader.Add(columnIndex, str);
                                try
                                {
                                    if (string.IsNullOrEmpty(str) == false)
                                        table.Columns.Add(str);
                                }
                                catch (System.Data.DuplicateNameException)
                                {
                                    throw new ArgumentException(string.Format("在Excel文件：{0}的{1}工作表中，{2}行的值有重复！",
                                        sheet.Owner.FileName, sheet.Name, sheet.CurrentRowIndex));
                                }
                            }
                            else
                            {
                                string columnName = columnHeader[columnIndex] as string;
                                if (table.Columns.Contains(columnName))
                                    row[columnName] = str;
                            }
                        }
                        if (row != null)
                            table.Rows.Add(row);
                        row = table.NewRow();
                        isColumnHeader = false;
                    }
                }
                return table;
            }
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
            String value = String.Empty;
            try
            {
                GHPageSearch search = new GHPageSearch(values, searchOption, base.Browser);
                if (search.SearchAndSelect())
                {
                    FillElement(parameter, ref value);
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
            string formatStr = "{0}\\配置号\\{1}{2}.xlsx";
            string publicPage = string.Format(formatStr, base.RulePath, "", "公共页面");
            return publicPage;
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
                        case "元素href":
                            dElement.href = value;
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
                        case "可删除":
                            dElement.CanDelete = value == "是";
                            break;
                        case "模糊匹配":
                            dElement.CanContain = value == "是";
                            break;
                        case "是否必填":
                            dElement.IsRequired = value == "是";
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
                            key = new FillParameterKey(dElement.ParameterName, dElement.Type, dElement.CanAdd, dElement.IsRequired, dElement.CanDelete, dElement.CanContain, dElement.SearchString);
                            _parameters.Add(new KeyValuePair<FillParameterKey, object>(key, group));
                            uniqueTable.Add(dElement.ParameterName, group);
                        }

                        //只保存第一个radio或者checkbox参数,在group里面保存全部的信息
                        group.Add(dElement.Value, dElement);
                    }
                    else
                    {
                        group = null;
                        key = new FillParameterKey(dElement.ParameterName, dElement.Type, dElement.CanAdd, dElement.CanDelete, dElement.IsRequired, dElement.CanContain, dElement.SearchString);
                        key.TableName = dElement.TableName;
                        _parameters.Add(new KeyValuePair<FillParameterKey, object>(key, dElement));
                    }
                }
                uniqueTable.Clear();
            }
        }

    }
}
