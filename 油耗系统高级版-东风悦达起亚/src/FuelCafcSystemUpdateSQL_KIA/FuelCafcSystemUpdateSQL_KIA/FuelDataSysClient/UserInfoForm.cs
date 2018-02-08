using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool;
using Oracle.ManagedDataAccess.Client;

namespace FuelDataSysClient
{
    public partial class UserInfoForm : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 是否显示为修改密码界面
        /// </summary>
        bool _isModify = false;
        string userId = string.Empty;

        public UserInfoForm()
        {
            InitializeComponent();
        }

        public UserInfoForm(string id)
        {
            InitializeComponent();
            _isModify = true;
            setConValues(id);
        }

        private void setConValues(string id) 
        {
            string sql = @"SELECT ID,USERNAME,PWD,NAME,PHONE,STATUS FROM SYS_USERINFO WHERE ID=" + id + "";
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            userId = id;
            this.txtOperatorName.Text = ds.Tables[0].Rows[0]["USERNAME"].ToString();
            this.txtOperatorPwd.Text = ds.Tables[0].Rows[0]["PWD"].ToString();
            this.txtValidatePwd.Text = ds.Tables[0].Rows[0]["PWD"].ToString();
            this.txtUserName.Text = ds.Tables[0].Rows[0]["NAME"].ToString();
            this.txtPhone.Text = ds.Tables[0].Rows[0]["PHONE"].ToString();
            this.chkOperatorState.Checked = ds.Tables[0].Rows[0]["STATUS"].ToString() == "1" ? true : false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
           
            if (UserInputCheck())
            {
                if (!_isModify)
                {
                    if (OracleHelper.Exists(OracleHelper.conn, string.Format("SELECT COUNT(*) FROM SYS_USERINFO WHERE USERNAME='{0}'", this.txtOperatorName.Text)))
                    {
                        MessageBox.Show(string.Format("用户名{0}已存在", this.txtOperatorName.Text), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    if (string.IsNullOrEmpty(userId))
                    {
                        userId = "SEQ_SYS_USERINFO.nextval";
                    }
                    string OperatorName = this.txtOperatorName.Text.Trim();
                    string Password = this.txtOperatorPwd.Text.Trim();
                    string UserName = this.txtUserName.Text.Trim();
                    string Phone = this.txtPhone.Text.Trim();
                    bool status = this.chkOperatorState.Checked;

                    string strInsSQL = "INSERT INTO SYS_USERINFO (ID,USERNAME,PWD,NAME,PHONE,AUTHORITY,STATUS) VALUES ({0},'{1}','{2}','{3}','{4}','{5}',{6})";
                    strInsSQL = string.Format(strInsSQL, userId, OperatorName, Password, UserName, Phone, "", status ? 1 : 0);

                    string strUpdSQL = "UPDATE SYS_USERINFO SET USERNAME='{0}',PWD='{1}',NAME='{2}',PHONE='{3}',STATUS='{4}' WHERE ID={5}";
                    strUpdSQL = string.Format(strUpdSQL, OperatorName, Password, UserName, Phone, status ? 1 : 0, userId);

                    int count = OracleHelper.ExecuteNonQuery(con, _isModify ? strUpdSQL : strInsSQL, null);
                    if (count > 0)
                    {
                        this.Close();
                        AuthorityForm af = new AuthorityForm();
                        af.ResfurbishData();
                        MessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else { MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            }
        }

        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <returns></returns>
        private bool UserInputCheck()
        {
            string operatorName = txtOperatorName.Text.Trim();
            string operatorPwd = txtOperatorPwd.Text.Trim();
            string validatePwd = txtValidatePwd.Text.Trim();
            string userName = txtUserName.Text.Trim();

            if (string.IsNullOrEmpty(operatorName))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.txtOperatorName.Location.X + this.txtOperatorName.Width,
                    this.txtOperatorName.Location.Y);
                this.toolTip.Show(!_isModify ? "请输入登录名称！" : "请输入登录名称！", this, showLocation, 5000);
                this.txtOperatorName.Focus();
                return false;
            }

            if (operatorPwd.Length < 6)
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Warning;
                this.toolTip.ToolTipTitle = !_isModify ? "添加警告" : "修改警告";
                Point showLocation = new Point(
                    this.txtOperatorPwd.Location.X + this.txtOperatorPwd.Width,
                    this.txtOperatorPwd.Location.Y);
                this.toolTip.Show("用户密码长度不能小于六位！", this, showLocation, 5000);
                this.txtOperatorPwd.Focus();
                return false;
            }

            if (validatePwd.Length < 6)
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Warning;
                this.toolTip.ToolTipTitle = !_isModify ? "添加警告" : "修改警告";
                Point showLocation = new Point(
                    this.txtValidatePwd.Location.X + this.txtValidatePwd.Width,
                    this.txtValidatePwd.Location.Y);
                this.toolTip.Show("确认密码长度不能小于六位！", this, showLocation, 5000);
                this.txtValidatePwd.Focus();
                return false;
            }

            if (operatorPwd != validatePwd)
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Warning;
                this.toolTip.ToolTipTitle = !_isModify ? "添加警告" : "修改警告";
                Point showLocation = new Point(
                    this.txtValidatePwd.Location.X + this.txtValidatePwd.Width,
                    this.txtValidatePwd.Location.Y);
                this.toolTip.Show("两次输入的密码必须一致！", this, showLocation, 5000);
                this.txtValidatePwd.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(userName))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.txtUserName.Location.X + this.txtUserName.Width,
                    this.txtUserName.Location.Y);
                this.toolTip.Show(!_isModify ? "请输入姓名！" : "请输入姓名！", this, showLocation, 5000);
                this.txtUserName.Focus();
                return false;
            }

            

            return true;
        }

    }
}