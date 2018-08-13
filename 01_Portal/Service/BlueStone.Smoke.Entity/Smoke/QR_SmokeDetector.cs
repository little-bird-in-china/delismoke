using BlueStone.Utility;
using System;
using System.Collections.Generic;

namespace BlueStone.Smoke.Entity
{
    public class QR_SmokeDetector : QueryResult
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

        public bool IsUnbound { get; set; }
        /// <summary>
        /// ：
        /// </summary>
        public string AddressCode { get; set; }


        public string AddressSysNo { get; set; }
        /// <summary>
        /// ：
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// 位置：如：进门左边，进门右边，进门正前方，进门左前方，进门右前方
        /// </summary>
        public string Position { get; set; }

        public string PositionAddress
        {
            get { return string.Concat(AddressName, " ", Position); }
        }

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

        public string DeviceID { get; set; }
        public string ClientName { get; set; }
    }

    public class SmokeDetectorCount
    {
        /// <summary>
        /// 所有设备
        /// </summary>
        public int ALLSmokeCount { get; set; }

        /// <summary>
        /// 在线设备
        /// </summary>
        public int OnlineCount { get; set; }

        /// <summary>
        /// 离线设备
        /// </summary>
        public int OfflineCount { get; set; }

        /// <summary>
        /// 报警设备
        /// </summary>
        public int WarningCount { get; set; }

        /// <summary>
        /// 低电量设备
        /// </summary>
        public int LowPowerCount { get; set; }

    }

    public class SmokeDetectorCountAddress : SmokeDetectorCount
    {

        public int AddressSysNo { get; set; }

    }

    public class QF_SmokeDetector : QueryFilter
    {
        public UISmokeDetectorStatus? Status { get; set; }
        /// <summary>
        /// 公司编号：
        /// </summary>
        public int? CompanySysNo { get; set; }

        public string AddressCode { get; set; }
        public int? InstallerSysNo { get; set; }

        public string Code { get; set; }

        public string DeviceID { get; set; }

        public List<int> InstallSysNos { get; set; }

        public string ClientName { get; set; }
    }


}