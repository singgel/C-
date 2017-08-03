using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.Entity
{
    /// <summary>
    /// 单个参数类
    /// 
    /// </summary>
    public class EngineParam : Param
    {
        public String type;

        public EngineParam(String cname, String patacode, String value) : base(cname, patacode, value) {
            this.type = ParamType.engineParam;
        }

        public EngineParam() : base()
        {
            this.type = ParamType.engineParam;
        }
    }
}
