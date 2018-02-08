using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using FuelDataSysClient.Properties;
using System.IO;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using DevExpress.XtraGrid.Views.Grid;
using System.Collections.Specialized;
using System.Configuration;

namespace FuelDataSysClient.Tool.Tool_Nissan
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

        public class NissanUtils
        {
        private const string CTNY = "传统能源";
        private const string FCDSHHDL = "非插电式混合动力";
        private const string VIN = "VIN";
        private const string CDD = "纯电动";
        private readonly List<string> PARAMFLOAT1 = new List<string>() { "CT_EDGL", "CT_JGL", "CT_SJGKRLXHL", "CT_SQGKRLXHL", "CT_ZHGKRLXHL", "FCDS_HHDL_CDDMSXZHGKXSLC", "FCDS_HHDL_DLXDCZZNL", "FCDS_HHDL_ZHGKRLXHL", "FCDS_HHDL_EDGL", "FCDS_HHDL_JGL", "FCDS_HHDL_SJGKRLXHL", "FCDS_HHDL_SQGKRLXHL", "FCDS_HHDL_QDDJEDGL", "CDS_HHDL_DLXDCZZNL", "FCDS_HHDL_DLXDCBNL", "CDS_HHDL_ZHGKRLXHL", "CDS_HHDL_QDDJEDGL", "CDS_HHDL_EDGL", "CDS_HHDL_JGL", "CDD_DLXDCZEDNL", "CDD_QDDJEDGL", "RLDC_DDGLMD", "RLDC_ZHGKHQL", "RLDC_QDDJEDGL" };
        private readonly List<string> PARAMFLOAT2 = new List<string>() { "CDS_HHDL_HHDLZDDGLB", "FCDS_HHDL_HHDLZDDGLB" };

        private readonly string strCon = AccessHelper.conn;
        DataTable checkData = new DataTable();

        Dictionary<string, string> dictCTNY;  //存放列头转换模板(传统能源)
        Dictionary<string, string> dictFCDSHHDL;  //存放列头转换模板（非插电式混合动力）
        Dictionary<string, string> dictCDD;  //存放列头转换模板（纯电动）
        Dictionary<string, string> dictVin; //存放列头转换模板（VIN）

        DataTable dtCtnyStatic;
        DataTable dtFcdsStatic;
        DataTable dtCddStatic;

        private List<string> listHoliday; // 节假日数据

        readonly string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate_Nissan"];
        private readonly static NameValueCollection FILE_NAME = (NameValueCollection)ConfigurationManager.GetSection("fileName");

        public NissanUtils()
        {
        checkData = GetCheckData();    //获取参数数据  RLLX_PARAM  

        ReadTemplate(path);   //读取表头转置模板
        }

        // VIN excel文件名称的开头
        private readonly string vinFileName = FILE_NAME["VIN"];

        public string VinFileName
        {
        get { return vinFileName; }
        }

        // 主表Excel文件名称的开头
        private readonly string mainFileName = FILE_NAME["MAIN"];

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
        catch (Exception ex)
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
        /// 保存VIN信息
        /// </summary>
        /// <param name="ds"></param>
        public string SaveVinInfo(DataSet ds)
        {
        string msg = string.Empty;
        ProcessForm pf = new ProcessForm();
        try
        {
            DataTable dtCtnyPam = this.GetRllxData("传统能源");
            DataTable dtFcdsPam = this.GetRllxData("非插电式混合动力");
            DataTable dtCddpam = this.GetRllxData("纯电动");

            // 获取节假日数据
            listHoliday = this.GetHoliday();

            // 显示进度条
            pf.Show();
            int pageSize = 1;
            int totalVin = ds.Tables[0].Rows.Count;

            pf.TotalMax = (int)Math.Ceiling((decimal)totalVin / (decimal)pageSize);
            pf.ShowProcessBar();

            foreach (DataRow drVin in ds.Tables[0].Rows)
            {
                string vin = drVin["VIN"] == null ? "" : drVin["VIN"].ToString().Trim();
                string mainId = drVin["MAIN_ID"] == null ? "" : drVin["MAIN_ID"].ToString().Trim();
                //string agechi = drVin["AGECHI"] == null ? "" : drVin["MAIN_ID"].ToString().Trim();
                if (!string.IsNullOrEmpty(mainId))
                {
                    bool matchFlag = false;

                    // 遍历传统能源主表，查询VIN表中的参数代码（MAIN_ID）信息
                    foreach (DataRow drCtny in dtCtnyStatic.Rows)
                    {
                        if (drCtny["MAIN_ID"].ToString().Trim() == mainId)
                        {
                            matchFlag = true;
                            msg += this.SaveReadyData(drVin, drCtny, dtCtnyPam);
                            break;
                        }
                    }
                    if (!matchFlag)
                    {
                        foreach (DataRow drCdd in dtCddStatic.Rows)
                        {
                            if (drCdd["MAIN_ID"].ToString().Trim() == mainId)
                            {
                                matchFlag = true;
                                msg += this.SaveReadyData(drVin, drCdd, dtCddpam);
                                break;
                            }
                        }

                        if (!matchFlag)
                        {
                            // 遍历非插电式主表，查询VIN表中的参数代码（MAIN_ID）信息
                            foreach (DataRow drFcds in dtFcdsStatic.Rows)
                            {
                                if (drFcds["MAIN_ID"].ToString().Trim() == mainId)
                                {
                                    matchFlag = true;
                                    msg += this.SaveReadyData(drVin, drFcds, dtFcdsPam);
                                    break;
                                }
                            }

                            if (!matchFlag)
                            {
                                msg += string.Format("{0} 缺少参数代码数据{1}\r\n", vin, mainId);
                            }
                        }
                    }

                    pf.progressBarControl1.PerformStep();
                    Application.DoEvents();
                }
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
        }
        finally
        {
            if (pf != null)
            {
                pf.Close();
            }
        }
        return msg;
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
        //DataSet ds = this.ReadVinCsv(true, ',', fileName);
        DataSet ds = this.ReadVinExcel(fileName);

        if (ds != null)
        {
            rtnMsg += this.SaveVinInfo(ds);
            if (string.IsNullOrEmpty(rtnMsg))
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
                rtnMsg += this.SaveMainData(ds);
            }
            else if (importType == "UPDATE")
            {
                rtnMsg += this.UpdateMainData(ds, mainUpdateList);
            }
            if (string.IsNullOrEmpty(rtnMsg))
                rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_MAIN");

            if (!string.IsNullOrEmpty(rtnMsg))
            {
                rtnMsg = Path.GetFileName(fileName) + ":\r\n" + rtnMsg + "\r\n";
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
        string strConn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties='Excel 8.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
        DataSet ds = new DataSet();

        try
        {
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [传统能源$]", strConn);
            oada.Fill(ds, CTNY);

            oada = new OleDbDataAdapter("select * from [非插电式混合动力$]", strConn);
            oada.Fill(ds, FCDSHHDL);

            oada = new OleDbDataAdapter("select * from [纯电动$]", strConn);
            oada.Fill(ds, CDD);
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
        string strConn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties='Excel 8.0'", fileName); //; HDR=No
        DataSet ds = new DataSet();
        try
        {
            OleDbDataAdapter oada = new OleDbDataAdapter("select * from [传统能源$]", strConn);
            oada.Fill(ds, CTNY);

            oada = new OleDbDataAdapter("select * from [非插电式混合动力$]", strConn);
            oada.Fill(ds, FCDSHHDL);

            oada = new OleDbDataAdapter("select * from [纯电动$]", strConn);
            oada.Fill(ds, CDD);


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
        string msg = string.Empty;
        string strCon = AccessHelper.conn;
        OleDbConnection con = new OleDbConnection(strCon);
        con.Open();
        OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

        DataTable dtCtny = D2D(ds.Tables[CTNY]);      

        DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + CTNY + "' and STATUS=1");

        DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);

        DataRow[] drFcdsHhdl = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", FCDSHHDL));


        DataTable dtCdd = D2D(ds.Tables[CDD]);
        DataRow[] drCdd = checkData.Select("FUEL_TYPE='" + CDD + "' and STATUS=1");


        if (dtCtny != null && dtCtny.Rows.Count > 0)
        {
            string error = string.Empty;
            foreach (DataRow dr in dtCtny.Rows)
            {
                error = VerifyData(dr, drCtny, "IMPORT");      //单行验证
                if (!string.IsNullOrEmpty(error))
                {
                    msg += error;
                }
                else
                {
                    #region insert
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("INSERT INTO CTNY_MAIN(");
                    strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,CT_BSQDWS,CT_BSQXS,CT_EDGL,CT_FDJXH,CT_JGL,CT_PL,CT_QGS,CT_QTXX,CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKCO2PFL,CT_ZHGKRLXHL,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM)");
                    strSql.Append(" VALUES ('{0}',	'{1}',	'{2}',	'{3}',	'{4}',	'{5}',	'{6}',	'{7}',	'{8}',	'{9}',	'{10}',	'{11}',	'{12}',	'{13}',	'{14}',	'{15}',	'{16}',	'{17}',	'{18}',	'{19}',	'{20}',	'{21}',	'{22}',	'{23}',	'{24}',	'{25}',	'{26}',	'{27}',	'{28}',	'{29}',	'{30}',	'{31}',	'{32}',	'{33}',	'{34}',	'{35}','{36}')");
                     
                    string sqlInsertBasic = string.Format(strSql.ToString(),
                                            dr["MAIN_ID"],
                                            Utils.localUserId,
                                            dr["JKQCZJXS"],
                                            dr["QCSCQY"],
                                            dr["CLXH"],
                                            dr["CLZL"],
                                            dr["RLLX"],
                                            dr["ZCZBZL"],
                                            dr["ZGCS"],
                                            dr["LTGG"],
                                            dr["ZJ"],
                                            dr["TYMC"],
                                            dr["YYC"],
                                            dr["ZWPS"],
                                            dr["ZDSJZZL"],
                                            dr["EDZK"],
                                            dr["LJ"],
                                            dr["QDXS"],
                                            (int)Status.待上报,
                                            dr["JYJGMC"],
                                            dr["JYBGBH"],
                                            dr["CT_BSQDWS"],
                                            dr["CT_BSQXS"],
                                            dr["CT_EDGL"],
                                            dr["CT_FDJXH"],
                                            dr["CT_JGL"],
                                            dr["CT_PL"],
                                            dr["CT_QGS"],
                                            dr["CT_QTXX"],
                                            dr["CT_SJGKRLXHL"],
                                            dr["CT_SQGKRLXHL"],
                                            dr["CT_ZHGKCO2PFL"],
                                            dr["CT_ZHGKRLXHL"],
                                            DateTime.Today,
                                            Utils.localUserId,
                                            DateTime.Today,
                                            dr["HGSPBM"]
                                            );

                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlInsertBasic, null);

                    #endregion
                }
            }
        }
        if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
        {
            string error = string.Empty;
            foreach (DataRow dr in dtFcdsHhdl.Rows)
            {
                error = VerifyData(dr, drFcdsHhdl, "IMPORT");      //单行验证
                if (!string.IsNullOrEmpty(error))
                {
                    msg += error;
                }
                else
                {
                    #region insert
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("INSERT INTO CDS_MAIN(");
                    strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS,FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC,FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY,FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL,FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH,FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB,FCDS_HHDL_JGL,FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL,FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS,FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL,FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHGKRLXHL,FCDS_HHDL_ZHKGCO2PL,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM,CT_QTXX)");
                    strSql.Append(" VALUES (");
                    strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@FCDS_HHDL_BSQDWS,@FCDS_HHDL_BSQXS,@FCDS_HHDL_CDDMSXZGCS,@FCDS_HHDL_CDDMSXZHGKXSLC,@FCDS_HHDL_DLXDCBNL,@FCDS_HHDL_DLXDCZBCDY,@FCDS_HHDL_DLXDCZZL,@FCDS_HHDL_DLXDCZZNL,@FCDS_HHDL_EDGL,@FCDS_HHDL_FDJXH,@FCDS_HHDL_HHDLJGXS,@FCDS_HHDL_HHDLZDDGLB,@FCDS_HHDL_JGL,@FCDS_HHDL_PL,@FCDS_HHDL_QDDJEDGL,@FCDS_HHDL_QDDJFZNJ,@FCDS_HHDL_QDDJLX,@FCDS_HHDL_QGS,@FCDS_HHDL_SJGKRLXHL,@FCDS_HHDL_SQGKRLXHL,@FCDS_HHDL_XSMSSDXZGN,@FCDS_HHDL_ZHGKRLXHL,@FCDS_HHDL_ZHKGCO2PL,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM,@CT_QTXX)");
                    OleDbParameter[] parameters = {
		        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"]),
		        new OleDbParameter("@CREATE_BY", Utils.localUserId),
		        new OleDbParameter("@JKQCZJXS", dr["JKQCZJXS"]),
		        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
		        new OleDbParameter("@CLXH", dr["CLXH"]),
		        new OleDbParameter("@CLZL", dr["CLZL"]),
		        new OleDbParameter("@RLLX", dr["RLLX"]),
		        new OleDbParameter("@ZCZBZL", dr["ZCZBZL"]),
		        new OleDbParameter("@ZGCS", dr["ZGCS"]),
		        new OleDbParameter("@LTGG", dr["LTGG"]),
		        new OleDbParameter("@ZJ", dr["ZJ"]),
		        new OleDbParameter("@TYMC", dr["TYMC"]),
		        new OleDbParameter("@YYC", dr["YYC"]),
		        new OleDbParameter("@ZWPS", dr["ZWPS"]),
		        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
		        new OleDbParameter("@EDZK", dr["EDZK"]),
		        new OleDbParameter("@LJ", dr["LJ"]),
		        new OleDbParameter("@QDXS", dr["QDXS"]),
		        new OleDbParameter("@STATUS", (int)Status.待上报),
		        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
		        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
		        new OleDbParameter("@FCDS_HHDL_BSQDWS", dr["FCDS_HHDL_BSQDWS"]),
		        new OleDbParameter("@FCDS_HHDL_BSQXS", dr["FCDS_HHDL_BSQXS"]),
		        new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", dr["FCDS_HHDL_CDDMSXZGCS"]),
		        new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", dr["FCDS_HHDL_CDDMSXZHGKXSLC"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCBNL", dr["FCDS_HHDL_DLXDCBNL"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", dr["FCDS_HHDL_DLXDCZBCDY"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCZZL", dr["FCDS_HHDL_DLXDCZZL"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", dr["FCDS_HHDL_DLXDCZZNL"]),
		        new OleDbParameter("@FCDS_HHDL_EDGL", dr["FCDS_HHDL_EDGL"]),
		        new OleDbParameter("@FCDS_HHDL_FDJXH", dr["FCDS_HHDL_FDJXH"]),
		        new OleDbParameter("@FCDS_HHDL_HHDLJGXS", dr["FCDS_HHDL_HHDLJGXS"]),
		        new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", dr["FCDS_HHDL_HHDLZDDGLB"]),
		        new OleDbParameter("@FCDS_HHDL_JGL", dr["FCDS_HHDL_JGL"]),
		        new OleDbParameter("@FCDS_HHDL_PL", dr["FCDS_HHDL_PL"]),
		        new OleDbParameter("@FCDS_HHDL_QDDJEDGL", dr["FCDS_HHDL_QDDJEDGL"]),
		        new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", dr["FCDS_HHDL_QDDJFZNJ"]),
		        new OleDbParameter("@FCDS_HHDL_QDDJLX", dr["FCDS_HHDL_QDDJLX"]),
		        new OleDbParameter("@FCDS_HHDL_QGS", dr["FCDS_HHDL_QGS"]),
		        new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", dr["FCDS_HHDL_SJGKRLXHL"]),
		        new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", dr["FCDS_HHDL_SQGKRLXHL"]),
		        new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", dr["FCDS_HHDL_XSMSSDXZGN"]),
		        new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", dr["FCDS_HHDL_ZHGKRLXHL"]),
		        new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", dr["FCDS_HHDL_ZHKGCO2PL"]),
		        new OleDbParameter("@CREATETIME", DateTime.Today),
                new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                new OleDbParameter("@UPDATETIME", DateTime.Today),
                new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
                new OleDbParameter("@CT_QTXX", dr["CT_QTXX"])};

                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                    #endregion
                }
            }
        }

        if (dtCdd != null && dtCdd.Rows.Count > 0)
        {
            string error = string.Empty;
            foreach (DataRow dr in dtCdd.Rows)
            {
                error = VerifyData(dr, drCdd, "IMPORT");      //单行验证
                if (!string.IsNullOrEmpty(error))
                {
                    msg += error;
                }
                else
                {
                    #region insert
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("insert into CDD_MAIN(");
                    strSql.Append("MAIN_ID,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,CDD_DLXDCZZL,CDD_DLXDCBNL,CDD_DLXDCZEDNL,CDD_DDQC30FZZGCS,CDD_DDXDCZZLYZCZBZLDBZ,CDD_DLXDCZBCDY,CDD_ZHGKXSLC,CDD_QDDJLX,CDD_QDDJEDGL,CDD_QDDJFZNJ,CDD_ZHGKDNXHL,CREATE_BY,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM,CT_QTXX)");
                    strSql.Append(" values (");
                    strSql.Append("@MAIN_ID,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@CDD_DLXDCZZL,@CDD_DLXDCBNL,@CDD_DLXDCZEDNL,@CDD_DDQC30FZZGCS,@CDD_DDXDCZZLYZCZBZLDBZ,@CDD_DLXDCZBCDY,@CDD_ZHGKXSLC,@CDD_QDDJLX,@CDD_QDDJEDGL,@CDD_QDDJFZNJ,@CDD_ZHGKDNXHL,@CREATE_BY,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM,@CT_QTXX)");
                    OleDbParameter[] parameters = {
	        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"]),
	        new OleDbParameter("@JKQCZJXS", dr["JKQCZJXS"]),
	        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
	        new OleDbParameter("@CLXH", dr["CLXH"]),
	        new OleDbParameter("@CLZL", dr["CLZL"]),
	        new OleDbParameter("@RLLX", dr["RLLX"]),
	        new OleDbParameter("@ZCZBZL", dr["ZCZBZL"]),
	        new OleDbParameter("@ZGCS", dr["ZGCS"]),
	        new OleDbParameter("@LTGG", dr["LTGG"]),
	        new OleDbParameter("@ZJ", dr["ZJ"]),
	        new OleDbParameter("@TYMC", dr["TYMC"]),
	        new OleDbParameter("@YYC", dr["YYC"]),
	        new OleDbParameter("@ZWPS", dr["ZWPS"]),
	        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
	        new OleDbParameter("@EDZK",dr["EDZK"]),
	        new OleDbParameter("@LJ", dr["LJ"]),
	        new OleDbParameter("@QDXS", dr["QDXS"]),
	        new OleDbParameter("@STATUS", (int)Status.待上报),
	        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
	        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
	        new OleDbParameter("@CDD_DLXDCZZL", dr["CDD_DLXDCZZL"]),
	        new OleDbParameter("@CDD_DLXDCBNL", dr["CDD_DLXDCBNL"]),
	        new OleDbParameter("@CDD_DLXDCZEDNL", dr["CDD_DLXDCZEDNL"]),
	        new OleDbParameter("@CDD_DDQC30FZZGCS", dr["CDD_DDQC30FZZGCS"]),
	        new OleDbParameter("@CDD_DDXDCZZLYZCZBZLDBZ", dr["CDD_DDXDCZZLYZCZBZLDBZ"]),
	        new OleDbParameter("@CDD_DLXDCZBCDY", dr["CDD_DLXDCZBCDY"]),
	        new OleDbParameter("@CDD_ZHGKXSLC", dr["CDD_ZHGKXSLC"]),
	        new OleDbParameter("@CDD_QDDJLX", dr["CDD_QDDJLX"]),
	        new OleDbParameter("@CDD_QDDJEDGL", dr["CDD_QDDJEDGL"]),
	        new OleDbParameter("@CDD_QDDJFZNJ", dr["CDD_QDDJFZNJ"]),
	        new OleDbParameter("@CDD_ZHGKDNXHL", dr["CDD_ZHGKDNXHL"]),
	        new OleDbParameter("@CREATE_BY", Utils.localUserId),
	        new OleDbParameter("@CREATETIME", DateTime.Today),
	        new OleDbParameter("@UPDATE_BY", Utils.localUserId),
	        new OleDbParameter("@UPDATETIME", DateTime.Today),
            new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
            new OleDbParameter("@CT_QTXX", dr["CT_QTXX"])};
                  
                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                    #endregion
                }
            }
        }

        return msg;
        }

        /// <summary>
        /// 修改已经导入的主表信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string UpdateMainData(DataSet ds, List<string> mainUpdateList)
        {
        string msg = string.Empty;
        string strCon = AccessHelper.conn;
        string mainId = string.Empty;
        OleDbConnection con = new OleDbConnection(strCon);
        con.Open();
        OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务


        DataTable dtCtny = D2D(ds.Tables[CTNY]);
        //DataTable dtCtny = ds.Tables[CTNY];

        DataRow[] drCtny = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", CTNY));

        DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);
        //DataTable dtFcdsHhdl = ds.Tables[FCDSHHDL];

        DataRow[] drFcdsHhdl = checkData.Select("FUEL_TYPE='" + FCDSHHDL + "' and STATUS=1");

        DataTable dtCdd = D2D(ds.Tables[CDD]);
        DataRow[] drCdd = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", CDD));

        if (dtCtny != null && dtCtny.Rows.Count > 0)
        {
            string error = string.Empty;
            foreach (DataRow dr in dtCtny.Rows)
            {
                error = VerifyData(dr, drCtny, "UPDATE");      //单行验证
                if (!string.IsNullOrEmpty(error))
                {
                    msg += error;
                }
                else
                {
                    #region UPDATE
                    mainId = dr["MAIN_ID"].ToString();
                    string sqlCtny = @"UPDATE CTNY_MAIN 
                                    SET  JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                                    ZCZBZL=@ZCZBZL,ZGCS=@ZGCS,LTGG=@LTGG,ZJ=@ZJ,
                                    TYMC=@TYMC,YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,
                                    EDZK=@EDZK,LJ=@LJ,QDXS=@QDXS,STATUS=@STATUS,
                                    JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,CT_BSQDWS=@CT_BSQDWS,CT_BSQXS=@CT_BSQXS,
                                    CT_EDGL=@CT_EDGL,CT_FDJXH=@CT_FDJXH,CT_JGL=@CT_JGL,CT_PL=@CT_PL,
                                    CT_QGS=@CT_QGS,CT_QTXX=@CT_QTXX,CT_SJGKRLXHL=@CT_SJGKRLXHL,CT_SQGKRLXHL=@CT_SQGKRLXHL,
                                    CT_ZHGKCO2PFL=@CT_ZHGKCO2PFL,CT_ZHGKRLXHL=@CT_ZHGKRLXHL,UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                                    HGSPBM=@HGSPBM
                                    WHERE MAIN_ID=@MAIN_ID";


                    OleDbParameter[] parameters = {
			        new OleDbParameter("@JKQCZJXS", dr["JKQCZJXS"]),
			        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
			        new OleDbParameter("@CLXH", dr["CLXH"]),
			        new OleDbParameter("@CLZL", dr["CLZL"]),
			        new OleDbParameter("@RLLX", dr["RLLX"]),

			        new OleDbParameter("@ZCZBZL", dr["ZCZBZL"]),
			        new OleDbParameter("@ZGCS", dr["ZGCS"]),
			        new OleDbParameter("@LTGG", dr["LTGG"]),
			        new OleDbParameter("@ZJ", dr["ZJ"]),
			        new OleDbParameter("@TYMC", dr["TYMC"]),

			        new OleDbParameter("@YYC", dr["YYC"]),
			        new OleDbParameter("@ZWPS", dr["ZWPS"]),
			        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
			        new OleDbParameter("@EDZK", dr["EDZK"]),
			        new OleDbParameter("@LJ", dr["LJ"]),

			        new OleDbParameter("@QDXS", dr["QDXS"]),
			        new OleDbParameter("@STATUS", (int)Status.待上报),
			        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
			        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),

			        new OleDbParameter("@CT_BSQDWS", dr["CT_BSQDWS"]),
			        new OleDbParameter("@CT_BSQXS", dr["CT_BSQXS"]),
			        new OleDbParameter("@CT_EDGL", dr["CT_EDGL"]),
			        new OleDbParameter("@CT_FDJXH",dr["CT_FDJXH"]),
			        new OleDbParameter("@CT_JGL", dr["CT_JGL"]),

			        new OleDbParameter("@CT_PL", dr["CT_PL"]),
			        new OleDbParameter("@CT_QGS", dr["CT_QGS"]),
			        new OleDbParameter("@CT_QTXX", dr["CT_QTXX"]),
			        new OleDbParameter("@CT_SJGKRLXHL", dr["CT_SJGKRLXHL"]),
			        new OleDbParameter("@CT_SQGKRLXHL", dr["CT_SQGKRLXHL"]),

			        new OleDbParameter("@CT_ZHGKCO2PFL", dr["CT_ZHGKCO2PFL"]),
			        new OleDbParameter("@CT_ZHGKRLXHL", dr["CT_ZHGKRLXHL"]),
                    new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                    new OleDbParameter("@UPDATETIME", DateTime.Today),
                    new OleDbParameter("@HGSPBM", dr["JKQCZJXS"]),
                            
			        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"])
                };

                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlCtny, parameters);
                    mainUpdateList.Add(mainId);
                    #endregion
                }
            }
        }
        if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
        {
            string error = string.Empty;
            foreach (DataRow dr in dtFcdsHhdl.Rows)
            {
                error = VerifyData(dr, drFcdsHhdl, "UPDATE");      //单行验证
                if (!string.IsNullOrEmpty(error))
                {
                    msg += error;
                }
                else
                {
                    #region UPDATE
                    mainId = dr["MAIN_ID"].ToString();
                    string sqlFcds = @"UPDATE CDS_MAIN
                                SET JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                                    ZCZBZL=@ZCZBZL,ZGCS=@ZGCS,LTGG=@LTGG,ZJ=@ZJ,TYMC=@TYMC,
                                    YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,EDZK=@EDZK,LJ=@LJ,
                                    QDXS=@QDXS,STATUS=@STATUS,JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,
                                    FCDS_HHDL_BSQDWS=@FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS=@FCDS_HHDL_BSQXS,
                                    FCDS_HHDL_CDDMSXZGCS=@FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC=@FCDS_HHDL_CDDMSXZHGKXSLC,
                                    FCDS_HHDL_DLXDCBNL=@FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY=@FCDS_HHDL_DLXDCZBCDY,
                                    FCDS_HHDL_DLXDCZZL=@FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL=@FCDS_HHDL_DLXDCZZNL,
                                    FCDS_HHDL_EDGL=@FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH=@FCDS_HHDL_FDJXH,
                                    FCDS_HHDL_HHDLJGXS=@FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB=@FCDS_HHDL_HHDLZDDGLB,
                                    FCDS_HHDL_JGL=@FCDS_HHDL_JGL,FCDS_HHDL_PL=@FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL=@FCDS_HHDL_QDDJEDGL,
                                    FCDS_HHDL_QDDJFZNJ=@FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX=@FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS=@FCDS_HHDL_QGS,
                                    FCDS_HHDL_SJGKRLXHL=@FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL=@FCDS_HHDL_SQGKRLXHL,
                                    FCDS_HHDL_XSMSSDXZGN=@FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHGKRLXHL=@FCDS_HHDL_ZHGKRLXHL,
                                    FCDS_HHDL_ZHKGCO2PL=@FCDS_HHDL_ZHKGCO2PL,UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,HGSPBM=@HGSPBM,CT_QTXX=@CT_QTXX
                                    WHERE MAIN_ID=@MAIN_ID";



                    OleDbParameter[] parameters = {
		        new OleDbParameter("@JKQCZJXS", dr["JKQCZJXS"]),
		        new OleDbParameter("@QCSCQY",dr["QCSCQY"]),
		        new OleDbParameter("@CLXH", dr["CLXH"]),
		        new OleDbParameter("@CLZL", dr["CLZL"]),
		        new OleDbParameter("@RLLX", dr["RLLX"]),

		        new OleDbParameter("@ZCZBZL", dr["ZCZBZL"]),
		        new OleDbParameter("@ZGCS", dr["ZGCS"]),
		        new OleDbParameter("@LTGG", dr["LTGG"]),
		        new OleDbParameter("@ZJ", dr["ZJ"]),
		        new OleDbParameter("@TYMC", dr["TYMC"]),

		        new OleDbParameter("@YYC", dr["YYC"]),
		        new OleDbParameter("@ZWPS", dr["ZWPS"]),
		        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
		        new OleDbParameter("@EDZK", dr["EDZK"]),
		        new OleDbParameter("@LJ", dr["LJ"]),

		        new OleDbParameter("@QDXS", dr["QDXS"]),
		        new OleDbParameter("@STATUS", (int)Status.待上报),
		        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
		        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
		        new OleDbParameter("@FCDS_HHDL_BSQDWS", dr["FCDS_HHDL_BSQDWS"]),

		        new OleDbParameter("@FCDS_HHDL_BSQXS", dr["FCDS_HHDL_BSQXS"]),
		        new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", dr["FCDS_HHDL_CDDMSXZGCS"]),
		        new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", dr["FCDS_HHDL_CDDMSXZHGKXSLC"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCBNL", dr["FCDS_HHDL_DLXDCBNL"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", dr["FCDS_HHDL_DLXDCZBCDY"]),

		        new OleDbParameter("@FCDS_HHDL_DLXDCZZL", dr["FCDS_HHDL_DLXDCZZL"]),
		        new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", dr["FCDS_HHDL_DLXDCZZNL"]),
		        new OleDbParameter("@FCDS_HHDL_EDGL", dr["FCDS_HHDL_EDGL"]),
		        new OleDbParameter("@FCDS_HHDL_FDJXH", dr["FCDS_HHDL_FDJXH"]),
		        new OleDbParameter("@FCDS_HHDL_HHDLJGXS",dr["FCDS_HHDL_HHDLJGXS"]),

		        new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", dr["FCDS_HHDL_HHDLZDDGLB"]),
		        new OleDbParameter("@FCDS_HHDL_JGL", dr["FCDS_HHDL_JGL"]),
		        new OleDbParameter("@FCDS_HHDL_PL", dr["FCDS_HHDL_PL"]),
		        new OleDbParameter("@FCDS_HHDL_QDDJEDGL", dr["FCDS_HHDL_QDDJEDGL"]),
		        new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", dr["FCDS_HHDL_QDDJFZNJ"]),

		        new OleDbParameter("@FCDS_HHDL_QDDJLX", dr["FCDS_HHDL_QDDJLX"]),
		        new OleDbParameter("@FCDS_HHDL_QGS", dr["FCDS_HHDL_QGS"]),
		        new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", dr["FCDS_HHDL_SJGKRLXHL"]),
		        new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", dr["FCDS_HHDL_SQGKRLXHL"]),
		        new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", dr["FCDS_HHDL_XSMSSDXZGN"]),

		        new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", dr["FCDS_HHDL_ZHGKRLXHL"]),
		        new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", dr["FCDS_HHDL_ZHKGCO2PL"]),
                new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                new OleDbParameter("@UPDATETIME", DateTime.Today),
                new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
		        new OleDbParameter("@CT_QTXX", dr["CT_QTXX"]),

                        
		        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"])
                                                };

                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlFcds.ToString(), parameters);
                    mainUpdateList.Add(mainId);
                    #endregion
                }
            }
        }

        if (dtCdd != null && dtCdd.Rows.Count > 0)
        {
            string error = string.Empty;
            foreach (DataRow dr in dtCdd.Rows)
            {
                error = VerifyData(dr, drCdd, "UPDATE");      //单行验证
                if (!string.IsNullOrEmpty(error))
                {
                    msg += error;
                }
                else
                {
                    #region UPDATE
                    mainId = dr["MAIN_ID"].ToString();
                    StringBuilder strSql = new StringBuilder();
                    strSql.Append("update CDD_MAIN set ");
                    strSql.Append("JKQCZJXS=@JKQCZJXS,");
                    strSql.Append("QCSCQY=@QCSCQY,");
                    strSql.Append("CLXH=@CLXH,");
                    strSql.Append("CLZL=@CLZL,");
                    strSql.Append("RLLX=@RLLX,");
                    strSql.Append("ZCZBZL=@ZCZBZL,");
                    strSql.Append("ZGCS=@ZGCS,");
                    strSql.Append("LTGG=@LTGG,");
                    strSql.Append("ZJ=@ZJ,");
                    strSql.Append("TYMC=@TYMC,");
                    strSql.Append("YYC=@YYC,");
                    strSql.Append("ZWPS=@ZWPS,");
                    strSql.Append("ZDSJZZL=@ZDSJZZL,");
                    strSql.Append("EDZK=@EDZK,");
                    strSql.Append("LJ=@LJ,");
                    strSql.Append("QDXS=@QDXS,");
                    strSql.Append("STATUS=@STATUS,");
                    strSql.Append("JYJGMC=@JYJGMC,");
                    strSql.Append("JYBGBH=@JYBGBH,");
                    strSql.Append("CDD_DLXDCZZL=@CDD_DLXDCZZL,");
                    strSql.Append("CDD_DLXDCBNL=@CDD_DLXDCBNL,");
                    strSql.Append("CDD_DLXDCZEDNL=@CDD_DLXDCZEDNL,");
                    strSql.Append("CDD_DDQC30FZZGCS=@CDD_DDQC30FZZGCS,");
                    strSql.Append("CDD_DDXDCZZLYZCZBZLDBZ=@CDD_DDXDCZZLYZCZBZLDBZ,");
                    strSql.Append("CDD_DLXDCZBCDY=@CDD_DLXDCZBCDY,");
                    strSql.Append("CDD_ZHGKXSLC=@CDD_ZHGKXSLC,");
                    strSql.Append("CDD_QDDJLX=@CDD_QDDJLX,");
                    strSql.Append("CDD_QDDJEDGL=@CDD_QDDJEDGL,");
                    strSql.Append("CDD_QDDJFZNJ=@CDD_QDDJFZNJ,");
                    strSql.Append("CDD_ZHGKDNXHL=@CDD_ZHGKDNXHL,");
                    strSql.Append("UPDATE_BY=@UPDATE_BY,");
                    strSql.Append("UPDATETIME=@UPDATETIME,");
                    strSql.Append("HGSPBM=@HGSPBM,");
                    strSql.Append("CT_QTXX=@CT_QTXX");
                    strSql.Append(" where MAIN_ID=@MAIN_ID ");
                    OleDbParameter[] parameters = {
	        new OleDbParameter("@JKQCZJXS", dr["JKQCZJXS"]),
	        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
	        new OleDbParameter("@CLXH", dr["CLXH"]),
	        new OleDbParameter("@CLZL", dr["CLZL"]),
	        new OleDbParameter("@RLLX", dr["RLLX"]),
	        new OleDbParameter("@ZCZBZL", dr["ZCZBZL"]),
	        new OleDbParameter("@ZGCS", dr["ZGCS"]),
	        new OleDbParameter("@LTGG", dr["LTGG"]),
	        new OleDbParameter("@ZJ", dr["ZJ"]),
	        new OleDbParameter("@TYMC", dr["TYMC"]),
	        new OleDbParameter("@YYC", dr["YYC"]),
	        new OleDbParameter("@ZWPS", dr["ZWPS"]),
	        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
	        new OleDbParameter("@EDZK", dr["EDZK"]),
	        new OleDbParameter("@LJ", dr["LJ"]),
	        new OleDbParameter("@QDXS", dr["QDXS"]),
	        new OleDbParameter("@STATUS", (int)Status.待上报),
	        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
	        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
	        new OleDbParameter("@CDD_DLXDCZZL", dr["CDD_DLXDCZZL"]),
	        new OleDbParameter("@CDD_DLXDCBNL", dr["CDD_DLXDCBNL"]),
	        new OleDbParameter("@CDD_DLXDCZEDNL", dr["CDD_DLXDCZEDNL"]),
	        new OleDbParameter("@CDD_DDQC30FZZGCS", dr["CDD_DDQC30FZZGCS"]),
	        new OleDbParameter("@CDD_DDXDCZZLYZCZBZLDBZ", dr["CDD_DDXDCZZLYZCZBZLDBZ"]),
	        new OleDbParameter("@CDD_DLXDCZBCDY", dr["CDD_DLXDCZBCDY"]),
	        new OleDbParameter("@CDD_ZHGKXSLC", dr["CDD_ZHGKXSLC"]),
	        new OleDbParameter("@CDD_QDDJLX", dr["CDD_QDDJLX"]),
	        new OleDbParameter("@CDD_QDDJEDGL", dr["CDD_QDDJEDGL"]),
	        new OleDbParameter("@CDD_QDDJFZNJ", dr["CDD_QDDJFZNJ"]),
	        new OleDbParameter("@CDD_ZHGKDNXHL", dr["CDD_ZHGKDNXHL"]),
	        new OleDbParameter("@UPDATE_BY", Utils.localUserId),
	        new OleDbParameter("@UPDATETIME", DateTime.Today),
            new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
	        new OleDbParameter("@CT_QTXX", dr["CT_QTXX"]),

	        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"])};
                  
                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                    mainUpdateList.Add(mainId);
                    #endregion
                }
            }
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
            else if (dt.TableName == CDD)
            {
                if (!dictCDD.ContainsKey(c.ColumnName))
                {
                    dt.Columns.Remove(c);
                    continue;
                }
                d.Columns.Add(dictCDD[c.ColumnName]);
            }

            i++;
        }

        foreach (DataRow r in dt.Rows)
        {
            if (r["COCNO"] != null && !string.IsNullOrEmpty(r["COCNO"].ToString()))
            {
                DataRow ddr = d.NewRow();
                ddr = r;
                d.Rows.Add(ddr.ItemArray);
            }
        }

        return d;
        }

        private DataSet D2DVin(DataSet dsVin)
        {
        DataSet d = new DataSet();
        DataTable dtNew = new DataTable();
        DataTable dt = dsVin.Tables[0];
        for (int i = 0; i < dt.Columns.Count; )
        {
            DataColumn c = dt.Columns[i];

            if (!dictVin.ContainsKey(c.ColumnName))
            {
                dt.Columns.Remove(c);
                continue;
            }
            dtNew.Columns.Add(dictVin[c.ColumnName]);
            i++;
        }

        foreach (DataRow r in dt.Rows)
        {
            if (r["车辆识别码(VIN)(17位)"] != null && !string.IsNullOrEmpty(r["车辆识别码(VIN)(17位)"].ToString()))
            {
                DataRow ddr = dtNew.NewRow();
                ddr = r;
                dtNew.Rows.Add(ddr.ItemArray);
            }
        }
        d.Tables.Add(dtNew);
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
        dictCDD = new Dictionary<string, string>();
        dictVin = new Dictionary<string, string>();

        foreach (DataRow r in ds.Tables[CTNY].Rows)
        {
            dictCTNY.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
        }

        foreach (DataRow r in ds.Tables[FCDSHHDL].Rows)
        {
            dictFCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
        }

        foreach (DataRow r in ds.Tables[CDD].Rows)
        {
            dictCDD.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
        }

        foreach (DataRow r in ds.Tables[VIN].Rows)
        {
            dictVin.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
        }
        }

        /// <summary>
        /// 提取燃料消耗量数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveParam(DataSet ds)
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
                            //SqlHelper.ExecuteNonQuery(tra, sqlDelBasic, null);

                            // 如果当前vin数据已经存在，则跳过
                            if (this.IsFuelDataExist(vin))
                            {
                                genMsg += vin + "已经存在。\r\n";
                                continue;
                            }

                            #region 待生成的燃料基本信息数据存入燃料基本信息表

                            tra = con.BeginTransaction();
                            DateTime clzzrqDate = Convert.ToDateTime(dr["CLZZRQ"].ToString().Trim());
                            OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                            clzzrq.OleDbType = OleDbType.DBTime;

                            DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                            OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                            uploadDeadline.OleDbType = OleDbType.DBTime;

                            OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                            creTime.OleDbType = OleDbType.DBTime;

                            OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                            upTime.OleDbType = OleDbType.DBTime;

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
                            AccessHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX
                        (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                            RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                            TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                            QDXS,JYJGMC,JYBGBH,STATUS,CREATETIME,UPDATETIME
                        ) VALUES
                        (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                            @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                            @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                            @QDXS,@JYJGMC,@JYBGBH,@STATUS,@CREATETIME,@UPDATETIME)", param);

                            #endregion

                            #region 删除参数表中vin已存在的信息

                            string sqlDelParam = String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", vin);
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
                            string sqlInsertHis = @"INSERT INTO VIN_INFO_HIS(VIN,MAIN_ID,AGECHI) Values (@VIN, @MAIN_ID,@AGECHI)";
                            OleDbParameter[] paramHis = { 
                                                new OleDbParameter("@VIN", vin),
                                                new OleDbParameter("@MAIN_ID", dr["MAIN_ID"].ToString().Trim()),
                                                new OleDbParameter("@AGECHI", dr["AGECHI"].ToString().Trim())
                                            };
                            AccessHelper.ExecuteNonQuery(tra, sqlInsertHis, paramHis);

                            // 删除已经用完的待生成VIN数据
                            string sqlDelVinInfo = String.Format(@"DELETE FROM VIN_INFO WHERE VIN='{0}''", vin);
                            AccessHelper.ExecuteNonQuery(tra, sqlDelVinInfo, null);

                            tra.Commit();
                            pf.progressBarControl1.PerformStep();
                            System.Windows.Forms.Application.DoEvents();

                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                tra.Rollback();
                            }
                            catch (Exception)
                            {
                            }
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

            using (OleDbConnection con = new OleDbConnection(strCon))
            {
                con.Open();
                OleDbTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    string vin = drVin["VIN"].ToString().Trim().ToUpper();

                    // 如果当前vin数据已经存在，则跳过
                    if (this.IsFuelDataExist(vin))
                    {
                        genMsg += vin + "已经存在。\r\n";
                    }

                    #region 待生成的燃料基本信息数据存入燃料基本信息表

                    tra = con.BeginTransaction();

                    string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,MAIN_ID,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,STATUS,CREATETIME,UPDATETIME,AGECHI,HGSPBM,QTXX
                                ) VALUES
                                (   @VIN,@MAIN_ID,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@STATUS,@CREATETIME,@UPDATETIME,@AGECHI,@HGSPBM,@QTXX)";

                    DateTime clzzrqDate;
                    try
                    {
                        clzzrqDate = DateTime.ParseExact(drVin["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception)
                    {
                        clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"]);
                    }
                    DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);

                    OleDbParameter[] param = { 
                                new OleDbParameter("@VIN",vin),
                                new OleDbParameter("@MAIN_ID",drMain["MAIN_ID"].ToString().Trim()),
                                new OleDbParameter("@USER_ID",drMain["CREATE_BY"].ToString().Trim()),
                                new OleDbParameter("@QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                new OleDbParameter("@JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                new OleDbParameter("@CLZZRQ", clzzrqDate),
                                new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate),
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
                                new OleDbParameter("@STATUS","1"),
                                new OleDbParameter("@CREATETIME", DateTime.Today),
                                new OleDbParameter("@UPDATETIME", DateTime.Today),
                                new OleDbParameter("@AGECHI",drVin["AGECHI"].ToString().Trim()),
                                new OleDbParameter("@HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                    new OleDbParameter("@QTXX",drMain["CT_QTXX"].ToString().Trim())
                                };
                    AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                    #endregion

                    #region 插入参数信息

                    string sqlDelParam = String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", vin);
                    AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                    // 待生成的燃料参数信息存入燃料参数表
                    foreach (DataRow drParam in dtPam.Rows)
                    {
                        string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                        OleDbParameter[] paramList = { 
                                new OleDbParameter("@PARAM_CODE",paramCode),
                                new OleDbParameter("@VIN",vin),
                                new OleDbParameter("@PARAM_VALUE",drMain[paramCode]),
                                new OleDbParameter("@V_ID","")
                            };
                        AccessHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO RLLX_PARAM_ENTITY 
                                    (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                VALUES
                                    (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)", paramList);
                    }
                    #endregion

                    #region 保存VIN信息备用

                    string sqlDel = String.Format("DELETE FROM VIN_INFO WHERE VIN = '{0}'", vin);
                    AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                    string sqlStr = @"INSERT INTO VIN_INFO(VIN,MAIN_ID,CLZZRQ,AGECHI) Values (@VIN, @MAIN_ID,@CLZZRQ,@AGECHI)";
                    OleDbParameter[] vinParamList = { 
                                    new OleDbParameter("@VIN",vin),
                                    new OleDbParameter("@MAIN_ID",drVin["MAIN_ID"].ToString().Trim()),
                                    new OleDbParameter("@CLZZRQ",Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim())),
                                    new OleDbParameter("@AGECHI",drVin["AGECHI"].ToString().Trim())
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

        string sqlQuery = String.Format(@"SELECT * FROM FC_CLJBXX WHERE VIN='{0}'", vin);
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

        /// <summary>
        /// 获取全部主表数据，用作合并VIN数据
        /// </summary>
        /// <returns></returns>
        public bool GetMainData()
        {
        bool flag = true;
        const string sqlCtny = @"SELECT * FROM CTNY_MAIN";
        const string sqlFcds = @"SELECT * FROM CDS_MAIN";
        const string sqlCdd = @"SELECT * FROM CDD_MAIN";

        DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
        dtCtnyStatic = ds.Tables[0];

        ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
        dtFcdsStatic = ds.Tables[0];

        ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCdd, null);
        dtCddStatic = ds.Tables[0];

        if (dtCtnyStatic.Rows.Count < 1 && dtFcdsStatic.Rows.Count < 1 && dtCddStatic.Rows.Count < 1)
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
        string sqlCtny = string.Format(@"SELECT MAIN_ID FROM CTNY_MAIN WHERE MAIN_ID='{0}'", mainId);
        string sqlFcds = string.Format(@"SELECT MAIN_ID FROM CDS_MAIN WHERE MAIN_ID='{0}'", mainId);
        string sqlCdd = string.Format(@"SELECT MAIN_ID FROM CDD_MAIN WHERE MAIN_ID='{0}'", mainId);
        try
        {
            DataSet dsCtnyMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
            DataSet dsFcdsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
            DataSet dsCddMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCdd, null);

            dataCount = dsCtnyMainId.Tables[0].Rows.Count + dsFcdsMainId.Tables[0].Rows.Count + dsCddMainId.Tables[0].Rows.Count;
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


        /// <summary>
        /// 根据VIN从vin信息表获取参数编码
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        public string GetAgechiFromVinData(string vin)
        {
        string CocId = string.Empty;
        string sqlMain = string.Format(@"SELECT AGECHI FROM VIN_INFO WHERE VIN='{0}'", vin);
        try
        {
            DataSet dsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlMain, null);
            if (dsMainId != null && dsMainId.Tables[0].Rows.Count > 0)
            {
                CocId = dsMainId.Tables[0].Rows[0]["AGECHI"].ToString();
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

        if (dt != null)
        {
            DataRow[] drVinArr = dt.Select("check=True");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if ((bool)dt.Rows[i]["check"])
                {
                    mainIdList.Add(dt.Rows[i]["MAIN_ID"].ToString());
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
        string mainId = Convert.ToString(r["MAIN_ID"]);
        message += this.VerifyMainId(mainId, importType);

        string Jkqczjxs = Convert.ToString(r["JKQCZJXS"]);
        string Qcscqy = Convert.ToString(r["QCSCQY"]);

        // 乘用车生产企业
        if (string.IsNullOrEmpty(Qcscqy))
        {
            message += "乘用车生产企业不能为空!\r\n";
        }

        // 产品型号
        string clxh = Convert.ToString(r["CLXH"]);
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
            message = r["MAIN_ID"].ToString() + "：\r\n" + message;
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
                                string sql = String.Format("UPDATE FC_CLJBXX SET CLZZRQ='{0}', UPLOADDEADLINE='{1}'{2}  where VIN='{3}'", clzzrqDate, uploadDeadlineDate, statuswhere, r["VIN"]);
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
        string strConn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}; Extended Properties='Excel 8.0'", fileName); //; HDR=No
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

        public DataSet ReadVinExcel(string fileName)
        {
        DataSet dsExcel = this.ReadExcel(fileName, "");

        DataSet dsVin = this.D2DVin(dsExcel);

        return dsVin;
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

        #region 参数验证

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
                        message += VerifyInt("发动机排量", Convert.ToString(r[code]));
                        break;
                    case "CT_EDGL":
                        message += VerifyFloat("发动机功率", Convert.ToString(r[code]));
                        break;
                    case "CT_JGL":
                        message += VerifyFloat("发动机最大净功率", Convert.ToString(r[code]));
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
                        message += VerifyDLXDCBNL("动力电池系统能量密度", Convert.ToString(r[code]));
                        break;
                    case "CDD_DLXDCZEDNL":
                        message += VerifyFloat("储能装置总储电量", Convert.ToString(r[code]));
                        break;
                    case "CDD_DDXDCZZLYZCZBZLDBZ":
                        message += VerifyDLXDCBNL("储能装置总成质量与整备质量的比值", Convert.ToString(r[code]));
                        break;
                    case "CDD_DLXDCZBCDY":
                        message += VerifyInt("储能装置总成标称电压", Convert.ToString(r[code]));
                        break;
                    case "CDD_DDQC30FZZGCS":
                        message += VerifyInt("30分钟最高车速", Convert.ToString(r[code]));
                        break;
                    case "CDD_ZHGKXSLC":
                        message += VerifyDLXDCBNL("电动汽车续驶里程（工况法）", Convert.ToString(r[code]));
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
                        message += VerifyDLXDCBNL("动力电池系统能量密度", Convert.ToString(r[code]));
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
                        message += VerifyDLXDCBNL("纯电驱动模式续驶里程（工况法）", Convert.ToString(r[code]));
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
                        message += VerifyDLXDCBNL("动力电池系统能量密度", Convert.ToString(r[code]));
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
                        message += VerifyDLXDCBNL("纯电驱动模式续驶里程（工况法）", Convert.ToString(r[code]));
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
                        message += VerifyDLXDCBNL("电电混合技术条件下动力电池系统能量密度", Convert.ToString(r[code]));
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

        // 验证动力电池系统能量密度和纯电驱动模式续驶里程（工况法）,电动汽车续驶里程（工况法）
        protected string VerifyDLXDCBNL(string strName, string value)
        {
        string msg = string.Empty;
        if (!string.IsNullOrEmpty(value))
        {
            if (!Regex.IsMatch(value, @"(\d){1,}\.\d{1}$") && !Regex.IsMatch(value.ToString(), "^[0-9]*$"))
            {
                msg = "\n" + strName + "应为整数!";
            }
        }
        return msg;
        }

        protected string FormatParam(string obj, string strFormat)
        {
        string msg = string.Empty;
        try
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                if (Regex.IsMatch(obj.ToString(), "\\d+(.\\d+)?$") && strFormat == "1")
                {
                    obj = (float.Parse(obj)).ToString("0.0");
                }
                if (Regex.IsMatch(obj.ToString(), "\\d+(.\\d+)?$") && strFormat == "2")
                {
                    obj = (float.Parse(obj)).ToString("0.00");
                }
            }
        }
        catch (Exception ex)
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

        // 验证浮点型1位小数
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

        // 验证浮点型两位小数
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
