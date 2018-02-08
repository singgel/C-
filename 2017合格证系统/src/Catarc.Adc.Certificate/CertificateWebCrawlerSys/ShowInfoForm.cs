using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Helpers;
using CertificateWebCrawlerSys.Utils;
using System.Net;

namespace CertificateWebCrawlerSys
{
    public partial class ShowInfoForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        bool IsLogin = false;
        CookieContainer cookieContainer = new CookieContainer();
        public ShowInfoForm()
        {
            InitializeComponent();
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }
        //tab窗体切换
        private void ShowWindows<F>(bool alone) where F : DevExpress.XtraBars.Ribbon.RibbonForm, new()
        {
            try
            {
                if (!IsLogin)
                {
                    MessageBox.Show("请先登录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (alone)
                {
                    foreach (DevExpress.XtraBars.Ribbon.RibbonForm form in this.MdiChildren)
                    {
                        if (form.GetType().Equals(typeof(F)))
                        {
                            form.Activate();
                            ribbon.SelectedPage = form.Ribbon.Pages[0];
                            return;
                        }
                    }
                }
                F f = new F();
                f.MdiParent = this;
                f.Show();
                ribbon.SelectedPage = f.Ribbon.Pages[0];
            }
            catch
            {
            }
        }
        //ribbon导航窗体内的控件改变
        private void xtraTabbedMdiManager1_SelectedPageChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                {
                    var xmtp = ((DevExpress.XtraTabbedMdi.XtraTabbedMdiManager)sender);
                    var fm = ((DevExpress.XtraBars.Ribbon.RibbonForm)xmtp.SelectedPage.MdiChild);
                    if (fm.Ribbon.Pages.Count > 0)
                    {
                        ribbon.SelectedPage = fm.Ribbon.Pages[0];
                    }
                }
            }
            catch
            {
            }
        }
        //登录
        private void barBtnLogin_ItemClick(object sender, ItemClickEventArgs e)
        {
            //string strLoginUrl = "http://resource.autoidc.cn/pages/login.aspx";
            //string strTargerUrl1 = "http://resource.autoidc.cn/Default.aspx";
            //// 登录后-资源编录列表页面
            //GetHtmlSourceLogin(strLoginUrl, strTargerUrl1);
            cookieContainer = Tool.Login();
            if (cookieContainer==null)
            {
                IsLogin = false;
                MessageBox.Show("登录失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                IsLogin = true;
                MessageBox.Show("登录成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        //机动车合格证申请数据抓取
        private void navBarItemHGZ_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<HGZForm>(true);
        }

        private void navBarItemPZ_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<PZForm>(true);
        }

        private void navBarItemWS_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<WSForm>(true);
        }

        //全部抓取
        private void navBarItemAll_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            IsLogin = true;
            ShowWindows<AllForm>(true);
        }


       

    }
}