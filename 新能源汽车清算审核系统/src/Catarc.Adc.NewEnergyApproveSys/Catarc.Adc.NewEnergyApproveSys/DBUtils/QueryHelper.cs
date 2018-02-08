using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Catarc.Adc.NewEnergyApproveSys.DBUtils
{
    public class QueryHelper
    {
        /// <summary>
        /// 根据城市省份数据
        /// </summary>
        /// <returns>省份</returns>
        public static Dictionary<string, string> queryProvinceOfCity()
        {
            Dictionary<string, string> mapData = new Dictionary<string, string>();
            try
            {
                DataTable ProvinceOfCity = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from SYS_CITY ").Tables[0];
                foreach (DataRow dr in ProvinceOfCity.Rows)
                {
                    mapData.Add(dr["CITY_NAME"].ToString(), dr["COUNTY_NAME"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mapData;
        }

        /// <summary>
        /// 根据VIN获取整车数据
        /// </summary>
        /// <param name="vin">车架号</param>
        /// <returns>整车数据</returns>
        public static Dictionary<string, string> queryInfomationByVin(string guid)
        {
            Dictionary<string, string> mapData = new Dictionary<string, string>();
            try
            {
                string sqlAll = String.Format("select * from DB_INFOMATION where guid = '{0}' ", guid);
                DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlAll, null);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string strKey = dt.Columns[i].ColumnName;
                        mapData.Add(strKey, dt.Rows[0][i].ToString());
                    }
                }
                var param = OracleHelper.GetSingle(OracleHelper.conn, (string)"select DIC_NAME from SYS_DIC where DIC_TYPE='截止日期'");
                if (mapData.Keys.Contains("JZNF") && param != null)
                {
                    mapData["JZNF"] = param.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mapData;
        }

        /// <summary>
        /// 根据VIN获取对比结果
        /// </summary>
        /// <param name="vin">车架号</param>
        /// <returns>对比结果</returns>
        public static Dictionary<string, string> queryRightByVin(string vin)
        {
            Dictionary<string, string> mapData = new Dictionary<string, string>();
            try
            {
                string sqlXT = String.Format("select * from DB_INFOMATION where vin = '{0}' ", vin);
                DataTable dtXT = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlXT).Tables[0];
                string sqlGG_New = String.Format("select * from ANNOUNCE_MAX_ENTITY where MODEL_VEHICLE = '{0}' ", dtXT.Rows[0]["CLXH"]);
                DataTable dtGG_New = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlGG_New).Tables[0];
                string sqlGG_Old = String.Format("select * from ANNOUNCE_TIP_ENTITY where MODEL_VEHICLE = '{0}' ", dtXT.Rows[0]["CLXH"]);
                DataTable dtGG_Old = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlGG_Old).Tables[0];
                if (!dtXT.Rows[0]["DCDTXX_XH"].ToString().Trim().Equals(dtGG_New.Rows[0]["MODEL_SINGLE"].ToString().Trim()))
                {
                    mapData.Add("DCDTXX_XH", dtGG_Old.Rows[0]["MODEL_SINGLE"].ToString());
                }
                if (!dtXT.Rows[0]["DCDTXX_SCQY"].ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(dtGG_New.Rows[0]["MFRS_SINGLE"].ToString().Replace('(', '（').Replace(')', '）').Trim()))
                {
                    mapData.Add("DCDTXX_SCQY", dtGG_Old.Rows[0]["MFRS_SINGLE"].ToString().Replace('(', '（').Replace(')', '）'));
                }
                if (!dtXT.Rows[0]["DCZXX_XH"].ToString().Trim().Equals(dtGG_New.Rows[0]["MODEL_WHOLE"].ToString().Trim()))
                {
                    mapData.Add("DCZXX_XH", dtGG_Old.Rows[0]["MODEL_WHOLE"].ToString());
                }
                if (!dtXT.Rows[0]["DCZXX_ZRL"].ToString().Trim().Equals(dtGG_New.Rows[0]["CAPACITY_BAT"].ToString().Trim()))
                {
                    mapData.Add("DCZXX_ZRL", dtGG_Old.Rows[0]["CAPACITY_BAT"].ToString());
                }
                if (!dtXT.Rows[0]["DCZXX_SCQY"].ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(dtGG_New.Rows[0]["MFRS_BAT"].ToString().Replace('(', '（').Replace(')', '）').Trim()))
                {
                    mapData.Add("DCZXX_SCQY", dtGG_Old.Rows[0]["MFRS_BAT"].ToString().Replace('(', '（').Replace(')', '）'));
                }
                if (!dtXT.Rows[0]["QDDJXX_XH_1"].ToString().Trim().Equals(dtGG_New.Rows[0]["MODEL_DRIVE"].ToString().Trim()))
                {
                    mapData.Add("QDDJXX_XH_1", dtGG_Old.Rows[0]["MODEL_DRIVE"].ToString());
                }
                if (!dtXT.Rows[0]["QDDJXX_EDGL_1"].ToString().Trim().Equals(dtGG_New.Rows[0]["RATEPOW_DRIVE"].ToString().Trim()))
                {
                    mapData.Add("QDDJXX_EDGL_1", dtGG_Old.Rows[0]["RATEPOW_DRIVE"].ToString());
                }
                if (!dtXT.Rows[0]["QDDJXX_SCQY_1"].ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(dtGG_New.Rows[0]["MFRS_DRIVE"].ToString().Replace('(', '（').Replace(')', '）').Trim()))
                {
                    mapData.Add("QDDJXX_SCQY_1", dtGG_Old.Rows[0]["MFRS_DRIVE"].ToString().Replace('(', '（').Replace(')', '）'));
                }
                if (!dtXT.Rows[0]["RLDCXX_XH"].ToString().Trim().Equals(dtGG_New.Rows[0]["MDEL_FUEL"].ToString().Trim()))
                {
                    mapData.Add("RLDCXX_XH", dtGG_Old.Rows[0]["MDEL_FUEL"].ToString());
                }
                if (!dtXT.Rows[0]["RLDCXX_EDGL"].ToString().Trim().Equals(dtGG_New.Rows[0]["RATEPOW_FUEL"].ToString().Trim()))
                {
                    mapData.Add("RLDCXX_EDGL", dtGG_Old.Rows[0]["RATEPOW_FUEL"].ToString());
                }
                if (!dtXT.Rows[0]["RLDCXX_SCQY"].ToString().Replace('(', '（').Replace(')', '）').Trim().Equals(dtGG_New.Rows[0]["MFRS_FUEL"].ToString().Replace('(', '（').Replace(')', '）').Trim()))
                {
                    mapData.Add("RLDCXX_SCQY", dtGG_Old.Rows[0]["MFRS_FUEL"].ToString().Replace('(', '（').Replace(')', '）'));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return mapData;
        }
    }
}
