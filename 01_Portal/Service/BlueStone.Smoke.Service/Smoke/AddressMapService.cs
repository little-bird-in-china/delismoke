using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.DataAccess;
using BlueStone.Utility;
using Newtonsoft.Json;

namespace BlueStone.Smoke.Service
{
    public class AddressMapService
    {
        public static int InsertAddressMap(AddressMap entity)
        {
            var sysNo= AddressMapDA.InsertAddressMap(entity);

            UpdateHomeCacheData(entity.AddressSysNo);

            return sysNo; 
        }

        public static int InsertOrUpdateAddressMapImg(AddressMap entity) {
            var addressMaps=AddressMapDA.QueryAddressMapList(new AddressMapFilter { AddressSysNo = entity.AddressSysNo, PageSize = 100 }).data;
            if (addressMaps.Count > 0)
            {
                entity.SysNo = addressMaps.First().SysNo;
                AddressMapDA.UpdateAddressMapImg(entity);
                UpdateHomeCacheData(entity.AddressSysNo);
            }
            else {
                entity.SysNo=InsertAddressMap(entity);
            }

            return entity.SysNo;

        }


        public static void UpdateAddressMapCoordinate(int sysno, string SmokeCoordinate, CurrentUser currentUser)
        {
            AddressMapDA.UpdateAddressMapCoordinate(sysno, SmokeCoordinate,currentUser);

            UpdateHomeCacheDataByAddressMap(sysno);

        }

        public static void UpdateAddressMapCoordinateBatch(List<AddressMap> list,int companySysNo, CurrentUser currentUser)
        {
            AddressMapDA.UpdateAddressMapCoordinateBatch(list,currentUser);

            (new MapDataService(companySysNo)).DataChangeAsync();
        }

        



        public static void DeleteAddressMap(int sysNo)
        {
            var addressMap=AddressMapDA.LoadAddressMap(sysNo);
            AddressMapDA.DeleteAddressMap(sysNo);

            UpdateHomeCacheDataByAddressMap(addressMap);
        }


        public static void UpdateAddressMapName(int sysno, string name, CurrentUser currentUser)
        {
            AddressMapDA.UpdateAddressMapName(sysno,name,currentUser);
        }

        /// <summary>
        /// 分页查询AddressSmoke信息
        /// </summary>
        public static QueryResult<AddressMap> QueryAddressMapList(AddressMapFilter filter)
        {
            return AddressMapDA.QueryAddressMapList(filter);
        }

        public static List<AddressMap> GetCompanyAddressMap(int companySysNo)
        {
            return AddressMapDA.GetCompanyAddressMap(companySysNo);
        }

        public static List<HomeMapMarker> GetHomeMarkers(int addressMapSysNo,int compnaySysNo) {
            //报警状态:烟雾报警，测试报警，低电量报警
            var warnStatus = new List<SmokeDetectorStatus> { SmokeDetectorStatus.Warning, SmokeDetectorStatus.TestWarning, SmokeDetectorStatus.LowPower };

            var addressMap=AddressMapDA.LoadAddressMap(addressMapSysNo);
            if (addressMap == null) {
                throw new BusinessException("addressMap 不存在");
            }

            if (!string.IsNullOrWhiteSpace(addressMap.SmokeCoordinate)) {
                var countAddressList = new List<SmokeDetectorCountAddress>(); 
                var addressWarnList = new List<AddressWarn>();
                var smokeDetectors = new List<SmokeDetector>();
               
                var markers=JsonConvert.DeserializeObject<List<HomeMapMarker>>(addressMap.SmokeCoordinate);
                var addressMarker = markers.Where(a => a.Type == AddressMapMarkerType.Address);
                var smokeMarker= markers.Where(a => a.Type == AddressMapMarkerType.SmokeDetector);

                if (addressMarker.Count() > 0) {
                    var addressSysNos = addressMarker.Select(a => a.SysNo).ToList();
                    countAddressList = SmokeDetectorDA.LoadSmokeDetectorCountInArea(compnaySysNo, addressSysNos);
                    addressWarnList = SmokeDetectorDA.GetAddressWarnCount(addressSysNos);
                }

                if (smokeDetectors.Count() > 0) {
                    smokeDetectors = SmokeDetectorDA.LoadSmokeDetectors(smokeMarker.Select(a=>a.SysNo).ToList());
                }

                markers.ForEach(marker=> {
                    if (marker.Type == AddressMapMarkerType.Address)
                    {
                        var countAddress = countAddressList.FirstOrDefault(a => a.AddressSysNo == marker.SysNo);
                        if (countAddress != null)
                        {
                            marker.DeviceTotal = countAddress.ALLSmokeCount;
                            marker.DeviceOnline = countAddress.OnlineCount;
                            marker.DeviceOffline = countAddress.OfflineCount;
                        }

                        var addressWarn = addressWarnList.FirstOrDefault(a => a.AddressSysNo == marker.SysNo);
                        if (addressWarn != null && addressWarn.WarnCount > 0)
                        {
                            marker.IsWarning = true;
                        }
                    }
                    else if (marker.Type == AddressMapMarkerType.SmokeDetector) {
                        var smokeDetector = smokeDetectors.FirstOrDefault(a => a.SysNo == marker.SysNo);
                        if (smokeDetector != null && smokeDetector.Status.HasValue && warnStatus.Contains(smokeDetector.Status.Value))
                        {
                            marker.IsWarning = true;
                        }
                    }
                });

                return markers;
            }

            return new List<HomeMapMarker>();
        }


        private static void UpdateHomeCacheDataByAddressMap(int addressMapSysNo) {
            var addressMap = AddressMapDA.LoadAddressMap(addressMapSysNo);
            UpdateHomeCacheDataByAddressMap(addressMap);
        }

        private static void UpdateHomeCacheDataByAddressMap(AddressMap addressMap)
        {
            if (addressMap != null)
            {
                UpdateHomeCacheData(addressMap.AddressSysNo);
            }
        }

        private static void UpdateHomeCacheData(int addressSysNo) {
            var address = AddressService.LoadAddress(addressSysNo);
            if (address != null)
            {
                (new MapDataService(address.CompanySysNo)).DataChangeAsync();
            }
        }


        //获取烟感器所在address对应的addressmap
        public static List<AddressMap> GetSmokeDetectorAddressMap(int smokeDetectorSysNo)
        {
            return AddressMapDA.GetSmokeDetectorAddressMap(smokeDetectorSysNo);
        }



        //public static List<AddressMap> GetHomeMapMarkers(int compnaySysNo,List<int> addressSysNos) {

        //    var data = AddressMapService.QueryAddressMapList(new AddressMapFilter { PageSize=10000, AddressSysNos = addressSysNos }).data;
        //    if (data != null && data.Count > 0) {
        //        var list = new List<List<HomeMapMarker>>();
        //        data.ForEach(addressMap =>
        //        {
        //            if (!string.IsNullOrWhiteSpace(addressMap.SmokeCoordinate))
        //            {
        //                list.Add(JsonConvert.DeserializeObject<List<HomeMapMarker>>(addressMap.SmokeCoordinate));
        //            }
        //            else {
        //                list.Add(null);
        //            }
        //        });

        //        var notNullMarkers= list.Where(a => a != null).SelectMany(b=>b);
        //        var addressMarkers = notNullMarkers.Where(a => a.Type == AddressMapMarkerType.Address).ToList();
        //        var counts=SmokeDetectorDA.LoadSmokeDetectorCountInArea(compnaySysNo, addressMarkers.Select(a=>a.SysNo).ToList());


        //        foreach (var item in notNullMarkers) {
        //            if (item.Type == AddressMapMarkerType.Address) {
        //                var count=counts.FirstOrDefault(a => a.AddressSysNo == item.SysNo);
        //                if (count != null) {
        //                    item.DeviceTotal = count.ALLSmokeCount;
        //                    item.DeviceOnline = count.OnlineCount;
        //                    item.DeviceOffline = count.OfflineCount;
        //                }
        //            }
        //        }
        //    }

        //}

    }
}
