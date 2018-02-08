using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using FuelDataSysClient.SubForm;
using System.Threading;
using FuelDataSysClient.FuelCafc;
using FuelDataSysClient.Change;
using FuelDataSysClient.CertificateService;
using Common;
using System.IO;
using DevExpress.XtraNavBar;
using FuelDataSysClient.Form_SJHS;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Form_BGSC;
using System.Linq;

namespace FuelDataSysClient
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public string strUserId;

        LocalLoginForm locLoginForm = new LocalLoginForm();
        public MainForm()
        {
            InitializeComponent();
            if (locLoginForm.ShowDialog() != DialogResult.OK)
            { this.Dispose(); }
            else
            {
                InitAuthority();

                //加载主页
                ShowWindows<IndexForm>(true);

                this.barUser.Caption = string.Format("当前用户：{0} 当前线路：{1}", Utils.localUserId, FuelDataSysClient.Properties.Settings.Default.IsFuelTest ? "正式线路" : "测试线路");
                if (Utils.localUserId != "admin")
                {
                    barUserManager.Enabled = false;
                }

                //AutoUpdater.AutoUpdater au = new AutoUpdater.AutoUpdater();
                //au.ShowDialog();
                // 检测到新版本的燃料参数是自动同步
                //if (Utils.CompareParamVersion())
                //{
                //    MessageBox.Show("系统检测到燃料参数新版本，请点击“工具”->“燃料参数同步”->“同步到本地”进行更新（未及时更新将导致上报参数错误）");
                //}

                //try
                //{
                //    new Thread(new ThreadStart(() =>
                //    {
                //        // 更新节假日信息
                //        if (Utils.CompareHoliday())
                //        {
                //            Utils.AutoSyncHolidays();
                //        }
                //    })).Start();
                //}
                //catch (Exception)
                //{
                //}
            }
        }

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

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (cljbxxForm cljbxxForm = new cljbxxForm())
            {
                cljbxxForm.ShowDialog();
            }
        }

        private void barDataImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<ImportForm>(true);
        }

        // 查询服务器
        private void barCxServer_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SearchServerForm>(true);
        }

        // 查询本地
        private void barCxLocal_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SearchLocalForm>(true);
        }

        private void barPreUpload_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SearchLocalForm>(true);
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalForm>(true);
        }

        private void barSetup_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (SetForm SetForm = new SetForm())
            {
                SetForm.ShowDialog();
            }
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalUpdateForm>(true);
        }

        // 本地已上报
        private void barUploaded_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SearchLocalUploadedForm>(true);
        }

        // 本地修改未上传
        private void barUpdateApp_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SearchLocalUpdateForm>(true);
        }

        private void barSynParam_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SyncParamForm>(true);
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            using (cljbxxForm cljbxxForm = new cljbxxForm())
            {
                cljbxxForm.ShowDialog();
            }
        }

        private void navBarItem13_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ImportForm>(true);
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalUploadedForm>(true);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ToolStripMenuItem_Show_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchServerForm>(true);
        }

        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            using (SetForm setForm = new SetForm())
            {
                setForm.ShowDialog();
            }
        }

        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SyncParamForm>(true);
        }

        private void barDelSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<ApplyDelViewForm>(true);
        }

        private void barEditSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<ApplyEditViewForm>(true);
        }

        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyEditViewForm>(true);
        }

        private void navBarItem11_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyDelViewForm>(true);
        }

        private void btnOvertimeSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<ApplyUpOTForm>(true);
        }

        private void navBarItem12_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyUpOTForm>(true);
        }

        private void barPreUploadOT_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SearchLocalOTForm>(true);
        }

        private void navBarItem14_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalOTForm>(true);
        }

        private void barSyncHoliday_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SyncHolidayForm>(true);
        }

        private void btnSyncHolday_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SyncHolidayForm>(true);
        }

        private void barUserManager_ItemClick(object sender, ItemClickEventArgs e)
        {
            UserManager um = new UserManager();
            um.Show();
        }

        private void barAutoUpdate_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + "\\AutoUpdater.exe"); 
        }

        private void navBarItem16_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<AvgFuelDetailForm>(true);
        }

        private void navBarItem17_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<CAFCForm>(true);
        }

        private void navBarItem18_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<PreReportForm>(true);
        }

        private void navBarItem19_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ContrastForm>(true);

        }
        private void navBarItem20_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<Word2ViewerForm>(true);
        }

        private void navBarItem21_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ChangeRibbonForm>(true);
        }

        private void navBarItem22_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<CertificateComparisonForm>(true);
        }

        private void navBarItem23_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<YhlsbhForm>(true);
        }

        private void navBarItem0503_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AnnouncementForm>(true);
        }

        // 油耗达标排名
        private void navBarItem0403_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelRankingForm>(true);
        }

        // 首页
        private void barButtonItem1_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            ShowWindows<IndexForm>(true);
        }

        private void navBarItem0603_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<WordViewerForm>(true);
        }
        //权限管理
        private void navBarItem0704_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.SubForm.AuthorityForm>(true);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (1 <= DateTime.Now.Day && DateTime.Now.Day  <= 10)
            {
                if (FuelDataSysClient.Properties.Settings.Default.CompareFlag)
                {
                    //DialogResult result = MessageBox.Show(
                    //"是否进行油耗数据和合格证数据比对？",
                    //"系统提示",
                    //MessageBoxButtons.OKCancel,
                    //MessageBoxIcon.Question,
                    //MessageBoxDefaultButton.Button2);
                    //if (result == DialogResult.OK)
                    //{
                    //    ShowWindows<CertificateComparisonForm>(true);
                    //    FuelDataSysClient.Properties.Settings.Default.CompareFlag = false;
                    //    FuelDataSysClient.Properties.Settings.Default.Save();
                    //}
                }
            }
            else
            {
                FuelDataSysClient.Properties.Settings.Default.CompareFlag = true;
                FuelDataSysClient.Properties.Settings.Default.Save();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 定义用户对话框返回结果变量
            DialogResult result;
            // 根据关闭窗体的原因确定主界面响应方式
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                case CloseReason.TaskManagerClosing:
                case CloseReason.UserClosing:
                case CloseReason.WindowsShutDown:
                case CloseReason.None:
                    result = MessageBox.Show(
                    string.Format("确实要退出 [{0}] 油耗系统吗？", Utils.qymc),
                    "退出提示",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                    // 如果用户点击[取消]，则取消窗体关闭事件
                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    else if (result == DialogResult.OK)
                    {
                        this.Dispose(true);
                        this.Close();
                        Application.Exit();
                    }
                    break;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = true;
            }
            else
            {
                this.notifyIcon1.Visible = true;
                this.ShowInTaskbar = true;
            }
        }

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

        // 获取菜单权限
        private void InitAuthority()
        {
            AuthorityManager.Authority am = Utils.serverAuthority;
            DataSet ds = am.QueryAuthorityByUserName(Utils.userId, Utils.password);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                Dictionary<string, string> aDir = new Dictionary<string, string>();
                foreach (DataTable dt in ds.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        aDir.Add(dr[0].ToString(), "true");
                    }
                }
                aDir.Add("navBarItem0705", "true");
                //设置权限管理
                if (aDir.ContainsKey("navBarItem0704"))
                {
                    string sql = String.Format("SELECT AUTHORITY FROM SYS_USERINFO WHERE USERNAME='{0}'", Utils.localUserId);
                    var result = AccessHelper.ExecuteScalar(AccessHelper.conn, sql, null);
                    if (result != null)
                    {
                        string[] resultArr = result.ToString().Split(',');
                        AuthorityForm af = new AuthorityForm();
                        List<string> listModel = af.ReadMenusXmlData().Where(c => resultArr.Contains(c.ID.ToString())).Select(c => c.ID).ToList<string>();
                        listModel.Add("navBarItem0705");
                        foreach (NavBarGroup g in navBarControl1.Groups)
                        {
                            foreach (NavBarItemLink i in g.ItemLinks)
                            {
                                if (listModel.Contains(i.ItemName))
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
                }
                else
                {
                    foreach (NavBarGroup g in navBarControl1.Groups)
                    {
                        foreach (NavBarItemLink i in g.ItemLinks)
                        {
                            if (aDir.ContainsKey(i.ItemName))
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
            }
            //foreach (NavBarGroup g in navBarControl1.Groups)
            //{
            //    foreach (NavBarItemLink i in g.ItemLinks)
            //    {
            //        i.Visible = true;
            //        g.Visible = true;
            //    }
            //}
        }
        //自动更新
        private void navBarItem0705_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            if (new UpdateForm().ShowDialog() == DialogResult.OK)
            {
                this.Close();
            }
        }

        
    }
}