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

namespace FuelDataSysClient.FuelCafc
{
    public partial class VinDetailForm : DevExpress.XtraEditors.XtraForm
    {
        CafcService.CafcWebService cafcService = StaticUtil.cafcService;
        
        private string paramType = string.Empty;
        string startTime = string.Empty;
        string endTime = string.Empty;
        private CafcService.StatisticsData statData;

        public CafcService.StatisticsData StatData
        {
            get { return statData; }
            set { statData = value; }
        }

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

        public VinDetailForm(CafcService.StatisticsData statData)  : this()
        {
            this.paramType = ParamType.Statistic.ToString();
            this.StatData = statData;
        }

        public VinDetailForm(string startTime,string endTime, CafcService.FuelCAFCDetails fuelDetail) : this()
        {
            // 统计开始时间和结束时间，用于反查VIN时确定时间范围
            this.startTime = startTime;
            this.endTime = endTime;
            this.paramType = ParamType.Detail.ToString();
            this.FuelDetail = fuelDetail;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VinDetail_Load(object sender, EventArgs e)
        {
            this.GetVinData();
        }

        private void GetVinData()
        {
            try
            {
                CafcService.VinData[] vinDataArr = null;
                if (this.StatData != null || this.FuelDetail != null)
                {
                    if (this.paramType == ParamType.Statistic.ToString())
                    {
                        vinDataArr = cafcService.QueryVinData(Utils.userId, Utils.password, this.StatData);
                        this.SetControlVisible(false);
                    }
                    else
                    {
                        vinDataArr = cafcService.QueryVinKeyParam(Utils.userId, Utils.password,this.startTime, this.endTime, this.FuelDetail);
                        this.SetControlVisible(true);
                    }
                    if (vinDataArr != null)
                    {
                        this.gcVinData.DataSource = vinDataArr;
                        this.lblSum.Text = string.Format("共{0}条", vinDataArr.Length);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void SetControlVisible(bool isVisible)
        {
            this.Bsqxs.Visible = isVisible;
            this.Zhgkxslc.Visible = isVisible;
            this.Zhgkrlxhl.Visible = isVisible;
        }

        /// <summary>
        /// 导出Vin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "导出Excel";
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                saveFileDialog.FileName = "Vin明细";
                DialogResult dialogResult = saveFileDialog.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    XlsExportOptions options = new XlsExportOptions();
                    options.TextExportMode = TextExportMode.Value;
                    //options.ExportMode = XlsExportMode.SingleFile;

                    this.gcVinData.ExportToXls(saveFileDialog.FileName, options);

                    MessageBox.Show("导出成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            GetVinData();
            FuelDataSysClient.CafcService.VinData[] vinList = (FuelDataSysClient.CafcService.VinData[])this.gcVinData.DataSource;
            var dt = L2D(vinList);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(this.tbVin.Text))
                {
                    DataTable temp = dt.Clone();
                    var dr = dt.Select(String.Format("Vin='{0}'", tbVin.Text));
                    for (int i = 0; i < dr.Length; i++)
                    {
                        temp.ImportRow(dr[i]);
                    }
                    dt = temp;
                }
                if (!string.IsNullOrEmpty(this.tbClxh.Text))
                {
                    DataTable temp = dt.Clone();
                    var dr = dt.Select(String.Format("Clxh='{0}'", tbClxh.Text));
                    for (int i = 0; i < dr.Length; i++)
                    {
                        temp.ImportRow(dr[i]);
                    }
                    dt = temp;
                }
                this.gcVinData.DataSource = dt;
                this.lblSum.Text = string.Format("共{0}条", dt.Rows.Count);
                MessageBox.Show("查询完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            this.lblSum.Text = string.Format("共{0}条", "0");
        }

        private DataTable L2D(FuelDataSysClient.CafcService.VinData[] list)
        {
            DataTable result = new DataTable();
            if (list != null && list.Length > 0)
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