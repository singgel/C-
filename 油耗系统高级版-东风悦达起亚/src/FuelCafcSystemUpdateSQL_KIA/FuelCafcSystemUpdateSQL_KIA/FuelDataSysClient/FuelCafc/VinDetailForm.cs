using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using System.Reflection;
using System.Collections;
using FuelDataSysClient.Tool;
using DevExpress.XtraSplashScreen;
using FuelDataSysClient.SubForm;

namespace FuelDataSysClient.FuelCafc
{
    public partial class VinDetailForm : DevExpress.XtraEditors.XtraForm
    {
        private string startTime = string.Empty;
        private string endTime = string.Empty;
        private CafcService.FuelCAFCDetails fuelDetail;
        public CafcService.FuelCAFCDetails FuelDetail
        {
            get { return fuelDetail; }
            set { fuelDetail = value; }
        }

        public VinDetailForm()
        {
            InitializeComponent();
        }

        public VinDetailForm(string startTime, string endTime, CafcService.FuelCAFCDetails fuelDetail)
            : this()
        {
            // 统计开始时间和结束时间，用于反查VIN时确定时间范围
            this.startTime = startTime;
            this.endTime = endTime;
            this.FuelDetail = fuelDetail;
        }

        // 窗体加载完-加载数据
        private void VinDetailForm_Load(object sender, EventArgs e)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                CafcService.VinData[] vinDataArr = Utils.serviceCafc.QueryVinKeyParam(Utils.userId, Utils.password, this.startTime, this.endTime, this.FuelDetail);
                if (vinDataArr != null)
                {
                    this.gcVinData.DataSource = vinDataArr;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        // 导出Vin
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    FileName = "VIN明细页面数据",
                    Title = "导出Excel",
                    Filter = "Excel文件(*.xls)|*.xls"
                };
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions() { TextExportMode = TextExportMode.Value };
                    this.gcVinData.ExportToXls(saveFileDialog.FileName, options);
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable L2D(FuelDataSysClient.CafcService.VinData[] list)
        {
            DataTable result = new DataTable();
            if (list.Length > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name, pi.PropertyType);
                }

                for (int i = 0; i < list.Length; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}