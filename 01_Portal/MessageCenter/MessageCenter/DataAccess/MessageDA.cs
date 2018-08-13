using BlueStone.Utility;
using BlueStone.Utility.DataAccess;
using MessageCenter.Entity;
using MessageCenter.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.DataAccess
{
    public class MessageDA
    {
        /// <summary>
        /// 创建Message信息
        /// </summary>
        public static void InsertMessage(ref List<MessageEntity> list)
        {
            foreach (var entity in list)
            {
                DataCommand cmd = new DataCommand("InsertMessage");
                cmd.SetParameter<MessageEntity>(entity);
                entity.SysNo = cmd.ExecuteScalar<int>();
            }
        }
        /// <summary>
        /// 创建Message信息
        /// </summary>
        public static void InsertMessage(ref MessageEntity entity)
        {
            DataCommand cmd = new DataCommand("InsertMessage");
            cmd.SetParameter<MessageEntity>(entity);
            entity.SysNo = cmd.ExecuteScalar<int>();
        }
        /// <summary>
        /// 检查在单位时间间隔内，该号码是否发送过短信
        /// </summary>
        /// <param name="cellPhoneNumber">手机号码</param>
        /// <param name="timeSpanSecond">时间间隔，以秒为单位</param>
        /// <returns></returns>
        public static bool CheckSendSMSTimespan(string cellPhoneNumber, int timeSpanSecond)
        {
            var cmd = new DataCommand("CheckSendSMSTimespan");
            cmd.SetParameter("@CellPhoneNumber", DbType.String, cellPhoneNumber);
            cmd.SetParameter("@TimeSpanSecond", DbType.Int32, timeSpanSecond);
            return cmd.ExecuteScalar<bool>();
        }
        public static List<MessageEntity> LoadMessageReSendCount(int CompanySysNo, string msgReceiver, int msgType = (int)MsgType.SMS)
        {
            var cmd = new DataCommand("LoadMessageReSendCount");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, CompanySysNo);
            cmd.SetParameter("@msgReceiver", DbType.String, msgReceiver);
            cmd.SetParameter("@msgType", DbType.Int32, msgType);
            var result = cmd.ExecuteEntityList<MessageEntity>();
            return result;
        }
        /// <summary>
        /// 一个小时内，重试次数小于5次的短信
        /// </summary>
        /// <param name="maxSmsCount"></param>
        /// <returns></returns>
        public static List<MessageEntity> SelectSMSNotHandledDuringOneHour(int maxSmsCount)
        {
            var cmd = new DataCommand("SelectSMSNotHandledDuringOneHour");
            cmd.CommandText = cmd.CommandText.Replace("#MoreCondition#", string.Empty);
            cmd.SetParameter("@MaxSmsCount", DbType.Int32, maxSmsCount);
            return cmd.ExecuteEntityList<MessageEntity>();
        }

        /// <summary>
        /// 根据短信发送结果，更新短信的状态
        /// </summary>
        /// <param name="sysno"></param>
        /// <param name="handleResult"></param>
        public static void UpdateSmsStatusAfterHandled(int sysno, bool handleResult, bool IsReject = false)
        {
            if (IsReject)
            {
                var cmd = new DataCommand("UpdateSMSToReject");
                cmd.SetParameter("@SysNo", DbType.Int32, sysno);
                cmd.ExecuteNonQuery();
                return;
            }
            if (handleResult)
            {
                var cmd = new DataCommand("UpdateSMSToSuccess");
                cmd.SetParameter("@SysNo", DbType.Int32, sysno);
                cmd.ExecuteNonQuery();
            }
            else
            {
                var cmd = new DataCommand("UpdateSMSToFail");
                cmd.SetParameter("@SysNo", DbType.Int32, sysno);
                cmd.ExecuteNonQuery();
            }
        }

        public static List<MessageEntity> WillBeSendMessgesBySpanSeconds(MsgType? msgType, int seconds)
        {
            var cmd = new DataCommand("WillSendMessgeBySpanSeconds");
            cmd.SetParameter("@MsgType", DbType.Int32, msgType);
            cmd.SetParameter("@SpanSeconds", DbType.Int32, seconds);
            return cmd.ExecuteEntityList<MessageEntity>();
        }

        public static List<QR_Message> LoadMessageByMasterIDAndMasterName(QF_Message filter)
        {
            DataCommand cmd = new DataCommand("LoadMessageByMasterIDAndMasterName");
            cmd.QuerySetCondition("m.MasterID", ConditionOperation.Equal, DbType.String, filter.MasterID);
            cmd.QuerySetCondition("m.MasterName", ConditionOperation.Equal, DbType.String, filter.MasterName);
            cmd.QuerySetCondition("m.InDate", ConditionOperation.LessThanEqual, DbType.DateTime, filter.EndInDate);
            cmd.QuerySetCondition("m.InDate", ConditionOperation.MoreThanEqual, DbType.DateTime, filter.BeginInDate);
            cmd.QuerySetCondition("m.ActionCode", ConditionOperation.Equal, DbType.String, filter.ActionCode);
            cmd.QuerySetCondition("m.MsgReceiver", ConditionOperation.Equal, DbType.String, filter.MsgReceiver);
            cmd.QuerySetCondition("m.MsgType", ConditionOperation.Equal, DbType.Int32, filter.MsgType);
            List<QR_Message> list = cmd.ExecuteEntityList<QR_Message>();
            return list;
        }
        /// <summary>
        /// 最近一条非客户拒收的微信报警消息
        /// </summary>
        /// <param name="MsgReceiver">接收方(openID)</param>
        /// <returns></returns>
        public static MessageEntity LoadLastMessageByMsgReceiver(string MsgReceiver)
        {
            var cmd = new DataCommand("LoadLastMessageByMsgReceiver");
            cmd.SetParameter("@MsgReceiver", DbType.String, MsgReceiver);
            return cmd.ExecuteEntity<MessageEntity>();
        }
    }
}
