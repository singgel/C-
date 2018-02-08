using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraEditors;

namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    public partial class SingleDicData : DevExpress.XtraEditors.XtraForm
    {
        readonly bool _isModify = false;
        string userId = string.Empty;
        public SingleDicData()
        {
            InitializeComponent();
            initCombobox();
        }
        //修改
        public SingleDicData(string id)
        {
            InitializeComponent();
            initCombobox();
            _isModify = true;
            this.btn_Save.Text = "修改";
            setConValues(id);
        }

        private void setConValues(string id)
        {
            string sql = String.Format(@"SELECT ID,DIC_NAME,DIC_TYPE FROM SYS_DIC WHERE ID={0}", id);
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            userId = id;
            this.txtDicName.Text = ds.Tables[0].Rows[0]["DIC_NAME"].ToString();
            this.cmb_Type.Text = ds.Tables[0].Rows[0]["DIC_TYPE"].ToString();

        }

        private void initCombobox()
        {
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, "SELECT DISTINCT DIC_TYPE FROM SYS_DIC", null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            { 
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    this.cmb_Type.Properties.Items.Add(ds.Tables[0].Rows[i]["DIC_TYPE"].ToString());
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (InputCheck())
            {
                if (!_isModify)
                {
                    if (OracleHelper.Exists(OracleHelper.conn, string.Format("SELECT COUNT(*) FROM SYS_DIC WHERE DIC_NAME='{0}' AND DIC_TYPE = '{1}'", this.txtDicName.Text, this.cmb_Type.Text)))
                    {
                        XtraMessageBox.Show(string.Format("数据{0}已存在", String.Format("{0}:{1}", this.txtDicName.Text, this.cmb_Type.Text)), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    if (string.IsNullOrEmpty(userId))
                    {
                        userId = "SEQ_SYS_DIC.nextval";
                    }
                    string strInsSQL = "INSERT INTO SYS_DIC (ID,DIC_NAME,DIC_TYPE) VALUES ({0},'{1}','{2}')";
                    strInsSQL = string.Format(strInsSQL, userId, this.txtDicName.Text, this.cmb_Type.Text);
                    string strUpdSQL = "UPDATE SYS_DIC SET DIC_NAME='{0}',DIC_TYPE='{1}' WHERE ID={2}";
                    strUpdSQL = string.Format(strUpdSQL, this.txtDicName.Text, this.cmb_Type.Text, userId);
                    int count = OracleHelper.ExecuteNonQuery(con, _isModify ? strUpdSQL : strInsSQL, null);
                    if (count > 0)
                    {
                        this.Close();
                        DataDictionaryForm df = new DataDictionaryForm();
                        df.SearchLocal(1);

                        XtraMessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        XtraMessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <returns></returns>
        private bool InputCheck()
        {
            string operatorName = this.txtDicName.Text.Trim();
            string operatorType = this.cmb_Type.Text.Trim();

            if (string.IsNullOrEmpty(operatorName))
            {
                this.toolTip1.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip1.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.txtDicName.Location.X + this.txtDicName.Width,
                    this.txtDicName.Location.Y);
                this.toolTip1.Show(!_isModify ? "请输入名称！" : "请输入名称！", this, showLocation, 5000);
                this.txtDicName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(operatorType))
            {
                this.toolTip1.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip1.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.cmb_Type.Location.X + this.cmb_Type.Width,
                    this.cmb_Type.Location.Y);
                this.toolTip1.Show(!_isModify ? "请输入类型！" : "请输入类型！", this, showLocation, 5000);
                this.cmb_Type.Focus();
                return false;
            }
            return true;
        }

        private void btn_Cancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

    }
}
