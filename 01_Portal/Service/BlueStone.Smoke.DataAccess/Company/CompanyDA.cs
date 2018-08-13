using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BlueStone.Smoke.DataAccess
{
    public class CompanyDA
    {
        /// <summary>
        /// 创建Company信息
        /// </summary>
        public static int InsertCompany(Company entity)
        {
            DataCommand cmd = new DataCommand("InsertCompany");
            cmd.SetParameter<Company>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新Company信息
        /// </summary>
        public static void UpdateCompany(Company entity)
        {
            DataCommand cmd = new DataCommand("UpdateCompany");
            cmd.SetParameter<Company>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个Company信息
        /// </summary>
        public static Company LoadCompany(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadCompany");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            Company result = cmd.ExecuteEntity<Company>();
            return result;
        }

        /// <summary>
        /// 分页查询Company信息
        /// </summary>
        public static QueryResult<Company> QueryCompanyList2(QF_Company filter)
        {
            DataCommand cmd = new DataCommand("QueryCompanyList");
            cmd.QuerySetCondition("c.SysNo", ConditionOperation.Equal, DbType.Int32, filter.SysNo);

            if (!string.IsNullOrEmpty(filter.Name))
            {
                filter.Name = cmd.SetSafeParameter(filter.Name);
                cmd.QuerySetCondition($"And (c.Name like '%{ filter.Name}%' Or c.ContactName like '%{ filter.Name}%' Or c.ContactCellPhone like '%{ filter.Name}%')");
            }
            cmd.QuerySetCondition("c.CompanyStatus", ConditionOperation.Equal, DbType.Int32, filter.CompanyStatus);


            QueryResult<Company> result = cmd.Query<Company>(filter, " SysNo DESC");
            return result;
        }


        /// <summary>
        /// 分页查询Company信息
        /// </summary>
        public static QueryResult<Company> QueryCompanyList(QF_Company filter)
        {
            DataCommand cmd = new DataCommand("QueryCompanyList");
            cmd.QuerySetCondition("c.SysNo", ConditionOperation.Equal, DbType.Int32, filter.SysNo);

            cmd.QuerySetCondition("c.Name", ConditionOperation.Like, DbType.String, filter.Name);

            cmd.QuerySetCondition("c.CompanyStatus", ConditionOperation.Equal, DbType.Int32, filter.CompanyStatus);


            QueryResult<Company> result = cmd.Query<Company>(filter, " SysNo DESC");
            return result;
        }

        public static Company LoadCompanyByName(string name)
        {
            DataCommand cmd = new DataCommand("LoadCompanyByName");
            cmd.SetParameter("@Name", DbType.String, name);
            Company result = cmd.ExecuteEntity<Company>();
            return result;
        }

        /// <summary>
        /// 批量修改Company状态
        /// </summary>
        public static void UpdateCompanyStatusBatch(IEnumerable<int> sysNos, CompanyStatus status)
        {
            DataCommand cmd = new DataCommand("UpdateCompanyStatusBatch");
            cmd.SetParameter("@CompanyStatus", DbType.Int32, status);
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }
        public static void UpdateCompanyStatus(int sysNo, CompanyStatus status)
        {
            DataCommand cmd = new DataCommand("UpdateCompanyStatus");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.SetParameter("@CompanyStatus", DbType.Int32, (int)status);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取公司的用户SysNo
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static string GetCompanyUserNoStr(int companySysNo)
        {
            DataCommand cmd = new DataCommand("GetCompanyUserNoStr");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            return cmd.ExecuteScalar<string>();
        }


        public static List<Company> LoadAllCompany()
        {
            DataCommand cmd = new DataCommand("LoadAllCompany");
            return cmd.ExecuteEntityList<Company>();
        }

        public static Company GetCompanyUser(int systemusersysno)
        {
            DataCommand cmd = new DataCommand("GetCompanyUser");
            cmd.SetParameter("@SystemUserSysNo", DbType.Int32, systemusersysno);
            return cmd.ExecuteEntity<Company>();
        }
        /// <summary>
        /// 批量删除客户
        /// </summary>
        /// <param name="sysNo"></param>
        public static void DeleteCompanyBatch(IEnumerable<int> sysNos)
        {
            DataCommand cmd = new DataCommand("DeleteCompanyBatch");
            cmd.CommandText = cmd.CommandText.Replace("#SysNos#", string.Join(",", sysNos));
            cmd.ExecuteNonQuery();
        }
    }
}