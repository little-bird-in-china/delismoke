using BlueStone.Smoke.Entity;
using BlueStone.Utility.Caching;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace BlueStone.Smoke.Backend.Facade
{
    public class CommonFacade
    {

    }

    public class CommonService
    {
        public static List<Area> GetAreaList()
        {
            string cacheKey = CacheManager.GenerateKey("GetAreaList");
            if (HttpRuntime.Cache[cacheKey] != null)
            {
                return (List<Area>)HttpRuntime.Cache[cacheKey];
            }

            List<Area> list = BlueStone.Smoke.Service.CommonService.GetAreaList();
            HttpRuntime.Cache.Insert(cacheKey, list, null, DateTime.Now.AddSeconds(CacheTime.Longest), Cache.NoSlidingExpiration);

            return list;
        }

        public static Area LoadAreaBySysNo(int sysNo)
        {
            List<Area> areaList = GetAreaList();
            return areaList.Find(a => a.SysNo == sysNo);
            //  return Rpc.Call<Area>("RPCService.CommonRPCService.LoadArea", sysNo) ?? new Area();
        }
    }
}