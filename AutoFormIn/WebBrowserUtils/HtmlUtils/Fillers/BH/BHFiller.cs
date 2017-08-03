using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using mshtml;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Detectors;
using Assistant.DataProviders;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    internal class BHFiller : FillBase
    {
        private int _formIndex;
        private Dictionary<string, int> formIndexes;
        private BHUrlParameter _urlParameter;
        private static readonly string[] emptyStringArray = new string[] { };
        private List<KeyValuePair<FillParameterKey, object>> _parameters;
        //全局变量保存自动生成的文本框name
        private int nameNum = 100;
        private const string standard = "发动机排放标准";

        protected BHUrlParameter UrlParameter
        {
            get { return _urlParameter; }
        }

        protected List<KeyValuePair<FillParameterKey, object>> Parameters
        {
            get { return _parameters; }
        }

        public BHFiller(WebBrowser2 browser, Uri currentUri, BHUrlParameter urlParameter)
            : base(browser, currentUri)
        {
            _parameters = new List<KeyValuePair<FillParameterKey, object>>();
            _urlParameter = urlParameter;
            formIndexes = new Dictionary<string,int>();
        }

        public string CarType
        {
            get { return DataTable["CarType"] as string; }
        }

        public static string FillType
        {
            get { return "北环"; }
        }

        private string Standard
        {
            get { return DataTable[standard] as string; }
        }
        //开始填报
        protected override void BeginFillProtected()
        {
            if (base.Document == null)
                return;
            this.GetParameters();
            _formIndex = -1;
            Hashtable data = InitFillData();
            if (_urlParameter.IsTableData)
            {
                this.FillTableValue(data);
                return;
            }
            // 存储可新增/可查找的数据
            List<KeyValuePair<FillParameterKey, object>> list = new List<KeyValuePair<FillParameterKey, object>>();
            List<string> values = new List<string>();
            foreach (KeyValuePair<FillParameterKey, object> parameter in _parameters)
            {
                if (string.IsNullOrEmpty(parameter.Key.TableName) == false)
                {
                    // 对于表格数据，若未找到对应的表格则忽略此填报参数
                    List<int> indexes = new List<int>();
                    foreach (var fillBase in base.FillIndexes)
                    {
                        indexes.Add(fillBase.CurrentIndex);
                    }
                    if (DataProvider.ProvideData(new object[] { parameter.Key.TableName, indexes }) == null)
                        continue;
                }
                string parameterValue = string.IsNullOrEmpty(parameter.Key.Key) ? "" : data[parameter.Key.Key] as string; // 将向网页中填报的数据。
                if (string.IsNullOrEmpty(parameterValue) && (parameter.Key.Key == null || data.ContainsKey(parameter.Key.Key) == false))
                {
                    if (parameter.Key.Type != Matcher.TYPE_A && parameter.Key.Type != Matcher.TYPE_BUTTON && parameter.Key.Type != Matcher.TYPE_FORM
                        && parameter.Key.Type != Matcher.TYPE_SUBMIT && parameter.Key.Type != "BUTTON/SUBMIT" )
                        base.FillRecords.Add(new FillRecord(GetElementType(parameter.Key.Type), RecordType.Failed, "数据文件中不包含此参数", parameter.Key.Key));
                }
                //是否是可新增
                if (parameter.Key.CanAdd)
                {
                    list.Add(parameter);
                    values.Add(parameterValue);
                    if (parameter.Key.Type == Matcher.TYPE_SUBMIT || parameter.Key.Type == "BUTTON/SUBMIT" || parameter.Key.Type == Matcher.TYPE_BUTTON || parameter.Key.Type == Matcher.TYPE_A)
                        FillListElement(list, values);
                    continue;
                }
                else if (string.IsNullOrEmpty(parameter.Key.SearchString) == false)
                {
                    if (parameter.Key.Type == Matcher.TYPE_SUBMIT || parameter.Key.Type == "BUTTON/SUBMIT" || parameter.Key.Type == Matcher.TYPE_BUTTON)
                    {
                        if (SearchMatch(parameter.Value as FillParameter, list, values))
                        {
                            list.Clear();
                            values.Clear();
                            return;
                        }
                    }
                    else
                    {
                        list.Add(parameter);
                        values.Add(parameterValue);
                    }
                    continue;
                }
                //如果存在可新增
                if (list.Count != 0)
                    FillListElement(list, values);
                FillElement(parameter.Key, parameter.Value, parameterValue);
            }
        }//end BeginFillProtected

        protected override void FillTextElement(IHTMLElement element, string value)
        {
            switch (element.tagName)
            {
                case "INPUT":
                    IHTMLInputElement textElement = element as IHTMLInputElement;
                    textElement.value = value;
                    this.InvokeOnChange(element);
                    this.FillRecords[(int)ElementType.Text].RecordCount++;
                    break;
                case "TEXTAREA":
                    IHTMLTextAreaElement textAreaElement = element as IHTMLTextAreaElement;
                    textAreaElement.value = value;
                    this.InvokeOnChange(element);
                    this.FillRecords[(int)ElementType.Text].RecordCount++;
                    break;
            }
        }

        private void FillTableValue(Hashtable data)
        {
            if (data == null)
            {
                base.Browser.Invoke((Action)(() =>
                {
                    WebBrowser2.InvokeScript(base.Document.DomDocument as IHTMLDocument, "goBack");
                }));
                return;
            }
            int count = 0;
            List<string> valueList = null;
            List<KeyValuePair<FillParameterKey, object>> fillList = new List<KeyValuePair<FillParameterKey, object>>();
            int fillIndex = 0, rowIndex = 0;
            foreach (DictionaryEntry entry in data)
            {
                rowIndex = 0;
                DataTable table = entry.Value as DataTable;
                bool canAdd;
                while (table.Rows.Count > rowIndex)
                {
                    formIndexes.Clear();
                    valueList = GetValueList(table, fillList, out fillIndex, ref rowIndex, out canAdd);
                    for (int i = 0; i < fillIndex; i++)
                    {
                        if (_parameters[i].Key.Type == Matcher.TYPE_FORM)
                        {
                            FillParameter parameter = _parameters[i].Value as FillParameter;
                            if (parameter != null)
                            {
                                string key = parameter.FrameId;
                                if (string.IsNullOrEmpty(key))
                                    key = "@";
                                if (formIndexes.ContainsKey(key))
                                    formIndexes[key] = formIndexes[key] + 1;
                                else
                                    formIndexes[key] = 0;
                            }
                        }
                    }
                    FillListElement(fillList, valueList, canAdd);
                    count = Math.Max(count, fillIndex);
                    fillList.Clear();
                }
            }
            
            for (int index = count; index < _parameters.Count; index++)
            {
                this.FillElement(_parameters[index].Key, _parameters[index].Value, null);
            }
        }

        private List<string> GetValueList(DataTable values, List<KeyValuePair<FillParameterKey, object>> parameters, out int index, ref int rowIndex, out bool canAdd)
        {
            // 存储以参数名为键值，多个参数值之间以默认分割符连接的值。
            canAdd = false;
            Hashtable table = new Hashtable();
            char separator = base.Converter.SpliterChars[0];
            string lastPrefix = "";
            bool loop = true;
            for (; rowIndex < values.Rows.Count; rowIndex++)
            {
                DataRow row = values.Rows[rowIndex];
                foreach (DataColumn column in values.Columns)
                {
                    if (column.Ordinal == 0)
                    {
                        string prefix = column.ColumnName == "行标题" ? string.IsNullOrEmpty(row[column] as string) ? lastPrefix : row[column] as string : "";
                        if (string.IsNullOrEmpty(lastPrefix))
                            lastPrefix = prefix;
                        else if (lastPrefix != prefix)
                        {
                            loop = false;
                            break;
                        }
                    }
                    string key = string.Format("{0}{1}", lastPrefix, column.ColumnName);
                    if (key == null)
                        continue;
                    if (table.ContainsKey(key))
                        table[key] = string.Format("{0}{1}{2}", table[key], separator, row[column]);
                    else
                        table.Add(key, row[column]);
                }
                if (loop == false)
                    break;
            }
            index = 0;
            bool begin = false;
            foreach (var parameter in _parameters)
            {
                if (parameter.Key.Key != null && table.ContainsKey(parameter.Key.Key)) // 若当前表格的参数名包含此参数
                {
                    parameters.Add(parameter);
                    begin = true;
                }
                else if (begin) // 一组表格的填报规则应为连续存放的
                {
                    if (parameter.Key.CanAdd && parameters[0].Key.CanAdd)
                    {
                        parameters.Add(parameter);
                        canAdd = true;
                        index++;
                    }
                    break;
                }
                index++;
            }
            List<string> valueList = new List<string>();
            foreach (KeyValuePair<FillParameterKey, object> parameter in parameters)
            {
                if (parameter.Key.Key != null && table.ContainsKey(parameter.Key.Key))
                    valueList.Add(table[parameter.Key.Key] as string);
            }
            if (begin == false)
                index = 0;
            return valueList;
        }
        
        private void FillListElement(List<KeyValuePair<FillParameterKey, object>> list, List<string> values, bool canAdd)
        {
            if (canAdd == false)
            {
                string parameterValue = "";
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Key.Type == Matcher.TYPE_SUBMIT || list[i].Key.Type == Matcher.TYPE_A || list[i].Key.Type == "BUTTON/SUBMIT")
                        parameterValue = null;
                    else
                        parameterValue = values[i];
                    FillElement(list[i].Key, list[i].Value, parameterValue);
                }
            }
            else
                FillListElement(list, values);
        }
        /// <summary>
        ///  填充元素（分割元素）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="values"></param>
        private void FillListElement(List<KeyValuePair<FillParameterKey, object>> list, List<string> values)
        {
            //包含分割字符串
            if (list.Count == 1 && (list[0].Key.Type == Matcher.TYPE_SUBMIT || list[0].Key.Type == "BUTTON/SUBMIT"
                || list[0].Key.Type == Matcher.TYPE_BUTTON))
            {
                //仅包含新增按钮的新增列表
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
            string name = ((FillParameter)(list[0].Value)).Name;
            bool skipLastButton = (string.IsNullOrEmpty(name) == false && name.Contains("[") && name.Contains("]"));
            FillIndexes.Add(this);
            for (int valueIndex = 0; valueIndex < maxValueLength; valueIndex++)
            {
                base.CurrentIndex = valueIndex;  // 更新当前填报索引
                FillParameterKey key = null;
                //具体的某个控件
                object elementContainer = null;
                string parameterValue = "";
                if (valueIndex > 0)
                    nameNum++;
                for (int i = 0; i < list.Count; i++)
                {
                    key = list[i].Key;
                    elementContainer = list[i].Value;
                    if (list[i].Key.Type == Matcher.TYPE_SUBMIT || list[i].Key.Type == Matcher.TYPE_A || list[i].Key.Type == "BUTTON/SUBMIT" || list[i].Key.Type == Matcher.TYPE_BUTTON)
                    {
                        if (valueIndex == maxValueLength - 1 && skipLastButton)
                            continue; 
                        parameterValue = null;
                    }
                    else if (valueList[i].Length > valueIndex)
                        parameterValue = valueList[i][valueIndex];
                    else
                        parameterValue = "";
                    //判断所填写是否为新增元素
                    if (valueIndex > 0)
                    {
                        //修改name
                        FillParameter fp = (FillParameter)elementContainer;
                        name = fp.Name;
                        if (string.IsNullOrEmpty(name) == false && name.Contains("[") && name.Contains("]"))
                        {
                            int index1 = name.IndexOf("["), index2 = name.IndexOf("]", index1 == -1 ? 0 : index1);
                            System.Diagnostics.Trace.Assert(index1 != -1 && index2 != -1);
                            string left = name.Substring(0, index1);
                            string right = name.Substring(index2 + 1);
                            //拼接name
                            fp.Name = string.Format("{0}[{1}]{2}", left, nameNum, right);
                            fp.FindCode = "";
                        }
                    }
                    FillElement(key, elementContainer, parameterValue);
                }//end for
                this.Wait();
            }
            FillIndexes.RemoveAt(FillIndexes.Count - 1);
            list.Clear();
            values.Clear();
        }

        private void FillListElementUncertain(FillParameter parameter)
        {
            base.Continue = true;
            base.CurrentIndex = -1;
            FillIndexes.Add(this);
            while (base.Continue)
            {
                base.Continue = false;
                base.CurrentIndex++;
                FillElement(parameter, null);
                this.Wait();
            }
            FillIndexes.RemoveAt(FillIndexes.Count - 1);
        }
        ///// <summary>
        ///// 填报checkbox数据。
        ///// </summary>
        ///// <param name="value"></param>
        ///// <param name="checkBoxGroup"></param>
        //private void FillCheckBoxGroup(string value, Hashtable checkBoxGroup)
        //{
        //    string[] values = value == null ? null : value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
        //    if (values == null)
        //        return;
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        FillParameter dElement = checkBoxGroup[values[i]] as FillParameter;
        //        if (dElement == null)
        //            continue;
        //        IHTMLInputElement element = base.GetElement(dElement, _formIndex) as IHTMLInputElement;
        //        if (element == null)
        //            continue;
        //        element.@checked = true;
        //    }
        //}
        protected override IHTMLElement FindElement(System.Windows.Forms.HtmlDocument doc, FillParameter parameter, bool isContain, string elementType, int formIndex)
        {
            if (string.IsNullOrEmpty(parameter.FindCode) == false)
                return InvokeScriptSync(doc, "findElementByExpr", new object[] { parameter.FindCode }) as IHTMLElement;
            else
                return InvokeScriptSync(doc, "findElement", new object[] { elementType, parameter.Id, parameter.Name, parameter.Value, parameter.OnClick, formIndex }) as IHTMLElement;
        }

        protected override int GetFormIndex(FillParameter parameter)
        {
            if (parameter == null)
                return -1;
            else if (string.IsNullOrEmpty(parameter.FrameId) && formIndexes.ContainsKey("@"))
                return formIndexes["@"];
            else if (string.IsNullOrEmpty(parameter.FrameId) == false && formIndexes.ContainsKey(parameter.FrameId))
                return formIndexes[parameter.FrameId];
            return -1;
        }

        private bool SearchMatch(FillParameter parameter, List<KeyValuePair<FillParameterKey, object>> searchOption, List<string> values)
        {
            this.Wait();
            BHPageSearch search = new BHPageSearch(searchOption, values, base.Browser);
            if (search.SearchAndSelect())
            {
                FillElement(parameter, null);
                return true;
            }
            WebFillManager.ShowMessageBox("未找到指定维修站！", "消息",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
            return false;
        }

        private Hashtable InitFillData()
        {
            Hashtable data = null;
            if (_urlParameter.IsTableData == false)
            {
                if (FillIndexes.Count == 0)
                    return DataTable;
                data = new Hashtable();
                foreach (var item in _parameters)
                {
                    if (item.Key.Key == null || data.ContainsKey(item.Key.Key) || item.Key.Type == Matcher.TYPE_FORM || item.Key.Type == Matcher.TYPE_A
                        || item.Key.Type == Matcher.TYPE_SUBMIT || item.Key.Type == "BUTTON/SUBMIT")
                        continue;
                    data.Add(item.Key.Key, GetMultiFillValue(DataTable[item.Key.Key] as string, item.Key.CanAdd));
                }
            }
            else
            {
                List<int> list = new List<int>();
                foreach (var fillBase in base.FillIndexes)
                {
                    list.Add(fillBase.CurrentIndex);
                }
                data = base.DataProvider.ProvideData(new object[] { _urlParameter.LabelName, list }) as Hashtable;
            }
            return data;
        }

        protected override string GetJSCode()
        {
            return
@"
function findElement(elementType,id,name,value,onclick,formIndex){
    var form = document.forms[formIndex];
    var findCode = '';
    if(elementType != null && elementType != '' && elementType != undefined)
        findCode = elementType;
    if(id != null && id != '' && id != undefined)
        findCode += '[id=""'+id+'""]';
    if(name != null && name != '' && name != undefined)
        findCode += '[name=""'+name+'""]';
    if(value != null && value != '' && value != undefined)
        findCode += '[value*=""'+value+'""]';
    if(onclick != null && onclick != '' && onclick != undefined)
        findCode += '[onclick*=""'+onclick+'""]';
    var obj = $(findCode).filter(':visible');
    if(obj.length > 1)
    {
        var i;
        for(i = 0;i<obj.length;i++)
        {
            if(obj.get(i).form == form)
                return obj.get(i);
        }
        return null;
    }
    return obj.get(0);
}

function goBack(){ history.back(); }

function canInvoke(){ return findElement != null && findElement != undefined; }

function invokeOnChange(obj){ obj.onchange(); }

// 删除复制的多余行
function deleteRows(){
    var row = $('a[onclick*=""delColumn(this);""]');
    alert(row.length);
    for(var i = 0; i < row.length; i++){
        var element = row.get(i);
        element.click();
    }
}

function findElementByExpr(expr){
    var result = $(expr);
    return result.length > 0? result.get(0):null;
}";
        }

        //private Hashtable GetTableData()
        //{
        //    bool updateColumnHeader = false;
        //    string prefix = "";
        //    object content = null;
        //    Hashtable data = new Hashtable();
        //    List<KeyValuePair<string, string>> list;
        //    int rowHeaderColumn = 0;
        //    using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(this.DataFile))
        //    {
        //        reader.Open();
        //        Office.Excel.ForwardReadWorksheet sheet = reader.Activate(_urlParameter.LabelName) as Office.Excel.ForwardReadWorksheet;
        //        if (sheet != null)
        //        {
        //            Hashtable columnHeader = FillManagerBase.GetColumnHeader(sheet);
        //            prefix = columnHeader[1] as string;
        //            while (sheet.ReadNextRow())
        //            {
        //                list = new List<KeyValuePair<string, string>>();
        //                while (sheet.ReadNextCell(false))
        //                {
        //                    content = sheet.GetContent();
        //                    string str = content == null ? "" : content.ToString();
        //                    if (str != "行标题" && sheet.CurrentCell.ColumnIndex == rowHeaderColumn)
        //                    {
        //                        if (string.IsNullOrEmpty(str) == false && prefix != str)
        //                            prefix = str;
        //                    }
        //                    else if (str == "行标题")
        //                    {
        //                        updateColumnHeader = true;
        //                        rowHeaderColumn = sheet.CurrentCell.ColumnIndex;
        //                    }
        //                    else
        //                    {
        //                        if (updateColumnHeader)
        //                            columnHeader[sheet.CurrentCell.ColumnIndex] = str;
        //                        else
        //                        {
        //                            string key = string.Format("{0}{1}", prefix, columnHeader[sheet.CurrentCell.ColumnIndex]);
        //                            list.Add(new KeyValuePair<string, string>(key, str));
        //                        }
        //                    }
        //                }
        //                List<IList> stored = null;
        //                if (data.ContainsKey(prefix))
        //                    stored = data[prefix] as List<IList>;
        //                else
        //                {
        //                    stored = new List<IList>();
        //                    data.Add(prefix, stored);
        //                }
        //                stored.Add(list);
        //                updateColumnHeader = false;
        //            }
        //        }
        //    }
        //    return data;
        //}

        //private Hashtable GetTableData()
        //{
        //    bool isColumnHeader = false;
        //    object content = null;
        //    Hashtable columnHeader = new Hashtable();
        //    Hashtable tableList = new Hashtable();
        //    DataTable table = null;
        //    DataRow row = null;
        //    int index = 0;
        //    using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(this.DataFile))
        //    {
        //        reader.Open();
        //        // 表格数据的工作表标签使用"前缀_序号"格式命名。
        //        string dataSheetName = this.Converter == null ? _urlParameter.LabelName : this.Converter.GetSheetName(_urlParameter.LabelName);
        //        string sheetName = string.Format("{0}_{1}", dataSheetName, base.FillIndexes.Count == 0 ? 1 :
        //            base.FillIndexes[0].CurrentIndex + 1);
        //        Office.Excel.ForwardReadWorksheet sheet = reader.Activate(sheetName) as Office.Excel.ForwardReadWorksheet;
        //        if (sheet != null)
        //        {
        //            while (sheet.ReadNextRow())
        //            {
        //                while (sheet.ReadNextCell(false))
        //                {
        //                    content = sheet.GetContent();
        //                    string str = content == null ? "" : content.ToString();
        //                    if (isColumnHeader == false && (str == "行标题" || table == null))
        //                    {
        //                        table = new System.Data.DataTable();
        //                        tableList.Add(index, table);
        //                        isColumnHeader = true;
        //                        row = null;
        //                        index++;
        //                    }
        //                    int columnIndex = sheet.CurrentCell.ColumnIndex;
        //                    if (isColumnHeader)
        //                    {
        //                        if (string.IsNullOrEmpty(str))
        //                            continue;
        //                        if (columnHeader.Contains(columnIndex))
        //                            columnHeader[columnIndex] = str;
        //                        else
        //                            columnHeader.Add(columnIndex, str);
        //                        try
        //                        {
        //                            if (string.IsNullOrEmpty(str) == false)
        //                                table.Columns.Add(str);
        //                        }
        //                        catch (System.Data.DuplicateNameException)
        //                        {
        //                            throw new ArgumentException(string.Format("在Excel文件：{0}的{1}工作表中，{2}行的值有重复！",
        //                                sheet.Owner.FileName, sheet.Name, sheet.CurrentRowIndex));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        string columnName = columnHeader[columnIndex] as string;
        //                        if(table.Columns.Contains(columnName))
        //                            row[columnName] = str;
        //                    }
        //                }
        //                if (row != null)
        //                    table.Rows.Add(row);
        //                row = table.NewRow();
        //                isColumnHeader = false;
        //            }
        //        }
        //        return tableList;
        //    }
        //}

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

        private string GetParameterFile()
        {
            if (_urlParameter.IsPublicUrl)
                return string.Format("{0}\\{1}", base.RulePath, FileHelper.GetPublicPage(base.FillVersion, FillType)); 
            //string dir = "\\北环\\", filename = "";
            //北环排放标准待定
            //switch (EmissionsStandras)
            //{
            //    case "第四阶段":
            //        dir = "\\北环\\国四\\";
            //        break;
            //    default:
            //        return "";
            //}
            string filename = FileHelper.GetFillRuleFile(base.FillVersion, FillType, "All", CarType);
            //if (filename == "")
            //    return "";
            return string.Format("{0}\\{1}", base.RulePath, filename);

        }
        //得到参数
        protected void GetParameters()
        {
            Hashtable uniqueTable = new Hashtable();
            _parameters.Clear();
            string note = "";
            Hashtable group = null;
            Hashtable columnHeader = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(GetParameterFile()))
            {
                reader.Open();
                string sheetName = string.Format("{0}（{1}）", _urlParameter.LabelName, this.Standard);
                // 同类车型中不同排放标准的填报规则可能不同。
                sheetName = reader.Contains(sheetName) ? sheetName : _urlParameter.LabelName;
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(sheetName) as Office.Excel.ForwardReadWorksheet;
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
                            case "是否必填":
                                dElement.IsRequired = value == "是";
                                break;
                            case "数据表":
                                dElement.TableName = value;
                                break;
                            case "查找代码":
                                dElement.FindCode = value;
                                break;
                        }
                    }
                    if (string.IsNullOrEmpty(dElement.Type))
                        continue;
                    if (dElement.Type == Matcher.TYPE_RADIO || dElement.Type == Matcher.TYPE_CHECKBOX)
                    {   // 若当前参数为radio或checkbox则将其后面出现的所有此类元素作为同一参数
                        if (uniqueTable.ContainsKey(dElement.ParameterName) == false)
                        {
                            group = new Hashtable();
                            key = new FillParameterKey(dElement.ParameterName, dElement.Type, dElement.CanAdd, dElement.IsRequired, dElement.SearchString);
                            _parameters.Add(new KeyValuePair<FillParameterKey, object>(key, group));
                            uniqueTable.Add(dElement.ParameterName, group);
                        }
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
            string key = parameter.FrameId;
            if (string.IsNullOrEmpty(key))
                key = "@";
            if (formIndexes.ContainsKey(key))
                formIndexes[key] = formIndexes[key] + 1;
            else
                formIndexes.Add(key, 0);
        }
    }
}
