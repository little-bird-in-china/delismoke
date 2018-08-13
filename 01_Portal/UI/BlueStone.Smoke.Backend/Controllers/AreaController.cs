using BlueStone.Smoke.Backend.Facade;
using BlueStone.Smoke.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    public class AreaController : BaseController
    {
        /// <summary>
        /// 获取所有省
        /// </summary>
        /// <returns></returns>
        public JsonResult GetProvince()
        {
            List<Area> list = CommonService.GetAreaList();
            if (list != null)
            {
                list = list.FindAll(item => !item.ProvinceSysNo.HasValue);
            }

            return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取省所有市
        /// </summary>
        /// <param name="provinceSysNo"></param>
        /// <returns></returns>
        public JsonResult GetCity(int? provinceSysNo)
        {
            List<Area> list = CommonService.GetAreaList();
            if (list != null)
            {
                list = list.FindAll(item => (item.ProvinceSysNo.HasValue && item.ProvinceSysNo.Value == provinceSysNo && !item.CitySysNo.HasValue));
            }

            return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取市所有区
        /// </summary>
        /// <param name="citySysNo"></param>
        /// <returns></returns>
        public JsonResult GetDistrict(int citySysNo)
        {
            List<Area> list = CommonService.GetAreaList();
            if (list != null)
            {
                list = list.FindAll(item => (item.CitySysNo.HasValue && item.CitySysNo.Value == citySysNo));
            }

            return Json(new { Success = true, Data = list }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreaPath(int districtSysNo)
        {
            List<Area> list = CommonService.GetAreaList();
            if (list != null)
            {
                list = list.FindAll(item => (item.SysNo.HasValue && item.SysNo.Value == districtSysNo));
            }

            Area district = list.FirstOrDefault();

            return Json(new
            {
                Success = true,
                Data = new
                {
                    ProvinceSysNo = district.ProvinceSysNo,
                    CitySysNo = district.CitySysNo
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadAreaBySysNo(int sysNo)
        {
            var area = CommonService.LoadAreaBySysNo(sysNo);
            return Json(new { Success = true, Data = area });
        }

        [HttpGet]
        public ActionResult InitAddressArea()
        {
            Task.Factory.StartNew(() =>
            {
                List<Area> list = CommonService.GetAreaList();
                if (list != null)
                {

                    List<Area> provinceList = list.FindAll(item => !item.ProvinceSysNo.HasValue);
                    foreach (var province in provinceList)
                    {
                        //插入province
                        Address provinceAddress = new Address()
                        {
                            Name = province.ProvinceName,
                            CompanySysNo = 1,
                            ParentSysNo = 1,
                            Grade = 0
                        };
                        Service.CompanyService.EditAddress(provinceAddress);
                        List<Area> cityList = list.FindAll(item => (item.ProvinceSysNo.HasValue && item.ProvinceSysNo.Value == province.SysNo && !item.CitySysNo.HasValue));

                        foreach (var city in cityList)
                        {
                            Address cityAddress = new Address()
                            {
                                Name = city.CityName,
                                CompanySysNo = 1,
                                ParentSysNo = provinceAddress.SysNo,
                                Grade = 0
                            };
                            Service.CompanyService.EditAddress(cityAddress);
                            //插入city
                            List<Area> districtList = list.FindAll(item => (item.CitySysNo.HasValue && item.CitySysNo.Value == city.SysNo));

                            foreach (var district in districtList)
                            {
                                Address districtAddress = new Address()
                                {
                                    Name = district.DistrictName,
                                    CompanySysNo = 1,
                                    ParentSysNo = cityAddress.SysNo,
                                    Grade = 0
                                };
                                Service.CompanyService.EditAddress(districtAddress);
                                //插入district
                            }
                        }
                    }
                }
            });
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}