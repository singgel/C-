using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using Catarc.Adc.NewEnergyAccountSys.OfficeHelper;
using System.Data;
using NPOI;
using NPOI.SS.Util;

namespace Catarc.Adc.NewEnergyAccountSys.Common
{
    public class NPOITool
    {
        string strManufacturer = "企业名称";
        public static string strYear = "2017";
        private string strEvPath = Utils.installPath;

        /// <summary>
        /// 附件1: 推广应用车辆补助资金清算信息汇总表
        /// </summary>
        /// <param name="dtExport"></param>
        /// <param name="exportTemplatePath"></param>
        /// <param name="strFilePath"></param>
        /// <param name="strTitle"></param>
        /// <param name="strMinDate"></param>
        /// <param name="pb"></param>
        /// <param name="countNum"></param>
        /// <param name="countSum"></param>
        /// <param name="dtUser"></param>
        public static void DataTabletoCountExcel(System.Data.DataTable dtExport, string exportTemplatePath, string strFilePath, string strTitle, string strMinDate, int countNum, double countSum)
        {
            if (dtExport == null)
                return;
            long num1 = (long)dtExport.Rows.Count;
            int num2 = dtExport.Columns.Count - 1;
            int num3 = num2;

            IWorkbook wk = null;
            using (FileStream fs = File.Open(exportTemplatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    wk = new XSSFWorkbook(fs);
                }
                catch (Exception ex)
                {
                    wk = new HSSFWorkbook(fs);
                }
                finally
                {
                    fs.Close();
                }
            }

            ImportExcelNPOI ieNOPI = new ImportExcelNPOI();

            try
            {
                //excel赋值
                string strMessage = WriterCountExcel(wk, 0, dtExport, strTitle, strMinDate, countNum, countSum);

                wk.SetActiveSheet(0);//直接打开第一sheet

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                wk.Write(stream);
                var buf = stream.ToArray();
                //保存文件
                SaveFile(strFilePath, buf);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
        }

        /// <summary>
        /// excel赋值
        /// </summary>
        /// <param name="hssfworkbookDown"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="DT"></param>
        /// <param name="dtHeader"></param>
        /// <returns></returns>
        public static string WriterCountExcel(IWorkbook hssfworkbookDown, int sheetIndex, DataTable dtExport, string strTitle, string strMinDate, int countNum, double countSum)
        {
            try
            {
                #region 设置单元格样式
                //字体
                IFont fontS9 = hssfworkbookDown.CreateFont();
                fontS9.FontName = "微软雅黑";
                fontS9.FontHeightInPoints = 9;
                fontS9.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Normal;
                //表格
                ICellStyle style = hssfworkbookDown.CreateCellStyle();
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//左右居中
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//上下居中
                style.WrapText = true;
                style.SetFont(fontS9);
                //结尾
                ICellStyle styleEnd = hssfworkbookDown.CreateCellStyle();
                styleEnd.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                styleEnd.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                styleEnd.WrapText = true;
                styleEnd.SetFont(fontS9);
                #endregion

                ISheet sheet = hssfworkbookDown.GetSheetAt(sheetIndex);
                ISheet sheetNew = null;
                IRow dataRow = null;
                ICell cell = null;
                int numberpage = 65522;

                long num1 = (long)dtExport.Rows.Count;
                int dtColNum = dtExport.Columns.Count - 1;//num2


                if (num1 > numberpage)
                {
                    #region
                    long length = numberpage;
                    int num4 = (int)(num1 / length);
                    if ((long)num4 * length < num1)
                        ++num4;
                    int num5 = 0;//dtExport的行数
                    string sheetName = "附件1";

                    for (int index1 = 1; index1 <= num4; ++index1)
                    {

                        sheetName = "附件1（" + (index1 + 1).ToString() + "）";
                        sheet.CopySheet(sheetName, true);//复制第一个SHEET内容和格式


                        sheetNew = hssfworkbookDown.GetSheet(sheetName);
                        //重写年度和企业名称
                        cell = sheetNew.GetRow(1).GetCell(0);
                        cell.SetCellValue(strMinDate + "推广应用车辆补助资金清算信息汇总表");
                        sheetNew.GetRow(2).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);

                        int num7 = length * (long)index1 < num1 ? numberpage : (int)(num1 - (long)(index1 - 1) * length);//不是最后一页为numberpage，最后一页为
                        int numpageColum = 5;//当前表格的行数
                        int index3 = 0;//num6;//当前sheet内循环

                        for (index3 = 0; index3 < num7; ++index3)
                        {
                            dataRow = sheetNew.CreateRow(numpageColum);//新增行
                            for (int index4 = 0; index4 < dtColNum; ++index4)
                            {
                                cell = dataRow.CreateCell(index4);

                                switch (index4)
                                {
                                    case 0:
                                        cell.SetCellValue(num5 + 1);//序号
                                        break;
                                    case 1:
                                        cell.SetCellValue(dtExport.Rows[num5]["GCCS"].ToString());
                                        break;
                                    case 2:
                                        cell.SetCellValue(dtExport.Rows[num5]["CLYXDW"].ToString());
                                        break;
                                    case 3:
                                        cell.SetCellValue(dtExport.Rows[num5]["GGPC"].ToString());
                                        break;
                                    case 4:
                                        cell.SetCellValue(dtExport.Rows[num5]["CLXH"].ToString());
                                        break;
                                    case 5:
                                        cell.SetCellValue(dtExport.Rows[num5]["SQBZBZ"].ToString());
                                        break;
                                    case 6:
                                        cell.SetCellValue(dtExport.Rows[num5]["NUMCOUNT"].ToString());
                                        break;
                                    case 7:
                                        cell.SetCellValue(dtExport.Rows[num5]["SUMCOUNT"].ToString());
                                        break;
                                }
                                cell.CellStyle = style;//添加样式
                            }
                            num5++;
                            numpageColum++;
                        }
                        if (index1 == num4)
                        {
                            //合计
                            dataRow = sheetNew.CreateRow(numpageColum);//新增行

                            cell = dataRow.CreateCell(0);
                            cell.SetCellValue("合计");
                            cell.CellStyle = style;

                            for (int i = 1; i <= 5; i++)
                            {
                                cell = dataRow.CreateCell(i);
                                cell.SetCellValue("--");
                                cell.CellStyle = style;
                            }
                            cell = dataRow.CreateCell(6);
                            cell.SetCellValue(countNum);
                            cell.CellStyle = style;
                            cell = dataRow.CreateCell(7);
                            cell.SetCellValue(countSum);
                            cell.CellStyle = style;

                            dataRow = sheetNew.CreateRow(++numpageColum);//新增行
                            cell = dataRow.CreateCell(0);
                            cell.SetCellValue(String.Format("1、\t车辆统计时间段为{0}年1月1日—{0}年12月31日", strMinDate));
                            cell.CellStyle = styleEnd;
                            //合并
                            sheetNew.AddMergedRegion(new CellRangeAddress(numpageColum, numpageColum, 0, 7));

                            dataRow = sheetNew.CreateRow(++numpageColum);//新增行
                            cell = dataRow.CreateCell(0);
                            cell.SetCellValue("2、\t车辆型号必须严格按照国家《新能源汽车推广应用推荐车型目录》填写");
                            cell.CellStyle = styleEnd;
                            //合并
                            sheetNew.AddMergedRegion(new CellRangeAddress(numpageColum, numpageColum, 0, 7));

                            dataRow = sheetNew.CreateRow(++numpageColum);//新增行
                            cell = dataRow.CreateCell(0);
                            cell.SetCellValue("3、\t公告批次填写阿拉伯数字");
                            cell.CellStyle = styleEnd;
                            //合并
                            sheetNew.AddMergedRegion(new CellRangeAddress(numpageColum, numpageColum, 0, 7));
                        }
                    }
                    //删除第一个表 重命名最后一个表
                    hssfworkbookDown.RemoveSheetAt(0);
                    hssfworkbookDown.SetSheetName(num4 - 1, "附件1");//重命名sheet
                    #endregion
                }
                else
                {
                    #region
                    int dtRowNum = dtExport.Rows.Count;
                    //重写年度和企业名称
                    cell = sheet.GetRow(1).GetCell(0);
                    cell.SetCellValue(strMinDate + "推广应用车辆补助资金清算信息汇总表");
                    sheet.GetRow(2).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);

                    int n = 5;//因为模板有表头和标题，所以从第6行开始写

                    int num4 = 1;
                    int length = dtColNum;

                    for (int index1 = 0; index1 < dtRowNum; ++index1)
                    {
                        dataRow = sheet.CreateRow(index1 + n);//新增行

                        for (int index2 = 0; index2 < dtColNum; ++index2)
                        {
                            cell = dataRow.CreateCell(index2);

                            switch (index2)
                            {
                                case 0:
                                    cell.SetCellValue(num4++);//序号
                                    break;
                                case 1:
                                    cell.SetCellValue(dtExport.Rows[index1]["GCCS"].ToString());
                                    break;
                                case 2:
                                    cell.SetCellValue(dtExport.Rows[index1]["CLYXDW"].ToString());
                                    break;
                                case 3:
                                    cell.SetCellValue(dtExport.Rows[index1]["GGPC"].ToString());
                                    break;
                                case 4:
                                    cell.SetCellValue(dtExport.Rows[index1]["CLXH"].ToString());
                                    break;
                                case 5:
                                    cell.SetCellValue(dtExport.Rows[index1]["SQBZBZ"].ToString());
                                    break;
                                case 6:
                                    cell.SetCellValue(dtExport.Rows[index1]["NUMCOUNT"].ToString());
                                    break;
                                case 7:
                                    cell.SetCellValue(dtExport.Rows[index1]["SUMCOUNT"].ToString());
                                    break;
                            }
                            cell.CellStyle = style;//添加样式
                        }
                    }
                    //合计
                    dataRow = sheet.CreateRow(dtRowNum + n);//新增行

                    cell = dataRow.CreateCell(0);
                    cell.SetCellValue("合计");
                    cell.CellStyle = style;

                    for (int i = 1; i <= 5; i++)
                    {
                        cell = dataRow.CreateCell(i);
                        cell.SetCellValue("--");
                        cell.CellStyle = style;
                    }
                    cell = dataRow.CreateCell(6);
                    cell.SetCellValue(countNum);
                    cell.CellStyle = style;
                    cell = dataRow.CreateCell(7);
                    cell.SetCellValue(countSum);
                    cell.CellStyle = style;

                    dataRow = sheet.CreateRow(n + dtRowNum + 1);//新增行
                    cell = dataRow.CreateCell(0);
                    cell.SetCellValue(String.Format("1、\t车辆统计时间段为{0}年1月1日—{0}年12月31日", strMinDate));
                    cell.CellStyle = styleEnd;
                    //合并
                    sheet.AddMergedRegion(new CellRangeAddress(n + dtRowNum + 1, n + dtRowNum + 1, 0, 7));

                    dataRow = sheet.CreateRow(n + dtRowNum + 2);//新增行
                    cell = dataRow.CreateCell(0);
                    cell.SetCellValue("2、\t车辆型号必须严格按照国家《新能源汽车推广应用推荐车型目录》填写");
                    cell.CellStyle = styleEnd;
                    //合并
                    sheet.AddMergedRegion(new CellRangeAddress(n + dtRowNum + 2, n + dtRowNum + 2, 0, 7));

                    dataRow = sheet.CreateRow(n + dtRowNum + 3);//新增行
                    cell = dataRow.CreateCell(0);
                    cell.SetCellValue("3、\t公告批次填写阿拉伯数字");
                    cell.CellStyle = styleEnd;
                    //合并
                    sheet.AddMergedRegion(new CellRangeAddress(n + dtRowNum + 3, n + dtRowNum + 3, 0, 7));
                    #endregion
                }
                //设定第一行，第一列的单元格选中
                sheet.SetActiveCell(0, 0);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 附件2: 推广应用车辆补助资金清算信息明细表
        /// </summary>
        /// <param name="dtExport"></param>
        /// <param name="exportTemplatePath"></param>
        /// <param name="strFilePath"></param>
        /// <param name="strTitle"></param>
        /// <param name="strMinDate"></param>
        /// <param name="pb"></param>
        public static void DataTabletoMessageExcel(System.Data.DataTable dtExport, string exportTemplatePath, string strFilePath, string strTitle, string strMinDate)
        {
            if (dtExport == null)
                return;
            long num1 = (long)dtExport.Rows.Count;
            int num2 = dtExport.Columns.Count - 1;
            int num3 = num2;

            IWorkbook wk = null;
            using (FileStream fs = File.Open(exportTemplatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    wk = new XSSFWorkbook(fs);
                }
                catch (Exception ex)
                {
                    wk = new HSSFWorkbook(fs);
                }
                finally
                {
                    fs.Close();
                }
            }

            ImportExcelNPOI ieNOPI = new ImportExcelNPOI();

            try
            {
                //excel赋值
                string strMessage = WriterMessageExcel(wk, dtExport, strTitle, strMinDate);

                wk.SetActiveSheet(0);//直接打开第一sheet

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                wk.Write(stream);
                var buf = stream.ToArray();
                //保存文件
                SaveFile(strFilePath, buf);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //附件2
        public static string WriterMessageExcel(IWorkbook hssfworkbookDown, DataTable dtExport, string strTitle, string strMinDate)
        {
            try
            {
                #region 设置单元格样式
                //字体
                IFont fontS9 = hssfworkbookDown.CreateFont();
                fontS9.FontName = "微软雅黑";
                fontS9.FontHeightInPoints = 8;
                fontS9.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Normal;
                //表格
                ICellStyle style = hssfworkbookDown.CreateCellStyle();
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//左右居中
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//上下居中
                style.WrapText = true;
                style.SetFont(fontS9);
                //结尾
                fontS9.FontHeightInPoints = 9;
                ICellStyle styleEnd = hssfworkbookDown.CreateCellStyle();
                styleEnd.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                styleEnd.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                styleEnd.WrapText = true;
                styleEnd.SetFont(fontS9);
                #endregion

                int sheetIndex = 0;
                ISheet sheet = hssfworkbookDown.GetSheetAt(sheetIndex);
                ISheet sheetNew = null;
                IRow dataRow = null;
                ICell cell = null;
                int numberpage =  65522;
                string[] strArray = new string[7]{
                                "备注："
                                ,"1、\t车辆补助标准按照《关于2016-2020年新能源汽车推广应用财政支持政策的通知》 （财建[2015] 134号）执行"
                                ,"2、\t车辆种类：插电式混合动力客车、插电式混合动力乘用车、纯电动客车、纯电动乘用车、纯电动特种车、燃料电池客车、燃料电池乘用车、燃料电池货车等。"
                                ,"3、\t车辆用途： "
                                ,"（1）\t客车主要包括：公交、通勤、旅游 、公路"
                                ,"（2）\t乘用车主要包括：公务（含企事业单位用车）、出租、租赁、私人 "
                                ,"（3）\t特种车主要包括：邮政、物流、环卫、工程"};

                long num1 = (long)dtExport.Rows.Count;
                int dtColNum = dtExport.Columns.Count - 1;//num2


                if (num1 > numberpage)
                {
                    #region
                    long length = numberpage;
                    int num4 = (int)(num1 / length);
                    if ((long)num4 * length < num1)
                        ++num4;
                    int num5 = 0;//dtExport的行数
                    string sheetName = "附件2";

                    for (int index1 = 1; index1 <= num4; ++index1)
                    {

                        sheetName = "附件2（" + (index1 + 1).ToString() + "）";
                        sheet.CopySheet(sheetName, true);//复制第一个SHEET内容和格式


                        sheetNew = hssfworkbookDown.GetSheet(sheetName);
                        //重写年度和企业名称
                        cell = sheetNew.GetRow(1).GetCell(0);
                        cell.SetCellValue(strMinDate + "年推广应用车辆补助资金清算信息明细表");
                        sheetNew.GetRow(2).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);

                        int num7 = length * (long)index1 < num1 ? numberpage : (int)(num1 - (long)(index1 - 1) * length);//不是最后一页为numberpage，最后一页为
                        int numpageColum = 5;//当前表格的行数
                        int index3 = 0;//num6;//当前sheet内循环

                        for (index3 = 0; index3 < num7; ++index3)
                        {
                            string str1 = dtExport.Rows[num5]["发票时间"].ToString();
                            DateTime dt = Convert.ToDateTime(str1);
                            str1 = dt.GetDateTimeFormats('D')[0].ToString();
                            string str2 = dtExport.Rows[num5]["行驶证时间"].ToString();
                            DateTime dt2 = Convert.ToDateTime(str2);
                            str2 = dt2.GetDateTimeFormats('D')[0].ToString();

                            dataRow = sheetNew.CreateRow(numpageColum);//新增行

                            for (int index4 = 0; index4 < dtColNum; ++index4)
                            {
                                #region
                                switch (index4)
                                {
                                    case 0:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(num5 + 1);//序号
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 1:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆性质"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 2:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["购车城市"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 3:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆运行单位"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 4:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆种类"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 5:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆用途"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 6:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆型号"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 7:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["Ekg值"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 8:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["公告批次"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 9:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆识别代码(VIN)"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 10:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆牌照"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 11:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["申请补助标准"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 12:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["购买价格"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 13:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(dtExport.Rows[num5]["发票号"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 14:
                                        cell = dataRow.CreateCell(index4);
                                        cell.SetCellValue(str1.Substring(0, 4));
                                        cell.CellStyle = style;//添加样式
                                        cell = dataRow.CreateCell(index4 + 1);
                                        cell.SetCellValue(str1.Substring(str1.IndexOf("年") + 1, str1.IndexOf("月") - str1.IndexOf("年") - 1));
                                        cell.CellStyle = style;//添加样式
                                        cell = dataRow.CreateCell(index4 + 2);
                                        cell.SetCellValue(str1.Substring(str1.IndexOf("月") + 1, str1.IndexOf("日") - str1.IndexOf("月") - 1));
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 15:
                                        cell = dataRow.CreateCell(index4 + 2);
                                        cell.SetCellValue(str2.Substring(0, 4));
                                        cell.CellStyle = style;//添加样式
                                        cell = dataRow.CreateCell(index4 + 3);
                                        cell.SetCellValue(str2.Substring(str2.IndexOf("年") + 1, str2.IndexOf("月") - str2.IndexOf("年") - 1));
                                        cell.CellStyle = style;//添加样式
                                        cell = dataRow.CreateCell(index4 + 4);
                                        cell.SetCellValue(str2.Substring(str2.IndexOf("月") + 1, str2.IndexOf("日") - str2.IndexOf("月") - 1));
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 16:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容单体型号"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池单体型号"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池单体型号"].ToString() + "|" + dtExport.Rows[num5]["电容单体型号"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 17:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容单体生产企业"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池单体生产企业"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池单体生产企业"].ToString() + "|" + dtExport.Rows[num5]["电容单体生产企业"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 18:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容成箱型号"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池成箱型号"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池成箱型号"].ToString() + "|" + dtExport.Rows[num5]["电容成箱型号"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 19:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容总容量（kWh）"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组总容量（kWh）"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组总容量（kWh）"].ToString() + "|" + dtExport.Rows[num5]["电容总容量（kWh）"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 20:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容成组生产企业"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组生产企业"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组生产企业"].ToString() + "|" + dtExport.Rows[num5]["电容成组生产企业"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 21:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容系统价格（万元）"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组系统价格（万元）"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组系统价格（万元）"].ToString() + "|" + dtExport.Rows[num5]["电容系统价格（万元）"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 22:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容质保年限（年）"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组质保年限（年）"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电池组质保年限（年）"].ToString() + "|" + dtExport.Rows[num5]["电容质保年限（年）"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 23:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["驱动电机型号2"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["驱动电机型号1"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["驱动电机型号1"].ToString() + "|" + dtExport.Rows[num5]["驱动电机型号2"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 24:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电机额定功率2（kW）"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电机额定功率1（kW）"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电机额定功率1（kW）"].ToString() + "|" + dtExport.Rows[num5]["电机额定功率2（kW）"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 25:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电机生产企业2"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电机生产企业1"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电机生产企业1"].ToString() + "|" + dtExport.Rows[num5]["电机生产企业2"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 26:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        if (string.IsNullOrEmpty(dtExport.Rows[num5]["电机系统价格2（万元）"].ToString()))
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电机系统价格1（万元）"].ToString());
                                        }
                                        else
                                        {
                                            cell.SetCellValue(dtExport.Rows[num5]["电机系统价格1（万元）"].ToString() + "|" + dtExport.Rows[num5]["电机系统价格2（万元）"].ToString());
                                        }
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 27:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        cell.SetCellValue(dtExport.Rows[num5]["燃料电池型号"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 28:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        cell.SetCellValue(dtExport.Rows[num5]["燃料电池额定功率"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 29:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        cell.SetCellValue(dtExport.Rows[num5]["燃料电池生产企业"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 30:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        cell.SetCellValue(dtExport.Rows[num5]["燃料电池系统价格"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                    case 31:
                                        cell = dataRow.CreateCell(index4 + 4);
                                        cell.SetCellValue(dtExport.Rows[num5]["燃料电池质保年限（年）"].ToString());
                                        cell.CellStyle = style;//添加样式
                                        break;
                                }
                                #endregion

                            }
                            num5++;
                            numpageColum++;
                        }
                        if (index1 == num4)
                        {
                            
                            for (int iAarr = 0; iAarr < 7; iAarr++)
                            {
                                dataRow = sheetNew.CreateRow(numpageColum);//新增行
                                cell = dataRow.CreateCell(0);
                                cell.SetCellValue(strArray[iAarr]);
                                cell.CellStyle = styleEnd;
                                sheetNew.AddMergedRegion(new CellRangeAddress(numpageColum, numpageColum, 0, 24));//合并
                                numpageColum++;
                            }
                        }
                    }
                    //删除第一个表 重命名最后一个表
                    hssfworkbookDown.RemoveSheetAt(0);
                    hssfworkbookDown.SetSheetName(num4 - 1, "附件2");//重命名sheet
                    #endregion
                }
                else
                {
                    #region
                    int dtRowNum = dtExport.Rows.Count;
                    //重写年度和企业名称
                    cell = sheet.GetRow(1).GetCell(0);
                    cell.SetCellValue(strMinDate + "年推广应用车辆补助资金清算信息明细表");
                    sheet.GetRow(2).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);

                    int n = 5;//因为模板有表头和标题，所以从第6行开始写

                    int num5 = 0;
                    int length = dtColNum;

                    for (num5 = 0; num5 < dtRowNum; ++num5)
                    {
                        string str1 = dtExport.Rows[num5]["发票时间"].ToString();
                        DateTime dt = Convert.ToDateTime(str1);
                        str1 = dt.GetDateTimeFormats('D')[0].ToString();
                        string str2 = dtExport.Rows[num5]["行驶证时间"].ToString();
                        DateTime dt2 = Convert.ToDateTime(str2);
                        str2 = dt2.GetDateTimeFormats('D')[0].ToString();

                        dataRow = sheet.CreateRow(num5 + n);//新增行

                        for (int index4 = 0; index4 < dtColNum; ++index4)
                        {
                            switch (index4)
                            {
                                case 0:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(num5 + 1);//序号
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 1:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆性质"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 2:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["购车城市"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 3:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆运行单位"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 4:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆种类"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 5:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆用途"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 6:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆型号"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 7:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["Ekg值"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 8:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["公告批次"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 9:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆识别代码(VIN)"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 10:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆牌照"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 11:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["申请补助标准"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 12:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["购买价格"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 13:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(dtExport.Rows[num5]["发票号"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 14:
                                    cell = dataRow.CreateCell(index4);
                                    cell.SetCellValue(str1.Substring(0, 4));
                                    cell.CellStyle = style;//添加样式
                                    cell = dataRow.CreateCell(index4 + 1);
                                    cell.SetCellValue(str1.Substring(str1.IndexOf("年") + 1, str1.IndexOf("月") - str1.IndexOf("年") - 1));
                                    cell.CellStyle = style;//添加样式
                                    cell = dataRow.CreateCell(index4 + 2);
                                    cell.SetCellValue(str1.Substring(str1.IndexOf("月") + 1, str1.IndexOf("日") - str1.IndexOf("月") - 1));
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 15:
                                    cell = dataRow.CreateCell(index4 + 2);
                                    cell.SetCellValue(str2.Substring(0, 4));
                                    cell.CellStyle = style;//添加样式
                                    cell = dataRow.CreateCell(index4 + 3);
                                    cell.SetCellValue(str2.Substring(str2.IndexOf("年") + 1, str2.IndexOf("月") - str2.IndexOf("年") - 1));
                                    cell.CellStyle = style;//添加样式
                                    cell = dataRow.CreateCell(index4 + 4);
                                    cell.SetCellValue(str2.Substring(str2.IndexOf("月") + 1, str2.IndexOf("日") - str2.IndexOf("月") - 1));
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 16:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容单体型号"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池单体型号"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池单体型号"].ToString() + "|" + dtExport.Rows[num5]["电容单体型号"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 17:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容单体生产企业"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池单体生产企业"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池单体生产企业"].ToString() + "|" + dtExport.Rows[num5]["电容单体生产企业"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 18:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容成箱型号"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池成箱型号"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池成箱型号"].ToString() + "|" + dtExport.Rows[num5]["电容成箱型号"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 19:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容总容量（kWh）"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组总容量（kWh）"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组总容量（kWh）"].ToString() + "|" + dtExport.Rows[num5]["电容总容量（kWh）"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 20:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容成组生产企业"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组生产企业"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组生产企业"].ToString() + "|" + dtExport.Rows[num5]["电容成组生产企业"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 21:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容系统价格（万元）"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组系统价格（万元）"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组系统价格（万元）"].ToString() + "|" + dtExport.Rows[num5]["电容系统价格（万元）"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 22:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电容质保年限（年）"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组质保年限（年）"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电池组质保年限（年）"].ToString() + "|" + dtExport.Rows[num5]["电容质保年限（年）"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 23:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["驱动电机型号2"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["驱动电机型号1"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["驱动电机型号1"].ToString() + "|" + dtExport.Rows[num5]["驱动电机型号2"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 24:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电机额定功率2（kW）"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电机额定功率1（kW）"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电机额定功率1（kW）"].ToString() + "|" + dtExport.Rows[num5]["电机额定功率2（kW）"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 25:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电机生产企业2"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电机生产企业1"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电机生产企业1"].ToString() + "|" + dtExport.Rows[num5]["电机生产企业2"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 26:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    if (string.IsNullOrEmpty(dtExport.Rows[num5]["电机系统价格2（万元）"].ToString()))
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电机系统价格1（万元）"].ToString());
                                    }
                                    else
                                    {
                                        cell.SetCellValue(dtExport.Rows[num5]["电机系统价格1（万元）"].ToString() + "|" + dtExport.Rows[num5]["电机系统价格2（万元）"].ToString());
                                    }
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 27:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    cell.SetCellValue(dtExport.Rows[num5]["燃料电池型号"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 28:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    cell.SetCellValue(dtExport.Rows[num5]["燃料电池额定功率"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 29:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    cell.SetCellValue(dtExport.Rows[num5]["燃料电池生产企业"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 30:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    cell.SetCellValue(dtExport.Rows[num5]["燃料电池系统价格"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                                case 31:
                                    cell = dataRow.CreateCell(index4 + 4);
                                    cell.SetCellValue(dtExport.Rows[num5]["燃料电池质保年限（年）"].ToString());
                                    cell.CellStyle = style;//添加样式
                                    break;
                            }
                            
                        }
                    }
                    //尾部
                    for (int iAarr = 0; iAarr < 7; iAarr++)
                    {
                        dataRow = sheet.CreateRow(n + dtRowNum + iAarr);//新增行
                        cell = dataRow.CreateCell(0);
                        cell.SetCellValue(strArray[iAarr]);
                        cell.CellStyle = styleEnd;
                        sheet.AddMergedRegion(new CellRangeAddress(n + dtRowNum + iAarr, n + dtRowNum + iAarr, 0, 24));//合并

                    }

                    #endregion
                }
                //设定第一行，第一列的单元格选中
                sheet.SetActiveCell(0, 0);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 附件3：推广应用车辆运行情况表
        /// </summary>
        /// <param name="dtExport"></param>
        /// <param name="exportTemplatePath"></param>
        /// <param name="strFilePath"></param>
        /// <param name="strTitle"></param>
        /// <param name="strMinDate"></param>
        /// <param name="pb"></param>
        public static void DataTabletoRunExcel(System.Data.DataTable dtExport, string exportTemplatePath, string strFilePath, string strTitle, string strMinDate)
        {
            if (dtExport == null)
                return;
            long num1 = (long)dtExport.Rows.Count;
            int num2 = dtExport.Columns.Count - 1;
            int num3 = num2;

            IWorkbook wk = null;
            using (FileStream fs = File.Open(exportTemplatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    wk = new XSSFWorkbook(fs);
                }
                catch (Exception ex)
                {
                    wk = new HSSFWorkbook(fs);
                }
                finally
                {
                    fs.Close();
                }
            }

            ImportExcelNPOI ieNOPI = new ImportExcelNPOI();

            try
            {
                //excel赋值
                string strMessage = WriterRunExcel(wk, dtExport, strTitle, strMinDate);

                wk.SetActiveSheet(0);//直接打开第一sheet

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                wk.Write(stream);
                var buf = stream.ToArray();
                //保存文件
                SaveFile(strFilePath, buf);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //3
        public static string WriterRunExcel(IWorkbook hssfworkbookDown, DataTable dtExport, string strTitle, string strMinDate)
        {
            try
            {
                #region 设置单元格样式
                //字体
                IFont fontS9 = hssfworkbookDown.CreateFont();
                fontS9.FontName = "微软雅黑";
                fontS9.FontHeightInPoints = 8;
                fontS9.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Normal;
                //表格
                ICellStyle style = hssfworkbookDown.CreateCellStyle();
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;//左右居中
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;//上下居中
                style.WrapText = true;
                style.SetFont(fontS9);
                //结尾
                fontS9.FontHeightInPoints = 9;
                ICellStyle styleEnd = hssfworkbookDown.CreateCellStyle();
                styleEnd.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                styleEnd.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                styleEnd.WrapText = true;
                styleEnd.SetFont(fontS9);
                #endregion

                ISheet sheet = hssfworkbookDown.GetSheetAt(0);
                ISheet sheetNew = null;
                IRow dataRow = null;
                ICell cell = null;
                int numberpage = 65522;

                long num1 = (long)dtExport.Rows.Count;
                int dtColNum = dtExport.Columns.Count - 1;//num2


                if (num1 > numberpage)
                {
                    #region
                    long length = numberpage;
                    int num4 = (int)(num1 / length);
                    if ((long)num4 * length < num1)
                        ++num4;
                    int num5 = 0;//dtExport的行数
                    string sheetName = "附件3";

                    for (int index1 = 1; index1 <= num4; ++index1)
                    {

                        sheetName = "附件3（" + (index1 + 1).ToString() + "）";
                        sheet.CopySheet(sheetName, true);//复制第一个SHEET内容和格式


                        sheetNew = hssfworkbookDown.GetSheet(sheetName);
                        //重写年度和企业名称
                        cell = sheetNew.GetRow(1).GetCell(0);
                        cell.SetCellValue(strMinDate + "年推广应用车辆运行情况表");
                        sheetNew.GetRow(2).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);

                        int num7 = length * (long)index1 < num1 ? numberpage : (int)(num1 - (long)(index1 - 1) * length);//不是最后一页为numberpage，最后一页为
                        int numpageColum = 5;//当前表格的行数
                        int index3 = 0;//num6;//当前sheet内循环

                        for (index3 = 0; index3 < num7; ++index3)
                        {
                            dataRow = sheetNew.CreateRow(numpageColum);//新增行
                            for (int index4 = 0; index4 < 19; ++index4)//有19列 最后一列为空
                            {
                                cell = dataRow.CreateCell(index4);

                                switch (index4)
                                {
                                    case 0:
                                        cell.SetCellValue(num5 + 1);//序号
                                        break;
                                    case 1:
                                        cell.SetCellValue(dtExport.Rows[num5]["购车城市"].ToString());
                                        break;
                                    case 2:
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆种类"].ToString());
                                        break;
                                    case 3:
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆用途"].ToString());
                                        break;
                                    case 4:
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆牌照"].ToString());
                                        break;
                                    case 5:
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆充满一次电能够行驶里程（公里）"].ToString());
                                        break;
                                    case 6:
                                        cell.SetCellValue(dtExport.Rows[num5]["车辆一次充满电所需时间（h）"].ToString());
                                        break;
                                    case 7:
                                        cell.SetCellValue(dtExport.Rows[num5]["最大充电功率（kW）"].ToString());
                                        break;
                                    case 8:
                                        cell.SetCellValue(dtExport.Rows[num5]["累计行驶里程*（公里）"].ToString());
                                        break;
                                    case 9:
                                        cell.SetCellValue(dtExport.Rows[num5]["月均行驶里程（公里）"].ToString());
                                        break;
                                    case 10:
                                        cell.SetCellValue(dtExport.Rows[num5]["百公里耗电量（kWh/100km）"].ToString());
                                        break;
                                    case 11:
                                        cell.SetCellValue(dtExport.Rows[num5]["平均单日运行时间（h）"].ToString());
                                        break;
                                    case 12:
                                        cell.SetCellValue(dtExport.Rows[num5]["加油量（L）"].ToString());
                                        break;
                                    case 13:
                                        cell.SetCellValue(dtExport.Rows[num5]["加气量（kg）"].ToString());
                                        break;
                                    case 14:
                                        cell.SetCellValue(dtExport.Rows[num5]["加气量（L）"].ToString());
                                        break;
                                    case 15:
                                        cell.SetCellValue(dtExport.Rows[num5]["加氢量（kg）"].ToString());
                                        break;
                                    case 16:
                                        cell.SetCellValue(dtExport.Rows[num5]["充电量（kWh）"].ToString());
                                        break;
                                    case 17:
                                        cell.SetCellValue(dtExport.Rows[num5]["是否安装监控装置"].ToString());
                                        break;
                                    case 18:
                                        cell.SetCellValue(dtExport.Rows[num5]["监控平台运行单位"].ToString());
                                        break;
                                }
                                cell.CellStyle = style;//添加样式
                            }
                            num5++;
                            numpageColum++;
                        }
                        if (index1 == num4)
                        {
                            dataRow = sheetNew.CreateRow(numpageColum);//新增行
                            cell = dataRow.CreateCell(0);
                            cell.SetCellValue(String.Format("*注：截至{0}年底的行驶里程", strMinDate));
                            cell.CellStyle = styleEnd;
                            sheetNew.AddMergedRegion(new CellRangeAddress(numpageColum, numpageColum, 0, 16));//合并
                            numpageColum++;
                        }
                    }
                    //删除第一个表 重命名最后一个表
                    hssfworkbookDown.RemoveSheetAt(0);
                    hssfworkbookDown.SetSheetName(num4 - 1, "附件3");//重命名sheet
                    #endregion
                }
                else
                {
                    #region
                    int dtRowNum = dtExport.Rows.Count;
                    //重写年度和企业名称
                    cell = sheet.GetRow(1).GetCell(0);
                    cell.SetCellValue(strMinDate + "年推广应用车辆运行情况表");
                    sheet.GetRow(2).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);

                    int n = 5;//因为模板有表头和标题，所以从第6行开始写

                    int num5 = 0;
                    int length = dtColNum;

                    for (num5 = 0; num5 < dtRowNum; ++num5)
                    {
                        dataRow = sheet.CreateRow(num5 + n);//新增行

                        for (int index4 = 0; index4 < 19; ++index4)
                        {
                            cell = dataRow.CreateCell(index4);
                            switch (index4)
                            {
                                case 0:
                                    cell.SetCellValue(num5 + 1);//序号
                                    break;
                                case 1:
                                    cell.SetCellValue(dtExport.Rows[num5]["购车城市"].ToString());
                                    break;
                                case 2:
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆种类"].ToString());
                                    break;
                                case 3:
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆用途"].ToString());
                                    break;
                                case 4:
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆牌照"].ToString());
                                    break;
                                case 5:
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆充满一次电能够行驶里程（公里）"].ToString());
                                    break;
                                case 6:
                                    cell.SetCellValue(dtExport.Rows[num5]["车辆一次充满电所需时间（h）"].ToString());
                                    break;
                                case 7:
                                    cell.SetCellValue(dtExport.Rows[num5]["最大充电功率（kW）"].ToString());
                                    break;
                                case 8:
                                    cell.SetCellValue(dtExport.Rows[num5]["累计行驶里程*（公里）"].ToString());
                                    break;
                                case 9:
                                    cell.SetCellValue(dtExport.Rows[num5]["月均行驶里程（公里）"].ToString());
                                    break;
                                case 10:
                                    cell.SetCellValue(dtExport.Rows[num5]["百公里耗电量（kWh/100km）"].ToString());
                                    break;
                                case 11:
                                    cell.SetCellValue(dtExport.Rows[num5]["平均单日运行时间（h）"].ToString());
                                    break;
                                case 12:
                                    cell.SetCellValue(dtExport.Rows[num5]["加油量（L）"].ToString());
                                    break;
                                case 13:
                                    cell.SetCellValue(dtExport.Rows[num5]["加气量（kg）"].ToString());
                                    break;
                                case 14:
                                    cell.SetCellValue(dtExport.Rows[num5]["加气量（L）"].ToString());
                                    break;
                                case 15:
                                    cell.SetCellValue(dtExport.Rows[num5]["加氢量（kg）"].ToString());
                                    break;
                                case 16:
                                    cell.SetCellValue(dtExport.Rows[num5]["充电量（kWh）"].ToString());
                                    break;
                                case 17:
                                    cell.SetCellValue(dtExport.Rows[num5]["是否安装监控装置"].ToString());
                                    break;
                                case 18:
                                    cell.SetCellValue(dtExport.Rows[num5]["监控平台运行单位"].ToString());
                                    break;
                            }
                            cell.CellStyle = style;
                        }
                    }
                    //尾部
                    dataRow = sheet.CreateRow(n + dtRowNum);//新增行
                    cell = dataRow.CreateCell(0);
                    cell.SetCellValue(String.Format("*注：截至{0}年底的行驶里程", strMinDate));
                    cell.CellStyle = styleEnd;
                    sheet.AddMergedRegion(new CellRangeAddress(n + dtRowNum, n + dtRowNum, 0, 16));//合并

                    #endregion
                }
                //设定第一行，第一列的单元格选中
                sheet.SetActiveCell(0, 0);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 附件4： 新能源汽车推广应用车辆生产企业联络人
        /// </summary>
        /// <param name="dtExport"></param>
        /// <param name="exportTemplatePath"></param>
        /// <param name="strFilePath"></param>
        /// <param name="strTitle"></param>
        /// <param name="pb"></param>
        public static void DataTabletoUserExcel(System.Data.DataTable dtExport, string exportTemplatePath, string strFilePath, string strTitle)
        {
            if (dtExport == null)
                return;
            long num1 = (long)dtExport.Rows.Count;
            int num2 = dtExport.Columns.Count - 1;
            int num3 = num2;

            IWorkbook wk = null;
            using (FileStream fs = File.Open(exportTemplatePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                try
                {
                    wk = new XSSFWorkbook(fs);
                }
                catch (Exception ex)
                {
                    wk = new HSSFWorkbook(fs);
                }
                finally
                {
                    fs.Close();
                }
            }

            ImportExcelNPOI ieNOPI = new ImportExcelNPOI();

            try
            {
                //excel赋值
                string strMessage = WriterUserExcel(wk, dtExport, strTitle);

                wk.SetActiveSheet(0);//直接打开第一sheet

                //转为字节数组
                MemoryStream stream = new MemoryStream();
                wk.Write(stream);
                var buf = stream.ToArray();
                //保存文件
                SaveFile(strFilePath, buf);


            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //4
        public static string WriterUserExcel(IWorkbook hssfworkbookDown, DataTable dtExport, string strTitle)
        {
            try
            {
                ISheet sheet = hssfworkbookDown.GetSheetAt(0);
                ISheet sheetNew = null;
                IRow dataRow = null;
                ICell cell = null;

                #region

                //重写企业名称
                sheet.GetRow(3).GetCell(0).SetCellValue("车辆生产企业（盖章）:" + strTitle);


                dataRow = sheet.GetRow(5);//新能源负责人
                dataRow.GetCell(1).SetCellValue(dtExport.Rows[0]["Head_Name"].ToString());
                dataRow.GetCell(2).SetCellValue(dtExport.Rows[0]["Head_Name"].ToString());
                dataRow.GetCell(3).SetCellValue(dtExport.Rows[0]["Head_Department"].ToString());
                dataRow.GetCell(4).SetCellValue(dtExport.Rows[0]["Head_Post"].ToString());
                dataRow.GetCell(5).SetCellValue(dtExport.Rows[0]["Head_Tel"].ToString());
                dataRow.GetCell(6).SetCellValue(dtExport.Rows[0]["Head_Phone"].ToString());
                dataRow = sheet.GetRow(6);//主要联系人员
                dataRow.GetCell(1).SetCellValue(dtExport.Rows[0]["Head_Email"].ToString());
                dataRow.GetCell(2).SetCellValue(dtExport.Rows[0]["Contact_Name"].ToString());
                dataRow.GetCell(3).SetCellValue(dtExport.Rows[0]["Contact_Department"].ToString());
                dataRow.GetCell(4).SetCellValue(dtExport.Rows[0]["Contact_Post"].ToString());
                dataRow.GetCell(5).SetCellValue(dtExport.Rows[0]["Contact_Tel"].ToString());
                dataRow.GetCell(6).SetCellValue(dtExport.Rows[0]["Contact_Phone"].ToString());
                dataRow.GetCell(7).SetCellValue(dtExport.Rows[0]["Contact_Email"].ToString());

                #endregion

                //设定第一行，第一列的单元格选中
                sheet.SetActiveCell(0, 0);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        /// <summary>
        /// 保存文件方法
        /// </summary>
        /// <param name="strSaveFile"></param>
        /// <param name="buf"></param>
        public static void SaveFile(string strSaveFile, byte[] buf)
        {
            using (FileStream fs = new FileStream(strSaveFile, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }


    }
}
