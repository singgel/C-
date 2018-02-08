using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils
{
    public class ApproveSecondUtils
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
                        int num2 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_2=null,APP_TIME_2=null,APP_RESULT_2=null,APP_MONEY=null,APP_STATUS=11 where APP_RESULT_2 is not null {0}", guidStr));
                        if (num2 != 0)
                        {
                            msg += String.Format("{0}条数据成功恢复到二审待审批状态，操作成功！{1}", num2, Environment.NewLine);
                        }
                        else
                        {
                            msg += String.Format("{0}条数据未能恢复到二审待审批状态，操作失败！{1}", selectedDT.Rows.Count, Environment.NewLine);
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

    }
}
