using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;
using FuelDataModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid;
using System.Text;
using FuelDataSysClient.Properties;

namespace FuelDataSysClient.Tool.Tool_JBLH
{
    public class JaguarUtils
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;

        // 读取配置文件信息
        string path = System.Windows.Forms.Application.StartupPath + Settings.Default["ExcelHeaderTemplate_JBLH"];

        private static Dictionary<string, string> FILE_NAME = new Dictionary<string, string>() 
        {
            {"VIN", "VIN*" },
            {"COC", "A*" },
            {"UPDATE_COC", "UPDATE_COC*"},
            {"F_VIN", "已导入VIN"},
            {"F_COC", "已导入COC"},
            {"U_COC", "已修改COC"},
            {"FcdsCOC", "非插电式混合动力车COC"},
            //{"CtnyCOC", "sheet1"},
            {"COCDATA", "sheet1"},
            {"FcdsData", "混合动力车数据"},
            {"FcdsFlag", "电"},
        };
        private static Dictionary<string, string> FUEL_DATA = new Dictionary<string, string>() 
        {
            {"cocNo", "A5"},
            {"clxh", "A10"},
            {"tymc", "A8"},
            {"clzl" , "A14"},
            {"yyc", "A14"},
            {"qcscqy", "A17"},
            {"rllx", "G44"},
            {"zwps" , "G52"},
            {"zczbzl", "C36"},
            {"zdsjzzl", "G36"},
            {"zgcs" , "C54"},
            {"edzk", "C52"},
            {"ltgg", "C48"},
            {"lj" , "C32"},
            {"zj", "G31"},
            {"qdxs", "C31"},
            {"jybgbh", "C76"},
            {"jyjgmc", "C75"},
            {"fdjxh" , "G41"},
            {"qgs", "C43"},
            {"pl", "C44"},
            {"edgl" , "C45"},
            {"jgl", "C45"},
            {"bsqxs", "G46"},
            {"bsqdws" , "C47"},
            {"qcjnjs", "C73"},
            {"qtxx", "C74"},
            {"sqgkrlxhl" , "G66"},
            {"sjgkrlxhl", "G67"},
            {"zhgkrlxhl", "G68"},
            {"zhgkco2pfl" , "E68"},
            {"fdjscc", "C41"}
        };

        private List<string> listHoliday; // 节假日数据
        string strCon = AccessHelper.conn;

        private const string VIN = "VIN";
        Dictionary<string, string> dictVin; //存放列头转换模板（VIN）

        // VIN excel文件名称的开头
        private string vinFileName = FILE_NAME["VIN"].ToString();

        public string VinFileName
        {
            get { return vinFileName; }
        }

        // COC excel文件名称的开头
        private string cocFileName = FILE_NAME["COC"].ToString();

        public string CocFileName
        {
            get { return cocFileName; }
        }

        // 待修改COC excel文件名称的开头
        private string updateCocFileName = FILE_NAME["UPDATE_COC"].ToString();

        public string UpdateCocFileName
        {
            get { return updateCocFileName; }
        }

        private string cocDataName = FILE_NAME["COCDATA"].ToString();

        public string CocDataName
        {
            get { return cocDataName; }
        }

        private string fcdsSheetName = FILE_NAME["FcdsCOC"].ToString();

        public string FcdsSheetName
        {
            get { return fcdsSheetName; }
        }

        private string fcdsDataSheetName = FILE_NAME["FcdsData"].ToString();

        public string FcdsDataSheetName
        {
            get { return fcdsDataSheetName; }
        }

        private string fcdsFlag = FILE_NAME["FcdsFlag"].ToString();

        public string FcdsFlag
        {
            get { return fcdsFlag; }
        }

        public JaguarUtils()
        {

        }

        /// <summary>
        /// 读取导入模板和数据库字段对性关系，模板保存在根目录的ExcelHeaderTemplate文件夹下
        /// </summary>
        /// <param name="fileName"></param>
        public DataSet ReadTemplateExcel(string fileName)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("SELECT * FROM [VIN$]", strConn);
                oada.Fill(ds, VIN);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /// <summary>
        /// 读取模板表头和数据库字段对应关系模板
        /// </summary>
        /// <param name="filePath"></param>
        private void ReadTemplate(string filePath)
        {
            DataSet ds = this.ReadTemplateExcel(filePath);
            dictVin = new Dictionary<string, string>();

            foreach (DataRow r in ds.Tables[VIN].Rows)
            {
                dictVin.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
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
        public System.Data.DataTable ReadVINExcel(string fileName)
        {
            string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            System.Data.DataTable dtVin = new System.Data.DataTable();

            try
            {
                OleDbDataAdapter oada = new OleDbDataAdapter("select * from [Sheet1$]", strConn);
                oada.Fill(ds, VIN);

                // 读取列名对应模板
                this.ReadTemplate(this.path);

                if (ds != null)
                {
                    dtVin = this.D2D(ds.Tables[VIN]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dtVin;
        }

        /// <summary>
        /// 转换表头
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private System.Data.DataTable D2D(System.Data.DataTable dt)
        {
            string fiedName = "车辆识别码(VIN)(17位)"; // "VIN"是VIN表中的字段,用于验证某行是否为空数据
            System.Data.DataTable d = new System.Data.DataTable();

            for (int i = 0; i < dt.Columns.Count; )
            {
                DataColumn c = dt.Columns[i];

                if (dt.TableName == VIN)
                {
                    if (!dictVin.ContainsKey(c.ColumnName.Trim()))
                    {
                        dt.Columns.Remove(c);
                        continue;
                    }
                    d.Columns.Add(dictVin[c.ColumnName.Trim()]);
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
        /// 转移已用完的文件
        /// </summary>
        /// <param name="srcFileName">源文件路径</param>
        /// <param name="folderPath">目的文件夹路径</param>
        /// <param name="fileType">文件类型</param>
        public void MoveFinishedFile(string srcFileName, string folderPath, string fileType)
        {
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
                throw ex;
            }

        }

        /// <summary>
        /// 保存VIN信息
        /// </summary>
        /// <param name="ds"></param>
        public string SaveVinInfo(System.Data.DataTable dt, string DataType, out int result)
        {
            int succImCount = 0;   //成功导入的数量
            int failCount = 0;  //导入失败的数量
            int totalCount = dt.Rows.Count;
            string msg = string.Empty;
            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();
            OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务

            ProcessForm pf = new ProcessForm();
            pf.Text = "正在导入，请稍候";
            pf.Show();
            pf.TotalMax = (int)Math.Ceiling((decimal)dt.Rows.Count / (decimal)1);
            pf.ShowProcessBar();

            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string vin = dr["VIN"] == null ? "" : dr["VIN"].ToString().Trim().ToUpper();
                    string hgspbm = string.Empty;
                    try
                    {
                        hgspbm = dr["HGSPBM"] == null ? "" : dr["HGSPBM"].ToString().Trim().ToUpper();
                    }
                    catch
                    {
                    }
                    if (!string.IsNullOrEmpty(vin))
                    {
                        string sqlDel = "DELETE FROM VIN_INFO WHERE VIN = '" + vin + "'";
                        AccessHelper.ExecuteNonQuery(tra, sqlDel, null);

                        string sqlStr = @"INSERT INTO VIN_INFO(VIN,HGSPBM,COC_ID,GH_FDJXLH,CLZZRQ,TGRQ,DATA_TYPE) Values (@VIN, @COC_ID,@HGSPBM,@GH_FDJXLH,@CLZZRQ,@TGRQ,@DATA_TYPE)";

                        OleDbParameter[] paramList = { 
                                         new OleDbParameter("@VIN",vin),
                                         new OleDbParameter("@HGSPBM",hgspbm),
                                         new OleDbParameter("@COC_ID",dr["COC_ID"].ToString().Trim()),
                                         new OleDbParameter("@GH_FDJXLH",dr["GH_FDJXLH"].ToString().Trim()),
                                         new OleDbParameter("@CLZZRQ",Convert.ToDateTime(dr["CLZZRQ"].ToString().Trim())),
                                         new OleDbParameter("@TGRQ",Convert.ToDateTime(dr["TGRQ"].ToString().Trim())),
                                         new OleDbParameter("@DATA_TYPE",DataType)
                                      };
                        succImCount += AccessHelper.ExecuteNonQuery(tra, sqlStr, paramList);

                        pf.progressBarControl1.PerformStep();
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                tra.Commit();
            }
            catch (Exception ex)
            {
                tra.Rollback();
                msg = ex.Message;
                failCount = totalCount;
                //throw ex;
            }
            finally
            {
                con.Close();
                pf.Close();
            }

            string msgSummary = string.Format("共{0}条数据：\r\n \t{1}条导入成功 \r\n \t{2}条导入失败\r\n",
                                totalCount, succImCount, failCount);
            result = failCount;
            return msg + msgSummary;
        }

        /// <summary>
        /// 导入VIN信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public string ImportVinData(string fileName, string folderName, string DataType)
        {
            string rtnMsg = string.Empty;
            System.Data.DataTable dtVin = this.ReadVINExcel(fileName);
            if (dtVin != null)
            {
                int result = 0;
                rtnMsg += this.SaveVinInfo(dtVin, DataType, out result);
                if (result == 0)
                {
                    this.MoveFinishedFile(fileName, folderName, "F_VIN");
                    rtnMsg = "\r\n" + fileName + "导入成功";
                }
                else
                {
                    rtnMsg = fileName + "导入失败;\r\n";
                }
            }
            else
            {
                rtnMsg = fileName + "导入失败;\r\n";
            }

            return rtnMsg;
        }

        /// <summary>
        /// 读传统能源COC信息
        /// </summary>
        /// <param name="fileName"></param>
        public string ReadCtnyCOCExcel(string fileName, string folderName, string importType, List<string> cocIdList, string promptMsg)
        {
            //string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            //DataSet ds = new DataSet();

            Microsoft.Office.Interop.Excel.Application xApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook xBook = xApp.Workbooks.Open(fileName,
                        Type.Missing, Missing.Value, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value);
            Worksheet xSheet = null;
            if (promptMsg == "导入")
            {
                xSheet = (Worksheet)xBook.Sheets[this.CocDataName];
            }
            else if (promptMsg == "修改")
            {
                //xSheet = (Worksheet)xBook.Sheets[this.FcdsSheetName];
                xSheet = (Worksheet)xBook.Sheets[this.CocDataName];
            }
            string rllx = string.Empty;
            string saveMsg = string.Empty;

            Range range = null;

            VehicleBasicInfo basicInfo = new VehicleBasicInfo();
            CtnyRllx ctnyInfo = new CtnyRllx();
            FcdsRllx fcdsInfo = new FcdsRllx();
            string cocId = string.Empty;
            string readMsg = string.Empty;
            string paramMsg = string.Empty;

            try
            {
                #region 读取传统能源与非插电式混合动力共有内容

                range = xSheet.get_Range(FUEL_DATA["cocNo"], Type.Missing);
                cocId = this.FormatCocId(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("COC编号", cocId);

                range = xSheet.get_Range(FUEL_DATA["clxh"], Type.Missing);
                basicInfo.Clxh = this.FormatClxh(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("产品型号", basicInfo.Clxh);

                range = xSheet.get_Range(FUEL_DATA["tymc"], Type.Missing);
                basicInfo.Tymc = this.FormatTymc(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("通用名称", basicInfo.Tymc);

                range = xSheet.get_Range(FUEL_DATA["clzl"], Type.Missing);
                basicInfo.Clzl = this.FormatClzl(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("车辆类型", basicInfo.Clzl);

                range = xSheet.get_Range(FUEL_DATA["yyc"], Type.Missing);
                basicInfo.Yyc = this.FormatYyc(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("越野车", basicInfo.Yyc);

                range = xSheet.get_Range(FUEL_DATA["qcscqy"], Type.Missing);
                basicInfo.Qcscqy = this.FormatQcscqy(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("乘用车生产企业", basicInfo.Qcscqy);

                range = xSheet.get_Range(FUEL_DATA["rllx"], Type.Missing);
                basicInfo.Rllx = this.FormatRllx(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("燃料种类", basicInfo.Rllx);

                range = xSheet.get_Range(FUEL_DATA["zwps"], Type.Missing);
                basicInfo.Zwps = this.FormatZwps(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("座椅排数", basicInfo.Zwps);

                range = xSheet.get_Range(FUEL_DATA["zczbzl"], Type.Missing);
                basicInfo.Zczbzl = this.FormatZczbzl(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("整备质量", basicInfo.Zczbzl);

                range = xSheet.get_Range(FUEL_DATA["zdsjzzl"], Type.Missing);
                basicInfo.Zdsjzzl = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("总质量", basicInfo.Zdsjzzl);

                range = xSheet.get_Range(FUEL_DATA["zgcs"], Type.Missing);
                basicInfo.Zgcs = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("最高车速", basicInfo.Zgcs);

                range = xSheet.get_Range(FUEL_DATA["edzk"], Type.Missing);
                basicInfo.Edzk = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("额定载客", basicInfo.Edzk);

                range = xSheet.get_Range(FUEL_DATA["ltgg"], Type.Missing);
                basicInfo.Ltgg = this.FormatLtgg(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("轮胎规格", basicInfo.Ltgg);

                range = xSheet.get_Range(FUEL_DATA["lj"], Type.Missing);
                basicInfo.Lj = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("轮距", basicInfo.Lj);

                range = xSheet.get_Range(FUEL_DATA["zj"], Type.Missing);
                basicInfo.Zj = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("轴距", basicInfo.Zj);

                range = xSheet.get_Range(FUEL_DATA["qdxs"], Type.Missing);
                basicInfo.Qdxs = this.FormatQdxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("驱动型式", basicInfo.Qdxs);

                range = xSheet.get_Range(FUEL_DATA["jybgbh"], Type.Missing);
                basicInfo.Jybgbh = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("检验报告编号", basicInfo.Jybgbh);

                range = xSheet.get_Range(FUEL_DATA["jyjgmc"], Type.Missing);
                basicInfo.Jyjgmc = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("检验机构名称", basicInfo.Jyjgmc);

                #endregion

                #region 传统能源参数信息

                if (basicInfo.Rllx.IndexOf(FcdsFlag) < 0)
                {
                    rllx = "CTNY";

                    range = xSheet.get_Range(FUEL_DATA["fdjxh"], Type.Missing);
                    ctnyInfo.Fdjxh = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                    paramMsg += Utils.VerifyRequired("发动机型号", ctnyInfo.Fdjxh);

                    range = xSheet.get_Range(FUEL_DATA["qgs"], Type.Missing);
                    ctnyInfo.Qgs = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                    paramMsg += Utils.VerifyRequired("发动机气缸数目", ctnyInfo.Qgs);

                    range = xSheet.get_Range(FUEL_DATA["pl"], Type.Missing);
                    ctnyInfo.Pl = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
                    paramMsg += Utils.VerifyRequired("发动机排量", ctnyInfo.Pl);

                    range = xSheet.get_Range(FUEL_DATA["edgl"], Type.Missing);
                    ctnyInfo.Edgl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("发动机功率", ctnyInfo.Edgl);

                    range = xSheet.get_Range(FUEL_DATA["jgl"], Type.Missing);
                    ctnyInfo.Jgl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());

                    range = xSheet.get_Range(FUEL_DATA["bsqxs"], Type.Missing);
                    ctnyInfo.Bsqxs = this.FormatBsqxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("变速器型式", ctnyInfo.Bsqxs);

                    range = xSheet.get_Range(FUEL_DATA["bsqdws"], Type.Missing);
                    ctnyInfo.Bsqdws = this.FormatBsqdws(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("变速器档位数", ctnyInfo.Bsqdws);

                    range = xSheet.get_Range(FUEL_DATA["qcjnjs"], Type.Missing);
                    ctnyInfo.Qcjnjs = range.Value2 == null ? "" : range.Value2.ToString().Trim();

                    range = xSheet.get_Range(FUEL_DATA["qtxx"], Type.Missing);
                    ctnyInfo.Qtxx = range.Value2 == null ? "" : range.Value2.ToString().Trim();

                    range = xSheet.get_Range(FUEL_DATA["sqgkrlxhl"], Type.Missing);
                    ctnyInfo.Sqgkrlxhl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("燃料消耗量（市区）", ctnyInfo.Sqgkrlxhl);

                    range = xSheet.get_Range(FUEL_DATA["sjgkrlxhl"], Type.Missing);
                    ctnyInfo.Sjgkrlxhl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("燃料消耗量（市郊）", ctnyInfo.Sjgkrlxhl);

                    range = xSheet.get_Range(FUEL_DATA["zhgkrlxhl"], Type.Missing);
                    ctnyInfo.Zhgkrlxhl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("燃料消耗量（综合）", ctnyInfo.Zhgkrlxhl);

                    range = xSheet.get_Range(FUEL_DATA["zhgkco2pfl"], Type.Missing);
                    ctnyInfo.Zhgkco2pfl = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
                    paramMsg += Utils.VerifyRequired("CO2排放量（综合）", ctnyInfo.Zhgkco2pfl);

                    range = xSheet.get_Range(FUEL_DATA["fdjscc"], Type.Missing);
                    ctnyInfo.Gh_Fdjscc = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                    paramMsg += Utils.VerifyRequired("发动机生产厂", ctnyInfo.Gh_Fdjscc);
                }

                #endregion

                #region 非插电式参数信息

                if (basicInfo.Rllx.IndexOf(FcdsFlag) > -1)
                {
                    rllx = "FCDS";

                    range = xSheet.get_Range(FUEL_DATA["fdjxh"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_FDJXH = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                    paramMsg += Utils.VerifyRequired("发动机型号", fcdsInfo.FCDS_HHDL_FDJXH);

                    range = xSheet.get_Range(FUEL_DATA["qgs"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_QGS = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                    paramMsg += Utils.VerifyRequired("发动机气缸数目", fcdsInfo.FCDS_HHDL_QGS);

                    range = xSheet.get_Range(FUEL_DATA["pl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_PL = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
                    paramMsg += Utils.VerifyRequired("发动机排量", fcdsInfo.FCDS_HHDL_PL);

                    range = xSheet.get_Range(FUEL_DATA["edgl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_EDGL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("发动机功率", fcdsInfo.FCDS_HHDL_EDGL);

                    range = xSheet.get_Range(FUEL_DATA["jgl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_JGL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());

                    range = xSheet.get_Range(FUEL_DATA["bsqxs"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_BSQXS = this.FormatBsqxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("变速器型式", fcdsInfo.FCDS_HHDL_BSQXS);

                    range = xSheet.get_Range(FUEL_DATA["bsqdws"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_BSQDWS = this.FormatBsqdws(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("变速器档位数", fcdsInfo.FCDS_HHDL_BSQDWS);


                    range = xSheet.get_Range(FUEL_DATA["qtxx"], Type.Missing);
                    fcdsInfo.CT_QTXX = range.Value2 == null ? "" : range.Value2.ToString().Trim();

                    range = xSheet.get_Range(FUEL_DATA["sqgkrlxhl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_SQGKRLXHL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("燃料消耗量（市区）", fcdsInfo.FCDS_HHDL_SQGKRLXHL);

                    range = xSheet.get_Range(FUEL_DATA["sjgkrlxhl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_SJGKRLXHL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("燃料消耗量（市郊）", fcdsInfo.FCDS_HHDL_SJGKRLXHL);

                    range = xSheet.get_Range(FUEL_DATA["zhgkrlxhl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_ZHGKRLXHL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                    paramMsg += Utils.VerifyRequired("燃料消耗量（综合）", fcdsInfo.FCDS_HHDL_ZHGKRLXHL);

                    range = xSheet.get_Range(FUEL_DATA["zhgkco2pfl"], Type.Missing);
                    fcdsInfo.FCDS_HHDL_ZHKGCO2PL = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
                    paramMsg += Utils.VerifyRequired("CO2排放量（综合）", fcdsInfo.FCDS_HHDL_ZHKGCO2PL);

                    range = xSheet.get_Range(FUEL_DATA["fdjscc"], Type.Missing);
                    fcdsInfo.FCDS_GH_FDJSCC = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                    paramMsg += Utils.VerifyRequired("发动机生产厂", fcdsInfo.FCDS_GH_FDJSCC);

                    DataSet ds = this.ReadFcdsConst();
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        System.Data.DataTable dt = ds.Tables[0];
                        DataRow r = dt.Rows[0];

                        fcdsInfo.FCDS_HHDL_HHDLJGXS = Convert.ToString(r["FCDS_HHDL_HHDLJGXS"]);
                        paramMsg += Utils.VerifyRequired("混合动力结构型式", fcdsInfo.FCDS_HHDL_HHDLJGXS);

                        fcdsInfo.FCDS_HHDL_XSMSSDXZGN = Convert.ToString(r["FCDS_HHDL_XSMSSDXZGN"]);
                        paramMsg += Utils.VerifyRequired("是否具有行驶模式手动选择功能", fcdsInfo.FCDS_HHDL_XSMSSDXZGN);

                        fcdsInfo.FCDS_HHDL_DLXDCZZL = Convert.ToString(r["FCDS_HHDL_DLXDCZZL"]);
                        paramMsg += Utils.VerifyRequired("电动汽车储能装置种类", fcdsInfo.FCDS_HHDL_DLXDCZZL);

                        fcdsInfo.FCDS_HHDL_DLXDCZZNL = Convert.ToString(r["FCDS_HHDL_DLXDCZZNL"]);
                        paramMsg += Utils.VerifyRequired("储能装置总储电量", fcdsInfo.FCDS_HHDL_DLXDCZZNL);

                        fcdsInfo.FCDS_HHDL_DLXDCBNL = Convert.ToString(r["FCDS_HHDL_DLXDCBNL"]);
                        paramMsg += Utils.VerifyRequired("动力电池系统能量密度", fcdsInfo.FCDS_HHDL_DLXDCBNL);

                        fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC = Convert.ToString(r["FCDS_HHDL_CDDMSXZHGKXSLC"]);
                        if (!string.IsNullOrEmpty(fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC))
                            paramMsg += Utils.VerifyInt(fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC) ? "" : "纯电驱动模式续驶里程（工况法）为整数";

                        fcdsInfo.FCDS_HHDL_CDDMSXZGCS = Convert.ToString(r["FCDS_HHDL_CDDMSXZGCS"]);
                        if (!string.IsNullOrEmpty(fcdsInfo.FCDS_HHDL_CDDMSXZGCS))
                            paramMsg += Utils.VerifyInt(fcdsInfo.FCDS_HHDL_CDDMSXZGCS) ? "" : "纯电动模式下1km最高车速为整数";

                        fcdsInfo.FCDS_HHDL_DLXDCZBCDY = Convert.ToString(r["FCDS_HHDL_DLXDCZBCDY"]);
                        paramMsg += Utils.VerifyRequired("储能装置总成标称电压", fcdsInfo.FCDS_HHDL_DLXDCZBCDY);

                        fcdsInfo.FCDS_HHDL_QDDJLX = Convert.ToString(r["FCDS_HHDL_QDDJLX"]);
                        paramMsg += Utils.VerifyRequired("驱动电机类型", fcdsInfo.FCDS_HHDL_QDDJLX);

                        fcdsInfo.FCDS_HHDL_HHDLZDDGLB = Convert.ToString(r["FCDS_HHDL_HHDLZDDGLB"]);
                        paramMsg += Utils.VerifyRequired("混合动力汽车电功率比", fcdsInfo.FCDS_HHDL_HHDLZDDGLB);

                        fcdsInfo.FCDS_HHDL_QDDJFZNJ = Convert.ToString(r["FCDS_HHDL_QDDJFZNJ"]);
                        paramMsg += Utils.VerifyRequired("驱动电机峰值转矩", fcdsInfo.FCDS_HHDL_QDDJFZNJ);

                        fcdsInfo.FCDS_HHDL_QDDJEDGL = Convert.ToString(r["FCDS_HHDL_QDDJEDGL"]);
                        paramMsg += Utils.VerifyRequired("驱动电机额定功率", fcdsInfo.FCDS_HHDL_QDDJEDGL);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                readMsg = ex.Message + "\r\n";
            }
            finally
            {
                xBook.Close();

                Kill(xApp);
                //KillProcess("EXCEL");
            }

            if (!string.IsNullOrEmpty(readMsg))
            {
                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, readMsg);
            }

            if (!string.IsNullOrEmpty(paramMsg))
            {
                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, paramMsg);
            }


            if (importType == "IMPORT")
            {
                if (rllx == "CTNY")
                {
                    saveMsg = this.SaveCtnyCocInfo(cocId, basicInfo, ctnyInfo);
                }
                if (rllx == "FCDS")
                {
                    saveMsg = this.SaveFcdsCocInfo(cocId, basicInfo, fcdsInfo);
                }
            }
            else if (importType == "UPDATE")
            {
                if (rllx == "CTNY")
                {
                    saveMsg = this.UpdateCtnyCocInfo(cocId, basicInfo, ctnyInfo, cocIdList);
                }
                if (rllx == "FCDS")
                {
                    saveMsg = this.UpdateFcdsCocInfo(cocId, basicInfo, fcdsInfo, cocIdList);
                }
            }

            if (string.IsNullOrEmpty(saveMsg))
            {
                // 移动文件
                string templateFileName = string.Empty;
                if (importType == "IMPORT")
                {
                    templateFileName = "F_COC";
                }
                else if (importType == "UPDATE")
                {
                    templateFileName = "U_COC";
                }
                this.MoveFinishedFile(fileName, folderName, templateFileName);
            }
            if (string.IsNullOrEmpty(saveMsg))
            {
                return "\r\n" + fileName + " 操作成功\r\n";
            }
            return saveMsg;
        }

        // 获取非插电式混合动力固定参数
        protected DataSet ReadFcdsConst()
        {
            DataSet ds = null;
            string sql = string.Empty;
            try
            {
                sql = @" SELECT 
                                FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC,FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY,
                                FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL,FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB,
                                FCDS_HHDL_QDDJEDGL,FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX,FCDS_HHDL_XSMSSDXZGN
                            FROM FCDS_CONST";

                ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        #region sundongbo
        ///// <summary>
        ///// 读传统能源COC信息
        ///// </summary>
        ///// <param name="fileName"></param>
        //public string ReadCtnyCOCExcel(string fileName, string folderName, string importType, List<string> cocIdList)
        //{
        //    //string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
        //    //DataSet ds = new DataSet();

        //    Microsoft.Office.Interop.Excel.Application xApp = new Microsoft.Office.Interop.Excel.Application();
        //    Workbook xBook = xApp.Workbooks.Open(fileName, 
        //                Type.Missing, Missing.Value, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value);

        //    Worksheet xSheet = null;
        //    string rllx = string.Empty;
        //    string saveMsg = string.Empty;
        //    int sheetCount = xBook.Sheets.Count;
        //    for (int i = 1; i <= sheetCount; i++)
        //    {
        //        xSheet = (Worksheet)xBook.Sheets[i];
        //        if (xSheet.Name == this.FcdsSheetName)
        //        {
        //            #region 非插电式

        //            Range range = null;

        //            VehicleBasicInfo basicInfo = new VehicleBasicInfo();
        //            CtnyRllx ctnyInfo = new CtnyRllx();
        //            FcdsRllx fcdsInfo = new FcdsRllx();
        //            string cocId = string.Empty;
        //            string readMsg = string.Empty;
        //            string paramMsg = string.Empty;

        //            try
        //            {
        //                #region 读取Excel

        //                range = xSheet.get_Range(FUEL_DATA["cocNo"], Type.Missing);
        //                cocId = this.FormatCocId(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("COC编号", cocId);

        //                range = xSheet.get_Range(FUEL_DATA["clxh"], Type.Missing);
        //                basicInfo.Clxh = this.FormatClxh(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("产品型号", basicInfo.Clxh);

        //                range = xSheet.get_Range(FUEL_DATA["tymc"], Type.Missing);
        //                basicInfo.Tymc = this.FormatTymc(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("通用名称", basicInfo.Tymc);

        //                range = xSheet.get_Range(FUEL_DATA["clzl"], Type.Missing);
        //                basicInfo.Clzl = this.FormatClzl(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("车辆类型", basicInfo.Clzl);

        //                range = xSheet.get_Range(FUEL_DATA["yyc"], Type.Missing);
        //                basicInfo.Yyc = this.FormatYyc(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("越野车", basicInfo.Yyc);

        //                range = xSheet.get_Range(FUEL_DATA["qcscqy"], Type.Missing);
        //                basicInfo.Qcscqy = this.FormatQcscqy(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("乘用车生产企业", basicInfo.Qcscqy);

        //                range = xSheet.get_Range(FUEL_DATA["rllx"], Type.Missing);
        //                rllx = basicInfo.Rllx = this.FormatRllx(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料类型", basicInfo.Rllx);

        //                range = xSheet.get_Range(FUEL_DATA["zwps"], Type.Missing);
        //                basicInfo.Zwps = this.FormatZwps(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("座椅排数", basicInfo.Zwps);

        //                range = xSheet.get_Range(FUEL_DATA["zczbzl"], Type.Missing);
        //                basicInfo.Zczbzl = this.FormatZczbzl(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("整备质量", basicInfo.Zczbzl);

        //                range = xSheet.get_Range(FUEL_DATA["zdsjzzl"], Type.Missing);
        //                basicInfo.Zdsjzzl = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("总质量", basicInfo.Zdsjzzl);

        //                range = xSheet.get_Range(FUEL_DATA["zgcs"], Type.Missing);
        //                basicInfo.Zgcs = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("最高车速", basicInfo.Zgcs);

        //                range = xSheet.get_Range(FUEL_DATA["edzk"], Type.Missing);
        //                basicInfo.Edzk = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("额定载客", basicInfo.Edzk);

        //                range = xSheet.get_Range(FUEL_DATA["ltgg"], Type.Missing);
        //                basicInfo.Ltgg = this.FormatLtgg(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("轮胎规格", basicInfo.Ltgg);

        //                range = xSheet.get_Range(FUEL_DATA["lj"], Type.Missing);
        //                basicInfo.Lj = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("轮距", basicInfo.Lj);

        //                range = xSheet.get_Range(FUEL_DATA["zj"], Type.Missing);
        //                basicInfo.Zj = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("轴距", basicInfo.Zj);

        //                range = xSheet.get_Range(FUEL_DATA["qdxs"], Type.Missing);
        //                basicInfo.Qdxs = this.FormatQdxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("驱动型式", basicInfo.Qdxs);

        //                range = xSheet.get_Range(FUEL_DATA["jybgbh"], Type.Missing);
        //                basicInfo.Jybgbh = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("检验报告编号", basicInfo.Jybgbh);

        //                range = xSheet.get_Range(FUEL_DATA["jyjgmc"], Type.Missing);
        //                basicInfo.Jyjgmc = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("检验机构名称", basicInfo.Jyjgmc);

        //                // 非插电式混合动用能源参数信息
        //                range = xSheet.get_Range(FUEL_DATA["fdjxh"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_FDJXH = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("发动机型号", fcdsInfo.FCDS_HHDL_FDJXH);

        //                range = xSheet.get_Range(FUEL_DATA["qgs"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_QGS = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("发动机气缸数目", fcdsInfo.FCDS_HHDL_QGS);

        //                range = xSheet.get_Range(FUEL_DATA["pl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_PL = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
        //                paramMsg += Utils.VerifyRequired("发动机排量", fcdsInfo.FCDS_HHDL_PL);

        //                range = xSheet.get_Range(FUEL_DATA["edgl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_EDGL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("发动机功率", fcdsInfo.FCDS_HHDL_EDGL);

        //                range = xSheet.get_Range(FUEL_DATA["jgl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_JGL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());

        //                range = xSheet.get_Range(FUEL_DATA["bsqxs"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_BSQXS = this.FormatBsqxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("变速器型式", fcdsInfo.FCDS_HHDL_BSQXS);

        //                range = xSheet.get_Range(FUEL_DATA["bsqdws"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_BSQDWS = this.FormatBsqdws(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("变速器档位数", fcdsInfo.FCDS_HHDL_BSQDWS);



        //                range = xSheet.get_Range(FUEL_DATA["qtxx"], Type.Missing);
        //                fcdsInfo.CT_QTXX = range.Value2 == null ? "" : range.Value2.ToString().Trim();

        //                range = xSheet.get_Range(FUEL_DATA["sqgkrlxhl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_SQGKRLXHL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料消耗量（市区）", fcdsInfo.FCDS_HHDL_SQGKRLXHL);

        //                range = xSheet.get_Range(FUEL_DATA["sjgkrlxhl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_SJGKRLXHL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料消耗量（市郊）", fcdsInfo.FCDS_HHDL_SJGKRLXHL);

        //                range = xSheet.get_Range(FUEL_DATA["zhgkrlxhl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_ZHGKRLXHL = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料消耗量（综合）", fcdsInfo.FCDS_HHDL_ZHGKRLXHL);

        //                range = xSheet.get_Range(FUEL_DATA["zhgkco2pfl"], Type.Missing);
        //                fcdsInfo.FCDS_HHDL_ZHKGCO2PL = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
        //                paramMsg += Utils.VerifyRequired("CO2排放量（综合）", fcdsInfo.FCDS_HHDL_ZHKGCO2PL);

        //                range = xSheet.get_Range(FUEL_DATA["fdjscc"], Type.Missing);
        //                fcdsInfo.FCDS_GH_FDJSCC = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("发动机生产厂", fcdsInfo.FCDS_GH_FDJSCC);

        //                #endregion

        //                if (rllx.IndexOf(FcdsFlag) > -1)
        //                {
        //                    DataSet ds = ReadExcel(fileName, this.FcdsDataSheetName);
        //                    if (ds != null && ds.Tables[0].Rows.Count > 0)
        //                    {
        //                        System.Data.DataTable dt = ds.Tables[0];
        //                        foreach (DataRow r in dt.Rows)
        //                        {
        //                            #region
        //                            fcdsInfo.FCDS_HHDL_HHDLJGXS = Convert.ToString(r["混合动力结构型式"]);
        //                            paramMsg += Utils.VerifyRequired("混合动力结构型式", fcdsInfo.FCDS_HHDL_HHDLJGXS);

        //                            fcdsInfo.FCDS_HHDL_XSMSSDXZGN = Convert.ToString(r["是否具有行驶模式手动选择功能"]);
        //                            paramMsg += Utils.VerifyRequired("是否具有行驶模式手动选择功能", fcdsInfo.FCDS_HHDL_XSMSSDXZGN);

        //                            fcdsInfo.FCDS_HHDL_DLXDCZZL = Convert.ToString(r["电动汽车储能装置种类"]);
        //                            paramMsg += Utils.VerifyRequired("电动汽车储能装置种类", fcdsInfo.FCDS_HHDL_DLXDCZZL);



        //                            fcdsInfo.FCDS_HHDL_DLXDCZZNL = Convert.ToString(r["储能装置总储电量"]);
        //                            paramMsg += Utils.VerifyRequired("储能装置总储电量", fcdsInfo.FCDS_HHDL_DLXDCZZNL);

        //                            fcdsInfo.FCDS_HHDL_DLXDCBNL = Convert.ToString(r["动力电池系统能量密度"]);
        //                            paramMsg += Utils.VerifyRequired("动力电池系统能量密度", fcdsInfo.FCDS_HHDL_DLXDCBNL);

        //                            fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC = Convert.ToString(r["纯电驱动模式续驶里程（工况法）"]);
        //                            if (!string.IsNullOrEmpty(fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC))
        //                                paramMsg += Utils.VerifyFloat(fcdsInfo.FCDS_HHDL_DLXDCBNL);

        //                            fcdsInfo.FCDS_HHDL_CDDMSXZGCS = Convert.ToString(r["纯电动模式下1km最高车速"]);
        //                            if (!string.IsNullOrEmpty(fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC))
        //                                paramMsg += Utils.VerifyInt(fcdsInfo.FCDS_HHDL_DLXDCBNL);

        //                            fcdsInfo.FCDS_HHDL_DLXDCZBCDY = Convert.ToString(r["储能装置总成标称电压"]);
        //                            paramMsg += Utils.VerifyRequired("储能装置总成标称电压", fcdsInfo.FCDS_HHDL_DLXDCZBCDY);

        //                            fcdsInfo.FCDS_HHDL_QDDJLX = Convert.ToString(r["驱动电机类型"]);
        //                            paramMsg += Utils.VerifyRequired("驱动电机类型", fcdsInfo.FCDS_HHDL_QDDJLX);

        //                            fcdsInfo.FCDS_HHDL_HHDLZDDGLB = Convert.ToString(r["混合动力汽车电功率比"]);
        //                            paramMsg += Utils.VerifyRequired("混合动力汽车电功率比", fcdsInfo.FCDS_HHDL_HHDLZDDGLB);

        //                            fcdsInfo.FCDS_HHDL_QDDJFZNJ = Convert.ToString(r["驱动电机峰值转矩"]);
        //                            paramMsg += Utils.VerifyRequired("驱动电机峰值转矩", fcdsInfo.FCDS_HHDL_QDDJFZNJ);


        //                            fcdsInfo.FCDS_HHDL_QDDJEDGL = Convert.ToString(r["驱动电机额定功率"]);
        //                            paramMsg += Utils.VerifyRequired("驱动电机额定功率", fcdsInfo.FCDS_HHDL_QDDJEDGL);

        //                            #endregion
        //                        }
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                readMsg = ex.Message + "\r\n";
        //            }
        //            finally
        //            {
        //                xBook.Close();
        //                Kill(xApp);
        //                //KillProcess("EXCEL");
        //            }

        //            if (!string.IsNullOrEmpty(readMsg))
        //            {
        //                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, readMsg);
        //            }

        //            if (!string.IsNullOrEmpty(paramMsg))
        //            {
        //                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, paramMsg);
        //            }


        //            if (importType == "IMPORT")
        //            {
        //                saveMsg = this.SaveFcdsCocInfo(cocId, basicInfo, fcdsInfo);
        //            }
        //            else if (importType == "UPDATE")
        //            {
        //                saveMsg = this.UpdateFcdsCocInfo(cocId, basicInfo, fcdsInfo, cocIdList);
        //            }

        //            if (string.IsNullOrEmpty(saveMsg))
        //            {
        //                // 移动文件
        //                string templateFileName = string.Empty;
        //                if (importType == "IMPORT")
        //                {
        //                    templateFileName = "F_COC";
        //                }
        //                else if (importType == "UPDATE")
        //                {
        //                    templateFileName = "U_COC";
        //                }
        //                this.MoveFinishedFile(fileName, folderName, templateFileName);
        //            }
        //            if (string.IsNullOrEmpty(saveMsg))
        //            {
        //                return "\r\n" + fileName + " 操作成功\r\n";
        //            }

        //            #endregion
        //        }

        //        if (xSheet.Name == this.CtnySheetName)
        //        {

        //            //Worksheet xSheet = (Worksheet)xBook.Sheets["Sheet1"];
        //            Range range = null;

        //            VehicleBasicInfo basicInfo = new VehicleBasicInfo();
        //            CtnyRllx ctnyInfo = new CtnyRllx();
        //            FcdsRllx fcdsInfo = new FcdsRllx();
        //            string cocId = string.Empty;
        //            string readMsg = string.Empty;
        //            string paramMsg = string.Empty;


        //            #region 读取Excel
        //            try
        //            {
        //                range = xSheet.get_Range(FUEL_DATA["cocNo"], Type.Missing);
        //                cocId = this.FormatCocId(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("COC编号", cocId);

        //                range = xSheet.get_Range(FUEL_DATA["clxh"], Type.Missing);
        //                basicInfo.Clxh = this.FormatClxh(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("产品型号", basicInfo.Clxh);

        //                range = xSheet.get_Range(FUEL_DATA["tymc"], Type.Missing);
        //                basicInfo.Tymc = this.FormatTymc(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("通用名称", basicInfo.Tymc);

        //                range = xSheet.get_Range(FUEL_DATA["clzl"], Type.Missing);
        //                basicInfo.Clzl = this.FormatClzl(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("车辆类型", basicInfo.Clzl);

        //                range = xSheet.get_Range(FUEL_DATA["yyc"], Type.Missing);
        //                basicInfo.Yyc = this.FormatYyc(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("越野车", basicInfo.Yyc);

        //                range = xSheet.get_Range(FUEL_DATA["qcscqy"], Type.Missing);
        //                basicInfo.Qcscqy = this.FormatQcscqy(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("乘用车生产企业", basicInfo.Qcscqy);

        //                range = xSheet.get_Range(FUEL_DATA["rllx"], Type.Missing);
        //                basicInfo.Rllx = this.FormatRllx(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料类型", basicInfo.Rllx);

        //                range = xSheet.get_Range(FUEL_DATA["zwps"], Type.Missing);
        //                basicInfo.Zwps = this.FormatZwps(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("座椅排数", basicInfo.Zwps);

        //                range = xSheet.get_Range(FUEL_DATA["zczbzl"], Type.Missing);
        //                basicInfo.Zczbzl = this.FormatZczbzl(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("整备质量", basicInfo.Zczbzl);

        //                range = xSheet.get_Range(FUEL_DATA["zdsjzzl"], Type.Missing);
        //                basicInfo.Zdsjzzl = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("总质量", basicInfo.Zdsjzzl);

        //                range = xSheet.get_Range(FUEL_DATA["zgcs"], Type.Missing);
        //                basicInfo.Zgcs = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("最高车速", basicInfo.Zgcs);

        //                range = xSheet.get_Range(FUEL_DATA["edzk"], Type.Missing);
        //                basicInfo.Edzk = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("额定载客", basicInfo.Edzk);

        //                range = xSheet.get_Range(FUEL_DATA["ltgg"], Type.Missing);
        //                basicInfo.Ltgg = this.FormatLtgg(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("轮胎规格", basicInfo.Ltgg);

        //                range = xSheet.get_Range(FUEL_DATA["lj"], Type.Missing);
        //                basicInfo.Lj = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("轮距", basicInfo.Lj);

        //                range = xSheet.get_Range(FUEL_DATA["zj"], Type.Missing);
        //                basicInfo.Zj = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("轴距", basicInfo.Zj);

        //                range = xSheet.get_Range(FUEL_DATA["qdxs"], Type.Missing);
        //                basicInfo.Qdxs = this.FormatQdxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("驱动型式", basicInfo.Qdxs);

        //                range = xSheet.get_Range(FUEL_DATA["jybgbh"], Type.Missing);
        //                basicInfo.Jybgbh = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("检验报告编号", basicInfo.Jybgbh);

        //                range = xSheet.get_Range(FUEL_DATA["jyjgmc"], Type.Missing);
        //                basicInfo.Jyjgmc = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("检验机构名称", basicInfo.Jyjgmc);

        //                // 传统能源参数信息
        //                range = xSheet.get_Range(FUEL_DATA["fdjxh"], Type.Missing);
        //                ctnyInfo.Fdjxh = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("发动机型号", ctnyInfo.Fdjxh);

        //                range = xSheet.get_Range(FUEL_DATA["qgs"], Type.Missing);
        //                ctnyInfo.Qgs = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("发动机气缸数目", ctnyInfo.Qgs);

        //                range = xSheet.get_Range(FUEL_DATA["pl"], Type.Missing);
        //                ctnyInfo.Pl = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
        //                paramMsg += Utils.VerifyRequired("发动机排量", ctnyInfo.Pl);

        //                range = xSheet.get_Range(FUEL_DATA["edgl"], Type.Missing);
        //                ctnyInfo.Edgl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("发动机功率", ctnyInfo.Edgl);

        //                range = xSheet.get_Range(FUEL_DATA["jgl"], Type.Missing);
        //                ctnyInfo.Jgl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());

        //                range = xSheet.get_Range(FUEL_DATA["bsqxs"], Type.Missing);
        //                ctnyInfo.Bsqxs = this.FormatBsqxs(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("变速器型式", ctnyInfo.Bsqxs);

        //                range = xSheet.get_Range(FUEL_DATA["bsqdws"], Type.Missing);
        //                ctnyInfo.Bsqdws = this.FormatBsqdws(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("变速器档位数", ctnyInfo.Bsqdws);

        //                range = xSheet.get_Range(FUEL_DATA["qcjnjs"], Type.Missing);
        //                ctnyInfo.Qcjnjs = range.Value2 == null ? "" : range.Value2.ToString().Trim();

        //                range = xSheet.get_Range(FUEL_DATA["qtxx"], Type.Missing);
        //                ctnyInfo.Qtxx = range.Value2 == null ? "" : range.Value2.ToString().Trim();

        //                range = xSheet.get_Range(FUEL_DATA["sqgkrlxhl"], Type.Missing);
        //                ctnyInfo.Sqgkrlxhl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料消耗量（市区）", ctnyInfo.Sqgkrlxhl);

        //                range = xSheet.get_Range(FUEL_DATA["sjgkrlxhl"], Type.Missing);
        //                ctnyInfo.Sjgkrlxhl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料消耗量（市郊）", ctnyInfo.Sjgkrlxhl);

        //                range = xSheet.get_Range(FUEL_DATA["zhgkrlxhl"], Type.Missing);
        //                ctnyInfo.Zhgkrlxhl = this.FormatDecimal(range.Value2 == null ? "" : range.Value2.ToString().Trim());
        //                paramMsg += Utils.VerifyRequired("燃料消耗量（综合）", ctnyInfo.Zhgkrlxhl);

        //                range = xSheet.get_Range(FUEL_DATA["zhgkco2pfl"], Type.Missing);
        //                ctnyInfo.Zhgkco2pfl = range.Value2 == null ? "" : Double.Parse(range.Value2.ToString()).ToString("0").Trim();
        //                paramMsg += Utils.VerifyRequired("CO2排放量（综合）", ctnyInfo.Zhgkco2pfl);

        //                range = xSheet.get_Range(FUEL_DATA["fdjscc"], Type.Missing);
        //                ctnyInfo.Gh_Fdjscc = range.Value2 == null ? "" : range.Value2.ToString().Trim();
        //                paramMsg += Utils.VerifyRequired("发动机生产厂", ctnyInfo.Gh_Fdjscc);
        //            }
        //            catch (Exception ex)
        //            {
        //                readMsg = ex.Message + "\r\n";
        //            }
        //            finally
        //            {
        //                xBook.Close();

        //                Kill(xApp);
        //                //KillProcess("EXCEL");
        //            }

        //            #endregion

        //            if (!string.IsNullOrEmpty(readMsg))
        //            {
        //                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, readMsg);
        //            }

        //            if (!string.IsNullOrEmpty(paramMsg))
        //            {
        //                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, paramMsg);
        //            }


        //            if (importType == "IMPORT")
        //            {
        //                saveMsg = this.SaveCtnyCocInfo(cocId, basicInfo, ctnyInfo);
        //            }
        //            else if (importType == "UPDATE")
        //            {
        //                saveMsg = this.UpdateCtnyCocInfo(cocId, basicInfo, ctnyInfo, cocIdList);
        //            }


        //        }
        //    }

        //    if (string.IsNullOrEmpty(saveMsg))
        //    {
        //        // 移动文件
        //        string templateFileName = string.Empty;
        //        if (importType == "IMPORT")
        //        {
        //            templateFileName = "F_COC";
        //        }
        //        else if (importType == "UPDATE")
        //        {
        //            templateFileName = "U_COC";
        //        }
        //        this.MoveFinishedFile(fileName, folderName, templateFileName);
        //    }
        //    if (string.IsNullOrEmpty(saveMsg))
        //    {
        //        return "\r\n" + fileName + " 操作成功\r\n";
        //    }
        //    return saveMsg;
        //}
        #endregion

        /// <summary>
        /// 保存非插电式混合动力
        /// </summary>
        /// <param name="cocId"></param>
        /// <param name="basicInfo"></param>
        /// <param name="ctnyInfo"></param>
        /// <returns></returns>
        public string SaveFcdsCocInfo(string cocId, VehicleBasicInfo basicInfo, CtnyRllx ctnyInfo)
        {
            string msg = string.Empty;


            return msg;
        }

        /// <summary>
        /// 保存非插电式COC信息
        /// </summary>
        /// <param name="basicInfo"></param>
        /// <param name="ctnyInfo"></param>
        public string SaveFcdsCocInfo(string cocId, VehicleBasicInfo basicInfo, FcdsRllx fcdsInfo)
        {
            string msg = string.Empty;

            if (this.IsFcdsCocIdExist(cocId))
            {
                return cocId + " 已经导入，如需修改请点击“修改COC”功能";
            }

            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();//创建事务，开始执行事务
            try
            {
                string rllx = basicInfo.Rllx;
                if (basicInfo.Rllx.IndexOf(this.FcdsFlag) > -1)
                {
                    rllx = "非插电式混合动力";
                }
                OleDbParameter createTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                createTime.OleDbType = OleDbType.DBDate;
                OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                updateTime.OleDbType = OleDbType.DBDate;
                StringBuilder strSql = new StringBuilder();
                strSql.Append("insert into COC_FCDS(");
                strSql.Append("COC_ID,JKQCZJXS,QCSCQY,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,STATUS,JYJGMC,JYBGBH,FCDS_HHDL_BSQDWS,FCDS_HHDL_BSQXS,FCDS_HHDL_CDDMSXZGCS,FCDS_HHDL_CDDMSXZHGKXSLC,FCDS_HHDL_DLXDCBNL,FCDS_HHDL_DLXDCZBCDY,FCDS_HHDL_DLXDCZZL,FCDS_HHDL_DLXDCZZNL,FCDS_HHDL_EDGL,FCDS_HHDL_FDJXH,FCDS_HHDL_HHDLJGXS,FCDS_HHDL_HHDLZDDGLB,FCDS_HHDL_JGL,FCDS_HHDL_PL,FCDS_HHDL_QDDJEDGL,FCDS_HHDL_QDDJFZNJ,FCDS_HHDL_QDDJLX,FCDS_HHDL_QGS,CT_QTXX,FCDS_HHDL_SJGKRLXHL,FCDS_HHDL_SQGKRLXHL,FCDS_HHDL_XSMSSDXZGN,FCDS_HHDL_ZHKGCO2PL,FCDS_HHDL_ZHGKRLXHL,CREATETIME,UPDATETIME,GH_FDJSCC)");
                strSql.Append(" values (");
                strSql.Append("@COC_ID,@JKQCZJXS,@QCSCQY,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@STATUS,@JYJGMC,@JYBGBH,@FCDS_HHDL_BSQDWS,@FCDS_HHDL_BSQXS,@FCDS_HHDL_CDDMSXZGCS,@FCDS_HHDL_CDDMSXZHGKXSLC,@FCDS_HHDL_DLXDCBNL,@FCDS_HHDL_DLXDCZBCDY,@FCDS_HHDL_DLXDCZZL,@FCDS_HHDL_DLXDCZZNL,@FCDS_HHDL_EDGL,@FCDS_HHDL_FDJXH,@FCDS_HHDL_HHDLJGXS,@FCDS_HHDL_HHDLZDDGLB,@FCDS_HHDL_JGL,@FCDS_HHDL_PL,@FCDS_HHDL_QDDJEDGL,@FCDS_HHDL_QDDJFZNJ,@FCDS_HHDL_QDDJLX,@FCDS_HHDL_QGS,@CT_QTXX,@FCDS_HHDL_SJGKRLXHL,@FCDS_HHDL_SQGKRLXHL,@FCDS_HHDL_XSMSSDXZGN,@FCDS_HHDL_ZHKGCO2PL,@FCDS_HHDL_ZHGKRLXHL,@CREATETIME,@UPDATETIME,@GH_FDJSCC)");
                OleDbParameter[] parameters = {

					new OleDbParameter("@COC_ID",cocId),
                                        new OleDbParameter("@JKQCZJXS",Utils.qymc),
                                        new OleDbParameter("@QCSCQY",basicInfo.Qcscqy),
                                        new OleDbParameter("@CLXH",basicInfo.Clxh),
                                        new OleDbParameter("@CLZL",basicInfo.Clzl),
                                        new OleDbParameter("@RLLX",rllx),
                                        new OleDbParameter("@ZCZBZL",basicInfo.Zczbzl),
                                        new OleDbParameter("@ZGCS",basicInfo.Zgcs),
                                        new OleDbParameter("@LTGG",basicInfo.Ltgg),
                                        new OleDbParameter("@ZJ",basicInfo.Zj),
                                        new OleDbParameter("@TYMC",basicInfo.Tymc),
                                        new OleDbParameter("@YYC",basicInfo.Yyc),
                                        new OleDbParameter("@ZWPS",basicInfo.Zwps),
                                        new OleDbParameter("@ZDSJZZL",basicInfo.Zdsjzzl),
                                        new OleDbParameter("@EDZK",basicInfo.Edzk),
                                        new OleDbParameter("@LJ",basicInfo.Lj),
                                        new OleDbParameter("@QDXS",basicInfo.Qdxs),
                                        new OleDbParameter("@STATUS","1"),
                                        new OleDbParameter("@JYJGMC",basicInfo.Jyjgmc),
                                        new OleDbParameter("@JYBGBH",basicInfo.Jybgbh),
					new OleDbParameter("@FCDS_HHDL_BSQDWS", fcdsInfo.FCDS_HHDL_BSQDWS),
					new OleDbParameter("@FCDS_HHDL_BSQXS", fcdsInfo.FCDS_HHDL_BSQXS),
					new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", fcdsInfo.FCDS_HHDL_CDDMSXZGCS),
					new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC),
					new OleDbParameter("@FCDS_HHDL_DLXDCBNL", Convert.ToDouble(fcdsInfo.FCDS_HHDL_DLXDCBNL).ToString(".0")),
					new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY",fcdsInfo.FCDS_HHDL_DLXDCZBCDY),
					new OleDbParameter("@FCDS_HHDL_DLXDCZZL", fcdsInfo.FCDS_HHDL_DLXDCZZL),
					new OleDbParameter("@FCDS_HHDL_DLXDCZZNL",fcdsInfo.FCDS_HHDL_DLXDCZZNL),
					new OleDbParameter("@FCDS_HHDL_EDGL", fcdsInfo.FCDS_HHDL_EDGL),
					new OleDbParameter("@FCDS_HHDL_FDJXH", fcdsInfo.FCDS_HHDL_FDJXH),
					new OleDbParameter("@FCDS_HHDL_HHDLJGXS", fcdsInfo.FCDS_HHDL_HHDLJGXS),
					new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB",Convert.ToDouble(fcdsInfo.FCDS_HHDL_HHDLZDDGLB).ToString(".00")),
					new OleDbParameter("@FCDS_HHDL_JGL", fcdsInfo.FCDS_HHDL_JGL),
					new OleDbParameter("@FCDS_HHDL_PL", fcdsInfo.FCDS_HHDL_PL),
					new OleDbParameter("@FCDS_HHDL_QDDJEDGL", Convert.ToDouble(fcdsInfo.FCDS_HHDL_QDDJEDGL).ToString(".0")),
					new OleDbParameter("@FCDS_HHDL_QDDJFZNJ",fcdsInfo.FCDS_HHDL_QDDJFZNJ),
					new OleDbParameter("@FCDS_HHDL_QDDJLX", fcdsInfo.FCDS_HHDL_QDDJLX),
					new OleDbParameter("@FCDS_HHDL_QGS", fcdsInfo.FCDS_HHDL_QGS),
					new OleDbParameter("@CT_QTXX", fcdsInfo.CT_QTXX),
					new OleDbParameter("@FCDS_HHDL_SJGKRLXHL",fcdsInfo.FCDS_HHDL_SJGKRLXHL),
					new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", fcdsInfo.FCDS_HHDL_SQGKRLXHL),
					new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN",fcdsInfo.FCDS_HHDL_XSMSSDXZGN),
					new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", fcdsInfo.FCDS_HHDL_ZHKGCO2PL),
					new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", fcdsInfo.FCDS_HHDL_ZHGKRLXHL),
					createTime,
					updateTime,
					new OleDbParameter("@GH_FDJSCC", fcdsInfo.FCDS_GH_FDJSCC)};
                AccessHelper.ExecuteNonQuery(con, strSql.ToString(), parameters);
            }
            catch (Exception ex)
            {
                msg = cocId + " 导入失败:" + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return msg;
        }

        /// <summary>
        /// 保存传统能源COC信息
        /// </summary>
        /// <param name="basicInfo"></param>
        /// <param name="ctnyInfo"></param>
        public string SaveCtnyCocInfo(string cocId, VehicleBasicInfo basicInfo, CtnyRllx ctnyInfo)
        {
            string msg = string.Empty;

            if (this.IsCocIdExist(cocId))
            {
                return cocId + " 已经导入，如需修改请点击“修改COC”功能";
            }

            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();//创建事务，开始执行事务
            try
            {
                string sqlStr = @"INSERT INTO COC_INFO(
                                            COC_ID,JKQCZJXS, QCSCQY,
                                            CLXH, CLZL, RLLX,ZCZBZL, ZGCS, 
                                            LTGG, ZJ, TYMC, YYC, ZWPS, 
                                            ZDSJZZL, EDZK, LJ, QDXS, 
                                            JYJGMC, JYBGBH, 
                                            CT_BSQDWS, CT_BSQXS, CT_EDGL, CT_FDJXH, CT_JGL, 
                                            CT_PL, CT_QCJNJS, CT_QGS, CT_QTXX, CT_SJGKRLXHL, 
                                            CT_SQGKRLXHL, CT_ZHGKCO2PFL, CT_ZHGKRLXHL, 
                                            STATUS, CREATETIME, GH_FDJSCC) 
                                    Values (
                                            @COC_ID, @JKQCZJXS, @QCSCQY, 
                                            @CLXH, @CLZL, @RLLX, @ZCZBZL, @ZGCS, 
                                            @LTGG, @ZJ, @TYMC, @YYC, @ZWPS, 
                                            @ZDSJZZL, @EDZK, @LJ, @QDXS, 
                                            @JYJGMC, @JYBGBH, 
                                            @CT_BSQDWS, @CT_BSQXS, @CT_EDGL, @CT_FDJXH, @CT_JGL, 
                                            @CT_PL, @CT_QCJNJS, @CT_QGS, @CT_QTXX, @CT_SJGKRLXHL, 
                                            @CT_SQGKRLXHL, @CT_ZHGKCO2PFL, @CT_ZHGKRLXHL, 
                                            @STATUS, @CREATETIME, @GH_FDJSCC)";

                OleDbParameter createTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                createTime.OleDbType = OleDbType.DBDate;
                OleDbParameter[] paramList = { 
                                        new OleDbParameter("@COC_ID",cocId),
                                        new OleDbParameter("@JKQCZJXS",Utils.qymc),
                                        new OleDbParameter("@QCSCQY",basicInfo.Qcscqy),
                                        new OleDbParameter("@CLXH",basicInfo.Clxh),
                                        new OleDbParameter("@CLZL",basicInfo.Clzl),
                                        new OleDbParameter("@RLLX",basicInfo.Rllx),
                                        new OleDbParameter("@ZCZBZL",basicInfo.Zczbzl),
                                        new OleDbParameter("@ZGCS",basicInfo.Zgcs),
                                        new OleDbParameter("@LTGG",basicInfo.Ltgg),
                                        new OleDbParameter("@ZJ",basicInfo.Zj),
                                        new OleDbParameter("@TYMC",basicInfo.Tymc),
                                        new OleDbParameter("@YYC",basicInfo.Yyc),
                                        new OleDbParameter("@ZWPS",basicInfo.Zwps),
                                        new OleDbParameter("@ZDSJZZL",basicInfo.Zdsjzzl),
                                        new OleDbParameter("@EDZK",basicInfo.Edzk),
                                        new OleDbParameter("@LJ",basicInfo.Lj),
                                        new OleDbParameter("@QDXS",basicInfo.Qdxs),
                                        new OleDbParameter("@JYJGMC",basicInfo.Jyjgmc),
                                        new OleDbParameter("@JYBGBH",basicInfo.Jybgbh),

                                        new OleDbParameter("@CT_BSQDWS",ctnyInfo.Bsqdws),
                                        new OleDbParameter("@CT_BSQXS",ctnyInfo.Bsqxs),
                                        new OleDbParameter("@CT_EDGL",ctnyInfo.Edgl),
                                        new OleDbParameter("@CT_FDJXH",ctnyInfo.Fdjxh),
                                        new OleDbParameter("@CT_JGL",ctnyInfo.Jgl),
                                        new OleDbParameter("@CT_PL",ctnyInfo.Pl),
                                        new OleDbParameter("@CT_QCJNJS",ctnyInfo.Qcjnjs),
                                        new OleDbParameter("@CT_QGS",ctnyInfo.Qgs),
                                        new OleDbParameter("@CT_QTXX",ctnyInfo.Qtxx),
                                        new OleDbParameter("@CT_SJGKRLXHL",ctnyInfo.Sjgkrlxhl),
                                        new OleDbParameter("@CT_SQGKRLXHL",ctnyInfo.Sqgkrlxhl),
                                        new OleDbParameter("@CT_ZHGKCO2PFL",ctnyInfo.Zhgkco2pfl),
                                        new OleDbParameter("@CT_ZHGKRLXHL",ctnyInfo.Zhgkrlxhl),

                                        new OleDbParameter("@STATUS","1"),
                                        createTime,
                                        new OleDbParameter("@GH_FDJSCC",ctnyInfo.Gh_Fdjscc)
                                   };
                AccessHelper.ExecuteNonQuery(con, sqlStr, paramList);
            }
            catch (Exception ex)
            {
                msg = cocId + " 导入失败:" + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return msg;
        }

        /// <summary>
        /// 修改非插电式COC信息
        /// </summary>
        /// <param name="cocoId"></param>
        /// <param name="basicInfo"></param>
        /// <param name="fcdsInfo"></param>
        /// <param name="cocIdList"></param>
        /// <returns></returns>
        public string UpdateFcdsCocInfo(string cocId, VehicleBasicInfo basicInfo, FcdsRllx fcdsInfo, List<string> cocIdList)
        {
            string msg = string.Empty;

            if (!this.IsFcdsCocIdExist(cocId))
            {
                return cocId + " 不存在，请直接导入";
            }

            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();//创建事务，开始执行事务
            try
            {
                string rllx = basicInfo.Rllx;
                if (basicInfo.Rllx.IndexOf(this.FcdsFlag) > -1)
                {
                    rllx = "非插电式混合动力";
                }
                OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                updateTime.OleDbType = OleDbType.DBDate;
                StringBuilder strSql = new StringBuilder();
                strSql.Append("update COC_FCDS set ");
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
                strSql.Append("JYJGMC=@JYJGMC,");
                strSql.Append("JYBGBH=@JYBGBH,");
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
                strSql.Append("CT_QTXX=@CT_QTXX,");
                strSql.Append("FCDS_HHDL_SJGKRLXHL=@FCDS_HHDL_SJGKRLXHL,");
                strSql.Append("FCDS_HHDL_SQGKRLXHL=@FCDS_HHDL_SQGKRLXHL,");
                strSql.Append("FCDS_HHDL_XSMSSDXZGN=@FCDS_HHDL_XSMSSDXZGN,");
                strSql.Append("FCDS_HHDL_ZHKGCO2PL=@FCDS_HHDL_ZHKGCO2PL,");
                strSql.Append("FCDS_HHDL_ZHGKRLXHL=@FCDS_HHDL_ZHGKRLXHL,");
                strSql.Append("UPDATETIME=@UPDATETIME,");
                strSql.Append("GH_FDJSCC=@GH_FDJSCC");
                strSql.Append(" where COC_ID=@COC_ID ");
                OleDbParameter[] parameters = {
					new OleDbParameter("@JKQCZJXS", Utils.qymc),
					new OleDbParameter("@QCSCQY",basicInfo.Qcscqy),
                                        new OleDbParameter("@CLXH",basicInfo.Clxh),
                                        new OleDbParameter("@CLZL",basicInfo.Clzl),
                                        new OleDbParameter("@RLLX",rllx),
                                        new OleDbParameter("@ZCZBZL",basicInfo.Zczbzl),
                                        new OleDbParameter("@ZGCS",basicInfo.Zgcs),
                                        new OleDbParameter("@LTGG",basicInfo.Ltgg),
                                        new OleDbParameter("@ZJ",basicInfo.Zj),
                                        new OleDbParameter("@TYMC",basicInfo.Tymc),
                                        new OleDbParameter("@YYC",basicInfo.Yyc),
                                        new OleDbParameter("@ZWPS",basicInfo.Zwps),
                                        new OleDbParameter("@ZDSJZZL",basicInfo.Zdsjzzl),
                                        new OleDbParameter("@EDZK",basicInfo.Edzk),
                                        new OleDbParameter("@LJ",basicInfo.Lj),
                                        new OleDbParameter("@QDXS",basicInfo.Qdxs),
                                        new OleDbParameter("@JYJGMC",basicInfo.Jyjgmc),
                                        new OleDbParameter("@JYBGBH",basicInfo.Jybgbh),
					new OleDbParameter("@FCDS_HHDL_BSQDWS", fcdsInfo.FCDS_HHDL_BSQDWS),
					new OleDbParameter("@FCDS_HHDL_BSQXS", fcdsInfo.FCDS_HHDL_BSQXS),
					new OleDbParameter("@FCDS_HHDL_CDDMSXZGCS", fcdsInfo.FCDS_HHDL_CDDMSXZGCS),
					new OleDbParameter("@FCDS_HHDL_CDDMSXZHGKXSLC", fcdsInfo.FCDS_HHDL_CDDMSXZHGKXSLC),
					new OleDbParameter("@FCDS_HHDL_DLXDCBNL", Convert.ToDouble(fcdsInfo.FCDS_HHDL_DLXDCBNL).ToString(".0")),
					new OleDbParameter("@FCDS_HHDL_DLXDCZBCDY",fcdsInfo.FCDS_HHDL_DLXDCZBCDY),
					new OleDbParameter("@FCDS_HHDL_DLXDCZZL", fcdsInfo.FCDS_HHDL_DLXDCZZL),
					new OleDbParameter("@FCDS_HHDL_DLXDCZZNL",fcdsInfo.FCDS_HHDL_DLXDCZZNL),
					new OleDbParameter("@FCDS_HHDL_EDGL", fcdsInfo.FCDS_HHDL_EDGL),
					new OleDbParameter("@FCDS_HHDL_FDJXH", fcdsInfo.FCDS_HHDL_FDJXH),
					new OleDbParameter("@FCDS_HHDL_HHDLJGXS", fcdsInfo.FCDS_HHDL_HHDLJGXS),
					new OleDbParameter("@FCDS_HHDL_HHDLZDDGLB",Convert.ToDouble(fcdsInfo.FCDS_HHDL_HHDLZDDGLB).ToString(".00")),
					new OleDbParameter("@FCDS_HHDL_JGL", fcdsInfo.FCDS_HHDL_JGL),
					new OleDbParameter("@FCDS_HHDL_PL", fcdsInfo.FCDS_HHDL_PL),
					new OleDbParameter("@FCDS_HHDL_QDDJEDGL", Convert.ToDouble(fcdsInfo.FCDS_HHDL_QDDJEDGL).ToString(".0")),
					new OleDbParameter("@FCDS_HHDL_QDDJFZNJ",fcdsInfo.FCDS_HHDL_QDDJFZNJ),
					new OleDbParameter("@FCDS_HHDL_QDDJLX", fcdsInfo.FCDS_HHDL_QDDJLX),
					new OleDbParameter("@FCDS_HHDL_QGS", fcdsInfo.FCDS_HHDL_QGS),
					new OleDbParameter("@CT_QTXX", fcdsInfo.CT_QTXX),
					new OleDbParameter("@FCDS_HHDL_SJGKRLXHL",fcdsInfo.FCDS_HHDL_SJGKRLXHL),
					new OleDbParameter("@FCDS_HHDL_SQGKRLXHL", fcdsInfo.FCDS_HHDL_SQGKRLXHL),
					new OleDbParameter("@FCDS_HHDL_XSMSSDXZGN",fcdsInfo.FCDS_HHDL_XSMSSDXZGN),
					new OleDbParameter("@FCDS_HHDL_ZHKGCO2PL", fcdsInfo.FCDS_HHDL_ZHKGCO2PL),
					new OleDbParameter("@FCDS_HHDL_ZHGKRLXHL", fcdsInfo.FCDS_HHDL_ZHGKRLXHL),
					updateTime,
					new OleDbParameter("@GH_FDJSCC", fcdsInfo.FCDS_GH_FDJSCC),
                    new OleDbParameter("@COC_ID",cocId),                      
                                              };
                AccessHelper.ExecuteNonQuery(con, strSql.ToString(), parameters);
                cocIdList.Add(cocId);
            }
            catch (Exception ex)
            {
                msg = cocId + " 修改失败:" + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return msg;
        }

        /// <summary>
        /// 修改传统能源COC信息
        /// </summary>
        /// <param name="basicInfo"></param>
        /// <param name="ctnyInfo"></param>
        public string UpdateCtnyCocInfo(string cocId, VehicleBasicInfo basicInfo, CtnyRllx ctnyInfo, List<string> cocIdList)
        {
            string msg = string.Empty;

            if (!this.IsCocIdExist(cocId))
            {
                return cocId + " 不存在，请直接导入";
            }

            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();//创建事务，开始执行事务
            try
            {
                string sqlStr = @"UPDATE COC_INFO SET
                                            JKQCZJXS=@JKQCZJXS, QCSCQY=@QCSCQY,
                                            CLXH=@CLXH, CLZL=@CLZL, RLLX=@RLLX,ZCZBZL=@ZCZBZL, ZGCS=@ZGCS, 
                                            LTGG=@LTGG, ZJ=@ZJ, TYMC=@TYMC, YYC=@YYC, ZWPS=@ZWPS, 
                                            ZDSJZZL=@ZDSJZZL, EDZK=@EDZK, LJ=@LJ, QDXS=@QDXS, 
                                            JYJGMC=@JYJGMC, JYBGBH=@JYBGBH, 
                                            CT_BSQDWS=@CT_BSQDWS, CT_BSQXS=@CT_BSQXS, CT_EDGL=@CT_EDGL, CT_FDJXH=@CT_FDJXH, CT_JGL=@CT_JGL, 
                                            CT_PL=@CT_PL, CT_QCJNJS=@CT_QCJNJS, CT_QGS=@CT_QGS, CT_QTXX=@CT_QTXX, CT_SJGKRLXHL=@CT_SJGKRLXHL, 
                                            CT_SQGKRLXHL=@CT_SQGKRLXHL, CT_ZHGKCO2PFL=@CT_ZHGKCO2PFL, CT_ZHGKRLXHL=@CT_ZHGKRLXHL, 
                                            STATUS=@STATUS, UPDATETIME=@UPDATETIME,GH_FDJSCC=@GH_FDJSCC
                                          WHERE COC_ID=@COC_ID";

                OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                updateTime.OleDbType = OleDbType.DBDate;
                OleDbParameter[] paramList = { 
                                        new OleDbParameter("@JKQCZJXS",Utils.qymc),
                                        new OleDbParameter("@QCSCQY",basicInfo.Qcscqy),
                                        new OleDbParameter("@CLXH",basicInfo.Clxh),
                                        new OleDbParameter("@CLZL",basicInfo.Clzl),
                                        new OleDbParameter("@RLLX",basicInfo.Rllx),
                                        new OleDbParameter("@ZCZBZL",basicInfo.Zczbzl),
                                        new OleDbParameter("@ZGCS",basicInfo.Zgcs),
                                        new OleDbParameter("@LTGG",basicInfo.Ltgg),
                                        new OleDbParameter("@ZJ",basicInfo.Zj),
                                        new OleDbParameter("@TYMC",basicInfo.Tymc),
                                        new OleDbParameter("@YYC",basicInfo.Yyc),
                                        new OleDbParameter("@ZWPS",basicInfo.Zwps),
                                        new OleDbParameter("@ZDSJZZL",basicInfo.Zdsjzzl),
                                        new OleDbParameter("@EDZK",basicInfo.Edzk),
                                        new OleDbParameter("@LJ",basicInfo.Lj),
                                        new OleDbParameter("@QDXS",basicInfo.Qdxs),
                                        new OleDbParameter("@JYJGMC",basicInfo.Jyjgmc),
                                        new OleDbParameter("@JYBGBH",basicInfo.Jybgbh),

                                        new OleDbParameter("@CT_BSQDWS",ctnyInfo.Bsqdws),
                                        new OleDbParameter("@CT_BSQXS",ctnyInfo.Bsqxs),
                                        new OleDbParameter("@CT_EDGL",ctnyInfo.Edgl),
                                        new OleDbParameter("@CT_FDJXH",ctnyInfo.Fdjxh),
                                        new OleDbParameter("@CT_JGL",ctnyInfo.Jgl),
                                        new OleDbParameter("@CT_PL",ctnyInfo.Pl),
                                        new OleDbParameter("@CT_QCJNJS",ctnyInfo.Qcjnjs),
                                        new OleDbParameter("@CT_QGS",ctnyInfo.Qgs),
                                        new OleDbParameter("@CT_QTXX",ctnyInfo.Qtxx),
                                        new OleDbParameter("@CT_SJGKRLXHL",ctnyInfo.Sjgkrlxhl),
                                        new OleDbParameter("@CT_SQGKRLXHL",ctnyInfo.Sqgkrlxhl),
                                        new OleDbParameter("@CT_ZHGKCO2PFL",ctnyInfo.Zhgkco2pfl),
                                        new OleDbParameter("@CT_ZHGKRLXHL",ctnyInfo.Zhgkrlxhl),

                                        new OleDbParameter("@STATUS","1"),
                                        updateTime,
                                        new OleDbParameter("@GH_FDJSCC",ctnyInfo.Gh_Fdjscc),
                                        new OleDbParameter("@COC_ID",cocId),
                                   };
                AccessHelper.ExecuteNonQuery(con, sqlStr, paramList);
                cocIdList.Add(cocId);
            }
            catch (Exception ex)
            {
                msg = cocId + " 修改失败:" + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return msg;
        }

        /// <summary>
        /// 获取界面选中的COC数据信息
        /// </summary>
        /// <param name="gv"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<string> GetCocIdFromControl(GridView gv, System.Data.DataTable dt)
        {
            List<string> cocIdList = new List<string>();

            gv.PostEditor();

            if (dt != null)
            {
                DataRow[] drVinArr = dt.Select("check=True");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if ((bool)dt.Rows[i]["check"])
                    {
                        cocIdList.Add(dt.Rows[i]["COC_ID"].ToString());
                    }
                }
            }
            return cocIdList;
        }

        /// <summary>
        /// 根据VIN从vin信息表获取COC编码
        /// </summary>
        /// <param name="vin"></param>
        /// <returns></returns>
        public string GetCocIdFromVinData(string vin)
        {
            string cocId = string.Empty;
            string sqlCocId = string.Format(@"SELECT COC_ID FROM VIN_INFO_HIS WHERE VIN='{0}' ORDER BY CREATETIME DESC", vin);
            try
            {
                DataSet dsCocId = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlCocId, null);
                if (dsCocId != null && dsCocId.Tables[0].Rows.Count > 0)
                {
                    cocId = dsCocId.Tables[0].Rows[0]["COC_ID"] == null ? "" : dsCocId.Tables[0].Rows[0]["COC_ID"].ToString();
                }
            }
            catch (Exception)
            {
            }
            return cocId;
        }

        /// <summary>
        /// 提取燃料消耗量数据
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string SaveParam(DataSet ds, string fuelType)
        {
            string genMsg = string.Empty;
            string strCon = AccessHelper.conn;

            int pageSize = 20;
            ProcessForm pf = new ProcessForm();
            pf.Text = "正在生成油耗数据，请稍候";
            pf.Show();
            pf.TotalMax = (int)Math.Ceiling((decimal)ds.Tables[0].Rows.Count / (decimal)1);
            pf.ShowProcessBar();

            try
            {
                System.Data.DataTable dt = ds.Tables[0];
                string strCreater = Utils.userId;
                string sqlQueryParam = string.Empty;

                //DONE-lyc L767-778
                if (fuelType == Utils.CTNY)
                {
                    sqlQueryParam = @"SELECT PARAM_CODE 
                                    FROM RLLX_PARAM 
                                    WHERE (FUEL_TYPE='传统能源' AND STATUS='1')";
                }
                if (fuelType == Utils.FCDS)
                {
                    sqlQueryParam = @"SELECT PARAM_CODE 
                                    FROM RLLX_PARAM 
                                    WHERE (FUEL_TYPE='非插电式混合动力' AND STATUS='1')";
                }
                System.Data.DataTable dtPam = AccessHelper.ExecuteDataSet(strCon, sqlQueryParam, null).Tables[0];

                // 获取节假日数据,用于生成上报截止时间
                listHoliday = this.GetHoliday();

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
                                string hgspbm = dr["HGSPBM"] == null ? "" : dr["HGSPBM"].ToString().Trim().ToUpper();
                                //string sqlDelBasic = "DELETE FROM FC_CLJBXX WHERE VIN = '" + vin + "' AND STATUS='1'";
                                //AccessHelper.ExecuteNonQuery(tra, sqlDelBasic, null);

                                bool fuelFlag = true;
                                // 如果当前vin数据已经存在，则跳过
                                if (this.IsFuelDataExist(vin))
                                {
                                    fuelFlag = false;
                                    genMsg += vin + "已经存在。\r\n";
                                }

                                tra = con.BeginTransaction();

                                DateTime clzzrqDate = Convert.ToDateTime(dr["CLZZRQ"].ToString().Trim());
                                DateTime tgDate = Convert.ToDateTime(dr["TGRQ"].ToString().Trim());
                                OleDbParameter tgrq = new OleDbParameter("@TGRQ", tgDate);
                                tgrq.OleDbType = OleDbType.DBDate;
                                OleDbParameter clzzrqFuel = new OleDbParameter("@CLZZRQ", tgDate);
                                clzzrqFuel.OleDbType = OleDbType.DBDate;
                                OleDbParameter clzzrqGH = new OleDbParameter("@CLZZRQ", clzzrqDate);
                                clzzrqGH.OleDbType = OleDbType.DBDate;

                                DateTime uploadDeadlineDate = this.QueryUploadDeadLine(tgDate);
                                OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                                uploadDeadline.OleDbType = OleDbType.DBDate;

                                OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                                creTime.OleDbType = OleDbType.DBDate;
                                OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                                upTime.OleDbType = OleDbType.DBDate;

                                if (fuelFlag)
                                {

                                    #region 待生成的燃料基本信息数据存入燃料基本信息表

                                    string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,HGSPBM,USER_ID,COC_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,QTXX,STATUS,CREATETIME,UPDATETIME
                                ) VALUES
                                (   @VIN,@HGSPBM,@USER_ID,@COC_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@QTXX,@STATUS,@CREATETIME,@UPDATETIME)";

                                    OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@HGSPBM",hgspbm),
                                     new OleDbParameter("@USER_ID",strCreater),
                                     new OleDbParameter("@COC_ID",dr["COC_ID"].ToString().Trim()),
                                     new OleDbParameter("@QCSCQY",dr["QCSCQY"].ToString().Trim()),
                                     new OleDbParameter("@JKQCZJXS",dr["JKQCZJXS"].ToString().Trim()),
                                     clzzrqFuel,
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
                                     new OleDbParameter("@QTXX",dr["CT_QTXX"].ToString().Trim()),
                                     // 状态为1表示数据已经生成
                                     new OleDbParameter("@STATUS","1"),
                                     creTime,
                                     upTime
                                     };
                                    AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                                    #endregion

                                    #region 待生成的燃料参数信息存入燃料参数表

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

                                    #region 生成国环数据

                                    if (!this.IsGHDataExist(vin))
                                    {
                                        string sqlInsertGh = @"INSERT INTO GH_DATA
                                (VIN,COC_ID,CLXH,GH_FDJXLH,GH_FDJSCC,CLZZRQ,TGRQ,CREATETIME,UPDATETIME,STATUS) 
                                VALUES
                                (@VIN,@COC_ID,@CLXH,@GH_FDJXLH,@GH_FDJSCC,@CLZZRQ,@TGRQ,@CREATETIME,@UPDATETIME,@STATUS)";

                                        OleDbParameter[] paramGh = { 
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@COC_ID",dr["COC_ID"].ToString().Trim()),
                                     new OleDbParameter("@CLXH",dr["CLXH"].ToString().Trim()),
                                     new OleDbParameter("@GH_FDJXLH",dr["GH_FDJXLH"].ToString().Trim()),
                                     new OleDbParameter("@GH_FDJSCC",dr["GH_FDJSCC"].ToString().Trim()),
                                     clzzrqGH,
                                     tgrq, // 作为通关日期
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@STATUS","1") // 国环数据状态1表示数据有效
                                     };
                                        AccessHelper.ExecuteNonQuery(tra, sqlInsertGh, paramGh);
                                    }
                                    #endregion
                                }

                                // VIN数据插入VIN历史表
                                string sqlInsertHis = @"INSERT INTO VIN_INFO_HIS(VIN,HGSPBM,COC_ID,CLZZRQ,TGRQ,GH_FDJXLH,CREATETIME,DATA_TYPE) Values (@VIN, @HGSPBM,@COC_ID, @CLZZRQ, @TGRQ, @GH_FDJXLH, @CREATETIME, @DATA_TYPE)";
                                OleDbParameter[] paramHis = { 
                                                        new OleDbParameter("@VIN", vin),
                                                        new OleDbParameter("@HGSPBM", hgspbm),
                                                        new OleDbParameter("@COC_ID", dr["COC_ID"].ToString().Trim()),
                                                        clzzrqGH,
                                                        tgrq,
                                                        new OleDbParameter("@GH_FDJXLH",dr["GH_FDJXLH"].ToString().Trim()),
                                                        creTime,
                                                        new OleDbParameter("@DATA_TYPE",dr["DATA_TYPE"].ToString().Trim())
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
            finally
            {
                pf.Close();
            }
            return genMsg;
        }

        // 提取国环数据
        public string SaveGHParam(DataSet ds)
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
                // 获取节假日数据,用于生成上报截止时间

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

                                // 如果当前vin数据已经存在，则跳过
                                if (this.IsGHDataExist(vin))
                                {
                                    genMsg += vin + "已经存在。\r\n";
                                    continue;
                                }

                                tra = con.BeginTransaction();

                                DateTime clzzrqDate = Convert.ToDateTime(dr["CLZZRQ"].ToString().Trim());
                                DateTime tgDate = Convert.ToDateTime(dr["TGRQ"].ToString().Trim());

                                OleDbParameter tgrq = new OleDbParameter("@TGRQ", tgDate);
                                tgrq.OleDbType = OleDbType.DBDate;

                                OleDbParameter clzzrqFuel = new OleDbParameter("@CLZZRQ", tgDate);
                                clzzrqFuel.OleDbType = OleDbType.DBDate;

                                OleDbParameter clzzrqGH = new OleDbParameter("@CLZZRQ", clzzrqDate);
                                clzzrqGH.OleDbType = OleDbType.DBDate;

                                OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                                creTime.OleDbType = OleDbType.DBDate;

                                OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                                upTime.OleDbType = OleDbType.DBDate;

                                #region 生成国环数据

                                string sqlInsertGh = @"INSERT INTO GH_DATA
                                (VIN,COC_ID,CLXH,GH_FDJXLH,GH_FDJSCC,CLZZRQ,TGRQ,CREATETIME,UPDATETIME,STATUS) 
                                VALUES
                                (@VIN,@COC_ID,@CLXH,@GH_FDJXLH,@GH_FDJSCC,@CLZZRQ,@TGRQ,@CREATETIME,@UPDATETIME,@STATUS)";

                                OleDbParameter[] paramGh = { 
                                     new OleDbParameter("@VIN",vin),
                                     new OleDbParameter("@COC_ID",dr["COC_ID"].ToString().Trim()),
                                     new OleDbParameter("@CLXH",dr["CLXH"].ToString().Trim()),
                                     new OleDbParameter("@GH_FDJXLH",dr["GH_FDJXLH"].ToString().Trim()),
                                     new OleDbParameter("@GH_FDJSCC",dr["GH_FDJSCC"].ToString().Trim()),
                                     clzzrqGH,
                                     tgrq, // 作为通关日期
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@STATUS","1") // 国环数据状态1表示数据有效
                                     };
                                AccessHelper.ExecuteNonQuery(tra, sqlInsertGh, paramGh);

                                #endregion

                                // VIN数据插入VIN历史表
                                string sqlInsertHis = @"INSERT INTO VIN_INFO_HIS(VIN,COC_ID,CLZZRQ,TGRQ,GH_FDJXLH,CREATETIME,DATA_TYPE) Values (@VIN, @COC_ID, @CLZZRQ, @TGRQ, @GH_FDJXLH, @CREATETIME, @DATA_TYPE)";
                                OleDbParameter[] paramHis = { 
                                                        new OleDbParameter("@VIN", vin),
                                                        new OleDbParameter("@COC_ID", dr["COC_ID"].ToString().Trim()),
                                                        clzzrqGH,
                                                        tgrq,
                                                        new OleDbParameter("@GH_FDJXLH",dr["GH_FDJXLH"].ToString().Trim()),
                                                        creTime,
                                                        new OleDbParameter("@DATA_TYPE",dr["DATA_TYPE"].ToString().Trim())
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
            finally
            {
                pf.Close();
            }
            return genMsg;
        }

        /// <summary>
        /// 查找coc数据是否已经导入
        /// </summary>
        /// <param name="cocId"></param>
        /// <returns></returns>
        public bool IsCocIdExist(string cocId)
        {
            bool flag = false;
            string querySql = @"SELECT COC_ID FROM COC_INFO WHERE COC_ID='" + cocId + "'";
            try
            {
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, querySql, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    flag = true;
                }
            }
            catch (Exception)
            {
            }

            return flag;
        }


        /// <summary>
        /// 查找非插电式coc数据是否已经导入
        /// </summary>
        /// <param name="cocId"></param>
        /// <returns></returns>
        public bool IsFcdsCocIdExist(string cocId)
        {
            bool flag = false;
            string querySql = @"SELECT COC_ID FROM COC_FCDS WHERE COC_ID='" + cocId + "'";
            try
            {
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, querySql, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    flag = true;
                }
            }
            catch (Exception)
            {
            }

            return flag;
        }

        /// <summary>
        /// 获取燃料参数规格数据
        /// </summary>
        /// <param name="fuelType"></param>
        /// <returns></returns>
        public System.Data.DataTable GetRllxData(string fuelType)
        {
            string sqlQueryParam = string.Format(@"SELECT PARAM_CODE "
                                + " FROM RLLX_PARAM WHERE FUEL_TYPE='{0}' AND STATUS='1'", fuelType);
            System.Data.DataTable dtPam = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQueryParam, null).Tables[0];

            return dtPam;
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

        // 同步状态
        public bool ActionUpdate(GridView gv, System.Data.DataTable dt)
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

                        DataSet tempDt = service.QueryVidByVins(Utils.userId, Utils.password, vin);
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

        //得到选中数据
        private string GetUploadData(System.Data.DataTable dt)
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
        private bool UpdateV_ID(System.Data.DataTable dt)
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
            catch
            {
                tra.Rollback();
            }
            finally
            {
                con.Close();
            }
            return false;
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

        // 检查当前VIN数据是否已经存在于国环数据表中
        protected bool IsGHDataExist(string vin)
        {
            bool isExist = false;

            string sqlQuery = @"SELECT VIN FROM GH_DATA WHERE VIN='" + vin + "'";
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
        /// 根据导入EXCEL 查询 本地库数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet ReadSharchExcel(string path, string sheet, string status, string Date)
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
        /// 根据导入EXCEL 查询国环数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet SearchGHExcel(string path, string sheet)
        {
            DataSet ds = ReadExcel(path, sheet);
            StringBuilder strAdd = new StringBuilder();
            strAdd.Append("select * from GH_DATA where VIN in(");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    strAdd.Append("'");
                    strAdd.Append(Convert.ToString(r["VIN"]));
                    strAdd.Append("',");
                }
                string sql = strAdd.ToString().TrimEnd(',') + ")";
                return AccessHelper.ExecuteDataSet(strCon, sql, null);
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
                                    case (int)Status.已上报:
                                        statuswhere = ", STATUS=" + (int)Status.修改待上报;
                                        rel = true;
                                        break;
                                    case (int)Status.撤销待上报:
                                        break;
                                }

                                if (rel)
                                {
                                    // 获取节假日数据,用于生成上报截止时间
                                    listHoliday = this.GetHoliday();

                                    DateTime clzzrqDate = Convert.ToDateTime(r[1].ToString());
                                    DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);
                                    string sql = "UPDATE FC_CLJBXX SET CLZZRQ='" + clzzrqDate + "', UPLOADDEADLINE='" + uploadDeadlineDate + "'" + statuswhere + "  where VIN='" + r["VIN"] + "'";
                                    AccessHelper.ExecuteNonQuery(tra, sql, null);

                                    pf.progressBarControl1.PerformStep();
                                    System.Windows.Forms.Application.DoEvents();
                                }
                            }
                            tra.Commit();
                            result = 1;
                        }
                        catch
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
        /// 批量修改进口日期(国环数据)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public int ReadUpdateGHDate(string path, string sheet)
        {
            int result = 0;
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
                                DateTime tgrqDate = Convert.ToDateTime(r[1].ToString());
                                string sql = "UPDATE GH_DATA SET TGRQ='" + tgrqDate + "' where VIN='" + r["VIN"] + "'";
                                AccessHelper.ExecuteNonQuery(tra, sql, null);

                                pf.progressBarControl1.PerformStep();
                                System.Windows.Forms.Application.DoEvents();
                            }
                            tra.Commit();
                            result = 1;
                        }
                        catch
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
                System.Data.DataTable sheetNames = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
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
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheet + "$]", strConn);
                    oada.Fill(ds);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        // 修改燃料数据进口日期
        public bool UpdateImportDate(List<string> vinList, string status, string date)
        {
            bool flag = false;
            try
            {
                string vinStr = string.Empty;
                if (vinList != null && vinList.Count > 0)
                {
                    foreach (string vin in vinList)
                    {
                        vinStr += vin + "','";
                    }
                }

                vinStr = vinStr.Substring(0, vinStr.Length - 3);
                string sqlStatus = string.Empty;
                if (status == "0")
                {
                    sqlStatus = ", STATUS='2'";
                }
                string sqlUpdateDate = string.Format(@"UPDATE FC_CLJBXX SET CLZZRQ=@CLZZRQ, UPLOADDEADLINE=@UPLOADDEADLINE {1} WHERE VIN IN ('{0}')", vinStr, sqlStatus);
                DateTime clzzrqDate = Convert.ToDateTime(date);
                OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                clzzrq.OleDbType = OleDbType.DBDate;

                DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                uploadDeadline.OleDbType = OleDbType.DBDate;

                OleDbParameter[] param = { clzzrq, uploadDeadline };

                using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                {
                    AccessHelper.ExecuteNonQuery(con, sqlUpdateDate, param);
                }

                //this.DialogResult = DialogResult.OK;
                flag = true;
                MessageBox.Show("修改成功！", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败：" + ex.Message, "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return flag;
        }

        // 修改国环数据进口日期
        public bool UpdateGHImportDate(List<string> vinList, string date)
        {
            bool flag = false;
            try
            {
                string vinStr = string.Empty;
                if (vinList != null && vinList.Count > 0)
                {
                    foreach (string vin in vinList)
                    {
                        vinStr += vin + "','";
                    }
                }

                vinStr = vinStr.Substring(0, vinStr.Length - 3);
                string sqlUpdateDate = string.Format(@"UPDATE GH_DATA SET TGRQ=@TGRQ WHERE VIN IN ('{0}')", vinStr);
                DateTime tgrqDate = Convert.ToDateTime(date);
                OleDbParameter tgrq = new OleDbParameter("@TGRQ", tgrqDate);
                tgrq.OleDbType = OleDbType.DBDate;

                OleDbParameter[] param = { tgrq };

                using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                {
                    AccessHelper.ExecuteNonQuery(con, sqlUpdateDate, param);
                }

                flag = true;
                MessageBox.Show("修改成功！", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败：" + ex.Message, "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return flag;
        }

        // 提取Coc ID
        public string FormatCocId(string cocId)
        {
            if (!string.IsNullOrEmpty(cocId))
            {
                cocId = cocId.Substring(10);
            }
            return cocId;
        }

        // 提取并格式化产品型号
        public string FormatClxh(string clxh)
        {
            if (!string.IsNullOrEmpty(clxh))
            {
                string[] clxhArr = clxh.Split(':');
                string clxhTemp = clxhArr[clxhArr.Length - 1].Trim();
                clxh = clxhTemp.Trim().Substring(0, clxhTemp.LastIndexOf("/"));
            }
            return clxh;
        }

        // 提取并格式化车辆类型
        public string FormatClzl(string clzl)
        {
            if (!string.IsNullOrEmpty(clzl))
            {
                if (clzl.IndexOf("M1") > -1)
                {
                    clzl = "乘用车（M1）";
                }
                else if (clzl.IndexOf("M2") > -1)
                {
                    clzl = "轻型客车（M2）";
                }
                else if (clzl.IndexOf("N1") > -1)
                {
                    clzl = "轻型货车（N1）";
                }
            }
            return clzl;
        }

        // 提取并格式化通用名称
        public string FormatTymc(string tymc)
        {
            string tymcRe = string.Empty;
            if (!string.IsNullOrEmpty(tymc))
            {
                //tymc = tymc.Substring(tymc.LastIndexOf("/") + 1);
                string[] tymcArr = tymc.Split(':');
                string tymcTemp = tymcArr[tymcArr.Length - 1].Trim();
                tymc = tymcTemp.Trim().Substring(0, tymcTemp.LastIndexOf("/"));

                switch (tymc)
                {
                    case "X150":
                        tymcRe = "XK";
                        break;
                    case "X250":
                        tymcRe = "XF";
                        break;
                    case "X351":
                        tymcRe = "XJ";
                        break;
                    case "L319":
                        tymcRe = "发现";
                        break;
                    case "L320":
                        tymcRe = "揽胜运动";
                        break;
                    case "L322":
                        tymcRe = "揽胜";
                        break;
                    case "L359":
                        tymcRe = "神行者";
                        break;
                    case "L538":
                        tymcRe = "极光";
                        break;
                    case "L405":
                        tymcRe = "揽胜";
                        break;
                    case "L494":
                        tymcRe = "揽胜运动";
                        break;
                    default: tymcRe = tymc;
                        break;
                }
            }
            return tymcRe;
        }

        // 提取并格式化越野车
        public string FormatYyc(string yyc)
        {
            string isYyc = string.Empty;
            if (!string.IsNullOrEmpty(yyc))
            {
                if (yyc.IndexOf("G") > -1)
                {
                    isYyc = "是";
                }
                else
                {
                    isYyc = "否";
                }
            }
            return isYyc;
        }

        // 提取并格式化乘用车生产企业
        public string FormatQcscqy(string qcscqy)
        {
            if (!string.IsNullOrEmpty(qcscqy))
            {
                int indexStr = qcscqy.IndexOf(':');
                if (indexStr > -1 && indexStr < qcscqy.Length - 1)
                {
                    qcscqy = qcscqy.Substring(indexStr + 1).Trim();
                }
            }
            return qcscqy;
        }

        // 燃料类型
        public string FormatRllx(string rllx)
        {
            if (rllx.ToUpper() == "DIESEL")
            {
                rllx = "柴油";
            }
            if (rllx.ToUpper() == "PETROL")
            {
                rllx = "汽油";
            }
            return rllx;
        }

        // 提取并格式化座椅排数
        public string FormatZwps(string zwps)
        {
            int iZwps = 0;
            if (!string.IsNullOrEmpty(zwps))
            {
                char[] cZwps = zwps.ToCharArray();
                for (int i = 0; i < cZwps.Length; i++)
                {
                    if ("0123456789".IndexOf(cZwps[i]) > -1)
                    {
                        iZwps++;
                    }
                }
            }
            return iZwps.ToString();
        }

        // 提取并格式化整车装备质量
        public string FormatZczbzl(string zczbzl)
        {
            if (!string.IsNullOrEmpty(zczbzl))
            {
                zczbzl = (Int32.Parse(zczbzl) - 75).ToString();
            }
            return zczbzl;
        }

        // 提取并格式化轮胎规格
        public string FormatLtgg(string ltgg)
        {
            if (!string.IsNullOrEmpty(ltgg))
            {
                if (ltgg.IndexOf("第2轴：") > -1)
                {
                    ltgg = ltgg.Replace("第2轴：", ")/(") + ")";
                    if (ltgg.IndexOf("第1轴：") > -1)
                    {
                        ltgg = ltgg.Replace("第1轴：", "(");
                    }
                }
                else if (ltgg.IndexOf("第1轴：") > -1)
                {
                    ltgg = ltgg.Replace("第1轴：", "");
                }
            }
            return ltgg;
        }

        // 提取并格式化驱动型式
        public string FormatQdxs(string qdxs)
        {
            string qdxsStr = string.Empty;
            if (!string.IsNullOrEmpty(qdxs))
            {
                if (qdxs.Contains("第1轴") && !qdxs.Contains("第2轴"))
                {
                    qdxsStr = "前轮驱动";
                }
                else if (!qdxs.Contains("第1轴") && qdxs.Contains("第2轴"))
                {
                    qdxsStr = "后轮驱动";
                }
                else if (qdxs.Contains("第1轴") && qdxs.Contains("第2轴"))
                {
                    qdxsStr = "全时全轮驱动";
                }
            }
            return qdxsStr;
        }

        // 提取并格式化变速器挡位数
        public string FormatBsqdws(string bsqdws)
        {
            string bsqdwsNum = string.Empty;
            if (!string.IsNullOrEmpty(bsqdws))
            {
                string[] str = bsqdws.Split('/');
                bsqdwsNum = (str.Length - 1).ToString();
            }
            return bsqdwsNum;
        }

        // 提取并格式化变速器型式
        public string FormatBsqxs(string bsqxs)
        {
            string bsqxsStr = string.Empty;
            if (!string.IsNullOrEmpty(bsqxs))
            {
                switch (bsqxs)
                {
                    case "自动":
                        bsqxsStr = "AT";
                        break;
                    case "手动":
                        bsqxsStr = "MT";
                        break;
                    default: bsqxsStr = "其它";
                        break;
                }
            }
            return bsqxsStr;
        }

        // 保留一位小数
        public string FormatDecimal(string decNum)
        {
            if (!string.IsNullOrEmpty(decNum))
            {
                decNum = Math.Round(Double.Parse(decNum), 1).ToString("0.0");
            }
            return decNum;
        }

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

        #region 杀死进程
        private void KillProcess(string processName)
        {
            //获得进程对象，以用来操作  
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();
            //得到所有打开的进程   
            try
            {
                //获得需要杀死的进程名  
                foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName(processName))
                {
                    //立即杀死进程  
                    thisproc.Kill();
                }
            }
            catch (Exception Exc)
            {
                throw new Exception("", Exc);
            }
        }
        #endregion



        #region 临时功能：补充导入发动机生产厂字段

        //temp
        public string ReadCtnyCOCExcelFDJ(string fileName, string folderName, string importType, List<string> cocIdList)
        {
            //string strConn = String.Format("PROVIDER=MICROSOFT.ACE.OLEDB.12.0;DATA SOURCE={0}; EXTENDED PROPERTIES='EXCEL 12.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            //DataSet ds = new DataSet();

            Microsoft.Office.Interop.Excel.Application xApp = new Microsoft.Office.Interop.Excel.Application();
            Workbook xBook = xApp.Workbooks.Open(fileName,
                        Type.Missing, Missing.Value, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                        Missing.Value, Missing.Value);
            Worksheet xSheet = (Worksheet)xBook.Sheets["Sheet1"];
            Range range = null;

            VehicleBasicInfo basicInfo = new VehicleBasicInfo();
            CtnyRllx ctnyInfo = new CtnyRllx();
            string cocId = string.Empty;
            string readMsg = string.Empty;
            string paramMsg = string.Empty;

            try
            {
                range = xSheet.get_Range(FUEL_DATA["cocNo"], Type.Missing);
                cocId = this.FormatCocId(range.Value2 == null ? "" : range.Value2.ToString().Trim());
                paramMsg += Utils.VerifyRequired("COC编号", cocId);

                range = xSheet.get_Range(FUEL_DATA["fdjscc"], Type.Missing);
                ctnyInfo.Gh_Fdjscc = range.Value2 == null ? "" : range.Value2.ToString().Trim();
                paramMsg += Utils.VerifyRequired("发动机生产厂", ctnyInfo.Gh_Fdjscc);
            }
            catch (Exception ex)
            {
                readMsg = ex.Message + "\r\n";
            }
            finally
            {
                xBook.Close();

                Kill(xApp);

            }

            if (!string.IsNullOrEmpty(readMsg))
            {
                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, readMsg);
            }

            if (!string.IsNullOrEmpty(paramMsg))
            {
                return string.Format("文件“{0}”导入失败\r\n失败原因：\r\n {1}", fileName, paramMsg);
            }

            string saveMsg = string.Empty;
            if (importType == "IMPORT")
            {
                //saveMsg = this.SaveCtnyCocInfo(cocId, basicInfo, ctnyInfo);
            }
            else if (importType == "UPDATE")
            {
                saveMsg = this.UpdateCtnyCocInfoFDJ(cocId, basicInfo, ctnyInfo, cocIdList);
            }

            if (string.IsNullOrEmpty(saveMsg))
            {
                // 移动文件
                string templateFileName = string.Empty;
                if (importType == "IMPORT")
                {
                    templateFileName = "F_COC";
                }
                else if (importType == "UPDATE")
                {
                    templateFileName = "U_COC";
                }
                this.MoveFinishedFile(fileName, folderName, templateFileName);
            }
            if (string.IsNullOrEmpty(saveMsg))
            {
                return "\r\n" + fileName + " 操作成功\r\n";
            }
            return saveMsg;
        }

        public string UpdateCtnyCocInfoFDJ(string cocId, VehicleBasicInfo basicInfo, CtnyRllx ctnyInfo, List<string> cocIdList)
        {
            string msg = string.Empty;

            if (!this.IsCocIdExist(cocId))
            {
                return cocId + " 不存在，请直接导入";
            }

            string strCon = AccessHelper.conn;
            OleDbConnection con = new OleDbConnection(strCon);
            con.Open();//创建事务，开始执行事务
            try
            {
                string sqlStr = @"UPDATE COC_INFO SET UPDATETIME=@UPDATETIME,GH_FDJSCC=@GH_FDJSCC
                                          WHERE COC_ID=@COC_ID";

                OleDbParameter updateTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                updateTime.OleDbType = OleDbType.DBDate;
                OleDbParameter[] paramList = { 
                                        updateTime,
                                        new OleDbParameter("@GH_FDJSCC",ctnyInfo.Gh_Fdjscc),
                                        new OleDbParameter("@COC_ID",cocId),
                                   };
                AccessHelper.ExecuteNonQuery(con, sqlStr, paramList);
                cocIdList.Add(cocId);
            }
            catch (Exception ex)
            {
                msg = cocId + " 修改失败:" + ex.Message;
            }
            finally
            {
                con.Close();
            }

            return msg;
        }

        #endregion
    }
}
