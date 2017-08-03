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
    public class ConfigParam : Param
    {
        public String type;

        public ConfigParam(String cname, String patacode, String value)
            : base(cname, patacode, value)
        {
            this.type = ParamType.configParam;
        }

        public ConfigParam()
            : base()
        {
            this.type = ParamType.configParam;
        }
    }
}
