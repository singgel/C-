using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;

using WebBrowserUtils;
using ExcelUtils;

using WindowsFormsApplication2.Matcher;
using WindowsFormsApplication2.Actions;

namespace WindowsFormsApplication2
{
    // 汽车柔性申报系统
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private Boolean validateInput()
        {
            if (this.marketFileTextBox.Text.Length < 1)
            {
               MessageBox.Show(this, "市场部数据文件不能为空！");
               return false;
            }
            if (this.ggTextBox.Text.Length < 1)
            {
                //MessageBox.Show(this, "公告数据文件不能为空！");
                //return false;
            }
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 执行操作之前的验证
            //if (!this.validateInput())
            //{
            //    return;
            //}

            //Matcher.Matcher.readMatchRules("", this.marketFileTextBox.Text);

            // 开始填报


            this.mainWebBrowser.Url = new Uri("http://www.vecc-mep.org.cn/newvip/login.jsp");
            while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete) {
                Application.DoEvents();
            }
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                // 调用填参方法
                FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep1();
                f.tryFillGHStep1Submit();
                this.textBox1.Text += "成功登入国环申报系统";
                this.textBox1.Text += "\r\n";
            }

            while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            //第二步直接跳转到填报页面
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                // 直接跳转页面
                this.mainWebBrowser.Navigate("http://www.vecc-mep.org.cn/newvip/newplan/step01.jsp");
                
                this.textBox1.Text += "第一次跳转,成功跳转页面进入第一步填报";
                this.textBox1.Text += "\r\n";
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            //在页面上填报内部编号,选择排放标准和车辆类型
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                // 直接跳转页面
                FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep2();
                //f.tryFillGHStep2Submit();

                this.textBox1.Text += "第二次跳转,成功跳转页面进入第二步填报";
                this.textBox1.Text += "\r\n";
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }

        }

        private void mainWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            IHTMLDocument2 win = (IHTMLDocument2)this.mainWebBrowser.Document.DomDocument;
            

            
            // 流转控制
            if (win.url.Equals(Constants.LOGIN_PAGE))
            {
                LoginAction.tryLogin(this.mainWebBrowser);
            }
            else if (win.url.Equals(Constants.MAIN_MENU))
            {
                this.mainWebBrowser.Navigate(Constants.APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE);
            }
            else if (win.url.Equals(Constants.APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE))
            {
                // 重写js文件中的alert方法。将alert参数导出。
                /*
                win.parentWindow.execScript("var testparam='';function nalert(ss){testparam =ss;}", "javascript");
                win.parentWindow.execScript("window.alert=nalert;window.onerror=null;window.confirm=null;window.open=null;window.showModalDialog=null;", "javascript");
                
                try
                {

                    win.parentWindow.execScript("window.alert=nalert;");
                    win.parentWindow.execScript("alert(11);", "javascript");
                }
                catch
                { }
                
                // 启动timer
                this.timer1.Enabled = true;
                this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
                */

                // 调用填参方法
                FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
                f.tryFillConfiguration();
            }

            /*
             *                 
             * win.parentWindow.execScript("var testparam='';function nalert(ss){testparam =ss;}", "javascript");
                win.parentWindow.execScript("window.alert=nalert;window.onerror=null;window.confirm=null;window.open=null;window.showModalDialog=null;", "javascript");
                win.parentWindow.execScript("alert(11);", "javascript");
                try
                {
                    win.parentWindow.execScript("document.getElementById('iframemenupage').contentWindow.alert=nalert;");
                }
                catch
                { }
                win = null;

                // 调用填参方法
                FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
                f.tryFillConfiguration();
             * */
        }

        private static String targetAttribute = null; 

        // 定时器
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
              
            }
            catch
            { }

            if (mainForm.targetAttribute != null)
            {
                this.Text = mainForm.targetAttribute;
                this.timer1.Enabled = false;
                
            }
        }

        private void excelButton_click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "选择市场部数据Excel文件";
            open.Filter = "Excel文件 (*.xls)|*.xls";
            if (open.ShowDialog() == DialogResult.OK)
            {
                this.marketFileTextBox.Text = open.FileName;
            }
        }

        private void testButton_click(object sender, EventArgs e)
        {
            // 调用填参方法
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            //Matcher.Matcher.readMatchRules();

            String dataFileName = "e:\\example.xls";
            String sheetName = "asdf";
            ExcelInstance data = ExcelUtil.readExcel(dataFileName, sheetName);

            int item = 0;
            int col = 1;
            int row = 2;
            String key = ExcelUtil.reader.getCellValue(row, item, data.dataTables[0]);
            while (true)
            {
                String value = ExcelUtil.reader.getCellValue(row, col, data.dataTables[0]);
                Console.WriteLine(key);
                row++;
                if (row < data.dataTables[0].Rows.Count)
                {
                    key = ExcelUtil.reader.getCellValue(row, item, data.dataTables[0]);
                }
                else
                {
                    break;
                }
            }

        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            // 填数之前先把文件选好
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "选择公告数据Excel文件";
            open.Filter = "Excel文件 (*.xls)|*.xls";
            if (open.ShowDialog() == DialogResult.OK)
            {
                this.ggTextBox.Text = open.FileName;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                IHTMLDocument2 d2 = this.mainWebBrowser.Document.DomDocument as IHTMLDocument2;
                IHTMLWindow2 w2 = (IHTMLWindow2)d2.parentWindow;

                Type windowType = w2.GetType();
                String testText = (String)windowType.InvokeMember("testparam",
                    System.Reflection.BindingFlags.GetProperty, null, w2, new Object[] { });
                mainForm.targetAttribute = testText;

                MessageBox.Show(mainForm.targetAttribute);
            }
            catch
            {
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
