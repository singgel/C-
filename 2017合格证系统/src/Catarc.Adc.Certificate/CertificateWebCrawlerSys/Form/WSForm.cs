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
    public partial class WSForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
         WSUtils wsUtils;
        string WS_MaxPage = "1";
        string WS_page = "1";
        public WSForm()
        {
            InitializeComponent();
            
        }
        private void WSForm_Load(object sender, EventArgs e)
        {
            //WS_MaxPage = Tool.GetHtmlSourceListMaxPageNum(Tool.SetPostParams(), Tool.strLoginUrl, Tool.strTargerUrlListWS + WS_page);
            //this.CrawlerLog.Items.Add(string.Format("共{0}页", WS_MaxPage));
            ////autoCrawler = new AutoCrawler("WS",this.gridControl1);
            wsUtils = new WSUtils("WS", this.gridControl1, this.CrawlerLog);

        }

        //开始自动抓取
        private void barBtnItemStart_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;

                this.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取完税信息  ", DateTime.Now.ToString("G")));
                }));
                this.CrawlerLog.SelectedIndex = 0;
            
                wsUtils.Start("","1","");
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取完税信息开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //停止自动抓取
        private void barBtnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                wsUtils.Stop();
                this.barBtnItemStart.Enabled = true;
                this.barBtnStop.Enabled = false;
                this.Invoke(new Action(() => { this.CrawlerLog.Items.Add(string.Format("{0} 停止抓取完税信息  ", DateTime.Now.ToString("G"))); }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取完税信息停止异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //查询
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            wsUtils = new WSUtils("WS", this.gridControl2, this.label2);
            string param = queryParam();
            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;
                //this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取完税信息  ", DateTime.Now.ToString("G")));
                this.label2.Invoke(new Action(() =>
                {
                    this.label2.Text = string.Format("{0} 开始抓取完税信息  ", DateTime.Now.ToString("G"));
                }));
               

                wsUtils.Start(param, this.txtFrom.Text, this.txtTo.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取完税信息开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" 1=1 ");
            if (!string.IsNullOrEmpty(dtStart.Text))
            {
                sqlStr.AppendFormat(" AND WS_CREATETIME >= '{0}' ", Convert.ToDateTime(this.dtStart.Text));
            }
            if (!string.IsNullOrEmpty(dtEnd.Text))
            {
                sqlStr.AppendFormat(" AND WS_CREATETIME < '{0}' ", Convert.ToDateTime(this.dtEnd.Text).AddDays(1));
            }
            return sqlStr.ToString();
        }

    }
}