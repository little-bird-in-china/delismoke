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
    public static class MessageTemplateDA
    {
        public static List<MsgTemplateEntity> LoadAsyncMsgTemplateList(int CompanySysNo, string actionCode)
        {
            DataCommand cmd = new DataCommand("QueryMessageTemplateList");
            cmd.SetParameter("@CompanySysNo", DbType.Int32, CompanySysNo);
            cmd.SetParameter("@ActionCode", DbType.String, actionCode);
            cmd.SetParameter("@Enabled", DbType.Int32, 1);
            var result = cmd.ExecuteEntityList<MsgTemplateEntity>();
            return result;
        }
        /// <summary>
        /// 分页查询MsgTemplate信息
        /// </summary>
        public static QueryResult<MsgTemplate> QueryMsgTemplateList(QF_MsgTemplate filter)
        {
            DataCommand cmd = new DataCommand("QueryMsgTemplateList");
            cmd.QuerySetCondition("m.MsgType", ConditionOperation.Equal, DbType.Int32, filter.MsgType);
            cmd.QuerySetCondition("m.ActionCode", ConditionOperation.Equal, DbType.String, filter.ActionCode);
            cmd.QuerySetCondition("m.CompanySysNo",ConditionOperation.Equal, DbType.Int32, filter.CompanySysNo);
            QueryResult<MsgTemplate> result = cmd.Query<MsgTemplate>(filter, " SysNo DESC");
            return result;
        }
        internal static MsgTemplate LoadMsgTemplate(int sysNo)
        {
            DataCommand cmd = new DataCommand("LoadMsgTemplate");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            MsgTemplate result = cmd.ExecuteEntity<MsgTemplate>();
            return result;
        }
        /// <summary>
        /// 加载当前用户MsgTemplateList信息
        /// </summary>
        public static List<MsgTemplate> GetMsgTemplateList(int userSysNo, int companySysNo)
        {
            DataCommand cmd = new DataCommand("GetMsgTemplateList");
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            cmd.SetParameter("@ReceiverType", DbType.Int32, MsgReceiverType.Manufacturer);
            List<MsgTemplate> result = cmd.ExecuteEntityList<MsgTemplate>();
            return result;
        }
        public static MsgTemplateUser MsgTemplateUserIsExist(int msgTemplateSysNo, int userSysNo)
        {
            DataCommand cmd = new DataCommand("MsgTemplateUserIsExist");
            cmd.SetParameter("@MsgTemplateSysNo", DbType.Int32, msgTemplateSysNo);
            cmd.SetParameter("@UserSysNo", DbType.Int32, userSysNo);
            MsgTemplateUser msgTemplateUser = cmd.ExecuteEntity<MsgTemplateUser>();
            if (msgTemplateUser == null)
            {
                msgTemplateUser = new MsgTemplateUser();
            }
            return msgTemplateUser;
        }
        /// <summary>
        /// 修改MsgTemplateUser信息
        /// </summary>
        public static void UpdateMsgTemplateUser(MsgTemplate entity)
        {
            DataCommand cmd = new DataCommand("UpdateMsgTemplateUser");
            cmd.SetParameter("@MsgTemplateSysNo", DbType.Int32, entity.SysNo);
            cmd.SetParameter("@UserSysNo", DbType.Int32, entity.EditUserSysNo);
            cmd.SetParameter("@EditUserSysNo", DbType.Int32, entity.EditUserSysNo);
            cmd.SetParameter("@EditUserName", DbType.String, entity.EditUserName);
            cmd.SetParameter("@Enabled", DbType.Boolean, entity.Enabled);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 新增MsgTemplateUser信息
        /// </summary>
        public static void InsertMsgTemplateUser(MsgTemplate entity)
        {
            DataCommand cmd = new DataCommand("InsertMsgTemplateUser");
            cmd.SetParameter<MsgTemplate>(entity);
            cmd.ExecuteNonQuery();
        }
        /// <summary>
        /// 删除MsgTemplate信息
        /// </summary>
        public static bool DeleteMsgTemplate(int sysNo)
        {
            DataCommand cmd = new DataCommand("DeleteMsgTemplate");
            cmd.SetParameter("@SysNo", DbType.Int32, sysNo);
            int r = cmd.ExecuteNonQuery();
            return r > 0 ? true : false;
        }
        /// <summary>
        /// 更新MsgTemplate信息
        /// </summary>
        public static bool UpdateMsgTemplate(MsgTemplate entity)
        {
            DataCommand cmd = new DataCommand("UpdateMsgTemplate");
            cmd.SetParameter<MsgTemplate>(entity);
            int r = cmd.ExecuteNonQuery();
            return r > 0 ? true : false;
        }
        public static bool IsExistMsgTemplate(string actionCode, int msgType, int companySysNo)
        {
            DataCommand cmd = new DataCommand("IsExistMsgTemplate");
            cmd.SetParameter("@MsgType", DbType.Int32, msgType);
            cmd.SetParameter("@ActionCode", DbType.String,cmd.SetSafeParameter(actionCode));
            cmd.SetParameter("@CompanySysNo", DbType.Int32, companySysNo);
            MsgTemplate result = cmd.ExecuteEntity<MsgTemplate>();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        internal static int InsertMsgTemplate(MsgTemplate entity)
        {
            DataCommand cmd = new DataCommand("InsertMsgTemplate");
            cmd.SetParameter<MsgTemplate>(entity);
            int result = cmd.ExecuteScalar<int>();
            return result;
        }
    }
}
