using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Model
{
    public class ResultMap
    {
        //主键
        public int ID { get; set; }
        //生产企业
        public string scqy { get; set; }
        //通用名称
        public string tymc { get; set; }
        //车辆型号
        public string clxh { get; set; }
        //车辆种类
        public string clzl { get; set; }
        //排量
        public string pl { get; set; }
        //额定功率
        public string edgl { get; set; }
        //变速器类型
        public string bsqlx { get; set; }
        //市区工况
        public string sqgk { get; set; }
        //市郊工况
        public string sjgk { get; set; }
        //综合工况
        public string zhgk { get; set; }
        //通告日期
        public string tgrq { get; set; }
        //备注
        public string bz { get; set; }
        //车辆产地
        public string clcd { get; set; }
        //车辆品牌
        public string clpp { get; set; }
        //燃料类型
        public string rllx { get; set; }
        //驱动形式
        public string qdxs { get; set; }
        //备案号
        public string baID { get; set; }
        //发动机型号
        public string fdjxh { get; set; }
        //整车整备质量
        public string zczbzl { get; set; }
        //最大设计总质量
        public string zdsjzzl { get; set; }
        //适用国家标准
        public string sygjbz { get; set; }
        //数据录入时间
        public string sjlrsj { get; set; }

        public ResultMap()
        {

        }
    }
}
