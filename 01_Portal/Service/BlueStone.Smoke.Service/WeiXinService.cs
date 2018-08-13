using BlueStone.Utility;
using BlueStone.Utility.HttpClient;
using MessageCenter.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace BlueStone.Smoke.Service
{
    public class WeiXinService
    {


        private static MessageCenter.Entity.WeiXinConfig _config = null;
        static WeiXinService()
        {
            if (_config == null)
            {
                _config = WechatSenderService.WXConfig;
            }
        }

        /// <summary>
        /// 登录微信
        /// </summary>
        /// <param name="param">自定义参数</param>
        /// <returns>返回微信登录地址和参数</returns>
        public static string WeiXinLogin(NameValueCollection param)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(_config.LoginUrl);
            builder.Append(string.Format("?appid={0}&", _config.AppID));
            builder.Append(string.Format("redirect_uri={0}&", _config.LoginBackUrl));
            builder.Append("response_type=code&");
            builder.Append("scope=snsapi_userinfo");
            if (param != null && param.AllKeys.Length > 0)
            {
                builder.Append(string.Format("&state={0}#wechat_redirect", HttpUtility.UrlEncode(param["ReturnUrl"])));
            }
            else
            {
                builder.Append("&state=STATE#wechat_redirect");
            }
            //Logger.WriteLog("ReturnUrl" + builder.ToString());
            return builder.ToString().Trim();
        }

        public static string GetWeixinJsApiTicket()
        {
            if (string.IsNullOrEmpty(WechatSenderService.WX_KEY_JSAPITICKET) || string.IsNullOrEmpty(WechatSenderService.WX_KEY_COMMONTOKEN))
            {
                throw new BusinessException("请在web配置文件appsetting中加入相应的配置");
            }
            if (HttpRuntime.Cache[WechatSenderService.WX_KEY_JSAPITICKET] != null)
            {
                return (string)HttpRuntime.Cache[WechatSenderService.WX_KEY_JSAPITICKET];
            }
            if (_config != null)
            {
                string commonToken = string.Empty;
                if (HttpRuntime.Cache[WechatSenderService.WX_KEY_COMMONTOKEN] != null)
                {
                    commonToken = (string)HttpRuntime.Cache[WechatSenderService.WX_KEY_COMMONTOKEN];
                }
                else
                {
                    var commonTokenET = GetCommonToken(null, true);
                    commonToken = commonTokenET.Token;
                    HttpRuntime.Cache.Insert(WechatSenderService.WX_KEY_COMMONTOKEN, commonToken, null, DateTime.Now.AddSeconds(commonTokenET.ExpiresIn - 30), Cache.NoSlidingExpiration);
                }
                StringBuilder builder = new StringBuilder();
                builder.Append(_config.JsapiTicketUrl);
                builder.Append(string.Format("?access_token={0}&", commonToken));
                builder.Append("type=jsapi");
                AsyncResult<WeiXinTicketResult> result = HttpClient.Get<WeiXinTicketResult>(builder.ToString());
                if (result != null)
                {
                    if (result.Error == null)
                    {
                        if (result.Result != null && result.Result.ErrCode == 0)
                        {
                            HttpRuntime.Cache.Insert(WechatSenderService.WX_KEY_JSAPITICKET, result.Result.Ticket, null, DateTime.Now.AddSeconds(result.Result.ExpiresIn - 30), Cache.NoSlidingExpiration);
                            return result.Result.Ticket;
                        }
                        else
                        {
                            throw new BusinessException(string.Format("获取微信jssdk凭证失败,错误Code：{0},ErrorMsg：{1}", result.Result.ErrCode, result.Result.ErrMsg));
                        }
                    }
                    else
                    {
                        throw new BusinessException(string.Format("获取微信jssdk凭证失败,{0}", result.Error.ToString()));
                    }
                }

            }
            return string.Empty;
        }

        /// <summary>
        /// 登录微信回调处理
        /// </summary>
        /// <param name="param"></param>
        /// <returns>返回openid和自定义参数</returns>
        public static NameValueCollection WeiXinLoginBack(NameValueCollection param)
        {
            NameValueCollection result = new NameValueCollection();
            if (param != null && param.AllKeys.Length > 0)
            {
                if (param.AllKeys.Contains("state"))
                {
                    string state = HttpUtility.UrlDecode(param["state"]);
                    if (!string.IsNullOrEmpty(state))
                    {
                        string[] customParams = state.Split('|');
                        if (customParams != null && customParams.Length > 0)
                        {
                            foreach (var item in customParams)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    string[] customParam = item.Split('=');
                                    if (customParam != null && customParam.Length == 2)
                                    {
                                        result.Add(customParam[0], customParam[1]);
                                    }
                                }
                            }
                        }
                    }
                }
                if (param.AllKeys.Contains("code"))
                {
                    string code = param["code"];
                    if (!string.IsNullOrEmpty(code))
                    {
                        result.Add("OpenId", GetOpendId(code));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获取微信用户基本信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static WeiXinUser GetWeiXinUser(string openId, string code)
        {
            if (_config != null)
            {
                var commonToken = GetCommonToken(code);
                if (!string.IsNullOrEmpty(commonToken.Token) && !string.IsNullOrEmpty(commonToken.Openid))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(_config.GetUserInfoUrl);
                    sb.AppendFormat("?access_token={0}", commonToken.Token);
                    sb.AppendFormat("&openid={0}", commonToken.Openid);
                    sb.AppendFormat("&lang={0}", "zh_CN");
                    AsyncResult<WeiXinUser> result = HttpClient.Get<WeiXinUser>(sb.ToString());

                    if (result != null)
                    {
                        if (result.Error == null)
                        {
                            if (result.Result != null && result.Result.ErrCode == 0)
                            {
                                return result.Result;
                            }
                            else
                            {
                                throw new BusinessException(string.Format("获取微信用户基本信息失败,错误Code：{0},ErrorMsg：{1}", result.Result.ErrCode, result.Result.ErrMsg));
                            }
                        }
                        else
                        {
                            throw new BusinessException(string.Format("获取微信用户基本信息失败,{0}", result.Error.ToString()));
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 获取共用Token,调用基础api时使用和网页的token、JSAPI的token不相同
        /// </summary>
        /// <returns></returns>
        public static WeiXinTokenResult GetCommonToken(string code = null, bool isCommonToken = false)
        {
            //string token = WeiXinDA.GetWeiXinCommonToken();
            //if (!string.IsNullOrEmpty(token))
            //{
            //    return token;
            //}

            if (_config != null)
            {
                StringBuilder builder = new StringBuilder();
                if (isCommonToken)
                {
                    builder.Append(_config.CommonAccessTokenUrl);
                    builder.Append(string.Format("&appid={0}", _config.AppID));
                    builder.Append(string.Format("&secret={0}", _config.AppSecret));
                }
                else
                {
                    builder.Append(_config.WebAccessTokenUrl);
                    builder.Append(string.Format("?appid={0}", _config.AppID));
                    builder.Append(string.Format("&secret={0}", _config.AppSecret));
                    builder.Append(string.Format("&code={0}", code));
                    builder.Append(string.Format("&grant_type={0}", "authorization_code"));
                }
                AsyncResult<WeiXinTokenResult> result = HttpClient.Get<WeiXinTokenResult>(builder.ToString());
                if (result != null)
                {
                    if (result.Error == null)
                    {
                        if (result.Result != null)
                        {
                            if (!string.IsNullOrEmpty(result.Result.Token))
                            {
                                return result.Result;
                            }
                            if (result.Result.ErrCode > 0)
                            {
                                throw new BusinessException(string.Format("获取CommonToken失败,错误Code：{0},ErrorMsg：{1}", result.Result.ErrCode, result.Result.ErrMsg));
                            }
                        }
                    }
                    else
                    {
                        throw new BusinessException(string.Format("获取CommonToken失败,{0}", result.Error.ToString()));
                    }
                }
            }

            return new WeiXinTokenResult();
        }

        /// <summary>
        /// 通过code换取OpendId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string GetOpendId(string code)
        {
            //获取AccessToken
            StringBuilder sb = new StringBuilder();
            sb.Append(_config.WebAccessTokenUrl);
            sb.AppendFormat("?appid={0}", _config.AppID);
            sb.AppendFormat("&secret={0}", _config.AppSecret);
            sb.AppendFormat("&code={0}", code);
            sb.Append("&grant_type=authorization_code");

            AsyncResult<WeiXinWebTokenResult> result = HttpClient.Get<WeiXinWebTokenResult>(sb.ToString());
            if (result.Error == null)
            {
                if (result.Result != null)
                {
                    if (result.Result.ErrCode > 0)
                    {
                        Logger.WriteLog(string.Format("获取OpenId失败,错误Code：{0},ErrorMsg：{1}", result.Result.ErrCode, result.Result.ErrMsg));
                    }

                    return result.Result.OpenId;
                }
            }
            else
            {
                Logger.WriteLog(string.Format("获取OpenId失败,{0}", result.Error.ToString()));
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        private static MessageCenter.Entity.WeiXinConfig GetWeiXinConfig()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/WeiXinPush.config"));
            XmlNode node = doc.SelectSingleNode("weiXinPush");
            MessageCenter.Entity.WeiXinConfig config = new MessageCenter.Entity.WeiXinConfig();
            config.AppID = GetXmlNodeValue(node, "appId");
            config.AppSecret = GetXmlNodeValue(node, "appSecret");
            config.Encoding = GetXmlNodeValue(node, "encoding");
            config.GetUserInfoUrl = GetXmlNodeValue(node, "getUserInfoUrl");
            config.CommonAccessTokenUrl = GetXmlNodeValue(node, "commonAccessTokenUrl");
            config.TemplateMessageUrl = GetXmlNodeValue(node, "templateMessageUrl");
            config.LoginUrl = GetXmlNodeValue(node, "loginUrl");
            config.LoginBackUrl = GetXmlNodeValue(node, "loginBackUrl");
            config.WebAccessTokenUrl = GetXmlNodeValue(node, "webAccessTokenUrl");
            config.JsapiTicketUrl = GetXmlNodeValue(node, "jsapiTicketUrl");
            config.JsApis = GetXmlNodeValue(node, "jsApis").Split(',').ToList();
            return config;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetAttributeValue(XmlNode node, string name)
        {
            if (node.Attributes != null && node.Attributes.Count > 0)
            {
                XmlAttribute attribute = node.Attributes[name];
                if (attribute != null)
                {
                    string value = attribute.Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        return value.Trim();
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取节点值
        /// </summary>
        /// <param name="node"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetXmlNodeValue(XmlNode node, string name)
        {
            XmlNode x = node.SelectSingleNode(name);
            if (x != null)
            {
                string value = x.InnerText;
                if (!string.IsNullOrEmpty(value))
                {
                    return value.Trim();
                }
            }
            return string.Empty;
        }
    }


    public class WeiXinUser : WeiXinResult
    {
        /// <summary>
        /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
        /// </summary>
        [JsonProperty("subscribe")]
        public int Subscribe { get; set; }
        /// <summary>
        /// 用户的标识，对当前公众号唯一
        /// </summary>
        [JsonProperty("openid")]
        public string Openid { get; set; }
        /// <summary>
        /// 用户的昵称
        /// </summary>
        [JsonProperty("nickname")]
        public string NickName { get; set; }
        /// <summary>
        /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
        /// </summary>
        [JsonProperty("sex")]
        public int Sex { get; set; }
        /// <summary>
        /// 所在国家
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        [JsonProperty("province")]
        public string Province { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }
        /// <summary>
        /// 用户的语言，简体中文为zh_CN
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; }
        /// <summary>
        /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像）
        /// ，用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
        /// </summary>
        [JsonProperty("headimgurl")]
        public string HeadImgUrl { get; set; }
        /// <summary>
        /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
        /// </summary>
        [JsonProperty("subscribe_time")]
        public string SubscribeTime { get; set; }
        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。
        /// </summary>
        [JsonProperty("unionid")]
        public string Unionid { get; set; }
        /// <summary>
        /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
        /// </summary>
        [JsonProperty("remark")]
        public string Remark { get; set; }
        /// <summary>
        /// 用户所在的分组ID（兼容旧的用户分组接口）
        /// </summary>
        [JsonProperty("groupid")]
        public string Groupid { get; set; }
        /// <summary>
        /// 用户被打上的标签ID列表
        /// </summary>
        [JsonProperty("tagid_list")]
        public List<string> TagidList { get; set; }
    }

    public class WeiXinTokenResult : WeiXinResult
    {
        /// <summary>
        /// 获取到的凭证 
        /// </summary>
        [JsonProperty("access_token")]
        public string Token { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒 
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("openid")]
        public string Openid { get; set; }
    }

    public class WeiXinWebTokenResult : WeiXinTokenResult
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("openid")]
        public string OpenId { get; set; }
        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonProperty("unionid")]
        public string UnionId { get; set; }
    }

    public class WeiXinTicketResult : WeiXinResult
    {
        /// <summary>
        /// jssdkTicket
        /// </summary>
        [JsonProperty("ticket")]
        public string Ticket { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒 
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

    }
    public class WeiXinResult
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [JsonProperty("errcode")]
        public int ErrCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }
    }

    /// <summary>
    ///jsapiconfig ：对所有待签名参数按照字段名的ASCII 码从小到大排序（字典序）后，
    ///使用URL键值对的格式（即key1=value1&key2=value2…）拼接成字符串string1。
    ///这里需要注意的是所有参数名均为小写字符。对string1作sha1加密，字段名和字段值都采用原始值，不进行URL 转义。
    /// 文档: https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421141115
    /// </summary>
    public class WeixinJsApiConfig
    {
        /// <summary>
        /// 必填，生成签名的随机串
        /// </summary>
        [JsonProperty("nonceStr")]
        public string Noncestr { get; set; }
        /// <summary>
        /// 必填，生成签名的时间戳
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
        /// <summary>
        ///  开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
        /// </summary>
        [JsonProperty("debug")]
        public bool Debug { get; set; }
        /// <summary>
        /// 必填，公众号的唯一标识
        /// </summary>
        [JsonProperty("appId")]
        public string AppId { get; set; }
        /// <summary>
        /// 必填，签名
        /// </summary>
        [JsonProperty("signature")]
        public string Signature { get; set; }
        /// <summary>
        ///  必填，需要使用的JS接口列表
        /// </summary>
        [JsonProperty("jsApiList")]
        public List<string> JsApiList { get; set; }
    }
}
