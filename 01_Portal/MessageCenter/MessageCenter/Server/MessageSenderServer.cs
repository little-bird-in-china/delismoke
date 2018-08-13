using BlueStone.Utility;
using MessageCenter.DataAccess;
using MessageCenter.Entity;
using MessageCenter.Processor;
using MessageCenter.Template;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessageCenter.Server
{
    public class MessageSenderServer
    {
        public static int SendMsg<ParmaterTemplate>(int companySysNo, List<ReceiverInfo> receiverList, ParmaterTemplate parmater, bool bl) where ParmaterTemplate : BaseMsgTemplate
        {
            try
            {
                var msgList = GetMessageEntityList(companySysNo, receiverList, parmater, MessageStatus.Sent);
                if (msgList.Count == 0)
                {
                    return 1;//未检测到匹配的消息模版！
                }

                if (parmater.RetryCount == 0)
                {
                    foreach (var m in msgList)
                    {
                        m.Status = MessageStatus.Sent;
                    }
                }
                if (msgList.Any())
                {
                    foreach (var msg in msgList)
                    {
                        //msg.Url = parmater.GetRealUrl();
                        if (msg.MsgType == MsgType.WeiXin)
                        {

                            WechatMessage wechatMsg = new WechatMessage()
                            {
                                ToUser = msg.MsgReceiver,
                                TemplateId = msg.ExternalTemplateID,
                                Url = msg.Url
                            };

                            msg.TemplateParmaters.ForEach(item =>
                            {
                                wechatMsg.Data.Add(item.Name, new WeiXinParam()
                                {
                                    Value = item.Value,
                                    Color = item.Color
                                });
                            });
                            SendMsgToDB(msg, (sysno) =>
                            {
                                try
                                {
                                    WechatSenderService.PushMessage(wechatMsg);
                                    MessageDA.UpdateSmsStatusAfterHandled(sysno, true);
                                    msg.SysNo = sysno;
                                    if (msg.LimitCount > 1)
                                    {
                                        FirstSendSucess(msg);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageDA.UpdateSmsStatusAfterHandled(sysno, false);
                                    throw ex;
                                }
                            });
                        }
                        else if (msg.MsgType == MsgType.SMS)
                        {
                            if (parmater.SendCount > 0)
                            {
                                //从数据库检测同模版发送且发送成功的条数
                                var sendedList = SMSProcessor.LoadMessageReSendCount(companySysNo, msg.MsgReceiver, (int)MsgType.SMS) ?? new List<MessageEntity>();
                                if (sendedList.Any())
                                {
                                    //SendCount < 0 不做限制   modify  by vickytang 
                                    if (parmater.SendCount > 0 && sendedList.Count() >= parmater.SendCount)//在过去24小时内发送短信的次数超过模版限制，不用判断IP，因短信模版自身含有限制
                                    {
                                        return 2;//当前操作在过去24小时内发送短信的次数超过系统限制！
                                    }
                                    //从数据库获取接受者最后一条发送的验证码,1分钟之内不能重复发送
                                    var t = sendedList.OrderByDescending(x => x.SysNo).First();
                                    TimeSpan ts = DateTime.Now - t.InDate;
                                    if (ts.Minutes <= 1)
                                    {
                                        return 3;//当前接受者1分钟之类不能发送超过两条的验证码
                                    }
                                    var clist = sendedList.Where(x => x.ClientIP == msg.ClientIP);//同IP

                                    //SendCount < 0 不做限制   modify  by vickytang
                                    if (parmater.SendCount > 0 && clist.Count() > parmater.SendCount)
                                    {
                                        return 2;//当前操作在过去24小时内发送短信的次数超过系统限制！
                                    }
                                }
                            }
                            ////检测24小时同IP同模版只能发送x条
                            //if (parmater.SendCount > 0 && sendedList.Any())
                            //{
                            //    var clist = sendedList.Where(x => x.ClientIP == msg.ClientIP);//同IP
                            //    if (sendedList.Count() > parmater.SendCount || clist.Count() >= parmater.SendCount)
                            //    {
                            //        return 2;//当前操作在过去24小时内发送短信的次数超过系统限制！
                            //    }
                            //    else
                            //    {
                            //        //从数据库获取最后一条发送的验证码
                            //        var t = sendedList.OrderByDescending(x => x.SysNo).First();
                            //        TimeSpan ts = DateTime.Now - t.InDate;
                            //        if (ts.Minutes <= 1)
                            //        {
                            //            return 3;//当前接受者1分钟之类不能发送超过两条的验证码
                            //        }
                            //    }
                            //}
                            var paras = from p in msg.TemplateParmaters orderby p.Name ascending select p.Value;

                            SendMsgToDB(msg, (sysno) =>
                            {
                                SendSMSImmediately(msg.MsgReceiver, msg.ExternalTemplateID, paras.ToList(), bl, sysno);
                                msg.SysNo = sysno;
                                if (msg.LimitCount > 1)
                                {
                                    FirstSendSucess(msg);
                                }
                            });
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.ToString(), "SMS_EXception");
                throw ex;
            }
        }
        /// <summary>
        /// 返回0表示成功
        /// </summary>
        /// <typeparam name="ParmaterTemplate"></typeparam>
        /// <param name="companySysNo"></param>
        /// <param name="receiverInfo"></param>
        /// <param name="parmater"></param>
        /// <returns></returns>
        public static int SendMsg<ParmaterTemplate>(int companySysNo, ReceiverInfo receiverInfo, ParmaterTemplate parmater, bool bl = true) where ParmaterTemplate : BaseMsgTemplate
        {
            return SendMsg(companySysNo, new List<ReceiverInfo> { receiverInfo }, parmater, bl);
        }
        public static void SendMsg(MessageEntity msg)
        {
            bool result = false;
            try
            {
                if (msg.MsgType == MsgType.SMS)
                {
                    var paras = from p in msg.TemplateParmaters orderby p.Name ascending select p.Value;
                    result = SMSSenderService.SendSMS(msg.MsgReceiver, msg.ExternalTemplateID, paras.ToArray());
                }
                else
                {
                    WechatMessage wechatMsg = new WechatMessage()
                    {
                        ToUser = msg.MsgReceiver,
                        TemplateId = msg.ExternalTemplateID,
                        Url = msg.Url
                    };

                    msg.TemplateParmaters.ForEach(item =>
                    {
                        wechatMsg.Data.Add(item.Name, new WeiXinParam()
                        {
                            Value = item.Value,
                            Color = item.Color
                        });
                    });
                    WechatSenderService.PushMessage(wechatMsg);
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            finally
            {
                if (msg.SysNo > 0)
                {
                    MessageDA.UpdateSmsStatusAfterHandled(msg.SysNo, result);
                }
            }
        }

        private static List<MessageEntity> GetMessageEntityList<ParmaterTemplate>(int companySysNo, List<ReceiverInfo> receiverInfo, ParmaterTemplate parmaterT, MessageStatus status = MessageStatus.Sending, int priority = 0, int retryCount = 0) where ParmaterTemplate : BaseMsgTemplate
        {
            List<MessageEntity> msgList = new List<MessageEntity>();
            if (receiverInfo == null)
            {
                return msgList;
            }
            var templateList = MessageTemplateDA.LoadAsyncMsgTemplateList(companySysNo, parmaterT.TemplateCode);
            if (templateList == null)
            {
                return msgList;
            }

            foreach (var receiverNo in receiverInfo)
            {
                if (string.IsNullOrWhiteSpace(receiverNo.ReceiverNo))
                {
                    continue;
                }
                var e = receiverNo.MsgType;
                var temp = templateList.Find(x => x.MsgType == (int)e);
                if (temp == null)
                {
                    continue;
                }
                parmaterT.Template = temp.TemplateContent;
                msgList.Add(new MessageEntity()
                {
                    ActionCode = parmaterT.TemplateCode,
                    CompanySysNo = companySysNo,
                    MsgReceiver = receiverNo.ReceiverNo,
                    MsgType = e,
                    MsgContent = parmaterT.BuildeMessage(),
                    Status = status,
                    Priority = priority,
                    RetryCount = retryCount,
                    TemplateParmaters = parmaterT.GetParmaterList(e),
                    ExternalTemplateID = temp.ExternalTemplateID,
                    ClientIP = parmaterT.ClientIP,
                    MasterName = parmaterT.MasterName,
                    MasterID = parmaterT.MasterID,
                    LimitCount = temp.LimitCount,
                    SendFrequency = temp.SendFrequency,
                    SendCount = 1,
                    LastSendTime = DateTime.Now,
                    Url = temp.GetRealUrl(parmaterT.MasterID)
                });
            }
            return msgList;
        }
        private static void SendMsgToDB(ref List<MessageEntity> list)
        {
            MessageDA.InsertMessage(ref list);
        }
        private static void SendMsgToDB(MessageEntity entity, Action<int> action)
        {
            MessageDA.InsertMessage(ref entity);
            action(entity.SysNo);
        }
        /// <summary>
        /// 通过数据库推送队列消息
        /// </summary>
        /// <typeparam name="ParmaterTemplate"></typeparam>
        /// <param name="companySysNo"></param>
        /// <param name="actionCode"></param>
        /// <param name="receiverInfo"></param>
        /// <param name="parmaterT"></param>
        /// <returns></returns>
        public static void SendMsgToDB<ParmaterTemplate>(int companySysNo, List<ReceiverInfo> receiverList, ParmaterTemplate parmaterT, int priority = 0, int retryCount = 0) where ParmaterTemplate : BaseMsgTemplate
        {
            var msgList = GetMessageEntityList(companySysNo, receiverList, parmaterT, MessageStatus.Sending, priority, retryCount);
            if (msgList.Count == 0)
            {
                return;
            }
            SendMsgToDB(ref msgList);
        }
        /// <summary>
        /// 获取消息模版列表
        /// </summary>
        /// <param name="companySysNo"></param>
        /// <returns></returns>
        public static List<BaseMsgTemplate> GetTemplateList(int? companySysNo = null)
        {
            List<BaseMsgTemplate> list = new List<BaseMsgTemplate>();
            //list.Add(new B2CCustomerRegisterSendVerifyCodeTemplate());
            //list.Add(new B2CCustomerBindPhoneSendVerifyCodeTemplate());
            //list.Add(new B2CCustomerRetrievePasswordSendVerifyCodeTemplate());
            //list.Add(new OrderAssignedMsgTemplate());
            //list.Add(new OrderOutStorageSendToCustomerMsgTemplate());
            //list.Add(new OrderSplitedMsgTemplate());
            //list.Add(new SplitedOrderAllCanceledMsgTemplate());
            //list.Add(new SplitedOrderPartialCanceledMsgTemplate());
            //list.Add(new SplitedOrderOutStorageMsgTemplate());
            //list.Add(new AdvanceChargePassMsgTemplate());
            //list.Add(new APPUserRegisterSendVerifyCodeMsgTemplate());
            //list.Add(new APPUserBindPhoneSendVerifyCodeTemplate());
            //list.Add(new APPUserRetrievePasswordSendVerifyCodeTemplate());
            //list.Add(new DistributorSendMessageTemplate());
            list.Add(new WechatUserBindDevicesTemplate());
            list.Add(new DevicesOfflineTemplateTemplate());
            list.Add(new DevicesWarningTemplateTemplate());
            return list;
        }
        public static BaseMsgTemplate GetTemplateByActionCode(string code)
        {
            List<BaseMsgTemplate> template1 = null;
            template1 = MessageCenter.Server.MessageSenderServer.GetTemplateList();
            var temp1 = template1.Where(x => x.TemplateCode == code).FirstOrDefault();
            if (temp1 != null)
            {
                return temp1;
            }
            return null;
        }

        private static void SendSMSImmediately(string cellPhoneNumber, string templateID, List<string> paras, bool bl, int smsSysNo = 0)
        {
            if (bl)
            {
                var timeSpanSecond = Convert.ToInt32(ConfigurationManager.AppSettings["TimeSpanSecond"] ?? "30");
                //检验手机号是否合法
                if (!CheckCellPhoneNumber(cellPhoneNumber))
                {
                    throw new BusinessException("手机号码格式不正确！");
                }
                else if (MessageDA.CheckSendSMSTimespan(cellPhoneNumber, timeSpanSecond))
                {
                    throw new BusinessException("验证码发送失败,请在一分钟后重试！");
                }
                else
                {
                    var result = SMSSenderService.SendSMS(cellPhoneNumber, templateID, paras.ToArray());
                    if (result == false)
                    {
                        throw new BusinessException("验证码发送失败！");
                    }
                    if (smsSysNo != 0)
                    {
                        MessageDA.UpdateSmsStatusAfterHandled(smsSysNo, result);
                    }
                }
            }
            else
            {
                var result = SMSSenderService.SendSMS(cellPhoneNumber, templateID, paras.ToArray());
                if (smsSysNo != 0)
                {
                    MessageDA.UpdateSmsStatusAfterHandled(smsSysNo, result);
                }
            }

        }

        /// <summary>
        ///检查手机号码是否合法 
        /// </summary>
        /// <param name="cellPhoneNumer">手机号</param>
        /// <returns></returns>
        public static bool CheckCellPhoneNumber(string cellPhoneNumer)
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


        private static void FirstSendSucess(MessageEntity msg)
        {
            string triggerUrl = ConfigurationManager.AppSettings["SendMessageTriggerUrl"];
            if (string.IsNullOrWhiteSpace(triggerUrl))
            {
                Logger.WriteLog("请配置消息发送触发器的Url,节点名称是SendMessageTriggerUrl，此Url由烟感平台的提供。", "MessageCenter");
            }
            try
            {
                HttpClient.Get<string>(triggerUrl);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.ToString(), "MessageCenter", null, new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("SendMessageTriggerUrl", triggerUrl) });
            }
        }
    }
}
