using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;
using System.Windows.Forms;

namespace FuelDataSysServer.Tool
{
    class StatisticsData
    {
        TextBox txtDetails; //主窗体的统计控件

        public void Statistics(TextBox textBox)
        {
            this.txtDetails = textBox;
            var socket = SocketDetailed();
            var mergerSucc = MergerDetailed();
            var mergerError = MergerErrorDetailed();
            var mergerNormal = MergerNormalDetailed();
            var uploadSucc = UploadDetailed();
            var uploadError = UploadErrorDetailed();
            string countMsg = @"=== 统计" + DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd") + "结果如下 " + DateTime.Now.ToString("G") + "===" + Environment.NewLine
                                       + "Socket共" + socket.Rows.Count + "条接收数据" + Environment.NewLine
                                       + "Merger共" + mergerSucc.Rows.Count + "条合并成功数据" + Environment.NewLine
                                       + "Merger共" + mergerError.Rows.Count + "条合并失败数据" + Environment.NewLine
                                       + "Merger共" + mergerNormal.Rows.Count + "条未合并数据" + Environment.NewLine
                                       + "Upload共" + uploadSucc.Rows.Count + "条上传成功数据" + Environment.NewLine
                                       + "Upload共" + uploadError.Rows.Count + "条上传失败数据" + Environment.NewLine
                                       + "=== 详细内容如下 ===" + Environment.NewLine;
            string socketVin = string.Empty;
            foreach (DataRow dr in socket.Rows)
            {
                socketVin += dr["VIN"] + Environment.NewLine;
            }
            string mergerSuccVin = string.Empty;
            foreach (DataRow dr in mergerSucc.Rows)
            {
                mergerSuccVin += dr["VIN"] + Environment.NewLine;
            }
            string mergerErrorVin = string.Empty;
            foreach (DataRow dr in mergerError.Rows)
            {
                mergerErrorVin += dr["VIN"] + Environment.NewLine;
            }
            string mergerNormalVin = string.Empty;
            foreach (DataRow dr in mergerNormal.Rows)
            {
                mergerNormalVin += dr["VIN"] + Environment.NewLine;
            }
            string uploadSuccVin = string.Empty;
            foreach (DataRow dr in uploadSucc.Rows)
            {
                uploadSuccVin += dr["VIN"] + Environment.NewLine;
            }
            string uploadErrorVin = string.Empty;
            foreach (DataRow dr in uploadError.Rows)
            {
                uploadErrorVin += dr["VIN"] + Environment.NewLine;
            }
            string msg = countMsg +
                @"== Socket接收数据 ==" + Environment.NewLine + socketVin
                + @"== Merger合并成功数据 ==" + Environment.NewLine + mergerSuccVin
                + @"== Merger合并失败数据 ==" + Environment.NewLine + mergerErrorVin
                + @"== Merger未合并数据 ==" + Environment.NewLine + mergerNormalVin
                + @"== Upload上传成功数据 ==" + Environment.NewLine + uploadSuccVin
                + @"== Upload上传失败数据 ==" + Environment.NewLine + uploadErrorVin;
            this.txtDetails.Invoke(new Action(() =>
            {
                this.txtDetails.Text = msg;
            }));
            LogManager.Log("StatisticsLog", "CountMessage", msg);
        }

        //统计上报成功详细信息
        private DataTable UploadDetailed()
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    string sql = string.Format("select VIN,STATUS from FC_CLJBXX where to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and status='0'", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd"));
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                }
                catch(Exception ex) 
                {
                    LogManager.Log("StatisticsLog", "Error", ex.Message);
                    dt = null; 
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Log("StatisticsLog", "Error", ex.Message);
                return new DataTable(); 
            }
        }
        //统计上报失败详细信息
        private DataTable UploadErrorDetailed()
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    string sql = string.Format("select VIN,STATUS from FC_CLJBXX where to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and status='1'", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd"));
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    LogManager.Log("StatisticsLog", "Error", ex.Message);
                    dt = null;
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Log("StatisticsLog", "Error", ex.Message);
                return new DataTable();
            }
        }
        //统计合并成功详细信息
        private DataTable MergerDetailed()
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    string sql = string.Format("select VIN,SC_OCN,CLZZRQ from VIN_INFO where to_date(to_char(CREATE_TIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and MERGER_STATUS=1", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd"));
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    LogManager.Log("StatisticsLog", "Error", ex.Message); 
                    dt = null;
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Log("StatisticsLog", "Error", ex.Message); 
                return new DataTable();
            }
        }
        //统计合并失败详细信息
        private DataTable MergerErrorDetailed()
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    string sql = string.Format("select VIN,SC_OCN,CLZZRQ from VIN_INFO where to_date(to_char(CREATE_TIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and MERGER_STATUS=2", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd"));
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    LogManager.Log("StatisticsLog", "Error", ex.Message); 
                    dt = null;
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Log("StatisticsLog", "Error", ex.Message); 
                return new DataTable();
            }
        }
        //统计未合并详细信息
        private DataTable MergerNormalDetailed()
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    string sql = string.Format("select VIN,SC_OCN,CLZZRQ from VIN_INFO where to_date(to_char(CREATE_TIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') and MERGER_STATUS=0", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd"));
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    LogManager.Log("StatisticsLog", "Error", ex.Message);
                    dt = null;
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Log("StatisticsLog", "Error", ex.Message);
                return new DataTable();
            }
        }
        //统计Socket接收信息
        private DataTable SocketDetailed()
        {
            try
            {
                DataTable dt = new DataTable();
                try
                {
                    string sql = string.Format("select VIN,SC_OCN,CLZZRQ from VIN_INFO where to_date(to_char(CREATE_TIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss')", DateTime.Today.AddDays(-1).ToString("yyyy/MM/dd"));
                    DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                        dt = ds.Tables[0];
                }
                catch (Exception ex)
                {
                    LogManager.Log("StatisticsLog", "Error", ex.Message); 
                    dt = null;
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogManager.Log("StatisticsLog", "Error", ex.Message); 
                return new DataTable();
            }
        }
    }
}
