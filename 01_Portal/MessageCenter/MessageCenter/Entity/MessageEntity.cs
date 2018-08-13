using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class MessageEntity
    {
        public int SysNo { get; set; }

        public string ActionCode { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantID { get; set; }
        /// <summary>
        /// 消息发送类型
        /// </summary>
        public MsgType MsgType { get; set; }
        /// <summary>
        /// 接收者
        /// 例如：手机号、邮箱地址
        /// </summary>
        public string MsgReceiver { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 消息状态
        /// </summary>
        public MessageStatus Status { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// 外部模版id
        /// </summary>
        public string ExternalTemplateID { get; set; }

        private List<MsgTemplateParmater> _templateParmaters;
        public string Parmaters
        {
            get
            {
                string parmaters = Utility.SerializeToString(_templateParmaters, new System.Xml.Serialization.XmlRootAttribute("Parmaters"));
                return parmaters;
            }
            set
            {
                _templateParmaters = Utility.DeserializeFromString(value, typeof(List<MsgTemplateParmater>), new System.Xml.Serialization.XmlRootAttribute("Parmaters")) as List<MsgTemplateParmater>;

            }
        }

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<MsgTemplateParmater> TemplateParmaters
        {
            get
            {
                return  _templateParmaters;
            }
            set 
            {
                _templateParmaters = value;
            }

        }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP { get; set; }
    }
    public enum MsgType
    {
        [Description("短信")]
        PhoneMessage = 1,
        [Description("微信")]
        WeiXin = 2,
        [Description("安卓")]
        Android = 3,
        [Description("苹果")]
        IOS = 4
    }
    public enum MessageStatus
    {
        [Description("待发送")]
        待发送 = 0,
        [Description("已发送")]
        已发送 = 1
    }
}
