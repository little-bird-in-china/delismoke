using BlueStone.JsonRpc;
using BlueStone.JsonRpc.Client;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using BlueStone.Utility.Web.Error;
using System;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.PMPortal.App_Start
{
    public class WebHandleErrorAttribute : CustomHandleErrorAttribute
    {
        protected override System.Web.Mvc.ActionResult BuildAjaxHtmlActionResult(Exception ex, bool isLocalRequest)
        {
            string message = GetExceptionInfo(ex, isLocalRequest);
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"service_Error_Message_Panel\">");
            sb.AppendFormat("<input id=\"errorMessage\" type=\"hidden\" value=\"{0}\" />", HttpUtility.HtmlEncode(message));
            sb.Append("</div>");
            return new ContentResult
            {
                Content = sb.ToString(),
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/html"
            };
        }

        protected override System.Web.Mvc.ActionResult BuildAjaxJsonActionResult(Exception ex, bool isLocalRequest)
        {
            var bizEx = ex as BusinessException;
            JsonResult jr = new JsonResult();
            jr.Data = new AjaxResult() { Success = false, Message = GetExceptionInfo(ex, isLocalRequest) };
            jr.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jr;
        }

        protected override System.Web.Mvc.ActionResult BuildAjaxXmlActionResult(Exception ex, bool isLocalRequest)
        {
            string message = GetExceptionInfo(ex, isLocalRequest);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\"?>");
            sb.AppendLine("<result>");
            sb.AppendLine("<error>true</error>");
            sb.AppendLine("<message>" + message.Replace("<", "&lt;").Replace(">", "&gt;") + "</message>");
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

        protected override bool HandleException(Exception ex)
        {
            if (!IsBizException(ex))
            {
                string logSource = "AuthCenter";
                Logger.WriteLog(ex.ToString(), logSource);
            }
            return true;
        }

        private bool IsBizException(Exception ex)
        {
            if (ex is BusinessException
                || ((ex is FaultException) && ((FaultException)ex).Code.Name == "1")
                || ((ex is JsonRpcException) && ((JsonRpcException)ex).code == 32000)
                || (ex is HttpRequestValidationException))
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

                if (ex is JsonRpcException && ((JsonRpcException)ex).code == 32000)
                {
                    errMsg = ((JsonRpcException)ex).message;
                }
                else if (ex is HttpRequestValidationException)
                {
                    errMsg = "检测到非法输入（例如：html标签），不能提交。";
                }

                return errMsg;
            }
            if (isLocalRequest)
            {
                if (ex is FaultException)    // throw on wcf service
                {
                    return ((FaultException)ex).Reason.ToString();
                }
                else if (ex is JsonRpcException)
                {
                    var jsonRpcException = ex as JsonRpcException;
                    string method = null;
                    string requestRaw = null;
                    //if (jsonRpcException.rpcRequest != null)
                    //{
                    //    method = jsonRpcException.rpcRequest.Method;
                    //    requestRaw = JsonConvert.SerializeObject(jsonRpcException.rpcRequest);
                    //}
                    //else if (jsonRpcException.rpcRequestRaw != null)
                    //{
                    //    requestRaw = jsonRpcException.rpcRequestRaw;
                    //}
                    return string.Format("调用服务发生异常{0}：{1};\r\n异常详细：{2} \r\n请求参数：{3};",
                        method,
                        jsonRpcException.message,
                        jsonRpcException.data,
                        requestRaw);
                }
                else  // throw on web portal
                {
                    return ex.ToString();
                }
            }
            else
            {
                return LanguageHelper.GetText("系统发生异常，请稍后再试。");
            }
        }
    }
}