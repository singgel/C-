using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace WebBrowserUtils.HtmlUtils.History
{
    public static class HistoryHelper
    {
        private static System.Data.ConnectionPool Local;

        private class OleDbConnectionProvider : System.Data.DbConnectionFactory
        {
            internal static OleDbConnectionProvider Provider;

            public override System.Data.Common.DbConnection CreateConnection()
            {
                return new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=history.mdb;Persist Security Info=False;Jet OLEDB:Database Password=nicai");
            }

            static OleDbConnectionProvider()
            {
                Provider = new OleDbConnectionProvider();
            }
        }

        static HistoryHelper()
        {
            Local = new System.Data.ConnectionPool(OleDbConnectionProvider.Provider);
        }

        public static void InsertList(IList<FillRecord> list, string type, string username)
        {
            Local.ExecuteCommand((proxy) =>
            {
                int successCount = 0, failCount = 0;
                proxy.BeginTransaction();
                string command = "insert into FlEX_DECLARATION_INFO (filldate, username, filltype, success_count, fail_count) values('{0}', '{1}', '{2}', {3}, {4})";
                foreach (var item in list)
                {
                    if (item.RecordType == RecordType.Success)
                        successCount += item.RecordCount;
                    else if (item.RecordType == RecordType.Failed)
                        failCount += 1;
                }
                proxy.CommandText = string.Format(command, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), username, type, successCount, failCount);
                proxy.ExecuteNonQuery();
                proxy.CommandText = "select @@identity";
                IConvertible c = proxy.ExecuteScalar() as IConvertible;
                int id = c == null ? -1 : c.ToInt32(null);
                if (id == -1)
                {
                    proxy.Rollback();
                    throw new ArgumentException("记录填报历史时发生错误！");
                }
                command = "insert into FlEX_DECLARATION_ITEM (declaration_info_id, element_type, total_count, param_name, status, reason) values ({0}, {1}, {2}, '{3}', {4}, '{5}')";
                foreach (var item in list)
                {
                    proxy.CommandText = string.Format(command, id, (int)item.ElementType, item.RecordCount, item.ParaName == null ? DBNull.Value : (object)item.ParaName, (int)item.RecordType, item.Note);
                    proxy.ExecuteNonQuery();
                }
                proxy.Commit();
            });
        }

        public static List<HistoryItem> GetHistoryList()
        {
            List<HistoryItem> list = new List<HistoryItem>();
            Local.ExecuteCommand((proxy) =>
            {
                proxy.CommandText = "select ID, filldate, username, filltype, success_count, fail_count from FlEX_DECLARATION_INFO";
                using (DbDataReader reader = proxy.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new HistoryItem()
                        {
                            Id = (int)reader["ID"],
                            UserName = reader["username"] as string,
                            FillType = reader["filltype"] as string,
                            FillDate = (DateTime)reader["filldate"],
                            FailCount = (int)reader["fail_count"],
                            SuccessCount = (int)reader["success_count"]
                        });
                    }
                }
            });
            return list;
        }

        public static List<FillRecord> GetRecordList(int id)
        {
            List<FillRecord> list = new List<FillRecord>();
            Local.ExecuteCommand((proxy) =>
            {
                proxy.CommandText = string.Format("select param_name, element_type, total_count, status, reason from FlEX_DECLARATION_ITEM where declaration_info_id = {0}", id);
                using (DbDataReader reader = proxy.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new FillRecord((ElementType)((int)reader["element_type"]), (RecordType)((int)reader["status"]), reader["reason"] as string, reader["param_name"] as string)
                        {
                            RecordCount = (int)reader["total_count"]
                        });
                    }
                }
            });
            return list;
        }
    }
}
