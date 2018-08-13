using BlueStone.Smoke.Entity.AuthEnum;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System.Collections.Generic;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class SysMenu : EntityBase
    {
        public SysMenu()
        {
            Permissions = new List<SysPermission>();
            Childrens = new List<SysMenu>();
        }

        /// <summary>
        /// 菜单编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 菜单父级编号
        /// </summary>
        public int ParentSysNo { get; set; }


        /// <summary>
        /// 自动生成，三位一级
        /// </summary>
        public string SysCode { get; set; }


        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }


        /// <summary>
        /// ControllerName_ActionName
        /// </summary>
        public string MenuKey { get; set; }

        /// <summary>
        /// 图标样式
        /// </summary>
        public string IconStyle { get; set; }


        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkPath { get; set; }


        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplay { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int SortIndex { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public BlueStone.Smoke.Entity.AuthEnum.PageType Type { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Memo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ApplicationID { get; set; }


        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus CommonStatus { get; set; }
        /// <summary>
        /// 状态：状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public string CommonStatusStr { get { return EnumHelper.GetDescription(CommonStatus); } }




        public int ChildrenCount { get; set; }

        public List<SysMenu> Childrens { get; set; }

        public List<SysPermission> Permissions { get; set; }
    }
    
}
