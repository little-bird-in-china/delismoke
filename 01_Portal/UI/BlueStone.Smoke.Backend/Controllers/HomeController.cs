using BlueStone.Smoke.Entity;

using BlueStone.Smoke.Service;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using BlueStone.Utility.Web.Auth.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            List<AuthMenuModel> menuItemList = AuthMgr.GetUserMenuList();
            var topLevelMenuList = menuItemList.Where(item => !menuItemList.Exists(tm => tm.SysNo == item.ParentSysNo) && item.IsDisplay == "1").ToList();

            foreach (var mItemL1 in topLevelMenuList)
            {
                if (!string.IsNullOrWhiteSpace(mItemL1.LinkPath))
                {
                    return Redirect(mItemL1.LinkPath);
                }
                List<AuthMenuModel> mItemL2List = menuItemList.FindAll(x => x.ParentSysNo == mItemL1.SysNo && x.IsDisplay == "1");

                if (mItemL2List.Count == 0 && string.IsNullOrWhiteSpace(mItemL1.LinkPath))
                {
                    continue;
                }
                AuthMenuModel menu = mItemL2List.FirstOrDefault(f => f.IsDisplay == "1" && !string.IsNullOrWhiteSpace(f.LinkPath));
                if (menu != null)
                {
                    return Redirect(menu.LinkPath);
                }
            }
            return Content("");
            /*经测试以下语句无效
            if (CurrUser.MasterSysNo.GetValueOrDefault() > 0)
            {

                return Redirect(Url.Action("MapIndex", "Home"));
            }
            else
            {
                return Redirect(Url.Action("Index", "Company"));
            }
            */
        }

        #region 首页地图

        public ActionResult MapIndex()
        {
            if (CurrUser.CompanySysNo == 0)
            {
                return RedirectToAction("Index", "Company");
            }

            return View(CurrUser);
        }

        public ActionResult MapIndexFull()
        {
            //var companySysNo = CurrUser.CompanySysNo;
            return View(CurrUser);
        }


        public JsonResult GetAddressMap(int? addressSysNo = null)
        {
            var companySysNo = CurrUser.CompanySysNo;
            var filter = new AddressFilter { CompanySysNo = companySysNo, PageSize = 1000 };
            if (addressSysNo == null)
            {
                filter.SelectRoot = true;
            }
            else
            {
                filter.ParentSysNo = addressSysNo;
            }

            var addressResult = AddressService.QueryAddressList(filter);
            if (addressResult.data.Count <= 0)
            {
                return Json(new AjaxResult { Success = false });
            }

            var queryResult = AddressMapService.QueryAddressMapList(new AddressMapFilter { AddressSysNos = addressResult.data.Select(a => a.SysNo).ToList() });
            return Json(new AjaxResult { Success = true, Data = queryResult.data });
        }


        public JsonResult GetHomeMarkers(int addressMapSysNo)
        {
            var companySysNo = CurrUser.CompanySysNo;
            var data = AddressMapService.GetHomeMarkers(addressMapSysNo, companySysNo);
            return Json(new AjaxResult { Success = true, Data = data });
        }

        public JsonResult GetShowData(int? addressSysNo = null)
        {
            var companySysNo = CurrUser.CompanySysNo;
            var mapDataService = new MapDataService(companySysNo);
            var result = mapDataService.GetShowModel(addressSysNo);
            return Json(new AjaxResult { Success = true, Data = result });
        }

        #endregion


        #region 修改密码
        public ActionResult EditPwd()
        {
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult SavePwd()
        {
            string OldPassword = Request["OldPassword"];
            string new1 = Request["new1"];
            string new2 = Request["new2"];
            if (new1 != new2)
            {
                throw new BusinessException("您输入的新密码与确认密码不匹配 ");
            }
            string encrptedPassword = AuthMgr.EncryptPassword(OldPassword);
            string encrptednew1 = AuthMgr.EncryptPassword(new1);
            SystemUserService systemUserService = new SystemUserService();
            systemUserService.ResetSystemUserPassword(CurrUser.UserName, encrptedPassword, encrptednew1, AuthMgr.GetApplicationKey());
            //Rpc.Call<int>("AuthService.ResetSystemUserPassword", CurrUser.UserName, encrptedPassword, encrptednew1, AuthMgr.GetApplicationKey());
            AuthMgr.Logout();
            return Json(new AjaxResult { Success = true, Message = "修改成功" }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}