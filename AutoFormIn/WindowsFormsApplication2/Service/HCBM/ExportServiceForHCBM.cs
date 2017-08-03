using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ExcelUtils;
using Assistant.Container;
using Assistant.Entity;
using Assistant.Matcher;
using Assistant.ConfigCodeService;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace Assistant.Service
{
    class ExportServiceForHCBM
    {
        /// <summary>
        /// 将车辆信息导出为excel文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="listParams"></param>
        public static void exportCarParams(String filePath, DataTable dt)
        {
            MemoryStream ms = ExcelUtil.RenderToExcel(dt);
            ExcelUtil.SaveToFile(ms, filePath);
        }
        /// <summary>
        /// 直接将申报参数填写会导入的excel文件中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="list"></param>
        public static void exportCarParams(String filePath, CarParams cps)
        {
            ExcelUtil.exportCarParamsForHCBM(filePath, cps.packageCode, cps.ConfigCode);
        }
    }
}
