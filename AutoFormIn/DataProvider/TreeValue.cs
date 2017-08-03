using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    [System.Diagnostics.DebuggerDisplay("Name = {Name}, Suffix = {Suffix}")]
    public class TreeValue
    {
        private TreeValue _parent;
        private Hashtable _values;
        private List<TreeValue> _children;
        private string _name;
        private string _suffix;
        private bool _isAppendNode;
        /// <summary>
        /// 获取当前节点是否为追加节点。
        /// </summary>
        public bool IsAppendNode
        {
            get { return _isAppendNode; }
        }
        /// <summary>
        /// 获取当前节点的名称。
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        /// <summary>
        /// 获取子级节点。
        /// </summary>
        public List<TreeValue> Children
        {
            get { return _children; }
        }
        /// <summary>
        /// 获取当前节点的可填报数据。
        /// </summary>
        public Hashtable Values
        {
            get { return _values; }
        }
        /// <summary>
        /// 获取此节点的父级节点。
        /// </summary>
        public TreeValue Parent
        {
            get { return _parent; }
        }
        /// <summary>
        /// 获取当前节点名称的后缀。
        /// </summary>
        public string Suffix
        {
            get { return _suffix; }
        }

        public TreeValue(string name)
            : this(name, null)
        {
        }

        public TreeValue(string name, string suffix)
        {
            _name = name;
            if (string.IsNullOrEmpty(suffix))
                _suffix = null;
            else
                _suffix = suffix;
            _children = new List<TreeValue>();
            _isAppendNode = string.IsNullOrEmpty(suffix) == false;
            _values = new Hashtable();
        }

        public TreeValue(string name, string parentName, string suffix)
            : this(name, suffix)
        {
            if(parentName != null)
                _parent = new TreeValue(parentName);
        }
        /// <summary>
        /// 向此节点添加一个子级。
        /// </summary>
        /// <param name="child">要添加的子节点。</param>
        public void AddChild(TreeValue child)
        {
            _children.Add(child);
            child._parent = this;
        }
        /// <summary>
        /// 从此节点中查找具有指定名称和后缀的子孙节点。
        /// </summary>
        /// <param name="nodeName">节点名称。</param>
        /// <param name="suffix">节点后缀。</param>
        /// <returns></returns>
        public TreeValue Find(string nodeName, string suffix)
        {
            if (this._name == nodeName && this._suffix == suffix)
                return this;
            TreeValue result = null;
            foreach (var item in _children)
            {
                result = item.Find(nodeName, suffix);
                if (result != null)
                    break;
            }
            return result;
        }
        /// <summary>
        /// 当前节点的子级是否包含指点的节点名称。
        /// </summary>
        /// <param name="nodeName">节点名称。</param>
        /// <returns></returns>
        public bool Contains(string nodeName)
        {
            foreach (var item in _children)
            {
                if (item._name == nodeName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 复制指定节点的树结构复制到当前节点。
        /// </summary>
        /// <param name="node"></param>
        public void CopyFrom(TreeValue node)
        {
            foreach (var item in node.Children)
            {
                TreeValue value = new TreeValue(item.Name);
                value.CopyFrom(item);
                this.AddChild(value);
            }
        }
        /// <summary>
        /// 从当前节点中查找具有指定名称和后缀的子节点。
        /// </summary>
        /// <param name="nodeName">节点名称。</param>
        /// <param name="suffix">节点后缀。</param>
        /// <returns></returns>
        public TreeValue FindChild(string nodeName, string suffix)
        {
            suffix = string.IsNullOrEmpty(suffix) ? null : suffix;
            foreach (var item in _children)
            {
                if (item._name == nodeName && item._suffix == suffix)
                    return item;
            }
            return null;
        }
        /// <summary>
        /// 将当前节点重命名。
        /// </summary>
        /// <param name="name"></param>
        public void Rename(string name)
        {
            _name = name;
        }

        public IEnumerable<TreeValue> GetTreeNodeValue(string nodeName)
        {
            foreach (var item in _children)
            {
                if (item.Name == nodeName)
                    yield return item;
            }
        }
        /// <summary>
        /// 使用路径选择节点。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TreeValue Select(string path)
        {
            string[] pathes = string.IsNullOrEmpty(path) ? null : path.Split('#');
            if (pathes == null)
                return null;
            else
            {
                TreeValue selected = this;
                foreach(string dir in pathes)
                {
                    if (string.Format("{0}{1}", selected._name, string.IsNullOrEmpty(selected._suffix) ? "" : string.Format("_{0}", selected._suffix)) == dir)
                        continue;
                    int index = dir.IndexOf("_");
                    if (index == -1)
                        selected = selected.FindChild(dir, null);
                    else if (dir.Length > index + 1)
                        selected = selected.FindChild(dir.Substring(0, index), dir.Substring(index + 1, dir.Length - index - 1));
                    else
                        selected = selected.FindChild(dir, null);
                    if (selected == null)
                        return null;
                }
                return selected;
            }
        }
    }
}
