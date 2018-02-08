using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using System.Data;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using System.Data;
using System.Windows.Forms;
using System.Collections;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils
{
    public class InspectInfoUtils
    {
        private Microsoft.Office.Interop.Excel.Application excelApp = null;

        public string dataToModelExcel_XLS(DataTable dtExportAll, string strSaveFile)
        {
            string msg = string.Empty;
            try
            {
                this.ClearMemory();
                dtExportAll.Columns["CLXZ"].ColumnName = "车辆性质";
                dtExportAll.Columns["GCCS"].ColumnName = "购车城市";
                dtExportAll.Columns["CLYXDW"].ColumnName = "车辆运行单位";
                dtExportAll.Columns["CLSCQY"].ColumnName = "车辆生产企业";
                dtExportAll.Columns["CLZL"].ColumnName = "车辆种类";
                dtExportAll.Columns["CLYT"].ColumnName = "车辆用途";
                dtExportAll.Columns["CLXH"].ColumnName = "车辆型号";
                dtExportAll.Columns["SPSJ"].ColumnName = "车辆数量";
                dtExportAll.Columns["CLSL"].ColumnName = "上牌时间（年月）";
                dtExportAll.Columns["LJXSLC"].ColumnName = "平均累计行驶里程（万公里）";
                msg += WriterExcel_XLS(strSaveFile, dtExportAll);
            }
            catch (Exception ex)
            {
                msg = String.Format("{0}:{1}", ex.Source, ex.Message);
            }
            return msg;
        }

        private string WriterExcel_XLS(string saveName, DataTable DT)
        {
            string msg = null;

            excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
            excelApp.Visible = false;

            try
            {
                int rowCount = DT.Rows.Count;//行数
                int colCount = DT.Columns.Count;//列数

                long pageRows = 65000;//定义每页显示的行数,行数必须小于65536   
                if (rowCount > pageRows)
                {
                    int scount = (int)(rowCount / pageRows);//导出数据生成的表单数   
                    if (scount * pageRows < rowCount)//当总行数不被pageRows整除时，经过四舍五入可能页数不准   
                    {
                        scount = scount + 1;
                    }
                    for (int sc = 1; sc <= scount; sc++)
                    {
                        object missing = System.Reflection.Missing.Value;
                        excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets.Add(
                                    missing, missing, missing, missing);
                        excelSheet.Name = String.Format(@"实地核查-{0}", sc);

                        object[,] datas = new object[pageRows + 1, colCount];

                        for (int i = 0; i < colCount; i++)
                        {
                            datas[0, i] = DT.Columns[i].ColumnName;
                        }

                        int init = int.Parse(((sc - 1) * pageRows).ToString());
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
                        for (int j = init; j < result; j++)
                        {
                            index = index + 1;
                            for (int i = 0; i < colCount; i++)
                            {
                                Type type = DT.Rows[j][i].GetType();
                                if (type == typeof(int) || type == typeof(float) || type == typeof(Decimal) || type == typeof(Double))
                                {
                                    datas[index, i] = Double.Parse(DT.Rows[j][i].ToString());
                                }
                                else if (type == typeof(DateTime))
                                {
                                    datas[index, i] = DateTime.Parse(DT.Rows[j][i].ToString()).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    datas[index, i] = DT.Rows[j][i].ToString();
                                }
                            }

                        }
                        Microsoft.Office.Interop.Excel.Range fchR = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[index + 1, colCount]];
                        fchR.Value = datas;
                        Microsoft.Office.Interop.Excel.Range allColumn = excelSheet.Columns;
                        allColumn.AutoFit();
                    }
                }
                else
                {
                    object[,] datas = new object[rowCount + 1, colCount];
                    for (int i = 0; i < colCount; i++)
                    {
                        datas[0, i] = DT.Columns[i].ColumnName;
                    }
                    int index = 1;
                    for (int j = 0; j < rowCount; j++)
                    {
                        index = j + 1;
                        for (int i = 0; i < colCount; i++)
                        {
                            Type type = DT.Rows[j][i].GetType();
                            if (type == typeof(int) || type == typeof(float) || type == typeof(Decimal) || type == typeof(Double))//值类型
                            {
                                datas[index, i] = Double.Parse(DT.Rows[j][i].ToString());
                            }
                            else if (type == typeof(DateTime))//类型
                            {
                                datas[index, i] = DateTime.Parse(DT.Rows[j][i].ToString()).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                datas[index, i] = DT.Rows[j][i].ToString();
                            }
                        }
                    }

                    Microsoft.Office.Interop.Excel.Range range = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowCount + 1, colCount]];
                    range.Value = datas;
                    Microsoft.Office.Interop.Excel.Range allColumn = excelSheet.Columns;
                    allColumn.AutoFit();
                }
                excelBook.SaveAs(saveName, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                return msg;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return msg;
            }
            finally
            {
                this.EndReport();
            }
        }

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

        private void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
