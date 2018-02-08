using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace CertificateMvcApplication.Utils
{
    public class Dt2ModelUtils
    {
        /// <summary>
        /// 根据dataTable转成对应的object类型
        /// </summary>
        /// <typeparam name="T">object类型</typeparam>
        /// <param name="table">dataTable数据</param>
        /// <returns>object</returns>
        public static T GetEntity<T>(DataTable table) where T : new()
        {
            T entity = new T();
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (row.Table.Columns.Contains(item.Name))
                    {
                        if (DBNull.Value != row[item.Name])
                        {
                            item.SetValue(entity, Convert.ChangeType(row[item.Name], item.PropertyType), null);
                        }
                    }
                }
            }
            return entity;
        }
        /// <summary>
        /// 根据dataTable转成对应的IList类型
        /// </summary>
        /// <typeparam name="T">IList</typeparam>
        /// <param name="table">dataTable数据</param>
        /// <returns>IList</object></returns>
        public static IList<T> GetEntities<T>(DataTable table) where T : new()
        {
            IList<T> entities = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T entity = new T();
                foreach (var item in entity.GetType().GetProperties())
                {
                    item.SetValue(entity, Convert.ChangeType(row[item.Name], item.PropertyType), null);
                }
                entities.Add(entity);
            }
            return entities;
        }
    }
}