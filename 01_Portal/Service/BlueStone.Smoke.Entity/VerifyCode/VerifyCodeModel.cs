using BlueStone.Smoke.Entity; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueStone.Smoke.Entity
{
    public class VerifyCodeModel
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        public int CompanySysNo { get; set; } 

        public string Sender { get; set; }

        public string VerifyCode { get; set; }

        public DateTime? VerifyTime { get; set; }

        public string VerifyTimeStr
        {
            get
            {
                if (VerifyTime.HasValue)
                {
                    return VerifyTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return "";
            }
        }
        public DateTime? VerifyExpireTime { get; set; }
        public string VerifyExpireTimeStr
        {
            get
            {
                if (VerifyExpireTime.HasValue)
                {
                    return VerifyExpireTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return "";
            }

        }
    }
}