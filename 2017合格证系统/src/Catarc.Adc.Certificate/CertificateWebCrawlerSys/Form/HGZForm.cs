using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using CertificateWebCrawlerSys.Utils;
using System.Net;
using System.IO;

namespace CertificateWebCrawlerSys
{
    public partial class HGZForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        HGZUtilsThreadPool hgzUtilsPool;
        HGZUtils hgzUtils;

        public HGZForm()
        {
            InitializeComponent();

        }
        private void HGZForm_Load(object sender, EventArgs e)
        {
            hgzUtils = new HGZUtils("HGZ", this.gridControl1, this.CrawlerLog);
            hgzUtilsPool = new HGZUtilsThreadPool("HGZ", this.gridControl1, this.CrawlerLog);
        }

        //开始自动抓取
        private void barBtnItemStart_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;

                this.CrawlerLog.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取机动车合格证申请数据  ", DateTime.Now.ToString("G")));
                }));
                this.CrawlerLog.SelectedIndex = 0;

                //hgzUtils = new HGZUtils("HGZ", this.gridControl1, this.CrawlerLog);
                //hgzUtils.StartThreadPool("", "1", "");
                hgzUtilsPool = new HGZUtilsThreadPool("HGZ", this.gridControl1, this.CrawlerLog);
                hgzUtilsPool.StartThreadPool("", "1", "1000");
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        //停止自动抓取
        private void barBtnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                hgzUtilsPool.Stop();
                hgzUtils.Stop();
                this.barBtnItemStart.Enabled = true;
                this.barBtnStop.Enabled = false;
                this.Invoke(new Action(() => { this.CrawlerLog.Items.Add(string.Format("{0} 停止抓取  ", DateTime.Now.ToString("G"))); }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取停止异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //查询
        private void simpleButton2_Click(object sender, EventArgs e)
        {

            string param = queryParam();
            string paramPageFrom = this.txtFrom.Text;
            string paramPageTo = this.txtTo.Text;
            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;
                this.label2.Invoke(new Action(() =>
                {
                    this.label2.Text = string.Format("{0} 开始抓取机动车合格证申请数据  ", DateTime.Now.ToString("G"));
                }));

                hgzUtils = new HGZUtils("HGZ", this.gridControl2, this.label2);
                hgzUtils.StartThreadPool(param, paramPageFrom, paramPageTo);
                //hgzUtilsPool = new HGZUtilsThreadPool("HGZ", this.gridControl1, this.CrawlerLog);
                //hgzUtilsPool.StartThreadPool(param, paramPageFrom, paramPageTo);

            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //查询条件
        private string queryParam()
        {
            var sqlStr = new StringBuilder();
            sqlStr.Append(" 1=1 ");
            if (!string.IsNullOrEmpty(dtStart.Text))
            {
                sqlStr.AppendFormat(" AND APP_TIME >= '{0}' ", Convert.ToDateTime(this.dtStart.Text));
            }
            if (!string.IsNullOrEmpty(dtEnd.Text))
            {
                sqlStr.AppendFormat(" AND APP_TIME < '{0}' ", Convert.ToDateTime(this.dtEnd.Text).AddDays(1));
            }
            return sqlStr.ToString();
        }

        /// <summary>
        /// 导入ID列表的TXT文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void barBtnReadPAGE_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            string fileName = file.FileName;
            StringBuilder sbID = new StringBuilder();
            StreamReader sr = new StreamReader(fileName, Encoding.Default);
            String line;
            bool isNull = true;
            while ((line = sr.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    sbID.Append(",");
                    sbID.Append(line.ToString().Trim());
                    isNull = false;
                }
            }
            if (isNull)//如果文件内容为空
            {
                MessageBox.Show("文件为空。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.barBtnItemStart.Enabled = false;
                this.barBtnStop.Enabled = true;

                this.CrawlerLog.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取机动车合格证申请数据  ", DateTime.Now.ToString("G")));
                }));
                this.CrawlerLog.SelectedIndex = 0;

                hgzUtils = new HGZUtils("HGZ", this.gridControl1, this.CrawlerLog);
                hgzUtils.Start(sbID.ToString().Substring(1));
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


    }
}