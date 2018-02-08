using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Catarc.Adc.NewEnergyAccountSys.DBUtils;
using Catarc.Adc.NewEnergyAccountSys.OfficeHelper;
using Catarc.Adc.NewEnergyAccountSys.Properties;
using System.Collections.Generic;
using System.Linq;
using Catarc.Adc.NewEnergyAccountSys.FormUtils;
using System.Data.OleDb;
using Catarc.Adc.NewEnergyAccountSys.Common;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraSplashScreen;
using Catarc.Adc.NewEnergyAccountSys.DevForm;
using DevExpress.XtraGrid.Views.Base;

namespace Catarc.Adc.NewEnergyAccountSys.Form_Data
{
    public partial class NoticeParam : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static NewEnergyWeb.INewEnergyClearingServiceService NewEnergyService = NewEnergyUtils.newEnergyservice;

        public NoticeParam()
        {
            InitializeComponent();
            dgvNotice.OptionsView.ColumnAutoWidth = false;  
            //按钮显示
            string Item = this.Text;
            List<string> ButtonModel = Authority.ReadMenusXmlData("AuthorityUrl").Where(c => Item.Contains(c.ParentID.ToString())).Select(c => c.MenuName).ToList<string>();
            foreach (BarItemLink link in ribbonPageGroup1.ItemLinks)
            {
                object j = link.Item.Tag;
                if (ButtonModel.Contains(link.Caption))
                {
                    link.Item.Visibility = BarItemVisibility.Always;
                }
                else
                {
                    link.Item.Visibility = BarItemVisibility.Never;
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.SearchParamData();
        }

        protected void SearchParamData() 
        {
            string sql = "SELECT * FROM ANNOUNCE_PARAM WHERE 1=1 ";
            if (!string.IsNullOrEmpty(this.txt_BATCH.Text))
            {
                sql += string.Format(@"AND BATCH like '%{0}%'", this.txt_BATCH.Text);
            }
            if (!string.IsNullOrEmpty(this.txt_vehModel.Text))
            {
                sql += string.Format(@"AND MODEL_VEHICLE like '%{0}%'", this.txt_vehModel.Text);
            }
            DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                dgcNotice.DataSource = ds.Tables[0];
                this.dgvNotice.BestFitColumns();
                foreach (GridColumn col in this.dgvNotice.Columns)
                    col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                this.lblSum.Text = string.Format("共{0}条", ds.Tables[0].Rows.Count);
            }
            else
            {
                MessageBox.Show("该企业公告参数不存在", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshNotice_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.SearchParamData();
        }

        //同步
        private void btn_synchronize_ItemClick(object sender, ItemClickEventArgs e)
        {
            //将接口数据同步
            string CompanyName = Settings.Default.Vehicle_MFCS;
            string UserName = Settings.Default.UserName;
            string Pwd = Settings.Default.UserPwd;
            string ClearYear = Settings.Default.ClearYear;
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                NewEnergyWeb.newEnergyVehicle[] NoticeParamArr = NewEnergyUtils.newEnergyservice.getNewEnergyVehicleInfo(UserName, Pwd, "", ClearYear, 1000000, 1);
                DataTable dt = NewEnergyUtils.NewEnergyInfoS2DT(NoticeParamArr);
                string columnNameStr = string.Empty;

                foreach (DataColumn dc in dt.Columns)
                {
                    columnNameStr += dc.ColumnName + ",";
                }
                columnNameStr = columnNameStr.TrimEnd(',');

                if (NoticeParamArr != null && NoticeParamArr.Length > 0)
                {
                    using (OleDbConnection conn = new OleDbConnection(AccessHelper.conn))
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        AccessHelper.ExecuteNonQuery(AccessHelper.conn, "delete from ANNOUNCE_PARAM");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            using (var tra = conn.BeginTransaction())
                            {
                                string columnValuesStr = string.Empty;
                                for (int j = 0; j < dt.Columns.Count; j++)
                                {
                                    columnValuesStr += dt.Rows[i][j].ToString().Trim() + "','";
                                }
                                string MyChar = "','";
                                columnValuesStr = columnValuesStr.TrimEnd(MyChar.ToCharArray());
                                string sql = String.Format("insert into ANNOUNCE_PARAM ({0}) values ('{1}')", columnNameStr, columnValuesStr);
                                AccessHelper.ExecuteNonQuery(tra, sql, null);
                                tra.Commit();
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("操作出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
            this.SearchParamData();

        }

        //修改列显示文本
        private void dgvNotice_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "DATASOURCE")
            {
                switch (e.Value.ToString().Trim())
                {
                    case "gg":
                        e.DisplayText = "公告";
                        break;
                    case "nerds":
                        e.DisplayText = "推荐目录";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
        }

        //编辑列的行号
        private void dgvNotice_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
            }
        }

    }
}