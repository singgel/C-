using Assistant.DataProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 表示CCC的填报参数值。
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Key = {Key}, Value = {Value}, AttachFile = {AttachFile}")]
    public class FillValue3C
    {
        private string publicAttachFile;
        private string attachFile;
        /// <summary>
        /// CCC参数编号
        /// </summary>
        public string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get;
            set;
        }
        /// <summary>
        /// 参数值。
        /// </summary>
        public string Value
        {
            get;
            set;
        }
        /// <summary>
        /// 原始字符串
        /// </summary>
        public string OriginString
        {
            get;
            set;
        }
        /// <summary>
        /// 是否使用鼠标双击选项
        /// </summary>
        public bool DoubleClick
        {
            get;
            set;
        }
        /// <summary>
        /// 参数分隔符
        /// </summary>
        public char[] Separators
        {
            get;
            set;
        }
        /// <summary>
        /// 附件（高优先级）
        /// </summary>
        public string AttachFile
        {
            get { return string.IsNullOrEmpty(attachFile) ? publicAttachFile : attachFile; }
        }
        /// <summary>
        /// 附件（低优先级）
        /// </summary>
        public string PublicAttachFile
        {
            get { return publicAttachFile; }
            set { publicAttachFile = value; }
        }

        public FillValue3C()
        {
        }
        /// <summary>
        /// 使用指定的参数值设置此CCC填报参数
        /// 
        /// </summary>
        /// <param name="value">格式：(参数值1;[参数值2];...[参数值n])(&amp;&amp;[附件1];[附件2];...[附件n])(备注：备注内容)。例如： "1&amp;&amp;1.jpg;2.jpg备注：备注信息"，表示将Value设置为"1"、AttachFile设置为"1.jpg;2.jpg"、Note设置为"备注信息"。</param>
        public void SetValue(string value)
        {
            OriginString = value;
             if (string.IsNullOrEmpty(value) == false)
            {
                int index = value.IndexOf("备注：");
                if (index == -1)
                    Value = value;
                else
                {
                    Value = value.Substring(0, index);
                    Note = index + 3 < value.Length ? value.Substring(index + 3) : "";
                }
                // 提取使用&&符号分割的附件
                string[] values = string.IsNullOrEmpty(Value) ? new string[0] : Value.Split(new string[] { "&&" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length >= 2)
                {
                    Value = values[0];
                    this.attachFile = values[1];
                }
            }
        }

        internal void SetAttachFile(string attachFile)
        {
            this.attachFile = attachFile;
        }
        /// <summary>
        /// 从指定的CCC填报参数复制信息。
        /// </summary>
        /// <param name="other"></param>
        public void CopyFrom(FillValue3C other)
        {
            this.Key = other.Key;
            this.DoubleClick = other.DoubleClick;
            this.PublicAttachFile = other.publicAttachFile;
            this.OriginString = other.OriginString;
            this.DoubleClick = other.DoubleClick;
            this.Separators = other.Separators;
        }
    }
}
