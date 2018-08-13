using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class ReceiverInfo
    {
        /// <summary>
        /// 短信
        /// </summary>
        public string PhoneMessage { get; set; }
        /// <summary>
        /// 微信OpenID
        /// </summary>
        public string WechatOpenID { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// IOS推送
        /// </summary>
        public string IOS { get; set; }
        /// <summary>
        /// 安卓推送
        /// </summary>
        public string Android { get; set; }
    }
}
