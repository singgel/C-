using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LoggerUtils
{
    public class LoggerUtils
    {
        public static void logger(String info) {
            Console.WriteLine(info);
            Console.ReadLine();
        }

        public static void loggerTxt(String filename, String text) {

            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(text);
                sw.Close();
            }
        }
    }
}
