using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data;
using Common;
using System.Windows.Forms;

namespace FuelDataSysServer.Tool
{
    class AutoActionHelper
    {
        private bool isStopped = false;
        private int iImportInterval = 300;
        private int iMergerInterval = 300;
        private int iUploadInterval = 300;
        private int iStatisticsInterval = 300;
        public bool IsStopped
        {
            get { return isStopped; }
            set { isStopped = value; }
        }
        TextBox txtDetails;
        ListBox userLoginLog;
        AutoSocket socket = new AutoSocket();
        public AutoActionHelper(ListBox listBox,TextBox textBox)
        {
            this.userLoginLog = listBox;
            this.txtDetails = textBox;
        }

        //开始监控
        public void Start()
        {
            IsStopped = true;
            try
            {
                Thread thAutoImport;//获取线程
                Thread thAutoMerger;//合并线程
                Thread thAutoUpload;//上报线程
                Thread thAutoStatistics;//统计线程
                thAutoImport = new Thread(new ThreadStart(AutoImport));  //获取线程
                thAutoImport.IsBackground = true;
                thAutoImport.Start();

                thAutoMerger = new Thread(new ThreadStart(AutoMerger));  //合并线程
                thAutoMerger.IsBackground = true;
                thAutoMerger.Start();

                thAutoUpload = new Thread(new ThreadStart(AutoUpload));  //上传线程
                thAutoUpload.IsBackground = true;
                thAutoUpload.Start();

                thAutoStatistics = new Thread(new ThreadStart(AutoStatistics));  //统计线程
                thAutoStatistics.IsBackground = true;
                thAutoStatistics.Start();
            }
            catch (System.Exception ex)
            {
                LogManager.Log("Log", "Error", ex.Message);
            }
        }
        //停止监控
        public void Stop()
        {
            IsStopped = false;
        }
        //自动获取数据
        private void AutoImport()
        {
            int i = 0;
            while (true)
            {
                if (IsStopped)
                {
                    try
                    {
                        if (OracleHelper.Exists(OracleHelper.conn, "SELECT count(*) FROM SYS_AUTOMATIC WHERE AUTOTYPE='IsAutoImport' AND STATIC=1"))
                        {
                            if (!socket.IsStopped)
                            {
                                LogManager.Log("Log", "Log", "== 自动Socket接收打开 ==");
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==  自动Socket接收打开 ==");
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                                try
                                {
                                    socket.Import(this.userLoginLog);
                                   
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Log("Log", "Error", ex.Message);
                                    this.userLoginLog.Invoke(new Action(() =>
                                    {
                                        this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动Socket失败：" + ex.Message + "==");
                                        this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                    }));
                                }
                            }
                        }
                        else
                        {
                            if (socket.IsStopped)
                            {
                                LogManager.Log("Log", "Log", "== 自动Socket接收停止 ==");
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==  自动Socket接收停止 ==");
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                                try
                                {
                                    socket.Stop();
                                   // socket.IsStopped = false;
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Log("Log", "Error", ex.Message);
                                    this.userLoginLog.Invoke(new Action(() =>
                                    {
                                        this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动Socket失败：" + ex.Message + "==");
                                        this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                    }));
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Log("Log", "Error", ex.Message);
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动Socket失败：" + ex.Message + "==");
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                    }
                    
                   
                }
                else
                {
                    break;
                }
                i++;
                Thread.Sleep(5000);
            }

        }
        //自动合并
        private void AutoMerger()
        {
            int i = 0;
            while (true)
            {
                if (IsStopped)
                {
                    try
                    {
                        if (OracleHelper.Exists(OracleHelper.conn, "SELECT count(*) FROM SYS_AUTOMATIC WHERE AUTOTYPE='IsAutoMerger' AND STATIC=1"))
                        {
                           
                            if (i % iMergerInterval == 0)
                            {
                                LogManager.Log("Log", "Log", "== 自动Merger合并打开 ==");
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "== 自动Merger合并打开 ==");
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                                try
                                {
                                    MergerData merger = new MergerData();
                                    Dictionary<string, string> mapMessage = new Dictionary<string, string>();
                                    merger.Merger("", mapMessage, this.userLoginLog);
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Log("Log", "Error", ex.Message);
                                    this.userLoginLog.Invoke(new Action(() =>
                                    {
                                        this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动合并失败:" + ex.Message + "==");
                                        this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                    }));
                                }
                            }

                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Log("Log", "Error", ex.Message);
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动合并失败：" + ex.Message + "==");
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                    }
                   
                    
                }
                else
                {
                    break;
                }
                i++;
                Thread.Sleep(1000);

            }

        }
        //自动上报
        private void AutoUpload()
        {
            int i = 0;
            while (true)
            {
                if (IsStopped)
                {
                    try
                    {
                        if (OracleHelper.Exists(OracleHelper.conn, "SELECT count(*) FROM SYS_AUTOMATIC WHERE AUTOTYPE='IsAutoUpload' AND STATIC=1"))
                        {
                           
                            if (i % iUploadInterval == 0)
                            {
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "== 自动Upload上报打开 ==");
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                                try
                                {
                                    UploadData upload = new UploadData();
                                    upload.Upload(this.userLoginLog);

                                }
                                catch (Exception ex)
                                {
                                    LogManager.Log("Log", "Error", ex.Message);
                                    this.userLoginLog.Invoke(new Action(() =>
                                    {
                                        this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动上报失败：" + ex.Message + "==");
                                        this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                    }));
                                }
                            }

                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Log("Log", "Error", ex.Message);
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动上报失败：" + ex.Message + "==");
                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                    }
                    

                }
                else
                {
                    break;
                }
                i++;
                Thread.Sleep(1000);
            }
        }
        //自动统计
        private void AutoStatistics()
        {
            int i = 0;
            while (true)
            {
                if (IsStopped)
                {
                    try
                    {
                        if (OracleHelper.Exists(OracleHelper.conn, "SELECT count(*) FROM SYS_AUTOMATIC WHERE AUTOTYPE='IsAutoCount' AND STATIC=1"))
                        {
                            
                            if (i % iStatisticsInterval == 0)
                            {
                                LogManager.Log("Log", "Log", "== 自动Statistics统计打开 ==");
                                this.userLoginLog.Invoke(new Action(() =>
                                {
                                    this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "== 自动Statistics统计打开 ==");
                                    this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                }));
                                try
                                {
                                    if (DateTime.Now.Hour.Equals(04))
                                    {
                                        StatisticsData statis = new StatisticsData();
                                        statis.Statistics(this.txtDetails);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogManager.Log("Log", "Error", ex.Message);
                                    this.userLoginLog.Invoke(new Action(() =>
                                    {
                                        this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动统计失败：" + ex.Message + "==");
                                        this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                                    }));
                                }
                            }

                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Log("Log", "Error", ex.Message);
                        this.userLoginLog.Invoke(new Action(() =>
                        {
                            this.userLoginLog.Items.Add(DateTime.Now.ToString("G") + "==自动统计失败：" + ex.Message + "==");

                            this.userLoginLog.TopIndex = this.userLoginLog.Items.Count - 1;
                        }));
                    }
                }
                else
                {
                    break;
                }
                i++;
                Thread.Sleep(1000);
            }
        }
    }
}
