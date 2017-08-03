using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace AssistantUpdater
{
    [DataContract]
    public class Enterprise
    {
        //{"entId":7,"entEname":"test4","entCname":"测试企业四","licenseCnt":12}
        [DataMember]
        public string entId
        {
            get;
            set;
        }

        [DataMember]
        public string entEname
        {
            get;
            set;
        }

        [DataMember]
        public string entCname
        {
            get;
            set;
        }
    }
}
