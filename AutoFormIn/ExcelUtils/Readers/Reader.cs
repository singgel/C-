using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ExcelUtils.Readers
{
    public interface Reader
    {
        /// <summary>
        /// 获取单元格值
        /// </summary>
        /// <param name="row">行数，0开始</param>
        /// <param name="col">列数，0开始</param>
        /// <param name="table"></param>
        /// <returns>字符串格式返回值</returns>
        String getCellValue(int row, int col, DataTable table);

        /// <summary>
        /// 获取一行中的某一列值
        /// </summary>
        /// <param name="col">列数，0开始</param>
        /// <param name="row">DataRow 数据</param>
        /// <returns></returns>
        String getCellValue(int col, DataRow row);
    }
}
