
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class Tenant
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }


        /// <summary>
        /// 企业（商家）唯一编码
        /// </summary>
        public string TenantID { get; set; }


        /// <summary>
        /// 企业（商家）名称
        /// </summary>
        public string TenantName { get; set; }


        /// <summary>
        /// 企业电话
        /// </summary>
        public string TenantTel { get; set; }


        /// <summary>
        /// 联系地址
        /// </summary>
        public string TenantAddress { get; set; }


        /// <summary>
        /// 联系人
        /// </summary>
        public string TenantContact { get; set; }


        /// <summary>
        /// 联系电话
        /// </summary>
        public string TenantContactTel { get; set; }

        /// <summary>
        /// 微信APPID
        /// </summary>
        public string WebChatAPPID { get; set; }
        /// <summary>
        /// 微信APPSecret
        /// </summary>
        public string WebChatAPPSecret { get; set; }
        /// <summary>
        /// 微信授权回调地址
        /// </summary>
        public string WebChatCallbackUrl { get; set; }
        /// <summary>
        /// 微信公众号二维码
        /// </summary>
        public string WebChat2DCodeImg { get; set; }
        /// <summary>
        /// 微信公众号名称
        /// </summary>
        public string WebChatName { get; set; }

        public int? AreaSysNo { get; set; }
        /// <summary>
        /// 租户B2C前台网站地址
        /// </summary>
        public string FrontUrl { get; set; }
        /// <summary>
        /// 租户logo地址
        /// </summary>
        public string TenantLogo { get; set; }
    }
}
