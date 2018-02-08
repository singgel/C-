using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Catarc.Adc.NewEnergyAccountSys.FormUtils;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using System.Linq;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;

namespace Catarc.Adc.NewEnergyAccountSys.Form_Set
{
    public partial class SetForm : DevExpress.XtraEditors.XtraForm
    {
        public static NewEnergyWeb.INewEnergyClearingServiceService NewEnergyService = NewEnergyUtils.newEnergyservice;
        public SetForm()
        {
            InitializeComponent();
            this.UserName.Text = Settings.Default.UserName;
            this.Password.Text = Settings.Default.UserPwd;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            try
            {
                message = this.SaveUserInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrEmpty(message))
                {
                    //将企业名称保存进数据库
                    string username = Settings.Default.UserName;
                    string Pwd = Settings.Default.UserPwd;
                    getQyName(username, Pwd);
                    MessageBox.Show("保存成功！");
                    this.Close();
                }
                else
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //如果设置了用户名和密码，刷新企业名称和年份列表
        private void getQyName(string userName, string userPwd)
        {
            NewEnergyWeb.qynameByYear[] qynameByyear = NewEnergyUtils.newEnergyservice.getNerdsQyname(userName, userPwd);
            DataTable dt = NewEnergyUtils.QyNameInfoS2DT(qynameByyear);

            if (qynameByyear != null && qynameByyear.Length > 0)
            {
                using (OleDbConnection conn = new OleDbConnection(AccessHelper.conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    using (var tra = conn.BeginTransaction())
                    {
                        string sql_delete = "delete from NerdsMFRSName";
                        AccessHelper.ExecuteNonQuery(tra, sql_delete, null);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string sql = String.Format("insert into NerdsMFRSName (MFRSName,ClearYear) values ('{0}','{1}')", dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
                            AccessHelper.ExecuteNonQuery(tra, sql, null);
                           
                        }
                        tra.Commit();
                    }
                   

                }
            }

        }
        protected string SaveUserInfo()
        {
            string msg = string.Empty;
            string userName = this.UserName.Text.Trim();
            string userPwd = this.Password.Text.Trim();
            try
            {
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPwd))
                {
                    msg = "用户名或密码错误";
                }
                else
                {
                    Settings.Default.UserName = userName;
                    Settings.Default.UserPwd = userPwd;
                    Settings.Default.Save();
                }
                   
                
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}