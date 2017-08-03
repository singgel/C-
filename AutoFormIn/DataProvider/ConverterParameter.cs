using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    internal class ConverterParameter
    {
        public string OriginName, EntName, SplitParten, RegexParten, ValueReplaceGroup, MergePara, MergeSpliter, DefaultValue, CalcExpression, ElementType;
        public bool IsSheetRelation, UseOriginString, IsRequired;
    }
}
