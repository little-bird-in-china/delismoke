using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using System.Collections.Generic;
using System.Linq;
using System;
using BlueStone.JsonRpc;

namespace BlueStone.Smoke.Service
{
    public class RoleService
    {
        /// <summary>
        /// 创建Role信息
        /// </summary>
        public int InsertRole(Role entity)
        {
            CheckRole(entity, true);
            return RoleDA.InsertRole(entity);
        }

        /// <summary>
        /// 更新Role信息
        /// </summary>
        public void UpdateRole(Role entity)
        {
            if (entity.SysNo == 1)
            {
                throw new BusinessException("超级管理员不能修改");
            }
            CheckRole(entity, false);
            RoleDA.UpdateRole(entity);
        }

        /// <summary>
        /// 删除Role信息
        /// </summary>
        public void DeleteRole(int sysNo)
        {
            if (sysNo == 1)
            {
                throw new BusinessException("超级管理员不能删除");
            }
            RoleDA.DeleteRole(sysNo);
        }

        /// <summary>
        /// 分页查询Role信息
        /// </summary>
        public QueryResult<Role> QueryRoleList(QF_Role filter)
        {
            return RoleDA.QueryRoleList(filter);
        }

        /// <summary>
        /// 加载Role信息
        /// </summary>
        public Role LoadRole(int sysNo)
        {
            return RoleDA.LoadRole(sysNo);
        }
        /// <summary>
        /// 通过状态加载Role信息
        /// </summary>
        public static List<Role> LoadRoleByCommonStatus(CommonStatus CommonStatus, string ApplicationID)
        {
            return RoleDA.LoadRoleByCommonStatus(CommonStatus, ApplicationID);
        }
        /// <summary>
        /// 加载UserRole信息
        /// </summary>
        public static List<User_Role> LoadUserRole(int UserSysNo)
        {
            return RoleDA.LoadUserRole(UserSysNo);
        }

        /// <summary>
        /// 创建UserRole信息
        /// </summary>
        public static int InsertUserRole(User_Role entity)
        {
            int sysNo = RoleDA.InsertUserRole(entity);
            return sysNo;
        }
        /// <summary>
        /// 删除UserRole信息
        /// </summary>
        public static void DeleteUserRole(int UserSysNo)
        {
            RoleDA.DeleteUserRole(UserSysNo);
        }

        /// <summary>
        /// 检查Role信息
        /// </summary>
        private void CheckRole(Role entity, bool isCreate)
        {
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if (string.IsNullOrWhiteSpace(entity.RoleName))
            {
                throw new BusinessException(LangHelper.GetText("角色名称不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.RoleName))
            {
                if (entity.RoleName.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("角色名称长度不能超过50！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.Memo))
            {
                if (entity.Memo.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("备注长度不能超过200！"));
                }
            }
            if (isCreate && string.IsNullOrWhiteSpace(entity.ApplicationID))
            {
                throw new BusinessException(LangHelper.GetText("不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.ApplicationID))
            {
                if (entity.ApplicationID.Length > 40)
                {
                    throw new BusinessException(LangHelper.GetText("长度不能超过40！"));
                }
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sysNos"></param>
        public void DeleteRoleBatch(IEnumerable<int> sysNos)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的编号");
            }
            if (sysNos.Any(x => x == 1))
            {
                throw new BusinessException("传入的要批量删除的角色中含有超级管理员,不能执行删除");
            }
            RoleDA.DeleteRoleBatch(sysNos);
        }

        public void UpdateRoleStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的编号");
            }
            if (sysNos.Any(x => x == 1))
            {
                throw new BusinessException("传入的要批量操作的角色中含有超级管理员,不能执行");
            }
            RoleDA.UpdateRoleStatusBatch(sysNos, status);
        }

        [JsonRpcMethod("AuthService.GetAllRolesByApplicationID")]
        public List<Role> GetAllRolesByApplicationID(string applicationID)
        {
            if (string.IsNullOrWhiteSpace(applicationID))
            {
                return new List<Role>();
            }
            return RoleDA.GetAllRolesByApplicationID(applicationID);
        }

        public List<Role> GetAllRolesByUserSysNo(int userSysNo)
        {
            return RoleDA.GetAllRolesByUserSysNo(userSysNo);
        }

        public void SaveRolesPermission(int roleSysNo, List<SysPermission> list)
        {
            RoleDA.SaveRolesPermission(roleSysNo, list);
        }


        public void SaveUsersRole(int userSysNo, List<Role> roles, string ApplicationID)
        {
            var rolesysnos = from r in roles select r.SysNo;
            if (!string.IsNullOrWhiteSpace(ApplicationID))
            {
                RoleDA.SaveUsersRoleByApplicationID(userSysNo, rolesysnos, ApplicationID);
            }
            else
            {
                RoleDA.SaveUsersRole(userSysNo, rolesysnos);
            }
        }

        [JsonRpcMethod("AuthService.SaveUsersRoleForRPC")]
        public void SaveUsersRoleForRPC(int userSysNo, IEnumerable<int> rolesysnos)
        {
            if(userSysNo<=0||rolesysnos == null|| rolesysnos.Count() == 0)
            {
                return;
            }
            RoleDA.InsertUsersRole(userSysNo, rolesysnos);
        }

        public List<SysPermission> LoadAllSysPermissionsByRoleSysNo(int roleSysNo)
        {
            if (roleSysNo == 0)
            {
                throw new BusinessException("请传入角色编号!");
            }
            return SysPermissionDA.LoadAllSysPermissionsByRoleSysNo(roleSysNo);
        }
    }
}
