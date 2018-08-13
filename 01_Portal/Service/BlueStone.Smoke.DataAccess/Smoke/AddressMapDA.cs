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
    public class AddressMapDA
    {
        /// <summary>
        /// 创建AddressMap信息
        /// </summary>
        public static int InsertAddressMap(AddressMap entity)
        {
            DataCommand cmd = new DataCommand("InsertAddressMap");
            cmd.SetParameter<AddressMap>(entity); 
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新AddressMap信息
        /// </summary>
        public static void UpdateAddressMap(AddressMap entity)
        {
            DataCommand cmd = new DataCommand("UpdateAddressMap");
            cmd.SetParameter<AddressMap>(entity);
            cmd.ExecuteNonQuery();			 
        }

        /// <summary>
        /// 更新AddressMap信息
        /// </summary>
        public static void UpdateAddressMapImg(AddressMap entity)
        {
            DataCommand cmd = new DataCommand("UpdateAddressMapImg");
            cmd.SetParameter<AddressMap>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个AddressMap信息
        /// </summary>
        public static AddressMap LoadAddressMap(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadAddressMap");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            AddressMap result = cmd.ExecuteEntity<AddressMap>();
            return result; 
        }


        /// <summary>
        /// 删除AddressMap信息
        /// </summary>
        public static void DeleteAddressMap(int sysNo)
        { 
            DataCommand cmd = new DataCommand("DeleteAddressMap");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 分页查询AddressSmoke信息
        /// </summary>
        public static QueryResult<AddressMap> QueryAddressMapList(AddressMapFilter filter)
        {
            DataCommand cmd = new DataCommand("QueryAddressMapList");
            cmd.QuerySetCondition("AddressSysNo", ConditionOperation.Equal, DbType.Int32, filter.AddressSysNo);
            if (filter.AddressSysNos != null) {
                cmd.QuerySetCondition("AddressSysNo", ConditionOperation.In, DbType.String, string.Join(",", filter.AddressSysNos));
            }

            QueryResult<AddressMap> result = cmd.Query<AddressMap>(filter, "SysNo ASC");
            return result;
        }

        public static void UpdateAddressMapCoordinate(int sysno,string SmokeCoordinate,CurrentUser currentUser) {
            DataCommand cmd = new DataCommand("UpdateAddressMapCoordinate");
            cmd.SetParameter("SysNo",DbType.Int32,sysno);
            cmd.SetParameter("SmokeCoordinate", DbType.String, SmokeCoordinate);
            cmd.SetParameter("EditUserSysNo", DbType.Int32, currentUser.UserSysNo);
            cmd.SetParameter("EditUserName", DbType.String, currentUser.UserDisplayName);

            cmd.ExecuteNonQuery();
        }

        public static void UpdateAddressMapCoordinateBatch(List<AddressMap> list, CurrentUser currentUser)
        {
            if (list == null) {
                return;
            }

            DataCommand cmd = new DataCommand("UpdateAddressMapCoordinate");
            var sql = new StringBuilder();
            int i = 0;
            list.ForEach(addressmap => {
                sql.Append($@"UPDATE smoke.AddressMap SET 
                SmokeCoordinate=@SmokeCoordinate{i},
                `EditUserSysNo` = {currentUser.UserSysNo},
                `EditUserName` = @EditUserName,
                `EditDate` = now(3) WHERE SysNo ={ addressmap.SysNo} ;");

                cmd.SetParameter("SmokeCoordinate" + i, DbType.String, addressmap.SmokeCoordinate);

                i++;
            });

            
            cmd.CommandText = sql.ToString();
            cmd.SetParameter("EditUserName",DbType.String,currentUser.UserDisplayName);

            if (string.IsNullOrEmpty(cmd.CommandText)) {
                return;
            }
            cmd.ExecuteNonQuery();
        }

        public static void UpdateAddressMapName(int sysno, string name, CurrentUser currentUser)
        {
            DataCommand cmd = new DataCommand("UpdateAddressMapName");
            cmd.SetParameter("SysNo", DbType.Int32, sysno);
            cmd.SetParameter("Name", DbType.String, name);
            cmd.SetParameter("EditUserSysNo", DbType.Int32, currentUser.UserSysNo);
            cmd.SetParameter("EditUserName", DbType.String, currentUser.UserDisplayName);
            cmd.ExecuteNonQuery();
        }

        public static List<AddressMap> GetCompanyAddressMap(int companySysNo) {
            DataCommand cmd = new DataCommand("GetCompanyAddressMap");
            cmd.SetParameter("CompanySysNo", DbType.Int32, companySysNo);
            return cmd.ExecuteEntityList<AddressMap>();
        }

        //获取烟感器所在address对应的addressmap
        public static List<AddressMap> GetSmokeDetectorAddressMap(int smokeDetectorSysNo)
        {
            DataCommand cmd = new DataCommand("GetSmokeDetectorAddressMap");
            cmd.SetParameter("SmokeDetectorSysNo", DbType.Int32, smokeDetectorSysNo);
            return cmd.ExecuteEntityList<AddressMap>();
        }

        

    }
}