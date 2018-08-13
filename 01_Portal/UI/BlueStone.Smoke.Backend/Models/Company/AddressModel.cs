using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueStone.Smoke.Backend.Models
{
    public class AddressManagerModel
    {
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 公司地区
        /// </summary>
        public int? AreaSysNo { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public List<SystemUser> ManagerList {get;set;}
    }
    
}