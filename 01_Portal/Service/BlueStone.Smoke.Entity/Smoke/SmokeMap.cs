using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueStone.Utility;

namespace BlueStone.Smoke.Entity
{
    //public class AddressMapSmoke:AddressMap
    //{
    //    public List<SmokeDetector> Smokes { get; set; }

    //}


    //public class AddressMapSmokeFilter : QueryFilter
    //{
    //    public int? CompanySysNo { get; set; }

    //    public int? AddressSysNo { get; set; }
    //}

    public class SmokeMap {

        public int AddressNo { get; set; }

        public List<AddressMap> AddressMaps { get; set; }

        //public List<SmokeDetector> Smokes { get; set; }

        //public List<Address> ChildrenAddress { get; set; }


        //待标记的数据
        public List<AddressMapMarker> Smokes { get; set; }

        //标记点
        public List<AddressMapMarker> Markers { get; set; }
    }

}
