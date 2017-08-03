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
    public class Param
    {
        //参数中文名
        public String cname;
        //参数英文名
        public String ename;
        //型式认证编号（如果是配置信息则没有型式认证编号）
        public String patacode;
        //参数值
        public String value;

        public Param(String cname, String patacode, String value) {
            this.cname = cname;
            this.patacode = patacode;
            this.value = value;
        }

        public Param()
        {
 
        }
    }
}
