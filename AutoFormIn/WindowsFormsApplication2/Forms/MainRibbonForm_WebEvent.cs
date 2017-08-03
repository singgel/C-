using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Assistant.Forms;
using Assistant.Forms.Util;
using Microsoft.Win32;
using mshtml;
using WebBrowserUtils;
using WebBrowserUtils.ExtendWebBrowser;
using WebBrowserUtils.HtmlUtils.Fillers;
using WebBrowserUtils.WinApi;
using Assistant.DataProviders;

namespace Assistant
{
    public partial class MainRibbonForm : IOpenDialog
    {
        private bool _isDialogOpen;
        private IntPtr textHandle, btnHandle;
        private FillManagerBase _fillManager;
        private const int edit_ID = 0x47C, button_OK_ID = 0x1;
        private const int NOTIFY_WM_CREATE = WMMSG.WM_USER + 0x8000;
        private string importDataFilter;
        public event EventHandler DialogClosed;
        public event DialogOpenedEventHandler DialogOpened;

        public FillManagerBase FillManager
        {
            get { return _fillManager; }
        }

        private FillManagerBase GetFillManager(string dataFile, WebBrowser2 browser)
        {
            FillManagerBase manager = null;
            if (ribbon.SelectedPage == ghsite)
                manager = new GHFillManager(browser, dataFile);
            else if (ribbon.SelectedPage == fdlsite)
                manager = new FDLFillManager(browser, dataFile);
            else if (ribbon.SelectedPage == bhsite)
                manager = new BHFillManager(browser, dataFile);
            else if (ribbon.SelectedPage == xnysite)
                manager = new XNYFillManager(browser, dataFile);
            else if (ribbon.SelectedPage == cocsite)
                manager = new COCFillManager(browser, dataFile);
            else if (ribbon.SelectedPage == pzhsite)
                manager = new PZHFillManager(browser, dataFile);
            else if (ribbon.SelectedPage == cccsite)
            {
                if (cccProcess == null)
                {
                    this.Start3CProcess();
                    if (cccProcess == null)
                        return null;
                    while (cccProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        Application.DoEvents();
                    }
                }
                manager = new CCCFillManager((uint)cccProcess.Id, dataFile);
            }
            // 为填报器指定数据提供程序。
            if (manager != null)
            {
                string entripise = FileHelper.GetEntName();
                manager.DataProvider = DataProviders.DataProviderFactory.CreateProvider(entripise, GetSelectedPageType());

                manager.DataProvider.DataSourceFile = dataFile;
                if (manager.DataProvider.AllowAlternately)
                {
                    bool result = manager.DataProvider.ShowWindow();
                    return result ? manager : null;
                }
            }
            return manager;
        }

        private void InitWebEvent()
        {
            //this.mainWebBrowser.DocumentCompleted += mainWebBrowser_DocumentCompleted;
            //this.mainWebBrowser.NewWindow3 += mainWebBrowser_NewWindow3;
        }

        private void Start3CProcess()
        {
            cccProcess = Get3CProcess();
            if (cccProcess == null)
            {
                try
                {
                    RegistryKey sub = null;
                    sub = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\{0}ManuFaxturer",
                        Environment.Is64BitOperatingSystem ? @"Wow6432Node\" : ""));
                    object path = sub == null ? null : sub.GetValue("path", null);
                    if (path == null)
                        path = string.Format(@"{0}\参数填报工具", Environment.GetEnvironmentVariable("programfiles"));
                    if(File.Exists("licenses.xml"))
                        File.Copy("licenses.xml", string.Format("{0}\\licenses.xml", path), true);
                    cccProcess = Process.Start(string.Format(@"{0}\ManuFaxturer.exe", path));
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
                    dialog.Filter = "(应用程序文件)|*.exe";
                    dialog.CheckFileExists = true;
                    dialog.Title = "打开“参数填报工具”应用程序";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        FileInfo file = new FileInfo(dialog.FileName);
                        if (File.Exists("licenses.xml"))
                            File.Copy("licenses.xml", string.Format("{0}\\licenses.xml", file.DirectoryName), true);
                        cccProcess = Process.Start(dialog.FileName);
                    }
                    else
                        return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK);
                    return;
                }
            }
            this.resumeLocation = this.Location;
            this.resumeSize = this.Size;
            mainWebBrowser.Navigate("about:blank");
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
                cccProcess.WaitForExit();
                if (_fillManager != null && _fillManager is CCCFillManager)
                {
                    _fillManager.EndFill();
                    _fillManager = null;
                }
                cccProcess = null;
                this.Invoke((Action)(() =>
                {
                    this.TopMost = false;
                    this.Size = resumeSize;
                    this.Location = resumeLocation;
                }));
            }, null);
        }

        private Process Get3CProcess()
        {
            Process[] p = Process.GetProcessesByName("ManuFaxturer");
            if (p == null || p.Length == 0)
                return null;
            return p[0];
        }

        protected virtual void OnDialogClosed(EventArgs e)
        {
            if (DialogClosed != null)
                DialogClosed(this, e);
        }

        protected virtual void OnDialogOpened(DialogOpenedEventArgs e)
        {
            if (DialogOpened != null)
                DialogOpened(this, e);
        }

        private void SetText(IntPtr handle)
        {
            WebFillManager fillManager = _fillManager as WebFillManager;
            if (fillManager != null)
            {
                GetTextHandle(handle);
                if (textHandle != IntPtr.Zero)
                    WebBrowserUtils.HtmlUtils.Fillers.ApiSetter.SetText(textHandle, fillManager.CurrentValue);
                if (btnHandle != IntPtr.Zero)
                    WebBrowserUtils.HtmlUtils.Fillers.ApiSetter.ClickButton(btnHandle, this.Handle, null, null);
            }
        }

        private void GetTextHandle(IntPtr handle)
        {
            NativeApi.EnumChildWindows(handle, EnumChildWindowProc, IntPtr.Zero);
        }

        private bool EnumChildWindowProc(IntPtr hwnd, IntPtr lParam)
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.GetClassName(hwnd, className, 255);
            if (className.ToString() == "Edit" && NativeApi.GetDlgCtrlID(hwnd) == edit_ID)
                textHandle = hwnd;
            else if (className.ToString() == "Button" && NativeApi.GetDlgCtrlID(hwnd) == button_OK_ID)
                btnHandle = hwnd;
            return (textHandle == IntPtr.Zero || btnHandle == IntPtr.Zero);
        }

        protected override void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
            case NOTIFY_WM_CREATE:
                break;
            case WMMSG.WM_ENTERIDLE:
                if (_isDialogOpen == false)
                {
                    int wparam = msg.WParam.ToInt32();
                    IntPtr handle = msg.LParam;
                    if (wparam == 0) // 判断是否为模态窗口
                    {
                        _isDialogOpen = true;
                        OnDialogOpened(new DialogOpenedEventArgs(handle));
                        StringBuilder className = new StringBuilder(256);
                        NativeApi.GetClassName(handle, className, 255);
                        if (className.ToString() == "#32770" && _fillManager != null) // 判断是否为打开文件对话框
                            SetText(handle);
                    }
                }
                _isDialogOpen = true;
                break;
            case WMMSG.WM_GETICON:
            case WMMSG.WM_SETCURSOR:
            case WMMSG.WM_ACTIVATEAPP:
            case WMMSG.WM_WINDOWPOSCHANGING:
            case WMMSG.WM_WINDOWPOSCHANGED:
                break;
            default:
                if (_isDialogOpen)
                {
                    _isDialogOpen = false;
                    textHandle = IntPtr.Zero;
                    btnHandle = IntPtr.Zero;
                    OnDialogClosed(EventArgs.Empty);
                }
                break;
            }
            base.WndProc(ref msg);
        }
    }
}
