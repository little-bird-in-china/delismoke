using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MessageCenter.Entity
{
    public class WechatMessage
    {
        public WechatMessage()
        {
            Data = new Dictionary<string, WeiXinParam>();
        }
        /// <summary>
        /// OPENID
        /// </summary>
        [JsonProperty("touser")]
        public string ToUser { get; set; }
        /// <summary>
        /// 模板Id
        /// </summary>
        [JsonProperty("template_id")]
        public string TemplateId { get; set; }
        /// <summary>
        /// 点击消息跳转连接
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public Dictionary<string, WeiXinParam> Data { get; set; }
    }

    public class WeiXinParam
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
    }


    public class WeiXinConfig
    {
        public string AppID { get; set; }

        public string AppSecret { get; set; }
        /// <summary>
        /// 微信登录地址
        /// </summary>
        public string LoginUrl { get; set; }
        /// <summary>
        /// 微信登录回调地址
        /// </summary>
        public string LoginBackUrl { get; set; }

        public string Encoding { get; set; }
        /// <summary>
        /// 获取用户基本信息地址
        /// </summary>
        public string GetUserInfoUrl { get; set; }
        /// <summary>
        /// 共用token地址调用基础api时使用和网页的token、JSAPI的token不相同
        /// </summary>
        public string CommonAccessTokenUrl { get; set; }
        /// <summary>
        /// 推送模板消息地址
        /// </summary>
        public string TemplateMessageUrl { get; set; }
        /// <summary>
        /// 网页的token地址和共用、JSAPI的token不相同
        /// </summary>
        public string WebAccessTokenUrl { get; set; }
        /// <summary>
        /// 微信jssdk需要的ticket
        /// </summary>
        public string JsapiTicketUrl { get; set; }
        public List<string> JsApis { get; set; }
    }

    [XmlRoot("xml")]
    public class WXBaseMessage
    {
        /// <summary>
        ///  开发者微信号
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        ///  开发者微信号发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; set; }
        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>

        public Int64 CreateTime { get; set; }

        /// <summary>
        /// 消息id，64位整型
        /// </summary>

        public Int64 MsgId { get; set; }
    }

    [XmlRoot("xml")]
    public class WXTextMessage : WXBaseMessage
    {
        public string Content { get; set; }
        public string MsgType { get; set; }= WXMsgType.text.ToString();
    }
    [XmlRoot("xml")]
    public class WXImageMessage : WXBaseMessage
    {
        /// <summary>
        /// 图片链接（由系统生成）
        /// </summary>

        public string PicUrl { get; set; }

        public string MsgType { get; set; }=WXMsgType.image.ToString();
        /// <summary>
        /// 图片消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>

        public int MediaId { get; set; }
    }
    [XmlRoot("xml")]
    public class WXVoiceMessage : WXBaseMessage
    {
        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>

        public string Format { get; set; }

        public string MsgType { get; set; } = WXMsgType.voice.ToString();
        /// <summary>
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>

        public string MediaId { get; set; }
        /// <summary>
        /// 语音识别结果，UTF8编码
        /// </summary>

        public string Recognition { get; set; }
    }
    [XmlRoot("xml")]
    public class WXVideoMessage : WXBaseMessage
    {
        /// <summary>
        ///视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>

        public string ThumbMediaId { get; set; }

        public string MsgType { get; set; } = WXMsgType.video.ToString();
        /// <summary>
        /// 视频消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>

        public int MediaId { get; set; }
    }
    public enum WXMsgType
    {
        [Description("文本")]
        text = 0,
        [Description("图片")]
        image = 1,
        [Description("视频")]
        video = 2,
        [Description("语音")]
        voice = 3,
    }

}
