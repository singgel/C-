using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace FuelDataSysClient.FuelCafc
{
    public class CalcDBHelper
    {
        protected string connectionString = "Data Source=.;Initial Catalog=油耗核算数据库;Persist Security Info=True;User ID=adc;Password=catarcadc2012; Connect Timeout=3000";

        /// <summary>
        /// 执行sql语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public int ExecuteSql(string sqlString)
        {
            int rows = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        rows = cmd.ExecuteNonQuery();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        // log
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// 执行多条sql语句，实现数据库事务,每条sql存在数组sqlStringList中。
        /// </summary>
        /// <param name="sqlStringList"></param>
        public int ExecuteSqlTran(List<string> sqlStringList)
        {
            int rows = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                SqlTransaction sqlTran = connection.BeginTransaction();
                cmd.Transaction = sqlTran;
                try
                {
                    foreach (string sqlStr in sqlStringList)
                    {
                        if (sqlStr.Trim() != "")
                        {
                            cmd.CommandText = sqlStr;
                            rows = cmd.ExecuteNonQuery();
                            if (rows < 1)
                            {
                                throw new Exception();
                            }
                        }
                    }
                    sqlTran.Commit();
                }
                catch (Exception e)
                {
                    sqlTran.Rollback();
                    throw e;
                }
                finally
                {
                    connection.Close();
                }
            }
            return rows;
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public DataTable QuerySingleDT(string sqlString)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    Connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(sqlString, Connection);
                    command.Fill(dt);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return dt;
            }

        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public DataSet QuerySingleDS(string sqlString)
        {
            using (SqlConnection Connection = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    Connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(sqlString, Connection);
                    command.Fill(ds);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }

        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（整数）。
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public int GetCount(string sqlString)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlDataAdapter command = new SqlDataAdapter(sqlString, connection);
                    command.Fill(dt);
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return dt.Rows.Count;
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public object QuerySingleObject(string sqlString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(sqlString, connection);
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((object.Equals(obj, null)) || (object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行存储过程，返回 System.Data.DataTable。
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="paraValues">传递给存储过程的参数值列表</param>
        /// <returns></returns>
        public DataTable QueryProcedureDT(string sprocName, params object[] paraValues)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(sprocName, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    this.DeriveParameters(command);
                    this.AssignParameterValues(command, paraValues);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    return dataTable;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 从在 System.Data.SqlClient.SqlCommand 中指定的存储过程中检索参数信息并填充指定的 
        /// System.Data.SqlClient.SqlCommand 对象的 System.Data.SqlClient.SqlCommand.Parameters 集合。
        /// </summary>
        /// <param name="sqlCommand">将从其中导出参数信息的存储过程的 System.Data.SqlClient.SqlCommand 对象。</param>
        internal void DeriveParameters(SqlCommand sqlCommand)
        {
            try
            {
                sqlCommand.Connection.Open();
                SqlCommandBuilder.DeriveParameters(sqlCommand);
                sqlCommand.Connection.Close();
            }
            catch
            {
                if (sqlCommand.Connection != null)
                {
                    sqlCommand.Connection.Close();
                }
                throw;
            }
        }

        /// <summary>
        /// 用指定的参数值列表为存储过程参数赋值。
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="paraValues"></param>
        private void AssignParameterValues(SqlCommand sqlCommand, params object[] paraValues)
        {
            if (paraValues != null)
            {
                if ((sqlCommand.Parameters.Count - 1) != paraValues.Length)
                {
                    throw new ArgumentNullException("The number of parameters does not match number of values for stored procedure.");
                }
                for (int i = 0; i < paraValues.Length; i++)
                {
                    sqlCommand.Parameters[i + 1].Value = (paraValues[i] == null) ? DBNull.Value : paraValues[i];
                }
            }
        }

        
    }
}
