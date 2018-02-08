using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using FuelDataSysClient.Properties;
using Common;

namespace FuelDataSysClient.Tool
{
    public class ImportExcel
    {
        /// <summary>  
        /// 导入Excel到DataSet中  
        /// </summary>  
        /// <param name="strFileUrl">文件的路径和文件全名，含扩展名</param>  
        /// <returns></returns>  
        public static DataSet ReadExcelToDataSet(string strFileUrl)
        {

            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", strFileUrl);
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
                DataTableHelper.removeEmpty(ds);
                return ds;
            }
        }

        /// <summary>
        /// 修改整车基础数据dataset的表头
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <returns></returns>
        public static DataTable SwitchCLJBXXColumnName(DataSet ds)
        {
            using (DataSet dsTemp = ImportExcel.ReadExcelToDataSet(Application.StartupPath + Settings.Default["ExcelHeaderTemplate_KIA"]))
            {
                Dictionary<string, string> dictCLJBXX = new Dictionary<string, string>();
                foreach (DataRow r in dsTemp.Tables["整车基础数据"].Rows)
                {
                    dictCLJBXX.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                //避免用户的excel缺少表，或者表名错误，采用for循环判断
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].TableName.Equals("TEMPLATE"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictCLJBXX.ContainsKey(dc.ColumnName))
                            {
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictCLJBXX[dc.ColumnName];
                        }
                    }
                }
            }
            return ds.Tables.Contains("TEMPLATE") == true ? ds.Tables["TEMPLATE"] : null;
        }

        /// <summary>
        /// 修改燃料参数维护dataset的表头
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <returns></returns>
        public static DataTable SwitchRLLXPARAMColumnName(DataSet ds)
        {
            using (DataSet dsTemp = ImportExcel.ReadExcelToDataSet(Application.StartupPath + Settings.Default["ExcelHeaderTemplate_KIA"]))
            {
                Dictionary<string, string> dictRLLXPARAM = new Dictionary<string, string>();
                foreach (DataRow r in dsTemp.Tables["燃料参数维护"].Rows)
                {
                    dictRLLXPARAM.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
                }
                //避免用户的excel缺少表，或者表名错误，采用for循环判断
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].TableName.Equals("TEMPLATE"))
                    {
                        foreach (DataColumn dc in ds.Tables[i].Columns)
                        {
                            if (!dictRLLXPARAM.ContainsKey(dc.ColumnName))
                            {
                                continue;
                            }
                            ds.Tables[i].Columns[dc.ColumnName].ColumnName = dictRLLXPARAM[dc.ColumnName];
                        }
                    }
                }
            }
            return ds.Tables.Contains("TEMPLATE") == true ? ds.Tables["TEMPLATE"] : null;
        }

    }
}
