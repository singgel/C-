using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CertificateMvcApplication.Models;
using System.Data;
using CertificateMvcApplication.Model;
using CertificateMvcApplication.Utils;

namespace CertificateMvcApplication.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        // **************************************
        // URL: /Home/Index
        // **************************************

        public ActionResult Index()
        {
            // 若用户信息未记录或已过期，跳转到登录页面
            if (Session["UserName"] == null)
            {
                return RedirectToAction("LogOn", "Account");
            }
            return View();
        }

        // **************************************
        // URL: /Home/ZTree
        // **************************************

        public ActionResult ZTree()
        {
            ReadFilesUtils rf = new ReadFilesUtils();
            // 根据ReadFilesUtils获取合格证的文件数据目录
            List<FileModel> listModel = rf.GetFiles();
            return Json(listModel, JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Home/TData
        // 根据查询的文件名进行缓存，缓存时长600秒
        // **************************************

        [OutputCache(CacheProfile = "TDataCache", VaryByParam = "name"/* 缓存参数 */)]
        public ActionResult TData(string parentName, string name)
        {
            List<HGZ_APPLIC_PART> lsData = new List<HGZ_APPLIC_PART>();
            lsData = ReadCsvUtils.readCSV(parentName, name);

            return Json(lsData, JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Home/QueryByID
        // 根据查询的参数主键进行缓存，缓存时长600秒
        // **************************************

        [OutputCache(CacheProfile = "QueryByIDCache", VaryByParam = "id"/* 缓存参数 */)]
        public ActionResult QueryByID(string parentName, string name, string id)
        {
            DataTable dt = ReadCsvUtils.OpenCSV(parentName, name);
            var deatil = (from d in dt.AsEnumerable()
                          where d.Field<string>("H_ID").Equals(id)
                          select d).CopyToDataTable();
            var Model = Dt2ModelUtils.GetEntity<HGZ_APPLIC>(deatil);
            return Json(Model, JsonRequestBehavior.AllowGet);
        }

        // **************************************
        // URL: /Home/About
        // **************************************

        public ActionResult About()
        {
            return View();
        }

    }
}
