using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

namespace Catarc.Adc.NewEnergyApproveSys.PopForm
{
    public partial class GlobalExceptionForm : DevExpress.XtraEditors.XtraForm
    {
        #region Private Members
        // 创建异常对象用于接收传入的异常
        Exception ex = new Exception();

        // 声明 Win32API 的删除文件函数
        //[DllImport("Kernel32.dll", EntryPoint = "DeleteFileW", SetLastError = true,
        //    CharSet = CharSet.Unicode, ExactSpelling = true,
        //    CallingConvention = CallingConvention.StdCall)]
        //public static extern bool DeleteFile(String lpFileName); 
        #endregion

        #region Public Methods
        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="globalException">捕获到的全局异常</param>
        /// <param name="applicationName">当前应用程序名称</param>
        /// <param name="developerName">程序开发者名称</param>
        public GlobalExceptionForm(Exception globalException, string applicationName, string developerName)
        {
            // 设计器构造控件所须方法
            InitializeComponent();

            // 截取前13个字符作为应用程序标题显示
            applicationName =
                applicationName.Length > 13
                ? string.Format("{0}...", applicationName.Substring(0, 13))
                : applicationName;

            // 显示用户提示消息
            lblInfo.Text = string.Format(
                "{0} 遇到问题需要关闭。我们对此引起的不便表示抱歉。请将此问题报告给 {1}。",
                applicationName,
                developerName);

            // 接收异常
            ex = globalException;

            // ------ 异常信息 ------ //
            // 消息
            txtMessage.Text += ex.Message;
            // 帮助链接
            linkLblHelpLink.Text += ex.HelpLink != null ? " " + ex.HelpLink : " " + "None";
            // 对象
            txtSource.Text += ex.Source;
            // 堆栈
            txtStackTrace.Text += ex.StackTrace;
            // 方法
            txtTargeSite.Text += ex.TargetSite.ToString();

            // ------ 环境信息 ------ //
            // 当前路径
            linkLblCurrentDirectory.Text += " " + Environment.CurrentDirectory;
            // 机器名
            lblMachineName.Text += " " + string.Format("{0}[{1}]", Environment.MachineName, Environment.UserDomainName);
            // 操作系统
            lblOSVersion.Text += " " + Environment.OSVersion;
            // 系统路径
            linkLblSystemDirectory.Text += " " + Environment.SystemDirectory;
            // 用户名
            lblUserName.Text += " " + Environment.UserName;
            // .NET版本
            lblVersion.Text += " " + Environment.Version;

            // 将错误信息写入今天的错误日志文件

            // 日志文件名称
            string today = string.Format(
                "{0}-{1}-{2}",
                DateTime.Today.Year.ToString(),
                DateTime.Today.Month.ToString(),
                DateTime.Today.Day.ToString());
            string fileName = string.Format(@".\ErrLog\{0}.log", today);

            // 提取错误信息
            string msg = errorMessage();

            // 如果日志文件存放目录不存在则重建它
            if (!Directory.Exists(@".\ErrLog"))
            {
                // 其它方法
                // DirectoryInfo myDi = new DirectoryInfo(@".\ErrLog");
                // myDi.Create();
                Directory.CreateDirectory(@".\ErrLog");
            }

            // 去除日志文件被人为修改的只读、隐藏、系统属性
            if (File.Exists(fileName))
            {
                FileInfo myFi = new FileInfo(fileName);
                // 设置为正常属性
                myFi.Attributes = FileAttributes.Normal;
            }

            // 声明文件流
            FileStream myFs = null;
            // 声明写入器
            StreamWriter mySw = null;

            try
            {
                // 创建文件流
                myFs = new FileStream(
                    fileName,
                    File.Exists(fileName) ? FileMode.Append : FileMode.CreateNew);
                // 创建写入器
                mySw = new StreamWriter(myFs);
                // 将错误信息写入文件
                mySw.Write(msg);
            }
            catch (Exception e)
            {
                XtraMessageBox.Show(string.Format(
                    "自动错误报告文件创建失败！\n\n原因：{0}",
                    e.Message),
                    "程序错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // 关闭写入器
                mySw.Close();
                // 关闭文件流
                myFs.Close();
            }
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// 单击帮助链接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLblHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 置为已访问呈现样式
            linkLblHelpLink.LinkVisited = true;
        }

        /// <summary>
        /// 单击当前路径链接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLblCurrentDirectory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 打开链接目录
            OpenDirectory(linkLblCurrentDirectory.Text);

            // 置为已访问呈现样式
            linkLblCurrentDirectory.LinkVisited = true;
        }

        /// <summary>
        /// 单击系统路径链接时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkLblSystemDirectory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 打开链接目录
            OpenDirectory(linkLblSystemDirectory.Text);

            // 置为已访问呈现样式
            linkLblSystemDirectory.LinkVisited = true;
        }

        /// <summary>
        /// 使用 Windows 资源管理器 打开指定目录
        /// </summary>
        /// <param name="path">目录路径</param>
        private void OpenDirectory(string path)
        {
            // 使用 Windows 资源管理器 打开路径
            try
            {
                Process.Start("Explorer.exe", string.Format("/e, {0}", path));
            }
            catch
            {
                XtraMessageBox.Show("抱歉！未能在系统中找到指定的文件 'Explorer.exe'。", "Explorer.exe", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string errorMessage()
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("// ■■■■■■■■■■ [{0}] ■■■■■■■■■■ //", DateTime.Now.ToString()));
            content.AppendLine(Environment.NewLine);
            content.AppendLine("// ---------- [错误信息] ---------- //");
            content.AppendLine();
            content.AppendLine(lblMessage.Text.Trim() + " " + txtMessage.Text.Trim());
            content.AppendLine(lblHelpLink.Text.Trim() + " " + linkLblHelpLink.Text.Trim());
            content.AppendLine(lblSource.Text.Trim() + " " + txtSource.Text.Trim());
            content.AppendLine(lblStackTrace.Text.Trim() + Environment.NewLine + "   " + txtStackTrace.Text.Trim());
            content.AppendLine(lblTargeSite.Text.Trim() + " " + txtTargeSite.Text.Trim());
            content.AppendLine(Environment.NewLine);
            content.AppendLine("// ---------- [系统环境] ---------- //");
            content.AppendLine();
            content.AppendLine(lblCurrentDirectory.Text.Trim() + " " + linkLblCurrentDirectory.Text.Trim());
            content.AppendLine(lblMachineName.Text);
            content.AppendLine(lblOSVersion.Text);
            content.AppendLine(lblSystemDirectory.Text.Trim() + " " + linkLblSystemDirectory.Text.Trim());
            content.AppendLine(lblUserName.Text);
            content.AppendLine(lblVersion.Text);
            content.AppendLine(Environment.NewLine + Environment.NewLine);
            return content.ToString();
        }
        #endregion

        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFeedback_Click(object sender, EventArgs e)
        {
            string from = "adc_catarc@163.com";
            string to = "adc_catarc@163.com";
            string subject = "新能源汽车清算填报系统Bug提交";
            string body = errorMessage();
            string server = "smtp.163.com";
            MailMessage message = new MailMessage(from, to, subject, body);
            SmtpClient client = new SmtpClient(server);
            client.Credentials = new NetworkCredential("adc_catarc", "catarc_adc");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
                return;
            }
            XtraMessageBox.Show("邮件发送成功!");
            this.Close();
        }

        /// <summary>
        /// 中止
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAbort_Click(object sender, EventArgs e)
        {
            // 关闭异常提示
            this.Close();
            // 中止当前线程
            Thread.CurrentThread.Abort("Abort");
        }

        /// <summary>
        /// 忽略
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIgnore_Click(object sender, EventArgs e)
        {
            // 关闭异常提示
            this.Close();
        }

    }
}