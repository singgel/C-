using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace ReflectorEncrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            Stream sr = Assembly.GetExecutingAssembly().GetManifestResourceStream("命名空间.程序.exe");
            byte[] fileBytes = new byte[sr.Length];
            sr.Read(fileBytes, 0, (int)sr.Length - 1);
            Assembly assembly = Assembly.Load(fileBytes);
            MethodInfo mi = assembly.EntryPoint;
            mi.Invoke(null, null);
        }
    }
}
