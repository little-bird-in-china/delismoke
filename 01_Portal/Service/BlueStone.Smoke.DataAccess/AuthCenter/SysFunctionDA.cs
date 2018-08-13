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
    public class SysFunctionDA
    {
        /// <summary>
        /// 创建SysFunction信息
        /// </summary>
        public static int InsertSysFunction(SysFunction entity)
        {
            DataCommand cmd = new DataCommand("InsertSysFunction");
            cmd.SetParameter<SysFunction>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;

        }

        /// <summary>
        /// 更新SysFunction信息
        /// </summary>
        public static void UpdateSysFunction(SysFunction entity)
        {
            DataCommand cmd = new DataCommand("UpdateSysFunction");
            cmd.SetParameter<SysFunction>(entity);
            cmd.ExecuteNonQuery();
        }

        public static void GetBuildSysCode(int parentSysNo, out string parentCode, out string brotherCode,string ApplicationID)
        {
            DataCommand cmd = new DataCommand("GetBuildFunctionSysCode");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentSysNo);
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            DataSet ds = cmd.ExecuteDataSet();
            parentCode = ds.Tables[0].Rows.Count == 0 || string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0].Field<string>("ParentCode")) ? string.Empty : ds.Tables[0].Rows[0].Field<string>("ParentCode").Trim();
            brotherCode = ds.Tables[1].Rows.Count == 0 || string.IsNullOrWhiteSpace(ds.Tables[1].Rows[0].Field<string>("BrotherCode")) ? string.Empty : ds.Tables[1].Rows[0].Field<string>("BrotherCode").Trim();
        }

        /// <summary>
        /// 删除SysFunction信息
        /// </summary>
        public static void DeleteSysFunction(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteSysFunction");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询SysFunction信息
        /// </summary>
        public static QueryResult<SysFunction> QuerySysFunctionList(QF_SysFunction filter)
        {
            DataCommand cmd = new DataCommand("QuerySysFunctionList");
            cmd.QuerySetCondition("FunctionName", ConditionOperation.Equal, DbType.String, filter.FunctionName);
            cmd.QuerySetCondition("CommonStatus", ConditionOperation.Equal, DbType.Int32, filter.CommonStatus);
            QueryResult<SysFunction> result = cmd.Query<SysFunction>(filter, " SysNo DESC");
            return result;
        }

        /// <summary>
        /// 获取单个SysFunction信息
        /// </summary>
        public static SysFunction LoadSysFunction(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadSysFunction");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            SysFunction result = cmd.ExecuteEntity<SysFunction>();
            return result;
        }

        public static int CountFunctionsChildren(int parentsysno)
        {
            DataCommand cmd = new DataCommand("CountFunctionsChildren");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentsysno);
            return cmd.ExecuteScalar<int>();
        }

        public static List<SysFunction> DynamicLoadFunctions(int parentsysno,string ApplicationID)
        {
            DataCommand cmd = new DataCommand("DynamicLoadFunctions");
            cmd.SetParameter("@ParentSysNo", DbType.Int32, parentsysno);
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            List<SysFunction> result = cmd.ExecuteEntityList<SysFunction>();
            return result;
        }

        public static List<SysFunction> LoadAllFunctionsWithPermission(string ApplicationID)
        {
            DataCommand cmd = new DataCommand("LoadAllFunctionsWithPermission");
            cmd.SetParameter("@ApplicationID", DbType.String, ApplicationID);
            var dataSet = cmd.ExecuteDataSet();

            List<SysFunction> functions = DataMapper.GetEntityList<SysFunction, List<SysFunction>>(dataSet.Tables[0].Rows);
            List<SysPermission> permissions = DataMapper.GetEntityList<SysPermission, List<SysPermission>>(dataSet.Tables[1].Rows);

            functions.ForEach(x=> {
                x.Permissions = permissions.FindAll(y=>y.FunctionSysNo==x.SysNo);
            });

            return functions;
        }

        public static List<SysFunction> LoadAllFunctions(string applicationID)
        {
            DataCommand cmd = new DataCommand("LoadAllFunctions");
            cmd.SetParameter("@ApplicationID", DbType.String, applicationID);
            return cmd.ExecuteEntityList<SysFunction>();
        }
    }
}
