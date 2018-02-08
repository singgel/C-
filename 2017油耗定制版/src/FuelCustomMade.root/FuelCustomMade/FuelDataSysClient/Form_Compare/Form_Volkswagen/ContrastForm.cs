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
using FuelDataSysClient.Tool.Tool_Volkswagen;
using FuelDataSysClient.Form_DBManager;
using FuelDataSysClient.Utils_Control;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.DevForm;

namespace FuelDataSysClient.Form_Compare.Form_Volkswagen
{
    public partial class ContrastForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        MitsUtils miutils = new MitsUtils();
        DataTableHelper dth = new DataTableHelper();
        DataTable dtCtnyPam = null;

        public ContrastForm()
        {
            InitializeComponent();
        }
        private void ContrastForm_Load(object sender, EventArgs e)
        {
            dtCtnyPam = miutils.GetCheckData();
            this.gvDataXT.OptionsBehavior.Editable = false;
            this.gvDataQY.OptionsBehavior.Editable = false;
            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");
        }

        //获取本地能源数据
        private DataTable GetLocalALL(string tableName)
        {
            string sql = String.Format(@"select * from {0} where USER_ID = '{1}' and (CLZZRQ>=#{2}#) and  (CLZZRQ<#{3}#)", tableName, Utils.userId, Convert.ToDateTime(dtStartTime.Text), Convert.ToDateTime(dtEndTime.Text).Add(new TimeSpan(24, 0, 0)));
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return ds.Tables[0].Copy();
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
            OpenFileDialog ofd = new OpenFileDialog();
            DataTable dt = null;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                VolkswagenCompareUtils volksUtils = new VolkswagenCompareUtils();
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                this.gcDataQY.DataSource = null;
                this.groupBox2.Text = "企业油耗数据";
                this.gvDataQY.Columns.Clear();
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        dt = volksUtils.ReadBackToSYS(ofd.FileName, VolkswagenCompareUtils.CTNY);
                        break;
                    case 1:
                        dt = volksUtils.ReadBackToSYS(ofd.FileName, VolkswagenCompareUtils.FCDSHHDL);
                        break;
                    case 2:
                        dt = volksUtils.ReadBackToSYS(ofd.FileName, VolkswagenCompareUtils.CDSHHDL);
                        break;
                    case 3:
                        dt = volksUtils.ReadBackToSYS(ofd.FileName, VolkswagenCompareUtils.CDD);
                        break;
                    case 4:
                        dt = volksUtils.ReadBackToSYS(ofd.FileName, VolkswagenCompareUtils.RLDC);
                        break;
                }
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataTableHelper.removeEmpty(dt);
                    this.gcDataQY.DataSource = dt;
                    this.gvDataXT.BestFitColumns();
                    this.groupBox2.Text = String.Format("企业油耗数据（共{0}条）", dt.Rows.Count);
                    this.gvDataQY.OptionsView.ColumnAutoWidth = false;
                }
            }
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
                this.xtraTabControl1.SelectedTabPageIndex = 0;
                this.gcDataXT.DataSource = null;
                this.groupBox1.Text = "系统油耗数据";
                DataTable dt = null;
                this.gvDataXT.Columns.Clear();
                switch (radioGroup1.SelectedIndex)
                {
                    case 0:
                        dt = GetLocalALL("VIEW_T_ALL");
                        break;
                    case 1:
                        dt = GetLocalALL("VIEW_T_ALL_FCDS");
                        break;
                    case 2:
                        dt = GetLocalALL("VIEW_T_ALL_CDS");
                        break;
                    case 3:
                        dt = GetLocalALL("VIEW_T_ALL_CDD");
                        break;
                    case 4:
                        dt = GetLocalALL("VIEW_T_ALL_RLDC");
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

        //数据分类
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (this.xtraTabControl1.SelectedTabPage.Text == "补传数据")   //需要补传数据
                {
                    var vins = GetCheckData();

                    if (vins != null && vins.Table.Rows.Count == 0)
                    {
                        MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var rllxName = GetRadio();
                    if (InsertFC_CLJBXX(vins, "1", rllxName))
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
                    var vins = GetCheckData();

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

                    var dt = (DataTable)gcDataQY.DataSource;
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
                        }
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
                        if (dv.Table.Columns.Contains("CT_QTXX"))
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

        //数据比对
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
                DataTable dtTable1 = (DataTable)gcDataXT.DataSource;
                DataTable dtTable2 = (DataTable)gcDataQY.DataSource;

                if (dtTable1 == null || dtTable2 == null)
                {
                    MessageBox.Show("没有需要比较的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    this.gcSupplements.DataSource = null;
                    this.gcRecall.DataSource = null;
                    this.gcModify.DataSource = null;
                    this.gvSupplements.Columns.Clear();
                    this.gvRecall.Columns.Clear();
                    this.gvModify.Columns.Clear();
                    //补传数据
                    var vinArr_GF = dtTable1.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                    var supplementsData = from d in dtTable2.AsEnumerable()
                                          where !vinArr_GF.Contains(d.Field<string>("VIN"))
                                          select d;
                    DataTable dtRetAdd = supplementsData.AsDataView().ToTable();
                    //撤销数据
                    var vinArr_SC = dtTable2.AsEnumerable().Select(d => d.Field<string>("VIN")).ToArray();
                    var recallData = from d in dtTable1.AsEnumerable()
                                     where !vinArr_SC.Contains(d.Field<string>("VIN"))
                                     select d;
                    DataTable dtRetDel = recallData.AsDataView().ToTable();
                    DataTable dtRetDiff = CompareDataTableDiff(dtTable1, dtTable2);
                    dtRetAdd.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetAdd.Columns["check"].SetOrdinal(0);
                    dtRetDel.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDel.Columns["check"].SetOrdinal(0);
                    dtRetDiff.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dtRetDiff.Columns["check"].SetOrdinal(0);
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
            var vinList = dtList.AsEnumerable().Select(c => c.Field<string>("VIN"));
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

            //存所有列名
            DataTable ColumnList = new DataTable();
            ColumnList.Columns.Add("dtHead");
            foreach (DataColumn dc in dt.Columns)
            {
                string s = dc.ColumnName;
                ColumnList.Rows.Add(dc.ColumnName);
            }
            //比较每一个表头对应的值
            foreach (DataRow dr in ColumnList.Rows)
            {
                string columnName = dr["dtHead"].ToString();
                if (columnName.Equals("USER_ID"))
                    continue;
                foreach (DataRow dr1 in listDv1)
                {
                    string vin = dr1["VIN"].ToString();
                    string val1 = dt1.AsEnumerable().Where(c => c.Field<string>("VIN") == vin).Select(c => c.Field<string>(columnName)).FirstOrDefault();
                    if (columnName.Equals("CLZZRQ"))
                    {
                        val1 = Convert.ToDateTime(val1).ToString();
                    }
                    string val2 = dt2.AsEnumerable().Where(c => c.Field<string>("VIN") == vin).Select(c => c.Field<string>(columnName)).FirstOrDefault();
                    if (columnName.Equals("CLZZRQ"))
                    {
                        val2 = Convert.ToDateTime(val2).ToString();
                    }
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
            this.gcDataXT.DataSource = null;
            this.groupBox1.Text = "系统油耗数据";
            this.gvDataXT.Columns.Clear();
            this.gcDataQY.DataSource = null;
            this.groupBox2.Text = "企业油耗数据";
            this.gvDataQY.Columns.Clear();
            this.xtraTabControl1.SelectedTabPageIndex = 0;
        }
    }
}