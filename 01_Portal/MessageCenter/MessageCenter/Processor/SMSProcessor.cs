using BlueStone.Utility;
using MessageCenter.DataAccess;
using MessageCenter.Entity;
using MessageCenter.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace MessageCenter.Processor
{
    public class SMSProcessor
    {
        /// <summary>
        /// 异步发送短信
        /// </summary>
        public static void SendSMSAsync()
        {
            //非休息时间
            if (!IsNowDuringTheBreakTime())
            {
                int maxSmsCount = Convert.ToInt32(ConfigurationManager.AppSettings["TopSelectNumber"] ?? "50");
                var smsList = MessageDA.SelectSMSNotHandledDuringOneHour(maxSmsCount);
                if (smsList != null && smsList.Count > 0)
                {
                    smsList.ForEach(sms =>
                    {
                        var timeSpanSecond = Convert.ToInt32(ConfigurationManager.AppSettings["TimeSpanSecond"] ?? "30");
                        //检验手机号是否合法
                        if (CheckCellPhoneNumber(sms.MsgReceiver) && !MessageDA.CheckSendSMSTimespan(sms.MsgReceiver, timeSpanSecond))
                        {
                            var paras = from p in sms.TemplateParmaters orderby p.Name ascending select p.Value;
                            try
                            {
                                var result = SMSSenderService.SendSMS(sms.MsgReceiver, sms.ExternalTemplateID, paras.ToArray());
                                MessageDA.UpdateSmsStatusAfterHandled(sms.SysNo, result);
                            }
                            catch (Exception ex)
                            {
                                Logger.WriteLog(ex.ToString(), "SMS_Exception");
                                MessageDA.UpdateSmsStatusAfterHandled(sms.SysNo, false);
                            }
                        }
                    });
                }
            }
        }


        /// <summary>
        /// 是否在休息时间，休息时间不发送短信
        /// 需在程序中配置UnSendTimeStart，UnSendTimeEnd
        /// 不配置默认为不休息
        /// </summary>
        /// <returns></returns>
        private static bool IsNowDuringTheBreakTime()
        {
            int UnSendTimeStart = Convert.ToInt32(ConfigurationManager.AppSettings["UnSendTimeStart"] ?? "0");//某时间段内不发送开始时间，例如０代表午夜０点开始不发送短信
            int UnSendTimeEnd = Convert.ToInt32(ConfigurationManager.AppSettings["UnSendTimeEnd"] ?? "0");//某时间段内不发送结束时间，例如７代表７点结束

            if (DateTimeHelper.GetTimeZoneNow().Hour >= UnSendTimeStart && DateTimeHelper.GetTimeZoneNow().Hour < UnSendTimeEnd)//在规定的时间段内不发送短信息
                return true;
            else
                return false;
        }

        /// <summary>
        ///检查手机号码是否合法 
        /// </summary>
        /// <param name="cellPhoneNumer">手机号</param>
        /// <returns></returns>
        private static bool CheckCellPhoneNumber(string cellPhoneNumer)
        {
            Match M = Regex.Match(cellPhoneNumer, "^\\d{11}$");
            if (M.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static List<MessageEntity> LoadMessageReSendCount(int CompanySysNo, string msgReceiver, int msgType)
        {
            return MessageDA.LoadMessageReSendCount(CompanySysNo, msgReceiver, msgType);
        }

        public static List<QR_Message> LoadMessageByMasterIDAndMasterName(QF_Message filter)
        {
            return MessageDA.LoadMessageByMasterIDAndMasterName(filter);
        }
        public static MessageEntity LoadLastMessageByMsgReceiver(string MsgReceiver)
        {
            return MessageDA.LoadLastMessageByMsgReceiver(MsgReceiver);
        }
    }
    
}
