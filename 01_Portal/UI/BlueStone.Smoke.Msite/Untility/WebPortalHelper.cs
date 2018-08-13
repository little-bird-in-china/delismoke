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

        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsPhonenum(string phonenum)
        {
            if (string.IsNullOrEmpty(phonenum))
            {
                return false;
            }
            return System.Text.RegularExpressions.Regex.IsMatch(phonenum, @"^1[3-9]{1}\d{9}$");
        }
        public static Int64 GetTimeStamp(bool utcTS=false) {
            TimeSpan ts;
            if (utcTS)
            {
                ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            }
            else
            {
                ts = DateTime.Now - new DateTime(1970, 1, 1, 8, 0, 0, 0);
            }
            return Convert.ToInt64(ts.TotalSeconds);
        }
        public static long GetGuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }


        public static int GetRandom(int? maxValue=null)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            if (maxValue.HasValue)
            {
                return random.Next(maxValue.Value);
            }
            return random.Next();
        }
        public static int GetRandom(int minValue, int maxValue)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            return random.Next(minValue, maxValue);
        }
    }
  
}
