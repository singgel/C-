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
using Oracle.ManagedDataAccess.Client;
using FuelDataSysClient.SubForm;

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
        public static Dictionary<string, string> dictRllx = new Dictionary<string, string>();
        private const string VIN = "VIN";
        private List<string> PARAMFLOAT1 = new List<string>() { "CT_EDGL", "CT_JGL", "CT_SJGKRLXHL", "CT_SQGKRLXHL", "CT_ZHGKRLXHL", "FCDS_HHDL_DLXDCZZNL", "FCDS_HHDL_ZHGKRLXHL", "FCDS_HHDL_EDGL", "FCDS_HHDL_JGL", "FCDS_HHDL_SJGKRLXHL", "FCDS_HHDL_SQGKRLXHL", "FCDS_HHDL_QDDJEDGL", "CDS_HHDL_DLXDCZZNL", "FCDS_HHDL_DLXDCBNL", "CDS_HHDL_ZHGKRLXHL", "CDS_HHDL_QDDJEDGL", "CDS_HHDL_EDGL", "CDS_HHDL_JGL", "CDD_DLXDCZEDNL", "CDD_QDDJEDGL", "RLDC_DDGLMD", "RLDC_ZHGKHQL", "RLDC_QDDJEDGL" };
        private List<string> PARAMFLOAT2 = new List<string>() { "CDS_HHDL_HHDLZDDGLB", "FCDS_HHDL_HHDLZDDGLB" };
        public Dictionary<string, string> dictCTNY;  //存放列头转换模板(传统能源)
        public Dictionary<string, string> dictFCDSHHDL;  //存放列头转换模板（非插电式混合动力）
        public Dictionary<string, string> dictCDSHHDL;  //存放列头转换模板（插电式混合动力）
        public Dictionary<string, string> dictCDD;  //存放列头转换模板（纯电动）
        public Dictionary<string, string> dictRLDC;  //存放列头转换模板（燃料电池）
        public Dictionary<string, DataTable> dsMainStatic = new Dictionary<string, DataTable>();
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
        private string mainFileName = FILE_NAME["MAIN"].ToString();

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
        /// 转移已用完的文件
        /// </summary>
        /// <param name="srcFileName">源文件路径</param>
        /// <param name="folderPath">目的文件夹路径</param>
        /// <param name="fileType">文件类型</param>
        public string MoveFinishedFile(string srcFileName, string folderPath, string fileType)
        {
            string msg = string.Empty;
            string folderName = Path.Combine(folderPath, FILE_NAME[fileType]);

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
                msg = ex.Message + Environment.NewLine;
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
                if (clxh == Convert.ToString(dr["CLXH"]).Trim())
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
            message += string.Format("\r\n{0}: 对应车型参数“{1}”不存在", vin, uniqueCode);
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
                DataTable dtCtnyPam = MitsUtils.GetRllxData(CTNY);
                DataTable dtFcdsPam = MitsUtils.GetRllxData(FCDSHHDL);
                DataTable dtCdsPam = MitsUtils.GetRllxData(CDSHHDL);
                DataTable dtCddPam = MitsUtils.GetRllxData(CDD);
                DataTable dtRldcPam = MitsUtils.GetRllxData(RLDC);

                Dictionary<string, DataTable> dicDtPam = new Dictionary<string, DataTable>();
                dicDtPam.Add(CTNY, dtCtnyPam);
                dicDtPam.Add(FCDSHHDL, dtFcdsPam);
                dicDtPam.Add(CDSHHDL, dtCdsPam);
                dicDtPam.Add(CDD, dtCddPam);
                dicDtPam.Add(RLDC, dtRldcPam);

                // 获取节假日数据
                listHoliday = MitsUtils.GetHoliday();

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

                        pf.progressBarControl1.PerformStep();
                        Application.DoEvents();
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + Environment.NewLine;
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
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = con.BeginTransaction();
                try
                {
                    OracleParameter creTime = new OracleParameter("CREATETIME", DateTime.Today);
                    creTime.DbType = DbType.Date;
                    DateTime clzzrqDate;
                    try
                    {
                        clzzrqDate = DateTime.ParseExact(drVin["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    catch (Exception)
                    {
                        clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"]);
                    }
                    OracleParameter clzzrq = new OracleParameter("CLZZRQ", clzzrqDate);
                    clzzrq.DbType = DbType.Date;
                    string sqlDel = String.Format("DELETE FROM VIN_INFO WHERE VIN = '{0}'", vin);
                    OracleHelper.ExecuteNonQuery(tra, sqlDel, null);

                    OracleParameter[] vinParamList = { 
                                    new OracleParameter("VIN",vin),
                                    new OracleParameter("CLXH",drVin["CLXH"].ToString().Trim()),
                                    clzzrq,
                                    new OracleParameter("STATUS","1"),
                                    creTime,
                                    new OracleParameter("RLLX",drVin["RLLX"].ToString().Trim()),
                                    new OracleParameter("UNIQUE_CODE",drVin["UNIQUE_CODE"].ToString().Trim())
                                };
                    OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO VIN_INFO(VIN,CLXH,CLZZRQ,STATUS,CREATETIME,RLLX,UNIQUE_CODE) Values (:VIN, :CLXH,:CLZZRQ,:STATUS,:CREATETIME,:RLLX,:UNIQUE_CODE)", vinParamList);
                    tra.Commit();
                }
                catch (Exception ex)
                {
                    genMsg += ex.Message + Environment.NewLine;
                    tra.Rollback();
                }
            }
            return genMsg;
        }

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
                }
            }
            catch { }
            return vinStr;
        }

        //批量更新V_ID
        private bool UpdateV_ID(DataTable dt)
        {
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = con.BeginTransaction();
                try
                {

                    foreach (DataRow dr in dt.Rows)
                    {
                        OracleHelper.ExecuteNonQuery(tra, string.Format("update FC_CLJBXX set V_ID='{0}',STATUS='0' where VIN='{1}'", dr[1], dr[0]));
                    }
                    tra.Commit();
                    return true;
                }
                catch
                {
                    tra.Rollback();
                }
            }
            return false;
        }

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

                        DataSet tempDt = Utils.service.QueryVidByVins(Settings.Default.UserId, Settings.Default.UserPWD, vin);
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
        /// <param name="fileName">文件名</param>
        /// <param name="folderName">文件所在文件夹</param>
        /// <param name="importType">导入、修改</param>
        /// <returns></returns>
        public string ImportMainData(string fileName, string folderName, string importType)
        {
            string rtnMsg = string.Empty;
            DataSet ds = ImportExcel.ReadExcelToDataSet(fileName);
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
                    rtnMsg += this.UpdateMainData2(ds);
                }

                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_MAIN");
                }
                else
                {
                    rtnMsg = String.Format("{0}{1}\r\n", Path.GetFileName(fileName), rtnMsg);
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
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
                succCount += ImpMainData(dtCtny, CTNY, ref msg);

                DataTable dtFcds = D2D(dictFCDSHHDL, ds.Tables[FCDSHHDL], FCDSHHDL);
                totalCount += dtFcds.Rows.Count;
                succCount += ImpMainData(dtFcds, FCDSHHDL, ref msg);

                DataTable dtCds = D2D(dictCDSHHDL, ds.Tables[CDSHHDL], CDSHHDL);
                totalCount += dtCds.Rows.Count;
                succCount += ImpMainData(dtCds, CDSHHDL, ref msg);

                DataTable dtCdd = D2D(dictCDD, ds.Tables[CDD], CDD);
                totalCount += dtCdd.Rows.Count;
                succCount += ImpMainData(dtCdd, CDD, ref msg);

                DataTable dtRldc = D2D(dictRLDC, ds.Tables[RLDC], RLDC);
                totalCount += dtRldc.Rows.Count;
                succCount += ImpMainData(dtRldc, RLDC, ref msg);

            }
            catch (Exception ex)
            {
                msg += ex.Message + Environment.NewLine;
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

        public int ImpMainData(DataTable dt, string rlzl, ref string msg)
        {
            int succCount = 0;
            if (string.IsNullOrEmpty(msg))
            {
                msg = string.Empty;
            }

            try
            {
                // 转换表头（用户模板中的表头转为数据库列名）
                DataTable checkData = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from RLLX_PARAM", null).Tables[0];
                DataRow[] tdr = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", rlzl));

                if (dt != null && dt.Rows.Count > 0)
                {
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
                            #region 定编、选装、车型号的重复检验
                            try
                            {
                                String sql = string.Empty;
                                sql = string.Format("select * from CTNY_MAIN where (CAR_CODE='{0}' and OPTION_CODE='{1}' and ATTRIBUTE_CODE='{2}') or (UNIQUE_CODE='{3}')", dr["CAR_CODE"], dr["OPTION_CODE"], dr["ATTRIBUTE_CODE"], dr["UNIQUE_CODE"]);
                                if (OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null) != null && OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null).Tables[0].Rows.Count > 0)
                                {
                                    msg += string.Format("{0}唯一标识为【{1}】在传统能源基础数据中已存在!\r\n", rlzl, dr["UNIQUE_CODE"]);
                                    continue;
                                }
                                sql = string.Format("select * from FCDS_MAIN where (CAR_CODE='{0}' and OPTION_CODE='{1}' and ATTRIBUTE_CODE='{2}') or (UNIQUE_CODE='{3}')", dr["CAR_CODE"], dr["OPTION_CODE"], dr["ATTRIBUTE_CODE"], dr["UNIQUE_CODE"]);
                                if (OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null) != null && OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null).Tables[0].Rows.Count > 0)
                                {
                                    msg += string.Format("{0}唯一标识为【{1}】在非插电式混合动力基础数据中已存在!\r\n", rlzl, dr["UNIQUE_CODE"]);
                                    continue;
                                }
                                sql = string.Format("select * from CDS_MAIN where (CAR_CODE='{0}' and OPTION_CODE='{1}' and ATTRIBUTE_CODE='{2}') or (UNIQUE_CODE='{3}')", dr["CAR_CODE"], dr["OPTION_CODE"], dr["ATTRIBUTE_CODE"], dr["UNIQUE_CODE"]);
                                if (OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null) != null && OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null).Tables[0].Rows.Count > 0)
                                {
                                    msg += string.Format("{0}唯一标识为【{1}】在插电式混合动力基础数据中已存在!\r\n", rlzl, dr["UNIQUE_CODE"]);
                                    continue;
                                }
                                sql = string.Format("select * from CDD_MAIN where (CAR_CODE='{0}' and OPTION_CODE='{1}' and ATTRIBUTE_CODE='{2}') or (UNIQUE_CODE='{3}')", dr["CAR_CODE"], dr["OPTION_CODE"], dr["ATTRIBUTE_CODE"], dr["UNIQUE_CODE"]);
                                if (OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null) != null && OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null).Tables[0].Rows.Count > 0)
                                {
                                    msg += string.Format("{0}唯一标识为【{1}】在纯电动基础数据中已存在!\r\n", rlzl, dr["UNIQUE_CODE"]);
                                    continue;
                                }
                                sql = string.Format("select * from RLDC_MAIN where (CAR_CODE='{0}' and OPTION_CODE='{1}' and ATTRIBUTE_CODE='{2}') or (UNIQUE_CODE='{3}')", dr["CAR_CODE"], dr["OPTION_CODE"], dr["ATTRIBUTE_CODE"], dr["UNIQUE_CODE"]);
                                if (OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null) != null && OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null).Tables[0].Rows.Count > 0)
                                {
                                    msg += string.Format("{0}唯一标识为【{1}】在燃料电池基础数据中已存在!\r\n", rlzl, dr["UNIQUE_CODE"]);
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                msg += ex.Message + Environment.NewLine;
                            }
                            #endregion
                            if (rlzl.Equals(CTNY))
                            {
                                #region 传统能源
                                #region insert
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("insert into CTNY_MAIN(");
                                    strSql.Append("UNIQUE_CODE,CAR_CODE,OPTION_CODE,ATTRIBUTE_CODE,QCSCQY,JYBGBH,JKQCZJXS,CLXH,HGSPBM,CLZL,YYC,QDXS,ZWPS,ZCZBZL,ZDSJZZL,ZGCS,EDZK,LTGG,LJ,JYJGMC,TYMC,ZJ,RLLX,CT_BSQXS,CT_EDGL,CT_FDJXH,CT_JGL,CT_PL,CT_QGS,CT_QTXX,CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKCO2PFL,CT_ZHGKRLXHL,CT_BSQDWS,CREATE_TIME,CREATE_BY,UPDATE_TIME,UPDATE_BY,QTXX)");
                                    strSql.Append(" values (");
                                    strSql.Append(":UNIQUE_CODE,:CAR_CODE,:OPTION_CODE,:ATTRIBUTE_CODE,:QCSCQY,:JYBGBH,:JKQCZJXS,:CLXH,:HGSPBM,:CLZL,:YYC,:QDXS,:ZWPS,:ZCZBZL,:ZDSJZZL,:ZGCS,:EDZK,:LTGG,:LJ,:JYJGMC,:TYMC,:ZJ,:RLLX,:CT_BSQXS,:CT_EDGL,:CT_FDJXH,:CT_JGL,:CT_PL,:CT_QGS,:CT_QTXX,:CT_SJGKRLXHL,:CT_SQGKRLXHL,:CT_ZHGKCO2PFL,:CT_ZHGKRLXHL,:CT_BSQDWS,:CREATE_TIME,:CREATE_BY,:UPDATE_TIME,:UPDATE_BY,:QTXX)");
                                    OracleParameter[] parameters = {
					                    new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2,255),
					                    new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                    new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                    new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                    new OracleParameter("QCSCQY", OracleDbType.Varchar2,255),
					                    new OracleParameter("JYBGBH", OracleDbType.Varchar2,255),
					                    new OracleParameter("JKQCZJXS", OracleDbType.Varchar2,255),
					                    new OracleParameter("CLXH", OracleDbType.Varchar2,255),
					                    new OracleParameter("HGSPBM", OracleDbType.Varchar2,255),
					                    new OracleParameter("CLZL", OracleDbType.Varchar2,255),
					                    new OracleParameter("YYC", OracleDbType.Varchar2,255),
					                    new OracleParameter("QDXS", OracleDbType.Varchar2,255),
					                    new OracleParameter("ZWPS", OracleDbType.Varchar2,255),
					                    new OracleParameter("ZCZBZL", OracleDbType.Varchar2,255),
					                    new OracleParameter("ZDSJZZL", OracleDbType.Varchar2,255),
					                    new OracleParameter("ZGCS", OracleDbType.Varchar2,255),
					                    new OracleParameter("EDZK", OracleDbType.Varchar2,255),
					                    new OracleParameter("LTGG", OracleDbType.Varchar2,255),
					                    new OracleParameter("LJ", OracleDbType.Varchar2,255),
					                    new OracleParameter("JYJGMC", OracleDbType.Varchar2,255),
					                    new OracleParameter("TYMC", OracleDbType.Varchar2,255),
					                    new OracleParameter("ZJ", OracleDbType.Varchar2,255),
					                    new OracleParameter("RLLX", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_BSQXS", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_EDGL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_FDJXH", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_JGL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_PL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_QGS", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_QTXX", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_SJGKRLXHL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_SQGKRLXHL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_ZHGKCO2PFL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_ZHGKRLXHL", OracleDbType.Varchar2,255),
					                    new OracleParameter("CT_BSQDWS", OracleDbType.Varchar2,255),
					                    new OracleParameter("CREATE_TIME", OracleDbType.Date),
					                    new OracleParameter("CREATE_BY", OracleDbType.Varchar2,255),
					                    new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                    new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                    new OracleParameter("QTXX", OracleDbType.Varchar2,255)
                                    };
                                    parameters[0].Value = dr["UNIQUE_CODE"];
                                    parameters[1].Value = dr["CAR_CODE"];
                                    parameters[2].Value = dr["OPTION_CODE"];
                                    parameters[3].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[4].Value = dr["QCSCQY"];
                                    parameters[5].Value = dr["JYBGBH"];
                                    parameters[6].Value = dr["JKQCZJXS"];
                                    parameters[7].Value = dr["CLXH"];
                                    parameters[8].Value = dr["HGSPBM"];
                                    parameters[9].Value = dr["CLZL"];
                                    parameters[10].Value = dr["YYC"];
                                    parameters[11].Value = dr["QDXS"];
                                    parameters[12].Value = dr["ZWPS"];
                                    parameters[13].Value = dr["ZCZBZL"];
                                    parameters[14].Value = dr["ZDSJZZL"];
                                    parameters[15].Value = dr["ZGCS"];
                                    parameters[16].Value = dr["EDZK"];
                                    parameters[17].Value = dr["LTGG"];
                                    parameters[18].Value = dr["LJ"];
                                    parameters[19].Value = dr["JYJGMC"];
                                    parameters[20].Value = dr["TYMC"];
                                    parameters[21].Value = dr["ZJ"];
                                    parameters[22].Value = dr["RLLX"];
                                    parameters[23].Value = dr["CT_BSQXS"];
                                    parameters[24].Value = dr["CT_EDGL"];
                                    parameters[25].Value = dr["CT_FDJXH"];
                                    parameters[26].Value = dr["CT_JGL"];
                                    parameters[27].Value = dr["CT_PL"];
                                    parameters[28].Value = dr["CT_QGS"];
                                    parameters[29].Value = dr["CT_QTXX"];
                                    parameters[30].Value = dr["CT_SJGKRLXHL"];
                                    parameters[31].Value = dr["CT_SQGKRLXHL"];
                                    parameters[32].Value = dr["CT_ZHGKCO2PFL"];
                                    parameters[33].Value = dr["CT_ZHGKRLXHL"];
                                    parameters[34].Value = dr["CT_BSQDWS"];
                                    parameters[35].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[36].Value = Utils.localUserId;
                                    parameters[37].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[38].Value = Utils.localUserId;
                                    parameters[39].Value = dr["QTXX"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(FCDSHHDL))
                            {
                                #region 非插电式混合动力
                                #region insert
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("insert into FCDS_MAIN(");
                                    strSql.Append("UNIQUE_CODE,CAR_CODE,OPTION_CODE,ATTRIBUTE_CODE,QCSCQY,JYBGBH,JKQCZJXS,CLXH,HGSPBM,CLZL,YYC,QDXS,ZWPS,ZCZBZL,ZDSJZZL,ZGCS,EDZK,LTGG,LJ,JYJGMC,TYMC,ZJ,RLLX,FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS,FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC,FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY,FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL,FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH,FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB,FCDS_HHDL_JGL,FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL,FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS,FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL,FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHGKRLXHL,FCDS_HHDL_ZHKGCO2PL,CREATE_TIME,CREATE_BY,UPDATE_TIME,UPDATE_BY,QTXX)");
                                    strSql.Append(" values (");
                                    strSql.Append(":UNIQUE_CODE,:CAR_CODE,:OPTION_CODE,:ATTRIBUTE_CODE,:QCSCQY,:JYBGBH,:JKQCZJXS,:CLXH,:HGSPBM,:CLZL,:YYC,:QDXS,:ZWPS,:ZCZBZL,:ZDSJZZL,:ZGCS,:EDZK,:LTGG,:LJ,:JYJGMC,:TYMC,:ZJ,:RLLX,:FCDS_HHDL_BSQDWS,:FCDS_HHDL_BSQXS,:FCDS_HHDL_CDDMSXZGCS,:FCDS_HHDL_CDDMSXZHGKXSLC,:FCDS_HHDL_DLXDCBNL,:FCDS_HHDL_DLXDCZBCDY,:FCDS_HHDL_DLXDCZZL,:FCDS_HHDL_DLXDCZZNL,:FCDS_HHDL_EDGL,:FCDS_HHDL_FDJXH,:FCDS_HHDL_HHDLJGXS,:FCDS_HHDL_HHDLZDDGLB,:FCDS_HHDL_JGL,:FCDS_HHDL_PL,:FCDS_HHDL_QDDJEDGL,:FCDS_HHDL_QDDJFZNJ,:FCDS_HHDL_QDDJLX,:FCDS_HHDL_QGS,:FCDS_HHDL_SJGKRLXHL,:FCDS_HHDL_SQGKRLXHL,:FCDS_HHDL_XSMSSDXZGN,:FCDS_HHDL_ZHGKRLXHL,:FCDS_HHDL_ZHKGCO2PL,:CREATE_TIME,:CREATE_BY,:UPDATE_TIME,:UPDATE_BY,:QTXX)");
                                    OracleParameter[] parameters = {
					                new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2,255),
                                    new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("QCSCQY", OracleDbType.Varchar2,255),
					                new OracleParameter("JYBGBH", OracleDbType.Varchar2,255),
					                new OracleParameter("JKQCZJXS", OracleDbType.Varchar2,255),
					                new OracleParameter("CLXH", OracleDbType.Varchar2,255),
					                new OracleParameter("HGSPBM", OracleDbType.Varchar2,255),
					                new OracleParameter("CLZL", OracleDbType.Varchar2,255),
					                new OracleParameter("YYC", OracleDbType.Varchar2,255),
					                new OracleParameter("QDXS", OracleDbType.Varchar2,255),
					                new OracleParameter("ZWPS", OracleDbType.Varchar2,255),
					                new OracleParameter("ZCZBZL", OracleDbType.Varchar2,255),
					                new OracleParameter("ZDSJZZL", OracleDbType.Varchar2,255),
					                new OracleParameter("ZGCS", OracleDbType.Varchar2,255),
					                new OracleParameter("EDZK", OracleDbType.Varchar2,255),
					                new OracleParameter("LTGG", OracleDbType.Varchar2,255),
					                new OracleParameter("LJ", OracleDbType.Varchar2,255),
					                new OracleParameter("JYJGMC", OracleDbType.Varchar2,255),
					                new OracleParameter("TYMC", OracleDbType.Varchar2,255),
					                new OracleParameter("ZJ", OracleDbType.Varchar2,255),
					                new OracleParameter("RLLX", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_BSQDWS", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_BSQXS", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_CDDMSXZGCS", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_CDDMSXZHGKXSLC", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_DLXDCBNL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_DLXDCZBCDY", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_DLXDCZZL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_DLXDCZZNL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_EDGL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_FDJXH", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_HHDLJGXS", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_HHDLZDDGLB", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_JGL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_PL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_QDDJEDGL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_QDDJFZNJ", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_QDDJLX", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_QGS", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_SJGKRLXHL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_SQGKRLXHL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_XSMSSDXZGN", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_ZHGKRLXHL", OracleDbType.Varchar2,255),
					                new OracleParameter("FCDS_HHDL_ZHKGCO2PL", OracleDbType.Varchar2,255),
					                new OracleParameter("CREATE_TIME", OracleDbType.Date),
					                new OracleParameter("CREATE_BY", OracleDbType.Varchar2,255),
					                new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                new OracleParameter("QTXX", OracleDbType.Varchar2,255)};
                                    parameters[0].Value = dr["UNIQUE_CODE"];
                                    parameters[1].Value = dr["CAR_CODE"];
                                    parameters[2].Value = dr["OPTION_CODE"];
                                    parameters[3].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[4].Value = dr["QCSCQY"];
                                    parameters[5].Value = dr["JYBGBH"];
                                    parameters[6].Value = dr["JKQCZJXS"];
                                    parameters[7].Value = dr["CLXH"];
                                    parameters[8].Value = dr["HGSPBM"];
                                    parameters[9].Value = dr["CLZL"];
                                    parameters[10].Value = dr["YYC"];
                                    parameters[11].Value = dr["QDXS"];
                                    parameters[12].Value = dr["ZWPS"];
                                    parameters[13].Value = dr["ZCZBZL"];
                                    parameters[14].Value = dr["ZDSJZZL"];
                                    parameters[15].Value = dr["ZGCS"];
                                    parameters[16].Value = dr["EDZK"];
                                    parameters[17].Value = dr["LTGG"];
                                    parameters[18].Value = dr["LJ"];
                                    parameters[19].Value = dr["JYJGMC"];
                                    parameters[20].Value = dr["TYMC"];
                                    parameters[21].Value = dr["ZJ"];
                                    parameters[22].Value = dr["RLLX"];
                                    parameters[23].Value = dr["FCDS_HHDL_BSQDWS"];
                                    parameters[24].Value = dr["FCDS_HHDL_BSQXS"];
                                    parameters[25].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                                    parameters[26].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                                    parameters[27].Value = dr["FCDS_HHDL_DLXDCBNL"];
                                    parameters[28].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                                    parameters[29].Value = dr["FCDS_HHDL_DLXDCZZL"];
                                    parameters[30].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                                    parameters[31].Value = dr["FCDS_HHDL_EDGL"];
                                    parameters[32].Value = dr["FCDS_HHDL_FDJXH"];
                                    parameters[33].Value = dr["FCDS_HHDL_HHDLJGXS"];
                                    parameters[34].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                                    parameters[35].Value = dr["FCDS_HHDL_JGL"];
                                    parameters[36].Value = dr["FCDS_HHDL_PL"];
                                    parameters[37].Value = dr["FCDS_HHDL_QDDJEDGL"];
                                    parameters[38].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                                    parameters[39].Value = dr["FCDS_HHDL_QDDJLX"];
                                    parameters[40].Value = dr["FCDS_HHDL_QGS"];
                                    parameters[41].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                                    parameters[42].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                                    parameters[43].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                                    parameters[44].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                                    parameters[45].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                                    parameters[46].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[47].Value = Utils.localUserId;
                                    parameters[48].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[49].Value = Utils.localUserId;
                                    parameters[50].Value = dr["QTXX"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(CDSHHDL))
                            {
                                #region 插电式混合动力
                                #region insert
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("insert into CDS_MAIN(");
                                    strSql.Append("UNIQUE_CODE,CAR_CODE,OPTION_CODE,ATTRIBUTE_CODE,QCSCQY,JYBGBH,JKQCZJXS,CLXH,HGSPBM,CLZL,YYC,QDXS,ZWPS,ZCZBZL,ZDSJZZL,ZGCS,EDZK,LTGG,LJ,JYJGMC,TYMC,ZJ,RLLX,CDS_HHDL_BSQDWS,CDS_HHDL_BSQXS,CDS_HHDL_CDDMSXZGCS,CDS_HHDL_CDDMSXZHGKXSLC,CDS_HHDL_DLXDCBNL,CDS_HHDL_DLXDCZBCDY,CDS_HHDL_DLXDCZZL,CDS_HHDL_DLXDCZZNL,CDS_HHDL_EDGL,CDS_HHDL_FDJXH,CDS_HHDL_HHDLJGXS,CDS_HHDL_HHDLZDDGLB,CDS_HHDL_JGL,CDS_HHDL_PL,CDS_HHDL_QDDJEDGL,CDS_HHDL_QDDJFZNJ,CDS_HHDL_QDDJLX,CDS_HHDL_QGS,CDS_HHDL_XSMSSDXZGN,CDS_HHDL_ZHGKDNXHL,CDS_HHDL_ZHGKRLXHL,CDS_HHDL_ZHKGCO2PL,CREATE_TIME,CREATE_BY,UPDATE_TIME,UPDATE_BY,QTXX)");
                                    strSql.Append(" values (");
                                    strSql.Append(":UNIQUE_CODE,:CAR_CODE,:OPTION_CODE,:ATTRIBUTE_CODE,:QCSCQY,:JYBGBH,:JKQCZJXS,:CLXH,:HGSPBM,:CLZL,:YYC,:QDXS,:ZWPS,:ZCZBZL,:ZDSJZZL,:ZGCS,:EDZK,:LTGG,:LJ,:JYJGMC,:TYMC,:ZJ,:RLLX,:CDS_HHDL_BSQDWS,:CDS_HHDL_BSQXS,:CDS_HHDL_CDDMSXZGCS,:CDS_HHDL_CDDMSXZHGKXSLC,:CDS_HHDL_DLXDCBNL,:CDS_HHDL_DLXDCZBCDY,:CDS_HHDL_DLXDCZZL,:CDS_HHDL_DLXDCZZNL,:CDS_HHDL_EDGL,:CDS_HHDL_FDJXH,:CDS_HHDL_HHDLJGXS,:CDS_HHDL_HHDLZDDGLB,:CDS_HHDL_JGL,:CDS_HHDL_PL,:CDS_HHDL_QDDJEDGL,:CDS_HHDL_QDDJFZNJ,:CDS_HHDL_QDDJLX,:CDS_HHDL_QGS,:CDS_HHDL_XSMSSDXZGN,:CDS_HHDL_ZHGKDNXHL,:CDS_HHDL_ZHGKRLXHL,:CDS_HHDL_ZHKGCO2PL,:CREATE_TIME,:CREATE_BY,:UPDATE_TIME,:UPDATE_BY,:QTXX)");
                                    OracleParameter[] parameters = {
					                new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2,255),
                                    new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("QCSCQY", OracleDbType.Varchar2,255),
					                new OracleParameter("JYBGBH", OracleDbType.Varchar2,255),
					                new OracleParameter("JKQCZJXS", OracleDbType.Varchar2,255),
					                new OracleParameter("CLXH", OracleDbType.Varchar2,255),
					                new OracleParameter("HGSPBM", OracleDbType.Varchar2,255),
					                new OracleParameter("CLZL", OracleDbType.Varchar2,255),
					                new OracleParameter("YYC", OracleDbType.Varchar2,255),
					                new OracleParameter("QDXS", OracleDbType.Varchar2,255),
					                new OracleParameter("ZWPS", OracleDbType.Varchar2,255),
					                new OracleParameter("ZCZBZL", OracleDbType.Varchar2,255),
					                new OracleParameter("ZDSJZZL", OracleDbType.Varchar2,255),
					                new OracleParameter("ZGCS", OracleDbType.Varchar2,255),
					                new OracleParameter("EDZK", OracleDbType.Varchar2,255),
					                new OracleParameter("LTGG", OracleDbType.Varchar2,255),
					                new OracleParameter("LJ", OracleDbType.Varchar2,255),
					                new OracleParameter("JYJGMC", OracleDbType.Varchar2,255),
					                new OracleParameter("TYMC", OracleDbType.Varchar2,255),
					                new OracleParameter("ZJ", OracleDbType.Varchar2,255),
					                new OracleParameter("RLLX", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_BSQDWS", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_BSQXS", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_CDDMSXZGCS", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_CDDMSXZHGKXSLC", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_DLXDCBNL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_DLXDCZBCDY", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_DLXDCZZL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_DLXDCZZNL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_EDGL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_FDJXH", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_HHDLJGXS", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_HHDLZDDGLB", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_JGL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_PL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_QDDJEDGL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_QDDJFZNJ", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_QDDJLX", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_QGS", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_XSMSSDXZGN", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_ZHGKDNXHL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_ZHGKRLXHL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDS_HHDL_ZHKGCO2PL", OracleDbType.Varchar2,255),
					                new OracleParameter("CREATE_TIME", OracleDbType.Date),
					                new OracleParameter("CREATE_BY", OracleDbType.Varchar2,255),
					                new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                new OracleParameter("QTXX", OracleDbType.Varchar2,255)
                                };
                                    parameters[0].Value = dr["UNIQUE_CODE"];
                                    parameters[1].Value = dr["CAR_CODE"];
                                    parameters[2].Value = dr["OPTION_CODE"];
                                    parameters[3].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[4].Value = dr["QCSCQY"];
                                    parameters[5].Value = dr["JYBGBH"];
                                    parameters[6].Value = dr["JKQCZJXS"];
                                    parameters[7].Value = dr["CLXH"];
                                    parameters[8].Value = dr["HGSPBM"];
                                    parameters[9].Value = dr["CLZL"];
                                    parameters[10].Value = dr["YYC"];
                                    parameters[11].Value = dr["QDXS"];
                                    parameters[12].Value = dr["ZWPS"];
                                    parameters[13].Value = dr["ZCZBZL"];
                                    parameters[14].Value = dr["ZDSJZZL"];
                                    parameters[15].Value = dr["ZGCS"];
                                    parameters[16].Value = dr["EDZK"];
                                    parameters[17].Value = dr["LTGG"];
                                    parameters[18].Value = dr["LJ"];
                                    parameters[19].Value = dr["JYJGMC"];
                                    parameters[20].Value = dr["TYMC"];
                                    parameters[21].Value = dr["ZJ"];
                                    parameters[22].Value = dr["RLLX"];
                                    parameters[23].Value = dr["CDS_HHDL_BSQDWS"];
                                    parameters[24].Value = dr["CDS_HHDL_BSQXS"];
                                    parameters[25].Value = dr["CDS_HHDL_CDDMSXZGCS"];
                                    parameters[26].Value = dr["CDS_HHDL_CDDMSXZHGKXSLC"];
                                    parameters[27].Value = dr["CDS_HHDL_DLXDCBNL"];
                                    parameters[28].Value = dr["CDS_HHDL_DLXDCZBCDY"];
                                    parameters[29].Value = dr["CDS_HHDL_DLXDCZZL"];
                                    parameters[30].Value = dr["CDS_HHDL_DLXDCZZNL"];
                                    parameters[31].Value = dr["CDS_HHDL_EDGL"];
                                    parameters[32].Value = dr["CDS_HHDL_FDJXH"];
                                    parameters[33].Value = dr["CDS_HHDL_HHDLJGXS"];
                                    parameters[34].Value = dr["CDS_HHDL_HHDLZDDGLB"];
                                    parameters[35].Value = dr["CDS_HHDL_JGL"];
                                    parameters[36].Value = dr["CDS_HHDL_PL"];
                                    parameters[37].Value = dr["CDS_HHDL_QDDJEDGL"];
                                    parameters[38].Value = dr["CDS_HHDL_QDDJFZNJ"];
                                    parameters[39].Value = dr["CDS_HHDL_QDDJLX"];
                                    parameters[40].Value = dr["CDS_HHDL_QGS"];
                                    parameters[41].Value = dr["CDS_HHDL_XSMSSDXZGN"];
                                    parameters[42].Value = dr["CDS_HHDL_ZHGKDNXHL"];
                                    parameters[43].Value = dr["CDS_HHDL_ZHGKRLXHL"];
                                    parameters[44].Value = dr["CDS_HHDL_ZHKGCO2PL"];
                                    parameters[45].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[46].Value = Utils.localUserId;
                                    parameters[47].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[48].Value = Utils.localUserId;
                                    parameters[49].Value = dr["QTXX"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(CDD))
                            {
                                #region 纯电动
                                #region insert
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("insert into CDD_MAIN(");
                                    strSql.Append("UNIQUE_CODE,CAR_CODE,OPTION_CODE,ATTRIBUTE_CODE,QCSCQY,JYBGBH,JKQCZJXS,CLXH,HGSPBM,CLZL,YYC,QDXS,ZWPS,ZCZBZL,ZDSJZZL,ZGCS,EDZK,LTGG,LJ,JYJGMC,TYMC,ZJ,RLLX,CDD_DDQC30FZZGCS,CDD_DDXDCZZLYZCZBZLDBZ,CDD_DLXDCBNL,CDD_DLXDCZBCDY,CDD_DLXDCZEDNL,CDD_DLXDCZZL,CDD_QDDJEDGL,CDD_QDDJFZNJ,CDD_QDDJLX,CDD_ZHGKDNXHL,CDD_ZHGKXSLC,CREATE_TIME,CREATE_BY,UPDATE_TIME,UPDATE_BY,QTXX)");
                                    strSql.Append(" values (");
                                    strSql.Append(":UNIQUE_CODE,:CAR_CODE,:OPTION_CODE,:ATTRIBUTE_CODE,:QCSCQY,:JYBGBH,:JKQCZJXS,:CLXH,:HGSPBM,:CLZL,:YYC,:QDXS,:ZWPS,:ZCZBZL,:ZDSJZZL,:ZGCS,:EDZK,:LTGG,:LJ,:JYJGMC,:TYMC,:ZJ,:RLLX,:CDD_DDQC30FZZGCS,:CDD_DDXDCZZLYZCZBZLDBZ,:CDD_DLXDCBNL,:CDD_DLXDCZBCDY,:CDD_DLXDCZEDNL,:CDD_DLXDCZZL,:CDD_QDDJEDGL,:CDD_QDDJFZNJ,:CDD_QDDJLX,:CDD_ZHGKDNXHL,:CDD_ZHGKXSLC,:CREATE_TIME,:CREATE_BY,:UPDATE_TIME,:UPDATE_BY,:QTXX)");
                                    OracleParameter[] parameters = {
					                new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2,255),
                                    new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                new OracleParameter("QCSCQY", OracleDbType.Varchar2,255),
					                new OracleParameter("JYBGBH", OracleDbType.Varchar2,255),
					                new OracleParameter("JKQCZJXS", OracleDbType.Varchar2,255),
					                new OracleParameter("CLXH", OracleDbType.Varchar2,255),
					                new OracleParameter("HGSPBM", OracleDbType.Varchar2,255),
					                new OracleParameter("CLZL", OracleDbType.Varchar2,255),
					                new OracleParameter("YYC", OracleDbType.Varchar2,255),
					                new OracleParameter("QDXS", OracleDbType.Varchar2,255),
					                new OracleParameter("ZWPS", OracleDbType.Varchar2,255),
					                new OracleParameter("ZCZBZL", OracleDbType.Varchar2,255),
					                new OracleParameter("ZDSJZZL", OracleDbType.Varchar2,255),
					                new OracleParameter("ZGCS", OracleDbType.Varchar2,255),
					                new OracleParameter("EDZK", OracleDbType.Varchar2,255),
					                new OracleParameter("LTGG", OracleDbType.Varchar2,255),
					                new OracleParameter("LJ", OracleDbType.Varchar2,255),
					                new OracleParameter("JYJGMC", OracleDbType.Varchar2,255),
					                new OracleParameter("TYMC", OracleDbType.Varchar2,255),
					                new OracleParameter("ZJ", OracleDbType.Varchar2,255),
					                new OracleParameter("RLLX", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_DDQC30FZZGCS", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_DDXDCZZLYZCZBZLDBZ", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_DLXDCBNL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_DLXDCZBCDY", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_DLXDCZEDNL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_DLXDCZZL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_QDDJEDGL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_QDDJFZNJ", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_QDDJLX", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_ZHGKDNXHL", OracleDbType.Varchar2,255),
					                new OracleParameter("CDD_ZHGKXSLC", OracleDbType.Varchar2,255),
					                new OracleParameter("CREATE_TIME", OracleDbType.Date),
					                new OracleParameter("CREATE_BY", OracleDbType.Varchar2,255),
					                new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                new OracleParameter("QTXX", OracleDbType.Varchar2,255)
                                };
                                    parameters[0].Value = dr["UNIQUE_CODE"];
                                    parameters[1].Value = dr["CAR_CODE"];
                                    parameters[2].Value = dr["OPTION_CODE"];
                                    parameters[3].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[4].Value = dr["QCSCQY"];
                                    parameters[5].Value = dr["JYBGBH"];
                                    parameters[6].Value = dr["JKQCZJXS"];
                                    parameters[7].Value = dr["CLXH"];
                                    parameters[8].Value = dr["HGSPBM"];
                                    parameters[9].Value = dr["CLZL"];
                                    parameters[10].Value = dr["YYC"];
                                    parameters[11].Value = dr["QDXS"];
                                    parameters[12].Value = dr["ZWPS"];
                                    parameters[13].Value = dr["ZCZBZL"];
                                    parameters[14].Value = dr["ZDSJZZL"];
                                    parameters[15].Value = dr["ZGCS"];
                                    parameters[16].Value = dr["EDZK"];
                                    parameters[17].Value = dr["LTGG"];
                                    parameters[18].Value = dr["LJ"];
                                    parameters[19].Value = dr["JYJGMC"];
                                    parameters[20].Value = dr["TYMC"];
                                    parameters[21].Value = dr["ZJ"];
                                    parameters[22].Value = dr["RLLX"];
                                    parameters[23].Value = dr["CDD_DDQC30FZZGCS"];
                                    parameters[24].Value = dr["CDD_DDXDCZZLYZCZBZLDBZ"];
                                    parameters[25].Value = dr["CDD_DLXDCBNL"];
                                    parameters[26].Value = dr["CDD_DLXDCZBCDY"];
                                    parameters[27].Value = dr["CDD_DLXDCZEDNL"];
                                    parameters[28].Value = dr["CDD_DLXDCZZL"];
                                    parameters[29].Value = dr["CDD_QDDJEDGL"];
                                    parameters[30].Value = dr["CDD_QDDJFZNJ"];
                                    parameters[31].Value = dr["CDD_QDDJLX"];
                                    parameters[32].Value = dr["CDD_ZHGKDNXHL"];
                                    parameters[33].Value = dr["CDD_ZHGKXSLC"];
                                    parameters[34].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[35].Value = Utils.localUserId;
                                    parameters[36].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[37].Value = Utils.localUserId;
                                    parameters[38].Value = dr["QTXX"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(RLDC))
                            {
                                #region 燃料电池
                                #region insert
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("insert into RLDC_MAIN(");
                                    strSql.Append("UNIQUE_CODE,CAR_CODE,OPTION_CODE,ATTRIBUTE_CODE,QCSCQY,JYBGBH,JKQCZJXS,CLXH,HGSPBM,CLZL,YYC,QDXS,ZWPS,ZCZBZL,ZDSJZZL,ZGCS,EDZK,LTGG,LJ,JYJGMC,TYMC,ZJ,RLLX,RLDC_CDDMSXZGXSCS,RLDC_CQPBCGZYL,RLDC_CQPRJ,RLDC_DDGLMD,RLDC_CQPLX,RLDC_DDHHJSTJXXDCZBNL,RLDC_DLXDCZZL,RLDC_QDDJEDGL,RLDC_QDDJFZNJ,RLDC_QDDJLX,RLDC_RLLX,RLDC_ZHGKHQL,RLDC_ZHGKXSLC,CREATE_TIME,CREATE_BY,UPDATE_TIME,UPDATE_BY,QTXX)");
                                    strSql.Append(" values (");
                                    strSql.Append(":UNIQUE_CODE,:CAR_CODE,:OPTION_CODE,:ATTRIBUTE_CODE,:QCSCQY,:JYBGBH,:JKQCZJXS,:CLXH,:HGSPBM,:CLZL,:YYC,:QDXS,:ZWPS,:ZCZBZL,:ZDSJZZL,:ZGCS,:EDZK,:LTGG,:LJ,:JYJGMC,:TYMC,:ZJ,:RLLX,:RLDC_CDDMSXZGXSCS,:RLDC_CQPBCGZYL,:RLDC_CQPRJ,:RLDC_DDGLMD,:RLDC_CQPLX,:RLDC_DDHHJSTJXXDCZBNL,:RLDC_DLXDCZZL,:RLDC_QDDJEDGL,:RLDC_QDDJFZNJ,:RLDC_QDDJLX,:RLDC_RLLX,:RLDC_ZHGKHQL,:RLDC_ZHGKXSLC,:CREATE_TIME,:CREATE_BY,:UPDATE_TIME,:UPDATE_BY,:QTXX)");
                                    OracleParameter[] parameters = {
					                        new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("QCSCQY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYBGBH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JKQCZJXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("HGSPBM", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("YYC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("QDXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZWPS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZCZBZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZDSJZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("EDZK", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LTGG", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYJGMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("TYMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CDDMSXZGXSCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CQPBCGZYL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CQPRJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_DDGLMD", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CQPLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_DDHHJSTJXXDCZBNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_DLXDCZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_QDDJEDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_QDDJFZNJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_QDDJLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_ZHGKHQL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_ZHGKXSLC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CREATE_TIME", OracleDbType.Date),
					                        new OracleParameter("CREATE_BY", OracleDbType.Varchar2,255),
					                        new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                        new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                        new OracleParameter("QTXX", OracleDbType.Varchar2,255)
                                        };
                                    parameters[0].Value = dr["UNIQUE_CODE"];
                                    parameters[1].Value = dr["CAR_CODE"];
                                    parameters[2].Value = dr["OPTION_CODE"];
                                    parameters[3].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[4].Value = dr["QCSCQY"];
                                    parameters[5].Value = dr["JYBGBH"];
                                    parameters[6].Value = dr["JKQCZJXS"];
                                    parameters[7].Value = dr["CLXH"];
                                    parameters[8].Value = dr["HGSPBM"];
                                    parameters[9].Value = dr["CLZL"];
                                    parameters[10].Value = dr["YYC"];
                                    parameters[11].Value = dr["QDXS"];
                                    parameters[12].Value = dr["ZWPS"];
                                    parameters[13].Value = dr["ZCZBZL"];
                                    parameters[14].Value = dr["ZDSJZZL"];
                                    parameters[15].Value = dr["ZGCS"];
                                    parameters[16].Value = dr["EDZK"];
                                    parameters[17].Value = dr["LTGG"];
                                    parameters[18].Value = dr["LJ"];
                                    parameters[19].Value = dr["JYJGMC"];
                                    parameters[20].Value = dr["TYMC"];
                                    parameters[21].Value = dr["ZJ"];
                                    parameters[22].Value = dr["RLLX"];
                                    parameters[23].Value = dr["RLDC_CDDMSXZGXSCS"];
                                    parameters[24].Value = dr["RLDC_CQPBCGZYL"];
                                    parameters[25].Value = dr["RLDC_CQPRJ"];
                                    parameters[26].Value = dr["RLDC_DDGLMD"];
                                    parameters[27].Value = dr["RLDC_CQPLX"];
                                    parameters[28].Value = dr["RLDC_DDHHJSTJXXDCZBNL"];
                                    parameters[29].Value = dr["RLDC_DLXDCZZL"];
                                    parameters[30].Value = dr["RLDC_QDDJEDGL"];
                                    parameters[31].Value = dr["RLDC_QDDJFZNJ"];
                                    parameters[32].Value = dr["RLDC_QDDJLX"];
                                    parameters[33].Value = dr["RLDC_RLLX"];
                                    parameters[34].Value = dr["RLDC_ZHGKHQL"];
                                    parameters[35].Value = dr["RLDC_ZHGKXSLC"];
                                    parameters[36].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[37].Value = Utils.localUserId;
                                    parameters[38].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[39].Value = Utils.localUserId;
                                    parameters[40].Value = dr["QTXX"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + Environment.NewLine;
            }

            return succCount;
        }

        /// <summary>
        /// 修改已经导入的主表信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string UpdateMainData2(DataSet ds)
        {
            int totalCount = 0;
            int succCount = 0;
            string msg = string.Empty;

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
                msg += ex.Message + Environment.NewLine;
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
                DataTable checkData = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from RLLX_PARAM", null).Tables[0];
                DataRow[] tdr = checkData.Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", rlzl));

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
                                #region update
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update CTNY_MAIN set ");
                                    strSql.Append("CAR_CODE=:CAR_CODE,");
                                    strSql.Append("OPTION_CODE=:OPTION_CODE,");
                                    strSql.Append("ATTRIBUTE_CODE=:ATTRIBUTE_CODE,");
                                    strSql.Append("QCSCQY=:QCSCQY,");
                                    strSql.Append("JYBGBH=:JYBGBH,");
                                    strSql.Append("JKQCZJXS=:JKQCZJXS,");
                                    strSql.Append("CLXH=:CLXH,");
                                    strSql.Append("HGSPBM=:HGSPBM,");
                                    strSql.Append("CLZL=:CLZL,");
                                    strSql.Append("YYC=:YYC,");
                                    strSql.Append("QDXS=:QDXS,");
                                    strSql.Append("ZWPS=:ZWPS,");
                                    strSql.Append("ZCZBZL=:ZCZBZL,");
                                    strSql.Append("ZDSJZZL=:ZDSJZZL,");
                                    strSql.Append("ZGCS=:ZGCS,");
                                    strSql.Append("EDZK=:EDZK,");
                                    strSql.Append("LTGG=:LTGG,");
                                    strSql.Append("LJ=:LJ,");
                                    strSql.Append("JYJGMC=:JYJGMC,");
                                    strSql.Append("TYMC=:TYMC,");
                                    strSql.Append("ZJ=:ZJ,");
                                    strSql.Append("RLLX=:RLLX,");
                                    strSql.Append("CT_BSQXS=:CT_BSQXS,");
                                    strSql.Append("CT_EDGL=:CT_EDGL,");
                                    strSql.Append("CT_FDJXH=:CT_FDJXH,");
                                    strSql.Append("CT_JGL=:CT_JGL,");
                                    strSql.Append("CT_PL=:CT_PL,");
                                    strSql.Append("CT_QGS=:CT_QGS,");
                                    strSql.Append("CT_QTXX=:CT_QTXX,");
                                    strSql.Append("CT_SJGKRLXHL=:CT_SJGKRLXHL,");
                                    strSql.Append("CT_SQGKRLXHL=:CT_SQGKRLXHL,");
                                    strSql.Append("CT_ZHGKCO2PFL=:CT_ZHGKCO2PFL,");
                                    strSql.Append("CT_ZHGKRLXHL=:CT_ZHGKRLXHL,");
                                    strSql.Append("CT_BSQDWS=:CT_BSQDWS,");
                                    strSql.Append("UPDATE_TIME=:UPDATE_TIME,");
                                    strSql.Append("UPDATE_BY=:UPDATE_BY");
                                    strSql.Append(" where UNIQUE_CODE=:UNIQUE_CODE ");
                                    OracleParameter[] parameters = {
                                            new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
                                            new OracleParameter("QCSCQY", OracleDbType.Varchar2, 255),
                                            new OracleParameter("JYBGBH", OracleDbType.Varchar2, 255),
                                            new OracleParameter("JKQCZJXS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CLXH", OracleDbType.Varchar2, 255),
                                            new OracleParameter("HGSPBM", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CLZL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("YYC", OracleDbType.Varchar2, 255),
                                            new OracleParameter("QDXS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("ZWPS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("ZCZBZL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("ZDSJZZL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("ZGCS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("EDZK", OracleDbType.Varchar2, 255),
                                            new OracleParameter("LTGG", OracleDbType.Varchar2, 255),
                                            new OracleParameter("LJ", OracleDbType.Varchar2, 255),
                                            new OracleParameter("JYJGMC", OracleDbType.Varchar2, 255),
                                            new OracleParameter("TYMC", OracleDbType.Varchar2, 255),
                                            new OracleParameter("ZJ", OracleDbType.Varchar2, 255),
                                            new OracleParameter("RLLX", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_BSQXS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_EDGL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_FDJXH", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_JGL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_PL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_QGS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_QTXX", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_SJGKRLXHL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_SQGKRLXHL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_ZHGKCO2PFL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_ZHGKRLXHL", OracleDbType.Varchar2, 255),
                                            new OracleParameter("CT_BSQDWS", OracleDbType.Varchar2, 255),
                                            new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                        new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
                                            new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2, 255)
                                        };
                                    parameters[0].Value = dr["CAR_CODE"];
                                    parameters[1].Value = dr["OPTION_CODE"];
                                    parameters[2].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[3].Value = dr["QCSCQY"];
                                    parameters[4].Value = dr["JYBGBH"];
                                    parameters[5].Value = dr["JKQCZJXS"];
                                    parameters[6].Value = dr["CLXH"];
                                    parameters[7].Value = dr["HGSPBM"];
                                    parameters[8].Value = dr["CLZL"];
                                    parameters[9].Value = dr["YYC"];
                                    parameters[10].Value = dr["QDXS"];
                                    parameters[11].Value = dr["ZWPS"];
                                    parameters[12].Value = dr["ZCZBZL"];
                                    parameters[13].Value = dr["ZDSJZZL"];
                                    parameters[14].Value = dr["ZGCS"];
                                    parameters[15].Value = dr["EDZK"];
                                    parameters[16].Value = dr["LTGG"];
                                    parameters[17].Value = dr["LJ"];
                                    parameters[18].Value = dr["JYJGMC"];
                                    parameters[19].Value = dr["TYMC"];
                                    parameters[20].Value = dr["ZJ"];
                                    parameters[21].Value = dr["RLLX"];
                                    parameters[22].Value = dr["CT_BSQXS"];
                                    parameters[23].Value = dr["CT_EDGL"];
                                    parameters[24].Value = dr["CT_FDJXH"];
                                    parameters[25].Value = dr["CT_JGL"];
                                    parameters[26].Value = dr["CT_PL"];
                                    parameters[27].Value = dr["CT_QGS"];
                                    parameters[28].Value = dr["CT_QTXX"];
                                    parameters[29].Value = dr["CT_SJGKRLXHL"];
                                    parameters[30].Value = dr["CT_SQGKRLXHL"];
                                    parameters[31].Value = dr["CT_ZHGKCO2PFL"];
                                    parameters[32].Value = dr["CT_ZHGKRLXHL"];
                                    parameters[33].Value = dr["CT_BSQDWS"];
                                    parameters[34].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[35].Value = Utils.localUserId;
                                    parameters[36].Value = dr["UNIQUE_CODE"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(FCDSHHDL))
                            {
                                #region 非插电式混合动力
                                #region update
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update FCDS_MAIN set ");
                                    strSql.Append("CAR_CODE=:CAR_CODE,");
                                    strSql.Append("OPTION_CODE=:OPTION_CODE,");
                                    strSql.Append("ATTRIBUTE_CODE=:ATTRIBUTE_CODE,");
                                    strSql.Append("QCSCQY=:QCSCQY,");
                                    strSql.Append("JYBGBH=:JYBGBH,");
                                    strSql.Append("JKQCZJXS=:JKQCZJXS,");
                                    strSql.Append("CLXH=:CLXH,");
                                    strSql.Append("HGSPBM=:HGSPBM,");
                                    strSql.Append("CLZL=:CLZL,");
                                    strSql.Append("YYC=:YYC,");
                                    strSql.Append("QDXS=:QDXS,");
                                    strSql.Append("ZWPS=:ZWPS,");
                                    strSql.Append("ZCZBZL=:ZCZBZL,");
                                    strSql.Append("ZDSJZZL=:ZDSJZZL,");
                                    strSql.Append("ZGCS=:ZGCS,");
                                    strSql.Append("EDZK=:EDZK,");
                                    strSql.Append("LTGG=:LTGG,");
                                    strSql.Append("LJ=:LJ,");
                                    strSql.Append("JYJGMC=:JYJGMC,");
                                    strSql.Append("TYMC=:TYMC,");
                                    strSql.Append("ZJ=:ZJ,");
                                    strSql.Append("RLLX=:RLLX,");
                                    strSql.Append("FCDS_HHDL_BSQDWS=:FCDS_HHDL_BSQDWS,");
                                    strSql.Append("FCDS_HHDL_BSQXS=:FCDS_HHDL_BSQXS,");
                                    strSql.Append("FCDS_HHDL_CDDMSXZGCS=:FCDS_HHDL_CDDMSXZGCS,");
                                    strSql.Append("FCDS_HHDL_CDDMSXZHGKXSLC=:FCDS_HHDL_CDDMSXZHGKXSLC,");
                                    strSql.Append("FCDS_HHDL_DLXDCBNL=:FCDS_HHDL_DLXDCBNL,");
                                    strSql.Append("FCDS_HHDL_DLXDCZBCDY=:FCDS_HHDL_DLXDCZBCDY,");
                                    strSql.Append("FCDS_HHDL_DLXDCZZL=:FCDS_HHDL_DLXDCZZL,");
                                    strSql.Append("FCDS_HHDL_DLXDCZZNL=:FCDS_HHDL_DLXDCZZNL,");
                                    strSql.Append("FCDS_HHDL_EDGL=:FCDS_HHDL_EDGL,");
                                    strSql.Append("FCDS_HHDL_FDJXH=:FCDS_HHDL_FDJXH,");
                                    strSql.Append("FCDS_HHDL_HHDLJGXS=:FCDS_HHDL_HHDLJGXS,");
                                    strSql.Append("FCDS_HHDL_HHDLZDDGLB=:FCDS_HHDL_HHDLZDDGLB,");
                                    strSql.Append("FCDS_HHDL_JGL=:FCDS_HHDL_JGL,");
                                    strSql.Append("FCDS_HHDL_PL=:FCDS_HHDL_PL,");
                                    strSql.Append("FCDS_HHDL_QDDJEDGL=:FCDS_HHDL_QDDJEDGL,");
                                    strSql.Append("FCDS_HHDL_QDDJFZNJ=:FCDS_HHDL_QDDJFZNJ,");
                                    strSql.Append("FCDS_HHDL_QDDJLX=:FCDS_HHDL_QDDJLX,");
                                    strSql.Append("FCDS_HHDL_QGS=:FCDS_HHDL_QGS,");
                                    strSql.Append("FCDS_HHDL_SJGKRLXHL=:FCDS_HHDL_SJGKRLXHL,");
                                    strSql.Append("FCDS_HHDL_SQGKRLXHL=:FCDS_HHDL_SQGKRLXHL,");
                                    strSql.Append("FCDS_HHDL_XSMSSDXZGN=:FCDS_HHDL_XSMSSDXZGN,");
                                    strSql.Append("FCDS_HHDL_ZHGKRLXHL=:FCDS_HHDL_ZHGKRLXHL,");
                                    strSql.Append("FCDS_HHDL_ZHKGCO2PL=:FCDS_HHDL_ZHKGCO2PL,");
                                    strSql.Append("UPDATE_TIME=:UPDATE_TIME,");
                                    strSql.Append("UPDATE_BY=:UPDATE_BY");
                                    strSql.Append(" where UNIQUE_CODE=:UNIQUE_CODE ");
                                    OracleParameter[] parameters = {
                                            new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("QCSCQY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYBGBH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JKQCZJXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("HGSPBM", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("YYC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("QDXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZWPS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZCZBZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZDSJZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("EDZK", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LTGG", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYJGMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("TYMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_BSQDWS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_BSQXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_CDDMSXZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_CDDMSXZHGKXSLC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_DLXDCBNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_DLXDCZBCDY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_DLXDCZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_DLXDCZZNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_EDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_FDJXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_HHDLJGXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_HHDLZDDGLB", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_JGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_PL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_QDDJEDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_QDDJFZNJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_QDDJLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_QGS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_SJGKRLXHL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_SQGKRLXHL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_XSMSSDXZGN", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_ZHGKRLXHL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("FCDS_HHDL_ZHKGCO2PL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                        new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                        new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2, 255)
                                        };
                                    parameters[0].Value = dr["CAR_CODE"];
                                    parameters[1].Value = dr["OPTION_CODE"];
                                    parameters[2].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[3].Value = dr["QCSCQY"];
                                    parameters[4].Value = dr["JYBGBH"];
                                    parameters[5].Value = dr["JKQCZJXS"];
                                    parameters[6].Value = dr["CLXH"];
                                    parameters[7].Value = dr["HGSPBM"];
                                    parameters[8].Value = dr["CLZL"];
                                    parameters[9].Value = dr["YYC"];
                                    parameters[10].Value = dr["QDXS"];
                                    parameters[11].Value = dr["ZWPS"];
                                    parameters[12].Value = dr["ZCZBZL"];
                                    parameters[13].Value = dr["ZDSJZZL"];
                                    parameters[14].Value = dr["ZGCS"];
                                    parameters[15].Value = dr["EDZK"];
                                    parameters[16].Value = dr["LTGG"];
                                    parameters[17].Value = dr["LJ"];
                                    parameters[18].Value = dr["JYJGMC"];
                                    parameters[19].Value = dr["TYMC"];
                                    parameters[20].Value = dr["ZJ"];
                                    parameters[21].Value = dr["RLLX"];
                                    parameters[22].Value = dr["FCDS_HHDL_BSQDWS"];
                                    parameters[23].Value = dr["FCDS_HHDL_BSQXS"];
                                    parameters[24].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                                    parameters[25].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                                    parameters[26].Value = dr["FCDS_HHDL_DLXDCBNL"];
                                    parameters[27].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                                    parameters[28].Value = dr["FCDS_HHDL_DLXDCZZL"];
                                    parameters[29].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                                    parameters[30].Value = dr["FCDS_HHDL_EDGL"];
                                    parameters[31].Value = dr["FCDS_HHDL_FDJXH"];
                                    parameters[32].Value = dr["FCDS_HHDL_HHDLJGXS"];
                                    parameters[33].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                                    parameters[34].Value = dr["FCDS_HHDL_JGL"];
                                    parameters[35].Value = dr["FCDS_HHDL_PL"];
                                    parameters[36].Value = dr["FCDS_HHDL_QDDJEDGL"];
                                    parameters[37].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                                    parameters[38].Value = dr["FCDS_HHDL_QDDJLX"];
                                    parameters[39].Value = dr["FCDS_HHDL_QGS"];
                                    parameters[40].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                                    parameters[41].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                                    parameters[42].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                                    parameters[43].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                                    parameters[44].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                                    parameters[45].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[46].Value = Utils.localUserId;
                                    parameters[47].Value = dr["UNIQUE_CODE"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(CDSHHDL))
                            {
                                #region 插电式混合动力
                                #region update
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update CDS_MAIN set ");
                                    strSql.Append("CAR_CODE=:CAR_CODE,");
                                    strSql.Append("OPTION_CODE=:OPTION_CODE,");
                                    strSql.Append("ATTRIBUTE_CODE=:ATTRIBUTE_CODE,");
                                    strSql.Append("QCSCQY=:QCSCQY,");
                                    strSql.Append("JYBGBH=:JYBGBH,");
                                    strSql.Append("JKQCZJXS=:JKQCZJXS,");
                                    strSql.Append("CLXH=:CLXH,");
                                    strSql.Append("HGSPBM=:HGSPBM,");
                                    strSql.Append("CLZL=:CLZL,");
                                    strSql.Append("YYC=:YYC,");
                                    strSql.Append("QDXS=:QDXS,");
                                    strSql.Append("ZWPS=:ZWPS,");
                                    strSql.Append("ZCZBZL=:ZCZBZL,");
                                    strSql.Append("ZDSJZZL=:ZDSJZZL,");
                                    strSql.Append("ZGCS=:ZGCS,");
                                    strSql.Append("EDZK=:EDZK,");
                                    strSql.Append("LTGG=:LTGG,");
                                    strSql.Append("LJ=:LJ,");
                                    strSql.Append("JYJGMC=:JYJGMC,");
                                    strSql.Append("TYMC=:TYMC,");
                                    strSql.Append("ZJ=:ZJ,");
                                    strSql.Append("RLLX=:RLLX,");
                                    strSql.Append("CDS_HHDL_BSQDWS=:CDS_HHDL_BSQDWS,");
                                    strSql.Append("CDS_HHDL_BSQXS=:CDS_HHDL_BSQXS,");
                                    strSql.Append("CDS_HHDL_CDDMSXZGCS=:CDS_HHDL_CDDMSXZGCS,");
                                    strSql.Append("CDS_HHDL_CDDMSXZHGKXSLC=:CDS_HHDL_CDDMSXZHGKXSLC,");
                                    strSql.Append("CDS_HHDL_DLXDCBNL=:CDS_HHDL_DLXDCBNL,");
                                    strSql.Append("CDS_HHDL_DLXDCZBCDY=:CDS_HHDL_DLXDCZBCDY,");
                                    strSql.Append("CDS_HHDL_DLXDCZZL=:CDS_HHDL_DLXDCZZL,");
                                    strSql.Append("CDS_HHDL_DLXDCZZNL=:CDS_HHDL_DLXDCZZNL,");
                                    strSql.Append("CDS_HHDL_EDGL=:CDS_HHDL_EDGL,");
                                    strSql.Append("CDS_HHDL_FDJXH=:CDS_HHDL_FDJXH,");
                                    strSql.Append("CDS_HHDL_HHDLJGXS=:CDS_HHDL_HHDLJGXS,");
                                    strSql.Append("CDS_HHDL_HHDLZDDGLB=:CDS_HHDL_HHDLZDDGLB,");
                                    strSql.Append("CDS_HHDL_JGL=:CDS_HHDL_JGL,");
                                    strSql.Append("CDS_HHDL_PL=:CDS_HHDL_PL,");
                                    strSql.Append("CDS_HHDL_QDDJEDGL=:CDS_HHDL_QDDJEDGL,");
                                    strSql.Append("CDS_HHDL_QDDJFZNJ=:CDS_HHDL_QDDJFZNJ,");
                                    strSql.Append("CDS_HHDL_QDDJLX=:CDS_HHDL_QDDJLX,");
                                    strSql.Append("CDS_HHDL_QGS=:CDS_HHDL_QGS,");
                                    strSql.Append("CDS_HHDL_XSMSSDXZGN=:CDS_HHDL_XSMSSDXZGN,");
                                    strSql.Append("CDS_HHDL_ZHGKDNXHL=:CDS_HHDL_ZHGKDNXHL,");
                                    strSql.Append("CDS_HHDL_ZHGKRLXHL=:CDS_HHDL_ZHGKRLXHL,");
                                    strSql.Append("CDS_HHDL_ZHKGCO2PL=:CDS_HHDL_ZHKGCO2PL,");
                                    strSql.Append("UPDATE_TIME=:UPDATE_TIME,");
                                    strSql.Append("UPDATE_BY=:UPDATE_BY");
                                    strSql.Append(" where UNIQUE_CODE=:UNIQUE_CODE ");
                                    OracleParameter[] parameters = {
                                            new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("QCSCQY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYBGBH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JKQCZJXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("HGSPBM", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("YYC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("QDXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZWPS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZCZBZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZDSJZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("EDZK", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LTGG", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYJGMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("TYMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_BSQDWS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_BSQXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_CDDMSXZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_CDDMSXZHGKXSLC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_DLXDCBNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_DLXDCZBCDY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_DLXDCZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_DLXDCZZNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_EDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_FDJXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_HHDLJGXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_HHDLZDDGLB", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_JGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_PL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_QDDJEDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_QDDJFZNJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_QDDJLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_QGS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_XSMSSDXZGN", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_ZHGKDNXHL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_ZHGKRLXHL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDS_HHDL_ZHKGCO2PL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                        new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                        new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2, 255)
                                        };
                                    parameters[0].Value = dr["CAR_CODE"];
                                    parameters[1].Value = dr["OPTION_CODE"];
                                    parameters[2].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[3].Value = dr["QCSCQY"];
                                    parameters[4].Value = dr["JYBGBH"];
                                    parameters[5].Value = dr["JKQCZJXS"];
                                    parameters[6].Value = dr["CLXH"];
                                    parameters[7].Value = dr["HGSPBM"];
                                    parameters[8].Value = dr["CLZL"];
                                    parameters[9].Value = dr["YYC"];
                                    parameters[10].Value = dr["QDXS"];
                                    parameters[11].Value = dr["ZWPS"];
                                    parameters[12].Value = dr["ZCZBZL"];
                                    parameters[13].Value = dr["ZDSJZZL"];
                                    parameters[14].Value = dr["ZGCS"];
                                    parameters[15].Value = dr["EDZK"];
                                    parameters[16].Value = dr["LTGG"];
                                    parameters[17].Value = dr["LJ"];
                                    parameters[18].Value = dr["JYJGMC"];
                                    parameters[19].Value = dr["TYMC"];
                                    parameters[20].Value = dr["ZJ"];
                                    parameters[21].Value = dr["RLLX"];
                                    parameters[22].Value = dr["CDS_HHDL_BSQDWS"];
                                    parameters[23].Value = dr["CDS_HHDL_BSQXS"];
                                    parameters[24].Value = dr["CDS_HHDL_CDDMSXZGCS"];
                                    parameters[25].Value = dr["CDS_HHDL_CDDMSXZHGKXSLC"];
                                    parameters[26].Value = dr["CDS_HHDL_DLXDCBNL"];
                                    parameters[27].Value = dr["CDS_HHDL_DLXDCZBCDY"];
                                    parameters[28].Value = dr["CDS_HHDL_DLXDCZZL"];
                                    parameters[29].Value = dr["CDS_HHDL_DLXDCZZNL"];
                                    parameters[30].Value = dr["CDS_HHDL_EDGL"];
                                    parameters[31].Value = dr["CDS_HHDL_FDJXH"];
                                    parameters[32].Value = dr["CDS_HHDL_HHDLJGXS"];
                                    parameters[33].Value = dr["CDS_HHDL_HHDLZDDGLB"];
                                    parameters[34].Value = dr["CDS_HHDL_JGL"];
                                    parameters[35].Value = dr["CDS_HHDL_PL"];
                                    parameters[36].Value = dr["CDS_HHDL_QDDJEDGL"];
                                    parameters[37].Value = dr["CDS_HHDL_QDDJFZNJ"];
                                    parameters[38].Value = dr["CDS_HHDL_QDDJLX"];
                                    parameters[39].Value = dr["CDS_HHDL_QGS"];
                                    parameters[40].Value = dr["CDS_HHDL_XSMSSDXZGN"];
                                    parameters[41].Value = dr["CDS_HHDL_ZHGKDNXHL"];
                                    parameters[42].Value = dr["CDS_HHDL_ZHGKRLXHL"];
                                    parameters[43].Value = dr["CDS_HHDL_ZHKGCO2PL"];
                                    parameters[44].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[45].Value = Utils.localUserId;
                                    parameters[46].Value = dr["UNIQUE_CODE"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(CDD))
                            {
                                #region 纯电动
                                #region update
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update CDD_MAIN set ");
                                    strSql.Append("CAR_CODE=:CAR_CODE,");
                                    strSql.Append("OPTION_CODE=:OPTION_CODE,");
                                    strSql.Append("ATTRIBUTE_CODE=:ATTRIBUTE_CODE,");
                                    strSql.Append("QCSCQY=:QCSCQY,");
                                    strSql.Append("JYBGBH=:JYBGBH,");
                                    strSql.Append("JKQCZJXS=:JKQCZJXS,");
                                    strSql.Append("CLXH=:CLXH,");
                                    strSql.Append("HGSPBM=:HGSPBM,");
                                    strSql.Append("CLZL=:CLZL,");
                                    strSql.Append("YYC=:YYC,");
                                    strSql.Append("QDXS=:QDXS,");
                                    strSql.Append("ZWPS=:ZWPS,");
                                    strSql.Append("ZCZBZL=:ZCZBZL,");
                                    strSql.Append("ZDSJZZL=:ZDSJZZL,");
                                    strSql.Append("ZGCS=:ZGCS,");
                                    strSql.Append("EDZK=:EDZK,");
                                    strSql.Append("LTGG=:LTGG,");
                                    strSql.Append("LJ=:LJ,");
                                    strSql.Append("JYJGMC=:JYJGMC,");
                                    strSql.Append("TYMC=:TYMC,");
                                    strSql.Append("ZJ=:ZJ,");
                                    strSql.Append("RLLX=:RLLX,");
                                    strSql.Append("CDD_DDQC30FZZGCS=:CDD_DDQC30FZZGCS,");
                                    strSql.Append("CDD_DDXDCZZLYZCZBZLDBZ=:CDD_DDXDCZZLYZCZBZLDBZ,");
                                    strSql.Append("CDD_DLXDCBNL=:CDD_DLXDCBNL,");
                                    strSql.Append("CDD_DLXDCZBCDY=:CDD_DLXDCZBCDY,");
                                    strSql.Append("CDD_DLXDCZEDNL=:CDD_DLXDCZEDNL,");
                                    strSql.Append("CDD_DLXDCZZL=:CDD_DLXDCZZL,");
                                    strSql.Append("CDD_QDDJEDGL=:CDD_QDDJEDGL,");
                                    strSql.Append("CDD_QDDJFZNJ=:CDD_QDDJFZNJ,");
                                    strSql.Append("CDD_QDDJLX=:CDD_QDDJLX,");
                                    strSql.Append("CDD_ZHGKDNXHL=:CDD_ZHGKDNXHL,");
                                    strSql.Append("CDD_ZHGKXSLC=:CDD_ZHGKXSLC,");
                                    strSql.Append("UPDATE_TIME=:UPDATE_TIME,");
                                    strSql.Append("UPDATE_BY=:UPDATE_BY");
                                    strSql.Append(" where UNIQUE_CODE=:UNIQUE_CODE ");
                                    OracleParameter[] parameters = {
                                            new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("QCSCQY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYBGBH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JKQCZJXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("HGSPBM", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("YYC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("QDXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZWPS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZCZBZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZDSJZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("EDZK", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LTGG", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYJGMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("TYMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_DDQC30FZZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_DDXDCZZLYZCZBZLDBZ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_DLXDCBNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_DLXDCZBCDY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_DLXDCZEDNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_DLXDCZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_QDDJEDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_QDDJFZNJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_QDDJLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_ZHGKDNXHL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CDD_ZHGKXSLC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                        new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                        new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2, 255)
                                        };
                                    parameters[0].Value = dr["CAR_CODE"];
                                    parameters[1].Value = dr["OPTION_CODE"];
                                    parameters[2].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[3].Value = dr["QCSCQY"];
                                    parameters[4].Value = dr["JYBGBH"];
                                    parameters[5].Value = dr["JKQCZJXS"];
                                    parameters[6].Value = dr["CLXH"];
                                    parameters[7].Value = dr["HGSPBM"];
                                    parameters[8].Value = dr["CLZL"];
                                    parameters[9].Value = dr["YYC"];
                                    parameters[10].Value = dr["QDXS"];
                                    parameters[11].Value = dr["ZWPS"];
                                    parameters[12].Value = dr["ZCZBZL"];
                                    parameters[13].Value = dr["ZDSJZZL"];
                                    parameters[14].Value = dr["ZGCS"];
                                    parameters[15].Value = dr["EDZK"];
                                    parameters[16].Value = dr["LTGG"];
                                    parameters[17].Value = dr["LJ"];
                                    parameters[18].Value = dr["JYJGMC"];
                                    parameters[19].Value = dr["TYMC"];
                                    parameters[20].Value = dr["ZJ"];
                                    parameters[21].Value = dr["RLLX"];
                                    parameters[22].Value = dr["CDD_DDQC30FZZGCS"];
                                    parameters[23].Value = dr["CDD_DDXDCZZLYZCZBZLDBZ"];
                                    parameters[24].Value = dr["CDD_DLXDCBNL"];
                                    parameters[25].Value = dr["CDD_DLXDCZBCDY"];
                                    parameters[26].Value = dr["CDD_DLXDCZEDNL"];
                                    parameters[27].Value = dr["CDD_DLXDCZZL"];
                                    parameters[28].Value = dr["CDD_QDDJEDGL"];
                                    parameters[29].Value = dr["CDD_QDDJFZNJ"];
                                    parameters[30].Value = dr["CDD_QDDJLX"];
                                    parameters[31].Value = dr["CDD_ZHGKDNXHL"];
                                    parameters[32].Value = dr["CDD_ZHGKXSLC"];
                                    parameters[33].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[34].Value = Utils.localUserId;
                                    parameters[35].Value = dr["UNIQUE_CODE"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                            else if (rlzl.Equals(RLDC))
                            {
                                #region 燃料电池
                                #region update
                                try
                                {
                                    StringBuilder strSql = new StringBuilder();
                                    strSql.Append("update RLDC_MAIN set ");
                                    strSql.Append("CAR_CODE=:CAR_CODE,");
                                    strSql.Append("OPTION_CODE=:OPTION_CODE,");
                                    strSql.Append("ATTRIBUTE_CODE=:ATTRIBUTE_CODE,");
                                    strSql.Append("QCSCQY=:QCSCQY,");
                                    strSql.Append("JYBGBH=:JYBGBH,");
                                    strSql.Append("JKQCZJXS=:JKQCZJXS,");
                                    strSql.Append("CLXH=:CLXH,");
                                    strSql.Append("HGSPBM=:HGSPBM,");
                                    strSql.Append("CLZL=:CLZL,");
                                    strSql.Append("YYC=:YYC,");
                                    strSql.Append("QDXS=:QDXS,");
                                    strSql.Append("ZWPS=:ZWPS,");
                                    strSql.Append("ZCZBZL=:ZCZBZL,");
                                    strSql.Append("ZDSJZZL=:ZDSJZZL,");
                                    strSql.Append("ZGCS=:ZGCS,");
                                    strSql.Append("EDZK=:EDZK,");
                                    strSql.Append("LTGG=:LTGG,");
                                    strSql.Append("LJ=:LJ,");
                                    strSql.Append("JYJGMC=:JYJGMC,");
                                    strSql.Append("TYMC=:TYMC,");
                                    strSql.Append("ZJ=:ZJ,");
                                    strSql.Append("RLLX=:RLLX,");
                                    strSql.Append("RLDC_CDDMSXZGXSCS=:RLDC_CDDMSXZGXSCS,");
                                    strSql.Append("RLDC_CQPBCGZYL=:RLDC_CQPBCGZYL,");
                                    strSql.Append("RLDC_CQPRJ=:RLDC_CQPRJ,");
                                    strSql.Append("RLDC_DDGLMD=:RLDC_DDGLMD,");
                                    strSql.Append("RLDC_CQPLX=:RLDC_CQPLX,");
                                    strSql.Append("RLDC_DDHHJSTJXXDCZBNL=:RLDC_DDHHJSTJXXDCZBNL,");
                                    strSql.Append("RLDC_DLXDCZZL=:RLDC_DLXDCZZL,");
                                    strSql.Append("RLDC_QDDJEDGL=:RLDC_QDDJEDGL,");
                                    strSql.Append("RLDC_QDDJFZNJ=:RLDC_QDDJFZNJ,");
                                    strSql.Append("RLDC_QDDJLX=:RLDC_QDDJLX,");
                                    strSql.Append("RLDC_RLLX=:RLDC_RLLX,");
                                    strSql.Append("RLDC_ZHGKHQL=:RLDC_ZHGKHQL,");
                                    strSql.Append("RLDC_ZHGKXSLC=:RLDC_ZHGKXSLC,");
                                    strSql.Append("UPDATE_TIME=:UPDATE_TIME,");
                                    strSql.Append("UPDATE_BY=:UPDATE_BY");
                                    strSql.Append(" where UNIQUE_CODE=:UNIQUE_CODE ");
                                    OracleParameter[] parameters = {
                                            new OracleParameter("CAR_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("OPTION_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("ATTRIBUTE_CODE", OracleDbType.Varchar2,255),
					                        new OracleParameter("QCSCQY", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYBGBH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JKQCZJXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLXH", OracleDbType.Varchar2, 255),
					                        new OracleParameter("HGSPBM", OracleDbType.Varchar2, 255),
					                        new OracleParameter("CLZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("YYC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("QDXS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZWPS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZCZBZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZDSJZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZGCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("EDZK", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LTGG", OracleDbType.Varchar2, 255),
					                        new OracleParameter("LJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("JYJGMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("TYMC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("ZJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CDDMSXZGXSCS", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CQPBCGZYL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CQPRJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_DDGLMD", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_CQPLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_DDHHJSTJXXDCZBNL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_DLXDCZZL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_QDDJEDGL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_QDDJFZNJ", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_QDDJLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_RLLX", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_ZHGKHQL", OracleDbType.Varchar2, 255),
					                        new OracleParameter("RLDC_ZHGKXSLC", OracleDbType.Varchar2, 255),
					                        new OracleParameter("UPDATE_TIME", OracleDbType.Date),
					                        new OracleParameter("UPDATE_BY", OracleDbType.Varchar2,255),
					                        new OracleParameter("UNIQUE_CODE", OracleDbType.Varchar2, 255)
                                        };
                                    parameters[0].Value = dr["CAR_CODE"];
                                    parameters[1].Value = dr["OPTION_CODE"];
                                    parameters[2].Value = dr["ATTRIBUTE_CODE"];
                                    parameters[3].Value = dr["QCSCQY"];
                                    parameters[4].Value = dr["JYBGBH"];
                                    parameters[5].Value = dr["JKQCZJXS"];
                                    parameters[6].Value = dr["CLXH"];
                                    parameters[7].Value = dr["HGSPBM"];
                                    parameters[8].Value = dr["CLZL"];
                                    parameters[9].Value = dr["YYC"];
                                    parameters[10].Value = dr["QDXS"];
                                    parameters[11].Value = dr["ZWPS"];
                                    parameters[12].Value = dr["ZCZBZL"];
                                    parameters[13].Value = dr["ZDSJZZL"];
                                    parameters[14].Value = dr["ZGCS"];
                                    parameters[15].Value = dr["EDZK"];
                                    parameters[16].Value = dr["LTGG"];
                                    parameters[17].Value = dr["LJ"];
                                    parameters[18].Value = dr["JYJGMC"];
                                    parameters[19].Value = dr["TYMC"];
                                    parameters[20].Value = dr["ZJ"];
                                    parameters[21].Value = dr["RLLX"];
                                    parameters[22].Value = dr["RLDC_CDDMSXZGXSCS"];
                                    parameters[23].Value = dr["RLDC_CQPBCGZYL"];
                                    parameters[24].Value = dr["RLDC_CQPRJ"];
                                    parameters[25].Value = dr["RLDC_DDGLMD"];
                                    parameters[26].Value = dr["RLDC_CQPLX"];
                                    parameters[27].Value = dr["RLDC_DDHHJSTJXXDCZBNL"];
                                    parameters[28].Value = dr["RLDC_DLXDCZZL"];
                                    parameters[29].Value = dr["RLDC_QDDJEDGL"];
                                    parameters[30].Value = dr["RLDC_QDDJFZNJ"];
                                    parameters[31].Value = dr["RLDC_QDDJLX"];
                                    parameters[32].Value = dr["RLDC_RLLX"];
                                    parameters[33].Value = dr["RLDC_ZHGKHQL"];
                                    parameters[34].Value = dr["RLDC_ZHGKXSLC"];
                                    parameters[35].Value = Convert.ToDateTime(DateTime.Now, CultureInfo.InvariantCulture);
                                    parameters[36].Value = Utils.localUserId;
                                    parameters[37].Value = dr["UNIQUE_CODE"];
                                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, strSql.ToString(), parameters);
                                    succCount++;
                                }
                                catch (Exception ex)
                                {
                                    msg += ex.Message + Environment.NewLine;
                                }
                                #endregion
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message + Environment.NewLine;
            }

            return succCount;
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
        /// 中文转英文
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable C2E(Dictionary<string, string> dict, DataTable dt, string tableName)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                foreach (var kv in dict)
                {
                    if (kv.Key == dc.ColumnName)
                    {
                        dc.ColumnName = kv.Value;
                        break;
                    }
                }
            }
            return dt;
        }


        /// <summary>
        /// 转换表头
        /// </summary>
        /// <param name="dict">表头转换中英文对照模板</param>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable D2D(Dictionary<string, string> dict, DataTable dt, string tableName)
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

        private void ReadTemplate(string filePath)
        {
            DataSet ds = ImportExcel.ReadExcelToDataSet(filePath);
            dictCTNY = new Dictionary<string, string>();
            dictFCDSHHDL = new Dictionary<string, string>();
            dictCDSHHDL = new Dictionary<string, string>();
            dictCDD = new Dictionary<string, string>();
            dictRLDC = new Dictionary<string, string>();

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
            try
            {
                string vin = drVin["VIN"].ToString().Trim().ToUpper();

                // 如果当前vin数据已经存在，则跳过
                if (OracleHelper.Exists(OracleHelper.conn, String.Format(@"SELECT VIN FROM FC_CLJBXX WHERE VIN='{0}'", vin)))
                {
                    genMsg += vin + "已经存在。\r\n";
                    return genMsg;
                }
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    OracleTransaction tra = con.BeginTransaction();
                    try
                    {
                        #region 待生成的燃料基本信息数据存入燃料基本信息表
                        DateTime clzzrqDate;
                        try
                        {
                            clzzrqDate = DateTime.ParseExact(drVin["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"]);
                        }

                        OracleParameter clzzrq = new OracleParameter("CLZZRQ", clzzrqDate);
                        clzzrq.DbType = DbType.Date;

                        DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);
                        OracleParameter uploadDeadline = new OracleParameter("UPLOADDEADLINE", uploadDeadlineDate);
                        uploadDeadline.DbType = DbType.Date;

                        OracleParameter creTime = new OracleParameter("CREATETIME", DateTime.Now);
                        creTime.DbType = DbType.Date;
                        OracleParameter upTime = new OracleParameter("UPDATETIME", DateTime.Now);
                        upTime.DbType = DbType.Date;

                        OracleParameter[] param = { 
                                    new OracleParameter("VIN",vin),
                                    new OracleParameter("USER_ID",Utils.localUserId),
                                    new OracleParameter("QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                    new OracleParameter("JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("CLXH",drMain["CLXH"].ToString().Trim()),
                                    new OracleParameter("CLZL",drMain["CLZL"].ToString().Trim()),
                                    new OracleParameter("RLLX",drMain["RLLX"].ToString().Trim()),
                                    new OracleParameter("ZCZBZL",drMain["ZCZBZL"].ToString().Trim()),
                                    new OracleParameter("ZGCS",drMain["ZGCS"].ToString().Trim()),
                                    new OracleParameter("LTGG",drMain["LTGG"].ToString().Trim()),
                                    new OracleParameter("ZJ",drMain["ZJ"].ToString().Trim()),
                                    new OracleParameter("TYMC",drMain["TYMC"].ToString().Trim()),
                                    new OracleParameter("YYC",drMain["YYC"].ToString().Trim()),
                                    new OracleParameter("ZWPS",drMain["ZWPS"].ToString().Trim()),
                                    new OracleParameter("ZDSJZZL",drMain["ZDSJZZL"].ToString().Trim()),
                                    new OracleParameter("EDZK",drMain["EDZK"].ToString().Trim()),
                                    new OracleParameter("LJ",drMain["LJ"].ToString().Trim()),
                                    new OracleParameter("QDXS",drMain["QDXS"].ToString().Trim()),
                                    new OracleParameter("JYJGMC",drMain["JYJGMC"].ToString().Trim()),
                                    new OracleParameter("JYBGBH",drMain["JYBGBH"].ToString().Trim()),
                                    new OracleParameter("HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                    //new OracleParameter("QTXX",drMain["CT_QTXX"].ToString().Trim()),
                                    new OracleParameter("QTXX",drMain.Table.Columns.Contains("CT_QTXX") ? drMain["CT_QTXX"].ToString().Trim() : ""),
                                    // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                    new OracleParameter("STATUS","1"),
                                    creTime,
                                    upTime,
                                    new OracleParameter("UNIQUE_CODE",drVin["UNIQUE_CODE"].ToString().Trim())
                                    };
                        OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX
                            (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME,UNIQUE_CODE
                            ) VALUES
                            (   :VIN,:USER_ID,:QCSCQY,:JKQCZJXS,:CLZZRQ,:UPLOADDEADLINE,:CLXH,:CLZL,
                                :RLLX,:ZCZBZL,:ZGCS,:LTGG,:ZJ,
                                :TYMC,:YYC,:ZWPS,:ZDSJZZL,:EDZK,:LJ,
                                :QDXS,:JYJGMC,:JYBGBH,:HGSPBM,:QTXX,:STATUS,:CREATETIME,:UPDATETIME,:UNIQUE_CODE)", param);
                        #endregion

                        #region 插入参数信息

                        string sqlDelParam = String.Format("DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='{0}'", vin);
                        OracleHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                        // 待生成的燃料参数信息存入燃料参数表
                        foreach (DataRow drParam in dtPam.Rows)
                        {
                            string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                            string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY 
                                        (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                    VALUES
                                        (:PARAM_CODE,:VIN,:PARAM_VALUE,:V_ID)";
                            OracleParameter[] paramList = { 
                                    new OracleParameter("PARAM_CODE",paramCode),
                                    new OracleParameter("VIN",vin),
                                    new OracleParameter("PARAM_VALUE",drMain[paramCode]),
                                    new OracleParameter("V_ID","")
                                };
                            OracleHelper.ExecuteNonQuery(tra, sqlInsertParam, paramList);
                        }
                        #endregion

                        #region 保存VIN信息备用

                        string sqlDel = String.Format("DELETE FROM VIN_INFO WHERE VIN = '{0}'", vin);
                        OracleHelper.ExecuteNonQuery(tra, sqlDel, null);
                        OracleParameter[] vinParamList = { 
                                        new OracleParameter("VIN",vin),
                                        new OracleParameter("CLXH",drVin["CLXH"].ToString().Trim()),
                                        new OracleParameter("CLZZRQ",clzzrqDate),
                                        new OracleParameter("STATUS","0"),
                                        creTime,
                                        new OracleParameter("RLLX",drVin["RLLX"].ToString().Trim()),
                                        new OracleParameter("UNIQUE_CODE",drVin["UNIQUE_CODE"].ToString().Trim())
                                    };
                        OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO VIN_INFO(VIN,CLXH,CLZZRQ,STATUS,CREATETIME,RLLX,UNIQUE_CODE) Values (:VIN, :CLXH,:CLZZRQ,:STATUS,:CREATETIME,:RLLX,:UNIQUE_CODE)", vinParamList);
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

        // 获取以导入但未生成油耗数据的VIN
        public DataTable GetImportedVinData(string vin)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT VI.* FROM VIN_INFO VI WHERE VI.STATUS='1' ");
            if (!string.IsNullOrEmpty(vin))
            {
                sql.AppendFormat(" AND VIN LIKE '%{0}%'", vin);
            }
            try
            {
                return OracleHelper.ExecuteDataSet(OracleHelper.conn, sql.ToString(), null).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取全部主表数据，用作合并VIN数据
        /// </summary>
        /// <returns></returns>
        public bool GetMainData()
        {
            dsMainStatic.Add(CTNY, OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT * FROM CTNY_MAIN", null).Tables[0]);
            dsMainStatic.Add(FCDSHHDL, OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT * FROM FCDS_MAIN", null).Tables[0]);
            dsMainStatic.Add(CDSHHDL, OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT * FROM CDS_MAIN", null).Tables[0]);
            dsMainStatic.Add(CDD, OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT * FROM CDD_MAIN", null).Tables[0]);
            dsMainStatic.Add(RLDC, OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT * FROM RLDC_MAIN", null).Tables[0]);

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT unique_code from ctny_main union all ");
            sql.Append("SELECT unique_code from fcds_main union all ");
            sql.Append("SELECT unique_code from cds_main  union all ");
            sql.Append("SELECT unique_code from cdd_main  union all ");
            sql.Append("SELECT unique_code from rldc_main");
            return OracleHelper.Exists(OracleHelper.conn, sql.ToString());
        }

        /// <summary>
        /// 获取已经导入的参数编码（MAIN_ID）,用于导入判断
        /// </summary>
        public int GetMainId(string mainId)
        {
            int dataCount = 0;
            try
            {
                DataSet dsCtnyMainId = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT MAIN_ID FROM MAIN_CTNY WHERE MAIN_ID='{0}'", mainId), null);
                DataSet dsFcdsMainId = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT MAIN_ID FROM MAIN_FCDSHHDL WHERE MAIN_ID='{0}'", mainId), null);
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
            try
            {
                DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT MAIN_ID FROM VIN_INFO WHERE VIN='{0}'", vin), null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    CocId = ds.Tables[0].Rows[0]["MAIN_ID"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return CocId;
        }

        /// <summary>
        /// 获取选中的基本信息的主键
        /// </summary>
        /// <param name="gv">要筛选的GridView</param>
        /// <param name="dt">要筛选的DataTable</param>
        /// <returns></returns>
        public List<string> GetMainIdFromControl(GridView gv, DataTable dt)
        {
            List<string> mainIdList = new List<string>();
            gv.PostEditor();
            if (dt != null && dt.Rows.Count > 0)
            {
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
        private static DataTable GetRllxData(string fuelType)
        {
            return OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT PARAM_CODE FROM RLLX_PARAM WHERE FUEL_TYPE='{0}' AND STATUS='1'", fuelType), null).Tables[0];
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

            // 汽车生产企业
            if (string.IsNullOrEmpty(Qcscqy))
            {
                message += "汽车生产企业不能为空!\r\n";
            }

            // 车辆型号
            string clxh = Convert.ToString(r["CLXH"]);
            string uniqueCode = Convert.ToString(r["UNIQUE_CODE"]);
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
                message += "整车整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格\r\n";
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
                message += "轮距（前/后）应填写整数，前后轮距，中间用”/”隔开\r\n";
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
                message = String.Format("{0}【{1}】：\r\n{2}", uniqueCode, clxh, message);
            }

            return message;
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
                    OleDbDataAdapter oada = new OleDbDataAdapter(String.Format("select * from [{0}]", sheet), strConn);
                    oada.Fill(ds, sheet.IndexOf('$') > 0 ? sheet.Substring(0, sheet.Length - 1) : sheet);
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
        /// 获取节假日数据
        /// </summary>
        /// <returns></returns>
        protected static List<string> GetHoliday()
        {
            List<string> holidayList = new List<string>();
            try
            {
                DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT HOL_DAYS FROM FC_HOLIDAY", null);
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


            //message += this.VerifyRequired("车辆型号", Convert.ToString(drVIN["CLXH"]));
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
                if (rllx == "汽油" || rllx == "柴油" || rllx == "两用燃料" || rllx == "双燃料" || rllx == "非插电式混合动力" || rllx == "插电式混合动力" || rllx == "纯电动" || rllx == "燃料电池")
                {
                    return string.Empty;
                }
                else
                {
                    return "燃料类型参数填写汽油、柴油、两用燃料、双燃料、纯电动、非插电式混合动力、插电式混合动力、燃料电池\r\n";
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
                    return "最大设计总质量应≥整车整备质量＋乘员质量（额定载客×乘客质量，乘用车按65㎏/人核算)!\r\n";
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

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
                    return "车辆种类参数应填写“乘用车（M1）/轻型客车（M2）/轻型货车（N1）”\r\n";
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
                    return "动力蓄电池组种类参数应填写“铅酸电池/金属氢化物镍电池/锂电池/超级电容/其它”\r\n";
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
                            message += VerifyInt("排量", Convert.ToString(r[code]));
                            break;
                        case "CT_EDGL":
                            message += VerifyFloat("额定功率", Convert.ToString(r[code]));
                            break;
                        case "CT_JGL":
                            message += VerifyFloat("最大净功率", Convert.ToString(r[code]));
                            break;
                        case "CT_SJGKRLXHL":
                            message += VerifyFloat("市郊工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "CT_SQGKRLXHL":
                            message += VerifyFloat("市区工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKCO2PFL":
                            message += VerifyInt("综合工况CO2排放量", Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKRLXHL":
                            message += VerifyFloat("综合工况燃料消耗量", Convert.ToString(r[code]));
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
                            message += VerifyInt("动力蓄电池组比能量", Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZEDNL":
                            message += VerifyFloat("动力蓄电池组总能量", Convert.ToString(r[code]));
                            break;
                        case "CDD_DDXDCZZLYZCZBZLDBZ":
                            message += VerifyInt("动力蓄电池总质量与整车整备质量的比值", Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZBCDY":
                            message += VerifyInt("动力蓄电池组标称电压", Convert.ToString(r[code]));
                            break;
                        case "CDD_DDQC30FZZGCS":
                            message += VerifyInt("电动汽车30分钟最高车速", Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKXSLC":
                            message += VerifyInt("综合工况续驶里程", Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值扭矩", Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKDNXHL":
                            message += VerifyInt("综合工况电能消耗量", Convert.ToString(r[code]));
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
                            message += VerifyInt("动力蓄电池组比能量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat("动力蓄电池组总能量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat("综合工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_EDGL":
                            message += VerifyFloat("额定功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_JGL":
                            message += VerifyFloat("最大净功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_PL":
                            message += VerifyInt("排量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt("综合工况CO2排放", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt("动力蓄电池组标称电压", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SJGKRLXHL":
                            message += VerifyFloat("市郊工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SQGKRLXHL":
                            message += VerifyFloat("市区工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt("纯电动模式下1km最高车速", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyInt("纯电动模式下综合工况续驶里程", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值扭矩", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2("混合动力最大电功率比", Convert.ToString(r[code]));
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
                            message += VerifyInt("动力蓄电池组比能量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat("动力蓄电池组总能量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat("综合工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKDNXHL":
                            message += VerifyInt("综合工况电能消耗量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyInt("纯电动模式下综合工况续驶里程", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt("纯电动模式下1km最高车速", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值扭矩", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_EDGL":
                            message += VerifyFloat("额定功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_JGL":
                            message += VerifyFloat("最大净功率", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_PL":
                            message += VerifyInt("排量", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt("综合工况CO2排放", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt("动力蓄电池组标称电压", Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2("混合动力最大电功率比", Convert.ToString(r[code]));
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
                            message += VerifyInt("电电混合技术条件下动力蓄电池组比能量", Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKHQL":
                            message += VerifyFloat("综合工况燃料消耗量", Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKXSLC":
                            message += VerifyInt("综合工况续驶里程", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CDDMSXZGXSCS":
                            message += VerifyInt("电动汽车30分钟最高车速", Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJEDGL":
                            message += VerifyFloat("驱动电机额定功率", Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJFZNJ":
                            message += VerifyInt("驱动电机峰值扭矩", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPBCGZYL":
                            message += VerifyInt("储氢瓶标称工作压力", Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPRJ":
                            message += VerifyInt("储氢瓶容积", Convert.ToString(r[code]));
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

        protected string FormatParam(string obj, string strFormat)
        {
            string msg = string.Empty;
            try
            {
                if (obj != null && !string.IsNullOrEmpty(obj))
                {
                    if (Regex.IsMatch(obj, "\\d+(.\\d+)?$") && strFormat == "1")
                    {
                        obj = (double.Parse(obj)).ToString("0.0");
                    }
                    if (Regex.IsMatch(obj, "\\d+(.\\d+)?$") && strFormat == "2")
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
                    msg = String.Format("{0}长度过长，最长为{1}位!\r\n", strName, expectedLen);
                }
            }
            return msg;
        }

        // 验证整型
        protected string VerifyInt(string strName, string value)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, "^[0-9]*$"))
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
    }
}
