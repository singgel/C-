using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using Catarc.Adc.NewEnergyApproveSys.LogUtils;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Oracle.ManagedDataAccess.Client;

namespace Catarc.Adc.NewEnergyApproveSys.DataUtils
{
    public class DataCompareHelper
    {
        /// <summary>
        /// 比对公告的参数
        /// </summary>
        /// <param name="queryParam">查询条件</param>
        /// <returns>比对信息</returns>
        public string CompareDataTableThread(string CLSCQY)
        {
            string msg = string.Empty;
            try
            {
                using (var con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    OracleHelper.ExecuteNonQuery(con, string.Format("update DB_INFOMATION set BDJG=-1 where CLSCQY='{0}' and BDJG=0", CLSCQY));
                    using (var tra = con.BeginTransaction())
                    {
                        //STEP5:推荐目录金额
                        int num5 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BTBZ=(select max(BTBZ) from DB_SUBSIDY where trim(to_single_byte(DB_INFOMATION.CLXH))=trim(to_single_byte(DB_SUBSIDY.CLXH))) where CLSCQY='{0}' and BDJG=-1", CLSCQY));
                        msg += String.Format("推荐目录金额的数据有条{0}{1}", num5, Environment.NewLine);
                        //STEP211:VIN重复审批已通过
                        int num211 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG=21 where CLSCQY='{0}' and BDJG=-1 and exists(select VIN from APPLY_IMP_DETAIL_201601 where APPROVALFLAG=40 AND APPLY_IMP_DETAIL_201601.VIN=DB_INFOMATION.VIN)", CLSCQY));
                        msg += String.Format("201601VIN重复审批已通过的数据有条{0}{1}", num211, Environment.NewLine);
                        //STEP212:VIN重复审批已通过
                        int num212 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG=21 where CLSCQY='{0}' and BDJG=-1 and exists(select VIN from APPLY_IMP_DETAIL_201602 where APPROVALFLAG=40 AND APPLY_IMP_DETAIL_201602.VIN=DB_INFOMATION.VIN)", CLSCQY));
                        msg += String.Format("201602VIN重复审批已通过的数据有条{0}{1}", num212, Environment.NewLine);
                        //STEP22:VIN重复审批已驳回
                        int num221 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG=22 where CLSCQY='{0}' and BDJG=-1 and exists(select VIN from APPLY_IMP_DETAIL_201601 where (APPROVALFLAG=5 OR APPROVALFLAG=15 OR APPROVALFLAG=25) AND APPLY_IMP_DETAIL_201601.VIN=DB_INFOMATION.VIN)", CLSCQY));
                        msg += String.Format("201601VIN重复审批已驳回的数据有条{0}{1}", num221, Environment.NewLine);
                        //STEP22:VIN重复审批已驳回
                        int num222 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG=22 where CLSCQY='{0}' and BDJG=-1 and exists(select VIN from APPLY_IMP_DETAIL_201602 where (APPROVALFLAG=5 OR APPROVALFLAG=15 OR APPROVALFLAG=25) AND APPLY_IMP_DETAIL_201602.VIN=DB_INFOMATION.VIN)", CLSCQY));
                        msg += String.Format("201602VIN重复审批已驳回的数据有条{0}{1}", num222, Environment.NewLine);
                        //STEP20:VIN重复未审批
                        int num20 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG=20 where CLSCQY='{0}' and BDJG=-1 and VIN in(select VIN from DB_INFOMATION i group by VIN having count(vin) > 1)", CLSCQY));
                        msg += String.Format("VIN重复未审批的数据有条{0}{1}", num20, Environment.NewLine);
                        //STEP3:没有实际意义
                        int num3 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG=3 where CLSCQY='{0}' and BDJG=-1", CLSCQY));
                        msg += String.Format("没有实际意义的数据有条{0}{1}", num3, Environment.NewLine);

                        OracleHelper.ExecuteNonQuery(con, string.Format("update DB_INFOMATION set BDJG_GG=-1 where CLSCQY='{0}' and BDJG_GG=0", CLSCQY));
                        //STEP9:删除上次公告目录不匹配结果
                        int num9 = OracleHelper.ExecuteNonQuery(tra, string.Format("delete DB_DIFFERENT where exists(select VIN from DB_INFOMATION where BDJG_GG=-1 and DB_DIFFERENT.VIN=DB_INFOMATION.VIN)", CLSCQY));
                        msg += String.Format("删除上次公告目录不匹配结果的数据有条{0}{1}", num9, Environment.NewLine);
                        //STEP10:插入这次公告目录不匹配参数
                        int num10 = OracleHelper.ExecuteNonQuery(tra, string.Format("insert into DB_DIFFERENT select * from VIEW_COMPARE_DIFF where exists(select VIN from DB_INFOMATION where BDJG_GG=-1 and DB_INFOMATION.VIN=VIEW_COMPARE_DIFF.VIN)", CLSCQY));
                        msg += String.Format("插入这次公告目录不匹配参数的数据有条{0}{1}", num10, Environment.NewLine);
                        //STEP6:公告目录未找到
                        int num6 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION d set BDJG_GG=1 where exists(select VIN from DB_INFOMATION b where CLSCQY='{0}' and BDJG_GG=-1 and not exists(select trim(to_single_byte(DB_NOTICEPARAM.CLXH)) from DB_NOTICEPARAM where trim(to_single_byte(b.CLXH))=trim(to_single_byte(DB_NOTICEPARAM.CLXH))) and d.VIN=b.VIN)", CLSCQY));
                        msg += String.Format("公告目录未找到的数据有条{0}{1}", num6, Environment.NewLine);
                        //STEP7:公告目录找到匹配
                        int num7 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG_GG=2 where exists(select VIN from VIEW_COMPARE_SAME where DB_INFOMATION.CLSCQY='{0}' and DB_INFOMATION.BDJG_GG=-1 and DB_INFOMATION.VIN=VIEW_COMPARE_SAME.VIN)", CLSCQY));
                        msg += String.Format("公告目录找到匹配的数据有条{0}{1}", num7, Environment.NewLine);
                        //STEP8:公告目录不匹配结果
                        int num8 = OracleHelper.ExecuteNonQuery(tra, string.Format("update DB_INFOMATION set BDJG_GG=3 where exists(select VIN from VIEW_COMPARE_DIFF where DB_INFOMATION.CLSCQY='{0}' and DB_INFOMATION.BDJG_GG=-1 and DB_INFOMATION.VIN=VIEW_COMPARE_DIFF.VIN)", CLSCQY));
                        msg += String.Format("公告目录不匹配结果的数据有条{0}{1}", num8, Environment.NewLine);
                        tra.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + Environment.NewLine;
            }
            return msg;
        }

        /// <summary>
        /// 比对公告的参数
        /// </summary>
        /// <param name="queryParam">查询条件</param>
        /// <returns>比对信息</returns>
        public string CompareDataTable(string queryParam)
        {
            string msg = string.Empty;
            try
            {
                using (var con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    OracleHelper.ExecuteNonQuery(con, string.Format("update DB_INFOMATION set BDJG=-2 where 1=1 {0}", queryParam));
                    using (var tra = con.BeginTransaction())
                    {
                        //STEP5:推荐目录金额
                        int num5 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BTBZ=(select max(BTBZ) from DB_SUBSIDY where trim(to_single_byte(DB_INFOMATION.CLXH))=trim(to_single_byte(DB_SUBSIDY.CLXH))) where BDJG=-2");
                        msg += String.Format("推荐目录金额的数据有条{0}{1}", num5, Environment.NewLine);
                        //STEP211:VIN重复审批已通过
                        int num211 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG=21 where BDJG=-2 and exists(select VIN from APPLY_IMP_DETAIL_201601 where APPROVALFLAG=40 AND APPLY_IMP_DETAIL_201601.VIN=DB_INFOMATION.VIN)");
                        msg += String.Format("201601VIN重复审批已通过的数据有条{0}{1}", num211, Environment.NewLine);
                        //STEP212:VIN重复审批已通过
                        int num212 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG=21 where BDJG=-2 and exists(select VIN from APPLY_IMP_DETAIL_201602 where APPROVALFLAG=40 AND APPLY_IMP_DETAIL_201602.VIN=DB_INFOMATION.VIN)");
                        msg += String.Format("201602VIN重复审批已通过的数据有条{0}{1}", num212, Environment.NewLine);
                        //STEP22:VIN重复审批已驳回
                        int num221 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG=22 where BDJG=-2 and exists(select VIN from APPLY_IMP_DETAIL_201601 where (APPROVALFLAG=5 OR APPROVALFLAG=15 OR APPROVALFLAG=25) AND APPLY_IMP_DETAIL_201601.VIN=DB_INFOMATION.VIN)");
                        msg += String.Format("201601VIN重复审批已驳回的数据有条{0}{1}", num221, Environment.NewLine);
                        //STEP22:VIN重复审批已驳回
                        int num222 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG=22 where BDJG=-2 and exists(select VIN from APPLY_IMP_DETAIL_201602 where (APPROVALFLAG=5 OR APPROVALFLAG=15 OR APPROVALFLAG=25) AND APPLY_IMP_DETAIL_201602.VIN=DB_INFOMATION.VIN)");
                        msg += String.Format("201602VIN重复审批已驳回的数据有条{0}{1}", num222, Environment.NewLine);
                        //STEP3:推荐目录找到
                        int num3 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG=3 where BDJG=-2");
                        //msg += String.Format("推荐目录找到的数据有条{0}{1}", num3, Environment.NewLine);

                        OracleHelper.ExecuteNonQuery(con, string.Format("update DB_INFOMATION set BDJG_GG=-2 where 1=1 {0}", queryParam));
                        //STEP9:删除上次公告目录不匹配结果
                        int num9 = OracleHelper.ExecuteNonQuery(tra, "delete DB_DIFFERENT where exists(select VIN from DB_INFOMATION where DB_INFOMATION.BDJG_GG=-2 and DB_DIFFERENT.VIN=DB_INFOMATION.VIN)");
                        //msg += String.Format("删除上次公告目录不匹配结果的数据有条{0}{1}", num9, Environment.NewLine);
                        //STEP10:插入这次公告目录不匹配参数
                        int num10 = OracleHelper.ExecuteNonQuery(tra, "insert into DB_DIFFERENT select * from VIEW_COMPARE_DIFF where exists(select VIN from DB_INFOMATION where DB_INFOMATION.BDJG_GG=-2 and DB_INFOMATION.VIN=VIEW_COMPARE_DIFF.VIN)");
                        //msg += String.Format("插入这次公告目录不匹配参数的数据有条{0}{1}", num10, Environment.NewLine);
                        //STEP6:公告目录未找到
                        int num6 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION d set BDJG_GG=1 where exists(select VIN from DB_INFOMATION b where BDJG_GG=-2 and not exists(select trim(to_single_byte(DB_NOTICEPARAM.CLXH)) from DB_NOTICEPARAM where trim(to_single_byte(b.CLXH))=trim(to_single_byte(DB_NOTICEPARAM.CLXH))) and d.VIN=b.VIN)");
                        msg += String.Format("公告目录未找到的数据有条{0}{1}", num6, Environment.NewLine);
                        //STEP7:公告目录找到匹配
                        int num7 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG_GG=2 where exists(select VIN from VIEW_COMPARE_SAME where DB_INFOMATION.BDJG_GG=-2 and DB_INFOMATION.VIN=VIEW_COMPARE_SAME.VIN)");
                        msg += String.Format("公告目录找到匹配的数据有条{0}{1}", num7, Environment.NewLine);
                        //STEP8:公告目录不匹配结果
                        int num8 = OracleHelper.ExecuteNonQuery(tra, "update DB_INFOMATION set BDJG_GG=3 where exists(select VIN from VIEW_COMPARE_DIFF where DB_INFOMATION.BDJG_GG=-2 and DB_INFOMATION.VIN=VIEW_COMPARE_DIFF.VIN)");
                        msg += String.Format("公告目录不匹配结果的数据有条{0}{1}", num8, Environment.NewLine);
                        tra.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + Environment.NewLine;
            }
            return msg;
        }
    }
}
