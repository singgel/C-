using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 表示用于查找填报参数的关键字。
    /// </summary>
    public class FillParameterKey : IComparable<string>, IEquatable<string>
    {
        private string _key, _parameterType, _searchSting, _tableName;
        private bool _canAdd, _isRequired;
        //填充完成之后将参数值从data中删除
        private bool _canDelete;
        //当参数值包含于网页备选值是否可以选中
        private bool _canContain;
        public string Key
        {
            get { return _key; }
        }

        public string Type
        {
            get { return _parameterType; }
        }

        public bool CanAdd
        {
            get { return _canAdd; }
        }

        public bool IsRequired
        {
            get { return _isRequired; }
        }

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public string SearchString
        {
            get { return _searchSting; }
        }
        /// <summary>
        /// 初始化一个新的填报参数关键字。
        /// </summary>
        /// <param name="key">参数名称。</param>
        /// <param name="type">填报参数的元素类别。</param>
        /// <param name="canAdd">指示此填报参数是否为可新增参数。</param>
        /// <param name="searchString">指示此填报参数是否的查找方式（空字符表示非查找参数，“等于”为判等查找，其它字符串将使用正则表达式匹配）。</param>
        public FillParameterKey(string key, string type, bool canAdd, bool isRequired, string searchString)
        {
            _key = key;
            _parameterType = type;
            _canAdd = canAdd;
            _searchSting = searchString;
        }

        public FillParameterKey(string key, string type, bool canAdd, bool canDelete, bool isRequired, bool canContain, string searchString)
        {
            _key = key;
            _parameterType = type;
            _canAdd = canAdd;
            _searchSting = searchString;
            _canDelete = canDelete;
            _canContain = canContain;
        }

        public int CompareTo(string other)
        {
            return _key == null ? (other == null ? 0 : -1) : _key.CompareTo(other);
        }

        public bool Equals(string other)
        {
            return _key == other;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(_key) ? string.Empty.GetHashCode() : _key.GetHashCode();
        }
    }
}
