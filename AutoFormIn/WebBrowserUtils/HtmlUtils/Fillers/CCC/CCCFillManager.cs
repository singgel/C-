using Assistant.DataProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    // 1B0AAE  
    public class CCCFillManager : FillManagerBase
    {
        private uint processId;
        private SysTreeView treeView;
        private List<FillRecord> _records;
        private Main_3C main;
        private Hashtable _data, _fillParameters;
        private System.Threading.Thread _fillThread;
        public const string ContainerClassName = "WindowsForms10.Window";
        public const string LISTBOXClassName = "WindowsForms10.LISTBOX";
        public const string StaticClassName = "WindowsForms10.STATIC";
        public const string EditClassName = "WindowsForms10.EDIT";
        public const string ComboBoxClassName = "WindowsForms10.COMBOBOX";
        public const string ButtonClassName = "WindowsForms10.BUTTON";
        public const string TreeViewClassName = "WindowsForms10.SysTreeView32";

        public bool IsRunning
        {
            get { return _fillThread != null && _fillThread.IsAlive; }
        }

        public int ProcessId
        {
            get { return (int)processId; }
        }

        public Main_3C Main
        {
            get { return main; }
        }
        ///// <summary>
        ///// 获取数据文件所在目录下的文件名与路径列表对应关系。
        ///// </summary>
        //public Hashtable Files
        //{
        //    get { return files; }
        //}

        public override IList<FillRecord> FillRecords
        {
            get { return _records; }
        }

        public CCCFillManager(uint processId, string dataFile)
            : this(processId, dataFile, FileHelper.GetFillVersionByName(WebBrowserUtils.Properties.Resources.FillRule))
        {
        }
        /// <summary>
        /// 初始化3C填报管理器。
        /// </summary>
        /// <param name="processId">CCC进程Id。</param>
        /// <param name="dataFile">数据文件路径。</param>
        /// <param name="ruleFilePath">填报规则所在文件夹。</param>
        private CCCFillManager(uint processId, string dataFile, string ruleFilePath)
            :base(dataFile, ruleFilePath)
        {
            base.FillType = "CCC";
            treeView = null;
            _data = new Hashtable();
            this.processId = processId;
            _records = new List<FillRecord>();
            _records.Add(new FillRecord(ElementType.Unknown, RecordType.Success));
            this.DataProvider = new Default3CDataProvider();
        }

        public override void BeginFill()
        {
            string fileName = FileHelper.GetFillRuleFile(Properties.Resources.FillRule, this.FillType, null, null);
            fileName = string.Format(@"{0}\{1}", base.RuleFilePath, fileName);
            if (main != null)
            {
                if (main.State == FillState.Suspended || main.State == FillState.Waiting)
                {
                    _fillParameters = this.ReadFillParameter(fileName);
                    TreeValue root = ReadAndConvertData();
                    main.FillParameters = this._fillParameters;
                    main.TreeValue = root;
                    main.Resume();
                }
                else
                    return;
            }
            if (_fillThread == null)
            {
                _fillThread = new System.Threading.Thread(FillWorker);
                _fillThread.Start(fileName);
            }
        }

        private void FillWorker(object state)
        {
            string windowType = null;
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(out windowType, processId);
            try
            {
                // 存储数据文件所在目录下的所有文件（不包括子文件夹中的文件）
                _fillParameters = this.ReadFillParameter(state as string);
                TreeValue root = ReadAndConvertData();
                windowType = null;
                FillDialog_3C.BeginListen();
                fill = FillDialog_3C.GetFillDialog(out windowType, processId);
                if (fill != null)
                {
                    if (windowType == CCCWindowType.LoginWindow)
                    {
                        fill.FillValue = _data["厂商关系"] as FillValue3C;
                        fill.DoFillWork(_data["登录证书"]);
                    }
                    else if (windowType == CCCWindowType.FirmWindow)
                    {
                        FillValue3C value = _data["厂商关系"] as FillValue3C;
                        fill.DoFillWork(value == null ? "" : value.Value);
                    }
                }

                main = Main_3C.GetMainWindow(base.DataProvider.DataSourceFile, processId);
                //this.SetWindowPos();
                //main.FileTable = this.files;
                main.FillParameters = this._fillParameters;
                main.Records = this._records;
                if (main.TreeView.GetCount() == 0)
                    this.GetTreeView();
                else
                    main.UpdateSelectedNode();
                main.TreeValue = root;
                FillDialog_3C.BeginListenSaveRequire(main);
                do
                {
                    System.Threading.Thread.Sleep(200);
                    main.FillPage();
                } while (main.SelectNextNode(true));
                this.EndFill();
                OnFinished(EventArgs.Empty);
                WebFillManager.ShowMessageBox("填报完成！", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                if (main != null)
                {
                    LogHelper.Write(ex);
                    MessageBox.Show(ex.StackTrace + ex.Message);
                    this.EndFill();
                }
            }
        }

        private TreeValue ReadAndConvertData()
        {
            TreeValue root = (TreeValue)base.DataProvider.ProvideData(new object[] { _data, _fillParameters });
            // 从配置文件中获取是否进行数据转换
            if (FileHelper.GetIsUseConverter(Properties.Resources.FillRule, this.FillType))
            {
                // 获取数据转换器
                ValueConverter converter = this.DataProvider.GetConverter() as ValueConverter;
                converter.DataFilePath = string.IsNullOrEmpty(base.DataProvider.DataSourceFile) ? "" : System.IO.Directory.GetParent(base.DataProvider.DataSourceFile).FullName;
                string converterFile = FileHelper.GetConverterFile(this.FillType);
                root = converter.Convert(converterFile, this.FillType, root) as TreeValue;
            }
            return root;
        }
        /// <summary>
        /// 生成树结构
        /// </summary>
        public void GenerateDir()
        {
            if (main == null)
                main = Main_3C.GetMainWindow("", processId);
            if (main.TreeView.GetCount() == 0)
                return;
            TreeValue root = main.GetTreeStructure();
            using (Office.Excel.ForwardExcelWriter writer = new Office.Excel.ForwardExcelWriter("目录字典.xlsx"))
            {
                writer.Open();
                Office.Excel.ForwardWriteWorksheet sheet = writer.CreateWorksheet() as Office.Excel.ForwardWriteWorksheet;
                sheet.WriteNextRow();
                sheet.WriteTextCell(1, writer.AddSharedString("Id"));
                sheet.WriteTextCell(2, writer.AddSharedString("目录名称"));
                sheet.WriteTextCell(3, writer.AddSharedString("父级目录Id"));
                WriteTreeDir(sheet, root);
            }
        }

        public override void EndFill()
        {
            try
            {
                if(main != null)
                    main.Dispose();
                this.DataProvider.Clean();
                //if (_fillThread != null)
                //    _fillThread.Abort();
            }
            catch
            {
            }
            finally
            {
                _fillThread = null;
                main = null;
                _data.Clear();
                FillDialog_3C.EndListen();
            }
        }

        private void GetTreeView()
        {
            treeView = main.TreeView;
            FillDialog_3C dialog = null;
            dialog = FillDialog_3C.GetFillDialog(CCCWindowType.PropertyWindow, main, this.processId);
            string category = _data["车辆类别"] == null ? "" : (_data["车辆类别"] as FillValue3C).Value;
            string property = _data["车辆属性"] == null ? "" : (_data["车辆属性"] as FillValue3C).Value;
            dialog.DoFillWork(new string[] { category, property });
            dialog = FillDialog_3C.GetFillDialog(CCCWindowType.InputFileNameWindow, main, this.processId);
            dialog.DoFillWork(new FillValue3C() { Value = Guid.NewGuid().ToString() });
            uint count = 0;
            while (count == 0)
            {
                count = treeView.GetCount();
                System.Threading.Thread.Sleep(500);
            }
        }
        /// <summary>
        /// 读取填报规则。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private Hashtable ReadFillParameter(string fileName)
        {
            Hashtable columnHeader = new Hashtable();
            Hashtable fillParameters = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(fileName))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate("填报规则") as Office.Excel.ForwardReadWorksheet;
                if (sheet != null)
                {
                    object header = null;
                    if (sheet.ReadNextRow() && sheet.CurrentRowIndex == 1)
                    {
                        while (sheet.ReadNextCell(false))
                        {
                            header = sheet.GetContent();
                            columnHeader.Add(sheet.CurrentCell.ColumnIndex, header == null ? "" : header.ToString());
                        }
                    }
                    FillParameter3C fillParameter = null;
                    object content = null;
                    string str = null;
                    while (sheet.ReadNextRow())
                    {
                        fillParameter = new FillParameter3C();
                        while (sheet.ReadNextCell(false))
                        {
                            content = sheet.GetContent();
                            str = content == null ? "" : content.ToString();
                            switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                            {
                            case "参数编号":
                                fillParameter.Key = str;
                                break;
                            case "类型":
                                fillParameter.Type = str;
                                break;
                            case "编辑窗口类型":
                                fillParameter.EditType = str;
                                break;
                            case "下拉框选择方式":
                                fillParameter.IsComboBoxPreciseMatch = str == "包含";
                                break;
                            case "参数分割符":
                                fillParameter.SetSeparator(str);
                                break;
                            }
                        }
                        if(string.IsNullOrEmpty(fillParameter.Key) == false)
                            fillParameters.Add(fillParameter.Key, fillParameter);
                    }
                }
            }
            return fillParameters;
        }
        ///// <summary>
        ///// 从指定文件中读取需填报的参数值。
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //private TreeValue ReadData(string fileName)
        //{
        //    try
        //    {
        //        TreeValue root = InitTreeNode();
        //        Hashtable treeDir = GetTreeDir(root);
        //        Hashtable current = null;
        //        TreeValue lastTreeNode = root;
        //        Hashtable columnHeader = new Hashtable();
        //        using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(fileName))
        //        {
        //            reader.Open();
        //            Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
        //            if (sheet != null)
        //            {
        //                object header = null;
        //                if (sheet.ReadNextRow() && sheet.CurrentRowIndex == 1)
        //                {
        //                    while (sheet.ReadNextCell(false))
        //                    {
        //                        header = sheet.GetContent();
        //                        columnHeader.Add(sheet.CurrentCell.ColumnIndex, header == null ? "" : header.ToString());
        //                    }
        //                }
        //                FillValue3C fillValue = null;
        //                object content = null;
        //                string str = null;
        //                bool nextRow = false;
        //                while (sheet.ReadNextRow())
        //                {
        //                    fillValue = new FillValue3C();
        //                    while (sheet.ReadNextCell(false))
        //                    {
        //                        content = sheet.GetContent();
        //                        str = content == null ? "" : content.ToString();
        //                        switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
        //                        {
        //                            case "序号":
        //                                fillValue.Key = str;
        //                                break;
        //                            case "参数项的值":
        //                                fillValue.SetValue(string.Format("{0}{1}", string.IsNullOrEmpty(fillValue.Value) ? "" : string.Format("{0}\t", fillValue.Value), str));
        //                                break;
        //                            case "参数项名称":
        //                                Match match = paraNameMatcher.Match(str);
        //                                if (match.Success && match.Groups["name"].Success)
        //                                    str = match.Groups["name"].Value.Trim();
        //                                else
        //                                    str = str.Trim();
        //                                if (treeDir.ContainsKey(str))
        //                                {
        //                                    TreeValue parent = lastTreeNode;
        //                                    TreeValue child = null;
        //                                    while (parent != null)
        //                                    {
        //                                        child = parent.FindChild(str, null);
        //                                        if (child == null)
        //                                            parent = parent.Parent;
        //                                        else
        //                                            break;
        //                                    }
        //                                    if (child == null)
        //                                    {
        //                                        child = treeDir[str] as TreeValue;
        //                                        parent = child.Parent;
        //                                    }
        //                                    System.Diagnostics.Trace.Assert(child != null && parent != null);
        //                                    match = Main_3C.suffixMatcher.Match(fillValue.Key);
        //                                    if (match.Success)
        //                                    {
        //                                        string suffix = match.Groups["suffix"].Value;
        //                                        TreeValue temp = new TreeValue(child.Name, suffix);
        //                                        temp.CopyFrom(child);
        //                                        parent.AddChild(temp);
        //                                        child = temp;
        //                                    }
        //                                    current = child.Values;
        //                                    lastTreeNode = child;
        //                                    nextRow = true;  // 当前行为目录行，忽略其它内容
        //                                }
        //                                break;
        //                            case "附件":
        //                                fillValue.PublicAttachFile = str;
        //                                break;
        //                        }
        //                        if (nextRow)
        //                            break;
        //                    }
        //                    if (string.IsNullOrEmpty(fillValue.Key) == false)
        //                    {
        //                        if (nextRow)
        //                        {
        //                            nextRow = false;
        //                            continue;
        //                        }
        //                        AddFillValue(fillValue, lastTreeNode, current == null ? _data : current);
        //                        //if (current == null)                                    //  若当前值为树节点下的值则将其添加到当前树节点的Values中，
        //                        //    _data.Add(fillValue.Key, fillValue);        //  否则（如证书名称、厂商关系）添加到管理器的数据中。
        //                        //else
        //                        //    current.Add(fillValue.Key, fillValue);
        //                    }
        //                }
        //            }
        //        }
        //        return root;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.StackTrace + ex.Message);
        //        throw;
        //    }
        //}

        //private void AddFillValue(FillValue3C fillValue, TreeValue value, Hashtable set)
        //{
        //    string[] values = string.IsNullOrEmpty(fillValue.Value) ? new string[] { fillValue.Value } : fillValue.Value.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
        //    TreeValue temp = null;
        //    if (values.Length == 1 || value == null || value.Parent == null)
        //    {
        //        fillValue.SetValue(values[0]);
        //        set.Add(fillValue.Key, fillValue);
        //    }
        //    else
        //    {
        //        int appendIndex = 1000;
        //        for (int i = 0; i < values.Length; i++)
        //        {
        //            if (i == 0)
        //                temp = value;
        //            else
        //                temp = value.Parent.FindChild(value.Name, string.Format("{0}{1}", value.Suffix, appendIndex));
        //            if (temp == null)
        //            {
        //                temp = new TreeValue(value.Name, string.Format("{0}{1}", value.Suffix, appendIndex));
        //                temp.CopyFrom(value);
        //                value.Parent.AddChild(temp);
        //            }
        //            FillValue3C tempValue = new FillValue3C();
        //            tempValue.CopyFrom(fillValue);
        //            tempValue.SetValue(values[i]);
        //            temp.Values.Add(tempValue.Key, tempValue);
        //            appendIndex++;
        //        }
        //    }
        //    if (value.Parent == null)
        //        return;
        //    // 设置新增节点默认值
        //    int index = 1001;
        //    temp = value.Parent.FindChild(value.Name, string.Format("{0}{1}", value.Suffix, index));
        //    while (temp != null)
        //    {
        //        foreach (DictionaryEntry entry in value.Values)
        //        {
        //            object val = temp.Values[entry.Key];
        //            if (val == null)
        //            {
        //                temp.Values[entry.Key] = entry.Value;
        //            }
        //        }
        //        index++;
        //        temp = value.Parent.FindChild(value.Name, string.Format("{0}{1}", value.Suffix, index));
        //    }
        //}
        ///// <summary>
        ///// 从树节点生成一个以其Name为键值的散列表。
        ///// </summary>
        ///// <param name="root"></param>
        ///// <returns></returns>
        //private Hashtable GetTreeDir(TreeValue root)
        //{
        //    Hashtable table = new Hashtable();
        //    Stack<KeyValuePair<TreeValue, int>> stack = new Stack<KeyValuePair<TreeValue, int>>();
        //    table.Add(root.Name, root);
        //    TreeValue current = root;
        //    for (int index = 0; index < current.Children.Count; index++)
        //    {
        //        table.Add(current.Children[index].Name, current.Children[index]);
        //        if (current.Children[index].Children.Count > 0)
        //        {
        //            stack.Push(new KeyValuePair<TreeValue, int>(current, index));
        //            current = current.Children[index];
        //            index = -1;
        //            continue;
        //        }
        //        else if(index >= current.Children.Count - 1)
        //        {
        //            while (stack.Count > 0)
        //            {
        //                KeyValuePair<TreeValue, int> lastPush = stack.Pop();
        //                current = lastPush.Key;
        //                index = lastPush.Value;
        //                if (index >= current.Children.Count - 1)
        //                    continue;
        //                else
        //                    break;
        //            }
        //        }
        //    }
        //    return table;
        //}
        ///// <summary>
        ///// 从Excel文件中读取树节点的父子级关系。
        ///// </summary>
        ///// <returns></returns>
        //private TreeValue InitTreeNode()
        //{
        //    TreeValue value = null, parent = null, root = null;
        //    string fileName = AssistantUpdater.FileHelper.GetPublicPage(Properties.Resources.FillRule, this.FillType);
        //    Hashtable table = new Hashtable();
        //    using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(string.Format(@"{0}\{1}", base.RuleFilePath, fileName)))
        //    {
        //        reader.Open();
        //        Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
        //        sheet.ReadFollowingRow(2);
        //        string name = null, parentName = null;
        //        object content = null;
        //        do
        //        {
        //            while (sheet.ReadNextCell(false))
        //            {
        //                content = sheet.GetContent();
        //                switch (sheet.CurrentCell.ColumnIndex)
        //                {
        //                case 1:
        //                    name = content == null ? null : content.ToString();
        //                    break;
        //                case 2:
        //                    parentName = content == null ? null : content.ToString();
        //                    break;
        //                }
        //            }
        //            if (name == null)
        //                continue;
        //            value = new TreeValue(name, parentName, null);
        //            table.Add(name, value);
        //        } while (sheet.ReadNextRow());
        //    }
        //    foreach (DictionaryEntry entry in table)
        //    {
        //        value = entry.Value as TreeValue;
        //        if (value != null)
        //        {
        //            if (value.Parent == null)
        //                root = value;
        //            else
        //            {
        //                parent = table[value.Parent.Name] as TreeValue;
        //                parent.AddChild(value);
        //            }
        //        }
        //    }
        //    return root;
        //}

        //private void GetAllFiles()
        //{
        //    string directory = Path.GetDirectoryName(base.DataProvider.DataSourceFile);
        //    GetAllFilesFromDirectory(directory);
        //}

        //private void GetAllFilesFromDirectory(string directory)
        //{
        //    ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
        //    string[] files = Directory.GetFiles(directory);
        //    List<string> fileList = null;
        //    foreach (var file in files)
        //    {
        //        string extension = Path.GetExtension(file);
        //        if(extension == ".zip")
        //            zip.ExtractZip(file, directory, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, "", "", false);
        //    }
        //    files = Directory.GetFiles(directory);
        //    foreach (var file in files)
        //    {
        //        string name = Path.GetFileNameWithoutExtension(file);
        //        if (this.files.ContainsKey(name))
        //            fileList = this.files[name] as List<string>;
        //        else
        //        {
        //            fileList = new List<string>();
        //            this.files.Add(name, fileList);
        //        }
        //        fileList.Add(file);
        //    }
        //    //string[] directories = Directory.GetDirectories(directory);

        //    //foreach (var dir in directories)
        //    //{
        //    //    GetAllFilesFromDirectory(dir);
        //    //}
        //}

        internal static void WriteTreeDir(Office.Excel.ForwardWriteWorksheet sheet, TreeValue value)
        {
            sheet.WriteNextRow();
            Match match = Main_3C.nodeMatcher.Match(value.Name);
            if (match.Success)
                sheet.WriteTextCell(1, sheet.Owner.AddSharedString(match.Groups["no"].Value));
            else
                sheet.WriteTextCell(1, sheet.Owner.AddSharedString("#"));
            sheet.WriteTextCell(2, sheet.Owner.AddSharedString(value.Name));
            match = Main_3C.nodeMatcher.Match(value.Parent == null ? "" : value.Parent.Name);
            if (match.Success)
                sheet.WriteTextCell(3, sheet.Owner.AddSharedString(match.Groups["no"].Value));
            else if (value.Parent != null)
                sheet.WriteTextCell(3, sheet.Owner.AddSharedString("#"));
            foreach (var item in value.Children)
            {
                WriteTreeDir(sheet, item);
            }
        }

        protected override object GetData(object state)
        {
            throw new NotImplementedException();
            //return ReadData(base.DataProvider.DataSourceFile);
        }
    }
}
