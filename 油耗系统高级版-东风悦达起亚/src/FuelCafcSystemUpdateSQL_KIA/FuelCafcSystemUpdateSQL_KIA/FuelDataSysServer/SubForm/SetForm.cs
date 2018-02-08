using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysServer.Tool;
using Oracle.ManagedDataAccess.Client;
using System.Net;


namespace FuelDataSysServer.SubForm
{
    public partial class SetForm : DevExpress.XtraEditors.XtraForm
    {

        public SetForm()
        {
            InitializeComponent();
            bool IsFuelTest = FuelDataSysServer.Properties.Settings.Default.IsFuelTest;
            if (IsFuelTest == true)
            {
                this.comboBox1.SelectedIndex = 1;
            }
            else
            {
                this.comboBox1.SelectedIndex = 0;
            }
            // 初始化网络代理信息
            bool IsProxy = FuelDataSysServer.Properties.Settings.Default.IsProxy;
            if (IsProxy)
            {
                this.ceProxy.Checked = IsProxy;
                this.pcProxy.Enabled = IsProxy;
                this.teProxyAddr.Text = FuelDataSysServer.Properties.Settings.Default.ProxyAddr;
                this.teProxyPort.Text = FuelDataSysServer.Properties.Settings.Default.ProxyPort;
                this.teProxyUserId.Text = FuelDataSysServer.Properties.Settings.Default.ProxyUserId;
                this.teProxyPwd.Text = FuelDataSysServer.Properties.Settings.Default.ProxyPwd;
            }
            else
            {
                this.ceProxy.Checked = false;
                this.pcProxy.Enabled = false;
            }

            // 初始化用户名密码信息
            this.teUserName.Text = FuelDataSysServer.Properties.Settings.Default.UserId;
            this.tePassword.Text = FuelDataSysServer.Properties.Settings.Default.UserPWD;
            //初始化数据库连接
            this.teDataAddr.Text = FuelDataSysServer.Properties.Settings.Default.DataAddr;
            this.teDataBase.Text = FuelDataSysServer.Properties.Settings.Default.DataBase;
            this.teDataPort.Text = FuelDataSysServer.Properties.Settings.Default.DataPort;
            this.teDataUserName.Text = FuelDataSysServer.Properties.Settings.Default.DataUserId;
            this.teDataPassword.Text = FuelDataSysServer.Properties.Settings.Default.DataUserPWD;
            //初始化数据地址

            this.tePORT1.Text = FuelDataSysServer.Properties.Settings.Default.SocketPort1;
            this.tePORT2_1.Text = FuelDataSysServer.Properties.Settings.Default.SocketPort2_1;
            this.tePORT3_1.Text = FuelDataSysServer.Properties.Settings.Default.SocketPort3_1;
            this.tePORT2_2.Text = FuelDataSysServer.Properties.Settings.Default.SocketPort2_2;
            this.tePORT3_2.Text = FuelDataSysServer.Properties.Settings.Default.SocketPort3_2;
        }

        // 保存所有设置
        private void btnSave_Click(object sender, EventArgs e)
        {
        
            string message = string.Empty;
            try
            {
                message = this.SaveNetLine();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                message = this.SaveProxyInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                message = this.SaveDatabaseInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                message = this.SaveUserInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                message = this.SaveSocketInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("保存成功！");
                }
                else
                {
                    MessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // 设置代理
        private void ceProxy_CheckStateChanged(object sender, EventArgs e)
        {
            if (this.ceProxy.Checked)
            {
                this.pcProxy.Enabled = true;
            }
            else
            {
                this.pcProxy.Enabled = false;
            }
        }

        // 保存上传线路设置
        protected string SaveNetLine()
        {
            string msg = string.Empty;
            try
            {
                string strVal = this.comboBox1.Text;
                if (string.IsNullOrEmpty(strVal))
                {
                    return "请选择上传线路";
                }
                if (strVal == "正式线路")
                {
                    FuelDataSysServer.Properties.Settings.Default.IsFuelTest = true;
                    Utils.isFuelTest = true;
                }
                else
                {
                    FuelDataSysServer.Properties.Settings.Default.IsFuelTest = false;
                    Utils.isFuelTest = false;
                }

                FuelDataSysServer.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }
            return msg;
        }

        // 保存代理设置
        protected string SaveProxyInfo()
        {
            string msg = string.Empty;
            try
            {
                bool IsProxy = this.ceProxy.Checked;
                FuelDataSysServer.Properties.Settings.Default.IsProxy = IsProxy;

                if (IsProxy)
                {
                    FuelDataSysServer.Properties.Settings.Default.ProxyAddr = this.teProxyAddr.Text.Trim();
                    FuelDataSysServer.Properties.Settings.Default.ProxyPort = this.teProxyPort.Text.Trim();
                    FuelDataSysServer.Properties.Settings.Default.ProxyUserId = this.teProxyUserId.Text.Trim();
                    FuelDataSysServer.Properties.Settings.Default.ProxyPwd = this.teProxyPwd.Text.Trim();
                }
                else
                {
                    FuelDataSysServer.Properties.Settings.Default.ProxyAddr = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.ProxyPort = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.ProxyUserId = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.ProxyPwd = string.Empty;
                }
                FuelDataSysServer.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        // 保存用户名密码
        protected string SaveUserInfo()
        {
            string msg = string.Empty;
            //string userName = this.teUserName.Text.Trim();
            //string userPassowrd = this.tePassword.Text.Trim();
            try
            {
                //var qymc = Utils.service.QueryQymc(userName, userPassowrd);
                //if (Utils.service.CheckUser(userName, userPassowrd) && !string.IsNullOrEmpty(qymc))
                //{
                //    FuelDataSysServer.Properties.Settings.Default.UserId = userName;
                //    FuelDataSysServer.Properties.Settings.Default.UserPWD = userPassowrd;
                //    FuelDataSysServer.Properties.Settings.Default.Qymc = qymc;
                //}
                //else
                //{
                //    msg = "用户名和密码错误\n";
                //    FuelDataSysServer.Properties.Settings.Default.UserId = string.Empty;
                //    FuelDataSysServer.Properties.Settings.Default.UserPWD = string.Empty;
                //    FuelDataSysServer.Properties.Settings.Default.Qymc = string.Empty;
                //}
                FuelDataSysServer.Properties.Settings.Default.UserId = "FADCCYDQYU001";
                FuelDataSysServer.Properties.Settings.Default.UserPWD = "6e63d8$F";
                FuelDataSysServer.Properties.Settings.Default.Qymc = "东风悦达起亚汽车有限公司";

                FuelDataSysServer.Properties.Settings.Default.Save();
                Utils.userId = FuelDataSysServer.Properties.Settings.Default.UserId;
                Utils.userPWD = FuelDataSysServer.Properties.Settings.Default.UserPWD;
                Utils.qymc = FuelDataSysServer.Properties.Settings.Default.Qymc;
                Utils.timeCons = FuelDataSysServer.Properties.Settings.Default.TimeConstrain;
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        //保存中间库连接信息
        protected string SaveDatabaseInfo()
        {
            string msg = string.Empty;
            try
            {
                FuelDataSysServer.Properties.Settings.Default.DataAddr = this.teDataAddr.Text.Trim();
                FuelDataSysServer.Properties.Settings.Default.DataPort = this.teDataPort.Text.Trim();
                FuelDataSysServer.Properties.Settings.Default.DataUserId = this.teDataUserName.Text.Trim();
                FuelDataSysServer.Properties.Settings.Default.DataUserPWD = this.teDataPassword.Text.Trim();
                FuelDataSysServer.Properties.Settings.Default.DataBase = this.teDataBase.Text.Trim();
                if (!checkOracleConn())
                {
                    msg = "数据库配置错误\n";
                    FuelDataSysServer.Properties.Settings.Default.DataAddr = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.DataPort = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.DataUserId = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.DataUserPWD = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.DataBase = string.Empty;
                }

                FuelDataSysServer.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        protected string SaveSocketInfo()
        {
            string msg = string.Empty;
            //string socketIP1 = this.teIP1.Text.Trim();
            //string socketIP2 = this.teIP2.Text.Trim();
            //string socketIP3 = this.teIP3.Text.Trim();
            string socketPORT1 = this.tePORT1.Text.Trim();
            string socketPORT2_1 = this.tePORT2_1.Text.Trim();
            string socketPORT3_1 = this.tePORT3_1.Text.Trim();
            string socketPORT2_2 = this.tePORT2_2.Text.Trim();
            string socketPORT3_2 = this.tePORT3_2.Text.Trim();
            try
            {
                if (this.dxErrorProvider1.HasErrors || string.IsNullOrEmpty(socketPORT1) || string.IsNullOrEmpty(socketPORT2_1) || string.IsNullOrEmpty(socketPORT3_1) || string.IsNullOrEmpty(socketPORT2_2) || string.IsNullOrEmpty(socketPORT3_2))
                {
                    msg = "数据地址错误\n";
                    FuelDataSysServer.Properties.Settings.Default.SocketPort1 = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort2_1 = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort3_1 = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort2_2 = string.Empty;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort3_2 = string.Empty;
                }
                else
                {
                    FuelDataSysServer.Properties.Settings.Default.SocketPort1 = socketPORT1;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort2_1 = socketPORT2_1;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort3_1 = socketPORT3_1;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort2_2 = socketPORT2_2;
                    FuelDataSysServer.Properties.Settings.Default.SocketPort3_2 = socketPORT3_2;
                }

                FuelDataSysServer.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        private bool checkOracleConn()
        {
            string connectionString = String.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", this.teDataAddr.Text.Trim(), this.teDataPort.Text.Trim(), this.teDataBase.Text.Trim(), this.teDataUserName.Text.Trim(), this.teDataPassword.Text.Trim());
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

  
        private void tePORT1_Validating(object sender, CancelEventArgs e)
        {
            int number;
            if (!string.IsNullOrEmpty(this.tePORT1.Text.Trim()) && !int.TryParse(this.tePORT1.Text.Trim(), out number))
            {
                this.dxErrorProvider1.SetError(tePORT1, "一工厂的端口号不符合规则");
            }
            else
            {
                dxErrorProvider1.SetError(tePORT1, "");
            }
        }

        private void tePORT2_1_Validating(object sender, CancelEventArgs e)
        {
            int number;
            if (!string.IsNullOrEmpty(this.tePORT2_1.Text.Trim()) && !int.TryParse(this.tePORT2_1.Text.Trim(), out number))
            {
                this.dxErrorProvider1.SetError(tePORT2_1, "二工厂第一生产线的端口号不符合规则");
            }
            else
            {
                dxErrorProvider1.SetError(tePORT2_1, "");
            }
        }

        private void tePORT3_1_Validating(object sender, CancelEventArgs e)
        {
            int number;
            if (!string.IsNullOrEmpty(this.tePORT3_1.Text.Trim()) && !int.TryParse(this.tePORT3_1.Text.Trim(), out number))
            {
                this.dxErrorProvider1.SetError(tePORT3_1, "三工厂第一生产线的端口号不符合规则");
            }
            else
            {
                dxErrorProvider1.SetError(tePORT3_1, "");
            }
        }
        private void tePORT2_2_Validating(object sender, CancelEventArgs e)
        {
            int number;
            if (!string.IsNullOrEmpty(this.tePORT2_2.Text.Trim()) && !int.TryParse(this.tePORT2_2.Text.Trim(), out number))
            {
                this.dxErrorProvider1.SetError(tePORT2_2, "二工厂第二生产线的端口号不符合规则");
            }
            else
            {
                dxErrorProvider1.SetError(tePORT2_2, "");
            }
        }

        private void tePORT3_2_Validating(object sender, CancelEventArgs e)
        {
            int number;
            if (!string.IsNullOrEmpty(this.tePORT3_2.Text.Trim()) && !int.TryParse(this.tePORT3_2.Text.Trim(), out number))
            {
                this.dxErrorProvider1.SetError(tePORT3_2, "三工厂第二生产线的端口号不符合规则");
            }
            else
            {
                dxErrorProvider1.SetError(tePORT3_2, "");
            }
        }
    }
}