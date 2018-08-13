using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;

namespace BlueStone.Smoke.DataAccess
{
    public class SmokeDetectorDA
    {
        /// <summary>
        /// 创建SmokeDetector信息
        /// </summary>
        public static int InsertSmokeDetector(SmokeDetector entity)
        {
            DataCommand cmd = new DataCommand("InsertSmokeDetector");
            cmd.SetParameter<SmokeDetector>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }

        /// <summary>
        /// 更新SmokeDetector信息
        /// </summary>
        public static void UpdateSmokeDetector(SmokeDetector entity)
        {
            DataCommand cmd = new DataCommand("UpdateSmokeDetector");
            cmd.SetParameter<SmokeDetector>(entity);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取单个SmokeDetector信息
        /// </summary>
        public static SmokeDetector LoadSmokeDetector(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetector");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            SmokeDetector result = cmd.ExecuteEntity<SmokeDetector>();
            return result;
        }

        /// <summary>
        /// 获取多个SmokeDetector信息
        /// </summary>
        public static List<SmokeDetector> LoadSmokeDetectors(List<int> sysNos)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetectors");
            cmd.CommandText = cmd.CommandText.Replace("#SysNos", string.Join(",", sysNos));
            List<SmokeDetector> result = cmd.ExecuteEntityList<SmokeDetector>();
            return result;
        }

        /// <summary>
        /// 删除SmokeDetector信息
        /// </summary>
        public static void DeleteSmokeDetector(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteSmokeDetector");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 查询客户烟感器设备列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult<QR_SmokeDetector> QuerySmokeDetectorList(QF_SmokeDetector filter)
        {
            DataCommand cmd = new DataCommand("QuerySmokeDetectorList");
            if (filter.Status.HasValue)
            {
                switch (filter.Status)
                {
                    case UISmokeDetectorStatus.FireWarning:
                        cmd.QuerySetCondition(" AND sd.Status IN (99,100)");
                        break;
                    case UISmokeDetectorStatus.LowPowerWarning:
                        cmd.QuerySetCondition(" AND sd.Status IN (90)");
                        break;
                    case UISmokeDetectorStatus.OffLine:
                        cmd.QuerySetCondition(" AND sd.Status IN (78,79,80)");
                        break;
                    case UISmokeDetectorStatus.Online:
                        cmd.QuerySetCondition(" AND sd.Status IN (0,1,2,3,4,5,6,7,8,90,99,100)");
                        break;
                }

            }
            string clientWhere = "";
            string clientJoin = "LEFT JOIN";
            if (!string.IsNullOrWhiteSpace(filter.ClientName))
            {
                clientWhere = "WHERE c.Name like '%" + cmd.SetSafeParameter(filter.ClientName) + "%'";
                clientJoin = "INNER JOIN";
            }

            cmd.CommandText = cmd.CommandText.Replace("#CLINETSTRWHERE#", clientWhere).Replace("#CLINETJOIN# ", clientJoin);
            cmd.QuerySetCondition("sd.Status", ConditionOperation.NotEqual, DbType.Int32, -999);
            if (filter.CompanySysNo.HasValue && filter.CompanySysNo.Value > 0)
            {
                cmd.QuerySetCondition("sd.CompanySysNo", ConditionOperation.Equal, DbType.Int32, filter.CompanySysNo.Value);
            }
            cmd.QuerySetCondition("sd.InstallerSysNo", ConditionOperation.In, DbType.Int32, filter.InstallSysNos);
            cmd.QuerySetCondition("sd.Code", ConditionOperation.Like, DbType.String, filter.Code);
            cmd.QuerySetCondition("sd.DeviceID", ConditionOperation.Like, DbType.String, filter.DeviceID);
            cmd.QuerySetCondition("sd.AddressCode", ConditionOperation.LikeLeft, DbType.String, filter.AddressCode);
            cmd.QuerySetCondition("sd.InstallerSysNo", ConditionOperation.Equal, DbType.Int32, filter.InstallerSysNo);
            QueryResult<QR_SmokeDetector> result = cmd.Query<QR_SmokeDetector>(filter, "sd.SysNo DESC ");
            return result;
        }

        /// <summary>
        /// 加载客户烟感器在线&离线数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static SmokeDetectorCount LoadSmokeDetectorCount(int companySysNo)
        {
            DataCommand cmd = new DataCommand("QuerySmokeDetectorList");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            SmokeDetectorCount result = cmd.ExecuteEntity<SmokeDetectorCount>();
            return result;
        }

        /// <summary>
        /// 加载烟感器的数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static SmokeDetectorCount LoadSmokeDetectorCountInArea(int companySysNo, int? addressId = null)
        {
            string addressCode = string.Empty;
            if (addressId.HasValue)
            {
                var address = AddressDA.LoadAddress(addressId.Value);
                if (address != null)
                {
                    addressCode = address.Code;
                }
            }

            //烟感器按状态分为 三类  在线(除去 离线与未初始化都时在线)  离线(OutNet，Lost,Offline)  未初始化(status is not)
            List<int> offlineStatus = new List<int> { (int)SmokeDetectorStatus.Lost, (int)SmokeDetectorStatus.Offline, (int)SmokeDetectorStatus.OutNet };

            DataCommand cmd = new DataCommand("LoadSmokeDetectorCountInArea");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.CommandText = cmd.CommandText.Replace("#offlineStatus", string.Join(",", offlineStatus));
            if (!string.IsNullOrWhiteSpace(addressCode))
            {
                cmd.CommandText = cmd.CommandText.Replace("#STRADDRESS#", $" AND AddressCode LIKE '{addressCode}%'");
            }
            else
            {
                cmd.CommandText = cmd.CommandText.Replace("#STRADDRESS#", "");
            }

            SmokeDetectorCount result = cmd.ExecuteEntity<SmokeDetectorCount>();
            return result;
        }


        /// <summary>
        /// 加载烟感器的数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static List<SmokeDetectorCountAddress> LoadSmokeDetectorCountInArea(int companySysNo, List<int> addressIds)
        {
            var signleSql = string.Empty;

            //烟感器按状态分为 三类  在线(除去 离线与未初始化都时在线)  离线(OutNet，Lost,Offline)  未初始化(status is not)
            List<int> offlineStatus = new List<int> { (int)SmokeDetectorStatus.Lost, (int)SmokeDetectorStatus.Offline, (int)SmokeDetectorStatus.OutNet };

            DataCommand cmd = new DataCommand("LoadSmokeDetectorCountInAreaMany");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            signleSql = cmd.CommandText.Replace("#offlineStatus", string.Join(",", offlineStatus));

            var allAddress = AddressDA.GetAddressBySysNos(addressIds);


            for (int i = 0; i < addressIds.Count; i++)
            {
                var currentSql = signleSql.Replace("#AddressSysNo", addressIds[i].ToString());
                var address = allAddress.FirstOrDefault(a => a.SysNo == addressIds[i]);

                if (address != null)
                {
                    var addSql = currentSql.Replace("#STRADDRESS#", $" AND AddressCode LIKE '{address.Code}%'");
                    if (i == 0)
                    {
                        cmd.CommandText = addSql;
                    }
                    else
                    {
                        cmd.CommandText += " union " + addSql;
                    }
                }
            }


            List<SmokeDetectorCountAddress> result = cmd.ExecuteEntityList<SmokeDetectorCountAddress>();

            return result;
        }

        public static List<SmokeDetector> LoadSmokeDetectorsByClientSysNo(int clientSysNo)
        {
            DataCommand cmd = new DataCommand("LoadUserSmokeDeletetorListList");
            cmd.SetParameter("@ClientSysNo", DbType.Int32, clientSysNo);
            List<SmokeDetector> result = cmd.ExecuteEntityList<SmokeDetector>();
            return result;
        }

        /// <summary>
        /// 加载烟感器的数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static List<AddressWarn> GetAddressWarnCount(List<int> addressIds)
        {
            var signleSql = string.Empty;


            DataCommand cmd = new DataCommand("GetAddressWarnCount");
            signleSql = cmd.CommandText;

            var allAddress = AddressDA.GetAddressBySysNos(addressIds);


            for (int i = 0; i < addressIds.Count; i++)
            {
                var addSql = signleSql.Replace("#AddressSysNo", addressIds[i].ToString());

                //var addSql = currentSql.Replace("#STRADDRESS#", $" AND AddressCode LIKE '{address.Code}%'");
                if (i == 0)
                {
                    cmd.CommandText = addSql;
                }
                else
                {
                    cmd.CommandText += " union " + addSql;
                }
            }



            List<AddressWarn> result = cmd.ExecuteEntityList<AddressWarn>();

            return result;
        }

        /// <summary>
        /// 用户绑定烟感器列表
        /// </summary>
        /// <param name="clientSysNo"></param>
        /// <returns></returns>
        public static QueryResult<QR_SmokeDetector> LoadUserSmokeDeletetorList(QF_UserDetector filter)
        {
            DataCommand cmd = new DataCommand("LoadUserSmokeDeletetorList");
            switch (filter.Status)
            {
                case UISmokeDetectorStatus.FireWarning:
                    cmd.QuerySetCondition(" AND sd.Status IN (99,100)");
                    break;
                case UISmokeDetectorStatus.LowPowerWarning:
                    cmd.QuerySetCondition(" AND sd.Status IN (90)");
                    break;
                case UISmokeDetectorStatus.OffLine:
                    cmd.QuerySetCondition(" AND sd.Status IN (78,79,80)");
                    break;
                case UISmokeDetectorStatus.Online:
                    cmd.QuerySetCondition(" AND sd.Status IN (0,1,2,3,4,5,6,7,8,90,99,100)");
                    break;
            }
            cmd.QuerySetCondition("c.ClientSysNo", ConditionOperation.Equal, DbType.Int32, filter.ClientSysNo);
            return cmd.Query<QR_SmokeDetector>(filter, "sd.Status DESC, sd.InDate DESC");
        }


        /// <summary>
        /// 加载用户绑定的烟感器在线&离线数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static SmokeDetectorCount LoadUserSmokeDetectorCount(int clientSysNo)
        {
            DataCommand cmd = new DataCommand("LoadUserSmokeDetectorCount");
            cmd.SetParameter("@ClientSysNo", DbType.Int32, clientSysNo);
            SmokeDetectorCount result = cmd.ExecuteEntity<SmokeDetectorCount>();
            return result;
        }


        /// <summary>
        /// 用户批量解绑
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="clientSysNo"></param>
        public static void DeleteClientSmokeDetector(List<string> codes, int clientSysNo)
        {
            DataCommand cmd = new DataCommand("DeleteClientSmokeDetector");
            cmd.SetParameter("@ClientSysNo", DbType.Int32, clientSysNo);
            cmd.CommandText = cmd.CommandText.Replace("#CodeList#", "'" + string.Join("','", codes) + "'");
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 解绑设备
        /// </summary>
        /// <param name="code"></param>
        public static void DeleteClientSmokeDetectorBycode(string code)
        {
            DataCommand cmd = new DataCommand("DeleteClientSmokeDetectorBycode");
            cmd.SetParameter("@SmokeDetectorCode", DbType.String, code);
            cmd.ExecuteNonQuery();
        }


        public static int InsertClientSmokeDetector(ClientSmokeDetector entity)
        {
            DataCommand cmd = new DataCommand("InsertClientSmokeDetector");
            cmd.SetParameter<ClientSmokeDetector>(entity);
            return cmd.ExecuteScalar<int>();
        }

        /// <summary>
        /// 设备详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SmokeDetector LoadSmokeDetail(string code)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetail");
            cmd.SetParameter("@Code", DbType.String, code);
            return cmd.ExecuteEntity<SmokeDetector>();
        }

        public static SmokeDetector LoadSmokeDetailByDeviceID(string deviceID)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetailByDeviceID");
            cmd.SetParameter("@DeviceID", DbType.String, deviceID);
            return cmd.ExecuteEntity<SmokeDetector>();
        }
        public static List<SmokeDetector> GetSmokeDetectorByAddressSysNo(int companySysNo, List<int> addressSysNos)
        {
            DataCommand cmd = new DataCommand("GetSmokeDetectorByAddressSysNo");
            cmd.SetParameter("CompanySysNo", DbType.Int32, companySysNo);
            cmd.CommandText = cmd.CommandText.Replace("#AddressNos", string.Join(",", addressSysNos));
            return cmd.ExecuteEntityList<SmokeDetector>();
        }

        public static List<MapDataSmokeDetector> GetMapDataSmokeDetector(int companySysNo)
        {
            DataCommand cmd = new DataCommand("GetCompanySmokeDetector");
            cmd.SetParameter("CompnaySysNo", DbType.Int32, companySysNo);
            return cmd.ExecuteEntityList<MapDataSmokeDetector>();
        }

        public static List<SmokeDetector> LoadSmokeDetectorByInstaller(int installerSysNo)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetectorByInstaller");
            cmd.SetParameter("@InstallerSysNo", DbType.Int32, installerSysNo);
            return cmd.ExecuteEntityList<SmokeDetector>();
        }

        public static SmokeDetector LoadSmokeDetectorByDeviceID(string DeviceID)
        {
            DataCommand cmd = new DataCommand("LoadSmokeDetectorByDeviceID");
            cmd.SetParameter("@DeviceID", DbType.String, DeviceID);
            return cmd.ExecuteEntity<SmokeDetector>();
        }

        public static QueryResult<SmokeDetectorMessage> QuerySmokeDetectorMessage(QF_SmokeDetectorMessage filter)
        {
            DataCommand cmd = new DataCommand("QuerySmokeDetectorMessage");
            cmd.QuerySetCondition("m.MasterID", ConditionOperation.Equal, DbType.String, filter.MasterID);
            cmd.QuerySetCondition("m.MasterName", ConditionOperation.Equal, DbType.String, filter.MasterName);
            cmd.QuerySetCondition("m.MsgType", ConditionOperation.Equal, DbType.Int32, filter.MsgType);
            cmd.QuerySetCondition("m.Status", ConditionOperation.Equal, DbType.Int32,1);
            QueryResult<SmokeDetectorMessage> result = cmd.Query<SmokeDetectorMessage>(filter, "m.handleTime DESC");
            return result;

        }
    }
}