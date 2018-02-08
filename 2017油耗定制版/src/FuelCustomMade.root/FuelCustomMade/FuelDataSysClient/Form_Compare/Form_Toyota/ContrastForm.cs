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
using FuelDataSysClient.Tool.Tool_Toyota;
using FuelDataSysClient.Form_DBManager;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Compare.Form_Toyota
{
    public partial class ContrastForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        MitsUtils miutils = new MitsUtils();
        DataTable dtTable1 = null;
        DataTable dtTable2 = null;
        DataTable dtCtnyPam = null;
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        ToyotaCompareUtils compareUtils = new ToyotaCompareUtils();

        public ContrastForm()
        {
            InitializeComponent();
        }

        private void ContrastForm_Load(object sender, EventArgs e)
        {
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
            dtCtnyPam = miutils.GetCheckData();
            this.gvDataXT.OptionsBehavior.Editable = false;
            this.gvDataQY.OptionsBehavior.Editable = false;
        }

        //获取本地能源数据
        private DataTable GetLocalALL(string tableName)
        {
            string sql = String.Format(@"select * from {0} where USER_ID = '{1}' and (CLZZRQ>=#{2}#) and  (CLZZRQ<#{3}#)", tableName, Utils.userId, Convert.ToDateTime(dtStartTime.Text), Convert.ToDateTime(dtEndTime.Text).Add(new TimeSpan(24, 0, 0)));
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[0].Copy();
                dt.Columns.Remove("USER_ID");
                dt.Columns.Remove("HGSPBM");
                if (tableName.Equals("VIEW_T_ALL"))
                {
                    dt.Columns.Remove("CT_QTXX");
                }
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
            this.gcDataQY.DataSource = null;
            this.groupBox2.Text = "企业油耗数据";
            this.gvDataQY.Columns.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            string tableName = string.Empty;
            List<string> dtHeadList = new List<string>();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        tableName = MitsUtils.CTNY;
                        dtHeadList = compareUtils.dictCTNY.AsEnumerable().Select(d => d.Key).ToList<string>();
                        break;
                    case 1:
                        tableName = MitsUtils.FCDSHHDL;
                        dtHeadList = compareUtils.dictFCDSHHDL.AsEnumerable().Select(d => d.Key).ToList<string>();
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

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0].AsDataView().ToTable(true,dtHeadList.ToArray());
                    DataTableHelper.removeEmpty(dt);
                    this.gcDataQY.DataSource = dt;
                    this.gvDataQY.BestFitColumns();
                    this.groupBox2.Text = String.Format("企业油耗数据（共{0}条）", ds.Tables[0].Rows.Count);
                }
                else
                {
                    MessageBox.Show("该时间段内企业油耗数据不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            this.gvDataQY.OptionsView.ColumnAutoWidth = false;
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
                CafcService.FuelCAFCDetails[] detailData = cafcService.QueryCalParamDetails(Utils.userId, Utils.password, string.Empty, string.Empty, dtStartTime.Text, dtEndTime.Text);
                if (detailData == null || detailData.Length < 1)
                {
                    MessageBox.Show("获取远程数据失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                this.gcDataXT.DataSource = null;
                this.groupBox1.Text = "系统油耗数据";
                DataTable dt = null;
                this.gvDataXT.Columns.Clear();
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        dt = GetLocalALL("VIEW_T_ALL_ADC");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = addColumon_Actzhgkrlxhl_CTNY(dt, detailData);
                            dt = compareUtils.E2C(compareUtils.dictCTNY, dt, MitsUtils.CTNY);
                        }
                        break;
                    case 1:
                        dt = GetLocalALL("VIEW_T_ALL_FCDS_ADC");
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            dt = addColumon_Actzhgkrlxhl_FCDS(dt, detailData);
                            dt = compareUtils.E2C(compareUtils.dictFCDSHHDL, dt, MitsUtils.FCDSHHDL);
                        }
                        break;
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    this.gcDataXT.DataSource = dt;
                    this.gvDataXT.BestFitColumns();
                    this.groupBox1.Text = String.Format("系统油耗数据（共{0}条）", dt.Rows.Count);
                    this.gvDataXT.OptionsView.ColumnAutoWidth = false;
                }
                else
                {
                    MessageBox.Show("该时间段内油耗数据未同步", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    ExportToExcel toExcel = new ExportToExcel();
                    DataSet ds = new DataSet();
                    if (gcSupplements.DataSource != null)
                    {
                        DataTable dt1 = (DataTable)gcSupplements.DataSource;
                        DataTable dtc1 = dt1.Copy();
                        dtc1.TableName = "补传数据";
                        dtc1.Columns.Remove(dtc1.Columns[0]);
                        ds.Tables.Add(dtc1);
                    }
                    if (gcRecall.DataSource != null)
                    {
                        DataTable dt2 = (DataTable)gcRecall.DataSource;
                        DataTable dtc2 = dt2.Copy();
                        dtc2.TableName = "撤销数据";
                        dtc2.Columns.Remove(dtc2.Columns[0]);
                        ds.Tables.Add(dtc2);
                    }
                    if (gcModify.DataSource != null)
                    {
                        DataTable dtDiff = (DataTable)gcModify.DataSource;
                        DataTable dtcDiff = dtDiff.Copy();
                        dtcDiff.TableName = "修改数据";
                        dtcDiff.Columns.Remove(dtcDiff.Columns[0]);
                        ds.Tables.Add(dtcDiff);
                    }
                    toExcel.ToExcelSheet(ds, saveFileDialog.FileName);
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //全选
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridview = (GridView)((GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0]).MainView;
                gridview.FocusedRowHandle = 0;
                gridview.FocusedColumn = gridview.Columns[1];
                GridControlHelper.SelectItem(gridview, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //反选
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("比对数据"))
                {
                    MessageBox.Show("不能进行此操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var gridview = (GridView)((GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0]).MainView;
                gridview.FocusedRowHandle = 0;
                gridview.FocusedColumn = gridview.Columns[1];
                GridControlHelper.SelectItem(gridview, false);
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
            }
            return radioName;
        }

        //数据分类
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var vins = GetCheckData();
                if (vins != null && vins.Table.Rows.Count == 0)
                {
                    MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "补传数据")   //需要补传数据
                {
                    if (InsertFC_CLJBXX(vins, "1", GetRadio()))
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
                        SearchLocalOTForm slo = new SearchLocalOTForm() { MdiParent = this.MdiParent };
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
                    SearchLocalUploadedForm sluf = new SearchLocalUploadedForm() { MdiParent = this.MdiParent };
                    sluf.LocalData(vins);
                    ((MainForm)this.MdiParent).Ribbon.SelectedPage = sluf.Ribbon.Pages[0];
                    sluf.Show();
                }
                if (this.xtraTabControl1.SelectedTabPage.Text == "修改数据")    //需要修改数据
                {
                    var str = GetCheckString();
                    var dt = (DataTable)gcDataQY.DataSource;
                    DataTable dtNew = dt.Clone();
                    foreach (string s in str)
                    {
                        var dr = dt.Select(String.Format("VIN车架号='{0}'", s));
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
                            dtNew = compareUtils.C2E(compareUtils.dictCTNY, dtNew, ToyotaCompareUtils.CTNY);
                            break;
                        case 1:
                            dtNew = compareUtils.C2E(compareUtils.dictFCDSHHDL, dtNew, ToyotaCompareUtils.FCDSHHDL);
                            break;
                    }
                    if (InsertFC_CLJBXX(dtNew.DefaultView, "2", GetRadio()))
                    {
                        foreach (Form f in Application.OpenForms)
                        {
                            if (f.Name == "SearchLocalUpdateForm")
                            {
                                f.Activate();
                                ((SearchLocalUpdateForm)f).LocalData(dtNew.DefaultView);
                                ((MainForm)this.MdiParent).Ribbon.SelectedPage = ((SearchLocalUpdateForm)f).Ribbon.Pages[0];
                                return;
                            }
                        }
                        SearchLocalUpdateForm suf = new SearchLocalUpdateForm() { MdiParent = this.MdiParent };
                        suf.LocalData(dtNew.DefaultView);
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

        private bool InsertFC_CLJBXX(DataView dv, string flag, string rllxParam)
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

                        string sqlQueryHGSPBM = String.Format("SELECT * FROM FC_CLJBXX WHERE VIN='{0}'", vin);
                        string strHGSPBM = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlQueryHGSPBM, null).Tables[0].Rows[0]["HGSPBM"].ToString();

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
                        OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate) { OleDbType = OleDbType.DBDate };
                        DateTime uploadDeadlineDate = miutils.QueryUploadDeadLine(clzzrqDate);
                        OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate) { OleDbType = OleDbType.DBDate };
                        OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now) { OleDbType = OleDbType.DBDate };
                        OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now) { OleDbType = OleDbType.DBDate };
                        string qtxx = string.Empty;
                        if (dv.Table.Columns.Contains("CT_QTXX"))
                        {
                            qtxx = drMain["CT_QTXX"].ToString().Trim();
                        }
                        else if (dv.Table.Columns.Contains("QTXX"))
                        {
                            qtxx = drMain["QTXX"].ToString().Trim();
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
                                     new OleDbParameter("@HGSPBM",strHGSPBM),
                                     new OleDbParameter("@QTXX",qtxx),
                                     // 1：待上报，补传待上报/2：已修改未上报
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
                            if (paramCode.Equals("CT_QTXX")) continue;
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
            var gridControl = (GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView vins = C2M.SelectedParamEntityDataView((DataView)view.DataSource, "check");
            DataTable TempDt = new DataTable();
            if (vins != null)
            {
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:

                        TempDt = compareUtils.C2E(compareUtils.dictCTNY, vins.Table, MitsUtils.CTNY);
                        break;
                    case 1:

                        TempDt = compareUtils.C2E(compareUtils.dictFCDSHHDL, vins.Table, MitsUtils.FCDSHHDL);
                        break;
                }
            }
            vins = TempDt.DefaultView;
            return vins;
        }


        /// <summary>
        /// 选择选中数据
        /// </summary>
        /// <returns></returns>
        private List<string> GetCheckString()
        {

            var gridControl = (GridControl)((GroupBox)this.xtraTabControl1.SelectedTabPage.Controls[0]).Controls[0];
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            var selectedParamEntityIds = C2M.SelectedParamEntityIds(dv, "VIN车架号");
            if (selectedParamEntityIds.Count > 0)
            {
                return selectedParamEntityIds;
            }
            return null;
        }

        // 数据比对
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
                dtTable1 = (DataTable)gcDataXT.DataSource;
                dtTable2 = (DataTable)gcDataQY.DataSource;

                if (dtTable1 == null || dtTable2 == null)
                {
                    MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    DataTable dtRetAdd = new DataTable();
                    DataTable dtRetDiff = new DataTable();
                    DataTable dtRetDel = new DataTable();
                    dtRetAdd = CompareDataTableAdd(dtTable1, dtTable2);
                    dtRetDiff = CompareDataTableDiff(dtTable1, dtTable2);
                    dtRetDel = CompareDataTableDel(dtTable1, dtTable2);
                    dtRetAdd.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetAdd.Columns["check"].SetOrdinal(0);
                    dtRetDel.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDel.Columns["check"].SetOrdinal(0);
                    dtRetDiff.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDiff.Columns["check"].SetOrdinal(0);
                    this.groupBox3.Text = String.Format("共计{0}条", dtRetAdd.Rows.Count);
                    this.groupBox4.Text = String.Format("共计{0}条", dtRetDel.Rows.Count);
                    this.groupBox5.Text = String.Format("共计{0}条", dtRetDiff.Rows.Count);
                    // 以下代码导出用，导入到一个Excel的多个sheet
                    this.gcSupplements.DataSource = dtRetAdd;
                    this.gvSupplements.BestFitColumns();
                    this.gcRecall.DataSource = dtRetDel;
                    this.gvRecall.BestFitColumns();
                    this.gcModify.DataSource = dtRetDiff;
                    this.gvModify.BestFitColumns();
                    xtraTabControl1.SelectedTabPageIndex = 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DataTable dtADD = new DataTable();
            dtADD = dt2.Clone();

            var vin1List = from d1 in dt1.AsEnumerable()
                           select d1.Field<string>("VIN车架号");

            var tbAdd = from t2 in dt2.AsEnumerable()
                        where vin1List.Contains(t2.Field<string>("VIN车架号")) == false
                        select t2;
            var rowList = tbAdd.ToList();
            foreach (DataRow dr in rowList)
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

            var vin2List = from d2 in dt2.AsEnumerable()
                           select d2.Field<string>("VIN车架号");

            var tbDel = from t1 in dt1.AsEnumerable()
                        where vin2List.Contains(t1.Field<string>("VIN车架号")) == false
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

        // 比较两个DataTable中每一列对应的值
        private DataTable CompareDataTableDiff(DataTable dt11, DataTable dt22)
        {
            DataTable dt1 = ClonTableData(dt11);
            DataTable dt2 = ClonTableData(dt22);
            DataTable dtNew = dt2.Clone();
            dtNew.Columns.Add("错误参数");
            dtNew.Columns.Add("系统值");
            dtNew.Columns.Add("企业值");
            //选取两个表中同时存在的VIN
            var dtList = from d1 in dt1.AsEnumerable()
                         join d2 in dt2.AsEnumerable() on d1.Field<string>("VIN车架号") equals d2.Field<string>("VIN车架号")
                         select d1;
            var vinList = dtList.AsEnumerable().Select(c => c.Field<string>("VIN车架号"));
            var count = vinList.ToList();
            if (count.Count == 0)
            {
                return dtNew;
            }
            //创建一个临时表
            DataTable dt = dtList.CopyToDataTable();

            var dv1 = (from d in dt11.AsEnumerable()
                       where vinList.Contains(d.Field<string>("VIN车架号"))
                       select d);
            var listDv1 = dv1.ToList();

            //存所有列名
            DataTable ColumnList = new DataTable();
            ColumnList.Columns.Add("dtHead");
            foreach (DataColumn dc in dt.Columns)
            {
                ColumnList.Rows.Add(dc.ColumnName);
            }
            //比较每一个表头对应的值
            foreach (DataRow dr in ColumnList.Rows)
            {
                string columnName = dr["dtHead"].ToString();
                foreach (DataRow dr1 in listDv1)
                {
                    string vin = dr1["VIN车架号"].ToString();
                    string val1 = dt1.AsEnumerable().Where(c => c.Field<string>("VIN车架号") == vin).Select(c => c.Field<string>(columnName)).FirstOrDefault();
                    string val2 = dt2.AsEnumerable().Where(c => c.Field<string>("VIN车架号") == vin).Select(c => c.Field<string>(columnName)).FirstOrDefault();
                    val1 = string.IsNullOrEmpty(val1) ? "" : val1;
                    val2 = string.IsNullOrEmpty(val2) ? "" : val2;
                    if (!string.IsNullOrEmpty(val1) && Regex.IsMatch(val1, @"^[+-]?\d*[.]?\d*$") && val1.Contains('.') && !string.IsNullOrEmpty(val2) && Regex.IsMatch(val2, @"^[+-]?\d*[.]?\d*$"))
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

        // 克隆表结构及赋值
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

        private DataTable addColumon_Actzhgkrlxhl_CTNY(DataTable dt,CafcService.FuelCAFCDetails[] detailData)
        {
            var unionResult = (from d in dt.AsEnumerable()
                               join l in detailData
                               on d.Field<string>("CLXH") equals l.Clxh into detail
                               from dl in detail.DefaultIfEmpty()
                               select new
                               {
                                   VIN = d.Field<string>("VIN"),
                                   CLXH = d.Field<string>("CLXH"),
                                   TYMC = d.Field<string>("TYMC"),
                                   TGTZHGKRLXHL = dl == null ? "" : dl.TgtZhgkrlxhl.ToString(),
                                   CT_ZHGKRLXHL = d.Field<string>("CT_ZHGKRLXHL"),
                                   ZCZBZL = d.Field<string>("ZCZBZL"),
                                   ZWPS = d.Field<string>("ZWPS"),
                                   RLLX = d.Field<string>("RLLX"),
                                   CT_BSQXS = d.Field<string>("CT_BSQXS"),
                                   QCSCQY = d.Field<string>("QCSCQY"),
                                   JKQCZJXS = d.Field<string>("JKQCZJXS"),
                                   JYJGMC = d.Field<string>("JYJGMC"),
                                   JYBGBH = d.Field<string>("JYBGBH"),
                                   CLZZRQ = d.Field<DateTime>("CLZZRQ").ToString("yyyy/MM/dd"),
                                   CLZL = d.Field<string>("CLZL"),
                                   YYC = d.Field<string>("YYC"),
                                   QDXS = d.Field<string>("QDXS"),
                                   ZDSJZZL = d.Field<string>("ZDSJZZL"),
                                   ZGCS = d.Field<string>("ZGCS"),
                                   EDZK = d.Field<string>("EDZK"),
                                   LTGG = d.Field<string>("LTGG"),
                                   LJ = d.Field<string>("LJ"),
                                   ZJ = d.Field<string>("ZJ"),
                                   CT_FDJXH = d.Field<string>("CT_FDJXH"),
                                   CT_QGS = d.Field<string>("CT_QGS"),
                                   CT_PL = d.Field<string>("CT_PL"),
                                   CT_EDGL = d.Field<string>("CT_EDGL"),
                                   CT_JGL = d.Field<string>("CT_JGL"),
                                   CT_BSQDWS = d.Field<string>("CT_BSQDWS"),
                                   CT_SQGKRLXHL = d.Field<string>("CT_SQGKRLXHL"),
                                   CT_SJGKRLXHL = d.Field<string>("CT_SJGKRLXHL"),
                                   CT_ZHGKCO2PFL = d.Field<string>("CT_ZHGKCO2PFL"),
                               }).ToList();
            return ObjectReflect.ToDataTable(unionResult);
        }

        private DataTable addColumon_Actzhgkrlxhl_FCDS(DataTable dt, CafcService.FuelCAFCDetails[] detailData)
        {
            var unionResult = (from d in dt.AsEnumerable()
                               join l in detailData
                               on d.Field<string>("CLXH") equals l.Clxh into detail
                               from dl in detail.DefaultIfEmpty()
                               select new
                               {
                                   VIN = d.Field<string>("VIN"),
                                   CLXH = d.Field<string>("CLXH"),
                                   TYMC = d.Field<string>("TYMC"),
                                   TGTZHGKRLXHL = dl == null ? "" : dl.TgtZhgkrlxhl.ToString(),
                                   FCDS_HHDL_ZHGKRLXHL = d.Field<string>("FCDS_HHDL_ZHGKRLXHL"),
                                   ZCZBZL = d.Field<string>("ZCZBZL"),
                                   ZWPS = d.Field<string>("ZWPS"),
                                   RLLX = d.Field<string>("RLLX"),
                                   FCDS_HHDL_BSQXS = d.Field<string>("FCDS_HHDL_BSQXS"),
                                   QCSCQY = d.Field<string>("QCSCQY"),
                                   JKQCZJXS = d.Field<string>("JKQCZJXS"),
                                   JYJGMC = d.Field<string>("JYJGMC"),
                                   JYBGBH = d.Field<string>("JYBGBH"),
                                   CLZZRQ = d.Field<DateTime>("CLZZRQ").ToString("yyyy/MM/dd"),
                                   CLZL = d.Field<string>("CLZL"),
                                   YYC = d.Field<string>("YYC"),
                                   QDXS = d.Field<string>("QDXS"),
                                   ZDSJZZL = d.Field<string>("ZDSJZZL"),
                                   ZGCS = d.Field<string>("ZGCS"),
                                   EDZK = d.Field<string>("EDZK"),
                                   LTGG = d.Field<string>("LTGG"),
                                   LJ = d.Field<string>("LJ"),
                                   ZJ = d.Field<string>("ZJ"),
                                   FCDS_HHDL_HHDLJGXS = d.Field<string>("FCDS_HHDL_HHDLJGXS"),
                                   FCDS_HHDL_XSMSSDXZGN = d.Field<string>("FCDS_HHDL_XSMSSDXZGN"),
                                   FCDS_HHDL_DLXDCZZL = d.Field<string>("FCDS_HHDL_DLXDCZZL"),
                                   FCDS_HHDL_DLXDCZZNL = d.Field<string>("FCDS_HHDL_DLXDCZZNL"),
                                   FCDS_HHDL_DLXDCBNL = d.Field<string>("FCDS_HHDL_DLXDCBNL"),
                                   FCDS_HHDL_CDDMSXZHGKXSLC = d.Field<string>("FCDS_HHDL_CDDMSXZHGKXSLC"),
                                   FCDS_HHDL_CDDMSXZGCS = d.Field<string>("FCDS_HHDL_CDDMSXZGCS"),
                                   FCDS_HHDL_DLXDCZBCDY = d.Field<string>("FCDS_HHDL_DLXDCZBCDY"),
                                   FCDS_HHDL_QDDJLX = d.Field<string>("FCDS_HHDL_QDDJLX"),
                                   FCDS_HHDL_HHDLZDDGLB = d.Field<string>("FCDS_HHDL_HHDLZDDGLB"),
                                   FCDS_HHDL_QDDJFZNJ = d.Field<string>("FCDS_HHDL_QDDJFZNJ"),
                                   FCDS_HHDL_QDDJEDGL = d.Field<string>("FCDS_HHDL_QDDJEDGL"),
                                   FCDS_HHDL_SQGKRLXHL = d.Field<string>("FCDS_HHDL_SQGKRLXHL"),
                                   FCDS_HHDL_SJGKRLXHL = d.Field<string>("FCDS_HHDL_SJGKRLXHL"),
                                   FCDS_HHDL_ZHKGCO2PL = d.Field<string>("FCDS_HHDL_ZHKGCO2PL"),
                                   FCDS_HHDL_FDJXH = d.Field<string>("FCDS_HHDL_FDJXH"),
                                   FCDS_HHDL_QGS = d.Field<string>("FCDS_HHDL_QGS"),
                                   FCDS_HHDL_PL = d.Field<string>("FCDS_HHDL_PL"),
                                   FCDS_HHDL_EDGL = d.Field<string>("FCDS_HHDL_EDGL"),
                                   FCDS_HHDL_JGL = d.Field<string>("FCDS_HHDL_JGL"),
                                   FCDS_HHDL_BSQDWS = d.Field<string>("FCDS_HHDL_BSQDWS"),
                               }).ToList();
            return ObjectReflect.ToDataTable(unionResult);
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.gcDataXT.DataSource = null;
            this.groupBox1.Text = "系统油耗数据";
            this.gcDataQY.DataSource = null;
            this.groupBox2.Text = "企业油耗数据";
        }
    }
}