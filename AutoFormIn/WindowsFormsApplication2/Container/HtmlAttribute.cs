using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.Container
{
    public class HtmlAttribute
    {
        public HtmlAttribute()
        {
        }

        public HtmlAttribute(String elementId, String name, String elementType, String value)
        {
            this.elementId = elementId;
            this.name = name;
            this.elementType = elementType;
            this.value = value;
        }

        public String elementId;
        public String name;
        public String elementType;
        public String value;
    }
}
