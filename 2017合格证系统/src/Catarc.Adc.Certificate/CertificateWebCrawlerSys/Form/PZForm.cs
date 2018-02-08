using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using CertificateWebCrawlerSys.Utils;

namespace CertificateWebCrawlerSys
{
    public partial class PZForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        PZUtils pzUtils;
        string PZ_MaxPage = "1";
        string PZ_page = "1";
        public PZForm()
        {
            InitializeComponent();
            
        }
        private void PZForm_Load(object sender, EventArgs e)
        {
            //PZ_MaxPage = Tool.GetHtmlSourceListMaxPageNum(Tool.SetPostParams(), Tool.strLoginUrl, Tool.strTargerUrlListPZ + PZ_page);
            //this.CrawlerLog.Items.Add(string.Format("共{0}页", PZ_MaxPage));
            ////autoCrawler = new AutoCrawler("PZ",this.gridControl1);
            pzUtils = new PZUtils("PZ", this.gridControl1, this.CrawlerLog);

        }

        //开始自动抓取
        private void barBtnItemStart_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;
                //this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取配置信息  ", DateTime.Now.ToString("G")));
                this.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取配置信息  ", DateTime.Now.ToString("G")));
                }));
                this.CrawlerLog.SelectedIndex = 0;
           
                pzUtils.Start("","","");
               
                //logMgr.WriteToFile(string.Format("{0}开始发送", System.DateTime.Now.ToString("G")));
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取配置信息开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //停止自动抓取
        private void barBtnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                pzUtils.Stop();
                this.barBtnItemStart.Enabled = true;
                this.barBtnStop.Enabled = false;
                this.Invoke(new Action(() => { this.CrawlerLog.Items.Add(string.Format("{0} 停止抓取配置信息  ", DateTime.Now.ToString("G"))); }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取配置信息停止异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //查询
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            pzUtils = new PZUtils("PZ", this.gridControl2, this.label1);
            string param = queryParam();
            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;
                //this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取配置信息  ", DateTime.Now.ToString("G")));
                this.label1.Invoke(new Action(() =>
                {
                    this.label1.Text = string.Format("{0} 开始抓取配置信息  ", DateTime.Now.ToString("G"));
                }));
                

                pzUtils.Start(param, this.txtFrom.Text, this.txtTo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取配置信息开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" 1=1 ");
            if (!string.IsNullOrEmpty(dtStart.Text))
            {
                sqlStr.AppendFormat(" AND PZ_UPDATETIME >= '{0}' ", Convert.ToDateTime(this.dtStart.Text));
            }
            if (!string.IsNullOrEmpty(dtEnd.Text))
            {
                sqlStr.AppendFormat(" AND PZ_UPDATETIME < '{0}' ", Convert.ToDateTime(this.dtEnd.Text).AddDays(1));
            }
            return sqlStr.ToString();
        }


    }
}