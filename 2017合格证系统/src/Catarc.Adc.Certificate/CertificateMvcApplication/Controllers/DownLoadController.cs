using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.IO;
using System.Text;

namespace CertificateMvcApplication.Controllers
{
    [HandleError]
    public class DownLoadController : Controller
    {

        // **************************************
        // URL: /DownLoad/Download
        // **************************************

        public FileStreamResult Download(string parentName, string name)
        {
            // 根据web.xml设置和页面传递的参数找到对应的下载文件
            string floder = System.Configuration.ConfigurationManager.AppSettings["FilePath"];
            string filePath = Path.Combine(floder, parentName, name);

            return File(new FileStream(filePath, FileMode.Open), "application/octet-stream", Server.UrlEncode(name));
        }

    }
}