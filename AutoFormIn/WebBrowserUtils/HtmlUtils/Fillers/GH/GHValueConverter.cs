using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Converter;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    class GHValueConverter : ValueConverter
    {
        private List<ConverterParameter> _converter;
        private static readonly System.Text.RegularExpressions.Regex replaceItem;
        public GHValueConverter()
            : base()
        {
            _converter = new List<ConverterParameter>();
        }

        static GHValueConverter()
        {
            replaceItem = new System.Text.RegularExpressions.Regex(@"(?<origin>[^:]+):(?<actual>[^\|]+)");
        }

        protected override System.Collections.Hashtable ConvertProtected(string fileName, System.Collections.Hashtable table)
        {
            this.ReadConverterRule(fileName);
            StringBuilder result = new StringBuilder();
            foreach (var item in _converter)
            {
                if (table.ContainsKey(item.EntName) == false)
                    continue;
                string value = table[item.EntName] as string;
                result.Clear();
            }
            return null;
        }

        private void GetValue(string originValue, ConverterParameter para, StringBuilder result)
        {
            if (string.IsNullOrEmpty(para.SplitParten))
            {
                if (string.IsNullOrEmpty(para.RegexParten))
                {
                }
            }
            foreach (char c in para.SplitParten.Reverse())
            {
            }
        }

        private Hashtable GetReplace(string replaceGroup)
        {
            if (string.IsNullOrEmpty(replaceGroup))
                return null;
            System.Text.RegularExpressions.Match match = replaceItem.Match(replaceGroup);
            Hashtable table = new Hashtable();
            while (match.Success)
            {
                if (string.IsNullOrEmpty(match.Groups["origin"].Value))
                {
                    match = match.NextMatch();
                    continue;
                }

            }
            return null;
        }

        private void ReadConverterRule(string fileName)
        {
            _converter.Clear();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(fileName))
            {
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
                if (sheet == null)
                    return;
                Hashtable columnHeader = new Hashtable();
                while (sheet.ReadNextRow())
                {
                    columnHeader.Add(sheet.CurrentCell.ColumnIndex, sheet.GetContent());
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
                            para.ReplaceGroup = str;
                            break;
                        case "企业内部参数编号":
                            para.EntName = str;
                            break;
                        case "原参数名称":
                            para.OriginName = str;
                            break;
                        case "填报级数分割符":
                            para.RegexParten = str;
                            break;
                        case "参数拆分正则":
                            para.SplitParten = str;
                            break;
                        }
                    }
                    if (para.OriginName == null)
                        continue;
                    _converter.Add(para);
                }
            }
        }
    }
}
