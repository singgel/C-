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

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils
{
    public class ExportAccountInfo
    {
        public string ExportAccountTemplate(SaveFileDialog sflg,DataTable dt)
        {
            string message = string.Empty;
            try
            {
                string filename = sflg.FileName;
                NPOI.SS.UserModel.IWorkbook book = null;
                if (sflg.FilterIndex == 1)
                {
                    book = new NPOI.HSSF.UserModel.HSSFWorkbook();
                }
                else
                {
                    book = new NPOI.XSSF.UserModel.XSSFWorkbook();
                }
                //整体样式
                ICellStyle style = book.CreateCellStyle();
                style.FillPattern = FillPattern.NoFill;
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Top;
                style.WrapText = true;
                IFont font = book.CreateFont();
                font.FontHeightInPoints = 10;
                font.FontName = "宋体";
                style.SetFont(font);
                //style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                //一般样式
                ICellStyle style_center = book.CreateCellStyle();
                style_center.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                style_center.VerticalAlignment = VerticalAlignment.Center;
                style_center.WrapText = true;
                //style_center.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //style_center.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //style_center.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //style_center.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;

                //一般样式
                ICellStyle style_left = book.CreateCellStyle();
                style_left.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                style_left.WrapText = true;
                //style_left.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //style_left.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //style_left.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //style_left.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                


                NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("清算信息汇总");
                sheet.DefaultRowHeight = 20;
                for (int columnNum = 0; columnNum < 11; columnNum++)
                {
                    sheet.SetDefaultColumnStyle(columnNum, style);
                }

                int rowIndex = 0;
                #region 添加表头
                NPOI.SS.UserModel.IRow rowName = sheet.CreateRow(rowIndex);
                rowName.CreateCell(0, CellType.String).SetCellValue("地区");
                rowName.CreateCell(1, CellType.String).SetCellValue("序号");
                rowName.CreateCell(2, CellType.String).SetCellValue("车辆生产企业");
                rowName.CreateCell(3, CellType.String).SetCellValue("车辆型号");
                rowName.CreateCell(4, CellType.Numeric).SetCellValue("企业申报推广数");
                rowName.CreateCell(5, CellType.Numeric).SetCellValue("企业申请补助标准");
                rowName.CreateCell(6, CellType.Numeric).SetCellValue("企业申请清算资金");
                rowName.CreateCell(7, CellType.Numeric).SetCellValue("专家组核定的推广数");
                rowName.CreateCell(8, CellType.Numeric).SetCellValue("专家组核定的补助标准");
                rowName.CreateCell(9, CellType.Numeric).SetCellValue("应清算补助资金");
                rowName.CreateCell(10, CellType.String).SetCellValue("核减原因");

                rowIndex++;
                #endregion
                #region 添加数据
                int dealerNum = 0;
                //第一行总计
                var countENTNum = (from DataRow row in dt.Rows select new { ent_num = row["ENT_NUM"] }).Sum(a => Convert.ToDecimal(a.ent_num));
                var countENTMoney = (from DataRow row in dt.Rows select new { ent_money = row["SQBZBZ"] }).Sum(a => Convert.ToDecimal(a.ent_money));
                var countENTCount = (from DataRow row in dt.Rows select new { ent_count = row["ENT_COUNT"] }).Sum(a => Convert.ToDecimal(a.ent_count));
                var countAPPNum = (from DataRow row in dt.Rows select new { app_num = row["APP_NUM"] }).Sum(a => Convert.ToDecimal(a.app_num));
                var countAPPMoney = (from DataRow row in dt.Rows select new { app_money = row["APP_MONEY"] }).Sum(a => Convert.ToDecimal(a.app_money));
                var countAPPCount = (from DataRow row in dt.Rows select new { app_count = row["APP_COUNT"] }).Sum(a => Convert.ToDecimal(a.app_count));


                IRow rowTotal = sheet.CreateRow(rowIndex);
                ICell cellTotal = rowTotal.CreateCell(0);
                cellTotal.SetCellValue("总计");
                cellTotal.CellStyle = style_center;
                sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 0, 3));
                rowTotal.CreateCell(4, CellType.String).SetCellValue(countENTNum.ToString());
                rowTotal.CreateCell(5, CellType.String).SetCellValue(countENTMoney.ToString());
                rowTotal.CreateCell(6, CellType.String).SetCellValue(countENTCount.ToString());
                rowTotal.CreateCell(7, CellType.String).SetCellValue(countAPPNum.ToString());
                rowTotal.CreateCell(8, CellType.String).SetCellValue(countAPPMoney.ToString());
                rowTotal.CreateCell(9, CellType.String).SetCellValue(countAPPCount.ToString());
                rowIndex++;

                #region 循环省份写入
                var ProvinceList = dt.AsEnumerable().Select(d => d.Field<string>("DQ")).Distinct().ToList();
                for (int i = 0; i < ProvinceList.Count(); i++)
                {
                    dealerNum = 1;
                    //地区 省份
                    int DealerCount = dt.AsEnumerable().Where(d => d.Field<string>("DQ") == ProvinceList[i]).Select(d => d.Field<string>("CLSCQY")).Distinct().Count();
                    int ClxhCount = dt.AsEnumerable().Where(d => d.Field<string>("DQ") == ProvinceList[i]).Select(d => d.Field<string>("CLXH")).Count();
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + ClxhCount + DealerCount, 0, 0));
                    IRow rowSFTotal = sheet.CreateRow(rowIndex);
                    ICell cellSF = rowSFTotal.CreateCell(0);
                    cellSF.SetCellValue(ProvinceList[i]);

                    #region 省份合计

                    // IRow rowSFTotal = sheet.CreateRow(rowIndex);

                    var dvShengfen = dt.DefaultView;
                    dvShengfen.RowFilter = String.Format("DQ='{0}'", ProvinceList[i]);
                    var dtShengfen = dvShengfen.ToTable();
                    var sum_ENT_NUM = Convert.ToDecimal(dtShengfen.Compute("sum(ENT_NUM)", "TRUE"));
                    var sum_SQBZBZ = dtShengfen.Compute("sum(SQBZBZ)", "TRUE");
                    var sum_ENT_COUNT = dtShengfen.Compute("sum(ENT_COUNT)", "TRUE");
                    var sum_APP_NUM = dtShengfen.Compute("sum(APP_NUM)", "TRUE");
                    var sum_APP_MONEY = dtShengfen.Compute("sum(APP_MONEY)", "TRUE");
                    var sum_APP_COUNT = dtShengfen.Compute("sum(APP_COUNT)", "TRUE");

                   
                    sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex, 1, 3));
                    ICell cellSFTotal = rowSFTotal.CreateCell(1);
                    cellSFTotal.SetCellValue("合计");
                    cellSFTotal.CellStyle = style_center;
                    rowSFTotal.CreateCell(4, CellType.String).SetCellValue(sum_ENT_NUM.ToString());
                    rowSFTotal.CreateCell(5, CellType.String).SetCellValue(sum_SQBZBZ.ToString());
                    rowSFTotal.CreateCell(6, CellType.String).SetCellValue(sum_ENT_COUNT.ToString());
                    rowSFTotal.CreateCell(7, CellType.String).SetCellValue(sum_APP_NUM.ToString());
                    rowSFTotal.CreateCell(8, CellType.String).SetCellValue(sum_APP_MONEY.ToString());
                    rowSFTotal.CreateCell(9, CellType.String).SetCellValue(sum_APP_COUNT.ToString());
                    rowIndex++;
                    #endregion
                    #region 按照汽车生产企业写入

                    var DealerList = dt.AsEnumerable().Where(d => d.Field<string>("DQ") == ProvinceList[i]).Select(d => d.Field<string>("CLSCQY")).Distinct().ToList();
                    for (int j = 0; j < DealerList.Count(); j++)
                    {
                        //企业序号

                        int DealerClxhCount = dt.AsEnumerable().Where(d => d.Field<string>("DQ") == ProvinceList[i] && d.Field<string>("CLSCQY") == DealerList[j]).Select(d => d.Field<string>("CLXH")).Count();
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + DealerClxhCount, 1, 1));
                        IRow rowDealerTotal = sheet.CreateRow(rowIndex);
                        ICell cellDealerNum = rowDealerTotal.CreateCell(1);
                        cellDealerNum.SetCellValue(dealerNum);
                        

                        //企业名称
                        sheet.AddMergedRegion(new CellRangeAddress(rowIndex, rowIndex + DealerClxhCount, 2, 2));
                        ICell cellDealer = rowDealerTotal.CreateCell(2);
                        cellDealer.SetCellValue(DealerList[j]);
                        
                        //企业小计
                        var dvDealer = dt.DefaultView;
                        dvDealer.RowFilter = String.Format("CLSCQY='{0}' AND DQ = '{1}'", DealerList[j], ProvinceList[i]);
                        var dtDealer = dvDealer.ToTable();
                        var dealer_ENT_NUM = dtDealer.Compute("sum(ENT_NUM)", "TRUE");
                        var dealer_SQBZBZ = dtDealer.Compute("sum(SQBZBZ)", "TRUE");
                        var dealer_ENT_COUNT = dtDealer.Compute("sum(ENT_COUNT)", "TRUE");
                        var dealer_APP_NUM = dtDealer.Compute("sum(APP_NUM)", "TRUE");
                        var dealer_APP_MONEY = dtDealer.Compute("sum(APP_MONEY)", "TRUE");
                        var dealer_APP_COUNT = dtDealer.Compute("sum(APP_COUNT)", "TRUE");

                        rowDealerTotal.CreateCell(3, CellType.String).SetCellValue("小计");
                        rowDealerTotal.CreateCell(4, CellType.String).SetCellValue(dealer_ENT_NUM.ToString());
                        rowDealerTotal.CreateCell(5, CellType.String).SetCellValue(dealer_SQBZBZ.ToString());
                        rowDealerTotal.CreateCell(6, CellType.String).SetCellValue(dealer_ENT_COUNT.ToString());
                        rowDealerTotal.CreateCell(7, CellType.String).SetCellValue(dealer_APP_NUM.ToString());
                        rowDealerTotal.CreateCell(8, CellType.String).SetCellValue(dealer_APP_MONEY.ToString());
                        rowDealerTotal.CreateCell(9, CellType.String).SetCellValue(dealer_APP_COUNT.ToString());
                        rowIndex++;

                        //按照车辆型号写入
                        var detailData = dt.AsEnumerable().Where(d => d.Field<string>("DQ") == ProvinceList[i] && d.Field<string>("CLSCQY") == DealerList[j]).CopyToDataTable();

                        for (int k = 0; k < detailData.Rows.Count; k++)
                        {
                            IRow rowDealer = sheet.CreateRow(rowIndex);
                            rowDealer.CreateCell(3, CellType.String).SetCellValue(detailData.Rows[k]["CLXH"].ToString());
                            rowDealer.CreateCell(4, CellType.String).SetCellValue(detailData.Rows[k]["ENT_NUM"].ToString());
                            rowDealer.CreateCell(5, CellType.String).SetCellValue(detailData.Rows[k]["SQBZBZ"].ToString());
                            rowDealer.CreateCell(6, CellType.String).SetCellValue(detailData.Rows[k]["ENT_COUNT"].ToString());
                            rowDealer.CreateCell(7, CellType.String).SetCellValue(detailData.Rows[k]["APP_NUM"].ToString());
                            rowDealer.CreateCell(8, CellType.String).SetCellValue(detailData.Rows[k]["APP_MONEY"].ToString());
                            rowDealer.CreateCell(9, CellType.String).SetCellValue(detailData.Rows[k]["APP_COUNT"].ToString());
                            rowDealer.CreateCell(10, CellType.String).SetCellValue(detailData.Rows[k]["APP_RESULT"].ToString());
                            rowDealer.Cells[7].CellStyle = style_left;

                            rowIndex++;
                        }
                        dealerNum++;
                    }
                    #endregion
                }
                for (int colNum = 0; colNum <= 10; colNum++)
                {
                    int columnWidth = sheet.GetColumnWidth(colNum) / 256;
                    for (int rowNum = 0; rowNum < sheet.LastRowNum; rowNum++)
                    {
                        IRow currentRow = sheet.GetRow(rowNum);
                        ICell currentCell = currentRow.GetCell(colNum);
                        if (currentCell != null)
                        {
                            int length = Encoding.UTF8.GetBytes(currentCell.ToString()).Length;
                            if (columnWidth < length + 1)
                            {
                                columnWidth = length + 1 > 254 ? 255 : length + 1;
                            }
                            currentRow.HeightInPoints = 20;
                        }
                    }
                    sheet.SetColumnWidth(colNum, columnWidth * 256);
                }


                sheet.ProtectSheet("1");
                #endregion
                #endregion
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                book.Write(ms);
                book = null;

                using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }

                ms.Close();
                ms.Dispose();
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            return message;      
                  
        }
    }
}
