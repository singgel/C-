using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.Container
{
    public class OtherCheckBoxAttritute:HtmlAttribute
    {
        public OtherCheckBoxAttritute()
            : base()
        { }

        public OtherCheckBoxAttritute(String elementId, String name, String elementType, String value,
            String divId, String check)
            : base(elementId, name, elementType, value)
        {
            this.divId = divId;
            this.check = check;
        }

        public String divId = null;

        public String check = null;
    }
}
