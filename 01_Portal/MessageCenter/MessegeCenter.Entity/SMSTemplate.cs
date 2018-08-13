using MessageCenter.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessegeCenter.Entity
{
    /*
     * 提供port端展示
     */
    public class SMSTemplate
    {
        public MsgReceiverType MsgReceiverType { get; set; }
        public string SMSTemplateName { get; set; }
        public string SMSTemplateCode { get; set; }
        public Dictionary<string, string> SMSTemplateVariableList { get; set; }
    }
    public class SMSTemplateVariable
    {
        public string VariableName { get; set; }
        public string VariableValue { get; set; }
    }
}
