using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertificateWindowsService.Model
{
    public class HGZ_APPLIC
    {

        /// <summary>
        /// 申请序列号
        /// </summary>
        public virtual string H_ID { get; set; }

        /// <summary>
        /// QYID_BJ
        /// </summary>
        public virtual string QYID_BJ { get; set; }

        /// <summary>
        /// QYID
        /// </summary>
        public virtual string QYID { get; set; }

        /// <summary>
        /// 车辆状态信息
        /// </summary>
        public virtual string CLZTXX { get; set; }

        /// <summary>
        /// 完整合格证编号
        /// </summary>
        public virtual string WZHGZBH { get; set; }

        /// <summary>
        /// 发证日期
        /// </summary>
        public virtual DateTime? FZRQ { get; set; }

        /// <summary>
        /// 车辆制造企业名称
        /// </summary>
        public virtual string CLZZQYMC { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public virtual string CLLX { get; set; }

        /// <summary>
        /// 车辆名称
        /// </summary>
        public virtual string CLMC { get; set; }

        /// <summary>
        /// 车辆品牌
        /// </summary>
        public virtual string CLPP { get; set; }

        /// <summary>
        /// 车辆型号
        /// </summary>
        public virtual string CLXH { get; set; }

        /// <summary>
        /// 车辆颜色
        /// </summary>
        public virtual string CLYS { get; set; }

        /// <summary>
        /// 底盘型号
        /// </summary>
        public virtual string DPXH { get; set; }

        /// <summary>
        /// 地盘ID
        /// </summary>
        public virtual string DPID { get; set; }

        /// <summary>
        /// 地盘合格证编号
        /// </summary>
        public virtual string DPHGZBH { get; set; }

        /// <summary>
        /// 车辆识别代号
        /// </summary>
        public virtual string CLSBDM { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        public virtual string CJH { get; set; }

        /// <summary>
        /// 发动机号
        /// </summary>
        public virtual string FDJH { get; set; }

        /// <summary>
        /// 发动机型号
        /// </summary>
        public virtual string FDJXH { get; set; }

        /// <summary>
        /// 燃料种类
        /// </summary>
        public virtual string RLZL { get; set; }

        /// <summary>
        /// 排放标准
        /// </summary>
        public virtual string PFBZ { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        public virtual string PL { get; set; }

        /// <summary>
        /// 功率
        /// </summary>
        public virtual string GL { get; set; }

        /// <summary>
        /// 转向形式
        /// </summary>
        public virtual string ZXXS { get; set; }

        /// <summary>
        /// 前轮距
        /// </summary>
        public virtual string QLJ { get; set; }

        /// <summary>
        /// 后轮距
        /// </summary>
        public virtual string HLJ { get; set; }

        /// <summary>
        /// 轮胎数
        /// </summary>
        public virtual string LTS { get; set; }

        /// <summary>
        /// 轮胎规格
        /// </summary>
        public virtual string LTGG { get; set; }

        /// <summary>
        /// 钢板弹簧片数
        /// </summary>
        public virtual string GBTHPS { get; set; }

        /// <summary>
        /// 轴距
        /// </summary>
        public virtual string ZJ { get; set; }

        /// <summary>
        /// 轴荷
        /// </summary>
        public virtual string ZH { get; set; }

        /// <summary>
        /// 轴数
        /// </summary>
        public virtual string ZS { get; set; }

        /// <summary>
        /// 外廓长
        /// </summary>
        public virtual string WKC { get; set; }

        /// <summary>
        /// 外廓宽
        /// </summary>
        public virtual string WKK { get; set; }

        /// <summary>
        /// 外廓高
        /// </summary>
        public virtual string WKG { get; set; }

        /// <summary>
        /// 货箱内部长
        /// </summary>
        public virtual string HXNBC { get; set; }

        /// <summary>
        /// 货箱内部宽
        /// </summary>
        public virtual string HXNBK { get; set; }

        /// <summary>
        /// 货箱内部高
        /// </summary>
        public virtual string HXNBG { get; set; }

        /// <summary>
        /// 总货量
        /// </summary>
        public virtual string ZHL { get; set; }

        /// <summary>
        /// ZZL
        /// </summary>
        public virtual string ZZL { get; set; }

        /// <summary>
        /// 额定载质量
        /// </summary>
        public virtual string EDZZL { get; set; }

        /// <summary>
        /// 整备质量
        /// </summary>
        public virtual string ZBZL { get; set; }

        /// <summary>
        /// 载质量利用系数
        /// </summary>
        public virtual string ZZLLYXS { get; set; }

        /// <summary>
        /// 准牵引总质量
        /// </summary>
        public virtual string ZQYZZL { get; set; }

        /// <summary>
        /// 额定载客
        /// </summary>
        public virtual string EDZK { get; set; }

        /// <summary>
        /// 半挂车鞍座最大允许总质量
        /// </summary>
        public virtual string BGCAZZDYXZZL { get; set; }

        /// <summary>
        /// 驾驶室准乘人数
        /// </summary>
        public virtual string JSSZCRS { get; set; }

        /// <summary>
        /// QZDFS
        /// </summary>
        public virtual string QZDFS { get; set; }

        /// <summary>
        /// HZDFS
        /// </summary>
        public virtual string HZDFS { get; set; }

        /// <summary>
        /// QZDCZFS
        /// </summary>
        public virtual string QZDCZFS { get; set; }

        /// <summary>
        /// HZDCZFS
        /// </summary>
        public virtual string HZDCZFS { get; set; }

        /// <summary>
        /// ZGCS
        /// </summary>
        public virtual string ZGCS { get; set; }

        /// <summary>
        /// 转向轴数
        /// </summary>
        public virtual string ZXZS { get; set; }

        /// <summary>
        /// 最高设计车速
        /// </summary>
        public virtual string ZGSJCS { get; set; }

        /// <summary>
        /// 车辆制造日期
        /// </summary>
        public virtual DateTime CLZZRQ { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string BZ { get; set; }

        /// <summary>
        /// 企业标准
        /// </summary>
        public virtual string QYBZ { get; set; }

        /// <summary>
        /// 产品生产地址
        /// </summary>
        public virtual string CPSCDZ { get; set; }

        /// <summary>
        /// 车辆生产单位名称
        /// </summary>
        public virtual string CLSCDWMC { get; set; }

        /// <summary>
        /// 油耗
        /// </summary>
        public virtual string YH { get; set; }

        /// <summary>
        /// 纯电动标记
        /// </summary>
        public virtual string CDDBJ { get; set; }

        /// <summary>
        /// VERCODE
        /// </summary>
        public virtual string VERCODE { get; set; }

        /// <summary>
        /// HD_HOST
        /// </summary>
        public virtual string HD_HOST { get; set; }

        /// <summary>
        /// RESPONSE_CODE
        /// </summary>
        public virtual string RESPONSE_CODE { get; set; }

        /// <summary>
        /// CLIENT_HARDWARE_INFO
        /// </summary>
        public virtual string CLIENT_HARDWARE_INFO { get; set; }

        /// <summary>
        /// APPLICMEMO
        /// </summary>
        public virtual string APPLICMEMO { get; set; }


        /// <summary>
        /// APPLICTYPE
        /// </summary>
        public virtual string APPLICTYPE { get; set; }

        /// <summary>
        /// APPLICTIME
        /// </summary>
        public virtual DateTime APPLICTIME { get; set; }

        /// <summary>
        /// STATUS
        /// </summary>
        public virtual string STATUS { get; set; }

        /// <summary>
        /// APPROVETIME
        /// </summary>
        public virtual DateTime APPROVETIME { get; set; }

        /// <summary>
        /// APPROVEUSER
        /// </summary>
        public virtual string APPROVEUSER { get; set; }

        /// <summary>
        /// APPROVEMEMO
        /// </summary>
        public virtual string APPROVEMEMO { get; set; }

        /// <summary>
        /// 产品号
        /// </summary>
        public virtual string CPH { get; set; }

        /// <summary>
        /// 批次
        /// </summary>
        public virtual string PC { get; set; }

        /// <summary>
        /// 公告生效日期
        /// </summary>
        public virtual DateTime? GGSXRQ { get; set; }

        /// <summary>
        /// UKEY
        /// </summary>
        public virtual string UKEY { get; set; }

        /// <summary>
        /// VERSION
        /// </summary>
        public virtual string VERSION { get; set; }

        /// <summary>
        /// 纸张编号
        /// </summary>
        public virtual string ZZBH { get; set; }

        /// <summary>
        /// DYWYM
        /// </summary>
        public virtual string DYWYM { get; set; }

        /// <summary>
        /// 打印唯一码
        /// </summary>
        public virtual string DYWYN { get; set; }

        /// <summary>
        /// U盾标识
        /// </summary>
        public virtual string UPSEND_TAG { get; set; }

        /// <summary>
        /// 配置序列号
        /// </summary>
        public virtual string PZXLH { get; set; }

        /// <summary>
        /// 企业其他信息
        /// </summary>
        public virtual string QYQTXX { get; set; }

        /// <summary>
        /// 合格证首次上传时间
        /// </summary>
        public virtual DateTime? FIRSTGETTIME { get; set; }

        /// <summary>
        /// 合格证最后上传时间
        /// </summary>
        public virtual DateTime? LASTGETTIME { get; set; }

        /// <summary>
        /// 合格证打印时间
        /// </summary>
        public virtual DateTime? CZRQ { get; set; }

        /// <summary>
        /// FEEDBACKTIME
        /// </summary>
        public virtual DateTime? FEEDBACKTIME { get; set; }

        /// <summary>
        /// FEEDBACKEMEMO
        /// </summary>
        public virtual string FEEDBACKEMEMO { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CREATETIME { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public virtual DateTime UPDATETIME { get; set; }

        /// <summary>
        /// HD_USER
        /// </summary>
        public virtual string HD_USER { get; set; }

        /// <summary>
        /// 是否免征
        /// </summary>
        public virtual string ZCHGZBH { get; set; }

        /// <summary>
        /// 临时配置序列号
        /// </summary>
        public virtual string LSPZXLH { get; set; }

        /// <summary>
        /// IMPORTFLAG
        /// </summary>
        public virtual string IMPORTFLAG { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        public virtual string HSJE { get; set; }

        /// <summary>
        /// TypeCode 
        /// </summary>
        public virtual string TypeCode { get; set; }

        /// <summary>
        /// 发票编号
        /// </summary>
        public virtual string InvNo { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public virtual string FPLX { get; set; }

        /// <summary>
        /// 发票类型
        /// </summary>
        public virtual string PFLX { get; set; }

        /// <summary>
        /// CLSBDH
        /// </summary>
        public virtual string CLSBDH { get; set; }
        

    }
}
