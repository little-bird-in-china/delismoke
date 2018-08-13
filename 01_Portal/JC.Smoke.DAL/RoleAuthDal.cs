using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using JC.Smoke.Model;
using MySql.Data.MySqlClient;

namespace JC.Smoke.DAL
{
    public class RoleAuthDal
    {
        public static RoleAuth[] GetRoleAuths(int role_id, MySqlConnection conn)
        {
            string rawSql = "SELECT * FROM RoleAuth WHERE role_id=@role_id";
            return conn.Query<RoleAuth>(rawSql, new { role_id = role_id }).ToArray();
        }
    }
}
