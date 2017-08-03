using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using OAUS.Core;

namespace OAUS.Server
{
    public partial class AddFileVersionForm : Form
    {
        public event ESBasic.CbGeneric addSuccessEvent;
        private UpdateConfiguration fileConfig;
        private bool isNew = true;
        public AddFileVersionForm(UpdateConfiguration _fileConfig, FileUnit fileObject)
        {
            InitializeComponent();
            this.fileConfig = _fileConfig;
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");

            List<string> files = ESBasic.Helpers.FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
            files.Sort();
            this.comboBox1.DataSource = files;            

            if (fileObject != null)
            {
                this.isNew = false;
            }

            if (files.Count > 0)
            {
                this.comboBox1.SelectedIndex = 0;
            }

            if (!isNew)
            {
                this.comboBox1.Text = fileObject.FileRelativePath;
                this.textBox_version.Text = fileObject.Version.ToString();
                this.button1.Text = "修改";
                this.comboBox1.Enabled = false;
                this.Text = "修改";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileUnit fileObject = new FileUnit();
            fileObject.FileRelativePath = this.comboBox1.Text.Trim();
            try
            {
                fileObject.Version = float.Parse(this.textBox_version.Text.Trim());
            }
            catch
            {
                MessageBox.Show("版本格式输入不正确，请输入小数！");
                return;
            }
            if (this.isNew)
            {
                if (this.fileConfig.FileList.Contains(fileObject))
                {
                    MessageBox.Show("该文件已经存在，不能重复添加");
                    return;
                }
                else
                {
                    this.fileConfig.FileList.Add(fileObject);                   
                    this.fileConfig.Save();
                    this.addSuccessEvent();
                    this.Close();
                }
            }
            else
            {
                foreach (FileUnit file in this.fileConfig.FileList)
                {
                    if (file.FileRelativePath == this.comboBox1.Text.Trim())
                    {
                        file.Version = float.Parse(this.textBox_version.Text.Trim());                       
                        break;
                    }
                }                           
                this.fileConfig.Save();
                
                this.addSuccessEvent();
                this.Close();
            }
        }
    }
}
