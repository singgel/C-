using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Catarc.Adc.NewEnergyAccountSys.OfficeHelper;
using System.Text.RegularExpressions;
using System.Data;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using Catarc.Adc.NewEnergyAccountSys.Common;
using System.Diagnostics;
using Catarc.Adc.NewEnergyAccountSys.LogUtils;

namespace Catarc.Adc.NewEnergyAccountSys.FormUtils.DataUtils
{
    public class ImportNewInfoUtils
    {
        Microsoft.Office.Interop.Access.Dao.DBEngine dbEngine = new Microsoft.Office.Interop.Access.Dao.DBEngine();

        /// <summary>
        /// 导入新模板数据
        /// </summary>
        /// <param name="floderPath">模板所在路径</param>
        /// <returns></returns>
        /// 
        public string ImportNewTemplate(string floderPath)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            string msg = string.Empty;
            var TheFolder = new DirectoryInfo(floderPath);
            //Step1：校验模板,看是否齐全，并修改附件中图片后缀
            if (TheFolder.GetFiles("附件2*.xlsx").Length < 1)
            {
                msg += "当前路径下缺少附件2*.xlsx类型文件" + Environment.NewLine;
            }
            if (TheFolder.GetFiles("附件3*.xlsx").Length < 1)
            {
                msg += "当前路径下缺少附件3*.xlsx类型文件" + Environment.NewLine;
            }
            if (TheFolder.GetDirectories("附件6*").Length < 1)
            {
                msg += "当前路径下缺少附件6*类型文件夹" + Environment.NewLine;
            }
            if (TheFolder.GetDirectories("附件7*").Length < 1)
            {
                msg += "当前路径下缺少附件7*类型文件夹" + Environment.NewLine;
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step1：校验模板,看是否齐全，并修改附件中图片后缀耗时：{0}时{1}分{2}秒", ts.Hours, ts.Minutes, ts.Seconds));
            stopWatch.Start();
            if (!string.IsNullOrEmpty(msg))
            {
                return msg;
            }
            //Step2：统一图片后缀为.jpg，拷贝缩略图
            this.copyPictureTemplate6(TheFolder);
            this.copyPictureTemplate7(TheFolder);
            stopWatch.Stop();
            TimeSpan ts5 = stopWatch.Elapsed - ts;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step2：统一图片后缀为.jpg，拷贝缩略图耗时：{0}时{1}分{2}秒", ts5.Hours, ts5.Minutes, ts5.Seconds));
            stopWatch.Start();
            //Step3：读取模板，按照固定的行列读取数据
            var dtTemp2 = this.readExcelTemplate2(TheFolder.GetFiles("附件2*.xlsx")[0].FullName);
            var dtTemp3 = this.readExcelTemplate3(TheFolder.GetFiles("附件3*.xlsx")[0].FullName);
            stopWatch.Stop();
            TimeSpan ts2 = stopWatch.Elapsed - ts5;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step3：读取模板，按照固定的行列读取耗时：{0}时{1}分{2}秒", ts2.Hours, ts2.Minutes, ts2.Seconds));
            stopWatch.Start();
            //Step4：分析模板，看附件2*.xlsx对应的车牌在附件3*.xlsx中是否存在,剔除双方不匹配的行
            msg += this.MatchDataTable(ref dtTemp2, ref dtTemp3);
            stopWatch.Stop();
            TimeSpan ts3 = stopWatch.Elapsed - ts2;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step4：分析模板，看附件2*.xlsx对应的车牌在附件3*.xlsx中是否存在耗时：{0}时{1}分{2}秒", ts3.Hours, ts3.Minutes, ts3.Seconds));
            stopWatch.Start();
            if (dtTemp2.Rows.Count == 0)
            {
                return msg;
            }
            //Step5：检验VIN、车辆牌照、发票号是否在系统中已存在,剔除双方不匹配的行
            msg += this.UniqueKeyDataTable(ref dtTemp2, ref dtTemp3);
            stopWatch.Stop();
            TimeSpan ts4 = stopWatch.Elapsed - ts3;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step5：检验VIN、车辆牌照是否在系统中已存在耗时：{0}时{1}分{2}秒", ts4.Hours, ts4.Minutes, ts4.Seconds));
            stopWatch.Start();
            if (dtTemp2.Rows.Count == 0)
            {
                return msg;
            }
            //Step6：插入数据
            string sqlMsg = this.insertDataTable(dtTemp2, dtTemp3);
            if (!string.IsNullOrEmpty(sqlMsg))
            {
                msg += sqlMsg;
            }
            stopWatch.Stop();
            TimeSpan ts6 = stopWatch.Elapsed - ts4;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step6：插入数据耗时==========：{0}时{1}分{2}秒", ts6.Hours, ts6.Minutes, ts6.Seconds));
            //Step7：提示旧模板不存在车辆性质和公告批次
            msg += String.Format("请注意：新模板数据导入操作完成{0}", Environment.NewLine);
            return msg;
        }

        /// <summary>
        /// 删除选中数据
        /// </summary>
        /// <param name="obj">要删除的控件信息</param>
        /// <returns></returns>
        public string deleteDataInfo(DataTable selectedDT)
        {
            string msg = string.Empty;
            using (var con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                for (int i = 0; i < selectedDT.Rows.Count; i++)
                {
                    using (var tra = con.BeginTransaction())
                    {
                        string vin = selectedDT.Rows[i]["VIN"].ToString().Trim();
                        try
                        {
                            if (AccessHelper.ExecuteNonQuery(AccessHelper.conn, String.Format("delete from INFOMATION_ENTITIES where VIN='{0}'", vin)) > 0)
                            {
                                msg += String.Format("车辆识别代码(VIN)：{0}成功从本系统中删除，操作成功！{1}", vin, Environment.NewLine);
                            }
                            else
                            {
                                msg += String.Format("车辆识别代码(VIN)：{0}未能从本系统中删除，操作失败！{1}", vin, Environment.NewLine);
                            }
                            tra.Commit();
                        }
                        catch (Exception ex)
                        {
                            tra.Rollback();
                            msg += String.Format("车辆识别代码(VIN)：{0}的异常信息{1}，操作异常！{2}", vin, ex.Message, Environment.NewLine);
                        }
                    }
                    string fptp = String.Format("{0}\\{1}", Utils.billImage, selectedDT.Rows[i]["FPTP"].ToString().Trim());
                    if (File.Exists(fptp))
                    {
                        File.Delete(fptp);
                    }
                    string xsztp = String.Format("{0}\\{1}", Utils.driveImage, selectedDT.Rows[i]["XSZTP"].ToString().Trim());
                    if (File.Exists(xsztp))
                    {
                        File.Delete(xsztp);
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// 拷贝，缩略发票图片
        /// </summary>
        /// <param name="dtTemp2">要录入的VIN信息</param>
        /// <returns></returns>
        private string copyPictureTemplate6(DirectoryInfo theFolder)
        {
            string msg = string.Empty;
            if (!Directory.Exists(Utils.billImage))
            {
                Directory.CreateDirectory(Utils.billImage);
            } 
            var billFolder = new DirectoryInfo(theFolder.GetDirectories("附件6*")[0].FullName);
            Array.ForEach(billFolder.GetFiles(), NextFile =>
            {
                try
                {
                    System.IO.File.Copy(NextFile.FullName, String.Format("{0}\\{1}.jpg", Utils.billImage, Path.GetFileNameWithoutExtension(NextFile.FullName)), false);
                }
                catch(Exception ex)
                {
                    LogManager.Log("Error", "error", String.Format("{0}发票导入重名，失败原因：{1}", NextFile.Name, ex.Message));
                }
            });
            return msg;
        }

        /// <summary>
        /// 拷贝、缩略行驶证图片
        /// </summary>
        /// <param name="dtTemp2">要录入的VIN信息</param>
        /// <returns></returns>
        private string copyPictureTemplate7(DirectoryInfo theFolder)
        {
            string msg = string.Empty;
            if (!Directory.Exists(Utils.driveImage))
            {
                Directory.CreateDirectory(Utils.driveImage);
            } 
            var driveFolder = new DirectoryInfo(theFolder.GetDirectories("附件7*")[0].FullName);
            Array.ForEach(driveFolder.GetFiles(), NextFile =>
            {
                try
                {
                    System.IO.File.Copy(NextFile.FullName, String.Format("{0}\\{1}.jpg", Utils.driveImage, Path.GetFileNameWithoutExtension(NextFile.FullName)), false);
                }
                catch (Exception ex)
                {
                    LogManager.Log("Error", "error", String.Format("{0}行驶证导入重名，失败原因：{1}", NextFile.Name, ex.Message));
                }
            });
            return msg;
        }

        /// <summary>
        /// 读取附件2
        /// </summary>
        /// <param name="filePath">附件路径</param>
        /// <returns></returns>
        private DataTable readExcelTemplate2(string filePath)
        {
            var dt2 = ImportExcel.ReadExcelToDataSet(filePath, "附件2").Tables[0];
            var dicCity = QueryHelper.queryProvinceOfCity();
            var dtTemp2 = new DataTable();
            dtTemp2.Columns.Add("CLXZ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("GCCS", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLYXDW", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLZL", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLYT", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLXH", Type.GetType("System.String"));
            dtTemp2.Columns.Add("EKGZ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("GGPC", Type.GetType("System.String"));
            dtTemp2.Columns.Add("VIN", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLPZ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("SQBZBZ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("GMJG", Type.GetType("System.String"));
            dtTemp2.Columns.Add("FPHM", Type.GetType("System.String"));
            dtTemp2.Columns.Add("FPSJ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("XSZSJ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLSFYCJDR", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCDTXX_XH", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_DTXH", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCDTXX_SCQY", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_DTSCQY", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCZXX_XH", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_CXXH", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCZXX_ZRL", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_DRZRL", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCZXX_SCQY", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_DRZSCQY", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCZXX_XTJG", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_XTJG", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DCZXX_ZBNX", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CJDRXX_ZBNX", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLSFYQDDJ2", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_XH_1", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_XH_2", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_EDGL_1", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_EDGL_2", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_SCQY_1", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_SCQY_2", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_XTJG_1", Type.GetType("System.String"));
            dtTemp2.Columns.Add("QDDJXX_XTJG_2", Type.GetType("System.String"));
            dtTemp2.Columns.Add("CLSFYRLDC", Type.GetType("System.String"));
            dtTemp2.Columns.Add("RLDCXX_XH", Type.GetType("System.String"));
            dtTemp2.Columns.Add("RLDCXX_EDGL", Type.GetType("System.String"));
            dtTemp2.Columns.Add("RLDCXX_SCQY", Type.GetType("System.String"));
            dtTemp2.Columns.Add("RLDCXX_GMJG", Type.GetType("System.String"));
            dtTemp2.Columns.Add("RLDCXX_ZBNX", Type.GetType("System.String"));
            dtTemp2.Columns.Add("JZNF", Type.GetType("System.String"));
            dtTemp2.Columns.Add("GCSF", Type.GetType("System.String"));
            dtTemp2.Columns.Add("FPTP", Type.GetType("System.String"));
            dtTemp2.Columns.Add("FPTP_PICTURE", Type.GetType("System.String"));
            dtTemp2.Columns.Add("XSZTP", Type.GetType("System.String"));
            dtTemp2.Columns.Add("XSZTP_PICTURE", Type.GetType("System.String"));
            for (int i = 4; i < dt2.Rows.Count; i++)
            {
                if (!Regex.IsMatch(dt2.Rows[i][0].ToString().Trim(), @"^[+-]?\d*$")) break;
                dtTemp2.Rows.Add();
                dtTemp2.Rows[i - 4][0] = dt2.Rows[i][1].ToString().Trim();
                dtTemp2.Rows[i - 4][1] = dt2.Rows[i][2].ToString().Trim();
                dtTemp2.Rows[i - 4][2] = dt2.Rows[i][3].ToString().Trim();
                dtTemp2.Rows[i - 4][3] = dt2.Rows[i][4].ToString().Trim();
                dtTemp2.Rows[i - 4][4] = dt2.Rows[i][5].ToString().Trim();
                dtTemp2.Rows[i - 4][5] = dt2.Rows[i][6].ToString().Trim();
                dtTemp2.Rows[i - 4][6] = dt2.Rows[i][7].ToString().Trim();
                dtTemp2.Rows[i - 4][7] = dt2.Rows[i][8].ToString().Trim();
                dtTemp2.Rows[i - 4][8] = dt2.Rows[i][9].ToString().Trim();
                dtTemp2.Rows[i - 4][9] = dt2.Rows[i][10].ToString().Trim();
                dtTemp2.Rows[i - 4][10] = dt2.Rows[i][11].ToString().Trim();
                dtTemp2.Rows[i - 4][11] = dt2.Rows[i][12].ToString().Trim();
                dtTemp2.Rows[i - 4][12] = dt2.Rows[i][13].ToString().Trim();
                dtTemp2.Rows[i - 4][13] = String.Format("{0}/{1}/{2}", dt2.Rows[i][14].ToString().Trim(), dt2.Rows[i][15].ToString().Trim().Length == 2 ? dt2.Rows[i][15].ToString().Trim() : "0" + dt2.Rows[i][15].ToString().Trim(), dt2.Rows[i][16].ToString().Trim().Length == 2 ? dt2.Rows[i][16].ToString().Trim() : "0" + dt2.Rows[i][16].ToString().Trim());
                dtTemp2.Rows[i - 4][14] = String.Format("{0}/{1}/{2}", dt2.Rows[i][17].ToString().Trim(), dt2.Rows[i][18].ToString().Trim().Length == 2 ? dt2.Rows[i][18].ToString().Trim() : "0" + dt2.Rows[i][18].ToString().Trim(), dt2.Rows[i][19].ToString().Trim().Length == 2 ? dt2.Rows[i][19].ToString().Trim() : "0" + dt2.Rows[i][19].ToString().Trim());
                dtTemp2.Rows[i - 4][15] = dt2.Rows[i][20].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][21].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][22].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][23].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][24].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][25].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][26].ToString().Trim().Split('|').Length > 1 ? "是" : "否";
                dtTemp2.Rows[i - 4][16] = dt2.Rows[i][20].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][20].ToString().Trim().Split('|')[0] : dt2.Rows[i][20].ToString().Trim();
                dtTemp2.Rows[i - 4][17] = dt2.Rows[i][20].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][20].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][18] = dt2.Rows[i][21].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][21].ToString().Trim().Split('|')[0] : dt2.Rows[i][21].ToString().Trim();
                dtTemp2.Rows[i - 4][19] = dt2.Rows[i][21].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][21].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][20] = dt2.Rows[i][22].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][22].ToString().Trim().Split('|')[0] : dt2.Rows[i][22].ToString().Trim();
                dtTemp2.Rows[i - 4][21] = dt2.Rows[i][22].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][22].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][22] = dt2.Rows[i][23].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][23].ToString().Trim().Split('|')[0] : dt2.Rows[i][23].ToString().Trim();
                dtTemp2.Rows[i - 4][23] = dt2.Rows[i][23].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][23].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][24] = dt2.Rows[i][24].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][24].ToString().Trim().Split('|')[0] : dt2.Rows[i][24].ToString().Trim();
                dtTemp2.Rows[i - 4][25] = dt2.Rows[i][24].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][24].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][26] = dt2.Rows[i][25].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][25].ToString().Trim().Split('|')[0] : dt2.Rows[i][25].ToString().Trim();
                dtTemp2.Rows[i - 4][27] = dt2.Rows[i][25].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][25].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][28] = dt2.Rows[i][26].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][26].ToString().Trim().Split('|')[0] : dt2.Rows[i][26].ToString().Trim();
                dtTemp2.Rows[i - 4][29] = dt2.Rows[i][26].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][26].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][30] = dt2.Rows[i][27].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][28].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][29].ToString().Trim().Split('|').Length > 1 || dt2.Rows[i][30].ToString().Trim().Split('|').Length > 1 ? "是" : "否";
                dtTemp2.Rows[i - 4][31] = dt2.Rows[i][27].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][27].ToString().Trim().Split('|')[0] : dt2.Rows[i][27].ToString().Trim();
                dtTemp2.Rows[i - 4][32] = dt2.Rows[i][27].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][27].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][33] = dt2.Rows[i][28].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][28].ToString().Trim().Split('|')[0] : dt2.Rows[i][28].ToString().Trim();
                dtTemp2.Rows[i - 4][34] = dt2.Rows[i][28].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][28].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][35] = dt2.Rows[i][29].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][29].ToString().Trim().Split('|')[0] : dt2.Rows[i][29].ToString().Trim();
                dtTemp2.Rows[i - 4][36] = dt2.Rows[i][29].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][29].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][37] = dt2.Rows[i][30].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][30].ToString().Trim().Split('|')[0] : dt2.Rows[i][30].ToString().Trim();
                dtTemp2.Rows[i - 4][38] = dt2.Rows[i][30].ToString().Trim().Split('|').Length > 1 ? dt2.Rows[i][30].ToString().Trim().Split('|')[1] : string.Empty;
                dtTemp2.Rows[i - 4][39] = !string.IsNullOrEmpty(dt2.Rows[i][31].ToString().Trim()) || !string.IsNullOrEmpty(dt2.Rows[i][32].ToString().Trim()) || !string.IsNullOrEmpty(dt2.Rows[i][33].ToString().Trim()) || !string.IsNullOrEmpty(dt2.Rows[i][34].ToString().Trim()) || !string.IsNullOrEmpty(dt2.Rows[i][35].ToString().Trim()) ? "是" : "否";
                dtTemp2.Rows[i - 4][40] = dt2.Rows[i][31].ToString().Trim();
                dtTemp2.Rows[i - 4][41] = dt2.Rows[i][32].ToString().Trim();
                dtTemp2.Rows[i - 4][42] = dt2.Rows[i][33].ToString().Trim();
                dtTemp2.Rows[i - 4][43] = dt2.Rows[i][34].ToString().Trim();
                dtTemp2.Rows[i - 4][44] = dt2.Rows[i][35].ToString().Trim();
                dtTemp2.Rows[i - 4][45] = dt2.Rows[0][0].ToString().Trim().Length > 4 ? dt2.Rows[0][0].ToString().Trim().Substring(0, 4) : string.Empty;
                dtTemp2.Rows[i - 4][46] = dicCity.ContainsKey(dt2.Rows[i][2].ToString().Trim()) ? dicCity[dt2.Rows[i][2].ToString().Trim()] : string.Empty;
                string vin = dt2.Rows[i][9].ToString().Trim();
                dtTemp2.Rows[i - 4][47] = File.Exists(Path.Combine(Utils.billImage, String.Format("车辆发票VIN-{0}.jpg", vin))) ? String.Format("车辆发票VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][48] = File.Exists(Path.Combine(Utils.billImage, String.Format("车辆发票VIN-{0}.jpg", vin))) ? String.Format("车辆发票VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][49] = File.Exists(Path.Combine(Utils.driveImage, String.Format("行驶证VIN-{0}.jpg", vin))) ? String.Format("行驶证VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][50] = File.Exists(Path.Combine(Utils.driveImage, String.Format("行驶证VIN-{0}.jpg", vin))) ? String.Format("行驶证VIN-{0}.jpg", vin) : string.Empty;
            }
            return dtTemp2;
        }

        /// <summary>
        /// 读取附件3
        /// </summary>
        /// <param name="filePath">附件路径</param>
        /// <returns></returns>
        private DataTable readExcelTemplate3(string filePath)
        {
            var dt3 = ImportExcel.ReadExcelToDataSet(filePath, "附件3").Tables[0];
            var dtTemp3 = new DataTable();
            dtTemp3.Columns.Add("GCCS", Type.GetType("System.String"));
            dtTemp3.Columns.Add("CLZL", Type.GetType("System.String"));
            dtTemp3.Columns.Add("CLYT", Type.GetType("System.String"));
            dtTemp3.Columns.Add("CLPZ", Type.GetType("System.String"));
            dtTemp3.Columns.Add("CLCMYCDNGXSLC", Type.GetType("System.String"));
            dtTemp3.Columns.Add("CLYCCMDSXSJ", Type.GetType("System.String"));
            dtTemp3.Columns.Add("ZDCDGL", Type.GetType("System.String"));
            dtTemp3.Columns.Add("LJXSLC", Type.GetType("System.String"));
            dtTemp3.Columns.Add("YJXSLC", Type.GetType("System.String"));
            dtTemp3.Columns.Add("BGLHDL", Type.GetType("System.String"));
            dtTemp3.Columns.Add("PJDRXYSJ", Type.GetType("System.String"));
            dtTemp3.Columns.Add("LJJYL", Type.GetType("System.String"));
            dtTemp3.Columns.Add("LJJQL_G", Type.GetType("System.String"));
            dtTemp3.Columns.Add("LJJQL_L", Type.GetType("System.String"));
            dtTemp3.Columns.Add("LJJQL", Type.GetType("System.String"));
            dtTemp3.Columns.Add("LJCDL", Type.GetType("System.String"));
            dtTemp3.Columns.Add("SFAZJKZZ", Type.GetType("System.String"));
            dtTemp3.Columns.Add("JKPDXXDW", Type.GetType("System.String"));
            for (int i = 4; i < dt3.Rows.Count; i++)
            {
                if (!Regex.IsMatch(dt3.Rows[i][0].ToString().Trim(), @"^[+-]?\d*$")) break;
                dtTemp3.Rows.Add();
                dtTemp3.Rows[i - 4][0] = dt3.Rows[i][1].ToString().Trim();
                dtTemp3.Rows[i - 4][1] = dt3.Rows[i][2].ToString().Trim();
                dtTemp3.Rows[i - 4][2] = dt3.Rows[i][3].ToString().Trim();
                dtTemp3.Rows[i - 4][3] = dt3.Rows[i][4].ToString().Trim();
                dtTemp3.Rows[i - 4][4] = dt3.Rows[i][5].ToString().Trim();
                dtTemp3.Rows[i - 4][5] = dt3.Rows[i][6].ToString().Trim();
                dtTemp3.Rows[i - 4][6] = dt3.Rows[i][7].ToString().Trim();
                dtTemp3.Rows[i - 4][7] = dt3.Rows[i][8].ToString().Trim();
                dtTemp3.Rows[i - 4][8] = dt3.Rows[i][9].ToString().Trim();
                dtTemp3.Rows[i - 4][9] = dt3.Rows[i][10].ToString().Trim();
                dtTemp3.Rows[i - 4][10] = dt3.Rows[i][11].ToString().Trim();
                dtTemp3.Rows[i - 4][11] = dt3.Rows[i][12].ToString().Trim();
                dtTemp3.Rows[i - 4][12] = dt3.Rows[i][13].ToString().Trim();
                dtTemp3.Rows[i - 4][13] = dt3.Rows[i][14].ToString().Trim();
                dtTemp3.Rows[i - 4][14] = dt3.Rows[i][15].ToString().Trim();
                dtTemp3.Rows[i - 4][15] = dt3.Rows[i][16].ToString().Trim();
                dtTemp3.Rows[i - 4][16] = dt3.Rows[i][17].ToString().Trim();
                dtTemp3.Rows[i - 4][17] = dt3.Rows[i][18].ToString().Trim();
            }
            return dtTemp3;
        }

        /// <summary>
        /// 根据车辆牌照匹配两个dataTable的行，不匹配的删掉
        /// </summary>
        /// <param name="dtTemp2">附件2的模板数据</param>
        /// <param name="dtTemp3">附件3的模板数据</param>
        /// <returns></returns>
        private string MatchDataTable(ref DataTable dtTemp2, ref DataTable dtTemp3)
        {
            string msg = string.Empty;
            dtTemp2 = dtTemp2.AsEnumerable().Cast<DataRow>().GroupBy(p => p.Field<string>("VIN")).Select(p => p.FirstOrDefault()).CopyToDataTable();
            dtTemp2 = dtTemp2.AsEnumerable().Cast<DataRow>().GroupBy(p => p.Field<string>("CLPZ")).Select(p => p.FirstOrDefault()).CopyToDataTable();
            dtTemp3 = dtTemp3.AsEnumerable().Cast<DataRow>().GroupBy(p => p.Field<string>("CLPZ")).Select(p => p.FirstOrDefault()).CopyToDataTable();
            var clpzArr2 = dtTemp2.AsEnumerable().Select(d => d.Field<string>("CLPZ")).Distinct().ToArray();
            var clpzArr3 = dtTemp3.AsEnumerable().Select(d => d.Field<string>("CLPZ")).Distinct().ToArray();
            var clpzOutArr2 = (from t2 in dtTemp2.AsEnumerable()
                               where !clpzArr3.Contains(t2.Field<string>("CLPZ"))
                               select t2.Field<string>("CLPZ")).ToArray<string>();
            var clpzOutArr3 = (from t3 in dtTemp3.AsEnumerable()
                               where !clpzArr2.Contains(t3.Field<string>("CLPZ"))
                               select t3.Field<string>("CLPZ")).ToArray<string>();
            var dt2 = from t2 in dtTemp2.AsEnumerable()
                      where clpzArr3.Contains(t2.Field<string>("CLPZ"))
                      select t2;
            var dt3 = from t3 in dtTemp3.AsEnumerable()
                      where clpzArr2.Contains(t3.Field<string>("CLPZ"))
                      select t3;
            if (dt2.Count() > 0)
            {
                dtTemp2 = dt2.CopyToDataTable(); ;
            }
            else
            {
                dtTemp2.Clear();
            }
            if (dt3.Count() > 0)
            {
                dtTemp3 = dt3.CopyToDataTable(); ;
            }
            else
            {
                dtTemp3.Clear();
            }
            if (clpzOutArr2.Length > 0)
                msg += String.Format("附件2*.xlsx车辆牌照：{0}在附件3*.xlsx中没有找到对应信息！{1}", string.Join(",", clpzOutArr2), Environment.NewLine);
            if (clpzOutArr3.Length > 0)
                msg += String.Format("附件3*.xlsx车辆牌照：{0}在附件2*.xlsx中没有找到对应信息！{1}", string.Join(",", clpzOutArr3), Environment.NewLine);
            return msg;
        }

        /// <summary>
        /// 去除车辆识别代码(VIN)、车辆牌照已存在的数据
        /// </summary>
        /// <param name="dtTemp2">附件2的模板数据</param>
        /// <param name="dtTemp3">附件3的模板数据</param>
        /// <returns></returns>
        private string UniqueKeyDataTable(ref DataTable dtTemp2, ref DataTable dtTemp3)
        {
            string msg = string.Empty;
            var dtXT = AccessHelper.ExecuteDataSet(AccessHelper.conn, "select * from INFOMATION_ENTITIES").Tables[0];
            var vinArr = dtXT.AsEnumerable().Select(d => d.Field<string>("VIN")).Distinct().ToArray();
            var clpzArr = dtXT.AsEnumerable().Select(d => d.Field<string>("CLPZ")).Distinct().ToArray();
           
            var vinRepeatArr = (from t2 in dtTemp2.AsEnumerable()
                                where vinArr.Contains(t2.Field<string>("VIN"))
                                select t2.Field<string>("VIN")).ToArray<string>();
            var clpzRepeatArr = (from t2 in dtTemp2.AsEnumerable()
                                 where clpzArr.Contains(t2.Field<string>("CLPZ"))
                                 select t2.Field<string>("CLPZ")).ToArray<string>();
           
            var dt2 = from t2 in dtTemp2.AsEnumerable()
                      where !vinArr.Contains(t2.Field<string>("VIN"))
                      && !clpzArr.Contains(t2.Field<string>("CLPZ"))
                      select t2;
            var dt3 = from t3 in dtTemp3.AsEnumerable()
                      where !clpzArr.Contains(t3.Field<string>("CLPZ"))
                      select t3;
            if (dt2.Count() > 0)
            {
                dtTemp2 = dt2.CopyToDataTable(); ;
            }
            else
            {
                dtTemp2.Clear();
            }
            if (dt3.Count() > 0)
            {
                dtTemp3 = dt3.CopyToDataTable(); ;
            }
            else
            {
                dtTemp3.Clear();
            }
            if (vinRepeatArr.Length > 0)
                msg += String.Format("车辆识别代码(VIN)：{0}已在本系统中收录！{1}", string.Join(",", vinRepeatArr), Environment.NewLine);
            if (clpzRepeatArr.Length > 0)
                msg += String.Format("车辆牌照：{0}已在本系统中收录！{1}", string.Join(",", clpzRepeatArr), Environment.NewLine);
            return msg;
        }

        /// <summary>
        /// 录入附件信息
        /// </summary>
        /// <param name="dtTemp2">附件2的模板数据</param>
        /// <param name="dtTemp3">附件3的模板数据</param>
        /// <returns></returns>
        private string insertDataTable(DataTable dtTemp2, DataTable dtTemp3)
        {
            string msg = string.Empty;

            Microsoft.Office.Interop.Access.Dao.Database db = dbEngine.OpenDatabase(Utils.dataPath);
            Microsoft.Office.Interop.Access.Dao.Recordset rs = db.OpenRecordset("INFOMATION_ENTITIES");
            Microsoft.Office.Interop.Access.Dao.Field[] myFields = new Microsoft.Office.Interop.Access.Dao.Field[65];
            
            myFields[0] = rs.Fields["VIN"];
            myFields[1] = rs.Fields["CLXZ"];
            myFields[2] = rs.Fields["CLZL"];
            myFields[3] = rs.Fields["GCSF"];
            myFields[4] = rs.Fields["GCCS"];
            myFields[5] = rs.Fields["CLYT"];
            myFields[6] = rs.Fields["CLXH"];
            myFields[7] = rs.Fields["GGPC"];
            myFields[8] = rs.Fields["CLPZ"];
            myFields[9] = rs.Fields["EKGZ"];
            myFields[10] = rs.Fields["GMJG"];
            myFields[11] = rs.Fields["SQBZBZ"];
            myFields[12] = rs.Fields["FPHM"];
            myFields[13] = rs.Fields["FPSJ"];
            myFields[14] = rs.Fields["XSZSJ"];
            myFields[15] = rs.Fields["FPTP"];
            myFields[16] = rs.Fields["XSZTP"];
            myFields[17] = rs.Fields["FPTP_PICTURE"];
            myFields[18] = rs.Fields["XSZTP_PICTURE"];
            myFields[19] = rs.Fields["CJDRXX_CXXH"];
            myFields[20] = rs.Fields["CJDRXX_DRZRL"];
            myFields[21] = rs.Fields["CJDRXX_DRZSCQY"];
            myFields[22] = rs.Fields["CJDRXX_DTSCQY"];
            myFields[23] = rs.Fields["CJDRXX_DTXH"];
            myFields[24] = rs.Fields["CJDRXX_XTJG"];
            myFields[25] = rs.Fields["CJDRXX_ZBNX"];
            myFields[26] = rs.Fields["CLSFYCJDR"];
            myFields[27] = rs.Fields["CLSFYQDDJ2"];
            myFields[28] = rs.Fields["CLSFYRLDC"];
            myFields[29] = rs.Fields["DCDTXX_SCQY"];
            myFields[30] = rs.Fields["DCDTXX_XH"];
            myFields[31] = rs.Fields["DCZXX_SCQY"];
            myFields[32] = rs.Fields["DCZXX_XH"];
            myFields[33] = rs.Fields["DCZXX_XTJG"];
            myFields[34] = rs.Fields["DCZXX_ZBNX"];
            myFields[35] = rs.Fields["DCZXX_ZRL"];
            myFields[36] = rs.Fields["QDDJXX_EDGL_1"];
            myFields[37] = rs.Fields["QDDJXX_EDGL_2"];
            myFields[38] = rs.Fields["QDDJXX_SCQY_1"];
            myFields[39] = rs.Fields["QDDJXX_SCQY_2"];
            myFields[40] = rs.Fields["QDDJXX_XH_1"];
            myFields[41] = rs.Fields["QDDJXX_XH_2"];
            myFields[42] = rs.Fields["QDDJXX_XTJG_1"];
            myFields[43] = rs.Fields["QDDJXX_XTJG_2"];
            myFields[44] = rs.Fields["RLDCXX_EDGL"];
            myFields[45] = rs.Fields["RLDCXX_GMJG"];
            myFields[46] = rs.Fields["RLDCXX_SCQY"];
            myFields[47] = rs.Fields["RLDCXX_XH"];
            myFields[48] = rs.Fields["RLDCXX_ZBNX"];
            myFields[49] = rs.Fields["JZNF"];
            myFields[50] = rs.Fields["BGLHDL"];
            myFields[51] = rs.Fields["CLCMYCDNGXSLC"];
            myFields[52] = rs.Fields["CLYCCMDSXSJ"];
            myFields[53] = rs.Fields["CLYXDW"];
            myFields[54] = rs.Fields["JKPDXXDW"];
            myFields[55] = rs.Fields["LJCDL"];
            myFields[56] = rs.Fields["LJJQL"];
            myFields[57] = rs.Fields["LJJQL_G"];
            myFields[58] = rs.Fields["LJJQL_L"];
            myFields[59] = rs.Fields["LJJYL"];
            myFields[60] = rs.Fields["LJXSLC"];
            myFields[61] = rs.Fields["PJDRXYSJ"];
            myFields[62] = rs.Fields["SFAZJKZZ"];
            myFields[63] = rs.Fields["YJXSLC"];
            myFields[64] = rs.Fields["ZDCDGL"];
            for (int i = 0; i < dtTemp2.Rows.Count; i++)
            {
                string vin = dtTemp2.Rows[i]["VIN"].ToString().Trim();
                string clpz = dtTemp2.Rows[i]["CLPZ"].ToString().Trim();
                try
                {

                    var sel3 = (from t3 in dtTemp3.AsEnumerable()
                                where t3.Field<string>("CLPZ").Equals(clpz)
                                select t3).CopyToDataTable();
                    rs.AddNew();
                    myFields[0].Value = dtTemp2.Rows[i]["VIN"];
                    myFields[1].Value = dtTemp2.Rows[i]["CLXZ"];
                    myFields[2].Value = dtTemp2.Rows[i]["CLZL"];
                    myFields[3].Value = dtTemp2.Rows[i]["GCSF"];
                    myFields[4].Value = dtTemp2.Rows[i]["GCCS"];
                    myFields[5].Value = dtTemp2.Rows[i]["CLYT"];
                    myFields[6].Value = dtTemp2.Rows[i]["CLXH"];
                    myFields[7].Value = dtTemp2.Rows[i]["GGPC"];
                    myFields[8].Value = dtTemp2.Rows[i]["CLPZ"];
                    myFields[9].Value = dtTemp2.Rows[i]["EKGZ"];
                    myFields[10].Value = dtTemp2.Rows[i]["GMJG"];
                    myFields[11].Value = dtTemp2.Rows[i]["SQBZBZ"];
                    myFields[12].Value = dtTemp2.Rows[i]["FPHM"];
                    myFields[13].Value = dtTemp2.Rows[i]["FPSJ"];
                    myFields[14].Value = dtTemp2.Rows[i]["XSZSJ"];
                    myFields[15].Value = dtTemp2.Rows[i]["FPTP"];
                    myFields[16].Value = dtTemp2.Rows[i]["XSZTP"];
                    myFields[17].Value = dtTemp2.Rows[i]["FPTP_PICTURE"];
                    myFields[18].Value = dtTemp2.Rows[i]["XSZTP_PICTURE"];
                    myFields[19].Value = dtTemp2.Rows[i]["CJDRXX_CXXH"];
                    myFields[20].Value = dtTemp2.Rows[i]["CJDRXX_DRZRL"];
                    myFields[21].Value = dtTemp2.Rows[i]["CJDRXX_DRZSCQY"];
                    myFields[22].Value = dtTemp2.Rows[i]["CJDRXX_DTSCQY"];
                    myFields[23].Value = dtTemp2.Rows[i]["CJDRXX_DTXH"];
                    myFields[24].Value = dtTemp2.Rows[i]["CJDRXX_XTJG"];
                    myFields[25].Value = dtTemp2.Rows[i]["CJDRXX_ZBNX"];
                    myFields[26].Value = dtTemp2.Rows[i]["CLSFYCJDR"];
                    myFields[27].Value = dtTemp2.Rows[i]["CLSFYQDDJ2"];
                    myFields[28].Value = dtTemp2.Rows[i]["CLSFYRLDC"];
                    myFields[29].Value = dtTemp2.Rows[i]["DCDTXX_SCQY"];
                    myFields[30].Value = dtTemp2.Rows[i]["DCDTXX_XH"];
                    myFields[31].Value = dtTemp2.Rows[i]["DCZXX_SCQY"];
                    myFields[32].Value = dtTemp2.Rows[i]["DCZXX_XH"];
                    myFields[33].Value = dtTemp2.Rows[i]["DCZXX_XTJG"];
                    myFields[34].Value = dtTemp2.Rows[i]["DCZXX_ZBNX"];
                    myFields[35].Value = dtTemp2.Rows[i]["DCZXX_ZRL"];
                    myFields[36].Value = dtTemp2.Rows[i]["QDDJXX_EDGL_1"];
                    myFields[37].Value = dtTemp2.Rows[i]["QDDJXX_EDGL_2"];
                    myFields[38].Value = dtTemp2.Rows[i]["QDDJXX_SCQY_1"];
                    myFields[39].Value = dtTemp2.Rows[i]["QDDJXX_SCQY_2"];
                    myFields[40].Value = dtTemp2.Rows[i]["QDDJXX_XH_1"];
                    myFields[41].Value = dtTemp2.Rows[i]["QDDJXX_XH_2"];
                    myFields[42].Value = dtTemp2.Rows[i]["QDDJXX_XTJG_1"];
                    myFields[43].Value = dtTemp2.Rows[i]["QDDJXX_XTJG_2"];
                    myFields[44].Value = dtTemp2.Rows[i]["RLDCXX_EDGL"];
                    myFields[45].Value = dtTemp2.Rows[i]["RLDCXX_GMJG"];
                    myFields[46].Value = dtTemp2.Rows[i]["RLDCXX_SCQY"];
                    myFields[47].Value = dtTemp2.Rows[i]["RLDCXX_XH"];
                    myFields[48].Value = dtTemp2.Rows[i]["RLDCXX_ZBNX"];
                    myFields[49].Value = dtTemp2.Rows[i]["JZNF"];
                    myFields[50].Value = sel3.Rows[0]["BGLHDL"];
                    myFields[51].Value = sel3.Rows[0]["CLCMYCDNGXSLC"];
                    myFields[52].Value = sel3.Rows[0]["CLYCCMDSXSJ"];
                    myFields[53].Value = dtTemp2.Rows[i]["CLYXDW"];
                    myFields[54].Value = sel3.Rows[0]["JKPDXXDW"];
                    myFields[55].Value = sel3.Rows[0]["LJCDL"];
                    myFields[56].Value = sel3.Rows[0]["LJJQL"];
                    myFields[57].Value = sel3.Rows[0]["LJJQL_G"];
                    myFields[58].Value = sel3.Rows[0]["LJJQL_L"];
                    myFields[59].Value = sel3.Rows[0]["LJJYL"];
                    myFields[60].Value = sel3.Rows[0]["LJXSLC"];
                    myFields[61].Value = sel3.Rows[0]["PJDRXYSJ"];
                    myFields[62].Value = sel3.Rows[0]["SFAZJKZZ"];
                    myFields[63].Value = sel3.Rows[0]["YJXSLC"];
                    myFields[64].Value = sel3.Rows[0]["ZDCDGL"];
                    rs.Update();
                }
                catch (Exception ex)
                {
                    msg += String.Format("车辆识别代码(VIN)：{0}的异常信息{1}，操作异常{2}", vin, ex.Message, Environment.NewLine);
                    continue;
                }
            }
            rs.Close();
            db.Close();
            return msg;
        }

    }
}
