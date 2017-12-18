using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FuelDataSynchroWindowsService.Utils
{
    public class LogManager
    {
        //protected string strLogFileName = System.AppDomain.CurrentDomain.BaseDirectory + @"log\" + DateTime.Today.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo) + ".log";
        private readonly string strLogFileName = String.Format(@"{0}log\LOG{1}.log", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo));
        private readonly string strCafcFileName = String.Format(@"{0}log\CAFC_LOG{1}.log", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo));
        private readonly string strLFileName = String.Format(@"{0}log\L{1}.dat", System.AppDomain.CurrentDomain.BaseDirectory, DateTime.Today.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo));

        public LogManager()
        {

        }

        public LogManager(string strFileName)
        {
            this.strLogFileName = strFileName;
        }
        /// <summary>
        ///	写入错误日志
        /// </summary>
        /// <param name="strLogFileName">日志文件名称</param>
        /// <param name="strMessage">错误信息主题</param>
        /// <param name="strSource">错误来源</param>
        public void WriteToFile(string strSource)
        {
            lock (typeof(LogManager))
            {
                string path = Path.GetDirectoryName(strLFileName);
                if (!Directory.Exists(path))//如果不存在就创建path文件夹
                {
                    Directory.CreateDirectory(path);
                }

                FileInfo fi1 = new FileInfo(strLFileName);

                if (!fi1.Exists)
                {
                    using (StreamWriter sw = fi1.CreateText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
                else
                {
                    using (StreamWriter sw = fi1.AppendText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
            }
        }
        public void WriteToFile(string strFileName,string strSource)
        {
            lock (typeof(LogManager))
            {
                string path = Path.GetDirectoryName(strLFileName);
                if (!Directory.Exists(path))//如果不存在就创建path文件夹
                {
                    Directory.CreateDirectory(path);
                }

                FileInfo fi1 = new FileInfo(strFileName);

                if (!fi1.Exists)
                {
                    using (StreamWriter sw = fi1.CreateText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
                else
                {
                    using (StreamWriter sw = fi1.AppendText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
            }
        }
        /// <summary>
        ///	写入错误日志
        /// </summary>
        /// <param name="strLogFileName">日志文件名称</param>
        /// <param name="strMessage">错误信息主题</param>
        /// <param name="strSource">错误来源</param>
        public void WriteToLFile(string strSource)
        {
            lock (typeof(LogManager))
            {
                FileInfo fi1 = new FileInfo(strLFileName);

                if (!fi1.Exists)
                {
                    using (StreamWriter sw = fi1.CreateText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
                else
                {
                    using (StreamWriter sw = fi1.AppendText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
            }
        }

        /// <summary>
        ///	写入错误日志
        /// </summary>
        /// <param name="strLogFileName">日志文件名称</param>
        /// <param name="strMessage">错误信息主题</param>
        /// <param name="strSource">错误来源</param>
        public void WriteToLogFile(string strSource)
        {
            lock (typeof(LogManager))
            {
                FileInfo fi1 = new FileInfo(strLogFileName);

                if (!fi1.Exists)
                {
                    using (StreamWriter sw = fi1.CreateText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
                else
                {
                    using (StreamWriter sw = fi1.AppendText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
            }
        }

        /// <summary>
        ///	写入核算系统使用日志
        /// </summary>
        /// <param name="strCafcFileName">日志文件名称</param>
        /// <param name="strSource">来源</param>
        public void WriteToCafcLogFile(string strSource)
        {
            lock (typeof(LogManager))
            {
                FileInfo fi1 = new FileInfo(strCafcFileName);

                if (!fi1.Exists)
                {
                    using (StreamWriter sw = fi1.CreateText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
                else
                {
                    using (StreamWriter sw = fi1.AppendText())
                    {
                        sw.WriteLine(System.DateTime.Now);
                        sw.Write(strSource);
                        sw.WriteLine("\r");
                    }
                }
            }
        }

        /// <summary>
        /// 记录服务操作的记录日志
        /// </summary>
        /// <param name="name">日志文件名称</param>
        /// <param name="msg">要写入的操作信息</param>
        public static void Log(string folder, string name, string msg)
        {
            try
            {
                string strRootPath = AppDomain.CurrentDomain.BaseDirectory; //应用程序目录
                string strLogPath = Path.Combine(strRootPath, folder); //日志目录
                string strLogFileName = Path.Combine(strLogPath, String.Format("{0}_{1:yyyyMMdd}.log", name, DateTime.Now)); //日志文件名称
                lock (typeof(LogManager))
                {
                    if (!Directory.Exists(strLogPath))
                    {
                        Directory.CreateDirectory(strLogPath);
                    }
                    FileInfo file = new FileInfo(strLogFileName);
                    StreamWriter sw = null;
                    try
                    {
                        if (!file.Exists)
                            sw = file.CreateText();
                        else
                            sw = file.AppendText();
                        sw.WriteLine("{0} {1}", DateTime.Now.ToString("HH:mm:ss:fff") + "   ", msg);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (sw != null)
                            sw.Close();
                    }
                    CheckLogLength(strLogPath, file);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 如果单个日志文件超过40M，则开始建新的日志 
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="filename">文件名称</param>
        private static void CheckLogLength(string path, FileInfo file)
        {
            try
            {
                if (file.Length > 41943040)//40M
                {
                    int i = 1;
                    string NewFiName = Path.Combine(path, file.Name + "_1.txt");
                    while (File.Exists(NewFiName))
                    {
                        i++;
                        NewFiName = Path.Combine(path, String.Format("{0}_{1}.txt", file.Name, i));
                    }
                    file.MoveTo(NewFiName);
                }
            }
            catch
            {
            }
        }
    }
}
