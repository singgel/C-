using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;

namespace Catarc.Adc.NewEnergyAccountSys.ControlUtils
{
    public class ComboBoxEditHelper
    {
        /// <summary>
        /// 根据数据库中的名字获取下拉框内容
        /// </summary>
        /// <param name="cbeName">下拉框的CODE</param>
        /// <returns>下拉框内容</returns>
        public static string[] getOptionsByName(string PARAM_CODE)
        {
            var obj = AccessHelper.GetSingle(AccessHelper.conn, String.Format("select CONTROL_VALUE from BASE_INFOMATION where PARAM_CODE='{0}'", PARAM_CODE));
            return obj.ToString().Split('/');
        }
    }
}
