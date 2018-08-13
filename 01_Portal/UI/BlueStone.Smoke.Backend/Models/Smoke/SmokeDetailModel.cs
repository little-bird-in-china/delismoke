using BlueStone.Smoke.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MessageCenter.Entity;

namespace BlueStone.Smoke.Backend.Models
{
    public class SmokeDetailModel
    {
        /// <summary>
        /// 设备信息
        /// </summary>
        public SmokeDetector DetectorInfo { get; set; }

        /// <summary>
        /// 消息历史
        /// </summary>
        public List<MessageEntity> MsgHistory { get; set; }


        public List<SmokeDetectorStatusLog> LogList { get; set; }
        /// <summary>
        /// 关注的客户列表
        /// </summary>
        public List<Client> ClientList { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}