using BlueStone.Utility;
using BlueStone.Utility.Web;
using BlueStone.Utility.Web.Error;
using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.App_Start
{
    public class ErrorHandleAttribute : CustomHandleErrorAttribute
    {
        protected override bool HandleException(Exception ex)
        {
            if (!IsBizException(ex))
            {
                Logger.WriteLog(ex.ToString(), "Portal_Exception");
            }
            return true;
        }

        private bool IsBizException(Exception ex)
        {
            if (ex is BusinessException)
            {
                return true;
            }
            return false;
        }

        private string GetExceptionInfo(Exception ex, bool isLocalRequest)
        {
            if (IsBizException(ex))
            {
                string errMsg = ex.Message;
                errMsg = LanguageHelper.GetText(errMsg);
                return errMsg;
            }
            else if (ex is System.Web.HttpRequestValidationException)
            {
                return "页面中输入了不安全的字符组合，如&lt;a&gt;。错误信息：" + HttpUtility.HtmlEncode(ex.Message);
            }
            if (isLocalRequest)
            {
                return ex.ToString();
            }
            else
            {
                return LanguageHelper.GetText("系统发生异常，请稍后再试。");
            }
        }


        protected override ActionResult BuildAjaxJsonActionResult(Exception ex, bool isLocalRequest)
        {
            var data = new
            {
                Success = false,
                Message = GetExceptionInfo(ex, isLocalRequest)
            };
            JsonResult jr = new JsonResult();
            jr.Data = data;
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        protected override ActionResult BuildAjaxHtmlActionResult(Exception ex, bool isLocalRequest)
        {
            string message = GetExceptionInfo(ex, isLocalRequest);
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"bad_request\">");
            sb.AppendFormat("<input id=\"errorMessage\" type=\"hidden\" value=\"{0}\" />", HttpUtility.HtmlEncode(message));
            sb.Append("</div>");
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/html"
            };
        }

        protected override ActionResult BuildAjaxXmlActionResult(Exception ex, bool isLocalRequest)
        {
            string message = GetExceptionInfo(ex, isLocalRequest);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<result>");
            sb.AppendLine("<Success>true</Success>");
            sb.AppendLine("<Message>" + message.Replace("<", "&lt;").Replace(">", "&gt;") + "</Message>");
            sb.AppendLine("</result>");
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentEncoding = Encoding.UTF8,
                ContentType = "application/xml"
            };
        }





        protected override ActionResult BuildWebPageActionResult(Exception ex, bool isLocalRequest, ExceptionContext filterContext)
        {
            string errorStr = GetExceptionInfo(ex, isLocalRequest);
            Exception exception = new Exception(errorStr);

            string controller = filterContext.RouteData.Values["controller"].ToString();
            string action = filterContext.RouteData.Values["action"].ToString();
            exception.HelpLink = IsBizException(ex) ? "BizEx" : "";
            HandleErrorInfo model = new HandleErrorInfo(exception, controller, action);

            return new ViewResult
            {
                ViewName = this.View,
                MasterName = this.Master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };
        }
    }
}
