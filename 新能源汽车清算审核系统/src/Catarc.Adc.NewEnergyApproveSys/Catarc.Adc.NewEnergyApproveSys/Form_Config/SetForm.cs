using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Data.OleDb;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using DevExpress.XtraEditors;
using System.Net;

namespace Catarc.Adc.NewEnergyApproveSys.Form_Config
{
    public partial class SetForm : DevExpress.XtraEditors.XtraForm
    {
        public SetForm()
        {
            InitializeComponent();
        }

        private void SetForm_Load(object sender, EventArgs e)
        {
            this.teDataAddr.Text = Settings.Default.DataAddr;
            this.teDataPort.Text = Settings.Default.DataPort;
            this.teDataUserName.Text = Settings.Default.DataUserId;
            this.teDataPassword.Text = Settings.Default.DataUserPWD;
            this.teDataBase.Text = Settings.Default.DataBase;
            this.teFtpAddr.Text = Settings.Default.FtpIPAddr;
            this.teFtpUserName.Text = Settings.Default.FtpUserID;
            this.teFtpPassword.Text = Settings.Default.FtpPassword;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string message = string.Empty;
            try
            {

                message = this.SaveDatabaseInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    XtraMessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                message += this.SaveFtpInfo();
                if (!string.IsNullOrEmpty(message))
                {
                    XtraMessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(message))
                {
                    XtraMessageBox.Show("保存成功！");
                }
                else
                {
                    XtraMessageBox.Show(message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, "保存失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //保存中间库连接信息
        protected string SaveDatabaseInfo()
        {
            string msg = string.Empty;
            try
            {
                Settings.Default.DataAddr = this.teDataAddr.Text.Trim();
                Settings.Default.DataPort = this.teDataPort.Text.Trim();
                Settings.Default.DataUserId = this.teDataUserName.Text.Trim();
                Settings.Default.DataUserPWD = this.teDataPassword.Text.Trim();
                Settings.Default.DataBase = this.teDataBase.Text.Trim();
                if (!checkOracleConn())
                {
                    msg = "数据库配置错误\n";
                    Settings.Default.DataAddr = string.Empty;
                    Settings.Default.DataPort = string.Empty;
                    Settings.Default.DataUserId = string.Empty;
                    Settings.Default.DataUserPWD = string.Empty;
                    Settings.Default.DataBase = string.Empty;
                }
                else
                {
                    Settings.Default.DataAddr = this.teDataAddr.Text.Trim();
                    Settings.Default.DataPort = this.teDataPort.Text.Trim();
                    Settings.Default.DataUserId = this.teDataUserName.Text.Trim();
                    Settings.Default.DataUserPWD = this.teDataPassword.Text.Trim();
                    Settings.Default.DataBase = this.teDataBase.Text.Trim();
                    Settings.Default.Save();
                }
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\n";
            }

            return msg;
        }

        //保存ftp服务连接信息
        protected string SaveFtpInfo()
        {
            string msg = string.Empty;
            try
            {
                Settings.Default.FtpIPAddr = this.teFtpAddr.Text.Trim();
                Settings.Default.FtpUserID = this.teFtpUserName.Text.Trim();
                Settings.Default.FtpPassword = this.teFtpPassword.Text.Trim();
                if (!checkFtpConn())
                {
                    msg = "ftp服务配置错误\n";
                    Settings.Default.FtpIPAddr = string.Empty;
                    Settings.Default.FtpUserID = string.Empty;
                    Settings.Default.FtpPassword = string.Empty;
                }
                else
                {
                    Settings.Default.FtpIPAddr = this.teFtpAddr.Text.Trim();
                    Settings.Default.FtpUserID = this.teFtpUserName.Text.Trim();
                    Settings.Default.FtpPassword = this.teFtpPassword.Text.Trim();
                    Settings.Default.Save();
                }
                Settings.Default.Save();
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

        private bool checkFtpConn()
        {
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(String.Format("ftp://{0}/", Settings.Default.FtpIPAddr)));
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Settings.Default.FtpUserID, Settings.Default.FtpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                return true; 
            }
            catch
            {
                return false;
            }
        }
    }
}