using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using DevExpress.XtraEditors;
using Common;
using FuelDataSysServer.Tool;

namespace FuelDataSysServer
{
    public partial class MainServerForm : DevExpress.XtraEditors.XtraForm
    {
        AutoActionHelper auto;

        public MainServerForm()
        {
            InitializeComponent();
            auto = new AutoActionHelper(userLoginLog, txtDetails);
        }

        private void MainServerForm_Load(object sender, EventArgs e)
        {
            this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 服务器启动 {0} *** ", DateTime.Now.ToString("G"))); })); 
            LogManager.Log("Log", "Log", "*** 服务器启动 *** ");
            auto.Start();
            if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE!='IsAutoUpload'"))
            {
                this.Invoke(new Action(() => { this.barStart.Enabled = false; this.barStop.Enabled = true; }));
                this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始接收合并 {0} *** ", DateTime.Now.ToString("G"))); }));
                LogManager.Log("Log", "Log", "*** 开始接收合并，伴随启动 *** ");
            }
            else
            {
                this.Invoke(new Action(() => { this.barStart.Enabled = true; this.barStop.Enabled = false; }));
                this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止接收合并 {0} *** ", DateTime.Now.ToString("G"))); }));
                LogManager.Log("Log", "Log", "*** 停止接收合并，伴随启动 *** ");
            }
            if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE='IsAutoUpload'"))
            {
                this.Invoke(new Action(() => { this.barStartUpload.Enabled = false; this.barStopUpload.Enabled = true; }));
                this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始上报 {0} *** ", DateTime.Now.ToString("G"))); }));
                LogManager.Log("Log", "Log", "*** 开始上报，伴随启动 *** ");
            }
            else
            {
                this.Invoke(new Action(() => { this.barStartUpload.Enabled = true; this.barStopUpload.Enabled = false; }));
                this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止上报 {0} *** ", DateTime.Now.ToString("G"))); }));
                LogManager.Log("Log", "Log", "*** 停止上报，伴随启动 *** ");
            }
        }

        private void MainServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 服务器关闭 {0} *** ", DateTime.Now.ToString("G"))); })); 
            LogManager.Log("Log", "Log", "*** 服务器关闭 *** ");
            auto.Stop();
            Application.Exit();
        }

        //开始接收合并
        private void barStart_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=1,OPERATOR='Server',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE!='IsAutoUpload'", DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss"));
                //LogManager.Log("Log", "Log", sql);
                if (OracleHelper.ExecuteNonQuery(OracleHelper.conn, sql, null) > 0)
                {
                    this.Invoke(new Action(() => { this.barStart.Enabled = false; this.barStop.Enabled = true; })); 
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始接收合并 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 开始接收合并 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("控制异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //停止接收合并
        private void barStop_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=0,OPERATOR='Server',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE!='IsAutoUpload'", DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss"));
                if (OracleHelper.ExecuteNonQuery(OracleHelper.conn, sql, null) > 0)
                {
                    this.Invoke(new Action(() => { this.barStart.Enabled = true; this.barStop.Enabled = false; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止接收合并 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 停止接收合并 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("控制异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //开始上报
        private void barStartUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=1,OPERATOR='Client',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE='IsAutoUpload'", DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss"));
                if (OracleHelper.ExecuteNonQuery(OracleHelper.conn, sql, null) > 0)
                {
                    this.Invoke(new Action(() => { this.barStartUpload.Enabled = false; this.barStopUpload.Enabled = true; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 开始上报 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 开始上报 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //停止上报
        private void barStopUpload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sql = string.Format("update SYS_AUTOMATIC set STATIC=0,OPERATOR='Client',OPERATOR_TIME=to_date('{0}','yyyy-mm-dd hh24:mi:ss') where AUTOTYPE='IsAutoUpload'", DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss"));
                if (OracleHelper.ExecuteNonQuery(OracleHelper.conn, sql, null) > 0)
                {
                    this.Invoke(new Action(() => { this.barStartUpload.Enabled = true; this.barStopUpload.Enabled = false; }));
                    this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 停止上报 {0} *** ", DateTime.Now.ToString("G"))); })); 
                    LogManager.Log("Log", "Log", "*** 停止上报 *** ");
                }
                else
                {
                    MessageBox.Show("操作失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("控制异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //统计
        private void barStatistics_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.Invoke(new Action(() => { this.userLoginLog.Items.Add(string.Format("*** 统计成功 {0} *** ", DateTime.Now.ToString("G"))); })); 
            StatisticsData statis = new StatisticsData();
            statis.Statistics(this.txtDetails);
        }

        //清空
        private void barClear_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            userLoginLog.Items.Clear();
            txtDetails.Text = string.Empty;
        }

        //保存日志
        private void barSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog() { FileName = "日志_" + DateTime.Now.ToString("yyyy-MM-dd"), Title = "保存日志", Filter = "TXT文件(*.txt)|*.txt" };
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName);
                for (int i = 0; i < this.userLoginLog.Items.Count; i++)
                {
                    sw.WriteLine(userLoginLog.Items[i].ToString());
                }
                sw.Close();
            }
        }

        //版权说明
        private void barCopyright_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            MessageBox.Show(String.Format("燃料消耗量数据管理系统 @[{0}][{1}] CopyRight", DateTime.Now.Year, Utils.qymc));
        }

        //定时器刷新功能状态
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE!='IsAutoUpload'"))
            {
                this.Invoke(new Action(() => { this.barStart.Enabled = false; this.barStop.Enabled = true; })); 
            }
            else
            {
                this.Invoke(new Action(() => { this.barStart.Enabled = true; this.barStop.Enabled = false; })); 
            }
            if (OracleHelper.Exists(OracleHelper.conn, "select count(*) from SYS_AUTOMATIC where STATIC=1 and AUTOTYPE='IsAutoUpload'"))
            {
                this.Invoke(new Action(() => { this.barStartUpload.Enabled = false; this.barStopUpload.Enabled = true; })); 
            }
            else
            {
                this.Invoke(new Action(() => { this.barStartUpload.Enabled = true; this.barStopUpload.Enabled = false; })); 
            }
        }

    }
}