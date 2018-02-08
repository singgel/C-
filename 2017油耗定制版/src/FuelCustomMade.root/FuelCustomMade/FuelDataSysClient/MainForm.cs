using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraBars;
using System.Threading;
using FuelDataSysClient.Form_Modify;
using FuelDataSysClient.Form_Compare;
using DevExpress.XtraNavBar;
using FuelDataSysClient.Form_Account;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Form_Report;
using FuelDataSysClient.Form_Configure;
using FuelDataSysClient.Form_DBManager;
using FuelDataSysClient.Form_Statistics;

namespace FuelDataSysClient
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public MainForm()
        {
            InitializeComponent();
            InitAuthority();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowWindows<IndexForm>(true);
            this.barUser.Caption = string.Format("当前用户：{0} 当前线路：{1}", Utils.localUserId, FuelDataSysClient.Properties.Settings.Default.IsFuelTest ? "正式线路" : "测试线路");

            if (1 <= DateTime.Now.Day && DateTime.Now.Day <= 10)
            {
                if (FuelDataSysClient.Properties.Settings.Default.CompareFlag)
                {
                    DialogResult result = MessageBox.Show(
                    "是否进行油耗数据和合格证数据比对？",
                    "系统提示",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.OK)
                    {
                        ShowWindows<CertificateComparisonForm>(true);
                        FuelDataSysClient.Properties.Settings.Default.CompareFlag = false;
                        FuelDataSysClient.Properties.Settings.Default.Save();
                    }
                }
            }
            else
            {
                FuelDataSysClient.Properties.Settings.Default.CompareFlag = true;
                FuelDataSysClient.Properties.Settings.Default.Save();
            }
            try
            {
                new Thread(() =>
                           {
                               if (Utils.CompareHoliday())
                               {
                                   Utils.AutoSyncHolidays();
                               }
                           }).Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.ApplicationExitCall:
                case CloseReason.TaskManagerClosing:
                case CloseReason.UserClosing:
                case CloseReason.WindowsShutDown:
                case CloseReason.None:
                    var result = MessageBox.Show(string.Format("确实要退出 [{0}] 油耗系统吗？", Utils.qymc), "退出提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
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

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<IndexForm>(true);
        }

        private void barSetup_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (var setForm = new SetForm())
            {
                setForm.ShowDialog();
            }
        }

        private void barSynParam_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SyncParamForm>(true);
        }

        private void barSyncHoliday_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SyncHolidayForm>(true);
        }

        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            using (var cljbxxForm = new cljbxxForm())
            {
                cljbxxForm.ShowDialog();
            }
        }

        private void navBarItem1_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalForm>(true);
        }

        private void navBarItem13_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ImportForm>(true);
        }

        private void navBarItem14_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalOTForm>(true);
        }

        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalUploadedForm>(true);
        }

        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalUpdateForm>(true);
        }

        private void navBarItem6_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SearchServerForm>(true);
        }

        private void navBarItem15_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataUploadTotalForm>(true);
        }

        private void navBarItem16_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<AvgFuelDetailForm>(true);
        }

        private void navBarItem17_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<CAFCForm>(true);
        }

        private void navBarItem23_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<FuelHistoryForm>(true);
        }

        private void navBarItem0403_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelRankingForm>(true);
        }

        private void navBarItem19_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ContrastForm>(true);
        }

        private void navBarItem22_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<CertificateComparisonForm>(true);
        }

        private void navBarItem0503_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AnnouncementForm>(true);
        }

        private void navBarItem21_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ChangeRibbonForm>(true);
        }

        private void navBarItem10_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyEditViewForm>(true);
        }

        private void navBarItem11_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyDelViewForm>(true);
        }

        private void navBarItem12_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyUpOTForm>(true);
        }

        private void navBarItem0603_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<MiddleYearForm>(true);
        }

        private void navBarItem20_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<WholeYearForm>(true);
        }

        private void navBarItem18_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<PreReportForm>(true);
        }

        private void navBarItem0705_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            using (var updaterForm = new UpdaterForm())
            {
                updaterForm.ShowDialog();
            }
        }

        private void navBarItem0704_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AuthorityForm>(true);
        }

        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            using (var setForm = new SetForm())
            {
                setForm.ShowDialog();
            }
        }

        private void navBarItem9_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowWindows<SyncParamForm>(true);
        }

        private void navBarItem0703_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SyncHolidayForm>(true);
        }

        // 丰田中国-导入数据
        private void navBarItem0112_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Toyota.ImportForm>(true);
        }

        // 丰田中国-一键数据比对
        private void navBarItem0505_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_Compare.Form_Toyota.ComparisonForm>(true);
        }

        // 丰田中国-本地数据比对
        private void navBarItem0506_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_Compare.Form_Toyota.ContrastForm>(true);
        }

        // 斯巴鲁-导入数据
        private void navBarItem0107_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Subaru.ImportForm>(true);
        }

        // 捷豹路虎-导入数据
        private void navBarItem0108_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Jaguar.ImportForm>(true);
        }

        // 捷豹路虎-国环数据
        private void navBarItem0801_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_National.GHDataForm>(true);
        }

        // 宝马中国-导入数据
        private void navBarItem0109_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_BMW.ImportForm>(true);
        }

        // 保时捷-导入数据
        private void navBarItem0110_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Porsche.ImportForm>(true);
        }

        // 保时捷-油耗数据上报统计
        private void navBarItem0303_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataUploadTotalForm>(true);
        }

        // 三菱中国-导入数据
        private void navBarItem0111_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Mits.ImportForm>(true);
        }

        // 克莱斯勒-导入数据
        private void navBarItem0113_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Chrysler.ImportForm>(true);
        }

        //日产中国-导入数据
        private void navBarItem0115_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Nissan.ImportForm>(true);
        }

        // 大众中国-导入数据
        private void navBarItem0116_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_DBManager.Form_Volkswagen.ImportForm>(true);
        }

        // 大众中国-本地数据比对
        private void navBarItem0507_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_Compare.Form_Volkswagen.ContrastForm>(true);
        }

        // 油耗核算标准-大众中国
        private void navBarItem0404_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelDataSysClient.Form_Account.Form_Volkswagen.CAFCInfoForm>(true);
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
                if (aDir.ContainsKey("navBarItem0704"))
                {
                    string sql = String.Format("SELECT AUTHORITY FROM SYS_USERINFO WHERE USERNAME='{0}'", Utils.localUserId);
                    var result = AccessHelper.ExecuteScalar(AccessHelper.conn, sql, null);
                    if (result != null)
                    {
                        string[] resultArr = result.ToString().Split(',');
                        AuthorityForm af = new AuthorityForm();
                        List<string> listModel = af.ReadMenusXmlData().Where(c => resultArr.Contains(c.ID)).Select(c => c.ID).ToList<string>();
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
        }
    }
}