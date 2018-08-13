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
    public class MessageProcessor
    {
        #region 预加载消息
        /// <summary>
        /// 待发送的信息列表
        /// </summary>
        private List<MessageEntity> WaitingToSendMessageList;

        /// <summary>
        /// 预加载消息的频率,单位秒
        /// </summary>
        private int PerLoadMessagesTimerInterval = 30;

        /// <summary>
        /// 预加载定时器：定时从数据库预加载未来一段时间（Timer 执行的频率确定）要发送的信息到待发送信息列表中。
        /// </summary>
        private Timer PreLoadMessgesTimer;

        /// <summary>
        /// 最后一次预加载时间
        /// </summary>
        private DateTime LastPreLoadTime;

        private void IniPreLoadMessagesTimer()
        {
            WaitingToSendMessageList = new List<MessageEntity>();
            if (PreLoadMessgesTimer != null)
            {
                PreLoadMessgesTimer.Stop();
                PreLoadMessgesTimer.Dispose();
            }

            int minseconds = PerLoadMessagesTimerInterval * 1000;
            PreLoadMessgesTimer = new Timer(minseconds);
            PreLoadMessgesTimer.AutoReset = true;
            PreLoadMessgesTimer.Elapsed += PreLoadMessgesTimer_Elapsed;
            PreLoadMessgesTimer.Start();
        }

        private void PreLoadMessgesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LastPreLoadTime = DateTime.Now;
            PreLoadMessages(null);
        }

        /// <summary>
        /// 预加载消息锁
        /// </summary>
        private object PerLoadMessageLocker = new object();

        /// <summary>
        /// 预加载消息
        /// </summary>
        private void PreLoadMessages(List<MessageEntity> newMsgList)
        {
            lock (PerLoadMessageLocker)
            {
                if (newMsgList == null)
                {
                    newMsgList = MessageDA.WillBeSendMessgesBySpanSeconds(null, PerLoadMessagesTimerInterval);
                }
                if (newMsgList == null || newMsgList.Count == 0)
                {
                    // 不在newMsgList列表中的数据表示不需要发送，则将此数据设置为“客户拒收”（注意，这里不能直接删除，可以与发送短信的线程同时操作WaitingToSendMessageList，从而引起错误），在下面发送时会直接排除。
                    for (int i = 0; i < WaitingToSendMessageList.Count; i++)
                    {
                        var msg = WaitingToSendMessageList[i];
                        msg.Status = MessageStatus.CustomerReject;
                    }
                    return;
                };

                for (int i = 0; i < WaitingToSendMessageList.Count; i++)
                {
                    // 不在newMsgList列表中的数据表示不需要发送，则将此数据设置为“客户拒收”（注意，这里不能直接删除，可以与发送短信的线程同时操作WaitingToSendMessageList，从而引起错误），在下面发送时会直接排除。
                    var msg = WaitingToSendMessageList[i];
                    if (!newMsgList.Exists(o => o.SysNo == msg.SysNo))
                    {
                        msg.Status = MessageStatus.CustomerReject;
                    }
                }

                foreach (var nmsg in newMsgList)
                {
                    var omsg = WaitingToSendMessageList.Find(o => o.SysNo == nmsg.SysNo);
                    if (omsg == null)
                    {
                        WaitingToSendMessageList.Add(nmsg);
                    }
                    else
                    {
                        omsg.Status = nmsg.Status;
                    }
                }

            }
        }
        public void PreLoadMessages()
        {
            PreLoadMessages(null);
        }
        public void AddMessages(MessageEntity msg)
        {
            PreLoadMessages(new List<MessageEntity> { msg });
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送消息定时器
        /// </summary>
        private Timer SendMsgTimer;
        /// <summary>
        /// 发送消息检查频率，单位秒
        /// </summary>
        private int SendMsgTimerInterval = 5;
        /// <summary>
        /// 发送消息的任务（线程）数
        /// </summary>
        private int SendingMsgTaskCount = 20;
        /// <summary>
        /// 初始化发送短信的定时器
        /// </summary>
        private void IniSendMsgTimer()
        {

            if (SendMsgTimer != null)
            {
                SendMsgTimer.Stop();
                SendMsgTimer.Dispose();
            }

            int minseconds = SendMsgTimerInterval * 1000;
            SendMsgTimer = new Timer(minseconds);
            SendMsgTimer.AutoReset = true;
            SendMsgTimer.Elapsed += SendMsgTimer_Elapsed;
            SendMsgTimer.Start();
            SendMsgTimer_Elapsed(null, null);
        }

        /// <summary>
        /// 最后一次预发送时间
        /// </summary>
        private DateTime LastSentTime;
        private void SendMsgTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            LastSentTime = DateTime.Now;
            SendMsg();
        }
        /// <summary>
        /// 是否正在发送短信的，ture 表是正在发送。
        /// </summary>
        private bool msgIsSending = false;
        private void SendMsg()
        {
            if (msgIsSending == true)
            {
                return;
            }
            msgIsSending = true;
            try
            {
                if (WaitingToSendMessageList != null && WaitingToSendMessageList.Count > 0)
                {
                    List<Task> SendingMsgTaskList = null;
                    SendingMsgTaskList = SendingMsgTaskList ?? new List<Task>(SendingMsgTaskCount);
                    // 需要马上发送的消息列表
                    List<MessageEntity> sendingMsgList = new List<MessageEntity>(WaitingToSendMessageList.Count / 4);
                    for (int i = WaitingToSendMessageList.Count - 1; i >= 0; i--)
                    {
                        var msg = WaitingToSendMessageList[0];
                        if (msg.Status == MessageStatus.CustomerReject)
                        {
                            WaitingToSendMessageList.Remove(msg);
                            continue;
                        }

                        DateTime sendingTime = msg.LastSendTime.AddSeconds(msg.SendFrequency);
                        // 如果发送时间小于当前时间，则加入马上发送的列表
                        if (sendingTime <= DateTime.Now)
                        {
                            msg.LastSendTime = DateTime.Now;
                            msg.SendCount = msg.SendCount + 1;
                            // 如果达到发送上限则移除等发送列表
                            if (msg.SendCount >= msg.LimitCount)
                            {
                                WaitingToSendMessageList.Remove(msg);
                            }
                            sendingMsgList.Add(msg);
                        }
                    }

                    if (sendingMsgList.Count > 0)
                    {
                        // 计算每个线程发送消息的数量
                        int size = sendingMsgList.Count / SendingMsgTaskCount;
                        if ((sendingMsgList.Count % SendingMsgTaskCount) > 0)
                        {
                            size = size + 1;
                        }
                        // 将马上要发送的消息分配给线程发送
                        for (int i = 0; i < sendingMsgList.Count;)
                        {
                            if ((sendingMsgList.Count - i) < size)
                            {
                                size = sendingMsgList.Count - i;
                            }
                            IEnumerable<MessageEntity> l = sendingMsgList.GetRange(i, size);
                            var t = SendMsg(l);
                            SendingMsgTaskList.Add(t);
                            i = i + size;
                        }

                        Task.WaitAll(SendingMsgTaskList.ToArray());
                        foreach (var t in SendingMsgTaskList)
                        {
                            t.Dispose();
                        }
                    }
                }
            }
            finally
            {
                msgIsSending = false;
            }
        }
        /// <summary>
        /// 异步发送短信
        /// </summary>
        /// <param name="msgList"></param>
        /// <returns></returns>
        private Task SendMsg(IEnumerable<MessageEntity> msgList)
        {
            Task task = new Task(() =>
            {
                foreach (var msg in msgList)
                {
                    try
                    {
                        MessageSenderServer.SendMsg(msg);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog(ex.ToString(), "MessageCenter", "MessageSysNo_" + msg.SysNo.ToString());
                    }
                }
            });
            task.Start();
            return task;
        }
        #endregion

        private MessageProcessor()
        {
        }
        public void Start()
        {
            IniPreLoadMessagesTimer();
            IniSendMsgTimer();
        }

        public void Stop()
        {
            WaitingToSendMessageList = null;
            PreLoadMessgesTimer.Stop();
            SendMsgTimer.Stop();
            PreLoadMessgesTimer.Dispose();
            SendMsgTimer.Dispose();
        }
        /// <summary>
        /// 设置预加载短信频率(默认为120(秒))： 不能小于30秒，当前小于30时，按30计
        /// </summary>
        /// <param name="intervalSeconds">不能小于30(秒)，当前小于30时，按30计</param>
        public void SetPerLoadMessagesTimerInterval(short intervalSeconds = 120)
        {

            if (intervalSeconds <= 30) intervalSeconds = 30;
            if (intervalSeconds == PerLoadMessagesTimerInterval) return;

            PerLoadMessagesTimerInterval = intervalSeconds;
            IniPreLoadMessagesTimer();
        }

        /// <summary>
        /// 设置发送短信监控的频率(默认为5(秒)) 
        /// </summary>
        /// <param name="intervalSeconds">不能等于0(秒)，当前等于0时，按5计</param>
        public void SetSendMsgTimerInterval(byte intervalSeconds = 5)
        {

            if (intervalSeconds <= 0) intervalSeconds = 5;
            if (intervalSeconds == SendMsgTimerInterval) return;
            SendMsgTimerInterval = intervalSeconds;
            IniSendMsgTimer();
        }

        public void RestartIfNotRuning()
        {
            // 如果 超过30秒没有预加载则重新启动预加载;
            if (LastPreLoadTime.AddSeconds(30 + PerLoadMessagesTimerInterval) < DateTime.Now)
            {
                IniPreLoadMessagesTimer();
            }
            // 如果 超过15秒没有发送则重新启动发送;
            if (!msgIsSending && LastSentTime.AddSeconds(15 + SendMsgTimerInterval) < DateTime.Now)
            {
                IniSendMsgTimer();
            }
        }

        private static MessageProcessor processor;

        private static object IniLocker = new object();

        public static MessageProcessor Instance
        {
            get
            {
                if (processor == null)
                {
                    lock (IniLocker)
                    {
                        if (processor == null)
                        {
                            processor = new MessageProcessor();
                        }
                    }
                }
                return processor;
            }
        }
    }
}
