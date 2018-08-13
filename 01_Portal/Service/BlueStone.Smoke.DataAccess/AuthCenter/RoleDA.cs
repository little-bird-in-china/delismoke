using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.DataAccess
{
    public class RoleDA
    {
        /// <summary>
        /// 创建Role信息
        /// </summary>
        public static int InsertRole(Role entity)
        {
            DataCommand cmd = new DataCommand("InsertRole");
            cmd.SetParameter<Role>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新Role信息
        /// </summary>
        public static void UpdateRole(Role entity)
        {
            DataCommand cmd = new DataCommand("UpdateRole");
            cmd.SetParameter<Role>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除Role信息
        /// </summary>
        public static void DeleteRole(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteRole");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询Role信息
        /// </summary>
        public static QueryResult<Role> QueryRoleList(QF_Role filter)
        {
            DataCommand cmd = new DataCommand("QueryRoleList");
            cmd.QuerySetCondition("r.SysNo", ConditionOperation.Equal, DbType.Int32, filter.SysNo);
            cmd.QuerySetCondition("r.RoleName", ConditionOperation.Like, DbType.String, cmd.SetSafeParameter(filter.RoleName));
            cmd.QuerySetCondition("r.CommonStatus", ConditionOperation.Equal, DbType.Int32, filter.CommonStatus);
            cmd.QuerySetCondition("r.CommonStatus", ConditionOperation.NotEqual, DbType.Int32, CommonStatus.Deleted);//TOOD： 查询需不需要排除角色删除(已禁用)的？
            cmd.QuerySetCondition("r.ApplicationID", ConditionOperation.Equal, DbType.String, cmd.SetSafeParameter(filter.ApplicationID));
            QueryResult<Role> result = cmd.Query<Role>(filter, " r.SysNo DESC");
            return result;
        }

        /// <summary>
        /// 获取单个Role信息
        /// </summary>
        public static Role LoadRole(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadRole");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            Role result = cmd.ExecuteEntity<Role>();
            return result;
        }

        /// <summary>
        /// 创建UserRole信息
        /// </summary>
        public static int InsertUserRole(User_Role entity)
        {
            DataCommand cmd = new DataCommand("InsertUserRole");
            cmd.SetParameter<User_Role>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 删除UserRole信息
        /// </summary>
        public static void DeleteUserRole(int UserSysNo)
        {
            DataCommand cmd = new DataCommand("DeleteUsersRole");
            cmd.SetParameter("@UserSysNo", DbType.Int32, UserSysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 通过状态加载Role信息
        /// </summary>
        public static List<Role> LoadRoleByCommonStatus(CommonStatus CommonStatus,string ApplicationID)
        {
            DataCommand cmd = new DataCommand("LoadRoleByCommonStatus");
            cmd.SetParameter("@CommonStatus", DbType.Int32, CommonStatus);
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            List<Role> list = cmd.ExecuteEntityList<Role>();
            return list;
        }
        /// <summary>
        /// 加载UserRole信息
        /// </summary>
        public static List<User_Role> LoadUserRole(int UserSysNo)
        {
            DataCommand cmd = new DataCommand("LoadUserRole");
            cmd.SetParameter("@UserSysNo", DbType.Int32, UserSysNo);
            List<User_Role> list = cmd.ExecuteEntityList<User_Role>();
            return list;
        }

        public static void DeleteRoleBatch(IEnumerable<int> sysNos)
        {
            DataCommand cmd = new DataCommand("DeleteRoleBatch");
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateRoleStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        {
            DataCommand cmd = new DataCommand("UpdateRoleStatusBatch");
            cmd.SetParameter("@Status", DbType.Int32, status);
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        public static List<Role> GetAllRolesByApplicationID(string applicationID)
        {
            DataCommand cmd = new DataCommand("GetAllRolesByApplicationID");
            cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            return cmd.ExecuteEntityList<Role>();
        }

        public static List<Role> GetAllRolesByUserSysNo(int userSysNo)
        {
            DataCommand cmd = new DataCommand("GetAllRolesByUserSysNo");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            return cmd.ExecuteEntityList<Role>();
        }

        public static void SaveUsersRole(int userSysNo, IEnumerable<int> rolesysnos)
        {
            DataCommand cmd = new DataCommand("SaveUsersRole");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            cmd.CommandText = cmd.CommandText.Replace("#HasRoleSysNo#", string.Join(",", rolesysnos));

            StringBuilder InsertSql = new StringBuilder();
            foreach (int r in rolesysnos)
            {
                InsertSql.Append(string.Format(" INSERT INTO My_User_Role_temp Values({0},{1}); {2}", userSysNo, r, Environment.NewLine));
            }

            cmd.CommandText = cmd.CommandText.Replace("#InsertSql#", InsertSql.ToString());

            cmd.ExecuteNonQuery();
        }

        public static void InsertUsersRole(int userSysNo, IEnumerable<int> rolesysnos)
        {
            DataCommand cmd = new DataCommand("InsertUsersRole");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);

            StringBuilder InsertSql = new StringBuilder();
            foreach (int r in rolesysnos)
            {
                InsertSql.AppendLine(string.Format(@"INSERT INTO authcenter.user_role (UserSysNo,RoleSysNo) 
                select {0}, {1} from DUAL
                where not exists(
                select 1 from authcenter.user_role where UserSysNo = {0} And RoleSysNo = {1}
                ); ", userSysNo, r));
            }

            cmd.CommandText = cmd.CommandText.Replace("#InsertSql#", InsertSql.ToString());

            cmd.ExecuteNonQuery();
        }

        public static void SaveRolesPermission(int roleSysNo, List<SysPermission> permissions)
        {
            DataCommand cmd = new DataCommand("SaveRolesPermission");
            cmd.SetParameter("@RoleSysNo", DbType.Int32, roleSysNo);
            cmd.CommandText = cmd.CommandText.Replace("#HasPermissionSysNo#", string.Join(",", (from s in permissions select s.SysNo)));

            StringBuilder InsertSql = new StringBuilder();
            for (int i = 0; i < permissions.Count; i++)
            {
                InsertSql.Append(string.Format(" INSERT INTO My_Role_Permission_temp Values({0},{1}); {2}", roleSysNo, permissions[i].SysNo, Environment.NewLine));
            }

            cmd.CommandText = cmd.CommandText.Replace("#InsertSql#", InsertSql.ToString());

            cmd.ExecuteNonQuery();
        }

        public static void SaveUsersRoleByApplicationID(int userSysNo, IEnumerable<int> rolesysnos, string applicationID)
        {
            DataCommand cmd = new DataCommand("SaveUsersRoleByApplicationID");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            cmd.CommandText = cmd.CommandText.Replace("#HasRoleSysNo#", string.Join(",", rolesysnos));

            StringBuilder InsertSql = new StringBuilder();
            foreach(int r in rolesysnos)
            {
                InsertSql.Append(string.Format(" INSERT INTO My_User_Role_temp Values({0},{1}); {2}", userSysNo, r, Environment.NewLine));
            }

            cmd.CommandText = cmd.CommandText.Replace("#InsertSql#", InsertSql.ToString());

            cmd.ExecuteNonQuery();
        }
    }
}
