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
    public class ClientSmokeDetectorDA
    {
        /// <summary>
        /// 创建ClientSmokeDetector信息
        /// </summary>
        public static int InsertClientSmokeDetector(ClientSmokeDetector entity)
        {
            DataCommand cmd = new DataCommand("InsertClientSmokeDetector");
            cmd.SetParameter<ClientSmokeDetector>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新ClientSmokeDetector信息
        /// </summary>
        public static void UpdateClientSmokeDetector(ClientSmokeDetector entity)
        {
            DataCommand cmd = new DataCommand("UpdateClientSmokeDetector");
            cmd.SetParameter<ClientSmokeDetector>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个ClientSmokeDetector信息
        /// </summary>
        public static ClientSmokeDetector LoadClientSmokeDetector(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadClientSmokeDetector");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            ClientSmokeDetector result = cmd.ExecuteEntity<ClientSmokeDetector>();
            return result;
        }

        /// <summary>
        /// 删除ClientSmokeDetector信息
        /// </summary>
        public static void DeleteClientSmokeDetector(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteClientSmokeDetector");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }



        /// <summary>
        /// 
        /// </summary>
        public static List<ClientSmokeDetector> LoadClientSmokeDetectorByClientSysNo(int ClientSysNo)
        {
            DataCommand cmd = new DataCommand("LoadClientSmokeDetectorByClientSysNo");
            cmd.SetParameter("@ClientSysNo", DbType.Int32, ClientSysNo);
            return cmd.ExecuteEntityList<ClientSmokeDetector>();

        }


        /// <summary>
        /// 查找所有绑定设备的用户信息
        /// </summary>
        public static List<ClientSmokeDetector> LoadAllBindClientUser(string SerId)
        {
            DataCommand cmd = new DataCommand("LoadAllBindClientUser");
            cmd.SetParameter("@SmokeDetectorCode", DbType.String, SerId);
            return cmd.ExecuteEntityList<ClientSmokeDetector>();
        }


        public static List<ClientSmokeDetector> LoadAllUsertSmokeDetectors(string SerId)
        {
            DataCommand cmd = new DataCommand("LoadAllUsertSmokeDetectors");
            cmd.SetParameter("@SmokeDetectorCode", DbType.String, SerId);
            return cmd.ExecuteEntityList<ClientSmokeDetector>();
        }
    }
}