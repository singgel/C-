using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebBrowserUtils
{
    // 用于标记所有html元素中的常量标识符
    class HtmlConstants
    {
        public static class ElementValueType
        {
            /// <summary>
            /// checkbox的返回值类别选中
            /// </summary>
            public static String CHECK_BOX_CHECKED_ON = "checked";

            /// <summary>
            /// checkbox的返回值类别不选中
            /// </summary>
            public static String CHECK_BOX_CHECKED_OFF = "none";
        }

        public static class ElementType
        {
            /// <summary>
            /// 文本框的录入值的参数类型
            /// </summary>
            public static String TEXT_BOX_VALUE = "Value";
        }

        public static class ActionType
        {
            /// <summary>
            /// 点击动作
            /// </summary>
            public static String CLICK = "click";
        }

        public static class ActionReturnValues
        {
            /// <summary>
            /// 操作成功值
            /// </summary>
            public static String SUCCESS = "success";
            public static String ERROR = "error";
        }
    }
}
