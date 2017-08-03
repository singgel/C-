using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class FillParameter3C
    {
        public static char[] DefaultSeparators;
        public string Key, Type, EditType;
        public char[] Separators;
        public bool IsComboBoxPreciseMatch;

        public FillParameter3C()
        {
            Separators = DefaultSeparators;
        }

        static FillParameter3C()
        {
            DefaultSeparators = new char[] { ';', '@' };
        }

        public void SetSeparator(string separator)
        {
            if (string.IsNullOrEmpty(separator))
                Separators = DefaultSeparators;
            else
            {
                Separators = new char[separator.Length];
                int index = 0;
                foreach (char c in separator)
                {
                    Separators[index++] = c;
                }
            }
        }
    }
}
