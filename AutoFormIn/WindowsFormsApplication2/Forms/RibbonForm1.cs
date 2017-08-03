using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraTab;

namespace Assistant.Forms
{
    public partial class RibbonForm1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public RibbonForm1()
        {
            InitializeComponent();
            this.xtraTabControl1.CloseButtonClick += new EventHandler(xtraTabControl1_CloseButtonClick);
        }

        void xtraTabControl1_CloseButtonClick(object sender, EventArgs e)
        {
            DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs EArg = (DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs)e;
            string name = EArg.Page.Text;
            foreach (XtraTabPage page in this.xtraTabControl1.TabPages)//遍历得到和关闭的选项卡一样的Text
            {
                if (page.Text == name)
                {
                    this.xtraTabControl1.TabPages.Remove(page);
                    page.Dispose();
                    return;
                }
            }

        }

        private void xtraTabControl1_Click(object sender, EventArgs e)
        {
           
        }

        private void testButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            //XtraTabPage xpage = new XtraTabPage();
            TabPageWithWebBrowser page = new TabPageWithWebBrowser();
            page.Name = "test";
            page.Text = "test";
            page.WebBrowser1.Url = new Uri("http://www.baidu.com/s?wd=devexpress+tab&ie=UTF-8");
            this.xtraTabControl1.TabPages.Add(page);
            xtraTabControl1.SelectedTabPage = page;//显示该页
        }
    }
}