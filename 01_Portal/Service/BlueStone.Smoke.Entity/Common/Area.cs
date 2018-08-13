using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlueStone.Smoke.Entity;

namespace BlueStone.Smoke.Entity
{
    public class Area
    {
        public int? SysNo { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        public int? ProvinceSysNo { get; set; }
        /// <summary>
        /// 省名称
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public int? CitySysNo { get; set; }
        /// <summary>
        /// 市名称
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区名称
        /// </summary>
        public string DistrictName { get; set; }
        /// <summary>
        /// 状态，通用状态，共两种：有效，无效，删除是将状态设置为-999
        /// </summary>
        public CommonStatus? Status { get; set; }

        public string GBCode { get; set; }

        public string OrderNumber { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public int? RegionSysNo
        {
            get
            {
                if (CitySysNo.HasValue && CitySysNo.Value > 0)
                {
                    return SysNo;
                }

                return null;
            }
        }
    }
}
