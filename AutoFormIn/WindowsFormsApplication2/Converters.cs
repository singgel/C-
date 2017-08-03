using Assistant.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Comparer;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace Assistant
{
    public class FileStatusConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                FileStatus status = (FileStatus)value;
                switch (status)
                {
                case FileStatus.Deleted:
                    return "已删除";
                case FileStatus.DeleteFlag:
                    return "删除标记";
                case FileStatus.New:
                    return "新文件";
                case FileStatus.Update:
                    return "更新";
                case FileStatus.Uploaded:
                    return "已上传";
                default:
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RuleItemStatusConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                RuleItemStatus status = (RuleItemStatus)value;
                switch (status)
                {
                case RuleItemStatus.Added:
                    return "新增元素";
                case RuleItemStatus.Removed:
                    return "已从新页面中移除";
                default:
                    return "未更改";
                }
            }
            catch
            {
                return "未更改";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RecordTypeConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                RecordType type = (RecordType)value;
                switch (type)
                {
                case RecordType.Success:
                    return "成功";
                case RecordType.Failed:
                    return "失败";
                default:
                    return "对话框消息";
                }
            }
            catch
            {
                return "对话框消息";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ElementTypeConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                ElementType type = (ElementType)value;
                switch (type)
                {
                case ElementType.Button:
                    return "按钮";
                case ElementType.CheckBox:
                    return "复选框";
                case ElementType.Radio:
                    return "单选框";
                case ElementType.Select:
                    return "下拉选择框";
                case ElementType.Text:
                    return "文本框";
                case ElementType.File:
                    return "文件";
                default:
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
