using JC.Smoke.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace JC.Smoke.DAL
{
    public class MenuDal
    {
        public static Menu GetMenu(int id, MySqlConnection conn)
        {
            string rawSql = "SELECT * FROM Menu WHERE id=@id";
            return conn.Query<Menu>(rawSql, new { id = id }).FirstOrDefault();
        }

        public static Menu[] GetMenusByParentID(int parent_id, MySqlConnection conn)
        {
            string rawSql = "SELECT * FROM Menu WHERE parent_id=@parent_id";
            return conn.Query<Menu>(rawSql, new { parent_id = parent_id }).ToArray();
        }

        public static Menu[] GetAllMenus(MySqlConnection conn)
        {
            string rawSql = "SELECT * FROM Menu WHERE parent_id>0";
            return conn.Query<Menu>(rawSql).ToArray();
        }

        public static Menu[] GetMenus(string ids, MySqlConnection conn)
        {
            string rawSql = String.Format("SELECT * FROM Menu WHERE id IN ({0})", ids);
            return conn.Query<Menu>(rawSql).ToArray();
        }
    }
}
