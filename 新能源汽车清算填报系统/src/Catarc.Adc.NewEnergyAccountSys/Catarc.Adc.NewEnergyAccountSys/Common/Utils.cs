using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;  
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using Microsoft.Win32;

namespace Catarc.Adc.NewEnergyAccountSys.Common
{
    public class Utils
    {
        private static string getInstalPath()
        {
            try
            {
                RegistryKey sub = Registry.CurrentUser.OpenSubKey(@"Software\CATARC ADC-SOFT\NewEnergyAccountSys");
                var installPath = sub.GetValue("NewEnergyAccountSys");
                return installPath == null ? AppDomain.CurrentDomain.BaseDirectory : Path.GetDirectoryName(installPath.ToString());
            }
            catch
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string installPath = getInstalPath();//系统安装路径
        public static string dataPath = Path.Combine(Utils.installPath, "data\\data.mdb");//数据库路径
        public static string tempctreatepath = Path.Combine(Utils.installPath, "IMAGE_Temp\\Temp_Create\\");//临时单条图片创建路径
        public static string tempviewpath = Path.Combine(Utils.installPath, "IMAGE_Temp\\Temp_View\\");//临时单条pdf生成路径
        public static string temppdfname = "temp.pdf";//pdf文件名

        public static string tempFilePath6 = Path.Combine(Utils.installPath, "IMAGE_Temp\\Temp_ImportNewInfo\\IMAGEBill");//临时导入发票路径
        public static string tempFilePath7 = Path.Combine(Utils.installPath, "IMAGE_Temp\\Temp_ImportNewInfo\\IMAGEDrive");//临时导入行驶证路径
        public static string billImage = Path.Combine(Utils.installPath, "IMAGE\\IMAGEBill");//发票图片路径
        public static string driveImage = Path.Combine(Utils.installPath, "IMAGE\\IMAGEDrive");//行驶证图片路径

    }
}
