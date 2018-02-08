using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using FuelDataSysClient.Tool;

namespace FuelDataSysClient.Form_DBManager
{
    public partial class TemplateForm : Form
    {
        public string tempVin;//模板VIN

        public TemplateForm()
        {
            InitializeComponent();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string newVin = tbCopyVin.Text.Trim().ToUpper();
            if (newVin == "")
            {
                MessageBox.Show("请输入备案号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (IsVinExit(newVin))
            {
                MessageBox.Show(string.Format("VIN:{0}数据已经录入", newVin), "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            char bi;
            DataCheck dc = new DataCheck();
            if (!dc.CheckCLSBDH(this.tbCopyVin.Text.Trim().ToUpper(), out bi))
            {
                if (bi == '-')
                {
                    MessageBox.Show("请核对【备案号(VIN)】为17位字母或者数字!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("【备案号(VIN)】校验失败！第9位应为:'" + bi + "'", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            OleDbConnection conn = new OleDbConnection(AccessHelper.conn);
            conn.Open();
            OleDbTransaction ts = conn.BeginTransaction();

            try
            {
                string jbxxsql = @"INSERT INTO FC_CLJBXX
                                    (V_ID,VIN,HGSPBM,USER_ID,QCSCQY,JKQCZJXS,CLXH,CLZL,RLLX,ZCZBZL,
                                    ZGCS,LTGG,ZJ,CLZZRQ,UPLOADDEADLINE,TYMC,YYC,ZWPS,ZDSJZZL,EDZK,LJ,
                                    QDXS,STATUS,JYJGMC,JYBGBH,QTXX,CREATETIME,UPDATETIME)
                                SELECT 
                                    NULL AS V_ID,@VIN AS VIN,t.HGSPBM,t.USER_ID,t.QCSCQY,t.JKQCZJXS,t.CLXH,t.CLZL,t.RLLX,t.ZCZBZL,
                                    t.ZGCS,t.LTGG,t.ZJ,t.CLZZRQ,t.UPLOADDEADLINE,t.TYMC,t.YYC,t.ZWPS,t.ZDSJZZL,t.EDZK,t.LJ,
                                    t.QDXS,'1' AS STATUS,t.JYJGMC,t.JYBGBH ,t.QTXX,@CREATETIME AS CREATETIME,@UPDATETIME AS UPDATETIME
                                FROM FC_CLJBXX t WHERE t.VIN=@TEMP_VIN";

                OleDbParameter creTime = new OleDbParameter("@CREATETIME", DateTime.Now);
                creTime.OleDbType = OleDbType.DBDate;
                OleDbParameter upTime = new OleDbParameter("@UPDATETIME", DateTime.Now);
                upTime.OleDbType = OleDbType.DBDate;

                OleDbParameter[] param = {
                                     new OleDbParameter("@VIN", tbCopyVin.Text.Trim().ToUpper()),
                                     creTime,
                                     upTime,
                                     new OleDbParameter("@TEMP_VIN", tempVin.ToUpper())
                                     };
                // 复制基本信息
                int jbxxcount = AccessHelper.ExecuteNonQuery(ts, jbxxsql, param);

                string entitysql = @"INSERT INTO RLLX_PARAM_ENTITY (PARAM_CODE,VIN,PARAM_VALUE,V_ID)
                                  SELECT 
                                    PARAM_CODE,@VIN,PARAM_VALUE,NULL AS V_ID
                                  FROM RLLX_PARAM_ENTITY WHERE VIN=@TEMP_VIN";
                OleDbParameter[] eneityparam = {
                                                new OleDbParameter("@VIN", tbCopyVin.Text.Trim().ToUpper()),
                                                new OleDbParameter("@TEMP_VIN", tempVin.ToUpper())
                                               };
                // 复制参数信息
                int entitycount = AccessHelper.ExecuteNonQuery(ts, entitysql, eneityparam);

                ts.Commit();
                MessageBox.Show("复制成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ts.Rollback();
                MessageBox.Show("复制失败："+ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsVinExit(string vin)
        {
            bool isVinExit = false;
            try
            {
                string sql = string.Format("select * from FC_CLJBXX where vin='{0}'", vin);

                DataSet ds = AccessHelper.ExecuteDataSet(AccessHelper.conn, sql, null);
                DataTable dt = ds.Tables[0];

                if (dt.Rows.Count > 0)
                {
                    isVinExit = true;
                }
            }
            catch (Exception)
            {
            }

            return isVinExit;
        }
    }
}
