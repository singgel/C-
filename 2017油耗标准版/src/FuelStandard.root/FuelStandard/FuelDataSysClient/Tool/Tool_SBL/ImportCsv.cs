using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FuelDataSysClient.Properties;

namespace FuelDataSysClient.Tool.Tool_SBL
{
    public class ImportCsv
    {
        private string fileMark = "*.xls";
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        string strCon = AccessHelper.conn;
        DataTable checkData = new DataTable();
        string path = Application.StartupPath + Settings.Default["ExcelHeaderTemplate"];
        Dictionary<string, string> dict;  //存放列头转换模板
        IList<string> errorFile = new List<string>();

        public event ProgressEventHandler ProgressDoing;
        public event ProgressCountEventHandel ProgressCountDoing;

        Dictionary<string, Dictionary<string, string>> fileList; //记录文件名

        private List<string> listHoliday; // 节假日数据

        public ImportCsv()
        {
            dictionary.Add("TF", "传统能源");
            dictionary.Add("EF", "纯电动");
            dictionary.Add("PF", "插电式混合动力");
            dictionary.Add("NF", "非插电式混合动力");
            dictionary.Add("BF", "燃料电池");
            checkData = GetCheckData();    //获取参数数据  RLLX_PARAM  

            // 获取节假日数据
            listHoliday = this.GetHoliday();
        }

        /// <summary>
        /// 读取CSV文件
        /// </summary>
        /// <param name="notPath">待上传路径文件夹</param>
        /// <param name="yesPath">已上传路径文件夹</param>
        /// <param name="isMove">是否自动导入</param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<string, string>> ReadCsv(string filePath)
        {
            int Number = 0;
            Dictionary<string, string> error;

            fileList = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                IList<string> fileNameList = GetFileName(filePath, fileMark);
                if (fileNameList.Count > 0)
                {

                    foreach (string str in fileNameList)
                    {
                        bool result = false;
                        DataTable dt = ReadExcel(str, "").Tables[0];//ReadCsvFileToTable(true, ',', str);
                        dt.TableName = GetTableName(str);
                        error = VerifyData(dt);

                        Number++;
                        ProgressCountArgs e = new ProgressCountArgs(fileNameList.Count, Number);
                        if (ProgressCountDoing != null)
                        {
                            ProgressCountDoing(this, e);
                        }
                        string er = string.Empty;
                        if (error.Count == 0)
                        {
                            er = InsertData(dt);
                            Dictionary<string, string> ss = new Dictionary<string, string>();
                            if (string.IsNullOrEmpty(er))
                            {
                                result = true;
                                ss.Add("成功插入" + dt.Rows.Count + "条", "");
                            }
                            else
                            {
                                ss.Add(er, "");
                            }
                            fileList.Add(str, ss);
                        }
                        else
                        {
                            fileList.Add(str, error);
                        }
                        if (result) 
                        {
                            string folderName = Path.Combine(filePath, "已导入燃料数据");
                            MoveFile(GetPathFileName(filePath, str), folderName);
                            fileList.Add(Message(String.Format("{0}移动到{1}成功", GetPathFileName(filePath, str), folderName)), new Dictionary<string, string>());
                        }

                    }
                }
            }
            catch (Exception ex) { fileList.Add(ex.Message, new Dictionary<string, string>()); }
            return fileList;
        }

        /// <summary>
        /// 验证当前DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>返回VIN码与错误信息</returns>
        private Dictionary<string, string> VerifyData(DataTable dt)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            DataRow[] dr = checkData.Select("FUEL_TYPE='" + dictionary[dt.TableName] + "' and STATUS=1");
            foreach (DataRow r in dt.Rows)
            {
                string error = VerifyData(r, dr);
                if (!string.IsNullOrEmpty(error))
                    dict.Add(Convert.ToString(r["VIN"]), error);
            }
            return dict;
        }

        /// <summary>
        /// 插入数据库
        /// </summary>
        /// <param name="dt"></param>
        private string InsertData(DataTable dt)
        {
            int numBer = 0;

            string error = string.Empty;
            if (dt != null && dt.Rows.Count > 0)
            {
                //根据表头 查找 参数数据匹配 插入相应数据
                DataRow[] dr = checkData.Select("FUEL_TYPE='" + dictionary[dt.TableName] + "' and STATUS=1");
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    using (OleDbTransaction tra = con.BeginTransaction())
                    {
                        try
                        {
                            DataTable dtRLLX_PARAM_ENTITY;
                            foreach (DataRow r in dt.Rows)
                            {
                                #region 插入基础数据表

                                StringBuilder strSqlFC_CLJBXX = new StringBuilder();
                                strSqlFC_CLJBXX.Append("insert into FC_CLJBXX(");
                                strSqlFC_CLJBXX.Append("VIN,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,V_ID,HGSPBM,QTXX)");
                                strSqlFC_CLJBXX.Append(" values (");
                                strSqlFC_CLJBXX.Append("@VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLXH,@CLZL,@RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,@CLZZRQ,@UPLOADDEADLINE,@TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,@QDXS,@CREATETIME,@UPDATETIME,@STATUS,@JYJGMC,@JYBGBH,@V_ID,@HGSPBM,QTXX)");
                                OleDbParameter[] parametersFC_CLJBXX = {
				                new OleDbParameter("@VIN", OleDbType.VarChar,17),
				                new OleDbParameter("@USER_ID", OleDbType.VarChar,32),
				                new OleDbParameter("@QCSCQY", OleDbType.VarChar,200),
				                new OleDbParameter("@JKQCZJXS", OleDbType.VarChar,200),
				                new OleDbParameter("@CLXH", OleDbType.VarChar,100),
				                new OleDbParameter("@CLZL", OleDbType.VarChar,200),
				                new OleDbParameter("@RLLX", OleDbType.VarChar,200),
				                new OleDbParameter("@ZCZBZL", OleDbType.VarChar,255),
				                new OleDbParameter("@ZGCS", OleDbType.VarChar,255),
				                new OleDbParameter("@LTGG", OleDbType.VarChar,200),
				                new OleDbParameter("@ZJ", OleDbType.VarChar,255),
				                new OleDbParameter("@CLZZRQ", OleDbType.Date),
				                new OleDbParameter("@UPLOADDEADLINE", OleDbType.Date),
				                new OleDbParameter("@TYMC", OleDbType.VarChar,200),
				                new OleDbParameter("@YYC", OleDbType.VarChar,200),
				                new OleDbParameter("@ZWPS", OleDbType.VarChar,255),
				                new OleDbParameter("@ZDSJZZL", OleDbType.VarChar,255),
				                new OleDbParameter("@EDZK", OleDbType.VarChar,255),
				                new OleDbParameter("@LJ", OleDbType.VarChar,255),
				                new OleDbParameter("@QDXS", OleDbType.VarChar,200),
				                new OleDbParameter("@CREATETIME", OleDbType.Date),
				                new OleDbParameter("@UPDATETIME", OleDbType.Date),
				                new OleDbParameter("@STATUS", OleDbType.VarChar,1),
				                new OleDbParameter("@JYJGMC", OleDbType.VarChar,255),
				                new OleDbParameter("@JYBGBH", OleDbType.VarChar,255),
				                new OleDbParameter("@V_ID", OleDbType.VarChar,255),
                                new OleDbParameter("@HGSPBM", OleDbType.VarChar,255),
                                new OleDbParameter("@QTXX", OleDbType.VarChar,255 )};
                                parametersFC_CLJBXX[0].Value = r["VIN"];
                                parametersFC_CLJBXX[1].Value = "";
                                parametersFC_CLJBXX[2].Value = r["QCSCQY"];
                                parametersFC_CLJBXX[3].Value = r["JKQCZJXS"];
                                parametersFC_CLJBXX[4].Value = r["CLXH"];
                                parametersFC_CLJBXX[5].Value = r["CLZL"];
                                parametersFC_CLJBXX[6].Value = r["RLLX"];
                                parametersFC_CLJBXX[7].Value = r["ZCZBZL"];
                                parametersFC_CLJBXX[8].Value = r["ZGCS"];
                                parametersFC_CLJBXX[9].Value = r["LTGG"];
                                parametersFC_CLJBXX[10].Value = r["ZJ"];
                                DateTime clzzrq = DateTime.Parse(r["CLZZRQ"].ToString());//Utils.de2zu(r["CLZZRQ"].ToString());
                                parametersFC_CLJBXX[11].Value = clzzrq;//r["CLZZRQ"];
                                DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrq);
                                parametersFC_CLJBXX[12].Value = uploadDeadlineDate;//r["UPLOADDEADLINE"];
                                parametersFC_CLJBXX[13].Value = r["TYMC"];
                                parametersFC_CLJBXX[14].Value = r["YYC"];
                                parametersFC_CLJBXX[15].Value = r["ZWPS"];
                                parametersFC_CLJBXX[16].Value = r["ZDSJZZL"];
                                parametersFC_CLJBXX[17].Value = r["EDZK"];
                                parametersFC_CLJBXX[18].Value = r["LJ"];
                                parametersFC_CLJBXX[19].Value = r["QDXS"];
                                parametersFC_CLJBXX[20].Value = DateTime.Now;
                                parametersFC_CLJBXX[21].Value = DateTime.Now;
                                parametersFC_CLJBXX[22].Value = 1;
                                parametersFC_CLJBXX[23].Value = r["JYJGMC"];
                                parametersFC_CLJBXX[24].Value = r["JYBGBH"];
                                parametersFC_CLJBXX[25].Value = "";
                                parametersFC_CLJBXX[26].Value = r["HGSPBM"];
                                parametersFC_CLJBXX[27].Value = r["CT_QTXX"];
                                AccessHelper.ExecuteNonQuery(tra, strSqlFC_CLJBXX.ToString(), parametersFC_CLJBXX);
                                #endregion

                                dtRLLX_PARAM_ENTITY = new DataTable();
                                dtRLLX_PARAM_ENTITY.Columns.Add("PARAM_CODE", System.Type.GetType("System.String"));
                                dtRLLX_PARAM_ENTITY.Columns.Add("VIN", System.Type.GetType("System.String"));
                                dtRLLX_PARAM_ENTITY.Columns.Add("PARAM_VALUE", System.Type.GetType("System.String"));
                                dtRLLX_PARAM_ENTITY.Columns.Add("V_ID", System.Type.GetType("System.String"));
                                #region 插入燃料类别表
                                foreach (DataRow er in dr)
                                {
                                    string paramCode = er["PARAM_CODE"].ToString().Trim();
                                    DataRow ddr = dtRLLX_PARAM_ENTITY.NewRow();
                                    ddr["PARAM_CODE"] = paramCode;
                                    ddr["VIN"] = r["VIN"];
                                    ddr["PARAM_VALUE"] = r[paramCode];
                                    ddr["V_ID"] = "";

                                    dtRLLX_PARAM_ENTITY.Rows.Add(ddr);

                                    //                    StringBuilder strSqlRLLX_PARAM_ENTITY = new StringBuilder();
                                    //                    strSqlRLLX_PARAM_ENTITY.Append("insert into RLLX_PARAM_ENTITY(");
                                    //                    strSqlRLLX_PARAM_ENTITY.Append("PARAM_CODE,VIN,PARAM_VALUE,V_ID)");
                                    //                    strSqlRLLX_PARAM_ENTITY.Append(" values (");
                                    //                    strSqlRLLX_PARAM_ENTITY.Append("@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)");
                                    //                    OleDbParameter[] parametersRLLX_PARAM_ENTITY = {
                                    //new OleDbParameter("@PARAM_CODE", OleDbType.VarChar,200),
                                    //new OleDbParameter("@VIN", OleDbType.VarChar,17),
                                    //new OleDbParameter("@PARAM_VALUE", OleDbType.VarChar,200),
                                    //new OleDbParameter("@V_ID", OleDbType.VarChar,30)};
                                    //                    string paramCode = er["PARAM_CODE"].ToString().Trim();
                                    //                    parametersRLLX_PARAM_ENTITY[0].Value = paramCode;
                                    //                    parametersRLLX_PARAM_ENTITY[1].Value = r["VIN"];
                                    //                    parametersRLLX_PARAM_ENTITY[2].Value = r[paramCode];
                                    //                    parametersRLLX_PARAM_ENTITY[3].Value = "";
                                    //                    AccessHelper.ExecuteNonQuery(tra, strSqlRLLX_PARAM_ENTITY.ToString(), parametersRLLX_PARAM_ENTITY);

                                }

                                OleDbDataAdapter sda = new OleDbDataAdapter("select * from RLLX_PARAM_ENTITY ", con);
                                sda.SelectCommand.Transaction = tra;
                                OleDbCommandBuilder comb = new OleDbCommandBuilder(sda);
                                sda.InsertCommand = comb.GetInsertCommand();
                                sda.Update(dtRLLX_PARAM_ENTITY);

                                #endregion

                                numBer++;
                                ProgressEventArgs e = new ProgressEventArgs(numBer, dt.Rows.Count);
                                if (this.ProgressDoing != null)
                                {
                                    this.ProgressDoing(this, e);
                                }
                            }
                            tra.Commit();
                        }
                        catch (Exception ex)
                        {
                            tra.Rollback();
                            //MessageBox.Show( ex.Message);
                            error = ex.Message;
                        }
                    }
                }
            }
            return error;
        }

        public DataTable ToDataTable(DataRow[] rows)
        {
            if (rows == null || rows.Length == 0) return null;
            DataTable tmp = rows[0].Table.Clone();  // 复制DataRow的表结构
            foreach (DataRow row in rows)
                tmp.Rows.Add(row);  // 将DataRow添加到DataTable中
            return tmp;
        }

        private string Message(string str)
        {
            return DateTime.Now + ":" + str + "\r\n";
        }

        /// <summary>
        /// 从路径中获取文件名 然后和新路径组合
        /// </summary>
        /// <param name="path">新路径</param>
        /// <param name="pathFile">带文件名路径</param>
        /// <returns></returns>
        private string GetPathFileName(string path, string pathFile)
        {
            string[] str = pathFile.Split('\\');
            return path + "\\" + str[str.Length - 1];

        }

        /// <summary>
        /// 验证单行数据
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        private string VerifyData(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;

            // VIN
            string vin = Convert.ToString(r["VIN"]);


            message += this.VerifyRequired("VIN", vin);
            message += this.VerifyStrLen("VIN", vin, 17);
            if (vin.StartsWith("L"))
            {
                message += this.VerifyVin(vin, false);
            }
            if (!vin.StartsWith("L"))
            {
                message += this.VerifyVin(vin, true);
            }

            string Jkqczjxs = Convert.ToString(r["JKQCZJXS"]);
            string Qcscqy = Convert.ToString(r["QCSCQY"]);

            // 乘用车生产企业
            if (string.IsNullOrEmpty(Qcscqy))
            {
                message += "\n乘用车生产企业不能为空!";
            }

            //海关编码
            message += this.VerifyRequired("海关编码", Convert.ToString(r["HGSPBM"]));

            // 产品型号
            string clxh = Convert.ToString(r["CLXH"]);
            message += this.VerifyRequired("产品型号", clxh);
            message += this.VerifyStrLen("产品型号", clxh, 100);

            // 车辆类型
            string Clzl = Convert.ToString(r["CLZL"]);
            message += this.VerifyRequired("车辆类型", Clzl);
            Clzl = Clzl.Replace("(", "（").Replace(")", "）");
            if (Clzl == "乘用车（M1类）")
            {
                Clzl = "乘用车（M1）";
            }
            message += this.VerifyClzl(Clzl);
            message += this.VerifyStrLen("车辆类型", Clzl, 200);

            // 燃料类型
            string Rllx = Convert.ToString(r["RLLX"]);
            message += this.VerifyRequired("燃料种类", Rllx);
            message += this.VerifyStrLen("燃料种类", Rllx, 200);
            message += this.VerifyRllx(Rllx);

            // 整备质量
            string Zczbzl = Convert.ToString(r["ZCZBZL"]);
            message += this.VerifyRequired("整备质量", Zczbzl);
            if (!this.VerifyParamFormat(Zczbzl, ','))
            {
                message += "\n整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格";
            }

            // 最高车速
            string Zgcs = Convert.ToString(r["ZGCS"]);
            message += this.VerifyRequired("最高车速", Zgcs);
            if (!this.VerifyParamFormat(Zgcs, ','))
            {
                message += "\n最高车速应填写整数，多个数值应以半角“,”隔开，中间不留空格";
            }

            // 轮胎规格
            string Ltgg = Convert.ToString(r["LTGG"]);
            message += this.VerifyRequired("轮胎规格", Ltgg);
            message += this.VerifyStrLen("轮胎规格", Ltgg, 200);
            message += this.VerifyLtgg(Ltgg);
            // 前后轮距相同只填写一个型号数据即可，不同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)

            // 轴距
            string Zj = Convert.ToString(r["ZJ"]);
            message += this.VerifyRequired("轴距", Zj);
            message += this.VerifyInt("轴距", Zj);

            // 通用名称
            string Tymc = Convert.ToString(r["Tymc"]);
            message += this.VerifyRequired("通用名称", Tymc);
            message += this.VerifyStrLen("通用名称", Tymc, 200);

            // 越野车（G类）
            string Yyc = Convert.ToString(r["YYC"]);
            message += this.VerifyRequired("越野车（G类）", Yyc);
            message += this.VerifyYyc(Yyc);
            message += this.VerifyStrLen("越野车（G类）", Yyc, 200);

            // 座椅排数
            string Zwps = Convert.ToString(r["ZWPS"]);
            message += this.VerifyRequired("座椅排数", Zwps);
            message += this.VerifyInt("座椅排数", Zwps);

            // 总质量
            string Zdsjzzl = Convert.ToString(r["ZDSJZZL"]);
            string Edzk = Convert.ToString(r["EDZK"]);
            message += this.VerifyRequired("总质量", Zdsjzzl);
            message += this.VerifyZdsjzzl(Zdsjzzl, Zczbzl, Edzk);
            message += this.VerifyInt("总质量", Zdsjzzl);

            // 额定载客
            message += this.VerifyRequired("额定载客", Edzk);
            message += this.VerifyInt("额定载客", Edzk);

            // 前轮距/后轮距
            string Lj = Convert.ToString(r["LJ"]);
            message += this.VerifyRequired("前轮距/后轮距", Lj);
            if (!this.VerifyParamFormat(Lj, '/') && Lj.IndexOf('/') < 0)
            {
                message += "\n前轮距/后轮距应填写整数，前后轮距，中间用”/”隔开";
            }

            // 驱动型式 
            string Qdxs = Convert.ToString(r["QDXS"]);
            message += this.VerifyRequired("驱动型式", Qdxs);
            message += this.VerifyQdxs(Qdxs);
            message += this.VerifyStrLen("驱动型式", Qdxs, 200);

            // 检测机构名称
            string Jyjgmc = Convert.ToString(r["JYJGMC"]);
            message += this.VerifyRequired("检测机构名称", Jyjgmc);
            message += this.VerifyStrLen("检测机构名称", Jyjgmc, 500);

            // 报告编号
            string Jybgbh = Convert.ToString(r["JYBGBH"]);
            message += this.VerifyRequired("报告编号", Jybgbh);
            message += this.VerifyStrLen("报告编号", Jybgbh, 500);

            switch (Rllx)
            {
                case "纯电动":
                    message += this.VerifyCDD(r, dr);
                    break;
                case "非插电式混合动力":
                    message += this.VerifyHHDL(r, dr);
                    break;
                case "插电式混合动力":
                    message += this.VerifyHHDL(r, dr);
                    break;
                case "燃料电池":
                    message += this.VerifyRLDC(r, dr);
                    break;
                default:
                    message += this.VerifyCTNY(r, dr);
                    break;
            }
            return message;
        }

        /// <summary>
        /// 获取全部参数数据
        /// </summary>
        /// <returns></returns>
        private DataTable GetCheckData()
        {
            string sql = "select * from RLLX_PARAM";
            DataSet ds = AccessHelper.ExecuteDataSet(strCon, sql, null);
            return ds.Tables[0];
        }

        /// <summary>
        /// 获取路径文件名前两位当燃料类型
        /// </summary>
        /// <param name="pathFile">路径文件名</param>
        /// <returns></returns>
        private string GetTableName(string pathFile)
        {
            string[] file = pathFile.Split('\\');
            string tableName = file[file.Length - 1].Substring(0, 2);
            return tableName;
        }

        /// <summary>
        /// 通过文件流的方式来读取CSV文件
        /// </summary>
        /// <param name="HeadYes">第一行是否为列标题</param>
        /// <param name="span">分隔符</param>
        /// <param name="files">文件路径和文件名</param>
        /// <returns></returns>
        public DataTable ReadCsvFileToTable(bool HeadYes, char span, string files)
        {
            string tableName = GetTableName(files);    //燃料类型当表头
            DataTable dt = new DataTable(tableName);
            ReadTemplate(tableName);
            StreamReader fileReader = new StreamReader(files, Encoding.Default);
            try
            {
                //是否为第一行（如果HeadYes为TRUE，则第一行为标题行）
                int lsi = 0;

                //列之间的分隔符
                char cv = span;
                while (fileReader.EndOfStream == false)
                {
                    string line = fileReader.ReadLine();
                    string[] y = line.Split(cv);

                    //第一行为标题行
                    if (HeadYes == true)
                    {
                        //第一行
                        if (lsi == 0)
                        {
                            for (int i = 0; i < y.Length; i++)
                            {
                                dt.Columns.Add(s2s(y[i].Trim().ToString()));  //s2s 模板列头转置表列头
                            }
                            lsi++;
                        }
                        //从第二列开始为数据列
                        else
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < y.Length; i++)
                            {
                                dr[i] = y[i].Trim();
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                    //第一行不为标题行
                    else
                    {
                        if (lsi == 0)
                        {
                            for (int i = 0; i < y.Length; i++)
                            {
                                dt.Columns.Add("Col" + i.ToString());
                            }
                            lsi++;
                        }
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < y.Length; i++)
                        {
                            dr[i] = y[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                // throw ex;
            }
            finally
            {
                fileReader.Close();
                fileReader.Dispose();
            }

            return dt;
        }

        /// <summary>
        /// 模板列头转置表列头
        /// </summary>
        /// <param name="str">模板列头</param>
        ///  <param name="type">燃料类型</param>
        /// <returns></returns>
        private string s2s(string str)
        {
            try
            {
                return dict[str];
            }
            catch
            {
                return str;
            }
        }

        /// <summary>
        /// 读取Excel模板
        /// </summary>
        /// <param name="type">燃料类型</param>
        /// <returns></returns>
        private Dictionary<string, string> ReadTemplate(string type)
        {
            type = type + "$";
            DataTable dt = ReadExcel(path, type).Tables[0];
            dict = new Dictionary<string, string>();

            foreach (DataRow r in dt.Rows)
            {
                dict.Add(Convert.ToString(r[0]).Trim(), Convert.ToString(r[1]).Trim());
            }

            return dict;
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="fileName">文件地址</param>
        /// <param name="sheet">名称</param>
        /// <returns></returns>
        public DataSet ReadExcel(string fileName, string sheet)
        {
            string strConn = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'", fileName); //; HDR=No
            DataSet ds = new DataSet();
            OleDbConnection conn = new OleDbConnection(strConn);
            try
            {
                conn.Open();
                DataTable sheetNames = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
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
                    OleDbDataAdapter oada = new OleDbDataAdapter("select * from [" + sheet + "]", strConn);
                    oada.Fill(ds);
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        /// <summary>
        /// 获取路径folderPath下所有以fileMark开头的文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        /// <param name="fileMark">文件名字的开头字符</param>
        /// <returns></returns>
        public IList<string> GetFileName(string folderPath, string fileMark)
        {
            List<string> fileNameList = new List<string>();
            try
            {
                DirectoryInfo folder = new DirectoryInfo(folderPath);

                foreach (FileInfo file in folder.GetFiles(fileMark))
                {
                    if (file.Name.IndexOf("TF") == 0 || file.Name.IndexOf("EF") == 0 || file.Name.IndexOf("PF") == 0 || file.Name.IndexOf("NF") == 0 || file.Name.IndexOf("BF") == 0)
                    {
                        fileNameList.Add(file.FullName);
                    }
                }
            }
            catch (Exception ex) { }
            return fileNameList;
        }

        /// <summary>
        /// 转移文件
        /// </summary>
        /// <param name="srcFileName">源文件路径</param>
        /// <param name="folderPath">目的文件夹路径</param>
        public string MoveFile(string srcFileName, string folderPath)
        {
            string msg = string.Empty;
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string shortFileName = Path.GetFileNameWithoutExtension(srcFileName) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(srcFileName);
                string desFileName = Path.Combine(folderPath, shortFileName);

                File.Move(srcFileName, desFileName);
            }
            catch (Exception ex)
            {
                msg = ex.Message + Environment.NewLine;
            }
            return msg;
        }

        #region 参数验证
        // 验证VIN
        protected string VerifyVin(string vin, bool isImport)
        {
            string message = string.Empty;
            DataCheck dc = new DataCheck();
            char bi;
            try
            {
                if (!isImport)
                {
                    if (!dc.CheckCLSBDH(vin, out bi))
                    {
                        if (bi == '-')
                        {
                            message += "\n请核对【备案号(VIN)】为17位字母或者数字!";
                        }
                        else
                        {
                            message += "\n【备案号(VIN)】校验失败！第9位应为:'" + bi + "'";
                        }
                    }
                }
                else
                {
                    // TODO 进口车验证
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return message;
        }

        // 验证燃料类型
        protected string VerifyRllx(string rllx)
        {
            if (!string.IsNullOrEmpty(rllx))
            {
                if (rllx == "汽油" || rllx == "柴油" || rllx == "两用燃料" || rllx == "双燃料" || rllx == "气体燃料" || rllx == "非插电式混合动力" || rllx == "插电式混合动力" || rllx == "纯电动" || rllx == "燃料电池")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n燃料种类参数填写汽油、柴油、两用燃料、双燃料、气体燃料、纯电动、非插电式混合动力、插电式混合动力、燃料电池";
                }
            }
            return string.Empty;
        }

        protected string VerifyLtgg(string ltgg)
        {
            string message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ltgg))
                {
                    int indexLtgg = ltgg.IndexOf(")/(");
                    if (indexLtgg > -1)
                    {
                        string ltggHead = ltgg.Substring(0, indexLtgg + 1);
                        string ltggEnd = ltgg.Substring(indexLtgg + 3);

                        if (!ltggHead.StartsWith("(") || !ltggEnd.EndsWith(")"))
                        {
                            message = "前后轮距不相同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        // 验证总质量
        protected string VerifyZdsjzzl(string zdsjzzl, string zczbzl, string edzk)
        {
            if (!string.IsNullOrEmpty(zdsjzzl) && !string.IsNullOrEmpty(zczbzl) && !string.IsNullOrEmpty(edzk))
            {
                if (Convert.ToInt32(zdsjzzl) < (Convert.ToInt32(zczbzl) + Convert.ToInt32(edzk) * 65))
                {
                    return "\n总质量应≥整备质量＋乘员质量（额定载客×乘客质量，乘用车按65㎏/人核算)!";
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        // 验证燃料类型
        // 车辆类型
        protected string VerifyClzl(string clzl)
        {
            if (!string.IsNullOrEmpty(clzl))
            {
                if (clzl == "乘用车（M1）" || clzl == "轻型客车（M2）" || clzl == "轻型货车（N1）")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n车辆类型参数应填写“乘用车（M1）/轻型客车（M2）/轻型货车（N1）”";
                }
            }
            return string.Empty;
        }

        // 越野车
        protected string VerifyYyc(string yyc)
        {
            if (!string.IsNullOrEmpty(yyc))
            {
                if (yyc == "是" || yyc == "否")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n越野车(G类)参数应填写“是/否”";
                }
            }
            return string.Empty;
        }

        // 驱动型式
        protected string VerifyQdxs(string qdxs)
        {
            if (!string.IsNullOrEmpty(qdxs))
            {
                if (qdxs == "前轮驱动" || qdxs == "后轮驱动" || qdxs == "分时全轮驱动" || qdxs == "全时全轮驱动" || qdxs == "智能(适时)全轮驱动")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n驱动型式参数应填写“前轮驱动/后轮驱动/分时全轮驱动/全时全轮驱动/智能(适时)全轮驱动”";
                }
            }
            return string.Empty;
        }

        // 变速器型式
        protected string VerifyBsqxs(string bsqxs)
        {
            if (!string.IsNullOrEmpty(bsqxs))
            {
                if (bsqxs == "MT" || bsqxs == "AT" || bsqxs == "AMT" || bsqxs == "CVT" || bsqxs == "DCT" || bsqxs == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n变速器型式参数应填写“MT/AT/AMT/CVT/DCT/其它”";
                }
            }
            return string.Empty;
        }

        // 变速器档位数
        protected string VerifyBsqdws(string bsqdws)
        {
            if (!string.IsNullOrEmpty(bsqdws))
            {
                if (bsqdws == "1" || bsqdws == "2" || bsqdws == "3" || bsqdws == "4" || bsqdws == "5" || bsqdws == "6" || bsqdws == "7" || bsqdws == "8" || bsqdws == "9" || bsqdws == "10" || bsqdws == "N.A")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n变速器档位数参数应填写“1/2/3/4/5/6/7/8/9/10/N.A”";
                }
            }
            return string.Empty;
        }

        // 混合动力结构型式
        protected string VerifyHhdljgxs(string hhdljgxs)
        {
            if (!string.IsNullOrEmpty(hhdljgxs))
            {
                if (hhdljgxs == "串联" || hhdljgxs == "并联" || hhdljgxs == "混联" || hhdljgxs == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n混合动力结构型式参数应填写“串联/并联/混联/其它”";
                }
            }
            return string.Empty;
        }

        // 是否具有行驶模式手动选择功能
        protected string VerifySdxzgn(string sdxzgn)
        {
            if (!string.IsNullOrEmpty(sdxzgn))
            {
                if (sdxzgn == "是" || sdxzgn == "否")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n是否具有行驶模式手动选择功能参数应填写“是/否”";
                }
            }
            return string.Empty;
        }

        // 电动汽车储能装置种类
        protected string VerifyDlxdczzl(string dlxdczzl)
        {
            if (!string.IsNullOrEmpty(dlxdczzl))
            {
                if (dlxdczzl == "铅酸电池" || dlxdczzl == "金属氢化物镍电池" || dlxdczzl == "锂电池" || dlxdczzl == "超级电容" || dlxdczzl == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n电动汽车储能装置种类参数应填写“铅酸电池/金属氢化物镍电池/锂电池/超级电容/其它”";
                }
            }
            return string.Empty;
        }

        #endregion

        #region 燃料类型验证
        /// <summary>
        /// 验证传统能源参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected string VerifyCTNY(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;

            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "CT_PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CT_EDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_JGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_SJGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_SQGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKCO2PFL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CT_ZHGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CT_BSQXS":
                            message += VerifyBsqxs(Convert.ToString(r[code]));
                            break;
                        case "CT_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "CT_JGL" && code != "CT_QTXX")
                    {
                        message += this.VerifyRequired(name, Convert.ToString(r[code]));
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        /// <summary>
        /// 验证纯电动参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected string VerifyCDD(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "CDD_DLXDCBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZEDNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DDXDCZZLYZCZBZLDBZ":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZBCDY":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DDQC30FZZGCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_ZHGKDNXHL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDD_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    message += this.VerifyRequired(name, Convert.ToString(r[code]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        // 验证混合动力参数
        protected string VerifyHHDL(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "FCDS_HHDL_DLXDCBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_EDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_JGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SJGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_SQGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2(name, Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_BSQXS":
                            message += VerifyBsqxs(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_HHDLJGXS":
                            message += VerifyHhdljgxs(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_XSMSSDXZGN":
                            message += VerifySdxzgn(Convert.ToString(r[code]));
                            break;
                        case "FCDS_HHDL_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;

                        case "CDS_HHDL_DLXDCBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKRLXHL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHGKDNXHL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_CDDMSXZGCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_EDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_JGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_ZHKGCO2PL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZBCDY":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_HHDLZDDGLB":
                            message += VerifyFloat2(name, Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_BSQXS":
                            message += VerifyBsqxs(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_BSQDWS":
                            message += VerifyBsqdws(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_HHDLJGXS":
                            message += VerifyHhdljgxs(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_XSMSSDXZGN":
                            message += VerifySdxzgn(Convert.ToString(r[code]));
                            break;
                        case "CDS_HHDL_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "FCDS_HHDL_CDDMSXZGCS" && code != "FCDS_HHDL_CDDMSXZHGKXSLC" && code != "FCDS_HHDL_JGL" && code != "CDS_HHDL_JGL")
                    {
                        message += this.VerifyRequired(name, Convert.ToString(r[code]));
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        // 验证燃料电池参数
        protected string VerifyRLDC(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            try
            {
                foreach (DataRow edr in dr)
                {
                    string code = Convert.ToString(edr["PARAM_CODE"]);
                    string name = Convert.ToString(edr["PARAM_NAME"]);
                    switch (code)
                    {
                        case "RLDC_DDGLMD":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_DDHHJSTJXXDCZBNL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKHQL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_ZHGKXSLC":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_CDDMSXZGXSCS":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJEDGL":
                            message += VerifyFloat(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_QDDJFZNJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPBCGZYL":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_CQPRJ":
                            message += VerifyInt(name, Convert.ToString(r[code]));
                            break;
                        case "RLDC_DLXDCZZL":
                            message += VerifyDlxdczzl(Convert.ToString(r[code]));
                            break;
                        default: break;
                    }
                    if (code != "RLDC_ZHGKHQL" && code != "RLDC_ZHGKXSLC" && code != "RLDC_CDDMSXZGXSCS")
                    {
                        message += this.VerifyRequired(name, Convert.ToString(r[code]));
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        #endregion

        #region 输入验证

        // 验证不为空
        protected string VerifyRequired(string strName, string value)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                msg = "\n" + strName + "不能为空!";
            }
            return msg;
        }

        // 验证字符长度
        protected string VerifyStrLen(string strName, string value, int expectedLen)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > expectedLen)
                {
                    msg = "\n" + strName + "长度过长，最长为" + expectedLen + "位!";
                }
            }
            return msg;
        }

        // 验证整型
        protected string VerifyInt(string strName, string value)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value.ToString(), "^[0-9]*$"))
            {
                msg = "\n" + strName + "应为整数!";
            }
            return msg;
        }

        // 验证浮点型1位小数
        protected string VerifyFloat(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
            {
                msg = "\n" + strName + "应保留1位小数!";
            }
            return msg;
        }

        // 验证浮点型两位小数
        protected string VerifyFloat2(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{2}$"))
            {
                msg = "\n" + strName + "应保留2位小数!";
            }
            return msg;
        }

        // 验证时间类型
        protected string VerifyDateTime(string strName, DateTime value)
        {
            string msg = string.Empty;
            try
            {
                if (value != null)
                {
                    DateTime time = Convert.ToDateTime(value.ToString());
                }
            }
            catch (Exception)
            {
                msg = "\n" + strName + "应为时间类型!";
            }
            return msg;
        }


        // 验证时间类型
        protected string VerifyDateTime(string strName, string value)
        {
            string msg = string.Empty;
            try
            {
                if (value != null)
                {
                    DateTime time = Convert.ToDateTime(value);
                }
            }
            catch (Exception)
            {
                msg = "\n" + strName + "应为时间类型!";
            }
            return msg;
        }

        /// <summary>
        /// 参数格式验证，多个数值以参数c隔开，中间不能有空格
        /// </summary>
        /// <param name="value">参数值</param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool VerifyParamFormat(string value, char c)
        {
            if (!string.IsNullOrEmpty(c.ToString()))
            {
                string[] valueArr = value.Split(c);
                if (valueArr[0] == "" || valueArr[valueArr.Length - 1] == "")
                {
                    return false;
                }
                foreach (string val in valueArr)
                {
                    if (!Regex.IsMatch(val, @"^[+]?\d*$"))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion


        // 查询数据上报的截止日期
        public DateTime QueryUploadDeadLine(DateTime manufactureDate)
        {
            DateTime deadLine = new DateTime();

            try
            {
                int timeCons = Utils.timeCons;
                string strManufactureDate = manufactureDate.ToString("yyyy-MM-dd");
                if (!string.IsNullOrEmpty(strManufactureDate) && timeCons > 0)
                {
                    // 限制时长的开始计时日期为（制造日+1天）的零点
                    DateTime startDate = Convert.ToDateTime(strManufactureDate).AddDays(1);

                    // 临时截止日期为 开始计时时间+限制时长
                    deadLine = startDate.Add(new TimeSpan(timeCons, 0, 0));

                    // 查看 开始计时时间和临时截止日期 之间有无节假日
                    for (DateTime dt = startDate; dt < deadLine; dt = dt.AddDays(1))
                    {
                        if (this.VerifyHolidays(dt.ToString("yyyy-MM-dd")))
                        {
                            deadLine = deadLine.AddDays(1);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return deadLine;
        }

        // 验证节假日
        protected bool VerifyHolidays(string date)
        {
            if (string.IsNullOrEmpty(date))
            {
                return false;
            }

            try
            {
                if (listHoliday != null && listHoliday.Contains(date))
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        protected List<string> GetHoliday()
        {
            List<string> holidayList = new List<string>();
            try
            {
                string sqlHol = string.Format(@"SELECT HOL_DAYS FROM FC_HOLIDAY");
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlHol, null);
                //dbUtil.QuerySingleDT(sqlHol);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        holidayList.Add(dr["HOL_DAYS"].ToString());
                    }
                }
            }
            catch (Exception)
            {
            }
            return holidayList;
        }

        /*--------------------------------功能分隔符------------------------------------*/

        /// <summary>
        /// 根据导入EXCEL 查询 本地库数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DataSet ReadSharchExcel(string path, string sheet, string status)
        {
            DataSet ds = ReadExcel(path, sheet);
            StringBuilder strAdd = new StringBuilder();
            strAdd.Append("select * from FC_CLJBXX where VIN in(");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    strAdd.Append("'");
                    strAdd.Append(Convert.ToString(r["VIN"]));
                    strAdd.Append("',");
                }
                string sql = strAdd.ToString().TrimEnd(',') + ")";
                return AccessHelper.ExecuteDataSet(strCon, String.Format("{0} and STATUS='{1}'", sql, status), null);
            }
            return new DataSet();
        }

        /// <summary>
        /// 查询EXCEL中VIN状态
        /// </summary>
        /// <param name="vin">VIN码</param>
        /// <returns></returns>
        private int SearchStatus(string vin)
        {
            string sql = String.Format("select status from FC_CLJBXX where VIN='{0}'", vin);
            return Convert.ToInt32(AccessHelper.ExecuteScalar(strCon, sql, null));
        }

        /// <summary>
        /// 批量修改进口日期
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public int ReadUpdateDate(string path, string sheet)
        {
            int result = 0;
            DataSet ds = ReadExcel(path, sheet);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    using (OleDbTransaction tra = con.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataRow r in ds.Tables[0].Rows)
                            {
                                string statuswhere = string.Empty;
                                int status = SearchStatus(Convert.ToString(r["VIN"]));
                                bool rel = false;
                                switch (status)
                                {
                                    case (int)Status.待上报:
                                        rel = true;
                                        break;
                                    case (int)Status.已上报:
                                        statuswhere = ", STATUS=" + (int)Status.修改待上报;
                                        rel = true;
                                        break;
                                    case (int)Status.修改待上报:
                                        rel = true;
                                        break;
                                }

                                if (rel)
                                {
                                    DateTime clzzrqDate = Convert.ToDateTime(r[1].ToString());
                                    DateTime uploadDeadlineDate = this.QueryUploadDeadLine(clzzrqDate);
                                    string sql = "UPDATE FC_CLJBXX SET CLZZRQ='" + clzzrqDate + "', UPLOADDEADLINE='" + uploadDeadlineDate + "'" + statuswhere + "  where VIN='" + r["VIN"] + "'";
                                    AccessHelper.ExecuteNonQuery(tra, sql, null);
                                }
                            }
                            tra.Commit();
                            result = 1;
                        }
                        catch
                        {
                            tra.Rollback();
                        }
                    }
                }
            }
            return result;
        }

    }

}
