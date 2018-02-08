using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using FuelDataModel;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Tool.Tool_Jaguar;
using FuelDataSysClient.Form_DBManager;
using FuelDataSysClient.Utils_Control;

namespace FuelDataSysClient.Form_National
{
    public partial class GHDataForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        OpenFileDialog openFileDialog1 = new OpenFileDialog();

        public GHDataForm()
        {
            InitializeComponent();

        }

        void refrashBySubForm(object sender, FormClosingEventArgs args)
        {
            searchLocal();
        }

        // 全选
        private void barBtnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvGH.FocusedRowHandle = 0;
            this.gvGH.FocusedColumn = this.gvGH.Columns[1];
            GridControlHelper.SelectItem(this.gvGH, true);
        }

        // 取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvGH.FocusedRowHandle = 0;
            this.gvGH.FocusedColumn = this.gvGH.Columns[1];
            GridControlHelper.SelectItem(this.gvGH, false);
        }

        // 删除
        private void barBtnLocalDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvGH.PostEditor();

            DataView dv = (DataView)this.gvGH.DataSource;
            string selectedParamEntityIds = "";
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if ((bool)dv.Table.Rows[i]["check"])
                    {
                        selectedParamEntityIds += ",'" + dv.Table.Rows[i]["VIN"] + "'";
                    }
                }
            }
            if ("" == selectedParamEntityIds)
            {
                MessageBox.Show("请选择要删除的数据！");
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            selectedParamEntityIds = selectedParamEntityIds.Substring(1);

            OleDbConnection conn = new OleDbConnection(AccessHelper.conn);
            conn.Open();
            OleDbTransaction ts = conn.BeginTransaction();
            try
            {
                if ("" != selectedParamEntityIds)
                {
                    string sql = @"delete * from GH_DATA where vin in (" + selectedParamEntityIds + ")";
                    int jbxxcount = AccessHelper.ExecuteNonQuery(ts, sql, null);

                    ts.Commit();
                }
            }
            catch (Exception ex)
            {
                ts.Rollback();
            }
            finally
            {
                conn.Close();
            }
            searchLocal();
        }

        // 批量修改进口时间
        private void btnUpdateDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                // 获取对象
                List<string> vinList = Utils.GetUpdateVin(this.gvGH, (DataTable)this.gcGH.DataSource);

                if (vinList == null || vinList.Count < 1)
                {
                    MessageBox.Show("请选择要修改的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (UpdateDateForm dateForm = new UpdateDateForm() { Date = "GH" })
                {
                    if (dateForm.ShowDialog() == DialogResult.OK)
                    {
                        JaguarUtils jaguarUtil = new JaguarUtils();
                        if (jaguarUtil.UpdateGHImportDate(vinList, dateForm.Date))
                        {
                            this.searchLocal();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        // 查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchLocal();
        }

        // 查询
        private void searchLocal()
        {
            try
            {
                // 验证查询时间：结束时间不能小于开始时间
                if (!this.VerifyStartEndTime())
                {
                    MessageBox.Show("结束时间不能小于开始时间");
                    return;
                }
                // 获取本地车辆基本信息
                string sql = string.Format(@"SELECT iif(status= '1 ', '未导出 ',iif(status= '0 ', '已导出 ')) as STATUS, GH_DATA.VIN,CLXH,COC_ID,GH_FDJSCC,CLZZRQ,TGRQ,GH_FDJXLH,tt.PARAM_VALUE FROM GH_DATA
left join (select * from rllx_param_entity where PARAM_CODE='CT_FDJXH' or PARAM_CODE='FCDS_HHDL_FDJXH') as tt on gh_data.vin=tt.vin WHERE 1=1");

                string sw = "";

                if (!ceExported.Checked)
                {
                    sw += " AND STATUS='1'";
                }
                if (!string.IsNullOrEmpty(tbVin.Text))
                {
                    sw += " and (gh_data.vin = '" + tbVin.Text + "')";
                }

                string timeFiled = "TGRQ";
                if (cbTimeType.Text.Trim() == "车辆制造日期")
                {
                    timeFiled = "CLZZRQ";
                }
                if (!string.IsNullOrEmpty(dtStartTime.Text))
                {
                    sw += " and (" + timeFiled + ">=#" + Convert.ToDateTime(this.dtStartTime.Text) + "#)";
                }
                if (!string.IsNullOrEmpty(dtEndTime.Text))
                {
                    sw += " and (" + timeFiled + "<#" + Convert.ToDateTime(this.dtEndTime.Text).Add(new TimeSpan(24, 0, 0)) + "#)";
                }

                sw += " ORDER BY TGRQ DESC";

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql + sw, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                this.gcGH.DataSource = dt;
                this.gvGH.BestFitColumns();
                this.gcGHide.DataSource = dt; // 导出用
                lblSum.Text = string.Format("共{0}条", dt.Rows.Count);
                Utils.SelectItem(this.gvGH, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message);
            }
        }

        // 验证查询时间：结束时间不能小于开始时间
        protected bool VerifyStartEndTime()
        {
            string startTime = dtStartTime.Text;
            string endTime = dtEndTime.Text;

            try
            {
                if (!string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime) && Convert.ToDateTime(startTime) > Convert.ToDateTime(endTime))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        // 通过excel查询数据
        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            JaguarUtils jaguarUtil = new JaguarUtils();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                DataSet ds = jaguarUtil.SearchGHExcel(openFileDialog1.FileName, "");
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                gcGH.DataSource = dt;

                lblSum.Text = string.Format("共{0}条", dt.Rows.Count);
                Utils.SelectItem(this.gvGH, false);
            }
        }

        // 修改进口日期
        private void btnUpdateImportDate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            JaguarUtils jaguarUtil = new JaguarUtils();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (jaguarUtil.ReadUpdateGHDate(openFileDialog1.FileName, "") == 1)
                {

                    MessageBox.Show("批量修改时间成功");

                }
                else
                {
                    MessageBox.Show("批量修改时间失败");
                }
                this.searchLocal();
            }
        }

        // 导出国环数据
        private void barExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };
                    gcGHide.ExportToXls(saveFileDialog.FileName, options);
                    // 导出后更新状态
                    UpdateGHstatus();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 导出后更新状态
        private void UpdateGHstatus()
        {
            string vins = string.Empty;
            DataTable dt = (DataTable)this.gcGHide.DataSource;
            foreach (DataRow dr in dt.Rows)
            {
                vins += ",'" + (dr["VIN"] == null ? "" : dr["VIN"].ToString().Trim()) + "'";
            }

            if (!string.IsNullOrEmpty(vins))
            {
                vins = vins.Substring(1);

                string updateSql = string.Format(@"UPDATE GH_DATA SET STATUS='0' WHERE VIN IN ({0})", vins);

                AccessHelper.ExecuteNonQuery(AccessHelper.conn, updateSql, null);
            }
        }
    }
}
