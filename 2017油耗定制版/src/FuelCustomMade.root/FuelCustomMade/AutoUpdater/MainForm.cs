using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace AutoUpdater
{
    public partial class MainForm : DevExpress.XtraEditors.XtraForm
    {
        string[] args = null;
        string root = "";
        string mainprocess = "";
        int number = 0;
        int filenumber = 0;
        List<string> lsFolder = new List<string>();
        int sleeptotal = 100;

        public MainForm(string[] args)
        {
            InitializeComponent();
            this.args = args;
            root = args[0];
            mainprocess =args[1];;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //创建线程
            Thread threadDown = new Thread(CopyFileList) { IsBackground = true };
            threadDown.Start();
        } 

        private void CopyFileList()
        {

            Invoke((MethodInvoker)delegate
            {
                try
                {
                    KillProcess();
                    getFileNumber();//获取文件个数
                    //更新进度条
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = filenumber + sleeptotal;
                    int sleepNumber = 0;
                    do
                    {
                        if (sleepNumber > sleeptotal)
                        {
                            break;
                        }
                        sleepNumber++;
                        progressBar1.Value = sleepNumber;
                        Thread.Sleep(100);
                    } while (true);
                    //拷贝文件
                    CopyFile(root, Directory.GetCurrentDirectory());
                    // 删除文件
                    System.IO.Directory.Delete(root, true);
                    //开始主程序
                    StartMainProcess();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show( ex.Message);
                    
                }
                finally
                {
                    this.Close();
                }
                
            });
         }
        public void KillProcess()
         {
            try
            {
                Process[] allProcess = Process.GetProcesses();
                foreach (Process p in allProcess)
                {
                    if (p.ProcessName.ToLower() + ".exe" == mainprocess.ToLower())
                    {
                        for (int i = 0; i < p.Threads.Count; i++)
                            p.Threads[i].Dispose();
                        p.Kill();
                    }
                }
               
                
            }
            catch (System.Exception ex)
            {
            	
            }
             
         }
        public void StartMainProcess()
        {
            //启动主程序
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = mainprocess /*启动的应用程序名称  */ };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("无法启动主程序，失败原因:" + ex.Message);
            } 
        }
        public void CopyFile(string sourcePath, string objPath)
        {
            try
            {
                if (!Directory.Exists(objPath))
                {
                    Directory.CreateDirectory(objPath);
                }
                string[] files = Directory.GetFiles(sourcePath);
                for (int i = 0; i < files.Length; i++)
                {
                    string[] childfile = files[i].Split('\\');
                    File.Copy(files[i], String.Format(@"{0}\{1}", objPath, childfile[childfile.Length - 1]), true);
                    number++;
                    progressBar1.Value = sleeptotal+number;
                }
                string[] dirs = Directory.GetDirectories(sourcePath);
                for (int i = 0; i < dirs.Length; i++)
                {
                    string[] childdir = dirs[i].Split('\\');
                    CopyFile(dirs[i], String.Format(@"{0}\{1}", objPath, childdir[childdir.Length - 1]));
                }
            }
            catch (System.Exception ex)
            {
                throw new Exception("无法拷贝程序，失败原因:" + ex.Message);
            }
           
        }

       //查询文件个数
        public void getFileNumber()
        {
            try
            {
                traverse("");
                do
                {
                    if (lsFolder.Count == 0)
                    {
                        break;
                    }
                    traverse(lsFolder[0]);
                    lsFolder.RemoveAt(0);
                } while (true);
            }
            catch (System.Exception ex)
            {
                throw new Exception("无法拷贝程序，失败原因:" + ex.Message);
            }
            
        }
        public void traverse(string folderFullName)
        {
            DirectoryInfo TheFolder = new DirectoryInfo(String.Format("{0}\\{1}", root, folderFullName));
            //查询文件个数
            filenumber += TheFolder.GetFiles().Length;
            //遍历文件夹
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
            {
                lsFolder.Add(folderFullName +  NextFolder.Name);
            }
        }
    }
}