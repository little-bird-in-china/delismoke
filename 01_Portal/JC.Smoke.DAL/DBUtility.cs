using JC.Smoke.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JC.Smoke.DAL
{
    public class DBUtility
    {
        public static string DEFAULTDB = "server=localhost;database=smoke;uid=root;password=123456;Port=3306";
        public static MySqlConnection OpenConnection()
        {
            return OpenConnection(DEFAULTDB);
        }
        public static MySqlConnection OpenConnection(string connstr)
        {
            MySqlConnection retValue = null;
            try
            {
                //string sqlconnct = ConfigurationManager.ConnectionStrings[db].ConnectionString;
                retValue = new MySqlConnection(connstr);
                retValue.Open();
            }
            catch(Exception e)
            {
                Logger.Default.Error(e.Message);
            }
            return retValue;
        }
    }
}
