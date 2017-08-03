using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    internal class NormalAddPage
    {
        private string addText;
        private IntPtr hwnd, editRange, add, save, cancel;
        private Office.Excel.ForwardReadWorksheet sheet;

        public IntPtr HWnd
        {
            get { return hwnd; }
        }

        public IntPtr EditRange
        {
            get { return editRange; }
        }

        public IntPtr Add
        {
            get { return add; }
        }

        public IntPtr Save
        {
            get { return save; }
        }

        public Main_3C Main
        {
            get;
            private set;
        }

        protected Office.Excel.ForwardReadWorksheet Sheet
        {
            get { return sheet; }
        }

        public NormalAddPage(IntPtr hwnd, Main_3C owner, Office.Excel.ForwardReadWorksheet sheet)
            : this(hwnd, owner, "添加一行", sheet)
        {
        }

        public NormalAddPage(IntPtr hwnd, Main_3C owner, string addText, Office.Excel.ForwardReadWorksheet sheet)
        {
            this.hwnd = hwnd;
            this.sheet = sheet;
            this.addText = addText;
            this.Main = owner;
        }

        public void InitHandle()
        {
            if (hwnd == IntPtr.Zero)
                return;
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(hwnd, (handle, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(handle, className, 255);
                string classNameStr = className.ToString();
                if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                {
                    StringBuilder text = className.Clear();
                    NativeApi.GetWindowText(handle, text, 255);
                    if (text.ToString() == addText)
                        add = handle;
                    else if (text.ToString() == "保存")
                        save = handle;
                }
                else if (classNameStr.StartsWith(CCCFillManager.ContainerClassName))
                {
                    StringBuilder text = className.Clear();
                    NativeApi.GetWindowText(handle, text, 255);
                    if (text.ToString() == "编辑区域")
                        editRange = handle;
                }
                return editRange == IntPtr.Zero || add == IntPtr.Zero || save == IntPtr.Zero;
            }, IntPtr.Zero);
        }

        public virtual bool FillPage()
        {
            if (editRange == IntPtr.Zero)
                return false;
            if (sheet.ReadFollowingRow(2))
            {
                bool isEmptyRow;
                Dictionary<int, string> values = new Dictionary<int, string>();
                IntPtr ok;
                do
                {
                    values.Clear();
                    object content;
                    isEmptyRow = true;
                    string str = "";
                    while (sheet.ReadNextCell(false))
                    {
                        content = sheet.GetContent();
                        str = content == null ? "" : content.ToString();
                        if (string.IsNullOrEmpty(str) == false)
                            isEmptyRow = false;
                        values.Add(sheet.CurrentCell.ColumnIndex - 1, content == null ? null : content.ToString());
                    }
                    if (isEmptyRow)
                        continue;
                    ApiSetter.ClickButton(add, hwnd, null, null);
                    ok = IntPtr.Zero;
                    List<IntPtr> list = ControlSorter.SortContainer(editRange);
                    StringBuilder className = new StringBuilder(256);
                    int index = 0;
                    foreach (var handle in list)
                    {
                        className.Clear();
                        NativeApi.GetClassName(handle, className, 255);
                        string classNameStr = className.ToString();
                        if (classNameStr.StartsWith(CCCFillManager.ComboBoxClassName))
                        {
                            if (values.ContainsKey(index) && ApiSetter.IsEditable(handle))
                            {
                                ApiSetter.SetComboBoxSelected(hwnd, handle, values[index]);
                                index++;
                            }
                        }
                        else if (classNameStr.StartsWith(CCCFillManager.EditClassName))
                        {
                            if (values.ContainsKey(index) && ApiSetter.IsEditable(handle))
                            {
                                ApiSetter.SetText(handle, values[index]);
                                index++;
                            }
                        }
                        else if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                        {
                            StringBuilder text = className.Clear();
                            NativeApi.GetWindowText(handle, text, 255);
                            if (text.ToString() == "确定")
                                ok = handle;
                            else if (text.ToString() == "附件" && values.ContainsKey(index) && string.IsNullOrEmpty(values[index]) == false)
                            {
                                ApiSetter.ClickButton(handle, hwnd, ListenAttachWindow,
                                    new FillValue3C() { PublicAttachFile = values[index], Separators = FillParameter3C.DefaultSeparators });
                            }
                        }
                    }
                    if (ok != IntPtr.Zero)
                        ApiSetter.ClickButton(ok, hwnd, null, null);
                } while (sheet.ReadNextRow());
                this.Main.ClickSaveButton(this.Save);
                return true;
            }
            return false;
        }

        private void ListenAttachWindow(object state)
        {
            AttachWindow_3C attachWindow = FillDialog_3C.GetFillDialog(CCCWindowType.AttachWindow, this.Main, this.Main.ProcessId) as AttachWindow_3C;
            if (attachWindow != null)
                attachWindow.DoFillWork(state);
        }
    }
}
