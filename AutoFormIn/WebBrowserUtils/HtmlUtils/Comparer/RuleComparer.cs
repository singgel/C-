using Assistant.DataProviders;
using AssistantUpdater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Data.Collection;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    public class RuleComparer
    {
        private RuleCompareNode _root;
        private WebBrowserUtils.ExtendWebBrowser.WebBrowser2 _webBrowser;
        private FillAsyncHandler _asyncHandler;
        private WebFillManager fillManager;
        private System.Threading.Thread _thread;
        private static readonly string version;
        private const string configName = "config.xml";

        public event EventHandler Finished;

        public RuleCompareNode Root
        {
            get { return _root; }
        }

        public TreeModel Result
        {
            get;
            private set;
        }

        public WebFillManager FillManager
        {
            get { return fillManager; }
        }

        public RuleComparer(RuleCompareNode root, WebBrowserUtils.ExtendWebBrowser.WebBrowser2 webBrowser)
        {
            _root = root;
            _webBrowser = webBrowser;
        }

        static RuleComparer()
        {
            version = FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.Compare);
        }

        private void UpdateDetectRule(string type, string standard, string carType)
        {
            string detectVersion = FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.Detect);
            string fileName = string.Format("{0}\\{1}", detectVersion, FileHelper.GetFillRuleFile(Properties.Resources.Detect, type, standard, carType));
            string oldMd5 = ServiceHelper.GetFillRuleMd5(type, Properties.Resources.Detect, standard, carType, fileName);
            if (oldMd5 == FileHelper.GetFileMd5(fileName))
                return;
            ServiceHelper.DownloadFillRule(type, Properties.Resources.Detect, standard, carType, fileName);
        }

        private string GetCompareRuleFileName(string type, string standard, string carType, string appendix)
        {
            string fileName = FileHelper.GetFillRuleFile(Properties.Resources.Compare, type, standard, carType);
            FileInfo info = new FileInfo(fileName);
            int len = fileName.Length > info.Name.Length ? fileName.Length - info.Name.Length - 1 : 0;
            return string.Format("{0}\\{1}", fileName.Substring(0, len), GetName(info.Name, appendix));
        }

        private string GetName(string fileName, string appendix)
        {
            return string.IsNullOrEmpty(appendix) ? fileName : string.Format("{0}-{1}", appendix , fileName);
        }

        private string GetServerFile(string standard, string carType, string type, string appendix)
        {
            string oldFile = string.Format("{0}\\{1}", version, GetCompareRuleFileName(type, standard, carType, appendix));
            string serverMd5 = ServiceHelper.GetFillRuleMd5(type, Properties.Resources.Compare, standard, carType, oldFile);
            if (serverMd5 != FileHelper.GetFileMd5(oldFile))
            {
                ServiceHelper.DownloadFillRule(type, Properties.Resources.Compare, standard, carType, oldFile);
            }
            return oldFile;
        }

        public void Compare()
        {
            if (_thread != null && _thread.IsAlive)
            {
                _thread.Abort();
            }
            string type = _root.Header;
            string dir = string.Format("{0}\\Temp", version);
            string dataFile = string.Format("{0}\\{1}\\{2}", version, type, FileHelper.GetTextValue("DefaultDetectDataFile"));
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);
            if (type == "国环")
                fillManager = new GHComparerFillManager(_webBrowser, dataFile);
            else if (type == "北环")
                fillManager = new BHComparerFillManager(_webBrowser, dataFile);
            if (fillManager != null)
            {
                fillManager.Finished += fillManager_Finished;
                _thread = new System.Threading.Thread(CompareWorker);
                _thread.Start(dir);
            }
            else
                throw new ArgumentException("未定义的元素感知页面！");
        }

        private void CompareWorker(object state)
        {
            try
            {
                TreeModel result = new TreeModel();
                string type = _root.Header;
                RuleCompareNode root = new RuleCompareNode() { Header = _root.Header, IsExpanded = true };
                result.AddChild(root);
                RuleCompareNode resultItem;
                string path = state as string;
                if (type == "国环")
                {
                    _asyncHandler = new FillAsyncHandler();
                    List<KeyValuePair<string, string>> appendixes = FileHelper.GetAppendixes(type);
                    foreach (RuleCompareNode item in _root.Children)
                    {
                        if (item.IsChecked == false)
                            continue;
                        // 排放标准
                        resultItem = new RuleCompareNode() { Header = item.Header };
                        root.AddChild(resultItem);
                        foreach (var appendix in appendixes)
                        {
                            RuleCompareNode appendixNode = new RuleCompareNode() { Header = appendix.Key };
                            resultItem.AddChild(appendixNode);
                            CompareInner(item, appendixNode, path, type, appendix);
                        }
                    }
                }
                else if (type == "北环")
                {
                    _asyncHandler = new FillAsyncHandler();
                    foreach (RuleCompareNode item in _root.Children)
                    {
                        if (item.IsChecked == false)
                            continue;
                        // 排放标准
                        resultItem = new RuleCompareNode() { Header = item.Header };
                        root.AddChild(resultItem);
                        CompareInner(item, resultItem, path, type, null);
                    }
                }
                this.Result = result;
                this.fillManager.EndFill();
                OnFinished(EventArgs.Empty);
                this.fillManager = null;
            }
            catch (Exception ex)
            {
                WebFillManager.ShowMessageBox(ex.Message, "错误", System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                if (this.fillManager != null)
                {
                    this.fillManager.EndFill();
                    this.fillManager = null;
                }
            }
        }

        public void StopCompare()
        {
            if (this.fillManager != null)
            {
                try
                {
                    fillManager.EndFill();
                    fillManager.Finished -= fillManager_Finished;
                    if (_thread != null)
                        _thread.Abort();
                }
                catch (System.Threading.ThreadAbortException)
                {
                }
                finally
                {
                    _thread = null;
                    fillManager = null;
                }
            }
        }

        private void CompareInner(RuleCompareNode standard, RuleCompareNode parent,
            string path, string type, KeyValuePair<string,string>? appendix)
        {
            string uri = fillManager.StartPageUri;
            string appendixKey = appendix == null ? "" : appendix.Value.Key;
            foreach (RuleCompareNode child in standard.Children)
            {
                if (child.IsChecked != true)
                    continue;
                this.UpdateDetectRule(type, standard.Content as string, child.Header);
                fillManager.Browser.Navigate(uri);
                RuleCompareNode resultChildItem = new RuleCompareNode() { Header = child.Header };
                parent.AddChild(resultChildItem);
                // 车辆类型
                _webBrowser.Invoke((Action)(() => { fillManager.BeginFill(); }));
                fillManager.Standard = standard.Content as string;
                fillManager.CarType = child.Header;
                fillManager.Data["选择底盘类型"] = appendix == null ? "" : appendix.Value.Value;
                string fileName = GetCompareRuleFileName(type, fillManager.Standard, fillManager.CarType, appendixKey);
                fileName = string.Format("{0}\\{1}", path, fileName);
                FileInfo info = new FileInfo(fileName);
                if (info.Directory.Exists == false)
                    info.Directory.Create();
                _asyncHandler.Reset();
                _asyncHandler.Wait();
                string detectedFileName = "";
                GHComparerFillManager gh = fillManager as GHComparerFillManager;
                BHComparerFillManager bh = null;
                if (gh != null)
                    detectedFileName = gh.DetectedFileName;
                else
                    bh = fillManager as BHComparerFillManager;
                if (bh != null)
                    detectedFileName = bh.DetectedFileName;
                if (bh == null && gh == null)
                    return;
                else if (bh != null)
                    bh.Reset();
                else
                    gh.Reset();
                if (File.Exists(fileName))
                    File.Delete(fileName);
                File.Move(detectedFileName, fileName);
                string oldFile = GetServerFile(fillManager.Standard, fillManager.CarType, type, appendixKey);
                string newFile = fileName;
                if (File.Exists(oldFile) == false)
                {
                    using (Office.Excel.ForwardExcelReader newWorkbook = new Office.Excel.ForwardExcelReader(newFile))
                    {
                        newWorkbook.Open();
                        foreach (var node in CompareFile(null, newWorkbook))
                        {
                            resultChildItem.AddChild(node);
                            if(node.HasChange)
                                resultChildItem.HasChange = node.HasChange;
                        }
                    }
                }
                else
                {
                    using (Office.Excel.ForwardExcelReader newWorkbook = new Office.Excel.ForwardExcelReader(newFile))
                    {
                        using (Office.Excel.ForwardExcelReader oldWorkbook = new Office.Excel.ForwardExcelReader(oldFile))
                        {
                            oldWorkbook.Open();
                            newWorkbook.Open();
                            foreach (var node in CompareFile(oldWorkbook, newWorkbook))
                            {
                                resultChildItem.AddChild(node);
                                if (node.HasChange)
                                    resultChildItem.HasChange = node.HasChange;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<RuleCompareNode> CompareFile(Office.Excel.ForwardExcelReader oldWorkbook, Office.Excel.ForwardExcelReader newWorkbook)
        {
            string[] sheets = oldWorkbook == null?new string[0] : oldWorkbook.GetSheetsName();
            int index;
            for (index = 0; index < sheets.Length; index++)
            {
                RuleCompareNode node = new RuleCompareNode();
                node.Header = sheets[index];
                PageGroup group = new PageGroup(sheets[index], oldWorkbook, newWorkbook);
                group.Parent = node;
                node.Content = group;
                group.ReadRules();
                group.Compare();
                yield return node;
            }
            sheets = newWorkbook.GetSheetsName();
            for (; index < sheets.Length; index++)
            {
                RuleCompareNode node = new RuleCompareNode();
                node.Header = sheets[index];
                PageGroup group = new PageGroup(sheets[index], oldWorkbook, newWorkbook);
                group.Parent = node;
                node.Content = group;
                group.ReadRules();
                group.Compare();
                yield return node;
            }
        }

        protected virtual void OnFinished(EventArgs e)
        {
            if (Finished != null)
                Finished(this, e);
        }

        private void fillManager_Finished(object sender, EventArgs e)
        {
            if(_asyncHandler != null)
                _asyncHandler.Resume();
        }
    }
}
