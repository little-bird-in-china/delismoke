using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlueStone.Smoke.Entity.AuthCenter
{
    public class SystemUser : EntityBase
    {

        public SystemUser()
        {
            Applications = new List<SystemApplication>(5);
            RoleList = new List<AuthCenter.Role>();
        }

        /// <summary>
        /// 
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 要求全局唯一
        /// </summary>
        public string LoginName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string LoginPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserFullName { get; set; }
        /// <summary>
        /// 公司编号
        /// </summary>
        public int? MasterSysNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Gender Gender { get; set; }

        public string GenderStr {
            get
            {
                return EnumHelper.GetDescription(Gender);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CellPhone { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string QQ { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string AvatarImageUrl { get; set; }

        /// <summary>
        /// 地区：地区
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        ///地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CommonStatus CommonStatus { get; set; }

        public string CommonStatusStr
        {
            get
            {
                string str = string.Empty;
                switch (CommonStatus)
                {
                    case CommonStatus.Actived:
                        str = "有效";
                        break;
                    case CommonStatus.DeActived:
                        str = "无效";
                        break;
                    case CommonStatus.Deleted:
                        str = "已删除";
                        break;
                    default:
                        str = EnumHelper.GetDescription(CommonStatus);
                        break;
                }
                return str;
            }
        }
        public string ApplicationID { get; set; }

        /// <summary>
        /// 用户所属系统
        /// </summary>
        public List<SystemApplication> Applications { get; set; }


        public List<Role> RoleList { get; set; }
      
        
        //  public int? TotalItems { get; set; }
    }
    public enum UserCommonStatus : int
    {
        [Description("有效")]
        Actived = 1,
        [Description("待审核")]
        DeActived = 0,
        [Description("无效")]
        Deleted = -1
    }
}
