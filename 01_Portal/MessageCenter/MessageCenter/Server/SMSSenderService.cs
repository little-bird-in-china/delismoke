using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Server
{
    public class SMSSenderService
    {

        public static bool SendSMS(string phone, string templateID, string[] data)
        {
            var api = new CCPRestSDK.CCPRestSDK();

            bool isInit = api.init(ConfigurationManager.AppSettings["sms_restaddress"].ToString(), ConfigurationManager.AppSettings["sms_restpoint"].ToString());
            api.setAccount(ConfigurationManager.AppSettings["sms_account"].ToString(), ConfigurationManager.AppSettings["sms_pwd"].ToString());
            api.setAppId(ConfigurationManager.AppSettings["sms_appid"].ToString());
            if (!isInit)
            {
                throw new BusinessException("短信接口初始化失败!");

            }
            Logger.WriteLog(ConfigurationManager.AppSettings["sms_restaddress"].ToString());
            var retData = api.SendTemplateSMS(phone, templateID, data);
            if (retData["statusCode"].ToString() == "000000")
            {
                return true;
            }
            throw new BusinessException(retData["statusMsg"].ToString()); 

           // return retData["statusCode"].ToString() == "000000";
        }
    }
}
