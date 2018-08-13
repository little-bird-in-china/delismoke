using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueStone.Smoke.Entity
{
    [Serializable]
    public class CurrentUser
    {

        public int CompanySysNo { get {
                return MasterSysNo.GetValueOrDefault();} }

        public int IncubatorsSysNo { get; set; }

        public string CompanyName { get; set; }
        /// <summary>
        /// 操作人系统编号
        /// </summary>
        public int UserSysNo { get; set; }
        /// <summary>
        /// 为平台管理员
        /// </summary>
        public bool IsPMAdmin { get; set; }
        public int? MasterSysNo { get; set; } 

        /// <summary>
        /// 操作人显示名称
        /// </summary>
        public string UserDisplayName { get; set; }

        public string AvatarImageUrl { get; set; } 

        public string UserName { get; set; }

        public string LoginTime { get; set; }

    }
}
