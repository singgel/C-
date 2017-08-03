using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.IO;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 附件窗口。
    /// </summary>
    public class AttachWindow_3C : FillDialog_3C
    {
        private IntPtr saveButton, addButton;

        public AttachWindow_3C(IntPtr hwnd)
            :base(hwnd)
        {
            saveButton = IntPtr.Zero;
            addButton = IntPtr.Zero;
        }

        public override bool DoFillWork(object state)
        {
            FillValue3C fillValue = state as FillValue3C;
            if (fillValue == null || fillValue.AttachFile == null || fillValue.Separators == null || fillValue.Separators.Length < 1)
                return false;
            string[] fileNames = fillValue.AttachFile.Split(fillValue.Separators[0]);
            //bool flag = fillValue.AttachFile == fillValue.PublicAttachFile;
            foreach (string fileName in fileNames)
            {
                //string fullName = (base.Owner == null || string.IsNullOrEmpty(base.Owner.DataFilePath)) ? 
                //    fileName : string.Format("{0}\\{1}", base.Owner.DataFilePath, fileName);
                //List<string> files = CheckFile(fileName, base.Owner.DataFilePath, fillValue.PublicAttachFile, flag);
                //if (files != null)
                //{
                //    foreach (var file in files)
                //    {
                string file = fileName;

                if (Path.GetFileName(fileName) == fileName)
                    file = string.Format("{0}\\{1}", base.Owner.DataFilePath, fileName);

                if (File.Exists(file))
                    ApiSetter.ClickButton(addButton, base.HWnd, FillOpenFileDialog, file); // 点击添加按钮
                else
                    this.Owner.Records.Add(new FillRecord(ElementType.Unknown, RecordType.Failed, string.Format("文件 {0} 不存在", file), null));
                //    }
                //}
                //else
                //    this.Owner.Records.Add(new FillRecord(ElementType.Unknown, RecordType.Failed, string.Format("文件 {0} 不存在", fullName), null));
            }
            ApiSetter.ClickButton(saveButton, base.HWnd, null, null); // 点击保存按钮
            return true;
        }

        public override void InitHandle()
        {
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(base.HWnd, (hwnd, lParam) =>
            {
                NativeApi.GetClassName(hwnd, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                {
                    StringBuilder text = className;
                    text.Clear();
                    NativeApi.GetWindowText(hwnd, text, 255);
                    string textStr = text.ToString();
                    if (textStr == "保存")
                        saveButton = hwnd;
                    else if (textStr == "添加")
                        addButton = hwnd;
                }
                return saveButton == IntPtr.Zero || addButton == IntPtr.Zero;
            }, IntPtr.Zero);
        }

        private void FillOpenFileDialog(object fileName)
        {
            uint processId;
            NativeApi.GetWindowThreadProcessId(addButton, out processId);
            FillDialog_3C fill = FillDialog_3C.GetFillDialog(CCCWindowType.OpenFileWindow, processId);
            if (fill != null)
            {
                fill.DoFillWork(fileName);
            }
        }

        public override bool IsValidWindow()
        {
            return base.Title == "编辑附件";
        }

        //public List<string> CheckFile(string fileName, string path, string publicAttachFile, bool usePublicAttachFile)
        //{
        //    List<string> list = null;

        //    if (usePublicAttachFile)
        //    {
        //        // 若参数值中没有附件信息，则解压zip文件，并将所有文件添加到文件列表中
        //        list = new List<string>();
        //        string name = Path.GetFileNameWithoutExtension(publicAttachFile);
        //        list = base.Owner.FileTable[name] as List<string>;
        //        if (list == null)
        //            return list;
        //        else
        //        {
        //            List<string> result = new List<string>();
        //            foreach (var zipFile in list)
        //            {
        //                if (Path.GetExtension(zipFile) == ".zip")
        //                {
        //                    string temp = Path.GetTempFileName();
        //                    File.Delete(temp);
        //                    Directory.CreateDirectory(temp);
        //                    ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
        //                    zip.ExtractZip(zipFile, temp, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, "", "", false);
        //                    GetAllFilesFromDirectory(temp, result);
        //                }
        //                else
        //                    result.Add(zipFile);
        //            }
        //            list = result;
        //        }
        //    }
        //    else
        //    {
        //        string name = Path.GetFileNameWithoutExtension(fileName);
        //        list = name == null ? null : base.Owner.FileTable[name] as List<string>;
        //    }
        //    return list;
        //    //string fullname = string.Format("{0}\\{1}.zip", path, publicAttachFile);
        //    //if (File.Exists(fullname))
        //    //{
        //    //    try
        //    //    {
        //    //        ICSharpCode.SharpZipLib.Zip.FastZip zip = new ICSharpCode.SharpZipLib.Zip.FastZip();
        //    //        zip.ExtractZip(fullname, path, ICSharpCode.SharpZipLib.Zip.FastZip.Overwrite.Always, null, "", "", false);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        this.Owner.Records.Add(new FillRecord(ElementType.Unknown, RecordType.Failed, string.Format("文件 {0} 解压失败，原因为：{1}\n{2}", fullname, ex.Message, ex.StackTrace), null));
        //    //    }
        //    //}
        //    //foreach (var ext in extensions)
        //    //{
        //    //    fullname = string.Format("{0}\\{1}.{2}", path, fileName, ext);
        //    //    if (File.Exists(fullname))
        //    //        return fullname;
        //    //}
        //    //this.Owner.Records.Add(new FillRecord(ElementType.Unknown, RecordType.Failed, string.Format("文件 {0} 不存在！", fullname), null));
        //    //return null;
        //}

        //private void GetAllFilesFromDirectory(string directory, List<string> list)
        //{
        //    string[] files = Directory.GetFiles(directory);
        //    foreach (var file in files)
        //    {
        //        list.Add(file);
        //    }
        //}
    }
}
