using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertificateWebCrawlerWindowsService
{

    //队列中的数据结构
    public class DetailInfo
    {
        public int itype;
        public string SQBH;//SQBH
        public string APP_TIME;
        public string APP_TYPE;
        public string TargerUrl;

        public DetailInfo()
        {
            SQBH = "";
            APP_TIME = "";
            APP_TYPE = "";
            TargerUrl = "";
        }
    }
    //存储队列
    public class InsertQueue
    {
        List<DetailInfo> lsData = new List<DetailInfo>();//存放数据

        public InsertQueue()
        {

        }
        //插入数据
        public void Push(DetailInfo data)
        {
            lock (lsData)
            {
                lsData.Add(data);
            }
        }
        //取出数据
        public DetailInfo Pop()
        {
            DetailInfo lsTemp = null;
            lock (lsData)
            {
                if (lsData.Count > 0)
                {
                    lsTemp = lsData[0];
                    lsData.RemoveAt(0);
                }
            }
            return lsTemp;
        }
        //取出队列大小
        public int Size()
        {
            int size = 0;
            lock (lsData)
            {
                size = lsData.Count;
            }
            return size;
        }
    }

}
