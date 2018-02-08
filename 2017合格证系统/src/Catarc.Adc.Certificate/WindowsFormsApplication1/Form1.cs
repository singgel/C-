using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using HtmlAgilityPack;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            CookieContainer cookieContainer = Login();


            string tablelist = ReadHTML("http://resource.autoidc.cn/pages/list.aspx?RESOURCEKEY=HGZ", cookieContainer);
            this.textBox1.Text = tablelist;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(tablelist);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(@"/html[1]/body[1]/div[1]/div[1]/div[1]/table[1]/tbody[1]/tr");


            this.progressBar1.Maximum = nodes.Count;
            foreach (HtmlNode n in nodes)
            {
                string str = n.ChildNodes[11].ChildNodes[0].Attributes["href"].Value;

                this.progressBar1.Value += 1;
                Application.DoEvents();

                //System.Threading.Thread t = new Thread(new ThreadStart(() =>
                //{
                //    WriteLogin(ReadHTML("http://resource.autoidc.cn/pages/" + str, cookieContainer));


                //}));
                //t.Start();


            }





        }


        private void WriteLogin(string text)
        {
            this.Invoke(new EventHandler((_o, _e) =>
            {
                this.textBox1.Text += text + "\r\n";
                this.textBox1.SelectionStart = this.textBox1.Text.Length - 1;
            }));
        }




        private CookieContainer Login()
        {
            CookieContainer cookieContainer = new CookieContainer();
            ///////////////////////////////////////////////////  
            // 1.打开 MyLogin.aspx 页面，获得 GetVeiwState & EventValidation  
            ///////////////////////////////////////////////////                  
            // 设置打开页面的参数  
            byte[] postData = Encoding.ASCII.GetBytes("__EVENTARGUMENT=&__EVENTTARGET=login&__EVENTVALIDATION=%2FwEdAAT0DMIHeQWCXCoQAfP2Wj0fKhoCyVdJtLIis5AgYZ%2FRYe4sciJO3Hoc68xTFtZGQEgrU8x5SglfzmEU2KqYFKCX&__VIEWSTATE=%2FwEPDwULLTIxMzY1ODgzNDNkZA%3D%3D&password=uer&username=user");

            HttpWebRequest request = WebRequest.Create("http://resource.autoidc.cn/pages/login.aspx") as HttpWebRequest;
            request.CookieContainer = cookieContainer;
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            request.AllowAutoRedirect = true;

            // 提交请求数据  
            System.IO.Stream outputStream = request.GetRequestStream();
            outputStream.Write(postData, 0, postData.Length);
            outputStream.Close();


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            return cookieContainer;
        }





        private string ReadHTML(string url, CookieContainer cookieContainer)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.CookieContainer = cookieContainer;
            request.Method = "GET";
            request.KeepAlive = true;
            request.AllowAutoRedirect = true;


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            System.IO.Stream responseStream = response.GetResponseStream();
            System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}
