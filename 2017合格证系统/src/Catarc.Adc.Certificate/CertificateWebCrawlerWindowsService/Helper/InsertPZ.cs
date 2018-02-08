using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace CertificateWebCrawlerWindowsService.Helper
{
    public class InsertPZ
    {
        readonly SqlHelper sqlHelepr = new SqlHelper();

        /// <summary>
        /// 插入配置信息List库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertListPZ(DataTable dt)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var listSQBH = dt.AsEnumerable().Select(d => d.Field<string>("PZXLH")).ToList<string>();
                while (listSQBH.Count > 0)
                {
                    var guidArrSkip = listSQBH.Take(1000);
                    stringBuilder.AppendFormat(" or PZXLH in('{0}')", string.Join("','", guidArrSkip));
                    if (listSQBH.Count > 999)
                    {
                        listSQBH.RemoveRange(0, 999);
                    }
                    else
                    {
                        listSQBH.RemoveRange(0, listSQBH.Count);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    string sql = string.Format("DELETE LIST_PZ WHERE 1=1 AND ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    sqlHelepr.ExecuteNonQuery(SqlHelper.connectionString, sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// 插入配置信息Details库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertDetailsPZ(DataTable dt)
        {
            using (var con = new SqlConnection(SqlHelper.connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string sqlDelete = string.Format("DELETE DETAILS_PZ WHERE PZXLH='{0}'", dt.Rows[i]["PZXLH"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlDelete);
                            string sqlInsert = String.Format("INSERT INTO DETAILS_PZ (PZXLH,CLLX,QYDM,HGZQYMC,QYMC,CPXH,DW,ZW,PQL,GB,JBPZ,CJSJ,GXSJ,CREATETIME,UPDATETIME) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", dt.Rows[i]["PZXLH"], dt.Rows[i]["CLLX"], dt.Rows[i]["QYDM"], dt.Rows[i]["HGZQYMC"], dt.Rows[i]["QYMC"], dt.Rows[i]["CPXH"], dt.Rows[i]["DW"], dt.Rows[i]["ZW"], dt.Rows[i]["PQL"], dt.Rows[i]["GB"], dt.Rows[i]["JBPZ"], dt.Rows[i]["CJSJ"], dt.Rows[i]["GXSJ"], dt.Rows[i]["CREATETIME"], dt.Rows[i]["UPDATETIME"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlInsert);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 插入配置信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据源</param>
        public void InsertDBPZ(DataTable dt)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var listSQBH = dt.AsEnumerable().Select(d => d.Field<string>("PZXLH")).ToList<string>();
                while (listSQBH.Count > 0)
                {
                    var guidArrSkip = listSQBH.Take(1000);
                    stringBuilder.AppendFormat(" or PZXLH in('{0}')", string.Join("','", guidArrSkip));
                    if (listSQBH.Count > 999)
                    {
                        listSQBH.RemoveRange(0, 999);
                    }
                    else
                    {
                        listSQBH.RemoveRange(0, listSQBH.Count);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    string sql = string.Format("DELETE DB_PZ WHERE 1=1 AND ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    sqlHelepr.ExecuteNonQuery(SqlHelper.connectionString, sql);
                    sqlHelepr.CopyToServer(SqlHelper.connectionString, "DB_PZ", dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// 插入配置信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据源</param>
        public void InsertSingleDBPZ(DataTable dt)
        {
            using (var con = new SqlConnection(SqlHelper.connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string sqlDelete = string.Format("DELETE DB_PZ WHERE PZXLH='{0}'", dt.Rows[i]["PZXLH"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlDelete);
                            SqlParameter[] param = { 
                                                       new SqlParameter("@PZXLH",dt.Rows[i]["PZXLH"])
                                                      ,new SqlParameter("@CLLX",dt.Rows[i]["CLLX"])
                                                      ,new SqlParameter("@QYDM",dt.Rows[i]["QYDM"])
                                                      ,new SqlParameter("@HGZQYMC",dt.Rows[i]["HGZQYMC"])
                                                      ,new SqlParameter("@QYMC",dt.Rows[i]["QYMC"])
                                                      ,new SqlParameter("@CPXH",dt.Rows[i]["CPXH"])
                                                      ,new SqlParameter("@DW",dt.Rows[i]["DW"])
                                                      ,new SqlParameter("@ZW",dt.Rows[i]["ZW"])
                                                      ,new SqlParameter("@PQL",dt.Rows[i]["PQL"])
                                                      ,new SqlParameter("@GB",dt.Rows[i]["GB"])
                                                      ,new SqlParameter("@JBPZ",dt.Rows[i]["JBPZ"])
                                                      ,new SqlParameter("@CJSJ",string.IsNullOrEmpty(dt.Rows[i]["CJSJ"].ToString())==true?DBNull.Value:dt.Rows[i]["CJSJ"])
                                                      ,new SqlParameter("@GXSJ",string.IsNullOrEmpty(dt.Rows[i]["GXSJ"].ToString())==true?DBNull.Value:dt.Rows[i]["GXSJ"])
                                                      ,new SqlParameter("@CREATETIME",string.IsNullOrEmpty(dt.Rows[i]["CREATETIME"].ToString())==true?DBNull.Value:dt.Rows[i]["CREATETIME"])
                                                      ,new SqlParameter("@UPDATETIME",string.IsNullOrEmpty(dt.Rows[i]["UPDATETIME"].ToString())==true?DBNull.Value:dt.Rows[i]["UPDATETIME"])
                                                   };
                            sqlHelepr.ExecuteNonQuery(transaction, (string)"INSERT INTO DB_PZ (PZXLH,CLLX,QYDM,HGZQYMC,QYMC,CPXH,DW,ZW,PQL,GB,JBPZ,CJSJ,GXSJ,CREATETIME,UPDATETIME) VALUES (@PZXLH,@CLLX,@QYDM,@HGZQYMC,@QYMC,@CPXH,@DW,@ZW,@PQL,@GB,@JBPZ,@CJSJ,@GXSJ,@CREATETIME,@UPDATETIME)", param);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 百万级插入配置信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertTvpDBPZ(DataTable dt)
        {
            try
            {
                List<string> colList = new List<string>();
                colList.Add("PZXLH");
                sqlHelepr.TvpDeleteDB(SqlHelper.connectionString, "DB_PZ", "TEST_PZType", dt, colList);
                sqlHelepr.TvpInsertDB(SqlHelper.connectionString, "DB_PZ", "TEST_PZType", dt);
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}
