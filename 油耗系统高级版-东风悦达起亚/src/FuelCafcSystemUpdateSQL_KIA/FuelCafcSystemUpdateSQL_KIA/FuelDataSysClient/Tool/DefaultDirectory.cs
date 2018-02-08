using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FuelDataSysClient.Tool
{
    public class DefaultDirectory
    {

        public static string MonitorSysMsg
        {
            get { return DefaultDirectory.initDirectory("MonitorSysMsg"); }
        }

        public static string Signature
        {
            get { return DefaultDirectory.initDirectory("Signature"); }
        }

        public static string ChangeDoc
        {
            get { return DefaultDirectory.initDirectory("ChangeDoc"); }
        }

        public static string TempDoc
        {
            get { return DefaultDirectory.initDirectory("TempDoc"); }
        }

        public static string initDirectory(string folder)
        {
            string filePath = Path.Combine(System.Windows.Forms.Application.StartupPath, folder);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            return filePath;
        }
    }
}
