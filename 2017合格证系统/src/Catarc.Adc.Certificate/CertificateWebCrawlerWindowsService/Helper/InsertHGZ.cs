using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace CertificateWebCrawlerWindowsService.Helper
{
    public class InsertHGZ
    {
        readonly SqlHelper sqlHelepr = new SqlHelper();

        /// <summary>
        /// 插入机动车合格证申请List库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertListHGZ(DataTable dt)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var listSQBH = dt.AsEnumerable().Select(d => d.Field<string>("SQBH")).ToList<string>();
                while (listSQBH.Count > 0)
                {
                    var guidArrSkip = listSQBH.Take(1000);
                    stringBuilder.AppendFormat(" or SQBH in('{0}')", string.Join("','", guidArrSkip));
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
                    string sql = string.Format("DELETE LIST_HGZ WHERE 1=1 AND ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    sqlHelepr.ExecuteNonQuery(SqlHelper.connectionString, sql);
                    sqlHelepr.CopyToServer(SqlHelper.connectionString, "LIST_HGZ", dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        /// <summary>
        /// 插入机动车合格证申请Details库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertDetailsHGZ(DataTable dt)
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
                            string sqlDelete = string.Format("DELETE DETAILS_HGZ WHERE SQXLH='{0}'", dt.Rows[i]["SQXLH"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlDelete);
                            string sqlInsert = String.Format("INSERT INTO DETAILS_HGZ (SQXLH,CLZTXX,HGZBH,CLSBDH,PZXLH,FZRQ,CLZZQYMC,CLLX,CLMC,CLPP,CLXH,CSYS,DPXH,DPID,DPHGZBH,CJH,FDJXH,FDJH,RLZL,PFBZ,PL,GL,ZXXS,QLJ,HLJ,LTS,LTGG,GBTHPS,ZJ,ZH,ZS,WKC,WKK,WKG,HXNBC,HXNBK,HXNBG,ZZL,EDZZL,ZBZL,ZZLLYXS,ZQYZZL,EDZK,BGCAZZDYXZZL,JSSZCRS,ZXZS,ZGSJCS,CLZZRQ,BZ,QYBZ,CLSCDWMC,CPSCDZ,QYQTXX,YH,CPH,PC,GGSXRQ,SCSJ,CREATETIME,UPDATETIME) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}','{46}','{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}')", dt.Rows[i]["SQXLH"], dt.Rows[i]["CLZTXX"], dt.Rows[i]["HGZBH"], dt.Rows[i]["CLSBDH"], dt.Rows[i]["PZXLH"], dt.Rows[i]["FZRQ"], dt.Rows[i]["CLZZQYMC"], dt.Rows[i]["CLLX"], dt.Rows[i]["CLMC"], dt.Rows[i]["CLPP"], dt.Rows[i]["CLXH"], dt.Rows[i]["CSYS"], dt.Rows[i]["DPXH"], dt.Rows[i]["DPID"], dt.Rows[i]["DPHGZBH"], dt.Rows[i]["CJH"], dt.Rows[i]["FDJXH"], dt.Rows[i]["FDJH"], dt.Rows[i]["RLZL"], dt.Rows[i]["PFBZ"], dt.Rows[i]["PL"], dt.Rows[i]["GL"], dt.Rows[i]["ZXXS"], dt.Rows[i]["QLJ"], dt.Rows[i]["HLJ"], dt.Rows[i]["LTS"], dt.Rows[i]["LTGG"], dt.Rows[i]["GBTHPS"], dt.Rows[i]["ZJ"], dt.Rows[i]["ZH"], dt.Rows[i]["ZS"], dt.Rows[i]["WKC"], dt.Rows[i]["WKK"], dt.Rows[i]["WKG"], dt.Rows[i]["HXNBC"], dt.Rows[i]["HXNBK"], dt.Rows[i]["HXNBG"], dt.Rows[i]["ZZL"], dt.Rows[i]["EDZZL"], dt.Rows[i]["ZBZL"], dt.Rows[i]["ZZLLYXS"], dt.Rows[i]["ZQYZZL"], dt.Rows[i]["EDZK"], dt.Rows[i]["BGCAZZDYXZZL"], dt.Rows[i]["JSSZCRS"], dt.Rows[i]["ZXZS"], dt.Rows[i]["ZGSJCS"], dt.Rows[i]["CLZZRQ"], dt.Rows[i]["BZ"], dt.Rows[i]["QYBZ"], dt.Rows[i]["CLSCDWMC"], dt.Rows[i]["CPSCDZ"], dt.Rows[i]["QYQTXX"], dt.Rows[i]["YH"], dt.Rows[i]["CPH"], dt.Rows[i]["PC"], dt.Rows[i]["GGSXRQ"], dt.Rows[i]["SCSJ"], dt.Rows[i]["CREATETIME"], dt.Rows[i]["UPDATETIME"]);
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
        /// 插入机动车合格证申请信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertDBHGZ(DataTable dt)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                var listSQBH = dt.AsEnumerable().Select(d => d.Field<string>("SQXLH")).ToList<string>();
                while (listSQBH.Count > 0)
                {
                    var guidArrSkip = listSQBH.Take(1000);
                    stringBuilder.AppendFormat(" or SQXLH in('{0}')", string.Join("','", guidArrSkip));
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
                    string sql = string.Format("DELETE DB_HGZ WHERE 1=1 AND ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    sqlHelepr.ExecuteNonQuery(SqlHelper.connectionString, sql);
                    sqlHelepr.CopyToServer(SqlHelper.connectionString, "DB_HGZ", dt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }



        /// <summary>
        /// 插入机动车合格证申请信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertSingleDBHGZ(DataTable dt)
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
                            string sqlDelete = string.Format("DELETE DB_HGZ WHERE SQXLH='{0}'", dt.Rows[i]["SQXLH"]);
                            sqlHelepr.ExecuteNonQuery(transaction, sqlDelete);
                            SqlParameter[] param = { 
                                                       new SqlParameter("@SQXLH",dt.Rows[i]["SQXLH"])
                                                      ,new SqlParameter("@CLZTXX",dt.Rows[i]["CLZTXX"])
                                                      ,new SqlParameter("@HGZBH",dt.Rows[i]["HGZBH"])
                                                      ,new SqlParameter("@CLSBDH",dt.Rows[i]["CLSBDH"])
                                                      ,new SqlParameter("@PZXLH",dt.Rows[i]["PZXLH"])
                                                      ,new SqlParameter("@FZRQ",string.IsNullOrEmpty(dt.Rows[i]["FZRQ"].ToString())==true?DBNull.Value:dt.Rows[i]["FZRQ"])
                                                      ,new SqlParameter("@CLZZQYMC",dt.Rows[i]["CLZZQYMC"])
                                                      ,new SqlParameter("@CLLX",dt.Rows[i]["CLLX"])
                                                      ,new SqlParameter("@CLMC",dt.Rows[i]["CLMC"])
                                                      ,new SqlParameter("@CLPP",dt.Rows[i]["CLPP"])
                                                      ,new SqlParameter("@CLXH",dt.Rows[i]["CLXH"])
                                                      ,new SqlParameter("@CSYS",dt.Rows[i]["CSYS"])
                                                      ,new SqlParameter("@DPXH",dt.Rows[i]["DPXH"])
                                                      ,new SqlParameter("@DPID",dt.Rows[i]["DPID"])
                                                      ,new SqlParameter("@DPHGZBH",dt.Rows[i]["DPHGZBH"])
                                                      ,new SqlParameter("@CJH",dt.Rows[i]["CJH"])
                                                      ,new SqlParameter("@FDJXH",dt.Rows[i]["FDJXH"])
                                                      ,new SqlParameter("@FDJH",dt.Rows[i]["FDJH"])
                                                      ,new SqlParameter("@RLZL",dt.Rows[i]["RLZL"])
                                                      ,new SqlParameter("@PFBZ",dt.Rows[i]["PFBZ"])
                                                      ,new SqlParameter("@PL",dt.Rows[i]["PL"])
                                                      ,new SqlParameter("@GL",dt.Rows[i]["GL"])
                                                      ,new SqlParameter("@ZXXS",dt.Rows[i]["ZXXS"])
                                                      ,new SqlParameter("@QLJ",dt.Rows[i]["QLJ"])
                                                      ,new SqlParameter("@HLJ",dt.Rows[i]["HLJ"])
                                                      ,new SqlParameter("@LTS",dt.Rows[i]["LTS"])
                                                      ,new SqlParameter("@LTGG",dt.Rows[i]["LTGG"])
                                                      ,new SqlParameter("@GBTHPS",dt.Rows[i]["GBTHPS"])
                                                      ,new SqlParameter("@ZJ",dt.Rows[i]["ZJ"])
                                                      ,new SqlParameter("@ZH",dt.Rows[i]["ZH"])
                                                      ,new SqlParameter("@ZS",dt.Rows[i]["ZS"])
                                                      ,new SqlParameter("@WKC",dt.Rows[i]["WKC"])
                                                      ,new SqlParameter("@WKK",dt.Rows[i]["WKK"])
                                                      ,new SqlParameter("@WKG",dt.Rows[i]["WKG"])
                                                      ,new SqlParameter("@HXNBC",dt.Rows[i]["HXNBC"])
                                                      ,new SqlParameter("@HXNBK",dt.Rows[i]["HXNBK"])
                                                      ,new SqlParameter("@HXNBG",dt.Rows[i]["HXNBG"])
                                                      ,new SqlParameter("@ZZL",dt.Rows[i]["ZZL"])
                                                      ,new SqlParameter("@EDZZL",dt.Rows[i]["EDZZL"])
                                                      ,new SqlParameter("@ZBZL",dt.Rows[i]["ZBZL"])
                                                      ,new SqlParameter("@ZZLLYXS",dt.Rows[i]["ZZLLYXS"])
                                                      ,new SqlParameter("@ZQYZZL",dt.Rows[i]["ZQYZZL"])
                                                      ,new SqlParameter("@EDZK",dt.Rows[i]["EDZK"])
                                                      ,new SqlParameter("@BGCAZZDYXZZL",dt.Rows[i]["BGCAZZDYXZZL"])
                                                      ,new SqlParameter("@JSSZCRS",dt.Rows[i]["JSSZCRS"])
                                                      ,new SqlParameter("@ZXZS",dt.Rows[i]["ZXZS"])
                                                      ,new SqlParameter("@ZGSJCS",dt.Rows[i]["ZGSJCS"])
                                                      ,new SqlParameter("@CLZZRQ",string.IsNullOrEmpty(dt.Rows[i]["CLZZRQ"].ToString())==true?DBNull.Value:dt.Rows[i]["CLZZRQ"])
                                                      ,new SqlParameter("@BZ",dt.Rows[i]["BZ"])
                                                      ,new SqlParameter("@QYBZ",dt.Rows[i]["QYBZ"])
                                                      ,new SqlParameter("@CLSCDWMC",dt.Rows[i]["CLSCDWMC"])
                                                      ,new SqlParameter("@CPSCDZ",dt.Rows[i]["CPSCDZ"])
                                                      ,new SqlParameter("@QYQTXX",dt.Rows[i]["QYQTXX"])
                                                      ,new SqlParameter("@YH",dt.Rows[i]["YH"])
                                                      ,new SqlParameter("@CPH",dt.Rows[i]["CPH"])
                                                      ,new SqlParameter("@PC",dt.Rows[i]["PC"])
                                                      ,new SqlParameter("@GGSXRQ",string.IsNullOrEmpty(dt.Rows[i]["GGSXRQ"].ToString())==true?DBNull.Value:dt.Rows[i]["GGSXRQ"])
                                                      ,new SqlParameter("@SCSJ",string.IsNullOrEmpty(dt.Rows[i]["SCSJ"].ToString())==true?DBNull.Value:dt.Rows[i]["SCSJ"])
                                                      ,new SqlParameter("@APP_TIME",string.IsNullOrEmpty(dt.Rows[i]["APP_TIME"].ToString())==true?DBNull.Value:dt.Rows[i]["APP_TIME"])
                                                      ,new SqlParameter("@APP_TYPE",dt.Rows[i]["APP_TYPE"])
                                                      ,new SqlParameter("@CREATETIME",string.IsNullOrEmpty(dt.Rows[i]["CREATETIME"].ToString())==true?DBNull.Value:dt.Rows[i]["CREATETIME"])
                                                      ,new SqlParameter("@UPDATETIME",string.IsNullOrEmpty(dt.Rows[i]["UPDATETIME"].ToString())==true?DBNull.Value:dt.Rows[i]["UPDATETIME"])
                                                   };
                            sqlHelepr.ExecuteNonQuery(transaction, (string)"INSERT INTO DB_HGZ (SQXLH,CLZTXX,HGZBH,CLSBDH,PZXLH,FZRQ,CLZZQYMC,CLLX,CLMC,CLPP,CLXH,CSYS,DPXH,DPID,DPHGZBH,CJH,FDJXH,FDJH,RLZL,PFBZ,PL,GL,ZXXS,QLJ,HLJ,LTS,LTGG,GBTHPS,ZJ,ZH,ZS,WKC,WKK,WKG,HXNBC,HXNBK,HXNBG,ZZL,EDZZL,ZBZL,ZZLLYXS,ZQYZZL,EDZK,BGCAZZDYXZZL,JSSZCRS,ZXZS,ZGSJCS,CLZZRQ,BZ,QYBZ,CLSCDWMC,CPSCDZ,QYQTXX,YH,CPH,PC,GGSXRQ,SCSJ,APP_TIME,APP_TYPE,CREATETIME,UPDATETIME) VALUES (@SQXLH,@CLZTXX,@HGZBH,@CLSBDH,@PZXLH,@FZRQ,@CLZZQYMC,@CLLX,@CLMC,@CLPP,@CLXH,@CSYS,@DPXH,@DPID,@DPHGZBH,@CJH,@FDJXH,@FDJH,@RLZL,@PFBZ,@PL,@GL,@ZXXS,@QLJ,@HLJ,@LTS,@LTGG,@GBTHPS,@ZJ,@ZH,@ZS,@WKC,@WKK,@WKG,@HXNBC,@HXNBK,@HXNBG,@ZZL,@EDZZL,@ZBZL,@ZZLLYXS,@ZQYZZL,@EDZK,@BGCAZZDYXZZL,@JSSZCRS,@ZXZS,@ZGSJCS,@CLZZRQ,@BZ,@QYBZ,@CLSCDWMC,@CPSCDZ,@QYQTXX,@YH,@CPH,@PC,@GGSXRQ,@SCSJ,@APP_TIME,@APP_TYPE,@CREATETIME,@UPDATETIME)", param);
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
        /// 百万级插入机动车合格证申请信息DB库
        /// </summary>
        /// <param name="dt">要插入的数据</param>
        public void InsertTvpDBHGZ(DataTable dt)
        {
            try
            {
                List<string> colList = new List<string>();
                colList.Add("SQXLH");
                sqlHelepr.TvpDeleteDB(SqlHelper.connectionString, "DB_HGZ", "TEST_HGZType", dt, colList);
                sqlHelepr.TvpInsertDB(SqlHelper.connectionString, "DB_HGZ", "TEST_HGZType", dt);
            }
            catch (Exception ex)
            {
                throw new Exception(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

    }
}
