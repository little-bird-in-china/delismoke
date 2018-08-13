using BlueStone.Smoke.Entity;
using MessegeCenter.Entity;
using System.Collections.Generic;

namespace BlueStone.RPCService.SMS
{
    public class SMSTemplateRPCService
    {
        public List<SMSTemplate> GetSMSTemplateList(CurrentUser user)
        {
            var template = MessageCenter.Server.MessageSenderServer.GetTemplateList(user.MasterSysNo.GetValueOrDefault());
            List<SMSTemplate> list = new List<SMSTemplate>();
            Dictionary<string, string> varList = new Dictionary<string, string>();
            foreach (var item in template)
            {
                SMSTemplate temp = new SMSTemplate();
                temp.SMSTemplateVariableList = new Dictionary<string, string>();
                foreach (var paramItem in item.GetParmaterList())
                {
                    temp.SMSTemplateVariableList.Add("{" + paramItem.Name + "}", paramItem.DisplayName);
                }
                temp.SMSTemplateCode = item.TemplateCode;
                temp.SMSTemplateName = item.TemplateName;
                temp.MsgReceiverType = item.ReceiverType;
                list.Add(temp);
            }
            return list;
        }
        public SMSTemplate LoadMsgTemplate(string ActionCode)
        {
            var data = MessageCenter.Server.MessageSenderServer.GetTemplateByActionCode(ActionCode);
            SMSTemplate template = new SMSTemplate();
            template.SMSTemplateVariableList = new Dictionary<string, string>();
            if (data != null)
            {
                foreach (var paramItem in data.GetParmaterList())
                {
                    template.SMSTemplateVariableList.Add("{" + paramItem.Name + "}", paramItem.DisplayName);
                }
                template.SMSTemplateCode = data.TemplateCode;
                template.SMSTemplateName = data.TemplateName;
                template.MsgReceiverType = data.ReceiverType;
            }
            return template;
        }
    }
}
