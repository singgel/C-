using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace FuelDataSysClient.Tool
{
    public class DataVerifyHelper
    {

        /// <summary>
        /// 验证整车基础数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>返回VIN码与错误信息</returns>
        public static Dictionary<string, string> VerifyCLJBXXData(DataTable dt)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (DataRow r in dt.Rows)
            {
                string message = string.Empty;
                // 生产OCN
                string SC_OCN = Convert.ToString(r["SC_OCN"]).Trim();
                message += DataVerifyHelper.VerifyRequired("生产OCN", SC_OCN);
                message += DataVerifyHelper.VerifyStrLen("生产OCN", SC_OCN, 100);
                // 系统OCN
                string XT_OCN = Convert.ToString(r["XT_OCN"]).Trim();
                message += DataVerifyHelper.VerifyRequired("系统OCN", XT_OCN);
                message += DataVerifyHelper.VerifyStrLen("系统OCN", XT_OCN, 100);
                // MI+系统OCN
                string MI_XT_OCN = Convert.ToString(r["MI_XT_OCN"]).Trim();
                message += DataVerifyHelper.VerifyRequired("MI+系统OCN", MI_XT_OCN);
                message += DataVerifyHelper.VerifyStrLen("MI+系统OCN", MI_XT_OCN, 100);
                // 通用名称
                string TYMC = Convert.ToString(r["TYMC"]).Trim();
                message += DataVerifyHelper.VerifyRequired("通用名称", TYMC);
                message += DataVerifyHelper.VerifyStrLen("通用名称", TYMC, 200);
                // 车辆型号
                string CLXH = Convert.ToString(r["CLXH"]).Trim();
                message += DataVerifyHelper.VerifyRequired("车辆型号", CLXH);
                message += DataVerifyHelper.VerifyStrLen("车辆型号", CLXH, 100);
                // 排放标准
                string PFBZ = Convert.ToString(r["PFBZ"]).Trim();
                message += DataVerifyHelper.VerifyRequired("排放标准", PFBZ);
                message += DataVerifyHelper.VerifyStrLen("排放标准", PFBZ, 200);
                // 是否进口汽车
                string SFJKQC = Convert.ToString(r["SFJKQC"]).Trim();
                message += DataVerifyHelper.VerifyRequired("是否进口汽车", SFJKQC);
                message += DataVerifyHelper.VerifyStrLen("是否进口汽车", SFJKQC, 100);
                // 汽车生产企业
                string QCSCQY = Convert.ToString(r["QCSCQY"]).Trim();
                message += DataVerifyHelper.VerifyRequired("汽车生产企业", QCSCQY);
                message += DataVerifyHelper.VerifyStrLen("汽车生产企业", QCSCQY, 100);
                // 进口汽车总经销商
                //string JKQCZJXS = Convert.ToString(r["JKQCZJXS"]).Trim();
                //message += DataVerifyHelper.VerifyRequired("进口汽车总经销商", JKQCZJXS);
                //message += DataVerifyHelper.VerifyStrLen("进口汽车总经销商", JKQCZJXS, 100);
                // 检测机构名称
                string JCJGMC = Convert.ToString(r["JCJGMC"]).Trim();
                message += DataVerifyHelper.VerifyRequired("检测机构名称", JCJGMC);
                message += DataVerifyHelper.VerifyStrLen("检测机构名称", JCJGMC, 500);
                // 报告编号
                string BGBH = Convert.ToString(r["BGBH"]).Trim();
                message += DataVerifyHelper.VerifyRequired("报告编号", BGBH);
                message += DataVerifyHelper.VerifyStrLen("报告编号", BGBH, 500);
                // 备案号
                //string BAH = Convert.ToString(r["BAH"]).Trim();
                //message += DataVerifyHelper.VerifyRequired("备案号", BAH);
                //message += DataVerifyHelper.VerifyStrLen("备案号", BAH, 17);
                // 车辆制造日期/进口日期
                //string CLZZRQ = Convert.ToString(r["CLZZRQ"]).Trim();
                //message += DataVerifyHelper.VerifyRequired("备案号", CLZZRQ);
                //message += DataVerifyHelper.VerifyStrLen("备案号", CLZZRQ, 17);
                // 车辆种类
                string CLZL = Convert.ToString(r["CLZL"]).Trim();
                message += DataVerifyHelper.VerifyRequired("车辆种类", CLZL);
                message += DataVerifyHelper.VerifyStrLen("车辆种类", CLZL, 200);
                // 越野车（G类）
                string YYC = Convert.ToString(r["YYC"]).Trim();
                message += DataVerifyHelper.VerifyRequired("越野车（G类）", YYC);
                message += DataVerifyHelper.VerifyStrLen("越野车（G类）", YYC, 100);
                // 驱动型式
                string QDXS = Convert.ToString(r["QDXS"]).Trim();
                message += DataVerifyHelper.VerifyRequired("驱动型式", QDXS);
                message += DataVerifyHelper.VerifyQdxs(QDXS);
                message += DataVerifyHelper.VerifyStrLen("驱动型式", QDXS, 200);
                // 座位排数
                string ZWPS = Convert.ToString(r["ZWPS"]).Trim();
                message += DataVerifyHelper.VerifyRequired("座位排数", ZWPS);
                message += DataVerifyHelper.VerifyInt("座位排数", ZWPS);
                // 最高车速(km/h)
                string ZGCS = Convert.ToString(r["ZGCS"]).Trim();
                message += DataVerifyHelper.VerifyRequired("最高车速(km/h)", ZGCS);
                message += DataVerifyHelper.VerifyInt("最高车速(km/h)", ZGCS);
                // 额定载客（人）
                string EDZK = Convert.ToString(r["EDZK"]).Trim();
                message += DataVerifyHelper.VerifyRequired("额定载客（人）", EDZK);
                message += DataVerifyHelper.VerifyInt("额定载客（人）", EDZK);
                // 轮胎规格
                string LTGG = Convert.ToString(r["LTGG"]).Trim();
                message += DataVerifyHelper.VerifyRequired("轮胎规格", LTGG);
                message += DataVerifyHelper.VerifyStrLen("轮胎规格", LTGG, 200);
                message += DataVerifyHelper.VerifyLtgg(LTGG);
                // 轮距（前/后）(mm)
                string LJ = Convert.ToString(r["LJ"]).Trim();
                message += DataVerifyHelper.VerifyRequired("轮距（前/后）", LJ);
                if (!DataVerifyHelper.VerifyParamFormat(LJ, '/') && LJ.IndexOf('/') < 0)
                {
                    message += "\n轮距（前/后）应填写整数，前后轮距，中间用”/”隔开";
                }
                // 轴距(mm)
                string ZJ = Convert.ToString(r["ZJ"]).Trim();
                message += DataVerifyHelper.VerifyRequired("轴距(mm)", ZJ);
                message += DataVerifyHelper.VerifyInt("轴距(mm)", ZJ);
                // 燃料类型
                string RLLX = Convert.ToString(r["RLLX"]).Trim();
                message += DataVerifyHelper.VerifyRequired("燃料类型", RLLX);
                message += DataVerifyHelper.VerifyStrLen("燃料类型", RLLX, 100);
                message += DataVerifyHelper.VerifyRllx(RLLX);
                // 油耗打印备案号
                string YHDYBAH = Convert.ToString(r["YHDYBAH"]).Trim();
                message += DataVerifyHelper.VerifyRequired("油耗打印备案号", YHDYBAH);
                message += DataVerifyHelper.VerifyStrLen("油耗打印备案号", YHDYBAH, 100);
                // 整车整备质量(kg)
                string ZCZBZL = Convert.ToString(r["ZCZBZL"]).Trim();
                message += DataVerifyHelper.VerifyRequired("整车整备质量(kg)", ZCZBZL);
                message += DataVerifyHelper.VerifyInt("整车整备质量(kg)", ZCZBZL);
                // 最大设计总质量(kg)
                string ZDSJZZL = Convert.ToString(r["ZDSJZZL"]).Trim();
                message += DataVerifyHelper.VerifyRequired("最大设计总质量(kg)", ZDSJZZL);
                message += DataVerifyHelper.VerifyInt("最大设计总质量(kg)", ZDSJZZL);
                // 综合工况燃料消耗量
                string ZHGKRLXHL = Convert.ToString(r["ZHGKRLXHL"]).Trim();
                message += DataVerifyHelper.VerifyRequired("综合工况燃料消耗量", ZHGKRLXHL);
                message += DataVerifyHelper.VerifyFloat("综合工况燃料消耗量", ZHGKRLXHL);
                // 燃料消耗量目标值
                string RLXHLMBZ = Convert.ToString(r["RLXHLMBZ"]).Trim();
                message += DataVerifyHelper.VerifyRequired("燃料消耗量目标值", RLXHLMBZ);
                message += DataVerifyHelper.VerifyFloat("燃料消耗量目标值", RLXHLMBZ);
                // 4阶段标准目标值
                string JDBZMBZ4 = Convert.ToString(r["JDBZMBZ4"]).Trim();
                message += DataVerifyHelper.VerifyRequired("4阶段标准目标值", JDBZMBZ4);
                message += DataVerifyHelper.VerifyFloat("4阶段标准目标值", JDBZMBZ4);
                // 变速器形式
                string BSQXS = Convert.ToString(r["BSQXS"]).Trim();
                message += DataVerifyHelper.VerifyRequired("变速器形式", BSQXS);
                message += DataVerifyHelper.VerifyBsqxs(BSQXS);
                // 排量
                string PL = Convert.ToString(r["PL"]).Trim();
                message += DataVerifyHelper.VerifyRequired("排量", PL);
                message += DataVerifyHelper.VerifyStrLen("排量", PL, 100);
                // 纯电动驱动模式综合工况续航里程
                //string CDDQDMSZHGKXHLC = Convert.ToString(r["CDDQDMSZHGKXHLC"]).Trim();
                //message += DataVerifyHelper.VerifyRequired("纯电动驱动模式综合工况续航里程", CDDQDMSZHGKXHLC);
                //message += DataVerifyHelper.VerifyStrLen("纯电动驱动模式综合工况续航里程", CDDQDMSZHGKXHLC, 100);
                // 验证结果
                if (!string.IsNullOrEmpty(message))
                {
                    dict.Add(Convert.ToString(r["SC_OCN"]), message);
                }
            }
            return dict;
        }

        /// <summary>
        /// 验证整车基础数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>返回VIN码与错误信息</returns>
        public static Dictionary<string, string> VerifyRLLXPARAMData(DataTable dt)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            int paramCountCTNY = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='传统能源'"));
            int paramCountCDD = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='纯电动'"));
            int paramCountCDS = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='插电式混合动力'"));
            int paramCountFCDS = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='非插电式混合动力'"));
            int paramCountRLDC = Convert.ToInt32(OracleHelper.GetSingle(OracleHelper.conn, "SELECT COUNT(*) FROM RLLX_PARAM WHERE STATUS='1' AND FUEL_TYPE='燃料电池'"));
            foreach (DataRow r in dt.Rows)
            {
                string message = string.Empty;
                // 生产OCN
                string SC_OCN = Convert.ToString(r["SC_OCN"]).Trim();
                message += DataVerifyHelper.VerifyRequired("生产OCN", SC_OCN);
                message += DataVerifyHelper.VerifyStrLen("生产OCN", SC_OCN, 100);
                // 参数编码
                string CSBM = Convert.ToString(r["CSBM"]).Trim();
                message += DataVerifyHelper.VerifyRequired("参数编码", CSBM);
                message += DataVerifyHelper.VerifyStrLen("参数编码", CSBM, 100);
                // 参数名称
                string CSMC = Convert.ToString(r["CSMC"]).Trim();
                message += DataVerifyHelper.VerifyRequired("参数名称", CSMC);
                message += DataVerifyHelper.VerifyStrLen("参数名称", CSMC, 100);
                // 燃料类型
                string RLLX = Convert.ToString(r["RLLX"]).Trim();
                message += DataVerifyHelper.VerifyRequired("燃料类型", RLLX);
                message += DataVerifyHelper.VerifyStrLen("燃料类型", RLLX, 100);
                // 参数个数验证
                if (RLLX.Equals("纯电动") && paramCountCDD != dt.Rows.Count)
                {
                    message += RLLX + "的燃料参数个数应为" + paramCountCDD + "个，当前为" + dt.Rows.Count + "个";
                    break;
                }
                else if (RLLX.Equals("非插电式混合动力") && paramCountFCDS != dt.Rows.Count)
                {
                    message += RLLX + "的燃料参数个数应为" + paramCountFCDS + "个，当前为" + dt.Rows.Count + "个";
                    break;
                }
                else if (RLLX.Equals("插电式混合动力") && paramCountCDS != dt.Rows.Count)
                {
                    message += RLLX + "的燃料参数个数应为" + paramCountCDS + "个，当前为" + dt.Rows.Count + "个";
                    break;
                }
                else if (RLLX.Equals("燃料电池") && paramCountRLDC != dt.Rows.Count)
                {
                    message += RLLX + "的燃料参数个数应为" + paramCountRLDC + "个，当前为" + dt.Rows.Count + "个";
                    break;
                }
                else if(paramCountCTNY != dt.Rows.Count)
                {
                    message += RLLX + "的燃料参数个数应为" + paramCountCTNY + "个，当前为" + dt.Rows.Count + "个";
                    break;
                }
                // 参数值
                switch (RLLX)
                {
                    case "纯电动":
                        message += DataVerifyHelper.VerifyCDD_RLLXPARAM(r);
                        break;
                    case "非插电式混合动力":
                        message += DataVerifyHelper.VerifyHHDL_RLLXPARAM(r);
                        break;
                    case "插电式混合动力":
                        message += DataVerifyHelper.VerifyHHDL_RLLXPARAM(r);
                        break;
                    case "燃料电池":
                        message += DataVerifyHelper.VerifyRLDC_RLLXPARAM(r);
                        break;
                    default:
                        message += DataVerifyHelper.VerifyCTNY_RLLXPARAM(r);
                        break;
                }
                // 验证结果
                if (!string.IsNullOrEmpty(message))
                {
                    dict.Add(Convert.ToString(String.Format("{0} {1}", r["SC_OCN"], r["CSBM"])), message);
                }
            }
            return dict;
        }

        /// <summary>
        /// 验证当前DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>返回VIN码与错误信息</returns>
        public static Dictionary<string, string> VerifyData(DataTable dt)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            DataRow[] dr = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from RLLX_PARAM", null).Tables[0].Select(String.Format("FUEL_TYPE='{0}' and STATUS=1", dt.TableName));
            foreach (DataRow r in dt.Rows)
            {
                string error = VerifyData(r, dr);
                if (!string.IsNullOrEmpty(error))
                    dict.Add(Convert.ToString(r["VIN"]), error);
            }
            return dict;
        }

        /// <summary>
        /// 验证单行数据
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected static string VerifyData(DataRow r, DataRow[] dr)
        {
            string message = string.Empty;
            // VIN
            string vin = Convert.ToString(r["VIN"]);
            message += DataVerifyHelper.VerifyRequired("VIN", vin);
            message += DataVerifyHelper.VerifyStrLen("VIN", vin, 17);
            if (vin.StartsWith("L"))
            {
                message += DataVerifyHelper.VerifyVin(vin, false);
            }
            if (!vin.StartsWith("L"))
            {
                message += DataVerifyHelper.VerifyVin(vin, true);
            }
            // 汽车生产企业
            string Qcscqy = Convert.ToString(r["QCSCQY"]);
            if (string.IsNullOrEmpty(Qcscqy))
            {
                message += "\n汽车生产企业不能为空!";
            }
            // 车辆型号
            string clxh = Convert.ToString(r["CLXH"]);
            message += DataVerifyHelper.VerifyRequired("车辆型号", clxh);
            message += DataVerifyHelper.VerifyStrLen("车辆型号", clxh, 100);
            // 车辆种类
            string Clzl = Convert.ToString(r["CLZL"]);
            message += DataVerifyHelper.VerifyRequired("车辆种类", Clzl);
            Clzl = Clzl.Replace("(", "（").Replace(")", "）");
            if (Clzl == "乘用车（M1类）")
            {
                Clzl = "乘用车（M1）";
            }
            message += DataVerifyHelper.VerifyClzl(Clzl);
            message += DataVerifyHelper.VerifyStrLen("车辆种类", Clzl, 200);
            // 燃料类型
            string Rllx = Convert.ToString(r["RLLX"]);
            message += DataVerifyHelper.VerifyRequired("燃料类型", Rllx);
            message += DataVerifyHelper.VerifyStrLen("燃料类型", Rllx, 200);
            message += DataVerifyHelper.VerifyRllx(Rllx);
            // 整车整备质量
            string Zczbzl = Convert.ToString(r["ZCZBZL"]);
            message += DataVerifyHelper.VerifyRequired("整车整备质量", Zczbzl);
            if (!DataVerifyHelper.VerifyParamFormat(Zczbzl, ','))
            {
                message += "\n整车整备质量应填写整数，多个数值应以半角“,”隔开，中间不留空格";
            }
            // 最高车速
            string Zgcs = Convert.ToString(r["ZGCS"]);
            message += DataVerifyHelper.VerifyRequired("最高车速", Zgcs);
            if (!DataVerifyHelper.VerifyParamFormat(Zgcs, ','))
            {
                message += "\n最高车速应填写整数，多个数值应以半角“,”隔开，中间不留空格";
            }
            // 轮胎规格
            string Ltgg = Convert.ToString(r["LTGG"]);
            message += DataVerifyHelper.VerifyRequired("轮胎规格", Ltgg);
            message += DataVerifyHelper.VerifyStrLen("轮胎规格", Ltgg, 200);
            message += DataVerifyHelper.VerifyLtgg(Ltgg);
            // 前后轮距相同只填写一个型号数据即可，不同以(前轮轮胎型号)/(后轮轮胎型号)(引号内为半角括号，且中间不留不必要的空格)
            // 轴距
            string Zj = Convert.ToString(r["ZJ"]);
            message += DataVerifyHelper.VerifyRequired("轴距", Zj);
            message += DataVerifyHelper.VerifyInt("轴距", Zj);
            // 通用名称
            string Tymc = Convert.ToString(r["Tymc"]);
            message += DataVerifyHelper.VerifyRequired("通用名称", Tymc);
            message += DataVerifyHelper.VerifyStrLen("通用名称", Tymc, 200);
            // 越野车（G类）
            string Yyc = Convert.ToString(r["YYC"]);
            message += DataVerifyHelper.VerifyRequired("越野车（G类）", Yyc);
            message += DataVerifyHelper.VerifyYyc(Yyc);
            message += DataVerifyHelper.VerifyStrLen("越野车（G类）", Yyc, 200);
            // 座位排数
            string Zwps = Convert.ToString(r["ZWPS"]);
            message += DataVerifyHelper.VerifyRequired("座位排数", Zwps);
            message += DataVerifyHelper.VerifyInt("座位排数", Zwps);
            // 最大设计总质量
            string Zdsjzzl = Convert.ToString(r["ZDSJZZL"]);
            string Edzk = Convert.ToString(r["EDZK"]);
            message += DataVerifyHelper.VerifyRequired("最大设计总质量", Zdsjzzl);
            message += DataVerifyHelper.VerifyZdsjzzl(Zdsjzzl, Zczbzl, Edzk);
            message += DataVerifyHelper.VerifyInt("最大设计总质量", Zdsjzzl);
            // 额定载客
            message += DataVerifyHelper.VerifyRequired("额定载客", Edzk);
            message += DataVerifyHelper.VerifyInt("额定载客", Edzk);
            // 轮距（前/后）
            string Lj = Convert.ToString(r["LJ"]);
            message += DataVerifyHelper.VerifyRequired("轮距（前/后）", Lj);
            if (!DataVerifyHelper.VerifyParamFormat(Lj, '/') && Lj.IndexOf('/') < 0)
            {
                message += "\n轮距（前/后）应填写整数，前后轮距，中间用”/”隔开";
            }
            // 驱动型式 
            string Qdxs = Convert.ToString(r["QDXS"]);
            message += DataVerifyHelper.VerifyRequired("驱动型式", Qdxs);
            message += DataVerifyHelper.VerifyQdxs(Qdxs);
            message += DataVerifyHelper.VerifyStrLen("驱动型式", Qdxs, 200);
            // 检测机构名称
            string Jyjgmc = Convert.ToString(r["JYJGMC"]);
            message += DataVerifyHelper.VerifyRequired("检测机构名称", Jyjgmc);
            message += DataVerifyHelper.VerifyStrLen("检测机构名称", Jyjgmc, 500);
            // 报告编号
            string Jybgbh = Convert.ToString(r["JYBGBH"]);
            message += DataVerifyHelper.VerifyRequired("报告编号", Jybgbh);
            message += DataVerifyHelper.VerifyStrLen("报告编号", Jybgbh, 500);
            switch (Rllx)
            {
                case "纯电动":
                    message += DataVerifyHelper.VerifyCDD(r, dr);
                    break;
                case "非插电式混合动力":
                    message += DataVerifyHelper.VerifyHHDL(r, dr);
                    break;
                case "插电式混合动力":
                    message += DataVerifyHelper.VerifyHHDL(r, dr);
                    break;
                case "燃料电池":
                    message += DataVerifyHelper.VerifyRLDC(r, dr);
                    break;
                default:
                    message += DataVerifyHelper.VerifyCTNY(r, dr);
                    break;
            }
            return message;
        }

        // 验证VIN
        protected static string VerifyVin(string vin, bool isImport)
        {
            string message = string.Empty;
            DataCheckVINHelper dc = new DataCheckVINHelper();
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
                            message += String.Format("\n【备案号(VIN)】校验失败！第9位应为:'{0}'", bi);
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
        protected static string VerifyRllx(string rllx)
        {
            if (!string.IsNullOrEmpty(rllx))
            {
                if (rllx == "汽油" || rllx == "柴油" || rllx == "两用燃料" || rllx == "双燃料" || rllx == "非插电式混合动力" || rllx == "插电式混合动力" || rllx == "纯电动" || rllx == "燃料电池")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n燃料类型参数填写汽油、柴油、两用燃料、双燃料、纯电动、非插电式混合动力、插电式混合动力、燃料电池";
                }
            }
            return string.Empty;
        }

        // 验证轮胎规格
        protected static string VerifyLtgg(string ltgg)
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

        // 验证最大设计总质量
        protected static string VerifyZdsjzzl(string zdsjzzl, string zczbzl, string edzk)
        {
            if (!string.IsNullOrEmpty(zdsjzzl) && !string.IsNullOrEmpty(zczbzl) && !string.IsNullOrEmpty(edzk))
            {
                if (Convert.ToInt32(zdsjzzl) < (Convert.ToInt32(zczbzl) + Convert.ToInt32(edzk) * 65))
                {
                    return "\n最大设计总质量应≥整车整备质量＋乘员质量（额定载客×乘客质量，乘用车按65㎏/人核算)!";
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        // 车辆种类
        protected static string VerifyClzl(string clzl)
        {
            if (!string.IsNullOrEmpty(clzl))
            {
                if (clzl == "乘用车（M1）" || clzl == "轻型客车（M2）" || clzl == "轻型货车（N1）")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n车辆种类参数应填写“乘用车（M1）/轻型客车（M2）/轻型货车（N1）”";
                }
            }
            return string.Empty;
        }

        // 越野车
        protected static string VerifyYyc(string yyc)
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
        protected static string VerifyQdxs(string qdxs)
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
        protected static string VerifyBsqxs(string bsqxs)
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
        protected static string VerifyBsqdws(string bsqdws)
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
        protected static string VerifyHhdljgxs(string hhdljgxs)
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
        protected static string VerifySdxzgn(string sdxzgn)
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

        // 动力蓄电池组种类
        protected static string VerifyDlxdczzl(string dlxdczzl)
        {
            if (!string.IsNullOrEmpty(dlxdczzl))
            {
                if (dlxdczzl == "铅酸电池" || dlxdczzl == "金属氢化物镍电池" || dlxdczzl == "锂电池" || dlxdczzl == "超级电容" || dlxdczzl == "其它")
                {
                    return string.Empty;
                }
                else
                {
                    return "\n动力蓄电池组种类参数应填写“铅酸电池/金属氢化物镍电池/锂电池/超级电容/其它”";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 验证传统能源参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected static string VerifyCTNY(DataRow r, DataRow[] dr)
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
                        message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r[code]));
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
        protected static string VerifyCDD(DataRow r, DataRow[] dr)
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
                            message += VerifyInt(name, Convert.ToString(r[code]));
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
                    message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r[code]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        /// <summary>
        /// 验证非插电式&插电式混合动力参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected static string VerifyHHDL(DataRow r, DataRow[] dr)
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
                        message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r[code]));
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
        /// 验证燃料电池参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        protected static string VerifyRLDC(DataRow r, DataRow[] dr)
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
                        message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r[code]));
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
        /// 验证传统能源参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        public static string VerifyCTNY_RLLXPARAM(DataRow r)
        {
            string message = string.Empty;

            try
            {
                string code = Convert.ToString(r["CSBM"]);
                string name = Convert.ToString(r["CSMC"]);
                switch (code)
                {
                    case "CT_PL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_EDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_JGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_SJGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_SQGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_ZHGKCO2PFL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_ZHGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_BSQXS":
                        message += VerifyBsqxs(Convert.ToString(r["CSZ"]));
                        break;
                    case "CT_BSQDWS":
                        message += VerifyBsqdws(Convert.ToString(r["CSZ"]));
                        break;
                    default: break;
                }
                if (code != "CT_JGL" && code != "CT_QTXX")
                {
                    message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r["CSZ"]));
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
        public static string VerifyCDD_RLLXPARAM(DataRow r)
        {
            string message = string.Empty;
            try
            {
                string code = Convert.ToString(r["CSBM"]);
                string name = Convert.ToString(r["CSMC"]);
                switch (code)
                {
                    case "CDD_DLXDCBNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_DLXDCZEDNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_DDXDCZZLYZCZBZLDBZ":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_DLXDCZBCDY":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_DDQC30FZZGCS":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_ZHGKXSLC":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_QDDJFZNJ":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_QDDJEDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_ZHGKDNXHL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDD_DLXDCZZL":
                        message += VerifyDlxdczzl(Convert.ToString(r["CSZ"]));
                        break;
                    default: break;
                }
                message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r["CSZ"]));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        /// <summary>
        /// 验证非插电式&插电式混合动力参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        public static string VerifyHHDL_RLLXPARAM(DataRow r)
        {
            string message = string.Empty;
            try
            {
                string code = Convert.ToString(r["CSBM"]);
                string name = Convert.ToString(r["CSMC"]);
                switch (code)
                {
                    case "FCDS_HHDL_DLXDCBNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_DLXDCZZNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_ZHGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_EDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_JGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_PL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_ZHKGCO2PL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_DLXDCZBCDY":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_SJGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_SQGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_CDDMSXZGCS":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_CDDMSXZHGKXSLC":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_QDDJFZNJ":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_QDDJEDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_HHDLZDDGLB":
                        message += VerifyFloat2(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_BSQXS":
                        message += VerifyBsqxs(Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_BSQDWS":
                        message += VerifyBsqdws(Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_HHDLJGXS":
                        message += VerifyHhdljgxs(Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_XSMSSDXZGN":
                        message += VerifySdxzgn(Convert.ToString(r["CSZ"]));
                        break;
                    case "FCDS_HHDL_DLXDCZZL":
                        message += VerifyDlxdczzl(Convert.ToString(r["CSZ"]));
                        break;

                    case "CDS_HHDL_DLXDCBNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_DLXDCZZNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_ZHGKRLXHL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_ZHGKDNXHL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_CDDMSXZHGKXSLC":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_CDDMSXZGCS":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_QDDJFZNJ":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_QDDJEDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_EDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_JGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_PL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_ZHKGCO2PL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_DLXDCZBCDY":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_HHDLZDDGLB":
                        message += VerifyFloat2(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_BSQXS":
                        message += VerifyBsqxs(Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_BSQDWS":
                        message += VerifyBsqdws(Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_HHDLJGXS":
                        message += VerifyHhdljgxs(Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_XSMSSDXZGN":
                        message += VerifySdxzgn(Convert.ToString(r["CSZ"]));
                        break;
                    case "CDS_HHDL_DLXDCZZL":
                        message += VerifyDlxdczzl(Convert.ToString(r["CSZ"]));
                        break;
                    default: break;
                }
                if (code != "FCDS_HHDL_CDDMSXZGCS" && code != "FCDS_HHDL_CDDMSXZHGKXSLC" && code != "FCDS_HHDL_JGL" && code != "CDS_HHDL_JGL")
                {
                    message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r["CSZ"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        /// <summary>
        /// 验证燃料电池参数
        /// </summary>
        /// <param name="r">验证数据</param>
        /// <param name="dr">匹配数据</param>
        /// <returns></returns>
        public static string VerifyRLDC_RLLXPARAM(DataRow r)
        {
            string message = string.Empty;
            try
            {
                string code = Convert.ToString(r["CSBM"]);
                string name = Convert.ToString(r["CSMC"]);
                switch (code)
                {
                    case "RLDC_DDGLMD":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_DDHHJSTJXXDCZBNL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_ZHGKHQL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_ZHGKXSLC":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_CDDMSXZGXSCS":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_QDDJEDGL":
                        message += VerifyFloat(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_QDDJFZNJ":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_CQPBCGZYL":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_CQPRJ":
                        message += VerifyInt(name, Convert.ToString(r["CSZ"]));
                        break;
                    case "RLDC_DLXDCZZL":
                        message += VerifyDlxdczzl(Convert.ToString(r["CSZ"]));
                        break;
                    default: break;
                }
                if (code != "RLDC_ZHGKHQL" && code != "RLDC_ZHGKXSLC" && code != "RLDC_CDDMSXZGXSCS")
                {
                    message += DataVerifyHelper.VerifyRequired(name, Convert.ToString(r["CSZ"]));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return message;
        }

        // 验证不为空
        public static string VerifyRequired(string strName, string value)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(value.Trim()))
            {
                msg = String.Format("\n{0}不能为空!", strName);
            }
            return msg;
        }

        // 验证字符长度
        public static string VerifyStrLen(string strName, string value, int expectedLen)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > expectedLen)
                {
                    msg = String.Format("\n{0}长度过长，最长为{1}位!", strName, expectedLen);
                }
            }
            return msg;
        }

        // 验证整型
        public static string VerifyInt(string strName, string value)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value.ToString(), "^[0-9]*$"))
            {
                msg = String.Format("\n{0}应为整数!", strName);
            }
            return msg;
        }

        // 验证浮点型1位小数
        public static string VerifyFloat(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{1}$"))
            {
                msg = String.Format("\n{0}应保留1位小数!", strName);
            }
            return msg;
        }

        // 验证浮点型两位小数
        public static string VerifyFloat2(string strName, string value)
        {
            string msg = string.Empty;
            // 保留一位小数
            if (!string.IsNullOrEmpty(value) && !Regex.IsMatch(value, @"(\d){1,}\.\d{2}$"))
            {
                msg = String.Format("\n{0}应保留2位小数!", strName);
            }
            return msg;
        }

        // 验证时间类型
        public static string VerifyDateTime(string strName, DateTime value)
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
                msg = String.Format("\n{0}应为时间类型!", strName);
            }
            return msg;
        }


        // 验证时间类型
        public static string VerifyDateTime(string strName, string value)
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
                msg = String.Format("\n{0}应为时间类型!", strName);
            }
            return msg;
        }

        // 参数格式验证，多个数值以参数c隔开，中间不能有空格
        public static bool VerifyParamFormat(string value, char c)
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

    }

}
