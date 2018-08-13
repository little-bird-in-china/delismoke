using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MessageCenter.Entity
{
    public class MsgTemplateUser : EntityBase
    {

        /// <summary>
        /// 系统编号 
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 模板编号
        /// </summary>
        public string MsgTemplateSysNo { get; set; }


        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserSysNo { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; set; }

        public string ExternalTemplateID { get; set; }
    }
}
