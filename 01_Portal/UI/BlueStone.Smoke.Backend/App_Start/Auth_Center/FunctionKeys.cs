using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlueStone.Smoke.Backend.App_Start
{
    public class FunctionKeys
    {
        #region 授权中心

        #region 菜单
        /// <summary>
        /// 查看菜单列表
        /// </summary>
        public const string Menu_List = "Auth_All";// "Menu_List";
        /// <summary>
        /// 菜单编辑
        /// </summary>
        public const string Menu_SaveMenu = "Auth_All";// "Menu_SaveMenu";
        /// <summary>
        /// 菜单删除
        /// </summary>
        public const string Menu_DeleteMenu = "Auth_All";//"Menu_DeleteMenu";
        /// <summary>
        /// 菜单权限点查看
        /// </summary>
        public const string Menu_MenuPermissionEdit = "Auth_All";// "Menu_MenuPermissionEdit";
        /// <summary>
        /// 菜单权限点保存
        /// </summary>
        public const string Menu_SaveMenusPermission = "Auth_All";// "Menu_SaveMenusPermission";
        #endregion

        #region 用户
        /// <summary>
        /// 查看用户列表
        /// </summary>
        public const string User_List = "Auth_All";// "User_List";
        /// <summary>
        /// 用户编辑
        /// </summary>
        public const string User_SaveSystemUser = "Auth_All";// "User_SaveSystemUser";
        /// <summary>
        /// 重置密码
        /// </summary>
        public const string User_ResetUserPassword = "Auth_All";// "User_ResetUserPassword";
        /// <summary>
        /// 用户删除
        /// </summary>
        public const string User_DeleteSystemUser = "Auth_All";// "User_DeleteSystemUser";
        /// <summary>
        /// 查看角色
        /// </summary>
        public const string User_UserRoleEdit = "Auth_All";// "User_UserRoleEdit";
        /// <summary>
        /// 用户角色保存
        /// </summary>
        public const string User_SaveUsersRole = "Auth_All";// "User_SaveUsersRole";
        #endregion

        #region 角色
        /// <summary>
        /// 查看角色列表
        /// </summary>
        public const string Role_List = "Auth_All";// "Role_List";
        /// <summary>
        /// 角色编辑
        /// </summary>
        public const string Role_SaveRole = "Auth_All";// "Role_SaveRole";
        /// <summary>
        /// 角色删除
        /// </summary>
        public const string Role_DeleteRoles = "Auth_All";// "Role_DeleteRoles";
        /// <summary>
        /// 查看角色权限
        /// </summary>
        public const string Role_RolePermissionEdit = "Auth_All";// "Role_RolePermissionEdit";
        /// <summary>
        /// 角色权限保存
        /// </summary>
        public const string Role_SaveRolesPermission = "Auth_All";// "Role_SaveRolesPermission";
        #endregion

        #region 功能
        /// <summary>
        /// 查看功能列表
        /// </summary>
        public const string Permission_FunctionList = "Auth_All";// "Permission_FunctionList";
        /// <summary>
        /// 功能编辑
        /// </summary>
        public const string Permission_SaveFunction = "Auth_All";// "Permission_SaveFunction";
        /// <summary>
        /// 功能删除
        /// </summary>
        public const string Permission_DeleteFunction = "Auth_All";// "Permission_DeleteFunction";
        #endregion

        #region 权限
        /// <summary>
        /// 查看权限列表
        /// </summary>
        public const string Permission_List = "Auth_All";// "Permission_List";
        /// <summary>
        /// 编辑权限
        /// </summary>
        public const string Permission_SaveSysPermission = "Auth_All";// "Permission_SaveSysPermission";
        /// <summary>
        /// 权限删除
        /// </summary>
        public const string Permission_DeleteSysPermission = "Auth_All";// "Permission_DeleteSysPermission";
        #endregion

        #endregion

        #region 平台管理端
        /// <summary>
        /// 平台客户管理
        /// </summary>
        public const string PM_Customer_All = "PM_Company_All";
        /// <summary>
        /// 平台烟感器管理
        /// </summary>
        public const string PM_Smoke_All = "PM_Smoke_All";
        /// <summary>
        /// 平台消息模板管理
        /// </summary>
        public const string PM_MsgTemplate_All = "PM_MsgTemplate_All";
        /// <summary>
        /// 平台新闻管理
        /// </summary>
        public const string PM_Topic_All = "PM_Topic_All";

        #endregion

        #region 客户后台管理端
        /// <summary>
        /// 客户自己的信息管理
        /// </summary>
        public const string Cus_MyInfo_All = "Cus_MyInfo_All";
        /// <summary>
        /// 客户烟感器管理
        /// </summary>
        public const string Cus_Smoke_All = "Cus_Smoke_All";

        #endregion
    }
}