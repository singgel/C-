using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars;
using FuelDataSysClient.Tool;
using Common;
using FuelDataSysClient.FuelCafc;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.Form_BGSC
{
    public partial class PreReportForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        private string forecast = System.Windows.Forms.Application.StartupPath + "\\ExcelHeaderTemplate\\forecast.doc";

        public PreReportForm()
        {
            InitializeComponent();
        }

        private void PreReportForm_Load(object sender, EventArgs e)
        {
            for (int i = 2013; i <= 2020; i++)
            {
                comboBoxEdit1.Properties.Items.Add(i);
            }
            comboBoxEdit1.Text = DateTime.Now.Year.ToString();
        }

        //导入预测数据
        private void btnImport_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel|*.xls;*.xlsx|All Files|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    MitsUtils miutils = new MitsUtils();
                    DataSet ds = miutils.ReadExcel(ofd.FileName, "Sheet1");

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var dt = ds.Tables[0];
                        dt.Columns["车辆型号"].ColumnName = "CLXH";
                        dt.Columns["燃料类型"].ColumnName = "RLLX";
                        dt.Columns["变速器型式"].ColumnName = "BSQXS";
                        dt.Columns["整车整备质量"].ColumnName = "ZCZBZL";
                        dt.Columns["座位排数"].ColumnName = "ZWPS";
                        dt.Columns["纯电动驱动模式综合工况续驶里程"].ColumnName = "ZHGKXSLC";
                        dt.Columns["综合工况燃料消耗量"].ColumnName = "ZHGKRLXHL";
                        dt.Columns["预计制造/进口量"].ColumnName = "SL";
                        DataTableHelper.removeEmpty(dt);
                        Utils.serviceCafc.DeleteForeCast(Utils.userId, Utils.password, "prj_" + Utils.userId);
                        if (newProject())
                        {
                            if (newCarType(dt))
                            {
                                this.gcParam.DataSource = null;
                                this.gcParam.DataSource = dt;
                                this.gcCafc.DataSource = null;
                                MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("数据为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
            }
        }

        //生成预报告
        private void btnProduce_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataTable dt = (DataTable)this.gcParam.DataSource;
            if (dt == null)
            {
                MessageBox.Show("无核算结果", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dt.Rows.Count > 0)
            {
               // SaveFileDialog saveFileDialog = new SaveFileDialog() { Title = "生成Word", Filter = "Word文件(*.doc)|*.docx" };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    try
                    {
                        SplashScreenManager.ShowForm(typeof(DevWaitForm));
                        List<CafcService.FuelCafcAndTcafc> cafcData = new List<CafcService.FuelCafcAndTcafc>();
                        if (Convert.ToInt32(this.comboBoxEdit1.Text) > 2015)
                        {
                            var data = Utils.serviceCafc.QueryForeCastNECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                            if (data != null && data.Length > 0)
                            {
                                cafcData = data.ToList();
                            }
                        }
                        else
                        {
                            var data = Utils.serviceCafc.QueryForeCastTECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                            if (data != null && data.Length > 0)
                            {
                                cafcData = data.ToList();
                            }
                        }
                        if (cafcData.Count > 0)
                        {
                            Dictionary<string, string> datas = new Dictionary<string, string>();
                            datas.Add("{qymc}", Utils.qymc);
                            datas.Add("{year}", comboBoxEdit1.Text);
                            datas.Add("{count}", cafcData[0].Sl_act.ToString());
                            datas.Add("{tcafc}", cafcData[0].Tcafc.ToString());
                            datas.Add("{cafc}", cafcData[0].Cafc.ToString());
                            decimal percent = (cafcData[0].Tcafc - cafcData[0].Cafc) / cafcData[0].Tcafc;
                            percent = percent * 10;
                            percent = Math.Round(percent, 1, MidpointRounding.ToEven);
                            datas.Add("{percent}", percent.ToString());
                            WordHelper wordBuilder = new WordHelper();
                            wordBuilder.CreateNewDocument(forecast);
                            foreach (var item in datas)
                            {
                                wordBuilder.InsertReplaceText(item.Key, item.Value);
                            }
                            wordBuilder.SaveDocument(saveFileDialog.FileName);
                            if (MessageBox.Show("生成成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(saveFileDialog.FileName);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        SplashScreenManager.CloseForm();
                    }
                }
            }
            else
            {
                MessageBox.Show("请导入预测车型", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //核算预报告
        private void btnTeSearch_Click(object sender, EventArgs e)
        {
            string msg = string.Empty;
            DataTable dt = (DataTable)this.gcParam.DataSource;
            if (dt == null)
            {
                MessageBox.Show("请先导入预测数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dt.Rows.Count > 0)
            {
                this.DataBinds();
            }
            else
            {
                MessageBox.Show("请先导入预测数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //向服务器传递新的预测数据项目
        private bool newProject()
        {
            string preTime = String.Format("{0}/{1}/{2}", this.comboBoxEdit1.Text, DateTime.Now.Month, DateTime.Now.Day);
            CafcService.ForecastPrj prjObj = new CafcService.ForecastPrj()
            {
                Prj_Id = "prj_" + Utils.userId,
                StartTime = Convert.ToDateTime(preTime),
                EndTime = Convert.ToDateTime(preTime)
            };
            return Utils.serviceCafc.SaveForecastPrj(Utils.userId, Utils.password, prjObj);
        }

        //向服务器传递新的预测数据
        private static bool newCarType(DataTable _dt)
        {
            DataTable dt = _dt;
            List<CafcService.ForecastParam> dataList = new List<CafcService.ForecastParam>();
            foreach (DataRow dr in dt.Rows)
            {
                CafcService.ForecastParam obj = new CafcService.ForecastParam();
                obj.Prj_Id = "prj_" + Utils.userId;
                obj.Qcscqy = Utils.qymc;
                obj.Clxh = string.IsNullOrEmpty(dr["CLXH"].ToString()) ? "" : dr["CLXH"].ToString();
                obj.Rllx = string.IsNullOrEmpty(dr["RLLX"].ToString()) ? "" : dr["RLLX"].ToString();
                obj.Bsqxs = string.IsNullOrEmpty(dr["BSQXS"].ToString()) ? "" : dr["BSQXS"].ToString();
                obj.Zwps = string.IsNullOrEmpty(dr["ZWPS"].ToString()) ? 0 : Convert.ToInt32(dr["ZWPS"]);
                obj.Zczbzl = string.IsNullOrEmpty(dr["ZCZBZL"].ToString()) ? 0 : Convert.ToInt32(dr["ZCZBZL"]);
                obj.Zhgkxslc = string.IsNullOrEmpty(dr["ZHGKXSLC"].ToString()) ? 0 : Convert.ToDecimal(dr["ZHGKXSLC"]);
                obj.Zhgkrlxhl = string.IsNullOrEmpty(dr["ZHGKRLXHL"].ToString()) ? 0 : Convert.ToDecimal(dr["ZHGKRLXHL"]);
                obj.Sl = string.IsNullOrEmpty(dr["SL"].ToString()) ? 0 : Convert.ToInt32(dr["SL"]);
                dataList.Add(obj);
            }
            return Utils.serviceCafc.SaveForecastParam(Utils.userId, Utils.password, dataList.ToArray());
        }

        //获取预测结果
        private void DataBinds()
        {
            try
            {
                CafcService.FuelCafcAndTcafc[] neCafcData = Utils.serviceCafc.QueryForeCastNECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                CafcService.FuelCafcAndTcafc[] teCafcData = Utils.serviceCafc.QueryForeCastTECafc(Utils.userId, Utils.password, "prj_" + Utils.userId);
                CafcModel cafc = new CafcModel();
                if (neCafcData != null && neCafcData.Length > 0)
                {
                    cafc.ne_Sl_act = neCafcData[0].Sl_act;
                    cafc.ne_Sl_hs = neCafcData[0].Sl_hs;
                    cafc.ne_Cafc = neCafcData[0].Cafc;
                    cafc.ne_Tcafc = neCafcData[0].Tcafc;
                }
                if (teCafcData != null && teCafcData.Length > 0)
                {
                    cafc.te_Sl_act = teCafcData[0].Sl_act;
                    cafc.te_Sl_hs = teCafcData[0].Sl_hs;
                    cafc.te_Cafc = teCafcData[0].Cafc;
                    cafc.te_Tcafc = teCafcData[0].Tcafc;
                }
                this.gcCafc.DataSource = new CafcModel[] { cafc };

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //预测年份更换
        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)this.gcParam.DataSource;
            if (dt.Rows.Count > 0)
            {
                Utils.serviceCafc.DelForecastPrj(Utils.userId, Utils.password, new string[] { "prj_" + Utils.userId });
                newProject();
                newCarType(dt);
            }
        }

        //获取预测数据的demo类
        public class CafcModel
        {

            public int te_Sl_act { get; set; }

            public int te_Sl_hs { get; set; }

            public decimal te_Cafc { get; set; }

            public decimal te_Tcafc { get; set; }

            public int ne_Sl_act { get; set; }

            public int ne_Sl_hs { get; set; }

            public decimal ne_Cafc { get; set; }

            public decimal ne_Tcafc { get; set; }

        }
    }
}