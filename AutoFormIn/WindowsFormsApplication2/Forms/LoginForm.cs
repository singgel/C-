using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Assistant.Service;
using Assistant.Forms;
using Updater = AssistantUpdater;
using Assistant.DataProviders;


namespace Assistant
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Updater.LoginUser user = null;
            try
            {
                if(string.IsNullOrEmpty(this.textBox1.Text))
                    throw new ArgumentException("请输入用户名！");
                string password = "";
                if(this.textBox2.Text != null)
                    password = FileHelper.GetStrMd5(this.textBox2.Text).ToUpper();
                string machineInfo = MachineCodeUtils.MachineCodeUtil.GetComputerInfo();
                if(this.textBox1.Text == "admin")
                    machineInfo = "123";
                user = Updater.ServiceHelper.Login(this.textBox1.Text, password, machineInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            System.Diagnostics.Debug.Assert(user != null);
            Constants.CurrentUser = user;
            this.Hide();
            MainRibbonForm mainRibbonForm = new MainRibbonForm();
            mainRibbonForm.Show();
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            RegUserForm regUserForm = new RegUserForm();
            regUserForm.ShowDialog();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                SettingForm sf = new SettingForm();
                sf.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
