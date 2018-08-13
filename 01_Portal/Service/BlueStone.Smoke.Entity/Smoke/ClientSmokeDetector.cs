using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;
 
namespace BlueStone.Smoke.Entity
{
    public class ClientSmokeDetector
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 用户编号：
        /// </summary>
        public int ClientSysNo { get; set; }

        /// <summary>
        /// 烟感器编码：
        /// </summary>
        public string SmokeDetectorCode { get; set; }

        /// <summary>
        /// 是否使用默认的紧急电话：
        /// </summary>
        public bool IsDefaultCellPhone { get; set; }

        /// <summary>
        /// 手机1：
        /// </summary>
        public string CellPhone { get; set; }

        /// <summary>
        /// 手机2：
        /// </summary>
        public string CellPhone2 { get; set; }

        /// <summary>
        /// 手机3：
        /// </summary>
        public string CellPhone3 { get; set; }

        /// <summary>
        /// 修改时间：
        /// </summary>
        public DateTime EditTime { get; set; }

        /// <summary>
        /// 修改时间：
        /// </summary>
        public string EditTimeStr { get { return EditTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        /// <summary>
        /// App客户编号：微信的OpenID
        /// </summary>
        public string AppCustomerID { get; set; }
        /// <summary>
        /// 微信名字
        /// </summary>
        public string Name {get;set;}

    } 

}