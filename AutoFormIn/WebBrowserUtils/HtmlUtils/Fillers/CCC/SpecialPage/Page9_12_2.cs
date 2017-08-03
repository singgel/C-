using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    class Page9_12_2 : NormalAddPage
    {
        private List<Page9_12_2Value> data;

        public Page9_12_2(IntPtr hwnd, Main_3C owner, Office.Excel.ForwardReadWorksheet sheet)
            : base(hwnd, owner, "添加一排", sheet)
        {
            data = new List<Page9_12_2Value>();
        }
        //第几排座椅	座椅位置	前气囊	侧气囊	安全带预加载装置
        private IntPtr GetFillControlList(out List<IntPtr> outList, out IntPtr cancel)
        {
            IntPtr ok = IntPtr.Zero;
            IntPtr container = IntPtr.Zero;
            IntPtr childAfter = IntPtr.Zero;
            cancel = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            do
            {
                childAfter = NativeApi.FindWindowEx(base.EditRange, childAfter, null, null);
                if (childAfter != IntPtr.Zero)
                {
                    long style = NativeApi.GetWindowLong(childAfter, -16);
                    if ((style & WindowStyles.WS_VISIBLE) != WindowStyles.WS_VISIBLE)
                        continue;
                    NativeApi.GetClassName(childAfter, className, 255);
                    string classNameStr = className.ToString();
                    if (classNameStr.StartsWith(CCCFillManager.ButtonClassName))
                    {
                        className.Clear();
                        NativeApi.GetWindowText(childAfter, className, 255);
                        if (className.ToString() == "确定")
                            ok = childAfter;
                        else if (className.ToString() == "取消")
                            cancel = childAfter;
                    }
                    else if (classNameStr.StartsWith(CCCFillManager.ContainerClassName))
                        container = childAfter;
                }
                if (container != IntPtr.Zero && ok != IntPtr.Zero)
                    break;
            } while (childAfter != IntPtr.Zero);
            outList = ControlSorter.SortChild(container);
            return ok;
        }

        public override bool FillPage()
        {
            List<IntPtr> containerList;
            this.ReadData();
            Hashtable controlGroup = new Hashtable();
            IntPtr ok = IntPtr.Zero, rowNumber = IntPtr.Zero, cancel = IntPtr.Zero;
            foreach (var item in data)
            {
                ApiSetter.ClickButton(base.Add, base.HWnd, null, null);
                Page9_12_2_Relation relation = null;
                // 初始化填报控件
                if (ok == IntPtr.Zero || cancel == IntPtr.Zero || rowNumber == IntPtr.Zero || controlGroup.Count == 0)
                {
                    controlGroup.Clear();
                    ok = GetFillControlList(out containerList, out cancel);
                    if (containerList == null)
                        return false;
                    rowNumber = containerList[6];
                    int startIndex = 7;
                    int count = 3;
                    while (count > 0)
                    {
                        relation = new Page9_12_2_Relation(containerList, startIndex);
                        if (relation.SeatPosDesc != null)
                            controlGroup.Add(relation.SeatPosDesc, relation);
                        count--;
                        startIndex += 5;
                    }
                }
                // 填写内容
                if (item.SeatPosition != null)
                {
                    relation = controlGroup[item.SeatPosition] as Page9_12_2_Relation;
                    if (relation != null)
                    {
                        FillValue(item.RowOfSeat, rowNumber, relation, item);
                        relation.IsUsed = true;
                    }
                }
                // 将未填写项标为不适用
                foreach (DictionaryEntry c in controlGroup)
                {
                    relation = c.Value as Page9_12_2_Relation;
                    if (relation != null && relation.IsUsed == false)
                        ApiSetter.ClickButton(relation.SeatPosition, base.HWnd, null, null);
                }
                ApiSetter.ClickButton(ok, base.HWnd, null, null);
                // 还原CheckBox状态
                ApiSetter.ClickButton(base.Add, base.HWnd, null, null);
                foreach (DictionaryEntry c in controlGroup)
                {
                    relation = c.Value as Page9_12_2_Relation;
                    if (relation != null && relation.IsUsed == false)
                        ApiSetter.ClickButton(relation.SeatPosition, base.HWnd, null, null);
                    relation.IsUsed = false;
                }
                ApiSetter.ClickButton(cancel, base.HWnd, null, null);
            }
            this.Main.ClickSaveButton(base.Save);
            return true;
        }

        private void FillValue(string key, IntPtr rowNumber, Page9_12_2_Relation relation, Page9_12_2Value value)
        {
            if (ApiSetter.SetComboBoxSelected(base.HWnd, rowNumber, key) == false)
                return;
            ApiSetter.SetText(relation.FrontAirBags, value.FrontAirBags);
            ApiSetter.SetText(relation.SideAirBags, value.SideAirBags);
            ApiSetter.SetText(relation.SeatBeltPreloader, value.SeatBeltPreloader);
        }

        private void ReadData()
        {
            Hashtable columnHeader = new Hashtable();
            Office.Excel.ForwardReadWorksheet sheet = base.Sheet;
            object content;
            if (sheet.ReadNextRow())
            {
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    columnHeader.Add(sheet.CurrentCell.ColumnIndex, content == null ? "" : content.ToString());
                }
            }
            bool isEmptyRow;
            Page9_12_2Value value = null;
            while (sheet.ReadNextRow())
            {
                isEmptyRow = true;
                value = new Page9_12_2Value();
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    string str = content == null ? "" : content.ToString();
                    if (string.IsNullOrEmpty(str) == false)
                        isEmptyRow = false;
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "座椅位置":
                        value.SeatPosition = str;
                        break;
                    case "前气囊":
                        value.FrontAirBags = str;
                        break;
                    case "侧气囊":
                        value.SideAirBags = str;
                        break;
                    case "安全带预加载装置":
                        value.SeatBeltPreloader = str;
                        break;
                    case "第几排座椅":
                        value.RowOfSeat = str;
                        break;
                    }
                }
                if (isEmptyRow)
                    continue;
                data.Add(value);
            }
        }
    }
}
