using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace AssistantUpdater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Hashtable _table;
        private bool UpdateSelf;
        private string tmpFile;
        private const string ruleVersion = "填报规则";
        public MainWindow()
        {
            InitializeComponent();
            UpdateSelf = false;
            _table = new Hashtable();
        }

        internal void BeginUpdate()
        {
            try
            {
                int pId = CheckOpenedApplication();
                if (pId != -1)
                {
                    MessageBox.Show("应用程序已打开！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    Application.Current.Shutdown();
                }
                System.Threading.ThreadPool.QueueUserWorkItem(UpdateWorker, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
        }

        private void UpdateRuleWorker(object state)
        {
            string entName = state as string;
            string version = FileHelper.GetFillVersionByName(ruleVersion);
            List<string> allFillType = FileHelper.GetFillTypes();
            foreach (var item in allFillType)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    tips.Text = string.Format("正在更新{0}填报规则...", item);
                }));
                Thread.Sleep(500);
                bool canUseAppendix;
                List<UpdateRuleParameter> list = null;
                if(item == "转换规则" || item == "应用程序配置")
                    list = FileHelper.GetRuleFileList(entName, item, "", out canUseAppendix);
                else
                    list = FileHelper.GetRuleFileList(ruleVersion, item, version, out canUseAppendix);
                foreach (var parameter in list)
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        tips.Text = string.Format("正在更新{0}填报规则，正在比较文件“{1}”...", item, parameter.FileName);
                    }));
                    string md5 = ServiceHelper.GetFillRuleMd5(item, ruleVersion, parameter.Standard, parameter.CarType, parameter.FileName);
                    if (string.IsNullOrEmpty(md5))
                        continue;
                    string currentMd5 = FileHelper.GetFileMd5(parameter.FileName);
                    if (md5 != currentMd5)
                    {
                        Dispatcher.Invoke((Action)(() =>
                        {
                            tips.Text = string.Format("正在更新{0}填报规则，正在下载文件“{1}”...", item, parameter.FileName);
                        }));
                        ServiceHelper.DownloadFillRule(item, ruleVersion, parameter.Standard, parameter.CarType, parameter.FileName);
                    }
                }
            }
            Dispatcher.Invoke((Action)(() =>
            {
                tips.Text = "填报规则已更新...";
                if(UpdateSelf)
                    Process.Start("Assistant.exe", tmpFile);
                else
                    Process.Start("Assistant.exe");
                System.Threading.Thread.Sleep(500);
                Application.Current.Shutdown();
            }));
        }

        private void UpdateWorker(object state)
        {
            try
            {
                string entName = FileHelper.GetEntName();
                Version version = new Version(ServiceHelper.GetAppVersion(entName));
                Version currentVersion = FileHelper.GetCurrentVersion();
                if (currentVersion >= version)
                {
                    UpdateRuleWorker(entName);
                    return;
                }
                else
                {
                    Dispatcher.Invoke((Action)(() =>
                    {
                        tips.Text = "检测到新的应用程序版本，准备更新...";
                    }));
                    System.Threading.Thread.Sleep(500);
                }
                Dispatcher.Invoke((Action)(() =>
                {
                    tips.Text = "正在获取应用程序文件列表...";
                }));
                List<AppFileInfo> fileList = ServiceHelper.GetAllFiles(entName, version.ToString());
                foreach (var item in fileList)
                {
                    if (item.IsDeleted && item.fileName != "AssistantUpdater.exe")
                    {
                        if (File.Exists(item.fileName))
                            File.Delete(item.fileName);
                    }
                    else
                    {
                        string md5 = FileHelper.GetFileMd5(item.fileName);
                        if (md5 != item.md5Str)
                        {
                            if (item.fileName == "AssistantUpdater.exe")
                            {
                                UpdateSelf = true;
                                tmpFile = string.Format("{0}\\{1}", Path.GetTempPath(), item.fileName);
                                item.fileName = tmpFile;
                            }
                            Dispatcher.Invoke((Action)(() =>
                            {
                                tips.Text = string.Format("正在下载{0}...", item.fileName);
                            }));
                            ServiceHelper.DownloadAppFile(entName, version.ToString(), item.fileName);
                        }
                    }
                }
                UpdateRuleWorker(entName);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    MessageBox.Show(this, ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }));
            }
            finally
            {
                
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            this.BeginUpdate();
            base.OnSourceInitialized(e);
        }

        private int CheckOpenedApplication()
        {
            string currentUpdateFile = System.IO.Path.GetFullPath("AssistantUpdater.exe");
            string currentAppFile = System.IO.Path.GetFullPath("Assistant.exe");
            Process process = Process.GetCurrentProcess();
            foreach (var item in Process.GetProcesses())
            {
                string fileName = "";
                try
                {
                    fileName = item.MainModule.FileName;
                }
                catch
                {
                    continue;
                }
                if (fileName == currentAppFile)
                    return item.Id;
                else if (fileName == currentUpdateFile && item.Id != process.Id)
                    return item.Id;
            }
            return -1;
        }
    }
}
