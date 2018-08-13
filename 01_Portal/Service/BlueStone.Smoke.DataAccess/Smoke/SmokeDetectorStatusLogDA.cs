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
    public class SmokeDetectorStatusLogDA
    {
        /// <summary>
        /// 创建SmokeDetectorStatusLog信息
        /// </summary>
        public static int InsertSmokeDetectorStatusLog(SmokeDetectorStatusLog entity)
        {
            DataCommand cmd = new DataCommand("InsertSmokeDetectorStatusLog");
            cmd.SetParameter<SmokeDetectorStatusLog>(entity); 
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新SmokeDetectorStatusLog信息
        /// </summary>
        public static void UpdateSmokeDetectorStatusLog(SmokeDetectorStatusLog entity)
        {
            DataCommand cmd = new DataCommand("UpdateSmokeDetectorStatusLog");
            cmd.SetParameter<SmokeDetectorStatusLog>(entity);
            cmd.ExecuteNonQuery();			 
        }

        /// <summary>
        /// 获取单个SmokeDetectorStatusLog信息
        /// </summary>
        public static SmokeDetectorStatusLog LoadSmokeDetectorStatusLog(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetectorStatusLog");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            SmokeDetectorStatusLog result = cmd.ExecuteEntity<SmokeDetectorStatusLog>();
            return result; 
        }
        public static SmokeDetectorStatusLog LoadSmokeDetectorStatusLogByDeviceCode(string code)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetectorStatusLogByDeviceCode");
            cmd.SetParameter("@SmokeDetectorCode", DbType.String, code);
            SmokeDetectorStatusLog result = cmd.ExecuteEntity<SmokeDetectorStatusLog>();
            return result;
        }
        
        /// <summary>
        /// 删除SmokeDetectorStatusLog信息
        /// </summary>
        public static void DeleteSmokeDetectorStatusLog(int sysNo)
        { 
            DataCommand cmd = new DataCommand("DeleteSmokeDetectorStatusLog");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        public static QueryResult<SmokeDetectorStatusLog> QueryDeviceNoticeList(QF_SmokeDetectorStatusLog filter)
        {

            DataCommand cmd = new DataCommand("QueryDeviceNoticeList");
            cmd.QuerySetCondition("m.SmokeDetectorCode", ConditionOperation.Equal, DbType.String, filter.DeviceCode); 
            QueryResult<SmokeDetectorStatusLog> result = cmd.Query<SmokeDetectorStatusLog>(filter, "m.BeginTime DESC ");
            return result;
        }
    }
}