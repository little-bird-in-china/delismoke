using BlueStone.Smoke.Entity;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class QF_SysPermission : QueryFilter
    {

        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus? CommonStatus { get; set; }

        public int? FunctionSysNo { get; set; }

    }
}
