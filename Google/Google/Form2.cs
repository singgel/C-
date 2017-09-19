using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//Download by http://www.codefans.net
namespace Google
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            try
            {
                
                //加载地图

                string address = "File:\\" + Application.StartupPath + "\\search.html";

                Uri url = new Uri(address);

                webBrowser1.Url = url;

                webBrowser1.ScriptErrorsSuppressed = false;

            }

            catch (Exception except)
            {

                MessageBox.Show(except.Message, "提示！",

                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


            this.SetAllWebItemSelf(this.webBrowser1.Document.All);  //complete之前，将document上面所有的的控件的target 设置成_self，避免，新窗口变成IE默认打开



        }

        private void webBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            try
            {
                string url = this.webBrowser1.Document.ActiveElement.GetAttribute("href");

                this.webBrowser1.Url = new Uri(url);
            }
            catch
            {
            }


        }

        private void SetAllWebItemSelf(HtmlElementCollection items)    //装载回来的网页上面所有的控件的target属性修改还_self
        {
            try
            {
                foreach (HtmlElement item in items)
                {
                    if (item.TagName.ToLower().Equals("iframe", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        try
                        {
                            item.SetAttribute("target", "_self");
                        }
                        catch
                        { }
                    }
                    else
                    {
                        try
                        {
                            HtmlElementCollection fitems = item.Document.Window.Frames[item.Name].Document.All;

                            this.SetAllWebItemSelf(fitems);
                        }
                        catch
                        { }
                    }
                }
            }
            catch
            {
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 frm = new Form1();
            frm.Show();
        }


    }
}
