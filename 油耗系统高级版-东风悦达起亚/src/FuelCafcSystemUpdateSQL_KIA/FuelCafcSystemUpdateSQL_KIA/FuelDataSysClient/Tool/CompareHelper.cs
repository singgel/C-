using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Common;

namespace FuelDataSysClient.Tool
{
    public class CompareHelper
    {
        /// <summary>
        /// 本地数据比对-查找不一致VIN
        /// </summary>
        /// <param name="dtTable_gf">官方数据</param>
        /// <param name="dtTable_sc">生产线数据</param>
        /// <param name="rllx">燃料类型</param>
        /// <returns>参数有差异的VIN列表</returns>
        public static List<string> CompareDataTableDiff(DataTable dtTable_gf, DataTable dtTable_sc, string rllx)
        {
            List<string> vinList = new List<string>();
            switch (rllx)
            {
                case "传统能源":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd") != dd.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                                   //|| d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                               || d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                               || d.Field<string>("额定功率(kW)") != dd.Field<string>("额定功率(kW)")
                               || d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                               || d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                               || d.Field<string>("排量(mL)") != dd.Field<string>("排量(mL)")
                               || d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                               || d.Field<string>("其他燃料信息") != dd.Field<string>("其他燃料信息")
                               || d.Field<string>("市郊工况燃料消耗量(L/100km)") != dd.Field<string>("市郊工况燃料消耗量(L/100km)")
                               || d.Field<string>("市区工况燃料消耗量(L/100km)") != dd.Field<string>("市区工况燃料消耗量(L/100km)")
                               || d.Field<string>("综合工况CO2排放量(g/km)") != dd.Field<string>("综合工况CO2排放量(g/km)")
                               || d.Field<string>("综合工况燃料消耗量(L/100km)") != dd.Field<string>("综合工况燃料消耗量(L/100km)")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "非插电式混合动力":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd") != dd.Field<string>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                                   //|| d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                               || d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                               || d.Field<string>("纯电动模式下1km最高车速") != dd.Field<string>("纯电动模式下1km最高车速")
                               || d.Field<string>("纯电动模式下综合工况续驶里程") != dd.Field<string>("纯电动模式下综合工况续驶里程")
                               || d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                               || d.Field<string>("额定功率") != dd.Field<string>("额定功率")
                               || d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                               || d.Field<string>("混合动力结构型式") != dd.Field<string>("混合动力结构型式")
                               || d.Field<string>("混合动力最大电功率比") != dd.Field<string>("混合动力最大电功率比")
                               || d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                               || d.Field<string>("排量") != dd.Field<string>("排量")
                                   //|| d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                               || d.Field<string>("市郊工况燃料消耗量") != dd.Field<string>("市郊工况燃料消耗量")
                               || d.Field<string>("市区工况燃料消耗量") != dd.Field<string>("市区工况燃料消耗量")
                               || d.Field<string>("是否具有行驶模式手动选择功能") != dd.Field<string>("是否具有行驶模式手动选择功能")
                               || d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                               || d.Field<string>("综合工况CO2排放") != dd.Field<string>("综合工况CO2排放")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "插电式混合动力":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd") != dd.Field<string>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                                   //|| d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                               || d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                               || d.Field<string>("纯电动模式下1km最高车速") != dd.Field<string>("纯电动模式下1km最高车速")
                               || d.Field<string>("纯电动模式下综合工况续驶里程") != dd.Field<string>("纯电动模式下综合工况续驶里程")
                               || d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                               || d.Field<string>("额定功率") != dd.Field<string>("额定功率")
                               || d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                               || d.Field<string>("混合动力结构型式") != dd.Field<string>("混合动力结构型式")
                               || d.Field<string>("混合动力最大电功率比") != dd.Field<string>("混合动力最大电功率比")
                               || d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                               || d.Field<string>("排量") != dd.Field<string>("排量")
                                   //|| d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                               || d.Field<string>("是否具有行驶模式手动选择功能") != dd.Field<string>("是否具有行驶模式手动选择功能")
                               || d.Field<string>("综合工况电能消耗量") != dd.Field<string>("综合工况电能消耗量")
                               || d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                               || d.Field<string>("综合工况CO2排放") != dd.Field<string>("综合工况CO2排放")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "纯电动":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd") != dd.Field<string>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                                   //|| d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("电动汽车30分钟最高车速") != dd.Field<string>("电动汽车30分钟最高车速")
                               || d.Field<string>("动力蓄电池总质量与整车整备质量的比值") != dd.Field<string>("动力蓄电池总质量与整车整备质量的比值")
                               || d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                               || d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("综合工况电能消耗量") != dd.Field<string>("综合工况电能消耗量")
                               || d.Field<string>("综合工况续驶里程") != dd.Field<string>("综合工况续驶里程")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "燃料电池":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd") != dd.Field<string>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                                   //|| d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("电动汽车30分钟最高车速") != dd.Field<string>("电动汽车30分钟最高车速")
                               || d.Field<string>("储氢瓶标称工作压力") != dd.Field<string>("储氢瓶标称工作压力")
                               || d.Field<string>("储氢瓶类型") != dd.Field<string>("储氢瓶类型")
                               || d.Field<string>("储氢瓶容积") != dd.Field<string>("储氢瓶容积")
                               || d.Field<string>("燃料电池堆功率密度") != dd.Field<string>("燃料电池堆功率密度")
                               || d.Field<string>("电电混合技术条件下动力蓄电池组比能量") != dd.Field<string>("电电混合技术条件下动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("燃料电池燃料类型") != dd.Field<string>("燃料电池燃料类型")
                               || d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                               || d.Field<string>("综合工况续驶里程") != dd.Field<string>("综合工况续驶里程")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
            }
            return vinList;
        }

        /// <summary>
        /// 生产线数据比对-查找不一致VIN
        /// </summary>
        /// <param name="dtTable_gf">官方数据</param>
        /// <param name="dtTable_sc">生产线数据</param>
        /// <param name="rllx">燃料类型</param>
        /// <returns>参数字段有差异的VIN列表</returns>
        public static List<string> CompareDataTableDiff_pro(DataTable dtTable_gf, DataTable dtTable_sc, string rllx)
        {
            List<string> vinList = new List<string>();
            switch (rllx)
            {
                case "传统能源":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("反馈码") != dd.Field<string>("反馈码")
                                   //|| d.Field<string>("上报人") != dd.Field<string>("上报人")
                               || d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期") != dd.Field<System.DateTime>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                               || d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                               || d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                               || d.Field<string>("额定功率(kW)") != dd.Field<string>("额定功率(kW)")
                               || d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                               || d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                               || d.Field<string>("排量(mL)") != dd.Field<string>("排量(mL)")
                               || d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                               || d.Field<string>("其他燃料信息") != dd.Field<string>("其他燃料信息")
                               || d.Field<string>("市郊工况燃料消耗量(L/100km)") != dd.Field<string>("市郊工况燃料消耗量(L/100km)")
                               || d.Field<string>("市区工况燃料消耗量(L/100km)") != dd.Field<string>("市区工况燃料消耗量(L/100km)")
                               || d.Field<string>("综合工况CO2排放量(g/km)") != dd.Field<string>("综合工况CO2排放量(g/km)")
                               || d.Field<string>("综合工况燃料消耗量(L/100km)") != dd.Field<string>("综合工况燃料消耗量(L/100km)")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "非插电式混合动力":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("反馈码") != dd.Field<string>("反馈码")
                                   //|| d.Field<string>("上报人") != dd.Field<string>("上报人")
                               || d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期") != dd.Field<System.DateTime>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                               || d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                               || d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                               || d.Field<string>("纯电动模式下1km最高车速") != dd.Field<string>("纯电动模式下1km最高车速")
                               || d.Field<string>("纯电动模式下综合工况续驶里程") != dd.Field<string>("纯电动模式下综合工况续驶里程")
                               || d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                               || d.Field<string>("额定功率") != dd.Field<string>("额定功率")
                               || d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                               || d.Field<string>("混合动力结构型式") != dd.Field<string>("混合动力结构型式")
                               || d.Field<string>("混合动力最大电功率比") != dd.Field<string>("混合动力最大电功率比")
                               || d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                               || d.Field<string>("排量") != dd.Field<string>("排量")
                                   //|| d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                               || d.Field<string>("市郊工况燃料消耗量") != dd.Field<string>("市郊工况燃料消耗量")
                               || d.Field<string>("市区工况燃料消耗量") != dd.Field<string>("市区工况燃料消耗量")
                               || d.Field<string>("是否具有行驶模式手动选择功能") != dd.Field<string>("是否具有行驶模式手动选择功能")
                               || d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                               || d.Field<string>("综合工况CO2排放") != dd.Field<string>("综合工况CO2排放")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "插电式混合动力":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("反馈码") != dd.Field<string>("反馈码")
                                   //|| d.Field<string>("上报人") != dd.Field<string>("上报人")
                               || d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期") != dd.Field<System.DateTime>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                               || d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                               || d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                               || d.Field<string>("纯电动模式下1km最高车速") != dd.Field<string>("纯电动模式下1km最高车速")
                               || d.Field<string>("纯电动模式下综合工况续驶里程") != dd.Field<string>("纯电动模式下综合工况续驶里程")
                               || d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                               || d.Field<string>("额定功率") != dd.Field<string>("额定功率")
                               || d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                               || d.Field<string>("混合动力结构型式") != dd.Field<string>("混合动力结构型式")
                               || d.Field<string>("混合动力最大电功率比") != dd.Field<string>("混合动力最大电功率比")
                               || d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                               || d.Field<string>("排量") != dd.Field<string>("排量")
                                   //|| d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                               || d.Field<string>("是否具有行驶模式手动选择功能") != dd.Field<string>("是否具有行驶模式手动选择功能")
                               || d.Field<string>("综合工况电能消耗量") != dd.Field<string>("综合工况电能消耗量")
                               || d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                               || d.Field<string>("综合工况CO2排放") != dd.Field<string>("综合工况CO2排放")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "纯电动":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("反馈码") != dd.Field<string>("反馈码")
                                   //|| d.Field<string>("上报人") != dd.Field<string>("上报人")
                               || d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期") != dd.Field<System.DateTime>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                               || d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("电动汽车30分钟最高车速") != dd.Field<string>("电动汽车30分钟最高车速")
                               || d.Field<string>("动力蓄电池总质量与整车整备质量的比值") != dd.Field<string>("动力蓄电池总质量与整车整备质量的比值")
                               || d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                               || d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("综合工况电能消耗量") != dd.Field<string>("综合工况电能消耗量")
                               || d.Field<string>("综合工况续驶里程") != dd.Field<string>("综合工况续驶里程")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
                case "燃料电池":
                    vinList = (from d in dtTable_gf.AsEnumerable()
                               join dd in dtTable_sc.AsEnumerable()
                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                               where d.Field<string>("反馈码") != dd.Field<string>("反馈码")
                                   //|| d.Field<string>("上报人") != dd.Field<string>("上报人")
                               || d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                               || d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                               || d.Field<System.DateTime>("车辆制造日期") != dd.Field<System.DateTime>("车辆制造日期")
                               || d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                               || d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                               || d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                               || d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                               || d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                               || d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                               || d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                               || d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                               || d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                               || d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                               || d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                               || d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                               || d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                               || d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                               || d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                               || d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                               || d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                               || d.Field<System.DateTime>("上报时间") != dd.Field<System.DateTime>("上报时间")
                               || d.Field<string>("电动汽车30分钟最高车速") != dd.Field<string>("电动汽车30分钟最高车速")
                               || d.Field<string>("储氢瓶标称工作压力") != dd.Field<string>("储氢瓶标称工作压力")
                               || d.Field<string>("储氢瓶类型") != dd.Field<string>("储氢瓶类型")
                               || d.Field<string>("储氢瓶容积") != dd.Field<string>("储氢瓶容积")
                               || d.Field<string>("燃料电池堆功率密度") != dd.Field<string>("燃料电池堆功率密度")
                               || d.Field<string>("电电混合技术条件下动力蓄电池组比能量") != dd.Field<string>("电电混合技术条件下动力蓄电池组比能量")
                               || d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                               || d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                               || d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                               || d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                               || d.Field<string>("燃料电池燃料类型") != dd.Field<string>("燃料电池燃料类型")
                               || d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                               || d.Field<string>("综合工况续驶里程") != dd.Field<string>("综合工况续驶里程")
                               select new
                               {
                                   VIN = d.Field<string>("VIN")
                               }).ToList().Select(d => d.VIN).ToList<string>();
                    break;
            }
            return vinList;
        }

        /// <summary>
        /// 生产线数据比对-查找不一致VIN和参数
        /// </summary>
        /// <param name="dtTable_gf">官方数据</param>
        /// <param name="dtTable_sc">生产线数据</param>
        /// <param name="rllx">燃料类型</param>
        /// <returns>参数字段有差异的VIN列表</returns>
        public static DataTable CompareDataTableDetail_pro(DataTable dtTable_gf, DataTable dtTable_sc, string rllx)
        {
            DataTable dataTableDetail = new DataTable();
            //var V_ID = from d in dtTable_gf.AsEnumerable()
            //           join dd in dtTable_sc.AsEnumerable()
            //           on d.Field<string>("VIN") equals dd.Field<string>("VIN")
            //           where d.Field<string>("反馈码") != dd.Field<string>("反馈码")
            //           select new
            //           {
            //               VIN = d.Field<string>("VIN"),
            //               差异 = "反馈码",
            //               官方数值 = d.Field<string>("反馈码"),
            //               系统数值 = dd.Field<string>("反馈码")
            //           };
            //var User_ID = from d in dtTable_gf.AsEnumerable()
            //              join dd in dtTable_sc.AsEnumerable()
            //              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
            //              where d.Field<string>("上报人") != dd.Field<string>("上报人")
            //              select new
            //              {
            //                  VIN = d.Field<string>("VIN"),
            //                  差异 = "上报人",
            //                  官方数值 = d.Field<string>("上报人"),
            //                  系统数值 = dd.Field<string>("上报人")
            //              };
            var QCSCQY = from d in dtTable_gf.AsEnumerable()
                         join dd in dtTable_sc.AsEnumerable()
                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                         where d.Field<string>("汽车生产企业") != dd.Field<string>("汽车生产企业")
                         select new
                         {
                             VIN = d.Field<string>("VIN"),
                             差异 = "汽车生产企业",
                             官方数值 = d.Field<string>("汽车生产企业"),
                             系统数值 = dd.Field<string>("汽车生产企业")
                         };
            var JKQCZJXS = from d in dtTable_gf.AsEnumerable()
                           join dd in dtTable_sc.AsEnumerable()
                           on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                           where d.Field<string>("进口汽车总经销商") != dd.Field<string>("进口汽车总经销商")
                           select new
                           {
                               VIN = d.Field<string>("VIN"),
                               差异 = "进口汽车总经销商",
                               官方数值 = d.Field<string>("进口汽车总经销商"),
                               系统数值 = dd.Field<string>("进口汽车总经销商")
                           };
            var CLZZRQ = from d in dtTable_gf.AsEnumerable()
                         join dd in dtTable_sc.AsEnumerable()
                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                         where d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd") != dd.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd")
                         select new
                         {
                             VIN = d.Field<string>("VIN"),
                             差异 = "车辆制造日期",
                             官方数值 = d.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd"),
                             系统数值 = dd.Field<System.DateTime>("车辆制造日期").ToString("yyyy/MM/dd")
                         };
            var CLXH = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("车辆型号") != dd.Field<string>("车辆型号")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "车辆型号",
                           官方数值 = d.Field<string>("车辆型号"),
                           系统数值 = dd.Field<string>("车辆型号")
                       };
            var HGSPBM = from d in dtTable_gf.AsEnumerable()
                         join dd in dtTable_sc.AsEnumerable()
                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                         where d.Field<string>("海关商品编码") != dd.Field<string>("海关商品编码")
                         select new
                         {
                             VIN = d.Field<string>("VIN"),
                             差异 = "海关商品编码",
                             官方数值 = d.Field<string>("海关商品编码"),
                             系统数值 = dd.Field<string>("海关商品编码")
                         };
            var CLZL = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("车辆种类") != dd.Field<string>("车辆种类")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "车辆种类",
                           官方数值 = d.Field<string>("车辆种类"),
                           系统数值 = dd.Field<string>("车辆种类")
                       };
            var YYC = from d in dtTable_gf.AsEnumerable()
                      join dd in dtTable_sc.AsEnumerable()
                      on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                      where d.Field<string>("越野车（G类）") != dd.Field<string>("越野车（G类）")
                      select new
                      {
                          VIN = d.Field<string>("VIN"),
                          差异 = "越野车（G类）",
                          官方数值 = d.Field<string>("越野车（G类）"),
                          系统数值 = dd.Field<string>("越野车（G类）")
                      };
            var QDXS = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("驱动型式") != dd.Field<string>("驱动型式")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "驱动型式",
                           官方数值 = d.Field<string>("驱动型式"),
                           系统数值 = dd.Field<string>("驱动型式")
                       };
            var ZWPS = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("座位排数") != dd.Field<string>("座位排数")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "座位排数",
                           官方数值 = d.Field<string>("座位排数"),
                           系统数值 = dd.Field<string>("座位排数")
                       };
            var ZCZBZL = from d in dtTable_gf.AsEnumerable()
                         join dd in dtTable_sc.AsEnumerable()
                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                         where d.Field<string>("整车整备质量(kg)") != dd.Field<string>("整车整备质量(kg)")
                         select new
                         {
                             VIN = d.Field<string>("VIN"),
                             差异 = "整车整备质量(kg)",
                             官方数值 = d.Field<string>("整车整备质量(kg)"),
                             系统数值 = dd.Field<string>("整车整备质量(kg)")
                         };
            var ZDSJZZL = from d in dtTable_gf.AsEnumerable()
                          join dd in dtTable_sc.AsEnumerable()
                          on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                          where d.Field<string>("最大设计总质量(kg)") != dd.Field<string>("最大设计总质量(kg)")
                          select new
                          {
                              VIN = d.Field<string>("VIN"),
                              差异 = "最大设计总质量(kg)",
                              官方数值 = d.Field<string>("最大设计总质量(kg)"),
                              系统数值 = dd.Field<string>("最大设计总质量(kg)")
                          };
            var JYBGBH = from d in dtTable_gf.AsEnumerable()
                         join dd in dtTable_sc.AsEnumerable()
                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                         where d.Field<string>("报告编号") != dd.Field<string>("报告编号")
                         select new
                         {
                             VIN = d.Field<string>("VIN"),
                             差异 = "报告编号",
                             官方数值 = d.Field<string>("报告编号"),
                             系统数值 = dd.Field<string>("报告编号")
                         };
            var JCJGMC = from d in dtTable_gf.AsEnumerable()
                         join dd in dtTable_sc.AsEnumerable()
                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                         where d.Field<string>("检测机构名称") != dd.Field<string>("检测机构名称")
                         select new
                         {
                             VIN = d.Field<string>("VIN"),
                             差异 = "检测机构名称",
                             官方数值 = d.Field<string>("检测机构名称"),
                             系统数值 = dd.Field<string>("检测机构名称")
                         };
            var TYMC = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("通用名称") != dd.Field<string>("通用名称")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "通用名称",
                           官方数值 = d.Field<string>("通用名称"),
                           系统数值 = dd.Field<string>("通用名称")
                       };
            var ZGCS = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("最高车速(km/h)") != dd.Field<string>("最高车速(km/h)")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "最高车速(km/h)",
                           官方数值 = d.Field<string>("最高车速(km/h)"),
                           系统数值 = dd.Field<string>("最高车速(km/h)")
                       };
            var EDZK = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("额定载客（人）") != dd.Field<string>("额定载客（人）")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "额定载客（人）",
                           官方数值 = d.Field<string>("额定载客（人）"),
                           系统数值 = dd.Field<string>("额定载客（人）")
                       };
            var LTGG = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("轮胎规格") != dd.Field<string>("轮胎规格")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "轮胎规格",
                           官方数值 = d.Field<string>("轮胎规格"),
                           系统数值 = dd.Field<string>("轮胎规格")
                       };
            var LJ = from d in dtTable_gf.AsEnumerable()
                     join dd in dtTable_sc.AsEnumerable()
                     on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                     where d.Field<string>("轮距（前/后）(mm)") != dd.Field<string>("轮距（前/后）(mm)")
                     select new
                     {
                         VIN = d.Field<string>("VIN"),
                         差异 = "轮距（前/后）(mm)",
                         官方数值 = d.Field<string>("轮距（前/后）(mm)"),
                         系统数值 = dd.Field<string>("轮距（前/后）(mm)")
                     };
            var ZJ = from d in dtTable_gf.AsEnumerable()
                     join dd in dtTable_sc.AsEnumerable()
                     on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                     where d.Field<string>("轴距(mm)") != dd.Field<string>("轴距(mm)")
                     select new
                     {
                         VIN = d.Field<string>("VIN"),
                         差异 = "轴距(mm)",
                         官方数值 = d.Field<string>("轴距(mm)"),
                         系统数值 = dd.Field<string>("轴距(mm)")
                     };
            var RLLX = from d in dtTable_gf.AsEnumerable()
                       join dd in dtTable_sc.AsEnumerable()
                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                       where d.Field<string>("燃料类型") != dd.Field<string>("燃料类型")
                       select new
                       {
                           VIN = d.Field<string>("VIN"),
                           差异 = "燃料类型",
                           官方数值 = d.Field<string>("燃料类型"),
                           系统数值 = dd.Field<string>("燃料类型")
                       };
            //var UPDATETIME = from d in dtTable_gf.AsEnumerable()
            //                 join dd in dtTable_sc.AsEnumerable()
            //                 on d.Field<string>("VIN") equals dd.Field<string>("VIN")
            //                 where d.Field<System.DateTime>("上报时间").ToString("yyyy/MM/dd") != dd.Field<System.DateTime>("上报时间").ToString("yyyy/MM/dd")
            //                 select new
            //                 {
            //                     VIN = d.Field<string>("VIN"),
            //                     差异 = "上报时间",
            //                     官方数值 = d.Field<System.DateTime>("上报时间").ToString("yyyy/MM/dd"),
            //                     系统数值 = dd.Field<System.DateTime>("上报时间").ToString("yyyy/MM/dd")
            //                 };
            switch (rllx)
            {
                case "传统能源":
                    var CT_BSQDWS = from d in dtTable_gf.AsEnumerable()
                                    join dd in dtTable_sc.AsEnumerable()
                                    on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                    where d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                                    select new
                                    {
                                        VIN = d.Field<string>("VIN"),
                                        差异 = "变速器档位数",
                                        官方数值 = d.Field<string>("变速器档位数"),
                                        系统数值 = dd.Field<string>("变速器档位数")
                                    };
                    var CT_BSQXS = from d in dtTable_gf.AsEnumerable()
                                   join dd in dtTable_sc.AsEnumerable()
                                   on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                   where d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                                   select new
                                   {
                                       VIN = d.Field<string>("VIN"),
                                       差异 = "变速器型式",
                                       官方数值 = d.Field<string>("变速器型式"),
                                       系统数值 = dd.Field<string>("变速器型式")
                                   };
                    var CT_EDGL = from d in dtTable_gf.AsEnumerable()
                                  join dd in dtTable_sc.AsEnumerable()
                                  on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                  where d.Field<string>("额定功率(kW)") != dd.Field<string>("额定功率(kW)")
                                  select new
                                  {
                                      VIN = d.Field<string>("VIN"),
                                      差异 = "额定功率(kW)",
                                      官方数值 = d.Field<string>("额定功率(kW)"),
                                      系统数值 = dd.Field<string>("额定功率(kW)")
                                  };
                    var CT_FDJXH = from d in dtTable_gf.AsEnumerable()
                                   join dd in dtTable_sc.AsEnumerable()
                                   on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                   where d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                                   select new
                                   {
                                       VIN = d.Field<string>("VIN"),
                                       差异 = "发动机型号",
                                       官方数值 = d.Field<string>("发动机型号"),
                                       系统数值 = dd.Field<string>("发动机型号")
                                   };
                    var CT_JGL = from d in dtTable_gf.AsEnumerable()
                                 join dd in dtTable_sc.AsEnumerable()
                                 on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                 where d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                                 select new
                                 {
                                     VIN = d.Field<string>("VIN"),
                                     差异 = "最大净功率",
                                     官方数值 = d.Field<string>("最大净功率"),
                                     系统数值 = dd.Field<string>("最大净功率")
                                 };
                    var CT_PL = from d in dtTable_gf.AsEnumerable()
                                join dd in dtTable_sc.AsEnumerable()
                                on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                where d.Field<string>("排量(mL)") != dd.Field<string>("排量(mL)")
                                select new
                                {
                                    VIN = d.Field<string>("VIN"),
                                    差异 = "排量(mL)",
                                    官方数值 = d.Field<string>("排量(mL)"),
                                    系统数值 = dd.Field<string>("排量(mL)")
                                };
                    //var CT_QCJNJS = from d in dtTable_gf.AsEnumerable()
                    //                join dd in dtTable_sc.AsEnumerable()
                    //                on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                    //                where d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                    //                select new
                    //                {
                    //                    VIN = d.Field<string>("VIN"),
                    //                    差异 = "汽车节能技术",
                    //                    官方数值 = d.Field<string>("汽车节能技术"),
                    //                    系统数值 = dd.Field<string>("汽车节能技术")
                    //                };
                    var CT_QGS = from d in dtTable_gf.AsEnumerable()
                                 join dd in dtTable_sc.AsEnumerable()
                                 on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                 where d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                                 select new
                                 {
                                     VIN = d.Field<string>("VIN"),
                                     差异 = "气缸数",
                                     官方数值 = d.Field<string>("气缸数"),
                                     系统数值 = dd.Field<string>("气缸数")
                                 };
                    var CT_QTXX = from d in dtTable_gf.AsEnumerable()
                                  join dd in dtTable_sc.AsEnumerable()
                                  on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                  where d.Field<string>("其他燃料信息") != dd.Field<string>("其他燃料信息")
                                  select new
                                  {
                                      VIN = d.Field<string>("VIN"),
                                      差异 = "其他燃料信息",
                                      官方数值 = d.Field<string>("其他燃料信息"),
                                      系统数值 = dd.Field<string>("其他燃料信息")
                                  };
                    var CT_SJGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("市郊工况燃料消耗量(L/100km)") != dd.Field<string>("市郊工况燃料消耗量(L/100km)")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "市郊工况燃料消耗量(L/100km)",
                                           官方数值 = d.Field<string>("市郊工况燃料消耗量(L/100km)"),
                                           系统数值 = dd.Field<string>("市郊工况燃料消耗量(L/100km)")
                                       };
                    var CT_SQGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("市区工况燃料消耗量(L/100km)") != dd.Field<string>("市区工况燃料消耗量(L/100km)")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "市区工况燃料消耗量(L/100km)",
                                           官方数值 = d.Field<string>("市区工况燃料消耗量(L/100km)"),
                                           系统数值 = dd.Field<string>("市区工况燃料消耗量(L/100km)")
                                       };
                    var CT_ZHGKCO2PFL = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("综合工况CO2排放量(g/km)") != dd.Field<string>("综合工况CO2排放量(g/km)")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "综合工况CO2排放量(g/km)",
                                            官方数值 = d.Field<string>("综合工况CO2排放量(g/km)"),
                                            系统数值 = dd.Field<string>("综合工况CO2排放量(g/km)")
                                        };
                    var CT_ZHGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("综合工况燃料消耗量(L/100km)") != dd.Field<string>("综合工况燃料消耗量(L/100km)")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "综合工况燃料消耗量(L/100km)",
                                           官方数值 = d.Field<string>("综合工况燃料消耗量(L/100km)"),
                                           系统数值 = dd.Field<string>("综合工况燃料消耗量(L/100km)")
                                       };
                    dataTableDetail = ObjectReflect.ToDataTable(QCSCQY.Union(JKQCZJXS).Union(CLZZRQ).Union(CLXH).Union(HGSPBM).Union(CLZL).Union(YYC).Union(QDXS).Union(ZWPS).Union(ZCZBZL).Union(ZDSJZZL).Union(JYBGBH).Union(JCJGMC).Union(TYMC).Union(ZGCS).Union(EDZK).Union(LTGG).Union(LJ).Union(ZJ).Union(RLLX).Union(CT_BSQDWS).Union(CT_BSQXS).Union(CT_EDGL).Union(CT_FDJXH).Union(CT_JGL).Union(CT_PL).Union(CT_QGS).Union(CT_QTXX).Union(CT_SJGKRLXHL).Union(CT_SQGKRLXHL).Union(CT_ZHGKCO2PFL).Union(CT_ZHGKRLXHL));
                    break;
                case "非插电式混合动力":
                    var FCDS_HHDL_BSQDWS = from d in dtTable_gf.AsEnumerable()
                                           join dd in dtTable_sc.AsEnumerable()
                                           on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                           where d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                                           select new
                                           {
                                               VIN = d.Field<string>("VIN"),
                                               差异 = "变速器档位数",
                                               官方数值 = d.Field<string>("变速器档位数"),
                                               系统数值 = dd.Field<string>("变速器档位数")
                                           };
                    var FCDS_HHDL_BSQXS = from d in dtTable_gf.AsEnumerable()
                                          join dd in dtTable_sc.AsEnumerable()
                                          on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                          where d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                                          select new
                                          {
                                              VIN = d.Field<string>("VIN"),
                                              差异 = "变速器型式",
                                              官方数值 = d.Field<string>("变速器型式"),
                                              系统数值 = dd.Field<string>("变速器型式")
                                          };
                    var FCDS_HHDL_CDDMSXZGCS = from d in dtTable_gf.AsEnumerable()
                                               join dd in dtTable_sc.AsEnumerable()
                                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                               where d.Field<string>("纯电动模式下1km最高车速") != dd.Field<string>("纯电动模式下1km最高车速")
                                               select new
                                               {
                                                   VIN = d.Field<string>("VIN"),
                                                   差异 = "纯电动模式下1km最高车速",
                                                   官方数值 = d.Field<string>("纯电动模式下1km最高车速"),
                                                   系统数值 = dd.Field<string>("纯电动模式下1km最高车速")
                                               };
                    var FCDS_HHDL_CDDMSXZHGKXSLC = from d in dtTable_gf.AsEnumerable()
                                                   join dd in dtTable_sc.AsEnumerable()
                                                   on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                                   where d.Field<string>("纯电动模式下综合工况续驶里程") != dd.Field<string>("纯电动模式下综合工况续驶里程")
                                                   select new
                                                   {
                                                       VIN = d.Field<string>("VIN"),
                                                       差异 = "纯电动模式下综合工况续驶里程",
                                                       官方数值 = d.Field<string>("纯电动模式下综合工况续驶里程"),
                                                       系统数值 = dd.Field<string>("纯电动模式下综合工况续驶里程")
                                                   };
                    var FCDS_HHDL_DLXDCBNL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "动力蓄电池组比能量",
                                                 官方数值 = d.Field<string>("动力蓄电池组比能量"),
                                                 系统数值 = dd.Field<string>("动力蓄电池组比能量")
                                             };
                    var FCDS_HHDL_DLXDCZBCDY = from d in dtTable_gf.AsEnumerable()
                                               join dd in dtTable_sc.AsEnumerable()
                                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                               where d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                                               select new
                                               {
                                                   VIN = d.Field<string>("VIN"),
                                                   差异 = "动力蓄电池组标称电压",
                                                   官方数值 = d.Field<string>("动力蓄电池组标称电压"),
                                                   系统数值 = dd.Field<string>("动力蓄电池组标称电压")
                                               };
                    var FCDS_HHDL_DLXDCZZL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "动力蓄电池组种类",
                                                 官方数值 = d.Field<string>("动力蓄电池组种类"),
                                                 系统数值 = dd.Field<string>("动力蓄电池组种类")
                                             };
                    var FCDS_HHDL_DLXDCZZNL = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "动力蓄电池组总能量",
                                                  官方数值 = d.Field<string>("动力蓄电池组总能量"),
                                                  系统数值 = dd.Field<string>("动力蓄电池组总能量")
                                              };
                    var FCDS_HHDL_EDGL = from d in dtTable_gf.AsEnumerable()
                                         join dd in dtTable_sc.AsEnumerable()
                                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                         where d.Field<string>("额定功率") != dd.Field<string>("额定功率")
                                         select new
                                         {
                                             VIN = d.Field<string>("VIN"),
                                             差异 = "额定功率",
                                             官方数值 = d.Field<string>("额定功率"),
                                             系统数值 = dd.Field<string>("额定功率")
                                         };
                    var FCDS_HHDL_FDJXH = from d in dtTable_gf.AsEnumerable()
                                          join dd in dtTable_sc.AsEnumerable()
                                          on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                          where d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                                          select new
                                          {
                                              VIN = d.Field<string>("VIN"),
                                              差异 = "发动机型号",
                                              官方数值 = d.Field<string>("发动机型号"),
                                              系统数值 = dd.Field<string>("发动机型号")
                                          };
                    var FCDS_HHDL_HHDLJGXS = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("混合动力结构型式") != dd.Field<string>("混合动力结构型式")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "混合动力结构型式",
                                                 官方数值 = d.Field<string>("混合动力结构型式"),
                                                 系统数值 = dd.Field<string>("混合动力结构型式")
                                             };
                    var FCDS_HHDL_HHDLZDDGLB = from d in dtTable_gf.AsEnumerable()
                                               join dd in dtTable_sc.AsEnumerable()
                                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                               where d.Field<string>("混合动力最大电功率比") != dd.Field<string>("混合动力最大电功率比")
                                               select new
                                               {
                                                   VIN = d.Field<string>("VIN"),
                                                   差异 = "混合动力最大电功率比",
                                                   官方数值 = d.Field<string>("混合动力最大电功率比"),
                                                   系统数值 = dd.Field<string>("混合动力最大电功率比")
                                               };
                    var FCDS_HHDL_JGL = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "最大净功率",
                                            官方数值 = d.Field<string>("最大净功率"),
                                            系统数值 = dd.Field<string>("最大净功率")
                                        };
                    var FCDS_HHDL_PL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("排量") != dd.Field<string>("排量")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "排量",
                                           官方数值 = d.Field<string>("排量"),
                                           系统数值 = dd.Field<string>("排量")
                                       };
                    //var FCDS_HHDL_QCJNJS = from d in dtTable_gf.AsEnumerable()
                    //                       join dd in dtTable_sc.AsEnumerable()
                    //                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                    //                       where d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                    //                       select new
                    //                       {
                    //                           VIN = d.Field<string>("VIN"),
                    //                           差异 = "汽车节能技术",
                    //                           官方数值 = d.Field<string>("汽车节能技术"),
                    //                           系统数值 = dd.Field<string>("汽车节能技术")
                    //                       };
                    var FCDS_HHDL_QDDJEDGL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "驱动电机额定功率",
                                                 官方数值 = d.Field<string>("驱动电机额定功率"),
                                                 系统数值 = dd.Field<string>("驱动电机额定功率")
                                             };
                    var FCDS_HHDL_QDDJFZNJ = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "驱动电机峰值扭矩",
                                                 官方数值 = d.Field<string>("驱动电机峰值扭矩"),
                                                 系统数值 = dd.Field<string>("驱动电机峰值扭矩")
                                             };
                    var FCDS_HHDL_QDDJLX = from d in dtTable_gf.AsEnumerable()
                                           join dd in dtTable_sc.AsEnumerable()
                                           on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                           where d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                                           select new
                                           {
                                               VIN = d.Field<string>("VIN"),
                                               差异 = "驱动电机类型",
                                               官方数值 = d.Field<string>("驱动电机类型"),
                                               系统数值 = dd.Field<string>("驱动电机类型")
                                           };
                    var FCDS_HHDL_QGS = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "气缸数",
                                            官方数值 = d.Field<string>("气缸数"),
                                            系统数值 = dd.Field<string>("气缸数")
                                        };
                    var FCDS_HHDL_SJGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("市郊工况燃料消耗量") != dd.Field<string>("市郊工况燃料消耗量")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "市郊工况燃料消耗量",
                                                  官方数值 = d.Field<string>("市郊工况燃料消耗量"),
                                                  系统数值 = dd.Field<string>("市郊工况燃料消耗量")
                                              };
                    var FCDS_HHDL_SQGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("市区工况燃料消耗量") != dd.Field<string>("市区工况燃料消耗量")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "市区工况燃料消耗量",
                                                  官方数值 = d.Field<string>("市区工况燃料消耗量"),
                                                  系统数值 = dd.Field<string>("市区工况燃料消耗量")
                                              };
                    var FCDS_HHDL_XSMSSDXZGN = from d in dtTable_gf.AsEnumerable()
                                               join dd in dtTable_sc.AsEnumerable()
                                               on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                               where d.Field<string>("是否具有行驶模式手动选择功能") != dd.Field<string>("是否具有行驶模式手动选择功能")
                                               select new
                                               {
                                                   VIN = d.Field<string>("VIN"),
                                                   差异 = "是否具有行驶模式手动选择功能",
                                                   官方数值 = d.Field<string>("是否具有行驶模式手动选择功能"),
                                                   系统数值 = dd.Field<string>("是否具有行驶模式手动选择功能")
                                               };
                    var FCDS_HHDL_ZHGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "综合工况燃料消耗量",
                                                  官方数值 = d.Field<string>("综合工况燃料消耗量"),
                                                  系统数值 = dd.Field<string>("综合工况燃料消耗量")
                                              };
                    var FCDS_HHDL_ZHKGCO2PL = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("综合工况CO2排放") != dd.Field<string>("综合工况CO2排放")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "综合工况CO2排放",
                                                  官方数值 = d.Field<string>("综合工况CO2排放"),
                                                  系统数值 = dd.Field<string>("综合工况CO2排放")
                                              };
                    dataTableDetail = ObjectReflect.ToDataTable(QCSCQY.Union(JKQCZJXS).Union(CLZZRQ).Union(CLXH).Union(HGSPBM).Union(CLZL).Union(YYC).Union(QDXS).Union(ZWPS).Union(ZCZBZL).Union(ZDSJZZL).Union(JYBGBH).Union(JCJGMC).Union(TYMC).Union(ZGCS).Union(EDZK).Union(LTGG).Union(LJ).Union(ZJ).Union(RLLX).Union(FCDS_HHDL_BSQDWS).Union(FCDS_HHDL_BSQXS).Union(FCDS_HHDL_CDDMSXZGCS).Union(FCDS_HHDL_CDDMSXZHGKXSLC).Union(FCDS_HHDL_DLXDCBNL).Union(FCDS_HHDL_DLXDCZBCDY).Union(FCDS_HHDL_DLXDCZZL).Union(FCDS_HHDL_DLXDCZZNL).Union(FCDS_HHDL_EDGL).Union(FCDS_HHDL_FDJXH).Union(FCDS_HHDL_HHDLJGXS).Union(FCDS_HHDL_HHDLZDDGLB).Union(FCDS_HHDL_JGL).Union(FCDS_HHDL_PL).Union(FCDS_HHDL_QDDJEDGL).Union(FCDS_HHDL_QDDJFZNJ).Union(FCDS_HHDL_QDDJLX).Union(FCDS_HHDL_QGS).Union(FCDS_HHDL_SJGKRLXHL).Union(FCDS_HHDL_SQGKRLXHL).Union(FCDS_HHDL_XSMSSDXZGN).Union(FCDS_HHDL_ZHGKRLXHL).Union(FCDS_HHDL_ZHKGCO2PL));
                    break;
                case "插电式混合动力":
                    var CDS_HHDL_BSQDWS = from d in dtTable_gf.AsEnumerable()
                                          join dd in dtTable_sc.AsEnumerable()
                                          on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                          where d.Field<string>("变速器档位数") != dd.Field<string>("变速器档位数")
                                          select new
                                          {
                                              VIN = d.Field<string>("VIN"),
                                              差异 = "变速器档位数",
                                              官方数值 = d.Field<string>("变速器档位数"),
                                              系统数值 = dd.Field<string>("变速器档位数")
                                          };
                    var CDS_HHDL_BSQXS = from d in dtTable_gf.AsEnumerable()
                                         join dd in dtTable_sc.AsEnumerable()
                                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                         where d.Field<string>("变速器型式") != dd.Field<string>("变速器型式")
                                         select new
                                         {
                                             VIN = d.Field<string>("VIN"),
                                             差异 = "变速器型式",
                                             官方数值 = d.Field<string>("变速器型式"),
                                             系统数值 = dd.Field<string>("变速器型式")
                                         };
                    var CDS_HHDL_CDDMSXZGCS = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("纯电动模式下1km最高车速") != dd.Field<string>("纯电动模式下1km最高车速")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "纯电动模式下1km最高车速",
                                                  官方数值 = d.Field<string>("纯电动模式下1km最高车速"),
                                                  系统数值 = dd.Field<string>("纯电动模式下1km最高车速")
                                              };
                    var CDS_HHDL_CDDMSXZHGKXSLC = from d in dtTable_gf.AsEnumerable()
                                                  join dd in dtTable_sc.AsEnumerable()
                                                  on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                                  where d.Field<string>("纯电动模式下综合工况续驶里程") != dd.Field<string>("纯电动模式下综合工况续驶里程")
                                                  select new
                                                  {
                                                      VIN = d.Field<string>("VIN"),
                                                      差异 = "纯电动模式下综合工况续驶里程",
                                                      官方数值 = d.Field<string>("纯电动模式下综合工况续驶里程"),
                                                      系统数值 = dd.Field<string>("纯电动模式下综合工况续驶里程")
                                                  };
                    var CDS_HHDL_DLXDCBNL = from d in dtTable_gf.AsEnumerable()
                                            join dd in dtTable_sc.AsEnumerable()
                                            on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                            where d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                                            select new
                                            {
                                                VIN = d.Field<string>("VIN"),
                                                差异 = "动力蓄电池组比能量",
                                                官方数值 = d.Field<string>("动力蓄电池组比能量"),
                                                系统数值 = dd.Field<string>("动力蓄电池组比能量")
                                            };
                    var CDS_HHDL_DLXDCZBCDY = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "动力蓄电池组标称电压",
                                                  官方数值 = d.Field<string>("动力蓄电池组标称电压"),
                                                  系统数值 = dd.Field<string>("动力蓄电池组标称电压")
                                              };
                    var CDS_HHDL_DLXDCZZL = from d in dtTable_gf.AsEnumerable()
                                            join dd in dtTable_sc.AsEnumerable()
                                            on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                            where d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                                            select new
                                            {
                                                VIN = d.Field<string>("VIN"),
                                                差异 = "动力蓄电池组种类",
                                                官方数值 = d.Field<string>("动力蓄电池组种类"),
                                                系统数值 = dd.Field<string>("动力蓄电池组种类")
                                            };
                    var CDS_HHDL_DLXDCZZNL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "动力蓄电池组总能量",
                                                 官方数值 = d.Field<string>("动力蓄电池组总能量"),
                                                 系统数值 = dd.Field<string>("动力蓄电池组总能量")
                                             };
                    var CDS_HHDL_EDGL = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("额定功率") != dd.Field<string>("额定功率")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "额定功率",
                                            官方数值 = d.Field<string>("额定功率"),
                                            系统数值 = dd.Field<string>("额定功率")
                                        };
                    var CDS_HHDL_FDJXH = from d in dtTable_gf.AsEnumerable()
                                         join dd in dtTable_sc.AsEnumerable()
                                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                         where d.Field<string>("发动机型号") != dd.Field<string>("发动机型号")
                                         select new
                                         {
                                             VIN = d.Field<string>("VIN"),
                                             差异 = "发动机型号",
                                             官方数值 = d.Field<string>("发动机型号"),
                                             系统数值 = dd.Field<string>("发动机型号")
                                         };
                    var CDS_HHDL_HHDLJGXS = from d in dtTable_gf.AsEnumerable()
                                            join dd in dtTable_sc.AsEnumerable()
                                            on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                            where d.Field<string>("混合动力结构型式") != dd.Field<string>("混合动力结构型式")
                                            select new
                                            {
                                                VIN = d.Field<string>("VIN"),
                                                差异 = "混合动力结构型式",
                                                官方数值 = d.Field<string>("混合动力结构型式"),
                                                系统数值 = dd.Field<string>("混合动力结构型式")
                                            };
                    var CDS_HHDL_HHDLZDDGLB = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("混合动力最大电功率比") != dd.Field<string>("混合动力最大电功率比")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "混合动力最大电功率比",
                                                  官方数值 = d.Field<string>("混合动力最大电功率比"),
                                                  系统数值 = dd.Field<string>("混合动力最大电功率比")
                                              };
                    var CDS_HHDL_JGL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("最大净功率") != dd.Field<string>("最大净功率")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "最大净功率",
                                           官方数值 = d.Field<string>("最大净功率"),
                                           系统数值 = dd.Field<string>("最大净功率")
                                       };
                    var CDS_HHDL_PL = from d in dtTable_gf.AsEnumerable()
                                      join dd in dtTable_sc.AsEnumerable()
                                      on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                      where d.Field<string>("排量") != dd.Field<string>("排量")
                                      select new
                                      {
                                          VIN = d.Field<string>("VIN"),
                                          差异 = "排量",
                                          官方数值 = d.Field<string>("排量"),
                                          系统数值 = dd.Field<string>("排量")
                                      };
                    //var CDS_HHDL_QCJNJS = from d in dtTable_gf.AsEnumerable()
                    //                      join dd in dtTable_sc.AsEnumerable()
                    //                      on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                    //                      where d.Field<string>("汽车节能技术") != dd.Field<string>("汽车节能技术")
                    //                      select new
                    //                      {
                    //                          VIN = d.Field<string>("VIN"),
                    //                          差异 = "汽车节能技术",
                    //                          官方数值 = d.Field<string>("汽车节能技术"),
                    //                          系统数值 = dd.Field<string>("汽车节能技术")
                    //                      };
                    var CDS_HHDL_QDDJEDGL = from d in dtTable_gf.AsEnumerable()
                                            join dd in dtTable_sc.AsEnumerable()
                                            on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                            where d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                                            select new
                                            {
                                                VIN = d.Field<string>("VIN"),
                                                差异 = "驱动电机额定功率",
                                                官方数值 = d.Field<string>("驱动电机额定功率"),
                                                系统数值 = dd.Field<string>("驱动电机额定功率")
                                            };
                    var CDS_HHDL_QDDJFZNJ = from d in dtTable_gf.AsEnumerable()
                                            join dd in dtTable_sc.AsEnumerable()
                                            on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                            where d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                                            select new
                                            {
                                                VIN = d.Field<string>("VIN"),
                                                差异 = "驱动电机峰值扭矩",
                                                官方数值 = d.Field<string>("驱动电机峰值扭矩"),
                                                系统数值 = dd.Field<string>("驱动电机峰值扭矩")
                                            };
                    var CDS_HHDL_QDDJLX = from d in dtTable_gf.AsEnumerable()
                                          join dd in dtTable_sc.AsEnumerable()
                                          on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                          where d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                                          select new
                                          {
                                              VIN = d.Field<string>("VIN"),
                                              差异 = "驱动电机类型",
                                              官方数值 = d.Field<string>("驱动电机类型"),
                                              系统数值 = dd.Field<string>("驱动电机类型")
                                          };
                    var CDS_HHDL_QGS = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("气缸数") != dd.Field<string>("气缸数")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "气缸数",
                                           官方数值 = d.Field<string>("气缸数"),
                                           系统数值 = dd.Field<string>("气缸数")
                                       };
                    var CDS_HHDL_XSMSSDXZGN = from d in dtTable_gf.AsEnumerable()
                                              join dd in dtTable_sc.AsEnumerable()
                                              on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                              where d.Field<string>("是否具有行驶模式手动选择功能") != dd.Field<string>("是否具有行驶模式手动选择功能")
                                              select new
                                              {
                                                  VIN = d.Field<string>("VIN"),
                                                  差异 = "是否具有行驶模式手动选择功能",
                                                  官方数值 = d.Field<string>("是否具有行驶模式手动选择功能"),
                                                  系统数值 = dd.Field<string>("是否具有行驶模式手动选择功能")
                                              };
                    var CDS_HHDL_ZHGKDNXHL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("综合工况电能消耗量") != dd.Field<string>("综合工况电能消耗量")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "综合工况电能消耗量",
                                                 官方数值 = d.Field<string>("综合工况电能消耗量"),
                                                 系统数值 = dd.Field<string>("综合工况电能消耗量")
                                             };
                    var CDS_HHDL_ZHGKRLXHL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "综合工况燃料消耗量",
                                                 官方数值 = d.Field<string>("综合工况燃料消耗量"),
                                                 系统数值 = dd.Field<string>("综合工况燃料消耗量")
                                             };
                    var CDS_HHDL_ZHKGCO2PL = from d in dtTable_gf.AsEnumerable()
                                             join dd in dtTable_sc.AsEnumerable()
                                             on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                             where d.Field<string>("综合工况CO2排放") != dd.Field<string>("综合工况CO2排放")
                                             select new
                                             {
                                                 VIN = d.Field<string>("VIN"),
                                                 差异 = "综合工况CO2排放",
                                                 官方数值 = d.Field<string>("综合工况CO2排放"),
                                                 系统数值 = dd.Field<string>("综合工况CO2排放")
                                             };
                    dataTableDetail = ObjectReflect.ToDataTable(QCSCQY.Union(JKQCZJXS).Union(CLZZRQ).Union(CLXH).Union(HGSPBM).Union(CLZL).Union(YYC).Union(QDXS).Union(ZWPS).Union(ZCZBZL).Union(ZDSJZZL).Union(JYBGBH).Union(JCJGMC).Union(TYMC).Union(ZGCS).Union(EDZK).Union(LTGG).Union(LJ).Union(ZJ).Union(RLLX).Union(CDS_HHDL_BSQDWS).Union(CDS_HHDL_BSQXS).Union(CDS_HHDL_CDDMSXZGCS).Union(CDS_HHDL_CDDMSXZHGKXSLC).Union(CDS_HHDL_DLXDCBNL).Union(CDS_HHDL_DLXDCZBCDY).Union(CDS_HHDL_DLXDCZZL).Union(CDS_HHDL_DLXDCZZNL).Union(CDS_HHDL_EDGL).Union(CDS_HHDL_FDJXH).Union(CDS_HHDL_HHDLJGXS).Union(CDS_HHDL_HHDLZDDGLB).Union(CDS_HHDL_JGL).Union(CDS_HHDL_PL).Union(CDS_HHDL_QDDJEDGL).Union(CDS_HHDL_QDDJFZNJ).Union(CDS_HHDL_QDDJLX).Union(CDS_HHDL_QGS).Union(CDS_HHDL_XSMSSDXZGN).Union(CDS_HHDL_ZHGKDNXHL).Union(CDS_HHDL_ZHGKRLXHL).Union(CDS_HHDL_ZHKGCO2PL));
                    break;
                case "纯电动":
                    var CDD_DDQC30FZZGCS = from d in dtTable_gf.AsEnumerable()
                                           join dd in dtTable_sc.AsEnumerable()
                                           on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                           where d.Field<string>("电动汽车30分钟最高车速") != dd.Field<string>("电动汽车30分钟最高车速")
                                           select new
                                           {
                                               VIN = d.Field<string>("VIN"),
                                               差异 = "电动汽车30分钟最高车速",
                                               官方数值 = d.Field<string>("电动汽车30分钟最高车速"),
                                               系统数值 = dd.Field<string>("电动汽车30分钟最高车速")
                                           };
                    var CDD_DDXDCZZLYZCZBZLDBZ = from d in dtTable_gf.AsEnumerable()
                                                 join dd in dtTable_sc.AsEnumerable()
                                                 on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                                 where d.Field<string>("动力蓄电池总质量与整车整备质量的比值") != dd.Field<string>("动力蓄电池总质量与整车整备质量的比值")
                                                 select new
                                                 {
                                                     VIN = d.Field<string>("VIN"),
                                                     差异 = "动力蓄电池总质量与整车整备质量的比值",
                                                     官方数值 = d.Field<string>("动力蓄电池总质量与整车整备质量的比值"),
                                                     系统数值 = dd.Field<string>("动力蓄电池总质量与整车整备质量的比值")
                                                 };
                    var CDD_DLXDCBNL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("动力蓄电池组比能量") != dd.Field<string>("动力蓄电池组比能量")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "动力蓄电池组比能量",
                                           官方数值 = d.Field<string>("动力蓄电池组比能量"),
                                           系统数值 = dd.Field<string>("动力蓄电池组比能量")
                                       };
                    var CDD_DLXDCZBCDY = from d in dtTable_gf.AsEnumerable()
                                         join dd in dtTable_sc.AsEnumerable()
                                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                         where d.Field<string>("动力蓄电池组标称电压") != dd.Field<string>("动力蓄电池组标称电压")
                                         select new
                                         {
                                             VIN = d.Field<string>("VIN"),
                                             差异 = "动力蓄电池组标称电压",
                                             官方数值 = d.Field<string>("动力蓄电池组标称电压"),
                                             系统数值 = dd.Field<string>("动力蓄电池组标称电压")
                                         };
                    var CDD_DLXDCZEDNL = from d in dtTable_gf.AsEnumerable()
                                         join dd in dtTable_sc.AsEnumerable()
                                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                         where d.Field<string>("动力蓄电池组总能量") != dd.Field<string>("动力蓄电池组总能量")
                                         select new
                                         {
                                             VIN = d.Field<string>("VIN"),
                                             差异 = "动力蓄电池组总能量",
                                             官方数值 = d.Field<string>("动力蓄电池组总能量"),
                                             系统数值 = dd.Field<string>("动力蓄电池组总能量")
                                         };
                    var CDD_DLXDCZZL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "动力蓄电池组种类",
                                           官方数值 = d.Field<string>("动力蓄电池组种类"),
                                           系统数值 = dd.Field<string>("动力蓄电池组种类")
                                       };
                    var CDD_QDDJEDGL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "驱动电机额定功率",
                                           官方数值 = d.Field<string>("驱动电机额定功率"),
                                           系统数值 = dd.Field<string>("驱动电机额定功率")
                                       };
                    var CDD_QDDJFZNJ = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "驱动电机峰值扭矩",
                                           官方数值 = d.Field<string>("驱动电机峰值扭矩"),
                                           系统数值 = dd.Field<string>("驱动电机峰值扭矩")
                                       };
                    var CDD_QDDJLX = from d in dtTable_gf.AsEnumerable()
                                     join dd in dtTable_sc.AsEnumerable()
                                     on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                     where d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                                     select new
                                     {
                                         VIN = d.Field<string>("VIN"),
                                         差异 = "驱动电机类型",
                                         官方数值 = d.Field<string>("驱动电机类型"),
                                         系统数值 = dd.Field<string>("驱动电机类型")
                                     };
                    var CDD_ZHGKDNXHL = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("综合工况电能消耗量") != dd.Field<string>("综合工况电能消耗量")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "综合工况电能消耗量",
                                            官方数值 = d.Field<string>("综合工况电能消耗量"),
                                            系统数值 = dd.Field<string>("综合工况电能消耗量")
                                        };
                    var CDD_ZHGKXSLC = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("综合工况续驶里程") != dd.Field<string>("综合工况续驶里程")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "综合工况续驶里程",
                                           官方数值 = d.Field<string>("综合工况续驶里程"),
                                           系统数值 = dd.Field<string>("综合工况续驶里程")
                                       };
                    dataTableDetail = ObjectReflect.ToDataTable(QCSCQY.Union(JKQCZJXS).Union(CLZZRQ).Union(CLXH).Union(HGSPBM).Union(CLZL).Union(YYC).Union(QDXS).Union(ZWPS).Union(ZCZBZL).Union(ZDSJZZL).Union(JYBGBH).Union(JCJGMC).Union(TYMC).Union(ZGCS).Union(EDZK).Union(LTGG).Union(LJ).Union(ZJ).Union(RLLX).Union(CDD_DDQC30FZZGCS).Union(CDD_DDXDCZZLYZCZBZLDBZ).Union(CDD_DLXDCBNL).Union(CDD_DLXDCZBCDY).Union(CDD_DLXDCZEDNL).Union(CDD_DLXDCZZL).Union(CDD_QDDJEDGL).Union(CDD_QDDJFZNJ).Union(CDD_QDDJLX).Union(CDD_ZHGKDNXHL).Union(CDD_ZHGKXSLC));
                    break;
                case "燃料电池":
                    var RLDC_CDDMSXZGXSCS = from d in dtTable_gf.AsEnumerable()
                                            join dd in dtTable_sc.AsEnumerable()
                                            on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                            where d.Field<string>("电动汽车30分钟最高车速") != dd.Field<string>("电动汽车30分钟最高车速")
                                            select new
                                            {
                                                VIN = d.Field<string>("VIN"),
                                                差异 = "电动汽车30分钟最高车速",
                                                官方数值 = d.Field<string>("电动汽车30分钟最高车速"),
                                                系统数值 = dd.Field<string>("电动汽车30分钟最高车速")
                                            };
                    var RLDC_CQPBCGZYL = from d in dtTable_gf.AsEnumerable()
                                         join dd in dtTable_sc.AsEnumerable()
                                         on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                         where d.Field<string>("储氢瓶标称工作压力") != dd.Field<string>("储氢瓶标称工作压力")
                                         select new
                                         {
                                             VIN = d.Field<string>("VIN"),
                                             差异 = "储氢瓶标称工作压力",
                                             官方数值 = d.Field<string>("储氢瓶标称工作压力"),
                                             系统数值 = dd.Field<string>("储氢瓶标称工作压力")
                                         };
                    var RLDC_CQPLX = from d in dtTable_gf.AsEnumerable()
                                     join dd in dtTable_sc.AsEnumerable()
                                     on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                     where d.Field<string>("储氢瓶类型") != dd.Field<string>("储氢瓶类型")
                                     select new
                                     {
                                         VIN = d.Field<string>("VIN"),
                                         差异 = "储氢瓶类型",
                                         官方数值 = d.Field<string>("储氢瓶类型"),
                                         系统数值 = dd.Field<string>("储氢瓶类型")
                                     };
                    var RLDC_CQPRJ = from d in dtTable_gf.AsEnumerable()
                                     join dd in dtTable_sc.AsEnumerable()
                                     on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                     where d.Field<string>("储氢瓶容积") != dd.Field<string>("储氢瓶容积")
                                     select new
                                     {
                                         VIN = d.Field<string>("VIN"),
                                         差异 = "储氢瓶容积",
                                         官方数值 = d.Field<string>("储氢瓶容积"),
                                         系统数值 = dd.Field<string>("储氢瓶容积")
                                     };
                    var RLDC_DDGLMD = from d in dtTable_gf.AsEnumerable()
                                      join dd in dtTable_sc.AsEnumerable()
                                      on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                      where d.Field<string>("燃料电池堆功率密度") != dd.Field<string>("燃料电池堆功率密度")
                                      select new
                                      {
                                          VIN = d.Field<string>("VIN"),
                                          差异 = "燃料电池堆功率密度",
                                          官方数值 = d.Field<string>("燃料电池堆功率密度"),
                                          系统数值 = dd.Field<string>("燃料电池堆功率密度")
                                      };
                    var RLDC_DDHHJSTJXXDCZBNL = from d in dtTable_gf.AsEnumerable()
                                                join dd in dtTable_sc.AsEnumerable()
                                                on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                                where d.Field<string>("电电混合技术条件下动力蓄电池组比能量") != dd.Field<string>("电电混合技术条件下动力蓄电池组比能量")
                                                select new
                                                {
                                                    VIN = d.Field<string>("VIN"),
                                                    差异 = "电电混合技术条件下动力蓄电池组比能量",
                                                    官方数值 = d.Field<string>("电电混合技术条件下动力蓄电池组比能量"),
                                                    系统数值 = dd.Field<string>("电电混合技术条件下动力蓄电池组比能量")
                                                };
                    var RLDC_DLXDCZZL = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("动力蓄电池组种类") != dd.Field<string>("动力蓄电池组种类")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "动力蓄电池组种类",
                                            官方数值 = d.Field<string>("动力蓄电池组种类"),
                                            系统数值 = dd.Field<string>("动力蓄电池组种类")
                                        };
                    var RLDC_QDDJEDGL = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("驱动电机额定功率") != dd.Field<string>("驱动电机额定功率")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "驱动电机额定功率",
                                            官方数值 = d.Field<string>("驱动电机额定功率"),
                                            系统数值 = dd.Field<string>("驱动电机额定功率")
                                        };
                    var RLDC_QDDJFZNJ = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("驱动电机峰值扭矩") != dd.Field<string>("驱动电机峰值扭矩")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "驱动电机峰值扭矩",
                                            官方数值 = d.Field<string>("驱动电机峰值扭矩"),
                                            系统数值 = dd.Field<string>("驱动电机峰值扭矩")
                                        };
                    var RLDC_QDDJLX = from d in dtTable_gf.AsEnumerable()
                                      join dd in dtTable_sc.AsEnumerable()
                                      on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                      where d.Field<string>("驱动电机类型") != dd.Field<string>("驱动电机类型")
                                      select new
                                      {
                                          VIN = d.Field<string>("VIN"),
                                          差异 = "驱动电机类型",
                                          官方数值 = d.Field<string>("驱动电机类型"),
                                          系统数值 = dd.Field<string>("驱动电机类型")
                                      };
                    var RLDC_RLLX = from d in dtTable_gf.AsEnumerable()
                                    join dd in dtTable_sc.AsEnumerable()
                                    on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                    where d.Field<string>("燃料电池燃料类型") != dd.Field<string>("燃料电池燃料类型")
                                    select new
                                    {
                                        VIN = d.Field<string>("VIN"),
                                        差异 = "燃料电池燃料类型",
                                        官方数值 = d.Field<string>("燃料电池燃料类型"),
                                        系统数值 = dd.Field<string>("燃料电池燃料类型")
                                    };
                    var RLDC_ZHGKHQL = from d in dtTable_gf.AsEnumerable()
                                       join dd in dtTable_sc.AsEnumerable()
                                       on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                       where d.Field<string>("综合工况燃料消耗量") != dd.Field<string>("综合工况燃料消耗量")
                                       select new
                                       {
                                           VIN = d.Field<string>("VIN"),
                                           差异 = "综合工况燃料消耗量",
                                           官方数值 = d.Field<string>("综合工况燃料消耗量"),
                                           系统数值 = dd.Field<string>("综合工况燃料消耗量")
                                       };
                    var RLDC_ZHGKXSLC = from d in dtTable_gf.AsEnumerable()
                                        join dd in dtTable_sc.AsEnumerable()
                                        on d.Field<string>("VIN") equals dd.Field<string>("VIN")
                                        where d.Field<string>("综合工况续驶里程") != dd.Field<string>("综合工况续驶里程")
                                        select new
                                        {
                                            VIN = d.Field<string>("VIN"),
                                            差异 = "综合工况续驶里程",
                                            官方数值 = d.Field<string>("综合工况续驶里程"),
                                            系统数值 = dd.Field<string>("综合工况续驶里程")
                                        };
                    dataTableDetail = ObjectReflect.ToDataTable(QCSCQY.Union(JKQCZJXS).Union(CLZZRQ).Union(CLXH).Union(HGSPBM).Union(CLZL).Union(YYC).Union(QDXS).Union(ZWPS).Union(ZCZBZL).Union(ZDSJZZL).Union(JYBGBH).Union(JCJGMC).Union(TYMC).Union(ZGCS).Union(EDZK).Union(LTGG).Union(LJ).Union(ZJ).Union(RLLX).Union(RLDC_CDDMSXZGXSCS).Union(RLDC_CQPBCGZYL).Union(RLDC_CQPLX).Union(RLDC_CQPRJ).Union(RLDC_DDGLMD).Union(RLDC_DDHHJSTJXXDCZBNL).Union(RLDC_DLXDCZZL).Union(RLDC_QDDJEDGL).Union(RLDC_QDDJFZNJ).Union(RLDC_QDDJLX).Union(RLDC_RLLX).Union(RLDC_ZHGKHQL).Union(RLDC_ZHGKXSLC));
                    break;
            }
            return dataTableDetail;
        }
    }
}
