using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity
{
    public  class WebPortalHelper
    {
        /// <summary>
        /// ERP应用编号
        /// </summary>
        public static string ApplicationID_ERP = ConfigurationManager.AppSettings["ErpApplicationID"];
        public static bool IsPhonenum(string phonenum)
        {
            if (string.IsNullOrEmpty(phonenum))
            {
                return false;
            }
            return System.Text.RegularExpressions.Regex.IsMatch(phonenum, @"^1[3-9]{1}\d{9}$");
        }
    }
}
