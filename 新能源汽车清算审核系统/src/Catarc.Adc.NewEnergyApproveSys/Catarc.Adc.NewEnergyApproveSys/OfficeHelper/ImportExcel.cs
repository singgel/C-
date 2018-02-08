using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyApproveSys.DataUtils;

namespace Catarc.Adc.NewEnergyApproveSys.OfficeHelper
{
    public class ImportExcel
    {
        /// <summary>  
        /// 导入Excel到DataSet中，第一行做列名
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
                    throw ex;
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
        /// 根据sheet页导入Excel，第一行做列名
        /// </summary>
        /// <param name="strFileUrl">文件的路径和文件全名，含扩展名</param>
        /// <param name="sheet">名称</param>
        /// <returns></returns>
        public static DataSet ReadExcelToDataSet(string strFileUrl, string sheet)
        {
            string strConn = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0}; Extended Properties='Excel 12.0;HDR=YES;IMEX=1'", strFileUrl); //; HDR=No
            using (var conn = new OleDbConnection(strConn))
            {
                var ds = new DataSet();
                try
                {
                    conn.Open();
                    var sheetNames = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
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
                        using (var oada = new OleDbDataAdapter(String.Format("select * from [{0}]", sheet), strConn))
                        {
                            oada.Fill(ds, sheet.IndexOf('$') > 0 ? sheet.Substring(0, sheet.Length - 1) : sheet);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                DataTableHelper.removeEmpty(ds);
                return ds;
            }
        }

    }
}
