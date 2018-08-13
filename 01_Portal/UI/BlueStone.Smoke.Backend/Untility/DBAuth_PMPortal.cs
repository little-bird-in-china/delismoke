using BlueStone.Smoke.Service;
using BlueStone.Utility.Caching;
using BlueStone.Utility.Web;
using BlueStone.Utility.Web.Auth.Models;
using System.Collections.Generic;

namespace BlueStone.Smoke.Backend
{
    public class DBAuth_PMPortal : IAuth
    {
        private const string CACHE_KEY_USER_MENU = "User_MenuList";
        private const string CACHE_KEY_ALL_PERMISSION = "AllPermissionKeys";
        private AuthService authService = new AuthService();
        private string applicationKey;
        public string ApplicationKey
        {
            get
            {
                return applicationKey;
            }
            set
            {
                applicationKey = value;
            }
        }

        public void ClearCachedData(AuthUserModel user)
        {
            string keyWithoutLoginTime = CacheManager.GenerateKey(CACHE_KEY_USER_MENU, this.ApplicationKey, user.UserID);
            CacheManager.RemoveStartsWith(keyWithoutLoginTime + "_");
            authService.ClearAuthData(user.UserID, AuthMgr.GetApplicationKey());
            //Rpc.Call<AuthUserModel>("AuthService.AuthService.ClearAuthData");
        }

        public AuthUserDataModel GetUserMenuList(AuthUserModel user)
        {
            if (user == null || user.UserSysNo <= 0)
            {
                AuthUserDataModel result = new AuthUserDataModel();
                result.Menus = new List<AuthMenuModel>();
                result.Permissions = new List<AuthPermissionModel>();
                result.Roles = new List<AuthRoleModel>();
                return result;
            }

            string keyWithoutLoginTime = CacheManager.GenerateKey(CACHE_KEY_USER_MENU, this.ApplicationKey, user.UserID);
            string keyWithLoginTime = CacheManager.GenerateKey(keyWithoutLoginTime, user.UILoginTime);

            return CacheManager.GetWithCache<AuthUserDataModel>(keyWithLoginTime, () =>
            {
                //移除失效的缓存
                CacheManager.RemoveStartsWith(keyWithoutLoginTime + "_");
                BlueStone.Smoke.Entity.CurrentUser cUser = new Entity.CurrentUser()
                {
                    UserSysNo = user.UserSysNo,
                    LoginTime = user.UILoginTime,
                    UserName = user.UserID
                };

                return authService.GetUserMenuList(cUser, AuthMgr.GetApplicationKey());
                //return Rpc.Call<AuthUserDataModel>("AuthService.AuthService.GetUserMenuList", user.UserSysNo, AuthMgr.GetApplicationKey());
            }, 60 * 60 * 8);
        }

        public bool HasAuth(string authKey, AuthUserModel user)
        {

            var result = GetUserMenuList(user);
            var permissionList = result.Permissions;

            return permissionList.Exists(p => p.PermissionKey.ToLower().Trim() == authKey.ToLower().Trim());
        }

        public List<AuthPermissionModel> LoadAllPermissions(AuthUserModel user)
        {
            string keyWithoutLoginTime = CacheManager.GenerateKey(CACHE_KEY_ALL_PERMISSION, this.ApplicationKey);

            return CacheManager.GetWithCache<List<AuthPermissionModel>>(keyWithoutLoginTime, () =>
            {
                return authService.LoadAllPermissions(this.ApplicationKey);
                //return Rpc.Call<List<AuthPermissionModel>>("AuthService.AuthService.LoadAllPermissions", this.ApplicationKey);
            }, 60 * 60 * 8);
        }

        public AuthUserModel Login(string userID, string pwd, string applicationKey)
        {
            return authService.Login(userID, pwd, applicationKey);
            // return Rpc.Call<AuthUserModel>("AuthService.AuthService.Login", userID, pwd, applicationKey);
        }
    }
}