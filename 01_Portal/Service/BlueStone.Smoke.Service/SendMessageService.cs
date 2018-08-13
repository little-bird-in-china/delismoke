using System.Collections.Generic;
using MessageCenter.Server;
using MessageCenter.Template;
using MessageCenter.Entity;
using BlueStone.Smoke.DataAccess;

namespace BlueStone.Smoke.Service
{
    public class SendMessageService
    {
        /// <summary>
        /// 对某一用户发送消息
        /// </summary>
        /// <typeparam name="ParmaterTemplate">消息模板</typeparam>
        /// <param name="parmater">消息模板</param>
        /// <param name="receiverInfo">接收者</param>
        public static void SendMessageOnce<ParmaterTemplate>(ParmaterTemplate parmater, List<ReceiverInfo> receiverInfos) where ParmaterTemplate : BaseMsgTemplate
        {
            SendMessage(parmater, receiverInfos);
        }


        public static void SendMessage<ParmaterTemplate>(ParmaterTemplate parmater, List<ReceiverInfo> receiverInfos) where ParmaterTemplate : BaseMsgTemplate
        {
            //var commonToken = CacheManager.GetWithCache(WechatSenderService.WX_KEY_COMMONTOKEN, () =>
            //{
            //    CacheManager.Remove(WechatSenderService.WX_KEY_COMMONTOKEN);
            //    var wxtoken = WeiXinService.GetCommonToken(null, true);
            //    return wxtoken;
            //}, 7200);
            //parmater.JsApiToken = commonToken.Token;
            MessageSenderServer.SendMsg(0, receiverInfos, parmater, false);
        }




        /// <summary>
        /// 对绑定设备的所有用户发送sms和微信
        /// </summary>
        /// <typeparam name="ParmaterTemplate"></typeparam>
        /// <param name="parmater"></param>
        ///  <param name="serID">设备ID</param>
        public static void SendMessage<ParmaterTemplate>(ParmaterTemplate parmater, string serID) where ParmaterTemplate : BaseMsgTemplate
        {
            List<ReceiverInfo> receiverInfoList = new List<ReceiverInfo>();
            if ((parmater.ReceiverType & MsgReceiverType.WechatUsr) == MsgReceiverType.WechatUsr)
            {
                var list = ClientSmokeDetectorDA.LoadAllUsertSmokeDetectors(serID);
                var IsOffLine = parmater is DevicesOfflineTemplateTemplate;
                var IsWarning = parmater is DevicesWarningTemplateTemplate;
                list.ForEach(e =>
                {
                    receiverInfoList.Add(new ReceiverInfo { ReceiverNo = e.AppCustomerID, MsgType = MsgType.WeiXin });
                    if (IsOffLine)
                    {
                        var cellphone = e.CellPhone ?? e.CellPhone2 ?? e.CellPhone3 ?? null;
                        if (!string.IsNullOrEmpty(cellphone))
                        {
                            receiverInfoList.Add(new ReceiverInfo { ReceiverNo = cellphone, MsgType = MsgType.SMS });
                        }
                    }
                    if (IsWarning)
                    {
                        if (!string.IsNullOrEmpty(e.CellPhone))
                        {
                            receiverInfoList.Add(new ReceiverInfo { ReceiverNo = e.CellPhone, MsgType = MsgType.SMS });
                        }
                        if (!string.IsNullOrEmpty(e.CellPhone2))
                        {
                            receiverInfoList.Add(new ReceiverInfo { ReceiverNo = e.CellPhone2, MsgType = MsgType.SMS });
                        }
                        if (!string.IsNullOrEmpty(e.CellPhone3))
                        {
                            receiverInfoList.Add(new ReceiverInfo { ReceiverNo = e.CellPhone3, MsgType = MsgType.SMS });
                        }
                    }
                });
            }
            if (receiverInfoList.Count == 0)
            {
                return;
            }
            SendMessage(parmater, receiverInfoList);
        }
    }
}
