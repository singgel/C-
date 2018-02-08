using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.Common;
using Catarc.Adc.NewEnergyApproveSys.Properties;

namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    public partial class UpdateUserPWD : DevExpress.XtraEditors.XtraForm
    {

        readonly string userId = string.Empty;

        public UpdateUserPWD()
        {
            InitializeComponent();
        }
        public UpdateUserPWD(string id)
        {
            InitializeComponent();
            userId = id;
            //管理员重置密码时不需要输入原密码
            if ("admin".Equals(Settings.Default.LocalUserName))
            {
                labelControl1.Visible = false;
                txtOldPWD.Visible = false;
                label5.Visible = false;
            }
        }
        //提交
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (PWDInputCheck())
            {
                //管理员重置密码时不需要输入原密码
                if (!"admin".Equals(Settings.Default.LocalUserName) && !string.IsNullOrEmpty(userId))
                {
                    if (!OracleHelper.Exists(OracleHelper.conn, string.Format("SELECT COUNT(ID) FROM SYS_USERINFO WHERE ID='{0}' AND PWD='{1}' ", userId, EncryptUtil.Md532(this.txtOldPWD.Text.Trim()))))
                    {
                        XtraMessageBox.Show("原密码输入不正确！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                string strNewPWD = EncryptUtil.Md532(this.txtNewPWD.Text.Trim());
                //修改
                StringBuilder sbUpdSQL = new StringBuilder();
                sbUpdSQL.AppendFormat("UPDATE SYS_USERINFO SET PWD='{0}' WHERE ID='{1}' ", strNewPWD, userId);
                using (OracleConnection con = new OracleConnection(OracleHelper.conn))
                {
                    con.Open();
                    int count = OracleHelper.ExecuteNonQuery(con, sbUpdSQL.ToString(), null);
                    if (count > 0)
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
        //重置
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtOldPWD.Text = string.Empty;
            this.txtNewPWD.Text = string.Empty;
            this.txtNewPWD2.Text = string.Empty;

        }
        //验证
        private bool PWDInputCheck()
        {
            string strOldPWD = this.txtOldPWD.Text;
            string strNewPWD = this.txtNewPWD.Text;
            string strNewPWD2 = this.txtNewPWD2.Text;

            //管理员重置密码时不需要输入原密码
            if (!"admin".Equals(Settings.Default.LocalUserName) && !string.IsNullOrEmpty(userId))
            {
                if (string.IsNullOrEmpty(strOldPWD))
                {
                    this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                    this.toolTip.ToolTipTitle = "提示";
                    Point showLocation = new Point(
                        this.txtOldPWD.Location.X + this.txtOldPWD.Width,
                        this.txtOldPWD.Location.Y);
                    this.toolTip.Show("请输入原密码！", this, showLocation, 5000);
                    this.txtOldPWD.Focus();
                    return false;
                }
            }
             if (string.IsNullOrEmpty(strNewPWD))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = "提示";
                Point showLocation = new Point(
                    this.txtNewPWD.Location.X + this.txtNewPWD.Width,
                    this.txtNewPWD.Location.Y);
                this.toolTip.Show("请输入新密码！", this, showLocation, 5000);
                this.txtNewPWD.Focus();
                return false;
            }
            else if (string.IsNullOrEmpty(strNewPWD2))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = "提示";
                Point showLocation = new Point(
                    this.txtNewPWD2.Location.X + this.txtNewPWD2.Width,
                    this.txtNewPWD2.Location.Y);
                this.toolTip.Show("请再次输入新密码！", this, showLocation, 5000);
                this.txtNewPWD2.Focus();
                return false;
            }
            else if (!strNewPWD2.Equals(strNewPWD))
            {
                this.toolTip.ToolTipIcon = ToolTipIcon.Info;
                this.toolTip.ToolTipTitle = "提示";
                Point showLocation = new Point(
                    this.txtNewPWD.Location.X + this.txtNewPWD.Width,
                    this.txtNewPWD.Location.Y);
                this.toolTip.Show("两次输入的新密码不一致，请核对！", this, showLocation, 5000);
                this.txtNewPWD.Focus();
                return false;
            }
            return true;
        }

    }
}