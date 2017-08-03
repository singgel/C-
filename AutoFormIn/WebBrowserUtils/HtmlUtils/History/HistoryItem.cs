using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.History
{
    public class HistoryItem
    {
        public int Id
        {
            get;
            internal set;
        }

        public string UserName
        {
            get;
            internal set;
        }

        public DateTime FillDate
        {
            get;
            internal set;
        }

        public string FillType
        {
            get;
            internal set;
        }

        public int SuccessCount
        {
            get;
            internal set;
        }

        public int FailCount
        {
            get;
            internal set;
        }
    }
}
