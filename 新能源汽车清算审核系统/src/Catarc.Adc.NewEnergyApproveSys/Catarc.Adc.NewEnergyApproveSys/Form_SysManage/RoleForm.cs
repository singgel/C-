using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.Collections;
using Catarc.Adc.NewEnergyApproveSys.ControlUtils;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;

namespace Catarc.Adc.NewEnergyApproveSys.Form_SysManage
{
    public partial class RoleForm : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public static Hashtable htRole = new Hashtable();
        public static Hashtable htDept = new Hashtable();
        public static List<MenuModel> listMenu = new List<MenuModel>();
        private int _isModify = -1;
        private readonly List<string> lstCheckedMenuID = new List<string>();
        private string strCheckedMenuID = string.Empty;

        public RoleForm()
        {
            InitializeComponent();
            
        }

        private void RoleForm_Load(object sender, EventArgs e)
        {
            htRole = GridControlHelper.GetRoleName("");
            htDept = GridControlHelper.GetDeptName("");
            string[] roleList = new string[htRole.Count + 1];
            roleList[0] = "";
            htRole.Values.CopyTo(roleList, 1);
            this.cbRole.Properties.Items.AddRange(roleList);
            listMenu = GridControlHelper.GetMenusData();

            ResfurbishData();
        }
        //保存
        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.gvRole.CloseEditor();
            DataRow dr = null;
            if (_isModify != -1)
            {
                if (_isModify == 0)
                {
                    dr = this.gvRole.GetDataRow(this.gvRole.DataRowCount - 1);
                }
                else
                {
                    dr = this.gvRole.GetDataRow(_isModify - 1);
                }
                if (dr != null)
                {
                    lstCheckedMenuID.Clear();
                    strCheckedMenuID = string.Empty;
                    if (treeList1.Nodes.Count > 0)
                    {
                        foreach (TreeListNode root in treeList1.Nodes)
                        {
                            GetCheckedOfficeID(root);
                        }
                    }
                    string roleId = dr["ID"].ToString();
                    string roleName = dr["ROLENAME"].ToString();

                    string insSql = string.Format(@"INSERT INTO SYS_ROLE(ID,ROLENAME,AUTHORITY) VALUES('{0}','{1}','{2}') ", roleId, roleName, strCheckedMenuID);

                    string upSql = string.Format(@"UPDATE SYS_ROLE SET AUTHORITY='{0}',ROLENAME='{2}' WHERE ID='{1}'", strCheckedMenuID, roleId, roleName);


                    int count = OracleHelper.ExecuteNonQuery(OracleHelper.conn, _isModify == 0 ? insSql : upSql, null);
                    if (count > 0)
                    {
                        XtraMessageBox.Show("操作成功！", "提示");
                        _isModify = -1;
                        this.gvRole.OptionsBehavior.Editable = false;
                        this.treeList1.OptionsBehavior.Editable = false;
                    }
                }
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnSave.Caption));
            }
            else
            {
                XtraMessageBox.Show("请选择一条用户信息！", "提示");
            }
        }
        //刷新
        private void btnRefresh_ItemClick(object sender, ItemClickEventArgs e)
        {
            ResfurbishData();
        }
        //删除
        private void btnDelete_ItemClick(object sender, ItemClickEventArgs e)
        {

            DataRow dr = this.gvRole.GetFocusedDataRow();
            if (dr != null)
            {
                DialogResult dialog = XtraMessageBox.Show(String.Format("确认删除角色【{0}】吗？", dr["ROLENAME"]), "提示", MessageBoxButtons.OKCancel);
                if (dialog == DialogResult.OK)
                {
                    string sql = String.Format(@"DELETE FROM SYS_ROLE WHERE ID='{0}' ", dr["ID"]);
                    int count = OracleHelper.ExecuteNonQuery(OracleHelper.conn, sql, null);

                    if (count > 0)
                    {
                        ResfurbishData();
                        XtraMessageBox.Show("操作成功！", "提示");
                    }
                    //记录操作日志
                    LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnDelete.Caption));
               
                }
            }
            else
            {
                XtraMessageBox.Show("请选择一条用户信息！", "提示");
            }
        }
        //添加
        private void btnADD_ItemClick(object sender, ItemClickEventArgs e)
        {
            string strRoleID = string.Empty;
            if (!string.IsNullOrEmpty(this.cbRole.Text))
            {
                foreach (System.Collections.DictionaryEntry de in htRole)
                {
                    if (de.Value.ToString() == this.cbRole.Text.Trim())
                    {
                        strRoleID = de.Key.ToString();
                        break;
                    }
                }
            }
            else
            {
                strRoleID = "";
            }

            DataTable dt = QueryRoleData(strRoleID);
            this.gcRole.DataSource = null;
            this.gcRole.DataSource = dt;
            this.gvRole.BestFitColumns();
            this.gvRole.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.gvRole.OptionsView.ColumnAutoWidth = true;
            DataTable table = new DataTable();
            table = dt;
            DataRow newRow = table.NewRow();
            newRow["ID"] = Guid.NewGuid().ToString();
            table.Rows.Add(newRow);
            this.gcRole.BeginUpdate();
            this.gvRole.BeginUpdate();
            this.gcRole.DataSource = table;
            this.gvRole.EndUpdate();
            this.gcRole.EndUpdate();
            this.gvRole.ShowEditor();
            this.gvRole.OptionsBehavior.Editable = true;
            this.gvRole.FocusedRowHandle = this.gvRole.DataRowCount - 1;
            lstCheckedMenuID.Clear();
            this.treeList1.DataSource = listMenu;
            this.treeList1.KeyFieldName = "ID";
            treeList1.ParentFieldName = "ParentID";
            treeList1.OptionsView.ShowCheckBoxes = true;
            treeList1.ExpandAll();
            this.treeList1.OptionsBehavior.Editable = true;
            RecursionCheckedNodes(treeList1.Nodes);
            _isModify = 0;
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnADD.Caption));
            
        }
        //修改
        private void btnUpdate_ItemClick(object sender, ItemClickEventArgs e)
        {
            DataRow dr = this.gvRole.GetFocusedDataRow();
            if (dr != null)
            {
                string no = dr["ID"].ToString();
                this.gvRole.OptionsBehavior.Editable = true;
                RefurbishTree();
                this.treeList1.OptionsBehavior.Editable = true;
                _isModify = this.gvRole.FocusedRowHandle + 1;
                //记录操作日志
                LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnUpdate.Caption));
               
            }
            else
            {
                XtraMessageBox.Show("请选择一条用户信息！", "提示");
            }
        }
        //取消
        private void btnCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_isModify != -1)
            {
                this.gvRole.CloseEditor();
                //权限树不可编辑
                this.treeList1.OptionsBehavior.Editable = false;
            }
            ResfurbishData();
            _isModify = -1;
        }
        //查询
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strRoleID = string.Empty;
            if (!string.IsNullOrEmpty(this.cbRole.Text))
            {
                foreach (System.Collections.DictionaryEntry de in htRole)
                {
                    if (de.Value.ToString() == this.cbRole.Text.Trim())
                    {
                        strRoleID = de.Key.ToString();//得到key
                        break;//退出foreach遍历
                    }
                }
            }
            else
            {
                strRoleID = "";
            }
            DataTable dt = QueryRoleData(strRoleID);
            this.gcRole.DataSource = null;
            this.gcRole.DataSource = dt;
            //列自适应
            this.gvRole.BestFitColumns();
            this.gvRole.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.gvRole.OptionsView.ColumnAutoWidth = true;
            lstCheckedMenuID.Clear();
            this.treeList1.DataSource = listMenu;
            this.treeList1.KeyFieldName = "ID";
            treeList1.ParentFieldName = "ParentID";
            ////显示复选框  
            treeList1.OptionsView.ShowCheckBoxes = true;
            //展开树结构
            treeList1.ExpandAll();
            //不可编辑
            this.treeList1.OptionsBehavior.Editable = false;
            //根据角色ID查询权限菜单
            DataRow dr = this.gvRole.GetFocusedDataRow();
            if (dr != null)
            {
                var vAUTHORITY = from q in dt.AsEnumerable()
                                 where q.Field<string>("ID") == dr["ID"].ToString()
                                 select q;

                DataTable dtAut = vAUTHORITY.CopyToDataTable();

                string authority = dtAut.Rows[0]["AUTHORITY"].ToString();
                string[] arr = authority.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        lstCheckedMenuID.Add(arr[i]);
                    }
                }
                RecursionCheckedNodes(treeList1.Nodes);
            }
            //记录操作日志
            LogUtils.ReviewLogManager.ReviewLog(Properties.Settings.Default.LocalUserName, String.Format("{0}-{1}", this.Text, this.btnSearch.Text.Trim()));
        }

        #region 内部方法

        public void ResfurbishData()
        {
            string strRoleID = string.Empty;
            if (!string.IsNullOrEmpty(this.cbRole.Text))
            {
                foreach (System.Collections.DictionaryEntry de in htRole)
                {
                    if (de.Value.ToString() == this.cbRole.Text.Trim())
                    {
                        strRoleID = de.Key.ToString();//得到key
                        break;//退出foreach遍历
                    }
                }
            }
            else
            {
                strRoleID = "";
            }

            DataTable dt = QueryRoleData(strRoleID);
            this.gcRole.DataSource = null;
            this.gcRole.DataSource = dt;

            //列自适应
            this.gvRole.BestFitColumns();
            this.gvRole.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.gvRole.OptionsView.ColumnAutoWidth = true;

            lstCheckedMenuID.Clear();

            this.treeList1.DataSource = listMenu;
            this.treeList1.KeyFieldName = "ID";
            treeList1.ParentFieldName = "ParentID";
            ////显示复选框  
            treeList1.OptionsView.ShowCheckBoxes = true;
            //展开树结构
            treeList1.ExpandAll();
            //不可编辑
            this.treeList1.OptionsBehavior.Editable = false;

            //根据角色ID查询权限菜单
            DataRow dr = this.gvRole.GetFocusedDataRow();
            if (dr != null)
            {
                var vAUTHORITY = from q in dt.AsEnumerable()
                                 where q.Field<string>("ID") == dr["ID"].ToString()
                                 select q;
                DataTable dtAut = vAUTHORITY.CopyToDataTable();

                string authority = dtAut.Rows[0]["AUTHORITY"].ToString();
                string[] arr = authority.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    if (!string.IsNullOrEmpty(arr[i]))
                    {
                        lstCheckedMenuID.Add(arr[i]);
                    }
                }
                RecursionCheckedNodes(treeList1.Nodes);
            }
            else
            {
                RecursionCheckedNodes(treeList1.Nodes);
            }
        }

        public void RefurbishTree()
        {
            lstCheckedMenuID.Clear();
            this.treeList1.DataSource = listMenu;
            this.treeList1.KeyFieldName = "ID";
            treeList1.ParentFieldName = "ParentID";
            ////显示复选框  
            treeList1.OptionsView.ShowCheckBoxes = true;
            //展开树结构
            treeList1.ExpandAll();

            //根据用户ID查询权限菜单
            DataRow dr = this.gvRole.GetFocusedDataRow();
            if (dr != null)
            {
                DataTable dt = QueryRoleData(dr["ID"].ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    string authority = dt.Rows[0]["AUTHORITY"].ToString();
                    string[] arr = authority.Split(',');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(arr[i]))
                        {
                            lstCheckedMenuID.Add(arr[i]);
                        }
                    }
                    RecursionCheckedNodes(treeList1.Nodes);
                }
            }
        }
        /// <summary>
        ///  查询角色信息
        /// </summary>
        /// <returns></returns>
        private DataTable QueryRoleData(string id)
        {
            string sql = string.Empty;
            if (string.IsNullOrEmpty(id))
            {
                sql = "SELECT R.ID,R.ROLENAME,AUTHORITY FROM SYS_ROLE R ORDER BY R.ID";
            }
            else
            {
                sql = String.Format("SELECT R.ID,R.ROLENAME,AUTHORITY FROM SYS_ROLE R WHERE ID='{0}' ", id);
            }
            DataSet dsLocal = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            if (dsLocal == null || dsLocal.Tables.Count == 0)
            {
                return null;
            }
            return dsLocal.Tables[0];
        }
        /// <summary>
        /// 遍历菜单并选中
        /// </summary>
        /// <param name="Nodes"></param>
        private void RecursionCheckedNodes(TreeListNodes Nodes)
        {
            foreach (TreeListNode node in Nodes)
            {
                string id = node.GetValue("ID").ToString();
                if (lstCheckedMenuID.Contains(id))
                {
                    node.Checked = true;
                }
                else
                {
                    node.Checked = false;
                }

                if (node.Nodes.Count > 0)
                {
                    RecursionCheckedNodes(node.Nodes);
                }
            }
        }
        // 获取选择状态的数据主键ID集合  
        private void GetCheckedOfficeID(TreeListNode parentNode)
        {
            if (parentNode.Nodes.Count == 0)
            {
                return;//递归终止  
            }

            foreach (TreeListNode node in parentNode.Nodes)
            {
                if (node.CheckState == CheckState.Checked)
                {
                    MenuModel model = treeList1.GetDataRecordByNode(node) as MenuModel;
                    if (model != null)
                    {
                        string id = model.ID;
                        lstCheckedMenuID.Add(id);
                        strCheckedMenuID += id + ",";
                    }
                }
                GetCheckedOfficeID(node);
            }
        }


        #endregion
        //选中行改变
        private void gvRole_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            RefurbishTree();
        }
        //权限树是否可以编辑
        private void treeList1_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            if ((_isModify == 0 && this.gvRole.FocusedRowHandle == this.gvRole.DataRowCount - 1)
               || (_isModify != -1 && this.gvRole.FocusedRowHandle == _isModify - 1))
            {
                e.CanCheck = true;
            }
            else
            {
                e.CanCheck = false;
            }
        }

        private void gvRole_ShowingEditor(object sender, CancelEventArgs e)
        {
            if (_isModify == 0 && this.gvRole.FocusedRowHandle == this.gvRole.DataRowCount - 1)
            {
                e.Cancel = false;
                //权限树可编辑
                this.treeList1.OptionsBehavior.Editable = true;
            }
            else if (_isModify != -1 && this.gvRole.FocusedRowHandle == _isModify - 1)
            {
                e.Cancel = false;
                //权限树可编辑
                this.treeList1.OptionsBehavior.Editable = true;
            }
            else
            {
                e.Cancel = true;
                //权限树不可编辑
                this.treeList1.OptionsBehavior.Editable = false;
            }
        }

        private void treeList1_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            SetCheckedChildNodes(e.Node, e.Node.CheckState);
            SetCheckedParentNodes(e.Node, e.Node.CheckState);
        }

        //选择某一节点时,该节点的子节点全部选择  取消某一节点时,该节点的子节点全部取消选择  
        private void SetCheckedChildNodes(TreeListNode node, CheckState check)
        {
            for (int i = 0; i < node.Nodes.Count; i++)
            {
                node.Nodes[i].CheckState = check;
                SetCheckedChildNodes(node.Nodes[i], check);
            }
        }

        // 某节点的子节点全部选择时,该节点选择   某节点的子节点未全部选择时,该节点不选择  
        private void SetCheckedParentNodes(TreeListNode node, CheckState check)
        {
            if (node.ParentNode != null && check == CheckState.Checked)
            {
                node.ParentNode.CheckState = check;
                SetCheckedParentNodes(node.ParentNode, check);
            }
        }

        private void gvRole_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (this.gvRole.GetDataRow(e.RowHandle) == null)
            {
                return;
            }
            if (e.RowHandle == gvRole.FocusedRowHandle)
            {
                e.Appearance.ForeColor = Color.White;
                e.Appearance.BackColor = Color.LightBlue;
            }
        }

    }
}