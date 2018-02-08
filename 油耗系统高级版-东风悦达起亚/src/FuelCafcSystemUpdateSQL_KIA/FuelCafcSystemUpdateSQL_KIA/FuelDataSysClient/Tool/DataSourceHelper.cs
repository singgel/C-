using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FuelDataSysClient.Tool
{
    public class DataSourceHelper
    {

        /// <summary>
        /// 模拟测试合格证数据  测试用户使用
        /// </summary>
        /// <returns></returns>
        public static DataTable CertificateData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("VIN");
            dt.Columns.Add("CLXH");
            dt.Columns.Add("CLZZRQ");
            dt.Columns.Add("RLLX");
            for (int i = 1; i < 20; i++)
            {
                var dr = dt.NewRow();
                dr["VIN"] = String.Format("TESTXLLIEVBNM{0}EI{0}", i);
                dr["CLXH"] = "TESTCLXH";
                dr["CLZZRQ"] = "2015-02-05";
                dr["RLLX"] = "汽油";
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 模拟测试合格证数据  测试用户使用
        /// </summary>
        /// <returns></returns>
        public static DataTable AnnouncementData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CLXH");
            dt.Columns.Add("RLLX");
            dt.Columns.Add("ZCZBZL");
            dt.Columns.Add("ZHGK");
            for (int i = 1; i < 20; i++)
            {
                var dr = dt.NewRow();
                dr["CLXH"] = String.Format("TEST{0}CLXH{0}", i);
                dr["RLLX"] = "汽油";
                dr["ZCZBZL"] = "2522";
                dr["ZHGK"] = "7.8";
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 模拟测试平均油耗参数明细数据  测试用户使用
        /// </summary>
        /// <returns></returns>
        public static DataTable AvgFuelDetailData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Clxh", "CCLEIX");
            data.Add("Rllx", "汽油");
            data.Add("Bsqxs", "334");
            data.Add("Zczbzl", "3322");
            data.Add("Zwps", "8");
            data.Add("Zhgkxslc", "4.4");
            data.Add("TgtZhgkrlxhl", "6.6");
            data.Add("ActZhgkrlxhl", "5.6");
            data.Add("Sl_hs", "556");
            data.Add("Sl_act", "224");
            return Common.DataTableHelper.FillDataTable(data, 20);
        }

        /// <summary>
        /// 模拟测试数据核算  测试用户使用
        /// </summary>
        /// <returns></returns>
        public static DataTable CAFCFuelData()
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Sl_act", "334");
            data.Add("Sl_hs", "445");
            data.Add("Cafc", "8.2");
            data.Add("Tcafc", "9.2");
            return Common.DataTableHelper.FillDataTable(data, 2);
        }
    }
}
