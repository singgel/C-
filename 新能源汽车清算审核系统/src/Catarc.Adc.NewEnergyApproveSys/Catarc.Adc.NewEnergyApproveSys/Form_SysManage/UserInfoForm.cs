using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using System.Collections;
using Catarc.Adc.NewEnergyApproveSys.Common;

namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    public partial class UserInfoForm : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 是否显示为修改密码界面 
        /// </summary>
        readonly bool _isModify = false;
        string userId = string.Empty;
        public static string loginId = string.IsNullOrEmpty(Settings.Default.LocalUserName) ? "" : Settings.Default.LocalUserName;
        public static Hashtable htRole = GridControlHelper.GetRoleName("");
        public static Hashtable htDept = GridControlHelper.GetDeptName("");

        public UserInfoForm()
        {
            InitializeComponent();

            string[] roleList = new string[htRole.Count+1];
            roleList[0] = "";
            htRole.Values.CopyTo(roleList,1);
            this.cbRole.Properties.Items.AddRange(roleList);

            string[] deptList = new string[htDept.Count+1];
            deptList[0] = "";
            htDept.Values.CopyTo(deptList,1);
            this.cbDept.Properties.Items.AddRange(deptList);
        }

        public UserInfoForm(string id)
        {
            InitializeComponent();
            _isModify = true;
            string[] roleList = new string[htRole.Count];
            htRole.Values.CopyTo(roleList, 0);
            this.cbRole.Properties.Items.AddRange(roleList);

            string[] deptList = new string[htDept.Count];
            htDept.Values.CopyTo(deptList, 0);
            this.cbDept.Properties.Items.AddRange(deptList);

            setConValues(id);
        }

        private void UserInfoForm_Load(object sender, EventArgs e)
        {
            if (this.Text.Equals("修改用户信息"))
            {
                this.txtName.Enabled = false;
                this.txtLoginName.Enabled = false;
            }
        }

        private void setConValues(string id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT U.ID,U.USERLOGINNAME,U.USERNAME,U.USERROLEID AS ROLENAME  ");
            sql.Append(",U.USERDEPTID AS DEPTNAME,U.USERTEL,U.USEREMAIL,TO_CHAR(U.MODIFYTIME,'yyyy-MM-dd hh24:mi:ss') MODIFYTIME  ");
            //sql.Append(",CASE  WHEN U.STATUS=0 THEN '有效' ELSE '无效' END STATUE  ");
            sql.Append(",U.STATUS STATUE  ");
            sql.Append("FROM SYS_USERINFO U   where 1=1 ");
            sql.AppendFormat(" AND U.ID='{0}' ", id);
            //修改时间
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql.ToString(), null);
            userId = id;
            this.txtName.Text = ds.Tables[0].Rows[0]["USERNAME"].ToString();
            this.txtLoginName.Text = ds.Tables[0].Rows[0]["USERLOGINNAME"].ToString();
            this.cbRole.Text = (string)htRole[ds.Tables[0].Rows[0]["ROLENAME"].ToString()];
            this.txtEmail.Text = ds.Tables[0].Rows[0]["USEREMAIL"].ToString();
            this.cbDept.Text = (string)htDept[ds.Tables[0].Rows[0]["DEPTNAME"].ToString()];
            this.txtPhone.Text = ds.Tables[0].Rows[0]["USERTEL"].ToString();
            this.chkState.Checked = ds.Tables[0].Rows[0]["STATUE"].ToString() == "0" ? true : false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            if (UserInputCheck())
            {
                if (!_isModify)
                {
                    if (OracleHelper.Exists(OracleHelper.conn, string.Format("SELECT COUNT(*) FROM SYS_USERINFO WHERE USERLOGINNAME='{0}'", this.txtLoginName.Text.ToLower().Trim())))
                    {
                        XtraMessageBox.Show(string.Format("用户账号{0}已存在", this.txtLoginName.Text), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                string OperatorName = this.txtName.Text.Trim();
                string LoginName = this.txtLoginName.Text.Trim();
                string strRoleID = string.Empty;
                foreach (System.Collections.DictionaryEntry de in htRole)
                {
                    if (de.Value.ToString() == this.cbRole.Text.Trim())
                    {
                        strRoleID = de.Key.ToString();//得到key
                        break;//退出foreach遍历
                    }
                }
                string DeptID = string.Empty;
                foreach (System.Collections.DictionaryEntry de in htDept)
                {
                    if (de.Value.ToString() == this.cbDept.Text)
                    {
                        DeptID = de.Key.ToString();//得到key
                        break;//退出foreach遍历
                    }
                }

                string Phone = String.IsNullOrEmpty(this.txtPhone.Text) ? null : this.txtPhone.Text;
                string Email = String.IsNullOrEmpty(this.txtEmail.Text) ? null : this.txtEmail.Text;

                int status = this.chkState.Checked ? 0 : 1;//0有效1无效
                if (string.IsNullOrEmpty(userId))
                {
                    userId = Guid.NewGuid().ToString();
                }
                string Password = EncryptUtil.Md532(LoginName.ToLower().Trim()) ;//string.Empty;//初始密码跟账号保持一致

                //新增
                StringBuilder sbInsSQL = new StringBuilder();
                sbInsSQL.Append("INSERT INTO SYS_USERINFO (ID,MODIFYTIME,USERDEPTID");
                sbInsSQL.Append(",USEREMAIL,USERLOGINNAME,USERNAME,PWD,USERROLEID,USERTEL,STATUS) VALUES ('{0}',sysdate,'{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8}) ");
                string strInsSQL = string.Format(sbInsSQL.ToString(), userId, DeptID, Email, LoginName.ToLower().Trim(), OperatorName, Password, strRoleID, Phone, status);

                //修改
                StringBuilder sbUpdSQL = new StringBuilder();
                sbUpdSQL.AppendFormat("UPDATE SYS_USERINFO SET MODIFYTIME=sysdate");
                sbUpdSQL.AppendFormat(",USEREMAIL='{0}',USERLOGINNAME='{1}',USERNAME='{2}',USERDEPTID='{3}'", Email, LoginName, OperatorName, DeptID);
                sbUpdSQL.AppendFormat(",USERROLEID='{0}',USERTEL='{1}',STATUS={2} ", strRoleID, Phone, status);
                sbUpdSQL.AppendFormat("WHERE ID='{0}' ", userId);

                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();

                    int count = OracleHelper.ExecuteNonQuery(con, _isModify ? sbUpdSQL.ToString() : strInsSQL, null);
                    if (count > 0 )
                    {
                        this.Close();
                        UserForm uf = new UserForm();
                        uf.SearchLocal(1); 
                        XtraMessageBox.Show("操作成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else { XtraMessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            }
        }

        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <returns></returns>
        private bool UserInputCheck()
        {
            string operatorName = txtName.Text.Trim();
            string strLoginName = txtLoginName.Text.Trim();
            string strRole = cbRole.Text.Trim();
            string strDept = cbDept.Text.Trim();

            if (string.IsNullOrEmpty(operatorName))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.txtName.Location.X + this.txtName.Width,
                    this.txtName.Location.Y);
                this.toolTip.Show(!_isModify ? "请输入用户姓名！" : "请输入用户姓名！", this, showLocation, 5000);
                this.txtName.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(strLoginName))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.txtLoginName.Location.X + this.txtLoginName.Width,
                    this.txtLoginName.Location.Y);
                this.toolTip.Show(!_isModify ? "请输入登录账号！" : "请输入登录账号！", this, showLocation, 5000);
                this.txtLoginName.Focus();
                return false;
            }



            if (string.IsNullOrEmpty(strRole))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.cbRole.Location.X + this.cbRole.Width,
                    this.cbRole.Location.Y);
                this.toolTip.Show(!_isModify ? "请输入用户角色！" : "请输入用户角色！", this, showLocation, 5000);
                this.cbRole.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(strDept))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = !_isModify ? "添加提示" : "修改提示";
                Point showLocation = new Point(
                    this.cbDept.Location.X + this.cbDept.Width,
                    this.cbDept.Location.Y);
                this.toolTip.Show(!_isModify ? "请输入用户部门！" : "请输入用户部门！", this, showLocation, 5000);
                this.cbDept.Focus();
                return false;
            }

            return true;
        }

    }
}