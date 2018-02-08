using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.OleDb;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient
{
    public partial class UpdateDateForm : DevExpress.XtraEditors.XtraForm
    {
        private List<string> vinList;

        public List<string> VinList
        {
            get { return vinList; }
            set { vinList = value; }
        }
        private string date;

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public UpdateDateForm()
        {
            InitializeComponent();
            this.deClzzrq.EditValue = DateTime.Today;
        }

        /// <summary>
        /// 保存进口时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.Date != null && this.Date.Equals("GH"))
                {
                    this.Date = this.deClzzrq.Text;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
                string vinStr = string.Empty;
                if (vinList != null && vinList.Count > 0)
                {
                    foreach (string vin in vinList)
                    {
                        vinStr += vin + "','";
                    }
                }

                vinStr = vinStr.Substring(0, vinStr.Length - 3);
                string sqlUpdateDate = string.Format(@"UPDATE FC_CLJBXX SET CLZZRQ=@CLZZRQ, UPLOADDEADLINE=@UPLOADDEADLINE WHERE VIN IN ('{0}')", vinStr);
                DateTime clzzrqDate = Convert.ToDateTime(deClzzrq.Text);
                OleDbParameter clzzrq = new OleDbParameter("@CLZZRQ", clzzrqDate);
                clzzrq.OleDbType = OleDbType.DBDate;

                DateTime uploadDeadlineDate = Utils.QueryUploadDeadLine(clzzrqDate);
                OleDbParameter uploadDeadline = new OleDbParameter("@UPLOADDEADLINE", uploadDeadlineDate);
                uploadDeadline.OleDbType = OleDbType.DBDate;

                OleDbParameter[] param = { clzzrq, uploadDeadline };

                using (OleDbConnection con = new OleDbConnection(AccessHelper.conn))
                {
                    AccessHelper.ExecuteNonQuery(con, sqlUpdateDate, param);
                }

                this.DialogResult = DialogResult.OK;
                MessageBox.Show("修改成功！", "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改失败："+ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}