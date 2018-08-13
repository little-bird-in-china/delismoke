using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueStone.Smoke.Entity;
using System.Runtime.Caching;
using Newtonsoft.Json;

namespace BlueStone.Smoke.Service
{
    public class MapDataService
    {
        public static int CacheDays = 10;  //缓存时间(天)
        public int CompanySysNo;
        public string Key;
        public MemoryCache Cache;

        public MapDataService(int companySysNo) {
            this.CompanySysNo = companySysNo;
            this.Key = $"MapData{CompanySysNo}";

            this.Cache = MemoryCache.Default;
        }

        public void Init() {
            var model = new MapData();

            model.SmokeDetectors = SmokeDetectorServices.GetMapDataSmokeDetector(CompanySysNo);
            model.Address = AddressService.GetMapDataAddress(CompanySysNo);
            model.AddressMaps = AddressMapService.GetCompanyAddressMap(CompanySysNo);

            Cache.Set(Key, model, DateTime.Now.AddDays(CacheDays));
        }

        public Task DataChangeAsync() {
            return Task.Run(() => {
                Init();

                var companyUserNoStr=CompanyService.GetCompanyUserNoStr(this.CompanySysNo);
                WebSocketService.SendMessageToUser(companyUserNoStr, "{\"action\":\"reload\"}");

                //WebSocketService.SendClientMessage("{\"action\":\"reload\"}");
            });
        }


        public Task InitAsync() {
            return Task.Run(() => {
                Init();
            });
        }

        public MapData Get() {
            var obj = Cache.Get(Key);
            var model = obj as MapData;
            if (model == null)
            {
                Init();
                obj = Cache.Get(Key);
                model = obj as MapData;
            }


            //Init();
            //var obj = Cache.Get(Key);
            //var model = obj as MapData;
            return model;
        }

        public void Set(MapData model) {
            Cache.Set(Key, model, DateTime.Now.AddDays(CacheDays));
        }

        public MapDataShowModel GetShowModel(int? addressSysNo = null)
        {
            var model = new MapDataShowModel();
            var mapData = Get();
            if (mapData != null && mapData.Address != null)
            {
                MapDataAddress address = null;
                if (addressSysNo == null || addressSysNo <= 0)
                {
                    address = mapData.Address.Where(a => a.ParentSysNo == null || a.ParentSysNo == 0).FirstOrDefault();
                }
                else
                {
                    address = mapData.Address.Where(a => a.SysNo == addressSysNo).FirstOrDefault();
                }

                if (address != null)
                {
                    //var addressMaps=GetChildrenAddressMap(address.Code, mapData.AddressMaps, mapData.Address);
                    var addressMaps = GetCurrentAddressMap(address.SysNo, mapData.AddressMaps);

                    addressMaps.ForEach(addressMap => {
                        if (!string.IsNullOrWhiteSpace(addressMap.SmokeCoordinate))
                        {
                            var markers = JsonConvert.DeserializeObject<List<HomeMapMarker>>(addressMap.SmokeCoordinate);
                            if (markers != null)
                            {
                                SetMarkers(markers, mapData);
                                addressMap.Markers = markers;
                            }

                        }
                    });

                    model.Address = address;
                    model.AddressMaps = addressMaps;
                    model.WarningSmokeDetectors = GetWarnSmokeDetector(mapData.Address,mapData.SmokeDetectors);
                    return model;
                }
            }

            return model;
        }

        //public List<AddressMap> GetAddressMapAndMarker(int? addressSysNo=null) {
        //    var mapData = Get();
        //    if (mapData != null&&mapData.Address!=null) {
        //        MapDataAddress address = null;
        //        if (addressSysNo == null||addressSysNo<=0)
        //        {
        //            address = mapData.Address.Where(a => a.ParentSysNo == null||a.ParentSysNo==0).FirstOrDefault();
        //        }
        //        else {
        //            address = mapData.Address.Where(a => a.SysNo == addressSysNo).FirstOrDefault();
        //        }

        //        if (address != null) {
        //            //var addressMaps=GetChildrenAddressMap(address.Code, mapData.AddressMaps, mapData.Address);
        //            var addressMaps = GetCurrentAddressMap(address.SysNo, mapData.AddressMaps);
                    
        //            addressMaps.ForEach(addressMap=> {
        //                if (!string.IsNullOrWhiteSpace(addressMap.SmokeCoordinate)) {
        //                    var markers = JsonConvert.DeserializeObject<List<HomeMapMarker>>(addressMap.SmokeCoordinate);
        //                    if (markers != null) {
        //                        SetMarkers(markers,mapData);
        //                        addressMap.Markers = markers;
        //                    }

        //                }
        //            });

        //            return addressMaps;
        //        }
        //    }

        //    return new List<AddressMap>();
        //}


        public List<AddressMap> GetChildrenAddressMap(string code,List<AddressMap> addressMaps,List<MapDataAddress> address) {
            var childHasImgAddress=
            from a in address
            join b in addressMaps
            on a.SysNo equals b.AddressSysNo
            where a.Code.StartsWith(code) && !string.IsNullOrWhiteSpace(b.MapImage)
            orderby a.Code
            select a;

            var child = childHasImgAddress.FirstOrDefault();

            if (child != null) {
                var result=
                from a in address
                join b in addressMaps
                on a.SysNo equals b.AddressSysNo
                where a.ParentSysNo==child.ParentSysNo 
                select b;

                if (result.Count() > 0) {
                    return result.ToList();
                }
            }
            return new List<AddressMap>();

        }

        public List<AddressMap> GetCurrentAddressMap(int addressSysNo, List<AddressMap> addressMaps)
        {
            var result= addressMaps.Where(a => a.AddressSysNo == addressSysNo);
            if (result.Count() > 0) {
                return result.ToList();
            }
            return new List<AddressMap>();

        }

        public List<MapDataSmokeDetector> GetWarnSmokeDetector(List<MapDataAddress> address, List<MapDataSmokeDetector> smokeDetectors) {
            var data = new List<MapDataSmokeDetector>();
            if (smokeDetectors == null || smokeDetectors.Count == 0) {
                return data;
            }

            var warnSmokeDetectors=smokeDetectors.Where(a => a.Status.HasValue && (a.Status == SmokeDetectorStatus.Warning || a.Status == SmokeDetectorStatus.TestWarning));
            if (warnSmokeDetectors == null || warnSmokeDetectors.Count() == 0) {
                return data;
            }
            foreach (var warnSmokeDetector in warnSmokeDetectors) {
                var currentAddress = address.Where(a => a.Code == warnSmokeDetector.AddressCode).FirstOrDefault();
                warnSmokeDetector.AddressSysNo = currentAddress?.SysNo;
                warnSmokeDetector.Name = $"{warnSmokeDetector.AddressName}>{(string.IsNullOrEmpty(warnSmokeDetector.Position)? warnSmokeDetector.Code:warnSmokeDetector.Position)}";
            }

            return warnSmokeDetectors.ToList();
        }

        public void SetMarkers(List<HomeMapMarker> markers,MapData mapData) {
            var addressMarkers = markers.Where(a => a.Type == AddressMapMarkerType.Address);
            var smokeDetectorMarkers = markers.Where(a => a.Type == AddressMapMarkerType.SmokeDetector);

            foreach (var addressMarker in addressMarkers)
            {
                var countAddress = mapData.Address.FirstOrDefault(a => a.SysNo == addressMarker.SysNo);
                if (countAddress != null)
                {
                    addressMarker.DeviceTotal = countAddress.DeviceTotal;
                    addressMarker.DeviceOnline = countAddress.DeviceOnline;
                    addressMarker.DeviceOffline = countAddress.DeviceOffline;
                    if (countAddress.IsWarning == true)
                    {
                        addressMarker.IsWarning = countAddress.IsWarning;
                    }

                }
            }

            foreach (var smokeDetector in smokeDetectorMarkers)
            {
                var dbSmokeDetector = mapData.SmokeDetectors.FirstOrDefault(a => a.SysNo == smokeDetector.SysNo);
                if (dbSmokeDetector != null && (dbSmokeDetector.Status == SmokeDetectorStatus.Warning || dbSmokeDetector.Status == SmokeDetectorStatus.TestWarning))
                {
                    smokeDetector.IsWarning = true;
                }
            }
        }

    }
}
