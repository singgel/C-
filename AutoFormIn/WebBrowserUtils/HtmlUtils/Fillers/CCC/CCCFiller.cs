using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.Collections;
using System.Text.RegularExpressions;

namespace WebBrowserUtils.HtmlUtils.Fillers.CCC
{
    // 1B0AAE  
    public class CCCFiller
    {
        public static int ProcessId;
        private SysTreeView treeView;
        private Main_3C main;
        private Hashtable _data, _fillParameters;
        private IntPtr container;
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

        public string DataFile
        {
            get;
            set;
        }

        public CCCFiller(IntPtr container)
        {
            treeView = null;
            _data = new Hashtable();
            //this.ReadData(@"..\..\..\演示数据\3cData.xlsx");
            //_fillParameters = this.ReadFillParameter(@"..\..\..\演示数据\CCC\CCC填报规则.xlsx");
            this.container = container;
        }

        public void BeginFill()
        {
            if (main != null)
            {
                if (main.State == FillState.Suspended || main.State == FillState.Waiting)
                    main.Resume();
                else
                    return;
            }
            if (_fillThread == null)
            {
                _fillThread = new System.Threading.Thread(FillWorker);
                _fillThread.Start();
            }
        }

        private void FillWorker(object state)
        {
            FillDialog_3C.BeginListen();
            string windowType = null;
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(out windowType);
            try
            {
                _fillParameters = this.ReadFillParameter(@"..\..\..\演示数据\CCC\CCC填报规则.xlsx");
                TreeValue root = this.ReadData(DataFile);
                FillDialog_3C.BeginListen();
                windowType = null;
                fill = FillDialog_3C.GetFillDialog(out windowType);
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

                main = Main_3C.GetMainWindow(this.DataFile);
                //this.SetWindowPos();
                main.FillParameters = this._fillParameters;
                this.GetTreeView();
                main.TreeValue = root;
                while (main.SelectNextNode(true))
                {
                    main.FillPage();
                }
            }
            catch
            {
                this.EndFill();
            }
        }

        public void GenerateDir()
        {
            if (main == null)
                main = Main_3C.GetMainWindow("");
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

        public void EndFill()
        {
            try
            {
                NativeApi.SetWindowPos(main.HWnd, new IntPtr(1), 0, 0, 0, 0, 0x3);
                main.Dispose();
                _fillThread.Abort();
            }
            catch
            {
            }
            finally
            {
                _fillThread = null;
                main = null;
                _data.Clear();
                container = IntPtr.Zero;
                FillDialog_3C.EndListen();
            }
        }

        private void GetTreeView()
        {
            treeView = main.TreeView;
            FillDialog_3C dialog = null;
            dialog = FillDialog_3C.GetFillDialog(CCCWindowType.PropertyWindow, main.HWnd);
            string category = _data["车辆类别"] == null ? "" : (_data["车辆类别"] as FillValue3C).Value;
            string property = _data["车辆属性"] == null ? "" : (_data["车辆属性"] as FillValue3C).Value;
            dialog.DoFillWork(new string[] { category, property });
            dialog = FillDialog_3C.GetFillDialog(CCCWindowType.InputFileNameWindow, main.HWnd);
            dialog.DoFillWork(new FillValue3C() { Value = Guid.NewGuid().ToString() });
            uint count = 0;
            while (count == 0)
            {
                count = treeView.GetCount();
                System.Threading.Thread.Sleep(500);
            }
        }

        public void SetWindowPos()
        {
            if(main == null || container == IntPtr.Zero)
                return;
            RECT rect;
            NativeApi.GetWindowRect(container, out rect);
            if(_fillThread != null && _fillThread.IsAlive)
                NativeApi.SetWindowPos(main.HWnd, IntPtr.Zero, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 0x40);
            else
                NativeApi.SetWindowPos(main.HWnd, new IntPtr(1), 0, 0, 0, 0, 0x3);
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
        /// <summary>
        /// 从指定文件中读取需填报的参数值。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private TreeValue ReadData(string fileName)
        {
            TreeValue root = InitTreeNode();
            Hashtable treeDir = GetTreeDir(root);
            Hashtable current = null;
            TreeValue lastTreeNode = root;
            Hashtable columnHeader = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(fileName))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
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
                    FillValue3C fillValue = null;
                    object content = null;
                    string str = null;
                    bool nextRow = false;
                    while (sheet.ReadNextRow())
                    {
                        fillValue = new FillValue3C();
                        while (sheet.ReadNextCell(false))
                        {
                            content = sheet.GetContent();
                            str = content == null ? "" : content.ToString();
                            switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                            {
                            case "序号":
                                fillValue.Key = str;
                                break;
                            case "参数项的值":
                                fillValue.SetValue(str);
                                break;
                            case "参数项名称":
                                str = str.Trim();
                                if (treeDir.ContainsKey(str))
                                {
                                    TreeValue parent = lastTreeNode;
                                    TreeValue child = null;
                                    while (parent != null)
                                    {
                                        child = parent.FindChild(str, null);
                                        if (child == null)
                                            parent = parent.Parent;
                                        else
                                            break;
                                    }
                                    if (child == null)
                                    {
                                        child = treeDir[str] as TreeValue;
                                        parent = child.Parent;
                                    }
                                    System.Diagnostics.Trace.Assert(child != null && parent != null);
                                    Match match = Main_3C.suffixMatcher.Match(fillValue.Key);
                                    if (match.Success)
                                    {
                                        string suffix = match.Groups["suffix"].Value;
                                        TreeValue temp = new TreeValue(child.Name, suffix);
                                        temp.CopyFrom(child);
                                        parent.AddChild(temp);
                                        child = temp;
                                    }
                                    current = child.Values;
                                    lastTreeNode = child;
                                    nextRow = true;  // 当前行为目录行，忽略其它内容
                                }
                                break;
                            case "附件":
                                fillValue.AttachFile = str;
                                break;
                            }
                            if (nextRow)
                                break;
                        }
                        if (string.IsNullOrEmpty(fillValue.Key) == false)
                        {
                            if (nextRow)
                            {
                                nextRow = false;
                                continue;
                            }
                            if (current == null)
                                _data.Add(fillValue.Key, fillValue);
                            else
                                current.Add(fillValue.Key, fillValue);
                        }
                    }
                }
            }
            return root;
        }
        /// <summary>
        /// 从树节点生成一个以其Name为键值的散列表。
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private Hashtable GetTreeDir(TreeValue root)
        {
            Hashtable table = new Hashtable();
            Stack<KeyValuePair<TreeValue, int>> stack = new Stack<KeyValuePair<TreeValue, int>>();
            table.Add(root.Name, root);
            TreeValue current = root;
            for (int index = 0; index < current.Children.Count; index++)
            {
                table.Add(current.Children[index].Name, current.Children[index]);
                if (current.Children[index].Children.Count > 0)
                {
                    stack.Push(new KeyValuePair<TreeValue, int>(current, index));
                    current = current.Children[index];
                    index = -1;
                    continue;
                }
                else if(index >= current.Children.Count - 1)
                {
                    while (stack.Count > 0)
                    {
                        KeyValuePair<TreeValue, int> lastPush = stack.Pop();
                        current = lastPush.Key;
                        index = lastPush.Value;
                        if (index >= current.Children.Count - 1)
                            continue;
                        else
                            break;
                    }
                }
            }
            return table;
        }
        /// <summary>
        /// 从Excel文件中读取树节点的父子级关系。
        /// </summary>
        /// <returns></returns>
        private TreeValue InitTreeNode()
        {
            TreeValue value = null, parent = null, root = null;
            Hashtable table = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(@"..\..\..\演示数据\CCC\目录字典.xlsx"))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
                sheet.ReadFollowingRow(2);
                string id = null, name = null, parentId = null;
                object content = null;
                do
                {
                    while (sheet.ReadNextCell(false))
                    {
                        content = sheet.GetContent();
                        switch (sheet.CurrentCell.ColumnIndex)
                        {
                        case 1:
                            id = content == null ? null : content.ToString();
                            break;
                        case 2:
                            name = content == null ? null : content.ToString();
                            break;
                        case 3:
                            parentId = content == null ? null : content.ToString();
                            break;
                        }
                    }
                    if (id == null || name == null)
                        continue;
                    value = new TreeValue(name, parentId, null);
                    table.Add(id, value);
                } while (sheet.ReadNextRow());
            }
            foreach (DictionaryEntry entry in table)
            {
                value = entry.Value as TreeValue;
                if (value != null)
                {
                    if (value.Parent == null)
                        root = value;
                    else
                    {
                        parent = table[value.Parent.Name] as TreeValue;
                        parent.AddChild(value);
                    }
                }
            }
            return root;
        }

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
    }
}
