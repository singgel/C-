using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using FuelDataModel;
using FuelDataSysClient.SubForm;
using System.Data.OleDb;
using DevExpress.XtraEditors.Repository;
using System.Threading;
using FuelDataSysClient.Tool;
using DevExpress.XtraGrid.Views.Grid;
using System.Reflection;
using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.Form_SJSB;
using FuelDataSysClient.Model;
using Common;

namespace FuelDataSysClient.Form_SJTJ
{
    public partial class SearchServerForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private Dictionary<string, FuelDataModel.RllxParamEntity[]> rpeht = new Dictionary<string, FuelDataModel.RllxParamEntity[]>();
        int totalCount = 0;
        int totalTarget = 0;
        List<Thread> listThread = new List<Thread>();
        int threadCount = 1;
        int pageCount = 1;
        private object lockThis = new object();
        private object lockList = new object();
        
        public SearchServerForm()
        {
            InitializeComponent();
            // 设置燃料类型下拉框的值
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //修改车辆基本信息
        private void dgvCljbxx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)gcQueryCljbxx.FocusedView;
            VehicleBasicInfo vbi = (VehicleBasicInfo)cv.GetFocusedRow();
            if (vbi == null)
            {
                return;
            }
            // 弹出详细信息窗口
            JbxxViewForm jvf = new JbxxViewForm();
            setControlValue(jvf, "tbvin", vbi.Vin, false);
            setControlValue(jvf, "tbHgspbm", vbi.Hgspbm, false);
            setControlValue(jvf, "tbQcscqy", vbi.Qcscqy, false);
            setControlValue(jvf, "tbJkqczjxs", vbi.Jkqczjxs, false);
            setControlValue(jvf, "tbClxh", vbi.Clxh, false);
            setControlValue(jvf, "tbClzl", vbi.Clzl, false);
            setControlValue(jvf, "tbRllx", vbi.Rllx, false);
            setControlValue(jvf, "tbZczbzl", vbi.Zczbzl.ToString(), false);
            setControlValue(jvf, "tbZgcs", vbi.Zgcs.ToString(), false);
            setControlValue(jvf, "tbLtgg", vbi.Ltgg, false);
            setControlValue(jvf, "tbZj", vbi.Zj.ToString(), false);
            setControlValue(jvf, "tbClzzrq", vbi.Clzzrq.ToString(), false);
            setControlValue(jvf, "tbTymc", vbi.Tymc, false);
            setControlValue(jvf, "tbYyc", vbi.Yyc, false);
            setControlValue(jvf, "tbZwps", vbi.Zwps.ToString(), false);
            setControlValue(jvf, "tbZdsjzzl", vbi.Zdsjzzl.ToString(), false);
            setControlValue(jvf, "tbEdzk", vbi.Edzk.ToString(), false);
            setControlValue(jvf, "tbLj", vbi.Lj.ToString(), false);
            setControlValue(jvf, "tbQdxs", vbi.Qdxs, false);
            setControlValue(jvf, "tbJyjgmc", vbi.Jyjgmc, false);
            setControlValue(jvf, "tbJybgbh", vbi.Jybgbh, false);
            setControlValue(jvf, "tbQtxx", vbi.Qtxx, false);
            // 获取燃料信息
            FuelDataModel.RllxParamEntity[] rpelist = this.rpeht[vbi.Vin];
            for (int i = 0; rpelist != null && i < rpelist.Length; i++)
            {
                FuelDataModel.RllxParamEntity rpe = rpelist[i];
                setControlValue(jvf, rpe.Param_Code, rpe.Param_Value, false);
            }
            (jvf.Controls.Find("tc", true)[0] as XtraTabControl).SelectedTabPageIndex = 0;
            jvf.MaximizeBox = false;
            jvf.MinimizeBox = false;
            Utils.SetFormMid(jvf);
            jvf.setVisible("btnbaocun", false);
            jvf.setVisible("btnbaocunshangbao", false);
            jvf.setVisible("btnPrint", true);
            jvf.ShowDialog();
        }

        //给窗体-控件-赋值-是否可以编辑
        public static void setControlValue(JbxxViewForm jvf, string cName, String val, bool enable)
        {
            if (cName == null || "" == cName)
            {
                return;
            }
            Control[] c = jvf.Controls.Find(cName, true);
            if (c.Length > 0)
            {
                if (c[0] is TextEdit)
                {
                    c[0].Text = val;
                }
                if (c[0] is DevExpress.XtraEditors.ComboBoxEdit)
                {
                    DevExpress.XtraEditors.ComboBoxEdit cb = c[0] as DevExpress.XtraEditors.ComboBoxEdit;
                    cb.Text = val;
                    if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                        || cb.Text == "双燃料" || cb.Text == "纯电动" || cb.Text == "非插电式混合动力" || cb.Text == "插电式混合动力" || cb.Text == "燃料电池")
                    {
                        string rlval = cb.Text;
                        if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                        || cb.Text == "双燃料")
                        {
                            rlval = "传统能源";
                        }
                        // 构建燃料参数控件
                        jvf.getParamList(rlval, false);
                    }
                }
                c[0].Enabled = enable;
            }
        }

        //将远程查到的数据同步到本地
        private void cmsSaveToLocal_Click(object sender, EventArgs e)
        {
            ColumnView cv = (ColumnView)gcQueryCljbxx.FocusedView;
            VehicleBasicInfo vbi = (VehicleBasicInfo)cv.GetFocusedRow();
            if (vbi == null)
            {
                MessageBox.Show("请选中要保存的行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FuelDataModel.RllxParamEntity[] rpelist = this.rpeht[vbi.Vin];
            OracleParameter clzzrq = new OracleParameter("CLZZRQ", vbi.Clzzrq) { DbType = DbType.Date };
            DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(vbi.Clzzrq);
            OracleParameter uploadDeadline = new OracleParameter("UPLOADDEADLINE", uploadDeadlineDate) { DbType = DbType.Date };
            OracleParameter creTime = new OracleParameter("CREATETIME", DateTime.Now) { DbType = DbType.Date };
            OracleParameter upTime = new OracleParameter("UPDATETIME", vbi.CreateTime) { DbType = DbType.Date };
            OracleParameter[] param = { 
                                    new OracleParameter("V_ID",vbi.V_Id), 
                                    new OracleParameter("VIN",vbi.Vin),
                                    new OracleParameter("HGSPBM",string.IsNullOrEmpty(vbi.Hgspbm)?"":vbi.Hgspbm),
                                    new OracleParameter("USER_ID",Utils.userId),
                                    new OracleParameter("QCSCQY",vbi.Qcscqy),
                                    new OracleParameter("JKQCZJXS",vbi.Jkqczjxs),
                                    new OracleParameter("CLXH",vbi.Clxh),
                                    new OracleParameter("CLZL",vbi.Clzl),
                                    new OracleParameter("RLLX",vbi.Rllx),
                                    new OracleParameter("ZCZBZL",vbi.Zczbzl),
                                    new OracleParameter("ZGCS",vbi.Zgcs),
                                    new OracleParameter("LTGG",vbi.Ltgg),
                                    new OracleParameter("ZJ",vbi.Zj),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("TYMC",vbi.Tymc),
                                    new OracleParameter("YYC",vbi.Yyc),
                                    new OracleParameter("ZWPS",vbi.Zwps),
                                    new OracleParameter("ZDSJZZL",vbi.Zdsjzzl),
                                    new OracleParameter("EDZK",vbi.Edzk),
                                    new OracleParameter("LJ",vbi.Lj),
                                    new OracleParameter("QDXS",vbi.Qdxs),
                                    new OracleParameter("STATUS","0"),
                                    new OracleParameter("JYJGMC",vbi.Jyjgmc),
                                    new OracleParameter("JYBGBH",vbi.Jybgbh),
                                    new OracleParameter("QTXX",string.IsNullOrEmpty(vbi.Qtxx)?"":vbi.Qtxx),
                                    creTime,
                                    upTime
                                    };
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction trans = con.BeginTransaction();
                try
                {
                    // 删除本地车辆基本信息
                    OracleHelper.ExecuteNonQuery(trans, string.Format(@"DELETE FROM FC_CLJBXX_ADC WHERE VIN='{0}'", vbi.Vin), null);
                    // 保存服务器端基本信息到本地
                    OracleHelper.ExecuteNonQuery(trans, (string)@"INSERT INTO FC_CLJBXX_ADC
                            (   V_ID,
                                VIN,
                                HGSPBM,
                                USER_ID,
                                QCSCQY,
                                JKQCZJXS,
                                CLXH,
                                CLZL,
                                RLLX,
                                ZCZBZL,
                                ZGCS,
                                LTGG,
                                ZJ,
                                CLZZRQ,
                                UPLOADDEADLINE,
                                TYMC,
                                YYC,
                                ZWPS,
                                ZDSJZZL,
                                EDZK,
                                LJ,
                                QDXS,
                                STATUS,
                                JYJGMC,
                                JYBGBH,
                                QTXX,
                                CREATETIME,
                                UPDATETIME
                            ) VALUES
                            (   :V_ID,
                                :VIN,
                                :HGSPBM,
                                :USER_ID,
                                :QCSCQY,
                                :JKQCZJXS,
                                :CLXH,
                                :CLZL,
                                :RLLX,
                                :ZCZBZL,
                                :ZGCS,
                                :LTGG,
                                :ZJ,
                                :CLZZRQ,
                                :UPLOADDEADLINE,
                                :TYMC,
                                :YYC,
                                :ZWPS,
                                :ZDSJZZL,
                                :EDZK,
                                :LJ,
                                :QDXS,
                                :STATUS,
                                :JYJGMC,
                                :JYBGBH,
                                :QTXX,
                                :CREATETIME,
                                :UPDATETIME)", param);

                    // 删除本地燃料参数信息
                    OracleHelper.ExecuteNonQuery(trans, string.Format(@"DELETE FROM RLLX_PARAM_ENTITY_ADC WHERE VIN='{0}'", vbi.Vin), null);
                    string sqlSaveParams = string.Empty;
                    foreach (RllxParamEntity entity in rpelist)
                    {
                        sqlSaveParams = @"INSERT INTO RLLX_PARAM_ENTITY_ADC (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                            VALUES(:PARAM_CODE,:VIN,:PARAM_VALUE,:V_ID)";

                        OracleParameter[] paramList = { 
                                new OracleParameter("PARAM_CODE",entity.Param_Code),
                                new OracleParameter("VIN",entity.Vin),
                                new OracleParameter("PARAM_VALUE",entity.Param_Value),
                                new OracleParameter("V_ID",entity.V_Id)
                            };
                        OracleHelper.ExecuteNonQuery(trans, sqlSaveParams, paramList);
                    }
                    trans.Commit();
                    MessageBox.Show("同步成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("同步失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        //获取模糊查询中的时间类型
        protected string GetTimeType()
        {
            string timeType = "UPLOAD_TIME";// UPLOAD_TIME 表示上报日期
            if (cbTimeType.Text == "制造/进口日期")
            {
                timeType = "MANUFACTURE_TIME";// MANUFACTURE_TIME 表示车辆制造日期/进口日期
            }
            return timeType;
        }

        //同步到本地
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime start = DateTime.Now;
            try
            {
                //统计下载数量
                this.totalTarget = Utils.service.QueryUploadedFuelDataCount(Utils.userId, Utils.password, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                SplashScreenManager.ShowForm(typeof(DevWaitForm));

                listThread.Clear();
                for (int i = 0; i < threadCount; i++)
                {
                    listThread.Add(new Thread(this.LoadRemoteData));
                }
                for (int i = 0; i < threadCount; i++)
                {
                    listThread[i].Start();
                }
                for (int i = 0; i < threadCount; i++)
                {
                    listThread[i].Join();
                }
                if (this.totalCount < this.totalTarget)
                {
                    MessageBox.Show(String.Format("同步数据不全:应同步{0},实际同步为{1}", this.totalTarget, this.totalCount));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DateTime end = DateTime.Now;
                TimeSpan ts = end.Subtract(start).Duration();
                string dateDiff = String.Format("{0}天{1}小时{2}分钟{3}秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                this.pageCount = 1;
                int localCount = DownLoadDataCount();
                //关闭登录提示画面
                SplashScreenManager.CloseForm();
                MessageBox.Show(String.Format("同步完成,消耗时间{0},应同步{1},实际同步为{2}", dateDiff,this.totalCount, localCount));
                this.totalCount = 0;
                this.totalTarget = 0;
            }
        }

        //STEP1:数据下载到本地
        private void LoadRemoteData()
        {
            //出现异常标志
            bool exception = false;
            //出现长度为0的数据标志
            bool zeroError = false;
            int page = 0;
            while (true)
            {
                try
                {
                    if (exception == false && zeroError == false)
                    {
                        page = this.GetPageCount();
                    }
                    string timeType = this.GetTimeType();

                    var fuelData = Utils.service.QueryUploadedFuelData(Utils.userId, Utils.password, page, 100, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, this.dtStartTime.Text, this.dtEndTime.Text, timeType);
                    exception = false;
                    if (fuelData != null)
                    {
                        zeroError = fuelData.Length == 0 ? true : false;
                        this.totalCount += fuelData.Length;
                        this.SynchronousDataEasy(fuelData);
                        if (this.totalCount < this.totalTarget)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (System.Net.WebException ex)
                {
                    exception = true;
                    continue;
                }
                catch (Exception e)
                {
                    exception = true;
                    continue;
                }
            }
        }

        //线程锁，用来获取页数
        private int GetPageCount()
        {
            lock (lockThis)
            {
                return this.pageCount++;
            }
        }

        //STEP2:异步锁遍历数据
        private void SynchronousDataEasy(FuelDataService.VehicleBasicInfo[] vbInfo)
        {
            lock (lockList)
            {
                try
                {
                    foreach (var serverItem in vbInfo)
                    {
                        InsertData(serverItem);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        //STEP3：插入到本地数据库
        private void InsertData(FuelDataService.VehicleBasicInfo vbInfo)
        {
            using (OracleConnection con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                OracleTransaction tra = con.BeginTransaction();
                try
                {
                    OracleHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM FC_CLJBXX_ADC WHERE VIN ='{0}'", vbInfo.Vin), null);
                    OracleParameter clzzrq = new OracleParameter("CLZZRQ", vbInfo.Clzzrq) { DbType = DbType.Date };
                    MitsUtils miutils = new MitsUtils();
                    DateTime uploadDeadlineDate = miutils.QueryUploadDeadLine(vbInfo.Clzzrq);
                    OracleParameter uploadDeadline = new OracleParameter("UPLOADDEADLINE", uploadDeadlineDate) { DbType = DbType.Date };
                    OracleParameter creTime = new OracleParameter("CREATETIME", vbInfo.CreateTime) { DbType = DbType.Date };
                    OracleParameter upTime = new OracleParameter("UPDATETIME", vbInfo.UpdateTime) { DbType = DbType.Date };
                    OracleParameter[] param = { 
                                    new OracleParameter("VIN",vbInfo.Vin),
                                    new OracleParameter("USER_ID",vbInfo.User_Id),
                                    new OracleParameter("QCSCQY",vbInfo.Qcscqy),
                                    new OracleParameter("JKQCZJXS",vbInfo.Jkqczjxs),
                                    clzzrq,
                                    uploadDeadline,
                                    new OracleParameter("CLXH",vbInfo.Clxh),
                                    new OracleParameter("CLZL",vbInfo.Clzl),
                                    new OracleParameter("RLLX",vbInfo.Rllx),
                                    new OracleParameter("ZCZBZL",vbInfo.Zczbzl),
                                    new OracleParameter("ZGCS",vbInfo.Zgcs),
                                    new OracleParameter("LTGG",vbInfo.Ltgg),
                                    new OracleParameter("ZJ",vbInfo.Zj),
                                    new OracleParameter("TYMC",vbInfo.Tymc),
                                    new OracleParameter("YYC",vbInfo.Yyc),
                                    new OracleParameter("ZWPS",vbInfo.Zwps),
                                    new OracleParameter("ZDSJZZL",vbInfo.Zdsjzzl),
                                    new OracleParameter("EDZK",vbInfo.Edzk),
                                    new OracleParameter("LJ",vbInfo.Lj),
                                    new OracleParameter("QDXS",vbInfo.Qdxs),
                                    new OracleParameter("JYJGMC",vbInfo.Jyjgmc),
                                    new OracleParameter("JYBGBH",vbInfo.Jybgbh),
                                    new OracleParameter("HGSPBM",string.IsNullOrEmpty(vbInfo.Hgspbm)?"":vbInfo.Hgspbm),
                                    new OracleParameter("QTXX",string.IsNullOrEmpty(vbInfo.Qtxx)?"":vbInfo.Qtxx),
                                    new OracleParameter("STATUS","0"),
                                    creTime,
                                    upTime,
                                    new OracleParameter("V_ID",vbInfo.V_Id)
                                    };
                    OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO FC_CLJBXX_ADC
                            (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME,V_ID
                            ) VALUES
                            (   :VIN,:USER_ID,:QCSCQY,:JKQCZJXS,:CLZZRQ,:UPLOADDEADLINE,:CLXH,:CLZL,
                                :RLLX,:ZCZBZL,:ZGCS,:LTGG,:ZJ,
                                :TYMC,:YYC,:ZWPS,:ZDSJZZL,:EDZK,:LJ,
                                :QDXS,:JYJGMC,:JYBGBH,:HGSPBM,:QTXX,:STATUS,:CREATETIME,:UPDATETIME,:V_ID)", param);
                    OracleHelper.ExecuteNonQuery(tra, String.Format("DELETE FROM RLLX_PARAM_ENTITY_ADC WHERE VIN ='{0}'", vbInfo.Vin), null);
                    // 待生成的燃料参数信息存入燃料参数表
                    int i = 0;
                    foreach (var drParam in vbInfo.EntityList)
                    {
                        OracleParameter[] paramList = { 
                                    new OracleParameter("PARAM_CODE",drParam.Param_Code),
                                    new OracleParameter("VIN",drParam.Vin),
                                    new OracleParameter("PARAM_VALUE",drParam.Param_Value),
                                    new OracleParameter("V_ID","")
                                };
                        OracleHelper.ExecuteNonQuery(tra, (string)@"INSERT INTO RLLX_PARAM_ENTITY_ADC 
                                        (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                    VALUES
                                        (:PARAM_CODE,:VIN,:PARAM_VALUE,:V_ID)", paramList);
                        i++;
                    }
                    LogManager.Log("Remote", "log", String.Format("{0}    {1} {2}", vbInfo.Vin, vbInfo.Rllx, i));
                    tra.Commit();
                }
                catch (Exception ex)
                {
                    tra.Rollback();
                    throw ex;
                }
            }
        }

        //统计下载数量
        private int DownLoadDataCount()
        {
            try
            {
                StringBuilder sqlWhere = new StringBuilder();
                if (!string.IsNullOrEmpty(this.tbVin.Text))
                {
                    sqlWhere.AppendFormat(@" AND VIN LIKE '%{0}%' ", this.tbVin.Text);
                }
                if (!string.IsNullOrEmpty(this.tbClzl.Text))
                {
                    sqlWhere.AppendFormat(@" AND CLZL LIKE '%{0}%' ", this.tbClzl.Text);
                }
                if (!string.IsNullOrEmpty(this.tbClxh.Text))
                {
                    sqlWhere.AppendFormat(@" AND  Lower(CLXH) LIKE  Lower('%{0}%') ", this.tbClxh.Text);
                }
                if (!string.IsNullOrEmpty(this.cbRllx.Text))
                {
                    sqlWhere.AppendFormat(@" AND RLLX LIKE '%{0}%' ", this.cbRllx.Text);
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
                {
                    sqlWhere.AppendFormat(@" AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("制造/进口日期"))
                {
                    sqlWhere.AppendFormat(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
                }
                string sqlVins = string.Format(@"select count(*) from FC_CLJBXX_ADC where USER_ID='{0}' AND STATUS = '0' {1} ", Utils.userId, sqlWhere);
                DataTable dt = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlVins, null).Tables[0];
                return Int32.Parse(dt.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //导出EXCEL
        private void barExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (gcQueryCljbxx.DataSource == null )
            {
                MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SplashScreenManager.ShowForm(typeof(DevWaitForm));
                        StringBuilder sqlWhere = new StringBuilder();
                        if (!string.IsNullOrEmpty(this.tbVin.Text))
                        {
                            sqlWhere.AppendFormat(@" AND VIN LIKE '%{0}%' ", this.tbVin.Text);
                        }
                        if (!string.IsNullOrEmpty(this.tbClzl.Text))
                        {
                            sqlWhere.AppendFormat(@" AND CLZL LIKE '%{0}%' ", this.tbClzl.Text);
                        }
                        if (!string.IsNullOrEmpty(this.tbClxh.Text))
                        {
                            sqlWhere.AppendFormat(@" AND  Lower(CLXH) LIKE  Lower('%{0}%') ", this.tbClxh.Text);
                        }
                        if (!string.IsNullOrEmpty(this.cbRllx.Text))
                        {
                            sqlWhere.AppendFormat(@" AND RLLX LIKE '%{0}%' ", this.cbRllx.Text);
                        }
                        if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
                        {
                            sqlWhere.AppendFormat(@" AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(UPDATETIME,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') <= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
                        }
                        if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("制造/进口日期"))
                        {
                            sqlWhere.AppendFormat(@" AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss') >= to_date('{0}','yyyy-mm-dd hh24:mi:ss') AND to_date(to_char(CLZZRQ,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<= to_date('{1}','yyyy-mm-dd hh24:mi:ss') ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text));
                        }
                        string sqlStrCTNY = string.Format(@"select * from ADC_T_ALL where 1=1 {0} ", sqlWhere);
                        string sqlStrFCDS = string.Format(@"select * from ADC_T_ALL_FCDS where 1=1 {0} ", sqlWhere);
                        string sqlStrCDS = string.Format(@"select * from ADC_T_ALL_CDS where 1=1 {0} ", sqlWhere);
                        string sqlStrCDD = string.Format(@"select * from ADC_T_ALL_CDD where 1=1 {0} ", sqlWhere);
                        string sqlStrRLDC = string.Format(@"select * from ADC_T_ALL_RLDC where 1=1 {0} ", sqlWhere);
                        DataSet dsCTNY = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrCTNY, null);
                        DataSet dsFCDS = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrFCDS, null);
                        DataSet dsCDS = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrCDS, null);
                        DataSet dsCDD = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrCDD, null);
                        DataSet dsRLDC = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStrRLDC, null);
                        using (DataSet dsExport = new DataSet())
                        {
                            if (dsCTNY != null && dsCTNY.Tables.Count > 0 && dsCTNY.Tables[0] != null && dsCTNY.Tables[0].Rows.Count > 0)
                            {
                                dsCTNY.Tables[0].TableName = "传统能源官方同步数据";
                                dsExport.Tables.Add(dsCTNY.Tables[0].Copy());
                            }
                            if (dsFCDS != null && dsFCDS.Tables.Count > 0 && dsFCDS.Tables[0] != null && dsFCDS.Tables[0].Rows.Count > 0)
                            {
                                dsFCDS.Tables[0].TableName = "非插电式混合动力官方同步数据";
                                dsExport.Tables.Add(dsFCDS.Tables[0].Copy());
                            }
                            if (dsCDS != null && dsCDS.Tables.Count > 0 && dsCDS.Tables[0] != null && dsCDS.Tables[0].Rows.Count > 0)
                            {
                                dsCDS.Tables[0].TableName = "插电式混合动力官方同步数据";
                                dsExport.Tables.Add(dsCDS.Tables[0].Copy());
                            }
                            if (dsCDD != null && dsCDD.Tables.Count > 0 && dsCDD.Tables[0] != null && dsCDD.Tables[0].Rows.Count > 0)
                            {
                                dsCDD.Tables[0].TableName = "纯电动官方同步数据";
                                dsExport.Tables.Add(dsCDD.Tables[0].Copy());
                            }
                            if (dsRLDC != null && dsRLDC.Tables.Count > 0 && dsRLDC.Tables[0] != null && dsRLDC.Tables[0].Rows.Count > 0)
                            {
                                dsRLDC.Tables[0].TableName = "燃料电池官方同步数据";
                                dsExport.Tables.Add(dsRLDC.Tables[0].Copy());
                            }
                            if (dsExport.Tables.Count < 1)
                            {
                                MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            ExportToExcel toExcel = new ExportToExcel();
                            for (int i = 0; i < dsExport.Tables.Count; i++)
                            {
                                toExcel.ExportExcel(folderBrowserDialog.SelectedPath, dsExport.Tables[i]);
                            }
                        }
                        MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SplashScreenManager.CloseForm();
                    }
                }
            }
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetDataFromService(1);
        }

        //清除查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVin.Text = string.Empty;
            this.tbClxh.Text = string.Empty;
            this.tbClzl.Text = string.Empty;
            this.cbRllx.Text = string.Empty;
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //首页
        private void btnFirPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) GetDataFromService(1);
        }

        //上一页
        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) GetDataFromService(--pageNum);
        }

        //下一页
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) GetDataFromService(++pageNum);
        }

        //尾页
        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) GetDataFromService(pageCou);
        }

        //查询远程信息
        private void GetDataFromService(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            //验证用户
            if (!Utils.CheckUser())
            {
                MessageBox.Show("请检查用户名密码", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 验证查询时间：结束时间不能小于开始时间
            if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && Convert.ToDateTime(this.dtStartTime.Text) > Convert.ToDateTime(this.dtEndTime.Text))
            {
                MessageBox.Show("结束时间不能小于开始时间", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //查询出服务器的数据
                int dataCount = Utils.service.QueryUploadedFuelDataCount(Utils.userId, Utils.password, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                FuelDataService.VehicleBasicInfo[] queryInfoArr = Utils.service.QueryUploadedFuelData(Utils.userId, Utils.password, pageNum, pageSize, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                if ((queryInfoArr != null && dataCount > 0) || (queryInfoArr == null && dataCount == 0))
                {
                    if (queryInfoArr != null)
                    {
                        List<VehicleBasicInfo> vbis = Utils.FuelInfoS2C(queryInfoArr);
                        for (int i = 0; i < vbis.Count; i++)
                        {
                            VehicleBasicInfo vbi = vbis[i];
                            if (rpeht.ContainsKey(vbi.Vin))
                            {
                                rpeht.Remove(vbi.Vin);
                            }
                            rpeht.Add(vbi.Vin, vbi.EntityList);
                            vbi.EntityList = null;
                        }
                        gcQueryCljbxx.DataSource = vbis;
                    }
                    else
                    {
                        gcQueryCljbxx.DataSource = null;
                    }
                    this.gvQueryCljbxx.BestFitColumns();
                    //页码显示信息计算
                    int pageCount = dataCount / pageSize;
                    if (dataCount % pageSize > 0) pageCount++;
                    int dataLast = pageSize * pageNum;
                    if (pageNum == pageCount) dataLast = dataCount;
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    if (dataCount == 0)
                    {
                        this.labPage.Text = "当前显示0至0条";
                        this.txtPage.Text = "0/0";
                    }
                    else
                    {
                        this.labPage.Text = String.Format("当前显示{0}至{1}条", (pageSize * (pageNum - 1) + 1), dataLast);
                        this.txtPage.Text = String.Format("{0}/{1}", pageNum, pageCount);
                    }
                }
                else
                {
                    MessageBox.Show("获取数据失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("获取数据失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }
    }
}