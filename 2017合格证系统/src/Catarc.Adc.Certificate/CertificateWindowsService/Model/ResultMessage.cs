using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertificateWindowsService.Model
{
    /// <summary>
    /// 返回结果
    /// </summary>
    class ResultMessage
    {
        /// <summary>
        /// 结果成功标志
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 返回结果信息
        /// </summary>
        public string Message { get; set; }
    }
}
