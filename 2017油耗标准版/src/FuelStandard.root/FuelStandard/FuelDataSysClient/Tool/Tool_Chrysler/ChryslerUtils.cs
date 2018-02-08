using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using FuelDataSysClient.Properties;

namespace FuelDataSysClient.Tool.Tool_Chrysler
{
    public enum Status
    {
        已上报 = 0,
        待上报 = 1,
        修改待上报 = 2,
        撤销待上报 = 3,
        //未被激活 = 9
        //（9：未被激活（数据通过excel导入但未被激活）；0：已上传；1：没上传；2：修改没上传；3：撤销未上传）
    }

    public class ChryslerUtils
    {
        private const string CTNY = "传统能源";
        private const string FCDSHHDL = "非插电式混合动力";
        private const string VIN = "VIN";
        private const string FUEL = "FUEL";
        private string strCon = AccessHelper.conn;

        // 保留一位小数的字段
        private List<string> PARAMFLOAT1 = new List<string>() 
        { 
            "CT_EDGL", "CT_JGL", "CT_SJGKRLXHL", "CT_SQGKRLXHL", "CT_ZHGKRLXHL", "FCDS_HHDL_DLXDCZZNL",
            "FCDS_HHDL_ZHGKRLXHL", "FCDS_HHDL_EDGL", "FCDS_HHDL_JGL", "FCDS_HHDL_SJGKRLXHL", "FCDS_HHDL_SQGKRLXHL",
            "FCDS_HHDL_QDDJEDGL", "CDS_HHDL_DLXDCZZNL", "FCDS_HHDL_DLXDCBNL", "CDS_HHDL_ZHGKRLXHL", 
            "CDS_HHDL_QDDJEDGL", "CDS_HHDL_EDGL", "CDS_HHDL_JGL", "CDD_DLXDCZEDNL", "CDD_QDDJEDGL", "RLDC_DDGLMD",
            "RLDC_ZHGKHQL", "RLDC_QDDJEDGL" 
        };
        // 保留两位小数的字段
        private List<string> PARAMFLOAT2 = new List<string>() { "CDS_HHDL_HHDLZDDGLB", "FCDS_HHDL_HHDLZDDGLB" };

        DataTable checkData = new DataTable();
        Dictionary<string, string> dictCTNY;  //存放列头转换模板(传统能源)
        Dictionary<string, string> dictFCDSHHDL;  //存放列头转换模板（非插电式混合动力）
        Dictionary<string, string> dictFUEL; // 存放列头转换模板（轮胎信息表）
        Dictionary<string, string> dictVin; //存放列头转换模板（VIN）

        // 预先读取一下内容，提高性能
        DataTable dtFuelStatic; // CPOS信息
        DataTable dtCtnyStatic; // 传统能源数据
        //DataTable dtFcdsStatic;

        private List<string> listHoliday; // 节假日数据

        // 读取配置文件信息
        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate_Chrysler"];
        private static Dictionary<string, string> FILE_NAME = new Dictionary<string, string>() 
        {
            {"VIN"  ,"VIN*.xls"},
            {"FUEL"  ,"油耗*.xlsx"},
            {"MAIN"  ,"车型参数*.xlsx"},
            {"F_VIN"  ,"已导入的VIN"},
            {"F_FUEL"  ,"已导入的油耗数据"},
            {"F_MAIN"  ,"已导入的车型参数"},
            {"JKQCZJXS" ,"克莱斯勒（中国）汽车销售有限公司"},
            {"CPOS_FUEL"  ,"CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKCO2PFL,CT_ZHGKRLXHL"}
        };

        public ChryslerUtils()
        {
            checkData = GetCheckData();    //获取参数数据  RLLX_PARAM  

            ReadTemplate(path);   //读取表头转置模板
        }

        // 克莱斯勒企业备案名称
        private string JKQCZJXS = FILE_NAME["JKQCZJXS"].ToString();

        // VIN excel文件名称的开头
        private string vinFileName = FILE_NAME["VIN"].ToString();
        public string VinFileName
        {
            get { return vinFileName; }
        }

        // 车型参数Excel文件名称的开头
        private string mainFileName = FILE_NAME["MAIN"].ToString();
        public string MainFileName
        {
            get { return mainFileName; }
        }

        // CPOS Excel文件名称的开头
        private string cposFileName = FILE_NAME["FUEL"].ToString();
        public string CposFileName
        {
            get { return cposFileName; }
            set { cposFileName = value; }
        }


        // CPOS Excel文件名称的开头
        private string paramName = FILE_NAME["CPOS_FUEL"].ToString();
        public string ParamName
        {
            get { return paramName; }
            set { paramName = value; }
        }

        /// <summary>
        /// 获取路径folderPath下所有以fileMark开头的文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="fileMark">文件名字的开头字符</param>
        /// <returns></returns>
        public List<string> GetFileName(string folderPath, string fileMark)
        {
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            List<string> fileNameList = new List<string>();
            foreach (FileInfo file in folder.GetFiles(fileMark))
            {
                fileNameList.Add(file.FullName);
            }
            return fileNameList;
        }

        /// <summary>
        /// 导入VIN信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public string ImportVinData(string fileName, string folderName)
        {
            string rtnMsg = string.Empty;
            DataSet ds = this.ReadVinExcel(fileName);

            if (ds != null)
            {
                // 表头根据模板转换
                DataTable dtVin = D2D(ds.Tables[VIN]);

                rtnMsg += this.SaveVinInfo(dtVin);

                // 如果保存VIN时没有异常信息，说明VIN都已经保存成功，将该VIN文件移动到已完成目录
                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    this.MoveFinishedFile(fileName, folderName, "F_VIN");
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
        }

        /// <summary>
        /// 新导入油耗信息
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="folderName">文件所在路径名</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <param name="cposUpdateList">更新列表，记录更新的数据。存CPOS编号</param>
        /// <returns>返回错误信息，没有则返回空串</returns>
        public string ImportFuelData(string fileName, string folderName, string importType, List<string> fuelUpdateList)
        {
            string rtnMsg = string.Empty;

            // 读油耗模板数据信息
            DataSet ds = this.ReadCposFuelExcel(fileName);

            if (ds != null)
            {
                DataTable dtFuel = D2D(ds.Tables[FUEL]);  //转换表头（用户模板中的表头转为数据库列名）

                if (importType == "IMPORT")
                {
                    // 新导入
                    rtnMsg += this.SaveCposFuelData(dtFuel);
                }
                else if (importType == "UPDATE")
                {
                    // 修改
                    rtnMsg += this.UpdateCposFuelData(dtFuel, fuelUpdateList);
                }

                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    // 读取成功后，将车型参数模板文件移动到已完成的文件夹
                    //this.MoveFinishedFile(fileName, folderName, "F_FUEL");
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
        }

        /// <summary>
        /// 导入车型参数信息
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="folderName">文件所在路径名</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <param name="mainUpdateList">更新列表，记录更新的数据。存车型参数编号</param>
        /// <returns>返回错误信息，没有则返回空串</returns>
        public string ImportMainData(string fileName, string folderName, string importType, List<string> mainUpdateList)
        {
            string rtnMsg = string.Empty;

            // 读取车型参数模板数据
            DataSet ds = this.ReadMainExcel(fileName);

            if (ds != null)
            {
                if (importType == "IMPORT")
                {
                    // 新导入
                    rtnMsg += this.SaveMainData(ds);
                }
                else if (importType == "UPDATE")
                {
                    // 修改
                    rtnMsg += this.UpdateMainData(ds, mainUpdateList);
                }
                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    // 读取成功后，将车型参数模板文件移动到已完成的文件夹
                    // this.MoveFinishedFile(fileName, folderName, "F_MAIN");
                }
                else
                {
                    rtnMsg = Path.GetFileName(fileName) + rtnMsg + "\r\n";
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
        }

        /// <summary>
        /// 读取车型参数模板数据
        /// </summary>
        /// <param name="fileName">文件全名</param>
        public DataSet ReadMainExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [传统能源$]", strConn);
                oada.Fill(ds, CTNY);

                //oada = new OleDbDataAdapter("select * from [非插电式混合动力$]", strConn);
                //oada.Fill(ds, FCDSHHDL);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 读CPOS模板数据信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet ReadCposFuelExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [sheet1$]", strConn);
                oada.Fill(ds, FUEL);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 读取VIN的CSV模板数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet ReadVinCsv(bool HeadYes, char span, string fileName)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            StreamReader fileReader = new StreamReader(fileName, Encoding.Default);
            try
            {
                //是否为第一行（如果HeadYes为TRUE，则第一行为标题行）
                int lsi = 0;

                //列之间的分隔符
                char cv = span;
                while (fileReader.EndOfStream == false)
                {
                    string line = fileReader.ReadLine();
                    string[] y = line.Split(cv);

                    //第一行为标题行
                    if (HeadYes == true)
                    {
                        //第一行
                        if (lsi == 0)
                        {
                            for (int i = 0; i < y.Length; i++)
                            {
                                dt.Columns.Add(s2s(y[i].Trim().ToString()));
                            }
                            lsi++;
                        }
                        //从第二列开始为数据列
                        else
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < y.Length; i++)
                            {
                                dr[i] = y[i].Trim();
                            }
                            dt.Rows.Add(dr);
                        }
                    }

                    //第一行不为标题行
                    else
                    {
                        if (lsi == 0)
                        {
                            for (int i = 0; i < y.Length; i++)
                            {
                                dt.Columns.Add("Col" + i.ToString());
                            }
                            lsi++;
                        }
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < y.Length; i++)
                        {
                            dr[i] = y[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                fileReader.Close();
                fileReader.Dispose();
            }
            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// 读取VIN的Excel模板数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet ReadVinExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [sheet1$]", strConn);
                oada.Fill(ds, VIN);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 保存VIN信息并生成油耗数据
        /// </summary>
        /// <param name="ds"></param>
        public string SaveVinInfo(DataTable dtVin)
        {
            int succFuelCount = 0; //生成油耗数据的数量
            int succImCount = 0;   //成功导入的数量
            int failCount = 0;  //导入失败的数量
            int totalCount = dtVin.Rows.Count;
            string msg = string.Empty;
            ProcessForm pf = new ProcessForm();
            try
            {
                DataTable dtCtnyPam = this.GetRllxData("传统能源");
                //DataTable dtFcdsPam = this.GetRllxData("非插电式混合动力"); 

                // 获取节假日数据
                listHoliday = this.GetHoliday();

                // 显示进度条
                pf.Show();
                int pageSize = 20;
                int totalVin = dtVin.Rows.Count;
                int count = 0;

                pf.TotalMax = (int)Math.Ceiling((decimal)totalVin / (decimal)pageSize);
                pf.ShowProcessBar();

                foreach (DataRow drVin in dtVin.Rows)
                {
                    count++;
                    string vin = FormatBrackets(Convert.ToString(drVin["VIN"]).Trim());
                    string cpos = FormatBrackets(Convert.ToString(drVin["CPOS"]).Trim());
                    string yearMode = FormatBrackets(Convert.ToString(drVin["YEAR_MODE"]).Trim());
                    string clxh = FormatBrackets(Convert.ToString(drVin["CLXH"]).Trim());
                    string bsx = FormatBrackets(Convert.ToString(drVin["BSX"]).Trim());
                    string pl = FormatBrackets(Convert.ToString(drVin["CT_PL"]).Trim());
                    string qcscqy = FormatBrackets(Convert.ToString(drVin["QCSCQY"]).Trim());

                    string vinMsg = string.Empty;
                    vinMsg += this.VerifyVinData(drVin);

                    if (string.IsNullOrEmpty(vinMsg))
                    {
                        string ctnyMsg = string.Empty;
                        string fuelMsg = string.Empty;

                        Dictionary<string, string> paramDict = new Dictionary<string, string>();
                        paramDict.Add("CPOS", cpos);
                        paramDict.Add("YEAR_MODE", yearMode);
                        paramDict.Add("CLXH", clxh);
                        paramDict.Add("BSX", bsx);
                        paramDict.Add("CT_PL", pl);
                        paramDict.Add("QCSCQY", qcscqy);

                        DataRow drCtny = this.GetDivideMain(dtCtnyStatic, vin, paramDict, CTNY, ref ctnyMsg);
                        DataRow drCposFuel = this.GetDivideMain(dtFuelStatic, vin, paramDict, FUEL, ref fuelMsg);

                        if (!string.IsNullOrEmpty(ctnyMsg))
                        {
                            vinMsg += ctnyMsg;
                        }
                        if (!string.IsNullOrEmpty(fuelMsg))
                        {
                            vinMsg += fuelMsg;
                        }
                        if (string.IsNullOrEmpty(vinMsg))
                        {
                            if (string.IsNullOrEmpty(ctnyMsg))
                            {
                                vinMsg += this.SaveReadyData(drVin, drCtny, drCposFuel, dtCtnyPam, CTNY);
                                if (string.IsNullOrEmpty(vinMsg))
                                {
                                    succFuelCount++; //统计导入并生成油耗数据的VIN数量
                                }
                            }
                        }
                        else
                        {
                            string saveMsg = string.Empty;
                            vinMsg += saveMsg = this.SaveVinBak(drVin);
                            if (string.IsNullOrEmpty(saveMsg))
                            {
                                succImCount++;
                            }
                        }
                    }
                    msg += vinMsg;


                    if (count % 20 == 0 || count == totalCount)
                    {
                        pf.progressBarControl1.PerformStep();
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\r\n";
            }
            finally
            {
                if (pf != null)
                {
                    pf.Close();
                }
            }

            failCount = totalCount - succFuelCount - succImCount;

            if (failCount > 0)
            {
                msg += "FAILED-IMPORT";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条导入成功（其中{2}条生成油耗数据成功；{3}条生成耗数据失败） \r\n \t{4}条导入失败\r\n",
                                totalCount, succFuelCount + succImCount, succFuelCount, succImCount, failCount);
            msg = msgSummary + msg;

            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="mainId"></param>
        /// <param name="paramId"></param>
        /// <param name="paramName"></param>
        /// <param name="IsExist"></param>
        /// <returns></returns>
        public DataRow GetDivideMain(DataTable dt, string vin, Dictionary<string, string> paramDict, string paramName, ref string message)
        {
            string FilterExpression = string.Empty;
            foreach (var dict in paramDict)
            {
                FilterExpression += " and " + dict.Key + "='" + dict.Value + "'";
            }
            FilterExpression = FilterExpression.Substring(5);
            DataRow[] dr = dt.Select(FilterExpression);
            if (dr.Length < 1)
            {
                switch (paramName)
                {
                    case FUEL:
                        message += string.Format("\r\n{0}: 对应油耗值参数不存在", vin);
                        break;
                    case CTNY:
                        message += string.Format("\r\n{0}: 对应车型参数不存在", vin);
                        break;
                    default: break;
                }
            }
            else
            {
                return dr[0];
            }



            //foreach (DataRow dr in dt.Rows)
            //{
            //    bool isMatch = true;
            //    foreach (var dic in paramDict)
            //    {
            //        if (dr[dic.Key].ToString().Trim() != dic.Value)
            //        {
            //            isMatch = false;
            //            break;
            //        }
            //    }
            //    if (isMatch)
            //    {
            //        message += "";
            //        return dr;
            //    }
            //}
            //switch (paramName)
            //{
            //    case FUEL:
            //        message += string.Format("\r\n{0}: 对应油耗值参数不存在", vin);
            //        break;
            //    case CTNY:
            //        message += string.Format("\r\n{0}: 对应车型参数不存在", vin);
            //        break;
            //    default: break;
            //}
            return null;
        }

        /// <summary>
        /// 保存没有生成燃料数据的VIN
        /// </summary>
        /// <param name="drVin"></param>
        /// <returns></returns>
        public string SaveVinBak(DataRow drVin)
        {
            string genMsg = string.Empty;
            string vin = drVin["VIN"].ToString().Trim();
            string strCon = AccessHelper.conn;
            OleDbTransaction tra = null; //创建事务，开始执行事务

            try
            {
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();

                    tra = con.BeginTransaction();

                    OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                    creTime.OleDbType = OleDbType.DBDate;

                    #region 保存VIN信息备用

                    string sqlDel = "DELETE FROM VIN_INFO WHERE VIN = '" + vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                    string sqlStr = @"INSERT INTO VIN_INFO(VIN,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,CLZZRQ,JYJGMC,STATUS,CREATETIME) 
                                            Values 
                                          (@VIN, @CPOS,@YEAR_MODE,@CLXH,@BSX,@CT_PL,@CLZZRQ,@JYJGMC,@STATUS,@CREATETIME)";
                    OleDbParameter[] vinParamList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@CPOS",FormatBrackets(Convert.ToString(drVin["CPOS"]).Trim())),
                                         new OleDbParameter("@YEAR_MODE",FormatBrackets(Convert.ToString(drVin["YEAR_MODE"]).Trim())),
                                         new OleDbParameter("@CLXH",FormatBrackets(Convert.ToString(drVin["CLXH"]).Trim())),
                                         new OleDbParameter("@BSX",FormatBrackets(Convert.ToString(drVin["BSX"]).Trim())),
                                         new OleDbParameter("@CT_PL",FormatBrackets(Convert.ToString(drVin["CT_PL"]).Trim())),
                                         new OleDbParameter("@CLZZRQ",Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim())),
                                         new OleDbParameter("@JYJGMC",FormatBrackets(Convert.ToString(drVin["JYJGMC"]).Trim())),
                                         new OleDbParameter("@STATUS","1"),
                                         creTime
                                      };
                    AccessHelper.ExecuteNonQuery(tra, sqlStr, vinParamList);

                    tra.Commit();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                tra.Rollback();
                genMsg += ex.Message + "\r\n";
            }

            return genMsg;
        }

        /// <summary>
        /// 新导入CPOS信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveCposFuelData(DataTable dtCposFuel)
        {
            int succCount = 0;
            int totalCount = 0;

            string msg = string.Empty;
            string error = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);

            try
            {
                if (dtCposFuel != null && dtCposFuel.Rows.Count > 0)
                {
                    totalCount = dtCposFuel.Rows.Count;
                    con.Open();
                    foreach (DataRow dr in dtCposFuel.Rows)
                    {
                        error = this.VerifyCposFuelData(dr, "IMPORT"); // 校验油耗值数据
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region INSERT

                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO MAIN_FUEL(");
                            strSql.Append("CPOS,YEAR_MODE,CLXH,BSX,CT_PL,QCSCQY,HGSPBM,CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKRLXHL,CT_ZHGKCO2PFL,CREATETIME,UPDATETIME)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@CPOS,@YEAR_MODE,@CLXH,@BSX,@CT_PL,@QCSCQY,@HGSPBM,@CT_SJGKRLXHL,@CT_SQGKRLXHL,@CT_ZHGKRLXHL,@CT_ZHGKCO2PFL,@CREATETIME,@UPDATETIME)");

                            DateTime createDate = DateTime.Now;
                            OleDbParameter createTime = new OleDbParameter("@CREATETIME", createDate);
                            createTime.OleDbType = OleDbType.DBDate;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            OleDbParameter[] parameters = {
                                                          new OleDbParameter("@CPOS",FormatBrackets(Convert.ToString(dr["CPOS"]).Trim())),
                                                          new OleDbParameter("@YEAR_MODE",FormatBrackets(Convert.ToString(dr["YEAR_MODE"]).Trim())),
                                                          new OleDbParameter("@CLXH",FormatBrackets(Convert.ToString(dr["CLXH"]).Trim())),
                                                          new OleDbParameter("@BSX",FormatBrackets(Convert.ToString(dr["BSX"]).Trim())),
                                                          new OleDbParameter("@CT_PL",FormatBrackets(Convert.ToString(dr["CT_PL"]).Trim())),
                                                          new OleDbParameter("@QCSCQY",FormatBrackets(Convert.ToString(dr["QCSCQY"]).Trim())),
                                                          new OleDbParameter("@HGSPBM",Convert.ToString(dr["HGSPBM"]).Trim()),
                                                          new OleDbParameter("@CT_SJGKRLXHL",Convert.ToString(dr["CT_SJGKRLXHL"]).Trim()),
                                                          new OleDbParameter("@CT_SQGKRLXHL",Convert.ToString(dr["CT_SQGKRLXHL"]).Trim()),
                                                          new OleDbParameter("@CT_ZHGKRLXHL",Convert.ToString(dr["CT_ZHGKRLXHL"]).Trim()),
                                                          new OleDbParameter("@CT_ZHGKCO2PFL",Convert.ToString(dr["CT_ZHGKCO2PFL"]).Trim()),
                                                          createTime,
                                                          updateTime
                                                      };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + "\r\n";
            }
            finally
            {
                con.Close();
            }

            if (totalCount - succCount > 0)
            {
                msg += "FAILED-IMPORT";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条导入成功 \r\n \t{2}条导入失败\r\n",
                            totalCount, succCount, totalCount - succCount);
            msg = msgSummary + msg;

            return msg;
        }

        /// <summary>
        /// 修改已经导入的CPOS信息
        /// </summary>
        /// <param name="ds">模板中读取的数据</param>
        /// <param name="cposUpdateList">更新列表，记录更新的数据。存CPOS编号</param>
        /// <returns></returns>
        public string UpdateCposFuelData(DataTable dtCposFuel, List<string> fuelUpdateList)
        {
            int succCount = 0;
            int totalCount = 0;

            string msg = string.Empty;
            string error = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);

            try
            {
                if (dtCposFuel != null && dtCposFuel.Rows.Count > 0)
                {
                    totalCount = dtCposFuel.Rows.Count;
                    con.Open();
                    foreach (DataRow dr in dtCposFuel.Rows)
                    {
                        string cpos = FormatBrackets(Convert.ToString(dr["CPOS"]).Trim());
                        string yearMode = FormatBrackets(Convert.ToString(dr["YEAR_MODE"]).Trim());
                        string clxh = FormatBrackets(Convert.ToString(dr["CLXH"]).Trim());
                        string bsx = FormatBrackets(Convert.ToString(dr["BSX"]).Trim());
                        string pl = FormatBrackets(Convert.ToString(dr["CT_PL"]).Trim());
                        string qcscqy = FormatBrackets(Convert.ToString(dr["QCSCQY"]).Trim());

                        // 根据5个关联字段找到油耗编号
                        DataSet dsFuelId = this.GetMainId(cpos, yearMode, clxh, pl, bsx, qcscqy, FUEL);
                        string fuelId = dsFuelId.Tables[0].Rows[0]["FUEL_ID"].ToString();

                        error = this.VerifyCposFuelData(dr, "UPDATE"); // 校验油耗数据

                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region UPDATE

                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("UPDATE MAIN_FUEL SET ");
                            strSql.Append(@"HGSPBM=@HGSPBM,CT_SJGKRLXHL=@CT_SJGKRLXHL,CT_SQGKRLXHL=@CT_SQGKRLXHL,
                                            CT_ZHGKRLXHL=@CT_ZHGKRLXHL,CT_ZHGKCO2PFL=@CT_ZHGKCO2PFL,UPDATETIME=@UPDATETIME 
                                            WHERE FUEL_ID=@FUEL_ID");

                            DateTime createDate = DateTime.Now;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            OleDbParameter[] parameters = {
                                                          new OleDbParameter("@HGSPBM",Convert.ToString(dr["HGSPBM"]).Trim()),
                                                          new OleDbParameter("@CT_SJGKRLXHL",Convert.ToString(dr["CT_SJGKRLXHL"]).Trim()),
                                                          new OleDbParameter("@CT_SQGKRLXHL",Convert.ToString(dr["CT_SQGKRLXHL"]).Trim()),
                                                          new OleDbParameter("@CT_ZHGKRLXHL",Convert.ToString(dr["CT_ZHGKRLXHL"]).Trim()),
                                                          new OleDbParameter("@CT_ZHGKCO2PFL",Convert.ToString(dr["CT_ZHGKCO2PFL"]).Trim()),
                                                          updateTime,
                                                          new OleDbParameter("@FUEL_ID",fuelId)
                                                      };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;

                            // 加入更新列表。更新完成，界面显示更新过的数据
                            fuelUpdateList.Add(fuelId);
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + "\r\n";
            }
            finally
            {
                con.Close();
            }

            if (totalCount - succCount > 0)
            {
                msg += "FAILED-IMPORT";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条修改成功 \r\n \t{2}条修改失败\r\n",
                            totalCount, succCount, totalCount - succCount);
            msg = msgSummary + msg;

            return msg;
        }

        /// <summary>
        /// 新导入车型参数信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveMainData(DataSet ds)
        {
            int succCount = 0;
            int totalCount = 0;
            string msg = string.Empty;
            //string strCon = AccessHelper.conn;
            //OleDbConnection con = new OleDbConnection(strCon);
            //con.Open();
            //OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

            try
            {
                // 传统能源
                DataTable dtCtny = D2D(ds.Tables[CTNY]); // 转换表头（用户模板中的表头转为数据库列名）
                DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + CTNY + "' and STATUS=1"); // 燃料参数字典

                //// 非插电式混合动力
                //DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);
                //DataRow[] drFcdsHhdl = checkData.Select("FUEL_TYPE='" + FCDSHHDL + "' and STATUS=1");

                // 传统能源
                if (dtCtny != null && dtCtny.Rows.Count > 0)
                {
                    totalCount += dtCtny.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtCtny.Rows)
                    {
                        // 校验车型参数数据
                        error = VerifyMainData(dr, drCtny, "IMPORT", CTNY);      //单行验证

                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region insert
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO MAIN_CTNY(");
                            strSql.Append("MAIN_TYPE,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,JKQCZJXS,QCSCQY,CLZL,RLLX,ZCZBZL,ZGCS,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,LTGG,STATUS,JYBGBH,CT_BSQDWS,CT_BSQXS,CT_EDGL,CT_FDJXH,CT_JGL,CT_QGS,CT_QTXX,CREATETIME,UPDATETIME)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_TYPE,@CPOS,@YEAR_MODE,@CLXH,@BSX,@CT_PL,@JKQCZJXS,@QCSCQY,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@LTGG,@STATUS,@JYBGBH,@CT_BSQDWS,@CT_BSQXS,@CT_EDGL,@CT_FDJXH,@CT_JGL,@CT_QGS,@CT_QTXX,@CREATETIME,@UPDATETIME)");

                            DateTime createDate = DateTime.Now;
                            OleDbParameter createTime = new OleDbParameter("@CREATETIME", createDate);
                            createTime.OleDbType = OleDbType.DBDate;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            string bsx = FormatBrackets(Convert.ToString(dr["BSX"]).Trim());
                            string ct_bsqxs = this.GetBsqxs(bsx);

                            OleDbParameter[] parameters = {
					        new OleDbParameter("@MAIN_TYPE", CTNY),
					        new OleDbParameter("@CPOS", FormatBrackets(Convert.ToString(dr["CPOS"]).Trim())),
					        new OleDbParameter("@YEAR_MODE", FormatBrackets(Convert.ToString(dr["YEAR_MODE"]).Trim())),
					        new OleDbParameter("@CLXH", FormatBrackets(Convert.ToString(dr["CLXH"]).Trim())),
                            new OleDbParameter("@BSX", bsx),
                            new OleDbParameter("@CT_PL", FormatBrackets(Convert.ToString(dr["CT_PL"]).Trim())),
					        new OleDbParameter("@JKQCZJXS", JKQCZJXS),
					        new OleDbParameter("@QCSCQY", Convert.ToString(dr["QCSCQY"]).Trim()),
					        new OleDbParameter("@CLZL", Convert.ToString(dr["CLZL"]).Trim()),
					        new OleDbParameter("@RLLX", Convert.ToString(dr["RLLX"]).Trim()),
					        new OleDbParameter("@ZCZBZL", Convert.ToString(dr["ZCZBZL"]).Trim()),
					        new OleDbParameter("@ZGCS", Convert.ToString(dr["ZGCS"]).Trim()),
					        new OleDbParameter("@ZJ", Convert.ToString(dr["ZJ"]).Trim()),
					        new OleDbParameter("@TYMC", Convert.ToString(dr["TYMC"]).Trim()),
					        new OleDbParameter("@YYC", Convert.ToString(dr["YYC"]).Trim()),
					        new OleDbParameter("@ZWPS", Convert.ToString(dr["ZWPS"]).Trim()),
					        new OleDbParameter("@ZDSJZZL", Convert.ToString(dr["ZDSJZZL"]).Trim()),
					        new OleDbParameter("@EDZK", Convert.ToString(dr["EDZK"]).Trim()),
					        new OleDbParameter("@LJ", Convert.ToString(dr["LJ"]).Trim()),
					        new OleDbParameter("@QDXS", Convert.ToString(dr["QDXS"]).Trim()),
					        new OleDbParameter("@LTGG", Convert.ToString(dr["LTGG"]).Trim()),
					        new OleDbParameter("@STATUS", "1"),
					        new OleDbParameter("@JYBGBH", Convert.ToString(dr["JYBGBH"]).Trim()),
					        new OleDbParameter("@CT_BSQDWS", Convert.ToString(dr["CT_BSQDWS"]).Trim()),
					        new OleDbParameter("@CT_BSQXS", ct_bsqxs),
					        new OleDbParameter("@CT_EDGL", Convert.ToString(dr["CT_EDGL"]).Trim()),
					        new OleDbParameter("@CT_FDJXH", Convert.ToString(dr["CT_FDJXH"]).Trim()),
					        new OleDbParameter("@CT_JGL", Convert.ToString(dr["CT_JGL"]).Trim()),
					        new OleDbParameter("@CT_QGS", Convert.ToString(dr["CT_QGS"]).Trim()),
					        new OleDbParameter("@CT_QTXX", Convert.ToString(dr["CT_QTXX"]).Trim()),
                            createTime,
                            updateTime
                        };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;
                            #endregion
                        }
                    }

                }

                #region 非插电式混合动力

                //if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
                //{
                //    string error = string.Empty;
                //    foreach (DataRow dr in dtFcdsHhdl.Rows)
                //    {
                //        error = VerifyData(dr, drFcdsHhdl, "IMPORT", FCDSHHDL);      //单行验证
                //        if (!string.IsNullOrEmpty(error))
                //        {
                //            msg += error;
                //        }
                //        else
                //        {
                //            #region insert
                //            StringBuilder strSql = new StringBuilder();
                //            strSql.Append("INSERT INTO MAIN_FCDSHHDL(");
                //            strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS,FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC,FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY,FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL,FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH,FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB,FCDS_HHDL_JGL,FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL,FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS,FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL,FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHGKRLXHL,FCDS_HHDL_ZHKGCO2PL,CREATETIME,UPDATE_BY,UPDATETIME)");
                //            strSql.Append(" VALUES (");
                //            strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@FCDS_HHDL_BSQDWS,@FCDS_HHDL_BSQXS,@FCDS_HHDL_CDDMSXZGCS,@FCDS_HHDL_CDDMSXZHGKXSLC,@FCDS_HHDL_DLXDCBNL,@FCDS_HHDL_DLXDCZBCDY,@FCDS_HHDL_DLXDCZZL,@FCDS_HHDL_DLXDCZZNL,@FCDS_HHDL_EDGL,@FCDS_HHDL_FDJXH,@FCDS_HHDL_HHDLJGXS,@FCDS_HHDL_HHDLZDDGLB,@FCDS_HHDL_JGL,@FCDS_HHDL_PL,@FCDS_HHDL_QDDJEDGL,@FCDS_HHDL_QDDJFZNJ,@FCDS_HHDL_QDDJLX,@FCDS_HHDL_QGS,@FCDS_HHDL_SJGKRLXHL,@FCDS_HHDL_SQGKRLXHL,@FCDS_HHDL_XSMSSDXZGN,@FCDS_HHDL_ZHGKRLXHL,@FCDS_HHDL_ZHKGCO2PL,@CREATETIME,@UPDATE_BY,@UPDATETIME)");
                //            OleDbParameter[] parameters = {
                //            new OleDbParameter("@MAIN_ID", OleDbType.VarChar,50),
                //            new OleDbParameter("@CREATE_BY", OleDbType.VarChar,255),
                //            new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
                //            new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
                //            new OleDbParameter("@CLXH", OleDbType.VarChar,100),
                //            new OleDbParameter("@CLZL", OleDbType.VarChar,200),
                //            new OleDbParameter("@RLLX", OleDbType.VarChar,200),
                //            new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                //            new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                //            new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                //            new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                //            new OleDbParameter("@TYMC", OleDbType.VarChar,200),
                //            new OleDbParameter("@YYC", OleDbType.VarChar,200),
                //            new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                //            new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                //            new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                //            new OleDbParameter("@LJ", OleDbType.VarChar,255),
                //            new OleDbParameter("@QDXS", OleDbType.VarChar,200),
                //            new OleDbParameter("@STATUS", OleDbType.VarChar,1),
                //            new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                //            new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                //            new OleDbParameter("@FCDS_HHDL_BSQDWS", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_BSQXS", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_DLXDCBNL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_DLXDCZZL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_EDGL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_FDJXH", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_HHDLJGXS", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_JGL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_PL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_QDDJEDGL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_QDDJLX", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_QGS", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", OleDbType.VarChar,200),
                //            new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", OleDbType.VarChar,200),
                //            new OleDbParameter("@CREATETIME", OleDbType.Date),
                //            new OleDbParameter("@UPDATE_BY", OleDbType.VarChar,200),
                //            new OleDbParameter("@UPDATETIME", OleDbType.Date)};

                //            parameters[0].Value = dr["MAIN_ID"];
                //            parameters[1].Value = Utils.localUserId;
                //            parameters[2].Value = dr["JKQCZJXS"];
                //            parameters[3].Value = dr["QCSCQY"];
                //            parameters[4].Value = dr["CLXH"];
                //            parameters[5].Value = dr["CLZL"];
                //            parameters[6].Value = dr["RLLX"];
                //            parameters[7].Value = dr["ZCZBZL"];
                //            parameters[8].Value = dr["ZGCS"];
                //            parameters[9].Value = dr["LTGG"];
                //            parameters[10].Value = dr["ZJ"];
                //            parameters[11].Value = dr["TYMC"];
                //            parameters[12].Value = dr["YYC"];
                //            parameters[13].Value = dr["ZWPS"];
                //            parameters[14].Value = dr["ZDSJZZL"];
                //            parameters[15].Value = dr["EDZK"];
                //            parameters[16].Value = dr["LJ"];
                //            parameters[17].Value = dr["QDXS"];
                //            parameters[18].Value = (int)Status.待上报;
                //            parameters[19].Value = dr["JYJGMC"];
                //            parameters[20].Value = dr["JYBGBH"];
                //            parameters[21].Value = dr["FCDS_HHDL_BSQDWS"];
                //            parameters[22].Value = dr["FCDS_HHDL_BSQXS"];
                //            parameters[23].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                //            parameters[24].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                //            parameters[25].Value = dr["FCDS_HHDL_DLXDCBNL"];
                //            parameters[26].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                //            parameters[27].Value = dr["FCDS_HHDL_DLXDCZZL"];
                //            parameters[28].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                //            parameters[29].Value = dr["FCDS_HHDL_EDGL"];
                //            parameters[30].Value = dr["FCDS_HHDL_FDJXH"];
                //            parameters[31].Value = dr["FCDS_HHDL_HHDLJGXS"];
                //            parameters[32].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                //            parameters[33].Value = dr["FCDS_HHDL_JGL"];
                //            parameters[34].Value = dr["FCDS_HHDL_PL"];
                //            parameters[35].Value = dr["FCDS_HHDL_QDDJEDGL"];
                //            parameters[36].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                //            parameters[37].Value = dr["FCDS_HHDL_QDDJLX"];
                //            parameters[38].Value = dr["FCDS_HHDL_QGS"];
                //            parameters[39].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                //            parameters[40].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                //            parameters[41].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                //            parameters[42].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                //            parameters[43].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                //            parameters[44].Value = DateTime.Today;
                //            parameters[45].Value = Utils.localUserId;
                //            parameters[46].Value = DateTime.Today;
                //            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                //            #endregion
                //        }
                //    }
                //}

                #endregion

            }
            catch (Exception ex)
            {
                msg += ex.Message + "\r\n";
            }

            if (totalCount - succCount > 0)
            {
                msg += "FAILED-IMPORT";
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条导入成功 \r\n \t{2}条导入失败\r\n",
                            totalCount, succCount, totalCount - succCount);
            msg = msgSummary + msg;

            return msg;
        }

        /// <summary>
        /// 修改已经导入的车型参数信息
        /// </summary>
        /// <param name="ds">模板中读取的数据</param>
        /// <param name="mainUpdateList">更新列表，记录更新的数据。存车型参数编号</param>
        /// <returns></returns>
        public string UpdateMainData(DataSet ds, List<string> mainUpdateList)
        {
            int totalCount = 0;
            int succCount = 0;
            string msg = string.Empty;
            //string strCon = AccessHelper.conn;
            //OleDbConnection con = new OleDbConnection(strCon);
            //con.Open();
            //OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
            try
            {

                // 传统能源
                DataTable dtCtny = D2D(ds.Tables[CTNY]); // 转换表头（用户模板中的表头转为数据库列名）
                DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + CTNY + "' and STATUS=1");

                //// 非插电式混合动力
                //DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);
                //DataRow[] drFcdsHhdl = checkData.Select("FUEL_TYPE='" + FCDSHHDL + "' and STATUS=1");

                // 传统能源
                if (dtCtny != null && dtCtny.Rows.Count > 0)
                {
                    totalCount += dtCtny.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtCtny.Rows)
                    {
                        error = VerifyMainData(dr, drCtny, "UPDATE", CTNY);      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region UPDATE

                            string cpos = FormatBrackets(Convert.ToString(dr["CPOS"]).Trim());
                            string yearMode = FormatBrackets(Convert.ToString(dr["YEAR_MODE"]).Trim());
                            string clxh = FormatBrackets(Convert.ToString(dr["CLXH"]).Trim());
                            string bsx = FormatBrackets(Convert.ToString(dr["BSX"]).Trim());
                            string pl = FormatBrackets(Convert.ToString(dr["CT_PL"]).Trim());
                            string qcscqy = FormatBrackets(Convert.ToString(dr["QCSCQY"]).Trim());

                            // 根据5个关联字段找到车型参数编号
                            DataSet dsMainId = this.GetMainId(cpos, yearMode, clxh, pl, bsx, qcscqy, CTNY);
                            string mainId = dsMainId.Tables[0].Rows[0]["MAIN_ID"].ToString();

                            string sqlCtny = @"UPDATE MAIN_CTNY 
                                         SET QCSCQY=@QCSCQY,CLZL=@CLZL,RLLX=@RLLX,
                                            ZCZBZL=@ZCZBZL,ZGCS=@ZGCS,ZJ=@ZJ,
                                            TYMC=@TYMC,YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,
                                            EDZK=@EDZK,LJ=@LJ,QDXS=@QDXS,LTGG=@LTGG,STATUS=@STATUS,
                                            JYBGBH=@JYBGBH,CT_BSQDWS=@CT_BSQDWS,CT_BSQXS=@CT_BSQXS,
                                            CT_EDGL=@CT_EDGL,CT_FDJXH=@CT_FDJXH,CT_JGL=@CT_JGL,
                                            CT_QGS=@CT_QGS,CT_QTXX=@CT_QTXX
                                          WHERE MAIN_ID=@MAIN_ID";

                            DateTime createDate = DateTime.Now;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            string ct_bsqxs = this.GetBsqxs(bsx);

                            OleDbParameter[] parameters = {
					        //new OleDbParameter("@JKQCZJXS", JKQCZJXS),
					        new OleDbParameter("@QCSCQY", Convert.ToString(dr["QCSCQY"]).Trim()),
					        new OleDbParameter("@CLZL", Convert.ToString(dr["CLZL"]).Trim()),
					        new OleDbParameter("@RLLX", Convert.ToString(dr["RLLX"]).Trim()),

					        new OleDbParameter("@ZCZBZL", Convert.ToString(dr["ZCZBZL"]).Trim()),
					        new OleDbParameter("@ZGCS", Convert.ToString(dr["ZGCS"]).Trim()),
					        new OleDbParameter("@ZJ", Convert.ToString(dr["ZJ"]).Trim()),
					        new OleDbParameter("@TYMC", Convert.ToString(dr["TYMC"]).Trim()),

					        new OleDbParameter("@YYC", Convert.ToString(dr["YYC"]).Trim()),
					        new OleDbParameter("@ZWPS", Convert.ToString(dr["ZWPS"]).Trim()),
					        new OleDbParameter("@ZDSJZZL", Convert.ToString(dr["ZDSJZZL"]).Trim()),
					        new OleDbParameter("@EDZK", Convert.ToString(dr["EDZK"]).Trim()),
					        new OleDbParameter("@LJ", Convert.ToString(dr["LJ"]).Trim()),

					        new OleDbParameter("@QDXS", Convert.ToString(dr["QDXS"]).Trim()),
					        new OleDbParameter("@LTGG", Convert.ToString(dr["LTGG"]).Trim()),
					        new OleDbParameter("@STATUS", "1"),
					        new OleDbParameter("@JYBGBH", Convert.ToString(dr["JYBGBH"]).Trim()),

					        new OleDbParameter("@CT_BSQDWS", Convert.ToString(dr["CT_BSQDWS"]).Trim()),
					        new OleDbParameter("@CT_BSQXS", ct_bsqxs),
					        new OleDbParameter("@CT_EDGL", Convert.ToString(dr["CT_EDGL"]).Trim()),
					        new OleDbParameter("@CT_FDJXH", Convert.ToString(dr["CT_FDJXH"]).Trim()),
					        new OleDbParameter("@CT_JGL", Convert.ToString(dr["CT_JGL"]).Trim()),

					        new OleDbParameter("@CT_QGS", Convert.ToString(dr["CT_QGS"]).Trim()),
					        new OleDbParameter("@CT_QTXX", Convert.ToString(dr["CT_QTXX"]).Trim()),
                            
                            new OleDbParameter("@MAIN_ID", Convert.ToInt64(mainId))
                        };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlCtny.ToString(), parameters);
                            succCount++;

                            // 加入更新列表。更新完成，界面显示更新过的数据
                            mainUpdateList.Add(mainId);

                            #endregion
                        }
                    }
                }

                #region 非插电式混合动力

                //            if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
                //            {
                //                string error = string.Empty;
                //                foreach (DataRow dr in dtFcdsHhdl.Rows)
                //                {
                //                    error = VerifyMainData(dr, drFcdsHhdl, "UPDATE", FCDSHHDL);      //单行验证
                //                    if (!string.IsNullOrEmpty(error))
                //                    {
                //                        msg += error;
                //                    }
                //                    else
                //                    {
                //                        #region UPDATE
                //                        mainId = dr["MAIN_ID"].ToString();
                //                        string sqlFcds = @"UPDATE MAIN_FCDSHHDL
                //                                        SET JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                //                                            ZCZBZL=@ZCZBZL,ZGCS=@ZGCS,LTGG=@LTGG,ZJ=@ZJ,TYMC=@TYMC,
                //                                            YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,EDZK=@EDZK,LJ=@LJ,
                //                                            QDXS=@QDXS,STATUS=@STATUS,JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,
                //                                            FCDS_HHDL_BSQDWS=@FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS=@FCDS_HHDL_BSQXS,
                //                                            FCDS_HHDL_CDDMSXZGCS=@FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC=@FCDS_HHDL_CDDMSXZHGKXSLC,
                //                                            FCDS_HHDL_DLXDCBNL=@FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY=@FCDS_HHDL_DLXDCZBCDY,
                //                                            FCDS_HHDL_DLXDCZZL=@FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL=@FCDS_HHDL_DLXDCZZNL,
                //                                            FCDS_HHDL_EDGL=@FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH=@FCDS_HHDL_FDJXH,
                //                                            FCDS_HHDL_HHDLJGXS=@FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB=@FCDS_HHDL_HHDLZDDGLB,
                //                                            FCDS_HHDL_JGL=@FCDS_HHDL_JGL,FCDS_HHDL_PL=@FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL=@FCDS_HHDL_QDDJEDGL,
                //                                            FCDS_HHDL_QDDJFZNJ=@FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX=@FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS=@FCDS_HHDL_QGS,
                //                                            FCDS_HHDL_SJGKRLXHL=@FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL=@FCDS_HHDL_SQGKRLXHL,
                //                                            FCDS_HHDL_XSMSSDXZGN=@FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHGKRLXHL=@FCDS_HHDL_ZHGKRLXHL,
                //                                            FCDS_HHDL_ZHKGCO2PL=@FCDS_HHDL_ZHKGCO2PL,UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME
                //                                         WHERE MAIN_ID=@MAIN_ID";

                //                        OleDbParameter[] parameters = {
                //                        new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
                //                        new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
                //                        new OleDbParameter("@CLXH", OleDbType.VarChar,100),
                //                        new OleDbParameter("@CLZL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@RLLX", OleDbType.VarChar,200),

                //                        new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                //                        new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                //                        new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                //                        new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                //                        new OleDbParameter("@TYMC", OleDbType.VarChar,200),

                //                        new OleDbParameter("@YYC", OleDbType.VarChar,200),
                //                        new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                //                        new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                //                        new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                //                        new OleDbParameter("@LJ", OleDbType.VarChar,255),

                //                        new OleDbParameter("@QDXS", OleDbType.VarChar,200),
                //                        new OleDbParameter("@STATUS", OleDbType.VarChar,1),
                //                        new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                //                        new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                //                        new OleDbParameter("@FCDS_HHDL_BSQDWS", OleDbType.VarChar,200),

                //                        new OleDbParameter("@FCDS_HHDL_BSQXS", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_DLXDCBNL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", OleDbType.VarChar,200),

                //                        new OleDbParameter("@FCDS_HHDL_DLXDCZZL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_EDGL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_FDJXH", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_HHDLJGXS", OleDbType.VarChar,200),

                //                        new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_JGL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_PL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_QDDJEDGL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", OleDbType.VarChar,200),

                //                        new OleDbParameter("@FCDS_HHDL_QDDJLX", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_QGS", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", OleDbType.VarChar,200),

                //                        new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", OleDbType.VarChar,200),
                //                        new OleDbParameter("@UPDATE_BY", OleDbType.VarChar,200),
                //                        new OleDbParameter("@UPDATETIME", OleDbType.Date),

                //                        new OleDbParameter("@MAIN_ID", OleDbType.VarChar,50)
                //                                                      };

                //                        parameters[0].Value = dr["JKQCZJXS"];
                //                        parameters[1].Value = dr["QCSCQY"];
                //                        parameters[2].Value = dr["CLXH"];
                //                        parameters[3].Value = dr["CLZL"];
                //                        parameters[4].Value = dr["RLLX"];

                //                        parameters[5].Value = dr["ZCZBZL"];
                //                        parameters[6].Value = dr["ZGCS"];
                //                        parameters[7].Value = dr["LTGG"];
                //                        parameters[8].Value = dr["ZJ"];
                //                        parameters[9].Value = dr["TYMC"];

                //                        parameters[10].Value = dr["YYC"];
                //                        parameters[11].Value = dr["ZWPS"];
                //                        parameters[12].Value = dr["ZDSJZZL"];
                //                        parameters[13].Value = dr["EDZK"];
                //                        parameters[14].Value = dr["LJ"];

                //                        parameters[15].Value = dr["QDXS"];
                //                        parameters[16].Value = (int)Status.待上报;
                //                        parameters[17].Value = dr["JYJGMC"];
                //                        parameters[18].Value = dr["JYBGBH"];
                //                        parameters[19].Value = dr["FCDS_HHDL_BSQDWS"];

                //                        parameters[20].Value = dr["FCDS_HHDL_BSQXS"];
                //                        parameters[21].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                //                        parameters[22].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                //                        parameters[23].Value = dr["FCDS_HHDL_DLXDCBNL"];
                //                        parameters[24].Value = dr["FCDS_HHDL_DLXDCZBCDY"];

                //                        parameters[25].Value = dr["FCDS_HHDL_DLXDCZZL"];
                //                        parameters[26].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                //                        parameters[27].Value = dr["FCDS_HHDL_EDGL"];
                //                        parameters[28].Value = dr["FCDS_HHDL_FDJXH"];
                //                        parameters[29].Value = dr["FCDS_HHDL_HHDLJGXS"];

                //                        parameters[30].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                //                        parameters[31].Value = dr["FCDS_HHDL_JGL"];
                //                        parameters[32].Value = dr["FCDS_HHDL_PL"];
                //                        parameters[33].Value = dr["FCDS_HHDL_QDDJEDGL"];
                //                        parameters[34].Value = dr["FCDS_HHDL_QDDJFZNJ"];

                //                        parameters[35].Value = dr["FCDS_HHDL_QDDJLX"];
                //                        parameters[36].Value = dr["FCDS_HHDL_QGS"];
                //                        parameters[37].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                //                        parameters[38].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                //                        parameters[39].Value = dr["FCDS_HHDL_XSMSSDXZGN"];

                //                        parameters[40].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                //                        parameters[41].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                //                        parameters[42].Value = Utils.localUserId;
                //                        parameters[43].Value = DateTime.Today;
                //                        parameters[44].Value = dr["MAIN_ID"];

                //                        AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlFcds.ToString(), parameters);
                //                        mainUpdateList.Add(mainId);
                //                        #endregion
                //                    }
                //                }
                //            }

                #endregion

            }
            catch (Exception ex)
            {
                msg += ex.Message + "\r\n";
            }

            if (totalCount - succCount > 0)
            {
                msg += "FAILED-IMPORT";
            }
            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条修改成功 \r\n \t{2}条修改失败\r\n",
                            totalCount, succCount, totalCount - succCount);
            msg = msgSummary + msg;

            return msg;
        }

        /// <summary>
        /// 根据变速箱,获取变速器型式
        /// </summary>
        /// <param name="bsx"></param>
        /// <returns></returns>
        public string GetBsqxs(string bsx)
        {
            string ct_bsqxs = string.Empty;

            if (bsx.Contains("AT"))
            { ct_bsqxs = "AT"; }
            else if (bsx.Contains("MT"))
            { ct_bsqxs = "MT"; }
            else if (bsx.Contains("AMT"))
            { ct_bsqxs = "AMT"; }
            else if (bsx.Contains("CVT"))
            { ct_bsqxs = "CVT"; }
            else if (bsx.Contains("DCT"))
            { ct_bsqxs = "DCT"; }
            else
            { ct_bsqxs = "其它"; }

            return ct_bsqxs;
        }

        /// <summary>
        /// 保存已经就绪的数据
        /// </summary>
        /// <param name="drVin"></param>
        /// <param name="drMain"></param>
        /// <returns></returns>
        public string SaveReadyData(DataRow drVin, DataRow drMain, DataRow drCposFuel, DataTable dtPam, string mainType)
        {
            string genMsg = string.Empty;
            string strCon = AccessHelper.conn;

            try
            {
                string strCreater = Utils.userId;
                string vin = Convert.ToString(drVin["VIN"]).Trim().ToUpper();

                // 如果当前vin数据已经存在，则跳过
                if (this.IsFuelDataExist(vin))
                {
                    genMsg += vin + "已经存在。\r\n";
                    return genMsg;
                }
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    OleDbTransaction tra = null; //创建事务，开始执行事务
                    try
                    {
                        #region 待生成的燃料基本信息数据存入燃料基本信息表

                        tra = con.BeginTransaction();
                        string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,MAIN_ID,MAIN_TYPE,FUEL_ID,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,STATUS,CREATETIME,UPDATETIME,HGSPBM,QTXX
                                ) VALUES
                                (   @VIN,@MAIN_ID,@MAIN_TYPE,@FUEL_ID,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@STATUS,@CREATETIME,@UPDATETIME,@HGSPBM,@QTXX)";

                        DateTime clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim());
                        OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                        clzzrq.OleDbType = OleDbType.DBDate;

                        DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);
                        OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                        uploadDeadline.OleDbType = OleDbType.DBDate;

                        OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                        creTime.OleDbType = OleDbType.DBDate;
                        OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                        upTime.OleDbType = OleDbType.DBDate;

                        OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@MAIN_ID",Convert.ToString(drMain["MAIN_ID"]).Trim()),
                                     new OleDbParameter("@MAIN_TYPE",mainType),
                                     new OleDbParameter("@FUEL_ID",Convert.ToString(drCposFuel["FUEL_ID"]).Trim()),
                                     new OleDbParameter("@USER_ID",strCreater),
                                     new OleDbParameter("@QCSCQY",Convert.ToString(drMain["QCSCQY"])),
                                     new OleDbParameter("@JKQCZJXS",Convert.ToString(drMain["JKQCZJXS"]).Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@CLXH",Convert.ToString(drMain["CLXH"]).Trim()),
                                     new OleDbParameter("@CLZL",Convert.ToString(drMain["CLZL"]).Trim()),
                                     new OleDbParameter("@RLLX",Convert.ToString(drMain["RLLX"]).Trim()),
                                     new OleDbParameter("@ZCZBZL",Convert.ToString(drMain["ZCZBZL"]).Trim()),
                                     new OleDbParameter("@ZGCS",Convert.ToString(drMain["ZGCS"]).Trim()),
                                     new OleDbParameter("@LTGG",Convert.ToString(drMain["LTGG"]).Trim()),
                                     new OleDbParameter("@ZJ",Convert.ToString(drMain["ZJ"]).Trim()),
                                     new OleDbParameter("@TYMC",Convert.ToString(drMain["TYMC"]).Trim()),
                                     new OleDbParameter("@YYC",Convert.ToString(drMain["YYC"]).Trim()),
                                     new OleDbParameter("@ZWPS",Convert.ToString(drMain["ZWPS"]).Trim()),
                                     new OleDbParameter("@ZDSJZZL",Convert.ToString(drMain["ZDSJZZL"]).Trim()),
                                     new OleDbParameter("@EDZK",Convert.ToString(drMain["EDZK"]).Trim()),
                                     new OleDbParameter("@LJ",Convert.ToString(drMain["LJ"]).Trim()),
                                     new OleDbParameter("@QDXS",Convert.ToString(drMain["QDXS"]).Trim()),
                                     new OleDbParameter("@JYJGMC",Convert.ToString(drVin["JYJGMC"]).Trim()),
                                     new OleDbParameter("@JYBGBH",Convert.ToString(drMain["JYBGBH"]).Trim()),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","1"),
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@HGSPBM",Convert.ToString(drCposFuel["HGSPBM"]).Trim()),
                                     new OleDbParameter("@QTXX",Convert.ToString(drMain["CT_QTXX"]).Trim()),
                                     };
                        AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                        #endregion

                        #region 插入参数信息

                        string sqlDelParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + vin + "'";
                        AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                        // 待生成的燃料参数信息存入燃料参数表
                        foreach (DataRow drParam in dtPam.Rows)
                        {
                            string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                            string paraValue = string.Empty;
                            if (ParamName.IndexOf(paramCode) < 0)
                            {
                                paraValue = Convert.ToString(drMain[paramCode]).Trim();
                            }
                            else
                            {
                                paraValue = Convert.ToString(drCposFuel[paramCode]).Trim();
                            }
                            string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY 
                                            (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                      VALUES
                                            (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";
                            OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",paramCode),
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@PARAM_VALUE",paraValue),
                                     new OleDbParameter("@V_ID","")
                                   };
                            AccessHelper.ExecuteNonQuery(tra, sqlInsertParam, paramList);
                        }
                        #endregion

                        #region 保存VIN信息备用

                        string sqlDel = "DELETE FROM VIN_INFO WHERE VIN = '" + vin + "'";
                        AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                        string sqlStr = @"INSERT INTO VIN_INFO(VIN,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,CLZZRQ,JYJGMC,STATUS,CREATETIME) 
                                            Values 
                                          (@VIN, @CPOS,@YEAR_MODE,@CLXH,@BSX,@CT_PL,@CLZZRQ,@JYJGMC,@STATUS,@CREATETIME)";
                        OleDbParameter[] vinParamList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@CPOS",FormatBrackets(Convert.ToString(drVin["CPOS"]).Trim())),
                                         new OleDbParameter("@YEAR_MODE",FormatBrackets(Convert.ToString(drVin["YEAR_MODE"]).Trim())),
                                         new OleDbParameter("@CLXH",FormatBrackets(Convert.ToString(drVin["CLXH"]).Trim())),
                                         new OleDbParameter("@BSX",FormatBrackets(Convert.ToString(drVin["BSX"]).Trim())),
                                         new OleDbParameter("@CT_PL",FormatBrackets(Convert.ToString(drVin["CT_PL"]).Trim())),
                                         new OleDbParameter("@CLZZRQ",Convert.ToDateTime(drVin["CLZZRQ"])),
                                         new OleDbParameter("@JYJGMC",Convert.ToString(drVin["JYJGMC"]).Trim()),
                                         new OleDbParameter("@STATUS","0"),
                                         creTime
                                      };
                        AccessHelper.ExecuteNonQuery(tra, sqlStr, vinParamList);

                        #endregion

                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                genMsg += ex.Message;
            }

            return genMsg;
        }

        /// <summary>
        /// 提取燃料消耗量数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveFuelData(DataSet ds)
        {
            string genMsg = string.Empty;
            string strCon = AccessHelper.conn;
            int pageSize = 20;
            ProcessForm pf = new ProcessForm();
            pf.TotalMax = (int)Math.Ceiling((decimal)ds.Tables[0].Rows.Count / (decimal)pageSize);
            pf.Show();
            try
            {
                System.Data.DataTable dt = ds.Tables[0];
                string strCreater = Utils.userId;
                string sqlQueryParam = "SELECT PARAM_CODE "
                                + " FROM RLLX_PARAM WHERE (FUEL_TYPE='传统能源' AND STATUS='1')";
                System.Data.DataTable dtPam = AccessHelper.ExecuteDataSet(strCon, sqlQueryParam, null).Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    using (OleDbConnection con = new OleDbConnection(strCon))
                    {
                        con.Open();
                        foreach (DataRow dr in dt.Rows)
                        {
                            OleDbTransaction tra = null; //创建事务，开始执行事务
                            try
                            {
                                string vin = dr["VIN"].ToString().Trim().ToUpper();
                                //string sqlDelBasic = "DELETE FROM FC_CLJBXX WHERE VIN = '" + vin + "' AND STATUS='1'";
                                //AccessHelper.ExecuteNonQuery(tra, sqlDelBasic, null);

                                // 如果当前vin数据已经存在，则跳过
                                if (this.IsFuelDataExist(vin))
                                {
                                    genMsg += vin + "已经存在。\r\n";
                                    continue;
                                }

                                #region 待生成的燃料基本信息数据存入燃料基本信息表

                                tra = con.BeginTransaction();
                                string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,STATUS,CREATETIME,UPDATETIME
                                ) VALUES
                                (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@STATUS,@CREATETIME,@UPDATETIME)";

                                DateTime clzzrqDate = Convert.ToDateTime(dr["CLZZRQ"].ToString().Trim());
                                OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                                clzzrq.OleDbType = OleDbType.DBDate;

                                DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                                OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                                uploadDeadline.OleDbType = OleDbType.DBDate;

                                OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                                creTime.OleDbType = OleDbType.DBDate;
                                OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                                upTime.OleDbType = OleDbType.DBDate;

                                OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@USER_ID",strCreater),
                                     new OleDbParameter("@QCSCQY",dr["QCSCQY"].ToString().Trim()),
                                     new OleDbParameter("@JKQCZJXS",dr["JKQCZJXS"].ToString().Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@CLXH",dr["CLXH"].ToString().Trim()),
                                     new OleDbParameter("@CLZL",dr["CLZL"].ToString().Trim()),
                                     new OleDbParameter("@RLLX",dr["RLLX"].ToString().Trim()),
                                     new OleDbParameter("@ZCZBZL",dr["ZCZBZL"].ToString().Trim()),
                                     new OleDbParameter("@ZGCS",dr["ZGCS"].ToString().Trim()),
                                     new OleDbParameter("@LTGG",dr["LTGG"].ToString().Trim()),
                                     new OleDbParameter("@ZJ",dr["ZJ"].ToString().Trim()),
                                     new OleDbParameter("@TYMC",dr["TYMC"].ToString().Trim()),
                                     new OleDbParameter("@YYC",dr["YYC"].ToString().Trim()),
                                     new OleDbParameter("@ZWPS",dr["ZWPS"].ToString().Trim()),
                                     new OleDbParameter("@ZDSJZZL",dr["ZDSJZZL"].ToString().Trim()),
                                     new OleDbParameter("@EDZK",dr["EDZK"].ToString().Trim()),
                                     new OleDbParameter("@LJ",dr["LJ"].ToString().Trim()),
                                     new OleDbParameter("@QDXS",dr["QDXS"].ToString().Trim()),
                                     new OleDbParameter("@JYJGMC",dr["JYJGMC"].ToString().Trim()),
                                     new OleDbParameter("@JYBGBH",dr["JYBGBH"].ToString().Trim()),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","9"),
                                     creTime,
                                     upTime
                                     };
                                AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                                #endregion

                                #region 删除参数表中vin已存在的信息

                                string sqlDelParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + vin + "'";
                                AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                                // 待生成的燃料参数信息存入燃料参数表
                                foreach (DataRow drParam in dtPam.Rows)
                                {
                                    string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                                    string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY 
                                            (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                      VALUES
                                            (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";
                                    OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",paramCode),
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@PARAM_VALUE",dr[paramCode]),
                                     new OleDbParameter("@V_ID","")
                                   };
                                    AccessHelper.ExecuteNonQuery(tra, sqlInsertParam, paramList);
                                }

                                #endregion

                                // 待生成的VIN数据插入VIN历史表
                                string sqlInsertHis = @"INSERT INTO VIN_INFO_HIS(VIN,MAIN_ID) Values (@VIN, @MAIN_ID)";
                                OleDbParameter[] paramHis = { 
                                                        new OleDbParameter("@VIN", vin),
                                                        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"].ToString().Trim())
                                                 };
                                AccessHelper.ExecuteNonQuery(tra, sqlInsertHis, paramHis);

                                // 删除已经用完的待生成VIN数据
                                string sqlDelVinInfo = @"DELETE FROM VIN_INFO WHERE VIN='" + vin + "'";
                                AccessHelper.ExecuteNonQuery(tra, sqlDelVinInfo, null);

                                tra.Commit();
                                pf.progressBarControl1.PerformStep();
                                System.Windows.Forms.Application.DoEvents();

                            }
                            catch (Exception ex)
                            {
                                tra.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            pf.Close();
            return genMsg;
        }

        /// <summary>
        /// 读取模板表头和数据库字段对应关系模板
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadTemplate(string filePath)
        {
            DataSet ds = this.ReadTemplateExcel(filePath);
            dictCTNY = new Dictionary<string, string>();
            dictFCDSHHDL = new Dictionary<string, string>();
            dictFUEL = new Dictionary<string, string>();
            dictVin = new Dictionary<string, string>();

            foreach (DataRow r in ds.Tables[CTNY].Rows)
            {
                dictCTNY.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            //foreach (DataRow r in ds.Tables[FCDSHHDL].Rows)
            //{
            //    dictFCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            //}

            foreach (DataRow r in ds.Tables[FUEL].Rows)
            {
                dictFUEL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[VIN].Rows)
            {
                dictVin.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
        }

        /// <summary>
        /// 读取导入模板和数据库字段对性关系，模板保存在根目录的ExcelHeaderTemplate文件夹下
        /// </summary>
        /// <param name="fileName"></param>
        public DataSet ReadTemplateExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [传统能源$]", strConn);
                oada.Fill(ds, CTNY);

                //oada = new OleDbDataAdapter("select * from [非插电式混合动力$]", strConn);
                //oada.Fill(ds, FCDSHHDL);

                oada = new OleDbDataAdapter("SELECT * FROM [VIN$]", strConn);
                oada.Fill(ds, VIN);

                oada = new OleDbDataAdapter("SELECT * FROM [FUEL$]", strConn);
                oada.Fill(ds, FUEL);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 转移已用完的文件
        /// </summary>
        /// <param name="srcFileName">源文件路径</param>
        /// <param name="folderPath">目的文件夹路径</param>
        /// <param name="fileType">文件类型</param>
        public string MoveFinishedFile(string srcFileName, string folderPath, string fileType)
        {
            string msg = string.Empty;
            string folderName = Path.Combine(folderPath, FILE_NAME[fileType].ToString());

            try
            {
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }
                string shortFileName = Path.GetFileNameWithoutExtension(srcFileName) + DateTime.Now.ToString("-yyyy-MMdd-HHmmss") + Path.GetExtension(srcFileName);
                string desFileName = Path.Combine(folderName, shortFileName);

                File.Move(srcFileName, desFileName);
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 删除车型参数或CPOS
        /// </summary>
        /// <param name="deleteType">修改类型：“FUEL”表示修改车型参数对应的油耗数据；“CPOS”表示修改轮胎规格对应的油耗数据</param>
        /// <param name="ids">要删除的数据标识</param>
        /// <returns></returns>
        public string DeleteMainCpos(string deleteType, string ids)
        {
            string msg = string.Empty;
            string tableName = string.Empty;
            string sqlHis = string.Empty;
            string field = string.Empty;
            OleDbConnection con = new OleDbConnection(AccessHelper.conn);
            OleDbTransaction tra = null;
            try
            {
                con.Open();
                tra = con.BeginTransaction();

                if (deleteType == "MAIN")
                {
                    tableName = "MAIN_CTNY";
                    field = "MAIN_ID";

                    sqlHis = string.Format(@"INSERT INTO MAIN_CTNY_HIS(MAIN_ID,MAIN_TYPE,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,JKQCZJXS,
                                                    QCSCQY,CLZL,RLLX,ZCZBZL,ZGCS,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,LTGG,
                                                    STATUS,JYBGBH,CT_BSQDWS,CT_BSQXS,CT_EDGL,CT_FDJXH,CT_JGL,CT_QGS,
                                                    CT_QTXX,CREATETIME,UPDATETIME)
                                                SELECT MAIN_ID,MAIN_TYPE,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,JKQCZJXS,
                                                    QCSCQY,CLZL,RLLX,ZCZBZL,ZGCS,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,LTGG,
                                                    STATUS,JYBGBH,CT_BSQDWS,CT_BSQXS,CT_EDGL,CT_FDJXH,CT_JGL,CT_QGS,
                                                    CT_QTXX,CREATETIME,UPDATETIME
                                                FROM MAIN_CTNY
                                                WHERE MAIN_ID IN ({0})", ids);
                }
                else if (deleteType == "FUEL")
                {
                    tableName = "MAIN_FUEL";
                    field = "FUEL_ID";

                    sqlHis = string.Format(@"INSERT INTO MAIN_FUEL_HIS(FUEL_ID,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,QCSCQY,HGSPBM,CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKRLXHL,CT_ZHGKCO2PFL,CREATETIME,UPDATETIME)
                                                SELECT FUEL_ID,CPOS,YEAR_MODE,CLXH,BSX,CT_PL,QCSCQY,HGSPBM,CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKRLXHL,CT_ZHGKCO2PFL,CREATETIME,UPDATETIME
                                                FROM MAIN_FUEL
                                                WHERE FUEL_ID IN ({0})", ids);
                }

                string sqlDel = string.Format(@"DELETE * FROM {0} WHERE {1} IN ({2})", tableName, field, ids);

                AccessHelper.ExecuteNonQuery(tra, sqlHis, null);
                AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                tra.Commit();
            }
            catch (Exception ex)
            {
                tra.Rollback();
                msg += ex.Message + "\r\n";
            }
            finally
            {
                con.Close();
            }

            return msg;
        }

        /// <summary>
        /// 转换表头
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable D2D(DataTable dt)
        {
            string fiedName = "CPOS"; // "CPOS"是数据库车型参数表中的字段,用于验证某行是否为空数据
            DataTable d = new DataTable();

            for (int i = 0; i < dt.Columns.Count; )
            {
                DataColumn c = dt.Columns[i];

                if (dt.TableName == CTNY)
                {
                    if (!dictCTNY.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictCTNY[c.ColumnName]);
                }
                else if (dt.TableName == FCDSHHDL)
                {
                    if (!dictFCDSHHDL.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictFCDSHHDL[c.ColumnName]);
                }
                else if (dt.TableName == FUEL)
                {
                    fiedName = "CPOS";  // "CPOS"是数据库油耗值表中的字段,用于验证某行是否为空数据

                    if (!dictFUEL.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictFUEL[c.ColumnName]);
                }
                else if (dt.TableName == VIN)
                {
                    fiedName = "VIN";

                    if (!dictVin.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictVin[c.ColumnName]);
                }
                i++;
            }

            foreach (DataRow r in dt.Rows)
            {
                if (r[fiedName] != null && !string.IsNullOrEmpty(Convert.ToString(r[fiedName])))
                {
                    DataRow ddr = d.NewRow();
                    ddr = r;
                    d.Rows.Add(ddr.ItemArray);
                }
            }

            return d;
        }

        /// <summary>
        /// 模板列头转置表列头
        /// </summary>
        /// <param name="str">模板列头</param>
        ///  <param name="type">燃料类型</param>
        /// <returns></returns>
        private string s2s(string str)
        {
            try
            {
                return dictVin[str];
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// 检查当前VIN数据是否已经存在于燃料数据表中
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        protected bool IsFuelDataExist(string vin)
        {
            bool isExist = false;

            string sqlQuery = @"SELECT * FROM FC_CLJBXX WHERE VIN='" + vin + "'";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQuery, null);

            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    isExist = true;
                }
            }

            return isExist;
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

        public DataTable GetImportedVinData(string vin)
        {
            DataSet dsQuery = new DataSet();
            string sqlQuery = @"SELECT VI.* FROM VIN_INFO VI WHERE VI.STATUS='1' ";

            string sw = string.Empty;
            if (!string.IsNullOrEmpty(vin))
            {
                sw += string.Format(" AND VIN LIKE '%{0}%'", vin);
            }

            try
            {
                dsQuery = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQuery + sw, null);
            }
            catch (Exception ex)
            {
                throw ex;
                //MessageBox.Show(ex.Message, "数据未准备就绪", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return dsQuery.Tables[0];
        }

        /// <summary>
        /// 获取全部车型参数数据，用作合并VIN数据
        /// </summary>
        /// <returns></returns>
        public bool GetMainData(string tableType)
        {
            bool flag = true;

            try
            {
                string sqlCtny = string.Format(@"SELECT * FROM MAIN_CTNY");
                DataSet dsCtny = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                dtCtnyStatic = dsCtny.Tables[0];

                string sqlCposFuel = string.Format(@"SELECT * FROM MAIN_FUEL");
                DataSet dsCposFuel = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCposFuel, null);
                dtFuelStatic = dsCposFuel.Tables[0];

                if (tableType == "MAIN_RLLX")
                {
                    // string sqlFcds = string.Format(@"SELECT * FROM MAIN_FCDSHHDL");
                    //ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
                    //dtFcdsStatic = ds.Tables[0];

                    if (dtCtnyStatic.Rows.Count < 1)  // && dtFcdsStatic.Rows.Count < 1)
                    {
                        flag = false;
                    }
                }
                else if (tableType == "MAIN_FUEL")
                {
                    if (dtFuelStatic.Rows.Count < 1)
                    {
                        flag = false;
                    }
                }
            }
            catch (Exception)
            {
                flag = false;
            }

            return flag;
        }

        /// <summary>
        /// 获取已经导入的参数编码（MAIN_ID）,用于导入判断
        /// </summary>
        public DataSet GetMainId(string cpos, string yearMode, string clxh, string pl, string bsx, string qcscqy, string mainType)
        {
            //int dataCount;
            DataSet dsMain = new DataSet();

            string tableName = string.Empty;
            string fieldName = "MAIN_ID";
            if (mainType == CTNY)
            {
                tableName = "MAIN_CTNY";
            }
            if (mainType == FCDSHHDL)
            {
                tableName = "MAIN_FCDSHHDL";
            }
            if (mainType == FUEL)
            {
                tableName = "MAIN_FUEL";
                fieldName = "FUEL_ID";
            }
            // 其他燃料类型用到时再加

            string sqlMain = string.Format(@"SELECT CPOS,YEAR_MODE,CLXH,CT_PL,BSX, {7} FROM {0} WHERE CPOS='{1}' AND YEAR_MODE='{2}' AND CLXH='{3}' AND CT_PL='{4}' AND BSX='{5}' AND QCSCQY='{6}' ", tableName, cpos, yearMode, clxh, pl, bsx, qcscqy, fieldName);
            try
            {
                dsMain = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlMain, null);
                //dataCount = dsMain.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
                throw ex;
                //dataCount = 0;
            }
            return dsMain;
        }

        /// <summary>
        /// 获取已经导入的CPOS,用于导入判断
        /// </summary>
        public int GetCposFuelData(string cpos, string yearMode, string clxh, string pl, string bsx, string qcscqy)
        {
            int dataCount;

            string sqlCposFuel = string.Format(@"SELECT FUEL_ID FROM MAIN_FUEL WHERE CPOS='{0}' AND YEAR_MODE='{1}'
                                                 AND CLXH='{2}' AND CT_PL='{3}' AND BSX='{4}' AND QCSCQY='{5}'", cpos, yearMode, clxh, pl, bsx, qcscqy);
            try
            {
                DataSet dsCposFuel = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCposFuel, null);
                dataCount = dsCposFuel.Tables[0].Rows.Count;
            }
            catch (Exception)
            {
                dataCount = 0;
            }
            return dataCount;
        }

        /// <summary>
        /// 修改或同步数据时，有部分字段会丢失。此方法为在进行此操作前获取这些字段
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        public string[] GetOldValue(string vin)
        {
            List<string> oldValueList = new List<string>();
            string sqlUser = string.Format(@"SELECT MAIN_ID, MAIN_TYPE, FUEL_ID, USER_ID FROM FC_CLJBXX WHERE VIN='{0}'", vin);
            try
            {
                DataSet dsOldValue = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlUser, null);
                if (dsOldValue != null && dsOldValue.Tables[0].Rows.Count > 0)
                {
                    oldValueList.Add(Convert.ToString(dsOldValue.Tables[0].Rows[0]["MAIN_ID"]));
                    oldValueList.Add(Convert.ToString(dsOldValue.Tables[0].Rows[0]["MAIN_TYPE"]));
                    oldValueList.Add(Convert.ToString(dsOldValue.Tables[0].Rows[0]["FUEL_ID"]));
                    oldValueList.Add(Convert.ToString(dsOldValue.Tables[0].Rows[0]["USER_ID"]));
                }
            }
            catch (Exception)
            {
            }
            return oldValueList.ToArray();
        }

        /// <summary>
        /// 从控件中查询被选中的数据
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<string> GetMainIdFromControl(GridView gv, DataTable dt, string updateType)
        {
            string fieldName = string.Empty;
            List<string> mainIdList = new List<string>();
            gv.PostEditor();

            if (updateType == "MAIN")
            {
                fieldName = "MAIN_ID";
            }
            else if (updateType == "FUEL")
            {
                fieldName = "FUEL_ID";
            }

            if (dt != null)
            {
                DataRow[] drVinArr = dt.Select("check=True");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((bool)dt.Rows[i]["check"])
                    {
                        mainIdList.Add(dt.Rows[i][fieldName].ToString());
                    }
                }
            }
            return mainIdList;
        }

        /// <summary>
        /// 获取燃料参数规格数据
        /// </summary>
        /// <param name="fuelType"></param>
        /// <returns></returns>
        public DataTable GetRllxData(string fuelType)
        {
            string sqlQueryParam = string.Format(@"SELECT PARAM_CODE "
                                + " FROM RLLX_PARAM WHERE FUEL_TYPE='{0}' AND STATUS='1'", fuelType);
            System.Data.DataTable dtPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQueryParam, null).Tables[0];

            return dtPam;
        }

        /// <summary>
        /// 校验车型参数单行数据
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <param name="mainType">车型参数的类型：”CTNY“表示传统能源</param>
        /// <returns></returns>
        private string VerifyMainData(DataRow r, DataRow[] dr, string importType, string mainType)
        {
            string message = string.Empty;

            // CPOS
            string cposMsg = string.Empty;
            string cpos = Convert.ToString(r["CPOS"]).Trim();
            cposMsg += this.VerifyCpos(cpos);
            message += cposMsg;

            // 年款
            string yearModeMsg = string.Empty;
            string yearMode = Convert.ToString(r["YEAR_MODE"]);
            yearModeMsg += this.VerifyRequired("年款", yearMode);
            message += yearModeMsg;

            // 产品型号
            string clxhMsg = string.Empty;
            string clxh = Convert.ToString(r["CLXH"]);
            clxhMsg += this.VerifyRequired("产品型号", clxh);
            //clxhMsg += this.VerifyStrLen("产品型号", clxh, 100);
            message += clxhMsg;

            // 发动机排量
            string plMsg = string.Empty;
            string pl = Convert.ToString(r["CT_PL"]);
            plMsg += this.VerifyRequired("发动机排量", pl);
            message += plMsg;

            // 变速箱
            string bsxMsg = string.Empty;
            string bsx = Convert.ToString(r["BSX"]);
            bsxMsg += this.VerifyRequired("变速箱", bsx);
            message += bsxMsg;

            // 乘用车生产企业
            string qcscqyMsg = string.Empty;
            string qcscqy = Convert.ToString(r["QCSCQY"]);
            qcscqyMsg += this.VerifyRequired("乘用车生产企业", qcscqy);
            message += bsxMsg;

            // 校验该条参数是否在数据库中存在
            if (string.IsNullOrEmpty(cposMsg) && string.IsNullOrEmpty(clxhMsg) && string.IsNullOrEmpty(yearModeMsg)
                && string.IsNullOrEmpty(plMsg) && string.IsNullOrEmpty(bsxMsg))
            {
                message += this.VerifyMainParam(cpos, yearMode, clxh, pl, bsx, qcscqy, importType, mainType);
            }

            // 轮胎规格
            string ltggMsg = string.Empty;
            string Ltgg = Convert.ToString(r["LTGG"]);
            ltggMsg += this.VerifyRequired("轮胎规格", Ltgg);
            ltggMsg += this.VerifyStrLen("轮胎规格", Ltgg, 200);
            ltggMsg += this.VerifyLtgg(Ltgg);
            if (string.IsNullOrEmpty(ltggMsg))
            {
                r["LTGG"] = Ltgg.Replace(" ", "");
            }
            message += ltggMsg;
            // 前后轮距相同只填写一个型号数据即可，不同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)

            // string Jkqczjxs = Convert.ToString(r["JKQCZJXS"]);
            string Qcscqy = Convert.ToString(r["QCSCQY"]);

            // 乘用车生产企业
            if (string.IsNullOrEmpty(Qcscqy))
            {
                message += "乘用车生产企业不能为空!\r\n";
            }

            // 车辆类型
            string Clzl = Convert.ToString(r["CLZL"]);
            message += this.VerifyRequired("车辆类型", Clzl);
            if (Clzl.ToUpper().IndexOf("M1") >= 0)
            {
                r["CLZL"] = Clzl = "乘用车（M1）";
            }
            message += this.VerifyClzl(Clzl);
            message += this.VerifyStrLen("车辆类型", Clzl, 200);

            // 燃料类型
            string Rllx = Convert.ToString(r["RLLX"]);
            message += this.VerifyRequired("燃料种类", Rllx);
            message += this.VerifyStrLen("燃料种类", Rllx, 200);
            message += this.VerifyRllx(Rllx);

            // 整备质量
            string Zczbzl = Convert.ToString(r["ZCZBZL"]);
            message += this.VerifyRequired("整备质量", Zczbzl);
            if (!this.VerifyParamFormat(Zczbzl, ','))
            {
                message += "整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格\r\n";
            }

            // 最高车速
            string Zgcs = Convert.ToString(r["ZGCS"]);
            message += this.VerifyRequired("最高车速", Zgcs);
            if (!this.VerifyParamFormat(Zgcs, ','))
            {
                message += "最高车速应填写整数，多个数值应以半角“,”隔开，中间不留空格\r\n";
            }

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

            // 座椅排数
            string Zwps = Convert.ToString(r["ZWPS"]);
            message += this.VerifyRequired("座椅排数", Zwps);
            message += this.VerifyInt("座椅排数", Zwps);

            // 总质量
            string Zdsjzzl = Convert.ToString(r["ZDSJZZL"]);
            string Edzk = Convert.ToString(r["EDZK"]);
            message += this.VerifyRequired("总质量", Zdsjzzl);
            message += this.VerifyZdsjzzl(Zdsjzzl, Zczbzl, Edzk);
            message += this.VerifyInt("总质量", Zdsjzzl);

            // 额定载客
            message += this.VerifyRequired("额定载客", Edzk);
            message += this.VerifyInt("额定载客", Edzk);

            // 前轮距/后轮距
            string Lj = Convert.ToString(r["LJ"]);
            message += this.VerifyRequired("前轮距/后轮距", Lj);
            if (!this.VerifyParamFormat(Lj, '/') && Lj.IndexOf('/') < 0)
            {
                message += "前轮距/后轮距应填写整数，前后轮距，中间用”/”隔开\r\n";
            }

            // 驱动型式 
            string Qdxs = Convert.ToString(r["QDXS"]);
            if (Qdxs.IndexOf('（') >= 0 || Qdxs.IndexOf('）') >= 0)
            {
                r["QDXS"] = Qdxs = Qdxs.Replace('（', '(').Replace('）', ')');
            }
            message += this.VerifyRequired("驱动型式", Qdxs);
            message += this.VerifyQdxs(Qdxs);
            message += this.VerifyStrLen("驱动型式", Qdxs, 200);

            // 报告编号
            string Jybgbh = Convert.ToString(r["JYBGBH"]);
            message += this.VerifyRequired("报告编号", Jybgbh);
            message += this.VerifyStrLen("报告编号", Jybgbh, 500);

            switch (Rllx)
            {
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
                    message += this.VerifyCTNY(r, dr);
                    break;
            }
            if (!string.IsNullOrEmpty(message))
            {
                message = cpos + "--" + yearMode + "--" + clxh + "--" + pl + "--" + bsx + "：\r\n" + message;
            }
            return message;
        }

        /// <summary>
        /// 验证CPOS单行数据
        /// </summary>
        /// <param name="drCpos">CPOS数据</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        private string VerifyCposFuelData(DataRow drCposFuel, string importType)
        {
            string message = string.Empty;
            string cpos = Convert.ToString(drCposFuel["CPOS"]);
            string yearMode = Convert.ToString(drCposFuel["YEAR_MODE"]);
            string clxh = Convert.ToString(drCposFuel["CLXH"]);
            string pl = Convert.ToString(drCposFuel["CT_PL"]);
            string bsx = Convert.ToString(drCposFuel["BSX"]);
            string hgspbm = Convert.ToString(drCposFuel["HGSPBM"]);
            string qcscqy = Convert.ToString(drCposFuel["QCSCQY"]);

            message += this.VerifyRequired("CPOS", cpos);
            message += this.VerifyRequired("年款", yearMode);
            message += this.VerifyRequired("产品型号", clxh);
            message += this.VerifyRequired("发动机排量", pl);
            message += this.VerifyRequired("变速箱", bsx);
            message += this.VerifyRequired("海关商品编码", hgspbm);
            message += this.VerifyRequired("乘用车生产企业", qcscqy);

            // 校验该数据是否已在数据库中存在
            message += this.VerifyIsCposFuelExist(cpos, yearMode, clxh, pl, bsx, qcscqy, importType);

            drCposFuel["CT_SJGKRLXHL"] = this.FormatParam(Convert.ToString(drCposFuel["CT_SJGKRLXHL"]), "1");
            drCposFuel["CT_SQGKRLXHL"] = this.FormatParam(Convert.ToString(drCposFuel["CT_SQGKRLXHL"]), "1");
            drCposFuel["CT_ZHGKRLXHL"] = this.FormatParam(Convert.ToString(drCposFuel["CT_ZHGKRLXHL"]), "1");

            message += VerifyFloat("Fuel consumption(urban condition)", Convert.ToString(drCposFuel["CT_SJGKRLXHL"]));
            message += VerifyFloat("Fuel consumption(extra-urban condition)", Convert.ToString(drCposFuel["CT_SQGKRLXHL"]));
            message += VerifyFloat("Fuel consumption(combined)", Convert.ToString(drCposFuel["CT_ZHGKRLXHL"]));
            message += VerifyInt("CO2排放量（综合）", Convert.ToString(drCposFuel["CT_ZHGKCO2PFL"]));

            return message;
        }

        /// <summary>
        /// 验证VIN单行数据
        /// </summary>
        /// <param name="drCpos">CPOS数据</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        private string VerifyVinData(DataRow drVIN)
        {
            string message = string.Empty;
            string vin = Convert.ToString(drVIN["VIN"]);
            string cpos = Convert.ToString(drVIN["CPOS"]);
            string yearMode = Convert.ToString(drVIN["YEAR_MODE"]);
            string clxh = Convert.ToString(drVIN["CLXH"]);
            string pl = Convert.ToString(drVIN["CT_PL"]);
            string bsx = Convert.ToString(drVIN["BSX"]);
            string jyjgmc = Convert.ToString(drVIN["JYJGMC"]);

            message += this.VerifyRequired("CPOS", cpos);
            message += this.VerifyRequired("年款", yearMode);
            message += this.VerifyRequired("产品型号", clxh);
            message += this.VerifyRequired("发动机排量", pl);
            message += this.VerifyRequired("变速箱", bsx);
            message += this.VerifyRequired("检测机构名称", jyjgmc);
            message += this.VerifyDateTime("进口日期", Convert.ToString(drVIN["CLZZRQ"]));

            return message;
        }

        /// <summary>
        /// 根据导入EXCEL中的VIN 查询油耗数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet ReadSearchExcel(string path, string sheet, string status, string Date)
        {
            DataSet ds = ReadExcel(path, sheet);
            StringBuilder strAdd = new StringBuilder();
            strAdd.Append("SELECT * FROM FC_CLJBXX WHERE VIN IN(");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    strAdd.Append("'");
                    strAdd.Append(Convert.ToString(r["VIN"]));
                    strAdd.Append("',");
                }
                string sql = strAdd.ToString().TrimEnd(',') + ")";
                return AccessHelper.ExecuteDataSet(strCon, sql + " and STATUS='" + status + "'" + Date, null);
            }
            return new DataSet();
        }

        /// <summary>
        /// 批量修改进口日期
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public int ReadUpdateDate(string path, string sheet)
        {
            int result = 0;

            // 获取节假日信息，用于生成上报截止日期
            listHoliday = this.GetHoliday();

            ProcessForm pf = new ProcessForm();

            DataSet ds = ReadExcel(path, sheet);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    using (OleDbTransaction tra = con.BeginTransaction())
                    {
                        try
                        {
                            // 显示进度条
                            pf.Show();
                            int pageSize = 20;
                            int totalVin = ds.Tables[0].Rows.Count;
                            pf.TotalMax = (int)Math.Ceiling((decimal)totalVin / (decimal)pageSize);
                            pf.ShowProcessBar();

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
                                    case (int)Status.修改待上报:
                                        rel = true;
                                        break;
                                    case (int)Status.已上报:
                                        statuswhere = ", STATUS=" + (int)Status.修改待上报;
                                        rel = true;
                                        break;
                                    case (int)Status.撤销待上报:
                                        break;
                                }

                                if (rel)
                                {
                                    DateTime clzzrqDate = Convert.ToDateTime(r[1].ToString());
                                    DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);
                                    string sql = "UPDATE FC_CLJBXX SET CLZZRQ='" + clzzrqDate + "', UPLOADDEADLINE='" + uploadDeadlineDate + "'" + statuswhere + "  where VIN='" + r["VIN"] + "'";
                                    AccessHelper.ExecuteNonQuery(tra, sql, null);
                                    pf.progressBarControl1.PerformStep();
                                    Application.DoEvents();
                                }
                                //if (processCount % pageSize == 0)
                                //{
                                //    pf.progressBarControl1.PerformStep();
                                //    Application.DoEvents();
                                //}
                            }
                            tra.Commit();
                            result = 1;
                        }
                        catch (Exception ex)
                        {
                            tra.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            if (pf != null)
                            {
                                pf.Close();
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="fileName">文件地址</param>
        /// <param name="sheet">名称</param>
        /// <returns></returns>
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
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheet + "]", strConn);
                    oada.Fill(ds);
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

        /// <summary>
        /// 查询油耗数据中VIN状态
        /// </summary>
        /// <param name="vin">VIN码</param>
        /// <returns></returns>
        private int SearchStatus(string vin)
        {
            string sql = "SELECT STATUS FROM FC_CLJBXX WHERE VIN='" + vin + "'";
            return Convert.ToInt32(AccessHelper.ExecuteScalar(strCon, sql, null));
        }

        /// <summary>
        /// 获取节假日数据
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 查询数据上报的截止日期
        /// </summary>
        /// <param name="manufactureDate"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 验证节假日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 括号处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string FormatBrackets(string value)
        {
            return value.Replace('（', '(').Replace('）', ')');
        }

        #region Excel导出

        private Microsoft.Office.Interop.Excel.Application excelApp = null;

        public void ExportExcel(string saveName, DataTable dt)
        {
            excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
            excelApp.Visible = true;

            try
            {
                int rowCount = dt.Rows.Count;
                int colCount = dt.Columns.Count;

                // 表头字段
                Dictionary<string, string> dictHeader = this.FillHeader(dt);

                long pageRows = 65535;//定义每页显示的行数,行数必须小于65536   
                if (rowCount > pageRows)
                {
                    int scount = (int)(rowCount / pageRows);//导出数据生成的表单数   
                    if (scount * pageRows < rowCount)//当总行数不被pageRows整除时，经过四舍五入可能页数不准   
                    {
                        scount = scount + 1;
                    }
                    for (int sc = 1; sc <= scount; sc++)
                    {
                        if (sc > 1)
                        {
                            object missing = System.Reflection.Missing.Value;
                            excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets.Add(
                                        missing, missing, missing, missing);//添加一个sheet   
                        }
                        else
                        {
                            excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets[sc];//取得sheet1   
                        }
                        object[,] datas = new object[pageRows + 1, colCount];

                        for (int i = 0; i < colCount; i++) //写入字段   
                        {
                            datas[0, i] = dictHeader[dt.Columns[i].ColumnName];//表头信息   
                        }

                        int init = int.Parse(((sc - 1) * pageRows).ToString());
                        int r = 0;
                        int index = 0;
                        int result;
                        if (pageRows * sc >= rowCount)
                        {
                            result = (int)rowCount;
                        }
                        else
                        {
                            result = int.Parse((pageRows * sc).ToString());
                        }

                        for (r = init; r < result; r++)
                        {
                            index = index + 1;
                            for (int i = 0; i < colCount; i++)
                            {
                                datas[index, i] = dt.Rows[r][dt.Columns[i].ToString()];
                            }

                        }

                        Microsoft.Office.Interop.Excel.Range fchR = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[index + 1, colCount]];
                        fchR.Value = datas;
                    }
                }
                else
                {
                    object[,] dataArray = new object[rowCount + 1, colCount];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dataArray[0, i] = dictHeader[dt.Columns[i].ColumnName];
                    }

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            dataArray[i + 1, j] = dt.Rows[i][j];
                        }
                    }
                    Microsoft.Office.Interop.Excel.Range range = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowCount + 1, colCount]];
                    range.Value = dataArray;
                }

                excelBook.SaveAs(saveName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.EndReport();
            }
        }

        private Dictionary<string, string> FillHeader(DataTable dt)
        {
            Dictionary<string, string> dictHeader = new Dictionary<string, string>();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                switch (dt.Columns[i].ColumnName)
                {
                    case "VIN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "VIN");
                        break;
                    case "MAIN_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "参数编号");
                        break;
                    case "CLXH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "产品型号");
                        break;
                    case "CLZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆类型");
                        break;
                    case "RLLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料种类");
                        break;
                    case "CLZZRQ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "通关日期");
                        break;
                    case "SQGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料消耗量（市区）");
                        break;
                    case "SJGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料消耗量（市郊）");
                        break;
                    case "ZHGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料消耗量（综合）");
                        break;
                    default: break;
                }
            }
            return dictHeader;
        }

        /// <summary>   
        /// 退出报表时关闭Excel和清理垃圾Excel进程   
        /// </summary>   
        private void EndReport()
        {
            object missing = System.Reflection.Missing.Value;
            try
            {
                excelApp.Workbooks.Close();
                excelApp.Workbooks.Application.Quit();
                excelApp.Application.Quit();
                excelApp.Quit();
            }
            catch { }
            finally
            {
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp.Workbooks);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp.Application);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                }
                catch { }
                try
                {
                    //清理垃圾进程   
                    this.killProcessThread();
                }
                catch { }
                GC.Collect();
            }
        }
        /// <summary>   
        /// 杀掉不死进程   
        /// </summary>   
        private void killProcessThread()
        {
            ArrayList myProcess = new ArrayList();
            for (int i = 0; i < myProcess.Count; i++)
            {
                try
                {
                    System.Diagnostics.Process.GetProcessById(int.Parse((string)myProcess[i])).Kill();
                }
                catch { }
            }
        }

        #endregion

        #region 参数校验

        /// <summary>
        /// 校验车型参数编码是否已经存在
        /// </summary>
        /// <param name="vin8">vin前八位</param>
        /// <param name="ltgg">轮胎规格</param>
        /// <param name="idenParam">特殊车型参数标识</param>
        /// <param name="importType">导入类型：新导入？修改已存在的</param>
        /// <param name="mainType">车型参数表类型：传统能源？非插电式混合动力？</param>
        /// <returns></returns>
        protected string VerifyMainParam(string cpos, string yearMode, string clxh, string pl, string bsx, string qcscqy, string importType, string mainType)
        {
            DataSet ds = this.GetMainId(cpos, yearMode, clxh, pl, bsx, qcscqy, mainType);
            int dataCount = ds.Tables[0].Rows.Count;

            if (importType == "IMPORT")
            {
                if (dataCount > 0)
                {
                    return "该参数编号数据已经导入，请勿重复导入\r\n";
                }
            }
            else if (importType == "UPDATE")
            {
                if (dataCount < 1)
                {
                    return "该参数编号数据不存在\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验油耗数据是否已在数据库中存在
        /// </summary>
        /// <param name="cpos"></param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        protected string VerifyIsCposFuelExist(string cpos, string yearMode, string clxh, string pl, string bsx, string qcscqy, string importType)
        {
            int dataCount = this.GetCposFuelData(cpos, yearMode, clxh, pl, bsx, qcscqy);

            if (importType == "IMPORT")
            {
                if (dataCount > 0)
                {
                    return cpos + "--" + yearMode + "--" + clxh + "--" + pl + "--" + bsx + "：已经导入，请勿重复导入\r\n";
                }
            }
            else if (importType == "UPDATE")
            {
                if (dataCount < 1)
                {
                    return cpos + "--" + yearMode + "--" + clxh + "--" + pl + "--" + bsx + "：数据不存在，请直接导入\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验VIN前8位
        /// </summary>
        /// <param name="vin8"></param>
        /// <returns></returns>
        protected string VerifyCpos(string cpos)
        {
            if (string.IsNullOrEmpty(cpos))
            {
                return "CPOS位不能为空!";
            }
            return "";
        }

        /// <summary>
        /// 校验燃料类型
        /// </summary>
        /// <param name="rllx"></param>
        /// <returns></returns>
        protected string VerifyRllx(string rllx)
        {
            if (!string.IsNullOrEmpty(rllx))
            {
                if (rllx == "汽油" || rllx == "柴油" || rllx == "两用燃料" || rllx == "双燃料" || rllx == "气体燃料" || rllx == "非插电式混合动力" || rllx == "插电式混合动力" || rllx == "纯电动" || rllx == "燃料电池")
                {
                    return string.Empty;
                }
                else
                {
                    return "燃料种类参数填写汽油、柴油、两用燃料、双燃料、气体、纯电动、非插电式混合动力、插电式混合动力、燃料电池\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验轮胎规格
        /// </summary>
        /// <param name="ltgg"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 校验总质量
        /// </summary>
        /// <param name="zdsjzzl"></param>
        /// <param name="zczbzl"></param>
        /// <param name="edzk"></param>
        /// <returns></returns>
        protected string VerifyZdsjzzl(string zdsjzzl, string zczbzl, string edzk)
        {
            if (!string.IsNullOrEmpty(zdsjzzl) && !string.IsNullOrEmpty(zczbzl) && !string.IsNullOrEmpty(edzk))
            {
                if (Convert.ToInt32(zdsjzzl) < (Convert.ToInt32(zczbzl) + Convert.ToInt32(edzk) * 65))
                {
                    return "总质量应≥整备质量＋乘员质量（额定载客×乘客质量，乘用车按65㎏/人核算)!\r\n";
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验车辆类型
        /// </summary>
        /// <param name="clzl"></param>
        /// <returns></returns>
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
                    return "车辆类型参数应填写“乘用车（M1）/轻型客车（M2）/轻型货车（N1）”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验越野车
        /// </summary>
        /// <param name="yyc"></param>
        /// <returns></returns>
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
                    return "越野车(G类)参数应填写“是/否”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验驱动型式
        /// </summary>
        /// <param name="qdxs"></param>
        /// <returns></returns>
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
                    return "驱动型式参数应填写“前轮驱动/后轮驱动/分时全轮驱动/全时全轮驱动/智能(适时)全轮驱动”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验变速器型式
        /// </summary>
        /// <param name="bsqxs"></param>
        /// <returns></returns>
        protected string VerifyBsqxs(string bsqxs)
        {
            if (!string.IsNullOrEmpty(bsqxs))
            {
                if (bsqxs.Contains("MT") || bsqxs.Contains("AT") || bsqxs.Contains("AMT") || bsqxs.Contains("CVT") || bsqxs.Contains("DCT") || bsqxs.Contains("其它"))
                {
                    return string.Empty;
                }
                else
                {
                    return "变速器型式参数应填写“MT/AT/AMT/CVT/DCT/其它”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验变速器档位数
        /// </summary>
        /// <param name="bsqdws"></param>
        /// <returns></returns>
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
                    return "变速器档位数参数应填写“1/2/3/4/5/6/7/8/9/10/N.A”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验混合动力结构型式
        /// </summary>
        /// <param name="hhdljgxs"></param>
        /// <returns></returns>
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
                    return "混合动力结构型式参数应填写“串联/并联/混联/其它”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 是否具有行驶模式手动选择功能
        /// </summary>
        /// <param name="sdxzgn"></param>
        /// <returns></returns>
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
                    return "是否具有行驶模式手动选择功能参数应填写“是/否”\r\n";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 校验电动汽车储能装置种类
        /// </summary>
        /// <param name="dlxdczzl"></param>
        /// <returns></returns>
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
                    return "电动汽车储能装置种类参数应填写“铅酸电池/金属氢化物镍电池/锂电池/超级电容/其它”\r\n";
                }
            }
            return string.Empty;
        }

        #endregion

        #region 燃料类型校验

        /// <summary>
        /// 校验传统能源参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected string VerifyCTNY(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;

            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);

                    // 需求变更后，以下几项与基本参数分开导入
                    if (code == "CT_SJGKRLXHL" || code == "CT_SQGKRLXHL" || code == "CT_ZHGKCO2PFL" || code == "CT_ZHGKRLXHL")
                    {
                        continue;
                    }

                    // 转换数据格式
                    if (PARAMFLOAT1.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    }
                    if (PARAMFLOAT2.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    }

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
                            message += VerifyBsqxs(Convert.ToString(r["BSX"]));
                            break;
                        case "CT_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }

                    // 校验必填字段
                    if (code != "CT_JGL" && code != "CT_QTXX")
                    {
                        if (code == "CT_BSQXS")
                        {
                            code = "BSX";
                        }
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

        /// <summary>
        /// 校验纯电动参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected string VerifyCDD(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);

                    // 转换数据格式
                    if (PARAMFLOAT1.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    }
                    if (PARAMFLOAT2.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    }

                    switch (code)
                    {
                        case "CDD_DLXDCBNL":
                            message += VerifyFloat("动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZEDNL":
                            message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                            break;
                        case "CDD_DDXDCZZLYZCZBZLDBZ":
                            message += VerifyFloat("储能装置总成质量与整备质量的比值", Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZBCDY":
                            message += VerifyInt("储能装置总成标称电压", Convert.ToString(r[code]));
                            break;
                        case "CDD_DDQC30FZZGCS":
                            message += VerifyInt("30分钟最高车速", Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKXSLC":
                            message += VerifyFloat("电动汽车续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKDNXHL":
                            message += VerifyInt("工况条件下百公里耗电量", Convert.ToString(r[code]));
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

        /// <summary>
        /// 校验混合动力参数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected string VerifyHHDL(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);

                    // 转换数据格式
                    if (PARAMFLOAT1.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    }
                    if (PARAMFLOAT2.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    }

                    switch (code)
                    {
                        case "FCDS_HHDL_DLXDCBNL":
                            message += VerifyFloat("动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_EDGL":
                            message += VerifyFloat("发动机功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_JGL":
                            message += VerifyFloat("发动机净功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_PL":
                            message += VerifyInt("发动机排量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt("CO2排放量（综合）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt("储能装置总成标称电压", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SJGKRLXHL":
                            message += VerifyFloat("燃料消耗量（市郊）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SQGKRLXHL":
                            message += VerifyFloat("燃料消耗量（市区）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt("纯电动模式下1km最高车速", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyFloat("纯电驱动模式续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2("混合动力汽车电功率比", Convert.ToString(r[code]));
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
                            message += VerifyFloat("动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKDNXHL":
                            message += VerifyInt("工况条件下百公里耗电量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyFloat("纯电驱动模式续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt("纯电动模式下1km最高车速", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_EDGL":
                            message += VerifyFloat("发动机功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_JGL":
                            message += VerifyFloat("发动机净功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_PL":
                            message += VerifyInt("发动机排量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt("CO2排放量（综合）", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt("储能装置总成标称电压", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2("混合动力汽车电功率比", Convert.ToString(r[code]));
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

                    // 校验必填字段
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

        /// <summary>
        /// 校验燃料电池参数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        protected string VerifyRLDC(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);

                    // 转换数据格式
                    if (PARAMFLOAT1.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    }
                    if (PARAMFLOAT2.Contains(code))
                    {
                        r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    }

                    switch (code)
                    {
                        case "RLDC_DDGLMD":
                            message += VerifyFloat("燃料电池堆功率密度", Convert.ToString(r[code]));
                            break;
                        case "RLDC_DDHHJSTJXXDCZBNL":
                            message += VerifyFloat("电电混合技术条件下动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKHQL":
                            message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKXSLC":
                            message += VerifyFloat("电动汽车续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CDDMSXZGXSCS":
                            message += VerifyInt("30分钟最高车速", Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPBCGZYL":
                            message += VerifyInt("燃料电池汽车气瓶公称工作压力", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPRJ":
                            message += VerifyInt("燃料电池汽车气瓶公称水容积", Convert.ToString(r[code]));
                            break;
                        case "RLDC_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }

                    // 校验必填字段
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

        #region 输入校验

        protected string FormatParam(string obj, string strFormat)
        {
            string msg = string.Empty;
            try
            {
                if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                {
                    if (Regex.IsMatch(obj.ToString(), "\\d+(.\\d+)?$") && strFormat == "1")
                    {
                        obj = (double.Parse(obj)).ToString("0.0");
                    }
                    if (Regex.IsMatch(obj.ToString(), "\\d+(.\\d+)?$") && strFormat == "2")
                    {
                        obj = (double.Parse(obj)).ToString("0.00");
                    }
                }
            }
            catch (Exception)
            {
            }
            return obj;
        }

        /// <summary>
        /// 校验不为空
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string VerifyRequired(string strName, string value)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                msg = strName + "不能为空!\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 校验字符长度
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        /// <param name="expectedLen"></param>
        /// <returns></returns>
        protected string VerifyStrLen(string strName, string value, int expectedLen)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > expectedLen)
                {
                    msg = strName + "长度过长，最长为" + expectedLen + "位!\r\n";
                }
            }
            return msg;
        }

        /// <summary>
        /// 校验验证整型
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string VerifyInt(string strName, string value)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value.ToString(), "^[0-9]*$"))
            {
                msg = strName + "应为整数!\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 校验浮点型1位小数
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string VerifyFloat(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
            {
                msg = strName + "应保留1位小数!\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 验证浮点型两位小数
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string VerifyFloat2(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{2}$"))
            {
                msg = strName + "应保留2位小数!\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 校验时间类型
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string VerifyDateTime(string strName, string value)
        {
            string msg = string.Empty;
            try
            {
                if (value != null)
                {
                    DateTime time = Convert.ToDateTime(value);
                }
                else
                {
                    msg = strName + "不能为空!\r\n";
                }
            }
            catch (Exception)
            {
                msg = strName + "应为时间类型!\r\n";
            }
            return msg;
        }

        /// <summary>
        /// 校验参数格式，多个数值以参数c隔开，中间不能有空格
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
    }
}
