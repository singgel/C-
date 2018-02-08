using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using SQLHelper;
using System.Data;
using Common;

namespace CertificateWindowsService
{
    class CertificateProcess
    {
        public string connectionString = "";//数据库连接
        public bool bStop = false;//是否停止
        DateTime dtQuery = new DateTime();//查询时间
        public int iInterval = 3600;//查询间隔时间（s）
        public String filename = "";//生成文件名
        DateTime dtDelete = new DateTime();//删除时间
        public int iDeleteInterval = 1;//删除时间  （天）

        /// <summary>
        /// 开始服务
        /// </summary>
        public void Start()
        {
            //创建线程
            Thread thread1 = new Thread(run);
            thread1.IsBackground = true;
            thread1.Start();
        }
        /// <summary>
        /// 开始线程
        /// </summary>
        public void run()
        {
            while(true)
            {
                if (bStop)
                {
                    break;
                }
                if (DateTime.Now > dtQuery.AddSeconds(iInterval))
                {
                    try
                    {
                        //查询数据
                        SqlHelper sh = new SqlHelper();
                        DataTable dt = sh.QuerySingleDT(connectionString, "Select top 5000 * from HGZ_APPLIC order by H_ID");
                        //导出数据
                        Export export = new Export();
                        export.ExportDataToCsv(dt, GetFilePath());
                        //删除数据
                        List<string> sqlStringList = new List<string>();
                        foreach(DataRow dr in dt.Rows)
                        {
                            sqlStringList.Add(String.Format("Delete from HGZ_APPLIC where H_ID = '{0}'", dr["H_ID"].ToString()));
                        }
                        sh.ExecuteSqlTran(connectionString, sqlStringList);
                        dtQuery = DateTime.Now;
                    }
                    catch(Exception ex)
                    {
                        LogManager.Log("Log", "Error", ex.Message);     
                    }
                }

                if (DateTime.Now > dtDelete.AddHours(iDeleteInterval))
                {
                    try
                    {
                        //删除上个月数据
                        Export export = new Export();
                        export.ClearLastMonthData(filename);
                        dtDelete = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Log("Log", "Error", ex.Message);
                    }
                }
                Thread.Sleep(1000);
            }
        }
        
        /// <summary>
        /// 获取CSV路径
        /// </summary>
        String  GetFilePath()
        {
            String path = String.Format("{0}\\{1}\\{2}.csv",filename, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMddHHmmss"));  
            return path;
        }
    }
}
