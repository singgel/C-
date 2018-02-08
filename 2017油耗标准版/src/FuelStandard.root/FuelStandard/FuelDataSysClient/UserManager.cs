using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient
{
    public partial class UserManager : DevExpress.XtraEditors.XtraForm
    {
        string con = AccessHelper.conn;
        string id = string.Empty; // 用户表自增Id
        string is_Admin = string.Empty; //是否管理员
        public UserManager()
        {
            InitializeComponent();
        }

        private void UserManager_Load(object sender, EventArgs e)
        {
            DataBinds();
        }

        private void DataBinds()
        {
            try
            {
                string sql = "select * from LOCAL_USER";
                DataSet ds = AccessHelper.ExecuteDataSet(con, sql, null);
                DataTable dt = ds.Tables[0];
                dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                dgvCljbxx.DataSource = dt;
                Utils.SelectItem(this.gridView1, false);
            }
            catch { }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string userId = this.txtUserName.Text.Trim();
                if (string.IsNullOrEmpty(this.txtUserName.Text))
                {
                    MessageBox.Show("请输入用户名");
                    return;
                }
                if (string.IsNullOrEmpty(userId))
                {
                    MessageBox.Show("请输入密码");
                    return;
                }

                DataView dv = (DataView)this.gridView1.DataSource;
                if (dv != null)
                {
                    if (dv.Table.Select(string.Format("USER_ID='{0}'", userId)).Length > 0)
                    {
                        MessageBox.Show("该用户已存在");
                        return;
                    }
                }

                string sql = string.Format("insert into LOCAL_USER(USER_ID,USER_PASSWORD,IS_ADMIN) values('{0}','{1}','否')", this.txtUserName.Text.Trim(), this.txtPassWord.Text);
                AccessHelper.ExecuteDataSet(con, sql, null);
            }
            catch { }
            DataBinds();
            Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridView1.PostEditor();

                DataView dv = (DataView)this.gridView1.DataSource;
                string selectedParamEntityIds = "";
                if (dv != null)
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        if ((bool)dv.Table.Rows[i]["check"])
                        {
                            if ((string)dv.Table.Rows[i]["IS_ADMIN"] == "是")
                            {
                                MessageBox.Show("不能删除管理员");
                            }
                            else
                            {
                                selectedParamEntityIds += "'" + dv.Table.Rows[i]["USER_ID"] + "',";
                            }
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
                selectedParamEntityIds = selectedParamEntityIds.TrimEnd(',');
                string sql = @"delete from LOCAL_USER where USER_ID in (" + selectedParamEntityIds + ")";
                AccessHelper.ExecuteNonQuery(con, sql, null);
            }
            catch { }
            DataBinds();
        }

        private void Clear()
        {
            this.txtPassWord.Text = "";
            txtUserName.Text = "";
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            //this.txtUserName.Enabled = false;
            ColumnView cv = (ColumnView)dgvCljbxx.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            this.txtUserName.Text  = (string)dr.Row.ItemArray[1];
            this.txtPassWord.Text = (string)dr.Row.ItemArray[2];
            this.id = dr.Row["ID"].ToString();
            this.is_Admin = dr.Row["IS_ADMIN"].ToString();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //this.txtUserName.Enabled = true;
                if (this.is_Admin == "是" && this.txtUserName.Text.Trim()!="admin")
                {
                    MessageBox.Show("不能修改管理员帐号");
                    return;
                }
                string sql = string.Format("update LOCAL_USER set USER_ID='{0}', USER_PASSWORD='{1}' where ID={2}", txtUserName.Text, txtPassWord.Text, this.id);
                AccessHelper.ExecuteNonQuery(con, sql, null);
            }
            catch { }
            DataBinds();
            Clear();
        }
    }
}