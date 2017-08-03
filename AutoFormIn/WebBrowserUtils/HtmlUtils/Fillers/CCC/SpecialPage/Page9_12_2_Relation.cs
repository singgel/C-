using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    class Page9_12_2_Relation
    {
        public bool IsUsed;
        public string SeatPosDesc;
        public IntPtr SeatPosition, FrontAirBags, SideAirBags, SeatBeltPreloader;

        public Page9_12_2_Relation(List<IntPtr> list, int startIndex)
        {
            if (startIndex + 5 > list.Count)
                return;
            SeatPosition = list[startIndex++];
            StringBuilder text = new StringBuilder(256);
            NativeApi.GetWindowText(list[startIndex++], text, 255);
            SeatPosDesc = text.ToString();
            FrontAirBags = list[startIndex++];
            SideAirBags = list[startIndex++];
            SeatBeltPreloader = list[startIndex];
            NativeApi.EnumChildWindows(SeatBeltPreloader, (handle, lParam) =>
            {
                SeatBeltPreloader = handle;
                return true;
            }, IntPtr.Zero);
        }
    }
}
