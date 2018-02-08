using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.IO;
using DevExpress.XtraEditors.Repository;
using System.Data.OleDb;
using FuelDataSysClient.Tool;
using DevExpress.XtraPrinting;
using FuelDataSysClient.SubForm;
using DevExpress.XtraSplashScreen;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class ImportRLLXPARAMForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportRLLXPARAMForm()
        {
            InitializeComponent();
            this.cbRllx.Properties.Items.AddRange(Utils.GetFuelRLLX("SEARCH").ToArray());
        }

        // 导入Excel
        private void barBtnImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Dictionary<string, string> error = new Dictionary<string, string>();
                string msg = string.Empty;
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    // STEP1：导入系统，验证单元格格式
                    var ds = ImportExcel.ReadExcelToDataSet(openFileDialog1.FileName);
                    for (int i = 0; i < ds.Tables["TEMPLATE"].Columns.Count; i++)
                    {
                        for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                        {
                            if (ds.Tables[0].Rows[j][i].GetType() != typeof(System.DBNull))
                            {
                                if (ds.Tables[0].Rows[j][i].GetType() != typeof(System.String))
                                {
                                    MessageBox.Show(String.Format("【{1}】列中第【{0}】行的单元格格式不正确，应为文本格式!", j + 2, ds.Tables[0].Columns[i].ColumnName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }
                        }
                    }
                    // STEP2：替换列名，验证参数的数值
                    var dt = ImportExcel.SwitchRLLXPARAMColumnName(ds);
                    if (dt != null)
                    {
                        string sc_ocn_error = string.Empty;
                        error = DataVerifyHelper.VerifyRLLXPARAMData(dt);
                        if (error.Count == 0)
                        {
                            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                            {
                                conn.Open();
                                using (OracleTransaction trans = conn.BeginTransaction())
                                {
                                    // STEP3：验证无误，导入系统数据库
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        if (sc_ocn_error.Equals(dr["SC_OCN"].ToString())) continue;
                                        try
                                        {
                                            string exist = OracleHelper.ExecuteScalar(trans, string.Format("SELECT COUNT(*) FROM OCN_RLLX_PARAM_ENTITY WHERE OPERATION!='4' AND SC_OCN='{0}' AND CSBM='{1}'", dr["SC_OCN"], dr["CSBM"])).ToString();
                                            int existNum = string.IsNullOrEmpty(exist) ? 0 : Convert.ToInt32(exist);
                                            if (existNum > 0)
                                            {
                                                error.Add(dr["SC_OCN"].ToString().Trim(), "系统已经存在改生产OCN的燃料参数数据！");
                                                sc_ocn_error = dr["SC_OCN"].ToString();
                                                continue;
                                            }
                                            OracleParameter[] parameters = 
                                            {
				                                new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CSBM", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CSMC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CSZ", OracleDbType.NVarchar2,255),
				                                new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                                new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                                new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                                new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                                new OracleParameter("VERSION", OracleDbType.Int32),
                                            };
                                            parameters[0].Value = dr["SC_OCN"];
                                            parameters[1].Value = dr["CSBM"];
                                            parameters[2].Value = dr["CSMC"];
                                            parameters[3].Value = dr["RLLX"];
                                            parameters[4].Value = dr["CSZ"];
                                            parameters[5].Value = "0";
                                            parameters[6].Value = System.DateTime.Today;
                                            parameters[7].Value = Utils.localUserId;
                                            parameters[8].Value = System.DateTime.Today;
                                            parameters[9].Value = Utils.localUserId;
                                            parameters[10].Value = 0;
                                            OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_RLLX_PARAM_ENTITY (SC_OCN,CSBM,CSMC,RLLX,CSZ,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:CSBM,:CSMC,:RLLX,:CSZ,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
                                        }
                                        catch (Exception ex)
                                        {
                                            error.Add(String.Format("{0} {0}", dr["SC_OCN"], dr["CSBM"]), ex.Message);
                                            sc_ocn_error = dr["SC_OCN"].ToString();
                                            continue;
                                        }
                                    }
                                    if (trans.Connection != null) trans.Commit();
                                }
                            }
                            // STEP4：处理无误，处理完成的文件
                            if (error.Count == 0)
                            {
                                var destFolder = Path.Combine(Path.GetDirectoryName(openFileDialog1.FileName), DateTime.Today.ToLongDateString() + "-燃料参数数据-Imported");
                                Directory.CreateDirectory(destFolder);
                                try
                                {
                                    File.Move(openFileDialog1.FileName, Path.Combine(destFolder, String.Format("Imported-{0}{1}", Path.GetFileNameWithoutExtension(openFileDialog1.FileName), Path.GetExtension(openFileDialog1.FileName))));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(String.Format("Excel处理操作异常：导入完成，{0}", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                        foreach (KeyValuePair<string, string> kvp in error)
                        {
                            msg += String.Format("{0}\r\n{1}\r\n", kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excel导入操作异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
                MessageForm msgForm = new MessageForm(msg + String.Format("\r\n{0}Excel导入操作完成", Path.GetFileNameWithoutExtension(openFileDialog1.FileName))) { Text = "燃料参数导入信息" };
                msgForm.Show();
                SearchLocal(1);
            }
        }

        // 新增
        private void barBtnAdd_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (RLLXParamForm rllxParamForm = new RLLXParamForm())
            {
                rllxParamForm.ShowDialog();
                if (rllxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
            }
        }

        // 复制新增
        private void barBtnCopy_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.gvOCN_RLLXPARAM.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvOCN_RLLXPARAM.PostEditor();
            var dataSource = (DataView)this.gvOCN_RLLXPARAM.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                    }
                }
            }
            if (dtSelected.AsDataView().ToTable(true, new string[] { "SC_OCN", "VERSION" }).Rows.Count != 1)
            {
                MessageBox.Show(String.Format("每次只能操作一组记录，您选择了{0}组！", dtSelected.AsDataView().ToTable(true, new string[] { "SC_OCN" }).Rows.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dtSelected.Rows[0]["OPERATION"].Equals("4"))
            {
                MessageBox.Show(String.Format("OCN为{0}：已经删除，无法进行此操作！", dtSelected.Rows[0]["SC_OCN"]), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RLLXParamForm rllxParamForm = new RLLXParamForm() { Text = "燃料参数复制" };
            this.setControlValue(rllxParamForm, "teSC_OCN", dtSelected.Rows[0]["SC_OCN"].ToString(), true);
            this.setControlValue(rllxParamForm, "cbeRLLX", dtSelected.Rows[0]["RLLX"].ToString(), true);
            this.setControlValue(rllxParamForm, "labOPERATION", "原操作类型号：V" + dtSelected.Rows[0]["OPERATION"], true);
            this.setControlValue(rllxParamForm, "labVERSION", "燃料参数版本号：V" + dtSelected.Rows[0]["VERSION"], true);
            rllxParamForm.ShowDialog();
            if (rllxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
        }
        
        // 修改
        private void barBtnEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.gvOCN_RLLXPARAM.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvOCN_RLLXPARAM.PostEditor();
            var dataSource = (DataView)this.gvOCN_RLLXPARAM.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                        
                    }
                }
            }
            if (dtSelected.AsDataView().ToTable(true, new string[] { "SC_OCN", "VERSION" }).Rows.Count != 1)
            {
                MessageBox.Show(String.Format("每次只能操作一组记录，您选择了{0}组！", dtSelected.AsDataView().ToTable(true, new string[] { "SC_OCN" }).Rows.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
           
            if (dtSelected.Rows[0]["OPERATION"].Equals("4"))
            {
                MessageBox.Show(String.Format("OCN为{0}：已经删除，无法进行此操作！", dtSelected.Rows[0]["SC_OCN"]), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RLLXParamForm rllxParamForm = new RLLXParamForm() { Text = "燃料参数修改" };
            this.setControlValue(rllxParamForm, "teSC_OCN", dtSelected.Rows[0]["SC_OCN"].ToString(), true);
            this.setControlValue(rllxParamForm, "cbeRLLX", dtSelected.Rows[0]["RLLX"].ToString(), true);
            this.setControlValue(rllxParamForm, "labOPERATION", "原操作类型号：V" + dtSelected.Rows[0]["OPERATION"], true);
            this.setControlValue(rllxParamForm, "labVERSION", "燃料参数版本号：V" + dtSelected.Rows[0]["VERSION"], true);
            rllxParamForm.ShowDialog();
            if (rllxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
        }

        // 删除
        private void barBtnDel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.gvOCN_RLLXPARAM.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvOCN_RLLXPARAM.PostEditor();
            var dataSource = (DataView)this.gvOCN_RLLXPARAM.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                    }
                }
            }
            var SC_OCNSelected = dtSelected.AsDataView().ToTable(true, new string[] { "SC_OCN", "VERSION" });
            if (SC_OCNSelected.Rows.Count == 0)
            {
                MessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var sc_ocnArray = SC_OCNSelected.AsEnumerable().Select(d => d.Field<string>("SC_OCN")).ToArray();
            if (OracleHelper.Exists(OracleHelper.conn, String.Format("SELECT COUNT(*) FROM VIN_INFO WHERE SC_OCN IN ('{0}') AND MERGER_STATUS=0", string.Join("','", sc_ocnArray))))
            {
                MessageBox.Show("您选择要操作的记录包含未和合成的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("编组删除，确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    foreach (DataRow dr in dtSelected.Rows)
                    {
                        try
                        {
                            string version = OracleHelper.ExecuteScalar(OracleHelper.conn, string.Format("SELECT MIN(VERSION) FROM OCN_RLLX_PARAM_ENTITY WHERE OPERATION='4' AND SC_OCN='{0}'", dr["SC_OCN"])).ToString();
                            int versionNew = string.IsNullOrEmpty(version) ? 0 : Convert.ToInt32(version) - 1;
                            OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("UPDATE OCN_RLLX_PARAM_ENTITY SET OPERATION = '4',VERSION = '{0}' WHERE SC_OCN='{1}' AND OPERATION = '{2}' AND VERSION={3} ", versionNew, dr["SC_OCN"], dr["OPERATION"], dr["VERSION"]));
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            MessageBox.Show(String.Format("数据库操作出现异常，删除失败：{0}！", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
            this.refrashCurrentPage();
        }

        // 全选
        private void barBtnSelectAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvOCN_RLLXPARAM.FocusedRowHandle = 0;
            this.gvOCN_RLLXPARAM.FocusedRowHandle = 1;
            Utils.SelectItem(this.gvOCN_RLLXPARAM, true);
        }

        // 取消全选
        private void barBtnClearAll_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvOCN_RLLXPARAM.FocusedRowHandle = 0;
            this.gvOCN_RLLXPARAM.FocusedRowHandle = 1;
            Utils.SelectItem(this.gvOCN_RLLXPARAM, false);
        }

        // 导出Excel
        private void barBtnExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                DataTable dtExport = (DataTable)gcOCN_RLLXPARAM.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DialogResult dialogResult = saveFileDialog1.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    gcOCN_RLLXPARAM.ExportToXls(saveFileDialog1.FileName, options);
                    ExcelHelper excelBuilder = new ExcelHelper(saveFileDialog1.FileName);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.SaveFile();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 刷新
        private void barBtnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            refrashCurrentPage();
        }

        // 查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchLocal(1);
        }

        // 清除查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbSCOCN.Text = string.Empty;
            this.tbCSBM.Text = string.Empty;
            this.tbCSMC.Text = string.Empty;
            this.tbCSZ.Text = string.Empty;
            this.cbRllx.Text = string.Empty;
        }

        // 首页
        private void btnFirPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(1);
        }

        // 上一页
        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(--pageNum);
        }

        // 下一页
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(++pageNum);
        }

        // 尾页
        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(pageCou);
        }

        // 刷新
        private void refrashCurrentPage()
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 0) SearchLocal(pageNum);
        }

        // 是否显示全部
        private void ceQueryAll_CheckedChanged_1(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }

        // 查询
        private void SearchLocal(int pageNum)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //获取总数目
                int dataCount = queryCount();
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    var dt = queryByPage(pageNum);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    gcOCN_RLLXPARAM.DataSource = dt;
                    this.gvOCN_RLLXPARAM.BestFitColumns();
                    int pageSize = Convert.ToInt32(this.spanNumber.Text);
                    int pageCount = dataCount / pageSize;
                    if (dataCount % pageSize > 0) pageCount++;
                    int dataLast;
                    if (pageNum == pageCount)
                        dataLast = dataCount;
                    else
                        dataLast = pageSize * pageNum;
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", (pageSize * (pageNum - 1) + 1), dataLast);
                    this.txtPage.Text = String.Format("{0}/{1}", pageNum, pageCount);
                }
                else
                {
                    var dt = queryAll();
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    gcOCN_RLLXPARAM.DataSource = dt;
                    this.gvOCN_RLLXPARAM.BestFitColumns();
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", 1, dataCount);
                    this.txtPage.Text = String.Format("{0}/{1}", 1, 1);
                }
                if (dataCount == 0)
                {
                    this.labPage.Text = "当前显示0至0条";
                    this.txtPage.Text = "0/0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        // 获取总数
        private int queryCount()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COUNT(*) FROM OCN_RLLX_PARAM_ENTITY WHERE 1=1 AND OPERATION!='4' ");
            if (!string.IsNullOrEmpty(tbSCOCN.Text))
            {
                sql.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSCOCN.Text);
            }
            if (!string.IsNullOrEmpty(tbCSBM.Text))
            {
                sql.AppendFormat(" AND (CSBM like '%{0}%')", tbCSBM.Text);
            }
            if (!string.IsNullOrEmpty(tbCSMC.Text))
            {
                sql.AppendFormat(" AND (CSMC like '%{0}%')", tbCSMC.Text);
            }
            if (!string.IsNullOrEmpty(tbCSZ.Text))
            {
                sql.AppendFormat(" AND (CSZ like '%{0}%')", tbCSZ.Text);
            }
            if (!string.IsNullOrEmpty(cbRllx.Text))
            {
                sql.AppendFormat(" AND (RLLX like '%{0}%')", cbRllx.Text);
            }
            var count = OracleHelper.ExecuteScalar(OracleHelper.conn, sql.ToString());
            return count != null ? Convert.ToInt32(count.ToString()) : 0;
        }

        // 获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(tbSCOCN.Text))
            {
                sqlWhere.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSCOCN.Text);
            }
            if (!string.IsNullOrEmpty(tbCSBM.Text))
            {
                sqlWhere.AppendFormat(" AND (CSBM like '%{0}%')", tbCSBM.Text);
            }
            if (!string.IsNullOrEmpty(tbCSMC.Text))
            {
                sqlWhere.AppendFormat(" AND (CSMC like '%{0}%')", tbCSMC.Text);
            }
            if (!string.IsNullOrEmpty(tbCSZ.Text))
            {
                sqlWhere.AppendFormat(" AND (CSZ like '%{0}%')", tbCSZ.Text);
            }
            if (!string.IsNullOrEmpty(cbRllx.Text))
            {
                sqlWhere.AppendFormat(" AND (RLLX like '%{0}%')", cbRllx.Text);
            }
            string sqlVins = string.Format(@"select * from OCN_RLLX_PARAM_ENTITY where 1=1 AND OPERATION!='4' {0}", sqlWhere);
            string sqlStr = string.Format(@"select * from (select F.*,ROWNUM RN from ({0}) F where ROWNUM<={1}) where RN>{2}", sqlVins, pageSize * pageNum, pageSize * (pageNum - 1));
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        // 获取全部数据
        private DataTable queryAll()
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(@"SELECT * FROM OCN_RLLX_PARAM_ENTITY WHERE 1=1 AND OPERATION!='4' ");
            if (!string.IsNullOrEmpty(tbSCOCN.Text))
            {
                sqlStr.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSCOCN.Text);
            }
            if (!string.IsNullOrEmpty(tbCSBM.Text))
            {
                sqlStr.AppendFormat(" AND (CSBM like '%{0}%')", tbCSBM.Text);
            }
            if (!string.IsNullOrEmpty(tbCSMC.Text))
            {
                sqlStr.AppendFormat(" AND (CSMC like '%{0}%')", tbCSMC.Text);
            }
            if (!string.IsNullOrEmpty(tbCSZ.Text))
            {
                sqlStr.AppendFormat(" AND (CSZ like '%{0}%')", tbCSZ.Text);
            }
            if (!string.IsNullOrEmpty(cbRllx.Text))
            {
                sqlStr.AppendFormat(" AND (RLLX like '%{0}%')", cbRllx.Text);
            }
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr.ToString(), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        // 状态列的数据显示文本
        private void dv_cljbxx_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Equals("OPERATION"))
            {
                switch (e.Value.ToString().Trim())
                {
                    case "0":
                        e.DisplayText = "Excel导入";
                        break;
                    case "1":
                        e.DisplayText = "新增";
                        break;
                    case "2":
                        e.DisplayText = "复制新增";
                        break;
                    case "3":
                        e.DisplayText = "修改";
                        break;
                    case "4":
                        e.DisplayText = "删除";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
        }

        //初始化详细信息
        private void setControlValue(RLLXParamForm rllx, string cName, String val, bool enable)
        {
            if (cName == null || "" == cName)
            {
                return;
            }

            Control[] c = rllx.Controls.Find(cName, true);
            if (c.Length > 0)
            {
                c[0].Text = val;
                c[0].Enabled = enable;
            }
        }
        
        //双击行
        private void gcOCN_RLLXPARAM_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)gcOCN_RLLXPARAM.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null)
            {
                return;
            }
            if (dr.Row["OPERATION"].Equals("4"))
            {
                MessageBox.Show(String.Format("OCN为{0}：已经删除，无法进行此操作！", dr.Row["SC_OCN"]), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RLLXParamForm rllxParamForm = new RLLXParamForm() { Text = "燃料参数修改" };
            this.setControlValue(rllxParamForm, "teSC_OCN", dr.Row["SC_OCN"].ToString(), true);
            this.setControlValue(rllxParamForm, "cbeRLLX", dr.Row["RLLX"].ToString(), true);
            this.setControlValue(rllxParamForm, "labVERSION", "燃料参数版本号：V" + dr.Row["VERSION"], true);
            rllxParamForm.ShowDialog();
            if (rllxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();

        }

        //"选择"状态改变时
        private void gvOCN_RLLXPARAM_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            ColumnView cv = (ColumnView)gcOCN_RLLXPARAM.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null)
            {
                return;
            }
            bool checkflag = (bool)dr.Row["check"];
            string SCOCN = (string)dr.Row["SC_OCN"];
            GridView dgv = gvOCN_RLLXPARAM as GridView;
            DataView dv = (DataView)dgv.DataSource;
            if (dv != null)
            {
                for (int i = 0; i < dv.Table.Rows.Count; i++)
                {
                    if ((string)dv.Table.Rows[i]["SC_OCN"] == SCOCN)
                    {
                        if (checkflag)
                        {
                            dv.Table.Rows[i]["check"] = false;
                        }
                        else
                        {
                            dv.Table.Rows[i]["check"] = true;
                        }
                    }
                }
                dgv.RefreshData();
            }
        }


    }
}
