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
using System.Globalization;
using System.Linq;
using FuelDataSysClient.Tool;
using DAO = Microsoft.Office.Interop.Access.Dao;

namespace FuelDataSysClient.Tool
{
    public enum Status
    {
        已上报 = 0,
        待上报 = 1,
        修改待上报 = 2,
        撤销待上报 = 3,
        未被激活 = 9
        //（9：未被激活（数据通过excel导入但未被激活）；0：已上传；1：没上传；2：修改没上传；3：撤销未上传）
    }

    public class MitsUtils
    {
        public static string CTNY = "传统能源";
        public static string FCDSHHDL = "非插电式混合动力";
        public static string CDSHHDL = "插电式混合动力";
        public static string CDD = "纯电动";
        public static string RLDC = "燃料电池";

        public static Dictionary<string, string> dictRllx = new Dictionary<string,string>();

        private const string VIN = "VIN";
        private List<string> PARAMFLOAT1 = new List<string>() { "CT_EDGL", "CT_JGL", "CT_SJGKRLXHL", "CT_SQGKRLXHL", "CT_ZHGKRLXHL", "CDD_DLXDCZEDNL", "CDD_QDDJEDGL", "FCDS_HHDL_DLXDCZZNL", "FCDS_HHDL_ZHGKRLXHL", "FCDS_HHDL_EDGL", "FCDS_HHDL_JGL", "FCDS_HHDL_SJGKRLXHL", "FCDS_HHDL_SQGKRLXHL", "FCDS_HHDL_QDDJEDGL", "CDS_HHDL_DLXDCZZNL", "CDS_HHDL_ZHGKRLXHL", "CDS_HHDL_QDDJEDGL", "CDS_HHDL_EDGL", "CDS_HHDL_JGL", "CDS_HHDL_TJBSYZDNXHL", "RLDC_DDGLMD", "RLDC_ZHGKHQL", "RLDC_QDDJEDGL", "RLDC_RLDCXTEDGL" };
        private List<string> PARAMFLOAT2 = new List<string>() { "CDD_ZHGKDNXHL", "FCDS_HHDL_HHDLZDDGLB", "CDS_HHDL_ZHGKDNXHL", "CDS_HHDL_HHDLZDDGLB", "CDS_HHDL_TJASYZDNXHL" };

        private string strCon = AccessHelper.conn;
        public DataTable checkData = new DataTable();
        public Dictionary<string, string> dictCTNY;  //存放列头转换模板(传统能源)
        public Dictionary<string, string> dictFCDSHHDL;  //存放列头转换模板（非插电式混合动力）
        public Dictionary<string, string> dictCDSHHDL;  //存放列头转换模板（插电式混合动力）
        public Dictionary<string, string> dictCDD;  //存放列头转换模板（纯电动）
        public Dictionary<string, string> dictRLDC;  //存放列头转换模板（燃料电池）
        public Dictionary<string, string> dictVin; //存放列头转换模板（VIN）

        DataTable dtCtnyStatic;
        //DataTable dtFcdsStatic;
        public Dictionary<string, DataTable> dsMainStatic = new Dictionary<string,DataTable>();
        DataTable excelDT;

        private List<string> listHoliday; // 节假日数据

        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate"];
        private static NameValueCollection FILE_NAME = (NameValueCollection)ConfigurationManager.GetSection("fileName");

        static MitsUtils()
        {
            dictRllx.Add("CTNY", CTNY);
            dictRllx.Add("FCDS", FCDSHHDL);
            dictRllx.Add("CDS", CDSHHDL);
            dictRllx.Add("CDD", CDD);
            dictRllx.Add("RLDC", RLDC);
        }

        public MitsUtils()
        {
            checkData = GetCheckData();    //获取参数数据  RLLX_PARAM  

            excelDT = this.ReadExcel(path, "").Tables[0]; //读取表头转置模板

            ReadTemplate(path);   //读取表头转置模板
        }

        // VIN excel文件名称的开头
        private string vinFileName = FILE_NAME["VIN"].ToString();

        public string VinFileName
        {
            get { return vinFileName; }
        }

        // 主表Excel文件名称的开头
        private string mainFileName =  FILE_NAME["MAIN"].ToString();

        public string MainFileName
        {
            get { return mainFileName; }
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
        /// 读取VIN信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet ReadVinCsv(bool HeadYes, char span, string fileName)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            StreamReader fileReader = new StreamReader(fileName, Encoding.UTF8);
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
                    if (y.Length == 4) continue;
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
                string shortFileName = Path.GetFileNameWithoutExtension(srcFileName) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(srcFileName);
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
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="mainId"></param>
        /// <param name="paramId"></param>
        /// <param name="paramName"></param>
        /// <param name="IsExist"></param>
        /// <returns></returns>
        public DataRow GetDivideMain(DataTable dt, string vin, string clxh, string paramName, ref string message)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (clxh==Convert.ToString(dr["CLXH"]).Trim())
                {
                    message += "";
                    return dr;
                }
            }
            switch (paramName)
            {
                case "传统能源":
                    message += string.Format("\r\n{0}: 对应车型参数“{1}”不存在", vin, clxh);
                    break;
                default: break;
            }
            return null;
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
        public DataRow GetDivideMain(DataTable dt, string vin, string clxh, string uniqueCode, string paramName, ref string message)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (uniqueCode == Convert.ToString(dr["UNIQUE_CODE"]).Trim())
                {
                    message += "";
                    return dr;
                }
            }
            //switch (paramName)
            //{
            //    case "传统能源":
                    message += string.Format("\r\n{0}: 对应车型参数“{1}”不存在", vin, uniqueCode);
            //        break;
            //    default: break;
            //}
            return null;
        }

        /// <summary>
        /// 保存VIN信息
        /// </summary>
        /// <param name="ds"></param>
        public string SaveVinInfo(DataTable dt)
        {
            int succFuelCount = 0; //生成油耗数据的数量
            int succImCount = 0;   //成功导入的数量
            int failCount = 0;  //导入失败的数量
            int totalCount = dt.Rows.Count;
            string msg = string.Empty;

            ProcessForm pf = new ProcessForm();
            try
            {
                DataTable dtCtnyPam = this.GetRllxData(CTNY);
                DataTable dtFcdsPam = this.GetRllxData(FCDSHHDL);
                DataTable dtCdsPam = this.GetRllxData(CDSHHDL);
                DataTable dtCddPam = this.GetRllxData(CDD);
                DataTable dtRldcPam = this.GetRllxData(RLDC);

                Dictionary<string, DataTable> dicDtPam = new Dictionary<string, DataTable>();
                dicDtPam.Add(CTNY, dtCtnyPam);
                dicDtPam.Add(FCDSHHDL, dtFcdsPam);
                dicDtPam.Add(CDSHHDL, dtCdsPam);
                dicDtPam.Add(CDD, dtCddPam);
                dicDtPam.Add(RLDC, dtRldcPam);

                // 获取节假日数据
                listHoliday = this.GetHoliday();

                // 显示进度条
                pf.Show();
                int pageSize = 1;
                int totalVin = totalCount;
                int count = 0;

                pf.TotalMax = (int)Math.Ceiling((decimal)totalVin / (decimal)pageSize);
                pf.ShowProcessBar();

                foreach (DataRow drVin in dt.Rows)
                {
                    count++;
                    string vin = drVin["VIN"] == null ? "" : drVin["VIN"].ToString().Trim();
                    string clxh = drVin["CLXH"] == null ? "" : drVin["CLXH"].ToString().Trim();
                    string rllx = drVin["RLLX"] == null ? "" : drVin["RLLX"].ToString().Trim();
                    string uniqueCode = drVin["UNIQUE_CODE"] == null ? "" : drVin["UNIQUE_CODE"].ToString().Trim();
                    if (!string.IsNullOrEmpty(uniqueCode))
                    {
                        string vinMsg = this.VerifyVinData(drVin);

                        if (string.IsNullOrEmpty(vinMsg))
                        {
                            string ctnyMsg = string.Empty;

                            //DataRow drCtny = this.GetDivideMain(dtCtnyStatic, vin, clxh, CTNY, ref ctnyMsg);
                            DataRow drCtny = this.GetDivideMain(dsMainStatic[rllx], vin, clxh, uniqueCode, rllx, ref ctnyMsg);

                            if (!string.IsNullOrEmpty(ctnyMsg))
                            {
                                vinMsg += ctnyMsg;
                            }
                            
                            if (string.IsNullOrEmpty(vinMsg))
                            {
                                if (string.IsNullOrEmpty(ctnyMsg))
                                {
                                    vinMsg += this.SaveReadyData(drVin, drCtny, dicDtPam[rllx]);
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

                        //if (count % 20 == 0 || count == totalCount)
                        //{
                            pf.progressBarControl1.PerformStep();
                            Application.DoEvents();
                        //}
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

                    OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Today);
                    creTime.OleDbType = OleDbType.DBDate;
                    DateTime clzzrqDate;

                    try
                    {
                        clzzrqDate = DateTime.ParseExact(drVin["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception)
                    {
                        clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"]);
                    }
                    
                    OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                    clzzrq.OleDbType = OleDbType.DBDate;

                    #region 保存VIN信息备用

                    string sqlDel = "DELETE FROM VIN_INFO WHERE VIN = '" + vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                    string sqlStr = @"INSERT INTO VIN_INFO(VIN,CLXH,CLZZRQ,STATUS,CREATETIME,RLLX,UNIQUE_CODE) Values (@VIN, @CLXH,@CLZZRQ,@STATUS,@CREATETIME,@RLLX,@UNIQUE_CODE)";
                    OleDbParameter[] vinParamList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@CLXH",drVin["CLXH"].ToString().Trim()),
                                         clzzrq,
                                         new OleDbParameter("@STATUS","1"),
                                         creTime,
                                         new OleDbParameter("@RLLX",drVin["RLLX"].ToString().Trim()),
                                         new OleDbParameter("@UNIQUE_CODE",drVin["UNIQUE_CODE"].ToString().Trim())
                                      };
                    AccessHelper.ExecuteNonQuery(tra, sqlStr, vinParamList);

                    tra.Commit();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                genMsg += ex.Message + "\r\n";
                tra.Rollback();
            }
            finally
            {
            }

            return genMsg;
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
            DataSet ds = this.ReadExcel(fileName, "");
   
            if (ds != null)
            {
                DataTable daVin = D2D(dictVin, ds.Tables[0], VIN);
                rtnMsg += this.SaveVinInfo(daVin);

                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_VIN");
                }

                if (!string.IsNullOrEmpty(rtnMsg))
                {
                    rtnMsg = Path.GetFileName(fileName) + ":\r\n" + rtnMsg + "\r\n";
                }
                else
                {
                    rtnMsg = Path.GetFileName(fileName) + ":\r\n导入成功\r\n";
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
        }


        #region ImportVinData  方法 
        //string rtnMsg = string.Empty;
        //    DataSet ds = this.ReadVinCsv(false, ',', fileName);  //读取CSV
        //    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataTable dt = DataFormat(ds.Tables[0]);         //格式化数据
        //        rtnMsg += this.SaveData(ConvertTableHeader(dt));


        //        rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_VIN");

        //        if (!string.IsNullOrEmpty(rtnMsg))
        //        {
        //            rtnMsg = Path.GetFileName(fileName) + ":\r\n" + rtnMsg + "\r\n";
        //        }
        //        else
        //        {
        //            rtnMsg = Path.GetFileName(fileName) + ":\r\n导入成功\r\n";
        //        }
        //    }
        //    else
        //    {
        //        rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
        //    }

        //    return rtnMsg;
        #endregion

        #region 转换表头

        /// <summary>
        /// 转换表头
        /// </summary>
        /// <param name="dt">datatable</param>
        /// <returns></returns>
        private DataTable ConvertTableHeader(DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                foreach (DataRow r in excelDT.Rows)
                {
                    if (dc.ColumnName == Convert.ToString(r[0]))
                    {
                        dc.ColumnName = Convert.ToString(r[1]);
                        break;
                    }
                }
            }


            return dt;
        }

        #endregion

        #region 格式化数据

        /// <summary>
        /// 把TABLE参数数据和明细数据格式化成一条 
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <returns></returns>
        private DataTable DataFormat(DataTable dataSource)
        {
            DataTable newDt = new DataTable();
            int rowNumber = 0;
            try
            {
                newDt = dataSource.Clone();

                DataRow newdr = null;
                for (int i = 0; i < dataSource.Rows.Count; i++)
                {
                    if ("P" == Convert.ToString(dataSource.Rows[i][0]))   //保留参数 数据
                    {
                        newdr = dataSource.Rows[i];
                        rowNumber++;
                    }
                    else if ("C" == Convert.ToString(dataSource.Rows[i][0]))
                    {
                        newDt.Rows.Add(dataSource.Rows[i].ItemArray);  //插入保留参数
                        for (int j = 0; j < dataSource.Columns.Count - 1; j++)  //循环插入明细
                        {
                            if (j == dataSource.Columns.Count - 10) break;
                            newDt.Rows[i - rowNumber][j + 10] = Convert.ToString(newdr[j]);
                        }
                    }
                }
            }
            catch { newDt = null; }
            return newDt;
        }

        #endregion

        #region 保存数据
        private string SaveData(DataTable dt)
        {
            DataTable dtCtnyPam = this.GetRllxData("传统能源");
            string msg = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();
            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

            try
            {
                DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + CTNY + "' and STATUS=1");
                if (dt != null && dt.Rows.Count > 0)
                {
                    string error = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {

                        string vin = dr["VIN"].ToString().Trim().ToUpper();

                        // 如果当前vin数据已经存在，则跳过
                        if (this.IsFuelDataExist(vin))
                        {
                            msg += vin + "已经存在。\r\n";
                            continue;
                        }

                        error = VerifyData(dr, drCtny, "IMPORT");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region 插入主表
                            string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME
                                ) VALUES
                                (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@HGSPBM,@QTXX,@STATUS,@CREATETIME,@UPDATETIME)";

                            DateTime clzzrqDate = Convert.ToDateTime(dr["CLZZRQ"].ToString().Trim());
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
                                     new OleDbParameter("@USER_ID",Utils.userId),
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
                                     new OleDbParameter("@YYC",dr["YYC"].ToString().Trim()=="1"?"是":"否"),
                                     new OleDbParameter("@ZWPS",dr["ZWPS"].ToString().Trim()),
                                     new OleDbParameter("@ZDSJZZL",dr["ZDSJZZL"].ToString().Trim()),
                                     new OleDbParameter("@EDZK",dr["EDZK"].ToString().Trim()),
                                     new OleDbParameter("@LJ",dr["LJ"].ToString().Trim()),
                                     new OleDbParameter("@QDXS",dr["QDXS"].ToString().Trim()),
                                     new OleDbParameter("@JYJGMC",dr["JYJGMC"].ToString().Trim()),
                                     new OleDbParameter("@JYBGBH",dr["JYBGBH"].ToString().Trim()),
                                     new OleDbParameter("@HGSPBM",dr["HGSPBM"].ToString().Trim()),
                                     new OleDbParameter("@QTXX",dr["CT_QTXX"].ToString().Trim()),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","1"),
                                     creTime,
                                     upTime
                                     };
                            AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                            #endregion

                            #region 插入参数信息

                            string sqlDelParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + vin + "'";
                            AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                            // 待生成的燃料参数信息存入燃料参数表
                            foreach (DataRow drParam in dtCtnyPam.Rows)
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
                        }

                    }
                    tra.Commit();
                }
            }
            catch { tra.Rollback(); }
            finally { con.Close(); }


            return msg;
        }
        #endregion

        //得到选中数据
        private string GetUploadData(DataTable dt)
        {
            string vinStr = string.Empty;
            int count = 0;
            try
            {
                if (dt != null)
                {
                    DataRow[] drVinArr = dt.Select("check=True");

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if ((bool)dt.Rows[i]["check"])
                        {
                            count++;
                            vinStr += string.Format(",'{0}'", dt.Rows[i]["VIN"].ToString());
                            if (count % 50 == 0)
                            {
                                vinStr += ";";
                            }
                        }
                    }
                    //if (!string.IsNullOrEmpty(vinStr))
                    //{
                    //    vinStr = vinStr.Substring(1);
                    //}
                }
            }
            catch { }
            return vinStr;
        }

        //批量更新V_ID
        private bool UpdateV_ID(DataTable dt)
        {
            string sql = "update FC_CLJBXX set V_ID='{0}',STATUS='0' where VIN='{1}'";
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();
            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
            try
            {

                foreach (DataRow dr in dt.Rows)
                {
                    AccessHelper.ExecuteNonQuery(tra, string.Format(sql, dr[1], dr[0]));//dr[0]:VIN;dr[1]:V_ID
                }
                tra.Commit();
                return true;
            }
            catch { tra.Rollback(); }
            finally { con.Close(); }
            return false;
        }

        FuelDataService.FuelDataSysWebService service = Utils.service;

        public bool ActionUpdate(GridView gv, DataTable dt)
        {
            bool flag = true;
            ProcessForm pf = new ProcessForm();
            pf.Text = "正在同步，请稍候";

            try
            {
                gv.PostEditor();

                string strVin = this.GetUploadData(dt);
                string[] arrVin = strVin.Split(';');

                pf.Show();

                pf.TotalMax = arrVin.Length;
                pf.ShowProcessBar();

                foreach (string vins in arrVin)
                {
                    string vin = string.Empty;
                    if (!string.IsNullOrEmpty(vins))
                    {
                        vin = vins.Substring(1);

                        DataSet tempDt = service.QueryVidByVins(Settings.Default.UserId, Settings.Default.UserPWD, vin);
                        if (tempDt != null)
                        {
                            if (tempDt.Tables[0].Rows.Count > 0)
                                flag = flag && UpdateV_ID(tempDt.Tables[0]);
                        }
                        pf.progressBarControl1.PerformStep();
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (pf != null)
                {
                    pf.Close();
                }
            }
            return flag;
        }


        /// <summary>
        /// 导入主表信息信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public string ImportMainData(string fileName, string folderName, string importType, List<string> mainUpdateList)
        {
            string rtnMsg = string.Empty;
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
                    rtnMsg += this.UpdateMainData2(ds, mainUpdateList);
                }

                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_MAIN");
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
        /// 读主表信息
        /// </summary>
        /// <param name="fileName"></param>
        public DataSet ReadMainExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("select * from [传统能源$]", strConn);
                oada.Fill(ds, CTNY);

                oada = new OleDbDataAdapter("select * from [非插电式混合动力$]", strConn);
                oada.Fill(ds, FCDSHHDL);

                oada = new OleDbDataAdapter("select * from [插电式混合动力$]", strConn);
                oada.Fill(ds, CDSHHDL);

                oada = new OleDbDataAdapter("select * from [纯电动$]", strConn);
                oada.Fill(ds, CDD);

                oada = new OleDbDataAdapter("select * from [燃料电池$]", strConn);
                oada.Fill(ds, RLDC);

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
        /// 读表头对应关系模板
        /// </summary>
        /// <param name="fileName"></param>
        public DataSet ReadTemplateExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("select * from [传统能源$]", strConn);
                oada.Fill(ds, CTNY);

                oada = new OleDbDataAdapter("select * from [非插电式混合动力$]", strConn);
                oada.Fill(ds, FCDSHHDL);

                oada = new OleDbDataAdapter("select * from [插电式混合动力$]", strConn);
                oada.Fill(ds, CDSHHDL);

                oada = new OleDbDataAdapter("select * from [纯电动$]", strConn);
                oada.Fill(ds, CDD);

                oada = new OleDbDataAdapter("select * from [燃料电池$]", strConn);
                oada.Fill(ds, RLDC);

                oada = new OleDbDataAdapter("select * from [VIN$]", strConn);
                oada.Fill(ds, VIN);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 导入主表信息
        /// </summary>
        /// <param name="basicInfo"></param>
        /// <param name="ctnyInfo"></param>
        public string SaveMainData(DataSet ds)
        {
            int succCount = 0;
            int totalCount = 0;
            string msg = string.Empty;

            try
            {
                // 转换表头（用户模板中的表头转为数据库列名）
                DataTable dtCtny = D2D(dictCTNY, ds.Tables[CTNY], CTNY);
                totalCount += dtCtny.Rows.Count;
                succCount += ImpMainData_DAO(dtCtny, CTNY, ref msg);

                DataTable dtFcds = D2D(dictFCDSHHDL, ds.Tables[FCDSHHDL], FCDSHHDL);
                totalCount += dtFcds.Rows.Count;
                succCount += ImpMainData_DAO(dtFcds, FCDSHHDL, ref msg);

                DataTable dtCds = D2D(dictCDSHHDL, ds.Tables[CDSHHDL], CDSHHDL);
                totalCount += dtCds.Rows.Count;
                succCount += ImpMainData_DAO(dtCds, CDSHHDL, ref msg);

                DataTable dtCdd = D2D(dictCDD, ds.Tables[CDD], CDD);
                totalCount += dtCdd.Rows.Count;
                succCount += ImpMainData_DAO(dtCdd, CDD, ref msg);

                DataTable dtRldc = D2D(dictRLDC, ds.Tables[RLDC], RLDC);
                totalCount += dtRldc.Rows.Count;
                succCount += ImpMainData_DAO(dtRldc, RLDC, ref msg);

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
        public int ImpMainData_DAO(DataTable dt, string rlzl, ref string msg)
        {
            int succCount = 0;
            if (string.IsNullOrEmpty(msg))
            {
                msg = string.Empty;
            }

            try
            {
                DAO.DBEngine dbEngine = new DAO.DBEngine();
                DAO.Database db = dbEngine.OpenDatabase(Utils.dataPath);
               

                // 转换表头（用户模板中的表头转为数据库列名）
                DataRow[] tdr = checkData.Select("FUEL_TYPE='" + rlzl + "' and STATUS=1");

                if (dt != null && dt.Rows.Count > 0)
                {
                    //初始化
                    #region 传统能源
                    DAO.Recordset rs_CTNY = db.OpenRecordset("CTNY_MAIN");
                    DAO.Field[] myFields = new DAO.Field[34];
                    myFields[0] = rs_CTNY.Fields["UNIQUE_CODE"];
                    myFields[1] = rs_CTNY.Fields["QCSCQY"];
                    myFields[2] = rs_CTNY.Fields["JYBGBH"];
                    myFields[3] = rs_CTNY.Fields["JKQCZJXS"];
                    myFields[4] = rs_CTNY.Fields["CLXH"];
                    myFields[5] = rs_CTNY.Fields["HGSPBM"];
                    myFields[6] = rs_CTNY.Fields["CLZL"];
                    myFields[7] = rs_CTNY.Fields["YYC"];
                    myFields[8] = rs_CTNY.Fields["QDXS"];
                    myFields[9] = rs_CTNY.Fields["ZWPS"];
                    myFields[10] = rs_CTNY.Fields["ZCZBZL"];
                    myFields[11] = rs_CTNY.Fields["ZDSJZZL"];
                    myFields[12] = rs_CTNY.Fields["ZGCS"];
                    myFields[13] = rs_CTNY.Fields["EDZK"];
                    myFields[14] = rs_CTNY.Fields["LTGG"];
                    myFields[15] = rs_CTNY.Fields["LJ"];
                    myFields[16] = rs_CTNY.Fields["JYJGMC"];
                    myFields[17] = rs_CTNY.Fields["TYMC"];
                    myFields[18] = rs_CTNY.Fields["ZJ"];
                    myFields[19] = rs_CTNY.Fields["RLLX"];
                    myFields[20] = rs_CTNY.Fields["CT_BSQXS"];
                    myFields[21] = rs_CTNY.Fields["CT_EDGL"];
                    myFields[22] = rs_CTNY.Fields["CT_FDJXH"];
                    myFields[23] = rs_CTNY.Fields["CT_JGL"];
                    myFields[24] = rs_CTNY.Fields["CT_PL"];
                    myFields[25] = rs_CTNY.Fields["CT_QGS"];
                    myFields[26] = rs_CTNY.Fields["CT_QTXX"];
                    myFields[27] = rs_CTNY.Fields["CT_SJGKRLXHL"];
                    myFields[28] = rs_CTNY.Fields["CT_SQGKRLXHL"];
                    myFields[29] = rs_CTNY.Fields["CT_ZHGKCO2PFL"];
                    myFields[30] = rs_CTNY.Fields["CT_ZHGKRLXHL"];
                    myFields[31] = rs_CTNY.Fields["CT_BSQDWS"];
                    myFields[32] = rs_CTNY.Fields["CREATETIME"];
                    myFields[33] = rs_CTNY.Fields["UPDATETIME"];
                    #endregion

                    #region 非插电式混合动力
                    DAO.Recordset rs_FCDS = db.OpenRecordset("FCDS_MAIN");
                    DAO.Field[] myFields_FCDS = new DAO.Field[45];
                    myFields_FCDS[0] = rs_FCDS.Fields["UNIQUE_CODE"];
                    myFields_FCDS[1] = rs_FCDS.Fields["QCSCQY"];
                    myFields_FCDS[2] = rs_FCDS.Fields["JYBGBH"];
                    myFields_FCDS[3] = rs_FCDS.Fields["JKQCZJXS"];
                    myFields_FCDS[4] = rs_FCDS.Fields["CLXH"];
                    myFields_FCDS[5] = rs_FCDS.Fields["HGSPBM"];
                    myFields_FCDS[6] = rs_FCDS.Fields["CLZL"];
                    myFields_FCDS[7] = rs_FCDS.Fields["YYC"];
                    myFields_FCDS[8] = rs_FCDS.Fields["QDXS"];
                    myFields_FCDS[9] = rs_FCDS.Fields["ZWPS"];
                    myFields_FCDS[10] = rs_FCDS.Fields["ZCZBZL"];
                    myFields_FCDS[11] = rs_FCDS.Fields["ZDSJZZL"];
                    myFields_FCDS[12] = rs_FCDS.Fields["ZGCS"];
                    myFields_FCDS[13] = rs_FCDS.Fields["EDZK"];
                    myFields_FCDS[14] = rs_FCDS.Fields["LTGG"];
                    myFields_FCDS[15] = rs_FCDS.Fields["LJ"];
                    myFields_FCDS[16] = rs_FCDS.Fields["JYJGMC"];
                    myFields_FCDS[17] = rs_FCDS.Fields["TYMC"];
                    myFields_FCDS[18] = rs_FCDS.Fields["ZJ"];
                    myFields_FCDS[19] = rs_FCDS.Fields["RLLX"];
                    myFields_FCDS[20] = rs_FCDS.Fields["FCDS_HHDL_BSQDWS"];
                    myFields_FCDS[21] = rs_FCDS.Fields["FCDS_HHDL_BSQXS"];
                    myFields_FCDS[22] = rs_FCDS.Fields["FCDS_HHDL_CDDMSXZGCS"];
                    myFields_FCDS[23] = rs_FCDS.Fields["FCDS_HHDL_CDDMSXZHGKXSLC"];
                    myFields_FCDS[24] = rs_FCDS.Fields["FCDS_HHDL_DLXDCBNL"];
                    myFields_FCDS[25] = rs_FCDS.Fields["FCDS_HHDL_DLXDCZBCDY"];
                    myFields_FCDS[26] = rs_FCDS.Fields["FCDS_HHDL_DLXDCZZL"];
                    myFields_FCDS[27] = rs_FCDS.Fields["FCDS_HHDL_DLXDCZZNL"];
                    myFields_FCDS[28] = rs_FCDS.Fields["FCDS_HHDL_EDGL"];
                    myFields_FCDS[29] = rs_FCDS.Fields["FCDS_HHDL_FDJXH"];
                    myFields_FCDS[30] = rs_FCDS.Fields["FCDS_HHDL_HHDLJGXS"];
                    myFields_FCDS[31] = rs_FCDS.Fields["FCDS_HHDL_HHDLZDDGLB"];
                    myFields_FCDS[32] = rs_FCDS.Fields["FCDS_HHDL_JGL"];
                    myFields_FCDS[33] = rs_FCDS.Fields["FCDS_HHDL_PL"];
                    myFields_FCDS[34] = rs_FCDS.Fields["FCDS_HHDL_QDDJEDGL"];
                    myFields_FCDS[35] = rs_FCDS.Fields["FCDS_HHDL_QDDJFZNJ"];

                    myFields_FCDS[36] = rs_FCDS.Fields["FCDS_HHDL_QDDJLX"];
                    myFields_FCDS[37] = rs_FCDS.Fields["FCDS_HHDL_QGS"];
                    myFields_FCDS[38] = rs_FCDS.Fields["FCDS_HHDL_SJGKRLXHL"];

                    myFields_FCDS[39] = rs_FCDS.Fields["FCDS_HHDL_SQGKRLXHL"];
                    myFields_FCDS[40] = rs_FCDS.Fields["FCDS_HHDL_XSMSSDXZGN"];
                    myFields_FCDS[41] = rs_FCDS.Fields["FCDS_HHDL_ZHGKRLXHL"];
                    myFields_FCDS[42] = rs_FCDS.Fields["FCDS_HHDL_ZHKGCO2PL"];
                    myFields_FCDS[43] = rs_FCDS.Fields["CREATETIME"];
                    myFields_FCDS[44] = rs_FCDS.Fields["UPDATETIME"];
 
                    #endregion

                    #region 插电式混合动力
                    DAO.Recordset rs_CDS = db.OpenRecordset("CDS_MAIN");
                    DAO.Field[] myFields_CDS = new DAO.Field[46];
                    myFields_CDS[0] = rs_CDS.Fields["UNIQUE_CODE"];
                    myFields_CDS[1] = rs_CDS.Fields["QCSCQY"];
                    myFields_CDS[2] = rs_CDS.Fields["JYBGBH"];
                    myFields_CDS[3] = rs_CDS.Fields["JKQCZJXS"];
                    myFields_CDS[4] = rs_CDS.Fields["CLXH"];
                    myFields_CDS[5] = rs_CDS.Fields["HGSPBM"];
                    myFields_CDS[6] = rs_CDS.Fields["CLZL"];
                    myFields_CDS[7] = rs_CDS.Fields["YYC"];
                    myFields_CDS[8] = rs_CDS.Fields["QDXS"];
                    myFields_CDS[9] = rs_CDS.Fields["ZWPS"];
                    myFields_CDS[10] = rs_CDS.Fields["ZCZBZL"];
                    myFields_CDS[11] = rs_CDS.Fields["ZDSJZZL"];
                    myFields_CDS[12] = rs_CDS.Fields["ZGCS"];
                    myFields_CDS[13] = rs_CDS.Fields["EDZK"];
                    myFields_CDS[14] = rs_CDS.Fields["LTGG"];
                    myFields_CDS[15] = rs_CDS.Fields["LJ"];
                    myFields_CDS[16] = rs_CDS.Fields["JYJGMC"];
                    myFields_CDS[17] = rs_CDS.Fields["TYMC"];
                    myFields_CDS[18] = rs_CDS.Fields["ZJ"];
                    myFields_CDS[19] = rs_CDS.Fields["RLLX"];
                    myFields_CDS[20] = rs_CDS.Fields["CDS_HHDL_BSQDWS"];
                    myFields_CDS[21] = rs_CDS.Fields["CDS_HHDL_BSQXS"];
                    myFields_CDS[22] = rs_CDS.Fields["CDS_HHDL_CDDMSXZGCS"];
                    myFields_CDS[23] = rs_CDS.Fields["CDS_HHDL_CDDMSXZHGKXSLC"];
                    myFields_CDS[24] = rs_CDS.Fields["CDS_HHDL_DLXDCBNL"];
                    myFields_CDS[25] = rs_CDS.Fields["CDS_HHDL_DLXDCZBCDY"];
                    myFields_CDS[26] = rs_CDS.Fields["CDS_HHDL_DLXDCZZL"];
                    myFields_CDS[27] = rs_CDS.Fields["CDS_HHDL_DLXDCZZNL"];
                    myFields_CDS[28] = rs_CDS.Fields["CDS_HHDL_EDGL"];
                    myFields_CDS[29] = rs_CDS.Fields["CDS_HHDL_FDJXH"];
                    myFields_CDS[30] = rs_CDS.Fields["CDS_HHDL_HHDLJGXS"];
                    myFields_CDS[31] = rs_CDS.Fields["CDS_HHDL_HHDLZDDGLB"];
                    myFields_CDS[32] = rs_CDS.Fields["CDS_HHDL_JGL"];
                    myFields_CDS[33] = rs_CDS.Fields["CDS_HHDL_PL"];
                    myFields_CDS[34] = rs_CDS.Fields["CDS_HHDL_QDDJEDGL"];

                    myFields_CDS[35] = rs_CDS.Fields["CDS_HHDL_QDDJFZNJ"];
                    myFields_CDS[36] = rs_CDS.Fields["CDS_HHDL_QDDJLX"];
                    myFields_CDS[37] = rs_CDS.Fields["CDS_HHDL_QGS"];

                    myFields_CDS[38] = rs_CDS.Fields["CDS_HHDL_XSMSSDXZGN"];
                    myFields_CDS[39] = rs_CDS.Fields["CDS_HHDL_ZHGKDNXHL"];
                    myFields_CDS[40] = rs_CDS.Fields["CDS_HHDL_ZHGKRLXHL"];
                    myFields_CDS[41] = rs_CDS.Fields["CDS_HHDL_ZHKGCO2PL"];
                    myFields_CDS[42] = rs_CDS.Fields["CREATETIME"];
                    myFields_CDS[43] = rs_CDS.Fields["UPDATETIME"];
                    myFields_CDS[44] = rs_CDS.Fields["CDS_HHDL_TJASYZDNXHL"];
                    myFields_CDS[45] = rs_CDS.Fields["CDS_HHDL_TJBSYZDNXHL"];
                    #endregion

                    #region 纯电动
                    DAO.Recordset rs_CDD = db.OpenRecordset("CDD_MAIN");
                    DAO.Field[] myFields_CDD = new DAO.Field[33];
                    myFields_CDD[0] = rs_CDD.Fields["UNIQUE_CODE"];
                    myFields_CDD[1] = rs_CDD.Fields["QCSCQY"];
                    myFields_CDD[2] = rs_CDD.Fields["JYBGBH"];
                    myFields_CDD[3] = rs_CDD.Fields["JKQCZJXS"];
                    myFields_CDD[4] = rs_CDD.Fields["CLXH"];
                    myFields_CDD[5] = rs_CDD.Fields["HGSPBM"];
                    myFields_CDD[6] = rs_CDD.Fields["CLZL"];
                    myFields_CDD[7] = rs_CDD.Fields["YYC"];
                    myFields_CDD[8] = rs_CDD.Fields["QDXS"];
                    myFields_CDD[9] = rs_CDD.Fields["ZWPS"];
                    myFields_CDD[10] = rs_CDD.Fields["ZCZBZL"];
                    myFields_CDD[11] = rs_CDD.Fields["ZDSJZZL"];
                    myFields_CDD[12] = rs_CDD.Fields["ZGCS"];
                    myFields_CDD[13] = rs_CDD.Fields["EDZK"];
                    myFields_CDD[14] = rs_CDD.Fields["LTGG"];
                    myFields_CDD[15] = rs_CDD.Fields["LJ"];
                    myFields_CDD[16] = rs_CDD.Fields["JYJGMC"];
                    myFields_CDD[17] = rs_CDD.Fields["TYMC"];
                    myFields_CDD[18] = rs_CDD.Fields["ZJ"];
                    myFields_CDD[19] = rs_CDD.Fields["RLLX"];
                    myFields_CDD[20] = rs_CDD.Fields["CDD_DDQC30FZZGCS"];
                    myFields_CDD[21] = rs_CDD.Fields["CDD_DDXDCZZLYZCZBZLDBZ"];
                    myFields_CDD[22] = rs_CDD.Fields["CDD_DLXDCBNL"];
                    myFields_CDD[23] = rs_CDD.Fields["CDD_DLXDCZBCDY"];
                    myFields_CDD[24] = rs_CDD.Fields["CDD_DLXDCZEDNL"];
                    myFields_CDD[25] = rs_CDD.Fields["CDD_DLXDCZZL"];
                    myFields_CDD[26] = rs_CDD.Fields["CDD_QDDJEDGL"];
                    myFields_CDD[27] = rs_CDD.Fields["CDD_QDDJFZNJ"];
                    myFields_CDD[28] = rs_CDD.Fields["CDD_QDDJLX"];
                    myFields_CDD[29] = rs_CDD.Fields["CDD_ZHGKDNXHL"];
                    myFields_CDD[30] = rs_CDD.Fields["CDD_ZHGKXSLC"];
                    myFields_CDD[31] = rs_CDD.Fields["CREATETIME"];
                    myFields_CDD[32] = rs_CDD.Fields["UPDATETIME"];
                    #endregion

                    #region  燃料电池
                    DAO.Recordset rs_RLDC = db.OpenRecordset("RLDC_MAIN");
                    DAO.Field[] myFields_RLDC = new DAO.Field[36];
                    myFields_RLDC[0] = rs_RLDC.Fields["UNIQUE_CODE"];
                    myFields_RLDC[1] = rs_RLDC.Fields["QCSCQY"];
                    myFields_RLDC[2] = rs_RLDC.Fields["JYBGBH"];
                    myFields_RLDC[3] = rs_RLDC.Fields["JKQCZJXS"];
                    myFields_RLDC[4] = rs_RLDC.Fields["CLXH"];
                    myFields_RLDC[5] = rs_RLDC.Fields["HGSPBM"];
                    myFields_RLDC[6] = rs_RLDC.Fields["CLZL"];
                    myFields_RLDC[7] = rs_RLDC.Fields["YYC"];
                    myFields_RLDC[8] = rs_RLDC.Fields["QDXS"];
                    myFields_RLDC[9] = rs_RLDC.Fields["ZWPS"];
                    myFields_RLDC[10] = rs_RLDC.Fields["ZCZBZL"];
                    myFields_RLDC[11] = rs_RLDC.Fields["ZDSJZZL"];
                    myFields_RLDC[12] = rs_RLDC.Fields["ZGCS"];
                    myFields_RLDC[13] = rs_RLDC.Fields["EDZK"];
                    myFields_RLDC[14] = rs_RLDC.Fields["LTGG"];
                    myFields_RLDC[15] = rs_RLDC.Fields["LJ"];
                    myFields_RLDC[16] = rs_RLDC.Fields["JYJGMC"];
                    myFields_RLDC[17] = rs_RLDC.Fields["TYMC"];
                    myFields_RLDC[18] = rs_RLDC.Fields["ZJ"];
                    myFields_RLDC[19] = rs_RLDC.Fields["RLLX"];
                    myFields_RLDC[20] = rs_RLDC.Fields["RLDC_CDDMSXZGXSCS"];
                    myFields_RLDC[21] = rs_RLDC.Fields["RLDC_CQPBCGZYL"];
                    myFields_RLDC[22] = rs_RLDC.Fields["RLDC_CQPRJ"];
                    myFields_RLDC[23] = rs_RLDC.Fields["RLDC_DDGLMD"];
                    myFields_RLDC[24] = rs_RLDC.Fields["RLDC_CQPLX"];
                    myFields_RLDC[25] = rs_RLDC.Fields["RLDC_DDHHJSTJXXDCZBNL"];
                    myFields_RLDC[26] = rs_RLDC.Fields["RLDC_DLXDCZZL"];
                    myFields_RLDC[27] = rs_RLDC.Fields["RLDC_QDDJEDGL"];
                    myFields_RLDC[28] = rs_RLDC.Fields["RLDC_QDDJFZNJ"];
                    myFields_RLDC[29] = rs_RLDC.Fields["RLDC_QDDJLX"];
                    myFields_RLDC[30] = rs_RLDC.Fields["RLDC_RLLX"];
                    myFields_RLDC[31] = rs_RLDC.Fields["RLDC_ZHGKHQL"];
                    myFields_RLDC[32] = rs_RLDC.Fields["RLDC_ZHGKXSLC"];
                    myFields_RLDC[33] = rs_RLDC.Fields["CREATETIME"];
                    myFields_RLDC[34] = rs_RLDC.Fields["UPDATETIME"];
                    myFields_RLDC[35] = rs_RLDC.Fields["RLDC_RLDCXTEDGL"];
                    #endregion
                    string error = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {
                        error = VerifyData(dr, tdr, "IMPORT");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            if (rlzl.Equals(CTNY))
                            {
                                #region 传统能源
                                rs_CTNY.AddNew();
                                myFields[0].Value = dr["UNIQUE_CODE"];
                                myFields[1].Value = dr["QCSCQY"];
                                myFields[2].Value = dr["JYBGBH"];
                                myFields[3].Value = dr["JKQCZJXS"];
                                myFields[4].Value = dr["CLXH"];
                                myFields[5].Value = dr["HGSPBM"];
                                myFields[6].Value = dr["CLZL"];
                                myFields[7].Value = dr["YYC"];
                                myFields[8].Value = dr["QDXS"];
                                myFields[9].Value = dr["ZWPS"];
                                myFields[10].Value = dr["ZCZBZL"];
                                myFields[11].Value = dr["ZDSJZZL"];
                                myFields[12].Value = dr["ZGCS"];
                                myFields[13].Value = dr["EDZK"];
                                myFields[14].Value = dr["LTGG"];
                                myFields[15].Value = dr["LJ"];
                                myFields[16].Value = dr["JYJGMC"];
                                myFields[17].Value = dr["TYMC"];
                                myFields[18].Value = dr["ZJ"];
                                myFields[19].Value = dr["RLLX"];
                                myFields[20].Value = dr["CT_BSQXS"];
                                myFields[21].Value = dr["CT_EDGL"];
                                myFields[22].Value = dr["CT_FDJXH"];
                                myFields[23].Value = dr["CT_JGL"];
                                myFields[24].Value = dr["CT_PL"];
                                myFields[25].Value = dr["CT_QGS"];
                                myFields[26].Value = dr["CT_QTXX"];
                                myFields[27].Value = dr["CT_SJGKRLXHL"];
                                myFields[28].Value = dr["CT_SQGKRLXHL"];
                                myFields[29].Value = dr["CT_ZHGKCO2PFL"];
                                myFields[30].Value = dr["CT_ZHGKRLXHL"];
                                myFields[31].Value = dr["CT_BSQDWS"];
                                myFields[32].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                myFields[33].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                rs_CTNY.Update();

                                succCount++;
                                #endregion
                            }
                            else if (rlzl.Equals(FCDSHHDL))
                            {
                                #region 非插电式混合动力
                                try
                                {
                                    #region insert
                                    rs_FCDS.AddNew();
                                    myFields_FCDS[0].Value = dr["UNIQUE_CODE"];
                                    myFields_FCDS[1].Value = dr["QCSCQY"];
                                    myFields_FCDS[2].Value = dr["JYBGBH"];
                                    myFields_FCDS[3].Value = dr["JKQCZJXS"];
                                    myFields_FCDS[4].Value = dr["CLXH"];
                                    myFields_FCDS[5].Value = dr["HGSPBM"];
                                    myFields_FCDS[6].Value = dr["CLZL"];
                                    myFields_FCDS[7].Value = dr["YYC"];
                                    myFields_FCDS[8].Value = dr["QDXS"];
                                    myFields_FCDS[9].Value = dr["ZWPS"];
                                    myFields_FCDS[10].Value = dr["ZCZBZL"];
                                    myFields_FCDS[11].Value = dr["ZDSJZZL"];
                                    myFields_FCDS[12].Value = dr["ZGCS"];
                                    myFields_FCDS[13].Value = dr["EDZK"];
                                    myFields_FCDS[14].Value = dr["LTGG"];
                                    myFields_FCDS[15].Value = dr["LJ"];
                                    myFields_FCDS[16].Value = dr["JYJGMC"];
                                    myFields_FCDS[17].Value = dr["TYMC"];
                                    myFields_FCDS[18].Value = dr["ZJ"];
                                    myFields_FCDS[19].Value = dr["RLLX"];
                                    myFields_FCDS[20].Value = dr["FCDS_HHDL_BSQDWS"];
                                    myFields_FCDS[21].Value = dr["FCDS_HHDL_BSQXS"];
                                    myFields_FCDS[22].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                                    myFields_FCDS[23].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                                    myFields_FCDS[24].Value = dr["FCDS_HHDL_DLXDCBNL"];
                                    myFields_FCDS[25].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                                    myFields_FCDS[26].Value = dr["FCDS_HHDL_DLXDCZZL"];
                                    myFields_FCDS[27].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                                    myFields_FCDS[28].Value = dr["FCDS_HHDL_EDGL"];
                                    myFields_FCDS[29].Value = dr["FCDS_HHDL_FDJXH"];
                                    myFields_FCDS[30].Value = dr["FCDS_HHDL_HHDLJGXS"];
                                    myFields_FCDS[31].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                                    myFields_FCDS[32].Value = dr["FCDS_HHDL_JGL"];
                                    myFields_FCDS[33].Value = dr["FCDS_HHDL_PL"];
                                    myFields_FCDS[34].Value = dr["FCDS_HHDL_QDDJEDGL"];
                                    myFields_FCDS[35].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                                    myFields_FCDS[36].Value = dr["FCDS_HHDL_QDDJLX"];
                                    myFields_FCDS[37].Value = dr["FCDS_HHDL_QGS"];
                                    myFields_FCDS[38].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                                    myFields_FCDS[39].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                                    myFields_FCDS[40].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                                    myFields_FCDS[41].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                                    myFields_FCDS[42].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                                    myFields_FCDS[43].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    myFields_FCDS[44].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    rs_FCDS.Update();
                                    succCount++;

                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(CDSHHDL))
                            {
                                #region 插电式混合动力
                                try
                                {
                                    #region insert
                                    rs_CDS.AddNew();
                                    myFields_CDS[0].Value = dr["UNIQUE_CODE"];
                                    myFields_CDS[1].Value = dr["QCSCQY"];
                                    myFields_CDS[2].Value = dr["JYBGBH"];
                                    myFields_CDS[3].Value = dr["JKQCZJXS"];
                                    myFields_CDS[4].Value = dr["CLXH"];
                                    myFields_CDS[5].Value = dr["HGSPBM"];
                                    myFields_CDS[6].Value = dr["CLZL"];
                                    myFields_CDS[7].Value = dr["YYC"];
                                    myFields_CDS[8].Value = dr["QDXS"];
                                    myFields_CDS[9].Value = dr["ZWPS"];
                                    myFields_CDS[10].Value = dr["ZCZBZL"];
                                    myFields_CDS[11].Value = dr["ZDSJZZL"];
                                    myFields_CDS[12].Value = dr["ZGCS"];
                                    myFields_CDS[13].Value = dr["EDZK"];
                                    myFields_CDS[14].Value = dr["LTGG"];
                                    myFields_CDS[15].Value = dr["LJ"];
                                    myFields_CDS[16].Value = dr["JYJGMC"];
                                    myFields_CDS[17].Value = dr["TYMC"];
                                    myFields_CDS[18].Value = dr["ZJ"];
                                    myFields_CDS[19].Value = dr["RLLX"];
                                    myFields_CDS[20].Value = dr["CDS_HHDL_BSQDWS"];
                                    myFields_CDS[21].Value = dr["CDS_HHDL_BSQXS"];
                                    myFields_CDS[22].Value = dr["CDS_HHDL_CDDMSXZGCS"];
                                    myFields_CDS[23].Value = dr["CDS_HHDL_CDDMSXZHGKXSLC"];
                                    myFields_CDS[24].Value = dr["CDS_HHDL_DLXDCBNL"];
                                    myFields_CDS[25].Value = dr["CDS_HHDL_DLXDCZBCDY"];
                                    myFields_CDS[26].Value = dr["CDS_HHDL_DLXDCZZL"];
                                    myFields_CDS[27].Value = dr["CDS_HHDL_DLXDCZZNL"];
                                    myFields_CDS[28].Value = dr["CDS_HHDL_EDGL"];
                                    myFields_CDS[29].Value = dr["CDS_HHDL_FDJXH"];
                                    myFields_CDS[30].Value = dr["CDS_HHDL_HHDLJGXS"];
                                    myFields_CDS[31].Value = dr["CDS_HHDL_HHDLZDDGLB"];
                                    myFields_CDS[32].Value = dr["CDS_HHDL_JGL"];
                                    myFields_CDS[33].Value = dr["CDS_HHDL_PL"];
                                    myFields_CDS[34].Value = dr["CDS_HHDL_QDDJEDGL"];
                                    myFields_CDS[35].Value = dr["CDS_HHDL_QDDJFZNJ"];
                                    myFields_CDS[36].Value = dr["CDS_HHDL_QDDJLX"];
                                    myFields_CDS[37].Value = dr["CDS_HHDL_QGS"];
                                    myFields_CDS[38].Value = dr["CDS_HHDL_XSMSSDXZGN"];
                                    myFields_CDS[39].Value = dr["CDS_HHDL_ZHGKDNXHL"];
                                    myFields_CDS[40].Value = dr["CDS_HHDL_ZHGKRLXHL"];
                                    myFields_CDS[41].Value = dr["CDS_HHDL_ZHKGCO2PL"];
                                    myFields_CDS[42].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    myFields_CDS[43].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    myFields_CDS[44].Value = dr["CDS_HHDL_TJASYZDNXHL"];
                                    myFields_CDS[45].Value = dr["CDS_HHDL_TJBSYZDNXHL"];
                                    rs_CDS.Update();
                                    succCount++;

                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(CDD))
                            {
                                #region 纯电动
                                try
                                {
                                    #region insert

                                    rs_CDD.AddNew();
                                    myFields_CDD[0].Value = dr["UNIQUE_CODE"];
                                    myFields_CDD[1].Value = dr["QCSCQY"];
                                    myFields_CDD[2].Value = dr["JYBGBH"];
                                    myFields_CDD[3].Value = dr["JKQCZJXS"];
                                    myFields_CDD[4].Value = dr["CLXH"];
                                    myFields_CDD[5].Value = dr["HGSPBM"];
                                    myFields_CDD[6].Value = dr["CLZL"];
                                    myFields_CDD[7].Value = dr["YYC"];
                                    myFields_CDD[8].Value = dr["QDXS"];
                                    myFields_CDD[9].Value = dr["ZWPS"];
                                    myFields_CDD[10].Value = dr["ZCZBZL"];
                                    myFields_CDD[11].Value = dr["ZDSJZZL"];
                                    myFields_CDD[12].Value = dr["ZGCS"];
                                    myFields_CDD[13].Value = dr["EDZK"];
                                    myFields_CDD[14].Value = dr["LTGG"];
                                    myFields_CDD[15].Value = dr["LJ"];
                                    myFields_CDD[16].Value = dr["JYJGMC"];
                                    myFields_CDD[17].Value = dr["TYMC"];
                                    myFields_CDD[18].Value = dr["ZJ"];
                                    myFields_CDD[19].Value = dr["RLLX"];
                                    myFields_CDD[20].Value = dr["CDD_DDQC30FZZGCS"];
                                    myFields_CDD[21].Value = dr["CDD_DDXDCZZLYZCZBZLDBZ"];
                                    myFields_CDD[22].Value = dr["CDD_DLXDCBNL"];
                                    myFields_CDD[23].Value = dr["CDD_DLXDCZBCDY"];
                                    myFields_CDD[24].Value = dr["CDD_DLXDCZEDNL"];
                                    myFields_CDD[25].Value = dr["CDD_DLXDCZZL"];
                                    myFields_CDD[26].Value = dr["CDD_QDDJEDGL"];
                                    myFields_CDD[27].Value = dr["CDD_QDDJFZNJ"];
                                    myFields_CDD[28].Value = dr["CDD_QDDJLX"];
                                    myFields_CDD[29].Value = dr["CDD_ZHGKDNXHL"];
                                    myFields_CDD[30].Value = dr["CDD_ZHGKXSLC"];
                                    myFields_CDD[31].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    myFields_CDD[32].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);

                                    rs_CDD.Update();
                                    succCount++;

                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(RLDC))
                            {
                                #region 燃料电池
                                try
                                {
                                    #region insert
                                    rs_RLDC.AddNew();
                                    myFields_RLDC[0].Value = dr["UNIQUE_CODE"];
                                    myFields_RLDC[1].Value = dr["QCSCQY"];
                                    myFields_RLDC[2].Value = dr["JYBGBH"];
                                    myFields_RLDC[3].Value = dr["JKQCZJXS"];
                                    myFields_RLDC[4].Value = dr["CLXH"];
                                    myFields_RLDC[5].Value = dr["HGSPBM"];
                                    myFields_RLDC[6].Value = dr["CLZL"];
                                    myFields_RLDC[7].Value = dr["YYC"];
                                    myFields_RLDC[8].Value = dr["QDXS"];
                                    myFields_RLDC[9].Value = dr["ZWPS"];
                                    myFields_RLDC[10].Value = dr["ZCZBZL"];
                                    myFields_RLDC[11].Value = dr["ZDSJZZL"];
                                    myFields_RLDC[12].Value = dr["ZGCS"];
                                    myFields_RLDC[13].Value = dr["EDZK"];
                                    myFields_RLDC[14].Value = dr["LTGG"];
                                    myFields_RLDC[15].Value = dr["LJ"];
                                    myFields_RLDC[16].Value = dr["JYJGMC"];
                                    myFields_RLDC[17].Value = dr["TYMC"];
                                    myFields_RLDC[18].Value = dr["ZJ"];
                                    myFields_RLDC[19].Value = dr["RLLX"];
                                    myFields_RLDC[20].Value = dr["RLDC_CDDMSXZGXSCS"];
                                    myFields_RLDC[21].Value = dr["RLDC_CQPBCGZYL"];
                                    myFields_RLDC[22].Value = dr["RLDC_CQPRJ"];
                                    myFields_RLDC[23].Value = dr["RLDC_DDGLMD"];
                                    myFields_RLDC[24].Value = dr["RLDC_CQPLX"];
                                    myFields_RLDC[25].Value = dr["RLDC_DDHHJSTJXXDCZBNL"];
                                    myFields_RLDC[26].Value = dr["RLDC_DLXDCZZL"];
                                    myFields_RLDC[27].Value = dr["RLDC_QDDJEDGL"];
                                    myFields_RLDC[28].Value = dr["RLDC_QDDJFZNJ"];
                                    myFields_RLDC[29].Value = dr["RLDC_QDDJLX"];
                                    myFields_RLDC[30].Value = dr["RLDC_RLLX"];
                                    myFields_RLDC[31].Value = dr["RLDC_ZHGKHQL"];
                                    myFields_RLDC[32].Value = dr["RLDC_ZHGKXSLC"];
                                    myFields_RLDC[33].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    myFields_RLDC[34].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    myFields_RLDC[35].Value = dr["RLDC_RLDCXTEDGL"];
                                    rs_RLDC.Update();
                                   
                                    succCount++;

                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                        }
                    }
                    rs_CTNY.Close();
                    rs_CDD.Close();
                    rs_CDS.Close();
                    rs_FCDS.Close();
                    rs_RLDC.Close();
                }

                db.Close();
              
            }
            catch (Exception ex)
            {
                msg += ex.Message + "\r\n";
            }

            return succCount;
        }

        /// <summary>
        /// 修改已经导入的主表信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string UpdateMainData(DataSet ds, List<string> mainUpdateList)
        {
            int totalCount = 0;
            int succCount = 0;
            string msg = string.Empty;
            string clxh = string.Empty;
            //string strCon = AccessHelper.conn;
            //OleDbConnection con = new OleDbConnection(strCon);
            //con.Open();
            //OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

            try
            {
                // 传统能源
                DataTable dtCtny = D2D(dictCTNY, ds.Tables[CTNY], CTNY);
                //DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + dtCtny.TableName + "' and STATUS=1");
                DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + CTNY + "' and STATUS=1");

                //// 非插电式混合动力
                //DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);
                //DataRow[] drFcdsHhdl = checkData.Select("FUEL_TYPE='" + dtFcdsHhdl.TableName + "' and STATUS=1");

                // 传统能源
                if (dtCtny != null && dtCtny.Rows.Count > 0)
                {
                    totalCount = dtCtny.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtCtny.Rows)
                    {
                        //totalCount += dtCtny.Rows.Count;
                        error = VerifyData(dr, drCtny, "UPDATE");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region UPDATE
                            clxh = dr["CLXH"].ToString();
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("update CTNY_MAIN set ");
                            strSql.Append("QCSCQY=@QCSCQY,");
                            strSql.Append("JYBGBH=@JYBGBH,");
                            strSql.Append("JKQCZJXS=@JKQCZJXS,");
                            strSql.Append("HGSPBM=@HGSPBM,");
                            strSql.Append("CLZL=@CLZL,");
                            strSql.Append("YYC=@YYC,");
                            strSql.Append("QDXS=@QDXS,");
                            strSql.Append("ZWPS=@ZWPS,");
                            strSql.Append("ZCZBZL=@ZCZBZL,");
                            strSql.Append("ZDSJZZL=@ZDSJZZL,");
                            strSql.Append("ZGCS=@ZGCS,");
                            strSql.Append("EDZK=@EDZK,");
                            strSql.Append("LTGG=@LTGG,");
                            strSql.Append("LJ=@LJ,");
                            strSql.Append("JYJGMC=@JYJGMC,");
                            strSql.Append("TYMC=@TYMC,");
                            strSql.Append("ZJ=@ZJ,");
                            strSql.Append("RLLX=@RLLX,");
                            strSql.Append("CT_BSQXS=@CT_BSQXS,");
                            strSql.Append("CT_EDGL=@CT_EDGL,");
                            strSql.Append("CT_FDJXH=@CT_FDJXH,");
                            strSql.Append("CT_JGL=@CT_JGL,");
                            strSql.Append("CT_PL=@CT_PL,");
                            strSql.Append("CT_QCJNJS=@CT_QCJNJS,");
                            strSql.Append("CT_QGS=@CT_QGS,");
                            strSql.Append("CT_QTXX=@CT_QTXX,");
                            strSql.Append("CT_SJGKRLXHL=@CT_SJGKRLXHL,");
                            strSql.Append("CT_SQGKRLXHL=@CT_SQGKRLXHL,");
                            strSql.Append("CT_ZHGKCO2PFL=@CT_ZHGKCO2PFL,");
                            strSql.Append("CT_ZHGKRLXHL=@CT_ZHGKRLXHL,");
                            strSql.Append("CT_BSQDWS=@CT_BSQDWS");
                            strSql.Append(",CLXH=@CLXH");
                            strSql.Append(" where UNIQUE_CODE=@UNIQUE_CODE");

                            OleDbParameter[] parameters = {
					        new OleDbParameter("@QCSCQY", OleDbType.VarChar,255),
					        new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
					        new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,255),
					        new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
					        new OleDbParameter("@CLZL", OleDbType.VarChar,255),
					        new OleDbParameter("@YYC", OleDbType.VarChar,255),
					        new OleDbParameter("@QDXS", OleDbType.VarChar,255),
					        new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
					        new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
					        new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
					        new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
					        new OleDbParameter("@EDZK", OleDbType.VarChar,255),
					        new OleDbParameter("@LTGG", OleDbType.VarChar,200),
					        new OleDbParameter("@LJ", OleDbType.VarChar,255),
					        new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
					        new OleDbParameter("@TYMC", OleDbType.VarChar,255),
					        new OleDbParameter("@ZJ", OleDbType.VarChar,255),
					        new OleDbParameter("@RLLX", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_BSQXS", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_EDGL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_FDJXH", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_JGL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_PL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_QCJNJS", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_QGS", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_QTXX", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_SJGKRLXHL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_SQGKRLXHL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_ZHGKCO2PFL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_ZHGKRLXHL", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_BSQDWS", OleDbType.VarChar,255),
					        new OleDbParameter("@CLXH", OleDbType.VarChar,255),
					        new OleDbParameter("@UNIQUE_CODE", OleDbType.VarChar,255)
                            };

                            parameters[0].Value = dr["QCSCQY"];
                            parameters[1].Value = dr["JYBGBH"];
                            parameters[2].Value = dr["JKQCZJXS"];
                            parameters[3].Value = dr["HGSPBM"];
                            parameters[4].Value = dr["CLZL"];
                            parameters[5].Value = dr["YYC"];
                            parameters[6].Value = dr["QDXS"];
                            parameters[7].Value = dr["ZWPS"];
                            parameters[8].Value = dr["ZCZBZL"];
                            parameters[9].Value = dr["ZDSJZZL"];
                            parameters[10].Value = dr["ZGCS"];
                            parameters[11].Value = dr["EDZK"];
                            parameters[12].Value = dr["LTGG"];
                            parameters[13].Value = dr["LJ"];
                            parameters[14].Value = dr["JYJGMC"];
                            parameters[15].Value = dr["TYMC"];
                            parameters[16].Value = dr["ZJ"];
                            parameters[17].Value = dr["RLLX"];
                            parameters[18].Value = dr["CT_BSQXS"];
                            parameters[19].Value = dr["CT_EDGL"];
                            parameters[20].Value = dr["CT_FDJXH"];
                            parameters[21].Value = dr["CT_JGL"];
                            parameters[22].Value = dr["CT_PL"];
                            parameters[23].Value = dr["CT_QCJNJS"];
                            parameters[24].Value = dr["CT_QGS"];
                            parameters[25].Value = dr["CT_QTXX"];
                            parameters[26].Value = dr["CT_SJGKRLXHL"];
                            parameters[27].Value = dr["CT_SQGKRLXHL"];
                            parameters[28].Value = dr["CT_ZHGKCO2PFL"];
                            parameters[29].Value = dr["CT_ZHGKRLXHL"];
                            parameters[30].Value = dr["CT_BSQDWS"];
                            parameters[31].Value = dr["CLXH"];
                            parameters[32].Value = dr["UNIQUE_CODE"];

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;

                            mainUpdateList.Add(clxh);
                            #endregion
                        }
                    }
                }

                #region 非插电式
                //            if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
                //            {
                //                string error = string.Empty;
                //                foreach (DataRow dr in dtFcdsHhdl.Rows)
                //                {
                //                    error = VerifyData(dr, drFcdsHhdl, "UPDATE");      //单行验证
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
                //                                            FCDS_HHDL_ZHKGCO2PL=@FCDS_HHDL_ZHKGCO2PL,UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                //                                            HGSPBM=@HGSPBM,CT_QTXX=@CT_QTXX
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
                //                        new OleDbParameter("@HGSPBM", OleDbType.VarChar,50),
                //                        new OleDbParameter("@CT_QTXX", OleDbType.VarChar,255),

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
                //                        parameters[44].Value = dr["HGSPBM"];
                //                        parameters[45].Value = dr["CT_QTXX"];
                //                        parameters[46].Value = dr["MAIN_ID"];

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
        public int UpdMainData_DAO(DataTable dt, string rlzl, ref string msg)
        {
            int succCount = 0;
            if (string.IsNullOrEmpty(msg))
            {
                msg = string.Empty;
            }
            try
            {
                DAO.DBEngine dbEngine = new DAO.DBEngine();
                DAO.Database db = dbEngine.OpenDatabase(Utils.dataPath);

                DataRow[] tdr = checkData.Select("FUEL_TYPE='" + rlzl + "' and STATUS=1");
                if (dt != null && dt.Rows.Count > 0)
                {
                    //初始化
                    #region 传统能源
                    DAO.Recordset rs_CTNY = db.OpenRecordset("CTNY_MAIN");
                    DAO.Field[] myFields = new DAO.Field[34];
                    myFields[0] = rs_CTNY.Fields["UNIQUE_CODE"];
                    myFields[1] = rs_CTNY.Fields["QCSCQY"];
                    myFields[2] = rs_CTNY.Fields["JYBGBH"];
                    myFields[3] = rs_CTNY.Fields["JKQCZJXS"];
                    myFields[4] = rs_CTNY.Fields["CLXH"];
                    myFields[5] = rs_CTNY.Fields["HGSPBM"];
                    myFields[6] = rs_CTNY.Fields["CLZL"];
                    myFields[7] = rs_CTNY.Fields["YYC"];
                    myFields[8] = rs_CTNY.Fields["QDXS"];
                    myFields[9] = rs_CTNY.Fields["ZWPS"];
                    myFields[10] = rs_CTNY.Fields["ZCZBZL"];
                    myFields[11] = rs_CTNY.Fields["ZDSJZZL"];
                    myFields[12] = rs_CTNY.Fields["ZGCS"];
                    myFields[13] = rs_CTNY.Fields["EDZK"];
                    myFields[14] = rs_CTNY.Fields["LTGG"];
                    myFields[15] = rs_CTNY.Fields["LJ"];
                    myFields[16] = rs_CTNY.Fields["JYJGMC"];
                    myFields[17] = rs_CTNY.Fields["TYMC"];
                    myFields[18] = rs_CTNY.Fields["ZJ"];
                    myFields[19] = rs_CTNY.Fields["RLLX"];
                    myFields[20] = rs_CTNY.Fields["CT_BSQXS"];
                    myFields[21] = rs_CTNY.Fields["CT_EDGL"];
                    myFields[22] = rs_CTNY.Fields["CT_FDJXH"];
                    myFields[23] = rs_CTNY.Fields["CT_JGL"];
                    myFields[24] = rs_CTNY.Fields["CT_PL"];
                    myFields[25] = rs_CTNY.Fields["CT_QGS"];
                    myFields[26] = rs_CTNY.Fields["CT_QTXX"];
                    myFields[27] = rs_CTNY.Fields["CT_SJGKRLXHL"];
                    myFields[28] = rs_CTNY.Fields["CT_SQGKRLXHL"];
                    myFields[29] = rs_CTNY.Fields["CT_ZHGKCO2PFL"];
                    myFields[30] = rs_CTNY.Fields["CT_ZHGKRLXHL"];
                    myFields[31] = rs_CTNY.Fields["CT_BSQDWS"];
                    myFields[32] = rs_CTNY.Fields["CREATETIME"];
                    myFields[33] = rs_CTNY.Fields["UPDATETIME"];
                    #endregion

                    #region 非插电式混合动力
                    DAO.Recordset rs_FCDS = db.OpenRecordset("FCDS_MAIN");
                    DAO.Field[] myFields_FCDS = new DAO.Field[45];
                    myFields_FCDS[0] = rs_FCDS.Fields["UNIQUE_CODE"];
                    myFields_FCDS[1] = rs_FCDS.Fields["QCSCQY"];
                    myFields_FCDS[2] = rs_FCDS.Fields["JYBGBH"];
                    myFields_FCDS[3] = rs_FCDS.Fields["JKQCZJXS"];
                    myFields_FCDS[4] = rs_FCDS.Fields["CLXH"];
                    myFields_FCDS[5] = rs_FCDS.Fields["HGSPBM"];
                    myFields_FCDS[6] = rs_FCDS.Fields["CLZL"];
                    myFields_FCDS[7] = rs_FCDS.Fields["YYC"];
                    myFields_FCDS[8] = rs_FCDS.Fields["QDXS"];
                    myFields_FCDS[9] = rs_FCDS.Fields["ZWPS"];
                    myFields_FCDS[10] = rs_FCDS.Fields["ZCZBZL"];
                    myFields_FCDS[11] = rs_FCDS.Fields["ZDSJZZL"];
                    myFields_FCDS[12] = rs_FCDS.Fields["ZGCS"];
                    myFields_FCDS[13] = rs_FCDS.Fields["EDZK"];
                    myFields_FCDS[14] = rs_FCDS.Fields["LTGG"];
                    myFields_FCDS[15] = rs_FCDS.Fields["LJ"];
                    myFields_FCDS[16] = rs_FCDS.Fields["JYJGMC"];
                    myFields_FCDS[17] = rs_FCDS.Fields["TYMC"];
                    myFields_FCDS[18] = rs_FCDS.Fields["ZJ"];
                    myFields_FCDS[19] = rs_FCDS.Fields["RLLX"];
                    myFields_FCDS[20] = rs_FCDS.Fields["FCDS_HHDL_BSQDWS"];
                    myFields_FCDS[21] = rs_FCDS.Fields["FCDS_HHDL_BSQXS"];
                    myFields_FCDS[22] = rs_FCDS.Fields["FCDS_HHDL_CDDMSXZGCS"];
                    myFields_FCDS[23] = rs_FCDS.Fields["FCDS_HHDL_CDDMSXZHGKXSLC"];
                    myFields_FCDS[24] = rs_FCDS.Fields["FCDS_HHDL_DLXDCBNL"];
                    myFields_FCDS[25] = rs_FCDS.Fields["FCDS_HHDL_DLXDCZBCDY"];
                    myFields_FCDS[26] = rs_FCDS.Fields["FCDS_HHDL_DLXDCZZL"];
                    myFields_FCDS[27] = rs_FCDS.Fields["FCDS_HHDL_DLXDCZZNL"];
                    myFields_FCDS[28] = rs_FCDS.Fields["FCDS_HHDL_EDGL"];
                    myFields_FCDS[29] = rs_FCDS.Fields["FCDS_HHDL_FDJXH"];
                    myFields_FCDS[30] = rs_FCDS.Fields["FCDS_HHDL_HHDLJGXS"];
                    myFields_FCDS[31] = rs_FCDS.Fields["FCDS_HHDL_HHDLZDDGLB"];
                    myFields_FCDS[32] = rs_FCDS.Fields["FCDS_HHDL_JGL"];
                    myFields_FCDS[33] = rs_FCDS.Fields["FCDS_HHDL_PL"];
                    myFields_FCDS[34] = rs_FCDS.Fields["FCDS_HHDL_QDDJEDGL"];
                    myFields_FCDS[35] = rs_FCDS.Fields["FCDS_HHDL_QDDJFZNJ"];

                    myFields_FCDS[36] = rs_FCDS.Fields["FCDS_HHDL_QDDJLX"];
                    myFields_FCDS[37] = rs_FCDS.Fields["FCDS_HHDL_QGS"];
                    myFields_FCDS[38] = rs_FCDS.Fields["FCDS_HHDL_SJGKRLXHL"];

                    myFields_FCDS[39] = rs_FCDS.Fields["FCDS_HHDL_SQGKRLXHL"];
                    myFields_FCDS[40] = rs_FCDS.Fields["FCDS_HHDL_XSMSSDXZGN"];
                    myFields_FCDS[41] = rs_FCDS.Fields["FCDS_HHDL_ZHGKRLXHL"];
                    myFields_FCDS[42] = rs_FCDS.Fields["FCDS_HHDL_ZHKGCO2PL"];
                    myFields_FCDS[43] = rs_FCDS.Fields["CREATETIME"];
                    myFields_FCDS[44] = rs_FCDS.Fields["UPDATETIME"];

                    #endregion

                    #region 插电式混合动力
                    DAO.Recordset rs_CDS = db.OpenRecordset("CDS_MAIN");
                    DAO.Field[] myFields_CDS = new DAO.Field[46];
                    myFields_CDS[0] = rs_CDS.Fields["UNIQUE_CODE"];
                    myFields_CDS[1] = rs_CDS.Fields["QCSCQY"];
                    myFields_CDS[2] = rs_CDS.Fields["JYBGBH"];
                    myFields_CDS[3] = rs_CDS.Fields["JKQCZJXS"];
                    myFields_CDS[4] = rs_CDS.Fields["CLXH"];
                    myFields_CDS[5] = rs_CDS.Fields["HGSPBM"];
                    myFields_CDS[6] = rs_CDS.Fields["CLZL"];
                    myFields_CDS[7] = rs_CDS.Fields["YYC"];
                    myFields_CDS[8] = rs_CDS.Fields["QDXS"];
                    myFields_CDS[9] = rs_CDS.Fields["ZWPS"];
                    myFields_CDS[10] = rs_CDS.Fields["ZCZBZL"];
                    myFields_CDS[11] = rs_CDS.Fields["ZDSJZZL"];
                    myFields_CDS[12] = rs_CDS.Fields["ZGCS"];
                    myFields_CDS[13] = rs_CDS.Fields["EDZK"];
                    myFields_CDS[14] = rs_CDS.Fields["LTGG"];
                    myFields_CDS[15] = rs_CDS.Fields["LJ"];
                    myFields_CDS[16] = rs_CDS.Fields["JYJGMC"];
                    myFields_CDS[17] = rs_CDS.Fields["TYMC"];
                    myFields_CDS[18] = rs_CDS.Fields["ZJ"];
                    myFields_CDS[19] = rs_CDS.Fields["RLLX"];
                    myFields_CDS[20] = rs_CDS.Fields["CDS_HHDL_BSQDWS"];
                    myFields_CDS[21] = rs_CDS.Fields["CDS_HHDL_BSQXS"];
                    myFields_CDS[22] = rs_CDS.Fields["CDS_HHDL_CDDMSXZGCS"];
                    myFields_CDS[23] = rs_CDS.Fields["CDS_HHDL_CDDMSXZHGKXSLC"];
                    myFields_CDS[24] = rs_CDS.Fields["CDS_HHDL_DLXDCBNL"];
                    myFields_CDS[25] = rs_CDS.Fields["CDS_HHDL_DLXDCZBCDY"];
                    myFields_CDS[26] = rs_CDS.Fields["CDS_HHDL_DLXDCZZL"];
                    myFields_CDS[27] = rs_CDS.Fields["CDS_HHDL_DLXDCZZNL"];
                    myFields_CDS[28] = rs_CDS.Fields["CDS_HHDL_EDGL"];
                    myFields_CDS[29] = rs_CDS.Fields["CDS_HHDL_FDJXH"];
                    myFields_CDS[30] = rs_CDS.Fields["CDS_HHDL_HHDLJGXS"];
                    myFields_CDS[31] = rs_CDS.Fields["CDS_HHDL_HHDLZDDGLB"];
                    myFields_CDS[32] = rs_CDS.Fields["CDS_HHDL_JGL"];
                    myFields_CDS[33] = rs_CDS.Fields["CDS_HHDL_PL"];
                    myFields_CDS[34] = rs_CDS.Fields["CDS_HHDL_QDDJEDGL"];

                    myFields_CDS[35] = rs_CDS.Fields["CDS_HHDL_QDDJFZNJ"];
                    myFields_CDS[36] = rs_CDS.Fields["CDS_HHDL_QDDJLX"];
                    myFields_CDS[37] = rs_CDS.Fields["CDS_HHDL_QGS"];

                    myFields_CDS[38] = rs_CDS.Fields["CDS_HHDL_XSMSSDXZGN"];
                    myFields_CDS[39] = rs_CDS.Fields["CDS_HHDL_ZHGKDNXHL"];
                    myFields_CDS[40] = rs_CDS.Fields["CDS_HHDL_ZHGKRLXHL"];
                    myFields_CDS[41] = rs_CDS.Fields["CDS_HHDL_ZHKGCO2PL"];
                    myFields_CDS[42] = rs_CDS.Fields["CREATETIME"];
                    myFields_CDS[43] = rs_CDS.Fields["UPDATETIME"];
                    myFields_CDS[44] = rs_CDS.Fields["CDS_HHDL_TJASYZDNXHL"];
                    myFields_CDS[45] = rs_CDS.Fields["CDS_HHDL_TJBSYZDNXHL"];
                    #endregion

                    #region 纯电动
                    DAO.Recordset rs_CDD = db.OpenRecordset("CDD_MAIN");
                    DAO.Field[] myFields_CDD = new DAO.Field[33];
                    myFields_CDD[0] = rs_CDD.Fields["UNIQUE_CODE"];
                    myFields_CDD[1] = rs_CDD.Fields["QCSCQY"];
                    myFields_CDD[2] = rs_CDD.Fields["JYBGBH"];
                    myFields_CDD[3] = rs_CDD.Fields["JKQCZJXS"];
                    myFields_CDD[4] = rs_CDD.Fields["CLXH"];
                    myFields_CDD[5] = rs_CDD.Fields["HGSPBM"];
                    myFields_CDD[6] = rs_CDD.Fields["CLZL"];
                    myFields_CDD[7] = rs_CDD.Fields["YYC"];
                    myFields_CDD[8] = rs_CDD.Fields["QDXS"];
                    myFields_CDD[9] = rs_CDD.Fields["ZWPS"];
                    myFields_CDD[10] = rs_CDD.Fields["ZCZBZL"];
                    myFields_CDD[11] = rs_CDD.Fields["ZDSJZZL"];
                    myFields_CDD[12] = rs_CDD.Fields["ZGCS"];
                    myFields_CDD[13] = rs_CDD.Fields["EDZK"];
                    myFields_CDD[14] = rs_CDD.Fields["LTGG"];
                    myFields_CDD[15] = rs_CDD.Fields["LJ"];
                    myFields_CDD[16] = rs_CDD.Fields["JYJGMC"];
                    myFields_CDD[17] = rs_CDD.Fields["TYMC"];
                    myFields_CDD[18] = rs_CDD.Fields["ZJ"];
                    myFields_CDD[19] = rs_CDD.Fields["RLLX"];
                    myFields_CDD[20] = rs_CDD.Fields["CDD_DDQC30FZZGCS"];
                    myFields_CDD[21] = rs_CDD.Fields["CDD_DDXDCZZLYZCZBZLDBZ"];
                    myFields_CDD[22] = rs_CDD.Fields["CDD_DLXDCBNL"];
                    myFields_CDD[23] = rs_CDD.Fields["CDD_DLXDCZBCDY"];
                    myFields_CDD[24] = rs_CDD.Fields["CDD_DLXDCZEDNL"];
                    myFields_CDD[25] = rs_CDD.Fields["CDD_DLXDCZZL"];
                    myFields_CDD[26] = rs_CDD.Fields["CDD_QDDJEDGL"];
                    myFields_CDD[27] = rs_CDD.Fields["CDD_QDDJFZNJ"];
                    myFields_CDD[28] = rs_CDD.Fields["CDD_QDDJLX"];
                    myFields_CDD[29] = rs_CDD.Fields["CDD_ZHGKDNXHL"];
                    myFields_CDD[30] = rs_CDD.Fields["CDD_ZHGKXSLC"];
                    myFields_CDD[31] = rs_CDD.Fields["CREATETIME"];
                    myFields_CDD[32] = rs_CDD.Fields["UPDATETIME"];
                    #endregion

                    #region  燃料电池
                    DAO.Recordset rs_RLDC = db.OpenRecordset("RLDC_MAIN");
                    DAO.Field[] myFields_RLDC = new DAO.Field[36];
                    myFields_RLDC[0] = rs_RLDC.Fields["UNIQUE_CODE"];
                    myFields_RLDC[1] = rs_RLDC.Fields["QCSCQY"];
                    myFields_RLDC[2] = rs_RLDC.Fields["JYBGBH"];
                    myFields_RLDC[3] = rs_RLDC.Fields["JKQCZJXS"];
                    myFields_RLDC[4] = rs_RLDC.Fields["CLXH"];
                    myFields_RLDC[5] = rs_RLDC.Fields["HGSPBM"];
                    myFields_RLDC[6] = rs_RLDC.Fields["CLZL"];
                    myFields_RLDC[7] = rs_RLDC.Fields["YYC"];
                    myFields_RLDC[8] = rs_RLDC.Fields["QDXS"];
                    myFields_RLDC[9] = rs_RLDC.Fields["ZWPS"];
                    myFields_RLDC[10] = rs_RLDC.Fields["ZCZBZL"];
                    myFields_RLDC[11] = rs_RLDC.Fields["ZDSJZZL"];
                    myFields_RLDC[12] = rs_RLDC.Fields["ZGCS"];
                    myFields_RLDC[13] = rs_RLDC.Fields["EDZK"];
                    myFields_RLDC[14] = rs_RLDC.Fields["LTGG"];
                    myFields_RLDC[15] = rs_RLDC.Fields["LJ"];
                    myFields_RLDC[16] = rs_RLDC.Fields["JYJGMC"];
                    myFields_RLDC[17] = rs_RLDC.Fields["TYMC"];
                    myFields_RLDC[18] = rs_RLDC.Fields["ZJ"];
                    myFields_RLDC[19] = rs_RLDC.Fields["RLLX"];
                    myFields_RLDC[20] = rs_RLDC.Fields["RLDC_CDDMSXZGXSCS"];
                    myFields_RLDC[21] = rs_RLDC.Fields["RLDC_CQPBCGZYL"];
                    myFields_RLDC[22] = rs_RLDC.Fields["RLDC_CQPRJ"];
                    myFields_RLDC[23] = rs_RLDC.Fields["RLDC_DDGLMD"];
                    myFields_RLDC[24] = rs_RLDC.Fields["RLDC_CQPLX"];
                    myFields_RLDC[25] = rs_RLDC.Fields["RLDC_DDHHJSTJXXDCZBNL"];
                    myFields_RLDC[26] = rs_RLDC.Fields["RLDC_DLXDCZZL"];
                    myFields_RLDC[27] = rs_RLDC.Fields["RLDC_QDDJEDGL"];
                    myFields_RLDC[28] = rs_RLDC.Fields["RLDC_QDDJFZNJ"];
                    myFields_RLDC[29] = rs_RLDC.Fields["RLDC_QDDJLX"];
                    myFields_RLDC[30] = rs_RLDC.Fields["RLDC_RLLX"];
                    myFields_RLDC[31] = rs_RLDC.Fields["RLDC_ZHGKHQL"];
                    myFields_RLDC[32] = rs_RLDC.Fields["RLDC_ZHGKXSLC"];
                    myFields_RLDC[33] = rs_RLDC.Fields["CREATETIME"];
                    myFields_RLDC[34] = rs_RLDC.Fields["UPDATETIME"];
                    myFields_RLDC[35] = rs_RLDC.Fields["RLDC_RLDCXTEDGL"];
                    #endregion
                   string error = string.Empty;
                   foreach (DataRow dr in dt.Rows)
                   {
                       error = VerifyData(dr, tdr, "UPDATE");
                       if (!string.IsNullOrEmpty(error))
                       {
                           msg += error;
                       }
                       else
                       {
                           if (rlzl.Equals(CTNY))
                           {
                               #region 传统能源
                               //先删除
                               string sql = "DELETE FROM CTNY_MAIN WHERE UNIQUE_CODE = '" + dr["UNIQUE_CODE"] + "'";
                               AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);

                               //后插入新的
                               try
                               {
                                   rs_CTNY.AddNew();
                                   myFields[0].Value = dr["UNIQUE_CODE"];
                                   myFields[1].Value = dr["QCSCQY"];
                                   myFields[2].Value = dr["JYBGBH"];
                                   myFields[3].Value = dr["JKQCZJXS"];
                                   myFields[4].Value = dr["CLXH"];
                                   myFields[5].Value = dr["HGSPBM"];
                                   myFields[6].Value = dr["CLZL"];
                                   myFields[7].Value = dr["YYC"];
                                   myFields[8].Value = dr["QDXS"];
                                   myFields[9].Value = dr["ZWPS"];
                                   myFields[10].Value = dr["ZCZBZL"];
                                   myFields[11].Value = dr["ZDSJZZL"];
                                   myFields[12].Value = dr["ZGCS"];
                                   myFields[13].Value = dr["EDZK"];
                                   myFields[14].Value = dr["LTGG"];
                                   myFields[15].Value = dr["LJ"];
                                   myFields[16].Value = dr["JYJGMC"];
                                   myFields[17].Value = dr["TYMC"];
                                   myFields[18].Value = dr["ZJ"];
                                   myFields[19].Value = dr["RLLX"];
                                   myFields[20].Value = dr["CT_BSQXS"];
                                   myFields[21].Value = dr["CT_EDGL"];
                                   myFields[22].Value = dr["CT_FDJXH"];
                                   myFields[23].Value = dr["CT_JGL"];
                                   myFields[24].Value = dr["CT_PL"];
                                   myFields[25].Value = dr["CT_QGS"];
                                   myFields[26].Value = dr["CT_QTXX"];
                                   myFields[27].Value = dr["CT_SJGKRLXHL"];
                                   myFields[28].Value = dr["CT_SQGKRLXHL"];
                                   myFields[29].Value = dr["CT_ZHGKCO2PFL"];
                                   myFields[30].Value = dr["CT_ZHGKRLXHL"];
                                   myFields[31].Value = dr["CT_BSQDWS"];
                                   myFields[32].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields[33].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   rs_CTNY.Update();
                                   succCount++;
                               }
                               catch(Exception ex)
                               {
                                   msg += ex.Message + "\r\n";
                               }
                               #endregion
                           }
                           else if (rlzl.Equals(FCDSHHDL))
                           {
                               #region 非插电式混合动力
                               try
                               {
                                   //先删除
                                   string sql = "DELETE FROM FCDS_MAIN WHERE UNIQUE_CODE = '" + dr["UNIQUE_CODE"] + "'";
                                   AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);

                                   //后插入新的
                                   #region insert
                                   rs_FCDS.AddNew();
                                   myFields_FCDS[0].Value = dr["UNIQUE_CODE"];
                                   myFields_FCDS[1].Value = dr["QCSCQY"];
                                   myFields_FCDS[2].Value = dr["JYBGBH"];
                                   myFields_FCDS[3].Value = dr["JKQCZJXS"];
                                   myFields_FCDS[4].Value = dr["CLXH"];
                                   myFields_FCDS[5].Value = dr["HGSPBM"];
                                   myFields_FCDS[6].Value = dr["CLZL"];
                                   myFields_FCDS[7].Value = dr["YYC"];
                                   myFields_FCDS[8].Value = dr["QDXS"];
                                   myFields_FCDS[9].Value = dr["ZWPS"];
                                   myFields_FCDS[10].Value = dr["ZCZBZL"];
                                   myFields_FCDS[11].Value = dr["ZDSJZZL"];
                                   myFields_FCDS[12].Value = dr["ZGCS"];
                                   myFields_FCDS[13].Value = dr["EDZK"];
                                   myFields_FCDS[14].Value = dr["LTGG"];
                                   myFields_FCDS[15].Value = dr["LJ"];
                                   myFields_FCDS[16].Value = dr["JYJGMC"];
                                   myFields_FCDS[17].Value = dr["TYMC"];
                                   myFields_FCDS[18].Value = dr["ZJ"];
                                   myFields_FCDS[19].Value = dr["RLLX"];
                                   myFields_FCDS[20].Value = dr["FCDS_HHDL_BSQDWS"];
                                   myFields_FCDS[21].Value = dr["FCDS_HHDL_BSQXS"];
                                   myFields_FCDS[22].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                                   myFields_FCDS[23].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                                   myFields_FCDS[24].Value = dr["FCDS_HHDL_DLXDCBNL"];
                                   myFields_FCDS[25].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                                   myFields_FCDS[26].Value = dr["FCDS_HHDL_DLXDCZZL"];
                                   myFields_FCDS[27].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                                   myFields_FCDS[28].Value = dr["FCDS_HHDL_EDGL"];
                                   myFields_FCDS[29].Value = dr["FCDS_HHDL_FDJXH"];
                                   myFields_FCDS[30].Value = dr["FCDS_HHDL_HHDLJGXS"];
                                   myFields_FCDS[31].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                                   myFields_FCDS[32].Value = dr["FCDS_HHDL_JGL"];
                                   myFields_FCDS[33].Value = dr["FCDS_HHDL_PL"];
                                   myFields_FCDS[34].Value = dr["FCDS_HHDL_QDDJEDGL"];
                                   myFields_FCDS[35].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                                   myFields_FCDS[36].Value = dr["FCDS_HHDL_QDDJLX"];
                                   myFields_FCDS[37].Value = dr["FCDS_HHDL_QGS"];
                                   myFields_FCDS[38].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                                   myFields_FCDS[39].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                                   myFields_FCDS[40].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                                   myFields_FCDS[41].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                                   myFields_FCDS[42].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                                   myFields_FCDS[43].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields_FCDS[44].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   rs_FCDS.Update();
                                   succCount++;

                                   #endregion
                               }
                               catch (Exception ex)
                               {
                                   msg += ex.Message + "\r\n";
                               }
                               #endregion
                           }
                           else if (rlzl.Equals(CDSHHDL))
                           {
                               #region 插电式混合动力
                               try
                               {
                                   //先删除
                                   string sql = "DELETE FROM CDS_MAIN WHERE UNIQUE_CODE = '" + dr["UNIQUE_CODE"] + "'";
                                   AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);

                                   //后插入新的
                                   #region insert
                                   rs_CDS.AddNew();
                                   myFields_CDS[0].Value = dr["UNIQUE_CODE"];
                                   myFields_CDS[1].Value = dr["QCSCQY"];
                                   myFields_CDS[2].Value = dr["JYBGBH"];
                                   myFields_CDS[3].Value = dr["JKQCZJXS"];
                                   myFields_CDS[4].Value = dr["CLXH"];
                                   myFields_CDS[5].Value = dr["HGSPBM"];
                                   myFields_CDS[6].Value = dr["CLZL"];
                                   myFields_CDS[7].Value = dr["YYC"];
                                   myFields_CDS[8].Value = dr["QDXS"];
                                   myFields_CDS[9].Value = dr["ZWPS"];
                                   myFields_CDS[10].Value = dr["ZCZBZL"];
                                   myFields_CDS[11].Value = dr["ZDSJZZL"];
                                   myFields_CDS[12].Value = dr["ZGCS"];
                                   myFields_CDS[13].Value = dr["EDZK"];
                                   myFields_CDS[14].Value = dr["LTGG"];
                                   myFields_CDS[15].Value = dr["LJ"];
                                   myFields_CDS[16].Value = dr["JYJGMC"];
                                   myFields_CDS[17].Value = dr["TYMC"];
                                   myFields_CDS[18].Value = dr["ZJ"];
                                   myFields_CDS[19].Value = dr["RLLX"];
                                   myFields_CDS[20].Value = dr["CDS_HHDL_BSQDWS"];
                                   myFields_CDS[21].Value = dr["CDS_HHDL_BSQXS"];
                                   myFields_CDS[22].Value = dr["CDS_HHDL_CDDMSXZGCS"];
                                   myFields_CDS[23].Value = dr["CDS_HHDL_CDDMSXZHGKXSLC"];
                                   myFields_CDS[24].Value = dr["CDS_HHDL_DLXDCBNL"];
                                   myFields_CDS[25].Value = dr["CDS_HHDL_DLXDCZBCDY"];
                                   myFields_CDS[26].Value = dr["CDS_HHDL_DLXDCZZL"];
                                   myFields_CDS[27].Value = dr["CDS_HHDL_DLXDCZZNL"];
                                   myFields_CDS[28].Value = dr["CDS_HHDL_EDGL"];
                                   myFields_CDS[29].Value = dr["CDS_HHDL_FDJXH"];
                                   myFields_CDS[30].Value = dr["CDS_HHDL_HHDLJGXS"];
                                   myFields_CDS[31].Value = dr["CDS_HHDL_HHDLZDDGLB"];
                                   myFields_CDS[32].Value = dr["CDS_HHDL_JGL"];
                                   myFields_CDS[33].Value = dr["CDS_HHDL_PL"];
                                   myFields_CDS[34].Value = dr["CDS_HHDL_QDDJEDGL"];
                                   myFields_CDS[35].Value = dr["CDS_HHDL_QDDJFZNJ"];
                                   myFields_CDS[36].Value = dr["CDS_HHDL_QDDJLX"];
                                   myFields_CDS[37].Value = dr["CDS_HHDL_QGS"];
                                   myFields_CDS[38].Value = dr["CDS_HHDL_XSMSSDXZGN"];
                                   myFields_CDS[39].Value = dr["CDS_HHDL_ZHGKDNXHL"];
                                   myFields_CDS[40].Value = dr["CDS_HHDL_ZHGKRLXHL"];
                                   myFields_CDS[41].Value = dr["CDS_HHDL_ZHKGCO2PL"];
                                   myFields_CDS[42].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields_CDS[43].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields_CDS[44].Value = dr["CDS_HHDL_TJASYZDNXHL"];
                                   myFields_CDS[45].Value = dr["CDS_HHDL_TJBSYZDNXHL"];
                                   rs_CDS.Update();
                                   succCount++;

                                   #endregion
                               }
                               catch (Exception ex)
                               {
                                   msg += ex.Message + "\r\n";
                               }
                               #endregion
                           }
                           else if (rlzl.Equals(CDD))
                           {
                               #region 纯电动
                               try
                               {
                                   //先删除
                                   string sql = "DELETE FROM CDD_MAIN WHERE UNIQUE_CODE = '" + dr["UNIQUE_CODE"] + "'";
                                   AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);

                                   //后插入新的
                                   #region insert

                                   rs_CDD.AddNew();
                                   myFields_CDD[0].Value = dr["UNIQUE_CODE"];
                                   myFields_CDD[1].Value = dr["QCSCQY"];
                                   myFields_CDD[2].Value = dr["JYBGBH"];
                                   myFields_CDD[3].Value = dr["JKQCZJXS"];
                                   myFields_CDD[4].Value = dr["CLXH"];
                                   myFields_CDD[5].Value = dr["HGSPBM"];
                                   myFields_CDD[6].Value = dr["CLZL"];
                                   myFields_CDD[7].Value = dr["YYC"];
                                   myFields_CDD[8].Value = dr["QDXS"];
                                   myFields_CDD[9].Value = dr["ZWPS"];
                                   myFields_CDD[10].Value = dr["ZCZBZL"];
                                   myFields_CDD[11].Value = dr["ZDSJZZL"];
                                   myFields_CDD[12].Value = dr["ZGCS"];
                                   myFields_CDD[13].Value = dr["EDZK"];
                                   myFields_CDD[14].Value = dr["LTGG"];
                                   myFields_CDD[15].Value = dr["LJ"];
                                   myFields_CDD[16].Value = dr["JYJGMC"];
                                   myFields_CDD[17].Value = dr["TYMC"];
                                   myFields_CDD[18].Value = dr["ZJ"];
                                   myFields_CDD[19].Value = dr["RLLX"];
                                   myFields_CDD[20].Value = dr["CDD_DDQC30FZZGCS"];
                                   myFields_CDD[21].Value = dr["CDD_DDXDCZZLYZCZBZLDBZ"];
                                   myFields_CDD[22].Value = dr["CDD_DLXDCBNL"];
                                   myFields_CDD[23].Value = dr["CDD_DLXDCZBCDY"];
                                   myFields_CDD[24].Value = dr["CDD_DLXDCZEDNL"];
                                   myFields_CDD[25].Value = dr["CDD_DLXDCZZL"];
                                   myFields_CDD[26].Value = dr["CDD_QDDJEDGL"];
                                   myFields_CDD[27].Value = dr["CDD_QDDJFZNJ"];
                                   myFields_CDD[28].Value = dr["CDD_QDDJLX"];
                                   myFields_CDD[29].Value = dr["CDD_ZHGKDNXHL"];
                                   myFields_CDD[30].Value = dr["CDD_ZHGKXSLC"];
                                   myFields_CDD[31].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields_CDD[32].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);

                                   rs_CDD.Update();
                                   succCount++;

                                   #endregion
                               }
                               catch (Exception ex)
                               {
                                   msg += ex.Message + "\r\n";
                               }
                               #endregion
                           }
                           else if (rlzl.Equals(RLDC))
                           {
                               #region 燃料电池
                               try
                               {
                                   //先删除
                                   string sql = "DELETE FROM RLDC_MAIN WHERE UNIQUE_CODE = '" + dr["UNIQUE_CODE"] + "'";
                                   AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);

                                   //后插入新的
                                   #region insert
                                   rs_RLDC.AddNew();
                                   myFields_RLDC[0].Value = dr["UNIQUE_CODE"];
                                   myFields_RLDC[1].Value = dr["QCSCQY"];
                                   myFields_RLDC[2].Value = dr["JYBGBH"];
                                   myFields_RLDC[3].Value = dr["JKQCZJXS"];
                                   myFields_RLDC[4].Value = dr["CLXH"];
                                   myFields_RLDC[5].Value = dr["HGSPBM"];
                                   myFields_RLDC[6].Value = dr["CLZL"];
                                   myFields_RLDC[7].Value = dr["YYC"];
                                   myFields_RLDC[8].Value = dr["QDXS"];
                                   myFields_RLDC[9].Value = dr["ZWPS"];
                                   myFields_RLDC[10].Value = dr["ZCZBZL"];
                                   myFields_RLDC[11].Value = dr["ZDSJZZL"];
                                   myFields_RLDC[12].Value = dr["ZGCS"];
                                   myFields_RLDC[13].Value = dr["EDZK"];
                                   myFields_RLDC[14].Value = dr["LTGG"];
                                   myFields_RLDC[15].Value = dr["LJ"];
                                   myFields_RLDC[16].Value = dr["JYJGMC"];
                                   myFields_RLDC[17].Value = dr["TYMC"];
                                   myFields_RLDC[18].Value = dr["ZJ"];
                                   myFields_RLDC[19].Value = dr["RLLX"];
                                   myFields_RLDC[20].Value = dr["RLDC_CDDMSXZGXSCS"];
                                   myFields_RLDC[21].Value = dr["RLDC_CQPBCGZYL"];
                                   myFields_RLDC[22].Value = dr["RLDC_CQPRJ"];
                                   myFields_RLDC[23].Value = dr["RLDC_DDGLMD"];
                                   myFields_RLDC[24].Value = dr["RLDC_CQPLX"];
                                   myFields_RLDC[25].Value = dr["RLDC_DDHHJSTJXXDCZBNL"];
                                   myFields_RLDC[26].Value = dr["RLDC_DLXDCZZL"];
                                   myFields_RLDC[27].Value = dr["RLDC_QDDJEDGL"];
                                   myFields_RLDC[28].Value = dr["RLDC_QDDJFZNJ"];
                                   myFields_RLDC[29].Value = dr["RLDC_QDDJLX"];
                                   myFields_RLDC[30].Value = dr["RLDC_RLLX"];
                                   myFields_RLDC[31].Value = dr["RLDC_ZHGKHQL"];
                                   myFields_RLDC[32].Value = dr["RLDC_ZHGKXSLC"];
                                   myFields_RLDC[33].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields_RLDC[34].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                   myFields_RLDC[35].Value = dr["RLDC_RLDCXTEDGL"];
                                   rs_RLDC.Update();

                                   succCount++;

                                   #endregion
                               }
                               catch (Exception ex)
                               {
                                   msg += ex.Message + "\r\n";
                               }
                               #endregion
                           }
                       }
                   }
                   rs_CDD.Close();
                   rs_CDS.Close();
                   rs_CTNY.Close();
                   rs_FCDS.Close();
                   rs_RLDC.Close();
                }
                db.Close();
            }
            catch(Exception ex)
            {
                msg += ex.Message + "\r\n";
            }
            return succCount;
        }
        public int UpdMainData(DataTable dt, string rlzl, ref string msg)
        {
            int succCount = 0;
            if (string.IsNullOrEmpty(msg))
            {
                msg = string.Empty;
            }
            try
            {
                // 转换表头（用户模板中的表头转为数据库列名）
                DataRow[] tdr = checkData.Select("FUEL_TYPE='" + rlzl + "' and STATUS=1");
                if (dt != null && dt.Rows.Count > 0)
                {
                    string error = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {
                        error = VerifyData(dr, tdr, "UPDATE");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            if (rlzl.Equals(CTNY))
                            {
                                #region 传统能源
                                try
                                {
                                    #region update
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update CTNY_MAIN set ");
                                    strSql.Append("QCSCQY=@QCSCQY,");
                                    strSql.Append("JYBGBH=@JYBGBH,");
                                    strSql.Append("JKQCZJXS=@JKQCZJXS,");
                                    strSql.Append("CLXH=@CLXH,");
                                    strSql.Append("HGSPBM=@HGSPBM,");
                                    strSql.Append("CLZL=@CLZL,");
                                    strSql.Append("YYC=@YYC,");
                                    strSql.Append("QDXS=@QDXS,");
                                    strSql.Append("ZWPS=@ZWPS,");
                                    strSql.Append("ZCZBZL=@ZCZBZL,");
                                    strSql.Append("ZDSJZZL=@ZDSJZZL,");
                                    strSql.Append("ZGCS=@ZGCS,");
                                    strSql.Append("EDZK=@EDZK,");
                                    strSql.Append("LTGG=@LTGG,");
                                    strSql.Append("LJ=@LJ,");
                                    strSql.Append("JYJGMC=@JYJGMC,");
                                    strSql.Append("TYMC=@TYMC,");
                                    strSql.Append("ZJ=@ZJ,");
                                    strSql.Append("RLLX=@RLLX,");
                                    strSql.Append("CT_BSQXS=@CT_BSQXS,");
                                    strSql.Append("CT_EDGL=@CT_EDGL,");
                                    strSql.Append("CT_FDJXH=@CT_FDJXH,");
                                    strSql.Append("CT_JGL=@CT_JGL,");
                                    strSql.Append("CT_PL=@CT_PL,");
                                    strSql.Append("CT_QGS=@CT_QGS,");
                                    strSql.Append("CT_QTXX=@CT_QTXX,");
                                    strSql.Append("CT_SJGKRLXHL=@CT_SJGKRLXHL,");
                                    strSql.Append("CT_SQGKRLXHL=@CT_SQGKRLXHL,");
                                    strSql.Append("CT_ZHGKCO2PFL=@CT_ZHGKCO2PFL,");
                                    strSql.Append("CT_ZHGKRLXHL=@CT_ZHGKRLXHL,");
                                    strSql.Append("CT_BSQDWS=@CT_BSQDWS,");
                                    strSql.Append("UPDATETIME=@UPDATETIME");
                                    strSql.Append(" where UNIQUE_CODE=@UNIQUE_CODE ");
                                    OleDbParameter[] parameters = {
                                                      new OleDbParameter("@QCSCQY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@YYC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@QDXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                                                      new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                                                      new OleDbParameter("@LJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@TYMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLLX", OleDbType.VarChar,200),
                                                      new OleDbParameter("@CT_BSQXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_EDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_FDJXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_JGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_PL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_QGS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_QTXX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_SJGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_SQGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_ZHGKCO2PFL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_ZHGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CT_BSQDWS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@UPDATETIME", OleDbType.Date),
                                                      new OleDbParameter("@UNIQUE_CODE", OleDbType.VarChar,255)
                                    };
                                    parameters[0].Value = dr["QCSCQY"];
                                    parameters[1].Value = dr["JYBGBH"];
                                    parameters[2].Value = dr["JKQCZJXS"];
                                    parameters[3].Value = dr["CLXH"];
                                    parameters[4].Value = dr["HGSPBM"];
                                    parameters[5].Value = dr["CLZL"];
                                    parameters[6].Value = dr["YYC"];
                                    parameters[7].Value = dr["QDXS"];
                                    parameters[8].Value = dr["ZWPS"];
                                    parameters[9].Value = dr["ZCZBZL"];
                                    parameters[10].Value = dr["ZDSJZZL"];
                                    parameters[11].Value = dr["ZGCS"];
                                    parameters[12].Value = dr["EDZK"];
                                    parameters[13].Value = dr["LTGG"];
                                    parameters[14].Value = dr["LJ"];
                                    parameters[15].Value = dr["JYJGMC"];
                                    parameters[16].Value = dr["TYMC"];
                                    parameters[17].Value = dr["ZJ"];
                                    parameters[18].Value = dr["RLLX"];
                                    parameters[19].Value = dr["CT_BSQXS"];
                                    parameters[20].Value = dr["CT_EDGL"];
                                    parameters[21].Value = dr["CT_FDJXH"];
                                    parameters[22].Value = dr["CT_JGL"];
                                    parameters[23].Value = dr["CT_PL"];
                                    parameters[24].Value = dr["CT_QGS"];
                                    parameters[25].Value = dr["CT_QTXX"];
                                    parameters[26].Value = dr["CT_SJGKRLXHL"];
                                    parameters[27].Value = dr["CT_SQGKRLXHL"];
                                    parameters[28].Value = dr["CT_ZHGKCO2PFL"];
                                    parameters[29].Value = dr["CT_ZHGKRLXHL"];
                                    parameters[30].Value = dr["CT_BSQDWS"];
                                    parameters[31].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[32].Value = dr["UNIQUE_CODE"];
                                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(FCDSHHDL))
                            {
                                #region 非插电式混合动力
                                try
                                {
                                    #region update
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update FCDS_MAIN set ");
                                    strSql.Append("QCSCQY=@QCSCQY,");
                                    strSql.Append("JYBGBH=@JYBGBH,");
                                    strSql.Append("JKQCZJXS=@JKQCZJXS,");
                                    strSql.Append("CLXH=@CLXH,");
                                    strSql.Append("HGSPBM=@HGSPBM,");
                                    strSql.Append("CLZL=@CLZL,");
                                    strSql.Append("YYC=@YYC,");
                                    strSql.Append("QDXS=@QDXS,");
                                    strSql.Append("ZWPS=@ZWPS,");
                                    strSql.Append("ZCZBZL=@ZCZBZL,");
                                    strSql.Append("ZDSJZZL=@ZDSJZZL,");
                                    strSql.Append("ZGCS=@ZGCS,");
                                    strSql.Append("EDZK=@EDZK,");
                                    strSql.Append("LTGG=@LTGG,");
                                    strSql.Append("LJ=@LJ,");
                                    strSql.Append("JYJGMC=@JYJGMC,");
                                    strSql.Append("TYMC=@TYMC,");
                                    strSql.Append("ZJ=@ZJ,");
                                    strSql.Append("RLLX=@RLLX,");
                                    strSql.Append("FCDS_HHDL_BSQDWS=@FCDS_HHDL_BSQDWS,");
                                    strSql.Append("FCDS_HHDL_BSQXS=@FCDS_HHDL_BSQXS,");
                                    strSql.Append("FCDS_HHDL_CDDMSXZGCS=@FCDS_HHDL_CDDMSXZGCS,");
                                    strSql.Append("FCDS_HHDL_CDDMSXZHGKXSLC=@FCDS_HHDL_CDDMSXZHGKXSLC,");
                                    strSql.Append("FCDS_HHDL_DLXDCBNL=@FCDS_HHDL_DLXDCBNL,");
                                    strSql.Append("FCDS_HHDL_DLXDCZBCDY=@FCDS_HHDL_DLXDCZBCDY,");
                                    strSql.Append("FCDS_HHDL_DLXDCZZL=@FCDS_HHDL_DLXDCZZL,");
                                    strSql.Append("FCDS_HHDL_DLXDCZZNL=@FCDS_HHDL_DLXDCZZNL,");
                                    strSql.Append("FCDS_HHDL_EDGL=@FCDS_HHDL_EDGL,");
                                    strSql.Append("FCDS_HHDL_FDJXH=@FCDS_HHDL_FDJXH,");
                                    strSql.Append("FCDS_HHDL_HHDLJGXS=@FCDS_HHDL_HHDLJGXS,");
                                    strSql.Append("FCDS_HHDL_HHDLZDDGLB=@FCDS_HHDL_HHDLZDDGLB,");
                                    strSql.Append("FCDS_HHDL_JGL=@FCDS_HHDL_JGL,");
                                    strSql.Append("FCDS_HHDL_PL=@FCDS_HHDL_PL,");
                                    strSql.Append("FCDS_HHDL_QDDJEDGL=@FCDS_HHDL_QDDJEDGL,");
                                    strSql.Append("FCDS_HHDL_QDDJFZNJ=@FCDS_HHDL_QDDJFZNJ,");
                                    strSql.Append("FCDS_HHDL_QDDJLX=@FCDS_HHDL_QDDJLX,");
                                    strSql.Append("FCDS_HHDL_QGS=@FCDS_HHDL_QGS,");
                                    strSql.Append("FCDS_HHDL_SJGKRLXHL=@FCDS_HHDL_SJGKRLXHL,");
                                    strSql.Append("FCDS_HHDL_SQGKRLXHL=@FCDS_HHDL_SQGKRLXHL,");
                                    strSql.Append("FCDS_HHDL_XSMSSDXZGN=@FCDS_HHDL_XSMSSDXZGN,");
                                    strSql.Append("FCDS_HHDL_ZHGKRLXHL=@FCDS_HHDL_ZHGKRLXHL,");
                                    strSql.Append("FCDS_HHDL_ZHKGCO2PL=@FCDS_HHDL_ZHKGCO2PL,");
                                    strSql.Append("UPDATETIME=@UPDATETIME");
                                    strSql.Append(" where UNIQUE_CODE=@UNIQUE_CODE ");
                                    OleDbParameter[] parameters = {
                                                      new OleDbParameter("@QCSCQY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@YYC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@QDXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                                                      new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                                                      new OleDbParameter("@LJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@TYMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLLX", OleDbType.VarChar,200),
                                                      new OleDbParameter("@FCDS_HHDL_BSQDWS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_BSQXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_DLXDCBNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_DLXDCZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_EDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_FDJXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_HHDLJGXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_JGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_PL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_QDDJEDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_QDDJLX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_QGS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@UPDATETIME", OleDbType.Date),
                                                      new OleDbParameter("@UNIQUE_CODE", OleDbType.VarChar,255)
                                    };
                                    parameters[0].Value = dr["QCSCQY"];
                                    parameters[1].Value = dr["JYBGBH"];
                                    parameters[2].Value = dr["JKQCZJXS"];
                                    parameters[3].Value = dr["CLXH"];
                                    parameters[4].Value = dr["HGSPBM"];
                                    parameters[5].Value = dr["CLZL"];
                                    parameters[6].Value = dr["YYC"];
                                    parameters[7].Value = dr["QDXS"];
                                    parameters[8].Value = dr["ZWPS"];
                                    parameters[9].Value = dr["ZCZBZL"];
                                    parameters[10].Value = dr["ZDSJZZL"];
                                    parameters[11].Value = dr["ZGCS"];
                                    parameters[12].Value = dr["EDZK"];
                                    parameters[13].Value = dr["LTGG"];
                                    parameters[14].Value = dr["LJ"];
                                    parameters[15].Value = dr["JYJGMC"];
                                    parameters[16].Value = dr["TYMC"];
                                    parameters[17].Value = dr["ZJ"];
                                    parameters[18].Value = dr["RLLX"];
                                    parameters[19].Value = dr["FCDS_HHDL_BSQDWS"];
                                    parameters[20].Value = dr["FCDS_HHDL_BSQXS"];
                                    parameters[21].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                                    parameters[22].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                                    parameters[23].Value = dr["FCDS_HHDL_DLXDCBNL"];
                                    parameters[24].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                                    parameters[25].Value = dr["FCDS_HHDL_DLXDCZZL"];
                                    parameters[26].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                                    parameters[27].Value = dr["FCDS_HHDL_EDGL"];
                                    parameters[28].Value = dr["FCDS_HHDL_FDJXH"];
                                    parameters[29].Value = dr["FCDS_HHDL_HHDLJGXS"];
                                    parameters[30].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                                    parameters[31].Value = dr["FCDS_HHDL_JGL"];
                                    parameters[32].Value = dr["FCDS_HHDL_PL"];
                                    parameters[33].Value = dr["FCDS_HHDL_QDDJEDGL"];
                                    parameters[34].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                                    parameters[35].Value = dr["FCDS_HHDL_QDDJLX"];
                                    parameters[36].Value = dr["FCDS_HHDL_QGS"];
                                    parameters[37].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                                    parameters[38].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                                    parameters[39].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                                    parameters[40].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                                    parameters[41].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                                    parameters[42].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[43].Value = dr["UNIQUE_CODE"];
                                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(CDSHHDL))
                            {
                                #region 插电式混合动力
                                try
                                {
                                    #region update
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update CDS_MAIN set ");
                                    strSql.Append("QCSCQY=@QCSCQY,");
                                    strSql.Append("JYBGBH=@JYBGBH,");
                                    strSql.Append("JKQCZJXS=@JKQCZJXS,");
                                    strSql.Append("CLXH=@CLXH,");
                                    strSql.Append("HGSPBM=@HGSPBM,");
                                    strSql.Append("CLZL=@CLZL,");
                                    strSql.Append("YYC=@YYC,");
                                    strSql.Append("QDXS=@QDXS,");
                                    strSql.Append("ZWPS=@ZWPS,");
                                    strSql.Append("ZCZBZL=@ZCZBZL,");
                                    strSql.Append("ZDSJZZL=@ZDSJZZL,");
                                    strSql.Append("ZGCS=@ZGCS,");
                                    strSql.Append("EDZK=@EDZK,");
                                    strSql.Append("LTGG=@LTGG,");
                                    strSql.Append("LJ=@LJ,");
                                    strSql.Append("JYJGMC=@JYJGMC,");
                                    strSql.Append("TYMC=@TYMC,");
                                    strSql.Append("ZJ=@ZJ,");
                                    strSql.Append("RLLX=@RLLX,");
                                    strSql.Append("CDS_HHDL_BSQDWS=@CDS_HHDL_BSQDWS,");
                                    strSql.Append("CDS_HHDL_BSQXS=@CDS_HHDL_BSQXS,");
                                    strSql.Append("CDS_HHDL_CDDMSXZGCS=@CDS_HHDL_CDDMSXZGCS,");
                                    strSql.Append("CDS_HHDL_CDDMSXZHGKXSLC=@CDS_HHDL_CDDMSXZHGKXSLC,");
                                    strSql.Append("CDS_HHDL_DLXDCBNL=@CDS_HHDL_DLXDCBNL,");
                                    strSql.Append("CDS_HHDL_DLXDCZBCDY=@CDS_HHDL_DLXDCZBCDY,");
                                    strSql.Append("CDS_HHDL_DLXDCZZL=@CDS_HHDL_DLXDCZZL,");
                                    strSql.Append("CDS_HHDL_DLXDCZZNL=@CDS_HHDL_DLXDCZZNL,");
                                    strSql.Append("CDS_HHDL_EDGL=@CDS_HHDL_EDGL,");
                                    strSql.Append("CDS_HHDL_FDJXH=@CDS_HHDL_FDJXH,");
                                    strSql.Append("CDS_HHDL_HHDLJGXS=@CDS_HHDL_HHDLJGXS,");
                                    strSql.Append("CDS_HHDL_HHDLZDDGLB=@CDS_HHDL_HHDLZDDGLB,");
                                    strSql.Append("CDS_HHDL_JGL=@CDS_HHDL_JGL,");
                                    strSql.Append("CDS_HHDL_PL=@CDS_HHDL_PL,");
                                    strSql.Append("CDS_HHDL_QDDJEDGL=@CDS_HHDL_QDDJEDGL,");
                                    strSql.Append("CDS_HHDL_QDDJFZNJ=@CDS_HHDL_QDDJFZNJ,");
                                    strSql.Append("CDS_HHDL_QDDJLX=@CDS_HHDL_QDDJLX,");
                                    strSql.Append("CDS_HHDL_QGS=@CDS_HHDL_QGS,");
                                    strSql.Append("CDS_HHDL_XSMSSDXZGN=@CDS_HHDL_XSMSSDXZGN,");
                                    strSql.Append("CDS_HHDL_ZHGKDNXHL=@CDS_HHDL_ZHGKDNXHL,");
                                    strSql.Append("CDS_HHDL_ZHGKRLXHL=@CDS_HHDL_ZHGKRLXHL,");
                                    strSql.Append("CDS_HHDL_ZHKGCO2PL=@CDS_HHDL_ZHKGCO2PL,");
                                    strSql.Append("CDS_HHDL_TJASYZDNXHL=@CDS_HHDL_TJASYZDNXHL,");
                                    strSql.Append("CDS_HHDL_TJBSYZDNXHL=@CDS_HHDL_TJBSYZDNXHL,");
                                    strSql.Append("UPDATETIME=@UPDATETIME");
                                    strSql.Append(" where UNIQUE_CODE=@UNIQUE_CODE ");
                                    OleDbParameter[] parameters = {
                                                      new OleDbParameter("@QCSCQY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@YYC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@QDXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                                                      new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                                                      new OleDbParameter("@LJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@TYMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLLX", OleDbType.VarChar,200),
                                                      new OleDbParameter("@CDS_HHDL_BSQDWS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_BSQXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_CDDMSXZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_CDDMSXZHGKXSLC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_DLXDCBNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_DLXDCZBCDY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_DLXDCZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_DLXDCZZNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_EDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_FDJXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_HHDLJGXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_HHDLZDDGLB", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_JGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_PL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_QDDJEDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_QDDJFZNJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_QDDJLX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_QGS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_XSMSSDXZGN", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_ZHGKDNXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_ZHGKRLXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDS_HHDL_ZHKGCO2PL", OleDbType.VarChar,255),
                                        new OleDbParameter("@CDS_HHDL_TJASYZDNXHL",OleDbType.VarChar,255),
                                        new OleDbParameter("@CDS_HHDL_TJBSYZDNXHL",OleDbType.VarChar,255),
                                                      new OleDbParameter("@UPDATETIME", OleDbType.Date),
                                                      new OleDbParameter("@UNIQUE_CODE", OleDbType.VarChar,255)
                                       
                                    };
                                    parameters[0].Value = dr["QCSCQY"];
                                    parameters[1].Value = dr["JYBGBH"];
                                    parameters[2].Value = dr["JKQCZJXS"];
                                    parameters[3].Value = dr["CLXH"];
                                    parameters[4].Value = dr["HGSPBM"];
                                    parameters[5].Value = dr["CLZL"];
                                    parameters[6].Value = dr["YYC"];
                                    parameters[7].Value = dr["QDXS"];
                                    parameters[8].Value = dr["ZWPS"];
                                    parameters[9].Value = dr["ZCZBZL"];
                                    parameters[10].Value = dr["ZDSJZZL"];
                                    parameters[11].Value = dr["ZGCS"];
                                    parameters[12].Value = dr["EDZK"];
                                    parameters[13].Value = dr["LTGG"];
                                    parameters[14].Value = dr["LJ"];
                                    parameters[15].Value = dr["JYJGMC"];
                                    parameters[16].Value = dr["TYMC"];
                                    parameters[17].Value = dr["ZJ"];
                                    parameters[18].Value = dr["RLLX"];
                                    parameters[19].Value = dr["CDS_HHDL_BSQDWS"];
                                    parameters[20].Value = dr["CDS_HHDL_BSQXS"];
                                    parameters[21].Value = dr["CDS_HHDL_CDDMSXZGCS"];
                                    parameters[22].Value = dr["CDS_HHDL_CDDMSXZHGKXSLC"];
                                    parameters[23].Value = dr["CDS_HHDL_DLXDCBNL"];
                                    parameters[24].Value = dr["CDS_HHDL_DLXDCZBCDY"];
                                    parameters[25].Value = dr["CDS_HHDL_DLXDCZZL"];
                                    parameters[26].Value = dr["CDS_HHDL_DLXDCZZNL"];
                                    parameters[27].Value = dr["CDS_HHDL_EDGL"];
                                    parameters[28].Value = dr["CDS_HHDL_FDJXH"];
                                    parameters[29].Value = dr["CDS_HHDL_HHDLJGXS"];
                                    parameters[30].Value = dr["CDS_HHDL_HHDLZDDGLB"];
                                    parameters[31].Value = dr["CDS_HHDL_JGL"];
                                    parameters[32].Value = dr["CDS_HHDL_PL"];
                                    parameters[33].Value = dr["CDS_HHDL_QDDJEDGL"];
                                    parameters[34].Value = dr["CDS_HHDL_QDDJFZNJ"];
                                    parameters[35].Value = dr["CDS_HHDL_QDDJLX"];
                                    parameters[36].Value = dr["CDS_HHDL_QGS"];
                                    parameters[37].Value = dr["CDS_HHDL_XSMSSDXZGN"];
                                    parameters[38].Value = dr["CDS_HHDL_ZHGKDNXHL"];
                                    parameters[39].Value = dr["CDS_HHDL_ZHGKRLXHL"];
                                    parameters[40].Value = dr["CDS_HHDL_ZHKGCO2PL"];
                                    parameters[41].Value = dr["CDS_HHDL_TJASYZDNXHL"];
                                    parameters[42].Value = dr["CDS_HHDL_TJBSYZDNXHL"];
                                    parameters[43].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[44].Value = dr["UNIQUE_CODE"];
                                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(CDD))
                            {
                                #region 纯电动
                                try
                                {
                                    #region update
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update CDD_MAIN set ");
                                    strSql.Append("QCSCQY=@QCSCQY,");
                                    strSql.Append("JYBGBH=@JYBGBH,");
                                    strSql.Append("JKQCZJXS=@JKQCZJXS,");
                                    strSql.Append("CLXH=@CLXH,");
                                    strSql.Append("HGSPBM=@HGSPBM,");
                                    strSql.Append("CLZL=@CLZL,");
                                    strSql.Append("YYC=@YYC,");
                                    strSql.Append("QDXS=@QDXS,");
                                    strSql.Append("ZWPS=@ZWPS,");
                                    strSql.Append("ZCZBZL=@ZCZBZL,");
                                    strSql.Append("ZDSJZZL=@ZDSJZZL,");
                                    strSql.Append("ZGCS=@ZGCS,");
                                    strSql.Append("EDZK=@EDZK,");
                                    strSql.Append("LTGG=@LTGG,");
                                    strSql.Append("LJ=@LJ,");
                                    strSql.Append("JYJGMC=@JYJGMC,");
                                    strSql.Append("TYMC=@TYMC,");
                                    strSql.Append("ZJ=@ZJ,");
                                    strSql.Append("RLLX=@RLLX,");
                                    strSql.Append("CDD_DDQC30FZZGCS=@CDD_DDQC30FZZGCS,");
                                    strSql.Append("CDD_DDXDCZZLYZCZBZLDBZ=@CDD_DDXDCZZLYZCZBZLDBZ,");
                                    strSql.Append("CDD_DLXDCBNL=@CDD_DLXDCBNL,");
                                    strSql.Append("CDD_DLXDCZBCDY=@CDD_DLXDCZBCDY,");
                                    strSql.Append("CDD_DLXDCZEDNL=@CDD_DLXDCZEDNL,");
                                    strSql.Append("CDD_DLXDCZZL=@CDD_DLXDCZZL,");
                                    strSql.Append("CDD_QDDJEDGL=@CDD_QDDJEDGL,");
                                    strSql.Append("CDD_QDDJFZNJ=@CDD_QDDJFZNJ,");
                                    strSql.Append("CDD_QDDJLX=@CDD_QDDJLX,");
                                    strSql.Append("CDD_ZHGKDNXHL=@CDD_ZHGKDNXHL,");
                                    strSql.Append("CDD_ZHGKXSLC=@CDD_ZHGKXSLC,");
                                    strSql.Append("UPDATETIME=@UPDATETIME");
                                    strSql.Append(" where UNIQUE_CODE=@UNIQUE_CODE ");
                                    OleDbParameter[] parameters = {
                                                      new OleDbParameter("@QCSCQY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@YYC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@QDXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                                                      new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                                                      new OleDbParameter("@LJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@TYMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLLX", OleDbType.VarChar,200),
                                                      new OleDbParameter("@CDD_DDQC30FZZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_DDXDCZZLYZCZBZLDBZ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_DLXDCBNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_DLXDCZBCDY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_DLXDCZEDNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_DLXDCZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_QDDJEDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_QDDJFZNJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_QDDJLX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_ZHGKDNXHL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CDD_ZHGKXSLC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@UPDATETIME", OleDbType.Date),
                                                      new OleDbParameter("@UNIQUE_CODE", OleDbType.VarChar,255)
                                    };
                                    parameters[0].Value = dr["QCSCQY"];
                                    parameters[1].Value = dr["JYBGBH"];
                                    parameters[2].Value = dr["JKQCZJXS"];
                                    parameters[3].Value = dr["CLXH"];
                                    parameters[4].Value = dr["HGSPBM"];
                                    parameters[5].Value = dr["CLZL"];
                                    parameters[6].Value = dr["YYC"];
                                    parameters[7].Value = dr["QDXS"];
                                    parameters[8].Value = dr["ZWPS"];
                                    parameters[9].Value = dr["ZCZBZL"];
                                    parameters[10].Value = dr["ZDSJZZL"];
                                    parameters[11].Value = dr["ZGCS"];
                                    parameters[12].Value = dr["EDZK"];
                                    parameters[13].Value = dr["LTGG"];
                                    parameters[14].Value = dr["LJ"];
                                    parameters[15].Value = dr["JYJGMC"];
                                    parameters[16].Value = dr["TYMC"];
                                    parameters[17].Value = dr["ZJ"];
                                    parameters[18].Value = dr["RLLX"];
                                    parameters[19].Value = dr["CDD_DDQC30FZZGCS"];
                                    parameters[20].Value = dr["CDD_DDXDCZZLYZCZBZLDBZ"];
                                    parameters[21].Value = dr["CDD_DLXDCBNL"];
                                    parameters[22].Value = dr["CDD_DLXDCZBCDY"];
                                    parameters[23].Value = dr["CDD_DLXDCZEDNL"];
                                    parameters[24].Value = dr["CDD_DLXDCZZL"];
                                    parameters[25].Value = dr["CDD_QDDJEDGL"];
                                    parameters[26].Value = dr["CDD_QDDJFZNJ"];
                                    parameters[27].Value = dr["CDD_QDDJLX"];
                                    parameters[28].Value = dr["CDD_ZHGKDNXHL"];
                                    parameters[29].Value = dr["CDD_ZHGKXSLC"];
                                    parameters[30].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[31].Value = dr["UNIQUE_CODE"];
                                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                            else if (rlzl.Equals(RLDC))
                            {
                                #region 燃料电池
                                try
                                {
                                    #region update
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update RLDC_MAIN set ");
                                    strSql.Append("QCSCQY=@QCSCQY,");
                                    strSql.Append("JYBGBH=@JYBGBH,");
                                    strSql.Append("JKQCZJXS=@JKQCZJXS,");
                                    strSql.Append("CLXH=@CLXH,");
                                    strSql.Append("HGSPBM=@HGSPBM,");
                                    strSql.Append("CLZL=@CLZL,");
                                    strSql.Append("YYC=@YYC,");
                                    strSql.Append("QDXS=@QDXS,");
                                    strSql.Append("ZWPS=@ZWPS,");
                                    strSql.Append("ZCZBZL=@ZCZBZL,");
                                    strSql.Append("ZDSJZZL=@ZDSJZZL,");
                                    strSql.Append("ZGCS=@ZGCS,");
                                    strSql.Append("EDZK=@EDZK,");
                                    strSql.Append("LTGG=@LTGG,");
                                    strSql.Append("LJ=@LJ,");
                                    strSql.Append("JYJGMC=@JYJGMC,");
                                    strSql.Append("TYMC=@TYMC,");
                                    strSql.Append("ZJ=@ZJ,");
                                    strSql.Append("RLLX=@RLLX,");
                                    strSql.Append("RLDC_CDDMSXZGXSCS=@RLDC_CDDMSXZGXSCS,");
                                    strSql.Append("RLDC_CQPBCGZYL=@RLDC_CQPBCGZYL,");
                                    strSql.Append("RLDC_CQPRJ=@RLDC_CQPRJ,");
                                    strSql.Append("RLDC_DDGLMD=@RLDC_DDGLMD,");
                                    strSql.Append("RLDC_CQPLX=@RLDC_CQPLX,");
                                    strSql.Append("RLDC_DDHHJSTJXXDCZBNL=@RLDC_DDHHJSTJXXDCZBNL,");
                                    strSql.Append("RLDC_DLXDCZZL=@RLDC_DLXDCZZL,");
                                    strSql.Append("RLDC_QDDJEDGL=@RLDC_QDDJEDGL,");
                                    strSql.Append("RLDC_QDDJFZNJ=@RLDC_QDDJFZNJ,");
                                    strSql.Append("RLDC_QDDJLX=@RLDC_QDDJLX,");
                                    strSql.Append("RLDC_RLLX=@RLDC_RLLX,");
                                    strSql.Append("RLDC_ZHGKHQL=@RLDC_ZHGKHQL,");
                                    strSql.Append("RLDC_ZHGKXSLC=@RLDC_ZHGKXSLC,");
                                    strSql.Append("RLDC_RLDCXTEDGL=@RLDC_RLDCXTEDGL,");
                                    strSql.Append("UPDATETIME=@UPDATETIME");
                                    strSql.Append(" where UNIQUE_CODE=@UNIQUE_CODE ");
                                    OleDbParameter[] parameters = {
                                                      new OleDbParameter("@QCSCQY", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLXH", OleDbType.VarChar,255),
                                                      new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                                      new OleDbParameter("@CLZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@YYC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@QDXS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@EDZK", OleDbType.VarChar,255),
                                                      new OleDbParameter("@LTGG", OleDbType.VarChar,200),
                                                      new OleDbParameter("@LJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@TYMC", OleDbType.VarChar,255),
                                                      new OleDbParameter("@ZJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLLX", OleDbType.VarChar,200),
                                                      new OleDbParameter("@RLDC_CDDMSXZGXSCS", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_CQPBCGZYL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_CQPRJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_DDGLMD", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_CQPLX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_DDHHJSTJXXDCZBNL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_DLXDCZZL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_QDDJEDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_QDDJFZNJ", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_QDDJLX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_RLLX", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_ZHGKHQL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@RLDC_ZHGKXSLC", OleDbType.VarChar,255),
                                        new OleDbParameter("@RLDC_RLDCXTEDGL", OleDbType.VarChar,255),
                                                      new OleDbParameter("@UPDATETIME", OleDbType.Date),
                                                      new OleDbParameter("@UNIQUE_CODE", OleDbType.VarChar,255)
                                    };
                                    parameters[0].Value = dr["QCSCQY"];
                                    parameters[1].Value = dr["JYBGBH"];
                                    parameters[2].Value = dr["JKQCZJXS"];
                                    parameters[3].Value = dr["CLXH"];
                                    parameters[4].Value = dr["HGSPBM"];
                                    parameters[5].Value = dr["CLZL"];
                                    parameters[6].Value = dr["YYC"];
                                    parameters[7].Value = dr["QDXS"];
                                    parameters[8].Value = dr["ZWPS"];
                                    parameters[9].Value = dr["ZCZBZL"];
                                    parameters[10].Value = dr["ZDSJZZL"];
                                    parameters[11].Value = dr["ZGCS"];
                                    parameters[12].Value = dr["EDZK"];
                                    parameters[13].Value = dr["LTGG"];
                                    parameters[14].Value = dr["LJ"];
                                    parameters[15].Value = dr["JYJGMC"];
                                    parameters[16].Value = dr["TYMC"];
                                    parameters[17].Value = dr["ZJ"];
                                    parameters[18].Value = dr["RLLX"];
                                    parameters[19].Value = dr["RLDC_CDDMSXZGXSCS"];
                                    parameters[20].Value = dr["RLDC_CQPBCGZYL"];
                                    parameters[21].Value = dr["RLDC_CQPRJ"];
                                    parameters[22].Value = dr["RLDC_DDGLMD"];
                                    parameters[23].Value = dr["RLDC_CQPLX"];
                                    parameters[24].Value = dr["RLDC_DDHHJSTJXXDCZBNL"];
                                    parameters[25].Value = dr["RLDC_DLXDCZZL"];
                                    parameters[26].Value = dr["RLDC_QDDJEDGL"];
                                    parameters[27].Value = dr["RLDC_QDDJFZNJ"];
                                    parameters[28].Value = dr["RLDC_QDDJLX"];
                                    parameters[29].Value = dr["RLDC_RLLX"];
                                    parameters[30].Value = dr["RLDC_ZHGKHQL"];
                                    parameters[31].Value = dr["RLDC_ZHGKXSLC"];
                                    parameters[32].Value = dr["RLDC_RLDCXTEDGL"];
                                    parameters[33].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[34].Value = dr["UNIQUE_CODE"];
                                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                    #endregion
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + "\r\n";
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + "\r\n";
            }
            return succCount;
        }

        /// <summary>
        /// 修改已经导入的主表信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string UpdateMainData2(DataSet ds, List<string> mainUpdateList)
        {
            int totalCount = 0;
            int succCount = 0;
            string msg = string.Empty;
            string clxh = string.Empty;
            //string strCon = AccessHelper.conn;
            //OleDbConnection con = new OleDbConnection(strCon);
            //con.Open();
            //OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

            try
            {
                // 传统能源
                DataTable dtCtny = D2D(dictCTNY, ds.Tables[CTNY], CTNY);
                totalCount += dtCtny.Rows.Count;
                succCount += UpdMainData(dtCtny, CTNY, ref msg);

                DataTable dtFcds = D2D(dictFCDSHHDL, ds.Tables[FCDSHHDL], FCDSHHDL);
                totalCount += dtFcds.Rows.Count;
                succCount += UpdMainData(dtFcds, FCDSHHDL, ref msg);

                DataTable dtCds = D2D(dictCDSHHDL, ds.Tables[CDSHHDL], CDSHHDL);
                totalCount += dtCds.Rows.Count;
                succCount += UpdMainData(dtCds, CDSHHDL, ref msg);

                DataTable dtCdd = D2D(dictCDD, ds.Tables[CDD], CDD);
                totalCount += dtCdd.Rows.Count;
                succCount += UpdMainData(dtCdd, CDD, ref msg);

                DataTable dtRldc = D2D(dictRLDC, ds.Tables[RLDC], RLDC);
                totalCount += dtRldc.Rows.Count;
                succCount += UpdMainData(dtRldc, RLDC, ref msg);

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
        /// 转换表头
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable FilterD2D(Dictionary<string, string> dict, DataTable dt, string tableName)
        {
            DataTable d = new DataTable();
            for (int i = 0; i < dt.Columns.Count; )
            {
                DataColumn c = dt.Columns[i];
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

        /// <summary>
        /// 英文转中文
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable E2C(Dictionary<string, string> dict, DataTable dt, string tableName)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                foreach (var kv in dict)
                {
                    if (kv.Value == dc.ColumnName)
                    {
                        dc.ColumnName = kv.Key;
                        break;
                    }
                }
            }
            return dt;
        }


        /// <summary>
        /// 转换表头
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable D2D(Dictionary<string,string>dict, DataTable dt,string tableName)
        {
            DataTable d = new DataTable();
            //d.TableName = tableName;
            for (int i = 0; i < dt.Columns.Count; )
            {
                DataColumn c = dt.Columns[i];

                //if (dt.TableName == tableName)
                //{
                    if (!dict.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dict[c.ColumnName]);
                //}
                //else
                //{
                //    if (!dictFCDSHHDL.ContainsKey(c.ColumnName))
                //    {
                //        dt.Columns.Remove(c);
                //        continue;
                //    }
                //    d.Columns.Add(dictFCDSHHDL[c.ColumnName]);
                //}
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

        private void ReadTemplate(string filePath)
        {
            DataSet ds = this.ReadTemplateExcel(filePath);
            dictCTNY = new Dictionary<string, string>();
            dictFCDSHHDL = new Dictionary<string, string>();
            dictCDSHHDL = new Dictionary<string, string>();
            dictCDD = new Dictionary<string, string>();
            dictRLDC = new Dictionary<string, string>();
            dictVin = new Dictionary<string, string>();

            foreach (DataRow r in ds.Tables[CTNY].Rows)
            {
                dictCTNY.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[FCDSHHDL].Rows)
            {
                dictFCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[CDSHHDL].Rows)
            {
                dictCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[CDD].Rows)
            {
                dictCDD.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[RLDC].Rows)
            {
                dictRLDC.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[VIN].Rows)
            {
                dictVin.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
        }

        /// <summary>
        /// 保存已经就绪的数据
        /// </summary>
        /// <param name="drVin"></param>
        /// <param name="drMain"></param>
        /// <returns></returns>
        public string SaveReadyData(DataRow drVin, DataRow drMain, DataTable dtPam)
        {
            string genMsg = string.Empty;
            string strCon = AccessHelper.conn;

            try
            {
                string strCreater = Utils.userId;
                string vin = drVin["VIN"].ToString().Trim().ToUpper();

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
                                (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME,UNIQUE_CODE
                                ) VALUES
                                (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@HGSPBM,@QTXX,@STATUS,@CREATETIME,@UPDATETIME,@UNIQUE_CODE)";

                        DateTime clzzrqDate;
                        try
                        {
                            clzzrqDate = DateTime.ParseExact(drVin["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"]);
                        }
                        
                        //DateTime clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim(), CultureInfo.InvariantCulture);
                        
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
                                     new OleDbParameter("@USER_ID",Utils.userId),
                                     new OleDbParameter("@QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                     new OleDbParameter("@JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@CLXH",drMain["CLXH"].ToString().Trim()),
                                     new OleDbParameter("@CLZL",drMain["CLZL"].ToString().Trim()),
                                     new OleDbParameter("@RLLX",drMain["RLLX"].ToString().Trim()),
                                     new OleDbParameter("@ZCZBZL",drMain["ZCZBZL"].ToString().Trim()),
                                     new OleDbParameter("@ZGCS",drMain["ZGCS"].ToString().Trim()),
                                     new OleDbParameter("@LTGG",drMain["LTGG"].ToString().Trim()),
                                     new OleDbParameter("@ZJ",drMain["ZJ"].ToString().Trim()),
                                     new OleDbParameter("@TYMC",drMain["TYMC"].ToString().Trim()),
                                     new OleDbParameter("@YYC",drMain["YYC"].ToString().Trim()),
                                     new OleDbParameter("@ZWPS",drMain["ZWPS"].ToString().Trim()),
                                     new OleDbParameter("@ZDSJZZL",drMain["ZDSJZZL"].ToString().Trim()),
                                     new OleDbParameter("@EDZK",drMain["EDZK"].ToString().Trim()),
                                     new OleDbParameter("@LJ",drMain["LJ"].ToString().Trim()),
                                     new OleDbParameter("@QDXS",drMain["QDXS"].ToString().Trim()),
                                     new OleDbParameter("@JYJGMC",drMain["JYJGMC"].ToString().Trim()),
                                     new OleDbParameter("@JYBGBH",drMain["JYBGBH"].ToString().Trim()),
                                     new OleDbParameter("@HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                     //new OleDbParameter("@QTXX",drMain["CT_QTXX"].ToString().Trim()),
                                     new OleDbParameter("@QTXX",drMain.Table.Columns.Contains("CT_QTXX") ? drMain["CT_QTXX"].ToString().Trim() : ""),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","1"),
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@UNIQUE_CODE",drVin["UNIQUE_CODE"].ToString().Trim())
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
                            string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY 
                                            (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                      VALUES
                                            (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";
                            OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",paramCode),
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@PARAM_VALUE",drMain[paramCode]),
                                     new OleDbParameter("@V_ID","")
                                   };
                            AccessHelper.ExecuteNonQuery(tra, sqlInsertParam, paramList);
                        }
                        #endregion

                        #region 保存VIN信息备用

                        string sqlDel = "DELETE FROM VIN_INFO WHERE VIN = '" + vin + "'";
                        AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                        string sqlStr = @"INSERT INTO VIN_INFO(VIN,CLXH,CLZZRQ,STATUS,CREATETIME,RLLX,UNIQUE_CODE) Values (@VIN, @CLXH,@CLZZRQ,@STATUS,@CREATETIME,@RLLX,@UNIQUE_CODE)";
                        OleDbParameter[] vinParamList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@CLXH",drVin["CLXH"].ToString().Trim()),
                                         new OleDbParameter("@CLZZRQ",clzzrqDate),
                                         new OleDbParameter("@STATUS","0"),
                                         creTime,
                                         new OleDbParameter("@RLLX",drVin["RLLX"].ToString().Trim()),
                                         new OleDbParameter("@UNIQUE_CODE",drVin["UNIQUE_CODE"].ToString().Trim())
                                      };
                        AccessHelper.ExecuteNonQuery(tra, sqlStr, vinParamList);

                        tra.Commit();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        tra.Dispose();
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
        /// 检查当前VIN数据是否已经存在于燃料数据表中
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        protected bool IsFuelDataExist(string vin)
        {
            bool isExist = false;

            string sqlQuery = @"SELECT VIN FROM FC_CLJBXX WHERE VIN='" + vin + "'";
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

        // 获取以导入但未生成油耗数据的VIN
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
        /// 获取全部参数数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetCheckData()
        {
            string sql = "select * from RLLX_PARAM";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取全部主表数据，用作合并VIN数据
        /// </summary>
        /// <returns></returns>
        public bool GetMainData()
        {
            bool flag = true;
            string sqlCtny = string.Format(@"SELECT * FROM CTNY_MAIN");
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny.ToString(), null);
            dsMainStatic.Add(CTNY,ds.Tables[0]);

            sqlCtny = string.Format(@"SELECT * FROM FCDS_MAIN");
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny.ToString(), null);
            dsMainStatic.Add(FCDSHHDL,ds.Tables[0]);

            sqlCtny = string.Format(@"SELECT * FROM CDS_MAIN");
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny.ToString(), null);
            dsMainStatic.Add(CDSHHDL,ds.Tables[0]);

            sqlCtny = string.Format(@"SELECT * FROM CDD_MAIN");
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny.ToString(), null);
            dsMainStatic.Add(CDD,ds.Tables[0]);

            sqlCtny = string.Format(@"SELECT * FROM RLDC_MAIN");
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny.ToString(), null);
            dsMainStatic.Add(RLDC,ds.Tables[0]);

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT unique_code from ctny_main union all ");
            sql.Append("SELECT unique_code from fcds_main union all ");
            sql.Append("SELECT unique_code from cds_main  union all ");
            sql.Append("SELECT unique_code from cdd_main  union all ");
            sql.Append("SELECT unique_code from rldc_main");

            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql.ToString(), null);
            dtCtnyStatic = ds.Tables[0];

            if (dtCtnyStatic.Rows.Count < 1)  //&& dtFcdsStatic.Rows.Count < 1
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 获取已经导入的参数编码（MAIN_ID）,用于导入判断
        /// </summary>
        public int GetMainId(string mainId)
        {
            int dataCount;
            string sqlCtny = string.Format(@"SELECT MAIN_ID FROM MAIN_CTNY WHERE MAIN_ID='{0}'", mainId);
            string sqlFcds = string.Format(@"SELECT MAIN_ID FROM MAIN_FCDSHHDL WHERE MAIN_ID='{0}'", mainId);
            try
            {
                DataSet dsCtnyMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                DataSet dsFcdsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);

                dataCount = dsCtnyMainId.Tables[0].Rows.Count + dsFcdsMainId.Tables[0].Rows.Count;
            }
            catch (Exception)
            {
                dataCount = 0;
            }
            return dataCount;
        }

        /// <summary>
        /// 根据VIN从vin信息表获取参数编码
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        public string GetMainIdFromVinData(string vin)
        {
            string CocId = string.Empty;
            string sqlMain = string.Format(@"SELECT MAIN_ID FROM VIN_INFO WHERE VIN='{0}'", vin);
            try
            {
                DataSet dsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlMain, null);
                if (dsMainId != null && dsMainId.Tables[0].Rows.Count > 0)
                {
                    CocId = dsMainId.Tables[0].Rows[0]["MAIN_ID"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return CocId;
        }

        public string GetUploadUser(string vin)
        {
            string userId = string.Empty;
            string sqlUser = string.Format(@"SELECT USER_ID FROM FC_CLJBXX WHERE VIN='{0}'", vin);
            try
            {
                DataSet dsUserId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlUser, null);
                if (dsUserId != null && dsUserId.Tables[0].Rows.Count > 0)
                {
                    userId = dsUserId.Tables[0].Rows[0]["USER_ID"] == null ? "" : dsUserId.Tables[0].Rows[0]["USER_ID"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return userId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<string> GetMainIdFromControl(GridView gv, DataTable dt)
        {
            List<string> mainIdList = new List<string>();

            gv.PostEditor();

            if (dt != null && dt.Rows.Count>0)
            {
                DataRow[] drVinArr = dt.Select("check=True");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((bool)dt.Rows[i]["check"])
                    {
                        mainIdList.Add(dt.Rows[i]["UNIQUE_CODE"].ToString());
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
        /// 验证单行数据
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        private string VerifyData(DataRow r, DataRow[] dr, string importType)
        {
            string message = string.Empty;

            string Jkqczjxs = Convert.ToString(r["JKQCZJXS"]);
            string Qcscqy = Convert.ToString(r["QCSCQY"]);

            // 乘用车生产企业
            if (string.IsNullOrEmpty(Qcscqy))
            {
                message += "乘用车生产企业不能为空!\r\n";
            }

            // 产品型号
            string clxh = Convert.ToString(r["CLXH"]);
            string uniqueCode = Convert.ToString(r["UNIQUE_CODE"]);
            message += this.VerifyRequired("产品型号", clxh);
            message += this.VerifyStrLen("产品型号", clxh, 100);

            // 车辆类型
            string Clzl = Convert.ToString(r["CLZL"]);
            message += this.VerifyRequired("车辆类型", Clzl);
            Clzl = Clzl.Replace("(", "（").Replace(")", "）");
            if (Clzl == "乘用车（M1类）")
            {
                Clzl = "乘用车（M1）";
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
            string Tymc = Convert.ToString(r["TYMC"]);
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
                message = uniqueCode + "【" + clxh + "】" + "：\r\n" + message;
            }

            return message;
        }

        /// <summary>
        /// 验证当前DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>返回VIN码与错误信息</returns>
        private Dictionary<string, string> VerifyData(DataTable dt, string fuelType)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            //DataRow[] dr = checkData.Select("FUEL_TYPE='" + dictionary[dt.TableName] + "' and STATUS=1");
            //foreach (DataRow r in dt.Rows)
            //{
            //    string error = VerifyData(r, dr);
            //    if (!string.IsNullOrEmpty(error))
            //        dict.Add(Convert.ToString(r["VIN"]), error);
            //}
            return dict;
        }

        /// <summary>
        /// 根据导入EXCEL 查询 本地库数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet ReadSearchExcel(string path, string sheet, string status, string Date)
        {
            DataSet ds = ReadExcel(path, sheet);
            StringBuilder strAdd = new StringBuilder();
            strAdd.Append("select * from FC_CLJBXX where VIN in(");
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
                        catch (Exception)
                        {
                            tra.Rollback();
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
                    else
                    {
                        sheet = sheet + "$";
                    }
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheet + "]", strConn);
                    oada.Fill(ds,sheet.IndexOf('$')>0?sheet.Substring(0,sheet.Length-1):sheet);
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
        /// 查询EXCEL中VIN状态
        /// </summary>
        /// <param name="vin">VIN码</param>
        /// <returns></returns>
        private int SearchStatus(string vin)
        {
            string sql = "select status from FC_CLJBXX where VIN='" + vin + "'";
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

        #region Excel导出

        private Microsoft.Office.Interop.Excel.Application excelApp = null;

        public void ExportExcel(string saveName, DataTable dt)
        {
            excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
            excelApp.Visible = false;

            try
            {
                int rowCount = dt.Rows.Count;
                int colCount = dt.Columns.Count;

                // 表头字段
                Dictionary<string, string> dictHeader = this.FillHeader(dt);

                long pageRows = 50000;//定义每页显示的行数,行数必须小于65536   
                if (rowCount > pageRows)
                {
                    int scount = (int)(rowCount / pageRows);//导出数据生成的表单数   
                    if (scount * pageRows < rowCount)//当总行数不被pageRows整除时，经过四舍五入可能页数不准   
                    {
                        scount = scount + 1;
                    }
                    for (int sc = 1; sc <= scount; sc++)
                    {
                        if (sc > 3)
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
                                datas[index, i] = dt.Rows[r][dt.Columns[i].ToString()].ToString();
                            }

                        }

                        Microsoft.Office.Interop.Excel.Range fchR = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[index + 1, colCount]];
                        fchR.NumberFormatLocal = "@";
                        fchR.Value = datas;
                      
                    }
                }
                else
                {
                    object[,] dataArray = new object[rowCount + 1, colCount];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dataArray[0, i] = dictHeader.Keys.Contains(dt.Columns[i].ColumnName) == true ? dictHeader[dt.Columns[i].ColumnName] : dt.Columns[i].ColumnName;
                    }

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            dataArray[i + 1, j] = dt.Rows[i][j].ToString();
                        }
                    }
                    Microsoft.Office.Interop.Excel.Range range = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowCount + 1, colCount]];
                    range.NumberFormatLocal = "@";
                    range.Value = dataArray;
                }

                excelBook.SaveAs(saveName, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
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
                    case "V_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "反馈码(V_ID)");
                        break;
                    case "VIN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "备案号(VIN)");
                        break;
                    case "HGSPBM":
                        dictHeader.Add(dt.Columns[i].ColumnName, "海关商品编码");
                        break;
                    case "QCSCQY":
                        dictHeader.Add(dt.Columns[i].ColumnName, "乘用车生产企业");
                        break;
                    case "JKQCZJXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "进口乘用车供应企业");
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
                    case "ZCZBZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "整备质量");
                        break;
                    case "ZGCS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "最高车速");
                        break;
                    case "LTGG":
                        dictHeader.Add(dt.Columns[i].ColumnName, "轮胎规格");
                        break;
                    case "ZJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "轴距");
                        break;
                    case "TYMC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "通用名称");
                        break;
                    case "YYC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "越野车（G类）");
                        break;
                    case "ZWPS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "座椅排数");
                        break;
                    case "ZDSJZZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "总质量");
                        break;
                    case "EDZK":
                        dictHeader.Add(dt.Columns[i].ColumnName, "额定载客");
                        break;
                    case "LJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "前轮距/后轮距");
                        break;
                    case "QDXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动型式");
                        break;
                    case "JYJGMC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "检测机构名称");
                        break;
                    case "JYBGBH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "检验报告编号");
                        break;
                    case "QTXX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "其他信息");
                        break;
                    case "STATUS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "本地状态（9：未被激活（数据通过excel导入但未被激活）；0：已上传；1：没上传；2：修改没上传；3：撤销未上传）");
                        break;
                    case "CLZZRQ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆制造日期/进口核销日期");
                        break;
                    case "UPLOADDEADLINE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "上报截止日期");
                        break;
                    case "CREATETIME":
                        dictHeader.Add(dt.Columns[i].ColumnName, "创建日期");
                        break;
                    case "USER_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "上报人");
                        break;
                    case "UPDATETIME":
                        dictHeader.Add(dt.Columns[i].ColumnName, "上报日期");
                        break;
                    case "UNIQUE_CODE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车型标示号");
                        break;
                    case "MAIN_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车型标示号");
                        break;
                    case "COCNO":
                        dictHeader.Add(dt.Columns[i].ColumnName, "COC编号");
                        break;
                    case "COCHOLDER":
                        dictHeader.Add(dt.Columns[i].ColumnName, "COC持有人");
                        break;
                    case "HGNO":
                        dictHeader.Add(dt.Columns[i].ColumnName, "海关编号");
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

        #region 参数验证

        // 验证VIN
        private string VerifyVinData(DataRow drVIN)
        {
            string message = string.Empty;
            string clzzrqDate = string.Empty;

            try
            {
                clzzrqDate = Convert.ToString(DateTime.ParseExact(drVIN["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture));
            }
            catch (Exception)
            {
                clzzrqDate = Convert.ToString(drVIN["CLZZRQ"]);
            }
            

            //message += this.VerifyRequired("产品型号", Convert.ToString(drVIN["CLXH"]));
            message += this.VerifyDateTime("进口日期", clzzrqDate);

            return message;
        }

        // 验证主表参数编码是否已经存在
        protected string VerifyMainId(string mainId, string importType)
        {
            int dataCount = this.GetMainId(mainId);

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

        // 验证燃料类型
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
                    return "燃料种类参数填写汽油、柴油、两用燃料、双燃料、气体燃料、纯电动、非插电式混合动力、插电式混合动力、燃料电池\r\n";
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

        // 验证总质量
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

        // 车辆类型
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

        // 越野车
        protected string VerifyYyc(string yyc)
        {
            if (!string.IsNullOrEmpty(yyc))
            {
                if (yyc == "是" || yyc == "否" || yyc == "1" || yyc == "0")
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
                    return "驱动型式参数应填写“前轮驱动/后轮驱动/分时全轮驱动/全时全轮驱动/智能(适时)全轮驱动”\r\n";
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
                    return "变速器型式参数应填写“MT/AT/AMT/CVT/DCT/其它”\r\n";
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
                    return "变速器档位数参数应填写“1/2/3/4/5/6/7/8/9/10/N.A”\r\n";
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
                    return "混合动力结构型式参数应填写“串联/并联/混联/其它”\r\n";
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
                    return "是否具有行驶模式手动选择功能参数应填写“是/否”\r\n";
                }
            }
            return string.Empty;
        }

        // 电动汽车储能装置种类
        protected string VerifyDlxdczzl(string dlxdczzl)
        {
            if (!string.IsNullOrEmpty(dlxdczzl))
            {
                if (dlxdczzl == "金属氢化物镍电池" || dlxdczzl == "三元锂电池" || dlxdczzl == "磷酸铁锂电池" || dlxdczzl == "锰酸锂电池" || dlxdczzl == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "电动汽车储能装置种类参数应填写“金属氢化物镍电池/三元锂电池/磷酸铁锂电池/锰酸锂电池/其它”\r\n";
                }
            }
            return string.Empty;
        }

        #endregion

        #region 燃料类型验证

        /// <summary>
        /// 验证传统能源参数
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

                    //if (PARAMFLOAT1.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    //}
                    //if (PARAMFLOAT2.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    //}

                    switch (code)
                    {
                        case "CT_FDJXH":
                            message += VerifySpace("发动机型号", Convert.ToString(r[code]));
                            break;
                        case "CT_PL":
                            message += VerifyInt("发动机排量", Convert.ToString(r[code]));
                            break;
                        case "CT_EDGL":
                            message += VerifyGLFloat("发动机功率", Convert.ToString(r[code]));
                            break;
                        case "CT_JGL":
                            if (!string.IsNullOrEmpty(Convert.ToString(r[code])))
                                message += VerifyGLFloat("发动机最大净功率", Convert.ToString(r[code]));
                            break;
                        case "CT_SJGKRLXHL":
                            message += VerifyFloat("燃料消耗量（市郊）", Convert.ToString(r[code]));
                            break;
                        case "CT_SQGKRLXHL":
                            message += VerifyFloat("燃料消耗量（市区）", Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKCO2PFL":
                            message += VerifyInt("CO2排放量（综合）", Convert.ToString(r[code]));
                            break;
                        case "CT_QGS":
                            message += VerifyInt("发动机气缸数目", Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKRLXHL":
                            message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
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

        /// <summary>
        /// 验证纯电动参数
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

                    //if (PARAMFLOAT1.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    //}
                    //if (PARAMFLOAT2.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    //}

                    switch (code)
                    {
                        case "CDD_DLXDCBNL":
                            message += VerifyInt("动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZEDNL":
                            message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                            break;
                        case "CDD_DDXDCZZLYZCZBZLDBZ":
                            message += VerifyInt("储能装置总成质量与整备质量的比值", Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZBCDY":
                            message += VerifyInt("储能装置总成标称电压", Convert.ToString(r[code]));
                            break;
                        case "CDD_DDQC30FZZGCS":
                            message += VerifyInt("30分钟最高车速", Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKXSLC":
                            message += VerifyInt("电动汽车续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJFZNJ":
                            message += VerifyQDDJInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJEDGL":
                            message += VerifyQDDJFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKDNXHL":
                            message += VerifyFloat2("工况条件下百公里耗电量", Convert.ToString(r[code]));
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

                    //if (PARAMFLOAT1.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    //}
                    //if (PARAMFLOAT2.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    //}

                    switch (code)
                    {
                        case "FCDS_HHDL_FDJXH":
                            message += VerifySpace("发动机型号", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCBNL":
                            message += VerifyInt("动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_EDGL":
                            message += VerifyGLFloat("发动机功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_JGL":
                            if (!string.IsNullOrEmpty(Convert.ToString(r[code])))
                                message += VerifyGLFloat("发动机净功率", Convert.ToString(r[code]));
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
                            message += VerifyInt("纯电驱动模式续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJFZNJ":
                            message += VerifyQDDJInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJEDGL":
                            message += VerifyQDDJFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2("混合动力汽车电功率比", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QGS":
                            message += VerifyInt("发动机气缸数目", Convert.ToString(r[code]));
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

                        case "CDS_HHDL_FDJXH":
                            message += VerifySpace("发动机型号", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCBNL":
                            message += VerifyInt("动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKDNXHL":
                            message += VerifyFloat2("工况条件下百公里耗电量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyInt("纯电驱动模式续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt("纯电动模式下1km最高车速", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJFZNJ":
                            message += VerifyQDDJInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJEDGL":
                            message += VerifyQDDJFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_EDGL":
                            message += VerifyGLFloat("发动机功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_JGL":
                            if (!string.IsNullOrEmpty(Convert.ToString(r[code])))
                                message += VerifyGLFloat("发动机净功率", Convert.ToString(r[code]));
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
                        case "CDS_HHDL_QGS":
                            message += VerifyInt("发动机气缸数目", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_TJASYZDNXHL":
                            message += VerifyFloat2("条件A试验电能消耗量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_TJBSYZDNXHL":
                            message += VerifyFloat("条件B试验燃料消耗量", Convert.ToString(r[code]));
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
                    if (code != "FCDS_HHDL_JGL" && code != "CDS_HHDL_JGL")
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

                    //if (PARAMFLOAT1.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "1");
                    //}
                    //if (PARAMFLOAT2.Contains(code))
                    //{
                    //    r[code] = this.FormatParam(Convert.ToString(r[code]), "2");
                    //}

                    switch (code)
                    {
                        case "RLDC_DDGLMD":
                            message += VerifyFloat("燃料电池堆功率密度", Convert.ToString(r[code]));
                            break;
                        case "RLDC_DDHHJSTJXXDCZBNL":
                            message += VerifyInt("电电混合技术条件下动力电池系统能量密度", Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKHQL":
                            if (!string.IsNullOrEmpty(Convert.ToString(r[code])))
                                message += VerifyFloat("燃料消耗量（综合）", Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKXSLC":
                            message += VerifyInt("电动汽车续驶里程（工况法）", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CDDMSXZGXSCS":
                            message += VerifyInt("30分钟最高车速", Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJEDGL":
                            message += VerifyQDDJFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJFZNJ":
                            message += VerifyQDDJInt("驱动电机峰值转矩", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPBCGZYL":
                            message += VerifyInt("燃料电池汽车气瓶公称工作压力", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPRJ":
                            message += VerifyInt("燃料电池汽车气瓶公称水容积", Convert.ToString(r[code]));
                            break;
                        case "RLDC_RLDCXTEDGL":
                            message += VerifyFloat("燃料电池系统额定功率", Convert.ToString(r[code]));
                            break;
                        case "RLDC_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "RLDC_ZHGKHQL")
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
        /// <summary>
        /// 参数格式验证，中间不能有空格
        /// </summary>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        private string VerifySpace(string strName, string value)
        {
            string msg = string.Empty;
            // 中间空格
            if (value.Trim().IndexOf(" ") >= 0)
            {
                msg = strName + "参数值中不能带有空格!\r\n";
            }
            return msg;
        }

        #region 输入验证
    
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

        // 验证不为空
        protected string VerifyRequired(string strName, string value)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                msg = strName + "不能为空!\r\n";
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
                    msg = strName + "长度过长，最长为" + expectedLen + "位!\r\n";
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
                msg = strName + "应为整数!\r\n";
            }
            return msg;
        }

        //验证功率
        protected static string VerifyGLFloat(string strName, string value)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (IsNumber(value))
                {
                    if (!Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
                    {
                        msg = "\n" + strName + "应保留1位小数!";
                    }
                }
                else
                {
                    msg = "\n" + strName + "应保留1位小数!";
                }
            }

            return msg;
        }
        //验证数字
        protected static bool IsNumber(string input)
        {
            string pattern = @"^-?\d+$|^(-?\d+)(\.\d+)?$";  // 数值的正则表达式
            return Regex.IsMatch(input, pattern);  // 匹配成功则说明是数值，否则不是数值
        }
        //验证驱动电机峰值转矩
        protected static string VerifyQDDJInt(string strName, string value)
        {
            string msg = string.Empty;
            if (IsNumber(value))
            {
                if (!Regex.IsMatch(value, @"^[+]?\d*$"))
                {
                    msg = strName + "应保留整数!\r\n";
                }
            }
            else if (value.Split('/').Length - 1 > 0)
            {
                string[] splitValue = value.Split('/');
                for (int i = 0; i < value.Split('/').Length; i++)
                {
                    if (!(Regex.IsMatch(splitValue[i], @"^[+]?\d*$") && IsNumber(splitValue[i])))
                    {
                        msg = strName + "应保留整数!\r\n";
                    }
                }
            }
            else
            {
                msg = strName + "应保留整数!\r\n";
            }
            return msg;
        }
        //验证驱动电机额定功率
        protected static string VerifyQDDJFloat(string strName, string value)
        {
            string msg = string.Empty;
            if (IsNumber(value))
            {
                if (!Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
                {
                    msg = strName + "应保留1位小数!\r\n";
                }
            }
            else if (value.Split('/').Length - 1 > 0)
            {
                string[] splitValue = value.Split('/');
                for (int i = 0; i < value.Split('/').Length; i++)
                {
                    if (!(Regex.IsMatch(splitValue[i], @"(\d){1,}\.\d{1}$") && IsNumber(splitValue[i])))
                    {
                        msg = strName + "应保留1位小数!\r\n";
                    }
                }
            }
            else
            {
                msg = strName + "应保留1位小数!\r\n";
            }
            return msg;
        }

        // 验证浮点型1位小数
        protected string VerifyFloat(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value))
            {
                if (IsNumber(value))
                {
                    if (!Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
                    {
                        msg = strName + "应保留1位小数!\r\n";
                    }
                }
                else
                {
                    msg = strName + "应保留1位小数!\r\n";
                }
            }
            return msg;
        }

        // 验证浮点型两位小数
        protected string VerifyFloat2(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value))
            {
                if (!Regex.IsMatch(value, @"(\d){1,}\.\d{2}$") || !(value.Split('.').Length - 1 == 1))
                {
                    msg = strName + "应保留2位小数!\r\n";
                }
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
                msg = strName + "应为时间类型!\r\n";
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
                msg = strName + "应为时间类型!\r\n";
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

        // Kill进程
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);
        public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
        {
            IntPtr t = new IntPtr(excel.Hwnd);   //得到这个句柄，具体作用是得到这块内存入口 

            int k = 0;
            GetWindowThreadProcessId(t, out k);   //得到本进程唯一标志k
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);   //得到对进程k的引用
            p.Kill();     //关闭进程k
        }

    }
}
