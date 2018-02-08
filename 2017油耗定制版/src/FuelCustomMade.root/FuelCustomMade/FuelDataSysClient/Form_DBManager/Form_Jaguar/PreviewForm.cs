using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FuelDataSysClient.Tool.Tool_Jaguar;


namespace FuelDataSysClient.Form_DBManager.Form_Jaguar
{
    public partial class PreviewForm : DevExpress.XtraEditors.XtraForm
    {
        private DataSet dsGenCtny = new DataSet();
        private DataSet dsGenFcds = new DataSet();
        private string dataType;  // 国环数据还是燃料数据

        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        public PreviewForm()
        {
            InitializeComponent();
        }

        //TODO-lyc：增加tab页面显示非插电式
        public PreviewForm(DataSet dsCtny, DataSet dsFcds, string dataType)
        {
            InitializeComponent();
            this.dsGenCtny = dsCtny;
            this.dsGenFcds = dsFcds;
            this.dataType = dataType;

            if (this.dataType == Utils.FUEL)
            {
                if (dsCtny.Tables[0].Rows.Count > 0)
                {
                    this.xtcPreview.SelectedTabPage = this.xtpCtny;
                }
                else
                {
                    this.xtcPreview.SelectedTabPage = this.xtpFcds;
                }
            }
            else if (this.dataType == Utils.GH)
            {
                this.xtcPreview.SelectedTabPage = this.xtpGh;
            }

            this.ShowReadyData(dsGenCtny, dsGenFcds);
        }

        //TODO-lyc 增加tab页面显示非插电式
        protected void ShowReadyData(DataSet dsCtny, DataSet dsFcds)
        {
            try
            {
                if (dsCtny != null)
                {
                    if (this.dataType == Utils.FUEL)
                    {
                        this.dgvCtny.DataSource = dsCtny.Tables[0];
                        this.gvCtny.BestFitColumns();
                        this.dgvFcds.DataSource = dsFcds.Tables[0];
                        this.gvFcds.BestFitColumns();
                    }
                    else if (this.dataType == Utils.GH)
                    {
                        DataTable dt = new DataTable();
                        dt = dsCtny.Tables[0].Copy();
                        dt.Merge(dsFcds.Tables[0]);
                        this.gcGhData.DataSource = dt;
                        this.gvGhData.BestFitColumns();
                    }
                    //this.lblReadyData.Text=string.Format("待生成数据共{0}条",dt.Rows.Count);
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnGenFuelData_Click(object sender, EventArgs e)
        {
            try
            {
                JaguarUtils jaguarUtils = new JaguarUtils();
                string msg = string.Empty;

                if (this.dataType == Utils.FUEL)
                {
                    msg += jaguarUtils.SaveParam(dsGenCtny, Utils.CTNY);
                    msg += jaguarUtils.SaveParam(dsGenFcds, Utils.FCDS);
                }
                else if (this.dataType == Utils.GH)
                {
                    msg += jaguarUtils.SaveGHParam(dsGenCtny);
                    msg += jaguarUtils.SaveGHParam(dsGenFcds);
                }

                if (string.IsNullOrEmpty(msg))
                {
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("生成成功", "生成成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageForm mf = new MessageForm("以下数据已经存在\r\n" + msg);
                    Utils.SetFormMid(mf);
                    mf.Text = "生成结果";
                    mf.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成失败：" + ex.Message, "生成失败");
            }
        }
    }
}