using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System.Collections.Generic;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class SysFunction : EntityBase
    {

        /// <summary>
        /// 功能权限编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int ParentSysNo { get; set; }


        /// <summary>
        /// 自动生成，三位一级
        /// </summary>
        public string SysCode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string FunctionName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ApplicationID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Memo { get; set; }


        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus CommonStatus { get; set; }

        public int ChildrenCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SortIndex { get; set; }

        public List<SysPermission> Permissions { get; set; }
}
}
