using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Data;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using System.Windows.Forms;
using System.Diagnostics;


namespace Catarc.Adc.NewEnergyAccountSys.Common
{
    public class Tool
    {
        string strManufacturer = "企业名称";
        public static string strYear = "2017";
        private string strEvPath = Utils.installPath;

        /// <summary>
        /// 导出PDF
        /// </summary>
        /// <param name="ExportPath"></param>
        /// <param name="dtUser"></param>
        public void ExportPDF(string ExportPath, System.Data.DataTable dtUser)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("select ID,Bill,BillPDF,Drive,DrivePDF,vin,pin,bill_num from AutoFill_Message order by ID");
                System.Data.DataTable dataTable = AccessHelper.ExecuteDataSet(AccessHelper.conn,stringBuilder.ToString(),null).Tables[0];
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4);
                PdfWriter instance1 = PdfWriter.GetInstance(document, (Stream)new FileStream(ExportPath + "\\附件5：推广应用车辆补助资金清算信息.PDF", FileMode.CreateNew));
                document.Open();
                BaseFont font1 = BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", "Identity-H", false);
                iTextSharp.text.Font font2 = new iTextSharp.text.Font(font1, 24f, 1, new iTextSharp.text.Color(0, 0, 0));
                iTextSharp.text.Paragraph paragraph1 = new iTextSharp.text.Paragraph(" ", font2);
                document.Add((IElement)new iTextSharp.text.Paragraph("附件5：", font2)
                {
                    Alignment = 0
                });
                Paragraph paragraph2 = new Paragraph(" ", font2);
                paragraph2.Alignment = 1;
                paragraph2.SetLeading(20f, 2f);
                document.Add((IElement)paragraph2);
                document.Add((IElement)new Paragraph("______年度推广应用车辆补助资金清算信息", font2)
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
                    string str1 = dataTable.Rows[index]["ID"].ToString();
                    string str2 = dataTable.Rows[index]["VIN"].ToString();
                    string str3 = dataTable.Rows[index]["bill_num"].ToString();
                    string str4 = dataTable.Rows[index]["Pin"].ToString();
                    string str5 = this.strEvPath + "\\IMAGEBill\\" + dataTable.Rows[index]["Bill"].ToString();
                    string str6 = this.strEvPath + "\\IMAGEDrive\\" + dataTable.Rows[index]["Drive"].ToString();
                    try
                    {
                        File.Copy(str5, ExportPath + "\\附件6：车辆发票图片\\车辆发票VIN-" + str2 + Path.GetExtension(str5), true);
                        File.Copy(str6, ExportPath + "\\附件7：车辆行驶证图片\\行驶证VIN-" + str2 + Path.GetExtension(str6), true);
                    }
                    catch (Exception ex)
                    {
                    }
                    document.NewPage();
                    iTextSharp.text.Font font3 = new iTextSharp.text.Font(font1, 16f, 1, new iTextSharp.text.Color(0, 0, 0));
                    document.Add((IElement)new Paragraph("页码：" + str1, font3)
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
                    iTextSharp.text.Image instance2 = iTextSharp.text.Image.GetInstance(str6);
                    instance2.Alignment = 1;
                    instance2.ScaleToFit(400f, 400f);
                    document.Add((IElement)instance2);
                    document.Add((IElement)new Paragraph("车辆购置发票", font3)
                    {
                        Alignment = 1
                    });
                    iTextSharp.text.Image instance3 = iTextSharp.text.Image.GetInstance(str5);
                    instance3.Alignment = 1;
                    instance3.ScaleToFit(400f, 400f);
                    document.Add((IElement)instance3);
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

        /// <summary>
        /// PDF预览
        /// </summary>
        /// <param name="strPdfPath"></param>
        /// <param name="strImagePath"></param>
        /// <param name="strManufacturer"></param>
        /// <param name="strInvoice"></param>
        /// <param name="strVIN"></param>
        /// <returns></returns>
        public string ImageConvertPDF(string strPdfPath, string strImagePath, string strManufacturer, string strInvoice, string strVIN)
        {
            Document document = new Document(PageSize.A4);
            if (File.Exists(strPdfPath))
                File.Delete(strPdfPath);
            else if (!Directory.Exists(Path.GetDirectoryName(strPdfPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(strPdfPath));
            PdfWriter.GetInstance(document, (Stream)new FileStream(strPdfPath, FileMode.CreateNew));
            document.Open();
            iTextSharp.text.Font font = new iTextSharp.text.Font(BaseFont.CreateFont("C:\\WINDOWS\\Fonts\\simsun.ttc,1", "Identity-H", false), 24f, 1, new Color(0, 0, 0));
            Paragraph paragraph1 = new Paragraph(" ", font);
            Paragraph paragraph2 = new Paragraph("车辆生产企业：" + strManufacturer, font);
            paragraph2.Alignment = 1;
            paragraph2.SetLeading(20f, 2f);
            document.Add((IElement)paragraph2);
            document.Add((IElement)new Paragraph("车辆发票：" + strInvoice, font)
            {
                Alignment = 1
            });
            document.Add((IElement)new Paragraph("VIN车辆：" + strVIN, font)
            {
                Alignment = 1
            });
            Paragraph paragraph3 = new Paragraph(" ", font);
            paragraph3.Alignment = 1;
            paragraph3.SetLeading(20f, 2f);
            document.Add((IElement)paragraph3);
            Image instance = Image.GetInstance(strImagePath);
            instance.Alignment = 1;
            instance.ScaleToFit(600f, 600f);
            document.Add((IElement)instance);
            document.Close();
            return strPdfPath;
        }

        public static byte[] ImageConvertByte(string strImagePath)
        {
            FileStream fileStream = new FileStream(strImagePath, FileMode.Open, FileAccess.Read);
            return new BinaryReader((Stream)fileStream).ReadBytes((int)fileStream.Length);
        }

        public static void ByteConvertImage(byte[] byteImage, string strImagePath)
        {
            try
            {
                FileStream fileStream = !File.Exists(strImagePath) ? new FileStream(strImagePath, FileMode.CreateNew) : new FileStream(strImagePath, FileMode.Truncate);
                BinaryWriter binaryWriter = new BinaryWriter((Stream)fileStream);
                binaryWriter.Write(byteImage, 0, byteImage.Length);
                binaryWriter.Close();
                fileStream.Close();
                Process.Start(strImagePath);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message.ToString());
            }
        }

        public static byte[] PDFConvertByte(string strPDFPath)
        {
            FileStream fileStream = new FileStream(strPDFPath, FileMode.Open, FileAccess.Read);
            return new BinaryReader((Stream)fileStream).ReadBytes((int)fileStream.Length);
        }

        public static void ByteConvertPDF(byte[] bytePDF, string strPDFPath)
        {
            try
            {
                FileStream fileStream = !File.Exists(strPDFPath) ? new FileStream(strPDFPath, FileMode.CreateNew) : new FileStream(strPDFPath, FileMode.Truncate);
                BinaryWriter binaryWriter = new BinaryWriter((Stream)fileStream);
                binaryWriter.Write(bytePDF, 0, bytePDF.Length);
                binaryWriter.Close();
                fileStream.Close();
                Process.Start(strPDFPath);
            }
            catch (Exception ex)
            {
                int num = (int)MessageBox.Show(ex.Message.ToString());
            }
        }

        public static void ImageCopy(string strImagePath, string strFromPath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(strImagePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(strImagePath));
            File.Copy(strFromPath, strImagePath, false);
        }
    }
}
