using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using Common;
using FuelDataModel;
using System.Windows.Forms;

namespace FuelDataSysClient.Tool
{
    class UploadData
    {
        ListBox userLoginLog;   //主窗体的日志控件

        public void Upload(ListBox listBox)
        {
            this.userLoginLog = listBox;
            try
            {
                DataTable dtData = QueryAll();
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    ActionUpdate(dtData); 
                }
                dtData = QueryAllUpload();
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    ActionUpload(dtData); 
                }
                dtData = QueryAllUploadOT();
                if (dtData != null && dtData.Rows.Count > 0)
                {
                    ActionUploadOT(dtData);
                }
                
            }
            catch (System.Exception ex)
            {
                LogManager.Log("UploadLog", "Error", String.Format("失败原因：",ex.Message));
            }
            
        }
        //获取全部要上报的数据
        private DataTable QueryAll()
        {
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT VIN FROM FC_CLJBXX WHERE STATUS='1' AND UPLOADDEADLINE>to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", DateTime.Today), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }
        //获取全部待上报数据
        private DataTable QueryAllUpload()
        {
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT VIN FROM FC_CLJBXX WHERE STATUS='1' AND UPLOADDEADLINE>to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND CREATETIME<=to_date('{1}','yyyy-mm-dd hh24:mi:ss')", DateTime.Today, Utils.QueryUploadLimitLine(DateTime.Today)), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }
        //获取全部补传待上报数据
        private DataTable QueryAllUploadOT()
        {
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format(@"SELECT VIN FROM FC_CLJBXX WHERE STATUS='1' AND UPLOADDEADLINE<=to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND CREATETIME<=to_date('{1}','yyyy-mm-dd hh24:mi:ss')", DateTime.Today, Utils.QueryUploadLimitLine(DateTime.Today)), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }
        //正常上报信息
        private List<OperateResult> ApplyParamMultiRows(List<string> vinList)
        {
            // 一次调用接口上传数据条数
            int pageSize = 50;
            // 返回结果
            FuelDataService.OperateResult result = null;
            List<FuelDataService.OperateResult> resSerList = new List<FuelDataService.OperateResult>();
            List<OperateResult> resCltList = new List<OperateResult>();
            // 分组上传时的VIN临时变量
            List<string> tempList = new List<string>();
          
            try
            {
                // 上报
                if (vinList.Count > 0)
                {
                    for (int i = 0; i < vinList.Count; )
                    {
                        // 最后一组上传不足50条
                        if (vinList.Count < pageSize)
                        {
                            pageSize = vinList.Count;
                        }
                        // 截取剩余记录中的pageSize条
                        var res = vinList.Take(pageSize);
                        tempList = (from string s in res select s).ToList<string>();
                        List<VehicleBasicInfo> vbiList = Utils.GetApplyParam(tempList);
                        // 上传

                        result = Utils.service.UploadInsertFuelDataList(Utils.userId, Utils.password, Utils.FuelInfoC2S(vbiList).ToArray(), "CATARC_CUSTOM_2012");
                       
                        
                        resSerList.Add(result);
                        // 移除已上传的pageSize条记录
                        var leftRes = vinList.Skip(pageSize);
                        vinList = (from string s in leftRes select s).ToList<string>();
                    }
                }
                else
                {
                    return null;
                }
                // 将service端OperateResult转换为Client端
                foreach (FuelDataService.OperateResult res in resSerList)
                {
                    resCltList.Add(Utils.OperateResultS2C(res));
                }
               
            }
            catch (Exception ex)
            {
                LogManager.Log("UploadLog", "Error", ex.Message);
            }
            return resCltList;
        }
        //补传上报信息
        private List<OperateResult> ApplyParamMultiRowsOT(List<string> vinList)
        {
            // 一次调用接口上传数据条数
            int pageSize = 50;
            // 返回结果
            FuelDataService.OperateResult result = null;
            List<FuelDataService.OperateResult> resSerList = new List<FuelDataService.OperateResult>();
            List<OperateResult> resCltList = new List<OperateResult>();
            // 分组上传时的VIN临时变量
            List<string> tempList = new List<string>();

            try
            {
                // 上报
                if (vinList.Count > 0)
                {
                    for (int i = 0; i < vinList.Count; )
                    {
                        // 最后一组上传不足50条
                        if (vinList.Count < pageSize)
                        {
                            pageSize = vinList.Count;
                        }
                        // 截取剩余记录中的pageSize条
                        var res = vinList.Take(pageSize);
                        tempList = (from string s in res select s).ToList<string>();
                        List<VehicleBasicInfo> vbiList = Utils.GetApplyParam(tempList);
                        // 上传
                        result = Utils.service.UploadOverTime(Utils.userId, Utils.password, Utils.FuelInfoC2S(vbiList).ToArray(), "CATARC_CUSTOM_2012");
 
                        resSerList.Add(result);
                        // 移除已上传的pageSize条记录
                        var leftRes = vinList.Skip(pageSize);
                        vinList = (from string s in leftRes select s).ToList<string>();
                    }
                }
                else
                {
                    return null;
                }
                // 将service端OperateResult转换为Client端
                foreach (FuelDataService.OperateResult res in resSerList)
                {
                    resCltList.Add(Utils.OperateResultS2C(res));
                }

            }
            catch (Exception ex)
            {
                LogManager.Log("UploadLog", "Error", ex.Message);
            }
            return resCltList;
        }
        private  bool ActionUpdate(DataTable dt)
        {
            bool flag = true;
           
            try
            {
                string strVin = GetUploadData(dt);
                string[] arrVin = strVin.Split(';');

                foreach (string vins in arrVin)
                {
                    string vin = string.Empty;
                    if (!string.IsNullOrEmpty(vins))
                    {
                        vin = vins.Substring(1);

                        DataSet tempDt = Utils.service.QueryVidByVins(Utils.userId, Utils.password, vin);
                        if (tempDt != null)
                        {
                            if (tempDt.Tables[0].Rows.Count > 0)
                                flag = flag && UpdateV_ID(tempDt.Tables[0]);
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log("UploadLog", "Error", ex.Message);
            }
            return flag;
        }
        private string GetUploadData(DataTable dt)
        {
            string vinStr = string.Empty;
            int count = 0;
            try
            {
                if (dt != null)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        count++;
                        vinStr += string.Format(",'{0}'", dt.Rows[i]["VIN"].ToString());
                        if (count % 50 == 0)
                        {
                            vinStr += ";";
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Log("UploadLog", "Error", ex.Message);
            }
            return vinStr;
        }
        private bool UpdateV_ID(DataTable dt)
        {
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                using (OracleTransaction tra = con.BeginTransaction())
                {
                    try
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            OracleHelper.ExecuteNonQuery(tra, string.Format("update FC_CLJBXX set V_ID='{0}',STATUS='0' where VIN='{1}'", dr[1], dr[0]));
                            this.userLoginLog.Invoke(new Action(() =>
                            {
                                this.userLoginLog.Items.Add(String.Format("{0:G}== 自动上报 VIN:{1} 已经上报，反馈码为:{2} ==", DateTime.Now, dr[0], dr[1]));
                                this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                            }));
                        }
                        tra.Commit();
                        return true;
                    }
                    catch(Exception ex)
                    {
                        tra.Rollback();
                        LogManager.Log("UploadLog", "Error", ex.Message);
                    }
                }
            }
            return false;
        }
        private bool ActionUpload(DataTable dtData)
        {
            try
            {
                List<string> vinList = new List<string>();
                foreach (DataRow r in dtData.Rows)
                {
                    vinList.Add(String.Format("'{0}'", r["VIN"]));
                }
                List<OperateResult> orList = ApplyParamMultiRows(vinList);
                // 获取上报结果
                List<string> vinsSucc = new List<string>();
                List<NameValuePair> vinsFail = new List<NameValuePair>();
                Dictionary<string, string> dSuccVinVid = new Dictionary<string, string>();
                Utils.getOperateResultListVins(orList, vinsSucc, vinsFail, dSuccVinVid);
                string strSucc = "";// "备案号（VIN）：返回码（VID）";
                // 修改本地状态为“0：已上报”
                if (vinsSucc.Count > 0)
                {
                    string strUpdate = "";
                    for (int i = 0; i < vinsSucc.Count; i++)
                    {
                        strUpdate += String.Format(",'{0}'", vinsSucc[i]);
                        strSucc += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：{1}, \r成功 \r\n", vinsSucc[i], dSuccVinVid[vinsSucc[i]]); 
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(String.Format("{0:G}== 自动上报 VIN:{1} 上报成功，反馈码为:{2} ==", DateTime.Now, vinsSucc[i], dSuccVinVid[vinsSucc[i]]));
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                        LogManager.Log("UploadLog", "Succ", String.Format("备案号（VIN）：{0}， 反馈码（VID）：{1}", vinsSucc[i], dSuccVinVid[vinsSucc[i]]));
                    }
                    //// 反馈码入库
                    Utils.setVidStatusForUpload(dSuccVinVid);
                }
                if (vinsFail.Count > 0)
                {
                    for (int i = 0; i < vinsFail.Count; i++)
                    {
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(String.Format("{0:G}== 自动上报 VIN:{1} 上报失败，失败原因:{2} ==", DateTime.Now, vinsFail[i].Name, vinsFail[i].Value));
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                        LogManager.Log("UploadLog", "Error", String.Format("备案号（VIN）：{0}， 失败原因：{1}", vinsFail[i].Name, vinsFail[i].Value));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("UploadLog", "Error", ex.Message);
            }
            return true;
        }
        private bool ActionUploadOT(DataTable dtData)
        {
            try
            {
                List<string> vinList = new List<string>();
                foreach (DataRow r in dtData.Rows)
                {
                    vinList.Add(String.Format("'{0}'", r["VIN"]));
                }
                List<OperateResult> orList = ApplyParamMultiRowsOT(vinList);
                // 获取上报结果
                List<string> vinsSucc = new List<string>();
                List<NameValuePair> vinsFail = new List<NameValuePair>();
                Dictionary<string, string> dSuccVinVid = new Dictionary<string, string>();
                Utils.getOperateResultListVins(orList, vinsSucc, vinsFail, dSuccVinVid);
                string strSucc = "";// "备案号（VIN）：返回码（VID）";
                // 修改本地状态为“0：已上报”
                if (vinsSucc.Count > 0)
                {
                    string strUpdate = "";
                    for (int i = 0; i < vinsSucc.Count; i++)
                    {
                        strUpdate += String.Format(",'{0}'", vinsSucc[i]);
                        strSucc += String.Format("备案号（VIN）：{0}，\r反馈码（VID）：{1}, \r成功 \r\n", vinsSucc[i], dSuccVinVid[vinsSucc[i]]);
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(String.Format("{0:G}== 自动上报 VIN:{1} 上报成功，反馈码为:{2} ==", DateTime.Now, vinsSucc[i], dSuccVinVid[vinsSucc[i]]));
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                        LogManager.Log("UploadLog", "Succ", String.Format("备案号（VIN）：{0}， 反馈码（VID）：{1}", vinsSucc[i], dSuccVinVid[vinsSucc[i]]));
                    }
                    //// 反馈码入库
                    Utils.setVidStatusForUpload(dSuccVinVid);
                }
                if (vinsFail.Count > 0)
                {
                    for (int i = 0; i < vinsFail.Count; i++)
                    {
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(String.Format("{0:G}== 自动上报 VIN:{1} 上报失败，失败原因:{2} ==", DateTime.Now, vinsFail[i].Name, vinsFail[i].Value));
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                        LogManager.Log("UploadLog", "Error", String.Format("备案号（VIN）：{0}， 失败原因：{1}", vinsFail[i].Name, vinsFail[i].Value));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Log("UploadLog", "Error", ex.Message);
            }
            return true;
        }
    }
}
