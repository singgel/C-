using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.WinApi;

namespace WebBrowserUtils.HtmlUtils.Fillers.SpecialPage
{
    class Page9_12_1_Relation
    {
        public string PositionDesc;
        public IntPtr Position, Cert, CertNo, Transform, TransformText, HeightAdj, Productor, SeatBeltType,
            Model, RetractorType, RetractorAngle, SeatBeltPosition, SeatBeltPosAttach, LockType, FixedPosNum,
            CCCSignPos, CCCSignPosAttach, CCCSignFixation;
        public bool IsUsed;

        public Page9_12_1_Relation(List<IntPtr> controlList)
        {
            IsUsed = false;
            this.GetRelation(controlList);
        }

        private void GetRelation(List<IntPtr> list)
        {
            StringBuilder text = new StringBuilder(256);
            NativeApi.GetWindowText(list[0], text, 255);
            PositionDesc = text.ToString();
            int index = 1;
            Position = list[index++];
            Cert = list[index++];
            CertNo = list[index++];
            Transform = list[index++];
            TransformText = list[index++];
            HeightAdj = list[index++];
            Productor = list[index++];
            SeatBeltType = list[index++];
            Model = list[index++];
            RetractorType = list[index++];
            RetractorAngle = list[index++];
            SeatBeltPosition = list[index++];
            SeatBeltPosAttach = list[index++];
            LockType = list[index++];
            FixedPosNum = list[index++];
            CCCSignPos = list[index++];
            CCCSignPosAttach = list[index++];
            CCCSignFixation = list[index++];
        }
    }
}
