using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using FuelDataSysClient.Properties;

namespace FuelDataSysClient.Tool.Tool_Volkswagen
{
    public class ImportCsv
    {
        DataTable checkData = new DataTable();
        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate_Volkswagen"];
        private List<string> listHoliday; // 节假日数据

        public ImportCsv()
        {
            //获取参数数据  RLLX_PARAM  
            checkData = GetCheckData();    
            // 获取节假日数据
            listHoliday = this.GetHoliday();
        }

        /// <summary>
        /// 读取Excel文件
        /// </summary>
        /// <param name="notPath">待上传路径文件夹</param>
        /// <param name="yesPath">已上传路径文件夹</param>
        /// <param name="isMove">是否自动导入</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> ReadCsv(string filePath)
        {
            Dictionary<string, Dictionary<string, string>> fileList = new Dictionary<string, Dictionary<string, string>>();
            ProcessForm pf = new ProcessForm();
            try
            {
                IList<string> fileNameList = GetFileName(filePath, "*.xls",fileList);
                if (fileNameList.Count > 0)
                {
                    // 显示进度条
                    pf.Show();
                    int pageSize = 1;
                    int total = fileNameList.Count * 5;
                    pf.TotalMax = (int)Math.Ceiling((decimal)total / (decimal)pageSize);
                    pf.ShowProcessBar();
                    foreach (string fileName in fileNameList)
                    {
                        // step1:处理完Excel
                        DataSet ds = SwitchColumnName(ReadExcel(fileName));
                        // step2:Excel数据验证
                        List<string> vinSuccessed = new List<string>();
                        Dictionary<string, string> error = VerifyData(ds.Tables["基本信息"], ref vinSuccessed);
                        if (error.Count == 0)
                        {
                            fileList.Add(fileName + "\r\n==========导入完成==========", new Dictionary<string, string>());
                            for (int i = 0; i < ds.Tables.Count; i++)
                            {
                                if (ds.Tables[i] == null || ds.Tables[i].Rows.Count < 1 || ds.Tables[i].TableName.Equals("基本信息"))
                                    continue;
                                error = VerifyData(ds.Tables[i], ref vinSuccessed);
                                if (error.Count == 0)
                                {
                                    //step3:燃料数据匹配验证
                                    string msg = InsertData(ds.Tables["基本信息"], ds.Tables[i], ref vinSuccessed);
                                    fileList.Add(String.Format("{0}中{1}", fileName, String.Format("【{0}】", ds.Tables[i].TableName)), new Dictionary<string, string>() { { msg, string.Empty } });
                                }
                                else
                                {
                                    fileList.Add(String.Format("{0}中{1}", fileName, String.Format("【{0}】", ds.Tables[i].TableName)), error);
                                }
                                pf.progressBarControl1.PerformStep();
                            }
                            //step4:基本信息匹配验证
                            DataTable vinFail = (from d in ds.Tables["基本信息"].AsEnumerable()
                                          where !vinSuccessed.Contains(d.Field<string>("VIN"))
                                                 select d).AsDataView().ToTable();
                            error = new Dictionary<string, string>();
                            foreach (DataRow r in vinFail.Rows)
                            {
                                error.Add(Convert.ToString(r["VIN"]), String.Format("没有对应的#{0}#燃料参数信息", r["RLLX"]));
                            }
                            fileList.Add(fileName + "【基本信息】", error);
                            MoveFile(GetPathFileName(filePath, fileName), Path.Combine(filePath, "已处理油耗数据"));
                            fileList.Add(fileName + "\r\n==========处理完成==========", new Dictionary<string, string>());
                        }
                        else
                        {
                            fileList.Add(fileName + "\r\n==========导入失败==========\r\n【基本信息】", error);
                            fileList.Add(fileName + "\r\n==========处理完成==========", new Dictionary<string, string>());
                        }
                    }
                    Application.DoEvents();
                    fileList.Add("==========全部处理完成==========", new Dictionary<string, string>());
                }
            }
            catch (Exception ex) 
            { 
                fileList.Add(ex.Message, new Dictionary<string, string>());
            }
            finally
            {
                if (pf != null)
                {
                    pf.Close();
                }
            }
            return fileList;
        }

        private DataSet SwitchColumnName(DataSet ds)
        {
            DataSet dsNew = new DataSet();
            using (DataSet dsTemp = ReadExcel(path))
            {
                Dictionary<string, string> dictJBXX = new Dictionary<string, string>();
                Dictionary<string, string> dictCTNY = new Dictionary<string, string>();
                Dictionary<string, string> dictFCDSHHDL = new Dictionary<string, string>();
                Dictionary<string, string> dictCDSHHDL = new Dictionary<string, string>();
                Dictionary<string, string> dictCDD = new Dictionary<string, string>();
                Dictionary<string, string> dictRLDC = new Dictionary<string, string>();
                foreach (DataRow r in dsTemp.Tables["基本信息"].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(r[0])))
                        continue;
                    dictJBXX.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                foreach (DataRow r in dsTemp.Tables["传统能源"].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(r[0])))
                        continue;
                    dictCTNY.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                foreach (DataRow r in dsTemp.Tables["非插电式混合动力"].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(r[0])))
                        continue;
                    dictFCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                foreach (DataRow r in dsTemp.Tables["插电式混合动力"].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(r[0])))
                        continue;
                    dictCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                foreach (DataRow r in dsTemp.Tables["纯电动"].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(r[0])))
                        continue;
                    dictCDD.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                foreach (DataRow r in dsTemp.Tables["燃料电池"].Rows)
                {
                    if (string.IsNullOrEmpty(Convert.ToString(r[0])))
                        continue;
                    dictRLDC.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                //避免用户的excel缺少表，或者表名错误，采用for循环判断
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].TableName.Equals("vehicleBasicInfo"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictJBXX.ContainsKey(dc.ColumnName))
                            {
                                //ds.Tables[i].Columns.Remove(dc.ColumnName);
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictJBXX[dc.ColumnName];
                        }
                        DataView dv = ds.Tables[i].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        dt.TableName = "基本信息";
                        dsNew.Tables.Add(dt);
                    }
                    if (ds.Tables[i].TableName.Equals("CT"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictCTNY.ContainsKey(dc.ColumnName))
                            {
                                //ds.Tables[i].Columns.Remove(dc.ColumnName);
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictCTNY[dc.ColumnName];
                        }
                        DataView dv = ds.Tables[i].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        dt.TableName = "传统能源";
                        dsNew.Tables.Add(dt);
                    }
                    if (ds.Tables[i].TableName.Equals("FCDS_HHDL"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictFCDSHHDL.ContainsKey(dc.ColumnName))
                            {
                                //ds.Tables[i].Columns.Remove(dc.ColumnName);
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictFCDSHHDL[dc.ColumnName];
                        }
                        DataView dv = ds.Tables[i].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        dt.TableName = "非插电式混合动力";
                        dsNew.Tables.Add(dt);
                    }
                    if (ds.Tables[i].TableName.Equals("CDS_HHDL"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictCDSHHDL.ContainsKey(dc.ColumnName))
                            {
                                //ds.Tables[i].Columns.Remove(dc.ColumnName);
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictCDSHHDL[dc.ColumnName];
                        }
                        DataView dv = ds.Tables[i].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        dt.TableName = "插电式混合动力";
                        dsNew.Tables.Add(dt);
                    }
                    if (ds.Tables[i].TableName.Equals("CDD"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictCDD.ContainsKey(dc.ColumnName))
                            {
                                //ds.Tables[i].Columns.Remove(dc.ColumnName);
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictCDD[dc.ColumnName];
                        }
                        DataView dv = ds.Tables[i].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        dt.TableName = "纯电动";
                        dsNew.Tables.Add(dt);
                    }
                    if (ds.Tables[i].TableName.Equals("RLDC"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictRLDC.ContainsKey(dc.ColumnName))
                            {
                                //ds.Tables[i].Columns.Remove(dc.ColumnName);
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictRLDC[dc.ColumnName];
                        }
                        DataView dv = ds.Tables[i].DefaultView;
                        dv.RowFilter = "VIN <> '' or VIN is not null";
                        DataTable dt = dv.ToTable();
                        dt.TableName = "燃料电池";
                        dsNew.Tables.Add(dt);
                    }
                }
            }
            return dsNew;
        }

        /// <summary>
        /// 验证当前DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>返回VIN码与错误信息</returns>
        private Dictionary<string, string> VerifyData(DataTable dt, ref List<string> vinSuccessed)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            DataRow[] dr = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", dt.TableName));
            foreach (DataRow r in dt.Rows)
            {
                string error = VerifyData(r, dr, dt.TableName);
                if (!string.IsNullOrEmpty(error))
                {
                    dict.Add(Convert.ToString(r["VIN"]), error);
                    if (!dt.TableName.Equals("基本信息"))
                        vinSuccessed.Add(Convert.ToString(r["VIN"]));
                }
            }
            return dict;
        }

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="dt"></param>
        private string InsertData(DataTable dtJBXX, DataTable dtRLLX, ref List<string> vinSuccessed)
        {
            string error = string.Empty;
            int number = 0;
            string query = string.Empty;
            if (dtRLLX.TableName.Equals("传统能源"))
            {
                query = "and (RLLX='汽油' Or RLLX='柴油' Or RLLX='两用燃料' Or RLLX='双燃料')";
            }
            else
            {
                query = String.Format("and RLLX='{0}'", dtRLLX.TableName);
            }
            if (dtRLLX != null && dtRLLX.Rows.Count > 0)
            {
                using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                {
                    con.Open();
                    using (OleDbTransaction tra = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataRow r in dtRLLX.Rows)
                            {
                                #region 基本信息数据源校验
                                DataRow[] drJBXX = dtJBXX.Select(String.Format("VIN='{0}'{1}", r["VIN"], query));
                                if (drJBXX.Length == 0)
                                {
                                    error += String.Format("{0}：该条数据无对应基本信息{1}", r["VIN"], Environment.NewLine);
                                    continue;
                                }
                                #endregion
                                #region 插入原本数据状态校验
                                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("select * from FC_CLJBXX where vin='{0}'", r["VIN"]), null);
                                if (ds != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    if (ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("0") || ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("2"))
                                    {
                                        error += String.Format("{0}：该条数据已经上报{1}", r["VIN"], Environment.NewLine);
                                        continue;
                                    }
                                    else
                                    {
                                        AccessHelper.ExecuteNonQuery(tra, String.Format("delete from FC_CLJBXX where vin='{0}'", r["VIN"]), null);
                                        AccessHelper.ExecuteNonQuery(tra, String.Format("delete from RLLX_PARAM_ENTITY where vin='{0}'", r["VIN"]), null);
                                    }
                                }
                                #endregion
                                #region 插入基础数据表
                                StringBuilder strSqlFC_CLJBXX = new StringBuilder();
                                strSqlFC_CLJBXX.Append("insert into FC_CLJBXX(");
                                strSqlFC_CLJBXX.Append("VIN,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,V_ID,HGSPBM,QTXX)");
                                strSqlFC_CLJBXX.Append(" values (");
                                strSqlFC_CLJBXX.Append("@VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@CLZZRQ,@UPLOADDEADLINE,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@CREATETIME,@UPDATETIME,@STATUS,@JYJGMC,@JYBGBH,@V_ID,@HGSPBM,QTXX)");
                                OleDbParameter[] parametersFC_CLJBXX = {
				                new OleDbParameter("@VIN", OleDbType.VarChar,17),
				                new OleDbParameter("@USER_ID", OleDbType.VarChar,32),
				                new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
				                new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
				                new OleDbParameter("@CLXH", OleDbType.VarChar,100),
				                new OleDbParameter("@CLZL", OleDbType.VarChar,200),
				                new OleDbParameter("@RLLX", OleDbType.VarChar,200),
				                new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
				                new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
				                new OleDbParameter("@LTGG", OleDbType.VarChar,200),
				                new OleDbParameter("@ZJ", OleDbType.VarChar,255),
				                new OleDbParameter("@CLZZRQ", OleDbType.Date),
				                new OleDbParameter("@UPLOADDEADLINE", OleDbType.Date),
				                new OleDbParameter("@TYMC", OleDbType.VarChar,200),
				                new OleDbParameter("@YYC", OleDbType.VarChar,200),
				                new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
				                new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
				                new OleDbParameter("@EDZK", OleDbType.VarChar,255),
				                new OleDbParameter("@LJ", OleDbType.VarChar,255),
				                new OleDbParameter("@QDXS", OleDbType.VarChar,200),
				                new OleDbParameter("@CREATETIME", OleDbType.Date),
				                new OleDbParameter("@UPDATETIME", OleDbType.Date),
				                new OleDbParameter("@STATUS", OleDbType.VarChar,1),
				                new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
				                new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
				                new OleDbParameter("@V_ID", OleDbType.VarChar,255),
                                new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                new OleDbParameter("@QTXX", OleDbType.VarChar,255 )};
                                parametersFC_CLJBXX[0].Value = drJBXX[0]["VIN"];
                                parametersFC_CLJBXX[1].Value = drJBXX[0]["USER_ID"];
                                parametersFC_CLJBXX[2].Value = drJBXX[0]["QCSCQY"];
                                parametersFC_CLJBXX[3].Value = drJBXX[0]["JKQCZJXS"];
                                parametersFC_CLJBXX[4].Value = drJBXX[0]["CLXH"];
                                parametersFC_CLJBXX[5].Value = drJBXX[0]["CLZL"];
                                parametersFC_CLJBXX[6].Value = drJBXX[0]["RLLX"];
                                parametersFC_CLJBXX[7].Value = drJBXX[0]["ZCZBZL"];
                                parametersFC_CLJBXX[8].Value = drJBXX[0]["ZGCS"];
                                parametersFC_CLJBXX[9].Value = drJBXX[0]["LTGG"];
                                parametersFC_CLJBXX[10].Value = drJBXX[0]["ZJ"];
                                DateTime clzzrq = DateTime.Parse(drJBXX[0]["CLZZRQ"].ToString());
                                parametersFC_CLJBXX[11].Value = clzzrq;
                                DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrq);
                                parametersFC_CLJBXX[12].Value = uploadDeadlineDate;
                                parametersFC_CLJBXX[13].Value = drJBXX[0]["TYMC"];
                                parametersFC_CLJBXX[14].Value = drJBXX[0]["YYC"];
                                parametersFC_CLJBXX[15].Value = drJBXX[0]["ZWPS"];
                                parametersFC_CLJBXX[16].Value = drJBXX[0]["ZDSJZZL"];
                                parametersFC_CLJBXX[17].Value = drJBXX[0]["EDZK"];
                                parametersFC_CLJBXX[18].Value = drJBXX[0]["LJ"];
                                parametersFC_CLJBXX[19].Value = drJBXX[0]["QDXS"];
                                parametersFC_CLJBXX[20].Value = DateTime.Now;
                                parametersFC_CLJBXX[21].Value = DateTime.Now;
                                parametersFC_CLJBXX[22].Value = 1;
                                parametersFC_CLJBXX[23].Value = drJBXX[0]["JYJGMC"];
                                parametersFC_CLJBXX[24].Value = drJBXX[0]["JYBGBH"];
                                parametersFC_CLJBXX[25].Value = string.Empty;
                                parametersFC_CLJBXX[26].Value = drJBXX[0]["HGSPBM"];
                                parametersFC_CLJBXX[27].Value = drJBXX[0]["QTXX"];
                                AccessHelper.ExecuteNonQuery(tra, strSqlFC_CLJBXX.ToString(), parametersFC_CLJBXX);
                                #endregion
                                #region 插入燃料类别表
                                DataRow[] dr = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", dtRLLX.TableName));
                                DataTable dtRLLX_PARAM_ENTITY = new DataTable();
                                dtRLLX_PARAM_ENTITY.Columns.Add("PARAM_CODE", System.Type.GetType("System.String"));
                                dtRLLX_PARAM_ENTITY.Columns.Add("VIN", System.Type.GetType("System.String"));
                                dtRLLX_PARAM_ENTITY.Columns.Add("PARAM_VALUE", System.Type.GetType("System.String"));
                                dtRLLX_PARAM_ENTITY.Columns.Add("V_ID", System.Type.GetType("System.String"));
                                foreach (DataRow er in dr)
                                {
                                    string paramCode = er["PARAM_CODE"].ToString().Trim();
                                    DataRow ddr = dtRLLX_PARAM_ENTITY.NewRow();
                                    ddr["PARAM_CODE"] = paramCode;
                                    ddr["VIN"] = r["VIN"];
                                    ddr["PARAM_VALUE"] = r[paramCode];
                                    ddr["V_ID"] = "";
                                    dtRLLX_PARAM_ENTITY.Rows.Add(ddr);
                                }

                                OleDbDataAdapter sda = new OleDbDataAdapter("select * from RLLX_PARAM_ENTITY ", con);
                                sda.SelectCommand.Transaction = tra;
                                OleDbCommandBuilder comb = new OleDbCommandBuilder(sda);
                                sda.InsertCommand = comb.GetInsertCommand();
                                sda.Update(dtRLLX_PARAM_ENTITY);
                                #endregion
                                vinSuccessed.Add(r["VIN"].ToString());
                                number++;
                            }
                            tra.Commit();
                            error += string.Format("成功插入{0}条，失败{1}条", number, dtRLLX.Rows.Count - number) + Environment.NewLine;
                        }
                        catch (Exception ex)
                        {
                            tra.Rollback();
                            error = ex.Message;
                            number = 0;
                        }
                    }
                }
            }
            return error;
        }

        public DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构
            foreach (DataRow row in rows)
                tmp.Rows.Add(row);  // 将DataRow添加到DataTable中
            return tmp;
        }

        private string Message(string str)
        {
            return DateTime.Now + ":" + str + "\r\n";
        }

        /// <summary>
        /// 从路径中获取文件名 然后和新路径组合
        /// </summary>
        /// <param name="path">新路径</param>
        /// <param name="pathFile">带文件名路径</param>
        /// <returns></returns>
        private string GetPathFileName(string path, string pathFile)
        {
            string[] str = pathFile.Split('\\');
            return path + "\\" + str[str.Length - 1];

        }

        /// <summary>
        /// 验证单行数据
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        private string VerifyData(DataRow r, DataRow[] dr, string Rllx)
        {
            string message = string.Empty;

            switch (Rllx)
            {
                case "传统能源":
                    message += this.VerifyCTNY(r, dr);
                    break;
                case "纯电动":
                    message += this.VerifyCDD(r, dr);
                    break;
                case "非插电式混合动力":
                    message += this.VerifyHHDL(r, dr);
                    break;
                case "插电式混合动力":
                    message += this.VerifyHHDL(r, dr);
                    break;
                case "燃料电池":
                    message += this.VerifyRLDC(r, dr);
                    break;
                default:
                    message += this.VerifyJBXX(r);
                    break;
            }
            return message;
        }

        /// <summary>
        /// 获取全部参数数据
        /// </summary>
        /// <returns></returns>
        private DataTable GetCheckData()
        {
            string sql = "select * from RLLX_PARAM";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取路径文件名前两位当燃料类型
        /// </summary>
        /// <param name="pathFile">路径文件名</param>
        /// <returns></returns>
        private string GetTableName(string pathFile)
        {
            string[] file = pathFile.Split('\\');
            string tableName = file[file.Length - 1].Substring(0, 2);
            return tableName;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="fileName">文件地址</param>
        /// <param name="sheet">名称</param>
        /// <returns></returns>
        public DataSet ReadExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
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
                    throw new Exception(ex.Message);
                }
                finally
                {
                    objConn.Close();
                }
                return ds;
            }
        }

        /// <summary>
        /// 获取路径folderPath下所有sheet符合的文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="fileMark">文件名字的开头字符</param>
        /// <returns></returns>
        public IList<string> GetFileName(string folderPath, string fileMark, Dictionary<string, Dictionary<string, string>> fileList)
        {
            List<string> fileNameList = new List<string>();
            try
            {
                StringBuilder msgBuilder = new StringBuilder();
                DirectoryInfo folder = new DirectoryInfo(folderPath);
                foreach (FileInfo file in folder.GetFiles(fileMark))
                {
                    int flag = 0;
                    DataSet ds = new DataSet();
                    try
                    {
                        ds = ReadExcel(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToString() == "Microsoft Jet 数据库引擎找不到对象'vehicleBasicInfo$_'。请确定对象是否存在，并正确地写出它的名称和路径。")
                        {
                            fileList.Add(file.FullName + "\r\n==========导入失败==========\r\n", new Dictionary<string, string>() { { ex.Message.ToString(), "" } });
                        }
                        continue;
                    }
                    if (ds.Tables.Count == 6)
                    {
                        for (int i = 0; i < ds.Tables.Count; i++)
                        {
                            if (ds.Tables[i].TableName.Equals("vehicleBasicInfo") || ds.Tables[i].TableName.Equals("CT") || ds.Tables[i].TableName.Equals("CDS_HHDL") || ds.Tables[i].TableName.Equals("FCDS_HHDL") || ds.Tables[i].TableName.Equals("CDD") || ds.Tables[i].TableName.Equals("RLDC"))
                            {
                                flag++;
                            }
                        }
                        if (flag == 6)
                        {
                            fileNameList.Add(file.FullName);
                        }
                        else
                        {
                            fileList.Add(file.FullName + "\r\n==========导入失败==========\r\n", new Dictionary<string, string>() { { "该文件中sheet页命名错误,已跳过，请查看", "" } });
                        }
                    }
                    else
                    {
                        fileList.Add(file.FullName + "\r\n==========导入失败==========\r\n", new Dictionary<string, string>() { { "该文件中sheet页命名错误,已跳过，请查看", "" } });
                    }
                }
            }
            catch (Exception ex) { }

            return fileNameList;
        }

        /// <summary>
        /// 转移文件
        /// </summary>
        /// <param name="srcFileName">源文件路径</param>
        /// <param name="folderPath">目的文件夹路径</param>
        public string MoveFile(string srcFileName, string folderPath)
        {
            string msg = string.Empty;
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string shortFileName = Path.GetFileNameWithoutExtension(srcFileName) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(srcFileName);
                string desFileName = Path.Combine(folderPath, shortFileName);

                File.Move(srcFileName, desFileName);
            }
            catch (Exception ex)
            {
                msg = ex.Message + Environment.NewLine;
            }
            return msg;
        }

        #region 参数验证
        // 验证基本信息
        protected string VerifyJBXX(DataRow r)
        {
            string message = string.Empty;

            // VIN
            string vin = Convert.ToString(r["VIN"]);

            message += this.VerifyRequired("VIN", vin);
            message += this.VerifyStrLen("VIN", vin, 17);
            if (vin.StartsWith("L"))
            {
                message += this.VerifyVin(vin, false);
            }
            if (!vin.StartsWith("L"))
            {
                message += this.VerifyVin(vin, true);
            }

            string Jkqczjxs = Convert.ToString(r["JKQCZJXS"]);
            string Qcscqy = Convert.ToString(r["QCSCQY"]);

            // 汽车生产企业
            if (string.IsNullOrEmpty(Qcscqy))
            {
                message += "\n汽车生产企业不能为空!";
            }

            //海关编码
            message += this.VerifyRequired("海关编码", Convert.ToString(r["HGSPBM"]));

            // 车辆型号
            string clxh = Convert.ToString(r["CLXH"]);
            message += this.VerifyRequired("车辆型号", clxh);
            message += this.VerifyStrLen("车辆型号", clxh, 100);

            // 车辆种类
            string Clzl = Convert.ToString(r["CLZL"]);
            message += this.VerifyRequired("车辆种类", Clzl);
            Clzl = Clzl.Replace("(", "（").Replace(")", "）");
            if (Clzl == "乘用车（M1类）")
            {
                Clzl = "乘用车（M1）";
            }
            message += this.VerifyClzl(Clzl);
            message += this.VerifyStrLen("车辆种类", Clzl, 200);

            // 燃料类型
            string Rllx = Convert.ToString(r["RLLX"]);
            message += this.VerifyRequired("燃料类型", Rllx);
            message += this.VerifyStrLen("燃料类型", Rllx, 200);
            message += this.VerifyRllx(Rllx);

            // 整车整备质量
            string Zczbzl = Convert.ToString(r["ZCZBZL"]);
            message += this.VerifyRequired("整车整备质量", Zczbzl);
            if (!this.VerifyParamFormat(Zczbzl, ','))
            {
                message += "\n整车整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格";
            }

            // 最高车速
            string Zgcs = Convert.ToString(r["ZGCS"]);
            message += this.VerifyRequired("最高车速", Zgcs);
            if (!this.VerifyParamFormat(Zgcs, ','))
            {
                message += "\n最高车速应填写整数，多个数值应以半角“,”隔开，中间不留空格";
            }

            // 轮胎规格
            string Ltgg = Convert.ToString(r["LTGG"]);
            message += this.VerifyRequired("轮胎规格", Ltgg);
            message += this.VerifyStrLen("轮胎规格", Ltgg, 200);
            message += this.VerifyLtgg(Ltgg);
            // 前后轮距相同只填写一个型号数据即可，不同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)

            // 轴距
            string Zj = Convert.ToString(r["ZJ"]);
            message += this.VerifyRequired("轴距", Zj);
            message += this.VerifyInt("轴距", Zj);

            // 通用名称
            string Tymc = Convert.ToString(r["Tymc"]);
            message += this.VerifyRequired("通用名称", Tymc);
            message += this.VerifyStrLen("通用名称", Tymc, 200);

            // 越野车（G类）
            string Yyc = Convert.ToString(r["YYC"]);
            message += this.VerifyRequired("越野车（G类）", Yyc);
            message += this.VerifyYyc(Yyc);
            message += this.VerifyStrLen("越野车（G类）", Yyc, 200);

            // 座位排数
            string Zwps = Convert.ToString(r["ZWPS"]);
            message += this.VerifyRequired("座位排数", Zwps);
            message += this.VerifyInt("座位排数", Zwps);

            // 最大设计总质量
            string Zdsjzzl = Convert.ToString(r["ZDSJZZL"]);
            string Edzk = Convert.ToString(r["EDZK"]);
            message += this.VerifyRequired("最大设计总质量", Zdsjzzl);
            message += this.VerifyZdsjzzl(Zdsjzzl, Zczbzl, Edzk);
            message += this.VerifyInt("最大设计总质量", Zdsjzzl);

            // 额定载客
            message += this.VerifyRequired("额定载客", Edzk);
            message += this.VerifyInt("额定载客", Edzk);

            // 轮距（前/后）
            string Lj = Convert.ToString(r["LJ"]);
            message += this.VerifyRequired("轮距（前/后）", Lj);
            if (!this.VerifyParamFormat(Lj, '/') && Lj.IndexOf('/') < 0)
            {
                message += "\n轮距（前/后）应填写整数，前后轮距，中间用”/”隔开";
            }

            // 驱动型式 
            string Qdxs = Convert.ToString(r["QDXS"]);
            message += this.VerifyRequired("驱动型式", Qdxs);
            message += this.VerifyQdxs(Qdxs);
            message += this.VerifyStrLen("驱动型式", Qdxs, 200);

            // 检测机构名称
            string Jyjgmc = Convert.ToString(r["JYJGMC"]);
            message += this.VerifyRequired("检测机构名称", Jyjgmc);
            message += this.VerifyStrLen("检测机构名称", Jyjgmc, 500);

            // 报告编号
            string Jybgbh = Convert.ToString(r["JYBGBH"]);
            message += this.VerifyRequired("报告编号", Jybgbh);
            message += this.VerifyStrLen("报告编号", Jybgbh, 500);
            return message;
        }
        // 验证VIN
        protected string VerifyVin(string vin, bool isImport)
        {
            string message = string.Empty;
            DataCheck dc = new DataCheck();
            char bi;
            try
            {
                if (!isImport)
                {
                    if (!dc.CheckCLSBDH(vin, out bi))
                    {
                        if (bi == '-')
                        {
                            message += "\n请核对【备案号(VIN)】为17位字母或者数字!";
                        }
                        else
                        {
                            message += "\n【备案号(VIN)】校验失败！第9位应为:'" + bi + "'";
                        }
                    }
                }
                else
                {
                    // TODO 进口车验证
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return message;
        }

        // 验证燃料类型
        protected string VerifyRllx(string rllx)
        {
            if (!string.IsNullOrEmpty(rllx))
            {
                if (rllx == "汽油" || rllx == "柴油" || rllx == "两用燃料" || rllx == "双燃料" || rllx == "非插电式混合动力" || rllx == "插电式混合动力" || rllx == "纯电动" || rllx == "燃料电池")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n燃料类型参数填写汽油、柴油、两用燃料、双燃料、纯电动、非插电式混合动力、插电式混合动力、燃料电池";
                }
            }
            return string.Empty;
        }

        protected string VerifyLtgg(string ltgg)
        {
            string message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ltgg))
                {
                    int indexLtgg = ltgg.IndexOf(")/(");
                    if (indexLtgg > -1)
                    {
                        string ltggHead = ltgg.Substring(0, indexLtgg + 1);
                        string ltggEnd = ltgg.Substring(indexLtgg + 3);

                        if (!ltggHead.StartsWith("(") || !ltggEnd.EndsWith(")"))
                        {
                            message = "前后轮距不相同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        // 验证最大设计总质量
        protected string VerifyZdsjzzl(string zdsjzzl, string zczbzl, string edzk)
        {
            if (!string.IsNullOrEmpty(zdsjzzl) && !string.IsNullOrEmpty(zczbzl) && !string.IsNullOrEmpty(edzk))
            {
                if (Convert.ToInt32(zdsjzzl) < (Convert.ToInt32(zczbzl) + Convert.ToInt32(edzk) * 65))
                {
                    return "\n最大设计总质量应≥整车整备质量＋乘员质量（额定载客×乘客质量，乘用车按65㎏/人核算)!";
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        // 验证燃料类型
        // 车辆种类
        protected string VerifyClzl(string clzl)
        {
            if (!string.IsNullOrEmpty(clzl))
            {
                if (clzl == "乘用车（M1）" || clzl == "轻型客车（M2）" || clzl == "轻型货车（N1）")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n车辆种类参数应填写“乘用车（M1）/轻型客车（M2）/轻型货车（N1）”";
                }
            }
            return string.Empty;
        }

        // 越野车
        protected string VerifyYyc(string yyc)
        {
            if (!string.IsNullOrEmpty(yyc))
            {
                if (yyc == "是" || yyc == "否")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n越野车(G类)参数应填写“是/否”";
                }
            }
            return string.Empty;
        }

        // 驱动型式
        protected string VerifyQdxs(string qdxs)
        {
            if (!string.IsNullOrEmpty(qdxs))
            {
                if (qdxs == "前轮驱动" || qdxs == "后轮驱动" || qdxs == "分时全轮驱动" || qdxs == "全时全轮驱动" || qdxs == "智能(适时)全轮驱动")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n驱动型式参数应填写“前轮驱动/后轮驱动/分时全轮驱动/全时全轮驱动/智能(适时)全轮驱动”";
                }
            }
            return string.Empty;
        }

        // 变速器型式
        protected string VerifyBsqxs(string bsqxs)
        {
            if (!string.IsNullOrEmpty(bsqxs))
            {
                if (bsqxs == "MT" || bsqxs == "AT" || bsqxs == "AMT" || bsqxs == "CVT" || bsqxs == "DCT" || bsqxs == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n变速器型式参数应填写“MT/AT/AMT/CVT/DCT/其它”";
                }
            }
            return string.Empty;
        }

        // 变速器档位数
        protected string VerifyBsqdws(string bsqdws)
        {
            if (!string.IsNullOrEmpty(bsqdws))
            {
                if (bsqdws == "1" || bsqdws == "2" || bsqdws == "3" || bsqdws == "4" || bsqdws == "5" || bsqdws == "6" || bsqdws == "7" || bsqdws == "8" || bsqdws == "9" || bsqdws == "10" || bsqdws == "N.A")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n变速器档位数参数应填写“1/2/3/4/5/6/7/8/9/10/N.A”";
                }
            }
            return string.Empty;
        }

        // 混合动力结构型式
        protected string VerifyHhdljgxs(string hhdljgxs)
        {
            if (!string.IsNullOrEmpty(hhdljgxs))
            {
                if (hhdljgxs == "串联" || hhdljgxs == "并联" || hhdljgxs == "混联" || hhdljgxs == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n混合动力结构型式参数应填写“串联/并联/混联/其它”";
                }
            }
            return string.Empty;
        }

        // 是否具有行驶模式手动选择功能
        protected string VerifySdxzgn(string sdxzgn)
        {
            if (!string.IsNullOrEmpty(sdxzgn))
            {
                if (sdxzgn == "是" || sdxzgn == "否")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n是否具有行驶模式手动选择功能参数应填写“是/否”";
                }
            }
            return string.Empty;
        }

        // 动力蓄电池组种类
        protected string VerifyDlxdczzl(string dlxdczzl)
        {
            if (!string.IsNullOrEmpty(dlxdczzl))
            {
                if (dlxdczzl == "铅酸电池" || dlxdczzl == "金属氢化物镍电池" || dlxdczzl == "锂电池" || dlxdczzl == "超级电容" || dlxdczzl == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n动力蓄电池组种类参数应填写“铅酸电池/金属氢化物镍电池/锂电池/超级电容/其它”";
                }
            }
            return string.Empty;
        }

        #endregion

        #region 燃料类型验证
        // 验证传统能源参数
        protected string VerifyCTNY(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;

            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "CT_PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CT_EDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_JGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_SJGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_SQGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKCO2PFL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_BSQXS":
                            message += VerifyBsqxs(Convert.ToString(r[code]));
                            break;
                        case "CT_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "CT_JGL" && code != "CT_QTXX")
                    {
                        message += this.VerifyRequired(name, Convert.ToString(r[code]));
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        // 验证纯电动参数
        protected string VerifyCDD(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "CDD_DLXDCBNL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZEDNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DDXDCZZLYZCZBZLDBZ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZBCDY":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DDQC30FZZGCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKXSLC":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKDNXHL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    message += this.VerifyRequired(name, Convert.ToString(r[code]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        // 验证混合动力参数
        protected string VerifyHHDL(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "FCDS_HHDL_DLXDCBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_EDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_JGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SJGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SQGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_BSQXS":
                            message += VerifyBsqxs(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLJGXS":
                            message += VerifyHhdljgxs(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_XSMSSDXZGN":
                            message += VerifySdxzgn(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;

                        case "CDS_HHDL_DLXDCBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKDNXHL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_EDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_JGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_BSQXS":
                            message += VerifyBsqxs(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_HHDLJGXS":
                            message += VerifyHhdljgxs(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_XSMSSDXZGN":
                            message += VerifySdxzgn(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "FCDS_HHDL_CDDMSXZGCS" && code != "FCDS_HHDL_CDDMSXZHGKXSLC" && code != "FCDS_HHDL_JGL" && code != "CDS_HHDL_JGL")
                    {
                        message += this.VerifyRequired(name, Convert.ToString(r[code]));
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        // 验证燃料电池参数
        protected string VerifyRLDC(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "RLDC_DDGLMD":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_DDHHJSTJXXDCZBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKHQL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_CDDMSXZGXSCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPBCGZYL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPRJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "RLDC_ZHGKHQL" && code != "RLDC_ZHGKXSLC" && code != "RLDC_CDDMSXZGXSCS")
                    {
                        message += this.VerifyRequired(name, Convert.ToString(r[code]));
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        #endregion

        #region 输入验证

        // 验证不为空
        protected string VerifyRequired(string strName, string value)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                msg = "\n" + strName + "不能为空!";
            }
            return msg;
        }

        // 验证字符长度
        protected string VerifyStrLen(string strName, string value, int expectedLen)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > expectedLen)
                {
                    msg = "\n" + strName + "长度过长，最长为" + expectedLen + "位!";
                }
            }
            return msg;
        }

        // 验证整型
        protected string VerifyInt(string strName, string value)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value.ToString(), "^[0-9]*$"))
            {
                msg = "\n" + strName + "应为整数!";
            }
            return msg;
        }

        // 验证浮点型1位小数
        protected string VerifyFloat(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
            {
                msg = "\n" + strName + "应保留1位小数!";
            }
            return msg;
        }

        // 验证浮点型两位小数
        protected string VerifyFloat2(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{2}$"))
            {
                msg = "\n" + strName + "应保留2位小数!";
            }
            return msg;
        }

        // 验证时间类型
        protected string VerifyDateTime(string strName, DateTime value)
        {
            string msg = string.Empty;
            try
            {
                if (value != null)
                {
                    DateTime time = Convert.ToDateTime(value.ToString());
                }
            }
            catch (Exception)
            {
                msg = "\n" + strName + "应为时间类型!";
            }
            return msg;
        }


        // 验证时间类型
        protected string VerifyDateTime(string strName, string value)
        {
            string msg = string.Empty;
            try
            {
                if (value != null)
                {
                    DateTime time = Convert.ToDateTime(value);
                }
            }
            catch (Exception)
            {
                msg = "\n" + strName + "应为时间类型!";
            }
            return msg;
        }

        /// <summary>
        /// 参数格式验证，多个数值以参数c隔开，中间不能有空格
        /// </summary>
        /// <param name="value">参数值</param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool VerifyParamFormat(string value, char c)
        {
            if (!string.IsNullOrEmpty(c.ToString()))
            {
                string[] valueArr = value.Split(c);
                if (valueArr[0] == "" || valueArr[valueArr.Length - 1] == "")
                {
                    return false;
                }
                foreach (string val in valueArr)
                {
                    if (!Regex.IsMatch(val, @"^[+]?\d*$"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion


        // 查询数据上报的截止日期
        public DateTime QueryUploadDeadLine(DateTime manufactureDate)
        {
            DateTime deadLine = new DateTime();

            try
            {
                int timeCons = Utils.timeCons;
                string strManufactureDate = manufactureDate.ToString("yyyy-MM-dd");
                if (!string.IsNullOrEmpty(strManufactureDate) && timeCons > 0)
                {
                    // 限制时长的开始计时日期为（制造日+1天）的零点
                    DateTime startDate = Convert.ToDateTime(strManufactureDate).AddDays(1);

                    // 临时截止日期为 开始计时时间+限制时长
                    deadLine = startDate.Add(new TimeSpan(timeCons, 0, 0));

                    // 查看 开始计时时间和临时截止日期 之间有无节假日
                    for (DateTime dt = startDate; dt < deadLine; dt = dt.AddDays(1))
                    {
                        if (this.VerifyHolidays(dt.ToString("yyyy-MM-dd")))
                        {
                            deadLine = deadLine.AddDays(1);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return deadLine;
        }

        // 验证节假日
        protected bool VerifyHolidays(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }

            try
            {
                if (listHoliday != null && listHoliday.Contains(date))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        protected List<string> GetHoliday()
        {
            List<string> holidayList = new List<string>();
            try
            {
                string sqlHol = string.Format(@"SELECT HOL_DAYS FROM FC_HOLIDAY");
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlHol, null);
                //dbUtil.QuerySingleDT(sqlHol);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        holidayList.Add(dr["HOL_DAYS"].ToString());
                    }
                }
            }
            catch (Exception)
            {
            }
            return holidayList;
        }

        /*--------------------------------功能分隔符------------------------------------*/

        /// <summary>
        /// 根据导入EXCEL 查询 本地库数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet ReadSharchExcel(string path, string status)
        {
            DataSet ds = new DataSet();
            try
            {
                ds = ReadExcel(path);
        
                StringBuilder strAdd = new StringBuilder();
                strAdd.Append("select * from FC_CLJBXX where VIN in(");
                if (ds != null && ds.Tables[0].Rows.Count > 0 )
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        strAdd.Append("'");
                        strAdd.Append(Convert.ToString(r["VIN"]));
                        strAdd.Append("',");
                    }
                    string sql = strAdd.ToString().TrimEnd(',') + ")";
                    return AccessHelper.ExecuteDataSet(AccessHelper.conn, String.Format("{0} and STATUS='{1}'", sql, status), null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        /// <summary>
        /// 查询EXCEL中VIN状态
        /// </summary>
        /// <param name="vin">VIN码</param>
        /// <returns></returns>
        private int SearchStatus(string vin)
        {
            string sql = String.Format("select status from FC_CLJBXX where VIN='{0}'", vin);
            return Convert.ToInt32(AccessHelper.ExecuteScalar(AccessHelper.conn, sql, null));
        }

        /// <summary>
        /// 批量修改进口日期
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public int ReadUpdateDate(string path)
        {
            int result = 0;
            try
            {
                DataSet ds = ReadExcel(path);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                    {
                        con.Open();
                        using (OleDbTransaction tra = con.BeginTransaction())
                        {
                            try
                            {
                                foreach (DataRow r in ds.Tables[0].Rows)
                                {
                                    string statuswhere = string.Empty;
                                    int status = SearchStatus(Convert.ToString(r["VIN"]));
                                    bool rel = false;
                                    switch (status)
                                    {
                                        case (int)Status.待上报:
                                            rel = true;
                                            break;
                                        case (int)Status.已上报:
                                            statuswhere = ", STATUS=" + (int)Status.修改待上报;
                                            rel = true;
                                            break;
                                        case (int)Status.修改待上报:
                                            rel = true;
                                            break;
                                    }

                                    if (rel)
                                    {
                                        DateTime clzzrqDate = Convert.ToDateTime(r[1].ToString());
                                        DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);
                                        string sql = "UPDATE FC_CLJBXX SET CLZZRQ='" + clzzrqDate + "', UPLOADDEADLINE='" + uploadDeadlineDate + "'" + statuswhere + "  where VIN='" + r["VIN"] + "'";
                                        AccessHelper.ExecuteNonQuery(tra, sql, null);
                                    }
                                }
                                tra.Commit();
                                result = 1;
                            }
                            catch
                            {
                                tra.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }

}
