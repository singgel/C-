using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.OleDb;
using System.Threading;
using Common;
using DevExpress.XtraGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrintingLinks;
using System.Web.Services.Protocols;
using System.Net;
using FuelDataSysClient.FuelCafc;
using FuelDataSysClient.Tool;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using FuelDataSysClient.SubForm;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraGrid.Views.Grid;

namespace FuelDataSysClient
{
    public partial class ContrastForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        MitsUtils miutils = new MitsUtils();
        string userID = Utils.userId;
        string passWD = Utils.password;
        string timeType = "MANUFACTURE_TIME";
        string CTNY = "传统能源";
        List<string> keyList = new List<string>();
        DataTable dtTable1 = null;
        DataTable dtTable2 = null;
        DataTable localDataTable = null;
        DataTableHelper dth = new DataTableHelper();
        DataTable dtCtnyPam = null;
        List<Thread> listThread = new List<Thread>();
        int pageCount = 1;
        private object lockThis = new object();
        private object lockList = new object();
        private string startTime;
        private string endTime;
        int totalCount = 0;
        int totalTarget = 0;
        int threadCount = 1;
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        InitDataTime initTime = new InitDataTime();
        
        public ContrastForm()
        {
            InitializeComponent();

            dtStartTime.Text = initTime.getStartTime();
            dtEndTime.Text = initTime.getEndTime();
            localDataTable = GetLocal();
            dtCtnyPam = miutils.GetCheckData();
            if (!Utils.IsFuelTest)  //测试线路
            {
                barButtonItem8.Enabled = false;
            }
        }

        /// <summary>
        /// 同步服务器端数据到本地
        /// </summary>
        /// <param name="vbInfo">服务器端数据</param>
        /// <returns></returns>
        private bool SynchronousData(FuelDataService.VehicleBasicInfo[] vbInfo)
        {
            bool result = false;
            string sqlUpdate = "update FC_CLJBXX set {0}{1} where VIN='{2}'";
            try
            {
                bool status = false;
                var LocalDT = GetLocal();  //本地数据
                foreach (var serverItem in vbInfo)
                {
                    status = false;
                    if (LocalDT != null)
                    {
                        foreach (DataRow localItem in LocalDT.Rows)
                        {
                            if (serverItem.Vin == Convert.ToString(localItem["vin"]))   //本地数据和服务器数据相同
                            {
                                status = true;
                                string whereStatus = string.Empty;
                                string whereVid = string.Empty;
                                if (Convert.ToString(localItem["status"]) != "0")   //需要更新本地数据状态为0
                                {
                                    whereStatus = "STATUS='0' ,";
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(localItem["V_ID"])))  //如V_ID等于空 同步V_ID
                                {
                                    whereVid = "V_ID='" +serverItem.V_Id + "'";
                                }
                                if (!string.IsNullOrEmpty(whereStatus) || !string.IsNullOrEmpty(whereVid))
                                {
                                    sqlUpdate = string.Format(sqlUpdate, whereStatus, whereVid, serverItem.Vin);
                                    int flag = AccessHelper.ExecuteNonQuery(AccessHelper.conn, sqlUpdate, null);
                                }
                                break;
                            }
                        }
                    }
                    if (!status)  // 本地数据库无数据  
                    {
                        InsertData(serverItem);
                    }
                    
                }


                result = true;
            }
            catch (Exception ex){
                var aa = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// 单条插入本地数据库
        /// </summary>
        /// <param name="vbInfo"></param>
        private void InsertData(FuelDataService.VehicleBasicInfo vbInfo)
        {
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                OleDbTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    #region 待生成的燃料基本信息数据存入燃料基本信息表

                    tra = con.BeginTransaction();
                    string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME,V_ID
                                ) VALUES
                                (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@HGSPBM,@QTXX,@STATUS,@CREATETIME,@UPDATETIME,@V_ID)";

                   

                    OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", vbInfo.Clzzrq);
                    clzzrq.OleDbType = OleDbType.DBDate;

                    DateTime uploadDeadlineDate = miutils.QueryUploadDeadLine(vbInfo.Clzzrq);
                    OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                    uploadDeadline.OleDbType = OleDbType.DBDate;

                    OleDbParameter creTime = new OleDbParameter("@CREATETIME",vbInfo.CreateTime);
                    creTime.OleDbType = OleDbType.DBDate;
                    OleDbParameter upTime = new OleDbParameter("@UPDATETIME", vbInfo.UpdateTime);
                    upTime.OleDbType = OleDbType.DBDate;

                    OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",vbInfo.Vin),
                                     
                                     new OleDbParameter("@USER_ID",vbInfo.User_Id),
                                     new OleDbParameter("@QCSCQY",vbInfo.Qcscqy),
                                     new OleDbParameter("@JKQCZJXS",vbInfo.Jkqczjxs),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@CLXH",vbInfo.Clxh),
                                     new OleDbParameter("@CLZL",vbInfo.Clzl),
                                     new OleDbParameter("@RLLX",vbInfo.Rllx),
                                     new OleDbParameter("@ZCZBZL",vbInfo.Zczbzl),
                                     new OleDbParameter("@ZGCS",vbInfo.Zgcs),
                                     new OleDbParameter("@LTGG",vbInfo.Ltgg),
                                     new OleDbParameter("@ZJ",vbInfo.Zj),
                                     new OleDbParameter("@TYMC",vbInfo.Tymc),
                                     new OleDbParameter("@YYC",vbInfo.Yyc),
                                     new OleDbParameter("@ZWPS",vbInfo.Zwps),
                                     new OleDbParameter("@ZDSJZZL",vbInfo.Zdsjzzl),
                                     new OleDbParameter("@EDZK",vbInfo.Edzk),
                                     new OleDbParameter("@LJ",vbInfo.Lj),
                                     new OleDbParameter("@QDXS",vbInfo.Qdxs),
                                     new OleDbParameter("@JYJGMC",vbInfo.Jyjgmc),
                                     new OleDbParameter("@JYBGBH",vbInfo.Jybgbh),
                                     new OleDbParameter("@HGSPBM",vbInfo.Hgspbm),
                                     new OleDbParameter("@QTXX",vbInfo.Qtxx),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","0"),
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@V_ID",vbInfo.V_Id)
                                     };
                    AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                    #endregion

                    #region 插入参数信息

                    string sqlDelParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + vbInfo.Vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                    // 待生成的燃料参数信息存入燃料参数表
                    foreach (var drParam in vbInfo.EntityList)
                    {
                        string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY 
                                            (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                      VALUES
                                            (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";
                        OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",drParam.Param_Code),
                                     new OleDbParameter("@VIN",drParam.Vin),
                                     new OleDbParameter("@PARAM_VALUE",drParam.Param_Value),
                                     new OleDbParameter("@V_ID","")
                                   };
                        AccessHelper.ExecuteNonQuery(tra, sqlInsertParam, paramList);
                    }

                    tra.Commit();
                    #endregion
                }
                catch (Exception ex)
                {
                    tra.Rollback();
                    throw ex;
                }
                finally
                {
                    tra.Dispose();
                    con.Close();
                }
            }
        }


        /// <summary>
        /// 未使用 获取服务器数据同步字典
        /// </summary>
        /// <param name="vbInfo">服务器对象</param>
        /// <returns></returns>
        private IDictionary<string, string> DownServiceDict(FuelDataService.VehicleBasicInfo[] vbInfo)
        {
            IDictionary<string, string> Dict = new Dictionary<string, string>();
            foreach (var item in vbInfo)
            {
                if (item.Status == "0")  //已上报成功
                {
                    Dict.Add(item.Vin, item.V_Id);
                }
            }
            return Dict;
        }

        /// <summary>
        /// 按日期获取本地查询数据  
        /// </summary>
        /// <returns></returns>
        private DataTable GetLocal()
        {
            string sql = "select v_id,vin,status from FC_CLJBXX where  (CLZZRQ>=#" + Convert.ToDateTime(dtStartTime.Text) + "#) and  (CLZZRQ<#" + Convert.ToDateTime(dtEndTime.Text).Add(new TimeSpan(24, 0, 0)) + "#)";
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql , null);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0];
            }
            return null;
        }

        //获取本地能源数据
        private DataTable GetLocalALL(string tableName)
        {
            string sql = String.Format(@"select * from {0} where cdate(Format(CLZZRQ,'yyyy/mm/dd')) >=#{1}# and cdate(Format(CLZZRQ,'yyyy/mm/dd')) <=#{2}# ", tableName, dtStartTime.Text, dtEndTime.Text);
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0].Copy();
                dt.Columns.Remove("USER_ID");
                return dt;
            }
            return null;
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gridControl2.DataSource = null;
            this.groupBox2.Text = "企业油耗数据";
            this.gridView2.Columns.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            string tableName = string.Empty;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        tableName = MitsUtils.CTNY;
                        break;
                    case 1:
                        tableName = MitsUtils.FCDSHHDL;
                        break;
                    case 2:
                        tableName = MitsUtils.CDSHHDL;
                        break;
                    case 3:
                        tableName = MitsUtils.CDD;
                        break;
                    case 4:
                        tableName = MitsUtils.RLDC;
                        break;
                }
                DataSet ds = miutils.ReadExcel(ofd.FileName, tableName);
                if (!miutils.dictCTNY.ContainsKey("VIN"))
                {
                    miutils.dictCTNY.Add("VIN", "VIN");
                }
                if (!miutils.dictCTNY.ContainsKey("车辆制造日期"))
                {
                    miutils.dictCTNY.Add("车辆制造日期", "CLZZRQ");
                }
                if (!miutils.dictFCDSHHDL.ContainsKey("VIN"))
                {
                    miutils.dictFCDSHHDL.Add("VIN", "VIN");
                }
                if (!miutils.dictFCDSHHDL.ContainsKey("车辆制造日期"))
                {
                    miutils.dictFCDSHHDL.Add("车辆制造日期", "CLZZRQ");
                }
                if (!miutils.dictCDSHHDL.ContainsKey("VIN"))
                {
                    miutils.dictCDSHHDL.Add("VIN", "VIN");
                }
                if (!miutils.dictCDSHHDL.ContainsKey("车辆制造日期"))
                {
                    miutils.dictCDSHHDL.Add("车辆制造日期", "CLZZRQ");
                }
                if (!miutils.dictCDD.ContainsKey("VIN"))
                {
                    miutils.dictCDD.Add("VIN", "VIN");
                }
                if (!miutils.dictCDD.ContainsKey("车辆制造日期"))
                {
                    miutils.dictCDD.Add("车辆制造日期", "CLZZRQ");
                }
                if (!miutils.dictRLDC.ContainsKey("VIN"))
                {
                    miutils.dictRLDC.Add("VIN", "VIN");
                }
                if (!miutils.dictRLDC.ContainsKey("车辆制造日期"))
                {
                    miutils.dictRLDC.Add("车辆制造日期", "CLZZRQ");
                }

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    DataTableHelper.removeEmpty(dt);
                    this.gridControl2.DataSource = dt;
                    this.groupBox2.Text = String.Format("企业油耗数据（共{0}条）", ds.Tables[0].Rows.Count);
                }
                else
                {
                    MessageBox.Show("该时间段内企业油耗数据不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //DataTable dtCtny = miutils.FilterD2D(miutils.dictCTNY, ds.Tables[0], CTNY);
                
            }
            this.gridView2.OptionsView.ColumnAutoWidth = false;
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (Convert.ToDateTime(this.dtEndTime.Text) < Convert.ToDateTime(this.dtStartTime.Text))
                {
                    MessageBox.Show("结束时间不能小于开始时间", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch
            {
                MessageBox.Show("时间格式有误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "系统提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                this.gridControl1.DataSource = null;
                this.groupBox1.Text = "系统油耗数据";
                DataTable dt = null;
                this.gridView1.Columns.Clear();
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        dt = GetLocalALL("VIEW_T_ALL");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictCTNY, dt, MitsUtils.CTNY);
                        }

                        break;
                    case 1:
                        dt = GetLocalALL("VIEW_T_ALL_FCDS");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictFCDSHHDL, dt, MitsUtils.FCDSHHDL);
                        }
                        break;
                    case 2:
                        dt = GetLocalALL("VIEW_T_ALL_CDS");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictCDSHHDL, dt, MitsUtils.CDSHHDL);
                        }
                        break;
                    case 3:
                        dt = GetLocalALL("VIEW_T_ALL_CDD");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictCDD, dt, MitsUtils.CDD);
                        }
                        break;
                    case 4:
                        dt = GetLocalALL("VIEW_T_ALL_RLDC");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = miutils.E2C(miutils.dictRLDC, dt, MitsUtils.RLDC);
                        }
                        break;
                }



                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Columns.Contains("CLZZRQ"))
                    {
                        dt.Columns["CLZZRQ"].ColumnName = "车辆制造日期";
                    }

                    this.gridControl1.DataSource = dt;
                    this.groupBox1.Text = String.Format("系统油耗数据（共{0}条）", dt.Rows.Count);
                    this.gridView1.OptionsView.ColumnAutoWidth = false;
                }
                else
                {
                    MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                if (gcTable1.MainView.RowCount == 0 && gcTable2.MainView.RowCount == 0 && gcDiff.MainView.RowCount == 0)
                {
                    return;
                }
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "本地油耗数据比对结果";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    ExportToExcel toExcel = new ExportToExcel();
                    DataSet ds = new DataSet();
                    if (gcTable1.DataSource != null)
                    {
                        DataTable dt1 = (DataTable)gcTable1.DataSource;
                        DataTable dtc1 = dt1.Copy();
                        dtc1.TableName = "补传数据";
                        dtc1.Columns.Remove(dtc1.Columns[0]);
                        ds.Tables.Add(dtc1);
                    }
                    if (gcTable2.DataSource != null)
                    {
                        DataTable dt2 = (DataTable)gcTable2.DataSource;
                        DataTable dtc2 = dt2.Copy();
                        dtc2.TableName = "撤销数据";
                        dtc2.Columns.Remove(dtc2.Columns[0]);
                        ds.Tables.Add(dtc2);
                    }
                    if (gcDiff.DataSource != null)
                    {
                        DataTable dtDiff = (DataTable)gcDiff.DataSource;
                        DataTable dtcDiff = dtDiff.Copy();
                        dtcDiff.TableName = "修改数据";
                        dtcDiff.Columns.Remove(dtcDiff.Columns[0]);
                        ds.Tables.Add(dtcDiff);
                    }
                    toExcel.ToExcelSheet(ds, saveFileDialog.FileName);
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败，请检查！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally 
            {
                SplashScreenManager.CloseForm();
            }
        }

        /// <summary>
        /// 全选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                GridView view = (GridView)gridControl.MainView;
                if (view.RowCount > 0)
                {
                    view.FocusedRowHandle = 0;
                    view.FocusedColumn = view.Columns[1];
                    Utils.SelectItem(view, true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
                GridView view = (GridView)gridControl.MainView;
                if (view.RowCount > 0)
                {
                    view.FocusedRowHandle = 0;
                    view.FocusedColumn = view.Columns[1];
                    Utils.SelectItem(view, false);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetRadio()
        {
            string radioName = string.Empty;
            switch (radioGroup1.SelectedIndex)
            {
                case 0:

                    radioName = "传统能源";
                    break;
                case 1:

                    radioName = "非插电式混合动力";
                    break;
                case 2:

                    radioName = "插电式混合动力";
                    break;
                case 3:

                    radioName = "纯电动";
                    break;
                case 4:

                    radioName = "燃料电池";
                    break;
            }
            return radioName;
        }

        /// <summary>
        /// 格式化表头
        /// </summary>
        /// <returns></returns>
        private DataView GetDataFormat()
        {
            DataView vins = GetCheckData();
            DataTable TempDt = new DataTable();
            if (vins != null)
            {
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:

                        TempDt = miutils.D2D(miutils.dictCTNY, vins.Table, MitsUtils.CTNY);
                        break;
                    case 1:

                        TempDt = miutils.D2D(miutils.dictFCDSHHDL, vins.Table, MitsUtils.FCDSHHDL);
                        break;
                    case 2:

                        TempDt = miutils.D2D(miutils.dictCDSHHDL, vins.Table, MitsUtils.CDSHHDL);
                        break;
                    case 3:

                        TempDt = miutils.D2D(miutils.dictCDD, vins.Table, MitsUtils.CDD);
                        break;
                    case 4:

                        TempDt = miutils.D2D(miutils.dictRLDC, vins.Table, MitsUtils.RLDC);
                        break;
                }
            }
            vins = TempDt.DefaultView;
            return vins;
        }
       

        //数据分类
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text == "补传数据")   //需要补传数据
                {
                    var vins = GetDataFormat();

                    if (vins != null && vins.Table.Rows.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var rllxName = GetRadio();
                    if (InsertFC_CLJBXX(vins,"1",rllxName))
                    {
                        foreach (Form f in Application.OpenForms)
                        {
                            if (f.Name == "SearchLocalOTForm")
                            {
                                f.Activate();
                                ((SearchLocalOTForm)f).LocalData(vins);
                                ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalOTForm)f).Ribbon.Pages[0];
                                return;
                            }
                        }
                        SearchLocalOTForm slo = new SearchLocalOTForm();
                        slo.MdiParent = this.MdiParent;
                        slo.LocalData(vins);
                        ((MainForm)this.MdiParent).Ribbon.SelectedPage = slo.Ribbon.Pages[0];
                        slo.Show();
                    }
                    else
                    {
                        MessageBox.Show("操作失败，请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "撤销数据")  //需要撤销数据
                {
                    var vins = GetDataFormat();

                    if (vins != null && vins.Table.Rows.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    foreach (Form f in Application.OpenForms)
                    {
                        if (f.Name == "SearchLocalUploadedForm")
                        {
                            f.Activate();
                            ((SearchLocalUploadedForm)f).LocalData(vins);
                            ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUploadedForm)f).Ribbon.Pages[0];
                            return;
                        }
                    }

                    SearchLocalUploadedForm sluf = new SearchLocalUploadedForm();
                    sluf.MdiParent = this.MdiParent;
                    sluf.LocalData(vins);
                    ((MainForm)this.MdiParent).Ribbon.SelectedPage = sluf.Ribbon.Pages[0];
                    sluf.Show();
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "修改数据")    //需要修改数据
                {
                    var str = GetCheckString();
                    if (str == null || str.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    DataView vins = GetCheckData();

                    var dt = (DataTable)gridControl2.DataSource;
                    DataTable dtNew = dt.Clone();
                    foreach (string s in str)
                    {
                        var dr = dt.Select(String.Format("vin='{0}'", s));
                        if (dr.Length > 0)
                        {
                            foreach (DataRow r in dr)
                            {
                                dtNew.Rows.Add(r.ItemArray);
                            }
                            
                            continue;
                        }
                    }
                    switch (radioGroup1.SelectedIndex)
                    {
                        case 0:

                            dtNew = miutils.D2D(miutils.dictCTNY, dtNew, MitsUtils.CTNY);
                            break;
                        case 1:

                            dtNew = miutils.D2D(miutils.dictFCDSHHDL, dtNew, MitsUtils.FCDSHHDL);
                            break;
                        case 2:

                            dtNew = miutils.D2D(miutils.dictCDSHHDL, dtNew, MitsUtils.CDSHHDL);
                            break;
                        case 3:

                            dtNew = miutils.D2D(miutils.dictCDD, dtNew, MitsUtils.CDD);
                            break;
                        case 4:

                            dtNew = miutils.D2D(miutils.dictRLDC, dtNew, MitsUtils.RLDC);
                            break;
                    }
                    var rllxName = GetRadio();
                    if (InsertFC_CLJBXX(dtNew.DefaultView, "2", rllxName))
                    {
                        foreach (Form f in Application.OpenForms)
                        {
                            if (f.Name == "SearchLocalUpdateForm")
                            {
                                f.Activate();
                                ((SearchLocalUpdateForm)f).LocalData(vins);
                                ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUpdateForm)f).Ribbon.Pages[0];
                                return;
                            }
                        }
                        SearchLocalUpdateForm suf = new SearchLocalUpdateForm();
                        suf.MdiParent = this.MdiParent;
                        suf.LocalData(vins);
                        ((MainForm)this.MdiParent).Ribbon.SelectedPage = suf.Ribbon.Pages[0];
                        suf.Show();
                    }
                    else
                    {
                        MessageBox.Show("操作失败，请检查数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool InsertFC_CLJBXX(DataView dv,string flag,string rllxParam)
        {
            bool result = false;
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                OleDbTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    tra = con.BeginTransaction();
                    foreach (DataRow drMain in dv.Table.Rows)
                    {
                        #region 待生成的燃料基本信息数据存入燃料基本信息表

                        
                        string vin = drMain["VIN"].ToString().Trim();
                        string sqlDeleteBasic = "DELETE FROM FC_CLJBXX WHERE VIN='" + vin + "'";
                        AccessHelper.ExecuteNonQuery(tra, sqlDeleteBasic, null);

                        string sqlInsertBasic = @"INSERT INTO FC_CLJBXX
                                (   VIN,USER_ID,QCSCQY,JKQCZJXS,CLZZRQ,UPLOADDEADLINE,CLXH,CLZL,
                                    RLLX,ZCZBZL,ZGCS,LTGG,ZJ,
                                    TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,JYJGMC,JYBGBH,HGSPBM,QTXX,STATUS,CREATETIME,UPDATETIME
                                ) VALUES
                                (   @VIN,@USER_ID,@QCSCQY,@JKQCZJXS,@CLZZRQ,@UPLOADDEADLINE,@CLXH,@CLZL,
                                    @RLLX,@ZCZBZL,@ZGCS,@LTGG,@ZJ,
                                    @TYMC,@YYC,@ZWPS,@ZDSJZZL,@EDZK,@LJ,
                                    @QDXS,@JYJGMC,@JYBGBH,@HGSPBM,@QTXX,@STATUS,@CREATETIME,@UPDATETIME)";

                        DateTime clzzrqDate;
                        try
                        {
                            clzzrqDate = DateTime.ParseExact(drMain["CLZZRQ"].ToString().Trim(), "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch (Exception)
                        {
                            clzzrqDate = Convert.ToDateTime(drMain["CLZZRQ"]);
                        }

                        //DateTime clzzrqDate = Convert.ToDateTime(drVin["CLZZRQ"].ToString().Trim(), CultureInfo.InvariantCulture);

                        OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                        clzzrq.OleDbType = OleDbType.DBDate;

                        DateTime uploadDeadlineDate = miutils.QueryUploadDeadLine(clzzrqDate);
                        OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                        uploadDeadline.OleDbType = OleDbType.DBDate;

                        OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                        creTime.OleDbType = OleDbType.DBDate;
                        OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                        upTime.OleDbType = OleDbType.DBDate;
                        string qtxx = string.Empty;
                        if(dv.Table.Columns.Contains("CT_QTXX"))
                        {
                            qtxx = drMain["CT_QTXX"].ToString().Trim();
                        }
                      
                           
                        OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",drMain["VIN"].ToString().Trim()),
                                     new OleDbParameter("@USER_ID",Utils.userId),
                                     new OleDbParameter("@QCSCQY",drMain["QCSCQY"].ToString().Trim()),
                                     new OleDbParameter("@JKQCZJXS",drMain["JKQCZJXS"].ToString().Trim()),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@CLXH",drMain["CLXH"].ToString().Trim()),
                                     new OleDbParameter("@CLZL",drMain["CLZL"].ToString().Trim()),
                                     new OleDbParameter("@RLLX",drMain["RLLX"].ToString().Trim()),
                                     new OleDbParameter("@ZCZBZL",drMain["ZCZBZL"].ToString().Trim()),
                                     new OleDbParameter("@ZGCS",drMain["ZGCS"].ToString().Trim()),
                                     new OleDbParameter("@LTGG",drMain["LTGG"].ToString().Trim()),
                                     new OleDbParameter("@ZJ",drMain["ZJ"].ToString().Trim()),
                                     new OleDbParameter("@TYMC",drMain["TYMC"].ToString().Trim()),
                                     new OleDbParameter("@YYC",drMain["YYC"].ToString().Trim()),
                                     new OleDbParameter("@ZWPS",drMain["ZWPS"].ToString().Trim()),
                                     new OleDbParameter("@ZDSJZZL",drMain["ZDSJZZL"].ToString().Trim()),
                                     new OleDbParameter("@EDZK",drMain["EDZK"].ToString().Trim()),
                                     new OleDbParameter("@LJ",drMain["LJ"].ToString().Trim()),
                                     new OleDbParameter("@QDXS",drMain["QDXS"].ToString().Trim()),
                                     new OleDbParameter("@JYJGMC",drMain["JYJGMC"].ToString().Trim()),
                                     new OleDbParameter("@JYBGBH",drMain["JYBGBH"].ToString().Trim()),
                                     new OleDbParameter("@HGSPBM",drMain["HGSPBM"].ToString().Trim()),
                                     
                                     new OleDbParameter("@QTXX",qtxx),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS",flag),
                                     creTime,
                                     upTime
                                     };
                        AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                        #endregion

                        #region 插入参数信息

                        string sqlDelParam = "DELETE FROM RLLX_PARAM_ENTITY WHERE VIN ='" + drMain["VIN"].ToString().Trim() + "'";
                        AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                        var rows = dtCtnyPam.Select("FUEL_TYPE='" + rllxParam + "' and STATUS='1'");
                        // 待生成的燃料参数信息存入燃料参数表
                        foreach (DataRow drParam in rows)
                        {
                            string paramCode = drParam["PARAM_CODE"].ToString().Trim();
                            string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY 
                                            (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                      VALUES
                                            (@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";
                            OleDbParameter[] paramList = { 
                                     new OleDbParameter("@PARAM_CODE",paramCode),
                                     new OleDbParameter("@VIN",drMain["VIN"].ToString().Trim()),
                                     new OleDbParameter("@PARAM_VALUE",drMain[paramCode].ToString().Trim()),
                                     new OleDbParameter("@V_ID","")
                                   };
                            AccessHelper.ExecuteNonQuery(tra, sqlInsertParam, paramList);
                        }
                        #endregion
                    }
                    
                    tra.Commit();
                    result = true;

                }
                catch (Exception ex)
                {
                    tra.Rollback();
                    throw ex;
                }
                finally
                {
                    tra.Dispose();
                    con.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 选择选中数据
        /// </summary>
        /// <returns></returns>
        private DataView GetCheckData()
        {

            var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            return C2M.SelectedParamEntityDataView(dv, "check");
            //var selectedParamEntityIds = C2M.SelectedParamEntityIds(dv, "vin");
            //if (selectedParamEntityIds.Count > 0)
            //{
            //    return C2M.List2Str(selectedParamEntityIds);
            //}
            //return dv;
        }


        /// <summary>
        /// 选择选中数据
        /// </summary>
        /// <returns></returns>
        private List<string> GetCheckString()
        {

            var gridControl = (GridControl)this.xtraTabControl1.SelectedTabPage.Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            var selectedParamEntityIds = C2M.SelectedParamEntityIds(dv, "VIN");
            if (selectedParamEntityIds.Count > 0)
            {
                return selectedParamEntityIds;
            }
            return null;
        }

        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult result = MessageBox.Show(
                       "请确认该时间段内已同步最新油耗数据？",
                       "系统提示",
                       MessageBoxButtons.OKCancel,
                       MessageBoxIcon.Question,
                       MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                dtTable1 = (DataTable)gridControl1.DataSource;
                dtTable2 = (DataTable)gridControl2.DataSource;

                if (dtTable1 == null || dtTable2 == null)
                {
                    MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    //弹出加载提示画面  
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    DataTable dtRetAdd = new DataTable();
                    DataTable dtRetDiff = new DataTable();
                    DataTable dtRetDel = new DataTable();

                    dtRetAdd = CompareDataTableAdd(dtTable1, dtTable2);
                    dtRetDiff = CompareDataTableDiff(dtTable1, dtTable2);
                    dtRetDel = CompareDataTableDel(dtTable1, dtTable2);

                    dtRetAdd.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetAdd.Columns["check"].SetOrdinal(0);
                    dtRetAdd.Columns["check"].Caption = "选择";
                    dtRetDel.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDel.Columns["check"].SetOrdinal(0);
                    dtRetDel.Columns["check"].Caption = "选择";
                    dtRetDiff.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDiff.Columns["check"].SetOrdinal(0);
                    dtRetDiff.Columns["check"].Caption = "选择";

                    // 以下代码导出用，导入到一个Excel的多个sheet
                    this.gvTable1.Columns.Clear();
                    this.gvTable2.Columns.Clear();
                    this.gvDiff.Columns.Clear();
                    this.gcTable1.DataSource = dtRetAdd;
                    this.gcTable2.DataSource = dtRetDel;
                    this.gcDiff.DataSource = dtRetDiff;
                    var dt1 = this.gcTable1.DataSource;
                    var dt2 = this.gcTable2.DataSource;
                    xtraTabControl1.SelectedTabPageIndex = 1;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }


        /// <summary>
        /// 需要补传的数据
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public DataTable CompareDataTableAdd(DataTable dt1, DataTable dt2)
        {
            DataTable dtADD= new DataTable();
            dtADD = dt1.Clone();

            var vin1List = from d1 in dt1.AsEnumerable()
                            select d1.Field<string>("VIN");

            var vin2List = from d2 in dt2.AsEnumerable()
                            select d2.Field<string>("VIN");

            var tbAdd = from t2 in dt2.AsEnumerable()
                        where vin1List.Contains(t2.Field<string>("VIN")) ==false
                        select t2;
            var rowList = tbAdd.ToList();
            foreach(DataRow dr in rowList)
            {
                DataRow dd = dtADD.NewRow();
                dd.ItemArray = dr.ItemArray;
                dtADD.Rows.Add(dd);
            }
            return dtADD;
        }

        /// <summary>
        /// 需要撤销的数据
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        public DataTable CompareDataTableDel(DataTable dt1, DataTable dt2)
        {
            DataTable dtDel = new DataTable();
            dtDel = dt1.Clone();

            var vin1List = from d1 in dt1.AsEnumerable()
                           select d1.Field<string>("VIN");

            var vin2List = from d2 in dt2.AsEnumerable()
                           select d2.Field<string>("VIN");

            var tbDel = from t1 in dt1.AsEnumerable()
                        where vin2List.Contains(t1.Field<string>("VIN")) == false
                        select t1;
            var rowList = tbDel.ToList();
            foreach (DataRow dr in rowList)
            {
                DataRow dd = dtDel.NewRow();
                dd.ItemArray = dr.ItemArray;
                dtDel.Rows.Add(dd);
            }
            return dtDel;
        }

        /// <summary>
        /// 比较两个DataTable中每一列对应的值
        /// </summary>
        /// <param name="dt11"></param>
        /// <param name="dt22"></param>
        /// <returns></returns>
        private DataTable CompareDataTableDiff(DataTable dt11, DataTable dt22) 
        {
            DataTable dt1 = ClonTableData(dt11);
            DataTable dt2 = ClonTableData(dt22);
            DataTable dtNew = dt1.Clone();
            dtNew.Columns.Add("错误参数");
            dtNew.Columns.Add("系统值");
            dtNew.Columns.Add("企业值");
            //选取两个表中同时存在的VIN
            var dtList = from d1 in dt1.AsEnumerable()
                          join d2 in dt2.AsEnumerable() on d1.Field<string>("VIN") equals d2.Field<string>("VIN")
                           select d1;
            var vinList = dtList.AsEnumerable().Select(c=>c.Field<string>("VIN"));
            var count = vinList.ToList();
            if (count.Count == 0) 
            {
                return dtNew;
            }
            //创建一个临时表
            DataTable dt = dtList.CopyToDataTable();

            var dv1 = (from d in dt11.AsEnumerable()
                       where vinList.Contains(d.Field<string>("VIN"))
                       select d);
            var listDv1 = dv1.ToList();

            //var dv2 = (from d in dt22.AsEnumerable()
            //           where vinList.Contains(d.Field<string>("VIN"))
            //           select d);
            //var listDv2 = dv2.ToList();
            //dt1.PrimaryKey = new DataColumn[] { dt1.Columns["VIN"] };
            //dt2.PrimaryKey = new DataColumn[] { dt2.Columns["VIN"] };

            //DataView dv1 = dt1.AsEnumerable().Where(c => vinList.Contains(c.Field<string>("VIN"))).CopyToDataTable().DefaultView;
            //DataView dv2 = dt2.AsEnumerable().Where(c => vinList.Contains(c.Field<string>("VIN"))).CopyToDataTable().DefaultView;

            //存所有列名
            DataTable ColumnList=new DataTable();
            ColumnList.Columns.Add("dtHead");
            foreach(DataColumn dc in dt.Columns)
            {
                string s = dc.ColumnName;
                ColumnList.Rows.Add(dc.ColumnName);
            }
            //比较每一个表头对应的值
            foreach (DataRow dr in ColumnList.Rows)
            {
                string columnName = dr["dtHead"].ToString();
                foreach (DataRow dr1 in listDv1)
                {
                    string vin = dr1["VIN"].ToString();
                    string val1 = dt1.AsEnumerable().Where(c => c.Field<string>("VIN") == vin).Select(c => c.Field<string>(columnName)).FirstOrDefault();
                    string val2 = dt2.AsEnumerable().Where(c => c.Field<string>("VIN") == vin).Select(c => c.Field<string>(columnName)).FirstOrDefault();
                    val1 = string.IsNullOrEmpty(val1) ? "" : val1;
                    val2 = string.IsNullOrEmpty(val2) ? "" : val2;
                    if (!string.IsNullOrEmpty(val1) && Regex.IsMatch(val1, @"^[+-]?\d*[.]?\d*$")&& val1.Contains('.') && !string.IsNullOrEmpty(val2) && Regex.IsMatch(val2, @"^[+-]?\d*[.]?\d*$"))
                    {
                        int length = val1.Remove(0, val1.IndexOf(".") + 1).Length;
                        val1 = Convert.ToDecimal(val1).ToString("N" + length);
                        val2 = Convert.ToDecimal(val2).ToString("N" + length);
                    }
                    if (val1 != val2) 
                    {
                        DataRow dd = dtNew.NewRow();
                        dd.ItemArray = dr1.ItemArray;
                        dd["错误参数"] = columnName;
                        dd["系统值"] = val1;
                        dd["企业值"] = val2;
                        dtNew.Rows.Add(dd);
                    }
                }
            }
            return dtNew;
        }

        /// <summary>
        /// 克隆表结构及赋值
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable ClonTableData(DataTable dt) 
        {
            DataTable dtNew = new DataTable();
            //克隆表结构
            dtNew = dt.Clone();
            foreach (DataColumn col in dtNew.Columns)
            {
                col.DataType = typeof(String);
            }
            foreach (DataRow dr in dt.Rows)
            {
                DataRow dd = dtNew.NewRow();
                dd.ItemArray = dr.ItemArray;
                dtNew.Rows.Add(dd);
            }
            return dtNew;
        }
       private void ContrastForm_Load(object sender, EventArgs e)
        {
            //gridView1 全部只读
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView2.OptionsBehavior.Editable = false;
            //this.gridView4.OptionsBehavior.Editable = false;
            //this.gridView5.OptionsBehavior.Editable = false;
            //this.gridView6.OptionsBehavior.Editable = false;

            //this.gridView4.Columns["check"].OptionsColumn.AllowEdit = true;
            //this.gridView5.Columns["check"].OptionsColumn.AllowEdit = true;
            //this.gridView6.Columns["check"].OptionsColumn.AllowEdit = true;
           
        }

        ////数据同步
        //private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    //弹出加载提示画面  
        //    new Thread((ThreadStart)delegate
        //    {
        //        WaitBeforeLogin = new DevExpress.Utils.WaitDialogForm("请稍候...", "正在同步数据");
        //        Application.Run(WaitBeforeLogin);
        //    }).Start();

        //    int count = 1;
        //    while (true)
        //    {
        //        try
        //        {
        //            var fuelData = service.QueryUploadedFuelData(userID, passWD, count, 50, string.Empty, string.Empty, string.Empty, string.Empty, dtStartTime.Text.ToString(), dtEndTime.Text.ToString(), timeType);
        //            if (fuelData != null)
        //            {
        //                if (fuelData.Length == 0)
        //                    break;

        //                SynchronousData(fuelData);
        //                count++;
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        catch (WebException ex)
        //        {
        //            WaitBeforeLogin.Invoke((EventHandler)delegate { WaitBeforeLogin.Close(); });
        //            MessageBox.Show("请本地检测网络");
        //            return;
        //        }
        //    }

        //    //关闭登录提示画面  
        //    WaitBeforeLogin.Invoke((EventHandler)delegate { WaitBeforeLogin.Close(); });
        //    MessageBox.Show("同步完成");
        //}

        private int GetPageCount()
        {

            lock (lockThis)
            {
                return this.pageCount++;
            }

        }

        private bool SynchronousDataEasy(FuelDataService.VehicleBasicInfo[] vbInfo)
        {
            lock (lockList)
            {
                bool result = false;
                string sqlUpdate = "update FC_CLJBXX set {0}{1} where VIN='{2}'";
                try
                {
                    foreach (var serverItem in vbInfo)
                    {
                        String whereStatus = "STATUS='0' ,";
                        String whereVid = "V_ID='" + serverItem.V_Id + "'";

                        String sql = string.Format(sqlUpdate, whereStatus, whereVid, serverItem.Vin.Trim());
                        int flag = AccessHelper.ExecuteNonQuery(AccessHelper.conn, sql, null);
                        if (flag == 0)
                        {
                            InsertData(serverItem);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    var aa = ex.Message;
                }
                return result;
            }
        }

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
                    var fuelData = service.QueryUploadedFuelData(userID, passWD, page, 100, string.Empty, string.Empty, string.Empty, string.Empty, this.startTime, this.endTime, timeType);
                    exception = false;
                    if (fuelData != null)
                    {
                        zeroError = fuelData.Length == 0 ? true : false;
                        if (zeroError)
                        {
                            //MessageBox.Show("出现集合长度为空");
                        }
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
                catch (WebException ex)
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


        ////数据同步
        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            DateTime start = DateTime.Now;
            try
            {
                //弹出加载提示画面  
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                this.startTime = this.dtStartTime.Text.ToString();
                this.endTime = this.dtEndTime.Text.ToString();
                CafcService.StatisticsData[] staData = null;
                staData = cafcService.QueryStatisticsData(Utils.userId, Utils.password, "", "", this.startTime, this.endTime);
                foreach (CafcService.StatisticsData s in staData)
                {
                    this.totalTarget += s.Sl;
                }

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
                    MessageBox.Show(String.Format("同步数据不全:应同步{0},实际同步为{1}", this.totalTarget.ToString(), this.totalCount.ToString()), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                DateTime end = DateTime.Now;
                TimeSpan ts = end.Subtract(start).Duration();
                string dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
                this.pageCount = 1;
                //关闭登录提示画面
                SplashScreenManager.CloseForm();
                MessageBox.Show(String.Format("同步完成,消耗时间{0},同步数据{1}条", dateDiff, totalCount.ToString()), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.totalCount = 0;
                this.totalTarget = 0;
            }
        }

        private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView2_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView4_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView5_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void gridView6_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.gridControl1.DataSource = null;
            this.gridView1.Columns.Clear();
            this.groupBox1.Text = "系统油耗数据";
            this.gridControl2.DataSource = null;
            this.gridView2.Columns.Clear();
            this.groupBox2.Text = "企业油耗数据";
        }
    }
}