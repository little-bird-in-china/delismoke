using JC.Smoke.Model;
using MySql.Data.MySqlClient;
using Dapper;
using System.Linq;

namespace JC.Smoke.DAL
{
    public class EmployeeDal
    {
        public static Employee GetEmployee(int id, MySqlConnection conn)
        {
            string rawSql = "SELECT * FROM Employee WHERE id=@id";
            return conn.Query<Employee>(rawSql, new { id = id }).FirstOrDefault();
        }
        public static Employee Login(string username, string password, MySqlConnection conn)
        {
            string rawSql = "SELECT * FROM Employee WHERE mobile_no=@un AND password=@pwd AND status&1>0";
            return conn.Query<Employee>(rawSql,new {un=username, pwd=password}).FirstOrDefault();
        }

        public static bool CheckPwd(int uid, string password, MySqlConnection conn)
        {
            string rawSql = "SELECT COUNT(*) FROM Employee WHERE uid=@uid AND password=@pwd";
            return conn.ExecuteScalar<int>(rawSql, new { uid=uid, pwd=password}) < 0;
        }

        public static int UpdatePwd(int uid, string password, MySqlConnection conn)
        {
            string rawSql = "UPDATE Employee SET password=@pwd WHERE uid=@uid";
            return conn.Execute(rawSql, new { uid = uid, pwd = password });
        }
    }
}
