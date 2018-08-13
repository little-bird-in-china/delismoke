using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class Client
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 用户名称：
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 头像地址：
        /// </summary>
        public string HeaderImage { get; set; }

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
        /// App客户编号：微信的OpenID
        /// </summary>
        public string AppCustomerID { get; set; }

        /// <summary>
        /// 注册时间：
        /// </summary>
        public DateTime RegisterTime { get; set; }

        /// <summary>
        /// 注册时间：
        /// </summary>
        public string RegisterTimeStr { get { return RegisterTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        /// <summary>
        /// 绑定的管理员账号：
        /// </summary>
        public int? ManagerSysNo { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        public int? CompanySysNo { get; set; }

        /// <summary>
        /// 修改时间：
        /// </summary>
        public DateTime EditTime { get; set; }

        /// <summary>
        /// 修改时间：
        /// </summary>
        public string EditTimeStr { get { return EditTime.ToString("yyyy-MM-dd HH:mm:ss"); } }

        public CommonStatus? ManagerStatus { get; set; }

        public string ManagerLoginName { get; set; }

        public string ManagerName { get; set; }

    }

    public class ClientFilter : QueryFilter
    {

        public int? CompanySysNo
        {
            get; set;
        }

        public string SmokeDetectorCode
        {
            get; set;
        }

        /// <summary>
        /// 从名称和手机号中查询 
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// 精确手机号查询 
        /// </summary>
        public string ExactCellphone { get; set; }

    }
}