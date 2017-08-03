using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Converter
{
    public abstract class ValueConverter
    {
        public string Type
        {
            get;
            protected set;
        }

        public ValueConverter()
        {
        }

        public Hashtable Converte(string fileName, Hashtable table)
        {
            return ConvertProtected(fileName, table);
        }

        protected abstract Hashtable ConvertProtected(string fileName, Hashtable table);
    }
}
