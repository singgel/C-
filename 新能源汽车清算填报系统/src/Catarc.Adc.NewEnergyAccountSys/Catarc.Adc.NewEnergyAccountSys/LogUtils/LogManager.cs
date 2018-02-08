using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Catarc.Adc.NewEnergyAccountSys.LogUtils
{
    /// <summary>
    /// 日志管理
    /// </summary>
    public class LogManager
    {
        #region 成员变量
        public static object objLock = new object();
        //private static string isWriteLog;
        // private static string isWriteErrorLog;
        private volatile static LogManager logManage = null;
        #endregion

        #region 构造函数

        public LogManager()
        {
            //objLock = new object();
            //isWriteLog = "1";
            //isWriteErrorLog = "1";
        }
        #endregion

        public static LogManager GetConfig()
        {
            if (logManage == null)
            {
                logManage = new LogManager();
            }
            return logManage;
        }
        #region 记录服务操作的记录日志
        /// <summary>
        /// 记录服务操作的记录日志
        /// </summary>
        /// <param name="msg">要写入的操作信息</param>
        public static void Log(string msg)
        {
            try
            {
                //if (isWriteLog == "1")
                // {
                string strRootPath = AppDomain.CurrentDomain.BaseDirectory; //应用程序目录
                string strLogPath = Path.Combine(strRootPath, "Log"); //日志目录
                string strLogFileName = Path.Combine(strLogPath, "日志_" + DateTime.Now.ToString("yyyyMMdd") + ".log"); //日志文件名称
                lock (objLock)
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
                // }
            }
            catch
            {

            }
        }
        #endregion
        /// <summary>
        /// 记录服务操作的记录日志
        /// </summary>
        /// <param name="name">日志文件名称</param>
        /// <param name="msg">要写入的操作信息</param>
        public static void Log(string folder, string name, string msg)
        {
            try
            {
                // if (isWriteLog == "1")
                // {
                string strRootPath = AppDomain.CurrentDomain.BaseDirectory; //应用程序目录
                string strLogPath = Path.Combine(strRootPath, folder); //日志目录
                string strLogFileName = Path.Combine(strLogPath, String.Format("{0}_{1:yyyyMMdd}.log", name, DateTime.Now)); //日志文件名称
                lock (objLock)
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
                // }
            }
            catch
            {

            }
        }
        #region 记录服务操作的错误日志
        /// <summary>   
        /// 记录服务操作的错误日志
        /// </summary>
        /// <param name="msg">错误信息</param>
        public static void ErrorLog(string msg)
        {
            try
            {
                // if (isWriteErrorLog == "1")
                // {
                string strRootPath = AppDomain.CurrentDomain.BaseDirectory; //应用程序目录
                string strLogPath = Path.Combine(strRootPath, "ErrorLog"); //日志目录
                string strLogFileName = Path.Combine(strLogPath, "错误日志_" + DateTime.Now.ToString("yyyyMMdd") + ".log"); //日志文件名称
                lock (objLock)
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
                    { }
                    finally
                    {
                        if (sw != null)
                            sw.Close();
                    }
                    CheckLogLength(strLogPath, file);
                }
                // }
            }
            catch
            {

            }
        }
        #endregion
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
        /// <summary>
        /// 清除日志 
        /// </summary>
        /// <param name="path">文件路径</param>
        private void ClearLastFile(string Path)
        {
            try
            {
                if (DateTime.Now.Hour.Equals(0))
                {
                    FileInfo[] files = new DirectoryInfo(Path).GetFiles("*.log");
                    foreach (FileInfo info2 in files)
                    {
                        if (info2.LastWriteTime.CompareTo(DateTime.Now.AddDays(-1)) <= 0)
                        {
                            File.Delete(info2.FullName);
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}

