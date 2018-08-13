using BlueStone.Smoke.Service;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class PermissionController : BaseController
    {
        private SysPermissionService permission_service = new SysPermissionService();
        private SysFunctionService function_service = new SysFunctionService();

        #region SysPermission

        [Auth(FunctionKeys.Permission_List)]
        public ActionResult Query()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }
            QF_SysPermission qf = JsonConvert.DeserializeObject<QF_SysPermission>(json);
            var result = permission_service.QuerySysPermissionList(qf);
            return AjaxJsonTableData(result);
        }

        [Auth(FunctionKeys.Permission_SaveSysPermission)]
        public ActionResult SaveSysPermission()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }

            SysPermission permission = JsonConvert.DeserializeObject<SysPermission>(json);
            if (permission.SysNo > 0)
            {
                permission_service.UpdateSysPermission(permission);
            }
            else
            {
                permission_service.InsertSysPermission(permission);
            }

            return Json(new AjaxResult { Success = true, Message = "保存成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Permission_SaveSysPermission)]
        public ActionResult ActiveSysPermission()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的权限");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            permission_service.UpdateSysPermissionStatusBatch(sysNos, CommonStatus.Actived);
            return Json(new AjaxResult { Success = true, Message = "启用成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Permission_SaveSysPermission)]
        public ActionResult DeActiveSysPermission()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的权限");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            permission_service.UpdateSysPermissionStatusBatch(sysNos, CommonStatus.DeActived);
            return Json(new AjaxResult { Success = true, Message = "禁用成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Permission_DeleteSysPermission)]
        public ActionResult DeleteSysPermission()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的权限");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            permission_service.DeleteSysPermissionBatch(sysNos);
            return Json(new AjaxResult { Success = true, Message = "删除成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Permission_List)]
        public ActionResult List()
        {
            return View();
        }

        #endregion

        #region Function
        [Auth(FunctionKeys.Permission_FunctionList)]
        public ActionResult FunctionList()
        {
            return View();
        }

        public ActionResult DynamicLoadFunctions()
        {
            var id = Request["id"];
            int parent = 0;
            int.TryParse(id, out parent);

            var appkey = ConstValue.ApplicationID;

            var result = function_service.DynamicLoadFunctions(parent, appkey);

            return Json(
                from s in result
                select new
                {
                    id = s.SysNo,
                    text = s.FunctionName,
                    parent = s.ParentSysNo == 0 ? "#" : s.ParentSysNo.ToString(),
                    children = s.ChildrenCount > 0,
                    icon = s.ChildrenCount > 0 ? "icon-folder-close-alt" : "icon-leaf",
                    data = s
                }
                , JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Permission_SaveFunction)]
        public ActionResult SaveFunction()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }

            SysFunction menu = JsonConvert.DeserializeObject<SysFunction>(json);
            if (menu.SysNo > 0)
            {
                function_service.UpdateSysFunction(menu);
            }
            else
            {
                function_service.InsertSysFunction(menu);
            }

            return Json(new AjaxResult { Success = true, Message = "保存成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Permission_DeleteFunction)]
        public ActionResult DeleteFunction()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要删除的菜单");
            }
            int sysno = 0;
            int.TryParse(json, out sysno);
            if (sysno == 0)
            {
                throw new BusinessException("传入的菜单编号不是有效编号");
            }
            function_service.DeleteSysFunction(sysno);
            return Json(new AjaxResult { Success = true, Message = "删除成功" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoadFunctions()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);

            List<SysFunction> functions = function_service.LoadAllFunctions(Filter.ApplicationID);
            var result = from f in functions
                         select new
                         {
                             id = f.SysNo,
                             text = f.FunctionName,
                             parent = f.ParentSysNo,
                             data = f
                         };

            return Json(new AjaxResult
            {
                Success = true,
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}