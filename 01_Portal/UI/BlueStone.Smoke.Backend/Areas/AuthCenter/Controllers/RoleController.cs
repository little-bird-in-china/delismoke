using BlueStone.Smoke.Service;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Models;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class RoleController : BaseController
    {
        RoleService role_sevice = new RoleService();

        [Auth(FunctionKeys.Role_List)]
        public ActionResult List()
        {
            return View();
        }

        [Auth(FunctionKeys.Role_List)]
        public ActionResult Query()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }
            QF_Role qf = JsonConvert.DeserializeObject<QF_Role>(json);
            var result = role_sevice.QueryRoleList(qf);
            return AjaxJsonTableData(result);
        }

        [Auth(FunctionKeys.Role_SaveRole)]
        public ActionResult SaveRole()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }

            Role role = JsonConvert.DeserializeObject<Role>(json);
            role.ApplicationID = ConstValue.ApplicationID;//默认招商系统
            if (role.SysNo > 0)
            {
                role.EditUserSysNo = CurrUser.UserSysNo;
                role.EditUserName = CurrUser.UserDisplayName;
                role_sevice.UpdateRole(role);
            }
            else
            {
                role.CommonStatus = CommonStatus.Actived;
                role.InUserSysNo = CurrUser.UserSysNo;
                role.InUserName = CurrUser.UserDisplayName;
                role.SysNo = role_sevice.InsertRole(role);
            }

            return Json(new AjaxResult { Success = true, Message = "保存成功", Data = role }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Role_SaveRole)]
        public ActionResult ActiveRoles()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的角色");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            role_sevice.UpdateRoleStatusBatch(sysNos, CommonStatus.Actived);
            return Json(new AjaxResult { Success = true, Message = "启用成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Role_SaveRole)]
        public ActionResult DeActiveRoles()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的用户");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            role_sevice.UpdateRoleStatusBatch(sysNos, CommonStatus.DeActived);
            return Json(new AjaxResult { Success = true, Message = "禁用成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Role_DeleteRoles)]
        public ActionResult DeleteRoles()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的用户");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            role_sevice.DeleteRoleBatch(sysNos);
            return Json(new AjaxResult { Success = true, Message = "删除成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Role_RolePermissionEdit)]
        public ActionResult RolePermissionEdit()
        {
            return View();
        }

        [Auth(FunctionKeys.Role_SaveRolesPermission)]
        public ActionResult SaveRolesPermission()
        {
            string json = Request["data"];
            int roleSysNo = 0;
            int.TryParse(Request["RoleSysNo"], out roleSysNo);

            if (roleSysNo == 1 && !AuthMgr.GetUserRoles().Exists(x => x.SysNo == 1))
            {
                throw new BusinessException("只有超级管理员才可以编辑超级管理员角色的权限");
            }

            List<SysPermission> list = JsonConvert.DeserializeObject<List<SysPermission>>(json);

            role_sevice.SaveRolesPermission(roleSysNo, list);

            return Json(new AjaxResult { Success = true, Message = "保存成功!" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.Role_RolePermissionEdit)]
        public ActionResult LoadFunctionsWithPermissionForRole()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty,
                RoleSysNo = 0
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);

            List<SysFunction> functions = SysMenuService.LoadAllFunctionsWithPermission(Filter.ApplicationID);
            List<SysPermission> has = role_sevice.LoadAllSysPermissionsByRoleSysNo(Filter.RoleSysNo);

            var result = from f in functions
                         select new
                         {
                             id = f.SysNo,
                             text = f.FunctionName,
                             parent = f.ParentSysNo,
                             data = f,
                             parentcheck = false,
                             options = from p in f.Permissions
                                       select new
                                       {
                                           id = p.SysNo,
                                           text = p.PermissionName,
                                           data = p,
                                           rootno= f.ParentSysNo,
                                           parentno = f.SysNo,
                                           Default = has.Exists(m => m.SysNo == p.SysNo)
                                       }
                         };

            return Json(new AjaxResult { Success = true, Data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RoleEdit()
        {
            int RoleSysNo = 0;
            int.TryParse(Request["RoleSysNo"], out RoleSysNo);

            RoleEdit_Model model = new RoleEdit_Model();

            if (RoleSysNo != 0)
            {
                model.RoleInfo = role_sevice.LoadRole(RoleSysNo);
            }
            else
            {
                model.RoleInfo.CommonStatus = CommonStatus.Actived;
            }
            model.allApps = AuthServiceLocal.LoadAllSystemApplication();
            return View(model);
        }

    }
}