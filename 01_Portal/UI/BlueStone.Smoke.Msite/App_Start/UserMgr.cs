using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Web.Caching;
using BlueStone.Utility.Web;
using BlueStone.Utility;
using System;
using System.Configuration;

namespace BlueStone.Smoke.Msite
{
    public static class UserMgr
    {
        private static string SPECIAL_VERIFY_CODE = ConfigurationManager.AppSettings["StaticVerifyCode"];
        public const string LOGIN_VERIFYCODE_COOKIE = "bluestone_loginVerifyCode";
        /// <summary>
        /// 读取用户登录信息
        /// </summary>
        /// <returns></returns>
        public static AppUserInfo ReadUserInfo()
        {
            return CookieHelper.GetCookie<AppUserInfo>("SmokeLoginCookie");
        }


        public static bool ValidationCode(string verifycode)
        {
            if (string.IsNullOrWhiteSpace(verifycode))
                throw new BusinessException(LangHelper.GetText("请输入验证码！"));
            if (verifycode != SPECIAL_VERIFY_CODE)
            {
                string serverVerifyCode = CookieHelper.GetCookie<string>(LOGIN_VERIFYCODE_COOKIE);
                if (string.Compare(serverVerifyCode, verifycode, true) != 0)
                    throw new BusinessException(LangHelper.GetText("验证码输入错误！"), 1);
            }
            return true;
        }
        /// <summary>
        /// 写用户登录信息
        /// </summary>
        /// <param name="userSysNo">用户编号</param>
        /// <param name="userID">用户名</param>
        /// <param name="userDisplayName">用户显示名</param>
        public static void WriteUserInfo(AppUserInfo user)
        {
            CookieHelper.SaveCookie<AppUserInfo>("SmokeLoginCookie", user);
        }


        /// <summary>
        /// 退出登录
        /// </summary>
        public static void Logout()
        {
            CookieHelper.RemoveCookie("SmokeLoginCookie");

        }

        public static bool HasLogin()
        {
            var userAuth = UserMgr.ReadUserInfo();
            if (userAuth == null)
            {
                return false;
            }
            int userSysNo = userAuth.UserSysNo;
            string userID = userAuth.UserID;
            if (userSysNo <= 0 || string.IsNullOrWhiteSpace(userID))
            {
                return false;
            }
            return true;
        }

    }

    public class WebAuth : IWebAuth
    {
        public bool ValidateAuth()
        {
            return UserMgr.HasLogin();
        }
    }
}
