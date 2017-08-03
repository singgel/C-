using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public class FillRecord
    {
        private int count;
        private ElementType elementType;
        private string _Note;
        private string _ParaName;
        private RecordType _type;

        public ElementType ElementType
        {
            get { return elementType; }
        }

        public string Note
        {
            get {
                if (_type == Fillers.RecordType.Success)
                    return string.Format("个数：{0}", count);
                return _Note; 
            }
        }

        public string ParaName
        {
            get { return _ParaName; }
        }

        public int RecordCount
        {
            get { return count; }
            set { count = value; }
        }

        public RecordType RecordType
        {
            get { return _type; }
        }

        public FillRecord(ElementType elementType, RecordType type)
            : this(elementType, type, null, null)
        {
        }

        public FillRecord(ElementType elementType, RecordType type, string note)
            :this(elementType, type, note, null)
        {
        }

        public FillRecord(ElementType elementType, RecordType type, string note, string paraName)
        {
            this.elementType = elementType;
            _type = type;
            _Note = note;
            _ParaName = paraName;
            count = 0;
        }
    }
}
