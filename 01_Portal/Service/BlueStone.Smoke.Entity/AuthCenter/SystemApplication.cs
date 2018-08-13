using BlueStone.Utility;
using System.Collections.Generic;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class SystemApplication : EntityBase
    {

        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ApplicationID { get; set; }


        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus CommonStatus { get; set; }

        /// <summary>
        /// 记录usersysno rolesysno,方便查询
        /// </summary>
        public int BizSysNo { get; set; }

    }

    public class SystemApplicationComparer: IEqualityComparer<SystemApplication>
    {
        public bool Equals(SystemApplication x, SystemApplication y)
        {
            return x.ApplicationID == y.ApplicationID;
        }

        public int GetHashCode(SystemApplication obj)
        {
            return base.GetHashCode();
        }
    }

}
