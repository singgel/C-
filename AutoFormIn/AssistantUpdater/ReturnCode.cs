using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AssistantUpdater
{
    [DataContract]
    class ReturnCode
    {
        [DataMember]
        public string message
        {
            get;
            set;
        }
        [DataMember]
        public string code
        {
            get;
            set;
        }
    }
}
