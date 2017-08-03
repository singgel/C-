using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Devart.Data.Oracle;

namespace OracleUtils
{
    public class OracleUtils
    {
        public static OracleConnection getConnection(String server, int port, String sid, String username, string password)
        {
            try
            {
                OracleConnectionStringBuilder oraCSB = new OracleConnectionStringBuilder();
                oraCSB.Direct = true;
                oraCSB.Server = server;
                oraCSB.Port = port;
                oraCSB.Sid = sid;
                oraCSB.UserId = username;
                oraCSB.Password = password;
                oraCSB.MaxPoolSize = 150;
                oraCSB.ConnectionTimeout = 30;
                return new OracleConnection(oraCSB.ToString());
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //增删改查 都已经测试完毕了
        public void connOracleUsingDevartReading(OracleConnection conn)
        {
            try
            {
                conn.Open();
                OracleCommand command = conn.CreateCommand();
                command.CommandText = "select count(*) from ti_task";
                using (OracleDataReader reader = command.ExecuteReader())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        MessageBox.Show(reader.GetName(i).ToString());
                    }
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            MessageBox.Show(reader.GetValue(i).ToString() + "\t");

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        public static Boolean executeCommand(OracleConnection conn, String text)
        {
            if (conn == null) {
                throw new Exception("连接为空");
            }
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed) {
                    conn.Open();
                }
                OracleCommand command = conn.CreateCommand();
                command.CommandText = text;
                int affectedCounter = command.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //conn.Close();
            }
        }

        public static Boolean executeBatCommand(OracleConnection conn, List<String> listCommand)
        {
            if (conn == null)
            {
                throw new Exception("连接为空");
            }
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed) {
                    conn.Open();
                }
                foreach (String s in listCommand) {
                    OracleCommand command = conn.CreateCommand();
                    command.CommandText = s;
                    int affectedCounter = command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //conn.Close();
            }
        
        }
    }
}
