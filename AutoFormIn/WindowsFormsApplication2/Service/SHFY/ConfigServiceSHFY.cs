using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelUtils;
using Assistant.Container;
using Assistant.Entity;
using Assistant.Matcher;
using Assistant.ConfigCodeService;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace Assistant.Service
{
    class ConfigServiceForSHFY
    {
        public static Boolean setConfigCode(CarParams cp)
        {
            ConfigCodeService.IPackageInfoServiceService piis = new IPackageInfoServiceService();
            String configCode = cp.configCode;
            String packageCode = cp.packageCode;
            String version = cp.version;
            if (!String.IsNullOrEmpty(configCode) && !String.IsNullOrEmpty(packageCode))
            {
                String result = piis.setConfigCode(packageCode, configCode, version);
                if (result.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static Boolean testSetConfigCode()
        {
            ConfigCodeService.IPackageInfoServiceService piis = new IPackageInfoServiceService();
            String result = piis.setConfigCode("test", "test", "1");
            if (result.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //重复多次提交配置号
        public static Boolean setConfigCodeUtil(CarParams cp, int times)
        {
            if (times <= 0)
            {
                times = 1;
            }
            Boolean result = false;
            for (int i = 0; i < times; i++)
            {
                result = setConfigCode(cp);
                if (result)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }
            return result;
        }
    }
}
