using BlueStone.Utility.Web;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Msite.Controllers
{
    /// <summary>
    /// 普通页面Controller从此基类基础
    /// </summary>
    public class WWWControllerBase : ControllerBase
    {



        /// <summary>
        /// 构造函数
        /// </summary>
        ///

        public WWWControllerBase()
        {
            this.ViewBag.IsLogin = UserMgr.HasLogin();
            if (ViewBag.IsLogin)
            {
                this.ViewBag.DisplayName = HttpUtility.UrlDecode(curentUser.UserDisplayName);
            }
        }


    }

    /// <summary>
    /// 需要登录的Controller
    /// </summary>
    [WebAuth(NeedAuth = true)]
    public class SSLControllerBase : ControllerBase
    {

    }

    public class ControllerBase : Controller
    {

        protected AppUserInfo curentUser = UserMgr.ReadUserInfo();

        protected ActionResult GotoErrorPage(string msg)
        {
            TempData["ErrorMessage"] = msg;
            return Redirect("/ErrorMsg");
        }
        protected object BuildAjaxErrorObject(string msg)
        {
            return new
            {
                error = true,
                message = msg
            };
        }

    }
}