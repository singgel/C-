using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESPlus.Core;
using ESBasic.Loggers;
using ESFramework.Core;
using System.Configuration;
using ESPlus.Application.P2PSession;
using ESPlus;
using ESBasic.Threading.Engines;
using ESBasic;
using System.Diagnostics;
using ESFramework.Engine;
using ESPlus.Rapid;

namespace OAUS.Server
{
    public partial class MainForm : Form
    {
        private bool toExit = false;
        private IRapidServerEngine rapidServerEngine;         

        public MainForm(IRapidServerEngine engine)
        {
            InitializeComponent();
           
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            this.rapidServerEngine = engine;            
            this.label_port.Text = string.Format("{0} [TCP]",this.rapidServerEngine.Port );          
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.GotoExit();
        }

        private void GotoExit()
        {
            if (ESBasic.Helpers.WindowsHelper.ShowQuery("您确定要退出OAUS服务器吗？"))
            {                
                this.toExit = true;
                this.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.toExit)
            {
                this.Visible = false;
                e.Cancel = true;
                return;
            }

            this.rapidServerEngine.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FileVersionForm form = new FileVersionForm(Program.UpgradeConfiguration);
                form.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }
    }
}
