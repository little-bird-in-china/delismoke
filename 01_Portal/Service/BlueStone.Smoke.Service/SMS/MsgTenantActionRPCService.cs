using BlueStone.Smoke.Entity;
using MessegeCenter.Entity;
using System.Collections.Generic;

namespace BlueStone.RPCService.SMS
{
    public class MsgTenantActionRPCService
    {
        public List<SMSTemplate> LoadMsgTenantActionList(CurrentUser user)
        {
            var data = new SMSTemplateRPCService().GetSMSTemplateList(user);// MsgTenantActionProcessor.LoadMsgTenantActionList(TenantID);
            foreach (var item in data)
            {
                item.SMSTemplateName = new SMSTemplateRPCService().LoadMsgTemplate(item.SMSTemplateCode).SMSTemplateName;
            }
            return data;
        }
    }
}
