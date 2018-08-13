using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
