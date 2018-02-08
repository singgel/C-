using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Linq;
using DevExpress.XtraNavBar;
using Catarc.Adc.NewEnergyApproveSys.Common;
using Catarc.Adc.NewEnergyApproveSys.Properties;
using DevExpress.XtraBars.Helpers;
using System.IO;
using Catarc.Adc.NewEnergyApproveSys.Form_SysManage;
using Catarc.Adc.NewEnergyApproveSys.Form_WorkManage;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using DevExpress.XtraBars.Ribbon;


namespace Catarc.Adc.NewEnergyApproveSys
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static List<MenuModel> listMenu = new List<MenuModel>();

        public MainForm()
        {
            InitializeComponent();
            initAuthorityMenu();
            InitSkinGallery();
        }

        //窗体加载成功
        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowWindows<Form_Index.IndexForm>(true);
            this.barStaticQYMC.Caption = String.Format("当前账户：{0}", Settings.Default.LocalUserName);
            this.barStaticCopyRight.Caption = String.Format("工业和信息化部装备工业司 Copyright © 2017-{0},All Rights Reserved", DateTime.Today.Year);
        }

        //初始化权限
        private void initAuthorityMenu()
        {
            listMenu = GridControlHelper.GetMenusData();
            string authority = Settings.Default.Authority;
            string[] arr = authority.Split(',');
            List<string> AuthorityName = listMenu.Where(c => arr.Contains(c.ID)).Select(c => c.MenuName).ToList<string>();


            foreach (NavBarGroup g in navBarControl1.Groups)
            {
                g.Visible = false;
                foreach (NavBarItemLink i in g.ItemLinks)
                {
                    if (AuthorityName.Contains(i.Caption))
                    {
                        i.Visible = true;
                        g.Visible = true;
                    }
                    else
                    {
                        i.Visible = false;
                    }
                }
            }
        }

        //初始化RIBBONFORM中的Control权限
        static void initAuthorityRibbonControl(DevExpress.XtraBars.Ribbon.RibbonForm form)
        {
            if (form.Name.Equals("IndexForm"))
                return;
            string formID = string.Empty;
            foreach (MenuModel mm in listMenu)
            {
                if (mm.MenuName == form.Text)
                {
                    formID = mm.ID;
                    break;
                }
            }

            string authority = Settings.Default.Authority;
            string[] arr = authority.Split(',');
            List<string> AuthorityName = listMenu.Where(c => formID == c.ParentID && arr.Contains(c.ID)).Select(c => c.MenuName).ToList<string>();

            foreach (RibbonPageGroup group in form.Ribbon.Pages[0].Groups)
            {
                group.Visible = false;
                foreach (BarButtonItemLink link in group.ItemLinks)
                {
                    BarButtonItem bbi = link.Item;
                    if (AuthorityName.Contains(link.Caption))
                    {
                        bbi.Visibility = BarItemVisibility.Always;
                        bbi.Enabled = true;

                        group.Visible = true;
                    }
                    else
                    {
                        bbi.Visibility = BarItemVisibility.Never;
                        bbi.Enabled = false;
                    }
                }
            }
        }

        //初始化主题设置
        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }

        //tab窗体切换
        private void ShowWindows<F>(bool alone) where F : DevExpress.XtraBars.Ribbon.RibbonForm, new()
        {
            try
            {
                if (alone)
                {
                    foreach (DevExpress.XtraBars.Ribbon.RibbonForm form in this.MdiChildren)
                    {
                        if (form.GetType().Equals(typeof(F)))
                        {
                            form.Activate();
                            if (form.Ribbon != null)
                                ribbon.SelectedPage = form.Ribbon.Pages[0];
                            initAuthorityRibbonControl(form);
                            return;

                        }
                    }
                }
                F f = new F();
                f.MdiParent = this;
                if (f.Ribbon != null)
                    ribbon.SelectedPage = f.Ribbon.Pages[0];
                initAuthorityRibbonControl((RibbonForm)f);
                f.Show();
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

        //首页
        private void barBtnSysInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<Form_Index.IndexForm>(true);
        }

        //主体变更
        private void rgbiSkins_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            Settings.Default.SkinStyle = e.Item.Caption;
            Settings.Default.Save();
        }

        //角色管理
        private void navBarRoleManager_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<RoleForm>(true);
        }

        //用户管理
        private void navBarUserManager_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<UserForm>(true);
        }

        //数据字典
        private void navBarDataDic_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<DataDictionaryForm>(true);
        }

        //公告参数管理
        private void navBarNoticeParam_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<NoticeForm>(true);
        }

        //补贴标准管理
        private void navBarSubsidyManager_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SubsidyForm>(true);
        }

        //数据导入
        private void navBarImport_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ImportDataForm>(true);
        }

        //核查情况
        private void navBarInspect_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<InspectInfoForm>(true);
        }

        //审查一次
        private void navBarCheck1_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ApproveFirstForm>(true);
        }

        //审查二次
        private void navBarCheck2_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ApproveSecondForm>(true);
        }

        //审查三次
        private void navBarCheck3_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ApproveThirdForm>(true);
        }

        //清算信息汇总
        private void navBarClearSum_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AccountInfoForm>(true);
        }

        //生产企业联络人
        private void navBarContacts_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ContactInfoForm>(true);
        }

        //修改个人密码
        private void btnUpdatePWD_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (UpdateUserPWD uif = new UpdateUserPWD(Settings.Default.LocalUserID) { StartPosition = FormStartPosition.CenterScreen, Text = "修改密码" })
            {
                uif.ShowDialog();
            }
        }

    }
}