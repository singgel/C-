using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Catarc.Adc.NewEnergyAccountSys.Form_Data;
using Catarc.Adc.NewEnergyAccountSys.Form_Set;
using System.Linq;
using DevExpress.XtraNavBar;
using Catarc.Adc.NewEnergyAccountSys.Common;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using DevExpress.XtraBars.Helpers;
using Catarc.Adc.NewEnergyAccountSys.Form_Index;
using System.IO;


namespace Catarc.Adc.NewEnergyAccountSys
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MainForm()
        {
            InitializeComponent();
            initAuthorityMenu();
            InitSkinGallery();
        }

        //窗体加载成功
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.barStaticQYMC.Caption = String.Format("企业名称：{0}", Settings.Default.Vehicle_MFCS);
            //商业版与免费版区分
            if (Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
            {
                this.barStaticCopyRight.Caption = String.Format("中国汽车技术研究中心-数据资源中心 Copyright © 2017-{0},All Rights Reserved", DateTime.Today.Year);
                this.ribbonPageGroup2.Visible = true;
            }
            else
            {
                this.barStaticCopyRight.Caption = "©版权所有：工业和信息化部装备工业司";
                this.ribbonPageGroup2.Visible = false;
            }
            if (!AccessHelper.Exists(AccessHelper.conn, String.Format("select count(*) from CONTRACT_USER where AutoFill_Manufacturer = '{0}' and LiquYear = '{1}'", Settings.Default.Vehicle_MFCS, Settings.Default.ClearYear)))
            {
                ShowWindows<ContactsForm>(true);
            }
            else
            {
                //商业版与免费版区分
                if (Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
                {
                    ShowWindows<IndexForm>(true);
                }
                else
                {
                    ShowWindows<ImportOldInfoForm>(true);
                }
            }
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

        //主页信息
        private void barBtnSysInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            //商业版与免费版区分
            if (Settings.Default.AuthorityUrl.Equals(@"\Template\Authority.XML"))
            {
                ShowWindows<IndexForm>(true);
            }
            else
            {
                System.Diagnostics.Process.Start(Utils.installPath + Settings.Default.Instructions);
            }
        }

        //主题设置
        private void rgbiSkins_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            Settings.Default.SkinStyle = e.Item.Caption;
            Settings.Default.Save();
        }

        //单条信息录入
        private void navBarSingleInfo_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (AccessHelper.Exists(AccessHelper.conn, String.Format("select count(*) from CONTRACT_USER where AutoFill_Manufacturer = '{0}' and LiquYear = '{1}'", Settings.Default.Vehicle_MFCS, Settings.Default.ClearYear)))
            {
                using (SingleInfoForm singleInfoForm = new SingleInfoForm())
                {
                    singleInfoForm.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("请先填写企业联络人相关信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowWindows<ContactsForm>(true);
            }
        }

        //旧模板数据录入
        private void navBarImportOldInfo_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (AccessHelper.Exists(AccessHelper.conn, String.Format("select count(*) from CONTRACT_USER where AutoFill_Manufacturer = '{0}' and LiquYear = '{1}'", Settings.Default.Vehicle_MFCS, Settings.Default.ClearYear)))
            {
                ShowWindows<ImportOldInfoForm>(true);
            }
            else
            {
                MessageBox.Show("请先填写企业联络人相关信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowWindows<ContactsForm>(true);
            }
        }

        //新模板信息录入
        private void navBarImportNewInfo_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (AccessHelper.Exists(AccessHelper.conn, String.Format("select count(*) from CONTRACT_USER where AutoFill_Manufacturer = '{0}' and LiquYear = '{1}'", Settings.Default.Vehicle_MFCS, Settings.Default.ClearYear)))
            {
                ShowWindows<ImportNewInfoForm>(true);
            }
            else
            {
                MessageBox.Show("请先填写企业联络人相关信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowWindows<ContactsForm>(true);
            }
        }

        //联系人信息录入
        private void navBarContacts_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ContactsForm>(true);
        }

        //公告参数
        private void navBarNoticeParam_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            if (AccessHelper.Exists(AccessHelper.conn, String.Format("select count(*) from CONTRACT_USER where AutoFill_Manufacturer = '{0}' and LiquYear = '{1}'", Settings.Default.Vehicle_MFCS, Settings.Default.ClearYear)))
            {
                ShowWindows<NoticeParam>(true);
            }
            else
            {
                MessageBox.Show("请先填写企业联络人相关信息！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowWindows<ContactsForm>(true);
            }
        }

        //设置
        private void navBarSetForm_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            using (SetForm SetForm = new SetForm())
            {
                SetForm.ShowDialog();
            }
        }

        private void navBarExit_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (MessageBox.Show("您确定要退出系统吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        //初始化权限
        private void initAuthorityMenu()
        {
            const string menuArr = "组类";
            List<string> listModel = Authority.ReadMenusXmlData("AuthorityUrl").Where(c => menuArr.Contains(c.ParentID)).Select(c => c.MenuName).ToList<string>();
            foreach (NavBarGroup g in navBarControl1.Groups)
            {
                g.Visible = false;
                foreach (NavBarItemLink i in g.ItemLinks)
                {
                    if (listModel.Contains(i.Caption))
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

        //初始化主题设置
        void InitSkinGallery()
        {
            SkinHelper.InitSkinGallery(rgbiSkins, true);
        }
    }
}