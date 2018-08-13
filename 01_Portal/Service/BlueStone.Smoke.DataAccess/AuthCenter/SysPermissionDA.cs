using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BlueStone.Smoke.Entity;
using System;

namespace BlueStone.Smoke.DataAccess
{
    public class SysPermissionDA
    {
        /// <summary>
        /// 创建SysPermission信息
        /// </summary>
        public static int InsertSysPermission(SysPermission entity)
        {
            DataCommand cmd = new DataCommand("InsertSysPermission");
            cmd.SetParameter<SysPermission>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新SysPermission信息
        /// </summary>
        public static void UpdateSysPermission(SysPermission entity)
        {
            DataCommand cmd = new DataCommand("UpdateSysPermission");
            cmd.SetParameter<SysPermission>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除SysPermission信息
        /// </summary>
        public static void DeleteSysPermission(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteSysPermission");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询SysPermission信息
        /// </summary>
        public static QueryResult<SysPermission> QuerySysPermissionList(QF_SysPermission filter)
        {
            DataCommand cmd = new DataCommand("QuerySysPermissionList");
            cmd.QuerySetCondition("CommonStatus", ConditionOperation.Equal, DbType.Int32, filter.CommonStatus);
            cmd.QuerySetCondition("FunctionSysNo", ConditionOperation.Equal, DbType.Int32, filter.FunctionSysNo);
            cmd.QuerySetCondition("CommonStatus", ConditionOperation.NotEqual, DbType.Int32, CommonStatus.Deleted);//TODO： 排除 权限 delete的?
            QueryResult<SysPermission> result = cmd.Query<SysPermission>(filter, " SysNo DESC");
            return result;
        }

        /// <summary>
        /// 获取单个SysPermission信息
        /// </summary>
        public static SysPermission LoadSysPermission(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadSysPermission");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            SysPermission result = cmd.ExecuteEntity<SysPermission>();
            return result;
        }
        

        public static void DeleteSysPermissionBatch(IEnumerable<int> sysNos)
        {
            DataCommand cmd = new DataCommand("DeleteSysPermissionBatch");
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        public static void UpdateSysPermissionStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        {
            DataCommand cmd = new DataCommand("UpdateSysPermissionStatusBatch");
            cmd.SetParameter("@Status", DbType.Int32, status);
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        public static List<SysPermission> LoadAllSysPermissionsByMenuSysNo(int menuSysNo)
        {
            DataCommand cmd = new DataCommand("LoadAllSysPermissionsByMenuSysNo");
            cmd.SetParameter("@MenuSysNo", DbType.Int32, menuSysNo);
            List<SysPermission> result = cmd.ExecuteEntityList<SysPermission>();
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuSysNo"></param>
        /// <param name="list"></param>
        public static void SaveMenusPermission(int menuSysNo, List<SysPermission> list)
        {

            //INSERTSTR
            DataCommand cmd = new DataCommand("SaveMenusPermission");
            cmd.CommandText = cmd.CommandText.Replace("#HasPermissionSysNo#", string.Join(",", (from s in list select s.SysNo)));
            cmd.SetParameter("@MenuSysNo", DbType.Int32, menuSysNo);
            string str = String.Empty;
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    str += "(" + item.SysNo + "," + menuSysNo +"),";
                }
            }
            if (!String.IsNullOrEmpty(str))
            {
                str = str.TrimEnd(',');
                cmd.CommandText = cmd.CommandText.Replace("#INSERTSTR#", str);
            }
            cmd.ExecuteNonQuery();
            //cmd.SetParameter("@PermissionSysNo", DbType.Int32, permissionSysNo);
        }

        public static List<SysPermission> LoadAllSysPermissionsByRoleSysNo(int roleSysNo)
        {
            DataCommand cmd = new DataCommand("LoadAllSysPermissionsByRoleSysNo");
            cmd.SetParameter("@RoleSysNo", DbType.Int32, roleSysNo);
            List<SysPermission> result = cmd.ExecuteEntityList<SysPermission>();
            return result;
        }
    }
}
