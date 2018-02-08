using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Tool
{
    public class InitDataTime
    {
        DateTime now = DateTime.Now;

        public string getStartTime() 
        {
            DateTime monthFirstDate = new DateTime(now.Year,now.Month,1);  //当前月第一天
            return monthFirstDate.ToString("yyyy/MM/dd");
        }

        public string getEndTime()
        {
            DateTime monthFirstDate = new DateTime(now.Year, now.Month, 1);
            DateTime monthLastDate = monthFirstDate.AddMonths(1).AddDays(-1);  //当前月最后一天
            return monthLastDate.ToString("yyyy/MM/dd");
        }
    }
}
