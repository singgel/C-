using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    public enum ValueState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0x0,
        /// <summary>
        /// 拆分
        /// </summary>
        Splited = 0x1,
        /// <summary>
        /// 错误状态
        /// </summary>
        Error = 0x2
    }
}
