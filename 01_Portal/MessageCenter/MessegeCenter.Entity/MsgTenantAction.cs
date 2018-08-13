using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class MsgTenantAction
    {
        /// <summary>
        /// 
        /// </summary>
        public string TenantID { get; set; }


        /// <summary>
        /// 发送短信的动作名称
        /// </summary>
        public string ActionCode { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { get; set; }
    }
}
