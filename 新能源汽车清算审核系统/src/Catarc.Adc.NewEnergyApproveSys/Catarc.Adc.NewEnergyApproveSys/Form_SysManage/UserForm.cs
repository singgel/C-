using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using DevExpress.XtraEditors;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using System.Collections;
using Oracle.ManagedDataAccess.Client;
using DevExpress.XtraSplashScreen;
using Catarc.Adc.NewEnergyApproveSys.DevForm;

namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    public partial class UserForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static Hashtable htRole = GridControlHelper.GetRoleName("");
        public static Hashtable htDept = GridControlHelper.GetDeptName("");

        public UserForm()
        {
            InitializeComponent();
        }

        //添加用户
        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (UserInfoForm uf = new UserInfoForm() { StartPosition = FormStartPosition.CenterScreen, Text = "新增用户信息" })
            {
                uf.ShowDialog();
            }
            SearchLocal(1);
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnAdd.Caption));
               
        }
        // 删除
        private void barBtnLocalDel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvUser.PostEditor();

            DataView dv = (DataView)this.gvUser.DataSource;
            string selectedParamEntityIds = "";
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if ((bool)dv.Table.Rows[i]["check"])
                    {
                        selectedParamEntityIds += String.Format(",'{0}'", dv.Table.Rows[i]["ID"]);
                    }
                }
            }
            if ("" == selectedParamEntityIds)
            {
                XtraMessageBox.Show("请选择要删除的数据！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (XtraMessageBox.Show("确定要删除吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.OK)
            {
                return;
            }

            using (OracleConnection conn = new OracleConnection(OracleHelper.conn))
            {
                conn.Open();
                try
                {
                    if ("" != selectedParamEntityIds)
                    {
                        var dsName = OracleHelper.ExecuteDataSet(OracleHelper.conn, string.Format("select USERNAME from SYS_USERINFO where ID in({0})", selectedParamEntityIds.Substring(1)), null);
                        var nameArr = dsName.Tables[0].AsEnumerable().Select(d => d.Field<string>("USERNAME")).Distinct().ToArray();
                        if (nameArr.Contains(Properties.Settings.Default.LocalUserName))
                        {
                            XtraMessageBox.Show("要删除的数据不允许包含所登录账号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        string sql = String.Format(@"delete from SYS_USERINFO where ID in ({0})", selectedParamEntityIds.Substring(1));
                        OracleHelper.ExecuteNonQuery(conn, sql, null);

                        //记录操作日志
                        LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.barBtnLocalDel.Caption));
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message, "报错");
                }
            }

            SearchLocal(1);
        }
        //修改密码
        private void btnUpdatePWD_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.gvUser.PostEditor();

            DataView dv = (DataView)this.gvUser.DataSource;
            string selectedParamEntityIds = "";
            int iCheckNum = 0;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if ((bool)dv.Table.Rows[i]["check"])
                    {
                        selectedParamEntityIds = dv.Table.Rows[i]["ID"].ToString();
                        iCheckNum++;
                        if (iCheckNum > 1)
                        {
                            XtraMessageBox.Show("请选择一条用户信息！", "提示");
                            return;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(selectedParamEntityIds))
            {
                UpdateUserPWD uif = new UpdateUserPWD(selectedParamEntityIds);
                uif.StartPosition = FormStartPosition.CenterScreen;
                uif.Text = "修改密码";
                uif.ShowDialog();
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnUpdatePWD.Caption));
             
            }
            else
            {
                XtraMessageBox.Show("请选择一条用户信息！", "提示");
            }
        }

        // 全选
        private void barBtnSelectAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.gvUser.FocusedRowHandle = 0;
                this.gvUser.FocusedColumn = this.gvUser.Columns[1];
                GridControlHelper.SelectItem(this.gvUser, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // 取消全选
        private void barBtnClearAll_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                this.gvUser.FocusedRowHandle = 0;
                this.gvUser.FocusedColumn = this.gvUser.Columns[1];
                GridControlHelper.SelectItem(this.gvUser, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //刷新
        private void btnRefurbish_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            SearchLocal(1);
        }
        //修改用户
        private void btnUpdateUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvUser.PostEditor();

            DataView dv = (DataView)this.gvUser.DataSource;
            string selectedParamEntityIds = "";
            int iCheckNum = 0;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    if ((bool)dv.Table.Rows[i]["check"])
                    {
                        selectedParamEntityIds = dv.Table.Rows[i]["ID"].ToString();
                        iCheckNum++;
                        if (iCheckNum > 1)
                        {
                            XtraMessageBox.Show("请选择一条用户信息！", "提示");
                            return;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(selectedParamEntityIds))
            {
                using (UserInfoForm uif = new UserInfoForm(selectedParamEntityIds) { StartPosition = FormStartPosition.CenterScreen, Text = "修改用户信息" })
                {
                    uif.ShowDialog();
                }
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnUpdate.Caption));
             
            }
            else
            {
                XtraMessageBox.Show("请选择一条用户信息！", "提示");
            }
        }
        // 查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchLocal(1);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.tbName.Text = string.Empty;
            this.cbRole.Text = string.Empty;
            this.cbStatue.Text = string.Empty;
        }

        #region 内部方法
        // 查询
        public void SearchLocal(int pageNum)
        {
            try
            {
                SplashScreenManager.ShowForm(typeof(DevWaitForm));
                string strParam = queryParam();
                //获取总数目
                int dataCount = queryCount(strParam);
                //是否显示全部
                if (this.spanNumber.Enabled)
                {
                    DataTable dt = queryByPage(pageNum, strParam);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    gcUser.DataSource = dt;
                    //gvUser.BestFitColumns();
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
                    DataTable dt = queryAll(strParam);
                    dt.Columns.Add("check", System.Type.GetType("System.Boolean"));
                    dt.Columns["check"].ReadOnly = false;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        dt.Rows[i]["check"] = false;
                    }
                    gcUser.DataSource = dt;
                    //gvUser.BestFitColumns();
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
                XtraMessageBox.Show("查询出现错误：" + ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SplashScreenManager.CloseForm();
            }
        }

        //获取总数
        private int queryCount(string strParam)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT COUNT(U.ID)  ");
            sql.Append("FROM SYS_USERINFO U  ");
            sql.Append("LEFT JOIN SYS_ROLE R ON U.USERROLEID=R.ID  ");
            sql.Append("LEFT JOIN SYS_DEPARTMENT D ON D.ID=U.USERDEPTID where 1=1  ");
            sql.Append(strParam);


            DataSet ds = new DataSet();

            ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql.ToString(), null);

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                return Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }

        //获取当前页数据
        private DataTable queryByPage(int pageNum, string sqlWhere)
        {
            int pageSize = Convert.ToInt32(this.spanNumber.Text);

            DataSet ds = new DataSet();

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT U.ID,U.USERLOGINNAME,U.USERNAME,R.ROLENAME  ");
            sql.Append(",D.DEPTNAME,U.USERTEL,U.USEREMAIL,TO_CHAR(U.MODIFYTIME,'yyyy-MM-dd hh24:mi:ss') MODIFYTIME  ");
            sql.Append(",CASE  WHEN U.STATUS=0 THEN '有效' ELSE '无效' END STATUE  ");
            sql.Append("FROM SYS_USERINFO U  ");
            sql.Append("LEFT JOIN SYS_ROLE R ON U.USERROLEID=R.ID  ");
            sql.Append("LEFT JOIN SYS_DEPARTMENT D ON D.ID=U.USERDEPTID where 1=1  ");
            sql.Append(sqlWhere.ToString());

            string sqlStr = string.Format(@"select * from (select F.*,ROWNUM RN from ({0}) F where ROWNUM<={1}) where RN>{2}", sql, pageSize * pageNum, pageSize * (pageNum - 1));

            ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sqlStr, null);

            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }

        //获取全部数据
        private DataTable queryAll(string strParam)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT U.ID,U.USERLOGINNAME,U.USERNAME,R.ROLENAME  ");
            sql.Append(",D.DEPTNAME,U.USERTEL,U.USEREMAIL,TO_CHAR(U.MODIFYTIME,'yyyy-MM-dd hh24:mi:ss') MODIFYTIME  ");
            sql.Append(",CASE  WHEN U.STATUS=0 THEN '有效' ELSE '无效' END STATUE  ");
            sql.Append("FROM SYS_USERINFO U  ");
            sql.Append("LEFT JOIN SYS_ROLE R ON U.USERROLEID=R.ID  ");
            sql.Append("LEFT JOIN SYS_DEPARTMENT D ON D.ID=U.USERDEPTID where 1=1  ");
            sql.Append(strParam);
            

            DataSet ds = new DataSet();

            ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql.ToString(), null);

            if (ds != null && ds.Tables.Count > 0)
            {
                return ds.Tables[0];
            }
            else
            {
                return null;
            }
        }
       
        //查询条件
        private string queryParam()
        {
            var sql = new StringBuilder();
            if (!string.IsNullOrEmpty(tbName.Text))
            {
                sql.AppendFormat(" AND U.USERNAME like '%{0}%' ", tbName.Text);
            }
            if (!string.IsNullOrEmpty(cbRole.Text))
            {
                sql.AppendFormat(" AND R.ROLENAME like '%{0}%' ", cbRole.Text);
            }
            if (!string.IsNullOrEmpty(cbStatue.Text))
            {
                if (cbStatue.Text.Trim().Equals("有效"))
                    sql.AppendFormat(" AND U.STATUS=0 ");
                else if (cbStatue.Text.Trim().Equals("无效"))
                    sql.AppendFormat(" AND U.STATUS=1 ");
            }
            return sql.ToString();
        }

        #endregion

        private void UserForm_Load(object sender, EventArgs e)
        {
            string[] roleList = new string[htRole.Count + 1];
            roleList[0] = "";
            htRole.Values.CopyTo(roleList, 1);
            this.cbRole.Properties.Items.AddRange(roleList);
        }
        //下一页
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(++pageNum);
        }
        //尾页
        private void btnLastPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            int pageCou = Convert.ToInt32(txtPage.Text.Substring(txtPage.Text.LastIndexOf("/") + 1, (txtPage.Text.Length - txtPage.Text.IndexOf("/") - 1)));
            if (pageNum < pageCou) SearchLocal(pageCou);
        }
        //上一页
        private void btnPrePage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 1) SearchLocal(--pageNum);
        }
        //首页
        private void btnFirPage_Click(object sender, EventArgs e)
        {
            int pageNum = Convert.ToInt32(txtPage.Text.Substring(0, txtPage.Text.LastIndexOf("/")));
            if (pageNum > 0) SearchLocal(1);
        }
        //是否显示全部
        private void ceQueryAll_CheckedChanged(object sender, EventArgs e)
        {
            this.spanNumber.Enabled = !ceQueryAll.Checked;
        }




        
    }
}