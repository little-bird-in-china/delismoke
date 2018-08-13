namespace BlueStone.Smoke.Msite
{
    public class AppUserInfo
    {
        /// <summary>
        /// 用户系统编号
        /// </summary>
        public int UserSysNo { get; set; }


        /// <summary>
        /// 登录名称
        /// </summary>
        public string UserID { get; set; }


        /// <summary>
        /// 显示名称
        /// </summary>        
        public string UserDisplayName { get; set; }


        public bool RememberLogin { get; set; }


        public string LastLoginDateText { get; set; }

        public string HeadImage { get; set; }

        /// <summary>
        /// 微信登陆的openID
        /// </summary>
        public string AppCustomerID { get; set; }

        public int? ManagerSysNo { get; set; }

        public UserType? UserType { get; set; }

        public string ManagerLoginName { get; set; }

        public string ManagerName { get; set; }
    }


    public enum UserType
    {
        Common = 1,//普通用户
        Manager = 2,//管理员  
        Installer = 3//安装人员
    }
}