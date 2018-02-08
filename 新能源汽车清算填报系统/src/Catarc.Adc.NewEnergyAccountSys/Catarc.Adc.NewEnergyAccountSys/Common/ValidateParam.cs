using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;

namespace Catarc.Adc.NewEnergyAccountSys.Common
{
    public class ValidateParam
    {
        //电子邮件
        public static bool IsEmail(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            return new Regex("^[\\w-]+(\\.[\\w-]+)*@[\\w-]+(\\.[\\w-]+)+$").IsMatch(str);
        }
        //数字
        public static bool IsNumber(string str)
        {
            if (str.Trim().Length == 0)
            {
                return false;
            }
            return new Regex("^[0-9]*$").IsMatch(str);
        }
        //8位数字
        public static bool IsNumberEight(string str)
        {
            if (str.Length!= 8)
            {
                return false;
            }
            return new Regex("^[0-9]*$").IsMatch(str);
        }
        //区号
        public static bool IsArea(string str)
        {
            if (str.Substring(0, 1) != "0" || str.Length > 4 || str.Length < 3)
                return false;
            return new Regex("^[0-9]*$").IsMatch(str);
        }
        //电话号码
        public static bool IsTel(string str)
        {
            if (str.Substring(0, 1) == "0" || str.Substring(0, 1) == "1" || (str.Length > 8 || str.Length < 7))
                return false;
            return new Regex("^[0-9]*$").IsMatch(str);
        }
        //是否为空
        public static bool IsNUll(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        //是否为数字
        public static bool IsNumeric(string str)
        {
            if (!IsNUll(str))
                return false;
            str = str.Trim();
            return new Regex("^-?\\d+$|^(-?\\d+)(\\.\\d+)?$").IsMatch(str);
        }
        //是否为1位小数
        public static bool IsNumericOne(string str)
        {
            if (!IsNUll(str))
                return false;
            str = str.Trim();
            return new Regex("^-?\\d+$|^(-?\\d+)(\\.\\d)?$").IsMatch(str);
        }
        //是否为4位小数
        public static bool IsNumericFour(string str)
        {
            if (!IsNUll(str))
                return false;
            str = str.Trim();
            return new Regex("^-?\\d+$|^(-?\\d+)(\\.\\d{1,4})?$").IsMatch(str);
        }
        //手机号
        public static bool IsPhone(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Substring(0, 1) != "1")
                return false;
            return new Regex("^[0-9]{11}$").IsMatch(value.Trim());
        }
        //vin
        public static bool IsVIN(string value)
        {
            if (value.Length != 17)
                return false;
            return new Regex("^[a-zA-Z0-9]+$").IsMatch(value);
        }
        //车辆型号
        public static bool IsCLXH(string value)
        {
            return new Regex("^[a-zA-Z0-9]+$").IsMatch(value);
        }
        public static bool IsCharNum(string value)
        {
            return new Regex("^[a-zA-Z0-9-]+$").IsMatch(value);
        }

        public static bool IsCharNumLine(string value)
        {
            return new Regex("^[a-zA-Z0-9-]+$").IsMatch(value);
        }
        //是否含有汉字
        public static bool IsModel(string value)
        {
            char[] chArray = value.ToCharArray();
            for (int index = 0; index < chArray.Length; ++index)
            {
                if ((int)chArray[index] >= 19968 && (int)chArray[index] <= 40891)
                    return false;
            }
            return true;
        }
        //是否全部是汉字
        public static bool IsChineseUn(string value)
        {
            if (value.Trim().Length == 0)
            {
                return false;
            }
            char[] chArray = value.ToCharArray();
            for (int index = 0; index < chArray.Length; ++index)
            {
                if ((int)chArray[index] < 19968 || (int)chArray[index] > 40891)
                    return false;
            }
            return true;
        }
        //是否全部为汉字与（）
        public static bool IsUnit(string value)
        {
            char[] chArray = value.ToCharArray();
            for (int index = 0; index < chArray.Length; ++index)
            {
                if ((int)chArray[index] != 65288 && (int)chArray[index] != 65289 && ((int)chArray[index] < 19968 || (int)chArray[index] > 40891))
                    return false;
            }
            return true;
        }
        //是否为车牌
        public static bool IsPlate(string value)
        {
            return new Regex("^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$").IsMatch(value.Trim());
        }

        public static bool IsCLXZ(string value)
        {
            if (value != "营运"&&value != "非营运")
            {
                return false;
            }
            return true;
        }
        public static bool IsCLZL(string value)
        {
            if (value != "插电式混合动力客车" && value != "插电式混合动力乘用车" && value != "纯电动客车" && value != "纯电动乘用车" && value != "纯电动特种车" && value != "燃料电池客车" && value != "燃料电池乘用车" && value != "燃料电池货车")
            {
                return false;
            }
            return true;
        }
        public static bool IsCLYT(string clyt,string value)
        {
            if (clyt == "插电式混合动力客车" || clyt == "纯电动客车"||clyt == "燃料电池客车")
            {
                if (value != "公交"&&value != "通勤"&&value != "旅游"&& value != "公路")
                {
                    return false;
                }
            }
            else if (clyt == "插电式混合动力乘用车" || clyt == "纯电动乘用车" || clyt == "燃料电池乘用车")
            {
                if (value != "公务"&&value != "出租"&&value != "租赁"&&value != "私人")
                {
                    return false;
                }
            }
            else if (clyt == "纯电动特种车" || clyt == "燃料电池货车")
            {
                if (value != "邮政"&&value != "物流"&&value != "环卫"&&value != "工程")
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
        public static string getCLYTMsg(string clyt)
        {
            string msg = "";
            if (clyt == "插电式混合动力客车" || clyt == "纯电动客车" || clyt == "燃料电池客车")
            {
                msg = clyt + "车辆用途应为“公交/通勤/旅游/公路”" + Environment.NewLine; 
            }
            else if (clyt == "插电式混合动力乘用车" || clyt == "纯电动乘用车" || clyt == "燃料电池乘用车")
            {
                msg = clyt + "车辆用途应为“公务/出租/租赁/私人”" + Environment.NewLine; 
            }
            else if (clyt == "纯电动特种车" || clyt == "燃料电池货车")
            {
                msg = clyt + "车辆用途应为“邮政/物流/环卫/工程”" + Environment.NewLine; 
            }
            else
            {
                msg = "车辆性质参数应填写“插电式混合动力客车/插电式混合动力乘用车/纯电动客车/纯电动乘用车/纯电动特种车/燃料电池客车/燃料电池乘用车/燃料电池货车“\n";
            }
            return msg;
        }
        public static bool IsGCSF(string value)
        {
            try
            {
                string sqlAll = String.Format("select count(*) from COUNTY where COUNTY_NAME = '{0}' ", value);
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlAll, null);
                if (Convert.ToInt32(ds.Tables[0].Rows[0][0])==0)
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static string getGCSFMsg()
        {
            string msg = "购车省份参数应填写";
            try
            {
                string sqlAll = String.Format("select * from COUNTY  ");
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlAll, null);
                bool bRet = false;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!bRet)
                    {
                        msg +=  dr["COUNTY_NAME"];
                        bRet = true;
                    }
                    else
                    {
                        msg += "/" + dr["COUNTY_NAME"];
                    }
                    
                }
                msg += "\n";
            }
            catch (System.Exception ex)
            {
              
            }
            return msg;
        }

        public static bool IsGCCS(string gcsf,string value)
        {
            try
            {
                string sqlAll = String.Format("select count(*) from CITY where COUNTY_NAME = '{0}' and CITY_NAME='{1}' ", gcsf, value);
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlAll, null);
                if (Convert.ToInt32(ds.Tables[0].Rows[0][0]) == 0)
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }
            return true;
        }
        public static string getGCCSMsg(string gcsf)
        {
            string msg =gcsf+ "购车城市参数应填写";
            try
            {
                string sqlAll = String.Format("select * from CITY where COUNTY_NAME = '{0}'  ", gcsf);
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlAll, null);
                bool bRet = false;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!bRet)
                    {
                        msg += dr["CITY_NAME"];
                        bRet = true;
                    }
                    else
                    {
                        msg += "/" + dr["CITY_NAME"];
                    }

                }
                msg += Environment.NewLine; 
            }
            catch (System.Exception ex)
            {

            }
            return msg;
        }
        public static bool IsFactory(string value)
        {
            if (string.IsNullOrEmpty(value)||value.IndexOf('(')!= -1||value.IndexOf(")")!= -1)
            {
                return false;
            }
            return true;
        }
        public static bool IsData(string value)
        {
           try
           {
               DateTime dt = Convert.ToDateTime(value);
               if (dt >= DateTime.Now)
               {
                   return false;
               }

           }
           catch (System.Exception ex)
           {
               return false;
           }
           return true;
        }
        public static string CheckParameter(Dictionary<string,string> mapParemeter)
        {
          //  string msg = "";
          //  msg += CheckBaseInfomation(mapParemeter);
          //  msg += CheckConfigInfomation(mapParemeter);
          //  msg += CheckWorkingInfomation(mapParemeter);
          //  return msg;
            string msg = "";
            foreach (var val in mapParemeter)
            {
                switch (val.Key)
                {
                    case "CLXZ":
                        {
                            if (!IsCLXZ(mapParemeter["CLXZ"]))
                            {
                                msg += "车辆性质参数应填写“营运/非营运”" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "CLZL":
                        {
                            if (!IsCLZL(mapParemeter["CLZL"]))
                            {
                                msg += "车辆用途参数应填写“插电式混合动力客车/插电式混合动力乘用车/纯电动客车/纯电动乘用车/纯电动特种车/燃料电池客车/燃料电池乘用车/燃料电池货车“" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "CLYT":
                        {
                            if (mapParemeter.ContainsKey("CLZL"))
                            {
                                if (!IsCLYT(mapParemeter["CLZL"], mapParemeter["CLYT"]))
                                {
                                    msg += getCLYTMsg(mapParemeter["CLZL"]);
                                }
                            }
                            break;
                        }
                    case "GCSF":
                        {
                            if (!IsGCSF(mapParemeter["GCSF"]))
                            {
                                msg += getGCSFMsg();
                            }
                            break;
                        }
                    case "GCCS":
                        {
                            if (mapParemeter.ContainsKey("GCSF"))
                            {
                                if (!IsGCCS(mapParemeter["GCSF"], mapParemeter["GCCS"]))
                                {
                                    msg += getGCCSMsg(mapParemeter["GCSF"]);
                                }
                            }
                            break;
                        }
                    case "VIN":
                        {
                            if (!IsVIN(mapParemeter["VIN"]))
                            {
                                msg += "VIN参数应为17位数字与字母" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "CLXH":
                        {
                            if (string.IsNullOrEmpty(mapParemeter["CLXH"]))
                            {
                                msg += "车辆型号参数应为字符" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "FPHM":
                        {
                            if (!IsNumberEight(mapParemeter["FPHM"]))
                            {
                                msg += "发票号参数应为8位数字" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "GGPC":
                        {
                            if (!IsNumber(mapParemeter["GGPC"]))
                            {
                                msg += "公告批次参数应为数字" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "CLPZ":
                        {
                            if (!IsNUll(mapParemeter["CLPZ"]))
                            {
                                msg += "车辆牌照不可为空" + Environment.NewLine; ;
                            }
                            break;
                        }
                    case "FPSJ":
                        {
                            if (!IsData(mapParemeter["FPSJ"]))
                            {
                                msg += "发票时间参数格式不正确或大于当前时间" + Environment.NewLine;
                            }
                            break;
                        }
                    case "XSZSJ":
                        {
                            if (!IsData(mapParemeter["XSZSJ"]))
                            {
                                msg += "行驶证时间参数格式不正确或大于当前时间" + Environment.NewLine;
                            }
                            break;
                        }
                    case "SQBZBZ":
                        {
                            if (!IsNumericOne(mapParemeter["SQBZBZ"]))
                            {
                                msg += "申请补助标准参数应为数字 小数点后最多1位" + Environment.NewLine;
                            }
                            break;
                        }
                    case "GMJG":
                        {
                            if (!IsNumeric(mapParemeter["GMJG"]))
                            {
                                msg += "购买价格参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "EKGZ":
                        {
                            if (!IsNumericFour(mapParemeter["EKGZ"]))
                            {
                                msg += "Ekg值参数应为数字,保留4位小数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "FPTP":
                        {
                            if (!IsNUll(mapParemeter["FPTP"]))
                            {
                                msg += "发票照片参数不可为空" + Environment.NewLine;
                            }
                            break;
                        }
                    case "XSZTP":
                        {
                            if (!IsNUll(mapParemeter["XSZTP"]))
                            {
                                msg += "行驶证图片参数不可为空" + Environment.NewLine;
                            }
                            break;
                        }
                         case "DCDTXX_XH":
                        {
                            if (!IsNUll(mapParemeter["DCDTXX_XH"]))
                            {
                                msg += "电池单体型号参数应为字符" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCDTXX_SCQY":
                        {
                            if (!IsFactory(mapParemeter["DCDTXX_SCQY"]))
                            {
                                msg += "电池单体生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_XH":
                        {
                            if (!IsNUll(mapParemeter["DCZXX_XH"]))
                            {
                                msg += "电池组型号参数应为字符" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_ZRL":
                        {
                            if (!IsNumeric(mapParemeter["DCZXX_ZRL"]))
                            {
                                msg += "电池组总能量参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_SCQY":
                        {
                            if (!IsFactory(mapParemeter["DCZXX_SCQY"]))
                            {
                                msg += "电池组生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_XTJG":
                        {
                            if (!IsNumericFour(mapParemeter["DCZXX_XTJG"]))
                            {
                                msg += "电池组系统价格参数应为数字,保留4位小数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_ZBNX":
                        {
                            if (!IsNumeric(mapParemeter["DCZXX_ZBNX"]))
                            {
                                msg += "电池组质保年限参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_EDGL_1":
                        {
                            if (!IsNumeric(mapParemeter["QDDJXX_EDGL_1"]))
                            {
                                msg += "驱动电机1额定功率参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_XH_1":
                        {
                            if (!IsNUll(mapParemeter["QDDJXX_XH_1"]))
                            {
                                msg += "驱动电机1型号参数应为字符" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_SCQY_1":
                        {
                            if (!IsFactory(mapParemeter["QDDJXX_SCQY_1"]))
                            {
                                msg += "驱动电机1生产企业参数应为字符（若带括号为中文输入法括号" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_XTJG_1":
                        {
                            if (!IsNumericFour(mapParemeter["QDDJXX_XTJG_1"]))
                            {
                                msg += "驱动电机1系统价格参数应为数字,保留4位小数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "CLSFYQDDJ2":
                        {
                            if (mapParemeter["CLSFYQDDJ2"] != "是" && mapParemeter["CLSFYQDDJ2"] != "否")
                            {
                                msg += "车辆是否有驱动电机2参数应为“是/否”" + Environment.NewLine; 
                            }
                            break;
                        }
                    case "QDDJXX_EDGL_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["QDDJXX_EDGL_2"]))
                                    {
                                        msg += "驱动电机2额定功率参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "QDDJXX_XH_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["QDDJXX_XH_2"]))
                                    {
                                        msg += "驱动电机2型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "QDDJXX_SCQY_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["QDDJXX_SCQY_2"]))
                                    {
                                        msg += "驱动电机2生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "QDDJXX_XTJG_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsNumericFour(mapParemeter["QDDJXX_XTJG_2"]))
                                    {
                                        msg += "驱动电机2系统价格参数应为数字，保留4位小数" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CLSFYCJDR":
                        {
                            if (mapParemeter["CLSFYCJDR"] != "是" && mapParemeter["CLSFYCJDR"] != "否")
                            {
                                msg += "车辆是否有超级电容参数应为“是/否”" + Environment.NewLine; 
                            }
                            break;
                        }
                    case "CJDRXX_CXXH":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["CJDRXX_CXXH"]))
                                    {
                                        msg += "超级电容成箱型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DTSCQY":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["CJDRXX_DTSCQY"]))
                                    {
                                        msg += "超级电容单体生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DRZRL":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["CJDRXX_DRZRL"]))
                                    {
                                        msg += "超级电容电容组容量参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DRZSCQY":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["CJDRXX_DRZSCQY"]))
                                    {
                                        msg += "电容组生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_XTJG":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumericFour(mapParemeter["CJDRXX_XTJG"]))
                                    {
                                        msg += "超级电容系统价格参数应为数字,保留4位小数" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DTXH":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["CJDRXX_DTXH"]))
                                    {
                                        msg += "超级电容单体型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_ZBNX":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["CJDRXX_ZBNX"]))
                                    {
                                        msg += "超级电容质保年限参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CLSFYRLDC":
                        {
                            if (mapParemeter["CLSFYRLDC"] != "是" && mapParemeter["CLSFYRLDC"] != "否")
                            {
                                msg += "车辆是否有燃料电池参数应为“是/否”" + Environment.NewLine; 
                            }
                            break;
                        }
                    case "RLDCXX_ZBNX":
                        {
                            if (mapParemeter.ContainsKey("CLSFYRLDC"))
                            {
                                if (mapParemeter["CLSFYRLDC"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["RLDCXX_ZBNX"]))
                                    {
                                        msg += "燃料电池质保年限参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_GMJG":
                        {
                            if (mapParemeter.ContainsKey("CLSFYRLDC"))
                            {
                                if (mapParemeter["CLSFYRLDC"] == "是")
                                {
                                    if (!IsNumericFour(mapParemeter["RLDCXX_GMJG"]))
                                    {
                                        msg += "燃料电池购买价格参数应为数字,保留4位小数" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_SCQY":
                        {
                            if (mapParemeter.ContainsKey("CLSFYRLDC"))
                            {
                                if (mapParemeter["CLSFYRLDC"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["RLDCXX_SCQY"]))
                                    {
                                        msg += "燃料电池生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_EDGL":
                        {
                            if (mapParemeter.ContainsKey("CLSFYRLDC"))
                            {
                                if (mapParemeter["CLSFYRLDC"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["RLDCXX_EDGL"]))
                                    {
                                        msg += "燃料电池额定功率参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_XH":
                        {
                            if (mapParemeter.ContainsKey("CLSFYRLDC"))
                            {
                                if (mapParemeter["CLSFYRLDC"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["RLDCXX_XH"]))
                                    {
                                        msg += "燃料电池型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "LJXSLC":
                        {
                            if (!IsNumeric(mapParemeter["LJXSLC"]))
                            {
                                msg += "累计行驶里程参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "CLCMYCDNGXSLC":
                        {
                            if (!IsNumeric(mapParemeter["CLCMYCDNGXSLC"]))
                            {
                                msg += "车辆充满一次电能够行驶里程参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJYL":
                        {
                            if (!IsNumber(mapParemeter["LJJYL"]))
                            {
                                msg += "累计加油量参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "CLYCCMDSXSJ":
                        {
                            if (!IsNumeric(mapParemeter["CLYCCMDSXSJ"]))
                            {
                                msg += "车辆一次充满电所需时间参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJQL_G":
                        {
                            if (!IsNumber(mapParemeter["LJJQL_G"]))
                            {
                                msg += "累计加气量（氢除外）(KG)参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "ZDCDGL":
                        {
                            if (!IsNumeric(mapParemeter["ZDCDGL"]))
                            {
                                msg += "最大充电功率参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJQL_L":
                        {
                            if (!IsNumber(mapParemeter["LJJQL_L"]))
                            {
                                msg += "累计加气量（氢除外）(L)参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "YJXSLC":
                        {
                            if (!IsNumeric(mapParemeter["YJXSLC"]))
                            {
                                msg += "月均行驶里程参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJQL":
                        {
                            if (!IsNumber(mapParemeter["LJJQL"]))
                            {
                                msg += "累计加氢量参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "BGLHDL":
                        {
                            if (!IsNumeric(mapParemeter["BGLHDL"]))
                            {
                                msg += "百公里耗电量参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJCDL":
                        {
                            if (!IsNumber(mapParemeter["LJCDL"]))
                            {
                                msg += "累计充电量应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "PJDRXYSJ":
                        {
                            if (!IsNumeric(mapParemeter["PJDRXYSJ"]))
                            {
                                msg += "平均单日运行时间参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "SFAZJKZZ":
                        {
                            if (mapParemeter["SFAZJKZZ"] != "否" && mapParemeter["SFAZJKZZ"] != "是")
                            {
                                msg += "是否安装监控装置参数应为“是/否”" + Environment.NewLine;
                            }
                            break;
                        }
                    case "JKPDXXDW":
                        {
                            if (mapParemeter.ContainsKey("SFAZJKZZ"))
                            {
                                if (mapParemeter["SFAZJKZZ"] == "是")
                                {
                                    //JKPDXXDW  监控平台运行单位
                                    if (!IsNUll(mapParemeter["JKPDXXDW"]))
                                    {
                                        msg += "监控平台运行单位不可以为空" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CLYXDW":
                        {
                            if (mapParemeter.ContainsKey("CLYT"))
                            {
                                if (mapParemeter["CLYT"] != "私人")
                                {
                                    if (!IsFactory(mapParemeter["CLYXDW"]))
                                    {
                                        msg += "车辆运行单位参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    default: break;
                }
            }

            return msg;
        }
        public static string CheckBaseInfomation(Dictionary<string, string> mapParemeter)
        {
             string msg = "";
             foreach (var val in mapParemeter)
             {
                 switch (val.Key)
                 {
                     case "CLXZ":
                         {
                             if (!IsCLXZ(mapParemeter["CLXZ"]))
                             {
                                 msg += "车辆性质参数应填写“营运/非营运”" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "CLZL":
                         {
                             if (!IsCLZL(mapParemeter["CLZL"]))
                             {
                                 msg += "车辆用途参数应填写“插电式混合动力客车/插电式混合动力乘用车/纯电动客车/纯电动乘用车/纯电动特种车/燃料电池客车/燃料电池乘用车/燃料电池货车“" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "CLYT":
                         {
                             if (mapParemeter.ContainsKey("CLZL"))
                             {
                                 if (!IsCLYT(mapParemeter["CLZL"], mapParemeter["CLYT"]))
                                 {
                                     msg += getCLYTMsg(mapParemeter["CLZL"]);
                                 }
                             }
                             break;
                         }
                     case "GCSF":
                         {
                             if (!IsGCSF(mapParemeter["GCSF"]))
                             {
                                 msg += getGCSFMsg();
                             }
                             break;
                         }
                     case "GCCS":
                         {
                             if (mapParemeter.ContainsKey("GCSF"))
                             {
                                 if (!IsGCCS(mapParemeter["GCSF"], mapParemeter["GCCS"]))
                                 {
                                     msg += getGCCSMsg(mapParemeter["GCSF"]);
                                 }
                             }
                             break;
                         }
                     case "VIN":
                         {
                             if (!IsVIN(mapParemeter["VIN"]))
                             {
                                 msg += "VIN参数应为17位数字与字母" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "CLXH":
                         {
                             if (!IsCLXH(mapParemeter["CLXH"]))
                             {
                                 msg += "车辆型号参数应为数字与字母" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "FPHM":
                         {
                             if (!IsNumberEight(mapParemeter["FPHM"]))
                             {
                                 msg += "发票号参数应为8位数字" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "GGPC":
                         {
                             if (!IsNumber(mapParemeter["GGPC"]))
                             {
                                 msg += "公告批次参数应为数字" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "CLPZ":
                         {
                             if (!IsPlate(mapParemeter["CLPZ"]))
                             {
                                 msg += "车辆牌照参数应首字符为汉字" + Environment.NewLine; ;
                             }
                             break;
                         }
                     case "FPSJ":
                         {
                             if (!IsData(mapParemeter["FPSJ"]))
                             {
                                 msg += "发票时间参数格式不正确或大于当前时间" + Environment.NewLine;
                             }
                             break;
                         }
                     case "XSZSJ":
                         {
                             if (!IsData(mapParemeter["XSZSJ"]))
                             {
                                 msg += "行驶证时间参数格式不正确或大于当前时间" + Environment.NewLine;
                             }
                             break;
                         }
                     case "SQBZBZ":
                         {
                             if (!IsNumericOne(mapParemeter["SQBZBZ"]))
                             {
                                 msg += "申请补助标准参数应为数字 小数点后最多1位" + Environment.NewLine;
                             }
                             break;
                         }
                     case "GMJG":
                         {
                             if (!IsNumeric(mapParemeter["GMJG"]))
                             {
                                 msg += "购买价格参数应为数字" + Environment.NewLine;
                             }
                             break;
                         }
                     case "EKGZ":
                         {
                             if (!IsNumericFour(mapParemeter["EKGZ"]))
                             {
                                 msg += "Ekg值参数应为数字,保留4位小数" + Environment.NewLine;
                             }
                             break;
                         }
                     case "FPTP":
                         {
                             if (!IsNUll(mapParemeter["FPTP"]))
                             {
                                 msg += "发票照片参数不可为空" + Environment.NewLine;
                             }
                             break;
                         }
                     case "XSZTP":
                         {
                             if (!IsNUll(mapParemeter["XSZTP"]))
                             {
                                 msg += "行驶证图片参数不可为空" + Environment.NewLine;
                             }
                             break;
                         }
                 }
             }
           
            return msg;
        }
        public static string CheckConfigInfomation(Dictionary<string, string> mapParemeter)
        {
            string msg = "";
            foreach (var val in mapParemeter)
            {
                switch (val.Key)
                {
                    case "DCDTXX_XH":
                        {
                            if (!IsNUll(mapParemeter["DCDTXX_XH"]))
                            {
                                msg += "电池单体型号参数应为字符" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCDTXX_SCQY":
                        {
                            if (!IsFactory(mapParemeter["DCDTXX_SCQY"]))
                            {
                                msg += "电池单体生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_XH":
                        {
                            if (!IsNUll(mapParemeter["DCZXX_XH"]))
                            {
                                msg += "电池组型号参数应为字符" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_ZRL":
                        {
                            if (!IsNumeric(mapParemeter["DCZXX_ZRL"]))
                            {
                                msg += "电池组总能量参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_SCQY":
                        {
                            if (!IsFactory(mapParemeter["DCZXX_SCQY"]))
                            {
                                msg += "电池组生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_XTJG":
                        {
                            if (!IsNumericFour(mapParemeter["DCZXX_XTJG"]))
                            {
                                msg += "电池组系统价格参数应为数字,保留4位小数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "DCZXX_ZBNX":
                        {
                            if (!IsNumeric(mapParemeter["DCZXX_ZBNX"]))
                            {
                                msg += "电池组质保年限参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_EDGL_1":
                        {
                            if (!IsNumeric(mapParemeter["QDDJXX_EDGL_1"]))
                            {
                                msg += "驱动电机1额定功率参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_XH_1":
                        {
                            if (!IsNUll(mapParemeter["QDDJXX_XH_1"]))
                            {
                                msg += "驱动电机1型号参数应为字符" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_SCQY_1":
                        {
                            if (!IsFactory(mapParemeter["QDDJXX_SCQY_1"]))
                            {
                                msg += "驱动电机1生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                            }
                            break;
                        }
                    case "QDDJXX_XTJG_1":
                        {
                            if (!IsNumericFour(mapParemeter["QDDJXX_XTJG_1"]))
                            {
                                msg += "驱动电机1系统价格参数应为数字,保留4位小数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "CLSFYQDDJ2":
                        {
                            if (mapParemeter["CLSFYQDDJ2"] != "是" && mapParemeter["CLSFYQDDJ2"] != "否")
                            {
                                msg += "车辆是否有驱动电机2参数应为“是/否”" + Environment.NewLine; 
                            }
                            break;
                        }
                    case "QDDJXX_EDGL_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["QDDJXX_EDGL_2"]))
                                    {
                                        msg += "驱动电机2额定功率参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "QDDJXX_XH_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["QDDJXX_XH_2"]))
                                    {
                                        msg += "驱动电机2型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "QDDJXX_SCQY_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["QDDJXX_SCQY_2"]))
                                    {
                                        msg += "驱动电机2生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "QDDJXX_XTJG_2":
                        {
                            if (mapParemeter.ContainsKey("CLSFYQDDJ2"))
                            {
                                if (mapParemeter["CLSFYQDDJ2"] == "是")
                                {
                                    if (!IsNumericFour(mapParemeter["QDDJXX_XTJG_2"]))
                                    {
                                        msg += "驱动电机2系统价格参数应为数字，保留4位小数" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CLSFYCJDR":
                        {
                            if (mapParemeter["CLSFYCJDR"] != "是" && mapParemeter["CLSFYCJDR"] != "否")
                            {
                                msg += "车辆是否有超级电容参数应为“是/否”" + Environment.NewLine; 
                            }
                            break;
                        }
                    case "CJDRXX_CXXH":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["CJDRXX_CXXH"]))
                                    {
                                        msg += "超级电容成箱型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DTSCQY":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["CJDRXX_DTSCQY"]))
                                    {
                                        msg += "超级电容单体生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DRZRL":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["CJDRXX_DRZRL"]))
                                    {
                                        msg += "超级电容电容组容量参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DRZSCQY":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["CJDRXX_DRZSCQY"]))
                                    {
                                        msg += "电容组生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_XTJG":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumericFour(mapParemeter["CJDRXX_XTJG"]))
                                    {
                                        msg += "超级电容系统价格参数应为数字,保留4位小数" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_DTXH":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["CJDRXX_DTXH"]))
                                    {
                                        msg += "超级电容单体型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CJDRXX_ZBNX":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["CJDRXX_ZBNX"]))
                                    {
                                        msg += "超级电容质保年限参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CLSFYRLDC":
                        {
                            if (mapParemeter["CLSFYRLDC"] != "是" && mapParemeter["CLSFYRLDC"] != "否")
                            {
                                msg += "车辆是否有燃料电池参数应为“是/否”" + Environment.NewLine; 
                            }
                            break;
                        }
                    case "RLDCXX_ZBNX":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["RLDCXX_ZBNX"]))
                                    {
                                        msg += "燃料电池质保年限参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_GMJG":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumericFour(mapParemeter["RLDCXX_GMJG"]))
                                    {
                                        msg += "燃料电池购买价格参数应为数字,保留4位小数" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_SCQY":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsFactory(mapParemeter["RLDCXX_SCQY"]))
                                    {
                                        msg += "燃料电池生产企业参数应为字符（若带括号为中文输入法括号）" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_EDGL":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNumeric(mapParemeter["RLDCXX_EDGL"]))
                                    {
                                        msg += "燃料电池额定功率参数应为数字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "RLDCXX_XH":
                        {
                            if (mapParemeter.ContainsKey("CLSFYCJDR"))
                            {
                                if (mapParemeter["CLSFYCJDR"] == "是")
                                {
                                    if (!IsNUll(mapParemeter["RLDCXX_XH"]))
                                    {
                                        msg += "燃料电池型号参数应为字符" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                }
            }

            return msg;
        }
        public static string CheckWorkingInfomation(Dictionary<string, string> mapParemeter)
        {
            string msg = "";
            foreach(var val in mapParemeter)
            {
                switch (val.Key)
                {
                    case "LJXSLC":
                        {
                            if (!IsNumeric(mapParemeter["LJXSLC"]))
                            {
                                msg += "累计行驶里程参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "CLCMYCDNGXSLC":
                        {
                            if (!IsNumeric(mapParemeter["CLCMYCDNGXSLC"]))
                            {
                                msg += "车辆充满一次电能够行驶里程参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJYL":
                        {
                            if (!IsNumber(mapParemeter["LJJYL"]))
                            {
                                msg += "累计加油量参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "CLYCCMDSXSJ":
                        {
                            if (!IsNumeric(mapParemeter["CLYCCMDSXSJ"]))
                            {
                                msg += "车辆一次充满电所需时间参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJQL_G":
                        {
                            if (!IsNumber(mapParemeter["LJJQL_G"]))
                            {
                                msg += "累计加气量（氢除外）(KG)参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "ZDCDGL":
                        {
                            if (!IsNumeric(mapParemeter["ZDCDGL"]))
                            {
                                msg += "最大充电功率参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJQL_L":
                        {
                            if (!IsNumber(mapParemeter["LJJQL_L"]))
                            {
                                msg += "累计加气量（氢除外）(L)参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "YJXSLC":
                        {
                            if (!IsNumeric(mapParemeter["YJXSLC"]))
                            {
                                msg += "月均行驶里程参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJJQL":
                        {
                            if (!IsNumber(mapParemeter["LJJQL"]))
                            {
                                msg += "累计加氢量参数应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "BGLHDL":
                        {
                            if (!IsNumeric(mapParemeter["BGLHDL"]))
                            {
                                msg += "百公里耗电量参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "LJCDL":
                        {
                            if (!IsNumber(mapParemeter["LJCDL"]))
                            {
                                msg += "累计充电量应为整数" + Environment.NewLine;
                            }
                            break;
                        }
                    case "PJDRXYSJ":
                        {
                            if (!IsNumeric(mapParemeter["PJDRXYSJ"]))
                            {
                                msg += "平均单日运行时间参数应为数字" + Environment.NewLine;
                            }
                            break;
                        }
                    case "SFAZJKZZ":
                        {
                            if (mapParemeter["SFAZJKZZ"] != "否" && mapParemeter["SFAZJKZZ"] != "是")
                            {
                                msg += "是否安装监控装置参数应为“是/否”" + Environment.NewLine;
                            }
                            break;
                        }
                    case "JKPDXXDW":
                        {
                            if (mapParemeter.ContainsKey("SFAZJKZZ"))
                            {
                                if (mapParemeter["SFAZJKZZ"] == "是")
                                {
                                    //JKPDXXDW  监控平台运行单位
                                    if (!IsNUll(mapParemeter["JKPDXXDW"]))
                                    {
                                        msg += "监控平台运行单位不可以为空" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                    case "CLYXDW":
                        {
                            if (mapParemeter.ContainsKey("CLYT"))
                            {
                                if (mapParemeter["CLYT"] != "私人")
                                {
                                    if (!IsChineseUn(mapParemeter["CLYXDW"]))
                                    {
                                        msg += "车辆运行单位参数应为汉字" + Environment.NewLine;
                                    }
                                }
                            }
                            break;
                        }
                }
            }

            return msg;
        }

        public static string CheckQYParam(Dictionary<string, string> mapParemeter)
        {
            string msg = "";
            msg += CheckQYInfomation(mapParemeter);
            return msg;
        }
        public static string CheckQYInfomation(Dictionary<string, string> mapParemeter)
        {
            string msg = "";
            #region 新能源负责人员
            //姓名
            if (mapParemeter.ContainsKey("NewName"))
            {
                if (!IsChineseUn(mapParemeter["NewName"]) || (mapParemeter["NewName"].Length > 4 || mapParemeter["NewName"].Length < 2))
                {
                    msg += "新能源负责人姓名应为2-4位汉字\n";
                }
            }
            //部门
            if (mapParemeter.ContainsKey("NewDepartment"))
            {
                if (!IsChineseUn(mapParemeter["NewDepartment"]))
                {
                    msg += "新能源负责人部门应为汉字\n";
                }
            }
            //职务
            if (mapParemeter.ContainsKey("NewPost"))
            {
                if (!IsChineseUn(mapParemeter["NewPost"]))
                {
                    msg += "新能源负责人职务应为汉字\n";
                }
            }
            //电话
            if (mapParemeter.ContainsKey("NewMobile1"))
            {
                if (string.IsNullOrEmpty(mapParemeter["NewMobile1"]))
                {
                    msg += "新能源负责人电话区号不能为空\n";
                }
                else
                {
                    if (!IsArea(mapParemeter["NewMobile1"]))
                    {
                        msg += "新能源负责人电话区号应为3-4位数字且以0开头\n";
                    }
                }
            }
            if (mapParemeter.ContainsKey("NewMobile2"))
            {
                if (string.IsNullOrEmpty(mapParemeter["NewMobile2"]))
                {
                    msg += "新能源负责人电话号码不能为空\n";
                }
                else
                {
                    if (!IsTel(mapParemeter["NewMobile2"]))
                    {
                        msg += "新能源负责人电话号码应为7-8位数字，且为非0、1开头\n";
                    }
                }
            }
            //手机
            if (mapParemeter.ContainsKey("NewPhone"))
            {
                if (!IsPhone(mapParemeter["NewPhone"]))
                {
                    msg += "新能源负责人手机号应为11位数字且以1开头\n";
                }
            }
            //邮箱
            if (mapParemeter.ContainsKey("NewMail"))
            {
                if (!IsEmail(mapParemeter["NewMail"]))
                {
                    msg += "新能源负责人邮箱应包含'@'和'，'字符\n";
                }
            }
            #endregion
            #region 主要负责人员
            //姓名
            if (mapParemeter.ContainsKey("MainName"))
            {
                if (!IsChineseUn(mapParemeter["MainName"]) || (mapParemeter["MainName"].Length > 4 || mapParemeter["MainName"].Length < 2))
                {
                    msg += "主要联系人员姓名应为2-4位汉字\n";
                }
            }
            //部门
            if (mapParemeter.ContainsKey("MainDepartment"))
            {
                if (!IsChineseUn(mapParemeter["MainDepartment"]))
                {
                    msg += "主要联系人员部门应为汉字\n";
                }
            }
            //职务
            if (mapParemeter.ContainsKey("MainPost"))
            {
                if (!IsChineseUn(mapParemeter["MainPost"]))
                {
                    msg += "主要联系人员职务应为汉字\n";
                }
            }
            //电话
            if (mapParemeter.ContainsKey("MainMobile1"))
            {
                if (string.IsNullOrEmpty(mapParemeter["MainMobile1"]))
                {
                    msg += "主要联系人员电话区号不能为空\n";
                }
                else
                {
                    if (!IsArea(mapParemeter["MainMobile1"]))
                    {
                        msg += "主要联系人员电话区号应为3-4位数字且以0开头\n";
                    }
                }
            }
            if (mapParemeter.ContainsKey("MainMobile2"))
            {
                if (string.IsNullOrEmpty(mapParemeter["MainMobile2"]))
                {
                    msg += "主要联系人员电话号码不能为空\n";
                }
                else
                {
                    if (!IsTel(mapParemeter["MainMobile2"]))
                    {
                        msg += "主要联系人员电话号码应为7-8位数字，且为非0、1开头\n";
                    }
                }
            }
            //手机
            if (mapParemeter.ContainsKey("MainPhone"))
            {
                if (!IsPhone(mapParemeter["MainPhone"]))
                {
                    msg += "主要联系人员手机号应为11位数字且以1开头\n";
                }
            }
            //邮箱
            if (mapParemeter.ContainsKey("MainMail"))
            {
                if (!IsEmail(mapParemeter["MainMail"]))
                {
                    msg += "主要联系人员邮箱应包含'@'和'，'字符\n";
                }
            }
            #endregion
            return msg;
        }
    }
}
