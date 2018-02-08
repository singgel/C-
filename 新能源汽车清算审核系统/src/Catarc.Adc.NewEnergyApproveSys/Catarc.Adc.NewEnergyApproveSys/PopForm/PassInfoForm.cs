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
    public partial class PassInfoForm : DevExpress.XtraEditors.XtraForm
    {
        readonly DataTable dt = new DataTable();//需要处理的数据
        readonly int APP_STATUS = 0;//0：未审批/10：一审驳回、11：一审通过、13：A通过、14：A驳回/20：二审驳回、21：二审通过/30：三审驳回、31：三审通过
        readonly int APP_STATUS_31 = 0;//0：三审 批准通过,修改金额隐藏/1：三审 修改金额并通过

        public PassInfoForm()
        {
            InitializeComponent();
        }

        public PassInfoForm(DataTable dt,int APP_STATUS)
        {
            InitializeComponent();
            this.dt = dt;
            this.APP_STATUS = APP_STATUS;
            if (APP_STATUS == 21)
            {
                this.labDetail.Visible = false;
                this.txtDetail.Visible = false;
                this.groupBox1.Location = new System.Drawing.Point(12, 30);
                this.groupBox2.Text = "注意,推荐金额有以下标准:";
                this.groupBox2.ForeColor = System.Drawing.Color.Red;
                LabelControl lab = new LabelControl()
                {
                    Location = new System.Drawing.Point(176, 5),
                    Size = new System.Drawing.Size(50, 14),
                    Text = "您确定要审核通过所选中审批数据吗？"
                };
                this.xtraScrollableControl2.Controls.Add(lab);
            }
        }

        public PassInfoForm(DataTable dt, int APP_STATUS, int APP_STATUS_31)
        {
            InitializeComponent();
            this.dt = dt;
            this.APP_STATUS = APP_STATUS;
            this.APP_STATUS_31 = APP_STATUS_31;
            if (APP_STATUS == 21)
            {
                this.groupBox1.Size = new System.Drawing.Size(563, 70);
                this.groupBox2.Size = new System.Drawing.Size(563, 70);
                this.groupBox2.Text = "注意,推荐金额有以下标准:";
                this.groupBox2.ForeColor = System.Drawing.Color.Red;
                LabelControl lab = new LabelControl()
                {
                    Location = new System.Drawing.Point(176, 5),
                    Size = new System.Drawing.Size(50, 14),
                    Text = "您确定要审核通过所选中审批数据吗？"
                };
                this.xtraScrollableControl2.Controls.Add(lab);
            }
        }

        private void PassInfoForm_Load(object sender, EventArgs e)
        {
            showDetail();
            showBtbz();
        }

        //申请金额
        private void showDetail()
        {
            if (dt != null || dt.Rows.Count > 0)
            {
                var detailArr = dt.AsEnumerable().Select(d => d.Field<string>("SQBZBZ")).Distinct().ToArray();
                if (detailArr.Length > 1)
                {
                    if (APP_STATUS != 21 && XtraMessageBox.Show("申请补贴不同，是否继续！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.Cancel;
                        return;
                    }
                }
                for (int i = 0; i < detailArr.Length; i++)
                {
                    LabelControl lab = new LabelControl() 
                    {
                        Name = detailArr[i],
                        Text = detailArr[i], 
                        Location = new System.Drawing.Point(4 + i * 55, 7), 
                        Size = new System.Drawing.Size(50, 14),
                        ForeColor = System.Drawing.Color.Blue
                    };
                    lab.Click += lab_Click;
                    this.xtraScrollableControl1.Controls.Add(lab);
                }
            }
        }

        //补贴标准
        private void showBtbz()
        {
            if (dt != null || dt.Rows.Count > 0)
            {
                var detailArr = dt.AsEnumerable().Select(d => d.Field<string>("BTBZ")).Distinct().ToArray();
                if (detailArr.Length > 1)
                {
                    if (APP_STATUS != 21 && XtraMessageBox.Show("补贴标准不同，是否继续！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.Cancel;
                        return;
                    }
                }
                var btbzList = new List<string>();
                for (int i = 0; i < detailArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(detailArr[i])) continue;
                    btbzList.AddRange(detailArr[i].Split(';'));
                }
                btbzList = btbzList.Distinct().ToList();
                for (int j = 0; j < btbzList.Count; j++)
                {
                    LabelControl lab = new LabelControl()
                    {
                        Name = btbzList[j],
                        Text = btbzList[j],
                        Location = new System.Drawing.Point(4 + j * 55, 7),
                        Size = new System.Drawing.Size(50, 14),
                        ForeColor = System.Drawing.Color.Blue
                    };
                    lab.Click += lab_Click;
                    this.xtraScrollableControl2.Controls.Add(lab);
                }
            }
        }

        //点击使用该申请金额
        private void lab_Click(object sender, EventArgs e)
        {
            var value = ((DevExpress.XtraEditors.LabelControl)sender).Text;
            this.txtDetail.Text = value;
        }

        //保存
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.txtDetail.Visible && string.IsNullOrEmpty(this.txtDetail.Text))
            {
                XtraMessageBox.Show("请输入金额！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                if (APP_STATUS_31 == 1)//修改金额并通过
                {
                    msg = saveDeatils_3();
                }
                else
                {
                    msg = saveDeatils_3();
                }
            }
            using (var mf = new MessageForm(msg) { Text = "审批操作结果" })
            {
                mf.ShowDialog();
            }
            this.DialogResult = DialogResult.OK;
        }

        //一审
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
                        int num_13_Diff = OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("update DB_INFOMATION set APP_NAME_1_B='{0}',APP_TIME_1_B=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_B='{2}',APP_STATUS=11 where APP_STATUS=13 and APP_NAME_1_A!='{0}'and APP_RESULT_1_A!='{2}' {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        msg += String.Format("一审B身份通过,进入二审的数据有{0}条{1}", num_13_Diff, Environment.NewLine);
                        int num_13_Same = OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("update DB_INFOMATION set APP_NAME_1_B='{0}',APP_TIME_1_B=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_B='{2}',APP_MONEY='{2}',APP_STATUS=21 where APP_STATUS=13 and APP_NAME_1_A!='{0}' and APP_RESULT_1_A='{2}' {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        msg += String.Format("一审B身份通过,进入三审的数据有{0}条{1}", num_13_Same, Environment.NewLine);
                        int num_14 = OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("update DB_INFOMATION set APP_NAME_1_B='{0}',APP_TIME_1_B=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_B='{2}',APP_STATUS=11 where APP_STATUS=14 and APP_NAME_1_A!='{0}' {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        msg += String.Format("一审B身份通过,进入二审的数据有{0}条{1}", num_14, Environment.NewLine);
                        int num_0 = OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("update DB_INFOMATION set APP_NAME_1_A='{0}',APP_TIME_1_A=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_1_A='{2}',APP_STATUS=13 where APP_STATUS=0 {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        msg += String.Format("一审A身份通过,未进入二审的数据有{0}条{1}", num_0, Environment.NewLine);
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

        //二审
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
                        int num_11 = OracleHelper.ExecuteNonQuery(tra, String.Format("update DB_INFOMATION set APP_NAME_2='{0}',APP_TIME_2=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_2='{2}' ,APP_MONEY='{2}',APP_STATUS=21 where APP_STATUS=11 {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        msg += String.Format("二审通过,进入三审的数据有{0}条{1}", num_11, Environment.NewLine);
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

        //三审
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
                        int num_21 = -1;
                        if (APP_STATUS_31 == 1 && this.txtDetail.Visible)
                        {
                            num_21 = OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("update DB_INFOMATION set APP_NAME_3='{0}',APP_TIME_3=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_3='{2}',APP_MONEY='{2}',APP_STATUS=31 where APP_STATUS=21 {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        }
                        else
                        {
                            num_21 = OracleHelper.ExecuteNonQuery(OracleHelper.conn, String.Format("update DB_INFOMATION set APP_NAME_3='{0}',APP_TIME_3=to_date('{1}','yyyy-mm-dd hh24:mi:ss'),APP_RESULT_3='{2}',APP_STATUS=31 where APP_STATUS=21 {3}", Settings.Default.LocalUserName, DateTime.Now, this.txtDetail.Text, guidStr));
                        }
                        msg += String.Format("三审通过,审核结束的数据有{0}条{1}", num_21, Environment.NewLine);
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