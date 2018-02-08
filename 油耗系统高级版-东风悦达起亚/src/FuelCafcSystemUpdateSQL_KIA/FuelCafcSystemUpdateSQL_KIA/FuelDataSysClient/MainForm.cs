using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;
using FuelDataSysClient.SubForm;
using System.Threading;
using System.Linq;
using DevExpress.XtraNavBar;
using FuelDataSysClient.Tool;
using FuelDataSysClient.Form_BGSC;
using FuelDataSysClient.Form_SJSB;
using FuelDataSysClient.Form_SCSJ;
using FuelDataSysClient.Form_SJBD;
using FuelDataSysClient.Form_SJTJ;
using FuelDataSysClient.Form_SJHS;
using FuelDataSysClient.Form_ZY;
using FuelDataSysClient.Form_SJBG;
using FuelDataSysClient.Form_GJ;
using System.Data;
using Common;

namespace FuelDataSysClient
{
    public partial class MainForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public string strUserId;
        public MainForm()
        {
            InitializeComponent();
            initAuthorityMenu();
            this.barUser.Caption = string.Format("当前用户：{0} 当前线路：{1}", Utils.localUserId, FuelDataSysClient.Properties.Settings.Default.IsFuelTest ? "正式线路" : "测试线路");
            ShowWindows<IndexForm>(true);
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
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //窗体双击
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        //工具框退出
        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //工具框显示
        private void ToolStripMenuItem_Show_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        //窗体加载成功
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (1 <= DateTime.Now.Day && DateTime.Now.Day  <= 10)
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
        }

        //窗体关闭
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 服务器关闭 {0} *** ", DateTime.Now.ToString("G"))); }));
            //LogManager.Log("Log", "Log", "*** 服务器关闭 *** ");
            //Form_SCSJ.SocketDataForm_New
            //auto.Stop();

            DialogResult result;
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
                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                    else if (result == DialogResult.OK)
                    {
                        LogManager.Log("Log", "Log", string.Format("*** 服务器关闭 {0} *** ", DateTime.Now.ToString("G")));
                        this.Dispose(true);
                        this.Close();
                        Application.Exit();
                    }
                    break;
            }
        }

        //窗体大小改变
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

        //head:首页
        private void barBtnHome_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<IndexForm>(true);
        }

        //head:设置
        private void barSetup_ItemClick(object sender, ItemClickEventArgs e)
        {
            using (SetForm SetForm = new SetForm())
            {
                SetForm.ShowDialog();
            }
        }

        //head:油耗参数同步
        private void barSynParam_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SyncParamForm>(true);
        }

        //head:节假日数据同步
        private void barSyncHoliday_ItemClick(object sender, ItemClickEventArgs e)
        {
            ShowWindows<SyncHolidayForm>(true);
        }

        //单挑燃料数据填报
        private void bavBarSingle_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            using (cljbxxForm SetForm = new cljbxxForm())
            {
                SetForm.ShowDialog();
            }
        }

        //整车数据管理
        private void navBarImport_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ImportCLJBXXForm>(true);
        }

        //燃料参数数据管理
        private void navBarImport_VIN_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ImportRLLXPARAMForm>(true);
        }

        //待上报
        private void navBarUpload_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalForm>(true);
        }

        //补传待上报
        private void navBarUploadOT_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalOTForm>(true);
        }

        //已上报
        private void navBarUploaded_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalUploadedForm>(true);
        }

        //已修改未上报数据
        private void navBarUpdate_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SearchLocalUpdateForm>(true);
        }

        //SOCKET已接收数据
        private void navBarCombine_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SocketDataForm_New>(true);
        }

        //SOCKET已处理数据
        private void navBarUncombine_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SocketMergerDataForm>(true);
        }

        //远程数据查询
        private void navBarSearch_Remote_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SearchServerForm>(true);
        }

        //油耗参数明细
        private void navBarDetail_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AvgFuelDetailForm>(true);
        }

        //油耗数据核算
        private void navBarCheck_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<CAFCForm>(true);
        }

        //油耗历史变化
        private void navBarHistory_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<YhlsbhForm>(true);
        }

        //油耗达标排名
        private void navBarRanking_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<FuelRankingForm>(true);
        }

        //本地数据比对——生产线
        private void navBarContrast_Line_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ProductContrastForm>(true);
        }

        //本地数据比对——本地数据
        private void navBarContrast_Local_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ContrastForm>(true);
        }

        //合格证数据比对
        private void navBarCertificate_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<CertificateComparisonForm>(true);
        }

        //通告数据比对
        private void navBarAnnounce_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AnnouncementForm>(true);
        }

        //数据变更申请
        private void navBarModify_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ChangeRibbonForm>(true);
        }

        //补传状态查询
        private void navBarUploadOT_Status_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyUpOTViewForm>(true);
        }

        //修改状态查询
        private void navBarUpdate_Status_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyEditViewForm>(true);
        }

        //撤销状态查询
        private void navBarRecall_Status_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<ApplyDelViewForm>(true);
        }

        //年中报告生成
        private void navBarMiddle_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<MiddleReportForm>(true);
        }

        //年度报告声称
        private void navBarWhole_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<WholeReportForm>(true);
        }

        //预报告生成
        private void navBarPreReport_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<PreReportForm>(true);
        }

        //系统设置
        private void navBarSet_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            using (SetForm SetForm = new SetForm())
            {
                SetForm.ShowDialog();
            }
        }

        //油耗参数同步
        private void navBarParams_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SyncParamForm>(true);
        }

        //节假日数据同步
        private void navBarHoliday_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<SyncHolidayForm>(true);
        }

        //4阶段计算
        private void navBarCafaTemp_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<CafcTempCountForm>(true);
        }
        //权限管理
        private void navBarAtuhority_LinkClicked(object sender, NavBarLinkEventArgs e)
        {
            ShowWindows<AuthorityForm>(true);
        }

        private void initAuthorityMenu() 
        {
            var menuString = OracleHelper.ExecuteScalar(OracleHelper.conn, String.Format("SELECT AUTHORITY FROM SYS_USERINFO WHERE USERNAME='{0}'", Utils.localUserId), null);
            if (menuString != null)
            {
                string[] menuArr = menuString.ToString().Split(',');
                AuthorityForm af = new AuthorityForm();
                List<string> listModel = af.ReadMenusXmlData().Where(c => menuArr.Contains(c.ID.ToString())).Select(c => c.MenuName).ToList<string>();

                foreach (NavBarGroup g in navBarControl1.Groups)
                {
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
        }
       
    }
}