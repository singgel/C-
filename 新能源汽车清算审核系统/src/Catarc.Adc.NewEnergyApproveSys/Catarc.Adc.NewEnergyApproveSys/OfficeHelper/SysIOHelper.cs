using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Catarc.Adc.NewEnergyApproveSys.OfficeHelper
{
    public class SysIOHelper
    {
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="fromDir">源文件夹</param>
        /// <param name="toDir">目标文件夹</param>
        public static void CopyDir(string fromDir, string toDir)
        {
            if (!Directory.Exists(fromDir))
                return;

            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }

            string[] files = Directory.GetFiles(fromDir);
            Array.ForEach(files, formFileName =>
            {
                string fileName = Path.GetFileName(formFileName);
                string toFileName = Path.Combine(toDir, fileName);
                File.Copy(formFileName, toFileName, true);
            });
            string[] fromDirs = Directory.GetDirectories(fromDir);
            Array.ForEach(fromDirs, fromDirName =>
            {
                string dirName = Path.GetFileName(fromDirName);
                string toDirName = Path.Combine(toDir, dirName);
                CopyDir(fromDirName, toDirName);
            });
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="fromFile">源文件全路径</param>
        /// <param name="toDir">目的文件夹</param>
        /// <param name="toFile">目的文件名</param>
        public static void CopyFile(string fromFile, string toDir, string toFile)
        {
            if (!File.Exists(fromFile))
                return;
            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
               
            }
            File.Copy(fromFile, Path.Combine(toDir, toFile), true);
        }

        /// <summary>
        /// 获取文件信息
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="path">路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="suffix">后缀名</param>
        public static void GetFileNameInfo(string filepath,ref string path,ref string fileName,ref string suffix)
        {
            path = filepath.Substring(0,filepath.LastIndexOf("\\") ); //文件名
            fileName = filepath.Substring(filepath.LastIndexOf("\\") + 1, (filepath.LastIndexOf(".") - filepath.LastIndexOf("\\") - 1)); //文件名
            suffix = filepath.Substring(filepath.LastIndexOf(".") + 1, (filepath.Length - filepath.LastIndexOf(".") - 1)); //扩展名
        }
    }
}
