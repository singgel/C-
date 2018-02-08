using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.LogUtils;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Catarc.Adc.NewEnergyApproveSys.Common;
using Catarc.Adc.NewEnergyApproveSys.OfficeHelper;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using NPOI.SS.UserModel;
using System.Windows.Forms;
using NPOI.XSSF.UserModel;
using System.Collections;

namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils
{
    public class ImportDataUtils
    {
        private Microsoft.Office.Interop.Excel.Application excelApp = null;

        private enum APP_STATUS
        {
            未审批 = 0,
            一审驳回 = 10,
            一审通过 = 11,
            A通过 = 13,
            A驳回 = 14,
            二审驳回 = 20,
            二审通过 = 21,
            三审驳回 = 30,
            三审通过 = 31
        };

        /// <summary>
        /// 导入新模板数据
        /// </summary>
        /// <param name="floderPath">模板所在路径</param>
        /// <returns></returns>
        /// 
        public string ImportNewTemplate(string floderPath, string area, string batch, ref string CLSCQY)
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
            if (TheFolder.GetFiles("附件4*.xlsx").Length < 1)
            {
                msg += "当前路径下缺少附件4*.xlsx类型文件" + Environment.NewLine;
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
            //Step3：读取模板，按照固定的行列读取数据
            var dtTemp2 = this.readExcelTemplate2(TheFolder.GetFiles("附件2*.xlsx")[0].FullName, area, batch, ref CLSCQY);
            var dtTemp3 = this.readExcelTemplate3(TheFolder.GetFiles("附件3*.xlsx")[0].FullName);
            var dtTemp4 = this.readExcelTemplate4(TheFolder.GetFiles("附件4*.xlsx")[0].FullName);
            stopWatch.Stop();
            TimeSpan ts2 = stopWatch.Elapsed - ts;
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
            //msg += this.UniqueKeyDataTable(ref dtTemp2, ref dtTemp3);
            //stopWatch.Stop();
            //TimeSpan ts4 = stopWatch.Elapsed - ts3;
            //LogManager.Log("TimeSpend", "ImportTime", String.Format("Step5：检验VIN、车辆牌照是否在系统中已存在耗时：{0}时{1}分{2}秒", ts4.Hours, ts4.Minutes, ts4.Seconds));
            //stopWatch.Start();
            //if (dtTemp2.Rows.Count == 0)
            //{
            //    return msg;
            //}
            //Step6：插入数据
            string sqlMsg = this.insertDataTable(dtTemp2, dtTemp3);
            sqlMsg += this.insertDataTable4(dtTemp4);
            if (!string.IsNullOrEmpty(sqlMsg))
            {
                msg += sqlMsg;
            }
            stopWatch.Stop();
            TimeSpan ts6 = stopWatch.Elapsed - ts3;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step6：插入数据耗时==========：{0}时{1}分{2}秒", ts6.Hours, ts6.Minutes, ts6.Seconds));
            //Step2：统一图片后缀为.jpg，拷贝缩略图
            this.copyPictureTemplate6(TheFolder, CLSCQY);
            this.copyPictureTemplate7(TheFolder, CLSCQY);
            stopWatch.Stop();
            TimeSpan ts5 = stopWatch.Elapsed - ts6;
            LogManager.Log("TimeSpend", "ImportTime", String.Format("Step2：统一图片后缀为.jpg，拷贝缩略图耗时：{0}时{1}分{2}秒", ts5.Hours, ts5.Minutes, ts5.Seconds));
            stopWatch.Start();
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
            using (var con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                for (int i = 0; i < selectedDT.Rows.Count; i++)
                {
                    using (var tra = con.BeginTransaction())
                    {
                        string vin = selectedDT.Rows[i]["VIN"].ToString().Trim();
                        string guid = selectedDT.Rows[i]["GUID"].ToString().Trim();
                        try
                        {
                            if (OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("delete from DB_INFOMATION where GUID='{0}'", guid)) > 0)
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
                    string fptp = String.Format("{0}\\{1}", ApplicationFolder.billImage, selectedDT.Rows[i]["FPTP"].ToString().Trim());
                    if (File.Exists(fptp))
                    {
                        File.Delete(fptp);
                    }
                    string xsztp = String.Format("{0}\\{1}", ApplicationFolder.driveImage, selectedDT.Rows[i]["XSZTP"].ToString().Trim());
                    if (File.Exists(xsztp))
                    {
                        File.Delete(xsztp);
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// 恢复选中数据
        /// </summary>
        /// <param name="obj">要恢复的控件信息</param>
        /// <returns></returns>
        public string recoverDataInfo(DataTable selectedDT)
        {
            string msg = string.Empty;
            using (var con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    var guidList = selectedDT.AsEnumerable().Select(d => d.Field<string>("GUID")).ToList();
                    while (guidList.Count > 0)
                    {
                        var guidArrSkip = guidList.Take(1000);
                        stringBuilder.AppendFormat(" or GUID in('{0}')", string.Join("','", guidArrSkip));
                        if (guidList.Count > 999)
                        {
                            guidList.RemoveRange(0, 999);
                        }
                        else
                        {
                            guidList.RemoveRange(0, guidList.Count);
                        }
                    }
                    var guidStr = string.Format("and ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                    try
                    {
                        int num1 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_1_A=null,APP_NAME_1_B=null,APP_TIME_1_A=null,APP_TIME_1_B=null,APP_RESULT_1_A=null,APP_RESULT_1_B=null,APP_NAME_2=null,APP_TIME_2=null,APP_RESULT_2=null,APP_NAME_3=null,APP_TIME_3=null,APP_RESULT_3=null,APP_MONEY=null,APP_STATUS=0 where 1=1 {0}", guidStr));

                        msg += String.Format("{0}条数据成功从本系统中恢复，操作成功！{1}", num1, Environment.NewLine);
                        msg += String.Format("{0}条数据未能从本系统中恢复，操作失败！{1}", selectedDT.Rows.Count - num1, Environment.NewLine);
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        msg += String.Format("数据恢复的异常信息，操作异常！{0}", ex.Message);
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
        private string copyPictureTemplate6(DirectoryInfo theFolder, string CLSCQY)
        {
            string msg = string.Empty;
            string ftpFolder = OracleHelper.GetSingle(OracleHelper.conn, "select DIC_NAME from SYS_DIC where DIC_TYPE='ftp路径'").ToString();
            string billImage = Path.Combine(ftpFolder, "IMAGEBill", CLSCQY);
            if (!Directory.Exists(billImage))
            {
                Directory.CreateDirectory(billImage);
            }
            var billFolder = new DirectoryInfo(theFolder.GetDirectories("附件6*")[0].FullName);
            Array.ForEach(billFolder.GetFiles(), NextFile =>
            {
                try
                {
                    System.IO.File.Copy(NextFile.FullName, String.Format("{0}\\{1}.jpg", billImage, Path.GetFileNameWithoutExtension(NextFile.FullName)), false);
                }
                catch (Exception ex)
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
        private string copyPictureTemplate7(DirectoryInfo theFolder, string CLSCQY)
        {
            string msg = string.Empty;
            string ftpFolder = OracleHelper.GetSingle(OracleHelper.conn, "select DIC_NAME from SYS_DIC where DIC_TYPE='ftp路径'").ToString();
            string driveImage = Path.Combine(ftpFolder, "IMAGEDrive", CLSCQY);
            if (!Directory.Exists(driveImage))
            {
                Directory.CreateDirectory(driveImage);
            }
            var driveFolder = new DirectoryInfo(theFolder.GetDirectories("附件7*")[0].FullName);
            Array.ForEach(driveFolder.GetFiles(), NextFile =>
            {
                try
                {
                    System.IO.File.Copy(NextFile.FullName, String.Format("{0}\\{1}.jpg", driveImage, Path.GetFileNameWithoutExtension(NextFile.FullName)), false);
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
        private DataTable readExcelTemplate2(string filePath, string area, string batch, ref string CLSCQY)
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
            dtTemp2.Columns.Add("FPSJ", Type.GetType("System.DateTime"));
            dtTemp2.Columns.Add("XSZSJ", Type.GetType("System.DateTime"));
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
            dtTemp2.Columns.Add("CLSCQY", Type.GetType("System.String"));
            dtTemp2.Columns.Add("DQ", Type.GetType("System.String"));
            dtTemp2.Columns.Add("PC", Type.GetType("System.String"));
            var entNameArr = dt2.Rows[1][0].ToString().Trim().Split(':');
            var entName = entNameArr.Length > 1 ? entNameArr[1] : string.Empty;
            CLSCQY = entName;
            int batchInt = Convert.ToInt32(batch);
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
                dtTemp2.Rows[i - 4][13] = Convert.ToDateTime(String.Format("{0}/{1}/{2}", dt2.Rows[i][14].ToString().Trim(), dt2.Rows[i][15].ToString().Trim(), dt2.Rows[i][16].ToString().Trim()));
                dtTemp2.Rows[i - 4][14] = Convert.ToDateTime(String.Format("{0}/{1}/{2}", dt2.Rows[i][17].ToString().Trim(), dt2.Rows[i][18].ToString().Trim(), dt2.Rows[i][19].ToString().Trim()));
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
                dtTemp2.Rows[i - 4][47] = String.Format("车辆发票VIN-{0}.jpg", vin);// File.Exists(Path.Combine(ApplicationFolder.billImage, String.Format("车辆发票VIN-{0}.jpg", vin))) ? String.Format("车辆发票VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][48] = String.Format("车辆发票VIN-{0}.jpg", vin);// File.Exists(Path.Combine(ApplicationFolder.billImage, String.Format("车辆发票VIN-{0}.jpg", vin))) ? String.Format("车辆发票VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][49] = String.Format("行驶证VIN-{0}.jpg", vin);// File.Exists(Path.Combine(ApplicationFolder.driveImage, String.Format("行驶证VIN-{0}.jpg", vin))) ? String.Format("行驶证VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][50] = String.Format("行驶证VIN-{0}.jpg", vin);// File.Exists(Path.Combine(ApplicationFolder.driveImage, String.Format("行驶证VIN-{0}.jpg", vin))) ? String.Format("行驶证VIN-{0}.jpg", vin) : string.Empty;
                dtTemp2.Rows[i - 4][51] = entName;
                dtTemp2.Rows[i - 4][52] = area;
                dtTemp2.Rows[i - 4][53] = batchInt;
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
        /// 读取附件4
        /// </summary>
        /// <param name="filePath">附件路径</param>
        /// <returns></returns>
        private DataTable readExcelTemplate4(string filePath)
        {
            var dt4 = ImportExcel.ReadExcelToDataSet(filePath, "附件4").Tables[0];
            var dtTemp4 = new DataTable();
            dtTemp4.Columns.Add("ENTNAME", Type.GetType("System.String"));
            dtTemp4.Columns.Add("PTYPE", Type.GetType("System.String"));
            dtTemp4.Columns.Add("NAME", Type.GetType("System.String"));
            dtTemp4.Columns.Add("DEPT", Type.GetType("System.String"));
            dtTemp4.Columns.Add("JOB", Type.GetType("System.String"));
            dtTemp4.Columns.Add("TEL", Type.GetType("System.String"));
            dtTemp4.Columns.Add("PHONE", Type.GetType("System.String"));
            dtTemp4.Columns.Add("EMAIL", Type.GetType("System.String"));
            var entNameArr = dt4.Rows[1][0].ToString().Trim().Split(':');
            var entName = entNameArr.Length > 1 ? entNameArr[1] : string.Empty;
            for (int i = 3; i < dt4.Rows.Count; i++)
            {
                if (string.IsNullOrEmpty(dt4.Rows[i][0].ToString().Trim())) break;
                dtTemp4.Rows.Add();
                dtTemp4.Rows[i - 3][0] = entName;
                dtTemp4.Rows[i - 3][1] = dt4.Rows[i][0].ToString().Trim();
                dtTemp4.Rows[i - 3][2] = dt4.Rows[i][1].ToString().Trim();
                dtTemp4.Rows[i - 3][3] = dt4.Rows[i][2].ToString().Trim();
                dtTemp4.Rows[i - 3][4] = dt4.Rows[i][3].ToString().Trim();
                dtTemp4.Rows[i - 3][5] = dt4.Rows[i][4].ToString().Trim();
                dtTemp4.Rows[i - 3][6] = dt4.Rows[i][5].ToString().Trim();
                dtTemp4.Rows[i - 3][7] = dt4.Rows[i][6].ToString().Trim();
            }
            return dtTemp4;
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
            //dtTemp2 = dtTemp2.AsEnumerable().Cast<DataRow>().GroupBy(p => p.Field<string>("VIN") + p.Field<string>("CLPZ")).Select(p => p.FirstOrDefault()).CopyToDataTable();
            //dtTemp2 = dtTemp2.AsEnumerable().Cast<DataRow>().GroupBy(p => p.Field<string>("CLPZ")).Select(p => p.FirstOrDefault()).CopyToDataTable();
            //dtTemp3 = dtTemp3.AsEnumerable().Cast<DataRow>().GroupBy(p => p.Field<string>("CLPZ")).Select(p => p.FirstOrDefault()).CopyToDataTable();
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
            var dtXT = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from DB_INFOMATION").Tables[0];
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
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dtDB = new DataTable();
                        dtDB.Columns.Add("VIN", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLXZ", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLZL", Type.GetType("System.String"));
                        dtDB.Columns.Add("GCSF", Type.GetType("System.String"));
                        dtDB.Columns.Add("GCCS", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLYT", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLXH", Type.GetType("System.String"));
                        dtDB.Columns.Add("GGPC", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLPZ", Type.GetType("System.String"));
                        dtDB.Columns.Add("EKGZ", Type.GetType("System.String"));
                        dtDB.Columns.Add("GMJG", Type.GetType("System.String"));
                        dtDB.Columns.Add("SQBZBZ", Type.GetType("System.String"));
                        dtDB.Columns.Add("FPHM", Type.GetType("System.String"));
                        dtDB.Columns.Add("FPSJ", Type.GetType("System.DateTime"));
                        dtDB.Columns.Add("XSZSJ", Type.GetType("System.DateTime"));
                        dtDB.Columns.Add("FPTP", Type.GetType("System.String"));
                        dtDB.Columns.Add("XSZTP", Type.GetType("System.String"));
                        dtDB.Columns.Add("FPTP_PICTURE", Type.GetType("System.String"));
                        dtDB.Columns.Add("XSZTP_PICTURE", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_CXXH", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_DRZRL", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_DRZSCQY", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_DTSCQY", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_DTXH", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_XTJG", Type.GetType("System.String"));
                        dtDB.Columns.Add("CJDRXX_ZBNX", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLSFYCJDR", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLSFYQDDJ2", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLSFYRLDC", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCDTXX_SCQY", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCDTXX_XH", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCZXX_SCQY", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCZXX_XH", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCZXX_XTJG", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCZXX_ZBNX", Type.GetType("System.String"));
                        dtDB.Columns.Add("DCZXX_ZRL", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_EDGL_1", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_EDGL_2", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_SCQY_1", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_SCQY_2", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_XH_1", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_XH_2", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_XTJG_1", Type.GetType("System.String"));
                        dtDB.Columns.Add("QDDJXX_XTJG_2", Type.GetType("System.String"));
                        dtDB.Columns.Add("RLDCXX_EDGL", Type.GetType("System.String"));
                        dtDB.Columns.Add("RLDCXX_GMJG", Type.GetType("System.String"));
                        dtDB.Columns.Add("RLDCXX_SCQY", Type.GetType("System.String"));
                        dtDB.Columns.Add("RLDCXX_XH", Type.GetType("System.String"));
                        dtDB.Columns.Add("RLDCXX_ZBNX", Type.GetType("System.String"));
                        dtDB.Columns.Add("JZNF", Type.GetType("System.String"));
                        dtDB.Columns.Add("BGLHDL", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLCMYCDNGXSLC", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLYCCMDSXSJ", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLYXDW", Type.GetType("System.String"));
                        dtDB.Columns.Add("JKPDXXDW", Type.GetType("System.String"));
                        dtDB.Columns.Add("LJCDL", Type.GetType("System.String"));
                        dtDB.Columns.Add("LJJQL", Type.GetType("System.String"));
                        dtDB.Columns.Add("LJJQL_G", Type.GetType("System.String"));
                        dtDB.Columns.Add("LJJQL_L", Type.GetType("System.String"));
                        dtDB.Columns.Add("LJJYL", Type.GetType("System.String"));
                        dtDB.Columns.Add("LJXSLC", Type.GetType("System.String"));
                        dtDB.Columns.Add("PJDRXYSJ", Type.GetType("System.String"));
                        dtDB.Columns.Add("SFAZJKZZ", Type.GetType("System.String"));
                        dtDB.Columns.Add("YJXSLC", Type.GetType("System.String"));
                        dtDB.Columns.Add("ZDCDGL", Type.GetType("System.String"));
                        dtDB.Columns.Add("CLSCQY", Type.GetType("System.String"));
                        dtDB.Columns.Add("DQ", Type.GetType("System.String"));
                        dtDB.Columns.Add("PC", Type.GetType("System.String"));
                        dtDB.Columns.Add("GUID", Type.GetType("System.String"));
                        OracleCommandBuilder ocb = new OracleCommandBuilder(da);
                        da.SelectCommand.CommandText = "SELECT VIN,CLXZ,CLZL,GCSF,GCCS,CLYT,CLXH,GGPC,CLPZ,EKGZ,GMJG,SQBZBZ,FPHM,FPSJ,XSZSJ,FPTP,XSZTP,FPTP_PICTURE,XSZTP_PICTURE,CJDRXX_CXXH,CJDRXX_DRZRL,CJDRXX_DRZSCQY,CJDRXX_DTSCQY,CJDRXX_DTXH,CJDRXX_XTJG,CJDRXX_ZBNX,CLSFYCJDR,CLSFYQDDJ2,CLSFYRLDC,DCDTXX_SCQY,DCDTXX_XH,DCZXX_SCQY,DCZXX_XH,DCZXX_XTJG,DCZXX_ZBNX,DCZXX_ZRL,QDDJXX_EDGL_1,QDDJXX_EDGL_2,QDDJXX_SCQY_1,QDDJXX_SCQY_2,QDDJXX_XH_1,QDDJXX_XH_2,QDDJXX_XTJG_1,QDDJXX_XTJG_2,RLDCXX_EDGL,RLDCXX_GMJG,RLDCXX_SCQY,RLDCXX_XH,RLDCXX_ZBNX,JZNF,BGLHDL,CLCMYCDNGXSLC,CLYCCMDSXSJ,CLYXDW,JKPDXXDW,LJCDL,LJJQL,LJJQL_G,LJJQL_L,LJJYL,LJXSLC,PJDRXYSJ,SFAZJKZZ,YJXSLC,ZDCDGL,CLSCQY,DQ,PC,GUID FROM DB_INFOMATION";
                        da.InsertCommand = ocb.GetInsertCommand();
                        for (int i = 0; i < dtTemp2.Rows.Count; i++)
                        {
                            DataRow newRow = dtDB.NewRow();
                            string clpz = dtTemp2.Rows[i]["CLPZ"].ToString().Trim();
                            var sel3 = (from t3 in dtTemp3.AsEnumerable()
                                        where t3.Field<string>("CLPZ").Equals(clpz)
                                        select t3).CopyToDataTable();
                            newRow["VIN"] = dtTemp2.Rows[i]["VIN"];
                            newRow["CLXZ"] = dtTemp2.Rows[i]["CLXZ"];
                            newRow["CLZL"] = dtTemp2.Rows[i]["CLZL"];
                            newRow["GCSF"] = dtTemp2.Rows[i]["GCSF"];
                            newRow["GCCS"] = dtTemp2.Rows[i]["GCCS"];
                            newRow["CLYT"] = dtTemp2.Rows[i]["CLYT"];
                            newRow["CLXH"] = dtTemp2.Rows[i]["CLXH"];
                            newRow["GGPC"] = dtTemp2.Rows[i]["GGPC"];
                            newRow["CLPZ"] = dtTemp2.Rows[i]["CLPZ"];
                            newRow["EKGZ"] = dtTemp2.Rows[i]["EKGZ"];
                            newRow["GMJG"] = dtTemp2.Rows[i]["GMJG"];
                            newRow["SQBZBZ"] = dtTemp2.Rows[i]["SQBZBZ"];
                            newRow["FPHM"] = dtTemp2.Rows[i]["FPHM"];
                            newRow["FPSJ"] = dtTemp2.Rows[i]["FPSJ"];
                            newRow["XSZSJ"] = dtTemp2.Rows[i]["XSZSJ"];
                            newRow["FPTP"] = dtTemp2.Rows[i]["FPTP"];
                            newRow["XSZTP"] = dtTemp2.Rows[i]["XSZTP"];
                            newRow["FPTP_PICTURE"] = dtTemp2.Rows[i]["FPTP_PICTURE"];
                            newRow["XSZTP_PICTURE"] = dtTemp2.Rows[i]["XSZTP_PICTURE"];
                            newRow["CJDRXX_CXXH"] = dtTemp2.Rows[i]["CJDRXX_CXXH"];
                            newRow["CJDRXX_DRZRL"] = dtTemp2.Rows[i]["CJDRXX_DRZRL"];
                            newRow["CJDRXX_DRZSCQY"] = dtTemp2.Rows[i]["CJDRXX_DRZSCQY"];
                            newRow["CJDRXX_DTSCQY"] = dtTemp2.Rows[i]["CJDRXX_DTSCQY"];
                            newRow["CJDRXX_DTXH"] = dtTemp2.Rows[i]["CJDRXX_DTXH"];
                            newRow["CJDRXX_XTJG"] = dtTemp2.Rows[i]["CJDRXX_XTJG"];
                            newRow["CJDRXX_ZBNX"] = dtTemp2.Rows[i]["CJDRXX_ZBNX"];
                            newRow["CLSFYCJDR"] = dtTemp2.Rows[i]["CLSFYCJDR"];
                            newRow["CLSFYQDDJ2"] = dtTemp2.Rows[i]["CLSFYQDDJ2"];
                            newRow["CLSFYRLDC"] = dtTemp2.Rows[i]["CLSFYRLDC"];
                            newRow["DCDTXX_SCQY"] = dtTemp2.Rows[i]["DCDTXX_SCQY"];
                            newRow["DCDTXX_XH"] = dtTemp2.Rows[i]["DCDTXX_XH"];
                            newRow["DCZXX_SCQY"] = dtTemp2.Rows[i]["DCZXX_SCQY"];
                            newRow["DCZXX_XH"] = dtTemp2.Rows[i]["DCZXX_XH"];
                            newRow["DCZXX_XTJG"] = dtTemp2.Rows[i]["DCZXX_XTJG"];
                            newRow["DCZXX_ZBNX"] = dtTemp2.Rows[i]["DCZXX_ZBNX"];
                            newRow["DCZXX_ZRL"] = dtTemp2.Rows[i]["DCZXX_ZRL"];
                            newRow["QDDJXX_EDGL_1"] = dtTemp2.Rows[i]["QDDJXX_EDGL_1"];
                            newRow["QDDJXX_EDGL_2"] = dtTemp2.Rows[i]["QDDJXX_EDGL_2"];
                            newRow["QDDJXX_SCQY_1"] = dtTemp2.Rows[i]["QDDJXX_SCQY_1"];
                            newRow["QDDJXX_SCQY_2"] = dtTemp2.Rows[i]["QDDJXX_SCQY_2"];
                            newRow["QDDJXX_XH_1"] = dtTemp2.Rows[i]["QDDJXX_XH_1"];
                            newRow["QDDJXX_XH_2"] = dtTemp2.Rows[i]["QDDJXX_XH_2"];
                            newRow["QDDJXX_XTJG_1"] = dtTemp2.Rows[i]["QDDJXX_XTJG_1"];
                            newRow["QDDJXX_XTJG_2"] = dtTemp2.Rows[i]["QDDJXX_XTJG_2"];
                            newRow["RLDCXX_EDGL"] = dtTemp2.Rows[i]["RLDCXX_EDGL"];
                            newRow["RLDCXX_GMJG"] = dtTemp2.Rows[i]["RLDCXX_GMJG"];
                            newRow["RLDCXX_SCQY"] = dtTemp2.Rows[i]["RLDCXX_SCQY"];
                            newRow["RLDCXX_XH"] = dtTemp2.Rows[i]["RLDCXX_XH"];
                            newRow["RLDCXX_ZBNX"] = dtTemp2.Rows[i]["RLDCXX_ZBNX"];
                            newRow["JZNF"] = dtTemp2.Rows[i]["JZNF"];
                            newRow["BGLHDL"] = sel3.Rows[0]["BGLHDL"];
                            newRow["CLCMYCDNGXSLC"] = sel3.Rows[0]["CLCMYCDNGXSLC"];
                            newRow["CLYCCMDSXSJ"] = sel3.Rows[0]["CLYCCMDSXSJ"];
                            newRow["CLYXDW"] = dtTemp2.Rows[i]["CLYXDW"];
                            newRow["JKPDXXDW"] = sel3.Rows[0]["JKPDXXDW"];
                            newRow["LJCDL"] = sel3.Rows[0]["LJCDL"];
                            newRow["LJJQL"] = sel3.Rows[0]["LJJQL"];
                            newRow["LJJQL_G"] = sel3.Rows[0]["LJJQL_G"];
                            newRow["LJJQL_L"] = sel3.Rows[0]["LJJQL_L"];
                            newRow["LJJYL"] = sel3.Rows[0]["LJJYL"];
                            newRow["LJXSLC"] = sel3.Rows[0]["LJXSLC"];
                            newRow["PJDRXYSJ"] = sel3.Rows[0]["PJDRXYSJ"];
                            newRow["SFAZJKZZ"] = sel3.Rows[0]["SFAZJKZZ"];
                            newRow["YJXSLC"] = sel3.Rows[0]["YJXSLC"];
                            newRow["ZDCDGL"] = sel3.Rows[0]["ZDCDGL"];
                            newRow["CLSCQY"] = dtTemp2.Rows[i]["CLSCQY"];
                            newRow["DQ"] = dtTemp2.Rows[i]["DQ"];
                            newRow["PC"] = dtTemp2.Rows[i]["PC"];
                            newRow["GUID"] = Guid.NewGuid();
                            dtDB.Rows.Add(newRow);
                        }
                        int count = da.Update(dtDB);
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// 录入附件信息
        /// </summary>
        private string insertDataTable4(DataTable dtTemp4)
        {
            string msg = string.Empty;
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        OracleCommandBuilder ocb = new OracleCommandBuilder(da);
                        da.SelectCommand.CommandText = "SELECT * FROM DB_CONTACTS";
                        da.InsertCommand = ocb.GetInsertCommand();
                        int count = da.Update(dtTemp4);
                    }
                }
                con.Open();
                using (OracleTransaction trans = con.BeginTransaction())
                {
                    OracleHelper.ExecuteNonQuery(trans, "delete from DB_CONTACTS a where (a.ENTNAME,a.PTYPE,a.NAME,a.DEPT,a.JOB,a.TEL,a.PHONE,a.EMAIL) in (select ENTNAME,PTYPE,NAME,DEPT,JOB,TEL,PHONE,EMAIL from DB_CONTACTS group by ENTNAME,PTYPE,NAME,DEPT,JOB,TEL,PHONE,EMAIL having count(*) > 1) and rowid not in (select min(rowid) from DB_CONTACTS group by ENTNAME,PTYPE,NAME,DEPT,JOB,TEL,PHONE,EMAIL having count(*)>1)");
                    trans.Commit();
                }
            }
            return msg;
        }

        /// <summary>
        /// 数据导出--详情导出
        /// </summary>
        public string dataToModelExcel(DataTable dtExportAll, string strSaveFile)
        {
            this.ClearMemory();
            string modelExlPath = Application.StartupPath + Settings.Default["ExcelXNYTemplate"];
            if (File.Exists(modelExlPath) == false)//模板不存在
            {
                return "模板不存在。";
            }

            IWorkbook wk = null;
            using (FileStream fs = File.Open(modelExlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
            DataTable dt = ieNOPI.ExcelToDataTable(modelExlPath, "表头", true);
            string msg = string.Empty;
            int pageNum = dtExportAll.Rows.Count / 20000;
            for (int i = 0; i < pageNum; i++)
            {
                var dtExport = GetPagedTable(dtExportAll, i + 1, 20000);
                msg += this.WriterExcel(wk, i, dtExport, dt);
            }
            int pageLeftNum = dtExportAll.Rows.Count % 20000;
            if (pageLeftNum > 0)
            {
                var dtExport = GetPagedTable(dtExportAll, pageNum + 1, pageLeftNum);
                msg += this.WriterExcel(wk, pageNum, dtExport, dt);
            }
            if (!string.IsNullOrEmpty(msg))
            {
                return msg;
            }
            //删除“表头”sheet
            wk.RemoveSheetAt(3);
            //重命名SHEET1
            string filename = System.IO.Path.GetFileNameWithoutExtension(strSaveFile);// 没有扩展名的文件名("\\");
            wk.SetSheetName(0, filename);

            wk.SetActiveSheet(0);//直接打开第一sheet
            this.ClearMemory();
            //转为字节数组
            MemoryStream stream = new MemoryStream();
            wk.Write(stream);
            var buf = stream.ToArray();
            //保存文件
            this.SaveFile(strSaveFile, buf);

            if (File.Exists(strSaveFile) == false)//生成失败
            {
                return "导出失败";
            }
            return "";

        }

        public string dataToModelExcel_XLS(DataTable dtExportAll, string strSaveFile)
        {
            this.ClearMemory();
            string modelExlPath = Application.StartupPath + Settings.Default["ExcelXNYTemplate"];
            if (File.Exists(modelExlPath) == false)//模板不存在
            {
                return "模板不存在。";
            }

            IWorkbook wk = null;
            using (FileStream fs = File.Open(modelExlPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
            DataTable dt = ieNOPI.ExcelToDataTable(modelExlPath, "表头", true);
            string msg = string.Empty;
            msg += this.WriterExcel_XLS(strSaveFile, dtExportAll, dt);
            if (!string.IsNullOrEmpty(msg))
            {
                return msg;
            }
            return "";
        }
        
        
        
        
        /// <summary>
        /// 内存回收
        /// </summary>
        private void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        /// <summary>
        /// DataTable分页
        /// </summary>
        private DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)//PageIndex表示第几页，PageSize表示每页的记录数
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }
        /// <summary>
        /// excel赋值
        /// </summary>
        private string WriterExcel(IWorkbook hssfworkbookDown, int sheetIndex, DataTable DT, DataTable dtHeader)
        {
            try
            {
                //字体
                IFont fontS9 = hssfworkbookDown.CreateFont();
                fontS9.FontName = "宋体";
                fontS9.FontHeightInPoints = 11;
                fontS9.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Normal;
                //表格
                ICellStyle style = hssfworkbookDown.CreateCellStyle();
                style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                style.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                style.WrapText = true;
                style.SetFont(fontS9);

                ISheet sheet = hssfworkbookDown.GetSheetAt(sheetIndex);
                ICell cell = null;

                for (int j = 0; j < DT.Rows.Count; j++)
                {
                    IRow dataRow = sheet.CreateRow(j + 2);
                    for (int i = 0; i < dtHeader.Rows.Count; i++)
                    {
                        DataRow dr = dtHeader.Rows[i];
                        cell = dataRow.CreateCell(i);

                        string strAppStatus = DT.Rows[j]["APP_STATUS"].ToString().Trim();
                        string strSHZT = Enum.GetName(typeof(APP_STATUS), Convert.ToInt32(strAppStatus));

                        if (dr[0].ToString().Trim().Equals("序号"))
                        {
                            cell.SetCellValue(j + 1);
                        }
                        else if (dr[0].ToString().Trim().Equals("审核状态"))
                        {
                            cell.SetCellValue(strSHZT);
                        }
                        else if (dr[0].ToString().Trim().Equals("驳回说明"))
                        {
                            string BHSM = string.Empty;
                            
                            if (strAppStatus == "10")
                            {
                                BHSM = DT.Rows[j]["APP_RESULT_1_A"] + DT.Rows[j]["APP_RESULT_1_B"].ToString();
                            }
                            else if (strAppStatus == "20")
                            {
                                BHSM = DT.Rows[j]["APP_RESULT_2"].ToString();
                            }
                            else if (strAppStatus == "30")
                            {
                                BHSM = DT.Rows[j]["APP_RESULT_3"].ToString();
                            }
                            cell.SetCellValue(BHSM);
                        }
                        else if (dr[0].ToString().Trim().Equals("一审人员"))
                        {
                            string YSRY = string.Empty;
                            if (!string.IsNullOrEmpty(DT.Rows[j]["APP_NAME_1_A"].ToString()))
                            {
                                if (!string.IsNullOrEmpty(DT.Rows[j]["APP_NAME_1_B"].ToString()))
                                    YSRY = String.Format("{0}+{1}", DT.Rows[j]["APP_NAME_1_A"], DT.Rows[j]["APP_NAME_1_B"]);
                                else
                                    YSRY = DT.Rows[j]["APP_NAME_1_A"].ToString();
                            }
                            cell.SetCellValue(YSRY);
                        }
                        else
                        {
                            string colName = dr[1].ToString();
                            Type type = DT.Rows[j][colName].GetType();
                            if (type == typeof(int) || type == typeof(float) || type == typeof(Decimal) || type == typeof(Double))//值类型
                            {
                                dataRow.Cells[i].SetCellValue(Double.Parse(DT.Rows[j][colName].ToString()));
                            }
                            else if (type == typeof(DateTime))//类型
                            {
                                dataRow.Cells[i].SetCellValue(DateTime.Parse(DT.Rows[j][colName].ToString()).ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                dataRow.Cells[i].SetCellValue(DT.Rows[j][colName].ToString());
                            }
                        }
                        cell.CellStyle = style;//添加样式
                    }
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
        /// excel赋值
        /// </summary>
        /// <param name="saveName">保存路径</param>
        /// <param name="DT">待导出的数据</param>
        /// <param name="dtHeader">数据表头对应的列名</param>
        /// <returns></returns>
        private string WriterExcel_XLS(string saveName, DataTable DT, DataTable dtHeader)
        {
            string msg = null;
            
            excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelBook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet excelSheet = (Microsoft.Office.Interop.Excel.Worksheet)excelBook.ActiveSheet;
            excelApp.Visible = false;

            try
            {
                int rowCount = DT.Rows.Count;//行数
                int colCount = dtHeader.Rows.Count;//列数
                
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
                                    missing, missing, missing, missing);//添加一个sheet  
                        excelSheet.Name = String.Format(@"详情导出-{0}",  sc); //String.Format(@"{0}{1}", dt.TableName, sc); 

                        object[,] datas = new object[pageRows + 1, colCount];

                        for (int i = 0; i < colCount; i++) //写入字段   
                        {
                            datas[0, i] = dtHeader.Rows[i][0].ToString();//dictHeader[dt.Columns[i].ColumnName];//表头信息
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
                        //DT转化成符合导出模式的表格
                        for (int j = init; j < result; j++)//行数 从第几行到第几行
                        {
                            index = index + 1;
                            for (int i = 0; i < colCount; i++)
                            {
                                    DataRow dr = dtHeader.Rows[i];
                                
                                    string strAppStatus = DT.Rows[j]["APP_STATUS"].ToString().Trim();
                                    string strSHZT = Enum.GetName(typeof(APP_STATUS), Convert.ToInt32(strAppStatus));

                                    if (dr[0].ToString().Trim().Equals("序号"))
                                    {
                                        datas[index, i]=j+1;
                                    }
                                    else if (dr[0].ToString().Trim().Equals("审核状态"))
                                    {
                                        datas[index, i]=strSHZT;
                                    }
                                    else if (dr[0].ToString().Trim().Equals("驳回说明"))
                                    {
                                        string BHSM = string.Empty;

                                        if (strAppStatus == "10")
                                        {
                                            BHSM = DT.Rows[j]["APP_RESULT_1_A"] + DT.Rows[j]["APP_RESULT_1_B"].ToString();
                                        }
                                        else if (strAppStatus == "20")
                                        {
                                            BHSM = DT.Rows[j]["APP_RESULT_2"].ToString();
                                        }
                                        else if (strAppStatus == "30")
                                        {
                                            BHSM = DT.Rows[j]["APP_RESULT_3"].ToString();
                                        }
                                        datas[index, i]=BHSM;
                                    }
                                    else if (dr[0].ToString().Trim().Equals("一审人员"))
                                    {
                                        string YSRY = string.Empty;
                                        if (!string.IsNullOrEmpty(DT.Rows[j]["APP_NAME_1_A"].ToString()))
                                        {
                                            if (!string.IsNullOrEmpty(DT.Rows[j]["APP_NAME_1_B"].ToString()))
                                                YSRY = String.Format("{0}+{1}", DT.Rows[j]["APP_NAME_1_A"], DT.Rows[j]["APP_NAME_1_B"]);
                                            else
                                                YSRY = DT.Rows[j]["APP_NAME_1_A"].ToString();
                                        }
                                        datas[index, i]=YSRY;
                                    }
                                    else
                                    {
                                        string colName = dr[1].ToString();
                                        Type type = DT.Rows[j][colName].GetType();
                                        if (type == typeof(int) || type == typeof(float) || type == typeof(Decimal) || type == typeof(Double))//值类型
                                        {
                                            datas[index, i]=Double.Parse(DT.Rows[j][colName].ToString());
                                        }
                                        else if (type == typeof(DateTime))//类型
                                        {
                                            datas[index, i] = DateTime.Parse(DT.Rows[j][colName].ToString()).ToString("yyyy-MM-dd");
                                        }
                                        else
                                        {
                                           datas[index, i]=DT.Rows[j][colName].ToString();
                                        }
                                    }
                            }

                        }
                        excelSheet.Range["A1:A1"].Value = "详情数据";
                        excelSheet.Range["A1:K1"].Merge(0);//合并单元格
                        excelSheet.Range["A1:K1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;//水平居中  
                        excelSheet.Range["A1:K1"].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;//垂直居中  

                        Microsoft.Office.Interop.Excel.Range fchR = excelSheet.Range[excelSheet.Cells[2, 1], excelSheet.Cells[index + 2, colCount]];
                        fchR.NumberFormat = "@";
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
                        datas[0, i] = dtHeader.Rows[i][0].ToString();
                    }
                    int index = 1;
                    for (int j = 0; j < rowCount; j++)
                    {
                        index = j + 1;
                        for (int i = 0; i< colCount; i++)
                        {
                            DataRow dr = dtHeader.Rows[i];

                            string strAppStatus = DT.Rows[j]["APP_STATUS"].ToString().Trim();
                            string strSHZT = Enum.GetName(typeof(APP_STATUS), Convert.ToInt32(strAppStatus));

                            if (dr[0].ToString().Trim().Equals("序号"))
                            {
                                datas[index, i] = j + 1;
                            }
                            else if (dr[0].ToString().Trim().Equals("审核状态"))
                            {
                                datas[index, i] = strSHZT;
                            }
                            else if (dr[0].ToString().Trim().Equals("驳回说明"))
                            {
                                string BHSM = string.Empty;

                                if (strAppStatus == "10")
                                {
                                    BHSM = DT.Rows[j]["APP_RESULT_1_A"] + DT.Rows[j]["APP_RESULT_1_B"].ToString();
                                }
                                else if (strAppStatus == "20")
                                {
                                    BHSM = DT.Rows[j]["APP_RESULT_2"].ToString();
                                }
                                else if (strAppStatus == "30")
                                {
                                    BHSM = DT.Rows[j]["APP_RESULT_3"].ToString();
                                }
                                datas[index, i] = BHSM;
                            }
                            else if (dr[0].ToString().Trim().Equals("一审人员"))
                            {
                                string YSRY = string.Empty;
                                if (!string.IsNullOrEmpty(DT.Rows[j]["APP_NAME_1_A"].ToString()))
                                {
                                    if (!string.IsNullOrEmpty(DT.Rows[j]["APP_NAME_1_B"].ToString()))
                                        YSRY = String.Format("{0}+{1}", DT.Rows[j]["APP_NAME_1_A"], DT.Rows[j]["APP_NAME_1_B"]);
                                    else
                                        YSRY = DT.Rows[j]["APP_NAME_1_A"].ToString();
                                }
                                datas[index, i] = YSRY;
                            }
                            else
                            {
                                string colName = dr[1].ToString();
                                Type type = DT.Rows[j][colName].GetType();
                                if (type == typeof(int) || type == typeof(float) || type == typeof(Decimal) || type == typeof(Double))//值类型
                                {
                                    datas[index, i] = Double.Parse(DT.Rows[j][colName].ToString());
                                }
                                else if (type == typeof(DateTime))//类型
                                {
                                    datas[index, i] = DateTime.Parse(DT.Rows[j][colName].ToString()).ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    datas[index, i] = DT.Rows[j][colName].ToString();
                                }
                            }
                        }
                    }
                    excelSheet.Range["A1:A1"].Value = "详情数据";
                    excelSheet.Range["A1:K1"].Merge(0);//合并单元格
                    excelSheet.Range["A1:K1"].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;//水平居中  
                    excelSheet.Range["A1:K1"].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;//垂直居中

                    Microsoft.Office.Interop.Excel.Range range = excelSheet.Range[excelSheet.Cells[2, 1], excelSheet.Cells[rowCount + 2, colCount]];
                    range.NumberFormat = "@";
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
                msg=ex.Message;
                return msg;
            }
            finally
            {
                this.EndReport();
            }
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

        /// <summary>
        /// 保存文件方法
        /// </summary>
        private void SaveFile(string strSaveFile, byte[] buf)
        {
            using (FileStream fs = new FileStream(strSaveFile, FileMode.Create, FileAccess.Write))
            {
                fs.Write(buf, 0, buf.Length);
                fs.Flush();
            }
        }

        public string NewExport(DataTable dtAll, string filepath)
        {
            string msg = string.Empty;
            Microsoft.Office.Interop.Excel.Application appexcel = new Microsoft.Office.Interop.Excel.Application();
            System.Reflection.Missing miss = System.Reflection.Missing.Value;
            appexcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbookdata = appexcel.Workbooks.Open(Application.StartupPath + Settings.Default["ExcelXNYTemplate"], Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet worksheetdata = workbookdata.Sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

            //设置对象不可见  
            appexcel.Visible = false;
            appexcel.DisplayAlerts = false;
            try
            {
                for (int i = 0; i < 1; i++)
                {
                    //因为第一行已经写了表头，所以所有数据都应该从a2开始  
                    Microsoft.Office.Interop.Excel.Range xlrang = null;

                    //irowcount为实际行数，最大行  
                    int irowcount = dtAll.Rows.Count;
                    int iparstedrow = 0, icurrsize = 0;

                    //ieachsize为每次写行的数值，可以自己设置  
                    int ieachsize = 65533;

                    //icolumnaccount为实际列数，最大列数  
                    int icolumnaccount = dtAll.Columns.Count;

                    //在内存中声明一个ieachsize×icolumnaccount的数组，ieachsize是每次最大存储的行数，icolumnaccount就是存储的实际列数  
                    object[,] objval = new object[ieachsize, icolumnaccount];
                    icurrsize = ieachsize;

                    while (iparstedrow < irowcount)
                    {
                        if ((irowcount - iparstedrow) < ieachsize)
                            icurrsize = irowcount - iparstedrow;

                        //用for循环给数组赋值  
                        for (int n = 0; n < icurrsize; n++)
                        {
                            for (int m = 0; m < icolumnaccount; m++)
                            {
                                var v = dtAll.Rows[n + iparstedrow][m];
                                objval[n, m] = v != null ? v.ToString() : "";
                            }
                        }
                        string X = "A" + ((int)(iparstedrow + 3));
                        string col = "";
                        if (icolumnaccount <= 26)
                        {
                            col = ((char)('A' + icolumnaccount - 1)) + ((int)(iparstedrow + icurrsize + 1)).ToString();
                        }
                        else
                        {
                            col = ((char)('A' + (icolumnaccount / 26 - 1))) + ((char)('A' + (icolumnaccount % 26 - 1))).ToString() + ((int)(iparstedrow + icurrsize + 1));
                        }
                        xlrang = worksheetdata.get_Range(X, col);
                        xlrang.NumberFormat = "@";
                        // 调用range的value2属性，把内存中的值赋给excel  
                        xlrang.Value2 = objval;
                        iparstedrow = iparstedrow + icurrsize;
                    }
                }
                //保存工作表  
                workbookdata.SaveAs(filepath, miss, miss, miss, miss, miss, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, miss, miss, miss);
                workbookdata.Close(false, miss, miss);
                appexcel.Workbooks.Close();
                appexcel.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbookdata);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel.Workbooks);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(appexcel);
                GC.Collect();
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                if (appexcel != null)
                {
                    appexcel.Workbooks.Close();
                    appexcel.Quit();
                    appexcel = null;  
                }
            }
            return msg;
        } 
    }
}
