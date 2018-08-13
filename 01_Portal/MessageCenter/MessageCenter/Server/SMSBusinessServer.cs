using BlueStone.Utility;
using MessageCenter.DataAccess;
using MessageCenter.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Server
{
    /*
     * RPC调用类
     */
    public class SMSBusinessServer
    {
        /*
         * MessageTemplate
         */
        public static QueryResult<MsgTemplate> QueryMsgTemplateList(QF_MsgTemplate filter)
        {
            return MessageTemplateDA.QueryMsgTemplateList(filter);
        }
        /// <summary>
        /// 获取单个MsgTemplate信息
        /// </summary>
        public static MsgTemplate LoadMsgTemplate(int sysNo)
        {
            return MessageTemplateDA.LoadMsgTemplate(sysNo);
        }


        public static List<MsgTemplate> GetMsgTemplateList(int userSysNo, int companySysNo)
        {
            return MessageTemplateDA.GetMsgTemplateList(userSysNo, companySysNo);
        }

        public static MsgTemplateUser MsgTemplateUserIsExist(int tempSysNo, int userSysNo) {
            return MessageTemplateDA.MsgTemplateUserIsExist(tempSysNo, userSysNo);
        }
        /// <summary>
        /// 修改MsgTemplateUser信息
        /// </summary>
        public static void UpdateMsgTemplateUser(MsgTemplate entity)
        {
            MessageTemplateDA.UpdateMsgTemplateUser(entity);
        }
        public static void InsertMsgTemplateUser(MsgTemplate entity)
        {
            MessageTemplateDA.InsertMsgTemplateUser(entity);
        }
        public static bool DeleteMsgTemplate(int sysNo)
        {
           return MessageTemplateDA.DeleteMsgTemplate(sysNo);
        }

        public static bool UpdateMsgTemplate(MsgTemplate entity)
        {
            return MessageTemplateDA.UpdateMsgTemplate(entity);
        }

        public static bool IsExistMsgTemplate(string ActionCode, int MsgType, int companySysNo)
        {
            return MessageTemplateDA.IsExistMsgTemplate(ActionCode, MsgType, companySysNo);
        }

        public static int InsertMsgTemplate(MsgTemplate entity)
        {
            return MessageTemplateDA.InsertMsgTemplate(entity);
        }
    }
}
