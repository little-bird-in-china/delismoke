using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class WechatResult
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
    /// 微信推送返回消息
    /// </summary>
    public class WeiXinMsgResult : WechatResult
    {
        [JsonProperty("errcode")]
        public int MsgId { get; set; }
    }
    public class WeiXinTokenResult : WechatResult
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
    }
}
