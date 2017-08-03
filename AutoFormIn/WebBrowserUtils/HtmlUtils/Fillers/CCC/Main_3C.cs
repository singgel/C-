//#define export // 导出下拉列表框时使用
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 主窗口。
    /// </summary>
    public class Main_3C : IDisposable
    {
        private uint processId;
        private IntPtr _handle, _selectedNode;
        private IntPtr hProcess, processAddr;
        private SysTreeView _treeView;
        private List<KeyValuePair<string, object>> _keyRelation;
        private Hashtable _pageData;
        private Hashtable _fillParameters;
        private FillAsyncHandler _asyncHandle;
        private FillState _state;
        private string dataFile;
        internal static readonly Regex keyMatcher, nodeMatcher;
        internal static readonly Regex suffixMatcher;
        private static readonly Hashtable UnnumberedParameter;
#if export
        private Office.Excel.ForwardExcelWriter writer;
        private Office.Excel.ForwardWriteWorksheet sheet;
#endif
        public string DataFile
        {
            get { return dataFile; }
            set
            {
                if (dataFile != value)
                {
                    dataFile = value;
                    if (string.IsNullOrEmpty(dataFile) == false)
                    {
                        FileInfo info = new FileInfo(dataFile);
                        DataFilePath = info.DirectoryName;
                    }
                }
            }
        }

        public string DataFilePath
        {
            get;
            private set;
        }

        //public Hashtable FileTable
        //{
        //    get;
        //    internal set;
        //}

        public Hashtable FillParameters
        {
            get { return _fillParameters; }
            internal set { _fillParameters = value; }
        }

        internal List<FillRecord> Records
        {
            get;
            set;
        }

        public IntPtr Hwnd
        {
            get { return _handle; }
        }

        internal IntPtr HProcess
        {
            get { return hProcess; }
        }

        internal IntPtr ProcessAddr
        {
            get { return processAddr; }
        }

        public uint ProcessId
        {
            get { return processId; }
        }
        /// <summary>
        /// 获取当前选中节点的文本内容。
        /// </summary>
        public string SelectedText
        {
            get;
            private set;
        }

        public FillState State
        {
            get { return _state; }
        }

        public TreeValue TreeValue
        {
            get;
            internal set;
        }

        public SysTreeView TreeView
        {
            get { return _treeView; }
        }

        private Main_3C(IntPtr hwnd, string dataFile, uint processId)
        {
            DataFile = dataFile;
            _handle = hwnd;
            _selectedNode = IntPtr.Zero;
            _keyRelation = new List<KeyValuePair<string, object>>();
            _treeView = null;
            this.processId = processId;
            hProcess = NativeApi.OpenProcess(0x1FFFFF, false, this.processId);
            processAddr = NativeApi.VirtualAllocEx(hProcess, IntPtr.Zero, 4096, NativeApi.MEM_COMMIT, NativeApi.PAGE_READWRITE);
            _asyncHandle = new FillAsyncHandler();
            this.GetTreeView();
#if export
            sheet = null;
            writer = null;
#endif
        }

        ~Main_3C()
        {
            Dispose(false);
        }

        static Main_3C()
        {
            keyMatcher = new Regex(@"^C?\.?[0-9]+(\.[0-9]+)*$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            nodeMatcher = new Regex(@"^(?<no>C?\.?[0-9]+(\.[0-9]+)*).*$", RegexOptions.Singleline | RegexOptions.ExplicitCapture);
            suffixMatcher = new Regex(@"^C?\.?[0-9]+(\.[0-9]+)*_(?<suffix>.*)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
            UnnumberedParameter = new Hashtable();
            UnnumberedParameter.Add("是否具有\"前伸量\"和\"后伸量\":", "C2.4.2");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool p)
        {
            if (processAddr != IntPtr.Zero)
            {
                NativeApi.VirtualFreeEx(hProcess, processAddr, 4096, NativeApi.MEM_RELEASE);
                processAddr = IntPtr.Zero;
            }
            if (hProcess != IntPtr.Zero)
            {
                NativeApi.CloseHandle(hProcess);
                hProcess = IntPtr.Zero;
            }
            if (_asyncHandle != null)
            {
                _asyncHandle.Dispose();
                _asyncHandle = null;
            }
#if export
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
                sheet = null;
            }
#endif
        }
        
        public static Main_3C GetMainWindow(string dataFileName, uint processId)
        {
            IntPtr hwnd = IntPtr.Zero;
            StringBuilder title = new StringBuilder(256);
            do
            {
                System.Threading.Thread.Sleep(500);
                hwnd = FillDialog_3C.GetWindowHandle(IntPtr.Zero, processId);
                NativeApi.GetWindowText(hwnd, title, 255);
            } while (hwnd == IntPtr.Zero || title.ToString().StartsWith("机动车产品强制认证参数申报管理系统客户端") == false);

            return new Main_3C(hwnd, dataFileName, processId);
        }
        /// <summary>
        /// 获取当前窗体中的TreeView控件。
        /// </summary>
        /// <returns></returns>
        public SysTreeView GetTreeView()
        {
            IntPtr treeView = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(_handle, (handle, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(handle, className, 255);
                string str = className.ToString();
                if (str.StartsWith(CCCFillManager.TreeViewClassName))
                    treeView = handle;

                if (treeView != IntPtr.Zero)
                    return false;
                return true;
            }, IntPtr.Zero);
            _treeView = treeView == IntPtr.Zero ? null : new SysTreeView(treeView);
            return _treeView;
        }
        /// <summary>
        /// 获得窗口中TreeView的节点间的父子级关系。
        /// </summary>
        /// <returns></returns>
        public TreeValue GetTreeStructure()
        {
            if (_treeView == null)
                return null;
            TreeValue root = null, current = null, parent = null;
            IntPtr nextNode = IntPtr.Zero, currentNode = IntPtr.Zero, parentNode = IntPtr.Zero;
            parentNode = _treeView.GetRoot();
            currentNode = parentNode;
            root = new TreeValue(_treeView.GetItemText(parentNode, hProcess, processAddr));
            current = parent = root;
            do
            {
                ClickNode(currentNode);
                _treeView.ExpandNode(currentNode);
                nextNode = _treeView.GetFirstChildItem(currentNode);
                if (nextNode == IntPtr.Zero)
                {
                    nextNode = _treeView.GetNextNode(currentNode);
                    if (nextNode == IntPtr.Zero)
                    {
                        do
                        {
                            _treeView.CollapseNode(parentNode);
                            nextNode = _treeView.GetNextNode(parentNode);
                            parentNode = _treeView.GetParentNode(parentNode);
                            parent = parent.Parent;
                        } while (nextNode == IntPtr.Zero && parent != null);
                    }
                }
                else
                {
                    parentNode = currentNode;
                    parent = current;
                }
                if (nextNode != IntPtr.Zero)
                {
                    currentNode = nextNode;
                    current = new TreeValue(_treeView.GetItemText(currentNode, hProcess, processAddr));
                    parent.AddChild(current);
                }
            } while (parent != null);
            return root;
        }

        internal void UpdateSelectedNode()
        {
            _selectedNode = _treeView.GetSelected();
        }
        /// <summary>
        /// 选择当前节点的下一个可见节点。
        /// </summary>
        /// <param name="click">是否对其执行单击操作。</param>
        /// <returns></returns>
        public bool SelectNextNode(bool click)
        {
            _asyncHandle.Wait();
            this.UpdateSelectedNode();
            IntPtr root = _selectedNode;
            IntPtr child = IntPtr.Zero;
            if (root != IntPtr.Zero)
            {
                ClickNode(root);
                _treeView.ExpandNode(root);
            }
            child = _treeView.GetFirstChildItem(root);
            if (child == IntPtr.Zero)
                child = _treeView.GetNextNode(root);
            if (child == IntPtr.Zero)
                child = _treeView.GetNextVisible(root);

            if (child != IntPtr.Zero && click)
                return ClickNode(child);
            return false;
        }
        /// <summary>
        /// 点击指定节点。
        /// </summary>
        /// <param name="node"></param>
        private unsafe bool ClickNode(IntPtr node)
        {
            uint n = 0;
            RECT dest;
            RECT* rect = (RECT*)processAddr.ToPointer();
            dest.Left = node.ToInt32();
            NativeApi.WriteProcessMemory(hProcess, processAddr, (IntPtr)(&dest), sizeof(RECT), ref n);
            NativeApi.SendMessage(_treeView.Handle, WMMSG.TVM_GETITEMRECT, 0, processAddr.ToInt32());
            NativeApi.ReadProcessMemory(hProcess, processAddr, (IntPtr)(&dest), sizeof(RECT), ref n);

            NativeApi.SendMessage(_treeView.Handle, WMMSG.WM_LBUTTONDOWN, 0, (dest.Top << 16) | (dest.Left & 0xFFFF));
            _treeView.SelectNode(node);
            System.Threading.Thread.Sleep(10);
            NativeApi.SendMessage(_treeView.Handle, WMMSG.WM_LBUTTONUP, 0, (dest.Top << 16) | (dest.Left & 0xFFFF));
            _selectedNode = node;
            //this.GetSelectedText();
            return true;
        }
        /// <summary>
        /// 对指定节点进行追加。
        /// </summary>
        /// <param name="node">要对其进行追加的节点句柄。</param>
        /// <param name="name">追加的节点的后缀名称。</param>
        public void AppendNode(IntPtr node, string name)
        {
            this.ClickAppendMenu(node);
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.InputFileNameWindow, this, processId);
            fill.DoFillWork(new FillValue3C() { Value = name });
        }

        private unsafe bool ClickAppendMenu(IntPtr appendNode)
        {
            //System.Threading.ThreadPool.QueueUserWorkItem(this.ListenInputFileNameWindow, null);
            uint n = 0;
            RECT dest;
            RECT* rect = (RECT*)processAddr.ToPointer();
            RECT windowRect;
            NativeApi.SwitchToThisWindow(_handle, true);
            NativeApi.GetWindowRect(_treeView.Handle, out windowRect);
            dest.Left = appendNode.ToInt32();
            NativeApi.WriteProcessMemory(hProcess, processAddr, (IntPtr)(&dest), sizeof(RECT), ref n);
            NativeApi.SendMessage(_treeView.Handle, WMMSG.TVM_GETITEMRECT, 0, processAddr.ToInt32());
            NativeApi.ReadProcessMemory(hProcess, processAddr, (IntPtr)(&dest), sizeof(RECT), ref n);
            dest.Left = windowRect.Left + dest.Left + 10;
            dest.Top = windowRect.Top + dest.Top + 3;
            NativeApi.SetCursorPos(dest.Left, dest.Top);
            NativeApi.mouse_event(MouseEventFlags.MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
            NativeApi.mouse_event(MouseEventFlags.MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            NativeApi.keybd_event((byte)System.Windows.Forms.Keys.Down, 0, 0, 0);
            NativeApi.keybd_event((byte)System.Windows.Forms.Keys.Enter, 0, 0, 0);
            return true;
        }
        /// <summary>
        /// 点击保存按钮。
        /// </summary>
        /// <param name="saveButton"></param>
        internal void ClickSaveButton(IntPtr saveButton)
        {
            this.ClickSaveButton(saveButton, false);
        }

        internal void ClickSaveButton(IntPtr saveButton, bool selectNextNode)
        {
            if (saveButton != IntPtr.Zero)
            {
                ApiSetter.ClickButton(saveButton, _handle, (state) =>
                {
                    SaveDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.SaveWindow, this, processId) as SaveDialog_3C;
                    if (string.IsNullOrEmpty(fill.Message) || fill.Message.Contains("保存成功") == false)
                    {
                        _state = FillState.Waiting;
                        if (_asyncHandle != null)
                            _asyncHandle.Reset();
                    }
                    else
                    {
                        fill.DoFillWork(null);
                        if (selectNextNode)
                            this.SelectNextNode(true);
                    }
                }, null);
            }
            else
            {
                _state = FillState.Waiting;
                if (_asyncHandle != null)
                    _asyncHandle.Reset();
            }
        }
        /// <summary>
        /// 获取当前选中节点的文本值。
        /// </summary>
        private void GetSelectedText()
        {
            if (_selectedNode == IntPtr.Zero || _treeView == null)
                SelectedText = null;
            SelectedText = _treeView.GetItemText(_selectedNode, hProcess, processAddr);
        }
        /// <summary>
        /// 获取当前选中节点的以"/"分割的路径。
        /// </summary>
        /// <returns></returns>
        private string GetSelectedNodePath()
        {
            string path = this.SelectedText;
            IntPtr root = _treeView.GetRoot();
            IntPtr parent = IntPtr.Zero, current = _selectedNode;
            while(current != root)
            {
                parent = _treeView.GetParentNode(current);
                //if (parent != root && parent != IntPtr.Zero)
                //{
                    path = string.Format("{0}#{1}", _treeView.GetItemText(parent, hProcess, processAddr), path.Trim());
                    current = parent;
                //}
            } 
            //while (parent != IntPtr.Zero && parent != root);
            return path;
        }
        /// <summary>
        /// 从当前程序的TreeView中查找具有指定节点名称的子级节点句柄。
        /// </summary>
        /// <param name="parentNode">要对其进行查找的节点的父级。</param>
        /// <param name="nodeName">节点名称。</param>
        /// <returns></returns>
        private IntPtr FindChild(IntPtr parentNode, string nodeName)
        {
            _treeView.ExpandNode(parentNode);
            IntPtr child = _treeView.GetFirstChildItem(parentNode);
            while (child != IntPtr.Zero)
            {
                if (_treeView.GetItemText(child, hProcess, processAddr) == nodeName)
                    return child;
                child = _treeView.GetNextNode(child);
            }
            return child;
        }

        public bool FillPage()
        {
            this.GetSelectedText();
            if (string.IsNullOrEmpty(this.SelectedText))
                return true;
            string path = GetSelectedNodePath();
            TreeValue currentPage = TreeValue;
            if (path != null)
                currentPage = currentPage.Select(path);
            if (currentPage == null)
                return false;
           
            if (currentPage != null && currentPage.Parent != null)
            {
#if export
                if (writer == null)
                {
                    writer = new Office.Excel.ForwardExcelWriter(string.Format("{0}\\{1}.xlsx", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "3C下拉列表框"));
                    writer.Open();
                    sheet = writer.CreateWorksheet() as Office.Excel.ForwardWriteWorksheet;
                    sheet.WriteNextRow();
                    sheet.WriteTextCell(1, writer.AddSharedString("CCC编号"));
                }
#endif
                _pageData = currentPage.Values;
                FillPage(currentPage);
            }

            foreach (TreeValue child in currentPage.Children)
            {
                if (child.IsAppendNode)
                {
                    IntPtr childPtr = FindChild(_selectedNode, child.Name);
                    if (childPtr != IntPtr.Zero)
                    {
                        _treeView.ScrollIntoView(childPtr);
                        this.AppendNode(childPtr, child.Suffix);
                    }
                    Match match = child.Name == null ? Match.Empty : nodeMatcher.Match(child.Name);
                    if (match.Success)
                    {
                        if (match.Groups["no"].Value == "C10.1.1.1")
                        {
                            child.Rename("C10.1.1.1前照灯（远光/近光）");
                        }
                    }
                }
            }
            return true;
        }

        private bool FillPage(TreeValue currentValue)
        {
            Match match = nodeMatcher.Match(this.SelectedText);
            if (match.Success == false)
                return false;
            switch (match.Groups["no"].Value)   // 填报特殊页面
            {
            case "9.10.2.3.1":
            case "9.10.2.3.2":
            case "4.6":
            case "12.7.1":
            case "9.12.1":
            case "9.12.2":
            case "9.13.3":
            case "3.9.8.1.1":
                return FillSpecialPage(match.Groups["no"].Value, null);
            }
            StringBuilder className = new StringBuilder(256);
            IntPtr saveButton = IntPtr.Zero;
            IntPtr container = IntPtr.Zero;
            NativeApi.EnumChildWindows(_handle, (child, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(child, className, 255);
                string name = className.ToString();
                if (name.StartsWith(CCCFillManager.StaticClassName))
                {
                    className.Clear();
                    NativeApi.GetWindowText(child, className, 255);
                    string text = className.ToString();
                    if (keyMatcher.IsMatch(text))
                    {
                        container = NativeApi.GetParent(child);
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);  // 获取承载填报页的控件句柄。

            if (container != IntPtr.Zero)
            {
                saveButton = this.GetParameters(container);
                foreach (KeyValuePair<string, object> entry in _keyRelation)
                {
                    FillValue(entry.Key, entry.Value);
                }
                this.ClickSaveButton(saveButton);
            }
            return true;
        }

        private bool FillSpecialPage(string pageNo, string suffix)
        {
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(this.dataFile))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(pageNo) as Office.Excel.ForwardReadWorksheet;
                if (sheet == null)
                    return true;
                SpecialPage.NormalAddPage addPage = null;
                switch (pageNo)   // 填报特殊页面
                {
                case "9.10.2.3.1":
                case "9.10.2.3.2":
                    SpecialPage.Page9_10_2_3 page = new SpecialPage.Page9_10_2_3(this._handle, this, sheet);
                    return page.FillPage();
                case "4.6":
                case "12.7.1":
                case "3.9.8.1.1":
                    addPage = new SpecialPage.NormalAddPage(_handle, this, sheet);
                    addPage.InitHandle();
                    return addPage.FillPage();
                case "9.12.1":
                    addPage = new SpecialPage.Page9_12_1(_handle, this, sheet);
                    addPage.InitHandle();
                    return addPage.FillPage();
                case "9.12.2":
                    addPage = new SpecialPage.Page9_12_2(_handle, this, sheet);
                    addPage.InitHandle();
                    return addPage.FillPage();
                case "9.13.3":
                    addPage = new SpecialPage.Page9_13_3(_handle, this, sheet);
                    addPage.InitHandle();
                    return addPage.FillPage();
                }
            }
            return false;
        }
        /// <summary>
        /// 从页面中查找保存按钮。
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private IntPtr FindSaveButton(IntPtr container)
        {
            IntPtr parent = NativeApi.GetParent(container);
            if (parent == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr childAfter = IntPtr.Zero;
            StringBuilder text = new StringBuilder(256);
            StringBuilder className = new StringBuilder(256);
            do
            {
                text.Clear();
                className.Clear();
                childAfter = NativeApi.FindWindowEx(parent, childAfter, null, null);
                NativeApi.GetClassName(childAfter, className, 255);
                if (className.ToString().StartsWith(CCCFillManager.ButtonClassName))
                {
                    NativeApi.GetWindowText(childAfter, text, 255);
                    if (text.ToString() == "保存")
                        return childAfter;
                }
            } while (childAfter != IntPtr.Zero);
            return FindSaveButton(parent);
        }
        /// <summary>
        /// 从页面中查找需填写的参数键值及控件句柄间的对应关系，将其保存到_keyRelation列表中。
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        private IntPtr GetParameters(IntPtr container)
        {
            _keyRelation.Clear();
            IntPtr afterChild = IntPtr.Zero;
            IntPtr finded = IntPtr.Zero;
            IntPtr saveButton = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            StringBuilder text = new StringBuilder(256);
            List<IntPtr> controlList = new List<IntPtr>();
            do
            {
                text.Clear();
                className.Clear();
                finded = NativeApi.FindWindowEx(container, afterChild, null, null);
                if (finded != IntPtr.Zero)
                {
                    NativeApi.GetClassName(finded, className, 255);
                    string ClassName = className.ToString();
                    if (ClassName.StartsWith(CCCFillManager.StaticClassName))
                    {
                        NativeApi.GetWindowText(finded, text, 255);
                        string controlText = text.ToString();
                        if (keyMatcher.IsMatch(controlText))
                            _keyRelation.Add(new KeyValuePair<string, object>(controlText, finded));
                        else if (UnnumberedParameter.ContainsKey(controlText))
                            _keyRelation.Add(new KeyValuePair<string, object>(UnnumberedParameter[controlText] as string, finded));
                    }
                    else if (ClassName.StartsWith(CCCFillManager.ButtonClassName))
                    {
                        text.Clear();
                        NativeApi.GetWindowText(finded, text, 255);
                        if (text.ToString() == "保存" && NativeApi.GetParent(finded) == container)
                            saveButton = finded;
                        controlList.Add(finded);
                    }
                    else 
                        controlList.Add(finded);
                    afterChild = finded;
                }
            } while (finded != IntPtr.Zero);
            ControlSorter.SortKey(_keyRelation);
            this.GetKeyRelation(container, controlList);
            if (saveButton == IntPtr.Zero)
                return FindSaveButton(container);
            return saveButton;
        }
        /// <summary>
        /// 获取参数名与控件句柄间的对应关系
        /// </summary>
        /// <param name="container">用于查找本页的保存按钮</param>
        /// <param name="controlList">控件句柄列表</param>
        private void GetKeyRelation(IntPtr container, List<IntPtr> controlList)
        {
            IntPtr finding = IntPtr.Zero;
            RECT keyRect, findedRect;
            StringBuilder className = new StringBuilder(256);
            
            for (int keyIndex = 0; keyIndex < _keyRelation.Count; keyIndex++)
            {
                finding = (IntPtr)_keyRelation[keyIndex].Value;
                _keyRelation[keyIndex] = new KeyValuePair<string,object>(_keyRelation[keyIndex].Key, IntPtr.Zero);
                NativeApi.GetWindowRect(finding, out keyRect);
                List<IntPtr> list = null;
                for (int i = 0; i < controlList.Count; i++)
                {
                    NativeApi.GetWindowRect(controlList[i], out findedRect);
                    if (findedRect.Top <= keyRect.Top && findedRect.Bottom >= keyRect.Bottom)
                    {
                        list = _keyRelation[keyIndex].Value as List<IntPtr>;
                        if (list == null)
                        {
                            list = new List<IntPtr>();
                            _keyRelation[keyIndex] = new KeyValuePair<string, object>(_keyRelation[keyIndex].Key, list);
                        }
                        list.Add(controlList[i]);
                        controlList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        int diffrent = (findedRect.Top + findedRect.Bottom) / 2 - (keyRect.Top + keyRect.Bottom) / 2;
                        if (diffrent <= 8 && diffrent >= -8)
                        {
                            list = _keyRelation[keyIndex].Value as List<IntPtr>;
                            if (list == null)
                            {
                                list = new List<IntPtr>();
                                _keyRelation[keyIndex] = new KeyValuePair<string, object>(_keyRelation[keyIndex].Key, list);
                            }
                            list.Add(controlList[i]);
                            controlList.RemoveAt(i);
                            i--;
                        }
                    }
                    if (list != null)
                        ControlSorter.SortControlList(list); // 将与指定参数名称在同一行的控件按从左到右的顺序排列
                }
            }
        }
        /// <summary>
        /// 根据参数键值及控件句柄填写参数。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fillControl"></param>
        /// <returns></returns>
        private bool FillValue(string key, object fillControl)
        {
            IntPtr current = IntPtr.Zero;
            List<IntPtr> list = fillControl as List<IntPtr>;
           if (list != null && list.Count == 0)
                return true;

            if (string.IsNullOrEmpty(key))
                return true;
            FillParameter3C fillParameter = _fillParameters[key] as FillParameter3C;
            if (fillParameter == null)
            {
                fillParameter = new FillParameter3C() { Key = key, Separators = FillParameter3C.DefaultSeparators };
                _fillParameters.Add(key, fillParameter);
            }
            FillValue3C fillValue = _pageData[key] as FillValue3C;
            if (fillValue == null)
            {
                Records.Add(new FillRecord(ElementType.Unknown, RecordType.Failed, "数据文件中未定义此参数", key));
                return true;
            }
            StringBuilder className = new StringBuilder(256);
            string name = "";
            if (list != null)
            {
                current = list[0];
                className = new StringBuilder(256);
                NativeApi.GetClassName(current, className, 255);
                name = className.ToString();
                // 勾选是否适用复选框
                if (name.StartsWith(CCCFillManager.ButtonClassName))
                {
                    if (fillValue.Value == "N/A")
                    {
                        if (ApiSetter.IsDisabled(current) == false)
                        {
                            ApiSetter.SetCheck(current, this.Hwnd);
                            //ApiSetter.ClickButton(current, this.Hwnd, null, null);
                            return true;
                        }
                    }
                    current = list.Count > 1 ? list[list.Count - 1] : current;
                }
            }
            else
                current = (IntPtr)fillControl;


            if (fillParameter.Type == "5" || fillParameter.Type == "7")
            {
                return FillCheckBox(fillParameter, current);
            }


            className.Clear();
            NativeApi.GetClassName(current, className, 255);
            name = className.ToString();
            if (name.StartsWith(CCCFillManager.StaticClassName))
                return true;
            else if (name.StartsWith(CCCFillManager.EditClassName))
                return FillValue(CCCFillManager.EditClassName, current, fillValue);
            else if (name.StartsWith(CCCFillManager.ButtonClassName))
                return FillValue(CCCFillManager.ButtonClassName, current, fillValue);
            else if (name.StartsWith(CCCFillManager.ComboBoxClassName))
            {
#if export
                sheet.WriteNextRow();
                sheet.WriteTextCell(1, writer.AddSharedString(key));
#endif
                return FillValue(CCCFillManager.ComboBoxClassName, current, fillValue);
            }
            else
            {   // 多值填报
                List<IntPtr> sortedControl = ControlSorter.SortContainer(current);
                if (sortedControl == null)
                    return false;
                else if (sortedControl.Count == 1)
                    return FillValue(key, sortedControl[0]);
                int valueIndex = 0;
                for (int i = 0; i < sortedControl.Count; i++, valueIndex++)
                {
                    className.Clear();
                    NativeApi.GetClassName(sortedControl[i], className, 255);
                    if (className.ToString().StartsWith(CCCFillManager.ButtonClassName))
                        FillValue(CCCFillManager.ButtonClassName, ref valueIndex, sortedControl[i], fillValue);
                    else if (className.ToString().StartsWith(CCCFillManager.ComboBoxClassName))
                    {
#if export
                        sheet.WriteNextRow();
                        sheet.WriteTextCell(1, writer.AddSharedString(key));
#endif
                        FillValue(CCCFillManager.ComboBoxClassName, ref valueIndex, sortedControl[i], fillValue);
                    }
                    else if (className.ToString().StartsWith(CCCFillManager.EditClassName))
                        FillValue(CCCFillManager.EditClassName, ref valueIndex, sortedControl[i], fillValue);
                }
            }
            return true;
        }

        private bool FillCheckBox(FillParameter3C fillParameter, IntPtr container)
        {
            FillValue3C value = _pageData[fillParameter.Key] as FillValue3C;
            if (value == null || string.IsNullOrEmpty(value.Value))
                return false;
            string[] values = value.Value.Split(fillParameter.Separators[0]);
            List<string> valueList = new List<string>();
            foreach (var item in values)
            {
                valueList.AddRange(item.Split('|'));
            }
            return ApiSetter.CheckCheckBox(container, this, value.Note, valueList);
        }

        private bool FillValue(string className, IntPtr handle, FillValue3C value)
        {
            int index = -1;
            return FillValue(className, ref index, handle, value);
        }

        private bool FillValue(string className, ref int index , IntPtr handle, FillValue3C value)
        {
            _asyncHandle.Wait();
            string parameterValue = null;
            if (value == null)
                return false;
            FillParameter3C parameter = _fillParameters[value.Key] as FillParameter3C;
            if (index == -1)
                parameterValue = value.Value;
            else
            {
                string[] values = value.Value == null ? null : value.Value.Split(parameter.Separators[0]);
                parameterValue = (values == null || values.Length <= index) ? 
                    (string.IsNullOrEmpty(value.Note) ? "" : value.Note) : values[index];
            }
            StringBuilder text = null;
            switch(className)
            {
            case CCCFillManager.EditClassName:
                if (ApiSetter.IsEditable(handle, _handle) == false)
                {
                    index--;
                    break;
                }
                ApiSetter.SetText(handle, parameterValue);
                break;
            case CCCFillManager.ComboBoxClassName:
                if (ApiSetter.IsEditable(handle, _handle) == false)
                {
                    index--;
                    break;
                }
                ApiSetter.SetComboBoxSelected(_handle, handle, parameterValue, parameter.IsComboBoxPreciseMatch);
                break;
            case CCCFillManager.ButtonClassName:
                value.Separators = parameter.Separators;
                text = new StringBuilder(256);
                NativeApi.GetWindowText(handle, text, 255);
                if (text.ToString() == "附件" && value != null && string.IsNullOrEmpty(value.AttachFile) == false)
                    return ApiSetter.ClickButton(handle, _handle, ListenAttachWindow, value);
                else if (text.ToString() == "编辑")
                {
                    if (string.IsNullOrEmpty(value.OriginString))
                        return false;
                    if (string.IsNullOrEmpty(parameter.EditType))
                        parameter.EditType = "add";
                    switch (parameter.EditType.ToLower())
                    {
                        case "add":
                            return ApiSetter.ClickButton(handle, _handle, ListenAddWindow, value);
                        case "radio":
                            return ApiSetter.ClickButton(handle, _handle, ListenRadioWindow, value);
                        case "select":
                            return ApiSetter.ClickButton(handle, _handle, ListenListBoxWindow, value);
                        case "multiselect":
                            value.DoubleClick = true;
                            return ApiSetter.ClickButton(handle, _handle, ListenListBoxWindow, value);
                        case "text":
                            return ApiSetter.ClickButton(handle, _handle, ListenTextWindow, value);
                        case "column":
                            return ApiSetter.ClickButton(handle, _handle, ListenColumnWindow, value);
                        case "multinote":
                            return ApiSetter.ClickButton(handle, _handle, ListenMultiValueAndNoteWindow, value);
                        default:
                            return false;
                    }
                }
                else
                {
                }
                break;
            }
            return true;
        }
        /// <summary>
        /// 监听可追加类型窗口。
        /// </summary>
        /// <param name="value"></param>
        private void ListenAddWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.AddWindow, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
        /// <summary>
        /// 监听附件窗口
        /// </summary>
        /// <param name="value"></param>
        internal void ListenAttachWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.AttachWindow, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
        /// <summary>
        /// 监听输入文件名窗口。
        /// </summary>
        /// <param name="value"></param>
        private void ListenInputFileNameWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.InputFileNameWindow, this, processId);
            fill.DoFillWork(_pageData["文件名"]);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
        /// <summary>
        /// 监听Radio类型窗口。
        /// </summary>
        /// <param name="value"></param>
        private void ListenRadioWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.RadioWindow, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
        /// <summary>
        /// 监听Select类型窗口。
        /// </summary>
        /// <param name="value"></param>
        private void ListenListBoxWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.ListCheckBoxWindow, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
        /// <summary>
        /// 监听Text类型窗口。
        /// </summary>
        /// <param name="value"></param>
        private void ListenTextWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.TextWindow, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
        /// <summary>
        /// 监听Column类型窗口。
        /// </summary>
        /// <param name="value"></param>
        private void ListenColumnWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.ColumnWindow, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }

        private void ListenMultiValueAndNoteWindow(object value)
        {
            _asyncHandle.Reset();
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.MultiValueAndNote, this, processId);
            fill.DoFillWork(value);
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }

        public void Reset()
        {
            if(_asyncHandle != null)
                _asyncHandle.Reset();
        }

        public void Wait()
        {
            if (_asyncHandle != null)
            {
                _state = FillState.Running;
                _asyncHandle.Wait();
                _state = FillState.Waiting;
            }
        }

        public void Resume()
        {
            _selectedNode = _treeView.GetSelected();
            if (_asyncHandle != null)
                _asyncHandle.Resume();
        }
    }
}
