using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity.Common
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

    public class WechatTokenResult : WechatResult
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
}
