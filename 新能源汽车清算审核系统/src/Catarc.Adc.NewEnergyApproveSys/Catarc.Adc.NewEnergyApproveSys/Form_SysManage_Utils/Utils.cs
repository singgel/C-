using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using Catarc.Adc.NewEnergyApproveSys.DataUtils;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;

namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage_Utils
{
    class Utils
    {

        public Dictionary<string, string> dictDic;
        public Dictionary<string, string> dictNotice;//存放列头转换模板（数据字典）
        public Dictionary<string, string> dictSubsidy;//存放列头转换模板（数据字典）
        readonly string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate"];
        
        public Utils()
        {
            ReadTemplate(path);   //读取表头转置模板
        }
        //导入数据字典信息
        public string ImportDicData(string fileName)
        {
            string msg = string.Empty;
            int successCount = 0;
            int totalCount = 0;
            //先读取Excel中数据
            DataSet ds = this.ReadExcel(fileName, "");
            if (ds != null)
            {
                totalCount = ds.Tables[0].Rows.Count;
                successCount = SaveMainData(ds.Tables[0], ref msg);  
            }
            else
            {
                msg = fileName + "中没有数据或数据格式错误\r\n";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条导入成功 \r\n \t{2}条导入失败\r\n",
                         totalCount, successCount, totalCount - successCount);
            msg = msgSummary + msg;
            return msg;
        }

        //导入公告信息
        public string ImportNoticeData(string fileName)
        {
            string msg = string.Empty;
            int successCount = 0;
            int updateCount = 0;
            int totalCount = 0;
            //先读取Excel中数据
            DataSet ds = this.ReadExcel(fileName, "");
            if (ds != null)
            {
                //转换表头
                DataTable dtNotice = D2D(dictNotice, ds.Tables[0]);
                totalCount = dtNotice.Rows.Count;
                successCount = SaveNoticeData(dtNotice, ref updateCount, ref msg);
            }
            else
            {
                msg = fileName + "中没有数据或数据格式错误\r\n";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条插入成功\r\n \t{2}条修改成功 \r\n \t{3}条导入失败\r\n",
                         totalCount, successCount, updateCount, totalCount - successCount - updateCount);
            msg = msgSummary + msg;
            return msg;
        }

        //导入补贴标准
        public string ImportSubsidyData(string fileName)
        {
            string msg = string.Empty;
            int successCount = 0;
            int updateCount = 0;
            int totalCount = 0;
            //先读取Excel中数据
            DataSet ds = this.ReadExcel(fileName, "");
            if (ds != null)
            {
                //转换表头
                DataTable dtSubsidy = D2D(dictSubsidy, ds.Tables[0]);
                totalCount = dtSubsidy.Rows.Count;
                successCount = SaveSubsidyData(dtSubsidy, ref updateCount);
            }
            else
            {
                msg = fileName + "中没有数据或数据格式错误\r\n";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条导入成功 \r\n \t{2}条修改成功 \r\n \t{3}条导入、修改失败\r\n",
                         totalCount, successCount, updateCount, totalCount - successCount - updateCount);
            msg = msgSummary + msg;
            return msg;
        }

        private void ReadTemplate(string filePath)
        {
            DataSet ds = this.ReadTemplateExcel(filePath);

            dictDic = new Dictionary<string, string>();
            dictNotice = new Dictionary<string, string>();
            dictSubsidy = new Dictionary<string, string>();
            foreach (DataRow r in ds.Tables["Dic"].Rows)
            {
                dictDic.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
            foreach (DataRow r in ds.Tables["Notice"].Rows)
            {
                dictNotice.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
            foreach (DataRow r in ds.Tables["Subsidy"].Rows)
            {
                dictSubsidy.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
        }
        string userId = string.Empty;

        //导入数据字典信息
        public int SaveMainData(DataTable dt, ref string message)
        {
            int succcount = 0;
            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }
           
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                    {
                        conn.Open();
                        using (OracleTransaction trans = conn.BeginTransaction())
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                try
                                {
                                    if (OracleHelper.Exists(OracleHelper.conn, string.Format("SELECT COUNT(*) FROM SYS_DIC WHERE DIC_NAME='{0}' and DIC_TYPE = '{1}'", dr[0].ToString().Trim(), dt.Columns[0].ColumnName)))
                                    {
                                        message += String.Format("{0}:{1},已存在\r\n", dt.Columns[0].ColumnName, dr[0]);
                                        continue;
                                    }

                                    if (string.IsNullOrEmpty(userId))
                                    {
                                        userId = "SEQ_SYS_DIC.nextval";
                                    }
                                    string sql = string.Format("Insert into SYS_DIC (ID,DIC_NAME,DIC_TYPE) values ({0},'{1}','{2}')", userId, dr[0].ToString().Trim(), dt.Columns[0].ColumnName);
                                    OracleHelper.ExecuteNonQuery(trans, sql, null);
                                    succcount++;
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    throw ex;
                                }
                            }
                            if (trans.Connection != null) trans.Commit();
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return succcount;
        }

        //导入公告参数
        public int SaveNoticeData(DataTable dt, ref int updateCount, ref string message)
        {
            int succcount = 0;
            if (string.IsNullOrEmpty(message))
            {
                message = string.Empty;
            }

            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                    {
                        conn.Open();
                        using (OracleTransaction trans = conn.BeginTransaction())
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                bool intsertTypeFlag = true;
                                try
                                {
                                    var idObj = OracleHelper.GetSingle(OracleHelper.conn, string.Format("SELECT ID FROM DB_NOTICEPARAM WHERE CLXH='{0}' and CONFIGID='{1}' and TJMUFBSJ=to_date('{2}','yyyy-mm-dd hh24:mi:ss')", dr["CLXH"].ToString().Trim(), dr["CONFIGID"].ToString().Trim(), dr["TJMUFBSJ"].ToString().Trim()));
                                    //如果包含车辆型号 则更新
                                    if (idObj != null)
                                    {
                                        intsertTypeFlag = false;
                                        OracleHelper.ExecuteNonQuery(trans, string.Format("DELETE FROM DB_NOTICEPARAM WHERE ID='{0}'", idObj.ToString()));
                                    }
                                    OracleParameter[] parameters = 
                                            {
				                                new OracleParameter("CLSCQY", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CLXH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CLMC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CONFIGID", OracleDbType.NVarchar2,255),
                                                new OracleParameter("MLPC", OracleDbType.NVarchar2,255),
                                                new OracleParameter("CLENGTH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("EKG", OracleDbType.NVarchar2,255),
				                                new OracleParameter("XSLCGKF", OracleDbType.NVarchar2,255),
				                                new OracleParameter("XSLCDSF", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CDDXSLCGKF", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CDDXSLCDSF", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CNZZZZ", OracleDbType.NVarchar2,255),
				                                new OracleParameter("DTXH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("DTSCQY", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CXXH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("DCZZRL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("DCZSCQY", OracleDbType.NVarchar2,255),
				                                new OracleParameter("QDDJXH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("QDDJEDGL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("QDDJSCQY", OracleDbType.NVarchar2,255),
				                                new OracleParameter("RLDCXH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("RLDCEDGL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("RLDCSCQY", OracleDbType.NVarchar2,255),
				                                new OracleParameter("TJMUFBSJ", OracleDbType.Date),
				                                new OracleParameter("GGPC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("SJLY", OracleDbType.NVarchar2,255)
                                            };
                                    parameters[0].Value = dr["CLSCQY"].ToString().Trim();
                                    parameters[1].Value = dr["CLXH"].ToString().Trim();
                                    parameters[2].Value = dr["CLMC"].ToString().Trim();
                                    parameters[3].Value = dr["CONFIGID"].ToString().Trim();
                                    parameters[4].Value = dr["MLPC"].ToString().Trim();
                                    parameters[5].Value = dr["CLENGTH"].ToString().Trim();
                                    parameters[6].Value = dr["EKG"].ToString().Trim();
                                    parameters[7].Value = dr["XSLCGKF"].ToString().Trim();
                                    parameters[8].Value = dr["XSLCDSF"].ToString().Trim();
                                    parameters[9].Value = dr["CDDXSLCGKF"].ToString().Trim();
                                    parameters[10].Value = dr["CDDXSLCDSF"].ToString().Trim();
                                    parameters[11].Value = dr["CNZZZZ"].ToString().Trim();
                                    parameters[12].Value = dr["DTXH"].ToString().Trim();
                                    parameters[13].Value = dr["DTSCQY"].ToString().Trim();
                                    parameters[14].Value = dr["CXXH"].ToString().Trim();
                                    parameters[15].Value = dr["DCZZRL"].ToString().Trim();
                                    parameters[16].Value = dr["DCZSCQY"].ToString().Trim();
                                    parameters[17].Value = dr["QDDJXH"].ToString().Trim();
                                    parameters[18].Value = dr["QDDJEDGL"].ToString().Trim();
                                    parameters[19].Value = dr["QDDJSCQY"].ToString().Trim();
                                    parameters[20].Value = dr["RLDCXH"].ToString().Trim();
                                    parameters[21].Value = dr["RLDCEDGL"].ToString().Trim();
                                    parameters[22].Value = dr["RLDCSCQY"].ToString().Trim();
                                    parameters[23].Value = Convert.ToDateTime(dr["TJMUFBSJ"]);
                                    parameters[24].Value = dr["GGPC"].ToString().Trim();
                                    parameters[25].Value = dr["SJLY"].ToString().Trim();

                                    OracleHelper.ExecuteNonQuery(trans, "Insert into DB_NOTICEPARAM (CLSCQY,CLXH,CLMC,CONFIGID,MLPC,CLENGTH,EKG,XSLCGKF,XSLCDSF,CDDXSLCGKF,CDDXSLCDSF,CNZZZZ,DTXH,DTSCQY,CXXH,DCZZRL,DCZSCQY,QDDJXH,QDDJEDGL,QDDJSCQY,RLDCXH,RLDCEDGL,RLDCSCQY,TJMUFBSJ,GGPC,SJLY) values (:CLSCQY,:CLXH,:CLMC,:CONFIGID,:MLPC,:CLENGTH,:EKG,:XSLCGKF,:XSLCDSF,:CDDXSLCGKF,:CDDXSLCDSF,:CNZZZZ,:DTXH,:DTSCQY,:CXXH,:DCZZRL,:DCZSCQY,:QDDJXH,:QDDJEDGL,:QDDJSCQY,:RLDCXH,:RLDCEDGL,:RLDCSCQY,:TJMUFBSJ,:GGPC,:SJLY)", parameters);
                                    if (intsertTypeFlag)
                                    {
                                        succcount++;
                                    }
                                    else
                                    {
                                        updateCount++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    throw ex;
                                }
                            }
                            if (trans.Connection != null) trans.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return succcount;
        }

        //导入补贴标准参数
        public int SaveSubsidyData(DataTable dt, ref int updateCount)
        {
            int succcount = 0;
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                    {
                        conn.Open();
                        using (OracleTransaction trans = conn.BeginTransaction())
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                try
                                {
                                    bool intsertTypeFlag = true;
                                    string strSql = string.Empty;
                                    //如果包含车辆型号 则更新
                                    if (OracleHelper.Exists(OracleHelper.conn, string.Format("SELECT COUNT(CLXH) FROM DB_SUBSIDY WHERE CLXH='{0}' and TJMLFBSJ=to_date('{1}','yyyy-mm-dd hh24:mi:ss')", dr["CLXH"].ToString().Trim(), dr["TJMLFBSJ"].ToString().Trim())))
                                    {
                                        intsertTypeFlag = false;
                                        strSql = String.Format("Update DB_SUBSIDY set CLSCQY='{0}',BTBZ='{1}',TJMLFBSJ=to_date('{2}','yyyy-mm-dd hh24:mi:ss') where CLXH ='{3}'", dr["CLSCQY"].ToString().Trim(), dr["BTBZ"].ToString().Trim(), dr["TJMLFBSJ"].ToString().Trim(), dr["CLXH"].ToString().Trim());
                                    }
                                    else
                                    {
                                        strSql = String.Format("Insert into DB_SUBSIDY (CLSCQY,CLXH,BTBZ,TJMLFBSJ) values ('{0}','{1}','{2}',to_date('{3}','yyyy-mm-dd hh24:mi:ss')) ", dr["CLSCQY"].ToString().Trim(), dr["CLXH"].ToString().Trim(), dr["BTBZ"].ToString().Trim(), dr["TJMLFBSJ"].ToString().Trim());
                                    }

                                    OracleHelper.ExecuteNonQuery(trans, strSql, null);
                                    if (intsertTypeFlag)
                                    {
                                        succcount++;
                                    }
                                    else
                                    {
                                        updateCount++;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    trans.Rollback();
                                    throw ex;
                                }
                            }
                            if (trans != null) trans.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return succcount;
        }
        // 读表头对应关系模板
        public DataSet ReadTemplateExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName);
            using (OleDbConnection objConn = new OleDbConnection(strConn))
            {
                DataSet ds = new DataSet();
                try
                {
                    objConn.Open();
                    // 取得Excel工作簿中所有工作表  
                    using (DataTable schemaTable = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null))
                    {
                        using (OleDbDataAdapter sqlada = new OleDbDataAdapter())
                        {
                            foreach (DataRow dr in schemaTable.Rows)
                            {
                                string strSql = String.Format("Select * From [{0}]", dr[2].ToString().Trim());
                                OleDbCommand objCmd = new OleDbCommand(strSql, objConn);
                                sqlada.SelectCommand = objCmd;
                                sqlada.Fill(ds, dr[2].ToString().Trim().TrimEnd('$'));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                DataTableHelper.removeEmpty(ds);
                return ds;

            }

        }
        //导入Excel
        public DataSet ReadExcel(string fileName, string sheet)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            OleDbConnection conn = new OleDbConnection(strConn);
            try
            {
                conn.Open();
                DataTable sheetNames = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (sheetNames == null)
                {
                    return null;
                }
                else
                {
                    if (string.IsNullOrEmpty(sheet))
                    {
                        sheet = sheetNames.Rows[0]["TABLE_NAME"].ToString();
                    }
                    else
                    {
                        sheet = sheet + "$";
                    }
                    using (OleDbDataAdapter oada = new OleDbDataAdapter(String.Format("select * from [{0}]", sheet), strConn))
                    {
                        oada.Fill(ds, sheet.IndexOf('$') > 0 ? sheet.Substring(0, sheet.Length - 1) : sheet);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        //转换表头
        public DataTable D2D(Dictionary<string, string> dict, DataTable dt)
        {
            DataTable d = new DataTable();
            for (int i = 0; i < dt.Columns.Count; )
            {
                DataColumn c = dt.Columns[i];
                if (!dict.ContainsKey(c.ColumnName))
                {
                    dt.Columns.Remove(c);
                    continue;
                }
                d.Columns.Add(dict[c.ColumnName]);
                i++;
            }

            foreach (DataRow r in dt.Rows)
            {
                // 判断第一列是否为空，为空则认为此行数据无效
                if (r[0] != null && !string.IsNullOrEmpty(r[0].ToString()))
                {
                    DataRow ddr = d.NewRow();
                    ddr = r;
                    d.Rows.Add(ddr.ItemArray);
                }
            }

            return d;
        }

    }
}
