using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Windows.Forms;

namespace FuelDataSysClient.Tool
{
    class ExportToExcel
    {
        private Microsoft.Office.Interop.Excel.Application excelApp = null;

        public void ExportExcel(string savePath, DataTable dt)
        {
            excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
            excelApp.Visible = false;

            try
            {
                if (dt.Columns.Contains("USER_ID"))
                {
                    dt.Columns.Remove("USER_ID");
                }
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
                        object missing = System.Reflection.Missing.Value;
                        excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.Worksheets.Add(
                                    missing, missing, missing, missing);//添加一个sheet  
                        excelSheet.Name = String.Format(@"{0}{1}", dt.TableName, sc); 
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
                                datas[index, i] = dt.Rows[r][dt.Columns[i].ToString()];
                            }

                        }
                        Microsoft.Office.Interop.Excel.Range fchR = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[index + 1, colCount]];
                        fchR.NumberFormat = "@";
                        fchR.Value = datas;
                        excelSheet.Columns.EntireColumn.AutoFit();
                    }
                }
                else
                {
                    object[,] dataArray = new object[rowCount + 1, colCount];
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dataArray[0, i] = dictHeader[dt.Columns[i].ColumnName];
                    }

                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < colCount; j++)
                        {
                            dataArray[i + 1, j] = dt.Rows[i][j];
                        }
                    }
                    Microsoft.Office.Interop.Excel.Range range = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowCount + 1, colCount]];
                    range.NumberFormat = "@";
                    range.Value = dataArray;
                    excelSheet.Columns.EntireColumn.AutoFit();
                }
                string saveName = String.Format(@"{0}\{1}", savePath, dt.TableName);
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
                        dictHeader.Add(dt.Columns[i].ColumnName, "备案号(VIN)");
                        break;
                    case "QCSCQY":
                        dictHeader.Add(dt.Columns[i].ColumnName, "汽车生产企业");
                        break;
                    case "JKQCZJXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "进口汽车总经销商");
                        break;
                    case "CLZZRQ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆制造/进口日期");
                        break;
                    case "CLXH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆型号");
                        break;
                    case "HGSPBM":
                        dictHeader.Add(dt.Columns[i].ColumnName, "海关商品编码");
                        break;
                    case "CLZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车辆种类");
                        break;
                    case "YYC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "越野车(G类)");
                        break;
                    case "QDXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动形式");
                        break;
                    case "ZWPS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "座位牌数");
                        break;
                    case "ZCZBZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "整车整备质量(kg)");
                        break;
                    case "ZDSJZZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "最大设计总质量(kg)");
                        break;
                    case "JYBGBH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "报告编号");
                        break;
                    case "JYJGMC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "检测机构名称");
                        break;
                    case "TYMC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "通用名称");
                        break;
                    case "ZGCS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "最高车速(km/h)");
                        break;
                    case "EDZK":
                        dictHeader.Add(dt.Columns[i].ColumnName, "额定载客（人）");
                        break;
                    case "LTGG":
                        dictHeader.Add(dt.Columns[i].ColumnName, "轮胎规格");
                        break;
                    case "LJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "轮距（前/后）(mm)");
                        break;
                    case "ZJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "轴距(mm)");
                        break;
                    case "RLLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料类型");
                        break;
                    case "USER_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "上报人");
                        break;
                    case "UNIQUE_CODE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "唯一标示");
                        break;
                    case "UPLOADDEADLINE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "上报截止日期");
                        break;
                    case "CREATETIME":
                        dictHeader.Add(dt.Columns[i].ColumnName, "录入日期");
                        break;
                    case "UPDATETIME":
                        dictHeader.Add(dt.Columns[i].ColumnName, "上报日期");
                        break;
                    case "V_ID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "反馈码(V_ID)");
                        break;
                    case "QTXX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "其他基本信息");
                        break;
                    case "SJY":
                        dictHeader.Add(dt.Columns[i].ColumnName, "数据源标示");
                        break;
                    case "OPTION_CODE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "选装件代码");
                        break;
                    case "ATTRIBUTE_CODE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "定制编号");
                        break;
                    case "BASEID":
                        dictHeader.Add(dt.Columns[i].ColumnName, "基地代码");
                        break;
                    case "CAR_CODE":
                        dictHeader.Add(dt.Columns[i].ColumnName, "车型代码");
                        break;
                    case "PFBZ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "排放标准");
                        break;
                    case "YHDYBAH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "油耗标识实际打印备案号");
                        break;
                    case "SC_OCN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "生产OCN");
                        break;
                    case "XT_OCN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "系统OCN");
                        break;
                    case "MI_XT_OCN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "MI码+OCN");
                        break;
                    #region 传统能源
                    case "CT_BSQDWS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "变速器档位数");
                        break;
                    case "CT_BSQXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "变速器型式");
                        break;
                    case "CT_EDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "额定功率(kW)");
                        break;
                    case "CT_FDJXH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "发动机型号");
                        break;
                    case "CT_JGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "净功率");
                        break;
                    case "CT_PL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "排量(mL)");
                        break;
                    case "CT_QGS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "气缸数");
                        break;
                    case "CT_QTXX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "其他信息");
                        break;
                    case "CT_SJGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "市郊工况燃料消耗量(L/100km)");
                        break;
                    case "CT_SQGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "市区工况燃料消耗量(L/100km)");
                        break;
                    case "CT_ZHGKCO2PFL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况CO2排放量(g/km)");
                        break;
                    case "CT_ZHGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况燃料消耗量(L/100km)");
                        break;
                    #endregion

                    #region 非插电式混合动力
                    case "FCDS_HHDL_BSQDWS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "变速器档位数");
                        break;
                    case "FCDS_HHDL_BSQXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "变速器型式");
                        break;
                    case "FCDS_HHDL_CDDMSXZGCS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "纯电动模式下1km最高车速");
                        break;
                    case "FCDS_HHDL_CDDMSXZHGKXSLC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "纯电动模式下综合工况续驶里程");
                        break;
                    case "FCDS_HHDL_DLXDCBNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组比能量");
                        break;
                    case "FCDS_HHDL_DLXDCZBCDY":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组标称电压");
                        break;
                    case "FCDS_HHDL_DLXDCZZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组种类");
                        break;
                    case "FCDS_HHDL_DLXDCZZNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组总能量");
                        break;
                    case "FCDS_HHDL_EDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "额定功率");
                        break;
                    case "FCDS_HHDL_FDJXH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "发动机型号");
                        break;
                    case "FCDS_HHDL_HHDLJGXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "混合动力结构型式");
                        break;
                    case "FCDS_HHDL_HHDLZDDGLB":
                        dictHeader.Add(dt.Columns[i].ColumnName, "混合动力最大电功率比");
                        break;
                    case "FCDS_HHDL_JGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "最大净功率");
                        break;
                    case "FCDS_HHDL_PL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "排量");
                        break;
                    case "FCDS_HHDL_QDDJEDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机额定功率");
                        break;
                    case "FCDS_HHDL_QDDJFZNJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机峰值扭矩");
                        break;
                    case "FCDS_HHDL_QDDJLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机类型");
                        break;
                    case "FCDS_HHDL_QGS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "气缸数");
                        break;
                    case "FCDS_HHDL_SJGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "市郊工况燃料消耗量");
                        break;
                    case "FCDS_HHDL_SQGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "市区工况燃料消耗量");
                        break;
                    case "FCDS_HHDL_XSMSSDXZGN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "是否具有行驶模式手动选择功能");
                        break;
                    case "FCDS_HHDL_ZHGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况燃料消耗量");
                        break;
                    case "FCDS_HHDL_ZHKGCO2PL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况CO2排放");
                        break;
                    #endregion

                    #region 插电式混合动力
                    case "CDS_HHDL_BSQDWS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "变速器档位数");
                        break;
                    case "CDS_HHDL_BSQXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "变速器型式");
                        break;
                    case "CDS_HHDL_CDDMSXZGCS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "纯电动模式下1km最高车速");
                        break;
                    case "CDS_HHDL_CDDMSXZHGKXSLC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "纯电动模式下综合工况续驶里程");
                        break;
                    case "CDS_HHDL_DLXDCBNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组比能量");
                        break;
                    case "CDS_HHDL_DLXDCZBCDY":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组标称电压");
                        break;
                    case "CDS_HHDL_DLXDCZZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组种类");
                        break;
                    case "CDS_HHDL_DLXDCZZNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组总能量");
                        break;
                    case "CDS_HHDL_EDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "额定功率");
                        break;
                    case "CDS_HHDL_FDJXH":
                        dictHeader.Add(dt.Columns[i].ColumnName, "发动机型号");
                        break;
                    case "CDS_HHDL_HHDLJGXS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "混合动力结构型式");
                        break;
                    case "CDS_HHDL_HHDLZDDGLB":
                        dictHeader.Add(dt.Columns[i].ColumnName, "混合动力最大电功率比");
                        break;
                    case "CDS_HHDL_JGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "最大净功率");
                        break;
                    case "CDS_HHDL_PL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "排量");
                        break;
                    case "CDS_HHDL_QDDJEDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机额定功率");
                        break;
                    case "CDS_HHDL_QDDJFZNJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机峰值扭矩");
                        break;
                    case "CDS_HHDL_QDDJLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机类型");
                        break;
                    case "CDS_HHDL_QGS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "气缸数");
                        break;
                    case "CDS_HHDL_XSMSSDXZGN":
                        dictHeader.Add(dt.Columns[i].ColumnName, "是否具有行驶模式手动选择功能");
                        break;
                    case "CDS_HHDL_ZHGKDNXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况电能消耗量");
                        break;
                    case "CDS_HHDL_ZHGKRLXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况燃料消耗量");
                        break;
                    case "CDS_HHDL_ZHKGCO2PL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况CO2排放");
                        break;
                    #endregion

                    #region 纯电动
                    case "CDD_DDQC30FZZGCS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "电动汽车30分钟最高车速");
                        break;
                    case "CDD_DDXDCZZLYZCZBZLDBZ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池总质量与整车整备质量的比值");
                        break;
                    case "CDD_DLXDCBNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组比能量");
                        break;
                    case "CDD_DLXDCZBCDY":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组标称电压");
                        break;
                    case "CDD_DLXDCZEDNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组总能量");
                        break;
                    case "CDD_DLXDCZZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组种类");
                        break;
                    case "CDD_QDDJEDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机额定功率");
                        break;
                    case "CDD_QDDJFZNJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机峰值扭矩");
                        break;
                    case "CDD_QDDJLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机类型");
                        break;
                    case "CDD_ZHGKDNXHL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况电能消耗量");
                        break;
                    case "CDD_ZHGKXSLC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况续驶里程");
                        break;
                    #endregion

                    #region 燃料电池

                    case "RLDC_CDDMSXZGXSCS":
                        dictHeader.Add(dt.Columns[i].ColumnName, "电动汽车30分钟最高车速");
                        break;
                    case "RLDC_CQPBCGZYL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "储氢瓶标称工作压力");
                        break;
                    case "RLDC_CQPLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "储氢瓶类型");
                        break;
                    case "RLDC_CQPRJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "储氢瓶容积");
                        break;
                    case "RLDC_DDGLMD":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料电池堆功率密度");
                        break;
                    case "RLDC_DDHHJSTJXXDCZBNL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "电电混合技术条件下动力蓄电池组比能量");
                        break;
                    case "RLDC_DLXDCZZL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "动力蓄电池组种类");
                        break;
                    case "RLDC_QDDJEDGL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机额定功率");
                        break;
                    case "RLDC_QDDJFZNJ":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机峰值扭矩");
                        break;
                    case "RLDC_QDDJLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "驱动电机类型");
                        break;
                    case "RLDC_RLLX":
                        dictHeader.Add(dt.Columns[i].ColumnName, "燃料电池燃料类型");
                        break;
                    case "RLDC_ZHGKHQL":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况燃料消耗量");
                        break;
                    case "RLDC_ZHGKXSLC":
                        dictHeader.Add(dt.Columns[i].ColumnName, "综合工况续驶里程");
                        break;
                    #endregion
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

        public void ToExcelSheet(DataSet ds, string fileName)
        {
            Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
            if (appExcel == null)
            {
                MessageBox.Show("无法创建Excel对象，可能您的机子未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbook workbookData;
            Microsoft.Office.Interop.Excel.Worksheet worksheetData;
            Microsoft.Office.Interop.Excel.Range range;
            workbookData = appExcel.Workbooks.Add(System.Reflection.Missing.Value);
            appExcel.DisplayAlerts = false;//不显示警告
            for (int k = ds.Tables.Count-1; k >= 0; k--)
            {
                worksheetData = (Microsoft.Office.Interop.Excel.Worksheet)workbookData.Worksheets.Add(System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value);
                if (ds.Tables[k] != null)
                {
                    worksheetData.Name = ds.Tables[k].TableName;
                    //写入标题
                    for (int i = 0; i < ds.Tables[k].Columns.Count; i++)
                    {
                        worksheetData.Cells[1, i + 1] = ds.Tables[k].Columns[i].ColumnName;
                        range = (Microsoft.Office.Interop.Excel.Range)worksheetData.Cells[1, i + 1];
                        range.Interior.ColorIndex = 15;
                        range.Font.Bold = true;
                        range.NumberFormatLocal = "@";//文本格式 
                        range.ColumnWidth = 15;

                    }
                    //写入数值
                    for (int r = 0; r < ds.Tables[k].Rows.Count; r++)
                    {
                        for (int i = 0; i < ds.Tables[k].Columns.Count; i++)
                        {

                            worksheetData.Cells[r + 2, i + 1] = ds.Tables[k].Rows[r][i];
                        }
                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                worksheetData.Columns.EntireColumn.AutoFit();
                workbookData.Saved = true;
            }
            workbookData.SaveAs(fileName);
            workbookData.Close();
            appExcel.Quit();
            GC.Collect();
        }

    }
}
