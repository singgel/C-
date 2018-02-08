using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Xml;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using System.Linq;
using System.Data.OleDb;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using Catarc.Adc.NewEnergyAccountSys.FormUtils;
using Catarc.Adc.NewEnergyAccountSys.Common;
using Catarc.Adc.NewEnergyAccountSys.DevForm;
using DevExpress.XtraSplashScreen;

namespace Catarc.Adc.NewEnergyAccountSys.Form_Set
{
    public partial class ContactsForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ContactsForm()
        {
            InitializeComponent();
        }

        private void ContactsForm_Load(object sender, EventArgs e)
        {
            //按钮显示
            string Item = this.Text;
            List<string> ButtonModel = Authority.ReadMenusXmlData("AuthorityUrl").Where(c => Item.Contains(c.ParentID.ToString())).Select(c => c.MenuName).ToList<string>();
            if (ButtonModel.Contains("基础版"))
            {
                cb_Company.Enabled = false;
                cb_ClearYear.Enabled = false;
            }
            else
            {
                cb_Company.Enabled = true;
                cb_ClearYear.Enabled = true;
            }
            foreach (BarItemLink link in ribbonPageGroup1.ItemLinks)
            {
                if (ButtonModel.Contains(link.Caption))
                {
                    link.Item.Visibility = BarItemVisibility.Always;
                }
                else
                {
                    link.Item.Visibility = BarItemVisibility.Never;
                }
            }
            string str_Company = Settings.Default.Vehicle_MFCS;
            string ClearYear = Settings.Default.ClearYear;
            if (string.IsNullOrEmpty(str_Company) || string.IsNullOrEmpty(ClearYear))
            {
                MessageBox.Show("请先设置车辆生产企业名称和清算年份", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //将企业名称和年份绑定在页面上
            this.cb_Company.Text = Settings.Default.Vehicle_MFCS;
            this.cb_ClearYear.Text = Settings.Default.ClearYear;
            BindQyData();
            //绑定企业联络人数据
            BindData(str_Company, ClearYear);
        }

        //绑定企业数据
        private void BindQyData()
        {
            string sql_Name = @"select DISTINCT MFRSName from NerdsMFRSName";
            string sql_Year = @"select DISTINCT ClearYear from NerdsMFRSName";

            DataSet ds_Name = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql_Name, null);
            DataSet ds_Year = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql_Year, null);
            if (ds_Name != null && ds_Name.Tables[0].Rows.Count > 0)
            {
                cb_Company.Properties.Items.Clear();
                foreach (DataRow company in ds_Name.Tables[0].Rows)
                {
                    cb_Company.Properties.Items.Add(company[0]);
                } 
            }
            else
            {
                cb_Company.Properties.Items.Clear();
            }

            if (ds_Year != null && ds_Year.Tables[0].Rows.Count > 0)
            {
                cb_ClearYear.Properties.Items.Clear();
                foreach (DataRow ClearYear in ds_Year.Tables[0].Rows)
                {
                    cb_ClearYear.Properties.Items.Add(ClearYear[0]);
                }
            }
            else
            {
                cb_ClearYear.Properties.Items.Clear();
            }


        }
        // 页面绑定数据
        private void BindData(string VehicleCompany, string ClearYear)
        {
            try
            {
                string sql = @"select * from CONTRACT_USER where AutoFill_Manufacturer = @AutoFill_Manufacturer and LiquYear = @LiquYear";
                OleDbParameter[] param = {
                                    new OleDbParameter("@AutoFill_Manufacturer",VehicleCompany),
                                    new OleDbParameter("@LiquYear",ClearYear)
                                    };
                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, param);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    //this.txt_Company.Text = dt.Rows[0]["AutoFill_Manufacturer"].ToString();
                    this.txt_NewName.Text = dt.Rows[0]["Head_Name"].ToString();
                    this.txt_NewDepartment.Text = dt.Rows[0]["Head_Department"].ToString();
                    this.txt_NewPost.Text = dt.Rows[0]["Head_Post"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Head_Tel"].ToString()))
                    {
                        string Head_num = dt.Rows[0]["Head_Tel"].ToString();

                        this.txt_NewMobile1.Text = Head_num.Substring(0, Head_num.IndexOf("-"));
                        this.txt_NewMobile2.Text = Head_num.Substring(Head_num.IndexOf("-") + 1);
                    }
                    this.txt_NewPhone.Text = dt.Rows[0]["Head_Phone"].ToString();
                    this.txt_NewMail.Text = dt.Rows[0]["Head_Email"].ToString();
                    this.txt_MainName.Text = dt.Rows[0]["Contact_Name"].ToString();
                    this.txt_MainDepartment.Text = dt.Rows[0]["Contact_Department"].ToString();
                    this.txt_MainPost.Text = dt.Rows[0]["Contact_Post"].ToString();
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Contact_Tel"].ToString()))
                    {
                        string Contact_num = dt.Rows[0]["Contact_Tel"].ToString();

                        this.txt_MainMobile1.Text = Contact_num.Substring(0, Contact_num.IndexOf("-"));
                        this.txt_MainMobile2.Text = Contact_num.Substring(Contact_num.IndexOf("-") + 1);
                    }
                    this.txt_MainPhone.Text = dt.Rows[0]["Contact_Phone"].ToString();
                    this.txt_MainMail.Text = dt.Rows[0]["Contact_Email"].ToString();

                }
                else
                {
                    this.txt_NewName.Text = string.Empty;
                    this.txt_NewDepartment.Text = string.Empty;
                    this.txt_NewPost.Text = string.Empty;

                    this.txt_NewMobile1.Text = string.Empty;
                    this.txt_NewMobile2.Text = string.Empty;

                    this.txt_NewPhone.Text = string.Empty;
                    this.txt_NewMail.Text = string.Empty;
                    this.txt_MainName.Text = string.Empty;
                    this.txt_MainDepartment.Text = string.Empty;
                    this.txt_MainPost.Text = string.Empty;

                    this.txt_MainMobile1.Text = string.Empty;
                    this.txt_MainMobile2.Text = string.Empty;

                    this.txt_MainPhone.Text = string.Empty;
                    this.txt_MainMail.Text = string.Empty;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"错误",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
           
        }

        // 保存
        private void btn_Save_ItemClick(object sender, ItemClickEventArgs e)
        {
            string msg = string.Empty;
            string userName = Settings.Default.UserName;
            string userPwd = Settings.Default.UserPwd;
            string MFRS = this.cb_Company.Text;
            try
            {
                if (Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
                {
                    string result = NewEnergyUtils.newEnergyservice.checkEnterprise(userName, userPwd, MFRS);
                    if (result == "否")
                    {
                        MessageBox.Show("【用户名】【密码】与【车辆生产企业】不匹配，保存失败!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    } 
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
          
            
            msg += this.VerifyData();

            try
            {
                if (string.IsNullOrEmpty(msg))
                {
                    saveParam();
                    Settings.Default.Vehicle_MFCS = this.cb_Company.Text;
                    Settings.Default.ClearYear = this.cb_ClearYear.Text;
                    Settings.Default.Save();
                    MessageBox.Show("保存成功!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("请核对页面信息是否填写正确！\n" + msg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void saveParam()
        {
            using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
            {
                con.Open();
                using (OleDbTransaction tra = con.BeginTransaction())
                { 
                    try
                    {
                        string sqlParam = String.Format("DELETE FROM CONTRACT_USER WHERE AutoFill_Manufacturer ='{0}'", this.cb_Company.Text);
                        AccessHelper.ExecuteNonQuery(tra, sqlParam, null);
                        string sqlStr = "INSERT INTO CONTRACT_USER(AutoFill_Manufacturer, Head_Name, Head_Department, Head_Post, Head_Tel, Head_Phone, Head_Email, Contact_Name, Contact_Department, Contact_Post, Contact_Tel, Contact_Phone, Contact_Email, [LiquYear]) VALUES (@AutoFill_Manufacturer, @Head_Name, @Head_Department, @Head_Post, @Head_Tel, @Head_Phone, @Head_Email, @Contact_Name, @Contact_Department, @Contact_Post, @Contact_Tel, @Contact_Phone, @Contact_Email, @LiquYear)";
                        OleDbParameter[] param = { 
                                     new OleDbParameter("@AutoFill_Manufacturer",this.cb_Company.Text.Trim()),
                                     new OleDbParameter("@Head_Name",this.txt_NewName.Text.Trim()),
                                     new OleDbParameter("@Head_Department",this.txt_NewDepartment.Text.Trim()),
                                     new OleDbParameter("@Head_Post",this.txt_NewPost.Text.Trim()),
                                     new OleDbParameter("@Head_Tel",this.txt_NewMobile1.Text.Trim() + "-" + this.txt_NewMobile2.Text.Trim()),
                                     new OleDbParameter("@Head_Phone",this.txt_NewPhone.Text.Trim()),
                                     new OleDbParameter("@Head_Email",this.txt_NewMail.Text.Trim()),
                                     new OleDbParameter("@Contact_Name",this.txt_MainName.Text.Trim()),
                                     new OleDbParameter("@Contact_Department",this.txt_MainDepartment.Text.Trim()),
                                     new OleDbParameter("@Contact_Post",this.txt_MainPost.Text.Trim()),
                                     new OleDbParameter("@Contact_Tel", this.txt_MainMobile1.Text.Trim() + "-" +this.txt_MainMobile2.Text.Trim()),
                                     new OleDbParameter("@Contact_Phone",this.txt_MainPhone.Text.Trim()),
                                     new OleDbParameter("@Contact_Email",this.txt_MainMail.Text.Trim()),
                                     new OleDbParameter("@LiquYear",this.cb_ClearYear.Text.Trim())
                                     };

                        AccessHelper.ExecuteNonQuery(tra, sqlStr, param);
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        throw ex;
                    }
                }
            }
        }

        private void cb_Company_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str_Company = this.cb_Company.Text.Trim();
            string Clearyear = this.cb_ClearYear.Text.Trim();
            BindData(str_Company, Clearyear);
        }

        private string VerifyData()
        {
            string msg = string.Empty;
            if (this.txt_NewName.Text.Trim() == this.txt_MainName.Text.Trim())
            {
                msg += "负责人与联系人名字不可相同" + Environment.NewLine;
            }
            if (this.txt_NewPhone.Text.Trim() == this.txt_MainPhone.Text.Trim())
            {
                msg += "负责人与联系人手机不可相同" + Environment.NewLine;
            }
            if (this.txt_NewMail.Text.Trim() == this.txt_MainMail.Text.Trim())
            {
                msg += "负责人与联系人邮箱不可相同" + Environment.NewLine;
            }
            var VerifyDataDic = new Dictionary<string, string>();
            VerifyDataDic.Add("NewName", this.txt_NewName.Text.Trim());
            VerifyDataDic.Add("NewDepartment", this.txt_NewDepartment.Text.Trim());
            VerifyDataDic.Add("NewPost", this.txt_NewPost.Text.Trim());
            VerifyDataDic.Add("NewMobile1", this.txt_NewMobile1.Text.Trim());
            VerifyDataDic.Add("NewMobile2", this.txt_NewMobile2.Text.Trim());
            VerifyDataDic.Add("NewPhone", this.txt_NewPhone.Text.Trim());
            VerifyDataDic.Add("NewMail", this.txt_NewMail.Text.Trim());
            VerifyDataDic.Add("MainName", this.txt_MainName.Text.Trim());
            VerifyDataDic.Add("MainDepartment", this.txt_MainDepartment.Text.Trim());
            VerifyDataDic.Add("MainPost", this.txt_MainPost.Text.Trim());
            VerifyDataDic.Add("MainMobile1", this.txt_MainMobile1.Text.Trim());
            VerifyDataDic.Add("MainMobile2", this.txt_MainMobile2.Text.Trim());
            VerifyDataDic.Add("MainPhone", this.txt_MainPhone.Text.Trim());
            VerifyDataDic.Add("MainMail", this.txt_MainMail.Text.Trim());

            msg += ValidateParam.CheckQYInfomation(VerifyDataDic);
            return msg;

        }

        private void cb_ClearYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str_Company = this.cb_Company.Text.Trim();
            string Clearyear = this.cb_ClearYear.Text.Trim();
            BindData(str_Company, Clearyear);

        }

        private void btn_Refresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string str_Company = this.cb_Company.Text;
                string ClearYear = this.cb_ClearYear.Text;
                BindData(str_Company, ClearYear);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

      
 
    }
}