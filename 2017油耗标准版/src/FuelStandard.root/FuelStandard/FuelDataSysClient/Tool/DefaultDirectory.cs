using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FuelDataSysClient.Tool
{
    public class DefaultDirectory
    {

        public string MonitorSysMsg
        {
            get { return this.initDirectory("MonitorSysMsg"); }
        }

        public string Signature
        {
            get { return this.initDirectory("Signature"); }
        }

        public string ChangeDoc
        {
            get { return this.initDirectory("ChangeDoc"); }
        }

        public string initDirectory(string folder)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            return filePath;
        }
    }
}
