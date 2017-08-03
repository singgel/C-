using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    /// <summary>
    /// 表示向网页中填报数据时的参数（即规则中的条目）。
    /// </summary>
    public class FillParameter
    {
        public string Name, Type, Id, Value, OnClick, ParameterName, href, SearchString, TableName, FrameId, SplitExpr, FindCode;
        public bool CanAdd;
        public string SearchType;
        public string ReturnIndex;
        //是否在填写完成之后将参数值从数据文件data中删除
        public bool CanDelete, IsRequired;
        //当参数值包含于网页备选值是否可以选中
        public bool CanContain;
    }
}
