using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueStone.Smoke.Entity
{
    public class HomeMap
    {
        public int DeviceTotal { get; set; }

        public int DeviceOnline { get; set; }

        public int DeviceOffline { get; set; }

        public decimal OnlineRate
        {
            get
            {
                if (DeviceTotal > 0)
                {

                    return (decimal)DeviceOnline / DeviceTotal;
                }

                return 0;

            }
        }

        public decimal OffRate
        {
            get
            {
                if (DeviceTotal > 0)
                {

                    return (decimal)DeviceOffline / DeviceTotal;
                }

                return 0;

            }
        }

    }
}
