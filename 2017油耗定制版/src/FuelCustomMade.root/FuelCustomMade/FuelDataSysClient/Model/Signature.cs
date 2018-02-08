using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Model
{
    public class Signature
    {
        //主键
        public int ID { get; set; }
        //用户ID
        public string UserName_ID { get; set; }
        //企业名称
        public string QYMC { get; set; }
        //图片系统名称
        public string IMG_NEW_NAME { get; set; }
        //图片原文件名
        public string IMG_OLD_NAME { get; set; }
        //上传日期
        public DateTime APP_DATE { get; set; }
        //更新日期
        public DateTime UPD_DATE { get; set; }
        //状态
        public int STATUS { get; set; }
    }
}
