using BlueStone.Smoke.Entity;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class Role : EntityBase
    {

        /// <summary>
        /// 角色编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string RoleName { get; set; }


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

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationID { get; set; }

        public string ApplicationName { get; set; }
    }
}
