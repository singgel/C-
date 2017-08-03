using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace Assistant.DataProviders
{
    class Default3CValueConverter : ValueConverter
    {
        private Hashtable _converter;

        public Default3CValueConverter()
        {
            _converter = new Hashtable();
        }

        public override object Convert(string fileName, string sheetName, object parameters)
        {
            ReadConverterRule(fileName, sheetName);
            TreeValue value = (TreeValue)parameters;
            EnumAndConvertTree(value);
            return value;
        }

        private void InnerConvert(FillValue3C fillValue, ConverterParameter parameter)
        {
            StringBuilder builder = new StringBuilder(fillValue.Value);
            if (string.IsNullOrEmpty(parameter.ValueReplaceGroup) == false)
            {
                string[] replaceGroup = parameter.ValueReplaceGroup.Split('|');
                foreach (var group in replaceGroup)
                {
                    string[] keyAndValue = group.Split(':');
                    if (keyAndValue.Length >= 2)
                    {
                        builder.Replace(keyAndValue[0], keyAndValue[1]);
                    }
                }
            }
            fillValue.Value = builder.ToString();
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
                            case "参数编号":
                                para.OriginName = str;
                                break;
                        }
                    }
                    if (string.IsNullOrEmpty(para.OriginName))
                        continue;
                    _converter.Add(para.OriginName, para);
                }
            }
        }

        private void EnumAndConvertTree(TreeValue tree)
        {
            if (tree != null)
            {
                if (tree.Values != null)
                {
                    foreach (DictionaryEntry entry in tree.Values)
                    {
                        if (_converter.ContainsKey(entry.Key))
                        {
                            InnerConvert(entry.Value as FillValue3C, _converter[entry.Key] as ConverterParameter);
                        }
                    }
                }
                if (tree.Children != null)
                {
                    foreach (var child in tree.Children)
                    {
                        EnumAndConvertTree(child);
                    }
                }
            }
        }
    }
}
