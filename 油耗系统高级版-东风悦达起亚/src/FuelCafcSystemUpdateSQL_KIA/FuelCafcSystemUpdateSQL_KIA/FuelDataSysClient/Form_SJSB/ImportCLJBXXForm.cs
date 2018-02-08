using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraEditors;
using FuelDataModel;
using FuelDataSysClient.SubForm;
using System.Data.OleDb;
using System.IO;
using DevExpress.XtraPrinting;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using FuelDataSysClient.Tool;
using System.Threading;
using DevExpress.XtraSplashScreen;
using Oracle.ManagedDataAccess.Client;

namespace FuelDataSysClient.Form_SJSB
{
    public partial class ImportCLJBXXForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public ImportCLJBXXForm()
        {
            InitializeComponent();
            this.cbRLLX.Properties.Items.AddRange(Utils.GetFuelType("SEARCH").ToArray());
        }

        // 导入Excel
        private void barBtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Dictionary<string, string> error = new Dictionary<string, string>();
                string msg = string.Empty;
                try
                {
                    SplashScreenManager.ShowForm(typeof(DevWaitForm));
                    // STEP1：导入系统，验证单元格格式
                    var ds = ImportExcel.ReadExcelToDataSet(openFileDialog1.FileName);
                    for (int i = 0; i < ds.Tables["TEMPLATE"].Columns.Count; i++)
                    {
                        if (ds.Tables[0].Columns[i].ColumnName.Equals("车辆制造/进口日期"))
                        {
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                            {
                                if (ds.Tables[0].Rows[j][i].GetType() != typeof(System.DBNull))
                                {
                                    if (ds.Tables[0].Rows[j][i].GetType() != typeof(System.DateTime))
                                    {
                                        MessageBox.Show(String.Format("【{1}】列中第【{0}】行的单元格格式不正确，应为日期格式!", j + 2, ds.Tables[0].Columns[i].ColumnName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                }
                            }
                        }
                        else 
                        {
                            for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                            {
                                if (ds.Tables[0].Rows[j][i].GetType() != typeof(System.DBNull))
                                {
                                    if (ds.Tables[0].Rows[j][i].GetType() != typeof(System.String))
                                    {
                                        MessageBox.Show(String.Format("【{1}】列中第【{0}】行的单元格格式不正确，应为文本格式!", j + 2, ds.Tables[0].Columns[i].ColumnName), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    // STEP2：替换列名，验证参数的数值
                    var dt = ImportExcel.SwitchCLJBXXColumnName(ds);
                    if (dt != null)
                    {
                        error = DataVerifyHelper.VerifyCLJBXXData(dt);
                        if (error.Count == 0)
                        {
                            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
                            {
                                conn.Open();
                                using (OracleTransaction trans = conn.BeginTransaction())
                                {
                                    // STEP3：验证无误，导入系统数据库
                                    foreach (DataRow dr in dt.Rows)
                                    {
                                        try
                                        {
                                            string exist = OracleHelper.ExecuteScalar(trans, string.Format("SELECT COUNT(*) FROM OCN_CLJBXX WHERE OPERATION!='4' AND SC_OCN='{0}'", dr["SC_OCN"])).ToString();
                                            int existNum = string.IsNullOrEmpty(exist) ? 0 : Convert.ToInt32(exist);
                                            if (existNum > 0)
                                            {
                                                error.Add(dr["SC_OCN"].ToString().Trim(), "系统已经存在改生产OCN的整车基础数据！");
                                                continue;
                                            }
                                            OracleParameter[] parameters = 
                                            {
				                                new OracleParameter("SC_OCN", OracleDbType.NVarchar2,255),
				                                new OracleParameter("XT_OCN", OracleDbType.NVarchar2,255),
				                                new OracleParameter("MI_XT_OCN", OracleDbType.NVarchar2,255),
				                                new OracleParameter("TYMC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CLXH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("PFBZ", OracleDbType.NVarchar2,255),
				                                new OracleParameter("SFJKQC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("QCSCQY", OracleDbType.NVarchar2,255),
				                                new OracleParameter("JKQCZJXS", OracleDbType.NVarchar2,255),
				                                new OracleParameter("JCJGMC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("BGBH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("BAH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CLZZRQ", OracleDbType.Date),
				                                new OracleParameter("CLZL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("YYC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("QDXS", OracleDbType.NVarchar2,255),
				                                new OracleParameter("ZWPS", OracleDbType.NVarchar2,255),
				                                new OracleParameter("ZGCS", OracleDbType.NVarchar2,255),
				                                new OracleParameter("EDZK", OracleDbType.NVarchar2,255),
				                                new OracleParameter("LTGG", OracleDbType.NVarchar2,255),
				                                new OracleParameter("LJ", OracleDbType.NVarchar2,255),
				                                new OracleParameter("ZJ", OracleDbType.NVarchar2,255),
				                                new OracleParameter("RLLX", OracleDbType.NVarchar2,255),
				                                new OracleParameter("YHDYBAH", OracleDbType.NVarchar2,255),
				                                new OracleParameter("ZCZBZL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("ZDSJZZL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("ZHGKRLXHL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("RLXHLMBZ", OracleDbType.NVarchar2,255),
				                                new OracleParameter("JDBZMBZ4", OracleDbType.NVarchar2,255),
				                                new OracleParameter("BSQXS", OracleDbType.NVarchar2,255),
				                                new OracleParameter("PL", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CDDQDMSZHGKXHLC", OracleDbType.NVarchar2,255),
				                                new OracleParameter("OPERATION", OracleDbType.NVarchar2,255),
				                                new OracleParameter("CREATE_TIME", OracleDbType.Date),
				                                new OracleParameter("CREATE_ROLE", OracleDbType.NVarchar2,255),
				                                new OracleParameter("UPDATE_TIME", OracleDbType.Date),
				                                new OracleParameter("UPDATE_ROLE", OracleDbType.NVarchar2,255),
				                                new OracleParameter("VERSION", OracleDbType.Int32),
                                            };
                                            parameters[0].Value = dr["SC_OCN"].ToString().Trim();
                                            parameters[1].Value = dr["XT_OCN"].ToString().Trim();
                                            parameters[2].Value = dr["MI_XT_OCN"].ToString().Trim();
                                            parameters[3].Value = dr["TYMC"].ToString().Trim();
                                            parameters[4].Value = dr["CLXH"].ToString().Trim();
                                            parameters[5].Value = dr["PFBZ"].ToString().Trim();
                                            parameters[6].Value = dr["SFJKQC"].ToString().Trim();
                                            parameters[7].Value = dr["QCSCQY"].ToString().Trim();
                                            parameters[8].Value = dr["JKQCZJXS"].ToString().Trim();
                                            parameters[9].Value = dr["JCJGMC"].ToString().Trim();
                                            parameters[10].Value = dr["BGBH"].ToString().Trim();
                                            parameters[11].Value = dr["BAH"].ToString().Trim();
                                            parameters[12].Value = dr["CLZZRQ"];
                                            parameters[13].Value = dr["CLZL"].ToString().Trim();
                                            parameters[14].Value = dr["YYC"].ToString().Trim();
                                            parameters[15].Value = dr["QDXS"].ToString().Trim();
                                            parameters[16].Value = dr["ZWPS"].ToString().Trim();
                                            parameters[17].Value = dr["ZGCS"].ToString().Trim();
                                            parameters[18].Value = dr["EDZK"].ToString().Trim();
                                            parameters[19].Value = dr["LTGG"].ToString().Trim();
                                            parameters[20].Value = dr["LJ"].ToString().Trim();
                                            parameters[21].Value = dr["ZJ"].ToString().Trim();
                                            parameters[22].Value = dr["RLLX"].ToString().Trim();
                                            parameters[23].Value = dr["YHDYBAH"].ToString().Trim();
                                            parameters[24].Value = dr["ZCZBZL"].ToString().Trim();
                                            parameters[25].Value = dr["ZDSJZZL"].ToString().Trim();
                                            parameters[26].Value = dr["ZHGKRLXHL"].ToString().Trim();
                                            parameters[27].Value = dr["RLXHLMBZ"].ToString().Trim();
                                            parameters[28].Value = dr["JDBZMBZ4"].ToString().Trim();
                                            parameters[29].Value = dr["BSQXS"].ToString().Trim();
                                            parameters[30].Value = dr["PL"].ToString().Trim();
                                            parameters[31].Value = dr["CDDQDMSZHGKXHLC"].ToString().Trim();
                                            parameters[32].Value = "0";
                                            parameters[33].Value = System.DateTime.Today;
                                            parameters[34].Value = Utils.localUserId;
                                            parameters[35].Value = System.DateTime.Today;
                                            parameters[36].Value = Utils.localUserId;
                                            parameters[37].Value = 0;
                                            OracleHelper.ExecuteNonQuery(trans, "Insert into OCN_CLJBXX (SC_OCN,XT_OCN,MI_XT_OCN,TYMC,CLXH,PFBZ,SFJKQC,QCSCQY,JKQCZJXS,JCJGMC,BGBH,BAH,CLZZRQ,CLZL,YYC,QDXS,ZWPS,ZGCS,EDZK,LTGG,LJ,ZJ,RLLX,YHDYBAH,ZCZBZL,ZDSJZZL,ZHGKRLXHL,RLXHLMBZ,JDBZMBZ4,BSQXS,PL,CDDQDMSZHGKXHLC,OPERATION,CREATE_TIME,CREATE_ROLE,UPDATE_TIME,UPDATE_ROLE,VERSION) values (:SC_OCN,:XT_OCN,:MI_XT_OCN,:TYMC,:CLXH,:PFBZ,:SFJKQC,:QCSCQY,:JKQCZJXS,:JCJGMC,:BGBH,:BAH,:CLZZRQ,:CLZL,:YYC,:QDXS,:ZWPS,:ZGCS,:EDZK,:LTGG,:LJ,:ZJ,:RLLX,:YHDYBAH,:ZCZBZL,:ZDSJZZL,:ZHGKRLXHL,:RLXHLMBZ,:JDBZMBZ4,:BSQXS,:PL,:CDDQDMSZHGKXHLC,:OPERATION,:CREATE_TIME,:CREATE_ROLE,:UPDATE_TIME,:UPDATE_ROLE,:VERSION)", parameters);
                                        }
                                        catch (Exception ex)
                                        {
                                            error.Add(dr["SC_OCN"].ToString().Trim(), ex.Message);
                                            continue;
                                        }
                                    }
                                    if (trans.Connection != null) trans.Commit();
                                }
                            }
                            // STEP4：处理无误，处理完成的文件
                            if (error.Count == 0)
                            {
                                var destFolder = Path.Combine(Path.GetDirectoryName(openFileDialog1.FileName), DateTime.Today.ToLongDateString() + "-整车基础数据-Imported");
                                Directory.CreateDirectory(destFolder);
                                try
                                {
                                    File.Move(openFileDialog1.FileName, Path.Combine(destFolder, String.Format("Imported-{0}{1}", Path.GetFileNameWithoutExtension(openFileDialog1.FileName), Path.GetExtension(openFileDialog1.FileName))));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(String.Format("Excel处理操作异常：导入完成，{0}", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }
                        foreach (KeyValuePair<string, string> kvp in error)
                        {
                            msg += String.Format("{0}\r\n{1}\r\n", kvp.Key, kvp.Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Excel导入操作异常：" + ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                finally
                {
                    SplashScreenManager.CloseForm();
                }
                MessageForm msgForm = new MessageForm(msg + String.Format("\r\n{0}Excel导入操作完成", Path.GetFileNameWithoutExtension(openFileDialog1.FileName))) { Text = "整车基础数据导入信息" };
                msgForm.Show();
                SearchLocal(1);
            }
        }

        // 新增
        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (CLJBXXParamForm cljbxxParamForm = new CLJBXXParamForm())
            {
                cljbxxParamForm.ShowDialog();
                if (cljbxxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
            }
        }

        // 复制新增
        private void barBtnCopy_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.gvOCN_CLJBXX.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvOCN_CLJBXX.PostEditor();
            var dataSource = (DataView)this.gvOCN_CLJBXX.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                    }
                }
            }
            if (dtSelected.Rows.Count != 1)
            {
                MessageBox.Show(String.Format("每次只能操作一条记录，您选择了{0}条！", dtSelected.Rows.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dtSelected.Rows[0]["OPERATION"].Equals("4"))
            {
                MessageBox.Show(String.Format("OCN为{0}：已经删除，无法进行此操作！", dtSelected.Rows[0]["SC_OCN"]), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CLJBXXParamForm cljbxxParamForm = new CLJBXXParamForm() { Text = "整车数据复制" };
            this.setControlValue(cljbxxParamForm, "teSC_OCN", dtSelected.Rows[0]["SC_OCN"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teXT_OCN", dtSelected.Rows[0]["XT_OCN"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teMI_XT_OCN", dtSelected.Rows[0]["MI_XT_OCN"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teTYMC", dtSelected.Rows[0]["TYMC"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teCLXH", dtSelected.Rows[0]["CLXH"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "tePFBZ", dtSelected.Rows[0]["PFBZ"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "cbeSFJKQC", dtSelected.Rows[0]["SFJKQC"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teQCSCQY", dtSelected.Rows[0]["QCSCQY"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teJKQCZJXS", dtSelected.Rows[0]["JKQCZJXS"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teJCJGMC", dtSelected.Rows[0]["JCJGMC"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teBGBH", dtSelected.Rows[0]["BGBH"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teBAH", dtSelected.Rows[0]["BAH"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "dtCLZZRQ", dtSelected.Rows[0]["CLZZRQ"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "cbeCLZL", dtSelected.Rows[0]["CLZL"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "cbeYYC", dtSelected.Rows[0]["YYC"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "cbeQDXS", dtSelected.Rows[0]["QDXS"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teZWPS", dtSelected.Rows[0]["ZWPS"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teZGCS", dtSelected.Rows[0]["ZGCS"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teEDZK", dtSelected.Rows[0]["EDZK"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teLTGG", dtSelected.Rows[0]["LTGG"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teLJ", dtSelected.Rows[0]["LJ"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teZJ", dtSelected.Rows[0]["ZJ"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "cbeRLLX", dtSelected.Rows[0]["RLLX"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teYHDYBAH", dtSelected.Rows[0]["YHDYBAH"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teZCZBZL", dtSelected.Rows[0]["ZCZBZL"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teZDSJZZL", dtSelected.Rows[0]["ZDSJZZL"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teZHGKRLXHL", dtSelected.Rows[0]["ZHGKRLXHL"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teRLXHLMBZ", dtSelected.Rows[0]["RLXHLMBZ"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teJDBZMBZ4", dtSelected.Rows[0]["JDBZMBZ4"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "cbeBSQXS", dtSelected.Rows[0]["BSQXS"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "tePL", dtSelected.Rows[0]["PL"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teCDDQDMSZHGKXHLC", dtSelected.Rows[0]["CDDQDMSZHGKXHLC"].ToString(), false);
            cljbxxParamForm.ShowDialog();
            if (cljbxxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
        }

        // 修改
        private void barBtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.gvOCN_CLJBXX.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvOCN_CLJBXX.PostEditor();
            var dataSource = (DataView)this.gvOCN_CLJBXX.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                    }
                }
            }
            if (dtSelected.Rows.Count != 1)
            {
                MessageBox.Show(String.Format("每次只能操作一条记录，您选择了{0}条！", dtSelected.Rows.Count), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dtSelected.Rows[0]["OPERATION"].Equals("4"))
            {
                MessageBox.Show(String.Format("OCN为{0}：已经删除，无法进行此操作！", dtSelected.Rows[0]["SC_OCN"]), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CLJBXXParamForm cljbxxParamForm = new CLJBXXParamForm() { Text = "整车数据修改" };
            this.setControlValue(cljbxxParamForm, "teSC_OCN", dtSelected.Rows[0]["SC_OCN"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teXT_OCN", dtSelected.Rows[0]["XT_OCN"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teMI_XT_OCN", dtSelected.Rows[0]["MI_XT_OCN"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teTYMC", dtSelected.Rows[0]["TYMC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teCLXH", dtSelected.Rows[0]["CLXH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "tePFBZ", dtSelected.Rows[0]["PFBZ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeSFJKQC", dtSelected.Rows[0]["SFJKQC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teQCSCQY", dtSelected.Rows[0]["QCSCQY"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teJKQCZJXS", dtSelected.Rows[0]["JKQCZJXS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teJCJGMC", dtSelected.Rows[0]["JCJGMC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teBGBH", dtSelected.Rows[0]["BGBH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teBAH", dtSelected.Rows[0]["BAH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "dtCLZZRQ", string.IsNullOrEmpty(dtSelected.Rows[0]["CLZZRQ"].ToString()) == true ? string.Empty : Convert.ToDateTime(dtSelected.Rows[0]["CLZZRQ"].ToString()).ToString("yyyy/MM/dd"), true);
            this.setControlValue(cljbxxParamForm, "cbeCLZL", dtSelected.Rows[0]["CLZL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeYYC", dtSelected.Rows[0]["YYC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeQDXS", dtSelected.Rows[0]["QDXS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZWPS", dtSelected.Rows[0]["ZWPS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZGCS", dtSelected.Rows[0]["ZGCS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teEDZK", dtSelected.Rows[0]["EDZK"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teLTGG", dtSelected.Rows[0]["LTGG"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teLJ", dtSelected.Rows[0]["LJ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZJ", dtSelected.Rows[0]["ZJ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeRLLX", dtSelected.Rows[0]["RLLX"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teYHDYBAH", dtSelected.Rows[0]["YHDYBAH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZCZBZL", dtSelected.Rows[0]["ZCZBZL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZDSJZZL", dtSelected.Rows[0]["ZDSJZZL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZHGKRLXHL", dtSelected.Rows[0]["ZHGKRLXHL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teRLXHLMBZ", dtSelected.Rows[0]["RLXHLMBZ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teJDBZMBZ4", dtSelected.Rows[0]["JDBZMBZ4"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeBSQXS", dtSelected.Rows[0]["BSQXS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "tePL", dtSelected.Rows[0]["PL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teCDDQDMSZHGKXHLC", dtSelected.Rows[0]["CDDQDMSZHGKXHLC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "labOPERATION", "原操作类型号：V" + dtSelected.Rows[0]["OPERATION"], true);
            this.setControlValue(cljbxxParamForm, "labVERSION", "整车数据版本号：V" + dtSelected.Rows[0]["VERSION"], true);
            cljbxxParamForm.ShowDialog();
            if (cljbxxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
        }

        // 删除
        private void barBtnDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (this.gvOCN_CLJBXX.DataSource == null)
            {
                MessageBox.Show("没有可以操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            this.gvOCN_CLJBXX.PostEditor();
            var dataSource = (DataView)this.gvOCN_CLJBXX.DataSource;
            var dtSelected = dataSource.Table.Copy();
            dtSelected.Clear();
            if (dataSource != null && dataSource.Table.Rows.Count > 0)
            {
                for (int i = 0; i < dataSource.Table.Rows.Count; i++)
                {
                    bool result = false;
                    bool.TryParse(dataSource.Table.Rows[i]["check"].ToString(), out result);
                    if (result)
                    {
                        dtSelected.Rows.Add(dataSource.Table.Rows[i].ItemArray);
                    }
                }
            }
            if (dtSelected.Rows.Count == 0)
            {
                MessageBox.Show("请选择您要操作的记录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var sc_ocnArray = dtSelected.AsEnumerable().Select(d => d.Field<string>("SC_OCN")).ToArray();
            if (OracleHelper.Exists(OracleHelper.conn, String.Format("SELECT COUNT(*) FROM VIN_INFO WHERE SC_OCN IN ('{0}') AND MERGER_STATUS=0", string.Join("','", sc_ocnArray))))
            {
                MessageBox.Show("您选择要操作的记录包含未和合成的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }
            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    foreach (DataRow dr in dtSelected.Rows)
                    {
                        try
                        {
                            string version = OracleHelper.ExecuteScalar(OracleHelper.conn, string.Format("SELECT MIN(VERSION) FROM OCN_CLJBXX WHERE OPERATION='4' AND SC_OCN='{0}'", dr["SC_OCN"])).ToString();
                            int versionNew = string.IsNullOrEmpty(version) ? 0 : Convert.ToInt32(version) - 1;
                            OracleHelper.ExecuteNonQuery(OracleHelper.conn, string.Format("UPDATE OCN_CLJBXX SET OPERATION = '4',VERSION = '{0}' WHERE SC_OCN='{1}' AND OPERATION = '{2}' AND VERSION={3} ", versionNew, dr["SC_OCN"], dr["OPERATION"], dr["VERSION"]));
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            MessageBox.Show(String.Format("数据库操作出现异常，删除失败：{0}！", ex.Message), "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    if (trans.Connection != null) trans.Commit();
                }
            }
            this.refrashCurrentPage();
        }

        // 全选
        private void btnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvOCN_CLJBXX.FocusedRowHandle = 0;
            this.gvOCN_CLJBXX.FocusedColumn = gvOCN_CLJBXX.Columns["SC_OCN"];
            Utils.SelectItem(this.gcOCN_CLJBXX.MainView, true);
        }

        // 取消全选
        private void btnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvOCN_CLJBXX.FocusedRowHandle = 0;
            this.gvOCN_CLJBXX.FocusedColumn = gvOCN_CLJBXX.Columns["SC_OCN"];
            Utils.SelectItem(this.gcOCN_CLJBXX.MainView, false);
        }

        // 导出Excel
        private void barBtnExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                var dtExport = (DataTable)this.gcOCN_CLJBXX.DataSource;
                if (dtExport == null || dtExport.Rows.Count < 1)
                {
                    MessageBox.Show("当前没有数据可以下载！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var dialogResult = saveFileDialog1.ShowDialog(this);
                if (dialogResult == DialogResult.OK)
                {
                    var options = new XlsExportOptions() { TextExportMode = TextExportMode.Value, ExportMode = XlsExportMode.SingleFile };
                    this.gcOCN_CLJBXX.ExportToXls(saveFileDialog1.FileName, options);
                    var excelBuilder = new ExcelHelper(saveFileDialog1.FileName);
                    excelBuilder.DeleteColumns(1, 1);
                    excelBuilder.SaveFile();
                    if (MessageBox.Show("保存成功，是否打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 刷新
        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            refrashCurrentPage();
        }

        // 查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchLocal(1);
        }

        // 清除查询条件
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbTYMC.Text = string.Empty;
            this.tbCLXH.Text = string.Empty;
            this.tbSCOCN.Text = string.Empty;
            this.tbXTOCN.Text = string.Empty;
            this.cbRLLX.Text = string.Empty;
        }

        // 首页
        private void btnFirPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(1);
        }

        // 上一页
        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(--pageNum);
        }

        // 下一页
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(++pageNum);
        }

        // 尾页
        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(pageCou);
        }

        // 刷新
        private void refrashCurrentPage()
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 0) SearchLocal(pageNum);
        }

        // 是否显示全部
        private void ceQueryAll_CheckedChanged(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }

        // 查询
        private void SearchLocal(int pageNum)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                //获取总数目
                int dataCount = queryCount();
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    var dt = queryByPage(pageNum);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    this.gcOCN_CLJBXX.DataSource = dt;
                    this.gvOCN_CLJBXX.BestFitColumns();
                    int pageSize = Convert.ToInt32(this.spanNumber.Text);
                    int pageCount = dataCount / pageSize;
                    if (dataCount % pageSize > 0) pageCount++;
                    int dataLast;
                    if (pageNum == pageCount)
                        dataLast = dataCount;
                    else
                        dataLast = pageSize * pageNum;
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", (pageSize * (pageNum - 1) + 1), dataLast);
                    this.txtPage.Text = String.Format("{0}/{1}", pageNum, pageCount);
                }
                else
                {
                    var dt = queryAll();
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    this.gcOCN_CLJBXX.DataSource = dt;
                    this.gvOCN_CLJBXX.BestFitColumns();
                    this.lblSum.Text = String.Format("共{0}条", dataCount);
                    this.labPage.Text = String.Format("当前显示{0}至{1}条", 1, dataCount);
                    this.txtPage.Text = String.Format("{0}/{1}", 1, 1);
                }
                if (dataCount == 0)
                {
                    this.labPage.Text = "当前显示0至0条";
                    this.txtPage.Text = "0/0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        // 获取总数
        private int queryCount()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"SELECT COUNT(*) FROM OCN_CLJBXX WHERE 1=1 AND OPERATION!='4' ");
            if (!string.IsNullOrEmpty(tbTYMC.Text))
            {
                sql.AppendFormat(" AND (TYMC like '%{0}%')", tbTYMC.Text);
            }
            if (!string.IsNullOrEmpty(tbCLXH.Text))
            {
                sql.AppendFormat(" AND (CLXH like '%{0}%')", tbCLXH.Text);
            }
            if (!string.IsNullOrEmpty(tbSCOCN.Text))
            {
                sql.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSCOCN.Text);
            }
            if (!string.IsNullOrEmpty(tbXTOCN.Text))
            {
                sql.AppendFormat(" AND (XT_OCN like '%{0}%')", tbXTOCN.Text);
            }
            if (!string.IsNullOrEmpty(cbRLLX.Text))
            {
                sql.AppendFormat(" AND (RLLX like '%{0}%')", cbRLLX.Text);
            }
            var count = OracleHelper.ExecuteScalar(OracleHelper.conn, sql.ToString());
            return count != null ? Convert.ToInt32(count.ToString()) : 0;
        }

        // 获取当前页数据
        private DataTable queryByPage(int pageNum)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(tbTYMC.Text))
            {
                sqlWhere.AppendFormat(" AND (TYMC like '%{0}%')", tbTYMC.Text);
            }
            if (!string.IsNullOrEmpty(tbCLXH.Text))
            {
                sqlWhere.AppendFormat(" AND (CLXH like '%{0}%')", tbCLXH.Text);
            }
            if (!string.IsNullOrEmpty(tbSCOCN.Text))
            {
                sqlWhere.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSCOCN.Text);
            }
            if (!string.IsNullOrEmpty(tbXTOCN.Text))
            {
                sqlWhere.AppendFormat(" AND (XT_OCN like '%{0}%')", tbXTOCN.Text);
            }
            if (!string.IsNullOrEmpty(cbRLLX.Text))
            {
                sqlWhere.AppendFormat(" AND (RLLX like '%{0}%')", cbRLLX.Text);
            }
            string sqlVins = string.Format(@"select * from OCN_CLJBXX where 1=1 AND OPERATION!='4' {0}", sqlWhere);
            string sqlStr = string.Format(@"select * from (select F.*,ROWNUM RN from ({0}) F where ROWNUM<={1}) where RN>{2}", sqlVins, pageSize * pageNum, pageSize * (pageNum - 1));
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        // 获取全部数据
        private DataTable queryAll()
        {
            StringBuilder sqlStr = new StringBuilder();
            sqlStr.Append(@"SELECT * FROM OCN_CLJBXX WHERE 1=1 AND OPERATION!='4' ");
            if (!string.IsNullOrEmpty(tbTYMC.Text))
            {
                sqlStr.AppendFormat(" AND (TYMC like '%{0}%')", tbTYMC.Text);
            }
            if (!string.IsNullOrEmpty(tbCLXH.Text))
            {
                sqlStr.AppendFormat(" AND (CLXH like '%{0}%')", tbCLXH.Text);
            }
            if (!string.IsNullOrEmpty(tbSCOCN.Text))
            {
                sqlStr.AppendFormat(" AND (SC_OCN like '%{0}%')", tbSCOCN.Text);
            }
            if (!string.IsNullOrEmpty(tbXTOCN.Text))
            {
                sqlStr.AppendFormat(" AND (XT_OCN like '%{0}%')", tbXTOCN.Text);
            }
            if (!string.IsNullOrEmpty(cbRLLX.Text))
            {
                sqlStr.AppendFormat(" AND (RLLX like '%{0}%')", cbRLLX.Text);
            }
            DataSet ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr.ToString(), null);
            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        // 状态列的数据显示文本
        private void dvOCN_CLJBXX_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName.Equals("OPERATION"))
            {
                switch (e.Value.ToString().Trim())
                {
                    case "0":
                        e.DisplayText = "Excel导入";
                        break;
                    case "1":
                        e.DisplayText = "新增";
                        break;
                    case "2":
                        e.DisplayText = "复制新增";
                        break;
                    case "3":
                        e.DisplayText = "修改";
                        break;
                    case "4":
                        e.DisplayText = "删除";
                        break;
                    default:
                        e.DisplayText = "异常";
                        break;
                }
            }
        }

        //初始化详细信息
        private void setControlValue(CLJBXXParamForm cljbxx, string cName, String val, bool enable)
        {
            if (cName == null || "" == cName)
            {
                return;
            }

            Control[] c = cljbxx.Controls.Find(cName, true);
            if (c.Length > 0)
            {
                c[0].Text = val;
                c[0].Enabled = enable;
            }
        }
        //双击行
        private void gcOCN_CLJBXX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //双击行默认选中
            ColumnView cv = (ColumnView)gcOCN_CLJBXX.FocusedView;
            DataRowView dr = (DataRowView)cv.GetFocusedRow();
            if (dr == null)
            {
                return;
            }
            if (dr.Row["OPERATION"].Equals("4"))
            {
                MessageBox.Show(String.Format("OCN为{0}：已经删除，无法进行此操作！", dr.Row["SC_OCN"]), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CLJBXXParamForm cljbxxParamForm = new CLJBXXParamForm() { Text = "整车数据修改" };
            this.setControlValue(cljbxxParamForm, "teSC_OCN", dr.Row["SC_OCN"].ToString(), false);
            this.setControlValue(cljbxxParamForm, "teXT_OCN", dr.Row["XT_OCN"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teMI_XT_OCN", dr.Row["MI_XT_OCN"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teTYMC", dr.Row["TYMC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teCLXH", dr.Row["CLXH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "tePFBZ", dr.Row["PFBZ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeSFJKQC", dr.Row["SFJKQC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teQCSCQY", dr.Row["QCSCQY"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teJKQCZJXS", dr.Row["JKQCZJXS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teJCJGMC", dr.Row["JCJGMC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teBGBH", dr.Row["BGBH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teBAH", dr.Row["BAH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "dtCLZZRQ", string.IsNullOrEmpty(dr.Row["CLZZRQ"].ToString()) == true ? string.Empty : Convert.ToDateTime(dr.Row["CLZZRQ"].ToString()).ToString("yyyy/MM/dd"), true);
            this.setControlValue(cljbxxParamForm, "cbeCLZL", dr.Row["CLZL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeYYC", dr.Row["YYC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeQDXS", dr.Row["QDXS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZWPS", dr.Row["ZWPS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZGCS", dr.Row["ZGCS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teEDZK", dr.Row["EDZK"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teLTGG", dr.Row["LTGG"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teLJ", dr.Row["LJ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZJ", dr.Row["ZJ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeRLLX", dr.Row["RLLX"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teYHDYBAH", dr.Row["YHDYBAH"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZCZBZL", dr.Row["ZCZBZL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZDSJZZL", dr.Row["ZDSJZZL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teZHGKRLXHL", dr.Row["ZHGKRLXHL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teRLXHLMBZ", dr.Row["RLXHLMBZ"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teJDBZMBZ4", dr.Row["JDBZMBZ4"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "cbeBSQXS", dr.Row["BSQXS"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "tePL", dr.Row["PL"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "teCDDQDMSZHGKXHLC", dr.Row["CDDQDMSZHGKXHLC"].ToString(), true);
            this.setControlValue(cljbxxParamForm, "labVERSION", "整车数据版本号：V" + dr.Row["VERSION"], true);
            cljbxxParamForm.ShowDialog();
            if (cljbxxParamForm.DialogResult == DialogResult.Cancel) this.refrashCurrentPage();
        }

      
    }
}
