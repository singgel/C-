using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace CertificateWebCrawlerSys.Helper
{
    public class InsertWS
    {
        readonly SqlHelper sqlHelepr = new SqlHelper();

        /// <summary>
        /// 插入完税信息List库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertListWS(DataTable dt)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var listSQBH = dt.AsEnumerable().Select(d => d.Field<string>("RESOURCE_ID")).ToList<string>();
                while (listSQBH.Count > 0)
                {
                    var guidArrSkip = listSQBH.Take(1000);
                    stringBuilder.AppendFormat(" or RESOURCE_ID in('{0}')", string.Join("','", guidArrSkip));
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
                    string sql = string.Format("DELETE LIST_WS WHERE 1=1 AND ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    sqlHelepr.ExecuteNonQuery(SqlHelper.connectionString, sql);
                    sqlHelepr.CopyToServer(SqlHelper.connectionString, "LIST_WS", dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// 插入完税信息Details库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertDetailsWS(DataTable dt)
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
                            string sqlDelete = string.Format("DELETE DETAILS_WS WHERE RESOURCE_ID='{0}'", dt.Rows[i]["RESOURCE_ID"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlDelete);
                            string sqlInsert = String.Format("INSERT INTO DETAILS_WS (RESOURCE_ID,HGZBH,SWSBM,CJSJ,CREATETIME,UPDATETIME) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", dt.Rows[i]["RESOURCE_ID"], dt.Rows[i]["HGZBH"], dt.Rows[i]["SWSBM"], dt.Rows[i]["CJSJ"], dt.Rows[i]["CREATETIME"], dt.Rows[i]["UPDATETIME"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlInsert);
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex) {
                        transaction.Rollback();
                        throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
                    }
                }
            }
        }

        /// <summary>
        /// 插入完税信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertDBWS(DataTable dt)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var listSQBH = dt.AsEnumerable().Select(d => d.Field<string>("RESOURCE_ID")).ToList<string>();
                while (listSQBH.Count > 0)
                {
                    var guidArrSkip = listSQBH.Take(1000);
                    stringBuilder.AppendFormat(" or RESOURCE_ID in('{0}')", string.Join("','", guidArrSkip));
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
                    string sql = string.Format("DELETE DB_WS WHERE 1=1 AND ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    sqlHelepr.ExecuteNonQuery(SqlHelper.connectionString, sql);
                    sqlHelepr.CopyToServer(SqlHelper.connectionString, "DB_WS", dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// 插入完税信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertSingleDBWS(DataTable dt)
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
                            string sqlDelete = string.Format("DELETE DB_WS WHERE RESOURCE_ID='{0}'", dt.Rows[i]["RESOURCE_ID"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlDelete);
                            SqlParameter[] param = { 
                                                       new SqlParameter("@RESOURCE_ID",dt.Rows[i]["RESOURCE_ID"])
                                                      ,new SqlParameter("@HGZBH",dt.Rows[i]["HGZBH"])
                                                      ,new SqlParameter("@SWSBM",dt.Rows[i]["SWSBM"])
                                                      ,new SqlParameter("@CJSJ",string.IsNullOrEmpty(dt.Rows[i]["CJSJ"].ToString())==true?DBNull.Value:dt.Rows[i]["CJSJ"])
                                                      ,new SqlParameter("@CREATETIME",string.IsNullOrEmpty(dt.Rows[i]["CREATETIME"].ToString())==true?DBNull.Value:dt.Rows[i]["CREATETIME"])
                                                      ,new SqlParameter("@UPDATETIME",string.IsNullOrEmpty(dt.Rows[i]["UPDATETIME"].ToString())==true?DBNull.Value:dt.Rows[i]["UPDATETIME"])
                                                   };
                            sqlHelepr.ExecuteNonQuery(transaction, (string)"INSERT INTO DB_WS (RESOURCE_ID,HGZBH,SWSBM,CJSJ,CREATETIME,UPDATETIME) VALUES (@RESOURCE_ID,@HGZBH,@SWSBM,@CJSJ,@CREATETIME,@UPDATETIME)", param);
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
        /// 百万级插入完税信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertTvpDBWS(DataTable dt)
        {
            try
            {
                List<string> colList = new List<string>();
                colList.Add("RESOURCE_ID");
                sqlHelepr.TvpDeleteDB(SqlHelper.connectionString, "DB_WS", "TEST_WSType", dt, colList);
                sqlHelepr.TvpInsertDB(SqlHelper.connectionString, "DB_WS", "TEST_WSType", dt);
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }
    }
}
