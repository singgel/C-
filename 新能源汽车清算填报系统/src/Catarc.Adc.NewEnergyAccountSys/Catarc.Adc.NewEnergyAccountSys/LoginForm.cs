using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using Catarc.Adc.NewEnergyAccountSys.Form_Set;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyAccountSys.FormUtils;
using Catarc.Adc.NewEnergyAccountSys.Common;

namespace Catarc.Adc.NewEnergyAccountSys
{
    public partial class LoginForm : DevExpress.XtraEditors.XtraForm
    {
        public static NewEnergyWeb.INewEnergyClearingServiceService NewEnergyService = NewEnergyUtils.newEnergyservice;
        public LoginForm()
        {
            InitializeComponent();
            //按钮显示
            string Item = this.Text;
            List<string> ButtonModel = Authority.ReadMenusXmlData("AuthorityUrl").Where(c => Item.Contains(c.ParentID.ToString())).Select(c => c.MenuName).ToList<string>();
            if (ButtonModel.Contains("设置"))
            {
                pictureBox1.Visible = true;
                labelControl4.Visible = true;
                cb_MFRSName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                cb_ClearYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            }
            else
            {
                pictureBox1.Visible = false;
                labelControl4.Visible = false;
                cb_MFRSName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cb_ClearYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            }
            BindData();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.cb_MFRSName.Text = Settings.Default.Vehicle_MFCS;
            this.cb_ClearYear.Text = Settings.Default.ClearYear;
        }

        //绑定企业数据
        private void BindData()
        {
            string sql_Name = @"select DISTINCT MFRSName from NerdsMFRSName";

            DataSet ds_Name = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql_Name, null);
            if (ds_Name != null && ds_Name.Tables[0].Rows.Count > 0)
            {
                cb_MFRSName.Properties.Items.Clear();
                foreach (DataRow company in ds_Name.Tables[0].Rows)
                {
                    cb_MFRSName.Properties.Items.Add(company[0]);
                }
            }
            else
            {
                cb_MFRSName.Properties.Items.Clear();
            }
            this.cb_ClearYear.Properties.Items.Add("2016");
        }

        //登录
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
                {
                    if (string.IsNullOrEmpty(Settings.Default.UserName) || string.IsNullOrEmpty(Settings.Default.UserPwd))
                    {
                        MessageBox.Show("请先设置新能源汽车的用户名和密码!");
                        return;
                    }
                    else
                    {
                        getQyName(Settings.Default.UserName, Settings.Default.UserPwd);
                    }
                    if (!string.IsNullOrEmpty(this.cb_MFRSName.Text) && !string.IsNullOrEmpty(this.cb_ClearYear.Text) && AccessHelper.Exists(AccessHelper.conn, String.Format(@"SELECT * FROM NerdsMFRSName WHERE MFRSName='{0}' AND ClearYear='{1}'", this.cb_MFRSName.Text, this.cb_ClearYear.Text)))
                    {
                        Settings.Default.Vehicle_MFCS = this.cb_MFRSName.Text;
                        Settings.Default.ClearYear = this.cb_ClearYear.Text;
                        Settings.Default.Save();
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("车辆生产企业和清算年份不匹配!");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.cb_MFRSName.Text) && !string.IsNullOrEmpty(this.cb_ClearYear.Text))
                    {
                        Settings.Default.Vehicle_MFCS = this.cb_MFRSName.Text;
                        Settings.Default.ClearYear = this.cb_ClearYear.Text;
                        Settings.Default.Save();
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("车辆生产企业和清算年份不能为空!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cb_ClearYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        private void LoginForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin.PerformClick();
            }
        }

        //设置
        private void labelControl4_Click(object sender, EventArgs e)
        {
            using (var setform = new SetForm())
            {
                setform.ShowDialog();
            }
        }

        //设置
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (var setform = new SetForm())
            {
                setform.ShowDialog();
            }
        }

    }
}