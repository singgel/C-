using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient
{
    public class QueryCafcModel
    {
        public QueryCafcModel(int id, string bandYear, string bandT_SL, string bandT_ZBZL, string bandT_MBZ, string bandT_DBZ,
            string bandT_SJZ, string bandN_SL, string bandN_ZBZL, string bandN_MBZ, string bandN_DBZ, string bandN_SJZ, string bandS_FLG, string bandS_Value, string bandS_Raing)
        {
            this.ID = id;
            this.bandYear = bandYear;
            this.bandT_SL = bandT_SL;
            this.bandT_ZBZL = bandT_ZBZL;
            this.bandT_MBZ = bandT_MBZ;
            this.bandT_DBZ = bandT_DBZ;
            this.bandT_SJZ = bandT_SJZ;

            this.bandN_SL = bandN_SL;
            this.bandN_ZBZL = bandN_ZBZL;
            this.bandN_MBZ = bandN_MBZ;
            this.bandN_DBZ = bandN_DBZ;
            this.bandN_SJZ = bandN_SJZ;

            this.bandS_FLG = bandS_FLG;
            this.bandS_Value = bandS_Value;
            this.bandS_Raing = bandS_Raing;
        }

        public int ID { get; set; }
        public string bandYear { get; set; }
        public string bandT_SL { get; set; }
        public string bandT_ZBZL { get; set; }
        public string bandT_MBZ { get; set; }
        public string bandT_DBZ { get; set; }
        public string bandT_SJZ { get; set; }
        public string bandN_SL { get; set; }
        public string bandN_ZBZL { get; set; }
        public string bandN_MBZ { get; set; }
        public string bandN_DBZ { get; set; }
        public string bandN_SJZ { get; set; }
        public string bandS_FLG { get; set; }
        public string bandS_Value { get; set; }
        public string bandS_Raing { get; set; }
    }
}
