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

namespace FuelDataSysClient.Form_SJBG
{
    public partial class SigAddForm : DevExpress.XtraEditors.XtraForm
    {
        readonly FuelFileUpload.FileUploadService service = Utils.serviceFiel;

        public SigAddForm()
        {
            InitializeComponent();
        }

        //选择电子签章
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = " 图片文件(*.png;*.jpg;*.bmp) | *.png;*.jpg;*.bmp ";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.labImagePath.Text = ofd.FileName;
                this.pictureEdit1.Image = Image.FromFile(ofd.FileName);
            }
        }

        //上传电子签章
        private void button2_Click(object sender, EventArgs e)
        {
            byte[] imageStream = getImageStream();
            if (imageStream != null)
            {
                string extesion = Path.GetExtension(this.labImagePath.Text);
                string imageName = String.Format(@"{0}_{1}{2}", Utils.userId, DateTime.Now.ToString("yyyyMMddhhmmss"), extesion);
                int a = service.UpdateSignature(imageStream, Utils.userId, Utils.password, Utils.qymc, imageName, string.Empty, DateTime.Now, DateTime.Now, 0);
                if (a > 0)
                {
                    string imagePath = String.Format(@"{0}\{1}", DefaultDirectory.Signature, imageName);
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

        //获取图片流
        private byte[] getImageStream()
        {
            try
            {
                using (MemoryStream curImageStream = new MemoryStream())
                {
                    this.pictureEdit1.Image.Save(curImageStream, System.Drawing.Imaging.ImageFormat.Png);
                    curImageStream.Flush();
                    byte[] bmpBytes = curImageStream.ToArray();
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