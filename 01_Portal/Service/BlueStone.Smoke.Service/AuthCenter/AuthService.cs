using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.JsonRpc;
using BlueStone.Utility;
using BlueStone.Utility.Caching;
using BlueStone.Utility.Web.Auth.Models;
using System;
using System.Collections.Generic;

namespace BlueStone.Smoke.Service
{
    /// <summary>
    /// 该类供外部RPC调用
    /// </summary>
    public class AuthService
    {
        private AuthServiceLocal localservice = new AuthServiceLocal();

        /// <summary>
        /// 清除用户数据缓存
        /// </summary>
        public void ClearAuthData(string userID,string applicationKey)
        {
            localservice.ClearAuthData(userID, applicationKey);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pwd"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public AuthUserModel Login(string userID, string pwd, string applicationKey)
        {
            return localservice.Login(userID, pwd, applicationKey);
        }

        /// <summary>
        /// 获取用户菜单，角色，权限信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public AuthUserDataModel GetUserMenuList(CurrentUser user, string applicationKey,  string topMenuKey = null)
        {
            return localservice.GetUserMenuList(user.UserSysNo, applicationKey, user.UserName, user.LoginTime.ToString(), topMenuKey);
        }

        /// <summary>
        /// 获取指定应用所有权限点
        /// </summary>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public List<AuthPermissionModel> LoadAllPermissions(string applicationKey)
        {
            return localservice.LoadAllPermissions(applicationKey); 
        }

        public List<AuthFunctionModel> LoadFunctionsByUserSysNo(int userSysNo, string applicationKey, string TopName = null)
        {
            return AuthDA.LoadFunctionsByUserSysNo(userSysNo, applicationKey, TopName);
        }

    }
}
