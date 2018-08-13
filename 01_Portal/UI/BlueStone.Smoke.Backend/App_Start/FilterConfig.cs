using System;
using System.Web;
using System.Web.Mvc;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Utility;
using BlueStone.Utility.Web;

namespace BlueStone.Smoke.Backend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandleAttribute());
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ActionPermitAttribute : FilterAttribute, IActionFilter
    {
        string[] permissionKeys;
        public ActionPermitAttribute(params string[] permissionKeys)
        {
            this.permissionKeys = permissionKeys;
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!AuthMgr.HasAuth(permissionKeys))
            {
                throw new BusinessException("对不起，您没有此功能的操作权限，请联系管理员。");
            }
        }
    }
}
