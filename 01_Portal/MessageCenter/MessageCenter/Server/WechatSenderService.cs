using BlueStone.Utility;
using MessageCenter.DataAccess;
using MessageCenter.Entity;
using MessageCenter.Processor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace MessageCenter.Server
{
    public class WechatSenderService
    {
        private static WeiXinConfig _wxconfig = null;

        public static string WX_KEY_COMMONTOKEN = ConfigurationManager.AppSettings["MemoCache_Wx_key_CommonToken"];
        public static string WX_KEY_JSAPITICKET = ConfigurationManager.AppSettings["MemoCache_Wx_key_JsapiTicket"];


        public static WeiXinConfig WXConfig {
            get
            {
                if (_wxconfig == null)
                {
                    _wxconfig= GetWeiXinConfig();
                }
                return _wxconfig;
            }
        }



        /// <summary>
        /// 推送模板消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void PushMessage(WechatMessage message)
        {
            if (string.IsNullOrEmpty(message.ToUser))
            {
                throw new BusinessException("用户未绑定微信号！");
            }

            string token = GetCommonToken();
            if (string.IsNullOrEmpty(token))
            {
                throw new BusinessException("获取微信授权Token失败,请重试！");
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(WXConfig.TemplateMessageUrl);
            builder.Append(token);
            var result = HttpClient.Post<WeiXinMsgResult>(builder.ToString(), JsonConvert.SerializeObject(message));
            if (result.Result.ErrCode != 0||!string.Equals(result.Result.ErrMsg,"ok"))
            {
                throw new BusinessException(string.Format("发送微信消息异常，[{0}]{1}！", result.Result.ErrCode, result.Result.ErrMsg));
            }
        }


        private static string WebChatAPPID = WXConfig.AppID;
        private static string WebChatAPPSecret = WXConfig.AppSecret;


        /// <summary>
        /// 获取共用Token,调用基础api时使用和网页的token、JSAPI的token不相同
        /// </summary>
        /// <returns></returns>
        public static string GetCommonToken()
        {
            string cacheKey = WX_KEY_COMMONTOKEN;
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (string)HttpRuntime.Cache[cacheKey];
            }
            if (string.IsNullOrWhiteSpace(WebChatAPPID) || string.IsNullOrWhiteSpace(WebChatAPPSecret))
            {
                throw new BusinessException("请配置微信的APPID和APPSecret,登录微信开发平台可查询此值。");
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(WXConfig.CommonAccessTokenUrl);
            builder.Append(string.Format("&appid={0}", WebChatAPPID));
            builder.Append(string.Format("&secret={0}", WebChatAPPSecret));
            AsyncResult<WeiXinTokenResult> result = HttpClient.Get<WeiXinTokenResult>(builder.ToString());
            if (result != null)
            {
                if (result.Error == null)
                {
                    if (result.Result != null)
                    {
                        if (!string.IsNullOrEmpty(result.Result.Token))
                        {
                            HttpRuntime.Cache.Insert(cacheKey, result.Result.Token, null, DateTime.Now.AddSeconds(result.Result.ExpiresIn - 30), Cache.NoSlidingExpiration);
                            return result.Result.Token;
                        }
                        if (result.Result.ErrCode > 0)
                        {
                            Logger.WriteLog(result.Result.ErrMsg);
                            //TODO记录日志
                        }
                    }
                }
                else
                {
                    Logger.WriteLog(result.Error.Message);
                    //TODO记录日志
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 获取配置文件
        /// </summary>
        /// <returns></returns>
        private  static WeiXinConfig GetWeiXinConfig()
        {
            XmlDocument doc = new XmlDocument();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration/WeiXinPush.config");
            if (!File.Exists(path))
            {
                throw new BusinessException("当前项目目录下，  /Configuration/WeiXinPush.config 文件不存在!");
            }
            doc.Load(path);
            XmlNode node = doc.SelectSingleNode("weiXinPush");
            WeiXinConfig config = new WeiXinConfig();
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



        public static WXBaseMessage GetWXMsgEntity( string content)
        {
            WXBaseMessage message = new WXBaseMessage();
            if (string.IsNullOrEmpty(content))
            {
                return message;
            };
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(content);
            var msgtype = xml.SelectSingleNode("/xml/MsgType").InnerText;
            XmlNode node = xml.SelectSingleNode("xml");
            if (msgtype.ToLower().Equals(WXMsgType.text.ToString()))
            {
                 message = HttpClient.DeserializeXmlStr<WXTextMessage>(content);
            }
            else if (msgtype.ToLower().Equals(WXMsgType.image.ToString()))
            {
                message = HttpClient.DeserializeXmlStr<WXImageMessage>(content);
            }
            else if (msgtype.ToLower().Equals(WXMsgType.video.ToString()))
            {
                message = HttpClient.DeserializeXmlStr<WXVideoMessage>(content);

            }
            else if (msgtype.ToLower().Equals(WXMsgType.voice.ToString()))
            {
               message = HttpClient.DeserializeXmlStr<WXVoiceMessage>(content);
            }
            return message;
        }

        #region 构建微信消息字符串
        public static string SerializeEntity<T>(T entity)
        {
            return HttpClient.SerializeXML(entity);
        }
        public static void ConventToWXentity<T>(T entity)
        {
            if (entity != null)
            {
                Type t = entity.GetType();
                PropertyInfo[] propertylist = t.GetProperties();
                foreach (var property in propertylist)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var objval = property.GetValue(entity, null);
                        if (objval == null)
                        {
                            continue;
                        }
                        string val = objval.ToString();
                        string startTag = "<![CDATA[";
                        string endTag = "]]>";
                        //  char[] trims = new char[] { '\r', '\n', '\t', ' ' };
                        // val = val.Trim(trims);
                        if (!val.StartsWith(startTag) && !val.EndsWith(endTag))
                        {
                            val = val.Insert(0, startTag);
                            val = val.Insert(val.Length, endTag);
                        }
                        property.SetValue(entity, val);
                    }
                }
            }
        }
        public static string OpeationWXEntity<T>(T entity)
        {
            string resstr = string.Empty;
            ConventToWXentity(entity);
            resstr = SerializeEntity(entity);
            resstr = resstr.Replace("&lt;", "<");
            resstr = resstr.Replace("&gt;", ">");
            return resstr;
        }
        #endregion
    }
}
