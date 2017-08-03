using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    class Page9_13_3 : NormalAddPage
    {
        private Hashtable data;

        public Page9_13_3(IntPtr hwnd, Main_3C owner, Office.Excel.ForwardReadWorksheet sheet)
            : base(hwnd, owner, "添加一排", sheet)
        {
            data = new Hashtable();
        }

        private IntPtr GetFillControlList(out List<IntPtr> outList, out IntPtr cancel)
        {
            IntPtr ok = IntPtr.Zero;
            IntPtr container = base.EditRange;
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
                    if (className.ToString().StartsWith(CCCFillManager.ButtonClassName))
                    {
                        className.Clear();
                        NativeApi.GetWindowText(childAfter, className, 255);
                        if (className.ToString() == "确定")
                            ok = childAfter;
                        else if (className.ToString() == "取消")
                            cancel = childAfter;
                    }
                }
                if (ok != IntPtr.Zero)
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
            foreach (DictionaryEntry entry in data)
            {
                ApiSetter.ClickButton(base.Add, base.HWnd, null, null);
                // 初始化填报控件的对应关系
                if (ok == IntPtr.Zero || cancel == IntPtr.Zero || rowNumber == IntPtr.Zero || controlGroup.Count == 0)
                {
                    controlGroup.Clear();
                    ok = GetFillControlList(out containerList, out cancel);
                    rowNumber = IntPtr.Zero;
                    if (containerList == null)
                        return false;

                    containerList = ControlSorter.SortChild(containerList[0]);
                    List<IntPtr> sorted = ControlSorter.SortChild(containerList, 0, 1);
                    rowNumber = sorted[1];
                    sorted = ControlSorter.SortChild(containerList, 1, 1);
                    List<Page9_13_3_Relation> relations = new List<Page9_13_3_Relation>(4);
                    for (int i = 0; i < sorted.Count - 1; i += 2)
                    {
                        Page9_13_3_Relation relation = new Page9_13_3_Relation();
                        relation.SetSeatPosition(sorted[i], sorted[i + 1]);
                        relations.Add(relation);
                        if (relation.SeatPosDesc != null)
                            controlGroup.Add(relation.SeatPosDesc, relation);
                    }
                    int startIndex = 2;
                    int count = 3;
                    while (count > 0 && startIndex + 3 <= containerList.Count)
                    {
                        sorted = ControlSorter.SortChild(containerList, startIndex, 3);  // 生成9.13.3的控件对应关系。
                        if (sorted.Count != 11)
                            continue;
                        Page9_13_3_Relation relation = relations[3 - count];
                        relation.UpperFixedPoint = sorted[4];
                        relation.CarStructOuterSide_DFP = sorted[5]; relation.SeatStructOuterSide_DFP = sorted[6];
                        relation.CarStructInnerSide_DFP = sorted[7]; relation.SeatStructInnerSide_DFP = sorted[8];
                        relation.CarStruct_UFP = sorted[9]; relation.SeatStruct_UFP = sorted[10];
                        count--;
                        startIndex += 3;
                    }
                }
                List<Page9_13_3Value> values = entry.Value as List<Page9_13_3Value>;
                if (values == null || values.Count == 0)
                    continue;
                foreach (var item in values) // 根据数据填写各座椅位置数据，并将已填写的座椅位置移除，
                {
                    if (item.SeatPosition == null)
                        continue;
                    Page9_13_3_Relation relation = controlGroup[item.SeatPosition] as Page9_13_3_Relation;
                    if (relation != null)
                    {
                        FillValue(entry.Key as string, rowNumber, relation, item);
                        relation.IsUsed = true;
                    }
                }
                foreach (DictionaryEntry c in controlGroup)
                {
                    Page9_13_3_Relation relation = c.Value as Page9_13_3_Relation;
                    if (relation != null && relation.IsUsed == false)
                        ApiSetter.ClickButton(relation.SeatPosition, base.HWnd, null, null);// 最后剩余的未填写项设置为不适用
                }
                ApiSetter.ClickButton(ok, base.HWnd, null, null);
                // 还原CheckBox状态
                ApiSetter.ClickButton(base.Add, base.HWnd, null, null);
                foreach (DictionaryEntry c in controlGroup)
                {
                    Page9_13_3_Relation relation = c.Value as Page9_13_3_Relation;
                    if (relation != null && relation.IsUsed == false)
                        ApiSetter.ClickButton(relation.SeatPosition, base.HWnd, null, null);
                    relation.IsUsed = false;
                }
                ApiSetter.ClickButton(cancel, base.HWnd, null, null);
            }
            this.Main.ClickSaveButton(base.Save);
            return true;
        }

        private void FillValue(string key, IntPtr rowNumber, Page9_13_3_Relation relation, Page9_13_3Value value)
        {
            if (ApiSetter.SetComboBoxSelected(base.HWnd, rowNumber, key) == false)
                return;
            ApiSetter.SetText(relation.CarStructOuterSide_DFP, value.CarStructOuterSide_DFP);
            ApiSetter.SetText(relation.SeatStructOuterSide_DFP, value.SeatStructOuterSide_DFP);
            ApiSetter.SetText(relation.CarStructInnerSide_DFP, value.CarStructInnerSide_DFP);
            ApiSetter.SetText(relation.SeatStructInnerSide_DFP, value.SeatStructInnerSide_DFP);
            if (string.IsNullOrEmpty(value.CarStruct_UFP) && string.IsNullOrEmpty(value.SeatStruct_UFP))
                ApiSetter.ClickButton(relation.UpperFixedPoint, base.HWnd, null, null);  // 上固定点不适用
            else
            {
                long style = NativeApi.GetWindowLong(relation.CarStruct_UFP, -16);
                if((style & WindowStyles.WS_DISABLED) == WindowStyles.WS_DISABLED)
                    ApiSetter.ClickButton(relation.UpperFixedPoint, base.HWnd, null, null); 
                ApiSetter.SetText(relation.CarStruct_UFP, value.CarStruct_UFP);
                ApiSetter.SetText(relation.SeatStruct_UFP, value.SeatStruct_UFP);
            }
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
            Page9_13_3Value value = null;
            string lastKey = "", fixedPoint, fixedPointPos;
            while (sheet.ReadNextRow())
            {
                if(value == null)
                    value = new Page9_13_3Value();
                string key = "";
                fixedPointPos = "";
                fixedPoint = "";
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    string str = content == null ? "" : content.ToString();
                   //第几排座椅	座椅位置	固定点	固定点位置	车辆结构(例如：车身地板)	座椅结构(例如：座椅骨架)
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "座椅位置":
                        if (string.IsNullOrEmpty(value.SeatPosition))
                            value.SeatPosition = str;
                        else if (value.SeatPosition != str) // 座椅位置更改时保存当前读取的填报数据。
                        {
                            AddValue(lastKey, value); 
                            value = new Page9_13_3Value();
                            value.SeatPosition = str;
                        }
                        break;
                    case "固定点":
                        fixedPoint = str;
                        break;
                    case "固定点位置":
                        fixedPointPos = str;
                        break;
                    case "车辆结构(例如：车身地板)":
                        switch(fixedPoint)
                        {
                        case "上":
                            value.CarStruct_UFP = str;
                            break;
                        case "下":
                            switch(fixedPointPos)
                            {
                            case "外侧":
                                value.CarStructOuterSide_DFP = str;
                                break;
                            case "内侧":
                                value.CarStructInnerSide_DFP = str;
                                break;
                            }
                            break;
                        }
                        break;
                    case "座椅结构(例如：座椅骨架)":
                        switch (fixedPoint)
                        {
                        case "上":
                            value.SeatStruct_UFP = str;
                            break;
                        case "下":
                            switch (fixedPointPos)
                            {
                            case "外侧":
                                value.SeatStructOuterSide_DFP = str;
                                break;
                            case "内侧":
                                value.SeatStructInnerSide_DFP = str;
                                break;
                            }
                            break;
                        }
                        break;
                    case "第几排座椅":
                        if (string.IsNullOrEmpty(lastKey))// 新增座椅时保存当前读取的填报数据。
                        {
                            key = str;
                            lastKey = key;
                        }
                        else if (lastKey != str)
                        {
                            AddValue(lastKey, value);
                            value = new Page9_13_3Value();
                            key = str;
                            lastKey = key;
                        }
                        break;
                    }
                }
                if (key == null)
                    continue;
            }
        }

        private void AddValue(string key, Page9_13_3Value value)
        {
            List<Page9_13_3Value> values = null;
            if (data.ContainsKey(key))
            {
                values = data[key] as List<Page9_13_3Value>;
                if (values == null)
                {
                    values = new List<Page9_13_3Value>();
                    data[key] = values;
                }
            }
            else
            {
                values = new List<Page9_13_3Value>();
                data.Add(key, values);
            }
            values.Add(value);
        }
    }
}
