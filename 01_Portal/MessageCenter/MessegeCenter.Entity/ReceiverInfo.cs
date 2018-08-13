using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    /*
     * 接受客户端类型
     */
    public class ReceiverInfo
    {
        /// <summary>
        /// 消息发送类型
        /// </summary>
        public MsgType MsgType { get; set; }
        /// <summary>
        /// 消息接收账号，根据MsgType的不同，此值可能是手机号，微信OpenID
        /// </summary>
        public string ReceiverNo { get; set; }
    }
}
