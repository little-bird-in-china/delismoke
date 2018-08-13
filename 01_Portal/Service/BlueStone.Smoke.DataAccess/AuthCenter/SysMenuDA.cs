using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;
using System;

namespace BlueStone.Smoke.DataAccess
{
    public class SysMenuDA
    {
        /// <summary>
        /// 创建SysMenu信息
        /// </summary>
        public static int InsertSysMenu(SysMenu entity)
        {
            DataCommand cmd = new DataCommand("InsertSysMenu");
            cmd.SetParameter<SysMenu>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新SysMenu信息
        /// </summary>
        public static void UpdateSysMenu(SysMenu entity)
        {
            DataCommand cmd = new DataCommand("UpdateSysMenu");
            cmd.SetParameter<SysMenu>(entity);
            cmd.ExecuteNonQuery();
        }

        public static void GetBuildSysCode(int parentSysNo, out string ParentCode, out string BrotherCode,string ApplicationID)
        {
            DataCommand cmd = new DataCommand("GetBuildMenuSysCode");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentSysNo);
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            DataSet ds = cmd.ExecuteDataSet();
            ParentCode = ds.Tables[0].Rows.Count == 0 || string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0].Field<string>("ParentCode")) ? string.Empty : ds.Tables[0].Rows[0].Field<string>("ParentCode").Trim();
            BrotherCode = ds.Tables[1].Rows.Count == 0 || string.IsNullOrWhiteSpace(ds.Tables[1].Rows[0].Field<string>("BrotherCode")) ? string.Empty : ds.Tables[1].Rows[0].Field<string>("BrotherCode").Trim();
        }

        public static List<SysMenu> DynamicLoadMenus(int parentsysno,string ApplicationID)
        {
            DataCommand cmd = new DataCommand("DynamicLoadMenus");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentsysno);
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            List<SysMenu> result = cmd.ExecuteEntityList<SysMenu>();
            return result;
        }

        public static List<SysMenu> DisposableLoadMenus(int parentsysno)
        {
            DataCommand cmd = new DataCommand("DisposableLoadMenus");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentsysno);
            cmd.SetParameter("@ApplicationID", DbType.String, "AuthCenter");
            List<SysMenu> result = cmd.ExecuteEntityList<SysMenu>();
            return result;
        }

        public static int CountMenusChildrens(int parentsysno)
        {
            DataCommand cmd = new DataCommand("CountMenusChildrens");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentsysno);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 删除SysMenu信息
        /// </summary>
        public static void DeleteSysMenu(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteSysMenu");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询SysMenu信息
        /// </summary>
        public static QueryResult<SysMenu> QuerySysMenuList(QF_SysMenu filter)
        {
            DataCommand cmd = new DataCommand("QuerySysMenuList");

            cmd.QuerySetCondition("MenuName", ConditionOperation.Like, DbType.String, filter.MenuName);
            cmd.QuerySetCondition("Type", ConditionOperation.Equal, DbType.Int32, filter.Type);
            cmd.QuerySetCondition("CommonStatus", ConditionOperation.Equal, DbType.Int32, filter.CommonStatus);
            cmd.QuerySetCondition("ParentSysNo", ConditionOperation.Equal, DbType.Int32, filter.ParentSysNo);
            cmd.QuerySetCondition("CommonStatus", ConditionOperation.NotEqual, DbType.Int32, CommonStatus.Deleted);//TODO: 排除Deleted的菜单？
            QueryResult<SysMenu> result = cmd.Query<SysMenu>(filter, " SysNo DESC");

            return result;
        }

        /// <summary>
        /// 获取单个SysMenu信息
        /// </summary>
        public static SysMenu LoadSysMenu(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadSysMenu");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            SysMenu result = cmd.ExecuteEntity<SysMenu>();
            return result;
        }

        public static List<SysMenu> LoadAllMenusWithPermission(string ApplicationID)
        {
            DataCommand cmd = new DataCommand("LoadAllMenusWithPermission");
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            var dataSet = cmd.ExecuteDataSet();

            List<SysMenu> Menus = DataMapper.GetEntityList<SysMenu, List<SysMenu>>(dataSet.Tables[0].Rows);
            List<SysPermission> permissions = DataMapper.GetEntityList<SysPermission, List<SysPermission>>(dataSet.Tables[1].Rows);

            Menus.ForEach(x => {
                x.Permissions = permissions.FindAll(y => y.MenuSysNo == x.SysNo);
            });

            return Menus;
        }

        public static void DeleteMenusPermission(int sysno)
        {
            DataCommand cmd = new DataCommand("DeleteMenusPermission");
            cmd.SetParameter("@SysNo", DbType.Int32, sysno);
            cmd.ExecuteNonQuery();
        }
    }
}
