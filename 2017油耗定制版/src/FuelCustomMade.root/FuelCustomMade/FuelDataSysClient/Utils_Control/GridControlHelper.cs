using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraGrid.Views.Grid;
using System.Data;

namespace FuelDataSysClient.Utils_Control
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
            GridView dgv = obj as GridView;
            DataView dv = (DataView)dgv.DataSource;
            if (dv != null)
            {
                for (int i = 0; i < dv.Count; i++)
                {
                    dv.Table.Rows[i]["check"] = flg;
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
    }
}
