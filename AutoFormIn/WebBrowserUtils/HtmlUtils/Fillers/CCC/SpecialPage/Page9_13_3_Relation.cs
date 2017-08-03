using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    class Page9_13_3_Relation
    {
        public bool IsUsed;
        public string SeatPosDesc; // 座椅位置字符串
        public IntPtr SeatPosition, // 座椅位置是否适用
            UpperFixedPoint, // 上固定点是否适用
            CarStructOuterSide_DFP, // 车辆结构下固定点外侧
            CarStructInnerSide_DFP, // 车辆结构下固定点内侧
            SeatStructOuterSide_DFP, // 座椅结构下固定点外侧
            SeatStructInnerSide_DFP, // 座椅结构下固定点内侧
            SeatStruct_UFP, // 座椅结构 上固定点
            CarStruct_UFP; // 车辆结构上固定点

        public void SetSeatPosition(IntPtr desc, IntPtr handle)
        {
            StringBuilder text = new StringBuilder(256);
            NativeApi.GetWindowText(desc, text, 255);
            SeatPosDesc = text.ToString();
            SeatPosition = handle;
        }
    }
}
