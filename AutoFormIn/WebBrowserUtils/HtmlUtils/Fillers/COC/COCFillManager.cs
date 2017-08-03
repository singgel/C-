using Assistant.DataProviders;
using Office.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using WebBrowserUtils.ExtendWebBrowser;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class COCFillManager : WebFillManager
    {
        private ForwardExcelReader reader;
        private ForwardReadWorksheet sheet1, sheet2;
        private List<Hashtable> tableList;
        private int CurrentRowIndex = -1;
        private FillAsyncHandler _asyncObject;
        private System.Threading.Thread _fillThread;
        private bool waitForBrowser;

        public COCFillManager(WebBrowser2 browser, string dataFile)
            : base(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.FillRule))
        {
            base.FillType = "COC";
            base.Version = Properties.Resources.FillRule;
            _asyncObject = new FillAsyncHandler();
            _fillThread = new System.Threading.Thread(InnerBeginFill);
        }

        public COCFillManager(WebBrowser2 browser, string dataFile, string ruleFilePath)
            : base(browser, dataFile, ruleFilePath)
        {
            base.FillType = "COC";
            base.Version = Properties.Resources.FillRule;
            _asyncObject = new FillAsyncHandler();
        }

        public override void BeginFill()
        {
            _fillThread.Start();
        }

        public override void EndFill()
        {
            _fillThread.Abort();
            base.EndFill();
        }

        private void InnerBeginFill(object state)
        {
            try
            {
                using (reader = new ForwardExcelReader(base.DataProvider.DataSourceFile))
                {
                    reader.Open();
                    sheet1 = reader.Activate(1) as ForwardReadWorksheet;
                    sheet2 = reader.Activate(2) as ForwardReadWorksheet;
                    Hashtable table = ReadColumnSheet();
                    tableList = ReadRowSheet(table);
                    for (CurrentRowIndex = 0; CurrentRowIndex < tableList.Count; CurrentRowIndex++)
                    {
                        _asyncObject.Reset();
                        bool isBusy = true;
                        while (isBusy)
                        {
                            System.Threading.Thread.Sleep(100);
                            base.Browser.Invoke((Action)(() => { isBusy = base.Browser.IsBusy; }));
                        }
                        base.Browser.Invoke((Action)(() =>
                        {
                            base.Browser.Navigate(base.StartPageUri);
                        }));
                        base.BeginFill();
                        _asyncObject.Wait();
                    }
                    base.OnFinished(EventArgs.Empty);
                    WebFillManager.ShowMessageBox("填报已完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                if (ex is System.Threading.ThreadAbortException)
                    return;
                else
                    WebFillManager.ShowMessageBox(ex.Message, "错误", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        protected override void OnBrowserNavigating(WebBrowser2 browser, WebBrowserNavigatingEventArgs e)
        {
            base.OnBrowserNavigating(browser, e);
            if (e.Url.AbsolutePath == "/cocComplete!saveCocComplete.action")
            {
                base.EndFill();
                _asyncObject.Resume();
            }
        }

        protected override void OnFillerStateChanged(FillBase fill)
        {
            if (fill.FillState == FillState.End)
            {
                Uri url = null;
                try
                {
                    url = new Uri(this.EndPageUri);
                }
                catch (Exception)
                {
                    this.EndFill();
                    WebFillManager.ShowMessageBox(string.Format("填报类型{0}的EndFillPageUri的url地址不正确！", base.FillType), "错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
        }

        protected override FillBase CreateFill(Uri url, ExtendWebBrowser.WebBrowser2 browser)
        {
            CoCFiller filler = new CoCFiller(browser, url, Uris[url.AbsolutePath] as UrlParameter);
            filler.DataFile = base.DataProvider.DataSourceFile;
            filler.Converter = base.Converter;
            return filler;
        }

        protected override System.Collections.Hashtable GetUris()
        {
            if (base.Uris.Count != 0)
                return base.Uris;
            Hashtable uris = new Hashtable();
            Hashtable columnHeader = new Hashtable();
            string fileName = FileHelper.GetPublicPage(base.Version, base.FillType);
            using (Office.Excel.ForwardExcelReader pages = new Office.Excel.ForwardExcelReader(string.Format("{0}\\{1}", base.RuleFilePath, fileName)))
            {
                pages.Open();
                Office.Excel.ForwardReadWorksheet sheet = pages.Activate("页面汇总") as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                {
                    object content = null, uri = null;
                    if (sheet.ReadNextRow())
                    {
                        while (sheet.ReadNextCell(false))
                        {
                            columnHeader.Add(sheet.CurrentCell.ColumnIndex, sheet.GetContent());
                        }
                    }
                    UrlParameter urlParameter = null;
                    while (sheet.ReadNextRow()) // 读取下一行。
                    {
                        uri = null; content = null;
                        urlParameter = new UrlParameter(); // 存储Url对应的页面标签及Excel文件名称。
                        while (sheet.ReadNextCell(false)) // 读取本行的下一个单元格数据。
                        {
                            content = sheet.GetContent();
                            switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                            {
                                case "页面标签":
                                    urlParameter.LabelName = content == null ? "" : content.ToString();
                                    break;
                                case "所属Excel":
                                    urlParameter.IsPublicUrl = (content as string) == "公共页面";
                                    break;
                                case "URL":
                                    uri = content as string;
                                    break;
                            }
                        }
                        if (uri != null && string.IsNullOrEmpty(uri.ToString()) == false)
                        {
                            Uri url = new Uri(uri.ToString());
                            if (uris.ContainsKey(url.AbsolutePath))
                                continue;
                            uris.Add(url.AbsolutePath, urlParameter);
                        }
                    }
                }
            }
            return uris; 
        }

        protected override object GetData(object state)
        {
            return tableList[CurrentRowIndex];
        }

        private Hashtable ReadColumnSheet()
        {
            Hashtable data = new Hashtable();
            Hashtable keyList = new Hashtable();
            if (sheet2.ReadFollowingRow(6))
            {
                string content = null;
                object cellContent = null;
                string parameterName = "";
                while (sheet2.ReadNextCell(false))
                {
                    if (sheet2.CurrentCell.ColumnIndex == 3)
                    {
                        parameterName = sheet2.GetContent() as string;
                    }
                    else if (sheet2.CurrentCell.ColumnIndex > 3)
                    {
                        if (parameterName == null)
                            break;
                        cellContent = sheet2.GetContent();
                        content = cellContent == null ? null : cellContent.ToString();
                        Hashtable innerData = new Hashtable();
                        if (content != null)
                        {
                            innerData.Add(parameterName, content);
                            data.Add(content, innerData);
                            keyList.Add(sheet2.CurrentCell.ColumnIndex, content);
                        }
                    }
                }
                while (sheet2.ReadNextRow())
                {
                    parameterName = null;
                    while (sheet2.ReadNextCell(false))
                    {
                        short columnIndex = sheet2.CurrentCell.ColumnIndex;
                        if (columnIndex == 3)
                        {
                            parameterName = sheet2.GetContent() as string;
                        }
                        else if (columnIndex > 3)
                        {
                            if (parameterName == null)
                                break;
                            string key = keyList[columnIndex] as string;
                            cellContent = sheet2.GetContent();
                            content = cellContent == null ? null : cellContent.ToString();
                            Hashtable innerData = data[key] as Hashtable;
                            if (innerData != null)
                            {
                                System.Text.RegularExpressions.Match match = null;
                                switch (parameterName)
                                {
                                    case "CCC认证引用的标准号和实施阶段":
                                        if (string.IsNullOrEmpty(content))
                                            continue;
                                        match = System.Text.RegularExpressions.Regex.Match(content, @",?\s*(?<standard>GB[^0-9]*[0-9]+(\.[0-9]+)?\-[0-9]{4})\s*(?<em>[^,\s]*)\s*");
                                        while (match.Success)
                                        {
                                            string standard = match.Groups["standard"].Value;
                                            if (string.IsNullOrEmpty(standard) == false)
                                            {
                                                innerData.Add(standard, match.Groups["em"].Value);
                                                innerData["CCC认证引用的标准号和实施阶段"] = string.Format("{0}{1}",
                                                    innerData["CCC认证引用的标准号和实施阶段"] == null ? null : string.Format("{0}|", innerData["CCC认证引用的标准号和实施阶段"]), standard);
                                            }
                                            match = match.NextMatch();
                                        }
                                        break;
                                    case "CCC认证引用的标准号":
                                        innerData.Add("CCC认证引用的标准号", content == null ? "" : content.Replace(',', '|'));
                                        break;
                                    case "排气（液体燃料）":
                                        if (string.IsNullOrEmpty(content))
                                            continue;
                                        match = System.Text.RegularExpressions.Regex.Match(content, @"(?<name>[^:]+)\s*:\s*(?<value>[^\s]*)\s*");
                                        while (match.Success)
                                        {
                                            string name = match.Groups["name"].Value;
                                            string value = match.Groups["value"].Value;
                                            if (string.IsNullOrEmpty(name) == false)
                                            {
                                                innerData.Add(string.Format("{0}（液体）", name), value);
                                            }
                                            match = match.NextMatch();
                                        }
                                        break;
                                    case "排气（气体燃料）":
                                        if (string.IsNullOrEmpty(content))
                                            continue;
                                        match = System.Text.RegularExpressions.Regex.Match(content, @"(?<name>[^:]+)\s*:\s*(?<value>[^\s]*)\s*");
                                        while (match.Success)
                                        {
                                            string name = match.Groups["name"].Value;
                                            string value = match.Groups["value"].Value;
                                            if (string.IsNullOrEmpty(name) == false)
                                            {
                                                innerData.Add(string.Format("{0}（气体）", name), value);
                                            }
                                            match = match.NextMatch();
                                        }
                                        break;
                                    case "轮距（mm）":
                                        //前：1600  后：1627 
                                        if (string.IsNullOrEmpty(content))
                                            continue;
                                        match = System.Text.RegularExpressions.Regex.Match(content, @"前：(?<front>[0-9]+)\s*后：(?<back>[0-9]+)\s*");
                                        if (match.Success)
                                        {
                                            string front = match.Groups["front"].Value;
                                            string back = match.Groups["back"].Value;
                                            if (string.IsNullOrEmpty(front) == false)
                                            {
                                                innerData.Add("轮距（mm）", string.Format("{0}/{1}", front, back));
                                            }
                                            match = match.NextMatch();
                                        }
                                        break;
                                    case "轮胎规格":
                                        //第1轴：225/55 R17  第2轴：225/55 R17 
                                        if (string.IsNullOrEmpty(content))
                                            continue;
                                        match = System.Text.RegularExpressions.Regex.Match(content, @"第1轴：(?<first>[0-9]+/[0-9]+\s*R[0-9]+)\s*第2轴：(?<second>[0-9]+/[0-9]+\s*R[0-9]+)?\s*");
                                        if (match.Success)
                                        {
                                            string first = match.Groups["first"].Value;
                                            string second = match.Groups["second"].Success ? match.Groups["second"].Value : first;
                                            if (string.IsNullOrEmpty(first) == false)
                                            {
                                                innerData.Add("轮胎规格（第一轴）", first);
                                                innerData.Add("轮胎规格（第二轴）", second);
                                            }
                                            match = match.NextMatch();
                                        }
                                        break;
                                    case "CCC证书号（须包含版本号）":
                                        if (string.IsNullOrEmpty(content))
                                            continue;
                                        match = System.Text.RegularExpressions.Regex.Match(content, @"(?<cert>[0-9]+)\s*\((?<version>[0-9]+)\)");
                                        if (match.Success)
                                        {
                                            string cert = match.Groups["cert"].Value;
                                            string version = match.Groups["version"].Success ? match.Groups["version"].Value : "";
                                            if (string.IsNullOrEmpty(cert) == false)
                                            {
                                                innerData.Add("CCC证书编号", cert);
                                                innerData.Add("CCC证书版本号", version);
                                            }
                                            match = match.NextMatch();
                                        }
                                        break;
                                    default:
                                        innerData.Add(parameterName, content);
                                        break;
                                }
                            }
                        }
                    }
                }

            }
            return data;
        }

        private List<Hashtable> ReadRowSheet(Hashtable columnData)
        {
            Hashtable current = null, last = null;
            List<Hashtable> list = new List<Hashtable>();
            Hashtable columnHeader = new Hashtable();
            if (sheet1.ReadFollowingRow(5))
            {
                while (sheet1.ReadNextCell(false))
                {
                    columnHeader.Add(sheet1.CurrentCell.ColumnIndex, sheet1.GetContent());
                }
                System.Text.RegularExpressions.Match match = null;
                string str = "";
                while (sheet1.ReadNextRow())
                {
                    match = null;
                    current = new Hashtable();
                    while (sheet1.ReadNextCell(false))
                    {
                        object content = sheet1.GetContent();
                        str = content == null ? "" : content.ToString();
                        switch (columnHeader[sheet1.CurrentCell.ColumnIndex] as string)
                        {
                            case "Model":
                                current.Add("车型系列代号", str);
                                break;
                            case "CoC No. regulation":
                                current.Add("车辆一致性证书编号", str);
                                if (last == null)
                                    current.Add("Key", str);
                                break;
                            case "Product":
                                if (string.IsNullOrEmpty(str) && last != null)
                                {
                                   if(current.ContainsKey("Key") == false)
                                        current.Add("Key", last["Key"]);
                                   current.Add("车型名称", last["车型名称"]);
                                }
                                else
                                {
                                    match = System.Text.RegularExpressions.Regex.Match(str, "(?<=\r\n).*");
                                    if (match.Success)
                                    {
                                        current.Add("车型名称", match.Value);
                                    }
                                }
                                break;
                            case "Model Code":
                                if (string.IsNullOrEmpty(str) && last != null)
                                {
                                    current.Add("单元代号", last["单元代号"]);
                                }
                                else
                                {
                                    match = System.Text.RegularExpressions.Regex.Match(str, ".*(?=\r\n)");
                                    if (match.Success)
                                    {
                                        current.Add("单元代号", match.Value);
                                    }
                                }
                                break;
                            case "轮胎规格":
                                if (str == null)
                                    continue;
                                match = System.Text.RegularExpressions.Regex.Match(str, @"[0-9]{3,}/[0-9]+\s*R[0-9]+");
                                if (match.Success)
                                {
                                    string first = match.Value, second = "";
                                    current.Add("轮胎规格（第一轴）",  first);
                                    match = match.NextMatch();
                                    if (match.Success)
                                        second = match.Value;
                                    else
                                        second = first;
                                    current.Add("轮胎规格（第二轴）", second);
                                }
                                break;
                            case "前轮距":
                                current.Add("前轮距", str);
                                break;
                            case "后轮距":
                                current.Add("轮距（mm）", string.Format("{0}/{1}", current["前轮距"], str));
                                current.Remove("前轮距");
                                break;
                            case "车长":
                                current.Add("车长（mm）", str);
                                break;
                            case "前悬":
                                current.Add("前悬（mm）", str);
                                break;
                            case "后悬":
                                current.Add("后悬（mm）", str);
                                break;
                            case "接近角":
                                current.Add("接近角(º)", str);
                                break;
                            case "离去角":
                                current.Add("离去角(º)", str);
                                break;
                        }
                    }
                    string key = current["Key"] as string;
                    if (key == null)
                        continue;
                    Hashtable otherData = columnData[key] as Hashtable;
                    if(otherData != null)
                    {
                        foreach (DictionaryEntry entry in otherData)
                        {
                            if (current.ContainsKey(entry.Key))
                            {
                                if (string.IsNullOrEmpty(current[entry.Key] as string))
                                {
                                    current[entry.Key] = entry.Value;
                                }
                            }
                            else
                                current.Add(entry.Key, entry.Value);
                        }
                        current.Add("长度/前悬/轴距/后悬(mm)", string.Format("{0}/{1}/{2}/{3}", current["车长（mm）"], current["前悬（mm）"], current["轴距（mm）"], current["后悬（mm）"]));
                    }
                    list.Add(current);
                    last = current;
                }
            }
            return list;
        }
    }
}
