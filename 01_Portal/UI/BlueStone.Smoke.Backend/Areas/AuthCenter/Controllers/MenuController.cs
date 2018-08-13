using BlueStone.Smoke.Service;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BlueStone.Smoke.Entity;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class MenuController : BaseController
    {
        SysMenuService menu_service = new SysMenuService();

        [Auth(FunctionKeys.Menu_List)]
        public ActionResult List()
        {
            return View();
        }

        public ActionResult DynamicLoadMenus()
        {
            var id = Request["id"];
            int parent = 0;
            int.TryParse(id, out parent);

            var appkey = ConstValue.ApplicationID;

            var result = menu_service.DynamicLoadMenus(parent, appkey);

            return Json(
                from s in result
                select new
                {
                    id = s.SysNo,
                    text = s.MenuName,
                    parent = s.ParentSysNo == 0 ? "#" : s.ParentSysNo.ToString(),
                    children = s.ChildrenCount > 0,
                    icon = s.ChildrenCount > 0 ? "fa fa-sitemap" : "fa fa-leaf",
                    data = s
                }
                , JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisposableLoadMenus()
        {
            var id = Request["id"];
            int parent = 0;
            int.TryParse(id, out parent);

            var result = menu_service.DisposableLoadMenus(parent);
            return Json(
                from s in result
                select new
                {
                    id = s.SysNo,
                    text = s.MenuName,
                    parent = s.ParentSysNo == 0 ? "#" : s.ParentSysNo.ToString(),
                    data = s
                }
                , JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Menu_SaveMenu)]
        public ActionResult SaveMenu()
        {
            string json = "";
            try
            {
                json = Request["data"];
            }
            catch (Exception e)
            {

                return Json(new AjaxResult { Success = false, Message = "含有非法字符，请重新输入!" }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }

            SysMenu menu = JsonConvert.DeserializeObject<SysMenu>(json);
            if (menu.SysNo > 0)
            {
                menu.EditUserSysNo = CurrUser.UserSysNo;
                menu.EditUserName = CurrUser.UserDisplayName;
                menu_service.UpdateSysMenu(menu);
            }
            else
            {
                menu.InUserSysNo = CurrUser.UserSysNo;
                menu.InUserName = CurrUser.UserDisplayName;
                menu_service.InsertSysMenu(menu);
            }

            return Json(new AjaxResult { Success = true, Message = "保存成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Menu_DeleteMenu)]
        public ActionResult DeleteMenu()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要删除的菜单");
            }
            int sysno = 0;
            int.TryParse(json,out sysno);
            if (sysno==0)
            {
                throw new BusinessException("传入的菜单编号不是有效编号");
            }
            menu_service.DeleteSysMenu(sysno);
            return Json(new AjaxResult { Success = true, Data = "删除成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Menu_MenuPermissionEdit)]
        public ActionResult MenuPermissionEdit()
        {
            return View();
        }

        [Auth(FunctionKeys.Menu_MenuPermissionEdit)]
        public ActionResult LoadFunctionsWithPermissionForMenu()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty,
                MenuSysNo = 0
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);
            
            List<SysFunction> functions = SysMenuService.LoadAllFunctionsWithPermission(Filter.ApplicationID);
            List<SysPermission> has = SysMenuService.LoadAllSysPermissionsByMenuSysNo(Filter.MenuSysNo);

            var result = from f in functions select new
            {
                id = f.SysNo,
                text = f.FunctionName,
                parent = f.ParentSysNo,
                data = f,
                parentcheck = false,
                options = from p in f.Permissions select new {
                    id = p.SysNo,
                    text = p.PermissionName,
                    data = p,
                    rootno = f.ParentSysNo,
                    parentno = f.SysNo,
                    Default = has.Exists(m=>m.SysNo==p.SysNo)
                }
            };

            return Json(new AjaxResult { Success = true, Data = result }, JsonRequestBehavior.AllowGet);
        }



        [Auth(FunctionKeys.Menu_SaveMenusPermission)]
        public ActionResult SaveMenusPermission()
        {
            string json = Request["data"];
            int menuSysNo = 0;
           //int permissionSysNo = 0;
            int.TryParse(Request["MenuSysNo"], out menuSysNo);
            //int.TryParse(Request["SysNo"], out permissionSysNo);//TODO 前台需要传SYSNO
            List<SysPermission> list = JsonConvert.DeserializeObject<List<SysPermission>>(json);

            menu_service.SaveMenusPermission(menuSysNo, list);

            return Json(new AjaxResult { Success = true, Message = "保存成功!" }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult LoadMenusWithPermissions()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);

            List<SysMenu> menus = SysMenuService.LoadAllMenusWithPermission(Filter.ApplicationID);
            var result = from m in menus
                         select new
                         {
                             id = m.SysNo,
                             text = m.MenuName,
                             parent = m.ParentSysNo,
                             data = m,
                             options = from p in m.Permissions
                                       select new
                                       {
                                           id = p.SysNo,
                                           text = p.PermissionName,
                                           data = p
                                       }
                         };

            return Json(new AjaxResult { Success = true, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadMenus()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);

            List<SysMenu> menus = SysMenuService.LoadAllMenusWithPermission(Filter.ApplicationID);
            var result = from m in menus
                         select new
                         {
                             id = m.SysNo,
                             text = m.MenuName,
                             parent = m.ParentSysNo,
                             data = m
                         };

            return Json(new AjaxResult
            {
                Success = true,
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadFunctionsWithPermissions()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);

            List<SysFunction> functions = SysMenuService.LoadAllFunctionsWithPermission(Filter.ApplicationID);

            var result = from f in functions
                         select new
                         {
                             id = f.SysNo,
                             text = f.FunctionName,
                             parent = f.ParentSysNo,
                             data = f,
                             options = from p in f.Permissions
                                       select new
                                       {
                                           id = p.SysNo,
                                           text = p.PermissionName,
                                           data = p,
                                           Default = false
                                       }
                         };

            return Json(new AjaxResult { Success = true, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMenusPermission()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要删除的菜单的权限点");
            }
            int sysno = 0;
            int.TryParse(json, out sysno);
            if (sysno == 0)
            {
                throw new BusinessException("传入的菜单的权限点不是有效编号");
            }
            menu_service.DeleteMenusPermission(sysno);
            return Json(new AjaxResult { Success = true, Data = "删除成功" }, JsonRequestBehavior.AllowGet);
        }
    }
}