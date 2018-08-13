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
    public class MenuPurviewDal
    {
        public static MenuPurview[] GetPurviewsByMenuID(int menu_id, MySqlConnection conn)
        {
            string rawSql = @"SELECT id,menu_id,purview,name,create_by,CONVERT(varchar,create_date,120) AS create_date
                                FROM MenuPurview WHERE menu_id=@menu_id";
            return conn.Query<MenuPurview>(rawSql, new { menu_id = menu_id }).ToArray();
        }
    }
}
