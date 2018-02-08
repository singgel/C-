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

namespace FuelDataSysClient.Tool.Tool_Porsche
{
    //public enum Status
    //{
    //    已上报 = 0,
    //    待上报 = 1,
    //    修改待上报 = 2,
    //    撤销待上报 = 3,
    //    未被激活 = 9
    //    //（9：未被激活（数据通过excel导入但未被激活）；0：已上传；1：没上传；2：修改没上传；3：撤销未上传）
    //}

    public class PorscheUtils
    {
        private const string CTNY = "传统能源";
        private const string FCDSHHDL = "非插电式混合动力";
        private const string CDSHHDL = "插电式混合动力";
        private const string CDD = "纯电动";
        private const string RLDC = "燃料电池";
        private const string VIN = "VIN";
        public const string LTGG = "LTGG";
        public const string LJ = "LJ";
        public const string ZCZBZL = "ZCZBZL";
        public const string CTNYExport = "传统能源导出";
        public const string CDDExport = "纯电动导出";
        public const string CDSExport = "插电式导出";
        public const string FCDSExport = "非插电式导出";
        public const string RLDCExport = "燃料电池导出";
        public const string CLMXExport = "车辆明细导出";
        public const string HZBGExport = "三阶段汇总报告导出";
        public const string HZBGExport_New = "四阶段汇总报告导出";
        private List<string> PARAMFLOAT1 = new List<string>() 
        {
            "CT_EDGL", "CT_JGL", "CT_SJGKRLXHL", "CT_SQGKRLXHL", "CT_ZHGKRLXHL", "FCDS_HHDL_DLXDCZZNL", 
           "FCDS_HHDL_ZHGKRLXHL", "FCDS_HHDL_EDGL", "FCDS_HHDL_JGL", "FCDS_HHDL_SJGKRLXHL", "FCDS_HHDL_SQGKRLXHL", 
           "FCDS_HHDL_QDDJEDGL", "CDS_HHDL_DLXDCZZNL", "FCDS_HHDL_DLXDCBNL", "CDS_HHDL_ZHGKRLXHL", 
           "CDS_HHDL_QDDJEDGL", "CDS_HHDL_EDGL", "CDS_HHDL_JGL", "CDD_DLXDCZEDNL", "CDD_QDDJEDGL", "RLDC_DDGLMD", 
           "RLDC_ZHGKHQL", "RLDC_QDDJEDGL" 
        };
        private List<string> PARAMFLOAT2 = new List<string>() { "CDS_HHDL_HHDLZDDGLB", "FCDS_HHDL_HHDLZDDGLB" };

        private string strCon = AccessHelper.conn;
        DataTable checkData = new DataTable();
        Dictionary<string, string> dictCTNY;  //存放列头转换模板(传统能源)
        Dictionary<string, string> dictFCDSHHDL;  //存放列头转换模板（非插电式混合动力）
        Dictionary<string, string> dictCDSHHDL;  //存放列头转换模板（非插电式混合动力）
        Dictionary<string, string> dictCDD;  //存放列头转换模板（非插电式混合动力）
        Dictionary<string, string> dictRLDC;  //存放列头转换模板（非插电式混合动力）
        Dictionary<string, string> dictLTGG; // 存放列头转换模板（轮胎信息表）
        Dictionary<string, string> dictLJ; // 存放列头转换模板（轮距表）
        Dictionary<string, string> dictZCZBZL; // 存放列头转换模板（整车整备质量表）
        Dictionary<string, string> dictVin; //存放列头转换模板（VIN）
        Dictionary<string, string> dictCTNYExport; //传统能源导出模板
        Dictionary<string, string> dictCDSExport;
        Dictionary<string, string> dictFCDSExport;
        Dictionary<string, string> dictCDDExport;
        Dictionary<string, string> dictRLDCExport;
        Dictionary<string, string> dictCLMXExport; //车辆明细导出模板
        Dictionary<string, string> dictHZBGExport; //车辆明细导出模板三阶段
        Dictionary<string, string> dictHZBGExport_New; //车辆明细导出模板四阶段
        DataTable dtCtnyStatic;
        DataTable dtFcdsStatic;
        DataTable dtCdsStatic;
        DataTable dtCddStatic;
        DataTable dtRldcStatic;

        DataTable dtLtggStatic;
        DataTable dtLjStatic;
        DataTable dtZczbzlStatic;
        DataTable dtClmxStatic;

        public DataTable dtTargetFuel;

        private List<string> listHoliday; // 节假日数据

        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate_Porsche"];
        private static Dictionary<string, string> FILE_NAME = new Dictionary<string, string>() 
        {
            {"VIN" ,"VIN*.csv"},
            {"MAIN" ,"车型参数*.xlsx"},
            {"LTGG" ,"轮胎规格*.xlsx"},
            {"LJ" ,"轮距*.xlsx"},
            {"ZCZBZL" ,"整车整备质量*.xlsx"},
            {"F_VIN" ,"已导入的VIN"},
            {"F_MAIN" ,"已导入的车型参数"},
            {"COMPANY_NAME" ,"保时捷（中国）汽车销售有限公司"},
            {"FILE_PATH" ,"C:\\Users\\Administrator\\Desktop\\data\\porsche"}
        };

        public PorscheUtils(bool iniFlag)
        {
            if (iniFlag)
            {
                checkData = GetCheckData();    //获取参数数据  RLLX_PARAM  
                ReadTemplate(path);   //读取表头转置模板
                ReadTargetFuel();  //获取目标值
            }
        }

        public PorscheUtils(bool iniFlag,string startTime)
        {
            if (iniFlag)
            {
                checkData = GetCheckData();    //获取参数数据  RLLX_PARAM  
                ReadTemplate(path);   //读取表头转置模板
                if (Convert.ToDateTime(startTime).Year < 2016)
                {
                    ReadTargetFuel();
                }
                else
                {
                    ReadTargetFuel_New();
                }
            }
        }

        // VIN excel文件名称的开头
        private string _JKQCJXS = FILE_NAME["COMPANY_NAME"].ToString();

        public string JKQCJXS
        {
            get { return _JKQCJXS; }
        }

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

        private string filePath = FILE_NAME["FILE_PATH"].ToString();

        public string FilePath
        {
            get { return filePath; }
        }

        /// <summary>
        /// 返回fileType的文件名格式
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public string GetMainFileName(string fileType)
        {
            return FILE_NAME[fileType].ToString();
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
        /// 删除车型参数
        /// </summary>
        /// <param name="deleteType">修改类型：“CTNY”表示传统能源；“FCDS”表示非插电式</param>
        /// <param name="ids">要删除的数据标识</param>
        /// <returns></returns>
        public string DeleteMain(string deleteType, string ids)
        {
            string msg = string.Empty;
            string tableName = string.Empty;
            string field = string.Empty;
            OleDbConnection con = new OleDbConnection(AccessHelper.conn);
            OleDbTransaction tra = null;
            try
            {
                con.Open();
                tra = con.BeginTransaction();

                if (deleteType == "CTNY")
                {
                    tableName = "CTNY_MAIN";
                }
                else if (deleteType == "FCDS")
                {
                    tableName = "FCDS_MAIN";
                }
                else if (deleteType == "CDS")
                {
                    tableName = "CDS_MAIN";
                }
                else if (deleteType == "CDD")
                {
                    tableName = "CDD_MAIN";
                }
                else if (deleteType == "RLDC")
                {
                    tableName = "RLDC_MAIN";
                }

                string sqlDel = string.Format(@"DELETE * FROM {0} WHERE MAIN_ID IN ({1})", tableName, ids);

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
        /// 删除其他车型参数
        /// </summary>
        /// <param name="deleteType">修改类型：“LTGG”表示轮胎规格；“LJ”表示轮距</param>
        /// <param name="paramList">要删除的数据标识</param>
        /// <returns></returns>
        public string DeleteOtherMain(string deleteType, List<string> paramList)
        {
            string msg = string.Empty;
            string tableName = string.Empty;
            string fieldName = string.Empty;
            OleDbConnection con = new OleDbConnection(AccessHelper.conn);
            OleDbTransaction tra = null;
            try
            {
                con.Open();
                tra = con.BeginTransaction();

                if (deleteType == "LTGG")
                {
                    tableName = "MAIN_LTGG";
                    fieldName = "LTGG_ID";
                }
                else if (deleteType == "LJ")
                {
                    tableName = "MAIN_LJ";
                    fieldName = "LJ_ID";
                }
                else if (deleteType == "ZCZBZL")
                {
                    tableName = "MAIN_ZCZBZL";
                    fieldName = "ZCZBZL_ID";
                }

                string[] paramId = null;
                string sqlDel = string.Empty;
                foreach (string paramIds in paramList)
                {
                    paramId = paramIds.Split(',');
                    sqlDel = string.Format(@"DELETE * FROM {0} WHERE MAIN_ID='{2}' AND {1}='{3}'", tableName, fieldName, paramId[0], paramId[1]);
                    AccessHelper.ExecuteNonQuery(tra, sqlDel, null);
                }

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
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="mainId"></param>
        /// <param name="paramId"></param>
        /// <param name="paramName"></param>
        /// <param name="IsExist"></param>
        /// <returns></returns>
        public DataRow GetDivideMain(DataTable dt, string vin, string mainId, string paramValue, string paramField, string paramName, ref string message)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[paramField].ToString().Trim() == paramValue && dr["MAIN_ID"].ToString().Trim() == mainId)
                {
                    message += "";
                    return dr;
                }
            }
            switch (paramName)
            {
                case LTGG:
                    message += string.Format("\r\n{2}: 轮胎规格代码{0}-{1}不存在", mainId, paramValue, vin);
                    break;
                case LJ:
                    message += string.Format("\r\n{2}: 轮距代码{0}-{1}不存在", mainId, paramValue, vin);
                    break;
                case ZCZBZL:
                    message += string.Format("\r\n{2}: 整车整备质量代码{0}-{1}不存在", mainId, paramValue, vin);
                    break;
                case CTNY:
                    message += string.Format("\r\n{1}: 车型参数{0}不存在", mainId, vin);
                    break;
                case FCDSHHDL:
                    message += string.Format("\r\n{1}: 车型参数{0}不存在", mainId, vin);
                    break;
                case CDSHHDL:
                    message += string.Format("\r\n{1}: 车型参数{0}不存在", mainId, vin);
                    break;
                case CDD:
                    message += string.Format("\r\n{1}: 车型参数{0}不存在", mainId, vin);
                    break;
                case RLDC:
                    message += string.Format("\r\n{1}: 车型参数{0}不存在", mainId, vin);
                    break;
                default: break;
            }
            return null;
        }

        /// <summary>
        /// 保存VIN信息
        /// </summary>
        /// <param name="ds"></param>
        public string SaveVinInfo(DataSet ds)
        {
            DataTable dtVin = ds.Tables[0];

            int succFuelCount = 0; //生成油耗数据的数量
            int succImCount = 0;   //成功导入的数量
            int failCount = 0;  //导入失败的数量
            int totalCount = dtVin.Rows.Count;

            string msg = "";
            ProcessForm pf = new ProcessForm();
            try
            {
                DataTable dtCtnyPam = this.GetRllxData("传统能源");
                DataTable dtFcdsPam = this.GetRllxData("非插电式混合动力");
                DataTable dtCdsPam = this.GetRllxData("插电式混合动力");
                DataTable dtCddPam = this.GetRllxData("纯电动");
                DataTable dtRldcPam = this.GetRllxData("燃料电池");

                // 获取节假日数据
                listHoliday = this.GetHoliday();

                // 显示进度条
                pf.Show();
                int pageSize = 20;
                int totalVin = ds.Tables[0].Rows.Count;
                int count = 0;

                pf.TotalMax = (int)Math.Ceiling((decimal)totalVin / pageSize);
                pf.ShowProcessBar();

                foreach (DataRow drVin in ds.Tables[0].Rows)
                {
                    count++;
                    string vinMsg = string.Empty;
                    string vin = drVin["VIN"] == null ? "" : drVin["VIN"].ToString().Trim();
                    string mainId = drVin["MAIN_ID"] == null ? "" : drVin["MAIN_ID"].ToString().Trim();
                    string ltggId = drVin["LTGG_ID"] == null ? "" : drVin["LTGG_ID"].ToString().Trim();
                    string ljId = drVin["LJ_ID"] == null ? "" : drVin["LJ_ID"].ToString().Trim();
                    string zczbzlId = drVin["ZCZBZL_ID"] == null ? "" : drVin["ZCZBZL_ID"].ToString().Trim();

                    if (!string.IsNullOrEmpty(mainId))
                    {
                        string ctnyMsg = string.Empty;
                        string fcdsMsg = string.Empty;
                        string cdsMsg = string.Empty;
                        string cddMsg = string.Empty;
                        string rldcMsg = string.Empty;

                        DataRow drCtny = this.GetDivideMain(dtCtnyStatic, vin, mainId, mainId, "MAIN_ID", CTNY, ref ctnyMsg);
                        DataRow drFcds = this.GetDivideMain(dtFcdsStatic, vin, mainId, mainId, "MAIN_ID", FCDSHHDL, ref fcdsMsg);
                        DataRow drCds = this.GetDivideMain(dtCdsStatic, vin, mainId, mainId, "MAIN_ID", CDSHHDL, ref cdsMsg);
                        DataRow drCdd = this.GetDivideMain(dtCddStatic, vin, mainId, mainId, "MAIN_ID", CDD, ref cddMsg);
                        DataRow drRldc = this.GetDivideMain(dtRldcStatic, vin, mainId, mainId, "MAIN_ID", RLDC, ref rldcMsg);

                        DataRow drLtgg = this.GetDivideMain(dtLtggStatic, vin, mainId, ltggId, "LTGG_ID", LTGG, ref vinMsg);
                        DataRow drLj = this.GetDivideMain(dtLjStatic, vin, mainId, ljId, "LJ_ID", LJ, ref vinMsg);
                        DataRow drZb = this.GetDivideMain(dtZczbzlStatic, vin, mainId, zczbzlId, "ZCZBZL_ID", ZCZBZL, ref vinMsg);

                        if (!string.IsNullOrEmpty(ctnyMsg) && !string.IsNullOrEmpty(fcdsMsg) && !string.IsNullOrEmpty(cdsMsg) && !string.IsNullOrEmpty(cddMsg) && !string.IsNullOrEmpty(rldcMsg))
                        {
                            vinMsg += string.Format("{0} 缺少车型参数数据:“{1}”\r\n", vin, mainId);
                        }
                        if (string.IsNullOrEmpty(vinMsg))
                        {
                            if (string.IsNullOrEmpty(ctnyMsg))
                            {
                                vinMsg += this.SaveReadyData(drVin, drCtny, dtCtnyPam, drLtgg, drLj, drZb);
                                if (string.IsNullOrEmpty(vinMsg))
                                {
                                    succFuelCount++; //统计导入并生成油耗数据的VIN数量
                                }
                            }
                            else if (string.IsNullOrEmpty(fcdsMsg))
                            {
                                vinMsg += this.SaveReadyData(drVin, drFcds, dtFcdsPam, drLtgg, drLj, drZb);
                                if (string.IsNullOrEmpty(vinMsg))
                                {
                                    succFuelCount++; //统计导入并生成油耗数据的VIN数量
                                }
                            }
                            else if (string.IsNullOrEmpty(cdsMsg))
                            {
                                vinMsg += this.SaveReadyData(drVin, drCds, dtCdsPam, drLtgg, drLj, drZb);
                                if (string.IsNullOrEmpty(vinMsg))
                                {
                                    succFuelCount++; //统计导入并生成油耗数据的VIN数量
                                }
                            }
                            else if (string.IsNullOrEmpty(cddMsg))
                            {
                                vinMsg += this.SaveReadyData(drVin, drCdd, dtCddPam, drLtgg, drLj, drZb);
                                if (string.IsNullOrEmpty(vinMsg))
                                {
                                    succFuelCount++; //统计导入并生成油耗数据的VIN数量
                                }
                            }
                            else if (string.IsNullOrEmpty(rldcMsg))
                            {
                                vinMsg += this.SaveReadyData(drVin, drRldc, dtRldcPam, drLtgg, drLj, drZb);
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

            using (OleDbConnection con = new OleDbConnection(strCon))
            {
                try
                {
                    con.Open();

                    tra = con.BeginTransaction();
                    #region 保存VIN信息备用

                    string sqlDel = "DELETE FROM VIN_INFO WHERE VIN = '" + vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                    string sqlStr = @"INSERT INTO VIN_INFO(VIN,MAIN_ID,CLZZRQ,ZCZBZL_ID,LTGG_ID,LJ_ID,STATUS,COCNO,CCCHOLDER,HGNO) Values (@VIN, @MAIN_ID,@CLZZRQ,@ZCZBZL_ID,@LTGG_ID,@LJ_ID,@STATUS,@COCNO,@CCCHOLDER,@HGNO)";
                    OleDbParameter[] vinParamList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@MAIN_ID",drVin["MAIN_ID"].ToString().Trim()),
                                         new OleDbParameter("@CLZZRQ",Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim())),
                                         new OleDbParameter("@ZCZBZL_ID",drVin["ZCZBZL_ID"].ToString().Trim()),
                                         new OleDbParameter("@LTGG_ID",drVin["LTGG_ID"].ToString().Trim()),
                                         new OleDbParameter("@LJ_ID",drVin["LJ_ID"].ToString().Trim()),
                                         new OleDbParameter("@STATUS","1"),
                                         new OleDbParameter("@COCNO",drVin["COCNO"].ToString().Trim()),
                                         new OleDbParameter("@CCCHOLDER",drVin["CCCHOLDER"].ToString().Trim()),
                                         new OleDbParameter("@HGNO",drVin["HGNO"].ToString().Trim())
                                      };
                    AccessHelper.ExecuteNonQuery(tra, sqlStr, vinParamList);

                    tra.Commit();
                    #endregion
                }
                catch (Exception ex)
                {
                    tra.Rollback();
                    genMsg += ex.Message + "\r\n";
                }
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
            DataSet ds = this.ReadVinCsv(true, ',', fileName);
            if (ds != null)
            {
                // 验证VIN数据
                string vinMsg = this.IsVinDataReady(ds);
                if (string.IsNullOrEmpty(vinMsg))
                {
                    rtnMsg += this.SaveVinInfo(ds);
                }
                else
                {
                    rtnMsg += vinMsg + "\r\n FAILED-IMPORT";
                }

                // 如果保存VIN时没有异常信息，说明VIN都已经保存成功，将该VIN文件移动到已完成目录
                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_VIN");
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
        }

        /// <summary>
        /// 导入车型参数信息信息
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

                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    // 读取成功后，将车型参数模板文件移动到已完成的文件夹
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
        /// 导入车型参数信息信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public string ImportOtherMainData(string fileName, string folderName, string paramName, string importType, List<string> mainUpdateList)
        {
            string rtnMsg = string.Empty;

            DataSet ds = this.ReadOtherMainExcel(fileName, paramName);
            if (ds != null)
            {
                DataTable dt = D2D(ds.Tables[0]);  //转换表头（用户模板中的表头转为数据库列名）
                if (importType == "IMPORT")
                {
                    switch (paramName)
                    {
                        case "LTGG":
                            rtnMsg += this.ImportLtgg(dt);
                            break;
                        case "LJ":
                            rtnMsg += this.ImportLj(dt);
                            break;
                        case "ZCZBZL":
                            rtnMsg += this.ImportZczbzl(dt);
                            break;
                        default: break;
                    }
                }
                else if (importType == "UPDATE")
                {
                    rtnMsg += this.UpdateMainData(ds, mainUpdateList);
                }

                if (rtnMsg.ToUpper().IndexOf("FAILED-IMPORT") < 0)
                {
                    // 读取成功后，将车型参数模板文件移动到已完成的文件夹
                    rtnMsg += this.MoveFinishedFile(fileName, folderName, "F_MAIN");

                }
                else
                {
                    rtnMsg = Path.GetFileName(fileName) + "\r\n" + rtnMsg + "\r\n";
                }
            }
            else
            {
                rtnMsg = fileName + "中没有数据或数据格式错误\r\n";
            }

            return rtnMsg;
        }

        /// <summary>
        /// 导入轮胎规格
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected string ImportLtgg(DataTable dt)
        {
            string rtnMsg = string.Empty;
            string ltggMsg = this.IsLtggReady(dt);
            if (string.IsNullOrEmpty(ltggMsg))
            {
                rtnMsg += this.SaveLtggData(dt);
            }
            else
            {
                rtnMsg += ltggMsg + "\r\n FAILED-IMPORT";
            }

            return rtnMsg;

        }

        /// <summary>
        /// 导入轮距
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected string ImportLj(DataTable dt)
        {
            string rtnMsg = string.Empty;
            string ljMsg = this.IsLjReady(dt);
            if (string.IsNullOrEmpty(ljMsg))
            {
                rtnMsg += this.SaveLjData(dt);
            }
            else
            {
                rtnMsg += ljMsg + "\r\n FAILED-IMPORT";
            }

            return rtnMsg;

        }

        /// <summary>
        /// 导入轮胎规格
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        protected string ImportZczbzl(DataTable dt)
        {
            string rtnMsg = string.Empty;
            string zbMsg = this.IsZczbzlReady(dt);
            if (string.IsNullOrEmpty(zbMsg))
            {
                rtnMsg += this.SaveZczbzlData(dt);
            }
            else
            {
                rtnMsg += zbMsg + "\r\n FAILED-IMPORT";
            }

            return rtnMsg;

        }

        /// <summary>
        /// 读车型参数信息
        /// </summary>
        /// <param name="fileName"></param>
        public DataSet ReadMainExcel(string fileName)
        {
            string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [传统能源$]", strConn);
                oada.Fill(ds, CTNY);

                oada = new OleDbDataAdapter("SELECT * FROM [非插电式混合动力$]", strConn);
                oada.Fill(ds, FCDSHHDL);

                oada = new OleDbDataAdapter("SELECT * FROM [插电式混合动力$]", strConn);
                oada.Fill(ds, CDSHHDL);

                oada = new OleDbDataAdapter("SELECT * FROM [纯电动$]", strConn);
                oada.Fill(ds, CDD);

                oada = new OleDbDataAdapter("SELECT * FROM [燃料电池$]", strConn);
                oada.Fill(ds, RLDC);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 读其他车型参数模板数据信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public DataSet ReadOtherMainExcel(string fileName, string tableName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [sheet1$]", strConn);
                oada.Fill(ds, tableName);
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
            string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [传统能源$]", strConn);
                oada.Fill(ds, CTNY);

                oada = new OleDbDataAdapter("SELECT * FROM [非插电式混合动力$]", strConn);
                oada.Fill(ds, FCDSHHDL);

                oada = new OleDbDataAdapter("SELECT * FROM [插电式混合动力$]", strConn);
                oada.Fill(ds, CDSHHDL);

                oada = new OleDbDataAdapter("SELECT * FROM [纯电动$]", strConn);
                oada.Fill(ds, CDD);

                oada = new OleDbDataAdapter("SELECT * FROM [燃料电池$]", strConn);
                oada.Fill(ds, RLDC);

                oada = new OleDbDataAdapter("SELECT * FROM [LTGG$]", strConn);
                oada.Fill(ds, LTGG);

                oada = new OleDbDataAdapter("SELECT * FROM [LJ$]", strConn);
                oada.Fill(ds, LJ);

                oada = new OleDbDataAdapter("SELECT * FROM [ZCZBZL$]", strConn);
                oada.Fill(ds, ZCZBZL);

                oada = new OleDbDataAdapter("SELECT * FROM [VIN$]", strConn);
                oada.Fill(ds, VIN);

                oada = new OleDbDataAdapter("SELECT * FROM [传统能源导出$]", strConn);
                oada.Fill(ds, CTNYExport);

                oada = new OleDbDataAdapter("SELECT * FROM [纯电动导出$]", strConn);
                oada.Fill(ds, CDDExport);

                oada = new OleDbDataAdapter("SELECT * FROM [插电式混合动力导出$]", strConn);
                oada.Fill(ds, CDSExport);

                oada = new OleDbDataAdapter("SELECT * FROM [非插电式混合动力导出$]", strConn);
                oada.Fill(ds, FCDSExport);

                oada = new OleDbDataAdapter("SELECT * FROM [燃料电池导出$]", strConn);
                oada.Fill(ds, RLDCExport);

                oada = new OleDbDataAdapter("SELECT * FROM [车辆明细导出$]", strConn);
                oada.Fill(ds, CLMXExport);

                oada = new OleDbDataAdapter("SELECT * FROM [汇总报告导出三阶段$]", strConn);
                oada.Fill(ds, HZBGExport);

                oada = new OleDbDataAdapter("SELECT * FROM [汇总报告导出四阶段$]", strConn);
                oada.Fill(ds, HZBGExport_New);
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 导入车型参数信息
        /// </summary>
        /// <param name="basicInfo"></param>
        /// <param name="ctnyInfo"></param>
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
                DataTable dtCtny = D2D(ds.Tables[CTNY]);
                DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + CTNY + "' and STATUS=1");

                DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);
                DataRow[] drFcdsHhdl = checkData.Select("FUEL_TYPE='" + FCDSHHDL + "' and STATUS=1");

                DataTable dtCdsHhdl = D2D(ds.Tables[CDSHHDL]);
                DataRow[] drCdsHhdl = checkData.Select("FUEL_TYPE='" + CDSHHDL + "' and STATUS=1");

                DataTable dtCdd = D2D(ds.Tables[CDD]);
                DataRow[] drCdd = checkData.Select("FUEL_TYPE='" + CDD + "' and STATUS=1");

                DataTable dtRldc = D2D(ds.Tables[RLDC]);
                DataRow[] drRldc = checkData.Select("FUEL_TYPE='" + RLDC + "' and STATUS=1");

                if (dtCtny != null && dtCtny.Rows.Count > 0)
                {
                    totalCount += dtCtny.Rows.Count;
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
                            strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZGCS,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,QDXS,STATUS,JYJGMC,JYBGBH,CT_BSQDWS,CT_BSQXS,CT_EDGL,CT_FDJXH,CT_JGL,CT_PL,CT_QGS,CT_QTXX,CT_SJGKRLXHL,CT_SQGKRLXHL,CT_ZHGKCO2PFL,CT_ZHGKRLXHL,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZGCS,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@CT_BSQDWS,@CT_BSQXS,@CT_EDGL,@CT_FDJXH,@CT_JGL,@CT_PL,@CT_QGS,@CT_QTXX,@CT_SJGKRLXHL,@CT_SQGKRLXHL,@CT_ZHGKCO2PFL,@CT_ZHGKRLXHL,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM)");
                            OleDbParameter[] parameters = {
					        new OleDbParameter("@MAIN_ID", OleDbType.VarChar,50),
					        new OleDbParameter("@CREATE_BY", OleDbType.VarChar,255),
					        new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
					        new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
					        new OleDbParameter("@CLXH", OleDbType.VarChar,100),
					        new OleDbParameter("@CLZL", OleDbType.VarChar,200),
					        new OleDbParameter("@RLLX", OleDbType.VarChar,200),
					        new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
					        new OleDbParameter("@ZJ", OleDbType.VarChar,255),
					        new OleDbParameter("@TYMC", OleDbType.VarChar,200),
					        new OleDbParameter("@YYC", OleDbType.VarChar,200),
					        new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
					        new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
					        new OleDbParameter("@EDZK", OleDbType.VarChar,255),
					        new OleDbParameter("@QDXS", OleDbType.VarChar,200),
					        new OleDbParameter("@STATUS", OleDbType.VarChar,1),
					        new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
					        new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
					        new OleDbParameter("@CT_BSQDWS", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_BSQXS", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_EDGL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_FDJXH", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_JGL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_PL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_QGS", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_QTXX", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_SJGKRLXHL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_SQGKRLXHL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_ZHGKCO2PFL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_ZHGKRLXHL", OleDbType.VarChar,200),
                            new OleDbParameter("@CREATETIME", OleDbType.Date),
                            new OleDbParameter("@UPDATE_BY", OleDbType.VarChar,200),
                            new OleDbParameter("@UPDATETIME", OleDbType.Date),
					        new OleDbParameter("@HGSPBM", OleDbType.VarChar,50)
                        };

                            parameters[0].Value = dr["MAIN_ID"];
                            parameters[1].Value = Utils.localUserId;
                            parameters[2].Value = JKQCJXS;
                            parameters[3].Value = dr["QCSCQY"];
                            parameters[4].Value = dr["CLXH"];
                            parameters[5].Value = dr["CLZL"];
                            parameters[6].Value = dr["RLLX"];
                            parameters[7].Value = dr["ZGCS"];
                            parameters[8].Value = dr["ZJ"];
                            parameters[9].Value = dr["TYMC"];
                            parameters[10].Value = dr["YYC"];
                            parameters[11].Value = dr["ZWPS"];
                            parameters[12].Value = dr["ZDSJZZL"];
                            parameters[13].Value = dr["EDZK"];
                            parameters[14].Value = dr["QDXS"];
                            parameters[15].Value = (int)Status.待上报;
                            parameters[16].Value = dr["JYJGMC"];
                            parameters[17].Value = dr["JYBGBH"];
                            parameters[18].Value = dr["CT_BSQDWS"];
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
                            parameters[30].Value = DateTime.Now;
                            parameters[31].Value = Utils.localUserId;
                            parameters[32].Value = DateTime.Now;
                            parameters[33].Value = dr["HGSPBM"];

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;
                            #endregion
                        }
                    }
                }
                if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
                {
                    totalCount += dtFcdsHhdl.Rows.Count;
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
                            strSql.Append("INSERT INTO FCDS_MAIN(");
                            strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS,FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC,FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY,FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL,FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH,FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB,FCDS_HHDL_JGL,FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL,FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS,FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL,FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHGKRLXHL,FCDS_HHDL_ZHKGCO2PL,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM,CT_QTXX)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@FCDS_HHDL_BSQDWS,@FCDS_HHDL_BSQXS,@FCDS_HHDL_CDDMSXZGCS,@FCDS_HHDL_CDDMSXZHGKXSLC,@FCDS_HHDL_DLXDCBNL,@FCDS_HHDL_DLXDCZBCDY,@FCDS_HHDL_DLXDCZZL,@FCDS_HHDL_DLXDCZZNL,@FCDS_HHDL_EDGL,@FCDS_HHDL_FDJXH,@FCDS_HHDL_HHDLJGXS,@FCDS_HHDL_HHDLZDDGLB,@FCDS_HHDL_JGL,@FCDS_HHDL_PL,@FCDS_HHDL_QDDJEDGL,@FCDS_HHDL_QDDJFZNJ,@FCDS_HHDL_QDDJLX,@FCDS_HHDL_QGS,@FCDS_HHDL_SJGKRLXHL,@FCDS_HHDL_SQGKRLXHL,@FCDS_HHDL_XSMSSDXZGN,@FCDS_HHDL_ZHGKRLXHL,@FCDS_HHDL_ZHKGCO2PL,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM,@CT_QTXX)");
                            OleDbParameter[] parameters = {
					    new OleDbParameter("@MAIN_ID", OleDbType.VarChar,50),
					    new OleDbParameter("@CREATE_BY", OleDbType.VarChar,255),
					    new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
					    new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
					    new OleDbParameter("@CLXH", OleDbType.VarChar,100),
					    new OleDbParameter("@CLZL", OleDbType.VarChar,200),
					    new OleDbParameter("@RLLX", OleDbType.VarChar,200),
					    new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
					    new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
					    new OleDbParameter("@LTGG", OleDbType.VarChar,200),
					    new OleDbParameter("@ZJ", OleDbType.VarChar,255),
					    new OleDbParameter("@TYMC", OleDbType.VarChar,200),
					    new OleDbParameter("@YYC", OleDbType.VarChar,200),
					    new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
					    new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
					    new OleDbParameter("@EDZK", OleDbType.VarChar,255),
					    new OleDbParameter("@LJ", OleDbType.VarChar,255),
					    new OleDbParameter("@QDXS", OleDbType.VarChar,200),
					    new OleDbParameter("@STATUS", OleDbType.VarChar,1),
					    new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
					    new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
					    new OleDbParameter("@FCDS_HHDL_BSQDWS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_BSQXS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCBNL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCZZL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_EDGL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_FDJXH", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_HHDLJGXS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_JGL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_PL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QDDJEDGL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QDDJLX", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QGS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", OleDbType.VarChar,200),
					    new OleDbParameter("@CREATETIME", OleDbType.Date),
                        new OleDbParameter("@UPDATE_BY", OleDbType.VarChar,200),
                        new OleDbParameter("@UPDATETIME", OleDbType.Date),
					    new OleDbParameter("@HGSPBM", OleDbType.VarChar,50),
					    new OleDbParameter("@CT_QTXX", OleDbType.VarChar,255)};

                            parameters[0].Value = dr["MAIN_ID"];
                            parameters[1].Value = Utils.localUserId;
                            parameters[2].Value = JKQCJXS;
                            parameters[3].Value = dr["QCSCQY"];
                            parameters[4].Value = dr["CLXH"];
                            parameters[5].Value = dr["CLZL"];
                            parameters[6].Value = dr["RLLX"];
                            parameters[7].Value = "";   //dr["ZCZBZL"];
                            parameters[8].Value = dr["ZGCS"];
                            parameters[9].Value = "";   //dr["LTGG"];
                            parameters[10].Value = dr["ZJ"];
                            parameters[11].Value = dr["TYMC"];
                            parameters[12].Value = dr["YYC"];
                            parameters[13].Value = dr["ZWPS"];
                            parameters[14].Value = dr["ZDSJZZL"];
                            parameters[15].Value = dr["EDZK"];
                            parameters[16].Value = "";   //dr["LJ"];
                            parameters[17].Value = dr["QDXS"];
                            parameters[18].Value = (int)Status.待上报;
                            parameters[19].Value = dr["JYJGMC"];
                            parameters[20].Value = dr["JYBGBH"];
                            parameters[21].Value = dr["FCDS_HHDL_BSQDWS"];
                            parameters[22].Value = dr["FCDS_HHDL_BSQXS"];
                            parameters[23].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                            parameters[24].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                            parameters[25].Value = dr["FCDS_HHDL_DLXDCBNL"];
                            parameters[26].Value = dr["FCDS_HHDL_DLXDCZBCDY"];
                            parameters[27].Value = dr["FCDS_HHDL_DLXDCZZL"];
                            parameters[28].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                            parameters[29].Value = dr["FCDS_HHDL_EDGL"];
                            parameters[30].Value = dr["FCDS_HHDL_FDJXH"];
                            parameters[31].Value = dr["FCDS_HHDL_HHDLJGXS"];
                            parameters[32].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                            parameters[33].Value = dr["FCDS_HHDL_JGL"];
                            parameters[34].Value = dr["FCDS_HHDL_PL"];
                            parameters[35].Value = dr["FCDS_HHDL_QDDJEDGL"];
                            parameters[36].Value = dr["FCDS_HHDL_QDDJFZNJ"];
                            parameters[37].Value = dr["FCDS_HHDL_QDDJLX"];
                            parameters[38].Value = dr["FCDS_HHDL_QGS"];
                            parameters[39].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                            parameters[40].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                            parameters[41].Value = dr["FCDS_HHDL_XSMSSDXZGN"];
                            parameters[42].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                            parameters[43].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                            parameters[44].Value = DateTime.Today;
                            parameters[45].Value = Utils.localUserId;
                            parameters[46].Value = DateTime.Today;
                            parameters[47].Value = dr["HGSPBM"];
                            parameters[48].Value = dr["CT_QTXX"];

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;
                            #endregion
                        }
                    }
                }
                if (dtCdsHhdl != null && dtCdsHhdl.Rows.Count > 0)
                {
                    totalCount += dtCdsHhdl.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtCdsHhdl.Rows)
                    {
                        error = VerifyData(dr, drCdsHhdl, "IMPORT");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region insert
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO CDS_MAIN(");
                            strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,CDS_HHDL_HHDLJGXS,CDS_HHDL_XSMSSDXZGN,CDS_HHDL_DLXDCZZL,CDS_HHDL_DLXDCZZNL,CDS_HHDL_DLXDCBNL,CDS_HHDL_CDDMSXZHGKXSLC,CDS_HHDL_CDDMSXZGCS,CDS_HHDL_DLXDCZBCDY,CDS_HHDL_QDDJLX,CDS_HHDL_HHDLZDDGLB,CDS_HHDL_QDDJFZNJ,CDS_HHDL_QDDJEDGL,CDS_HHDL_ZHGKDNXHL,CDS_HHDL_ZHGKRLXHL,CDS_HHDL_ZHKGCO2PL,CDS_HHDL_FDJXH,CDS_HHDL_QGS,CDS_HHDL_PL,CDS_HHDL_EDGL,CDS_HHDL_JGL,CDS_HHDL_BSQXS,CDS_HHDL_BSQDWS,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM,CT_QTXX)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@CDS_HHDL_HHDLJGXS,@CDS_HHDL_XSMSSDXZGN,@CDS_HHDL_DLXDCZZL,@CDS_HHDL_DLXDCZZNL,@CDS_HHDL_DLXDCBNL,@CDS_HHDL_CDDMSXZHGKXSLC,@CDS_HHDL_CDDMSXZGCS,@CDS_HHDL_DLXDCZBCDY,@CDS_HHDL_QDDJLX,@CDS_HHDL_HHDLZDDGLB,@CDS_HHDL_QDDJFZNJ,@CDS_HHDL_QDDJEDGL,@CDS_HHDL_ZHGKDNXHL,@CDS_HHDL_ZHGKRLXHL,@CDS_HHDL_ZHKGCO2PL,@CDS_HHDL_FDJXH,@CDS_HHDL_QGS,@CDS_HHDL_PL,@CDS_HHDL_EDGL,@CDS_HHDL_JGL,@CDS_HHDL_BSQXS,@CDS_HHDL_BSQDWS,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM,@CT_QTXX)");
                            OleDbParameter[] parameters = {
					        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"]),
					        new OleDbParameter("@CREATE_BY", Utils.localUserId),
					        new OleDbParameter("@JKQCZJXS", JKQCJXS),
					        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
					        new OleDbParameter("@CLXH", dr["CLXH"]),
					        new OleDbParameter("@CLZL", dr["CLZL"]),
					        new OleDbParameter("@RLLX", dr["RLLX"]),
					        new OleDbParameter("@ZCZBZL", ""),
					        new OleDbParameter("@ZGCS", dr["ZGCS"]),
					        new OleDbParameter("@LTGG", ""),
					        new OleDbParameter("@ZJ", dr["ZJ"]),
					        new OleDbParameter("@TYMC", dr["TYMC"]),
					        new OleDbParameter("@YYC", dr["YYC"]),
					        new OleDbParameter("@ZWPS", dr["ZWPS"]),
					        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
					        new OleDbParameter("@EDZK", dr["EDZK"]),
					        new OleDbParameter("@LJ", ""),
					        new OleDbParameter("@QDXS", dr["QDXS"]),
					        new OleDbParameter("@STATUS", (int)Status.待上报),
					        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
					        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
					        new OleDbParameter("@CDS_HHDL_HHDLJGXS", dr["CDS_HHDL_HHDLJGXS"]),
					        new OleDbParameter("@CDS_HHDL_XSMSSDXZGN", dr["CDS_HHDL_XSMSSDXZGN"]),
					        new OleDbParameter("@CDS_HHDL_DLXDCZZL", dr["CDS_HHDL_DLXDCZZL"]),
					        new OleDbParameter("@CDS_HHDL_DLXDCZZNL", dr["CDS_HHDL_DLXDCZZNL"]),
					        new OleDbParameter("@CDS_HHDL_DLXDCBNL", dr["CDS_HHDL_DLXDCBNL"]),
					        new OleDbParameter("@CDS_HHDL_CDDMSXZHGKXSLC", dr["CDS_HHDL_CDDMSXZHGKXSLC"]),
					        new OleDbParameter("@CDS_HHDL_CDDMSXZGCS", dr["CDS_HHDL_CDDMSXZGCS"]),
					        new OleDbParameter("@CDS_HHDL_DLXDCZBCDY", dr["CDS_HHDL_DLXDCZBCDY"]),
					        new OleDbParameter("@CDS_HHDL_QDDJLX", dr["CDS_HHDL_QDDJLX"]),
					        new OleDbParameter("@CDS_HHDL_HHDLZDDGLB", dr["CDS_HHDL_HHDLZDDGLB"]),
					        new OleDbParameter("@CDS_HHDL_QDDJFZNJ", dr["CDS_HHDL_QDDJFZNJ"]),
					        new OleDbParameter("@CDS_HHDL_QDDJEDGL", dr["CDS_HHDL_QDDJEDGL"]),
					        new OleDbParameter("@CDS_HHDL_ZHGKDNXHL", dr["CDS_HHDL_ZHGKDNXHL"]),
					        new OleDbParameter("@CDS_HHDL_ZHGKRLXHL", dr["CDS_HHDL_ZHGKRLXHL"]),
					        new OleDbParameter("@CDS_HHDL_ZHKGCO2PL", dr["CDS_HHDL_ZHKGCO2PL"]),
					        new OleDbParameter("@CDS_HHDL_FDJXH", dr["CDS_HHDL_FDJXH"]),
					        new OleDbParameter("@CDS_HHDL_QGS", dr["CDS_HHDL_QGS"]),
					        new OleDbParameter("@CDS_HHDL_PL", dr["CDS_HHDL_PL"]),
					        new OleDbParameter("@CDS_HHDL_EDGL", dr["CDS_HHDL_EDGL"]),
					        new OleDbParameter("@CDS_HHDL_JGL", dr["CDS_HHDL_JGL"]),
					        new OleDbParameter("@CDS_HHDL_BSQXS", dr["CDS_HHDL_BSQXS"]),
					        new OleDbParameter("@CDS_HHDL_BSQDWS", dr["CDS_HHDL_BSQDWS"]),
					        new OleDbParameter("@CREATETIME", DateTime.Today),
                            new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                            new OleDbParameter("@UPDATETIME", DateTime.Today),
					        new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
					        new OleDbParameter("@CT_QTXX", dr["CT_QTXX"])};

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;
                            #endregion
                        }
                    }
                }
                if (dtCdd != null && dtCdd.Rows.Count > 0)
                {
                    totalCount += dtCdd.Rows.Count;
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
                            strSql.Append("INSERT INTO CDD_MAIN(");
                            strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,CDD_DLXDCZZL,CDD_DLXDCBNL,CDD_DLXDCZEDNL,CDD_DDQC30FZZGCS,CDD_DDXDCZZLYZCZBZLDBZ,CDD_DLXDCZBCDY,CDD_ZHGKXSLC,CDD_QDDJLX,CDD_QDDJEDGL,CDD_QDDJFZNJ,CDD_ZHGKDNXHL,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM,CT_QTXX)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@CDD_DLXDCZZL,@CDD_DLXDCBNL,@CDD_DLXDCZEDNL,@CDD_DDQC30FZZGCS,@CDD_DDXDCZZLYZCZBZLDBZ,@CDD_DLXDCZBCDY,@CDD_ZHGKXSLC,@CDD_QDDJLX,@CDD_QDDJEDGL,@CDD_QDDJFZNJ,@CDD_ZHGKDNXHL,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM,@CT_QTXX)");
                            OleDbParameter[] parameters = {
					        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"]),
					        new OleDbParameter("@CREATE_BY", Utils.localUserId),
					        new OleDbParameter("@JKQCZJXS", JKQCJXS),
					        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
					        new OleDbParameter("@CLXH", dr["CLXH"]),
					        new OleDbParameter("@CLZL", dr["CLZL"]),
					        new OleDbParameter("@RLLX", dr["RLLX"]),
					        new OleDbParameter("@ZCZBZL", ""),
					        new OleDbParameter("@ZGCS", dr["ZGCS"]),
					        new OleDbParameter("@LTGG", ""),
					        new OleDbParameter("@ZJ", dr["ZJ"]),
					        new OleDbParameter("@TYMC", dr["TYMC"]),
					        new OleDbParameter("@YYC", dr["YYC"]),
					        new OleDbParameter("@ZWPS", dr["ZWPS"]),
					        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
					        new OleDbParameter("@EDZK", dr["EDZK"]),
					        new OleDbParameter("@LJ", ""),
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
					        new OleDbParameter("@CREATETIME", DateTime.Today),
                            new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                            new OleDbParameter("@UPDATETIME", DateTime.Today),
					        new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
					        new OleDbParameter("@CT_QTXX", dr["CT_QTXX"])};

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, strSql.ToString(), parameters);
                            succCount++;
                            #endregion
                        }
                    }
                }
                if (dtRldc != null && dtRldc.Rows.Count > 0)
                {
                    totalCount += dtRldc.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtRldc.Rows)
                    {
                        error = VerifyData(dr, drRldc, "IMPORT");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region insert
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO RLDC_MAIN(");
                            strSql.Append("MAIN_ID,CREATE_BY,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,RLDC_RLLX,RLDC_DDGLMD,RLDC_DLXDCZZL,RLDC_DDHHJSTJXXDCZBNL,RLDC_ZHGKHQL,RLDC_ZHGKXSLC,RLDC_CDDMSXZGXSCS,RLDC_QDDJEDGL,RLDC_QDDJLX,RLDC_CQPLX,RLDC_QDDJFZNJ,RLDC_CQPBCGZYL,RLDC_CQPRJ,CREATETIME,UPDATE_BY,UPDATETIME,HGSPBM,CT_QTXX)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@CREATE_BY,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@RLDC_RLLX,@RLDC_DDGLMD,@RLDC_DLXDCZZL,@RLDC_DDHHJSTJXXDCZBNL,@RLDC_ZHGKHQL,@RLDC_ZHGKXSLC,@RLDC_CDDMSXZGXSCS,@RLDC_QDDJEDGL,@RLDC_QDDJLX,@RLDC_CQPLX,@RLDC_QDDJFZNJ,@RLDC_CQPBCGZYL,@RLDC_CQPRJ,@CREATETIME,@UPDATE_BY,@UPDATETIME,@HGSPBM,@CT_QTXX)");
                            OleDbParameter[] parameters = {
					        new OleDbParameter("@MAIN_ID", dr["MAIN_ID"]),
					        new OleDbParameter("@CREATE_BY", Utils.localUserId),
					        new OleDbParameter("@JKQCZJXS", JKQCJXS),
					        new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
					        new OleDbParameter("@CLXH", dr["CLXH"]),
					        new OleDbParameter("@CLZL", dr["CLZL"]),
					        new OleDbParameter("@RLLX", dr["RLLX"]),
					        new OleDbParameter("@ZCZBZL", ""),
					        new OleDbParameter("@ZGCS", dr["ZGCS"]),
					        new OleDbParameter("@LTGG", ""),
					        new OleDbParameter("@ZJ", dr["ZJ"]),
					        new OleDbParameter("@TYMC", dr["TYMC"]),
					        new OleDbParameter("@YYC", dr["YYC"]),
					        new OleDbParameter("@ZWPS", dr["ZWPS"]),
					        new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
					        new OleDbParameter("@EDZK", dr["EDZK"]),
					        new OleDbParameter("@LJ", ""),
					        new OleDbParameter("@QDXS", dr["QDXS"]),
					        new OleDbParameter("@STATUS", (int)Status.待上报),
					        new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
					        new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
					        new OleDbParameter("@RLDC_RLLX", dr["RLDC_RLLX"]),
					        new OleDbParameter("@RLDC_DDGLMD", dr["RLDC_DDGLMD"]),
					        new OleDbParameter("@RLDC_DLXDCZZL", dr["RLDC_DLXDCZZL"]),
					        new OleDbParameter("@RLDC_DDHHJSTJXXDCZBNL", dr["RLDC_DDHHJSTJXXDCZBNL"]),
					        new OleDbParameter("@RLDC_ZHGKHQL", dr["RLDC_ZHGKHQL"]),
					        new OleDbParameter("@RLDC_ZHGKXSLC", dr["RLDC_ZHGKXSLC"]),
					        new OleDbParameter("@RLDC_CDDMSXZGXSCS", dr["RLDC_CDDMSXZGXSCS"]),
					        new OleDbParameter("@RLDC_QDDJEDGL", dr["RLDC_QDDJEDGL"]),
					        new OleDbParameter("@RLDC_QDDJLX", dr["RLDC_QDDJLX"]),
					        new OleDbParameter("@RLDC_CQPLX", dr["RLDC_CQPLX"]),
					        new OleDbParameter("@RLDC_QDDJFZNJ", dr["RLDC_QDDJFZNJ"]),
					        new OleDbParameter("@RLDC_CQPBCGZYL", dr["RLDC_CQPBCGZYL"]),
					        new OleDbParameter("@RLDC_CQPRJ", dr["RLDC_CQPRJ"]),
					        new OleDbParameter("@CREATETIME", DateTime.Today),
                            new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                            new OleDbParameter("@UPDATETIME", DateTime.Today),
					        new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
					        new OleDbParameter("@CT_QTXX", dr["CT_QTXX"])};

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
        /// 新导入轮胎规格信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveLtggData(DataTable dtLtgg)
        {
            int succCount = 0;
            int totalCount = 0;

            string msg = string.Empty;
            string error = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);

            try
            {
                if (dtLtgg != null && dtLtgg.Rows.Count > 0)
                {
                    totalCount = dtLtgg.Rows.Count;
                    con.Open();
                    foreach (DataRow dr in dtLtgg.Rows)
                    {
                        error = this.VerifyLtggData(dr, "IMPORT"); // 校验CPOS数据
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region INSERT

                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO MAIN_LTGG(");
                            strSql.Append("MAIN_ID,LTGG_ID,LTGG,CREATETIME,UPDATETIME)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@LTGG_ID,@LTGG,@CREATETIME,UPDATETIME)");

                            DateTime createDate = DateTime.Now;
                            OleDbParameter createTime = new OleDbParameter("@CREATETIME", createDate);
                            createTime.OleDbType = OleDbType.DBDate;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            OleDbParameter[] parameters = {
                                                          new OleDbParameter("@MAIN_ID",Convert.ToString(dr["MAIN_ID"])),
                                                          new OleDbParameter("@LTGG_ID",Convert.ToString(dr["LTGG_ID"])),
                                                          new OleDbParameter("@LTGG",Convert.ToString(dr["LTGG"])),
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
        /// 新导入轮距信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveLjData(DataTable dtLj)
        {
            int succCount = 0;
            int totalCount = 0;

            string msg = string.Empty;
            string error = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);

            try
            {
                if (dtLj != null && dtLj.Rows.Count > 0)
                {
                    totalCount = dtLj.Rows.Count;
                    con.Open();
                    foreach (DataRow dr in dtLj.Rows)
                    {
                        error = this.VerifyLjData(dr, "IMPORT"); // 校验CPOS数据
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region INSERT

                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO MAIN_LJ(");
                            strSql.Append("MAIN_ID,LJ_ID,LJ,CREATETIME,UPDATETIME)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@LJ_ID,@LJ,@CREATETIME,UPDATETIME)");

                            DateTime createDate = DateTime.Now;
                            OleDbParameter createTime = new OleDbParameter("@CREATETIME", createDate);
                            createTime.OleDbType = OleDbType.DBDate;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            OleDbParameter[] parameters = {
                                                          new OleDbParameter("@MAIN_ID",Convert.ToString(dr["MAIN_ID"])),
                                                          new OleDbParameter("@LJ_ID",Convert.ToString(dr["LJ_ID"])),
                                                          new OleDbParameter("@LJ",Convert.ToString(dr["LJ"])),
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
        /// 新导入整车整备质量信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveZczbzlData(DataTable dtZczbzl)
        {
            int succCount = 0;
            int totalCount = 0;

            string msg = string.Empty;
            string error = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);

            try
            {
                if (dtZczbzl != null && dtZczbzl.Rows.Count > 0)
                {
                    totalCount = dtZczbzl.Rows.Count;
                    con.Open();
                    foreach (DataRow dr in dtZczbzl.Rows)
                    {
                        error = this.VerifyZczbzlData(dr, "IMPORT"); // 校验CPOS数据
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region INSERT

                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("INSERT INTO MAIN_ZCZBZL(");
                            strSql.Append("MAIN_ID,ZCZBZL_ID,ZCZBZL,CREATETIME,UPDATETIME)");
                            strSql.Append(" VALUES (");
                            strSql.Append("@MAIN_ID,@ZCZBZL_ID,@ZCZBZL,@CREATETIME,UPDATETIME)");

                            DateTime createDate = DateTime.Now;
                            OleDbParameter createTime = new OleDbParameter("@CREATETIME", createDate);
                            createTime.OleDbType = OleDbType.DBDate;
                            OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", createDate);
                            updateTime.OleDbType = OleDbType.DBDate;

                            OleDbParameter[] parameters = {
                                                          new OleDbParameter("@MAIN_ID",Convert.ToString(dr["MAIN_ID"])),
                                                          new OleDbParameter("@ZCZBZL_ID",Convert.ToString(dr["ZCZBZL_ID"])),
                                                          new OleDbParameter("@ZCZBZL",Convert.ToString(dr["ZCZBZL"])),
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
        /// 修改已经导入的车型参数信息
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string UpdateMainData(DataSet ds, List<string> mainUpdateList)
        {
            int totalCount = 0;
            int succCount = 0;
            string msg = string.Empty;
            string mainId = string.Empty;

            try
            {
                DataTable dtCtny = D2D(ds.Tables[CTNY]);
                DataRow[] drCtny = checkData.Select("FUEL_TYPE='" + dtCtny.TableName + "' and STATUS=1");

                DataTable dtFcdsHhdl = D2D(ds.Tables[FCDSHHDL]);
                DataRow[] drFcdsHhdl = checkData.Select("FUEL_TYPE='" + dtFcdsHhdl.TableName + "' and STATUS=1");

                DataTable dtCdsHhdl = D2D(ds.Tables[CDSHHDL]);
                DataRow[] drCdsHhdl = checkData.Select("FUEL_TYPE='" + dtCdsHhdl.TableName + "' and STATUS=1");

                DataTable dtCdd = D2D(ds.Tables[CDD]);
                DataRow[] drCdd = checkData.Select("FUEL_TYPE='" + dtCdd.TableName + "' and STATUS=1");

                DataTable dtRldc = D2D(ds.Tables[RLDC]);
                DataRow[] drRldc = checkData.Select("FUEL_TYPE='" + dtRldc.TableName + "' and STATUS=1");

                if (dtCtny != null && dtCtny.Rows.Count > 0)
                {
                    totalCount += dtCtny.Rows.Count;
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
                                         SET JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                                            ZGCS=@ZGCS,ZJ=@ZJ,
                                            TYMC=@TYMC,YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,
                                            EDZK=@EDZK,QDXS=@QDXS,STATUS=@STATUS,
                                            JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,CT_BSQDWS=@CT_BSQDWS,CT_BSQXS=@CT_BSQXS,
                                            CT_EDGL=@CT_EDGL,CT_FDJXH=@CT_FDJXH,CT_JGL=@CT_JGL,CT_PL=@CT_PL,
                                            CT_QGS=@CT_QGS,CT_QTXX=@CT_QTXX,CT_SJGKRLXHL=@CT_SJGKRLXHL,CT_SQGKRLXHL=@CT_SQGKRLXHL,
                                            CT_ZHGKCO2PFL=@CT_ZHGKCO2PFL,CT_ZHGKRLXHL=@CT_ZHGKRLXHL,UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                                            HGSPBM=@HGSPBM
                                          WHERE MAIN_ID=@MAIN_ID";

                            OleDbParameter[] parameters = {
					        new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
					        new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
					        new OleDbParameter("@CLXH", OleDbType.VarChar,100),
					        new OleDbParameter("@CLZL", OleDbType.VarChar,200),
					        new OleDbParameter("@RLLX", OleDbType.VarChar,200),

					        new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
					        new OleDbParameter("@ZJ", OleDbType.VarChar,255),
					        new OleDbParameter("@TYMC", OleDbType.VarChar,200),

					        new OleDbParameter("@YYC", OleDbType.VarChar,200),
					        new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
					        new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
					        new OleDbParameter("@EDZK", OleDbType.VarChar,255),

					        new OleDbParameter("@QDXS", OleDbType.VarChar,200),
					        new OleDbParameter("@STATUS", OleDbType.VarChar,1),
					        new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
					        new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),

					        new OleDbParameter("@CT_BSQDWS", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_BSQXS", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_EDGL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_FDJXH", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_JGL", OleDbType.VarChar,200),

					        new OleDbParameter("@CT_PL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_QGS", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_QTXX", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_SJGKRLXHL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_SQGKRLXHL", OleDbType.VarChar,200),

					        new OleDbParameter("@CT_ZHGKCO2PFL", OleDbType.VarChar,200),
					        new OleDbParameter("@CT_ZHGKRLXHL", OleDbType.VarChar,200),
                            new OleDbParameter("@UPDATE_BY", OleDbType.VarChar,200),
                            new OleDbParameter("@UPDATETIME", OleDbType.Date),
					        new OleDbParameter("@HGSPBM", OleDbType.VarChar,50),
                            
					        new OleDbParameter("@MAIN_ID", OleDbType.VarChar,50)
                        };

                            parameters[0].Value = JKQCJXS;
                            parameters[1].Value = dr["QCSCQY"];
                            parameters[2].Value = dr["CLXH"];
                            parameters[3].Value = dr["CLZL"];
                            parameters[4].Value = dr["RLLX"];

                            parameters[5].Value = dr["ZGCS"];
                            parameters[6].Value = dr["ZJ"];
                            parameters[7].Value = dr["TYMC"];

                            parameters[8].Value = dr["YYC"];
                            parameters[9].Value = dr["ZWPS"];
                            parameters[10].Value = dr["ZDSJZZL"];
                            parameters[11].Value = dr["EDZK"];

                            parameters[12].Value = dr["QDXS"];
                            parameters[13].Value = (int)Status.待上报;
                            parameters[14].Value = dr["JYJGMC"];
                            parameters[15].Value = dr["JYBGBH"];

                            parameters[16].Value = dr["CT_BSQDWS"];
                            parameters[17].Value = dr["CT_BSQXS"];
                            parameters[18].Value = dr["CT_EDGL"];
                            parameters[19].Value = dr["CT_FDJXH"];
                            parameters[20].Value = dr["CT_JGL"];

                            parameters[21].Value = dr["CT_PL"];
                            parameters[22].Value = dr["CT_QGS"];
                            parameters[23].Value = dr["CT_QTXX"];
                            parameters[24].Value = dr["CT_SJGKRLXHL"];
                            parameters[25].Value = dr["CT_SQGKRLXHL"];

                            parameters[26].Value = dr["CT_ZHGKCO2PFL"];
                            parameters[27].Value = dr["CT_ZHGKRLXHL"];
                            parameters[28].Value = Utils.localUserId;
                            parameters[29].Value = DateTime.Now;
                            parameters[30].Value = dr["HGSPBM"];

                            parameters[31].Value = dr["MAIN_ID"];

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlCtny.ToString(), parameters);
                            succCount++;
                            mainUpdateList.Add(mainId);
                            #endregion
                        }
                    }
                }
                if (dtFcdsHhdl != null && dtFcdsHhdl.Rows.Count > 0)
                {
                    totalCount += dtFcdsHhdl.Rows.Count;
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
                            string sqlFcds = @"UPDATE FCDS_MAIN
                                        SET JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                                            ZGCS=@ZGCS,ZJ=@ZJ,TYMC=@TYMC,
                                            YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,EDZK=@EDZK,
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
                                            FCDS_HHDL_ZHKGCO2PL=@FCDS_HHDL_ZHKGCO2PL,UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                                            HGSPBM=@HGSPBM,CT_QTXX=@CT_QTXX
                                         WHERE MAIN_ID=@MAIN_ID";

                            OleDbParameter[] parameters = {
					    new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
					    new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
					    new OleDbParameter("@CLXH", OleDbType.VarChar,100),
					    new OleDbParameter("@CLZL", OleDbType.VarChar,200),
					    new OleDbParameter("@RLLX", OleDbType.VarChar,200),

					    //new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
					    new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
					    //new OleDbParameter("@LTGG", OleDbType.VarChar,200),
					    new OleDbParameter("@ZJ", OleDbType.VarChar,255),
					    new OleDbParameter("@TYMC", OleDbType.VarChar,200),

					    new OleDbParameter("@YYC", OleDbType.VarChar,200),
					    new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
					    new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
					    new OleDbParameter("@EDZK", OleDbType.VarChar,255),
					    //new OleDbParameter("@LJ", OleDbType.VarChar,255),

					    new OleDbParameter("@QDXS", OleDbType.VarChar,200),
					    new OleDbParameter("@STATUS", OleDbType.VarChar,1),
					    new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
					    new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
					    new OleDbParameter("@FCDS_HHDL_BSQDWS", OleDbType.VarChar,200),

					    new OleDbParameter("@FCDS_HHDL_BSQXS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCBNL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY", OleDbType.VarChar,200),

					    new OleDbParameter("@FCDS_HHDL_DLXDCZZL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_DLXDCZZNL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_EDGL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_FDJXH", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_HHDLJGXS", OleDbType.VarChar,200),

					    new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_JGL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_PL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QDDJEDGL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QDDJFZNJ", OleDbType.VarChar,200),

					    new OleDbParameter("@FCDS_HHDL_QDDJLX", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_QGS", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_SJGKRLXHL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN", OleDbType.VarChar,200),

					    new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", OleDbType.VarChar,200),
					    new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", OleDbType.VarChar,200),
                        new OleDbParameter("@UPDATE_BY", OleDbType.VarChar,200),
                        new OleDbParameter("@UPDATETIME", OleDbType.Date),
					    new OleDbParameter("@HGSPBM", OleDbType.VarChar,50),
					    new OleDbParameter("@CT_QTXX", OleDbType.VarChar,255),
                        
					    new OleDbParameter("@MAIN_ID", OleDbType.VarChar,50)
                                                      };

                            parameters[0].Value = JKQCJXS;
                            parameters[1].Value = dr["QCSCQY"];
                            parameters[2].Value = dr["CLXH"];
                            parameters[3].Value = dr["CLZL"];
                            parameters[4].Value = dr["RLLX"];

                            parameters[5].Value = dr["ZGCS"];
                            parameters[6].Value = dr["ZJ"];
                            parameters[7].Value = dr["TYMC"];

                            parameters[8].Value = dr["YYC"];
                            parameters[9].Value = dr["ZWPS"];
                            parameters[10].Value = dr["ZDSJZZL"];
                            parameters[11].Value = dr["EDZK"];

                            parameters[12].Value = dr["QDXS"];
                            parameters[13].Value = (int)Status.待上报;
                            parameters[14].Value = dr["JYJGMC"];
                            parameters[15].Value = dr["JYBGBH"];
                            parameters[16].Value = dr["FCDS_HHDL_BSQDWS"];

                            parameters[17].Value = dr["FCDS_HHDL_BSQXS"];
                            parameters[18].Value = dr["FCDS_HHDL_CDDMSXZGCS"];
                            parameters[19].Value = dr["FCDS_HHDL_CDDMSXZHGKXSLC"];
                            parameters[20].Value = dr["FCDS_HHDL_DLXDCBNL"];
                            parameters[21].Value = dr["FCDS_HHDL_DLXDCZBCDY"];

                            parameters[22].Value = dr["FCDS_HHDL_DLXDCZZL"];
                            parameters[23].Value = dr["FCDS_HHDL_DLXDCZZNL"];
                            parameters[24].Value = dr["FCDS_HHDL_EDGL"];
                            parameters[25].Value = dr["FCDS_HHDL_FDJXH"];
                            parameters[26].Value = dr["FCDS_HHDL_HHDLJGXS"];

                            parameters[27].Value = dr["FCDS_HHDL_HHDLZDDGLB"];
                            parameters[28].Value = dr["FCDS_HHDL_JGL"];
                            parameters[29].Value = dr["FCDS_HHDL_PL"];
                            parameters[30].Value = dr["FCDS_HHDL_QDDJEDGL"];
                            parameters[31].Value = dr["FCDS_HHDL_QDDJFZNJ"];

                            parameters[32].Value = dr["FCDS_HHDL_QDDJLX"];
                            parameters[33].Value = dr["FCDS_HHDL_QGS"];
                            parameters[34].Value = dr["FCDS_HHDL_SJGKRLXHL"];
                            parameters[35].Value = dr["FCDS_HHDL_SQGKRLXHL"];
                            parameters[36].Value = dr["FCDS_HHDL_XSMSSDXZGN"];

                            parameters[37].Value = dr["FCDS_HHDL_ZHGKRLXHL"];
                            parameters[38].Value = dr["FCDS_HHDL_ZHKGCO2PL"];
                            parameters[39].Value = Utils.localUserId;
                            parameters[40].Value = DateTime.Today;
                            parameters[41].Value = dr["HGSPBM"];
                            parameters[42].Value = dr["CT_QTXX"];

                            parameters[43].Value = dr["MAIN_ID"];

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlFcds.ToString(), parameters);
                            succCount++;
                            mainUpdateList.Add(mainId);
                            #endregion
                        }
                    }
                }
                if (dtCdsHhdl != null && dtCdsHhdl.Rows.Count > 0)
                {
                    totalCount += dtCdsHhdl.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtCdsHhdl.Rows)
                    {
                        error = VerifyData(dr, drCdsHhdl, "UPDATE");      //单行验证
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
                                            ZGCS=@ZGCS,ZJ=@ZJ,TYMC=@TYMC,
                                            YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,EDZK=@EDZK,
                                            QDXS=@QDXS,STATUS=@STATUS,JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,
                                            CDS_HHDL_HHDLJGXS=@CDS_HHDL_HHDLJGXS,CDS_HHDL_XSMSSDXZGN=@CDS_HHDL_XSMSSDXZGN,
                                            CDS_HHDL_DLXDCZZL=@CDS_HHDL_DLXDCZZL,CDS_HHDL_DLXDCZZNL=@CDS_HHDL_DLXDCZZNL,
                                            CDS_HHDL_DLXDCBNL=@CDS_HHDL_DLXDCBNL,CDS_HHDL_CDDMSXZHGKXSLC=@CDS_HHDL_CDDMSXZHGKXSLC,
                                            CDS_HHDL_CDDMSXZGCS=@CDS_HHDL_CDDMSXZGCS,CDS_HHDL_DLXDCZBCDY=@CDS_HHDL_DLXDCZBCDY,
                                            CDS_HHDL_QDDJLX=@CDS_HHDL_QDDJLX,CDS_HHDL_HHDLZDDGLB=@CDS_HHDL_HHDLZDDGLB,
                                            CDS_HHDL_QDDJFZNJ=@CDS_HHDL_QDDJFZNJ,CDS_HHDL_QDDJEDGL=@CDS_HHDL_QDDJEDGL,
                                            CDS_HHDL_ZHGKDNXHL=@CDS_HHDL_ZHGKDNXHL,CDS_HHDL_ZHGKRLXHL=@CDS_HHDL_ZHGKRLXHL,
                                            CDS_HHDL_ZHKGCO2PL=@CDS_HHDL_ZHKGCO2PL,CDS_HHDL_FDJXH=@CDS_HHDL_FDJXH,
                                            CDS_HHDL_QGS=@CDS_HHDL_QGS,CDS_HHDL_PL=@CDS_HHDL_PL,CDS_HHDL_EDGL=@CDS_HHDL_EDGL,
                                            CDS_HHDL_JGL=@CDS_HHDL_JGL,CDS_HHDL_BSQXS=@CDS_HHDL_BSQXS,CDS_HHDL_BSQDWS=@CDS_HHDL_BSQDWS,
                                            UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                                            HGSPBM=@HGSPBM,CT_QTXX=@CT_QTXX
                                         WHERE MAIN_ID=@MAIN_ID";

                            OleDbParameter[] parameters = 
                            {
					            new OleDbParameter("@JKQCZJXS", JKQCJXS),
					            new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
					            new OleDbParameter("@CLXH", dr["CLXH"]),
					            new OleDbParameter("@CLZL", dr["CLZL"]),
					            new OleDbParameter("@RLLX", dr["RLLX"]),

					            //new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
					            new OleDbParameter("@ZGCS", dr["ZGCS"]),
					            //new OleDbParameter("@LTGG", dr["QCSCQY"]),
					            new OleDbParameter("@ZJ", dr["ZJ"]),
					            new OleDbParameter("@TYMC", dr["TYMC"]),

					            new OleDbParameter("@YYC", dr["YYC"]),
					            new OleDbParameter("@ZWPS", dr["ZWPS"]),
					            new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
					            new OleDbParameter("@EDZK", dr["EDZK"]),
					            //new OleDbParameter("@LJ", dr["QCSCQY"]),

					            new OleDbParameter("@QDXS", dr["QDXS"]),
					            new OleDbParameter("@STATUS", (int)Status.待上报),
					            new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
					            new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
					            new OleDbParameter("@CDS_HHDL_HHDLJGXS", dr["CDS_HHDL_HHDLJGXS"]),

					            new OleDbParameter("@CDS_HHDL_XSMSSDXZGN", dr["CDS_HHDL_XSMSSDXZGN"]),
					            new OleDbParameter("@CDS_HHDL_DLXDCZZL", dr["CDS_HHDL_DLXDCZZL"]),
					            new OleDbParameter("@CDS_HHDL_DLXDCZZNL", dr["CDS_HHDL_DLXDCZZNL"]),
					            new OleDbParameter("@CDS_HHDL_DLXDCBNL", dr["CDS_HHDL_DLXDCBNL"]),
					            new OleDbParameter("@CDS_HHDL_CDDMSXZHGKXSLC", dr["CDS_HHDL_CDDMSXZHGKXSLC"]),

					            new OleDbParameter("@CDS_HHDL_CDDMSXZGCS", dr["CDS_HHDL_CDDMSXZGCS"]),
					            new OleDbParameter("@CDS_HHDL_DLXDCZBCDY", dr["CDS_HHDL_DLXDCZBCDY"]),
					            new OleDbParameter("@CDS_HHDL_QDDJLX", dr["CDS_HHDL_QDDJLX"]),
					            new OleDbParameter("@CDS_HHDL_HHDLZDDGLB", dr["CDS_HHDL_HHDLZDDGLB"]),
					            new OleDbParameter("@CDS_HHDL_QDDJFZNJ", dr["CDS_HHDL_QDDJFZNJ"]),

					            new OleDbParameter("@CDS_HHDL_QDDJEDGL", dr["CDS_HHDL_QDDJEDGL"]),
					            new OleDbParameter("@CDS_HHDL_ZHGKDNXHL", dr["CDS_HHDL_ZHGKDNXHL"]),
					            new OleDbParameter("@CDS_HHDL_ZHGKRLXHL", dr["CDS_HHDL_ZHGKRLXHL"]),
					            new OleDbParameter("@CDS_HHDL_ZHKGCO2PL", dr["CDS_HHDL_ZHKGCO2PL"]),
					            new OleDbParameter("@CDS_HHDL_FDJXH", dr["CDS_HHDL_FDJXH"]),

					            new OleDbParameter("@CDS_HHDL_QGS", dr["CDS_HHDL_QGS"]),
					            new OleDbParameter("@CDS_HHDL_PL", dr["CDS_HHDL_PL"]),
					            new OleDbParameter("@CDS_HHDL_EDGL", dr["CDS_HHDL_EDGL"]),
					            new OleDbParameter("@CDS_HHDL_JGL", dr["CDS_HHDL_JGL"]),
					            new OleDbParameter("@CDS_HHDL_BSQXS", dr["CDS_HHDL_BSQXS"]),

					            new OleDbParameter("@CDS_HHDL_BSQDWS", dr["CDS_HHDL_BSQDWS"]),
                                new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                                new OleDbParameter("@UPDATETIME", DateTime.Today),
					            new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
					            new OleDbParameter("@CT_QTXX", dr["CT_QTXX"]),
                        
					            new OleDbParameter("@MAIN_ID", dr["MAIN_ID"])
                            };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlFcds.ToString(), parameters);
                            succCount++;
                            mainUpdateList.Add(mainId);
                            #endregion
                        }
                    }
                }
                if (dtCdd != null && dtCdd.Rows.Count > 0)
                {
                    totalCount += dtCdd.Rows.Count;
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
                            string sqlFcds = @"UPDATE CDD_MAIN
                                        SET JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                                            ZGCS=@ZGCS,ZJ=@ZJ,TYMC=@TYMC,
                                            YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,EDZK=@EDZK,
                                            QDXS=@QDXS,STATUS=@STATUS,JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,
                                            CDD_DLXDCZZL=@CDD_DLXDCZZL,CDD_DLXDCBNL=@CDD_DLXDCBNL,CDD_DLXDCZEDNL=@CDD_DLXDCZEDNL,
                                            CDD_DDQC30FZZGCS=@CDD_DDQC30FZZGCS,CDD_DDXDCZZLYZCZBZLDBZ=@CDD_DDXDCZZLYZCZBZLDBZ,
                                            CDD_DLXDCZBCDY=@CDD_DLXDCZBCDY,CDD_ZHGKXSLC=@CDD_ZHGKXSLC,CDD_QDDJLX=@CDD_QDDJLX,
                                            CDD_QDDJEDGL=@CDD_QDDJEDGL,CDD_QDDJFZNJ=@CDD_QDDJFZNJ,CDD_ZHGKDNXHL=@CDD_ZHGKDNXHL,
                                            UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                                            HGSPBM=@HGSPBM,CT_QTXX=@CT_QTXX
                                         WHERE MAIN_ID=@MAIN_ID";

                            OleDbParameter[] parameters = 
                            {
					            new OleDbParameter("@JKQCZJXS", JKQCJXS),
					            new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
					            new OleDbParameter("@CLXH", dr["CLXH"]),
					            new OleDbParameter("@CLZL", dr["CLZL"]),
					            new OleDbParameter("@RLLX", dr["RLLX"]),

					            //new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
					            new OleDbParameter("@ZGCS", dr["ZGCS"]),
					            //new OleDbParameter("@LTGG", dr["QCSCQY"]),
					            new OleDbParameter("@ZJ", dr["ZJ"]),
					            new OleDbParameter("@TYMC", dr["TYMC"]),

					            new OleDbParameter("@YYC", dr["YYC"]),
					            new OleDbParameter("@ZWPS", dr["ZWPS"]),
					            new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
					            new OleDbParameter("@EDZK", dr["EDZK"]),
					            //new OleDbParameter("@LJ", dr["QCSCQY"]),

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
                        
					            new OleDbParameter("@MAIN_ID", dr["MAIN_ID"])
                            };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlFcds.ToString(), parameters);
                            succCount++;
                            mainUpdateList.Add(mainId);
                            #endregion
                        }
                    }
                }
                if (dtRldc != null && dtRldc.Rows.Count > 0)
                {
                    totalCount += dtRldc.Rows.Count;
                    string error = string.Empty;
                    foreach (DataRow dr in dtRldc.Rows)
                    {
                        error = VerifyData(dr, drRldc, "UPDATE");      //单行验证
                        if (!string.IsNullOrEmpty(error))
                        {
                            msg += error;
                        }
                        else
                        {
                            #region UPDATE
                            mainId = dr["MAIN_ID"].ToString();
                            string sqlFcds = @"UPDATE RLDC_MAIN
                                        SET JKQCZJXS=@JKQCZJXS,QCSCQY=@QCSCQY,CLXH=@CLXH,CLZL=@CLZL,RLLX=@RLLX,
                                            ZGCS=@ZGCS,ZJ=@ZJ,TYMC=@TYMC,
                                            YYC=@YYC,ZWPS=@ZWPS,ZDSJZZL=@ZDSJZZL,EDZK=@EDZK,
                                            QDXS=@QDXS,STATUS=@STATUS,JYJGMC=@JYJGMC,JYBGBH=@JYBGBH,
                                            RLDC_RLLX=@RLDC_RLLX,RLDC_DDGLMD=@RLDC_DDGLMD,RLDC_DLXDCZZL=@RLDC_DLXDCZZL,
                                            RLDC_DDHHJSTJXXDCZBNL=@RLDC_DDHHJSTJXXDCZBNL,RLDC_ZHGKHQL=@RLDC_ZHGKHQL,
                                            RLDC_ZHGKXSLC=@RLDC_ZHGKXSLC,RLDC_CDDMSXZGXSCS=@RLDC_CDDMSXZGXSCS,
                                            RLDC_QDDJEDGL=@RLDC_QDDJEDGL,RLDC_QDDJLX=@RLDC_QDDJLX,RLDC_CQPLX=@RLDC_CQPLX,
                                            RLDC_QDDJFZNJ=@RLDC_QDDJFZNJ,RLDC_CQPBCGZYL=@RLDC_CQPBCGZYL,RLDC_CQPRJ=@RLDC_CQPRJ,
                                            UPDATE_BY=@UPDATE_BY,UPDATETIME=@UPDATETIME,
                                            HGSPBM=@HGSPBM,CT_QTXX=@CT_QTXX
                                         WHERE MAIN_ID=@MAIN_ID";

                            OleDbParameter[] parameters = 
                            {
					            new OleDbParameter("@JKQCZJXS", JKQCJXS),
					            new OleDbParameter("@QCSCQY", dr["QCSCQY"]),
					            new OleDbParameter("@CLXH", dr["CLXH"]),
					            new OleDbParameter("@CLZL", dr["CLZL"]),
					            new OleDbParameter("@RLLX", dr["RLLX"]),

					            //new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
					            new OleDbParameter("@ZGCS", dr["ZGCS"]),
					            //new OleDbParameter("@LTGG", dr["QCSCQY"]),
					            new OleDbParameter("@ZJ", dr["ZJ"]),
					            new OleDbParameter("@TYMC", dr["TYMC"]),

					            new OleDbParameter("@YYC", dr["YYC"]),
					            new OleDbParameter("@ZWPS", dr["ZWPS"]),
					            new OleDbParameter("@ZDSJZZL", dr["ZDSJZZL"]),
					            new OleDbParameter("@EDZK", dr["EDZK"]),
					            //new OleDbParameter("@LJ", dr["QCSCQY"]),

					            new OleDbParameter("@QDXS", dr["QDXS"]),
					            new OleDbParameter("@STATUS", (int)Status.待上报),
					            new OleDbParameter("@JYJGMC", dr["JYJGMC"]),
					            new OleDbParameter("@JYBGBH", dr["JYBGBH"]),
					            new OleDbParameter("@RLDC_RLLX", dr["RLDC_RLLX"]),

					            new OleDbParameter("@RLDC_DDGLMD", dr["RLDC_DDGLMD"]),
					            new OleDbParameter("@RLDC_DLXDCZZL", dr["RLDC_DLXDCZZL"]),
					            new OleDbParameter("@RLDC_DDHHJSTJXXDCZBNL", dr["RLDC_DDHHJSTJXXDCZBNL"]),
					            new OleDbParameter("@RLDC_ZHGKHQL", dr["RLDC_ZHGKHQL"]),
					            new OleDbParameter("@RLDC_ZHGKXSLC", dr["RLDC_ZHGKXSLC"]),

					            new OleDbParameter("@RLDC_CDDMSXZGXSCS", dr["RLDC_CDDMSXZGXSCS"]),
					            new OleDbParameter("@RLDC_QDDJEDGL", dr["RLDC_QDDJEDGL"]),
					            new OleDbParameter("@RLDC_QDDJLX", dr["RLDC_QDDJLX"]),
					            new OleDbParameter("@RLDC_CQPLX", dr["RLDC_CQPLX"]),
					            new OleDbParameter("@RLDC_QDDJFZNJ", dr["RLDC_QDDJFZNJ"]),

					            new OleDbParameter("@RLDC_CQPBCGZYL", dr["RLDC_CQPBCGZYL"]),
					            new OleDbParameter("@RLDC_CQPRJ", dr["RLDC_CQPRJ"]),

                                new OleDbParameter("@UPDATE_BY", Utils.localUserId),
                                new OleDbParameter("@UPDATETIME", DateTime.Today),
					            new OleDbParameter("@HGSPBM", dr["HGSPBM"]),
					            new OleDbParameter("@CT_QTXX", dr["CT_QTXX"]),
                        
					            new OleDbParameter("@MAIN_ID", dr["MAIN_ID"])
                            };

                            AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlFcds.ToString(), parameters);
                            succCount++;
                            mainUpdateList.Add(mainId);
                            #endregion
                        }
                    }
                }
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
        private DataTable D2D(DataTable dt)
        {
            string fiedName = "车型参数编号"; // 用于验证某行是否为空数据
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
                else if (dt.TableName == CDSHHDL)
                {
                    if (!dictCDSHHDL.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictCDSHHDL[c.ColumnName]);
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
                else if (dt.TableName == RLDC)
                {
                    if (!dictRLDC.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictRLDC[c.ColumnName]);
                }
                else if (dt.TableName == LTGG)
                {
                    if (!dictLTGG.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictLTGG[c.ColumnName]);
                }
                else if (dt.TableName == LJ)
                {
                    if (!dictLJ.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictLJ[c.ColumnName]);
                }
                else if (dt.TableName == ZCZBZL)
                {
                    if (!dictZCZBZL.ContainsKey(c.ColumnName))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictZCZBZL[c.ColumnName]);
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
        /// 将英文字段列名转换为中文名称 是D2D的反向转化
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="dt"></param>
        /// <param name="deleteIfNotFound">找不到对应列名是否删除该列</param>
        /// <returns></returns>
        public DataTable E2C(Dictionary<string, string> dict, DataTable dt, bool deleteIfNotFound)
        {
            for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
            {
                DataColumn dc = dt.Columns[colIndex];
                bool found = false;
                foreach (var kv in dict)
                {
                    if (kv.Value == dc.ColumnName)
                    {
                        dc.ColumnName = kv.Key;
                        found = true;
                        break;
                    }
                }
                if (!found && deleteIfNotFound)
                {
                    dt.Columns.Remove(dc);
                    colIndex--;
                }
            }
            return dt;
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
            dictLTGG = new Dictionary<string, string>();
            dictLJ = new Dictionary<string, string>();
            dictZCZBZL = new Dictionary<string, string>();
            dictVin = new Dictionary<string, string>();
            dictCTNYExport = new Dictionary<string, string>();
            dictCDDExport = new Dictionary<string, string>();
            dictCDSExport = new Dictionary<string, string>();
            dictFCDSExport = new Dictionary<string, string>();
            dictRLDCExport = new Dictionary<string, string>();
            dictCLMXExport = new Dictionary<string, string>();
            dictHZBGExport = new Dictionary<string, string>();
            dictHZBGExport_New = new Dictionary<string, string>();

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

            foreach (DataRow r in ds.Tables[LTGG].Rows)
            {
                dictLTGG.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[LJ].Rows)
            {
                dictLJ.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[ZCZBZL].Rows)
            {
                dictZCZBZL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[VIN].Rows)
            {
                dictVin.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[CTNYExport].Rows)
            {
                dictCTNYExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[CDSExport].Rows)
            {
                dictCDSExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[FCDSExport].Rows)
            {
                dictFCDSExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[CDDExport].Rows)
            {
                dictCDDExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[RLDCExport].Rows)
            {
                dictRLDCExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[CLMXExport].Rows)
            {
                dictCLMXExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[HZBGExport].Rows)
            {
                dictHZBGExport.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[HZBGExport_New].Rows)
            {
                dictHZBGExport_New.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
        }

        private void ReadTargetFuel()
        {
            string sql = @"select * from TARGET_FUEL where TARGET_STAGE = 3";
            this.dtTargetFuel = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null).Tables[0];
            this.dtTargetFuel.Columns.Remove("TARGET_STAGE");
        }

        private void ReadTargetFuel_New()
        {
            string sql = @"select * from TARGET_FUEL where TARGET_STAGE = 4";
            this.dtTargetFuel = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null).Tables[0];
            this.dtTargetFuel.Columns.Remove("TARGET_STAGE");
        }

        /// <summary>
        /// 保存已经就绪的数据
        /// </summary>
        /// <param name="drVin"></param>
        /// <param name="drMain"></param>
        /// <returns></returns>
        public string SaveReadyData(DataRow drVin, DataRow drMain, DataTable dtPam, DataRow drLtgg, DataRow drLj, DataRow drZb)
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
                                (   VIN,MAIN_ID,LTGG_ID,LJ_ID,ZCZBZL_ID,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,STATUS,CREATETIME,UPDATETIME,HGSPBM,QTXX,COCNO,CCCHOLDER,HGNO
                                ) VALUES
                                (   @VIN,@MAIN_ID,@LTGG_ID,@LJ_ID,@ZCZBZL_ID,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@STATUS,@CREATETIME,@UPDATETIME,@HGSPBM,@QTXX,@COCNO,@CCCHOLDER,@HGNO)";

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
                                     new OleDbParameter("@MAIN_ID",drMain["MAIN_ID"].ToString().Trim()),
                                     new OleDbParameter("@LTGG_ID",drLtgg["LTGG_ID"].ToString().Trim()),
                                     new OleDbParameter("@LJ_ID",drLj["LJ_ID"].ToString().Trim()),
                                     new OleDbParameter("@ZCZBZL_ID",drZb["ZCZBZL_ID"].ToString().Trim()),
                                     new OleDbParameter("@USER_ID",drMain["CREATE_BY"].ToString().Trim()),
                                     new OleDbParameter("@QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                     new OleDbParameter("@JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@CLXH",drMain["CLXH"].ToString().Trim()),
                                     new OleDbParameter("@CLZL",drMain["CLZL"].ToString().Trim()),
                                     new OleDbParameter("@RLLX",drMain["RLLX"].ToString().Trim()),
                                     new OleDbParameter("@ZCZBZL",drZb["ZCZBZL"].ToString().Trim()),
                                     new OleDbParameter("@ZGCS",drMain["ZGCS"].ToString().Trim()),
                                     new OleDbParameter("@LTGG",drLtgg["LTGG"].ToString().Trim()),
                                     new OleDbParameter("@ZJ",drMain["ZJ"].ToString().Trim()),
                                     new OleDbParameter("@TYMC",drMain["TYMC"].ToString().Trim()),
                                     new OleDbParameter("@YYC",drMain["YYC"].ToString().Trim()),
                                     new OleDbParameter("@ZWPS",drMain["ZWPS"].ToString().Trim()),
                                     new OleDbParameter("@ZDSJZZL",drMain["ZDSJZZL"].ToString().Trim()),
                                     new OleDbParameter("@EDZK",drMain["EDZK"].ToString().Trim()),
                                     new OleDbParameter("@LJ",drLj["LJ"].ToString().Trim()),
                                     new OleDbParameter("@QDXS",drMain["QDXS"].ToString().Trim()),
                                     new OleDbParameter("@JYJGMC",drMain["JYJGMC"].ToString().Trim()),
                                     new OleDbParameter("@JYBGBH",drMain["JYBGBH"].ToString().Trim()),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","1"),
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                     new OleDbParameter("@QTXX",drMain["CT_QTXX"].ToString().Trim()),
                                     new OleDbParameter("@COCNO",drVin["COCNO"].ToString().Trim()),
                                     new OleDbParameter("@CCCHOLDER",drVin["CCCHOLDER"].ToString().Trim()),
                                     new OleDbParameter("@HGNO",drVin["HGNO"].ToString().Trim())
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

                        string sqlStr = @"INSERT INTO VIN_INFO(VIN,MAIN_ID,CLZZRQ,STATUS,COCNO,CCCHOLDER,HGNO) Values (@VIN, @MAIN_ID,@CLZZRQ,@STATUS,@COCNO,@CCCHOLDER,@HGNO)";
                        OleDbParameter[] vinParamList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@MAIN_ID",drVin["MAIN_ID"].ToString().Trim()),
                                         new OleDbParameter("@CLZZRQ",Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim())),
                                         new OleDbParameter("@STATUS","0"),
                                         new OleDbParameter("@COCNO",drVin["COCNO"].ToString().Trim()),
                                         new OleDbParameter("@CCCHOLDER",drVin["CCCHOLDER"].ToString().Trim()),
                                         new OleDbParameter("@HGNO",drVin["HGNO"].ToString().Trim())
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
        /// 检查当前VIN数据是否已经存在于燃料数据表中
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        protected bool IsFuelDataExist(string vin)
        {
            bool isExist = false;

            string sqlQuery = @"SELECT VIN FROM FC_CLJBXX WHERE VIN='" + vin + "'";
            try
            {
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQuery, null);

                if (ds != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        isExist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isExist;
        }

        /// <summary>
        /// 验证导入的VIN表数据是否完整
        /// </summary>
        /// <param name="dsVin"></param>
        /// <returns></returns>
        protected string IsVinDataReady(DataSet dsVin)
        {
            string msg = string.Empty;
            string vin = string.Empty;

            try
            {
                foreach (DataRow dr in dsVin.Tables[0].Rows)
                {
                    string tempMsg = string.Empty;
                    vin = Convert.ToString(dr["VIN"]);
                    tempMsg += this.VerifyRequired("车型参数编号", Convert.ToString(dr["MAIN_ID"]));
                    tempMsg += this.VerifyRequired("整车整备质量代码", Convert.ToString(dr["ZCZBZL_ID"]));
                    tempMsg += this.VerifyRequired("轮胎规格代码", Convert.ToString(dr["LTGG_ID"]));
                    tempMsg += this.VerifyRequired("轮距代码", Convert.ToString(dr["LJ_ID"]));
                    tempMsg += this.VerifyDateTime("进口日期", Convert.ToString(dr["CLZZRQ"]));

                    if (!string.IsNullOrEmpty(tempMsg))
                    {
                        msg += vin + "：\r\n" + tempMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message;
            }

            return msg;
        }

        /// <summary>
        /// 验证导入的轮胎规格表数据是否完整
        /// </summary>
        /// <param name="dsVin"></param>
        /// <returns></returns>
        protected string IsLtggReady(DataTable dtLtgg)
        {
            string msg = string.Empty;
            string mainId = string.Empty;

            try
            {
                foreach (DataRow dr in dtLtgg.Rows)
                {
                    string tempMsg = string.Empty;
                    mainId = Convert.ToString(dr["MAIN_ID"]);
                    tempMsg += this.VerifyRequired("轮胎规格代码", Convert.ToString(dr["LTGG_ID"]));
                    tempMsg += this.VerifyRequired("轮胎规格", Convert.ToString(dr["LTGG"]));

                    if (!string.IsNullOrEmpty(tempMsg))
                    {
                        msg += mainId + "：\r\n" + tempMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message;
            }

            return msg;
        }

        /// <summary>
        /// 验证导入的轮距表数据是否完整
        /// </summary>
        /// <param name="dsVin"></param>
        /// <returns></returns>
        protected string IsLjReady(DataTable dtLj)
        {
            string msg = string.Empty;
            string mainId = string.Empty;

            try
            {
                foreach (DataRow dr in dtLj.Rows)
                {
                    string tempMsg = string.Empty;
                    mainId = Convert.ToString(dr["MAIN_ID"]);
                    tempMsg += this.VerifyRequired("轮距代码", Convert.ToString(dr["LJ_ID"]));
                    tempMsg += this.VerifyRequired("轮距", Convert.ToString(dr["LJ"]));

                    if (!string.IsNullOrEmpty(tempMsg))
                    {
                        msg += mainId + "：\r\n" + tempMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message;
            }

            return msg;
        }

        /// <summary>
        /// 验证导入的整车整备质量表数据是否完整
        /// </summary>
        /// <param name="dsVin"></param>
        /// <returns></returns>
        protected string IsZczbzlReady(DataTable dtZb)
        {
            string msg = string.Empty;
            string mainId = string.Empty;

            try
            {
                foreach (DataRow dr in dtZb.Rows)
                {
                    string tempMsg = string.Empty;
                    mainId = Convert.ToString(dr["MAIN_ID"]);
                    tempMsg += this.VerifyRequired("整车整备质量代码", Convert.ToString(dr["ZCZBZL_ID"]));
                    tempMsg += this.VerifyRequired("整车整备质量", Convert.ToString(dr["ZCZBZL"]));

                    if (!string.IsNullOrEmpty(tempMsg))
                    {
                        msg += mainId + "：\r\n" + tempMsg;
                    }
                }
            }
            catch (Exception ex)
            {
                msg += ex.Message;
            }

            return msg;
        }

        /// <summary>
        /// 获取全部参数数据
        /// </summary>
        /// <returns></returns>
        private DataTable GetCheckData()
        {
            string sql = "SELECT * FROM RLLX_PARAM";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            return ds.Tables[0];
        }

        public DataSet GetImportedVinData(string vin)
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

            return dsQuery;
        }

        /// <summary>
        /// 获取全部车型参数数据，用作合并VIN数据
        /// </summary>
        /// <returns></returns>
        public bool GetMainData()
        {
            bool flag = true;
            string sqlCtny = string.Format(@"SELECT * FROM CTNY_MAIN");
            string sqlFcds = string.Format(@"SELECT * FROM FCDS_MAIN");
            string sqlCds = string.Format(@"SELECT * FROM CDS_MAIN");
            string sqlCdd = string.Format(@"SELECT * FROM CDD_MAIN");
            string sqlRldc = string.Format(@"SELECT * FROM RLDC_MAIN");

            dtCtnyStatic = null;
            dtFcdsStatic = null;
            dtCdsStatic = null;
            dtCddStatic = null;
            dtRldcStatic = null;

            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
            dtCtnyStatic = ds.Tables[0];

            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
            dtFcdsStatic = ds.Tables[0];

            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCds, null);
            dtCdsStatic = ds.Tables[0];

            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCdd, null);
            dtCddStatic = ds.Tables[0];

            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlRldc, null);
            dtRldcStatic = ds.Tables[0];

            if (dtCtnyStatic.Rows.Count < 1 && dtFcdsStatic.Rows.Count < 1 && dtCdsStatic.Rows.Count < 1 && dtCddStatic.Rows.Count < 1 && dtRldcStatic.Rows.Count < 1)
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 获取全部车型参数数据，用作合并VIN数据
        /// </summary>
        /// <returns></returns>
        public bool GetOtherMainData(string paramName)
        {
            bool flag = true;
            string paramTable = string.Empty;
            DataTable dt = new DataTable();

            switch (paramName)
            {
                case LTGG:
                    paramTable = "MAIN_LTGG";
                    break;
                case LJ:
                    paramTable = "MAIN_LJ";
                    break;
                case ZCZBZL:
                    paramTable = "MAIN_ZCZBZL";
                    break;
                default: break;
            }

            string sqlOtherMain = string.Format(@"SELECT * FROM {0}", paramTable);

            try
            {
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlOtherMain, null);
                dt = ds.Tables[0];

                if (dt.Rows.Count < 1)
                {
                    flag = false;
                }

                switch (paramName)
                {
                    case LTGG:
                        dtLtggStatic = dt;
                        break;
                    case LJ:
                        dtLjStatic = dt;
                        break;
                    case ZCZBZL:
                        dtZczbzlStatic = dt;
                        break;
                    default: break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
            string sqlFcds = string.Format(@"SELECT MAIN_ID FROM FCDS_MAIN WHERE MAIN_ID='{0}'", mainId);
            string sqlCds = string.Format(@"SELECT MAIN_ID FROM CDS_MAIN WHERE MAIN_ID='{0}'", mainId);
            string sqlCdd = string.Format(@"SELECT MAIN_ID FROM CDD_MAIN WHERE MAIN_ID='{0}'", mainId);
            string sqlRldc = string.Format(@"SELECT MAIN_ID FROM RLDC_MAIN WHERE MAIN_ID='{0}'", mainId);
            try
            {
                DataSet dsCtnyMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCtny, null);
                DataSet dsFcdsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlFcds, null);
                DataSet dsCdsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCds, null);
                DataSet dsCddMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCdd, null);
                DataSet dsRldcMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlRldc, null);

                dataCount = dsCtnyMainId.Tables[0].Rows.Count + dsFcdsMainId.Tables[0].Rows.Count + dsCdsMainId.Tables[0].Rows.Count + dsCddMainId.Tables[0].Rows.Count + dsRldcMainId.Tables[0].Rows.Count;
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
            string paramIds = string.Empty;
            string sqlMain = string.Format(@"SELECT MAIN_ID,LTGG_ID,LJ_ID,ZCZBZL_ID FROM VIN_INFO WHERE VIN='{0}'", vin);
            try
            {
                DataSet dsMainId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlMain, null);
                if (dsMainId != null && dsMainId.Tables[0].Rows.Count > 0)
                {
                    paramIds = Convert.ToString(dsMainId.Tables[0].Rows[0]["MAIN_ID"]) + "," +
                                Convert.ToString(dsMainId.Tables[0].Rows[0]["LTGG_ID"]) + "," +
                                Convert.ToString(dsMainId.Tables[0].Rows[0]["LJ_ID"]) + "," +
                                Convert.ToString(dsMainId.Tables[0].Rows[0]["ZCZBZL_ID"]);
                }
                else
                {
                    paramIds = ",,,";
                }
            }
            catch (Exception)
            {
            }
            return paramIds;
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
                        mainIdList.Add(dt.Rows[i]["CLXH"].ToString());
                    }
                }
            }
            return mainIdList;
        }

        public List<string> GetMainParamIdFromControl(GridView gv, DataTable dt, string paramId)
        {
            List<string> mainParamIdList = new List<string>();

            gv.PostEditor();

            if (dt != null)
            {
                DataRow[] drVinArr = dt.Select("check=True");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((bool)dt.Rows[i]["check"])
                    {
                        mainParamIdList.Add(dt.Rows[i]["MAIN_ID"].ToString() + "," + dt.Rows[i][paramId].ToString());
                    }
                }
            }
            return mainParamIdList;
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

            //string Jkqczjxs = Convert.ToString(r["JKQCZJXS"]);
            //string Qcscqy = Convert.ToString(r["QCSCQY"]);

            // 汽车生产企业
            //if (string.IsNullOrEmpty(Qcscqy))
            //{
            //    message += "汽车生产企业不能为空!\r\n";
            //}

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

            //// 整车整备质量
            //string Zczbzl = Convert.ToString(r["ZCZBZL"]);
            //message += this.VerifyRequired("整车整备质量", Zczbzl);
            //if (!this.VerifyParamFormat(Zczbzl, ','))
            //{
            //    message += "整车整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格\r\n";
            //}

            // 最高车速
            string Zgcs = Convert.ToString(r["ZGCS"]);
            message += this.VerifyRequired("最高车速", Zgcs);
            if (!this.VerifyParamFormat(Zgcs, ','))
            {
                message += "最高车速应填写整数，多个数值应以半角“,”隔开，中间不留空格\r\n";
            }

            //// 轮胎规格
            //string Ltgg = Convert.ToString(r["LTGG"]);
            //message += this.VerifyRequired("轮胎规格", Ltgg);
            //message += this.VerifyStrLen("轮胎规格", Ltgg, 200);
            //message += this.VerifyLtgg(Ltgg);
            //// 前后轮距相同只填写一个型号数据即可，不同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)

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
            //message += this.VerifyZdsjzzl(Zdsjzzl, Zczbzl, Edzk);
            message += this.VerifyInt("最大设计总质量", Zdsjzzl);

            // 额定载客
            message += this.VerifyRequired("额定载客", Edzk);
            message += this.VerifyInt("额定载客", Edzk);

            //// 轮距（前/后）
            //string Lj = Convert.ToString(r["LJ"]);
            //message += this.VerifyRequired("轮距（前/后）", Lj);
            //if (!this.VerifyParamFormat(Lj, '/') && Lj.IndexOf('/') < 0)
            //{
            //    message += "轮距（前/后）应填写整数，前后轮距，中间用”/”隔开\r\n";
            //}

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
        /// 验证轮胎规格单行数据
        /// </summary>
        /// <param name="drLtgg">轮胎规格数据</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        private string VerifyLtggData(DataRow drLtgg, string importType)
        {
            string message = string.Empty;
            string mainId = Convert.ToString(drLtgg["MAIN_ID"]);
            string ltggId = Convert.ToString(drLtgg["LTGG_ID"]);

            // 校验该数据是否已在数据库中存在
            message += this.VerifyIsParamExist(mainId, ltggId, LTGG, importType);

            string ltgg = Convert.ToString(drLtgg["LTGG"]);
            if (string.IsNullOrEmpty(ltgg))
            {
                message += string.Format("车型参数编号：{0}-轮胎规格编号：{1} 对应的轮胎规格不能为空\r\n", mainId, ltggId);
            }
            else
            {
                //drLtgg["LTGG"] = ltgg.Replace(" ", "");
            }

            return message;
        }


        /// <summary>
        /// 验证轮距单行数据
        /// </summary>
        /// <param name="drLj">轮距数据</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        private string VerifyLjData(DataRow drLj, string importType)
        {
            string message = string.Empty;
            string mainId = Convert.ToString(drLj["MAIN_ID"]);
            string ljId = Convert.ToString(drLj["LJ_ID"]);

            // 校验该数据是否已在数据库中存在
            message += this.VerifyIsParamExist(mainId, ljId, LJ, importType);

            string lj = Convert.ToString(drLj["LJ"]);
            if (string.IsNullOrEmpty(lj))
            {
                message += string.Format("车型参数编号：{0}-轮距编号：{1} 对应的轮距不能为空\r\n", mainId, ljId);
            }
            else
            {
                drLj["LJ"] = lj.Replace(" ", "");
                if (lj.IndexOf('/') < 0 && !this.VerifyParamFormat(lj, '/'))
                {
                    message += "轮距（前/后）应填写整数，前后轮距，中间用”/”隔开\r\n";
                }
            }

            return message;
        }

        /// <summary>
        /// 验证整车整备质量单行数据
        /// </summary>
        /// <param name="drLj">轮距数据</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        private string VerifyZczbzlData(DataRow drZczbzl, string importType)
        {
            string message = string.Empty;
            string mainId = Convert.ToString(drZczbzl["MAIN_ID"]);
            string zczbzlId = Convert.ToString(drZczbzl["ZCZBZL_ID"]);

            // 校验该数据是否已在数据库中存在
            message += this.VerifyIsParamExist(mainId, zczbzlId, ZCZBZL, importType);

            string zczbzl = Convert.ToString(drZczbzl["ZCZBZL"]);
            if (string.IsNullOrEmpty(zczbzl))
            {
                message += string.Format("车型参数编号：{0}-轮距编号：{1} 对应的轮距不能为空\r\n", mainId, zczbzlId);
            }
            else
            {
                if (!this.VerifyParamFormat(zczbzl, ','))
                {
                    message += "整车整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格\r\n";
                }
            }

            return message;
        }

        /// <summary>
        /// 校验单独导入的数据是否已在数据库中存在
        /// </summary>
        /// <param name="cpos">cpos编号</param>
        /// <param name="importType">操作类型：“IMPORT”表示新导入；“UPDATE”表示修改</param>
        /// <returns></returns>
        protected string VerifyIsParamExist(string mainId, string paramId, string paramName, string importType)
        {
            int dataCount = this.GetParamData(mainId, paramId, paramName);

            if (importType == "IMPORT")
            {
                if (dataCount > 0)
                {
                    return string.Format("{0}-{1}：该数据已经导入，请勿重复导入\r\n", mainId, paramId);
                }
            }
            else if (importType == "UPDATE")
            {
                if (dataCount < 1)
                {
                    return string.Format("{0}-{1}：该数据不存在，请直接导入\r\n", mainId, paramId);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取已经导入的轮胎规格,用于导入判断
        /// </summary>
        public int GetParamData(string mainId, string paramId, string paramName)
        {
            int dataCount;
            string paramTable = string.Empty;
            string paramField = string.Empty;

            switch (paramName)
            {
                case LTGG:
                    paramTable = "MAIN_LTGG";
                    paramField = "LTGG_ID";
                    break;
                case LJ:
                    paramTable = "MAIN_LJ";
                    paramField = "LJ_ID";
                    break;
                case ZCZBZL:
                    paramTable = "MAIN_ZCZBZL";
                    paramField = "ZCZBZL_ID";
                    break;
                default: break;
            }

            string sqlParam = string.Format(@"SELECT MAIN_ID FROM {0} WHERE MAIN_ID='{1}' AND {2}='{3}'", paramTable, mainId, paramField, paramId);
            try
            {
                DataSet dsParam = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlParam, null);
                dataCount = dsParam.Tables[0].Rows.Count;
            }
            catch (Exception)
            {
                dataCount = 0;
            }
            return dataCount;
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
            strAdd.Append("SELECT * FROM FC_CLJBXX WHERE VIN in(");
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
                                    string sql = "UPDATE FC_CLJBXX SET CLZZRQ='" + clzzrqDate + "', UPLOADDEADLINE='" + uploadDeadlineDate + "'" + statuswhere + "  WHERE VIN='" + r["VIN"] + "'";
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
            string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
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
                    OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [" + sheet + "]", strConn);
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
        /// 查询EXCEL中VIN状态
        /// </summary>
        /// <param name="vin">VIN码</param>
        /// <returns></returns>
        private int SearchStatus(string vin)
        {
            string sql = "select status from FC_CLJBXX WHERE VIN='" + vin + "'";
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

        //导出之前进行判断显示相应窗口
        public void ExportExcel(DataTable source, IWin32Window owner, String exportType)
        {
            if (source == null)
            {
                MessageBox.Show("请首先查询数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                if (source.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Title = "导出Excel";
                    saveFileDialog.Filter = "Excel文件(*.xls)|*.xls|xlsx文件|*.xlsx";
                    DialogResult dialogResult = saveFileDialog.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        ExportExcel(saveFileDialog.FileName, source, exportType);
                        MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    }
                }
            }
        }

        //判断导出类型
        public void ExportExcel(string saveName, DataTable dtSource, String exportType)
        {
            Dictionary<String, String> header = null;
            Microsoft.Office.Interop.Excel.XlFileFormat fileFormat = Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8;
            switch (exportType)
            {
                case CTNYExport:
                    header = this.dictCTNY;
                    break;
                case CDDExport:
                    header = this.dictCDD;
                    break;
                case CDSExport:
                    header = this.dictCDSHHDL;
                    break;
                case FCDSExport:
                    header = this.dictFCDSHHDL;
                    break;
                case RLDCExport:
                    header = this.dictRLDC;
                    break;
                case LTGG:
                    header = this.dictLTGG;
                    break;
                case LJ:
                    header = this.dictLJ;
                    break;
                case ZCZBZL:
                    header = this.dictZCZBZL;
                    break;
                case CLMXExport:
                    header = this.dictCLMXExport;
                    break;
                case HZBGExport:
                    header = this.dictHZBGExport;
                    break;
                case HZBGExport_New:
                    header = this.dictHZBGExport_New;
                    break;
            }
            if (saveName.EndsWith("xlsx", StringComparison.OrdinalIgnoreCase))
            {
                fileFormat = Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;
            }
            else if (saveName.EndsWith("xls", StringComparison.OrdinalIgnoreCase))
            {
                fileFormat = Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8;
            }
            ExportExcel(saveName, dtSource, header, fileFormat);
        }

        public void ExportExcel(string saveName, DataTable dtSource, Dictionary<String, String> header, Microsoft.Office.Interop.Excel.XlFileFormat fileFormat)
        {
            excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
            excelApp.Visible = false;

            try
            {
                //将表头字段从英文名转换为中文名，并删除没有找到的列
                DataTable dt = dtSource.Copy();
                dt = this.E2C(header, dt, true);
                int rowCount = dt.Rows.Count;
                int colCount = dt.Columns.Count;

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
                            datas[0, i] = dt.Columns[i].ColumnName;//表头信息   
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
                        fchR.NumberFormatLocal = "@";
                        fchR.Value = datas;
                    }
                }
                else
                {
                    object[,] dataArray = new object[rowCount + 1, colCount];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dataArray[0, i] = dt.Columns[i].ColumnName;
                    }

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            dataArray[i + 1, j] = dt.Rows[i][j];
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
                    case "VIN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "VIN");
                        break;
                    case "MAIN_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "参数编号");
                        break;
                    case "CLXH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆型号");
                        break;
                    case "CLZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆种类");
                        break;
                    case "RLLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料类型");
                        break;
                    case "CLZZRQ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "通关日期");
                        break;
                    case "SQGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "市区工况燃料消耗量");
                        break;
                    case "SJGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "市郊工况燃料消耗量");
                        break;
                    case "ZHGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况燃料消耗量");
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
                //excelApp.Workbooks.Close();
                //excelApp.Workbooks.Application.Quit();
                //excelApp.Application.Quit();
                excelApp.Quit();
            }
            catch { }
            finally
            {
                try
                {
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp.Workbooks);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp.Application);
                    //System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    //excelApp = null;
                    Kill(excelApp);
                }
                catch { }
                try
                {
                    //清理垃圾进程   
                    this.killProcessThread();
                }
                catch { }
                //GC.Collect();
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

        // 验证车型参数参数编码是否已经存在
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
                            message += VerifyFloat("动力蓄电池组比能量", Convert.ToString(r[code]));
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
                            message += VerifyFloat("动力蓄电池组比能量", Convert.ToString(r[code]));
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
                            message += VerifyFloat("纯电动模式下综合工况续驶里程", Convert.ToString(r[code]));
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
                            message += VerifyFloat("综合工况续驶里程", Convert.ToString(r[code]));
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
