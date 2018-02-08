using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DevExpress.XtraGrid.Views.Grid;
using System.Reflection;

namespace FuelDataSysClient.FuelCafc
{
    public class CafcUtils
    {

        /// <summary>
        /// 根据条件查询数据上报统计
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="clxh">车辆型号</param>
        /// <param name="rllx">燃料类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="timeType">时间类型（制造日期or上传日期）</param>
        /// <returns></returns>
        public DataSet QueryStatisticsData(string userId, string clxh, string rllx, string startTime, string endTime)
        {
            CalcDBHelper dbUtil = new CalcDBHelper();

            string sqlCondition = string.Format("CLZL='乘用车（M1）' and USER_ID='{0}'", userId);
            if (!string.IsNullOrEmpty(clxh))
            {
                sqlCondition += " AND CLXH LIKE '%" + clxh.Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(rllx))
            {
                sqlCondition += " AND RLLX='" + rllx.Trim() + "'";
            }

            if (!string.IsNullOrEmpty(startTime))
            {
                sqlCondition += " AND CLZZRQ>='" + startTime.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sqlCondition += " AND CLZZRQ<'" + Convert.ToDateTime(endTime.Trim()).Add(new TimeSpan(24, 0, 0)).ToString() + "'";
            }

            string sqlBasic = string.Empty;

            sqlBasic = string.Format(@"SELECT T1.QCSCQY,T1.QYLX,T1.USER_ID,T1.CLXH,CAST(T1.CLZZRQ  AS NVARCHAR) CLZZRQ,CAST(SUM(SL) AS INT) SL FROM 
                                       (
                                       	SELECT QCSCQY,USER_ID,CLXH,
                                       	    CASE SUBSTRING(QYID,1,1) WHEN 'F' THEN '进口' ELSE '国产' END QYLX,
                                        	CONVERT(NVARCHAR(7),CLZZRQ,23) CLZZRQ,SL
                                       	FROM HS_JHB NOLOCK
                                        WHERE {0}
                                        ) T1
                                        GROUP BY QCSCQY,CLXH,CLZZRQ,QYLX,USER_ID
                                        ORDER BY CLXH,CLZZRQ", sqlCondition);

            DataSet dsBasic = new DataSet();

            try
            {
                dsBasic = dbUtil.QuerySingleDS(sqlBasic);

            }
            catch (Exception ex)
            {
            }
            return dsBasic;
        }

        /// <summary>
        /// 根据条件查询数据平均油耗聚合参数
        /// </summary>
        /// <param name="userId">用户名</param>
        /// <param name="clxh">车辆型号</param>
        /// <param name="rllx">燃料类型</param>
        /// <param name="clzl">车辆种类</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">截止时间</param>
        /// <param name="timeType">时间类型（制造日期or上传日期）</param>
        /// <returns></returns>
        public DataSet QueryFuelCalData(string userId, string clxh, string rllx, string clzl, string startTime, string endTime, string timeType)
        {
            CalcDBHelper dbUtil = new CalcDBHelper();

            string sqlCondition = string.Format("CLZL='乘用车（M1）' and USER_ID='{0}'", userId);
            if (!string.IsNullOrEmpty(clxh))
            {
                sqlCondition += " AND CLXH LIKE '%" + clxh.Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(rllx))
            {
                sqlCondition += " AND RLLX='" + rllx.Trim() + "'";
            }

            // 默认搜索上报日期，若timeType为CLZZRQ则搜索车辆制造日期
            string timeField = "CLZZRQ";
            if (timeType == "CREATETIME")
            {
                timeField = "CLZZRQ";
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                sqlCondition += " AND " + timeField + ">='" + startTime.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sqlCondition += " AND " + timeField + "<'" + Convert.ToDateTime(endTime.Trim()).Add(new TimeSpan(24, 0, 0)).ToString() + "'";
            }

            string sqlBasic = string.Empty;

            sqlBasic = string.Format(@"SELECT QCSCQY,USER_ID,CLZL,RLLX,CLXH,
		                                    MAX(CAST(ISNULL(ZWPS,0) AS INT)) ZWPS, 
		                                    MIN(CAST(ISNULL(ZCZBZL,0) AS INT)) ZCZBZL, 
		                                    MAX(CAST(ISNULL(ZHGKRLXHL,0) AS DECIMAL(10, 1))) ZHGKRLXHL, 
		                                    CAST(SUM(SL) AS INT) SL
                                        FROM HS_JHB WHERE {0} 
                                        GROUP BY QCSCQY,USER_ID,CLZL,RLLX,CLXH
                                        ORDER BY QCSCQY,USER_ID,CLZL,RLLX,CLXH,ZWPS,ZCZBZL,ZHGKRLXHL", sqlCondition);

            DataSet dsBasic = new DataSet();

            try
            {
                dsBasic = dbUtil.QuerySingleDS(sqlBasic);

            }
            catch (Exception)
            {
            }
            return dsBasic;
        }

        /// <summary>
        /// 平均油耗及参与核算的参数明细
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clxh"></param>
        /// <param name="rllx"></param>
        /// <param name="clzl"></param>
        /// <param name="zczbzl"></param>
        /// <param name="bsqxs"></param>
        /// <param name="bsqdws"></param>
        /// <param name="zhgkxhl"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataSet QueryFuelCalDetails(string userId, string rllx, string clxh, string zczbzl, string zwps, string bsqxs, string zhgkxhl, string startTime, string endTime)
        {
            CalcDBHelper dbUtil = new CalcDBHelper();

            string sqlCondition = string.Format("CLZL='乘用车（M1）' and USER_ID='{0}'", userId);
            if (!string.IsNullOrEmpty(clxh))
            {
                sqlCondition += " AND CLXH LIKE '%" + clxh.Trim() + "%'";
            }
            if (!string.IsNullOrEmpty(rllx))
            {
                sqlCondition += " AND RLLX='" + rllx.Trim() + "'";
            }

            if (!string.IsNullOrEmpty(startTime))
            {
                sqlCondition += " AND CLZZRQ>='" + startTime.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sqlCondition += " AND CLZZRQ<'" + Convert.ToDateTime(endTime.Trim()).Add(new TimeSpan(24, 0, 0)).ToString() + "'";
            }

            string sqlBasic = string.Empty;

            sqlBasic = string.Format(@"SELECT QCSCQY,CLXH,QYID,T1.ZCZBZL,CASE T1.BSQXS WHEN 'OT' then '非手动' else T1.BSQXS end as BSQXS,T1.ZWPS,T1.ZHGKXSLC,
		        T1.ZHGKRLXHL_ACT,
		        T1.ZHKGRLXHL_TGT,
		        SUM(SL_ACT) SL_ACT, 
		        SUM(SL_TGT) SL_TGT,
		        T1.ZHGKRLXHL_ACT * SUM(SL_ACT) AS P_ACT,
		        T1.ZHKGRLXHL_TGT * SUM(SL_TGT) AS P_TGT
	        FROM 
	        (
		        SELECT T2.QCSCQY,T2.CLXH,T2.QYID,T2.ZCZBZL,T2.BSQXS,T2.ZWPS,T2.ZHGKXSLC,
			        CASE WHEN (T2.RLLX='纯电动' OR T2.RLLX='燃料电池' OR 
				        (T2.RLLX='插电式混合动力' AND T2.ZHGKXSLC>=50)) THEN 0.0
				        ELSE ZHGKRLXHL END ZHGKRLXHL_ACT, 
			        T3.TGT_ZHKGRLXHL ZHKGRLXHL_TGT,
			        CASE WHEN (T2.RLLX='纯电动' OR T2.RLLX='燃料电池' OR 
				        (T2.RLLX='插电式混合动力' AND T2.ZHGKXSLC>=50)) THEN SUM(T2.SL)*5
				         WHEN (T2.RLLX='汽油' OR T2.RLLX='柴油' OR T2.RLLX='柴油'  OR T2.RLLX='两用燃料'  OR T2.RLLX='双燃料' OR T2.RLLX='非插电式混合动力') 
				         AND T2.ZHGKRLXHL<=2.8 THEN SUM(T2.SL)*3
				        ELSE SUM(T2.SL) END SL_ACT,
			        SUM(T2.SL) AS SL_TGT
		        FROM 
		        (
			        SELECT QCSCQY,CLXH,QYID,RLLX,
				        CASE WHEN MAX(ISNULL(BSQXS,0))='MT' THEN 'MT' ELSE 'OT' END BSQXS,
				        CASE WHEN MAX(CAST(ISNULL(ZWPS,0) AS INT))<3 THEN 2 ELSE 3 END ZWPS,
				        MIN(CAST(ISNULL(ZCZBZL,0) AS INT)) ZCZBZL,
				        MAX(CAST(ISNULL(ZHGKXSLC,0) AS DECIMAL(10, 1))) ZHGKXSLC,
				        MAX(CAST(ISNULL(ZHGKRLXHL,0) AS DECIMAL(10, 1))) ZHGKRLXHL,
				        SUM(SL) SL
			        FROM HS_JHB
			        WHERE CLZL='乘用车（M1）' AND {0}
			        GROUP BY QYID,QCSCQY,CLXH,RLLX
		        ) T2,
		        (
			        SELECT [ZWPS],[BSQXS],[MIN_ZCZBZL],[MAX_ZCZBZL],[TGT_ZHKGRLXHL]
			        FROM TARGET_FUEL
		        ) T3
		        WHERE T2.ZWPS=T3.[ZWPS] AND T2.BSQXS=T3.BSQXS AND T2.ZCZBZL>T3.[MIN_ZCZBZL] AND T2.ZCZBZL<T3.[MAX_ZCZBZL]
		        GROUP BY T2.QYID,T2.QCSCQY,T2.CLXH,T2.RLLX,T2.ZCZBZL,T2.BSQXS,T2.ZWPS,T2.ZHGKXSLC,T2.ZHGKRLXHL,T3.TGT_ZHKGRLXHL
	        ) T1
	        GROUP BY QYID,QCSCQY,CLXH,T1.ZCZBZL,T1.BSQXS,T1.ZWPS,T1.ZHGKXSLC,T1.ZHGKRLXHL_ACT,T1.ZHKGRLXHL_TGT ", sqlCondition);

            DataSet dsBasic = new DataSet();

            try
            {
                dsBasic = dbUtil.QuerySingleDS(sqlBasic);

            }
            catch (Exception)
            {
            }
            return dsBasic;
        }

        /// <summary>
        /// 平均油耗
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clxh"></param>
        /// <param name="rllx"></param>
        /// <param name="clzl"></param>
        /// <param name="zczbzl"></param>
        /// <param name="bsqxs"></param>
        /// <param name="bsqdws"></param>
        /// <param name="zhgkxhl"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataSet QueryCAFC(string userId, string rllx, string clxh, string zczbzl, string zwps, string bsqxs, string zhgkxhl, string startTime, string endTime)
        {
            CalcDBHelper dbUtil = new CalcDBHelper();

            string sqlCondition = string.Format(" And USER_ID='{0}'", userId);
            
            if (!string.IsNullOrEmpty(startTime))
            {
                sqlCondition += " AND CLZZRQ>='" + startTime.Trim() + "'";
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                sqlCondition += " AND CLZZRQ<'" + Convert.ToDateTime(endTime.Trim()).Add(new TimeSpan(24, 0, 0)).ToString() + "'";
            }

            string sqlBasic = string.Empty;

            sqlBasic = string.Format(@"SELECT T.QCSCQY AS 汽车企业名称, T.QYLX AS 企业类型, 
		                                SUM(T.SL) AS 产量,
		                                SUM(T.P_ACT) / SUM(T.SL) AS CAFC,
		                                SUM(T.P_TGT) / SUM(T.SL) AS TCAFC,
		                                (SUM(T.P_TGT) / SUM(T.SL))*1.06 AS TCAFC106,
		                                (SUM(T.P_TGT) / SUM(T.SL))*1.09 AS TCAFC109
                                FROM
                                (
	                                SELECT T1.QCSCQY, CASE SUBSTRING(QYID,1,1) WHEN 'F' THEN '进口'ELSE '国产'END QYLX, 
			                                T1.CLXH,T1.ZWPS,T1.BSQXS, T1.ACT_ZHGKRLXHL,T2.TGT_ZHKGRLXHL, 
			                                SUM(T1.SL) AS SL,
			                                MAX(CAST(ISNULL(T1.ACT_ZHGKRLXHL,0) AS decimal(10, 1))) * SUM(SL) AS P_ACT,
			                                MAX(CAST(ISNULL(T2.TGT_ZHKGRLXHL,0) AS decimal(10, 1))) * SUM(SL) AS P_TGT
	                                FROM      
	                                (
		                                SELECT QCSCQY,CLXH,QYID,
				                                CASE WHEN MAX(ISNULL(BSQXS,0))='MT' THEN 'MT' ELSE 'OT' END BSQXS,
				                                CASE WHEN MAX(CAST(ISNULL(ZWPS,0) AS INT))<3 THEN 2 ELSE 3 END ZWPS,
				                                MIN(CAST(ISNULL(ZCZBZL,0) AS INT)) ZCZBZL,
				                                MAX(CAST(ISNULL(ZHGKRLXHL,0) AS decimal(10, 1))) ACT_ZHGKRLXHL,SUM(SL) SL 
		                                FROM HS_JHB
		                                WHERE CLZL='乘用车（M1）' AND (RLLX='汽油' OR RLLX='柴油' OR RLLX='双燃料' OR RLLX='两用燃料' OR RLLX='非插电式混合动力') {0}
			
		                                GROUP BY QYID,QCSCQY,CLXH 
	                                ) T1,
	                                (
		                                SELECT [ZWPS],[BSQXS],[MIN_ZCZBZL],[MAX_ZCZBZL],[TGT_ZHKGRLXHL]
		                                FROM TARGET_FUEL
	                                ) T2
	                                WHERE T1.ZWPS=T2.[ZWPS] AND T2.BSQXS=T1.BSQXS AND T1.ZCZBZL>T2.[MIN_ZCZBZL] AND T1.ZCZBZL<T2.[MAX_ZCZBZL]
	                                GROUP BY T1.CLXH,T1.QCSCQY,QYID,T1.ZWPS,T1.BSQXS, T1.ACT_ZHGKRLXHL,T2.TGT_ZHKGRLXHL
                                ) T
                                GROUP BY T.QCSCQY,T.QYLX
                                ORDER BY T.QYLX,T.QCSCQY", sqlCondition);

            DataSet dsBasic = new DataSet();

            try
            {
                dsBasic = dbUtil.QuerySingleDS(sqlBasic);

            }
            catch (Exception)
            {
            }
            return dsBasic;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        public DataTable QueryExistParam(string userId)
        {
            string sql = string.Format(@"SELECT QCSCQY,CLXH,RLLX,
				                            CASE WHEN MAX(ISNULL(BSQXS,0))='MT' THEN 'MT' ELSE 'OT' END BSQXS,
				                            CASE WHEN MAX(CAST(ISNULL(ZWPS,0) AS INT))<3 THEN 2 ELSE 3 END ZWPS,
				                            MIN(CAST(ISNULL(ZCZBZL,0) AS INT)) ZCZBZL,
				                            MAX(CAST(ISNULL(ZHGKRLXHL,0) AS DECIMAL(10, 1))) ZHGKRLXHL,
				                            MAX(CAST(ISNULL(ZHGKRLXHL,0) AS DECIMAL(10, 1))) ZHGKRLXHL,
				                            SUM(SL) SL
			                            FROM HS_JHB
			                            WHERE CLZL='乘用车（M1）' AND  USER_ID='{0}' 
			                            GROUP BY QCSCQY,CLXH,RLLX
					                    ORDER BY QCSCQY,CLXH,RLLX,BSQXS,ZWPS,ZCZBZL,ZHGKRLXHL", userId);

            CalcDBHelper dbUtil = new CalcDBHelper();
            DataTable dtBasic = new DataTable();

            try
            {
                dtBasic = dbUtil.QuerySingleDT(sql);

            }
            catch (Exception)
            {
            }

            return dtBasic;
        }

        public int SaveForecastData(string calcId, string clxh, string rllx, string bsqxs, int zwps, int zczbzl, int jksl, double zhgkrlxhl)
        {
            int count=0;
            string sql = string.Format(@"INSERT INTO HS_FORECAST 
                                         (
                                            [CALC_ID],[QCSCQY],[CLXH]
                                            ,[RLLX],[BSQXS],[ZWPS]
                                            ,[ZCZBZL],[ZHGKRLXHL],[JKSL]
                                          )
                                          VALUES('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8})
                                       ", calcId,Utils.qymc, clxh, rllx, bsqxs, zwps, zczbzl, zhgkrlxhl, jksl);

            CalcDBHelper dbUtil = new CalcDBHelper();

            try
            {
                count = dbUtil.ExecuteSql(sql);

            }
            catch (Exception)
            {
            }

            return count;
        }

        /// <summary>
        /// 平均油耗
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clxh"></param>
        /// <param name="rllx"></param>
        /// <param name="clzl"></param>
        /// <param name="zczbzl"></param>
        /// <param name="bsqxs"></param>
        /// <param name="bsqdws"></param>
        /// <param name="zhgkxhl"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public DataSet QueryForeCastCAFC()
        {
            CalcDBHelper dbUtil = new CalcDBHelper();

            string sqlBasic = string.Empty;

            sqlBasic = string.Format(@"SELECT 	 T.QCSCQY,SUM(T.SL) AS SL,
		                                SUM(T.P_ACT) / SUM(T.SL) AS CAFC,
		                                SUM(T.P_TGT) / SUM(T.SL) AS TCAFC,
		                                (SUM(T.P_TGT) / SUM(T.SL))*1.06 AS TCAFC106,
		                                (SUM(T.P_TGT) / SUM(T.SL))*1.09 AS TCAFC109
                                FROM
                                (
	                                SELECT 	T1.QCSCQY,T1.CLXH,T1.ZWPS,T1.BSQXS, T1.ACT_ZHGKRLXHL,T2.TGT_ZHKGRLXHL, 
			                                SUM(T1.SL) AS SL,
			                                MAX(CAST(ISNULL(T1.ACT_ZHGKRLXHL,0) AS decimal(10, 1))) * SUM(SL) AS P_ACT,
			                                MAX(CAST(ISNULL(T2.TGT_ZHKGRLXHL,0) AS decimal(10, 1))) * SUM(SL) AS P_TGT
	                                FROM      
	                                (
		                                SELECT QCSCQY,CLXH,
				                                CASE WHEN MAX(ISNULL(BSQXS,0))='MT' THEN 'MT' ELSE 'OT' END BSQXS,
				                                CASE WHEN MAX(CAST(ISNULL(ZWPS,0) AS INT))<3 THEN 2 ELSE 3 END ZWPS,
				                                MIN(CAST(ISNULL(ZCZBZL,0) AS INT)) ZCZBZL,
				                                MAX(CAST(ISNULL(ZHGKRLXHL,0) AS decimal(10, 1))) ACT_ZHGKRLXHL,SUM(JKSL) SL 
		                                FROM HS_FORECAST
		                                WHERE (RLLX='汽油' OR RLLX='柴油' OR RLLX='双燃料' OR RLLX='两用燃料' OR RLLX='非插电式混合动力')
			
		                                GROUP BY QCSCQY,CLXH 
	                                ) T1,
	                                (
		                                SELECT [ZWPS],[BSQXS],[MIN_ZCZBZL],[MAX_ZCZBZL],[TGT_ZHKGRLXHL]
		                                FROM TARGET_FUEL
	                                ) T2
	                                WHERE T1.ZWPS=T2.[ZWPS] AND T2.BSQXS=T1.BSQXS AND T1.ZCZBZL>T2.[MIN_ZCZBZL] AND T1.ZCZBZL<T2.[MAX_ZCZBZL]
	                                GROUP BY T1.QCSCQY,T1.CLXH,T1.ZWPS,T1.BSQXS, T1.ACT_ZHGKRLXHL,T2.TGT_ZHKGRLXHL
                                ) T

								GROUP BY QCSCQY");

            DataSet dsBasic = new DataSet();

            try
            {
                dsBasic = dbUtil.QuerySingleDS(sqlBasic);

            }
            catch (Exception)
            {
            }
            return dsBasic;
        }


        public DataTable QueryForecastParam()
        {
            CalcDBHelper dbUtil = new CalcDBHelper();

            string sqlBasic = string.Empty;

            sqlBasic = string.Format(@"select * from HS_FORECAST");

            DataTable dsBasic = new DataTable();

            try
            {
                dsBasic = dbUtil.QuerySingleDT(sqlBasic);

            }
            catch (Exception)
            {
            }
            return dsBasic;
        }

        /// <summary>
        /// 获取选中的数据
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<string> GetSelectedItem<T>(T[] tArr) where T:new()
        {
            List<string> itemList = new List<string>();
            PropertyInfo[] property = null;

            T obj = new T();
            if (property == null)
            {
                property = obj.GetType().GetProperties();
            }

            try
            {
                if (tArr != null)
                {
                    foreach (T t in tArr)
                    {
                        string prjId = string.Empty;
                        bool check=false;
                        foreach (PropertyInfo pr in property)
                        {
                            //if (pr.Name == "Prj_Id")
                            //{
                            //    if (check)
                            //    {
                            //        prjId = Convert.ToString(pr.GetValue(t, null));
                            //    }
                            //}
                            if (pr.Name == "Check")
                            {
                               check=(bool)pr.GetValue(t, null);
                            }
                            
                        }

                        foreach (PropertyInfo pr in property)
                        {
                            if (pr.Name == "Prj_Id")
                            {
                                if (check)
                                {
                                    prjId = Convert.ToString(pr.GetValue(t, null));
                                }
                            }
                            
                        }
                    }
                    //for (int i = 0; i < dt.Rows.Count; i++)
                    //{
                    //    if ((bool)dt.Rows[i]["check"])
                    //    {
                    //        itemList.Add(dt.Rows[i]["Prj_Id"].ToString());
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return itemList;
        }
    }
}
