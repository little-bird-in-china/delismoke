using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Template
{
    public abstract class B2CSendVerifyCodeTemplate : BaseMsgTemplate
    {
        public B2CSendVerifyCodeTemplate()
            : base()
        {
            SendCount = 5;
        }
        public override MsgReceiverType ReceiverType
        {
            get
            {
                return MsgReceiverType.Cusmtomer;
            }
        }
        #region 验证码
        [MsgTemplateParameterDescription("签名", 3)]
        public string Sign { get; set; }
        [MsgTemplateParameterDescription("日期", 2)]
        public string DateStr { get; set; }

        [MsgTemplateParameterDescription("验证码", 1)]
        public string VerifyCode { get; set; }

        #endregion

    }
    public class B2CCustomerRegisterSendVerifyCodeTemplate : B2CSendVerifyCodeTemplate
    {
        public override string TemplateCode
        {
            get
            {
                return "B2C_CustomerRegisterSendVerifyCode";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "直销通网站用户注册发送的验证码模板";
            }
        }
    }

    public class B2CCustomerBindPhoneSendVerifyCodeTemplate : B2CSendVerifyCodeTemplate
    {
        public override string TemplateCode
        {
            get
            {
                return "B2C_CustomerBindPhoneSendVerifyCode";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "直销通网站用户手机绑定发送的验证码模板";
            }
        }

    }



    public class B2CCustomerRetrievePasswordSendVerifyCodeTemplate : B2CSendVerifyCodeTemplate
    {
        public override string TemplateCode
        {
            get
            {
                return "B2C_CustomerRetrievePwdSendVerifyCode";
            }
        }
        public override string TemplateName
        {
            get
            {
                return "直销通网站用户找回密码发送的验证码模板";
            }
        }

    }
}
