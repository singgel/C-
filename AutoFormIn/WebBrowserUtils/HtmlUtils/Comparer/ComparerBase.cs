using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using WebBrowserUtils.HtmlUtils.Fillers;

namespace WebBrowserUtils.HtmlUtils.Comparer
{
    public abstract class ComparerBase
    {
        private string _standard, _carType;
        /// <summary>
        /// 排放标准。
        /// </summary>
        public string Standard
        {
            get { return _standard; }
        }
        /// <summary>
        /// 车辆类型。
        /// </summary>
        public string CarType
        {
            get { return _carType; }
        }

        protected ComparerBase(string standard, string carType)
        {
            _standard = standard;
            _carType = carType;
        }

        protected virtual string GetServerFile()
        {
            return string.Empty;
        }
        /// <summary>
        /// 向服务器上传有变动的页面文件。
        /// </summary>
        protected virtual void UploadFile()
        {
        }
    }
}
