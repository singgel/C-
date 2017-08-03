using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AssistantUpdater
{
    [DataContract]
    public class LoginUser
    {
        public string password
        {
            get;
            internal set;
        }
        //{\"status\":\"Y\",\"roleId\":\"1\",\"catId\":\"1\"}
        public string userName
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

        [DataMember]
        public string catId
        {
            get;
            set;
        }

        [DataMember]
        public string status
        {
            get;
            set;
        }
    }
}
