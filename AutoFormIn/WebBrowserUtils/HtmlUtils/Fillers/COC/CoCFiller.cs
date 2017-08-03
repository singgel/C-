using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Detectors;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class CoCFiller : FillBase
    {
        private List<KeyValuePair<FillParameterKey, object>> _parameters;
        private UrlParameter _urlParameter;

        public static string FillType
        {
            get { return "COC"; }
        }


        public CoCFiller(WebBrowser2 browser, Uri currentUri, UrlParameter urlParameter)
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
            Hashtable data = DataTable;
            // 存储可新增/可查找的数据
            foreach (KeyValuePair<FillParameterKey, object> parameter in _parameters)
            {
                string parameterValue = string.IsNullOrEmpty(parameter.Key.Key) ? "" : data[parameter.Key.Key] as string; // 将向网页中填报的数据。
                if (string.IsNullOrEmpty(parameterValue) && (parameter.Key.Key == null || data.ContainsKey(parameter.Key.Key) == false))
                {
                    if (parameter.Key.Type != Matcher.TYPE_A && parameter.Key.Type != Matcher.TYPE_BUTTON && parameter.Key.Type != Matcher.TYPE_FORM
                        && parameter.Key.Type != Matcher.TYPE_SUBMIT && parameter.Key.Type != "BUTTON/SUBMIT" )
                        base.FillRecords.Add(new FillRecord(GetElementType(parameter.Key.Type), RecordType.Failed, "数据文件中不包含此参数", parameter.Key.Key));
                }
                FillElement(parameter.Key, parameter.Value, parameterValue);
            }
        }

        protected override int GetFormIndex(FillParameter parameter)
        {
            return -1;
        }

        protected override void UpdateFormIndex(FillParameter parameter)
        {
        }

        //获取参数
        private string GetParameterFile()
        {
            string formatStr = "{0}\\{1}";
            string publicPage = string.Format(formatStr, base.RulePath, FileHelper.GetPublicPage(base.FillVersion, FillType));
            if (_urlParameter.IsPublicUrl)
                return publicPage;
            string fileName = FileHelper.GetFillRuleFile(base.FillVersion, FillType, null, null);
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

        protected override string GetJSCode()
        {
            //function cocResult(result) function checkIsExist(f0)
            return @"
if(typeof checkIsExist != 'undefined' && typeof checkIsExistProxy == 'undefined'){
    window.checkIsExistProxy = checkIsExist;
    window.checkIsExist = function(f0){
        window.external.Suspend();
        checkIsExistProxy(f0);
    }
}

if(typeof cocResult != 'undefined' && typeof cocResultProxy == 'undefined'){
    window.cocResultProxy = cocResult;
    window.cocResult = function(result){
        cocResultProxy(result);
        window.external.Resume();
    }
}

function findElement(elementType,name,value,onclick,returnIndex)
{
    var findCode = '';
    if(elementType != null && elementType != '' && elementType != undefined)
        findCode = elementType;
    if(name != null && name != '')
        findCode += ""[name='""+name+""']"";
    if(value != null && value != '' )
        findCode += ""[value^='""+value+""']"";
    if(onclick != null && onclick != '')
        findCode += ""[onclick*='""+onclick+""']"";
    var obj = $(findCode);
    if(returnIndex != null && returnIndex != 'undefined'){
        var index = parseInt(returnIndex);
        return obj.get(index);
    }
    else
        return obj.get(0);
}

function canInvoke(){ return findElement != null && findElement != undefined; }

function findElementByFilter(filter, returnIndex)
{
    var obj = $(filter);
    if(returnIndex != null && returnIndex != 'undefined'){
        var index = parseInt(returnIndex);
        return obj.get(index);
    }
    else
        return obj.get(0);
}

function invokeOnChange(obj){ 
    try{
        obj.fireEvent('onchange');
//        obj.onchange();
    }catch(e){
        alert('invokeonchange' + e.message + e.description);
    }
}";
        }

        protected override mshtml.IHTMLElement FindElement(System.Windows.Forms.HtmlDocument doc, FillParameter parameter, bool isContain, string elementType, int formIndex)
        {
            if (string.IsNullOrEmpty(parameter.SearchType))
                return InvokeScriptSync(doc, "findElement", new object[] { elementType, parameter.Name, parameter.Value, parameter.OnClick, parameter.ReturnIndex }) as mshtml.IHTMLElement;
            else
            {
                string filter = string.Format(parameter.SearchType, parameter.ParameterName == null ? null : string.Format("\"{0}\"", base.DataTable[parameter.ParameterName]));
                return InvokeScriptSync(doc, "findElementByFilter", new object[] { filter, parameter.ReturnIndex }) as mshtml.IHTMLElement;
            }
        }

        private void GetParameters()
        {
            Hashtable uniqueTable = new Hashtable();
            _parameters.Clear();
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
                            case "查找方式":
                                dElement.SearchType = value;
                                break;
                            case "返回第几个元素":
                                dElement.ReturnIndex = value;
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
                            key = new FillParameterKey(dElement.ParameterName, dElement.Type, dElement.CanAdd, dElement.IsRequired, dElement.SearchString);
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
    }
}
