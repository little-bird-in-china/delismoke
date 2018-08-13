using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class QF_SystemUser : QueryFilter
    {

        /// <summary>
        /// 
        /// </summary>
        public int? SysNo { get; set; }


        /// <summary>
        /// 要求全局唯一
        /// </summary>
        //public string LoginName { get; set; }

        /// <summary>
        /// LoginName/UserFullName/CellPhone
        /// </summary>
        public string KeyWords { get; set; }

        /// <summary>
        /// 所属公司sysno
        /// </summary>
        public int? MasterSysNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public string UserFullName { get; set; }


        /// <summary>
        /// 
        /// </summary>
       // public string CellPhone { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除是将状态设置为-1
        /// </summary>
        public CommonStatus? CommonStatus { get; set; }
        public bool IsPlatformManager { get; set; }
        public string ApplicationID { get; set; }
    }
}
