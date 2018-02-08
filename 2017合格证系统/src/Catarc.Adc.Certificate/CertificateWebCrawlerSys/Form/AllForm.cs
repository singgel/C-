using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using CertificateWebCrawlerSys.Utils;
using CertificateWebCrawlerSys.Properties;
using System.Timers;
using System.Net;

namespace CertificateWebCrawlerSys
{
    public partial class AllForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        CookieContainer cookieContainer = new CookieContainer();
        private string[] tableNames = new string[] { "机动车合格证申请", "配置信息", "完税信息" };
        List<string> selectedParamEntityIds = new List<string>();
        HGZUtils hgzUtils;
        PZUtils pzUtils;
        WSUtils wsUtils;
        HGZUtilsThreadPool hgzUtilsPool;
        string dtStart;
        LogManager logMagr = new LogManager();

        public AllForm()
        {
            InitializeComponent();
        }


        private void AllForm_Load(object sender, EventArgs e)
        {
            //填充待抓取的数据资源名称
            //DataTable dtName = new DataTable();
            //dtName.Columns.Add("check", System.Type.GetType("System.Boolean"));
            //dtName.Columns["check"].ReadOnly = false;
            //dtName.Columns.Add("name", System.Type.GetType("System.String"));
            //for (int i = 0; i < tableNames.Length; i++)
            //{
            //    dtName.Rows.Add(false, tableNames[i]);
            //}
            //this.gcName.DataSource = dtName;
            hgzUtils = new HGZUtils("HGZ", null, this.CrawlerLog);
            pzUtils = new PZUtils("PZ", null, this.CrawlerLog);
            wsUtils = new WSUtils("WS", null, this.CrawlerLog);
            hgzUtilsPool = new HGZUtilsThreadPool("HGZ", null, this.CrawlerLog);
            this.txtTime.Text = Settings.Default.RunT;
            this.txtTime.Text = Settings.Default.RunMinute;
            this.dtStart1.Text = DateTime.Now.Date.AddDays(-1).ToString();
        }
        //保存配置
        private void barBtnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            string runHour = this.txtTime.Text;
            string runMin = txtMin.Text;

            if (string.IsNullOrEmpty(runHour))
            {
                MessageBox.Show("开始抓取数据时间小时未设置，默认为1点", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                runHour = "1";
            }
            else if (string.IsNullOrEmpty(runMin))
            {
                MessageBox.Show("开始抓取数据时间分钟未设置，默认为0分", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                runMin = "0";
            }

            Settings.Default.RunT = runHour.Trim();
            Settings.Default.RunMinute = runMin.Trim();
            MessageBox.Show("开始抓取数据时间设置为" + Settings.Default.RunT + "点" + Settings.Default.RunMinute + "分", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);


        }
        //定时抓取
        private void barBtnItemStart_ItemClick(object sender, ItemClickEventArgs e)
        {
            string msg = string.Format("{0} 定时启动已开启,开始抓取时间为{1}点{2}分。", DateTime.Now.ToString("G"), Settings.Default.RunT, Settings.Default.RunMinute);
            logMagr.WriteToFile(msg);
            //cookieContainer = Tool.Login();
            //HGZ_Start();
            //PZ_Start();
            //WS_Start();

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(RunDateInit);
            // 设置引发时间的时间间隔 此处设置为1分钟检测一次
            aTimer.Interval = 1000 * 60 * 1;
            //设置是执行一次（false）还是一直执行(true)，默认为true 
            aTimer.AutoReset = true;
            //开始计时
            aTimer.Enabled = true;

            //System.Threading.Thread.Sleep(1000 * 60 * 1);

        }

        private void barBtnStop_ItemClick(object sender, ItemClickEventArgs e)
        {
            hgzUtilsPool.Stop();
            hgzUtils.Stop();
            pzUtils.Stop();
            wsUtils.Stop();
        }

        private void RunDateInit(object source, ElapsedEventArgs e)
        {
            int iRunTime = int.Parse(Settings.Default.RunT);
            int iRunHour = int.Parse(Settings.Default.RunMinute);
            //在每天的定时执行一次
            if (DateTime.Now.Hour == iRunTime && DateTime.Now.Minute == iRunHour)
            {
                string msg = string.Format("{0} 开始抓取数据", DateTime.Now.ToString("G"));
                logMagr.WriteToFile(msg);

                this.dtStart1.Text = DateTime.Now.Date.AddDays(-1).ToString();
                cookieContainer = Tool.Login();
                HGZ_Start();
                PZ_Start();
                WS_Start();
            }
        }

        //开始自动抓取
        private void HGZ_Start()
        {
            try
            {
                string param = queryParam("HGZ");
                this.CrawlerLog.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取机动车合格证申请数据  ", DateTime.Now.ToString("G")));
                }));
                //this.CrawlerLog.SelectedIndex = 0;

                //hgzUtils = new HGZUtils("HGZ", null, this.CrawlerLog);
                //hgzUtils.StartThreadPool("", "1", "");
                hgzUtilsPool = new HGZUtilsThreadPool("HGZ", null, this.CrawlerLog);
                hgzUtilsPool.StartThreadPool(param, "1", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("机动车合格证申请抓取开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        //开始自动抓取
        private void PZ_Start()
        {
            try
            {
                this.CrawlerLog.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取配置信息  ", DateTime.Now.ToString("G")));
                }));
                string param = queryParam("PZ");
                pzUtils = new PZUtils("PZ", null, this.CrawlerLog);
                pzUtils.Start(param, "", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取配置信息开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        //开始自动抓取
        private void WS_Start()
        {
            try
            {
                this.CrawlerLog.Invoke(new Action(() =>
                {
                    this.CrawlerLog.Items.Add(string.Format("{0} 开始抓取完税信息  ", DateTime.Now.ToString("G")));
                }));

                wsUtils = new WSUtils("WS", null, this.CrawlerLog);
                wsUtils.Start(queryParam("WS"), "1", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("抓取完税信息开始异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }


        //查询条件
        private string queryParam(string timeType)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(" 1=1 ");
            string timetype = string.Empty;
            if ("HGZ".Equals(timeType))
            {
                timetype = "APP_TIME";
            }
            else if ("PZ".Equals(timeType))
            {
                timetype = "PZ_UPDATETIME";
            }
            else if ("WS".Equals(timeType))
            {
                timetype = "CREATETIME";
            }
            else
            {
                return "";
            }

            if (!string.IsNullOrEmpty(dtStart1.Text))
            {
                sqlStr.AppendFormat(" AND {0} >= '{1}' ", timetype, Convert.ToDateTime(this.dtStart1.Text));
            }
            if (!string.IsNullOrEmpty(dtEnd.Text))
            {
                sqlStr.AppendFormat(" AND {0} <='{1}' ", timetype, Convert.ToDateTime(this.dtEnd.Text).AddDays(1));
            }
            return sqlStr.ToString();
        }

        //选择选中数据
        private void GetCheckData()
        {
            var gridControl = gcName;
            var view = gridControl.MainView;
            view.PostEditor();
            DataView dv = (DataView)view.DataSource;
            selectedParamEntityIds = SelectedParamEntityIds(dv, "name");
            //if (selectedParamEntityIds.Count > 0)
            //{
            //    string strIDs = "('" + string.Join("','", selectedParamEntityIds.ToArray()) + "')";
            //    return strIDs;//List2Str(selectedParamEntityIds);
            //}

        }

        //选中选择项 数据源 返回选择列
        public static List<string> SelectedParamEntityIds(DataView dv, string column)
        {
            List<string> selectList = new List<string>();

            bool result = false;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    bool.TryParse(dv.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        selectList.Add(dv.Table.Rows[i][column].ToString());
                    }
                }
            }
            return selectList;
        }

        //List string 转换成 vins 逗号分隔
        public static string List2Str(List<string> list)
        {
            string vins = "(";
            foreach (string r in list)
            {
                vins += "'" + r + "'" + ",";
            }
            vins += "''" + ")";
            return vins;
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

        }


    }
}