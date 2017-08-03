using Assistant.DataProviders;
using System;
using System.Collections;
using System.Windows.Forms;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class PZHFillManager : WebFillManager
    {
        public static readonly String[] regularSeparator = new String[]{"、", "/"};
        public const String regularJoiner = "、";

        public PZHFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile)
            : this(browser, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.FillRule))
        {
        }

        internal PZHFillManager(ExtendWebBrowser.WebBrowser2 browser, string dataFile, string ruleFilePath)
            : base(browser, dataFile, ruleFilePath)
        {
            this.FillType = "配置号";
            this.Version = Properties.Resources.FillRule;
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
            PZHFiller filler = new PZHFiller(browser, url, Uris[url.AbsolutePath] as UrlParameter);
            filler.DefaultValue = base.CurrentValue;
            filler.DataFile = base.DataProvider.DataSourceFile;
            return filler;
        }

        protected override void OnBrowserDisposed(ExtendWebBrowser.WebBrowser2 browser)
        {
            if (base.CurrentFill != null)
                base.CurrentFill.Resume();
        }

        protected override Hashtable GetUris()
        {
            Hashtable uris = new Hashtable();
            Hashtable columnHeader = new Hashtable();
            string fileName = FileHelper.GetPublicPage(this.Version, this.FillType);
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
            return _data;
        }
    }
}
