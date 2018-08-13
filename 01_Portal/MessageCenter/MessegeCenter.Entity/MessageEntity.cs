using BlueStone.Utility;
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
        //test
        public int SysNo { get; set; }

        public string ActionCode { get; set; }

        /// <summary>
        /// 消息对象
        /// </summary>
        public string MasterName { get; set; }

        /// <summary>
        /// 对象ID
        /// </summary>
        public string MasterID { get; set; }
        public int CompanySysNo { get; set; }
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
        /// 插入时间
        /// </summary>
        public DateTime InDate { get; set; }


        public string InDateStr { get { return InDate.ToString("yyyy-MM-dd HH:mm:ss"); } }
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
                return _templateParmaters;
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

        /// <summary>
        /// 同一条消息同一个接受者最大的发送次数
        /// </summary>
        public int LimitCount { get; set; }
        /// <summary>
        /// 消息发送的频率（单位为秒）
        /// </summary>
        public int SendFrequency { set; get; }
        /// <summary>
        /// 已发送次数
        /// </summary>
        public int SendCount { get; set; }
        /// <summary>
        /// 上一次发送时间
        /// </summary>
        public DateTime LastSendTime { get; set; }

        public string Url { get; set; }
    }
    public class QR_Message : MessageEntity
    {

        /// <summary>
        /// 烟感器编号：必须唯一
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public string AddressCode { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// 位置：如：进门左边，进门右边，进门正前方，进门左前方，进门右前方
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public SmokeDetectorStatus? SmokeDetectorStatus { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public string SmokeDetectorStatusStr { get { return EnumHelper.GetDescription(SmokeDetectorStatus); } }

        /// <summary>
        /// 备注：
        /// </summary>
        public string Memo { get; set; }
    }

    public class QF_Message
    {
        /// <summary>
        /// 消息对象
        /// </summary>
        public string MasterName { get; set; }

        /// <summary>
        /// 对象ID
        /// </summary>
        public string MasterID { get; set; }


        public DateTime? BeginInDate { get; set; }


        public DateTime? EndInDate { get; set; }

        public string ActionCode { get; set; }

        public string MsgReceiver { get; set; }

        /// <summary>
        /// 消息发送类型
        /// </summary>
        public MsgType? MsgType { get; set; }

    }
    public enum SmokeDetectorStatus
    {
        /// <summary>
        /// 0000	Reserve          
        /// </summary>
        [Description("Reserve")] Reserve = 0,
        /// <summary>
        /// 0001	设备开机         
        /// </summary>
        [Description("设备开机")] Start = 1,
        /// <summary>
        /// 0010	设备心跳         
        /// </summary>
        [Description("设备心跳")] Beat = 2,
        /// <summary>
        /// 0011	烟雾报警         
        /// </summary>
        [Description("烟雾报警")] Warning = 3,
        /// <summary>
        /// 0100	测试报警         
        /// </summary>
        [Description("测试报警")] TestWarning = 4,
        /// <summary>
        /// 0101	低电压报警       
        /// </summary>
        [Description("低电压报警")] LowPower = 5,
        /// <summary>
        /// 0110	烟雾消警         
        /// </summary>
        [Description("烟雾消警")] CancelWarning = 6,
        /// <summary>
        /// 0111	静音命令         
        /// </summary>
        [Description("静音命令")] Mute = 7,
        /// <summary>
        /// 1000	设备入网         
        /// </summary>
        [Description("设备入网")] InNet = 8,
        /// <summary>
        /// 1001	设备消网         
        /// </summary>
        [Description("设备消网")] OutNet = 9,
        /// <summary>
        /// 1010	修改服务器地址   
        /// </summary>
        [Description("修改服务器地址")] EditServer = 10,
        /// <summary>
        /// 1011	常连接心跳       
        /// </summary>
        [Description("常连接心跳")] ActiveBeat = 11,
        /// <summary>
        /// 1100	底座低电压       
        /// </summary>
        [Description("底座低电压")] LampstandLowPower = 12,
        /// <summary>
        /// 1101	探头失联         
        /// </summary>
        [Description("探头失联")] Lost = 13,

        ///// <summary>
        ///// 在线
        ///// </summary>
        //[Description("在线")]
        //Online = 1,
        ///// <summary>
        ///// 离线
        ///// </summary>

        //[Description("离线")]
        //Offline = 2,
    }
    public enum MsgType
    {
        [Description("短信")]
        SMS = 1,
        [Description("微信")]
        WeiXin = 2
    }
    public enum MessageStatus
    {
        [Description("待发送")]
        Sending = 0,
        [Description("已发送")]
        Sent = 1,
        [Description("客户拒收")]
        CustomerReject = 2
    }
}
