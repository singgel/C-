using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using DevExpress.XtraEditors;
using Oracle.ManagedDataAccess.Client;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using Catarc.Adc.NewEnergyApproveSys.Properties;

namespace Catarc.Adc.NewEnergyApproveSys.PopForm
{
    public partial class RejectInfoForm : DevExpress.XtraEditors.XtraForm
    {
        readonly DataTable dt = new DataTable();//需要处理的数据
        readonly int APP_STATUS = 0;//0：未审批/10：一审驳回、11：一审通过、13：A通过、14：A驳回/20：二审驳回、21：二审通过/30：三审驳回、31：三审通过

        public RejectInfoForm()
        {
            InitializeComponent();
        }

        private void RejectInfoForm_Load(object sender, EventArgs e)
        {
            try
            {
                var dtReason = OracleHelper.ExecuteDataSet(OracleHelper.conn, "select * from SYS_DIC where DIC_TYPE='驳回原因'").Tables[0];
                if (dtReason != null && dtReason.Rows.Count > 0)
                {
                    this.comDetail.Properties.Items.Clear();
                    var reasonArr = dtReason.AsEnumerable().Select(d => d.Field<string>("DIC_NAME")).Distinct().ToArray();
                    this.comDetail.Properties.Items.AddRange(reasonArr);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("驳回原因获取异常：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public RejectInfoForm(DataTable dt, int APP_STATUS)
        {
            InitializeComponent();
            this.dt = dt;
            this.APP_STATUS = APP_STATUS;
        }

        //保存
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.comDetail.Text) && string.IsNullOrEmpty(this.memoDetail.Text))
            {
                XtraMessageBox.Show("驳回原因不可以为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string msg = string.Empty;
            if (APP_STATUS == 0)
            {
                msg = saveDeatils_1();
            }
            else if (APP_STATUS == 11)
            {
                msg = saveDeatils_2();
            }
            else if (APP_STATUS == 21)
            {
                msg = saveDeatils_3();
            }
            using (var mf = new MessageForm(msg) { Text = "审批操作结果" })
            {
                mf.ShowDialog();
            }
            this.DialogResult = DialogResult.OK;
        }

        private string saveDeatils_1()
        {
            string msg = string.Empty;
            using (var con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        var guidList = dt.AsEnumerable().Select(d => d.Field<string>("GUID")).ToList();
                        while (guidList.Count > 0)
                        {
                            var guidArrSkip = guidList.Take(1000);
                            stringBuilder.AppendFormat(" or GUID in('{0}')", string.Join("','", guidArrSkip));
                            if (guidList.Count > 999)
                            {
                                guidList.RemoveRange(0, 999);
                            }
                            else
                            {
                                guidList.RemoveRange(0, guidList.Count);
                            }
                        }
                        var guidStr = string.Format("and ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                        int num_13 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_1_B='{0}',APP_TIME_1_B=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_B='{2}',APP_STATUS=11,APP_MONEY=null where APP_STATUS=13 and APP_NAME_1_A!='{0}' {3}", Settings.Default.LocalUserName, DateTime.Now, this.comDetail.Text + this.memoDetail.Text, guidStr));
                        msg += String.Format("一审B身份驳回,进入二审的数据有{0}条{1}", num_13, Environment.NewLine);
                        int num_14 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_1_B='{0}',APP_TIME_1_B=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_B='{2}',APP_STATUS=10,APP_MONEY=null where APP_STATUS=14 and APP_NAME_1_A!='{0}' {3}", Settings.Default.LocalUserName, DateTime.Now, this.comDetail.Text + this.memoDetail.Text, guidStr));
                        msg += String.Format("一审B身份驳回,审核结束的数据有{0}条{1}", num_14, Environment.NewLine);
                        int num_0 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_1_A='{0}',APP_TIME_1_A=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_A='{2}',APP_STATUS=14,APP_MONEY=null where APP_STATUS=0 {3}", Settings.Default.LocalUserName, DateTime.Now, this.comDetail.Text + this.memoDetail.Text, guidStr));
                        msg += String.Format("一审A身份驳回,未进入二审的数据有{0}条{1}", num_0, Environment.NewLine);
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        msg += String.Format("异常信息:{0}，操作异常！{1}", ex.Message, Environment.NewLine);
                    }
                }
            }
            return msg;
        }

        private string saveDeatils_2()
        {
            string msg = string.Empty;
            using (var con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        var guidList = dt.AsEnumerable().Select(d => d.Field<string>("GUID")).ToList();
                        while (guidList.Count > 0)
                        {
                            var guidArrSkip = guidList.Take(1000);
                            stringBuilder.AppendFormat(" or GUID in('{0}')", string.Join("','", guidArrSkip));
                            if (guidList.Count > 999)
                            {
                                guidList.RemoveRange(0, 999);
                            }
                            else
                            {
                                guidList.RemoveRange(0, guidList.Count);
                            }
                        }
                        var guidStr = string.Format("and ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                        int num_11 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_2='{0}',APP_TIME_2=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_2='{2}',APP_STATUS=20,APP_MONEY=null where APP_STATUS=11 {3}", Settings.Default.LocalUserName, DateTime.Now, this.comDetail.Text + this.memoDetail.Text, guidStr));
                        msg += String.Format("二审驳回,审核结束的数据有{0}条{1}", num_11, Environment.NewLine);
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        msg += String.Format("异常信息：{0}，操作异常！{1}", ex.Message, Environment.NewLine);
                    }
                }
            }
            return msg;
        }

        private string saveDeatils_3()
        {
            string msg = string.Empty;
            using (var con = new OracleConnection(OracleHelper.conn))
            {
                con.Open();
                using (var tra = con.BeginTransaction())
                {
                    try
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        var guidList = dt.AsEnumerable().Select(d => d.Field<string>("GUID")).ToList();
                        while (guidList.Count > 0)
                        {
                            var guidArrSkip = guidList.Take(1000);
                            stringBuilder.AppendFormat(" or GUID in('{0}')", string.Join("','", guidArrSkip));
                            if (guidList.Count > 999)
                            {
                                guidList.RemoveRange(0, 999);
                            }
                            else
                            {
                                guidList.RemoveRange(0, guidList.Count);
                            }
                        }
                        var guidStr = string.Format("and ({0})", stringBuilder.ToString().Trim().TrimStart('o').TrimStart('r'));
                        int num_21 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_3='{0}',APP_TIME_3=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_3='{2}',APP_STATUS=30,APP_MONEY=null where APP_STATUS=21 {3}", Settings.Default.LocalUserName, DateTime.Now, this.comDetail.Text + this.memoDetail.Text, guidStr));
                        msg += String.Format("三审驳回,审核结束的数据有{0}条{1}", num_21, Environment.NewLine);
                        tra.Commit();
                    }
                    catch (Exception ex)
                    {
                        tra.Rollback();
                        msg += String.Format("异常信息:{0}，操作异常！{1}", ex.Message, Environment.NewLine);
                    }
                }
            }
            return msg;
        }

        //关闭
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}