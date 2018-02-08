using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Catarc.Adc.NewEnergyApproveSys.OfficeHelper;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils
{
    public class ApproveThirdUtils
    {
        /// <summary>
        /// 恢复选中数据
        /// </summary>
        /// <param name="obj">要恢复的控件信息</param>
        /// <returns></returns>
        public string recoverDataInfo(DataTable selectedDT)
        {
            string msg = string.Empty;
            using (var con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    var guidList = selectedDT.AsEnumerable().Select(d => d.Field<string>("GUID")).ToList();
                    while (guidList.Count > 0)
                    {
                        var guidArrSkip = guidList.Take(1000);
                        stringBuilder.AppendFormat(" or GUID in('{0}')", string.Join("','", guidArrSkip));
                        if (guidList.Count > 999)
                        {
                            guidList.RemoveRange(0, 999);
                        }
                        else
                        {
                            guidList.RemoveRange(0, guidList.Count);
                        }
                    }
                    var guidStr = string.Format("and ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    try
                    {
                        int num1 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_3=null,APP_TIME_3=null,APP_RESULT_3=null,APP_STATUS=21,APP_MONEY=APP_RESULT_1_A where APP_RESULT_2 is null {0}", guidStr));
                        int num2 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_3=null,APP_TIME_3=null,APP_RESULT_3=null,APP_STATUS=21,APP_MONEY=APP_RESULT_2 where APP_RESULT_2 is not null {0}", guidStr));
                        if ((num1 + num2) == selectedDT.Rows.Count)
                        {
                            msg += String.Format("{0}条数据成功恢复到三审待审批状态，操作成功！{1}", num1 + num2, Environment.NewLine);
                        }
                        else
                        {
                            msg += String.Format("{0}条数据未能恢复到三审待审批状态，操作失败！{1}", selectedDT.Rows.Count, Environment.NewLine);
                        }
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        msg += String.Format("数据恢复的异常信息，操作异常！{0}", ex.Message);
                    }
                }
            }
            return msg;
        }


        /// <summary>
        /// 批量VIN查询
        /// </summary>
        /// <returns></returns>
        public DataTable VINSelect(string FileName, string SelectedTabPage)
        {
            //导入excel中的VIN
            ImportExcelNPOI ieNOPI = new ImportExcelNPOI();
            DataTable dtVIN = ieNOPI.ExcelToDataTable(FileName, "Sheet1", true);
            if (dtVIN == null || dtVIN.Rows.Count < 1)
            {
                return null;
            }
            //查询数据
            DataSet dsDB = new DataSet();
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                try
                {
                    StringBuilder sqlStr = new StringBuilder("select * from DB_INFOMATION  where ");
                    if (SelectedTabPage.Equals("待审批"))
                    {
                        sqlStr.Append(" APP_STATUS=21 ");
                    }
                    else if (SelectedTabPage.Equals("驳回"))
                    {
                        sqlStr.Append(" APP_STATUS=30 ");
                    }
                    else
                    {
                        sqlStr.Append(" APP_STATUS=31 ");
                    }
                    string linqStr = string.Format("select i.*,n.GGPC_GG,n.DCDTXX_XH_GG,n.DCDTXX_SCQY_GG,n.DCZXX_XH_GG,n.DCZXX_ZRL_GG,n.DCZXX_SCQY_GG,n.QDDJXX_XH_1_GG,n.QDDJXX_EDGL_1_GG,n.QDDJXX_SCQY_1_GG,n.RLDCXX_XH_GG,n.RLDCXX_EDGL_GG,n.RLDCXX_SCQY_GG from ({0}) i left outer join DB_DIFFERENT n on i.VIN=n.VIN ", sqlStr);
                    dsDB = OracleHelper.ExecuteDataSet(OracleHelper.conn, linqStr, null);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (dsDB == null || dsDB.Tables.Count < 1 || dsDB.Tables[0].Rows.Count < 1)
            {
                return null;
            }
            //linq
            var dtVINArr = dtVIN.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
            var dt2 = from p in dsDB.Tables[0].AsEnumerable()
                       where dtVINArr.Contains(p.Field<string>("VIN"))
                       select p;
            if (dt2 == null || dt2.Count() == 0)
            {
                return null;
            }
            DataTable dtNew = dt2.CopyToDataTable();
            return dtNew;
        }
    }
}
