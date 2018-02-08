using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Threading;

namespace CertificateInsert
{
    public class Helper
    {
        public bool IsStopped;
        int id;
        protected string connectionString = "server=192.168.5.58;Initial Catalog=CertificateData;Persist Security Info=True;uid=catarc;pwd=00x0!@#; Connect Timeout=3000";

        //开始监控
        public void Start(int Id)
        {
            this.id = Id;
            IsStopped = true;
            try
            {
                Thread thAutoQueryTable;//自动发送整个Table数据线程

                thAutoQueryTable = new Thread(new ThreadStart(InsertData));  //获取线程
                thAutoQueryTable.IsBackground = true;
                thAutoQueryTable.Start();
            }
            catch (System.Exception ex)
            {
                IsStopped = false;
                throw ex;
            }
        }

        public void InsertData()
        {
            while (true)
            {
                if (IsStopped)
                {
                    try
                    {

                        #region sql

                        string sql = @"INSERT INTO [CertificateData].[dbo].[HGZ_APPLIC]([H_ID],[QYID_BJ],[CLZTXX],[ZCHGZBH],[WZHGZBH],[DPHGZBH],[FZRQ],[CLZZQYMC],[QYID],[CLLX],[CLMC],[CLPP],[CLXH],[CSYS],[DPXH],[DPID],[CLSBDH],[CJH],[FDJH],[FDJXH],[RLZL],[PFBZ],[PL],[GL],[ZXXS],[QLJ],[HLJ],[LTS],[LTGG],[GBTHPS],[ZJ],[ZH],[ZS],[WKC],[WKK],[WKG],[HXNBC],[HXNBK],[HXNBG],[ZZL],[EDZZL],[ZBZL],[ZZLLYXS],[ZQYZZL],[EDZK],[BGCAZZDYXZZL],[JSSZCRS],[QZDFS],[HZDFS],[QZDCZFS],[HZDCZFS],[ZGCS],[CLZZRQ],[BZ],[QYBZ],[CPSCDZ],[QYQTXX],[CZRQ],[CREATETIME],[UPDATETIME],[HD_USER],[CLSCDWMC],[YH],[ZXZS],[CDDBJ],[VERCODE],[HD_HOST],[RESPONSE_CODE],[CLIENT_HARDWARE_INFO],[APPLICMEMO],[APPLICTYPE],[APPLICTIME],[STATUS],[APPROVETIME],[APPROVEUSER],[APPROVEMEMO],[FIRSTGETTIME],[LASTGETTIME],[FEEDBACKTIME],[FEEDBACKEMEMO],[CPH],[PC],[GGSXRQ],[UKEY],[VERSION],[ZZBH],[DYWYM],[PZXLH],[LSPZXLH],[IMPORTFLAG],[UPSEND_TAG],[HSJE],[TypeCode],[InvNo],[FPLX]) VALUES (
                            @H_ID
,@QYID_BJ
,@CLZTXX
,@ZCHGZBH
,@WZHGZBH
,@DPHGZBH
,@FZRQ
,@CLZZQYMC
,@QYID
,@CLLX
,@CLMC
,@CLPP
,@CLXH
,@CSYS
,@DPXH
,@DPID
,@CLSBDH
,@CJH
,@FDJH
,@FDJXH
,@RLZL
,@PFBZ
,@PL
,@GL
,@ZXXS
,@QLJ
,@HLJ
,@LTS
,@LTGG
,@GBTHPS
,@ZJ
,@ZH
,@ZS
,@WKC
,@WKK
,@WKG
,@HXNBC
,@HXNBK
,@HXNBG
,@ZZL
,@EDZZL
,@ZBZL
,@ZZLLYXS
,@ZQYZZL
,@EDZK
,@BGCAZZDYXZZL
,@JSSZCRS
,@QZDFS
,@HZDFS
,@QZDCZFS
,@HZDCZFS
,@ZGCS
,@CLZZRQ
,@BZ
,@QYBZ
,@CPSCDZ
,@QYQTXX
,@CZRQ
,@CREATETIME
,@UPDATETIME
,@HD_USER
,@CLSCDWMC
,@YH
,@ZXZS
,@CDDBJ
,@VERCODE
,@HD_HOST
,@RESPONSE_CODE
,@CLIENT_HARDWARE_INFO
,@APPLICMEMO
,@APPLICTYPE
,@APPLICTIME
,@STATUS
,@APPROVETIME
,@APPROVEUSER
,@APPROVEMEMO
,@FIRSTGETTIME
,@LASTGETTIME
,@FEEDBACKTIME
,@FEEDBACKEMEMO
,@CPH
,@PC
,@GGSXRQ
,@UKEY
,@VERSION
,@ZZBH
,@DYWYM
,@PZXLH
,@LSPZXLH
,@IMPORTFLAG
,@UPSEND_TAG
,@HSJE
,@TypeCode
,@InvNo
,@FPLX)";
                        #endregion
                        // '56600','1','1','1','1','1','2017/11/22','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','11','1','1','2017/11/22','1','1','1','1','2017/11/22','2017/11/22','2017/11/22','zcc','1','1','1','1','1','1','1','1','1','1','2017/11/22','1','2017/11/22','1','1','2017/11/22','2017/11/22','2017/11/22','1','1','1','2017/11/22','1','1','1','1','1','1','1','1',1,'1','1','1')

                        #region  param0
                        SqlParameter[] param0 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
//new SqlParameter("@H_ID","4943273806427_0000076381"),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","QX"),
new SqlParameter("@ZCHGZBH","CAA0ZJ40507060"),
new SqlParameter("@WZHGZBH","CAA02ZJ40507060"),
new SqlParameter("@DPHGZBH",DBNull.Value),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","钻机车"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5821TZJ30"),
new SqlParameter("@CSYS","红色"),
new SqlParameter("@DPXH",DBNull.Value),
new SqlParameter("@DPID",DBNull.Value),
new SqlParameter("@CLSBDH","LA9E5VCH750SJZ060"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH","CAT3406 6TB22827"),
new SqlParameter("@FDJXH","CAT3406"),
new SqlParameter("@RLZL","柴油/ /"),
new SqlParameter("@PFBZ","GB3847-1999,GB17691-2001 (第二阶段）"),
new SqlParameter("@PL","14600"),
new SqlParameter("@GL","343"),
new SqlParameter("@ZXXS","方向盘"),
new SqlParameter("@QLJ","2412"),
new SqlParameter("@HLJ","2325"),
new SqlParameter("@LTS","20"),
new SqlParameter("@LTGG","18.00-22.5/12.00-20"),
new SqlParameter("@GBTHPS","9+9+9/-+-+-"),
new SqlParameter("@ZJ","1300+1300+4790+1370+1370+1370"),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS","7"),
new SqlParameter("@WKC","22169"),
new SqlParameter("@WKK","3240"),
new SqlParameter("@WKG","4530"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","82125"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","82060"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","1"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","48"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ","该车为双发动机，其另一发动机号：CAT3406 6TB22947"),
new SqlParameter("@QYBZ","Q/JQ.J02.300-2000（2005）《SJX5821TZJ30型钻机车》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                   质检科长:                 签发单位:第四石油机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L17TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·10·科技峰会"),
new SqlParameter("@VERSION","10"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{10}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","119.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676796"),
new SqlParameter("@FPLX","专票")
#region
//new SqlParameter("@QYID_BJ","1"),
//new SqlParameter("@CLZTXX","╮╭╯╰╳"),
//new SqlParameter("@ZCHGZBH","1╮╭╯╰╳frewrew"),
//new SqlParameter("@WZHGZBH","、\r\n\t"),
//new SqlParameter("@DPHGZBH","、r\t\n/r/t/n"),
//new SqlParameter("@FZRQ","2017/11/22"),
//new SqlParameter("@CLZZQYMC","1"),
//new SqlParameter("@QYID","1$^&7jhsdja┐┑┒┓└┕"),
//new SqlParameter("@CLLX","1"),
//new SqlParameter("@CLMC","  khka   &(*&=-=我们┐┑┒┓└┕"),
//new SqlParameter("@CLPP","1"),
//new SqlParameter("@CLXH","……&……&……！（）*&*……&……%￥%￥#￥#@#@！@~"),
//new SqlParameter("@CSYS","1"),
//new SqlParameter("@DPXH",",\"\"sjahfdjahf/\"┐┑┒┓└┕"),
//new SqlParameter("@DPID","┐┑┒┓└┕"),
//new SqlParameter("@CLSBDH","╮╭╯╰╳"),
//new SqlParameter("@CJH","1"),
//new SqlParameter("@FDJH","1"),
//new SqlParameter("@FDJXH",",\"\"sjahfdjahf/\"┐┑┒┓└┕"),
//new SqlParameter("@RLZL","1"),
//new SqlParameter("@PFBZ","1"),
//new SqlParameter("@PL","1"),
//new SqlParameter("@GL","1"),
//new SqlParameter("@ZXXS","1"),
//new SqlParameter("@QLJ","1"),
//new SqlParameter("@HLJ","1"),
//new SqlParameter("@LTS","1"),
//new SqlParameter("@LTGG","1"),
//new SqlParameter("@GBTHPS","1"),
//new SqlParameter("@ZJ","1"),
//new SqlParameter("@ZH","1"),
//new SqlParameter("@ZS","1"),
//new SqlParameter("@WKC","1"),
//new SqlParameter("@WKK","1"),
//new SqlParameter("@WKG","1"),
//new SqlParameter("@HXNBC","1"),
//new SqlParameter("@HXNBK","1"),
//new SqlParameter("@HXNBG","1"),
//new SqlParameter("@ZZL","1"),
//new SqlParameter("@EDZZL","1"),
//new SqlParameter("@ZBZL","1"),
//new SqlParameter("@ZZLLYXS","1"),
//new SqlParameter("@ZQYZZL","1"),
//new SqlParameter("@EDZK","1"),
//new SqlParameter("@BGCAZZDYXZZL","1"),
//new SqlParameter("@JSSZCRS","1"),
//new SqlParameter("@QZDFS","1"),
//new SqlParameter("@HZDFS","1"),
//new SqlParameter("@QZDCZFS","11"),
//new SqlParameter("@HZDCZFS","1"),
//new SqlParameter("@ZGCS","1"),
//new SqlParameter("@CLZZRQ","2017/11/22"),
//new SqlParameter("@BZ","1"),
//new SqlParameter("@QYBZ","1"),
//new SqlParameter("@CPSCDZ","1"),
//new SqlParameter("@QYQTXX","1"),
//new SqlParameter("@CZRQ","2017/11/22"),
//new SqlParameter("@CREATETIME","2017/11/22"),
//new SqlParameter("@UPDATETIME","2017/11/22"),
//new SqlParameter("@HD_USER","zcc"),
//new SqlParameter("@CLSCDWMC","1"),
//new SqlParameter("@YH","1"),
//new SqlParameter("@ZXZS","1"),
//new SqlParameter("@CDDBJ","1"),
//new SqlParameter("@VERCODE","1"),
//new SqlParameter("@HD_HOST","1"),
//new SqlParameter("@RESPONSE_CODE","1"),
//new SqlParameter("@CLIENT_HARDWARE_INFO","1"),
//new SqlParameter("@APPLICMEMO","skjfdakjsf  jahfahsfa;'l';lmlkfskf我们是好孩子┐┑┒┓└┕"),
//new SqlParameter("@APPLICTYPE","1"),
//new SqlParameter("@APPLICTIME","2017/11/22"),
//new SqlParameter("@STATUS","1"),
//new SqlParameter("@APPROVETIME","2017/11/22"),
//new SqlParameter("@APPROVEUSER","1"),
//new SqlParameter("@APPROVEMEMO","1"),
//new SqlParameter("@FIRSTGETTIME","2017/11/22"),
//new SqlParameter("@LASTGETTIME","2017/11/22"),
//new SqlParameter("@FEEDBACKTIME","2017/11/22"),
//new SqlParameter("@FEEDBACKEMEMO","1"),
//new SqlParameter("@CPH","1"),
//new SqlParameter("@PC","1"),
//new SqlParameter("@GGSXRQ","2017/11/22"),
//new SqlParameter("@UKEY","1"),
//new SqlParameter("@VERSION","1"),
//new SqlParameter("@ZZBH","1"),
//new SqlParameter("@DYWYM","1"),
//new SqlParameter("@PZXLH","1"),
//new SqlParameter("@LSPZXLH","1"),
//new SqlParameter("@IMPORTFLAG","1"),
//new SqlParameter("@UPSEND_TAG","1"),
//new SqlParameter("@HSJE","1"),
//new SqlParameter("@TypeCode","1"),
//new SqlParameter("@InvNo","1"),
//new SqlParameter("@FPLX","1")
#endregion

};


                        #endregion param

                        SqlHelper sqlHelper = new SqlHelper();
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param0);
                        id++;
                        #region  param1
                        SqlParameter[] param1 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","QX"),
new SqlParameter("@ZCHGZBH","CAA0XJ40510105"),
new SqlParameter("@WZHGZBH","CAA00XJ40510105"),
new SqlParameter("@DPHGZBH",DBNull.Value),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","修井机"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5430TXJ450"),
new SqlParameter("@CSYS","红色"),
new SqlParameter("@DPXH",DBNull.Value),
new SqlParameter("@DPID",DBNull.Value),
new SqlParameter("@CLSBDH","LA9E5VGG250SJX105"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH","CAT3408 67U21361"),
new SqlParameter("@FDJXH","CAT3408"),
new SqlParameter("@RLZL","柴油/ /"),
new SqlParameter("@PFBZ","GB3847-1999,GB17691-2001 (第二阶段）"),
new SqlParameter("@PL","18000"),
new SqlParameter("@GL","354"),
new SqlParameter("@ZXXS","方向盘"),
new SqlParameter("@QLJ","2180"),
new SqlParameter("@HLJ","2180"),
new SqlParameter("@LTS","10"),
new SqlParameter("@LTGG","18.00-22.5"),
new SqlParameter("@GBTHPS","8+8/-"),
new SqlParameter("@ZJ","1300+5500+1370+1370"),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS","5"),
new SqlParameter("@WKC","17950"),
new SqlParameter("@WKK","2800"),
new SqlParameter("@WKG","4250"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","43465"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","43400"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","1"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","48"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ",DBNull.Value),
new SqlParameter("@QYBZ","Q/JQ.J02.286-2003 (2005)《SJX5430TXJ450型修井机》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                   质检科长:                 签发单位:第四石油机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L9TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·2·科技峰会"),
new SqlParameter("@VERSION","2"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{2}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","111.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676788"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param1);
                        id++;
                        #region  param2
                        SqlParameter[] param2 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","QX"),
new SqlParameter("@ZCHGZBH","CAA0XJ60508108"),
new SqlParameter("@WZHGZBH","CAA00XJ60508108"),
new SqlParameter("@DPHGZBH",DBNull.Value),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","修井机"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5540TXJ650"),
new SqlParameter("@CSYS","红色"),
new SqlParameter("@DPXH",DBNull.Value),
new SqlParameter("@DPID",DBNull.Value),
new SqlParameter("@CLSBDH","LA9E5VGF950SJX108"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH","CAT3412 38S23687"),
new SqlParameter("@FDJXH","CAT3412"),
new SqlParameter("@RLZL","柴油/ /"),
new SqlParameter("@PFBZ","GB3847-1999,GB17691-2001 (第二阶段）"),
new SqlParameter("@PL","27000"),
new SqlParameter("@GL","485"),
new SqlParameter("@ZXXS","方向盘"),
new SqlParameter("@QLJ","2180"),
new SqlParameter("@HLJ","1962"),
new SqlParameter("@LTS","12"),
new SqlParameter("@LTGG","18.00-22.5"),
new SqlParameter("@GBTHPS","8+8//8-"),
new SqlParameter("@ZJ","1300+1300+6015+1370+1370"),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS","6"),
new SqlParameter("@WKC","18620"),
new SqlParameter("@WKK","3132"),
new SqlParameter("@WKG","4470"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","54150"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","54080"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","1"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","48"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ",DBNull.Value),
new SqlParameter("@QYBZ","Q/JQ.J02.196-2003 (2005)《SJX5540TXJ650型修井机》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                   质检科长:                 签发单位:第四石油机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L10TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·3·科技峰会"),
new SqlParameter("@VERSION","3"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{3}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","112.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676789"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param2);
                        id++;
                        #region  param3
                        SqlParameter[] param3 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","JH"),
new SqlParameter("@ZCHGZBH","CAA0JC20507086"),
new SqlParameter("@WZHGZBH","CAA01JC20507086"),
new SqlParameter("@DPHGZBH","WDT03005D006334"),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","洗井车"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5160TJC12"),
new SqlParameter("@CSYS","金属绿色"),
new SqlParameter("@DPXH","CQ1253BM434"),
new SqlParameter("@DPID","1214764"),
new SqlParameter("@CLSBDH","LZFC25M445D006334"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH",DBNull.Value),
new SqlParameter("@FDJXH",DBNull.Value),
new SqlParameter("@RLZL","/ /"),
new SqlParameter("@PFBZ",DBNull.Value),
new SqlParameter("@PL",DBNull.Value),
new SqlParameter("@GL",DBNull.Value),
new SqlParameter("@ZXXS",DBNull.Value),
new SqlParameter("@QLJ",DBNull.Value),
new SqlParameter("@HLJ",DBNull.Value),
new SqlParameter("@LTS",DBNull.Value),
new SqlParameter("@LTGG",DBNull.Value),
new SqlParameter("@GBTHPS",DBNull.Value),
new SqlParameter("@ZJ",DBNull.Value),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS",DBNull.Value),
new SqlParameter("@WKC","8755"),
new SqlParameter("@WKK","2500"),
new SqlParameter("@WKG","3020"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","16000"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","15870"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","2"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","90"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ",DBNull.Value),
new SqlParameter("@QYBZ","Q/JQ.J02.296-2005《SJX5160TJC12型洗井车》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                        质检科长:                     签发单位:  第四机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L11TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·4·科技峰会"),
new SqlParameter("@VERSION","4"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{4}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","113.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676790"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param3);
                        id++;
                        #region  param4
                        SqlParameter[] param4 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","QX"),
new SqlParameter("@ZCHGZBH","CAA0SN60511021"),
new SqlParameter("@WZHGZBH","CAA01SN60511021"),
new SqlParameter("@DPHGZBH",DBNull.Value),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","固井水泥车"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5310TSN30"),
new SqlParameter("@CSYS","红色"),
new SqlParameter("@DPXH",DBNull.Value),
new SqlParameter("@DPID",DBNull.Value),
new SqlParameter("@CLSBDH",DBNull.Value),
new SqlParameter("@CJH","WDB9323251L049108"),
new SqlParameter("@FDJH","OM501LA 54192100405721"),
new SqlParameter("@FDJXH","OM501LA"),
new SqlParameter("@RLZL","柴油/ /"),
new SqlParameter("@PFBZ","GB3847-1999，GB17691-2001 (第二阶段）"),
new SqlParameter("@PL","11946"),
new SqlParameter("@GL","315"),
new SqlParameter("@ZXXS","方向盘"),
new SqlParameter("@QLJ","2009"),
new SqlParameter("@HLJ","1832"),
new SqlParameter("@LTS","12"),
new SqlParameter("@LTGG","12.00R20"),
new SqlParameter("@GBTHPS","42799"),
new SqlParameter("@ZJ","1700+4800+1350"),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS","4"),
new SqlParameter("@WKC","11860"),
new SqlParameter("@WKK","2500"),
new SqlParameter("@WKG","3903"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","30980"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","30850"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","2"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","85"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ",DBNull.Value),
new SqlParameter("@QYBZ","Q/JQ.J02.347-2004《SJX5310TSN30型固井水泥车》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                   质检科长:                 签发单位:第四石油机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L12TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·5·科技峰会"),
new SqlParameter("@VERSION","5"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{5}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","114.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676791"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param4);
                        id++;
                        #region  param5
                        SqlParameter[] param5 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","JH"),
new SqlParameter("@ZCHGZBH","CAA0SN30512043"),
new SqlParameter("@WZHGZBH","CAA02SN30512043"),
new SqlParameter("@DPHGZBH","WEK035G00010415"),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","固井水泥车"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5191TSN12"),
new SqlParameter("@CSYS","樱桃红"),
new SqlParameter("@DPXH","SX1254BM434"),
new SqlParameter("@DPID","1215828"),
new SqlParameter("@CLSBDH","LZGFL2M415G010392"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH",DBNull.Value),
new SqlParameter("@FDJXH",DBNull.Value),
new SqlParameter("@RLZL","/ /"),
new SqlParameter("@PFBZ",DBNull.Value),
new SqlParameter("@PL",DBNull.Value),
new SqlParameter("@GL",DBNull.Value),
new SqlParameter("@ZXXS",DBNull.Value),
new SqlParameter("@QLJ",DBNull.Value),
new SqlParameter("@HLJ",DBNull.Value),
new SqlParameter("@LTS",DBNull.Value),
new SqlParameter("@LTGG",DBNull.Value),
new SqlParameter("@GBTHPS",DBNull.Value),
new SqlParameter("@ZJ",DBNull.Value),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS",DBNull.Value),
new SqlParameter("@WKC","9550"),
new SqlParameter("@WKK","2500"),
new SqlParameter("@WKG","3230"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","19200"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","19070"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","2"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","75"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ",DBNull.Value),
new SqlParameter("@QYBZ","Q/JQ.J02.295-2003（2005）《SJX5191TSN12型固井水泥车》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                        质检科长:                     签发单位:  第四机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L13TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·6·科技峰会"),
new SqlParameter("@VERSION","6"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{6}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","115.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676792"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param5
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param5);
                        id++;
                        #region  param6
                        SqlParameter[] param6 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","QX"),
new SqlParameter("@ZCHGZBH","CAA0ZJ40507060"),
new SqlParameter("@WZHGZBH","CAA02ZJ40507060"),
new SqlParameter("@DPHGZBH",DBNull.Value),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","钻机车"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5821TZJ30"),
new SqlParameter("@CSYS","红色"),
new SqlParameter("@DPXH",DBNull.Value),
new SqlParameter("@DPID",DBNull.Value),
new SqlParameter("@CLSBDH","LA9E5VCH750SJZ060"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH","CAT3406 6TB22827"),
new SqlParameter("@FDJXH","CAT3406"),
new SqlParameter("@RLZL","柴油/ /"),
new SqlParameter("@PFBZ","GB3847-1999,GB17691-2001 (第二阶段）"),
new SqlParameter("@PL","14600"),
new SqlParameter("@GL","343"),
new SqlParameter("@ZXXS","方向盘"),
new SqlParameter("@QLJ","2412"),
new SqlParameter("@HLJ","2325"),
new SqlParameter("@LTS","20"),
new SqlParameter("@LTGG","18.00-22.5/12.00-20"),
new SqlParameter("@GBTHPS","9+9+9/-+-+-"),
new SqlParameter("@ZJ","1300+1300+4790+1370+1370+1370"),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS","7"),
new SqlParameter("@WKC","22169"),
new SqlParameter("@WKK","3240"),
new SqlParameter("@WKG","4530"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","82125"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","82060"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","1"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","48"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ","该车为双发动机，其另一发动机号：CAT3406 6TB22947"),
new SqlParameter("@QYBZ","Q/JQ.J02.300-2000（2005）《SJX5821TZJ30型钻机车》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                   质检科长:                 签发单位:第四石油机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now.AddHours(-1)),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L17TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·10·科技峰会"),
new SqlParameter("@VERSION","10"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{10}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","119.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676796"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param6
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param6);
                        id++;
                        #region  param7
                        SqlParameter[] param7 = {

new SqlParameter("@H_ID","4943273806427_0000076381"+id.ToString()),
new SqlParameter("@QYID_BJ","HX175002"),
new SqlParameter("@CLZTXX","QX"),
new SqlParameter("@ZCHGZBH","CAA0ZJ30510111"),
new SqlParameter("@WZHGZBH","CAA02ZJ30510111"),
new SqlParameter("@DPHGZBH",DBNull.Value),
new SqlParameter("@FZRQ","2017/11/21 00:21:21"),
new SqlParameter("@CLZZQYMC","江汉石油管理局第四机械厂"),
new SqlParameter("@QYID","CAA0"),
new SqlParameter("@CLLX","专用汽车"),
new SqlParameter("@CLMC","钻机车"),
new SqlParameter("@CLPP","四机牌"),
new SqlParameter("@CLXH","SJX5560TZJ20"),
new SqlParameter("@CSYS","红色"),
new SqlParameter("@DPXH",DBNull.Value),
new SqlParameter("@DPID",DBNull.Value),
new SqlParameter("@CLSBDH","LA9E5VGF950SJX111"),
new SqlParameter("@CJH",DBNull.Value),
new SqlParameter("@FDJH","CAT3412 38S23685"),
new SqlParameter("@FDJXH","CAT3412"),
new SqlParameter("@RLZL","柴油/ /"),
new SqlParameter("@PFBZ","GB3847-1999,GB17691-2001 (第二阶段）"),
new SqlParameter("@PL","27000"),
new SqlParameter("@GL","485"),
new SqlParameter("@ZXXS","方向盘"),
new SqlParameter("@QLJ","2180"),
new SqlParameter("@HLJ","2180"),
new SqlParameter("@LTS","12"),
new SqlParameter("@LTGG","18.00-22.5"),
new SqlParameter("@GBTHPS","8+8//8-"),
new SqlParameter("@ZJ","1300+1300+4655+1370+1370"),
new SqlParameter("@ZH",DBNull.Value),
new SqlParameter("@ZS","6"),
new SqlParameter("@WKC","19800"),
new SqlParameter("@WKK","3100"),
new SqlParameter("@WKG","4470"),
new SqlParameter("@HXNBC",DBNull.Value),
new SqlParameter("@HXNBK",DBNull.Value),
new SqlParameter("@HXNBG",DBNull.Value),
new SqlParameter("@ZZL","56065"),
new SqlParameter("@EDZZL",DBNull.Value),
new SqlParameter("@ZBZL","5600"),
new SqlParameter("@ZZLLYXS",DBNull.Value),
new SqlParameter("@ZQYZZL",DBNull.Value),
new SqlParameter("@EDZK",DBNull.Value),
new SqlParameter("@BGCAZZDYXZZL",DBNull.Value),
new SqlParameter("@JSSZCRS","1"),
new SqlParameter("@QZDFS",DBNull.Value),
new SqlParameter("@HZDFS",DBNull.Value),
new SqlParameter("@QZDCZFS",DBNull.Value),
new SqlParameter("@HZDCZFS",DBNull.Value),
new SqlParameter("@ZGCS","48"),
new SqlParameter("@CLZZRQ",DateTime.Now.AddDays(-10)),
new SqlParameter("@BZ",DBNull.Value),
new SqlParameter("@QYBZ","Q/JQ.J02.234-2000 (2005)《SJX5560TZJ20型钻机车》"),
new SqlParameter("@CPSCDZ","湖北省荆州市荆州区西门龙山寺"),
new SqlParameter("@QYQTXX","检验员:                   质检科长:                 签发单位:第四石油机械厂质检科"),
new SqlParameter("@CZRQ",DateTime.Now.AddMonths(-1)),
new SqlParameter("@CREATETIME",DateTime.Now),
new SqlParameter("@UPDATETIME",DateTime.Now.AddHours(1)),
new SqlParameter("@HD_USER","HX175002U001"),
new SqlParameter("@CLSCDWMC",DBNull.Value),
new SqlParameter("@YH",DBNull.Value),
new SqlParameter("@ZXZS",DBNull.Value),
new SqlParameter("@CDDBJ",DBNull.Value),
new SqlParameter("@VERCODE",DBNull.Value),
new SqlParameter("@HD_HOST",DBNull.Value),
new SqlParameter("@RESPONSE_CODE",DBNull.Value),
new SqlParameter("@CLIENT_HARDWARE_INFO",DBNull.Value),
new SqlParameter("@APPLICMEMO"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTYPE"," ‘汽车产业需求’需修改；"),
new SqlParameter("@APPLICTIME","2017/11/21 00:21:22"),
new SqlParameter("@STATUS","43242"),
new SqlParameter("@APPROVETIME",DBNull.Value),
new SqlParameter("@APPROVEUSER",DBNull.Value),
new SqlParameter("@APPROVEMEMO",DBNull.Value),
new SqlParameter("@FIRSTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@LASTGETTIME","2017/11/21 00:21:21"),
new SqlParameter("@FEEDBACKTIME","2017/11/21 00:21:22"),
new SqlParameter("@FEEDBACKEMEMO","【测试数据】"),
new SqlParameter("@CPH",""),
new SqlParameter("@PC","LAPTOP-G28L16TBO"),
new SqlParameter("@GGSXRQ",""),
new SqlParameter("@UKEY","188-53DB-4D95-B7E6-F95&……%&*（）*（·9·科技峰会"),
new SqlParameter("@VERSION","9"),
new SqlParameter("@ZZBH","ZZBHi"),
new SqlParameter("@DYWYM","调用汽车网页码"),
new SqlParameter("@PZXLH","S001"),
new SqlParameter("@LSPZXLH","临时牌照序列号"),
new SqlParameter("@IMPORTFLAG","{9}"),
new SqlParameter("@UPSEND_TAG","S001"),
new SqlParameter("@HSJE","118.1234"),
new SqlParameter("@TypeCode","/\\\"\""),
new SqlParameter("@InvNo","#$#6767-=21387||euri\"\"676795"),
new SqlParameter("@FPLX","专票")
                                                };


                        #endregion param7
                        sqlHelper.ExecuteNonQuery(connectionString, sql, param7);
                        id++;

                    }
                    catch(Exception ex)
                    {
                        IsStopped = false;
                        throw ex;
                    }
                }

            }
                           
        }

        //停止
        public void Stop()
        {
            IsStopped = false;
        }
    
    }
}
