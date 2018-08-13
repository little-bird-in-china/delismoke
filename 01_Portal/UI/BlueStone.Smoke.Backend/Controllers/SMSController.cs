using BlueStone.RPCService.SMS;
using BlueStone.Smoke.Backend.App_Start;
using BlueStone.Smoke.Backend.Controllers;
using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using MessageCenter.Entity;
using MessegeCenter.Entity;
using System.Web.Mvc;

namespace BlueStone.ERP.Controllers
{
    [Auth(FunctionKeys.PM_MsgTemplate_All)]
    public class SMSController : BaseController
    {

        MsgTenantActionRPCService MsgTenantActionRPCService = new MsgTenantActionRPCService();
        MsgTemplateRPCService MsgTemplateRPCService = new MsgTemplateRPCService();
        SMSTemplateRPCService SMSTemplateRPCService = new SMSTemplateRPCService();
        // GET: /SMS/
        #region SMS模版

        public ActionResult SMSTemplateList(string companySysNo)
        {
            CurrentUser currentuser = new CurrentUser();
            int companysysno = -1;
            int.TryParse(companySysNo, out companysysno);
            if (companysysno != -1)
            {
                currentuser.MasterSysNo = 0;

                ViewBag.MsgTenantActionList = MsgTenantActionRPCService.LoadMsgTenantActionList(currentuser);
                // ViewBag.MsgTenantActionList = Rpc.Call<List<SMSTemplate>>("RPCService.MsgTenantActionRPCService.LoadMsgTenantActionList", currentuser);
                ViewBag.companySysNo = companysysno;
            }
            return View();
        }
        public ActionResult Query()
        {
            QF_MsgTemplate filter = BuildQueryFilterEntity<QF_MsgTemplate>();
            QueryResult<MsgTemplate> lists = MsgTemplateRPCService.QueryMsgTemplateList(filter);
            // QueryResult<MsgTemplate> lists = Rpc.Call<QueryResult<MsgTemplate>>("RPCService.MsgTemplateRPCService.QueryMsgTemplateList", filter);
            return AjaxGridJson(lists);
        }
        /// <summary>
        /// 新增/编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Mannager(int? id, int? companysysno)
        {
            MsgTemplate template = new MsgTemplate();
            template.LimitCount = 1;
            template.SendFrequency = 60;
            CurrentUser currentUser = new CurrentUser();
            if (companysysno.HasValue && companysysno.Value == 0)
            {
                currentUser.MasterSysNo = 0;
                ViewBag.MsgTenantActionList = MsgTenantActionRPCService.LoadMsgTenantActionList(currentUser);
                //ViewBag.MsgTenantActionList = Rpc.Call<List<SMSTemplate>>("RPCService.MsgTenantActionRPCService.LoadMsgTenantActionList", currentUser);
                ViewBag.companySysNo = companysysno;
            }
            if (id.HasValue && id.Value > 0)
            {
                template = MsgTemplateRPCService.LoadMsgTemplate(id.Value);
                //template = Rpc.Call<MsgTemplate>("RPCService.MsgTemplateRPCService.LoadMsgTemplate", id);
                ViewBag.MsgTemplateVarList = LoadMsgTemplate(template.ActionCode).SMSTemplateVariableList;
            }
            return View(template);
        }
        /// <summary>
        /// 维护信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Maintain(MsgTemplate info)
        {
            AjaxResult result = new AjaxResult();
            //info.CompanySysNo = CurrUser.CompanySysNo;
            if (info.SysNo > 0)//update
            {
                info.EditUserSysNo = CurrUser.UserSysNo;
                info.EditUserName = CurrUser.UserDisplayName;
                MsgTemplateRPCService.UpdateMsgTemplate(info, CurrUser);
                //Rpc.Call<dynamic>("RPCService.MsgTemplateRPCService.UpdateMsgTemplate", info, CurrUser);
                result = new AjaxResult
                {
                    Success = true,
                    Message = "修改成功！"
                };
                return Json(result);
            }
            //insert
            info.EditUserSysNo = CurrUser.UserSysNo;
            info.EditUserName = CurrUser.UserDisplayName;
            int r = MsgTemplateRPCService.InsertMsgTemplate(info, CurrUser);
            //int r = Rpc.Call<int>("RPCService.MsgTemplateRPCService.InsertMsgTemplate", info, CurrUser);
            result = new AjaxResult
            {
                Success = true,
                Message = "添加成功！",
                Code = r
            };
            return Json(result);
        }
        [HttpPost]
        public JsonResult Delete(string id)
        {
            int sysno = 0;
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out sysno))
            {
                MsgTemplateRPCService.DeleteMsgTemplate(sysno, CurrUser);
                //Rpc.Call<dynamic>("RPCService.MsgTemplateRPCService.DeleteMsgTemplate", sysno, CurrUser);
                return Json(new AjaxResult() { Success = true });
            }
            return Json(new AjaxResult() { Success = false, Message = "传入参数错误." });
        }

        [HttpPost]
        public JsonResult LoadTemplateVar(string actionCode)
        {
            var list = LoadMsgTemplate(actionCode);
            return Json(new AjaxResult() { Success = true, Data = list.SMSTemplateVariableList });
        }
        private SMSTemplate LoadMsgTemplate(string actionCode)
        {
            var list = SMSTemplateRPCService.LoadMsgTemplate(actionCode);
            // var list = Rpc.Call<SMSTemplate>("RPCService.SMSTemplateRPCService.LoadMsgTemplate", actionCode);
            return list;
        }

        public JsonResult SaveMsgTemplateUser(int sysNo)
        {
            MsgTemplate entity = new MsgTemplate();
            entity.SysNo = sysNo;
            entity.EditUserSysNo = CurrUser.UserSysNo;
            entity.EditUserName = CurrUser.UserDisplayName;
            entity.Enabled = true;
            MsgTemplateRPCService.SaveMsgTemplateUser(entity, CurrUser);
            // Rpc.Call<int>("RPCService.MsgTemplateRPCService.SaveMsgTemplateUser", entity, CurrUser);
            return Json(new AjaxResult { Success = true, Message = "保存成功！" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
