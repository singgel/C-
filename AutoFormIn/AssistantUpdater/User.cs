using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AssistantUpdater
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string status
        {
            get;
            set;
        }

        [DataMember]
        public string catId
        {
            get;
            set;
        }

        [DataMember]
        public string roleId
        {
            get;
            set;
        }
    }
}
