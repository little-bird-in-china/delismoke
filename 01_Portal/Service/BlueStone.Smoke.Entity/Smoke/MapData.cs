using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity
{
    public class MapData
    {
        public List<MapDataAddress> Address { get; set; }

        public List<MapDataSmokeDetector> SmokeDetectors { get; set; }

        public List<AddressMap> AddressMaps { get; set; }
    }

    public class MapDataAddress {
        public int SysNo { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int? ParentSysNo { get; set; }

        public int DeviceTotal { get; set; }

        public int DeviceOnline { get; set; }

        public int DeviceOffline { get; set; }



        //public decimal GetDeviceOnlineRate


        //public decimal DeviceOnlineRate { get {
        //        if (DeviceTotal > 0&& DeviceOnline<=DeviceTotal) {
        //            return Math.Round((decimal)DeviceOnline/DeviceTotal,2);
        //        }

        //        return 0;
        //    } }

        //public decimal DeviceOfflineRate
        //{
        //    get
        //    {
        //        if (DeviceTotal > 0 && DeviceOnline <= DeviceTotal)
        //        {
        //            return Math.Round((decimal)DeviceOnline / DeviceTotal, 2);
        //        }

        //        return 0;
        //    }
        //}


        //是否报警
        public bool? IsWarning { get; set; }

    }

    public class MapDataSmokeDetector
    {
        public int SysNo { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string AddressCode { get; set; }

        public int? AddressSysNo { get; set; }

        public string AddressName { get; set; }

        public string Position { get; set; }

        public SmokeDetectorStatus? Status { get; set; }
    }

    public class MapDataShowModel {
        public List<AddressMap> AddressMaps { get; set; }

        public MapDataAddress Address { get; set; }

        public List<MapDataSmokeDetector> WarningSmokeDetectors { get; set; }
    }
}
