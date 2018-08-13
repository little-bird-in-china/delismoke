using BlueStone.Smoke.Service;
using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Configuration;

namespace BlueStone.Smoke.Service
{
    public class SmokeDetectorServices
    {


        /// <summary>
        /// 创建SmokeDetector信息
        /// </summary>
        public static int InsertSmokeDetector(SmokeDetector entity)
        {
         
            var sysNo = SmokeDetectorDA.InsertSmokeDetector(entity);

           (new MapDataService(entity.CompanySysNo)).InitAsync();
           
            //更新首页缓存数据
          
            return sysNo;
        }

        /// <summary>
        /// 查询客户烟感器设备列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static QueryResult<QR_SmokeDetector> QuerySmokeDetectorList(QF_SmokeDetector filter)
        {
            return SmokeDetectorDA.QuerySmokeDetectorList(filter);
        }

        /// <summary>
        /// 加载客户烟感器在线&离线数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static SmokeDetectorCount LoadSmokeDetectorCount(int companySysNo)
        {
            return SmokeDetectorDA.LoadSmokeDetectorCount(companySysNo);
        }

        /// <summary>
        /// 加载烟感器的数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static SmokeDetectorCount LoadSmokeDetectorCountInArea(int companySysNo, int? addressId = null)
        {
            return SmokeDetectorDA.LoadSmokeDetectorCountInArea(companySysNo, addressId);
        }

        /// <summary>
        /// 用户绑定烟感器列表
        /// </summary>
        /// <param name="clientSysNo"></param>
        /// <returns></returns>
        public static QueryResult<QR_SmokeDetector> LoadUserSmokeDeletetorList(QF_UserDetector filter)
        {
            return SmokeDetectorDA.LoadUserSmokeDeletetorList(filter);
        }



        /// <summary>
        /// 加载用户绑定的烟感器在线&离线数量
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static SmokeDetectorCount LoadUserSmokeDetectorCount(int clientSysNo)
        {
            return SmokeDetectorDA.LoadUserSmokeDetectorCount(clientSysNo);
        }


        /// <summary>
        /// 用户批量解绑
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="clientSysNo"></param>
        public static void DeleteClientSmokeDetector(List<string> codes, int clientSysNo)
        {
            SmokeDetectorDA.DeleteClientSmokeDetector(codes, clientSysNo);
        }

        /// <summary>
        /// 解绑设备
        /// </summary>
        /// <param name="code"></param>
        public static void DeleteClientSmokeDetectorBycode(string code)
        {
            SmokeDetectorDA.DeleteClientSmokeDetectorBycode(code);
        }
        public static int InsertClientSmokeDetector(ClientSmokeDetector entity)
        {
            return SmokeDetectorDA.InsertClientSmokeDetector(entity);
        }
        /// <summary>
        /// 设备详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static SmokeDetector LoadSmokeDetail(string code)
        {
            SmokeDetector info= SmokeDetectorDA.LoadSmokeDetail(code);
            //if(info.Status== SmokeDetectorStatus.Delete)
            //{
            //    info = null;
            //}
            return info;
        }


        public static SmokeDetector LoadSmokeDetailByDeviceID(string deviceID)
        {
            return SmokeDetectorDA.LoadSmokeDetailByDeviceID(deviceID);
        }

        public static void UpdateSmokeDetector(SmokeDetector entity)
        {
            SmokeDetectorDA.UpdateSmokeDetector(entity);
        }

        public static void DeleteSmokeDetector(SmokeDetector smokeDetector, CurrentUser currentUser)
        {
            SmokeDetectorDA.DeleteSmokeDetector(smokeDetector.SysNo);
            DeleteSmokeCoordinate(smokeDetector, currentUser);
        }

        public static Task DeleteSmokeCoordinate(SmokeDetector smokeDetector, CurrentUser currentUser) {
            return Task.Run(()=> {
                try
                {
                    if (smokeDetector == null)
                    {
                        return;
                    }

                    var addressMaps = AddressMapService.GetSmokeDetectorAddressMap(smokeDetector.SysNo);
                    if (addressMaps == null)
                    {
                        return;
                    }

                    foreach (var addressMap in addressMaps)
                    {
                        if (string.IsNullOrEmpty(addressMap.SmokeCoordinate)) { continue; }
                        var markers = JsonConvert.DeserializeObject<List<AddressMapMarker>>(addressMap.SmokeCoordinate);
                        var marker = markers.Where(a => a.Type == AddressMapMarkerType.SmokeDetector && a.SysNo == smokeDetector.SysNo).FirstOrDefault();
                        if (marker != null)
                        {
                            markers.Remove(marker);
                            addressMap.SmokeCoordinate = JsonConvert.SerializeObject(markers);
                        }
                    }

                    AddressMapService.UpdateAddressMapCoordinateBatch(addressMaps, smokeDetector.CompanySysNo, currentUser);
                }
                catch (Exception e) {
                    Logger.WriteLog("删除烟感器时 删除地图点位失败:"+e.Message);
                }
            });
        }

        /// <summary>
        ///
        /// </summary>
        public static SmokeDetector IsUniquenessCode(string code)
        {
            SmokeDetector info = SmokeDetectorDA.LoadSmokeDetail(code);
            return info;
        }

        public static int InsertSmokeDetectorStatusLog(SmokeDetectorStatusLog entity)
        {
            return SmokeDetectorStatusLogDA.InsertSmokeDetectorStatusLog(entity);
        }

        public static SmokeDetectorStatusLog LoadSmokeDetectorStatusLogByDeviceCode(string code)
        {
            return SmokeDetectorStatusLogDA.LoadSmokeDetectorStatusLogByDeviceCode(code);
        }

        public static QueryResult<SmokeDetectorStatusLog> QueryDeviceNoticeList(QF_SmokeDetectorStatusLog filter)
        {
            return SmokeDetectorStatusLogDA.QueryDeviceNoticeList(filter);
        }
        public static QueryResult<SmokeDetectorMessage> QuerySmokeDetectorMessage(QF_SmokeDetectorMessage filter)
        {
            return SmokeDetectorDA.QuerySmokeDetectorMessage(filter);
        }
        public static List<MapDataSmokeDetector> GetMapDataSmokeDetector(int companySysNo)
        {
            return SmokeDetectorDA.GetMapDataSmokeDetector(companySysNo);
        }

        public static SmokeDetector LoadSmokeDetectorByInstaller(int installerSysNo)
        {
            return SmokeDetectorDA.LoadSmokeDetectorByInstaller(installerSysNo).FirstOrDefault();
        }

        public static List<SmokeDetector> LoadSmokeDetectorsByInstaller(int installerSysNo)
        {
            return SmokeDetectorDA.LoadSmokeDetectorByInstaller(installerSysNo);
        }
        public static List<SmokeDetector> LoadSmokeDetectorsByClientSysNo(int ClientSysNo)
        {
            return SmokeDetectorDA.LoadSmokeDetectorsByClientSysNo(ClientSysNo);
        }

        public static SmokeDetector LoadSmokeDetectorByDeviceID(string DeviceID)
        {
            return SmokeDetectorDA.LoadSmokeDetectorByDeviceID(DeviceID);
        }

    }
}