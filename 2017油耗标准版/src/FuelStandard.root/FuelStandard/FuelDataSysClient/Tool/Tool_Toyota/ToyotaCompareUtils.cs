using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FuelDataSysClient.Properties;
using System.Data;
using System.Data.OleDb;

namespace FuelDataSysClient.Tool.Tool_Toyota
{
    public class ToyotaCompareUtils
    {
        public static string CTNY = "传统能源";
        public static string FCDSHHDL = "非插电式混合动力";

        public Dictionary<string, string> dictCTNY;  //存放列头转换模板(传统能源)
        public Dictionary<string, string> dictFCDSHHDL;  //存放列头转换模板（非插电式混合动力）

        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate_ToyotaCompare"];

        public ToyotaCompareUtils()
        {
            ReadTemplate(path);   //读取表头转置模板
        }

        private void ReadTemplate(string filePath)
        {
            DataSet ds = this.ReadTemplateExcel(filePath);
            dictCTNY = new Dictionary<string, string>();
            dictFCDSHHDL = new Dictionary<string, string>();

            foreach (DataRow r in ds.Tables[CTNY].Rows)
            {
                dictCTNY.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            foreach (DataRow r in ds.Tables[FCDSHHDL].Rows)
            {
                dictFCDSHHDL.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }
        }

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
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
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
                    if (dc.ColumnName.Equals(kv.Key))
                    {
                        dc.ColumnName = kv.Value;
                        break;
                    }
                }
            }
            return dt;
        }
    }
}
