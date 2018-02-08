using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelDataSysClient.Model
{
    public class QueryResult
    {
        //当前第几页
        public string curPage { get; set; }
        //总共数据记录数
        public string totalContentNum { get; set; }
        //总共页面记录数
        public string totalPageNum { get; set; }
        //所有油耗信息
        public List<ResultMap> resultMap;

        public QueryResult()
        {

        }
    }
}
