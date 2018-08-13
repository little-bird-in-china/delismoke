using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class SmokeDetectorStatusLog
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 烟感器编号：
        /// </summary>
        public string SmokeDetectorCode { get; set; }

        /// <summary>
        /// 之前的状态：
        /// </summary>
        public SmokeDetectorStatus? PreStatus { get; set; }

        /// <summary>
        /// 当前状态：
        /// </summary>
        public SmokeDetectorStatus? Status { get; set; }

        public string StatusStr
        {
            get { return Status.HasValue ? EnumHelper.GetDescription(Status) : ""; }
        }


        public string ReceivedJsonData { get; set; }

        /// <summary>
        /// 当前状态开始时间：
        /// </summary>
        public DateTime? BeginTime { get; set; }

        public string BeginTimeStr
        {
            get { return BeginTime.HasValue ? BeginTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : ""; }
        }

        /// <summary>
        /// 当前状态持续时间（单位秒）：
        /// </summary>
        public int? DurationSeconds { get; set; }
    }

    public class QF_SmokeDetectorStatusLog:QueryFilter
    {
        public string DeviceCode { get; set; }
    }
}