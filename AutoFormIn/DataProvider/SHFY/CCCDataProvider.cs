using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace Assistant.DataProviders.SHFY
{
    /// <summary>
    /// 上海泛亚CCC填报管理器的数据提供程序。
    /// </summary>
    public class CCCCDataProvider : IDataProvider
    {
        private Hashtable files, fillParameters, tempDir;

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
            return ReadData(this.DataSourceFile, data);
        }

        public bool AllowAlternately
        {
            get { return false; }
        }

        public bool ShowWindow()
        {
            throw new NotImplementedException();
        }

        public CCCCDataProvider()
        {
            files = new Hashtable();
            tempDir = new Hashtable();
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
        private TreeValue ReadData(string fileName, Hashtable data)
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
                                        Match match = Default3CDataProvider.paraNameMatcher.Match(str);
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
                                            match = Default3CDataProvider.suffixMatcher.Match(fillValue.Key);
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
            string fileName = FileHelper.GetPublicPage(Default3CDataProvider.FillRuleVerson, Default3CDataProvider.FillType);
            string ruleFilePath = FileHelper.GetFillVersionByName(Default3CDataProvider.FillRuleVerson);
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

        private void WriteExcelRow(TreeValue value, ISheet source, ISheet dest, Hashtable valueColumns, int lastValueIndex)
        {
            foreach (DictionaryEntry entry in value.Values)
            {
                FillValue3C fillValue = entry.Value as FillValue3C;
                
            }
        }

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
            bool flag = fillValue.AttachFile == fillValue.PublicAttachFile;
            StringBuilder buffer = new StringBuilder();
            foreach (string fileName in fileNames)
            {
                List<string> files = CheckFile(fileName, flag);
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        buffer.Append(file);
                        buffer.Append(fillValue.Separators[0]);
                    }
                }
            }
            if (buffer.Length > 1)
                buffer.Remove(buffer.Length - 1, 1);
            fillValue.SetAttachFile(buffer.ToString());
        }

        private void GetAllFiles()
        {
            string directory = Path.GetDirectoryName(this.DataSourceFile);
            GetAllFilesFromDirectory(directory);
            foreach (DictionaryEntry entry in tempDir)
            {
                string dir = entry.Key as string;
                if(Directory.Exists(dir))
                    GetAllFilesFromDirectory(dir);
            }
        }

        private void GetAllFilesFromDirectory(string directory)
        {
            ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
            string[] files = Directory.GetFiles(directory);
            List<string> fileList = null;
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file);
                if (extension == ".zip")
                {
                    string temp = Path.GetTempFileName();
                    File.Delete(temp);
                    Directory.CreateDirectory(temp);
                    zip.ExtractZip(file, temp, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, "", "", false);
                    tempDir.Add(Path.GetFileName(file), temp);
                }
            }
            files = Directory.GetFiles(directory);
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
        }

        public List<string> CheckFile(string fileName, bool usePublicAttachFile)
        {
            List<string> list = null;

            if (usePublicAttachFile)
            {
                // 若参数值中没有附件信息，则解压zip文件，并将所有文件添加到文件列表中
                list = new List<string>();
                string name = Path.GetFileNameWithoutExtension(fileName);
                list = files[name] as List<string>;
                if (list == null)
                    return list;
                else
                {
                    List<string> result = new List<string>();
                    foreach (var zipFile in list)
                    {
                        if (Path.GetExtension(zipFile) == ".zip")
                        {
                            string temp = tempDir[Path.GetFileName(zipFile)] as string;
                            GetAllFilesFromDirectory(temp, result);
                            Directory.Delete(temp, true);
                        }
                        else
                            result.Add(zipFile);
                    }
                    list = result;
                }
            }
            else
            {
                string name = Path.GetFileNameWithoutExtension(fileName);
                list = name == null ? null : files[name] as List<string>;
            }
            return list;
        }

        private void GetAllFilesFromDirectory(string directory, List<string> list)
        {
            string path = Directory.GetParent(this.DataSourceFile).FullName;
            string[] files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                list.Add(string.Format("{0}\\{1}", path, Path.GetFileName(file)));
            }
        }

        public void Clean()
        {
            foreach (DictionaryEntry entry in tempDir)
            {
                string dir = entry.Key as string;
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
            }
        }

        DataProviders.ValueConverter IDataProvider.GetConverter()
        {
            return new Default3CValueConverter();
        }

        bool IDataProvider.CanValidation
        {
            get { return true; }
        }

        bool IDataProvider.Validate()
        {
            string path = FileHelper.GetFillVersionByName("填报规则");
            string validateFile = FileHelper.GetFillRuleFile("填报规则", "CCC", null, null);
            SHFYDataValidator validator = new SHFYDataValidator(string.Format("{0}\\{1}", path, validateFile));
            validator.ReadValidateRule();
            int needAddedColumn = 0, lastValueColumnIndex = 0;
            Hashtable specialParameters = FileHelper.Get3CSpecialParameter();
            CollectInfo(this.DataSourceFile, specialParameters, ref needAddedColumn, ref lastValueColumnIndex);
            return Validate(this.DataSourceFile, validator, specialParameters, needAddedColumn, lastValueColumnIndex);
        }

        private void CollectInfo(string fileName, Hashtable specialParameters, ref int needAddedColumn, ref int lastValueColumnIndex)
        {
            try
            {
                int valueColumnCount = 0;
                Hashtable columnHeader = new Hashtable();
                using (Office.Excel.ForwardExcelReader reader = new Office.Excel.ForwardExcelReader(fileName))
                {
                    reader.Open();
                    Office.Excel.ForwardReadWorksheet sheet = reader.Activate(1) as Office.Excel.ForwardReadWorksheet;
                    string str = null, key = "";
                    if (sheet != null)
                    {
                        object header = null;
                        if (sheet.ReadNextRow() && sheet.CurrentRowIndex == 1)
                        {
                            while (sheet.ReadNextCell(false))
                            {
                                header = sheet.GetContent();
                                str = header == null ? "" : header.ToString();
                                if (str == "参数项的值")
                                {
                                    lastValueColumnIndex = sheet.CurrentCell.ColumnIndex - 1;
                                    valueColumnCount++;
                                }
                                columnHeader.Add(sheet.CurrentCell.ColumnIndex, str);
                            }
                        }
                        object content = null;
                        string allValues = "";
                        while (sheet.ReadNextRow())
                        {
                            key = "";
                            allValues = "";
                            while (sheet.ReadNextCell(false))
                            {
                                content = sheet.GetContent();
                                str = content == null ? "" : content.ToString();
                                switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                                {
                                    case "序号":
                                        key = str;
                                        break;
                                    case "参数项的值":
                                        if (string.IsNullOrEmpty(str) == false)
                                            allValues = string.Format("{0}{1}", string.IsNullOrEmpty(allValues) ? "" : string.Format("{0},", allValues), str);
                                        break;
                                }
                            }
                            if (key != null && specialParameters.ContainsKey(key))
                            {
                                string[] buffer = null;
                                if (allValues.IndexOf(',') >= 0)
                                {
                                    buffer = allValues.Split(',');
                                }
                                else if (allValues.IndexOf('，') >= 0)
                                {
                                    buffer = allValues.Split('，');
                                }
                                else if (allValues.IndexOf(';') >= 0)
                                {
                                    buffer = allValues.Split(';');
                                }
                                else
                                {
                                    continue;
                                }
                                // 保存需要新增的最大列数
                                int len = buffer.Length - valueColumnCount;
                                needAddedColumn = Math.Max(len, needAddedColumn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.StackTrace + ex.Message);
                throw;
            }
        }
        /// <summary>
        /// 数据验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validator"></param>
        private bool Validate(string fileName, SHFYDataValidator validator, Hashtable specialParameters, int needAddedColumn, int lastValueColumnIndex)
        {
            bool result = true;
            TreeValue root = InitTreeNode();
            Hashtable treeDir = GetTreeDir(root);
            Hashtable columnHeader = new Hashtable();
            Hashtable startIndex = new Hashtable();
            string destFile = string.Format("{0}\\{1}_{2}.xlsx", Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName), DateTime.Now.ToString("MMddHHmmss"));
            File.Copy(fileName, destFile);
            XSSFWorkbook destWorkbook = null;
            using (FileStream destStream = new FileStream(destFile, FileMode.Open, FileAccess.Read))
            {
                destWorkbook = new XSSFWorkbook(destStream);
            }
            ISheet destSheet = destWorkbook.GetSheetAt(0);
            InsertColumn(lastValueColumnIndex, needAddedColumn, destSheet);

            IEnumerator rowEnumerator = destSheet.GetRowEnumerator();
            // 验证失败单元格格式
            ICellStyle abnormalStyle = CreateAbnormalStyle(destWorkbook);
            // 验证失败单元格同节点格式
            ICellStyle normalStyle = CreateNormalStyle(destWorkbook);
            if (rowEnumerator.MoveNext())
            {
                IRow destRow = destSheet.GetRow(0);
                for (int i = 0; i < needAddedColumn; i++)
                {
                    ICell cell = destRow.GetCell(lastValueColumnIndex + i + 1);
                    cell.SetCellValue(GetCellValue(destRow.GetCell(lastValueColumnIndex)));
                    CopyCell(destRow.GetCell(lastValueColumnIndex), cell, destWorkbook);
                }

                for (int columnIndex = destRow.FirstCellNum; columnIndex < destRow.LastCellNum - 1 && columnIndex <= lastValueColumnIndex + needAddedColumn; columnIndex++)
                {
                    ICell cell = destRow.GetCell(columnIndex);
                    columnHeader.Add(cell.ColumnIndex, cell == null ? "" : GetCellValue(cell));
                }
            }
            int lastDirIndex = -1, appendColumn = lastValueColumnIndex, currentBufferIndex = 0;
            string key = "", value = "";
            string[] buffer = null;
            List<int> columnIndexes = new List<int>();
            while (rowEnumerator.MoveNext())
            {
                IRow destRow = rowEnumerator.Current as IRow;
                IEnumerator cellEnumerator = destRow.GetEnumerator();
                for (int columnIndex = destRow.FirstCellNum; columnIndex < destRow.LastCellNum - 1 && columnIndex <= lastValueColumnIndex + needAddedColumn; columnIndex++)
                {
                    ICell destCell = destRow.GetCell(columnIndex);
                    if (destCell == null)
                        continue;
                    switch (columnHeader[columnIndex] as string)
                    {
                        case "序号":
                            key = GetCellValue(destCell);
                            break;
                        case "参数项的值":
                            value = GetCellValue(destCell);
                            int index = 0;
                            bool flag = false;
                            bool isTitle = false;
                            if (key != null && specialParameters.ContainsKey(key))
                            {
                                if (value.IndexOf(',') >= 0)
                                {
                                    buffer = value.Split(',');
                                }
                                else if (value.IndexOf('，') >= 0)
                                {
                                    buffer = value.Split('，');
                                }
                                else if (value.IndexOf(';') >= 0)
                                {
                                    buffer = value.Split(';');
                                }
                                //if (buffer == null)
                                //    buffer = value.Split(',');
                                if (buffer != null && currentBufferIndex < buffer.Length)
                                {
                                    columnIndexes.Add(destCell.ColumnIndex);
                                    destCell.SetCellValue(buffer[currentBufferIndex]);
                                    currentBufferIndex++;
                                }
                            }
                            flag = validator.IsValid(key, GetCellValue(destCell));
                            if (flag == false || validator.IsSpecial(key, GetCellValue(destCell)))
                            {
                                if (startIndex.Contains(destCell.ColumnIndex))
                                {
                                    index = (int)startIndex[destCell.ColumnIndex];
                                    isTitle = false;
                                }
                                else
                                {
                                    index = lastDirIndex;
                                    isTitle = true;
                                }
                                SetAbnormalCell(index, destCell.RowIndex, destCell.ColumnIndex, destSheet, abnormalStyle, normalStyle, isTitle);
                                startIndex[destCell.ColumnIndex] = destCell.RowIndex;
                            }
                            result = result && flag;
                            break;
                        case "参数项名称":
                            buffer = null;
                            string str = GetCellValue(destCell);
                            Match match = Default3CDataProvider.paraNameMatcher.Match(str);
                            if (match.Success && match.Groups["name"].Success)
                                str = match.Groups["name"].Value.Trim();
                            else
                                str = str.Trim();
                            if (treeDir.ContainsKey(str))
                            {
                                if (columnIndexes.Count > 1)
                                    CopyCell(lastDirIndex, destCell.RowIndex, columnIndexes, destSheet);
                                columnIndexes.Clear();
                                lastDirIndex = destCell.RowIndex;
                                foreach (DictionaryEntry entry in startIndex)
                                {
                                    SetAbnormalCell((int)entry.Value , lastDirIndex - 1, (int)entry.Key, destSheet, normalStyle, normalStyle, false);
                                }
                                startIndex.Clear();
                            }
                            currentBufferIndex = 0;
                            break;
                    }
                }
            }
            File.Delete(destFile);
            destFile = string.Format("{0}\\{3}_{1}_{2}.xlsx", Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName), DateTime.Now.ToString("MMddHHmmss"), result ? "已验证" : "验证失败");
            using (FileStream fileStream = new FileStream(destFile, FileMode.OpenOrCreate, FileAccess.Write))
            {
                destWorkbook.Write(fileStream);
            }
            return result;
        }

        private void InsertColumn(int afterColumn, int insertCount, ISheet destSheet)
        {
            if (insertCount <= 0)
                return;
            IEnumerator rowEnumerator = destSheet.GetRowEnumerator();
            {
                while (rowEnumerator.MoveNext())
                {
                    IRow row = rowEnumerator.Current as IRow;
                    for (int index = row.LastCellNum - 1; index > afterColumn && index > row.FirstCellNum; index--)
                    {
                        ICell cell = row.GetCell(index);
                        if(cell != null)
                            cell.CopyCellTo(index + insertCount);
                    }
                   
                    for (int index = 1; index <= insertCount; index++)
                    {
                        ICell source = row.GetCell(afterColumn);
                        ICell dest = row.GetCell(afterColumn + index);
                        if (dest != null)
                        {
                            dest.SetCellValue("");
                            if (row.RowNum == 0)
                            {
                                if (source != null)
                                    dest.SetCellValue(GetCellValue(source));
                            }
                            if (source != null)
                                CopyCell(source, dest, destSheet.Workbook);
                        }
                    }
                }
            }
        }

        private static void CopyCell(ICell sourceCell, ICell destCell, IWorkbook destWorkbook)
        {
            ICellStyle style = sourceCell.CellStyle;
            if (style != null)
            {
                ICellStyle newStyle = destWorkbook.CreateCellStyle();
                newStyle.CloneStyleFrom(style);
                destCell.CellStyle = newStyle;
            }
        }

        private static ICellStyle CreateAbnormalStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            XSSFFont ffont = (XSSFFont)workbook.CreateFont();
            ffont.Color = new NPOI.XSSF.UserModel.XSSFColor(System.Drawing.Color.Black).Indexed;
            style.SetFont(ffont);
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Red.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            return style;
        }

        private static ICellStyle CreateNormalStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            XSSFFont ffont = (XSSFFont)workbook.CreateFont();
            ffont.Color = new NPOI.XSSF.UserModel.XSSFColor(System.Drawing.Color.Black).Indexed;
            style.SetFont(ffont);
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Green.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderTop = BorderStyle.Thin;
            return style;
        }

        private static string GetCellValue(ICell cell)
        {
            switch (cell.CellType)
            {
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.String:
                    return cell.StringCellValue;
                default:
                    return "";
            }
        }

        private static void CopyCell(int startRowIndex, int endRowIndex, List<int> columnIndexes, ISheet destSheet)
        {
            IRow row = null;
            ICell sourceCell = null, destCell = null;
            for (int rowIndex = startRowIndex + 1; rowIndex < endRowIndex; rowIndex++)
            {
                row = destSheet.GetRow(rowIndex);
                if (row != null)
                {
                    for (int i = 1; i < columnIndexes.Count; i++)
                    {
                        sourceCell = row.GetCell(columnIndexes[i - 1]);
                        destCell = row.GetCell(columnIndexes[i]);
                        if (sourceCell != null && destCell != null && string.IsNullOrEmpty(GetCellValue(destCell)))
                        {
                            destCell.SetCellValue(GetCellValue(sourceCell));
                        }
                    }
                }
            }
        }

        private static void SetAbnormalCell(int startRowIndex, int endRowIndex, int columnIndex, ISheet sheet, ICellStyle abnormalStyle, ICellStyle normalStyle, bool isTitle)
        {
            if (isTitle)
            {
                IRow row = sheet.GetRow(startRowIndex);
                if (row != null)
                {
                    ICell cell = row.GetCell(columnIndex);
                    if (cell != null)
                        cell.CellStyle =  normalStyle;
                }
            }
            for (int index = startRowIndex + 1; index <= endRowIndex; index++)
            {
                IRow row = sheet.GetRow(index);
                if (row != null)
                {
                    ICell cell = row.GetCell(columnIndex);
                    if (cell != null)
                        cell.CellStyle = (index == endRowIndex ? abnormalStyle : normalStyle);
                }
            }
        }
    }
}
