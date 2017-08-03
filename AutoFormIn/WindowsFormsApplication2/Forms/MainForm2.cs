using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using mshtml;
using WebBrowserUtils;
using ExcelUtils;
using System.Runtime.InteropServices;
using Assistant.Matcher;
using Assistant.Actions;

namespace Assistant
{
    [ComVisible(true)]
    public partial class MainForm2 : System.Windows.Forms.Form
    {
        public MainForm2()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            this.mainWebBrowser.Url = new Uri(DeclarationSite.ADCSite);
            this.textBox3.Click += new EventHandler(textBox_fileWindow_Click);
            this.textBox4.Click += new EventHandler(textBox_fileWindow_Click);
            this.textBox7.Click += new EventHandler(textBox_fileWindow_Click);
            this.mainWebBrowser.NewWindow +=new CancelEventHandler(mainWebBrowser_NewWindow);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
            this.panel2.Location = this.panel1.Location;
            this.panel2.Visible = true;
            Constants.declarationType = DeclarationType.declarationPZ;
            this.mainWebBrowser.Navigate(DeclarationSite.PZSite);
            this.mainWebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(mainWebBrowser_DocumentCompleted);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.panel2.Visible = false;
            this.panel1.Visible = true;
            this.mainWebBrowser.Url = new Uri(DeclarationSite.ADCSite);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
            this.panel3.Location = this.panel1.Location;
            this.panel3.Visible = true;
            Constants.declarationType = DeclarationType.declarationGH;
            this.mainWebBrowser.Url = new Uri(DeclarationSite.GHSite);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.panel3.Visible = false;
            this.panel1.Visible = true;
            this.mainWebBrowser.Url = new Uri(DeclarationSite.ADCSite);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
            this.panel3.Location = this.panel1.Location;
            this.panel3.Visible = true;
            Constants.declarationType = DeclarationType.declarationBH;
            this.mainWebBrowser.Url = new Uri(DeclarationSite.BHSite);
        }

        private void Form1_FormClosing(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.panel1.Visible = false;
            this.panel4.Location = this.panel1.Location;
            this.panel4.Visible = true;
            Constants.declarationType = DeclarationType.declaration3C;
            this.mainWebBrowser.Url = new Uri("http://tec.cqccms.com.cn/");


        }

        private void button11_Click(object sender, EventArgs e)
        {
            //国环填报动作
            if (Constants.declarationType == DeclarationType.declarationGH)
            {
                GHFillConfigurationAction f = new GHFillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep1();
                f.tryFillGHStep1Submit();
                this.wait(1);
                this.mainWebBrowser.Navigate("http://www.vecc-mep.org.cn/newvip/newplan/step01.jsp");
                this.wait(1);

                {
                    GHFillConfigurationAction f1 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f1.tryFillGHStep2();
                    f1.tryFillGHStep2Submit();
                }

                this.wait(1);

                {
                    GHFillConfigurationAction f2 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f2.tryFillGHStep3();
                    f2.tryFillGHStep3Submit();
                }
                //适当延长等待时间
                this.wait(2);

                {
                    GHFillConfigurationAction f3 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f3.tryFillGHStep4Submit();
                }

                this.wait(1);

                {
                    GHFillConfigurationAction f4 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f4.tryFillGHStep4Submit();
                }

                this.wait(1);

                {
                    GHFillConfigurationAction f5 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f5.tryFillGHStep3AddAppendixInfo();
                }

                this.wait(2);

                {
                    GHFillConfigurationAction f6 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f6.testGH();
                }


 
            }
            //北环填报动作
            else if (Constants.declarationType == DeclarationType.declarationBH)
            {
                BHFillConfigurationAction f = new BHFillConfigurationAction(this.mainWebBrowser);

                f.step0Login();
                f.step0Submit();
                this.wait(1);

                this.mainWebBrowser.Navigate("http://58.30.229.122:8080/motor/car/car-declare-step1!input.action", "Framewindow");

                this.wait(1);

                {
                    BHFillConfigurationAction f1 = new BHFillConfigurationAction(this.mainWebBrowser);
                    f1.step1Fill();
                    f1.step1Submit();
                }

                this.wait(1);

                {
                    BHFillConfigurationAction f2 = new BHFillConfigurationAction(this.mainWebBrowser);
                    f2.step2Submit();
                }

                this.wait(1);

                {
                    BHFillConfigurationAction f3 = new BHFillConfigurationAction(this.mainWebBrowser);
                    f3.step3Fill();
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }


        private String getNowMessage(String message)
        {
            return DateTime.Now.ToString() + ":     " + message + "\r\n";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.mainWebBrowser.Url = new Uri("http://www.vecc-mep.org.cn/newvip/newplan/step02.jsp?zcj_city=%C7%E1%D0%CD%C6%FB%D3%CD%B3%B5");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ////this.textBox7.AppendText(getNowMessage(this.mainWebBrowser.DocumentText));
            HtmlWindow hw = this.mainWebBrowser.Document.Window.Frames["Framewindow"];
            hw.Document.InvokeScript("formSubmit");
        }




        private void timer1_tick_GHStep1Submit(object sender, EventArgs e)
        {
            //this.textBox7.AppendText("定时器结束");
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {

                // 直接跳转页面
                this.mainWebBrowser.Navigate("http://www.vecc-mep.org.cn/newvip/newplan/step01.jsp");
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_tick_GHStep1Submit;
            this.timer1.Stop();
        }

        private void timer1_Tick_ForStep2QYBJ(object sender, EventArgs e)
        {
            //this.textBox7.AppendText("定时器结束");
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {

                // 直接跳转页面
                GHFillConfigurationAction f = new GHFillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep3();
                f.tryFillGHStep3Submit();

                ////this.textBox7.AppendText(getNowMessage("第三次跳转,成功跳转页面进入第三步填报"));
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_ForStep2QYBJ;
            this.timer1.Stop();
        }

        private void timer1_Tick_ForStep3CreateAppendix(object sender, EventArgs e)
        {
            //this.textBox7.AppendText(getNowMessage("定时器结束"));
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {

                // 直接跳转页面
                GHFillConfigurationAction f = new GHFillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep4Submit();

                //this.textBox7.AppendText(getNowMessage("第四次跳转"));
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_ForStep3CreateAppendix;
            this.timer1.Stop();
        }

        //添加详细附录信息
        private void timer1_Tick_ForStep3AddAppendixInfo(object sender, EventArgs e)
        {
            //this.textBox7.AppendText(getNowMessage("定时器结束"));
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                GHFillConfigurationAction f = new GHFillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep3AddAppendixInfo();
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_ForStep3CreateAppendix;
            this.timer1.Stop();
        }

        //北环点击左侧新建按钮
        private void timer1_Tick_BHForStep1ClickButton(object sender, EventArgs e)
        {

            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                BHFillConfigurationAction f = new BHFillConfigurationAction(this.mainWebBrowser);
                this.mainWebBrowser.Navigate("http://58.30.229.122:8080/motor/car/car-declare-step1!input.action", "Framewindow");
                this.wait(1);
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_BHForStep1ClickButton;
            this.timer1.Stop();

        }

        //北环选择车辆型号
        private void timer1_Tick_BHForStep1ChooseType(object sender, EventArgs e)
        {
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                BHFillConfigurationAction f = new BHFillConfigurationAction(this.mainWebBrowser);
                f.step1Fill();
                f.step1Submit();
                this.wait(1);
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_BHForStep1ChooseType;
            this.timer1.Stop();
        }
        //北环填写申请直接提交
        private void timer1_Tick_BHForStep2Submit(object sender, EventArgs e)
        {
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                BHFillConfigurationAction f = new BHFillConfigurationAction(this.mainWebBrowser);
                f.step2Submit();
                this.wait(1);
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_BHForStep2Submit;
            this.timer1.Stop();
        }

        //北环填写申请直接提交
        private void timer1_Tick_BHForStep3Fill(object sender, EventArgs e)
        {
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                BHFillConfigurationAction f = new BHFillConfigurationAction(this.mainWebBrowser);
                f.step3Fill();
                this.wait(1);
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_BHForStep3Fill;
            this.timer1.Stop();
        }

        //3C点击总则按钮
        private void timer1_Tick_3CForStep2PressButton(object sender, EventArgs e)
        {
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                CQCFillConfigurationAction f = new CQCFillConfigurationAction(this.mainWebBrowser);
                f.tryFill3CStep2PressButton();
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_3CForStep2PressButton;
            this.timer1.Stop();
        }

        //3C点击总则按钮
        private void timer1_Tick_3CForStep2Fill(object sender, EventArgs e)
        {
            if (this.mainWebBrowser.ReadyState == WebBrowserReadyState.Complete)
            {
                CQCFillConfigurationAction f = new CQCFillConfigurationAction(this.mainWebBrowser);
                f.tryFill3CStep2Fill();
                while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Application.DoEvents();
                }
            }
            this.timer1.Tick -= this.timer1_Tick_3CForStep2Fill;
            this.timer1.Stop();
        }

        //3C填报确认按钮
        private void button14_Click(object sender, EventArgs e)
        {
            CQCFillConfigurationAction f = new CQCFillConfigurationAction(this.mainWebBrowser);
            //f.tryFill3CStep1PressButton();
            //f.tryFill3CStep2Fill();
            //foreach (HtmlWindow hw in this.mainWebBrowser.Document.Window.Frames) {
            //    this.textBox8.AppendText(getNowMessage(hw.Name));
            //}
            //HtmlWindow hw = this.mainWebBrowser.Document.Window.Frames["mainFrame"];
            //foreach (HtmlElement he in hw.Document.All) {
            //    this.textBox8.AppendText(getNowMessage(he.Id + "   " + he.Name + "   " + he.GetAttribute("value")));
            //}
            f.tryFill3CStep1Fill();
            f.tryFill3CStep1Submit();
            //点击总则按钮
            this.timer1.Interval = Constants.TIMER_INTERVAL * 1000;
            this.timer1.Tick += new EventHandler(timer1_Tick_3CForStep2PressButton);
            this.timer1.Start();
            while (this.timer1.Enabled)
            {
                Application.DoEvents();
            }
            //填报信息
            this.timer1.Tick += new EventHandler(timer1_Tick_3CForStep2Fill);
            this.timer1.Start();
            while (this.timer1.Enabled)
            {
                Application.DoEvents();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.panel4.Visible = false;
            this.panel1.Visible = true;
            this.mainWebBrowser.Url = new Uri(DeclarationSite.ADCSite);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Matcher.Matcher.readAllCarParams("", @"D:\柔性参数填报系统\上海通用\data.xls");
            ChooseSample cs = new ChooseSample();
            cs.ShowDialog();
            LoginAction.tryLogin(this.mainWebBrowser);
        }

        public void alertMessage(String str) {
            MessageBox.Show(str);
        }

        private void mainWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            IHTMLDocument2 win = (IHTMLDocument2)this.mainWebBrowser.Document.DomDocument;
            // 流转控制
            string s = @"function confirm() {";
            s += @"return true;";
            s += @"}";
            s += @"function alert(str)";
            s += @"{";
            s += @"window.external.alertMessage(str);"; 
            s += @"}";
            win.parentWindow.execScript(s, "javascript");
            this.mainWebBrowser.ObjectForScripting = this;

            if (win.url.Equals(Constants.MAIN_MENU))
            {
                this.mainWebBrowser.Navigate(Constants.APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE);
            }
            else if (win.url.Equals(Constants.APPLY_TEMPORARY_SEQUENCE_NUMBER_PAGE))
            {
                // 调用填参方法
                FillConfigurationAction f = new FillConfigurationAction(this.mainWebBrowser);
                f.tryFillConfiguration();
            }
        }

        private void mainWebBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            //MessageBox.Show("新窗口");
      
            //string url = this.mainWebBrowser.StatusText;
            String url = ((WebBrowser)sender).StatusText;
            //MessageBox.Show(url);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        void textBox_fileWindow_Click(object sender, EventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog();
            open.Title = "选择市场部数据Excel文件";
            open.Filter = "Excel文件 (*.xls)|*.xls";
            if (open.ShowDialog() == DialogResult.OK)
            {
                ((TextBox)sender).Text = open.FileName;
            }
        }

        private void textBox7_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            //Setting setting = new Setting();
            //setting.Location = new Point(500, 500);
            //setting.ShowDialog();
        }

        //使用定时器等待一定时间
        public void wait(int interval)
        {

            this.timer2.Interval = interval * 1000;
            this.timer2.Tick += new EventHandler(timer2_Tick);
            this.timer2.Start();
            while (this.timer2.Enabled)
            {
                Application.DoEvents();
            }

            while (this.mainWebBrowser.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
        }
        //定时器2时间到之后执行函数
        private void timer2_Tick(object sender, EventArgs e)
        {

            this.timer2.Tick -= this.timer2_Tick;
            this.timer2.Stop();
        }

        //删除已有方案的按钮
        private void button17_Click(object sender, EventArgs e)
        {
            //国环填报动作
            if (Constants.declarationType == DeclarationType.declarationGH)
            {
                GHFillConfigurationAction f = new GHFillConfigurationAction(this.mainWebBrowser);
                f.tryFillGHStep1();
                f.tryFillGHStep1Submit();
                this.wait(1);

                this.mainWebBrowser.Navigate("http://www.vecc-mep.org.cn/newvip/newplan/planSearch.jsp?action=search");

                this.wait(1);

                {
                    GHFillConfigurationAction f1 = new GHFillConfigurationAction(this.mainWebBrowser);
                    f1.overrideJS();
                    for (int i = 0; i < 10; i++) {
                        this.wait(10);
                        GHFillConfigurationAction f2 = new GHFillConfigurationAction(this.mainWebBrowser);
                        f2.overrideJS();
                        f2.deleteFirstRadio();
                    }
                }

            }
            //北环填报动作
            else if (Constants.declarationType == DeclarationType.declarationBH)
            {
                BHFillConfigurationAction f = new BHFillConfigurationAction(this.mainWebBrowser);

                f.step0Login();
                f.step0Submit();
                this.wait(1);

                this.mainWebBrowser.Navigate("http://58.30.229.122:8080/motor/car/car-declare-step1!input.action", "Framewindow");

                this.wait(1);



                {
                    BHFillConfigurationAction f1 = new BHFillConfigurationAction(this.mainWebBrowser);
                    f1.step1Fill();
                    f1.step1Submit();
                }

                this.wait(1);

                {
                    BHFillConfigurationAction f2 = new BHFillConfigurationAction(this.mainWebBrowser);
                    f2.step2Submit();
                }

                this.wait(1);

                {
                    BHFillConfigurationAction f3 = new BHFillConfigurationAction(this.mainWebBrowser);
                    f3.step3Fill();
                }


            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            GHFillConfigurationAction f1 = new GHFillConfigurationAction(this.mainWebBrowser);
            f1.overrideJS();
            for (int i = 0; i < 15; i++)
            {
                this.wait(10);
                GHFillConfigurationAction f2 = new GHFillConfigurationAction(this.mainWebBrowser);
                f2.overrideJS();
                f2.deleteFirstRadio();
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            FillConfigurationAction f2 = new FillConfigurationAction(this.mainWebBrowser);
            f2.selectItemAndPageChange();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "选择市场部数据Excel文件";
            open.Filter = "Excel文件 (*.xls)|*.xls";
            if (open.ShowDialog() == DialogResult.OK)
            {
                //((TextBox)sender).Text = open.FileName;
                this.textBox3.Text = open.FileName;
            }
        }
    }
}