using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlueStone.Smoke.Entity
{
    public class SmokeDetector : QueryResult
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 公司编号：
        /// </summary>
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 烟感器编号：必须唯一
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public string AddressCode { get; set; }


        public string LngLat { get; set; }
        public bool IsUnbound { get; set; }
        /// <summary>
        /// ：
        /// </summary>
        public string AddressName { get; set; }

        public int? AddressSysNo { get; set; }

        /// <summary>
        /// 位置：如：进门左边，进门右边，进门正前方，进门左前方，进门右前方
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public SmokeDetectorStatus? Status { get; set; }

        /// <summary>
        /// ：
        /// </summary>
        public string StatusStr { get { return EnumHelper.GetDescription(Status); } }

        public UISmokeDetectorStatus? UIStatus
        {
            get
            {
                if (Status == SmokeDetectorStatus.OutNet ||
                      Status == SmokeDetectorStatus.Offline ||
                      Status == SmokeDetectorStatus.Lost)
                {
                    return UISmokeDetectorStatus.OffLine;
                }
                else if (Status == SmokeDetectorStatus.Warning || Status == SmokeDetectorStatus.TestWarning)
                {
                    return UISmokeDetectorStatus.FireWarning;
                }
                else if (Status == SmokeDetectorStatus.LowPower)
                {
                    return UISmokeDetectorStatus.LowPowerWarning;
                }
                return UISmokeDetectorStatus.Online;

            }
        }

        public string UIStatusStr
        {

            get { return EnumHelper.GetDescription(UIStatus); }

        }

        /// <summary>
        /// 备注：
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 安装图片：
        /// </summary>
        public string InstalledImg { get; set; }

        /// <summary>
        /// 安装人系统编号：
        /// </summary>
        public int? InstallerSysNo { get; set; }

        /// <summary>
        /// 安装人：
        /// </summary>
        public string InstallerName { get; set; }

        /// <summary>
        /// 安装时间：
        /// </summary>
        public DateTime? InstalledTime { get; set; }

        /// <summary>
        /// 安装时间：
        /// </summary>
        public string InstalledTimeStr { get { return InstalledTime.HasValue ? InstalledTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }

        /// <summary>
        /// 创建者系统编号：
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建人：
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间：
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 创建时间：
        /// </summary>
        public string InDateStr { get { return InDate.HasValue ? InDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }

        /// <summary>
        /// ：最后修改人系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 修改人：最后修改人显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 修改时间：最后修改时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 修改时间：最后修改时间
        /// </summary>
        public string EditDateStr { get { return EditDate.HasValue ? EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty; } }

        public List<SmokeDetectorStatusLog> MessageList { get; set; }


        public string SFDID { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Tags { get; set; }
        public string Protocol { get; set; }
        public string Location_Lon { get; set; }
        public string Location_Lat { get; set; }
        public string Location_Ele { get; set; }
        //public string Imei { get { return Code; } }
        public string Imsi { get; set; }
        public string Obsv { get; set; }
        public string DeviceId { get; set; }
        public string IsOnLine { get; set; }

        public string DeviceStatus { get; set; }
        public string DeviceStatusText
        {
            get
            {
                return "";
            }
        }

        public string RecivedJsonData { get; set; }
    }


    public class QF_UserDetector : QueryFilter
    {
        public int ClientSysNo { get; set; }

        public UISmokeDetectorStatus? Status { get; set; }
    }

    public enum UISmokeDetectorStatus
    {
        [Description("在线")]
        Online = 1,
        [Description("离线")]
        OffLine = 2,
        [Description("火灾报警")]
        FireWarning = 3,
        [Description("低电量报警")]
        LowPowerWarning = 4,
    }
    public enum SmokeDetectorStatus
    {
        [Description("已删除")] Delete = -999,
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
        /// 0110	烟雾消警         
        /// </summary>
        [Description("烟雾消警")] CancelWarning = 3,
        /// <summary>
        /// 0111	静音命令         
        /// </summary>
        [Description("静音命令")] Mute = 4,
        /// <summary>
        /// 1000	设备入网         
        /// </summary>
        [Description("设备入网")] InNet = 5,

        /// <summary>
        /// 1010	修改服务器地址   
        /// </summary>
        [Description("修改服务器地址")] EditServer = 6,
        /// <summary>
        /// 1011	常连接心跳       
        /// </summary>
        [Description("常连接心跳")] ActiveBeat = 7,
        /// <summary>
        /// 1100	底座低电压       
        /// </summary>
        [Description("底座低电压")] LampstandLowPower = 8,

        /// <summary>
        /// 设备上线
        /// </summary>
        [Description("设备上线")] Online = 9,

        /// <summary>
        /// 1001	设备消网         
        /// </summary>
        [Description("设备消网")]
        OutNet = 78,
        /// <summary>
        /// 1101	探头失联         
        /// </summary>
        [Description("探头失联")] Lost = 79,

        /// <summary>
        /// 1111	设备离线         
        /// </summary>
        [Description("设备离线")]
        Offline = 80,
        /// <summary>
        /// 0101	低电压报警       
        /// </summary>
        [Description("低电量报警")]
        LowPower = 90,
        /// <summary>
        /// 0100	测试报警         
        /// </summary>
        [Description("测试报警")]
        TestWarning = 99,
        /// <summary>
        /// 0011	烟雾报警         
        /// </summary>
        [Description("烟雾报警")]
        Warning = 100,

    }
}