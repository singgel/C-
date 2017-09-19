using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml;
//Download by http://www.codefans.net
namespace Google
{


    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            try
            {

                //加载地图

                string address = "File:\\" + Application.StartupPath + "\\index.html";

                Uri url = new Uri(address);

                webBrowser1.Url = url;

                webBrowser1.ScriptErrorsSuppressed = false;

            }

            catch (Exception except)
            {

                MessageBox.Show(except.Message, "提示！",

                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }



        }

        private void zoombtn1_Click(object sender, EventArgs e)
        {
            try
            {

                mshtml.IHTMLDocument2 currentDoc = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;

                mshtml.IHTMLWindow2 win = (mshtml.IHTMLWindow2)currentDoc.parentWindow;

                win.execScript("ZoomInMap()", "javascript");

            }

            catch (Exception except)
            {

                MessageBox.Show(except.Message, "提示！",

                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


        }

        private void zoombtn2_Click(object sender, EventArgs e)
        {
            try
            {

                mshtml.IHTMLDocument2 currentDoc = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;

                mshtml.IHTMLWindow2 win = (mshtml.IHTMLWindow2)currentDoc.parentWindow;

                win.execScript("ZoomOutMap()", "javascript");

            }

            catch (Exception except)
            {

                MessageBox.Show(except.Message, "提示！",

                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


        }



        public string StrAddress = "";//记录用户输入的地址。


        private void checkbtn_Click(object sender, EventArgs e)
        {
            //int x = 100; // X 坐标的点击 
            //int y = 80; // Y 坐标的点击 
            //IntPtr handle = webBrowser1.Handle;
            //StringBuilder className = new StringBuilder(100);
            //while (className.ToString() != "Internet Explorer_Server") // 这类控制浏览器 
            //{
            //    handle = GetWindow(handle, 5); //得到一个处理子窗口 
            //    GetClassName(handle, className, className.Capacity);
            //}

            //IntPtr lParam = (IntPtr)((y << 16) | x); // 坐标 
            //IntPtr wParam = IntPtr.Zero; // 其他参数的点击（例如控制） 
            //const uint downCode = 0x201; // 左键点击代码
            //const uint upCode = 0x202; //左点击代码
            //SendMessage(handle, downCode, wParam, lParam); //鼠标按钮 
            //SendMessage(handle, upCode, wParam, lParam); // 鼠标按钮


            try
            {

                if (addressTextBox.Text.Trim() != "")
                {

                    mshtml.IHTMLDocument2 currentDoc = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;

                    mshtml.IHTMLWindow2 win = (mshtml.IHTMLWindow2)currentDoc.parentWindow;

                    win.execScript("codeAddress(\"" + addressTextBox + "\")", "javascript");

                }


            }

            catch (Exception except)
            {

                MessageBox.Show(except.Message, "提示！",

                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }


        }

        private void mapTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                mshtml.IHTMLDocument2 currentDoc = (mshtml.IHTMLDocument2)webBrowser1.Document.DomDocument;

                mshtml.IHTMLWindow2 win = (mshtml.IHTMLWindow2)currentDoc.parentWindow;

                switch (mapTypeComboBox.Text)
                {

                    case "电子地图":

                        win.execScript("SetRoadMap()", "javascript");

                        break;

                    case "卫星地图":

                        win.execScript("SetSatelliteMap()", "javascript");

                        break;

                    case "混合地图":

                        win.execScript("SetHybridMap()", "javascript");

                        break;

                    case "地形地图":

                        win.execScript("SetTerrainMap()", "javascript");

                        break;

                }

            }

            catch (Exception except)
            {

                MessageBox.Show(except.Message, "提示！",

                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                string tag01 = webBrowser1.Document.GetElementById("mouselatitute").InnerText;

                string tag02 = webBrowser1.Document.GetElementById("mouselongitude").InnerText;

                double lat, lng;

                if (double.TryParse(tag01, out lat)

                    && double.TryParse(tag02, out lng))
                {

                    currentXYStatusLabel.Text = "当前坐标："

                        + lat.ToString("F5") + "," + lng.ToString("F5");
                }

                double xmax, xmin, ymax, ymin;

                tag01 = webBrowser1.Document.GetElementById("XMax").InnerText;

                tag02 = webBrowser1.Document.GetElementById("XMin").InnerText;

                string tag03 = webBrowser1.Document.GetElementById("YMax").InnerText;

                string tag04 = webBrowser1.Document.GetElementById("YMin").InnerText;

                if (double.TryParse(tag01, out xmax)

                   && double.TryParse(tag02, out xmin)

                   && double.TryParse(tag03, out ymax)

                   && double.TryParse(tag04, out ymin))
                {

                    currentZoneStatusLabel.Text = "当前范围：XMin="

                        + xmin + ",XMax=" + xmax.ToString("F5")

                        + "; YMin=" + ymin.ToString("F5") + ",YMax=" + ymax.ToString("F5");
                }

                tag03 = webBrowser1.Document.GetElementById("ZoomClass").InnerText;

                int zoomclass;

                if (int.TryParse(tag03, out zoomclass))
                {
                    zoomclassStatusLabel.Text = "缩放等级：" + zoomclass.ToString();
                }

            }

            catch
            {
                //暂不处理

            }


        }

        private void 查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Form form2 = new Form2();
            form2.Owner = this;
            form2.Show();
            this.Hide();
        }

        private void btn2_search_Click(object sender, EventArgs e)
        {
            string t = this.tb1.Text.ToString();
            Geo g = new Geo(t);
            tb2.Text = g.Latitude.ToString();
            tb3.Text = g.Longtitude.ToString();
        }

        private void bt3_Search_Click(object sender, EventArgs e)
        {           
            string t1 = this.tb4.Text.ToString();
            string t2 = this.tb5.Text.ToString();            
            WebClient client = new WebClient(); //webclient客户端对象
            string url = "http://maps.google.com/maps/api/geocode/xml?latlng=" + t1 + "," + t2 + "&language=zh-CN&sensor=false";//请求地址 
            client.Encoding = Encoding.UTF8;//编码格式 
            string responseTest = client.DownloadString(url);
            //下载xml响应数据 
            string address = "";//返回的地址 
            XmlDocument doc = new XmlDocument();
            //创建XML文档对象 
            if (!string.IsNullOrEmpty(responseTest))
            {
                doc.LoadXml(responseTest);//加载xml字符串 
                //查询状态信息 
                string xpath = @"GeocodeResponse/status";
                XmlNode node = doc.SelectSingleNode(xpath);
                string status = node.InnerText.ToString();
                if (status == "OK")
                {
                    //查询详细地址信息 
                    xpath = @"GeocodeResponse/result/formatted_address";
                    node = doc.SelectSingleNode(xpath);
                    address = node.InnerText.ToString();
                    //查询地区信息 
                    XmlNodeList nodeListAll = doc.SelectNodes("GeocodeResponse/result");

                    XmlNode idt = nodeListAll[0];
                    XmlNodeList idts = idt.SelectNodes("address_component[type='sublocality']");
                    //address_component[type='sublocality']表示筛选type='sublocality'的所有相关子节点； 
                    XmlNode idtst = idts[0];

                    string area = idtst.SelectSingleNode("short_name").InnerText;
                    tb6.Text = address + "," + area;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double x1 = double.Parse(this.tb7.Text);
            double x2 = double.Parse(this.tb8.Text);
            double x3 = double.Parse(this.tb9.Text);
            double x4 = double.Parse(this.tb10.Text);
            tb11.Text = GetDistance(x1, x2, x3, x4).ToString();
        }
        private const double EARTH_RADIUS = 6378.137;//地球半径
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        private void 路线搜索ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void 定位查找ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        } 
        
    }          
               
       
    
    /// <summary>
    /// a class for latitude and longtitude
    /// </summary>
    [Serializable]
    public class Geo
    {
        /// <summary>
        /// latitude
        /// </summary>
        private string _latitude = "";

        /// <summary>
        /// longtitude
        /// </summary>
        private string _longtitude = "";

        /// <summary>
        /// default constructor
        /// </summary>
        public Geo()
        {
            
        }

        /// <summary>
        /// construct geo given latitude and longtitude
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longtitude"></param>
        public Geo(string latitude, string longtitude)
        {
            _latitude = latitude;
            _longtitude = longtitude;           
        }
        /// <summary>
        /// construct geo given name of a place
        /// </summary>
        /// <param name="location"></param>
        public Geo(string location)
        {
            string output = "csv";
            string url = string.Format("http://maps.google.com/maps/geo?q={0}&output={1}", location, output);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                string[] tmpArray = sr.ReadToEnd().Split(',');
                _latitude = tmpArray[2];
                _longtitude = tmpArray[3];
            }
        }
        /// <summary>
        /// get latitude(纬度)
        /// </summary>
        public string Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        /// <summary>
        /// get longtitude(经度)
        /// </summary>
        public string Longtitude
        {
            get { return _longtitude; }
            set { _longtitude = value; }
        }
    }

}
