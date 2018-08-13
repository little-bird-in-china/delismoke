using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using BlueStone.Utility.Web.Auth.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    [Auth]
    public class BaseController : Controller
    {
        public CurrentUser CurrUser;

        public BaseController()
        {
            this.ViewBag.IsUserLogin = AuthMgr.HasLogin();
            //加载登录信息
            var userInfo = AuthMgr.ReadUserInfo();

            if (userInfo != null)
            {
                CurrUser = new CurrentUser()
                {
                    UserName = userInfo.UserID,
                    UserSysNo = userInfo.UserSysNo,
                    UserDisplayName = userInfo.UserDisplayName,
                    AvatarImageUrl = userInfo.AvatarImageUrl,
                    LoginTime = userInfo.LoginTime.ToString()

                };
                if (userInfo.ExData != null)
                {
                    CurrUser.MasterSysNo = (int)userInfo.ExData;
                }
                this.ViewBag.CurrUser = CurrUser;
            }
        }
        

        protected override void HandleUnknownAction(string actionName)
        {
            try
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
            catch (InvalidOperationException ieox)
            {
                ViewData["error"] = "Unknown Action: \"" + Server.HtmlEncode(actionName) + "\"";
                ViewData["exMessage"] = ieox.Message;
                this.View("Error404").ExecuteResult(this.ControllerContext);
            }
        }

        protected T BuildQueryFilterEntity<T>() where T : QueryFilter, new()
        {
            return BuildQueryFilterEntity<T>(null);
        }

        protected T BuildQueryFilterEntity<T>(Action<T> manualMapping) where T : QueryFilter, new()
        {
            string data = Request.Form["data"];
            if (string.IsNullOrWhiteSpace(data))
            {
                data = Request.Form["data[]"];
            }
            T t = null;
            if (string.IsNullOrWhiteSpace(data))
            {
                t = new T();
            }
            else
            {
                t = JsonConvert.DeserializeObject<T>(HttpUtility.UrlDecode(data));
            }

            //每页显示条数:
            int pageSize = Convert.ToInt32(Request["iDisplayLength"]);
            if (t is QueryFilter && pageSize > 0)
            {
                //当前页码:
                int pageIndex = Convert.ToInt32(Request["iDisplayStart"]) % pageSize == 0 ? Convert.ToInt32(Request["iDisplayStart"]) / pageSize : Convert.ToInt32(Request["iDisplayStart"]) / pageSize + 1;
                //排序:
                string sortBy = null;
                if (!string.IsNullOrEmpty(Request["iSortingCols"]))
                {
                    string colIndex = Request["iSortingCols"];
                    if (!string.IsNullOrWhiteSpace(Request["sColumns"]))
                    {
                        var sortFields = Request["sColumns"].Split(',');
                        string sortByField = sortFields[int.Parse(colIndex)];
                        string sortDir = Request["sSortDir_0"];
                        sortBy = string.Format("{0} {1}", sortByField, sortDir.ToUpper());
                    }
                }

                t.PageSize = pageSize;
                t.PageIndex = pageIndex;
                t.SortFields = sortBy;
                t.draw = int.Parse(Request["sEcho"]);
            }

            if (manualMapping != null)
            {
                manualMapping((T)t);
            }
            return (T)t;

        }

        protected JsonResult AjaxGridJson<T>(QueryResult<T> result) where T : class
        {
            return Json(new
            {
                sEcho = result.draw,
                iTotalRecords = result.recordsTotal,
                iTotalDisplayRecords = result.recordsTotal,
                aaData = result.data
            }, JsonRequestBehavior.AllowGet);
        }

        protected JsonResult AjaxJsonTableData<T>(QueryResult<T> result) where T : class
        {
            return Json(new
            {
                Success = true,
                iTotalRecords = result.recordsTotal,
                aaData = result.data
            }, JsonRequestBehavior.AllowGet);
        }

    }
}