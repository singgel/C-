using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using WebBrowserUtils.ExtendWebBrowser;
using System.Windows.Forms;
using System.Windows;
using Assistant.DataProviders;

namespace WebBrowserUtils.HtmlUtils.Fillers
{
    public abstract class FillManagerBase : IDataProvider
    {
        private IDataProvider provider;
        /// <summary>
        /// 获取数据文件名称。
        /// </summary>
        string IDataProvider.DataSourceFile
        {
            get;
            set;
        }

        bool IDataProvider.AllowAlternately
        {
            get { return false; }
        }

        bool IDataProvider.ShowWindow()
        {
            return false;
        }

        /// <summary>
        /// 获取当前的填报类型。
        /// </summary>
        public virtual string FillType
        {
            get;
            set;
        }

        public string RuleFilePath
        {
            get;
            private set;
        }
        /// <summary>
        /// 获取或设置此填报管理器的数据提供程序。
        /// </summary>
        public IDataProvider DataProvider
        {
            get { return provider == null ? this : provider; }
            set { provider = value; }
        }

        public abstract IList<FillRecord> FillRecords
        {
            get;
        }

        public event EventHandler Finished;

        protected FillManagerBase(string dataFile, string ruleFilePath)
        {
            this.DataProvider.DataSourceFile = dataFile;
            this.RuleFilePath = ruleFilePath;
        }

        protected virtual void OnFinished(EventArgs e)
        {
            if (Finished != null)
                Finished(this, e);
        }

        public abstract void BeginFill();

        void IDataProvider.Clean()
        {
        }

        public abstract void EndFill();

        ValueConverter IDataProvider.GetConverter()
        {
            return new WebValueConverter();
        }

        protected abstract object GetData(object state);

        object IDataProvider.ProvideData(object state)
        {
            return this.GetData(state);
        }

        bool IDataProvider.CanValidation
        {
            get { return false; }
        }

        bool IDataProvider.Validate()
        {
            return true;
        }
    }
}
