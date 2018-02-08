using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SQLHelper
{
    public class SqlHelper
    {

        /// <summary>
        /// 执行sql语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public int ExecuteSql(string connectionString, string sqlString)
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
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// 执行多条sql语句，实现数据库事务,每条sql存在数组sqlStringList中。
        /// </summary>
        /// <param name="sqlStringList"></param>
        public int ExecuteSqlTran(string connectionString, List<string> sqlStringList)
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
                                //throw new Exception();
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
        ///使用现有的SQL事务执行一个sql命令（不返回数据集）
        /// </summary>
        /// <remarks>
        ///举例: 
        ///  int result = ExecuteNonQuery(trans, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">一个现有的事务</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public int ExecuteNonQuery(SqlTransaction trans, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        ///  给定连接的数据库用假设参数执行一个sql命令（不返回数据集）
        /// </summary>
        /// <param name="connectionString">一个有效的连接字符串</param>
        /// <param name="commandText">存储过程名称或者sql命令语句</param>
        /// <param name="commandParameters">执行命令所用参数的集合</param>
        /// <returns>执行命令所影响的行数</returns>
        public int ExecuteNonQuery(string connectionString, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="sqlString"></param>
        /// <returns></returns>
        public DataTable QuerySingleDT(string connectionString, string sqlString)
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
        public DataSet QuerySingleDS(string connectionString, string sqlString)
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
        public int GetCount(string connectionString, string sqlString)
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
        public object QuerySingleObject(string connectionString, string sqlString)
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
        public DataTable QueryProcedureDT(string connectionString, string sprocName, params object[] paraValues)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// 执行存储过程，返回 System.Data.DataSet。
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="paraValues">传递给存储过程的参数值列表</param>
        /// <returns></returns>
        public DataSet QueryProcedureDS(string connectionString, string sprocName, params object[] paraValues)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand command = new SqlCommand(sprocName, connection);
                    command.CommandType = CommandType.StoredProcedure;

                    this.DeriveParameters(command);
                    this.AssignParameterValues(command, paraValues);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet();

                    adapter.Fill(dataSet);

                    return dataSet;
                }
                catch (Exception ex)
                {
                    throw ex;
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

        /// <summary>
        /// 准备执行一个命令
        /// </summary>
        /// <param name="cmd">sql命令</param>
        /// <param name="conn">Sql连接</param>
        /// <param name="trans">Sql事务</param>
        /// <param name="cmdText">命令文本,例如：Select * from Products</param>
        /// <param name="cmdParms">执行命令的参数</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandTimeout = 60;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            if (cmdText.IndexOf(' ') > -1)
                cmd.CommandType = CommandType.Text;
            else
                cmd.CommandType = CommandType.StoredProcedure;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }


    }
}
