using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    /// <summary>
    /// 3C数据验证。
    /// </summary>
    public class DataValidator
    {
        /// <summary>
        /// 获取验证规则文件名称
        /// </summary>
        public string ValidateFileName
        {
            get;
            private set;
        }

        public DataValidator(string fileName)
        {
            ValidateFileName = fileName;
        }
        /// <summary>
        /// 验证指定的值对于指定的键值是否有效。
        /// </summary>
        /// <param name="key">CCC参数编码。</param>
        /// <param name="value">参数值。</param>
        /// <returns></returns>
        public virtual bool IsValid(string key, string value)
        {
            return true;
        }

        public virtual bool IsSpecial(string key, string value)
        {
            return false;
        }

        public virtual void ReadValidateRule()
        {
        }
    }
}
