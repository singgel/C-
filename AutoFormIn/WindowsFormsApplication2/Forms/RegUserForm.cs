using Assistant.DataProviders;
using AssistantUpdater;
using System;
using System.Windows.Forms;

namespace Assistant.Forms
{
    public partial class RegUserForm : DevExpress.XtraEditors.XtraForm
    {
        public RegUserForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.textBox1.Text))
                    throw new ArgumentException("请输入用户名！");
                if (string.IsNullOrEmpty(this.textBox2.Text))
                    throw new ArgumentException("登录密码不能为空！");
                else if (this.textBox2.Text != this.textBox3.Text)
                    throw new ArgumentException("两次输入的密码不一致！");
                string entName = FileHelper.GetEntName();
                ServiceHelper.RegisteUser(this.textBox1.Text, this.textBox2.Text, entName, MachineCodeUtils.MachineCodeUtil.GetComputerInfo());
                MessageBox.Show("用户注册成功");
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //this.DialogResult = System.Windows.Forms.DialogResult.No;
            }
            finally { 
            
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}