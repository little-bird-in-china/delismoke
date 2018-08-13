using BlueStone.Smoke.DataAccess;
using BlueStone.Smoke.Entity;
using BlueStone.Smoke.Entity.AuthCenter;
using BlueStone.JsonRpc;
using BlueStone.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlueStone.Smoke.Service
{
    public class SystemUserService
    {
        private static readonly object insertLock = new object();


        public int InsertSystemUser(SystemUser entity)
        {
            //entity.CommonStatus = CommonStatus.Actived;
            CheckSystemUser(entity, true);
            int UserSysNo = 0;
            lock (insertLock)
            {
                if (SystemUserDA.CountLoginName(entity.LoginName, ConstValue.ApplicationID) > 0)
                {
                    throw new BusinessException(string.Format("手机号:{0} 在系统中已存在，请重新输入！", entity.LoginName));
                }
                UserSysNo = SystemUserDA.InsertSystemUser(entity);
            }
            entity.Applications.Clear();
            entity.Applications.Add(new SystemApplication { ApplicationID = ConstValue.ApplicationID });
            //批量插入用户系统对应表
            if (UserSysNo > 0 && entity.Applications != null && entity.Applications.Count() > 0)
            {
                entity.Applications.ForEach(x => SystemUserDA.InsertSystemUser_Application(UserSysNo, x.ApplicationID));
            }

            return UserSysNo;
        }

        /// <summary>
        /// 更新SystemUser信息
        /// </summary>
        [JsonRpcMethod("AuthService.UpdateSystemUser")]
        public int UpdateSystemUser(SystemUser entity, string ApplicationID)
        {
            //if (entity.SysNo == 1)
            //{
            //    throw new BusinessException("超级用户不能修改");
            //}
            CheckSystemUser(entity, false);
            int EditUserSysNo = DataContext.GetContextItemInt("UserSysNo", 0);
            string EditUserName = DataContext.GetContextItemString("UserDisplayName");
            entity.EditUserSysNo = EditUserSysNo;
            entity.EditUserName = EditUserName;
           // lock ("check_app")
            //{
                var hasApp = AuthDA.GetSystemApplicationsByUserSysNo(new int[] { entity.SysNo });
                var roleHasApp = AuthDA.GetSystemApplicationsByUserRole(entity.SysNo);

                var needInsert = entity.Applications.Except(hasApp, new SystemApplicationComparer());
                var needDelete = hasApp.Except(entity.Applications, new SystemApplicationComparer());

                StringBuilder sb = new StringBuilder();
                foreach (var item in needDelete)
                {
                    if (roleHasApp.FirstOrDefault(x => x.ApplicationID == item.ApplicationID) != null)
                    {
                        sb.AppendLine(string.Format("用户还拥有系统{0}中的角色,不能移除所属的系统{0}", item.Name));
                    }
                }
                if (sb.ToString().Length > 0)
                {
                    throw new BusinessException(sb.ToString());
                }
                foreach (var item in needInsert)
                {
                    SystemUserDA.InsertSystemUser_Application(entity.SysNo, item.ApplicationID);
                }
                foreach (var item in needDelete)
                {
                    SystemUserDA.DeleteSystemUser_Application(entity.SysNo, item.ApplicationID);
                }
           // }
            return SystemUserDA.UpdateSystemUser(entity, ApplicationID);
        }

        [JsonRpcMethod("AuthService.LoadSystemUserBySysNo")]
        public SystemUser LoadSystemUserBySysNo(int sysno, string ApplicationID)
        {
            return SystemUserDA.LoadSystemUser(sysno, ApplicationID);
        }

        public int UpdateSystemUser(SystemUser entity)
        {
            //if (entity.SysNo == 1)
            //{
            //    throw new BusinessException("超级用户不能修改");
            //}
            CheckSystemUser(entity, false);

            if (entity.Applications == null)
            {
                entity.Applications = new List<SystemApplication>();
            }
            var hasApp = AuthDA.GetSystemApplicationsByUserSysNo(new int[] { entity.SysNo });
            var roleHasApp = AuthDA.GetSystemApplicationsByUserRole(entity.SysNo);

            var needInsert = entity.Applications.Except(hasApp, new SystemApplicationComparer());
            var needDelete = hasApp.Except(entity.Applications, new SystemApplicationComparer());

            StringBuilder sb = new StringBuilder();
            foreach (var item in needDelete)
            {
                if (roleHasApp.FirstOrDefault(x => x.ApplicationID == item.ApplicationID) != null)
                {
                    sb.AppendLine(string.Format("用户还拥有系统{0}中的角色,不能移除所属的系统{0}", item.Name));
                }
            }
            if (sb.ToString().Length > 0)
            {
                throw new BusinessException(sb.ToString());
            }

            foreach (var item in needInsert)
            {
                SystemUserDA.InsertSystemUser_Application(entity.SysNo, item.ApplicationID);
            }
            foreach (var item in needDelete)
            {
                SystemUserDA.DeleteSystemUser_Application(entity.SysNo, item.ApplicationID);
            }
            return SystemUserDA.UpdateSystemUser(entity);
        }

        /// <summary>
        /// 删除SystemUser信息
        /// </summary>
        public void DeleteSystemUser(int sysNo)
        {
            if (sysNo == 1)
            {
                throw new BusinessException("超级用户不能删除");
            }
            SystemUserDA.DeleteSystemUser(sysNo);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sysNos"></param>
        [JsonRpcMethod("AuthService.DeleteSystemUserBatch")]
        public void DeleteSystemUserBatch(IEnumerable<int> sysNos, string ApplicationID)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的编号");
            }
            if (sysNos.Any(x => x == 1))
            {
                throw new BusinessException("传入的要批量删除的用户中含有编号为1的超级用户,不能执行删除");
            }
            int EditUserSysNo = DataContext.GetContextItemInt("UserSysNo", 0);
            string EditUserName = DataContext.GetContextItemString("UserDisplayName");
            SystemUserDA.DeleteSystemUserBatch(sysNos, ApplicationID, EditUserSysNo, EditUserName);
        }

        public void DeleteSystemUserBatch(IEnumerable<int> sysNos)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的编号");
            }
            if (sysNos.Any(x => x == 1))
            {
                throw new BusinessException("传入的要批量删除的用户中含有编号为1的超级用户,不能执行删除");
            }
            SystemUserDA.DeleteSystemUserBatch(sysNos);
        }

        public void UpdateSystemUserStatusBatch(IEnumerable<int> sysNos, CommonStatus status, CurrentUser current)
        {
            if (sysNos == null || sysNos.Count() == 0)
            {
                throw new BusinessException("请传入要批量操作的编号");
            }
            if (sysNos.Any(x => x == 1))
            {
                throw new BusinessException("传入的要批量操作的用户中含有编号为1的超级用户,不能执行");
            }
            SystemUserDA.UpdateSystemUserStatusBatch(sysNos, status, current);
        }

        /// <summary>
        /// 分页查询SystemUser信息
        /// </summary>
        public QueryResult<SystemUser> QuerySystemUserList(QF_SystemUser filter)
        {
            return SystemUserDA.QuerySystemUserList(filter);
        }

        /// <summary>
        /// 批量查询用户信息
        /// </summary>
        /// <param name="SysNos"></param>
        /// <returns></returns>
        [JsonRpcMethod("AuthService.QuerySystemUserListBySysNos")]
        public List<SystemUser> QuerySystemUserListBySysNos(IEnumerable<int> SysNos, string ApplicationID)
        {
            if (SysNos == null || SysNos.Count() == 0)
            {
                throw new BusinessException("请传入要查询的用户编号的集合!");
            }
            return SystemUserDA.QuerySystemUserListBySysNos(SysNos, ApplicationID);
        }

        /// <summary>
        /// 加载SystemUser信息
        /// </summary>
        public SystemUser LoadSystemUser(int sysNo, string ApplicationID = null)
        {
            return SystemUserDA.LoadSystemUser(sysNo, ApplicationID);
        }

        /// <summary>
        /// 检查SystemUser信息
        /// </summary>
        private static void CheckSystemUser(SystemUser entity, bool isCreate)
        {
            if (!isCreate && entity.SysNo == 0)
            {
                throw new BusinessException(LangHelper.GetText("请传入数据主键！"));
            }
            if (string.IsNullOrWhiteSpace(entity.LoginName))
            {
                throw new BusinessException(LangHelper.GetText("登陆名称不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.LoginName))
            {
                if (entity.LoginName.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("登陆名称长度不能超过50！"));
                }
            }
            //Regex regex = new Regex("^[a-zA-Z][a-zA-Z0-9_]{2,9}$", RegexOptions.IgnoreCase);
            //if (!regex.IsMatch(entity.LoginName))
            //{
            //    throw new BusinessException(LangHelper.GetText("帐号格式不合法,以字母开头,长度三至十位的数字或字母！"));
            //}

            if (isCreate && string.IsNullOrWhiteSpace(entity.LoginPassword))
            {
                throw new BusinessException(LangHelper.GetText("密码不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.LoginPassword))
            {
                if (entity.LoginPassword.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("密码长度不能超过50！"));
                }
            }
            if (string.IsNullOrWhiteSpace(entity.UserFullName))
            {
                throw new BusinessException(LangHelper.GetText("显示名称不能为空！"));
            }
            if (!string.IsNullOrWhiteSpace(entity.UserFullName))
            {
                if (entity.UserFullName.Length > 20)
                {
                    throw new BusinessException(LangHelper.GetText("显示名称不能超过20！"));
                }
            }

            if (!string.IsNullOrWhiteSpace(entity.CellPhone))
            {
                if (entity.CellPhone.Length > 20)
                {
                    throw new BusinessException(LangHelper.GetText("手机号长度不能超过20！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.Email))
            {
                if (entity.Email.Length > 50)
                {
                    throw new BusinessException(LangHelper.GetText("邮箱长度不能超过50！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.QQ))
            {
                if (entity.QQ.Length > 20)
                {
                    throw new BusinessException(LangHelper.GetText("QQ长度不能超过20！"));
                }
            }
            if (!string.IsNullOrWhiteSpace(entity.AvatarImageUrl))
            {
                if (entity.AvatarImageUrl.Length > 200)
                {
                    throw new BusinessException(LangHelper.GetText("头像图片地址长度不能超过200！"));
                }
            }

        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="applicationKey"></param>
        /// <returns></returns>
        [JsonRpcMethod("AuthService.ResetSystemUserPassword")]
        public int ResetSystemUserPassword(string loginName, string oldPassword, string newPassword, string applicationKey)
        {
            var result = AuthDA.Login(loginName, oldPassword, applicationKey);

            if (result == null)
            {
                throw new BusinessException("原密码错误!");
            }

            int EditUserSysNo = DataContext.GetContextItemInt("UserSysNo", 0);
            string EditUserName = DataContext.GetContextItemString("UserDisplayName");

            return SystemUserDA.ResetSystemUserPassword(loginName, newPassword, applicationKey, EditUserSysNo, EditUserName);
        }

        /// <summary>
        /// 批量重置密码
        /// </summary>
        [JsonRpcMethod("AuthService.ResetSystemUserPasswordBatch")]
        public int ResetSystemUserPasswordBatch(Dictionary<int, string> sysno_password, string applicationKey)
        {
            if (sysno_password == null || sysno_password.Count == 0)
            {
                throw new BusinessException("请传入要批量重置的数据!");
            }

            int EditUserSysNo = DataContext.GetContextItemInt("UserSysNo", 0);
            string EditUserName = DataContext.GetContextItemString("UserDisplayName");

            return SystemUserDA.ResetSystemUserPasswordBatch(sysno_password, applicationKey, EditUserSysNo, EditUserName);
        }

        public int ResetSystemUserPasswordForAuthCenter(string loginName, string newPassword, int editUserSysNo, string editUserName)
        {
            return SystemUserDA.ResetSystemUserPassword(loginName, newPassword, string.Empty, editUserSysNo, editUserName);
        }


        [JsonRpcMethod("AuthService.FindPwd")]
        public void FindSystemUserPwd(string loginName, string newPwd, string applicationID,int? mastersysno=null)
        {
            SystemUser user = SystemUserDA.LoadSystemUserByLoginName(loginName, applicationID);
            if (user == null|| (mastersysno.HasValue&& mastersysno.Value>0&& user.MasterSysNo!= mastersysno.Value))
            {
                throw new BusinessException(string.Format("登录账号【{0}】不存在！", loginName));
            }
            SystemUserDA.FindSystemUserPwd(user.SysNo, newPwd);
        }

        public static SystemUser LoadSystemUserByLoginNameAndCellPhone(string loginname, string applicationID)
        {
            return SystemUserDA.LoadSystemUserByLoginName(loginname, applicationID);
        }
    }
}
