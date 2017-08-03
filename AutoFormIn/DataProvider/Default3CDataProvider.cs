using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace Assistant.DataProviders
{
    /// <summary>
    /// CCC填报管理器的默认数据提供类。
    /// </summary>
    public class Default3CDataProvider : IDataProvider
    {
        internal static string FillRuleVerson = "填报规则", FillType = "CCC";
        internal static readonly Regex suffixMatcher = new Regex(@"^C?\.?[0-9]+(\.[0-9]+)*_(?<suffix>.*)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        internal static readonly Regex paraNameMatcher = new Regex("(?<name>[^_]*)_.*");

        private Hashtable files, fillParameters;

        public string DataSourceFile
        {
            get;
            set;
        }
        /// <summary>
        /// 使用此提供类获取填报数据。
        /// </summary>
        /// <param name="state">存储非树节点参数的Hashtable;</param>
        /// <returns></returns>
        public object ProvideData(object state)
        {
            this.GetAllFiles();
            object[] parameters = state as object[];
            fillParameters = parameters[1] as Hashtable;
            Hashtable data = parameters[0] as Hashtable;
            return ReadData(this.DataSourceFile, data, fillParameters);
        }

        public bool AllowAlternately
        {
            get { return false; }
        }

        public bool ShowWindow()
        {
            throw new NotImplementedException();
        }

        public void Clean()
        {
        }

        public Default3CDataProvider()
        {
            files = new Hashtable();
        }

        private void AddFillValue(FillValue3C fillValue, TreeValue value, Hashtable set)
        {
            string[] values = string.IsNullOrEmpty(fillValue.Value) ? new string[] { fillValue.Value } : fillValue.Value.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            TreeValue temp = null;
            if (values.Length == 1 || value == null || value.Parent == null)
            {
                fillValue.SetValue(values[0]);
                InitFilesFullName(fillValue);
                set.Add(fillValue.Key, fillValue);
            }
            else
            {
                int appendIndex = 1000;
                for (int i = 0; i < values.Length; i++)
                {
                    if (i == 0)
                        temp = value;
                    else
                        temp = value.Parent.FindChild(value.Name, string.Format("{0}{1}", value.Suffix, appendIndex));
                    if (temp == null)
                    {
                        temp = new TreeValue(value.Name, string.Format("{0}{1}", value.Suffix, appendIndex));
                        temp.CopyFrom(value);
                        value.Parent.AddChild(temp);
                    }
                    FillValue3C tempValue = new FillValue3C();
                    tempValue.CopyFrom(fillValue);
                    tempValue.SetValue(values[i]);
                    InitFilesFullName(tempValue);
                    temp.Values.Add(tempValue.Key, tempValue);
                    appendIndex++;
                }
            }
            if (value.Parent == null)
                return;
            // 设置新增节点默认值
            int index = 1001;
            temp = value.Parent.FindChild(value.Name, string.Format("{0}{1}", value.Suffix, index));
            while (temp != null)
            {
                foreach (DictionaryEntry entry in value.Values)
                {
                    object val = temp.Values[entry.Key];
                    if (val == null)
                    {
                        temp.Values[entry.Key] = entry.Value;
                    }
                }
                index++;
                temp = value.Parent.FindChild(value.Name, string.Format("{0}{1}", value.Suffix, index));
            }
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
                else if (index >= current.Children.Count - 1)
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
        /// 从指定文件中读取需填报的参数值。
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private TreeValue ReadData(string fileName, Hashtable data, Hashtable fillParameters)
        {
            try
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
                                        fillValue.SetValue(string.Format("{0}{1}", string.IsNullOrEmpty(fillValue.Value) ? "" : string.Format("{0}\t", fillValue.Value), str));
                                        break;
                                    case "参数项名称":
                                        Match match = paraNameMatcher.Match(str);
                                        if (match.Success && match.Groups["name"].Success)
                                            str = match.Groups["name"].Value.Trim();
                                        else
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
                                            match = suffixMatcher.Match(fillValue.Key);
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
                                        fillValue.PublicAttachFile = str;
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
                                AddFillValue(fillValue, lastTreeNode, current == null ? data : current);
                                //if (current == null)                                    //  若当前值为树节点下的值则将其添加到当前树节点的Values中，
                                //    _data.Add(fillValue.Key, fillValue);        //  否则（如证书名称、厂商关系）添加到管理器的数据中。
                                //else
                                //    current.Add(fillValue.Key, fillValue);
                            }
                        }
                    }
                }
                return root;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.StackTrace + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 从Excel文件中读取树节点的父子级关系。
        /// </summary>
        /// <returns></returns>
        private TreeValue InitTreeNode()
        {
            TreeValue value = null, parent = null, root = null;
            string fileName = FileHelper.GetPublicPage(FillRuleVerson, FillType);
            string ruleFilePath = FileHelper.GetFillVersionByName(FillRuleVerson);
            Hashtable table = new Hashtable();
            using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(string.Format(@"{0}\{1}", ruleFilePath, fileName)))
            {
                reader.Open();
                Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
                sheet.ReadFollowingRow(2);
                string name = null, parentName = null;
                object content = null;
                do
                {
                    while (sheet.ReadNextCell(false))
                    {
                        content = sheet.GetContent();
                        switch (sheet.CurrentCell.ColumnIndex)
                        {
                            case 1:
                                name = content == null ? null : content.ToString();
                                break;
                            case 2:
                                parentName = content == null ? null : content.ToString();
                                break;
                        }
                    }
                    if (name == null)
                        continue;
                    value = new TreeValue(name, parentName, null);
                    table.Add(name, value);
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
        /// <summary>
        /// 使用用户输入的文件名初始化文件的绝对路径。
        /// </summary>
        /// <param name="fillValue"></param>
        private void InitFilesFullName(FillValue3C fillValue)
        {
            FillParameter3C fillParameter = fillParameters[fillValue.Key] as FillParameter3C;
            if (fillParameter == null)
                fillValue.Separators = FillParameter3C.DefaultSeparators;
            else
                fillValue.Separators = fillParameter.Separators;

            if (fillValue == null || fillValue.AttachFile == null || fillValue.Separators == null || fillValue.Separators.Length < 1)
                return;
            string[] fileNames = fillValue.AttachFile.Split(fillValue.Separators[0]);
            StringBuilder buffer = new StringBuilder();
            foreach (string fileName in fileNames)
            {
                string file = GetRealFile(fileName);
                buffer.Append(file);
                buffer.Append(fillValue.Separators[0]);
            }
            if (buffer.Length > 1)
                buffer.Remove(buffer.Length - 1, 1);

            fillValue.SetAttachFile(buffer.ToString());
        }

        private void GetAllFiles()
        {
            string directory = Path.GetDirectoryName(this.DataSourceFile);
            GetAllFilesFromDirectory(directory);
        }

        private void GetAllFilesFromDirectory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            List<string> fileList = null;
            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (this.files.ContainsKey(name))
                    fileList = this.files[name] as List<string>;
                else
                {
                    fileList = new List<string>();
                    this.files.Add(name, fileList);
                }
                fileList.Add(file);
            }
            //string[] directories = Directory.GetDirectories(directory);

            //foreach (var dir in directories)
            //{
            //    GetAllFilesFromDirectory(dir);
            //}
        }

        public string GetRealFile(string fileName)
        {
            // 若参数值中没有附件信息，则解压zip文件，并将所有文件添加到文件列表中
            string name = Path.GetFileNameWithoutExtension(fileName);
            List<string> list = name == null ? null : files[name] as List<string>;
            if (list == null)
                return null;
            foreach (var item in list)
            {
                if (Path.GetFileName(item) == fileName)
                    return item;
            }
            return null;
        }

        public ValueConverter GetConverter()
        {
            return new Default3CValueConverter();
        }

        public bool CanValidation
        {
            get { return false; }
        }

        public bool Validate()
        {
            return true;
        }
    }
}
