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
using System.IO;

namespace FuelDataSysClient.Change
{
    public partial class ChangeRibbonForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        FuelFileUpload.FileUploadService service = Utils.serviceFiel;
        InitDataTime initTime = new InitDataTime();

        public ChangeRibbonForm()
        {
            InitializeComponent();

            this.dateEdit1.Text = initTime.getStartTime();
            this.dateEdit2.Text = initTime.getEndTime();
        }

        private void ChangeRibbonForm_Load(object sender, EventArgs e)
        {

            this.repositoryItemButtonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1ButtonClick);
            this.repositoryItemButtonEdit2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit2ButtonClick);
            //List<string> qymcList = new List<string> 
            //{
            //    //沈阳华晨金杯汽车有限公司
            //    "FADCCHCJBU001",
            //    //华晨汽车集团控股有限公司
            //    "FADCCHCQCU001",
            //    //安徽江淮汽车股份有限公司
            //    "FADCCJHGFU001",
            //    //宝马（中国）汽车贸易有限公司
            //    "FADCFBMZGU001",
            //    //东南（福建）汽车工业有限公司
            //    "FADCCDNQCU001",
            //    //丰田汽车（中国）投资有限公司
            //    "FADCFFTZGU001",
            //    //广汽丰田汽车有限公司
            //    "FADCCGQFTU001",
            //    //捷豹路虎汽车贸易（上海）有限公司
            //    "FADCFJBLHU001",
            //    //斯巴鲁汽车（中国）有限公司
            //    "FADCFSBR0U001",
            //    //长安福特汽车有限公司
            //    "FADCCCAFTU001",
            //    //长城汽车股份有限公司
            //    "FADCCCCQCU001",
            //    //东风小康汽车有限公司
            //    "FADCCDFXKU001",
            //    //重庆长安汽车股份有限公司
            //    "FADCCCAQCU001",
            //    //大众汽车（中国）销售有限公司
            //    "FADCFDZXSU001",
            //    //阿尔法罗密欧（上海）汽车销售有限公司
            //    "FADCFALFAU001",
            //    //东风悦达起亚汽车有限公司
            //    "FADCCYDQYU001",
            //};
            //if (qymcList.Contains(Utils.userId))
            //{
            //    this.barButtonItem8.Visibility = BarItemVisibility.Always;
            //}
            this.barButtonItem8.Visibility = BarItemVisibility.Always;
        }

        private void btnCtnySearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string sql = String.Format("select ID,QCSCQY,APPLYDATA,REASON,REMARK from DATA_CHANGE_BASE where APPLYDATA>=#{0}# and APPLYDATA<=#{1}#", this.dateEdit1.Text, this.dateEdit2.Text);
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
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

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ChangeAddXtraForm caxf = new ChangeAddXtraForm();
            caxf.ShowDialog();
        }

        /// <summary>
        /// 导入变更申请单模板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem8_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel|*.xls;*.xlsx|All Files|*.*";
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
                        dt.Columns["产品型号"].ColumnName = "CLXH";
                        dt.Columns["通用名称"].ColumnName = "TYMC";
                        dt.Columns["燃料种类"].ColumnName = "RLLX";
                        dt.Columns["燃料消耗量（综合）"].ColumnName = "CT_ZHGKRLXHL";
                        dt.Columns["整备质量"].ColumnName = "ZCZBZL";
                        dt.Columns["变速器型式"].ColumnName = "CT_BSQXS";
                        dt.Columns["座椅排数"].ColumnName = "ZWPS";
                        dt.Columns["操作类型"].ColumnName = "APPLYTYPE";
                        dt.Columns.Add("UPDATEFIELD");
                        dt.Columns.Add("FIELDOLD");
                        dt.Columns.Add("FIELDNEW");
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

        /// <summary>
        /// 选择选中数据
        /// </summary>
        /// <returns></returns>
        private List<string> GetCheckData()
        {
            var view = dgvCtny.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            return C2M.SelectedParamEntityIds(dv, "ID");

        }

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

            OleDbConnection conn = new OleDbConnection(AccessHelper.conn);
            conn.Open();
            OleDbTransaction ts = conn.BeginTransaction();
            try
            {
                foreach (string str in checkList)
                {
                    string sql = String.Format(@"delete  from DATA_CHANGE where DID='{0}'", str);
                    int jbxxcount = AccessHelper.ExecuteNonQuery(ts, sql, null);
                    string sqlentity = String.Format(@"delete  from DATA_CHANGE_BASE where ID='{0}'", str);
                    int paramcount = AccessHelper.ExecuteNonQuery(ts, sqlentity, null);
                }
                ts.Commit();

            }
            catch (Exception)
            {
                ts.Rollback();
            }
            finally
            {
                conn.Close();
            }
            this.LoadData();
        }

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
                MessageBox.Show("每次只能操作一条记录，您选择了" + checkList.Count + "条！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

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

        /// <summary>
        /// 新增电子签章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem3_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ds = service.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("0"))
            {
                SigAddForm formSigAdd = new SigAddForm();
                formSigAdd.ShowInTaskbar = false;
                formSigAdd.ShowDialog();
            }
            else
            {
                MessageBox.Show("权限未开通，不能进行该操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 修改电子签章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            var ds = service.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["STATUS"].ToString().Equals("1"))
            {
                SigEditForm formSigEdit = new SigEditForm(ds.Tables[0].Rows[0]["IMG_NEW_NAME"].ToString(), ds.Tables[0].Rows[0]["APP_DATE"].ToString());
                formSigEdit.ShowInTaskbar = false;
                formSigEdit.ShowDialog();
            }
            else
            {
                MessageBox.Show("权限未开通，不能进行该操作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 查看授权
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem5_ItemClick(object sender, ItemClickEventArgs e)
        {
            EnterpriseAuthorityForm formEnterpriseAuthority = new EnterpriseAuthorityForm();
            formEnterpriseAuthority.ShowDialog();
        }

        /// <summary>
        /// 修改变更申请
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barButtonItem6_ItemClick(object sender, ItemClickEventArgs e)
        {
            var view = dgvCtny.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            List<string> listID = C2M.SelectedParamEntityIds(dv, "ID");
            if (listID.Count > 1)
            {
                MessageBox.Show("每次只能操作一条记录，您选择了" + listID.Count + "条！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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