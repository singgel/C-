using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.Container
{
    public class OtherSelectAttribute:HtmlAttribute
    {
        public OtherSelectAttribute()
            : base()
        { 
        }

        public OtherSelectAttribute(String elementId, String name, 
            String contextId,
            String value)
            : base(elementId, name, "select", value)
        {
            this.contextId = contextId;
        }

        public String contextId = null;
    }
}
