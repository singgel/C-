using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Text.RegularExpressions;
using Common;
using System.Windows.Forms;

namespace FuelDataSysClient.Tool
{
    public class MergerData
    {
        ListBox userLoginLog = null;    //主窗体的日志控件
        static string strSucess = "合并成功";
        static string strVinSucess = "vin重复 合并成功";
        Dictionary<string, string> mapMessage;
        int iTotal = 0;
        int iSuceess = 0;
        public String Merger(String selectedParamEntityIds,Dictionary<string,string> mapMessage,ListBox listBox = null)
        {
            this.mapMessage  = mapMessage;
            iTotal = mapMessage.Count;
            String msg = "";
            try
            {
                this.userLoginLog = listBox;
                String sqlOCN = "";
                if (selectedParamEntityIds == "")
                {
                    sqlOCN = String.Format("SELECT * FROM VIN_INFO VI LEFT JOIN OCN_CLJBXX OC  ON VI.SC_OCN = OC.SC_OCN AND  OC.OPERATION!=4   WHERE   VI.MERGER_STATUS = 0   order by VI.Create_TIME");
                }
                else
                {
                    sqlOCN = String.Format("SELECT * FROM VIN_INFO VI LEFT JOIN OCN_CLJBXX OC  ON VI.SC_OCN = OC.SC_OCN AND  OC.OPERATION!=4 WHERE   VI.MERGER_STATUS = 0  and VI.ID in ({0}) order by VI.Create_TIME", selectedParamEntityIds);
                }

                DataTable dtOCN_CLJBXX = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlOCN, null).Tables[0];
                if (dtOCN_CLJBXX == null)
                {
                    msg = "未查到相关数据";
                    if (userLoginLog != null)
                    {
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 合并失败 未查到相关数据 ==", DateTime.Now));
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                    }
                    return msg.ToString();
                }
                foreach (DataRow dr in dtOCN_CLJBXX.Rows)
                {
                    String ID = dr["ID"].ToString();
                    String VIN = dr["VIN"].ToString();
                    try
                    {
                        if (String.IsNullOrEmpty(dr["SC_OCN1"].ToString()))
                        {
                            if (mapMessage.ContainsKey(ID))
                            {
                                mapMessage[ID] = String.Format("合并失败  VIN:{0} 不存在对应OCN信息", VIN);
                            }
                            if (userLoginLog != null)
                            {
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 合并失败 ID:{1} VIN:{2}失败原因： 不存在对应OCN信息==", DateTime.Now, ID, VIN));
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                            }
                            continue;
                        }
                        String sqlOCN_RLLX = String.Format("SELECT * FROM  OCN_RLLX_PARAM_ENTITY WHERE SC_OCN='{0}' AND  OPERATION!=4   ", dr["SC_OCN1"].ToString());
                        DataTable dtOCN_RLLX = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlOCN_RLLX, null).Tables[0];
                        if (dtOCN_RLLX == null)
                        {
                            if (mapMessage.ContainsKey(ID))
                            {
                                mapMessage[ID] = String.Format("VIN:{0} 合并失败： 查找OCN失败", VIN);
                            }
                            //msg.Append(vin + "合并失败： 查找OCN失败\n");
                            if (userLoginLog != null)
                            {
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 ID:{1} VIN:{2} 合并失败 失败原因： 查找OCN失败FC_CLJBXX_PK==", DateTime.Now, ID, VIN));
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                            }
                            continue;
                        }
                        InsertFuelParamDate_New(dr, dtOCN_RLLX);
                    }
                    catch (System.Exception ex)
                    {
                        if (mapMessage.ContainsKey(ID))
                        {
                            mapMessage[ID] = String.Format("VIN:{0} 合并失败： "+ex.Message, VIN);
                        }
                        //msg.Append(vin + "合并失败： " + ex.Message + "\n");
                        if (userLoginLog != null)
                        {
                            this.userLoginLog.Invoke(new Action(() =>
                            {
                                this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 ID:{2} VIN:{3} 合并失败 失败原因：{1} ==", DateTime.Now, ex.Message, ID, VIN));
                                this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                            }));
                        }
                    }

                }
                string strFail = "";
                foreach (var val in mapMessage)
                {
                    strFail += String.Format("唯一标识  {0}：{1}\r\n", val.Key, val.Value);
                }
                string summary = string.Format("{0}条合并成功\r\n {1}条合并失败\r\n", iSuceess, (iTotal > iSuceess) ? (iTotal - iSuceess) : 0);
                msg = String.Format("{0}{1}", summary, strFail);
            }
            catch (System.Exception ex)
            {
                msg = ex.Message;
                if (userLoginLog != null)
                {
                    this.userLoginLog.Invoke(new Action(() =>
                    {
                        this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 合并失败 失败原因：{1} ==", DateTime.Now, ex.Message));
                        this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                    }));
                }
            }

            return msg.ToString();
        }
        private void InsertFuelParamDate_New(DataRow dr, DataTable dtOCN_RLLX)
        {
            bool bRet = true;
            try
            {
                using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                {
                    conn.Open();
                    OracleTransaction trans = conn.BeginTransaction();
                    try
                    {
                        DateTime CLZZRQ = Convert.ToDateTime(dr["CLZZRQ"]);
                        int RLLXVESTION = 0;
                        OracleParameter[] parameters_CLJBXX = 
                                        {
				                            new OracleParameter("VIN", OracleDbType.NVarchar2,17),
				                            new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("HGSPBM", OracleDbType.NVarchar2,255),
				                            new OracleParameter("USER_ID", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QCSCQY", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JKQCZJXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLXH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZCZBZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZGCS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LTGG", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLZZRQ", OracleDbType.Date),
				                            new OracleParameter("UPLOADDEADLINE", OracleDbType.Date),
				                            new OracleParameter("TYMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("YYC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZWPS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("ZDSJZZL", OracleDbType.NVarchar2,255),
				                            new OracleParameter("EDZK", OracleDbType.NVarchar2,255),
				                            new OracleParameter("LJ", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QDXS", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CREATETIME", OracleDbType.Date),
				                            new OracleParameter("UPDATETIME", OracleDbType.Date),
				                            new OracleParameter("STATUS", OracleDbType.NVarchar2,1),
				                            new OracleParameter("JYJGMC", OracleDbType.NVarchar2,255),
				                            new OracleParameter("JYBGBH", OracleDbType.NVarchar2,255),
				                            new OracleParameter("QTXX", OracleDbType.NVarchar2,255),
                                        };
                        parameters_CLJBXX[0].Value = dr["VIN"];
                        parameters_CLJBXX[1].Value = dr["SC_OCN"];
                        parameters_CLJBXX[2].Value = string.Empty;
                        parameters_CLJBXX[3].Value = Utils.localUserId;
                        parameters_CLJBXX[4].Value = dr["QCSCQY"];
                        parameters_CLJBXX[5].Value = dr["JKQCZJXS"];
                        parameters_CLJBXX[6].Value = dr["CLXH"];
                        parameters_CLJBXX[7].Value = dr["CLZL"];
                        parameters_CLJBXX[8].Value = dr["RLLX"];
                        parameters_CLJBXX[9].Value = dr["ZCZBZL"];
                        parameters_CLJBXX[10].Value = dr["ZGCS"];
                        parameters_CLJBXX[11].Value = dr["LTGG"];
                        parameters_CLJBXX[12].Value = dr["ZJ"];
                        parameters_CLJBXX[13].Value = CLZZRQ;
                        parameters_CLJBXX[14].Value = Utils.QueryUploadDeadLine(CLZZRQ);
                        parameters_CLJBXX[15].Value = dr["TYMC"];
                        parameters_CLJBXX[16].Value = dr["YYC"];
                        parameters_CLJBXX[17].Value = dr["ZWPS"];
                        parameters_CLJBXX[18].Value = dr["ZDSJZZL"];
                        parameters_CLJBXX[19].Value = dr["EDZK"];
                        parameters_CLJBXX[20].Value = dr["LJ"];
                        parameters_CLJBXX[21].Value = dr["QDXS"];
                        parameters_CLJBXX[22].Value = DateTime.Today;
                        parameters_CLJBXX[23].Value = DateTime.Today;
                        parameters_CLJBXX[24].Value = 1;
                        parameters_CLJBXX[25].Value = dr["JCJGMC"];
                        parameters_CLJBXX[26].Value = dr["BGBH"];
                        parameters_CLJBXX[27].Value = string.Empty;
                        try
                        {
                            OracleHelper.ExecuteNonQuery(trans, "Insert into FC_CLJBXX (VIN,SC_OCN,HGSPBM,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,QDXS,CREATETIME,UPDATETIME,STATUS,JYJGMC,JYBGBH,QTXX) values (:VIN,:SC_OCN,:HGSPBM,:USER_ID,:QCSCQY,:JKQCZJXS,:CLXH,:CLZL,:RLLX,:ZCZBZL,:ZGCS,:LTGG,:ZJ,:CLZZRQ,:UPLOADDEADLINE,:TYMC,:YYC,:ZWPS,:ZDSJZZL,:EDZK,:LJ,:QDXS,:CREATETIME,:UPDATETIME,:STATUS,:JYJGMC,:JYBGBH,:QTXX)", parameters_CLJBXX);
                        }
                        catch (System.Exception ex)
                        {
                            if (ex.Message.IndexOf("ORA-00001") == -1)
                            {
                                throw ex;
                            }
                            else
                            {
                                bRet = false;
                                //LogManager.Log("MergerLog", "Error", String.Format("数据插入重复 ：'{0}' '{1}'   '{2}'   '{3}'", dr["UNIQUE_CODE_PL"], dr["VIN_CODE"], dr["UNIQUE_CODE"], dr["PRODUCT_TIME"]));
                            }
                        }
                        if (bRet)
                        {
                            foreach (DataRow drRLLX in dtOCN_RLLX.Rows)
                            {
                                OracleParameter[] parameters_RLLX = 
                                        {
				                            new OracleParameter("PARAM_CODE", OracleDbType.NVarchar2,200),
				                            new OracleParameter("VIN", OracleDbType.NVarchar2,17),
				                            new OracleParameter("PARAM_VALUE", OracleDbType.NVarchar2,200),
                                        };
                                parameters_RLLX[0].Value = drRLLX["CSBM"];
                                parameters_RLLX[1].Value = dr["VIN"];
                                parameters_RLLX[2].Value = drRLLX["CSZ"];
                                RLLXVESTION = Convert.ToInt32(drRLLX["VERSION"]);
                                OracleHelper.ExecuteNonQuery(trans, "Insert into RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE) values (:PARAM_CODE,:VIN,:PARAM_VALUE)", parameters_RLLX);
                            }
                            //STEP4:做合并源版本记录,修改状态
                            OracleParameter[] parameters_MERGER_INFO = 
                                        {
				                            new OracleParameter("VIN", OracleDbType.NVarchar2,17),
				                            new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                            new OracleParameter("CLZZRQ", OracleDbType.Date),
				                            new OracleParameter("CLJBXX_VERSION", OracleDbType.Int32),
				                            new OracleParameter("RLLX_VERSION", OracleDbType.Int32),
				                            new OracleParameter("CREATE_TIME", OracleDbType.Date),
                                        };
                            parameters_MERGER_INFO[0].Value = dr["VIN"];
                            parameters_MERGER_INFO[1].Value = dr["SC_OCN"];
                            parameters_MERGER_INFO[2].Value = CLZZRQ;
                            parameters_MERGER_INFO[3].Value = dr["VERSION"];
                            parameters_MERGER_INFO[4].Value = RLLXVESTION.ToString();
                            parameters_MERGER_INFO[5].Value = DateTime.Today;
                            OracleHelper.ExecuteNonQuery(trans, "Insert into VIN_MERGER_INFO (VIN,SC_OCN,CLZZRQ,CLJBXX_VERSION,RLLX_VERSION,CREATE_TIME) values (:VIN,:SC_OCN,:CLZZRQ,:CLJBXX_VERSION,:RLLX_VERSION,:CREATE_TIME)", parameters_MERGER_INFO);
                        }
                        OracleHelper.ExecuteNonQuery(trans, string.Format("Update VIN_INFO set MERGER_STATUS=1 where ID={0}", dr["ID"]), null);
                        trans.Commit();
                        iSuceess++;
                        if (userLoginLog != null)
                        {
                            this.userLoginLog.Invoke(new Action(() =>
                            {
                                if (bRet)
                                {
                                    
                                    this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 合并成功 ID:{4} VIN:{1} 生产制造日期:{2} 生产OCN:{3} ==", DateTime.Now, dr["VIN"], CLZZRQ, dr["SC_OCN"], dr["ID"]));
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }
                                else
                                {
                                    this.userLoginLog.Items.Add(String.Format("{0:G}== 自动合并 合并失败 ID:{2} VIN:{1} 该vin已合并", DateTime.Now, dr["VIN"], dr["ID"]));
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }
                            }));
                        }
                        if (bRet)
                        {
                            if (mapMessage.ContainsKey(dr["ID"].ToString()))
                            {
                                mapMessage[dr["ID"].ToString()] = "VIN:" + dr["VIN"].ToString() + "合并成功 ";
                            }
                        }
                        else
                        {
                            if (mapMessage.ContainsKey(dr["ID"].ToString()))
                            {
                                mapMessage[dr["ID"].ToString()] = "VIN:" + dr["VIN"].ToString() + "合并失败 该vin已合并";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        //LogManager.Log("MergerLog", "Error", ex.Message);
                        throw ex;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
