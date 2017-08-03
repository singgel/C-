using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebBrowserUtils.ExtendWebBrowser;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class XNYFillManager : WebFillManager
    {
        private const string carTypeName = "车辆类型";
        private Hashtable _carTypeList;

        public override string CarType
        {
            get
            {
                return base.Data == null ? null : base.Data["CarType"] as string;
            }
            set
            {
                if (base.Data == null || _carTypeList == null)
                    return;
                base.Data["CarType"] = value;
                base.Data[carTypeName] = _carTypeList[value] as string;
            }
        }

        public override string Standard
        {
            get
            {
                return base.Data == null ? null : base.Data["排放标准"] as string;
            }
            set
            {
                if (base.Data == null)
                    return;
                base.Data["排放标准"] = value;
            }
        }

        public XNYFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile)
            : this(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.FillRule))
        {
        }

        internal XNYFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile, string ruleFilePath)
            : base(browser, dataFile, ruleFilePath)
        {
            base.FillType = "新能源";
            base.Version = Properties.Resources.FillRule;
        }

        protected override void OnFillerStateChanged(FillBase fill)
        {
            if (fill.FillState == FillState.Exception)
            {
                WebFillManager.ShowMessageBox(string.Format("填报过程发生错误，异常信息：{0}", fill.Exception.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            base.OnFillerStateChanged(fill);
        }

        protected override FillBase CreateFill(Uri url, ExtendWebBrowser.WebBrowser2 browser)
        {
            return new XNYFiller(browser, url, Uris[url.AbsolutePath] as XNYUrlParameter) { DataFile = this.DataProvider.DataSourceFile };
        }
         
        protected override Hashtable GetUris()
        {
            Hashtable uris = new Hashtable();
            //读取新能源下面的公共页面（路径）
            string fileName = FileHelper.GetPublicPage(base.Version, base.FillType);
            using (Office.Excel.ForwardExcelReader pages = new Office.Excel.ForwardExcelReader(string.Format("{0}\\{1}", base.RuleFilePath, fileName)))
            {
                pages.Open();
                Office.Excel.ForwardReadWorksheet sheet = pages.Activate("页面汇总") as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                {
                    XNYUrlParameter urlParameter = null;
                    Uri url = null;
                    Hashtable columnHeader = GetColumnHeader(sheet);
                    while (sheet.ReadNextRow()) // 读取下一行。
                    {
                        urlParameter = new XNYUrlParameter();
                        while (sheet.ReadNextCell(false)) // 读取本行的下一个单元格数据。
                        {
                            switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                            {
                            case "URL":
                                url = new Uri(sheet.GetContent() as string);
                                break;
                            case "页面标签":
                                urlParameter.LabelName = sheet.GetContent() as string;
                                break;
                            case "所属Excel":
                                urlParameter.IsPublicUrl = ((sheet.GetContent() as string) == "公共页面");
                                break;
                            case "表数据":
                                urlParameter.IsTableData = ((sheet.GetContent() as string) == "是");
                                break;
                            }
                        }
                        if (url != null && string.IsNullOrEmpty(url.ToString()) == false)
                        {
                            if (uris.ContainsKey(url.AbsolutePath))
                                continue;
                            uris.Add(url.AbsolutePath, urlParameter);
                        }
                    }
                }
                sheet = pages.Activate(carTypeName) as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                    _carTypeList = ReadCarTypeList(sheet);
                else
                    throw new ArgumentException(string.Format("填报规则中没有名为“{0}”的工作表。", carTypeName));
            }
            return uris;
        }

        protected override object GetData(object state)
        {
            Hashtable _data = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(base.DataProvider.DataSourceFile))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
                if (sheet == null)
                    return null;
                object parameter, content;
                while (sheet.ReadNextRow())
                {
                    parameter = null;
                    content = null;
                    while (sheet.ReadNextCell(false))
                    {
                        if (sheet.CurrentCell.ColumnIndex == 1)
                            parameter = sheet.GetContent();
                        else if (sheet.CurrentCell.ColumnIndex == 2)
                            content = sheet.GetContent();
                    }
                    if (parameter == null)
                        continue;
                    _data.Add(parameter, content == null ? "" : content.ToString());
                }
            }
            string carType = _data[carTypeName] as string;
            if (carType != null)
            {
                _data[carTypeName] = _carTypeList[carType];
                _data.Add("CarType", carType);
            }
            return _data;
        }

        private Hashtable ReadCarTypeList(Office.Excel.ForwardReadWorksheet sheet)
        {
            object content = null;
            Hashtable relation = new Hashtable();
            Hashtable columnHeader = GetColumnHeader(sheet);
            string key = "", value = "";
            while (sheet.ReadNextRow())
            {
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "全称":
                        key = content == null ? "" : content.ToString();
                        break;
                    case "缩写":
                        value = content == null ? "" : content.ToString();
                        break;
                    }
                }
                if (string.IsNullOrEmpty(key))
                    continue;
                relation.Add(key, value);
            }
            return relation;
        }
        /// <summary>
        /// 查找指定WebBrowser中的填报线程。
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private HtmlDocument FindDocument(Uri url, HtmlDocument doc)
        {
            if (doc == null)
                return null;
            if (doc.Url == url)
                return doc;
            HtmlDocument finded = null;
            foreach (HtmlWindow item in doc.Window.Frames)
            {
                finded = FindDocument(url, item.Document);
                if (finded != null)
                    return finded;
            }
            return null;
        }

        protected override void OnBrowserDisposed(ExtendWebBrowser.WebBrowser2 browser)
        {
            if (base.CurrentFill != null)
            {
                
            }
            //base.OnBrowserDisposed(browser);
        }

        //加载完成事件
        protected override void OnDocumentCompleted(ExtendWebBrowser.WebBrowser2 browser, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(browser, e);
            if(base.CurrentFill != null)
            {
                string uri = e.Url.AbsolutePath;
                if (uri == "/motor/car/car-declare-step6.action" || uri == "/motor/car/car-declare-step33.action" || uri == "/motor/part/carpart!method.action")
                {
                    base.CurrentFill.Resume();
                }
            }
        }
    }
}
