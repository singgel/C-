using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using System.Data;
using Catarc.Adc.NewEnergyApproveSys.LogUtils;

namespace Catarc.Adc.NewEnergyApproveSys.Common
{
    public class ApplicationFolder
    {

        public static string billImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMAGE\\IMAGEBill");//发票图片路径
        public static string driveImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IMAGE\\IMAGEDrive");//行驶证图片路径

        /// <summary>  
        /// 从FTP服务器下载文件，指定本地路径和本地文件名  
        /// </summary>  
        /// <param name="remoteFileName">远程文件名</param>  
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>  
        /// <param name="ifCredential">是否启用身份验证（false：表示允许用户匿名下载）</param>  
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>  
        /// <returns>是否下载成功</returns>  
        public static bool FtpDownload(string remoteFileName, string localFileName, bool ifCredential)
        {
            // Ftp服务器ip   
            string FtpServerIP = Settings.Default.FtpIPAddr;
            // Ftp 指定用户名  
            string FtpUserID = Settings.Default.FtpUserID;
            // Ftp 指定用户密码  
            string FtpPassword = Settings.Default.FtpPassword;

            FtpWebRequest reqFTP;
            Stream ftpStream = null;
            FtpWebResponse response = null;
            FileStream outputStream = null;
            try
            {
                string localFilePath = Path.GetDirectoryName(localFileName);
                //localFileName文件路径是否存在，不存在则创建
                if (!Directory.Exists(localFilePath))
                {
                    Directory.CreateDirectory(localFilePath);
                }
                //判断文件的存在
                if (File.Exists(localFileName))
                {
                    return true;
                }
                if (FtpServerIP == null || FtpServerIP.Trim().Length == 0)
                {
                    throw new Exception("ftp下载目标服务器地址未设置！");
                }
                Uri uri = new Uri(String.Format("ftp://{0}/{1}", FtpServerIP, remoteFileName));
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                //使用用户身份认证
                if (ifCredential)  
                {
                    reqFTP.Credentials = new NetworkCredential(FtpUserID, FtpPassword);
                }
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();
                outputStream = new FileStream(localFileName, FileMode.Create);
                long totalDownloadedByte = 0;
                const int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    totalDownloadedByte = readCount + totalDownloadedByte;
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.Log("Error", "FtpRemote", ex.Message);
                return false;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}
