using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;
 
namespace BlueStone.Smoke.Entity
{
    public class CompanyInstaller  
    {

        /// <summary>
        /// 公司编号：
        /// </summary>
        public int CompanySysNo { get; set; }

        /// <summary>
        /// 安装人员用户编号：
        /// </summary>
        public int InstallerSysNo { get; set; }
    }


    public class QF_CompanyInstaller : QueryFilter {
        public int? CompanySysNo { get; set; }
    }

}