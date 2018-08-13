using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    public class AddressMap:EntityBase
    {

        /// <summary>
        /// 系统编号：系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 地址系统编号：
        /// </summary>
        public int AddressSysNo { get; set; }

        /// <summary>
        /// 地国名称：
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地图图片地址：
        /// </summary>
        public string MapImage { get; set; }

        /// <summary>
        /// 烟感器坐标：烟感器坐标相对图片的坐标位置 JSON数据存储，格式如:[{No:123,X:40,Y:20},{No:124,X:40,Y:20}]
        /// </summary>
        public string SmokeCoordinate { get; set; }

        public List<HomeMapMarker> Markers { get; set; }

    }

    public class AddressMapFilter : QueryFilter {
        public int? AddressSysNo { get; set; }

        public List<int> AddressSysNos { get; set; }
    }


    public class AddressMapMarker {
        public int SysNo { get; set; }

        public string Name { get; set; }

        public AddressMapMarkerType Type { get; set; }

        public string X { get; set; }

        public string Y { get; set; }
    }

    public class HomeMapMarker:AddressMapMarker {
        public int DeviceTotal { get; set; }

        public int DeviceOnline { get; set; }

        public int DeviceOffline { get; set; }

        public bool? IsWarning { get; set; }

    }



    public enum AddressMapMarkerType
    {
        //烟感器
        SmokeDetector=1,

        //地址
        Address =2,
    }

}