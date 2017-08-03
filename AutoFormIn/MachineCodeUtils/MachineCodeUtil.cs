using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using LoggerUtils;

namespace MachineCodeUtils
{

    public class MachineCodeUtil
    {

        public MachineCodeUtil()
        {
        }

        public static void Main(String[] args) {
            LoggerUtils.LoggerUtils.logger(GetComputerInfo());
        
        }

        public static string GetComputerInfo()
        {
            string info = string.Empty;
            string cpu = GetCPUInfo();
            string baseBoard = GetBaseBoardInfo();
            string bios = GetBIOSInfo();
            //string mac = GetMACInfo();
            info = string.Concat(cpu, baseBoard, bios);
            return info;
        }

        // CPU
        public static string GetCPUInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_Processor", "ProcessorId");
            return info;
        }

        // BIOS
        public static string GetBIOSInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BIOS", "SerialNumber");
            return info;
        }

        // 主板
        public static string GetBaseBoardInfo()
        {
            string info = string.Empty;
            info = GetHardWareInfo("Win32_BaseBoard", "SerialNumber");
            return info;
        }

        // MAC
        public static string GetMACInfo()
        {
            ManagementClass mc;
            ManagementObjectCollection moc;
            mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            moc = mc.GetInstances();
            string str = "";
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"])
                    str = mo["MacAddress"].ToString();

            }

            return str;
        }

        private static string GetHardWareInfo(string typePath, string key)
        {
            try
            {
                ManagementClass managementClass = new ManagementClass(typePath);
                ManagementObjectCollection mn = managementClass.GetInstances();
                PropertyDataCollection properties = managementClass.Properties;
                foreach (PropertyData property in properties)
                {
                    if (property.Name == key)
                    {
                        foreach (ManagementObject m in mn)
                        {
                            return m.Properties[property.Name].Value.ToString();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //这里写异常的处理    
            }
            return string.Empty;
        }

    }
}
