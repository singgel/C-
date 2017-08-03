using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders.SHFY
{
    public class ValueConverter : WebValueConverter
    {
        private Hashtable files;

        public ValueConverter()
        {
        }

        public override Hashtable Convert(string fileName, string sheetName, Hashtable table)
        {
            files = FileHelper.GetAllFilesFromDirectory(base.DataFilePath);
            return base.Convert(fileName, sheetName, table);
        }

        internal override string GetValue(object value, ConverterParameter parameter)
        {
            object[] array = value as object[];
            if (array == null || array.Length == 0)
                return "";
            string str = array[0] as string;
            if (string.IsNullOrEmpty(str) && parameter.ElementType != null)
            {
                switch (parameter.ElementType.ToUpper())
                {
                case "FILE":
                case "BUTTON":
                case "BUTTON/SUBMIT":
                case "SUBMIT":
                    if(array.Length>=2)
                    {
                        str = array[1] as string;
                        if (str != null && files.ContainsKey(str))
                        {
                            List<string> fileList = files[str] as List<string>;
                            if (fileList != null)
                            {
                                str = fileList[0];
                            }
                        }
                    }
                    break;
                }
            }
            return str;
        }
    }
}
