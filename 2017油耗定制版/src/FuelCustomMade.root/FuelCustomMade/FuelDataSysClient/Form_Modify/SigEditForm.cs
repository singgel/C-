using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.IO;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_Modify
{
    public partial class SigEditForm : DevExpress.XtraEditors.XtraForm
    {
        FuelFileUpload.FileUploadService service = Utils.serviceFiel;
        DefaultDirectory defaultDirectory = new DefaultDirectory();

        public SigEditForm(string imageOldName,string dateTime)
        {
            InitializeComponent();
            this.labImgName.Text = imageOldName;
            this.labDateTime.Text = dateTime;
            this.pictureBox1.Image = downLoadImage(imageOldName);
        }

        /// <summary>
        /// 选择电子签章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = " 图片文件(*.png;*.jpg;*.bmp) | *.png;*.jpg;*.bmp ";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.labImagePath.Text = ofd.FileName;
                this.pictureBox2.Image = Image.FromFile(ofd.FileName);
            }
        }

        /// <summary>
        /// 上传电子签章
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] imageStream = getImageStream();
            if (imageStream != null)
            {
                string imageOldName = this.labImgName.Text;
                DateTime dateTime = Convert.ToDateTime(this.labDateTime.Text);
                string imageNewName = String.Format(@"{0}_{1}{2}", Utils.userId, DateTime.Now.ToString("yyyyMMddhhmmss"), Path.GetExtension(this.labImagePath.Text));
                int a = service.UpdateSignature(imageStream, Utils.userId, Utils.password, Utils.qymc, imageNewName, imageOldName, dateTime, DateTime.Now, 1);
                if (a > 0)
                {
                    string imagePath = String.Format(@"{0}\{1}", defaultDirectory.Signature, imageNewName);
                    File.Copy(this.labImagePath.Text, imagePath, true);
                    MessageBox.Show("上传成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("上传失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("请选择要上传的图片！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //下载电子签章
        private Image downLoadImage(string imageOldName)
        {
            string imageOld = String.Format(@"{0}\{1}", defaultDirectory.Signature, imageOldName);
            if (!File.Exists(imageOld))
            {
                byte[] bs = service.DownloadFile(Utils.userId, Utils.password, imageOldName, "signature");
                if (bs.Length > 0)
                {
                    FileStream stream = new FileStream(String.Format(@"{0}\{1}", defaultDirectory.Signature, imageOldName), FileMode.Create);
                    stream.Write(bs, 0, bs.Length);
                    stream.Flush();
                    stream.Close();
                }
            }
            return Image.FromFile(imageOld);
        }

        //获取图片流
        private byte[] getImageStream()
        {
            try
            {
                using (MemoryStream curImageStream = new MemoryStream())
                {
                    this.pictureBox2.Image.Save(curImageStream, System.Drawing.Imaging.ImageFormat.Png);
                    curImageStream.Flush();
                    byte[] bmpBytes = curImageStream.ToArray();
                    curImageStream.Close();
                    return bmpBytes;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}