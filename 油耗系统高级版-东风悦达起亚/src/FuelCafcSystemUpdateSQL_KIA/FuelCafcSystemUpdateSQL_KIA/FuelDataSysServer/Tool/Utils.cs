using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Common;
using System.Data;
using System.Reflection;
using FuelDataModel;
using Oracle.ManagedDataAccess.Client;

namespace FuelDataSysServer.Tool
{
    public class Utils
    {
        //全局变量，供系统整个运行周期调用
        public static bool isFuelTest = FuelDataSysServer.Properties.Settings.Default.IsFuelTest;
        public static int timeCons = FuelDataSysServer.Properties.Settings.Default.TimeConstrain;
        public static int timeInter = FuelDataSysServer.Properties.Settings.Default.TimeInterval;
        public static string userId = FuelDataSysServer.Properties.Settings.Default.UserId;
        public static string userPWD = FuelDataSysServer.Properties.Settings.Default.UserPWD;
        public static string qymc = FuelDataSysServer.Properties.Settings.Default.Qymc;
        public static string localUserId = FuelDataSysServer.Properties.Settings.Default.LocalUserId;
        public static string localUserPWD = FuelDataSysServer.Properties.Settings.Default.LocalUserPWD;

        //上报接口，控制线路，控制代理
        public static FuelDataService.FuelDataSysWebService service = new FuelDataService.FuelDataSysWebService();
        public static FuelDataService.FuelDataSysWebService GetLoginService()
        {
            try
            {
                if (FuelDataSysServer.Properties.Settings.Default.IsProxy) //代理设置
                {
                    WebProxy Proxy = new WebProxy(string.Format("http://{0}:{1}",FuelDataSysServer.Properties.Settings.Default.ProxyAddr,FuelDataSysServer.Properties.Settings.Default.ProxyPort), true);
                    Proxy.Credentials = new NetworkCredential(FuelDataSysServer.Properties.Settings.Default.ProxyUserId,FuelDataSysServer.Properties.Settings.Default.ProxyPwd, "");
                    service.Proxy = Proxy;
                }
                if (isFuelTest)  //正式线路
                {
                    service.Url = SelectLine.FormalStandardLine;
                }
                else             //测试线路
                {
                    service.Url = SelectLine.TestStandardLine;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return service;
        }

        public Utils()
        {
            Utils.GetLoginService();
        }

        // 获取上报信息
        public static List<VehicleBasicInfo> GetApplyParam(List<string> vinList)
        {
            List<VehicleBasicInfo> vehicleBasicInfoList = new List<VehicleBasicInfo>();
            List<RllxParamEntity> rllxEntityList = new List<RllxParamEntity>();
            string vinStr = string.Empty;

            if (vinList == null || vinList.Count < 1)
            {
                return null;
            }
            else
            {
                vinStr = string.Join(",", vinList.ToArray());
            }

            string sqlBasic = string.Format(@"SELECT V_ID,VIN,HGSPBM,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,
                                                ZJ,CLZZRQ,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,
                                                STATUS,JYJGMC,JYBGBH,QTXX
                                              FROM FC_CLJBXX WHERE VIN IN ({0})", vinStr);

            string sqlEntity = string.Format(@"SELECT V_ID,VIN,PARAM_CODE,PARAM_VALUE FROM RLLX_PARAM_ENTITY
                                              WHERE VIN IN ({0})", vinStr);
            try
            {
                vehicleBasicInfoList = Utils.DataTable2Object<VehicleBasicInfo>(OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlBasic, null).Tables[0]);
                rllxEntityList = Utils.DataTable2Object<RllxParamEntity>(OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlEntity, null).Tables[0]);

                foreach (VehicleBasicInfo basicInfo in vehicleBasicInfoList)
                {
                    List<RllxParamEntity> tempEntityList = new List<RllxParamEntity>();
                    foreach (RllxParamEntity entityInfo in rllxEntityList)
                    {
                        if (basicInfo.Vin == entityInfo.Vin)
                        {
                            tempEntityList.Add(entityInfo);
                        }
                    }
                    if (basicInfo.EntityList == null)
                    {
                        basicInfo.EntityList = new RllxParamEntity[tempEntityList.Count];
                    }
                    basicInfo.EntityList = tempEntityList.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("获取上报信息发送异常：" + ex.Message);
            }
            return vehicleBasicInfoList;
        }

        // 将datatable对象转换为T对象
        public static List<T> DataTable2Object<T>(DataTable table) where T : new()
        {
            List<T> result = new List<T>();
            PropertyInfo[] property = null;

            foreach (System.Data.DataRow r in table.Rows)
            {
                T obj = new T();
                if (property == null)
                {
                    property = obj.GetType().GetProperties();
                }

                foreach (PropertyInfo pr in property)
                {
                    try
                    {
                        if (table.Columns[pr.Name] != null)
                        {
                            object tableValue = r[pr.Name];
                            if (tableValue == System.DBNull.Value)
                            {
                                Type type = pr.PropertyType;
                                switch (type.Name)
                                {
                                    case "String":
                                        tableValue = (object)string.Empty;
                                        break;
                                    case "DateTime":
                                        tableValue = (object)DateTime.Today;
                                        break;
                                    case "Boolean":
                                        tableValue = (object)false;
                                        break;
                                    default: break;
                                }
                            }
                            pr.SetValue(obj, tableValue, null);
                        }
                        if (pr.Name.ToUpper() == "V_ID")
                        {
                            pr.SetValue(obj, r["V_ID"] == null ? "" : r["V_ID"].ToString(), null);
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                result.Add(obj);
            }

            return result;
        }

        // 将web service端的VehicleBasicInfo List对象转换为client端对象
        public static List<FuelDataService.VehicleBasicInfo> FuelInfoC2S(List<VehicleBasicInfo> clientBasicList)
        {
            List<FuelDataService.VehicleBasicInfo> serviceBasicList = new List<FuelDataService.VehicleBasicInfo>();

            foreach (VehicleBasicInfo clientBasic in clientBasicList)
            {
                List<FuelDataService.RllxParamEntity> serviceEntityList = new List<FuelDataService.RllxParamEntity>();

                foreach (RllxParamEntity clientEntity in clientBasic.EntityList)
                {
                    FuelDataService.RllxParamEntity serviceEntity = new FuelDataService.RllxParamEntity();
                    ConvertObj<RllxParamEntity, FuelDataService.RllxParamEntity>(clientEntity, serviceEntity);
                    serviceEntityList.Add(serviceEntity);
                }

                FuelDataService.VehicleBasicInfo serviceBasic = new FuelDataService.VehicleBasicInfo();
                Utils.BasicInfoC2W(clientBasic, serviceBasic);

                serviceBasic.EntityList = serviceEntityList.ToArray();
                serviceBasicList.Add(serviceBasic);
            }

            return serviceBasicList;
        }

        // 将结果集NameValue对象转换为Client端对象
        public static OperateResult OperateResultS2C(FuelDataService.OperateResult serviceResult)
        {
            if (serviceResult == null)
            {
                return null;
            }
            OperateResult clientResult = new OperateResult();
            List<NameValuePair> clientPairList = new List<NameValuePair>();

            foreach (FuelDataService.NameValuePair servicePair in serviceResult.ResultDetail)
            {
                NameValuePair clientPair = new NameValuePair();
                ConvertObj<FuelDataService.NameValuePair, NameValuePair>(servicePair, clientPair);
                clientPairList.Add(clientPair);
            }

            Utils.ConvertObj<FuelDataService.OperateResult, OperateResult>(serviceResult, clientResult);
            clientResult.ResultDetail = clientPairList.ToArray();

            return clientResult;
        }

        // 将Src类型对象转换为Des类型
        public static void ConvertObj<Src, Des>(Src src, Des des)
        {
            //PropertyInfo[] propertyInfoS = typeof(Src).GetProperties();
            PropertyInfo[] propertyInfoD = typeof(Des).GetProperties();
            foreach (PropertyInfo ppd in propertyInfoD)
            {
                PropertyInfo pps = typeof(Src).GetProperty(ppd.Name);
                if (pps != null)
                {
                    try
                    {
                        ppd.SetValue(des, pps.GetValue(src, null), null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        // 将Client端VehicleBasicInfo类型转换为Service端类型
        public static void BasicInfoC2W(FuelDataModel.VehicleBasicInfo src, FuelDataService.VehicleBasicInfo des)
        {
            Type typeSrc = src.GetType();
            Type typeDes = des.GetType();
            PropertyInfo[] properties = typeDes.GetProperties();
            foreach (PropertyInfo pf in properties)
            {
                PropertyInfo ps = typeSrc.GetProperty(pf.Name);
                if (ps != null)
                {
                    try
                    {
                        pf.SetValue(des, ps.GetValue(src, null), null);
                    }
                    catch
                    {
                    }
                }
            }
        }

        // 获取返回结果成功失败vins
        public static void getOperateResultListVins(List<OperateResult> orList, List<string> vinsSucc, List<NameValuePair> vinsFail, Dictionary<string, string> dSucc)
        {
            foreach (OperateResult or in orList)
            {
                List<NameValuePair> nvpList = or.ResultDetail.ToList();
                foreach (NameValuePair nvp in nvpList)
                {
                    if (nvp.Value.IndexOf("ADC") > -1)
                    {
                        vinsSucc.Add(nvp.Name);
                        dSucc.Add(nvp.Name, nvp.Value);
                    }
                    else
                    {
                        NameValuePair nvpFailed = new NameValuePair() { Name = nvp.Name, Value = nvp.Value };
                        vinsFail.Add(nvpFailed);
                    }
                }
            }
        }

        // 更新状态与反馈码
        public static void setVidStatusForUpload(Dictionary<string, string> dSuccVinVid)
        {
            try
            {
                foreach (KeyValuePair<string, string> d in dSuccVinVid)
                {
                    string sql = String.Format("UPDATE FC_CLJBXX SET V_ID=:VID,STATUS='0',USER_ID=:USER_ID, UPDATETIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') WHERE VIN=:VIN", DateTime.Now);
                    OracleParameter[] param = {
                                            new OracleParameter("VID", d.Value),
                                            new OracleParameter("USER_ID", Utils.localUserId),
                                            new OracleParameter("VIN", d.Key)
                                        };
                    OracleHelper.ExecuteNonQuery(OracleHelper.conn, sql, param);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 查询数据上报的截止日期
        public static DateTime QueryUploadDeadLine(DateTime manufactureDate)
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
                        if (Utils.VerifyHolidays(dt))
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

        // 计算数据上报的截止日期
        public static DateTime QueryUploadLimitLine(DateTime createDate)
        {
            DateTime limitLine = new DateTime();

            try
            {
                int timeInter = Utils.timeInter;
                string strCreateDate = createDate.ToString("yyyy-MM-dd");
                if (!string.IsNullOrEmpty(strCreateDate) && timeInter > 0)
                {
                    // 限制时长的开始计时日期为（制造日+1天）的零点
                    DateTime startDate = Convert.ToDateTime(strCreateDate).AddDays(-1);

                    // 临时截止日期为 开始计时时间+限制时长
                    limitLine = startDate.Subtract(new TimeSpan(timeInter, 0, 0));

                    // 查看 开始计时时间和临时截止日期 之间有无节假日
                    for (DateTime dt = limitLine; dt < startDate; dt = dt.AddDays(1))
                    {
                        if (Utils.VerifyHolidays(dt))
                        {
                            limitLine = limitLine.AddDays(-1);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return limitLine;
        }

        // 验证节假日
        protected static bool VerifyHolidays(DateTime date)
        {
            if (string.IsNullOrEmpty(date.ToString("yyyy-MM-dd")))
            {
                return false;
            }
            try
            {
                return OracleHelper.Exists(OracleHelper.conn, string.Format(@"SELECT count(*) FROM FC_HOLIDAY WHERE HOL_DAYS='{0}'", date.ToString("yyyy-MM-dd")));
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
