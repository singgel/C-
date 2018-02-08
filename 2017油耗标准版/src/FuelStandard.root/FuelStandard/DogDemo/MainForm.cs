using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SuperDog;

namespace DogDemo
{
    public partial class MainForm : Form
    {
        public const string scope = "<dogscope />";
        public const int FileId = 0xfff4;    // file id of default read/write file
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DogFeature feature = DogFeature.Default;
            using (Dog dog = new Dog(feature))
            {
                DogStatus status = dog.Login(VendorCode.Code, scope);
                if ((null == dog) || !dog.IsLoggedIn())
                    return;
                DogFile file = dog.GetFile(FileId);
                if (!file.IsLoggedIn())
                {
                    // Not logged into a dog - nothing left to do.  
                    return;
                }
                int size = 0;
                status = file.FileSize(ref size);
                if (DogStatus.StatusOk != status)
                {
                    return;
                }

                byte[] newBytes = System.Text.Encoding.UTF8.GetBytes(this.textBox1.Text);//new byte[] { 1, 2, 3, 4, 5, 6, 7 };

                status = file.Write(newBytes, 0, newBytes.Length);
                if (DogStatus.StatusOk != status)
                {
                    return;
                }



                // read the contents of the file into a buffer
                byte[] bytes = new byte[size];
                status = file.Read(bytes, 0, bytes.Length);
                if (DogStatus.StatusOk != status)
                {
                    return;
                }
                this.textBox2.Text = System.Text.Encoding.UTF8.GetString(bytes);
            }
            //textBox3.Text = dapi.WriteData(this.textBox1.Text.Trim());
            //this.textBox2.Text = dapi.ReadDate();
            //textBox3.SelectionStart = textBox3.TextLength;
            //textBox3.ScrollToCaret();
            //textBox3.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DogFeature feature = DogFeature.Default;
            using (Dog dog = new Dog(feature))
            {
                DogStatus status = dog.Login(VendorCode.Code, scope);
                if ((null == dog) || !dog.IsLoggedIn())
                    return;
                DogFile file = dog.GetFile(FileId);
                if (!file.IsLoggedIn())
                {
                    // Not logged into a dog - nothing left to do.  
                    return;
                }
                int size = 0;
                status = file.FileSize(ref size);
                if (DogStatus.StatusOk != status)
                {
                    return;
                }

                // read the contents of the file into a buffer
                byte[] bytes = new byte[size];
                status = file.Read(bytes, 0, bytes.Length);
                if (DogStatus.StatusOk != status)
                {
                    return;
                }
                this.textBox4.Text = System.Text.Encoding.UTF8.GetString(bytes);
            }
        }
    }
}
