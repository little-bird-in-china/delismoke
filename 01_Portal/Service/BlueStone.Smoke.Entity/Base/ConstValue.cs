using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace BlueStone.Smoke.Entity
{
    public class ConstValue
    {
        public const string LOGIN_COOKIE = "BlueStone_ERP_Login";
        public const string LOGIN_VERIFYCODE_COOKIE = "BlueStone_ERP_LoginVerifyCode";

        public static string ImageUploadServerDomain = WebConfigurationManager.AppSettings["ImageUploadServerDomain"];

        public static string ImageStorageServerDomain = WebConfigurationManager.AppSettings["ImageStorageServerDomain"];

        public static int IsPlatform = int.Parse(WebConfigurationManager.AppSettings["IsPlatform"]??"0");

        public static string ApplicationID = WebConfigurationManager.AppSettings["ApplicationID"];

        public static string ImageServerDomain = AppSettingManager.GetSetting("Base", "ImageServerDomain");

  
    }
}
