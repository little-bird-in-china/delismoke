using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class SmokeDetectorDayReport : EntityBase
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 统计时间：
        /// </summary>
        public string DayDate { get; set; }

        /// <summary>
        /// 公司编号：
        /// </summary>
        public int CompanySysNo { get; set; }

        public string CompanyName { get; set; }

        /// <summary>
        /// 烟感器总数：
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 在线数：
        /// </summary>
        public int OnlineCount { get; set; }
        public int LowPowerCount { get; set; }
        public int FireCount { get; set; }
        public DateTime InTime { get; set; }
        
    }
    public class QF_SmokeDayReport
    {
        public int? CompanySysNo { get; set; }
        public DateTime? StartDayDate { get; set; }
        public DateTime? EndDayDate { get; set; }
    }


    public class CompanyDayReport
    {
        public int CompanySysNo { get; set; }
        public List<DayReport> TotalCount { get; set; }
        public List<DayReport> OnlineCount { get; set; }
        public List<DayReport> LowPowerCount { get; set; }
        public List<DayReport> FireCount { get; set; }
        public List<DayReport> OffLineCount { get; set; }
    }
    public class DayReport
    {
        public string DayDate { get; set; }
        public int Count { get; set; }
        public string Percent { get; set; } = "";
    }
}