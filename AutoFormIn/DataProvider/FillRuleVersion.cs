using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    public class FillRuleVersion:IEquatable<FillRuleVersion>
    {
        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public bool UseAppendix
        {
            get;
            set;
        }

        public override int GetHashCode()
        {
            return this.Name == null ? 0 : this.Name.GetHashCode();
        }

        public bool Equals(FillRuleVersion other)
        {
            return (object.Equals(other, null) ? false : this.Name == other.Name);
        }

        public static bool operator ==(FillRuleVersion item1, FillRuleVersion item2)
        {
            return (object.Equals(item1, null) ? (object.Equals(item2, null) ? true : item2.Equals(item1)) : item1.Equals(item2));
        }

        public static bool operator !=(FillRuleVersion item1, FillRuleVersion item2)
        {
            return !(item1 == item2);
        }
    }
}
