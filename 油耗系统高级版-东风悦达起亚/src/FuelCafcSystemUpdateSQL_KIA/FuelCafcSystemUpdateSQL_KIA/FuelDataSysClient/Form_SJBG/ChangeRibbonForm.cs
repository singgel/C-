using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Data.OleDb;
using Common;
using DevExpress.XtraGrid.Views.Base;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Form_SJBG;
using FuelDataSysClient.Model;
using DevExpress.XtraEditors;
using System.Threading;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace FuelDataSysClient.Form_SJBG
{
    public partial class ChangeRibbonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ChangeRibbonForm()
        {
            InitializeComponent();

            this.dtStartTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).ToString("yyyy/MM/dd");
            this.dtEndTime.Text = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy/MM/dd");

            this.repositoryItemButtonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1ButtonClick);
            this.repositoryItemButtonEdit2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit2ButtonClick);
        }

        //查询变更申请
        private void btnCtnySearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        //查询数据库
        public void LoadData()
        {
            string sql = String.Format("select ID,QCSCQY,APPLYDATA,REASON,REMARK from DATA_CHANGE_BASE where to_date(to_char(APPLYDATA,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')>=to_date('{0}','yyyy-mm-dd hh24:mi:ss') and to_date(to_char(APPLYDATA,'yyyy/MM/dd'),'yyyy-mm-dd hh24:mi:ss')<=to_date('{1}','yyyy-mm-dd hh24:mi:ss')", this.dtStartTime.Text, this.dtEndTime.Text);
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Add("check", System.Type.GetType("System.Boolean"));
                ds.Tables[0].Columns["check"].SetOrdinal(0);
                this.dgvCtny.DataSource = ds.Tables[0];
            }
            else
            {
                this.dgvCtny.DataSource = null;
            }
        }

        //新增变更申请
        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ChangeAddXtraForm caxf = new ChangeAddXtraForm();
            caxf.ShowDialog();
        }

        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel|*.xls;*.xlsx|All Files|*.*" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MitsUtils miutils = new MitsUtils();
                    DataSet ds = miutils.ReadExcel(ofd.FileName, "Sheet1");

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        dt.Columns["备案号（VIN）"].ColumnName = "VIN";
                        dt.Columns["车辆型号"].ColumnName = "CLXH";
                        dt.Columns["通用名称"].ColumnName = "TYMC";
                        dt.Columns["燃料种类"].ColumnName = "RLLX";
                        dt.Columns["综合工况燃料消耗量"].ColumnName = "CT_ZHGKRLXHL";
                        dt.Columns["整车整备质量"].ColumnName = "ZCZBZL";
                        dt.Columns["变速器型式"].ColumnName = "CT_BSQXS";
                        dt.Columns["座位排数"].ColumnName = "ZWPS";
                        dt.Columns["操作类型"].ColumnName = "APPLYTYPE";
                        dt.Columns.Add("UPDATEFIELD", System.Type.GetType("System.String"));
                        dt.Columns.Add("FIELDOLD", System.Type.GetType("System.String"));
                        dt.Columns.Add("FIELDNEW", System.Type.GetType("System.String"));
                        dt.Columns.Add("DID", System.Type.GetType("System.String"));
                        dt.Columns["DID"].SetOrdinal(0);
                        dt.Columns["APPLYTYPE"].SetOrdinal(12);
                        ChangeAddXtraForm caxf = new ChangeAddXtraForm();
                        caxf.import = true;
                        caxf.LoadDataByVin(dt.AsDataView());
                        caxf.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        //选择选中数据
        private List<string> GetCheckData()
        {
            var view = dgvCtny.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            return C2M.SelectedParamEntityIds(dv, "ID");

        }

        //删除变更申请
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            var checkList = GetCheckData();

            if (checkList.Count == 0)
            {
                MessageBox.Show("请选择要删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                OracleTransaction ts = conn.BeginTransaction();
                try
                {
                    foreach (string str in checkList)
                    {
                        OracleHelper.ExecuteNonQuery(ts, String.Format(@"delete  from DATA_CHANGE where DID='{0}'", str), null);
                        OracleHelper.ExecuteNonQuery(ts, String.Format(@"delete  from DATA_CHANGE_BASE where ID='{0}'", str), null);
                    }
                    ts.Commit();

                }
                catch (Exception)
                {
                    ts.Rollback();
                }
            }
            this.LoadData();
        }

        //查看变更申请
        private void barButtonItem7_ItemClick(object sender, ItemClickEventArgs e)
        {
            var checkList = GetCheckData();

            if (checkList.Count == 1)
            {
                string ID = checkList[0];
                ChangeAddXtraForm caxf = new ChangeAddXtraForm(ID);
                caxf.Show();
            }
            else
            {
                MessageBox.Show(String.Format("每次只能操作一条记录，您选择了{0}条！", checkList.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        //双击-查看变更申请 
        private void dgvCtny_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ColumnView cv = (ColumnView)dgvCtny.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();

            if (dr == null)
            {
                return;
            }
            string ID = (string)dr.Row.ItemArray[1];
            ChangeAddXtraForm caxf = new ChangeAddXtraForm(ID);
            caxf.Show();
        }

        //新增电子签章
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ds = Utils.serviceFiel.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("0"))
            {
                using (SigAddForm formSigAdd = new SigAddForm() { ShowInTaskbar = false })
                {
                    formSigAdd.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("权限未开通，不能进行该操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //修改电子签章
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ds = Utils.serviceFiel.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("1"))
            {
                using (SigEditForm formSigEdit = new SigEditForm(ds.Tables[0].Rows[0]["IMG_NEW_NAME"].ToString(), ds.Tables[0].Rows[0]["APP_DATE"].ToString()) { ShowInTaskbar = false })
                {
                    formSigEdit.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("权限未开通，不能进行该操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //查看授权
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (EnterpriseAuthorityForm formEnterpriseAuthority = new EnterpriseAuthorityForm())
            {
                formEnterpriseAuthority.ShowDialog();
            }
        }

        //修改变更申请
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            var view = dgvCtny.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            List<string> listID = C2M.SelectedParamEntityIds(dv, "ID");
            if (listID.Count > 1)
            {
                MessageBox.Show(String.Format("每次只能操作一条记录，您选择了{0}条！", listID.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (listID.Count == 0)
            {
                MessageBox.Show("请选择要操作的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (listID.Count == 1)
            {
                string ID = (string)listID[0];
                ChangeAddXtraForm caxf = new ChangeAddXtraForm(ID);
                caxf.Show();
            }
        }

        private void repositoryItemButtonEdit1ButtonClick(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = ((DevExpress.XtraGrid.Views.Grid.GridView)(this.dgvCtny.MainView));
                int rowhandle = view.FocusedRowHandle;
                DataRow dr = view.GetDataRow(rowhandle);
                string ID = dr["ID"].ToString();
                ChangeAddXtraForm caxf = new ChangeAddXtraForm(ID);
                caxf.simpleButton2_Click(sender, e);
                caxf.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void repositoryItemButtonEdit2ButtonClick(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = ((DevExpress.XtraGrid.Views.Grid.GridView)(this.dgvCtny.MainView));
                int rowhandle = view.FocusedRowHandle;
                DataRow dr = view.GetDataRow(rowhandle);
                string ID = dr["ID"].ToString();
                ChangeAddXtraForm caxf = new ChangeAddXtraForm(ID);
                caxf.simpleButton4_Click(sender, e);
                caxf.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("发送失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}