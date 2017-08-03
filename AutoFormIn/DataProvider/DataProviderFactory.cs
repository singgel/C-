using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assistant.DataProviders
{
    public class DataProviderFactory
    {
        private const string CCC = "CCC", GH = "国环", BH = "北环";
        private static Hashtable _innerTypeList;

        static DataProviderFactory()
        {
            _innerTypeList = new Hashtable();
            RegisterProvider("HCBM", GH, typeof(HCBM.GHDataProvider));
            RegisterProvider("HCBM", BH, typeof(HCBM.GHDataProvider));
            RegisterProvider("SHFY", GH, typeof(SHFY.GHDataProvider));
            RegisterProvider("SHFY", BH, typeof(SHFY.BHDataProvider));
            RegisterProvider("SHFY", CCC, typeof(SHFY.CCCCDataProvider));
            RegisterProvider("VOLVO", GH, typeof(VOLVO.GHDataProvider));
            RegisterProvider("VOLVO", BH, typeof(VOLVO.BHDataProvider));
            RegisterProvider("VOLVO", CCC, typeof(VOLVO.CCCCDataProvider));
            RegisterProvider("GQFYT", GH, typeof(VOLVO.GHDataProvider));
            RegisterProvider("GQFYT", BH, typeof(VOLVO.BHDataProvider));
            RegisterProvider("GQFYT", CCC, typeof(VOLVO.CCCCDataProvider));
        }
        /// <summary>
        /// 为企业下指定填报器类型创建数据提供类。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="fillerType">填报器类型。</param>
        /// <exception cref="System.NotImplementedException">注册的数据提供类中不包含可见的无参构造函数。</exception>
        /// <returns>返回数据提供类，若找到；否则返回null;</returns>
        public static IDataProvider CreateProvider(string entName, string fillerType)
        {
            Type type = _innerTypeList[string.Format("{0}{1}", entName, fillerType)] as Type;
            if (type == null)
                return null;
            System.Reflection.ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new NotImplementedException("指定的数据提供类没有可见的无参构造函数！");
            return constructor.Invoke(null) as IDataProvider;
        }
        /// <summary>
        /// 为企业的指定填报器类型注册数据提供类。
        /// </summary>
        /// <param name="entName">企业名称。</param>
        /// <param name="fillerType">填报器类型。</param>
        /// <param name="type">执行数据提供逻辑的类类型。</param>
        public static void RegisterProvider(string entName, string fillerType, Type type)
        {
            if(type.GetInterface("IDataProvider") == null)
                throw new ArgumentException("指定的类型未实现Assistant.DataProviders.IDataProvider接口!", "type");
            _innerTypeList.Add(string.Format("{0}{1}", entName, fillerType), type);
        }
    }
}
