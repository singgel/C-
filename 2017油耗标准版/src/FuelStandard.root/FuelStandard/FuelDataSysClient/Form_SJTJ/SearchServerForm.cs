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
using FuelDataSysClient.SubForm;
using DevExpress.XtraSplashScreen;

namespace FuelDataSysClient
{
    /// <summary>
    /// 该WinForm类按照以下顺序编写方法
    /// 1：窗体内的btn按钮事件
    /// 2：操作框中的click事件
    /// 3：改窗体下的公共事件，该类事件都要添加注释
    /// </summary>
    public partial class SearchServerForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelDataService.FuelDataSysWebService service = Utils.service;
        private Dictionary<string, FuelDataModel.RllxParamEntity[]> rpeht = new Dictionary<string, FuelDataModel.RllxParamEntity[]>();
        InitDataTime initTime = new InitDataTime();

        int totalCount = 0;
        int totalTarget = 0;
        CafcService.CafcWebService cafcService = FuelDataSysClient.FuelCafc.StaticUtil.cafcService;

        List<Thread> listThread = new List<Thread>();
        int threadCount = 1;
        int pageCount = 1;
        private object lockThis = new object();
        private object lockList = new object();
        MitsUtils miutils = new MitsUtils();

        public SearchServerForm()
        {
            InitializeComponent();

            // 设置燃料类型下拉框的值
            this.SetFuelType();
            //初始化开始和结束时间
            this.dtStartTime.Text = initTime.getStartTime();
            this.dtEndTime.Text = initTime.getEndTime();
        }

        #region 页面按钮事件
        private void btnSearch_Click(object sender, EventArgs e)
        {
            // 查询服务器端的数据
            GetDataFromService(1);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbVin.Text = string.Empty;
            this.tbClxh.Text = string.Empty;
            this.tbClzl.Text = string.Empty;

            this.cbRllx.Text = string.Empty;

            RepositoryItemDateEdit de = new RepositoryItemDateEdit();
            this.dtStartTime.EditValue = de.NullDate;
            this.dtEndTime.EditValue = de.NullDate;
        }

        private void btnFirPage_Click(object sender, EventArgs e)
        {
            //首页
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) GetDataFromService(1);
        }

        private void btnPrePage_Click(object sender, EventArgs e)
        {
            //上一页
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) GetDataFromService(--pageNum);
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            //下一页
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) GetDataFromService(++pageNum);
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            //尾页
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) GetDataFromService(pageCou);
        }
        #endregion

        #region 操作栏按钮事件
        private void barBtnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectObjItem(this.gridView1, true);
        }

        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.FocusedRowHandle = 0;
            this.gridView1.FocusedColumn = this.gridView1.Columns[1];
            Utils.SelectObjItem(this.gridView1, false);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //如果还没有查询，则刷新默认首页
            try
            {
                int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
                GetDataFromService(pageNum);
            }
            catch
            {
                GetDataFromService(1);
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gridView1.PostEditor();
            List<PrintModel> printModelList = new List<PrintModel>();
            List<VehicleBasicInfo> dvList = (List<VehicleBasicInfo>)dgvCljbxx.DataSource;

            foreach (VehicleBasicInfo vbi in dvList)
            {
                if (vbi.Check == true)
                {
                    PrintModel printModel = new PrintModel();
                    printModel.Qcscqy = vbi.Qcscqy == "" ? vbi.Jkqczjxs : vbi.Qcscqy;
                    printModel.Clxh = vbi.Clxh;
                    printModel.Zczbzl = vbi.Zczbzl.ToString();
                    string strRllx = vbi.Rllx;

                    printModel.Qdxs = vbi.Qdxs;
                    printModel.Zdsjzzl = vbi.Zdsjzzl.ToString();

                    if (strRllx == "汽油" || strRllx == "柴油" || strRllx == "两用燃料" || strRllx == "双燃料" || strRllx == "气体燃料")
                    {
                        FuelDataModel.RllxParamEntity[] rpelist = this.rpeht[vbi.Vin];
                        for (int i = 0; rpelist != null && i < rpelist.Length; i++)
                        {
                            FuelDataModel.RllxParamEntity rpe = rpelist[i];
                            string strCode = rpe.Param_Code;
                            string strValue = rpe.Param_Value;
                            if (strCode == "CT_FDJXH")
                            {
                                printModel.Fdjxh = strValue;
                            }
                            if (strCode == "CT_PL")
                            {
                                printModel.Pl = strValue;
                            }
                            if (strCode == "CT_BSQXS")
                            {
                                printModel.Bsqlx = strValue;
                            }
                            if (strCode == "CT_QTXX")
                            {
                                printModel.Qtxx = strValue;
                            }
                            if (strCode == "CT_EDGL")
                            {
                                printModel.Edgl = strValue;
                            }
                            if (strCode == "CT_SJGKRLXHL")
                            {
                                printModel.Sj = strValue;
                            }
                            if (strCode == "CT_SQGKRLXHL")
                            {
                                printModel.Sq = strValue;
                            }
                            if (strCode == "CT_ZHGKRLXHL")
                            {
                                printModel.Zh = strValue;
                            }
                        }
                    }
                    printModel.Rllx = strRllx;

                    printModel.Bah = vbi.Vin;
                    printModel.Qysj = DateTime.Now.ToShortDateString();
                    printModelList.Add(printModel);
                }
            }
            if (printModelList.Count == 0)
            {
                MessageBox.Show("请选中要打印的行！");
                return;
            }

            Utils.printModel = printModelList;
            PrintForm pf = new PrintForm();
            pf.ShowDialog();
        }

        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                StringBuilder sqlWhere = new StringBuilder();
                if (!string.IsNullOrEmpty(this.tbVin.Text))
                {
                    sqlWhere.Append(string.Format(@" AND VIN LIKE '%{0}%' ", this.tbVin.Text));
                }
                if (!string.IsNullOrEmpty(this.tbClzl.Text))
                {
                    sqlWhere.Append(string.Format(@" AND CLZL LIKE '%{0}%' ", this.tbClzl.Text));
                }
                if (!string.IsNullOrEmpty(this.tbClxh.Text))
                {
                    sqlWhere.Append(string.Format(@" AND CLXH LIKE '%{0}%' ", this.tbClxh.Text));
                }
                if (!string.IsNullOrEmpty(this.cbRllx.Text))
                {
                    sqlWhere.Append(string.Format(@" AND RLLX LIKE '%{0}%' ", this.cbRllx.Text));
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
                {
                    sqlWhere.Append(string.Format(@" AND UPDATETIME >= #{0}# AND UPDATETIME <= #{1}# ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("车辆制造日期/进口核销日期"))
                {
                    sqlWhere.Append(string.Format(@" AND CLZZRQ >= #{0}# AND CLZZRQ<= #{1}# ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
                }
                DataSet dsExport = new DataSet();
                // 丰田中国
                if (Utils.userId.Equals("FADCFFTZGU001"))
                {

                    string sqlVins = string.Format(@"select VIN,RLLX from FC_CLJBXX_ADC where 1=1 {0} ", sqlWhere.ToString());
                    string sqlVinsCTNY = string.Format(@"select VIN from ({0}) where RLLX in ('汽油','柴油','两用燃料','双燃料','气体燃料') ", sqlVins);
                    string sqlVinsFCDS = string.Format(@"select VIN from ({0}) where RLLX = '非插电式混合动力' ", sqlVins);
                    string sqlStrCTNY = string.Format(@"select * from VIEW_T_ALL_ADC where VIN in({0}) ", sqlVinsCTNY);
                    string sqlStrFCDS = string.Format(@"select * from VIEW_T_ALL_FCDS_ADC where VIN in({0}) ", sqlVinsFCDS);
                    DataSet dsCTNY = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrCTNY, null);
                    DataSet dsFCDS = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrFCDS, null);
                    if (dsCTNY != null && dsCTNY.Tables.Count > 0 && dsCTNY.Tables[0] != null && dsCTNY.Tables[0].Rows.Count > 0)
                    {
                        dsCTNY.Tables[0].TableName = "传统能源";
                        dsExport.Tables.Add(dsCTNY.Tables[0].Copy());
                    }
                    if (dsFCDS != null && dsFCDS.Tables.Count > 0 && dsFCDS.Tables[0] != null && dsFCDS.Tables[0].Rows.Count > 0)
                    {
                        dsFCDS.Tables[0].TableName = "非插电式混合动力";
                        dsExport.Tables.Add(dsFCDS.Tables[0].Copy());
                    }
                }
                else
                {

                    string sqlVins = string.Format(@"select VIN,RLLX from FC_CLJBXX where 1=1 {0} ", sqlWhere.ToString());
                    string sqlVinsCTNY = string.Format(@"select VIN from ({0}) where RLLX in ('汽油','柴油','两用燃料','双燃料','气体燃料') ", sqlVins);
                    string sqlVinsFCDS = string.Format(@"select VIN from ({0}) where RLLX = '非插电式混合动力' ", sqlVins);
                    string sqlVinsCDS = string.Format(@"select VIN from ({0}) where RLLX = '插电式混合动力' ", sqlVins);
                    string sqlVinsCDD = string.Format(@"select VIN from ({0}) where RLLX = '纯电动' ", sqlVins);
                    string sqlVinsRLDC = string.Format(@"select VIN from ({0}) where RLLX = '燃料电池' ", sqlVins);
                    string sqlStrCTNY = string.Format(@"select * from VIEW_T_ALL where VIN in({0}) ", sqlVinsCTNY);
                    string sqlStrFCDS = string.Format(@"select * from VIEW_T_ALL_FCDS where VIN in({0}) ", sqlVinsFCDS);
                    string sqlStrCDS = string.Format(@"select * from VIEW_T_ALL_CDS where VIN in({0}) ", sqlVinsCDS);
                    string sqlStrCDD = string.Format(@"select * from VIEW_T_ALL_CDD where VIN in({0}) ", sqlVinsCDD);
                    string sqlStrRLDC = string.Format(@"select * from VIEW_T_ALL_RLDC where VIN in({0}) ", sqlVinsRLDC);
                    DataSet dsCTNY = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrCTNY, null);
                    DataSet dsFCDS = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrFCDS, null);
                    DataSet dsCDS = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrCDS, null);
                    DataSet dsCDD = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrCDD, null);
                    DataSet dsRLDC = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStrRLDC, null);
                    if (dsCTNY != null && dsCTNY.Tables.Count > 0 && dsCTNY.Tables[0] != null && dsCTNY.Tables[0].Rows.Count > 0)
                    {
                        dsCTNY.Tables[0].TableName = "传统能源";
                        dsExport.Tables.Add(dsCTNY.Tables[0].Copy());
                    }
                    if (dsFCDS != null && dsFCDS.Tables.Count > 0 && dsFCDS.Tables[0] != null && dsFCDS.Tables[0].Rows.Count > 0)
                    {
                        dsFCDS.Tables[0].TableName = "非插电式混合动力";
                        dsExport.Tables.Add(dsFCDS.Tables[0].Copy());
                    }
                    if (dsCDS != null && dsCDS.Tables.Count > 0 && dsCDS.Tables[0] != null && dsCDS.Tables[0].Rows.Count > 0)
                    {
                        dsCDS.Tables[0].TableName = "插电式混合动力";
                        dsExport.Tables.Add(dsCDS.Tables[0].Copy());
                    }
                    if (dsCDD != null && dsCDD.Tables.Count > 0 && dsCDD.Tables[0] != null && dsCDD.Tables[0].Rows.Count > 0)
                    {
                        dsCDD.Tables[0].TableName = "纯电动";
                        dsExport.Tables.Add(dsCDD.Tables[0].Copy());
                    }
                    if (dsRLDC != null && dsRLDC.Tables.Count > 0 && dsRLDC.Tables[0] != null && dsRLDC.Tables[0].Rows.Count > 0)
                    {
                        dsRLDC.Tables[0].TableName = "燃料电池";
                        dsExport.Tables.Add(dsRLDC.Tables[0].Copy());
                    }
                }
                if (dsExport.Tables.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    string filePath = folderBrowserDialog.SelectedPath;
                    ExportToExcel toExcel = new ExportToExcel();
                    for (int i = 0; i < dsExport.Tables.Count; i++)
                    {
                        toExcel.ExportExcel(filePath, dsExport.Tables[i]);
                    }
                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SplashScreenManager.CloseForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        /// <summary>
        /// 统计下载数量
        /// </summary>
        /// <returns></returns>
        private int DownLoadDataCount()
        {
            try
            {
                StringBuilder sqlWhere = new StringBuilder();
                if (!string.IsNullOrEmpty(this.tbVin.Text))
                {
                    sqlWhere.Append(string.Format(@" AND VIN LIKE '%{0}%' ", this.tbVin.Text));
                }
                if (!string.IsNullOrEmpty(this.tbClzl.Text))
                {
                    sqlWhere.Append(string.Format(@" AND CLZL LIKE '%{0}%' ", this.tbClzl.Text));
                }
                if (!string.IsNullOrEmpty(this.tbClxh.Text))
                {
                    sqlWhere.Append(string.Format(@" AND CLXH LIKE '%{0}%' ", this.tbClxh.Text));
                }
                if (!string.IsNullOrEmpty(this.cbRllx.Text))
                {
                    sqlWhere.Append(string.Format(@" AND RLLX LIKE '%{0}%' ", this.cbRllx.Text));
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("上报日期"))
                {
                    sqlWhere.Append(string.Format(@" AND UPDATETIME >= #{0}# AND UPDATETIME <= #{1}# ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
                }
                if (!string.IsNullOrEmpty(this.dtStartTime.Text) && !string.IsNullOrEmpty(this.dtEndTime.Text) && this.cbTimeType.Text.Trim().Equals("车辆制造日期/进口核销日期"))
                {
                    sqlWhere.Append(string.Format(@" AND CLZZRQ >= #{0}# AND CLZZRQ<= #{1}# ", Convert.ToDateTime(this.dtStartTime.Text), Convert.ToDateTime(this.dtEndTime.Text)));
                }
                string sqlVins = string.Empty;
                // 丰田中国
                if (Utils.userId.Equals("FADCFFTZGU001"))
                {
                    sqlVins = string.Format(@"select count(*) from FC_CLJBXX_ADC where USER_ID='{0}' {1}  AND STATUS = '0'", Utils.userId, sqlWhere);
                }
                else
                {
                    sqlVins = string.Format(@"select count(*) from FC_CLJBXX where USER_ID='{0}' {1}  AND STATUS = '0'", Utils.userId, sqlWhere);
                }
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlVins, null);
                DataTable dt = ds.Tables[0];
                return Int32.Parse(dt.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region 该窗体的公共方法
        /// <summary>
        /// 修改车辆基本信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCljbxx_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)dgvCljbxx.FocusedView;
            FuelDataModel.VehicleBasicInfo vbi = (FuelDataModel.VehicleBasicInfo)cv.GetFocusedRow();

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
        /// <summary>
        /// 给窗体-控件-赋值-是否可以编辑
        /// </summary>
        /// <param name="jvf"></param>
        /// <param name="cName"></param>
        /// <param name="val"></param>
        /// <param name="enable"></param>
        public void setControlValue(JbxxViewForm jvf, string cName, String val, bool enable)
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
                        || cb.Text == "双燃料" || cb.Text == "气体燃料" || cb.Text == "纯电动" || cb.Text == "非插电式混合动力" || cb.Text == "插电式混合动力" || cb.Text == "燃料电池")
                    {
                        string rlval = cb.Text;
                        if (cb.Text == "汽油" || cb.Text == "柴油" || cb.Text == "两用燃料"
                        || cb.Text == "双燃料" || cb.Text == "气体燃料")
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
        /// <summary>
        /// 查询远程信息
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
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
                int dataCount = service.QueryUploadedFuelDataCount(Utils.userId, Utils.password, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                FuelDataService.VehicleBasicInfo[] queryInfoArr = service.QueryUploadedFuelData_New(Utils.userId, Utils.password, pageNum, pageSize, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
                if ((queryInfoArr != null && dataCount > 0) || (queryInfoArr == null && dataCount == 0))
                {
                    if (queryInfoArr != null)
                    {
                        List<FuelDataModel.VehicleBasicInfo> vbis = Utils.FuelInfoS2C(queryInfoArr);

                        for (int i = 0; i < vbis.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(vbis[i].Jkqczjxs))
                                vbis[i].Jkqczjxs = Utils.qymc;
                            FuelDataModel.VehicleBasicInfo vbi = vbis[i];
                            if (rpeht.ContainsKey(vbi.Vin))
                            {
                                rpeht.Remove(vbi.Vin);
                            }
                            rpeht.Add(vbi.Vin, vbi.EntityList);
                            vbi.EntityList = null;
                        }
                        dgvCljbxx.DataSource = vbis;
                    }
                    else
                    {
                        dgvCljbxx.DataSource = null;
                    }
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
        /// <summary>
        /// 将远程查到的数据同步到本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsSaveToLocal_Click(object sender, EventArgs e)
        {
            // 丰田中国
            if (Utils.userId.Equals("FADCFFTZGU001"))
            {
                saveToLocal_ADC();
            }
            else
            {
                saveToLocal();
            }
        }

        private void saveToLocal()
        {
            ColumnView cv = (ColumnView)dgvCljbxx.FocusedView;
            FuelDataModel.VehicleBasicInfo vbi = (FuelDataModel.VehicleBasicInfo)cv.GetFocusedRow();

            if (vbi == null)
            {
                MessageBox.Show("请选中要保存的行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FuelDataModel.RllxParamEntity[] rpelist = this.rpeht[vbi.Vin];

            #region 收集车辆基本信息
            string sqlSaveBasic = @"INSERT INTO FC_CLJBXX
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
                                (   @V_ID,
                                    @VIN,
                                    @HGSPBM,
                                    @USER_ID,
                                    @QCSCQY,
                                    @JKQCZJXS,
                                    @CLXH,
                                    @CLZL,
                                    @RLLX,
                                    @ZCZBZL,
                                    @ZGCS,
                                    @LTGG,
                                    @ZJ,
                                    @CLZZRQ,
                                    @UPLOADDEADLINE,
                                    @TYMC,
                                    @YYC,
                                    @ZWPS,
                                    @ZDSJZZL,
                                    @EDZK,
                                    @LJ,
                                    @QDXS,
                                    @STATUS,
                                    @JYJGMC,
                                    @JYBGBH,
                                    @QTXX,
                                    @CREATETIME,
                                    @UPDATETIME)";

            MitsUtils util = new MitsUtils();
            string mainId = util.GetMainIdFromVinData(vbi.Vin);

            OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", vbi.Clzzrq);
            clzzrq.OleDbType = OleDbType.DBDate;

            DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(vbi.Clzzrq);
            OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
            uploadDeadline.OleDbType = OleDbType.DBDate;

            OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
            creTime.OleDbType = OleDbType.DBDate;
            OleDbParameter upTime = new OleDbParameter("@UPDATETIME", vbi.CreateTime);
            upTime.OleDbType = OleDbType.DBDate;

            OleDbParameter[] param = { 
                                     new OleDbParameter("@V_ID",vbi.V_Id), 
                                     new OleDbParameter("@VIN",vbi.Vin),
                                     new OleDbParameter("@HGSPBM",string.IsNullOrEmpty(vbi.Hgspbm)?"":vbi.Hgspbm),
                                     new OleDbParameter("@USER_ID",Utils.userId),
                                     new OleDbParameter("@QCSCQY",vbi.Qcscqy),
                                     new OleDbParameter("@JKQCZJXS",string.IsNullOrEmpty(vbi.Jkqczjxs)?"":Utils.qymc),
                                     new OleDbParameter("@CLXH",vbi.Clxh),
                                     new OleDbParameter("@CLZL",vbi.Clzl),
                                     new OleDbParameter("@RLLX",vbi.Rllx),
                                     new OleDbParameter("@ZCZBZL",vbi.Zczbzl),
                                     new OleDbParameter("@ZGCS",vbi.Zgcs),
                                     new OleDbParameter("@LTGG",vbi.Ltgg),
                                     new OleDbParameter("@ZJ",vbi.Zj),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@TYMC",vbi.Tymc),
                                     new OleDbParameter("@YYC",vbi.Yyc),
                                     new OleDbParameter("@ZWPS",vbi.Zwps),
                                     new OleDbParameter("@ZDSJZZL",vbi.Zdsjzzl),
                                     new OleDbParameter("@EDZK",vbi.Edzk),
                                     new OleDbParameter("@LJ",vbi.Lj),
                                     new OleDbParameter("@QDXS",vbi.Qdxs),
                                     new OleDbParameter("@STATUS","0"),
                                     new OleDbParameter("@JYJGMC",vbi.Jyjgmc),
                                     new OleDbParameter("@JYBGBH",vbi.Jybgbh),
                                     new OleDbParameter("@QTXX",string.IsNullOrEmpty(vbi.Qtxx)?"":vbi.Qtxx),
                                     creTime,
                                     upTime
                                     };
            #endregion

            string sqlDelBasic = string.Format(@"DELETE FROM FC_CLJBXX WHERE VIN='{0}'", vbi.Vin);
            string sqlDelParams = string.Format(@"DELETE FROM RLLX_PARAM_ENTITY WHERE VIN='{0}'", vbi.Vin);

            string strCon = AccessHelper.conn;
            using (OleDbConnection con = new OleDbConnection(strCon))
            {
                con.Open();
                OleDbTransaction trans = con.BeginTransaction();
                try
                {
                    // 删除本地车辆基本信息
                    AccessHelper.ExecuteNonQuery(trans, sqlDelBasic, null);

                    // 保存服务器端基本信息到本地
                    AccessHelper.ExecuteNonQuery(trans, sqlSaveBasic, param);

                    // 删除本地燃料参数信息
                    AccessHelper.ExecuteNonQuery(trans, sqlDelParams, null);
                    string sqlSaveParams = string.Empty;

                    #region 保存服务器端燃料参数信息到本地
                    foreach (RllxParamEntity entity in rpelist)
                    {
                        sqlSaveParams = @"INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                VALUES(@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";

                        OleDbParameter[] paramList = { 
                                    new OleDbParameter("@PARAM_CODE",entity.Param_Code),
                                    new OleDbParameter("@VIN",entity.Vin),
                                    new OleDbParameter("@PARAM_VALUE",entity.Param_Value),
                                    new OleDbParameter("@V_ID",entity.V_Id)
                                };
                        AccessHelper.ExecuteNonQuery(trans, sqlSaveParams, paramList);
                    }
                    #endregion

                    trans.Commit();
                    MessageBox.Show("同步成功");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("同步失败：" + ex.Message);
                    return;
                }
            }
        }
        private void saveToLocal_ADC()
        {
            ColumnView cv = (ColumnView)dgvCljbxx.FocusedView;
            FuelDataModel.VehicleBasicInfo vbi = (FuelDataModel.VehicleBasicInfo)cv.GetFocusedRow();

            if (vbi == null)
            {
                MessageBox.Show("请选中要保存的行", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FuelDataModel.RllxParamEntity[] rpelist = this.rpeht[vbi.Vin];

            #region 收集车辆基本信息
            string sqlSaveBasic = @"INSERT INTO FC_CLJBXX_ADC
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
                                (   @V_ID,
                                    @VIN,
                                    @HGSPBM,
                                    @USER_ID,
                                    @QCSCQY,
                                    @JKQCZJXS,
                                    @CLXH,
                                    @CLZL,
                                    @RLLX,
                                    @ZCZBZL,
                                    @ZGCS,
                                    @LTGG,
                                    @ZJ,
                                    @CLZZRQ,
                                    @UPLOADDEADLINE,
                                    @TYMC,
                                    @YYC,
                                    @ZWPS,
                                    @ZDSJZZL,
                                    @EDZK,
                                    @LJ,
                                    @QDXS,
                                    @STATUS,
                                    @JYJGMC,
                                    @JYBGBH,
                                    @QTXX,
                                    @CREATETIME,
                                    @UPDATETIME)";

            MitsUtils util = new MitsUtils();
            string mainId = util.GetMainIdFromVinData(vbi.Vin);

            OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", vbi.Clzzrq);
            clzzrq.OleDbType = OleDbType.DBDate;

            DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(vbi.Clzzrq);
            OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
            uploadDeadline.OleDbType = OleDbType.DBDate;

            OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
            creTime.OleDbType = OleDbType.DBDate;
            OleDbParameter upTime = new OleDbParameter("@UPDATETIME", vbi.CreateTime);
            upTime.OleDbType = OleDbType.DBDate;

            OleDbParameter[] param = { 
                                     new OleDbParameter("@V_ID",vbi.V_Id), 
                                     new OleDbParameter("@VIN",vbi.Vin),
                                     new OleDbParameter("@HGSPBM",string.IsNullOrEmpty(vbi.Hgspbm)?"":vbi.Hgspbm),
                                     new OleDbParameter("@USER_ID",Utils.userId),
                                     new OleDbParameter("@QCSCQY",vbi.Qcscqy),
                                     new OleDbParameter("@JKQCZJXS",string.IsNullOrEmpty(vbi.Jkqczjxs)?"":Utils.qymc),
                                     new OleDbParameter("@CLXH",vbi.Clxh),
                                     new OleDbParameter("@CLZL",vbi.Clzl),
                                     new OleDbParameter("@RLLX",vbi.Rllx),
                                     new OleDbParameter("@ZCZBZL",vbi.Zczbzl),
                                     new OleDbParameter("@ZGCS",vbi.Zgcs),
                                     new OleDbParameter("@LTGG",vbi.Ltgg),
                                     new OleDbParameter("@ZJ",vbi.Zj),
                                     clzzrq,
                                     uploadDeadline,
                                     new OleDbParameter("@TYMC",vbi.Tymc),
                                     new OleDbParameter("@YYC",vbi.Yyc),
                                     new OleDbParameter("@ZWPS",vbi.Zwps),
                                     new OleDbParameter("@ZDSJZZL",vbi.Zdsjzzl),
                                     new OleDbParameter("@EDZK",vbi.Edzk),
                                     new OleDbParameter("@LJ",vbi.Lj),
                                     new OleDbParameter("@QDXS",vbi.Qdxs),
                                     new OleDbParameter("@STATUS","0"),
                                     new OleDbParameter("@JYJGMC",vbi.Jyjgmc),
                                     new OleDbParameter("@JYBGBH",vbi.Jybgbh),
                                     new OleDbParameter("@QTXX",string.IsNullOrEmpty(vbi.Qtxx)?"":vbi.Qtxx),
                                     creTime,
                                     upTime
                                     };
            #endregion

            string sqlDelBasic = string.Format(@"DELETE FROM FC_CLJBXX_ADC WHERE VIN='{0}'", vbi.Vin);
            string sqlDelParams = string.Format(@"DELETE FROM RLLX_PARAM_ENTITY_ADC WHERE VIN='{0}'", vbi.Vin);

            string strCon = AccessHelper.conn;
            using (OleDbConnection con = new OleDbConnection(strCon))
            {
                con.Open();
                OleDbTransaction trans = con.BeginTransaction();
                try
                {
                    // 删除本地车辆基本信息
                    AccessHelper.ExecuteNonQuery(trans, sqlDelBasic, null);

                    // 保存服务器端基本信息到本地
                    AccessHelper.ExecuteNonQuery(trans, sqlSaveBasic, param);

                    // 删除本地燃料参数信息
                    AccessHelper.ExecuteNonQuery(trans, sqlDelParams, null);
                    string sqlSaveParams = string.Empty;

                    #region 保存服务器端燃料参数信息到本地
                    foreach (RllxParamEntity entity in rpelist)
                    {
                        sqlSaveParams = @"INSERT INTO RLLX_PARAM_ENTITY_ADC (PARAM_CODE,VIN,PARAM_VALUE,V_ID) 
                                VALUES(@PARAM_CODE,@VIN,@PARAM_VALUE,@V_ID)";

                        OleDbParameter[] paramList = { 
                                    new OleDbParameter("@PARAM_CODE",entity.Param_Code),
                                    new OleDbParameter("@VIN",entity.Vin),
                                    new OleDbParameter("@PARAM_VALUE",entity.Param_Value),
                                    new OleDbParameter("@V_ID",entity.V_Id)
                                };
                        AccessHelper.ExecuteNonQuery(trans, sqlSaveParams, paramList);
                    }
                    #endregion

                    trans.Commit();
                    MessageBox.Show("同步成功");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show("同步失败：" + ex.Message);
                    return;
                }
            }
        }
        /// <summary>
        /// 设置燃料类型下拉框的值
        /// </summary>
        protected void SetFuelType()
        {
            List<string> fuelTypeList = Utils.GetFuelType("SEARCH");
            this.cbRllx.Properties.Items.AddRange(fuelTypeList.ToArray());
        }

        /// <summary>
        /// 获取模糊查询中的时间类型
        /// </summary>
        /// <returns></returns>
        protected string GetTimeType()
        {
            // 查询日期类型：
            // UPLOAD_TIME 表示上报日期
            // MANUFACTURE_TIME 表示车辆制造日期/进口核销日期
            string timeType = "UPLOAD_TIME";
            if (cbTimeType.Text == "车辆制造日期/进口核销日期")
            {
                timeType = "MANUFACTURE_TIME";
            }
            return timeType;
        }

        ////数据同步
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DateTime start = DateTime.Now;
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //数量
                this.totalTarget = service.QueryUploadedFuelDataCount(Utils.userId, Utils.password, tbVin.Text, tbClxh.Text, tbClzl.Text, cbRllx.Text, dtStartTime.Text, dtEndTime.Text, this.GetTimeType());
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
                MessageBox.Show(ex.StackTrace + ex.Message);
            }
            finally
            {
                DateTime end = DateTime.Now;
                TimeSpan ts = end.Subtract(start).Duration();
                string dateDiff = String.Format("{0}天{1}小时{2}分钟{3}秒", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                this.pageCount = 1;
                //关闭登录提示画面
                SplashScreenManager.CloseForm();
                int localCount = DownLoadDataCount();
                MessageBox.Show(String.Format("同步完成,消耗时间{0},应同步{1},实际同步为{2}", dateDiff,this.totalCount, localCount));
                this.totalCount = 0;
                this.totalTarget = 0;
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
                    string timeType = this.GetTimeType();

                    var fuelData = service.QueryUploadedFuelData_New(Utils.userId, Utils.password, page, 100, tbVin.Text, this.tbClxh.Text, tbClzl.Text, cbRllx.Text, this.dtStartTime.Text, this.dtEndTime.Text, timeType);
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
                try
                {
                    foreach (var serverItem in vbInfo)
                    {
                        // 丰田中国
                        //if (Utils.userId.Equals("FADCFFTZGU001"))
                        //{
                        //    InsertData_ADC(serverItem);
                        //}
                        //else
                        //{
                            InsertData(serverItem);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace + ex.Message);
                    var aa = ex.Message;
                }
                return result;
            }
        }

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

                    string sqlDelCljbxx = "DELETE FROM FC_CLJBXX WHERE VIN ='" + vbInfo.Vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDelCljbxx, null);

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

                    OleDbParameter creTime = new OleDbParameter("@CREATETIME", vbInfo.CreateTime);
                    creTime.OleDbType = OleDbType.DBDate;
                    OleDbParameter upTime = new OleDbParameter("@UPDATETIME", vbInfo.UpdateTime);
                    upTime.OleDbType = OleDbType.DBDate;

                    OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",vbInfo.Vin),
                                     
                                     new OleDbParameter("@USER_ID",Utils.userId),
                                     new OleDbParameter("@QCSCQY",vbInfo.Qcscqy),
                                     new OleDbParameter("@JKQCZJXS",string.IsNullOrEmpty(vbInfo.Jkqczjxs)?"":Utils.qymc),
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
                                     new OleDbParameter("@HGSPBM",string.IsNullOrEmpty(vbInfo.Hgspbm)?"":vbInfo.Hgspbm),
                                     new OleDbParameter("@QTXX",string.IsNullOrEmpty(vbInfo.Qtxx)?"":vbInfo.Qtxx),
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

        private void InsertData_ADC(FuelDataService.VehicleBasicInfo vbInfo)
        {
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                OleDbTransaction tra = null; //创建事务，开始执行事务
                try
                {
                    #region 待生成的燃料基本信息数据存入燃料基本信息表

                    tra = con.BeginTransaction();

                    string sqlDelCljbxx = "DELETE FROM FC_CLJBXX_ADC WHERE VIN ='" + vbInfo.Vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDelCljbxx, null);

                    string sqlInsertBasic = @"INSERT INTO FC_CLJBXX_ADC
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

                    OleDbParameter creTime = new OleDbParameter("@CREATETIME", vbInfo.CreateTime);
                    creTime.OleDbType = OleDbType.DBDate;
                    OleDbParameter upTime = new OleDbParameter("@UPDATETIME", vbInfo.UpdateTime);
                    upTime.OleDbType = OleDbType.DBDate;

                    OleDbParameter[] param = { 
                                     new OleDbParameter("@VIN",vbInfo.Vin),
                                     
                                     new OleDbParameter("@USER_ID",Utils.userId),
                                     new OleDbParameter("@QCSCQY",vbInfo.Qcscqy),
                                     new OleDbParameter("@JKQCZJXS",string.IsNullOrEmpty(vbInfo.Jkqczjxs)?"":Utils.qymc),
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
                                     new OleDbParameter("@HGSPBM",string.IsNullOrEmpty(vbInfo.Hgspbm)?"":vbInfo.Hgspbm),
                                     new OleDbParameter("@QTXX",string.IsNullOrEmpty(vbInfo.Qtxx)?"":vbInfo.Qtxx),
                                     // 状态为9表示数据以导入，但未被激活，此时用来供用户修改
                                     new OleDbParameter("@STATUS","0"),
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@V_ID",vbInfo.V_Id)
                                     };
                    AccessHelper.ExecuteNonQuery(tra, sqlInsertBasic, param);

                    #endregion

                    #region 插入参数信息

                    string sqlDelParam = "DELETE FROM RLLX_PARAM_ENTITY_ADC WHERE VIN ='" + vbInfo.Vin + "'";
                    AccessHelper.ExecuteNonQuery(tra, sqlDelParam, null);

                    // 待生成的燃料参数信息存入燃料参数表
                    foreach (var drParam in vbInfo.EntityList)
                    {
                        string sqlInsertParam = @"INSERT INTO RLLX_PARAM_ENTITY_ADC 
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
        /// 获取泛型集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="connStr">数据库连接字符串</param>
        /// <param name="sqlStr">要查询的T-SQL</param>
        /// <returns></returns>
        public IList<T> GetList<T>(string sqlStr)
        {

            DataSet ds = new DataSet();
            ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStr, null);
            return DataSetToList<T>(ds, 0);

        }

        /// <summary>
        /// DataSetToList
        /// </summary>
        /// <typeparam name="T">转换类型</typeparam>
        /// <param name="dataSet">数据源</param>
        /// <param name="tableIndex">需要转换表的索引</param>
        /// <returns></returns>
        public IList<T> DataSetToList<T>(DataSet dataSet, int tableIndex)
        {
            //确认参数有效
            if (dataSet == null || dataSet.Tables.Count <= 0 || tableIndex < 0)
                return null;

            DataTable dt = dataSet.Tables[tableIndex];

            IList<T> list = new List<T>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //创建泛型对象
                T _t = Activator.CreateInstance<T>();
                //获取对象所有属性
                PropertyInfo[] propertyInfo = _t.GetType().GetProperties();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    foreach (PropertyInfo info in propertyInfo)
                    {
                        //属性名称和列名相同时赋值
                        if (dt.Columns[j].ColumnName.ToUpper().Equals(info.Name.ToUpper()))
                        {
                            if (dt.Rows[i][j] != DBNull.Value)
                            {
                                //if (info.PropertyType == typeof(System.Nullable<System.DateTime>))
                                //{
                                //    info.SetValue(_t, Convert.ToDateTime(dt.Rows[i][j].ToString()), null);
                                //}
                                //else
                                //{
                                info.SetValue(_t, Convert.ChangeType(dt.Rows[i][j], info.PropertyType), null);
                                //}
                                //info.SetValue(_t, dt.Rows[i][j], null);
                            }
                            else
                            {
                                info.SetValue(_t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(_t);
            }
            return list;
        }
        #endregion
    }
}