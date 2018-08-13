using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class SmokeDetectorMessage: QueryResult
    {
        public int SysNo { get; set; }
        public string MasterID { get; set; }

        public string ActionCode { get; set; }

        public MsgType MsgType { get; set; }

        public string MsgTypeStr { get
            {
                return EnumHelper.GetDescription(MsgType);
            }
        }

        public string MsgContent { get; set; }
    
        public string MsgReceiver { get; set; }

        public DateTime? HandleTime { get; set; }
        public string HandleTimeStr
        {
            get { return HandleTime.HasValue ? HandleTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
        }
        public string WechatName { get; set; }
    } 

    public class QF_SmokeDetectorMessage :QueryResult
    {
        public string MasterName { get; set; }

        public string MasterID { get; set; }

        public MsgType? MsgType { get; set; }

    }

    public enum MsgType
    {
        [Description("短信")]
        Phone = 1,
        [Description("微信")]
        Wechat = 2,
    }
}