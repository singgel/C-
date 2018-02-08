using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Microsoft.Office.Interop;
using DevExpress.XtraRichEdit;
using System.IO;
using FuelDataSysClient.FuelCafc;
using DevExpress.XtraRichEdit.API.Native;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Interop.Word;
using System.Reflection;
using Common;
using FuelDataSysClient.Tool;
using System.Threading;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Form_BGSC
{
    public partial class WholeReportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public WholeReportForm()
        {
            InitializeComponent();
            for (int i = 2013; i <= DateTime.Now.Year; i++)
            {
                comboBoxEdit1.Properties.Items.Add(i);
            }
            comboBoxEdit1.SelectedIndex = comboBoxEdit1.Properties.Items.Count - 1;
        }

        //生成年度报告
        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                var cafcDetai = Utils.serviceCafc.QueryNECafc(Utils.userId, Utils.password, String.Format("{0}-01-01", comboBoxEdit1.Text), String.Format("{0}-12-31", comboBoxEdit1.Text));
                if (cafcDetai != null)
                {
                    Dictionary<string, string> datas = new Dictionary<string, string>();
                    datas.Add("{qymc}", Utils.qymc);
                    datas.Add("{year}", comboBoxEdit1.Text);
                    datas.Add("{count}", cafcDetai[0].Sl_act.ToString());
                    datas.Add("{tcafc}", cafcDetai[0].Tcafc.ToString());
                    datas.Add("{cafc}", cafcDetai[0].Cafc.ToString());
                    datas.Add("{percent}", Math.Round(Math.Round((cafcDetai[0].Tcafc - cafcDetai[0].Cafc) / cafcDetai[0].Tcafc, 10, MidpointRounding.AwayFromZero) * 100, 1, MidpointRounding.AwayFromZero).ToString());
                    var dTime = Convert.ToDateTime(String.Format("{0}-12-31", comboBoxEdit1.Text)).AddDays(1);
                    CafcService.FuelCAFCDetails[] Data = Utils.serviceCafc.QueryCalParamDetails(Utils.userId, Utils.password, string.Empty, string.Empty, String.Format("{0}-01-01", comboBoxEdit1.Text), dTime.ToString());
                    var dt = DataTableHelper.ToDataTable<CafcService.FuelCAFCDetails>(Data);
                    dt.Columns["Bsqxs"].SetOrdinal(5);

                    string templateWordName = createTempWord(String.Format("{0}\\ExcelHeaderTemplate\\Year2.doc", System.Windows.Forms.Application.StartupPath), datas, dt);
                    this.richEditControl1.LoadDocument(templateWordName, DocumentFormat.Doc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //生成Word
        private static string createTempWord(string templateFilePath, Dictionary<string, string> dic, System.Data.DataTable dt)
        {
            WordHelper wordBuilder = new WordHelper();
            wordBuilder.CreateNewDocument(templateFilePath);
            foreach (var item in dic)
            {
                wordBuilder.InsertReplaceText(item.Key, item.Value);
            }
            Microsoft.Office.Interop.Word.Table table = wordBuilder.InsertTable("BOOKMARK_TABLE", dt.Rows.Count + 1, 12, 0);
            wordBuilder.SetParagraph_Table(table, 0, 0);
            wordBuilder.SetFont_Table(table, string.Empty, 10);
            wordBuilder.InsertCell(table, 1, 1, "序号");
            wordBuilder.InsertCell(table, 1, 2, "汽车生产企业");
            wordBuilder.InsertCell(table, 1, 3, "车辆型号");
            wordBuilder.InsertCell(table, 1, 4, "燃料种类");
            wordBuilder.InsertCell(table, 1, 5, "整车整备质量");
            wordBuilder.InsertCell(table, 1, 6, "变速器型式");
            wordBuilder.InsertCell(table, 1, 7, "座位排数");
            wordBuilder.InsertCell(table, 1, 8, "纯电动驱动模式综合工况续驶里程");
            wordBuilder.InsertCell(table, 1, 9, "车型燃料消耗量目标值①");
            wordBuilder.InsertCell(table, 1, 10, "综合工况燃烧消耗量实际值②");
            wordBuilder.InsertCell(table, 1, 11, "实际生产/进口量③");
            wordBuilder.InsertCell(table, 1, 12, "备注");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                table.Cell(i + 2, 1).Range.Text = (i + 1).ToString();
                table.Cell(i + 2, 2).Range.Text = dt.Rows[i]["Qcscqy"].ToString();
                table.Cell(i + 2, 3).Range.Text = dt.Rows[i]["Clxh"].ToString();
                table.Cell(i + 2, 4).Range.Text = dt.Rows[i]["Rllx"].ToString();
                table.Cell(i + 2, 5).Range.Text = dt.Rows[i]["Zczbzl"].ToString();
                table.Cell(i + 2, 6).Range.Text = dt.Rows[i]["Bsqxs"].ToString();
                table.Cell(i + 2, 7).Range.Text = dt.Rows[i]["Zwps"].ToString();
                table.Cell(i + 2, 8).Range.Text = dt.Rows[i]["Zhgkxslc"].ToString();
                table.Cell(i + 2, 9).Range.Text = dt.Rows[i]["TgtZhgkrlxhl"].ToString();
                table.Cell(i + 2, 10).Range.Text = dt.Rows[i]["ActZhgkrlxhl"].ToString();
                table.Cell(i + 2, 11).Range.Text = dt.Rows[i]["Sl_act"].ToString();
            }
            string tempWordName = String.Format("{0}\\tempYear2.doc", DefaultDirectory.TempDoc);
            wordBuilder.SaveDocument(tempWordName);
            return tempWordName;
        }

        //初始化编辑器
        private void richEditControl1_InitializeDocument(object sender, EventArgs e)
        {
            RichEditControl obj = (RichEditControl)sender;
            DevExpress.XtraRichEdit.API.Native.Document document = obj.Document;
            document.BeginUpdate();
            try
            {
                document.DefaultCharacterProperties.FontName = "新宋体";
                document.DefaultParagraphProperties.LineSpacingType = ParagraphLineSpacing.Sesquialteral;
            }
            finally
            {
                document.EndUpdate();
            }
        }

    }
}