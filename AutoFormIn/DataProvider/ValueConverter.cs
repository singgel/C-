using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    public abstract class ValueConverter
    {
        public string DataFilePath
        {
            get;
            set;
        }

        public abstract object Convert(string fileName, string sheetName, object parameters);
    }
}
