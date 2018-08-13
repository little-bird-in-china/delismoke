using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;

namespace BlueStone.Smoke.DataAccess
{
    public class ClientDA
    {
        /// <summary>
        /// 创建Client信息
        /// </summary>
        public static int InsertClient(Client entity)
        {
            DataCommand cmd = new DataCommand("InsertClient");
            cmd.SetParameter<Client>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新Client信息
        /// </summary>
        public static void UpdateClient(Client entity)
        {
            DataCommand cmd = new DataCommand("UpdateClient");
            cmd.SetParameter<Client>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个Client信息
        /// </summary>
        public static Client LoadClient(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadClient");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            Client result = cmd.ExecuteEntity<Client>();
            return result;
        }

        /// <summary>
        /// 获取单个Client信息
        /// </summary>
        public static Client LoadClientByMangerSysNo(int mangerSysNo)
        {
            DataCommand cmd = new DataCommand("LoadClientByMangerSysNo");
            cmd.SetParameter("@ManagerSysNo", DbType.Int32, mangerSysNo);
            Client result = cmd.ExecuteEntity<Client>();
            return result;
        }

        public static Client LoadClientByAppCustomerID(string appCustomerId)
        {
            DataCommand cmd = new DataCommand("LoadClientByAppCustomerID");
            cmd.SetParameter("@AppCustomerId", DbType.String, appCustomerId);
            Client result = cmd.ExecuteEntity<Client>();
            return result;
        }
        /// <summary>
        /// 删除Client信息
        /// </summary>
        public static void DeleteClient(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteClient");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询Client信息
        /// </summary>
        public static QueryResult<Client> QueryClientList(ClientFilter filter)
        {
            DataCommand cmd = new DataCommand("QueryClientList");
            cmd.QuerySetCondition("d.CompanySysNo", ConditionOperation.Equal, DbType.Int32, filter.CompanySysNo);
            cmd.QuerySetCondition("cd.SmokeDetectorCode", ConditionOperation.Equal, DbType.AnsiString, cmd.SetSafeParameter(filter.SmokeDetectorCode));

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                cmd.QuerySetCondition(string.Format(" AND (c.Name like '%{0}%' or c.Cellphone like '%{0}%' or c.Cellphone2 like '%{0}%'  or c.Cellphone3 like '%{0}%')", cmd.SetSafeParameter(filter.Keyword)));
            }
            cmd.SetParameter("@Cellphone", DbType.Int32, filter.ExactCellphone);
            QueryResult<Client> result = cmd.Query<Client>(filter, " c.Name ASC");
            return result;
        }

        /// <summary>
        /// 分页查询Client信息
        /// </summary>
        public static QueryResult<Client> QueryAllClientList(ClientFilter filter)
        {
            DataCommand cmd = new DataCommand("QueryAllClientList");
            string joinString = "";
            if (!string.IsNullOrWhiteSpace(filter.SmokeDetectorCode))
            {
                joinString = string.Format("INNER JOIN (select distinct ClientSysNo from smoke.clientsmokedetector where  (SmokeDetectorCode like '%{0}%')) cd on cd.ClientSysNo =  c.SysNo", cmd.SetSafeParameter(filter.SmokeDetectorCode));
            }
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                cmd.QuerySetCondition(string.Format(" AND (c.Name like '%{0}%' or c.Cellphone like '%{0}%' or c.Cellphone2 like '%{0}%'  or c.Cellphone3 like '%{0}%')", cmd.SetSafeParameter(filter.Keyword)));
            }
            cmd.CommandText = cmd.CommandText.Replace("#JOINSQL#", joinString);
            QueryResult<Client> result = cmd.Query<Client>(filter, " c.Name ASC");
            return result;
        }
        /// <summary>
        /// 批量修改Client状态
        /// </summary>
        public static void UpdateClientStatusBatch(IEnumerable<int> sysNos, CommonStatus status)
        {
            DataCommand cmd = new DataCommand("UpdateClientStatusBatch");
            cmd.SetParameter("@CommonStatus", DbType.Int32, status);
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 批量删除Client
        /// </summary>
        public static void DeleteClientBatch(IEnumerable<int> sysNos)
        {
            DataCommand cmd = new DataCommand("DeleteClientBatch");
            cmd.CommandText = cmd.CommandText.Replace("#BatchSysNo#", string.Join(",", from s in sysNos select s.ToString()));
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新ClientContact信息
        /// </summary>
        public static void UpdateClientContact(Client entity)
        {
            DataCommand cmd = new DataCommand("UpdateClientContact");
            cmd.SetParameter<Client>(entity);
            cmd.ExecuteNonQuery();
        }


    }
}