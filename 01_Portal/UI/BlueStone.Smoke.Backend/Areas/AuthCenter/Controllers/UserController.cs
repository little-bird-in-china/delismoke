using BlueStone.Smoke.Service;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Models;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BlueStone.Smoke.Backend.Areas.AuthCenter.Controllers
{
    public class UserController : BaseController
    {
        SystemUserService user_service = new SystemUserService();
        RoleService role_service = new RoleService();
        AuthServiceLocal auth_service = new AuthServiceLocal();

        [Auth(FunctionKeys.User_List)]
        public ActionResult List()
        {
            var application = auth_service.GetAllApplication();
            return View(application);
        }

        [Auth(FunctionKeys.User_List)]
        public ActionResult Query()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }

            QF_SystemUser qf = JsonConvert.DeserializeObject<QF_SystemUser>(json);
            qf.ApplicationID = ConstValue.ApplicationID;
            var result = user_service.QuerySystemUserList(qf);
            return AjaxJsonTableData(result);
        }

        [Auth(FunctionKeys.User_SaveSystemUser)]
        public ActionResult SaveSystemUser()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }
            SystemUser user = JsonConvert.DeserializeObject<SystemUser>(json);
            user.Applications = new List<SystemApplication>() { new SystemApplication() { ApplicationID = ConstValue.ApplicationID } };//默认招商系统
            user.EditUserSysNo = CurrUser.UserSysNo;
            user.EditUserName = CurrUser.UserDisplayName;
            user.InDate = DateTime.Now;
            if (user.SysNo > 0)
            {
                user.InUserSysNo = CurrUser.UserSysNo;
                user.InUserName = CurrUser.UserDisplayName;
                user.InDate = DateTime.Now;
                user_service.UpdateSystemUser(user);
            }
            else
            {
                user.CommonStatus = CommonStatus.Actived;
                user.LoginPassword = AuthMgr.EncryptPassword(user.LoginName.Trim());
                user.InUserSysNo = CurrUser.UserSysNo;
                user.InUserName = CurrUser.UserDisplayName;
                user.SysNo = user_service.InsertSystemUser(user);
            }

            return Json(new AjaxResult { Success = true, Message = "保存成功", Data = user }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.User_SaveSystemUser)]
        public ActionResult ActiveSystemUser()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的用户");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            user_service.UpdateSystemUserStatusBatch(sysNos, CommonStatus.Actived, CurrUser);
            return Json(new AjaxResult { Success = true, Message = "启用成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.User_SaveSystemUser)]
        public ActionResult DeActiveSystemUser()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的用户");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            user_service.UpdateSystemUserStatusBatch(sysNos, CommonStatus.DeActived, CurrUser);
            return Json(new AjaxResult { Success = true, Message = "禁用成功" }, JsonRequestBehavior.AllowGet);
        }

        //[Auth(FunctionKeys.User_ResetUserPassword)]
        public ActionResult ResetUserPassword()
        {
            string loginName = Request["data"];
            user_service.ResetSystemUserPasswordForAuthCenter(loginName, AuthMgr.EncryptPassword(loginName), CurrUser.UserSysNo, CurrUser.UserDisplayName);
            return Json(new AjaxResult { Success = true, Message = "重置成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.User_DeleteSystemUser)]
        public ActionResult DeleteSystemUser()
        {
            string json = Request["data"];
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("请选择要启用的用户");
            }
            List<int> sysNos = JsonConvert.DeserializeObject<List<int>>(json);
            user_service.DeleteSystemUserBatch(sysNos);
            return Json(new AjaxResult { Success = true, Message = "删除成功" }, JsonRequestBehavior.AllowGet);
        }

        [Auth(FunctionKeys.User_UserRoleEdit)]
        public ActionResult GetAllRolesByUserSysNo()
        {
            var FilterDefinition = new
            {
                ApplicationID = string.Empty,
                UserSysNo = 0
            };
            string json = Request["data"];

            var Filter = JsonConvert.DeserializeAnonymousType(json, FilterDefinition);

            List<Role> roles = role_service.GetAllRolesByApplicationID(Filter.ApplicationID);
            List<Role> has = role_service.GetAllRolesByUserSysNo(Filter.UserSysNo);

            has.RemoveAll(x => x.ApplicationID != Filter.ApplicationID);

            List<Role> notHas = roles.Except(has, new RoleCompare()).ToList();

            return Json(new
            {
                Success = true,
                Data = new
                {
                    leftData = from s in notHas select new { id = s.SysNo, text = s.RoleName, data = s },
                    rightData = from s in has select new { id = s.SysNo, text = s.RoleName, data = s }
                }
            }, JsonRequestBehavior.AllowGet);
        }

        private class RoleCompare : IEqualityComparer<Role>
        {
            public bool Equals(Role x, Role y)
            {
                return x.SysNo == y.SysNo;
            }

            public int GetHashCode(Role obj)
            {
                return base.GetHashCode();
            }
        }

        [Auth(FunctionKeys.User_UserRoleEdit)]
        public ActionResult UserRoleEdit()
        {
            return View();
        }

        [Auth(FunctionKeys.User_SaveUsersRole)]
        public ActionResult SaveUsersRole()
        {
            string json = Request["data"];
            int UserSysNo = 0;
            int.TryParse(Request["UserSysNo"], out UserSysNo);
            string ApplicationID = ConstValue.ApplicationID;

            if (UserSysNo == 0)
            {
                throw new BusinessException("没有传入有效的用户编号");
            }
            if (UserSysNo == CurrUser.UserSysNo)
            {
                throw new BusinessException("不能修改自己的角色");
            }
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new BusinessException("传入数据不能为空");
            }

            List<Role> roles = JsonConvert.DeserializeObject<List<Role>>(json);

            role_service.SaveUsersRole(UserSysNo, roles, ApplicationID);

            return Json(new AjaxResult { Success = true, Message = "保存成功" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UserEdit()
        {
            int UserSysNo = 0;
            int.TryParse(Request["UserSysNo"], out UserSysNo);

            UserEdit_Model model = new UserEdit_Model();

            if (UserSysNo != 0)
            {
                model.UserInfo = user_service.LoadSystemUser(UserSysNo);
                model.UserRoles = role_service.GetAllRolesByUserSysNo(UserSysNo);
            }
            else
            {
                model.UserInfo.CommonStatus = CommonStatus.Actived;
            }
            model.allApps = AuthServiceLocal.LoadAllSystemApplication();
            return View(model);
        }

    }
}