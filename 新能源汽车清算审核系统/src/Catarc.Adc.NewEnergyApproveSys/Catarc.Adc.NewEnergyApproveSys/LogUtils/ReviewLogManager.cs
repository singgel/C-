using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;

namespace Catarc.Adc.NewEnergyApproveSys.LogUtils
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class ReviewLogManager
    {
        public static void ReviewLog(string username,string operationName)
        {
            using(var con = new OracleConnection(OracleHelper.conn))
            {
                try
                {
                   string id = "SEQ_DB_REVIEWLOG.nextval";

                    string sql = string.Format("insert into DB_REVIEWLOG(ID,USERNAME,OPERATION,WORKTIME) VALUES({0},'{1}','{2}',sysdate) ",id,username, operationName);

                   OracleHelper.ExecuteNonQuery(OracleHelper.conn,sql,null);
                    
                }
                catch (Exception ex)
                {
                    LogManager.Log("Error", "DB_REVIEWLOG", "操作日志保存失败，失败原因：" + ex.Message);
                }
            }
        }


    }
}
