using BlueStone.Smoke.Entity;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class SysPermission : EntityBase
    {

        /// <summary>
        /// 权限编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 页面才能拥有权限
        /// </summary>
        public int MenuSysNo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string MenuSysCode { get; set; }


        /// <summary>
        /// 所属功能编号
        /// </summary>
        public int FunctionSysNo { get; set; }


        /// <summary>
        /// 自动生成，三位一级
        /// </summary>
        public string FunctionSysCode { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string ApplicationID { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string PermissionName { get; set; }


        /// <summary>
        /// 代码使用这个字段判断是否有权限
        /// </summary>
        public string PermissionKey { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Memo { get; set; }


        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除为-1
        /// </summary>
        public CommonStatus CommonStatus { get; set; }

        public string CommonStatusStr
        {
            get
            {
                return EnumHelper.GetDescription(CommonStatus);
            }
        }

    }
}
