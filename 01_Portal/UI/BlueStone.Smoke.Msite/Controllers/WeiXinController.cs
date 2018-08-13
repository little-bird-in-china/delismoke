using BlueStone.DataAdapter;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Service;
using BlueStone.Utility;
using MessageCenter.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Msite.Controllers
{
    /// <summary>
    /// 微信相关
    /// </summary>
    public class WeiXinController : WWWControllerBase
    {
        /// <summary>
        /// 微信登录
        /// </summary>
        public void WXLogin()
        {

            string returnUrl = Request.QueryString["ReturnUrl"];

            #region  是否开发环境
            string devClinetSysNo = ConfigurationManager.AppSettings["DevClinetUserSysNo"];
            int clientSysNo = 0;
            int.TryParse(devClinetSysNo, out clientSysNo);
            if (clientSysNo > 0)
            {
                Client cusomerInfo = ClientService.LoadClient(clientSysNo);
                if (cusomerInfo != null && cusomerInfo.SysNo > 0)
                {
                    var appuser = new AppUserInfo()
                    {
                        AppCustomerID = cusomerInfo.AppCustomerID,
                        UserSysNo = cusomerInfo.SysNo,
                        UserID = HttpUtility.UrlEncode(cusomerInfo.Name),
                        HeadImage = cusomerInfo.HeaderImage,
                        UserDisplayName = HttpUtility.UrlEncode(cusomerInfo.Name),

                        ManagerSysNo = cusomerInfo.ManagerSysNo,
                        UserType = UserType.Common,
                        LastLoginDateText = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                        ManagerLoginName = cusomerInfo.ManagerLoginName,
                        ManagerName = HttpUtility.UrlEncode(cusomerInfo.ManagerName)
                    };

                    if (cusomerInfo.ManagerSysNo.HasValue && cusomerInfo.ManagerSysNo.Value > 0)
                    {
                        appuser.UserType = UserType.Manager;
                        var company = CompanyService.GetCompanyUser(cusomerInfo.ManagerSysNo.Value);
                        if (company != null)
                        {
                            appuser.UserType = UserType.Installer;
                        }
                    }

                    UserMgr.Logout();
                    UserMgr.WriteUserInfo(appuser);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        Response.Redirect(returnUrl);
                        return;
                    }
                    Response.Redirect("/smoke/userInfo");
                    return;
                }
            }
            #endregion


            NameValueCollection collection = new NameValueCollection();
            collection.Add("ReturnUrl", HttpUtility.UrlDecode(returnUrl));
            Response.Redirect(WeiXinService.WeiXinLogin(collection));
        }



        [HttpGet]
        public ActionResult GetWXjsSdkConfig(string curl)
        {
            string ticket = string.Empty;
            //try
            //{
            ticket = WeiXinService.GetWeixinJsApiTicket();
            //}
            //catch (BusinessException e )
            //{
            //    return Json(new AjaxResult { Success = false,Message="获取微信接口信息失败,请重新进入后再试" }, JsonRequestBehavior.AllowGet);
            //}

            if (string.IsNullOrEmpty(ticket))
            {
                throw new BusinessException("调用微信接口失败,请重新进入页面后重试!");
            }
            Random random = new Random(unchecked((int)DateTime.Now.Ticks));
            var srandom = random.Next(11111, 999999);
            WeixinJsApiConfig jsApiConfig = new WeixinJsApiConfig
            {
                Noncestr = srandom.ToString(),
                Timestamp = WebPortalHelper.GetTimeStamp().ToString(),
            };
            string url = string.Empty;
            if (!string.IsNullOrEmpty(curl))
            {
                url = curl;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("jsapi_ticket={0}&", ticket));
            sb.Append(string.Format("noncestr={0}&", jsApiConfig.Noncestr));
            sb.Append(string.Format("timestamp={0}&", jsApiConfig.Timestamp));
            sb.Append(string.Format("url={0}", url));
            jsApiConfig.Signature = SecurityHelper.GetSHA1Value(sb.ToString());
            jsApiConfig.AppId = WechatSenderService.WXConfig.AppID;
            jsApiConfig.Debug = false;
            jsApiConfig.JsApiList = WechatSenderService.WXConfig.JsApis;
            var jsapiconfig = JsonConvert.SerializeObject(jsApiConfig);
            return Json(new AjaxResult { Success = true, Data = jsapiconfig }, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// </summary>
        /// 微信登录回调
        public void WXLoginBack()
        {
            Client customer = new Client();
            WeiXinUser weiXinUser = WeiXinService.GetWeiXinUser(string.Empty, Request.Params["code"].ToString());
            string callbackUrl = Request.QueryString["state"].ToString();
            if (weiXinUser != null && !string.IsNullOrEmpty(weiXinUser.Openid))
            {
                Client cusomerInfo = ClientService.LoadClientByAppCustomerID(weiXinUser.Openid);
                if (cusomerInfo != null && cusomerInfo.SysNo > 0)
                {

                    //更新用户头像以及昵称
                    cusomerInfo.Name = weiXinUser.NickName;
                    cusomerInfo.HeaderImage = weiXinUser.HeadImgUrl;
                    cusomerInfo.EditTime = DateTimeHelper.GetTimeZoneNow();

                    ClientService.UpdateClient(cusomerInfo);

                    var appuser = new AppUserInfo()
                    {
                        AppCustomerID = weiXinUser.Openid,
                        UserSysNo = cusomerInfo.SysNo,
                        UserID = HttpUtility.UrlEncode(cusomerInfo.Name),
                        HeadImage = cusomerInfo.HeaderImage,
                        UserDisplayName = HttpUtility.UrlEncode(cusomerInfo.Name),

                        ManagerSysNo = cusomerInfo.ManagerSysNo,
                        UserType = UserType.Common,
                        LastLoginDateText = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss"),
                        ManagerLoginName = cusomerInfo.ManagerLoginName,
                        ManagerName = HttpUtility.UrlEncode(cusomerInfo.ManagerName)
                    };

                    if (cusomerInfo.ManagerSysNo.HasValue && cusomerInfo.ManagerSysNo.Value > 0)
                    {
                        appuser.UserType = UserType.Manager;
                        var company = CompanyService.GetCompanyUser(cusomerInfo.ManagerSysNo.Value);
                        if (company != null)
                        {
                            appuser.UserType = UserType.Installer;
                        }
                    }

                    UserMgr.Logout();
                    UserMgr.WriteUserInfo(appuser);
                    if (!string.IsNullOrEmpty(callbackUrl))
                    {
                        Response.Redirect(callbackUrl);
                        return;
                    }
                    Response.Redirect("/smoke/userInfo");
                    return;
                }
                else//新建client
                {
                    customer.AppCustomerID = weiXinUser.Openid;
                    customer.Name = weiXinUser.NickName;
                    customer.HeaderImage = weiXinUser.HeadImgUrl;
                    customer.EditTime = DateTimeHelper.GetTimeZoneNow();
                    customer.RegisterTime = DateTimeHelper.GetTimeZoneNow();
                    //创建用户
                    customer.SysNo = ClientService.InsertClient(customer);
                    var appuser = new AppUserInfo()
                    {
                        AppCustomerID = weiXinUser.Openid,
                        UserSysNo = customer.SysNo,
                        UserID = HttpUtility.UrlEncode(customer.Name),
                        UserDisplayName = HttpUtility.UrlEncode(customer.Name),
                        HeadImage = customer.HeaderImage,
                        UserType = UserType.Common,
                        LastLoginDateText = DateTimeHelper.GetTimeZoneNow().ToString("yyyy-MM-dd HH:mm:ss")
                    };
                    UserMgr.Logout();
                    UserMgr.WriteUserInfo(appuser);
                    if (!string.IsNullOrEmpty(callbackUrl))
                    {
                        Response.Redirect(callbackUrl);
                        return;
                    }
                    Response.Redirect("/smoke/userInfo");
                    return;
                }
            }
        }
    }
}
