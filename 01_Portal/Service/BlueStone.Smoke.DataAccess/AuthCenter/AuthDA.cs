using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using BlueStone.Utility.Web.Auth.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.DataAccess
{
    public class AuthDA
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pwd"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static AuthUserModel Login(string userID, string pwd, string applicationKey)
        {
            DataCommand cmd = new DataCommand("SystemUser_Login");
            cmd.SetParameter("@UserID", DbType.String, userID);
            cmd.SetParameter("@Pwd", DbType.String, pwd);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationKey);
            AuthUserModel result = cmd.ExecuteEntity<AuthUserModel>();
            return result;
        }
        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="pwd"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        public static SystemUser LoadSystemUserByIDAndPassword(string userID, string pwd, string applicationKey)
        {
            DataCommand cmd = new DataCommand("LoadSystemUserByIDAndPassword");
            cmd.SetParameter("@UserID", DbType.String, userID);
            cmd.SetParameter("@Pwd", DbType.String, pwd);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationKey);
            SystemUser result = cmd.ExecuteEntity<SystemUser>();
            return result;
        }

        //public static AuthUserDataModel GetUserMenuList(int userSysNo, string applicationKey, string topMenuKey)
        //{
        //    DataCommand cmd = new DataCommand("GetUserMenuList");
        //    cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
        //    cmd.SetParameter("@ApplicationKey", DbType.AnsiString, applicationKey);
        //    var ds = cmd.ExecuteDataSet();
        //    var allmenus = DataMapper.GetEntityList<AuthMenuModel, List<AuthMenuModel>>(ds.Tables[0].Rows);
        //    var allpermissions = DataMapper.GetEntityList<AuthPermissionModel, List<AuthPermissionModel>>(ds.Tables[1].Rows);
        //    var permissions = DataMapper.GetEntityList<AuthPermissionModel, List<AuthPermissionModel>>(ds.Tables[2].Rows);
        //    var roles = DataMapper.GetEntityList<AuthRoleModel, List<AuthRoleModel>>(ds.Tables[3].Rows);

        //    AuthMenuModel topMenu = null;

        //    if (!string.IsNullOrWhiteSpace(topMenuKey))
        //    {
        //        topMenu = allmenus.FirstOrDefault(x => string.Compare(x.MenuKey, topMenuKey, true) == 0);
        //    }

        //    var menus = new List<AuthMenuModel>();
        //    var menusL = new List<AuthMenuModel>();
        //    //根据Permissions过滤菜单
        //    for (int i = 0; i < allmenus.Count; i++)
        //    {
        //        if (topMenu != null && !allmenus[i].SysCode.StartsWith(topMenu.SysCode))
        //        {
        //            continue;
        //        }
        //        //用户的权限中存在以菜单的SysCode开头的权限MenuSysCode,则认为该菜单可见
        //        if (permissions.Exists(p => p.MenuSysCode != null && p.MenuSysCode.Trim().StartsWith(allmenus[i].SysCode.Trim())))
        //        {
        //            menus.Add(allmenus[i]);
        //        }
        //        //如果菜单没有配置权限,则可见
        //        else if (allpermissions.All(x => x.MenuSysCode == null || x.MenuSysCode.Trim() != allmenus[i].SysCode.Trim()))
        //        {
        //            menus.Add(allmenus[i]);
        //        }
        //    }


        //    foreach (var menu in menus)
        //    {
        //        if (menu.ParentSysNo == "0")
        //        {
        //            menusL.Add(menu);
        //        }
        //        else
        //        {
        //            foreach (var me in menus)
        //            {
        //                if (menu.ParentSysNo == me.SysNo)
        //                {
        //                    menusL.Add(menu);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    AuthUserDataModel result = new AuthUserDataModel();
        //    result.Menus = menusL;
        //    result.Permissions = permissions;
        //    result.Roles = roles;
        //    return result;
        //}

        public static AuthUserDataModel GetUserMenuList(int userSysNo, string applicationKey, string topMenuKey)
        {
            DataCommand cmd = new DataCommand("GetUserMenuList");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            cmd.SetParameter("@ApplicationKey", DbType.AnsiString, applicationKey);
            var ds = cmd.ExecuteDataSet();
            var allmenus = DataMapper.GetEntityList<AuthMenuModel, List<AuthMenuModel>>(ds.Tables[0].Rows);
            var allpermissions = DataMapper.GetEntityList<AuthPermissionModel, List<AuthPermissionModel>>(ds.Tables[1].Rows);
            var permissions = DataMapper.GetEntityList<AuthPermissionModel, List<AuthPermissionModel>>(ds.Tables[2].Rows);
            var roles = DataMapper.GetEntityList<AuthRoleModel, List<AuthRoleModel>>(ds.Tables[3].Rows);

            AuthMenuModel topMenu = null;

            //if (!string.IsNullOrWhiteSpace(topMenuKey))
            //{
            //    topMenu = allmenus.FirstOrDefault(x => string.Compare(x.MenuKey, topMenuKey, true) == 0);
            //}

            var menus = new List<AuthMenuModel>();
            var menusL = new List<AuthMenuModel>();
            //根据Permissions过滤菜单
            for (int i = 0; i < allmenus.Count; i++)
            {
                if (topMenu != null && !allmenus[i].SysCode.StartsWith(topMenu.SysCode))
                {
                    continue;
                }
                //用户的权限中存在以菜单的SysCode开头的权限MenuSysCode,则认为该菜单可见
                if (permissions.Exists(p => p.MenuSysCode != null && p.MenuSysCode.Trim().StartsWith(allmenus[i].SysCode.Trim())))
                {
                    menus.Add(allmenus[i]);
                }
                //如果菜单没有配置权限,则可见
                else if (allpermissions.All(x => x.MenuSysCode == null || x.MenuSysCode.Trim() != allmenus[i].SysCode.Trim()))
                {
                    menus.Add(allmenus[i]);
                }
            }


            foreach (var menu in menus)
            {
                if (menu.ParentSysNo == "0")
                {
                    menusL.Add(menu);
                }
                else
                {
                    foreach (var me in menus)
                    {
                        if (menu.ParentSysNo == me.SysNo)
                        {
                            menusL.Add(menu);
                            break;
                        }
                    }
                }
            }
            AuthUserDataModel result = new AuthUserDataModel();
            result.Menus = menusL;
            result.Permissions = permissions;
            result.Roles = roles;
            return result;
        }



        public static List<SysPermission> LoadAllPermissions(string applicationKey)
        {
            DataCommand cmd = new DataCommand("LoadAllPermissions");
            cmd.SetParameter("@ApplicationKey", DbType.String, applicationKey);
            var result = cmd.ExecuteEntityList<SysPermission>();
            return result;
        }

        public static List<SystemApplication> LoadAllSystemApplication()
        {
            DataCommand cmd = new DataCommand("LoadAllSystemApplication");
            return cmd.ExecuteEntityList<SystemApplication>();
        }

        public static List<AuthFunctionModel> LoadFunctionsByUserSysNo(int userSysNo, string applicationKey, string topName)
        {
            DataCommand cmd = new DataCommand("LoadFunctionsByUserSysNo");

            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            cmd.SetParameter("@ApplicationKey", DbType.AnsiString, applicationKey);

            var ds = cmd.ExecuteDataSet();

            var permissions = DataMapper.GetEntityList<AuthPermissionModel, List<AuthPermissionModel>>(ds.Tables[0].Rows);
            var functions = DataMapper.GetEntityList<AuthFunctionModel, List<AuthFunctionModel>>(ds.Tables[1].Rows);

            if (!string.IsNullOrWhiteSpace(topName))
            {
                var topEntity = functions.FirstOrDefault(x => x.FunctionName == topName);
                if (topEntity != null)
                {
                    functions.RemoveAll(x => !x.SysCode.StartsWith(topEntity.SysCode));
                }
            }

            for (int i = 0; i < functions.Count; i++)
            {
                functions[i].Permissions = permissions.FindAll(x => x.FunctionSysNo == functions[i].SysNo);
            }

            return functions;
        }

        /// <summary>
        /// 获取用户的Application
        /// </summary>
        /// <param name="sysNos"></param>
        /// <returns></returns>
        public static List<SystemApplication> GetSystemApplicationsByUserSysNo(IEnumerable<int> sysNos)
        {
            DataCommand cmd = new DataCommand("GetSystemApplicationsByUserSysNo");
            cmd.CommandText = cmd.CommandText.Replace("#UserSysNos#", string.Join(",", sysNos));
            return cmd.ExecuteEntityList<SystemApplication>();
        }

        public static List<SystemApplication> GetSystemApplicationsByUserRole(int userSysNo)
        {
            DataCommand cmd = new DataCommand("GetSystemApplicationsByUserRole");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            return cmd.ExecuteEntityList<SystemApplication>();
        }
        /// <summary>
        /// 获取全部Application
        /// </summary>
        /// <returns></returns>
        public static List<SystemApplication> GetAllApplication()
        {
            DataCommand cmd = new DataCommand("GetAllApplication");
            return cmd.ExecuteEntityList<SystemApplication>();
        }

    }
}
