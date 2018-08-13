using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using BlueStone.Utility.Caching;
using BlueStone.Utility.Web.Auth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Service
{
    /// <summary>
    /// 该类供AuthCenter内部调用
    /// </summary>
    public class AuthServiceLocal
    {
        private const string Key_Service_User_Auth_Data = "Key_Service_User_Auth_Data";

        public void ClearAuthData(string userID, string applicationKey)
        {
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrWhiteSpace(applicationKey))
            {
                CacheManager.RemoveStartsWith(CacheManager.GenerateKey(Key_Service_User_Auth_Data, applicationKey, userID) + "_");
            }
        }

        public AuthUserModel Login(string userID, string pwd, string applicationKey)
        {
            var result = AuthDA.Login(userID, pwd, applicationKey);
            if (result == null)
            {
                throw new BusinessException("账号或密码错误！", 10);
            }
            if (result.CommonStatus != 1)
            {
                throw new BusinessException("您的账户已被禁用，请联系您的管理员。");
            }
            //生成登录时间
            result.LoginTime = DateTimeHelper.GetTimeZoneNow();
            return result;
        }

        public AuthUserDataModel GetUserMenuList(int userSysNo, string applicationKey, string userID, string loginTime, string topMenuKey = null)
        {
            string keyWithoutLoginTime = CacheManager.GenerateKey(Key_Service_User_Auth_Data, applicationKey, userID);
            string keyWithLoginTime = CacheManager.GenerateKey(keyWithoutLoginTime, loginTime);

            return CacheManager.GetWithCache(keyWithLoginTime, () => {
                //移除失效的缓存
                CacheManager.RemoveStartsWith(keyWithoutLoginTime + "_");
                AuthUserDataModel result = AuthDA.GetUserMenuList(userSysNo, applicationKey, topMenuKey);
                return result;
            }, 60 * 60 * 8);
        }

        public List<AuthPermissionModel> LoadAllPermissions(string applicationKey)
        {
            var p_list = AuthDA.LoadAllPermissions(applicationKey);
            List<AuthPermissionModel> result = new List<Utility.Web.Auth.Models.AuthPermissionModel>();
            for (int i = 0; i < p_list.Count; i++)
            {
                result.Add(new AuthPermissionModel
                {
                    PermissionKey = p_list[i].PermissionKey,
                    PermissionName = p_list[i].PermissionName,
                    MenuSysNo = p_list[i].MenuSysNo,
                    MenuSysCode = p_list[i].MenuSysCode
                });
            }
            return result;
        }

        public static List<SystemApplication> LoadAllSystemApplication()
        {
            return AuthDA.LoadAllSystemApplication();
        }

        public static List<SystemApplication> GetSystemApplicationsByUserSysNo(IEnumerable<int> sysNos)
        {
            return AuthDA.GetSystemApplicationsByUserSysNo(sysNos);
        }
        /// <summary>
        /// 获取全部Application
        /// </summary>
        /// <returns></returns>
        public List<SystemApplication> GetAllApplication()
        {
            return AuthDA.GetAllApplication();
        }

    }
}
