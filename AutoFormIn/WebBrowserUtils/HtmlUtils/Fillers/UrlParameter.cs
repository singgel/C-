using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 表示页面参数的存储位置。
    /// </summary>
    public class UrlParameter
    {
        /// <summary>
        /// 表示当前页面参数是否在“公共页面.xlsx”中。
        /// </summary>
        public bool IsPublicUrl;
        /// <summary>
        /// 表示当前页面参数在Excel文件中的哪个标签。
        /// </summary>
        public string LabelName; 
    }
}
