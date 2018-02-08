using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraGrid.Views.Grid;
using System.Data;
using Catarc.Adc.NewEnergyApproveSys.DBUtils;
using System.Collections;
using Catarc.Adc.NewEnergyApproveSys.Form_SysManage;

namespace Catarc.Adc.NewEnergyApproveSys.ControlUtils
{
    public class GridControlHelper
    {
        /// <summary>
        /// gridcontrol控件的全选与取消全选
        /// </summary>
        /// <param name="obj">gridcontrol控件</param>
        /// <param name="flg">全选的是与否</param>
        public static void SelectItem(object obj, bool flg)
        {
            if (obj == null) return;
            GridView dgv = obj as GridView;
            DataView dv = (DataView)dgv.DataSource;
            if (dv != null)
            {
                if (dgv.ActiveFilterCriteria != null)
                {
                    var filteredDataView = ((DataView)dgv.DataSource).Table.Copy().DefaultView;
                    filteredDataView.RowFilter =
                        DevExpress.Data.Filtering.CriteriaToWhereClauseHelper.GetDataSetWhere(dgv.ActiveFilterCriteria);
                    var filterRecords = filteredDataView.ToTable();
                    var detailArr = filterRecords.AsEnumerable().Select(d => d.Field<string>("GUID")).Distinct().ToArray();
                    for (int i = 0; i < dv.Count; i++)
                    {
                        if (detailArr.Contains(dv.Table.Rows[i]["GUID"]))
                            dv.Table.Rows[i]["check"] = flg;
                    }
                }
                else
                {
                    for (int i = 0; i < dv.Count; i++)
                    {
                        dv.Table.Rows[i]["check"] = flg;
                    }
                }
                dgv.RefreshData();
            }
        }

        /// <summary>
        /// gridcontrol控件的选中的行
        /// </summary>
        /// <param name="obj">gridcontrol控件</param>
        /// <returns></returns>
        public static DataTable SelectedItems(object obj)
        {
            if (obj == null) return null;
            GridView dgv = obj as GridView;
            dgv.PostEditor();
            var dataSource = (DataView)dgv.DataSource;
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
            return dtSelected;
        }

        // 获取角色名称
        public static Hashtable GetRoleName(string type)
        {
            Hashtable htRole = new Hashtable();
            try
            {
                string sql = string.Empty;
                DataSet ds = new DataSet();

                sql = @"SELECT R.ID,R.ROLENAME FROM SYS_ROLE R";
                ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);

                DataTable dt = ds.Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["ROLENAME"] != null)
                        {
                           // RoleList.Add(dr["ROLENAME"].ToString());
                            htRole.Add(dr["ID"].ToString(), dr["ROLENAME"].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return htRole;
        }
        // 获取部门名称
        public static Hashtable GetDeptName(string type)
        {
            Hashtable htDept = new Hashtable();
            try
            {
                string sql = string.Empty;
                DataSet ds = new DataSet();

                sql = @"SELECT D.ID,D.DEPTNAME FROM SYS_DEPARTMENT D ";
                ds = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);

                DataTable dt = ds.Tables[0];

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["DEPTNAME"] != null)
                        {
                            //DeptList.Add(dr["ROLENAME"].ToString());
                            htDept.Add(dr["ID"].ToString(), dr["DEPTNAME"].ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return htDept;
        }

        //读取MENU
        public static List<MenuModel> GetMenusData()
        {
            List<MenuModel> listModel = new List<MenuModel>();
            string sql = string.Empty;

            sql = "SELECT ID,PARENTID,ORDERID,MANUNAME FROM SYS_MENUS  ORDER BY ID";

            DataSet dsMenu = OracleHelper.ExecuteDataSet(OracleHelper.conn, sql, null);
            if (dsMenu != null && dsMenu.Tables.Count > 0)
            {
                int dtCount = dsMenu.Tables[0].Rows.Count;
                MenuModel model = new MenuModel();

                for (int i = 0; i < dtCount; i++)
                {
                    model = new MenuModel();
                    model.ID = dsMenu.Tables[0].Rows[i]["ID"].ToString();
                    model.MenuName = dsMenu.Tables[0].Rows[i]["MANUNAME"].ToString();
                    model.ParentID = dsMenu.Tables[0].Rows[i]["PARENTID"].ToString();
                    model.OrderID = Int32.Parse(dsMenu.Tables[0].Rows[i]["ORDERID"].ToString());
                    listModel.Add(model);
                }
            }
            return listModel;
        }
    }

    public class MenuModel
    {
        #region 字段属性

        private string id;
        /// <summary>
        /// Gets or sets the menu ID.
        /// </summary>
        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string parentID;
        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        /// <value>The parent ID.</value>
        public string ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }

        private int orderID;
        /// <summary>
        /// Gets or sets the order ID.
        /// </summary>
        /// <value>The order ID.</value>
        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        private string menuName;
        /// <summary>
        /// Gets or sets the name of the menu.
        /// </summary>
        /// <value>The name of the menu.</value>
        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }

        #endregion

        public MenuModel() { }

        protected MenuModel(MenuModel model)
        {
            this.id = model.id;
            this.menuName = model.menuName;
            this.orderID = model.orderID;
            this.parentID = model.parentID;
        }
    }

}
