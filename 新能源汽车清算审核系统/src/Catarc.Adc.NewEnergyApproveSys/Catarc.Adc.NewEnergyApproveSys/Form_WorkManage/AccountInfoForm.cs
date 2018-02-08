using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraSplashScreen;
using Catarc.Adc.NewEnergyApproveSys.DevForm;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Catarc.Adc.NewEnergyApproveSys.Form_WorkManage_Utils;
using DevExpress.XtraEditors;
using System.Linq;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using DevExpress.XtraPrinting;


namespace Catarc.Adc.NewEnergyApproveSys.Form_WorkManage
{
    public partial class AccountInfoForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public AccountInfoForm()
        {
            InitializeComponent();
        }

        private void AccountInfoForm_Load(object sender, EventArgs e)
        {
            //查询条件下拉框填充
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from SYS_DIC where 1=1 ", null);
            //CLXZ.Properties.Items.Add(string.Empty);
            //CLYT.Properties.Items.Add(string.Empty);
            //CLZL.Properties.Items.Add(string.Empty);
            DQ.Properties.Items.Add(string.Empty);
            JZNF.Properties.Items.Add(string.Empty);
            PC.Properties.Items.Clear();
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int iDIC = ds.Tables[0].Rows.Count;
                for (int i = 0; i < iDIC; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    switch (dr["DIC_TYPE"].ToString().Trim())
                    {
                        //case "购车领域":
                        //    GCLY.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                        //    break;
                        //case "车辆用途":
                        //    CLYT.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                        //    break;
                        //case "车辆种类":
                        //    CLZL.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                        //    break;
                        case "地区":
                            DQ.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        case "年份":
                            JZNF.Properties.Items.Add(dr["DIC_NAME"].ToString().Trim());
                            break;
                        default: break;
                    }
                }
            }
        }

        private void JZNF_SelectedIndexChanged(object sender, EventArgs e)
        {
            PC.Properties.Items.Clear();
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format("select * from SYS_DIC where DIC_TYPE='{0}' ", JZNF.Text));
            PC.Properties.Items.Add(string.Empty);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    PC.Properties.Items.Add(ds.Tables[0].Rows[i]["DIC_NAME"].ToString().Trim());
                }
            }
        }

        //全部选中
        private void barBtnSelect_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算"))
            {
                this.gvDataInfo3.FocusedRowHandle = 0;
                this.gvDataInfo3.FocusedColumn = gvDataInfo3.Columns[1];
                GridControlHelper.SelectItem(this.gvDataInfo3, true);
            }
            else
            {
                this.gvDataInfo2.FocusedRowHandle = 0;
                this.gvDataInfo2.FocusedColumn = gvDataInfo2.Columns[1];
                GridControlHelper.SelectItem(this.gvDataInfo2, true);
            }
        }

        //全部取消
        private void barBtnCancle_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算"))
            {
                this.gvDataInfo3.FocusedRowHandle = 0;
                this.gvDataInfo3.FocusedColumn = gvDataInfo3.Columns[1];
                GridControlHelper.SelectItem(this.gvDataInfo3, false);
            }
            else
            {
                this.gvDataInfo2.FocusedRowHandle = 0;
                this.gvDataInfo2.FocusedColumn = gvDataInfo2.Columns[1];
                GridControlHelper.SelectItem(this.gvDataInfo2, false);
            }
        }

        //刷新
        private void barBtnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.SearchLocal();
        }

        //导出Excel
        private void barBtnExport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnExport.Caption));
             

                var dtExport = this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算") ? (DataTable)this.gcDataInfo3.DataSource : (DataTable)this.gcDataInfo2.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    var options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    if (this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算"))
                    {
                        this.gcDataInfo3.ExportToXls(saveFileDialog.FileName, options);
                    }
                    else
                    {
                        this.gcDataInfo2.ExportToXls(saveFileDialog.FileName, options);
                    }
                    if (XtraMessageBox.Show("操作成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //导出数据
        private void barBtnExportFloder_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnExportFloder.Caption));
             
                var dtExport = this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算") ? (DataTable)this.gcDataInfo3.DataSource : (DataTable)this.gcDataInfo2.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    XtraMessageBox.Show("当前没有数据可以操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                saveFileDialog.FileName = this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算") ? "三审清算信息汇总" + DateTime.Now.ToString("yyyyMMddHHmmdd") : "二审清算信息汇总" + DateTime.Now.ToString("yyyyMMddHHmmdd");
                var dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    string msg = string.Empty;
                    var exutils = new ExportAccountInfo();
                    msg = exutils.ExportAccountTemplate(saveFileDialog, dtExport.Copy());
                    if (!string.IsNullOrEmpty(msg))
                    {
                        XtraMessageBox.Show(string.Format("操作失败，原因：{0}" , msg), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    if (XtraMessageBox.Show("操作成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.PC.Text) && this.xtraTabControl1.SelectedTabPage.Text.Equals("二审清算"))
            {
                XtraMessageBox.Show("请选择审核批次", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if ((this.PC.Text.Equals("01") || this.PC.Text.Equals("02")) && this.xtraTabControl1.SelectedTabPage.Text.Equals("二审清算"))
            {
                XtraMessageBox.Show("历史批次不包含二审清算", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if ((this.PC.Text.Equals("03") || this.PC.Text.Equals("04")) && this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算"))
            {
                XtraMessageBox.Show("03和04批次三审清算不做拆分", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.SearchLocal(); 
        }

        //清空查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.QYMC.Text = string.Empty;
            this.CLXH.Text = string.Empty;
            this.DQ.Text = string.Empty;
            this.JZNF.Text = string.Empty;
            this.PC.Text = string.Empty;
        }

        // 查询
        private void SearchLocal()
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                var dtQuery = queryAll();
                var dtConvert = ConvertDataTable(dtQuery);
                int dataCount = dtConvert.Rows.Count;
                dtConvert.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dtConvert.Columns["check"].ReadOnly = false;
                for (int i = 0; i < dataCount; i++)
                {
                    dtConvert.Rows[i]["check"] = false;
                }
                if (this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算"))
                {
                    this.gcDataInfo3.DataSource = dtConvert;
                    //this.gvDataInfo3.BestFitColumns();
                }
                else
                {
                    this.gcDataInfo2.DataSource = dtConvert;
                    //this.gvDataInfo2.BestFitColumns();
                }
                this.lblSum.Text = String.Format("共{0}条", dataCount);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //获取全部数据
        private DataTable queryAll()
        {
            string sqlAll = string.Empty;
            if (this.xtraTabControl1.SelectedTabPage.Text.Equals("三审清算"))
            {
                if (string.IsNullOrEmpty(JZNF.Text))
                {
                    sqlAll = String.Format("select * from VIEW_COUNT3 where 1=1 {0}", queryParam());
                }
                else 
                {
                    if (string.IsNullOrEmpty(PC.Text))
                    {
                        StringBuilder strBuildder = new StringBuilder();
                        strBuildder.Append("select DQ, JZNF, CLSCQY, CLXH, SQBZBZ, SUM(ENT_NUM) AS ENT_NUM, SUM(ENT_COUNT) AS ENT_COUNT, APP_MONEY, SUM(APP_NUM) AS APP_NUM, SUM(APP_COUNT) AS APP_COUNT, WM_CONCAT (DISTINCT APP_RESULT_1_A) AS APP_RESULT_1_A, WM_CONCAT (DISTINCT APP_RESULT_1_B) AS APP_RESULT_1_B, WM_CONCAT (DISTINCT APP_RESULT_2) AS APP_RESULT_2, WM_CONCAT (DISTINCT APP_RESULT_3) AS APP_RESULT_3 from (");
                        strBuildder.Append("  select * from APPLY_COUNT_DETAIL_201601 ");
                        strBuildder.Append("  union all select * from APPLY_COUNT_DETAIL_201602 ");
                        strBuildder.Append("  union all select * from VIEW_COUNT3 ");
                        strBuildder.AppendFormat(") T where 1=1 {0} GROUP BY DQ, JZNF, CLSCQY, CLXH, SQBZBZ, APP_MONEY", queryParam());
                        sqlAll = strBuildder.ToString();
                    }
                    else
                    {
                        sqlAll = String.Format("select * from APPLY_COUNT_DETAIL_2016{0} where 1=1 {1}", PC.Text, queryParam());
                    }
                }
            }
            else
            {
                sqlAll = String.Format("select * from VIEW_COUNT2{0} where 1=1 {1}", PC.Text, queryParam());
            }
            var ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlAll, null);
            return ds != null && ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            if (!string.IsNullOrEmpty(QYMC.Text))
            {
                sqlStr.AppendFormat(" AND (CLSCQY like '%{0}%')", QYMC.Text);
            }
            if (!string.IsNullOrEmpty(CLXH.Text))
            {
                sqlStr.AppendFormat(" AND (CLXH like '%{0}%')", CLXH.Text);
            }
            if (!string.IsNullOrEmpty(DQ.Text))
            {
                sqlStr.AppendFormat(" AND (DQ like '%{0}%')", DQ.Text);
            }
            if (!string.IsNullOrEmpty(JZNF.Text))
            {
                sqlStr.AppendFormat(" AND (JZNF = '{0}')", JZNF.Text);
            }
            return sqlStr.ToString();
        }

        //处理重复原因
        private static DataTable ConvertDataTable(DataTable dt)
        {
            DataTable dtDB = new DataTable();
            dtDB.Columns.Add("DQ", Type.GetType("System.String"));
            dtDB.Columns.Add("CLSCQY", Type.GetType("System.String"));
            dtDB.Columns.Add("JZNF", Type.GetType("System.String"));
            dtDB.Columns.Add("CLXH", Type.GetType("System.String"));
            dtDB.Columns.Add("SQBZBZ", Type.GetType("System.Double"));
            dtDB.Columns.Add("ENT_NUM", Type.GetType("System.Double"));
            dtDB.Columns.Add("ENT_COUNT", Type.GetType("System.Double"));
            dtDB.Columns.Add("APP_MONEY", Type.GetType("System.Double"));
            dtDB.Columns.Add("APP_NUM", Type.GetType("System.Double"));
            dtDB.Columns.Add("APP_COUNT", Type.GetType("System.Double"));
            dtDB.Columns.Add("APP_RESULT", Type.GetType("System.String"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dtDB.NewRow();
                dr["DQ"] = dt.Rows[i]["DQ"];
                dr["CLSCQY"] = dt.Rows[i]["CLSCQY"];
                dr["JZNF"] = dt.Rows[i]["JZNF"];
                dr["CLXH"] = dt.Rows[i]["CLXH"];
                dr["SQBZBZ"] = dt.Rows[i]["SQBZBZ"];
                dr["ENT_NUM"] = dt.Rows[i]["ENT_NUM"];
                dr["ENT_COUNT"] = dt.Rows[i]["ENT_COUNT"];
                dr["APP_MONEY"] = dt.Rows[i]["APP_MONEY"];
                dr["APP_NUM"] = dt.Rows[i]["APP_NUM"];
                dr["APP_COUNT"] = dt.Rows[i]["APP_COUNT"];
                if (dt.Rows[i]["ENT_NUM"].Equals(dt.Rows[i]["APP_NUM"]))
                {
                    dr["APP_RESULT"] = string.Empty;
                }
                else
                {
                    List<string> appResultList = new List<string>();
                    var APP_1_A = dt.Rows[i]["APP_RESULT_1_A"].ToString().Trim().Split('，');
                    appResultList.Add(string.Join(",", APP_1_A.Distinct().ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray()));
                    var APP_1_B = dt.Rows[i]["APP_RESULT_1_B"].ToString().Trim().Split('，');
                    appResultList.Add(string.Join(",", APP_1_B.Distinct().ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray()));
                    var APP_2 = dt.Rows[i]["APP_RESULT_2"].ToString().Trim().Split('，');
                    appResultList.Add(string.Join(",", APP_2.Distinct().ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray()));
                    var APP_3 = dt.Rows[i]["APP_RESULT_3"].ToString().Trim().Split('，');
                    appResultList.Add(string.Join(",", APP_3.Distinct().ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray()));
                    dr["APP_RESULT"] = string.Join(",", appResultList.Distinct().ToArray().Where(s => !string.IsNullOrEmpty(s)).ToArray());
                }
                dtDB.Rows.Add(dr);
            }
            return dtDB;
        }

        //编辑列的行号
        private void gvDataInfo_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }
    }
}