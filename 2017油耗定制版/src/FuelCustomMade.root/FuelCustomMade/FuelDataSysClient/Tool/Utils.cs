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
using FuelDataSysClient.Tool;

namespace FuelDataSysClient
{
    public class Utils
    {
        public const string FUEL = "FUEL"; // 汽车燃料消耗量数据管理系统数据
        public const string GH = "GH"; // 国环系统数据
        public const string CTNY = "CTNY";
        public const string FCDS = "FCDS";
        public static string localUserId = "";
        public static string userId = string.IsNullOrEmpty(FuelDataSysClient.Properties.Settings.Default.UserId) ? "" : FuelDataSysClient.Properties.Settings.Default.UserId;
        public static string password = string.IsNullOrEmpty(FuelDataSysClient.Properties.Settings.Default.UserPWD) ? "" : FuelDataSysClient.Properties.Settings.Default.UserPWD;
        public static string qymc = string.Empty;
        // 上传数据限制时长
        public static int timeCons = FuelDataSysClient.Properties.Settings.Default.TimeConstrain == 0 ? 48 : FuelDataSysClient.Properties.Settings.Default.TimeConstrain;
        public static List<PrintModel> printModel = new List<PrintModel>();
        private static bool isFuelTest;

        public static bool IsFuelTest
        {
            get { return Utils.isFuelTest; }
            set { Utils.isFuelTest = value; }
        }
        public static FuelDataOpen.FuelDataOpen serviceOpen = new FuelDataOpen.FuelDataOpen();
        public static CertificateService.CertificateComparison serviceCertificate = new CertificateService.CertificateComparison();
        public static CafcService.CafcWebService serviceCafc = new CafcService.CafcWebService();
        public static AuthorityManager.Authority serverAuthority = new AuthorityManager.Authority();
        public static FuelDataService.FuelDataSysWebService service = new FuelDataService.FuelDataSysWebService();
        public static FuelFileUpload.FileUploadService serviceFiel = new FuelFileUpload.FileUploadService();
        public static FuelDataService.FuelDataSysWebService GetLoginService()
        {
            FuelDataModel.Utils utils = new FuelDataModel.Utils();
            try
            {
                if (FuelDataSysClient.Properties.Settings.Default.IsProxy)
                {
                    WebProxy Proxy = new WebProxy(string.Format("http://{0}:{1}",
                        FuelDataSysClient.Properties.Settings.Default.ProxyAddr,
                        FuelDataSysClient.Properties.Settings.Default.ProxyPort), true);
                    Proxy.Credentials = new NetworkCredential(FuelDataSysClient.Properties.Settings.Default.ProxyUserId,
                        FuelDataSysClient.Properties.Settings.Default.ProxyPwd, "");
                    service.Proxy = Proxy;
                    serviceOpen.Proxy = Proxy;
                    serviceCertificate.Proxy = Proxy;
                    serviceCafc.Proxy = Proxy;
                    serverAuthority.Proxy = Proxy;
                    serviceFiel.Proxy = Proxy;
                }
                if (IsFuelTest)  //正式线路
                {
                    service.Url = SelectLine.FormalStandardLine;
                    serviceOpen.Url = SelectLine.FormalOpenLine;
                    serviceCertificate.Url = SelectLine.FormalCertificatedLine;
                    serviceCafc.Url = SelectLine.FormalCafcLine;
                    serverAuthority.Url = SelectLine.FormalAuthorityLine;
                    serviceFiel.Url = SelectLine.FormalFielLine;
                }
                else             //测试线路
                {
                    service.Url = SelectLine.TestStandardLine;
                    serviceOpen.Url = SelectLine.TestOpenLine;
                    serviceCertificate.Url = SelectLine.TestCertificatedLine;
                    serviceCafc.Url = SelectLine.TestCafcLine;
                    serverAuthority.Url = SelectLine.TestAuthorityLine;
                    serviceFiel.Url = SelectLine.TestFielLine;
                }
                qymc = service.QueryQymc(userId, password);
            }
            catch (Exception)
            {
            }
            return service;
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


        // 获取燃料类型名称
        public static List<string> GetFuelType(string type)
        {
            List<string> fuelTypeList = new List<string>();
            try
            {
                string sql = @"SELECT DISTINCT FUEL_TYPE,MID(ORDER_RULE,1,1) FROM RLLX_PARAM WHERE STATUS='1' ORDER BY MID(ORDER_RULE,1,1)";
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
                DataTable dt = ds.Tables[0];

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
                dgv.PostEditor();
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

        // 比较节假日数据
        public static bool CompareHoliday()
        {
            DataSet dsServer = new DataSet();
            DataSet dsLocal = new DataSet();
            try
            {
                dsServer = Utils.service.QueryHolidayData(Utils.userId, Utils.password);
                string sqlHol = string.Format(@"SELECT HOL_DAYS FROM FC_HOLIDAY ORDER BY HOL_DAYS DESC");
                dsLocal = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlHol, null);

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
            bool flag = false;
            if (string.IsNullOrEmpty(Utils.userId))
            {
                return false;
            }
            DataSet dsHolliday = new DataSet();
            try
            {
                dsHolliday = service.QueryHolidayData(Utils.userId, Utils.password);
            }
            catch (Exception)
            {
                return false;
            }

            if (dsHolliday != null)
            {
                string strCon = AccessHelper.conn;
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    OleDbTransaction trans = con.BeginTransaction();
                    try
                    {
                        string strDel = @"DELETE FROM FC_HOLIDAY";
                        AccessHelper.ExecuteNonQuery(trans, strDel, null);

                        foreach (DataRow dr in dsHolliday.Tables[0].Rows)
                        {
                            string strInsert = @"INSERT INTO FC_HOLIDAY (HOL_DAYS) 
                                    VALUES(@HOL_DAYS)";
                            OleDbParameter[] holidayList = { 
                                     new OleDbParameter("@HOL_DAYS",dr["HOL_DAYS"])
                                   };
                            AccessHelper.ExecuteNonQuery(trans, strInsert, holidayList);
                        }
                        trans.Commit();
                        flag = true;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                    }
                }
            }
            return flag;
        }

        public static bool CompareParamVersion()
        {
            bool isLatestVer = false;
            string clientVer = string.Empty;

            try
            {
                string serviceVer = Utils.service.QueryParamVersion(Utils.userId, Utils.password).Trim();
                string sqlStr = @"SELECT PARAM_REMARK FROM RLLX_PARAM RP WHERE PARAM_CODE='PARAM_VERSION'";

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStr, new OleDbParameter());

                if (ds.Tables[0].Rows.Count > 0)
                {
                    clientVer = ds.Tables[0].Rows[0]["PARAM_REMARK"].ToString().Trim();
                    if (Convert.ToDouble(serviceVer) > Convert.ToDouble(clientVer))
                    {
                        isLatestVer = true;
                    }
                }
                else
                {
                    isLatestVer = true;
                }
            }
            catch
            {
            }

            return isLatestVer;
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
                string strCon = AccessHelper.conn;
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    OleDbTransaction trans = con.BeginTransaction();
                    try
                    {
                        string strDel = @"DELETE FROM RLLX_PARAM";
                        AccessHelper.ExecuteNonQuery(trans, strDel, null);

                        foreach (RllxParam clientParam in clientParamList)
                        {
                            string strInsert = @"INSERT INTO RLLX_PARAM (PARAM_CODE,PARAM_NAME,FUEL_TYPE,PARAM_REMARK,CONTROL_TYPE,CONTROL_VALUE,STATUS,ORDER_RULE) 
                                    VALUES(@PARAM_CODE,@PARAM_NAME,@FUEL_TYPE,@PARAM_REMARK,@CONTROL_TYPE,@CONTROL_VALUE,@STATUS,@ORDER_RULE)";
                            OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",clientParam.Param_Code),
                                     new OleDbParameter("@PARAM_NAME",clientParam.Param_Name),
                                     new OleDbParameter("@FUEL_TYPE",clientParam.Fuel_Type),
                                     new OleDbParameter("@PARAM_REMARK",string.IsNullOrEmpty(clientParam.Param_Remark)?"":clientParam.Param_Remark),
                                     new OleDbParameter("@CONTROL_TYPE",string.IsNullOrEmpty(clientParam.Control_Type)?"":clientParam.Control_Type),
                                     new OleDbParameter("@CONTROL_VALUE",string.IsNullOrEmpty(clientParam.Control_Value)?"":clientParam.Control_Value),
                                     new OleDbParameter("@STATUS",string.IsNullOrEmpty(clientParam.Status)?"":clientParam.Status),
                                     new OleDbParameter("@ORDER_RULE",string.IsNullOrEmpty(clientParam.Order_Rule)?"":clientParam.Order_Rule)
                                   };
                            AccessHelper.ExecuteNonQuery(trans, strInsert, paramList);
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
            DataSet dsHolliday = new DataSet();
            try
            {
                dsHolliday = service.QueryHolidayData(Utils.userId, Utils.password);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询服务器节假日数据时出现错误: " + ex.Message);
                return;
            }
            if (dsHolliday != null)
            {
                string strCon = AccessHelper.conn;
                using (OleDbConnection con = new OleDbConnection(strCon))
                {
                    con.Open();
                    OleDbTransaction trans = con.BeginTransaction();
                    try
                    {
                        string strDel = @"DELETE FROM FC_HOLIDAY";
                        AccessHelper.ExecuteNonQuery(trans, strDel, null);

                        foreach (DataRow dr in dsHolliday.Tables[0].Rows)
                        {
                            string strInsert = @"INSERT INTO FC_HOLIDAY (HOL_DAYS) 
                                    VALUES(@HOL_DAYS)";
                            OleDbParameter[] holidayList = { 
                                     new OleDbParameter("@HOL_DAYS",dr["HOL_DAYS"])
                                   };
                            AccessHelper.ExecuteNonQuery(trans, strInsert, holidayList);
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
            string dateStr = date.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(dateStr))
            {
                return false;
            }
            try
            {
                string sqlHol = string.Format(@"SELECT HOL_DAYS FROM FC_HOLIDAY WHERE HOL_DAYS='{0}'", dateStr);
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlHol, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }


        #region Tools

        // 将Src类型对象转换为Des类型
        public static void ConvertObj<Src, Des>(Src src, Des des)
        {
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

        // 保时捷数据对象转换专用
        public static List<T> VehicleBasicInfoTo<T>(FuelDataService.VehicleBasicInfo[] source) where T : new()
        {
            List<T> listResult = new List<T>();
            PropertyInfo[] destPropertyInfo = typeof(T).GetProperties();
            PropertyInfo[] srcPropertyInfo = new FuelDataService.VehicleBasicInfo().GetType().GetProperties();
            foreach (FuelDataService.VehicleBasicInfo vb in source)
            {
                T newT = new T();
                foreach (PropertyInfo piDest in destPropertyInfo)
                {
                    bool found = false;
                    foreach (PropertyInfo piSrc in srcPropertyInfo)
                    {
                        if (piDest.Name.Equals(piSrc.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            object result = piSrc.GetValue(vb, null);
                            if (result is DateTime)
                            {
                                DateTime dt = (DateTime)result;
                                piDest.SetValue(newT, dt.ToString("yyyy-MM-dd"), null);
                            }
                            else
                            {
                                piDest.SetValue(newT, result, null);
                            }
                            found = true;

                            break;
                        }
                    }
                    if (!found)
                    {
                        FuelDataService.RllxParamEntity[] rllxParams = vb.EntityList;
                        if (rllxParams == null || rllxParams.Length == 0)
                        {
                            continue;
                        }
                        foreach (FuelDataService.RllxParamEntity rpe in rllxParams)
                        {
                            if (rpe.Param_Code.Contains(piDest.Name))
                            {
                                piDest.SetValue(newT, rpe.Param_Value, null);
                                break;
                            }
                        }
                    }
                }
                listResult.Add(newT);
            }
            return listResult;
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
                string strCon = AccessHelper.conn;
                OleDbConnection con = new OleDbConnection(strCon);
                con.Open();
                OleDbTransaction tra = con.BeginTransaction(); //创建事务，开始执行事务
                try
                {
                    foreach (NameValuePair nvp in nvpList)
                    {
                        if (nvp.Value.IndexOf("VAD") != -1)
                        {
                            string sqlJbxx = String.Format("UPDATE FC_CLJBXX SET V_ID = '{0}',STATUS = '0',UPDATETIME=#{1}#  WHERE VIN ='{2}'", nvp.Value, DateTime.Now, nvp.Name);
                            AccessHelper.ExecuteNonQuery(tra, sqlJbxx, null);
                        }
                    }
                    tra.Commit();
                }
                catch
                {
                    tra.Rollback();
                }
                finally
                {
                    con.Close();
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
                        NameValuePair nvpFailed = new NameValuePair();
                        nvpFailed.Name = nvp.Name;
                        nvpFailed.Value = nvp.Value;
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

            DataSet dsBasic = new DataSet();
            DataSet dsEntity = new DataSet();
            try
            {
                dsBasic = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlBasic, null);
                dsEntity = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlEntity, null);

                vehicleBasicInfoList = Utils.DataTable2Object<VehicleBasicInfo>(dsBasic.Tables[0]);
                rllxEntityList = Utils.DataTable2Object<RllxParamEntity>(dsEntity.Tables[0]);

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

        // 获取gridview选中的VIN
        public static List<string> GetUpdateVin(GridView gv, DataTable dt)
        {
            List<string> vinList = new List<string>();
            try
            {
                gv.PostEditor();

                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if ((bool)dt.Rows[i]["check"])
                        {
                            vinList.Add(dt.Rows[i]["vin"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取信息异常：" + ex.Message);
            }
            return vinList;
        }

        //更新状态与反馈码
        public static void setVidStatusForUpload(Dictionary<string, string> dSuccVinVid)
        {
            try
            {
                string sql = String.Format("UPDATE FC_CLJBXX SET V_ID=@VID,STATUS='0',USER_ID=@USER_ID, UPDATETIME=#{0}# WHERE VIN=@VIN", DateTime.Now);
                foreach (KeyValuePair<string, string> d in dSuccVinVid)
                {
                    OleDbParameter[] param = {
                                                new OleDbParameter("@VID", d.Value),
                                                new OleDbParameter("@USER_ID", Utils.userId),
                                                new OleDbParameter("@VIN", d.Key)
                                            };
                    AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, param);
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
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n发动机型号不能为空!";
                    }
                    if (Utils.VerifySpace(paramValue))
                    {
                        message += "\n发动机型号不能包含空格!";
                    }
                    break;
                case "CT_QGS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n气缸数应为整数!";
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
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyFloat(paramValue))
                    {
                        message += "\n最大净功率应保留1位小数!";
                    }
                    break;
                case "CT_BSQXS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n变速器型式不能为空!";
                    }
                    break;
                case "CT_BSQDWS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n变速器档位数不能为空!";
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

                // 非插电式混合动力
                case "FCDS_HHDL_HHDLJGXS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n混合动力结构型式不能为空!";
                    }
                    break;
                case "FCDS_HHDL_XSMSSDXZGN":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n是否具有行驶模式手动选择功能不能为空!";
                    }
                    break;
                case "FCDS_HHDL_DLXDCZZL":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n动力蓄电池组种类不能为空!";
                    }
                    break;
                case "FCDS_HHDL_QDDJLX":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n驱动电机类型不能为空!";
                    }
                    break;
                case "FCDS_HHDL_QGS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n气缸数应为整数!";
                    }
                    break;
                case "FCDS_HHDL_BSQXS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n变速器型式不能为空!";
                    }
                    break;
                case "FCDS_HHDL_BSQDWS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n变速器档位数不能为空!";
                    }
                    break;
                case "FCDS_HHDL_DLXDCZZNL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n动力蓄电池组总能量应保留1位小数!";
                    }
                    break;
                case "FCDS_HHDL_FDJXH":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n发动机型号不能为空!";
                    }
                    if (Utils.VerifySpace(paramValue))
                    {
                        message += "\n发动机型号不能包含空格!";
                    }
                    break;
                case "FCDS_HHDL_DLXDCBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池比能量应为整数!";
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
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyFloat(paramValue))
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
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyInt(paramValue))
                    {
                        message += "\n纯电动模式下1km最高车速应为整数!";
                    }
                    break;
                case "FCDS_HHDL_CDDMSXZHGKXSLC":
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyInt(paramValue))
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
                case "CDS_HHDL_FDJXH":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n发动机型号不能为空!";
                    }
                    if (Utils.VerifySpace(paramValue))
                    {
                        message += "\n发动机型号不能包含空格!";
                    }
                    break;
                case "CDS_HHDL_HHDLJGXS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n混合动力结构型式不能为空!";
                    }
                    break;
                case "CDS_HHDL_XSMSSDXZGN":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n是否具有行驶模式手动选择功能不能为空!";
                    }
                    break;
                case "CDS_HHDL_DLXDCZZL":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n动力蓄电池组种类不能为空!";
                    }
                    break;
                case "CDS_HHDL_QDDJLX":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n驱动电机类型不能为空!";
                    }
                    break;
                case "CDS_HHDL_BSQDWS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n变速器档位数不能为空!";
                    }
                    break;
                case "CDS_HHDL_BSQXS":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n变速器型式不能为空!";
                    }
                    break;
                case "CDS_HHDL_QGS":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n气缸数应为整数!";
                    }
                    break;
                case "CDS_HHDL_DLXDCBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池比能量应为整数!";
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
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyFloat(paramValue))
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
                    if (!Utils.VerifyFloat2(paramValue))
                    {
                        message += "\n综合工况电能消耗量应保留2位小数!";
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
                case "CDS_HHDL_TJASYZDNXHL":
                    if (!Utils.VerifyFloat2(paramValue))
                    {
                        message += "\n条件A试验中电能消耗量应保留2位小数!";
                    }
                    break;
                case "CDS_HHDL_TJBSYZDNXHL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n条件B试验中电能消耗量应保留1位小数!";
                    }
                    break;

                // 纯电动
                case "CDD_DLXDCZZL":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n动力蓄电池组种类不能为空!";
                    }
                    break;
                case "CDD_QDDJLX":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n驱动电机类型不能为空!";
                    }
                    break;
                case "CDD_DLXDCBNL":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池比能量应为整数!";
                    }
                    break;
                case "CDD_DLXDCZEDNL":
                    if (!Utils.VerifyFloat(paramValue))
                    {
                        message += "\n动力蓄电池组总能量应保留1位小数!";
                    }
                    break;
                case "CDD_DDXDCZZLYZCZBZLDBZ":
                    if (!Utils.VerifyInt(paramValue))
                    {
                        message += "\n动力蓄电池总质量与整车整备质量的比值应为整数!";
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
                    if (!Utils.VerifyFloat2(paramValue))
                    {
                        message += "\n综合工况电能消耗量应保留2位小数!";
                    }
                    break;

                // 燃料电池
                case "RLDC_RLLX":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n燃料电池燃料类型不能为空!";
                    }
                    break;
                case "RLDC_DLXDCZZL":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n动力蓄电池组种类不能为空!";
                    }
                    break;
                case "RLDC_QDDJLX":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n驱动电机类型不能为空!";
                    }
                    break;
                case "RLDC_CQPLX":
                    if (Utils.VerifyNull(paramValue))
                    {
                        message += "\n储氢瓶类型不能为空!";
                    }
                    break;
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
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyFloat(paramValue))
                    {
                        message += "\n综合工况燃料消耗量应保留1位小数!";
                    }
                    break;
                case "RLDC_ZHGKXSLC":
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyInt(paramValue))
                    {
                        message += "\n综合工况续驶里程应为整数!";
                    }
                    break;
                case "RLDC_CDDMSXZGXSCS":
                    if (!Utils.VerifyNull(paramValue) && !Utils.VerifyInt(paramValue))
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
                case "RLDC_RLDCXTEDGL":
                    if (Utils.VerifyFloat(paramValue))
                    {
                        message += "\n燃料电池系统额定功率应保留1位小数!";
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

        public static string VerifyStrSpace(string strName, string value)
        {
            string msg = string.Empty;
            if (value.IndexOf(" ") >= 0)
            {
                msg = strName + "不能包含空格;";
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
                    msg = String.Format("{0}长度过长，最长为{1}位;", strName, expectedLen);
                }
            }
            return msg;
        }

        public static bool VerifyNull(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool VerifySpace(string value)
        {
            return value.IndexOf(" ") > 0;
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

        public static void Open(string actionType)
        {
            string dogKey = DogAction.DogHelper.ReadData();
            
            if (dogKey.IndexOf(Utils.userId+Utils.password)>-1)
            {
                serviceOpen.Open(Utils.userId, Utils.password, string.Empty, actionType);
            }
        }

        public static void Close(string actionType)
        {
            serviceOpen.Close(Utils.userId, Utils.password, string.Empty, actionType);
        }

        //根据字符串获取对应类型
        public static Type GetTypeByString(string type)
        {
            switch (type.ToLower())
            {
                case "bool":
                    return Type.GetType("System.Boolean", true, true);
                case "byte":
                    return Type.GetType("System.Byte", true, true);
                case "sbyte":
                    return Type.GetType("System.SByte", true, true);
                case "char":
                    return Type.GetType("System.Char", true, true);
                case "decimal":
                    return Type.GetType("System.Decimal", true, true);
                case "double":
                    return Type.GetType("System.Double", true, true);
                case "float":
                    return Type.GetType("System.Single", true, true);
                case "int":
                    return Type.GetType("System.Int32", true, true);
                case "uint":
                    return Type.GetType("System.UInt32", true, true);
                case "long":
                    return Type.GetType("System.Int64", true, true);
                case "ulong":
                    return Type.GetType("System.UInt64", true, true);
                case "object":
                    return Type.GetType("System.Object", true, true);
                case "short":
                    return Type.GetType("System.Int16", true, true);
                case "ushort":
                    return Type.GetType("System.UInt16", true, true);
                case "string":
                    return Type.GetType("System.String", true, true);
                case "date":
                case "datetime":
                    return Type.GetType("System.DateTime", true, true);
                case "guid":
                    return Type.GetType("System.Guid", true, true);
                default:
                    return Type.GetType(type, true, true);
            }
        }
    }
}
