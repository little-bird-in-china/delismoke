using BlueStone.Smoke.Entity;
using BlueStone.Utility;
using MessageCenter.Entity;
using MessageCenter.Server;
using System.Collections.Generic;

namespace BlueStone.RPCService.SMS
{
    public class MsgTemplateRPCService
    {
        public int InsertMsgTemplate(MsgTemplate entity, CurrentUser user)
        {
            CheckMsgTemplate(entity, true);
            if (SMSBusinessServer.IsExistMsgTemplate(entity.ActionCode, (int)entity.MsgType, entity.CompanySysNo) == true)
            {
                throw new BusinessException(LangHelper.GetText("已存在当前选项的模板！"));
            }
            int sysNo = 0;
            using (ITransaction transaction = TransactionManager.Create())
            {
                var tep = new SMSTemplateRPCService().LoadMsgTemplate(entity.ActionCode);
                entity.ReceiverType = tep.MsgReceiverType;
                entity.TemplateName = tep.SMSTemplateName;
                //entity.CompanySysNo = DataContextHelper.CompanySysNo;

                sysNo = SMSBusinessServer.InsertMsgTemplate(entity);
                //记录操作日志
              //  OperationLogProcessor.WriteLog(BizObjectType.MsgTemplate, BizOperationType.Create, sysNo.ToString(), user, LangHelper.GetText("新增消息模板"), entity);
                transaction.Complete();
            }
            return sysNo;
        }


        /// <summary>
        /// 更新MsgTemplate信息
        /// </summary>
        public bool UpdateMsgTemplate(MsgTemplate entity, CurrentUser user)
        {
            //return MsgTemplateProcessor.UpdateMsgTemplate(entity,user);
            CheckMsgTemplate(entity, false);
            bool result = false;
            using (ITransaction transaction = TransactionManager.Create())
            {
                result = MessageCenter.Server.SMSBusinessServer.UpdateMsgTemplate(entity);
                //记录操作日志
                //OperationLogProcessor.WriteLog(BizObjectType.MsgTemplate, BizOperationType.Update, entity.SysNo.ToString(), user, LangHelper.GetText("修改消息模板"), entity);
                transaction.Complete();
            }
            return result;
        }
        /// <summary>
        /// 检查MsgTemplate信息
        /// </summary>
        private static void CheckMsgTemplate(MsgTemplate entity, bool isCreate)
        {
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if (entity.CompanySysNo < 0)
            {
                throw new BusinessException(LangHelper.GetText("公司id错误！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.TemplateContent))
            {
                if (entity.TemplateContent.Length > 1000)
                {
                    throw new BusinessException(LangHelper.GetText("模板内容长度不能超过1000！"));
                }
            }
        }
        /// <summary>
        /// 删除MsgTemplate信息
        /// </summary>
        public bool DeleteMsgTemplate(int sysNo, CurrentUser user)
        {
            bool result = false;
            using (ITransaction transaction = TransactionManager.Create())
            {
                result = MessageCenter.Server.SMSBusinessServer.DeleteMsgTemplate(sysNo);
                //记录操作日志
                //OperationLogProcessor.WriteLog(BizObjectType.MsgTemplate, BizOperationType.Delete, sysNo.ToString(), user, LangHelper.GetText("删除消息模板"));
                transaction.Complete();
            }
            return result;
        }

        /// <summary>
        /// 分页查询MsgTemplate信息
        /// </summary>
        public QueryResult<MsgTemplate> QueryMsgTemplateList(QF_MsgTemplate filter)
        {
            return MessageCenter.Server.SMSBusinessServer.QueryMsgTemplateList(filter);
        }
        /// <summary>
        /// 加载MsgTemplate信息
        /// </summary>
        public MsgTemplate LoadMsgTemplate(int sysNo)
        {
            return MessageCenter.Server.SMSBusinessServer.LoadMsgTemplate(sysNo);
        }

        /// <summary>
        /// 加载当前用户MsgTemplateList信息
        /// </summary>
        public List<MsgTemplate> GetMsgTemplateList(int userSysNo)
        {
            //int companySysNo = DataContextHelper.CompanySysNo;
            int companySysNo = 0;//TODO ......................
            return MessageCenter.Server.SMSBusinessServer.GetMsgTemplateList(userSysNo, companySysNo);
        }

        public void SaveMsgTemplateUser(MsgTemplate entity, CurrentUser user)
        {
            //检查是否存在
            MsgTemplateUser msgTemplateUser = MessageCenter.Server.SMSBusinessServer.MsgTemplateUserIsExist(entity.SysNo, user.UserSysNo);
            if (msgTemplateUser.SysNo > 0)
            {
                if (msgTemplateUser.Enabled == false)
                {
                    entity.Enabled = true;
                    MessageCenter.Server.SMSBusinessServer.UpdateMsgTemplateUser(entity);
                }
                else
                {
                    entity.Enabled = false;
                    MessageCenter.Server.SMSBusinessServer.UpdateMsgTemplateUser(entity);
                }
            }
            else
            {
                MessageCenter.Server.SMSBusinessServer.InsertMsgTemplateUser(entity);
            }
        }
    }
}
