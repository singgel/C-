using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Data.SqlClient;
using System.Data.OleDb;
using FuelDataSysClient.Tool;
using System.IO;

namespace FuelDataSysClient.Form_SJBG
{
    public partial class EnterpriseAuthorityForm : DevExpress.XtraEditors.XtraForm
    {
        readonly FuelFileUpload.FileUploadService service = Utils.serviceFiel;

        public EnterpriseAuthorityForm()
        {
            InitializeComponent();
            initControlsText();
        }

        //关闭当前窗体
        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //初始化数据
        private void initControlsText()
        {
            this.te_user_id.Text = Utils.qymc;
            this.te_userName.Text = Utils.userId;
            //端口权限
            var ds1 = service.QueryPortPERMISSION(Utils.qymc, Utils.userId);
            if (ds1 != null)
            {
                var dt1 = ds1.Tables[0];
                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    if ("INSERT_O".Equals(dt1.Rows[i]["PERMISSION_TYPE"]))
                    {
                        ce_bc.Checked = true;
                        this.te_starttime_BC.Text = dt1.Rows[i]["STARTTIME"].ToString();
                        this.te_endtime_BC.Text = dt1.Rows[i]["ENDTIME"].ToString();
                        continue;
                    }
                    else if ("DELETE".Equals(dt1.Rows[i]["PERMISSION_TYPE"]))
                    {
                        ce_cx.Checked = true;
                        this.te_starttime_XG.Text = dt1.Rows[i]["STARTTIME"].ToString();
                        this.te_endtime_XG.Text = dt1.Rows[i]["ENDTIME"].ToString();
                        continue;
                    }
                    else if ("UPDATE".Equals(dt1.Rows[i]["PERMISSION_TYPE"]))
                    {
                        ce_xg.Checked = true;
                        this.te_starttime_CX.Text = dt1.Rows[i]["STARTTIME"].ToString();
                        this.te_endtime_CX.Text = dt1.Rows[i]["ENDTIME"].ToString();
                        continue;
                    }
                }
            }
            //签章权限
            var ds2 = service.QuerySignatureByQymc(Utils.userId, Utils.password, Utils.qymc);
            if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0] != null && ds2.Tables[0].Rows.Count > 0)
            {
                var dt2 = ds2.Tables[0];
                if (!string.IsNullOrEmpty(dt2.Rows[0]["IMG_NEW_NAME"].ToString()) && DownFile(dt2.Rows[0]["IMG_NEW_NAME"].ToString()) != null)
                {
                    this.pictureEdit1.Image = Image.FromFile(String.Format(@"{0}\{1}", DefaultDirectory.Signature, dt2.Rows[0]["IMG_NEW_NAME"]));
                }
                if (dt2.Rows[0]["STATUS"].ToString() == "0")
                {
                    this.checkEdit0.Checked = true;
                }
                if (dt2.Rows[0]["STATUS"].ToString() == "1")
                {
                    this.checkEdit1.Checked = true;
                }
                if (dt2.Rows[0]["STATUS"].ToString() == "2")
                {
                    this.checkEdit2.Checked = true;
                }
                if (dt2.Rows[0]["STATUS"].ToString() == "3")
                {
                    this.checkEdit3.Checked = true;
                }
            }
        }

        //下载文件
        private Image DownFile(string imageName)
        {
            if (!File.Exists(String.Format(@"{0}\{1}", DefaultDirectory.Signature, imageName)))
            {
                byte[] bs = service.DownloadFile(Utils.userId, Utils.password, imageName, "signature");
                FileStream stream = new FileStream(String.Format(@"{0}\{1}", DefaultDirectory.Signature, imageName), FileMode.CreateNew);
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
            }
            return Image.FromFile(String.Format(@"{0}\{1}", DefaultDirectory.Signature, imageName));
        }

    }
}