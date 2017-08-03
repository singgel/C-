using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;
using System.Collections;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    internal class Page9_12_1 : NormalAddPage
    {
        private Hashtable data;

        public Page9_12_1(IntPtr hwnd, Main_3C owner, Office.Excel.ForwardReadWorksheet sheet)
            : base(hwnd, owner, "添加一排", sheet)
        {
            data = new Hashtable();
        }

        private IntPtr GetFillControlList(out List<IntPtr> outList, out IntPtr cancel)
        {
            IntPtr ok = IntPtr.Zero, container = IntPtr.Zero, childAfter = IntPtr.Zero;
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
        /// <summary>
        /// 将指定容器遍历到的第一个ComboBox作为rowNumber控件。
        /// </summary>
        /// <param name="container"></param>
        private IntPtr GetRowNumberControl(IntPtr container)
        {
            IntPtr result = IntPtr.Zero;
            StringBuilder className = new StringBuilder(256);
            NativeApi.EnumChildWindows(container, (child, lParam) =>
            {
                className.Clear();
                NativeApi.GetClassName(child, className, 255);
                if (className.ToString().StartsWith(CCCFillManager.ComboBoxClassName))
                {
                    result = child;
                    return false;
                }
                return true;
            }, IntPtr.Zero);
            return result;
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
                List<Page9_12_1Value> values = entry.Value as List<Page9_12_1Value>;
                // 初始化填报控件的对应关系
                if (ok == IntPtr.Zero || cancel == IntPtr.Zero || rowNumber == IntPtr.Zero || controlGroup.Count == 0)
                {
                    controlGroup.Clear();
                    ok = GetFillControlList(out containerList, out cancel);
                    if (containerList == null || containerList.Count < 11)
                        return false;
                    rowNumber = GetRowNumberControl(containerList[1]);
                    int startIndex = 2;
                    int count = 3;
                    while (count > 0)
                    {
                        List<IntPtr> sorted = ControlSorter.SortChild(containerList, startIndex, 3);
                        startIndex += 3;
                        Page9_12_1_Relation relation = new Page9_12_1_Relation(sorted);
                        if (relation.PositionDesc != null)
                            controlGroup.Add(relation.PositionDesc, relation);  // 用座椅位置作为键值存储控件句柄的对应关系。
                        count--;
                    }
                }
                if (values == null || values.Count == 0)
                    continue;
                foreach (var item in values) // 根据数据填写各座椅位置数据，并将已填写的座椅位置移除，
                {                                            // 最后剩余的未填写项设置为不适用
                    if (item.Position == null)
                        continue;
                    Page9_12_1_Relation relation = controlGroup[item.Position] as Page9_12_1_Relation;
                    if (relation != null)
                    {
                        FillValue(entry.Key as string, rowNumber, relation, item);
                        relation.IsUsed = true;
                    }
                }
                foreach (DictionaryEntry c in controlGroup)
                {
                    Page9_12_1_Relation relation = c.Value as Page9_12_1_Relation;
                    if (relation != null && relation.IsUsed == false)
                        ApiSetter.ClickButton(relation.Position, base.HWnd, null, null);
                }
                ApiSetter.ClickButton(ok, base.HWnd, null, null);
                // 还原CheckBox状态
                ApiSetter.ClickButton(base.Add, base.HWnd, null, null); // 显示控件
                foreach (DictionaryEntry c in controlGroup)
                {
                    Page9_12_1_Relation relation = c.Value as Page9_12_1_Relation;
                    if (relation != null && relation.IsUsed == false)
                        ApiSetter.ClickButton(relation.Position, base.HWnd, null, null);
                    relation.IsUsed = false;
                }
                ApiSetter.ClickButton(cancel, base.HWnd, null, null);
            }
            this.Main.ClickSaveButton(base.Save);
            return true;
        }
        //第几排座椅	座椅位置	是否已获证	CCC证书编号	变型,如适用	安全带高度调节装置	生产厂名称	安全带型式*	
        //型号	卷收器型式**	卷收器安装角度 	安全带的安装位置	安全带安装位置附件	搭扣锁型式***	安全带固定点数量	
        //CCC认证标志的位置	CCC认证标志位置附件	CCC认证标志的固定方法
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
            Page9_12_1Value value = null;
            while (sheet.ReadNextRow())
            {
                value = new Page9_12_1Value();
                string key = "";
                isEmptyRow = true;
                while (sheet.ReadNextCell(false))
                {
                    content = sheet.GetContent();
                    string str = content == null ? "" : content.ToString();
                    if (string.IsNullOrEmpty(str) == false)
                        isEmptyRow = false;
                    switch (columnHeader[sheet.CurrentCell.ColumnIndex] as string)
                    {
                    case "座椅位置":
                        value.Position = str;
                        break;
                    case "是否已获证":
                        value.Cert = str;
                        break;
                    case "CCC证书编号":
                        value.CertNo = str;
                        break;
                    case "变型,如适用":
                        value.Transform = str;
                        break;
                    case "安全带高度调节装置":
                        value.HeightAdj = str;
                        break;
                    case "生产厂名称":
                        value.Productor = str;
                        break;
                    case "安全带型式*":
                        value.SeatBeltType = str;
                        break;
                    case "型号":
                        value.Model = str;
                        break;
                    case "卷收器型式**":
                        value.RetractorType = str;
                        break;
                    case "卷收器安装角度":
                        value.RetractorAngle = str;
                        break;
                    case "安全带的安装位置":
                        value.SeatBeltPosition = str;
                        break;
                    case "安全带安装位置附件":
                        value.SeatBeltPosAttach = str;
                        break;
                    case "搭扣锁型式***":
                        value.LockType = str;
                        break;
                    case "安全带固定点数量":
                        value.FixedPosNum = str;
                        break;
                    case "CCC认证标志的位置":
                        value.CCCSignPos = str;
                        break;
                    case "CCC认证标志位置附件":
                        value.CCCSignPosAttach = str;
                        break;
                    case "CCC认证标志的固定方法":
                        value.CCCSignFixation = str;
                        break;
                    case "第几排座椅":
                        key = str;
                        break;
                    }
                }
                if (key == null || isEmptyRow)
                    continue;
                List<Page9_12_1Value> values = null;
                if (data.ContainsKey(key))
                {
                    values = data[key] as List<Page9_12_1Value>;
                    if (values == null)
                    {
                        values = new List<Page9_12_1Value>();
                        data[key] = values;
                    }
                }
                else
                {
                    values = new List<Page9_12_1Value>();
                    data.Add(key, values);
                }
                values.Add(value);
            }
        }

        private void FillValue(string key, IntPtr rowNumber, Page9_12_1_Relation relation, Page9_12_1Value value)
        {
            if(ApiSetter.SetComboBoxSelected(base.HWnd, rowNumber, key) == false)
                return;
            ApiSetter.SetComboBoxSelected(base.HWnd, relation.Cert, value.Cert);
            ApiSetter.SetText(relation.CertNo, value.CertNo);
            if (string.IsNullOrEmpty(value.Transform))
                ApiSetter.ClickButton(relation.Transform, base.HWnd, null, null);
            else
                ApiSetter.SetText(relation.TransformText, value.Transform);
            ApiSetter.SetComboBoxSelected(base.HWnd, relation.HeightAdj, value.HeightAdj);
            ApiSetter.SetText(relation.Productor, value.Productor);
            ApiSetter.SetText(relation.SeatBeltType, value.SeatBeltType);
            ApiSetter.SetText(relation.Model, value.Model);
            ApiSetter.SetText(relation.RetractorType, value.RetractorType);
            ApiSetter.SetText(relation.RetractorAngle, value.RetractorAngle);
            ApiSetter.SetText(relation.SeatBeltPosition, value.SeatBeltPosition);
            if (string.IsNullOrEmpty(value.SeatBeltPosAttach) == false)
            {
                ApiSetter.ClickButton(relation.SeatBeltPosAttach, base.HWnd, ListenAttachWindow,
                    new FillValue3C() { PublicAttachFile = value.SeatBeltPosAttach, Separators = FillParameter3C.DefaultSeparators });
            }
            ApiSetter.SetText(relation.LockType, value.LockType);
            ApiSetter.SetText(relation.FixedPosNum, value.FixedPosNum);
            ApiSetter.SetText(relation.CCCSignPos, value.CCCSignPos);
            if (string.IsNullOrEmpty(value.SeatBeltPosAttach) == false)
            {
                ApiSetter.ClickButton(relation.CCCSignPosAttach, base.HWnd, ListenAttachWindow,
                    new FillValue3C() { PublicAttachFile = value.CCCSignPosAttach, Separators = FillParameter3C.DefaultSeparators });
            }
            ApiSetter.SetText(relation.CCCSignFixation, value.CCCSignFixation);
        }

        private void ListenAttachWindow(object state)
        {
            AttachWindow_3C attachWindow = FillDialog_3C.GetFillDialog(CCCWindowType.AttachWindow, this.Main, this.Main.ProcessId) as AttachWindow_3C;
            if (attachWindow != null)
                attachWindow.DoFillWork(state);
        }
    }
}
