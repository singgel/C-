using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assistant;

namespace Assistant.Service
{
    /// <summary>
    /// 整个程序初始化
    /// </summary>
    class InitialService
    {
       
        public Boolean initialize() {
            String instName = Properties.Settings.Default.INSTNAME;
            if (String.IsNullOrEmpty(instName)) {
                throw new Exception("机构名称为空");
            }
            if (instName == InstName.HCBM) {



            }
            else if (instName == InstName.SHFY) { 
            
            
            
            
            }


            return true;
        }

    }
}
