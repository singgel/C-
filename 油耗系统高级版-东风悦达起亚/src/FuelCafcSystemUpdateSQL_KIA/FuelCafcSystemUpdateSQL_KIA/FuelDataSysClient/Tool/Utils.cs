using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using FuelDataModel;
using DevExpress.XtraGrid.Views.Grid;
using System.Data.OleDb;
using DevExpress.XtraEditors;
using System.Net;
using System.Collections.Specialized;
using System.Configuration;
using Common;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Model;

namespace FuelDataSysClient
{
    public class Utils
    {
        public static string localUserId = "";
        public static string userId = string.IsNullOrEmpty(FuelDataSysClient.Properties.Settings.Default.UserId) ? "" : FuelDataSysClient.Properties.Settings.Default.UserId;
        public static string password = string.IsNullOrEmpty(FuelDataSysClient.Properties.Settings.Default.UserPWD) ? "" : FuelDataSysClient.Properties.Settings.Default.UserPWD;
        public static string qymc = string.IsNullOrEmpty(FuelDataSysClient.Properties.Settings.Default.Qymc) ? "" : FuelDataSysClient.Properties.Settings.Default.Qymc;
        // 上传数据限制时长
        public static int timeCons = FuelDataSysClient.Properties.Settings.Default.TimeConstrain == 0 ? 48 : FuelDataSysClient.Properties.Settings.Default.TimeConstrain;
        public static List<PrintModel> printModel = new List<PrintModel>();
        private static bool isFuelTest;

        public static bool IsFuelTest
        {
            get { return Utils.isFuelTest; }
            set { Utils.isFuelTest = value; }
        }
        public static CertificateService.CertificateComparison serviceCertificate = new CertificateService.CertificateComparison();
        public static CafcService.CafcWebService serviceCafc = new CafcService.CafcWebService();
        public static FuelDataService.FuelDataSysWebService service = new FuelDataService.FuelDataSysWebService();
        public static FuelFileUpload.FileUploadService serviceFiel = new FuelFileUpload.FileUploadService();

        public static int timeInter = FuelDataSysClient.Properties.Settings.Default.TimeInterval;

        public static FuelDataService.FuelDataSysWebService GetLoginService()
        {
            FuelDataModel.Utils utils = new FuelDataModel.Utils();
            //service = 
            try
            {
                if (FuelDataSysClient.Properties.Settings.Default.IsProxy) //代理设置
                {
                    WebProxy Proxy = new WebProxy(string.Format("http://{0}:{1}",
                        FuelDataSysClient.Properties.Settings.Default.ProxyAddr,
                        FuelDataSysClient.Properties.Settings.Default.ProxyPort), true);
                    Proxy.Credentials = new NetworkCredential(FuelDataSysClient.Properties.Settings.Default.ProxyUserId,
                        FuelDataSysClient.Properties.Settings.Default.ProxyPwd, "");
                    service.Proxy = Proxy;
                    serviceCertificate.Proxy = Proxy;
                    serviceCafc.Proxy = Proxy;
                    serviceFiel.Proxy = Proxy;
                }
                if (IsFuelTest)  //正式线路
                {
                    service.Url = SelectLine.FormalStandardLine;
                    serviceCertificate.Url = SelectLine.FormalCertificatedLine;
                    serviceCafc.Url = SelectLine.FormalCafcLine;
                    serviceFiel.Url = SelectLine.FormalFielLine;
                }
                else             //测试线路
                {
                    service.Url = SelectLine.TestStandardLine;
                    serviceCertificate.Url = SelectLine.TestCertificatedLine;
                    serviceCafc.Url = SelectLine.TestCafcLine;
                    serviceFiel.Url = SelectLine.TestFielLine;
                }
            }
            catch (Exception)
            {
            }
            return service;
        }

        public Utils()
        {
            Utils.GetLoginService();
        }

        public static bool CheckUser()
        {
            if (string.IsNullOrEmpty(Utils.userId))
            {
                MessageBox.Show("请先设置用户名和密码:主界面--工具--设置--用户名密码设置");
                return false;
            }
            return true;
        }

        // 获取上报限制时长
        public static int GetUploadTimeConstrain()
        {
            int timeCons = 0;
            try
            {
                if (service == null)
                {
                    service = Utils.GetLoginService();
                }
                if (!string.IsNullOrEmpty(Utils.userId))
                {
                    timeCons = service.QueryUploadTimeConstrain(Utils.userId, Utils.password);
                }
                else
                {
                    timeCons = -1;
                }
            }
            catch (Exception)
            {
                timeCons = -1;
            }
            return timeCons;
        }

        // 获取燃料种类名称
        public static List<string> GetFuelRLLX(string type)
        {
            List<string> fuelTypeList = new List<string>();
            try
            {
                DataTable dt = OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT DISTINCT FUEL_TYPE,SUBSTR(ORDER_RULE,1,1) FROM RLLX_PARAM WHERE STATUS='1' ORDER BY SUBSTR(ORDER_RULE,1,1)", null).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    // 界面模糊搜索用“SEARCH”，否则为填报界面选择燃料类型
                    if (type == "SEARCH")
                    {
                        fuelTypeList.Add("");
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["FUEL_TYPE"] != null)
                        {
                            fuelTypeList.Add(dr["FUEL_TYPE"].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return fuelTypeList;
        }

        // 获取燃料类型名称
        public static List<string> GetFuelType(string type)
        {
            List<string> fuelTypeList = new List<string>();
            try
            {
                DataTable dt = OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT DISTINCT FUEL_TYPE,SUBSTR(ORDER_RULE,1,1) FROM RLLX_PARAM WHERE STATUS='1' ORDER BY SUBSTR(ORDER_RULE,1,1)", null).Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    // 界面模糊搜索用“SEARCH”，否则为填报界面选择燃料类型
                    if (type == "SEARCH")
                    {
                        fuelTypeList.Add("");
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["FUEL_TYPE"] != null)
                        {
                            if (dr["FUEL_TYPE"].ToString() == "传统能源")
                            {
                                fuelTypeList.Add("汽油");
                                fuelTypeList.Add("柴油");
                                fuelTypeList.Add("两用燃料");
                                fuelTypeList.Add("双燃料");
                            }
                            else
                            {
                                fuelTypeList.Add(dr["FUEL_TYPE"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return fuelTypeList;
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

        // 将client端的VehicleBasicInfo array对象转换为web service端对象
        public static List<VehicleBasicInfo> FuelInfoS2C(FuelDataService.VehicleBasicInfo[] serviceBasicList)
        {
            List<VehicleBasicInfo> clientBasicList = new List<VehicleBasicInfo>();

            foreach (FuelDataService.VehicleBasicInfo serviceBasic in serviceBasicList)
            {
                List<RllxParamEntity> clientEntityList = new List<RllxParamEntity>();

                foreach (FuelDataService.RllxParamEntity serviceEntity in serviceBasic.EntityList)
                {
                    RllxParamEntity clientEntity = new RllxParamEntity();
                    ConvertObj<FuelDataService.RllxParamEntity, RllxParamEntity>(serviceEntity, clientEntity);
                    clientEntityList.Add(clientEntity);
                }

                VehicleBasicInfo clientBasic = new VehicleBasicInfo();
                Utils.BasicInfoW2C(serviceBasic, clientBasic);

                clientBasic.EntityList = clientEntityList.ToArray();
                clientBasicList.Add(clientBasic);
            }

            return clientBasicList;
        }

        // 将参数基本信息和燃料参数合并为一个DataTable
        public static DataTable MergeDataTable(DataTable dtBasic, DataTable dtParams)
        {
            DataTable dtExport = new DataTable();

            dtExport = dtBasic.Copy();
            for (int i = 0; i < dtExport.Columns.Count; )
            {
                if ("VIN,MAIN_ID,CLZZRQ,CLXH,CLZL,RLLX".IndexOf(dtExport.Columns[i].ColumnName) < 0)
                {
                    dtExport.Columns.RemoveAt(i);
                    continue;
                }
                i++;
            }
            dtExport.Columns.Add("SQGKRLXHL");
            dtExport.Columns.Add("SJGKRLXHL");
            dtExport.Columns.Add("ZHGKRLXHL");

            foreach (DataRow drExport in dtExport.Rows)
            {
                DataRow[] drParamArr = dtParams.Select(string.Format("VIN='{0}'", drExport["VIN"].ToString()));

                foreach (DataRow drParam in drParamArr)
                {
                    if (drParam["PARAM_CODE"].ToString().Contains("SQGKRLXHL"))
                    {
                        drExport["SQGKRLXHL"] = Double.Parse(drParam["PARAM_VALUE"].ToString());
                    }

                    if (drParam["PARAM_CODE"].ToString().Contains("SJGKRLXHL"))
                    {
                        drExport["SJGKRLXHL"] = Double.Parse(drParam["PARAM_VALUE"].ToString());
                    }

                    if (drParam["PARAM_CODE"].ToString().Contains("ZHGKRLXHL"))
                    {
                        drExport["ZHGKRLXHL"] = Double.Parse(drParam["PARAM_VALUE"].ToString());
                    }
                }
            }

            return dtExport;
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

        public static List<RllxParam> RllxParamS2C(FuelDataService.RllxParam[] serviceParamList)
        {
            List<RllxParam> clientParamList = new List<RllxParam>();

            foreach (FuelDataService.RllxParam serviceParam in serviceParamList)
            {
                RllxParam clientParam = new RllxParam();
                Utils.ConvertObj<FuelDataService.RllxParam, RllxParam>(serviceParam, clientParam);
                clientParamList.Add(clientParam);
            }

            return clientParamList;
        }

        public static void SelectItem(object obj, bool flg)
        {
            GridView dgv = obj as GridView;
            DataView dv = (DataView)dgv.DataSource;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    dv.Table.Rows[i]["check"] = flg;
                }
                dgv.RefreshData();
            }
        }

        public static void SelectObjItem(object obj, bool flg)
        {
            GridView dgv = obj as GridView;
            List<VehicleBasicInfo> dv = (List<VehicleBasicInfo>)dgv.DataSource;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    VehicleBasicInfo vInfo = dv[i];
                    vInfo.Check = flg;
                }
                dgv.PostEditor();
                dgv.RefreshData();
            }
        }

       //计算数据上报的截止日期
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

        // 比较节假日数据
        public static bool CompareHoliday()
        {
            try
            {
                DataSet dsServer = Utils.service.QueryHolidayData(Utils.userId, Utils.password);
                DataSet dsLocal = OracleHelper.ExecuteDataSet(OracleHelper.conn, @"SELECT HOL_DAYS FROM FC_HOLIDAY ORDER BY HOL_DAYS DESC", null);
                if (dsServer.Tables[0].Rows.Count > 0 && dsLocal.Tables[0].Rows.Count > 0)
                {
                    string holServer = dsServer.Tables[0].Rows[0]["HOL_DAYS"] == null ? "" : dsServer.Tables[0].Rows[0]["HOL_DAYS"].ToString();
                    string holLocal = dsLocal.Tables[0].Rows[0]["HOL_DAYS"] == null ? "" : dsLocal.Tables[0].Rows[0]["HOL_DAYS"].ToString();

                    if (string.Compare(holServer, holLocal) > 0)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        // 自动同步节假日信息
        public static bool AutoSyncHolidays()
        {
            if (string.IsNullOrEmpty(Utils.userId))
            {
                return false;
            }
            try
            {
                using (DataSet dsHolliday = service.QueryHolidayData(Utils.userId, Utils.password))
                {
                    if (dsHolliday != null)
                    {
                        using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                        {
                            con.Open();
                            OracleTransaction trans = con.BeginTransaction();
                            try
                            {
                                OracleHelper.ExecuteNonQuery(trans, @"DELETE FROM FC_HOLIDAY", null);
                                foreach (DataRow dr in dsHolliday.Tables[0].Rows)
                                {
                                    OracleParameter[] holidayList = { new OracleParameter("HOL_DAYS", dr["HOL_DAYS"]) };
                                    OracleHelper.ExecuteNonQuery(trans, @"INSERT INTO FC_HOLIDAY (HOL_DAYS) 
                                                VALUES(:HOL_DAYS)", holidayList);
                                }
                                trans.Commit();
                                return true;
                            }
                            catch (Exception)
                            {
                                trans.Rollback();
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CompareParamVersion()
        {
            try
            {
                Double serviceVersion = Convert.ToDouble(Utils.service.QueryParamVersion(Utils.userId, Utils.password).Trim());
                Double clientVersion = Convert.ToDouble(OracleHelper.ExecuteScalar(OracleHelper.conn, @"SELECT PARAM_REMARK FROM RLLX_PARAM RP WHERE PARAM_CODE='PARAM_VERSION'"));
                if (clientVersion < serviceVersion)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        // 同步新版本的燃料参数
        public static void SyncParam()
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            List<RllxParam> clientParamList;
            try
            {
                FuelDataService.RllxParam[] serviceParamList = service.QueryRllxParamData(Utils.userId, Utils.password);
                clientParamList = Utils.RllxParamS2C(serviceParamList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("同步燃料数据时出现错误: " + ex.Message);
                return;
            }

            if (clientParamList != null)
            {
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    OracleTransaction trans = con.BeginTransaction();
                    try
                    {
                        OracleHelper.ExecuteNonQuery(trans, @"DELETE FROM RLLX_PARAM", null);
                        foreach (RllxParam clientParam in clientParamList)
                        {
                            OracleParameter[] paramList = { 
                                    new OracleParameter("PARAM_CODE",clientParam.Param_Code),
                                    new OracleParameter("PARAM_NAME",clientParam.Param_Name),
                                    new OracleParameter("FUEL_TYPE",clientParam.Fuel_Type),
                                    new OracleParameter("PARAM_REMARK",string.IsNullOrEmpty(clientParam.Param_Remark)?"":clientParam.Param_Remark),
                                    new OracleParameter("CONTROL_TYPE",string.IsNullOrEmpty(clientParam.Control_Type)?"":clientParam.Control_Type),
                                    new OracleParameter("CONTROL_VALUE",string.IsNullOrEmpty(clientParam.Control_Value)?"":clientParam.Control_Value),
                                    new OracleParameter("STATUS",string.IsNullOrEmpty(clientParam.Status)?"":clientParam.Status),
                                    new OracleParameter("ORDER_RULE",string.IsNullOrEmpty(clientParam.Order_Rule)?"":clientParam.Order_Rule)
                                };
                            OracleHelper.ExecuteNonQuery(trans, @"INSERT INTO RLLX_PARAM (PARAM_CODE,PARAM_NAME,FUEL_TYPE,PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE,STATUS,ORDER_RULE) 
                                VALUES(:PARAM_CODE,:PARAM_NAME,:FUEL_TYPE,:PARAM_REMARK,:CONTROL_TYPE,:CONTROL_VALUE,:STATUS,:ORDER_RULE)", paramList);
                        }
                        trans.Commit();

                        MessageBox.Show("同步成功");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("同步燃料数据时出现错误: " + ex.Message);
                    }
                } 
            }
            else
            {
                MessageBox.Show("查询服务器时错误");
            }
        }

        // 同步节假日信息
        public static void SyncHolidays()
        {
            if (!Utils.CheckUser())
            {
                return;
            }
            try
            {
                using (DataSet dsHolliday = service.QueryHolidayData(Utils.userId, Utils.password))
                {
                    if (dsHolliday != null)
                    {
                        using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                        {
                            con.Open();
                            OracleTransaction trans = con.BeginTransaction();
                            try
                            {
                                OracleHelper.ExecuteNonQuery(trans, @"DELETE FROM FC_HOLIDAY", null);
                                foreach (DataRow dr in dsHolliday.Tables[0].Rows)
                                {
                                    OracleParameter[] holidayList = { new OracleParameter("HOL_DAYS", dr["HOL_DAYS"]) };
                                    OracleHelper.ExecuteNonQuery(trans, @"INSERT INTO FC_HOLIDAY (HOL_DAYS) 
                                                    VALUES(:HOL_DAYS)", holidayList);
                                }
                                trans.Commit();
                                MessageBox.Show("同步节假日数据成功");
                            }
                            catch (Exception ex)
                            {
                                trans.Rollback();
                                MessageBox.Show("同步节假日数据时出现错误: " + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询服务器节假日数据时出现错误: " + ex.Message);
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


        #region Tools

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

        // 将Service端VehicleBasicInfo类型转换为Client端类型
        public static void BasicInfoW2C(FuelDataService.VehicleBasicInfo src, FuelDataModel.VehicleBasicInfo des)
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

        public static void SetFormMid(Form form)
        {
            form.SetBounds((Screen.GetBounds(form).Width / 2) - (form.Width / 2), (Screen.GetBounds(form).Height / 2) - (form.Height / 2), form.Width, form.Height, BoundsSpecified.Location);
        }

        // 修改状态,返回VID.
        public static void UpdataState(FuelDataService.OperateResult result)
        {
            OperateResult oResult = Utils.OperateResultS2C(result);
            List<NameValuePair> nvpList = new List<NameValuePair>();
            for (int i = 0; i < oResult.ResultDetail.Length; i++)
            {
                NameValuePair nvp = oResult.ResultDetail[i] as NameValuePair;
                nvpList.Add(nvp);
            }
            if (nvpList != null)
            {
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    OracleTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
                    try
                    {
                        foreach (NameValuePair nvp in nvpList)
                        {
                            if (nvp.Value.IndexOf("VAD") != -1)
                            {
                                OracleHelper.ExecuteNonQuery(tra, String.Format("UPDATE FC_CLJBXX SET V_ID = '{0}',STATUS = '0',UPDATETIME=to_date('{1}','yyyy-mm-dd hh24:mi:ss')  WHERE VIN ='{2}'", nvp.Value, DateTime.Now, nvp.Name), null);
                            }
                        }
                        tra.Commit();
                    }
                    catch
                    {
                        tra.Rollback();
                    }
                }
            }
        }

        // 返回上报错误项
        public static List<NameValuePair> ApplyFlg(FuelDataService.OperateResult result)
        {
            List<NameValuePair> returnList = new List<NameValuePair>();

            try
            {
                OperateResult oResult = Utils.OperateResultS2C(result);
                for (int i = 0; i < oResult.ResultDetail.Length; i++)
                {
                    NameValuePair nvp = oResult.ResultDetail[i] as NameValuePair;
                    if (nvp.Value.IndexOf("VAD") == -1)
                    {
                        returnList.Add(nvp);
                    }
                }
            }
            catch (Exception)
            {
            }
            return returnList;
        }

        #endregion

        #region common business methods

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

        /// <summary>
        /// 更新状态与反馈码
        /// </summary>
        /// <param name="dSuccVinVid"></param>
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

        #endregion

        #region 输入验证

        public static string VerifyRLParam(TableLayoutPanel.ControlCollection controls)
        {
            string message = string.Empty;
            string paramCode = string.Empty;
            string paramValue = string.Empty;
            foreach (Control c in controls)
            {
                if (c is TextEdit || c is DevExpress.XtraEditors.ComboBoxEdit)
                {
                    paramCode = c.Name;
                    paramValue = c.Text.Trim();
                    message += Utils.VerifyRLParam(paramCode, paramValue);
                }
            }
            return message;
        }

        public static string VerifyRLParam(string paramCode, string paramValue)
        {
            string message = string.Empty;
            switch (paramCode)
            {
                // 传统能源
                case "CT_FDJXH":
                    if (string.IsNullOrEmpty(paramValue.Trim()))
                    {
                        message += "\n发动机型号不能为空!";
                    }
                    break;
                case "CT_PL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n排量应为整数!";
                    }
                    break;
                case "CT_EDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n额定功率应保留1位小数!";
                    }
                    break;
                case "CT_JGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n最大净功率应保留1位小数!";
                    }
                    break;
                case "CT_SJGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n市郊工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "CT_SQGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n市区工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "CT_ZHGKCO2PFL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况CO2排放量应为整数!";
                    }
                    break;
                case "CT_ZHGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n综合工况燃料消耗量应保留1位小数!";
                    }
                    break;

                // 纯电动
                case "CDD_DLXDCBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池组比能量应为整数!";
                    }
                    break;
                case "CDD_DLXDCZEDNL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n动力蓄电池组总能量应保留1位小数!";
                    }
                    break;
                case "CDD_DDXDCZZLYZCZBZLDBZ":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n动力蓄电池总质量与整车整备质量的比值应保留1位小数!";
                    }
                    break;
                case "CDD_DLXDCZBCDY":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池组标称电压应为整数!";
                    }
                    break;
                case "CDD_DDQC30FZZGCS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n电动汽车30分钟最高车速应为整数!";
                    }
                    break;
                case "CDD_ZHGKXSLC":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况续驶里程应为整数!";
                    }
                    break;
                case "CDD_QDDJFZNJ":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n驱动电机峰值扭矩应为整数!";
                    }
                    break;
                case "CDD_QDDJEDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n驱动电机额定功率应保留1位小数!";
                    }
                    break;
                case "CDD_ZHGKDNXHL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况电能消耗量应为整数!";
                    }
                    break;

                // 非插电式混合动力
                case "FCDS_HHDL_DLXDCBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池组比能量应为整数!";
                    }
                    break;
                case "FCDS_HHDL_DLXDCZZNL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n动力蓄电池组总能量应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_ZHGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n综合工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_EDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n额定功率应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_JGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n最大净功率应为保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_PL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n排量应为整数!";
                    }
                    break;
                case "FCDS_HHDL_ZHKGCO2PL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况CO2排放应为整数!";
                    }
                    break;
                case "FCDS_HHDL_DLXDCZBCDY":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池组标称电压应为整数!";
                    }
                    break;
                case "FCDS_HHDL_SJGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n市郊工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_SQGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n市区工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_CDDMSXZGCS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n纯电动模式下1km最高车速应为整数!";
                    }
                    break;
                case "FCDS_HHDL_CDDMSXZHGKXSLC":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n纯电动模式下综合工况续驶里程应为整数!";
                    }
                    break;
                case "FCDS_HHDL_QDDJFZNJ":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n驱动电机峰值扭矩应为整数!";
                    }
                    break;
                case "FCDS_HHDL_QDDJEDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n驱动电机额定功率应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_HHDLZDDGLB":
                    if (!Utils.VerifyFloat2(paramValue))
                    {
                        message += "\n混合动力最大电功率比应保留2位小数!";
                    }
                    break;

                // 插电式混合动力
                case "CDS_HHDL_DLXDCBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池组比能量应为整数!";
                    }
                    break;
                case "CDS_HHDL_DLXDCZZNL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n动力蓄电池组总能量应保留1位小数!";
                    }
                    break;
                case "CDS_HHDL_ZHGKRLXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n综合工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "CDS_HHDL_EDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n额定功率应保留1位小数!";
                    }
                    break;
                case "CDS_HHDL_JGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n最大净功率应保留1位小数!";
                    }
                    break;
                case "CDS_HHDL_PL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n排量应为整数!";
                    }
                    break;
                case "CDS_HHDL_ZHKGCO2PL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况CO2排放应为整数!";
                    }
                    break;
                case "CDS_HHDL_DLXDCZBCDY":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池组标称电压应为整数!";
                    }
                    break;
                case "CDS_HHDL_ZHGKDNXHL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况电能消耗量应为整数!";
                    }
                    break;
                case "CDS_HHDL_CDDMSXZHGKXSLC":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n纯电动模式下综合工况续驶里程应为整数!";
                    }
                    break;
                case "CDS_HHDL_CDDMSXZGCS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n纯电动模式下1km最高车速应为整数!";
                    }
                    break;
                case "CDS_HHDL_QDDJFZNJ":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n驱动电机峰值扭矩应为整数!";
                    }
                    break;
                case "CDS_HHDL_QDDJEDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n驱动电机额定功率应保留1位小数!";
                    }
                    break;
                case "CDS_HHDL_HHDLZDDGLB":
                    if (!Utils.VerifyFloat2(paramValue))
                    {
                        message += "\n混合动力最大电功率比应保留2位小数!";
                    }
                    break;

                // 燃料电池
                case "RLDC_DDGLMD":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n燃料电池堆功率密度应保留1位小数!";
                    }
                    break;
                case "RLDC_DDHHJSTJXXDCZBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n电电混合技术条件下动力蓄电池组比能量应为整数!";
                    }
                    break;
                case "RLDC_ZHGKHQL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n综合工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "RLDC_ZHGKXSLC":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况续驶里程应为整数!";
                    }
                    break;
                case "RLDC_CDDMSXZGXSCS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n电动汽车30分钟最高车速应为整数!";
                    }
                    break;
                case "RLDC_QDDJEDGL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n驱动电机额定功率应保留1位小数!";
                    }
                    break;
                case "RLDC_QDDJFZNJ":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n驱动电机峰值扭矩应为整数!";
                    }
                    break;
                case "RLDC_CQPBCGZYL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n储氢瓶标称工作压力应为整数!";
                    }
                    break;
                case "RLDC_CQPRJ":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n储氢瓶容积应为整数!";
                    }
                    break;

                default: break;
            }
            return message;
        }

        public static string VerifyRequired(string strName, string value)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                msg = strName + "不能为空;";
            }
            return msg;
        }

        public static string VerifyStrLen(string strName, string value, int expectedLen)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length > expectedLen)
                {
                    msg = strName + "长度过长，最长为" + expectedLen + "位;";
                }
            }
            return msg;
        }

        public static bool VerifyInt(string value)
        {
            return Regex.IsMatch(value, @"^[+]?\d*$");
        }

        public static bool VerifyFloat(string value)
        {
            return Regex.IsMatch(value, @"(\d){1,}\.\d{1}$");
        }

        public static bool VerifyFloat2(string value)
        {
            return Regex.IsMatch(value, @"(\d){1,}\.\d{2}$");
        }

        public static bool VerifyDateTime(string value)
        {
            try
            {
                if (value != null)
                {
                    DateTime time = Convert.ToDateTime(value.ToString());
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion
    }
}
