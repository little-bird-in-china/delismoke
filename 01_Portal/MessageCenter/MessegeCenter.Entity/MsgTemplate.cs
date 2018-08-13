
using BlueStone.Utility;
using MessageCenter.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{    public class MsgTemplate 
    {

        /// <summary>
        /// 系统编号 
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 发送短信的动作名称
        /// </summary>
        public string ActionCode { get; set; }
        /// <summary>
        /// 同一条消息同一个接受者最大的发送次数
        /// </summary>
        public int LimitCount { get; set; }
        /// <summary>
        /// 消息发送的频率（单位为秒）
        /// </summary>
        public int SendFrequency { set; get; }
        /// <summary>
        /// 微信跳转url
        /// </summary>
        public string Url { set; get; }
        ///// <summary>
        ///// 租户ID
        ///// </summary>
        //public string TenantID { get; set; }

        public int CompanySysNo { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgType MsgType { get; set; }


        /// <summary>
        /// 模版内容
        /// </summary>
        public string TemplateContent { get; set; }
        public string TemplateName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        public MsgReceiverType ReceiverType { get; set; }

        public string MsgTypeStr
        {
            get { return EnumHelper.GetDescription(MsgType); }
        }
        public string EnabledStr
        {
            get { return Enabled ? "启用" : "禁用"; }
        }

        public long EditUserSysNo { get; set; }
        public string EditUserName { get; set; }
        public DateTime? EditDate { get; set; }
        public string ExternalTemplateID { get; set; }
    }
    public class QF_MsgTemplate : QueryFilter
    {
        /// <summary>
        /// 发送短信的动作名称
        /// </summary>
        public string ActionCode { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MsgType? MsgType { get; set; }

        //public string TenantID { get; set; }
        public int CompanySysNo { get; set; }
    } 
}
