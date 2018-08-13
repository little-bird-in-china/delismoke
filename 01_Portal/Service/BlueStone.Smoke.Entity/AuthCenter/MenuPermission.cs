using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class MenuPermission
    {

        /// <summary>
        /// 权限编号：
        /// </summary>
        public int PermissionSysNo { get; set; }

        /// <summary>
        /// 所属页面编号：
        /// </summary>
        public int? MenuSysNo { get; set; }
    }
}
