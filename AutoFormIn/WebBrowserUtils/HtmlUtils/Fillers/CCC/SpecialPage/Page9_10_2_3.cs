using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.Collections;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    internal class Page9_10_2_3
    {
        private IntPtr hwnd, saveButton;
        private Office.Excel.ForwardReadWorksheet sheet;

        public Main_3C Main
        {
            get;
            private set;
        }

        private class FillData
        {
            public string SymbolIdentity, Position;
        }

        public Page9_10_2_3(IntPtr hwnd, Main_3C owner, Office.Excel.ForwardReadWorksheet sheet)
        {
            this.hwnd = hwnd;
            this.sheet = sheet;
            this.Main = owner;
        }

        private List<IntPtr> GetFillControls()
        {
            IntPtr child = NativeApi.FindWindowEx(hwnd, IntPtr.Zero, null, null);
            List<IntPtr> children = ControlSorter.SortChild(child);
            return ControlSorter.SortChild(children, 1, 1);
        }

        public bool FillPage()
        {
            Hashtable columnHeader = new Hashtable();
            object content;
            if (sheet.ReadNextRow())
            {
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    columnHeader.Add(sheet.CurrentCell.ColumnIndex, content == null ? "" : content.ToString());
                }
            }
            FillData data = new FillData();
            List<IntPtr> controls = GetFillControls();
            int index = 0;
            StringBuilder className = new StringBuilder(256);
            while (sheet.ReadNextRow())
            {
                int fillCount = 0;
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "用符号识别（选择（是、否、不适用））":
                        data.SymbolIdentity = content == null ? "不适用" : string.IsNullOrEmpty(content.ToString()) ? "不适用" : content.ToString();
                        break;
                    case "位置":
                        data.Position = content == null ? null : content.ToString();
                        break;
                    }
                }
                for (; index < controls.Count && fillCount < 2; index++)
                {
                    NativeApi.GetClassName(controls[index], className, 255);
                    if (className.ToString().StartsWith(CCCFillManager.ComboBoxClassName))
                    {
                        ApiSetter.SetComboBoxSelected(hwnd, controls[index], data.SymbolIdentity);
                        fillCount++;
                    }
                    else if (className.ToString().StartsWith(CCCFillManager.EditClassName))
                    {
                        ApiSetter.SetText(controls[index], data.Position);
                        fillCount++;
                    }
                    else if (saveButton != IntPtr.Zero && className.ToString().StartsWith(CCCFillManager.ButtonClassName))
                    {
                        className.Clear();
                        NativeApi.GetWindowText(controls[index], className, 255);
                        if (className.ToString() == "保存")
                            saveButton = controls[index];
                    }
                }
            }
            if (saveButton == IntPtr.Zero)
            {
                for (; index < controls.Count; index++)
                {
                    className.Clear();
                    NativeApi.GetWindowText(controls[index], className, 255);
                    if (className.ToString() == "保存")
                        saveButton = controls[index];
                }
            }
            this.Main.ClickSaveButton(saveButton);
            return true;
        }
    }
}
