using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace Assistant.DataProviders
{
    /// <summary>
    /// 数据提供者接口。
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// 获取当前数据提供者是否可进行数据验证。
        /// </summary>
        bool CanValidation { get; }
        /// <summary>
        /// 源数据文件
        /// </summary>
        string DataSourceFile { get; set; }
        /// <summary>
        /// 从指定的数据源中获取数据
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        object ProvideData(object state);
        /// <summary>
        /// 获取是否展示界面交互。
        /// </summary>
        bool AllowAlternately { get; }
        /// <summary>
        /// 展示用户界面。
        /// </summary>
        /// <returns></returns>
        bool ShowWindow();
        /// <summary>
        /// 清理生成的临时文件或占用的资源。
        /// </summary>
        /// <returns></returns>
        void Clean();
        /// <summary>
        /// 获取数据转换器。
        /// </summary>
        /// <returns></returns>
        ValueConverter GetConverter();
        /// <summary>
        /// 验证数据。
        /// </summary>
        /// <returns></returns>
        bool Validate();
    }
}
