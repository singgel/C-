using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.IO;
using DevExpress.XtraEditors;

namespace FuelDataSysClient
{
    public partial class IndexForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        string comment = "公共消息";
        string policy = "相关政策";
        string enterprise = "企业消息";
        string userName = Utils.userId;
        string password = Utils.password;
        FuelFileUpload.FileUploadService service = Utils.serviceFiel;

        public IndexForm()
        {
            InitializeComponent();
            setEnterpriseMsg();
        }

        /// <summary>
        /// 定时刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.setEnterpriseMsg();
        }

        /// <summary>
        /// test webservice
        /// </summary>
        private void setEnterpriseMsg()
        {
            DataSet ds = service.QueryEnterpriseMsg(userName, password, enterprise);
            DataSet ds1 = service.QuerySystemMsg(userName, password, comment);
            DataSet ds2 = service.QuerySystemMsg(userName, password, policy);
            if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count > 0))
            {
                AddControls(this.slc1, ds);
            }
            if (((ds1 != null) && (ds1.Tables.Count > 0) && (ds1.Tables[0].Rows.Count > 0)) && ((ds2 == null) || (ds2.Tables.Count < 1) || (ds2.Tables[0].Rows.Count < 1)))
            {
                AddControls(this.slc2, ds1);
            }
            if (((ds1 == null) || (ds1.Tables.Count < 1) || (ds1.Tables[0].Rows.Count < 1)) && ((ds2 != null) && (ds2.Tables.Count > 0) && (ds2.Tables[0].Rows.Count > 0)))
            {
                AddControls(this.slc2, ds2);
            }
            if ((ds1 != null) && (ds1.Tables.Count > 0) && (ds1.Tables[0].Rows.Count > 0) && (ds2 != null) && (ds2.Tables.Count > 0) && (ds2.Tables[0].Rows.Count > 0))
            {
                ds1.Merge(ds2, true);
                AddControls(this.slc2, ds1);
            }
        }


        //添加控件
        public void AddControls(XtraScrollableControl xscl, DataSet ds)
        {
            DataView dv = ds.Tables[0].DefaultView;
            dv.RowFilter = "isDelete = 0";
            dv.Sort = "upTime Asc";
            DataTable dt = dv.ToTable();
            xscl.Controls.Clear();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                System.Windows.Forms.PictureBox pic = new System.Windows.Forms.PictureBox();
                pic.Name = string.Format("pic{0}", "asd");
                pic.Location = new Point(10, 25 + i * 30);
                pic.Width = 12; 
                pic.Height = 12;
                pic.SizeMode = PictureBoxSizeMode.Zoom;
                pic.Image = FuelDataSysClient.Properties.Resources.play_blue;
                xscl.Controls.Add(pic);

                System.Windows.Forms.LinkLabel txt = new System.Windows.Forms.LinkLabel();
                txt.Name = "ll_" + dt.Rows[i]["newFile"].ToString();
                txt.AutoSize = true;
                txt.Text = dt.Rows[i]["title"].ToString();// dt.Rows[i]["title"].ToString().Length > 10 ? dt.Rows[i]["title"].ToString().Substring(0, 10) + "..." : dt.Rows[i]["title"].ToString();
                txt.Location = new Point(30, 25 + i * 30);
                txt.LinkBehavior = LinkBehavior.NeverUnderline;
                txt.LinkColor = Color.Blue;

                txt.Click += new EventHandler(lbtn_Click);
                xscl.Controls.Add(txt);

                System.Windows.Forms.Label lab = new System.Windows.Forms.Label();
                lab.Name = "lab" + dt.Rows[i]["newFile"].ToString();
                lab.Text = dt.Rows[i]["upTime"].ToString();
                lab.Location = new Point(50+txt.Width, 25 + i * 30);
                xscl.Controls.Add(lab);
            }
        }

        protected void lbtn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.LinkLabel ob = (System.Windows.Forms.LinkLabel)sender;
            string[] obArr = ob.Name.Split('_');
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(obArr[1]))
            {
                try
                {
                    byte[] bs = service.DownloadFile(Utils.userId, Utils.password, obArr[1], "monitorSysMsg");
                    if (bs.Length > 0)
                    {
                        string titleName = ob.Text + Path.GetExtension(obArr[1]);
                        string filePath = folderBrowserDialog.SelectedPath;
                        FileStream stream = new FileStream(String.Format(@"{0}\{1}", filePath, titleName), FileMode.CreateNew);
                        stream.Write(bs, 0, bs.Length);
                        stream.Flush();
                        stream.Close();
                        MessageBox.Show("提示：下载成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("提示:该文件不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("提示：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

    }
}
