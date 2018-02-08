using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Catarc.Adc.NewEnergyAccountSys.Common;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using System.Data.OleDb;
using System.Diagnostics;
using Catarc.Adc.NewEnergyAccountSys.LogUtils;

namespace Catarc.Adc.NewEnergyAccountSys.DataUtils
{
    public class DataCompareHelper
    {

        /// <summary>
        /// 比对公告的参数
        /// </summary>
        /// <param name="dtGG">公告参数</param>
        /// <param name="dtXT">系统参数</param>
        /// <returns>比对信息</returns>
        public static DataTable CompareDataTable_DT(DataTable dt)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            DataTable dtResult = dt.Copy();
            dtResult.Columns.Add("STATUS", Type.GetType("System.String"));
            foreach (DataRow dr in dtResult.Rows)
                dr["STATUS"] = "-1";
            //Step1：获取公告数据
            var dtGG_All = AccessHelper.ExecuteDataSet(AccessHelper.conn, "select MODEL_VEHICLE,Ekg,MODEL_SINGLE,MFRS_SINGLE,MODEL_WHOLE,CAPACITY_BAT,MFRS_BAT,MODEL_DRIVE,RATEPOW_DRIVE,MFRS_DRIVE,MDEL_FUEL,RATEPOW_FUEL,MFRS_FUEL from ANNOUNCE_ENTITIES").Tables[0];
            DataView dv = dtGG_All.DefaultView;
            string[] strComuns = { "MODEL_VEHICLE", "Ekg", "MODEL_SINGLE", "MFRS_SINGLE", "MODEL_WHOLE", "CAPACITY_BAT", "MFRS_BAT", "MODEL_DRIVE", "RATEPOW_DRIVE", "MFRS_DRIVE", "MDEL_FUEL", "RATEPOW_FUEL", "MFRS_FUEL" };
            dtGG_All = dv.ToTable(true, strComuns);
            stopWatch.Stop();
            TimeSpan ts1 = stopWatch.Elapsed;
            LogManager.Log("TimeSpend", "CompareTime", String.Format("Step1：获取公告数据耗时：{0}时{1}分{2}秒", ts1.Hours, ts1.Minutes, ts1.Seconds));
            stopWatch.Start();
            if (dtGG_All == null || dtGG_All.Rows.Count == 0)
            {
                return dtResult;
            }
            //Step2：判断是否在公告中存在
            var clxhArr_GG = dtGG_All.AsEnumerable().Select(d => d.Field<string>("MODEL_VEHICLE")).Distinct().ToArray();
            var vinArr_NotExits = (from d in dt.AsEnumerable()
                                   where !clxhArr_GG.Contains(d.Field<string>("CLXH"))
                                   select d.Field<string>("VIN")).ToArray();
            stopWatch.Stop();
            TimeSpan ts2 = stopWatch.Elapsed - ts1;
            LogManager.Log("TimeSpend", "CompareTime", String.Format("Step2：判断是否在公告中存在耗时：{0}时{1}分{2}秒", ts2.Hours, ts2.Minutes, ts2.Seconds));
            stopWatch.Start();
            if (vinArr_NotExits.Length == dt.Rows.Count)
            {
                return dtResult;
            }
            //Step3:括号半角转全角
            foreach (DataRow row in dt.Rows)
            {
                row.BeginEdit();
                row["DCDTXX_SCQY"] = row["DCDTXX_SCQY"].ToString().Replace('(', '（').Replace(')', '）').Trim();
                row["DCZXX_SCQY"] = row["DCZXX_SCQY"].ToString().Replace('(', '（').Replace(')', '）').Trim();
                row["QDDJXX_SCQY_1"] = row["QDDJXX_SCQY_1"].ToString().Replace('(', '（').Replace(')', '）').Trim();
                row["RLDCXX_SCQY"] = row["RLDCXX_SCQY"].ToString().Replace('(', '（').Replace(')', '）').Trim();
            }
            foreach (DataRow row in dtGG_All.Rows)
            {
                row.BeginEdit();
                row["MFRS_SINGLE"] = row["MFRS_SINGLE"].ToString().Replace('(', '（').Replace(')', '）').Trim();
                row["MFRS_BAT"] = row["MFRS_BAT"].ToString().Replace('(', '（').Replace(')', '）').Trim();
                row["MFRS_DRIVE"] = row["MFRS_DRIVE"].ToString().Replace('(', '（').Replace(')', '）').Trim();
                row["MFRS_FUEL"] = row["MFRS_FUEL"].ToString().Replace('(', '（').Replace(')', '）').Trim();
            }
            stopWatch.Stop();
            TimeSpan ts3 = stopWatch.Elapsed - ts2;
            LogManager.Log("TimeSpend", "CompareTime", String.Format("Step3:括号半角转全角耗时：{0}时{1}分{2}秒", ts3.Hours, ts3.Minutes, ts3.Seconds));
            stopWatch.Start();
            //Step4:判断与公告数据是否一致
            var vinArr_Equal = (from d in dt.AsEnumerable()
                                join dd in dtGG_All.AsEnumerable()
                                on (d.Field<string>("CLXH") ?? "") equals (dd.Field<string>("MODEL_VEHICLE") ?? "")
                                where (d.Field<string>("CLXH") ?? "").Equals((dd.Field<string>("MODEL_VEHICLE") ?? ""))
                                    //&& (d.Field<string>("EKGZ") ?? "").Equals((dd.Field<string>("Ekg") ?? ""))
                                   && (d.Field<string>("DCDTXX_XH") ?? "").Equals((dd.Field<string>("MODEL_SINGLE") ?? ""))
                                   && (d.Field<string>("DCDTXX_SCQY") ?? "").Equals((dd.Field<string>("MFRS_SINGLE") ?? ""))
                                   && (d.Field<string>("DCZXX_XH") ?? "").Equals((dd.Field<string>("MODEL_WHOLE") ?? ""))
                                   && (d.Field<string>("DCZXX_ZRL") ?? "").Equals((dd.Field<string>("CAPACITY_BAT") ?? ""))
                                   && (d.Field<string>("DCZXX_SCQY") ?? "").Equals((dd.Field<string>("MFRS_BAT") ?? ""))
                                   && (d.Field<string>("QDDJXX_XH_1") ?? "").Equals((dd.Field<string>("MODEL_DRIVE") ?? ""))
                                   && (d.Field<string>("QDDJXX_EDGL_1") ?? "").Equals((dd.Field<string>("RATEPOW_DRIVE") ?? ""))
                                   && (d.Field<string>("QDDJXX_SCQY_1") ?? "").Equals((dd.Field<string>("MFRS_DRIVE") ?? ""))
                                   && (d.Field<string>("RLDCXX_XH") ?? "").Equals((dd.Field<string>("MDEL_FUEL") ?? ""))
                                   && (d.Field<string>("RLDCXX_EDGL") ?? "").Equals((dd.Field<string>("RATEPOW_FUEL") ?? ""))
                                   && (d.Field<string>("RLDCXX_SCQY") ?? "").Equals((dd.Field<string>("MFRS_FUEL") ?? ""))
                                select d.Field<string>("VIN")).ToArray();
            stopWatch.Stop();
            TimeSpan ts4 = stopWatch.Elapsed - ts3;
            LogManager.Log("TimeSpend", "CompareTime", String.Format("Step4:判断与公告数据是否一致耗时：{0}时{1}分{2}秒", ts4.Hours, ts4.Minutes, ts4.Seconds));
            foreach (DataRow dr in dtResult.Rows)
            {
                if (vinArr_Equal.Contains(dr["VIN"].ToString().Trim()))
                    dr["STATUS"] = "0";
            }
            if (dt.Rows.Count == (vinArr_NotExits.Length + vinArr_Equal.Length))
            {
                return dtResult;
            }
            //Step5:找出不一致数据
            var vinArr_Checked = vinArr_NotExits.Concat(vinArr_Equal).ToArray();
            foreach (DataRow dr in dtResult.Rows)
            {
                if (!vinArr_Checked.Contains(dr["VIN"].ToString().Trim()))
                    dr["STATUS"] = "1";
            }
            return dtResult;
        }
    }
}
