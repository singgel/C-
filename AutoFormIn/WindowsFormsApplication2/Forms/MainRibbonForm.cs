using Assistant.Actions;
using Assistant.DataProviders;
using Assistant.Entity;
using Assistant.Forms;
using Assistant.Manager;
using Assistant.Properties;
using Assistant.Service;
using AssistantUpdater;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using WebBrowserUtils.HtmlUtils.Comparer;
using WebBrowserUtils.HtmlUtils.Detectors;
using WebBrowserUtils.HtmlUtils.Fillers;
using WebBrowserUtils.HtmlUtils.History;

namespace Assistant
{
    public partial class MainRibbonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Process cccProcess;
        private System.Drawing.Size resumeSize;
        private System.Drawing.Point resumeLocation;
        public WebBrowserUtils.ExtendWebBrowser.WebBrowser2 mainWebBrowser;
        private RuleCompareNode selectedNode;
        private RuleComparer comparer;

        public MainRibbonForm()
        {
            InitializeComponent();
            this.mainWebBrowser = this.tabPage.WebBrowser1;
            InitWebEvent();
            ribbon.SelectedPageChanging += ribbon_SelectedPageChanging;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            //this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            this.dataFileTextBox.ItemClick += new ItemClickEventHandler(dataFileTextBox_ItemClick);
            //this.mainWebBrowser.Url = new Uri(DeclarationSite.PZSite);
            this.rememberUsernameCheckEdit.EditValueChanged += new EventHandler(rememberUsernameCheckEdit_EditValueChanged);
            this.usernameTextbox.EditValueChanged += new EventHandler(usernameTextbox_EditValueChanged);
            this.passwordTextbox.EditValueChanged += new EventHandler(passwordTextbox_EditValueChanged);
            this.patamAddress.EditValueChanged += new EventHandler(patamAddress_EditValueChanged);
            this.Shown += new EventHandler(MainRibbonForm_Shown);
            this.FormClosed += new FormClosedEventHandler(MainRibbonForm_FormClosed);
            this.UserTip.Caption = string.Format("你好，用户{0}!", Constants.CurrentUser == null ? "" : Constants.CurrentUser.userName);
            this.RunningCarInfo.Caption = "";
            this.tabPageControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(tabPageControl_SelectedPageChanged);
            this.tabPageControl.CloseButtonClick += new EventHandler(tabPageControl_CloseButtonClick);
            this.InitPages();
        }

        void tabPageControl_CloseButtonClick(object sender, EventArgs e)
        {
            DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs EArg = (DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs)e;
            IXtraTabPage closingPage = EArg.Page;
            foreach (XtraTabPage page in this.tabPageControl.TabPages)//遍历得到和关闭的选项卡一样的Text
            {
                if (closingPage == page)
                {
                    this.tabPageControl.TabPages.Remove(page);
                    page.Dispose();
                    int count = this.tabPageControl.TabPages.Count;
                    if (count > 0)
                        this.tabPageControl.SelectedTabPageIndex = count - 1;
                    return;
                }
            }
        }

        void tabPageControl_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            if (this.tabPageControl.TabPages.Count == 1) {
                //this.tabPageControl.ClosePageButtonShowMode = ClosePageButtonShowMode.Default;
            }
            if (e.Page != null) {
                //this.mainWebBrowser = ((TabPageWithWebBrowser)(e.Page)).WebBrowser1;
            }
            
        }

        void MainRibbonForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);  //关闭窗体后强制退出程序
        }

        void patamAddress_EditValueChanged(object sender, EventArgs e)
        {
            String patamAddress = this.patamAddress.EditValue.ToString();
            if (String.IsNullOrEmpty(patamAddress))
            {
                throw new Exception("型式认证地址为空");
            }
            else
            {
                Settings.Default.Assistant_ConfigCodeService_IPackageInfoServiceService = patamAddress;
                Settings.Default.Save();
            }
        }

        void MainRibbonForm_Shown(object sender, EventArgs e)
        {
            this.usernameTextbox.EditValue = Settings.Default.USERNAME;
            this.passwordTextbox.EditValue = Settings.Default.PASSWORD;
            this.patamAddress.EditValue = Settings.Default.Assistant_ConfigCodeService_IPackageInfoServiceService;
            this.RunningCarInfo.Caption = "";
        }

        void usernameTextbox_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                String username = this.usernameTextbox.EditValue.ToString();
                if (String.IsNullOrEmpty(username))
                {
                    throw new Exception("用户名为空");
                }
                else
                {
                    LoginAction.saveUsernameAndPassword(username, Properties.Settings.Default.PASSWORD);
                    //this.mainWebBrowser.Url = new Uri(DeclarationSite.PZSite);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        void passwordTextbox_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                String password = this.passwordTextbox.EditValue.ToString();
                if (String.IsNullOrEmpty(password))
                {
                    throw new Exception("密码为空");
                }
                else
                {
                    LoginAction.saveUsernameAndPassword(Properties.Settings.Default.USERNAME, password);
                    //this.mainWebBrowser.Url = new Uri(DeclarationSite.PZSite);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void rememberUsernameCheckEdit_EditValueChanged(object sender, EventArgs e)
        {
            MessageBox.Show(this.rememberUsernameCheckEdit.EditValue.ToString());
            //MessageBox.Show("test");
            String value = this.rememberUsernameCheckEdit.EditValue.ToString();
            String username = this.usernameTextbox.EditValue.ToString();
            String password = this.passwordTextbox.EditValue.ToString();
            if (!String.IsNullOrEmpty(value) && value.Equals(true.ToString(), StringComparison.OrdinalIgnoreCase)) {
                LoginAction.saveUsernameAndPassword(username, password);
            }
        }

        //private void Form1_FormClosing(object sender, EventArgs e)
        //{
        //    Application.Exit();
        //}

        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);
        //    if (cccFiller != null && ribbon.SelectedPage.Text == "CCC")
        //        cccFiller.SetWindowPos();
        //}

        //protected override void OnLocationChanged(EventArgs e)
        //{
        //    base.OnLocationChanged(e);
        //    if (cccFiller != null && ribbon.SelectedPage.Text == "CCC")
        //        cccFiller.SetWindowPos();
        //}

        //protected override void OnSizeChanged(EventArgs e)
        //{
        //    base.OnSizeChanged(e);
        //    if (cccFiller != null && ribbon.SelectedPage.Text == "CCC")
        //        cccFiller.SetWindowPos();
        //}

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //判断dataFile地址是不是为空
                if (this.dataFileTextBox.EditValue == null)
                {
                    throw new Exception("数据文件未导入，请选择数据文件");
                }
                String dataFilePath = this.dataFileTextBox.EditValue.ToString();
                String ruleFilePath = ParamsCollection.localAddr + "rules.xls";
                ImportAction importAction = new ImportAction();
                List<String> listPackageCode = importAction.importDataAndRules(ruleFilePath, dataFilePath);

                ChooseSample cs = new ChooseSample();
                if (cs.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    importAction.deleteUnselectedCarParams(ParamsCollection.selectedPackageCodes);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < ParamsCollection.carParams.Count; i++)
                    {
                        CarParams cps = ParamsCollection.carParams[i];
                        ParamsCollection.runningCarParams = cps;
                        this.RunningCarInfo.Caption = "当前正在处理的车型为" + cps.packageCode + "  (" + (i+1) + "/" + ParamsCollection.carParams.Count + ")";
                        cps.listHtmlAttributes.Clear();
                        cps.listHtmlAttributes.AddRange(DataTransformServiceForHCBM.getAllHAInACar(cps.listParams, ParamsCollection.dicRules));
                        LoginAction.tryLogin(this.mainWebBrowser);
                        this.wait(1);
                        this.mainWebBrowser.Navigate(Constants.APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE);
                        this.wait(1);
                        FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
                        f.fillConfigTest(cps.listHtmlAttributes);
                        this.wait(1);
                        //f.submit();
                        this.wait(1);
                        //while (String.IsNullOrEmpty(ParamsCollection.runningCarParams.configCode))
                        //{
                        //    Application.DoEvents();
                        //}
                        this.wait(1);
                        f = new FillConfigurationAction(this.mainWebBrowser);
                        ExportServiceForHCBM.exportCarParams(dataFilePath, cps);
                        //在网页上直接提交车型
                        //f.submitConfigCode(ParamsCollection.runningCarParams.configCode);
                        //while (ParamsCollection.runningCarParams.submitConfigCode != true)
                        //{
                        //    Application.DoEvents();
                        //}
                    }
                    //LoggerUtils.LoggerUtils.loggerTxt(@"d:\test.txt", sb.ToString());
                    this.mainWebBrowser.Url = new Uri(DeclarationSite.PZSite);
                    this.RunningCarInfo.Caption = "全部任务完成";
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message);
            }
        }

        void dataFileTextBox_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DialogResult dr = MessageBox.Show("确定要删除当前用户吗？", "删除用户", MessageBoxButtons.OKCancel);
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    ServiceHelper.DeleteUser(Constants.CurrentUser.userName);
                    //删除用户
                    bool result = ClassFactory.fuss.deleteUserInfo(Constants.username);
                    if (result)
                    {
                        MessageBox.Show("用户注销成功，即将关闭程序...");
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Environment.Exit(0);
        }
        /// <summary>
        /// 获取当前选定的标签页的填报类型
        /// </summary>
        /// <returns></returns>
        private string GetSelectedPageType()
        {
            if (ribbon.SelectedPage == ghsite)
                return "国环";
            else if (ribbon.SelectedPage == bhsite)
                return "北环";
            else if (ribbon.SelectedPage == xnysite)
                return "新能源";
            else if (ribbon.SelectedPage == cccsite)
                return "CCC";
            else if (ribbon.SelectedPage == cocsite)
                return "COC";
            else if (ribbon.SelectedPage == pzhsite)
                return "配置序列号";
            else if (ribbon.SelectedPage == fdlsite)
                return "非道路机动车";
            else
                return "";
        }

        /// <summary>
        /// 使用定时器等待一定时间,之后还要等待网页处理完成
        /// </summary>
        /// <param name="interval"></param>

        public void wait(int interval)
        {

            this.timer1.Interval = interval * 1000;
            this.timer1.Tick += new EventHandler(timer1_Tick);
            this.timer1.Start();
            while (this.timer1.Enabled)
            {
                Application.DoEvents();
            }

            while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
        //定时器2时间到之后执行函数
        private void timer1_Tick(object sender, EventArgs e)
        {

            this.timer1.Tick -= this.timer1_Tick;
            this.timer1.Stop();
        }

        private void usernameTextbox_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void rememberUsernameCheckEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            //MessageBox.Show("test");
        }

        private void barEditItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void dataFileTextBox_ItemClick_1(object sender, ItemClickEventArgs e)
        {

        }

        private void openFileButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "选择市场部数据Excel文件";
            open.Filter = "Excel文件|*.xlsx;*.xls|Access 数据库文件|*.mdb;*.accdb|所有文件|*.*";
            if (open.ShowDialog() == DialogResult.OK)
            {
                //((TextBox)sender).Text = open.FileName;
                this.dataFileTextBox.EditValue = open.FileName;
                this.importDataFilter = open.FileName;
            }
        }

        private void MainRibbonForm_Load(object sender, EventArgs e)
        {

        }

        private void UserTip_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void SettingButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            settingForm.ShowDialog();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
            f.submit();
        }

        private void ElementDetector_ItemClick(object sender, ItemClickEventArgs e)
        {
            ElementDetector.Alignment = BarItemLinkAlignment.Default;
            TabPageWithWebBrowser tabPage = this.tabPageControl.SelectedTabPage as TabPageWithWebBrowser;
            ElementDetectorBase detector = null;
            if (this.ribbon.SelectedPage.Name == "cocsite")
                detector = new COCElementDetector(tabPage.WebBrowser1, new string[] { "id", "name", "value", "onclick", "title", "ondblclick" });
            else
                detector = new GHElementDetector(tabPage.WebBrowser1, new string[] { "id", "name", "value", "onclick" });
            detector.DetectAndSave();
        }

        private void ribbon_SelectedPageChanged(object sender, EventArgs e)
        {
            if (ribbon.SelectedPage == null)
                return;
            string entripise = FileHelper.GetEntName();
            if (ribbon.SelectedPage == ghsite)
                mainWebBrowser.Url = new Uri(DeclarationSite.GHSite);
            else if (ribbon.SelectedPage == fdlsite)
                mainWebBrowser.Url = new Uri(DeclarationSite.FDLSite);
            else if (ribbon.SelectedPage == bhsite)
                mainWebBrowser.Url = new Uri(DeclarationSite.BHSite);
            else if (ribbon.SelectedPage == xnysite)
                mainWebBrowser.Url = new Uri(DeclarationSite.XNYSite);
            else if (ribbon.SelectedPage == cocsite)
                mainWebBrowser.Url = new Uri(DeclarationSite.CCCSite);
            else if (ribbon.SelectedPage == pzhsite)
                mainWebBrowser.Url = new Uri(DeclarationSite.PZSite);
            else if (ribbon.SelectedPage == cccsite)
                this.Start3CProcess();
            else if (ribbon.SelectedPage == userManagement)
            {
                if (Constants.CurrentUser == null)
                    return;
                if (Constants.CurrentUser.roleId != "4")
                {
                    try
                    {
                        string uri = FileHelper.GetTextValue("ManagementAddress");
                        uri = string.Format("{0}?userName={1}&password={2}&status=Y", uri, Constants.CurrentUser.userName, Constants.CurrentUser.password);
                        this.mainWebBrowser.Navigate(uri);
                        this.tabPageControl.SelectedTabPageIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (ribbon.SelectedPage == this.HistoryPage)
                ViewRecordHistory();
            this.ribbon.SelectedPage.Groups.Remove(this.ribbonPageGroup12);
            IDataProvider dataProvider = DataProviders.DataProviderFactory.CreateProvider(entripise, GetSelectedPageType());
            if (dataProvider != null && dataProvider.CanValidation)
            {
                this.ribbon.SelectedPage.Groups.Add(this.ribbonPageGroup12);
            }
        }

        private void ViewRecordHistory()
        {
            XtraTabPage result = new XtraTabPage();
            System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            elementHost1.Dock = DockStyle.Fill;
            System.Windows.Controls.ContentControl contentCtrl = new System.Windows.Controls.ContentControl();
            System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
            res.BeginInit();
            res.Source = new Uri("pack://application:,,,/Assistant;component/Template/RecordViewer.xaml");
            res.EndInit();
            contentCtrl.ContentTemplate = res["recordViewer"] as System.Windows.DataTemplate;
            try
            {
                contentCtrl.DataContext = HistoryHelper.GetHistoryList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            contentCtrl.Content = contentCtrl.DataContext;
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.View, this.OnViewHistoryDetail));
            elementHost1.Child = contentCtrl;
            result.Controls.Add(elementHost1);
            result.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
            result.Text = "历史记录";
            this.tabPageControl.TabPages.Add(result);
            this.tabPageControl.SelectedTabPage = result;
        }
        /// <summary>
        /// 填报按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartFill_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.importDataFilter))
                {
                    throw new Exception("未指定数据文件");
                }
                FillManagerBase test = GetFillManager(this.importDataFilter, mainWebBrowser);
                if (test == null)
                    return;
                if (test is CCCFillManager)
                    this.Start3CFill();
                test.Finished += FillManager_Finished;
                this._fillManager = test;
                this.tabPage.FillManager = this._fillManager as WebFillManager;
                test.BeginFill();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message);
            }
        }

        private void FillManager_Finished(object sender, EventArgs e)
        {
            FillManagerBase fillManager = sender as FillManagerBase;
            if (fillManager is CCCFillManager)
            {
                if (this.InvokeRequired)
                    this.Invoke((Action)(() =>
                    {
                        End3CFill();
                    }));
                else
                    End3CFill();
            }
            foreach (TabPageWithWebBrowser item in this.tabPageControl.TabPages)
            {
                if (item != null && item.FillManager == this._fillManager)
                    item.FillManager = null;
            }  
            this.Invoke((Action)(() =>
            {
                XtraTabPage result = new XtraTabPage();
                System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
                elementHost1.Dock = DockStyle.Fill;
                System.Windows.Controls.ContentControl contentCtrl = new System.Windows.Controls.ContentControl();
                System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
                res.BeginInit();
                res.Source = new Uri("pack://application:,,,/Assistant;component/Template/FillResult.xaml");
                res.EndInit();
                contentCtrl.ContentTemplate = res["fillResult"] as System.Windows.DataTemplate;
                contentCtrl.DataContext = fillManager.FillRecords;
                contentCtrl.Content = fillManager.FillRecords;
                elementHost1.Child = contentCtrl;
                result.Controls.Add(elementHost1);
                result.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                result.Text = "填报结果";
                this.tabPageControl.TabPages.Add(result);
                this.tabPageControl.SelectedTabPage = result;
            }));
            if (fillManager != null)
            {
                fillManager.Finished -= FillManager_Finished;
                try
                {
                    HistoryHelper.InsertList(fillManager.FillRecords, fillManager.FillType, Constants.CurrentUser == null ? "测试用户" : Constants.CurrentUser.userName);
                    this._fillManager = null;
                }
                catch(Exception ex)
                {
                    this.Invoke((Action)(() => {
                        MessageBox.Show(ex.Message);
                    }));
                }
            }
        }

        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.mainWebBrowser.Navigating += new WebBrowserNavigatingEventHandler(Browser_Navigating_DeleteHistory);
            this.mainWebBrowser.Navigate("http://www.vecc-mep.org.cn/newvip/newplan/planSearch.jsp?action=search");
            this.wait(1);
            GHFillConfigurationAction f1 = new GHFillConfigurationAction(this.mainWebBrowser);
            f1.overrideJS();
            for (int i = 0; i < 50; i++)
            {
                this.wait(8);
                GHFillConfigurationAction f2 = new GHFillConfigurationAction(this.mainWebBrowser);
                f2.overrideJS();
                f2.deleteFirstRadio();
            }
            this.mainWebBrowser.Navigating -= new WebBrowserNavigatingEventHandler(Browser_Navigating_DeleteHistory);
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        //删除网页历史信息
        private void Browser_Navigating_DeleteHistory(object sender, WebBrowserNavigatingEventArgs e)
        {
            WebBrowserUtils.WinApi.NativeApi.DeleteUrlCacheEntry(e.Url.OriginalString);
        }

        private void EndFill_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this._fillManager != null)
            {
                this._fillManager.EndFill();
                if (this._fillManager is CCCFillManager)
                    End3CFill();
                this._fillManager.Finished -= FillManager_Finished;
                foreach (TabPageWithWebBrowser item in this.tabPageControl.TabPages)
                {
                    if (item != null && item.FillManager == this._fillManager)
                        item.FillManager = null;
                }
                this._fillManager = null;
            }
        }

        private void Start3CFill()
        {
            resumeSize = this.Size;
            resumeLocation = this.Location;
            System.Drawing.Size newSize = new System.Drawing.Size(500, 230);
            System.Drawing.Rectangle rect = Screen.FromControl(this).Bounds;
            this.Location = new System.Drawing.Point(rect.Right - newSize.Width, rect.Bottom - newSize.Height);
            this.Size = newSize;
            this.TopMost = true;
        }

        private void End3CFill()
        {
            this.Size = resumeSize;
            this.Location = resumeLocation;
            this.TopMost = false;
        }

        private void importBtn_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void importData_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog openImportData = new OpenFileDialog();
            openImportData.Title = "选择申报车型的参数文件";
            openImportData.Filter = "Excel文件|*.xlsx;*.xls|Access 数据库文件|*.mdb;*.accdb|所有文件|*.*";
            if (openImportData.ShowDialog() == DialogResult.OK)
            {
               this.importDataFilter= openImportData.FileName;
            }
        }

        private void GenerateTreeDir_ItemClick(object sender, ItemClickEventArgs e)
        {
            CCCFillManager cccFiller = _fillManager as CCCFillManager;
            if (cccFiller != null)
                cccFiller.GenerateDir();
        }

        private void carSelect_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Windows.Window wnd = new System.Windows.Window();
            wnd.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.OK, OnCompareCarSelect));
            System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
            res.BeginInit();
            res.Source = new Uri("pack://application:,,,/Assistant;component/Template/CarTypeSelector.xaml");
            res.EndInit();
            wnd.Style = res["carTypeSelector"] as System.Windows.Style;
            wnd.ShowInTaskbar = false;
            wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            CarTypeSelector selector = new CarTypeSelector();
            if (selectedNode != null)
            {
                selector.SelectedItem = selectedNode.Header;
            }
            
            wnd.Content = selector;
            wnd.DataContext = selector;
            if (wnd.ShowDialog() == true)
            {
                selectedNode = selector.Tree.Children[0] as RuleCompareNode;
                comparer = new RuleComparer(selectedNode, mainWebBrowser);
            }
        }
        
        private void startDetect_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string httpHeader = string.Format("{0}:{1}\n{2}:{3}", "Accept", "*/*", "User-Agent", "Mozilla/5.0 (Windows NT 5.1; rv:14.0) Gecko/20100101 Firefox/14.0.1");
            //string password = FileHelper.GetStrMd5("123").ToUpper();
            //byte[] postData = Encoding.UTF8.GetBytes(string.Format("userName=admin&password={0}&status=Y", password));
            //string uri = string.Format("{0}?userName=admin&password={1}&status=Y", "http://219.234.2.24:8080/FlexServer/comm_login.action", password);
            //this.mainWebBrowser.Navigate(uri);
            //this.mainWebBrowser.Navigate("http://219.234.2.24:8080/FlexServer/admin_login.action", "", postData, httpHeader);
            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.ShowDialog();
            //string version = FileHelper.GetCurrentVersion().ToString();
            //ServiceHelper.UploadFillRule("国环", "detect", "国三", "轻型汽油车", dialog.FileName);
            //string oldMd5 = ServiceHelper.GetFillRuleMd5("国环", "detect", "国三", "轻型汽油车", dialog.FileName);
            //string md5 = FileHelper.GetFileMd5(dialog.FileName);
            //ServiceHelper.UploadAppFile("default", version, dialog.FileName);
            //ServiceHelper.UploadFillRule("国环", "detect", "国三", "轻型汽油车", dialog.FileName);
            //version = ServiceHelper.GetAppVersion("default");
            //List<AppFileInfo> fileList = ServiceHelper.GetAllFiles("default", version);
            //ServiceHelper.DownloadAppFile("default", version, dialog.FileName);
            //ServiceHelper.DownloadFillRule("国环", "detect", "国三", "轻型汽油车", dialog.FileName);
            //ServiceHelper.GetFillRuleMd5("国环", "detect", "国三", "轻型汽油车", dialog.FileName);
            //ServiceHelper.UploadFillRule("CCC", "fill", "", "", dialog.FileName);
            //ServiceHelper.DownloadFillRule("CCC", "fill", "", "", dialog.FileName);
            //ServiceHelper.DeleteAppFile("default", dialog.FileName);
            //ServiceHelper.DeleteUser("aaa");
            //ServiceHelper.RegisteUser("aaa", "zhuxiaohui", "default", "23123");
            //ServiceHelper.Login("aaa", "zhuxiaohui", "23123");
            //ServiceHelper.GetAllEnterprises();
            if (comparer == null)
            {
                MessageBox.Show("请选择站点及车辆类型！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                comparer.Finished += comparer_Finished;
                comparer.Compare();
                _fillManager = comparer.FillManager;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comparer_Finished(object sender, EventArgs e)
        {
            this.Invoke((Action)(() =>
            {
                if (this.comparer == null)
                    return;
                XtraTabPage result = new XtraTabPage();
                System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
                elementHost1.Dock = DockStyle.Fill;
                System.Windows.Controls.ContentControl contentCtrl = new System.Windows.Controls.ContentControl();
                System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
                res.BeginInit();
                res.Source = new Uri("pack://application:,,,/Assistant;component/Template/ResultViewer.xaml");
                res.EndInit();
                contentCtrl.ContentTemplate = res["resultViewer"] as System.Windows.DataTemplate;
                contentCtrl.DataContext = this.comparer.Result;
                contentCtrl.Content = this.comparer.Result;
                elementHost1.Child = contentCtrl;
                result.Controls.Add(elementHost1);
                result.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                result.Text = "网页元素对比结果";
                this.tabPageControl.TabPages.Add(result);
                this.tabPageControl.SelectedTabPage = result;
                this.comparer.Finished -= comparer_Finished;
                this.comparer = null;
                if (_fillManager != null)
                {
                    this._fillManager.EndFill();
                    this._fillManager = null;
                }
            }));
        }

        private void OpenFillRuleManager_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (XtraTabPage item in this.tabPageControl.TabPages)
            {
                if (item.Text == "填报规则管理")
                {
                    this.tabPageControl.SelectedTabPage = item;
                    return;
                }
            }
            XtraTabPage result = new XtraTabPage();
            System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            elementHost1.Dock = DockStyle.Fill;
            System.Windows.Controls.ContentControl contentCtrl = new System.Windows.Controls.ContentControl();
            System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
            res.BeginInit();
            res.Source = new Uri("pack://application:,,,/Assistant;component/Template/FillRuleManager.xaml");
            res.EndInit();
            contentCtrl.ContentTemplate = res["fillRuleManager"] as System.Windows.DataTemplate;
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.Browser, OnOpenRuleFile, CanOpenRuleFile));
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.OK, OnUploadFillRule, CanUploadFillRule));
            try
            {
                contentCtrl.DataContext = new RuleFileManager();
                contentCtrl.Content = contentCtrl.DataContext;
                elementHost1.Child = contentCtrl;
                result.Controls.Add(elementHost1);
                result.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                result.Text = "填报规则管理";
                this.tabPageControl.TabPages.Add(result);
                this.tabPageControl.SelectedTabPage = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenAppManager_ItemClick(object sender, ItemClickEventArgs e)
        {
            foreach (XtraTabPage item in this.tabPageControl.TabPages)
            {
                if (item.Text == "应用程序管理")
                {
                    this.tabPageControl.SelectedTabPage = item;
                    return;
                }
            }
            XtraTabPage result = new XtraTabPage();
            System.Windows.Forms.Integration.ElementHost elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            elementHost1.Dock = DockStyle.Fill;
            System.Windows.Controls.ContentControl contentCtrl = new System.Windows.Controls.ContentControl();
            System.Windows.ResourceDictionary res = new System.Windows.ResourceDictionary();
            res.BeginInit();
            res.Source = new Uri("pack://application:,,,/Assistant;component/Template/AppManager.xaml");
            res.EndInit();
            contentCtrl.ContentTemplate = res["appManager"] as System.Windows.DataTemplate;
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.Browser, OnOpenAppFile, CanOpenAppFile));
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.OK, OnUploadAppFile, CanUploadAppFile));
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.Add, OnAddAppFile));
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.Delete, OnDeleteAppFile, CanDeleteAppFiel));
            contentCtrl.CommandBindings.Add(new System.Windows.Input.CommandBinding(Commands.Remove, OnRemoveAppFile, CanRemoveAppFiel));
            try
            {
                contentCtrl.DataContext = new AppFileManager();
                contentCtrl.Content = contentCtrl.DataContext;
                elementHost1.Child = contentCtrl;
                result.Controls.Add(elementHost1);
                result.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
                result.Text = "应用程序管理";
                this.tabPageControl.TabPages.Add(result);
                this.tabPageControl.SelectedTabPage = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void InitPages()
        {
            this.ribbon.Pages.Clear();
            //if (Constants.CurrentUser == null)
            //    return;
            //if (Constants.CurrentUser.roleId == "1")
            //{

            List<string> allFillRights = FileHelper.GetFillRights();
            foreach (var catId in allFillRights)
            {
                switch (catId)
                {
                    case "配置序列号":
                        this.ribbon.Pages.Add(this.pzhsite);
                        break;
                    case "国环":
                        this.ribbon.Pages.Add(this.ghsite);
                        break;
                    case "非道路机动车":
                        this.fdlsite.MergedGroups.Add(this.ribbonPageGroup1);
                        this.ribbon.Pages.Add(this.fdlsite);
                        break;
                    case "北环":
                        //this.bhsite.MergedGroups.Add(this.ribbonPageGroup1);
                        this.ribbon.Pages.Add(this.bhsite);
                        break;
                    case "COC":
                        this.ribbon.Pages.Add(this.cocsite);
                        break;
                    case "CCC":
                        this.ribbon.Pages.Add(this.cccsite);
                        break;
                    case "SysManagement":
                        this.ribbon.Pages.Add(this.SysManagement);
                        break;
                    case "查看填报历史":
                        this.ribbon.Pages.Add(this.HistoryPage);
                        break;
                    case "用户管理":
                        this.ribbon.Pages.Add(this.userManagement);
                        break;
                }
            }


            //this.ribbon.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
            //this.pzhsite,
            //this.ghsite,
            //this.bhsite,
            //this.cocsite,
            //this.cccsite,
            //this.SysManagement,
            //this.HistoryPage,
            //this.userManagement});
            //    return;
            //}

            //string[] catIds = string.IsNullOrEmpty(Constants.CurrentUser.catId) ? new string[0] : Constants.CurrentUser.catId.Split(',', ' ');
            //foreach (var catId in catIds)
            //{
            //    switch (catId)
            //    {
            //    case "1":
            //        this.ribbon.Pages.Add(this.ribbonPage1);
            //        break;
            //    case "2":
            //        this.ribbon.Pages.Add(this.ribbonPage2);
            //        break;
            //    case "3":
            //        this.ribbon.Pages.Add(this.ribbonPage3);
            //        break;
            //    case "4":
            //        this.ribbon.Pages.Add(this.ribbonPage4);
            //        break;
            //    }
            //}
            //this.ribbon.Pages.Add(this.HistoryPage);
            //this.ribbon.Pages.Add(this.ribbonPage5);
        }

        private void endDetect_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (comparer != null)
            {
                comparer.StopCompare();
                this.comparer.Finished -= comparer_Finished;
                this._fillManager = null;
            }
        }

        private void ribbon_SelectedPageChanging(object sender, DevExpress.XtraBars.Ribbon.RibbonPageChangingEventArgs e)
        {
            if (this._fillManager != null)
            {
                MessageBox.Show("当前正在进行填报，若要打开其它页面，请结束当前填报！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
            }
        }

        private HtmlDocument FindDocument(HtmlDocument doc)
        {
            if (doc == null || doc.Url.OriginalString == "http://tec.cqccms.com.cn/carTypeSeq!allCarType.action")
            {
                return doc;
            }
            else
            {
                HtmlDocument finded = null;
                for (int i = 0; i < doc.Window.Frames.Count; i++)
                {
                    finded = FindDocument(doc.Window.Frames[i].Document);
                    if (finded != null)
                        return finded;
                }
                return null;
            }
        }

        private void Validate_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.importDataFilter))
                {
                    throw new Exception("未指定数据文件");
                }
                FillManagerBase test = GetFillManager(this.importDataFilter, mainWebBrowser);
                if (test == null)
                    return;
                if (test.DataProvider.CanValidation)
                {
                    test.DataProvider.DataSourceFile = this.importDataFilter;
                    if (test.DataProvider.Validate() == false)
                    {
                        MessageBox.Show("数据文件中存在错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        MessageBox.Show("数据已验证！", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message);
            }
        }
    }
}