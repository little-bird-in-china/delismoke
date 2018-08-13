using BlueStone.Utility;
using BlueStone.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class AccountController : Controller
    {
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoLogin(string account,string password,string keepalive)
        {
            string encrptedPassword = AuthMgr.EncryptPassword(password);
            AuthMgr.Login(account, encrptedPassword, "6666", !string.IsNullOrEmpty(keepalive) && keepalive.ToLower() == "true");
            return Json(new AjaxResult { Success = true, Message = "登录成功" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            AuthMgr.Logout();
            string returnurl = Request.QueryString["returnurl"];
            return Redirect(System.Configuration.ConfigurationManager.AppSettings["LoginUrl"] + "?returnurl=" + returnurl);
        }

        [OutputCache(Duration = 60 * 60 * 4/*4 hours*/, VaryByParam = "none")]
        public ActionResult MethodList()
        {
            return View();
        }

        [OutputCache(Duration = 60 * 60 * 4/*4 hours*/, VaryByParam = "name")]
        public ActionResult MethodDetail()
        {
            return View();
        }

    }
}