using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlueStone.Utility;
using BlueStone.Utility.Web;
using BlueStone.Smoke.Msite;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Service;
using System.Text;
using BlueStone.DataAdapter;
using Newtonsoft.Json;
using MessageCenter.Server;
using System.Web.Http;
using MessageCenter.Entity;
using System.Threading.Tasks;
using System.Configuration;
using MessageCenter.Processor;
using MessageCenter.DataAccess;
using System.Net.Http;

namespace BlueStone.Smoke.Msite.Controllers
{
    /// <summary>
    /// 微信相关
    /// </summary>
    public class WXApiController : Controller
    {
        public static string IgnoreMessage= ConfigurationManager.AppSettings["IgnoreMsgStr"];
        public static string ValidateWXMsgToken= ConfigurationManager.AppSettings["ValidateWXMsgToken"];
        #region 微信消息
        /// <summary>
        /// 微信消息
        /// </summary>
        /// <param name="msg"></param>
        [System.Web.Http.HttpPost]
        [System.Web.Http.HttpGet]
        public void ProcessWXMessage()
        {
            Stream reqstream =Request.InputStream;
            reqstream.Seek(0, SeekOrigin.Begin);
            string body = new StreamReader(reqstream).ReadToEnd();
            var echostr = Request["echostr"];
            var signature = Request["signature"];
            var timestamp = Request["timestamp"];
            var nonce = Request["nonce"];
            string resp = "success";
            if (CheckSignature(signature, timestamp, nonce)) {
                if (!string.IsNullOrEmpty(echostr))
                {
                    resp = echostr;
                }
                else
                {
                    WXTextMessage wxresult = null;
                    var result = WechatSenderService.GetWXMsgEntity(body);
                    var textmessage = result is WXTextMessage;
                    if (textmessage)
                    {
                        wxresult = result as WXTextMessage;
                        if (wxresult.Content.Equals(IgnoreMessage, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var lastmsg = SMSProcessor.LoadLastMessageByMsgReceiver(wxresult.FromUserName);
                            if (lastmsg != null)
                            {
                                MessageDA.UpdateSmsStatusAfterHandled(lastmsg.SysNo, true, true);
                                if (wxresult != null)
                                {
                                    WXTextMessage wXText = new WXTextMessage
                                    {
                                        ToUserName = wxresult.FromUserName,
                                        FromUserName = wxresult.ToUserName,
                                        CreateTime = WebPortalHelper.GetTimeStamp(),
                                        Content = "忽略当前报警信息成功!\n正在等待服务器响应!",
                                        MsgId = WebPortalHelper.GetGuidToLongID()
                                    };
                                    resp = WechatSenderService.OpeationWXEntity(wXText);
                                    Logger.WriteLog(resp);
                                }
                            }
                        }
                    }
                }
            }
            Response.Write(resp);
            Response.End();
            Response.Close();
        }
        #endregion



        internal bool CheckSignature(string signature, string timestamp, string nonce)
        {
            if(!string.IsNullOrEmpty(signature)&& !string.IsNullOrEmpty(timestamp) && !string.IsNullOrEmpty(nonce))
            {
                string[] tempArr = { ValidateWXMsgToken, timestamp, nonce };
                Array.Sort(tempArr);
                string tempstr = string.Join("", tempArr);
                tempstr = SecurityHelper.GetSHA1Value(tempstr);
                if (signature.Equals(tempstr))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
