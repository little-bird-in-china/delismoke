using BlueStone.Smoke.Entity.AuthEnum;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class QF_SysMenu : QueryFilter
    {

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public BlueStone.Smoke.Entity.AuthEnum.PageType Type { get; set; }


        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus? CommonStatus { get; set; }

        //父节点SysNo
        public int? ParentSysNo { get; set; }
    }
}
