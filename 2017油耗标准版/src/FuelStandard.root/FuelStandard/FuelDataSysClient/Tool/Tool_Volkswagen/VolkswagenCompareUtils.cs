using FuelDataSysClient.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FuelDataSysClient.Tool.Tool_Volkswagen
{
    public class VolkswagenCompareUtils
    {
        public static string CTNY = "传统能源";
        public static string FCDSHHDL = "非插电式混合动力";
        public static string CDSHHDL = "插电式混合动力";
        public static string CDD = "纯电动";
        public static string RLDC = "燃料电池";

        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate_Volkswagen"];

        public VolkswagenCompareUtils()
        {

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
        /// 修改列名
        /// </summary>
        /// <param name="ds">需要修改的DataSet</param>
        /// <returns></returns>
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
        /// 读取Excel并合并到DataTable返回到系统
        /// </summary>
        /// <param name="filePath">Excel路径</param>
        /// <param name="tableName">燃料类型</param>
        /// <returns></returns>
        public DataTable ReadBackToSYS(string filePath, string tableName)
        {
            DataSet ds = SwitchColumnName(ReadExcel(filePath));
            // STEP1：导入系统，拼接数据
            if (tableName.Equals("传统能源"))
            {
                var CT = from CLJBXX in ds.Tables["基本信息"].AsEnumerable()
                         join RLLXPARAM in ds.Tables["传统能源"].AsEnumerable()
                         on CLJBXX.Field<string>("VIN") equals RLLXPARAM.Field<string>("VIN")
                         select new
                         {
                             VIN = CLJBXX.Field<string>("VIN"),
                             CLZZRQ = CLJBXX.Field<string>("CLZZRQ") ?? string.Empty,
                             HGSPBM = CLJBXX.Field<string>("HGSPBM") ?? string.Empty,
                             QTXX = CLJBXX.Field<string>("QTXX") ?? string.Empty,
                             CLXH = CLJBXX.Field<string>("CLXH") ?? string.Empty,
                             CLZL = CLJBXX.Field<string>("CLZL") ?? string.Empty,
                             EDZK = CLJBXX.Field<string>("EDZK") ?? string.Empty,
                             JKQCZJXS = CLJBXX.Field<string>("JKQCZJXS") ?? string.Empty,
                             JYBGBH = (string)CLJBXX.Field<string>("JYBGBH") ?? string.Empty,
                             JYJGMC = CLJBXX.Field<string>("JYJGMC") ?? string.Empty,
                             LJ = (string)CLJBXX.Field<string>("LJ") ?? string.Empty,
                             LTGG = CLJBXX.Field<string>("LTGG") ?? string.Empty,
                             QCSCQY = CLJBXX.Field<string>("QCSCQY") ?? string.Empty,
                             QDXS = CLJBXX.Field<string>("QDXS") ?? string.Empty,
                             RLLX = CLJBXX.Field<string>("RLLX") ?? string.Empty,
                             USER_ID = CLJBXX.Field<string>("USER_ID") ?? string.Empty,
                             TYMC = (string)CLJBXX.Field<string>("TYMC") ?? string.Empty,
                             YYC = CLJBXX.Field<string>("YYC") ?? string.Empty,
                             ZCZBZL = CLJBXX.Field<string>("ZCZBZL") ?? string.Empty,
                             ZDSJZZL = CLJBXX.Field<string>("ZDSJZZL") ?? string.Empty,
                             ZGCS = CLJBXX.Field<string>("ZGCS") ?? string.Empty,
                             ZJ = CLJBXX.Field<string>("ZJ") ?? string.Empty,
                             ZWPS = CLJBXX.Field<string>("ZWPS") ?? string.Empty,
                             CT_BSQDWS = RLLXPARAM.Field<string>("CT_BSQDWS") ?? string.Empty,
                             CT_BSQXS = (string)RLLXPARAM.Field<string>("CT_BSQXS") ?? string.Empty,
                             CT_EDGL = RLLXPARAM.Field<string>("CT_EDGL") ?? string.Empty,
                             CT_FDJXH = RLLXPARAM.Field<string>("CT_FDJXH") ?? string.Empty,
                             CT_JGL = RLLXPARAM.Field<string>("CT_JGL") ?? string.Empty,
                             CT_PL = RLLXPARAM.Field<string>("CT_PL") ?? string.Empty,
                             CT_QCJNJS = RLLXPARAM.Field<string>("CT_QCJNJS") ?? string.Empty,
                             CT_QGS = RLLXPARAM.Field<string>("CT_QGS") ?? string.Empty,
                             CT_QTXX = RLLXPARAM.Field<string>("CT_QTXX") ?? string.Empty,
                             CT_SJGKRLXHL = RLLXPARAM.Field<string>("CT_SJGKRLXHL") ?? string.Empty,
                             CT_SQGKRLXHL = RLLXPARAM.Field<string>("CT_SQGKRLXHL") ?? string.Empty,
                             CT_ZHGKCO2PFL = RLLXPARAM.Field<string>("CT_ZHGKCO2PFL") ?? string.Empty,
                             CT_ZHGKRLXHL = RLLXPARAM.Field<string>("CT_ZHGKRLXHL") ?? string.Empty,
                         };
                return ObjectReflect.ToDataTable(CT);
            }
            if (tableName.Equals("非插电式混合动力"))
            {
                var FCDS_HHDL = from CLJBXX in ds.Tables["基本信息"].AsEnumerable()
                                join RLLXPARAM in ds.Tables["非插电式混合动力"].AsEnumerable()
                                on CLJBXX.Field<string>("VIN") equals RLLXPARAM.Field<string>("VIN")
                                select new
                                {
                                    VIN = CLJBXX.Field<string>("VIN"),
                                    CLZZRQ = CLJBXX.Field<string>("CLZZRQ") ?? string.Empty,
                                    HGSPBM = CLJBXX.Field<string>("HGSPBM") ?? string.Empty,
                                    QTXX = CLJBXX.Field<string>("QTXX") ?? string.Empty,
                                    CLXH = CLJBXX.Field<string>("CLXH") ?? string.Empty,
                                    CLZL = CLJBXX.Field<string>("CLZL") ?? string.Empty,
                                    EDZK = CLJBXX.Field<string>("EDZK") ?? string.Empty,
                                    JKQCZJXS = CLJBXX.Field<string>("JKQCZJXS") ?? string.Empty,
                                    JYBGBH = CLJBXX.Field<string>("JYBGBH") ?? string.Empty,
                                    JYJGMC = CLJBXX.Field<string>("JYJGMC") ?? string.Empty,
                                    LJ = CLJBXX.Field<string>("LJ") ?? string.Empty,
                                    LTGG = CLJBXX.Field<string>("LTGG") ?? string.Empty,
                                    QCSCQY = CLJBXX.Field<string>("QCSCQY") ?? string.Empty,
                                    QDXS = CLJBXX.Field<string>("QDXS") ?? string.Empty,
                                    RLLX = CLJBXX.Field<string>("RLLX") ?? string.Empty,
                                    USER_ID = CLJBXX.Field<string>("USER_ID") ?? string.Empty,
                                    TYMC = CLJBXX.Field<string>("TYMC") ?? string.Empty,
                                    YYC = CLJBXX.Field<string>("YYC") ?? string.Empty,
                                    ZCZBZL = CLJBXX.Field<string>("ZCZBZL") ?? string.Empty,
                                    ZDSJZZL = CLJBXX.Field<string>("ZDSJZZL") ?? string.Empty,
                                    ZGCS = CLJBXX.Field<string>("ZGCS") ?? string.Empty,
                                    ZJ = CLJBXX.Field<string>("ZJ") ?? string.Empty,
                                    ZWPS = CLJBXX.Field<string>("ZWPS") ?? string.Empty,
                                    FCDS_HHDL_BSQDWS = RLLXPARAM.Field<string>("FCDS_HHDL_BSQDWS") ?? string.Empty,
                                    FCDS_HHDL_BSQXS = RLLXPARAM.Field<string>("FCDS_HHDL_BSQXS") ?? string.Empty,
                                    FCDS_HHDL_CDDMSXZGCS = RLLXPARAM.Field<string>("FCDS_HHDL_CDDMSXZGCS") ?? string.Empty,
                                    FCDS_HHDL_CDDMSXZHGKXSLC = RLLXPARAM.Field<string>("FCDS_HHDL_CDDMSXZHGKXSLC") ?? string.Empty,
                                    FCDS_HHDL_DLXDCBNL = RLLXPARAM.Field<string>("FCDS_HHDL_DLXDCBNL") ?? string.Empty,
                                    FCDS_HHDL_DLXDCZBCDY = RLLXPARAM.Field<string>("FCDS_HHDL_DLXDCZBCDY") ?? string.Empty,
                                    FCDS_HHDL_DLXDCZZL = RLLXPARAM.Field<string>("FCDS_HHDL_DLXDCZZL") ?? string.Empty,
                                    FCDS_HHDL_DLXDCZZNL = RLLXPARAM.Field<string>("FCDS_HHDL_DLXDCZZNL") ?? string.Empty,
                                    FCDS_HHDL_EDGL = RLLXPARAM.Field<string>("FCDS_HHDL_EDGL") ?? string.Empty,
                                    FCDS_HHDL_FDJXH = RLLXPARAM.Field<string>("FCDS_HHDL_FDJXH") ?? string.Empty,
                                    FCDS_HHDL_HHDLJGXS = RLLXPARAM.Field<string>("FCDS_HHDL_HHDLJGXS") ?? string.Empty,
                                    FCDS_HHDL_HHDLZDDGLB = RLLXPARAM.Field<string>("FCDS_HHDL_HHDLZDDGLB") ?? string.Empty,
                                    FCDS_HHDL_JGL = RLLXPARAM.Field<string>("FCDS_HHDL_JGL") ?? string.Empty,
                                    FCDS_HHDL_PL = RLLXPARAM.Field<string>("FCDS_HHDL_PL") ?? string.Empty,
                                    FCDS_HHDL_QCJNJS = RLLXPARAM.Field<string>("FCDS_HHDL_QCJNJS") ?? string.Empty,
                                    FCDS_HHDL_QDDJEDGL = RLLXPARAM.Field<string>("FCDS_HHDL_QDDJEDGL") ?? string.Empty,
                                    FCDS_HHDL_QDDJFZNJ = RLLXPARAM.Field<string>("FCDS_HHDL_QDDJFZNJ") ?? string.Empty,
                                    FCDS_HHDL_QDDJLX = RLLXPARAM.Field<string>("FCDS_HHDL_QDDJLX") ?? string.Empty,
                                    FCDS_HHDL_QGS = RLLXPARAM.Field<string>("FCDS_HHDL_QGS") ?? string.Empty,
                                    FCDS_HHDL_SJGKRLXHL = RLLXPARAM.Field<string>("FCDS_HHDL_SJGKRLXHL") ?? string.Empty,
                                    FCDS_HHDL_SQGKRLXHL = RLLXPARAM.Field<string>("FCDS_HHDL_SQGKRLXHL") ?? string.Empty,
                                    FCDS_HHDL_XSMSSDXZGN = RLLXPARAM.Field<string>("FCDS_HHDL_XSMSSDXZGN") ?? string.Empty,
                                    FCDS_HHDL_ZHGKRLXHL = RLLXPARAM.Field<string>("FCDS_HHDL_ZHGKRLXHL") ?? string.Empty,
                                    FCDS_HHDL_ZHKGCO2PL = RLLXPARAM.Field<string>("FCDS_HHDL_ZHKGCO2PL") ?? string.Empty,
                                };
                return ObjectReflect.ToDataTable(FCDS_HHDL);
            }
            if (tableName.Equals("插电式混合动力"))
            {
                var CDS_HHDL = from CLJBXX in ds.Tables["基本信息"].AsEnumerable()
                               join RLLXPARAM in ds.Tables["插电式混合动力"].AsEnumerable()
                               on CLJBXX.Field<string>("VIN") equals RLLXPARAM.Field<string>("VIN")
                               select new
                               {
                                   VIN = CLJBXX.Field<string>("VIN"),
                                   CLZZRQ = CLJBXX.Field<string>("CLZZRQ") ?? string.Empty,
                                   HGSPBM = CLJBXX.Field<string>("HGSPBM") ?? string.Empty,
                                   QTXX = CLJBXX.Field<string>("QTXX") ?? string.Empty,
                                   CLXH = CLJBXX.Field<string>("CLXH") ?? string.Empty,
                                   CLZL = CLJBXX.Field<string>("CLZL") ?? string.Empty,
                                   EDZK = CLJBXX.Field<string>("EDZK").ToString() ?? string.Empty,
                                   JKQCZJXS = CLJBXX.Field<string>("JKQCZJXS") ?? string.Empty,
                                   JYBGBH = CLJBXX.Field<string>("JYBGBH") ?? string.Empty,
                                   JYJGMC = CLJBXX.Field<string>("JYJGMC") ?? string.Empty,
                                   LJ = CLJBXX.Field<string>("LJ") ?? string.Empty,
                                   LTGG = CLJBXX.Field<string>("LTGG") ?? string.Empty,
                                   QCSCQY = CLJBXX.Field<string>("QCSCQY") ?? string.Empty,
                                   QDXS = CLJBXX.Field<string>("QDXS") ?? string.Empty,
                                   RLLX = CLJBXX.Field<string>("RLLX") ?? string.Empty,
                                   USER_ID = CLJBXX.Field<string>("USER_ID") ?? string.Empty,
                                   TYMC = CLJBXX.Field<string>("TYMC") ?? string.Empty,
                                   YYC = CLJBXX.Field<string>("YYC") ?? string.Empty,
                                   ZCZBZL = CLJBXX.Field<string>("ZCZBZL") ?? string.Empty,
                                   ZDSJZZL = CLJBXX.Field<string>("ZDSJZZL") ?? string.Empty,
                                   ZGCS = CLJBXX.Field<string>("ZGCS") ?? string.Empty,
                                   ZJ = CLJBXX.Field<string>("ZJ") ?? string.Empty,
                                   ZWPS = CLJBXX.Field<string>("ZWPS") ?? string.Empty,
                                   CDS_HHDL_BSQDWS = RLLXPARAM.Field<string>("CDS_HHDL_BSQDWS") ?? string.Empty,
                                   CDS_HHDL_BSQXS = RLLXPARAM.Field<string>("CDS_HHDL_BSQXS") ?? string.Empty,
                                   CDS_HHDL_CDDMSXZGCS = RLLXPARAM.Field<string>("CDS_HHDL_CDDMSXZGCS") ?? string.Empty,
                                   CDS_HHDL_CDDMSXZHGKXSLC = RLLXPARAM.Field<string>("CDS_HHDL_CDDMSXZHGKXSLC") ?? string.Empty,
                                   CDS_HHDL_DLXDCBNL = RLLXPARAM.Field<string>("CDS_HHDL_DLXDCBNL") ?? string.Empty,
                                   CDS_HHDL_DLXDCZBCDY = RLLXPARAM.Field<string>("CDS_HHDL_DLXDCZBCDY") ?? string.Empty,
                                   CDS_HHDL_DLXDCZZL = RLLXPARAM.Field<string>("CDS_HHDL_DLXDCZZL") ?? string.Empty,
                                   CDS_HHDL_DLXDCZZNL = RLLXPARAM.Field<string>("CDS_HHDL_DLXDCZZNL") ?? string.Empty,
                                   CDS_HHDL_EDGL = RLLXPARAM.Field<string>("CDS_HHDL_EDGL") ?? string.Empty,
                                   CDS_HHDL_FDJXH = RLLXPARAM.Field<string>("CDS_HHDL_FDJXH") ?? string.Empty,
                                   CDS_HHDL_HHDLJGXS = RLLXPARAM.Field<string>("CDS_HHDL_HHDLJGXS") ?? string.Empty,
                                   CDS_HHDL_HHDLZDDGLB = RLLXPARAM.Field<string>("CDS_HHDL_HHDLZDDGLB") ?? string.Empty,
                                   CDS_HHDL_JGL = RLLXPARAM.Field<string>("CDS_HHDL_JGL") ?? string.Empty,
                                   CDS_HHDL_PL = RLLXPARAM.Field<string>("CDS_HHDL_PL") ?? string.Empty,
                                   CDS_HHDL_QDDJEDGL = RLLXPARAM.Field<string>("CDS_HHDL_QDDJEDGL") ?? string.Empty,
                                   CDS_HHDL_QDDJFZNJ = RLLXPARAM.Field<string>("CDS_HHDL_QDDJFZNJ") ?? string.Empty,
                                   CDS_HHDL_QDDJLX = RLLXPARAM.Field<string>("CDS_HHDL_QDDJLX") ?? string.Empty,
                                   CDS_HHDL_QGS = RLLXPARAM.Field<string>("CDS_HHDL_QGS") ?? string.Empty,
                                   CDS_HHDL_SJGKRLXHL = RLLXPARAM.Field<string>("CDS_HHDL_SJGKRLXHL") ?? string.Empty,
                                   CDS_HHDL_SQGKRLXHL = RLLXPARAM.Field<string>("CDS_HHDL_SQGKRLXHL") ?? string.Empty,
                                   CDS_HHDL_XSMSSDXZGN = RLLXPARAM.Field<string>("CDS_HHDL_XSMSSDXZGN") ?? string.Empty,
                                   CDS_HHDL_ZHGKDNXHL = RLLXPARAM.Field<string>("CDS_HHDL_ZHGKDNXHL") ?? string.Empty,
                                   CDS_HHDL_ZHGKRLXHL = RLLXPARAM.Field<string>("CDS_HHDL_ZHGKRLXHL") ?? string.Empty,
                                   CDS_HHDL_ZHKGCO2PL = RLLXPARAM.Field<string>("CDS_HHDL_ZHKGCO2PL") ?? string.Empty,
                               };
                return ObjectReflect.ToDataTable(CDS_HHDL);
            }
            if (tableName.Equals("纯电动"))
            {
                var CDD = from CLJBXX in ds.Tables["基本信息"].AsEnumerable()
                          join RLLXPARAM in ds.Tables["纯电动"].AsEnumerable()
                          on CLJBXX.Field<string>("VIN") equals RLLXPARAM.Field<string>("VIN")
                          select new
                          {
                              VIN = CLJBXX.Field<string>("VIN"),
                              CLZZRQ = CLJBXX.Field<string>("CLZZRQ") ?? string.Empty,
                              HGSPBM = CLJBXX.Field<string>("HGSPBM") ?? string.Empty,
                              QTXX = CLJBXX.Field<string>("QTXX") ?? string.Empty,
                              CLXH = CLJBXX.Field<string>("CLXH") ?? string.Empty,
                              CLZL = CLJBXX.Field<string>("CLZL") ?? string.Empty,
                              EDZK = CLJBXX.Field<string>("EDZK") ?? string.Empty,
                              JKQCZJXS = CLJBXX.Field<string>("JKQCZJXS") ?? string.Empty,
                              JYBGBH = CLJBXX.Field<string>("JYBGBH") ?? string.Empty,
                              JYJGMC = CLJBXX.Field<string>("JYJGMC") ?? string.Empty,
                              LJ = CLJBXX.Field<string>("LJ") ?? string.Empty,
                              LTGG = CLJBXX.Field<string>("LTGG") ?? string.Empty,
                              QCSCQY = CLJBXX.Field<string>("QCSCQY") ?? string.Empty,
                              QDXS = CLJBXX.Field<string>("QDXS") ?? string.Empty,
                              RLLX = CLJBXX.Field<string>("RLLX") ?? string.Empty,
                              USER_ID = CLJBXX.Field<string>("USER_ID") ?? string.Empty,
                              TYMC = CLJBXX.Field<string>("TYMC") ?? string.Empty,
                              YYC = CLJBXX.Field<string>("YYC") ?? string.Empty,
                              ZCZBZL = CLJBXX.Field<string>("ZCZBZL") ?? string.Empty,
                              ZDSJZZL = CLJBXX.Field<string>("ZDSJZZL") ?? string.Empty,
                              ZGCS = CLJBXX.Field<string>("ZGCS") ?? string.Empty,
                              ZJ = CLJBXX.Field<string>("ZJ") ?? string.Empty,
                              ZWPS = CLJBXX.Field<string>("ZWPS") ?? string.Empty,
                              CDD_DDQC30FZZGCS = RLLXPARAM.Field<string>("CDD_DDQC30FZZGCS") ?? string.Empty,
                              CDD_DDXDCZZLYZCZBZLDBZ = RLLXPARAM.Field<string>("CDD_DDXDCZZLYZCZBZLDBZ") ?? string.Empty,
                              CDD_DLXDCBNL = RLLXPARAM.Field<string>("CDD_DLXDCBNL") ?? string.Empty,
                              CDD_DLXDCZBCDY = RLLXPARAM.Field<string>("CDD_DLXDCZBCDY") ?? string.Empty,
                              CDD_DLXDCZEDNL = RLLXPARAM.Field<string>("CDD_DLXDCZEDNL") ?? string.Empty,
                              CDD_DLXDCZZL = RLLXPARAM.Field<string>("CDD_DLXDCZZL") ?? string.Empty,
                              CDD_QDDJEDGL = RLLXPARAM.Field<string>("CDD_QDDJEDGL") ?? string.Empty,
                              CDD_QDDJFZNJ = RLLXPARAM.Field<string>("CDD_QDDJFZNJ") ?? string.Empty,
                              CDD_QDDJLX = RLLXPARAM.Field<string>("CDD_QDDJLX") ?? string.Empty,
                              CDD_ZHGKDNXHL = RLLXPARAM.Field<string>("CDD_ZHGKDNXHL") ?? string.Empty,
                              CDD_ZHGKXSLC = RLLXPARAM.Field<string>("CDD_ZHGKXSLC") ?? string.Empty,
                          };
                return ObjectReflect.ToDataTable(CDD);
            }
            if (tableName.Equals("燃料电池"))
            {
                var RLDC = from CLJBXX in ds.Tables["基本信息"].AsEnumerable()
                           join RLLXPARAM in ds.Tables["燃料电池"].AsEnumerable()
                           on CLJBXX.Field<string>("VIN") equals RLLXPARAM.Field<string>("VIN")
                           select new
                           {
                               VIN = CLJBXX.Field<string>("VIN"),
                               CLZZRQ = CLJBXX.Field<string>("CLZZRQ") ?? string.Empty,
                               HGSPBM = CLJBXX.Field<string>("HGSPBM") ?? string.Empty,
                               QTXX = CLJBXX.Field<string>("QTXX") ?? string.Empty,
                               CLXH = CLJBXX.Field<string>("CLXH") ?? string.Empty,
                               CLZL = CLJBXX.Field<string>("CLZL") ?? string.Empty,
                               EDZK = CLJBXX.Field<string>("EDZK") ?? string.Empty,
                               JKQCZJXS = CLJBXX.Field<string>("JKQCZJXS") ?? string.Empty,
                               JYBGBH = CLJBXX.Field<string>("JYBGBH") ?? string.Empty,
                               JYJGMC = CLJBXX.Field<string>("JYJGMC") ?? string.Empty,
                               LJ = CLJBXX.Field<string>("LJ") ?? string.Empty,
                               LTGG = CLJBXX.Field<string>("LTGG") ?? string.Empty,
                               QCSCQY = CLJBXX.Field<string>("QCSCQY") ?? string.Empty,
                               QDXS = CLJBXX.Field<string>("QDXS") ?? string.Empty,
                               RLLX = CLJBXX.Field<string>("RLLX") ?? string.Empty,
                               USER_ID = CLJBXX.Field<string>("USER_ID") ?? string.Empty,
                               TYMC = CLJBXX.Field<string>("TYMC") ?? string.Empty,
                               YYC = CLJBXX.Field<string>("YYC") ?? string.Empty,
                               ZCZBZL = CLJBXX.Field<string>("ZCZBZL") ?? string.Empty,
                               ZDSJZZL = CLJBXX.Field<string>("ZDSJZZL") ?? string.Empty,
                               ZGCS = CLJBXX.Field<string>("ZGCS") ?? string.Empty,
                               ZJ = CLJBXX.Field<string>("ZJ") ?? string.Empty,
                               ZWPS = CLJBXX.Field<string>("ZWPS") ?? string.Empty,
                               RLDC_CDDMSXZGXSCS = RLLXPARAM.Field<string>("RLDC_CDDMSXZGXSCS") ?? string.Empty,
                               RLDC_CQPBCGZYL = RLLXPARAM.Field<string>("RLDC_CQPBCGZYL") ?? string.Empty,
                               RLDC_CQPLX = RLLXPARAM.Field<string>("RLDC_CQPLX") ?? string.Empty,
                               RLDC_CQPRJ = RLLXPARAM.Field<string>("RLDC_CQPRJ") ?? string.Empty,
                               RLDC_DDGLMD = RLLXPARAM.Field<string>("RLDC_DDGLMD") ?? string.Empty,
                               RLDC_DDHHJSTJXXDCZBNL = RLLXPARAM.Field<string>("RLDC_DDHHJSTJXXDCZBNL") ?? string.Empty,
                               RLDC_DLXDCZZL = RLLXPARAM.Field<string>("RLDC_DLXDCZZL") ?? string.Empty,
                               RLDC_QDDJEDGL = RLLXPARAM.Field<string>("RLDC_QDDJEDGL") ?? string.Empty,
                               RLDC_QDDJFZNJ = RLLXPARAM.Field<string>("RLDC_QDDJFZNJ") ?? string.Empty,
                               RLDC_QDDJLX = RLLXPARAM.Field<string>("RLDC_QDDJLX") ?? string.Empty,
                               RLDC_RLLX = RLLXPARAM.Field<string>("RLDC_RLLX") ?? string.Empty,
                               RLDC_ZHGKHQL = RLLXPARAM.Field<string>("RLDC_ZHGKHQL") ?? string.Empty,
                               RLDC_ZHGKXSLC = RLLXPARAM.Field<string>("RLDC_ZHGKXSLC") ?? string.Empty,
                           };
                return ObjectReflect.ToDataTable(RLDC);
            }
            return null;
        }
    }
}
