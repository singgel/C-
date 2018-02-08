using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using System.Data;
using System.Windows.Forms;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using Catarc.Adc.NewEnergyAccountSys.Common;
using DevExpress.XtraSplashScreen;
using Catarc.Adc.NewEnergyAccountSys.DevForm;
using System.Linq;
using System.Data;
using Catarc.Adc.NewEnergyAccountSys.PopForm;
namespace Catarc.Adc.NewEnergyAccountSys.FormUtils.DataUtils
{
    public class ExportNewInfoUtils
    {
        private readonly string strMinDate = Settings.Default.ClearYear;
        private readonly string strManufacturer = Settings.Default.Vehicle_MFCS;
        private readonly string countTemplatePath = Utils.installPath + "\\Template\\汇总模板.xlsx";
        private readonly string messageTemplatePath = Utils.installPath + "\\Template\\明细模板.xlsx";
        private readonly string runTemplatePath = Utils.installPath + "\\Template\\车辆运行模板.xlsx";
        private readonly string userTemplatePath = Utils.installPath + "\\Template\\联系人模板.xlsx";
        
        /// <summary>
        /// 导出模板
        /// </summary>
        /// <param name="filePath">导出路径</param>
        public string  ExportNewTemplate(string filePath, string selectedParamEntityIds)
        {
            string msg = string.Empty;
            try
            {
                msg += this.VerifyData(selectedParamEntityIds);
                if (string.IsNullOrEmpty(msg))
                {
                    // 获取用户选择的文件夹路径
                    string folderPath = String.Format(@"{0}\{1}{2:yyyyMMddHHmmss}", filePath, Settings.Default.Vehicle_MFCS, DateTime.Now);
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    Annex1_Export(folderPath, selectedParamEntityIds);
                    Annex2_Export(folderPath, selectedParamEntityIds);
                    Annex3_Export(folderPath, selectedParamEntityIds);
                    Annex4_Export(folderPath, selectedParamEntityIds);
                    Annex5_Export(folderPath, selectedParamEntityIds);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return msg;
        }
        //验证VIN，CLXZ，是否为空GGPC
        private string VerifyData(string selectedParamEntityIds)
        {
            string msg = string.Empty;
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat(" SELECT *");
            sqlStr.AppendFormat(" FROM INFOMATION_ENTITIES ");
            sqlStr.AppendFormat(" WHERE JZNF = '{0}' ", Settings.Default.ClearYear);
            if (!string.IsNullOrEmpty(selectedParamEntityIds))
            {
                sqlStr.AppendFormat(" AND VIN in ({0}) ", selectedParamEntityIds);
            }
            sqlStr.Append(" order by VIN desc ");
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStr.ToString(), null);
            DataTable dt =  ds.Tables[0];
            if (dt != null && ds.Tables[0].Rows.Count > 0)
            {
                Dictionary<string, string> mapData = new Dictionary<string, string>();
                string vin = "";
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    vin = dr["VIN"].ToString();
                    mapData.Clear();
                    for (int i = 0; i < dr.ItemArray.Count();i++ )
                    {
                        mapData.Add(dt.Columns[i].ColumnName, dr.ItemArray[i].ToString());
                    }
                    string msgdata = ValidateParam.CheckParameter(mapData);
                    if (msgdata != string.Empty)
                    {
                        msg += String.Format("vin：{0} 数据错误：{1}{2}", vin, Environment.NewLine, msgdata);
                    }

                }
                /*
                string vin = string.Empty;
                var Values = dt.AsEnumerable().Where(d => string.IsNullOrEmpty(d.Field<string>("CLXZ")) || string.IsNullOrEmpty(d.Field<string>("CLXZ"))).Select(d => d.Field<string>("VIN")).ToArray();
                vin = string.Join(",", Values);
                if (!string.IsNullOrEmpty(vin))
                {
                    msg = String.Format("VIN为{0}中的车辆性质或公告批次不能为空", vin);
                }
                else
                {
                    msg = string.Empty;
                }*/
            }
            else
            {
                msg = "不存在导出数据";
            }
            return msg;
        }
        //附件1
        private void Annex1_Export(string path, string selectedParamEntityIds)
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.AppendFormat(" SELECT COUNT(*),GCSF, GCCS,");
            sqlStr.AppendFormat(" CLYXDW, CLXH, SQBZBZ, GGPC,");
            sqlStr.AppendFormat(" COUNT(*) AS NUMCOUNT, COUNT(1)*SQBZBZ AS SUMCOUNT");
            sqlStr.AppendFormat(" FROM INFOMATION_ENTITIES ");
            sqlStr.AppendFormat(" WHERE JZNF = '{0}' ",Settings.Default.ClearYear);
            if (!string.IsNullOrEmpty(selectedParamEntityIds))
            {
                sqlStr.AppendFormat(" AND VIN in ({0}) ", selectedParamEntityIds);
            }
            sqlStr.AppendFormat(" GROUP BY GCSF, GCCS,CLYXDW, CLXH, SQBZBZ, GGPC");
            sqlStr.AppendFormat(" ORDER BY GCSF, GCCS,CLYXDW, CLXH, SQBZBZ, GGPC");
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sqlStr.ToString(), null);

            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dataTable1 = new DataTable();
                    if (!string.IsNullOrEmpty(selectedParamEntityIds))
                    {
                        dataTable1 = AccessHelper.ExecuteDataSet(AccessHelper.conn, string.Format("SELECT COUNT(1),SUM(SQBZBZ) FROM INFOMATION_ENTITIES WHERE JZNF = '{0}' AND VIN IN  ({1})", Settings.Default.ClearYear, selectedParamEntityIds), null).Tables[0];
                    }
                    else
                    {
                        dataTable1 = AccessHelper.ExecuteDataSet(AccessHelper.conn, string.Format("SELECT COUNT(1),SUM(SQBZBZ) FROM INFOMATION_ENTITIES WHERE JZNF = '{0}' ", Settings.Default.ClearYear), null).Tables[0];
                    }
                    int countNum = int.Parse(dataTable1.Rows[0][0].ToString());
                    double countSum = double.Parse(dataTable1.Rows[0][1].ToString());
                    //Common.Tool.DataTabletoCountExcel(ds.Tables[0], countTemplatePath, String.Format(@"{0}\附件1：{1}推广应用车辆补助资金清算信息汇总表.xlsx", path, strManufacturer), strManufacturer, strMinDate, countNum, countSum);
                    Common.NPOITool.DataTabletoCountExcel(ds.Tables[0], countTemplatePath, String.Format(@"{0}\附件1：{1}推广应用车辆补助资金清算信息汇总表.xlsx", path, strManufacturer), strManufacturer, strMinDate, countNum, countSum);

                }
                else
                {
                   //MessageBox.Show("附件1不存在导出数据！", "消息", MessageBoxButtons.OK);
                }
            }

        }

        //附件2
        private void Annex2_Export(string path,string selectedParamEntityIds)
        {
            DataTable dt = QueryMessageData(selectedParamEntityIds);
            //Common.Tool.DataTabletoMessageExcel(dt, this.messageTemplatePath, String.Format(@"{0}\附件2：{1}推广应用车辆补助资金清算信息明细表.xlsx", path, this.strManufacturer), this.strManufacturer, strMinDate);
            //NPOI 
            Common.NPOITool.DataTabletoMessageExcel(dt, this.messageTemplatePath, String.Format(@"{0}\附件2：{1}推广应用车辆补助资金清算信息明细表.xlsx", path, this.strManufacturer), this.strManufacturer, strMinDate);

        }
        private DataTable QueryMessageData(string selectedParamEntityIds)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat(" select '1' as 序号");
                stringBuilder.AppendFormat(" ,CLXZ as 车辆性质");
                stringBuilder.AppendFormat(" ,GCCS as 购车城市");
                stringBuilder.AppendFormat(" ,CLYXDW as 车辆运行单位");
                stringBuilder.AppendFormat(" ,CLZL as 车辆种类");
                stringBuilder.AppendFormat(" ,CLYT as 车辆用途");
                stringBuilder.AppendFormat(" ,CLXH as 车辆型号");
                stringBuilder.AppendFormat(" ,EKGZ as Ekg值");
                stringBuilder.AppendFormat(" ,GGPC as 公告批次");
                stringBuilder.AppendFormat(" ,VIN as 车辆识别代码（VIN）");
                stringBuilder.AppendFormat(" ,CLPZ as 车辆牌照");
                stringBuilder.AppendFormat(" ,SQBZBZ as 申请补助标准");
                stringBuilder.AppendFormat(" ,GMJG as 购买价格");
                stringBuilder.AppendFormat(" ,FPHM as 发票号");
                stringBuilder.AppendFormat(" ,FPSJ as 发票时间");
                stringBuilder.AppendFormat(" ,XSZSJ as 行驶证时间");

                stringBuilder.AppendFormat(" ,DCDTXX_XH as 电池单体型号");
                stringBuilder.AppendFormat(" ,DCDTXX_SCQY as 电池单体生产企业");
                stringBuilder.AppendFormat(" ,DCZXX_XH as 电池成箱型号");
                stringBuilder.AppendFormat(" ,DCZXX_ZRL as 电池组总容量（kWh）");
                stringBuilder.AppendFormat(" ,DCZXX_SCQY as 电池组生产企业");
                stringBuilder.AppendFormat(" ,DCZXX_XTJG as 电池组系统价格（万元）");
                stringBuilder.AppendFormat(" ,DCZXX_ZBNX as 电池组质保年限（年）");


                stringBuilder.AppendFormat(" ,CLSFYCJDR as 车辆是否有超级电容");
                stringBuilder.AppendFormat(" ,CJDRXX_DTXH as 电容单体型号");
                stringBuilder.AppendFormat(" ,CJDRXX_DTSCQY as 电容单体生产企业");
                stringBuilder.AppendFormat(" ,CJDRXX_CXXH as 电容成箱型号");
                stringBuilder.AppendFormat(" ,CJDRXX_DRZRL as 电容总容量（kWh）");
                stringBuilder.AppendFormat(" ,CJDRXX_DRZSCQY as 电容成组生产企业");
                stringBuilder.AppendFormat(" ,CJDRXX_XTJG as 电容系统价格（万元）");
                stringBuilder.AppendFormat(" ,CJDRXX_ZBNX as 电容质保年限（年）");

                stringBuilder.AppendFormat(" ,QDDJXX_XH_1 as 驱动电机型号1");
                stringBuilder.AppendFormat(" ,QDDJXX_EDGL_1 as 电机额定功率1（kW）");
                stringBuilder.AppendFormat(" ,QDDJXX_SCQY_1 as 电机生产企业1");
                stringBuilder.AppendFormat(" ,QDDJXX_XTJG_1 as 电机系统价格1（万元）");

                stringBuilder.AppendFormat(" ,QDDJXX_XH_2 as 驱动电机型号2");
                stringBuilder.AppendFormat(" ,QDDJXX_EDGL_2 as 电机额定功率2（kW）");
                stringBuilder.AppendFormat(" ,QDDJXX_SCQY_2 as 电机生产企业2");
                stringBuilder.AppendFormat(" ,QDDJXX_XTJG_2 as 电机系统价格2（万元）");

                stringBuilder.AppendFormat(" ,CLSFYRLDC as 车辆是否有燃料电池");
                stringBuilder.AppendFormat(" ,RLDCXX_XH as 燃料电池型号");
                stringBuilder.AppendFormat(" ,RLDCXX_EDGL as 燃料电池额定功率");
                stringBuilder.AppendFormat(" ,RLDCXX_SCQY as 燃料电池生产企业");
                stringBuilder.AppendFormat(" ,RLDCXX_GMJG as 燃料电池系统价格");
                stringBuilder.AppendFormat(" ,RLDCXX_ZBNX as 燃料电池质保年限（年）");

                stringBuilder.AppendFormat(" from INFOMATION_ENTITIES ");
                stringBuilder.AppendFormat(" WHERE JZNF = '{0}' ", Settings.Default.ClearYear);
                if (!string.IsNullOrEmpty(selectedParamEntityIds))
                {
                    stringBuilder.AppendFormat(" AND VIN in ({0}) ", selectedParamEntityIds);
                }
                stringBuilder.AppendFormat(" order by GCCS ");
                return AccessHelper.ExecuteDataSet(AccessHelper.conn, stringBuilder.ToString(), null).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //附件3
        private void Annex3_Export(string path, string selectedParamEntityIds)
        {
            DataTable dt = QueryRunData(selectedParamEntityIds);
            //Common.Tool.DataTabletoRunExcel(dt, this.runTemplatePath, String.Format(@"{0}\附件3：{1}推广应用车辆运行情况表.xlsx", path, this.strManufacturer), this.strManufacturer, strMinDate);
            //NPOI
            Common.NPOITool.DataTabletoRunExcel(dt, this.runTemplatePath, String.Format(@"{0}\附件3：{1}推广应用车辆运行情况表.xlsx", path, this.strManufacturer), this.strManufacturer, strMinDate);

        }
        public DataTable QueryRunData( string selectedParamEntityIds)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat(" select '1' as 序号");
                stringBuilder.AppendFormat(" ,GCCS as 购车城市");
                stringBuilder.AppendFormat(" ,CLZL as 车辆种类");
                stringBuilder.AppendFormat(" ,CLYT as 车辆用途");
                stringBuilder.AppendFormat(" ,CLPZ as 车辆牌照");

                stringBuilder.AppendFormat(" ,CLCMYCDNGXSLC as 车辆充满一次电能够行驶里程（公里）");
                stringBuilder.AppendFormat(" ,CLYCCMDSXSJ as  车辆一次充满电所需时间（h）");
                stringBuilder.AppendFormat(" ,ZDCDGL as 最大充电功率（kW）");
                stringBuilder.AppendFormat(" ,LJXSLC as [累计行驶里程*（公里）]");
                stringBuilder.AppendFormat(" ,YJXSLC as 月均行驶里程（公里）");
                stringBuilder.AppendFormat(" ,BGLHDL  as  [百公里耗电量（kWh/100km）]");
                stringBuilder.AppendFormat(" ,PJDRXYSJ as 平均单日运行时间（h）");

                stringBuilder.AppendFormat(" ,LJJYL as 加油量（L）");
                stringBuilder.AppendFormat(" ,LJJQL_G as 加气量（kg）");
                stringBuilder.AppendFormat(" ,LJJQL_L as 加气量（L）");
                stringBuilder.AppendFormat(" ,LJJQL as 加氢量（kg）");
                stringBuilder.AppendFormat(" ,LJCDL as 充电量（kWh）");
                stringBuilder.AppendFormat(" ,SFAZJKZZ as 是否安装监控装置");
                stringBuilder.AppendFormat(" ,JKPDXXDW as 监控平台运行单位");

                stringBuilder.AppendFormat(" from INFOMATION_ENTITIES ");
                stringBuilder.AppendFormat(" WHERE JZNF = '{0}' ", Settings.Default.ClearYear);
                if (!string.IsNullOrEmpty(selectedParamEntityIds))
                {
                    stringBuilder.AppendFormat(" AND VIN in ({0}) ", selectedParamEntityIds);
                }
                stringBuilder.AppendFormat(" order by GCCS ");
                return AccessHelper.ExecuteDataSet(AccessHelper.conn, stringBuilder.ToString(), null).Tables[0];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //附件4
        private void Annex4_Export(string path, string selectedParamEntityIds)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from CONTRACT_User where AutoFill_Manufacturer='{0}'", strManufacturer);
            DataTable dt = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql.ToString(), null).Tables[0];
            ProgressBar pb = new ProgressBar();
            //Common.Tool.DataTabletoUserExcel(dt, userTemplatePath, String.Format(@"{0}\附件4：{1}新能源汽车推广应用车辆生产企业联络人.xlsx", path, this.strManufacturer), strManufacturer);
            //NPOI
            Common.NPOITool.DataTabletoUserExcel(dt, userTemplatePath, String.Format(@"{0}\附件4：{1}新能源汽车推广应用车辆生产企业联络人.xlsx", path, this.strManufacturer), strManufacturer);

        }

        //附件5
        private void Annex5_Export(string path, string selectedParamEntityIds)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("select * from CONTRACT_User where AutoFill_Manufacturer='{0}'", strManufacturer);
            DataTable dt = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql.ToString(), null).Tables[0];
            ExportPDF(path + @"\", dt, selectedParamEntityIds);
        }
        private void ExportPDF(string ExportPath, DataTable dtUser, string selectedParamEntityIds)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (string.IsNullOrEmpty(selectedParamEntityIds))
                {
                    stringBuilder.AppendFormat("SELECT FPTP,XSZTP,XSZSJ,VIN,CLPZ,FPHM FROM INFOMATION_ENTITIES WHERE JZNF = '{0}' ", Settings.Default.ClearYear);
                }
                else 
                {
                    stringBuilder.AppendFormat("SELECT FPTP,XSZTP,XSZSJ,VIN,CLPZ,FPHM FROM INFOMATION_ENTITIES WHERE JZNF = '{0}' AND VIN IN ({1}) ", Settings.Default.ClearYear, selectedParamEntityIds);
                }
                DataTable dataTable = AccessHelper.ExecuteDataSet(AccessHelper.conn, stringBuilder.ToString(), null).Tables[0];
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter instance1 = PdfWriter.GetInstance(document, (Stream)new FileStream(ExportPath + "\\附件5：推广应用车辆补助资金清算信息.PDF", FileMode.CreateNew));
                document.Open();
                BaseFont font1 = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", "Identity-H", false);
                iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 24f, 1, new iTextSharp.text.Color(0, 0, 0));
                Paragraph paragraph1 = new Paragraph(" ", font2);
                document.Add((IElement)new Paragraph("附件5：", font2)
                {
                    Alignment = 0
                });
                Paragraph paragraph2 = new Paragraph(" ", font2);
                paragraph2.Alignment = 1;
                paragraph2.SetLeading(20f, 2f);
                document.Add((IElement)paragraph2);
                document.Add((IElement)new Paragraph(strMinDate + "年度推广应用车辆补助资金清算信息", font2)
                {
                    Alignment = 1
                });
                Paragraph paragraph3 = new Paragraph(" ", font2);
                paragraph3.Alignment = 1;
                paragraph3.SetLeading(20f, 3f);
                document.Add((IElement)paragraph3);
                document.Add((IElement)new Paragraph("企业名称（盖章）：" + this.strManufacturer, font2)
                {
                    Alignment = 1
                });
                if (dtUser.Rows.Count > 0)
                {
                    iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 16f, 1, new iTextSharp.text.Color(0, 0, 0));
                    Paragraph paragraph4 = new Paragraph(" ", font3);
                    paragraph4.Alignment = 1;
                    paragraph4.SetLeading(20f, 6f);
                    document.Add((IElement)paragraph4);
                    document.Add((IElement)new Paragraph("填报人姓名：" + dtUser.Rows[0]["Contact_Name"].ToString(), font3)
                    {
                        Alignment = 1
                    });
                    document.Add((IElement)new Paragraph("电话：" + dtUser.Rows[0]["Contact_Tel"].ToString(), font3)
                    {
                        Alignment = 1
                    });
                    document.Add((IElement)new Paragraph("手机：" + dtUser.Rows[0]["Contact_Phone"].ToString(), font3)
                    {
                        Alignment = 1
                    });
                    document.Add((IElement)new Paragraph("导出日期：" + DateTime.Now.ToString("yyyy年MM月dd日"), font3)
                    {
                        Alignment = 1
                    });
                }
                for (int index = 0; index < dataTable.Rows.Count; ++index)
                {
                    //string str1 = dataTable.Rows[index]["ID"].ToString();
                    string str2 = dataTable.Rows[index]["VIN"].ToString();
                    string str3 = dataTable.Rows[index]["FPHM"].ToString();
                    string str4 = dataTable.Rows[index]["CLPZ"].ToString();
                    string str5 = String.Format("{0}\\{1}", Utils.billImage, dataTable.Rows[index]["FPTP"]);
                    string str6 = String.Format("{0}\\{1}", Utils.driveImage, dataTable.Rows[index]["XSZTP"]);
                    try
                    {
                        if (!string.IsNullOrEmpty(dataTable.Rows[index]["FPTP"].ToString()))
                        {
                            string PicturePath1 = ExportPath + "\\附件6：车辆发票图片";
                            if (!Directory.Exists(PicturePath1))
                            {
                                Directory.CreateDirectory(PicturePath1);
                            }
                            ImageTool.MakeThumbnail(str5, PicturePath1 + "\\", "车辆发票VIN-" + str2);
                            //File.Copy(str5, PicturePath1 + "\\车辆发票VIN-" + str2 + Path.GetExtension(str5), true);
                        }
                        if (!string.IsNullOrEmpty(dataTable.Rows[index]["XSZTP"].ToString()))
                        {
                            string PicturePath2 = ExportPath + "\\附件7：车辆行驶证图片";
                            if (!Directory.Exists(PicturePath2))
                            {
                                Directory.CreateDirectory(PicturePath2);
                            }
                            ImageTool.MakeThumbnail(str6, PicturePath2 + "\\", "行驶证VIN-" + str2);
                            //File.Copy(str6, PicturePath2 + "\\行驶证VIN-" + str2 + Path.GetExtension(str5), true);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    document.NewPage();
                    iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 16f, 1, new iTextSharp.text.Color(0, 0, 0));
                    document.Add((IElement)new Paragraph("页码：" + (index + 1), font3)
                    {
                        Alignment = 2
                    });
                    document.Add((IElement)new Paragraph("车辆生产企业：" + this.strManufacturer, font3)
                    {
                        Alignment = 1
                    });
                    document.Add((IElement)new Paragraph("车牌：" + str4 + " 车辆发票：" + str3, font3)
                    {
                        Alignment = 1
                    });
                    document.Add((IElement)new Paragraph("VIN车辆：" + str2, font3)
                    {
                        Alignment = 1
                    });
                    document.Add((IElement)new Paragraph("车辆行驶证", font3)
                    {
                        Alignment = 1
                    });
                    if (!string.IsNullOrEmpty(dataTable.Rows[index]["XSZTP"].ToString()))
                    {
                        iTextSharp.text.Image instance2 = iTextSharp.text.Image.GetInstance(str6);
                        instance2.Alignment = 1;
                        instance2.ScaleToFit(400f, 400f);
                        document.Add((IElement)instance2);
                        document.Add((IElement)new Paragraph("车辆购置发票", font3)
                        {
                            Alignment = 1
                        });
                    }
                    if (!string.IsNullOrEmpty(dataTable.Rows[index]["FPTP"].ToString()))
                    {
                        iTextSharp.text.Image instance3 = iTextSharp.text.Image.GetInstance(str5);
                        instance3.Alignment = 1;
                        instance3.ScaleToFit(400f, 400f);
                        document.Add((IElement)instance3);
                    }
                    //++pb.Value;
                }
                document.Close();
                instance1.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
