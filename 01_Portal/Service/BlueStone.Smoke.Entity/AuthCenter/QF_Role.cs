using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class QF_Role : QueryFilter
    {
        public int? SysNo { get; set; }
        
        public string RoleName { get; set; }

        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除是将状态设置为-1
        /// </summary>
        public BlueStone.Smoke.Entity.CommonStatus? CommonStatus { get; set; }

        public string ApplicationID { get; set; }
    }
}
