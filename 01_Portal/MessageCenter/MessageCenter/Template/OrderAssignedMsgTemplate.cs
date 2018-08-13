using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Template
{
    /// <summary>
    /// 【XXX】您有新订单{1}需要处理，收货人：{2}({3})，请登录分销系统及时处理。
    /// </summary>
    public class OrderAssignedMsgTemplate : BaseMsgTemplate
    {
        public OrderAssignedMsgTemplate()
            : base()
        {
            SendCount = -1;
        }
        public override string TemplateCode
        {
            get
            {
                return "OrderAssigned";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "订单分配后消息模板";
            }
        }

        public override MsgReceiverType ReceiverType
        {
            get
            {
                return MsgReceiverType.Distibutor;
            }
        }

        /// <summary>
        /// 订单号
        /// </summary>
        [MsgTemplateParameterDescription("订单号", 1)]
        public string OrderSysNo { get; set; }

        /// <summary>
        /// 收货人名称
        /// </summary>
        [MsgTemplateParameterDescription("收货人名称", 2)]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收货人电话或手机
        /// </summary>
        [MsgTemplateParameterDescription("收货人电话", 3)]
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 第三方平台订单号
        /// </summary>
        [MsgTemplateParameterDescription("第三方平台名称", 4)]
        public string ChannelName { get; set; }

        /// <summary>
        /// 第三方平台订单号
        /// </summary>
        [MsgTemplateParameterDescription("第三方平台订单号", 5)]
        public string ChannelOrderSysNo { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [MsgTemplateParameterDescription("签名", 6)]
        public string Sign { get; set; }

    }
}
